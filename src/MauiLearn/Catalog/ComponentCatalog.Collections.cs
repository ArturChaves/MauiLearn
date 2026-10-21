using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Shapes;

namespace MauiLearn.Catalog;

public static partial class ComponentCatalog
{
    private static void AddCollections(List<ComponentInfo> list)
    {
        const ComponentCategory cat = ComponentCategory.Collections;

        C(list, "CollectionView", cat, "🗂️", "Lista/grade virtualizada de itens — o controle de coleção principal.")
            .About("Exibe ItemsSource com ItemTemplate, com virtualização (só cria as views visíveis). Suporta layout vertical/horizontal/grade, seleção simples ou múltipla, " +
                   "agrupamento, cabeçalho/rodapé, EmptyView, rolagem infinita (RemainingItemsThreshold) e reordenação.")
            .Demo(() =>
            {
                var items = new ObservableCollection<Fruit>(Fruits());
                var cv = new CollectionView
                {
                    HeightRequest = 280,
                    ItemsSource = items,
                    SelectionMode = SelectionMode.Single,
                    ItemsLayout = new GridItemsLayout(2, ItemsLayoutOrientation.Vertical) { HorizontalItemSpacing = 6, VerticalItemSpacing = 6 },
                    EmptyView = "Nenhuma fruta 😢",
                    Header = new Label { Text = "Cabeçalho (Header)", FontAttributes = FontAttributes.Bold, Padding = 4 },
                    ItemTemplate = FruitTemplate(),
                };
                var add = new Button { Text = "+ Adicionar" };
                add.Clicked += (_, _) => items.Add(new Fruit($"Nova {items.Count + 1}", "🍏", "Adicionada agora", 1));
                var clear = new Button { Text = "Limpar (EmptyView)" };
                clear.Clicked += (_, _) => items.Clear();
                var root = new VerticalStackLayout { Spacing = 8, WidthRequest = 460, Children = { new HorizontalStackLayout { Spacing = 8, Children = { add, clear } }, cv } };
                return new DemoInstance(root, cv);
            })
            .Platform("Microsoft.UI.Xaml.Controls.ItemsRepeater/ListView", "UIKit.UICollectionView")
            .Prop("ItemsSource", "A coleção (use ObservableCollection para refletir inserções/remoções).")
            .Prop("ItemTemplate", "DataTemplate de cada item (ou um DataTemplateSelector).")
            .Prop("ItemsLayout", "VerticalList, HorizontalList, \"VerticalGrid, 2\" ou objetos LinearItemsLayout/GridItemsLayout com espaçamentos.")
            .Prop("SelectionMode", "None, Single, Multiple.")
            .Prop("SelectedItem / SelectedItems", "Seleção (bindável).")
            .Prop("SelectionChangedCommand", "Comando executado ao mudar a seleção.")
            .Prop("EmptyView / EmptyViewTemplate", "O que mostrar quando a coleção está vazia.")
            .Prop("Header / Footer", "Conteúdo antes/depois dos itens (rola junto).")
            .Prop("IsGrouped / GroupHeaderTemplate", "Agrupamento: ItemsSource vira uma coleção de grupos (cada grupo é uma lista).")
            .Prop("RemainingItemsThreshold / RemainingItemsThresholdReachedCommand", "Rolagem infinita: carregue mais itens quando faltar N para o fim.")
            .Prop("ItemSizingStrategy", "MeasureAllItems (padrão) ou MeasureFirstItem (mais rápido se todos têm o mesmo tamanho).")
            .Prop("CanReorderItems", "Permite reordenar arrastando.")
            .Event("SelectionChanged", "Com PreviousSelection e CurrentSelection.")
            .Event("Scrolled", "Durante a rolagem (offsets e índices visíveis).")
            .Nuance("Precisa de uma altura limitada: dentro de StackLayout/ScrollView ele recebe altura infinita, cria TODOS os itens e perde a virtualização. Coloque-o numa linha '*' de um Grid.")
            .Nuance("Não há 'ItemTapped' como no ListView: use SelectionMode + SelectionChanged ou um TapGestureRecognizer no template.")
            .Nuance("Use x:DataType no DataTemplate para compiled bindings — mais rápido e verificado no build (avisos MAUIG2045 quando um caminho não existe).")
            .Nuance("Para rolar até um item: collectionView.ScrollTo(item, position: ScrollToPosition.Center, animate: true).")
            .Sample("""
                <CollectionView ItemsSource="{Binding Frutas}"
                                SelectionMode="Single"
                                SelectedItem="{Binding Selecionada}"
                                ItemsLayout="VerticalGrid, 2"
                                EmptyView="Nada por aqui">
                    <CollectionView.ItemTemplate>
                        <DataTemplate x:DataType="models:Fruta">
                            <Border Padding="10">
                                <VerticalStackLayout>
                                    <Label Text="{Binding Emoji}" FontSize="32" />
                                    <Label Text="{Binding Nome}" FontAttributes="Bold" />
                                </VerticalStackLayout>
                            </Border>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>
                """)
            .Docs("user-interface/controls/collectionview/")
            .Lesson("colecoes");

        C(list, "CarouselView", cat, "🎠", "Carrossel: um item por vez, navegação por deslize ou código.")
            .About("Baseado no mesmo motor do CollectionView. Position e CurrentItem indicam o item atual. Combine com IndicatorView para as 'bolinhas'.")
            .Demo(() =>
            {
                var carousel = new CarouselView
                {
                    HeightRequest = 190,
                    WidthRequest = 360,
                    ItemsSource = Fruits(),
                    PeekAreaInsets = new Thickness(30, 0),
                    Loop = false,
                    ItemTemplate = new DataTemplate(() =>
                    {
                        var emoji = new Label { FontSize = 56, HorizontalOptions = LayoutOptions.Center };
                        emoji.SetBinding(Label.TextProperty, static (Fruit f) => f.Emoji);
                        var name = new Label { FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center };
                        name.SetBinding(Label.TextProperty, static (Fruit f) => f.Name);
                        return new Border
                        {
                            Margin = 6,
                            Padding = 12,
                            StrokeShape = new RoundRectangle { CornerRadius = 16 },
                            BackgroundColor = Color.FromArgb("#22512BD4"),
                            Content = new VerticalStackLayout { Spacing = 4, VerticalOptions = LayoutOptions.Center, Children = { emoji, name } },
                        };
                    }),
                };
                var indicator = new IndicatorView { IndicatorColor = Colors.LightGray, SelectedIndicatorColor = Color.FromArgb("#512BD4"), HorizontalOptions = LayoutOptions.Center, IndicatorSize = 8 };
                carousel.IndicatorView = indicator;
                var prev = new Button { Text = "◀" };
                prev.Clicked += (_, _) => carousel.Position = Math.Max(0, carousel.Position - 1);
                var next = new Button { Text = "▶" };
                next.Clicked += (_, _) => carousel.Position = Math.Min(9, carousel.Position + 1);
                var root = new VerticalStackLayout
                {
                    Spacing = 6,
                    Children = { carousel, indicator, new HorizontalStackLayout { Spacing = 8, HorizontalOptions = LayoutOptions.Center, Children = { prev, next } } },
                };
                return new DemoInstance(root, carousel);
            })
            .Prop("ItemsSource / ItemTemplate", "Dados e aparência.")
            .Prop("Position / CurrentItem", "Item atual (bindáveis).")
            .Prop("Loop", "Volta ao início depois do último.")
            .Prop("PeekAreaInsets", "Mostra uma 'espiada' dos itens vizinhos.")
            .Prop("IsSwipeEnabled / IsBounceEnabled", "Controle do gesto.")
            .Prop("IndicatorView", "Liga um IndicatorView ao carrossel.")
            .Prop("ItemsLayout", "LinearItemsLayout com SnapPointsType/Alignment e ItemSpacing.")
            .Event("CurrentItemChanged / PositionChanged", "Quando o item atual muda.")
            .Nuance("No desktop, ofereça botões ◀ ▶ ou atalhos de teclado: arrastar com mouse não é tão natural quanto deslizar com o dedo.")
            .Nuance("Loop=True pode deixar Position 'estranha' durante a animação; prefira CurrentItem para lógica de negócio.")
            .Sample("""
                <CarouselView ItemsSource="{Binding Slides}" IndicatorView="indicador" PeekAreaInsets="40,0">
                    <CarouselView.ItemTemplate>
                        <DataTemplate x:DataType="models:Slide">
                            <Image Source="{Binding Imagem}" Aspect="AspectFill" />
                        </DataTemplate>
                    </CarouselView.ItemTemplate>
                </CarouselView>
                <IndicatorView x:Name="indicador" SelectedIndicatorColor="#512BD4" />
                """)
            .Docs("user-interface/controls/carouselview/");

        C(list, "IndicatorView", cat, "⚫", "Indicadores de posição (bolinhas) para CarouselView.")
            .About("Mostra quantos itens existem e qual está ativo. Pode ser ligado a um CarouselView (IndicatorView=\"...\") ou usado isoladamente com Count/Position.")
            .Demo(() => new IndicatorView
            {
                Count = 6,
                Position = 2,
                IndicatorSize = 14,
                IndicatorColor = Colors.LightGray,
                SelectedIndicatorColor = Color.FromArgb("#512BD4"),
                IndicatorsShape = IndicatorShape.Circle,
            })
            .Prop("Count / Position", "Total e índice ativo.")
            .Prop("IndicatorColor / SelectedIndicatorColor", "Cores.")
            .Prop("IndicatorSize", "Tamanho de cada indicador.")
            .Prop("IndicatorsShape", "Circle ou Square.")
            .Prop("IndicatorTemplate", "Template customizado (ex.: um ícone).")
            .Prop("MaximumVisible / HideSingle", "Limite de indicadores e se esconde com 1 item.")
            .Sample("""<IndicatorView x:Name="indicador" IndicatorsShape="Square" IndicatorSize="10" />""")
            .Docs("user-interface/controls/indicatorview");

        C(list, "ListView", cat, "📃", "Lista clássica com células — OBSOLETO, use CollectionView.")
            .Obsolete("ListView, TableView e as Cells (TextCell, ImageCell, SwitchCell, EntryCell, ViewCell) estão [Obsolete] no .NET MAUI 10+.")
            .About("Herdado do Xamarin.Forms. Usa Cells em vez de Views, tem ItemTapped, pull-to-refresh embutido e ContextActions. Mantido aqui para você reconhecer código legado.")
            .Demo(() => new ListView
            {
                HeightRequest = 220,
                WidthRequest = 360,
                ItemsSource = Fruits(),
                ItemTemplate = new DataTemplate(() =>
                {
                    var cell = new TextCell();
                    cell.SetBinding(TextCell.TextProperty, static (Fruit f) => f.Name);
                    cell.SetBinding(TextCell.DetailProperty, static (Fruit f) => f.Description);
                    return cell;
                }),
            })
            .Prop("ItemsSource / ItemTemplate", "Dados e template (com Cells).")
            .Prop("HasUnevenRows / RowHeight", "Altura das linhas.")
            .Prop("IsPullToRefreshEnabled / IsRefreshing", "Atualização embutida.")
            .Prop("SeparatorVisibility / SeparatorColor", "Separadores.")
            .Event("ItemTapped / ItemSelected", "Toque e seleção.")
            .Nuance("Migração: ListView → CollectionView; ViewCell → conteúdo direto do DataTemplate; ContextActions → SwipeView ou ContextFlyout; IsPullToRefreshEnabled → RefreshView.")
            .Sample("""
                <!-- Legado -->
                <ListView ItemsSource="{Binding Itens}" ItemTapped="OnTap">
                    <ListView.ItemTemplate>
                        <DataTemplate><TextCell Text="{Binding Nome}" /></DataTemplate>
                    </ListView.ItemTemplate>
                </ListView>
                """)
            .Docs("user-interface/controls/listview");

        C(list, "TableView", cat, "📋", "Tabela de seções e células (formulários/configurações) — OBSOLETO.")
            .Obsolete("TableView está [Obsolete] no .NET MAUI 10+. Monte formulários com Grid/VerticalStackLayout.")
            .About("Organiza Cells em TableSections dentro de um TableRoot. Era usado para telas de configurações.")
            .Demo(() => new TableView
            {
                Intent = TableIntent.Settings,
                HeightRequest = 260,
                WidthRequest = 380,
                Root = new TableRoot
                {
                    new TableSection("Conta") { new TextCell { Text = "Nome", Detail = "Artur" }, new EntryCell { Label = "E-mail", Placeholder = "voce@exemplo.com" } },
                    new TableSection("Preferências") { new SwitchCell { Text = "Notificações", On = true } },
                },
            })
            .Prop("Root", "TableRoot com TableSections.")
            .Prop("Intent", "Data, Form, Menu, Settings — dica de estilo.")
            .Prop("HasUnevenRows / RowHeight", "Altura das linhas.")
            .Nuance("Substitua por um layout simples: cada 'célula' vira um Grid com Label + controle.")
            .Sample("""
                <TableView Intent="Settings">
                    <TableRoot>
                        <TableSection Title="Geral">
                            <SwitchCell Text="Modo escuro" On="{Binding Escuro}" />
                        </TableSection>
                    </TableRoot>
                </TableView>
                """)
            .Docs("user-interface/controls/tableview");
    }

