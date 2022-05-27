using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Boneskinner : Bjora
    {
        public Boneskinner(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                // TODO find this
                //new HitOnPlayerMechanic(58811, "Grasp", new MechanicPlotlySetting(Symbols.Hexagram,Colors.Red), "Grasp","Grasp (hit by claw AoE)", "Grasp",4000),
                new HitOnPlayerMechanic(58851, "Charge", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "H.Charge","Hit by Charge", "Charge hit",0),
                new HitOnPlayerMechanic(58546, "Death Wind", new MechanicPlotlySetting(Symbols.Star,Colors.Red), "Launched","Launched by Death Wind", "Launched",0, (de, log) => !de.To.HasBuff(log, SkillIDs.Stability, de.Time - ParserHelper.ServerDelayConstant)),
                new EnemyCastEndMechanic(58851, "Charge", new MechanicPlotlySetting(Symbols.Hexagram,Colors.LightRed), "D.Torch","Destroyed a Torch", "Destroyed a Torch",0, (ce, log) => ce.Status != AbstractCastEvent.AnimationStatus.Interrupted),
                new EnemyCastEndMechanic(58546, "Death Wind", new MechanicPlotlySetting(Symbols.Square,Colors.Orange), "D.Wind","Death Wind (extinguished torches)", "Death Wind",0, (ce, log) => ce.Status != AbstractCastEvent.AnimationStatus.Interrupted),
                new EnemyCastEndMechanic(58809, "Douse in Darkness", new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "D.Darkness","Douse in Darkness (extinguished torches)", "Douse in Darkness",0, (ce, log) => ce.Status != AbstractCastEvent.AnimationStatus.Interrupted),
            }
            );
            Extension = "boneskin";
            Icon = "https://i.imgur.com/meYwQmA.png";
            EncounterCategoryInformation.InSubCategoryOrder = 2;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/kLjZ7eU.png",
                            (905, 789),
                            (-1013, -1600, 2221, 1416)/*,
                            (-0, -0, 0, 0),
                            (0, 0, 0, 0)*/);
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(58736, 58736, InstantCastFinder.DefaultICD), // Unnatural Aura
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.VigilTactician,
                ArcDPSEnums.TrashID.VigilRecruit,
                ArcDPSEnums.TrashID.PrioryExplorer,
                ArcDPSEnums.TrashID.PrioryScholar,
                ArcDPSEnums.TrashID.AberrantWisp,
            };
        }
    }
}
