using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class Minion : AbstractActor
    {

        public Minion(AgentItem agent) : base(agent)
        {
        }

        protected override void SetDamageLogs(ParsedLog log)
        {
            AddDamageLogs(log.CombatData.GetDamageData(AgentItem));
        }

        protected override void SetBuffStatusCleanseWasteData(ParsedLog log, BuffSimulator simulator, long boonid, bool updateCondiPresence)
        {
        }

        protected override void SetBuffStatusGenerationData(ParsedLog log, BuffSimulationItem simul, long boonid)
        {
        }

        protected override void InitBuffStatusData(ParsedLog log)
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
