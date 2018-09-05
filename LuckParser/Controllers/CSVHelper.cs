using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace LuckParser.Controllers
{
    class CSVHelper
    {

        public static void WriteOldCSV(StreamWriter sw, string delimiter,ParsedLog log,Statistics statistics)
        {
            double fightDuration = (log.FightData.FightDuration) / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fightDuration);
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
            foreach (Player p in log.PlayerList)
            {
                Statistics.FinalDPS dps = statistics.Dps[p][0];
                teamStats[0] += dps.BossDps;
                teamStats[1] += dps.AllDps;
                teamStats[2] += dps.AllDps - dps.BossDps;
            }

            foreach (Player p in log.PlayerList)
            {
                Statistics.FinalDPS dps = statistics.Dps[p][0];
                sw.Write(p.Group + delimiter + // group
                        p.Prof + delimiter +  // class
                        p.Character + delimiter + // character
                        p.Account.Substring(1) + delimiter + // account
                        dps.BossDps + delimiter + // dps
                        dps.BossPowerDps + delimiter + // physical
                        dps.BossCondiDps + delimiter + // condi
                        dps.AllDps + delimiter); // all dps

                Dictionary<long, Statistics.FinalBoonUptime> boons = statistics.SelfBoons[p][0];
                sw.Write(boons[1187].Uptime + delimiter + // Quickness
                         boons[30328].Uptime + delimiter + // Alacrity
                         boons[740].Uptime + delimiter); // Might

                sw.Write(teamStats[0] + delimiter  // boss dps
                        + teamStats[1] + delimiter // all
                        + durationString + delimiter + // duration
                          (dps.AllDps - dps.BossDps).ToString() + delimiter // cleave
                        + teamStats[2]); // team cleave
                sw.Write("\r\n");
            }
        }
    }

   
}
