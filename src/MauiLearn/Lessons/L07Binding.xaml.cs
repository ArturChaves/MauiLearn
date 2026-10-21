using MauiLearn.ViewModels;

namespace MauiLearn.Lessons;

public partial class L07Binding : ContentView
{
    public L07Binding()
    {
        InitializeComponent();
        // A origem padrão de todos os bindings desta aula:
        BindingContext = new BindingLessonViewModel();
    }
}
