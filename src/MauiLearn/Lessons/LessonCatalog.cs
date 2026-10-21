namespace MauiLearn.Lessons;

/// <summary>A trilha de aulas, em ordem.</summary>
public static class LessonCatalog
{
    private static List<LessonInfo>? _all;

    public static IReadOnlyList<LessonInfo> All => _all ??= Build();

    public static LessonInfo? Find(string? id) => All.FirstOrDefault(l => l.Id == id);

    public static int IndexOf(LessonInfo lesson) => All.ToList().IndexOf(lesson);

    private static List<LessonInfo> Build()
    {
        var n = 1;
        LessonInfo L(string id, string icon, string title, string summary, LessonLevel level, int minutes, Func<View> create) =>
            new(id, n++, icon, title, summary, level, minutes, create);

        return
        [
            L("intro", "👋", "O que é o .NET MAUI", "Arquitetura, handlers, plataformas e por que um único código roda no Windows e no macOS.", LessonLevel.Basico, 10, () => new L01Intro())
                .Q("O que é um Handler no .NET MAUI?", 1, "O Handler traduz o controle multiplataforma para o controle nativo e mapeia suas propriedades.",
                    "Um tipo de evento de clique", "A ponte entre o controle MAUI e o controle nativo da plataforma", "Um arquivo de configuração do projeto")
                .Q("Qual tecnologia de UI nativa o MAUI usa no Windows?", 2, "No Windows, o MAUI usa WinUI 3 (Windows App SDK).",
                    "WPF", "Windows Forms", "WinUI 3")
                .Q("No macOS, o .NET MAUI roda como…", 0, "O MAUI usa Mac Catalyst: UIKit (iOS) adaptado para o Mac.",
                    "Mac Catalyst (UIKit no Mac)", "AppKit puro", "Electron")
                .Related("Button", "Label", "Entry"),

            L("projeto", "🗂️", "Anatomia de um projeto MAUI", "Single project, MauiProgram, App, Window, Shell, Resources e a pasta Platforms.", LessonLevel.Basico, 12, () => new L02Projeto())
                .Q("Onde você registra serviços para injeção de dependência?", 0, "MauiProgram.CreateMauiApp configura o MauiAppBuilder, incluindo builder.Services.",
                    "MauiProgram.cs (builder.Services)", "App.xaml", "Platforms/Windows/App.xaml.cs")
                .Q("Qual é a forma recomendada de definir a página inicial desde o .NET 9?", 1, "Application.MainPage está obsoleto; sobrescreva CreateWindow e retorne new Window(page).",
                    "Application.MainPage = new MainPage()", "Sobrescrever CreateWindow e retornar new Window(...)", "Definir StartupPage no .csproj")
                .Q("Um arquivo .svg em Resources/Images é referenciado como…", 2, "O Resizetizer converte SVG em PNG no build; use a extensão .png.",
                    "imagem.svg", "Resources/Images/imagem.svg", "imagem.png"),

            L("hotreload", "🔥", "Hot Reload e fluxo de trabalho", "Como editar XAML e C# com o app rodando — base para os desafios.", LessonLevel.Basico, 8, () => new L03HotReload())
                .Q("O XAML Hot Reload exige que o app esteja rodando em…", 0, "Hot Reload funciona com o depurador anexado em configuração Debug.",
                    "Debug, com depurador anexado", "Release", "Qualquer configuração, sem depurador")
                .Q("Qual mudança normalmente NÃO é aplicada pelo Hot Reload?", 2, "Mudanças estruturais como adicionar novos tipos/assinaturas ou alterar o .csproj exigem reiniciar.",
                    "Trocar a cor de um Label no XAML", "Mudar o corpo de um método C#", "Alterar o .csproj ou adicionar um pacote NuGet"),

            L("xaml", "🧾", "XAML essencial", "Elementos, atributos, property elements, content property, markup extensions, x:Name e type converters.", LessonLevel.Basico, 18, () => new L04Xaml())
                .Q("Em <Label Text=\"Oi\" />, o atributo Text é…", 0, "Atributos XML mapeiam para propriedades do objeto criado.",
                    "Uma propriedade do objeto Label", "Um evento", "Um recurso")
                .Q("O que faz Margin=\"10,5\" virar um Thickness?", 1, "TypeConverters convertem strings do XAML em objetos (ThicknessTypeConverter).",
                    "Reflexão automática do C#", "Um TypeConverter", "O Handler da plataforma")
                .Q("Qual markup extension referencia outro elemento da página por nome?", 2, "{x:Reference nome} retorna o elemento com aquele x:Name.",
                    "{StaticResource}", "{x:Static}", "{x:Reference}")
                .Related("Label", "Span"),

            L("layouts", "📐", "Layouts: como o MAUI posiciona as coisas", "Medida e arranjo, Margin × Padding, HorizontalOptions/VerticalOptions e os principais layouts.", LessonLevel.Basico, 20, () => new L05Layouts())
                .Q("Qual a diferença entre Margin e Padding?", 1, "Margin é espaço FORA do elemento; Padding é espaço DENTRO (entre a borda e o conteúdo).",
                    "São sinônimos", "Margin é externo; Padding é interno", "Padding é externo; Margin é interno")
                .Q("Num VerticalStackLayout, qual propriedade do filho é ignorada?", 0, "A pilha vertical dá a cada filho exatamente a altura pedida; VerticalOptions não tem efeito.",
                    "VerticalOptions", "HorizontalOptions", "Margin")
                .Q("Qual layout quebra linha automaticamente quando falta espaço?", 2, "FlexLayout com Wrap=\"Wrap\" quebra linha.",
                    "HorizontalStackLayout", "Grid", "FlexLayout")
                .Related("VerticalStackLayout", "HorizontalStackLayout", "FlexLayout", "AbsoluteLayout", "ScrollView"),

            L("grid", "▦", "Grid a fundo", "Linhas e colunas Auto, * e absolutas, spans, sobreposição e responsividade.", LessonLevel.Basico, 15, () => new L06Grid())
                .Q("Com ColumnDefinitions=\"100,*,2*\" numa largura de 400, quanto mede a última coluna?", 2, "Sobram 300 após os 100 fixos; divididos em 3 partes (1*+2*), a última recebe 200.",
                    "100", "150", "200")
                .Q("Dois filhos na mesma célula do Grid…", 0, "Eles se sobrepõem; o último declarado fica por cima.",
                    "Se sobrepõem", "Geram erro", "São empilhados automaticamente")
                .Related("Grid"),

            L("binding", "🔗", "Data Binding", "BindingContext, modos, x:Reference, StringFormat, converters, MultiBinding e compiled bindings (x:DataType).", LessonLevel.Intermediario, 22, () => new L07Binding())
                .Q("Qual o modo padrão do binding em Entry.Text?", 1, "Propriedades editáveis pelo usuário (Entry.Text, Slider.Value, Switch.IsToggled...) usam TwoWay por padrão.",
                    "OneWay", "TwoWay", "OneTime")
                .Q("O que x:DataType faz?", 0, "Ativa compiled bindings: o caminho é resolvido em tempo de compilação, mais rápido e com problemas apontados no build.",
                    "Ativa compiled bindings", "Define o tipo do controle", "Converte valores automaticamente")
                .Q("Para transformar bool em cor num binding, você usa…", 2, "Um IValueConverter transforma valores entre fonte e destino.",
                    "StringFormat", "FallbackValue", "Um IValueConverter"),

            L("mvvm", "🏛️", "MVVM: ViewModels, INotifyPropertyChanged e Commands", "Separe UI e lógica. Construa um ViewModel do zero e entenda ICommand/CanExecute.", LessonLevel.Intermediario, 25, () => new L08Mvvm())
                .Q("Qual interface faz a UI atualizar quando uma propriedade do ViewModel muda?", 0, "INotifyPropertyChanged dispara PropertyChanged; o binding escuta e atualiza a UI.",
                    "INotifyPropertyChanged", "ICommand", "IQueryAttributable")
                .Q("O que acontece com um Button cujo Command.CanExecute retorna false?", 1, "O botão fica desabilitado automaticamente; chame CanExecuteChanged para reavaliar.",
                    "Nada", "Fica desabilitado", "Lança exceção")
                .Q("Para a lista na tela refletir Add/Remove, use…", 2, "ObservableCollection<T> dispara CollectionChanged.",
                    "List<T>", "T[]", "ObservableCollection<T>")
                .Related("Button"),

            L("estilos", "🎨", "Recursos, estilos e temas", "ResourceDictionary, StaticResource × DynamicResource, Style implícito/explícito, BasedOn, StyleClass, AppThemeBinding.", LessonLevel.Intermediario, 20, () => new L09Estilos())
                .Q("Qual a diferença de DynamicResource para StaticResource?", 1, "DynamicResource reage a mudanças do recurso em tempo de execução; StaticResource é resolvido uma vez.",
                    "Nenhuma", "DynamicResource atualiza se o recurso mudar", "StaticResource é mais lento")
                .Q("Um Style sem x:Key é…", 0, "Estilos sem chave são implícitos: aplicam-se a todos os controles do TargetType no escopo.",
                    "Implícito (aplica a todos do TargetType)", "Inválido", "Aplicado só ao primeiro controle"),

            L("navegacao", "🧭", "Navegação com Shell e páginas", "Rotas, GoToAsync, parâmetros (IQueryAttributable), pilha, modais e diálogos.", LessonLevel.Intermediario, 20, () => new L10Navegacao())
                .Q("Como voltar uma página com Shell?", 2, "\"..\" significa voltar um nível na navegação do Shell.",
                    "GoToAsync(\"back\")", "GoToAsync(\"//\")", "GoToAsync(\"..\")")
                .Q("Qual a forma trim-safe de receber parâmetros de navegação?", 0, "IQueryAttributable.ApplyQueryAttributes não depende de reflexão como [QueryProperty].",
                    "Implementar IQueryAttributable", "Usar MessagingCenter", "Variáveis estáticas")
                .Related("ContentPage", "NavigationPage", "Shell", "TabbedPage", "FlyoutPage"),

            L("colecoes", "🗂️", "Coleções: CollectionView e companhia", "Templates, seleção, agrupamento, EmptyView, layouts em grade, BindableLayout e CarouselView.", LessonLevel.Intermediario, 25, () => new L11Colecoes())
                .Q("Por que não colocar CollectionView dentro de um VerticalStackLayout?", 1, "A pilha oferece altura infinita: o CollectionView cria todos os itens e perde a virtualização/rolagem.",
                    "Não compila", "Perde virtualização e rolagem", "Fica invisível")
                .Q("Qual controle substitui o ListView (obsoleto)?", 0, "CollectionView é o substituto moderno e mais flexível.",
                    "CollectionView", "TableView", "StackLayout")
                .Related("CollectionView", "CarouselView", "IndicatorView", "BindableLayout", "ListView"),

            L("triggers", "⚡", "Triggers, Behaviors e VisualStateManager", "Mude a aparência por estado sem code-behind e encapsule comportamento reutilizável.", LessonLevel.Intermediario, 20, () => new L12Triggers())
                .Q("Qual VisualState é disparado quando o mouse passa sobre um controle?", 2, "PointerOver faz parte do grupo CommonStates.",
                    "Focused", "Selected", "PointerOver")
                .Q("Um DataTrigger reage a…", 0, "DataTrigger observa um Binding (qualquer propriedade de qualquer objeto).",
                    "Um valor vindo de um Binding", "Apenas eventos", "Somente propriedades do próprio controle"),

            L("gestos", "👆", "Gestos e entrada no desktop", "Tap (inclusive botão direito), ponteiro/hover, pan, long press (novo), drag & drop e teclado.", LessonLevel.Intermediario, 18, () => new L13Gestos())
                .Q("Como detectar clique com o botão direito?", 1, "TapGestureRecognizer com Buttons=\"Secondary\".",
                    "PointerGestureRecognizer.RightClick", "TapGestureRecognizer Buttons=\"Secondary\"", "Não é possível")
                .Q("Qual reconhecedor é novo no .NET MAUI 11?", 2, "LongPressGestureRecognizer chegou nativamente no .NET MAUI 11.",
                    "SwipeGestureRecognizer", "PinchGestureRecognizer", "LongPressGestureRecognizer")
                .Related("TapGestureRecognizer", "PointerGestureRecognizer", "LongPressGestureRecognizer", "PanGestureRecognizer", "DragDrop"),

            L("animacoes", "🎬", "Animações", "Métodos *ToAsync, Easing, composição, Animation com filhos e cancelamento.", LessonLevel.Intermediario, 15, () => new L14Animacoes())
                .Q("No .NET MAUI 10+, qual é o nome recomendado do método de fade?", 0, "Os métodos de animação ganharam o sufixo Async (FadeTo está obsoleto).",
                    "FadeToAsync", "FadeTo", "AnimateFade")
                .Q("Para rodar duas animações ao mesmo tempo…", 1, "Inicie as duas e aguarde com Task.WhenAll.",
                    "await uma depois a outra", "Task.WhenAll(a, b)", "Não é possível"),

            L("graficos", "🖌️", "Formas, pincéis, sombras e desenho", "Shapes, Brushes, Shadow, Clip e GraphicsView com Microsoft.Maui.Graphics.", LessonLevel.Intermediario, 18, () => new L15Graficos())
                .Q("Para desenhar livremente com código, use…", 2, "GraphicsView + IDrawable fornece um ICanvas para desenho imediato.",
                    "Path", "Border", "GraphicsView com IDrawable")
                .Related("Border", "Path", "LinearGradientBrush", "Shadow", "GraphicsView"),

            L("desktop", "🖥️", "Recursos de desktop", "MenuBar, atalhos, menu de contexto, tooltips, janelas múltiplas, tamanho/posição e TitleBar.", LessonLevel.Intermediario, 20, () => new L16Desktop())
                .Q("Onde ficam os itens da barra de menu?", 0, "Page.MenuBarItems — o menu pertence à página atual.",
                    "Page.MenuBarItems", "Application.Menu", "Window.Menu")
                .Q("Como abrir uma segunda janela?", 1, "Application.Current.OpenWindow(new Window(page)).",
                    "new Window().Show()", "Application.Current.OpenWindow(new Window(page))", "Navigation.PushWindowAsync")
                .Related("MenuBar", "ContextFlyout", "ToolTip", "KeyboardAccelerator", "Window", "TitleBar"),

            L("handlers", "🔧", "Handlers e código específico de plataforma", "Mapper, AppendToMapping, #if WINDOWS/MACCATALYST, partial classes e OnPlatform.", LessonLevel.Avancado, 20, () => new L17Handlers())
                .Q("Para alterar a view nativa de TODOS os Entry, você usa…", 0, "EntryHandler.Mapper.AppendToMapping(chave, (handler, view) => ...).",
                    "EntryHandler.Mapper.AppendToMapping", "Um Style", "Um Behavior")
                .Q("Qual a propriedade do handler que expõe o controle nativo?", 2, "handler.PlatformView é o controle nativo (ex.: TextBox no Windows).",
                    "handler.NativeControl", "handler.Control", "handler.PlatformView"),

            L("ciclo", "♻️", "Ciclo de vida", "Eventos de Window, Application e Page; Loaded/Unloaded; e a ordem em que acontecem.", LessonLevel.Avancado, 12, () => new L18Ciclo())
                .Q("Qual evento da Window indica que o app foi para segundo plano/minimizado?", 1, "Stopped indica que a janela deixou de estar visível; Resumed quando volta.",
                    "Destroying", "Stopped", "Created"),

            L("essentials", "🧰", "APIs de plataforma (Essentials)", "Preferences, SecureStorage, Clipboard, Launcher, FileSystem, FilePicker, DeviceInfo e Connectivity.", LessonLevel.Intermediario, 15, () => new L19Essentials())
                .Q("Onde guardar um token de acesso com segurança?", 2, "SecureStorage usa o Keychain (macOS) e DPAPI/Credential Locker (Windows).",
                    "Preferences", "Um arquivo .txt", "SecureStorage"),

            L("controles", "🧱", "Criando seus próprios controles", "BindableProperty, ContentView, ControlTemplate, ContentPresenter e TemplatedParent.", LessonLevel.Avancado, 22, () => new L20Controles())
                .Q("Para que uma propriedade aceite binding, ela deve ser…", 0, "BindableProperty registrada com BindableProperty.Create e exposta por GetValue/SetValue.",
                    "Uma BindableProperty", "Uma propriedade auto-implementada comum", "Um campo público")
                .Q("TemplateBinding está obsoleto. Use…", 1, "Binding com Source={RelativeSource TemplatedParent}.",
                    "x:Reference", "{Binding X, Source={RelativeSource TemplatedParent}}", "StaticResource")
                .Related("ContentView", "RadioButton"),

            L("novidades", "✨", "Novidades do .NET MAUI 10 e 11", "XAML SourceGen, xmlns global, C# Expressions, LongPress, badges, Async APIs e obsoletos.", LessonLevel.Intermediario, 15, () => new L21Novidades())
                .Q("Qual é o inflador de XAML padrão no .NET MAUI 11?", 2, "O SourceGen gera C# a partir do XAML; XamlC e Runtime estão obsoletos.",
                    "Runtime", "XamlC", "SourceGen")
                .Q("XAML C# Expressions exigem…", 0, "Por serem experimentais, exigem <EnablePreviewFeatures>true</EnablePreviewFeatures>.",
                    "EnablePreviewFeatures=true", "Nada — são padrão", "Um pacote NuGet do Toolkit")
                .Related("LongPressGestureRecognizer", "TabbedPage", "ToolbarItem", "Frame", "ListView"),
        ];
    }
}
