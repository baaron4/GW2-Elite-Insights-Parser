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
    class LegacyHTMLHelper
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
            sw.Write(" line: {color:'" + GeneralHelper.GetLink("Color-" + bgm.BoonName) + "', shape: 'hv'},");
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
            string color = GeneralHelper.GetLink("Color-" + p.Prof + ( total? "-Total" : ( cleave? "-NonBoss": "")));
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
                sw.Write("<th><img src=\"" + GeneralHelper.GetLink("Crit") + "\" alt=\"Crits\" title=\"Percent time hits critical\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GeneralHelper.GetLink("Scholar") + "\" alt=\"Scholar\" title=\"Percent time hits while above 90% health\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GeneralHelper.GetLink("SwS") + "\" alt=\"SwS\" title=\"Percent time hits while moving\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GeneralHelper.GetLink("Flank") + "\" alt=\"Flank\" title=\"Percent time hits while flanking\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GeneralHelper.GetLink("Glance") + "\" alt=\"Glance\" title=\"Percent time hits while glanceing\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GeneralHelper.GetLink("Blinded") + "\" alt=\"Miss\" title=\"Number of hits while blinded\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GeneralHelper.GetLink("Interupts") + "\" alt=\"Interupts\" title=\"Number of hits interupted?/hits used to interupt\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GeneralHelper.GetLink("Invuln") + "\" alt=\"Ivuln\" title=\"times the enemy was invulnerable to attacks\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GeneralHelper.GetLink("Wasted") + "\" alt=\"Wasted\" title=\"Time wasted(in seconds) interupting skill casts\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GeneralHelper.GetLink("Saved") + "\" alt=\"Saved\" title=\"Time saved(in seconds) interupting skill casts\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GeneralHelper.GetLink("Swap") + "\" alt=\"Swap\" title=\"Times weapon swapped\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GeneralHelper.GetLink("Stack") + "\" alt=\"Stack\" title=\"Average Distance from center of group stack\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GeneralHelper.GetLink("Downs") + "\" alt=\"Downs\" title=\"Times downed\" height=\"18\" width=\"18\"></th>");
                sw.Write("<th><img src=\"" + GeneralHelper.GetLink("Dead") + "\" alt=\"Dead\" title=\"Time died\" height=\"18\" width=\"18\"></th>");
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

    }
}
