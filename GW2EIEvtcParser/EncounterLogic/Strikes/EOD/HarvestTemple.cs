using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class HarvestTemple : CanthaStrike
    {
        public HarvestTemple(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            }
            );
            Extension = "harvsttmpl";
            EncounterCategoryInformation.InSubCategoryOrder = 3;
        }

        /*protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/kLjZ7eU.png",
                            (905, 789),
                            (-1013, -1600, 2221, 1416));
        }*/

        /*internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(58736, 58736, InstantCastFinder.DefaultICD), // Unnatural Aura
            };
        }*/

        /*protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.VigilTactician,
                ArcDPSEnums.TrashID.VigilRecruit,
                ArcDPSEnums.TrashID.PrioryExplorer,
                ArcDPSEnums.TrashID.PrioryScholar,
                ArcDPSEnums.TrashID.AberrantWisp,
            };
        }*/
    }
}
