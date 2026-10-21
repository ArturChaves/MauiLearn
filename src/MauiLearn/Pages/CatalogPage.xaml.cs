using MauiLearn.Controls;
using MauiLearn.Catalog;
using MauiLearn.Core;
using MauiLearn.ViewModels;

namespace MauiLearn.Pages;

public partial class CatalogPage : AppPage, IQueryAttributable
{
    private readonly CatalogViewModel _vm;

    public CatalogPage(ProgressService progress)
    {
        InitializeComponent();
        BindingContext = _vm = new CatalogViewModel(progress);

        // Desktop: ajusta o número de colunas conforme a largura da janela.
        SizeChanged += (_, _) => GridLayout.Span = Math.Max(1, (int)(Width / 330));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.Refresh(); // favoritos podem ter mudado
    }

    // Recebe parâmetros de navegação: GoToAsync("//catalog?focus=search")
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("focus", out var f) && f as string == "search")
            Dispatcher.Dispatch(() => Search.Focus());
    }

    private async void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not ComponentInfo info)
            return;

        Items.SelectedItem = null; // permite clicar de novo no mesmo item
        await Shell.Current.GoToAsync($"component?id={Uri.EscapeDataString(info.Id)}");
    }
}
