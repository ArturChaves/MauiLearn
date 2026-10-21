using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

namespace MauiLearn.Catalog;

public static partial class ComponentCatalog
{
    private static void AddLayouts(List<ComponentInfo> list)
    {
        const ComponentCategory cat = ComponentCategory.Layouts;

        C(list, "VerticalStackLayout", cat, "⬇️", "Empilha filhos verticalmente, um embaixo do outro.")
            .About("O layout mais usado. Mede cada filho com altura 'infinita' e os posiciona em sequência. " +
                   "É mais leve que o StackLayout porque não tem a propriedade Orientation.")
            .Demo(() =>
            {
                var s = new VerticalStackLayout { Spacing = 8, Padding = 8, BackgroundColor = Color.FromArgb("#22512BD4") };
                foreach (var c in Palette.Take(4)) s.Add(Box(c, 40));
                return s;
            })
            .Prop("Spacing", "Espaço (em unidades independentes de dispositivo) entre cada filho.")
            .Prop("Padding", "Espaço interno entre a borda do layout e os filhos.")
            .Prop("HorizontalOptions (nos filhos)", "Start/Center/End/Fill posicionam o filho na largura disponível.")
            .Nuance("Em um VerticalStackLayout, VerticalOptions dos filhos não tem efeito: a pilha dá a cada filho exatamente a altura que ele pede.")
            .Nuance("Não coloque um CollectionView/ScrollView dentro de um StackLayout: a pilha oferece altura infinita e a virtualização/rolagem quebra. Use Grid com linha '*'.")
            .Nuance("Spacing é aplicado entre filhos visíveis; filhos com IsVisible=False não ocupam espaço nem recebem espaçamento.")
            .Sample("""
                <VerticalStackLayout Spacing="10" Padding="20">
                    <Label Text="Título" FontSize="24" />
                    <Entry Placeholder="Nome" />
                    <Button Text="Salvar" HorizontalOptions="End" />
                </VerticalStackLayout>
                """)
            .Docs("user-interface/layouts/verticalstacklayout")
            .Lesson("layouts");

        C(list, "HorizontalStackLayout", cat, "➡️", "Empilha filhos horizontalmente, lado a lado.")
            .About("Versão horizontal da pilha. Mede cada filho com largura 'infinita'; se a soma for maior que o espaço, os filhos são cortados (não quebram linha — para isso use FlexLayout com Wrap).")
            .Demo(() =>
            {
                var s = new HorizontalStackLayout { Spacing = 8, Padding = 8, BackgroundColor = Color.FromArgb("#22512BD4") };
                foreach (var c in Palette.Take(4)) s.Add(Box(c, 40));
                return s;
            })
            .Prop("Spacing", "Espaço horizontal entre os filhos.")
            .Prop("VerticalOptions (nos filhos)", "Alinha cada filho verticalmente dentro da altura da pilha.")
            .Nuance("HorizontalOptions dos filhos é ignorado na direção do empilhamento.")
            .Nuance("Texto longo numa Label dentro de HorizontalStackLayout não quebra linha, pois a largura oferecida é infinita. Use Grid com coluna '*'.")
            .Sample("""
                <HorizontalStackLayout Spacing="8">
                    <Image Source="maui_logo.png" HeightRequest="32" />
                    <Label Text="MauiLearn" VerticalOptions="Center" />
                </HorizontalStackLayout>
                """)
            .Docs("user-interface/layouts/horizontalstacklayout")
            .Lesson("layouts");

        C(list, "StackLayout", cat, "🥞", "Pilha com orientação configurável (Vertical/Horizontal).")
            .About("O StackLayout clássico. Funciona como os dois anteriores, mas com Orientation. Prefira Vertical/HorizontalStackLayout quando a orientação é fixa: são mais rápidos.")
            .Demo(() =>
            {
                var s = new StackLayout { Spacing = 8, Padding = 8, Orientation = StackOrientation.Horizontal, BackgroundColor = Color.FromArgb("#22512BD4") };
                foreach (var c in Palette.Take(3)) s.Add(Box(c, 40));
                return s;
            })
            .Prop("Orientation", "Vertical (padrão) ou Horizontal. Experimente trocar no inspetor →")
            .Prop("Spacing", "Espaço entre filhos. No MAUI o padrão é 0 (no Xamarin.Forms era 6).")
            .Nuance("O padrão de Spacing mudou de 6 (Xamarin.Forms) para 0 no MAUI — um detalhe comum em migrações.")
            .Nuance("As opções '...AndExpand' (FillAndExpand etc.) são obsoletas no MAUI. Use Grid para distribuir espaço restante.")
            .Sample("""
                <StackLayout Orientation="Horizontal" Spacing="6">
                    <CheckBox />
                    <Label Text="Lembrar de mim" VerticalOptions="Center" />
                </StackLayout>
                """)
            .Docs("user-interface/layouts/stacklayout")
            .Lesson("layouts");

        C(list, "Grid", cat, "▦", "Organiza filhos em linhas e colunas — o layout mais poderoso.")
            .About("O Grid divide o espaço em linhas/colunas com tamanhos Auto (do conteúdo), absolutos (número) ou proporcionais (*). " +
                   "Filhos usam as propriedades anexadas Grid.Row, Grid.Column, Grid.RowSpan e Grid.ColumnSpan.")
            .Demo(() =>
            {
                var g = new Grid
                {
                    RowDefinitions = Rows("Auto,*,Auto"),
                    ColumnDefinitions = Columns("120,*"),
                    RowSpacing = 6,
                    ColumnSpacing = 6,
                    HeightRequest = 220,
                    BackgroundColor = Color.FromArgb("#22512BD4"),
                };
                var header = Cell("Cabeçalho (ColumnSpan=2)", "#512BD4");
                g.Add(header, 0, 0);
                Grid.SetColumnSpan(header, 2);
                g.Add(Cell("Menu", "#D600AA"), 0, 1);
                g.Add(Cell("Conteúdo (*)", "#2E9D5B"), 1, 1);
                var footer = Cell("Rodapé", "#E59A12");
                g.Add(footer, 0, 2);
                Grid.SetColumnSpan(footer, 2);
                return g;
            })
            .Prop("RowDefinitions / ColumnDefinitions", "Lista de tamanhos. Sintaxe curta: RowDefinitions=\"Auto,*,2*,100\".")
            .Prop("RowSpacing / ColumnSpacing", "Espaço entre linhas/colunas.")
            .Prop("Grid.Row / Grid.Column (anexadas)", "Em qual célula o filho fica. Começam em 0.")
            .Prop("Grid.RowSpan / Grid.ColumnSpan (anexadas)", "Quantas linhas/colunas o filho ocupa.")
            .Nuance("'*' divide o espaço RESTANTE após Auto e valores absolutos. '2*' recebe o dobro de '*'.")
            .Nuance("Vários filhos na mesma célula se sobrepõem — o último adicionado fica por cima. Ótimo para sobreposições (overlay) sem AbsoluteLayout.")
            .Nuance("Uma linha '*' dentro de um pai que oferece altura infinita (ex.: ScrollView vertical) se comporta como Auto.")
            .Nuance("No C#, grid.Add(view, coluna, linha) — atenção: a COLUNA vem antes da LINHA.")
            .Sample("""
                <Grid RowDefinitions="Auto,*" ColumnDefinitions="200,*"
                      RowSpacing="8" ColumnSpacing="8">
                    <Label Text="Cabeçalho" Grid.ColumnSpan="2" />
                    <CollectionView Grid.Row="1" />
                    <ContentView Grid.Row="1" Grid.Column="1" />
                </Grid>
                """)
            .Code("""
                var grid = new Grid
                {
                    RowDefinitions = { new(GridLength.Auto), new(GridLength.Star) },
                    ColumnDefinitions = { new(200), new(GridLength.Star) },
                };
                grid.Add(new Label { Text = "Cabeçalho" }, column: 0, row: 0);
                Grid.SetColumnSpan((BindableObject)grid.Children[0], 2);
                """)
            .Docs("user-interface/layouts/grid")
            .Lesson("grid");

        C(list, "FlexLayout", cat, "🧬", "Layout flexível (como CSS Flexbox): quebra linha, distribui e alinha.")
            .About("Baseado no Flexbox do CSS. Ideal para listas de 'chips', galerias que quebram linha e barras com distribuição de espaço.")
            .Demo(() =>
            {
                var f = new FlexLayout
                {
                    Wrap = FlexWrap.Wrap,
                    JustifyContent = FlexJustify.SpaceBetween,
                    AlignItems = FlexAlignItems.Center,
                    BackgroundColor = Color.FromArgb("#22512BD4"),
                    Padding = 6,
                };
                var i = 0;
                foreach (var word in new[] { "XAML", "C#", "Grid", "Binding", "MVVM", "Shell", "Estilos", "Handlers", "Gestos" })
                {
                    var chip = new Border
                    {
                        Padding = new Thickness(12, 6),
                        Margin = 4,
                        StrokeShape = new RoundRectangle { CornerRadius = 14 },
                        BackgroundColor = Color.FromArgb(Palette[i++ % Palette.Length]),
                        Content = new Label { Text = word, TextColor = Colors.White },
                    };
                    f.Add(chip);
                }
                return f;
            })
            .Prop("Direction", "Row, RowReverse, Column, ColumnReverse — o eixo principal.")
            .Prop("Wrap", "NoWrap (padrão) ou Wrap: quebra para a próxima linha quando falta espaço.")
            .Prop("JustifyContent", "Distribuição no eixo principal: Start, Center, End, SpaceBetween, SpaceAround, SpaceEvenly.")
            .Prop("AlignItems", "Alinhamento no eixo cruzado de cada linha: Stretch, Center, Start, End.")
            .Prop("AlignContent", "Com Wrap, como as LINHAS são distribuídas no eixo cruzado.")
            .Prop("FlexLayout.Grow / Shrink / Basis (anexadas)", "Quanto cada filho cresce/encolhe e seu tamanho base.")
            .Nuance("Direction=Column dentro de um ScrollView vertical não quebra linha: a altura é infinita, então nunca 'falta' espaço.")
            .Nuance("FlexLayout é mais custoso que Stack/Grid. Para listas grandes, prefira CollectionView com GridItemsLayout.")
            .Sample("""
                <FlexLayout Wrap="Wrap" JustifyContent="SpaceAround" AlignItems="Center">
                    <Label Text="Um" />
                    <Label Text="Dois" FlexLayout.Grow="1" />
                    <Label Text="Três" FlexLayout.Basis="30%" />
                </FlexLayout>
                """)
            .Docs("user-interface/layouts/flexlayout")
            .Lesson("layouts");

        C(list, "AbsoluteLayout", cat, "📍", "Posiciona filhos por coordenadas e tamanhos (absolutos ou proporcionais).")
            .About("Cada filho define AbsoluteLayout.LayoutBounds (x, y, largura, altura) e LayoutFlags, que diz quais valores são proporcionais (0..1) ao tamanho do layout.")
            .Demo(() =>
            {
                var a = new AbsoluteLayout { HeightRequest = 220, BackgroundColor = Color.FromArgb("#22512BD4") };
                var b1 = Box("#512BD4", 60);
                AbsoluteLayout.SetLayoutBounds(b1, new Rect(0, 0, 60, 60));
                var b2 = Box("#D600AA", 60);
                AbsoluteLayout.SetLayoutBounds(b2, new Rect(0.5, 0.5, 60, 60));
                AbsoluteLayout.SetLayoutFlags(b2, AbsoluteLayoutFlags.PositionProportional);
                var b3 = new BoxView { Color = Color.FromArgb("#2E9D5B") };
                AbsoluteLayout.SetLayoutBounds(b3, new Rect(1, 1, 0.4, 0.2));
                AbsoluteLayout.SetLayoutFlags(b3, AbsoluteLayoutFlags.All);
                a.Add(b1); a.Add(b2); a.Add(b3);
                var legend = new Label { Text = "(0,0) absoluto · (0.5,0.5) centro proporcional · (1,1,0.4,0.2) All", FontSize = 11 };
                AbsoluteLayout.SetLayoutBounds(legend, new Rect(0, 1, 1, AbsoluteLayout.AutoSize));
                AbsoluteLayout.SetLayoutFlags(legend, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);
                a.Add(legend);
                return a;
            })
            .Prop("AbsoluteLayout.LayoutBounds (anexada)", "Rect \"x,y,largura,altura\". Use AutoSize para tamanho do conteúdo.")
            .Prop("AbsoluteLayout.LayoutFlags (anexada)", "None, XProportional, YProportional, PositionProportional, SizeProportional, All...")
            .Nuance("Com posição proporcional, x=0.5 significa CENTRALIZADO (não 'metade da largura'): o MAUI considera o tamanho do filho no cálculo.")
            .Nuance("Para sobreposições simples (badge em cima de um ícone), um Grid com vários filhos na mesma célula costuma ser mais simples e responsivo.")
            .Sample("""
                <AbsoluteLayout>
                    <BoxView Color="Purple"
                             AbsoluteLayout.LayoutBounds="0.5,0.5,100,100"
                             AbsoluteLayout.LayoutFlags="PositionProportional" />
                </AbsoluteLayout>
                """)
            .Docs("user-interface/layouts/absolutelayout")
            .Lesson("layouts");

        C(list, "ScrollView", cat, "📜", "Adiciona rolagem a um conteúdo maior que o espaço disponível.")
            .About("Envolve UM filho (Content). Oferece espaço infinito na direção da rolagem. No desktop, rola com a roda do mouse/trackpad e mostra barras de rolagem.")
            .Demo(() =>
            {
                var content = new VerticalStackLayout { Spacing = 4 };
                for (var i = 1; i <= 30; i++) content.Add(new Label { Text = $"Linha {i} — role com o mouse/trackpad" });
                return new ScrollView { HeightRequest = 200, Content = content, BackgroundColor = Color.FromArgb("#22512BD4") };
            })
            .Prop("Orientation", "Vertical, Horizontal, Both ou Neither.")
            .Prop("VerticalScrollBarVisibility / HorizontalScrollBarVisibility", "Default, Always ou Never.")
            .Prop("ScrollX / ScrollY (somente leitura)", "Posição atual da rolagem.")
            .Event("Scrolled", "Disparado durante a rolagem, com ScrollX/ScrollY.")
            .Nuance("Use ScrollToAsync(x, y, animated) ou ScrollToAsync(elemento, ScrollToPosition, animated) para rolar por código.")
            .Nuance("Nunca coloque CollectionView dentro de ScrollView: o CollectionView já rola sozinho e perde a virtualização.")
            .Sample("""
                <ScrollView>
                    <VerticalStackLayout Padding="20" Spacing="12">
                        <!-- conteúdo longo -->
                    </VerticalStackLayout>
                </ScrollView>
                """)
            .Docs("user-interface/controls/scrollview")
            .Lesson("layouts");

        C(list, "Border", cat, "🔲", "Contorno, fundo e cantos arredondados em volta de um conteúdo.")
            .About("O substituto moderno do Frame. Desenha um traço (Stroke) com qualquer forma (StrokeShape): RoundRectangle, Ellipse, Path... e aceita Shadow.")
            .Demo(() => new Border
            {
                Padding = 16,
                Stroke = new SolidColorBrush(Color.FromArgb("#512BD4")),
                StrokeThickness = 3,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(20, 0, 0, 20) },
                Background = new LinearGradientBrush([new GradientStop(Color.FromArgb("#33D600AA"), 0), new GradientStop(Color.FromArgb("#33512BD4"), 1)], new Point(0, 0), new Point(1, 1)),
                Shadow = new Shadow { Radius = 12, Opacity = 0.35f, Offset = new Point(4, 4) },
                Content = new Label { Text = "Conteúdo dentro de um Border", HorizontalOptions = LayoutOptions.Center },
                HorizontalOptions = LayoutOptions.Center,
            })
            .Prop("Stroke", "Brush do contorno.")
            .Prop("StrokeThickness", "Espessura do contorno.")
            .Prop("StrokeShape", "Forma: \"RoundRectangle 10\", \"RoundRectangle 20,0,0,20\", \"Ellipse\", \"Rectangle\"...")
            .Prop("StrokeDashArray / StrokeDashOffset", "Linha tracejada, ex.: \"4,2\".")
            .Prop("Background / BackgroundColor", "Preenchimento (Brush aceita gradientes).")
            .Prop("Padding", "Espaço entre o contorno e o conteúdo.")
            .Nuance("O conteúdo é recortado (clip) pela StrokeShape — útil para imagens com cantos arredondados.")
            .Nuance("Border tem UM filho (Content). Para vários, coloque um layout dentro.")
            .Sample("""
                <Border Stroke="#512BD4" StrokeThickness="2" Padding="16"
                        StrokeShape="RoundRectangle 12">
                    <Border.Shadow>
                        <Shadow Radius="10" Opacity="0.3" Offset="4,4" />
                    </Border.Shadow>
                    <Label Text="Cartão" />
                </Border>
                """)
            .Docs("user-interface/controls/border")
            .Lesson("graficos");

