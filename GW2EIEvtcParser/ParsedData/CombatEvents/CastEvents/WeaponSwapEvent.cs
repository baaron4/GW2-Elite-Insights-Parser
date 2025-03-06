namespace GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;

public class WeaponSwapEvent : CastEvent
{
    // Swaps
    public int SwappedTo { get; protected set; }
    public int SwappedFrom { get; protected set; }

    internal WeaponSwapEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, EvtcVersionEvent evtcVersion) : base(evtcItem, agentData, skillData)
    {
        Status = AnimationStatus.Instant;
        SwappedTo = (int)evtcItem.DstAgent;
        SwappedFrom = -1;
        if (evtcVersion.Build >= ArcDPSBuilds.WeaponSwapValueIsPrevious_CrowdControlEvents_GliderEvents)
        {
            SwappedFrom = evtcItem.Value;
        }
        Skill = skillData.Get(SkillIDs.WeaponSwap);
        ActualDuration = 0;
        ExpectedDuration = 0;
    }
}
