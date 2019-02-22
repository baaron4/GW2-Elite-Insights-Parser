using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.Statistics;

namespace LuckParser.Models.ParseModels
{
    public abstract class DamageModifier
    {
        public enum DamageType { All, Power, Condition};
        public enum ModifierSource { All, Necromancer, Elementalist, Mesmer, Warrior, Revenant, Guardian, Thief, Ranger, Engineer};
        public static ModifierSource ProfToEnum(string prof)
        {
            switch (prof)
            {
                case "Druid":
                case "Ranger":
                case "Soulbeast":
                    return ModifierSource.Ranger;
                case "Scrapper":
                case "Holosmith":
                case "Engineer":
                    return ModifierSource.Engineer;
                case "Daredevil":
                case "Deadeye":
                case "Thief":
                    return ModifierSource.Thief;
                case "Weaver":
                case "Tempest":
                case "Elementalist":
                    return ModifierSource.Elementalist;
                case "Mirage":
                case "Chronomancer":
                case "Mesmer":
                    return ModifierSource.Mesmer;
                case "Scourge":
                case "Reaper":
                case "Necromancer":
                    return ModifierSource.Necromancer;
                case "Spellbreaker":
                case "Berserker":
                case "Warrior":
                    return ModifierSource.Warrior;
                case "Firebrand":
                case "Dragonhunter":
                case "Guardian":
                    return ModifierSource.Guardian;
                case "Renegade":
                case "Herald":
                case "Revenant":
                    return ModifierSource.Revenant;
            }
            return ModifierSource.All;
        }

        private DamageType _compareType { get; }
        private DamageType _srcType { get; }
        protected bool Multiplier { get; }
        protected double GainPerStack { get; }
        public ModifierSource Src { get; }
        public long ID { get; }
        public string Url { get; }
        public string Name { get; }

        private static readonly List<DamageModifier> _allDamageModifier = new List<DamageModifier>() {
            new DamageLogDamageModifier(Boon.ScholarRune,"Scholar Rune", true, 5.0,"https://wiki.guildwars2.com/images/2/2b/Superior_Rune_of_the_Scholar.png", x => x.IsNinety ),
            new DamageLogDamageModifier(Boon.EagleRune, "Eagle Rune", true, 10.0,"https://wiki.guildwars2.com/images/9/9b/Superior_Rune_of_the_Eagle.png", x => x.IsFifty ),
            new DamageLogDamageModifier(Boon.MovingBuff, "Moving", true, 5.0,"https://wiki.guildwars2.com/images/1/1c/Bowl_of_Seaweed_Salad.png", x => x.IsMoving ),
            new DamageLogDamageModifier(Boon.ThiefRune, "Thief Rune", true, 10.0,"https://wiki.guildwars2.com/images/9/96/Superior_Rune_of_the_Thief.png", x => x.IsFlanking ),
        };

        public static Dictionary<ModifierSource, List<DamageModifier>> DamageModifiersPerSource = _allDamageModifier.GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());

        protected DamageModifier(long id, string name, bool multiplier, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, string url)
        {
            ID = id;
            Name = name;
            Multiplier = multiplier;
            GainPerStack = gainPerStack;
            _compareType = compareType;
            _srcType = srctype;
            Src = src;
            Url = url;
        }

        protected (int total, int count)  GetTotalDamageData(Player p, ParsedLog log, Target t, PhaseData phase)
        {
            List<DamageLog> dls = new List<DamageLog>();
            switch (_compareType)
            {
                case DamageType.All:
                    dls = p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End);
                    break;
                case DamageType.Condition:
                    dls = p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End).Where(x => x.IsIndirectDamage).ToList();
                    break;
                case DamageType.Power:
                    dls = p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End).Where(x => !x.IsIndirectDamage).ToList();
                    break;
            }
            return (dls.Sum(x => x.Damage), dls.Count);
        }

        protected List<DamageLog> GetDamageLogs(Player p, ParsedLog log, Target t, PhaseData phase)
        {
            switch (_srcType)
            {
                case DamageType.All:
                    return p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End);
                case DamageType.Condition:
                    return p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End).Where(x => x.IsIndirectDamage).ToList();
                case DamageType.Power:
                default:
                    return p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End).Where(x => !x.IsIndirectDamage).ToList();
            }
        }

        public abstract void ComputeDamageModifier(Dictionary<long, List<ExtraBoonData>> data, Dictionary<Target, Dictionary<long, List<ExtraBoonData>>> dataTarget, Player p, ParsedLog log);
    }
}
