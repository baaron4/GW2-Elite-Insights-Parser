using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIParserAvalonia.InspectorContent;

public sealed class ContentGUIDModel
{
    public long ContentID => _event.ContentID;
    public GUID GUID => _event.GUID;
    public float DefaultDuration => _event is EffectGUIDEvent effect ? effect.DefaultDuration : 0;
    public bool IsCommanderTag => _event is MarkerGUIDEvent marker && marker.IsCommanderTag;

    private readonly IDToGUIDEvent _event;

    public ContentGUIDModel(IDToGUIDEvent @event)
    {
        _event = @event;
    }
}
