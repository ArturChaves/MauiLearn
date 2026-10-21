namespace MauiLearn.Lessons;

/// <summary>
/// Behavior&lt;T&gt;: adiciona comportamento a um controle sem herdar dele.
/// Este valida que a Entry contém um número e pinta o texto de vermelho se não.
/// </summary>
public class NumericValidationBehavior : Behavior<Entry>
{
    public static readonly BindableProperty IsValidProperty =
        BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(NumericValidationBehavior), true, BindingMode.OneWayToSource);

    public bool IsValid
    {
        get => (bool)GetValue(IsValidProperty);
        set => SetValue(IsValidProperty, value);
    }

    protected override void OnAttachedTo(Entry entry)
    {
        base.OnAttachedTo(entry);
        entry.TextChanged += OnTextChanged;
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        entry.TextChanged -= OnTextChanged; // sempre desassine: evita vazamentos
        base.OnDetachingFrom(entry);
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender!;
        IsValid = string.IsNullOrEmpty(e.NewTextValue) || double.TryParse(e.NewTextValue, out _);
        entry.TextColor = IsValid ? null : Colors.Red;
    }
}

/// <summary>TriggerAction: código executado por um EventTrigger (aqui, "chacoalha" o elemento).</summary>
public class ShakeTriggerAction : TriggerAction<VisualElement>
{
    protected override async void Invoke(VisualElement sender)
    {
        for (var i = 0; i < 3; i++)
        {
            await sender.TranslateToAsync(-10, 0, 50);
            await sender.TranslateToAsync(10, 0, 50);
        }
        await sender.TranslateToAsync(0, 0, 50);
    }
}
