using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class SuperKodanBrothers : Bjora
    {
        public SuperKodanBrothers(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerBuffApplyMechanic(Immobile, "Trapped", new MechanicPlotlySetting(Symbols.Circle,Colors.Blue), "Trapped","Trapped", "Trapped",2500),
                new EnemyBuffApplyMechanic(EnragedVC, "Enrage", new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "Enrage","Enrage", "Enrage",1 << 16),
            }
            );
            Extension = "supkodbros";
            Icon = "https://i.imgur.com/lNXXbnC.png";
            EncounterCategoryInformation.InSubCategoryOrder = 1;
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
                new DamageCastFinder(58174, 58174, InstantCastFinder.DefaultICD), // Vengeful Aura Claw
            };
        }
        protected override void SetSuccessByDeath(CombatData combatData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents, bool all)
        {
            SetSuccessByDeath(combatData, fightData, playerAgents, all, (int)ArcDPSEnums.TargetID.ClawOfTheFallen, (int)ArcDPSEnums.TargetID.VoiceOfTheFallen);
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, extensions);
            int voiceAndClawCount = 1;
            foreach (AbstractSingleActor target in Targets)
            {
                if (target.ID == (int)ArcDPSEnums.TargetID.VoiceAndClaw)
                {
                    target.OverrideName(target.Character + " " + voiceAndClawCount++);
                }
            }
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor voice = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.ClawOfTheFallen);
            AbstractSingleActor claw = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.VoiceOfTheFallen);
            if (voice == null || claw == null)
            {
                throw new MissingKeyActorsException("Claw or Voice not found");
            }
            phases[0].AddTarget(voice);
            phases[0].AddTarget(claw);
            long fightEnd = log.FightData.FightEnd;
            if (!requirePhases)
            {
                return phases;
            }
            //
            List<PhaseData> unmergedPhases = GetPhasesByInvul(log, 762, claw, false, true);
            for (int i = 0; i < unmergedPhases.Count; i++)
            {
                unmergedPhases[i].Name = "Phase " + (i + 1);
                unmergedPhases[i].AddTarget(claw);
                unmergedPhases[i].AddTarget(voice);
            }
            phases.AddRange(unmergedPhases);
            //
            int voiceAndClawCount = 0;
            foreach (AbstractSingleActor voiceAndClaw in Targets.Where(x => x.ID == (int)ArcDPSEnums.TargetID.VoiceAndClaw))
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
            var teleports = voice.GetCastEvents(log, 0, log.FightData.FightDuration).Where(x => x.SkillId == 58382).ToList();
            long tpCount = 0;
            long preTPPhaseStart = 0;
            foreach (AbstractCastEvent teleport in teleports)
            {
                long preTPPhaseEnd = Math.Min(teleport.Time, log.FightData.FightDuration);
                AbstractSingleActor voiceAndClaw = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.VoiceAndClaw && x.FirstAware >= preTPPhaseStart);
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
            AbstractBuffEvent enrage = log.CombatData.GetBuffData(SkillIDs.EnragedVC).FirstOrDefault(x => x is BuffApplyEvent);
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
                long finalPositionEnd = log.FightData.FightDuration;
                if (nextUnmergedPhase != null)
                {
                    finalStart = nextUnmergedPhase.Start + 1;
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
