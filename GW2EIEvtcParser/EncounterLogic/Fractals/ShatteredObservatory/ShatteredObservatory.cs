using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class ShatteredObservatory : FractalLogic
    {
        public ShatteredObservatory(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.ShatteredObservatory;
            EncounterID |= EncounterIDs.FractalMasks.ShatteredObservatoryMask;
        }

        protected static HashSet<AgentItem> GetParticipatingPlayerAgents(AbstractSingleActor target, CombatData combatData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            if (target == null)
            {
                return new HashSet<AgentItem>();
            }
            var participatingPlayerAgents = new HashSet<AgentItem>(combatData.GetDamageTakenData(target.AgentItem).Where(x => playerAgents.Contains(x.From.GetFinalMaster())).Select(x => x.From.GetFinalMaster()));
            participatingPlayerAgents.UnionWith(combatData.GetDamageData(target.AgentItem).Where(x => playerAgents.Contains(x.To.GetFinalMaster())).Select(x => x.To.GetFinalMaster()));
            return participatingPlayerAgents;
        }

        /// <summary>
        /// Returns true if the buff count was not reached so that another method can be called, if necessary
        /// </summary>
        protected static bool SetSuccessByBuffCount(CombatData combatData, FightData fightData, HashSet<AgentItem> playerAgents, AbstractSingleActor target, long buffID, int count)
        {
            if (target == null)
            {
                return false;
            }
            var invulsTarget = GetFilteredList(combatData, buffID, target, true, false).Where(x => x.Time >= 0).ToList();
            if (invulsTarget.Count == count)
            {
                AbstractBuffEvent last = invulsTarget.Last();
                if (!(last is BuffApplyEvent))
                {
                    SetSuccessByCombatExit(new List<AbstractSingleActor> { target }, combatData, fightData, playerAgents);
                    return false;
                }
            }
            return true;
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);
            IReadOnlyList<AbstractBuffEvent> beDynamic = log.CombatData.GetBuffData(AchievementEligibilityBeDynamic);
            int counter = 0;

            if (beDynamic.Any() && log.FightData.Success)
            {
                foreach (Player p in log.PlayerList)
                {
                    if (p.HasBuff(log, AchievementEligibilityBeDynamic, log.FightData.FightEnd - ServerDelayConstant))
                    {
                        counter++;
                    }
                }
            }
            // The party must have 5 players to be eligible
            if (counter == 5)
            {
                InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityBeDynamic], 1));
            }
        }
    }
}
