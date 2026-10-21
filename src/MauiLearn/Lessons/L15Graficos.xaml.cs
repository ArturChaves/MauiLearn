namespace MauiLearn.Lessons;

public partial class L15Graficos : ContentView
{
    private readonly BarChartDrawable _chart = new();

    public L15Graficos()
    {
        InitializeComponent();
        Chart.Drawable = _chart;

        for (var i = 0; i < _chart.Values.Length; i++)
        {
            var index = i;
            var slider = new Slider { Maximum = 10, Minimum = 0, Value = _chart.Values[i] };
            slider.ValueChanged += (_, e) =>
            {
                _chart.Values[index] = Math.Round(e.NewValue);
                Chart.Invalidate(); // pede um novo Draw
            };
            BarSliders.Add(new Label { Text = _chart.Labels[i], FontSize = 12 });
            BarSliders.Add(slider);
        }

        OnGradientChanged(null, EventArgs.Empty);
        OnShadowChanged(null, EventArgs.Empty);
    }

    private void OnGradientChanged(object? sender, EventArgs e)
    {
        if (GradientBox is null || GradientCode is null || MidOffset is null || EndY is null) return;

        var start = new Point(Math.Round(StartX.Value, 2), Math.Round(StartY.Value, 2));
        var end = new Point(Math.Round(EndX.Value, 2), Math.Round(EndY.Value, 2));
        var mid = (float)Math.Round(MidOffset.Value, 2);
        GradientBox.Background = new LinearGradientBrush(
            [
                new GradientStop(Color.FromArgb("#512BD4"), 0),
                new GradientStop(Color.FromArgb("#D600AA"), mid),
                new GradientStop(Color.FromArgb("#E59A12"), 1),
            ], start, end);

        // Cultura invariante: em pt-BR, 0.5 viraria "0,5" — e a vírgula separa X e Y em XAML!
        GradientCode.Code = FormattableString.Invariant($"""
            <LinearGradientBrush StartPoint="{start.X:0.##},{start.Y:0.##}" EndPoint="{end.X:0.##},{end.Y:0.##}">
                <GradientStop Color="#512BD4" Offset="0" />
                <GradientStop Color="#D600AA" Offset="{mid:0.##}" />
                <GradientStop Color="#E59A12" Offset="1" />
            </LinearGradientBrush>
            """);
    }

    private void OnShadowChanged(object? sender, EventArgs e)
    {
        if (ShadowBox is null || ShadowOpacity is null || ShadowY is null) return;
        ShadowBox.Shadow = new Shadow
        {
            Brush = new SolidColorBrush(Colors.Black),
            Radius = (float)ShadowRadius.Value,
            Offset = new Point(ShadowX.Value, ShadowY.Value),
            Opacity = (float)ShadowOpacity.Value,
        };
    }
}
