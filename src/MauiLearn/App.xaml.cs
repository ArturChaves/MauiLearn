using MauiLearn.Core;

namespace MauiLearn;

public partial class App : Application
{
    private readonly AppShell _shell;

    public App(AppShell shell)
    {
        InitializeComponent();
        _shell = shell;

        // Restaura o tema escolhido em Configurações (Preferences = armazenamento chave/valor).
        UserAppTheme = (AppTheme)Preferences.Default.Get(SettingsKeys.Theme, (int)AppTheme.Unspecified);
    }

    /// <summary>
    /// Desde o .NET MAUI 9, a forma recomendada é sobrescrever CreateWindow
    /// (Application.MainPage está obsoleto). No desktop, Window controla título,
    /// tamanho, posição e eventos de ciclo de vida.
    /// </summary>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        // A 1ª janela usa o Shell vindo da DI. Janelas extras (ex.: "Nova janela" do macOS)
        // precisam de uma instância nova — uma Page só pode pertencer a uma Window.
        var shell = _shell.Window is null ? _shell : new AppShell();
        var window = new Window(shell)
        {
            Title = "MauiLearn — Biblioteca interativa do .NET MAUI 11",
            Width = 1280,
            Height = 840,
            MinimumWidth = 900,
            MinimumHeight = 600,
        };

        window.Created += (_, _) => AppLog.Write("Window.Created");
        window.Activated += (_, _) => AppLog.Write("Window.Activated");
        window.Deactivated += (_, _) => AppLog.Write("Window.Deactivated");
        window.Stopped += (_, _) => AppLog.Write("Window.Stopped");
        window.Resumed += (_, _) => AppLog.Write("Window.Resumed");
        window.Destroying += (_, _) => AppLog.Write("Window.Destroying");

        return window;
    }
}
