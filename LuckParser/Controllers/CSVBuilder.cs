using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;

namespace LuckParser.Controllers
{
    class CSVBuilder
    {
        BossData boss_data;
        Boss boss;
        CombatData combat_data;
        AgentData agent_data;
        List<Player> p_list;
        MechanicData mech_data;
        SkillData skill_data;
        LogData log_data;

        bool[] SnapSettings;

        public CSVBuilder(ParsedLog log)
        {
            boss_data = log.getBossData();
            boss = log.getBoss();
            combat_data = log.getCombatData();
            agent_data = log.getAgentData();
            p_list = log.getPlayerList();
            mech_data = log.getMechanicData();
            skill_data = log.getSkillData();
            log_data = log.getLogData();
        }

        //Creating CSV---------------------------------------------------------------------------------
        public void CreateCSV(StreamWriter sw, String delimiter, bool[] settingsSnap)
        {
            double fight_duration = (boss_data.getAwareDuration()) / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fight_duration);
            String durationString = duration.ToString("mm") + ":" + duration.ToString("ss");
            SnapSettings = settingsSnap;
            HTMLHelper.SnapSettings = settingsSnap;
            sw.Write("Group" + delimiter +
                    "Class" + delimiter +
                    "Character" + delimiter +
                    "Account Name" + delimiter +
                    "Boss DPS" + delimiter +
                    "Boss Physical" + delimiter +
                    "Boss Condi" + delimiter +
                    "All DPS" + delimiter +
                    "Quick" + delimiter +
                    "Alacrity" + delimiter +
                    "Might" + delimiter +
                    "Boss Team DPS" + delimiter +
                    "All Team DPS" + delimiter +
                    "Time" + delimiter +
                    "Cleave" + delimiter +
                    "Team Cleave");
            sw.Write("\r\n");

            int[] teamStats = { 0, 0, 0 };
            foreach (Player p in p_list)
            {
                string[] finaldps = HTMLHelper.getFinalDPS(boss_data, combat_data, agent_data, p, boss, 0).Split('|');
                teamStats[0] += Int32.Parse(finaldps[6]);
                teamStats[1] += Int32.Parse(finaldps[0]);
                teamStats[2] += (Int32.Parse(finaldps[0]) - Int32.Parse(finaldps[6]));
            }

            foreach (Player p in p_list)
            {
                string[] finaldps = HTMLHelper.getFinalDPS(boss_data, combat_data, agent_data, p, boss, 0).Split('|');
                sw.Write(p.getGroup() + delimiter + // group
                        p.getProf() + delimiter +  // class
                        p.getCharacter() + delimiter + // character
                        p.getAccount().Substring(1) + delimiter + // account
                        finaldps[6] + delimiter + // dps
                        finaldps[8] + delimiter + // physical
                        finaldps[10] + delimiter + // condi
                        finaldps[0] + delimiter); // all dps

                Dictionary<int, string> boonArray = HTMLHelper.getfinalboons(boss_data, combat_data, skill_data, agent_data, boss, p, 0);
                sw.Write(boonArray[1187] + delimiter + // Quickness
                        boonArray[30328] + delimiter + // Alacrity
                        boonArray[740] + delimiter); // Might

                sw.Write(teamStats[0] + delimiter  // boss dps
                        + teamStats[1] + delimiter // all
                        + durationString + delimiter + // duration
                    (Int32.Parse(finaldps[0]) - Int32.Parse(finaldps[6])).ToString() + delimiter // cleave
                        + teamStats[2]); // team cleave
                sw.Write("\r\n");
            }
        }
    }
}
