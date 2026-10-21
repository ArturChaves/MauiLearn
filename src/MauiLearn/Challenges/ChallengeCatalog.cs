using MauiLearn.Challenges.Workspaces;
using MauiLearn.ViewModels;
using static MauiLearn.Challenges.Verify;

namespace MauiLearn.Challenges;

/// <summary>
/// Todos os desafios. Cada verificação inspeciona a área de trabalho AO VIVO — por isso funciona
/// com Hot Reload: o XAML que você salvou já está aplicado na instância em tela.
/// </summary>
public static class ChallengeCatalog
{
    private static List<ChallengeInfo>? _all;

    public static IReadOnlyList<ChallengeInfo> All => _all ??= Build();

    public static ChallengeInfo? Find(string? id) => All.FirstOrDefault(c => c.Id == id);

    private const string Workspaces = "Challenges/Workspaces/";

    private static List<ChallengeInfo> Build() =>
    [
        new ChallengeInfo("d01", 1, "Olá, Hot Reload!", "Fácil", Workspaces + "D01OlaHotReload.xaml", () => new D01OlaHotReload())
            .WithGoal("Seu primeiro contato com o XAML Hot Reload: mude texto, tamanho e cor de um Label com o app rodando.")
            .Lesson("hotreload")
            .Step("Rode o app em Debug (F5) e abra este desafio.")
            .Step("Abra o arquivo indicado no seu editor.")
            .Step("Mude Text para \"Olá, MAUI!\", FontSize para 32+ e TextColor para #512BD4.")
            .Step("Salve (Ctrl/⌘+S) — a área de trabalho muda na hora. Clique em Verificar.")
            .Hint("Os atributos ficam no próprio <Label ...>. Ex.: FontSize=\"36\".")
            .Hint("Se nada mudar ao salvar, confira se o app está rodando com o depurador (Debug) e se o Hot Reload está ligado na IDE.")
            .Check("Text é \"Olá, MAUI!\"", ws => Named<Label>(ws, "Titulo", l => That(l.Text == "Olá, MAUI!", "ok", $"Text atual: \"{l.Text}\"")))
            .Check("FontSize ≥ 32", ws => Named<Label>(ws, "Titulo", l => That(l.FontSize >= 32, "ok", $"FontSize atual: {l.FontSize}")))
            .Check("TextColor = #512BD4", ws => Named<Label>(ws, "Titulo", l => That(SameColor(l.TextColor, Color.FromArgb("#512BD4")), "ok", $"TextColor atual: {Hex(l.TextColor)}")))
            .WithSolution("""
                <Label x:Name="Titulo"
                       Text="Olá, MAUI!"
                       FontSize="36"
                       TextColor="#512BD4"
                       HorizontalOptions="Center"
                       VerticalOptions="Center" />
                """),

        new ChallengeInfo("d02", 2, "Botão estiloso", "Fácil", Workspaces + "D02BotaoEstiloso.xaml", () => new D02BotaoEstiloso())
            .WithGoal("Personalize um Button: texto, cor de fundo, cantos e espaçamento interno — e veja o valor local vencer o estilo implícito.")
            .Lesson("estilos")
            .Step("Defina Text, BackgroundColor, CornerRadius e Padding no <Button x:Name=\"Botao\">.")
            .Step("Passe o mouse no botão: o estado PointerOver vem do estilo do app.")
            .Hint("Padding=\"24,10\" significa 24 na horizontal e 10 na vertical.")
            .Check("Text é \"Salvar\"", ws => Named<Button>(ws, "Botao", b => That(b.Text == "Salvar", "ok", $"Text atual: \"{b.Text}\"")))
            .Check("BackgroundColor = #2E9D5B", ws => Named<Button>(ws, "Botao", b => That(SameColor(b.BackgroundColor, Color.FromArgb("#2E9D5B")), "ok", $"BackgroundColor atual: {Hex(b.BackgroundColor)}")))
            .Check("CornerRadius = 20", ws => Named<Button>(ws, "Botao", b => That(b.CornerRadius == 20, "ok", $"CornerRadius atual: {b.CornerRadius}")))
            .Check("Padding horizontal ≥ 24", ws => Named<Button>(ws, "Botao", b => That(b.Padding.Left >= 24 && b.Padding.Right >= 24, "ok", $"Padding atual: {b.Padding}")))
            .WithSolution("""
                <Button x:Name="Botao"
                        Text="Salvar"
                        BackgroundColor="#2E9D5B"
                        CornerRadius="20"
                        Padding="24,10"
                        HorizontalOptions="Center"
                        VerticalOptions="Center" />
                """),

        new ChallengeInfo("d03", 3, "Lado a lado", "Fácil", Workspaces + "D03LadoALado.xaml", () => new D03LadoALado())
            .WithGoal("Troque o layout de vertical para horizontal e ajuste espaçamento e alinhamento.")
            .Lesson("layouts")
            .Step("Troque VerticalStackLayout por HorizontalStackLayout (nas DUAS tags).")
            .Step("Defina Spacing=\"12\" e HorizontalOptions=\"Center\".")
            .Hint("Trocar o TIPO de um elemento é uma mudança estrutural; o XAML Hot Reload consegue aplicar. Se não aplicar, use \"↻ Recarregar área\".")
            .Check("Caixas é um HorizontalStackLayout", ws => Named<Element>(ws, "Caixas", e => That(e is HorizontalStackLayout, "ok", $"Tipo atual: {e.GetType().Name}")))
            .Check("Spacing = 12", ws => Named<StackBase>(ws, "Caixas", s => That(s.Spacing == 12, "ok", $"Spacing atual: {s.Spacing}")))
            .Check("HorizontalOptions = Center", ws => Named<View>(ws, "Caixas", v => That(v.HorizontalOptions.Alignment == LayoutAlignment.Center, "ok", $"HorizontalOptions atual: {v.HorizontalOptions.Alignment}")))
            .WithSolution("""
                <HorizontalStackLayout x:Name="Caixas" Spacing="12"
                                       HorizontalOptions="Center" VerticalOptions="Center">
                    <BoxView Color="#512BD4" WidthRequest="80" HeightRequest="80" />
                    <BoxView Color="#D600AA" WidthRequest="80" HeightRequest="80" />
                    <BoxView Color="#2E9D5B" WidthRequest="80" HeightRequest="80" />
                </HorizontalStackLayout>
                """),

        new ChallengeInfo("d04", 4, "Centralizado de verdade", "Fácil", Workspaces + "D04Centralizado.xaml", () => new D04Centralizado())
            .WithGoal("Domine HorizontalOptions/VerticalOptions, WidthRequest, Margin e Padding.")
            .Lesson("layouts")
            .Step("Centralize o Border nos dois eixos.")
            .Step("Dê WidthRequest=\"240\", Padding=\"20\" e Margin=\"0\".")
            .Hint("Por padrão, Options = Fill: o filho estica. Center faz ele usar só o tamanho de que precisa.")
            .Check("Centralizado na horizontal", ws => Named<View>(ws, "Cartao", v => That(v.HorizontalOptions.Alignment == LayoutAlignment.Center, "ok", $"HorizontalOptions: {v.HorizontalOptions.Alignment}")))
            .Check("Centralizado na vertical", ws => Named<View>(ws, "Cartao", v => That(v.VerticalOptions.Alignment == LayoutAlignment.Center, "ok", $"VerticalOptions: {v.VerticalOptions.Alignment}")))
            .Check("WidthRequest = 240", ws => Named<View>(ws, "Cartao", v => That(v.WidthRequest == 240, "ok", $"WidthRequest: {v.WidthRequest}")))
            .Check("Padding = 20 e Margin = 0", ws => Named<Border>(ws, "Cartao", b => That(b.Padding == new Thickness(20) && b.Margin == new Thickness(0), "ok", $"Padding: {b.Padding} · Margin: {b.Margin}")))
            .WithSolution("""
                <Border x:Name="Cartao"
                        BackgroundColor="#512BD4"
                        StrokeShape="RoundRectangle 16"
                        HorizontalOptions="Center" VerticalOptions="Center"
                        WidthRequest="240" Padding="20" Margin="0">
                    <Label Text="Cartão" TextColor="White" FontSize="20" />
                </Border>
                """),

        new ChallengeInfo("d05", 5, "Jogo da velha (Grid 3×3)", "Médio", Workspaces + "D05Grade.xaml", () => new D05Grade())
            .WithGoal("Monte uma grade 3×3 com Grid, RowDefinitions/ColumnDefinitions e Grid.Row/Grid.Column.")
            .Lesson("grid")
            .Step("Defina 3 linhas e 3 colunas \"*\".")
            .Step("Posicione as 9 caixas em células diferentes. O \"Centro\" vai em Row 1, Column 1.")
            .Hint("Sem Grid.Row/Grid.Column, o padrão é 0 — por isso todas estão em (0,0).")
            .Hint("Linha 0: Grid.Column 0, 1 e 2. Linha 1: Grid.Row=\"1\" e colunas 0, 1, 2. E assim por diante.")
            .Check("3 linhas e 3 colunas", ws => Named<Grid>(ws, "Tabuleiro", g => That(g.RowDefinitions.Count == 3 && g.ColumnDefinitions.Count == 3, "ok", $"Linhas: {g.RowDefinitions.Count} · Colunas: {g.ColumnDefinitions.Count}")))
            .Check("Centro em Row 1, Column 1", ws => Named<BoxView>(ws, "Centro", b => That(Grid.GetRow(b) == 1 && Grid.GetColumn(b) == 1, "ok", $"Centro está em Row {Grid.GetRow(b)}, Column {Grid.GetColumn(b)}")))
            .Check("Cada caixa numa célula diferente", ws => Named<Grid>(ws, "Tabuleiro", g =>
            {
                var cells = g.Children.OfType<BindableObject>().Select(c => (Grid.GetRow(c), Grid.GetColumn(c))).ToList();
                return That(cells.Count == 9 && cells.Distinct().Count() == 9, "ok", $"{cells.Distinct().Count()} células distintas de 9");
            }))
            .WithSolution("""
                <Grid x:Name="Tabuleiro" WidthRequest="300" HeightRequest="300"
                      RowDefinitions="*,*,*" ColumnDefinitions="*,*,*"
                      RowSpacing="6" ColumnSpacing="6">
                    <BoxView Color="#512BD4" />
                    <BoxView Color="#7B5CFA" Grid.Column="1" />
                    <BoxView Color="#512BD4" Grid.Column="2" />
                    <BoxView Color="#7B5CFA" Grid.Row="1" />
                    <BoxView x:Name="Centro" Color="#D600AA" Grid.Row="1" Grid.Column="1" />
                    <BoxView Color="#7B5CFA" Grid.Row="1" Grid.Column="2" />
                    <BoxView Color="#512BD4" Grid.Row="2" />
                    <BoxView Color="#7B5CFA" Grid.Row="2" Grid.Column="1" />
                    <BoxView Color="#512BD4" Grid.Row="2" Grid.Column="2" />
                </Grid>
                """),

        new ChallengeInfo("d06", 6, "Ligando um Slider", "Médio", Workspaces + "D06Binding.xaml", () => new D06Binding())
            .WithGoal("Crie um binding entre dois controles com x:Reference e StringFormat.")
            .Lesson("binding")
            .Step("No Label \"Valor\", troque o Text fixo por um Binding para Value do Slider \"Volume\".")
            .Step("Use StringFormat='Volume: {0:F0}'. Arraste o slider para ver.")
            .Hint("Text=\"{Binding Value, Source={x:Reference Volume}, StringFormat='Volume: {0:F0}'}\"")
            .Check("O Label acompanha o Slider (testa com 42)", ws => Named<Slider>(ws, "Volume", s => Named<Label>(ws, "Valor", l =>
            {
                var old = s.Value;
                s.Value = 42;
                var text = l.Text;
                s.Value = old;
                return That(text == "Volume: 42", "ok", $"Com o slider em 42, o texto foi \"{text}\"");
            })))
            .WithSolution("""
                <Label x:Name="Valor" FontSize="24" HorizontalOptions="Center"
                       Text="{Binding Value, Source={x:Reference Volume}, StringFormat='Volume: {0:F0}', x:DataType=Slider}" />
                """),

        new ChallengeInfo("d07", 7, "Estilo reutilizável", "Médio", Workspaces + "D07Estilo.xaml", () => new D07Estilo())
            .WithGoal("Extraia atributos repetidos para um Style com chave e aplique-o em vários controles.")
            .Lesson("estilos")
            .Step("Em <ContentView.Resources>, crie <Style x:Key=\"Destaque\" TargetType=\"Label\"> com Setters.")
            .Step("Aplique Style=\"{StaticResource Destaque}\" em L1, L2 e L3 e remova os atributos repetidos.")
            .Hint("<Setter Property=\"TextColor\" Value=\"#D600AA\" />")
            .Check("Existe um Style \"Destaque\" para Label", ws => That(ws.Resources.TryGetValue("Destaque", out var s) && s is Style { TargetType: var t } && t == typeof(Label), "ok", "Não encontrei Style x:Key=\"Destaque\" com TargetType=\"Label\" em ContentView.Resources."))
            .Check("L1, L2 e L3 usam o estilo Destaque", ws =>
            {
                ws.Resources.TryGetValue("Destaque", out var style);
                var labels = new[] { "L1", "L2", "L3" }.Select(n => ws.FindByName<Label>(n)).ToList();
                return That(style is not null && labels.All(l => l?.Style == style), "ok", "Algum Label não está com Style=\"{StaticResource Destaque}\".");
            })
            .Check("O estilo define TextColor", ws => That(ws.Resources.TryGetValue("Destaque", out var s) && s is Style st && st.Setters.Any(x => x.Property == Label.TextColorProperty), "ok", "Adicione um Setter para TextColor."))
            .Check("Labels sem atributos repetidos (TextColor vem do estilo)", ws =>
            {
                var labels = new[] { "L1", "L2", "L3" }.Select(n => ws.FindByName<Label>(n)).OfType<Label>().ToList();
                ws.Resources.TryGetValue("Destaque", out var s);
                var styleColor = (s as Style)?.Setters.FirstOrDefault(x => x.Property == Label.TextColorProperty)?.Value as Color;
                return That(labels.Count == 3 && styleColor is not null && labels.All(l => SameColor(l.TextColor, styleColor)), "ok", "A cor dos Labels deveria vir do Style. Remova TextColor dos Labels.");
            })
            .WithSolution("""
                <ContentView.Resources>
                    <Style x:Key="Destaque" TargetType="Label">
                        <Setter Property="TextColor" Value="#D600AA" />
                        <Setter Property="FontSize" Value="22" />
                        <Setter Property="FontAttributes" Value="Bold" />
                    </Style>
                </ContentView.Resources>

                <VerticalStackLayout Spacing="10" VerticalOptions="Center" HorizontalOptions="Center">
                    <Label x:Name="L1" Text="Primeiro" Style="{StaticResource Destaque}" />
                    <Label x:Name="L2" Text="Segundo" Style="{StaticResource Destaque}" />
                    <Label x:Name="L3" Text="Terceiro" Style="{StaticResource Destaque}" />
                </VerticalStackLayout>
                """),

        new ChallengeInfo("d08", 8, "Botão que só habilita com texto", "Médio", Workspaces + "D08Trigger.xaml", () => new D08Trigger())
            .WithGoal("Use um DataTrigger para habilitar/desabilitar um botão sem nenhuma linha de C#.")
            .Lesson("triggers")
            .Step("Adicione <Button.Triggers> com um DataTrigger observando Text.Length da Entry \"Email\".")
            .Step("Quando Value=\"0\", o Setter deve definir IsEnabled=\"False\".")
            .Hint("Binding=\"{Binding Text.Length, Source={x:Reference Email}}\"")
            .Check("Desabilitado com o campo vazio", ws => Named<Entry>(ws, "Email", e => Named<Button>(ws, "Enviar", b =>
            {
                var old = e.Text;
                e.Text = string.Empty;
                var enabled = b.IsEnabled;
                e.Text = old;
                return That(!enabled, "ok", "Com o e-mail vazio, o botão ainda está habilitado.");
            })))
            .Check("Habilitado com texto", ws => Named<Entry>(ws, "Email", e => Named<Button>(ws, "Enviar", b =>
            {
                var old = e.Text;
                e.Text = "voce@exemplo.com";
                var enabled = b.IsEnabled;
                e.Text = old;
                return That(enabled, "ok", "Com texto no e-mail, o botão deveria estar habilitado.");
            })))
            .WithSolution("""
                <Button x:Name="Enviar" Text="Enviar">
                    <Button.Triggers>
                        <DataTrigger TargetType="Button"
                                     Binding="{Binding Text.Length, Source={x:Reference Email}, x:DataType=Entry}"
                                     Value="0">
                            <Setter Property="IsEnabled" Value="False" />
                        </DataTrigger>
                    </Button.Triggers>
                </Button>
                """),

        new ChallengeInfo("d09", 9, "Lista em grade", "Médio", Workspaces + "D09ListaGrade.xaml", () => new D09ListaGrade())
            .WithGoal("Mude o ItemsLayout de uma CollectionView e melhore o ItemTemplate.")
            .Lesson("colecoes")
            .Step("Adicione ItemsLayout=\"VerticalGrid, 3\" à CollectionView.")
            .Step("No DataTemplate, envolva o Label com um Border.")
            .Hint("ItemsLayout aceita a sintaxe curta: \"VerticalList\", \"HorizontalList\", \"VerticalGrid, N\", \"HorizontalGrid, N\".")
            .Check("ItemsLayout em grade com 3 colunas", ws => Named<CollectionView>(ws, "Lista", c => That(c.ItemsLayout is GridItemsLayout { Span: 3, Orientation: ItemsLayoutOrientation.Vertical }, "ok", $"ItemsLayout atual: {Describe(c.ItemsLayout)}")))
            .Check("O template usa um Border", ws => Named<CollectionView>(ws, "Lista", c => That(c.ItemTemplate?.CreateContent() is Border, "ok", "A raiz do DataTemplate deveria ser um Border.")))
            .WithSolution("""
                <CollectionView x:Name="Lista" ItemsLayout="VerticalGrid, 3" SelectionMode="Single">
                    <!-- ItemsSource igual -->
                    <CollectionView.ItemTemplate>
                        <DataTemplate x:DataType="x:String">
                            <Border Padding="12" Margin="4" StrokeShape="RoundRectangle 10">
                                <Label Text="{Binding .}" FontSize="18" />
                            </Border>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>
                """),

        new ChallengeInfo("d10", 10, "Hover no desktop", "Médio", Workspaces + "D10Hover.xaml", () => new D10Hover())
            .WithGoal("Use o VisualStateManager para dar feedback de hover (PointerOver).")
            .Lesson("triggers")
            .Step("Dentro do Border \"Card\", adicione VisualStateManager.VisualStateGroups com o grupo CommonStates.")
            .Step("Crie os estados Normal e PointerOver; no PointerOver mude BackgroundColor e Scale.")
            .Hint("O estado Normal pode ficar vazio: <VisualState Name=\"Normal\" />. Ele 'desfaz' os setters do PointerOver quando o mouse sai.")
            .Check("Existe o estado PointerOver", ws => Named<Border>(ws, "Card", b => That(FindState(b, "PointerOver") is not null, "ok", "Não encontrei um VisualState chamado PointerOver no Card.")))
            .Check("PointerOver tem pelo menos 1 Setter", ws => Named<Border>(ws, "Card", b => That(FindState(b, "PointerOver")?.Setters.Count > 0, "ok", "O estado PointerOver não tem Setters.")))
            .Check("Existe o estado Normal", ws => Named<Border>(ws, "Card", b => That(FindState(b, "Normal") is not null, "ok", "Adicione <VisualState Name=\"Normal\" /> para o card voltar ao normal.")))
            .WithSolution("""
                <Border x:Name="Card" ...>
                    <VisualStateManager.VisualStateGroups>
                        <VisualStateGroup Name="CommonStates">
                            <VisualState Name="Normal" />
                            <VisualState Name="PointerOver">
                                <VisualState.Setters>
                                    <Setter Property="BackgroundColor" Value="#512BD4" />
                                    <Setter Property="Scale" Value="1.05" />
                                </VisualState.Setters>
                            </VisualState>
                        </VisualStateGroup>
                    </VisualStateManager.VisualStateGroups>
                    <Label Text="Passe o mouse aqui" ... />
                </Border>
                """),

        new ChallengeInfo("d11", 11, "Cantos e sombra", "Fácil", Workspaces + "D11Sombra.xaml", () => new D11Sombra())
            .WithGoal("Combine StrokeShape e Shadow para criar um cartão de foto moderno.")
            .Lesson("graficos")
            .Step("Troque StrokeShape=\"Rectangle\" por \"RoundRectangle 16\".")
            .Step("Adicione <Border.Shadow> com um Shadow de Radius 10+.")
            .Hint("O Border recorta o conteúdo pela forma: a imagem também ganha os cantos arredondados.")
            .Check("Cantos arredondados (≥ 16)", ws => Named<Border>(ws, "Foto", b => That(b.StrokeShape is Microsoft.Maui.Controls.Shapes.RoundRectangle rr && rr.CornerRadius.TopLeft >= 16, "ok", $"StrokeShape atual: {b.StrokeShape?.GetType().Name}")))
            .Check("Sombra com Radius ≥ 10", ws => Named<Border>(ws, "Foto", b => That(b.Shadow is { Radius: >= 10 }, "ok", b.Shadow is null ? "Sem Shadow." : $"Shadow.Radius atual: {b.Shadow.Radius}")))
            .WithSolution("""
                <Border x:Name="Foto" WidthRequest="300" HeightRequest="190"
                        HorizontalOptions="Center" VerticalOptions="Center"
                        StrokeShape="RoundRectangle 16" StrokeThickness="0">
                    <Border.Shadow>
                        <Shadow Radius="16" Opacity="0.4" Offset="0,6" />
                    </Border.Shadow>
                    <Image Source="sample_landscape.png" Aspect="AspectFill" />
                </Border>
                """),

        new ChallengeInfo("d12", 12, "MVVM: ligue o contador", "Difícil", Workspaces + "D12Mvvm.xaml", () => new D12Mvvm())
            .WithGoal("Conecte uma View a um ViewModel existente com bindings de propriedade e de comando.")
            .Lesson("mvvm")
            .Step("Label \"Contagem\": Text=\"{Binding Count}\".")
            .Step("Botões: Command=\"{Binding IncrementCommand}\" e Command=\"{Binding DecrementCommand}\".")
            .Step("Clique nos botões: repare que \"−\" fica desabilitado em 0 (CanExecute).")
            .Hint("O BindingContext já é um CounterViewModel (definido no code-behind). Você só escreve os bindings.")
            .Check("Botão Mais tem um Command", ws => Named<Button>(ws, "Mais", b => That(b.Command is not null, "ok", "O botão Mais ainda não tem Command.")))
            .Check("Clicar em Mais atualiza o Label", ws => Named<Button>(ws, "Mais", b => Named<Label>(ws, "Contagem", l =>
            {
                if (ws.BindingContext is not CounterViewModel vm) return Fail("BindingContext não é CounterViewModel.");
                vm.Count = 0;
                b.Command?.Execute(b.CommandParameter);
                b.Command?.Execute(b.CommandParameter);
                var ok = vm.Count == 2 && l.Text == "2";
                return That(ok, "ok", $"Após 2 cliques: Count={vm.Count}, Label=\"{l.Text}\" (esperado 2 e \"2\").");
            })))
            .Check("Menos desabilita em 0 (CanExecute)", ws => Named<Button>(ws, "Menos", b =>
            {
                if (ws.BindingContext is not CounterViewModel vm) return Fail("BindingContext não é CounterViewModel.");
                vm.Count = 0;
                return That(b.Command is not null && !b.IsEnabled, "ok", b.Command is null ? "O botão Menos ainda não tem Command." : "Com Count=0, o botão Menos deveria estar desabilitado.");
            }))
            .WithSolution("""
                <Label x:Name="Contagem" Text="{Binding Count}" FontSize="56" HorizontalOptions="Center" />
                <HorizontalStackLayout Spacing="10">
                    <Button x:Name="Menos" Text="−" WidthRequest="70" Command="{Binding DecrementCommand}" />
                    <Button x:Name="Mais" Text="+" WidthRequest="70" Command="{Binding IncrementCommand}" />
                </HorizontalStackLayout>
                """),

        new ChallengeInfo("d13", 13, "Duplo clique", "Fácil", Workspaces + "D13DuploClique.xaml", () => new D13DuploClique())
            .WithGoal("Adicione um reconhecedor de gesto e ligue-o a um método do code-behind.")
            .Lesson("gestos")
            .Step("Adicione <BoxView.GestureRecognizers> ao BoxView \"Alvo\".")
            .Step("Dentro, um TapGestureRecognizer com NumberOfTapsRequired=\"2\" e Tapped=\"OnDuploClique\".")
            .Hint("Adicionar um handler de evento novo pelo XAML pode exigir \"↻ Recarregar área\" ou reiniciar, dependendo da IDE.")
            .Check("Tem um TapGestureRecognizer de 2 toques", ws => Named<View>(ws, "Alvo", v => That(v.GestureRecognizers.OfType<TapGestureRecognizer>().Any(t => t.NumberOfTapsRequired == 2), "ok", "Não encontrei TapGestureRecognizer com NumberOfTapsRequired=\"2\" no Alvo.")))
            .WithSolution("""
                <BoxView x:Name="Alvo" Color="#512BD4" WidthRequest="140" HeightRequest="140" CornerRadius="20">
                    <BoxView.GestureRecognizers>
                        <TapGestureRecognizer NumberOfTapsRequired="2" Tapped="OnDuploClique" />
                    </BoxView.GestureRecognizers>
                </BoxView>
                """),

        new ChallengeInfo("d14", 14, "Layout responsivo", "Difícil", Workspaces + "D14Responsivo.xaml", () => new D14Responsivo())
            .WithGoal("Use AdaptiveTrigger para trocar a orientação do layout conforme a largura da janela.")
            .Lesson("triggers")
            .Step("Adicione um VisualStateGroup ao StackLayout \"Painel\" com dois estados.")
            .Step("Estado \"Larga\": AdaptiveTrigger MinWindowWidth=\"1000\" e Setter Orientation=Horizontal.")
            .Step("Estado \"Estreita\": AdaptiveTrigger MinWindowWidth=\"0\" e Setter Orientation=Vertical.")
            .Step("Redimensione a janela para ver a troca.")
            .Hint("O trigger com o MAIOR MinWindowWidth que for satisfeito vence.")
            .Check("Existe um AdaptiveTrigger com MinWindowWidth ≥ 1000", ws => Named<StackLayout>(ws, "Painel", p => That(AllStates(p).Any(s => s.StateTriggers.OfType<AdaptiveTrigger>().Any(t => t.MinWindowWidth >= 1000)), "ok", "Não encontrei AdaptiveTrigger MinWindowWidth=\"1000\".")))
            .Check("Algum estado define Orientation=Horizontal", ws => Named<StackLayout>(ws, "Painel", p => That(AllStates(p).Any(s => s.Setters.Any(x => x.Property == StackLayout.OrientationProperty && Equals(x.Value, StackOrientation.Horizontal))), "ok", "Nenhum Setter de Orientation=Horizontal.")))
            .Check("A orientação atual combina com a janela", ws => Named<StackLayout>(ws, "Painel", p =>
            {
                var width = p.Window?.Width ?? 0;
                var expected = width >= 1000 ? StackOrientation.Horizontal : StackOrientation.Vertical;
                return That(p.Orientation == expected, "ok", $"Janela com {width:F0} de largura → esperado {expected}, atual {p.Orientation}.");
            }))
            .WithSolution("""
                <StackLayout x:Name="Painel" Spacing="10" HorizontalOptions="Center" VerticalOptions="Center">
                    <VisualStateManager.VisualStateGroups>
                        <VisualStateGroup Name="Tamanho">
                            <VisualState Name="Larga">
                                <VisualState.StateTriggers>
                                    <AdaptiveTrigger MinWindowWidth="1000" />
                                </VisualState.StateTriggers>
                                <VisualState.Setters>
                                    <Setter Property="Orientation" Value="Horizontal" />
                                </VisualState.Setters>
                            </VisualState>
                            <VisualState Name="Estreita">
                                <VisualState.StateTriggers>
                                    <AdaptiveTrigger MinWindowWidth="0" />
                                </VisualState.StateTriggers>
                                <VisualState.Setters>
                                    <Setter Property="Orientation" Value="Vertical" />
                                </VisualState.Setters>
                            </VisualState>
                        </VisualStateGroup>
                    </VisualStateManager.VisualStateGroups>
                    <!-- BoxViews -->
                </StackLayout>
                """),
    ];

    private static IEnumerable<VisualState> AllStates(VisualElement element) =>
        VisualStateManager.GetVisualStateGroups(element)?.SelectMany(g => g.States) ?? [];

    private static VisualState? FindState(VisualElement element, string name) =>
        AllStates(element).FirstOrDefault(s => s.Name == name);

    private static string Describe(IItemsLayout? layout) => layout switch
    {
        GridItemsLayout g => $"{g.Orientation}Grid, {g.Span}",
        LinearItemsLayout l => $"{l.Orientation}List",
        null => "null",
        _ => layout.GetType().Name,
    };
}
