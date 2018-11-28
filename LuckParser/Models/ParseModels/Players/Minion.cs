using LuckParser.Models.DataModels;
using System;

namespace LuckParser.Models.ParseModels
{
    public class Minion : AbstractPlayer
    {

        public Minion(AgentItem agent) : base(agent)
        {
        }

        protected override void SetDamageLogs(ParsedLog log)
        {
            long timeStart = log.FightData.FightStart;
            long minTime = Math.Max(timeStart, AgentItem.FirstAware);
            long maxTime = Math.Min(log.FightData.FightEnd, AgentItem.LastAware);
            foreach (CombatItem c in log.GetDamageData(AgentItem.InstID))
            {
                if (c.Time >= minTime && c.Time <= maxTime)//selecting minion as caster
                {
                    long time = c.Time - timeStart;
                    AddDamageLog(time, c);
                }
            }
        }

        protected override void SetCastLogs(ParsedLog log)
        {
            long timeStart = log.FightData.FightStart;
            CastLog curCastLog = null;
            long minTime = Math.Max(timeStart, AgentItem.FirstAware);
            long maxTime = Math.Min(log.FightData.FightEnd, AgentItem.LastAware);
            foreach (CombatItem c in log.GetCastData(AgentItem.InstID))
            {
                if (!(c.Time >= minTime))
                {
                    continue;
                }
                ParseEnum.StateChange state = c.IsStateChange;
                if (state == ParseEnum.StateChange.Normal)
                {
                    if (c.IsActivation.IsCasting() && c.Time <= maxTime)
                    {
                        long time = c.Time - timeStart;
                        curCastLog = new CastLog(time, c.SkillID, c.Value, c.IsActivation);
                        CastLogs.Add(curCastLog);
                    }
                    else
                    {
                        if (curCastLog != null)
                        {
                            if (curCastLog.SkillId == c.SkillID)
                            {
                                curCastLog.SetEndStatus(c.Value, c.IsActivation, log.FightData.FightDuration);
                                curCastLog = null;
                            }
                        }
                    }
                }
            }
        }

        protected override void SetDamageTakenLogs(ParsedLog log)
        {
            throw new InvalidOperationException();
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
