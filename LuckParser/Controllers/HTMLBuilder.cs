using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LuckParser.Models;
using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;

namespace LuckParser.Controllers
{
    class HTMLBuilder
    {
        private SettingsContainer settings;

        private ParsedLog log;

        private Statistics statistics;

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

        public HTMLBuilder(ParsedLog log, SettingsContainer settings, Statistics statistics)
        {
            this.log = log;
            
            this.settings = settings;
            HTMLHelper.settings = settings;
            GraphHelper.settings = settings;

            this.statistics = statistics;
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
        /// <param name="sw">Stream writer</param>
        private void CreateDPSGraph(StreamWriter sw, int phase_index, GraphHelper.GraphMode mode)
        {
            //Generate DPS graph
            string plotID = "DPSGraph" + phase_index + "_" + mode;
            sw.Write("<div id=\"" + plotID + "\" style=\"height: 1000px;width:1200px; display:inline-block \"></div>");
            sw.Write("<script>");
            PhaseData phase = log.getBoss().getPhases(log, settings.ParsePhases)[phase_index];
            sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
            {
                sw.Write("var data = [");
                int maxDPS = 0;
                List<Point> totalDpsAllPlayers = new List<Point>();
                foreach (Player p in log.getPlayerList())
                {
                    //Adding dps axis

                    int pbdgdCount = 0;
                    if (settings.DPSGraphTotals)
                    {//Turns display on or off
                        sw.Write("{");
                        //Adding dps axis
                        List<Point> playertotaldpsgraphdata = GraphHelper.getTotalDPSGraph(log, p, phase_index, mode);
                        sw.Write("y: [");
                        pbdgdCount = 0;
                        foreach (Point dp in playertotaldpsgraphdata)
                        {
                            if (pbdgdCount == playertotaldpsgraphdata.Count - 1)
                            {
                                sw.Write("'" + dp.Y + "'");
                            }
                            else
                            {
                                sw.Write("'" + dp.Y + "',");
                            }
                            pbdgdCount++;

                        }
                        //cuts off extra comma
                        if (playertotaldpsgraphdata.Count == 0)
                        {
                            sw.Write("'0'");
                        }

                        sw.Write("],");
                        //add time axis
                        sw.Write("x: [");
                        pbdgdCount = 0;
                        foreach (Point dp in playertotaldpsgraphdata)
                        {
                            if (pbdgdCount == playertotaldpsgraphdata.Count - 1)
                            {
                                sw.Write("'" + dp.X + "'");
                            }
                            else
                            {
                                sw.Write("'" + dp.X + "',");
                            }

                            pbdgdCount++;
                        }
                        if (playertotaldpsgraphdata.Count == 0)
                        {
                            sw.Write("'0'");
                        }

                        sw.Write("],");
                        sw.Write("mode: 'lines'," +
                                "line: {shape: 'spline',color:'" + HTMLHelper.GetLink("Color-" + p.getProf() + "-Total") + "'}," +
                                "visible:'legendonly'," +
                                "name: '" + p.getCharacter() + " TDPS'" + "},");
                    }
                    List<Point> playerbossdpsgraphdata = GraphHelper.getBossDPSGraph(log, p, phase_index, mode);
                    if (totalDpsAllPlayers.Count == 0)
                    {
                        //totalDpsAllPlayers = new List<int[]>(playerbossdpsgraphdata);
                        foreach (Point point in playerbossdpsgraphdata)
                        {
                            int time = point.X;
                            int dmg = point.Y;
                            totalDpsAllPlayers.Add(new Point(time, dmg));
                        }
                    }

                    sw.Write("{y: [");
                    pbdgdCount = 0;
                    foreach (Point dp in playerbossdpsgraphdata)
                    {
                        if (pbdgdCount == playerbossdpsgraphdata.Count - 1)
                        {
                            sw.Write("'" + dp.Y + "'");
                        }
                        else
                        {
                            sw.Write("'" + dp.Y + "',");
                        }
                        pbdgdCount++;

                        if (dp.Y > maxDPS) { maxDPS = dp.Y; }
                        if (totalDpsAllPlayers.Count != 0)
                        {
                            totalDpsAllPlayers[dp.X] = new Point(dp.X, totalDpsAllPlayers[dp.X].Y + dp.Y);
                        }
                    }
                    if (playerbossdpsgraphdata.Count == 0)
                    {
                        sw.Write("'0'");
                    }

                    sw.Write("],");
                    //add time axis
                    sw.Write("x: [");
                    pbdgdCount = 0;
                    foreach (Point dp in playerbossdpsgraphdata)
                    {
                        if (pbdgdCount == playerbossdpsgraphdata.Count - 1)
                        {
                            sw.Write("'" + dp.X + "'");
                        }
                        else
                        {
                            sw.Write("'" + dp.X + "',");
                        }
                        pbdgdCount++;
                    }
                    if (playerbossdpsgraphdata.Count == 0)
                    {
                        sw.Write("'0'");
                    }

                    sw.Write("],");
                    sw.Write("mode: 'lines'," +
                            "line: {shape: 'spline',color:'" + HTMLHelper.GetLink("Color-" + p.getProf()) + "'}," +
                            "name: '" + p.getCharacter() + " DPS'" +
                            "},");
                    if (settings.ClDPSGraphTotals)
                    {//Turns display on or off
                        sw.Write("{");
                        //Adding dps axis
                        List<Point> playercleavedpsgraphdata = GraphHelper.getCleaveDPSGraph(log, p, phase_index, mode);
                        sw.Write("y: [");
                        pbdgdCount = 0;
                        foreach (Point dp in playercleavedpsgraphdata)
                        {
                            if (pbdgdCount == playercleavedpsgraphdata.Count - 1)
                            {
                                sw.Write("'" + dp.Y + "'");
                            }
                            else
                            {
                                sw.Write("'" + dp.Y+ "',");
                            }
                            pbdgdCount++;

                        }
                        //cuts off extra comma
                        if (playercleavedpsgraphdata.Count == 0)
                        {
                            sw.Write("'0'");
                        }

                        sw.Write("],");
                        //add time axis
                        sw.Write("x: [");
                        pbdgdCount = 0;
                        foreach (Point dp in playercleavedpsgraphdata)
                        {
                            if (pbdgdCount == playercleavedpsgraphdata.Count - 1)
                            {
                                sw.Write("'" + dp.X + "'");
                            }
                            else
                            {
                                sw.Write("'" + dp.X + "',");
                            }

                            pbdgdCount++;
                        }
                        if (playercleavedpsgraphdata.Count == 0)
                        {
                            sw.Write("'0'");
                        }

                        sw.Write("],");
                        sw.Write("mode: 'lines'," +
                                "line: {shape: 'spline',color:'" + HTMLHelper.GetLink("Color-" + p.getProf() + "-NonBoss") + "'}," +
                                "visible:'legendonly'," +
                                "name: '" + p.getCharacter() + " CleaveDPS'" + "},");
                    }
                }
                //All Player dps
                sw.Write("{");
                //Adding dps axis

                sw.Write("y: [");
                int tdalpcount = 0;
                foreach (Point dp in totalDpsAllPlayers)
                {
                    if (tdalpcount == totalDpsAllPlayers.Count - 1)
                    {
                        sw.Write("'" + dp.Y + "'");
                    }
                    else
                    {
                        sw.Write("'" + dp.Y + "',");
                    }
                    tdalpcount++;
                }

                sw.Write("],");
                //add time axis
                sw.Write("x: [");
                tdalpcount = 0;
                foreach (Point dp in totalDpsAllPlayers)
                {
                    if (tdalpcount == totalDpsAllPlayers.Count - 1)
                    {
                        sw.Write("'" + dp.X + "'");
                    }
                    else
                    {
                        sw.Write("'" + dp.X + "',");
                    }

                    tdalpcount++;
                }

                sw.Write("],");
                sw.Write(" mode: 'lines'," +
                        "line: {shape: 'spline'}," +
                        "visible:'legendonly'," +
                        "name: 'All Player Dps'");
                sw.Write("},");
                List<Mechanic> presMech = log.getMechanicData().GetMechList(log.getBossData().getID());
                List<string> distMech = presMech.Select(x => x.GetAltName()).Distinct().ToList();
                foreach (string mechAltString in distMech)
                {
                    List<Mechanic> mechs = presMech.Where(x => x.GetAltName() == mechAltString).ToList();
                    List<MechanicLog> filterdList = new List<MechanicLog>();
                    foreach (Mechanic me in mechs)
                    {
                        filterdList.AddRange(log.getMechanicData().GetMDataLogs().Where(x => x.GetSkill() == me.GetSkill() && phase.inInterval(1000 * x.GetTime())).ToList());
                    }
                    Mechanic mech = mechs[0];
                    //List<MechanicLog> filterdList = mech_data.GetMDataLogs().Where(x => x.GetName() == mech.GetName()).ToList();
                    sw.Write("{");
                    sw.Write("y: [");

                    int mechcount = 0;
                    foreach (MechanicLog ml in filterdList)
                    {
                        Point check = new Point();
                        if (ml.GetPlayer() != log.getBoss())
                        {
                            check = GraphHelper.getBossDPSGraph(log, ml.GetPlayer(), phase_index, mode).FirstOrDefault(x => x.X == ml.GetTime() - Math.Round(phase.getStart() / 1000.0));
                            if (check == Point.Empty)
                            {
                                check = new Point(0, GraphHelper.getBossDPSGraph(log, ml.GetPlayer(), phase_index, mode).Last().Y);
                            }
                        }
                        else
                        {
                            check = log.getBossData().getHealthOverTime().FirstOrDefault(x => x.X/1000f > ml.GetTime()); // boss_data.getHealthOverTime().Where(x => x.X >= start && x.X <= end).ToList();
                            if (check == Point.Empty)
                            {
                                if (log.getBossData().getHealthOverTime().Count == 0)
                                {
                                    check = new Point(0, 10000);
                                } else
                                {
                                    check = new Point(0, log.getBossData().getHealthOverTime().Last().Y);
                                }
                            }
                            check.Y = (int)((float)(check.Y / 10000f) * maxDPS);
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
                    tdalpcount = 0;
                    mechcount = 0;
                    foreach (MechanicLog ml in filterdList)
                    {
                        if (mechcount == filterdList.Count - 1)
                        {
                            sw.Write("'" + (ml.GetTime() - Math.Round(phase.getStart() / 1000.0)) + "'");
                        }
                        else
                        {
                            sw.Write("'" + (ml.GetTime() - Math.Round(phase.getStart() / 1000.0)) + "',");
                        }

                        mechcount++;
                    }

                    sw.Write("],");
                    sw.Write(" mode: 'markers',");
                    if (mech.GetName() != "DEAD" && mech.GetName() != "DOWN")
                    {
                        sw.Write("visible:'legendonly',");
                    }
                    sw.Write("type:'scatter'," +
                            "marker:{" + mech.GetPlotly() + "size: 15" + "}," +
                            "text:[");
                    foreach (MechanicLog ml in filterdList)
                    {
                        if (mechcount == filterdList.Count - 1)
                        {
                            sw.Write("'" + ml.GetPlayer().getCharacter() + "'");
                        }
                        else
                        {
                            sw.Write("'" + ml.GetPlayer().getCharacter() + "',");
                        }

                        mechcount++;
                    }

                    sw.Write("]," +
                            " name: '" + mech.GetAltName() + "'");
                    sw.Write("},");
                }
                //Downs and deaths

                List<String> DnDStringList = new List<string>();
                DnDStringList.Add("DOWN");
                DnDStringList.Add("DEAD");
                foreach (string state in DnDStringList)
                {
                    int mcount = 0;
                    List<MechanicLog> DnDList = log.getMechanicData().GetMDataLogs().Where(x => x.GetName() == state && phase.inInterval(1000 * x.GetTime())).ToList();
                    sw.Write("{");
                    {
                        sw.Write("y: [");
                        {
                            foreach (MechanicLog ml in DnDList)
                            {
                                Point check = GraphHelper.getBossDPSGraph(log, ml.GetPlayer(), phase_index, mode).FirstOrDefault(x => x.X == ml.GetTime() - Math.Round(phase.getStart() / 1000.0));
                                if (mcount == DnDList.Count - 1)
                                {
                                    if (check != null)
                                    {
                                        sw.Write("'" + check.Y + "'");
                                    }
                                    else
                                    {
                                        sw.Write("'" + GraphHelper.getBossDPSGraph(log, ml.GetPlayer(), phase_index, mode).Last().Y + "'");
                                    }

                                }
                                else
                                {
                                    if (check != null)
                                    {
                                        sw.Write("'" + check.Y + "',");
                                    }
                                    else
                                    {
                                        sw.Write("'" + GraphHelper.getBossDPSGraph(log, ml.GetPlayer(), phase_index, mode).Last().Y + "',");
                                    }
                                }

                                mcount++;
                            }
                        }

                        sw.Write("],");
                        //add time axis
                        sw.Write("x: [");
                        {
                            tdalpcount = 0;
                            mcount = 0;
                            foreach (MechanicLog ml in DnDList)
                            {
                                if (mcount == DnDList.Count - 1)
                                {
                                    sw.Write("'" + (ml.GetTime() - Math.Round(phase.getStart() / 1000.0)) + "'");
                                }
                                else
                                {
                                    sw.Write("'" + (ml.GetTime() - Math.Round(phase.getStart() / 1000.0)) + "',");
                                }

                                mcount++;
                            }
                        }

                        sw.Write("],");
                        sw.Write(" mode: 'markers',");
                        if (state != "DEAD" && state != "DOWN")
                        {
                            sw.Write("visible:'legendonly',");
                        }
                        sw.Write("type:'scatter'," +
                            "marker:{" + log.getMechanicData().GetPLoltyShape(state) + "size: 15" + "},");
                        sw.Write("text:[");
                        foreach (MechanicLog ml in DnDList)
                        {
                            if (mcount == DnDList.Count - 1)
                            {
                                sw.Write("'" + ml.GetPlayer().getCharacter() + "'");
                            }
                            else
                            {
                                sw.Write("'" + ml.GetPlayer().getCharacter() + "',");
                            }

                            mcount++;
                        }
                        sw.Write("]," +
                                " name: '" + state + "'");
                    }
                    sw.Write("},");
                }
                if (maxDPS > 0)
                {
                    sw.Write("{");
                    HTMLHelper.writeBossHealthGraph(sw, maxDPS, phase.getStart(), phase.getEnd(), log.getBossData());
                    sw.Write("}");
                }
                else
                {
                    sw.Write("{}");
                }
                if (settings.LightTheme)
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
        }
        private void GetRoles()
        {
            //tags: tank,healer,dps(power/condi)
            //Roles:greenteam,green split,cacnoneers,flakkiter,eater,KCpusher,agony,epi,handkiter,golemkiter,orbs
        }
        private void PrintWeapons(StreamWriter sw, Player p)
        {
            //print weapon sets
            string[] wep = p.getWeaponsArray(log);
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
            foreach (Player play in log.getPlayerList())
            {
                int playerGroup = play.getGroup();
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
                    List<Player> sortedList = log.getPlayerList().Where(x => x.getGroup() == n).ToList();
                    if (sortedList.Count > 0)
                    {
                        foreach (Player gPlay in sortedList)
                        {
                            string charName = "";
                            if (gPlay.getCharacter().Length > 10)
                            {
                                charName = gPlay.getCharacter().Substring(0, 10);
                            }
                            else
                            {
                                charName = gPlay.getCharacter().ToString();
                            }
                            //Getting Build
                            string build = "";
                            if (gPlay.getCondition() > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/5/54/Condition_Damage.png\" alt=\"Condition Damage\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Condition Damage-" + gPlay.getCondition() + "\">";//"<span class=\"badge badge-warning\">Condi("+ gPlay.getCondition() + ")</span>";
                            }
                            if (gPlay.getConcentration() > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/4/44/Boon_Duration.png\" alt =\"Concentration\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Concentration-" + gPlay.getConcentration() + "\">";//"<span class=\"badge badge-warning\">Condi("+ gPlay.getCondition() + ")</span>";
                            }
                            if (gPlay.getHealing() > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/8/81/Healing_Power.png\" alt=\"Healing Power\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Healing Power-" + gPlay.getHealing() + "\">";//"<span class=\"badge badge-success\">Heal("+ gPlay.getHealing() + ")</span>";
                            }
                            if (gPlay.getToughness() > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/1/12/Toughness.png\" alt=\"Toughness\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Toughness-" + gPlay.getToughness() + "\">";//"<span class=\"badge badge-secondary\">Tough("+ gPlay.getToughness() + ")</span>";
                            }
                            sw.Write("<td class=\"composition\">");
                            {
                                sw.Write("<img src=\"" + HTMLHelper.GetLink(gPlay.getProf()) + "\" alt=\"" + gPlay.getProf().ToString() + "\" height=\"18\" width=\"18\" >");
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
        /// <param name="sw">Stream writer</param>
        /// <param name="fight_duration">Duration of the fight</param>
        private void CreateDPSTable(StreamWriter sw, int phase_index)
        {
            //generate dps table
            PhaseData phase = log.getBoss().getPhases(log, settings.ParsePhases)[phase_index];
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#dps_table" + phase_index + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#dps_table" + phase_index + "').DataTable({ 'order': [[4, 'desc']]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#dps_table" + phase_index + "').DataTable({ 'order': [[4, 'desc']]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dps_table" + phase_index + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Sub</th>");
                        sw.Write("<th></th>");
                        sw.Write("<th>Name</th>");
                        sw.Write("<th>Account</th>");
                        sw.Write("<th>Boss DPS</th>");
                        sw.Write("<th>Power</th>");
                        sw.Write("<th>Condi</th>");
                        sw.Write("<th>All DPS</th>");
                        sw.Write("<th>Power</th>");
                        sw.Write("<th>Condi</th>");
                        sw.Write("<th><img src=\"" + HTMLHelper.GetLink("Downs") + "\" alt=\"Downs\" title=\"Times downed\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=\"" + HTMLHelper.GetLink("Dead") + "\" alt=\"Dead\" title=\"Time died\" height=\"18\" width=\"18\"></th>");
                    }
                    sw.Write("</tr>");
                }

                sw.Write("</thead>");
                List<string[]> footerList = new List<string[]>();
                sw.Write("<tbody>");
                foreach (Player player in log.getPlayerList())
                {
                    Statistics.FinalDPS dps = statistics.dps[player][phase_index];
                    Statistics.FinalStats stats = statistics.stats[player][phase_index];
                    //gather data for footer
                    footerList.Add(new string[]
                    {
                        player.getGroup().ToString(),
                        dps.allDps.ToString(), dps.allDamage.ToString(),
                        dps.allPowerDps.ToString(), dps.allPowerDamage.ToString(),
                        dps.allCondiDps.ToString(), dps.allCondiDamage.ToString(),
                        dps.bossDps.ToString(), dps.bossDamage.ToString(),
                        dps.bossPowerDps.ToString(), dps.bossPowerDamage.ToString(),
                        dps.bossCondiDps.ToString(), dps.bossCondiDamage.ToString()
                    });
                    sw.Write("<tr>");
                    {
                        sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                        sw.Write("<td>" + "<img src=\"" + HTMLHelper.GetLink(player.getProf()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >"+"<span style=\"display:none\">"+ player.getProf() + "</span>"+"</td>");
                        sw.Write("<td>" + player.getCharacter().ToString() + "</td>");
                        sw.Write("<td>" + player.getAccount().TrimStart(':') + "</td>");
                        //Boss dps
                        sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dps.bossDamage + " dmg \">" + dps.bossDps + "</span>" + "</td>");
                        sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dps.bossPowerDamage + " dmg \">" + dps.bossPowerDps + "</span>" + "</td>");
                        sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dps.bossCondiDamage + " dmg \">" + dps.bossCondiDps + "</span>" + "</td>");
                        //All DPS
                        sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dps.allDamage + " dmg \">" + dps.allDps + "</span>" + "</td>");
                        sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dps.allPowerDamage + " dmg \">" + dps.allPowerDps + "</span>" + "</td>");
                        sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dps.allCondiDamage + " dmg \">" + dps.allCondiDps + "</span>" + "</td>");
                        sw.Write("<td>" + stats.downCount + "</td>");
                        TimeSpan timedead = TimeSpan.FromMilliseconds(stats.died);
                        long fight_duration = phase.getDuration();
                        if (timedead > TimeSpan.Zero)
                        {
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + timedead + " (" + Math.Round((timedead.TotalMilliseconds / fight_duration) * 100,1) + "% Alive) \">" + timedead.Minutes + " m " + timedead.Seconds + " s</span>" + " </td>");
                        }
                        else
                        {
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"Never died\"> 0</span>" + " </td>");
                        }
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</tbody>");
                if (log.getPlayerList().Count > 1)
                {
                    sw.Write("<tfoot>");
                    {
                        foreach (string groupNum in footerList.Select(x => x[0]).Distinct())
                        {
                            List<string[]> groupList = footerList.Where(x => x[0] == groupNum).ToList();
                            sw.Write("<tr>");
                            {
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td>Group " + groupNum + "</td>");
                                sw.Write("<td></td>");
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[8])) + " dmg \">" + groupList.Sum(c => int.Parse(c[7])) + "</span>" + "</td>");
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[10])) + " dmg \">" + groupList.Sum(c => int.Parse(c[9])) + "</span>" + "</td>");
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[12])) + " dmg \">" + groupList.Sum(c => int.Parse(c[11])) + "</span>" + "</td>");
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[2])) + " dmg \">" + groupList.Sum(c => int.Parse(c[1])) + "</span>" + "</td>");
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[4])) + " dmg \">" + groupList.Sum(c => int.Parse(c[3])) + "</span>" + "</td>");
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[6])) + " dmg \">" + groupList.Sum(c => int.Parse(c[5])) + "</span>" + "</td>");
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                            }
                            sw.Write("</tr>");
                        }
                        sw.Write("<tr>");
                        {
                            sw.Write("<td></td>");
                            sw.Write("<td></td>");
                            sw.Write("<td>Total</td>");
                            sw.Write("<td></td>");
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[8])) + " dmg \">" + footerList.Sum(c => int.Parse(c[7])) + "</span>" + "</td>");
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[10])) + " dmg \">" + footerList.Sum(c => int.Parse(c[9])) + "</span>" + "</td>");
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[12])) + " dmg \">" + footerList.Sum(c => int.Parse(c[11])) + "</span>" + "</td>");
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[2])) + " dmg \">" + footerList.Sum(c => int.Parse(c[1])) + "</span>" + "</td>");
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[4])) + " dmg \">" + footerList.Sum(c => int.Parse(c[3])) + "</span>" + "</td>");
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[6])) + " dmg \">" + footerList.Sum(c => int.Parse(c[5])) + "</span>" + "</td>");
                            sw.Write("<td></td>");
                            sw.Write("<td></td>");
                        }
                        sw.Write("</tr>");
                    }
                    sw.Write("</tfoot>");
                }
            }

            sw.Write("</table>");
        }
        /// <summary>
        /// Creates the damage stats table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="fight_duration">Duration of the fight</param>
        private void CreateDMGStatsTable(StreamWriter sw, int phase_index)
        {
            //generate dmgstats table
            PhaseData phase = log.getBoss().getPhases(log, settings.ParsePhases)[phase_index];
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#dmgstats_table" + phase_index + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#dmgstats_table" + phase_index + "').DataTable({ 'order': [[0, 'asc']]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#dmgstats_table" + phase_index + "').DataTable({ 'order': [[0, 'asc']]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dmgstats_table" + phase_index + "\">");
            {
                sw.Write("<thead>");
                {
                    HTMLHelper.writeDamageStatsTableHeader(sw);
                }
                sw.Write("</thead>");
                List<string[]> footerList = new List<string[]>();
                sw.Write("<tbody>");
                {
                    foreach (Player player in log.getPlayerList())
                    {
                        Statistics.FinalStats stats = statistics.stats[player][phase_index];
                        Statistics.FinalDPS dps = statistics.dps[player][phase_index];

                        TimeSpan timedead = TimeSpan.FromMilliseconds(stats.died);//dead 

                        //gather data for footer
                        footerList.Add(new string[] {
                            player.getGroup().ToString(),
                            stats.powerLoopCount.ToString(),
                            stats.criticalRate.ToString(),
                            stats.scholarRate.ToString(),
                            stats.movingRate.ToString(),
                            stats.flankingRate.ToString(),
                            stats.glanceRate.ToString(),
                            stats.missed.ToString(),
                            stats.interupts.ToString(),
                            stats.invulned.ToString(),
                            stats.swapCount.ToString(),
                            stats.downCount.ToString(),
                            stats.critablePowerLoopCount.ToString()
                        });
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + HTMLHelper.GetLink(player.getProf().ToString()) + "\" alt=\"" 
                                + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "<span style=\"display:none\">" + player.getProf() + "</span>" + "</td>");
                            sw.Write("<td>" + player.getCharacter().ToString() + "</td>");

                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + stats.criticalRate + " out of " + stats.critablePowerLoopCount
                                + " critable hits<br> Total Damage Effected by Crits: " + stats.criticalDmg 
                                + " \">" + Math.Round((Double)(stats.criticalRate) / stats.critablePowerLoopCount * 100,1) 
                                + "%</span>" + "</td>");//crit
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + stats.scholarRate+ " out of " + stats.powerLoopCount + " hits <br> Pure Scholar Damage: " 
                                + stats.scholarDmg + "<br> Effective Physical Damage Increase: " 
                                + Math.Round(100.0 * (dps.playerPowerDamage / (Double)(dps.playerPowerDamage - stats.scholarDmg) - 1.0) , 3) 
                                + "% \">" + Math.Round((Double)(stats.scholarRate) / stats.powerLoopCount * 100,1) + "%</span>" + "</td>");//scholar
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\""
                                + stats.movingRate + " out of " + stats.powerLoopCount + " hits <br> Pure Seaweed Damage: "
                                + stats.movingDamage + "<br> Effective Physical Damage Increase: "
                                + Math.Round(100.0 * (dps.playerPowerDamage / (Double)(dps.playerPowerDamage - stats.movingDamage) - 1.0), 3)
                                + "% \">" + Math.Round((Double)(stats.movingRate) / stats.powerLoopCount * 100, 1) + "%</span>" + "</td>");//sws
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\""
                                + stats.flankingRate + " out of " + stats.powerLoopCount + " hits \">" 
                                + Math.Round(stats.flankingRate / (Double)stats.powerLoopCount * 100,1) + "%</span>" + "</td>");//flank
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + stats.glanceRate + " out of " + stats.powerLoopCount + " hits \">" 
                                + Math.Round(stats.glanceRate / (Double)stats.powerLoopCount * 100,1) + "%</span>" + "</td>");//glance
                            sw.Write("<td>" + stats.missed + "</td>");//misses
                            sw.Write("<td>" + stats.interupts + "</td>");//interupts
                            sw.Write("<td>" + stats.invulned + "</td>");//dmg invulned
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + stats.wasted + "cancels \">" + stats.timeWasted + "</span>" + "</td>");//time wasted
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + stats.saved + "cancels \">" + stats.timeSaved + "</span>" + "</td>");//timesaved
                            sw.Write("<td>" + stats.swapCount + "</td>");//w swaps
                            sw.Write("<td>" + stats.downCount + "</td>");//downs
                            long fight_duration = phase.getDuration();
                            if (timedead > TimeSpan.Zero)
                            {
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                    + timedead + "(" + Math.Round((timedead.TotalMilliseconds / fight_duration) * 100,1) + "% Alive) \">" 
                                    + timedead.Minutes + " m " + timedead.Seconds + " s</span>" + " </td>");
                            }
                            else
                            {
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\"" +
                                    " title=\"Never died\"> </span>" + " </td>");
                            }
                        }
                        sw.Write("</tr>");
                    }
                }
                sw.Write("</tbody>");
                if (log.getPlayerList().Count > 1)
                {
                    sw.Write("<tfoot>");
                    {
                        HTMLHelper.writeDamageStatsTableFoot(sw, footerList);
                    }
                    sw.Write("</tfoot>");
                }
            }
            sw.Write("</table>");

        }
        /// <summary>
        /// Creates the damage stats table for hits on just boss
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="fight_duration">Duration of the fight</param>
        private void CreateDMGStatsBossTable(StreamWriter sw, int phase_index)
        {
            //generate dmgstats table
            PhaseData phase = log.getBoss().getPhases(log, settings.ParsePhases)[phase_index];
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#dmgstatsBoss_table" + phase_index + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#dmgstatsBoss_table" + phase_index + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#dmgstatsBoss_table" + phase_index + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dmgstatsBoss_table" + phase_index + "\">");
            {
                sw.Write("<thead>");
                {
                    HTMLHelper.writeDamageStatsTableHeader(sw);
                }
                sw.Write("</thead>");
                List<string[]> footerList = new List<string[]>();
                sw.Write("<tbody>");
                {
                    foreach (Player player in log.getPlayerList())
                    {
                        Statistics.FinalStats stats = statistics.stats[player][phase_index];
                        Statistics.FinalDPS dps = statistics.dps[player][phase_index];

                        TimeSpan timedead = TimeSpan.FromMilliseconds(stats.died);//dead 

                        //gather data for footer
                        footerList.Add(new string[] {
                            player.getGroup().ToString(),
                            stats.powerLoopCountBoss.ToString(),
                            stats.criticalRateBoss.ToString(),
                            stats.scholarRateBoss.ToString(),
                            stats.movingRateBoss.ToString(),
                            stats.flankingRateBoss.ToString(),
                            stats.glanceRateBoss.ToString(),
                            stats.missedBoss.ToString(),
                            stats.interuptsBoss.ToString(),
                            stats.invulnedBoss.ToString(),
                            stats.swapCount.ToString(),
                            stats.downCount.ToString(),
                            stats.critablePowerLoopCountBoss.ToString()
                        });
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + HTMLHelper.GetLink(player.getProf().ToString()) + "\" alt=\"" 
                                + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "<span style=\"display:none\">" + player.getProf() + "</span>" + "</td>");
                            sw.Write("<td>" + player.getCharacter().ToString() + "</td>");

                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + stats.criticalRateBoss + " out of " + stats.critablePowerLoopCountBoss 
                                + " critable hits<br> Total Damage Effected by Crits: " + stats.criticalDmgBoss 
                                + " \">" + Math.Round((Double)(stats.criticalRateBoss) / stats.critablePowerLoopCountBoss * 100,1) 
                                + "%</span>" + "</td>");//crit
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + stats.scholarRateBoss + " out of " + stats.powerLoopCountBoss + " hits <br> Pure Scholar Damage: " 
                                + stats.scholarDmgBoss + "<br> Effective Physical Damage Increase: " 
                                + Math.Round(100.0* (dps.playerBossPowerDamage / (Double)(dps.playerBossPowerDamage - stats.scholarDmgBoss) - 1.0), 3) 
                                + "% \">" + Math.Round((Double)(stats.scholarRateBoss) / stats.powerLoopCountBoss * 100,1) + "%</span>" + "</td>");//scholar
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\""
                                + stats.movingRateBoss + " out of " + stats.powerLoopCountBoss + " hits <br> Pure Seaweed Damage: "
                                + stats.movingDamageBoss + "<br> Effective Physical Damage Increase: "
                                + Math.Round(100.0 * (dps.playerBossPowerDamage / (Double)(dps.playerBossPowerDamage - stats.movingDamageBoss) - 1.0), 3)
                                + "% \">" + Math.Round((Double)(stats.movingRateBoss) / stats.powerLoopCountBoss * 100, 1) + "%</span>" + "</td>");//sws
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + stats.flankingRateBoss + " out of " + stats.powerLoopCountBoss + " hits \">" 
                                + Math.Round(stats.flankingRateBoss / (Double)stats.powerLoopCountBoss * 100,1) + "%</span>" + "</td>");//flank
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + stats.glanceRateBoss + " out of " + stats.powerLoopCountBoss + " hits \">" 
                                + Math.Round(stats.glanceRateBoss / (Double)stats.powerLoopCountBoss * 100,1) + "%</span>" + "</td>");//glance
                            sw.Write("<td>" + stats.missedBoss + "</td>");//misses
                            sw.Write("<td>" + stats.interuptsBoss + "</td>");//interupts
                            sw.Write("<td>" + stats.invulnedBoss + "</td>");//dmg invulned
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + stats.wasted + "cancels \">" + stats.timeWasted + "</span>" + "</td>");//time wasted
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                + stats.saved + "cancels \">" + stats.timeSaved + "</span>" + "</td>");//timesaved
                            sw.Write("<td>" + stats.swapCount + "</td>");//w swaps
                            sw.Write("<td>" + stats.downCount + "</td>");//downs
                            long fight_duration = phase.getDuration();
                            if (timedead > TimeSpan.Zero)
                            {
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" 
                                    + timedead + "(" + Math.Round((timedead.TotalMilliseconds / fight_duration) * 100,1) + "% Alive) \">" 
                                    + timedead.Minutes + " m " + timedead.Seconds + " s</span>" + " </td>");
                            }
                            else
                            {
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\"" +
                                    " title=\"Never died\"> </span>" + " </td>");
                            }
                        }
                        sw.Write("</tr>");
                    }
                }
                sw.Write("</tbody>");
                if (log.getPlayerList().Count > 1)
                {
                    sw.Write("<tfoot>");
                    {
                        HTMLHelper.writeDamageStatsTableFoot(sw, footerList);
                    }
                    sw.Write("</tfoot>");
                }
            }
            sw.Write("</table>");

        }
        /// <summary>
        /// Creates the defense table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="fight_duration">Duration of the fight</param>
        private void CreateDefTable(StreamWriter sw, int phase_index)
        {
            //generate Tankstats table
            PhaseData phase = log.getBoss().getPhases(log, settings.ParsePhases)[phase_index];
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#defstats_table" + phase_index + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#defstats_table" + phase_index + "').DataTable({ \"order\": [[3, \"desc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#defstats_table" + phase_index + "').DataTable({ \"order\": [[3, \"desc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"defstats_table" + phase_index + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Sub</th>");
                        sw.Write("<th></th>");
                        sw.Write("<th>Name</th>");
                        sw.Write("<th>Dmg Taken</th>");
                        sw.Write("<th>Dmg Barrier</th>");
                        //sw.Write("<th>Heal Received</th>");
                        sw.Write("<th>Blocked</th>");
                        sw.Write("<th>Invulned</th>");
                        sw.Write("<th>Evaded</th>");
                        sw.Write("<th><span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"Dodges or Mirage Cloak \">Dodges</span></th>");
                        sw.Write("<th><img src=\"" + HTMLHelper.GetLink("Downs") + "\" alt=\"Downs\" title=\"Times downed\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=\"" + HTMLHelper.GetLink("Dead") + "\" alt=\"Dead\" title=\"Time died\" height=\"18\" width=\"18\">" + "</th>");
                    }
                    sw.Write("</tr>");
                }

                sw.Write("</thead>");
                List<string[]> footerList = new List<string[]>();
                sw.Write("<tbody>");
                {
                    foreach (Player player in log.getPlayerList())
                    {
                        Statistics.FinalDefenses defenses = statistics.defenses[player][phase_index];
                        Statistics.FinalStats stats = statistics.stats[player][phase_index];

                        

                        TimeSpan timedead = TimeSpan.FromMilliseconds(stats.died);//dead
                                                                                              //gather data for footer
                        footerList.Add(new string[]
                        {
                            player.getGroup().ToString(),
                            defenses.damageTaken.ToString(), defenses.damageBarrier.ToString(),
                            defenses.blockedCount.ToString(), defenses.invulnedCount.ToString(),
                            defenses.evadedCount.ToString(), stats.dodgeCount.ToString(),
                            stats.downCount.ToString()//, defenses.allHealReceived.ToString()
                        });
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + HTMLHelper.GetLink(player.getProf().ToString()) + "\" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "<span style=\"display:none\">" + player.getProf() + "</span>" + "</td>");
                            sw.Write("<td>" + player.getCharacter().ToString() + "</td>");
                            sw.Write("<td>" + defenses.damageTaken + "</td>");//dmg taken
                            sw.Write("<td>" + defenses.damageBarrier + "</td>");//dmgbarrier
                            //sw.Write("<td>" + defenses.allHealReceived + "</td>");//dmgbarrier
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + 0 + "Damage \">" + defenses.blockedCount + "</span>" + "</td>");//Blocks  
                            sw.Write("<td>" + defenses.invulnedCount + "</td>");//invulns
                            sw.Write("<td>" + defenses.evadedCount + "</td>");// evades
                            sw.Write("<td>" + stats.dodgeCount + "</td>");//dodges
                            sw.Write("<td>" + stats.downCount + "</td>");//downs
                            long fight_duration = phase.getDuration("s");
                            if (timedead > TimeSpan.Zero)
                            {
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + timedead + "(" + Math.Round((timedead.TotalMilliseconds / fight_duration) * 100,1) + "% Alive) \">" + timedead.Minutes + " m " + timedead.Seconds + " s</span>" + " </td>");
                            }
                            else
                            {
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"Never died 100% Alive) \"> </span>" + " </td>");
                            }
                        }
                        sw.Write("</tr>");
                    }
                }
                sw.Write("</tbody>");
                if (log.getPlayerList().Count > 1)
                {
                    sw.Write("<tfoot>");
                    {
                        foreach (string groupNum in footerList.Select(x => x[0]).Distinct())
                        {
                            List<string[]> groupList = footerList.Where(x => x[0] == groupNum).ToList();
                            sw.Write("<tr>");
                            {
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td>Group " + groupNum + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => long.Parse(c[1])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[2])) + "</td>");
                                //sw.Write("<td>" + groupList.Sum(c => int.Parse(c[8])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[3])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[4])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[5])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[6])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[7])) + "</td>");
                                sw.Write("<td></td>");
                            }
                            sw.Write("</tr>");
                        }
                        sw.Write("<tr>");
                        {
                            sw.Write("<td></td>");
                            sw.Write("<td></td>");
                            sw.Write("<td>Total</td>");
                            sw.Write("<td>" + footerList.Sum(c => long.Parse(c[1])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[2])) + "</td>");
                            //sw.Write("<td>" + footerList.Sum(c => int.Parse(c[8])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[3])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[4])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[5])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[6])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[7])) + "</td>");
                            sw.Write("<td></td>");
                        }
                        sw.Write("</tr>");

                    }
                    sw.Write("</tfoot>");
                }
            }

            sw.Write("</table>");
        }
        /// <summary>
        /// Creates the support table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="fight_duration">Duration of the fight</param>
        private void CreateSupTable(StreamWriter sw, int phase_index)
        {
            //generate suppstats table
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#supstats_table" + phase_index + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#supstats_table" + phase_index + "').DataTable({ \"order\": [[3, \"desc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#supstats_table" + phase_index + "').DataTable({ \"order\": [[3, \"desc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"supstats_table" + phase_index + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Sub</th>");
                        sw.Write("<th></th>");
                        sw.Write("<th>Name</th>");
                        //sw.Write("<th>Healing Done</th>");
                        sw.Write("<th>Condi Cleanse</th>");
                        sw.Write("<th>Resurrects</th>");
                    }

                    sw.Write("</tr>");
                }
                sw.Write("</thead>");
                List<string[]> footerList = new List<string[]>();
                sw.Write("<tbody>");
                {
                    foreach (Player player in log.getPlayerList())
                    {
                        Statistics.FinalSupport support = statistics.support[player][phase_index];

                        //gather data for footer
                        footerList.Add(new string[] {
                            player.getGroup().ToString(),
                            support.condiCleanseTime.ToString(), support.condiCleanse.ToString(),
                            support.ressurrectTime.ToString(), support.resurrects.ToString()//, support.allHeal.ToString()
                        });
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + HTMLHelper.GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "<span style=\"display:none\">" + player.getProf() + "</span>" + "</td>");
                            sw.Write("<td>" + player.getCharacter().ToString() + "</td>");
                            //sw.Write("<td>" + support.allHeal +"</td>");                                              
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + support.condiCleanseTime + " seconds \">" + support.condiCleanse + "</span>" + "</td>");//condicleanse                                                                                                                                                                   //HTML_defstats += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[6] + " Evades \">" + stats[7] + "dmg</span>" + "</td>";//evades
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + support.ressurrectTime + " seconds \">" + support.resurrects + "</span>" + "</td>");//res
                        }
                        sw.Write("</tr>");
                    }
                }
                sw.Write("</tbody>");
                if (log.getPlayerList().Count > 1)
                {
                    sw.Write("<tfoot>");
                    {
                        foreach (string groupNum in footerList.Select(x => x[0]).Distinct())
                        {
                            List<string[]> groupList = footerList.Where(x => x[0] == groupNum).ToList();
                            sw.Write("<tr>");
                            {
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td>Group " + groupNum + "</td>");
                                //sw.Write("<td>" + groupList.Sum(c => int.Parse(c[5])).ToString() + "</td>");
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => Double.Parse(c[1])).ToString() + " seconds \">" + groupList.Sum(c => int.Parse(c[2])).ToString() + " condis</span>" + "</td>");
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => Double.Parse(c[3])).ToString() + " seconds \">" + groupList.Sum(c => int.Parse(c[4])) + "</span>" + "</td>");
                            }
                            sw.Write("</tr>");
                        }
                        sw.Write("<tr>");
                        {
                            sw.Write("<td></td>");
                            sw.Write("<td></td>");
                            sw.Write("<td>Total</td>");
                            //sw.Write("<td>" + footerList.Sum(c => int.Parse(c[5])).ToString() + "</td>");
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => Double.Parse(c[1])).ToString() + " seconds \">" + footerList.Sum(c => int.Parse(c[2])).ToString() + " condis</span>" + "</td>");
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => Double.Parse(c[3])).ToString() + " seconds \">" + footerList.Sum(c => int.Parse(c[4])).ToString() + "</span>" + "</td>");
                        }
                        sw.Write("</tr>");
                    }
                    sw.Write("</tfoot>");
                }
            }

            sw.Write("</table>");
        }
        /// <summary>
        /// Create the buff uptime table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="list_to_use">Boon list to use</param>
        /// <param name="table_id">id of the table</param>
        private void CreateUptimeTable(StreamWriter sw, List<Boon> list_to_use, string table_id, int phase_index)
        {
            List<PhaseData> phases = log.getBoss().getPhases(log, settings.ParsePhases);
            //Generate Boon table------------------------------------------------------------------------------------------------
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#" + table_id + phase_index + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#" + table_id + phase_index + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#" + table_id + phase_index + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            List<List<string>> footList = new List<List<string>>();
            sw.Write("<table class=\"display table table-striped table-hover compact\" cellspacing=\"0\" id=\"" + table_id + phase_index + "\">");
            {
                HTMLHelper.writeBoonTableHeader(sw, list_to_use);
                HashSet<int> intensityBoon = new HashSet<int>();
                bool boonTable = list_to_use.Select(x => x.getID()).Contains(740);
                sw.Write("<tbody>");
                {
                    foreach (Player player in log.getPlayerList())
                    {

                        Dictionary<long, Statistics.FinalBoonUptime> boons = statistics.selfBoons[player][phase_index];
                        Dictionary<long, Dictionary<int, string[]>> extraBoonData = player.getExtraBoonData(log, phases, list_to_use);
                        List<string> boonArrayToList = new List<string>();
                        boonArrayToList.Add(player.getGroup().ToString());
                        long fight_duration = phases[phase_index].getDuration();
                        Dictionary<long, long> boonPresence = player.getBoonPresence(log, phases, list_to_use, phase_index);
                        int count = 0;

                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + HTMLHelper.GetLink(player.getProf().ToString()) + "\" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "<span style=\"display:none\">" + player.getProf() + "</span>" + "</td>");
                            if (boonTable)
                            {                        
                                double avg_boons = 0.0;
                                foreach (long duration in boonPresence.Values)
                                {
                                    avg_boons += duration;
                                }
                                avg_boons /= fight_duration;
                                sw.Write("<td data-toggle=\"tooltip\" title=\"Average number of boons: " + Math.Round(avg_boons, 1) + "\">" + player.getCharacter().ToString() + " </td>");
                            }
                            else
                            {
                                sw.Write("<td>" + player.getCharacter().ToString() + "</td>");
                            }
                            foreach (Boon boon in list_to_use)
                            {
                                if (boon.getType() == Boon.BoonType.Intensity)
                                {
                                    intensityBoon.Add(count);
                                }
                                string tooltip = "";
                                if (extraBoonData.TryGetValue(boon.getID(),out var myDict))
                                {
                                    string[] tooltips = myDict[phase_index];
                                    tooltip = " <br> <big><b>Boss</b></big> </br> " + tooltips[1] + " <br> <big><b>All</b></big> </br> " + tooltips[0];
                                }
                                string toWrite = boons[boon.getID()].uptime + (intensityBoon.Contains(count) ? "" : "%");
                                if (tooltip.Length > 0)
                                {
                                    sw.Write("<td data-html=\"true\" data-toggle=\"tooltip\" title=\"" + tooltip + "\">" + toWrite + " </td>");
                                }
                                else
                                {
                                    if (boonTable && boon.getType() == Boon.BoonType.Intensity && boonPresence.TryGetValue(boon.getID(),out long presenceValue))
                                    {
                                        tooltip = "uptime: " + Math.Round(100.0* presenceValue / fight_duration,1) + "%";
                                        sw.Write("<td data-toggle=\"tooltip\" title=\"" + tooltip + "\">" + toWrite + " </td>");
                                    } else
                                    {
                                        sw.Write("<td>" + toWrite + "</td>");
                                    }
                                }                                
                                boonArrayToList.Add(boons[boon.getID()].uptime.ToString());                        
                                count++;
                            }
                        }
                        sw.Write("</tr>");
                        //gather data for footer
                        footList.Add(boonArrayToList);
                    }
                }
                sw.Write("</tbody>");
                if (log.getPlayerList().Count > 1)
                {
                    sw.Write("<tfoot>");
                    {
                        foreach (string groupNum in footList.Select(x => x[0]).Distinct())//selecting group
                        {
                            List<List<string>> groupList = footList.Where(x => x[0] == groupNum).ToList();
                            sw.Write("<tr>");
                            {
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td>Group " + groupNum + "</td>");
                                for (int i = 1; i < groupList[0].Count; i++)
                                {
                                    if (intensityBoon.Contains(i - 1))
                                    {
                                        sw.Write("<td>" + Math.Round(groupList.Sum(c => Double.Parse(c[i])) / groupList.Count, 1) + "</td>");
                                    }
                                    else
                                    {
                                        sw.Write("<td>" + Math.Round(groupList.Sum(c => Double.Parse(c[i].TrimEnd('%'))) / groupList.Count, 1) + "%</td>");
                                    }

                                }
                            }
                            sw.Write("</tr>");
                        }
                        sw.Write("<tr>");
                        {
                            sw.Write("<td></td>");
                            sw.Write("<td></td>");
                            sw.Write("<td>Averages</td>");
                            for (int i = 1; i < footList[0].Count; i++)
                            {
                                if (intensityBoon.Contains(i - 1))
                                {
                                    sw.Write("<td>" + Math.Round(footList.Sum(c => Double.Parse(c[i])) / footList.Count, 1) + "</td>");
                                }
                                else
                                {
                                    sw.Write("<td>" + Math.Round(footList.Sum(c => Double.Parse(c[i].TrimEnd('%'))) / footList.Count, 1) + "%</td>");
                                }
                            }
                        }
                        sw.Write("</tr>");
                    }
                    sw.Write("</tfoot>");
                }
            }
            sw.Write("</table>");
        }
        /// <summary>
        /// Create the self buff generation table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="list_to_use">Boon list to use</param>
        /// <param name="table_id">id of the table</param>
        private void CreateGenSelfTable(StreamWriter sw, List<Boon> list_to_use, string table_id, int phase_index)
        { //Generate BoonGenSelf table
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#" + table_id + phase_index + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#" + table_id + phase_index + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#" + table_id + phase_index + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\" cellspacing=\"0\" width=\"100%\" id=\"" + table_id + phase_index + "\">");
            {
                HTMLHelper.writeBoonTableHeader(sw, list_to_use);
                sw.Write("<tbody>");
                {
                    foreach (Player player in log.getPlayerList())
                    {
                        Dictionary<long, Statistics.FinalBoonUptime> uptimes = statistics.selfBoons[player][phase_index];

                        Dictionary<long, string> rates = new Dictionary<long, string>();
                        foreach (Boon boon in list_to_use)
                        {
                            string rate = "0";

                            Statistics.FinalBoonUptime uptime = uptimes[boon.getID()];

                            if (uptime.generation > 0)
                            {
                                if (boon.getType() == Boon.BoonType.Duration)
                                {
                                    rate =
                                        "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + uptime.overstack + "% with overstack \">"
                                        + uptime.generation
                                        + "%</span>";
                                }
                                else if (boon.getType() == Boon.BoonType.Intensity)
                                {
                                    rate =
                                        "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + uptime.overstack + " with overstack \">"
                                        + uptime.generation
                                        + "</span>";
                                }

                            }

                            rates[boon.getID()] = rate;
                        }

                        HTMLHelper.writeBoonGenTableBody(sw, player, list_to_use, rates);
                    }
                }
                sw.Write("</tbody>");
            }

            sw.Write("</table>");
        }
        /// <summary>
        /// Create the group buff generation table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="list_to_use">Boon list to use</param>
        /// <param name="table_id">id of the table</param>
        private void CreateGenGroupTable(StreamWriter sw, List<Boon> list_to_use, string table_id, int phase_index)
        { //Generate BoonGenGroup table
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#" + table_id + phase_index + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#" + table_id + phase_index + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#" + table_id + phase_index + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"" + table_id + phase_index + "\">");
            {
                HTMLHelper.writeBoonTableHeader(sw, list_to_use);
                sw.Write("<tbody>");
                {
                    foreach (Player player in log.getPlayerList())
                    {
                        Dictionary<long, Statistics.FinalBoonUptime> boons =
                            statistics.groupBoons[player][phase_index];

                        Dictionary<long, string> rates = new Dictionary<long, string>();
                        foreach (Boon boon in list_to_use)
                        {
                            string rate = "0";

                            Statistics.FinalBoonUptime uptime = boons[boon.getID()];

                            if (uptime.generation > 0)
                            {
                                if (boon.getType() == Boon.BoonType.Duration)
                                {
                                    rate =
                                        "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + uptime.overstack + "% with overstack \">"
                                        + uptime.generation
                                        + "%</span>";
                                }
                                else if (boon.getType() == Boon.BoonType.Intensity)
                                {
                                    rate =
                                        "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + uptime.overstack + " with overstack \">"
                                        + uptime.generation
                                        + "</span>";
                                }
                            }

                            rates[boon.getID()] = rate;
                        }

                        HTMLHelper.writeBoonGenTableBody(sw, player, list_to_use, rates);
                    }
                }
                sw.Write("</tbody>");
            }
            sw.Write("</table>");
        }
        /// <summary>
        /// Create the off squade buff generation table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="list_to_use">Boon list to use</param>
        /// <param name="table_id">id of the table</param>
        private void CreateGenOGroupTable(StreamWriter sw, List<Boon> list_to_use, string table_id, int phase_index)
        {  //Generate BoonGenOGroup table
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#" + table_id + phase_index + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#" + table_id + phase_index + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#" + table_id + phase_index + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"" + table_id + phase_index + "\">");
            {
                HTMLHelper.writeBoonTableHeader(sw, list_to_use);
                sw.Write("<tbody>");
                {
                    foreach (Player player in log.getPlayerList())
                    {
                        Dictionary<long, Statistics.FinalBoonUptime> boons =
                            statistics.offGroupBoons[player][phase_index];

                        Dictionary<long, string> rates = new Dictionary<long, string>();
                        foreach (Boon boon in list_to_use)
                        {
                            string rate = "0";

                            Statistics.FinalBoonUptime uptime = boons[boon.getID()];

                            if (uptime.generation > 0)
                            {
                                if (boon.getType() == Boon.BoonType.Duration)
                                {
                                    rate =
                                        "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + uptime.overstack + "% with overstack \">"
                                        + uptime.generation
                                        + "%</span>";
                                }
                                else if (boon.getType() == Boon.BoonType.Intensity)
                                {
                                    rate =
                                        "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + uptime.overstack + " with overstack \">"
                                        + uptime.generation
                                        + "</span>";
                                }
                            }

                            rates[boon.getID()] = rate;
                        }

                        HTMLHelper.writeBoonGenTableBody(sw, player, list_to_use, rates);
                    }
                }
                sw.Write("</tbody>");
            }
            sw.Write("</table>");
        }
        /// <summary>
        /// Create the squad buff generation table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="list_to_use">Boon list to use</param>
        /// <param name="table_id">id of the table</param>
        private void CreateGenSquadTable(StreamWriter sw, List<Boon> list_to_use, string table_id, int phase_index)
        {
            //Generate BoonGenSquad table
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var lazyTable = document.querySelector('#" + table_id + phase_index + "');" +

                    "if ('IntersectionObserver' in window) {" +
                        "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                            "entries.forEach(function(entry) {" +
                                "if (entry.isIntersecting)" +
                                "{" +
                                    "$(function () { $('#" + table_id + phase_index + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                                    "lazyTableObserver.unobserve(entry.target);" +
                                "}" +
                            "});" +
                        "});" +
                    "lazyTableObserver.observe(lazyTable);" +
                    "} else {" +
                        "$(function () { $('#" + table_id + phase_index + "').DataTable({ \"order\": [[0, \"asc\"]]});});" +
                    "}");
                }
                sw.Write("});");
            }
            sw.Write("</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"" + table_id + phase_index + "\">");
            {
                HTMLHelper.writeBoonTableHeader(sw, list_to_use);
                sw.Write("<tbody>");
                {
                    foreach (Player player in log.getPlayerList())
                    {
                        Dictionary<long, Statistics.FinalBoonUptime> boons =
                            statistics.squadBoons[player][phase_index];

                        Dictionary<long, string> rates = new Dictionary<long, string>();
                        foreach (Boon boon in list_to_use)
                        {
                            string rate = "0";

                            Statistics.FinalBoonUptime uptime = boons[boon.getID()];

                            if (uptime.generation > 0)
                            {
                                if (boon.getType() == Boon.BoonType.Duration)
                                {
                                    rate =
                                        "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + uptime.overstack + "% with overstack \">"
                                        + uptime.generation
                                        + "%</span>";
                                }
                                else if (boon.getType() == Boon.BoonType.Intensity)
                                {
                                    rate =
                                        "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\""
                                        + uptime.overstack + " with overstack \">"
                                        + uptime.generation
                                        + "</span>";
                                }
                            }

                            rates[boon.getID()] = rate;
                        }

                        HTMLHelper.writeBoonGenTableBody(sw, player, list_to_use, rates);
                    }
                }
                sw.Write("</tbody>");
            }
            sw.Write("</table>");
        }
        /// <summary>
        /// Creates the player tab
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreatePlayerTab(StreamWriter sw, int phase_index)
        {
            List<PhaseData> phases = log.getBoss().getPhases(log, settings.ParsePhases);
            PhaseData phase = phases[phase_index];
            long start = phase.getStart() + log.getBossData().getFirstAware();
            long end = phase.getEnd() + log.getBossData().getFirstAware();
            List<SkillItem> s_list = log.getSkillData().getSkillList();
            //generate Player list Graphs
            foreach (Player p in log.getPlayerList())
            {
                List<CastLog> casting = p.getCastLogsActDur(log, phase.getStart(), phase.getEnd());

                bool died = p.getDeath(log, phase.getStart(), phase.getEnd()) > 0;
                string charname = p.getCharacter();
                string pid = p.getInstid() + "_" + phase_index;
                sw.Write("<div class=\"tab-pane fade\" id=\"" + pid + "\">");
                {
                    sw.Write("<h1 align=\"center\"> " + charname + "<img src=\"" + HTMLHelper.GetLink(p.getProf().ToString()) + "\" alt=\"" + p.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</h1>");
                    sw.Write("<ul class=\"nav nav-tabs\">");
                    {
                        sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#home" + pid + "\">" + p.getCharacter() + "</a></li>");
                        if (settings.SimpleRotation)
                        {
                            sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#SimpleRot" + pid + "\">Simple Rotation</a></li>");

                        }
                        if (died)
                        {
                            sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#DeathRecap" + pid + "\">Death Recap</a></li>");

                        }
                        //foreach pet loop here                        
                        foreach (KeyValuePair<string, Minions> pair in p.getMinions(log))
                        {
                            sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#minion" + pid + "_" + pair.Value.getInstid() + "\">" + pair.Key + "</a></li>");
                        }
                        //inc dmg
                        sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#incDmg" + pid + "\">Damage Taken</a></li>");
                    }
                    sw.Write("</ul>");
                    sw.Write("<div id=\"myTabContent\" class=\"tab-content\">");
                    {
                        sw.Write("<div class=\"tab-pane fade show active\" id=\"home" + pid + "\">");
                        {
                            List<long[]> consume = p.getConsumablesList(log, phase.getStart(), phase.getEnd());
                            List<long[]> initial = consume.Where(x => x[1] == 0).ToList();
                            List<long[]> refreshed = consume.Where(x => x[1] > 0).ToList();
                            if (initial.Count > 0)
                            {
                                Boon food = null;
                                Boon utility = null;
                                foreach (long[] buff in initial)
                                {

                                    Boon foodCheck = Boon.getFoodList().FirstOrDefault(x => x.getID() == buff[0]);
                                    if (foodCheck != null)
                                    {
                                        food = foodCheck;
                                        continue;
                                    }
                                    Boon utilCheck = Boon.getUtilityList().FirstOrDefault(x => x.getID() == buff[0]);
                                    if (utilCheck != null)
                                    {
                                        utility = utilCheck;
                                        continue;
                                    }
                                }
                                sw.Write("<p>Started with ");
                                if (food != null)
                                {
                                    sw.Write(food.getName() + "<img src=\"" + food.getLink() + "\" alt=\"" + food.getName() + "\" height=\"18\" width=\"18\" >");
                                }
                                if (utility != null)
                                {
                                    sw.Write((food != null ?" and " : "") + utility.getName() + "<img src=\"" + utility.getLink() + "\" alt=\"" + utility.getName() + "\" height=\"18\" width=\"18\" >");
                                }
                                sw.Write("</p>");
                            }
                            if (refreshed.Count > 0)
                            {
                                Boon food = null;
                                Boon utility = null;
                                foreach (long[] buff in refreshed)
                                {

                                    Boon foodCheck = Boon.getFoodList().FirstOrDefault(x => x.getID() == buff[0]);
                                    if (foodCheck != null)
                                    {
                                        food = foodCheck;
                                        continue;
                                    }
                                    Boon utilCheck = Boon.getUtilityList().FirstOrDefault(x => x.getID() == buff[0]);
                                    if (utilCheck != null)
                                    {
                                        utility = utilCheck;
                                        continue;
                                    }
                                }
                                sw.Write("<p>Refreshed ");
                                if (food != null)
                                {
                                    sw.Write(food.getName() + "<img src=\"" + food.getLink() + "\" alt=\"" + food.getName() + "\" height=\"18\" width=\"18\" >");
                                }
                                if (utility != null)
                                {
                                    sw.Write((food != null ? " and " : "") + utility.getName() + "<img src=\"" + utility.getLink() + "\" alt=\"" + utility.getName() + "\" height=\"18\" width=\"18\" >");
                                }
                                sw.Write("</p>");
                            }
                            sw.Write("<div id=\"Graph" + pid + "\" style=\"height: 1000px;width:1000px; display:inline-block \"></div>");
                            sw.Write("<script>");
                            {
                                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                                {
                                    sw.Write("var data = [");
                                    {
                                        if (settings.PlayerRot)//Display rotation
                                        {
                                            foreach (CastLog cl in casting)
                                            {
                                                HTMLHelper.writeCastingItem(sw, cl, log.getSkillData(), phase.getStart(), phase.getEnd());
                                            }
                                        }
                                        if (statistics.present_boons.Count > 0)
                                        {
                                            List<Boon> parseBoonsList = new List<Boon>();
                                            parseBoonsList.AddRange(statistics.present_boons);
                                            parseBoonsList.AddRange(statistics.present_offbuffs);
                                            parseBoonsList.AddRange(statistics.present_defbuffs);
                                            if (statistics.present_personnal.ContainsKey(p.getInstid()))
                                            {
                                                parseBoonsList.AddRange(statistics.present_personnal[p.getInstid()]);
                                            }
                                            Dictionary<long, BoonsGraphModel> boonGraphData = p.getBoonGraphs(log, phases, parseBoonsList);
                                            foreach (BoonsGraphModel bgm in boonGraphData.Values.Reverse().Where(x => x.getBoonName() != "Number of Conditions"))
                                            {
                                                sw.Write("{");
                                                {
                                                    HTMLHelper.writeBoonGraph(sw, bgm, phase.getStart(), phase.getEnd());
                                                }
                                                sw.Write(" },");

                                            }
                                        }
                                        if (settings.DPSGraphTotals)
                                        {//show total dps plot
                                            sw.Write("{");
                                            { //Adding dps axis
                                                HTMLHelper.writeDPSGraph(sw, "Total DPS", GraphHelper.getTotalDPSGraph(log, p, phase_index, GraphHelper.GraphMode.Full), p);
                                            }
                                            sw.Write("},");
                                            if (settings.Show10s)
                                            {
                                                sw.Write("{");
                                                HTMLHelper.writeDPSGraph(sw, "Total DPS - 10s", GraphHelper.getTotalDPSGraph(log, p, phase_index, GraphHelper.GraphMode.s10), p);
                                                sw.Write("},");
                                            }
                                            if (settings.Show30s)
                                            {
                                                sw.Write("{");
                                                HTMLHelper.writeDPSGraph(sw, "Total DPS - 30s", GraphHelper.getTotalDPSGraph(log, p, phase_index, GraphHelper.GraphMode.s30), p);
                                                sw.Write("},");
                                            }
                                        }
                                         //Adding dps axis
                                            sw.Write("{");
                                            {
                                                HTMLHelper.writeDPSGraph(sw, "Boss DPS", GraphHelper.getBossDPSGraph(log, p, phase_index, GraphHelper.GraphMode.Full), p);
                                            }
                                            sw.Write("},");
                                            if (settings.Show10s)
                                            {
                                                sw.Write("{");
                                                HTMLHelper.writeDPSGraph(sw, "Boss DPS - 10s", GraphHelper.getBossDPSGraph(log, p, phase_index, GraphHelper.GraphMode.s10), p);
                                                sw.Write("},");
                                            }
                                            if (settings.Show30s)
                                            {
                                                sw.Write("{");
                                                HTMLHelper.writeDPSGraph(sw, "Boss DPS - 30s", GraphHelper.getBossDPSGraph(log, p, phase_index, GraphHelper.GraphMode.s30), p);
                                                sw.Write("},");
                                            }

                                        //Adding dps axis
                                        if (settings.ClDPSGraphTotals)
                                        {//show total dps plot
                                            sw.Write("{");
                                            { //Adding dps axis
                                                HTMLHelper.writeDPSGraph(sw, "Cleave DPS", GraphHelper.getCleaveDPSGraph(log, p, phase_index, GraphHelper.GraphMode.Full), p);
                                            }
                                            sw.Write("},");
                                            if (settings.Show10s)
                                            {
                                                sw.Write("{");
                                                HTMLHelper.writeDPSGraph(sw, "Cleave DPS - 10s", GraphHelper.getCleaveDPSGraph(log, p, phase_index, GraphHelper.GraphMode.s10), p);
                                                sw.Write("},");
                                            }
                                            if (settings.Show30s)
                                            {
                                                sw.Write("{");
                                                HTMLHelper.writeDPSGraph(sw, "Cleave DPS - 30s", GraphHelper.getCleaveDPSGraph(log, p, phase_index, GraphHelper.GraphMode.s30), p);
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
                                            if (settings.PlayerRot && settings.PlayerRotIcons)//Display rotation
                                            {
                                                int castCount = 0;
                                                foreach (CastLog cl in casting)
                                                {
                                                    HTMLHelper.writeCastingItemIcon(sw, cl, log.getSkillData(), phase.getStart(), castCount == casting.Count - 1);
                                                    castCount++;
                                                }
                                            }
                                        }
                                        sw.Write("],");
                                        if (settings.LightTheme)
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
                                sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#distTabBoss" + pid + "\">" + "Boss" + "</a></li>");
                                sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#distTabAll" + pid + "\">" + "All" + "</a></li>");
                            }
                            sw.Write("</ul>");
                            sw.Write("<div class=\"tab-content\">");
                            {
                                sw.Write("<div class=\"tab-pane fade show active\" id=\"distTabBoss" + pid + "\">");
                                {
                                    CreateDMGDistTable(sw, p, true, phase_index);
                                }
                                sw.Write("</div>");
                                sw.Write("<div class=\"tab-pane fade \" id=\"distTabAll" + pid + "\">");
                                {
                                    CreateDMGDistTable(sw, p, false, phase_index);
                                }
                                sw.Write("</div>");
                            }
                            sw.Write("</div>");
                        }
                        sw.Write("</div>");
                        foreach (KeyValuePair<string, Minions> pair in p.getMinions(log))
                        {
                            string id = pid + "_" + pair.Value.getInstid();
                            sw.Write("<div class=\"tab-pane fade \" id=\"minion" + id + "\">");
                            {
                                sw.Write("<ul class=\"nav nav-tabs\">");
                                {
                                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#distTabBoss" + id + "\">" + "Boss" + "</a></li>");
                                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#distTabAll" + id + "\">" + "All" + "</a></li>");
                                }
                                sw.Write("</ul>");
                                sw.Write("<div class=\"tab-content\">");
                                {
                                    sw.Write("<div class=\"tab-pane fade show active\" id=\"distTabBoss" + id + "\">");
                                    {
                                        CreateDMGDistTable(sw, p, pair.Value, true, phase_index);
                                    }
                                    sw.Write("</div>");
                                    sw.Write("<div class=\"tab-pane fade\" id=\"distTabAll" + id + "\">");
                                    {
                                        CreateDMGDistTable(sw, p, pair.Value, false, phase_index);
                                    }
                                    sw.Write("</div>");
                                }
                                sw.Write("</div>");
                            }
                            sw.Write("</div>");
                        }
                        if (settings.SimpleRotation)
                        {
                            sw.Write("<div class=\"tab-pane fade \" id=\"SimpleRot" + pid + "\">");
                            {
                                int simpleRotSize = 20;
                                if (settings.LargeRotIcons)
                                {
                                    simpleRotSize = 30;
                                }
                                CreateSimpleRotationTab(sw, p, simpleRotSize, phase_index);
                            }
                            sw.Write("</div>");
                        }
                        if (died && phase_index == 0)
                        {
                            sw.Write("<div class=\"tab-pane fade \" id=\"DeathRecap" + pid + "\">");
                            {
                                CreateDeathRecap(sw, p);
                            }
                            sw.Write("</div>");
                        }
                        sw.Write("<div class=\"tab-pane fade \" id=\"incDmg" + pid + "\">");
                        {
                            CreateDMGTakenDistTable(sw, p, phase_index);
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
        /// <param name="sw">Stream writer</param>
        /// <param name="p">The player</param>
        /// <param name="simpleRotSize">Size of the images</param>
        private void CreateSimpleRotationTab(StreamWriter sw, Player p, int simpleRotSize, int phase_index)
        {
            if (settings.PlayerRot)//Display rotation
            {
                PhaseData phase = log.getBoss().getPhases(log, settings.ParsePhases)[phase_index];
                List<CastLog> casting = p.getCastLogs(log, phase.getStart(), phase.getEnd());
                GW2APISkill autoSkill = null;
                int autosCount = 0;
                foreach (CastLog cl in casting)
                {
                    GW2APISkill apiskill = null;
                    SkillItem skill = log.getSkillData().getSkillList().FirstOrDefault(x => x.getID() == cl.getID());
                    if (skill != null)
                    {
                        apiskill = skill.GetGW2APISkill();
                    }


                    if (apiskill != null)
                    {
                        if (apiskill.slot != "Weapon_1")
                        {
                            if (autosCount > 0 && settings.ShowAutos)
                            {
                                sw.Write("<span class=\"rot-skill\"><div class=\"rot-crop\"><img src=\"" + autoSkill.icon + "\" data-toggle=\"tooltip\" title= \"" + autoSkill.name + "[Auto Attack] x" + autosCount + " \" height=\"" + simpleRotSize + "\" width=\"" + simpleRotSize + "\"></div></span>");
                                autosCount = 0;
                            }
                            sw.Write("<span class=\"rot-skill\"><div class=\"rot-crop\"><img src=\"" + apiskill.icon + "\" data-toggle=\"tooltip\" title= \"" + apiskill.name + " Time: " + cl.getTime() + "ms " + "Dur: " + cl.getActDur() + "ms \" height=\"" + simpleRotSize + "\" width=\"" + simpleRotSize + "\"></div></span>");
                        }
                        else
                        {
                            if (autosCount == 0)
                            {
                                autoSkill = apiskill;
                            }
                            autosCount++;
                        }
                    }
                    else
                    {
                        string skillName = "";
                        string skillLink = "";

                        if (cl.getID() == -2)
                        {//wepswap
                            skillName = "Weapon Swap";
                            skillLink = HTMLHelper.GetLink("Swap");
                            sw.Write("<span class=\"rot-skill\"><div class=\"rot-crop\"><img src=\"" + skillLink + "\" data-toggle=\"tooltip\" title= \"" + skillName + " Time: " + cl.getTime() + "ms " + "Dur: " + cl.getActDur() + "ms \" height=\"" + simpleRotSize + "\" width=\"" + simpleRotSize + "\"></div></span>");
                            sw.Write("<br>");
                            continue;
                        }
                        else if (cl.getID() == 1066)
                        {
                            skillName = "Resurrect";
                            skillLink = HTMLHelper.GetLink("Downs");
                            sw.Write("<span class=\"rot-skill\"><div class=\"rot-crop\"><img src=\"" + skillLink + "\" data-toggle=\"tooltip\" title= \"" + skillName + " Time: " + cl.getTime() + "ms " + "Dur: " + cl.getActDur() + "ms \" height=\"" + simpleRotSize + "\" width=\"" + simpleRotSize + "\"></div></span>");

                        }
                        else
                        if (cl.getID() == 1175)
                        {
                            skillName = "Bandage";
                            skillLink = HTMLHelper.GetLink("Bandage");
                            sw.Write("<span class=\"rot-skill\"><div class=\"rot-crop\"><img src=\"" + skillLink + "\" data-toggle=\"tooltip\" title= \"" + skillName + " Time: " + cl.getTime() + "ms " + "Dur: " + cl.getActDur() + "ms \" height=\"" + simpleRotSize + "\" width=\"" + simpleRotSize + "\"></div></span>");

                        }
                        else
                        if (cl.getID() == 65001)
                        {
                            skillName = "Dodge";
                            skillLink = HTMLHelper.GetLink("Dodge");
                            sw.Write("<span class=\"rot-skill\"><div class=\"rot-crop\"><img src=\"" + skillLink + "\" data-toggle=\"tooltip\" title= \"" + skillName + " Time: " + cl.getTime() + "ms " + "Dur: " + cl.getActDur() + "ms \" height=\"" + simpleRotSize + "\" width=\"" + simpleRotSize + "\"></div></span>");

                        }

                    }

                }
            }

        }
        /// <summary>
        /// Creates the death recap tab for a given player
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="p">The player</param>
        private void CreateDeathRecap(StreamWriter sw, Player p)
        {
            List<DamageLog> damageLogs = p.getDamageTakenLogs(log, 0, log.getBossData().getAwareDuration());
            List<SkillItem> s_list = log.getSkillData().getSkillList();
            long start = log.getBossData().getFirstAware();
            long end = log.getBossData().getLastAware();
            List<CombatItem> down = log.getCombatData().getStates(p.getInstid(), ParseEnum.StateChange.ChangeDown, start, end);
            if (down.Count > 0)
            {
                List<CombatItem> ups = log.getCombatData().getStates(p.getInstid(), ParseEnum.StateChange.ChangeUp, start, end);
                // Surely a consumable in fractals
                if (ups.Count > down.Count)
                {
                    down = new List<CombatItem>();
                }
                else
                {
                    down = down.GetRange(ups.Count, down.Count - ups.Count);
                }
            }
            List<CombatItem> dead = log.getCombatData().getStates(p.getInstid(), ParseEnum.StateChange.ChangeDead, start, end);
            List<DamageLog> damageToDown = new List<DamageLog>();
            List<DamageLog> damageToKill = new List<DamageLog>();
            if (down.Count > 0)
            {//went to down state before death
                damageToDown = damageLogs.Where(x => x.getTime() < down.Last().getTime() - start && x.getDamage() > 0).ToList();
                damageToKill = damageLogs.Where(x => x.getTime() > down.Last().getTime() - start && x.getTime() < dead.Last().getTime() - start && x.getDamage() > 0).ToList();
                //Filter last 30k dmg taken
                int totaldmg = 0;
                for (int i = damageToDown.Count - 1; i > 0; i--)
                {
                    totaldmg += damageToDown[i].getDamage();
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
                sw.Write("<p>Took " + damageToDown.Sum(x => x.getDamage()) + " damage in " +
                ((damageToDown.Last().getTime() - damageToDown.First().getTime()) / 1000f).ToString() + " seconds to enter downstate");
                if (damageToKill.Count > 0)
                {
                    sw.Write("<p>Took " + damageToKill.Sum(x => x.getDamage()) + " damage in " +
                       ((damageToKill.Last().getTime() - damageToKill.First().getTime()) / 1000f).ToString() + " seconds to die</p>");
                }
                else
                {
                    sw.Write("<p>Instant death after a down</p>");
                }
                sw.Write("</center>");
            }
            else
            {
                damageToKill = damageLogs.Where(x => x.getTime() < dead.Last().getTime() && x.getDamage() > 0).ToList();
                //Filter last 30k dmg taken
                int totaldmg = 0;
                for (int i = damageToKill.Count - 1; i > 0; i--)
                {
                    totaldmg += damageToKill[i].getDamage();
                    if (totaldmg > 30000)
                    {
                        damageToKill = damageToKill.GetRange(i, damageToKill.Count - 1 - i);
                        break;
                    }
                }
                sw.Write("<center><h3>Player was insta killed by a mechanic, fall damage or by /gg</h3></center>");
            }
            string pid = p.getInstid().ToString();
            sw.Write("<center><div id=\"BarDeathRecap" + pid + "\"></div> </center>");
            sw.Write("<script>");
            {
                sw.Write("var data = [{");
                //Time on X
                sw.Write("x : [");
                if (damageToDown.Count != 0)
                {
                    for (int d = 0; d < damageToDown.Count; d++)
                    {
                        sw.Write("'" + damageToDown[d].getTime() / 1000f + "s',");
                    }
                }
                for (int d = 0; d < damageToKill.Count; d++)
                {
                    sw.Write("'" + damageToKill[d].getTime() / 1000f + "s'");

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
                    for (int d = 0; d < damageToDown.Count; d++)
                    {
                        sw.Write("'" + damageToDown[d].getDamage() + "',");
                    }
                }
                for (int d = 0; d < damageToKill.Count; d++)
                {
                    sw.Write("'" + damageToKill[d].getDamage() + "'");

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

                    if (down.Count == 0)
                    {
                        //damagetoKill was instant(not in downstate)
                        sw.Write("'rgb(0,255,0,1)'");
                    }
                    else
                    {
                        //damageto killwas from downstate
                        sw.Write("'rgb(255,0,0,1)'");
                    }


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
                    for (int d = 0; d < damageToDown.Count; d++)
                    {
                        AgentItem ag = log.getAgentData().GetAgentWInst(damageToDown[d].getInstidt());
                        string name = "UNKNOWN";
                        if (ag != null)
                        {
                            name = ag.getName().Replace("\0", "").Replace("\'", "\\'");
                        }
                        string skillname = log.getSkillData().getName(damageToDown[d].getID()).Replace("\'", "\\'");
                        sw.Write("'" + name + "<br>" + skillname + " hit you for " + damageToDown[d].getDamage() + "',");
                    }
                }
                for (int d = 0; d < damageToKill.Count; d++)
                {
                    AgentItem ag = log.getAgentData().GetAgentWInst(damageToKill[d].getInstidt());
                    string name = "UNKNOWN";
                    if (ag != null )
                    {
                        name = ag.getName().Replace("\0", "").Replace("\'", "\\'");
                    }
                    string skillname = log.getSkillData().getName(damageToKill[d].getID()).Replace("\'", "\\'");
                    sw.Write("'" + name + "<br>" +
                           "hit you with <b>" + skillname + "</b> for " + damageToKill[d].getDamage() + "'");

                    if (d != damageToKill.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write("],");
                sw.Write("type:'bar',");

                sw.Write("}];");

                if (!settings.LightTheme)
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

        private void CreateDMGDistTableBody(StreamWriter sw, bool toBoss, List<CastLog> casting, List<DamageLog> damageLogs, int finalTotalDamage)
        {
            HashSet<long> usedIDs = new HashSet<long>();
            List<SkillItem> s_list = log.getSkillData().getSkillList();
            HTMLHelper.writeDamageDistTableCondi(sw, usedIDs, damageLogs, finalTotalDamage);
            foreach (int id in damageLogs.Where(x => !usedIDs.Contains(x.getID())).Select(x => x.getID()).Distinct().ToList())
            {
                SkillItem skill = s_list.FirstOrDefault(x => x.getID() == id);
                List<DamageLog> list_to_use = damageLogs.Where(x => x.getID() == id).ToList();
                usedIDs.Add(id);
                if (skill != null && list_to_use.Count > 0)
                {
                    List<CastLog> clList = casting.Where(x => x.getID() == id).ToList();
                    int casts = clList.Count;
                    double timeswasted = 0;
                    int countwasted = 0;
                    double timessaved = 0;
                    int countsaved = 0;
                    foreach (CastLog cl in clList)
                    {
                        if (cl.endActivation() == ParseEnum.Activation.CancelCancel)
                        {
                            countwasted++;
                            timeswasted += cl.getActDur();
                        }
                        if (cl.endActivation() == ParseEnum.Activation.CancelFire)
                        {
                            countsaved++;
                            if (cl.getActDur() < cl.getExpDur())
                            {
                                timessaved += cl.getExpDur() - cl.getActDur();
                            }
                        }
                    }
                    HTMLHelper.writeDamageDistTableSkill(sw, skill,log.getSkillData(), list_to_use, finalTotalDamage, casts, timeswasted/1000.0, -timessaved/1000.0);
                }
            }
            // non damaging stuff
            if (!toBoss)
            {
                foreach (int id in casting.Where(x => !usedIDs.Contains(x.getID())).Select(x => x.getID()).Distinct())
                {
                    SkillItem skill = s_list.FirstOrDefault(x => x.getID() == id);
                    if (skill != null)
                    {
                        List<CastLog> clList = casting.Where(x => x.getID() == id).ToList();
                        int casts = clList.Count;
                        double timeswasted = 0;
                        int countwasted = 0;
                        double timessaved = 0;
                        int countsaved = 0;
                        foreach (CastLog cl in clList)
                        {
                            if (cl.endActivation() == ParseEnum.Activation.CancelCancel)
                            {
                                countwasted++;
                                timeswasted += cl.getActDur();
                            }
                            if (cl.endActivation() == ParseEnum.Activation.CancelFire)
                            {
                                countsaved++;
                                if (cl.getActDur() < cl.getExpDur())
                                {
                                    timessaved += cl.getExpDur() - cl.getActDur();
                                }
                            }
                        }
                        HTMLHelper.writeDamageDistTableSkill(sw, skill,log.getSkillData(), new List<DamageLog>(), finalTotalDamage, casts, timeswasted/1000.0, -timessaved/1000.0);
                    }
                }
            }
        }

        /// <summary>
        /// Creates the damage distribution table for a given player
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="p">The player</param>
        private void CreateDMGDistTable(StreamWriter sw, Player p, bool toBoss, int phase_index)
        {
            PhaseData phase = log.getBoss().getPhases(log, settings.ParsePhases)[phase_index];
            List<CastLog> casting = p.getCastLogs(log, phase.getStart(), phase.getEnd());
            List<DamageLog> damageLogs = p.getJustPlayerDamageLogs(toBoss ? log.getBossData().getInstid() : 0,log, phase.getStart(), phase.getEnd());
            Statistics.FinalDPS dps = statistics.dps[p][phase_index];

            int totalDamage = toBoss ? dps.bossDamage : dps.allDamage;
            int finalTotalDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.getDamage()) : 0;
            if (totalDamage > 0)
            {
                string contribution = String.Format("{0:0.00}", 100.0 * finalTotalDamage / totalDamage);
                sw.Write("<div>" + p.getCharacter() + " did " + contribution + "% of its own total " + (toBoss ? "boss " : "") + "dps</div>");
            }
            string tabid = p.getInstid() + "_" + phase_index + (toBoss ? "_boss" : "");
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
                HTMLHelper.writeDamageDistTableHeader(sw);
                sw.Write("<tbody>");
                {
                    CreateDMGDistTableBody(sw, toBoss, casting, damageLogs, finalTotalDamage);
                }
                sw.Write("</tbody>");
                HTMLHelper.writeDamageDistTableFoot(sw, finalTotalDamage);
            }
            sw.Write("</table>");
        }

        /// <summary>
        /// Creates the damage distribution table for a the boss
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="p">The player</param>
        private void CreateDMGBossDistTable(StreamWriter sw, AbstractPlayer p, int phase_index)
        {
            PhaseData phase = log.getBoss().getPhases(log, settings.ParsePhases)[phase_index];
            List<CastLog> casting = p.getCastLogs(log, phase.getStart(), phase.getEnd());
            List<DamageLog> damageLogs = p.getJustPlayerDamageLogs(0, log, phase.getStart(), phase.getEnd());
            Statistics.FinalDPS dps = statistics.bossDps[phase_index];

            int totalDamage = dps.allDamage;
            int finalTotalDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.getDamage()) : 0;
            if (totalDamage > 0)
            {
                string contribution = String.Format("{0:0.00}", 100.0 * finalTotalDamage / totalDamage);
                sw.Write("<div>" + p.getCharacter() + " did " + contribution + "% of its own total " + "dps</div>");
            }
            string tabid = p.getInstid() + "_" + phase_index;
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
                HTMLHelper.writeDamageDistTableHeader(sw);
                sw.Write("<tbody>");
                {
                    CreateDMGDistTableBody(sw, false, casting, damageLogs, finalTotalDamage);
                }
                sw.Write("</tbody>");
                HTMLHelper.writeDamageDistTableFoot(sw, finalTotalDamage);
            }
            sw.Write("</table>");
        }

        /// <summary>
        /// Creates the damage distribution table for a given minion
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="p">Player, master of the minion</param>
        /// <param name="damageLogs">Damage logs to use</param>
        /// <param name="agent">The minion</param>
        private void CreateDMGDistTable(StreamWriter sw, Player p, Minions minions, bool toBoss, int phase_index)
        {
            Statistics.FinalDPS dps = statistics.dps[p][phase_index];

            int totalDamage = toBoss ? dps.bossDamage : dps.allDamage;
            string tabid = p.getInstid() + "_" + phase_index + "_" + minions.getInstid() + (toBoss ? "_boss" : "");
            PhaseData phase = log.getBoss().getPhases(log, settings.ParsePhases)[phase_index];
            List<CastLog> casting = minions.getCastLogs(log, phase.getStart(), phase.getEnd());
            List<DamageLog> damageLogs = minions.getDamageLogs(toBoss ? log.getBossData().getInstid() : 0, log, phase.getStart(), phase.getEnd());
            int finalTotalDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.getDamage()) : 0;
            if (totalDamage > 0)
            {
                string contribution = String.Format("{0:0.00}", 100.0 * finalTotalDamage / totalDamage);
                sw.Write("<div>" + minions.getCharacter() + " did " + contribution + "% of " + p.getCharacter() + "'s total " + (toBoss ? "boss " : "") + "dps</div>");
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
                HTMLHelper.writeDamageDistTableHeader(sw);
                sw.Write("<tbody>");
                {
                    CreateDMGDistTableBody(sw, toBoss, casting, damageLogs, finalTotalDamage);
                }
                sw.Write("</tbody>");
                HTMLHelper.writeDamageDistTableFoot(sw, finalTotalDamage);
            }
            sw.Write("</table>");
        }

        /// <summary>
        /// Creates the damage distribution table for a given boss minion
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="p">Player, master of the minion</param>
        /// <param name="damageLogs">Damage logs to use</param>
        /// <param name="agent">The minion</param>
        private void CreateDMGBossDistTable(StreamWriter sw, AbstractPlayer p, Minions minions, int phase_index)
        {
            Statistics.FinalDPS dps = statistics.bossDps[phase_index];

            int totalDamage =  dps.allDamage;
            string tabid = p.getInstid() + "_" + phase_index + "_" + minions.getInstid();
            PhaseData phase = log.getBoss().getPhases(log, settings.ParsePhases)[phase_index];
            List<CastLog> casting = minions.getCastLogs(log, phase.getStart(), phase.getEnd());
            List<DamageLog> damageLogs = minions.getDamageLogs(0, log, phase.getStart(), phase.getEnd());
            int finalTotalDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.getDamage()) : 0;
            if (totalDamage > 0)
            {
                string contribution = String.Format("{0:0.00}", 100.0 * finalTotalDamage / totalDamage);
                sw.Write("<div>" + minions.getCharacter() + " did " + contribution + "% of " + p.getCharacter() + "'s total " + "dps</div>");
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
                HTMLHelper.writeDamageDistTableHeader(sw);
                sw.Write("<tbody>");
                {
                    CreateDMGDistTableBody(sw, false, casting, damageLogs, finalTotalDamage);
                }
                sw.Write("</tbody>");
                HTMLHelper.writeDamageDistTableFoot(sw, finalTotalDamage);
            }
            sw.Write("</table>");
        }

        /// <summary>
        /// Create the damage taken distribution table for a given player
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="p">The player</param>
        private void CreateDMGTakenDistTable(StreamWriter sw, Player p, int phase_index)
        {
            PhaseData phase = log.getBoss().getPhases(log, settings.ParsePhases)[phase_index];
            List<DamageLog> damageLogs = p.getDamageTakenLogs(log, phase.getStart(), phase.getEnd());
            List<SkillItem> s_list = log.getSkillData().getSkillList();
            long finalTotalDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => (long)x.getDamage()) : 0;
            string pid = p.getInstid() + "_" + phase_index;
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
                    List<Boon> condiList = Boon.getCondiBoonList();
                    foreach (Boon condi in condiList)
                    {
                        long condiID = condi.getID();
                        int totaldamage = 0;
                        int mindamage = 0;
                        int avgdamage = 0;
                        int hits = 0;
                        int maxdamage = 0;
                        usedIDs.Add(condiID);
                        foreach (DamageLog dl in damageLogs.Where(x => x.getID() == condiID))
                        {
                            int curdmg = dl.getDamage();
                            totaldamage += curdmg;
                            if (0 == mindamage || curdmg < mindamage) { mindamage = curdmg; }
                            if (0 == maxdamage || curdmg > maxdamage) { maxdamage = curdmg; }
                            hits++;

                        }
                        avgdamage = (int)(totaldamage / (double)hits);
                        if (totaldamage > 0)
                        {
                            string condiName = condi.getName();// Boon.getCondiName(condiID);
                            sw.Write("<tr>");
                            {
                                sw.Write("<td align=\"left\"><img src=\"" + condi.getLink() + "\" alt=\"" + condiName + "\" title=\"" + condiID + "\" height=\"18\" width=\"18\">" + condiName + "</td>");
                                sw.Write("<td>" + totaldamage + "</td>");
                                sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)totaldamage / finalTotalDamage) + "%</td>");
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
                    foreach (int id in damageLogs.Where(x => !usedIDs.Contains(x.getID())).Select(x => x.getID()).Distinct())
                    {//foreach casted skill
                        SkillItem skill = s_list.FirstOrDefault(x => x.getID() == id);

                        int totaldamage = 0;
                        int mindamage = 0;
                        int avgdamage = 0;
                        int hits = 0;
                        int maxdamage = 0;
                        int crit = 0;
                        int flank = 0;
                        int glance = 0;
                        foreach (DamageLog dl in damageLogs.Where(x => x.getID() == id))
                        {
                            int curdmg = dl.getDamage();
                            totaldamage += curdmg;
                            if (0 == mindamage || curdmg < mindamage) { mindamage = curdmg; }
                            if (0 == maxdamage || curdmg > maxdamage) { maxdamage = curdmg; }
                            if (curdmg != 0) { hits++; };
                            ParseEnum.Result result = dl.getResult();
                            if (result == ParseEnum.Result.Crit) { crit++; } else if (result == ParseEnum.Result.Glance) { glance++; }
                            if (dl.isFlanking() == 1) { flank++; }
                        }
                        avgdamage = (int)(totaldamage / (double)hits);

                        if (skill != null)
                        {
                            if (totaldamage > 0 && skill.GetGW2APISkill() != null)
                            {
                                sw.Write("<tr>");
                                {
                                    sw.Write("<td align=\"left\"><img src=\"" + skill.GetGW2APISkill().icon + "\" alt=\"" + skill.getName() + "\" title=\"" + skill.getID() + "\" height=\"18\" width=\"18\">" + skill.getName() + "</td>");
                                    sw.Write("<td>" + totaldamage + "</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)totaldamage / finalTotalDamage) + "%</td>");
                                    sw.Write("<td>" + hits + "</td>");
                                    sw.Write("<td>" + mindamage + "</td>");
                                    sw.Write("<td>" + avgdamage + "</td>");
                                    sw.Write("<td>" + maxdamage + "</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)crit / hits) + "%</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)flank / hits) + "%</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)glance / hits) + "%</td>");
                                }
                                sw.Write("</tr>");
                            }
                            else if (totaldamage > 0)
                            {
                                sw.Write("<tr>");
                                {
                                    sw.Write("<td align=\"left\">" + skill.getName() + "</td>");
                                    sw.Write("<td>" + totaldamage + "</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)totaldamage / finalTotalDamage) + "%</td>");
                                    sw.Write("<td>" + hits + "</td>");
                                    sw.Write("<td>" + mindamage + "</td>");
                                    sw.Write("<td>" + avgdamage + "</td>");
                                    sw.Write("<td>" + maxdamage + "</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)crit / hits) + "%</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)flank / hits) + "%</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)glance / hits) + "%</td>");
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
        /// <param name="sw">Stream writer</param>
        private void CreateMechanicTable(StreamWriter sw, int phase_index)
        {
            Dictionary<string, List<Mechanic>> presMech = new Dictionary<string, List<Mechanic>>();
            //Dictionary<string, List<Mechanic>> presBossMech = new Dictionary<string, List<Mechanic>>();
            //Dictionary<string, List<Mechanic>> presMobMech = new Dictionary<string, List<Mechanic>>();
            Dictionary<string, List<Mechanic>> presEnemyMech = new Dictionary<string, List<Mechanic>>();
            PhaseData phase = log.getBoss().getPhases(log, settings.ParsePhases)[phase_index];

            //create list of enemys that had mechanics
            List<AbstractMasterPlayer> enemyList = new List<AbstractMasterPlayer>();
            enemyList.Add(log.getBoss());
            
            foreach (AbstractMasterPlayer p in log.getMechanicData().GetMDataLogs().Select(x => x.GetPlayer()).Distinct().ToList())
            {
                bool enemyNew = true;
                foreach (AbstractMasterPlayer en in enemyList)
                {
                    if (en.getInstid() == p.getInstid())
                    {
                        enemyNew = false;
                        break;
                    }
                    
                }
                if (enemyNew)
                {
                    enemyList.Add(p);
                }
              
            }
            
            foreach (AbstractMasterPlayer p in log.getPlayerList())
            {
                if (enemyList.Contains(p))
                {
                    enemyList.Remove(p);
                }
            }
            foreach (Mechanic item in log.getMechanicData().GetMechList(log.getBossData().getID()))
            {
                MechanicLog first_m_log = log.getMechanicData().GetMDataLogs().FirstOrDefault(x => x.GetSkill() == item.GetSkill());
                if (first_m_log != null)
                {
                    if (log.getPlayerList().Contains(first_m_log.GetPlayer()))//player mech
                    {
                        if (!presMech.ContainsKey(item.GetAltName()))
                        {
                            presMech[item.GetAltName()] = new List<Mechanic>();
                        }
                        presMech[item.GetAltName()].Add(item);
                    }
                    else 
                    {
                        if (!presEnemyMech.ContainsKey(item.GetAltName()))
                        {
                            presEnemyMech[item.GetAltName()] = new List<Mechanic>();
                        }
                        presEnemyMech[item.GetAltName()].Add(item);
                    }
                    
                    
                }
            }
            if (presMech.Count > 0)
            {
                sw.Write("<script>");
                {
                    sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                    {
                        sw.Write("var lazyTable = document.querySelector('#mech_table" + phase_index + "');" +

                        "if ('IntersectionObserver' in window) {" +
                            "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                                "entries.forEach(function(entry) {" +
                                    "if (entry.isIntersecting)" +
                                    "{" +
                                        "$(function () { $('#mech_table" + phase_index + "').DataTable({ \"order\": [[0, \"desc\"]]});});" +
                                        "lazyTableObserver.unobserve(entry.target);" +
                                    "}" +
                                "});" +
                            "});" +
                        "lazyTableObserver.observe(lazyTable);" +
                        "} else {" +
                            "$(function () { $('#mech_table" + phase_index + "').DataTable({ \"order\": [[0, \"desc\"]]});});" +
                        "}");
                    }
                    sw.Write("});");
                }
                sw.Write("</script>");
                sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"mech_table" + phase_index + "\">");
                {
                    sw.Write("<thead>");
                    {
                        sw.Write("<tr>");
                        {
                            sw.Write("<th>Player</th>");
                            foreach (string mechalt in presMech.Keys)
                            {
                                sw.Write("<th><span data-toggle=\"tooltip\" title=\""+presMech[mechalt].First().GetName() +"\">" + mechalt + "</span></th>");
                            }
                        }
                        sw.Write("</tr>");
                    }

                    sw.Write("</thead>");
                    sw.Write("<tbody>");
                    {
                        foreach (Player p in log.getPlayerList())
                        {
                            sw.Write("<tr>");
                            {
                                sw.Write("<td>" + p.getCharacter() + "</td>");
                                foreach (List<Mechanic> mechs in presMech.Values)
                                {
                                    int count = 0;
                                    long timeFilter = 0;
                                    int filterCount = 0;
                                    foreach (Mechanic mech in mechs)//Filtering for mechs named the same thing
                                    {
                                        List<MechanicLog> test = log.getMechanicData().GetMDataLogs().Where(x => x.GetSkill() == mech.GetSkill() && x.GetPlayer() == p && x.GetTime() >= Math.Round(phase.getStart() / 1000.0) && x.GetTime() <= Math.Round(phase.getEnd() / 1000.0)).ToList();
                                        count += test.Count;
                                        foreach (MechanicLog ml in test)
                                        {
                                            if (timeFilter != ml.GetTime())//Check for multihit
                                            {
                                                if (mech.GetICD() != 0)//ICD check
                                                {
                                                    if (ml.GetTime() - timeFilter > mech.GetICD())
                                                    {
                                                        timeFilter = ml.GetTime();
                                                        filterCount++;
                                                    }
                                                }
                                                else
                                                {
                                                    timeFilter = ml.GetTime();
                                                    filterCount++;
                                                }
                                                

                                            }
                                        }
                                    }
                                    if (filterCount > 0)
                                    {
                                        sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\""
                               + filterCount + " times (filtering multi hits)\">"+count+ "</span>" + "</td>");
                                       // sw.Write("<td>" + count + "</td>");
                                    }
                                    else
                                    {
                                        sw.Write("<td>" + count + "</td>");
                                    }
                                   
                                }
                            }
                            sw.Write(" </tr>");
                        }
                    }
                    sw.Write("</tbody>");
                }
                sw.Write("</table>");
            }
            if (presEnemyMech.Count > 0)
            {
                sw.Write("<script>");
                {
                    sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                    {
                        sw.Write("var lazyTable = document.querySelector('#mechEnemy_table" + phase_index + "');" +

                        "if ('IntersectionObserver' in window) {" +
                            "let lazyTableObserver = new IntersectionObserver(function(entries, observer) {" +
                                "entries.forEach(function(entry) {" +
                                    "if (entry.isIntersecting)" +
                                    "{" +
                                        "$(function () { $('#mechEnemy_table" + phase_index + "').DataTable({ \"order\": [[0, \"desc\"]]});});" +
                                        "lazyTableObserver.unobserve(entry.target);" +
                                    "}" +
                                "});" +
                            "});" +
                        "lazyTableObserver.observe(lazyTable);" +
                        "} else {" +
                            "$(function () { $('#mechEnemy_table" + phase_index + "').DataTable({ \"order\": [[0, \"desc\"]]});});" +
                        "}");
                    }
                    sw.Write("});");
                }
                sw.Write("</script>");
                sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"mechEnemy_table" + phase_index + "\">");
                {
                    sw.Write("<thead>");
                    {
                        sw.Write("<tr>");
                        {
                            sw.Write("<th>Enemy</th>");
                            foreach (string mechalt in presEnemyMech.Keys)
                            {
                                sw.Write("<th><span data-toggle=\"tooltip\" title=\"" + presEnemyMech[mechalt].First().GetName() + "\">" + mechalt + "</span></th>");
                            }
                        }
                        sw.Write("</tr>");
                    }

                    sw.Write("</thead>");
                    sw.Write("<tbody>");
                    {
                      

                        foreach (AbstractMasterPlayer p in enemyList)
                        {
                            sw.Write("<tr>");
                            {
                                sw.Write("<td>" + p.getCharacter() + "</td>");
                                foreach (List<Mechanic> mechs in presEnemyMech.Values)
                                {
                                    int count = 0;
                                    foreach (Mechanic mech in mechs)
                                    {
                                        List<MechanicLog> test = log.getMechanicData().GetMDataLogs().Where(x => x.GetSkill() == mech.GetSkill() && x.GetPlayer().getInstid() == p.getInstid() && x.GetTime() >= Math.Round(phase.getStart() / 1000.0) && x.GetTime() <= Math.Round(phase.getEnd() / 1000.0)).ToList();
                                        count += test.Count;
                                    }
                                    sw.Write("<td>" + count + "</td>");
                                }
                            }
                            sw.Write(" </tr>");
                        }
                        //sw.Write("<tr>");
                        //{
                        //    sw.Write("<td>" + log.getBoss().getCharacter() + "</td>");
                        //    foreach (List<Mechanic> mechs in presEnemyMech.Values)
                        //    {
                        //        int count = 0;
                        //        foreach (Mechanic mech in mechs)
                        //        {
                        //            List<MechanicLog> test = log.getMechanicData().GetMDataLogs().Where(x => x.GetSkill() == mech.GetSkill() && x.GetPlayer() == log.getBoss() && x.GetTime() >= phase.getStart() / 1000 && x.GetTime() <= phase.getEnd() / 1000).ToList();
                        //            count += test.Count;
                        //        }
                        //        sw.Write("<td>" + count + "</td>");
                        //    }
                        //}
                        //sw.Write(" </tr>");

                    }
                    sw.Write("</tbody>");
                }
                sw.Write("</table>");
            }
        }
        /// <summary>
        /// Creates the event list of the generation. Debbuging only
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateEventList(StreamWriter sw)
        {
            sw.Write("<ul class=\"list-group\">");
            {
                foreach (CombatItem c in log.getCombatData().getCombatList())
                {
                    if (c.isStateChange() != ParseEnum.StateChange.Normal)
                    {
                        AgentItem agent = log.getAgentData().GetAgent(c.getSrcAgent());
                        if (agent != null)
                        {
                            switch (c.isStateChange())
                            {
                                case ParseEnum.StateChange.EnterCombat:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " entered combat in" + c.getDstAgent() + "subgroup" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.ExitCombat:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " exited combat" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.ChangeUp:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " is now alive" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.ChangeDead:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " is now dead" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.ChangeDown:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " is now downed" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.Spawn:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " is now in logging range of POV player" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.Despawn:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " is now out of range of logging player" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.HealthUpdate:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " is at " + c.getDstAgent() / 100 + "% health" +
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
                                                   agent.getName() + " weapon swapped to " + c.getDstAgent() + "(0/1 water, 4/5 land)" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.MaxHealthUpdate:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " max health changed to  " + c.getDstAgent() +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case ParseEnum.StateChange.PointOfView:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " is recording log " +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                default:
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
                foreach (SkillItem skill in log.getSkillData().getSkillList())
                {
                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                  skill.getID() + " : " + skill.getName() +
                             "</li>");
                }
            }
            sw.Write("</ul>");
        }
        /// <summary>
        /// Creates the condition uptime table of the given boss
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="boss">The boss</param>
        private void CreateCondiUptimeTable(StreamWriter sw, Boss boss, int phase_index)
        {
            List<PhaseData> phases = log.getBoss().getPhases(log, settings.ParsePhases);
            long fight_duration = phases[phase_index].getDuration();
            Dictionary<long, Statistics.FinalBossBoon> conditions = statistics.bossConditions[phase_index];
            bool hasBoons = false;
            foreach (Boon boon in Boon.getBoonList())
            {
                if (boon.getName() == "Retaliation")
                {
                    continue;
                }
                if (conditions[boon.getID()].uptime > 0.0)
                {
                    hasBoons = true;
                    break;
                }
            }
            List<Boon> boon_to_track = Boon.getCondiBoonList();
            boon_to_track.AddRange(Boon.getBoonList());
            Dictionary<long, long> condiPresence = boss.getCondiPresence(log, phases, boon_to_track, phase_index);
            Dictionary<long, long> boonPresence = boss.getBoonPresence(log, phases, boon_to_track, phase_index);
            double avg_condis = 0.0;
            foreach (long duration in condiPresence.Values)
            {
                avg_condis += duration;
            }
            avg_condis /= fight_duration;
            //Generate Boon table------------------------------------------------------------------------------------------------
            sw.Write("<h3 align=\"center\"> Condition Uptime </h3>");
            sw.Write("<script> $(function () { $('#condi_table" + phase_index + "').DataTable({ \"order\": [[3, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact mb-3\"  cellspacing=\"0\" width=\"100%\" id=\"condi_table" + phase_index + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Name</th>");
                        foreach (Boon boon in Boon.getCondiBoonList())
                        {
                            if (hasBoons && boon.getName() == "Retaliation")
                            {
                                continue;
                            }
                            sw.Write("<th>" + "<img src=\"" + boon.getLink() + " \" alt=\"" + boon.getName() + "\" title =\" " + boon.getName() + "\" height=\"18\" width=\"18\" >" + "</th>");
                        }
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</thead>");
                sw.Write("<tbody>");
                {
                    sw.Write("<tr>");
                    {
                        
                        sw.Write("<td style=\"width: 275px;\" data-toggle=\"tooltip\" title=\"Average number of conditions: " + Math.Round(avg_condis, 1) + "\">" + boss.getCharacter() + " </td>");
                        foreach (Boon boon in Boon.getCondiBoonList())
                        {
                            if (hasBoons && boon.getName() == "Retaliation")
                            {
                                continue;
                            }
                            if (conditions[boon.getID()].boonType == Boon.BoonType.Duration)
                            {
                                sw.Write("<td>" + conditions[boon.getID()].uptime + "%</td>");
                            }
                            else
                            {
                                if (condiPresence.TryGetValue(boon.getID(), out long presenceTime))
                                {
                                    string tooltip = "uptime: " + Math.Round(100.0 * presenceTime / fight_duration, 1) + "%";
                                    sw.Write("<td data-toggle=\"tooltip\" title=\"" + tooltip + "\">" + conditions[boon.getID()].uptime + " </td>");
                                }
                                else
                                {
                                   sw.Write("<td>" + conditions[boon.getID()].uptime + "</td>");
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
                sw.Write("<h3 align=\"center\"> Boon Uptime </h3>");
                sw.Write("<script> $(function () { $('#boss_boon_table" + phase_index + "').DataTable({ \"order\": [[3, \"desc\"]]});});</script>");
                sw.Write("<table class=\"display table table-striped table-hover compact mb-3\"  cellspacing=\"0\" width=\"100%\" id=\"boss_boon_table" + phase_index + "\">");
                {
                    sw.Write("<thead>");
                    {
                        sw.Write("<tr>");
                        {
                            sw.Write("<th>Name</th>");
                            foreach (Boon boon in Boon.getBoonList())
                            {
                                sw.Write("<th>" + "<img src=\"" + boon.getLink() + " \" alt=\"" + boon.getName() + "\" title =\" " + boon.getName() + "\" height=\"18\" width=\"18\" >" + "</th>");
                            }
                        }
                        sw.Write("</tr>");
                    }
                    sw.Write("</thead>");
                    sw.Write("<tbody>");
                    {
                        sw.Write("<tr>");
                        {
                            sw.Write("<td style=\"width: 275px;\">" + boss.getCharacter() + " </td>");
                            foreach (Boon boon in Boon.getBoonList())
                            {
                                if (conditions[boon.getID()].boonType == Boon.BoonType.Duration)
                                {
                                    sw.Write("<td>" + conditions[boon.getID()].uptime + "%</td>");
                                }
                                else
                                {
                                    if (boonPresence.TryGetValue(boon.getID(), out long presenceTime))
                                    {
                                        string tooltip = "uptime: " + Math.Round(100.0 * presenceTime / fight_duration, 1) + "%";
                                        sw.Write("<td data-toggle=\"tooltip\" title=\"" + tooltip + "\">" + conditions[boon.getID()].uptime + " </td>");
                                    }
                                    else
                                    {
                                        sw.Write("<td>" + conditions[boon.getID()].uptime + "</td>");
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
            sw.Write("<script> $(function () { $('#condigen_table" + phase_index + "').DataTable({ \"order\": [[3, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"condigen_table" + phase_index + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Sub</th>");
                        sw.Write("<th></th>");
                        sw.Write("<th>Name</th>");
                        foreach (Boon boon in Boon.getCondiBoonList())
                        {
                            if (boon.getName() == "Retaliation")
                            {
                                continue;
                            }
                            sw.Write("<th>" + "<img src=\"" + boon.getLink() + " \" alt=\"" + boon.getName() + "\" title =\" " + boon.getName() + "\" height=\"18\" width=\"18\" >" + "</th>");
                        }
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</thead>");
                sw.Write("<tbody>");
                {
                    foreach (Player player in log.getPlayerList())
                    {
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + HTMLHelper.GetLink(player.getProf().ToString()) + "\" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "<span style=\"display:none\">" + player.getProf() + "</span>" + "</td>");
                            sw.Write("<td>" + player.getCharacter() + " </td>");
                            foreach (Boon boon in Boon.getCondiBoonList())
                            {
                                if (boon.getName() == "Retaliation")
                                {
                                    continue;
                                }
                                if (conditions[boon.getID()].boonType == Boon.BoonType.Duration)
                                {
                                    sw.Write("<td>" + conditions[boon.getID()].generated[player] + "%</td>");
                                }
                                else
                                {
                                    sw.Write("<td>" + conditions[boon.getID()].generated[player] + " </td>");
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
        /// <param name="sw">Stream writer</param>
        private void CreateBossSummary(StreamWriter sw, int phase_index)
        {
            //generate Player list Graphs
            List<PhaseData> phases = log.getBoss().getPhases(log, settings.ParsePhases);
            PhaseData phase = phases[phase_index];
            List<CastLog> casting = log.getBoss().getCastLogsActDur(log, phase.getStart(), phase.getEnd());
            List<SkillItem> s_list = log.getSkillData().getSkillList();
            string charname = log.getBoss().getCharacter();
            string pid = log.getBoss().getInstid() + "_" + phase_index;
            sw.Write("<h1 align=\"center\"> " + charname + "</h1>");
            sw.Write("<ul class=\"nav nav-tabs\">");
            {
                sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#home" + pid + "\">" + log.getBoss().getCharacter() + "</a></li>");
                //foreach pet loop here
                foreach (KeyValuePair<string, Minions> pair in log.getBoss().getMinions(log))
                {
                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#minion" + pid + "_" + pair.Value.getInstid() + "\">" + pair.Key + "</a></li>");
                }
            }
            sw.Write("</ul>");
            //condi stats tab
            sw.Write("<div id=\"myTabContent\" class=\"tab-content\"><div class=\"tab-pane fade show active\" id=\"home" + pid + "\">");
            {
                CreateCondiUptimeTable(sw, log.getBoss(), phase_index);
                sw.Write("<div id=\"Graph" + pid + "\" style=\"height: 800px;width:1000px; display:inline-block \"></div>");
                sw.Write("<script>");
                {
                    sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                    {
                        sw.Write("var data = [");
                        {
                            if (settings.PlayerRot)//Display rotation
                            {

                                foreach (CastLog cl in casting)
                                {
                                    HTMLHelper.writeCastingItem(sw, cl, log.getSkillData(), phase.getStart(), phase.getEnd());
                                }
                            }
                            //============================================
                            List<Boon> parseBoonsList = new List<Boon>();
                            //Condis
                            parseBoonsList.AddRange(Boon.getCondiBoonList());
                            //Every buffs and boons
                            parseBoonsList.AddRange(Boon.getAllBuffList());
                            Dictionary<long, BoonsGraphModel> boonGraphData = log.getBoss().getBoonGraphs(log, phases, parseBoonsList);
                            foreach (BoonsGraphModel bgm in boonGraphData.Values.Reverse().Where(x => x.getBoonName() != "Number of Boons"))
                            {
                                sw.Write("{");
                                {
                                    HTMLHelper.writeBoonGraph(sw, bgm, phase.getStart(), phase.getEnd());
                                }
                                sw.Write(" },");

                            }
                            //int maxDPS = 0;
                            if (settings.DPSGraphTotals)
                            {//show total dps plot
                                List<Point> playertotaldpsgraphdata = GraphHelper.getTotalDPSGraph(log, log.getBoss(), phase_index, GraphHelper.GraphMode.Full);
                                sw.Write("{");
                                {
                                    //Adding dps axis
                                    HTMLHelper.writeDPSGraph(sw, "Total DPS", playertotaldpsgraphdata, log.getBoss());
                                }
                                sw.Write("},");
                            }
                            sw.Write("{");
                            HTMLHelper.writeBossHealthGraph(sw, GraphHelper.getTotalDPSGraph(log, log.getBoss(), phase_index, GraphHelper.GraphMode.Full).Max(x => x.Y), phase.getStart(), phase.getEnd(), log.getBossData(), "y3");
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
                                if (settings.PlayerRotIcons)//Display rotation
                                {
                                    int castCount = 0;
                                    foreach (CastLog cl in casting)
                                    {
                                        HTMLHelper.writeCastingItemIcon(sw, cl, log.getSkillData(), phase.getStart(), castCount == casting.Count - 1);
                                        castCount++;
                                    }
                                }
                            }
                            sw.Write("],");
                            if (settings.LightTheme)
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
                CreateDMGBossDistTable(sw, log.getBoss(), phase_index);
                sw.Write("</div>");
                foreach (KeyValuePair<string, Minions> pair in log.getBoss().getMinions(log))
                {
                    sw.Write("<div class=\"tab-pane fade \" id=\"minion" + pid + "_" + pair.Value.getInstid() + "\">");
                    {
                        CreateDMGBossDistTable(sw, log.getBoss(), pair.Value, phase_index);
                    }
                    sw.Write("</div>");
                }
            }
            sw.Write("</div>");
        }
        /// <summary>
        /// To define
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateEstimateTabs(StreamWriter sw, int phase_index)
        {
            sw.Write("<ul class=\"nav nav-tabs\">");
            {
                sw.Write("<li class=\"nav-item\">" +
                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#est_role" + phase_index + "\">Roles</a>" +
                        "</li>" +

                        "<li class=\"nav-item\">" +
                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#est_cc" + phase_index + "\">CC</a>" +
                        "</li>" +
                         "<li class=\"nav-item\">" +
                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#est" + phase_index + "\">Maybe more</a>" +
                        "</li>");
            }
            sw.Write("</ul>");
            sw.Write("<div id=\"myTabContent\" class=\"tab-content\">");
            {
                sw.Write("<div class=\"tab-pane fade show active\" id=\"est_role" + phase_index + "\">");
                {
                    //Use cards
                }
                sw.Write("</div>");
                sw.Write("<div class=\"tab-pane fade show active\" id=\"est_cc" + phase_index + "\">");
                {
                }
                sw.Write("</div>");
                sw.Write("<div class=\"tab-pane fade show active\" id=\"est" + phase_index + "\">");
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
            CombatReplayMap map = log.getBoss().getCombatMap(log);
            Tuple<int, int> canvasSize = map.getPixelMapSize();
            HTMLHelper.writeCombatReplayInterface(sw, canvasSize, log);
            HTMLHelper.writeCombatReplayScript(sw, log, canvasSize, map, settings.PollingRate);
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
                if (!settings.LightTheme)
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
                if (log.getBoss().getCombatReplay() != null)
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
        public void CreateHTML(StreamWriter sw)
        {
            double fight_duration = (log.getBossData().getAwareDuration()) / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fight_duration);
            string durationString = duration.Minutes + "m " + duration.Seconds + "s " + duration.Milliseconds + "ms";
            if (duration.Hours > 0)
            {
                durationString = duration.Hours + "h " + durationString;
            }
            string bossname = FilterStringChars(log.getBossData().getName());
            List<PhaseData> phases = log.getBoss().getPhases(log, settings.ParsePhases);
            // HTML STARTS
            sw.Write("<!DOCTYPE html><html lang=\"en\">");
            {
                sw.Write("<head>");
                {
                    sw.Write("<meta charset=\"utf-8\">");

                    if (!settings.LightTheme)
                    {
                        sw.Write(
                            "<link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/bootswatch/4.1.1/darkly/bootstrap.min.css\"  crossorigin=\"anonymous\">"
                        );
                    }
                    else
                    {
                        sw.Write(
                            "<link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/bootswatch/4.1.1/cosmo/bootstrap.min.css\"  crossorigin=\"anonymous\">"
                        );
                    }

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
                    if (this.settings.LargeRotIcons)
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
                        sw.Write("<p> Time Start: " + log.getLogData().getLogStart() + " | Time End: " + log.getLogData().getLogEnd() + " </p> ");
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
                                                    sw.Write("<img src=\"" + HTMLHelper.GetLink(log.getBossData().getID() + "-icon") + "\"alt=\"" + bossname + "-icon" + "\" style=\"height: 120px; width: 120px;\" >");
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div>");
                                                {
                                                    sw.Write("<div class=\"progress\" style=\"width: 100 %; height: 20px;\">");
                                                    {
                                                        if (log.getLogData().getBosskill())
                                                        {
                                                            string tp = log.getBossData().getHealth().ToString() + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-success\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:100%; ;\" aria-valuenow=\"100\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");
                                                        }
                                                        else
                                                        {
                                                            double finalPercent = 0;
                                                            if (log.getBossData().getHealthOverTime().Count > 0)
                                                            {
                                                                finalPercent = 100.0 - log.getBossData().getHealthOverTime()[log.getBossData().getHealthOverTime().Count - 1].Y * 0.01;
                                                            }
                                                            string tp = Math.Round(log.getBossData().getHealth() * finalPercent / 100.0) + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-success\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:" + finalPercent + "%;\" aria-valuenow=\"" + finalPercent + "\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");
                                                            tp = Math.Round(log.getBossData().getHealth() * (100.0 - finalPercent) / 100.0) + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-danger\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:" + (100.0 - finalPercent) + "%;\" aria-valuenow=\"" + (100.0 - finalPercent) + "\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");

                                                        }
                                                    }
                                                    sw.Write("</div>");
                                                    sw.Write("<p class=\"small\" style=\"text-align:center; color: "+ (settings.LightTheme ? "#000" : "#FFF") +";\">" + log.getBossData().getHealth().ToString() + " Health</p>");
                                                    if (log.getLogData().getBosskill())
                                                    {
                                                        sw.Write("<p class='text text-success'> Result: Success</p>");
                                                    }
                                                    else
                                                    {
                                                        sw.Write("<p class='text text-warning'> Result: Fail</p>");
                                                    }
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
                        if (phases.Count > 1 || log.getBoss().getCombatReplay() != null)
                        {
                            sw.Write("<ul class=\"nav nav-tabs\">");
                            {
                                for (int i = 0; i < phases.Count; i++)
                                {
                                    if (phases[i].getDuration() == 0)
                                        continue;
                                    string active = (i > 0 ? "" : "active");
                                    string name = phases[i].getName();
                                    sw.Write("<li  class=\"nav-item\">" +
                                            "<a class=\"nav-link " + active + "\" data-toggle=\"tab\" href=\"#phase" + i + "\">" +
                                                "<span data-toggle=\"tooltip\" title=\"" + phases[i].getDuration("s") + " seconds\">" + name + "</span>" +
                                            "</a>" +
                                        "</li>");
                                }
                                if (log.getBoss().getCombatReplay() != null)
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

                                if (phases[i].getDuration() == 0)
                                    continue;
                                sw.Write("<div class=\"tab-pane fade " + active + "\" id=\"phase" + i + "\">");
                                {
                                    if (phases.Count > 1)
                                        sw.Write("<h2 align=\"center\">"+ phases[i].getName()+ "</h2>");
                                    string Html_playerDropdown = "";
                                    foreach (Player p in log.getPlayerList())
                                    {
                                        string charname = p.getCharacter();
                                        Html_playerDropdown += "<a class=\"dropdown-item\"  data-toggle=\"tab\" href=\"#" + p.getInstid() + "_" + i + "\">" + charname +
                                            "<img src=\"" + HTMLHelper.GetLink(p.getProf().ToString()) + "\" alt=\"" + p.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</a>";
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
                                                        Html_playerDropdown +
                                                    "</div>" +
                                                "</li>");
                                        if (settings.BossSummary)
                                        {
                                            sw.Write("<li class=\"nav-item\">" +
                                                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#bossSummary" + i + "\">Boss</a>" +
                                                        "</li>");
                                        }
                                        if (settings.EventList)
                                        {
                                            sw.Write("<li class=\"nav-item\">" +
                                                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#eventList" + i + "\">Event List</a>" +
                                                        "</li>");
                                        }
                                        if (settings.ShowEstimates)
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
                                                    CreateDPSTable(sw, i);
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div class=\"tab-pane fade \" id=\"offStats" + i + "\">");
                                                {
                                                    sw.Write("<ul class=\"nav nav-tabs\">" +
                                                       "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#dpsStatsBoss" + i + "\">Boss</a></li>" +
                                                       "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#dpsStatsAll" + i + "\">All</a></li>" +
                                                     "</ul>");
                                                    sw.Write("<div id=\"subtabcontent" + "\" class=\"tab-content\">");
                                                    {
                                                        sw.Write("<div class=\"tab-pane fade show active \" id=\"dpsStatsBoss" + i + "\">");
                                                        {
                                                            //HTML_dmgstatsBoss
                                                            CreateDMGStatsBossTable(sw, i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade \" id=\"dpsStatsAll" + i + "\">");
                                                        {
                                                            // HTML_dmgstats 
                                                            CreateDMGStatsTable(sw, i);
                                                        }
                                                        sw.Write("</div>");
                                                    }
                                                    sw.Write("</div>");
                                                }
                                                sw.Write("</div>");
                                               


                                                sw.Write("<div class=\"tab-pane fade \" id=\"defStats" + i + "\">");
                                                {
                                                    // def stats
                                                    CreateDefTable(sw, i);
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div class=\"tab-pane fade\" id=\"healStats" + i + "\">");
                                                {
                                                    //  HTML_supstats
                                                    CreateSupTable(sw, i);
                                                }
                                                sw.Write("</div>");
                                            }
                                            sw.Write("</div>");

                                        }
                                        sw.Write("</div>");

                                        sw.Write("<div class=\"tab-pane fade\" id=\"dmgGraph" + i + "\">");
                                        {
                                            //Html_dpsGraph
                                            sw.Write("<ul class=\"nav nav-tabs\">");
                                            {
                                                if (settings.Show10s || settings.Show30s)
                                                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#Full" + i + "\">Full</a></li>");
                                                if (settings.Show10s)
                                                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#10s" + i + "\">10s</a></li>");
                                                if (settings.Show30s)
                                                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#30s" + i + "\">30s</a></li>");
                                            }
                                            sw.Write("</ul>");
                                            sw.Write("<div id=\"dpsSubTab" + i + "\" class=\"tab-content\">");
                                            {
                                                sw.Write("<div class=\"tab-pane fade show active  \" id=\"Full" + i + "\">");
                                                {
                                                    CreateDPSGraph(sw, i, GraphHelper.GraphMode.Full);
                                                }
                                                sw.Write("</div>");
                                                if (settings.Show10s)
                                                {
                                                    sw.Write("<div class=\"tab-pane fade \" id=\"10s" + i + "\">");
                                                    {
                                                        CreateDPSGraph(sw, i, GraphHelper.GraphMode.s10);
                                                    }
                                                    sw.Write("</div>");
                                                }
                                                if (settings.Show30s)
                                                {
                                                    sw.Write("<div class=\"tab-pane fade \" id=\"30s" + i + "\">");
                                                    {
                                                        CreateDPSGraph(sw, i, GraphHelper.GraphMode.s30);
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
                                                            // Html_boons
                                                            CreateUptimeTable(sw, statistics.present_boons, "boons_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenSelf" + i + "\">");
                                                        {
                                                            //Html_boonGenSelf
                                                            sw.Write("<p> Boons generated by a character for themselves</p>");
                                                            CreateGenSelfTable(sw, statistics.present_boons, "boongenself_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Boons generated by a character for their groupmates</p>");
                                                            // Html_boonGenGroup
                                                            CreateGenGroupTable(sw, statistics.present_boons, "boongengroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenOGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Boons generated by a character for any subgroup that is not their own</p>");
                                                            // Html_boonGenOGroup
                                                            CreateGenOGroupTable(sw, statistics.present_boons, "boongenogroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenSquad" + i + "\">");
                                                        {
                                                            sw.Write("<p> Boons generated by a character for their squadmates</p>");
                                                            //  Html_boonGenSquad
                                                            CreateGenSquadTable(sw, statistics.present_boons, "boongensquad_table", i);
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
                                                            CreateUptimeTable(sw, statistics.present_offbuffs, "offensive_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenSelf" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs generated by a character for themselves</p>");
                                                            CreateGenSelfTable(sw, statistics.present_offbuffs, "offensivegenself_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs generated by a character for their groupmates</p>");
                                                            CreateGenGroupTable(sw, statistics.present_offbuffs, "offensivegengroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenOGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs generated by a character for any subgroup that is not their own</p>");
                                                            CreateGenOGroupTable(sw, statistics.present_offbuffs, "offensivegenogroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenSquad" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs generated by a character for their squadmates</p>");
                                                            CreateGenSquadTable(sw, statistics.present_offbuffs, "offensivegensquad_table", i);
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
                                                            CreateUptimeTable(sw, statistics.present_defbuffs, "defensive_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenSelf" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs generated by a character for themselves</p>");
                                                            CreateGenSelfTable(sw, statistics.present_defbuffs, "defensivegenself_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs generated by a character for their groupmates</p>");
                                                            CreateGenGroupTable(sw, statistics.present_defbuffs, "defensivegengroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenOGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs generated by a character for any subgroup that is not their own</p>");
                                                            CreateGenOGroupTable(sw, statistics.present_defbuffs, "defensivegenogroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenSquad" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs generated by a character for their squadmates</p>");
                                                            CreateGenSquadTable(sw, statistics.present_defbuffs, "defensivegensquad_table", i);
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
                                            CreateMechanicTable(sw, i);
                                        }
                                        sw.Write("</div>");
                                        //boss summary
                                        if (settings.BossSummary)
                                        {
                                            sw.Write("<div class=\"tab-pane fade\" id=\"bossSummary" + i + "\">");
                                            {
                                                CreateBossSummary(sw, i);
                                            }
                                            sw.Write("</div>");
                                        }
                                        //event list
                                        if (settings.EventList && i == 0)
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
                                        if (settings.ShowEstimates)
                                        {
                                            sw.Write("<div class=\"tab-pane fade\" id=\"estimates" + i + "\">");
                                            {
                                                CreateEstimateTabs(sw, i);
                                            }
                                            sw.Write("</div>");
                                        }
                                        //Html_playertabs
                                        CreatePlayerTab(sw, i);
                                    }
                                    sw.Write("</div>");
                                }
                                sw.Write("</div>");

                            }
                            if (log.getBoss().getCombatReplay() != null)
                            {
                                sw.Write("<div class=\"tab-pane fade\" id=\"replay\">");
                                {
                                    CreateReplayTable(sw);
                                }
                            }
                        }
                        sw.Write("</div>");
                        sw.Write("<p style=\"margin-top:10px;\"> ARC:" + log.getLogData().getBuildVersion().ToString() + " | Bossid " + log.getBossData().getID().ToString() + "| EI Version: " +Application.ProductVersion + " </p> ");
                       
                        sw.Write("<p style=\"margin-top:-15px;\">File recorded by: " + log.getLogData().getPOV().Split(':')[0] + "</p>");
                    }
                    sw.Write("</div>");
                }
                sw.Write("</body>");
                sw.Write("<script> $(document).ready(function(){$('[data-toggle=\"tooltip\"]').tooltip(); });</script >");
            }
            //end
            sw.Write("</html>");
            return;
        }
        public void CreateSoloHTML(StreamWriter sw)
        {
            List<PhaseData> phases = log.getBoss().getPhases(log, settings.ParsePhases);
            double fight_duration = (log.getBossData().getAwareDuration()) / 1000.0;
            Player p = log.getPlayerList()[0];
            List<CastLog> casting = p.getCastLogsActDur(log, 0, log.getBossData().getAwareDuration());
            List<SkillItem> s_list = log.getSkillData().getSkillList();

            CreateDPSTable(sw, 0);
            CreateDMGStatsTable(sw, 0);
            CreateDefTable(sw, 0);
            CreateSupTable(sw, 0);
            // CreateDPSGraph(sw);
            sw.Write("<div id=\"Graph" + p.getInstid() + "\" style=\"height: 800px;width:1000px; display:inline-block \"></div>");
            sw.Write("<script>");
            {
                sw.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
                {
                    sw.Write("var data = [");
                    {
                        if (settings.PlayerRot)//Display rotation
                        {

                            foreach (CastLog cl in casting)
                            {
                                HTMLHelper.writeCastingItem(sw, cl, log.getSkillData(), 0, log.getBossData().getAwareDuration());
                            }
                        }
                        if (statistics.present_boons.Count > 0)
                        {
                            List<Boon> parseBoonsList = new List<Boon>();
                            parseBoonsList.AddRange(statistics.present_boons);
                            parseBoonsList.AddRange(statistics.present_offbuffs);
                            parseBoonsList.AddRange(statistics.present_defbuffs);
                            if (statistics.present_personnal.ContainsKey(p.getInstid()))
                            {
                                parseBoonsList.AddRange(statistics.present_personnal[p.getInstid()]);
                            }
                            Dictionary<long, BoonsGraphModel> boonGraphData = p.getBoonGraphs(log, phases, parseBoonsList);
                            foreach (BoonsGraphModel bgm in boonGraphData.Values.Reverse().Where(x => x.getBoonName() != "Number of Conditions"))
                            {
                                sw.Write("{");
                                {
                                    HTMLHelper.writeBoonGraph(sw, bgm, 0, log.getBossData().getAwareDuration());
                                }
                                sw.Write(" },");
                            }
                        }
                        int maxDPS = 0;
                        if (settings.DPSGraphTotals)
                        {//show total dps plot
                            List<Point> playertotaldpsgraphdata = GraphHelper.getTotalDPSGraph(log, p, 0, GraphHelper.GraphMode.Full);
                            sw.Write("{");
                            {
                                HTMLHelper.writeDPSGraph(sw, "Total DPS", playertotaldpsgraphdata, p);
                            }
                            sw.Write("},");
                        }
                        //Adding dps axis
                        List<Point> playerbossdpsgraphdata = GraphHelper.getBossDPSGraph(log, p, 0, GraphHelper.GraphMode.Full);
                        sw.Write("{");
                        {
                            HTMLHelper.writeDPSGraph(sw, "Boss DPS", playerbossdpsgraphdata, p);
                        }
                        maxDPS = Math.Max(maxDPS, playerbossdpsgraphdata.Max(x => x.Y));
                        sw.Write("},");
                        sw.Write("{");
                        HTMLHelper.writeBossHealthGraph(sw, maxDPS, 0, log.getBossData().getAwareDuration(), log.getBossData(), "y3");
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
                                 "yaxis2: { title: 'Boons', domain: [0.11, 0.50], fixedrange: true }," +
                                 "yaxis3: { title: 'DPS', domain: [0.51, 1] },"
                         );
                        sw.Write("images: [");
                        {
                            if (settings.PlayerRotIcons)//Display rotation
                            {
                                int castCount = 0;
                                foreach (CastLog cl in casting)
                                {
                                    HTMLHelper.writeCastingItemIcon(sw, cl, log.getSkillData(), 0, castCount == casting.Count - 1);
                                    castCount++;
                                }
                            }
                        }
                        sw.Write("],");
                        if (settings.LightTheme)
                        {
                            sw.Write("font: { color: '#000000' }," +
                                     "paper_bgcolor: 'rgba(255,255,255,0)'," +
                                     "plot_bgcolor: 'rgba(255,255,255,0)'");
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
                                "var lazyplot = document.querySelector('#Graph" + p.getInstid() + "');" +

                                "if ('IntersectionObserver' in window) {" +
                                    "let lazyPlotObserver = new IntersectionObserver(function(entries, observer) {" +
                                        "entries.forEach(function(entry) {" +
                                            "if (entry.isIntersecting)" +
                                            "{" +
                                                "Plotly.newPlot('Graph" + p.getInstid() + "', data, layout);" +
                                                "lazyPlotObserver.unobserve(entry.target);" +
                                            "}" +
                                        "});" +
                                    "});" +
                                    "lazyPlotObserver.observe(lazyplot);" +
                                "} else {" +
                                    "Plotly.newPlot('Graph" + p.getInstid() + "', data, layout);" +
                                "}");
                }
                sw.Write("});");
            }
            sw.Write("</script> ");
            sw.Write("<ul class=\"nav nav-tabs\">");
            {
                sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#distTabBoss" + p.getInstid() + "\">" + "Boss" + "</a></li>");
                sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#distTabAll" + p.getInstid() + "\">" + "All" + "</a></li>");
            }
            sw.Write("</ul>");
            sw.Write("<div class=\"tab-content\">");
            {
                sw.Write("<div class=\"tab-pane fade show active\" id=\"distTabBoss" + p.getInstid() + "\">");
                {
                    CreateDMGDistTable(sw, p, true, 0);
                }
                sw.Write("</div>");
                sw.Write("<div class=\"tab-pane fade\" id=\"distTabAll" + p.getInstid() + "\">");
                {
                    CreateDMGDistTable(sw, p, false, 0);
                }
                sw.Write("</div>");
            }
            sw.Write("</div>");
        }
    }
}
