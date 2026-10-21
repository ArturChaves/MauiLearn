using Microsoft.Maui.Controls.Shapes;

namespace MauiLearn.Catalog;

public static partial class ComponentCatalog
{
    private static void AddDesktop(List<ComponentInfo> list)
    {
        const ComponentCategory cat = ComponentCategory.Desktop;

        C(list, "MenuBar (MenuBarItem)", cat, "🍽️", "Barra de menus da janela (Windows) ou menu global (macOS).")
            .WithId("MenuBar")
            .About("Defina em Page.MenuBarItems: MenuBarItem (menu de topo) → MenuFlyoutItem, MenuFlyoutSubItem e MenuFlyoutSeparator. " +
                   "Este app inteiro usa um MenuBar: olhe o topo da janela (Windows) ou a barra de menus do macOS — veja Controls/AppPage.cs.")
            .Demo(() =>
            {
                var item = new MenuBarItem { Text = "Meu Menu" };
                var info = new VerticalStackLayout
                {
                    Spacing = 6,
                    WidthRequest = 420,
                    Children =
                    {
                        new Label { Text = "Os menus 'Navegar', 'Exibir' e 'Ajuda' desta janela são MenuBarItems.", FontAttributes = FontAttributes.Bold },
                        new Label { Text = "Atalhos: Ctrl/⌘+1..5 navegam, Ctrl/⌘+K busca, Ctrl/⌘+Shift+N abre nova janela." },
                        new Label { Text = "O inspetor está editando um MenuBarItem de exemplo (não anexado).", Opacity = 0.7, FontSize = 12 },
                    },
                };
                return new DemoInstance(info, item);
            })
            .Prop("Page.MenuBarItems", "Coleção de MenuBarItem da página atual.")
            .Prop("MenuBarItem.Text", "Título do menu de topo.")
            .Prop("MenuFlyoutItem.Text / Command / IconImageSource", "Itens clicáveis.")
            .Prop("MenuFlyoutItem.KeyboardAccelerators", "Atalhos de teclado (Modifiers + Key).")
            .Prop("MenuFlyoutSubItem", "Submenu.")
            .Prop("MenuFlyoutSeparator", "Linha separadora.")
            .Nuance("O menu pertence à PÁGINA: ao navegar para uma página sem MenuBarItems, o menu some. Por isso este app usa uma página base (AppPage).")
            .Nuance("No macOS, os menus de sistema (Arquivo, Editar, Janela...) coexistem com os seus.")
            .Sample("""
                <ContentPage.MenuBarItems>
                    <MenuBarItem Text="Arquivo">
                        <MenuFlyoutItem Text="Abrir" Command="{Binding AbrirCommand}">
                            <MenuFlyoutItem.KeyboardAccelerators>
                                <KeyboardAccelerator Modifiers="Ctrl" Key="O" />
                            </MenuFlyoutItem.KeyboardAccelerators>
                        </MenuFlyoutItem>
                        <MenuFlyoutSeparator />
                        <MenuFlyoutSubItem Text="Exportar">
                            <MenuFlyoutItem Text="PDF" />
                            <MenuFlyoutItem Text="CSV" />
                        </MenuFlyoutSubItem>
                    </MenuBarItem>
                </ContentPage.MenuBarItems>
                """)
            .Docs("user-interface/menu-bar")
            .Lesson("desktop");

        C(list, "ContextFlyout (menu de contexto)", cat, "🖱️", "Menu do botão direito em qualquer elemento.")
            .WithId("ContextFlyout")
            .About("Propriedade anexada FlyoutBase.ContextFlyout recebe um MenuFlyout com MenuFlyoutItems. Funciona com botão direito (Windows/macOS) e Control+clique (macOS).")
            .Demo(() =>
            {
                var label = new Label { Text = "Clique com o BOTÃO DIREITO aqui", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
                var target = new Border
                {
                    WidthRequest = 320,
                    HeightRequest = 120,
                    BackgroundColor = Color.FromArgb("#22512BD4"),
                    StrokeShape = new RoundRectangle { CornerRadius = 14 },
                    Content = label,
                };
                var menu = new MenuFlyout();
                foreach (var (text, color) in new[] { ("Roxo", "#512BD4"), ("Rosa", "#D600AA"), ("Verde", "#2E9D5B") })
                {
                    var item = new MenuFlyoutItem { Text = $"Pintar de {text}" };
                    item.Clicked += (_, _) => { target.BackgroundColor = Color.FromArgb(color).WithAlpha(0.4f); label.Text = $"Escolhido: {text}"; };
                    menu.Add(item);
                }
                menu.Add(new MenuFlyoutSeparator());
                var sub = new MenuFlyoutSubItem { Text = "Mais…" };
                var reset = new MenuFlyoutItem { Text = "Restaurar" };
                reset.Clicked += (_, _) => { target.BackgroundColor = Color.FromArgb("#22512BD4"); label.Text = "Clique com o BOTÃO DIREITO aqui"; };
                sub.Add(reset);
                menu.Add(sub);
                FlyoutBase.SetContextFlyout(target, menu);
                return new DemoInstance(target, menu);
            })
            .Prop("FlyoutBase.ContextFlyout (anexada)", "O MenuFlyout exibido.")
            .Prop("MenuFlyout", "Contêiner de MenuFlyoutItem / SubItem / Separator.")
            .Nuance("Dentro de CollectionView, coloque o ContextFlyout no elemento raiz do DataTemplate e use CommandParameter=\"{Binding .}\" para saber qual item foi clicado.")
            .Sample("""
                <Label Text="Clique direito">
                    <FlyoutBase.ContextFlyout>
                        <MenuFlyout>
                            <MenuFlyoutItem Text="Copiar" Command="{Binding CopiarCommand}" />
                            <MenuFlyoutItem Text="Excluir" Command="{Binding ExcluirCommand}" />
                        </MenuFlyout>
                    </FlyoutBase.ContextFlyout>
                </Label>
                """)
            .Docs("user-interface/context-menu")
            .Lesson("desktop");

        C(list, "ToolTipProperties", cat, "💬", "Dica que aparece ao passar o mouse.")
            .WithId("ToolTip")
            .About("Propriedade anexada ToolTipProperties.Text em qualquer View. Indispensável em botões com ícone no desktop.")
            .Demo(() =>
            {
                var btn = new Button { Text = "Passe o mouse em mim", HorizontalOptions = LayoutOptions.Center };
                ToolTipProperties.SetText(btn, "Eu sou um tooltip! Edite ToolTipProperties.Text no inspetor.");
                return btn;
            })
            .Prop("ToolTipProperties.Text (anexada)", "O texto da dica (aceita binding).")
            .Nuance("No inspetor, a propriedade aparece como 'ToolTipProperties.Text' (é uma propriedade anexada, declarada em outra classe).")
            .Sample("""<ImageButton Source="lixeira.png" ToolTipProperties.Text="Excluir item" />""")
            .Docs("user-interface/tooltips")
            .Lesson("desktop");

        C(list, "ToolbarItem", cat, "🔧", "Botões na barra de navegação/título da página.")
            .About("Page.ToolbarItems. Order=Primary aparece na barra; Secondary vai para um menu de 'mais opções'. No .NET MAUI 11, ToolbarItem ganhou BadgeText/BadgeColor.")
            .Demo(() =>
            {
                var item = new ToolbarItem { Text = "Salvar", Order = ToolbarItemOrder.Primary, Priority = 0 };
                var info = new VerticalStackLayout
                {
                    WidthRequest = 400,
                    Spacing = 6,
                    Children =
                    {
                        new Label { Text = "ToolbarItems aparecem na barra de navegação da página (com NavigationPage/Shell).", FontAttributes = FontAttributes.Bold },
                        new Label { Text = "Esta página de detalhe tem um ToolbarItem '⭐ Favoritar' — olhe a barra no topo!" },
                    },
                };
                return new DemoInstance(info, item);
            })
            .Prop("Text / IconImageSource", "Rótulo e ícone.")
            .Prop("Order", "Default, Primary, Secondary.")
            .Prop("Priority", "Ordem entre itens.")
            .Prop("Command / CommandParameter", "Ação.")
            .Prop("BadgeText / BadgeColor / BadgeTextColor", "Contador no item (novo no .NET MAUI 11).")
            .Sample("""
                <ContentPage.ToolbarItems>
                    <ToolbarItem Text="Filtros" IconImageSource="filtro.png" Order="Primary" BadgeText="2" />
                    <ToolbarItem Text="Sobre" Order="Secondary" />
                </ContentPage.ToolbarItems>
                """)
            .Docs("user-interface/toolbaritem");

        C(list, "KeyboardAccelerator", cat, "⌨️", "Atalho de teclado associado a um item de menu.")
            .About("Modifiers (Ctrl, Shift, Alt, Cmd, Windows) + Key. Usado em MenuFlyoutItem.KeyboardAccelerators — tanto no MenuBar quanto em menus de contexto.")
            .Demo(() =>
            {
                var acc = new KeyboardAccelerator { Modifiers = KeyboardAcceleratorModifiers.Ctrl | KeyboardAcceleratorModifiers.Shift, Key = "S" };
                var info = new VerticalStackLayout
                {
                    WidthRequest = 400,
                    Spacing = 6,
                    Children =
                    {
                        new Label { Text = "Teste os atalhos do app:", FontAttributes = FontAttributes.Bold },
                        new Label { Text = "Ctrl/⌘ + 1 → Início · 2 → Aulas · 3 → Catálogo · 4 → Desafios · 5 → Playground" },
                        new Label { Text = "Ctrl/⌘ + K → Buscar componente · Ctrl/⌘ + Shift + N → Nova janela" },
                    },
                };
                return new DemoInstance(info, acc);
            })
            .Prop("Modifiers", "Flags: Ctrl, Shift, Alt, Cmd (macOS), Windows.")
            .Prop("Key", "A tecla: \"S\", \"F5\", \"Delete\"...")
            .Nuance("Use OperatingSystem.IsMacCatalyst() para escolher Cmd no Mac e Ctrl no Windows (veja Controls/AppPage.cs).")
            .Sample("""<KeyboardAccelerator Modifiers="Ctrl,Shift" Key="S" />""")
            .Docs("user-interface/keyboard-accelerators")
            .Lesson("desktop");

        C(list, "TitleBar", cat, "🏷️", "Barra de título customizada da janela (Windows).")
            .New("Introduzido no .NET MAUI 9 (Windows).")
            .About("Window.TitleBar recebe um TitleBar com Icon, Title, Subtitle e áreas LeadingContent, Content (ex.: uma SearchBar) e TrailingContent. Cores via BackgroundColor/ForegroundColor.")
            .Demo(() =>
            {
                var bar = new TitleBar
                {
                    Title = "MauiLearn",
                    Subtitle = "Pré-visualização",
                    BackgroundColor = Color.FromArgb("#512BD4"),
                    ForegroundColor = Colors.White,
                    HeightRequest = 48,
                    Content = new SearchBar { Placeholder = "Buscar…", WidthRequest = 220, VerticalOptions = LayoutOptions.Center },
                };
                var apply = new Button { Text = "Aplicar à janela atual (Windows)" };
                apply.Clicked += (_, _) =>
                {
                    if (apply.Window is { } w)
                        w.TitleBar = w.TitleBar is null ? new TitleBar { Title = "MauiLearn — TitleBar customizada", Subtitle = ".NET MAUI 11", BackgroundColor = Color.FromArgb("#512BD4"), ForegroundColor = Colors.White } : null;
                };
                var stack = new VerticalStackLayout
                {
                    Spacing = 10,
                    WidthRequest = 460,
                    Children =
                    {
                        new Label { Text = "Prévia (dentro da página):", FontSize = 12, Opacity = 0.7 },
                        bar,
                        apply,
                        new Label { Text = "No macOS a barra de título é do sistema; esta API afeta principalmente o Windows.", FontSize = 12, Opacity = 0.7 },
                    },
                };
                return new DemoInstance(stack, bar);
            })
            .Prop("Title / Subtitle / Icon", "Conteúdo padrão.")
            .Prop("LeadingContent / Content / TrailingContent", "Áreas customizáveis.")
            .Prop("ForegroundColor / BackgroundColor", "Cores.")
            .Prop("PassthroughElements", "Elementos que recebem cliques (o resto arrasta a janela).")
            .Nuance("Elementos interativos dentro da TitleBar precisam estar em PassthroughElements para receber cliques em vez de arrastar a janela.")
            .Desktop("Exclusivo do Windows na prática; no Mac Catalyst a barra é gerenciada pelo sistema.")
            .Sample("""
                <!-- Em App.CreateWindow -->
                window.TitleBar = new TitleBar
                {
                    Title = "Meu App",
                    Content = new SearchBar { Placeholder = "Buscar" },
                };
                """)
            .Docs("user-interface/titlebar")
            .Lesson("desktop");

        C(list, "SemanticProperties", cat, "♿", "Acessibilidade: o que leitores de tela anunciam.")
            .About("Propriedades anexadas Description, Hint e HeadingLevel. Narrator (Windows) e VoiceOver (macOS) leem esses valores.")
            .Demo(() =>
            {
                var btn = new ImageButton { Source = "icon_star.png", WidthRequest = 60, HeightRequest = 60 };
                SemanticProperties.SetDescription(btn, "Favoritar");
                SemanticProperties.SetHint(btn, "Adiciona o componente aos favoritos");
                ToolTipProperties.SetText(btn, "Favoritar");
                return btn;
            })
            .Prop("SemanticProperties.Description", "Nome lido pelo leitor de tela.")
            .Prop("SemanticProperties.Hint", "Instrução extra.")
            .Prop("SemanticProperties.HeadingLevel", "Marca títulos (Level1..Level9) para navegação rápida.")
            .Nuance("Ative o Narrator (Ctrl+Win+Enter) ou VoiceOver (⌘+F5) e foque o botão para ouvir.")
            .Nuance("Use SemanticScreenReader.Announce(\"texto\") para anunciar mudanças importantes.")
            .Sample("""<Image Source="grafico.png" SemanticProperties.Description="Gráfico de vendas: alta de 20% em março" />""")
            .Docs("fundamentals/accessibility");
    }
}
