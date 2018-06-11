using LuckParser.Models;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.ParseModels.Boss;

namespace LuckParser.Controllers
{
    class HTMLHelper
    {

        public static string getFinalDPS(BossData b_data, CombatData c_data, AgentData a_data, AbstractPlayer p, Boss boss, int phase_index)
        {
            int totalboss_dps = 0;
            int totalboss_damage = 0;
            int totalbosscondi_dps = 0;
            int totalbosscondi_damage = 0;
            int totalbossphys_dps = 0;
            int totalbossphys_damage = 0;
            int totalAll_dps = 0;
            int totalAll_damage = 0;
            int totalAllcondi_dps = 0;
            int totalAllcondi_damage = 0;
            int totalAllphys_dps = 0;
            int totalAllphys_damage = 0;
            PhaseData phase = boss.getPhases(b_data, c_data.getCombatList(), a_data)[phase_index];
            double fight_duration = (phase.end - phase.start) / 1000.0;

            double damage = 0.0;
            double dps = 0.0;
            // All DPS

            damage = p.getDamageLogs(0, b_data, c_data.getCombatList(), a_data, phase.start, phase.end).Sum(x => x.getDamage());//p.getDamageLogs(b_data, c_data.getCombatList()).stream().mapToDouble(DamageLog::getDamage).sum();
            if (fight_duration > 0)
            {
                dps = damage / fight_duration;
            }
            totalAll_dps = (int)dps;
            totalAll_damage = (int)damage;
            //Allcondi
            damage = p.getDamageLogs(0, b_data, c_data.getCombatList(), a_data, phase.start, phase.end).Where(x => x.isCondi() > 0).Sum(x => x.getDamage());
            if (fight_duration > 0)
            {
                dps = damage / fight_duration;
            }
            totalAllcondi_dps = (int)dps;
            totalAllcondi_damage = (int)damage;
            //All Power
            damage = totalAll_damage - damage;
            if (fight_duration > 0)
            {
                dps = damage / fight_duration;
            }
            totalAllphys_dps = (int)dps;
            totalAllphys_damage = (int)damage;

            // boss DPS
            damage = p.getDamageLogs(b_data.getInstid(), b_data, c_data.getCombatList(), a_data, phase.start, phase.end).Sum(x => x.getDamage());//p.getDamageLogs(b_data, c_data.getCombatList()).stream().mapToDouble(DamageLog::getDamage).sum();
            if (fight_duration > 0)
            {
                dps = damage / fight_duration;
            }
            totalboss_dps = (int)dps;
            totalboss_damage = (int)damage;
            //bosscondi
            damage = p.getDamageLogs(b_data.getInstid(), b_data, c_data.getCombatList(), a_data, phase.start, phase.end).Where(x => x.isCondi() > 0).Sum(x => x.getDamage());
            if (fight_duration > 0)
            {
                dps = damage / fight_duration;
            }
            totalbosscondi_dps = (int)dps;
            totalbosscondi_damage = (int)damage;
            //boss Power
            damage = totalboss_damage - damage;
            if (fight_duration > 0)
            {
                dps = damage / fight_duration;
            }
            totalbossphys_dps = (int)dps;
            totalbossphys_damage = (int)damage;
            //Placeholders for further calc
            return totalAll_dps.ToString() + "|" + totalAll_damage.ToString() + "|" + totalAllphys_dps.ToString() + "|" + totalAllphys_damage.ToString() + "|" + totalAllcondi_dps.ToString() + "|" + totalAllcondi_damage.ToString() + "|"
                + totalboss_dps.ToString() + "|" + totalboss_damage.ToString() + "|" + totalbossphys_dps.ToString() + "|" + totalbossphys_damage.ToString() + "|" + totalbosscondi_dps.ToString() + "|" + totalbosscondi_damage.ToString();
        }
        public static string[] getFinalStats(BossData b_data, CombatData c_data, AgentData a_data, Player p, Boss boss, int phase_index)
        {
            String[] statsArray;
            PhaseData phase = boss.getPhases(b_data, c_data.getCombatList(), a_data)[phase_index];
            long start = phase.start + b_data.getFirstAware();
            long end = phase.end + b_data.getFirstAware();
            List<DamageLog> damage_logs = p.getDamageLogs(0, b_data, c_data.getCombatList(), a_data, phase.start, phase.end);
            List<CastLog> cast_logs = p.getCastLogs(b_data, c_data.getCombatList(), a_data, phase.start, phase.end);
            int instid = p.getInstid();

            // Rates
            int power_loop_count = 0;
            int critical_rate = 0;
            int scholar_rate = 0;
            int scholar_dmg = 0;
            int totaldamage = damage_logs.Sum(x => x.getDamage());

            int moving_rate = 0;
            int flanking_rate = 0;
            //glancerate
            int glance_rate = 0;
            //missed
            int missed = 0;
            //interupted
            int interupts = 0;
            //times enemy invulned
            int invulned = 0;

            //timeswasted
            int wasted = 0;
            double time_wasted = 0;
            //Time saved
            int saved = 0;
            double time_saved = 0;
            //avgboons
            double avgBoons = 0.0;

            foreach (DamageLog log in damage_logs)
            {
                if (log.isCondi() == 0)
                {
                    if (log.getResult().getEnum() == "CRIT")
                    {
                        critical_rate++;
                    }
                    if (log.isNinety() > 0)
                    {
                        scholar_rate++;

                        scholar_dmg += (int)(log.getDamage() / 11.0); //regular+10% damage
                    }
                    //scholar_rate += log.isNinety();
                    moving_rate += log.isMoving();
                    flanking_rate += log.isFlanking();
                    if (log.getResult().getEnum() == "GLANCE")
                    {
                        glance_rate++;
                    }
                    if (log.getResult().getEnum() == "BLIND")
                    {
                        missed++;
                    }
                    if (log.getResult().getEnum() == "INTERRUPT")
                    {
                        interupts++;
                    }
                    if (log.getResult().getEnum() == "ABSORB")
                    {
                        invulned++;
                    }
                    //if (log.isActivation().getEnum() == "CANCEL_FIRE" || log.isActivation().getEnum() == "CANCEL_CANCEL")
                    //{
                    //    wasted++;
                    //    time_wasted += log.getDamage();
                    //}
                    power_loop_count++;
                }
            }
            foreach (CastLog cl in cast_logs)
            {
                if (cl.endActivation() != null)
                {
                    if (cl.endActivation().getID() == 4)
                    {
                        wasted++;
                        time_wasted += cl.getActDur();
                    }
                    if (cl.endActivation().getID() == 3)
                    {
                        saved++;
                        if (cl.getActDur() < cl.getExpDur())
                        {
                            time_saved += cl.getExpDur() - cl.getActDur();
                        }
                    }
                }
            }

            power_loop_count = (power_loop_count == 0) ? 1 : power_loop_count;

            // Counts
            int swap = c_data.getStates(instid, "WEAPON_SWAP", start, end).Count();
            int down = c_data.getStates(instid, "CHANGE_DOWN", start, end).Count();
            int dodge = c_data.getSkillCount(instid, 65001, start, end) + c_data.getBuffCount(instid, 40408, start, end);//dodge = 65001 mirage cloak =40408
            int ress = c_data.getSkillCount(instid, 1066, start, end); //Res = 1066

            // R.I.P
            List<CombatItem> dead = c_data.getStates(instid, "CHANGE_DEAD", start, end);

            double died = 0.0;
            if (dead.Count() > 0)
            {
                died = dead[0].getTime() - start;
            }
            List<CombatItem> disconect = c_data.getStates(instid, "DESPAWN", start, end);
            double dcd = 0.0;
            if (disconect.Count() > 0)
            {
                dcd = disconect[0].getTime() - start;
            }
            statsArray = new string[] { power_loop_count.ToString(),
                                        critical_rate.ToString(),
                                        scholar_rate.ToString(),
                                        moving_rate.ToString(),
                                        flanking_rate.ToString(),
                                        swap.ToString(),
                                        down.ToString(),
                                        dodge.ToString(),
                                        ress.ToString(),
                                        died.ToString("0.00"),
                                        glance_rate.ToString(),
                                        missed.ToString(),
                                        interupts.ToString(),
                                        invulned.ToString(),
                                        (time_wasted/1000f).ToString(),
                                        wasted.ToString(),
                                        avgBoons.ToString(),
                                        (time_saved/1000f).ToString(),
                                        saved.ToString(),
                                        scholar_dmg.ToString(),
                                        totaldamage.ToString(),
                                        dcd.ToString("0.00"),
            };
            return statsArray;
        }
        public static string[] getFinalDefenses(BossData b_data, CombatData c_data, AgentData a_data, MechanicData m_data, Player p, Boss boss, int phase_index)
        {
            PhaseData phase = boss.getPhases(b_data, c_data.getCombatList(), a_data)[phase_index];
            long start = phase.start + b_data.getFirstAware();
            long end = phase.end + b_data.getFirstAware();
            List<DamageLog> damage_logs = p.getDamageTakenLogs(b_data, c_data.getCombatList(), a_data, m_data, phase.start, phase.end);
            int instid = p.getInstid();

            int damagetaken = damage_logs.Select(x => x.getDamage()).Sum();
            int blocked = 0;
            //int dmgblocked = 0;
            int invulned = 0;
            int dmginvulned = 0;
            int dodge = c_data.getSkillCount(instid, 65001, start, end);//dodge = 65001
            dodge += c_data.getBuffCount(instid, 40408, start, end);//mirage cloak add
            int evades = 0;
            //int dmgevaded = 0;
            int dmgBarriar = 0;
            foreach (DamageLog log in damage_logs.Where(x => x.getResult().getEnum() == "BLOCK"))
            {
                blocked++;
                //dmgblocked += log.getDamage();
            }
            foreach (DamageLog log in damage_logs.Where(x => x.getResult().getEnum() == "ABSORB"))
            {
                invulned++;
                dmginvulned += log.getDamage();
            }
            foreach (DamageLog log in damage_logs.Where(x => x.getResult().getEnum() == "EVADE"))
            {
                evades++;
                // dmgevaded += log.getDamage();
            }
            foreach (DamageLog log in damage_logs.Where(x => x.isShields() == 1))
            {

                dmgBarriar += log.getDamage();
            }
            int down = c_data.getStates(instid, "CHANGE_DOWN", start, end).Count();
            // R.I.P
            List<CombatItem> dead = c_data.getStates(instid, "CHANGE_DEAD", start, end);
            double died = 0.0;
            if (dead.Count() > 0)
            {
                died = dead[0].getTime() - start;
            }
            String[] statsArray = new string[] { damagetaken.ToString(),
                                                blocked.ToString(),
                                                "0"/*dmgblocked.ToString()*/,
                                                invulned.ToString(),
                                                dmginvulned.ToString(),
                                                dodge.ToString(),
                                                evades.ToString(),
                                                "0"/*dmgevaded.ToString()*/,
                                                down.ToString(),
                                                died.ToString("0.00"),
                                                dmgBarriar.ToString()};
            return statsArray;
        }
        //(currently not correct)
        public static string[] getFinalSupport(BossData b_data, CombatData c_data, AgentData a_data, Player p, Boss boss, int phase_index)
        {
            PhaseData phase = boss.getPhases(b_data, c_data.getCombatList(), a_data)[phase_index];
            long start = phase.start + b_data.getFirstAware();
            long end = phase.end + b_data.getFirstAware();
            // List<DamageLog> damage_logs = p.getDamageTakenLogs(b_data, c_data.getCombatList(), getAgentData());
            int instid = p.getInstid();
            int resurrects = 0;
            double restime = 0.0;
            int condiCleanse = 0;
            double condiCleansetime = 0.0;

            int[] resArray = p.getReses(b_data, c_data.getCombatList(), a_data, phase.start, phase.end);
            int[] cleanseArray = p.getCleanses(b_data, c_data.getCombatList(), a_data, phase.start, phase.end);
            resurrects = resArray[0];
            restime = resArray[1];
            condiCleanse = cleanseArray[0];
            condiCleansetime = cleanseArray[1];


            String[] statsArray = new string[] { resurrects.ToString(), (restime / 1000f).ToString(), condiCleanse.ToString(), (condiCleansetime / 1000f).ToString() };
            return statsArray;
        }
        public static Dictionary<int, string> getfinalboons(BossData b_data, CombatData c_data, SkillData s_data, Player p)
        {
            BoonDistribution boon_distrib = p.getBoonDistribution(b_data, s_data, c_data.getCombatList());
            Dictionary<int, string> rates = new Dictionary<int, string>();
            long fight_duration = b_data.getLastAware() - b_data.getFirstAware();
            foreach (Boon boon in Boon.getAllBuffList())
            {
                string rate = "0";
                if (boon_distrib.ContainsKey(boon.getID()))
                {
                    if (boon.getType() == Boon.BoonType.Duration)
                    {
                        rate = Math.Round(100.0 * boon_distrib.getUptime(boon.getID()) / fight_duration, 1) + "%";
                    }
                    else if (boon.getType() == Boon.BoonType.Intensity)
                    {
                        rate = Math.Round((double)boon_distrib.getUptime(boon.getID()) / fight_duration, 1).ToString();
                    }

                }
                rates[boon.getID()] = rate;
            }
            return rates;
        }
        public static Dictionary<int, string> getfinalboons(BossData b_data, CombatData c_data, SkillData s_data, Player p, List<Player> trgetPlayers)
        {
            if (trgetPlayers.Count() == 0)
            {
                return getfinalboons(b_data, c_data, s_data, p);
            }
            long fight_duration = b_data.getLastAware() - b_data.getFirstAware();
            Dictionary<Player, BoonDistribution> boon_logsDist = new Dictionary<Player, BoonDistribution>();
            foreach (Player player in trgetPlayers)
            {
                boon_logsDist[player] = player.getBoonDistribution(b_data, s_data, c_data.getCombatList());
            }
            Dictionary<int, string> rates = new Dictionary<int, string>();
            foreach (Boon boon in Boon.getAllBuffList())
            {
                string rate = "0";
                long total = 0;
                long totaloverstack = 0;
                foreach (Player player in trgetPlayers)
                {
                    BoonDistribution boon_dist = boon_logsDist[player];
                    if (boon_dist.ContainsKey(boon.getID()))
                    {
                        total += boon_dist.getGeneration(boon.getID(), p.getInstid());
                        totaloverstack += boon_dist.getOverstack(boon.getID(), p.getInstid());
                    }
                }
                totaloverstack += total;
                if (total > 0)
                {
                    if (boon.getType() == Boon.BoonType.Duration)
                    {
                        rate = "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                            + Math.Round(100.0 * totaloverstack / fight_duration / trgetPlayers.Count, 1) + "% with overstack \">"
                            + Math.Round(100.0 * total / fight_duration / trgetPlayers.Count, 1)
                            + "%</span>";
                    }
                    else if (boon.getType() == Boon.BoonType.Intensity)
                    {
                        rate = "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                            + Math.Round((double)totaloverstack / fight_duration / trgetPlayers.Count, 1).ToString() + " with overstack \">"
                            + Math.Round((double)total / fight_duration / trgetPlayers.Count, 1).ToString()
                            + "</span>";
                    }

                }
                rates[boon.getID()] = rate;
            }
            return rates;
        }
        public static Dictionary<int, string> getfinalcondis(BossData b_data, CombatData c_data, SkillData s_data, AbstractPlayer p)
        {
            BoonDistribution boon_distrib = p.getBoonDistribution(b_data, s_data, c_data.getCombatList());
            Dictionary<int, string> rates = new Dictionary<int, string>();
            foreach (Boon boon in Boon.getCondiBoonList())
            {
                rates[boon.getID()] = "0";
                if (boon_distrib.ContainsKey(boon.getID()))
                {
                    string rate = "0";
                    if (boon.getType() == Boon.BoonType.Duration)
                    {
                        long fight_duration = b_data.getLastAware() - b_data.getFirstAware();
                        rate = Math.Round(100.0 * boon_distrib.getUptime(boon.getID()) / fight_duration, 1) + "%";
                    }
                    else if (boon.getType() == Boon.BoonType.Intensity)
                    {
                        long fight_duration = b_data.getLastAware() - b_data.getFirstAware();
                        rate = Math.Round((double)boon_distrib.getUptime(boon.getID()) / fight_duration, 1).ToString();
                    }

                    rates[boon.getID()] = rate;
                }
            }
            return rates;
        }

