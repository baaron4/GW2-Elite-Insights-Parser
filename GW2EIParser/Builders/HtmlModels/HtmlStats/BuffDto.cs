using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Builders.HtmlModels
{
    public class BuffDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<string> Descriptions { get; set; }
        public string Icon { get; set; }
        public bool Stacking { get; set; }
        public bool Consumable { get; set; }
        public bool FightSpecific { get; set; }

        public BuffDto(Buff buff, CombatData combatData)
        {
            Id = buff.ID;
            Name = buff.Name;
            Icon = buff.Link;
            Stacking = (buff.Type == Buff.BuffType.Intensity);
            Consumable = (buff.Nature == Buff.BuffNature.Consumable);
            FightSpecific = (buff.Source == GeneralHelper.Source.FightSpecific);
            BuffInfoEvent buffDataEvent = combatData.GetBuffInfoEvent(buff.ID);
            if (buffDataEvent != null)
            {
                var descriptions = new List<string>();
            }
        }

        public static void AssembleBoons(ICollection<Buff> buffs, Dictionary<string, BuffDto> dict, CombatData combatData)
        {
            foreach (Buff buff in buffs)
            {
                dict["b" + buff.ID] = new BuffDto(buff, combatData);
            }
        }
    }
}
