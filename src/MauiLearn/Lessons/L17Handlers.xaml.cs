namespace MauiLearn.Lessons;

public partial class L17Handlers : ContentView
{
    public L17Handlers()
    {
        InitializeComponent();
    }

    private void OnShowNative(object? sender, EventArgs e)
    {
        var handler = NativeButton.Handler;
        if (handler?.PlatformView is not { } native)
        {
            NativeInfo.Text = "Sem handler ainda.";
            return;
        }

        var details = $"Handler:      {handler.GetType().FullName}\n" +
                      $"PlatformView: {native.GetType().FullName}\n" +
                      $"Base nativa:  {native.GetType().BaseType?.FullName}";
#if WINDOWS
        if (native is Microsoft.UI.Xaml.Controls.Button winButton)
            details += $"\nWinUI ActualWidth: {winButton.ActualWidth:F0}";
#elif MACCATALYST
        if (native is UIKit.UIButton uiButton)
            details += $"\nUIKit Frame: {uiButton.Frame}";
#endif
        NativeInfo.Text = details;
    }
}
