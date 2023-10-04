
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
            if (decoration.Skill != null)
            {
                Owner = decoration.OwnerConnector.GetConnectedTo(map, log);
                Category = (uint)decoration.Skill.Category;
            }
        }
    }
}
