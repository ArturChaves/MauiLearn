namespace MauiLearn.Lessons;

public partial class L13Gestos : ContentView
{
    private double _startX, _startY;

    public L13Gestos()
    {
        InitializeComponent();
    }

    private void Write(string text)
    {
        Log.Insert(0, new Label { Text = $"{DateTime.Now:HH:mm:ss}  {text}", FontSize = 12 });
        while (Log.Count > 12) Log.RemoveAt(Log.Count - 1);
    }

    private void OnPan(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _startX = Card.TranslationX;
                _startY = Card.TranslationY;
                Write("Pan: Started");
                break;
            case GestureStatus.Running:
                Card.TranslationX = _startX + e.TotalX;
                Card.TranslationY = _startY + e.TotalY;
                break;
            case GestureStatus.Completed:
                Write($"Pan: Completed ({Card.TranslationX:F0}, {Card.TranslationY:F0})");
                break;
        }
    }

    private async void OnDoubleTap(object? sender, EventArgs e)
    {
        Write("Duplo clique → centralizando");
        await Card.TranslateToAsync(0, 0, 400, Easing.SpringOut);
    }

    private void OnRightClick(object? sender, TappedEventArgs e) =>
        Write($"Botão direito em {e.GetPosition(Card)?.X:F0},{e.GetPosition(Card)?.Y:F0} (o menu de contexto também abre)");

    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        Card.Opacity = 0.85;
        Write("PointerEntered");
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        Card.Opacity = 1;
        CardInfo.Text = "arraste · 2x clique · botão direito";
        Write("PointerExited");
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var p = e.GetPosition(Card);
        CardInfo.Text = $"mouse em {p?.X:F0}, {p?.Y:F0}";
    }

    private async void OnLongPressed(object? sender, LongPressedEventArgs e)
    {
        Write("LongPressed! (novo no .NET MAUI 11)");
        await Card.ScaleToAsync(1.2, 150);
        await Card.ScaleToAsync(1, 150);
    }

    private void OnColorMenu(object? sender, EventArgs e)
    {
        if (sender is MenuFlyoutItem { CommandParameter: string hex, Text: var name })
        {
            Card.BackgroundColor = Color.FromArgb(hex);
            Write($"Menu de contexto: {name}");
        }
    }
}
