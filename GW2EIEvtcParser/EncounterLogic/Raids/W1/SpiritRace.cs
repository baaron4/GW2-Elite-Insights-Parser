using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class SpiritRace : SpiritVale
    {
        public SpiritRace(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            });
            Extension = "sprtrace";
            Icon = EncounterIconGeneric;
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000004;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.DummyTarget,
                (int)ArcDPSEnums.TargetID.EtherealBarrier,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.WallOfGhosts,
            };
        }

        internal override int GetTriggerID()
        {
            return (int)ArcDPSEnums.TrashID.WallOfGhosts;
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>();
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            RewardEvent reward = combatData.GetRewardEvents().FirstOrDefault(x => x.RewardType == ArcDPSEnums.RewardTypes.OldRaidReward2 && x.Time > fightData.FightStart);
            if (reward != null)
            {
                fightData.SetSuccess(true, reward.Time);
            }
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            if (Targetless)
            {
                return base.GetPhases(log, requirePhases);
            }
            List<PhaseData> phases = GetInitialPhase(log);
            phases[0].AddTargets(Targets.Where(x => x.IsSpecies(ArcDPSEnums.TargetID.EtherealBarrier)));
            return phases;
        }

        internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
        {

            AgentItem wallOfGhosts = agentData.GetNPCsByID(ArcDPSEnums.TrashID.WallOfGhosts).FirstOrDefault();
            if (wallOfGhosts == null)
            {
                return FightData.EncounterStartStatus.Late;
            }
            Point3D position = combatData.GetMovementData(wallOfGhosts).Where(x => x is PositionEvent positionEvt).Select(x => x.GetPoint3D()).FirstOrDefault();
            var initialPosition = new Point3D(-5669.139f, -7814.589f, -1138.749f);
            if (position != null)
            {
                return position.DistanceToPoint(initialPosition) > 10 ? FightData.EncounterStartStatus.Late : FightData.EncounterStartStatus.Normal;
            }
            // To investigate
            return FightData.EncounterStartStatus.Late;
        }

        internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            AgentItem wallOfGhosts = agentData.GetNPCsByID(ArcDPSEnums.TrashID.WallOfGhosts).FirstOrDefault();
            if (wallOfGhosts != null)
            {
                (Point3D, long Time) firstVelocity = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.Velocity && x.SrcMatchesAgent(wallOfGhosts)).Select(x => (AbstractMovementEvent.GetPoint3D(x), x.Time)).FirstOrDefault(x => x.Item1.Length2D() > 0);
                if (firstVelocity != default)
                {
                    return firstVelocity.Time;
                }
            }
            return EncounterLogicTimeUtils.GetGenericFightOffset(fightData);
        }

        internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            var maxHPs = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate && MaxHealthUpdateEvent.GetMaxHealth(x) == 1494000).ToList();
            bool needRefresh = false;
            foreach (CombatItem maxHP in maxHPs)
            {
                AgentItem candidate = agentData.GetAgent(maxHP.SrcAgent, maxHP.Time);
                if (candidate.Type == AgentItem.AgentType.Gadget)
                {
                    needRefresh = true;
                    candidate.OverrideID(ArcDPSEnums.TargetID.EtherealBarrier);
                    candidate.OverrideType(AgentItem.AgentType.NPC);
                }
            }
            if (needRefresh)
            {
                agentData.Refresh();
            } 
            else
            {
                agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "Dummy Spirit Race", Spec.NPC, ArcDPSEnums.TargetID.DummyTarget, true);
                Targetless = true;
            }
            base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
            int etherealBarriers = 0;
            foreach (AbstractSingleActor target in Targets)
            {
                switch (target.ID)
                {
                    case (int)ArcDPSEnums.TargetID.EtherealBarrier:
                        target.OverrideName(target.Character + " " + (++etherealBarriers));
                        break;
                    default:
                        break;
                }
            }
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Spirit Race";
        }


        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.EtherealBarrier:
                    HealthUpdateEvent hpZeroUpdate = log.CombatData.GetHealthUpdateEvents(target.AgentItem).FirstOrDefault(x => x.HealthPercent == 0);
                    if (hpZeroUpdate != null)
                    {
                        replay.Trim(replay.TimeOffsets.start, hpZeroUpdate.Time);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
