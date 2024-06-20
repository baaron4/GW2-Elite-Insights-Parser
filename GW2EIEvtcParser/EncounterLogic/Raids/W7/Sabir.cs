using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Sabir : TheKeyOfAhdashim
    {
        public Sabir(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
                new PlayerDstSkillMechanic(DireDrafts, "Dire Drafts", new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "B.Tornado", "Hit by big tornado", "Big Tornado Hit", 500).UsingChecker((de, log) => de.HasDowned || de.HasKilled),
                new PlayerDstSkillMechanic(UnbridledTempest, "Unbridled Tempest", new MechanicPlotlySetting(Symbols.Hexagon,Colors.Pink), "Shockwave", "Hit by Shockwave", "Shockwave Hit", 0).UsingChecker((de, log) => de.HasDowned || de.HasKilled),
                new PlayerDstSkillMechanic(FuryOfTheStorm, "Fury of the Storm", new MechanicPlotlySetting(Symbols.Circle,Colors.Purple), "Arena AoE", "Hit by Arena wide AoE", "Arena AoE hit", 0).UsingChecker( (de, log) => de.HasDowned || de.HasKilled ),
                new PlayerDstHitMechanic(new long [] { DynamicDeterrentNM, DynamicDeterrentCM }, "Dynamic Deterrent", new MechanicPlotlySetting(Symbols.YUpOpen,Colors.Pink), "Pushed", "Pushed by rotating breakbar", "Pushed", 0).UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ServerDelayConstant)),
                new PlayerDstHitMechanic(new long [] { StormsEdgeLeftHand, StormsEdgeRightHand }, "Storm's Edge", new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.Blue), "Storm's Edge", "Hit by Storm's Edge", "Storm's Edge", 0),
                new PlayerDstHitMechanic(ChainLightning, "Chain Lightning", new MechanicPlotlySetting(Symbols.HexagonOpen, Colors.White), "Chain Lightning", "Hit by Chain Lightning", "Chain Lightning Hit", 0),
                new PlayerDstHitMechanic(Electrospark, "Electrospark", new MechanicPlotlySetting(Symbols.CircleCross, Colors.Orange), "Electrospark", "Hit by Electrospark", "Electrospark", 0),
                new PlayerDstHitMechanic(Electrospark, "Charged Winds", new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.Orange), "Charged Winds", "Achievement Elegibility: Charged Winds", "Charged Winds", 0).UsingAchievementEligibility(true),
                new EnemyCastStartMechanic(RegenerativeBreakbar, "Regenerative Breakbar", new MechanicPlotlySetting(Symbols.DiamondWide,Colors.Magenta), "Reg.Breakbar","Regenerating Breakbar", "Regenerative Breakbar", 0),
                new EnemyCastStartMechanic(new long [] { DynamicDeterrentNM, DynamicDeterrentCM }, "Dynamic Deterrent", new MechanicPlotlySetting(Symbols.Star, Colors.Yellow), "Dynamic Deterrent", "Casted Dynamic Deterrent", "Cast Dynamic Deterrent", 0),
                new EnemyDstBuffRemoveMechanic(IonShield, "Regenerative Breakbar Broken", new MechanicPlotlySetting(Symbols.DiamondWide,Colors.DarkTeal), "Reg.Breakbar Brkn", "Regenerative Breakbar Broken", "Regenerative Breakbar Broken", 2000),
                new EnemyDstBuffApplyMechanic(RepulsionField, "Rotating Breakbar", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Magenta), "Rot.Breakbar","Rotating Breakbar", "Rotating Breakbar", 0),
                new EnemyDstBuffRemoveMechanic(RepulsionField, "Rotating Breakbar Broken", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "Rot.Breakbar Brkn","Rotating Breakbar Broken", "Rotating Breakbar Broken", 0),
            });
            // rotating cc 56403
            Extension = "sabir";
            Icon = EncounterIconSabir;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000002;
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>()
            {
                ArcDPSEnums.TrashID.ParalyzingWisp,
                ArcDPSEnums.TrashID.VoltaicWisp,
                ArcDPSEnums.TrashID.SmallKillerTornado,
                ArcDPSEnums.TrashID.SmallJumpyTornado,
                ArcDPSEnums.TrashID.BigKillerTornado
            };
        }

        internal override FightLogic AdjustLogic(AgentData agentData, List<CombatItem> combatData)
        {
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogNPCUpdate);
            // Handle potentially wrongly associated logs
            if (logStartNPCUpdate != null)
            {
                if (agentData.GetNPCsByID(ArcDPSEnums.TargetID.Adina).Any(adina => combatData.Any(evt => evt.IsDamagingDamage() && evt.DstMatchesAgent(adina) && agentData.GetAgent(evt.SrcAgent, evt.Time).GetFinalMaster().IsPlayer)))
                {
                    return new Adina((int)ArcDPSEnums.TargetID.Adina);
                }
            }
            return base.AdjustLogic(agentData, combatData);
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(BoltBreakSabir, BoltBreakSabir),
                new EffectCastFinder(FlashDischargeSAK, EffectGUIDs.SabirFlashDischarge)
                    .UsingChecker((effect, combatData, agentData, skillData) =>
                    {
                        BuffRemoveAllEvent buffRemove = combatData.GetBuffRemoveAllData(ViolentCurrents)
                            .Where(x => Math.Abs(effect.Time - x.Time) < ServerDelayConstant && x.To == effect.Src)
                            .FirstOrDefault();
                        return buffRemove != null;
                    }),
            };
        }

        internal override List<AbstractHealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, SkillData skillData)
        {
            NegateDamageAgainstBarrier(combatData, Targets.Select(x => x.AgentItem).ToList());
            return new List<AbstractHealthDamageEvent>();
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Sabir)) ?? throw new MissingKeyActorsException("Sabir not found");
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            IReadOnlyList<AbstractCastEvent> cls = mainTarget.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            var wallopingWinds = cls.Where(x => x.SkillId == WallopingWind).ToList();
            long start = 0, end = 0;
            for (int i = 0; i < wallopingWinds.Count; i++)
            {
                AbstractCastEvent wallopingWind = wallopingWinds[i];
                end = wallopingWind.Time;
                var phase = new PhaseData(start, end, "Phase " + (i + 1));
                phase.AddTarget(mainTarget);
                phases.Add(phase);
                AbstractCastEvent nextAttack = cls.FirstOrDefault(x => x.Time >= wallopingWind.EndTime && (x.SkillId == StormsEdgeRightHand || x.SkillId == StormsEdgeLeftHand || x.SkillId == ChainLightning));
                if (nextAttack == null)
                {
                    break;
                }
                start = nextAttack.Time;
                if (i == wallopingWinds.Count - 1)
                {
                    phase = new PhaseData(start, log.FightData.FightEnd, "Phase " + (i + 2));
                    phase.AddTarget(mainTarget);
                    phases.Add(phase);
                }
            }

            return phases;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplaySabir,
                            (1000, 910),
                            (-14122, 142, -9199, 4640)/*,
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970)*/);
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
            var boltBreaks = p.GetBuffStatus(log, BoltBreak, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            uint boltBreakRadius = 180;
            foreach (Segment seg in boltBreaks)
            {
                var circle = new CircleDecoration(boltBreakRadius, seg, "rgba(255, 150, 0, 0.3)", new AgentConnector(p));
                replay.AddDecorationWithGrowing(circle, seg.End);
            }
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            int crStart = (int)replay.TimeOffsets.start;
            int crEnd = (int)replay.TimeOffsets.end;
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Sabir:
                    var repulsionFields = target.GetBuffStatus(log, RepulsionField, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
                    foreach (Segment seg in repulsionFields)
                    {
                        replay.Decorations.Add(new CircleDecoration(120, seg, "rgba(80, 0, 255, 0.3)", new AgentConnector(target)));
                    }
                    var ionShields = target.GetBuffStatus(log, IonShield, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
                    foreach (Segment seg in ionShields)
                    {
                        replay.Decorations.Add(new CircleDecoration(120, seg, "rgba(0, 80, 255, 0.3)", new AgentConnector(target)));
                    }
                    //
                    var furyOfTheStorm = cls.Where(x => x.SkillId == FuryOfTheStorm).ToList();
                    foreach (AbstractCastEvent c in furyOfTheStorm)
                    {
                        replay.Decorations.Add(new CircleDecoration(1200, ((int)c.Time, (int)c.EndTime), Colors.LightBlue, 0.3, new AgentConnector(target)).UsingGrowingEnd(c.EndTime));
                    }
                    //
                    var unbridledTempest = cls.Where(x => x.SkillId == UnbridledTempest).ToList();
                    foreach (AbstractCastEvent c in unbridledTempest)
                    {
                        int start = (int)c.Time;
                        int delay = 3000; // casttime 0 from skill def
                        int duration = 5000;
                        uint radius = 1200;
                        (long, long) lifespanShockwave = (start + delay, start + duration);
                        GeographicalConnector connector = new AgentConnector(target);
                        replay.Decorations.Add(new CircleDecoration(radius, (start, start + delay), Colors.Orange, 0.2, connector));
                        replay.Decorations.Add(new CircleDecoration(radius, (start + delay - 10, start + delay + 100), Colors.Orange, 0.5, connector));
                        replay.AddShockwave(connector, lifespanShockwave, Colors.Grey, 0.7, radius);
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.BigKillerTornado:
                    replay.Decorations.Add(new CircleDecoration(480, (crStart, crEnd), Colors.LightOrange, 0.4, new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.SmallKillerTornado:
                    replay.Decorations.Add(new CircleDecoration(120, (crStart, crEnd), Colors.LightOrange, 0.4, new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.SmallJumpyTornado:
                case (int)ArcDPSEnums.TrashID.ParalyzingWisp:
                case (int)ArcDPSEnums.TrashID.VoltaicWisp:
                    break;
                default:
                    break;

            }
        }
        internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            // Find target
            if (!agentData.TryGetFirstAgentItem(ArcDPSEnums.TargetID.Sabir, out AgentItem sabir))
            {
                throw new MissingKeyActorsException("Sabir not found");
            }
            CombatItem enterCombat = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.EnterCombat && x.SrcMatchesAgent(sabir));
            if (enterCombat == null)
            {
                CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogNPCUpdate);
                if (logStartNPCUpdate == null)
                {
                    return GetGenericFightOffset(fightData);
                }
                else
                {
                    CombatItem firstDamageEvent = combatData.FirstOrDefault(x => x.DstMatchesAgent(sabir) && x.IsDamagingDamage());
                    if (firstDamageEvent != null)
                    {
                        return firstDamageEvent.Time;
                    }
                    else
                    {
                        return fightData.LogEnd;
                    }
                }
            }
            return enterCombat.Time;
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Sabir)) ?? throw new MissingKeyActorsException("Sabir not found");
            return (target.GetHealth(combatData) > 32e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }
    }
}
