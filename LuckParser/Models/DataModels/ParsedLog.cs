using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckParser.Models.ParseModels;
using LuckParser.Models.ParseModels.Players;

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
        private bool movement_possible;

        // reduced data
        private List<CombatItem> boon_data;
        private List<CombatItem> damage_data;
        private List<CombatItem> damage_taken_data;
        //private List<CombatItem> healing_data;
        //private List<CombatItem> healing_received_data;
        private List<CombatItem> cast_data;
        private List<CombatItem> movement_data;

        public ParsedLog(LogData log_data, BossData boss_data, AgentData agent_data, SkillData skill_data, 
                CombatData combat_data, MechanicData mech_data, List<Player> p_list, Boss boss, bool movement_possible)
        {
            this.log_data = log_data;
            this.boss_data = boss_data;
            this.agent_data = agent_data;
            this.skill_data = skill_data;
            this.combat_data = combat_data;
            this.mech_data = mech_data;
            this.p_list = p_list;
            this.boss = boss;
            this.movement_possible = movement_possible;
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
            doReduction();
            doMechData();
        }


        private void doReduction()
        {
            boon_data = combat_data.getCombatList().Where(x => x.isBuff() > 0 && (x.isBuff() == 18 || x.getBuffDmg() == 0 || x.isBuffremove() != ParseEnum.BuffRemove.None)).ToList();

            damage_data = combat_data.getCombatList().Where(x => x.getDstInstid() != 0 && x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Foe && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                        ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                        (x.isBuff() == 0 && x.getValue() > 0))).ToList();

            damage_taken_data = combat_data.getCombatList().Where(x => x.isStateChange() == ParseEnum.StateChange.Normal && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                            ((x.isBuff() == 1 && x.getBuffDmg() >= 0 && x.getValue() == 0) ||
                                                (x.isBuff() == 0 && x.getValue() >= 0))).ToList();

            cast_data = combat_data.getCombatList().Where(x => (x.isStateChange() == ParseEnum.StateChange.Normal && x.isActivation() != ParseEnum.Activation.None) || x.isStateChange() == ParseEnum.StateChange.WeaponSwap).ToList();

            movement_data = movement_possible? combat_data.getCombatList().Where(x => x.isStateChange() == ParseEnum.StateChange.Position || x.isStateChange() == ParseEnum.StateChange.Velocity).ToList() : new List<CombatItem>();

            /*healing_data = combat_data.getCombatList().Where(x => x.getDstInstid() != 0 && x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                         ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                         (x.isBuff() == 0 && x.getValue() > 0))).ToList();

            healing_received_data = combat_data.getCombatList().Where(x => x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                            ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                                (x.isBuff() == 0 && x.getValue() >= 0))).ToList();*/
        }

        public List<CombatItem> getBoonData()
        {
            return boon_data;
        }

        public List<CombatItem> getDamageData()
        {
            return damage_data;
        }

        public List<CombatItem> getCastData()
        {
            return cast_data;
        }

        public List<CombatItem> getDamageTakenData()
        {
            return damage_taken_data;
        }

        /*public List<CombatItem> getHealingData()
        {
            return healing_data;
        }

        public List<CombatItem> getHealingReceivedData()
        {
            return healing_received_data;
        }*/

        public List<CombatItem> getMovementData()
        {
            return movement_data;
        }

        private void doMechData()
        {
            List<int> mIDList = new List<int>();
            foreach (Player p in p_list)
            {
                List<CombatItem> down = combat_data.getStates(p.getInstid(), ParseEnum.StateChange.ChangeDown, boss_data.getFirstAware(), boss_data.getLastAware());
                foreach (CombatItem pnt in down)
                {
                    mech_data.AddItem(new MechanicLog((long)Math.Round((pnt.getTime() - boss_data.getFirstAware()) / 1000f), 0, "DOWN", 0, p, mech_data.GetPLoltyShape("DOWN")));
                }
                List<CombatItem> dead = combat_data.getStates(p.getInstid(), ParseEnum.StateChange.ChangeDead, boss_data.getFirstAware(), boss_data.getLastAware());
                foreach (CombatItem pnt in dead)
                {
                    mech_data.AddItem(new MechanicLog((long)Math.Round((pnt.getTime() - boss_data.getFirstAware()) / 1000f), 0, "DEAD", 0, p, mech_data.GetPLoltyShape("DEAD")));
                }
                List<DamageLog> dls = p.getDamageTakenLogs(this, 0, boss_data.getAwareDuration());
                //Player hit by skill 3
                MechanicLog prevMech = null;
                foreach (DamageLog dLog in dls)
                {
                    string name = skill_data.getName(dLog.getID());
                    if (dLog.getResult().IsHit())
                    {

                        foreach (Mechanic mech in mech_data.GetMechList(boss_data.getID()).Where(x => x.GetMechType() == Mechanic.MechType.SkillOnPlayer))
                        {
                            //Prevent multi hit attacks form multi registering
                            if (prevMech != null)
                            {
                                if (dLog.getID() == prevMech.GetSkill() && mech.GetName() == prevMech.GetName() && (long)Math.Round(dLog.getTime() / 1000f) == prevMech.GetTime())
                                {
                                    break;
                                }
                            }
                            if (dLog.getID() == mech.GetSkill())
                            {


                                prevMech = new MechanicLog((long)Math.Round(dLog.getTime() / 1000f), dLog.getID(), mech.GetName(), dLog.getDamage(), p, mech.GetPlotly());

                                mech_data.AddItem(prevMech);
                                break;
                            }
                        }
                    }
                }
                //Player gain buff 0,7
                foreach (CombatItem c in combat_data.getCombatList().Where(x => x.isBuffremove() == ParseEnum.BuffRemove.None && x.isStateChange() == ParseEnum.StateChange.Normal))
                {
                    if (p.getInstid() == c.getDstInstid())
                    {
                        if (c.isBuff() == 1 && c.getValue() > 0 && c.isBuffremove() == ParseEnum.BuffRemove.None && c.getResult().IsHit())
                        {
                            String name = skill_data.getName(c.getSkillID());
                            //buff on player 0
                            foreach (Mechanic mech in mech_data.GetMechList(boss_data.getID()).Where(x => x.GetMechType() == Mechanic.MechType.PlayerBoon))
                            {
                                if (c.getSkillID() == mech.GetSkill())
                                {
                                    //dst player
                                    mech_data.AddItem(new MechanicLog((long)Math.Round((c.getTime() - boss_data.getFirstAware()) / 1000f), c.getSkillID(), mech.GetName(), c.getValue(), p, mech.GetPlotly()));
                                    break;
                                }
                            }
                            //player on player 7
                            foreach (Mechanic mech in mech_data.GetMechList(boss_data.getID()).Where(x => x.GetMechType() == Mechanic.MechType.PlayerOnPlayer))
                            {
                                if (c.getSkillID() == mech.GetSkill())
                                {
                                    //dst player
                                    mech_data.AddItem(new MechanicLog((long)Math.Round((c.getTime() - boss_data.getFirstAware()) / 1000f), c.getSkillID(), mech.GetName(), c.getValue(), p, mech.GetPlotly()));
                                    //src player
                                    mech_data.AddItem(new MechanicLog((long)Math.Round((c.getTime() - boss_data.getFirstAware()) / 1000f), c.getSkillID(), mech.GetName(), c.getValue(), p_list.FirstOrDefault(i => i.getInstid() == c.getSrcInstid()), mech.GetPlotly()));
                                    break;
                                }
                            }

                        }
                    }
                }
            }
            //Boon Was applied to Enemy
            List<Mechanic> enamyBoonMechs = mech_data.GetMechList(boss_data.getID()).Where(x => x.GetMechType() == Mechanic.MechType.EnemyBoon ).ToList();
            if (enamyBoonMechs.Count > 0)
            {
               
                List<CombatItem> enamyBuffs =  combat_data.getCombatList().Where(x => x.isBuff() == 1/* && x.getDstAgent() == boss_data.getAgent() */&& x.isBuffremove() == 0).ToList();
                foreach (Mechanic mech in enamyBoonMechs)
                {
                    List<CombatItem> enemyMechBuffs = enamyBuffs.Where(x => x.getSkillID() == mech.GetSkill()).ToList();
                    if (enemyMechBuffs.Count > 0)
                    {
                        foreach (CombatItem c in enemyMechBuffs)
                        {
                            AbstractMasterPlayer amp = null;
                            if (c.getDstAgent() == boss_data.getAgent())
                            {
                                amp = getBoss();
                            }
                            else
                            {
                                amp = new Mob(this.getAgentData().GetAgent(c.getDstAgent()));
                                
                            
                            }
                            mech_data.AddItem(new MechanicLog((long)Math.Round((c.getTime() - boss_data.getFirstAware()) / 1000f), c.getSkillID(), mech.GetName(), c.getValue(),amp, mech.GetPlotly()));
                        }
                    }
                }
            }
            //Removed Boon on Enemy
             enamyBoonMechs = mech_data.GetMechList(boss_data.getID()).Where(x => x.GetMechType() == Mechanic.MechType.EnemyBoonStrip).ToList();
            if (enamyBoonMechs.Count > 0)
            {
              
                List<CombatItem> enamyBuffs = combat_data.getCombatList().Where(x => x.isBuff() == 1 && x.isBuffremove() == ParseEnum.BuffRemove.Manual).ToList();
                foreach (Mechanic mech in enamyBoonMechs)
                {
                    List<CombatItem> enemyMechBuffs = enamyBuffs.Where(x => x.getSkillID() == mech.GetSkill()).ToList();
                    if (enemyMechBuffs.Count > 0)
                    {
                        foreach (CombatItem c in enemyMechBuffs)
                        {
                            AbstractMasterPlayer amp = null;
                            if (c.getDstAgent() == boss_data.getAgent())
                            {
                                amp = getBoss();
                            }
                            else
                            {
                                amp = new Mob(this.getAgentData().GetAgent(c.getDstAgent()));


                            }
                            mech_data.AddItem(new MechanicLog((long)Math.Round((c.getTime() - boss_data.getFirstAware()) / 1000f), c.getSkillID(), mech.GetName(), c.getValue(), amp, mech.GetPlotly()));
                        }
                    }
                }
            }

        }

    }
}
