namespace MauiLearn.Catalog;

public static partial class ComponentCatalog
{
    private static void AddInput(List<ComponentInfo> list)
    {
        const ComponentCategory cat = ComponentCategory.Input;

        C(list, "Entry", cat, "✏️", "Campo de texto de uma linha.")
            .About("Herda de InputView (Text, Placeholder, MaxLength, IsReadOnly, Keyboard, IsSpellCheckEnabled...). Acrescenta IsPassword, ReturnType, ClearButtonVisibility e o evento Completed (Enter).")
            .Demo(() => new Entry { Placeholder = "Digite seu nome…", WidthRequest = 300 })
            .Platform("Microsoft.UI.Xaml.Controls.TextBox", "UIKit.UITextField (MauiTextField)")
            .Prop("Text", "O texto digitado (TwoWay por padrão em bindings).")
            .Prop("Placeholder / PlaceholderColor", "Texto de dica exibido quando vazio.")
            .Prop("IsPassword", "Oculta os caracteres.")
            .Prop("MaxLength", "Limite de caracteres.")
            .Prop("IsReadOnly", "Impede edição, mas permite selecionar/copiar.")
            .Prop("Keyboard", "Default, Email, Numeric, Telephone, Url, Chat, Text, Plain — no desktop influencia pouco.")
            .Prop("ClearButtonVisibility", "WhileEditing mostra um 'x' para limpar.")
            .Prop("CursorPosition / SelectionLength", "Controle do cursor e seleção por código.")
            .Prop("ReturnCommand", "Comando executado ao pressionar Enter.")
            .Event("TextChanged", "A cada alteração, com OldTextValue e NewTextValue.")
            .Event("Completed", "Ao pressionar Enter (Return).")
            .Event("Focused / Unfocused", "Ganho/perda de foco (herdados de VisualElement).")
            .Nuance("O binding de Entry.Text é TwoWay por padrão: o ViewModel é atualizado a cada tecla.")
            .Nuance("Keyboard=Numeric NÃO impede letras no desktop. Valide o texto (ou use um Behavior) para aceitar apenas números.")
            .Nuance("Para remover a borda nativa, customize o handler — veja BorderlessEntry e a aula de Handlers.")
            .Sample("""
                <Entry Placeholder="E-mail"
                       Text="{Binding Email}"
                       Keyboard="Email"
                       ClearButtonVisibility="WhileEditing"
                       ReturnCommand="{Binding EntrarCommand}" />
                """)
            .Docs("user-interface/controls/entry");

        C(list, "Editor", cat, "📝", "Campo de texto multilinha.")
            .About("Como o Entry, mas para várias linhas. AutoSize=TextChanges faz o Editor crescer conforme o texto.")
            .Demo(() => new Editor { Placeholder = "Escreva várias linhas…", HeightRequest = 120, WidthRequest = 340 })
            .Platform("Microsoft.UI.Xaml.Controls.TextBox (AcceptsReturn)", "UIKit.UITextView")
            .Prop("Text / Placeholder", "Conteúdo e dica.")
            .Prop("AutoSize", "Disabled (padrão) ou TextChanges (cresce com o conteúdo).")
            .Prop("IsTextPredictionEnabled / IsSpellCheckEnabled", "Correção e predição de texto.")
            .Event("TextChanged", "A cada alteração.")
            .Event("Completed", "Quando o Editor perde o foco (Enter apenas quebra linha).")
            .Nuance("AutoSize=TextChanges só funciona se o Editor NÃO tiver HeightRequest fixo.")
            .Sample("""<Editor Placeholder="Comentários" AutoSize="TextChanges" MaxLength="500" />""")
            .Docs("user-interface/controls/editor");

        C(list, "CheckBox", cat, "☑️", "Caixa de seleção (marcado/desmarcado).")
            .About("Não tem texto próprio — combine com uma Label (e um TapGestureRecognizer na Label, se quiser que o texto também alterne).")
            .Demo(() => new CheckBox { IsChecked = true })
            .Platform("Microsoft.UI.Xaml.Controls.CheckBox", "UIKit.UIButton (MauiCheckBox)")
            .Prop("IsChecked", "Estado atual (TwoWay).")
            .Prop("Color", "Cor da marca/caixa.")
            .Prop("Command / CommandParameter", "Executado ao alternar (adicionado no .NET 9).")
            .Event("CheckedChanged", "Ao alternar, com e.Value.")
            .Nuance("Visual states 'IsChecked' permitem mudar estilos quando marcado via VisualStateManager.")
            .Sample("""
                <HorizontalStackLayout>
                    <CheckBox x:Name="aceito" />
                    <Label Text="Aceito os termos" VerticalOptions="Center" />
                </HorizontalStackLayout>
                <Button Text="Continuar" IsEnabled="{Binding IsChecked, Source={x:Reference aceito}}" />
                """)
            .Docs("user-interface/controls/checkbox");

        C(list, "Switch", cat, "🔀", "Interruptor liga/desliga.")
            .About("Representa uma configuração binária que tem efeito imediato.")
            .Demo(() => new Switch { IsToggled = true })
            .Platform("Microsoft.UI.Xaml.Controls.ToggleSwitch", "UIKit.UISwitch")
            .Prop("IsToggled", "Estado atual (TwoWay).")
            .Prop("OnColor", "Cor do trilho quando ligado.")
            .Prop("OffColor", "Cor do trilho quando desligado (novo no .NET MAUI 10).")
            .Prop("ThumbColor", "Cor do 'botãozinho'.")
            .Event("Toggled", "Ao alternar, com e.Value.")
            .Nuance("Use os VisualStates \"On\" e \"Off\" para estilizar cada estado.")
            .Nuance("Switch = efeito imediato; CheckBox = escolha que será confirmada depois (ex.: em um formulário).")
            .Sample("""<Switch IsToggled="{Binding ModoEscuro}" OnColor="#512BD4" OffColor="LightGray" ThumbColor="White" />""")
            .Docs("user-interface/controls/switch");

        C(list, "Slider", cat, "🎚️", "Seleciona um valor numérico arrastando.")
            .About("Valor double entre Minimum e Maximum. Ótimo para volume, tamanho, opacidade...")
            .Demo(() => new Slider { Minimum = 0, Maximum = 100, Value = 40, WidthRequest = 300 })
            .Platform("Microsoft.UI.Xaml.Controls.Slider", "UIKit.UISlider")
            .Prop("Value", "Valor atual (TwoWay).")
            .Prop("Minimum / Maximum", "Intervalo. Padrão 0..1.")
            .Prop("MinimumTrackColor / MaximumTrackColor / ThumbColor", "Cores do trilho e do polegar.")
            .Prop("ThumbImageSource", "Imagem do polegar.")
            .Prop("DragStartedCommand / DragCompletedCommand", "Comandos no início/fim do arraste.")
            .Event("ValueChanged", "A cada mudança, com OldValue e NewValue.")
            .Event("DragStarted / DragCompleted", "Início e fim do arraste.")
            .Nuance("A ORDEM importa no XAML: se você definir Minimum=\"10\" enquanto Maximum ainda é o padrão (1), Minimum > Maximum lança exceção. Escreva Maximum antes: Maximum=\"100\" Minimum=\"10\".")
            .Nuance("Slider não tem 'passo'. Para valores inteiros, arredonde em ValueChanged ou no converter.")
            .Sample("""
                <Slider x:Name="slider" Maximum="100" Minimum="0" Value="50" />
                <Label Text="{Binding Value, Source={x:Reference slider}, StringFormat='Valor: {0:F0}'}" />
                """)
            .Docs("user-interface/controls/slider");

        C(list, "Stepper", cat, "➕", "Botões − / + que incrementam um valor.")
            .About("Valor double com Increment fixo entre Minimum e Maximum. Não exibe o valor — mostre-o numa Label.")
            .Demo(() => new Stepper { Minimum = 0, Maximum = 10, Increment = 1, Value = 3 })
            .Platform("Composição de botões (MauiStepper)", "UIKit.UIStepper")
            .Prop("Value", "Valor atual.")
            .Prop("Increment", "Quanto muda a cada clique.")
            .Prop("Minimum / Maximum", "Limites.")
            .Event("ValueChanged", "A cada mudança.")
            .Nuance("Assim como o Slider, a ordem de Maximum/Minimum no XAML importa para evitar exceções.")
            .Sample("""
                <Stepper x:Name="qtd" Maximum="10" Increment="1" />
                <Label Text="{Binding Value, Source={x:Reference qtd}, StringFormat='Quantidade: {0}'}" />
                """)
            .Docs("user-interface/controls/stepper");

        C(list, "DatePicker", cat, "📅", "Seleção de data.")
            .About("No .NET MAUI 10+, Date, MinimumDate e MaximumDate são DateTime? (anuláveis), permitindo 'sem data'. IsOpen abre/fecha o seletor por código.")
            .Demo(() => new DatePicker { Date = DateTime.Today, Format = "dd/MM/yyyy" })
            .Platform("Microsoft.UI.Xaml.Controls.CalendarDatePicker", "UIKit.UIDatePicker")
            .Prop("Date", "Data selecionada (DateTime?).")
            .Prop("MinimumDate / MaximumDate", "Limites (DateTime?).")
            .Prop("Format", "Formato de exibição: \"D\", \"dd/MM/yyyy\", \"MMMM yyyy\"...")
            .Prop("IsOpen", "Abre/fecha o calendário por código (novo).")
            .Prop("TextColor / FontSize / FontFamily", "Aparência do texto.")
            .Event("DateSelected", "Quando a data muda.")
            .Event("Opened / Closed", "Abertura e fechamento do seletor (novos).")
            .Nuance("Com Date anulável, bindings para DateTime (não anulável) no ViewModel continuam funcionando, mas considere DateTime? para representar 'nenhuma data'.")
            .Sample("""<DatePicker Date="{Binding Nascimento}" MaximumDate="{x:Static sys:DateTime.Today}" Format="dd/MM/yyyy" />""")
            .Docs("user-interface/controls/datepicker");

        C(list, "TimePicker", cat, "⏰", "Seleção de horário.")
            .About("Time é TimeSpan? (anulável) no .NET MAUI 10+. Format controla a exibição (\"HH:mm\", \"t\").")
            .Demo(() => new TimePicker { Time = new TimeSpan(14, 30, 0), Format = "HH:mm" })
            .Platform("Microsoft.UI.Xaml.Controls.TimePicker", "UIKit.UIDatePicker (modo hora)")
            .Prop("Time", "Horário selecionado (TimeSpan?).")
            .Prop("Format", "Formato de exibição.")
            .Prop("IsOpen", "Abre/fecha o seletor por código (novo).")
            .Event("TimeSelected", "Quando o horário muda.")
            .Event("Opened / Closed", "Abertura e fechamento do seletor.")
            .Nuance("Ao editar Time no inspetor, use o formato \"hh:mm:ss\" (ex.: 09:15:00).")
            .Sample("""<TimePicker Time="{Binding Alarme}" Format="HH:mm" />""")
            .Docs("user-interface/controls/timepicker");

        C(list, "Picker", cat, "🔽", "Lista suspensa (dropdown) para escolher um item.")
            .About("Itens via Items (strings) ou ItemsSource (objetos) + ItemDisplayBinding. SelectedIndex/SelectedItem indicam a escolha.")
            .Demo(() =>
            {
                var p = new Picker { Title = "Escolha uma fruta", WidthRequest = 260, ItemsSource = Fruits(), ItemDisplayBinding = new Binding(nameof(Fruit.Name)) };
                return p;
            })
            .Platform("Microsoft.UI.Xaml.Controls.ComboBox", "UIKit.UITextField + UIPickerView / UIMenu")
            .Prop("ItemsSource", "Coleção de objetos.")
            .Prop("ItemDisplayBinding", "Qual propriedade do objeto exibir.")
            .Prop("Items", "Lista de strings (alternativa a ItemsSource).")
            .Prop("SelectedIndex / SelectedItem", "Seleção atual (-1 = nenhuma).")
            .Prop("Title / TitleColor", "Texto exibido antes da seleção.")
            .Prop("IsOpen", "Abre/fecha a lista por código (novo).")
            .Event("SelectedIndexChanged", "Quando a seleção muda.")
            .Event("Opened / Closed", "Abertura/fechamento (novos).")
            .Nuance("Alterar a coleção ItemsSource (ObservableCollection) atualiza o Picker, mas manter SelectedItem consistente é responsabilidade sua.")
            .Nuance("ItemDisplayBinding com compiled bindings: use x:DataType no Picker ou Binding com Path explícito.")
            .Sample("""
                <Picker Title="Estado"
                        ItemsSource="{Binding Estados}"
                        ItemDisplayBinding="{Binding Nome, x:DataType=models:Estado}"
                        SelectedItem="{Binding EstadoSelecionado}" />
                """)
            .Docs("user-interface/controls/picker");

        C(list, "SearchBar", cat, "🔍", "Campo de busca com ícone e botão de cancelar.")
            .About("Um InputView especializado. Dispare a busca com SearchCommand (Enter) ou filtre ao vivo em TextChanged.")
            .Demo(() => new SearchBar { Placeholder = "Buscar componentes…", WidthRequest = 340 })
            .Platform("Microsoft.UI.Xaml.Controls.AutoSuggestBox", "UIKit.UISearchBar")
            .Prop("Text / Placeholder", "Texto e dica.")
            .Prop("SearchCommand / SearchCommandParameter", "Executado ao pressionar Enter/ícone de busca.")
            .Prop("CancelButtonColor", "Cor do botão de limpar.")
            .Prop("SearchIconColor", "Cor do ícone de lupa (novo no .NET MAUI 10).")
            .Event("SearchButtonPressed", "Ao confirmar a busca.")
            .Event("TextChanged", "A cada tecla — ideal para filtro instantâneo.")
            .Nuance("Para listas grandes, aplique um 'debounce' (aguarde ~300 ms sem digitação) antes de filtrar.")
            .Sample("""<SearchBar Placeholder="Buscar" SearchCommand="{Binding BuscarCommand}" SearchCommandParameter="{Binding Text, Source={RelativeSource Self}}" />""")
            .Docs("user-interface/controls/searchbar");
    }
}
