using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class Boss : Player
    {
        
        public struct PhaseData
        {
            public long start;
            public long end;

            public PhaseData(long start, long end)
            {
                this.start = start;
                this.end = end;
            }
        }
        
        // Constructors
        public Boss(AgentItem agent) : base(agent)
        {        

        }

        public List<PhaseData> phases = new List<PhaseData>();

        public List<PhaseData> getPhases(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            if (phases.Count == 0)
            {
                setPhases(bossData, combatList, agentData);
            }
            return phases;
        }

        public void forcePhase(List<PhaseData> phases)
        {
            this.phases = phases;
        }

        // Private Methods
        private void setPhases(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            long fight_dur = bossData.getLastAware() - bossData.getFirstAware();
            phases.Add(new PhaseData(0, fight_dur));
            string name = getCharacter();
            long start = 0;
            long end = 0;
            List<CastLog> cls;
            switch (name)
            {
                case "Gorseval the Multifarious":
                    cls = getCastLogs(bossData, combatList, agentData).Where(x => x.getID() == 31759).ToList();
                    foreach (CastLog cl in cls)
                    {
                        end = cl.getTime();
                        phases.Add(new PhaseData(start, end));
                        start = end + cl.getActDur();
                    }
                    break;
                case "Sabetha the Saboteur":
                    cls = getCastLogs(bossData, combatList, agentData).Where(x => x.getID() == 31372).ToList();
                    foreach (CastLog cl in cls)
                    {

                    }
                    break;
                default:
                    return;
            }
            if (phases.Last().end != fight_dur)
            {
                phases.Add(new PhaseData(start, fight_dur));
            }
        }

        protected override void setDamageLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            long time_start = bossData.getFirstAware();
            foreach (CombatItem c in combatList)
            {
                if (instid == c.getSrcInstid() || instid == c.getSrcMasterInstid())//selecting player or minion as caster
                {
                    long time = c.getTime() - time_start;
                    foreach (AgentItem item in agentData.getAllAgentsList())
                    {//selecting all
                        addDamageLog(time, item.getInstid(), c, damage_logs);
                    }
                }
            }
        }
        protected override void setDamagetaken(BossData bossData, List<CombatItem> combatList, AgentData agentData, MechanicData m_data)
        {
            long time_start = bossData.getFirstAware();
            foreach (CombatItem c in combatList)
            {
                if (instid == c.getDstInstid())
                {//selecting player as target
                    LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
                    long time = c.getTime() - time_start;
                    foreach (AgentItem item in agentData.getAllAgentsList())
                    {//selecting all

                        setDamageTakenLog(time, item.getInstid(), c);
                    }
                }
            }
        }
    }
}