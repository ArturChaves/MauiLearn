namespace MauiLearn.Lessons;

public partial class L20Controles : ContentView
{
    public L20Controles()
    {
        InitializeComponent();
    }

    private void OnRatingChanged(object? sender, int value) =>
        RatingLog.Text = $"Evento RatingChanged: {value} de {Rating.Maximum}";

    private void OnClassic(object? sender, EventArgs e) => SetTemplate("CardClassico");

    private void OnHighlight(object? sender, EventArgs e) => SetTemplate("CardDestaque");

    private void SetTemplate(string key)
    {
        var template = (ControlTemplate)Resources[key];
        Card1.ControlTemplate = template;
        Card2.ControlTemplate = template;
    }
}
