using System;
using System.Collections.Generic;
using GW2EIParserAvalonia.ViewModels;

namespace GW2EIParserAvalonia.InspectorContent;

public sealed class EventTypeFilterTree
{
    public IReadOnlyList<EventTypeFilterNode> Roots { get; }

    public IReadOnlyDictionary<Type, EventTypeFilterNode> ByType { get; }

    public EventTypeFilterTree(IReadOnlyList<EventTypeFilterNode> roots, IReadOnlyDictionary<Type, EventTypeFilterNode> byType)
    {
        Roots = roots;
        ByType = byType;
    }
}
