using System.Collections.ObjectModel;
using System.Reflection;

namespace MauiLearn.Controls;

/// <summary>
/// "Espião de eventos": assina, por reflexão, todos os eventos públicos do objeto
/// e registra cada disparo com os dados do EventArgs. Interaja com a prévia e observe!
/// </summary>
public class EventLogView : ContentView
{
    // Eventos de infraestrutura que poluiriam o log.
    private static readonly HashSet<string> Noisy =
    [
        "PropertyChanged", "PropertyChanging", "BindingContextChanged", "ChildAdded", "ChildRemoved",
        "DescendantAdded", "DescendantRemoved", "ParentChanging", "ParentChanged", "MeasureInvalidated",
        "BatchCommitted", "ChildrenReordered", "FocusChangeRequested", "PlatformEnabledChanged", "WindowChanged",
        "ScrollToRequested", "LayoutChanged", "HandlerChanging",
    ];

    private readonly ObservableCollection<string> _entries = [];
    private readonly List<(object Target, EventInfo Event, Delegate Handler)> _subscriptions = [];
    private readonly CheckBox _showPropertyChanged = new() { IsChecked = false, VerticalOptions = LayoutOptions.Center };
    private readonly Label _subscribed = new() { FontSize = 11, Opacity = 0.6 };

    public EventLogView()
    {
        var list = new CollectionView
        {
            ItemsSource = _entries,
            ItemTemplate = new DataTemplate(() =>
            {
                var l = new Label { FontSize = 12, Padding = new Thickness(8, 3) };
                l.SetDynamicResource(StyleProperty, "Mono");
                l.SetBinding(Label.TextProperty, static (string s) => s);
                return l;
            }),
            EmptyView = new Label { Text = "Interaja com a prévia para ver os eventos aqui…", Opacity = 0.6, Padding = 8 },
        };

        var clear = new Button { Text = "Limpar", FontSize = 11, Padding = new Thickness(8, 2), MinimumHeightRequest = 24 };
        clear.SetDynamicResource(StyleProperty, "SecondaryButton");
        clear.Clicked += (_, _) => _entries.Clear();

        var toolbar = new HorizontalStackLayout
        {
            Spacing = 6,
            Children = { _showPropertyChanged, new Label { Text = "incluir PropertyChanged", FontSize = 12, VerticalOptions = LayoutOptions.Center }, clear },
        };

        var root = new Grid { RowDefinitions = [new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star)], RowSpacing = 4 };
        root.Add(toolbar, 0, 0);
        root.Add(_subscribed, 0, 1);
        root.Add(list, 0, 2);
        Content = root;
    }

    public void Attach(object? target)
    {
        Detach();
        _entries.Clear();
        if (target is null) return;

        var names = new List<string>();
        foreach (var ev in target.GetType().GetEvents(BindingFlags.Public | BindingFlags.Instance))
        {
            if (Noisy.Contains(ev.Name) && ev.Name != "PropertyChanged")
                continue;
            if (CreateHandler(ev) is not { } handler)
                continue;
            try
            {
                ev.AddEventHandler(target, handler);
                _subscriptions.Add((target, ev, handler));
                if (ev.Name != "PropertyChanged") names.Add(ev.Name);
            }
            catch
            {
                // alguns eventos não suportam assinatura externa
            }
        }
        _subscribed.Text = names.Count == 0 ? "Nenhum evento público." : "Ouvindo: " + string.Join(", ", names.OrderBy(n => n));
    }

    public void Detach()
    {
        foreach (var (target, ev, handler) in _subscriptions)
        {
            try { ev.RemoveEventHandler(target, handler); } catch { }
        }
        _subscriptions.Clear();
    }

    private Delegate? CreateHandler(EventInfo ev)
    {
        var handlerType = ev.EventHandlerType;
        var invoke = handlerType?.GetMethod("Invoke");
        var parameters = invoke?.GetParameters();
        if (handlerType is null || parameters is not { Length: 2 } || invoke!.ReturnType != typeof(void))
            return null;

        // Cria um delegate do tipo exato do evento apontando para Relay.Handle<TArgs>.
        var relay = new Relay(this, ev.Name);
        var method = typeof(Relay).GetMethod(nameof(Relay.Handle))!.MakeGenericMethod(parameters[1].ParameterType);
        try
        {
            return Delegate.CreateDelegate(handlerType, relay, method);
        }
        catch
        {
            return null;
        }
    }

    private void Log(string eventName, object? args)
    {
        if (eventName == "PropertyChanged" && !_showPropertyChanged.IsChecked)
            return;

        var line = $"{DateTime.Now:HH:mm:ss.fff}  {eventName}{FormatArgs(args)}";
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _entries.Insert(0, line);
            while (_entries.Count > 80) _entries.RemoveAt(_entries.Count - 1);
        });
    }

    private static string FormatArgs(object? args)
    {
        if (args is null || args.GetType() == typeof(EventArgs))
            return string.Empty;

        var parts = new List<string>();
        foreach (var p in args.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (p.DeclaringType == typeof(EventArgs) || p.GetIndexParameters().Length > 0 || p.Name is "PlatformArgs")
                continue;
            try
            {
                var v = p.GetValue(args);
                var text = v switch
                {
                    null => "null",
                    string s => $"\"{(s.Length > 30 ? s[..30] + "…" : s)}\"",
                    double d => d.ToString("0.##"),
                    System.Collections.IEnumerable e and not string => $"[{e.Cast<object>().Count()}]",
                    _ => v.ToString(),
                };
                parts.Add($"{p.Name}={text}");
            }
            catch
            {
                // ignora getters que lançam exceção
            }
            if (parts.Count >= 4) break;
        }
        return parts.Count == 0 ? $"  ({args.GetType().Name})" : "  { " + string.Join(", ", parts) + " }";
    }

    private sealed class Relay(EventLogView owner, string eventName)
    {
        public void Handle<TArgs>(object? sender, TArgs args) => owner.Log(eventName, args);
    }
}
