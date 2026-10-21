using MauiLearn.Core;
using MauiLearn.Pages;
using Microsoft.Extensions.Logging;

namespace MauiLearn;

/// <summary>
/// Ponto de entrada multiplataforma. Cada plataforma (Platforms/*) chama CreateMauiApp().
/// Aqui configuramos: o App, fontes, handlers, serviços (injeção de dependência) e logging.
/// </summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                // Exemplo: fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                // Veja Resources/Fonts/README.md
            })
            .ConfigureMauiHandlers(handlers =>
            {
                // Aqui você poderia registrar handlers próprios:
                // handlers.AddHandler<MeuControle, MeuControleHandler>();
            });

        // Customização global de handlers (veja a aula "Handlers"):
        HandlerCustomizations.Apply();

        // Injeção de dependência: serviços e páginas.
        builder.Services.AddSingleton<ProgressService>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<CatalogPage>();
        builder.Services.AddTransient<LessonsPage>();
        builder.Services.AddTransient<ChallengesPage>();
        builder.Services.AddTransient<PlaygroundPage>();
        builder.Services.AddTransient<SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        ServiceHelper.Initialize(app.Services);
        return app;
    }
}
