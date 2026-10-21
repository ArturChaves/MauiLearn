using System.Collections.ObjectModel;

namespace MauiLearn.Core;

/// <summary>
/// Log simples em memória, usado na aula de Ciclo de Vida para mostrar
/// a ordem dos eventos de Window/Page.
/// </summary>
public static class AppLog
{
    public static ObservableCollection<string> Entries { get; } = new();

    public static void Write(string message)
    {
        var line = $"{DateTime.Now:HH:mm:ss.fff}  {message}";
        System.Diagnostics.Debug.WriteLine(line);

        // Coleções ligadas à UI devem ser alteradas na thread principal.
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Entries.Insert(0, line);
            if (Entries.Count > 200)
                Entries.RemoveAt(Entries.Count - 1);
        });
    }
}
