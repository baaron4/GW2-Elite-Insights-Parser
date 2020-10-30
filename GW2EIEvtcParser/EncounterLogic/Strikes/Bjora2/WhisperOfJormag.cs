using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class WhisperOfJormag : StrikeMissionLogic
    {
        public WhisperOfJormag(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new HitOnPlayerMechanic(59159, "Chains of Frost Hit", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "H.Chains","Hit by Chains of Frost", "Chains of Frost",50),
                new HitOnPlayerMechanic(59441, "Lethal Coalescence Soaked", new MechanicPlotlySetting("hexagram","rgb(255,0,0)"), "S.Lethal.Coal.","Soaked Lethal Coalescence Damage", "Soaked Lethal Coalescence",50),
                new HitOnPlayerMechanic(59102, "Spreading Ice (Own)", new MechanicPlotlySetting("circle","rgb(255,125,0)"), "S.Ice","Hit by own spreading ice", "Spreading Ice (Own)",50),
                new HitOnPlayerMechanic(59468, "Spreading Ice (Others)", new MechanicPlotlySetting("triangle","rgb(255,150,0)"), "S.Ice.O","Hit by other's spreading ice", "Spreading Ice (Others)",50),
                new HitOnPlayerMechanic(59076, "Icy Slice", new MechanicPlotlySetting("hexagram","rgb(255,100,0)"), "I.Slice","Hit by Icy Slice", "Icy Slice",50),
                new HitOnPlayerMechanic(59255, "Ice Tempest", new MechanicPlotlySetting("square","rgb(255,100,0)"), "I.Tornado","Hit by Ice Tornadoes", "Ice Tempest",50),
                new PlayerBuffApplyMechanic(59120, "Chains of Frost", new MechanicPlotlySetting("circle","rgb(0,0,255)"), "F.Chains","Selected for Chains of Frost", "Chains of Frost",500),
                new PlayerBuffRemoveMechanic(59054, "Teleport Back", new MechanicPlotlySetting("circle","rgb(0,125,255)"), "TP In","Teleported back to the arena", "Teleport Back",500),
                new PlayerBuffRemoveMechanic(59223, "Teleport Out", new MechanicPlotlySetting("circle-open","rgb(0,125,255)"), "TP Out","Teleported outside of the arena", "Teleport Out",500),
                new EnemyCastStartMechanic(59102, "Spreading Ice", new MechanicPlotlySetting("hexagram","rgb(255,0,125)"), "S.Ice.C","Cast Spreading Ice", "Cast Spreading Ice",0),
                new EnemyCastStartMechanic(59159, "Chains of Frost", new MechanicPlotlySetting("hexagram","rgb(255,125,125)"), "F.Chains.C","Cast Chains of Frost", "Cast Chains of Frost",0),
            }
            );
            Extension = "woj";
            Icon = "https://i.imgur.com/8GLwgfL.png";
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC woj = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.WhisperOfJormag);
            if (woj == null)
            {
                throw new InvalidOperationException("Whisper of Jormag not found");
            }
            phases[0].AddTarget(woj);
            if (!requirePhases)
            {
                return phases;
            }
            long start, end;
            var tpOutEvents = log.CombatData.GetBuffData(59223).Where(x => x is BuffRemoveAllEvent).ToList();
            var tpBackEvents = log.CombatData.GetBuffData(59054).Where(x => x is BuffRemoveAllEvent).ToList();
            // 75% tp happened
            if (tpOutEvents.Count > 0)
            {
                end = tpOutEvents.Min(x => x.Time);
                phases.Add(new PhaseData(0, end, "Pre Doppelganger 1"));
                // remove everything related to 75% tp out
                tpOutEvents.RemoveAll(x => x.Time <= end + 1000);
            }
            // 75% tp finished
            if (tpBackEvents.Count > 0)
            {
                start = tpBackEvents.Min(x => x.Time);
                // 25% tp happened
                if (tpOutEvents.Count > 0)
                {
                    end = tpOutEvents.Min(x => x.Time);
                    tpOutEvents.Clear();
                    tpBackEvents.RemoveAll(x => x.Time <= end);
                } 
                // 25% tp did not happen
                else
                {
                    end = log.FightData.FightEnd;
                    tpBackEvents.Clear();
                }
                phases.Add(new PhaseData(start, end, "Pre Doppelganger 2"));
                // 25% tp finished
                if (tpBackEvents.Count > 0)
                {
                    start = tpBackEvents.Min(x => x.Time);
                    phases.Add(new PhaseData(start, log.FightData.FightEnd, "Final"));
                }
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].AddTarget(woj);
            }
            return phases;
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.WhisperEcho,
                ArcDPSEnums.TrashID.DoppelgangerGuardian1,
                ArcDPSEnums.TrashID.DoppelgangerGuardian2,
                ArcDPSEnums.TrashID.DoppelgangerNecro,
                ArcDPSEnums.TrashID.DoppelgangerRevenant,
                ArcDPSEnums.TrashID.DoppelgangerThief1,
                ArcDPSEnums.TrashID.DoppelgangerThief2,
                ArcDPSEnums.TrashID.DoppelgangerWarrior,
            };
        }
    }
}
