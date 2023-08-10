namespace GW2EIEvtcParser.EIData
{
    public class IconOverheadDecorationCombatReplayDescription : IconDecorationCombatReplayDescription
    {

        internal IconOverheadDecorationCombatReplayDescription(ParsedEvtcLog log, IconDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "IconOverheadDecoration";
            IsMechanicOrSkill = false;
        }
    }

}
