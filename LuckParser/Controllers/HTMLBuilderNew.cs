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
        private readonly SettingsContainer _settings;

        private readonly ParsedLog _log;

        private readonly Statistics _statistics;

        public static void UpdateStatisticSwitches(StatisticsCalculator.Switches switches)
        {
            switches.CalculateBoons = true;
            switches.CalculateDPS = true;
            switches.CalculateConditions = true;
            switches.CalculateDefense = true;
            switches.CalculateStats = true;
            switches.CalculateSupport = true;
            switches.CalculateCombatReplay = true;
            switches.CalculateMechanics = true;
        }

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

            foreach (Player p in _log.GetPlayerList())
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
                /*
                //Adding dps axis
                if (_settings.DPSGraphTotals)
                {//Turns display on or off
                    sw.Write("{");
                    HTMLHelper.WriteDPSPlots(sw, GraphHelper.GetTotalDPSGraph(_log, p, phaseIndex, phase, mode));
                    sw.Write("mode: 'lines'," +
                            "line: {shape: 'spline',color:'" + HTMLHelper.GetLink("Color-" + p.GetProf() + "-Total") + "'}," +
                            "visible:'legendonly'," +
                            "name: '" + p.GetCharacter() + " TDPS'" + "},");
                }
                sw.Write("{");
                maxDPS = Math.Max(maxDPS, HTMLHelper.WriteDPSPlots(sw, bossPoints, totalDpsAllPlayers));
                sw.Write("mode: 'lines'," +
                        "line: {shape: 'spline',color:'" + HTMLHelper.GetLink("Color-" + p.GetProf()) + "'}," +
                        "name: '" + p.GetCharacter() + " DPS'" +
                        "},");
                if (_settings.ClDPSGraphTotals)
                {//Turns display on or off
                    sw.Write("{");
                    HTMLHelper.WriteDPSPlots(sw, GraphHelper.GetCleaveDPSGraph(_log, p, phaseIndex, phase, mode));
                    sw.Write("mode: 'lines'," +
                            "line: {shape: 'spline',color:'" + HTMLHelper.GetLink("Color-" + p.GetProf() + "-NonBoss") + "'}," +
                            "visible:'legendonly'," +
                            "name: '" + p.GetCharacter() + " CleaveDPS'" + "},");
                }
                */
            }
                /*
                sw.Write("{");
                HTMLHelper.WriteDPSPlots(sw, totalDpsAllPlayers);
                sw.Write(" mode: 'lines'," +
                        "line: {shape: 'spline'}," +
                        "visible:'legendonly'," +
                        "name: 'All Player Dps'");
                sw.Write("},");
                HashSet<Mechanic> presMech = _log.GetMechanicData().GetPresentMechanics(phaseIndex);
                List<ushort> playersIds = _log.GetPlayerList().Select(x => x.GetInstid()).ToList();
                foreach (Mechanic mech in presMech)
                {
                    List<MechanicLog> filterdList = _log.GetMechanicData()[mech].Where(x => phase.InInterval(x.GetTime())).ToList();
                    sw.Write("{");
                    sw.Write("y: [");

                    int mechcount = 0;
                    foreach (MechanicLog ml in filterdList)
                    {                     
                        Point check;
                        if (playersIds.Contains(ml.GetPlayer().GetInstid()))
                        {
                            double time = (ml.GetTime() - phase.GetStart()) / 1000.0;
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
                            check = _log.GetBossData().GetHealthOverTime().FirstOrDefault(x => x.X > ml.GetTime());
                            if (check == Point.Empty)
                            {
                                check = _log.GetBossData().GetHealthOverTime().Count == 0 ? new Point(0, 10000) : new Point(0, _log.GetBossData().GetHealthOverTime().Last().Y);
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
                            sw.Write("'" + Math.Round((ml.GetTime() - phase.GetStart()) / 1000.0,4) + "'");
                        }
                        else
                        {
                            sw.Write("'" + Math.Round((ml.GetTime() - phase.GetStart()) / 1000.0,4) + "',");
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
                            sw.Write("'" + ml.GetPlayer().GetCharacter().Replace("'"," ") + "'");
                        }
                        else
                        {
                            sw.Write("'" + ml.GetPlayer().GetCharacter().Replace("'", " ") + "',");
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
                    HTMLHelper.WriteBossHealthGraph(sw, maxDPS, phase.GetStart(), phase.GetEnd(), _log.GetBossData());
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
            foreach (Player play in _log.GetPlayerList())
            {
                int playerGroup = play.GetGroup();
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
                    List<Player> sortedList = _log.GetPlayerList().Where(x => x.GetGroup() == n).ToList();
                    if (sortedList.Count > 0)
                    {
                        foreach (Player gPlay in sortedList)
                        {
                            string charName = gPlay.GetCharacter().Length > 10 ? gPlay.GetCharacter().Substring(0, 10) : gPlay.GetCharacter();
                            //Getting Build
                            string build = "";
                            if (gPlay.GetCondition() > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/5/54/Condition_Damage.png\" alt=\"Condition Damage\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Condition Damage-" + gPlay.GetCondition() + "\">";//"<span class=\"badge badge-warning\">Condi("+ gPlay.getCondition() + ")</span>";
                            }
                            if (gPlay.GetConcentration() > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/4/44/Boon_Duration.png\" alt =\"Concentration\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Concentration-" + gPlay.GetConcentration() + "\">";//"<span class=\"badge badge-warning\">Condi("+ gPlay.getCondition() + ")</span>";
                            }
                            if (gPlay.GetHealing() > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/8/81/Healing_Power.png\" alt=\"Healing Power\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Healing Power-" + gPlay.GetHealing() + "\">";//"<span class=\"badge badge-success\">Heal("+ gPlay.getHealing() + ")</span>";
                            }
                            if (gPlay.GetToughness() > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/1/12/Toughness.png\" alt=\"Toughness\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Toughness-" + gPlay.GetToughness() + "\">";//"<span class=\"badge badge-secondary\">Tough("+ gPlay.getToughness() + ")</span>";
                            }
                            sw.Write("<td class=\"composition\">");
                            {
                                sw.Write("<img src=\"" + HTMLHelper.GetLink(gPlay.GetProf()) + "\" alt=\"" + gPlay.GetProf() + "\" height=\"18\" width=\"18\" >");
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
            List<List<Object>> list = new List<List<Object>>(_log.GetPlayerList().Count);
            PhaseData phase = _statistics.Phases[phaseIndex];
            List<string[]> footerList = new List<string[]>();

            foreach (Player player in _log.GetPlayerList())
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

            foreach (Player player in _log.GetPlayerList())
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
                playerData.Add(stats.Interupts); //18
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

            foreach (Player player in _log.GetPlayerList())
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
                playerData.Add(stats.InteruptsBoss); //18
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

            foreach (Player player in _log.GetPlayerList())
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

            foreach (Player player in _log.GetPlayerList())
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
            bool boonTable = listToUse.Select(x => x.GetID()).Contains(740);
                
            foreach (Player player in _log.GetPlayerList())
            {
                BoonData boonData = new BoonData();

                Dictionary<long, Statistics.FinalBoonUptime> boons = _statistics.SelfBoons[player][phaseIndex];
                Dictionary<long, Dictionary<int, string[]>> extraBoonData = player.GetExtraBoonData(_log, phases);
                List<string> boonArrayToList = new List<string>
                {
                    player.GetGroup().ToString()
                };
                long fightDuration = phases[phaseIndex].GetDuration();
                Dictionary<long, long> boonPresence = player.GetBoonPresence(_log, phases, phaseIndex);
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

                    boonVals.Add(boons[boon.GetID()].Uptime);

                    if (boon.GetBoonType() == Boon.BoonType.Intensity)
                    {
                        intensityBoon.Add(count);
                    }
                    string tooltip = "";
                    if (extraBoonData.TryGetValue(boon.GetID(), out var myDict))
                    {
                        string[] tooltips = myDict[phaseIndex];
                        tooltip = "<big><b>Boss</b></big><br>" + tooltips[1] + "<br><big><b>All</b></big><br>" + tooltips[0];
                        boonVals.Add(0);
                        boonVals.Add(tooltip);
                    }
                    else
                    {
                        if (boonTable && boon.GetBoonType() == Boon.BoonType.Intensity && boonPresence.TryGetValue(boon.GetID(), out long presenceValue))
                        {
                            boonVals.Add(Math.Round(100.0 * presenceValue / fightDuration, 1));
                        }
                    }                                
                    boonArrayToList.Add(boons[boon.GetID()].Uptime.ToString());                        
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

            foreach (Player player in _log.GetPlayerList())
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
                    Statistics.FinalBoonUptime uptime = uptimes[boon.GetID()];
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
        /// Creates the player tab
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private void CreatePlayerTab(StreamWriter sw, int phaseIndex)
        {
            List<PhaseData> phases = _statistics.Phases;
            PhaseData phase = phases[phaseIndex];
            //generate Player list Graphs
            foreach (Player p in _log.GetPlayerList())
            {
                List<CastLog> casting = p.GetCastLogsActDur(_log, phase.GetStart(), phase.GetEnd());

                bool died = p.GetDeath(_log, phase.GetStart(), phase.GetEnd()) > 0;
                string charname = p.GetCharacter();
                string pid = p.GetInstid() + "_" + phaseIndex;
                sw.Write("<div class=\"tab-pane fade\" id=\"" + pid + "\">");
                {
                    sw.Write("<h1 align=\"center\"> " + charname + "<img src=\"" + HTMLHelper.GetLink(p.GetProf()) + "\" alt=\"" + p.GetProf() + "\" height=\"18\" width=\"18\" >" + "</h1>");
                    sw.Write("<ul class=\"nav nav-tabs\">");
                    {
                        sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#home" + pid + "\">" + p.GetCharacter() + "</a></li>");
                        if (_settings.SimpleRotation)
                        {
                            sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#SimpleRot" + pid + "\">Simple Rotation</a></li>");

                        }
                        if (died)
                        {
                            sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#DeathRecap" + pid + "\">Death Recap</a></li>");

                        }
                        //foreach pet loop here                        
                        foreach (KeyValuePair<string, Minions> pair in p.GetMinions(_log))
                        {
                            sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#minion" + pid + "_" + pair.Value.GetInstid() + "\">" + pair.Key + "</a></li>");
                        }
                        //inc dmg
                        sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#incDmg" + pid + "\">Damage Taken</a></li>");
                    }
                    sw.Write("</ul>");
                    sw.Write("<div id=\"myTabContent\" class=\"tab-content\">");
                    {
                        sw.Write("<div class=\"tab-pane fade show active\" id=\"home" + pid + "\">");
                        {
                            List<Tuple<Boon,long>> consume = p.GetConsumablesList(_log, phase.GetStart(), phase.GetEnd());
                            List<Tuple<Boon, long>> initial = consume.Where(x => x.Item2 == 0).ToList();
                            List<Tuple<Boon, long>> refreshed = consume.Where(x => x.Item2 > 0).ToList();
                            if (initial.Count > 0)
                            {
                                Boon food = null;
                                Boon utility = null;
                                foreach (Tuple<Boon, long> buff in initial)
                                {
                                    if (buff.Item1.GetNature() == Boon.BoonEnum.Food)
                                    {
                                        food = buff.Item1;
                                    } else
                                    {
                                        utility = buff.Item1;
                                    }
                                }
                                sw.Write("<p>Started with ");
                                if (food != null)
                                {
                                    sw.Write(food.GetName() + "<img src=\"" + food.GetLink() + "\" alt=\"" + food.GetName() + "\" height=\"18\" width=\"18\" >");
                                }
                                if (utility != null)
                                {
                                    sw.Write((food != null ?" and " : "") + utility.GetName() + "<img src=\"" + utility.GetLink() + "\" alt=\"" + utility.GetName() + "\" height=\"18\" width=\"18\" >");
                                }
                                sw.Write("</p>");
                            }
                            if (refreshed.Count > 0)
                            {
                                sw.Write("<p>Refreshed: ");
                                sw.Write("<ul>");
                                foreach (Tuple<Boon, long> buff in refreshed)
                                {
                                    sw.Write("<li>" + buff.Item1.GetName() + "<img src=\"" + buff.Item1.GetLink() + "\" alt=\"" + buff.Item1.GetName() + "\" height=\"18\" width=\"18\" > at "+ Math.Round(buff.Item2 / 1000.0,3)+"s</li>");
                                }
                                sw.Write("</ul>");
                                sw.Write("</p>");
                            }
                            sw.Write("<div id=\"Graph" + pid + "\" style=\"height: 1000px;width:1000px; display:inline-block \"></div>");
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
                                                HTMLHelper.WriteCastingItem(sw, cl, _log.GetSkillData(), phase.GetStart(), phase.GetEnd());
                                            }
                                        }
                                        if (_statistics.PresentBoons.Count > 0)
                                        {
                                            Dictionary<long, BoonsGraphModel> boonGraphData = p.GetBoonGraphs(_log, phases);
                                            foreach (BoonsGraphModel bgm in boonGraphData.Values.Reverse().Where(x => x.GetBoonName() != "Number of Conditions"))
                                            {
                                                sw.Write("{");
                                                {
                                                    HTMLHelper.WritePlayerTabBoonGraph(sw, bgm, phase.GetStart(), phase.GetEnd());
                                                }
                                                sw.Write(" },");

                                            }
                                            boonGraphData = _log.GetBoss().GetBoonGraphs(_log, phases);
                                            foreach (BoonsGraphModel bgm in boonGraphData.Values.Reverse().Where(x => x.GetBoonName() == "Compromised" || x.GetBoonName() == "Unnatural Signet"))
                                            {
                                                sw.Write("{");
                                                {
                                                    HTMLHelper.WritePlayerTabBoonGraph(sw, bgm, phase.GetStart(), phase.GetEnd());
                                                }
                                                sw.Write(" },");

                                            }
                                        }
                                        if (_settings.DPSGraphTotals)
                                        {//show total dps plot
                                            sw.Write("{");
                                            { //Adding dps axis
                                                HTMLHelper.WritePlayerTabDPSGraph(sw, "Total DPS", GraphHelper.GetTotalDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.Full), p);
                                            }
                                            sw.Write("},");
                                            if (_settings.Show10s)
                                            {
                                                sw.Write("{");
                                                HTMLHelper.WritePlayerTabDPSGraph(sw, "Total DPS - 10s", GraphHelper.GetTotalDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.S10), p);
                                                sw.Write("},");
                                            }
                                            if (_settings.Show30s)
                                            {
                                                sw.Write("{");
                                                HTMLHelper.WritePlayerTabDPSGraph(sw, "Total DPS - 30s", GraphHelper.GetTotalDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.S30), p);
                                                sw.Write("},");
                                            }
                                        }
                                         //Adding dps axis
                                            sw.Write("{");
                                            {
                                                HTMLHelper.WritePlayerTabDPSGraph(sw, "Boss DPS", GraphHelper.GetBossDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.Full), p);
                                            }
                                            sw.Write("},");
                                            if (_settings.Show10s)
                                            {
                                                sw.Write("{");
                                                HTMLHelper.WritePlayerTabDPSGraph(sw, "Boss DPS - 10s", GraphHelper.GetBossDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.S10), p);
                                                sw.Write("},");
                                            }
                                            if (_settings.Show30s)
                                            {
                                                sw.Write("{");
                                                HTMLHelper.WritePlayerTabDPSGraph(sw, "Boss DPS - 30s", GraphHelper.GetBossDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.S30), p);
                                                sw.Write("},");
                                            }

                                        //Adding dps axis
                                        if (_settings.ClDPSGraphTotals)
                                        {//show total dps plot
                                            sw.Write("{");
                                            { //Adding dps axis
                                                HTMLHelper.WritePlayerTabDPSGraph(sw, "Cleave DPS", GraphHelper.GetCleaveDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.Full), p);
                                            }
                                            sw.Write("},");
                                            if (_settings.Show10s)
                                            {
                                                sw.Write("{");
                                                HTMLHelper.WritePlayerTabDPSGraph(sw, "Cleave DPS - 10s", GraphHelper.GetCleaveDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.S10), p);
                                                sw.Write("},");
                                            }
                                            if (_settings.Show30s)
                                            {
                                                sw.Write("{");
                                                HTMLHelper.WritePlayerTabDPSGraph(sw, "Cleave DPS - 30s", GraphHelper.GetCleaveDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.S30), p);
                                                sw.Write("},");
                                            }
                                        }

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
                                                 "yaxis2: { title: 'Boons', domain: [0.11, 0.50], fixedrange: true }," +
                                                 "yaxis3: { title: 'DPS', domain: [0.51, 1] },"
                                         );
                                        sw.Write("images: [");
                                        {
                                            if (_settings.PlayerRot && _settings.PlayerRotIcons)//Display rotation
                                            {
                                                int castCount = 0;
                                                foreach (CastLog cl in casting)
                                                {
                                                    HTMLHelper.WriteCastingItemIcon(sw, cl, _log.GetSkillData(), phase.GetStart(), castCount == casting.Count - 1);
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
                            sw.Write("<ul class=\"nav nav-tabs\">");
                            {
                                string bossText = phase.GetRedirection().Count > 0 ? "Adds" : "Boss";
                                sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#distTabBoss" + pid + "\">" + bossText + "</a></li>");
                                sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#distTabAll" + pid + "\">" + "All" + "</a></li>");
                            }
                            sw.Write("</ul>");
                            sw.Write("<div class=\"tab-content\">");
                            {
                                sw.Write("<div class=\"tab-pane fade show active\" id=\"distTabBoss" + pid + "\">");
                                {
                                    //CreatePlayerDMGDistTable(sw, p, true, phaseIndex);
                                }
                                sw.Write("</div>");
                                sw.Write("<div class=\"tab-pane fade \" id=\"distTabAll" + pid + "\">");
                                {
                                    //CreatePlayerDMGDistTable(sw, p, false, phaseIndex);
                                }
                                sw.Write("</div>");
                            }
                            sw.Write("</div>");
                        }
                        sw.Write("</div>");
                        foreach (KeyValuePair<string, Minions> pair in p.GetMinions(_log))
                        {
                            string id = pid + "_" + pair.Value.GetInstid();
                            sw.Write("<div class=\"tab-pane fade \" id=\"minion" + id + "\">");
                            {
                                string bossText = phase.GetRedirection().Count > 0 ? "Adds" : "Boss";
                                sw.Write("<ul class=\"nav nav-tabs\">");
                                {
                                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#distTabBoss" + id + "\">" + bossText + "</a></li>");
                                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#distTabAll" + id + "\">" + "All" + "</a></li>");
                                }
                                sw.Write("</ul>");
                                sw.Write("<div class=\"tab-content\">");
                                {
                                    sw.Write("<div class=\"tab-pane fade show active\" id=\"distTabBoss" + id + "\">");
                                    {
                                        CreatePlayerMinionDMGDistTable(sw, p, pair.Value, true, phaseIndex);
                                    }
                                    sw.Write("</div>");
                                    sw.Write("<div class=\"tab-pane fade\" id=\"distTabAll" + id + "\">");
                                    {
                                        CreatePlayerMinionDMGDistTable(sw, p, pair.Value, false, phaseIndex);
                                    }
                                    sw.Write("</div>");
                                }
                                sw.Write("</div>");
                            }
                            sw.Write("</div>");
                        }
                        if (_settings.SimpleRotation)
                        {
                            sw.Write("<div class=\"tab-pane fade \" id=\"SimpleRot" + pid + "\">");
                            {
                                int simpleRotSize = 20;
                                if (_settings.LargeRotIcons)
                                {
                                    simpleRotSize = 30;
                                }
                                //CreateSimpleRotationTab(sw, p, simpleRotSize, phaseIndex);
                            }
                            sw.Write("</div>");
                        }
                        if (died && phaseIndex == 0)
                        {
                            sw.Write("<div class=\"tab-pane fade \" id=\"DeathRecap" + pid + "\">");
                            {
                                CreateDeathRecap(sw, p);
                            }
                            sw.Write("</div>");
                        }
                        sw.Write("<div class=\"tab-pane fade \" id=\"incDmg" + pid + "\">");
                        {
                            CreatePlayerDMGTakenDistTable(sw, p, phaseIndex);
                        }
                        sw.Write("</div>");
                    }
                    sw.Write("</div>");
                }
                sw.Write("</div>");
            }

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
            List<CastLog> casting = p.GetCastLogs(_log, phase.GetStart(), phase.GetEnd());
            SkillData skillList = _log.GetSkillData();
            foreach (CastLog cl in casting)
            {
                if (!usedSkills.ContainsKey(cl.GetID())) usedSkills.Add(cl.GetID(), skillList.GetOrDummy(cl.GetID()));
                double[] rotEntry = new double[5];
                list.Add(rotEntry);
                rotEntry[0] = cl.GetTime()/1000.0;
                rotEntry[1] = cl.GetID();
                rotEntry[2] = cl.GetActDur();
                rotEntry[3] = encodeEndActivation(cl.EndActivation());
                rotEntry[4] = cl.StartActivation() == ParseEnum.Activation.Quickness ? 1 : 0;
            }
            return list;
        }

        private int encodeEndActivation(ParseEnum.Activation endActivation)
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
            List<DamageLog> damageLogs = p.GetDamageTakenLogs(_log, 0, _log.GetBossData().GetAwareDuration());
            long start = _log.GetBossData().GetFirstAware();
            long end = _log.GetBossData().GetLastAware();
            List<CombatItem> down = _log.GetCombatData().GetStates(p.GetInstid(), ParseEnum.StateChange.ChangeDown, start, end);
            if (down.Count > 0)
            {
                List<CombatItem> ups = _log.GetCombatData().GetStates(p.GetInstid(), ParseEnum.StateChange.ChangeUp, start, end);
                // Surely a consumable in fractals
                down = ups.Count > down.Count ? new List<CombatItem>() : down.GetRange(ups.Count, down.Count - ups.Count);
            }
            List<CombatItem> dead = _log.GetCombatData().GetStates(p.GetInstid(), ParseEnum.StateChange.ChangeDead, start, end);
            List<DamageLog> damageToDown = new List<DamageLog>();
            List<DamageLog> damageToKill = new List<DamageLog>();
            if (down.Count > 0)
            {//went to down state before death
                damageToDown = damageLogs.Where(x => x.GetTime() < down.Last().Time - start && x.GetDamage() > 0).ToList();
                damageToKill = damageLogs.Where(x => x.GetTime() > down.Last().Time - start && x.GetTime() < dead.Last().Time - start && x.GetDamage() > 0).ToList();
                //Filter last 30k dmg taken
                int totaldmg = 0;
                for (int i = damageToDown.Count - 1; i > 0; i--)
                {
                    totaldmg += damageToDown[i].GetDamage();
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
                sw.Write("<p>Took " + damageToDown.Sum(x => x.GetDamage()) + " damage in " +
                ((damageToDown.Last().GetTime() - damageToDown.First().GetTime()) / 1000f).ToString() + " seconds to enter downstate");
                if (damageToKill.Count > 0)
                {
                    sw.Write("<p>Took " + damageToKill.Sum(x => x.GetDamage()) + " damage in " +
                       ((damageToKill.Last().GetTime() - damageToKill.First().GetTime()) / 1000f).ToString() + " seconds to die</p>");
                }
                else
                {
                    sw.Write("<p>Instant death after a down</p>");
                }
                sw.Write("</center>");
            }
            else
            {
                damageToKill = damageLogs.Where(x => x.GetTime() < dead.Last().Time && x.GetDamage() > 0).ToList();
                //Filter last 30k dmg taken
                int totaldmg = 0;
                for (int i = damageToKill.Count - 1; i > 0; i--)
                {
                    totaldmg += damageToKill[i].GetDamage();
                    if (totaldmg > 30000)
                    {
                        damageToKill = damageToKill.GetRange(i, damageToKill.Count - 1 - i);
                        break;
                    }
                }
                sw.Write("<center><h3>Player was insta killed by a mechanic, fall damage or by /gg</h3></center>");
            }
            string pid = p.GetInstid().ToString();
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
                        sw.Write("'" + dl.GetTime() / 1000f + "s',");
                    }
                }
                for (int d = 0; d < damageToKill.Count; d++)
                {
                    sw.Write("'" + damageToKill[d].GetTime() / 1000f + "s'");

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
                        sw.Write("'" + dl.GetDamage() + "',");
                    }
                }
                for (int d = 0; d < damageToKill.Count; d++)
                {
                    sw.Write("'" + damageToKill[d].GetDamage() + "'");

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
                        AgentItem ag = _log.GetAgentData().GetAgentWInst(dl.GetSrcInstidt());
                        string name = "UNKNOWN";
                        if (ag != null)
                        {
                            name = ag.GetName().Replace("\0", "").Replace("\'", "\\'");
                        }
                        string skillname = _log.GetSkillData().GetName(dl.GetID()).Replace("\'", "\\'");
                        sw.Write("'" + name + "<br>" + skillname + " hit you for " + dl.GetDamage() + "',");
                    }
                }
                for (int d = 0; d < damageToKill.Count; d++)
                {
                    AgentItem ag = _log.GetAgentData().GetAgentWInst(damageToKill[d].GetSrcInstidt());
                    string name = "UNKNOWN";
                    if (ag != null )
                    {
                        name = ag.GetName().Replace("\0", "").Replace("\'", "\\'");
                    }
                    string skillname = _log.GetSkillData().GetName(damageToKill[d].GetID()).Replace("\'", "\\'");
                    sw.Write("'" + name + "<br>" +
                           "hit you with <b>" + skillname + "</b> for " + damageToKill[d].GetDamage() + "'");

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
            Dictionary<long, List<CastLog>> castLogsBySkill = casting.GroupBy(x => x.GetID()).ToDictionary(x => x.Key, x => x.ToList());
            Dictionary<long, List<DamageLog>> damageLogsBySkill = damageLogs.GroupBy(x => x.GetID()).ToDictionary(x => x.Key, x => x.ToList());
            Dictionary<long, Boon> conditionsById = _statistics.PresentConditions.ToDictionary(x => x.GetID());
            SkillData skillList = _log.GetSkillData();
            foreach (KeyValuePair<long,List<DamageLog>> entry in damageLogsBySkill)
            {
                int totaldamage = 0,mindamage = 0,maxdamage = 0,casts = 0,hits = 0,crit = 0,flank = 0,glance = 0,timeswasted = 0,timessaved = 0;
                foreach (DamageLog dl in entry.Value)
                {
                    int curdmg = dl.GetDamage();
                    totaldamage += curdmg;
                    if (0 == mindamage || curdmg < mindamage) { mindamage = curdmg; }
                    if (0 == maxdamage || curdmg > maxdamage) { maxdamage = curdmg; }
                    hits++;
                    if (dl.GetResult() == ParseEnum.Result.Crit) crit++;
                    if (dl.GetResult() == ParseEnum.Result.Glance) glance++;
                    if (dl.IsFlanking() == 1) flank++;
                }

                bool isCondi = conditionsById.ContainsKey(entry.Key);
                if (isCondi)
                {
                    Boon condi = conditionsById[entry.Key];
                    if (!usedBoons.ContainsKey(condi.GetID())) usedBoons.Add(condi.GetID(), condi);
                } else
                {
                    if (!usedSkills.ContainsKey(entry.Key)) usedSkills.Add(entry.Key, skillList.GetOrDummy(entry.Key));
                }

                if (!isCondi && castLogsBySkill.TryGetValue(entry.Key, out List<CastLog> clList))
                {

                    casts = clList.Count;
                    foreach (CastLog cl in clList)
                    {
                        if (cl.EndActivation() == ParseEnum.Activation.CancelCancel) timeswasted += cl.GetActDur();
                        if (cl.EndActivation() == ParseEnum.Activation.CancelFire && cl.GetActDur() < cl.GetExpDur())
                        {
                            timessaved += cl.GetExpDur() - cl.GetActDur();
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
            List<CastLog> casting = p.GetCastLogs(_log, phase.GetStart(), phase.GetEnd());
            List<DamageLog> damageLogs;
            if (toBoss && phase.GetRedirection().Count > 0)
            {
                damageLogs = p.GetJustPlayerDamageLogs(phase.GetRedirection(), _log, phase.GetStart(), phase.GetEnd());
            }
            else
            {
                damageLogs = p.GetJustPlayerDamageLogs(toBoss ? _log.GetBossData().GetInstid() : 0, _log, phase.GetStart(), phase.GetEnd());
            }
            int totalDamage = toBoss ? dps.BossDamage : dps.AllDamage;
            dto.totalDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.GetDamage()) : 0;
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
            string tabid = p.GetInstid() + "_" + phaseIndex + "_" + minions.GetInstid() + (toBoss ? "_boss" : "");
            PhaseData phase = _statistics.Phases[phaseIndex];
            List<CastLog> casting = minions.GetCastLogs(_log, phase.GetStart(), phase.GetEnd());
            List<DamageLog> damageLogs;
            if (toBoss && phase.GetRedirection().Count > 0)
            {
                damageLogs = minions.GetDamageLogs(phase.GetRedirection(), _log, phase.GetStart(), phase.GetEnd());
            }
            else
            {
                damageLogs = minions.GetDamageLogs(toBoss ? _log.GetBossData().GetInstid() : 0, _log, phase.GetStart(), phase.GetEnd());
            }
            int finalTotalDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.GetDamage()) : 0;
            if (totalDamage > 0)
            {
                string contribution = Math.Round(100.0 * finalTotalDamage / totalDamage,2).ToString();
                sw.Write("<div>" + minions.GetCharacter() + " did " + contribution + "% of " + p.GetCharacter() + "'s total " + (toBoss ? (phase.GetRedirection().Count > 0 ? "adds " : "boss " ) : "") + "dps</div>");
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
            List<DamageLog> damageLogs = p.GetDamageTakenLogs(_log, phase.GetStart(), phase.GetEnd());
            SkillData skillList = _log.GetSkillData();
            long finalTotalDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => (long)x.GetDamage()) : 0;
            string pid = p.GetInstid() + "_" + phaseIndex;
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
                        long condiID = condi.GetID();
                        int totaldamage = 0;
                        int mindamage = 0;
                        int hits = 0;
                        int maxdamage = 0;
                        usedIDs.Add(condiID);
                        foreach (DamageLog dl in damageLogs.Where(x => x.GetID() == condiID))
                        {
                            int curdmg = dl.GetDamage();
                            totaldamage += curdmg;
                            if (0 == mindamage || curdmg < mindamage) { mindamage = curdmg; }
                            if (0 == maxdamage || curdmg > maxdamage) { maxdamage = curdmg; }
                            hits++;

                        }
                        int avgdamage = (int)(totaldamage / (double)hits);
                        if (totaldamage > 0)
                        {
                            string condiName = condi.GetName();// Boon.getCondiName(condiID);
                            sw.Write("<tr>");
                            {
                                sw.Write("<td align=\"left\"><img src=\"" + condi.GetLink() + "\" alt=\"" + condiName + "\" title=\"" + condiID + "\" height=\"18\" width=\"18\">" + condiName + "</td>");
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
                    foreach (int id in damageLogs.Where(x => !usedIDs.Contains(x.GetID())).Select(x => (int)x.GetID()).Distinct())
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
                            foreach (DamageLog dl in damageLogs.Where(x => x.GetID() == id))
                            {
                                int curdmg = dl.GetDamage();
                                totaldamage += curdmg;
                                if (0 == mindamage || curdmg < mindamage) { mindamage = curdmg; }
                                if (0 == maxdamage || curdmg > maxdamage) { maxdamage = curdmg; }
                                if (curdmg >= 0) { hits++; };
                                ParseEnum.Result result = dl.GetResult();
                                if (result == ParseEnum.Result.Crit) { crit++; } else if (result == ParseEnum.Result.Glance) { glance++; }
                                if (dl.IsFlanking() == 1) { flank++; }
                            }
                            int avgdamage = (int)(totaldamage / (double)hits);

                            if (skill.GetGW2APISkill() != null)
                            {
                                sw.Write("<tr>");
                                {
                                    sw.Write("<td align=\"left\"><img src=\"" + skill.GetGW2APISkill().icon + "\" alt=\"" + skill.GetName() + "\" title=\"" + skill.GetID() + "\" height=\"18\" width=\"18\">" + skill.GetName() + "</td>");
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
                                    sw.Write("<td align=\"left\">" + skill.GetName() + "</td>");
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
        /// <summary>
        /// Creates the mechanics table of the fight
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private List<List<int[]>> CreateMechanicData(int phaseIndex)
        {
            List<List<int[]>> list = new List<List<int[]>>();
            HashSet<Mechanic> presMech = _log.GetMechanicData().GetPresentPlayerMechs();
            PhaseData phase = _statistics.Phases[phaseIndex];

            foreach (Player p in _log.GetPlayerList())
            {
                List<int[]> playerData = new List<int[]>(presMech.Count);
                foreach (Mechanic mech in presMech)
                {
                    long timeFilter = 0;
                    int filterCount = 0;
                    List<MechanicLog> mls = _log.GetMechanicData()[mech].Where(x => x.GetPlayer().GetInstid() == p.GetInstid() && phase.InInterval(x.GetTime())).ToList();
                    int count = mls.Count;
                    foreach (MechanicLog ml in mls)
                    {
                        if (mech.GetICD() != 0 && ml.GetTime() - timeFilter < mech.GetICD())//ICD check
                        {
                            filterCount++;
                        }
                        timeFilter = ml.GetTime();

                    }
                    int[] mechEntry = {count - filterCount,count};
                    playerData.Add(mechEntry);
                }
                list.Add(playerData);
            }
            return list;
        }

        private List<List<List<int>>> CreateEnemyMechanicTable(int phaseIndex)
        {
            List<List<List<int>>> list = new List<List<List<int>>>();
            HashSet<Mechanic> presEnemyMech = _log.GetMechanicData().GetPresentEnemyMechs(phaseIndex);
            PhaseData phase = _statistics.Phases[phaseIndex];
            List<AbstractMasterPlayer> enemyList = _log.GetMechanicData().GetEnemyList(phaseIndex);

            foreach (AbstractMasterPlayer p in enemyList)
            {
                List<List<int>> enemyData = new List<List<int>>();
                foreach (Mechanic mech in presEnemyMech)
                {
                    int count = _log.GetMechanicData()[mech].Count(x => x.GetPlayer().GetInstid() == p.GetInstid() && phase.InInterval(x.GetTime()));
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
                foreach (CombatItem c in _log.GetCombatData())
                {
                    if (c.IsStateChange != ParseEnum.StateChange.Normal)
                    {
                        AgentItem agent = _log.GetAgentData().GetAgent(c.SrcAgent);
                        if (agent != null)
                        {
                            switch (c.IsStateChange)
                            {
                                case ParseEnum.StateChange.EnterCombat:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.GetName() + " entered combat in" + c.DstAgent + "subgroup" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.ExitCombat:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.GetName() + " exited combat" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.ChangeUp:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.GetName() + " is now alive" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.ChangeDead:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.GetName() + " is now dead" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.ChangeDown:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.GetName() + " is now downed" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.Spawn:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.GetName() + " is now in logging range of POV player" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.Despawn:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.GetName() + " is now out of range of logging player" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.HealthUpdate:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.GetName() + " is at " + c.DstAgent / 100 + "% health" +
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
                                                   agent.GetName() + " weapon swapped to " + c.DstAgent + "(0/1 water, 4/5 land)" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.MaxHealthUpdate:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.GetName() + " max health changed to  " + c.DstAgent +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.PointOfView:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.GetName() + " is recording log " +
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
                foreach (SkillItem skill in _log.GetSkillData().Values)
                {
                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                  skill.GetID() + " : " + skill.GetName() +
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
                if (boon.GetName() == "Retaliation")
                {
                    continue;
                }
                if (conditions[boon.GetID()].Uptime > 0.0)
                {
                    hasBoons = true;
                    break;
                }
            }
            Dictionary<long, long> condiPresence = boss.GetCondiPresence(_log, phases, phaseIndex);
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
                            if (hasBoons && boon.GetName() == "Retaliation")
                            {
                                continue;
                            }
                            sw.Write("<th>" + "<img src=\"" + boon.GetLink() + " \" alt=\"" + boon.GetName() + "\" title =\" " + boon.GetName() + "\" height=\"18\" width=\"18\" >" + "</th>");
                        }
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</thead>");
                sw.Write("<tbody>");
                {
                    sw.Write("<tr>");
                    {
                        
                        sw.Write("<td style=\"width: 275px;\" data-toggle=\"tooltip\" title=\"Average number of conditions: " + Math.Round(avgCondis, 1) + "\">" + boss.GetCharacter() + " </td>");
                        foreach (Boon boon in _statistics.PresentConditions)
                        {
                            if (hasBoons && boon.GetName() == "Retaliation")
                            {
                                continue;
                            }
                            if (boon.GetBoonType() == Boon.BoonType.Duration)
                            {
                                sw.Write("<td>" + conditions[boon.GetID()].Uptime + "%</td>");
                            }
                            else
                            {
                                if (condiPresence.TryGetValue(boon.GetID(), out long presenceTime))
                                {
                                    string tooltip = "uptime: " + Math.Round(100.0 * presenceTime / fightDuration, 1) + "%";
                                    sw.Write("<td data-toggle=\"tooltip\" title=\"" + tooltip + "\">" + conditions[boon.GetID()].Uptime + " </td>");
                                }
                                else
                                {
                                   sw.Write("<td>" + conditions[boon.GetID()].Uptime + "</td>");
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
                Dictionary<long, long> boonPresence = boss.GetBoonPresence(_log, phases, phaseIndex);
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
                                sw.Write("<th>" + "<img src=\"" + boon.GetLink() + " \" alt=\"" + boon.GetName() + "\" title =\" " + boon.GetName() + "\" height=\"18\" width=\"18\" >" + "</th>");
                            }
                        }
                        sw.Write("</tr>");
                    }
                    sw.Write("</thead>");
                    sw.Write("<tbody>");
                    {
                        sw.Write("<tr>");
                        {
                            sw.Write("<td style=\"width: 275px;\">" + boss.GetCharacter() + " </td>");
                            foreach (Boon boon in _statistics.PresentBoons)
                            {
                                if (boon.GetBoonType() == Boon.BoonType.Duration)
                                {
                                    sw.Write("<td>" + conditions[boon.GetID()].Uptime + "%</td>");
                                }
                                else
                                {
                                    if (boonPresence.TryGetValue(boon.GetID(), out long presenceTime))
                                    {
                                        string tooltip = "uptime: " + Math.Round(100.0 * presenceTime / fightDuration, 1) + "%";
                                        sw.Write("<td data-toggle=\"tooltip\" title=\"" + tooltip + "\">" + conditions[boon.GetID()].Uptime + " </td>");
                                    }
                                    else
                                    {
                                        sw.Write("<td>" + conditions[boon.GetID()].Uptime + "</td>");
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
                            if (boon.GetName() == "Retaliation")
                            {
                                continue;
                            }
                            sw.Write("<th>" + "<img src=\"" + boon.GetLink() + " \" alt=\"" + boon.GetName() + "\" title =\" " + boon.GetName() + "\" height=\"18\" width=\"18\" >" + "</th>");
                        }
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</thead>");
                sw.Write("<tbody>");
                {
                    foreach (Player player in _log.GetPlayerList())
                    {
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.GetGroup().ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + HTMLHelper.GetLink(player.GetProf()) + "\" alt=\"" + player.GetProf() + "\" height=\"18\" width=\"18\" >" + "<span style=\"display:none\">" + player.GetProf() + "</span>" + "</td>");
                            sw.Write("<td>" + player.GetCharacter() + " </td>");
                            foreach (Boon boon in _statistics.PresentConditions)
                            {
                                if (boon.GetName() == "Retaliation")
                                {
                                    continue;
                                }
                                Statistics.FinalBossBoon toUse = conditions[boon.GetID()];
                                if (boon.GetBoonType() == Boon.BoonType.Duration)
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
            List<CastLog> casting = _log.GetBoss().GetCastLogsActDur(_log, phase.GetStart(), phase.GetEnd());
            string charname = _log.GetBoss().GetCharacter();
            string pid = _log.GetBoss().GetInstid() + "_" + phaseIndex;
            sw.Write("<h1 align=\"center\"> " + charname + "</h1>");
            sw.Write("<ul class=\"nav nav-tabs\">");
            {
                sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#home" + pid + "\">" + _log.GetBoss().GetCharacter() + "</a></li>");
                //foreach pet loop here
                foreach (KeyValuePair<string, Minions> pair in _log.GetBoss().GetMinions(_log))
                {
                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#minion" + pid + "_" + pair.Value.GetInstid() + "\">" + pair.Key + "</a></li>");
                }
            }
            sw.Write("</ul>");
            //condi stats tab
            sw.Write("<div id=\"myTabContent\" class=\"tab-content\"><div class=\"tab-pane fade show active\" id=\"home" + pid + "\">");
            {
                CreateCondiUptimeTable(sw, _log.GetBoss(), phaseIndex);
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
                                    HTMLHelper.WriteCastingItem(sw, cl, _log.GetSkillData(), phase.GetStart(), phase.GetEnd());
                                }
                            }
                            //============================================
                            Dictionary<long, BoonsGraphModel> boonGraphData = _log.GetBoss().GetBoonGraphs(_log, phases);
                            foreach (BoonsGraphModel bgm in boonGraphData.Values.Reverse().Where(x => x.GetBoonName() != "Number of Boons"))
                            {
                                sw.Write("{");
                                {
                                    HTMLHelper.WritePlayerTabBoonGraph(sw, bgm, phase.GetStart(), phase.GetEnd());
                                }
                                sw.Write(" },");

                            }
                            //int maxDPS = 0;
                            if (_settings.DPSGraphTotals)
                            {//show total dps plot
                                List<Point> playertotaldpsgraphdata = GraphHelper.GetTotalDPSGraph(_log, _log.GetBoss(), phaseIndex, phase, GraphHelper.GraphMode.Full);
                                sw.Write("{");
                                {
                                    //Adding dps axis
                                    HTMLHelper.WritePlayerTabDPSGraph(sw, "Total DPS", playertotaldpsgraphdata, _log.GetBoss());
                                }
                                sw.Write("},");
                            }
                            sw.Write("{");
                            HTMLHelper.WriteBossHealthGraph(sw, GraphHelper.GetTotalDPSGraph(_log, _log.GetBoss(), phaseIndex, phase, GraphHelper.GraphMode.Full).Max(x => x.Y), phase.GetStart(), phase.GetEnd(), _log.GetBossData(), "y3");
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
                                        HTMLHelper.WriteCastingItemIcon(sw, cl, _log.GetSkillData(), phase.GetStart(), castCount == casting.Count - 1);
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
                //CreateBossDMGDistTable(_log.GetBoss(), phaseIndex);
                sw.Write("</div>");
                foreach (KeyValuePair<string, Minions> pair in _log.GetBoss().GetMinions(_log))
                {
                    sw.Write("<div class=\"tab-pane fade \" id=\"minion" + pid + "_" + pair.Value.GetInstid() + "\">");
                    {
                        CreateBossMinionDMGDistTable(sw, _log.GetBoss(), pair.Value, phaseIndex);
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
            CombatReplayMap map = _log.GetBoss().GetCombatMap(_log);
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
                if (_log.GetBoss().GetCombatReplay() != null)
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

        /// <summary>
        /// Creates the whole html
        /// </summary>
        /// <param name="sw">Stream writer</param>
        public void CreateHTML(StreamWriter sw, String path)
        {
            string scriptFile = Path.Combine(path, "flomix-ei.js");
            using (var fs = new FileStream(scriptFile, FileMode.Create, FileAccess.Write))
            using (var scriptWriter = new StreamWriter(fs))
            {
                scriptWriter.Write(buildJavascript());
            }

            string cssFile = Path.Combine(path, "flomix-ei.css");
            using (var fs = new FileStream(cssFile, FileMode.Create, FileAccess.Write))
            using (var scriptWriter = new StreamWriter(fs))
            {
                scriptWriter.Write(Properties.Resources.flomix_ei_css);
            }

            string html = Properties.Resources.template_html;
            html = html.Replace("${bootstrapTheme}", !_settings.LightTheme ? "slate" : "cosmo");
            html = html.Replace("${logDataJson}", BuildLogData());

            html = html.Replace("<!--${playerData}-->", BuildPlayerData());

            html = html.Replace("${graphDataJson}", BuildGraphJson());

            sw.Write(html);
            return;



            double fightDuration = (_log.GetBossData().GetAwareDuration()) / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fightDuration);
            string durationString = duration.Minutes + "m " + duration.Seconds + "s " + duration.Milliseconds + "ms";
            if (duration.Hours > 0)
            {
                durationString = duration.Hours + "h " + durationString;
            }
            string bossname = FilterStringChars(_log.GetBossData().GetName());
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
                        sw.Write("<p> Time Start: " + _log.GetLogData().GetLogStart() + " | Time End: " + _log.GetLogData().GetLogEnd() + " </p> ");
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
                                                    sw.Write("<img src=\"" + HTMLHelper.GetLink(_log.GetBossData().GetID() + "-icon") + "\"alt=\"" + bossname + "-icon" + "\" style=\"height: 120px; width: 120px;\" >");
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div>");
                                                {
                                                    sw.Write("<div class=\"progress\" style=\"width: 100 %; height: 20px;\">");
                                                    {
                                                        if (_log.GetLogData().GetBosskill())
                                                        {
                                                            string tp = _log.GetBossData().GetHealth().ToString() + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-success\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:100%; ;\" aria-valuenow=\"100\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");
                                                        }
                                                        else
                                                        {
                                                            double finalPercent = 0;
                                                            if (_log.GetBossData().GetHealthOverTime().Count > 0)
                                                            {
                                                                finalPercent = 100.0 - _log.GetBossData().GetHealthOverTime()[_log.GetBossData().GetHealthOverTime().Count - 1].Y * 0.01;
                                                            }
                                                            string tp = Math.Round(_log.GetBossData().GetHealth() * finalPercent / 100.0) + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-success\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:" + finalPercent + "%;\" aria-valuenow=\"" + finalPercent + "\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");
                                                            tp = Math.Round(_log.GetBossData().GetHealth() * (100.0 - finalPercent) / 100.0) + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-danger\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:" + (100.0 - finalPercent) + "%;\" aria-valuenow=\"" + (100.0 - finalPercent) + "\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");

                                                        }
                                                    }
                                                    sw.Write("</div>");
                                                    sw.Write("<p class=\"small\" style=\"text-align:center; color: "+ (_settings.LightTheme ? "#000" : "#FFF") +";\">" + _log.GetBossData().GetHealth().ToString() + " Health</p>");
                                                    sw.Write(_log.GetLogData().GetBosskill() ? "<p class='text text-success'> Result: Success</p>" : "<p class='text text-warning'> Result: Fail</p>");
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
                        if (phases.Count > 1 || _log.GetBoss().GetCombatReplay() != null)
                        {
                            sw.Write("<ul class=\"nav nav-tabs\">");
                            {
                                for (int i = 0; i < phases.Count; i++)
                                {
                                    if (phases[i].GetDuration() == 0)
                                        continue;
                                    string active = (i > 0 ? "" : "active");
                                    string name = phases[i].GetName();
                                    sw.Write("<li  class=\"nav-item\">" +
                                            "<a class=\"nav-link " + active + "\" data-toggle=\"tab\" href=\"#phase" + i + "\">" +
                                                "<span data-toggle=\"tooltip\" title=\"" + phases[i].GetDuration("s") + " seconds\">" + name + "</span>" +
                                            "</a>" +
                                        "</li>");
                                }
                                if (_log.GetBoss().GetCombatReplay() != null)
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
                                        sw.Write("<h2 align=\"center\">"+ phases[i].GetName()+ "</h2>");
                                    string playerDropdown = "";
                                    foreach (Player p in _log.GetPlayerList())
                                    {
                                        string charname = p.GetCharacter();
                                        playerDropdown += "<a class=\"dropdown-item\"  data-toggle=\"tab\" href=\"#" + p.GetInstid() + "_" + i + "\">" + charname +
                                            "<img src=\"" + HTMLHelper.GetLink(p.GetProf()) + "\" alt=\"" + p.GetProf() + "\" height=\"18\" width=\"18\" >" + "</a>";
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
                                                    string bossText = phases[i].GetRedirection().Count > 0 ? "Adds" : "Boss";
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
                            if (_log.GetBoss().GetCombatReplay() != null)
                            {
                                sw.Write("<div class=\"tab-pane fade\" id=\"replay\">");
                                {
                                    CreateReplayTable(sw);
                                }
                            }
                        }
                        sw.Write("</div>");
                        sw.Write("<p style=\"margin-top:10px;\"> ARC:" + _log.GetLogData().GetBuildVersion() + " | Bossid " + _log.GetBossData().GetID().ToString() + "| EI Version: " +Application.ProductVersion + " </p> ");
                       
                        sw.Write("<p style=\"margin-top:-15px;\">File recorded by: " + _log.GetLogData().GetPOV().Split(':')[0] + "</p>");
                    }
                    sw.Write("</div>");
                }
                sw.Write("</body>");
                sw.Write("<script> $(document).ready(function(){$('[data-toggle=\"tooltip\"]').tooltip(); });</script >");
            }
            //end
            sw.Write("</html>");
        }

        private string BuildGraphJson()
        {
            List<List<PlayerChartDataDto>> chartData = new List<List<PlayerChartDataDto>>();
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                chartData.Add(CreateDPSGraphData(i));
             }
            return ToJson(chartData, typeof(List<List<PlayerChartDataDto>>));
        }

        private string BuildLogData()
        {
            LogDataDto data = new LogDataDto();
            foreach(Player player in _log.GetPlayerList())
            {
                data.players.Add(new PlayerDto(
                    player.GetGroup(),
                    player.GetCharacter(),
                    player.GetAccount().TrimStart(':'),
                    player.GetProf()));
            }

            data.simpleRotation = _settings.SimpleRotation;

            data.graphs.Add(new GraphDto("full", "Full"));
            data.graphs.Add(new GraphDto("s10", "10s"));
            data.graphs.Add(new GraphDto("s30", "30s"));

            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                PhaseData phaseData = _statistics.Phases[i];
                PhaseDto phaseDto = new PhaseDto(phaseData.GetName(), phaseData.GetDuration("s"));
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

                phaseDto.deaths = new List<long>();

                foreach (Player player in _log.GetPlayerList())
                {
                    phaseDto.deaths.Add(player.GetDeath(_log, phaseData.GetStart(), phaseData.GetEnd()));
                }
            }

            data.boons = AssembleBoons(_statistics.PresentBoons);
            data.offBuffs = AssembleBoons(_statistics.PresentOffbuffs);
            data.defBuffs = AssembleBoons(_statistics.PresentDefbuffs);
            data.mechanics = AssembleMechanics(_log.GetMechanicData().GetPresentPlayerMechs());

            return ToJson(data, typeof(LogDataDto));
        }

        private List<MechanicDto> AssembleMechanics(HashSet<Mechanic> mechanics)
        {
            List<MechanicDto> dtos = new List<MechanicDto>(mechanics.Count);
            foreach(Mechanic mechanic in mechanics)
            {
                MechanicDto dto = new MechanicDto();
                dto.name = mechanic.GetShortName();
                dto.description = mechanic.GetDescription();
                dto.color = mechanic.GetPlotly();
                dtos.Add(dto);
            }
            return dtos;
        }

        private string BuildPlayerData()
        {
            Dictionary<long, SkillItem> usedSkills = new Dictionary<long, SkillItem>();
            Dictionary<long, Boon> usedBoons = new Dictionary<long, Boon>();
            String scripts = "";
            for (var i = 0; i < _log.GetPlayerList().Count; i++) {
                Player player = _log.GetPlayerList()[i];
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
            dto.rotation = new List<List<double[]>>();
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                dto.rotation.Add(CreateSimpleRotationTabData(player, i, usedSkills));
                dto.dmgDistributions.Add(CreatePlayerDMGDistTable(player, false, i, usedSkills, usedBoons));
                dto.dmgDistributionsBoss.Add(CreatePlayerDMGDistTable(player, true, i, usedSkills, usedBoons));
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
                    boon.GetID(),
                    boon.GetName(),
                    boon.GetLink(),
                    boon.GetBoonType() == Boon.BoonType.Intensity);
        }

        private List<SkillDto> AssembleSkills(ICollection<SkillItem> skills)
        {
            List<SkillDto> dtos = new List<SkillDto>();
            foreach (SkillItem skill in skills)
            {
                GW2APISkill apiSkill = skill.GetGW2APISkill();
                SkillDto dto = new SkillDto(skill.GetID(), skill.GetName(), apiSkill?.icon);
                if (skill.GetID() == SkillItem.WeaponSwapId) dto.icon = "https://wiki.guildwars2.com/images/c/ce/Weapon_Swap_Button.png";
                else if (skill.GetID() == SkillItem.ResurrectId) dto.icon = "https://wiki.guildwars2.com/images/3/3d/Downed_ally.png";
                else if (skill.GetID() == SkillItem.BandageId) dto.icon = "https://wiki.guildwars2.com/images/0/0c/Bandage.png";
                else if (skill.GetID() == SkillItem.DodgeId) dto.icon = "https://wiki.guildwars2.com/images/b/b2/Dodge.png";

                dto.aa = apiSkill?.slot == "Weapon_1";
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

            return javascript;
        }
    }
}
