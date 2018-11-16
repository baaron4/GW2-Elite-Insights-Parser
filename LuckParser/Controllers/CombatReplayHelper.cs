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
    class CombatReplayHelper
    {
        public static SettingsContainer Settings;
  
        public static void WriteCombatReplayInterface(StreamWriter sw, Tuple<int,int> canvasSize, ParsedLog log)
        {
            string replayHTML = Properties.Resources.tmplCombatReplay;
            replayHTML = replayHTML.Replace("${canvasX}", canvasSize.Item1.ToString());
            replayHTML = replayHTML.Replace("${canvasY}", canvasSize.Item2.ToString());
            replayHTML = replayHTML.Replace("${maxTime}", log.PlayerList.First().CombatReplay.Times.Last().ToString());
            List<int> groups = log.PlayerList.Where(x => x.Account != ":Conjured Sword").Select(x => x.Group).Distinct().ToList();
            string groupsString = "";
            foreach (int group in groups)
            {
                string replayGroupHTML = Properties.Resources.tmplCombatReplayGroup;
                replayGroupHTML = replayGroupHTML.Replace("${group}", group.ToString());;
                string playerString = "";
                foreach (Player p in log.PlayerList.Where(x => x.Group == group))
                {
                    string replayPlayerHTML = Properties.Resources.tmplCombatReplayPlayer;
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
                sw.WriteLine("var initialOnLoad = window.onload;");
                sw.WriteLine("window.onload = function () { if (initialOnLoad) {initialOnLoad();} initCombatReplay(actors, options);};");
            }
            sw.WriteLine("</script>");
        }
    }
}
