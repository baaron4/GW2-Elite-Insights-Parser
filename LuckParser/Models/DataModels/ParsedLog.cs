using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckParser.Models.ParseModels;

namespace LuckParser.Models.DataModels
{
    public class ParsedLog
    {
        private LogData log_data;
        private BossData boss_data;
        private AgentData agent_data = new AgentData();
        private SkillData skill_data = new SkillData();
        private CombatData combat_data = new CombatData();
        private MechanicData mech_data = new MechanicData();
        private List<Player> p_list = new List<Player>();
        private Boss boss;

        public ParsedLog(LogData log_data, BossData boss_data, AgentData agent_data, SkillData skill_data, 
                CombatData combat_data, MechanicData mech_data, List<Player> p_list, Boss boss)
        {
            this.log_data = log_data;
            this.boss_data = boss_data;
            this.agent_data = agent_data;
            this.skill_data = skill_data;
            this.combat_data = combat_data;
            this.mech_data = mech_data;
            this.p_list = p_list;
            this.boss = boss;
        }

        public BossData getBossData()
        {
            return boss_data;
        }

        public Boss getBoss()
        {
            return boss;
        }

        public CombatData getCombatData()
        {
            return combat_data;
        }

        public AgentData getAgentData()
        {
            return agent_data;
        }

        public List<Player> getPlayerList()
        {
            return p_list;
        }

        public MechanicData getMechanicData()
        {
            return mech_data;
        }

        public SkillData getSkillData()
        {
            return skill_data;
        }

        public LogData getLogData()
        {
            return log_data;
        }

        public List<CombatItem> getCombatList()
        {
            return combat_data.getCombatList();
        }

        public void validateLogData()
        {
            doMechData();
        }


        private void doMechData()
        {
            List<int> mIDList = new List<int>();
            foreach (Player p in p_list)
            {
                List<CombatItem> down = combat_data.getStates(p.getInstid(), "CHANGE_DOWN", boss_data.getFirstAware(), boss_data.getLastAware());
                foreach (CombatItem pnt in down)
                {
                    mech_data.AddItem(new MechanicLog((long)((pnt.getTime() - boss_data.getFirstAware()) / 1000f), 0, "DOWN", 0, p, mech_data.GetPLoltyShape("DOWN")));
                }
                List<CombatItem> dead = combat_data.getStates(p.getInstid(), "CHANGE_DEAD", boss_data.getFirstAware(), boss_data.getLastAware());
                foreach (CombatItem pnt in dead)
                {
                    mech_data.AddItem(new MechanicLog((long)((pnt.getTime() - boss_data.getFirstAware()) / 1000f), 0, "DEAD", 0, p, mech_data.GetPLoltyShape("DEAD")));
                }
                List<DamageLog> dls = p.getDamageTakenLogs(this, 0, boss_data.getAwareDuration());
                //Player hit by skill 3
                MechanicLog prevMech = null;
                foreach (DamageLog dLog in dls)
                {
                    string name = skill_data.getName(dLog.getID());
                    if (dLog.getResult().getID() < 3)
                    {

                        foreach (Mechanic mech in mech_data.GetMechList(boss_data.getID()).Where(x => x.GetMechType() == Mechanic.MechType.SkillOnPlayer))
                        {
                            //Prevent multi hit attacks form multi registering
                            if (prevMech != null)
                            {
                                if (dLog.getID() == prevMech.GetSkill() && mech.GetName() == prevMech.GetName() && (dLog.getTime() / 1000f) == prevMech.GetTime())
                                {
                                    break;
                                }
                            }
                            if (dLog.getID() == mech.GetSkill())
                            {


                                prevMech = new MechanicLog((long)(dLog.getTime() / 1000f), dLog.getID(), mech.GetName(), dLog.getDamage(), p, mech.GetPlotly());

                                mech_data.AddItem(prevMech);
                                break;
                            }
                        }
                    }
                }
                //Player gain buff 0,7
                foreach (CombatItem c in combat_data.getCombatList().Where(x => x.isBuffremove().getID() == 0 && x.isStateChange().getID() == 0))
                {
                    if (p.getInstid() == c.getDstInstid())
                    {
                        if (c.isBuff() == 1 && c.getValue() > 0 && c.isBuffremove().getID() == 0 && c.getResult().getID() < 3)
                        {
                            String name = skill_data.getName(c.getSkillID());
                            //buff on player 0
                            foreach (Mechanic mech in mech_data.GetMechList(boss_data.getID()).Where(x => x.GetMechType() == Mechanic.MechType.PlayerBoon))
                            {
                                if (c.getSkillID() == mech.GetSkill())
                                {
                                    //dst player
                                    mech_data.AddItem(new MechanicLog((long)((c.getTime() - boss_data.getFirstAware()) / 1000f), c.getSkillID(), mech.GetName(), c.getValue(), p, mech.GetPlotly()));
                                    break;
                                }
                            }
                            //player on player 7
                            foreach (Mechanic mech in mech_data.GetMechList(boss_data.getID()).Where(x => x.GetMechType() == Mechanic.MechType.PlayerOnPlayer))
                            {
                                if (c.getSkillID() == mech.GetSkill())
                                {
                                    //dst player
                                    mech_data.AddItem(new MechanicLog((long)((c.getTime() - boss_data.getFirstAware()) / 1000f), c.getSkillID(), mech.GetName(), c.getValue(), p, mech.GetPlotly()));
                                    //src player
                                    mech_data.AddItem(new MechanicLog((long)((c.getTime() - boss_data.getFirstAware()) / 1000f), c.getSkillID(), mech.GetName(), c.getValue(), p_list.FirstOrDefault(i => i.getInstid() == c.getSrcInstid()), mech.GetPlotly()));
                                    break;
                                }
                            }

                        }
                    }
                }
            }

        }

    }
}
