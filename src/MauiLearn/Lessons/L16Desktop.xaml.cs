using MauiLearn.Controls;

namespace MauiLearn.Lessons;

public partial class L16Desktop : ContentView
{
    private Window? _window;

    public L16Desktop()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            _window = Window;
            if (_window is not null)
            {
                _window.SizeChanged += OnWindowSizeChanged;
                UpdateInfo();
            }
        };
        Unloaded += (_, _) =>
        {
            if (_window is not null)
                _window.SizeChanged -= OnWindowSizeChanged; // evita vazamento de memória
        };
    }

    private void OnWindowSizeChanged(object? sender, EventArgs e) => UpdateInfo();

    private void UpdateInfo()
    {
        if (_window is null) return;
        WindowInfo.Text = $"Window: {_window.Width:F0} × {_window.Height:F0}  em  ({_window.X:F0}, {_window.Y:F0})\n" +
                          $"Título: {_window.Title}\nJanelas abertas: {Application.Current?.Windows.Count}";
    }

    private async void OnCopyText(object? sender, EventArgs e) => await Clipboard.Default.SetTextAsync(ContextTarget.Text);

    private void OnBold(object? sender, EventArgs e) =>
        ContextTarget.FontAttributes = ContextTarget.FontAttributes == FontAttributes.Bold ? FontAttributes.None : FontAttributes.Bold;

    private void OnSize(object? sender, EventArgs e)
    {
        if (sender is MenuFlyoutItem { CommandParameter: string size })
            ContextTarget.FontSize = double.Parse(size);
    }

    private void OnSizeSmall(object? sender, EventArgs e) { if (_window is null) return; _window.Width = 1024; _window.Height = 700; }

    private void OnSizeLarge(object? sender, EventArgs e) { if (_window is null) return; _window.Width = 1400; _window.Height = 900; }

    private void OnMove(object? sender, EventArgs e) { if (_window is null) return; _window.X = 80; _window.Y = 80; }

    private void OnTitle(object? sender, EventArgs e)
    {
        if (_window is null) return;
        _window.Title = $"MauiLearn — título alterado às {DateTime.Now:HH:mm:ss}";
        UpdateInfo();
    }

    private void OnNewWindow(object? sender, EventArgs e) => AppMenu.OpenNewWindow();

    private void OnTitleBar(object? sender, EventArgs e)
    {
        if (_window is null) return;
        _window.TitleBar = _window.TitleBar is null
            ? new TitleBar
            {
                Title = "MauiLearn",
                Subtitle = "TitleBar customizada · .NET MAUI",
                BackgroundColor = Color.FromArgb("#512BD4"),
                ForegroundColor = Colors.White,
                HeightRequest = 48,
                TrailingContent = new Label { Text = "✨", TextColor = Colors.White, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(0, 0, 12, 0) },
            }
            : null;
    }
}
