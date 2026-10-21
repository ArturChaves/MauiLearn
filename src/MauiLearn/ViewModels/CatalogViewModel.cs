using System.Collections.ObjectModel;
using MauiLearn.Catalog;
using MauiLearn.Core;

namespace MauiLearn.ViewModels;

/// <summary>Filtro de categoria exibido como "chip".</summary>
public sealed class CategoryFilter(string name, string icon, ComponentCategory? category, bool favorites = false) : ObservableObject
{
    private bool _isSelected;

    public string Name { get; } = name;
    public string Icon { get; } = icon;
    public ComponentCategory? Category { get; } = category;
    public bool Favorites { get; } = favorites;
    public string Label => $"{Icon} {Name}";

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

/// <summary>
/// ViewModel do catálogo: busca + filtro por categoria. A página só faz binding nestas
/// propriedades — a lógica fica aqui, testável e sem dependência de UI (MVVM).
/// </summary>
public sealed class CatalogViewModel : ObservableObject
{
    private readonly ProgressService _progress;
    private string _searchText = string.Empty;
    private CategoryFilter _selected;
    private int _resultCount;

    public CatalogViewModel(ProgressService progress)
    {
        _progress = progress;
        Filters.Add(new CategoryFilter("Todos", "✨", null) { IsSelected = true });
        Filters.Add(new CategoryFilter("Favoritos", "⭐", null, favorites: true));
        foreach (var c in Enum.GetValues<ComponentCategory>())
            Filters.Add(new CategoryFilter(ComponentCatalog.CategoryName(c), ComponentCatalog.CategoryIcon(c), c));
        _selected = Filters[0];

        SelectFilterCommand = new RelayCommand(p =>
        {
            if (p is not CategoryFilter f) return;
            _selected.IsSelected = false;
            _selected = f;
            f.IsSelected = true;
            Apply();
        });

        Apply();
    }

    public ObservableCollection<CategoryFilter> Filters { get; } = [];

    public ObservableCollection<ComponentGroup> Groups { get; } = [];

    public RelayCommand SelectFilterCommand { get; }

    public int TotalCount => ComponentCatalog.All.Count;

    public int ResultCount
    {
        get => _resultCount;
        private set => SetProperty(ref _resultCount, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                Apply();
        }
    }

    public void Refresh() => Apply();

    private void Apply()
    {
        var q = _searchText.Trim();
        var items = ComponentCatalog.All.Where(c =>
            (_selected.Category is null || c.Category == _selected.Category) &&
            (!_selected.Favorites || _progress.IsFavorite(c.Id)) &&
            (q.Length == 0
             || c.Name.Contains(q, StringComparison.OrdinalIgnoreCase)
             || c.Summary.Contains(q, StringComparison.OrdinalIgnoreCase)
             || c.KeyProperties.Any(p => p.Name.Contains(q, StringComparison.OrdinalIgnoreCase))));

        Groups.Clear();
        var count = 0;
        foreach (var g in items.GroupBy(c => c.Category).OrderBy(g => g.Key))
        {
            var group = new ComponentGroup(g.Key, g);
            count += group.Count;
            Groups.Add(group);
        }
        ResultCount = count;
    }
}
