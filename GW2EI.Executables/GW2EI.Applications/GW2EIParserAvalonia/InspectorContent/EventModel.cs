using GW2EIEvtcParser.ParsedData;

namespace GW2EIParserAvalonia.InspectorContent;

public sealed class EventModel
{
    public object Event { get; }
    public long? Time => Event is TimeCombatEvent timeEvent ? timeEvent.Time : null;
    public string Type => Event.GetType().Name;

    public EventModel(object @event)
    {
        Event = @event;
    }
}
