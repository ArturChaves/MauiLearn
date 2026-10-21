using MauiLearn.Catalog;

namespace MauiLearn.Lessons;

public partial class L10Navegacao : ContentView
{
    public L10Navegacao()
    {
        InitializeComponent();
    }

    private Page Page => Window?.Page is Shell shell ? shell.CurrentPage : Window?.Page ?? Application.Current!.Windows[0].Page!;

    private async void OnGoCatalog(object? sender, EventArgs e) => await Shell.Current.GoToAsync("//catalog");

    private async void OnGoSlider(object? sender, EventArgs e) => await Shell.Current.GoToAsync("component?id=Slider");

    private async void OnGoWithDictionary(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("component", new Dictionary<string, object> { ["id"] = "CollectionView" });

    private async void OnPush(object? sender, EventArgs e) =>
        await Page.Navigation.PushAsync(ComponentCatalog.SamplePage("Página empilhada (PushAsync) — use o botão voltar", "#2E9D5B"));

    private async void OnModal(object? sender, EventArgs e) =>
        await Page.Navigation.PushModalAsync(ComponentCatalog.SamplePage("Página modal (PushModalAsync)", "#D600AA"));

    private void OnWindow(object? sender, EventArgs e) =>
        Application.Current?.OpenWindow(new Window(ComponentCatalog.SamplePage("Nova janela!", "#2F80ED")) { Width = 700, Height = 450 });

    private async void OnAlert(object? sender, EventArgs e)
    {
        var ok = await Page.DisplayAlertAsync("DisplayAlertAsync", "Você está gostando do MauiLearn?", "Sim!", "Ainda não");
        DialogResult.Text = $"Resposta: {(ok ? "Sim" : "Ainda não")}";
    }

    private async void OnSheet(object? sender, EventArgs e)
    {
        var choice = await Page.DisplayActionSheetAsync("Qual seu layout favorito?", "Cancelar", null, "Grid", "VerticalStackLayout", "FlexLayout");
        DialogResult.Text = $"Escolha: {choice ?? "(nenhuma)"}";
    }

    private async void OnPrompt(object? sender, EventArgs e)
    {
        var name = await Page.DisplayPromptAsync("DisplayPromptAsync", "Como você se chama?", "OK", "Cancelar", "Seu nome");
        DialogResult.Text = string.IsNullOrWhiteSpace(name) ? "Cancelado" : $"Olá, {name}!";
    }
}
