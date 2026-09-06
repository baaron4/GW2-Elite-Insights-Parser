using GW2EIEvtcParser.ParsedData;

namespace GW2EIParserAvalonia.InspectorContent;

public sealed class EventModel
{
    public TimeCombatEvent Event { get; }
    public long Time => Event.Time;
    public string Type => Event.GetType().Name;

    public EventModel(TimeCombatEvent @event)
    {
        Event = @event;
    }
}