    internal static DataTemplate FruitTemplate() => new(() =>
    {
        var emoji = new Label { FontSize = 30, VerticalOptions = LayoutOptions.Center };
        emoji.SetBinding(Label.TextProperty, static (Fruit f) => f.Emoji);
        var name = new Label { FontAttributes = FontAttributes.Bold };
        name.SetBinding(Label.TextProperty, static (Fruit f) => f.Name);
        var price = new Label { FontSize = 12, Opacity = 0.7 };
        price.SetBinding(Label.TextProperty, static (Fruit f) => f.PriceText);
        var grid = new Grid
        {
            ColumnDefinitions = Columns("Auto,*"),
            ColumnSpacing = 10,
            Padding = 10,
        };
        grid.Add(emoji, 0, 0);
        grid.Add(new VerticalStackLayout { Children = { name, price } }, 1, 0);

        var border = new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            Content = grid,
        };
        // Destaca o item selecionado via VisualStateManager (estado "Selected" do CollectionView).
        VisualStateManager.SetVisualStateGroups(border,
        [
            new VisualStateGroup
            {
                Name = "CommonStates",
                States =
                {
                    new VisualState { Name = "Normal" },
                    new VisualState { Name = "Selected", Setters = { new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromArgb("#33512BD4") } } },
                    new VisualState { Name = "PointerOver", Setters = { new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromArgb("#14512BD4") } } },
                },
            },
        ]);
        return border;
    });
}
