using Microsoft.Maui.Controls.Shapes;

namespace MauiLearn.Catalog;

public static partial class ComponentCatalog
{
    private static void AddCommands(List<ComponentInfo> list)
    {
        const ComponentCategory cat = ComponentCategory.Commands;

        C(list, "Button", cat, "🔘", "Botão com texto e/ou imagem que dispara uma ação.")
            .About("Responde a clique via evento Clicked ou via Command (MVVM). Pode ter imagem (ImageSource) posicionada com ContentLayout. " +
                   "Possui os estados visuais Normal, PointerOver, Pressed, Disabled e Focused.")
            .Demo(() => new Button { Text = "Clique em mim", HorizontalOptions = LayoutOptions.Center })
            .Platform("Microsoft.UI.Xaml.Controls.Button", "UIKit.UIButton")
            .Prop("Text", "Texto do botão.")
            .Prop("Command / CommandParameter", "ICommand executado no clique. Se CanExecute retornar false, o botão fica desabilitado automaticamente.")
            .Prop("ImageSource", "Ícone do botão.")
            .Prop("ContentLayout", "Posição do ícone e espaçamento: \"Left, 8\", \"Top, 4\", \"Right\", \"Bottom\".")
            .Prop("BackgroundColor / Background", "Fundo (Background aceita gradientes).")
            .Prop("TextColor / FontSize / FontAttributes", "Aparência do texto.")
            .Prop("CornerRadius / BorderColor / BorderWidth", "Bordas e cantos.")
            .Prop("Padding", "Espaço interno.")
            .Prop("LineBreakMode", "Como o texto longo é quebrado/truncado.")
            .Prop("IsEnabled", "Habilita/desabilita (herdado de VisualElement).")
            .Event("Clicked", "Clique completo (pressionar + soltar).")
            .Event("Pressed / Released", "Início e fim do pressionamento.")
            .Nuance("Se você usar Command, NÃO defina IsEnabled manualmente: o CanExecute do comando sobrescreve IsEnabled.")
            .Nuance("O estilo implícito deste app (Resources/Styles/Styles.xaml) define BackgroundColor e CornerRadius. Clique em ↺ no inspetor: o valor volta ao do ESTILO, não ao padrão do controle.")
            .Nuance("Teste o estado PointerOver passando o mouse: é um VisualState comum no desktop.")
            .Sample("""
                <Button Text="Salvar"
                        ImageSource="salvar.png"
                        ContentLayout="Left, 8"
                        Command="{Binding SalvarCommand}"
                        CommandParameter="{Binding ItemAtual}"
                        CornerRadius="20" />
                """)
            .Code("""
                var botao = new Button { Text = "Salvar" };
                botao.Clicked += async (s, e) => await DisplayAlertAsync("Ok", "Salvo!", "Fechar");
                """)
            .Docs("user-interface/controls/button")
            .Lesson("mvvm");

        C(list, "ImageButton", cat, "🖲️", "Botão cujo conteúdo é apenas uma imagem.")
            .About("Como o Button, mas exibe somente uma imagem (Source) com Aspect. Tem Clicked/Pressed/Released e Command.")
            .Demo(() => new ImageButton
            {
                Source = "maui_logo.png",
                WidthRequest = 80,
                HeightRequest = 80,
                Padding = 8,
                CornerRadius = 16,
                BorderColor = Color.FromArgb("#512BD4"),
                BorderWidth = 2,
                BackgroundColor = Color.FromArgb("#22512BD4"),
            })
            .Platform("Microsoft.UI.Xaml.Controls.Button + Image", "UIKit.UIButton")
            .Prop("Source", "A imagem.")
            .Prop("Aspect", "Encaixe da imagem.")
            .Prop("Padding / CornerRadius / BorderColor / BorderWidth", "Aparência.")
            .Prop("Command / CommandParameter", "Comando no clique.")
            .Event("Clicked / Pressed / Released", "Interação.")
            .Nuance("Sempre defina SemanticProperties.Description (acessibilidade) e ToolTipProperties.Text (desktop) em botões só com ícone.")
            .Sample("""
                <ImageButton Source="config.png" WidthRequest="40" HeightRequest="40"
                             Command="{Binding AbrirConfigCommand}"
                             SemanticProperties.Description="Configurações"
                             ToolTipProperties.Text="Configurações" />
                """)
            .Docs("user-interface/controls/imagebutton");

        C(list, "RadioButton", cat, "🔘", "Escolha única dentro de um grupo.")
            .About("RadioButtons com o mesmo GroupName (ou dentro do mesmo pai com RadioButtonGroup.GroupName) são mutuamente exclusivos. " +
                   "Content pode ser texto OU qualquer View; com ControlTemplate você muda toda a aparência.")
            .Demo(() =>
            {
                var first = new RadioButton { Content = "Café", GroupName = "bebida", IsChecked = true };
                var stack = new VerticalStackLayout
                {
                    Children =
                    {
                        first,
                        new RadioButton { Content = "Chá", GroupName = "bebida" },
                        new RadioButton { Content = "Suco", GroupName = "bebida" },
                    },
                };
                return new DemoInstance(stack, first);
            })
            .Platform("Microsoft.UI.Xaml.Controls.RadioButton", "UIKit (MauiRadioButton, desenhado via template)")
            .Prop("Content", "Texto ou View exibida.")
            .Prop("Value", "Valor associado (útil com RadioButtonGroup.SelectedValue).")
            .Prop("IsChecked", "Marcado ou não.")
            .Prop("GroupName", "Grupo de exclusividade.")
            .Prop("ControlTemplate", "Aparência customizada (ex.: cartões selecionáveis).")
            .Prop("RadioButtonGroup.GroupName / SelectedValue (anexadas no pai)", "Agrupa e faz binding do valor selecionado.")
            .Event("CheckedChanged", "Quando IsChecked muda.")
            .Nuance("O inspetor está editando o PRIMEIRO RadioButton (Café). Marque outro na prévia e veja IsChecked mudar sozinho.")
            .Nuance("Com RadioButtonGroup.SelectedValue=\"{Binding Escolha}\" no layout pai, você evita tratar CheckedChanged de cada botão.")
            .Sample("""
                <VerticalStackLayout RadioButtonGroup.GroupName="tamanho"
                                     RadioButtonGroup.SelectedValue="{Binding Tamanho}">
                    <RadioButton Content="P" Value="P" />
                    <RadioButton Content="M" Value="M" />
                    <RadioButton Content="G" Value="G" />
                </VerticalStackLayout>
                """)
            .Docs("user-interface/controls/radiobutton");

        C(list, "SwipeView", cat, "↔️", "Revela ações ao deslizar um item (ex.: excluir, arquivar).")
            .About("Envolve um conteúdo e define SwipeItems em LeftItems/RightItems/TopItems/BottomItems. Mode=Reveal mostra os botões; Execute dispara a ação ao completar o gesto.")
            .Demo(() =>
            {
                var swipe = new SwipeView
                {
                    WidthRequest = 340,
                    RightItems = new SwipeItems
                    {
                        new SwipeItem { Text = "Excluir", BackgroundColor = Color.FromArgb("#D64545") },
                        new SwipeItem { Text = "Arquivar", BackgroundColor = Color.FromArgb("#2F80ED") },
                    },
                    LeftItems = new SwipeItems { new SwipeItem { Text = "Favoritar", BackgroundColor = Color.FromArgb("#E59A12") } },
                    Content = new Border
                    {
                        Padding = 16,
                        BackgroundColor = Color.FromArgb("#22512BD4"),
                        Content = new Label { Text = "⇆ Arraste este cartão (toque/trackpad) ou use os botões abaixo" },
                    },
                };
                var open = new Button { Text = "Open(Right)" };
                open.Clicked += (_, _) => swipe.Open(OpenSwipeItem.RightItems);
                var close = new Button { Text = "Close()" };
                close.Clicked += (_, _) => swipe.Close();
                var stack = new VerticalStackLayout { Spacing = 8, Children = { swipe, new HorizontalStackLayout { Spacing = 8, Children = { open, close } } } };
                return new DemoInstance(stack, swipe);
            })
            .Prop("LeftItems / RightItems / TopItems / BottomItems", "Coleções de SwipeItem/SwipeItemView por direção.")
            .Prop("SwipeItems.Mode", "Reveal (mostra os itens) ou Execute (executa o primeiro).")
            .Prop("SwipeItems.SwipeBehaviorOnInvoked", "Auto, Close, RemainOpen após invocar um item.")
            .Prop("Threshold", "Distância para considerar o gesto completo.")
            .Event("SwipeStarted / SwipeChanging / SwipeEnded", "Ciclo do gesto.")
            .Nuance("Métodos Open(OpenSwipeItem) e Close() permitem controlar por código — essencial no desktop, onde arrastar com mouse é pouco intuitivo.")
            .Desktop("No desktop, considere oferecer as mesmas ações num menu de contexto (FlyoutBase.ContextFlyout, botão direito).")
            .Sample("""
                <SwipeView>
                    <SwipeView.RightItems>
                        <SwipeItems Mode="Reveal">
                            <SwipeItem Text="Excluir" BackgroundColor="Red"
                                       Command="{Binding ExcluirCommand}" />
                        </SwipeItems>
                    </SwipeView.RightItems>
                    <Label Text="Item da lista" Padding="16" />
                </SwipeView>
                """)
            .Docs("user-interface/controls/swipeview");

        C(list, "MenuItem / SwipeItem", cat, "📌", "Itens de comando reutilizados em menus, swipe e toolbar.")
            .WithId("MenuItem")
            .About("MenuItem é a base de SwipeItem, ToolbarItem e MenuFlyoutItem. Tem Text, IconImageSource, Command, CommandParameter, IsDestructive e o evento Clicked.")
            .Demo(() =>
            {
                var item = new SwipeItem { Text = "Ação", BackgroundColor = Color.FromArgb("#512BD4") };
                var swipe = new SwipeView
                {
                    WidthRequest = 320,
                    RightItems = [item],
                    Content = new Border { Padding = 14, Content = new Label { Text = "Abra para ver o SwipeItem (botão abaixo)" } },
                };
                var open = new Button { Text = "Abrir" };
                open.Clicked += (_, _) => swipe.Open(OpenSwipeItem.RightItems);
                return new DemoInstance(new VerticalStackLayout { Spacing = 8, Children = { swipe, open } }, item);
            })
            .Prop("Text", "Rótulo.")
            .Prop("IconImageSource", "Ícone.")
            .Prop("Command / CommandParameter", "Ação.")
            .Prop("IsDestructive", "Indica ação destrutiva (estilo especial em algumas plataformas).")
            .Event("Clicked", "Quando invocado.")
            .Sample("""<SwipeItem Text="Excluir" IconImageSource="lixeira.png" IsDestructive="True" Command="{Binding ExcluirCommand}" />""")
            .Docs("user-interface/menuitem");
    }
}
