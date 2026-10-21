using MauiLearn.Core;
using Microsoft.Maui.Controls.Shapes;

namespace MauiLearn.Controls;

/// <summary>Um editor de propriedade: a View e uma forma de atualizá-la com um novo valor.</summary>
public sealed class PropertyEditor(View view, Action<object?> refresh)
{
    public View View { get; } = view;
    public Action<object?> Refresh { get; } = refresh;
}

/// <summary>Valor pré-definido para tipos complexos (Brush, Shadow, formas...), com o XAML equivalente.</summary>
public sealed record Preset(string Name, Func<object?> Create, string Xaml);

/// <summary>
/// Cria o editor adequado para cada tipo de propriedade:
/// bool → Switch · número → Entry (+Slider) · enum → Picker · Color → amostra + hex + paleta ·
/// tipos com TypeConverter (Thickness, CornerRadius, LayoutOptions...) → Entry · tipos complexos → presets.
/// </summary>
public static class PropertyEditorFactory
{
    public static readonly (string Name, Color Color)[] NamedColors =
    [
        ("Transparent", Colors.Transparent), ("White", Colors.White), ("Black", Colors.Black), ("Gray", Colors.Gray),
        ("LightGray", Colors.LightGray), ("Red", Colors.Red), ("Orange", Colors.Orange), ("Gold", Colors.Gold),
        ("Green", Colors.Green), ("SeaGreen", Colors.SeaGreen), ("Teal", Colors.Teal), ("DodgerBlue", Colors.DodgerBlue),
        ("Blue", Colors.Blue), ("Purple", Colors.Purple), ("HotPink", Colors.HotPink), ("Brown", Colors.Brown),
        ("#512BD4 (MAUI)", Color.FromArgb("#512BD4")), ("#D600AA", Color.FromArgb("#D600AA")),
    ];

    public static PropertyEditor? Create(InspectableProperty prop, BindableObject target, Action<object?, string?> setValue)
    {
        var type = prop.ValueType;
        var name = prop.Property.PropertyName;

        if (type == typeof(bool))
            return BoolEditor(v => setValue(v, null));

        if (type.IsEnum)
            return EnumEditor(type, prop.IsNullable, v => setValue(v, null));

        if (type == typeof(Color))
            return ColorEditor(prop.IsNullable, v => setValue(v, null));

        if (Presets(type, name) is { } presets)
            return PresetEditor(presets, setValue);

        if (type == typeof(double) || type == typeof(float) || type == typeof(int) || type == typeof(uint) || type == typeof(long))
            return NumberEditor(prop, v => setValue(v, null));

        if (BindablePropertyReflector.CanParse(prop.Property.ReturnType))
            return TextEditor(prop, v => setValue(v, null));

        return null;
    }

    // ---------------- Editores ----------------

    private static PropertyEditor BoolEditor(Action<object?> set)
    {
        var sw = new Switch { HorizontalOptions = LayoutOptions.Start };
        var updating = false;
        sw.Toggled += (_, e) => { if (!updating) set(e.Value); };
        return new PropertyEditor(sw, v => { updating = true; sw.IsToggled = v is true; updating = false; });
    }

    private static PropertyEditor EnumEditor(Type enumType, bool nullable, Action<object?> set)
    {
        var names = Enum.GetNames(enumType).ToList();
        if (nullable) names.Insert(0, "(null)");
        var picker = new Picker { ItemsSource = names, FontSize = 13 };
        var updating = false;
        picker.SelectedIndexChanged += (_, _) =>
        {
            if (updating || picker.SelectedItem is not string s) return;
            set(s == "(null)" ? null : Enum.Parse(enumType, s));
        };
        return new PropertyEditor(picker, v =>
        {
            updating = true;
            var text = v?.ToString();
            var idx = text is null ? (nullable ? 0 : -1) : names.IndexOf(text);
            // Enums [Flags] com combinação (ex.: "Bold, Italic") não estão na lista.
            picker.Title = idx < 0 ? text ?? "" : "";
            picker.SelectedIndex = idx;
            updating = false;
        });
    }

