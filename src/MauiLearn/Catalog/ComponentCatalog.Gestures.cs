using Microsoft.Maui.Controls.Shapes;

namespace MauiLearn.Catalog;

public static partial class ComponentCatalog
{
    private static void AddGestures(List<ComponentInfo> list)
    {
        const ComponentCategory cat = ComponentCategory.Gestures;

        C(list, "TapGestureRecognizer", cat, "👆", "Detecta toques/cliques (inclusive duplo clique e botão direito).")
            .About("Adicione a qualquer View em GestureRecognizers. No desktop, Buttons=\"Secondary\" detecta o botão direito do mouse.")
            .Demo(() =>
            {
                var tap = new TapGestureRecognizer { NumberOfTapsRequired = 1, Buttons = ButtonsMask.Primary };
                var target = GestureTarget("Clique aqui");
                var count = 0;
                tap.Tapped += (_, e) =>
                {
                    count++;
                    var pos = e.GetPosition(target);
                    TargetLabel(target).Text = $"Toques: {count}\nBotões: {e.Buttons}\nPosição: {pos?.X:F0}, {pos?.Y:F0}";
                };
                target.GestureRecognizers.Add(tap);
                return new DemoInstance(target, tap);
            })
            .Prop("NumberOfTapsRequired", "1 = clique, 2 = duplo clique...")
            .Prop("Buttons", "Primary, Secondary (botão direito) ou ambos (\"Primary,Secondary\").")
            .Prop("Command / CommandParameter", "Comando no toque.")
            .Event("Tapped", "Com TappedEventArgs: Buttons e GetPosition(relativoA).")
            .Nuance("Mude Buttons para Secondary no inspetor e clique com o botão direito.")
            .Nuance("e.GetPosition(view) retorna a posição relativa àquela view (ou à janela se null).")
            .Sample("""
                <Border>
                    <Border.GestureRecognizers>
                        <TapGestureRecognizer NumberOfTapsRequired="2" Tapped="OnDuploClique" />
                        <TapGestureRecognizer Buttons="Secondary" Command="{Binding MenuCommand}" />
                    </Border.GestureRecognizers>
                </Border>
                """)
            .Docs("fundamentals/gestures/tap")
            .Lesson("gestos");

        C(list, "PointerGestureRecognizer", cat, "🖱️", "Detecta o ponteiro do mouse: entrar, sair, mover, pressionar, soltar.")
            .About("Essencial no desktop para efeitos de hover. Também há o estado visual 'PointerOver' no VisualStateManager.")
            .Demo(() =>
            {
                var pointer = new PointerGestureRecognizer();
                var target = GestureTarget("Passe o mouse por aqui");
                var label = TargetLabel(target);
                pointer.PointerEntered += (_, _) => { target.BackgroundColor = Color.FromArgb("#55512BD4"); label.Text = "PointerEntered"; };
                pointer.PointerExited += (_, _) => { target.BackgroundColor = Color.FromArgb("#22512BD4"); label.Text = "PointerExited"; };
                pointer.PointerMoved += (_, e) => { var p = e.GetPosition(target); label.Text = $"PointerMoved: {p?.X:F0}, {p?.Y:F0}"; };
                pointer.PointerPressed += (_, e) => label.Text = $"PointerPressed ({e.Button})";
                pointer.PointerReleased += (_, e) => label.Text = $"PointerReleased ({e.Button})";
                target.GestureRecognizers.Add(pointer);
                return new DemoInstance(target, pointer);
            })
            .Prop("PointerEnteredCommand / PointerExitedCommand / PointerMovedCommand", "Versões MVVM dos eventos.")
            .Prop("PointerPressedCommand / PointerReleasedCommand", "Pressionar/soltar.")
            .Prop("Buttons", "Quais botões disparam pressed/released.")
            .Event("PointerEntered / PointerExited / PointerMoved", "Hover.")
            .Event("PointerPressed / PointerReleased", "Botão do mouse (e.Button).")
            .Nuance("Para hover puramente visual, prefira o VisualState 'PointerOver' — sem código.")
            .Sample("""
                <Label Text="Hover">
                    <Label.GestureRecognizers>
                        <PointerGestureRecognizer PointerEntered="OnEntrou" PointerExited="OnSaiu" />
                    </Label.GestureRecognizers>
                </Label>
                """)
            .Docs("fundamentals/gestures/pointer")
            .Lesson("gestos");

        C(list, "LongPressGestureRecognizer", cat, "✋", "Detecta 'pressionar e segurar'.")
            .New("Novo no .NET MAUI 11.")
            .About("Dispara LongPressed depois de MinimumPressDuration (ms) sem mover além de AllowableMovement. LongPressing informa o andamento (State).")
            .Demo(() =>
            {
                var lp = new LongPressGestureRecognizer { MinimumPressDuration = 600 };
                var target = GestureTarget("Pressione e segure");
                var label = TargetLabel(target);
                lp.LongPressing += (_, e) => label.Text = $"LongPressing: {e.Status}";
                lp.LongPressed += (_, _) => { label.Text = "LongPressed! 🎉"; target.BackgroundColor = Color.FromArgb("#552E9D5B"); };
                target.GestureRecognizers.Add(lp);
                return new DemoInstance(target, lp);
            })
            .Prop("MinimumPressDuration", "Tempo mínimo em milissegundos.")
            .Prop("AllowableMovement", "Quanto o dedo/ponteiro pode se mover sem cancelar.")
            .Prop("NumberOfTouchesRequired", "Dedos necessários (toque).")
            .Prop("Command / CommandParameter", "Comando ao completar.")
            .Prop("State (somente leitura)", "Estado atual do gesto.")
            .Event("LongPressed", "Gesto completo.")
            .Event("LongPressing", "Durante o gesto (Started, Running, Completed, Canceled).")
            .Nuance("Antes do .NET MAUI 11 era preciso o CommunityToolkit (TouchBehavior) para pressionar-e-segurar.")
            .Sample("""
                <Border>
                    <Border.GestureRecognizers>
                        <LongPressGestureRecognizer MinimumPressDuration="800"
                                                    Command="{Binding ExcluirCommand}" />
                    </Border.GestureRecognizers>
                </Border>
                """)
            .Lesson("gestos");

        C(list, "PanGestureRecognizer", cat, "✊", "Detecta arrastar (pan) e informa o deslocamento.")
            .About("PanUpdated traz StatusType (Started, Running, Completed, Canceled) e TotalX/TotalY desde o início do gesto.")
            .Demo(() =>
            {
                var pan = new PanGestureRecognizer();
                var ball = new Border
                {
                    WidthRequest = 70,
                    HeightRequest = 70,
                    StrokeShape = new Ellipse(),
                    BackgroundColor = Color.FromArgb("#512BD4"),
                    Content = new Label { Text = "arraste", TextColor = Colors.White, FontSize = 11, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center },
                };
                double startX = 0, startY = 0;
                pan.PanUpdated += (_, e) =>
                {
                    switch (e.StatusType)
                    {
                        case GestureStatus.Started: startX = ball.TranslationX; startY = ball.TranslationY; break;
                        case GestureStatus.Running: ball.TranslationX = startX + e.TotalX; ball.TranslationY = startY + e.TotalY; break;
                    }
                };
                ball.GestureRecognizers.Add(pan);
                var area = new Grid { HeightRequest = 240, WidthRequest = 420, BackgroundColor = Color.FromArgb("#11512BD4"), Children = { ball } };
                return new DemoInstance(area, pan);
            })
            .Prop("TouchPoints", "Número de dedos necessário (toque).")
            .Event("PanUpdated", "StatusType, TotalX, TotalY.")
            .Nuance("TotalX/TotalY são acumulados desde o Started — guarde a posição inicial para somar.")
            .Desktop("No desktop, pan funciona com clique-e-arraste do mouse.")
            .Sample("""<PanGestureRecognizer PanUpdated="OnPan" />""")
            .Docs("fundamentals/gestures/pan");

        C(list, "PinchGestureRecognizer", cat, "🤏", "Detecta pinça (zoom) — no Mac, com o trackpad.")
            .About("PinchUpdated informa Scale (relativo ao último evento) e ScaleOrigin.")
            .Demo(() =>
            {
                var pinch = new PinchGestureRecognizer();
                var img = new Image { Source = "maui_logo.png", WidthRequest = 120, HeightRequest = 120 };
                var current = 1d;
                pinch.PinchUpdated += (_, e) =>
                {
                    if (e.Status == GestureStatus.Running)
                    {
                        current = Math.Clamp(current * e.Scale, 0.5, 4);
                        img.Scale = current;
                    }
                };
                var area = new Grid { HeightRequest = 240, WidthRequest = 420, BackgroundColor = Color.FromArgb("#11512BD4"), Children = { img } };
                area.GestureRecognizers.Add(pinch);
                return new DemoInstance(area, pinch);
            })
            .Event("PinchUpdated", "Status, Scale (multiplicativo) e ScaleOrigin (0..1).")
            .Nuance("Scale é relativo ao evento anterior: multiplique, não atribua.")
            .Desktop("Funciona com gesto de pinça no trackpad do Mac e em telas touch no Windows. Com mouse comum, ofereça botões de zoom.")
            .Sample("""<PinchGestureRecognizer PinchUpdated="OnPinch" />""")
            .Docs("fundamentals/gestures/pinch");

        C(list, "SwipeGestureRecognizer", cat, "👉", "Detecta um deslize rápido numa direção.")
            .Demo(() =>
            {
                var swipe = new SwipeGestureRecognizer { Direction = SwipeDirection.Left | SwipeDirection.Right, Threshold = 50 };
                var target = GestureTarget("Deslize ← ou → (toque/trackpad/arraste)");
                swipe.Swiped += (_, e) => TargetLabel(target).Text = $"Swiped: {e.Direction}";
                target.GestureRecognizers.Add(swipe);
                return new DemoInstance(target, swipe);
            })
            .Prop("Direction", "Left, Right, Up, Down (combináveis com vírgula em XAML).")
            .Prop("Threshold", "Distância mínima em unidades.")
            .Prop("Command / CommandParameter", "Comando no deslize.")
            .Event("Swiped", "Com e.Direction.")
            .Sample("""<SwipeGestureRecognizer Direction="Left" Swiped="OnProximo" />""")
            .Docs("fundamentals/gestures/swipe");

        C(list, "Drag & Drop", cat, "📦", "Arrastar e soltar entre elementos (e de/para o sistema, no desktop).")
            .WithId("DragDrop")
            .About("DragGestureRecognizer no elemento de origem; DropGestureRecognizer no destino. Os dados viajam em e.Data (DataPackage) — texto, imagem ou propriedades customizadas.")
            .Demo(() =>
            {
                var drag = new DragGestureRecognizer { CanDrag = true };
                var source = new Border
                {
                    Padding = 14,
                    BackgroundColor = Color.FromArgb("#512BD4"),
                    StrokeShape = new RoundRectangle { CornerRadius = 10 },
                    Content = new Label { Text = "🍎 Arraste-me", TextColor = Colors.White },
                };
                drag.DragStarting += (_, e) => e.Data.Text = "🍎";
                source.GestureRecognizers.Add(drag);

                var dropLabel = new Label { Text = "Solte aqui", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
                var target = new Border
                {
                    WidthRequest = 200,
                    HeightRequest = 110,
                    StrokeDashArray = [4, 2],
                    StrokeShape = new RoundRectangle { CornerRadius = 10 },
                    Content = dropLabel,
                };
                var drop = new DropGestureRecognizer { AllowDrop = true };
                drop.DragOver += (_, e) => { target.BackgroundColor = Color.FromArgb("#332E9D5B"); e.AcceptedOperation = DataPackageOperation.Copy; };
                drop.DragLeave += (_, _) => target.BackgroundColor = Colors.Transparent;
                drop.Drop += async (_, e) =>
                {
                    var text = await e.Data.GetTextAsync();
                    dropLabel.Text = $"Recebi: {text}";
                    target.BackgroundColor = Colors.Transparent;
                };
                target.GestureRecognizers.Add(drop);
                var row = new HorizontalStackLayout { Spacing = 30, Children = { source, target } };
                return new DemoInstance(row, drag);
            })
            .Prop("CanDrag (Drag)", "Habilita arrastar.")
            .Prop("AllowDrop (Drop)", "Habilita soltar.")
            .Prop("DragStartingCommand / DropCommand", "Versões MVVM.")
            .Event("DragStarting", "Preencha e.Data (Text, Image, Properties).")
            .Event("DragOver / DragLeave", "Feedback visual e e.AcceptedOperation.")
            .Event("Drop", "Leia os dados com await e.Data.GetTextAsync().")
            .Nuance("O inspetor edita o DragGestureRecognizer. Desligue CanDrag e tente arrastar.")
            .Desktop("No Windows/macOS você pode arrastar texto de OUTROS apps para o destino. Use e.PlatformArgs para acessar dados nativos (arquivos, etc.).")
            .Sample("""
                <Label Text="Arraste">
                    <Label.GestureRecognizers>
                        <DragGestureRecognizer DragStarting="OnDragStarting" />
                    </Label.GestureRecognizers>
                </Label>
                <Border>
                    <Border.GestureRecognizers>
                        <DropGestureRecognizer Drop="OnDrop" DragOver="OnDragOver" />
                    </Border.GestureRecognizers>
                </Border>
                """)
            .Docs("fundamentals/gestures/drag-and-drop")
            .Lesson("gestos");
    }

    private static Label TargetLabel(Border border) => (Label)border.Content!;

    private static Border GestureTarget(string text) => new()
    {
        WidthRequest = 300,
        HeightRequest = 140,
        BackgroundColor = Color.FromArgb("#22512BD4"),
        StrokeShape = new RoundRectangle { CornerRadius = 14 },
        Content = new Label
        {
            Text = text,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
        },
    };
}
