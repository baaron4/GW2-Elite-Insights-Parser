using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class WhisperOfJormag : Bjora
    {
        public WhisperOfJormag(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(ChainsOfFrostHit, "Chains of Frost", new MechanicPlotlySetting(Symbols.DiamondTall, Colors.Red), "H.Chains", "Hit by Chains of Frost", "Chains of Frost", 0),
                new PlayerDstHitMechanic(SlitheringRime, "Slithering Rime", new MechanicPlotlySetting(Symbols.CircleX, Colors.Red), "SlitRime.H", "Hit by Slithering Rime (Orbs)", "Slithering Rime", 0),
                new PlayerDstHitMechanic(LethalCoalescenceSoaked, "Lethal Coalescence Soaked", new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "S.Lethal.Coal.", "Soaked Lethal Coalescence Damage", "Soaked Lethal Coalescence", 50),
                new PlayerDstHitMechanic(SpreadingIceOwn, "Spreading Ice (Own)", new MechanicPlotlySetting(Symbols.Circle, Colors.Orange), "S.Ice", "Hit by own Spreading Ice", "Spreading Ice (Own)", 50),
                new PlayerDstHitMechanic(SpreadingIceOthers, "Spreading Ice (Others)", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.LightOrange), "S.Ice.O", "Hit by other's Spreading Ice", "Spreading Ice (Others)", 50),
                new PlayerDstHitMechanic(IcySlice, "Icy Slice", new MechanicPlotlySetting(Symbols.Hexagram, Colors.Orange), "I.Slice", "Hit by Icy Slice", "Icy Slice", 50),
                new PlayerDstHitMechanic(IceTempest, "Ice Tempest", new MechanicPlotlySetting(Symbols.Square, Colors.Orange), "I.Tornado", "Hit by Ice Tempest (Tornadoes)", "Ice Tempest", 50),
                new PlayerDstHitMechanic(FrigidVortexDamage, "Frigid Vortex", new MechanicPlotlySetting(Symbols.Star, Colors.Pink), "FrigVor.H", "Hit by Frigid Vortex", "Frigid Vortex Hit", 50),
                new PlayerDstHitMechanic(new long[] { IceShatterWhisper4, IceShatterWhisper2, IceShatterWhisper1, IceShatterWhisper3 }, "Ice Shatter", new MechanicPlotlySetting(Symbols.Circle, Colors.Teal), "IceShatt.H", "Hit by Ice Shatter (Large AoEs)", "Ice Shatter", 150),
                new PlayerDstBuffApplyMechanic(ChainsOfFrostApplication, "Chains of Frost", new MechanicPlotlySetting(Symbols.DiamondTall, Colors.LightRed), "F.Chains", "Selected for Chains of Frost", "Chains of Frost", 500),
                new PlayerDstBuffApplyMechanic(LethalCoalescenceBuff, "Lethal Coalescence", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Green), "LethalCoa.A", "Selected for Lethal Coalescence (Green)", "Lethal Coalescence", 500),
                new PlayerDstBuffApplyMechanic(FrigidVortexBuff, "Frigid Vortex", new MechanicPlotlySetting(Symbols.Star, Colors.LightBlue), "FrigVor.A", "Frigid Vortex Applied", "Frigid Vortex Buff", 0),
                new PlayerDstBuffRemoveMechanic(WhisperTeleportBack, "Teleport Back", new MechanicPlotlySetting(Symbols.Circle, Colors.LightBlue), "TP In", "Teleported back to the arena", "Teleport Back", 500),
                new PlayerDstBuffRemoveMechanic(WhisperTeleportOut, "Teleport Out", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.LightBlue), "TP Out", "Teleported outside of the arena", "Teleport Out", 500),
                new EnemyCastStartMechanic(SpreadingIceOwn, "Spreading Ice", new MechanicPlotlySetting(Symbols.Hexagram, Colors.DarkRed), "S.Ice.C", "Cast Spreading Ice", "Cast Spreading Ice", 0),
                new EnemyCastStartMechanic(ChainsOfFrostHit, "Chains of Frost", new MechanicPlotlySetting(Symbols.Hexagram, Colors.LightRed), "F.Chains.C", "Cast Chains of Frost", "Cast Chains of Frost", 0),
                new EnemyCastStartMechanic(FrigidVortexSkill, "Frigid Vortex", new MechanicPlotlySetting(Symbols.Star, Colors.Magenta), "Frigid Vortex", "Cast Frigid Vortex", "Cast Frigid Vortex", 50),
                new EnemyCastStartMechanic(LethalCoalescenceSoaked, "Lethal Coalescence", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkGreen), "Lethal Coalescence", "Cast Lethal Coalescence", "Cast Lethal Coalescence", 50),
                new EnemyCastStartMechanic(new long[] { ViciousSlam1, ViciousSlam2 }, "Vicious Slam",  new MechanicPlotlySetting(Symbols.TriangleUp, Colors.White), "Vicious Slam", "Cast Vicious Slam (Launch)", "Vicious Slam (Launch)", 150),
            }
            );
            Extension = "woj";
            Icon = EncounterIconWhisperOfJormag;
            EncounterCategoryInformation.InSubCategoryOrder = 3;
            EncounterID |= 0x000005;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayWhisperOfJormag,
                            (1682, 1682),
                            (-3287, -1772, 3313, 4828)/*,
                            (-0, -0, 0, 0),
                            (0, 0, 0, 0)*/);
        }
        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(FrostbiteAuraWhisperOfJormag, FrostbiteAuraWhisperOfJormag),
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor woj = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.WhisperOfJormag)) ?? throw new MissingKeyActorsException("Whisper of Jormag not found");
            phases[0].AddTarget(woj);
            if (!requirePhases)
            {
                return phases;
            }
            long start, end;
            var tpOutEvents = log.CombatData.GetBuffRemoveAllData(WhisperTeleportOut).ToList();
            var tpBackEvents = log.CombatData.GetBuffRemoveAllData(WhisperTeleportBack).ToList();
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

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.WhisperEcho,
                ArcDPSEnums.TrashID.DoppelgangerElementalist,
                ArcDPSEnums.TrashID.DoppelgangerElementalist2,
                ArcDPSEnums.TrashID.DoppelgangerEngineer,
                ArcDPSEnums.TrashID.DoppelgangerEngineer2,
                ArcDPSEnums.TrashID.DoppelgangerGuardian,
                ArcDPSEnums.TrashID.DoppelgangerGuardian2,
                ArcDPSEnums.TrashID.DoppelgangerMesmer,
                ArcDPSEnums.TrashID.DoppelgangerMesmer2,
                ArcDPSEnums.TrashID.DoppelgangerNecromancer,
                ArcDPSEnums.TrashID.DoppelgangerNecromancer2,
                ArcDPSEnums.TrashID.DoppelgangerRanger,
                ArcDPSEnums.TrashID.DoppelgangerRanger2,
                ArcDPSEnums.TrashID.DoppelgangerRevenant,
                ArcDPSEnums.TrashID.DoppelgangerRevenant2,
                ArcDPSEnums.TrashID.DoppelgangerThief,
                ArcDPSEnums.TrashID.DoppelgangerThief2,
                ArcDPSEnums.TrashID.DoppelgangerWarrior,
                ArcDPSEnums.TrashID.DoppelgangerWarrior2,
            };
        }
    }
}
