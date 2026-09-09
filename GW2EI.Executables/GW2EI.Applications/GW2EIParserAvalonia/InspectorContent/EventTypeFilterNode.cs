using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIParserAvalonia.ViewModels;

public sealed partial class EventTypeFilterNode : ObservableObject
{
    [ObservableProperty]
    private bool? isChecked = true;
    [ObservableProperty]
    private bool isExpanded = true;
    private bool _isUpdatingHierarchy;
    private Dictionary<Type, EventTypeFilterNode>? _typeLookup;
    private HashSet<Type>? _visibleTypes;
    private readonly string? _displayName;
    public string Name => _displayName ?? EventType.Name;
    public int Count { get; private set; }
    public Type EventType { get; }
    public EventTypeFilterNode? Parent { get; set; }
    public ObservableCollection<EventTypeFilterNode> Children { get; } = [];
    public event EventHandler? FilterChanged;

    public EventTypeFilterNode(Type type, int count, string? displayName = null)
    {
        EventType = type;
        Count = count;
        _displayName = displayName;
    }
    internal void SetTypeLookup(Dictionary<Type, EventTypeFilterNode> lookup)
    {
        _typeLookup = lookup;
        UpdateVisibleTypes();
    }

    public static IReadOnlyList<EventTypeFilterNode> Build(IReadOnlyList<TimeCombatEvent> timeEvents, IReadOnlyList<object> nonTimeEvents)
    {
        var timeRoot = BuildTimeEvents(timeEvents);
        var nonTimeRoot = BuildNonTimeEvents(nonTimeEvents);

        return [timeRoot, nonTimeRoot];
    }

    private static EventTypeFilterNode BuildTimeEvents(IReadOnlyList<TimeCombatEvent> events)
    {
        var nodes = events.GroupBy(e => e.GetType()).ToDictionary(g => g.Key, g => new EventTypeFilterNode(g.Key, g.Count()));

        foreach (var node in nodes.Values.ToList())
        {
            foreach (var baseType in GetEventBaseTypes(node.EventType))
            {
                if (!nodes.ContainsKey(baseType))
                {
                    nodes.Add(baseType, new EventTypeFilterNode(baseType, -1));
                }
            }
        }

        foreach (var node in nodes.Values)
        {
            foreach (var baseType in GetEventBaseTypes(node.EventType))
            {
                if (nodes.TryGetValue(baseType, out var parent))
                {
                    node.Parent = parent;
                    parent.Children.Add(node);
                    break;
                }
            }
        }

        var roots = nodes.Values.Where(x => x.Parent == null).ToArray();
        EventTypeFilterNode root;

        if (roots.Length == 1)
        {
            root = roots[0];
        }
        else
        {
            root = new EventTypeFilterNode(typeof(TimeCombatEvent), -1, "Time Combat Events");

            foreach (var child in roots)
            {
                child.Parent = root;
                root.Children.Add(child);
            }
        }

        CalculateParentCounts(root);

        root.SetTypeLookup(nodes);

        return root;
    }

    private static EventTypeFilterNode BuildNonTimeEvents(IReadOnlyList<object> events)
    {
        var nodes = events.GroupBy(e => e.GetType()).Select(g => new EventTypeFilterNode(g.Key, g.Count())).ToList();

        var root = new EventTypeFilterNode(typeof(object), nodes.Sum(x => x.Count), "Non-Time Combat Events");

        foreach (var node in nodes)
        {
            node.Parent = root;
            root.Children.Add(node);
        }

        var lookup = nodes.ToDictionary(x => x.EventType, x => x);
        root.SetTypeLookup(lookup);

        return root;
    }

    public bool IsEventVisible(Type eventType)
    {
        return _visibleTypes?.Contains(eventType) == true;
    }

    partial void OnIsCheckedChanged(bool? value)
    {
        var root = GetRoot();

        // This change was made by another node while the hierarchy was already being synchronized.
        if (root._isUpdatingHierarchy)
        {
            return;
        }

        root._isUpdatingHierarchy = true;

        try
        {
            SetDescendants(value);
            UpdateParents();
        }
        finally
        {
            root._isUpdatingHierarchy = false;
        }

        root.UpdateVisibleTypes();
        root.FilterChanged?.Invoke(root, EventArgs.Empty);
    }

    private void SetDescendants(bool? value)
    {
        foreach (var child in Children)
        {
            if (child.IsChecked != value)
            {
                child.IsChecked = value;
            }

            child.SetDescendants(value);
        }
    }

    private void UpdateParents()
    {
        var parent = Parent;

        while (parent != null)
        {
            bool allChecked = parent.Children.All(child => child.IsChecked == true);
            bool allUnchecked = parent.Children.All(child => child.IsChecked == false);

            bool? value = allChecked ? true : allUnchecked ? false : null;

            if (parent.IsChecked != value)
            {
                parent.IsChecked = value;
            }

            parent = parent.Parent;
        }
    }

    private EventTypeFilterNode GetRoot()
    {
        var root = this;

        while (root.Parent != null)
        {
            root = root.Parent;
        }

        return root;
    }

    private static IEnumerable<Type> GetEventBaseTypes(Type type)
    {
        Type? baseType = type.BaseType;

        while (baseType != null && typeof(TimeCombatEvent).IsAssignableFrom(baseType))
        {
            yield return baseType;
            baseType = baseType.BaseType;
        }
    }

    private static int CalculateParentCounts(EventTypeFilterNode node)
    {
        if (node.Children.Count == 0)
        {
            return node.Count;
        }

        node.Count = node.Children.Sum(CalculateParentCounts);
        return node.Count;
    }

    private void UpdateVisibleTypes()
    {
        if (_typeLookup == null)
        {
            return;
        }

        _visibleTypes = [];

        foreach (var pair in _typeLookup)
        {
            if (pair.Value.IsChecked != false)
            {
                _visibleTypes.Add(pair.Key);
            }
        }
    }
}
