using MauiLearn.Catalog;
using MauiLearn.Controls;
using MauiLearn.Core;

namespace MauiLearn.Pages;

/// <summary>
/// Recebe o id do componente via rota do Shell: GoToAsync("component?id=Button").
/// IQueryAttributable é a forma "trim-safe" de receber parâmetros (em vez de [QueryProperty]).
/// </summary>
public partial class ComponentDetailPage : AppPage, IQueryAttributable
{
    private readonly ProgressService _progress;
    private readonly EventLogView _eventLog = new() { HeightRequest = 320 };
    private readonly CodeBlock _liveXaml = new() { Title = "XAML gerado pelas suas alterações no inspetor" };
    private ComponentInfo? _info;
    private Page? _openedPage;
    private string _currentTab = "about";
    private int _backgroundMode;

    // Páginas criadas por rota do Shell: mantemos um construtor sem parâmetros e
    // buscamos os serviços no container (ServiceHelper).
    public ComponentDetailPage()
    {
        InitializeComponent();
        _progress = ServiceHelper.Get<ProgressService>();
        Inspector.Edited += (_, _) => _liveXaml.Code = Inspector.GenerateXaml();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var id) && ComponentCatalog.Find(Uri.UnescapeDataString(id?.ToString() ?? "")) is { } info)
            Load(info);
    }

    private void Load(ComponentInfo info)
    {
        _info = info;
        Title = info.Name;
        IconLabel.Text = info.Icon;
        NameLabel.Text = info.Name;
        SummaryLabel.Text = info.Summary;
        BadgeBorder.IsVisible = info.Badge.Length > 0;
        BadgeLabel.Text = info.Badge;
        DocsButton.IsVisible = info.DocsUrl is not null;
        LessonButton.IsVisible = info.LessonId is not null;
        UpdateFavorite();

        if (info.Status != ComponentStatus.Stable)
        {
            StatusCard.IsVisible = true;
            StatusCard.Kind = info.IsObsolete ? TipKind.Warning : TipKind.New;
            StatusCard.Header = info.IsObsolete ? "Obsoleto" : "Novidade";
            StatusCard.Text = info.StatusNote ?? string.Empty;
        }

        BuildTabs();
        CreateDemo();
    }

    // ---------------- Prévia ----------------

    private void CreateDemo()
    {
        if (_info is null) return;

        Inspector.Detach();
        _eventLog.Detach();

        if (_info.DemoFactory is { } factory)
        {
            DemoInstance demo;
            try
            {
                demo = factory();
            }
            catch (Exception ex)
            {
                PreviewHost.Content = new Label { Text = $"Não foi possível criar a demo: {ex.Message}", TextColor = Colors.Red };
                return;
            }
            PreviewHost.Content = demo.Preview;
            Attach(demo.Target);
            HierarchyLabel.Text = BindablePropertyReflector.Hierarchy(demo.Target.GetType());
        }
        else if (_info.PageFactory is not null)
        {
            PreviewHost.Content = PageLauncher();
            var page = _info.PageFactory();
            _openedPage = page;
            Attach(page);
            HierarchyLabel.Text = BindablePropertyReflector.Hierarchy(page.GetType());
        }
    }

    private void Attach(BindableObject target)
    {
        var docs = _info!.KeyProperties.ToDictionary(p => p.Name, p => p.Description);
        Inspector.Inspect(target, docs);
        _eventLog.Attach(target);
        InspectorHint.Text = $"Editando: {target.GetType().Name}. Passe o mouse no nome da propriedade para ver tipo, padrão e descrição.";
    }

    private View PageLauncher()
    {
        var newWindow = new Button { Text = "🪟 Abrir em nova janela" };
        newWindow.Clicked += (_, _) =>
        {
            var page = TakePage();
            Application.Current?.OpenWindow(new Window(page) { Title = page.Title ?? _info!.Name, Width = 800, Height = 560 });
        };

        var modal = new Button { Text = "⬆️ Abrir como modal" };
        modal.Clicked += async (_, _) =>
        {
            var page = TakePage();
            await Navigation.PushModalAsync(page);
        };

        return new VerticalStackLayout
        {
            Spacing = 10,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                new Label { Text = "Páginas não cabem dentro de outra página — abra a demonstração:", HorizontalTextAlignment = TextAlignment.Center },
                new HorizontalStackLayout { Spacing = 10, HorizontalOptions = LayoutOptions.Center, Children = { newWindow, modal } },
                new Label { Text = "Com a página aberta, edite as propriedades dela no inspetor →", Style = (Style)Application.Current!.Resources["Caption"], HorizontalTextAlignment = TextAlignment.Center },
            },
        };
    }

    /// <summary>Uma Page só pode estar em um lugar: se já foi usada, cria outra e reinspeciona.</summary>
    private Page TakePage()
    {
        if (_openedPage is null || _openedPage.Parent is not null || _openedPage.Window is not null)
        {
            _openedPage = _info!.PageFactory!();
            Attach(_openedPage);
        }
        return _openedPage;
    }

    private void OnRecreateClicked(object? sender, EventArgs e) => CreateDemo();

    private void OnBackgroundToggle(object? sender, EventArgs e)
    {
        _backgroundMode = (_backgroundMode + 1) % 3;
        PreviewSurface.Background = _backgroundMode switch
        {
            1 => new SolidColorBrush(Color.FromArgb("#1E1E1E")),
            2 => new LinearGradientBrush([new GradientStop(Color.FromArgb("#FFF3E0"), 0), new GradientStop(Color.FromArgb("#E3F2FD"), 1)], new Point(0, 0), new Point(1, 1)),
            _ => null,
        };
    }

    // ---------------- Abas ----------------

    private void BuildTabs()
    {
        Tabs.Clear();
        AddTab("about", "Sobre");
        AddTab("props", $"Propriedades-chave ({_info!.KeyProperties.Count})");
        AddTab("nuances", $"Nuances ({_info.Nuances.Count})");
        AddTab("code", "Código");
        AddTab("events", "Eventos ao vivo");
        ShowTab(_currentTab);
    }

    private void AddTab(string key, string text)
    {
        var b = new Button { Text = text, FontSize = 13, Padding = new Thickness(12, 6), CommandParameter = key };
        b.Clicked += (_, _) => ShowTab(key);
        Tabs.Add(b);
    }

    private void ShowTab(string key)
    {
        _currentTab = key;
        foreach (var b in Tabs.OfType<Button>())
        {
            var selected = (string)b.CommandParameter == key;
            if (selected) b.ClearValue(StyleProperty);
            else b.Style = (Style)Application.Current!.Resources["SecondaryButton"];
        }

        TabContent.Content = key switch
        {
            "props" => PropsTab(),
            "nuances" => NuancesTab(),
            "code" => CodeTab(),
            "events" => EventsTab(),
            _ => AboutTab(),
        };
    }

    private View AboutTab()
    {
        var info = _info!;
        var stack = new VerticalStackLayout { Spacing = 8 };
        stack.Add(new Label { Text = string.IsNullOrEmpty(info.Description) ? info.Summary : info.Description, Style = Res<Style>("Body") });

        if (info.WindowsView is not null)
        {
            stack.Add(new Label { Text = "Como ele vira um controle nativo (Handler)", Style = Res<Style>("H3") });
            stack.Add(new Label { Text = $"🪟 Windows (WinUI 3):  {info.WindowsView}", Style = Res<Style>("Mono") });
            stack.Add(new Label { Text = $"🍎 macOS (Mac Catalyst):  {info.MacView}", Style = Res<Style>("Mono") });
            stack.Add(new TipCard
            {
                Kind = TipKind.Info,
                Text = $"O {info.Name} que você usa é multiplataforma. Em tempo de execução, um Handler cria o controle nativo acima e mapeia cada propriedade (Mapper). " +
                       "É por isso que a aparência padrão difere entre Windows e macOS.",
            });
        }

        stack.Add(new Label { Text = "Como usar este laboratório", Style = Res<Style>("H3") });
        stack.Add(new Label
        {
            Text = "1. Altere propriedades no inspetor → a prévia muda na hora.\n" +
                   "2. Veja o XAML equivalente na aba Código.\n" +
                   "3. Interaja com a prévia e observe a aba Eventos ao vivo.\n" +
                   "4. Use ↺ para ClearValue — e repare quando o valor volta ao do ESTILO, não ao padrão.",
            Style = Res<Style>("Body"),
        });
        return stack;
    }

    private View PropsTab()
    {
        var stack = new VerticalStackLayout { Spacing = 0 };
        foreach (var p in _info!.KeyProperties)
        {
            var grid = new Grid { ColumnDefinitions = [new ColumnDefinition(new GridLength(260)), new ColumnDefinition(GridLength.Star)], ColumnSpacing = 12, Padding = new Thickness(4, 8) };
            grid.Add(new Label { Text = p.Name, FontAttributes = FontAttributes.Bold, Style = Res<Style>("Mono") }, 0, 0);
            grid.Add(new Label { Text = p.Description, Style = Res<Style>("Body") }, 1, 0);
            stack.Add(grid);
            stack.Add(new BoxView { HeightRequest = 1, Opacity = 0.15 });
        }
        if (_info.Events.Count > 0)
        {
            stack.Add(new Label { Text = "Eventos", Style = Res<Style>("H3") });
            foreach (var ev in _info.Events)
            {
                var grid = new Grid { ColumnDefinitions = [new ColumnDefinition(new GridLength(260)), new ColumnDefinition(GridLength.Star)], ColumnSpacing = 12, Padding = new Thickness(4, 6) };
                grid.Add(new Label { Text = "⚡ " + ev.Name, FontAttributes = FontAttributes.Bold, Style = Res<Style>("Mono") }, 0, 0);
                grid.Add(new Label { Text = ev.Description, Style = Res<Style>("Body") }, 1, 0);
                stack.Add(grid);
            }
        }
        stack.Add(new TipCard { Kind = TipKind.Tip, Text = "Esta é a lista curada das propriedades mais importantes. O inspetor à direita mostra TODAS — inclusive as herdadas de View, VisualElement e Element." });
        return stack;
    }

    private View NuancesTab()
    {
        var stack = new VerticalStackLayout { Spacing = 2 };
        foreach (var n in _info!.Nuances)
        {
            var desktop = n.StartsWith("🖥️ ");
            stack.Add(new TipCard { Kind = desktop ? TipKind.Desktop : TipKind.Nuance, Text = desktop ? n[3..] : n });
        }
        if (_info.Nuances.Count == 0)
            stack.Add(new Label { Text = "Sem nuances registradas.", Opacity = 0.6 });
        return stack;
    }

    private View CodeTab()
    {
        var stack = new VerticalStackLayout { Spacing = 12 };
        _liveXaml.Code = Inspector.GenerateXaml();
        stack.Add(_liveXaml);
        if (!string.IsNullOrWhiteSpace(_info!.Xaml))
            stack.Add(new CodeBlock { Title = "Exemplo em XAML", Language = "xaml", Code = _info.Xaml });
        if (_info.CSharp is not null)
            stack.Add(new CodeBlock { Title = "Exemplo em C#", Language = "csharp", Code = _info.CSharp });
        stack.Add(new TipCard { Kind = TipKind.Tip, Text = "Copie um exemplo para o Playground XAML (menu lateral) e experimente sem recompilar." });
        return stack;
    }

    private View EventsTab()
    {
        var stack = new VerticalStackLayout { Spacing = 8 };
        stack.Add(new Label { Text = "Todos os eventos públicos do objeto inspecionado estão sendo ouvidos por reflexão. Interaja com a prévia!", Style = Res<Style>("Body") });
        stack.Add(new Border { Style = Res<Style>("Card"), Padding = 8, Content = _eventLog });
        return stack;
    }

    // ---------------- Ações ----------------

    private void UpdateFavorite()
    {
        if (_info is null) return;
        FavoriteItem.Text = _progress.IsFavorite(_info.Id) ? "⭐ Favorito" : "☆ Favoritar";
    }

    private void OnFavoriteClicked(object? sender, EventArgs e)
    {
        if (_info is null) return;
        _progress.SetFavorite(_info.Id, !_progress.IsFavorite(_info.Id));
        UpdateFavorite();
    }

    private async void OnDocsClicked(object? sender, EventArgs e)
    {
        if (_info?.DocsUrl is { } url)
            await Launcher.Default.OpenAsync(url);
    }

    private async void OnLessonClicked(object? sender, EventArgs e)
    {
        if (_info?.LessonId is { } id)
            await Shell.Current.GoToAsync($"lesson?id={id}");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Evita vazamentos: desassina eventos ao sair da página (mas mantém se abrimos um modal).
        if (Navigation.ModalStack.Count == 0)
        {
            _eventLog.Detach();
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (Inspector.Target is { } t)
            _eventLog.Attach(t);
    }

    private static T Res<T>(string key) => (T)Application.Current!.Resources[key];
}