        C(list, "ContentView", cat, "📦", "Contêiner de um único filho — base para criar controles reutilizáveis.")
            .About("Sozinho, apenas hospeda um Content. Seu grande uso é como classe base de controles customizados (XAML + BindableProperties), como o CodeBlock e o TipCard deste app.")
            .Demo(() => new ContentView
            {
                Padding = 12,
                BackgroundColor = Color.FromArgb("#22512BD4"),
                Content = new Label { Text = "Eu sou o Content de um ContentView" },
            })
            .Prop("Content", "A view filha.")
            .Prop("ControlTemplate", "Template visual que pode envolver o Content (use ContentPresenter dentro do template).")
            .Nuance("Para controles 'lookless' (aparência trocável), combine ContentView + ControlTemplate + ContentPresenter. Veja a aula de Controles Customizados.")
            .Sample("""
                <!-- CartaoPerfil.xaml -->
                <ContentView x:Class="MeuApp.CartaoPerfil" ...>
                    <HorizontalStackLayout Spacing="8">
                        <Image Source="{Binding Foto, Source={x:Reference this}}" />
                        <Label Text="{Binding Nome, Source={x:Reference this}}" />
                    </HorizontalStackLayout>
                </ContentView>
                """)
            .Docs("user-interface/controls/contentview")
            .Lesson("controles");