    private static PropertyEditor ColorEditor(bool nullable, Action<object?> set)
    {
        var swatch = new Border
        {
            WidthRequest = 26,
            HeightRequest = 26,
            StrokeShape = new RoundRectangle { CornerRadius = 6 },
            Stroke = Colors.Gray,
            StrokeThickness = 1,
            VerticalOptions = LayoutOptions.Center,
        };
        var entry = new Entry { Placeholder = "#RRGGBB", WidthRequest = 104, FontSize = 13 };
        var names = NamedColors.Select(c => c.Name).ToList();
        names.Insert(0, "(null)");
        var picker = new Picker { ItemsSource = names, Title = "paleta", FontSize = 13, WidthRequest = 130 };
        var updating = false;

        entry.Completed += (_, _) => TryApply(entry.Text);
        entry.Unfocused += (_, _) => TryApply(entry.Text);
        picker.SelectedIndexChanged += (_, _) =>
        {
            if (updating || picker.SelectedIndex < 0) return;
            set(picker.SelectedIndex == 0 ? null : NamedColors[picker.SelectedIndex - 1].Color);
        };

        void TryApply(string? text)
        {
            if (updating) return;
            if (string.IsNullOrWhiteSpace(text)) { if (nullable) set(null); return; }
            if (Color.TryParse(text.Trim(), out var c)) set(c);
        }

        var row = new HorizontalStackLayout { Spacing = 6, Children = { swatch, entry, picker } };
        return new PropertyEditor(row, v =>
        {
            updating = true;
            var c = v as Color;
            swatch.Background = c is null ? new LinearGradientBrush([new GradientStop(Colors.White, 0.45f), new GradientStop(Colors.Red, 0.5f), new GradientStop(Colors.White, 0.55f)], new Point(0, 0), new Point(1, 1)) : new SolidColorBrush(c);
            if (!entry.IsFocused) entry.Text = c is null ? string.Empty : c.ToArgbHex(c.Alpha < 1);
            picker.SelectedIndex = -1;
            updating = false;
        });
    }

    private static PropertyEditor NumberEditor(InspectableProperty prop, Action<object?> set)
    {
        var type = prop.ValueType;
        var entry = new Entry { WidthRequest = 90, FontSize = 13, HorizontalTextAlignment = TextAlignment.End };
        var updating = false;
        var range = GuessRange(prop.Property.PropertyName);
        Slider? slider = null;

        void Apply(object? value)
        {
            if (updating) return;
            set(value);
        }

        entry.Completed += (_, _) => Parse();
        entry.Unfocused += (_, _) => Parse();
        void Parse()
        {
            if (BindablePropertyReflector.TryParse(entry.Text ?? "", prop.Property.ReturnType, out var v)) Apply(v);
        }

        var row = new Grid { ColumnDefinitions = [new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star)], ColumnSpacing = 8 };
        row.Add(entry, 0, 0);
        if (range is { } r)
        {
            slider = new Slider { Maximum = r.Max, Minimum = r.Min, VerticalOptions = LayoutOptions.Center };
            slider.ValueChanged += (_, e) =>
            {
                if (updating) return;
                object value = type == typeof(int) ? (int)Math.Round(e.NewValue)
                    : type == typeof(float) ? (float)e.NewValue
                    : type == typeof(uint) ? (uint)Math.Round(e.NewValue)
                    : type == typeof(long) ? (long)Math.Round(e.NewValue)
                    : Math.Round(e.NewValue, r.Max <= 1 ? 2 : 1);
                Apply(value);
            };
            row.Add(slider, 1, 0);
        }

