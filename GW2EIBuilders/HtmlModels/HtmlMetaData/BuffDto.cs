using System;
using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.HTMLMetaData
{
    internal class BuffDto : AbstractSkillDto
    {
        public string Description { get; set; } = null;
        public bool Stacking { get; set; }
        public bool Consumable { get; set; }
        public bool FightSpecific { get; set; }

        public BuffDto(Buff buff, ParsedEvtcLog log) : base(buff, log)
        {
            Stacking = (buff.Type == Buff.BuffType.Intensity);
            Consumable = (buff.Classification == Buff.BuffClassification.Nourishment || buff.Classification == Buff.BuffClassification.Enhancement || buff.Classification == Buff.BuffClassification.OtherConsumable);
            FightSpecific = (buff.Source == ParserHelper.Source.FightSpecific || buff.Source == ParserHelper.Source.FractalInstability);
            BuffInfoEvent buffInfoEvent = log.CombatData.GetBuffInfoEvent(buff.ID);
            if (buffInfoEvent != null)
            {
                var descriptions = new List<string>() {
                    "ID: " + buff.ID,
                    "Max Stack(s) " + buffInfoEvent.MaxStacks
                };
                if (buffInfoEvent.DurationCap > 0)
                {
                    descriptions.Add("Duration Cap: " + Math.Round(buffInfoEvent.DurationCap / 1000.0, 3) + " seconds");
                }
                foreach (BuffFormula formula in buffInfoEvent.Formulas)
                {
                    if (formula.IsConditional)
                    {
                        continue;
                    }
                    string desc = formula.GetDescription(false, log.Buffs.BuffsByIds, buff);
                    if (desc.Length > 0)
                    {
                        descriptions.Add(desc);
                    }
                }
                Description = "";
                foreach (string desc in descriptions)
                {
                    Description += desc + "<br>";
                }
            }
        }

        public static void AssembleBoons(ICollection<Buff> buffs, Dictionary<string, BuffDto> dict, ParsedEvtcLog log)
        {
            foreach (Buff buff in buffs)
            {
                dict["b" + buff.ID] = new BuffDto(buff, log);
            }
        }
    }
}
