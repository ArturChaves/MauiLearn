namespace MauiLearn.Challenges.Workspaces;

public partial class D13DuploClique : ContentView
{
    public D13DuploClique()
    {
        InitializeComponent();
    }

    // Já pronto: ligue-o pelo XAML com Tapped="OnDuploClique".
    private async void OnDuploClique(object? sender, TappedEventArgs e) =>
        await Alvo.RelRotateToAsync(90, 300, Easing.CubicOut);
}