        public static List<int[]> getDPSGraph(BossData b_data, CombatData c_data, AgentData a_data, AbstractPlayer p, Boss boss, int phase_index, ushort dstid)
        {
            List<int[]> dmgList = new List<int[]>();
            PhaseData phase = boss.getPhases(b_data, c_data.getCombatList(), a_data)[phase_index];
            List<DamageLog> damage_logs = p.getDamageLogs(dstid, b_data, c_data.getCombatList(), a_data, phase.start, phase.end).Where(x => x.getTime() >= phase.start && x.getTime() <= phase.end).ToList();
            int totaldmg = 0;

            int timeGraphed = (int)phase.start;
            foreach (DamageLog log in damage_logs)
            {

                totaldmg += log.getDamage();

                long time = log.getTime();
                if (time > 1000)
                {

                    //to reduce processing time only graph 1 point per sec
                    if (Math.Floor(time / 1000f) > timeGraphed)
                    {

                        if ((Math.Floor(time / 1000f) - timeGraphed) < 2)
                        {
                            timeGraphed = (int)Math.Floor(time / 1000f);
                            dmgList.Add(new int[] { (int)time / 1000, (int)(totaldmg / (time / 1000f)) });
                        }
                        else
                        {
                            int gap = (int)Math.Floor(time / 1000f) - timeGraphed;
                            bool startOfFight = true;
                            if (dmgList.Count > 0)
                            {
                                startOfFight = false;
                            }

                            for (int itr = 0; itr < gap - 1; itr++)
                            {
                                timeGraphed++;
                                if (!startOfFight)
                                {
                                    dmgList.Add(new int[] { timeGraphed, (int)(totaldmg / (float)timeGraphed) });
                                }
                                else
                                {//hasnt hit boss yet gap
                                    dmgList.Add(new int[] { timeGraphed, 0 });
                                }

                            }
                        }
                    }
                }
            }
            return dmgList;
        }
        /// <summary>
        /// Gets the points for the boss dps graph for a given player
        /// </summary>
        /// <param name="p">The player</param>
        /// <returns></returns>
        public static List<int[]> getBossDPSGraph(BossData b_data, CombatData c_data, AgentData a_data, AbstractPlayer p, Boss boss, int phase_index)
        {
            return getDPSGraph(b_data, c_data, a_data, p, boss, phase_index, b_data.getInstid());
        }
        /// <summary>
        /// Gets the points for the total dps graph for a given player
        /// </summary>
        /// <param name="p">The player</param>
        /// <returns></returns>
        public static List<int[]> getTotalDPSGraph(BossData b_data, CombatData c_data, AgentData a_data, AbstractPlayer p, Boss boss, int phase_index)
        {
            return getDPSGraph(b_data, c_data, a_data, p, boss, phase_index, 0);
        }

