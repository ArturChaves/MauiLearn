using MauiLearn.Controls;
using MauiLearn.Core;
using MauiLearn.Lessons;

namespace MauiLearn.Pages;

/// <summary>Linha da lista: a aula + se já foi concluída.</summary>
public sealed record LessonRow(LessonInfo Info, bool Done);

public partial class LessonsPage : AppPage
{
    private readonly ProgressService _progress;

    public LessonsPage(ProgressService progress)
    {
        InitializeComponent();
        _progress = progress;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var rows = LessonCatalog.All.Select(l => new LessonRow(l, _progress.IsLessonDone(l.Id))).ToList();
        List.ItemsSource = rows;
        var done = rows.Count(r => r.Done);
        Progress.Progress = rows.Count == 0 ? 0 : (double)done / rows.Count;
        ProgressText.Text = $"{done} de {rows.Count} aulas concluídas";
    }

    private async void OnSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not LessonRow row) return;
        List.SelectedItem = null;
        await Shell.Current.GoToAsync($"lesson?id={row.Info.Id}");
    }
}
