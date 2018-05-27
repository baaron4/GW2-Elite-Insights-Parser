using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class Boss : Player
    {

        // Constructors
        public Boss(AgentItem agent) : base(agent)
        {        

        }
        

        // Private Methods
        protected override void setDamageLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            long time_start = bossData.getFirstAware();
            foreach (CombatItem c in combatList)
            {

                if (instid == c.getSrcInstid() || instid == c.getSrcMasterInstid())//selecting player or minion as caster
                {
                    LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
                    long time = c.getTime() - time_start;
                    foreach (AgentItem item in agentData.getAllAgentsList())
                    {//selecting all
                        if (item.getInstid() == c.getDstInstid() && c.getIFF().getEnum() == "FOE")
                        {
                            if (state.getEnum() == "NORMAL" && c.isBuffremove().getID() == 0)
                            {

                                if (c.isBuff() == 1 && c.getBuffDmg() != 0)//condi
                                {

                                    damage_logs.Add(new CondiDamageLog(time, c));
                                }
                                else if (c.isBuff() == 0 && c.getValue() != 0)//power
                                {
                                    damage_logs.Add(new PowerDamageLog(time, c));
                                }
                                else if (c.getResult().getID() == 5 || c.getResult().getID() == 6 || c.getResult().getID() == 7)
                                {//Hits that where blinded, invulned, interupts

                                    damage_logs.Add(new PowerDamageLog(time, c));
                                }
                            }

                        }
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
                        if (item.getInstid() == c.getSrcInstid() && c.getIFF().getEnum() == "FOE")
                        {
                            if (state.getID() == 0)
                            {
                                if (c.isBuff() == 1 && c.getBuffDmg() != 0)
                                {
                                    //inco,ing condi dmg not working or just not present?
                                    // damagetaken.Add(c.getBuffDmg());
                                }
                                else if (c.isBuff() == 0 && c.getValue() != 0)
                                {
                                    damagetaken.Add(c.getValue());
                                    damageTaken_logs.Add(new PowerDamageLog(time, c));

                                }
                                else if (c.isBuff() == 0 && c.getValue() == 0)
                                {

                                    damageTaken_logs.Add(new PowerDamageLog(time, c));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}