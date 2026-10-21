using MauiLearn.ViewModels;

namespace MauiLearn.Lessons;

public partial class L08Mvvm : ContentView
{
    public L08Mvvm()
    {
        InitializeComponent();
        BindingContext = new MvvmLessonViewModel();
    }
}
