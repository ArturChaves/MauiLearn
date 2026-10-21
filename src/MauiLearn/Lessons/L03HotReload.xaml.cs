namespace MauiLearn.Lessons;

public partial class L03HotReload : ContentView
{
    private int _clicks;

    public L03HotReload()
    {
        InitializeComponent();
    }

    private void OnHotReloadClicked(object? sender, EventArgs e)
    {
        _clicks++;
        // 👇 Mude esta mensagem com o app rodando, salve e clique de novo (C# Hot Reload).
        CSharpOutput.Text = $"Olá do C#! Cliques: {_clicks}";
    }
}
