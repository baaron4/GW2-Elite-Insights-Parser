using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Controllers
{
    class CSVHelper
    {
        public static SettingsContainer settings;

        public static void writeOldCSV(StreamWriter sw, string delimiter,ParsedLog log,Statistics statistics)
        {
            double fight_duration = (log.getBossData().getAwareDuration()) / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fight_duration);
            String durationString = duration.ToString("mm") + ":" + duration.ToString("ss");
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
            foreach (Player p in log.getPlayerList())
            {
                Statistics.FinalDPS dps = statistics.dps[p][0];
                teamStats[0] += dps.bossDps;
                teamStats[1] += dps.allDps;
                teamStats[2] += dps.allDps - dps.bossDps;
            }

            foreach (Player p in log.getPlayerList())
            {
                Statistics.FinalDPS dps = statistics.dps[p][0];
                sw.Write(p.getGroup() + delimiter + // group
                        p.getProf() + delimiter +  // class
                        p.getCharacter() + delimiter + // character
                        p.getAccount().Substring(1) + delimiter + // account
                        dps.bossDps + delimiter + // dps
                        dps.bossPowerDps + delimiter + // physical
                        dps.bossCondiDps + delimiter + // condi
                        dps.allDps + delimiter); // all dps

                Dictionary<int, Statistics.FinalBoonUptime> boons = statistics.selfBoons[p][0];
                sw.Write(boons[1187].uptime + delimiter + // Quickness
                         boons[30328].uptime + delimiter + // Alacrity
                         boons[740].uptime + delimiter); // Might

                sw.Write(teamStats[0] + delimiter  // boss dps
                        + teamStats[1] + delimiter // all
                        + durationString + delimiter + // duration
                          (dps.allDps - dps.bossDps).ToString() + delimiter // cleave
                        + teamStats[2]); // team cleave
                sw.Write("\r\n");
            }
        }
    }

   
}
