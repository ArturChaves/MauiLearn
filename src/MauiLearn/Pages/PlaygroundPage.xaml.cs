using MauiLearn.Controls;
using MauiLearn.Core;
using Microsoft.Maui.Controls.Xaml;

namespace MauiLearn.Pages;

public partial class PlaygroundPage : AppPage
{
    private static readonly (string Name, string Xaml)[] Samples =
    [
        ("Olá, mundo", """
            <VerticalStackLayout Spacing="12" Padding="20">
                <Label Text="Olá, MAUI!" FontSize="32" TextColor="#512BD4" />
                <Button Text="Sou um botão" />
            </VerticalStackLayout>
            """),
        ("Grid 3×3", """
            <Grid RowDefinitions="*,*,*" ColumnDefinitions="*,*,*" RowSpacing="6" ColumnSpacing="6" HeightRequest="300">
                <BoxView Color="#512BD4" />
                <BoxView Grid.Column="1" Color="#D600AA" />
                <BoxView Grid.Column="2" Color="#2E9D5B" />
                <BoxView Grid.Row="1" Grid.ColumnSpan="2" Color="#E59A12" />
                <BoxView Grid.Row="1" Grid.Column="2" Grid.RowSpan="2" Color="#2F80ED" />
                <Label Grid.Row="2" Grid.ColumnSpan="2" Text="Spans!" HorizontalOptions="Center" VerticalOptions="Center" />
            </Grid>
            """),
        ("Binding entre controles (x:Reference)", """
            <VerticalStackLayout Spacing="10" Padding="20">
                <Slider x:Name="s" Maximum="360" />
                <Label Text="{Binding Value, Source={x:Reference s}, StringFormat='Rotação: {0:F0}°'}" />
                <BoxView Color="#D600AA" WidthRequest="120" HeightRequest="120"
                         Rotation="{Binding Value, Source={x:Reference s}}" />
            </VerticalStackLayout>
            """),
        ("Estilos locais", """
            <VerticalStackLayout Padding="20" Spacing="8">
                <VerticalStackLayout.Resources>
                    <Style TargetType="Label">
                        <Setter Property="FontSize" Value="20" />
                        <Setter Property="TextColor" Value="#2E9D5B" />
                    </Style>
                    <Style x:Key="Alerta" TargetType="Label">
                        <Setter Property="TextColor" Value="Red" />
                        <Setter Property="FontAttributes" Value="Bold" />
                    </Style>
                </VerticalStackLayout.Resources>
                <Label Text="Estilo implícito" />
                <Label Text="Estilo explícito" Style="{StaticResource Alerta}" />
                <Label Text="StyleClass do app" StyleClass="destaque,grande" />
            </VerticalStackLayout>
            """),
        ("Triggers", """
            <VerticalStackLayout Padding="20" Spacing="10">
                <Entry Placeholder="Digite algo…">
                    <Entry.Triggers>
                        <Trigger TargetType="Entry" Property="IsFocused" Value="True">
                            <Setter Property="BackgroundColor" Value="#FFF3C4" />
                        </Trigger>
                    </Entry.Triggers>
                </Entry>
                <CheckBox x:Name="c" />
                <Label Text="Marque o CheckBox">
                    <Label.Triggers>
                        <DataTrigger TargetType="Label" Binding="{Binding IsChecked, Source={x:Reference c}}" Value="True">
                            <Setter Property="Text" Value="Marcado! ✅" />
                            <Setter Property="TextColor" Value="Green" />
                        </DataTrigger>
                    </Label.Triggers>
                </Label>
            </VerticalStackLayout>
            """),
        ("Formas e gradientes", """
            <HorizontalStackLayout Spacing="16" Padding="20">
                <Ellipse WidthRequest="100" HeightRequest="100">
                    <Ellipse.Fill>
                        <RadialGradientBrush>
                            <GradientStop Color="Yellow" Offset="0" />
                            <GradientStop Color="OrangeRed" Offset="1" />
                        </RadialGradientBrush>
                    </Ellipse.Fill>
                </Ellipse>
                <Polygon Points="50,0 100,100 0,100" Fill="#512BD4" />
                <Path Data="M 0,50 C 30,0 70,100 100,50" Stroke="#D600AA" StrokeThickness="5" />
            </HorizontalStackLayout>
            """),
        ("VisualStateManager (hover)", """
            <Border Padding="30" Margin="20" StrokeShape="RoundRectangle 16" BackgroundColor="#EEE">
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
                <Label Text="Passe o mouse aqui" HorizontalOptions="Center" />
            </Border>
            """),
        ("CollectionView com x:Array", """
            <CollectionView HeightRequest="320">
                <CollectionView.ItemsSource>
                    <x:Array Type="{x:Type x:String}">
                        <x:String>🍎 Maçã</x:String>
                        <x:String>🍌 Banana</x:String>
                        <x:String>🍇 Uva</x:String>
                        <x:String>🍓 Morango</x:String>
                    </x:Array>
                </CollectionView.ItemsSource>
                <CollectionView.ItemTemplate>
                    <DataTemplate x:DataType="x:String">
                        <Border Margin="4" Padding="12">
                            <Label Text="{Binding .}" FontSize="18" />
                        </Border>
                    </DataTemplate>
                </CollectionView.ItemTemplate>
            </CollectionView>
            """),
    ];

