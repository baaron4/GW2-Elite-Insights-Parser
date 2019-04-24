using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using LuckParser.Parser;
using Newtonsoft.Json;
using LuckParser.Setting;
using Newtonsoft.Json.Serialization;

namespace LuckParser.Builders
{
    class CombatReplayHelper
    {
        public static string CreateCombatReplayInterface((int width, int height) canvasSize, ParsedLog log)
        {
            string replayHTML = Properties.Resources.tmplCombatReplay;
            replayHTML = replayHTML.Replace("${canvasX}", canvasSize.width.ToString());
            replayHTML = replayHTML.Replace("${canvasY}", canvasSize.height.ToString());
            replayHTML = replayHTML.Replace("${maxTime}", log.PlayerList.First().GetCombatReplayTimes(log).Last().ToString());
            List<int> groups = log.PlayerList.Where(x => !x.IsFakeActor).Select(x => x.Group).Distinct().ToList();
            string groupsString = "";
            foreach (int group in groups)
            {
                string replayGroupHTML = Properties.Resources.tmplCombatReplayGroup;
                replayGroupHTML = replayGroupHTML.Replace("${group}", group.ToString());;
                string playerString = "";
                foreach (Player p in log.PlayerList.Where(x => x.Group == group))
                {
                    string replayPlayerHTML = Properties.Resources.tmplCombatReplayPlayer;
                    replayPlayerHTML = replayPlayerHTML.Replace("${instid}", p.GetCombatReplayID(log).ToString());
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

        public static List<object> GetCombatReplayActors(ParsedLog log, CombatReplayMap map)
        {
            List<object> actors = new List<object>();
            foreach (Player p in log.PlayerList)
            {
                if (p.IsFakeActor)
                {
                    continue;
                }
                if (p.GetCombatReplayPositions(log).Count == 0)
                {
                    continue;
                }
                actors.Add(p.GetCombatReplayJSON(map, log));
                foreach (GenericActor a in p.GetCombatReplayActors(log))
                {
                    actors.Add(a.GetCombatReplayJSON(map, log));
                }
            }
            foreach (Mob m in log.FightData.Logic.TrashMobs)
            {
                if (m.GetCombatReplayPositions(log).Count == 0)
                {
                    continue;
                }
                actors.Add(m.GetCombatReplayJSON(map, log));
                foreach (GenericActor a in m.GetCombatReplayActors(log))
                {
                    actors.Add(a.GetCombatReplayJSON(map, log));
                }
            }
            foreach (Target target in log.FightData.Logic.Targets)
            {
                if (target.GetCombatReplayPositions(log).Count == 0)
                {
                    continue;
                }
                actors.Add(target.GetCombatReplayJSON(map, log));
                foreach (GenericActor a in target.GetCombatReplayActors(log))
                {
                    actors.Add(a.GetCombatReplayJSON(map, log));
                }
            }
            return actors;
        }

        public static string GetDynamicCombatReplayScript(ParsedLog log, int pollingRate, CombatReplayMap map)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ContractResolver = contractResolver
            };
            Dictionary<string, object> options = new Dictionary<string, object>
            {
                { "inch", map.GetInch() },
                { "pollingRate", pollingRate },
                { "mapLink", map.Link }
            };

            List<object> actors = GetCombatReplayActors(log, map);
            
            string script = "var initialOnLoad = window.onload;";
            script += "window.onload = function () { if (initialOnLoad) {initialOnLoad();} animator = new Animator(" + JsonConvert.SerializeObject(options, settings) + "); animator.initActors(" + JsonConvert.SerializeObject(actors, settings) + ");};";
            return script;
        }

        public static string CreateCombatReplayScript(ParsedLog log, CombatReplayMap map, int pollingRate)
        {
            string script = "";
            script += "<script>";

            script += Properties.Resources.combatreplay_js;

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
