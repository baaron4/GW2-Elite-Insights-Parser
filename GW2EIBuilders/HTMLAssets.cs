using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace GW2EIBuilders
{
    public class HTMLAssets
    {
        internal string EIJavascriptCode { get; }

        internal string EICRJavascriptCode { get; }

        public HTMLAssets()
        {
            var orderedScripts = new List<string>()
            {
                Properties.Resources.global,
                Properties.Resources.mixins,
                Properties.Resources.functions,
                Properties.Resources.main_js,
            };
            string scriptContent = orderedScripts[0];
            for (int i = 1; i < orderedScripts.Count; i++)
            {
                scriptContent += orderedScripts[i];
            }
            List<string> templates = BuildTemplates();
            templates.AddRange(BuildCRTemplates());
            EIJavascriptCode = scriptContent.Replace("TEMPLATE_COMPILE", string.Join("\n", templates));
            //
            var orderedCRScripts = new List<string>()
            {
                Properties.Resources.animator,
                Properties.Resources.actors,
                Properties.Resources.decorations,
            };
            string scriptCRContent = orderedCRScripts[0];
            for (int i = 1; i < orderedCRScripts.Count; i++)
            {
                scriptCRContent += orderedCRScripts[i];
            }
            EICRJavascriptCode = scriptCRContent;
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
    }
}
