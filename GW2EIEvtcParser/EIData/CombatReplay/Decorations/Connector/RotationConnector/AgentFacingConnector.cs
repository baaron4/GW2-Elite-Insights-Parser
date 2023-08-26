using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class AgentFacingConnector : RotationConnector
    {
        public AgentItem Agent { get; }

        public float RotationOffset { get; } = 0;

        public enum RotationOffsetMode
        {
            AddToMaster = 0,
            AbsoluteOrientation = 1,
            RotateAfterTranslationOffset = 2,
        }

        public RotationOffsetMode OffsetMode { get; } = RotationOffsetMode.RotateAfterTranslationOffset;

        public AgentFacingConnector(AbstractSingleActor agent) : this(agent.AgentItem)
        {
        }

        public AgentFacingConnector(AgentItem agent)
        {
            Agent = agent;
        }

        public AgentFacingConnector(AbstractSingleActor agent, float rotationOffset, RotationOffsetMode rotationOffsetMode) : this(agent.AgentItem, rotationOffset, rotationOffsetMode)
        {

        }

        public AgentFacingConnector(AgentItem agent, float rotationOffset, RotationOffsetMode rotationOffsetMode) : this(agent)
        {
            RotationOffset = rotationOffset;
            OffsetMode = rotationOffsetMode;
        }

        public AgentFacingConnector(AbstractSingleActor agent, Point3D rotationOffset, RotationOffsetMode rotationOffsetMode) : this(agent, Point3D.GetRotationFromFacing(rotationOffset), rotationOffsetMode)
        {
        }

        public AgentFacingConnector(AgentItem agent, Point3D rotationOffset, RotationOffsetMode rotationOffsetMode) : this(agent, Point3D.GetRotationFromFacing(rotationOffset), rotationOffsetMode)
        {
        }
        public class AgentFacingConnectorDescriptor : RotationConnectorDescriptor
        {
            public int MasterId { get; private set; }
            public float RotationOffset { get; private set; }
            public int RotationOffsetMode { get; private set; }
            public AgentFacingConnectorDescriptor(AgentFacingConnector connector, CombatReplayMap map) : base(connector, map)
            {
                MasterId = connector.Agent.UniqueID;
                RotationOffset = connector.RotationOffset;
                RotationOffsetMode = (int)connector.OffsetMode;
            }
        }

        public override object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new AgentFacingConnectorDescriptor(this, map);
        }
    }
}
