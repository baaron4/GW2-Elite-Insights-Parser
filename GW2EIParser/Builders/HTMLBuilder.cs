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
        private readonly bool _cr;
        private readonly bool _light;
        private readonly bool _externalScripts;

        private readonly string[] _uploadLink;

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

            _uploadLink = uploadString ?? new string[] { "", "", "" };

            _cr = _log.CanCombatReplay;
            _light = lightTheme;
            _externalScripts = externalScript;
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

            html = html.Replace("'${logDataJson}'", ToJson(LogDataDto.BuildLogData(_log, _usedSkills, _usedBuffs, _usedDamageMods, _cr, _light)));
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