        public static void writeCastingItem(StreamWriter sw, CastLog cl, SkillData skill_data, long start, long end)
        {
            string skillName = "";
            GW2APISkill skill = null;
            List<SkillItem> s_list = skill_data.getSkillList();
            if (s_list.FirstOrDefault(x => x.getID() == cl.getID()) != null)
            {
                skill = s_list.FirstOrDefault(x => x.getID() == cl.getID()).GetGW2APISkill();
            }
            if (skill == null)
            {
                skillName = skill_data.getName(cl.getID());
            }
            else
            {
                skillName = skill.name;
            }
            float dur = 0.0f;
            if (skillName == "Dodge")
            {
                dur = 0.5f;
            }
            else if (cl.getID() == -2)
            {//wepswap
                skillName = "Weapon Swap";
                dur = 0.1f;
            }
            else if (skillName == "Resurrect")
            {
                dur = cl.getActDur() / 1000f;
            }
            else if (skillName == "Bandage")
            {
                dur = cl.getActDur() / 1000f;
            }
            else
            {
                dur = cl.getActDur() / 1000f;
            }
            skillName = skillName.Replace("\"", "");
            float offset = (cl.getTime() - start) / 1000f;
            float xVal = dur;
            if (offset < 0.0f)
            {
                xVal += offset;
            }
            xVal = Math.Min(xVal, (end - cl.getTime()) / 1000f);
            sw.Write("{");
            {

                sw.Write("y: ['1.5'],");

                sw.Write(
                       "x: ['" + xVal + "']," +
                       "base:'" + Math.Max(offset,0.0f) + "'," +
                       "name: \"" + skillName + " " + dur + "s\"," +//get name should be handled by api
                       "orientation:'h'," +
                       "mode: 'markers'," +
                       "type: 'bar',");
                if (skill != null)
                {
                    if (skill.slot == "Weapon_1")
                    {
                        sw.Write("width:'0.5',");
                    }
                    else
                    {
                        sw.Write("width:'1',");
                    }

                }
                else
                {
                    sw.Write("width:'1',");
                }
                sw.Write("hoverinfo: 'name'," +
                        "hoverlabel:{namelength:'-1'},");
                sw.Write("marker: {");
                {
                    if (cl.endActivation() != null)
                    {
                        if (cl.endActivation().getID() == 3)
                        {
                            sw.Write("color: 'rgb(40,40,220)',");
                        }
                        else if (cl.endActivation().getID() == 4)
                        {
                            sw.Write("color: 'rgb(220,40,40)',");
                        }
                        else if (cl.endActivation().getID() == 5)
                        {
                            sw.Write("color: 'rgb(40,220,40)',");
                        }
                        else
                        {
                            sw.Write("color: 'rgb(220,220,0)',");
                        }
                    }
                    else
                    {
                        sw.Write("color: 'rgb(220,220,0)',");
                    }
                    sw.Write("width: '5',");
                    sw.Write("line:{");
                    {
                        if (cl.startActivation() != null)
                        {
                            if (cl.startActivation().getID() == 1)
                            {
                                sw.Write("color: 'rgb(20,20,20)',");
                            }
                            else if (cl.startActivation().getID() == 2)
                            {
                                sw.Write("color: 'rgb(220,40,220)',");
                            }
                        }
                        sw.Write("width: '1'");
                    }
                    sw.Write("}");
                }
                sw.Write("},");
                sw.Write("showlegend: false");
            }
            sw.Write(" },");
        }