        C(list, "Frame", cat, "🖼️", "Contêiner com borda e sombra — OBSOLETO, use Border.")
            .About("Herdado do Xamarin.Forms. Tem CornerRadius, BorderColor e HasShadow, mas é limitado e inconsistente entre plataformas.")
            .Obsolete("Frame está marcado como [Obsolete] no .NET MAUI 10/11. Migre para Border + Shadow.")
            .Demo(() => new Frame
            {
                BorderColor = Color.FromArgb("#512BD4"),
                CornerRadius = 12,
                HasShadow = true,
                Padding = 16,
                Content = new Label { Text = "Frame (obsoleto)" },
                HorizontalOptions = LayoutOptions.Center,
            })
            .Prop("BorderColor", "Cor da borda.")
            .Prop("CornerRadius", "Raio único para todos os cantos (float).")
            .Prop("HasShadow", "Liga/desliga uma sombra padrão da plataforma.")
            .Nuance("Equivalente moderno: <Border Stroke=\"...\" StrokeShape=\"RoundRectangle 12\"><Border.Shadow><Shadow /></Border.Shadow></Border>.")
            .Sample("""
                <!-- Antes (obsoleto) -->
                <Frame BorderColor="Gray" CornerRadius="8" HasShadow="True" />

                <!-- Depois -->
                <Border Stroke="Gray" StrokeShape="RoundRectangle 8">
                    <Border.Shadow><Shadow Radius="8" Opacity="0.3" /></Border.Shadow>
                </Border>
                """)
            .Docs("user-interface/controls/frame");

