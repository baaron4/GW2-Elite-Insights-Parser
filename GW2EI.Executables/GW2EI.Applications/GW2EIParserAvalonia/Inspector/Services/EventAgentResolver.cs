using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GW2EIEvtcParser.ParsedData;
using GW2EIParserAvalonia.Models;

namespace GW2EIParserAvalonia.Services;

internal static class EventAgentResolver
{
    internal readonly record struct EventAgentResolution(
        IReadOnlySet<ulong> All,
        IReadOnlySet<ulong> Source,
        IReadOnlySet<ulong> Destination);

    private static readonly HashSet<string> IgnoredPropertyNames =
    [
        "Master",
        "PositionAttachedAgentItem"
    ];

    public static EventAgentResolution ResolveDetailed(object @event)
    {
        var all = new HashSet<ulong>();
        var source = new HashSet<ulong>();
        var destination = new HashSet<ulong>();

        Visit(@event, all, source, destination, Endpoint.None, new HashSet<object>(ReferenceEqualityComparer.Instance));

        return new EventAgentResolution(all, source, destination);
    }

    private static void Visit(object? value, HashSet<ulong> all, HashSet<ulong> source, HashSet<ulong> destination, Endpoint endpoint, HashSet<object> visited)
    {
        if (value == null)
        {
            return;
        }

        if (value is AgentItem agent)
        {
            all.Add(agent.Agent);

            switch (endpoint)
            {
                case Endpoint.Source:
                    source.Add(agent.Agent);
                    break;
                case Endpoint.Destination:
                    destination.Add(agent.Agent);
                    break;
                case Endpoint.None:
                    break;
            }

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
                Visit(item, all, source, destination, endpoint, visited);
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
                var childEndpoint = endpoint;

                if (property.Name.Equals("Src", StringComparison.OrdinalIgnoreCase) ||
                    property.Name.Equals("From", StringComparison.OrdinalIgnoreCase) ||
                    property.Name.Equals("Caster", StringComparison.OrdinalIgnoreCase) ||
                    property.Name.Equals("By", StringComparison.OrdinalIgnoreCase) ||
                    property.Name.Equals("CreditedBy", StringComparison.OrdinalIgnoreCase) ||
                    property.Name.Equals("CreditedFrom", StringComparison.OrdinalIgnoreCase) ||
                    property.Name.Equals("PoV", StringComparison.OrdinalIgnoreCase) ||
                    property.Name.Equals("DamagingAgent", StringComparison.OrdinalIgnoreCase))
                {
                    childEndpoint = Endpoint.Source;
                }

                else if (property.Name.Equals("Dst", StringComparison.OrdinalIgnoreCase) ||
                    property.Name.Equals("To", StringComparison.OrdinalIgnoreCase) ||
                    property.Name.Equals("AttackTarget", StringComparison.OrdinalIgnoreCase) ||
                    property.Name.Equals("TargetedAgent", StringComparison.OrdinalIgnoreCase))
                {
                    childEndpoint = Endpoint.Destination;
                }

                Visit(property.GetValue(value), all, source, destination, childEndpoint, visited);
            }
            catch
            {

            }
        }
    }
}