        public static void writeCastingItemIcon(StreamWriter sw, CastLog cl, SkillData skill_data, long start, bool last)
        {
            string skillIcon = "";
            GW2APISkill skill = null;
            List<SkillItem> s_list = skill_data.getSkillList();
            if (s_list.FirstOrDefault(x => x.getID() == cl.getID()) != null)
            {
                skill = s_list.FirstOrDefault(x => x.getID() == cl.getID()).GetGW2APISkill();
            }
            if (skill != null && cl.getID() != -2)
            {
                float offset = (cl.getTime() - start) / 1000f;
                if (skill.slot != "Weapon_1")
                {
                    skillIcon = skill.icon;
                    sw.Write("{" +
                                 "source: '" + skillIcon + "'," +
                                 "xref: 'x'," +
                                 "yref: 'y'," +
                                 "x: " + Math.Max(offset, 0.0f) + "," +
                                 "y: 0," +
                                 "sizex: 1.1," +
                                 "sizey: 1.1," +
                                 "xanchor: 'left'," +
                                 "yanchor: 'bottom'" +
                            "}");
                }
            }
            else
            {
                string skillName = "";

                if (cl.getID() == -2)
                { //wepswap
                    skillName = "Weapon Swap";
                    // skillIcon = "https://wiki.guildwars2.com/images/archive/c/ce/20140606174035%21Weapon_Swap_Button.png";
                }
                else
                {
                    skillName = skill_data.getName(cl.getID());
                }


                if (skillName == "Dodge")
                {
                    // skillIcon = "https://wiki.guildwars2.com/images/c/cc/Dodge_Instructor.png";
                }
                else if (skillName == "Resurrect")
                {
                    //skillIcon = "https://wiki.guildwars2.com/images/archive/d/dd/20120611120554%21Downed.png";
                }
                else if (skillName == "Bandage")
                {
                    // skillIcon = "https://wiki.guildwars2.com/images/0/0c/Bandage.png";
                }
                sw.Write("{" +
                              "source: '" + skillIcon + "'," +
                              "xref: 'x'," +
                              "yref: 'y'," +
                              "x: " + (cl.getTime() - start) / 1000f + "," +
                              "y: 0," +
                              "sizex: 1.1," +
                              "sizey: 1.1," +
                              "xanchor: 'left'," +
                              "yanchor: 'bottom'" +
                          "}");
            }
            if (!last)
            {
                sw.Write(",");
            }
        }

