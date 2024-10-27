using System;

namespace GW2EIEvtcParser.ParsedData;

public class EffectGUIDEvent : IDToGUIDEvent
{
    public int EffectType { get; } = -1;
    public float LastDuration { get; } = -1;
    internal static EffectGUIDEvent DummyEffectGUID = new();
    internal EffectGUIDEvent(CombatItem evtcItem, EvtcVersionEvent evtcVersion) : base(evtcItem)
    {
        if (evtcVersion.Build > ArcDPSEnums.ArcDPSBuilds.ExtraDataInGUIDEvents)
        {
            EffectType = evtcItem.SrcInstid;
            LastDuration = Convert.ToSingle(evtcItem.BuffDmg);
        }
    }

    internal EffectGUIDEvent() : base()
    {
    }
}

