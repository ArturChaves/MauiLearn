using MauiLearn.Controls;
using MauiLearn.Core;

namespace MauiLearn.Pages;

public partial class SettingsPage : AppPage
{
    private readonly ProgressService _progress;
    private bool _loading;

    public SettingsPage(ProgressService progress)
    {
        InitializeComponent();
        _progress = progress;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _loading = true;
        var theme = Application.Current?.UserAppTheme ?? AppTheme.Unspecified;
        ThemeSystem.IsChecked = theme == AppTheme.Unspecified;
        ThemeLight.IsChecked = theme == AppTheme.Light;
        ThemeDark.IsChecked = theme == AppTheme.Dark;
        _loading = false;

        ProgressInfo.Text = $"Aulas concluídas: {_progress.LessonsDone} · Desafios: {_progress.ChallengesDone}";
        WindowInfo.Text = $"Janelas abertas: {Application.Current?.Windows.Count ?? 0}. Esta janela: {Window?.Width:F0}×{Window?.Height:F0}";

        var display = DeviceDisplay.Current.MainDisplayInfo;
        EnvInfo.Text =
            $"Plataforma:  {DeviceInfo.Current.Platform} {DeviceInfo.Current.VersionString}\n" +
            $"Idiom:       {DeviceInfo.Current.Idiom}\n" +
            $"Fabricante:  {DeviceInfo.Current.Manufacturer} · {DeviceInfo.Current.Model}\n" +
            $"Tela:        {display.Width}×{display.Height} px · densidade {display.Density}\n" +
            $"Tema do SO:  {Application.Current?.PlatformAppTheme}\n" +
            $".NET:        {Environment.Version}\n" +
            $"App:         {AppInfo.Current.Name} {AppInfo.Current.VersionString}";
    }

    private void OnThemeChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (_loading || !e.Value) return;
        AppMenu.SetTheme(sender == ThemeLight ? AppTheme.Light : sender == ThemeDark ? AppTheme.Dark : AppTheme.Unspecified);
    }

    private void OnNewWindow(object? sender, EventArgs e) => AppMenu.OpenNewWindow();

    private async void OnReset(object? sender, EventArgs e)
    {
        if (await DisplayAlertAsync("Zerar progresso", "Tem certeza? Isso não pode ser desfeito.", "Zerar", "Cancelar"))
        {
            _progress.ResetAll();
            OnAppearing();
        }
    }
}
