using MauiLearn.Challenges;
using MauiLearn.Controls;
using MauiLearn.Core;

namespace MauiLearn.Pages;

public partial class ChallengeHostPage : AppPage, IQueryAttributable
{
    private readonly ProgressService _progress = ServiceHelper.Get<ProgressService>();
    private ChallengeInfo? _challenge;
    private ContentView? _workspace;
    private int _hintsShown;

    public ChallengeHostPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var id) && ChallengeCatalog.Find(id?.ToString()) is { } c)
            Load(c);
    }

    private void Load(ChallengeInfo c)
    {
        _challenge = c;
        Title = $"Desafio {c.NumberText}";
        NumberLabel.Text = $"DESAFIO {c.NumberText} · {c.Difficulty.ToUpperInvariant()}";
        TitleLabel.Text = c.Title;
        GoalLabel.Text = c.Goal;
        FileLabel.Text = c.FilePath;

        StepsHost.Clear();
        var i = 1;
        foreach (var step in c.Steps)
            StepsHost.Add(new Label { Text = $"{i++}. {step}", Style = (Style)Application.Current!.Resources["Body"] });

        HintsHost.Clear();
        _hintsShown = 0;
        HintButton.IsVisible = c.Hints.Count > 0;
        SolutionBlock.IsVisible = false;
        SolutionBlock.Code = c.Solution;
        SolutionButton.IsVisible = !string.IsNullOrWhiteSpace(c.Solution);
        LessonButton.IsVisible = c.LessonId is not null;
        ResultsHost.Clear();
        SuccessLabel.IsVisible = _progress.IsChallengeDone(c.Id);
        CreateWorkspace();
    }

    private void CreateWorkspace()
    {
        if (_challenge is null) return;
        try
        {
            _workspace = _challenge.CreateWorkspace();
            WorkspaceHost.Content = _workspace;
            ReloadedLabel.Text = $"criada às {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            WorkspaceHost.Content = new Label { Text = $"Erro ao criar a área de trabalho:\n{ex.Message}", TextColor = Colors.Red };
        }
    }

    private async void OnVerify(object? sender, EventArgs e)
    {
        if (_challenge is null || _workspace is null) return;
        ResultsHost.Clear();
        var all = true;
        foreach (var check in _challenge.Checks)
        {
            CheckResult result;
            try { result = check.Run(_workspace); }
            catch (Exception ex) { result = Verify.Fail($"Erro: {ex.Message}"); }
            all &= result.Passed;
            ResultsHost.Add(new Label
            {
                Text = $"{(result.Passed ? "✅" : "❌")} {check.Description}" + (result.Passed ? string.Empty : $"\n     → {result.Message}"),
                TextColor = result.Passed ? Color.FromArgb("#2E9D5B") : Color.FromArgb("#D64545"),
                FontSize = 13,
            });
        }

        SuccessLabel.IsVisible = all;
        if (all)
        {
            _progress.SetChallengeDone(_challenge.Id);
            await SuccessLabel.ScaleToAsync(1.15, 150, Easing.CubicOut);
            await SuccessLabel.ScaleToAsync(1, 150, Easing.CubicIn);
        }
    }

    private void OnReload(object? sender, EventArgs e) => CreateWorkspace();

    private void OnHint(object? sender, EventArgs e)
    {
        if (_challenge is null || _hintsShown >= _challenge.Hints.Count) return;
        HintsHost.Add(new TipCard { Kind = TipKind.Tip, Header = $"Dica {_hintsShown + 1}", Text = _challenge.Hints[_hintsShown] });
        _hintsShown++;
        HintButton.IsVisible = _hintsShown < _challenge.Hints.Count;
    }

    private async void OnSolution(object? sender, EventArgs e)
    {
        if (!SolutionBlock.IsVisible)
        {
            var sure = await DisplayAlertAsync("Mostrar solução?", "Tente mais um pouco antes — errar faz parte do aprendizado! 😉", "Mostrar", "Vou tentar");
            if (!sure) return;
        }
        SolutionBlock.IsVisible = !SolutionBlock.IsVisible;
        SolutionButton.Text = SolutionBlock.IsVisible ? "🙈 Esconder solução" : "👀 Mostrar solução";
    }

    private async void OnCopyPath(object? sender, EventArgs e)
    {
        if (_challenge is not null)
            await Clipboard.Default.SetTextAsync(_challenge.FilePath);
    }

    private async void OnLesson(object? sender, EventArgs e)
    {
        if (_challenge?.LessonId is { } id)
            await Shell.Current.GoToAsync($"lesson?id={id}");
    }
}
