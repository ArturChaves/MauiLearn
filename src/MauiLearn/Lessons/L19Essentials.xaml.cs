namespace MauiLearn.Lessons;

public partial class L19Essentials : ContentView
{
    private const string PrefKey = "lesson.essentials.value";
    private const string SecretKey = "lesson.essentials.secret";

    public L19Essentials()
    {
        InitializeComponent();
        try
        {
            ShowEnvironment();
        }
        catch (Exception ex)
        {
            DeviceLabel.Text = $"APIs de plataforma indisponíveis aqui: {ex.Message}";
        }
    }

    private void ShowEnvironment()
    {
        PathsLabel.Text = $"AppDataDirectory: {FileSystem.Current.AppDataDirectory}\nCacheDirectory:   {FileSystem.Current.CacheDirectory}";
        DeviceLabel.Text =
            $"DeviceInfo: {DeviceInfo.Current.Platform} {DeviceInfo.Current.VersionString} · {DeviceInfo.Current.Idiom} · {DeviceInfo.Current.Model}\n" +
            $"Rede: {Connectivity.Current.NetworkAccess} ({string.Join(", ", Connectivity.Current.ConnectionProfiles)})\n" +
            $"Tema do sistema: {AppInfo.Current.RequestedTheme}";
    }

    private void OnPrefSave(object? sender, EventArgs e)
    {
        Preferences.Default.Set(PrefKey, PrefEntry.Text ?? string.Empty);
        PrefEntry.Text = string.Empty;
        PrefEntry.Placeholder = "Salvo! Clique em Ler (funciona mesmo após reiniciar o app)";
    }

    private void OnPrefLoad(object? sender, EventArgs e) => PrefEntry.Text = Preferences.Default.Get(PrefKey, "(nada salvo)");

    private async void OnSecretSave(object? sender, EventArgs e)
    {
        try
        {
            await SecureStorage.Default.SetAsync(SecretKey, SecretEntry.Text ?? string.Empty);
            SecretResult.Text = "Guardado com segurança 🔐";
            SecretEntry.Text = string.Empty;
        }
        catch (Exception ex)
        {
            SecretResult.Text = $"Falhou: {ex.Message}";
        }
    }

    private async void OnSecretLoad(object? sender, EventArgs e)
    {
        try
        {
            SecretResult.Text = $"Segredo: {await SecureStorage.Default.GetAsync(SecretKey) ?? "(vazio)"}";
        }
        catch (Exception ex)
        {
            SecretResult.Text = $"Falhou: {ex.Message}";
        }
    }

    private async void OnCopy(object? sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync("Olá MAUI");
        ClipResult.Text = "Copiado! Cole em outro app (Ctrl/⌘+V).";
    }

    private async void OnPaste(object? sender, EventArgs e) =>
        ClipResult.Text = Clipboard.Default.HasText ? $"Área de transferência: {await Clipboard.Default.GetTextAsync()}" : "Área de transferência vazia.";

    private async void OnLaunch(object? sender, EventArgs e) =>
        await Browser.Default.OpenAsync("https://learn.microsoft.com/dotnet/maui/", BrowserLaunchMode.External);

    private async void OnMail(object? sender, EventArgs e) =>
        await Launcher.Default.OpenAsync("mailto:?subject=Aprendendo%20MAUI");

    private async void OnPickFile(object? sender, EventArgs e)
    {
        try
        {
            var file = await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Escolha qualquer arquivo" });
            if (file is null) { FileResult.Text = "Cancelado."; return; }
            await using var stream = await file.OpenReadAsync();
            FileResult.Text = $"{file.FileName} · {file.ContentType} · {stream.Length:N0} bytes";
        }
        catch (Exception ex)
        {
            FileResult.Text = $"Erro: {ex.Message}";
        }
    }

    private async void OnWriteFile(object? sender, EventArgs e)
    {
        var path = Path.Combine(FileSystem.Current.AppDataDirectory, "mauilearn-nota.txt");
        await File.WriteAllTextAsync(path, $"Escrito pelo MauiLearn em {DateTime.Now}");
        FileResult.Text = $"Escrito: {path}\nConteúdo: {await File.ReadAllTextAsync(path)}";
    }

    private async void OnSpeak(object? sender, EventArgs e)
    {
        try { await TextToSpeech.Default.SpeakAsync("Olá! Eu sou o .NET MAUI falando com você."); }
        catch (Exception ex) { DeviceLabel.Text += $"\nTTS indisponível: {ex.Message}"; }
    }

    private async void OnScreenshot(object? sender, EventArgs e)
    {
        try
        {
            if (!Screenshot.Default.IsCaptureSupported) { DeviceLabel.Text += "\nCaptura não suportada."; return; }
            var shot = await Screenshot.Default.CaptureAsync();
            var bytes = await ReadAll(await shot.OpenReadAsync());
            ScreenshotImage.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
            ScreenshotImage.IsVisible = true;
        }
        catch (Exception ex)
        {
            DeviceLabel.Text += $"\nCaptura falhou: {ex.Message}";
        }
    }

    private static async Task<byte[]> ReadAll(Stream stream)
    {
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return ms.ToArray();
    }
}
