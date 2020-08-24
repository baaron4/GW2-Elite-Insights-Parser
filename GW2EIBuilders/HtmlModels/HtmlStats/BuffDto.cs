using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels
{
    public class BuffDto
    {
        public long Id { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; } = null;
        public string Icon { get; internal set; }
        public bool Stacking { get; internal set; }
        public bool Consumable { get; internal set; }
        public bool FightSpecific { get; internal set; }

        internal BuffDto(Buff buff, ParsedEvtcLog log)
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

        internal static void AssembleBoons(ICollection<Buff> buffs, Dictionary<string, BuffDto> dict, ParsedEvtcLog log)
        {
            foreach (Buff buff in buffs)
            {
                dict["b" + buff.ID] = new BuffDto(buff, log);
            }
        }
    }
}
