namespace GW2EIEvtcParser.ParsedData;

public class EffectGUIDEvent : IDToGUIDEvent
{
    public int EffectType { get; } = -1;
    public float DefaultDuration { get; } = -1;
    internal static EffectGUIDEvent DummyEffectGUID = new();
    internal EffectGUIDEvent(CombatItem evtcItem, EvtcVersionEvent evtcVersion) : base(evtcItem)
    {
        if (evtcVersion.Build > ArcDPSEnums.ArcDPSBuilds.ExtraDataInGUIDEvents)
        {
            EffectType = evtcItem.SrcInstid;
            DefaultDuration = Convert.ToSingle(evtcItem.BuffDmg);
        }
    }

    internal EffectGUIDEvent() : base()
    {
    }
}

