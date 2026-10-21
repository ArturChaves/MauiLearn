namespace MauiLearn.Lessons;

public enum LessonLevel
{
    Basico,
    Intermediario,
    Avancado,
}

/// <summary>Pergunta de múltipla escolha exibida no fim da aula.</summary>
public sealed record QuizQuestion(string Question, string[] Options, int CorrectIndex, string Explanation);

/// <summary>Metadados de uma aula. O conteúdo é um ContentView em Lessons/*.xaml.</summary>
public sealed class LessonInfo(string id, int number, string icon, string title, string summary, LessonLevel level, int minutes, Func<View> create)
{
    public string Id { get; } = id;
    public int Number { get; } = number;
    public string Icon { get; } = icon;
    public string Title { get; } = title;
    public string Summary { get; } = summary;
    public LessonLevel Level { get; } = level;
    public int Minutes { get; } = minutes;
    public Func<View> Create { get; } = create;
    public List<QuizQuestion> Quiz { get; } = [];
    public List<string> RelatedComponents { get; } = [];

    public string NumberText => Number.ToString("00");
    public string LevelText => Level switch
    {
        LessonLevel.Basico => "Básico",
        LessonLevel.Intermediario => "Intermediário",
        _ => "Avançado",
    };
    public string Meta => $"{LevelText} · ~{Minutes} min";

    public LessonInfo Q(string question, int correct, string explanation, params string[] options)
    {
        Quiz.Add(new QuizQuestion(question, options, correct, explanation));
        return this;
    }

    public LessonInfo Related(params string[] componentIds)
    {
        RelatedComponents.AddRange(componentIds);
        return this;
    }
}
