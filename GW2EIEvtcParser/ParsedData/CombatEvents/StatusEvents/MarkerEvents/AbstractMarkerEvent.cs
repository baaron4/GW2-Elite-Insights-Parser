using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractMarkerEvent : AbstractStatusEvent
    {
        public Point3D Position { get; protected set; } = new Point3D(0, 0, 0);

        public bool IsAroundSrc => Src != null;

        internal AbstractMarkerEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
        }

    }
}
