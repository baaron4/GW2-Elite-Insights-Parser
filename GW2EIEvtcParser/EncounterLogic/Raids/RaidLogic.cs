using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class RaidLogic : FightLogic
    {
        protected enum FallBackMethod { None, Death, CombatExit }

        protected FallBackMethod GenericFallBackMethod { get; set; } = FallBackMethod.Death;

        protected RaidLogic(int triggerID) : base(triggerID)
        {
            Mode = ParseMode.Instanced10;
            EncounterCategoryInformation.Category = FightCategory.Raid;
            EncounterID |= EncounterIDs.EncounterMasks.RaidMask; 
        }

        protected void SetSuccessByCombatExit(HashSet<int> targetIds, CombatData combatData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            var targets = Targets.Where(x => targetIds.Contains(x.ID)).ToList();
            SetSuccessByCombatExit(targets, combatData, fightData, playerAgents);
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            var raidRewardsTypes = new HashSet<int>();
            ulong build = combatData.GetBuildEvent().Build;
            if (build < 97235)
            {
                raidRewardsTypes = new HashSet<int>
                {
                    // Old types, on each kill
                    55821,
                    60685
                };
            }
            else
            {
                raidRewardsTypes = new HashSet<int>
                {
                    // New types, once per week
                    22797
                };
            }
            IReadOnlyList<RewardEvent> rewards = combatData.GetRewardEvents();
            RewardEvent reward = rewards.FirstOrDefault(x => raidRewardsTypes.Contains(x.RewardType));
            if (reward != null)
            {
                fightData.SetSuccess(true, reward.Time);
            }
            else
            {
                switch (GenericFallBackMethod)
                {
                    case FallBackMethod.Death:
                        SetSuccessByDeath(combatData, fightData, playerAgents, true, GetSuccessCheckIds());
                        break;
                    case FallBackMethod.CombatExit:
                        SetSuccessByDeath(combatData, fightData, playerAgents, true, GetSuccessCheckIds());
                        if (!fightData.Success)
                        {
                            SetSuccessByCombatExit(new HashSet<int>(GetSuccessCheckIds()), combatData, fightData, playerAgents);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                GenericTriggerID
            };
        }
    }
}
