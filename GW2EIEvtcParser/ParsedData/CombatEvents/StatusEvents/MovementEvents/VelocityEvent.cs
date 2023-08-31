using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class VelocityEvent : AbstractMovementEvent
    {

        internal VelocityEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
        }

        internal override void AddPoint3D(CombatReplay replay)
        {
            replay.Velocities.Add(GetParametricPoint3D());
        }
    }
}
