namespace MauiLearn.Catalog;

/// <summary>
/// Registro de todos os componentes. Cada categoria vive num arquivo parcial
/// (ComponentCatalog.Layouts.cs, ComponentCatalog.Input.cs, ...).
/// </summary>
public static partial class ComponentCatalog
{
    private static List<ComponentInfo>? _all;

    public static IReadOnlyList<ComponentInfo> All => _all ??= Build();

    public static ComponentInfo? Find(string? id) =>
        All.FirstOrDefault(c => string.Equals(c.Id, id, StringComparison.OrdinalIgnoreCase));

    public static string CategoryName(ComponentCategory c) => c switch
    {
        ComponentCategory.Layouts => "Layouts",
        ComponentCategory.Text => "Texto e exibição",
        ComponentCategory.Input => "Entrada de dados",
        ComponentCategory.Commands => "Botões e comandos",
        ComponentCategory.Collections => "Coleções",
        ComponentCategory.Graphics => "Formas, pincéis e gráficos",
        ComponentCategory.Gestures => "Gestos",
        ComponentCategory.Pages => "Páginas e navegação",
        ComponentCategory.Desktop => "Recursos de desktop",
        _ => c.ToString(),
    };

    public static string CategoryIcon(ComponentCategory c) => c switch
    {
        ComponentCategory.Layouts => "📐",
        ComponentCategory.Text => "🔤",
        ComponentCategory.Input => "⌨️",
        ComponentCategory.Commands => "🔘",
        ComponentCategory.Collections => "🗂️",
        ComponentCategory.Graphics => "🎨",
        ComponentCategory.Gestures => "👆",
        ComponentCategory.Pages => "📄",
        ComponentCategory.Desktop => "🖥️",
        _ => "•",
    };

    private static List<ComponentInfo> Build()
    {
        var list = new List<ComponentInfo>();
        AddLayouts(list);
        AddText(list);
        AddInput(list);
        AddCommands(list);
        AddCollections(list);
        AddGraphics(list);
        AddGestures(list);
        AddPages(list);
        AddDesktop(list);
        return list;
    }

    /// <summary>Atalho para criar uma entrada do catálogo.</summary>
    private static ComponentInfo C(List<ComponentInfo> list, string name, ComponentCategory cat, string icon, string summary)
    {
        var info = new ComponentInfo(name, cat, icon, summary);
        list.Add(info);
        return info;
    }

    // ---------- Helpers para montar demos ----------

    internal static Label Caption(string text) => new()
    {
        Text = text,
        FontSize = 12,
        Opacity = 0.7,
        HorizontalTextAlignment = TextAlignment.Center,
    };

    internal static BoxView Box(string color, double size = 50) => new()
    {
        Color = Color.FromArgb(color),
        WidthRequest = size,
        HeightRequest = size,
        CornerRadius = 6,
    };

    internal static readonly string[] Palette = ["#512BD4", "#D600AA", "#2E9D5B", "#E59A12", "#2F80ED", "#D64545"];

    internal static List<Fruit> Fruits() =>
    [
        new("Maçã", "🍎", "Vermelha e crocante", 4.5),
        new("Banana", "🍌", "Fonte de potássio", 3.2),
        new("Uva", "🍇", "Doce, em cachos", 7.9),
        new("Laranja", "🍊", "Cheia de vitamina C", 3.9),
        new("Morango", "🍓", "Pequeno e azedinho", 9.5),
        new("Abacaxi", "🍍", "Tropical", 6.0),
        new("Melancia", "🍉", "Refrescante", 2.8),
        new("Kiwi", "🥝", "Verde por dentro", 8.4),
        new("Cereja", "🍒", "Sempre em dupla", 12.0),
        new("Pêssego", "🍑", "Macio e aveludado", 7.1),
    ];
}

/// <summary>Modelo simples usado em demos de coleções.</summary>
public sealed record Fruit(string Name, string Emoji, string Description, double Price)
{
    public string PriceText => Price.ToString("C", new System.Globalization.CultureInfo("pt-BR"));
}
