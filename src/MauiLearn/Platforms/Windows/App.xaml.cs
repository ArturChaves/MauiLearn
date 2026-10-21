using Microsoft.UI.Xaml;

// Este App.xaml é do WinUI 3 (não do MAUI). Ele apenas inicializa o MAUI.
namespace MauiLearn.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        this.InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
