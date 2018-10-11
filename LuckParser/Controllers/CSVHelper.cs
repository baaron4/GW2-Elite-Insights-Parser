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
            string durationString = duration.ToString("mm") + ":" + duration.ToString("ss");
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
                Statistics.FinalDPS dps = statistics.DpsAll[p][0];
                Statistics.FinalDPS dpsBoss = statistics.DpsBoss[log.Boss][p][0];
                teamStats[0] += dps.Dps;
                teamStats[1] += dpsBoss.Dps;
                teamStats[2] += dps.Dps - dpsBoss.Dps;
            }

            foreach (Player p in log.PlayerList)
            {
                Statistics.FinalDPS dpsBoss = statistics.DpsBoss[log.Boss][p][0];
                Statistics.FinalDPS dpsAll = statistics.DpsAll[p][0];
                sw.Write(p.Group + delimiter + // group
                        p.Prof + delimiter +  // class
                        p.Character + delimiter + // character
                        p.Account.Substring(1) + delimiter + // account
                        dpsBoss.Dps + delimiter + // dps
                        dpsBoss.PowerDps + delimiter + // physical
                        dpsBoss.CondiDps + delimiter + // condi
                        dpsAll.Dps + delimiter); // all dps

                Dictionary<long, Statistics.FinalBoonUptime> boons = statistics.SelfBoons[p][0];
                sw.Write(boons[1187].Uptime + delimiter + // Quickness
                         boons[30328].Uptime + delimiter + // Alacrity
                         boons[740].Uptime + delimiter); // Might

                sw.Write(teamStats[0] + delimiter  // boss dps
                        + teamStats[1] + delimiter // all
                        + durationString + delimiter + // duration
                          (dpsAll.Dps - dpsBoss.Dps).ToString() + delimiter // cleave
                        + teamStats[2]); // team cleave
                sw.Write("\r\n");
            }
        }
    }

   
}
