namespace MauiLearn.Lessons;

/// <summary>
/// Um gráfico de barras desenhado "na mão" com Microsoft.Maui.Graphics.
/// IDrawable.Draw recebe um ICanvas — uma API de desenho imediato parecida com HTML Canvas/SkiaSharp.
/// </summary>
public sealed class BarChartDrawable : IDrawable
{
    public double[] Values { get; set; } = [3, 7, 5, 9, 4];
    public string[] Labels { get; set; } = ["Seg", "Ter", "Qua", "Qui", "Sex"];
    public Color BarColor { get; set; } = Color.FromArgb("#512BD4");
    public Color TextColor { get; set; } = Colors.Gray;

    public void Draw(ICanvas canvas, RectF rect)
    {
        const float padding = 28;
        var max = Math.Max(1, Values.Max());
        var chart = new RectF(rect.X + padding, rect.Y + 10, rect.Width - padding * 2, rect.Height - padding - 10);
        var slot = chart.Width / Values.Length;

        // Eixo
        canvas.StrokeColor = TextColor;
        canvas.StrokeSize = 1;
        canvas.DrawLine(chart.Left, chart.Bottom, chart.Right, chart.Bottom);

        canvas.FontSize = 12;
        canvas.FontColor = TextColor;
        for (var i = 0; i < Values.Length; i++)
        {
            var h = (float)(Values[i] / max * chart.Height);
            var x = chart.Left + i * slot + slot * 0.2f;
            var w = slot * 0.6f;

            // Barra com degradê (LinearGradientPaint)
            canvas.SetFillPaint(new LinearGradientPaint
            {
                StartColor = BarColor.WithLuminosity(0.65f),
                EndColor = BarColor,
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
            }, new RectF(x, chart.Bottom - h, w, h));
            canvas.FillRoundedRectangle(x, chart.Bottom - h, w, h, 6);

            canvas.DrawString(Values[i].ToString("0"), x, chart.Bottom - h - 18, w, 16, HorizontalAlignment.Center, VerticalAlignment.Bottom);
            canvas.DrawString(Labels[i % Labels.Length], x, chart.Bottom + 4, w, 16, HorizontalAlignment.Center, VerticalAlignment.Top);
        }
    }
}
