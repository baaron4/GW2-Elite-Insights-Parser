using System.Collections.Generic;
using System.Security.Cryptography;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Builders.HtmlModels
{
    public class BuffDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = null;
        public string Icon { get; set; }
        public bool Stacking { get; set; }
        public bool Consumable { get; set; }
        public bool FightSpecific { get; set; }

        public BuffDto(Buff buff, ParsedLog log)
        {
            Id = buff.ID;
            Name = buff.Name;
            Icon = buff.Link;
            Stacking = (buff.Type == Buff.BuffType.Intensity);
            Consumable = (buff.Nature == Buff.BuffNature.Consumable);
            FightSpecific = (buff.Source == GeneralHelper.Source.FightSpecific);
            BuffInfoEvent buffInfoEvent = log.CombatData.GetBuffInfoEvent(buff.ID);
            if (buffInfoEvent != null)
            {
                var descriptions = new List<string>();
                foreach (BuffInfoEvent.BuffFormula formula in buffInfoEvent.FormulaList)
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
                if (descriptions.Count > 0)
                {
                    Description = "";
                    foreach (string desc in descriptions)
                    {
                        Description += desc + "<br>";
                    }
                }
            }
        }

        public static void AssembleBoons(ICollection<Buff> buffs, Dictionary<string, BuffDto> dict, ParsedLog log)
        {
            foreach (Buff buff in buffs)
            {
                dict["b" + buff.ID] = new BuffDto(buff, log);
            }
        }
    }
}
