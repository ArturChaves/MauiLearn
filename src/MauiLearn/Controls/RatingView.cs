namespace MauiLearn.Controls;

/// <summary>
/// Controle customizado de avaliação (estrelas), feito do zero em C#.
/// Demonstra: BindableProperty com valor padrão, BindingMode.TwoWay padrão,
/// validação (coerceValue), callback de mudança (propertyChanged) e evento próprio.
/// </summary>
public class RatingView : ContentView
{
    public static readonly BindableProperty RatingProperty = BindableProperty.Create(
        nameof(Rating), typeof(int), typeof(RatingView), 0,
        defaultBindingMode: BindingMode.TwoWay,                                   // como Entry.Text
        coerceValue: (b, v) => Math.Clamp((int)v, 0, ((RatingView)b).Maximum),   // nunca fora do intervalo
        propertyChanged: (b, o, n) => ((RatingView)b).OnRatingChanged((int)o, (int)n));

    public static readonly BindableProperty MaximumProperty = BindableProperty.Create(
        nameof(Maximum), typeof(int), typeof(RatingView), 5,
        validateValue: (_, v) => (int)v is > 0 and <= 20,                         // valor inválido lança exceção
        propertyChanged: (b, _, _) => ((RatingView)b).Build());

    public static readonly BindableProperty StarColorProperty = BindableProperty.Create(
        nameof(StarColor), typeof(Color), typeof(RatingView), Color.FromArgb("#F5A623"),
        propertyChanged: (b, _, _) => ((RatingView)b).Render());

    public static readonly BindableProperty StarSizeProperty = BindableProperty.Create(
        nameof(StarSize), typeof(double), typeof(RatingView), 28d,
        propertyChanged: (b, _, _) => ((RatingView)b).Render());

    private readonly HorizontalStackLayout _stars = new() { Spacing = 2 };

    public RatingView()
    {
        Content = _stars;
        Build();
    }

    /// <summary>Evento próprio do controle.</summary>
    public event EventHandler<int>? RatingChanged;

    public int Rating
    {
        get => (int)GetValue(RatingProperty);
        set => SetValue(RatingProperty, value);
    }

    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public Color StarColor
    {
        get => (Color)GetValue(StarColorProperty);
        set => SetValue(StarColorProperty, value);
    }

    public double StarSize
    {
        get => (double)GetValue(StarSizeProperty);
        set => SetValue(StarSizeProperty, value);
    }

    private void Build()
    {
        _stars.Clear();
        for (var i = 1; i <= Maximum; i++)
        {
            var value = i;
            var star = new Label();
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) => Rating = Rating == value ? value - 1 : value; // clicar de novo desmarca
            star.GestureRecognizers.Add(tap);
            ToolTipProperties.SetText(star, $"{value} estrela(s)");
            _stars.Add(star);
        }
        if (Rating > Maximum) Rating = Maximum;
        Render();
    }

    private void Render()
    {
        var i = 1;
        foreach (var star in _stars.Children.OfType<Label>())
        {
            star.Text = i <= Rating ? "★" : "☆";
            star.TextColor = i <= Rating ? StarColor : Colors.Gray;
            star.FontSize = StarSize;
            i++;
        }
    }

    private void OnRatingChanged(int oldValue, int newValue)
    {
        Render();
        RatingChanged?.Invoke(this, newValue);
    }
}

/// <summary>
/// Controle "lookless": só define dados (Title, Icon). A APARÊNCIA vem de um ControlTemplate,
/// que pode ser trocado sem mexer no controle. O conteúdo aparece onde houver um ContentPresenter.
/// </summary>
public class CardView : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(CardView), string.Empty);

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(nameof(Icon), typeof(string), typeof(CardView), "📦");

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
