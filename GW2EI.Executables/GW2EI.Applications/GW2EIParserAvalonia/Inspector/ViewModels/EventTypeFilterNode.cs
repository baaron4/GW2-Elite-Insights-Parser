using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIParserAvalonia.ViewModels;

public sealed partial class EventTypeFilterNodeModel : ObservableObject
{
    [ObservableProperty]
    private bool? isChecked = true;
    [ObservableProperty]
    private bool isExpanded = true;
    private bool _isUpdatingHierarchy;
    private Dictionary<Type, EventTypeFilterNodeModel>? _typeLookup;
    private HashSet<Type>? _visibleTypes;
    private readonly string? _displayName;
    public string Name => _displayName ?? EventType.Name;
    public int Count { get; private set; }
    public Type EventType { get; }
    public EventTypeFilterNodeModel? Parent { get; set; }
    public ObservableCollection<EventTypeFilterNodeModel> Children { get; } = [];
    public event EventHandler? FilterChanged;

    public EventTypeFilterNodeModel(Type type, int count, string? displayName = null)
    {
        EventType = type;
        Count = count;
        _displayName = displayName;
    }
    internal void SetTypeLookup(Dictionary<Type, EventTypeFilterNodeModel> lookup)
    {
        _typeLookup = lookup;
        UpdateVisibleTypes();
    }

    public static IReadOnlyList<EventTypeFilterNodeModel> BuildRoots(IReadOnlyList<TimeCombatEvent> timeEvents, IReadOnlyList<NonTimeCombatEvent> nonTimeEvents, IReadOnlyList<EXTHealingExtensionEvent> healingExtensionEvents)
    {
        var timeRoot = BuildCombatEvents(timeEvents, typeof(TimeCombatEvent), "Time Combat Events");
        var nonTimeRoot = BuildCombatEvents(nonTimeEvents, typeof(NonTimeCombatEvent), "Non Time Combat Events");
        var healingExtensionRoot = BuildCombatEvents(healingExtensionEvents, typeof(EXTHealingExtensionEvent), "Healing Extension Combat Events");

        return [timeRoot, nonTimeRoot, healingExtensionRoot];
    }

    private static EventTypeFilterNodeModel BuildCombatEvents(IReadOnlyList<CombatEvent> events, Type breakAtType, string title)
    {
        var nodes = events.GroupBy(e => e.GetType()).ToDictionary(g => g.Key, g => new EventTypeFilterNodeModel(g.Key, g.Count()));

        foreach (var node in nodes.Values.ToList())
        {
            foreach (var baseType in GetEventBaseTypes(node.EventType))
            {
                if (!nodes.ContainsKey(baseType))
                {
                    nodes.Add(baseType, new EventTypeFilterNodeModel(baseType, -1, baseType == breakAtType ? title : null));
                }
                if (baseType == breakAtType)
                {
                    break;
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
        EventTypeFilterNodeModel root;

        if (roots.Length == 1)
        {
            root = roots[0];
        }
        else
        {
            root = new EventTypeFilterNodeModel(breakAtType, -1, title);

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

    private EventTypeFilterNodeModel GetRoot()
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

    private static int CalculateParentCounts(EventTypeFilterNodeModel node)
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
