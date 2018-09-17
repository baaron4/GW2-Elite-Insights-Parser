using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using LuckParser.Models.DataModels;
using LuckParser.Models.HtmlModels;
using LuckParser.Models.ParseModels;

namespace LuckParser.Controllers
{
    class HTMLBuilderNew
    {
        private const string scriptVersion = "0.5";
        private const int scriptVersionRev = 6;
        private readonly SettingsContainer _settings;

        private readonly ParsedLog _log;

        private readonly Statistics _statistics;

        public HTMLBuilderNew(ParsedLog log, SettingsContainer settings, Statistics statistics)
        {
            _log = log;

            _settings = settings;
            HTMLHelper.Settings = settings;
            GraphHelper.Settings = settings;

            _statistics = statistics;
        }

        private static String FilterStringChars(string str)
        {
            string filtered = "";
            string filter = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ";
            foreach (char c in str)
            {
                if (filter.Contains(c))
                {
                    filtered += c;
                }
            }
            return filtered;
        }

        //Generate HTML---------------------------------------------------------------------------------------------------------------------------------------------------------
        //Methods that make it easier to create Javascript graphs      
        /// <summary>
        /// Creates the dps graph
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        /// <param name="mode"></param>
        private List<PlayerChartDataDto> CreateDPSGraphData(int phaseIndex)
        {
            List<PlayerChartDataDto> list = new List<PlayerChartDataDto>();
            PhaseData phase = _statistics.Phases[phaseIndex];

            foreach (Player p in _log.PlayerList)
            {
                PlayerChartDataDto playerData = new PlayerChartDataDto();
                list.Add(playerData);
                playerData.boss = new List<int>();
                playerData.cleave = new List<int>();
                List<Point> bossPoints = GraphHelper.GetBossDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.S1);
                List<Point> cleavePoints = GraphHelper.GetCleaveDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.S1);
                foreach (Point point in bossPoints)
                {
                    playerData.boss.Add(point.Y);
                }
                foreach (Point point in cleavePoints)
                {
                    playerData.cleave.Add(point.Y);
                }
            }
                /*
                sw.Write("{");
                HTMLHelper.WriteDPSPlots(sw, totalDpsAllPlayers);
                sw.Write(" mode: 'lines'," +
                        "line: {shape: 'spline'}," +
                        "visible:'legendonly'," +
                        "name: 'All Player Dps'");
                sw.Write("},");
                HashSet<Mechanic> presMech = _log.MechanicData.GetPresentMechanics(phaseIndex);
                List<ushort> playersIds = _log.PlayerList.Select(x => x.InstID).ToList();
                foreach (Mechanic mech in presMech)
                {
                    List<MechanicLog> filterdList = _log.MechanicData[mech].Where(x => phase.InInterval(x.GetTime())).ToList();
                    sw.Write("{");
                    sw.Write("y: [");

                    int mechcount = 0;
                    foreach (MechanicLog ml in filterdList)
                    {                     
                        Point check;
                        if (playersIds.Contains(ml.GetPlayer().InstID))
                        {
                            double time = (ml.GetTime() - phase.Start) / 1000.0;
                            check = GraphHelper.GetBossDPSGraph(_log, ml.GetPlayer(), phaseIndex, phase, mode).LastOrDefault(x => x.X <= time);
                            if (check == Point.Empty)
                            {
                                check = new Point(0, GraphHelper.GetBossDPSGraph(_log, ml.GetPlayer(), phaseIndex, phase, mode).Last().Y);
                            } else
                            {
                                int time1 = check.X;
                                int y1 = check.Y;
                                check = GraphHelper.GetBossDPSGraph(_log, ml.GetPlayer(), phaseIndex, phase, mode).FirstOrDefault(x => x.X >= time);
                                if (check == Point.Empty)
                                {
                                    check.Y = y1;
                                } else
                                {
                                    int time2 = check.X;
                                    int y2 = check.Y;
                                    if (time2 - time1 > 0)
                                    {
                                        check.Y = (int)Math.Round((time - time1) * (y2 - y1) / (time2 - time1) + y1);
                                    }
                                }
                            }
                        }
                        else
                        {
                            check = _log.Boss.GetHealthOverTime().FirstOrDefault(x => x.X > ml.GetTime());
                            if (check == Point.Empty)
                            {
                                check = _log.Boss.GetHealthOverTime().Count == 0 ? new Point(0, 10000) : new Point(0, _log.Boss.GetHealthOverTime().Last().Y);
                            }
                            check.Y = (int)((check.Y / 10000f) * maxDPS);
                        }

                        if (mechcount == filterdList.Count - 1)
                        {
                            sw.Write("'" + check.Y + "'");
                        }
                        else
                        {
                            sw.Write("'" + check.Y + "',");

                        }

                        mechcount++;
                    }
                    sw.Write("],");
                    //add time axis
                    sw.Write("x: [");
                    mechcount = 0;
                    foreach (MechanicLog ml in filterdList)
                    {
                        if (mechcount == filterdList.Count - 1)
                        {
                            sw.Write("'" + Math.Round((ml.GetTime() - phase.Start) / 1000.0,4) + "'");
                        }
                        else
                        {
                            sw.Write("'" + Math.Round((ml.GetTime() - phase.Start) / 1000.0,4) + "',");
                        }

                        mechcount++;
                    }

                    sw.Write("],");
                    sw.Write(" mode: 'markers',");
                    if (!(mech.GetSkill() == -2 || mech.GetSkill() == -3))
                    {
                        sw.Write("visible:'legendonly',");
                    }
                    sw.Write("type:'scatter'," +
                            "marker:{" + "size: 15," + mech.GetPlotly() +  "}," +
                            "text:[");
                    foreach (MechanicLog ml in filterdList)
                    {
                        if (mechcount == filterdList.Count - 1)
                        {
                            sw.Write("'" + ml.GetPlayer().Character.Replace("'"," ") + "'");
                        }
                        else
                        {
                            sw.Write("'" + ml.GetPlayer().Character.Replace("'", " ") + "',");
                        }

                        mechcount++;
                    }

                    sw.Write("]," +
                            " name: '" + mech.GetPlotlyName().Replace("'", " ") + "'");
                    sw.Write("},");
                }
                if (maxDPS > 0)
                {
                    sw.Write("{");
                    HTMLHelper.WriteBossHealthGraph(sw, maxDPS, phase.Start, phase.End, _log.Boss);
                    sw.Write("}");
                }
                else
                {
                    sw.Write("{}");
                }
                if (_settings.LightTheme)
                {
                    sw.Write("];" +
                             "var layout = {" +
                             "yaxis:{title:'DPS'}," +
                             "xaxis:{title:'Time(sec)'}," +
                             //"legend: { traceorder: 'reversed' }," +
                             "hovermode: 'compare'," +
                             "legend: {orientation: 'h', font:{size: 15}}," +
                             // "yaxis: { title: 'DPS', domain: [0.51, 1] }," +
                             "font: { color: '#000000' }," +
                             "paper_bgcolor: 'rgba(255,255,255,0)'," +
                             "plot_bgcolor: 'rgba(255,255,255,0)'" +
                             "};");
                }
                else
                {
                    sw.Write("];" +
                             "var layout = {" +
                             "yaxis:{title:'DPS'}," +
                             "xaxis:{title:'Time(sec)'}," +
                             //"legend: { traceorder: 'reversed' }," +
                             "hovermode: 'compare'," +
                             "legend: {orientation: 'h', font:{size: 15}}," +
                             // "yaxis: { title: 'DPS', domain: [0.51, 1] }," +
                             "font: { color: '#ffffff' }," +
                             "paper_bgcolor: 'rgba(0,0,0,0)'," +
                             "plot_bgcolor: 'rgba(0,0,0,0)'" +
                             "};");
                }
                sw.Write(
                        "var lazyplot = document.querySelector(\"#" + plotID + "\");" +
                        "if ('IntersectionObserver' in window) {" +
                            "let lazyPlotObserver = new IntersectionObserver(function(entries, observer) {" +
                                "entries.forEach(function(entry) {" +
                                    "if (entry.isIntersecting)" +
                                    "{" +
                                        "Plotly.newPlot('" + plotID + "', data, layout);" +
                                        "lazyPlotObserver.unobserve(entry.target);" +
                                    "}" +
                                "});" +
                             "});" +
                            "lazyPlotObserver.observe(lazyplot);" +
                        "} else {"+
                            "Plotly.newPlot('" + plotID + "', data, layout);" +
                        "}");
            }
            sw.Write("});");
            sw.Write("</script> ");
            */
            return list;
        }

        private double[] CreateBossHealthData(int phaseIndex)
        {
            PhaseData phase = _statistics.Phases[phaseIndex];
            int duration = (int)phase.GetDuration("s");
            double[] chart = _statistics.BossHealth.Skip((int)phase.Start / 1000).Take(duration+1).ToArray();
            return chart;
        }

        private void GetRoles()
        {
            //tags: tank,healer,dps(power/condi)
            //Roles:greenteam,green split,cacnoneers,flakkiter,eater,KCpusher,agony,epi,handkiter,golemkiter,orbs
        }
        private void PrintWeapons(StreamWriter sw, Player p)
        {
            //print weapon sets
            string[] wep = p.GetWeaponsArray(_log);
            sw.Write("<div>");
            if (wep[0] != null)
            {
                sw.Write("<img src=\"" + HTMLHelper.GetLink(wep[0]) + "\" alt=\"" + wep[0] + "\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"" + wep[0] + "\">");
            }
            else if (wep[1] != null)
            {
                sw.Write("<img src=\"" + HTMLHelper.GetLink("Question") + "\" alt=\"Unknown\"  data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Unknown\">");
            }
            if (wep[1] != null)
            {
                if (wep[1] != "2Hand")
                {
                    sw.Write("<img src=\"" + HTMLHelper.GetLink(wep[1]) + "\" alt=\"" + wep[1] + "\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"" + wep[1] + "\">");
                }
            }
            else
            {
                sw.Write("<img src=\"" + HTMLHelper.GetLink("Question") + "\" alt=\"Unknown\"  data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Unknown\">");
            }
            if (wep[2] == null && wep[3] == null)
            {

            }
            else
            {
                sw.Write(" / ");
            }

            if (wep[2] != null)
            {
                sw.Write("<img src=\"" + HTMLHelper.GetLink(wep[2]) + "\" alt=\"" + wep[2] + "\"  data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"" + wep[2] + "\">");
            }
            else if (wep[3] != null)
            {
                sw.Write("<img src=\"" + HTMLHelper.GetLink("Question") + "\" alt=\"Unknown\" height=\"18\" width=\"18\" >");
            }
            if (wep[3] != null)
            {
                if (wep[3] != "2Hand")
                {
                    sw.Write("<img src=\"" + HTMLHelper.GetLink(wep[3]) + "\" alt=\"" + wep[3] + "\"  data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"" + wep[3] + "\">");
                }
            }
            else
            {
                //sw.Write("<img src=\"" + HTMLHelper.GetLink("Question") + "\" alt=\"Unknown\" height=\"18\" width=\"18\" >");
            }
            sw.Write("<br>");
            sw.Write("</div>");
        }

        /// <summary>
        /// Creates the composition table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateCompTable(StreamWriter sw)
        {
            int groupCount = 0;
            int firstGroup = 11;
            foreach (Player play in _log.PlayerList)
            {
                int playerGroup = play.Group;
                if (playerGroup > groupCount)
                {
                    groupCount = playerGroup;
                }
                if (playerGroup < firstGroup)
                {
                    firstGroup = playerGroup;
                }
            }
            //generate comp table
            sw.Write("<table class=\"table\"");
            {
                sw.Write("<tbody>");
                for (int n = firstGroup; n <= groupCount; n++)
                {
                    sw.Write("<tr>");
                    List<Player> sortedList = _log.PlayerList.Where(x => x.Group == n).ToList();
                    if (sortedList.Count > 0)
                    {
                        foreach (Player gPlay in sortedList)
                        {
                            string charName = gPlay.Character.Length > 10 ? gPlay.Character.Substring(0, 10) : gPlay.Character;
                            //Getting Build
                            string build = "";
                            if (gPlay.Condition > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/5/54/Condition_Damage.png\" alt=\"Condition Damage\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Condition Damage-" + gPlay.Condition + "\">";//"<span class=\"badge badge-warning\">Condi("+ gPlay.getCondition() + ")</span>";
                            }
                            if (gPlay.Concentration > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/4/44/Boon_Duration.png\" alt =\"Concentration\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Concentration-" + gPlay.Concentration + "\">";//"<span class=\"badge badge-warning\">Condi("+ gPlay.getCondition() + ")</span>";
                            }
                            if (gPlay.Healing > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/8/81/Healing_Power.png\" alt=\"Healing Power\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Healing Power-" + gPlay.Healing + "\">";//"<span class=\"badge badge-success\">Heal("+ gPlay.getHealing() + ")</span>";
                            }
                            if (gPlay.Toughness > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/1/12/Toughness.png\" alt=\"Toughness\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Toughness-" + gPlay.Toughness + "\">";//"<span class=\"badge badge-secondary\">Tough("+ gPlay.getToughness() + ")</span>";
                            }
                            sw.Write("<td class=\"composition\">");
                            {
                                sw.Write("<img src=\"" + HTMLHelper.GetLink(gPlay.Prof) + "\" alt=\"" + gPlay.Prof + "\" height=\"18\" width=\"18\" >");
                                sw.Write(build);
                                PrintWeapons(sw, gPlay);
                                sw.Write(charName);
                            }
                            sw.Write("</td>");
                        }
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</tbody>");
            }

            sw.Write("</table>");
        }
        /// <summary>
        /// Creates the dps table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private List<List<Object>> CreateDPSData(int phaseIndex)
        {
            List<List<Object>> list = new List<List<Object>>(_log.PlayerList.Count);
            PhaseData phase = _statistics.Phases[phaseIndex];
            List<string[]> footerList = new List<string[]>();

            foreach (Player player in _log.PlayerList)
            {
                Statistics.FinalDPS dps = _statistics.Dps[player][phaseIndex];
                Statistics.FinalStats stats = _statistics.Stats[player][phaseIndex];

                List<Object> playerData = new List<Object>();
                playerData.Add(dps.BossDamage);
                playerData.Add(dps.BossDps);
                playerData.Add(dps.BossPowerDamage);
                playerData.Add(dps.BossPowerDps);
                playerData.Add(dps.BossCondiDamage);
                playerData.Add(dps.BossCondiDps);

                playerData.Add(dps.AllDamage);
                playerData.Add(dps.AllDps);
                playerData.Add(dps.AllPowerDamage);
                playerData.Add(dps.AllPowerDps);
                playerData.Add(dps.AllCondiDamage);
                playerData.Add(dps.AllCondiDps);
                playerData.Add(stats.DownCount);

                TimeSpan timedead = TimeSpan.FromMilliseconds(stats.Died);
                long fightDuration = phase.GetDuration();
                if (timedead > TimeSpan.Zero)
                {
                    playerData.Add(timedead + " (" + Math.Round((timedead.TotalMilliseconds / fightDuration) * 100, 1) + "% Alive)");
                    playerData.Add(timedead.Minutes + " m " + timedead.Seconds + " s");
                }
                else
                {
                    playerData.Add("Never died");
                    playerData.Add("");
                }

                list.Add(playerData);
            }
            return list;
        }
        /// <summary>
        /// Creates the damage stats table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private List<List<Object>> CreateDMGStatsData(int phaseIndex)
        {
            List<List<Object>> list = new List<List<Object>>();
            PhaseData phase = _statistics.Phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                Statistics.FinalStats stats = _statistics.Stats[player][phaseIndex];
                Statistics.FinalDPS dps = _statistics.Dps[player][phaseIndex];

                List<Object> playerData = new List<Object>();
                playerData.Add(stats.PowerLoopCount); //0
                playerData.Add(stats.CritablePowerLoopCount); //1
                playerData.Add(Math.Round((Double)(stats.CriticalRate) / stats.CritablePowerLoopCount * 100, 1)); //2
                playerData.Add(stats.CriticalRate); //3
                playerData.Add(stats.CriticalDmg); //4

                playerData.Add(Math.Round((Double)(stats.ScholarRate) / stats.PowerLoopCount * 100, 1)); //5
                playerData.Add(stats.ScholarRate); //6
                playerData.Add(stats.ScholarDmg); //7
                playerData.Add(Math.Round(100.0 * (dps.PlayerPowerDamage / (Double)(dps.PlayerPowerDamage - stats.ScholarDmg) - 1.0), 3)); //8

                playerData.Add(Math.Round((Double)(stats.MovingRate) / stats.PowerLoopCount * 100, 1)); //9
                playerData.Add(stats.MovingRate); //10
                playerData.Add(stats.MovingDamage); //11
                playerData.Add(Math.Round(100.0 * (dps.PlayerPowerDamage / (Double)(dps.PlayerPowerDamage - stats.MovingDamage) - 1.0), 3)); //12

                playerData.Add(Math.Round(stats.FlankingRate / (Double)stats.PowerLoopCount * 100, 1)); //13
                playerData.Add(stats.FlankingRate); //14

                playerData.Add(Math.Round(stats.GlanceRate / (Double)stats.PowerLoopCount * 100, 1)); //15
                playerData.Add(stats.GlanceRate); //16

                playerData.Add(stats.Missed); //17
                playerData.Add(stats.Interrupts); //18
                playerData.Add(stats.Invulned); //19

                playerData.Add(stats.TimeWasted); //20
                playerData.Add(stats.Wasted); //21

                playerData.Add(stats.TimeSaved); //22
                playerData.Add(stats.Saved); //23

                playerData.Add(stats.SwapCount); //24
                playerData.Add(Math.Round(stats.StackDist, 2)); //25
                playerData.Add(stats.DownCount); //26

                TimeSpan timedead = TimeSpan.FromMilliseconds(stats.Died);//dead 
                if (timedead > TimeSpan.Zero)
                {
                    playerData.Add(timedead.Minutes + " m " + timedead.Seconds + " s"); //27
                    playerData.Add(timedead + "(" + Math.Round((timedead.TotalMilliseconds / phase.GetDuration()) * 100, 1) + "% Alive)"); //28
                }
                else
                {
                    playerData.Add(""); //27
                    playerData.Add("Never died"); //28
                }
                list.Add(playerData);
            }
            return list;
        }
        /// <summary>
        /// Creates the damage stats table for hits on just boss
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private List<List<Object>> CreateDMGStatsBossData(int phaseIndex)
        {
            List<List<Object>> list = new List<List<Object>>();

            PhaseData phase = _statistics.Phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                Statistics.FinalStats stats = _statistics.Stats[player][phaseIndex];
                Statistics.FinalDPS dps = _statistics.Dps[player][phaseIndex];

                List<Object> playerData = new List<object>();

                playerData.Add(stats.PowerLoopCountBoss); //0
                playerData.Add(stats.CritablePowerLoopCountBoss); //1
                playerData.Add(Math.Round((Double)(stats.CriticalRateBoss) / stats.CritablePowerLoopCountBoss * 100, 1)); //2
                playerData.Add(stats.CriticalRateBoss); //3
                playerData.Add(stats.CriticalDmgBoss); //4

                playerData.Add(Math.Round((Double)(stats.ScholarRateBoss) / stats.PowerLoopCountBoss * 100, 1)); //5
                playerData.Add(stats.ScholarRateBoss); //6
                playerData.Add(stats.ScholarDmgBoss); //7
                playerData.Add(Math.Round(100.0 * (dps.PlayerBossPowerDamage / (Double)(dps.PlayerBossPowerDamage - stats.ScholarDmgBoss) - 1.0), 3)); //8

                playerData.Add(Math.Round((Double)(stats.MovingRateBoss) / stats.PowerLoopCountBoss * 100, 1)); //9
                playerData.Add(stats.MovingRateBoss); //10
                playerData.Add(stats.MovingDamageBoss); //11
                playerData.Add(Math.Round(100.0 * (dps.PlayerBossPowerDamage / (Double)(dps.PlayerBossPowerDamage - stats.MovingDamageBoss) - 1.0), 3)); //12

                playerData.Add(Math.Round(stats.FlankingRateBoss / (Double)stats.PowerLoopCountBoss * 100, 1)); //13
                playerData.Add(stats.FlankingRateBoss); //14

                playerData.Add(Math.Round(stats.GlanceRateBoss / (Double)stats.PowerLoopCountBoss * 100, 1)); //15
                playerData.Add(stats.GlanceRateBoss); //16

                playerData.Add(stats.MissedBoss); //17
                playerData.Add(stats.InterruptsBoss); //18
                playerData.Add(stats.InvulnedBoss); //19

                playerData.Add(stats.TimeWasted); //20
                playerData.Add(stats.Wasted); //21

                playerData.Add(stats.TimeSaved); //22
                playerData.Add(stats.Saved); //23

                playerData.Add(stats.SwapCount); //24
                playerData.Add(Math.Round(stats.StackDist, 2)); //25
                playerData.Add(stats.DownCount); //26

                long fightDuration = phase.GetDuration();
                TimeSpan timedead = TimeSpan.FromMilliseconds(stats.Died);//dead 
                if (timedead > TimeSpan.Zero)
                {
                    playerData.Add(timedead.Minutes + " m " + timedead.Seconds + " s"); //27
                    playerData.Add(timedead + "(" + Math.Round((timedead.TotalMilliseconds / fightDuration) * 100, 1) + "% Alive)"); //28
                }
                else
                {
                    playerData.Add(""); //27
                    playerData.Add("Never died"); //28
                }
                list.Add(playerData);
            }
            return list;
        }
        /// <summary>
        /// Creates the defense table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private List<List<Object>> CreateDefData(int phaseIndex)
        {
            List<List<Object>> list = new List<List<Object>>();

            PhaseData phase = _statistics.Phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                Statistics.FinalDefenses defenses = _statistics.Defenses[player][phaseIndex];
                Statistics.FinalStats stats = _statistics.Stats[player][phaseIndex];

                List<Object> playerData = new List<object>();
                playerData.Add(defenses.DamageTaken);
                playerData.Add(defenses.DamageBarrier);
                playerData.Add(defenses.BlockedCount);
                playerData.Add(0);
                playerData.Add(defenses.InvulnedCount);
                playerData.Add(defenses.EvadedCount);
                playerData.Add(stats.DodgeCount);
                playerData.Add(stats.DownCount);

                TimeSpan timedead = TimeSpan.FromMilliseconds(stats.Died);//dead
                long fightDuration = phase.GetDuration("s");
                if (timedead > TimeSpan.Zero)
                {
                    playerData.Add(timedead.Minutes + " m " + timedead.Seconds + " s");
                    playerData.Add(timedead + "(" + Math.Round((timedead.TotalMilliseconds / fightDuration) * 100, 1) + "% Alive)");
                }
                else
                {
                    playerData.Add("");
                    playerData.Add("Never died");
                }

                list.Add(playerData);
            }

            return list;
        }
        /// <summary>
        /// Creates the support table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private List<List<Object>> CreateSupData(int phaseIndex)
        {
            List<List<Object>> list = new List<List<Object>>();

            foreach (Player player in _log.PlayerList)
            {
                Statistics.FinalSupport support = _statistics.Support[player][phaseIndex];
                List<Object> playerData = new List<Object>(4);
                playerData.Add(support.CondiCleanse);
                playerData.Add(support.CondiCleanseTime);
                playerData.Add(support.Resurrects);
                playerData.Add(support.ResurrectTime);
                list.Add(playerData);
            }
            return list;
        }
        /// <summary>
        /// Create the buff uptime table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="listToUse"></param>
        /// <param name="tableId"></param>
        /// <param name="phaseIndex"></param>
        private List<BoonData> CreateUptimeData(List<Boon> listToUse, int phaseIndex)
        {
            List<PhaseData> phases = _statistics.Phases;
            List<BoonData> list = new List<BoonData>();

            List<List<string>> footList = new List<List<string>>();
            
            HashSet<int> intensityBoon = new HashSet<int>();
            bool boonTable = listToUse.Select(x => x.ID).Contains(740);
                
            foreach (Player player in _log.PlayerList)
            {
                BoonData boonData = new BoonData();

                Dictionary<long, Statistics.FinalBoonUptime> boons = _statistics.SelfBoons[player][phaseIndex];
                Dictionary<long, Dictionary<int, string[]>> extraBoonData = player.GetExtraBoonData(_log);
                List<string> boonArrayToList = new List<string>
                {
                    player.Group.ToString()
                };
                long fightDuration = phases[phaseIndex].GetDuration();
                Dictionary<long, long> boonPresence = player.GetBoonPresence(_log, phaseIndex);
                int count = 0;

                        
                if (boonTable)
                {
                    double avgBoons = 0.0;
                    foreach (long duration in boonPresence.Values)
                    {
                        avgBoons += duration;
                    }
                    boonData.avg = Math.Round(avgBoons/fightDuration, 1);
                }
                foreach (Boon boon in listToUse)
                {
                    List<Object> boonVals = new List<Object>();
                    boonData.val.Add(boonVals);

                    boonVals.Add(boons[boon.ID].Uptime);

                    if (boon.Type == Boon.BoonType.Intensity)
                    {
                        intensityBoon.Add(count);
                    }
                    string tooltip = "";
                    if (extraBoonData.TryGetValue(boon.ID, out var myDict))
                    {
                        string[] tooltips = myDict[phaseIndex];
                        tooltip = "<big><b>Boss</b></big><br>" + tooltips[1] + "<br><big><b>All</b></big><br>" + tooltips[0];
                        boonVals.Add(0);
                        boonVals.Add(tooltip);
                    }
                    else
                    {
                        if (boonTable && boon.Type == Boon.BoonType.Intensity && boonPresence.TryGetValue(boon.ID, out long presenceValue))
                        {
                            boonVals.Add(Math.Round(100.0 * presenceValue / fightDuration, 1));
                        }
                    }                                
                    boonArrayToList.Add(boons[boon.ID].Uptime.ToString());                        
                    count++;
                }
                        
                //gather data for footer
                footList.Add(boonArrayToList);
                list.Add(boonData);
            }
            return list;
        }
        /// <summary>
        /// Create the self buff generation table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="listToUse"></param>
        /// <param name="tableId"></param>
        /// <param name="phaseIndex"></param>
        private List<BoonData> CreateGenData(List<Boon> listToUse, int phaseIndex, string target)
        {
            List<BoonData> list = new List<BoonData>();

            foreach (Player player in _log.PlayerList)
            {
                BoonData boonData = new BoonData();

                Dictionary<long, Statistics.FinalBoonUptime> uptimes;
                if (target == "self") uptimes = _statistics.SelfBoons[player][phaseIndex];
                else if (target == "group") uptimes = _statistics.GroupBoons[player][phaseIndex];
                else if (target == "off") uptimes = _statistics.OffGroupBoons[player][phaseIndex];
                else if (target == "squad") uptimes = _statistics.SquadBoons[player][phaseIndex];
                else throw new InvalidOperationException("unknown target type");

                Dictionary<long, string> rates = new Dictionary<long, string>();
                foreach (Boon boon in listToUse)
                {
                    Statistics.FinalBoonUptime uptime = uptimes[boon.ID];
                    List<Object> val = new List<Object>(2);
                    val.Add(uptime.Generation);
                    val.Add(uptime.Overstack);
                    boonData.val.Add(val);
                }
                list.Add(boonData);
            }
            return list;
        }

        /// <summary>
        /// Creates the rotation tab for a given player
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="p"></param>
        /// <param name="simpleRotSize"></param>
        /// <param name="phaseIndex"></param>
        private List<double[]> CreateSimpleRotationTabData(Player p, int phaseIndex, Dictionary<long, SkillItem> usedSkills)
        {
            List<double[]> list = new List<double[]>();

            PhaseData phase = _statistics.Phases[phaseIndex];
            List<CastLog> casting = p.GetCastLogs(_log, phase.Start, phase.End);
            SkillData skillList = _log.SkillData;
            foreach (CastLog cl in casting)
            {
                if (!usedSkills.ContainsKey(cl.SkillId)) usedSkills.Add(cl.SkillId, skillList.GetOrDummy(cl.SkillId));
                double[] rotEntry = new double[5];
                list.Add(rotEntry);
                rotEntry[0] = (cl.Time - phase.Start) / 1000.0;
                rotEntry[1] = cl.SkillId;
                rotEntry[2] = cl.ActualDuration;
                rotEntry[3] = EncodeEndActivation(cl.EndActivation);
                rotEntry[4] = cl.StartActivation == ParseEnum.Activation.Quickness ? 1 : 0;
            }
            return list;
        }

        private int EncodeEndActivation(ParseEnum.Activation endActivation)
        {
            switch (endActivation)
            {
                case ParseEnum.Activation.CancelFire: return 1;
                case ParseEnum.Activation.CancelCancel: return 2;
                case ParseEnum.Activation.Reset: return 3;
                default: return 0;
            }
        }

        /// <summary>
        /// Creates the death recap tab for a given player
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="p">The player</param>
        private void CreateDeathRecap(StreamWriter sw, Player p)
        {
            List<DamageLog> damageLogs = p.GetDamageTakenLogs(_log, 0, _log.FightData.FightDuration);
            long start = _log.FightData.FightStart;
            long end = _log.FightData.FightEnd;
            List<CombatItem> down = _log.CombatData.GetStates(p.InstID, ParseEnum.StateChange.ChangeDown, start, end);
            if (down.Count > 0)
            {
                List<CombatItem> ups = _log.CombatData.GetStates(p.InstID, ParseEnum.StateChange.ChangeUp, start, end);
                // Surely a consumable in fractals
                down = ups.Count > down.Count ? new List<CombatItem>() : down.GetRange(ups.Count, down.Count - ups.Count);
            }
            List<CombatItem> dead = _log.CombatData.GetStates(p.InstID, ParseEnum.StateChange.ChangeDead, start, end);
            List<DamageLog> damageToDown = new List<DamageLog>();
            List<DamageLog> damageToKill = new List<DamageLog>();
            if (down.Count > 0)
            {//went to down state before death
                damageToDown = damageLogs.Where(x => x.Time < down.Last().Time - start && x.Damage > 0).ToList();
                damageToKill = damageLogs.Where(x => x.Time > down.Last().Time - start && x.Time < dead.Last().Time - start && x.Damage > 0).ToList();
                //Filter last 30k dmg taken
                int totaldmg = 0;
                for (int i = damageToDown.Count - 1; i > 0; i--)
                {
                    totaldmg += damageToDown[i].Damage;
                    if (totaldmg > 30000)
                    {
                        damageToDown = damageToDown.GetRange(i, damageToDown.Count - i);
                        break;
                    }
                }
                if (totaldmg == 0)
                {
                    sw.Write("<center>");
                    sw.Write("<p>Something strange happened</p>");
                    sw.Write("</center>");
                    return;
                }
                sw.Write("<center>");
                sw.Write("<p>Took " + damageToDown.Sum(x => x.Damage) + " damage in " +
                ((damageToDown.Last().Time - damageToDown.First().Time) / 1000f).ToString() + " seconds to enter downstate");
                if (damageToKill.Count > 0)
                {
                    sw.Write("<p>Took " + damageToKill.Sum(x => x.Damage) + " damage in " +
                       ((damageToKill.Last().Time - damageToKill.First().Time) / 1000f).ToString() + " seconds to die</p>");
                }
                else
                {
                    sw.Write("<p>Instant death after a down</p>");
                }
                sw.Write("</center>");
            }
            else
            {
                damageToKill = damageLogs.Where(x => x.Time < dead.Last().Time && x.Damage > 0).ToList();
                //Filter last 30k dmg taken
                int totaldmg = 0;
                for (int i = damageToKill.Count - 1; i > 0; i--)
                {
                    totaldmg += damageToKill[i].Damage;
                    if (totaldmg > 30000)
                    {
                        damageToKill = damageToKill.GetRange(i, damageToKill.Count - 1 - i);
                        break;
                    }
                }
                sw.Write("<center><h3>Player was insta killed by a mechanic, fall damage or by /gg</h3></center>");
            }
            string pid = p.InstID.ToString();
            sw.Write("<center><div id=\"BarDeathRecap" + pid + "\"></div> </center>");
            sw.Write("<script>");
            {
                sw.Write("var data = [{");
                //Time on X
                sw.Write("x : [");
                if (damageToDown.Count != 0)
                {
                    foreach (DamageLog dl in damageToDown)
                    {
                        sw.Write("'" + dl.Time / 1000f + "s',");
                    }
                }
                for (int d = 0; d < damageToKill.Count; d++)
                {
                    sw.Write("'" + damageToKill[d].Time / 1000f + "s'");

                    if (d != damageToKill.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write("],");
                //damage on Y
                sw.Write("y : [");
                if (damageToDown.Count != 0)
                {
                    foreach (DamageLog dl in damageToDown)
                    {
                        sw.Write("'" + dl.Damage + "',");
                    }
                }
                for (int d = 0; d < damageToKill.Count; d++)
                {
                    sw.Write("'" + damageToKill[d].Damage + "'");

                    if (d != damageToKill.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write("],");
                //Color 
                sw.Write("marker : {color:[");
                if (damageToDown.Count != 0)
                {
                    for (int d = 0; d < damageToDown.Count; d++)
                    {
                        sw.Write("'rgb(0,255,0,1)',");
                    }
                }
                for (int d = 0; d < damageToKill.Count; d++)
                {
                    sw.Write(down.Count == 0 ? "'rgb(0,255,0,1)'" : "'rgb(255,0,0,1)'");

                    if (d != damageToKill.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write("]},");
                //text
                sw.Write("text : [");
                if (damageToDown.Count != 0)
                {
                    foreach (DamageLog dl in damageToDown)
                    {
                        AgentItem ag = _log.AgentData.GetAgentWInst(dl.SrcInstId);
                        string name = "UNKNOWN";
                        if (ag != null)
                        {
                            name = ag.Name.Replace("\0", "").Replace("\'", "\\'");
                        }
                        string skillname = _log.SkillData.GetName(dl.SkillId).Replace("\'", "\\'");
                        sw.Write("'" + name + "<br>" + skillname + " hit you for " + dl.Damage + "',");
                    }
                }
                for (int d = 0; d < damageToKill.Count; d++)
                {
                    AgentItem ag = _log.AgentData.GetAgentWInst(damageToKill[d].SrcInstId);
                    string name = "UNKNOWN";
                    if (ag != null )
                    {
                        name = ag.Name.Replace("\0", "").Replace("\'", "\\'");
                    }
                    string skillname = _log.SkillData.GetName(damageToKill[d].SkillId).Replace("\'", "\\'");
                    sw.Write("'" + name + "<br>" +
                           "hit you with <b>" + skillname + "</b> for " + damageToKill[d].Damage + "'");

                    if (d != damageToKill.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write("],");
                sw.Write("type:'bar',");

                sw.Write("}];");

                if (!_settings.LightTheme)
                {
                    sw.Write(
                        "var layout = { title: 'Last 30k Damage Taken before death', font: { color: '#ffffff' },width: 1100," +
                        "paper_bgcolor: 'rgba(0,0,0,0)', plot_bgcolor: 'rgba(0,0,0,0)',showlegend: false,bargap :0.05,yaxis:{title:'Damage'},xaxis:{title:'Time(seconds)',type:'catagory'}};");
                }
                else
                {
                    sw.Write(
                        "var layout = { title: 'Last 30k Damage Taken before death', font: { color: '#000000' },width: 1100," +
                        "paper_bgcolor: 'rgba(255,255,255,0)', plot_bgcolor: 'rgba(255,255,255,0)',showlegend: false,bargap :0.05,yaxis:{title:'Damage'},xaxis:{title:'Time(seconds)',type:'catagory'}};");

                }

                sw.Write("Plotly.newPlot('BarDeathRecap" + pid + "', data, layout);");

            }
            sw.Write("</script>");
        }

        private List<double[]> CreateDMGDistTableBody(List<CastLog> casting, List<DamageLog> damageLogs, long finalTotalDamage,
            Dictionary<long, SkillItem> usedSkills, Dictionary<long, Boon> usedBoons)
        {
            List<double[]> list = new List<double[]>();
            Dictionary<long, List<CastLog>> castLogsBySkill = casting.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            Dictionary<long, List<DamageLog>> damageLogsBySkill = damageLogs.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            Dictionary<long, Boon> conditionsById = _statistics.PresentConditions.ToDictionary(x => x.ID);
            SkillData skillList = _log.SkillData;
            foreach (KeyValuePair<long,List<DamageLog>> entry in damageLogsBySkill)
            {
                int totaldamage = 0,mindamage = 0,maxdamage = 0,casts = 0,hits = 0,crit = 0,flank = 0,glance = 0,timeswasted = 0,timessaved = 0;
                foreach (DamageLog dl in entry.Value)
                {
                    int curdmg = dl.Damage;
                    totaldamage += curdmg;
                    if (0 == mindamage || curdmg < mindamage) { mindamage = curdmg; }
                    if (0 == maxdamage || curdmg > maxdamage) { maxdamage = curdmg; }
                    hits++;
                    if (dl.Result == ParseEnum.Result.Crit) crit++;
                    if (dl.Result == ParseEnum.Result.Glance) glance++;
                    if (dl.IsFlanking == 1) flank++;
                }

                bool isCondi = conditionsById.ContainsKey(entry.Key);
                if (isCondi)
                {
                    Boon condi = conditionsById[entry.Key];
                    if (!usedBoons.ContainsKey(condi.ID)) usedBoons.Add(condi.ID, condi);
                } else
                {
                    if (!usedSkills.ContainsKey(entry.Key)) usedSkills.Add(entry.Key, skillList.GetOrDummy(entry.Key));
                }

                if (!isCondi && castLogsBySkill.TryGetValue(entry.Key, out List<CastLog> clList))
                {

                    casts = clList.Count;
                    foreach (CastLog cl in clList)
                    {
                        if (cl.EndActivation == ParseEnum.Activation.CancelCancel) timeswasted += cl.ActualDuration;
                        if (cl.EndActivation == ParseEnum.Activation.CancelFire && cl.ActualDuration < cl.ExpectedDuration)
                        {
                            timessaved += cl.ExpectedDuration - cl.ActualDuration;
                        }
                    }
                }

                double[] skillData = {
                    isCondi?1:0,
                    entry.Key,
                    finalTotalDamage > 0 ? Math.Round(100.0 * totaldamage / finalTotalDamage,3) : 0,
                    totaldamage, mindamage, maxdamage,
                    casts, hits, crit, flank, glance,
                    timeswasted / 1000.0,
                    -timessaved / 1000.0};
                list.Add(skillData);
            }
            return list;
        }

        private DmgDistributionDto _CreateDMGDistTable(Statistics.FinalDPS dps, AbstractMasterPlayer p, bool toBoss, int phaseIndex,
            Dictionary<long, SkillItem> usedSkills, Dictionary<long, Boon> usedBoons)
        {
            DmgDistributionDto dto = new DmgDistributionDto();
            PhaseData phase = _statistics.Phases[phaseIndex];
            List<CastLog> casting = p.GetCastLogs(_log, phase.Start, phase.End);
            List<DamageLog> damageLogs;
            if (toBoss && phase.Redirection.Count > 0)
            {
                damageLogs = p.GetJustPlayerDamageLogs(phase.Redirection, _log, phase.Start, phase.End);
            }
            else
            {
                damageLogs = p.GetJustPlayerDamageLogs(toBoss ? _log.Boss.InstID : 0, _log, phase.Start, phase.End);
            }
            int totalDamage = toBoss ? dps.BossDamage : dps.AllDamage;
            dto.totalDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.Damage) : 0;
            if (totalDamage > 0){
                dto.contribution = Math.Round(100.0 * dto.totalDamage / totalDamage,3);
            }

            dto.data = CreateDMGDistTableBody(casting, damageLogs, dto.totalDamage, usedSkills, usedBoons);

            return dto;
        }

        /// <summary>
        /// Creates the damage distribution table for a given player
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="p"></param>
        /// <param name="toBoss"></param>
        /// <param name="phaseIndex"></param>
        private DmgDistributionDto CreatePlayerDMGDistTable(Player p, bool toBoss, int phaseIndex,
            Dictionary<long, SkillItem> usedSkills, Dictionary<long, Boon> usedBoons)
        {
            Statistics.FinalDPS dps = _statistics.Dps[p][phaseIndex];
            return _CreateDMGDistTable(dps, p, toBoss, phaseIndex, usedSkills, usedBoons);
        }

        /// <summary>
        /// Creates the damage distribution table for a the boss
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="p"></param>
        /// <param name="phaseIndex"></param>
        private DmgDistributionDto CreateBossDMGDistTable(Boss p, int phaseIndex, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Boon> usedBoons)
        {
            Statistics.FinalDPS dps = _statistics.BossDps[phaseIndex];
            return _CreateDMGDistTable(dps, p, false, phaseIndex, usedSkills, usedBoons);
        }

        private List<Object> _CreateDMGDistTable(Statistics.FinalDPS dps, StreamWriter sw, AbstractMasterPlayer p, Minions minions, bool toBoss, int phaseIndex)
        {
            List<Object> list = new List<object>();
            int totalDamage = toBoss ? dps.BossDamage : dps.AllDamage;
            string tabid = p.InstID + "_" + phaseIndex + "_" + minions.InstID + (toBoss ? "_boss" : "");
            PhaseData phase = _statistics.Phases[phaseIndex];
            List<CastLog> casting = minions.GetCastLogs(_log, phase.Start, phase.End);
            List<DamageLog> damageLogs;
            if (toBoss && phase.Redirection.Count > 0)
            {
                damageLogs = minions.GetDamageLogs(phase.Redirection, _log, phase.Start, phase.End);
            }
            else
            {
                damageLogs = minions.GetDamageLogs(toBoss ? _log.Boss.InstID : 0, _log, phase.Start, phase.End);
            }
            int finalTotalDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.Damage) : 0;
            if (totalDamage > 0)
            {
                string contribution = Math.Round(100.0 * finalTotalDamage / totalDamage,2).ToString();
                sw.Write("<div>" + minions.Character + " did " + contribution + "% of " + p.Character + "'s total " + (toBoss ? (phase.Redirection.Count > 0 ? "adds " : "boss " ) : "") + "dps</div>");
            }
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#dist_table_" + tabid + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#dist_table_" + tabid + "').DataTable({\"columnDefs\": [ { \"title\": \"Skill\", className: \"dt-left\", \"targets\": [ 0 ]}], \"order\": [[2, \"desc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#dist_table_" + tabid + "').DataTable({\"columnDefs\": [ { \"title\": \"Skill\", className: \"dt-left\", \"targets\": [ 0 ]}], \"order\": [[2, \"desc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dist_table_" + tabid + "\">");
            {
                HTMLHelper.WriteDamageDistTableHeader(sw);
                sw.Write("<tbody>");
                {
                    //CreateDMGDistTableBody(casting, damageLogs, finalTotalDamage);
                }
                sw.Write("</tbody>");
                HTMLHelper.WriteDamageDistTableFoot(sw, finalTotalDamage);
            }
            sw.Write("</table>");
            return list;
        }

        /// <summary>
        /// Creates the damage distribution table for a given minion
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="p"></param>
        /// <param name="minions"></param>
        /// <param name="toBoss"></param>
        /// <param name="phaseIndex"></param>
        private void CreatePlayerMinionDMGDistTable(StreamWriter sw, Player p, Minions minions, bool toBoss, int phaseIndex)
        {
            Statistics.FinalDPS dps = _statistics.Dps[p][phaseIndex];

            _CreateDMGDistTable(dps, sw, p, minions, toBoss, phaseIndex);
        }

        /// <summary>
        /// Creates the damage distribution table for a given boss minion
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="p"></param>
        /// <param name="minions"></param>
        /// <param name="phaseIndex"></param>
        private void CreateBossMinionDMGDistTable(StreamWriter sw, Boss p, Minions minions, int phaseIndex)
        {
            Statistics.FinalDPS dps = _statistics.BossDps[phaseIndex];
            _CreateDMGDistTable(dps, sw, p, minions, false, phaseIndex);
        }

        /// <summary>
        /// Create the damage taken distribution table for a given player
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="p"></param>
        /// <param name="phaseIndex"></param>
        private void CreatePlayerDMGTakenDistTable(StreamWriter sw, Player p, int phaseIndex)
        {
            PhaseData phase = _statistics.Phases[phaseIndex];
            List<DamageLog> damageLogs = p.GetDamageTakenLogs(_log, phase.Start, phase.End);
            SkillData skillList = _log.SkillData;
            long finalTotalDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => (long)x.Damage) : 0;
            string pid = p.InstID + "_" + phaseIndex;
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#distTaken_table_" + pid + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#distTaken_table_" + pid + "').DataTable({\"columnDefs\": [ { \"title\": \"Skill\", className: \"dt-left\", \"targets\": [ 0 ]}], \"order\": [[2, \"desc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#distTaken_table_" + pid + "').DataTable({\"columnDefs\": [ { \"title\": \"Skill\", className: \"dt-left\", \"targets\": [ 0 ]}], \"order\": [[2, \"desc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"distTaken_table_" + pid + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Skill</th>");
                        sw.Write("<th>Damage</th>");
                        sw.Write("<th>Percent</th>");
                        sw.Write("<th>Hits</th>");
                        sw.Write("<th>Min</th>");
                        sw.Write("<th>Avg</th>");
                        sw.Write("<th>Max</th>");
                        sw.Write("<th>Crit</th>");
                        sw.Write("<th>Flank</th>");
                        sw.Write("<th>Glance</th>");
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</thead>");
                sw.Write("<tbody>");
                {
                    HashSet<long> usedIDs = new HashSet<long>();
                    List<Boon> condiList = _statistics.PresentConditions;
                    foreach (Boon condi in condiList)
                    {
                        long condiID = condi.ID;
                        int totaldamage = 0;
                        int mindamage = 0;
                        int hits = 0;
                        int maxdamage = 0;
                        usedIDs.Add(condiID);
                        foreach (DamageLog dl in damageLogs.Where(x => x.SkillId == condiID))
                        {
                            int curdmg = dl.Damage;
                            totaldamage += curdmg;
                            if (0 == mindamage || curdmg < mindamage) { mindamage = curdmg; }
                            if (0 == maxdamage || curdmg > maxdamage) { maxdamage = curdmg; }
                            hits++;

                        }
                        int avgdamage = (int)(totaldamage / (double)hits);
                        if (totaldamage > 0)
                        {
                            string condiName = condi.Name;// Boon.getCondiName(condiID);
                            sw.Write("<tr>");
                            {
                                sw.Write("<td align=\"left\"><img src=\"" + condi.Link + "\" alt=\"" + condiName + "\" title=\"" + condiID + "\" height=\"18\" width=\"18\">" + condiName + "</td>");
                                sw.Write("<td>" + totaldamage + "</td>");
                                sw.Write("<td>" + Math.Round(100 * (double)totaldamage / finalTotalDamage,2) + "%</td>");
                                sw.Write("<td>" + hits + "</td>");
                                sw.Write("<td>" + mindamage + "</td>");
                                sw.Write("<td>" + avgdamage + "</td>");
                                sw.Write("<td>" + maxdamage + "</td>");
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                            }
                            sw.Write("</tr>");
                        }
                    }
                    foreach (int id in damageLogs.Where(x => !usedIDs.Contains(x.SkillId)).Select(x => (int)x.SkillId).Distinct())
                    {//foreach casted skill
                        SkillItem skill = skillList.Get(id);

                        if (skill != null)
                        {
                            int totaldamage = 0;
                            int mindamage = 0;
                            int hits = 0;
                            int maxdamage = 0;
                            int crit = 0;
                            int flank = 0;
                            int glance = 0;
                            foreach (DamageLog dl in damageLogs.Where(x => x.SkillId == id))
                            {
                                int curdmg = dl.Damage;
                                totaldamage += curdmg;
                                if (0 == mindamage || curdmg < mindamage) { mindamage = curdmg; }
                                if (0 == maxdamage || curdmg > maxdamage) { maxdamage = curdmg; }
                                if (curdmg >= 0) { hits++; };
                                ParseEnum.Result result = dl.Result;
                                if (result == ParseEnum.Result.Crit) { crit++; } else if (result == ParseEnum.Result.Glance) { glance++; }
                                if (dl.IsFlanking == 1) { flank++; }
                            }
                            int avgdamage = (int)(totaldamage / (double)hits);

                            if (skill.ApiSkill != null)
                            {
                                sw.Write("<tr>");
                                {
                                    sw.Write("<td align=\"left\"><img src=\"" + skill.ApiSkill.icon + "\" alt=\"" + skill.Name + "\" title=\"" + skill.ID + "\" height=\"18\" width=\"18\">" + skill.Name + "</td>");
                                    sw.Write("<td>" + totaldamage + "</td>");
                                    sw.Write("<td>" + Math.Round(100 * (double)totaldamage / finalTotalDamage,2) + "%</td>");
                                    sw.Write("<td>" + hits + "</td>");
                                    sw.Write("<td>" + mindamage + "</td>");
                                    sw.Write("<td>" + avgdamage + "</td>");
                                    sw.Write("<td>" + maxdamage + "</td>");
                                    sw.Write("<td>" + Math.Round(100 * (double)crit / hits,2) + "%</td>");
                                    sw.Write("<td>" + Math.Round(100 * (double)flank / hits,2) + "%</td>");
                                    sw.Write("<td>" + Math.Round(100 * (double)glance / hits,2) + "%</td>");
                                }
                                sw.Write("</tr>");
                            }
                            else
                            {
                                sw.Write("<tr>");
                                {
                                    sw.Write("<td align=\"left\">" + skill.Name + "</td>");
                                    sw.Write("<td>" + totaldamage + "</td>");
                                    sw.Write("<td>" + Math.Round(100 * (double)totaldamage / finalTotalDamage,2) + "%</td>");
                                    sw.Write("<td>" + hits + "</td>");
                                    sw.Write("<td>" + mindamage + "</td>");
                                    sw.Write("<td>" + avgdamage + "</td>");
                                    sw.Write("<td>" + maxdamage + "</td>");
                                    sw.Write("<td>" + Math.Round(100 * (double)crit / hits,2) + "%</td>");
                                    sw.Write("<td>" + Math.Round(100 * (double)flank / hits,2) + "%</td>");
                                    sw.Write("<td>" + Math.Round(100 * (double)glance / hits,2) + "%</td>");
                                }
                                sw.Write("</tr>");
                            }
                        }
                    }
                }
                sw.Write("</tbody>");
            }
            sw.Write("</table>");
        }

        private List<BoonChartDataDto> CreatePlayerBoonGraphData(Player p, int phaseIndex)
        {
            List<BoonChartDataDto> list = new List<BoonChartDataDto>();
            PhaseData phase = _statistics.Phases[phaseIndex];
            if (_statistics.PresentBoons.Count > 0)
            {
                Dictionary<long, BoonsGraphModel> boonGraphData = p.GetBoonGraphs(_log);
                foreach (BoonsGraphModel bgm in boonGraphData.Values.Reverse().Where(x => x.BoonName != "Number of Conditions"))
                {
                    BoonChartDataDto graph = CreatePlayerTabBoonGraph(bgm, phase.Start, phase.End);
                    if (graph != null) list.Add(graph);
                }
                boonGraphData = _log.Boss.GetBoonGraphs(_log);
                //TODO add to used boon list?
                foreach (BoonsGraphModel bgm in boonGraphData.Values.Reverse().Where(x => x.BoonName == "Compromised" || x.BoonName == "Unnatural Signet"))
                {
                    BoonChartDataDto graph = CreatePlayerTabBoonGraph(bgm, phase.Start, phase.End);
                    if (graph != null) list.Add(graph);
                }
            }
            return list;
        }

        private BoonChartDataDto CreatePlayerTabBoonGraph(BoonsGraphModel bgm, long start, long end)
        {
            //TODO line: {shape: 'hv'}
            long roundedStart = 1000 * (start / 1000);
            long roundedEnd = 1000 * (end / 1000);
            List<BoonsGraphModel.Segment> bChart = bgm.BoonChart.Where(x => x.End >= roundedStart && x.Start <= roundedEnd).ToList();
            if (bChart.Count == 0 || (bChart.Count == 1 && bChart.First().Value == 0))
            {
                return null;
            }
            BoonChartDataDto dto = new BoonChartDataDto();
            dto.name = bgm.BoonName;
            dto.visible = bgm.BoonName == "Might" || bgm.BoonName == "Quickness";
            dto.color = HTMLHelper.GetLink("Color-" + bgm.BoonName);
            dto.data = new List<double[]>(bChart.Count + 1);

            foreach (BoonsGraphModel.Segment seg in bChart)
            {
                double segStart = Math.Round(Math.Max(seg.Start - roundedStart, 0) / 1000.0, 3);
                dto.data.Add(new double[] { segStart, seg.Value });
            }
            BoonsGraphModel.Segment lastSeg = bChart.Last();
            double segEnd = Math.Round(Math.Min(lastSeg.End - roundedStart, roundedEnd - roundedStart) / 1000.0, 3);
            dto.data.Add(new double[] { segEnd, lastSeg.Value });

            return dto;
        }

        private List<FoodDto> CreatePlayerFoodData(Player p, int phaseIndex)
        {
            PhaseData phase = _statistics.Phases[phaseIndex];
            List<FoodDto> list = new List<FoodDto>();
            List<Tuple<Boon, long, int>> consume = p.GetConsumablesList(_log, phase.Start, phase.End);

            foreach(Tuple<Boon, long, int> entry in consume)
            {
                FoodDto dto = new FoodDto();
                dto.time = entry.Item2 / 1000.0;
                dto.duration = entry.Item3 / 1000.0;
                dto.name = entry.Item1.Name;
                dto.icon = entry.Item1.Link;
                dto.dimished = entry.Item1.ID == 46587 || entry.Item1.ID == 46668;
                list.Add(dto);
            }

            return list;
        }

        /// <summary>
        /// Creates the mechanics table of the fight
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private List<List<int[]>> CreateMechanicData(int phaseIndex)
        {
            List<List<int[]>> list = new List<List<int[]>>();
            HashSet<Mechanic> presMech = _log.MechanicData.GetPresentMechanics(0);
            PhaseData phase = _statistics.Phases[phaseIndex];

            foreach (Player p in _log.PlayerList)
            {
                List<int[]> playerData = new List<int[]>(presMech.Count);
                foreach (Mechanic mech in presMech)
                {
                    long timeFilter = 0;
                    int filterCount = 0;
                    List<MechanicLog> mls = _log.MechanicData[mech].Where(x => x.Player.InstID == p.InstID && phase.InInterval(x.Time)).ToList();
                    int count = mls.Count;
                    foreach (MechanicLog ml in mls)
                    {
                        if (mech.InternalCooldown != 0 && ml.Time - timeFilter < mech.SkillId)//ICD check
                        {
                            filterCount++;
                        }
                        timeFilter = ml.Time;

                    }
                    int[] mechEntry = {count - filterCount,count};
                    playerData.Add(mechEntry);
                }
                list.Add(playerData);
            }
            return list;
        }

        private List<List<int[]>> CreateBossMechanicData(int phaseIndex)
        {
            List<List<int[]>> list = new List<List<int[]>>();
            HashSet<Mechanic> presMech = _log.MechanicData.GetPresentMechanics(0);
            PhaseData phase = _statistics.Phases[phaseIndex];
            foreach (AbstractMasterPlayer p in _log.MechanicData.GetEnemyList(0))
            {
                List<int[]> enemyData = new List<int[]>(presMech.Count);
                foreach (Mechanic mech in presMech)
                {
                    int count = _log.MechanicData[mech].Count(x => x.Player.InstID == p.InstID && phase.InInterval(x.Time));
                    enemyData.Add(new int[] { count, count });
                }
                list.Add(enemyData);
            }
            return list;
        }

        private string findPattern(string source, string regex)
        {
            if (String.IsNullOrEmpty(source)) return null;
            Match match = Regex.Match(source, regex);
            if (match.Success) return match.Groups[1].Value;
            return null;
        }

        private List<MechanicDto> CreateMechanicGraphData()
        {
            List<MechanicDto> mechanicDtos = new List<MechanicDto>();
            HashSet<Mechanic> playerMechs = _log.MechanicData.GetPresentPlayerMechs(0);
            HashSet<Mechanic> enemyMechs = _log.MechanicData.GetPresentEnemyMechs(0);
            foreach (Mechanic mech in _log.MechanicData.GetPresentMechanics(0))
            {
                List<MechanicLog> mechanicLogs = _log.MechanicData[mech];
                MechanicDto dto = new MechanicDto();
                dto.name = mech.ShortName;
                dto.description = mech.Description;
                dto.color = findPattern(mech.PlotlyShape,   "color\\s*:\\s*'([^']*)'");
                dto.symbol = findPattern(mech.PlotlyShape, "symbol\\s*:\\s*'([^']*)'");
                dto.visible = (mech.SkillId == -2 || mech.SkillId == -3);
                dto.data = BuildMechanicData(mechanicLogs);
                dto.playerMech = playerMechs.Contains(mech);
                dto.enemyMech = enemyMechs.Contains(mech);
                mechanicDtos.Add(dto);
            }
            //TODO add DOWN and DEAD data
            return mechanicDtos;
        }

        private List<List<List<double>>> BuildMechanicData(List<MechanicLog> mechanicLogs)
        {
            List<List<List<double>>> list = new List<List<List<double>>>();
            foreach (PhaseData phase in _statistics.Phases)
            {
                List<List<double>> phaseData = new List<List<double>>();
                list.Add(phaseData);
                Dictionary<long, int> playerIndexByInstId = new Dictionary<long, int>();
                for (var p = 0; p < _log.PlayerList.Count; p++)
                {
                    playerIndexByInstId.Add(_log.PlayerList[p].InstID, p);
                    phaseData.Add(new List<double>());
                }
                foreach (MechanicLog ml in mechanicLogs.Where(x => phase.InInterval(x.Time)))
                {
                    if (playerIndexByInstId.TryGetValue(ml.Player.InstID, out int p))
                    {
                        double time = (ml.Time - phase.Start) / 1000.0;
                        phaseData[p].Add(time);
                    }
                }
            }
            return list;
        }

        private List<List<List<int>>> CreateEnemyMechanicTable(int phaseIndex)
        {
            List<List<List<int>>> list = new List<List<List<int>>>();
            HashSet<Mechanic> presEnemyMech = _log.MechanicData.GetPresentEnemyMechs(phaseIndex);
            PhaseData phase = _statistics.Phases[phaseIndex];
            List<AbstractMasterPlayer> enemyList = _log.MechanicData.GetEnemyList(phaseIndex);

            foreach (AbstractMasterPlayer p in enemyList)
            {
                List<List<int>> enemyData = new List<List<int>>();
                foreach (Mechanic mech in presEnemyMech)
                {
                    int count = _log.MechanicData[mech].Count(x => x.Player.InstID == p.InstID && phase.InInterval(x.Time));
                    List<int> mechEntry = new List<int>(2);
                    mechEntry.Add(count);
                    mechEntry.Add(count);
                }
            }

            return list;
        }

        /// <summary>
        /// Creates the event list of the generation. Debug only
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateEventList(StreamWriter sw)
        {
            sw.Write("<ul class=\"list-group\">");
            {
                foreach (CombatItem c in _log.CombatData)
                {
                    if (c.IsStateChange != ParseEnum.StateChange.Normal)
                    {
                        AgentItem agent = _log.AgentData.GetAgent(c.SrcAgent);
                        if (agent != null)
                        {
                            switch (c.IsStateChange)
                            {
                                case ParseEnum.StateChange.EnterCombat:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " entered combat in" + c.DstAgent + "subgroup" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.ExitCombat:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " exited combat" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.ChangeUp:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " is now alive" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.ChangeDead:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " is now dead" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.ChangeDown:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " is now downed" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.Spawn:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " is now in logging range of POV player" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.Despawn:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " is now out of range of logging player" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.HealthUpdate:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " is at " + c.DstAgent / 100 + "% health" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.LogStart:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   " LOG START" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.LogEnd:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                  "LOG END" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.WeaponSwap:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " weapon swapped to " + c.DstAgent + "(0/1 water, 4/5 land)" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.MaxHealthUpdate:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " max health changed to  " + c.DstAgent +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.PointOfView:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.Name + " is recording log " +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                            }
                        }
                    }
                }
            }
            sw.Write("</ul>");
        }
        /// <summary>
        /// Creates a skill list. Debugging only
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateSkillList(StreamWriter sw)
        {
            sw.Write("<ul class=\"list-group\">");
            {
                foreach (SkillItem skill in _log.SkillData.Values)
                {
                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                  skill.ID + " : " + skill.Name +
                             "</li>");
                }
            }
            sw.Write("</ul>");
        }
        /// <summary>
        /// Creates the condition uptime table of the given boss
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="boss"></param>
        /// <param name="phaseIndex"></param>
        private void CreateCondiUptimeTable(StreamWriter sw, Boss boss, int phaseIndex)
        {
            List<PhaseData> phases = _statistics.Phases;
            long fightDuration = phases[phaseIndex].GetDuration();
            Dictionary<long, Statistics.FinalBossBoon> conditions = _statistics.BossConditions[phaseIndex];
            bool hasBoons = false;
            foreach (Boon boon in _statistics.PresentBoons)
            {
                if (boon.Name == "Retaliation")
                {
                    continue;
                }
                if (conditions[boon.ID].Uptime > 0.0)
                {
                    hasBoons = true;
                    break;
                }
            }
            Dictionary<long, long> condiPresence = boss.GetCondiPresence(_log, phaseIndex);
            double avgCondis = 0.0;
            foreach (long duration in condiPresence.Values)
            {
                avgCondis += duration;
            }
            avgCondis /= fightDuration;
            //Generate Boon table------------------------------------------------------------------------------------------------
            sw.Write("<h3 align=\"center\"> Condition Uptime </h3>");
            sw.Write("<script> $(function () { $('#condi_table" + phaseIndex + "').DataTable({ \"order\": [[3, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact mb-3\"  cellspacing=\"0\" width=\"100%\" id=\"condi_table" + phaseIndex + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Name</th>");
                        foreach (Boon boon in _statistics.PresentConditions)
                        {
                            if (hasBoons && boon.Name == "Retaliation")
                            {
                                continue;
                            }
                            sw.Write("<th>" + "<img src=\"" + boon.Link + " \" alt=\"" + boon.Name + "\" title =\" " + boon.Name + "\" height=\"18\" width=\"18\" >" + "</th>");
                        }
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</thead>");
                sw.Write("<tbody>");
                {
                    sw.Write("<tr>");
                    {
                        
                        sw.Write("<td style=\"width: 275px;\" data-toggle=\"tooltip\" title=\"Average number of conditions: " + Math.Round(avgCondis, 1) + "\">" + boss.Character + " </td>");
                        foreach (Boon boon in _statistics.PresentConditions)
                        {
                            if (hasBoons && boon.Name == "Retaliation")
                            {
                                continue;
                            }
                            if (boon.Type == Boon.BoonType.Duration)
                            {
                                sw.Write("<td>" + conditions[boon.ID].Uptime + "%</td>");
                            }
                            else
                            {
                                if (condiPresence.TryGetValue(boon.ID, out long presenceTime))
                                {
                                    string tooltip = "uptime: " + Math.Round(100.0 * presenceTime / fightDuration, 1) + "%";
                                    sw.Write("<td data-toggle=\"tooltip\" title=\"" + tooltip + "\">" + conditions[boon.ID].Uptime + " </td>");
                                }
                                else
                                {
                                   sw.Write("<td>" + conditions[boon.ID].Uptime + "</td>");
                                }
                            }
                        }
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</tbody>");
            }
            sw.Write("</table>");
            // Boon table if applicable
            if (hasBoons)
            {
                Dictionary<long, long> boonPresence = boss.GetBoonPresence(_log, phaseIndex);
                sw.Write("<h3 align=\"center\"> Boon Uptime </h3>");
                sw.Write("<script> $(function () { $('#boss_boon_table" + phaseIndex + "').DataTable({ \"order\": [[3, \"desc\"]]});});</script>");
                sw.Write("<table class=\"display table table-striped table-hover compact mb-3\"  cellspacing=\"0\" width=\"100%\" id=\"boss_boon_table" + phaseIndex + "\">");
                {
                    sw.Write("<thead>");
                    {
                        sw.Write("<tr>");
                        {
                            sw.Write("<th>Name</th>");
                            foreach (Boon boon in _statistics.PresentBoons)
                            {
                                sw.Write("<th>" + "<img src=\"" + boon.Link + " \" alt=\"" + boon.Name + "\" title =\" " + boon.Name + "\" height=\"18\" width=\"18\" >" + "</th>");
                            }
                        }
                        sw.Write("</tr>");
                    }
                    sw.Write("</thead>");
                    sw.Write("<tbody>");
                    {
                        sw.Write("<tr>");
                        {
                            sw.Write("<td style=\"width: 275px;\">" + boss.Character + " </td>");
                            foreach (Boon boon in _statistics.PresentBoons)
                            {
                                if (boon.Type == Boon.BoonType.Duration)
                                {
                                    sw.Write("<td>" + conditions[boon.ID].Uptime + "%</td>");
                                }
                                else
                                {
                                    if (boonPresence.TryGetValue(boon.ID, out long presenceTime))
                                    {
                                        string tooltip = "uptime: " + Math.Round(100.0 * presenceTime / fightDuration, 1) + "%";
                                        sw.Write("<td data-toggle=\"tooltip\" title=\"" + tooltip + "\">" + conditions[boon.ID].Uptime + " </td>");
                                    }
                                    else
                                    {
                                        sw.Write("<td>" + conditions[boon.ID].Uptime + "</td>");
                                    }
                                }
                            }
                        }
                        sw.Write("</tr>");
                    }
                    sw.Write("</tbody>");
                }
                sw.Write("</table>");
            }
            // Condition generation
            sw.Write("<h3 align=\"center\"> Condition Generation </h3>");
            sw.Write("<script> $(function () { $('#condigen_table" + phaseIndex + "').DataTable({ \"order\": [[3, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"condigen_table" + phaseIndex + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Sub</th>");
                        sw.Write("<th></th>");
                        sw.Write("<th>Name</th>");
                        foreach (Boon boon in _statistics.PresentConditions)
                        {
                            if (boon.Name == "Retaliation")
                            {
                                continue;
                            }
                            sw.Write("<th>" + "<img src=\"" + boon.Link + " \" alt=\"" + boon.Name + "\" title =\" " + boon.Name + "\" height=\"18\" width=\"18\" >" + "</th>");
                        }
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</thead>");
                sw.Write("<tbody>");
                {
                    foreach (Player player in _log.PlayerList)
                    {
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.Group.ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + HTMLHelper.GetLink(player.Prof) + "\" alt=\"" + player.Prof + "\" height=\"18\" width=\"18\" >" + "<span style=\"display:none\">" + player.Prof + "</span>" + "</td>");
                            sw.Write("<td>" + player.Character + " </td>");
                            foreach (Boon boon in _statistics.PresentConditions)
                            {
                                if (boon.Name == "Retaliation")
                                {
                                    continue;
                                }
                                Statistics.FinalBossBoon toUse = conditions[boon.ID];
                                if (boon.Type == Boon.BoonType.Duration)
                                {
                                    sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + toUse.Overstacked[player] + "% with overstack \">"
                                        + toUse.Generated[player]
                                        + "%</span>" + "</td>");
                                }
                                else
                                {
                                    sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + toUse.Overstacked[player] + " with overstack \">"
                                        + toUse.Generated[player]
                                        + "</span>" + " </td>");
                                }
                            }
                        }
                        sw.Write("</tr>");
                    }
                    
                }
                sw.Write("</tbody>");
            }
            sw.Write("</table>");
           
            
        }
        /// <summary>
        /// Creates the boss summary tab
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private void CreateBossSummary(StreamWriter sw, int phaseIndex)
        {
            //generate Player list Graphs
            List<PhaseData> phases = _statistics.Phases;
            PhaseData phase = phases[phaseIndex];
            List<CastLog> casting = _log.Boss.GetCastLogsActDur(_log, phase.Start, phase.End);
            string charname = _log.Boss.Character;
            string pid = _log.Boss.InstID + "_" + phaseIndex;
            sw.Write("<h1 align=\"center\"> " + charname + "</h1>");
            sw.Write("<ul class=\"nav nav-tabs\">");
            {
                sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#home" + pid + "\">" + _log.Boss.Character + "</a></li>");
                //foreach pet loop here
                foreach (KeyValuePair<string, Minions> pair in _log.Boss.GetMinions(_log))
                {
                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#minion" + pid + "_" + pair.Value.InstID + "\">" + pair.Key + "</a></li>");
                }
            }
            sw.Write("</ul>");
            //condi stats tab
            sw.Write("<div id=\"myTabContent\" class=\"tab-content\"><div class=\"tab-pane fade show active\" id=\"home" + pid + "\">");
            {
                CreateCondiUptimeTable(sw, _log.Boss, phaseIndex);
                sw.Write("<div id=\"Graph" + pid + "\" style=\"height: 800px;width:1000px; display:inline-block \"></div>");
                sw.Write("<script>");
                {
                    sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                    {
                        sw.Write("var data = [");
                        {
                            if (_settings.PlayerRot)//Display rotation
                            {

                                foreach (CastLog cl in casting)
                                {
                                    HTMLHelper.WriteCastingItem(sw, cl, _log.SkillData, phase.Start, phase.End);
                                }
                            }
                            //============================================
                            Dictionary<long, BoonsGraphModel> boonGraphData = _log.Boss.GetBoonGraphs(_log);
                            foreach (BoonsGraphModel bgm in boonGraphData.Values.Reverse().Where(x => x.BoonName != "Number of Boons"))
                            {
                                sw.Write("{");
                                {
                                    HTMLHelper.WritePlayerTabBoonGraph(sw, bgm, phase.Start, phase.End);
                                }
                                sw.Write(" },");

                            }
                            //int maxDPS = 0;
                            if (_settings.DPSGraphTotals)
                            {//show total dps plot
                                List<Point> playertotaldpsgraphdata = GraphHelper.GetTotalDPSGraph(_log, _log.Boss, phaseIndex, phase, GraphHelper.GraphMode.Full);
                                sw.Write("{");
                                {
                                    //Adding dps axis
                                    HTMLHelper.WritePlayerTabDPSGraph(sw, "Total DPS", playertotaldpsgraphdata, _log.Boss);
                                }
                                sw.Write("},");
                            }
                            sw.Write("{");
                         //TODO   HTMLHelper.WriteBossHealthGraph(sw, GraphHelper.GetTotalDPSGraph(_log, _log.Boss, phaseIndex, phase, GraphHelper.GraphMode.Full).Max(x => x.Y), phase.Start, phase.End, _log.Boss, "y3");
                            sw.Write("}");
                        }
                        sw.Write("];");
                        sw.Write("var layout = {");
                        {
                            sw.Write("barmode:'stack',");
                            sw.Write("yaxis: {" +
                                   "title: 'Rotation', domain: [0, 0.09], fixedrange: true, showgrid: false," +
                                   "range: [0, 2]" +
                               "}," +

                               "legend: { traceorder: 'reversed' }," +
                               "hovermode: 'compare'," +
                               "yaxis2: { title: 'Condis/Boons', domain: [0.11, 0.50], fixedrange: true }," +
                               "yaxis3: { title: 'DPS', domain: [0.51, 1] },");
                            sw.Write("images: [");
                            {
                                if (_settings.PlayerRotIcons)//Display rotation
                                {
                                    int castCount = 0;
                                    foreach (CastLog cl in casting)
                                    {
                                        HTMLHelper.WriteCastingItemIcon(sw, cl, _log.SkillData, phase.Start, castCount == casting.Count - 1);
                                        castCount++;
                                    }
                                }
                            }
                            sw.Write("],");
                            if (_settings.LightTheme)
                            {
                                sw.Write("font: { color: '#000000' }," +
                                         "paper_bgcolor: 'rgba(255, 255, 255, 0)'," +
                                         "plot_bgcolor: 'rgba(255, 255, 255, 0)'");
                            }
                            else
                            {
                                sw.Write("font: { color: '#ffffff' }," +
                                         "paper_bgcolor: 'rgba(0,0,0,0)'," +
                                         "plot_bgcolor: 'rgba(0,0,0,0)'");
                            }
                        }
                        sw.Write("};");
                        sw.Write(
                                "var lazyplot = document.querySelector('#Graph" + pid + "');" +

                                "if ('IntersectionObserver' in window) {" +
                                    "let lazyPlotObserver = new IntersectionObserver(function(entries, observer) {" +
                                        "entries.forEach(function(entry) {" +
                                            "if (entry.isIntersecting)" +
                                            "{" +
                                                "Plotly.newPlot('Graph" + pid + "', data, layout);" +
                                                "lazyPlotObserver.unobserve(entry.target);" +
                                            "}" +
                                        "});" +
                                    "});" +
                                    "lazyPlotObserver.observe(lazyplot);" +
                                "} else {"+
                                    "Plotly.newPlot('Graph" + pid + "', data, layout);" +
                                "}");
                    }
                    sw.Write("});");
                }
                sw.Write("</script> ");
                //CreateBossDMGDistTable(_log.Boss, phaseIndex);
                sw.Write("</div>");
                foreach (KeyValuePair<string, Minions> pair in _log.Boss.GetMinions(_log))
                {
                    sw.Write("<div class=\"tab-pane fade \" id=\"minion" + pid + "_" + pair.Value.InstID + "\">");
                    {
                        CreateBossMinionDMGDistTable(sw, _log.Boss, pair.Value, phaseIndex);
                    }
                    sw.Write("</div>");
                }
            }
            sw.Write("</div>");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private void CreateEstimateTabs(StreamWriter sw, int phaseIndex)
        {
            sw.Write("<ul class=\"nav nav-tabs\">");
            {
                sw.Write("<li class=\"nav-item\">" +
                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#est_role" + phaseIndex + "\">Roles</a>" +
                        "</li>" +

                        "<li class=\"nav-item\">" +
                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#est_cc" + phaseIndex + "\">CC</a>" +
                        "</li>" +
                         "<li class=\"nav-item\">" +
                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#est" + phaseIndex + "\">Maybe more</a>" +
                        "</li>");
            }
            sw.Write("</ul>");
            sw.Write("<div id=\"myTabContent\" class=\"tab-content\">");
            {
                sw.Write("<div class=\"tab-pane fade show active\" id=\"est_role" + phaseIndex + "\">");
                {
                    //Use cards
                }
                sw.Write("</div>");
                sw.Write("<div class=\"tab-pane fade show active\" id=\"est_cc" + phaseIndex + "\">");
                {
                }
                sw.Write("</div>");
                sw.Write("<div class=\"tab-pane fade show active\" id=\"est" + phaseIndex + "\">");
                {
                }
                sw.Write("</div>");
            }
            sw.Write("</div>");
        }
        /// <summary>
        /// Creates the combat replay tab
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateReplayTable(StreamWriter sw)
        {
            CombatReplayMap map = _log.Boss.GetCombatMap(_log);
            Tuple<int, int> canvasSize = map.GetPixelMapSize();
            HTMLHelper.WriteCombatReplayInterface(sw, canvasSize, _log);
            HTMLHelper.WriteCombatReplayScript(sw, _log, canvasSize, map, _settings.PollingRate);
        }
        /// <summary>
        /// Creates custom css'
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="simpleRotSize">Size of the simple rotation images</param>
        private void CreateCustomCSS(StreamWriter sw, int simpleRotSize)
        {
            sw.Write("<style>");
            {
                sw.Write("td, th {text-align: center; white-space: nowrap;}");
                sw.Write(".sorting_disabled {padding: 5px !important;}");
                sw.Write("th.dt-left, td.dt-left { text-align: left; }");
                sw.Write("div.dataTables_wrapper { width: 1100px; margin: 0 auto; }");
                sw.Write(".text-left {text-align: left;}");
                sw.Write("table.dataTable thead.sorting_asc{color: green;}");
                sw.Write(".rot-skill{width: " + simpleRotSize + "px;height: " + simpleRotSize + "px; display: inline-block;}");
                sw.Write(".rot-crop{width : " + simpleRotSize + "px;height: " + simpleRotSize + "px; display: inline-block}");
                sw.Write(".rot-table {width: 100%;border-collapse: separate;border-spacing: 5px 0px;}");
                sw.Write(".rot-table > tbody > tr > td {padding: 1px;text-align: left;}");
                sw.Write(".rot-table > thead {vertical-align: bottom;border-bottom: 2px solid #ddd;}");
                sw.Write(".rot-table > thead > tr > th {padding: 10px 1px 9px 1px;line-height: 18px;text-align: left;}");
                sw.Write("table.dataTable.table-condensed.sorting, table.dataTable.table-condensed.sorting_asc, table.dataTable.table-condensed.sorting_desc ");
                sw.Write("{right: 4px !important;}table.dataTable thead.sorting_desc { color: red;}");
                sw.Write("table.dataTable.table-condensed > thead > tr > th.sorting { padding-right: 5px !important; }");
                sw.Write("tr.even{ background-color: #F9F9F9 !important; }");
                sw.Write("tr.odd{ background-color: #D9D9D9 !important; }");
                sw.Write("tr.odd>.sorting_1{ background-color: #D0D0D0 !important; }");
                sw.Write("tr.even>.sorting_1{ background-color: #F0F0F0 !important; }");
                sw.Write("table.dataTable.display tbody tr.condi {background-color: #ff6666 !important;}");
                if (!_settings.LightTheme)
                {
                    sw.Write("table.dataTable.stripe tfoot tr, table.dataTable.display tfoot tr { background-color: #f9f9f9;}");
                    sw.Write("table.dataTable  td {color: black;}");
                    sw.Write(".card {border:1px solid #EE5F5B;}");
                    sw.Write("td.composition {width: 120px;border:1px solid #EE5F5B;}");
                }
                else
                {
                    sw.Write(".nav-link {color:#337AB7;}");
                    sw.Write(".card {border:1px solid #9B0000;}");
                    sw.Write("td.composition {width: 120px;border:1px solid #9B0000;}");
                }
                if (_log.Boss.CombatReplay != null)
                {
                    // from W3
                    sw.Write(".slidecontainer {width: 100%;}");
                    sw.Write(".slider {width: 100%;appearance: none;height: 25px;background: #F3F3F3;outline: none;opacity: 0.7;-webkit-transition: .2s;transition: opacity .2s;}");
                    sw.Write(".slider:hover {opacity: 1;}");
                    sw.Write(".slider::-webkit-slider-thumb {-webkit-appearance: none;appearance: none;width: 25px;height: 25px;background: #4CAF50;cursor: pointer;}");
                    sw.Write(".slider::-moz-range-thumb {width: 25px;height: 25px;background: #4CAF50;cursor: pointer;}");
                }
            }
            sw.Write("</style>");
        }

        private String ReplaceVariables(String html)
        {
            double fightDuration = _log.FightData.FightDuration / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fightDuration);
            string durationString = duration.Minutes + "m " + duration.Seconds + "s " + duration.Milliseconds + "ms";
            if (duration.Hours > 0)
            {
                durationString = duration.Hours + "h " + durationString;
            }

            int encounterPercent = 0;
            double healthLeft = 100;
            
            if (_log.LogData.Success)
            {
                encounterPercent = 100;
                healthLeft = 0;
            }
            else
            {
                if (_log.FightData.HealthOverTime.Count > 0)
                {
                    healthLeft = Math.Round(_log.FightData.HealthOverTime[_log.FightData.HealthOverTime.Count - 1].Y * 0.01, 2);
                    encounterPercent = (int)Math.Floor(100.0 - _log.FightData.HealthOverTime[_log.FightData.HealthOverTime.Count - 1].Y * 0.01);
                }
            }

            html = html.Replace("${bootstrapTheme}", !_settings.LightTheme ? "slate" : "cosmo");

            html = html.Replace("${encounterStart}", _log.LogData.LogStart);
            html = html.Replace("${encounterEnd}", _log.LogData.LogEnd);
            html = html.Replace("${encounterDuration}", durationString);
            html = html.Replace("${encounterResult}", _log.LogData.Success ? "Success": "Fail");
            html = html.Replace("${encounterResultCss}", _log.LogData.Success ? "text-success" : "text-warning");
            html = html.Replace("${encounterPercent}", encounterPercent.ToString());
            html = html.Replace("${evtcVersion}", _log.LogData.BuildVersion);
            html = html.Replace("${bossID}", _log.FightData.ID.ToString());
            html = html.Replace("${bossName}", FilterStringChars(_log.FightData.Name));
            html = html.Replace("${bossHealth}", _log.FightData.Health.ToString());
            html = html.Replace("${bossHealthLeft}", healthLeft.ToString());
            html = html.Replace("${bossIcon}", _log.FightData.Logic.IconUrl);
            html = html.Replace("${eiVersion}", Application.ProductVersion);
            html = html.Replace("${recordedBy}", _log.LogData.PoV.Split(':')[0]);

            return html;
        }

        /// <summary>
        /// Creates the whole html
        /// </summary>
        /// <param name="sw">Stream writer</param>
        public void CreateHTML(StreamWriter sw, String path)
        {
            string html = Properties.Resources.template_html;
            html = ReplaceVariables(html);

            html = html.Replace("<!--${flomixCss}-->", BuildFlomixCss(path));
            html = html.Replace("<!--${flomixJs}-->", BuildFlomixJs(path));

            html = html.Replace("${logDataJson}", BuildLogData());

            html = html.Replace("<!--${playerData}-->", BuildPlayerData());

            html = html.Replace("${graphDataJson}", BuildGraphJson());

            html = html.Replace("<!--${combatReplay}-->", BuildCombatReplayContent());

            sw.Write(html);
            return;

            /*


            double fightDuration = _log.FightData.FightDuration / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fightDuration);
            string durationString = duration.Minutes + "m " + duration.Seconds + "s " + duration.Milliseconds + "ms";
            if (duration.Hours > 0)
            {
                durationString = duration.Hours + "h " + durationString;
            }
            string bossname = FilterStringChars(_log.Boss.Name);
            List<PhaseData> phases = _statistics.Phases;
            // HTML STARTS
            sw.Write("<!DOCTYPE html><html lang=\"en\">");
            {
                sw.Write("<head>");
                {
                    sw.Write("<meta charset=\"utf-8\">");
                    sw.Write(!_settings.LightTheme ?
                            "<link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/bootswatch/4.1.1/darkly/bootstrap.min.css\"  crossorigin=\"anonymous\">"
                            :
                            "<link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/bootswatch/4.1.1/cosmo/bootstrap.min.css\"  crossorigin=\"anonymous\">"
                        );

                    sw.Write("<link href=\"https://fonts.googleapis.com/css?family=Open+Sans\" rel=\"stylesheet\">" +
                      "<link rel=\"stylesheet\" type=\"text/css\" href=\"https://cdn.datatables.net/1.10.16/css/jquery.dataTables.min.css\">" +
                      //JQuery
                      "<script src=\"https://code.jquery.com/jquery-3.3.1.js\"></script> " +
                      //popper
                      "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js\"></script>" +
                      //js
                      "<script src=\"https://cdn.plot.ly/plotly-latest.min.js\"></script>" +
                      "<script src=\"https://cdn.datatables.net/1.10.16/js/jquery.dataTables.min.js\"></script>" +
                      "<script src=\"https://cdn.datatables.net/plug-ins/1.10.13/sorting/alt-string.js\"></script>" +
                      "<script src=\"https://stackpath.bootstrapcdn.com/bootstrap/4.1.1/js/bootstrap.min.js\"></script>");
                    int simpleRotSize = 20;
                    if (_settings.LargeRotIcons)
                    {
                        simpleRotSize = 30;
                    }
                    CreateCustomCSS(sw, simpleRotSize);
                }
                sw.Write("<script>$.extend( $.fn.dataTable.defaults, {searching: false, ordering: true,paging: false,dom:\"t\"} );</script>");
                sw.Write("</head>");
                sw.Write("<body class=\"d-flex flex-column align-items-center\">");
                {
                    sw.Write("<div style=\"width: 1100px;\"class=\"d-flex flex-column\">");
                    {
                        sw.Write("<p> Time Start: " + _log.LogData.GetLogStart() + " | Time End: " + _log.LogData.GetLogEnd() + " </p> ");
                        sw.Write("<div class=\"d-flex flex-row justify-content-center align-items-center flex-wrap mb-3\">");
                        {
                            sw.Write("<div class=\"mr-3\">");
                            {
                                sw.Write("<div style=\"width: 400px;;\" class=\"card d-flex flex-column\">");
                                {
                                    sw.Write("<h3 class=\"card-header text-center\">" + bossname + "</h3>");
                                    sw.Write("<div class=\"card-body d-flex flex-column align-items-center\">");
                                    {
                                        sw.Write("<blockquote class=\"card-blockquote mb-0\">");
                                        {
                                            sw.Write("<div style=\"width: 300px;\" class=\"d-flex flex-row justify-content-between align-items-center\">");
                                            {
                                                sw.Write("<div>");
                                                {
                                                    sw.Write("<img src=\"" + HTMLHelper.GetLink(_log.Boss.ID + "-icon") + "\"alt=\"" + bossname + "-icon" + "\" style=\"height: 120px; width: 120px;\" >");
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div>");
                                                {
                                                    sw.Write("<div class=\"progress\" style=\"width: 100 %; height: 20px;\">");
                                                    {
                                                        if (_log.LogData.GetBosskill())
                                                        {
                                                            string tp = _log.Boss.GetHealth().ToString() + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-success\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:100%; ;\" aria-valuenow=\"100\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");
                                                        }
                                                        else
                                                        {
                                                            double finalPercent = 0;
                                                            if (_log.Boss.GetHealthOverTime().Count > 0)
                                                            {
                                                                finalPercent = 100.0 - _log.Boss.GetHealthOverTime()[_log.Boss.GetHealthOverTime().Count - 1].Y * 0.01;
                                                            }
                                                            string tp = Math.Round(_log.Boss.GetHealth() * finalPercent / 100.0) + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-success\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:" + finalPercent + "%;\" aria-valuenow=\"" + finalPercent + "\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");
                                                            tp = Math.Round(_log.Boss.GetHealth() * (100.0 - finalPercent) / 100.0) + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-danger\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:" + (100.0 - finalPercent) + "%;\" aria-valuenow=\"" + (100.0 - finalPercent) + "\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");

                                                        }
                                                    }
                                                    sw.Write("</div>");
                                                    sw.Write("<p class=\"small\" style=\"text-align:center; color: "+ (_settings.LightTheme ? "#000" : "#FFF") +";\">" + _log.Boss.GetHealth().ToString() + " Health</p>");
                                                    sw.Write(_log.LogData.GetBosskill() ? "<p class='text text-success'> Result: Success</p>" : "<p class='text text-warning'> Result: Fail</p>");
                                                    sw.Write("<p>Duration: " + durationString + " </p> ");
                                                }
                                                sw.Write("</div>");
                                            }
                                            sw.Write("</div>");
                                        }
                                        sw.Write("</blockquote>");
                                    }
                                    sw.Write("</div>");
                                }
                                sw.Write("</div>");
                            }
                            sw.Write("</div>");
                            sw.Write("<div class=\"ml-3 mt-3\">");
                            {
                                CreateCompTable(sw);
                            }
                            sw.Write("</div>");
                        }
                        sw.Write("</div>");
                        //if (p_list.Count == 1)//Create condensed version of log
                        //{
                        //    CreateSoloHTML(sw,settingsSnap);
                        //    return;
                        //}
                        if (phases.Count > 1 || _log.Boss.GetCombatReplay() != null)
                        {
                            sw.Write("<ul class=\"nav nav-tabs\">");
                            {
                                for (int i = 0; i < phases.Count; i++)
                                {
                                    if (phases[i].GetDuration() == 0)
                                        continue;
                                    string active = (i > 0 ? "" : "active");
                                    string name = phases[i].Name;
                                    sw.Write("<li  class=\"nav-item\">" +
                                            "<a class=\"nav-link " + active + "\" data-toggle=\"tab\" href=\"#phase" + i + "\">" +
                                                "<span data-toggle=\"tooltip\" title=\"" + phases[i].GetDuration("s") + " seconds\">" + name + "</span>" +
                                            "</a>" +
                                        "</li>");
                                }
                                if (_log.Boss.GetCombatReplay() != null)
                                {
                                    sw.Write("<li  class=\"nav-item\">" +
                                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#replay\">" +
                                                "<span>Combat Replay</span>" +
                                            "</a>" +
                                        "</li>");
                                }
                            }
                            sw.Write("</ul>");
                        }
                        sw.Write("<div id=\"myTabContent" + "\" class=\"tab-content\">");
                        {
                            for (int i = 0; i < phases.Count; i++)
                            {
                                string active = (i > 0 ? "" : "show active");

                                if (phases[i].GetDuration() == 0)
                                    continue;
                                sw.Write("<div class=\"tab-pane fade " + active + "\" id=\"phase" + i + "\">");
                                {
                                    if (phases.Count > 1)
                                        sw.Write("<h2 align=\"center\">"+ phases[i].Name+ "</h2>");
                                    string playerDropdown = "";
                                    foreach (Player p in _log.PlayerList)
                                    {
                                        string charname = p.Character;
                                        playerDropdown += "<a class=\"dropdown-item\"  data-toggle=\"tab\" href=\"#" + p.InstID + "_" + i + "\">" + charname +
                                            "<img src=\"" + HTMLHelper.GetLink(p.Prof) + "\" alt=\"" + p.Prof + "\" height=\"18\" width=\"18\" >" + "</a>";
                                    }
                                    sw.Write("<ul class=\"nav nav-tabs\">");
                                    {
                                        sw.Write("<li class=\"nav-item\">" +
                                                    "<a class=\"nav-link active\" data-toggle=\"tab\" href=\"#stats" + i + "\">Stats</a>" +
                                                "</li>" +

                                                "<li class=\"nav-item\">" +
                                                    "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#dmgGraph" + i + "\">Damage Graph</a>" +
                                                "</li>" +
                                                 "<li class=\"nav-item\">" +
                                                    "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#boons" + i + "\">Boons</a>" +
                                                "</li>" +
                                                "<li class=\"nav-item\">" +
                                                    "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#mechTable" + i + "\">Mechanics</a>" +
                                                "</li>" +
                                                "<li class=\"nav-item dropdown\">" +
                                                    "<a class=\"nav-link dropdown-toggle\" data-toggle=\"dropdown\" href=\"#\" role=\"button\" aria-haspopup=\"true\" aria-expanded=\"true\">Player</a>" +
                                                    "<div class=\"dropdown-menu \" x-placement=\"bottom-start\">" +
                                                        playerDropdown +
                                                    "</div>" +
                                                "</li>");
                                        if (_settings.BossSummary)
                                        {
                                            sw.Write("<li class=\"nav-item\">" +
                                                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#bossSummary" + i + "\">Boss</a>" +
                                                        "</li>");
                                        }
                                        if (_settings.EventList)
                                        {
                                            sw.Write("<li class=\"nav-item\">" +
                                                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#eventList" + i + "\">Event List</a>" +
                                                        "</li>");
                                        }
                                        if (_settings.ShowEstimates)
                                        {
                                            sw.Write("<li class=\"nav-item\">" +
                                                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#estimates" + i + "\">Estimates</a>" +
                                                        "</li>");
                                        }
                                    }
                                    sw.Write("</ul>");
                                    sw.Write("<div id=\"myTabContent" + "\" class=\"tab-content\">");
                                    {
                                        sw.Write("<div class=\"tab-pane fade show active\" id=\"stats" + i + "\">");
                                        {
                                            //Stats Tab
                                            sw.Write("<h3 align=\"center\"> Stats </h3>");

                                            sw.Write("<ul class=\"nav nav-tabs\">" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#dpsStats" + i + "\">DPS</a></li>" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offStats" + i + "\">Damage Stats</a></li>" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defStats" + i + "\">Defensive Stats</a></li>" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#healStats" + i + "\">Heal Stats</a></li>" +
                                                "</ul>");
                                            sw.Write("<div id=\"statsSubTab" + i + "\" class=\"tab-content\">");
                                            {
                                                sw.Write("<div class=\"tab-pane fade show active\" id=\"dpsStats" + i + "\">");
                                                {
                                                    // DPS table
                                                    //CreateDPSTable(sw, i);
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div class=\"tab-pane fade \" id=\"offStats" + i + "\">");
                                                {
                                                    string bossText = phases[i].Redirection.Count > 0 ? "Adds" : "Boss";
                                                    sw.Write("<ul class=\"nav nav-tabs\">" +
                                                       "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#dpsStatsBoss" + i + "\">"+ bossText + "</a></li>" +
                                                       "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#dpsStatsAll" + i + "\">All</a></li>" +
                                                     "</ul>");
                                                    sw.Write("<div id=\"subtabcontent" + "\" class=\"tab-content\">");
                                                    {
                                                        sw.Write("<div class=\"tab-pane fade show active \" id=\"dpsStatsBoss" + i + "\">");
                                                        {
                                                            //dmgstatsBoss
                                                            //CreateDMGStatsBossTable(sw, i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade \" id=\"dpsStatsAll" + i + "\">");
                                                        {
                                                            // dmgstats 
                                                            //CreateDMGStatsTable(sw, i);
                                                        }
                                                        sw.Write("</div>");
                                                    }
                                                    sw.Write("</div>");
                                                }
                                                sw.Write("</div>");
                                               


                                                sw.Write("<div class=\"tab-pane fade \" id=\"defStats" + i + "\">");
                                                {
                                                    // def stats
                                                    //CreateDefTable(sw, i);
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div class=\"tab-pane fade\" id=\"healStats" + i + "\">");
                                                {
                                                    //  supstats
                                                    //CreateSupTable(sw, i);
                                                }
                                                sw.Write("</div>");
                                            }
                                            sw.Write("</div>");

                                        }
                                        sw.Write("</div>");

                                        sw.Write("<div class=\"tab-pane fade\" id=\"dmgGraph" + i + "\">");
                                        {
                                            //dpsGraph
                                            sw.Write("<ul class=\"nav nav-tabs\">");
                                            {
                                                if (_settings.Show10s || _settings.Show30s)
                                                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#Full" + i + "\">Full</a></li>");
                                                if (_settings.Show10s)
                                                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#10s" + i + "\">10s</a></li>");
                                                if (_settings.Show30s)
                                                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#30s" + i + "\">30s</a></li>");
                                            }
                                            sw.Write("</ul>");
                                            sw.Write("<div id=\"dpsSubTab" + i + "\" class=\"tab-content\">");
                                            {
                                                sw.Write("<div class=\"tab-pane fade show active  \" id=\"Full" + i + "\">");
                                                {
                                                  //  CreateDPSGraphData(sw, i, GraphHelper.GraphMode.Full);
                                                }
                                                sw.Write("</div>");
                                                if (_settings.Show10s)
                                                {
                                                    sw.Write("<div class=\"tab-pane fade \" id=\"10s" + i + "\">");
                                                    {
                                                     //   CreateDPSGraphData(sw, i, GraphHelper.GraphMode.S10);
                                                    }
                                                    sw.Write("</div>");
                                                }
                                                if (_settings.Show30s)
                                                {
                                                    sw.Write("<div class=\"tab-pane fade \" id=\"30s" + i + "\">");
                                                    {
                                                     //   CreateDPSGraphData(sw, i, GraphHelper.GraphMode.S30);
                                                    }
                                                    sw.Write("</div>");
                                                }
                                            }
                                            sw.Write("</div>");
                                        }
                                        sw.Write("</div>");
                                        //Boon Stats
                                        sw.Write("<div class=\"tab-pane fade \" id=\"boons" + i + "\">");
                                        {
                                            //Boons Tab
                                            sw.Write("<h3 align=\"center\"> Boons </h3>");

                                            sw.Write("<ul class=\"nav nav-tabs\">" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#mainBoon" + i + "\">Boons</a></li>" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offBuff" + i + "\">Damage Buffs</a></li>" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defBuff" + i + "\">Defensive Buffs</a></li>" +
                                                "</ul>");
                                            sw.Write("<div id=\"boonsSubTab" + i + "\" class=\"tab-content\">");
                                            {
                                                sw.Write("<div class=\"tab-pane fade show active  \" id=\"mainBoon" + i + "\">");
                                                {
                                                    sw.Write("<ul class=\"nav nav-tabs\">" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#boonsUptime" + i + "\">Uptime</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#boonsGenSelf" + i + "\">Generation (Self)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#boonsGenGroup" + i + "\">Generation (Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#boonsGenOGroup" + i + "\">Generation (Off-Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#boonsGenSquad" + i + "\">Generation (Squad)</a></li>" +
                                                           "</ul>");
                                                    sw.Write("<div id=\"mainBoonsSubTab" + i + "\" class=\"tab-content\">");
                                                    {
                                                        sw.Write("<div class=\"tab-pane fade show active\" id=\"boonsUptime" + i + "\">");
                                                        {
                                                            sw.Write("<p> Boon Uptime</p>");
                                                            // boons
                                                            //CreateUptimeTable(sw, _statistics.PresentBoons, "boons_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenSelf" + i + "\">");
                                                        {
                                                            //boonGenSelf
                                                            sw.Write("<p> Boons generated by a character for themselves</p>");
                                                            //CreateGenSelfTable(sw, _statistics.PresentBoons, "boongenself_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Boons generated by a character for their groupmates</p>");
                                                            // boonGenGroup
                                                            //CreateGenGroupTable(sw, _statistics.PresentBoons, "boongengroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenOGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Boons generated by a character for any subgroup that is not their own</p>");
                                                            // boonGenOGroup
                                                            //CreateGenOGroupTable(sw, _statistics.PresentBoons, "boongenogroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenSquad" + i + "\">");
                                                        {
                                                            sw.Write("<p> Boons generated by a character for their squadmates</p>");
                                                            //  boonGenSquad
                                                            //CreateGenSquadTable(sw, _statistics.PresentBoons, "boongensquad_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                    }
                                                    sw.Write("</div>");
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div class=\"tab-pane fade  \" id=\"offBuff" + i + "\">");
                                                {
                                                    sw.Write("<ul class=\"nav nav-tabs\">" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#offensiveUptime" + i + "\">Uptime</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offensiveGenSelf" + i + "\">Generation (Self)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offensiveGenGroup" + i + "\">Generation (Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offensiveGenOGroup" + i + "\">Generation (Off-Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offensiveGenSquad" + i + "\">Generation (Squad)</a></li>" +
                                                           "</ul>");
                                                    sw.Write("<div id=\"offBuffSubTab" + i + "\" class=\"tab-content\">");
                                                    {
                                                        //Offensive Buffs stats
                                                        sw.Write("<div class=\"tab-pane fade show active\" id=\"offensiveUptime" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs Uptime</p>");
                                                            //CreateUptimeTable(sw, _statistics.PresentOffbuffs, "offensive_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenSelf" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs generated by a character for themselves</p>");
                                                            //CreateGenSelfTable(sw, _statistics.PresentOffbuffs, "offensivegenself_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs generated by a character for their groupmates</p>");
                                                            //CreateGenGroupTable(sw, _statistics.PresentOffbuffs, "offensivegengroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenOGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs generated by a character for any subgroup that is not their own</p>");
                                                            //CreateGenOGroupTable(sw, _statistics.PresentOffbuffs, "offensivegenogroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenSquad" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs generated by a character for their squadmates</p>");
                                                            //CreateGenSquadTable(sw, _statistics.PresentOffbuffs, "offensivegensquad_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                    }
                                                    sw.Write("</div>");
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div class=\"tab-pane fade  \" id=\"defBuff" + i + "\">");
                                                {
                                                    sw.Write("<ul class=\"nav nav-tabs\">" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#defensiveUptime" + i + "\">Uptime</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defensiveGenSelf" + i + "\">Generation (Self)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defensiveGenGroup" + i + "\">Generation (Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defensiveGenOGroup" + i + "\">Generation (Off-Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defensiveGenSquad" + i + "\">Generation (Squad)</a></li>" +
                                                           "</ul>");
                                                    sw.Write("<div id=\"defBuffSubTab" + i + "\" class=\"tab-content\">");
                                                    {
                                                        //Defensive Buffs stats
                                                        sw.Write("<div class=\"tab-pane fade show active\" id=\"defensiveUptime" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs Uptime</p>");
                                                            //CreateUptimeTable(sw, _statistics.PresentDefbuffs, "defensive_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenSelf" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs generated by a character for themselves</p>");
                                                            //CreateGenSelfTable(sw, _statistics.PresentDefbuffs, "defensivegenself_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs generated by a character for their groupmates</p>");
                                                            //CreateGenGroupTable(sw, _statistics.PresentDefbuffs, "defensivegengroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenOGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs generated by a character for any subgroup that is not their own</p>");
                                                            //CreateGenOGroupTable(sw, _statistics.PresentDefbuffs, "defensivegenogroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenSquad" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs generated by a character for their squadmates</p>");
                                                            //CreateGenSquadTable(sw, _statistics.PresentDefbuffs, "defensivegensquad_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                    }
                                                    sw.Write("</div>");
                                                }
                                                sw.Write("</div>");
                                            }
                                            sw.Write("</div>");
                                        }
                                        sw.Write("</div>");
                                        //mechanics
                                        sw.Write("<div class=\"tab-pane fade\" id=\"mechTable" + i + "\">");
                                        {
                                            sw.Write("<p>Mechanics</p>");
                                            //CreateMechanicTable(sw, i);
                                        }
                                        sw.Write("</div>");
                                        //boss summary
                                        if (_settings.BossSummary)
                                        {
                                            sw.Write("<div class=\"tab-pane fade\" id=\"bossSummary" + i + "\">");
                                            {
                                                CreateBossSummary(sw, i);
                                            }
                                            sw.Write("</div>");
                                        }
                                        //event list
                                        if (_settings.EventList && i == 0)
                                        {
                                            sw.Write("<div class=\"tab-pane fade\" id=\"eventList" + i + "\">");
                                            {
                                                sw.Write("<p>List of all events.</p>");
                                                // CreateEventList(sw);
                                                CreateSkillList(sw);
                                            }
                                            sw.Write("</div>");
                                        }
                                        //boss summary
                                        if (_settings.ShowEstimates)
                                        {
                                            sw.Write("<div class=\"tab-pane fade\" id=\"estimates" + i + "\">");
                                            {
                                                CreateEstimateTabs(sw, i);
                                            }
                                            sw.Write("</div>");
                                        }
                                        //playertabs
                                        CreatePlayerTab(sw, i);
                                    }
                                    sw.Write("</div>");
                                }
                                sw.Write("</div>");

                            }
                            if (_log.Boss.GetCombatReplay() != null)
                            {
                                sw.Write("<div class=\"tab-pane fade\" id=\"replay\">");
                                {
                                    CreateReplayTable(sw);
                                }
                            }
                        }
                        sw.Write("</div>");
                        sw.Write("<p style=\"margin-top:10px;\"> ARC:" + _log.LogData.GetBuildVersion() + " | Bossid " + _log.Boss.ID.ToString() + "| EI Version: " +Application.ProductVersion + " </p> ");
                       
                        sw.Write("<p style=\"margin-top:-15px;\">File recorded by: " + _log.LogData.GetPOV().Split(':')[0] + "</p>");
                    }
                    sw.Write("</div>");
                }
                sw.Write("</body>");
                sw.Write("<script> $(document).ready(function(){$('[data-toggle=\"tooltip\"]').tooltip(); });</script >");
            }
            //end
            sw.Write("</html>");

            */
        }

        private string BuildCombatReplayContent()
        {
            if (!_settings.ParseCombatReplay || !_log.FightData.Logic.CanCombatReplay)
            {
                return "";
            }
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write("<div id=\"replay_template\">");
                    CreateReplayTable(sw);
                    sw.Write("</div>");
                }
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private string BuildFlomixCss(string path)
        {
            string scriptContent = Properties.Resources.flomix_ei_css;
            string cssFilename = "flomix-ei-" + scriptVersion + ".css";
            if (Properties.Settings.Default.NewHtmlExternalScripts)
            {
                string cssPath = Path.Combine(path, cssFilename);
                using (var fs = new FileStream(cssPath, FileMode.Create, FileAccess.Write))
                using (var scriptWriter = new StreamWriter(fs))
                {
                    scriptWriter.Write(scriptContent);
                }
                return "<link rel=\"stylesheet\" type=\"text/css\" href=\"./"+ cssFilename + "?version="+scriptVersionRev+"\">";
            }
            else
            {
                return "<style type=\"text/css\">\r\n" + scriptContent + "\r\n</style>";
            }
        }

        private string BuildFlomixJs(string path)
        {
            String scriptContent = buildJavascript();
            string scriptFilename = "flomix-ei-" + scriptVersion + ".js";
            if (Properties.Settings.Default.NewHtmlExternalScripts)
            {
                string scriptPath = Path.Combine(path, scriptFilename);
                using (var fs = new FileStream(scriptPath, FileMode.Create, FileAccess.Write))
                using (var scriptWriter = new StreamWriter(fs))
                {
                    scriptWriter.Write(scriptContent);
                }
                return "<script src=\"./" + scriptFilename + "?version=" + scriptVersionRev + "\"></script>";
            }
            else
            {
                return "<script>\r\n" + scriptContent + "\r\n</script>";
            }
        }

        private string BuildGraphJson()
        {
            List<PhaseChartDataDto> chartData = new List<PhaseChartDataDto>();
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                PhaseChartDataDto phaseData = new PhaseChartDataDto();
                phaseData.bossHealth = CreateBossHealthData(i);
                phaseData.players = CreateDPSGraphData(i);
                chartData.Add(phaseData);
             }
            return ToJson(chartData, typeof(List<PhaseChartDataDto>));
        }

        private string BuildLogData()
        {
            LogDataDto data = new LogDataDto();
            foreach(Player player in _log.PlayerList)
            {
                PlayerDto playerDto = new PlayerDto(
                    player.Group,
                    player.Character,
                    player.Account.TrimStart(':'),
                    player.Prof);
                playerDto.condi = player.Condition;
                playerDto.conc = player.Concentration;
                playerDto.heal = player.Healing;
                playerDto.tough = player.Toughness;
                playerDto.weapons = player.GetWeaponsArray(_log);
                playerDto.colBoss = HTMLHelper.GetLink("Color-" + player.Prof);
                playerDto.colCleave = HTMLHelper.GetLink("Color-" + player.Prof + "-NonBoss");
                playerDto.colTotal = HTMLHelper.GetLink("Color-" + player.Prof + "-Total");
                data.players.Add(playerDto);
            }

            foreach(AbstractMasterPlayer enemy in _log.MechanicData.GetEnemyList(0))
            {
                data.enemies.Add(new EnemyDto(enemy.Character));
            }

            data.flags.simpleRotation = _settings.SimpleRotation;
            data.flags.dark = !_settings.LightTheme;
            data.flags.combatReplay = _settings.ParseCombatReplay && _log.FightData.Logic.CanCombatReplay;

            data.graphs.Add(new GraphDto("full", "Full"));
            data.graphs.Add(new GraphDto("s10", "10s"));
            data.graphs.Add(new GraphDto("s30", "30s"));

            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                PhaseData phaseData = _statistics.Phases[i];
                PhaseDto phaseDto = new PhaseDto(phaseData.Name, phaseData.GetDuration("s"));
                data.phases.Add(phaseDto);
                phaseDto.dpsStats = CreateDPSData(i);
                phaseDto.dmgStatsBoss = CreateDMGStatsBossData(i);
                phaseDto.dmgStats = CreateDMGStatsData(i);
                phaseDto.defStats = CreateDefData(i);
                phaseDto.healStats = CreateSupData(i);
                phaseDto.boonStats = CreateUptimeData(_statistics.PresentBoons, i);
                phaseDto.offBuffStats = CreateUptimeData(_statistics.PresentOffbuffs, i);
                phaseDto.defBuffStats = CreateUptimeData(_statistics.PresentDefbuffs, i);

                phaseDto.boonGenSelfStats = CreateGenData(_statistics.PresentBoons, i, "self");
                phaseDto.boonGenGroupStats = CreateGenData(_statistics.PresentBoons, i, "group");
                phaseDto.boonGenOGroupStats = CreateGenData(_statistics.PresentBoons, i, "off");
                phaseDto.boonGenSquadStats = CreateGenData(_statistics.PresentBoons, i, "squad");

                phaseDto.offBuffGenSelfStats = CreateGenData(_statistics.PresentOffbuffs, i, "self");
                phaseDto.offBuffGenGroupStats = CreateGenData(_statistics.PresentOffbuffs, i, "group");
                phaseDto.offBuffGenOGroupStats = CreateGenData(_statistics.PresentOffbuffs, i, "off");
                phaseDto.offBuffGenSquadStats = CreateGenData(_statistics.PresentOffbuffs, i, "squad");

                phaseDto.defBuffGenSelfStats = CreateGenData(_statistics.PresentDefbuffs, i, "self");
                phaseDto.defBuffGenGroupStats = CreateGenData(_statistics.PresentDefbuffs, i, "group");
                phaseDto.defBuffGenOGroupStats = CreateGenData(_statistics.PresentDefbuffs, i, "off");
                phaseDto.defBuffGenSquadStats = CreateGenData(_statistics.PresentDefbuffs, i, "squad");

                phaseDto.mechanicStats = CreateMechanicData(i);
                phaseDto.enemyMechanicStats = CreateBossMechanicData(i);

                phaseDto.deaths = new List<long>();

                foreach (Player player in _log.PlayerList)
                {
                    phaseDto.deaths.Add(player.GetDeath(_log, phaseData.Start, phaseData.End));
                }
            }


            data.boons = AssembleBoons(_statistics.PresentBoons);
            data.offBuffs = AssembleBoons(_statistics.PresentOffbuffs);
            data.defBuffs = AssembleBoons(_statistics.PresentDefbuffs);
            data.mechanics = CreateMechanicGraphData();

            return ToJson(data, typeof(LogDataDto));
        }

        private List<MechanicDto> AssembleMechanics(HashSet<Mechanic> mechanics)
        {
            List<MechanicDto> dtos = new List<MechanicDto>(mechanics.Count);
            foreach(Mechanic mechanic in mechanics)
            {
                MechanicDto dto = new MechanicDto();
                dto.name = mechanic.ShortName;
                dto.description = mechanic.Description;
                dto.color = mechanic.PlotlyShape;
                dtos.Add(dto);
            }
            return dtos;
        }

        private string BuildPlayerData()
        {
            Dictionary<long, SkillItem> usedSkills = new Dictionary<long, SkillItem>();
            Dictionary<long, Boon> usedBoons = new Dictionary<long, Boon>();
            String scripts = "";
            for (var i = 0; i < _log.PlayerList.Count; i++) {
                Player player = _log.PlayerList[i];
                String playerScript = "data.players[" + i + "].details = " + ToJson(BuildPlayerData(player, usedSkills, usedBoons), typeof(PlayerDetailsDto)) + ";\r\n";
                scripts += playerScript;
            }
            string skillsScript = "var usedSkills = " + ToJson(AssembleSkills(usedSkills.Values), typeof(ICollection<SkillDto>)) + ";" +
                "data.skillMap = {};" +
                "$.each(usedSkills, function(i, skill) {" +
                "data.skillMap['s'+skill.id]=skill;" +
                "});";
            string boonsScript = "var usedBoons = " + ToJson(AssembleBoons(usedBoons.Values), typeof(ICollection<BoonDto>)) + ";" +
                "data.boonMap = {};" +
                "$.each(usedBoons, function(i, boon) {" +
                "data.boonMap['b'+boon.id]=boon;" +
                "});";
            return "<script>\r\n"+ skillsScript+"\r\n"+boonsScript+"\r\n"+scripts + "\r\n</script>";
        }

        private PlayerDetailsDto BuildPlayerData(Player player, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Boon> usedBoons)
        {
            PlayerDetailsDto dto = new PlayerDetailsDto();
            dto.dmgDistributions = new List<DmgDistributionDto>();
            dto.dmgDistributionsBoss = new List<DmgDistributionDto>();
            dto.boonGraph = new List<List<BoonChartDataDto>>();
            dto.rotation = new List<List<double[]>>();
            dto.food = new List<List<FoodDto>>();
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                dto.rotation.Add(CreateSimpleRotationTabData(player, i, usedSkills));
                dto.dmgDistributions.Add(CreatePlayerDMGDistTable(player, false, i, usedSkills, usedBoons));
                dto.dmgDistributionsBoss.Add(CreatePlayerDMGDistTable(player, true, i, usedSkills, usedBoons));
                dto.boonGraph.Add(CreatePlayerBoonGraphData(player, i));
                dto.food.Add(CreatePlayerFoodData(player, i));
            }

            return dto;
        }

        private List<BoonDto> AssembleBoons(ICollection<Boon> boons)
        {
            List<BoonDto> dtos = new List<BoonDto>();
            foreach (Boon boon in boons)
            {
                dtos.Add(AssembleBoon(boon));
            }
            return dtos;
        }

        private BoonDto AssembleBoon(Boon boon)
        {
            return new BoonDto(
                    boon.ID,
                    boon.Name,
                    boon.Link,
                    boon.Type == Boon.BoonType.Intensity);
        }

        private List<SkillDto> AssembleSkills(ICollection<SkillItem> skills)
        {
            List<SkillDto> dtos = new List<SkillDto>();
            foreach (SkillItem skill in skills)
            {
                GW2APISkill apiSkill = skill.ApiSkill;
                SkillDto dto = new SkillDto(skill.ID, skill.Name, apiSkill?.icon, apiSkill?.slot == "Weapon_1");
                if (skill.ID == SkillItem.WeaponSwapId) dto.icon = "https://wiki.guildwars2.com/images/c/ce/Weapon_Swap_Button.png";
                else if (skill.ID == SkillItem.ResurrectId) dto.icon = "https://wiki.guildwars2.com/images/3/3d/Downed_ally.png";
                else if (skill.ID == SkillItem.BandageId) dto.icon = "https://wiki.guildwars2.com/images/0/0c/Bandage.png";
                else if (skill.ID == SkillItem.DodgeId) dto.icon = "https://wiki.guildwars2.com/images/b/b2/Dodge.png";
                dtos.Add(dto);
            }
            return dtos;
        }
 
        private string ToJson(Object value, Type type)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(type);
            MemoryStream memoryStream = new MemoryStream();
            ser.WriteObject(memoryStream, value);
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        private string escapeJsrender(string template)
        {
            // escape single quotation marks
            String escaped = template.Replace(@"\", @"\\");
            escaped = template.Replace("'", @"\'");
            // remove line breaks
            escaped = Regex.Replace(escaped, @"\s*\r?\n\s*", "");
            return escaped;
        }

        private string buildTemplateJS(string name, string code)
        {
            return "\r\nvar "+ name + " = $.templates('"+name+"', '"+escapeJsrender(code)+"');";
        }

        private string buildJavascript()
        {
            string javascript = Properties.Resources.flomix_ei_js;
            javascript+= buildTemplateJS("tmplTabs", Properties.Resources.tmplTabs);
            javascript += buildTemplateJS("tmplPlayerCells", Properties.Resources.tmplPlayerCells);
            javascript += buildTemplateJS("tmplDpsTable", Properties.Resources.tmplDpsTable);
            javascript += buildTemplateJS("tmplBoonTable", Properties.Resources.tmplBoonTable);
            javascript += buildTemplateJS("tmplSupTable", Properties.Resources.tmplSupTable);
            javascript += buildTemplateJS("tmplDefTable", Properties.Resources.tmplDefTable);

            javascript += buildTemplateJS("tmplDmgTable", Properties.Resources.tmplDmgTable);
            javascript += buildTemplateJS("tmplDmgDistTable", Properties.Resources.tmplDmgDistTable);
            javascript += buildTemplateJS("tmplMechanicTable", Properties.Resources.tmplMechanicTable);
            javascript += buildTemplateJS("tmplCompTable", Properties.Resources.tmplCompTable);

            return javascript;
        }
    }
}
