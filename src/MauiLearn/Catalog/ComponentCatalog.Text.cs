namespace MauiLearn.Catalog;

public static partial class ComponentCatalog
{
    private static void AddText(List<ComponentInfo> list)
    {
        const ComponentCategory cat = ComponentCategory.Text;

        C(list, "Label", cat, "🔤", "Exibe texto somente leitura — simples, formatado ou HTML.")
            .About("O controle de texto do MAUI. Suporta fonte, cor, alinhamento, quebra de linha, truncamento, número máximo de linhas, espaçamento, decorações, " +
                   "texto formatado (FormattedText com Spans) e um subconjunto de HTML (TextType=\"Html\").")
            .Demo(() => new Label
            {
                Text = "O rato roeu a roupa do rei de Roma. Este texto é longo o suficiente para você testar LineBreakMode, MaxLines e alinhamentos.",
                FontSize = 18,
                WidthRequest = 360,
                HorizontalTextAlignment = TextAlignment.Center,
            })
            .Platform("Microsoft.UI.Xaml.Controls.TextBlock", "UIKit.UILabel (MauiLabel)")
            .Prop("Text", "O texto exibido.")
            .Prop("FontSize / FontFamily / FontAttributes", "Tamanho, família e Bold/Italic.")
            .Prop("TextColor", "Cor do texto.")
            .Prop("HorizontalTextAlignment / VerticalTextAlignment", "Alinha o texto DENTRO da área da Label (diferente de HorizontalOptions, que posiciona a Label no pai).")
            .Prop("LineBreakMode", "NoWrap, WordWrap, CharacterWrap, HeadTruncation, TailTruncation, MiddleTruncation.")
            .Prop("MaxLines", "Limita as linhas exibidas (use com TailTruncation para '...').")
            .Prop("LineHeight", "Multiplicador da altura da linha (1.0 = normal).")
            .Prop("CharacterSpacing", "Espaço extra entre caracteres.")
            .Prop("TextDecorations", "Underline e/ou Strikethrough.")
            .Prop("TextTransform", "None, Default, Lowercase, Uppercase.")
            .Prop("TextType", "Text (padrão) ou Html (subconjunto de tags simples).")
            .Prop("FormattedText", "Texto com vários Spans, cada um com seu estilo e até gestos.")
            .Nuance("HorizontalTextAlignment só tem efeito visível se a Label for mais larga que o texto (ex.: HorizontalOptions=Fill ou WidthRequest).")
            .Nuance("Label não é selecionável/copiável. Para isso use um Editor/Entry com IsReadOnly=True.")
            .Nuance("Com TextType=Html, propriedades como TextColor e FontSize podem ser ignoradas em partes do texto, pois o HTML tem precedência.")
            .Nuance("Spans podem ter TapGestureRecognizer — ótimo para links dentro de um parágrafo.")
            .Sample("""
                <Label FontSize="16" LineBreakMode="WordWrap" MaxLines="3">
                    <Label.FormattedText>
                        <FormattedString>
                            <Span Text="Leia os " />
                            <Span Text="termos de uso" TextColor="Blue" TextDecorations="Underline">
                                <Span.GestureRecognizers>
                                    <TapGestureRecognizer Tapped="OnTermosTapped" />
                                </Span.GestureRecognizers>
                            </Span>
                            <Span Text=" antes de continuar." FontAttributes="Italic" />
                        </FormattedString>
                    </Label.FormattedText>
                </Label>
                """)
            .Docs("user-interface/controls/label")
            .Lesson("xaml");

        C(list, "Span / FormattedString", cat, "🌈", "Trechos de texto com estilos diferentes dentro de uma única Label.")
            .WithId("Span")
            .About("Span não é uma View: é um trecho de texto (GestureElement) com fonte, cor, fundo, decorações e gestos próprios. Agrupe Spans em um FormattedString e atribua a Label.FormattedText.")
            .Demo(() =>
            {
                var span = new Span { Text = "este trecho é um Span", TextColor = Color.FromArgb("#D600AA"), FontAttributes = FontAttributes.Bold, FontSize = 20 };
                var label = new Label
                {
                    FontSize = 16,
                    FormattedText = new FormattedString
                    {
                        Spans =
                        {
                            new Span { Text = "Texto normal, " },
                            span,
                            new Span { Text = " — e voltamos ao normal. ", },
                            new Span { Text = "Sublinhado", TextDecorations = TextDecorations.Underline },
                        },
                    },
                };
                return new DemoInstance(label, span);
            })
            .Prop("Text", "Conteúdo do trecho.")
            .Prop("TextColor / BackgroundColor", "Cores do trecho.")
            .Prop("FontSize / FontAttributes / FontFamily", "Fonte do trecho.")
            .Prop("TextDecorations", "Underline, Strikethrough.")
            .Prop("GestureRecognizers", "Torna um trecho clicável.")
            .Nuance("O inspetor à direita está editando o SPAN rosa (não a Label). Repare que Span herda de GestureElement → Element, não de View.")
            .Sample("""
                <Label>
                    <Label.FormattedText>
                        <FormattedString>
                            <Span Text="Olá, " />
                            <Span Text="MAUI" FontAttributes="Bold" TextColor="#512BD4" />
                        </FormattedString>
                    </Label.FormattedText>
                </Label>
                """)
            .Docs("user-interface/controls/label#use-formatted-text");

        C(list, "Image", cat, "🖼️", "Exibe imagens de arquivo, URL, stream ou fonte (glyph).")
            .About("Source aceita: nome de arquivo (de Resources/Images — SVGs viram PNG no build), URI (com cache), stream ou FontImageSource. Aspect controla o encaixe.")
            .Demo(() => new Image
            {
                Source = "sample_landscape.png",
                HeightRequest = 180,
                WidthRequest = 300,
                Aspect = Aspect.AspectFit,
                BackgroundColor = Color.FromArgb("#22512BD4"),
            })
            .Platform("Microsoft.UI.Xaml.Controls.Image", "UIKit.UIImageView")
            .Prop("Source", "ImageSource: arquivo, URL, stream ou fonte.")
            .Prop("Aspect", "AspectFit (cabe inteira), AspectFill (preenche e corta), Fill (estica), Center.")
            .Prop("IsAnimationPlaying", "Liga/pausa GIFs animados.")
            .Prop("IsLoading (somente leitura)", "Verdadeiro enquanto a imagem carrega.")
            .Nuance("Referencie SVGs pela extensão .png: o Resizetizer converte 'logo.svg' em 'logo.png' em tempo de build.")
            .Nuance("Image sem tamanho explícito usa o tamanho natural da imagem; combine HeightRequest/WidthRequest com Aspect para controlar.")
            .Nuance("Para cantos arredondados, coloque a Image dentro de um Border com StrokeShape — o Border recorta o conteúdo.")
            .Sample("""
                <Image Source="dotnet_bot.png" Aspect="AspectFill" HeightRequest="120" />

                <Image Aspect="AspectFit">
                    <Image.Source>
                        <UriImageSource Uri="https://exemplo.com/foto.jpg"
                                        CacheValidity="10:00:00:00" />
                    </Image.Source>
                </Image>
                """)
            .Docs("user-interface/controls/image");

        C(list, "BoxView", cat, "🟪", "Retângulo de cor sólida — separadores, placeholders e blocos.")
            .About("O controle visual mais simples. Útil para linhas divisórias, blocos de cor e protótipos de layout.")
            .Demo(() => new BoxView { Color = Color.FromArgb("#512BD4"), WidthRequest = 160, HeightRequest = 100, CornerRadius = 16 })
            .Prop("Color", "Cor de preenchimento.")
            .Prop("CornerRadius", "Raio dos cantos (pode ser diferente por canto: \"10,0,10,0\").")
            .Nuance("Sem WidthRequest/HeightRequest, o BoxView tem tamanho padrão 40x40 — mas estica se HorizontalOptions/VerticalOptions forem Fill num pai que permita.")
            .Nuance("Uma linha divisória: <BoxView HeightRequest=\"1\" Color=\"Gray\" />.")
            .Sample("""<BoxView Color="#512BD4" HeightRequest="1" HorizontalOptions="Fill" />""")
            .Docs("user-interface/controls/boxview");

        C(list, "ActivityIndicator", cat, "⏳", "Animação indeterminada de 'carregando'.")
            .About("Mostra que algo está em andamento sem indicar quanto falta. Normalmente ligado a uma propriedade IsBusy/Carregando do ViewModel.")
            .Demo(() => new ActivityIndicator { IsRunning = true, WidthRequest = 60, HeightRequest = 60 })
            .Platform("Microsoft.UI.Xaml.Controls.ProgressRing", "UIKit.UIActivityIndicatorView")
            .Prop("IsRunning", "Liga/desliga a animação.")
            .Prop("Color", "Cor do indicador.")
            .Nuance("IsRunning=False não necessariamente esconde o controle em todas as plataformas — combine com IsVisible se quiser liberar o espaço.")
            .Nuance("Page.IsBusy está obsoleto no .NET MAUI 10+: use um ActivityIndicator próprio ligado ao ViewModel.")
            .Sample("""<ActivityIndicator IsRunning="{Binding Carregando}" IsVisible="{Binding Carregando}" />""")
            .Docs("user-interface/controls/activityindicator");

        C(list, "ProgressBar", cat, "📊", "Barra de progresso determinada (0 a 1).")
            .About("Exibe o andamento de uma tarefa. Progress vai de 0.0 a 1.0. ProgressTo anima suavemente até um valor.")
            .Demo(() => new ProgressBar { Progress = 0.4, WidthRequest = 300 })
            .Platform("Microsoft.UI.Xaml.Controls.ProgressBar", "UIKit.UIProgressView")
            .Prop("Progress", "Valor entre 0 e 1. Valores fora do intervalo são limitados.")
            .Prop("ProgressColor", "Cor da parte preenchida.")
            .Nuance("Use await progressBar.ProgressTo(0.75, 500, Easing.Linear) para animar.")
            .Nuance("Não confunda com Slider: ProgressBar não é interativo.")
            .Sample("""<ProgressBar Progress="{Binding Percentual}" ProgressColor="Orange" />""")
            .Docs("user-interface/controls/progressbar");

        C(list, "WebView", cat, "🌐", "Exibe páginas web ou HTML local dentro do app.")
            .About("Hospeda o motor web da plataforma: WebView2 (Edge/Chromium) no Windows e WKWebView no macOS. Source aceita URL ou HtmlWebViewSource.")
            .Demo(() => new WebView
            {
                Source = new HtmlWebViewSource
                {
                    Html = """
                        <html><body style="font-family:sans-serif;padding:12px">
                        <h2 style="color:#512BD4">HTML dentro do MAUI</h2>
                        <p>Este conteúdo veio de um <b>HtmlWebViewSource</b>.</p>
                        <button onclick="this.innerText='Clicado!'">Clique</button>
                        </body></html>
                        """,
                },
                HeightRequest = 220,
                WidthRequest = 420,
            })
            .Platform("Microsoft.UI.Xaml.Controls.WebView2", "WebKit.WKWebView")
            .Prop("Source", "UrlWebViewSource (\"https://...\") ou HtmlWebViewSource (HTML em string).")
            .Prop("CanGoBack / CanGoForward", "Histórico de navegação.")
            .Event("Navigating", "Antes de navegar — pode cancelar (e.Cancel = true).")
            .Event("Navigated", "Depois de navegar, com o resultado.")
            .Nuance("Execute JavaScript com await webView.EvaluateJavaScriptAsync(\"document.title\").")
            .Nuance("Para arquivos locais, coloque-os em Resources/Raw e use Source=\"demo.html\".")
            .Desktop("No Mac Catalyst, URLs externas exigem o entitlement com.apple.security.network.client (já configurado neste projeto).")
            .Sample("""<WebView Source="https://learn.microsoft.com/dotnet/maui" />""")
            .Docs("user-interface/controls/webview");

        C(list, "HybridWebView", cat, "🧩", "WebView com ponte JavaScript ↔ C# para apps híbridos (React, Vue, etc.).")
            .New("Introduzido no .NET MAUI 9.")
            .About("Hospeda uma aplicação web empacotada (por padrão em Resources/Raw/wwwroot) e permite chamar JavaScript a partir do C# (InvokeJavaScriptAsync) e métodos C# a partir do JavaScript (SetInvokeJavaScriptTarget).")
            .Demo(() => new VerticalStackLayout
            {
                Spacing = 6,
                Children =
                {
                    new Label { Text = "HybridWebView precisa de uma pasta wwwroot com index.html e o script HybridWebView.js.", WidthRequest = 360 },
                    new Label { Text = "Veja o exemplo de código abaixo e a documentação oficial.", Opacity = 0.7 },
                },
            })
            .Prop("HybridRoot", "Pasta (em Resources/Raw) com o app web. Padrão: wwwroot.")
            .Prop("DefaultFile", "Arquivo inicial. Padrão: index.html.")
            .Event("RawMessageReceived", "Mensagens enviadas do JS com window.HybridWebView.SendRawMessage(...).")
            .Nuance("Use InvokeJavaScriptAsync com JsonTypeInfo (System.Text.Json source gen) para ser compatível com trimming/AOT.")
            .Code("""
                // C# → JavaScript
                var soma = await hybridWebView.InvokeJavaScriptAsync<int>(
                    "somar", MeuJsonContext.Default.Int32,
                    [2, 3], [MeuJsonContext.Default.Int32, MeuJsonContext.Default.Int32]);

                // JavaScript → C#
                hybridWebView.SetInvokeJavaScriptTarget(new MinhaPonte());
                // no JS: await window.HybridWebView.InvokeDotNet('Saudar', ['Artur']);
                """)
            .Sample("""<HybridWebView x:Name="hybridWebView" HybridRoot="wwwroot" RawMessageReceived="OnMensagem" />""")
            .Docs("user-interface/controls/hybridwebview");
    }
}
