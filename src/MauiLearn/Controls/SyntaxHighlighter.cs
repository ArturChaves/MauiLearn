using System.Text.RegularExpressions;

namespace MauiLearn.Controls;

/// <summary>
/// Realce de sintaxe minimalista para XAML e C#, gerando um FormattedString
/// (uma Label com vários Spans coloridos). Demonstra Span/FormattedString na prática.
/// </summary>
public static partial class SyntaxHighlighter
{
    private static readonly HashSet<string> CSharpKeywords =
    [
        "public", "private", "protected", "internal", "static", "readonly", "class", "record", "struct",
        "interface", "enum", "namespace", "using", "new", "return", "if", "else", "for", "foreach", "while",
        "var", "void", "string", "int", "double", "bool", "object", "true", "false", "null", "async", "await",
        "this", "base", "override", "virtual", "abstract", "partial", "get", "set", "init", "in", "is", "not",
        "switch", "case", "break", "default", "throw", "try", "catch", "finally", "event", "const", "out", "ref",
        "nameof", "typeof", "sealed", "float", "long", "char", "byte", "decimal", "yield", "where", "value",
    ];

    [GeneratedRegex("""(<!--[\s\S]*?-->)|(</?)([\w:.]+)|([\w:.]+)(=)|("[^"]*")|(/?>)""")]
    private static partial Regex XamlRegex();

    [GeneratedRegex("""(//[^\n]*)|(/\*[\s\S]*?\*/)|(@?\$?"(?:[^"\\\n]|\\.)*")|('(?:[^'\\]|\\.)')|(\b\d+(?:\.\d+)?[fdmL]?\b)|(\b[A-Za-z_]\w*\b)""")]
    private static partial Regex CSharpRegex();

    public static FormattedString Highlight(string code, string language, bool dark)
    {
        var palette = new Palette(dark);
        if (language.Equals("text", StringComparison.OrdinalIgnoreCase))
        {
            var plain = new FormattedString();
            Add(plain, code, palette.Text);
            return plain;
        }
        return language.Equals("csharp", StringComparison.OrdinalIgnoreCase) || language == "C#"
            ? HighlightCSharp(code, palette)
            : HighlightXaml(code, palette);
    }

    private static FormattedString HighlightXaml(string code, Palette p)
    {
        var fs = new FormattedString();
        var last = 0;
        foreach (Match m in XamlRegex().Matches(code))
        {
            Add(fs, code[last..m.Index], p.Text);
            if (m.Groups[1].Success) Add(fs, m.Value, p.Comment);
            else if (m.Groups[2].Success) { Add(fs, m.Groups[2].Value, p.Punct); Add(fs, m.Groups[3].Value, p.Tag); }
            else if (m.Groups[4].Success) { Add(fs, m.Groups[4].Value, p.Attr); Add(fs, m.Groups[5].Value, p.Punct); }
            else if (m.Groups[6].Success) Add(fs, m.Value, m.Value.Contains('{') ? p.Markup : p.String);
            else Add(fs, m.Value, p.Punct);
            last = m.Index + m.Length;
        }
        Add(fs, code[last..], p.Text);
        return fs;
    }

    private static FormattedString HighlightCSharp(string code, Palette p)
    {
        var fs = new FormattedString();
        var last = 0;
        foreach (Match m in CSharpRegex().Matches(code))
        {
            Add(fs, code[last..m.Index], p.Text);
            Color color =
                m.Groups[1].Success || m.Groups[2].Success ? p.Comment :
                m.Groups[3].Success || m.Groups[4].Success ? p.String :
                m.Groups[5].Success ? p.Number :
                CSharpKeywords.Contains(m.Value) ? p.Keyword :
                char.IsUpper(m.Value[0]) ? p.Type : p.Text;
            Add(fs, m.Value, color);
            last = m.Index + m.Length;
        }
        Add(fs, code[last..], p.Text);
        return fs;
    }

    private static void Add(FormattedString fs, string text, Color color)
    {
        if (text.Length > 0)
            fs.Spans.Add(new Span { Text = text, TextColor = color });
    }

    private sealed class Palette(bool dark)
    {
        public Color Text { get; } = dark ? Color.FromArgb("#D4D4D4") : Color.FromArgb("#1F1F1F");
        public Color Comment { get; } = dark ? Color.FromArgb("#6A9955") : Color.FromArgb("#008000");
        public Color Tag { get; } = dark ? Color.FromArgb("#569CD6") : Color.FromArgb("#800000");
        public Color Attr { get; } = dark ? Color.FromArgb("#9CDCFE") : Color.FromArgb("#E50000");
        public Color String { get; } = dark ? Color.FromArgb("#CE9178") : Color.FromArgb("#0000FF");
        public Color Markup { get; } = dark ? Color.FromArgb("#DCDCAA") : Color.FromArgb("#7A3E9D");
        public Color Punct { get; } = dark ? Color.FromArgb("#808080") : Color.FromArgb("#800000");
        public Color Keyword { get; } = dark ? Color.FromArgb("#569CD6") : Color.FromArgb("#0000FF");
        public Color Type { get; } = dark ? Color.FromArgb("#4EC9B0") : Color.FromArgb("#267F99");
        public Color Number { get; } = dark ? Color.FromArgb("#B5CEA8") : Color.FromArgb("#098658");
    }
}
