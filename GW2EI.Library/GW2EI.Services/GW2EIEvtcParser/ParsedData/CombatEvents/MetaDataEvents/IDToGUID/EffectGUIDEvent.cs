namespace GW2EIEvtcParser.ParsedData;

using static GW2EIEvtcParser.ArcDPSEnums;

public class EffectGUIDEvent : IDToGUIDEvent
{
    public int EffectType { get; } = -1;
    public long EffectID => ContentID;
    public float DefaultDuration { get; } = -1;
    internal static EffectGUIDEvent DummyEffectGUID = new();
    internal EffectGUIDEvent(CombatItem evtcItem) : base(evtcItem)
    {
        if (evtcItem.Build > ArcDPSBuilds.ExtraDataInGUIDEvents)
        {
            EffectType = evtcItem.SrcInstid;
            DefaultDuration = BitConverter.Int32BitsToSingle(evtcItem.BuffDmg);
        }
    }

    private EffectGUIDEvent() : base()
    {
    }
}

