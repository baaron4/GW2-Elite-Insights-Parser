using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using LuckParser.Models.DataModels;
using Newtonsoft.Json;

namespace LuckParser.Controllers
{
    class HTMLHelper
    {
        public static SettingsContainer Settings;

        public static void WriteCastingItem(StreamWriter sw, CastLog cl, SkillData skillList, PhaseData phase)
        {
            SkillItem skill = skillList.Get(cl.SkillId);
            GW2APISkill skillApi = skill?.ApiSkill;
            string skillName = skill.Name;
            float dur;
            if (cl.SkillId == SkillItem.DodgeId)
            {
                dur = 0.5f;
            }
            else if (cl.SkillId == SkillItem.WeaponSwapId)
            {
                dur = 0.1f;
            }
            else
            {
                dur = cl.ActualDuration / 1000f;
            }
            skillName = skillName.Replace("\"", "");
            float offset = (cl.Time - phase.Start) / 1000f;
            float xVal = dur;
            if (offset < 0.0f)
            {
                xVal += offset;
            }
            xVal = Math.Min(xVal, (phase.End - cl.Time) / 1000f);
            sw.Write("{");
            {
                sw.Write(cl.SkillId == -5 ? "y: ['1']," : "y: ['1.5'],");
              
                sw.Write(
                       "x: ['" + xVal + "']," +
                       "base:'" + Math.Max(offset,0.0f) + "'," +
                       "name: \"" + skillName + " " + dur + "s\"," +//get name should be handled by api
                       "orientation:'h'," +
                       "mode: 'markers'," +
                       "type: 'bar',");
                if (skillApi != null)
                {
                    sw.Write(skillApi.slot == "Weapon_1" ? "width:'0.5'," : "width:'1',");
                }
                else
                { 
                    sw.Write("width:'1',");
                }
                sw.Write("hoverinfo: 'name'," +
                        "hoverlabel:{namelength:'-1'},");
                sw.Write("marker: {");
                {
                        if (cl.EndActivation == ParseEnum.Activation.CancelFire)
                        {
                            sw.Write("color: 'rgb(40,40,220)',");
                        }
                        else if (cl.EndActivation == ParseEnum.Activation.CancelCancel)
                        {
                            sw.Write("color: 'rgb(220,40,40)',");
                        }
                        else if (cl.EndActivation == ParseEnum.Activation.Reset)
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
                            if (cl.StartActivation == ParseEnum.Activation.Normal)
                            {
                                sw.Write("color: 'rgb(20,20,20)',");
                            }
                            else if (cl.StartActivation == ParseEnum.Activation.Quickness)
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

        public static void WriteCastingItemIcon(StreamWriter sw, CastLog cl, SkillData skillList, PhaseData phase, bool last)
        {
            SkillItem skill = skillList.Get(cl.SkillId);
            GW2APISkill skillApi = skill?.ApiSkill;
            float offset = (cl.Time - phase.Start) / 1000f;
            if ((skillApi != null && skillApi.slot != "Weapon_1") || skillApi == null)
            {
                sw.Write("{" +
                             "source: '" + skill.Icon + "'," +
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

            if (!last)
            {
                sw.Write(",");
            }
        }

        public static void WriteBoonTableHeader(StreamWriter sw, List<Boon> listToUse)
        {
            sw.Write("<thead>");
            {
                sw.Write("<tr>");
                {
                    sw.Write("<th width=\"50px\">Sub</th>");
                    sw.Write("<th width=\"50px\"></th>");
                    sw.Write("<th>Name</th>");
                    foreach (Boon boon in listToUse)
                    {
                        sw.Write("<th width=\"50px\">" + "<img src=\"" + boon.Link + "\" alt=\"" + boon.Name + "\" title =\" " + boon.Name + "\" height=\"18\" width=\"18\" >" + "</th>");
                    }
                }
                sw.Write("</tr> ");
            }
            sw.Write("</thead>");

        }

        public static void WriteBoonGenTableBody(StreamWriter sw, Player player, List<Boon> listToUse, Dictionary<long, string> boonArray)
        {
            sw.Write("<tr>");
            {
                sw.Write("<td>" + player.Group + "</td>");
                sw.Write("<td>" + "<img src=\"" + GeneralHelper.GetProfIcon(player.Prof) + "\" alt=\"" + player.Prof + "\" height=\"20\" width=\"20\" >" + "<span style=\"display:none\">" + player.Prof + "</span>" + "</td>");
                sw.Write("<td>" + player.Character + "</td>");
                foreach (Boon boon in listToUse)
                {
                    if (boonArray.ContainsKey(boon.ID))
                    {
                        sw.Write("<td>" + boonArray[boon.ID] + "</td>");
                    }
                    else
                    {
                        sw.Write("<td>" + 0 + "</td>");
                    }
                }
            }
            sw.Write("</tr>");
        }

        public static void WriteDamageDistTableHeader(StreamWriter sw)
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

        public static void WriteDamageDistTableFoot(StreamWriter sw, int finalTotalDamage)
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

        public static void WriteDamageDistTableCondi(StreamWriter sw, HashSet<long> usedIDs, List<DamageLog> damageLogs, int finalTotalDamage, List<Boon> presentConditions)
        {
            foreach (Boon condi in presentConditions)
            {
                int totaldamage = 0;
                int mindamage = 0;
                int avgdamage;
                int hits = 0;
                int maxdamage = 0;
                long condiID = condi.ID;
                usedIDs.Add(condiID);
                foreach (DamageLog dl in damageLogs.Where(x => x.SkillId == condiID))
                {
                    int curdmg = dl.Damage;
                    totaldamage += curdmg;
                    if (0 == mindamage || curdmg < mindamage) { mindamage = curdmg; }
                    if (0 == maxdamage || curdmg > maxdamage) { maxdamage = curdmg; }
                    hits++;

                }
                avgdamage = (int)(totaldamage / (double)hits);
                if (totaldamage != 0)
                {
                    string condiName = condi.Name;// Boon.getCondiName(condiID);
                    sw.Write("<tr class=\"condi\">");
                    {
                        sw.Write("<td align=\"left\"><img src=\"" + condi.Link + "\" alt=\"" + condiName + "\" title=\"" + condiID + "\" height=\"18\" width=\"18\">" + condiName + "</td>");
                        sw.Write("<td>" + Math.Round(100 * (double)totaldamage / finalTotalDamage,2) + "%</td>");
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

        public static void WriteDamageDistTableSkill(StreamWriter sw, SkillItem skill, List<DamageLog> damageLogs, int finalTotalDamage, int casts = -1, double timeswasted = -1, double timessaved = 1)
        {
            int totaldamage = 0;
            int mindamage = 0;
            int avgdamage;
            int hits = 0;
            int maxdamage = 0;
            int crit = 0;
            int flank = 0;
            int glance = 0;
            foreach (DamageLog dl in damageLogs)
            {
                int curdmg = dl.Damage;
                totaldamage += curdmg;
                if (0 == mindamage || curdmg < mindamage) { mindamage = curdmg; }
                if (0 == maxdamage || curdmg > maxdamage) { maxdamage = curdmg; }
                hits++;
                ParseEnum.Result result = dl.Result;
                if (result == ParseEnum.Result.Crit) { crit++; } else if (result == ParseEnum.Result.Glance) { glance++; }
                if (dl.IsFlanking == 1) { flank++; }
            }
            avgdamage = (int)(totaldamage / (double)hits);
            string wasted = timeswasted > 0.0 ? Math.Round(timeswasted, 2) + "s" : "";
            string saved = timessaved < 0.0 ? Math.Round(timessaved, 2) + "s" : "";
            double hpcast = -1;
            if (casts > 0) {
                hpcast = Math.Round(hits / (double)casts, 2);
            }
            if (totaldamage != 0)
            {
                sw.Write("<tr>");
                {
                    sw.Write("<td align=\"left\"><img src=\"" + skill.Icon + "\" alt=\"" + skill.Name + "\" title=\"" + skill.ID + "\" height=\"18\" width=\"18\">" + skill.Name + "</td>");
                    sw.Write("<td>" + Math.Round(100 * (double)totaldamage / finalTotalDamage,2) + "%</td>");
                    sw.Write("<td>" + totaldamage + "</td>");
                    sw.Write("<td>" + mindamage + "</td>");
                    sw.Write("<td>" + avgdamage + "</td>");
                    sw.Write("<td>" + maxdamage + "</td>");
                    sw.Write("<td>" + (casts != -1 ? casts.ToString() : "") + "</td>");
                    sw.Write("<td>" + hits + "</td>");
                    sw.Write("<td>" + (hpcast > 0 ? hpcast.ToString() : "") + "</td>");

                    sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + crit + " out of " + hits + " hits\">" + Math.Round(100 * (double)crit / hits, 2) + "%</td>");
                    sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + flank + " out of " + hits + " hits\">" + Math.Round(100 * (double)flank / hits, 2) + "%</td>");
                    sw.Write("<td data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + glance + " out of " + hits + " hits\">" + Math.Round(100 * (double)glance / hits, 2) + "%</td>");
                    sw.Write("<td>" + wasted +"</td>");
                    sw.Write("<td>" + saved + "</td>");
                }
                sw.Write("</tr>");
            }
            else
            {
                sw.Write("<tr>");
                {
                    sw.Write("<td align=\"left\"><img src=\"" + skill.Icon + "\" alt=\"" + skill.Name + "\" title=\"" + skill.ID + "\" height=\"18\" width=\"18\">" + skill.Name + "</td>");
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
        
        public static void WriteBossHealthGraph(StreamWriter sw, int maxDPS, PhaseData phase, double[] bossHealth, string yAxis = "")
        {
            int duration = (int)phase.GetDuration("s");
            double[] chart = bossHealth.Skip((int)phase.Start / 1000).Take(duration+1).ToArray();
            //Boss Health
            //Adding dps axis
            sw.Write("y: [");
            if (maxDPS == 0)
            {
                maxDPS = 1000;
            }
            for (int i = 0; i < chart.Length; i++)
            {
                double health = chart[i];
                if (i == chart.Length - 1)
                {
                    sw.Write("'" + ((health / 100.0) * maxDPS).ToString().Replace(',', '.') + "'");
                }
                else
                {
                    sw.Write("'" + ((health / 100.0) * maxDPS).ToString().Replace(',', '.') + "',");
                }
            }
            sw.Write("],");
            //text axis is boss hp in %
            sw.Write("text: [");
            for (int i = 0; i < chart.Length; i++)
            {
                double health = chart[i];
                if (i == chart.Length - 1)
                {
                    sw.Write("'" + (health + "%").Replace(',', '.') + "'");
                }
                else
                {
                    sw.Write("'" + (health + "%").Replace(',', '.') + "',");
                }
            }

            sw.Write("],");
            //add time axis
            sw.Write("x: [");
            for (int i = 0; i < chart.Length; i++)
            {
                double health = chart[i];
                if (i == chart.Length - 1)
                {
                    sw.Write("'" + i + "'");
                }
                else
                {
                    sw.Write("'" + i + "',");
                }
            }

            sw.Write("],");
            sw.Write(" mode: 'lines'," +
                    " line: {shape: 'spline', dash: 'dashdot'}," +
                   ( yAxis.Length > 0 ? " yaxis: '"+ yAxis+"',"  : "") +
                    "hoverinfo: 'text'," +
                    " name: 'Boss health'");
            
        }

        public static void WritePlayerTabBoonGraph(StreamWriter sw, BoonsGraphModel bgm, PhaseData phase)
        {
            long roundedEnd = phase.Start + 1000 * phase.GetDuration("s");
            List<BoonsGraphModel.Segment> bChart = bgm.BoonChart.Where(x => x.End >= phase.Start && x.Start <= roundedEnd).ToList();
            if (bChart.Count == 0 || (bChart.Count == 1 && bChart.First().Value == 0))
            {
                return;
            }
            sw.Write("y: [");
            {
                foreach (BoonsGraphModel.Segment seg in bChart)
                {
                    sw.Write("'" + seg.Value + "',");
                }
                sw.Write("'" + bChart.Last().Value + "'");
            }
            sw.Write("],");
            sw.Write("x: [");
            {
                foreach (BoonsGraphModel.Segment seg in bChart)
                {
                    double segStart = Math.Round(Math.Max(seg.Start - phase.Start, 0) / 1000.0,3);
                    sw.Write("'" + segStart + "',");
                }
                sw.Write("'" + Math.Round(Math.Min(bChart.Last().End - phase.Start, roundedEnd - phase.Start) / 1000.0, 3) + "'");
            }
            sw.Write("],");
            sw.Write(" yaxis: 'y2'," +
                 " type: 'scatter',");
            //  "legendgroup: '"+Boon.getEnum(bgm.getBoonName()).getPloltyGroup()+"',";
            if (!(bgm.BoonName == "Might" || bgm.BoonName == "Quickness"))
            {
                sw.Write(" visible: 'legendonly',");
            }
            sw.Write(" line: {color:'" + GetLink("Color-" + bgm.BoonName) + "', shape: 'hv'},");
            sw.Write(" fill: 'tozeroy'," +
                 " name: \"" + bgm.BoonName + "\"");
        }

        public static void WritePlayerTabDPSGraph(StreamWriter sw, string name, List<Point> playerdpsgraphdata, AbstractPlayer p)
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
            string color = GetLink("Color-" + p.Prof + ( total? "-Total" : ( cleave? "-NonBoss": "")));
            sw.Write(" mode: 'lines'," +
                   "line: {shape: 'spline',color:'" + color + "'}," +
                   "yaxis: 'y3',");
            if (s10 || s30)
            {
                sw.Write(" visible: 'legendonly',");
            }
            // "legendgroup: 'Damage'," +
            sw.Write("name: '" + name+"'," +
                "legendgroup: '" + p.Character + (s10 ? "10s" : (s30 ? "30s" : ""))+"'");
        }

        public static int WriteDPSPlots(StreamWriter sw, List<Point> graphdata, List<Point> totalData = null)
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

        public static void WriteDamageStatsTableHeader(StreamWriter sw)
        {
            sw.Write("<tr>");
            {
                sw.Write("<th>Sub</th>");
                sw.Write("<th></th>");
                sw.Write("<th>Name</th>");
                sw.Write("<th><img src=\"" + GetLink("Crit") + "\" alt=\"Crits\" title=\"Percent time hits critical\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GetLink("Scholar") + "\" alt=\"Scholar\" title=\"Percent time hits while above 90% health\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GetLink("SwS") + "\" alt=\"SwS\" title=\"Percent time hits while moving\" height=\"18\" width=\"18\"></th>");
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

        public static void WriteDamageStatsTableFoot(StreamWriter sw, List<string[]> footerList)
        {
            foreach (string groupNum in footerList.Select(x => x[0]).Distinct())
            {
                List<string[]> groupList = footerList.Where(x => x[0] == groupNum).ToList();
                sw.Write("<tr>");
                {
                    sw.Write("<td></td>");
                    sw.Write("<td></td>");
                    sw.Write("<td>Group " + groupNum + "</td>");
                    sw.Write("<td>" + Math.Round(100 * groupList.Sum(c => double.Parse(c[2]) / double.Parse(c[1])) / groupList.Count,1) + "%</td>");
                    sw.Write("<td>" + Math.Round(100 * groupList.Sum(c => double.Parse(c[3]) / double.Parse(c[1])) / groupList.Count,1) + "%</td>");
                    sw.Write("<td>" + Math.Round(100 * groupList.Sum(c => double.Parse(c[4]) / double.Parse(c[1])) / groupList.Count,1) + "%</td>");
                    sw.Write("<td>" + Math.Round(100 * groupList.Sum(c => double.Parse(c[5]) / double.Parse(c[1])) / groupList.Count,1) + "%</td>");
                    sw.Write("<td>" + Math.Round(100 * groupList.Sum(c => double.Parse(c[6]) / double.Parse(c[1])) / groupList.Count,1) + "%</td>");
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
                sw.Write("<td>" + Math.Round(100 * footerList.Sum(c => double.Parse(c[2]) / double.Parse(c[1])) / footerList.Count,1) + "%</td>");
                sw.Write("<td>" + Math.Round(100 * footerList.Sum(c => double.Parse(c[3]) / double.Parse(c[1])) / footerList.Count,1) + "%</td>");
                sw.Write("<td>" + Math.Round(100 * footerList.Sum(c => double.Parse(c[4]) / double.Parse(c[1])) / footerList.Count,1) + "%</td>");
                sw.Write("<td>" + Math.Round(100 * footerList.Sum(c => double.Parse(c[5]) / double.Parse(c[1])) / footerList.Count,1) + "%</td>");
                sw.Write("<td>" + Math.Round(100 * footerList.Sum(c => double.Parse(c[6]) / double.Parse(c[1])) / footerList.Count,1) + "%</td>");
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

        public static void WriteCombatReplayInterface(StreamWriter sw, Tuple<int,int> canvasSize, ParsedLog log)
        {
            string replayHTML = Properties.Resources.template_replay_html;
            replayHTML = replayHTML.Replace("${canvasX}", canvasSize.Item1.ToString());
            replayHTML = replayHTML.Replace("${canvasY}", canvasSize.Item2.ToString());
            replayHTML = replayHTML.Replace("${maxTime}", log.PlayerList.First().CombatReplay.Times.Last().ToString());
            List<int> groups = log.PlayerList.Where(x => x.Account != ":Conjured Sword").Select(x => x.Group).Distinct().ToList();
            string groupsString = "";
            foreach (int group in groups)
            {
                string replayGroupHTML = Properties.Resources.tmplGroupCombatReplay;
                replayGroupHTML = replayGroupHTML.Replace("${group}", group.ToString());;
                string playerString = "";
                foreach (Player p in log.PlayerList.Where(x => x.Group == group))
                {
                    string replayPlayerHTML = Properties.Resources.tmplPlayerSelectCombatReplay;
                    replayPlayerHTML = replayPlayerHTML.Replace("${instid}", p.InstID.ToString());
                    replayPlayerHTML = replayPlayerHTML.Replace("${playerName}", p.Character.Substring(0, Math.Min(10, p.Character.Length)));
                    replayPlayerHTML = replayPlayerHTML.Replace("${imageURL}", GeneralHelper.GetProfIcon(p.Prof));
                    replayPlayerHTML = replayPlayerHTML.Replace("${prof}", p.Prof);
                    playerString += replayPlayerHTML;
                }
                replayGroupHTML = replayGroupHTML.Replace("<!--${players}-->", playerString);
                groupsString += replayGroupHTML;
            }
            replayHTML = replayHTML.Replace("<!--${groups}-->", groupsString);
            sw.Write(replayHTML);
        }

        public static void WriteCombatReplayScript(StreamWriter sw, ParsedLog log, Tuple<int,int> canvasSize, CombatReplayMap map, int pollingRate)
        {
            sw.WriteLine("<script>");
            sw.WriteLine(Properties.Resources.combatreplay_js);
            sw.WriteLine("</script>");

            Dictionary<string, object> options = new Dictionary<string, object>
            {
                { "inch", map.GetInch() },
                { "pollingRate", pollingRate },
                { "mapLink", map.Link }
            };

            string actors = "";
            int count = 0;
            foreach (Player p in log.PlayerList)
            {
                if (p.Account == ":Conjured Sword")
                {
                    continue;
                }
                if (p.CombatReplay.Positions.Count == 0)
                {
                    continue;
                }
                if (count > 0)
                {
                    actors += ",";
                }
                count++;
                actors += p.GetCombatReplayJSON(map);
                foreach (Actor a in p.CombatReplay.Actors)
                {
                    actors += ",";
                    actors += a.GetCombatReplayJSON(map);
                }
            }
            foreach (Mob m in log.FightData.Logic.TrashMobs)
            {
                if (m.CombatReplay.Positions.Count == 0)
                {
                    continue;
                }
                actors += ",";
                actors += m.GetCombatReplayJSON(map);
                foreach (Actor a in m.CombatReplay.Actors)
                {
                    actors += ",";
                    actors += a.GetCombatReplayJSON(map);
                }
            }
            foreach (Boss target in log.FightData.Logic.Targets)
            {
                if (target.CombatReplay.Positions.Count == 0)
                {
                    continue;
                }
                actors += ",";
                actors += target.GetCombatReplayJSON(map);
                foreach (Actor a in target.CombatReplay.Actors)
                {
                    actors += ",";
                    actors += a.GetCombatReplayJSON(map);
                }
            }

            sw.WriteLine("<script>");
            {
                sw.WriteLine("var options = " + JsonConvert.SerializeObject(options) + ";");
                sw.WriteLine("var actors = [" + actors + "];");
                sw.WriteLine("initCombatReplay(actors, options);");
            }
            sw.WriteLine("</script>");
        }

        public static string GetLink(string name)
        {
            switch (name)
            {
                case "Question":
                    return "https://wiki.guildwars2.com/images/d/de/Sword_slot.png";
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
                    return "https://wiki.guildwars2.com/images/2/2b/Superior_Rune_of_the_Scholar.png";
                case "SwS":
                    return "https://wiki.guildwars2.com/images/1/1c/Bowl_of_Seaweed_Salad.png";
                case "Downs":
                    return "https://wiki.guildwars2.com/images/c/c6/Downed_enemy.png";
                case "Resurrect":
                    return "https://wiki.guildwars2.com/images/3/3d/Downed_ally.png";
                case "Dead":
                    return "https://wiki.guildwars2.com/images/4/4a/Ally_death_%28interface%29.png";
                case "Flank":
                    return "https://wiki.guildwars2.com/images/b/bb/Hunter%27s_Tactics.png";
                case "Glance":
                    return "https://wiki.guildwars2.com/images/f/f9/Weakness.png";
                case "Miss":
                    return "https://wiki.guildwars2.com/images/3/33/Blinded.png";
                case "Interupts":
                    return "https://wiki.guildwars2.com/images/7/79/Daze.png";
                case "Invuln":
                    return "https://wiki.guildwars2.com/images/e/eb/Determined.png";
                case "Blinded":
                    return "https://wiki.guildwars2.com/images/3/33/Blinded.png";
                case "Wasted":
                    return "https://wiki.guildwars2.com/images/b/b3/Out_Of_Health_Potions.png";
                case "Saved":
                    return "https://wiki.guildwars2.com/images/e/eb/Ready.png";
                case "Swap":
                    return "https://wiki.guildwars2.com/images/c/ce/Weapon_Swap_Button.png";
                case "Blank":
                    return "https://wiki.guildwars2.com/images/d/de/Sword_slot.png";
                case "Dodge":
                    return "https://wiki.guildwars2.com/images/archive/b/b2/20150601155307%21Dodge.png";
                case "Bandage":
                    return "https://wiki.guildwars2.com/images/0/0c/Bandage.png";
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
