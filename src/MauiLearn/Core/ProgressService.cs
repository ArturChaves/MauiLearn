namespace MauiLearn.Core;

/// <summary>
/// Guarda o progresso do aluno (aulas e desafios concluídos, favoritos)
/// usando Preferences — um armazenamento chave/valor persistente do MAUI Essentials.
/// </summary>
public class ProgressService
{
    private readonly IPreferences _prefs;

    public ProgressService() : this(Preferences.Default) { }

    public ProgressService(IPreferences prefs) => _prefs = prefs;

    public event EventHandler? Changed;

    public bool IsLessonDone(string id) => Read(SettingsKeys.CompletedLessons).Contains(id);
    public bool IsChallengeDone(string id) => Read(SettingsKeys.CompletedChallenges).Contains(id);
    public bool IsFavorite(string id) => Read(SettingsKeys.FavoriteComponents).Contains(id);

    public int LessonsDone => Read(SettingsKeys.CompletedLessons).Count;
    public int ChallengesDone => Read(SettingsKeys.CompletedChallenges).Count;

    public void SetLessonDone(string id, bool done = true) => Toggle(SettingsKeys.CompletedLessons, id, done);
    public void SetChallengeDone(string id, bool done = true) => Toggle(SettingsKeys.CompletedChallenges, id, done);
    public void SetFavorite(string id, bool favorite) => Toggle(SettingsKeys.FavoriteComponents, id, favorite);

    public void ResetAll()
    {
        _prefs.Remove(SettingsKeys.CompletedLessons);
        _prefs.Remove(SettingsKeys.CompletedChallenges);
        _prefs.Remove(SettingsKeys.FavoriteComponents);
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private HashSet<string> Read(string key) =>
        _prefs.Get(key, string.Empty)
              .Split('|', StringSplitOptions.RemoveEmptyEntries)
              .ToHashSet();

    private void Toggle(string key, string id, bool on)
    {
        var set = Read(key);
        var changed = on ? set.Add(id) : set.Remove(id);
        if (!changed) return;

        _prefs.Set(key, string.Join('|', set));
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
