using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using GW2EIEvtcParser.Extensions;
using static GW2EIEvtcParser.ArcDPSEnums;
using System.Security.Cryptography;
using GW2EIEvtcParser.ParserHelpers;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class LonelyTowerCerusAndDeimos : LonelyTower
    {
        public LonelyTowerCerusAndDeimos(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            });
            Extension = "cerdei";
            Icon = EncounterIconCerusAndDeimos;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000001;
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor cerus = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.CerusLonelyTower));
            if (cerus == null)
            {
                throw new MissingKeyActorsException("Cerus not found");
            }
            AbstractSingleActor deimos = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.DeimosLonelyTower));
            if (deimos == null)
            {
                throw new MissingKeyActorsException("Deimos not found");
            }
            if (cerus.GetHealth(combatData) < 5e6 || deimos.GetHealth(combatData) < 5e6)
            {
                return FightData.EncounterMode.Normal;
            }
            return FightData.EncounterMode.CM;
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Cerus and Deimos";
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            // TODO: verify this
            long startToUse = base.GetFightOffset(evtcVersion, fightData, agentData, combatData);
            if (evtcVersion >= ArcDPSBuilds.NewLogStart)
            {
                AgentItem cerus = agentData.GetNPCsByID(TargetID.CerusLonelyTower).FirstOrDefault();
                if (cerus == null)
                {
                    throw new MissingKeyActorsException("Cerus not found");
                }
                AgentItem deimos = agentData.GetNPCsByID(TargetID.DeimosLonelyTower).FirstOrDefault();
                if (deimos == null)
                {
                    throw new MissingKeyActorsException("Deimos not found");
                }
                CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogStartNPCUpdate);
                if (logStartNPCUpdate != null)
                {
                    startToUse = Math.Min(GetEnterCombatTime(fightData, agentData, combatData, logStartNPCUpdate.Time, (int)TargetID.CerusLonelyTower, logStartNPCUpdate.DstAgent),
                            GetEnterCombatTime(fightData, agentData, combatData, logStartNPCUpdate.Time, (int)TargetID.DeimosLonelyTower, logStartNPCUpdate.DstAgent));
                    return startToUse;
                }
                CombatItem initialDamageToPlayers = combatData.Where(x => x.IsDamagingDamage() && agentData.GetAgent(x.DstAgent, x.Time).IsPlayer && (
                      agentData.GetAgent(x.SrcAgent, x.Time) == cerus || agentData.GetAgent(x.SrcAgent, x.Time) == deimos)).FirstOrDefault();
                long initialDamageTimeToTargets = Math.Min(GetFirstDamageEventTime(fightData, agentData, combatData, cerus), GetFirstDamageEventTime(fightData, agentData, combatData, deimos));
                if (initialDamageToPlayers != null)
                {
                    return Math.Min(initialDamageToPlayers.Time, initialDamageTimeToTargets);
                }
                return initialDamageTimeToTargets;
                throw new MissingKeyActorsException("Cerus or Deimos not found");
            }
            return startToUse;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            AbstractSingleActor deimos = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.DeimosLonelyTower));
            if (deimos == null)
            {
                throw new MissingKeyActorsException("Deimos not found");
            }
            AbstractSingleActor cerus = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.CerusLonelyTower));
            if (cerus == null)
            {
                throw new MissingKeyActorsException("Cerus not found");
            }
            BuffApplyEvent determinedApplyCerus = combatData.GetBuffDataByIDByDst(Determined762, cerus.AgentItem).OfType<BuffApplyEvent>().LastOrDefault();
            BuffApplyEvent determinedApplyDeimos = combatData.GetBuffDataByIDByDst(Determined762, deimos.AgentItem).OfType<BuffApplyEvent>().LastOrDefault();
            if (determinedApplyCerus != null && determinedApplyDeimos != null)
            {
                fightData.SetSuccess(true, Math.Max(determinedApplyCerus.Time, determinedApplyDeimos.Time));
            }
        }

        internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
        {
            if (TargetHPPercentUnderThreshold(TargetID.CerusLonelyTower, fightData.FightStart, combatData, Targets))
            {
                return FightData.EncounterStartStatus.Late;
            }
            if (TargetHPPercentUnderThreshold(TargetID.DeimosLonelyTower, fightData.FightStart, combatData, Targets))
            {
                return FightData.EncounterStartStatus.Late;
            }
            return FightData.EncounterStartStatus.Normal;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor cerus = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.CerusLonelyTower));
            if (cerus == null)
            {
                throw new MissingKeyActorsException("Cerus not found");
            }
            AbstractSingleActor deimos = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.DeimosLonelyTower));
            if (deimos == null)
            {
                throw new MissingKeyActorsException("Deimos not found");
            }
            phases[0].AddTarget(cerus);
            phases[0].AddTarget(deimos);
            if (!requirePhases)
            {
                return phases;
            }
            //
            BuffApplyEvent determinedApplyCerus = log.CombatData.GetBuffDataByIDByDst(Determined762, cerus.AgentItem).OfType<BuffApplyEvent>().LastOrDefault();
            long cerusEnd = determinedApplyCerus != null ? determinedApplyCerus.Time : cerus.LastAware;
            var cerusPhase = new PhaseData(log.FightData.FightStart, cerusEnd, "Cerus")
            {
                CanBeSubPhase = false
            };
            cerusPhase.AddTarget(cerus);
            phases.Add(cerusPhase);
            //
            BuffApplyEvent determinedApplyDeimos = log.CombatData.GetBuffDataByIDByDst(Determined762, deimos.AgentItem).OfType<BuffApplyEvent>().LastOrDefault();
            long deimosEnd = determinedApplyDeimos != null ? determinedApplyDeimos.Time : deimos.LastAware;
            var deimosPhase = new PhaseData(log.FightData.FightStart, deimosEnd, "Deimos")
            {
                CanBeSubPhase = false
            };
            deimosPhase.AddTarget(deimos);
            phases.Add(deimosPhase);
            return phases;
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
            switch (target.ID)
            {
                case (int)TargetID.DeimosLonelyTower:
                    AbstractSingleActor cerus = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.CerusLonelyTower));
                    if (cerus != null)
                    {
                        var brothersDeimos = target.GetBuffStatus(log, BrothersUnited, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
                        foreach (Segment seg in brothersDeimos)
                        {
                            replay.Decorations.Add(new LineDecoration(seg, Colors.LightBlue, 0.5, new AgentConnector(target), new AgentConnector(cerus)));
                        }
                    }
                    break;
                case (int)TargetID.CerusLonelyTower:
                    AbstractSingleActor deimos = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.DeimosLonelyTower));
                    if (deimos != null)
                    {
                        var brothersCerus = target.GetBuffStatus(log, BrothersUnited, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
                        foreach (Segment seg in brothersCerus)
                        {
                            replay.Decorations.Add(new LineDecoration(seg, Colors.LightBlue, 0.5, new AgentConnector(target), new AgentConnector(deimos)));
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
            AbstractSingleActor cerus = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.CerusLonelyTower));
            if (cerus != null)
            {
                var fixadedCerus = p.GetBuffStatus(log, CerussFocus, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
                foreach (Segment seg in fixadedCerus)
                {
                    replay.Decorations.Add(new LineDecoration(seg, Colors.Orange, 0.3, new AgentConnector(p), new AgentConnector(cerus)));
                    replay.AddOverheadIcon(seg, p, ParserIcons.FixationPurpleOverhead);
                }
            }
            AbstractSingleActor deimos = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.DeimosLonelyTower));
            if (deimos != null)
            {
                var fixatedDeimos = p.GetBuffStatus(log, DeimossFocus, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
                foreach (Segment seg in fixatedDeimos)
                {
                    replay.Decorations.Add(new LineDecoration(seg, Colors.Red, 0.3, new AgentConnector(p), new AgentConnector(deimos)));
                    replay.AddOverheadIcon(seg, p, ParserIcons.FixationPurpleOverhead);
                }
            }
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)TargetID.CerusLonelyTower,
                (int)TargetID.DeimosLonelyTower,
            };
        }
        protected override Dictionary<int, int> GetTargetsSortIDs()
        {
            return new Dictionary<int, int>()
            {
                {(int)TargetID.CerusLonelyTower, 0 },
                {(int)TargetID.DeimosLonelyTower, 0 },
            };
        }
    }
}
