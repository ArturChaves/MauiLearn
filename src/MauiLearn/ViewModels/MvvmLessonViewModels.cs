using System.Collections.ObjectModel;
using MauiLearn.Core;

namespace MauiLearn.ViewModels;

/// <summary>O "hello world" do MVVM: estado + comandos, sem nenhuma referência à UI.</summary>
public sealed class CounterViewModel : ObservableObject
{
    private int _count;

    public CounterViewModel()
    {
        IncrementCommand = new RelayCommand(() => Count++);
        DecrementCommand = new RelayCommand(() => Count--, () => Count > 0);
        ResetCommand = new RelayCommand(() => Count = 0, () => Count != 0);
    }

    public int Count
    {
        get => _count;
        set
        {
            if (!SetProperty(ref _count, value)) return;
            OnPropertyChanged(nameof(Message));
            // Avisa os botões para reavaliarem CanExecute (habilitar/desabilitar).
            DecrementCommand.NotifyCanExecuteChanged();
            ResetCommand.NotifyCanExecuteChanged();
        }
    }

    public string Message => Count switch
    {
        0 => "Clique em +",
        < 5 => $"Você clicou {Count} vez(es)",
        < 10 => "Continue! 🔥",
        _ => "Você é uma máquina de cliques! 🚀",
    };

    public RelayCommand IncrementCommand { get; }
    public RelayCommand DecrementCommand { get; }
    public RelayCommand ResetCommand { get; }
}

public sealed class TodoItem : ObservableObject
{
    private bool _done;

    public TodoItem(string title) => Title = title;

    public string Title { get; }

    public bool Done
    {
        get => _done;
        set => SetProperty(ref _done, value);
    }
}

/// <summary>Lista de tarefas: ObservableCollection + comandos com parâmetro.</summary>
public sealed class TodoViewModel : ObservableObject
{
    private string _newTitle = string.Empty;

    public TodoViewModel()
    {
        Items = [new("Estudar Layouts"), new("Entender Binding"), new("Fazer o desafio 06")];
        AddCommand = new RelayCommand(Add, () => !string.IsNullOrWhiteSpace(NewTitle));
        RemoveCommand = new RelayCommand(p => { if (p is TodoItem item) Items.Remove(item); });
        ClearDoneCommand = new RelayCommand(() =>
        {
            foreach (var done in Items.Where(i => i.Done).ToList())
                Items.Remove(done);
        });
    }

    public ObservableCollection<TodoItem> Items { get; }

    public string NewTitle
    {
        get => _newTitle;
        set
        {
            if (SetProperty(ref _newTitle, value))
                AddCommand.NotifyCanExecuteChanged();
        }
    }

    public RelayCommand AddCommand { get; }
    public RelayCommand RemoveCommand { get; }
    public RelayCommand ClearDoneCommand { get; }

    private void Add()
    {
        Items.Add(new TodoItem(NewTitle.Trim()));
        NewTitle = string.Empty;
    }
}

/// <summary>Agrupa os ViewModels da aula de MVVM.</summary>
public sealed class MvvmLessonViewModel
{
    public CounterViewModel Counter { get; } = new();
    public TodoViewModel Todo { get; } = new();
}
