
namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericAttachedDecorationCombatReplayDescription : GenericDecorationCombatReplayDescription
    {
        public object ConnectedTo { get; }
        public object RotationConnectedTo { get; }

        public object Owner { get; }

        public uint Category { get; }

        internal GenericAttachedDecorationCombatReplayDescription(ParsedEvtcLog log, GenericAttachedDecoration decoration, CombatReplayMap map) : base(decoration)
        {
            ConnectedTo = decoration.ConnectedTo.GetConnectedTo(map, log);
            RotationConnectedTo = decoration.RotationConnectedTo?.GetConnectedTo(map, log);
            IsMechanicOrSkill = true;
            if (decoration.SkillMode != null)
            {
                Category = (uint)decoration.SkillMode.Category;
                if (decoration.SkillMode.Owner != null)
                {
                    Owner = decoration.SkillMode.Owner.GetConnectedTo(map, log);
                }
            }
        }
    }
}
