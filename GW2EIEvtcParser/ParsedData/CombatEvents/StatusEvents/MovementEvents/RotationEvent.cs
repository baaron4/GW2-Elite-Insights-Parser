using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class RotationEvent : AbstractMovementEvent
    {

        internal RotationEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
        }

        internal override void AddPoint3D(CombatReplay replay)
        {
            replay.Rotations.Add(GetParametricPoint3D());
        }
    }
}
