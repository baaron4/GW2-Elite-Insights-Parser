using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Minion : AbstractActor
    {

        public Minion(AgentItem agent) : base(agent)
        {
        }

        protected override void SetDamageLogs(ParsedLog log)
        {
            foreach (CombatItem c in log.GetDamageData(InstID, FirstAware, LastAware))
            {
                long time = log.FightData.ToFightSpace(c.Time);
                AddDamageLog(time, c);
            }
        }

        protected override void SetExtraBoonStatusGenerationData(ParsedLog log, BoonSimulator simulator, long boonid, bool updateCondiPresence)
        {
        }

        protected override void SetBoonStatusGenerationData(ParsedLog log, BoonSimulationItem simul, long boonid, bool updateBoonPresence, bool updateCondiPresence)
        {
        }

        protected override void SetExtraBoonStatusData(ParsedLog log)
        {
        }

        protected override void InitBoonStatusData(ParsedLog log)
        {
        }

        /*protected override void setHealingLogs(ParsedLog log)
        {
            long time_start = log.getBossData().getFirstAware();
            long min_time = Math.Max(time_start, agent.getFirstAware());
            long max_time = Math.Min(log.getBossData().getLastAware(), agent.getLastAware());
            foreach (CombatItem c in log.getHealingData())
            {
                if (agent.getInstid() == c.getSrcInstid() && c.getTime() > min_time && c.getTime() < max_time)//selecting minion as caster
                {
                    long time = c.getTime() - time_start;
                    addHealingLog(time, c);
                }
            }
        }

        protected override void setHealingReceivedLogs(ParsedLog log)
        {
            //nothing to do
        }*/
    }
}