    private CancellationTokenSource? _debounce;

    public PlaygroundPage()
    {
        InitializeComponent();
        Templates.ItemsSource = Samples.Select(s => s.Name).ToList();
        Source.Text = Preferences.Default.Get(SettingsKeys.PlaygroundXaml, Samples[0].Xaml);

        // Atalho de teclado no menu de contexto do editor
        var run = new MenuFlyoutItem { Text = "Executar" };
        run.KeyboardAccelerators.Add(new KeyboardAccelerator
        {
            Modifiers = OperatingSystem.IsMacCatalyst() ? KeyboardAcceleratorModifiers.Cmd : KeyboardAcceleratorModifiers.Ctrl,
            Key = "Enter",
        });
        run.Clicked += (_, _) => Run();
        var menu = new MenuFlyout { run };
        FlyoutBase.SetContextFlyout(Source, menu);

        Run();
    }

    private void OnTemplateChanged(object? sender, EventArgs e)
    {
        if (Templates.SelectedIndex >= 0)
            Source.Text = Samples[Templates.SelectedIndex].Xaml;
    }

    private void OnRunClicked(object? sender, EventArgs e) => Run();

    private void OnSourceChanged(object? sender, TextChangedEventArgs e)
    {
        if (!AutoRun.IsToggled) return;
        _debounce?.Cancel();
        _debounce = new CancellationTokenSource();
        var token = _debounce.Token;
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(600), () =>
        {
            if (!token.IsCancellationRequested) Run();
        });
    }

    private void OnFormatClicked(object? sender, EventArgs e)
    {
        try
        {
            var doc = System.Xml.Linq.XElement.Parse($"<Root xmlns:x=\"x\">{Source.Text}</Root>");
            Source.Text = string.Join("\n", doc.Elements().Select(el => el.ToString())).Replace(" xmlns:x=\"x\"", "");
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = "Não consegui formatar: " + ex.Message;
        }
    }

    private void Run()
    {
        var fragment = Source.Text ?? string.Empty;
        Preferences.Default.Set(SettingsKeys.PlaygroundXaml, fragment);

        // Envolvemos o fragmento num ContentView com os namespaces padrão.
        var xaml = $"""
            <ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                         xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
            {fragment}
            </ContentView>
            """;
        try
        {
            // LoadFromXaml: parser em tempo de execução (mais lento e não verificado em build —
            // por isso o app "de verdade" usa o XAML compilado pelo SourceGen).
            var view = new ContentView().LoadFromXaml(xaml);
            Output.Content = view;
            ErrorLabel.Text = "✓ OK";
            ErrorLabel.TextColor = Color.FromArgb("#2E9D5B");
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex is XamlParseException xpe ? xpe.Message : $"{ex.GetType().Name}: {ex.Message}";
            ErrorLabel.TextColor = Color.FromArgb("#D64545");
        }
    }
}
