using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GW2EIParser.Builders.HtmlModels;
using GW2EIParser.EIData;
using GW2EIParser.Logic;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GW2EIParser.Builders
{
    public class HTMLBuilder
    {
        private readonly string _scriptVersion;
        private readonly int _scriptVersionRev;

        private readonly ParsedLog _log;
        private readonly List<PhaseData> _phases;
        private readonly bool _cr;
        private readonly bool _light;
        private readonly bool _externalScripts;

        private readonly string[] _uploadLink;

        private readonly GeneralStatistics _statistics;
        private readonly Dictionary<long, Buff> _usedBuffs = new Dictionary<long, Buff>();
        private readonly HashSet<DamageModifier> _usedDamageMods = new HashSet<DamageModifier>();
        private readonly Dictionary<long, SkillItem> _usedSkills = new Dictionary<long, SkillItem>();

        public HTMLBuilder(ParsedLog log, string[] uploadString, bool lightTheme, bool externalScript)
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            _scriptVersion = version.Major + "." + version.Minor;
#if !DEBUG
            _scriptVersion += "." + version.Build;
#endif
            _scriptVersionRev = version.Revision;
            _log = log;
            _phases = log.FightData.GetPhases(log);

            _statistics = log.Statistics;

            _uploadLink = uploadString ?? new string[] { "", "", "" };

            _cr = _log.CanCombatReplay;
            _light = lightTheme;
            _externalScripts = externalScript;
        }


        private List<List<object>> BuildDPSData(int phaseIndex)
        {
            var list = new List<List<object>>(_log.PlayerList.Count);
            foreach (Player player in _log.PlayerList)
            {
                FinalDPS dpsAll = player.GetDPSAll(_log, phaseIndex);
                list.Add(PhaseDto.GetDPSStatData(dpsAll));
            }
            return list;
        }

        private List<List<List<object>>> BuildDPSTargetsData(int phaseIndex)
        {
            var list = new List<List<List<object>>>(_log.PlayerList.Count);
            PhaseData phase = _phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                var playerData = new List<List<object>>();

                foreach (NPC target in phase.Targets)
                {
                    playerData.Add(PhaseDto.GetDPSStatData(player.GetDPSTarget(_log, phaseIndex, target)));
                }
                list.Add(playerData);
            }
            return list;
        }

        private List<List<object>> BuildDMGStatsData(int phaseIndex)
        {
            var list = new List<List<object>>();
            foreach (Player player in _log.PlayerList)
            {
                FinalGameplayStatsAll stats = player.GetGameplayStats(_log, phaseIndex);
                list.Add(PhaseDto.GetDMGStatData(stats));
            }
            return list;
        }

        private List<List<List<object>>> BuildDMGStatsTargetsData(int phaseIndex)
        {
            var list = new List<List<List<object>>>();

            PhaseData phase = _phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                var playerData = new List<List<object>>();
                foreach (NPC target in phase.Targets)
                {
                    FinalGameplayStats statsTarget = player.GetGameplayStats(_log, phaseIndex, target);
                    playerData.Add(PhaseDto.GetDMGTargetStatData(statsTarget));
                }
                list.Add(playerData);
            }
            return list;
        }

        private List<List<object>> BuildDefenseData(int phaseIndex)
        {
            var list = new List<List<object>>();

            PhaseData phase = _phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                FinalDefensesAll defenses = player.GetDefenses(_log, phaseIndex);
                list.Add(PhaseDto.GetDefenseStatData(defenses, phase));
            }

            return list;
        }

        private List<List<object>> BuildSupportData(int phaseIndex)
        {
            var list = new List<List<object>>();

            foreach (Player player in _log.PlayerList)
            {
                FinalPlayerSupport support = player.GetPlayerSupport(_log, phaseIndex);
                list.Add(PhaseDto.GetSupportStatData(support));
            }
            return list;
        }

        private Dictionary<string, List<Buff>> BuildPersonalBoonData(Dictionary<string, List<long>> dict)
        {
            var boonsBySpec = new Dictionary<string, List<Buff>>();
            // Collect all personal buffs by spec
            foreach (KeyValuePair<string, List<Player>> pair in _log.PlayerListBySpec)
            {
                List<Player> players = pair.Value;
                var specBoonIds = new HashSet<long>(_log.Buffs.GetRemainingBuffsList(pair.Key).Select(x => x.ID));
                var boonToUse = new HashSet<Buff>();
                foreach (Player player in players)
                {
                    for (int i = 0; i < _phases.Count; i++)
                    {
                        Dictionary<long, FinalPlayerBuffs> boons = player.GetBuffs(_log, i, BuffEnum.Self);
                        foreach (Buff boon in _statistics.PresentPersonalBuffs[player])
                        {
                            if (boons.TryGetValue(boon.ID, out FinalPlayerBuffs uptime))
                            {
                                if (uptime.Uptime > 0 && specBoonIds.Contains(boon.ID))
                                {
                                    boonToUse.Add(boon);
                                }
                            }
                        }
                    }
                }
                boonsBySpec[pair.Key] = boonToUse.ToList();
            }
            foreach (KeyValuePair<string, List<Buff>> pair in boonsBySpec)
            {
                dict[pair.Key] = new List<long>();
                foreach (Buff boon in pair.Value)
                {
                    dict[pair.Key].Add(boon.ID);
                    _usedBuffs[boon.ID] = boon;
                }
            }
            return boonsBySpec;
        }

        private Dictionary<string, List<DamageModifier>> BuildPersonalDamageModData(Dictionary<string, List<long>> dict)
        {
            var damageModBySpecs = new Dictionary<string, List<DamageModifier>>();
            // Collect all personal damage mods by spec
            foreach (KeyValuePair<string, List<Player>> pair in _log.PlayerListBySpec)
            {
                var specDamageModsName = new HashSet<string>(_log.DamageModifiers.GetModifiersPerProf(pair.Key).Select(x => x.Name));
                var damageModsToUse = new HashSet<DamageModifier>();
                foreach (Player player in pair.Value)
                {
                    var presentDamageMods = new HashSet<string>(player.GetPresentDamageModifier(_log).Intersect(specDamageModsName));
                    foreach (string name in presentDamageMods)
                    {
                        damageModsToUse.Add(_log.DamageModifiers.DamageModifiersByName[name]);
                    }
                }
                damageModBySpecs[pair.Key] = damageModsToUse.ToList();
            }
            foreach (KeyValuePair<string, List<DamageModifier>> pair in damageModBySpecs)
            {
                dict[pair.Key] = new List<long>();
                foreach (DamageModifier mod in pair.Value)
                {
                    dict[pair.Key].Add(mod.Name.GetHashCode());
                    _usedDamageMods.Add(mod);
                }
            }
            return damageModBySpecs;
        }
        
        /// <summary>
        /// Create the damage taken distribution table for a given player
        /// </summary>
        /// <param name="p"></param>
        /// <param name="phaseIndex"></param>
        

        private string ReplaceVariables(string html)
        {
            html = html.Replace("${bootstrapTheme}", !_light ? "slate" : "yeti");

            html = html.Replace("${encounterStart}", _log.LogData.LogStartStd);
            html = html.Replace("${encounterEnd}", _log.LogData.LogEndStd);
            html = html.Replace("${evtcVersion}", _log.LogData.BuildVersion);
            html = html.Replace("${gw2build}", _log.LogData.GW2Version.ToString());
            html = html.Replace("${fightID}", _log.FightData.TriggerID.ToString());
            html = html.Replace("${eiVersion}", Application.ProductVersion);
            html = html.Replace("${recordedBy}", _log.LogData.PoVName);

            string uploadString = "";
            if (_uploadLink[0].Length > 0)
            {
                uploadString += "<p>DPS Reports Link (EI): <a href=\"" + _uploadLink[0] + "\">" + _uploadLink[0] + "</a></p>";
            }
            if (_uploadLink[1].Length > 0)
            {
                uploadString += "<p>DPS Reports Link (RH): <a href=\"" + _uploadLink[1] + "\">" + _uploadLink[1] + "</a></p>";
            }
            if (_uploadLink[2].Length > 0)
            {
                uploadString += "<p>Raidar Link: <a href=\"" + _uploadLink[2] + "\">" + _uploadLink[2] + "</a></p>";
            }
            html = html.Replace("<!--${UploadLinks}-->", uploadString);

            return html;
        }

        public void CreateHTML(StreamWriter sw, string path)
        {
            string html = Properties.Resources.template_html;
            _log.UpdateProgressWithCancellationCheck("HTML: replacing global variables");
            html = ReplaceVariables(html);

            _log.UpdateProgressWithCancellationCheck("HTML: building CSS");
            html = html.Replace("<!--${Css}-->", BuildCss(path));
            _log.UpdateProgressWithCancellationCheck("HTML: building JS");
            html = html.Replace("<!--${Js}-->", BuildEIJs(path));
            _log.UpdateProgressWithCancellationCheck("HTML: building Combat Replay link");
            html = html.Replace("<!--${JsCRLink}-->", BuildCRLinkJs(path));

            html = html.Replace("'${logDataJson}'", BuildLogData());
#if DEBUG
            html = html.Replace("<!--${Vue}-->", "<script src=\"https://cdn.jsdelivr.net/npm/vue@2.5.17/dist/vue.js\"></script>");
#else
            html = html.Replace("<!--${Vue}-->", "<script src=\"https://cdn.jsdelivr.net/npm/vue@2.5.17/dist/vue.min.js\"></script>");
#endif

            _log.UpdateProgressWithCancellationCheck("HTML: building Graph Data");
            html = html.Replace("'${graphDataJson}'", ToJson(ChartDataDto.BuildChartData(_log)));

            _log.UpdateProgressWithCancellationCheck("HTML: building Combat Replay JS");
            html = html.Replace("<!--${CombatReplayScript}-->", BuildCombatReplayScript(path));
            sw.Write(html);
            return;
        }

        private string BuildCombatReplayScript(string path)
        {
            if (!_cr)
            {
                return "";
            }
            string scriptContent = Properties.Resources.combatreplay_js;
            if (_externalScripts && path != null)
            {
#if DEBUG
                string jsFileName = "EliteInsights-CR-" + _scriptVersion + ".debug.js";
#else
                string jsFileName = "EliteInsights-CR-" + _scriptVersion + ".js";
#endif
                string jsPath = Path.Combine(path, jsFileName);
                try
                {
                    using (var fs = new FileStream(jsPath, FileMode.Create, FileAccess.Write))
                    using (var scriptWriter = new StreamWriter(fs, GeneralHelper.NoBOMEncodingUTF8))
                    {
                        scriptWriter.Write(scriptContent);
                    }
                }
                catch (IOException)
                {
                }
                string content = "<script src=\"./" + jsFileName + "?version=" + _scriptVersionRev + "\"></script>\n";
                return content;
            }
            else
            {
                return "<script>\r\n" + scriptContent + "\r\n</script>";
            }
        }

        private static string BuildTemplates(string script)
        {
            string tmplScript = script;
            var templates = new Dictionary<string, string>()
            {
                {"${tmplBuffStats}",Properties.Resources.tmplBuffStats },
                {"${tmplBuffStatsTarget}",Properties.Resources.tmplBuffStatsTarget },
                {"${tmplBuffTable}",Properties.Resources.tmplBuffTable },
                {"${tmplDamageDistPlayer}",Properties.Resources.tmplDamageDistPlayer },
                {"${tmplDamageDistTable}",Properties.Resources.tmplDamageDistTable },
                {"${tmplDamageDistTarget}",Properties.Resources.tmplDamageDistTarget },
                {"${tmplDamageModifierTable}",Properties.Resources.tmplDamageModifierTable },
                {"${tmplDamageModifierStats}",Properties.Resources.tmplDamageModifierStats },
                {"${tmplDamageModifierPersStats}",Properties.Resources.tmplDamageModifierPersStats },
                {"${tmplDamageTable}",Properties.Resources.tmplDamageTable },
                {"${tmplDamageTaken}",Properties.Resources.tmplDamageTaken },
                {"${tmplDeathRecap}",Properties.Resources.tmplDeathRecap },
                {"${tmplDefenseTable}",Properties.Resources.tmplDefenseTable },
                {"${tmplEncounter}",Properties.Resources.tmplEncounter },
                {"${tmplFood}",Properties.Resources.tmplFood },
                {"${tmplGameplayTable}",Properties.Resources.tmplGameplayTable },
                {"${tmplGeneralLayout}",Properties.Resources.tmplGeneralLayout },
                {"${tmplMechanicsTable}",Properties.Resources.tmplMechanicsTable },
                {"${tmplPersonalBuffTable}",Properties.Resources.tmplPersonalBuffTable },
                {"${tmplPhase}",Properties.Resources.tmplPhase },
                {"${tmplPlayers}",Properties.Resources.tmplPlayers },
                {"${tmplPlayerStats}",Properties.Resources.tmplPlayerStats },
                {"${tmplPlayerTab}",Properties.Resources.tmplPlayerTab },
                {"${tmplSimpleRotation}",Properties.Resources.tmplSimpleRotation },
                {"${tmplSupportTable}",Properties.Resources.tmplSupportTable },
                {"${tmplTargets}",Properties.Resources.tmplTargets },
                {"${tmplTargetStats}",Properties.Resources.tmplTargetStats },
                {"${tmplTargetTab}",Properties.Resources.tmplTargetTab },
                {"${tmplDPSGraph}",Properties.Resources.tmplDPSGraph },
                {"${tmplGraphStats}",Properties.Resources.tmplGraphStats },
                {"${tmplPlayerTabGraph}",Properties.Resources.tmplPlayerTabGraph },
                {"${tmplRotationLegend}",Properties.Resources.tmplRotationLegend },
                {"${tmplTargetTabGraph}",Properties.Resources.tmplTargetTabGraph },
                {"${tmplTargetData}",Properties.Resources.tmplTargetData },
                {"${tmplMainView}",Properties.Resources.tmplMainView },
            };
            foreach (KeyValuePair<string, string> entry in templates)
            {
#if DEBUG
                tmplScript = tmplScript.Replace(entry.Key, entry.Value);
#else
                tmplScript = tmplScript.Replace(entry.Key, Regex.Replace(entry.Value, @"\t|\n|\r", ""));
#endif
            }
            return tmplScript;
        }

        private static string BuildCRTemplates(string script)
        {
            string tmplScript = script;
            var CRtemplates = new Dictionary<string, string>()
                {
                    {"${tmplCombatReplayDamageData}", Properties.Resources.tmplCombatReplayDamageData },
                    {"${tmplCombatReplayStatusData}", Properties.Resources.tmplCombatReplayStatusData },
                    {"${tmplCombatReplayDamageTable}", Properties.Resources.tmplCombatReplayDamageTable },
                    {"${tmplCombatReplayActorBuffStats}", Properties.Resources.tmplCombatReplayActorBuffStats },
                    {"${tmplCombatReplayPlayerStats}", Properties.Resources.tmplCombatReplayPlayerStats },
                    {"${tmplCombatReplayPlayerStatus}", Properties.Resources.tmplCombatReplayPlayerStatus },
                    {"${tmplCombatReplayActorRotation}", Properties.Resources.tmplCombatReplayActorRotation },
                    {"${tmplCombatReplayTargetStats}", Properties.Resources.tmplCombatReplayTargetStats },
                    {"${tmplCombatReplayTargetStatus}", Properties.Resources.tmplCombatReplayTargetStatus },
                    {"${tmplCombatReplayTargetsStats}", Properties.Resources.tmplCombatReplayTargetsStats },
                    {"${tmplCombatReplayPlayersStats}", Properties.Resources.tmplCombatReplayPlayersStats },
                    {"${tmplCombatReplayUI}", Properties.Resources.tmplCombatReplayUI },
                    {"${tmplCombatReplayPlayerSelect}", Properties.Resources.tmplCombatReplayPlayerSelect },
                    {"${tmplCombatReplayRangeSelect}", Properties.Resources.tmplCombatReplayRangeSelect },
                    {"${tmplCombatReplayAnimationControl}", Properties.Resources.tmplCombatReplayAnimationControl },
                    {"${tmplCombatReplayMechanicsList}", Properties.Resources.tmplCombatReplayMechanicsList },
                };
            foreach (KeyValuePair<string, string> entry in CRtemplates)
            {
                tmplScript = tmplScript.Replace(entry.Key, Regex.Replace(entry.Value, @"\t|\n|\r", ""));
            }
            return tmplScript;
        }

        private string BuildCss(string path)
        {
            string scriptContent = Properties.Resources.ei_css;

            if (_externalScripts && path != null)
            {
#if DEBUG
                string cssFilename = "EliteInsights-" + _scriptVersion + ".debug.css";
#else
                string cssFilename = "EliteInsights-" + _scriptVersion + ".css";
#endif
                string cssPath = Path.Combine(path, cssFilename);
                try
                {
                    using (var fs = new FileStream(cssPath, FileMode.Create, FileAccess.Write))
                    using (var scriptWriter = new StreamWriter(fs, GeneralHelper.NoBOMEncodingUTF8))
                    {
                        scriptWriter.Write(scriptContent);
                    }
                }
                catch (IOException)
                {
                }
                return "<link rel=\"stylesheet\" type=\"text/css\" href=\"./" + cssFilename + "?version=" + _scriptVersionRev + "\">";
            }
            else
            {
                return "<style type=\"text/css\">\r\n" + scriptContent + "\r\n</style>";
            }
        }

        private string BuildEIJs(string path)
        {
            var orderedScripts = new List<string>()
            {
                Properties.Resources.globalJS,
                Properties.Resources.commonsJS,
                Properties.Resources.headerJS,
                Properties.Resources.layoutJS,
                Properties.Resources.generalStatsJS,
                Properties.Resources.damageModifierStatsJS,
                Properties.Resources.buffStatsJS,
                Properties.Resources.graphsJS,
                Properties.Resources.mechanicsJS,
                Properties.Resources.targetStatsJS,
                Properties.Resources.playerStatsJS,
                Properties.Resources.ei_js
            };
            string scriptContent = orderedScripts[0];
            for (int i = 1; i < orderedScripts.Count; i++)
            {
                scriptContent += orderedScripts[i];
            }
            scriptContent = BuildTemplates(scriptContent);

            if (_externalScripts && path != null)
            {
#if DEBUG
                string scriptFilename = "EliteInsights-" + _scriptVersion + ".debug.js";
#else
                string scriptFilename = "EliteInsights-" + _scriptVersion + ".js";
#endif
                string scriptPath = Path.Combine(path, scriptFilename);
                try
                {
                    using (var fs = new FileStream(scriptPath, FileMode.Create, FileAccess.Write))
                    using (var scriptWriter = new StreamWriter(fs, GeneralHelper.NoBOMEncodingUTF8))
                    {
                        scriptWriter.Write(scriptContent);
                    }
                }
                catch (IOException)
                {
                }
                return "<script src=\"./" + scriptFilename + "?version=" + _scriptVersionRev + "\"></script>";
            }
            else
            {
                return "<script>\r\n" + scriptContent + "\r\n</script>";
            }
        }

        private string BuildCRLinkJs(string path)
        {
            if (!_cr)
            {
                return "";
            }
            var orderedScripts = new List<string>()
            {
                Properties.Resources.combatReplayStatsUI,
                Properties.Resources.combatReplayStatsJS,
            };
            string scriptContent = orderedScripts[0];
            for (int i = 1; i < orderedScripts.Count; i++)
            {
                scriptContent += orderedScripts[i];
            }
            scriptContent = BuildCRTemplates(scriptContent);

            if (_externalScripts && path != null)
            {
#if DEBUG
                string scriptFilename = "EliteInsights-CRLink-" + _scriptVersion + ".debug.js";
#else
                string scriptFilename = "EliteInsights-CRLink-" + _scriptVersion + ".js";
#endif
                string scriptPath = Path.Combine(path, scriptFilename);
                try
                {
                    using (var fs = new FileStream(scriptPath, FileMode.Create, FileAccess.Write))
                    using (var scriptWriter = new StreamWriter(fs, GeneralHelper.NoBOMEncodingUTF8))
                    {
                        scriptWriter.Write(scriptContent);
                    }
                }
                catch (IOException)
                {
                }
                return "<script src=\"./" + scriptFilename + "?version=" + _scriptVersionRev + "\"></script>";
            }
            else
            {
                return "<script>\r\n" + scriptContent + "\r\n</script>";
            }
        }

        private string BuildLogData()
        {
            ParsedLog log = _log;
            GeneralStatistics statistics = _statistics;
            log.UpdateProgressWithCancellationCheck("HTML: building Log Data");
            var logData = new LogDataDto();
            if (_cr)
            {
                logData.CrData = new CombatReplayDto(log);
            }
            log.UpdateProgressWithCancellationCheck("HTML: building Players");
            foreach (Player player in log.PlayerList)
            {
                logData.HasCommander = logData.HasCommander || player.HasCommanderTag;
                logData.Players.Add(new PlayerDto(player, log, _cr, BuildPlayerData(player)));
            }

            log.UpdateProgressWithCancellationCheck("HTML: building Enemies");
            foreach (AbstractActor enemy in log.MechanicData.GetEnemyList(log, 0))
            {
                logData.Enemies.Add(new EnemyDto() { Name = enemy.Character });
            }

            log.UpdateProgressWithCancellationCheck("HTML: building Targets");
            foreach (NPC target in log.FightData.Logic.Targets)
            {
                var targetDto = new TargetDto(target, log, _cr, BuildTargetData(target));
                logData.Targets.Add(targetDto);
            }
            //
            log.UpdateProgressWithCancellationCheck("HTML: building Skill/Buff dictionaries");
            Dictionary<string, List<Buff>> persBuffDict = BuildPersonalBoonData(logData.PersBuffs);
            Dictionary<string, List<DamageModifier>> persDamageModDict = BuildPersonalDamageModData(logData.DmgModifiersPers);
            var allDamageMods = new HashSet<string>();
            foreach (Player p in log.PlayerList)
            {
                allDamageMods.UnionWith(p.GetPresentDamageModifier(log));
            }
            var commonDamageModifiers = new List<DamageModifier>();
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(GeneralHelper.Source.Common, out List<DamageModifier> list))
            {
                foreach (DamageModifier dMod in list)
                {
                    if (allDamageMods.Contains(dMod.Name))
                    {
                        commonDamageModifiers.Add(dMod);
                        logData.DmgModifiersCommon.Add(dMod.Name.GetHashCode());
                        _usedDamageMods.Add(dMod);
                    }
                }
            }
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(GeneralHelper.Source.FightSpecific,out list))
            {
                foreach (DamageModifier dMod in list)
                {
                    if (allDamageMods.Contains(dMod.Name))
                    {
                        commonDamageModifiers.Add(dMod);
                        logData.DmgModifiersCommon.Add(dMod.Name.GetHashCode());
                        _usedDamageMods.Add(dMod);
                    }
                }
            }
            var itemDamageModifiers = new List<DamageModifier>();
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(GeneralHelper.Source.Item, out list))
            {
                foreach (DamageModifier dMod in list)
                {
                    if (allDamageMods.Contains(dMod.Name))
                    {
                        itemDamageModifiers.Add(dMod);
                        logData.DmgModifiersItem.Add(dMod.Name.GetHashCode());
                        _usedDamageMods.Add(dMod);
                    }
                }
            }
            foreach (Buff boon in statistics.PresentBoons)
            {
                logData.Boons.Add(boon.ID);
                _usedBuffs[boon.ID] = boon;
            }
            foreach (Buff boon in statistics.PresentConditions)
            {
                logData.Conditions.Add(boon.ID);
                _usedBuffs[boon.ID] = boon;
            }
            foreach (Buff boon in statistics.PresentOffbuffs)
            {
                logData.OffBuffs.Add(boon.ID);
                _usedBuffs[boon.ID] = boon;
            }
            foreach (Buff boon in statistics.PresentDefbuffs)
            {
                logData.DefBuffs.Add(boon.ID);
                _usedBuffs[boon.ID] = boon;
            }
            foreach (Buff boon in statistics.PresentFractalInstabilities)
            {
                logData.FractalInstabilities.Add(boon.ID);
                _usedBuffs[boon.ID] = boon;
            }
            //
            log.UpdateProgressWithCancellationCheck("HTML: building Phases");
            for (int i = 0; i < _phases.Count; i++)
            {
                PhaseData phaseData = _phases[i];
                var phaseDto = new PhaseDto(phaseData, _phases, log)
                {
                    DpsStats = BuildDPSData(i),
                    DpsStatsTargets = BuildDPSTargetsData(i),
                    DmgStatsTargets = BuildDMGStatsTargetsData(i),
                    DmgStats = BuildDMGStatsData(i),
                    DefStats = BuildDefenseData(i),
                    SupportStats = BuildSupportData(i),
                    //
                    BoonStats = BuffData.BuildBuffUptimeData(log, statistics.PresentBoons, i),
                    OffBuffStats = BuffData.BuildBuffUptimeData(log, statistics.PresentOffbuffs, i),
                    DefBuffStats = BuffData.BuildBuffUptimeData(log, statistics.PresentDefbuffs, i),
                    PersBuffStats = BuffData.BuildPersonalBuffUptimeData(log, persBuffDict, i),
                    BoonGenSelfStats = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Self),
                    BoonGenGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Group),
                    BoonGenOGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.OffGroup),
                    BoonGenSquadStats = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Squad),
                    OffBuffGenSelfStats = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Self),
                    OffBuffGenGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Group),
                    OffBuffGenOGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.OffGroup),
                    OffBuffGenSquadStats = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Squad),
                    DefBuffGenSelfStats = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Self),
                    DefBuffGenGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Group),
                    DefBuffGenOGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.OffGroup),
                    DefBuffGenSquadStats = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Squad),
                    //
                    BoonActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentBoons, i),
                    OffBuffActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentOffbuffs, i),
                    DefBuffActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentDefbuffs, i),
                    PersBuffActiveStats = BuffData.BuildActivePersonalBuffUptimeData(log, persBuffDict, i),
                    BoonGenActiveSelfStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Self),
                    BoonGenActiveGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Group),
                    BoonGenActiveOGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.OffGroup),
                    BoonGenActiveSquadStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Squad),
                    OffBuffGenActiveSelfStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Self),
                    OffBuffGenActiveGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Group),
                    OffBuffGenActiveOGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.OffGroup),
                    OffBuffGenActiveSquadStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Squad),
                    DefBuffGenActiveSelfStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Self),
                    DefBuffGenActiveGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Group),
                    DefBuffGenActiveOGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.OffGroup),
                    DefBuffGenActiveSquadStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Squad),
                    //
                    DmgModifiersCommon = DamageModData.BuildDmgModifiersData(log, i, commonDamageModifiers),
                    DmgModifiersItem = DamageModData.BuildDmgModifiersData(log, i, itemDamageModifiers),
                    DmgModifiersPers = DamageModData.BuildPersonalDmgModifiersData(log, i, persDamageModDict),
                    TargetsCondiStats = new List<List<BuffData>>(),
                    TargetsCondiTotals = new List<BuffData>(),
                    TargetsBoonTotals = new List<BuffData>(),
                    MechanicStats = MechanicDto.BuildPlayerMechanicData(log, i),
                    EnemyMechanicStats = MechanicDto.BuildEnemyMechanicData(log, i)
                };
                foreach (NPC target in phaseData.Targets)
                {
                    phaseDto.TargetsCondiStats.Add(BuffData.BuildTargetCondiData(log, i, target));
                    phaseDto.TargetsCondiTotals.Add(BuffData.BuildTargetCondiUptimeData(log, i, target));
                    phaseDto.TargetsBoonTotals.Add(HasBoons(i, target) ? BuffData.BuildTargetBoonData(log, i, target) : null);
                }
                logData.Phases.Add(phaseDto);
            }
            //
            log.UpdateProgressWithCancellationCheck("HTML: building Meta Data");
            logData.EncounterDuration = log.FightData.DurationString;
            logData.Success = log.FightData.Success;
            logData.Wvw = log.FightData.Logic.Mode == FightLogic.ParseMode.WvW;
            logData.Targetless = log.FightData.Logic.Targetless;
            logData.FightName = log.FightData.GetFightName(log);
            logData.FightIcon = log.FightData.Logic.Icon;
            logData.LightTheme = _light;
            logData.SingleGroup = log.PlayerList.Where(x => !x.IsFakeActor).Select(x => x.Group).Distinct().Count() == 1;
            logData.NoMechanics = log.FightData.Logic.HasNoFightSpecificMechanics;
            if (log.LogData.LogErrors.Count > 0)
            {
                logData.LogErrors = new List<string>(log.LogData.LogErrors);
            }
            //
            SkillDto.AssembleSkills(_usedSkills.Values, logData.SkillMap);
            DamageModDto.AssembleDamageModifiers(_usedDamageMods, logData.DamageModMap);
            BuffDto.AssembleBoons(_usedBuffs.Values, logData.BuffMap, log);
            MechanicDto.BuildMechanics(log.MechanicData.GetPresentMechanics(log, 0), logData.MechanicMap);
            return ToJson(logData);
        }

        private bool HasBoons(int phaseIndex, NPC target)
        {
            Dictionary<long, FinalBuffs> conditions = target.GetBuffs(_log, phaseIndex);
            foreach (Buff boon in _statistics.PresentBoons)
            {
                if (conditions.TryGetValue(boon.ID, out FinalBuffs uptime))
                {
                    if (uptime.Uptime > 0.0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private ActorDetailsDto BuildPlayerData(Player player)
        {
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>(),
                DmgDistributionsTargets = new List<List<DmgDistributionDto>>(),
                DmgDistributionsTaken = new List<DmgDistributionDto>(),
                BoonGraph = new List<List<BuffChartDataDto>>(),
                Rotation = new List<List<object[]>>(),
                Food = FoodDto.BuildPlayerFoodData(_log, player, _usedBuffs),
                Minions = new List<ActorDetailsDto>(),
                DeathRecap = DeathRecapDto.BuildDeathRecap(_log, player)
            };
            for (int i = 0; i < _phases.Count; i++)
            {
                dto.Rotation.Add(SkillDto.BuildRotationData(_log, player, i, _usedSkills));
                dto.DmgDistributions.Add(DmgDistributionDto.BuildPlayerDMGDistData(_log, player, null, i, _usedSkills, _usedBuffs));
                var dmgTargetsDto = new List<DmgDistributionDto>();
                foreach (NPC target in _phases[i].Targets)
                {
                    dmgTargetsDto.Add(DmgDistributionDto.BuildPlayerDMGDistData(_log, player, target, i, _usedSkills, _usedBuffs));
                }
                dto.DmgDistributionsTargets.Add(dmgTargetsDto);
                dto.DmgDistributionsTaken.Add(DmgDistributionDto.BuildDMGTakenDistData(_log, player, i, _usedSkills, _usedBuffs));
                dto.BoonGraph.Add(BuffChartDataDto.BuildBoonGraphData(_log, player, i, _usedBuffs));
            }
            foreach (KeyValuePair<long, Minions> pair in player.GetMinions(_log))
            {
                dto.Minions.Add(BuildPlayerMinionsData(player, pair.Value));
            }

            return dto;
        }

        private ActorDetailsDto BuildPlayerMinionsData(Player player, Minions minion)
        {
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>(),
                DmgDistributionsTargets = new List<List<DmgDistributionDto>>()
            };
            for (int i = 0; i < _phases.Count; i++)
            {
                var dmgTargetsDto = new List<DmgDistributionDto>();
                foreach (NPC target in _phases[i].Targets)
                {
                    dmgTargetsDto.Add(DmgDistributionDto.BuildPlayerMinionDMGDistData(_log, player, minion, target, i, _usedSkills, _usedBuffs));
                }
                dto.DmgDistributionsTargets.Add(dmgTargetsDto);
                dto.DmgDistributions.Add(DmgDistributionDto.BuildPlayerMinionDMGDistData(_log, player, minion, null, i, _usedSkills, _usedBuffs));
            }
            return dto;
        }

        private ActorDetailsDto BuildTargetData(NPC target)
        {
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>(),
                DmgDistributionsTaken = new List<DmgDistributionDto>(),
                BoonGraph = new List<List<BuffChartDataDto>>(),
                Rotation = new List<List<object[]>>()
            };
            for (int i = 0; i < _phases.Count; i++)
            {
                if (_phases[i].Targets.Contains(target) || (i == 0 && _cr))
                {
                    dto.DmgDistributions.Add(DmgDistributionDto.BuildTargetDMGDistData(_log, target, i, _usedSkills, _usedBuffs));
                    dto.DmgDistributionsTaken.Add(DmgDistributionDto.BuildDMGTakenDistData(_log, target, i, _usedSkills, _usedBuffs));
                    dto.Rotation.Add(SkillDto.BuildRotationData(_log, target, i, _usedSkills));
                    dto.BoonGraph.Add(BuffChartDataDto.BuildBoonGraphData(_log, target, i, _usedBuffs));
                }
                else
                {
                    dto.DmgDistributions.Add(new DmgDistributionDto());
                    dto.DmgDistributionsTaken.Add(new DmgDistributionDto());
                    dto.Rotation.Add(new List<object[]>());
                    dto.BoonGraph.Add(new List<BuffChartDataDto>());
                }
            }

            dto.Minions = new List<ActorDetailsDto>();
            foreach (KeyValuePair<long, Minions> pair in target.GetMinions(_log))
            {
                dto.Minions.Add(BuildTargetsMinionsData(target, pair.Value));
            }
            return dto;
        }

        private ActorDetailsDto BuildTargetsMinionsData(NPC target, Minions minion)
        {
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>()
            };
            for (int i = 0; i < _phases.Count; i++)
            {
                if (_phases[i].Targets.Contains(target) || (i == 0 && _cr))
                {
                    dto.DmgDistributions.Add(DmgDistributionDto.BuildTargetMinionDMGDistData(_log, target, minion, i, _usedSkills, _usedBuffs));
                }
                else
                {
                    dto.DmgDistributions.Add(new DmgDistributionDto());
                }
            }
            return dto;
        }

        private static string ToJson(object value)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = contractResolver
            };
            return JsonConvert.SerializeObject(value, settings);
        }
    }
}
