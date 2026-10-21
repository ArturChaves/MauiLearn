// .NET MAUI 10+/11: "xmlns global".
// Ao registrar nossos namespaces C# no URI global, todo XAML que declara
//   xmlns="http://schemas.microsoft.com/dotnet/maui/global"
// enxerga os controles do MAUI E os nossos — sem precisar de xmlns:local="clr-namespace:...".
// Compare com o estilo clássico: xmlns:controls="clr-namespace:MauiLearn.Controls".

// 1) Agrega o xmlns padrão do MAUI (todos os controles) ao xmlns global.
//    O XAML Source Generator já faz isso implicitamente no build, mas o parser em tempo
//    de execução (LoadFromXaml, Hot Reload "Legacy") precisa desta declaração.
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "http://schemas.microsoft.com/dotnet/2021/maui")]

// 2) Nossos namespaces C#.

[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "MauiLearn")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "MauiLearn.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "MauiLearn.Converters")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "MauiLearn.Core")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "MauiLearn.Catalog")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "MauiLearn.ViewModels")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "MauiLearn.Lessons")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "MauiLearn.Challenges")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "MauiLearn.Challenges.Workspaces")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "MauiLearn.Pages")]
