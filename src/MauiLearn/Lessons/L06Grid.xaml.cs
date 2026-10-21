using MauiLearn.Catalog;

namespace MauiLearn.Lessons;

public partial class L06Grid : ContentView
{
    public L06Grid()
    {
        InitializeComponent();
        OnApply(null, EventArgs.Empty);
    }

    private void OnApply(object? sender, EventArgs e)
    {
        if (LabGrid is null || RowsEntry is null || ColsEntry is null || LabCode is null || ErrorLabel is null || SpanCheck is null)
            return;

        RowDefinitionCollection rows;
        ColumnDefinitionCollection cols;
        try
        {
            rows = ComponentCatalog.Rows(RowsEntry.Text ?? "*");
            cols = ComponentCatalog.Columns(ColsEntry.Text ?? "*");
            ErrorLabel.Text = string.Empty;
        }
        catch
        {
            ErrorLabel.Text = "Formato inválido. Use valores separados por vírgula: Auto, *, 2*, 100…";
            return;
        }

        var spacing = Math.Round(SpacingSlider.Value);
        LabGrid.Clear();
        LabGrid.RowDefinitions = rows;
        LabGrid.ColumnDefinitions = cols;
        LabGrid.RowSpacing = spacing;
        LabGrid.ColumnSpacing = spacing;

        var i = 0;
        for (var r = 0; r < rows.Count; r++)
        {
            for (var c = 0; c < cols.Count; c++)
            {
                var spanned = SpanCheck.IsChecked && rows.Count > 1 && cols.Count > 1;
                if (spanned && r < 2 && c < 2 && !(r == 0 && c == 0))
                    continue; // células cobertas pelo span

                var cell = new Border
                {
                    BackgroundColor = Color.FromArgb(ComponentCatalog.Palette[i++ % ComponentCatalog.Palette.Length]),
                    StrokeThickness = 0,
                    Padding = 6,
                    Content = new Label
                    {
                        Text = $"Row={r} Column={c}\n{Describe(rows[r].Height)} × {Describe(cols[c].Width)}",
                        TextColor = Colors.White,
                        FontSize = 11,
                    },
                };
                LabGrid.Add(cell, c, r);
                if (spanned && r == 0 && c == 0)
                {
                    Grid.SetColumnSpan(cell, 2);
                    Grid.SetRowSpan(cell, 2);
                }
            }
        }

        LabCode.Code =
            $"<Grid RowDefinitions=\"{RowsEntry.Text}\" ColumnDefinitions=\"{ColsEntry.Text}\"\n" +
            $"      RowSpacing=\"{spacing}\" ColumnSpacing=\"{spacing}\">\n" +
            (SpanCheck.IsChecked ? "    <Border Grid.ColumnSpan=\"2\" Grid.RowSpan=\"2\" />\n" : "    <Border />\n") +
            "    <Border Grid.Column=\"1\" />  <!-- ... -->\n" +
            "</Grid>";
    }

    private static string Describe(GridLength g) =>
        g.IsAuto ? "Auto" : g.IsStar ? (g.Value == 1 ? "*" : $"{g.Value}*") : $"{g.Value}";
}
