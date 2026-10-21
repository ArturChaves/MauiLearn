using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace MauiLearn.Core;

/// <summary>Uma BindableProperty descoberta por reflexão, com metadados para o inspetor.</summary>
public sealed record InspectableProperty(
    BindableProperty Property,
    Type DeclaringType,
    string DisplayName,
    bool IsAttached)
{
    public Type ValueType => Nullable.GetUnderlyingType(Property.ReturnType) ?? Property.ReturnType;
    public bool IsNullable => !Property.ReturnType.IsValueType || Nullable.GetUnderlyingType(Property.ReturnType) is not null;
}

/// <summary>
/// Descobre TODAS as BindableProperties de um objeto por reflexão.
///
/// Por convenção, toda BindableProperty "Xyz" é exposta como um campo estático público
/// "XyzProperty" na classe que a declara. Percorrendo a hierarquia de herança
/// (Button → View → VisualElement → ...), encontramos todas — é assim que o inspetor
/// consegue mostrar "todas as propriedades" de qualquer controle.
/// </summary>
public static class BindablePropertyReflector
{
    /// <summary>
    /// Propriedades exibidas, mas nunca editáveis no inspetor (estruturais ou perigosas).
    /// Ex.: AutomationId só pode ser definido UMA vez — redefinir lança exceção.
    /// </summary>
    public static readonly HashSet<string> ForceReadOnly =
    [
        "BindingContext", "Style", "Window", "Parent", "StyleClass", "class", "AutomationId", "Visual",
        "ControlTemplate", "Handler", "Navigation", "Resources",
    ];

    private static readonly (BindableProperty Property, string Name)[] UsefulAttached =
    [
        (ToolTipProperties.TextProperty, "ToolTipProperties.Text"),
        (SemanticProperties.DescriptionProperty, "SemanticProperties.Description"),
        (SemanticProperties.HintProperty, "SemanticProperties.Hint"),
        (SemanticProperties.HeadingLevelProperty, "SemanticProperties.HeadingLevel"),
    ];

    public static IReadOnlyList<InspectableProperty> GetProperties(BindableObject target)
    {
        var result = new List<InspectableProperty>();
        var seen = new HashSet<string>();

        for (var type = target.GetType(); type is not null && type != typeof(object); type = type.BaseType)
        {
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                if (field.FieldType != typeof(BindableProperty) || field.GetValue(null) is not BindableProperty bp)
                    continue;

                // Só propriedades "de instância": existe uma propriedade CLR com o mesmo nome.
                // Isso filtra propriedades anexadas como Grid.RowProperty dentro de Grid.
                var clr = type.GetProperty(bp.PropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (clr is null)
                    continue;

                if (!seen.Add(bp.PropertyName))
                    continue;

                result.Add(new InspectableProperty(bp, type, bp.PropertyName, false));
            }
        }

        if (target is VisualElement)
        {
            foreach (var (bp, name) in UsefulAttached)
                result.Add(new InspectableProperty(bp, bp.DeclaringType, name, true));
        }

        return result;
    }

    /// <summary>Hierarquia de tipos, ex.: "Button → View → VisualElement → ... → BindableObject".</summary>
    public static string Hierarchy(Type type)
    {
        var names = new List<string>();
        for (var t = type; t is not null && t != typeof(object); t = t.BaseType)
            names.Add(FriendlyTypeName(t));
        return string.Join(" → ", names);
    }

    public static string FriendlyTypeName(Type t)
    {
        if (Nullable.GetUnderlyingType(t) is { } inner)
            return FriendlyTypeName(inner) + "?";
        if (!t.IsGenericType)
            return t.Name switch
            {
                "String" => "string",
                "Boolean" => "bool",
                "Double" => "double",
                "Int32" => "int",
                "Single" => "float",
                "Object" => "object",
                _ => t.Name,
            };
        var name = t.Name[..t.Name.IndexOf('`')];
        return $"{name}<{string.Join(", ", t.GetGenericArguments().Select(FriendlyTypeName))}>";
    }

    // ---------------- Conversão de valores (string ↔ objeto) ----------------

