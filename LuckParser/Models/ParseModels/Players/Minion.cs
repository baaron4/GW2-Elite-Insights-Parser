using LuckParser.Models.DataModels;
using System;

namespace LuckParser.Models.ParseModels
{
    public class Minion : AbstractPlayer
    {

        public Minion(ushort master, AgentItem agent) : base(agent)
        {
        }

        protected override void SetDamageLogs(ParsedLog log)
        {
            long timeStart = log.GetBossData().GetFirstAware();
            long minTime = Math.Max(timeStart, Agent.GetFirstAware());
            long maxTime = Math.Min(log.GetBossData().GetLastAware(), Agent.GetLastAware());
            foreach (CombatItem c in log.GetDamageData())
            {
                if (Agent.GetInstid() == c.GetSrcInstid() && c.GetTime() > minTime && c.GetTime() < maxTime)//selecting minion as caster
                {
                    long time = c.GetTime() - timeStart;
                    AddDamageLog(time, c);
                }
            }
        }

        protected override void SetCastLogs(ParsedLog log)
        {
            long timeStart = log.GetBossData().GetFirstAware();
            CastLog curCastLog = null;
            long minTime = Math.Max(timeStart, Agent.GetFirstAware());
            long maxTime = Math.Min(log.GetBossData().GetLastAware(), Agent.GetLastAware());
            foreach (CombatItem c in log.GetCastData())
            {
                if (!(c.GetTime() > minTime && c.GetTime() < maxTime))
                {
                    continue;
                }
                ParseEnum.StateChange state = c.IsStateChange();
                if (state == ParseEnum.StateChange.Normal)
                {
                    if (Agent.GetInstid() == c.GetSrcInstid())//selecting player as caster
                    {
                        if (c.IsActivation().IsCasting())
                        {
                            long time = c.GetTime() - timeStart;
                            curCastLog = new CastLog(time, c.GetSkillID(), c.GetValue(), c.IsActivation());
                            CastLogs.Add(curCastLog);
                        }
                        else
                        {
                            if (curCastLog != null)
                            {
                                if (curCastLog.GetID() == c.GetSkillID())
                                {
                                    curCastLog.SetEndStatus(c.GetValue(), c.IsActivation());
                                    curCastLog = null;
                                }
                            }
                        }

                    }
                }
            }
        }

        protected override void SetDamagetakenLogs(ParsedLog log)
        {
            // nothing to do
            return;
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
