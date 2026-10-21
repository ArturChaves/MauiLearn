using MauiLearn.Controls;
using MauiLearn.Catalog;
using MauiLearn.Challenges;
using MauiLearn.Core;
using MauiLearn.Lessons;

namespace MauiLearn.Pages;

public partial class HomePage : AppPage
{
    private readonly ProgressService _progress;

    // Injeção de dependência: o Shell cria a página via DI (registrada em MauiProgram).
    public HomePage(ProgressService progress)
    {
        InitializeComponent();
        _progress = progress;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var lessons = LessonCatalog.All.Count;
        var challenges = ChallengeCatalog.All.Count;
        LessonsStat.Text = $"{_progress.LessonsDone} / {lessons}";
        LessonsBar.Progress = lessons == 0 ? 0 : (double)_progress.LessonsDone / lessons;
        ChallengesStat.Text = $"{_progress.ChallengesDone} / {challenges}";
        ChallengesBar.Progress = challenges == 0 ? 0 : (double)_progress.ChallengesDone / challenges;
        ComponentsStat.Text = ComponentCatalog.All.Count.ToString();
    }

    private async void OnStartClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync($"lesson?id={LessonCatalog.All[0].Id}");

    private async void OnCatalogClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("//catalog");

    private async void OnNewsClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("lesson?id=novidades");

    // Um "easter egg" que demonstra animações combinadas (Task.WhenAll).
    private async void OnLogoTapped(object? sender, TappedEventArgs e)
    {
        await Task.WhenAll(Logo.RotateToAsync(360, 600, Easing.CubicInOut), Logo.ScaleToAsync(1.2, 300));
        await Logo.ScaleToAsync(1, 300, Easing.SpringOut);
        Logo.Rotation = 0;
    }
}
