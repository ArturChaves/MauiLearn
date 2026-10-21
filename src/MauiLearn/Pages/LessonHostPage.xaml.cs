using MauiLearn.Controls;
using MauiLearn.Catalog;
using MauiLearn.Core;
using MauiLearn.Lessons;

namespace MauiLearn.Pages;

public partial class LessonHostPage : AppPage, IQueryAttributable
{
    private readonly ProgressService _progress = ServiceHelper.Get<ProgressService>();
    private LessonInfo? _lesson;
    private int _correct;
    private int _answered;

    public LessonHostPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var id) && LessonCatalog.Find(id?.ToString()) is { } lesson)
            Load(lesson);
    }

    private void Load(LessonInfo lesson)
    {
        _lesson = lesson;
        Title = $"Aula {lesson.NumberText} — {lesson.Title}";
        NumberLabel.Text = $"AULA {lesson.NumberText}";
        MetaLabel.Text = lesson.Meta;
        IconLabel.Text = lesson.Icon;
        TitleLabel.Text = lesson.Title;
        SummaryLabel.Text = lesson.Summary;

        try
        {
            LessonContent.Content = lesson.Create();
        }
        catch (Exception ex)
        {
            LessonContent.Content = new Label { Text = $"Erro ao carregar a aula: {ex.Message}", TextColor = Colors.Red };
        }

        BuildRelated(lesson);
        BuildQuiz(lesson);

        DoneCheck.IsChecked = _progress.IsLessonDone(lesson.Id);
        var index = LessonCatalog.IndexOf(lesson);
        PrevButton.IsEnabled = index > 0;
        NextButton.IsEnabled = index < LessonCatalog.All.Count - 1;
        _ = Scroll.ScrollToAsync(0, 0, false);
    }

    private void BuildRelated(LessonInfo lesson)
    {
        RelatedChips.Clear();
        foreach (var id in lesson.RelatedComponents)
        {
            if (ComponentCatalog.Find(id) is not { } c) continue;
            var chip = new Button { Text = $"{c.Icon} {c.Name}", Margin = new Thickness(0, 0, 8, 8), Style = (Style)Application.Current!.Resources["SecondaryButton"] };
            chip.Clicked += async (_, _) => await Shell.Current.GoToAsync($"component?id={Uri.EscapeDataString(c.Id)}");
            RelatedChips.Add(chip);
        }
        RelatedSection.IsVisible = RelatedChips.Count > 0;
    }

    private void BuildQuiz(LessonInfo lesson)
    {
        QuizHost.Clear();
        _correct = 0;
        _answered = 0;
        QuizScore.Text = string.Empty;
        QuizSection.IsVisible = lesson.Quiz.Count > 0;

        var n = 1;
        foreach (var q in lesson.Quiz)
        {
            var card = new VerticalStackLayout { Spacing = 6 };
            card.Add(new Label { Text = $"{n++}. {q.Question}", FontAttributes = FontAttributes.Bold, FontSize = 15 });
            var feedback = new Label { IsVisible = false, FontSize = 13 };
            var group = $"q{n}-{lesson.Id}";
            var answered = false;
            for (var i = 0; i < q.Options.Length; i++)
            {
                var index = i;
                var rb = new RadioButton { Content = q.Options[i], GroupName = group };
                rb.CheckedChanged += (_, e) =>
                {
                    if (!e.Value || answered) return;
                    answered = true;
                    _answered++;
                    var right = index == q.CorrectIndex;
                    if (right) _correct++;
                    feedback.IsVisible = true;
                    feedback.Text = (right ? "✅ Correto! " : $"❌ Não. Resposta: \"{q.Options[q.CorrectIndex]}\". ") + q.Explanation;
                    feedback.TextColor = right ? Color.FromArgb("#2E9D5B") : Color.FromArgb("#D64545");
                    UpdateScore(lesson);
                };
                card.Add(rb);
            }
            card.Add(feedback);
            QuizHost.Add(new Border { Style = (Style)Application.Current!.Resources["Card"], Content = card });
        }
    }

    private void UpdateScore(LessonInfo lesson)
    {
        QuizScore.Text = $"Pontuação: {_correct}/{lesson.Quiz.Count}";
        if (_answered == lesson.Quiz.Count && _correct == lesson.Quiz.Count)
        {
            QuizScore.Text += "  🎉 Perfeito! Aula marcada como concluída.";
            DoneCheck.IsChecked = true;
        }
    }

    private void OnDoneChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (_lesson is not null)
            _progress.SetLessonDone(_lesson.Id, e.Value);
    }

    private void OnDoneLabelTapped(object? sender, TappedEventArgs e) => DoneCheck.IsChecked = !DoneCheck.IsChecked;

    private void OnPrev(object? sender, EventArgs e) => Move(-1);

    private void OnNext(object? sender, EventArgs e) => Move(+1);

    private void Move(int delta)
    {
        if (_lesson is null) return;
        var index = LessonCatalog.IndexOf(_lesson) + delta;
        if (index >= 0 && index < LessonCatalog.All.Count)
            Load(LessonCatalog.All[index]);
    }

    private async void OnTop(object? sender, EventArgs e) => await Scroll.ScrollToAsync(0, 0, true);
}
