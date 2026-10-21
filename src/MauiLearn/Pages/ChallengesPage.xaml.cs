using MauiLearn.Controls;
using MauiLearn.Challenges;
using MauiLearn.Core;

namespace MauiLearn.Pages;

public sealed record ChallengeRow(ChallengeInfo Info, bool Done);

public partial class ChallengesPage : AppPage
{
    private readonly ProgressService _progress;

    public ChallengesPage(ProgressService progress)
    {
        InitializeComponent();
        _progress = progress;
        SizeChanged += (_, _) => GridLayout.Span = Width > 1100 ? 2 : 1;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        List.ItemsSource = ChallengeCatalog.All.Select(c => new ChallengeRow(c, _progress.IsChallengeDone(c.Id))).ToList();
    }

    private async void OnSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not ChallengeRow row) return;
        List.SelectedItem = null;
        await Shell.Current.GoToAsync($"challenge?id={row.Info.Id}");
    }
}
