using System.ComponentModel;
using MauiLearn.Core;

namespace MauiLearn.Controls;

/// <summary>
/// Inspetor de propriedades ao vivo: lista TODAS as BindableProperties do objeto-alvo,
/// agrupadas pela classe que as declara, com editores por tipo. Alterações são aplicadas
/// imediatamente (SetValue) e o XAML equivalente é gerado.
///
/// É construído em C# puro — um bom exemplo de UI dinâmica que seria difícil em XAML.
/// </summary>
public class PropertyInspectorView : ContentView
{
    private readonly Entry _search = new() { Placeholder = "Filtrar propriedades…", FontSize = 13 };
    private readonly Picker _scope = new() { FontSize = 13, ItemsSource = new List<string> { "Agrupar por classe", "Ordem alfabética", "Somente alteradas" }, SelectedIndex = 0 };
    private readonly Label _count = new() { FontSize = 11, Opacity = 0.6, VerticalOptions = LayoutOptions.Center };
    private readonly VerticalStackLayout _rows = new() { Spacing = 0 };
    private readonly Dictionary<string, (PropertyEditor? Editor, Label Name, Label Current, InspectableProperty Prop)> _rowMap = [];
    private readonly List<(BindableProperty Property, string Name)> _changed = [];
    private readonly Dictionary<BindableProperty, string> _presetXaml = [];

    private BindableObject? _target;
    private IReadOnlyList<InspectableProperty> _all = [];
    private IReadOnlyDictionary<string, string> _docs = new Dictionary<string, string>();
    private CancellationTokenSource? _debounce;
    private bool _applying;

    public PropertyInspectorView()
    {
        var header = new Grid
        {
            ColumnDefinitions = [new ColumnDefinition(GridLength.Star), new ColumnDefinition(new GridLength(170))],
            ColumnSpacing = 8,
            Padding = new Thickness(12, 10, 12, 6),
        };
        header.Add(_search, 0, 0);
        header.Add(_scope, 1, 0);

        var resetAll = new Button { Text = "↺ Desfazer tudo", FontSize = 11, Padding = new Thickness(8, 2), MinimumHeightRequest = 24 };
        resetAll.SetDynamicResource(StyleProperty, "SecondaryButton");
        resetAll.Clicked += (_, _) => ResetAll();
        ToolTipProperties.SetText(resetAll, "ClearValue em todas as propriedades alteradas pelo inspetor");

        var sub = new Grid { ColumnDefinitions = [new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto)], Padding = new Thickness(12, 0, 12, 6) };
        sub.Add(_count, 0, 0);
        sub.Add(resetAll, 1, 0);

