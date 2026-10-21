using MauiLearn.Controls;

namespace MauiLearn.Lessons;

public partial class L09Estilos : ContentView
{
    public L09Estilos()
    {
        InitializeComponent();
    }

    private void OnColorClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: string hex })
            Resources["CorDinamica"] = Color.FromArgb(hex); // DynamicResource reage; StaticResource não
    }

    private void OnThemeLight(object? sender, EventArgs e) => AppMenu.SetTheme(AppTheme.Light);
    private void OnThemeDark(object? sender, EventArgs e) => AppMenu.SetTheme(AppTheme.Dark);
    private void OnThemeSystem(object? sender, EventArgs e) => AppMenu.SetTheme(AppTheme.Unspecified);
}
