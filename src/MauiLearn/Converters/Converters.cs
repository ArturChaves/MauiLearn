using System.Globalization;

namespace MauiLearn.Converters;

// IValueConverter transforma valores entre a fonte (ViewModel) e o alvo (controle).
// Convert: fonte -> tela. ConvertBack: tela -> fonte (usado em bindings TwoWay).

/// <summary>bool → texto. Parâmetro: "TextoSeVerdadeiro|TextoSeFalso".</summary>
public class BoolToTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var parts = (parameter as string ?? "Sim|Não").Split('|');
        return value is true ? parts[0] : parts.ElementAtOrDefault(1) ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

public class InvertBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is not true;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value is not true;
}

public class IsNotNullOrEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value switch
        {
            null => false,
            string s => !string.IsNullOrWhiteSpace(s),
            System.Collections.ICollection c => c.Count > 0,
            _ => true,
        };

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

public class DoneToGlyphConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is true ? "✅" : "⬜";

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

/// <summary>
/// Converte Celsius (double, no ViewModel) ↔ Fahrenheit (texto, numa Entry).
/// Repare que ConvertBack recebe o que está NA TELA (uma string) e devolve o tipo da ORIGEM.
/// </summary>
public class CelsiusToFahrenheitConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is double c ? Math.Round(c * 9 / 5 + 32, 1).ToString(culture) : string.Empty;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var text = value?.ToString();
        if (double.TryParse(text, NumberStyles.Float, culture, out var f) ||
            double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out f))
            return Math.Round((f - 32) * 5 / 9, 1);

        // Valor inválido: não altera a origem.
        return BindableProperty.UnsetValue;
    }
}

/// <summary>MultiBinding: junta nome e sobrenome.</summary>
public class FullNameConverter : IMultiValueConverter
{
    public object? Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture) =>
        string.Join(' ', values.Select(v => v?.ToString()).Where(s => !string.IsNullOrWhiteSpace(s)));

    public object?[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
