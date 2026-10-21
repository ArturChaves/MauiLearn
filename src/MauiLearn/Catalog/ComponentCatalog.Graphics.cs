using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using MauiRectangle = Microsoft.Maui.Controls.Shapes.Rectangle;
using ShapePath = Microsoft.Maui.Controls.Shapes.Path;

namespace MauiLearn.Catalog;

public static partial class ComponentCatalog
{
    private static void AddGraphics(List<ComponentInfo> list)
    {
        const ComponentCategory cat = ComponentCategory.Graphics;

        C(list, "Rectangle", cat, "▭", "Forma retangular com preenchimento e contorno (com RadiusX/RadiusY).")
            .About("Shapes (Rectangle, Ellipse, Line, Polyline, Polygon, Path, RoundRectangle) são Views que desenham geometria vetorial com Fill (Brush) e Stroke.")
            .Demo(() => new MauiRectangle
            {
                Fill = new SolidColorBrush(Color.FromArgb("#512BD4")),
                Stroke = new SolidColorBrush(Color.FromArgb("#D600AA")),
                StrokeThickness = 4,
                RadiusX = 12,
                RadiusY = 12,
                WidthRequest = 200,
                HeightRequest = 120,
            })
            .Prop("Fill", "Brush de preenchimento.")
            .Prop("Stroke / StrokeThickness", "Contorno.")
            .Prop("StrokeDashArray / StrokeDashOffset", "Tracejado.")
            .Prop("StrokeLineCap / StrokeLineJoin", "Pontas e junções das linhas.")
            .Prop("RadiusX / RadiusY", "Arredondamento dos cantos.")
            .Prop("Aspect", "Como a forma se estica: None, Fill, Uniform, UniformToFill.")
            .Nuance("Formas sem WidthRequest/HeightRequest podem ter tamanho 0. Sempre dimensione.")
            .Sample("""<Rectangle Fill="#512BD4" Stroke="Black" StrokeThickness="2" RadiusX="10" RadiusY="10" WidthRequest="150" HeightRequest="80" />""")
            .Docs("user-interface/controls/shapes/rectangle")
            .Lesson("graficos");

        C(list, "Ellipse", cat, "⬭", "Elipse ou círculo.")
            .About("Um círculo é uma Ellipse com largura igual à altura.")
            .Demo(() => new Ellipse
            {
                Fill = new RadialGradientBrush([new GradientStop(Color.FromArgb("#FFD54F"), 0), new GradientStop(Color.FromArgb("#E59A12"), 1)], new Point(0.35, 0.35), 0.8),
                Stroke = new SolidColorBrush(Color.FromArgb("#A06000")),
                StrokeThickness = 3,
                WidthRequest = 140,
                HeightRequest = 140,
            })
            .Prop("Fill / Stroke / StrokeThickness", "Preenchimento e contorno.")
            .Sample("""<Ellipse Fill="Gold" Stroke="Orange" StrokeThickness="3" WidthRequest="100" HeightRequest="100" />""")
            .Docs("user-interface/controls/shapes/ellipse")
            .Lesson("graficos");

        C(list, "Line", cat, "➖", "Linha reta entre dois pontos (X1,Y1) → (X2,Y2).")
            .Demo(() => new Line { X1 = 0, Y1 = 0, X2 = 220, Y2 = 80, Stroke = new SolidColorBrush(Color.FromArgb("#512BD4")), StrokeThickness = 6, StrokeLineCap = PenLineCap.Round, StrokeDashArray = [2, 2] })
            .Prop("X1 / Y1 / X2 / Y2", "Pontos inicial e final.")
            .Prop("StrokeLineCap", "Flat, Square, Round.")
            .Prop("StrokeDashArray", "Ex.: \"2,2\" (em múltiplos da espessura).")
            .Nuance("StrokeDashArray é medido em múltiplos de StrokeThickness, não em pixels.")
            .Sample("""<Line X1="0" Y1="0" X2="200" Y2="0" Stroke="Gray" StrokeThickness="2" StrokeDashArray="4,2" />""")
            .Docs("user-interface/controls/shapes/line");

        C(list, "Polyline", cat, "〰️", "Sequência de segmentos de linha conectados (aberta).")
            .Demo(() => new Polyline
            {
                Points = [new Point(0, 80), new Point(40, 20), new Point(80, 60), new Point(120, 10), new Point(160, 70), new Point(200, 30)],
                Stroke = new SolidColorBrush(Color.FromArgb("#2E9D5B")),
                StrokeThickness = 4,
                StrokeLineJoin = PenLineJoin.Round,
            })
            .Prop("Points", "Lista de pontos: \"0,80 40,20 80,60\".")
            .Prop("FillRule", "EvenOdd ou Nonzero (quando há Fill).")
            .Sample("""<Polyline Points="0,50 25,0 50,50 75,0 100,50" Stroke="Green" StrokeThickness="3" />""")
            .Docs("user-interface/controls/shapes/polyline");

        C(list, "Polygon", cat, "⬟", "Forma fechada por pontos.")
            .Demo(() => new Polygon
            {
                Points = [new Point(60, 0), new Point(120, 45), new Point(97, 115), new Point(23, 115), new Point(0, 45)],
                Fill = new SolidColorBrush(Color.FromArgb("#D600AA")),
                Stroke = new SolidColorBrush(Color.FromArgb("#512BD4")),
                StrokeThickness = 3,
            })
            .Prop("Points", "Vértices; o último se conecta ao primeiro automaticamente.")
            .Prop("FillRule", "EvenOdd / Nonzero — importa em polígonos que se cruzam (estrelas).")
            .Sample("""<Polygon Points="50,0 100,100 0,100" Fill="Orange" />""")
            .Docs("user-interface/controls/shapes/polygon");

        C(list, "Path", cat, "✒️", "Desenha qualquer geometria, inclusive curvas, via 'mini-linguagem' de caminho (SVG-like).")
            .About("Data aceita a sintaxe de caminho (M, L, C, Q, A, Z...), a mesma usada em SVG. Ótimo para ícones vetoriais.")
            .Demo(() => new ShapePath
            {
                Data = (Geometry)new PathGeometryConverter().ConvertFromInvariantString("M 10,100 C 40,0 80,0 110,100 S 180,200 210,100")!,
                Stroke = new SolidColorBrush(Color.FromArgb("#2F80ED")),
                StrokeThickness = 5,
                HeightRequest = 160,
                WidthRequest = 230,
            })
            .Prop("Data", "Geometria: string de caminho ou PathGeometry/EllipseGeometry/RectangleGeometry/GeometryGroup.")
            .Prop("RenderTransform", "Transformações (rotate, scale, translate).")
            .Nuance("Copie o atributo 'd' de um SVG para Data — geralmente funciona direto.")
            .Sample("""<Path Data="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z" Fill="Gold" Aspect="Uniform" HeightRequest="48" />""")
            .Docs("user-interface/controls/shapes/path");

        C(list, "SolidColorBrush", cat, "🟣", "Pincel de cor única.")
            .About("Brushes pintam Background, Fill e Stroke. Em XAML, uma cor simples (\"Red\") já vira SolidColorBrush via conversor.")
            .Demo(() => BrushDemo(new SolidColorBrush(Color.FromArgb("#512BD4"))))
            .Prop("Color", "A cor.")
            .Sample("""<Border Background="#512BD4" />""")
            .Docs("user-interface/brushes/solidcolor");

        C(list, "LinearGradientBrush", cat, "🌅", "Degradê linear entre dois pontos (StartPoint → EndPoint).")
            .About("StartPoint/EndPoint são relativos (0..1) à área pintada. Cada GradientStop tem Color e Offset (0..1).")
            .Demo(() => BrushDemo(new LinearGradientBrush(
                [new GradientStop(Color.FromArgb("#7B5CFA"), 0), new GradientStop(Color.FromArgb("#D600AA"), 0.5f), new GradientStop(Color.FromArgb("#E59A12"), 1)],
                new Point(0, 0), new Point(1, 1))))
            .Prop("StartPoint / EndPoint", "Direção: (0,0)→(1,0) horizontal; (0,0)→(0,1) vertical; (0,0)→(1,1) diagonal.")
            .Prop("GradientStops", "As cores e suas posições.")
            .Nuance("Edite StartPoint/EndPoint no inspetor (ex.: \"0,0\" e \"1,0\") e veja a direção mudar.")
            .Sample("""
                <Border>
                    <Border.Background>
                        <LinearGradientBrush StartPoint="0,0" EndPoint="1,0">
                            <GradientStop Color="Purple" Offset="0" />
                            <GradientStop Color="Orange" Offset="1" />
                        </LinearGradientBrush>
                    </Border.Background>
                </Border>
                """)
            .Docs("user-interface/brushes/lineargradient");

        C(list, "RadialGradientBrush", cat, "🌀", "Degradê circular a partir de um centro.")
            .Demo(() => BrushDemo(new RadialGradientBrush(
                [new GradientStop(Color.FromArgb("#FFFFFF"), 0), new GradientStop(Color.FromArgb("#2F80ED"), 1)],
                new Point(0.5, 0.5), 0.6)))
            .Prop("Center", "Centro relativo (0..1).")
            .Prop("Radius", "Raio relativo (0..1).")
            .Prop("GradientStops", "Cores e posições.")
            .Sample("""
                <RadialGradientBrush Center="0.5,0.5" Radius="0.5">
                    <GradientStop Color="White" Offset="0" />
                    <GradientStop Color="Blue" Offset="1" />
                </RadialGradientBrush>
                """)
            .Docs("user-interface/brushes/radialgradient");

        C(list, "Shadow", cat, "🌑", "Sombra aplicável a qualquer VisualElement.")
            .About("VisualElement.Shadow recebe um Shadow com Brush, Offset, Radius (desfoque) e Opacity.")
            .Demo(() =>
            {
                var shadow = new Shadow { Brush = new SolidColorBrush(Colors.Black), Offset = new Point(8, 8), Radius = 16, Opacity = 0.5f };
                var border = new Border
                {
                    WidthRequest = 200,
                    HeightRequest = 110,
                    BackgroundColor = Colors.White,
                    StrokeShape = new RoundRectangle { CornerRadius = 16 },
                    Shadow = shadow,
                    Content = new Label { Text = "Sombra", TextColor = Colors.Black, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center },
                };
                return new DemoInstance(new ContentView { Padding = 24, Content = border }, shadow);
            })
            .Prop("Brush", "Cor da sombra.")
            .Prop("Offset", "Deslocamento \"x,y\".")
            .Prop("Radius", "Desfoque.")
            .Prop("Opacity", "Transparência (0..1).")
            .Nuance("Sombras são caras para renderizar: evite-as em centenas de itens de uma lista.")
            .Nuance("Em Labels, a sombra se aplica ao texto; em Border, à forma.")
            .Sample("""
                <Border.Shadow>
                    <Shadow Brush="Black" Offset="4,4" Radius="10" Opacity="0.4" />
                </Border.Shadow>
                """)
            .Docs("user-interface/shadow");

        C(list, "GraphicsView", cat, "🖌️", "Tela de desenho 2D: você desenha com código (IDrawable + ICanvas).")
            .About("Microsoft.Maui.Graphics oferece uma API de desenho imediato (FillRectangle, DrawString, paths...). Implemente IDrawable.Draw e chame Invalidate() para redesenhar. " +
                   "Tem eventos de interação (StartInteraction, DragInteraction) e hover no desktop.")
            .Demo(() =>
            {
                var drawable = new SketchDrawable();
                var view = new GraphicsView { Drawable = drawable, HeightRequest = 240, WidthRequest = 420, BackgroundColor = Color.FromArgb("#11512BD4") };
                view.StartInteraction += (_, e) => { drawable.Begin(e.Touches[0]); view.Invalidate(); };
                view.DragInteraction += (_, e) => { drawable.Add(e.Touches[0]); view.Invalidate(); };
                view.StartHoverInteraction += (_, e) => { drawable.Hover = e.Touches[0]; view.Invalidate(); };
                view.MoveHoverInteraction += (_, e) => { drawable.Hover = e.Touches[0]; view.Invalidate(); };
                view.EndHoverInteraction += (_, _) => { drawable.Hover = null; view.Invalidate(); };
                return view;
            })
            .Prop("Drawable", "Objeto IDrawable que desenha.")
            .Event("StartInteraction / DragInteraction / EndInteraction", "Clique e arraste (mouse/toque).")
            .Event("StartHoverInteraction / MoveHoverInteraction / EndHoverInteraction", "Mouse passando por cima (desktop).")
            .Nuance("Desenhe com o mouse na prévia! O código está em Catalog/SketchDrawable.cs.")
            .Nuance("Invalidate() pede um novo Draw. Nunca desenhe fora do método Draw.")
            .Code("""
                public class MeuDesenho : IDrawable
                {
                    public void Draw(ICanvas canvas, RectF area)
                    {
                        canvas.FillColor = Colors.Purple;
                        canvas.FillRoundedRectangle(area.Center.X - 50, area.Center.Y - 50, 100, 100, 12);
                        canvas.FontColor = Colors.White;
                        canvas.DrawString("Olá", area, HorizontalAlignment.Center, VerticalAlignment.Center);
                    }
                }
                """)
            .Sample("""
                <ContentPage.Resources>
                    <local:MeuDesenho x:Key="desenho" />
                </ContentPage.Resources>
                <GraphicsView Drawable="{StaticResource desenho}" HeightRequest="300" />
                """)
            .Docs("user-interface/graphics/")
            .Lesson("graficos");

        C(list, "Clip (Geometry)", cat, "✂️", "Recorta qualquer elemento com uma geometria.")
            .WithId("Clip")
            .About("VisualElement.Clip recebe uma Geometry (EllipseGeometry, RectangleGeometry, RoundRectangleGeometry, PathGeometry). Só a área dentro da geometria é visível.")
            .Demo(() =>
            {
                var geometry = new EllipseGeometry { Center = new Point(100, 70), RadiusX = 90, RadiusY = 60 };
                var image = new Image { Source = "sample_landscape.png", WidthRequest = 200, HeightRequest = 140, Aspect = Aspect.AspectFill, Clip = geometry };
                return new DemoInstance(image, geometry);
            })
            .Prop("Center / RadiusX / RadiusY", "Para EllipseGeometry (o inspetor edita a geometria).")
            .Nuance("As coordenadas da geometria são absolutas (em unidades do elemento), não relativas.")
            .Sample("""
                <Image Source="foto.png" WidthRequest="120" HeightRequest="120">
                    <Image.Clip>
                        <EllipseGeometry Center="60,60" RadiusX="60" RadiusY="60" />
                    </Image.Clip>
                </Image>
                """)
            .Docs("user-interface/controls/shapes/geometries");
    }

