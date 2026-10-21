using MauiLearn.Core;

namespace MauiLearn.ViewModels;

/// <summary>ViewModel usado na aula de Data Binding.</summary>
public sealed class BindingLessonViewModel : ObservableObject
{
    private string _name = "Ada";
    private string _surname = "Lovelace";
    private string? _nickname;
    private double _volume = 30;
    private double _celsius = 25;
    private DateTime _date = DateTime.Today;
    private decimal _price = 1234.5m;

    public string Name
    {
        get => _name;
        set
        {
            if (SetProperty(ref _name, value))
                OnPropertyChanged(nameof(Greeting)); // propriedade calculada também precisa notificar
        }
    }

    public string Surname
    {
        get => _surname;
        set
        {
            if (SetProperty(ref _surname, value))
                OnPropertyChanged(nameof(Greeting));
        }
    }

    public string Greeting => $"Olá, {Name} {Surname}! 👋";

    public string? Nickname
    {
        get => _nickname;
        set => SetProperty(ref _nickname, value);
    }

    public double Volume
    {
        get => _volume;
        set => SetProperty(ref _volume, value);
    }

    public double Celsius
    {
        get => _celsius;
        set => SetProperty(ref _celsius, value);
    }

    public DateTime Date
    {
        get => _date;
        set => SetProperty(ref _date, value);
    }

    public decimal Price
    {
        get => _price;
        set => SetProperty(ref _price, value);
    }
}
