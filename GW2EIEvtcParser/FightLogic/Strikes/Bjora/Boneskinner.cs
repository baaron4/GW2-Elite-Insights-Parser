using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIUtils;

namespace GW2EIEvtcParser.EncounterLogic
{
    public class Boneskinner : StrikeMissionLogic
    {
        public Boneskinner(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                // TODO find this
                //new HitOnPlayerMechanic(58811, "Grasp", new MechanicPlotlySetting("hexagram","rgb(255,0,0)"), "Grasp","Grasp (hit by claw AoE)", "Grasp",4000),
                new HitOnPlayerMechanic(58851, "Charge", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "H.Charge","Hit by Charge", "Charge hit",0),
                new HitOnPlayerMechanic(58546, "Death Wind", new MechanicPlotlySetting("star","rgb(255,0,0)"), "Launched","Launched by Death Wind", "Launched",0, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
                new EnemyCastEndMechanic(58851, "Charge", new MechanicPlotlySetting("hexagram","rgb(255,0,125)"), "D.Torch","Destroyed a Torch", "Destroyed a Torch",0, (ce, log) => ce.Status != AbstractCastEvent.AnimationStatus.Iterrupted),
                new EnemyCastEndMechanic(58546, "Death Wind", new MechanicPlotlySetting("square","rgb(255,125,0)"), "D.Wind","Death Wind (extinguished torches)", "Death Wind",0, (ce, log) => ce.Status != AbstractCastEvent.AnimationStatus.Iterrupted),
                new EnemyCastEndMechanic(58809, "Douse in Darkness", new MechanicPlotlySetting("circle","rgb(255,125,0)"), "D.Darkness","Douse in Darkness (extinguished torches)", "Douse in Darkness",0, (ce, log) => ce.Status != AbstractCastEvent.AnimationStatus.Iterrupted),
            }
            );
            Extension = "boneskin";
            Icon = "https://i.imgur.com/meYwQmA.png";
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/kLjZ7eU.png",
                            (905, 789),
                            (-1013, -1600, 2221, 1416),
                            (-0, -0, 0, 0),
                            (0, 0, 0, 0));
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
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
