namespace MauiLearn.Controls;

public enum TipKind
{
    Tip,
    Warning,
    New,
    Desktop,
    Info,
    Nuance,
}

/// <summary>
/// Cartão de destaque usado nas aulas. Construído 100% em C# (sem XAML) para mostrar
/// que tudo que se faz em XAML também se faz em código.
/// Uso: &lt;TipCard Kind="Warning" Text="Cuidado com..." /&gt;
/// </summary>
[ContentProperty(nameof(Text))]
public class TipCard : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(TipCard), string.Empty,
        propertyChanged: (b, _, n) => ((TipCard)b)._body.Text = System.Text.RegularExpressions.Regex.Replace((string?)n ?? "", @"\s+", " ").Trim());

    public static readonly BindableProperty KindProperty = BindableProperty.Create(
        nameof(Kind), typeof(TipKind), typeof(TipCard), TipKind.Tip,
        propertyChanged: (b, _, _) => ((TipCard)b).ApplyKind());

    public static readonly BindableProperty HeaderProperty = BindableProperty.Create(
        nameof(Header), typeof(string), typeof(TipCard), null,
        propertyChanged: (b, _, _) => ((TipCard)b).ApplyKind());

    private readonly Label _icon = new() { FontSize = 18, VerticalOptions = LayoutOptions.Start };
    private readonly Label _title = new() { FontAttributes = FontAttributes.Bold, FontSize = 13 };
    private readonly Label _body = new() { FontSize = 14, LineHeight = 1.2 };
    private readonly Border _border;

    public TipCard()
    {
        var grid = new Grid
        {
            ColumnDefinitions = [new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star)],
            ColumnSpacing = 10,
        };
        grid.Add(_icon, 0, 0);
        grid.Add(new VerticalStackLayout { Spacing = 2, Children = { _title, _body } }, 1, 0);

        _border = new Border
        {
            Padding = new Thickness(14, 10),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
            Content = grid,
        };
        Content = _border;
        Margin = new Thickness(0, 6);
        ApplyKind();
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public TipKind Kind
    {
        get => (TipKind)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public string? Header
    {
        get => (string?)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    private void ApplyKind()
    {
        var (icon, title, accent) = Kind switch
        {
            TipKind.Warning => ("⚠️", "Atenção", Color.FromArgb("#E59A12")),
            TipKind.New => ("✨", "Novo no .NET MAUI 10/11", Color.FromArgb("#D600AA")),
            TipKind.Desktop => ("🖥️", "Desktop (Windows / macOS)", Color.FromArgb("#2F80ED")),
            TipKind.Info => ("ℹ️", "Saiba mais", Color.FromArgb("#6E6E6E")),
            TipKind.Nuance => ("🔍", "Nuance", Color.FromArgb("#2E9D5B")),
            _ => ("💡", "Dica", Color.FromArgb("#512BD4")),
        };

        _icon.Text = icon;
        _title.Text = Header ?? title;
        _title.TextColor = accent;
        _border.BackgroundColor = accent.WithAlpha(0.12f);
        _border.Stroke = accent;
    }
}
