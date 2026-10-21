namespace MauiLearn.Lessons;

public partial class L04Xaml : ContentView
{
    private int _total;

    public L04Xaml()
    {
        InitializeComponent();
    }

    private void OnContar(object? sender, EventArgs e)
    {
        _total++;
        BotaoContar.Text = $"Contei {_total}x";
    }
}