    /// <summary>
    /// Converte texto para o tipo da propriedade usando o mesmo mecanismo do XAML:
    /// TypeConverters. É por isso que em XAML você escreve Margin="10,5" e o MAUI
    /// cria um Thickness — o ThicknessTypeConverter faz a conversão.
    /// </summary>
    public static bool TryParse(string text, Type type, out object? value)
    {
        value = null;
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying is not null && string.IsNullOrWhiteSpace(text))
            return true; // null para tipos anuláveis

        var target = underlying ?? type;
        try
        {
            if (target == typeof(string)) { value = text; return true; }
            if (target == typeof(double)) { value = double.Parse(text, CultureInfo.InvariantCulture); return true; }
            if (target == typeof(float)) { value = float.Parse(text, CultureInfo.InvariantCulture); return true; }
            if (target == typeof(int)) { value = int.Parse(text, CultureInfo.InvariantCulture); return true; }
            if (target == typeof(uint)) { value = uint.Parse(text, CultureInfo.InvariantCulture); return true; }
            if (target == typeof(long)) { value = long.Parse(text, CultureInfo.InvariantCulture); return true; }
            if (target == typeof(DateTime)) { value = DateTime.Parse(text, CultureInfo.CurrentCulture); return true; }
            if (target == typeof(TimeSpan)) { value = TimeSpan.Parse(text, CultureInfo.InvariantCulture); return true; }
            if (target.IsEnum) { value = Enum.Parse(target, text.Replace('|', ','), ignoreCase: true); return true; }

            var converter = TypeDescriptor.GetConverter(target);
            if (converter.CanConvertFrom(typeof(string)))
            {
                value = converter.ConvertFromInvariantString(text);
                return true;
            }
        }
        catch
        {
            // valor inválido: devolve false
        }
        return false;
    }

    public static bool CanParse(Type type)
    {
        var t = Nullable.GetUnderlyingType(type) ?? type;
        if (t.IsPrimitive || t == typeof(string) || t.IsEnum || t == typeof(DateTime) || t == typeof(TimeSpan))
            return true;
        try
        {
            return TypeDescriptor.GetConverter(t) is { } c && c.GetType() != typeof(TypeConverter) && c.CanConvertFrom(typeof(string));
        }
        catch
        {
            return false;
        }
    }

    /// <summary>Formata um valor do jeito que você o escreveria em XAML.</summary>
    public static string ToXaml(object? value)
    {
        static string N(double d) => d.ToString("0.##", CultureInfo.InvariantCulture);

        return value switch
        {
            null => "{x:Null}",
            string s => s,
            bool b => b ? "True" : "False",
            double d => N(d),
            float f => N(f),
            Color c => c.ToArgbHex(c.Alpha < 1),
            SolidColorBrush sb => sb.Color is { } c2 ? c2.ToArgbHex(c2.Alpha < 1) : "Transparent",
            Thickness t when t.Left == t.Right && t.Top == t.Bottom && t.Left == t.Top => N(t.Left),
            Thickness t when t.Left == t.Right && t.Top == t.Bottom => $"{N(t.Left)},{N(t.Top)}",
            Thickness t => $"{N(t.Left)},{N(t.Top)},{N(t.Right)},{N(t.Bottom)}",
            CornerRadius cr when cr.TopLeft == cr.TopRight && cr.TopLeft == cr.BottomLeft && cr.TopLeft == cr.BottomRight => N(cr.TopLeft),
            CornerRadius cr => $"{N(cr.TopLeft)},{N(cr.TopRight)},{N(cr.BottomLeft)},{N(cr.BottomRight)}",
            LayoutOptions lo => lo.Alignment.ToString(),
            Point p => $"{N(p.X)},{N(p.Y)}",
            Size sz => $"{N(sz.Width)},{N(sz.Height)}",
            Rect r => $"{N(r.X)},{N(r.Y)},{N(r.Width)},{N(r.Height)}",
            DateTime dt => dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            TimeSpan ts => ts.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture),
            Enum e => e.ToString().Replace(", ", ","),
            FileImageSource fis => fis.File,
            UriImageSource uis => uis.Uri?.ToString() ?? string.Empty,
            IFormattable fmt => fmt.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? string.Empty,
        };
    }

    /// <summary>Texto para mostrar num campo editável (como ToXaml, mas null vira vazio).</summary>
    public static string ToEditText(object? value) => value is null ? string.Empty : ToXaml(value);
}