        public static void writeBoonTableHeader(StreamWriter sw, List<Boon> list_to_use)
        {
            sw.Write("<thead>");
            {
                sw.Write("<tr>");
                {
                    sw.Write("<th width=\"50px\">Sub</th>");
                    sw.Write("<th width=\"50px\"></th>");
                    sw.Write("<th>Name</th>");
                    foreach (Boon boon in list_to_use)
                    {
                        sw.Write("<th width=\"50px\">" + "<img src=\"" + boon.getLink() + " \" alt=\"" + boon.getName() + "\" title =\" " + boon.getName() + "\" height=\"18\" width=\"18\" >" + "</th>");
                    }
                }
                sw.Write("</tr> ");
            }
            sw.Write("</thead>");

        }

        public static void writeBoonGenTableBody(StreamWriter sw, Player player, List<Boon> list_to_use, Dictionary<int, string> boonArray, string prof_icon)
        {
            sw.Write("<tr>");
            {
                sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                sw.Write("<td>" + "<img src=\"" + prof_icon + " \" alt=\"" + player.getProf().ToString() + "\" height=\"20\" width=\"20\" >" + "</td>");
                sw.Write("<td>" + player.getCharacter().ToString() + "</td>");
                foreach (Boon boon in list_to_use)
                {
                    if (boonArray.ContainsKey(boon.getID()))
                    {
                        sw.Write("<td>" + boonArray[boon.getID()] + "</td>");
                    }
                    else
                    {
                        sw.Write("<td>" + 0 + "</td>");
                    }
                }
            }
            sw.Write("</tr>");
        }

