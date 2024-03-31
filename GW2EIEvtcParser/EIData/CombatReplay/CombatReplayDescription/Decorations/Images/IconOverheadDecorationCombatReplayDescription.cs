namespace GW2EIEvtcParser.EIData
{
    public class IconOverheadDecorationCombatReplayDescription : IconDecorationCombatReplayDescription
    {

        internal IconOverheadDecorationCombatReplayDescription(ParsedEvtcLog log, IconOverheadDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "IconOverheadDecoration";
            if (decoration.IsSquadMarker)
            {
                Type = "OverheadSquadMarkerDecoration";
            }
        }
    }

}
