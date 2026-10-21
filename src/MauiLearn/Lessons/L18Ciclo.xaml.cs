using MauiLearn.Catalog;
using MauiLearn.Core;

namespace MauiLearn.Lessons;

public partial class L18Ciclo : ContentView
{
    private View? _toggled;

    public L18Ciclo()
    {
        InitializeComponent();
        LogList.ItemsSource = AppLog.Entries;
        Loaded += (_, _) => AppLog.Write("Aula: ContentView.Loaded");
        Unloaded += (_, _) => AppLog.Write("Aula: ContentView.Unloaded");
    }

    private async void OnPushPage(object? sender, EventArgs e)
    {
        var page = ComponentCatalog.SamplePage("Página de teste do ciclo de vida — volte com o botão voltar", "#2E9D5B");
        page.NavigatedTo += (_, _) => AppLog.Write("Página filha: NavigatedTo");
        page.Appearing += (_, _) => AppLog.Write("Página filha: Appearing");
        page.Loaded += (_, _) => AppLog.Write("Página filha: Loaded");
        page.Disappearing += (_, _) => AppLog.Write("Página filha: Disappearing");
        page.NavigatedFrom += (_, _) => AppLog.Write("Página filha: NavigatedFrom");
        page.Unloaded += (_, _) => AppLog.Write("Página filha: Unloaded");
        if (Window?.Page is Shell shell)
            await shell.CurrentPage.Navigation.PushAsync(page);
    }

    private void OnToggleElement(object? sender, EventArgs e)
    {
        if (_toggled is null)
        {
            var label = new Label { Text = "👋 Eu sou um elemento novo", FontSize = 18, Padding = 6 };
            label.HandlerChanged += (_, _) => AppLog.Write($"Elemento: HandlerChanged (handler {(label.Handler is null ? "removido" : "criado")})");
            label.Loaded += (_, _) => AppLog.Write("Elemento: Loaded");
            label.Unloaded += (_, _) => AppLog.Write("Elemento: Unloaded");
            _toggled = label;
            ToggleHost.Content = label;
        }
        else
        {
            ToggleHost.Content = null;
            _toggled = null;
        }
    }

    private void OnClear(object? sender, EventArgs e) => AppLog.Entries.Clear();
}
