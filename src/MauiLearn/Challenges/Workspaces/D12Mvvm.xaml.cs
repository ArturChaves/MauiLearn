using MauiLearn.ViewModels;

namespace MauiLearn.Challenges.Workspaces;

public partial class D12Mvvm : ContentView
{
    public D12Mvvm()
    {
        InitializeComponent();
        BindingContext = new CounterViewModel();
    }
}