        C(list, "BindableLayout", cat, "🔗", "Gera filhos de qualquer layout a partir de uma coleção (ItemsSource + ItemTemplate).")
            .About("Não é um controle: são propriedades anexadas (BindableLayout.ItemsSource, ItemTemplate, ItemTemplateSelector, EmptyView) aplicáveis a StackLayout, Grid, FlexLayout... Sem virtualização — use para poucos itens.")
            .Demo(() =>
            {
                var flex = new FlexLayout { Wrap = FlexWrap.Wrap, BackgroundColor = Color.FromArgb("#22512BD4"), Padding = 4 };
                BindableLayout.SetItemTemplate(flex, new DataTemplate(() =>
                {
                    var l = new Label { FontSize = 28, Margin = 6 };
                    l.SetBinding(Label.TextProperty, static (Fruit f) => f.Emoji);
                    ToolTipProperties.SetText(l, "Passe o mouse!");
                    return l;
                }));
                BindableLayout.SetItemsSource(flex, Fruits());
                return flex;
            })
            .Prop("BindableLayout.ItemsSource", "A coleção de dados.")
            .Prop("BindableLayout.ItemTemplate", "DataTemplate de cada item.")
            .Prop("BindableLayout.ItemTemplateSelector", "Escolhe o template por item.")
            .Prop("BindableLayout.EmptyView", "Conteúdo quando a coleção está vazia.")
            .Nuance("Cria TODOS os itens de uma vez (sem virtualização). Para dezenas/centenas de itens, use CollectionView.")
            .Nuance("Se ItemsSource for ObservableCollection, adicionar/remover itens atualiza o layout automaticamente.")
            .Sample("""
                <HorizontalStackLayout BindableLayout.ItemsSource="{Binding Tags}" Spacing="6">
                    <BindableLayout.ItemTemplate>
                        <DataTemplate x:DataType="x:String">
                            <Border Padding="8,4"><Label Text="{Binding .}" /></Border>
                        </DataTemplate>
                    </BindableLayout.ItemTemplate>
                </HorizontalStackLayout>
                """)
            .Docs("user-interface/layouts/bindablelayout")
            .Lesson("colecoes");

