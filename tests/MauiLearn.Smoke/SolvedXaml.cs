using System.Text.RegularExpressions;
namespace MauiLearn.Smoke;

// Gera o XAML "resolvido" de cada desafio aplicando substituições no arquivo inicial.
public static class SolvedXaml
{
    public static string? For(string id, string original, string solution)
    {
        var x = Regex.Replace(original, "\\s+x:Class=\"[^\"]+\"", "");
        x = Regex.Replace(x, "<!--[\\s\\S]*?-->", "");
        string R(string a, string b) { if (!x.Contains(a)) throw new Exception($"[{id}] trecho não encontrado: {a}"); return x = x.Replace(a, b); }
        switch (id)
        {
            case "d01": R("Text=\"Mude-me!\"", "Text=\"Olá, MAUI!\""); R("FontSize=\"14\"", "FontSize=\"36\""); R("TextColor=\"Gray\"", "TextColor=\"#512BD4\""); break;
            case "d02": R("Text=\"Botão\"", "Text=\"Salvar\" BackgroundColor=\"#2E9D5B\" CornerRadius=\"20\" Padding=\"24,10\""); break;
            case "d03": R("<VerticalStackLayout x:Name=\"Caixas\" Spacing=\"2\"", "<HorizontalStackLayout x:Name=\"Caixas\" Spacing=\"12\" HorizontalOptions=\"Center\""); R("</VerticalStackLayout>", "</HorizontalStackLayout>"); break;
            case "d04": R("Margin=\"10\">", "HorizontalOptions=\"Center\" VerticalOptions=\"Center\" WidthRequest=\"240\" Padding=\"20\" Margin=\"0\">"); break;
            case "d05":
                var m = Regex.Match(solution, "<Grid[\\s\\S]*</Grid>");
                x = Regex.Replace(x, "<Grid[\\s\\S]*</Grid>", m.Value); break;
            case "d06": R("Text=\"Volume: ?\"", "Text=\"{Binding Value, Source={x:Reference Volume}, StringFormat='Volume: {0:F0}'}\""); break;
            case "d07":
                R("<ContentView.Resources>\n    \n  </ContentView.Resources>", "<ContentView.Resources><Style x:Key=\"Destaque\" TargetType=\"Label\"><Setter Property=\"TextColor\" Value=\"#D600AA\" /><Setter Property=\"FontSize\" Value=\"22\" /></Style></ContentView.Resources>");
                x = x.Replace(" TextColor=\"#D600AA\" FontSize=\"22\" FontAttributes=\"Bold\"", " Style=\"{StaticResource Destaque}\""); break;
            case "d08": R("<Button x:Name=\"Enviar\" Text=\"Enviar\" />", "<Button x:Name=\"Enviar\" Text=\"Enviar\"><Button.Triggers><DataTrigger TargetType=\"Button\" Binding=\"{Binding Text.Length, Source={x:Reference Email}}\" Value=\"0\"><Setter Property=\"IsEnabled\" Value=\"False\" /></DataTrigger></Button.Triggers></Button>"); break;
            case "d09": R("<CollectionView x:Name=\"Lista\">", "<CollectionView x:Name=\"Lista\" ItemsLayout=\"VerticalGrid, 3\">"); R("<Label Text=\"{Binding .}\" FontSize=\"18\" Padding=\"8\" />", "<Border Padding=\"12\"><Label Text=\"{Binding .}\" /></Border>"); break;
            case "d10": R("StrokeShape=\"RoundRectangle 16\">", "StrokeShape=\"RoundRectangle 16\"><VisualStateManager.VisualStateGroups><VisualStateGroup Name=\"CommonStates\"><VisualState Name=\"Normal\" /><VisualState Name=\"PointerOver\"><VisualState.Setters><Setter Property=\"Scale\" Value=\"1.05\" /></VisualState.Setters></VisualState></VisualStateGroup></VisualStateManager.VisualStateGroups>"); break;
            case "d11": R("StrokeShape=\"Rectangle\">", "StrokeShape=\"RoundRectangle 16\"><Border.Shadow><Shadow Radius=\"16\" /></Border.Shadow>"); break;
            case "d12": R("Text=\"0\"", "Text=\"{Binding Count}\""); R("Text=\"−\"", "Text=\"−\" Command=\"{Binding DecrementCommand}\""); R("Text=\"+\"", "Text=\"+\" Command=\"{Binding IncrementCommand}\""); x = x.Replace("x:DataType=\"CounterViewModel\"", ""); break;
            case "d13": R("CornerRadius=\"20\" />", "CornerRadius=\"20\"><BoxView.GestureRecognizers><TapGestureRecognizer NumberOfTapsRequired=\"2\" /></BoxView.GestureRecognizers></BoxView>"); break;
            case "d14":
                var vsm = Regex.Match(solution, "<VisualStateManager.VisualStateGroups>[\\s\\S]*</VisualStateManager.VisualStateGroups>").Value;
                R("VerticalOptions=\"Center\">\n    <BoxView", "VerticalOptions=\"Center\">" + vsm + "\n    <BoxView"); break;
            default: return null;
        }
        return x;
    }
}