        public static void writeDamageDistTableHeader(StreamWriter sw)
        {
            sw.Write("<thead>");
            {
                sw.Write("<tr>");
                {
                    sw.Write("<th>Skill</th>");
                    sw.Write("<th></th>");
                    sw.Write("<th>Damage</th>");
                    sw.Write("<th>Min</th>");
                    sw.Write("<th>Avg</th>");
                    sw.Write("<th>Max</th>");
                    sw.Write("<th>Casts</th>");
                    sw.Write("<th>Hits</th>");
                    sw.Write("<th>Hits per Cast</th>");
                    sw.Write("<th>Crit</th>");
                    sw.Write("<th>Flank</th>");
                    sw.Write("<th>Glance</th>");
                    sw.Write("<th>Wasted</th>");
                    sw.Write("<th>Saved</th>");
                }
                sw.Write("</tr>");
            }
            sw.Write("</thead>");
        }

        public static void writeDamageDistTableFoot(StreamWriter sw, int finalTotalDamage)
        {
            sw.Write("<tfoot class=\"text-dark\">");
            {
                sw.Write("<tr>");
                {
                    sw.Write("<th>Total</th>");
                    sw.Write("<th></th>");
                    sw.Write("<th>" + finalTotalDamage + "</th>");
                    sw.Write("<th></th>");
                    sw.Write("<th></th>");
                    sw.Write("<th></th>");
                    sw.Write("<th></th>");
                    sw.Write("<th></th>");
                    sw.Write("<th></th>");
                    sw.Write("<th></th>");
                    sw.Write("<th></th>");
                    sw.Write("<th></th>");
                    sw.Write("<th></th>");
                    sw.Write("<th></th>");
                }
                sw.Write("</tr>");
            }
            sw.Write("</tfoot>");
        }

