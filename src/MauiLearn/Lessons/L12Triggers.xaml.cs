namespace MauiLearn.Lessons;

public partial class L12Triggers : ContentView
{
    public L12Triggers()
    {
        InitializeComponent();
    }

    private void OnHappy(object? sender, EventArgs e) => VisualStateManager.GoToState(MoodCard, "Feliz");

    private void OnSad(object? sender, EventArgs e) => VisualStateManager.GoToState(MoodCard, "Triste");
}