        C(list, "RefreshView", cat, "🔄", "Adiciona 'puxar para atualizar' a um conteúdo rolável.")
            .About("Envolve um ScrollView/CollectionView. No desktop o gesto de puxar não é natural; normalmente você define IsRefreshing por código ou usa um botão 'Atualizar'.")
            .Demo(() =>
            {
                var list = new VerticalStackLayout { Spacing = 4, Padding = 8 };
                for (var i = 1; i <= 8; i++) list.Add(new Label { Text = $"Item {i}" });
                var rv = new RefreshView { HeightRequest = 180, Content = new ScrollView { Content = list } };
                rv.Command = new Command(async () => { await Task.Delay(1500); rv.IsRefreshing = false; });
                return rv;
            })
            .Prop("IsRefreshing", "Mostra o indicador. Ligue no inspetor para ver; o Command de demo o desliga após 1,5 s.")
            .Prop("Command", "Executado quando o usuário dispara a atualização.")
            .Prop("RefreshColor", "Cor do indicador.")
            .Event("Refreshing", "Disparado quando a atualização começa.")
            .Nuance("Lembre-se de voltar IsRefreshing para false ao terminar — senão o indicador fica girando para sempre.")
            .Desktop("No Windows o 'pull to refresh' funciona com toque; com mouse, prefira um botão/atalho (F5) que liga IsRefreshing.")
            .Sample("""
                <RefreshView IsRefreshing="{Binding Carregando}" Command="{Binding AtualizarCommand}">
                    <CollectionView ItemsSource="{Binding Itens}" />
                </RefreshView>
                """)
            .Docs("user-interface/controls/refreshview");
    }

    internal static RowDefinitionCollection Rows(string spec) =>
        [.. spec.Split(',').Select(s => new RowDefinition(ParseLength(s)))];

    internal static ColumnDefinitionCollection Columns(string spec) =>
        [.. spec.Split(',').Select(s => new ColumnDefinition(ParseLength(s)))];

    internal static GridLength ParseLength(string s)
    {
        s = s.Trim();
        if (s.Equals("Auto", StringComparison.OrdinalIgnoreCase)) return GridLength.Auto;
        if (s.EndsWith('*'))
            return new GridLength(s.Length == 1 ? 1 : double.Parse(s[..^1], System.Globalization.CultureInfo.InvariantCulture), GridUnitType.Star);
        return new GridLength(double.Parse(s, System.Globalization.CultureInfo.InvariantCulture));
    }

    internal static View Cell(string text, string color) => new Border
    {
        BackgroundColor = Color.FromArgb(color),
        StrokeThickness = 0,
        Padding = 8,
        Content = new Label { Text = text, TextColor = Colors.White, FontSize = 12, VerticalOptions = LayoutOptions.Center },
    };
}

internal static class ComponentInfoExtensions
{
    /// <summary>Adiciona uma nuance específica de desktop (prefixada).</summary>
    public static ComponentInfo Desktop(this ComponentInfo info, string text) => info.Nuance("🖥️ " + text);
}
