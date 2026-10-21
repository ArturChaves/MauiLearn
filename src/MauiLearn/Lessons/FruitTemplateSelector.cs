using MauiLearn.Catalog;

namespace MauiLearn.Lessons;

/// <summary>Escolhe o template de cada item: frutas "premium" (&gt; R$ 7) ganham destaque.</summary>
public class FruitTemplateSelector : DataTemplateSelector
{
    public DataTemplate? Cheap { get; set; }
    public DataTemplate? Expensive { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container) =>
        item is Fruit { Price: > 7 } ? Expensive! : Cheap!;
}

/// <summary>Um grupo de frutas para a CollectionView agrupada.</summary>
public sealed class FruitGroup(string name, IEnumerable<Fruit> items) : List<Fruit>(items)
{
    public string Name { get; } = name;
}
