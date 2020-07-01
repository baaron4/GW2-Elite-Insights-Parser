using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Logic
{
    public class ColdWar : StrikeMissionLogic
    {
        public ColdWar(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new HitOnPlayerMechanic(60354, "Icy Echoes", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "Icy.Ech","Tight stacking damage", "Icy Echoes",0),
                new HitOnPlayerMechanic(60006, "Detonate", new MechanicPlotlySetting("circle","rgb(255,125,0)"), "Det.","Hit by Detonation", "Detonate",50),
                new HitOnPlayerMechanic(60545, "Lethal Coalescence", new MechanicPlotlySetting("hexagram","rgb(255,100,0)"), "Leth.Coal.","Soaked damage", "Lethal Coalescence",50),
                new HitOnPlayerMechanic(60171, "Flame Wall", new MechanicPlotlySetting("square","rgb(255,125,0)"), "Flm.Wall","Stood in Flame Wall", "Flame Wall",50),
                new HitOnPlayerMechanic(60308, "Call Assassins", new MechanicPlotlySetting("diamond-tall","rgb(255,0,125)"), "Call Ass.","Hit by Assassins", "Call Assassins",50),
                new HitOnPlayerMechanic(60132, "Charge!", new MechanicPlotlySetting("diamond-tall","rgb(255,125,0)"), "Charge!","Hit by Charge", "Charge!",50),
            }
            );
            Extension = "coldwar";
            Icon = "https://i.imgur.com/r9b2oww.png";
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC varinia = Targets.Find(x => x.ID == (int)ParseEnum.TargetIDS.VariniaStormsounder);
            if (varinia == null)
            {
                throw new InvalidOperationException("Varinia Stormsounder not found");
            }
            phases[0].Targets.Add(varinia);
            //
            // TODO - add phases if applicable
            //
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Targets.Add(varinia);
            }
            return phases;
        }

        // TODO - complete IDs
        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                ParseEnum.TrashIDS.PropagandaBallon,
                ParseEnum.TrashIDS.DominionBladestorm,
                ParseEnum.TrashIDS.DominionStalker,
                ParseEnum.TrashIDS.DominionSpy1,
                ParseEnum.TrashIDS.DominionSpy2,
                ParseEnum.TrashIDS.DominionAxeFiend,
                ParseEnum.TrashIDS.DominionEffigy,
                ParseEnum.TrashIDS.FrostLegionCrusher,
                ParseEnum.TrashIDS.FrostLegionMusketeer,
                ParseEnum.TrashIDS.BloodLegionBlademaster,
                ParseEnum.TrashIDS.CharrTank,
                ParseEnum.TrashIDS.SonsOfSvanirHighShaman,
            };
        }
    }
}
