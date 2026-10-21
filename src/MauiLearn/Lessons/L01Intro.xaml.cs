namespace MauiLearn.Lessons;

public partial class L01Intro : ContentView
{
    public L01Intro()
    {
        InitializeComponent();

        // HandlerChanged dispara quando o handler (e portanto o controle nativo) é criado.
        Show(DemoButton, ButtonNative);
        Show(DemoEntry, EntryNative);
        Show(DemoSwitch, SwitchNative);
        Show(DemoLabel, LabelNative);
    }

    private static void Show(View view, Label output)
    {
        output.Text = "(aguardando o handler…)";
        view.HandlerChanged += (_, _) =>
        {
            var native = view.Handler?.PlatformView;
            output.Text = native is null
                ? "(sem handler)"
                : $"{view.Handler!.GetType().Name}  →  {native.GetType().FullName}";
        };
    }
}
