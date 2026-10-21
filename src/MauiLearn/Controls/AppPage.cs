using MauiLearn.Core;

namespace MauiLearn.Controls;

/// <summary>
/// Página base do app. Adiciona a barra de menus de desktop (MenuBar) com atalhos de teclado.
///
/// Desktop: Page.MenuBarItems vira o menu da janela no Windows e o menu global
/// (barra superior) no macOS. KeyboardAccelerator define atalhos (Ctrl no Windows, ⌘ no Mac).
/// </summary>
public class AppPage : ContentPage
{
    public AppPage()
    {
        AppMenu.Attach(this);
    }
}

public static class AppMenu
{
    private static KeyboardAcceleratorModifiers Primary =>
        OperatingSystem.IsMacCatalyst() ? KeyboardAcceleratorModifiers.Cmd : KeyboardAcceleratorModifiers.Ctrl;

    public static void Attach(Page page)
    {
        var navigate = new MenuBarItem { Text = "Navegar" };
        navigate.Add(Item("Início", "1", () => Go("//home")));
        navigate.Add(Item("Aulas", "2", () => Go("//lessons")));
        navigate.Add(Item("Catálogo de componentes", "3", () => Go("//catalog")));
        navigate.Add(Item("Desafios", "4", () => Go("//challenges")));
        navigate.Add(Item("Playground XAML", "5", () => Go("//playground")));
        navigate.Add(new MenuFlyoutSeparator());
        navigate.Add(Item("Buscar componente…", "K", () => Go("//catalog?focus=search")));

        var view = new MenuBarItem { Text = "Exibir" };
        var theme = new MenuFlyoutSubItem { Text = "Tema" };
        theme.Add(Item("Claro", null, () => SetTheme(AppTheme.Light)));
        theme.Add(Item("Escuro", null, () => SetTheme(AppTheme.Dark)));
        theme.Add(Item("Seguir o sistema", null, () => SetTheme(AppTheme.Unspecified)));
        view.Add(theme);
        view.Add(Item("Nova janela", "N", OpenNewWindow, shift: true));

        var help = new MenuBarItem { Text = "Ajuda" };
        help.Add(Item("Documentação oficial do .NET MAUI", null,
            () => _ = Launcher.Default.OpenAsync("https://learn.microsoft.com/dotnet/maui/")));
        help.Add(Item("Sobre o MauiLearn", null, () =>
            _ = Shell.Current?.CurrentPage?.DisplayAlertAsync("MauiLearn",
                "Uma biblioteca interativa do .NET MAUI 11, feita em .NET MAUI.", "OK")));

        page.MenuBarItems.Add(navigate);
        page.MenuBarItems.Add(view);
        page.MenuBarItems.Add(help);
    }

    private static MenuFlyoutItem Item(string text, string? key, Action action, bool shift = false)
    {
        var item = new MenuFlyoutItem { Text = text };
        item.Clicked += (_, _) => action();
        if (key is not null)
        {
            item.KeyboardAccelerators.Add(new KeyboardAccelerator
            {
                Modifiers = shift ? Primary | KeyboardAcceleratorModifiers.Shift : Primary,
                Key = key,
            });
        }
        return item;
    }

    private static void Go(string route) => _ = Shell.Current?.GoToAsync(route);

    public static void SetTheme(AppTheme theme)
    {
        if (Application.Current is null) return;
        Application.Current.UserAppTheme = theme;
        Preferences.Default.Set(SettingsKeys.Theme, (int)theme);
    }

    public static void OpenNewWindow()
    {
        // Múltiplas janelas: cada Window precisa de uma Page própria.
        Application.Current?.OpenWindow(new Window(new AppShell()) { Width = 1100, Height = 760 });
    }
}
