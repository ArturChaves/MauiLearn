namespace MauiLearn.Core;

/// <summary>
/// Acesso ao container de injeção de dependência fora de construtores
/// (ex.: páginas criadas por rotas). Prefira injeção via construtor quando possível.
/// </summary>
public static class ServiceHelper
{
    public static IServiceProvider Services { get; private set; } = default!;

    public static void Initialize(IServiceProvider services) => Services = services;

    public static T Get<T>() where T : notnull => Services.GetRequiredService<T>();
}
