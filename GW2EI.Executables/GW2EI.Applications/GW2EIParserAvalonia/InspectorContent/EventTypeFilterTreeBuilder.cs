using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using GW2EIParserAvalonia.ViewModels;

namespace GW2EIParserAvalonia.InspectorContent;

public sealed class EventTypeFilterTreeBuilder
{
    public static EventTypeFilterTree Build(IReadOnlyList<TimeCombatEvent> events)
    {
        var byType = events.GroupBy(e => e.GetType()).ToDictionary(g => g.Key, g => new EventTypeFilterNode(g.Key, g.Count()));

        foreach (EventTypeFilterNode? node in byType.Values.ToList())
        {
            AddParents(node, byType);
        }

        CollapseSyntheticNodes(byType);

        var roots = byType.Values.Where(n => n.Parent == null).OrderBy(n => n.Name).ToList();

        return new EventTypeFilterTree(roots, byType);
    }

    private static void AddParents(EventTypeFilterNode node, Dictionary<Type, EventTypeFilterNode> byType)
    {
        EventTypeFilterNode child = node;
        Type? parentType = node.EventType.BaseType;

        while (parentType != null && typeof(TimeCombatEvent).IsAssignableFrom(parentType))
        {
            if (!byType.TryGetValue(parentType, out var parent))
            {
                parent = new EventTypeFilterNode(parentType, -1)
                {
                    IsExpanded = true
                };

                byType.Add(parentType, parent);
            }

            parent.Children.Add(child);
            child.Parent = parent;

            child = parent;
            parentType = parentType.BaseType;
        }
    }

    private static void CollapseSyntheticNodes(Dictionary<Type, EventTypeFilterNode> byType)
    {
        foreach (EventTypeFilterNode? root in byType.Values.Where(n => n.Parent == null).ToList())
        {
            CollapseSyntheticNodes(root, byType);
        }
    }

    private static void CollapseSyntheticNodes(EventTypeFilterNode node, Dictionary<Type, EventTypeFilterNode> byType)
    {
        foreach (EventTypeFilterNode? child in node.Children.ToList())
        {
            CollapseSyntheticNodes(child, byType);
        }

        if (!node.IsSynthetic || node.Children.Count != 1 || node.Parent == null)
        {
            return;
        }

        EventTypeFilterNode childNode = node.Children[0];
        EventTypeFilterNode parent = node.Parent;

        parent.Children.Remove(node);
        parent.Children.Add(childNode);
        childNode.Parent = parent;

        byType.Remove(node.EventType);
    }
}
