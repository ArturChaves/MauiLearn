using System.Collections.ObjectModel;
using MauiLearn.Catalog;

namespace MauiLearn.Lessons;

public partial class L11Colecoes : ContentView
{
    private readonly ObservableCollection<Fruit> _fruits = new(ComponentCatalog.Fruits());
    private readonly ObservableCollection<string> _infinite = [];
    private static readonly string[] Layouts = ["VerticalList", "HorizontalList", "VerticalGrid, 2", "VerticalGrid, 3"];

    public L11Colecoes()
    {
        InitializeComponent();
        Fruits.ItemsSource = _fruits;

        LayoutPicker.ItemsSource = Layouts;
        LayoutPicker.SelectedIndex = 2;
        SelectionPicker.ItemsSource = Enum.GetNames<SelectionMode>();
        SelectionPicker.SelectedIndex = (int)SelectionMode.Single;

        Grouped.ItemsSource = ComponentCatalog.Fruits()
            .GroupBy(f => f.Price switch { < 4 => "💰 Até R$ 4", < 8 => "💰💰 R$ 4 a R$ 8", _ => "💰💰💰 Acima de R$ 8" })
            .Select(g => new FruitGroup(g.Key, g))
            .ToList();

        Infinite.ItemsSource = _infinite;
        LoadMore();
    }

    private void OnLayoutChanged(object? sender, EventArgs e)
    {
        Fruits.ItemsLayout = LayoutPicker.SelectedIndex switch
        {
            0 => new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 6 },
            1 => new LinearItemsLayout(ItemsLayoutOrientation.Horizontal) { ItemSpacing = 6 },
            3 => new GridItemsLayout(3, ItemsLayoutOrientation.Vertical) { HorizontalItemSpacing = 6, VerticalItemSpacing = 6 },
            _ => new GridItemsLayout(2, ItemsLayoutOrientation.Vertical) { HorizontalItemSpacing = 6, VerticalItemSpacing = 6 },
        };
    }

    private void OnSelectionModeChanged(object? sender, EventArgs e)
    {
        if (SelectionPicker.SelectedIndex >= 0)
            Fruits.SelectionMode = (SelectionMode)SelectionPicker.SelectedIndex;
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var names = e.CurrentSelection.OfType<Fruit>().Select(f => f.Name).ToList();
        SelectionLabel.Text = names.Count == 0 ? "Nada selecionado" : $"Selecionado(s): {string.Join(", ", names)}";
    }

    private void OnAdd(object? sender, EventArgs e)
    {
        var sample = ComponentCatalog.Fruits()[Random.Shared.Next(10)];
        _fruits.Add(sample with { Name = $"{sample.Name} #{_fruits.Count + 1}", Price = Math.Round(Random.Shared.NextDouble() * 12, 2) });
    }

    private void OnRemove(object? sender, EventArgs e)
    {
        if (_fruits.Count > 0) _fruits.RemoveAt(_fruits.Count - 1);
    }

    private void OnClear(object? sender, EventArgs e) => _fruits.Clear();

    private void OnScrollLast(object? sender, EventArgs e)
    {
        if (_fruits.Count > 0)
            Fruits.ScrollTo(_fruits.Count - 1, position: ScrollToPosition.End, animate: true);
    }

    private void OnLoadMore(object? sender, EventArgs e) => LoadMore();

    private void LoadMore()
    {
        var start = _infinite.Count;
        for (var i = 1; i <= 20; i++) _infinite.Add($"Item {start + i}");
        InfiniteLabel.Text = $"{_infinite.Count} itens carregados — role até o fim para carregar mais 20";
    }
}
