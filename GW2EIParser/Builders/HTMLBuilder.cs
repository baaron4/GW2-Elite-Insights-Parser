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

        private static string _eiJS;

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

        private static string PrepareTemplate(string template)
        {
            if (!template.Contains("<template>") || !template.Contains("<script>") || !template.Contains("${template}"))
            {
                throw new InvalidDataException("Not a template");
            }
            string html = template.Split(new string[] { "<template>" }, StringSplitOptions.None)[1].Split(new string[] { "</template>" }, StringSplitOptions.None)[0];
            string js = template.Split(new string[] { "<script>" }, StringSplitOptions.None)[1].Split(new string[] { "</script>" }, StringSplitOptions.None)[0];
            js = js.Replace("${template}", Regex.Replace(html, @"\t|\n|\r", ""));
            js = "{" + js + "}";
            return js;
        }

        private static List<string> BuildTemplates()
        {
            var templates = new List<string>
            {
                Properties.Resources.tmplBuffStats,
                Properties.Resources.tmplBuffStatsTarget,
                Properties.Resources.tmplBuffTable,
                Properties.Resources.tmplDamageDistPlayer,
                Properties.Resources.tmplDamageDistTable,
                Properties.Resources.tmplDamageDistTarget,
                Properties.Resources.tmplDamageModifierTable,
                Properties.Resources.tmplDamageModifierStats,
                Properties.Resources.tmplDamageModifierPersStats,
                Properties.Resources.tmplDamageTable,
                Properties.Resources.tmplDamageTaken,
                Properties.Resources.tmplDeathRecap,
                Properties.Resources.tmplDefenseTable,
                Properties.Resources.tmplEncounter,
                Properties.Resources.tmplFood,
                Properties.Resources.tmplGameplayTable,
                Properties.Resources.tmplGeneralLayout,
                Properties.Resources.tmplMechanicsTable,
                Properties.Resources.tmplPersonalBuffTable,
                Properties.Resources.tmplPhase,
                Properties.Resources.tmplPlayers,
                Properties.Resources.tmplPlayerStats,
                Properties.Resources.tmplPlayerTab,
                Properties.Resources.tmplSimpleRotation,
                Properties.Resources.tmplSupportTable,
                Properties.Resources.tmplTargets,
                Properties.Resources.tmplTargetStats,
                Properties.Resources.tmplTargetTab,
                Properties.Resources.tmplDPSGraph,
                Properties.Resources.tmplGraphStats,
                Properties.Resources.tmplPlayerTabGraph,
                Properties.Resources.tmplRotationLegend,
                Properties.Resources.tmplTargetTabGraph,
                Properties.Resources.tmplTargetData,
                Properties.Resources.tmplMainView,
            };
            var res = new List<string>();
            foreach (string template in templates)
            {
                res.Add(PrepareTemplate(template));
            }
            return res;
        }

        private static List<string> BuildCRTemplates()
        {
            var templates = new List<string>
            {
                Properties.Resources.tmplCombatReplayDamageData,
                Properties.Resources.tmplCombatReplayStatusData,
                Properties.Resources.tmplCombatReplayDamageTable,
                Properties.Resources.tmplCombatReplayActorBuffStats,
                Properties.Resources.tmplCombatReplayPlayerStats,
                Properties.Resources.tmplCombatReplayPlayerStatus,
                Properties.Resources.tmplCombatReplayActorRotation,
                Properties.Resources.tmplCombatReplayTargetStats,
                Properties.Resources.tmplCombatReplayTargetStatus,
                Properties.Resources.tmplCombatReplayTargetsStats,
                Properties.Resources.tmplCombatReplayPlayersStats,
                Properties.Resources.tmplCombatReplayUI,
                Properties.Resources.tmplCombatReplayPlayerSelect,
                Properties.Resources.tmplCombatReplayExtraDecorations,
                Properties.Resources.tmplCombatReplayAnimationControl,
                Properties.Resources.tmplCombatReplayMechanicsList
            };
            var res = new List<string>();
            foreach (string template in templates)
            {
                res.Add(PrepareTemplate(template));
            }
            return res;
        }

        public static void InitScripts()
        {
            var orderedScripts = new List<string>()
            {
                Properties.Resources.globalJS,
                Properties.Resources.mixinsJS,
                Properties.Resources.functionsJS,
                Properties.Resources.ei_js
            };
            string scriptContent = orderedScripts[0];
            for (int i = 1; i < orderedScripts.Count; i++)
            {
                scriptContent += orderedScripts[i];
            }
            List<string> templates = BuildTemplates();
            templates.AddRange(BuildCRTemplates());
            _eiJS = scriptContent.Replace("TEMPLATE_COMPILE", string.Join("\n", templates));
        }

        /// <summary>
        /// Create the damage taken distribution table for a given player
        /// </summary>
        /// <param name="p"></param>
        /// <param name="phaseIndex"></param>

        public void CreateHTML(StreamWriter sw, string path)
        {
            string html = Properties.Resources.template_html;
            _log.UpdateProgressWithCancellationCheck("HTML: replacing global variables");
            html = html.Replace("${bootstrapTheme}", !_light ? "slate" : "yeti");

            _log.UpdateProgressWithCancellationCheck("HTML: building CSS");
            html = html.Replace("<!--${Css}-->", BuildCss(path));
            _log.UpdateProgressWithCancellationCheck("HTML: building JS");
            html = html.Replace("<!--${Js}-->", BuildEIJs(path));
            _log.UpdateProgressWithCancellationCheck("HTML: building Combat Replay JS");
            html = html.Replace("<!--${CombatReplayJS}-->", BuildCombatReplayJS(path));

            html = html.Replace("'${logDataJson}'", ToJson(LogDataDto.BuildLogData(_log, _usedSkills, _usedBuffs, _usedDamageMods, _cr, _light, _uploadLink)));

            _log.UpdateProgressWithCancellationCheck("HTML: building Graph Data");
            html = html.Replace("'${graphDataJson}'", ToJson(ChartDataDto.BuildChartData(_log)));

            sw.Write(html);
            return;
        }

        private string BuildCombatReplayJS(string path)
        {
            if (!_cr)
            {
                return "";
            }
            string scriptContent = Properties.Resources.combatreplay_js;
            if (_externalScripts && path != null)
            {
                string jsFileName = "EliteInsights-CR-" + _scriptVersion + ".js";
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

        private string BuildCss(string path)
        {
            string scriptContent = Properties.Resources.ei_css;

            if (_externalScripts && path != null)
            {
                string cssFilename = "EliteInsights-" + _scriptVersion + ".css";
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
            string scriptContent = _eiJS;

            if (_externalScripts && path != null)
            {
                string scriptFilename = "EliteInsights-" + _scriptVersion + ".js";
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
