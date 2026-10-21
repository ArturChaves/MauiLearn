using MauiLearn.Catalog;

namespace MauiLearn.Lessons;

public partial class L05Layouts : ContentView
{
    private static readonly string[] Options = ["Fill", "Start", "Center", "End"];

    public L05Layouts()
    {
        InitializeComponent();
        HPicker.ItemsSource = Options;
        VPicker.ItemsSource = Options;
        HPicker.SelectedIndex = 0;
        VPicker.SelectedIndex = 0;
        OnLabChanged(null, EventArgs.Empty);
        OnKindChanged(null, EventArgs.Empty);
    }

    private static LayoutOptions ToOptions(int index) => index switch
    {
        1 => LayoutOptions.Start,
        2 => LayoutOptions.Center,
        3 => LayoutOptions.End,
        _ => LayoutOptions.Fill,
    };

    private void OnLabChanged(object? sender, EventArgs e)
    {
        if (Child is null || LabCode is null || ParentGrid is null) return;
        var padding = Math.Round(PaddingSlider.Value);
        var margin = Math.Round(MarginSlider.Value);
        var width = Math.Round(WidthSlider.Value);

        ParentGrid.Padding = padding;
        Child.Margin = margin;
        Child.WidthRequest = width <= 0 ? -1 : width; // -1 = automático
        Child.HorizontalOptions = ToOptions(HPicker.SelectedIndex);
        Child.VerticalOptions = ToOptions(VPicker.SelectedIndex);

        var h = Options[Math.Max(0, HPicker.SelectedIndex)];
        var v = Options[Math.Max(0, VPicker.SelectedIndex)];
        LabCode.Code =
            $"<Grid Padding=\"{padding}\">\n" +
            $"    <Border Margin=\"{margin}\"" + (width > 0 ? $" WidthRequest=\"{width}\"" : "") + "\n" +
            $"            HorizontalOptions=\"{h}\" VerticalOptions=\"{v}\" />\n" +
            "</Grid>";
    }

    private void OnKindChanged(object? sender, EventArgs e)
    {
        if (KindHost is null || KindCode is null || SpacingSlider is null) return;
        if (sender is RadioButton { IsChecked: false }) return;

        var spacing = Math.Round(SpacingSlider.Value);
        Layout layout;
        string code;
        if (KindHorizontal.IsChecked)
        {
            layout = new HorizontalStackLayout { Spacing = spacing };
            code = $"<HorizontalStackLayout Spacing=\"{spacing}\"> … </HorizontalStackLayout>";
        }
        else if (KindFlex.IsChecked)
        {
            layout = new FlexLayout { Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap, JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Start };
            code = "<FlexLayout Wrap=\"Wrap\" JustifyContent=\"Start\"> … </FlexLayout>\n<!-- FlexLayout não tem Spacing: use Margin nos filhos -->";
        }
        else if (KindGrid.IsChecked)
        {
            layout = new Grid
            {
                ColumnDefinitions = ComponentCatalog.Columns("*,*,*"),
                RowDefinitions = ComponentCatalog.Rows("Auto,Auto,Auto"),
                RowSpacing = spacing,
                ColumnSpacing = spacing,
            };
            code = $"<Grid ColumnDefinitions=\"*,*,*\" RowDefinitions=\"Auto,Auto,Auto\"\n      RowSpacing=\"{spacing}\" ColumnSpacing=\"{spacing}\">\n    <!-- filhos com Grid.Row / Grid.Column -->\n</Grid>";
        }
        else
        {
            layout = new VerticalStackLayout { Spacing = spacing };
            code = $"<VerticalStackLayout Spacing=\"{spacing}\"> … </VerticalStackLayout>";
        }

        for (var i = 0; i < 7; i++)
        {
            var box = new Border
            {
                BackgroundColor = Color.FromArgb(ComponentCatalog.Palette[i % ComponentCatalog.Palette.Length]),
                StrokeThickness = 0,
                Padding = new Thickness(14, 8),
                Content = new Label { Text = $"Item {i + 1}", TextColor = Colors.White },
            };
            if (layout is FlexLayout) box.Margin = spacing / 2;
            if (layout is Grid) { Grid.SetColumn(box, i % 3); Grid.SetRow(box, i / 3); }
            layout.Add(box);
        }

        KindHost.Content = layout;
        KindCode.Code = code;
    }
}
