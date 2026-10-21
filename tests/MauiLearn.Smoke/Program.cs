using System.Reflection;
using MauiLearn.Catalog;
using MauiLearn.Challenges;
using MauiLearn.Controls;
using MauiLearn.Core;
using MauiLearn.Lessons;
using MauiLearn.Pages;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Dispatching;
using Microsoft.Extensions.DependencyInjection;

namespace MauiLearn.Smoke;

// Stubs mínimos: sem plataforma real, precisamos de um Dispatcher e de Preferences falsos.
class StubTimer : IDispatcherTimer
{
    public TimeSpan Interval { get; set; }
    public bool IsRepeating { get; set; }
    public bool IsRunning { get; private set; }
    public event EventHandler? Tick;
    public void Start() => IsRunning = true;
    public void Stop() => IsRunning = false;
}
class StubDispatcher : IDispatcher
{
    public bool IsDispatchRequired => false;
    public bool Dispatch(Action action) { action(); return true; }
    public bool DispatchDelayed(TimeSpan delay, Action action) => true;
    public IDispatcherTimer CreateTimer() => new StubTimer();
}
class StubProvider : IDispatcherProvider
{
    static readonly StubDispatcher D = new();
    public IDispatcher? GetForCurrentThread() => D;
}
class FakePrefs : IPreferences
{
    readonly Dictionary<string, object?> _d = [];
    public bool ContainsKey(string key, string? sharedName = null) => _d.ContainsKey(key);
    public void Remove(string key, string? sharedName = null) => _d.Remove(key);
    public void Clear(string? sharedName = null) => _d.Clear();
    public void Set<T>(string key, T value, string? sharedName = null) => _d[key] = value;
    public T Get<T>(string key, T defaultValue, string? sharedName = null) => _d.TryGetValue(key, out var v) && v is T t ? t : defaultValue;
}

public static class Program
{
    static readonly string AppSrc = typeof(Program).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().First(a => a.Key == "AppSrc").Value!;

    // O parser de XAML em tempo de execução só enxerga o xmlns global para tipos do assembly
    // do elemento raiz; aqui o raiz é do próprio MAUI, então usamos o xmlns clássico.
    static string Classic(string xaml) => xaml.Replace("http://schemas.microsoft.com/dotnet/maui/global", "http://schemas.microsoft.com/dotnet/2021/maui");
    static int _fail, _ok;
    static readonly List<string> Failures = [];

    static void Try(string name, Action a)
    {
        try { a(); _ok++; }
        catch (Exception ex)
        {
            var root = ex is TargetInvocationException { InnerException: { } i } ? i : ex;
            _fail++;
            Failures.Add($"❌ {name}: {root.GetType().Name}: {root.Message.Split('\n')[0]}\n      at {root.StackTrace?.Split('\n').FirstOrDefault(l => l.Contains("MauiLearn"))?.Trim()}");
        }
    }