        var root = new Grid
        {
            RowDefinitions = [new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star)],
        };
        root.Add(header, 0, 0);
        root.Add(sub, 0, 1);
        root.Add(new ScrollView { Content = _rows, Padding = new Thickness(0, 0, 6, 0) }, 0, 2);
        Content = root;

        _search.TextChanged += (_, _) => ScheduleRebuild();
        _scope.SelectedIndexChanged += (_, _) => Rebuild();
    }

    /// <summary>Disparado quando o usuário altera uma propriedade (para atualizar o XAML gerado).</summary>
    public event EventHandler? Edited;

    public BindableObject? Target => _target;

    public IReadOnlyList<(BindableProperty Property, string Name)> ChangedProperties => _changed;

    public void Inspect(BindableObject? target, IReadOnlyDictionary<string, string>? docs = null)
    {
        if (_target is not null)
            _target.PropertyChanged -= OnTargetPropertyChanged;

        _target = target;
        _docs = docs ?? new Dictionary<string, string>();
        _changed.Clear();
        _presetXaml.Clear();
        _all = target is null ? [] : BindablePropertyReflector.GetProperties(target);

        if (_target is not null)
            _target.PropertyChanged += OnTargetPropertyChanged;

        Rebuild();
        Edited?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Gera o XAML equivalente às alterações feitas no inspetor.</summary>
    public string GenerateXaml()
    {
        if (_target is null) return string.Empty;
        var tag = _target.GetType().Name;
        if (_changed.Count == 0)
            return $"<!-- Altere propriedades no inspetor para ver o XAML aqui -->\n<{tag} />";

        var simple = new List<string>();
        var complex = new List<string>();
        foreach (var (bp, name) in _changed)
        {
            var value = _target.GetValue(bp);
            if (_presetXaml.TryGetValue(bp, out var px) && px.StartsWith('<'))
                complex.Add($"    <{tag}.{name}>\n        {px}\n    </{tag}.{name}>");
            else if (_presetXaml.TryGetValue(bp, out var px2))
                simple.Add($"{name}=\"{px2}\"");
            else if (value is RowDefinitionCollection or ColumnDefinitionCollection)
                simple.Add($"{name}=\"{PropertyEditorFactory.GridDefinitionsToText(value)}\"");
            else
                simple.Add($"{name}=\"{Escape(BindablePropertyReflector.ToXaml(value))}\"");
        }

        var attrs = simple.Count switch
        {
            0 => string.Empty,
            1 => " " + simple[0],
            _ => "\n    " + string.Join("\n    ", simple),
        };

        return complex.Count == 0
            ? $"<{tag}{attrs} />"
            : $"<{tag}{attrs}>\n{string.Join("\n", complex)}\n</{tag}>";
    }

    private static string Escape(string s) => s.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;");

    private void ScheduleRebuild()
    {
        _debounce?.Cancel();
        _debounce = new CancellationTokenSource();
        var token = _debounce.Token;
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(250), () =>
        {
            if (!token.IsCancellationRequested) Rebuild();
        });
    }

    private void Rebuild()
    {
        _rows.Clear();
        _rowMap.Clear();
        if (_target is null)
        {
            _count.Text = "Nada para inspecionar.";
            return;
        }

        var filter = _search.Text?.Trim() ?? string.Empty;
        IEnumerable<InspectableProperty> props = _all.Where(p => filter.Length == 0 || p.DisplayName.Contains(filter, StringComparison.OrdinalIgnoreCase));

        if (_scope.SelectedIndex == 2)
            props = props.Where(p => _changed.Any(c => c.Property == p.Property));

        var list = props.ToList();
        _count.Text = $"{list.Count} de {_all.Count} propriedades · {_target.GetType().Name}";

        if (_scope.SelectedIndex == 0)
        {
            foreach (var group in list.GroupBy(p => p.IsAttached ? "Propriedades anexadas" : p.DeclaringType.Name))
            {
                _rows.Add(GroupHeader(group.Key, group.Count()));
                foreach (var p in group.OrderBy(p => p.DisplayName)) _rows.Add(CreateRow(p));
            }
        }
        else
        {
            foreach (var p in list.OrderBy(p => p.DisplayName)) _rows.Add(CreateRow(p));
        }
    }

    private static View GroupHeader(string name, int count)
    {
        var label = new Label
        {
            Text = $"{name}  ·  {count}",
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            Padding = new Thickness(12, 10, 12, 4),
        };
        label.SetAppThemeColor(Label.TextColorProperty, Color.FromArgb("#512BD4"), Color.FromArgb("#AC99EA"));
        return label;
    }

    private View CreateRow(InspectableProperty prop)
    {
        var bp = prop.Property;
        var name = new Label { Text = prop.DisplayName, FontSize = 13, LineBreakMode = LineBreakMode.TailTruncation };
        var typeText = BindablePropertyReflector.FriendlyTypeName(bp.ReturnType);
        var defaultText = BindablePropertyReflector.ToXaml(bp.DefaultValue);
        var meta = new Label { Text = typeText, FontSize = 10, Opacity = 0.55, LineBreakMode = LineBreakMode.TailTruncation };
        var current = new Label { FontSize = 12, Opacity = 0.8, LineBreakMode = LineBreakMode.TailTruncation, VerticalOptions = LayoutOptions.Center };

        var doc = _docs.FirstOrDefault(d => d.Key.Split([' ', '/', '('], StringSplitOptions.RemoveEmptyEntries).Contains(prop.DisplayName)).Value;
        ToolTipProperties.SetText(name,
            $"{prop.DisplayName} ({typeText})\nDeclarada em: {bp.DeclaringType.Name}\nPadrão: {defaultText}\nModo de binding padrão: {bp.DefaultBindingMode}" +
            (doc is null ? string.Empty : $"\n\n{doc}"));

        var readOnly = bp.IsReadOnly || BindablePropertyReflector.ForceReadOnly.Contains(bp.PropertyName);
        PropertyEditor? editor = readOnly ? null : PropertyEditorFactory.Create(prop, _target!, (v, xaml) => Apply(prop, v, xaml));

        var reset = new Button
        {
            Text = "↺",
            FontSize = 12,
            Padding = 0,
            WidthRequest = 28,
            HeightRequest = 28,
            MinimumWidthRequest = 28,
            MinimumHeightRequest = 28,
            IsVisible = editor is not null,
            VerticalOptions = LayoutOptions.Center,
        };
        reset.SetDynamicResource(StyleProperty, "SecondaryButton");
        ToolTipProperties.SetText(reset, "ClearValue: volta ao valor do estilo/padrão");
        reset.Clicked += (_, _) => Reset(prop);

        var grid = new Grid
        {
            ColumnDefinitions = [new ColumnDefinition(new GridLength(150)), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto)],
            ColumnSpacing = 8,
            Padding = new Thickness(12, 4),
            MinimumHeightRequest = 40,
        };
        grid.Add(new VerticalStackLayout { Spacing = 0, VerticalOptions = LayoutOptions.Center, Children = { name, meta } }, 0, 0);
        grid.Add(editor?.View ?? current, 1, 0);
        grid.Add(reset, 2, 0);

        // Destaque ao passar o mouse (VisualState PointerOver criado em código).
        VisualStateManager.SetVisualStateGroups(grid,
        [
            new VisualStateGroup
            {
                Name = "CommonStates",
                States =
                {
                    new VisualState { Name = "Normal" },
                    new VisualState { Name = "PointerOver", Setters = { new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromArgb("#14512BD4") } } },
                },
            },
        ]);

        _rowMap[prop.DisplayName] = (editor, name, current, prop);
        RefreshRow(prop.DisplayName);
        return grid;
    }

    private void RefreshRow(string key)
    {
        if (_target is null || !_rowMap.TryGetValue(key, out var row)) return;
        object? value;
        try { value = _target.GetValue(row.Prop.Property); }
        catch { value = null; }

        if (row.Editor is not null) row.Editor.Refresh(value);
        else row.Current.Text = value is null ? "(nulo)" : DescribeReadOnly(value);

        var changed = _changed.Any(c => c.Property == row.Prop.Property);
        row.Name.FontAttributes = changed ? FontAttributes.Bold : FontAttributes.None;
        if (changed) row.Name.TextColor = Color.FromArgb("#D600AA");
        else row.Name.ClearValue(Label.TextColorProperty);
    }

    private static string DescribeReadOnly(object value) => value switch
    {
        System.Collections.ICollection c => $"{value.GetType().Name} ({c.Count} itens)",
        Element e => e.GetType().Name,
        _ => PropertyEditorFactory.Describe(value) is var d && d != value.GetType().Name ? d : BindablePropertyReflector.ToXaml(value),
    };

    private async void Apply(InspectableProperty prop, object? value, string? presetXaml)
    {
        if (_target is null) return;
        try
        {
            _applying = true;
            _target.SetValue(prop.Property, value);
            if (presetXaml is null) _presetXaml.Remove(prop.Property);
            else _presetXaml[prop.Property] = presetXaml;
            if (!_changed.Any(c => c.Property == prop.Property))
                _changed.Add((prop.Property, prop.DisplayName));
        }
        catch (Exception ex)
        {
            // Algumas combinações são inválidas (ex.: Minimum > Maximum no Slider).
            if (Window?.Page is { } page)
                await page.DisplayAlertAsync("Valor rejeitado", $"{prop.DisplayName}: {ex.Message}", "Entendi");
        }
        finally
        {
            _applying = false;
        }
        RefreshRow(prop.DisplayName);
        Edited?.Invoke(this, EventArgs.Empty);
    }

    private void Reset(InspectableProperty prop)
    {
        if (_target is null) return;
        try { _target.ClearValue(prop.Property); }
        catch { /* propriedades somente leitura */ }
        _changed.RemoveAll(c => c.Property == prop.Property);
        _presetXaml.Remove(prop.Property);
        RefreshRow(prop.DisplayName);
        Edited?.Invoke(this, EventArgs.Empty);
    }

    private void ResetAll()
    {
        foreach (var key in _changed.Select(c => c.Name).ToList())
            if (_rowMap.TryGetValue(key, out var row)) Reset(row.Prop);
        _changed.Clear();
        Rebuild();
        Edited?.Invoke(this, EventArgs.Empty);
    }

    private void OnTargetPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Mantém o inspetor em sincronia quando a prévia muda sozinha (ex.: arrastar um Slider).
        if (_applying || e.PropertyName is null) return;
        if (_rowMap.ContainsKey(e.PropertyName))
            MainThread.BeginInvokeOnMainThread(() => RefreshRow(e.PropertyName));
    }

    public void Detach()
    {
        if (_target is not null)
            _target.PropertyChanged -= OnTargetPropertyChanged;
    }
}
