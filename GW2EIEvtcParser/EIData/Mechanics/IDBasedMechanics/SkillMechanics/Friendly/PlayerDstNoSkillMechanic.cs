using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerDstNoSkillMechanic : PlayerSkillMechanic
    {

        public PlayerDstNoSkillMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public PlayerDstNoSkillMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }
        protected override AgentItem GetAgentItem(AbstractHealthDamageEvent ahde)
        {
            return ahde.To;
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            var allPlayers = new HashSet<AbstractSingleActor>(log.PlayerList);
            var regroupedSkillDst = new Dictionary<long, HashSet<AbstractSingleActor>>();
            foreach (long skillID in MechanicIDs)
            {
                long lastTime = int.MinValue;
                foreach (AbstractHealthDamageEvent ahde in log.CombatData.GetDamageData(skillID))
                {
                    if (!Keep(ahde, log))
                    {
                        continue;
                    }
                    long time = ahde.Time;
                    if (Math.Abs(ahde.Time - lastTime) < ParserHelper.ServerDelayConstant)
                    {
                        time = lastTime;
                    }
                    AbstractSingleActor amp = GetActor(log, GetCreditedAgentItem(ahde), regroupedMobs);
                    if (amp != null)
                    {
                        if (regroupedSkillDst.TryGetValue(time, out HashSet<AbstractSingleActor> set))
                        {
                            set.Add(amp);
                        }
                        else
                        {
                            regroupedSkillDst[time] = new HashSet<AbstractSingleActor> { amp };
                        }
                    }

                    lastTime = ahde.Time;
                }
            }
            var regroupedNeverSkillDst = new Dictionary<long, HashSet<AbstractSingleActor>>();
            foreach (KeyValuePair<long, HashSet<AbstractSingleActor>> pair in regroupedSkillDst)
            {
                regroupedNeverSkillDst[pair.Key] = new HashSet<AbstractSingleActor>();
                foreach (AbstractSingleActor p in allPlayers.Except(pair.Value))
                {
                    if (!regroupedSkillDst.Any(x => x.Value != pair.Value && x.Value.Contains(p)))
                    {
                        regroupedNeverSkillDst[pair.Key].Add(p);
                    }
                }
            }
            foreach (KeyValuePair<long, HashSet<AbstractSingleActor>> pair in regroupedNeverSkillDst)
            {
                foreach (AbstractSingleActor p in pair.Value)
                {
                    InsertMechanic(log, mechanicLogs, pair.Key, p);
                }
            }
        }
    }
}
