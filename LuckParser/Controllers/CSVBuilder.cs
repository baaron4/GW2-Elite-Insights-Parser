using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;

namespace LuckParser.Controllers
{
    class CSVBuilder
    {
        private SettingsContainer settings;

        private ParsedLog log;
        //private BossData boss_data;
      //  private List<Player> p_list;

        private Statistics statistics;
        private StreamWriter sw;
        private string delimiter;
        public static StatisticsCalculator.Switches GetStatisticSwitches()
        {
            StatisticsCalculator.Switches switches = new StatisticsCalculator.Switches();
            switches.calculateBoons = true;
            switches.calculateDPS = true;
            switches.calculateConditions = true;
            switches.calculateDefense = true;
            switches.calculateStats = true;
            switches.calculateSupport = true;
            switches.calculateCombatReplay = true;
            return switches;
        }

        public CSVBuilder(ParsedLog log, SettingsContainer settings, Statistics statistics)
        {
            this.log = log;

            this.settings = settings;
            CSVHelper.settings = settings;

            this.statistics = statistics;
        }
        private void WriteCell(string content)
        {
            sw.Write(content + delimiter);
        }
        private void WriteCells(List<string> content )
        {
            foreach (string cont in content)
            {
                sw.Write(cont + delimiter);
            }
        }
        private void WriteCells(string[] content)
        {
            foreach (string cont in content)
            {
                sw.Write(cont + delimiter);
            }
        }
        private void NewLine()
        {
            sw.Write("\r\n");
        }
        private void WriteLine(string[] content)
        {
            foreach (string cont in content)
            {
                sw.Write(cont + delimiter);
            }
            NewLine();
        }
        //Creating CSV---------------------------------------------------------------------------------
        public void CreateCSV(StreamWriter sw, String delimiter)
        {
            this.sw = sw;
            this.delimiter = delimiter;

            double fight_duration = (log.getBossData().getAwareDuration()) / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fight_duration);
            string durationString = duration.ToString("mm") + "m " + duration.ToString("ss") + "s";
            if (duration.ToString("hh") != "00")
            {
                durationString = duration.ToString("hh") + "h " + durationString;
            }
            string bossname = log.getBossData().getName();
            List<PhaseData> phases = log.getBoss().getPhases(log, settings.ParsePhases);
            //header
            WriteLine(new string[] { "Elite Insights Version", Application.ProductVersion });
            WriteLine(new string[] { "ARC Version", log.getLogData().getBuildVersion().ToString() });
            WriteLine(new string[] { "Boss ID", log.getBossData().getID().ToString() });
            WriteLine(new string[] { "Recorded By", log.getLogData().getPOV().Split(':')[0] });
            WriteLine(new string[] { "Time Start", log.getLogData().getLogStart()});
            WriteLine(new string[] { "Time End", log.getLogData().getLogEnd() });
            NewLine();
            NewLine();
            NewLine();
            NewLine();
            //Boss card
            WriteLine(new string[] { "Boss", bossname });
            WriteLine(new string[] { "Success", log.getLogData().getBosskill().ToString() });
            WriteLine(new string[] { "Total Boss Health", log.getBossData().getHealth().ToString() });
            int finalBossHealth = log.getBossData().getHealthOverTime()[log.getBossData().getHealthOverTime().Count - 1].Y;
            WriteLine(new string[] { "Final Boss Health",finalBossHealth.ToString() });
            WriteLine(new string[] { "Final Boss Health %", (100.0 -finalBossHealth * 0.01).ToString() });
            WriteLine(new string[] { "Duration", durationString });

            //DPSStats
            CreateDPSTable(sw, 0);
        }
        private void CreateDPSTable(StreamWriter sw, int phase_index)
        {
            PhaseData phase = log.getBoss().getPhases(log, settings.ParsePhases)[phase_index];
            WriteLine(new string[] { "Sub Group", "Profession","WepSet1_1","WepSet1_2","WepSet2_1","WepSet2_2","Name","Account",
                "Boss DPS","Boss DMG","Boss Power DPS","Boss Power DMG","Boss Condi DPS","Boss Condi DMG",
                "All DPS","All DMG","All Power DPS","All Power DMG","All Condi DPS","All Condi DMG",
                "Times Downed", "Time Died","Percent Alive"});
            foreach (Player player in log.getPlayerList())
            {
                Statistics.FinalDPS dps = statistics.dps[player][phase_index];
                Statistics.FinalStats stats = statistics.stats[player][phase_index];
                TimeSpan timedead = TimeSpan.FromMilliseconds(stats.died);
                long fight_duration = phase.getDuration("s");

                string[] wep = player.getWeaponsArray(log);
                WriteLine(new string[] { player.getGroup().ToString(), player.getProf().ToString(),wep[0],wep[1],wep[2],wep[3], player.getCharacter().ToString(), player.getAccount().TrimStart(':') ,
                dps.bossDps.ToString(),dps.bossDamage.ToString(),dps.bossPowerDps.ToString(),dps.bossPowerDamage.ToString(),dps.bossCondiDps.ToString(),dps.bossCondiDamage.ToString(),
                dps.allDps.ToString(),dps.allDamage.ToString(),dps.allPowerDps.ToString(),dps.allPowerDamage.ToString(),dps.allCondiDps.ToString(),dps.allCondiDamage.ToString(),
                stats.downCount.ToString(), timedead.Minutes + " m " + timedead.Seconds + " s",Math.Round((timedead.TotalSeconds / fight_duration) * 100,1) +"%"});
            }
        }
    }
}
