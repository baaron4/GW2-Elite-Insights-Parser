using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class IconOverheadDecoration : IconDecoration
    {

        public IconOverheadDecoration(string icon, int pixelSize, float opacity, (int start, int end) lifespan, AgentConnector connector) : base(icon, pixelSize, (int)connector.Agent.HitboxWidth, opacity, lifespan, connector)
        {
        }

        public IconOverheadDecoration(string icon, int pixelSize, float opacity, Segment lifespan, AgentConnector connector) : base(icon, pixelSize, (int)connector.Agent.HitboxWidth, opacity, lifespan, connector)
        {
        }

        public override GenericAttachedDecoration UsingSkillMode(AbstractSingleActor owner, bool drawOnSelect = true)
        {
            return this;
        }

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new IconOverheadDecorationCombatReplayDescription(log, this, map);
        }
    }
}
