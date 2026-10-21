namespace MauiLearn.Catalog;

public enum ComponentCategory
{
    Layouts,
    Text,
    Input,
    Commands,
    Collections,
    Graphics,
    Gestures,
    Pages,
    Desktop,
}

public enum ComponentStatus
{
    Stable,
    New,
    Obsolete,
}

/// <summary>Documentação de uma propriedade importante do componente.</summary>
public sealed record PropertyDoc(string Name, string Description);

/// <summary>Documentação de um evento do componente.</summary>
public sealed record EventDoc(string Name, string Description);

/// <summary>
/// O que o detalhe do componente exibe: Preview é a área visual; Target é o objeto
/// inspecionado (geralmente o próprio controle, mas pode ser um gesto, um pincel...).
/// </summary>
public sealed record DemoInstance(View Preview, BindableObject Target);

/// <summary>
/// Descrição de um componente do .NET MAUI no catálogo.
/// Criado com a API fluente de <see cref="ComponentCatalog"/>.
/// </summary>
public sealed class ComponentInfo
{
    public ComponentInfo(string name, ComponentCategory category, string icon, string summary)
    {
        Name = name;
        Id = name;
        Category = category;
        Icon = icon;
        Summary = summary;
    }

    public string Id { get; private set; }
    public string Name { get; }
    public ComponentCategory Category { get; }
    public string Icon { get; }
    public string Summary { get; }
    public string Description { get; private set; } = string.Empty;
    public ComponentStatus Status { get; private set; } = ComponentStatus.Stable;
    public string? StatusNote { get; private set; }
    public string? WindowsView { get; private set; }
    public string? MacView { get; private set; }
    public string? DocsUrl { get; private set; }
    public string? LessonId { get; private set; }
    public string Xaml { get; private set; } = string.Empty;
    public string? CSharp { get; private set; }
    public Func<DemoInstance>? DemoFactory { get; private set; }
    public Func<Page>? PageFactory { get; private set; }
    public List<PropertyDoc> KeyProperties { get; } = [];
    public List<EventDoc> Events { get; } = [];
    public List<string> Nuances { get; } = [];

    public string CategoryName => ComponentCatalog.CategoryName(Category);
    public bool IsObsolete => Status == ComponentStatus.Obsolete;
    public bool IsNew => Status == ComponentStatus.New;
    public string Badge => Status switch
    {
        ComponentStatus.New => "NOVO",
        ComponentStatus.Obsolete => "OBSOLETO",
        _ => string.Empty,
    };

    // ---------- API fluente ----------

    public ComponentInfo WithId(string id) { Id = id; return this; }

    public ComponentInfo About(string description) { Description = description; return this; }

    public ComponentInfo Demo(Func<View> factory)
    {
        DemoFactory = () => { var v = factory(); return new DemoInstance(v, v); };
        return this;
    }

    public ComponentInfo Demo(Func<DemoInstance> factory) { DemoFactory = factory; return this; }

    public ComponentInfo PageDemo(Func<Page> factory) { PageFactory = factory; return this; }

    public ComponentInfo Prop(string name, string description) { KeyProperties.Add(new(name, description)); return this; }

    public ComponentInfo Event(string name, string description) { Events.Add(new(name, description)); return this; }

    public ComponentInfo Nuance(string text) { Nuances.Add(text); return this; }

    public ComponentInfo Sample(string xaml) { Xaml = xaml; return this; }

    public ComponentInfo Code(string csharp) { CSharp = csharp; return this; }

    public ComponentInfo Platform(string windows, string mac) { WindowsView = windows; MacView = mac; return this; }

    public ComponentInfo Docs(string relativeUrl)
    {
        DocsUrl = relativeUrl.StartsWith("http") ? relativeUrl : "https://learn.microsoft.com/dotnet/maui/" + relativeUrl;
        return this;
    }

    public ComponentInfo Lesson(string lessonId) { LessonId = lessonId; return this; }

    public ComponentInfo New(string? note = null) { Status = ComponentStatus.New; StatusNote = note; return this; }

    public ComponentInfo Obsolete(string note) { Status = ComponentStatus.Obsolete; StatusNote = note; return this; }
}

/// <summary>Grupo para CollectionView agrupada (IsGrouped="True"): uma lista com cabeçalho.</summary>
public sealed class ComponentGroup(ComponentCategory category, IEnumerable<ComponentInfo> items) : List<ComponentInfo>(items)
{
    public ComponentCategory Category { get; } = category;
    public string Name { get; } = ComponentCatalog.CategoryName(category);
    public string Icon { get; } = ComponentCatalog.CategoryIcon(category);
}
