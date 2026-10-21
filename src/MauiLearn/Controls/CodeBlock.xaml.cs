namespace MauiLearn.Controls;

[ContentProperty(nameof(Code))]
public partial class CodeBlock : ContentView
{
    // Uma BindableProperty = "propriedade com superpoderes": suporta binding, estilos,
    // animação, valor padrão e callback de mudança (propertyChanged).
    public static readonly BindableProperty CodeProperty = BindableProperty.Create(
        nameof(Code), typeof(string), typeof(CodeBlock), string.Empty,
        propertyChanged: (b, _, _) => ((CodeBlock)b).Render());

    public static readonly BindableProperty LanguageProperty = BindableProperty.Create(
        nameof(Language), typeof(string), typeof(CodeBlock), "xaml",
        propertyChanged: (b, _, _) => ((CodeBlock)b).Render());

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), typeof(string), typeof(CodeBlock), null);

    public CodeBlock()
    {
        InitializeComponent();
        if (Application.Current is { } app)
            app.RequestedThemeChanged += (_, _) => Render();
    }

    public string Code
    {
        get => (string)GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }

    public string Language
    {
        get => (string)GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    public string? Title
    {
        get => (string?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    private string DisplayTitle => Title ?? Language.ToLowerInvariant() switch
    {
        "csharp" => "C#",
        "text" => "Texto",
        _ => "XAML",
    };

    private void Render()
    {
        if (Title is null)
            SetValue(TitleProperty, DisplayTitle);

        var code = Dedent(Code ?? string.Empty);
        var dark = Application.Current?.RequestedTheme == AppTheme.Dark;
        CodeLabel.FormattedText = SyntaxHighlighter.Highlight(code, Language, dark);
    }

    private async void OnCopyClicked(object? sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(Dedent(Code ?? string.Empty));
        CopyButton.Text = "Copiado ✓";
        await Task.Delay(1200);
        CopyButton.Text = "Copiar";
    }

    /// <summary>Remove a indentação comum (útil quando o código vem indentado dentro do XAML).</summary>
    public static string Dedent(string text)
    {
        var lines = text.Replace("\r\n", "\n").Split('\n').ToList();
        while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[0])) lines.RemoveAt(0);
        while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[^1])) lines.RemoveAt(lines.Count - 1);
        var indent = lines.Where(l => l.Trim().Length > 0)
                          .Select(l => l.Length - l.TrimStart().Length)
                          .DefaultIfEmpty(0).Min();
        return string.Join('\n', lines.Select(l => l.Length >= indent ? l[indent..] : l.TrimStart()));
    }
}
