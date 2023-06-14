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
            foreach (long skillID in MechanicIDs)
            {
                var regrouped = new Dictionary<long, HashSet<AbstractSingleActor>>();
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
                    AbstractSingleActor amp = GetActor(log, GetAgentItem(ahde), regroupedMobs);
                    if (amp != null)
                    {
                        if (regrouped.TryGetValue(time, out HashSet<AbstractSingleActor> set))
                        {
                            set.Add(amp);
                        }
                        else
                        {
                            regrouped[time] = new HashSet<AbstractSingleActor> { amp };
                        }
                    }    
                    
                    lastTime = ahde.Time;
                }
                foreach (KeyValuePair<long, HashSet<AbstractSingleActor>> pair in regrouped)
                {
                    foreach (AbstractSingleActor p in allPlayers.Except(pair.Value))
                    {
                        mechanicLogs[this].Add(new MechanicEvent(pair.Key, this, p));
                    }
                }
            }
        }
    }
}
