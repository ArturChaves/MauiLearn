using MauiLearn.Challenges;
using MauiLearn.Core;
using MauiLearn.Pages;

namespace MauiLearn;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Rotas "de detalhe": não aparecem no flyout, mas podem ser navegadas por URI:
        //   await Shell.Current.GoToAsync($"component?id=Button");
        Routing.RegisterRoute("component", typeof(ComponentDetailPage));
        Routing.RegisterRoute("challenge", typeof(ChallengeHostPage));
        Routing.RegisterRoute("lesson", typeof(LessonHostPage));

        // Badges em itens do Shell (.NET MAUI 11): desafios restantes.
        // ProgressService é singleton (vive mais que esta janela): assinamos no Loaded e
        // desassinamos no Unloaded para não vazar memória ao fechar janelas extras.
        _progress = ServiceHelper.Get<ProgressService>();
        Loaded += (_, _) => { _progress.Changed += OnProgressChanged; UpdateBadge(); };
        Unloaded += (_, _) => _progress.Changed -= OnProgressChanged;
        UpdateBadge();
    }

    private readonly ProgressService _progress;

    private void OnProgressChanged(object? sender, EventArgs e) => UpdateBadge();

    private void UpdateBadge()
    {
        var remaining = ChallengeCatalog.All.Count(c => !_progress.IsChallengeDone(c.Id));
        ChallengesItem.BadgeText = remaining > 0 ? remaining.ToString() : null;
        ChallengesItem.BadgeColor = Color.FromArgb("#D600AA");
    }
}