    public static int Main()
    {
        DispatcherProvider.SetCurrent(new StubProvider());
        var services = new ServiceCollection();
        services.AddSingleton(new ProgressService(new FakePrefs()));
        ServiceHelper.Initialize(services.BuildServiceProvider());
        var progress = ServiceHelper.Get<ProgressService>();

        var src = AppSrc;
        var app = new Application();
        Application.SetCurrentApplication(app);
        Try("Colors.xaml", () => app.Resources.MergedDictionaries.Add(new ResourceDictionary().LoadFromXaml(Classic(File.ReadAllText(src + "Resources/Styles/Colors.xaml")))));
        Try("Styles.xaml", () => app.Resources.MergedDictionaries.Add(new ResourceDictionary().LoadFromXaml(Classic(File.ReadAllText(src + "Resources/Styles/Styles.xaml")))));
        app.Resources["BoolToText"] = new MauiLearn.Converters.BoolToTextConverter();
        app.Resources["InvertBool"] = new MauiLearn.Converters.InvertBoolConverter();
        app.Resources["IsNotEmpty"] = new MauiLearn.Converters.IsNotNullOrEmptyConverter();
        app.Resources["DoneGlyph"] = new MauiLearn.Converters.DoneToGlyphConverter();

        Try("AppShell", () => _ = new MauiLearn.AppShell());

        foreach (var l in LessonCatalog.All)
            Try($"Lesson {l.Id}", () => _ = l.Create());

        foreach (var c in ComponentCatalog.All)
        {
            Try($"Component {c.Id}", () =>
            {
                BindableObject target;
                if (c.DemoFactory is not null) target = c.DemoFactory().Target;
                else target = c.PageFactory!();
                var inspector = new PropertyInspectorView();
                inspector.Inspect(target, c.KeyProperties.ToDictionary(p => p.Name, p => p.Description));
                _ = inspector.GenerateXaml();
                foreach (var p in BindablePropertyReflector.GetProperties(target))
                {
                    var editor = PropertyEditorFactory.Create(p, target, (v, x) => { });
                    editor?.Refresh(target.GetValue(p.Property));
                }
                new EventLogView().Attach(target);
            });
        }

        Try("HomePage", () => _ = new HomePage(progress));
        Try("CatalogPage", () => { var p = new CatalogPage(progress); p.ApplyQueryAttributes(new Dictionary<string, object> { ["focus"] = "search" }); });
        Try("LessonsPage", () => _ = new LessonsPage(progress));
        Try("ChallengesPage", () => _ = new ChallengesPage(progress));
        Try("SettingsPage", () => _ = new SettingsPage(progress));

        foreach (var c in ComponentCatalog.All)
            Try($"DetailPage {c.Id}", () =>
            {
                var page = new ComponentDetailPage();
                page.ApplyQueryAttributes(new Dictionary<string, object> { ["id"] = c.Id });
                var show = typeof(ComponentDetailPage).GetMethod("ShowTab", BindingFlags.NonPublic | BindingFlags.Instance)!;
                foreach (var tab in new[] { "about", "props", "nuances", "code", "events" }) show.Invoke(page, [tab]);
            });

        foreach (var l in LessonCatalog.All)
            Try($"LessonHost {l.Id}", () => new LessonHostPage().ApplyQueryAttributes(new Dictionary<string, object> { ["id"] = l.Id }));

        foreach (var ch in ChallengeCatalog.All)
            Try($"Challenge {ch.Id}", () =>
            {
                var ws = ch.CreateWorkspace();
                foreach (var check in ch.Checks)
                {
                    var r = check.Run(ws);
                    if (r.Passed) Console.WriteLine($"   ℹ️ {ch.Id}: '{check.Description}' já é verdadeira no estado inicial (ok: outras verificações falham)");
                }
                var host = new ChallengeHostPage();
                host.ApplyQueryAttributes(new Dictionary<string, object> { ["id"] = ch.Id });
            });

        // Valida que cada solução, aplicada ao workspace, passa nas verificações
        foreach (var ch in ChallengeCatalog.All)
            Try($"Solution {ch.Id}", () => SolutionCheck(ch));

        // Testa as amostras do Playground com o parser de runtime
        var samples = (Array)typeof(PlaygroundPage).GetField("Samples", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
        foreach (var s in samples)
        {
            var (name, xaml) = ((string, string))s!;
            Try($"Playground '{name}'", () => _ = new ContentView().LoadFromXaml($"<ContentView xmlns=\"http://schemas.microsoft.com/dotnet/2021/maui\" xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\">{xaml}</ContentView>"));
        }

        Console.WriteLine($"\nOK: {_ok}  FALHAS: {_fail}");
        Console.WriteLine("(Falhas de 'NotImplementedInReferenceAssembly' indicam APIs de plataforma — esperadas fora do Windows/Mac.)");
        foreach (var f in Failures) Console.WriteLine(f);
        return _fail == 0 ? 0 : 1;
    }

    // Monta um XAML "resolvido" a partir do arquivo original + solução e roda as verificações.
    static void SolutionCheck(ChallengeInfo ch)
    {
        var file = File.ReadAllText(AppSrc + ch.File);
        var solved = SolvedXaml.For(ch.Id, file, ch.Solution);
        if (solved is null) { Console.WriteLine($"   (sem aplicação automática da solução de {ch.Id})"); return; }
        var ws = new ContentView().LoadFromXaml(Classic(solved));
        if (ch.Id == "d12") ws.BindingContext = new MauiLearn.ViewModels.CounterViewModel();
        foreach (var check in ch.Checks)
        {
            var r = check.Run(ws);
            if (!r.Passed) throw new Exception($"Solução não passa em '{check.Description}': {r.Message}");
        }
    }
}
