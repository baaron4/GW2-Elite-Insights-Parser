using GW2EIEvtcParser.ParsedData;

namespace GW2EIParserAvalonia.Models;

public sealed class GadgetAnimationModel
{
    public string Data => _event.AnimationToken.DataString;

    private readonly GadgetAnimationEvent _event;

    public GadgetAnimationModel(GadgetAnimationEvent @event)
    {
        _event = @event;
    }
}
