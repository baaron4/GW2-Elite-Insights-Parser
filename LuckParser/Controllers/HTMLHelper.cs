using LuckParser.Models;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using LuckParser.Models.DataModels;

namespace LuckParser.Controllers
{
    class HTMLHelper
    {
        public static SettingsContainer settings;

        public static void writeCastingItem(StreamWriter sw, CastLog cl, SkillData s_list, long start, long end)
        {
            string skillName = "";
            GW2APISkill skill = null;
            if (s_list.FirstOrDefault(x => x.getID() == cl.getID()) != null)
            {
                skill = s_list.FirstOrDefault(x => x.getID() == cl.getID()).GetGW2APISkill();
            }
            if (skill == null)
            {
                skillName = s_list.getName(cl.getID());
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
                if (cl.getID() == -5)
                {
                    sw.Write("y: ['1'],");
                }
                else
                {
                    sw.Write("y: ['1.5'],");
                }
              

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
                        if (cl.endActivation() == ParseEnum.Activation.CancelFire)
                        {
                            sw.Write("color: 'rgb(40,40,220)',");
                        }
                        else if (cl.endActivation() == ParseEnum.Activation.CancelCancel)
                        {
                            sw.Write("color: 'rgb(220,40,40)',");
                        }
                        else if (cl.endActivation() == ParseEnum.Activation.Reset)
                        {
                            sw.Write("color: 'rgb(40,220,40)',");
                        }
                        else
                        {
                            sw.Write("color: 'rgb(220,220,0)',");
                        }
                    sw.Write("width: '5',");
                    sw.Write("line:{");
                    {
                            if (cl.startActivation() == ParseEnum.Activation.Normal)
                            {
                                sw.Write("color: 'rgb(20,20,20)',");
                            }
                            else if (cl.startActivation() == ParseEnum.Activation.Quickness)
                            {
                                sw.Write("color: 'rgb(220,40,220)',");
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

        public static void writeCastingItemIcon(StreamWriter sw, CastLog cl, SkillData s_list, long start, bool last)
        {
            string skillIcon = "";
            GW2APISkill skill = null;
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
                    skillName = s_list.getName(cl.getID());
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
                        sw.Write("<th width=\"50px\">" + "<img src=\"" + boon.getLink() + "\" alt=\"" + boon.getName() + "\" title =\" " + boon.getName() + "\" height=\"18\" width=\"18\" >" + "</th>");
                    }
                }
                sw.Write("</tr> ");
            }
            sw.Write("</thead>");

        }

        public static void writeBoonGenTableBody(StreamWriter sw, Player player, List<Boon> list_to_use, Dictionary<long, string> boonArray)
        {
            sw.Write("<tr>");
            {
                sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                sw.Write("<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + "\" alt=\"" + player.getProf().ToString() + "\" height=\"20\" width=\"20\" >" + "<span style=\"display:none\">" + player.getProf() + "</span>" + "</td>");
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

        public static void writeDamageDistTableCondi(StreamWriter sw, HashSet<long> usedIDs, List<DamageLog> damageLogs, int finalTotalDamage)
        {
            foreach (Boon condi in Boon.getCondiBoonList())
            {
                int totaldamage = 0;
                int mindamage = 0;
                int avgdamage = 0;
                int hits = 0;
                int maxdamage = 0;
                long condiID = condi.getID();
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
                if (totaldamage != 0)
                {
                    string condiName = condi.getName();// Boon.getCondiName(condiID);
                    sw.Write("<tr class=\"condi\">");
                    {
                        sw.Write("<td align=\"left\"><img src=\"" + condi.getLink() + "\" alt=\"" + condiName + "\" title=\"" + condiID + "\" height=\"18\" width=\"18\">" + condiName + "</td>");
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

        public static void writeDamageDistTableSkill(StreamWriter sw, SkillItem skill, SkillData skill_data, List<DamageLog> damageLogs, int finalTotalDamage, int casts = -1, double timeswasted = -1, double timessaved = 1)
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
                ParseEnum.Result result = dl.getResult();
                if (result == ParseEnum.Result.Crit) { crit++; } else if (result == ParseEnum.Result.Glance) { glance++; }
                if (dl.isFlanking() == 1) { flank++; }
            }
            avgdamage = (int)(totaldamage / (double)hits);
            string wasted = timeswasted > 0.0 ? Math.Round(timeswasted, 2) + "s" : "";
            string saved = timessaved < 0.0 ? Math.Round(timessaved, 2) + "s" : "";
            double hpcast = -1;
            if (casts > 0) {
                hpcast = Math.Round(hits / (double)casts, 2);
            }
            string skillName = (skill.getID().ToString() == skill.getName()) ? skill_data.getName(skill.getID()): skill.getName();
            if (totaldamage != 0 && skill.GetGW2APISkill() != null)
            {
                sw.Write("<tr>");
                {
                    sw.Write("<td align=\"left\"><img src=\"" + skill.GetGW2APISkill().icon + "\" alt=\"" + skill.getName() + "\" title=\"" + skill.getID() + "\" height=\"18\" width=\"18\">" + skillName + "</td>");
                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)totaldamage / finalTotalDamage) + "%</td>");
                    sw.Write("<td>" + totaldamage + "</td>");
                    sw.Write("<td>" + mindamage + "</td>");
                    sw.Write("<td>" + avgdamage + "</td>");
                    sw.Write("<td>" + maxdamage + "</td>");
                    sw.Write("<td>" + (casts != -1 ? casts.ToString() : "") + "</td>");
                    sw.Write("<td>" + hits + "</td>");
                    sw.Write("<td>" + (hpcast != -1 ? hpcast.ToString() : "") + "</td>");
                    sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\""+ crit+ " out of " + hits + " hits\">" + String.Format("{0:0.00}", 100 * (double)crit / hits) + "%</span></td>");
                    sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + flank + " out of " + hits + " hits\">" + String.Format("{0:0.00}", 100 * (double)flank / hits) + "%</span></td>");
                    sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + glance + " out of " + hits + " hits\">" + String.Format("{0:0.00}", 100 * (double)glance / hits) + "%</span></td>");
                    sw.Write("<td>" + wasted +"</td>");
                    sw.Write("<td>" + saved + "</td>");
                }
                sw.Write("</tr>");
            }
            else if (totaldamage != 0)
            {
                sw.Write("<tr>");
                {
                    sw.Write("<td align=\"left\">" + skillName + "</td>");
                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)totaldamage / finalTotalDamage) + "%</td>");
                    sw.Write("<td>" + totaldamage + "</td>");
                    sw.Write("<td>" + mindamage + "</td>");
                    sw.Write("<td>" + avgdamage + "</td>");
                    sw.Write("<td>" + maxdamage + "</td>");
                    sw.Write("<td>" + (casts != -1 ? casts.ToString() : "") + "</td>");
                    sw.Write("<td>" + hits + "</td>");
                    sw.Write("<td>" + (hpcast != -1 ? hpcast.ToString() : "") + "</td>");
                    sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + crit + " out of " + hits + " hits\">" + String.Format("{0:0.00}", 100 * (double)crit / hits) + "%</span></td>");
                    sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + flank + " out of " + hits + " hits\">" + String.Format("{0:0.00}", 100 * (double)flank / hits) + "%</span></td>");
                    sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + glance + " out of " + hits + " hits\">" + String.Format("{0:0.00}", 100 * (double)glance / hits) + "%</span></td>");
                    sw.Write("<td>" + wasted + "</td>");
                    sw.Write("<td>" + saved + "</td>");
                }
                sw.Write("</tr>");
            }
            else if (skill.GetGW2APISkill() != null)
            {
                sw.Write("<tr>");
                {
                    sw.Write("<td align=\"left\"><img src=\"" + skill.GetGW2APISkill().icon + "\" alt=\"" + skill.getName() + "\" title=\"" + skill.getID() + "\" height=\"18\" width=\"18\">" + skillName + "</td>");
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
                    sw.Write("<td align=\"left\">" + skillName + "</td>");
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

        public static void writePlayerTabBoonGraph(StreamWriter sw, BoonsGraphModel bgm, long start, long end)
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

        public static void writePlayerTabDPSGraph(StreamWriter sw, string name, List<Point> playerdpsgraphdata, AbstractPlayer p)
        {
            int ptdgCount = 0;
            bool total = name.Contains("Total");
            bool cleave = name.Contains("Cleave");
            bool s10 = name.Contains("10s");
            bool s30 = name.Contains("30s");
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
            string color = GetLink("Color-" + p.getProf() + ( total? "-Total" : ( cleave? "-NonBoss": "")));
            sw.Write(" mode: 'lines'," +
                   "line: {shape: 'spline',color:'" + color + "'}," +
                   "yaxis: 'y3',");
            if (s10 || s30)
            {
                sw.Write(" visible: 'legendonly',");
            }
            // "legendgroup: 'Damage'," +
            sw.Write("name: '" + name+"'," +
                "legendgroup: '" + p.getCharacter() + (s10 ? "10s" : (s30 ? "30s" : ""))+"'");
        }

        public static int writeDPSPlots(StreamWriter sw, List<Point> graphdata, List<Point> totalData = null)
        {
            //Adding dps axis
            int maxDPS = 0;
            sw.Write("y: [");
            for (int i = 0; i < graphdata.Count; i++)
            {
                if (i == graphdata.Count - 1)
                {
                    sw.Write("'" + graphdata[i].Y + "'");
                }
                else
                {
                    sw.Write("'" + graphdata[i].Y + "',");
                }
                if (totalData != null)
                {
                    maxDPS = Math.Max(maxDPS, graphdata[i].Y);
                    if (i >= totalData.Count)
                    {
                        totalData.Add(new Point(graphdata[i].X, graphdata[i].Y));
                    }
                    else
                    {
                        totalData[i] = new Point(graphdata[i].X, graphdata[i].Y + totalData[i].Y);
                    }
                }
            }
            //cuts off extra comma
            if (graphdata.Count == 0)
            {
                sw.Write("'0'");
            }

            sw.Write("],");
            //add time axis
            sw.Write("x: [");
            for (int i = 0; i < graphdata.Count; i++)
            {
                if (i == graphdata.Count - 1)
                {
                    sw.Write("'" + graphdata[i].X + "'");
                }
                else
                {
                    sw.Write("'" + graphdata[i].X + "',");
                }
            }
            if (graphdata.Count == 0)
            {
                sw.Write("'0'");
            }

            sw.Write("],");
            return maxDPS;
        }     

        public static void writeDamageStatsTableHeader(StreamWriter sw)
        {
            sw.Write("<tr>");
            {
                sw.Write("<th>Sub</th>");
                sw.Write("<th></th>");
                sw.Write("<th>Name</th>");
                sw.Write("<th><img src=\"" + GetLink("Crit") + "\" alt=\"Crits\" title=\"Percent time hits critical\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GetLink("Scholar") + "\" alt=\"Scholar\" title=\"Percent time hits while above 90% health\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GetLink("SwS") + "\" alt=\"SwS\" title=\"Percent time hits while moveing\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GetLink("Flank") + "\" alt=\"Flank\" title=\"Percent time hits while flanking\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GetLink("Glance") + "\" alt=\"Glance\" title=\"Percent time hits while glanceing\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GetLink("Blinded") + "\" alt=\"Miss\" title=\"Number of hits while blinded\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GetLink("Interupts") + "\" alt=\"Interupts\" title=\"Number of hits interupted?/hits used to interupt\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GetLink("Invuln") + "\" alt=\"Ivuln\" title=\"times the enemy was invulnerable to attacks\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GetLink("Wasted") + "\" alt=\"Wasted\" title=\"Time wasted(in seconds) interupting skill casts\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GetLink("Saved") + "\" alt=\"Saved\" title=\"Time saved(in seconds) interupting skill casts\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GetLink("Swap") + "\" alt=\"Swap\" title=\"Times weapon swapped\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GetLink("Stack") + "\" alt=\"Stack\" title=\"Average Distance from center of group stack\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GetLink("Downs") + "\" alt=\"Downs\" title=\"Times downed\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GetLink("Dead") + "\" alt=\"Dead\" title=\"Time died\" height=\"18\" width=\"18\"></th>");
            }
            sw.Write("</tr>");
        }

        public static void writeDamageStatsTableFoot(StreamWriter sw, List<string[]> footerList)
        {
            foreach (string groupNum in footerList.Select(x => x[0]).Distinct())
            {
                List<string[]> groupList = footerList.Where(x => x[0] == groupNum).ToList();
                sw.Write("<tr>");
                {
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td>Group " + groupNum + "</td>");
                    sw.Write("<td>" + Math.Round(100 * groupList.Sum(c => Double.Parse(c[2]) / Double.Parse(c[1])) / groupList.Count,1) + "%</td>");
                    sw.Write("<td>" + Math.Round(100 * groupList.Sum(c => Double.Parse(c[3]) / Double.Parse(c[1])) / groupList.Count,1) + "%</td>");
                    sw.Write("<td>" + Math.Round(100 * groupList.Sum(c => Double.Parse(c[4]) / Double.Parse(c[1])) / groupList.Count,1) + "%</td>");
                    sw.Write("<td>" + Math.Round(100 * groupList.Sum(c => Double.Parse(c[5]) / Double.Parse(c[1])) / groupList.Count,1) + "%</td>");
                    sw.Write("<td>" + Math.Round(100 * groupList.Sum(c => Double.Parse(c[6]) / Double.Parse(c[1])) / groupList.Count,1) + "%</td>");
                    sw.Write("<td>" + groupList.Sum(c => int.Parse(c[7])) + "</td>");
                    sw.Write("<td>" + groupList.Sum(c => int.Parse(c[8])) + "</td>");
                    sw.Write("<td>" + groupList.Sum(c => int.Parse(c[9])) + "</td>");
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td>" + groupList.Sum(c => int.Parse(c[10])) + "</td>");
                    sw.Write("<td></td>");
                    sw.Write("<td>" + groupList.Sum(c => int.Parse(c[11])) + "</td>");
                    sw.Write("<td></td>");
                }
                sw.Write("</tr>");
            }
            sw.Write("<tr>");
            {
                sw.Write("<td></td>");
                sw.Write("<td></td>");
                sw.Write("<td>Total</td>");
                sw.Write("<td>" + Math.Round(100 * footerList.Sum(c => Double.Parse(c[2]) / Double.Parse(c[1])) / footerList.Count,1) + "%</td>");
                sw.Write("<td>" + Math.Round(100 * footerList.Sum(c => Double.Parse(c[3]) / Double.Parse(c[1])) / footerList.Count,1) + "%</td>");
                sw.Write("<td>" + Math.Round(100 * footerList.Sum(c => Double.Parse(c[4]) / Double.Parse(c[1])) / footerList.Count,1) + "%</td>");
                sw.Write("<td>" + Math.Round(100 * footerList.Sum(c => Double.Parse(c[5]) / Double.Parse(c[1])) / footerList.Count,1) + "%</td>");
                sw.Write("<td>" + Math.Round(100 * footerList.Sum(c => Double.Parse(c[6]) / Double.Parse(c[1])) / footerList.Count,1) + "%</td>");
                sw.Write("<td>" + footerList.Sum(c => int.Parse(c[7])) + "</td>");
                sw.Write("<td>" + footerList.Sum(c => int.Parse(c[8])) + "</td>");
                sw.Write("<td>" + footerList.Sum(c => int.Parse(c[9])) + "</td>");
                sw.Write("<td></td>");
                sw.Write("<td></td>");
                sw.Write("<td>" + footerList.Sum(c => int.Parse(c[10])) + "</td>");
                sw.Write("<td></td>");
                sw.Write("<td>" + footerList.Sum(c => int.Parse(c[11])) + "</td>");
                sw.Write("<td></td>");
            }
            sw.Write("</tr>");
        }

        public static void writeCombatReplayInterface(StreamWriter sw, Tuple<int,int> canvasSize, ParsedLog log)
        {
            sw.Write("<div class=\"d-flex justify-content-around align-items-center justify-content-center\">");
            {
                sw.Write("<div class=\"d-flex flex-column flex-wrap\">");
                {
                    sw.Write("<canvas width=\"" + canvasSize.Item1 + "px\" height=\"" + canvasSize.Item2 + "px\" id=\"replayCanvas\" class=\"replay\">");
                    sw.Write("</canvas>");
                    sw.Write("<div class=\"d-flex justify-content-center slidecontainer\">");
                    {
                        sw.Write("<input oninput=\"updateTime(this.value);\"type=\"range\" min=\"0\" max=\"" + (log.getBoss().getCombatReplay().getPositions().Count - 1) + "\" value=\"0\" class=\"slider\" id=\"timeRange\">");
                        sw.Write("<input class=\"ml-5\" type=\"text\" id=\"timeRangeDisplay\" disabled value=\"0 secs\">");
                    }
                    sw.Write("</div>");
                    sw.Write("<div class=\"d-flex justify-content-center\">");
                    {
                        sw.Write("<div onclick=\"startAnimate();\" type=\"button\" class=\"btn btn-dark\">Animate</div>");
                        sw.Write("<div onclick=\"stopAnimate();\" type=\"button\" class=\"btn btn-dark\">Pause</div>");
                        sw.Write("<div onclick=\"restartAnimate();\" type=\"button\" class=\"btn btn-dark\">Restart</div>");
                    }
                    sw.Write("</div>");
                    sw.Write("<div class=\"d-flex justify-content-center btn-group btn-group-toggle\" data-toggle=\"buttons\">");
                    {
                        sw.Write("<label onclick=\"normalSpeed()\" class=\"btn btn-dark active\">" +
                                "<input type=\"radio\" autocomplete=\"off\" checked>1x" +
                            "</label>");
                        sw.Write("<label onclick=\"twoSpeed()\" class=\"btn btn-dark\">" +
                                 "<input  type=\"radio\" autocomplete=\"off\">2x" +
                             "</label>");
                        sw.Write("<label onclick=\"fourSpeed()\" class=\"btn btn-dark\">" +
                                 "<input  type=\"radio\" autocomplete=\"off\">4x" +
                             "</label>");
                    }
                    sw.Write("</div>");
                }
                sw.Write("</div>");

                sw.Write("<div class=\"d-flex flex-column justify-content-center align-items-center\">");
                {
                    sw.Write("<h3>Range Selectors</h3>");
                    sw.Write("<div style=\"width:200px;\" class=\"d-flex flex-row flex-wrap justify-content-center align-items-center btn-group btn-group-toggle mb-5\" data-toggle=\"buttons\">");
                    {
                        sw.Write("<label style=\"width: 60px;\" onclick=\"toggleRange(180);\" class=\"btn btn-dark\">" +
                            "<input class=\"invisible\" type=\"checkbox\" autocomplete=\"off\">" +
                            "180" +
                            "</label >");
                        sw.Write("<label style=\"width: 60px;\" onclick=\"toggleRange(240);\" class=\"btn btn-dark\">" +
                            "<input class=\"invisible\" type=\"checkbox\" autocomplete=\"off\">" +
                            "240" +
                            "</label >");
                        sw.Write("<label style=\"width: 60px;\" onclick=\"toggleRange(300);\" class=\"btn btn-dark\">" +
                            "<input class=\"invisible\" type=\"checkbox\" autocomplete=\"off\">" +
                            "300" +
                            "</label >");
                        sw.Write("<label style=\"width: 60px;\" onclick=\"toggleRange(600);\" class=\"btn btn-dark\">" +
                            "<input class=\"invisible\" type=\"checkbox\" autocomplete=\"off\">" +
                            "600" +
                            "</label >");
                        sw.Write("<label style=\"width: 60px;\" onclick=\"toggleRange(900);\" class=\"btn btn-dark\">" +
                            "<input class=\"invisible\" type=\"checkbox\" autocomplete=\"off\">" +
                            "900" +
                            "</label >");
                        sw.Write("<label style=\"width: 60px;\" onclick=\"toggleRange(1200);\" class=\"btn btn-dark\">" +
                            "<input class=\"invisible\" type=\"checkbox\" autocomplete=\"off\">" +
                            "1200" +
                            "</label >");
                    }
                    sw.Write("</div>");
                    sw.Write("<div class=\"d-flex flex-column justify-content-center align-items-center btn-group btn-group-toggle mb-5\" data-toggle=\"buttons\">");
                    {
                        List<int> groups = log.getPlayerList().Select(x => x.getGroup()).Distinct().ToList();
                        foreach (int group in groups)
                        {
                            sw.Write("<div class=\"d-flex flex-column justify-content-center align-items-center mt-2\">");
                            {
                                sw.Write("<h3>Group " + group + "</h3>");
                                foreach (Player p in log.getPlayerList().Where(x => x.getGroup() == group))
                                {
                                    sw.Write("<label id=\"id" + p.getInstid() + "\" style=\"width: 150px;\" onclick=\"selectActor(" + p.getInstid() + ")\"  class=\"btn btn-dark\">" +
                                        "<input class=\"invisible\" type=\"radio\" autocomplete=\"off\">" +
                                        p.getCharacter().Substring(0, Math.Min(10, p.getCharacter().Length))
                                        + " <img src=\"" + GetLink(p.getProf())
                                            + "\" alt=\"" + p.getProf()
                                            + "\" height=\"18\" width=\"18\" >" +
                                        "</label >");
                                }
                            }
                            sw.Write("</div>");
                        }
                    }
                    sw.Write("</div>");
                }
                sw.Write("</div>");

            }
            sw.Write("</div>");
        }

        private static void writeCombatReplayControls(StreamWriter sw, ParsedLog log, int pollingRate)
        {
            // animation control
            sw.Write("function startAnimate() {if (animation === null) { " +
                "if (time ===" + (log.getBoss().getCombatReplay().getPositions().Count - 1) + ") {" +
                    "time = 0;" +
                "}" +
                "animation = setInterval(function(){myanimate(time++)},speed);" +
                "}};");
            sw.Write("function stopAnimate(){ if (animation !== null) {window.clearInterval(animation); animation = null; time--;}};");
            sw.Write("function restartAnimate() { time = 0; myanimate(time++);};");
            // speed control
            sw.Write("function normalSpeed(){ speed = " + pollingRate + "; if (animation !== null) {window.clearInterval(animation); time--; animation = setInterval(function(){myanimate(time++)},speed);}};");
            sw.Write("function twoSpeed(){ speed = " + pollingRate/2 + "; if (animation !== null) {window.clearInterval(animation); time--; animation = setInterval(function(){myanimate(time++)},speed);}};");
            sw.Write("function fourSpeed(){ speed = " + pollingRate/4 + "; if (animation !== null) {window.clearInterval(animation); time--; animation = setInterval(function(){myanimate(time++)},speed);}};");
            // slider
            sw.Write("var timeSlider = document.getElementById('timeRange');");
            sw.Write("var timeSliderDisplay = document.getElementById('timeRangeDisplay');");
            sw.Write("function updateTime(value) { time = value; myanimate(time); updateTextInput(time)};");
            sw.Write("function updateTextInput(val) {" +
                "timeSliderDisplay.value = Math.round("+pollingRate+"*val/100.0)/10.0 + ' secs';" +
            "}");
            // Range marker control
            sw.Write("var rangeControl = new Map();" +
                "rangeControl.set(180,false);" +
                "rangeControl.set(240,false);" +
                "rangeControl.set(300,false);" +
                "rangeControl.set(600,false);" +
                "rangeControl.set(900,false);" +
                "rangeControl.set(1200,false);");
            sw.Write("function toggleRange(radius) {rangeControl.set(radius, !rangeControl.get(radius)); myanimate(time);};");
            // Selection
            sw.Write("function selectActor(pId) { " +
                    "var actor = data.get(pId);" +
                    "selectedPlayer = null;" +
                    "var oldSelect = actor.selected;" +
                    "data.forEach(function(value,key,map) {" +
                        "value.selected = false;" +
                    "});" +
                    "actor.selected = !oldSelect;" +
                    "selectedGroup = actor.selected ? actor.group : -1;" +
                    "if (!actor.selected){" +
                        "var hasActive = document.getElementById('id'+pId).classList.contains('active');" +
                        "if (hasActive) {" +
                            "setTimeout(function() {document.getElementById('id'+pId).classList.remove('active')},50);" +
                        "}" +
                    "} else {" +
                        "selectedPlayer = actor;" +
                    "}" +
                    "myanimate(time);" +
                "}");
        }

        private static void writeCombatReplayMainClass(StreamWriter sw, ParsedLog log,CombatReplayMap map, int pollingRate)
        {
            // Players and boss
            sw.Write("var mainActor = function(group, imgSrc) {" +
                    "this.group = group;" +
                    "this.pos = [];" +
                    "this.start = 0;" +
                    "this.dead = [];" +
                    "this.down = [];" +
                    "this.selected = false;" +
                    "this.img = new Image();" +
                    "this.img.src = imgSrc;" +
                "};");
            sw.Write("mainActor.prototype.died = function(timeToUse) {" +
                    "for (var i = 0; i < this.dead.length; i++) {" +
                        "if (!this.dead[i]) continue;" +
                        "if (this.dead[i][0] <= timeToUse && this.dead[i][1] >= timeToUse) {" +
                            "return true;" +
                        "}" +
                    "}" +
                    "return false;" +
                "};");
            sw.Write("mainActor.prototype.downed = function(timeToUse) {" +
                    "for (var i = 0; i < this.down.length; i++) {" +
                        "if (!this.down[i]) continue;"+
                        "if (this.down[i][0] <= timeToUse && this.down[i][1] >= timeToUse) {" +
                            "return true;" +
                        "}" +
                    "}" +
                    "return false;" +
                "};");
            sw.Write("mainActor.prototype.draw = function(ctx,timeToUse, pixelSize) {" +
                    "if (!this.pos.length) {" +
                    "   return;" +
                    "}" +
                    "var halfSize = pixelSize / 2;" +
                    "var x = this.pos.length > 2 ? this.pos[2*timeToUse] : this.pos[0];" +
                    "var y = this.pos.length > 2 ? this.pos[2*timeToUse + 1] : this.pos[1];" +
                    // the player is in the selected's player group
                    "if (!this.selected && this.group === selectedGroup) {" +
                        "ctx.beginPath();" +
                        "ctx.lineWidth='2';" +
                        "ctx.strokeStyle='blue';" +
                        "ctx.rect(x-halfSize,y-halfSize,pixelSize,pixelSize);" +
                        "ctx.stroke();" +
                    "} else if (this.selected){" +
                        // this player is selected
                        "ctx.beginPath();" +
                        "ctx.lineWidth='4';" +
                        "ctx.strokeStyle='green';" +
                        "ctx.rect(x-halfSize,y-halfSize,pixelSize,pixelSize);" +
                        "ctx.stroke();" +
                        "var _this = this;" +
                        // draw range markers
                        "rangeControl.forEach(function(enabled,radius,map) {" +
                            "if (!enabled) return;" +
                            "ctx.beginPath();" +
                            "ctx.lineWidth='2';" +
                            "ctx.strokeStyle='green';" +
                            "ctx.arc(x,y,inch * radius,0,2*Math.PI);" +
                            "ctx.stroke();" +
                        "});" +
                    "}" +
                    "if (this.died(timeToUse)) {" +
                        "ctx.drawImage(deadIcon," +
                        "x-1.5*halfSize," +
                        "y-1.5*halfSize,1.5*pixelSize,1.5*pixelSize);" +
                    "} else if (this.downed(timeToUse)) {" +
                        "ctx.drawImage(downIcon," +
                        "x-1.5*halfSize," +
                        "y-1.5*halfSize,1.5*pixelSize,1.5*pixelSize);" +
                    "} else {" +
                        "ctx.drawImage(this.img," +
                        "x-halfSize," +
                        "y-halfSize,pixelSize,pixelSize);" +
                    "}" +
                "};");
            // create players
            foreach (Player p in log.getPlayerList())
            {
                sw.Write("{");
                sw.Write("var p = new mainActor(" + p.getGroup() + ",'" + p.getCombatReplay().getIcon() + "');");
                sw.Write("data.set(" + p.getInstid() + ",p);");
                sw.Write("p.pos = [");
                foreach (Point3D pos in p.getCombatReplay().getPositions())
                {
                    Tuple<int, int> coord = map.getMapCoord(pos.X, pos.Y);
                    sw.Write(coord.Item1 + ",");
                    sw.Write(coord.Item2 + ",");
                }
                sw.Write("];");
                sw.Write("p.dead = [");
                foreach (Tuple<long, long> status in p.getCombatReplay().getDead())
                {
                    sw.Write("[" + status.Item1/pollingRate + ",");
                    sw.Write(status.Item2 / pollingRate + "],");
                }
                sw.Write("];");
                sw.Write("p.down = [");
                foreach (Tuple<long,long> status in p.getCombatReplay().getDown())
                {
                    sw.Write("[" + status.Item1 / pollingRate + ",");
                    sw.Write(status.Item2 / pollingRate + "],");
                }
                sw.Write("];");
                sw.Write("}");
            }
            // create boss
            sw.Write("boss = new mainActor(-2,'" + log.getBoss().getCombatReplay().getIcon() + "');");
            sw.Write("boss.pos = [");
            foreach (Point3D pos in log.getBoss().getCombatReplay().getPositions())
            {
                Tuple<int, int> coord = map.getMapCoord(pos.X, pos.Y);
                sw.Write(coord.Item1 + ",");
                sw.Write(coord.Item2 + ",");
            }
            sw.Write("];");
        }

        private static void writeCombatReplaySecondaryClass(StreamWriter sw, ParsedLog log, CombatReplayMap map, int pollingRate)
        {
            // thrash mobs
            sw.Write("var secondaryActor = function(imgSrc, start, end) {" +
                    "this.pos = [];" +
                    "this.start = start;" +
                    "this.end = end;" +
                    "this.img = new Image();" +
                    "this.img.src = imgSrc;" +
                "};");
            sw.Write("secondaryActor.prototype.draw = function(ctx,timeToUse,pixelSize){" +
                    "if (!(this.start > timeToUse || this.end < timeToUse) && this.pos.length) {" +
                        "var x = this.pos.length > 2 ? this.pos[2*(timeToUse - this.start)] : this.pos[0];" +
                        "var y = this.pos.length > 2 ? this.pos[2*(timeToUse - this.start) + 1] : this.pos[1];" +
                        "ctx.drawImage(this.img," +
                        "x-pixelSize/2,y-pixelSize/2," +
                        "pixelSize,pixelSize);" +
                    "}" +
                "};");
            // create thrash mobs
            foreach (Mob mob in log.getBoss().getThrashMobs())
            {
                sw.Write("{");
                sw.Write("var p = new secondaryActor('" + mob.getCombatReplay().getIcon() + "'," + mob.getCombatReplay().getTimeOffsets().Item1 / pollingRate + "," + mob.getCombatReplay().getTimeOffsets().Item2 / pollingRate + ");");
                sw.Write("secondaryData.set('" + mob.getInstid() + "_" + mob.getCombatReplay().getTimeOffsets().Item1 / pollingRate + "_" + mob.getCombatReplay().getTimeOffsets().Item2 / pollingRate + "',p);");
                sw.Write("p.pos = [");
                foreach (Point3D pos in mob.getCombatReplay().getPositions())
                {
                    Tuple<int, int> coord = map.getMapCoord(pos.X, pos.Y);
                    sw.Write(coord.Item1 + ",");
                    sw.Write(coord.Item2 + ",");
                }
                sw.Write("];");
                sw.Write("}");
            }
        }

        private static void writeCombatReplayCircleActors(StreamWriter sw, ParsedLog log, CombatReplayMap map, int pollingRate)
        {
            // Circle actors
            sw.Write("var circleActor = function(radius,fill,growing, color, start, end) {" +
                    "this.pos = null;" +
                    "this.master = null;" +
                    "this.start = start;" +
                    "this.radius = radius;" +
                    "this.end = end;" +
                    "this.growing = growing;" +
                    "this.fill = fill;" +
                    "this.color = color;" +
                "};");
            sw.Write("circleActor.prototype.draw = function(ctx,timeToUse){" +
                    "if (!(this.start > timeToUse || this.end < timeToUse)) {" +
                        "var x,y;" +
                        "if (this.pos instanceof Array) {" +
                            "x = this.pos[0];" +
                            "y = this.pos[1];" +
                        "} else {" +
                            "if (!this.master) {" +
                                "var playerID = parseInt(this.pos);" +
                                "this.master = data.has(playerID) ? data.get(playerID) : (secondaryData.has(this.pos) ? secondaryData.get(this.pos): boss);" +
                            "}" +
                            "var start = this.master.start ? this.master.start : 0;" +
                            "x = this.master.pos.length > 2 ? this.master.pos[2*(timeToUse - start)] : this.master.pos[0];" +
                            "y = this.master.pos.length > 2 ? this.master.pos[2*(timeToUse - start) + 1] : this.master.pos[1];" +
                        "}" +
                        "if (this.growing) {" +
                            "var percent = Math.min((timeToUse - this.start)/(this.growing - this.start),1.0);" +
                            "ctx.beginPath();" +
                            "ctx.arc(x,y,percent*inch * this.radius,0,2*Math.PI);" +
                            "if (this.fill) {" +
                                "ctx.fillStyle=this.color;" +
                                "ctx.fill();" +
                            "} else {" +
                                "ctx.lineWidth='2';" +
                                "ctx.strokeStyle=this.color;" +
                                "ctx.stroke();" +
                            "}" +
                        "} else {" +
                            "ctx.beginPath();" +
                            "ctx.arc(x,y,inch * this.radius,0,2*Math.PI);" +
                            "if (this.fill) {" +
                                "ctx.fillStyle=this.color;" +
                                "ctx.fill();" +
                            "} else {" +
                                "ctx.lineWidth='2';" +
                                "ctx.strokeStyle=this.color;" +
                                "ctx.stroke();" +
                            "}" +
                        "}" +
                    "}" +
                "};");
            foreach (Mob mob in log.getBoss().getThrashMobs())
            {
                CombatReplay replay = mob.getCombatReplay();
                foreach(CircleActor a in replay.getCircleActors())
                {
                    sw.Write("{");
                    sw.Write("var a = new circleActor("+a.getRadius()+","+(a.isFilled() ? "true" : "false") + ","+a.getGrowing() / pollingRate + ","+a.getColor()+","+a.getLifespan().Item1/pollingRate+","+ a.getLifespan().Item2 / pollingRate + ");");
                    sw.Write("mechanicData.add(a);");
                    sw.Write("a.pos ="+a.getPosition(mob.getInstid() + "_" + mob.getCombatReplay().getTimeOffsets().Item1 / pollingRate + "_" + mob.getCombatReplay().getTimeOffsets().Item2 / pollingRate, map)+";");
                    sw.Write("}");
                }
            }
            foreach (Player player in log.getPlayerList())
            {
                CombatReplay replay = player.getCombatReplay();
                foreach (CircleActor a in replay.getCircleActors())
                {
                    sw.Write("{");
                    sw.Write("var a = new circleActor(" + a.getRadius() + "," + (a.isFilled() ? "true" : "false") + "," + a.getGrowing() / pollingRate + "," + a.getColor() + "," + a.getLifespan().Item1 / pollingRate + "," + a.getLifespan().Item2 / pollingRate + ");");
                    sw.Write("mechanicData.add(a);");
                    sw.Write("a.pos =" + a.getPosition(player.getInstid().ToString(), map) + ";");
                    sw.Write("}");
                }
            }
            foreach (CircleActor a in log.getBoss().getCombatReplay().getCircleActors())
            {
                sw.Write("{");
                sw.Write("var a = new circleActor(" + a.getRadius() + "," + (a.isFilled() ? "true" : "false") + "," + a.getGrowing() / pollingRate + "," + a.getColor() + "," + a.getLifespan().Item1 / pollingRate + "," + a.getLifespan().Item2 / pollingRate + ");");
                sw.Write("mechanicData.add(a);");
                sw.Write("a.pos =" + a.getPosition(log.getBossData().getInstid().ToString(), map) + ";");
                sw.Write("}");

            }
        }

        private static void writeCombatReplayDoughnutActors(StreamWriter sw, ParsedLog log, CombatReplayMap map, int pollingRate)
        {
            // Circle actors
            sw.Write("var doughnutActor = function(innerRadius,outerRadius,growing, color, start, end) {" +
                    "this.pos = null;" +
                    "this.master = null;" +
                    "this.start = start;" +
                    "this.innerRadius = innerRadius;" +
                    "this.outerRadius = outerRadius;" +
                    "this.end = end;" +
                    "this.growing = growing;" +
                    "this.color = color;" +
                "};");
            sw.Write("doughnutActor.prototype.draw = function(ctx,timeToUse){" +
                    "if (!(this.start > timeToUse || this.end < timeToUse)) {" +
                        "var x,y;" +
                        "if (this.pos instanceof Array) {" +
                            "x = this.pos[0];" +
                            "y = this.pos[1];" +
                        "} else {" +
                            "if (!this.master) {" +
                                "var playerID = parseInt(this.pos);" +
                                "this.master = data.has(playerID) ? data.get(playerID) : (secondaryData.has(this.pos) ? secondaryData.get(this.pos): boss);" +
                            "}" +
                            "var start = this.master.start ? this.master.start : 0;" +
                            "x = this.master.pos.length > 2 ? this.master.pos[2*(timeToUse - start)] : this.master.pos[0];" +
                            "y = this.master.pos.length > 2 ? this.master.pos[2*(timeToUse - start) + 1] : this.master.pos[1];" +
                        "}" +
                        "var radius = 0.5*(this.innerRadius + this.outerRadius);" +
                        "var width = (this.outerRadius - this.innerRadius);" +
                        "if (this.growing) {" +
                            "var percent = Math.min((timeToUse - this.start)/(this.growing - this.start),1.0);" +
                            "ctx.beginPath();" +
                            "ctx.arc(x,y,inch * radius,0,2*Math.PI);" +
                            "ctx.lineWidth=(inch * percent * width).toString();" +
                            "ctx.strokeStyle=this.color;" +
                            "ctx.stroke();" +
                        "} else {" +
                            "ctx.beginPath();" +
                            "ctx.arc(x,y,inch * radius,0,2*Math.PI);" +
                            "ctx.lineWidth=(inch * width).toString();" +
                            "ctx.strokeStyle=this.color;" +
                            "ctx.stroke();" +
                        "}" +
                    "}" +
                "};");
            foreach (Mob mob in log.getBoss().getThrashMobs())
            {
                CombatReplay replay = mob.getCombatReplay();
                foreach (DoughnutActor a in replay.getDoughnutActors())
                {
                    sw.Write("{");
                    sw.Write("var a = new doughnutActor(" + a.getInnerRadius() + "," + a.getOuterRadius() + "," + a.getGrowing() / pollingRate + "," + a.getColor() + "," + a.getLifespan().Item1 / pollingRate + "," + a.getLifespan().Item2 / pollingRate + ");");
                    sw.Write("mechanicData.add(a);");
                    sw.Write("a.pos =" + a.getPosition(mob.getInstid() + "_" + mob.getCombatReplay().getTimeOffsets().Item1 / pollingRate + "_" + mob.getCombatReplay().getTimeOffsets().Item2 / pollingRate, map) + ";");
                    sw.Write("}");
                }
            }
            foreach (Player player in log.getPlayerList())
            {
                CombatReplay replay = player.getCombatReplay();
                foreach (DoughnutActor a in replay.getDoughnutActors())
                {
                    sw.Write("{");
                    sw.Write("var a = new doughnutActor(" + a.getInnerRadius() + "," + a.getOuterRadius() + "," + a.getGrowing() / pollingRate + "," + a.getColor() + "," + a.getLifespan().Item1 / pollingRate + "," + a.getLifespan().Item2 / pollingRate + ");");
                    sw.Write("mechanicData.add(a);");
                    sw.Write("a.pos =" + a.getPosition(player.getInstid().ToString(), map) + ";");
                    sw.Write("}");
                }
            }
            foreach (DoughnutActor a in log.getBoss().getCombatReplay().getDoughnutActors())
            {
                sw.Write("{");
                sw.Write("var a = new doughnutActor(" + a.getInnerRadius() + "," + a.getOuterRadius() + "," + a.getGrowing() / pollingRate + "," + a.getColor() + "," + a.getLifespan().Item1 / pollingRate + "," + a.getLifespan().Item2 / pollingRate + ");");
                sw.Write("mechanicData.add(a);");
                sw.Write("a.pos =" + a.getPosition(log.getBossData().getInstid().ToString(), map) + ";");
                sw.Write("}");

            }
        }

        public static void writeCombatReplayScript(StreamWriter sw, ParsedLog log, Tuple<int,int> canvasSize, CombatReplayMap map, int pollingRate)
        {
            sw.Write("<script>");
            {
                // globals
                sw.Write("var animation = null;");
                sw.Write("var time = 0;");
                sw.Write("var inch = " + map.getInch()+";");
                sw.Write("var speed = "+ pollingRate+";");
                sw.Write("var selectedGroup = -1;");
                sw.Write("var selectedPlayer = null;");
                sw.Write("var data = new Map();");
                sw.Write("var secondaryData = new Map();");
                sw.Write("var mechanicData = new Set();");
                sw.Write("var deadIcon = new Image();" +
                            "deadIcon.src = '"+GetLink("Dead")+"';");
                sw.Write("var downIcon = new Image();" +
                            "downIcon.src = '" + GetLink("Downs") + "';");
                sw.Write("var boss = null;");
                writeCombatReplayControls(sw, log, pollingRate);
                writeCombatReplayMainClass(sw, log, map, pollingRate);
                writeCombatReplaySecondaryClass(sw, log, map, pollingRate);
                writeCombatReplayCircleActors(sw, log, map, pollingRate);
                writeCombatReplayDoughnutActors(sw, log, map, pollingRate);
                // Main loop
                sw.Write("var ctx = document.getElementById('replayCanvas').getContext('2d');");
                sw.Write("ctx.imageSmoothingEnabled = true;");
                sw.Write("ctx.imageSmoothingQuality = 'high';");
                sw.Write("function myanimate(timeToUse) {");
                {
                    sw.Write("ctx.clearRect(0,0," + canvasSize.Item1 + "," + canvasSize.Item2 + ");");
                    // draw arena
                    sw.Write("ctx.drawImage(bgImage,0,0," + canvasSize.Item1 + "," + canvasSize.Item2 + ");");
                    // draw mechanics
                    sw.Write("mechanicData.forEach(function(value,key,map) {" +
                            "value.draw(ctx,timeToUse);" +
                        "});");
                    // draw unselected players
                    sw.Write("data.forEach(function(value,key,map) {" +
                            "if (!value.selected) {" +
                                "value.draw(ctx,timeToUse,20);"+
                            "}" +
                        "});");
                    // draw thrash mobs
                    sw.Write("secondaryData.forEach(function(value,key,map) {" +
                            "value.draw(ctx,timeToUse,28);"+
                        "});");
                    // draw boss
                    sw.Write("boss.draw(ctx,timeToUse,36);");
                    // draw selected player
                    sw.Write("if (selectedPlayer) {" +
                                "selectedPlayer.draw(ctx,timeToUse,20);"+                              
                            "}");
                    sw.Write("if (timeToUse === " + (log.getBoss().getCombatReplay().getPositions().Count - 1) + ") {stopAnimate();}");
                    sw.Write("timeSlider.value = time;");
                    sw.Write("updateTextInput(time);");
                }
                sw.Write("}");
                // when background loaded
                sw.Write("var bgImage = new Image();");
                sw.Write("bgImage.onload = function() { myanimate(0);};");
                sw.Write("bgImage.src = '" + map.getLink() + "';");
            }
            sw.Write("</script>");
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
                case "16202-icon"://Massive Kitty Golem
                    return "https://wiki.guildwars2.com/images/3/33/Mini_Snuggles.png";
                case "16177-icon"://Avg Kitty Golem
                    return "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                case "19676-icon"://Large Kitty Golem
                    return "https://wiki.guildwars2.com/images/4/47/Mini_Baron_von_Scrufflebutt.png";
                case "19645-icon"://Med Kitty Golem
                    return "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                case "16199-icon"://Std Kitty Golem
                    return "https://wiki.guildwars2.com/images/8/8f/Mini_Professor_Mew.png";

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

                case "16202-ext"://Massive Kitty Golem
                    return "MassiveGolem";
                case "16177-ext"://Avg Kitty Golem
                    return "AvgGolem";
                case "19676-ext"://Large Kitty Golem
                    return "LGolem";
                case "19645-ext"://Med Kitty Golem
                    return "MedGolem";
                case "16199-ext"://Std Kitty Golem
                    return "StdGolem";
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
                    return "arts";
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

                case "Color-Warrior-NonBoss": return "rgb(125,109,66)";
                case "Color-Berserker-NonBoss": return "rgb(125,109,66)";
                case "Color-Spellbreaker-NonBoss": return "rgb(125,109,66)";
                case "Color-Guardian-NonBoss": return "rgb(62,101,113)";
                case "Color-Dragonhunter-NonBoss": return "rgb(62,101,113)";
                case "Color-Firebrand-NonBoss": return "rgb(62,101,113)";
                case "Color-Revenant-NonBoss": return "rgb(110,60,50)";
                case "Color-Herald-NonBoss": return "rgb(110,60,50)";
                case "Color-Renegade-NonBoss": return "rgb(110,60,50)";
                case "Color-Engineer-NonBoss": return "rgb(109,83,48)";
                case "Color-Scrapper-NonBoss": return "rgb(109,83,48)";
                case "Color-Holosmith-NonBoss": return "rgb(109,83,48)";
                case "Color-Ranger-NonBoss": return "rgb(75,115,70)";
                case "Color-Druid-NonBoss": return "rgb(75,115,70)";
                case "Color-Soulbeast-NonBoss": return "rgb(75,115,70)";
                case "Color-Thief-NonBoss": return "rgb(101,76,79)";
                case "Color-Daredevil-NonBoss": return "rgb(101,76,79)";
                case "Color-Deadeye-NonBoss": return "rgb(101,76,79)";
                case "Color-Elementalist-NonBoss": return "rgb(127,74,72)";
                case "Color-Tempest-NonBoss": return "rgb(127,74,72)";
                case "Color-Weaver-NonBoss": return "rgb(127,74,72)";
                case "Color-Mesmer-NonBoss": return "rgb(96,60,111)";
                case "Color-Chronomancer-NonBoss": return "rgb(96,60,111)";
                case "Color-Mirage-NonBoss": return "rgb(96,60,111)";
                case "Color-Necromancer-NonBoss": return "rgb(46,88,60)";
                case "Color-Reaper-NonBoss": return "rgb(46,88,60)";
                case "Color-Scourge-NonBoss": return "rgb(46,88,60)";
                case "Color-Boss-NonBoss": return "rgb(92,177,250)";

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
                case "Stack":
                    return "https://wiki.guildwars2.com/images/e/ef/Commander_arrow_marker.png";

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
