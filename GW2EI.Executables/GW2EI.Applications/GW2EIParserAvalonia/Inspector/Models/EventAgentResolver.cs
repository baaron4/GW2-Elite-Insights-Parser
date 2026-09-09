using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIParserAvalonia.Models;

internal static class EventAgentResolver
{
    private static readonly HashSet<string> IgnoredPropertyNames =
    [
        "Master",
        "PositionAttachedAgentItem"
    ];

    public static IReadOnlySet<ulong> Resolve(object @event)
    {
        var result = new HashSet<ulong>();

        Visit(@event, result, new HashSet<object>(ReferenceEqualityComparer.Instance));

        return result;
    }

    private static void Visit(object? value, HashSet<ulong> result, HashSet<object> visited)
    {
        if (value == null)
        {
            return;
        }

        if (value is AgentItem agent)
        {
            result.Add(agent.Agent);
            return;
        }

        var type = value.GetType();

        if (type.IsPrimitive ||
            type.IsEnum ||
            type == typeof(string) ||
            type == typeof(decimal) ||
            type == typeof(DateTime) ||
            type == typeof(DateTimeOffset) ||
            type == typeof(TimeSpan))
        {
            return;
        }

        if (!type.IsValueType && !visited.Add(value))
        {
            return;
        }

        if (value is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                Visit(item, result, visited);
            }

            return;
        }

        foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanRead || property.GetIndexParameters().Length != 0 || IgnoredPropertyNames.Contains(property.Name))
            {
                continue;
            }

            try
            {
                Visit(property.GetValue(value), result, visited);
            }
            catch
            {

            }
        }
    }
}