        return new PropertyEditor(row, v =>
        {
            updating = true;
            if (!entry.IsFocused) entry.Text = BindablePropertyReflector.ToEditText(v);
            if (slider is not null && v is IConvertible conv)
            {
                var d = conv.ToDouble(System.Globalization.CultureInfo.InvariantCulture);
                if (!double.IsNaN(d) && !double.IsInfinity(d))
                    slider.Value = Math.Clamp(d, slider.Minimum, slider.Maximum);
            }
            updating = false;
        });
    }

    private static (double Min, double Max)? GuessRange(string name) => name switch
    {
        "Opacity" or "Progress" or "AnchorX" or "AnchorY" => (0, 1),
        "Rotation" or "RotationX" or "RotationY" => (0, 360),
        "Scale" or "ScaleX" or "ScaleY" => (0, 4),
        "TranslationX" or "TranslationY" => (-200, 200),
        "FontSize" or "IndicatorSize" => (6, 72),
        "Value" or "Minimum" or "Maximum" or "Increment" => (0, 100),
        "LineHeight" => (0.5, 3),
        "CharacterSpacing" => (0, 20),
        "MaxLines" => (0, 10),
        "NumberOfTapsRequired" => (1, 4),
        "Position" => (0, 9),
        "Count" => (0, 12),
        "Span" => (1, 6),
        "ZIndex" => (-10, 10),
        "MinimumPressDuration" => (100, 3000),
        _ when name.Contains("Spacing") || name.Contains("Radius") || name.Contains("Thickness") || name.Contains("Width") || name.Contains("Height") || name.StartsWith('X') || name.StartsWith('Y') => (0, 400),
        _ => null,
    };

    private static PropertyEditor TextEditor(InspectableProperty prop, Action<object?> set)
    {
        var placeholder = BindablePropertyReflector.FriendlyTypeName(prop.ValueType) switch
        {
            "Thickness" => "ex.: 10 ou 10,5 ou 10,5,10,5",
            "CornerRadius" => "ex.: 8 ou 8,0,8,0",
            "LayoutOptions" => "Start, Center, End, Fill",
            "Point" => "x,y",
            "Rect" => "x,y,largura,altura",
            "RowDefinitionCollection" or "ColumnDefinitionCollection" => "ex.: Auto,*,2*,100",
            "DateTime" => "dd/mm/aaaa",
            "TimeSpan" => "hh:mm:ss",
            "ImageSource" => "arquivo.png ou https://…",
            "FontSize" => "número",
            _ => prop.ValueType == typeof(string) ? "" : BindablePropertyReflector.FriendlyTypeName(prop.ValueType),
        };
        var entry = new Entry { Placeholder = placeholder, FontSize = 13 };
        var updating = false;
        var isString = prop.ValueType == typeof(string);

        if (isString)
            entry.TextChanged += (_, e) => { if (!updating) set(e.NewTextValue); };
        else
        {
            entry.Completed += (_, _) => Parse();
            entry.Unfocused += (_, _) => Parse();
        }

        void Parse()
        {
            if (updating) return;
            if (BindablePropertyReflector.TryParse(entry.Text ?? "", prop.Property.ReturnType, out var v)) set(v);
            else entry.TextColor = Colors.Red;
        }

        return new PropertyEditor(entry, v =>
        {
            updating = true;
            entry.ClearValue(Entry.TextColorProperty);
            if (!entry.IsFocused || !isString) entry.Text = v is RowDefinitionCollection or ColumnDefinitionCollection ? GridDefinitionsToText(v) : BindablePropertyReflector.ToEditText(v);
            updating = false;
        });
    }

    private static PropertyEditor PresetEditor(IReadOnlyList<Preset> presets, Action<object?, string?> set)
    {
        var picker = new Picker { ItemsSource = presets.Select(p => p.Name).ToList(), Title = "(valor atual)", FontSize = 13 };
        var updating = false;
        picker.SelectedIndexChanged += (_, _) =>
        {
            if (updating || picker.SelectedIndex < 0) return;
            var preset = presets[picker.SelectedIndex];
            set(preset.Create(), preset.Xaml);
        };
        return new PropertyEditor(picker, v =>
        {
            updating = true;
            picker.Title = Describe(v);
            if (v is null) picker.SelectedIndex = -1;
            updating = false;
        });
    }

    public static string GridDefinitionsToText(object? v)
    {
        static string L(GridLength g) => g.IsAuto ? "Auto" : g.IsStar ? (g.Value == 1 ? "*" : $"{g.Value:0.##}*") : g.Value.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
        return v switch
        {
            RowDefinitionCollection rows => string.Join(",", rows.Select(r => L(r.Height))),
            ColumnDefinitionCollection cols => string.Join(",", cols.Select(c => L(c.Width))),
            _ => string.Empty,
        };
    }

    public static string Describe(object? v) => v switch
    {
        null => "(nulo)",
        SolidColorBrush sb => $"Solid {sb.Color?.ToArgbHex()}",
        LinearGradientBrush lg => $"LinearGradient ({lg.GradientStops.Count} cores)",
        RadialGradientBrush rg => $"RadialGradient ({rg.GradientStops.Count} cores)",
        Shadow s => $"Shadow r={s.Radius} o={s.Opacity:0.##}",
        RoundRectangle rr => $"RoundRectangle {BindablePropertyReflector.ToXaml(rr.CornerRadius)}",
        FileImageSource f => f.File,
        UriImageSource u => u.Uri?.Host ?? "Uri",
        FontImageSource fi => $"FontImage {fi.Glyph}",
        GridItemsLayout g => $"{g.Orientation}Grid, {g.Span}",
        LinearItemsLayout l => $"{l.Orientation}List",
        DoubleCollection dc => string.Join(",", dc),
        _ => v.GetType().Name,
    };

    // ---------------- Presets ----------------

    private static IReadOnlyList<Preset>? Presets(Type type, string name)
    {
        if (typeof(Brush).IsAssignableFrom(type))
            return
            [
                new("(nulo)", () => null, "{x:Null}"),
                new("Roxo MAUI", () => new SolidColorBrush(Color.FromArgb("#512BD4")), "#512BD4"),
                new("Rosa", () => new SolidColorBrush(Color.FromArgb("#D600AA")), "#D600AA"),
                new("Verde", () => new SolidColorBrush(Color.FromArgb("#2E9D5B")), "#2E9D5B"),
                new("Transparente", () => new SolidColorBrush(Colors.Transparent), "Transparent"),
                new("Gradiente linear roxo → laranja", () => new LinearGradientBrush([new GradientStop(Color.FromArgb("#512BD4"), 0), new GradientStop(Color.FromArgb("#E59A12"), 1)], new Point(0, 0), new Point(1, 1)),
                    "<LinearGradientBrush EndPoint=\"1,1\"><GradientStop Color=\"#512BD4\" Offset=\"0\" /><GradientStop Color=\"#E59A12\" Offset=\"1\" /></LinearGradientBrush>"),
                new("Gradiente radial", () => new RadialGradientBrush([new GradientStop(Colors.White, 0), new GradientStop(Color.FromArgb("#2F80ED"), 1)], new Point(0.5, 0.5), 0.7),
                    "<RadialGradientBrush Radius=\"0.7\"><GradientStop Color=\"White\" Offset=\"0\" /><GradientStop Color=\"#2F80ED\" Offset=\"1\" /></RadialGradientBrush>"),
            ];

        if (type == typeof(Shadow))
            return
            [
                new("(nenhuma)", () => null, "{x:Null}"),
                new("Suave", () => new Shadow { Radius = 8, Opacity = 0.25f, Offset = new Point(2, 2) }, "<Shadow Radius=\"8\" Opacity=\"0.25\" Offset=\"2,2\" />"),
                new("Forte", () => new Shadow { Radius = 20, Opacity = 0.6f, Offset = new Point(8, 8) }, "<Shadow Radius=\"20\" Opacity=\"0.6\" Offset=\"8,8\" />"),
                new("Colorida (brilho)", () => new Shadow { Radius = 24, Opacity = 0.9f, Offset = new Point(0, 0), Brush = new SolidColorBrush(Color.FromArgb("#D600AA")) }, "<Shadow Brush=\"#D600AA\" Radius=\"24\" Opacity=\"0.9\" Offset=\"0,0\" />"),
            ];

        if (typeof(IShape).IsAssignableFrom(type) || name == "StrokeShape")
            return
            [
                new("Rectangle", () => new Microsoft.Maui.Controls.Shapes.Rectangle(), "Rectangle"),
                new("RoundRectangle 8", () => new RoundRectangle { CornerRadius = 8 }, "RoundRectangle 8"),
                new("RoundRectangle 24", () => new RoundRectangle { CornerRadius = 24 }, "RoundRectangle 24"),
                new("RoundRectangle 30,0,0,30", () => new RoundRectangle { CornerRadius = new CornerRadius(30, 0, 0, 30) }, "RoundRectangle 30,0,0,30"),
                new("Ellipse", () => new Ellipse(), "Ellipse"),
            ];

        if (typeof(ImageSource).IsAssignableFrom(type))
            return
            [
                new("(nenhuma)", () => null, "{x:Null}"),
                new("maui_logo.png", () => ImageSource.FromFile("maui_logo.png"), "maui_logo.png"),
                new("sample_landscape.png", () => ImageSource.FromFile("sample_landscape.png"), "sample_landscape.png"),
                new("icon_star.png", () => ImageSource.FromFile("icon_star.png"), "icon_star.png"),
                new("FontImageSource ★", () => new FontImageSource { Glyph = "★", Color = Color.FromArgb("#E59A12"), Size = 32 }, "<FontImageSource Glyph=\"★\" Color=\"#E59A12\" Size=\"32\" />"),
                new("URL (dotnet bot)", () => ImageSource.FromUri(new Uri("https://raw.githubusercontent.com/dotnet/maui/main/src/Templates/src/templates/maui-mobile/Resources/Images/dotnet_bot.png")),
                    "https://raw.githubusercontent.com/dotnet/maui/main/src/Templates/src/templates/maui-mobile/Resources/Images/dotnet_bot.png"),
            ];

        if (type == typeof(Keyboard))
            return
            [
                new("Default", () => Keyboard.Default, "Default"),
                new("Chat", () => Keyboard.Chat, "Chat"),
                new("Email", () => Keyboard.Email, "Email"),
                new("Numeric", () => Keyboard.Numeric, "Numeric"),
                new("Plain", () => Keyboard.Plain, "Plain"),
                new("Telephone", () => Keyboard.Telephone, "Telephone"),
                new("Text", () => Keyboard.Text, "Text"),
                new("Url", () => Keyboard.Url, "Url"),
            ];

        if (type == typeof(IItemsLayout))
            return
            [
                new("VerticalList", () => LinearItemsLayout.Vertical, "VerticalList"),
                new("HorizontalList", () => LinearItemsLayout.Horizontal, "HorizontalList"),
                new("VerticalGrid, 2", () => new GridItemsLayout(2, ItemsLayoutOrientation.Vertical) { HorizontalItemSpacing = 6, VerticalItemSpacing = 6 }, "VerticalGrid, 2"),
                new("VerticalGrid, 3", () => new GridItemsLayout(3, ItemsLayoutOrientation.Vertical) { HorizontalItemSpacing = 6, VerticalItemSpacing = 6 }, "VerticalGrid, 3"),
                new("HorizontalGrid, 2", () => new GridItemsLayout(2, ItemsLayoutOrientation.Horizontal), "HorizontalGrid, 2"),
            ];

        if (type == typeof(DoubleCollection))
            return
            [
                new("(sólido)", () => null, "{x:Null}"),
                new("4,2", () => new DoubleCollection([4, 2]), "4,2"),
                new("1,1", () => new DoubleCollection([1, 1]), "1,1"),
                new("8,4,2,4", () => new DoubleCollection([8, 4, 2, 4]), "8,4,2,4"),
            ];

        if (type == typeof(Geometry))
            return
            [
                new("(nenhum)", () => null, "{x:Null}"),
                new("Elipse 100×60", () => new EllipseGeometry { Center = new Point(60, 40), RadiusX = 60, RadiusY = 40 }, "<EllipseGeometry Center=\"60,40\" RadiusX=\"60\" RadiusY=\"40\" />"),
                new("Retângulo 0,0,80,40", () => new RectangleGeometry { Rect = new Rect(0, 0, 80, 40) }, "<RectangleGeometry Rect=\"0,0,80,40\" />"),
                new("RoundRectangle", () => new RoundRectangleGeometry { Rect = new Rect(0, 0, 120, 60), CornerRadius = 20 }, "<RoundRectangleGeometry Rect=\"0,0,120,60\" CornerRadius=\"20\" />"),
            ];

        return null;
    }
}
