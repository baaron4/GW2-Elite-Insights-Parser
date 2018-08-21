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
        public static void UpdateStatisticSwitches(StatisticsCalculator.Switches switches)
        {
            switches.calculateBoons = switches.calculateBoons || true;
            switches.calculateDPS = switches.calculateDPS || true;
            switches.calculateConditions = switches.calculateConditions || true;
            switches.calculateDefense = switches.calculateDefense || true;
            switches.calculateStats = switches.calculateStats || true;
            switches.calculateSupport = switches.calculateSupport || true;
            switches.calculateCombatReplay = switches.calculateCombatReplay || true;
            switches.calculateMechanics = switches.calculateMechanics || true;
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
            sw.Write(content + delimiter, Encoding.GetEncoding(1252));
        }
        private void WriteCells(List<string> content)
        {
            foreach (string cont in content)
            {
                sw.Write(cont + delimiter, Encoding.GetEncoding(1252));
            }
        }
        private void WriteCells(string[] content)
        {
            foreach (string cont in content)
            {
                sw.Write(cont + delimiter, Encoding.GetEncoding(1252));
            }
        }
        private void NewLine()
        {
            sw.Write("\r\n", Encoding.GetEncoding(1252));
        }
        private void WriteLine(string[] content)
        {
            foreach (string cont in content)
            {
                sw.Write(cont + delimiter, Encoding.GetEncoding(1252));
            }
            NewLine();
        }
        //Creating CSV---------------------------------------------------------------------------------
        public void CreateCSV(StreamWriter sw, String delimiter)
        {
            this.sw = sw;
            this.delimiter = delimiter;

            double fight_duration = (log.GetBossData().GetAwareDuration()) / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fight_duration);
            string durationString = duration.ToString("mm") + "m " + duration.ToString("ss") + "s";
            if (duration.ToString("hh") != "00")
            {
                durationString = duration.ToString("hh") + "h " + durationString;
            }
            string bossname = log.GetBossData().GetName();
            //header
            WriteLine(new string[] { "Elite Insights Version", Application.ProductVersion });
            WriteLine(new string[] { "ARC Version", log.GetLogData().GetBuildVersion().ToString() });
            WriteLine(new string[] { "Boss ID", log.GetBossData().GetID().ToString() });
            WriteLine(new string[] { "Recorded By", log.GetLogData().GetPOV().Split(':')[0] });
            WriteLine(new string[] { "Time Start", log.GetLogData().GetLogStart() });
            WriteLine(new string[] { "Time End", log.GetLogData().GetLogEnd() });
            NewLine();
            NewLine();
            NewLine();
            NewLine();
            //Boss card
            WriteLine(new string[] { "Boss", bossname });
            WriteLine(new string[] { "Success", log.GetLogData().GetBosskill().ToString() });
            WriteLine(new string[] { "Total Boss Health", log.GetBossData().GetHealth().ToString() });
            int finalBossHealth = log.GetBossData().GetHealthOverTime().Count > 0 ? log.GetBossData().GetHealthOverTime().Last().Y : 10000;
            WriteLine(new string[] { "Final Boss Health", (log.GetBossData().GetHealth() * (100.0 - finalBossHealth * 0.01)).ToString() });
            WriteLine(new string[] { "Boss Health Burned %", (100.0 - finalBossHealth * 0.01).ToString() });
            WriteLine(new string[] { "Duration", durationString });

            //DPSStats
            CreateDPSTable(sw, 0);

            //DMGStatsBoss
            CreateBossDMGStatsTable(sw, 0);

            //DMGStats All
            CreateDMGStatsTable(sw, 0);

            //Defensive Stats
            CreateDefTable(sw, 0);

            //Support Stats
            CreateSupTable(sw, 0);

            // Html_boons
            CreateUptimeTable(sw, statistics.present_boons, 0);

            //Html_boonGenSelf
            CreateGenSelfTable(sw, statistics.present_boons, 0);

            // Html_boonGenGroup
            CreateGenGroupTable(sw, statistics.present_boons, 0);

            // Html_boonGenOGroup
            CreateGenOGroupTable(sw, statistics.present_boons, 0);

            //  Html_boonGenSquad
            CreateGenSquadTable(sw, statistics.present_boons, 0);

            //Offensive Buffs stats
            // Html_boons
            CreateUptimeTable(sw, statistics.present_offbuffs, 0);

            //Html_boonGenSelf
            CreateGenSelfTable(sw, statistics.present_offbuffs, 0);

            // Html_boonGenGroup
            CreateGenGroupTable(sw, statistics.present_offbuffs, 0);

            // Html_boonGenOGroup
            CreateGenOGroupTable(sw, statistics.present_offbuffs, 0);

            //  Html_boonGenSquad
            CreateGenSquadTable(sw, statistics.present_offbuffs, 0);

            //Defensive Buffs stats
            // Html_boons
            CreateUptimeTable(sw, statistics.present_defbuffs, 0);

            //Html_boonGenSelf
            CreateGenSelfTable(sw, statistics.present_defbuffs, 0);

            // Html_boonGenGroup
            CreateGenGroupTable(sw, statistics.present_defbuffs, 0);

            // Html_boonGenOGroup
            CreateGenOGroupTable(sw, statistics.present_defbuffs, 0);

            //  Html_boonGenSquad
            CreateGenSquadTable(sw, statistics.present_defbuffs, 0);

            //Mechanics
            CreateMechanicTable(sw, 0);

            //Mech List
            CreateMechList(sw, 0);

            //Condi Uptime
            CreateCondiUptime(sw, 0);
            //Condi Gen
            CreateCondiGen(sw, 0);
            //Boss boons
            CreateBossBoonUptime(sw, 0);
        }
        private void CreateDPSTable(StreamWriter sw, int phase_index)
        {
            PhaseData phase = statistics.phases[phase_index];
            if (phase.GetRedirection().Count > 0)
            {
                WriteLine(new string[] { "Sub Group", "Profession","Role","Name","Account","WepSet1_1","WepSet1_2","WepSet2_1","WepSet2_2",
                "Adds DPS","Adds DMG","Adds Power DPS","Adds Power DMG","Adds Condi DPS","Adds Condi DMG",
                "All DPS","All DMG","All Power DPS","All Power DMG","All Condi DPS","All Condi DMG",
                "Times Downed", "Time Died","Percent Alive"});
            }
            else
            {
                WriteLine(new string[] { "Sub Group", "Profession","Role","Name","Account","WepSet1_1","WepSet1_2","WepSet2_1","WepSet2_2",
                "Boss DPS","Boss DMG","Boss Power DPS","Boss Power DMG","Boss Condi DPS","Boss Condi DMG",
                "All DPS","All DMG","All Power DPS","All Power DMG","All Condi DPS","All Condi DMG",
                "Times Downed", "Time Died","Percent Alive"});
            }
            int count = 0;
            foreach (Player player in log.GetPlayerList())
            {
                Statistics.FinalDPS dps = statistics.dps[player][phase_index];
                Statistics.FinalStats stats = statistics.stats[player][phase_index];
                TimeSpan timedead = TimeSpan.FromMilliseconds(stats.died);
                long fight_duration = phase.GetDuration("s");
                string[] wep = player.GetWeaponsArray(log);
                string build = "";
                if (player.GetCondition() > 0)
                {
                    build += " Condi:" + player.GetCondition();
                }
                if (player.GetConcentration() > 0)
                {
                    build += " Concentration:" + player.GetConcentration();
                }
                if (player.GetHealing() > 0)
                {
                    build += " Healing:" + player.GetHealing();
                }
                if (player.GetToughness() > 0)
                {
                    build += " Toughness:" + player.GetToughness();
                }
                WriteLine(new string[] { player.GetGroup().ToString(), player.GetProf().ToString(),build,player.GetCharacter().ToString(), player.GetAccount().TrimStart(':') ,wep[0],wep[1],wep[2],wep[3],
                dps.bossDps.ToString(),dps.bossDamage.ToString(),dps.bossPowerDps.ToString(),dps.bossPowerDamage.ToString(),dps.bossCondiDps.ToString(),dps.bossCondiDamage.ToString(),
                dps.allDps.ToString(),dps.allDamage.ToString(),dps.allPowerDps.ToString(),dps.allPowerDamage.ToString(),dps.allCondiDps.ToString(),dps.allCondiDamage.ToString(),
                stats.downCount.ToString(), timedead.Minutes + " m " + timedead.Seconds + " s",Math.Round((timedead.TotalSeconds / fight_duration) * 100,1) +"%"});
                count++;
            }
            while (count < 15)//so each graph has equal spaceing
            {
                NewLine();
                count++;
            }
        }
        private void CreateBossDMGStatsTable(StreamWriter sw, int phase_index)
        {
            //generate dmgstats table
            PhaseData phase = statistics.phases[phase_index];
            WriteLine(new string[] { "Sub Group", "Profession", "Name" ,
                "Critical%","Critical hits","Critical DMG",
                "Scholar%","Scholar hits","Scholar DMG","Scholar % increase",
                "Moving%","Moving Hits","Moving DMG","Moving % increase",
                "Flanking%","Flanking hits",
                "Glancing%","Glancing Hits",
                "Blind%","Blind Hits",
                "Total Hits",
                "Hits to Interupt","Hits Invulned","Time wasted","Time saved","Weapon Swaps"});
            int count = 0;
            foreach (Player player in log.GetPlayerList())
            {
                Statistics.FinalStats stats = statistics.stats[player][phase_index];
                Statistics.FinalDPS dps = statistics.dps[player][phase_index];

                WriteLine(new string[] { player.GetGroup().ToString(), player.GetProf().ToString(), player.GetCharacter().ToString(),
                Math.Round((Double)(stats.criticalRateBoss) / stats.critablePowerLoopCountBoss * 100,1).ToString(), stats.criticalRateBoss.ToString(),stats.criticalDmgBoss.ToString(),
                Math.Round((Double)(stats.scholarRateBoss) / stats.powerLoopCountBoss * 100,1).ToString(),stats.scholarRateBoss.ToString(),stats.scholarDmgBoss.ToString(),Math.Round(100.0 * (dps.playerBossPowerDamage / (Double)(dps.playerBossPowerDamage - stats.scholarDmgBoss) - 1.0), 3).ToString(),
                Math.Round((Double)(stats.movingRateBoss) / stats.powerLoopCountBoss * 100,1).ToString(),stats.movingRateBoss.ToString(),stats.movingDamageBoss.ToString(),Math.Round(100.0 * (dps.playerBossPowerDamage / (Double)(dps.playerBossPowerDamage - stats.movingDamageBoss) - 1.0), 3).ToString(),
                Math.Round(stats.flankingRateBoss / (Double)stats.powerLoopCountBoss * 100,1).ToString(),stats.flankingRateBoss.ToString(),
                Math.Round(stats.glanceRateBoss / (Double)stats.powerLoopCountBoss * 100,1).ToString(),stats.glanceRateBoss.ToString(),
                Math.Round(stats.missedBoss / (Double)stats.powerLoopCountBoss * 100,1).ToString(),stats.missedBoss.ToString(),
                stats.powerLoopCountBoss.ToString(),
                stats.interuptsBoss.ToString(),stats.invulnedBoss.ToString(),stats.timeWasted.ToString(),stats.timeSaved.ToString(),stats.swapCount.ToString() });
                count++;
            }
            while (count < 15)//so each graph has equal spaceing
            {
                NewLine();
                count++;
            }
        }
        private void CreateDMGStatsTable(StreamWriter sw, int phase_index)
        {
            //generate dmgstats table
            PhaseData phase = statistics.phases[phase_index];
            WriteLine(new string[] { "Sub Group", "Profession", "Name" ,
                "Critical%","Critical hits","Critical DMG",
                "Scholar%","Scholar hits","Scholar DMG","Scholar % increase",
                "Moving%","Moving Hits","Moving DMG","Moving % increase",
                "Flanking%","Flanking hits",
                "Glancing%","Glancing Hits",
                "Blind%","Blind Hits",
                "Total Hits",
                "Hits to Interupt","Hits Invulned","Time wasted","Time saved","Weapon Swaps"});
            int count = 0;
            foreach (Player player in log.GetPlayerList())
            {
                Statistics.FinalStats stats = statistics.stats[player][phase_index];
                Statistics.FinalDPS dps = statistics.dps[player][phase_index];

                WriteLine(new string[] { player.GetGroup().ToString(), player.GetProf().ToString(), player.GetCharacter().ToString(),
                Math.Round((Double)(stats.criticalRate) / stats.critablePowerLoopCount * 100,1).ToString(), stats.criticalRate.ToString(),stats.criticalDmg.ToString(),
                Math.Round((Double)(stats.scholarRate) / stats.powerLoopCount * 100,1).ToString(),stats.scholarRate.ToString(),stats.scholarDmg.ToString(),Math.Round(100.0 * (dps.playerPowerDamage / (Double)(dps.playerPowerDamage - stats.scholarDmg) - 1.0), 3).ToString(),
                Math.Round((Double)(stats.movingRate) / stats.powerLoopCount * 100,1).ToString(),stats.movingRate.ToString(),stats.movingDamage.ToString(),Math.Round(100.0 * (dps.playerPowerDamage / (Double)(dps.playerPowerDamage - stats.movingDamage) - 1.0), 3).ToString(),
                Math.Round(stats.flankingRate / (Double)stats.powerLoopCount * 100,1).ToString(),stats.flankingRate.ToString(),
                Math.Round(stats.glanceRate / (Double)stats.powerLoopCount * 100,1).ToString(),stats.glanceRate.ToString(),
                Math.Round(stats.missed / (Double)stats.powerLoopCount * 100,1).ToString(),stats.missed.ToString(),
                stats.powerLoopCount.ToString(),
                stats.interupts.ToString(),stats.invulned.ToString(),stats.timeWasted.ToString(),stats.timeSaved.ToString(),stats.swapCount.ToString() });
                count++;
            }
            while (count < 15)//so each graph has equal spaceing
            {
                NewLine();
                count++;
            }
        }
        private void CreateDefTable(StreamWriter sw, int phase_index)
        {
            //generate deftats table
            PhaseData phase = statistics.phases[phase_index];
            WriteLine(new string[] { "Sub Group", "Profession", "Name" ,
                "DMG Taken","DMG Barrier","Blocked","Invulned","Evaded","Dodges" });
            int count = 0;
            foreach (Player player in log.GetPlayerList())
            {
                Statistics.FinalDefenses defenses = statistics.defenses[player][phase_index];
                Statistics.FinalStats stats = statistics.stats[player][phase_index];

                WriteLine(new string[] { player.GetGroup().ToString(), player.GetProf().ToString(), player.GetCharacter().ToString(),
                defenses.damageTaken.ToString(),defenses.damageBarrier.ToString(),defenses.blockedCount.ToString(),defenses.invulnedCount.ToString(),defenses.evadedCount.ToString(),stats.dodgeCount.ToString() });
                count++;
            }
            while (count < 15)//so each graph has equal spaceing
            {
                NewLine();
                count++;
            }
        }
        private void CreateSupTable(StreamWriter sw, int phase_index)
        {
            //generate supstats table
            PhaseData phase = statistics.phases[phase_index];
            WriteLine(new string[] { "Sub Group", "Profession", "Name" ,
                "Condi Cleanse","Condi Cleanse time","Resurrects","Time Resurecting" });
            int count = 0;
            foreach (Player player in log.GetPlayerList())
            {
                Statistics.FinalSupport support = statistics.support[player][phase_index];

                WriteLine(new string[] { player.GetGroup().ToString(), player.GetProf().ToString(), player.GetCharacter().ToString(),
                support.condiCleanse.ToString(),support.condiCleanseTime.ToString(),support.resurrects.ToString(),support.ressurrectTime.ToString() });
                count++;
            }
            while (count < 15)//so each graph has equal spaceing
            {
                NewLine();
                count++;
            }
        }
        private void CreateUptimeTable(StreamWriter sw, List<Boon> list_to_use, int phase_index)
        {
            //generate Uptime Table table
            List<PhaseData> phases = statistics.phases;
            PhaseData phase = statistics.phases[phase_index];
            long fight_duration = phases[phase_index].GetDuration();

            WriteCells(new string[] { "Name", "Avg Boons" });
            foreach (Boon boon in list_to_use)
            {
                WriteCell(boon.GetName());

            }
            NewLine();

            int count = 0;
            foreach (Player player in log.GetPlayerList())
            {
                Dictionary<long, Statistics.FinalBoonUptime> boons = statistics.selfBoons[player][phase_index];
                Dictionary<long, long> boonPresence = player.GetBoonPresence(log, phases, list_to_use, phase_index);
                double avg_boons = 0.0;
                foreach (long duration in boonPresence.Values)
                {
                    avg_boons += duration;
                }
                avg_boons /= fight_duration;

                WriteCell(player.GetCharacter());
                WriteCell(Math.Round(avg_boons, 1).ToString());
                foreach (Boon boon in list_to_use)
                {
                    if (boons.ContainsKey(boon.GetID()))
                    {

                        if (boon.GetBoonType() == Boon.BoonType.Duration)
                        {
                            WriteCell(boons[boon.GetID()].uptime + "%");
                        }
                        else if (boon.GetBoonType() == Boon.BoonType.Intensity)
                        {
                            WriteCell(boons[boon.GetID()].uptime.ToString());
                        }

                    }
                    else
                    {
                        WriteCell("0");

                    }
                }
                NewLine();
                count++;
            }
            while (count < 15)//so each graph has equal spaceing
            {
                NewLine();
                count++;
            }
        }
        private void CreateGenSelfTable(StreamWriter sw, List<Boon> list_to_use, int phase_index)
        {
            //generate Uptime Table table
            PhaseData phase = statistics.phases[phase_index];

            WriteCell("Name");
            foreach (Boon boon in list_to_use)
            {
                WriteCell(boon.GetName());
                WriteCell(boon.GetName() + " Overstack");
            }
            NewLine();

            int count = 0;
            foreach (Player player in log.GetPlayerList())
            {
                Dictionary<long, Statistics.FinalBoonUptime> uptimes = statistics.selfBoons[player][phase_index];

                Dictionary<int, string> rates = new Dictionary<int, string>();


                WriteCell(player.GetCharacter());
                foreach (Boon boon in list_to_use)
                {
                    string rate = "0";
                    string overstack = "0";
                    Statistics.FinalBoonUptime uptime = uptimes[boon.GetID()];
                    if (uptime.generation > 0)
                    {
                        if (boon.GetBoonType() == Boon.BoonType.Duration)
                        {
                            rate = uptime.generation.ToString() + "%";
                            overstack = uptime.overstack.ToString() + "%";
                        }
                        else if (boon.GetBoonType() == Boon.BoonType.Intensity)
                        {
                            rate = uptime.generation.ToString();
                            overstack = uptime.overstack.ToString();
                        }

                    }
                    WriteCell(rate);
                    WriteCell(overstack);
                }
                NewLine();
                count++;
            }
            while (count < 15)//so each graph has equal spaceing
            {
                NewLine();
                count++;
            }
        }
        private void CreateGenGroupTable(StreamWriter sw, List<Boon> list_to_use, int phase_index)
        {
            //generate Uptime Table table
            PhaseData phase = statistics.phases[phase_index];

            WriteCell("Name");
            foreach (Boon boon in list_to_use)
            {
                WriteCell(boon.GetName());
                WriteCell(boon.GetName() + " Overstack");
            }
            NewLine();

            int count = 0;
            foreach (Player player in log.GetPlayerList())
            {
                Dictionary<long, Statistics.FinalBoonUptime> boons =
                            statistics.groupBoons[player][phase_index];

                Dictionary<int, string> rates = new Dictionary<int, string>();


                WriteCell(player.GetCharacter());
                foreach (Boon boon in list_to_use)
                {
                    string rate = "0";
                    string overstack = "0";
                    Statistics.FinalBoonUptime uptime = boons[boon.GetID()];
                    if (uptime.generation > 0)
                    {
                        if (boon.GetBoonType() == Boon.BoonType.Duration)
                        {
                            rate = uptime.generation.ToString() + "%";
                            overstack = uptime.overstack.ToString() + "%";
                        }
                        else if (boon.GetBoonType() == Boon.BoonType.Intensity)
                        {
                            rate = uptime.generation.ToString();
                            overstack = uptime.overstack.ToString();
                        }

                    }
                    WriteCell(rate);
                    WriteCell(overstack);
                }
                NewLine();
                count++;
            }
            while (count < 15)//so each graph has equal spaceing
            {
                NewLine();
                count++;
            }
        }
        private void CreateGenOGroupTable(StreamWriter sw, List<Boon> list_to_use, int phase_index)
        {
            //generate Uptime Table table
            PhaseData phase = statistics.phases[phase_index];

            WriteCell("Name");
            foreach (Boon boon in list_to_use)
            {
                WriteCell(boon.GetName());
                WriteCell(boon.GetName() + " Overstack");
            }
            NewLine();

            int count = 0;
            foreach (Player player in log.GetPlayerList())
            {
                Dictionary<long, Statistics.FinalBoonUptime> boons =
                              statistics.offGroupBoons[player][phase_index];

                Dictionary<int, string> rates = new Dictionary<int, string>();


                WriteCell(player.GetCharacter());
                foreach (Boon boon in list_to_use)
                {
                    string rate = "0";
                    string overstack = "0";
                    Statistics.FinalBoonUptime uptime = boons[boon.GetID()];
                    if (uptime.generation > 0)
                    {
                        if (boon.GetBoonType() == Boon.BoonType.Duration)
                        {
                            rate = uptime.generation.ToString() + "%";
                            overstack = uptime.overstack.ToString() + "%";
                        }
                        else if (boon.GetBoonType() == Boon.BoonType.Intensity)
                        {
                            rate = uptime.generation.ToString();
                            overstack = uptime.overstack.ToString();
                        }

                    }
                    WriteCell(rate);
                    WriteCell(overstack);
                }
                NewLine();
                count++;
            }
            while (count < 15)//so each graph has equal spaceing
            {
                NewLine();
                count++;
            }
        }
        private void CreateGenSquadTable(StreamWriter sw, List<Boon> list_to_use, int phase_index)
        {
            //generate Uptime Table table
            PhaseData phase = statistics.phases[phase_index];

            WriteCell("Name");
            foreach (Boon boon in list_to_use)
            {
                WriteCell(boon.GetName());
                WriteCell(boon.GetName() + " Overstack");
            }
            NewLine();

            int count = 0;
            foreach (Player player in log.GetPlayerList())
            {
                Dictionary<long, Statistics.FinalBoonUptime> boons =
                            statistics.squadBoons[player][phase_index];

                Dictionary<int, string> rates = new Dictionary<int, string>();


                WriteCell(player.GetCharacter());
                foreach (Boon boon in list_to_use)
                {
                    string rate = "0";
                    string overstack = "0";
                    Statistics.FinalBoonUptime uptime = boons[boon.GetID()];
                    if (uptime.generation > 0)
                    {
                        if (boon.GetBoonType() == Boon.BoonType.Duration)
                        {
                            rate = uptime.generation.ToString() + "%";
                            overstack = uptime.overstack.ToString() + "%";
                        }
                        else if (boon.GetBoonType() == Boon.BoonType.Intensity)
                        {
                            rate = uptime.generation.ToString();
                            overstack = uptime.overstack.ToString();
                        }

                    }
                    WriteCell(rate);
                    WriteCell(overstack);
                }
                NewLine();
                count++;
            }
            while (count < 15)//so each graph has equal spaceing
            {
                NewLine();
                count++;
            }
        }
        private void CreateMechanicTable(StreamWriter sw, int phase_index)
        {
            HashSet<Mechanic> presMech = log.GetMechanicData().GetPresentPlayerMechs(phase_index);
            //Dictionary<string, HashSet<Mechanic>> presEnemyMech = log.getMechanicData().getPresentEnemyMechs(phase_index);
            PhaseData phase = statistics.phases[phase_index];
            //List<AbstractMasterPlayer> enemyList = log.getMechanicData().getEnemyList(phase_index);
            int countLines = 0;
            if (presMech.Count > 0)
            {
                WriteCell("Name");
                foreach (Mechanic mech in presMech)
                {
                    WriteCell(mech.GetName());
                }
                NewLine();

                foreach (Player p in log.GetPlayerList())
                {
                    WriteCell(p.GetCharacter());
                    foreach (Mechanic mech in presMech)
                    {
                        int count = log.GetMechanicData()[mech].Count(x => x.GetPlayer().GetInstid() == p.GetInstid() && phase.InInterval(x.GetTime()));
                        WriteCell(count.ToString());
                    }
                    NewLine();
                    countLines++;

                }

            }
            while (countLines < 15)//so each graph has equal spaceing
            {
                NewLine();
                countLines++;
            }
        }
        private void CreateMechList(StreamWriter sw, int phase_index)
        {
            PhaseData phase = statistics.phases[phase_index];

            MechanicData m_Data = log.GetMechanicData();
            List<MechanicLog> m_Logs = new List<MechanicLog>();
            foreach (List<MechanicLog> mLogs in m_Data.Values)
            {
                m_Logs.AddRange(mLogs);
            }
            m_Logs.OrderBy(x => x.GetTime());
            int count = 0;
            WriteCell("Time");
            foreach (MechanicLog m in m_Logs)
            {
                WriteCell((m.GetTime() / 1000f).ToString());
            }
            NewLine();
            count++;
            WriteCell("Player");
            foreach (MechanicLog m in m_Logs)
            {
                WriteCell(m.GetPlayer().GetCharacter().ToString());
            }
            NewLine();
            count++;
            WriteCell("Mechanic");
            foreach (MechanicLog m in m_Logs)
            {
                WriteCell(m.GetName().ToString());
            }
            NewLine();
            count++;
            while (count < 15)//so each graph has equal spaceing
            {
                NewLine();
                count++;
            }
        }
        private void CreateCondiUptime(StreamWriter sw, int phase_index)
        {
            Boss boss = log.GetBoss();
            List<PhaseData> phases = statistics.phases;
            long fight_duration = phases[phase_index].GetDuration();
            Dictionary<long, Statistics.FinalBossBoon> conditions = statistics.bossConditions[phase_index];
            List<Boon> boon_to_track = Boon.GetCondiBoonList();
            boon_to_track.AddRange(Boon.GetBoonList());
            Dictionary<long, long> condiPresence = boss.GetCondiPresence(log, phases, boon_to_track, phase_index);
            Dictionary<long, long> boonPresence = boss.GetBoonPresence(log, phases, boon_to_track, phase_index);
            double avg_condis = 0.0;
            foreach (long duration in condiPresence.Values)
            {
                avg_condis += duration;
            }
            avg_condis /= fight_duration;


            WriteCell("Name");
            WriteCell("Avg");
            foreach (Boon boon in Boon.GetCondiBoonList())
            {
                if (boon.GetName() == "Retaliation")
                {
                    continue;
                }
                WriteCell(boon.GetName());
            }

            NewLine();
            int count = 0;
            WriteCell(boss.GetCharacter());
            WriteCell(Math.Round(avg_condis, 1).ToString());
            foreach (Boon boon in Boon.GetCondiBoonList())
            {
                if (boon.GetName() == "Retaliation")
                {
                    continue;
                }
                if (boon.GetBoonType() == Boon.BoonType.Duration)
                {
                    WriteCell(conditions[boon.GetID()].uptime.ToString() + "%");
                }
                else
                {
                    WriteCell(conditions[boon.GetID()].uptime.ToString());
                }
            }
            count++;

            while (count < 15)//so each graph has equal spaceing
            {
                NewLine();
                count++;
            }
        }
        private void CreateBossBoonUptime(StreamWriter sw, int phase_index)
        {
            Boss boss = log.GetBoss();
            List<PhaseData> phases = statistics.phases;
            long fight_duration = phases[phase_index].GetDuration();
            Dictionary<long, Statistics.FinalBossBoon> conditions = statistics.bossConditions[phase_index];
            WriteCell("Name");
            WriteCell("Avg");
            foreach (Boon boon in Boon.GetBoonList())
            {
                WriteCell(boon.GetName());
            }

            NewLine();
            int count = 0;
            WriteCell(boss.GetCharacter());
            foreach (Boon boon in Boon.GetBoonList())
            {
                if (boon.GetBoonType() == Boon.BoonType.Duration)
                {
                    WriteCell(conditions[boon.GetID()].uptime.ToString() + "%");
                }
                else
                {
                    WriteCell(conditions[boon.GetID()].uptime.ToString());
                }
            }
            count++;

            while (count < 15)//so each graph has equal spaceing
            {
                NewLine();
                count++;
            }
        }
        private void CreateCondiGen(StreamWriter sw, int phase_index)
        {
            List<PhaseData> phases = statistics.phases;
            long fight_duration = phases[phase_index].GetDuration();
            Dictionary<long, Statistics.FinalBossBoon> conditions = statistics.bossConditions[phase_index];
            //bool hasBoons = false;
            int count = 0;
            WriteCell("Name");
            foreach (Boon boon in Boon.GetCondiBoonList())
            {
                if (boon.GetName() == "Retaliation")
                {
                    continue;
                }
                WriteCell(boon.GetName());
                WriteCell(boon.GetName() + " Overstack");
            }
            NewLine();
            foreach (Player player in log.GetPlayerList())
            {
                WriteCell(player.GetCharacter());
                foreach (Boon boon in Boon.GetCondiBoonList())
                {
                    if (boon.GetName() == "Retaliation")
                    {
                        continue;
                    }
                    if (boon.GetBoonType() == Boon.BoonType.Duration)
                    {
                        WriteCell(conditions[boon.GetID()].generated[player].ToString() + "%");
                        WriteCell(conditions[boon.GetID()].overstacked[player].ToString() + "%");
                    }
                    else
                    {
                        WriteCell(conditions[boon.GetID()].generated[player].ToString());
                        WriteCell(conditions[boon.GetID()].overstacked[player].ToString());
                    }
                }
                NewLine();
                count++;
            }


            while (count < 15)//so each graph has equal spaceing
            {
                NewLine();
                count++;
            }
        }
    }
}