    private static DemoInstance BrushDemo(Brush brush)
    {
        var border = new Border
        {
            WidthRequest = 260,
            HeightRequest = 140,
            Background = brush,
            StrokeShape = new RoundRectangle { CornerRadius = 16 },
            StrokeThickness = 0,
        };
        return new DemoInstance(border, brush);
    }
}

/// <summary>Desenho livre com o mouse — demo do GraphicsView.</summary>
public sealed class SketchDrawable : IDrawable
{
    private readonly List<List<PointF>> _strokes = [];

    public PointF? Hover { get; set; }

    public void Begin(PointF p) => _strokes.Add([p]);

    public void Add(PointF p)
    {
        if (_strokes.Count == 0) Begin(p);
        _strokes[^1].Add(p);
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FontColor = Colors.Gray;
        canvas.FontSize = 12;
        canvas.DrawString("Clique e arraste para desenhar", dirtyRect.X + 8, dirtyRect.Y + 6, 300, 20,
            HorizontalAlignment.Left, VerticalAlignment.Top);

        canvas.StrokeColor = Color.FromArgb("#512BD4");
        canvas.StrokeSize = 4;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeLineJoin = LineJoin.Round;
        foreach (var stroke in _strokes)
        {
            if (stroke.Count < 2) continue;
            var path = new PathF(stroke[0]);
            foreach (var p in stroke.Skip(1)) path.LineTo(p);
            canvas.DrawPath(path);
        }

        if (Hover is { } h)
        {
            canvas.FillColor = Color.FromArgb("#55D600AA");
            canvas.FillCircle(h.X, h.Y, 10);
        }
    }
}
