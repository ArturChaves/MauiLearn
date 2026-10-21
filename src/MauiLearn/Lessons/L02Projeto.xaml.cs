namespace MauiLearn.Lessons;

public partial class L02Projeto : ContentView
{
    public L02Projeto()
    {
        InitializeComponent();
    }

    private async void OnReadAsset(object? sender, EventArgs e)
    {
        try
        {
            await using var stream = await FileSystem.OpenAppPackageFileAsync("sobre.txt");
            using var reader = new StreamReader(stream);
            AssetText.Text = await reader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            AssetText.Text = $"Erro: {ex.Message}";
        }
    }
}
