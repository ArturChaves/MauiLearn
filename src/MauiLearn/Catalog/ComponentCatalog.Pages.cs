using Microsoft.Maui.Controls.Shapes;

namespace MauiLearn.Catalog;

public static partial class ComponentCatalog
{
    private static void AddPages(List<ComponentInfo> list)
    {
        const ComponentCategory cat = ComponentCategory.Pages;

        C(list, "ContentPage", cat, "📄", "A página mais comum: exibe uma única View (Content).")
            .About("Toda tela típica é uma ContentPage. Tem Title, ToolbarItems, MenuBarItems (desktop), Padding, BackgroundImageSource, eventos de ciclo de vida " +
                   "(Appearing, Disappearing, NavigatedTo, NavigatingFrom, NavigatedFrom) e métodos de diálogo (DisplayAlertAsync, DisplayActionSheetAsync, DisplayPromptAsync).")
            .PageDemo(() => SamplePage("Sou uma ContentPage", "#512BD4"))
            .Prop("Content", "A View exibida.")
            .Prop("Title", "Título (mostrado na barra de navegação/janela).")
            .Prop("Padding", "Espaço interno.")
            .Prop("ToolbarItems", "Botões na barra de navegação.")
            .Prop("MenuBarItems", "Menu da janela no desktop.")
            .Prop("HideSoftInputOnTapped", "Esconde o teclado virtual ao tocar fora (mobile).")
            .Prop("SafeAreaEdges", "Quais bordas respeitam a área segura (novo no .NET MAUI 10).")
            .Event("Appearing / Disappearing", "A página ficou visível / deixou de estar.")
            .Event("NavigatedTo / NavigatingFrom / NavigatedFrom", "Eventos de navegação com informações de origem/destino.")
            .Nuance("DisplayAlert, DisplayActionSheet e IsBusy estão obsoletos no .NET MAUI 10+: use DisplayAlertAsync, DisplayActionSheetAsync e um ActivityIndicator próprio.")
            .Nuance("Appearing pode disparar mais de uma vez (ex.: ao voltar de outra página). Não coloque inicializações únicas ali sem controle.")
            .Sample("""
                <ContentPage x:Class="MeuApp.DetalhesPage" Title="Detalhes">
                    <ContentPage.ToolbarItems>
                        <ToolbarItem Text="Salvar" Command="{Binding SalvarCommand}" />
                    </ContentPage.ToolbarItems>
                    <VerticalStackLayout Padding="20" />
                </ContentPage>
                """)
            .Code("""
                bool ok = await DisplayAlertAsync("Excluir", "Tem certeza?", "Sim", "Não");
                string? acao = await DisplayActionSheetAsync("Compartilhar", "Cancelar", null, "E-mail", "Link");
                string? nome = await DisplayPromptAsync("Nome", "Como você se chama?");
                """)
            .Docs("user-interface/pages/contentpage")
            .Lesson("navegacao");

        C(list, "NavigationPage", cat, "🧭", "Navegação em pilha (push/pop) com barra de título e botão voltar.")
            .About("Gerencia uma pilha de páginas: Navigation.PushAsync(page) empilha e PopAsync() volta. Em apps com Shell, a pilha é gerenciada pelo Shell — NavigationPage é usada em apps sem Shell.")
            .PageDemo(() =>
            {
                ContentPage MakePage(int n)
                {
                    var push = new Button { Text = $"PushAsync(Página {n + 1})" };
                    var pop = new Button { Text = "PopAsync()", Style = (Style)Application.Current!.Resources["SecondaryButton"] };
                    var page = new ContentPage
                    {
                        Title = $"Página {n}",
                        Content = new VerticalStackLayout
                        {
                            Padding = 30,
                            Spacing = 12,
                            Children = { new Label { Text = $"Página {n} da pilha", FontSize = 24 }, push, pop },
                        },
                    };
                    push.Clicked += async (_, _) => await page.Navigation.PushAsync(MakePage(n + 1));
                    pop.Clicked += async (_, _) => { if (page.Navigation.NavigationStack.Count > 1) await page.Navigation.PopAsync(); };
                    return page;
                }
                return new NavigationPage(MakePage(1)) { BarBackgroundColor = Color.FromArgb("#512BD4"), BarTextColor = Colors.White };
            })
            .Prop("BarBackgroundColor / BarBackground", "Fundo da barra.")
            .Prop("BarTextColor", "Cor do título e botões.")
            .Prop("NavigationPage.HasNavigationBar (anexada)", "Esconde a barra em uma página específica.")
            .Prop("NavigationPage.HasBackButton (anexada)", "Esconde o botão voltar.")
            .Prop("NavigationPage.TitleView (anexada)", "View customizada no lugar do título.")
            .Event("Pushed / Popped / PoppedToRoot", "Mudanças na pilha.")
            .Nuance("Navigation.NavigationStack mostra a pilha atual; ModalStack mostra as modais.")
            .Nuance("PushModalAsync abre uma página sobre tudo, sem barra — o usuário precisa de um botão para fechar (PopModalAsync).")
            .Sample("""<!-- Em App.CreateWindow: --> new Window(new NavigationPage(new MainPage()));""")
            .Code("""
                await Navigation.PushAsync(new DetalhesPage(item));
                await Navigation.PopAsync();
                await Navigation.PushModalAsync(new LoginPage());
                """)
            .Docs("user-interface/pages/navigationpage")
            .Lesson("navegacao");

        C(list, "TabbedPage", cat, "📑", "Páginas organizadas em abas.")
            .About("Children são as páginas de cada aba. No Windows as abas ficam no topo; no Mac Catalyst, numa barra de abas.")
            .PageDemo(() =>
            {
                var tabs = new TabbedPage { Title = "TabbedPage", BarBackgroundColor = Color.FromArgb("#512BD4"), BarTextColor = Colors.White, SelectedTabColor = Colors.White, UnselectedTabColor = Color.FromArgb("#AC99EA") };
                tabs.Children.Add(SamplePage("Aba 1 — Início", "#512BD4"));
                tabs.Children.Add(SamplePage("Aba 2 — Busca", "#D600AA"));
                var third = SamplePage("Aba 3 — Perfil", "#2E9D5B");
                TabbedPage.SetBadgeText(third, "3");
                tabs.Children.Add(third);
                return tabs;
            })
            .Prop("Children", "As páginas (abas).")
            .Prop("CurrentPage", "A aba selecionada.")
            .Prop("BarBackgroundColor / BarTextColor", "Cores da barra.")
            .Prop("SelectedTabColor / UnselectedTabColor", "Cores das abas.")
            .Prop("TabbedPage.BadgeText / BadgeColor (anexadas)", "Contadores nas abas (novo no .NET MAUI 11).")
            .Event("CurrentPageChanged", "Troca de aba.")
            .Nuance("Com Shell, prefira TabBar/Tab — TabbedPage é para apps sem Shell.")
            .Sample("""
                <TabbedPage>
                    <local:InicioPage Title="Início" IconImageSource="home.png" />
                    <local:PerfilPage Title="Perfil" TabbedPage.BadgeText="2" />
                </TabbedPage>
                """)
            .Docs("user-interface/pages/tabbedpage");

        C(list, "FlyoutPage", cat, "🍔", "Menu lateral (Flyout) + página de detalhe.")
            .About("Duas páginas: Flyout (menu) e Detail (conteúdo). No desktop, FlyoutLayoutBehavior=Split deixa o menu sempre visível.")
            .PageDemo(() =>
            {
                var detail = new NavigationPage(SamplePage("Detalhe", "#512BD4"));
                var menu = new VerticalStackLayout { Padding = 20, Spacing = 8 };
                var flyout = new FlyoutPage { Title = "FlyoutPage", FlyoutLayoutBehavior = FlyoutLayoutBehavior.Split, Detail = detail };
                foreach (var (name, color) in new[] { ("Início", "#512BD4"), ("Mensagens", "#D600AA"), ("Ajustes", "#2E9D5B") })
                {
                    var b = new Button { Text = name };
                    b.Clicked += (_, _) => flyout.Detail = new NavigationPage(SamplePage(name, color));
                    menu.Add(b);
                }
                flyout.Flyout = new ContentPage { Title = "Menu", Content = menu, BackgroundColor = Color.FromArgb("#F0ECFF") };
                return flyout;
            })
            .Prop("Flyout", "Página do menu (precisa de Title).")
            .Prop("Detail", "Página de conteúdo (normalmente uma NavigationPage).")
            .Prop("IsPresented", "Menu aberto/fechado.")
            .Prop("FlyoutLayoutBehavior", "Default, Popover, Split, SplitOnLandscape, SplitOnPortrait.")
            .Event("IsPresentedChanged", "Abriu/fechou o menu.")
            .Nuance("A página Flyout PRECISA de Title definido, senão ocorre exceção.")
            .Sample("""
                <FlyoutPage FlyoutLayoutBehavior="Split">
                    <FlyoutPage.Flyout><local:MenuPage Title="Menu" /></FlyoutPage.Flyout>
                    <FlyoutPage.Detail><NavigationPage><x:Arguments><local:InicioPage /></x:Arguments></NavigationPage></FlyoutPage.Detail>
                </FlyoutPage>
                """)
            .Docs("user-interface/pages/flyoutpage");

        C(list, "Shell", cat, "🐚", "Estrutura de navegação completa: flyout, abas, rotas URI e busca.")
            .About("Este app usa Shell (veja AppShell.xaml). Hierarquia: Shell → FlyoutItem/TabBar → Tab → ShellContent. Navegação por rota: await Shell.Current.GoToAsync(\"component?id=Button\").")
            .PageDemo(() =>
            {
                var shell = new Shell { FlyoutBehavior = FlyoutBehavior.Flyout, Title = "Shell de exemplo" };
                var tabs = new TabBar();
                foreach (var (name, color) in new[] { ("Início", "#512BD4"), ("Explorar", "#D600AA"), ("Conta", "#2E9D5B") })
                {
                    var content = new ShellContent { Title = name, Content = SamplePage(name, color) };
                    if (name == "Explorar") content.BadgeText = "novo";
                    tabs.Items.Add(content);
                }
                shell.Items.Add(tabs);
                return shell;
            })
            .Prop("FlyoutBehavior", "Disabled, Flyout ou Locked (menu fixo — ótimo no desktop).")
            .Prop("FlyoutHeader / FlyoutFooter", "Conteúdo no topo/rodapé do menu.")
            .Prop("FlyoutWidth / FlyoutBackgroundColor", "Aparência do menu.")
            .Prop("CurrentItem / CurrentPage / CurrentState", "Onde o usuário está.")
            .Prop("Shell.NavBarIsVisible / TabBarIsVisible (anexadas)", "Mostram/escondem barras por página.")
            .Prop("Shell.TitleView / SearchHandler (anexadas)", "Título customizado e busca integrada.")
            .Prop("BaseShellItem.BadgeText / BadgeColor", "Contadores em itens do Shell (novo no .NET MAUI 11).")
            .Event("Navigating / Navigated", "Antes/depois de navegar (Navigating pode cancelar: e.Cancel()).")
            .Nuance("Rotas registradas com Routing.RegisterRoute(\"detalhe\", typeof(DetalhePage)) podem ser navegadas relativas (\"detalhe\") ou absolutas (\"//home/detalhe\").")
            .Nuance("Parâmetros: GoToAsync(\"detalhe\", new Dictionary<string, object> { [\"Item\"] = item }) e receba com IQueryAttributable.")
            .Nuance("\"..\" volta uma página: await Shell.Current.GoToAsync(\"..\").")
            .Sample("""
                <Shell FlyoutBehavior="Locked">
                    <FlyoutItem Title="Início" Route="home">
                        <ShellContent ContentTemplate="{DataTemplate local:HomePage}" />
                    </FlyoutItem>
                    <TabBar Route="abas">
                        <ShellContent Title="A" ContentTemplate="{DataTemplate local:APage}" />
                        <ShellContent Title="B" ContentTemplate="{DataTemplate local:BPage}" />
                    </TabBar>
                </Shell>
                """)
            .Docs("fundamentals/shell/")
            .Lesson("navegacao");

        C(list, "Window", cat, "🪟", "A janela do app: título, tamanho, posição e ciclo de vida.")
            .About("Application.CreateWindow retorna a janela principal. No desktop você controla X, Y, Width, Height, mínimos/máximos, Title e pode abrir várias com Application.Current.OpenWindow(window).")
            .PageDemo(() => SamplePage("Esta página está numa nova Window!", "#2F80ED"))
            .Prop("Title", "Título da janela.")
            .Prop("Width / Height / X / Y", "Tamanho e posição (desktop).")
            .Prop("MinimumWidth / MinimumHeight / MaximumWidth / MaximumHeight", "Limites de redimensionamento.")
            .Prop("Page", "A página raiz da janela.")
            .Prop("TitleBar", "Barra de título customizada (Windows).")
            .Event("Created / Activated / Deactivated / Stopped / Resumed / Destroying", "Ciclo de vida.")
            .Event("SizeChanged", "Janela redimensionada.")
            .Nuance("Use 'Abrir em nova janela' acima — o inspetor passa a editar a PÁGINA aberta. Para editar a própria Window, veja a aula de Desktop.")
            .Nuance("No Mac Catalyst, múltiplas janelas exigem UIApplicationSupportsMultipleScenes no Info.plist e um SceneDelegate (já configurados aqui).")
            .Code("""
                protected override Window CreateWindow(IActivationState? state) =>
                    new Window(new AppShell())
                    {
                        Title = "Meu App",
                        Width = 1200, Height = 800,
                        MinimumWidth = 800, MinimumHeight = 500,
                    };

                // Abrir outra janela:
                Application.Current?.OpenWindow(new Window(new DetalhesPage()));
                """)
            .Docs("fundamentals/windows")
            .Lesson("desktop");
    }

    internal static ContentPage SamplePage(string title, string color)
    {
        var close = new Button { Text = "Fechar (se for modal/janela)" };
        var page = new ContentPage
        {
            Title = title,
            Content = new Grid
            {
                Background = new LinearGradientBrush([new GradientStop(Color.FromArgb(color), 0), new GradientStop(Color.FromArgb(color).WithLuminosity(0.2f), 1)], new Point(0, 0), new Point(1, 1)),
                Children =
                {
                    new VerticalStackLayout
                    {
                        Spacing = 16,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        Children =
                        {
                            new Label { Text = title, FontSize = 28, TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Center },
                            new Label { Text = "Edite as propriedades desta página no inspetor do MauiLearn.", TextColor = Colors.White, Opacity = 0.8, HorizontalTextAlignment = TextAlignment.Center },
                            close,
                        },
                    },
                },
            },
        };
        close.Clicked += async (_, _) =>
        {
            if (page.Navigation.ModalStack.Count > 0)
                await page.Navigation.PopModalAsync();
            else if (page.Window is { } window && Application.Current?.Windows.Count > 1)
                Application.Current.CloseWindow(window);
        };
        return page;
    }
}
