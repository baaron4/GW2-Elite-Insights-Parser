using LuckParser.Models;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

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
            int crit_dmg = 0;
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

                        crit_dmg += log.getDamage();
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
                                        crit_dmg.ToString()
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

        public static List<Point> getDPSGraph(BossData b_data, CombatData c_data, AgentData a_data, AbstractPlayer p, Boss boss, int phase_index, ushort dstid)
        {
            int id = phase_index + dstid;
            if (p.getDPSGraph(id).Count > 0)
            {
                return p.getDPSGraph(id);
            }

            List<Point> dmgList = new List<Point>();
            PhaseData phase = boss.getPhases(b_data, c_data.getCombatList(), a_data)[phase_index];
            List<DamageLog> damage_logs = p.getDamageLogs(dstid, b_data, c_data.getCombatList(), a_data, phase.start, phase.end);
            // fill the graph, full precision
            List<double> dmgListFull = new List<double>();
            for (int i = 0; i <= (phase.end - phase.start); i++) {
                dmgListFull.Add(0.0);
            }
            int total_time = 1;
            int total_damage = 0;
            foreach (DamageLog log in damage_logs)
            {
                int time = (int)(log.getTime()- phase.start);
                // fill
                for (; total_time < time; total_time++)
                {
                    dmgListFull[total_time] = (1000.0 * total_damage / total_time);
                }
                total_damage += log.getDamage();
                dmgListFull[total_time] = (1000.0 * total_damage / total_time);
            }
            // fill
            for (; total_time <= (phase.end - phase.start); total_time++)
            {
                dmgListFull[total_time] = (1000.0 * total_damage / total_time);
            }
            for (int i = 0; i <= (phase.end - phase.start)/1000; i++)
            {
                dmgList.Add(new Point(i, (int)Math.Round(dmgListFull[1000 * i])));
            }
            p.addDPSGraph(id, dmgList);
            return dmgList;
        }
        /// <summary>
        /// Gets the points for the boss dps graph for a given player
        /// </summary>
        /// <param name="p">The player</param>
        /// <returns></returns>
        public static List<Point> getBossDPSGraph(BossData b_data, CombatData c_data, AgentData a_data, AbstractPlayer p, Boss boss, int phase_index)
        {
            return getDPSGraph(b_data, c_data, a_data, p, boss, phase_index, b_data.getInstid());
        }
       
        /// <summary>
        /// Gets the points for the total dps graph for a given player
        /// </summary>
        /// <param name="p">The player</param>
        /// <returns></returns>
        public static List<Point> getTotalDPSGraph(BossData b_data, CombatData c_data, AgentData a_data, AbstractPlayer p, Boss boss, int phase_index)
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

        public static void writeBoonGenTableBody(StreamWriter sw, Player player, List<Boon> list_to_use, Dictionary<int, string> boonArray)
        {
            sw.Write("<tr>");
            {
                sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                sw.Write("<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"20\" width=\"20\" >" + "</td>");
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
                    sw.Write("<td>" + (casts != -1 ? casts.ToString() : "") + "</td>");
                    sw.Write("<td></td>");
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
                    sw.Write("<td>" + (casts != -1 ? casts.ToString() : "") + "</td>");
                    sw.Write("<td></td>");
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

        public static void writeBossHealthGraph(StreamWriter sw, int maxDPS, long start, long end, BossData boss_data, string y_axis = "")
        {
            //Boss Health
            //Adding dps axis
            sw.Write("y: [");
            if (maxDPS == 0)
            {
                maxDPS = 1000;
            }
            int hotCount = 0;
            List<Point> BossHOT = boss_data.getHealthOverTime().Where(x => x.X >= start && x.X <= end).ToList();
            foreach (Point dp in BossHOT)
            {
                if (hotCount == BossHOT.Count - 1)
                {
                    sw.Write("'" + ((dp.Y / 10000f) * maxDPS).ToString().Replace(',', '.') + "'");
                }
                else
                {
                    sw.Write("'" + ((dp.Y / 10000f) * maxDPS).ToString().Replace(',', '.') + "',");
                }
                hotCount++;

            }

            sw.Write("],");
            //text axis is boss hp in %
            sw.Write("text: [");

            float scaler2 = boss_data.getHealth() / 100;
            hotCount = 0;
            foreach (Point dp in BossHOT)
            {
                if (hotCount == BossHOT.Count - 1)
                {
                    sw.Write("'" + dp.Y / 100f + "% HP'");
                }
                else
                {
                    sw.Write("'" + dp.Y / 100f + "% HP',");
                }
                hotCount++;

            }

            sw.Write("],");
            //add time axis
            sw.Write("x: [");
            hotCount = 0;
            foreach (Point dp in BossHOT)
            {
                if (hotCount == BossHOT.Count - 1)
                {
                    sw.Write("'" + ((dp.X - start) / 1000).ToString().Replace(',', '.') + "'");
                }
                else
                {
                    sw.Write("'" + ((dp.X - start) / 1000).ToString().Replace(',', '.') + "',");
                }

                hotCount++;
            }

            sw.Write("],");
            sw.Write(" mode: 'lines'," +
                    " line: {shape: 'spline', dash: 'dashdot'}," +
                   ( y_axis.Length > 0 ? " yaxis: '"+ y_axis+"',"  : "") +
                    "hoverinfo: 'text'," +
                    " name: 'Boss health'");
            
        }

        public static void writeBoonGraph(StreamWriter sw, BoonsGraphModel bgm, long start, long end)
        {
            List<Point> bChart = bgm.getBoonChart().Where(x => x.X >= start / 1000 && x.X <= end / 1000).ToList();
            int bChartCount = 0;
            sw.Write("y: [");
            {
                foreach (Point pnt in bChart)
                {
                    if (bChartCount == bChart.Count - 1)
                    {
                        sw.Write("'" + pnt.Y + "'");
                    }
                    else
                    {
                        sw.Write("'" + pnt.Y + "',");
                    }
                    bChartCount++;
                }
                if (bgm.getBoonChart().Count == 0)
                {
                    sw.Write("'0'");
                }
            }
            sw.Write("],");
            sw.Write("x: [");
            {
                bChartCount = 0;
                foreach (Point pnt in bChart)
                {
                    if (bChartCount == bChart.Count - 1)
                    {
                        sw.Write("'" + (pnt.X - (int)start / 1000) + "'");
                    }
                    else
                    {
                        sw.Write("'" + (pnt.X - (int)start / 1000) + "',");
                    }
                    bChartCount++;
                }
                if (bgm.getBoonChart().Count == 0)
                {
                    sw.Write("'0'");
                }
            }
            sw.Write("],");
            sw.Write(" yaxis: 'y2'," +
                 " type: 'scatter',");
            //  "legendgroup: '"+Boon.getEnum(bgm.getBoonName()).getPloltyGroup()+"',";
            if (bgm.getBoonName() == "Might" || bgm.getBoonName() == "Quickness")
            {

            }
            else
            {
                sw.Write(" visible: 'legendonly',");
            }
            sw.Write(" line: {color:'" + GetLink("Color-" + bgm.getBoonName()) + "'},");
            sw.Write(" fill: 'tozeroy'," +
                 " name: \"" + bgm.getBoonName() + "\"");
        }

        public static void writeDPSGraph(StreamWriter sw, string name, List<Point> playerdpsgraphdata, AbstractPlayer p)
        {
            int ptdgCount = 0;
            sw.Write("y: [");
            {
                foreach (Point dp in playerdpsgraphdata)
                {
                    if (ptdgCount == playerdpsgraphdata.Count - 1)
                    {
                        sw.Write("'" + dp.Y + "'");
                    }
                    else
                    {
                        sw.Write("'" + dp.Y + "',");
                    }
                    ptdgCount++;
                }
                if (playerdpsgraphdata.Count == 0)
                {
                    sw.Write("'0'");
                }
            }
            sw.Write("],");
            //add time axis
            sw.Write("x: [");
            {
                ptdgCount = 0;
                foreach (Point dp in playerdpsgraphdata)
                {
                    if (ptdgCount == playerdpsgraphdata.Count - 1)
                    {
                        sw.Write("'" + dp.X + "'");
                    }
                    else
                    {
                        sw.Write("'" + dp.X + "',");
                    }
                    ptdgCount++;
                }
                if (playerdpsgraphdata.Count == 0)
                {
                    sw.Write("'0'");
                }
            }
            sw.Write("],");
            sw.Write(" mode: 'lines'," +
                   "line: {shape: 'spline',color:'" + GetLink("Color-" + p.getProf() + (name.Contains("Total") ? "-Total" : ""))  + "'}," +
                   "yaxis: 'y3'," +
                   // "legendgroup: 'Damage'," +
                   "name: '"+ name+"'");
        }

        public static string GetLink(string name)
        {
            switch (name)
            {
                case "Question":
                    return "https://wiki.guildwars2.com/images/thumb/d/de/Sword_slot.png/40px-Sword_slot.png";
                case "Sword":
                    return "https://wiki.guildwars2.com/images/0/07/Crimson_Antique_Blade.png";
                case "Axe":
                    return "https://wiki.guildwars2.com/images/d/d4/Crimson_Antique_Reaver.png";
                case "Dagger":
                    return "https://wiki.guildwars2.com/images/6/65/Crimson_Antique_Razor.png";
                case "Mace":
                    return "https://wiki.guildwars2.com/images/6/6d/Crimson_Antique_Flanged_Mace.png";
                case "Pistol":
                    return "https://wiki.guildwars2.com/images/4/46/Crimson_Antique_Revolver.png";
                case "Scepter":
                    return "https://wiki.guildwars2.com/images/e/e2/Crimson_Antique_Wand.png";
                case "Focus":
                    return "https://wiki.guildwars2.com/images/8/87/Crimson_Antique_Artifact.png";
                case "Shield":
                    return "https://wiki.guildwars2.com/images/b/b0/Crimson_Antique_Bastion.png";
                case "Torch":
                    return "https://wiki.guildwars2.com/images/7/76/Crimson_Antique_Brazier.png";
                case "Warhorn":
                    return "https://wiki.guildwars2.com/images/1/1c/Crimson_Antique_Herald.png";
                case "Greatsword":
                    return "https://wiki.guildwars2.com/images/5/50/Crimson_Antique_Claymore.png";
                case "Hammer":
                    return "https://wiki.guildwars2.com/images/3/38/Crimson_Antique_Warhammer.png";
                case "Longbow":
                    return "https://wiki.guildwars2.com/images/f/f0/Crimson_Antique_Greatbow.png";
                case "Shortbow":
                    return "https://wiki.guildwars2.com/images/1/17/Crimson_Antique_Short_Bow.png";
                case "Rifle":
                    return "https://wiki.guildwars2.com/images/1/19/Crimson_Antique_Musket.png";
                case "Staff":
                    return "https://wiki.guildwars2.com/images/5/5f/Crimson_Antique_Spire.png";
                case "Vale Guardian-icon":
                    return "https://wiki.guildwars2.com/images/f/fb/Mini_Vale_Guardian.png";
                case "Gorseval the Multifarious-icon":
                    return "https://wiki.guildwars2.com/images/d/d1/Mini_Gorseval_the_Multifarious.png";
                case "Sabetha the Saboteur-icon":
                    return "https://wiki.guildwars2.com/images/5/54/Mini_Sabetha.png";
                case "Slothasor-icon":
                    return "https://wiki.guildwars2.com/images/e/ed/Mini_Slubling.png";
                case "Matthias Gabrel-icon":
                    return "https://wiki.guildwars2.com/images/5/5d/Mini_Matthias_Abomination.png";
                case "Keep Construct-icon":
                    return "https://wiki.guildwars2.com/images/e/ea/Mini_Keep_Construct.png";
                case "Xera-icon":
                    return "https://wiki.guildwars2.com/images/4/4b/Mini_Xera.png";
                case "Cairn the Indomitable-icon":
                    return "https://wiki.guildwars2.com/images/b/b8/Mini_Cairn_the_Indomitable.png";
                case "Mursaat Overseer-icon":
                    return "https://wiki.guildwars2.com/images/c/c8/Mini_Mursaat_Overseer.png";
                case "Samarog-icon":
                    return "https://wiki.guildwars2.com/images/f/f0/Mini_Samarog.png";
                case "Deimos-icon":
                    return "https://wiki.guildwars2.com/images/e/e0/Mini_Ragged_White_Mantle_Figurehead.png";
                case "Soulless Horror-icon":
                    return "https://wiki.guildwars2.com/images/d/d4/Mini_Desmina.png";
                case "Dhuum-icon":
                    return "https://wiki.guildwars2.com/images/e/e4/Mini_Dhuum.png";
                case "Vale Guardian-ext":
                    return "vg";
                case "Gorseval the Multifarious-ext":
                    return "gors";
                case "Sabetha the Saboteur-ext":
                    return "sab";
                case "Slothasor-ext":
                    return "sloth";
                case "Matthias Gabrel-ext":
                    return "matt";
                case "Keep Construct-ext":
                    return "kc";
                case "Xera-ext":
                    return "xera";
                case "Cairn the Indomitable-ext":
                    return "cairn";
                case "Mursaat Overseer-ext":
                    return "mo";
                case "Samarog-ext":
                    return "sam";
                case "Deimos-ext":
                    return "dei";
                case "Soulless Horror-ext":
                    return "sh";
                case "Dhuum-ext":
                    return "dhuum";

                //ID version for multilingual
                case "15438-icon":
                    return "https://wiki.guildwars2.com/images/f/fb/Mini_Vale_Guardian.png";
                case "15429-icon":
                    return "https://wiki.guildwars2.com/images/d/d1/Mini_Gorseval_the_Multifarious.png";
                case "15375-icon":
                    return "https://wiki.guildwars2.com/images/5/54/Mini_Sabetha.png";
                case "16123-icon":
                    return "https://wiki.guildwars2.com/images/e/ed/Mini_Slubling.png";
                case "16115-icon":
                    return "https://wiki.guildwars2.com/images/5/5d/Mini_Matthias_Abomination.png";
                case "16235-icon":
                    return "https://wiki.guildwars2.com/images/e/ea/Mini_Keep_Construct.png";
                case "16246-icon":
                    return "https://wiki.guildwars2.com/images/4/4b/Mini_Xera.png";
                case "17194-icon":
                    return "https://wiki.guildwars2.com/images/b/b8/Mini_Cairn_the_Indomitable.png";
                case "17172-icon":
                    return "https://wiki.guildwars2.com/images/c/c8/Mini_Mursaat_Overseer.png";
                case "17188-icon":
                    return "https://wiki.guildwars2.com/images/f/f0/Mini_Samarog.png";
                case "17154-icon":
                    return "https://wiki.guildwars2.com/images/e/e0/Mini_Ragged_White_Mantle_Figurehead.png";
                case "19767-icon":
                    return "https://wiki.guildwars2.com/images/d/d4/Mini_Desmina.png";
                case "19450-icon":
                    return "https://wiki.guildwars2.com/images/e/e4/Mini_Dhuum.png";
                case "17021-icon":
                    return "http://dulfy.net/wp-content/uploads/2016/11/gw2-nightmare-fractal-teaser.jpg";
                case "17028-icon":
                    return "https://wiki.guildwars2.com/images/d/dc/Siax_the_Corrupted.jpg";
                case "16948-icon":
                    return "https://wiki.guildwars2.com/images/5/57/Champion_Toxic_Hybrid.jpg";
                case "17632-icon":
                    return "https://wiki.guildwars2.com/images/c/c1/Skorvald_the_Shattered.jpg";
                case "17949-icon":
                    return "https://wiki.guildwars2.com/images/b/b4/Artsariiv.jpg";
                case "17759-icon":
                    return "https://wiki.guildwars2.com/images/5/5f/Arkk.jpg";

                case "15438-ext":
                    return "vg";
                case "15429-ext":
                    return "gors";
                case "15375-ext":
                    return "sab";
                case "16123-ext":
                    return "sloth";
                case "16115-ext":
                    return "matt";
                case "16235-ext":
                    return "kc";
                case "16246-ext":
                    return "xera";
                case "17194-ext":
                    return "cairn";
                case "17172-ext":
                    return "mo";
                case "17188-ext":
                    return "sam";
                case "17154-ext":
                    return "dei";
                case "19767-ext":
                    return "sh";
                case "19450-ext":
                    return "dhuum";
                case "17021-ext":
                    return "mama";
                case "17028-ext":
                    return "siax";
                case "16948-ext":
                    return "ensol";
                case "17632-ext":
                    return "skorv";
                case "17949-ext":
                    return "arstra";
                case "17759-ext":
                    return "arkk";

                case "Warrior":
                    return "https://wiki.guildwars2.com/images/4/43/Warrior_tango_icon_20px.png";
                case "Berserker":
                    return "https://wiki.guildwars2.com/images/d/da/Berserker_tango_icon_20px.png";
                case "Spellbreaker":
                    return "https://wiki.guildwars2.com/images/e/ed/Spellbreaker_tango_icon_20px.png";
                case "Guardian":
                    return "https://wiki.guildwars2.com/images/8/8c/Guardian_tango_icon_20px.png";
                case "Dragonhunter":
                    return "https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png";
                case "DragonHunter":
                    return "https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png";
                case "Firebrand":
                    return "https://wiki.guildwars2.com/images/0/02/Firebrand_tango_icon_20px.png";
                case "Revenant":
                    return "https://wiki.guildwars2.com/images/b/b5/Revenant_tango_icon_20px.png";
                case "Herald":
                    return "https://wiki.guildwars2.com/images/6/67/Herald_tango_icon_20px.png";
                case "Renegade":
                    return "https://wiki.guildwars2.com/images/7/7c/Renegade_tango_icon_20px.png";
                case "Engineer":
                    return "https://wiki.guildwars2.com/images/2/27/Engineer_tango_icon_20px.png";
                case "Scrapper":
                    return "https://wiki.guildwars2.com/images/3/3a/Scrapper_tango_icon_200px.png";
                case "Holosmith":
                    return "https://wiki.guildwars2.com/images/2/28/Holosmith_tango_icon_20px.png";
                case "Ranger":
                    return "https://wiki.guildwars2.com/images/4/43/Ranger_tango_icon_20px.png";
                case "Druid":
                    return "https://wiki.guildwars2.com/images/d/d2/Druid_tango_icon_20px.png";
                case "Soulbeast":
                    return "https://wiki.guildwars2.com/images/7/7c/Soulbeast_tango_icon_20px.png";
                case "Thief":
                    return "https://wiki.guildwars2.com/images/7/7a/Thief_tango_icon_20px.png";
                case "Daredevil":
                    return "https://wiki.guildwars2.com/images/e/e1/Daredevil_tango_icon_20px.png";
                case "Deadeye":
                    return "https://wiki.guildwars2.com/images/c/c9/Deadeye_tango_icon_20px.png";
                case "Elementalist":
                    return "https://wiki.guildwars2.com/images/a/aa/Elementalist_tango_icon_20px.png";
                case "Tempest":
                    return "https://wiki.guildwars2.com/images/4/4a/Tempest_tango_icon_20px.png";
                case "Weaver":
                    return "https://wiki.guildwars2.com/images/f/fc/Weaver_tango_icon_20px.png";
                case "Mesmer":
                    return "https://wiki.guildwars2.com/images/6/60/Mesmer_tango_icon_20px.png";
                case "Chronomancer":
                    return "https://wiki.guildwars2.com/images/f/f4/Chronomancer_tango_icon_20px.png";
                case "Mirage":
                    return "https://wiki.guildwars2.com/images/d/df/Mirage_tango_icon_20px.png";
                case "Necromancer":
                    return "https://wiki.guildwars2.com/images/4/43/Necromancer_tango_icon_20px.png";
                case "Reaper":
                    return "https://wiki.guildwars2.com/images/1/11/Reaper_tango_icon_20px.png";
                case "Scourge":
                    return "https://wiki.guildwars2.com/images/0/06/Scourge_tango_icon_20px.png";

                case "Color-Warrior": return "rgb(255,209,102)";
                case "Color-Berserker": return "rgb(255,209,102)";
                case "Color-Spellbreaker": return "rgb(255,209,102)";
                case "Color-Guardian": return "rgb(114,193,217)";
                case "Color-Dragonhunter": return "rgb(114,193,217)";
                case "Color-Firebrand": return "rgb(114,193,217)";
                case "Color-Revenant": return "rgb(209,110,90)";
                case "Color-Herald": return "rgb(209,110,90)";
                case "Color-Renegade": return "rgb(209,110,90)";
                case "Color-Engineer": return "rgb(208,156,89)";
                case "Color-Scrapper": return "rgb(208,156,89)";
                case "Color-Holosmith": return "rgb(208,156,89)";
                case "Color-Ranger": return "rgb(140,220,130)";
                case "Color-Druid": return "rgb(140,220,130)";
                case "Color-Soulbeast": return "rgb(140,220,130)";
                case "Color-Thief": return "rgb(192,143,149)";
                case "Color-Daredevil": return "rgb(192,143,149)";
                case "Color-Deadeye": return "rgb(192,143,149)";
                case "Color-Elementalist": return "rgb(246,138,135)";
                case "Color-Tempest": return "rgb(246,138,135)";
                case "Color-Weaver": return "rgb(246,138,135)";
                case "Color-Mesmer": return "rgb(182,121,213)";
                case "Color-Chronomancer": return "rgb(182,121,213)";
                case "Color-Mirage": return "rgb(182,121,213)";
                case "Color-Necromancer": return "rgb(82,167,111)";
                case "Color-Reaper": return "rgb(82,167,111)";
                case "Color-Scourge": return "rgb(82,167,111)";
                case "Color-Boss": return "rgb(82,167,250)";

                case "Color-Warrior-Total": return "rgb(125,109,66)";
                case "Color-Berserker-Total": return "rgb(125,109,66)";
                case "Color-Spellbreaker-Total": return "rgb(125,109,66)";
                case "Color-Guardian-Total": return "rgb(62,101,113)";
                case "Color-Dragonhunter-Total": return "rgb(62,101,113)";
                case "Color-Firebrand-Total": return "rgb(62,101,113)";
                case "Color-Revenant-Total": return "rgb(110,60,50)";
                case "Color-Herald-Total": return "rgb(110,60,50)";
                case "Color-Renegade-Total": return "rgb(110,60,50)";
                case "Color-Engineer-Total": return "rgb(109,83,48)";
                case "Color-Scrapper-Total": return "rgb(109,83,48)";
                case "Color-Holosmith-Total": return "rgb(109,83,48)";
                case "Color-Ranger-Total": return "rgb(75,115,70)";
                case "Color-Druid-Total": return "rgb(75,115,70)";
                case "Color-Soulbeast-Total": return "rgb(75,115,70)";
                case "Color-Thief-Total": return "rgb(101,76,79)";
                case "Color-Daredevil-Total": return "rgb(101,76,79)";
                case "Color-Deadeye-Total": return "rgb(101,76,79)";
                case "Color-Elementalist-Total": return "rgb(127,74,72)";
                case "Color-Tempest-Total": return "rgb(127,74,72)";
                case "Color-Weaver-Total": return "rgb(127,74,72)";
                case "Color-Mesmer-Total": return "rgb(96,60,111)";
                case "Color-Chronomancer-Total": return "rgb(96,60,111)";
                case "Color-Mirage-Total": return "rgb(96,60,111)";
                case "Color-Necromancer-Total": return "rgb(46,88,60)";
                case "Color-Reaper-Total": return "rgb(46,88,60)";
                case "Color-Scourge-Total": return "rgb(46,88,60)";
                case "Color-Boss-Total": return "rgb(92,177,250)";

                case "Crit":
                    return "https://wiki.guildwars2.com/images/9/95/Critical_Chance.png";
                case "Scholar":
                    return "https://wiki.guildwars2.com/images/thumb/2/2b/Superior_Rune_of_the_Scholar.png/40px-Superior_Rune_of_the_Scholar.png";
                case "SwS":
                    return "https://wiki.guildwars2.com/images/1/1c/Bowl_of_Seaweed_Salad.png";
                case "Downs":
                    return "https://wiki.guildwars2.com/images/c/c6/Downed_enemy.png";
                case "Dead":
                    return "https://wiki.guildwars2.com/images/4/4a/Ally_death_%28interface%29.png";
                case "Flank":
                    return "https://wiki.guildwars2.com/images/thumb/b/bb/Hunter%27s_Tactics.png/40px-Hunter%27s_Tactics.png";
                case "Glance":
                    return "https://wiki.guildwars2.com/images/f/f9/Weakness.png";
                case "Miss":
                    return "https://wiki.guildwars2.com/images/3/33/Blinded.png";
                case "Interupts":
                    return "https://wiki.guildwars2.com/images/thumb/7/79/Daze.png/20px-Daze.png";
                case "Invuln":
                    return "https://wiki.guildwars2.com/images/e/eb/Determined.png";
                case "Blinded":
                    return "https://wiki.guildwars2.com/images/thumb/3/33/Blinded.png/20px-Blinded.png";
                case "Wasted":
                    return "https://wiki.guildwars2.com/images/b/b3/Out_Of_Health_Potions.png";
                case "Saved":
                    return "https://wiki.guildwars2.com/images/e/eb/Ready.png";
                case "Swap":
                    return "https://wiki.guildwars2.com/images/c/ce/Weapon_Swap_Button.png";
                case "Blank":
                    return "https://wiki.guildwars2.com/images/thumb/d/de/Sword_slot.png/40px-Sword_slot.png";
                case "Dodge":
                    return "https://wiki.guildwars2.com/images/c/cc/Dodge_Instructor.png";
                case "Bandage":
                    return "https://render.guildwars2.com/file/D2D7D11874060D68760BFD519CFC77B6DF14981F/102928.png";

                case "Color-Aegis": return "rgb(102,255,255)";
                case "Color-Fury": return "rgb(255,153,0)";
                case "Color-Might": return "rgb(153,0,0)";
                case "Color-Protection": return "rgb(102,255,255)";
                case "Color-Quickness": return "rgb(255,0,255)";
                case "Color-Regeneration": return "rgb(0,204,0)";
                case "Color-Resistance": return "rgb(255, 153, 102)";
                case "Color-Retaliation": return "rgb(255, 51, 0)";
                case "Color-Stability": return "rgb(153, 102, 0)";
                case "Color-Swiftness": return "rgb(255,255,0)";
                case "Color-Vigor": return "rgb(102, 153, 0)";

                case "Color-Alacrity": return "rgb(0,102,255)";
                case "Color-Glyph of Empowerment": return "rgb(204, 153, 0)";
                case "Color-Grace of the Land": return "rgb(,,)";
                case "Color-Sun Spirit": return "rgb(255, 102, 0)";
                case "Color-Banner of Strength": return "rgb(153, 0, 0)";
                case "Color-Banner of Discipline": return "rgb(0, 51, 0)";
                case "Color-Spotter": return "rgb(0,255,0)";
                case "Color-Stone Spirit": return "rgb(204, 102, 0)";
                case "Color-Storm Spirit": return "rgb(102, 0, 102)";
                case "Color-Empower Allies": return "rgb(255, 153, 0)";

                case "Condi": return "https://wiki.guildwars2.com/images/5/54/Condition_Damage.png";
                case "Healing": return "https://wiki.guildwars2.com/images/8/81/Healing_Power.png";
                case "Tough": return "https://wiki.guildwars2.com/images/1/12/Toughness.png";
                default:
                    return "";
            }

        }

    }
}
