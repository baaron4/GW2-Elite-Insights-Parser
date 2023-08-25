using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal class ActorOrientationDecoration : GenericAttachedDecoration
    {
        public List<float> Angles { get; } = new List<float>();

        public ActorOrientationDecoration((int start, int end) lifespan, AgentConnector connector, IReadOnlyList<ParametricPoint3D> facings) : base(lifespan, connector)
        {
            foreach (ParametricPoint3D facing in facings)
            {
                if (facing.Time >= lifespan.start && facing.Time <= lifespan.end)
                {
                    Angles.Add(-Point3D.GetRotationFromFacing(facing));
                }
            }
        }

        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new ActorOrientationDecorationCombatReplayDescription(log, this, map);
        }

        public override GenericAttachedDecoration UsingSkillMode(AbstractSingleActor owner, ParserHelper.Spec Spec, long skillID = 0, SkillModeCategory category = SkillModeCategory.NotApplicable)
        {
            return this;
        }
    }
}