        public static void writeDamageDistTableCondi(StreamWriter sw, HashSet<int> usedIDs, List<DamageLog> damageLogs, int finalTotalDamage)
        {
            foreach (Boon condi in Boon.getCondiBoonList())
            {
                int totaldamage = 0;
                int mindamage = 0;
                int avgdamage = 0;
                int hits = 0;
                int maxdamage = 0;
                int condiID = condi.getID();
                usedIDs.Add(condiID);
                foreach (DamageLog dl in damageLogs.Where(x => x.getID() == condiID))
                {
                    int curdmg = dl.getDamage();
                    totaldamage += curdmg;
                    if (0 == mindamage || curdmg < mindamage) { mindamage = curdmg; }
                    if (0 == maxdamage || curdmg > maxdamage) { maxdamage = curdmg; }
                    hits++;
                    int result = dl.getResult().getID();

                }
                avgdamage = (int)(totaldamage / (double)hits);
                if (totaldamage != 0)
                {
                    string condiName = condi.getName();// Boon.getCondiName(condiID);
                    sw.Write("<tr class=\"condi\">");
                    {
                        sw.Write("<td align=\"left\"><img src=" + condi.getLink() + " alt=\"" + condiName + "\" title=\"" + condiID + "\" height=\"18\" width=\"18\">" + condiName + "</td>");
                        sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)totaldamage / finalTotalDamage) + "%</td>");
                        sw.Write("<td>" + totaldamage + "</td>");
                        sw.Write("<td>" + mindamage + "</td>");
                        sw.Write("<td>" + avgdamage + "</td>");
                        sw.Write("<td>" + maxdamage + "</td>");
                        sw.Write("<td></td>");
                        sw.Write("<td>" + hits + "</td>");
                        sw.Write("<td></td>");
                        sw.Write("<td></td>");
                        sw.Write("<td></td>");
                        sw.Write("<td></td>");
                        sw.Write("<td></td>");
                        sw.Write("<td></td>");
                    }
                    sw.Write("</tr>");
                }
            }
        }

        public static void writeDamageDistTableSkill(StreamWriter sw, SkillItem skill, List<DamageLog> damageLogs, int finalTotalDamage, int casts = -1, double timeswasted = -1, double timessaved = 1)
        {
            int totaldamage = 0;
            int mindamage = 0;
            int avgdamage = 0;
            int hits = 0;
            int maxdamage = 0;
            int crit = 0;
            int flank = 0;
            int glance = 0;
            foreach (DamageLog dl in damageLogs)
            {
                int curdmg = dl.getDamage();
                totaldamage += curdmg;
                if (0 == mindamage || curdmg < mindamage) { mindamage = curdmg; }
                if (0 == maxdamage || curdmg > maxdamage) { maxdamage = curdmg; }
                hits++;
                int result = dl.getResult().getID();
                if (result == 1) { crit++; } else if (result == 2) { glance++; }
                if (dl.isFlanking() == 1) { flank++; }
            }
            avgdamage = (int)(totaldamage / (double)hits);
            string wasted = timeswasted > 0.0 ? Math.Round(timeswasted, 2) + "s" : "";
            string saved = timessaved < 0.0 ? Math.Round(timessaved, 2) + "s" : "";
            double hpcast = -1;
            if (casts > 0) {
                hpcast = Math.Round(hits / (double)casts, 2);
            }
            if (totaldamage != 0 && skill.GetGW2APISkill() != null)
            {
                sw.Write("<tr>");
                {
                    sw.Write("<td align=\"left\"><img src=" + skill.GetGW2APISkill().icon + " alt=\"" + skill.getName() + "\" title=\"" + skill.getID() + "\" height=\"18\" width=\"18\">" + skill.getName() + "</td>");
                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)totaldamage / finalTotalDamage) + "%</td>");
                    sw.Write("<td>" + totaldamage + "</td>");
                    sw.Write("<td>" + mindamage + "</td>");
                    sw.Write("<td>" + avgdamage + "</td>");
                    sw.Write("<td>" + maxdamage + "</td>");
                    sw.Write("<td>" + (casts != -1 ? casts.ToString() : "") + "</td>");
                    sw.Write("<td>" + hits + "</td>");
                    sw.Write("<td>" + (hpcast != -1 ? hpcast.ToString() : "") + "</td>");
                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)crit / hits) + "%</td>");
                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)flank / hits) + "%</td>");
                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)glance / hits) + "%</td>");
                    sw.Write("<td>" + wasted +"</td>");
                    sw.Write("<td>" + saved + "</td>");
                }
                sw.Write("</tr>");
            }
            else if (totaldamage != 0)
            {
                sw.Write("<tr>");
                {
                    sw.Write("<td align=\"left\">" + skill.getName() + "</td>");
                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)totaldamage / finalTotalDamage) + "%</td>");
                    sw.Write("<td>" + totaldamage + "</td>");
                    sw.Write("<td>" + mindamage + "</td>");
                    sw.Write("<td>" + avgdamage + "</td>");
                    sw.Write("<td>" + maxdamage + "</td>");
                    sw.Write("<td>" + (casts != -1 ? casts.ToString() : "") + "</td>");
                    sw.Write("<td>" + hits + "</td>");
                    sw.Write("<td>" + (hpcast != -1 ? hpcast.ToString() : "") + "</td>");
                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)crit / hits) + "%</td>");
                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)flank / hits) + "%</td>");
                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)glance / hits) + "%</td>");
                    sw.Write("<td>" + wasted + "</td>");
                    sw.Write("<td>" + saved + "</td>");
                }
                sw.Write("</tr>");
            }
            else if (skill.GetGW2APISkill() != null)
            {
                sw.Write("<tr>");
                {
                    sw.Write("<td align=\"left\"><img src=" + skill.GetGW2APISkill().icon + " alt=\"" + skill.getName() + "\" title=\"" + skill.getID() + "\" height=\"18\" width=\"18\">" + skill.getName() + "</td>");
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td>" + (casts != -1 ? casts.ToString() : "") + "</td>");
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td>" + wasted + "</td>");
                    sw.Write("<td>" + saved + "</td>");
                }
                sw.Write("</tr>");
            }
            else
            {
                sw.Write("<tr>");
                {
                    sw.Write("<td align=\"left\">" + skill.getName() + "</td>");
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td>" + (casts != -1 ? casts.ToString() : "") + "</td>");
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td>" + wasted + "</td>");
                    sw.Write("<td>" + saved + "</td>");
                }
                sw.Write("</tr>");
            }
        }

    }
}
