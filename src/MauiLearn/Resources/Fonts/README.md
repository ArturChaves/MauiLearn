# Fontes

Coloque arquivos `.ttf`/`.otf` aqui. Todos são incluídos via `<MauiFont Include="Resources\Fonts\*" />`
no `.csproj`. Depois registre em `MauiProgram.cs`:

```csharp
.ConfigureFonts(fonts =>
{
    fonts.AddFont("MinhaFonte-Regular.ttf", "MinhaFonte");
});
```

E use em XAML: `<Label FontFamily="MinhaFonte" />`.

> O MauiLearn usa as fontes do sistema (Segoe UI no Windows, SF Pro no macOS) para não
> depender de arquivos binários. Veja a Aula "Recursos, Estilos e Temas".
