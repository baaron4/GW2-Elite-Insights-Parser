﻿using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels
{
    internal class BuffDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = null;
        public string Icon { get; set; }
        public bool Stacking { get; set; }
        public bool Consumable { get; set; }
        public bool FightSpecific { get; set; }

        public BuffDto(Buff buff, ParsedEvtcLog log)
        {
            Id = buff.ID;
            Name = buff.Name;
            Icon = buff.Link;
            Stacking = (buff.Type == Buff.BuffType.Intensity);
            Consumable = (buff.Nature == Buff.BuffNature.Consumable);
            FightSpecific = (buff.Source == ParserHelper.Source.FightSpecific || buff.Source == ParserHelper.Source.FractalInstability);
            BuffInfoEvent buffInfoEvent = log.CombatData.GetBuffInfoEvent(buff.ID);
            if (buffInfoEvent != null)
            {
                var descriptions = new List<string>() { 
                    "Max Stack(s) " + buff.Capacity
                };
                foreach (BuffFormula formula in buffInfoEvent.Formulas)
                {
                    if (formula.TraitSelf > 0 || formula.TraitSrc > 0)
                    {
                        continue;
                    }
                    var desc = formula.GetDescription(false, log.Buffs.BuffsByIds);
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
