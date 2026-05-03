using System.Numerics;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class GadgetInteractEvent : AnimatedCastEvent
{

    public AgentItem Gadget => EffectTarget;

    internal GadgetInteractEvent(CombatItem? startItem, AgentData agentData, SkillData skillData, 
        CombatItem? endItem, long maxEnd) : base(startItem, agentData, skillData, endItem, maxEnd)
    {
        if (startItem != null)
        {
            if (startItem.IsStateChange != StateChange.AnimationStart)
            {
                EffectTarget = agentData.GetAgentByInstID((ushort)startItem.Pad, startItem.Time);
            }
        }
        else
        {
            EffectTarget = ParserHelper._unknownAgent;
        }
        // Bandaid, may not be perfect
        if (AnimStop != AnimationStop.GadgetViaReset && AnimStop != AnimationStop.Ended && Status != AnimationStatus.Interrupted)
        {
            Status = AnimationStatus.Interrupted;
            SavedDuration = -ActualDuration;
        }
        ExpectedDuration = (int)(ExpectedDuration / AcceleratedToNonAcceleratedRatio);
        if (Status == AnimationStatus.Reduced)
        {
            int scaledExpectedDuration = (int)Math.Round(ExpectedDuration * AcceleratedToNonAcceleratedRatio);
            SavedDuration = Math.Max(scaledExpectedDuration - ActualDuration, 0);
        }
    }

}
