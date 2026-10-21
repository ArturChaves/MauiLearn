using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiLearn.Core;

/// <summary>
/// Implementação "na mão" de INotifyPropertyChanged — a base do MVVM.
/// Em projetos reais você pode usar o CommunityToolkit.Mvvm ([ObservableProperty]),
/// mas entender esta classe é essencial: é ISSO que faz o Binding atualizar a tela.
/// </summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>Atribui o campo e notifica apenas se o valor mudou.</summary>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
