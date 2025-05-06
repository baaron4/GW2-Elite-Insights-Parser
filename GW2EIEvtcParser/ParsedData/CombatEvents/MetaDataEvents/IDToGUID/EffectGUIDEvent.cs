namespace GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;

public class EffectGUIDEvent : IDToGUIDEvent
{
    public int EffectType { get; } = -1;
    public float DefaultDuration { get; } = -1;
    internal static EffectGUIDEvent DummyEffectGUID = new();
    internal EffectGUIDEvent(CombatItem evtcItem, EvtcVersionEvent evtcVersion) : base(evtcItem)
    {
        if (evtcVersion.Build > ArcDPSBuilds.ExtraDataInGUIDEvents)
        {
            EffectType = evtcItem.SrcInstid;
            DefaultDuration = BitConverter.Int32BitsToSingle(evtcItem.BuffDmg);
        }
    }

    internal EffectGUIDEvent() : base()
    {
    }
}

