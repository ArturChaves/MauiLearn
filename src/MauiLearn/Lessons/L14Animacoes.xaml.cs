namespace MauiLearn.Lessons;

public partial class L14Animacoes : ContentView
{
    private static readonly (string Name, Easing Easing)[] Easings =
    [
        ("Linear", Easing.Linear), ("CubicInOut", Easing.CubicInOut), ("CubicIn", Easing.CubicIn), ("CubicOut", Easing.CubicOut),
        ("SinInOut", Easing.SinInOut), ("SpringIn", Easing.SpringIn), ("SpringOut", Easing.SpringOut),
        ("BounceIn", Easing.BounceIn), ("BounceOut", Easing.BounceOut),
    ];

    private bool _pulsing;
    private bool _right;

    public L14Animacoes()
    {
        InitializeComponent();
        EasingPicker.ItemsSource = Easings.Select(e => e.Name).ToList();
        EasingPicker.SelectedIndex = 1;
    }

    private Easing CurrentEasing => EasingPicker.SelectedIndex >= 0 ? Easings[EasingPicker.SelectedIndex].Easing : Easing.Linear;

    private uint Duration => (uint)DurationSlider.Value;

    private async void OnTranslate(object? sender, EventArgs e)
    {
        _right = !_right;
        await Target.TranslateToAsync(_right ? 160 : -160, 0, Duration, CurrentEasing);
    }

    private async void OnScale(object? sender, EventArgs e) =>
        await Target.ScaleToAsync(Target.Scale > 1.1 ? 1 : 1.6, Duration, CurrentEasing);

    private async void OnRotate(object? sender, EventArgs e) =>
        await Target.RelRotateToAsync(90, Duration, CurrentEasing);

    private async void OnFade(object? sender, EventArgs e) =>
        await Target.FadeToAsync(Target.Opacity > 0.5 ? 0.15 : 1, Duration, CurrentEasing);

    private async void OnSequence(object? sender, EventArgs e)
    {
        await Target.ScaleToAsync(1.5, Duration / 3, CurrentEasing);
        await Target.RotateToAsync(Target.Rotation + 180, Duration / 3, CurrentEasing);
        await Target.ScaleToAsync(1, Duration / 3, CurrentEasing);
    }

    private async void OnParallel(object? sender, EventArgs e)
    {
        await Task.WhenAll(
            Target.RotateToAsync(Target.Rotation + 360, Duration, CurrentEasing),
            Target.ScaleToAsync(1.4, Duration / 2, CurrentEasing),
            Target.FadeToAsync(0.6, Duration, CurrentEasing));
        await Task.WhenAll(Target.ScaleToAsync(1, Duration / 2), Target.FadeToAsync(1, Duration / 2));
    }

    private void OnColorAnimation(object? sender, EventArgs e)
    {
        var anim = new Animation(v => Target.BackgroundColor = Color.FromHsla(v, 0.7, 0.5), 0, 1);
        anim.Commit(Target, "Arcoiris", length: Duration * 2, easing: Easing.Linear,
            finished: (_, _) => Target.BackgroundColor = Color.FromArgb("#512BD4"));
    }

    private void OnPulse(object? sender, EventArgs e)
    {
        _pulsing = !_pulsing;
        PulseButton.Text = _pulsing ? "⏸ Parar pulso" : "▶ Pulsar (loop)";
        if (!_pulsing) return;

        var pulse = new Animation
        {
            { 0, 0.5, new Animation(v => Target.Scale = v, 1, 1.15, Easing.SinOut) },
            { 0.5, 1, new Animation(v => Target.Scale = v, 1.15, 1, Easing.SinIn) },
        };
        pulse.Commit(Target, "Pulso", length: 800, repeat: () => _pulsing);
    }

    private void OnComposite(object? sender, EventArgs e)
    {
        var parent = new Animation
        {
            { 0.0, 0.5, new Animation(v => Target.Scale = v, 1, 2) },
            { 0.5, 1.0, new Animation(v => Target.Scale = v, 2, 1) },
            { 0.0, 1.0, new Animation(v => Target.Rotation = v, 0, 360) },
        };
        parent.Commit(Target, "Composta", length: 1200, finished: (_, _) => Target.Rotation = 0);
    }

    private void OnCancel(object? sender, EventArgs e)
    {
        _pulsing = false;
        PulseButton.Text = "▶ Pulsar (loop)";
        Target.CancelAnimations();
        Target.AbortAnimation("Pulso");
        Target.AbortAnimation("Arcoiris");
        Target.AbortAnimation("Composta");
    }

    private void OnReset(object? sender, EventArgs e)
    {
        OnCancel(sender, e);
        Target.TranslationX = Target.TranslationY = 0;
        Target.Scale = 1;
        Target.Rotation = 0;
        Target.Opacity = 1;
        Target.BackgroundColor = Color.FromArgb("#512BD4");
        _right = false;
    }
}
