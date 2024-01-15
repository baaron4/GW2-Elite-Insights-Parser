using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class SuperKodanBrothers : Bjora
    {
        public SuperKodanBrothers(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(Groundshaker, "Groundshaker", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Grey), "Groundshaker.H", "Hit by Groundshaker", "Groundshaker Hit", 150),
                new PlayerDstHitMechanic(Groundpiercer, "Groundpiercer", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.White), "Groundpiercer.H", "Hit by Groundpiercer", "Groundpiercer Knockdown", 150),
                new PlayerDstBuffApplyMechanic(UnrelentingPainBuff, "Unrelenting Pain", new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.Pink), "UnrelPain.A", "Unreleting Pain Applied", "Unrelenting Pain Applied", 0),
                new PlayerDstBuffApplyMechanic(Immobile, "Trapped", new MechanicPlotlySetting(Symbols.Circle, Colors.Blue), "Trapped", "Trapped", "Trapped", 2500),
                new EnemyDstBuffApplyMechanic(EnragedVC, "Enrage", new MechanicPlotlySetting(Symbols.Circle, Colors.Orange), "Enrage", "Enrage", "Enrage", 1 << 16),
                new EnemyCastStartMechanic(DeadlySynergy, "Deadly Synergy", new MechanicPlotlySetting(Symbols.Diamond, Colors.Blue), "Deadly Synergy", "Cast  Deadly Synergy", "Deadly Synergy", 10000),
                new EnemyCastStartMechanic(KodanTeleport, "Teleport", new MechanicPlotlySetting(Symbols.Hexagon, Colors.LightBlue), "Teleport", "Cast Teleport", "Teleport", 150),
            }
            );
            Extension = "supkodbros";
            Icon = EncounterIconKodanBrothers;
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000003;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayKodanBrothers,
                            (905, 789),
                            (-1013, -1600, 2221, 1416)/*,
                            (-0, -0, 0, 0),
                            (0, 0, 0, 0)*/);
        }
        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(VengefulAuraClaw, VengefulAuraClaw),
            };
        }

        internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
        {
            if (TargetHPPercentUnderThreshold(ArcDPSEnums.TargetID.ClawOfTheFallen, fightData.FightStart, combatData, Targets) ||
                TargetHPPercentUnderThreshold(ArcDPSEnums.TargetID.VoiceOfTheFallen, fightData.FightStart, combatData, Targets))
            {
                return FightData.EncounterStartStatus.Late;
            }
            return FightData.EncounterStartStatus.Normal;
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            long startToUse = base.GetFightOffset(evtcVersion, fightData, agentData, combatData);
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogStartNPCUpdate);
            if (logStartNPCUpdate != null)
            {
                AgentItem mainTarget = agentData.GetNPCsByID(GenericTriggerID).FirstOrDefault();
                if (mainTarget == null)
                {
                    throw new MissingKeyActorsException("Main target not found");
                }
                CombatItem firstCast = combatData.FirstOrDefault(x => x.SrcMatchesAgent(mainTarget) && x.IsActivation != ArcDPSEnums.Activation.None && x.Time <= logStartNPCUpdate.Time && x.SkillID != WeaponStow && x.SkillID != WeaponDraw);
                if (firstCast != null && combatData.Any(x => x.SrcMatchesAgent(mainTarget) && x.Time > logStartNPCUpdate.Time + TimeThresholdConstant))
                {
                    startToUse = firstCast.Time;
                }
            }
            return startToUse;
        }

        internal override void EIEvtcParse(ulong gw2Build, int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
            int voiceAndClawCount = 1;
            foreach (AbstractSingleActor target in Targets)
            {
                if (target.IsSpecies(ArcDPSEnums.TargetID.VoiceAndClaw))
                {
                    target.OverrideName(target.Character + " " + voiceAndClawCount++);
                }
            }
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor voice = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.ClawOfTheFallen));
            AbstractSingleActor claw = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.VoiceOfTheFallen));
            if (voice == null || claw == null)
            {
                throw new MissingKeyActorsException("Claw or Voice not found");
            }
            phases[0].AddTarget(voice);
            phases[0].AddTarget(claw);
            phases[0].AddSecondaryTargets(Targets.Where(x => x.IsSpecies(ArcDPSEnums.TargetID.VoiceAndClaw)));
            long fightEnd = log.FightData.FightEnd;
            if (!requirePhases)
            {
                return phases;
            }
            //
            List<PhaseData> unmergedPhases = GetPhasesByInvul(log, Determined762, claw, false, true);
            for (int i = 0; i < unmergedPhases.Count; i++)
            {
                unmergedPhases[i].Name = "Phase " + (i + 1);
                unmergedPhases[i].AddTarget(claw);
                unmergedPhases[i].AddTarget(voice);
            }
            phases.AddRange(unmergedPhases);
            //
            int voiceAndClawCount = 0;
            foreach (AbstractSingleActor voiceAndClaw in Targets.Where(x => x.IsSpecies(ArcDPSEnums.TargetID.VoiceAndClaw)))
            {
                EnterCombatEvent enterCombat = log.CombatData.GetEnterCombatEvents(voiceAndClaw.AgentItem).FirstOrDefault();
                long phaseStart = 0;
                if (enterCombat != null)
                {
                    phaseStart = enterCombat.Time;
                } else
                {
                    phaseStart = voiceAndClaw.FirstAware;
                }
                PhaseData nextUnmergedPhase = unmergedPhases.FirstOrDefault(x => x.Start > phaseStart);
                long phaseEnd = Math.Min(fightEnd, voiceAndClaw.LastAware);
                if (nextUnmergedPhase != null)
                {
                    phaseEnd = nextUnmergedPhase.Start - 1;
                }
                var phase = new PhaseData(phaseStart, phaseEnd , "Voice and Claw " + ++voiceAndClawCount);
                phase.AddTarget(voiceAndClaw);
                phases.Add(phase);
            }
            //
            var teleports = voice.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.SkillId == KodanTeleport).ToList();
            long tpCount = 0;
            long preTPPhaseStart = 0;
            foreach (AbstractCastEvent teleport in teleports)
            {
                long preTPPhaseEnd = Math.Min(teleport.Time, log.FightData.FightEnd);
                AbstractSingleActor voiceAndClaw = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.VoiceAndClaw) && x.FirstAware >= preTPPhaseStart);
                if (voiceAndClaw != null)
                {
                    long oldEnd = preTPPhaseEnd;
                    preTPPhaseEnd = Math.Min(preTPPhaseEnd, voiceAndClaw.FirstAware);
                    // To handle position phase after merge phase end
                    if (oldEnd != preTPPhaseEnd)
                    {
                        PhaseData nextUnmergedPhase = unmergedPhases.FirstOrDefault(x => x.Start > voiceAndClaw.LastAware);
                        if (nextUnmergedPhase != null) 
                        {
                            long postMergedStart = nextUnmergedPhase.Start + 1;
                            long postMergedEnd = oldEnd;
                            var phase = new PhaseData(postMergedStart, postMergedEnd, "Position " + (++tpCount));
                            phase.AddTarget(claw);
                            phase.AddTarget(voice);
                            phases.Add(phase);
                        }
                        
                    }
                }
                if (preTPPhaseEnd - preTPPhaseStart > 2000)
                {
                    var phase = new PhaseData(preTPPhaseStart, preTPPhaseEnd, "Position " + (++tpCount));
                    phase.AddTarget(claw);
                    phase.AddTarget(voice);
                    phases.Add(phase);
                }
                preTPPhaseStart = teleport.EndTime;
            }
            
            //
            AbstractBuffEvent enrage = log.CombatData.GetBuffData(EnragedVC).FirstOrDefault(x => x is BuffApplyEvent);
            if (enrage != null)
            {
                var phase = new PhaseData(enrage.Time, log.FightData.FightEnd, "Enrage");
                phase.AddTarget(claw.AgentItem == enrage.To ? claw : voice);
                phases.Add(phase);
            }
            // Missing final position event
            {
                PhaseData nextUnmergedPhase = unmergedPhases.FirstOrDefault(x => x.Start > preTPPhaseStart);
                long finalStart = preTPPhaseStart;
                long finalPositionEnd = log.FightData.FightEnd;
                if (nextUnmergedPhase != null)
                {
                    finalStart = nextUnmergedPhase.Start;
                }
                if (enrage != null)
                {
                    finalPositionEnd = enrage.Time;
                }
                if (finalPositionEnd - finalStart > 2000)
                {
                    var phase = new PhaseData(finalStart, finalPositionEnd, "Position " + (++tpCount));
                    phase.AddTarget(claw);
                    phase.AddTarget(voice);
                    phases.Add(phase);
                }
            }
            return phases;
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Super Kodan Brothers";
        }

        protected override List<int> GetSuccessCheckIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.ClawOfTheFallen,
                (int)ArcDPSEnums.TargetID.VoiceOfTheFallen,
            };
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.ClawOfTheFallen,
                (int)ArcDPSEnums.TargetID.VoiceOfTheFallen,
            };
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.VoiceOfTheFallen,
                (int)ArcDPSEnums.TargetID.ClawOfTheFallen,
                (int)ArcDPSEnums.TargetID.VoiceAndClaw,
            };
        }
    }
}
