using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using LuckParser.Parser;
using Newtonsoft.Json;
using NUglify;
using LuckParser.Setting;

namespace LuckParser.Builders
{
    class CombatReplayHelper
    {
        public static SettingsContainer Settings;
  
        public static string CreateCombatReplayInterface((int width, int height) canvasSize, ParsedLog log)
        {
            string replayHTML = Properties.Resources.tmplCombatReplay;
            replayHTML = replayHTML.Replace("${canvasX}", canvasSize.width.ToString());
            replayHTML = replayHTML.Replace("${canvasY}", canvasSize.height.ToString());
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
                    replayPlayerHTML = replayPlayerHTML.Replace("${instid}", p.GetCombatReplayID().ToString());
                    replayPlayerHTML = replayPlayerHTML.Replace("${playerName}", p.Character.Substring(0, Math.Min(10, p.Character.Length)));
                    replayPlayerHTML = replayPlayerHTML.Replace("${imageURL}", GeneralHelper.GetProfIcon(p.Prof));
                    replayPlayerHTML = replayPlayerHTML.Replace("${prof}", p.Prof);
                    playerString += replayPlayerHTML;
                }
                replayGroupHTML = replayGroupHTML.Replace("<!--${players}-->", playerString);
                groupsString += replayGroupHTML;
            }
            replayHTML = replayHTML.Replace("<!--${groups}-->", groupsString);
            return replayHTML;
        }

        public static string GetDynamicCombatReplayScript(ParsedLog log, int pollingRate, CombatReplayMap map)
        {

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
                foreach (GenericActor a in p.CombatReplay.Actors)
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
                foreach (GenericActor a in m.CombatReplay.Actors)
                {
                    actors += ",";
                    actors += a.GetCombatReplayJSON(map);
                }
            }
            foreach (Target target in log.FightData.Logic.Targets)
            {
                if (target.CombatReplay.Positions.Count == 0)
                {
                    continue;
                }
                actors += ",";
                actors += target.GetCombatReplayJSON(map);
                foreach (GenericActor a in target.CombatReplay.Actors)
                {
                    actors += ",";
                    actors += a.GetCombatReplayJSON(map);
                }
            }
            string script = "var initialOnLoad = window.onload;";
            script += "window.onload = function () { if (initialOnLoad) {initialOnLoad();} animator = new Animator(" + JsonConvert.SerializeObject(options) + "); animator.initActors([" + actors + "]);};";
#if !DEBUG
            script = Uglify.Js(script, GeneralHelper.JSMinifySettings).Code;
#endif
            return script;
        }

        public static string CreateCombatReplayScript(ParsedLog log, CombatReplayMap map, int pollingRate)
        {
            string script = "";
            script += "<script>";
#if DEBUG
            script += Properties.Resources.combatreplay_js;
#else          
            script += Uglify.Js(Properties.Resources.combatreplay_js, GeneralHelper.JSMinifySettings).Code;
#endif
            script += "</script>";

            script += "<script>";
            {
                script += GetDynamicCombatReplayScript(log, pollingRate, map);
            }
            script += "</script>";
            return script;
        }
    }
}
