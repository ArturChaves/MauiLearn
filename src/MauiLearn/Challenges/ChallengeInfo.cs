namespace MauiLearn.Challenges;

/// <summary>Resultado de uma verificação automática do desafio.</summary>
public sealed record CheckResult(bool Passed, string Message);

/// <summary>Uma verificação: descrição + função que inspeciona a área de trabalho ao vivo.</summary>
public sealed record ChallengeCheck(string Description, Func<ContentView, CheckResult> Run);

/// <summary>
/// Um desafio: você edita o arquivo XAML da área de trabalho (com o app rodando),
/// salva, vê a mudança via Hot Reload e clica em "Verificar".
/// </summary>
public sealed class ChallengeInfo(string id, int number, string title, string difficulty, string file, Func<ContentView> createWorkspace)
{
    public string Id { get; } = id;
    public int Number { get; } = number;
    public string Title { get; } = title;
    public string Difficulty { get; } = difficulty;
    public string File { get; } = file;
    public Func<ContentView> CreateWorkspace { get; } = createWorkspace;
    public string Goal { get; private set; } = string.Empty;
    public string? LessonId { get; private set; }
    public List<string> Steps { get; } = [];
    public List<string> Hints { get; } = [];
    public List<ChallengeCheck> Checks { get; } = [];
    public string Solution { get; private set; } = string.Empty;

    public string NumberText => Number.ToString("00");
    public string FilePath => $"src/MauiLearn/{File}";

    public ChallengeInfo WithGoal(string goal) { Goal = goal; return this; }
    public ChallengeInfo Step(string step) { Steps.Add(step); return this; }
    public ChallengeInfo Hint(string hint) { Hints.Add(hint); return this; }
    public ChallengeInfo Check(string description, Func<ContentView, CheckResult> run) { Checks.Add(new(description, run)); return this; }
    public ChallengeInfo WithSolution(string xaml) { Solution = xaml; return this; }
    public ChallengeInfo Lesson(string lessonId) { LessonId = lessonId; return this; }
}

/// <summary>Helpers para escrever verificações de forma legível.</summary>
public static class Verify
{
    public static CheckResult Ok(string message = "OK") => new(true, message);
    public static CheckResult Fail(string message) => new(false, message);

    public static T? Find<T>(ContentView ws, string name) where T : class => ws.FindByName<T>(name);

    public static CheckResult That(bool condition, string ok, string fail) => condition ? Ok(ok) : Fail(fail);

    public static CheckResult Named<T>(ContentView ws, string name, Func<T, CheckResult> next) where T : class
    {
        var element = ws.FindByName(name);
        return element switch
        {
            null => Fail($"Não encontrei um elemento com x:Name=\"{name}\". Não remova/renomeie o x:Name."),
            T typed => next(typed),
            _ => Fail($"\"{name}\" deveria ser {typeof(T).Name}, mas é {element.GetType().Name}."),
        };
    }

    public static bool SameColor(Color? a, Color? b, double tolerance = 0.02) =>
        a is not null && b is not null &&
        Math.Abs(a.Red - b.Red) <= tolerance && Math.Abs(a.Green - b.Green) <= tolerance &&
        Math.Abs(a.Blue - b.Blue) <= tolerance && Math.Abs(a.Alpha - b.Alpha) <= tolerance;

    public static string Hex(Color? c) => c?.ToArgbHex() ?? "null";

    public static IEnumerable<Element> Descendants(Element root)
    {
        foreach (var child in ((IElementController)root).LogicalChildren)
        {
            yield return child;
            foreach (var d in Descendants(child)) yield return d;
        }
    }
}
