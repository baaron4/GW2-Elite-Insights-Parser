namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericAttachedDecorationCombatReplayDescription : GenericDecorationCombatReplayDescription
    {
        public object ConnectedTo { get; }

        public object Owner { get; }

        public bool DrawOnSelect { get; }

        internal GenericAttachedDecorationCombatReplayDescription(ParsedEvtcLog log, GenericAttachedDecoration decoration, CombatReplayMap map) : base(decoration)
        {
            ConnectedTo = decoration.ConnectedTo.GetConnectedTo(map, log);
            IsMechanicOrSkill = true;
            if (decoration.Owner != null)
            {
                Owner = decoration.Owner.GetConnectedTo(map, log);
                DrawOnSelect = decoration.DrawOnSelect;
            }
        }
    }
}
