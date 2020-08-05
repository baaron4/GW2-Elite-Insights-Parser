using System;
using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using GW2EIUtils;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class ColdWar : StrikeMissionLogic
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

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC varinia = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.VariniaStormsounder);
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
        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.PropagandaBallon,
                ArcDPSEnums.TrashID.DominionBladestorm,
                ArcDPSEnums.TrashID.DominionStalker,
                ArcDPSEnums.TrashID.DominionSpy1,
                ArcDPSEnums.TrashID.DominionSpy2,
                ArcDPSEnums.TrashID.DominionAxeFiend,
                ArcDPSEnums.TrashID.DominionEffigy,
                ArcDPSEnums.TrashID.FrostLegionCrusher,
                ArcDPSEnums.TrashID.FrostLegionMusketeer,
                ArcDPSEnums.TrashID.BloodLegionBlademaster,
                ArcDPSEnums.TrashID.CharrTank,
                ArcDPSEnums.TrashID.SonsOfSvanirHighShaman,
            };
        }
    }
}
