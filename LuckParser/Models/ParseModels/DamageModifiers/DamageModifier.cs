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
        public enum DamageSource { All, NoPets };
        public enum ModifierSource { Shareable, Item, Necromancer, Elementalist, Mesmer, Warrior, Revenant, Guardian, Thief, Ranger, Engineer};
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
            return ModifierSource.Shareable;
        }

        private DamageType _compareType { get; }
        private DamageType _srcType { get; }
        private DamageSource _dmgSrc { get; }
        protected double GainPerStack { get; }
        protected readonly GainComputer GainComputer;
        public ModifierSource Src { get; }
        public string Url { get; }
        public string Name { get; }
        public delegate bool DamageLogChecker(DamageLog dl);
        protected DamageLogChecker DLChecker;

        protected static readonly GainComputer ByPresence = new GainComputerByPresence();
        protected static readonly GainComputer ByStack = new GainComputerByStack();
        protected static readonly GainComputer ByAbsence = new GainComputerByAbsence();

        private static readonly List<DamageModifier> _allDamageModifier = new List<DamageModifier>() {
            // Gear
            new DamageLogDamageModifier("Scholar Rune", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.Item,"https://wiki.guildwars2.com/images/2/2b/Superior_Rune_of_the_Scholar.png", x => x.IsNinety, ByPresence ),
            new DamageLogDamageModifier("Eagle Rune", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Item,"https://wiki.guildwars2.com/images/9/9b/Superior_Rune_of_the_Eagle.png", x => x.IsFifty, ByPresence ),
            new DamageLogDamageModifier("Thief Rune", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Item,"https://wiki.guildwars2.com/images/9/96/Superior_Rune_of_the_Thief.png", x => x.IsFlanking , ByPresence),
            new BuffDamageModifier(Boon.BoonsByName["Might"], "Strength Rune",  DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.Item, ByPresence, "https://wiki.guildwars2.com/images/2/2b/Superior_Rune_of_Strength.png"),
            new BuffDamageModifier(Boon.BoonsByName["Fire Aura"], "Fire Rune",  DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Item, ByPresence, "https://wiki.guildwars2.com/images/4/4a/Superior_Rune_of_the_Fire.png"),
            new BuffDamageModifierTarget(Boon.BoonsByName["Burning"], "Flame Legion Rune",  DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ModifierSource.Item, ByPresence, "https://wiki.guildwars2.com/images/4/4a/Superior_Rune_of_the_Flame_Legion.png"),
            new BuffDamageModifierTarget(Boon.BoonsByName["Number of Boons"], "Spellbreaker Rune",  DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ModifierSource.Item, ByAbsence, "https://wiki.guildwars2.com/images/1/1a/Superior_Rune_of_the_Spellbreaker.png"),
            new BuffDamageModifierTarget(Boon.BoonsByName["Chilled"], "Ice Rune",  DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ModifierSource.Item, ByPresence, "https://wiki.guildwars2.com/images/7/78/Superior_Rune_of_the_Ice.png"),
            new BuffDamageModifier(Boon.BoonsByName["Fury"], "Rage Rune",  DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.Item, ByPresence, "https://wiki.guildwars2.com/images/9/9e/Superior_Rune_of_Rage.png"),
            new BuffDamageModifier(Boon.BoonsByName["Fractal Offensive"],  DamageSource.NoPets, 3.0, DamageType.All, DamageType.All, ModifierSource.Item, ByStack),
            new BuffDamageModifier(Boon.BoonsByName["Bowl of Seaweed Salad"], DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.Item, ByPresence, x => x.IsMoving),
            /// commons
            new BuffDamageModifier(Boon.BoonsByName["Vulnerability"], DamageSource.All, 1.0, DamageType.All, DamageType.All, ModifierSource.Shareable, ByStack),
            new BuffDamageModifier(Boon.BoonsByName["Frost Spirit"], DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.Shareable, ByPresence),
            new DamageLogDamageModifier(Boon.BoonsByName["Soulcleave's Summit"], 0, DamageSource.NoPets, DamageType.Power, DamageType.Power, ModifierSource.Shareable, ByPresence),
            new DamageLogDamageModifier(Boon.BoonsByName["One Wolf Pack"], 0, DamageSource.NoPets, DamageType.Power, DamageType.Power, ModifierSource.Shareable, ByPresence),
            new DamageLogDamageModifier(Boon.BoonsByName["Static Charge"], 0, DamageSource.NoPets, DamageType.Power, DamageType.Power, ModifierSource.Shareable, ByPresence),
            //new BuffDamageModifier(Boon.BoonsByName["Glyph of Empowerment"], DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.All, _nonMultiplier),
            new BuffDamageModifierTarget(Boon.BoonsByName["Unnatural Signet"], DamageSource.All, 200.0, DamageType.Power, DamageType.Power, ModifierSource.Shareable, ByPresence),
            new BuffDamageModifierTarget(Boon.BoonsByName["Compromised"], DamageSource.All, 75.0, DamageType.Power, DamageType.Power, ModifierSource.Shareable, ByStack),
            new BuffDamageModifierTarget(Boon.BoonsByName["Fractured - Enemy"], DamageSource.All, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Shareable, ByStack),
            new BuffDamageModifier(Boon.BoonsByName["Blood Fueled"], DamageSource.NoPets, 20.0, DamageType.Power, DamageType.Power, ModifierSource.Shareable, ByStack),
            new BuffDamageModifier(Boon.BoonsByName["Blood Fueled Abo"], DamageSource.NoPets, 20.0, DamageType.Power, DamageType.Power, ModifierSource.Shareable, ByStack),
            /// Revenant
            new BuffDamageModifier(Boon.BoonsByName["Vicious Lacerations"], DamageSource.NoPets, 3.0, DamageType.Power, DamageType.Power, ModifierSource.Revenant, ByStack),
            new BuffDamageModifier(Boon.BoonsByName["Retaliation"], "Vicious Reprisal", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Revenant, ByPresence, "https://wiki.guildwars2.com/images/c/cf/Vicious_Reprisal.png"),
            new BuffDamageModifierTarget(Boon.BoonsByName["Weakness"], "Dwarven Battle Training", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Revenant, ByPresence, "https://wiki.guildwars2.com/images/5/50/Dwarven_Battle_Training.png"),
            new BuffDamageModifier(Boon.BoonsByName["Number of Boons"], "Reinforced Potency", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.Power, ModifierSource.Revenant, ByStack, "https://wiki.guildwars2.com/images/0/0a/Envoy_of_Sustenance.png"),
            new BuffDamageModifier(Boon.BoonsByName["Fury"], "Ferocious Aggression", DamageSource.NoPets, 7.0, DamageType.All, DamageType.All, ModifierSource.Revenant, ByPresence, "https://wiki.guildwars2.com/images/e/ec/Ferocious_Aggression.png"),
            new BuffDamageModifier(Boon.BoonsByName["Kalla's Fervor"], DamageSource.NoPets, 2.0, DamageType.Condition, DamageType.Condition, ModifierSource.Revenant, ByStack),
            new BuffDamageModifier(Boon.BoonsByName["Improved Kalla's Fervor"], DamageSource.NoPets, 3.0, DamageType.Condition, DamageType.Condition, ModifierSource.Revenant, ByStack),
            new BuffDamageModifierTarget(Boon.BoonsByName["Vulnerability"], "Targeted Destruction", DamageSource.NoPets, 0.5, DamageType.Power, DamageType.Power, ModifierSource.Revenant, ByStack, "https://wiki.guildwars2.com/images/e/ed/Targeted_Destruction.png"),
            new DamageLogDamageModifier("Swift Termination", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.Power, ModifierSource.Revenant,"https://wiki.guildwars2.com/images/b/bb/Swift_Termination.png", x => x.IsFifty, ByPresence),
            /// Warrior
            new BuffDamageModifier(Boon.BoonsByName["Peak Performance"], DamageSource.NoPets, 20.0, DamageType.Power, DamageType.Power, ModifierSource.Warrior, ByPresence),
            new BuffDamageModifier(Boon.BoonsByName["Always Angry"], DamageSource.NoPets, 7.0, DamageType.All, DamageType.All, ModifierSource.Warrior, ByStack),
            new BuffDamageModifierTarget(Boon.BoonsByName["Weakness"], "Cull the Weak", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ModifierSource.Warrior, ByPresence, "https://wiki.guildwars2.com/images/7/72/Cull_the_Weak.png"),
            new BuffDamageModifier(Boon.BoonsByName["Number of Boons"], "Empowered", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.Power, ModifierSource.Warrior, ByStack, "https://wiki.guildwars2.com/images/c/c2/Empowered.png"),
            new BuffDamageModifierTarget(Boon.BoonsByName["Number of Boons"], "Destruction of the Empowered", DamageSource.NoPets, 3.0, DamageType.Power, DamageType.Power, ModifierSource.Warrior, ByStack, "https://wiki.guildwars2.com/images/5/5c/Destruction_of_the_Empowered.png"),
            new BuffDamageModifierTarget(Boon.BoonsByName["Number of Boons"], "Pure Strike", DamageSource.NoPets, (1.14/1.07 * 100.0) - 100.0, DamageType.Power, DamageType.Power, ModifierSource.Warrior, ByAbsence, "https://wiki.guildwars2.com/images/7/76/Pure_Strike_%28trait%29.png"),
            // TO TRACK Berserker's Power
            /// Guardian
            new BuffDamageModifierTarget(Boon.BoonsByName["Burning"], "Fiery Wrath", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ModifierSource.Guardian, ByPresence, "https://wiki.guildwars2.com/images/7/70/Fiery_Wrath.png"),
            new BuffDamageModifier(Boon.BoonsByName["Retaliation"], "Retribution", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Guardian, ByStack, "https://wiki.guildwars2.com/images/d/d7/Retribution_%28trait%29.png"),
            new BuffDamageModifier(Boon.BoonsByName["Aegis"], "Unscathed Contender", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.Power, ModifierSource.Guardian, ByPresence, "https://wiki.guildwars2.com/images/b/b4/Unscathed_Contender.png"),
            new BuffDamageModifier(Boon.BoonsByName["Number of Boons"], "Power of the Virtuous", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.Power, ModifierSource.Guardian, ByStack, "https://wiki.guildwars2.com/images/b/b4/Unscathed_Contender.png"),
            new BuffDamageModifierTarget(Boon.BoonsByName["Crippled"], "Zealot's Aggression", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Guardian, ByPresence, "https://wiki.guildwars2.com/images/b/b4/Unscathed_Contender.png"),
            /// ENGINEER
            new BuffDamageModifier(Boon.BoonsByName["Laser's Edge"], DamageSource.NoPets, 15.0, DamageType.Power, DamageType.Power, ModifierSource.Engineer, ByPresence),
            new BuffDamageModifier(Boon.BoonsByName["Thermal Vision"], DamageSource.NoPets, 5.0, DamageType.Condition, DamageType.Condition, ModifierSource.Engineer, ByPresence),
            new BuffDamageModifier(Boon.BoonsByName["Vigor"], "Excessive Energy", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Engineer, ByPresence, "https://wiki.guildwars2.com/images/1/1f/Excessive_Energy.png"),
            new BuffDamageModifierTarget(Boon.BoonsByName["Vulnerability"], "Shaped Charge", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Engineer, ByPresence, "https://wiki.guildwars2.com/images/f/f3/Explosive_Powder.png"),
            new BuffDamageModifierTarget(Boon.BoonsByName["Number of Conditions"], "Modified Ammunition", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.Power, ModifierSource.Engineer, ByStack, "https://wiki.guildwars2.com/images/9/94/Modified_Ammunition.png"),
            /// RANGER
            new BuffDamageModifier(Boon.BoonsByName["Sic 'Em!"], DamageSource.NoPets, 40.0, DamageType.Power, DamageType.Power, ModifierSource.Ranger, ByPresence),
            new BuffDamageModifier(Boon.BoonsByName["Light on your Feet"], DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Ranger, ByPresence),
            new BuffDamageModifier(Boon.BoonsByName["Twice as Vicious"], DamageSource.NoPets, 5.0, DamageType.All, DamageType.All, ModifierSource.Ranger, ByPresence),
            new BuffDamageModifier(Boon.BoonsByName["Number of Boons"], "Bountiful Hunter", DamageSource.All, 1.0, DamageType.Power, DamageType.Power, ModifierSource.Ranger, ByStack, "https://wiki.guildwars2.com/images/2/25/Bountiful_Hunter.png"),
            new BuffDamageModifier(Boon.BoonsByName["Fury"], "Furious Strength", DamageSource.All, 7.0, DamageType.Power, DamageType.Power, ModifierSource.Ranger, ByStack, "https://wiki.guildwars2.com/images/c/ca/Furious_Strength.png"),
            // TODO Predator's Onslaught ? can daze and stun be tracked?
            /// THIEF
            new BuffDamageModifier(Boon.BoonsByName["Lead Attacks"], DamageSource.NoPets, 1.0, DamageType.All, DamageType.All, ModifierSource.Thief, ByStack),
            new BuffDamageModifier(Boon.BoonsByName["Lotus Training"], DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.Condition, ModifierSource.Thief, ByPresence),
            new BuffDamageModifier(Boon.BoonsByName["Bounding Dodger"], DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Thief, ByPresence),
            new BuffDamageModifierTarget(Boon.BoonsByName["Number of Conditions"], "Exposed Weakness", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.Power, ModifierSource.Thief, ByStack, "https://wiki.guildwars2.com/images/0/02/Exposed_Weakness.png"),
            new DamageLogDamageModifier("Executioner", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.Power, ModifierSource.Thief,"https://wiki.guildwars2.com/images/9/93/Executioner.png", x => x.IsFifty, ByPresence),
            new DamageLogDamageModifier("Ferocius Strikes", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Thief,"https://wiki.guildwars2.com/images/d/d1/Ferocious_Strikes.png", x => !x.IsFifty, ByPresence),
            //new BuffDamageModifierTarget(Boon.BoonsByName["Crippled"].ID, "Ankle Shots", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.Thief, _byPresence, "https://wiki.guildwars2.com/images/b/b4/Unscathed_Contender.png"), // It's not always possible to detect the presence of pistol and the trait is additive with itself. Staff master is worse as we can't detect endurance at all
            new DamageLogDamageModifier("Twin Fangs", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ModifierSource.Thief,"https://wiki.guildwars2.com/images/d/d1/Ferocious_Strikes.png", x => x.IsNinety && x.Result == ParseEnum.Result.Crit, ByPresence),
            new BuffDamageModifier(Boon.BoonsByName["Number of Boons"], "Premeditation", DamageSource.All, 1.0, DamageType.Power, DamageType.Power, ModifierSource.Thief, ByStack, "https://wiki.guildwars2.com/images/d/d7/Premeditation.png"),
            /// MESMER
            new BuffDamageModifier(Boon.BoonsByName["Compounding Power"], DamageSource.NoPets, 2.0, DamageType.Power, DamageType.Power, ModifierSource.Mesmer, ByStack),
            new BuffDamageModifierTarget(Boon.BoonsByName["Vulnerability"], "Fragility", DamageSource.NoPets, 0.5, DamageType.Power, DamageType.Power, ModifierSource.Mesmer, ByStack, "https://wiki.guildwars2.com/images/3/33/Fragility.png"),
            // Phantasmal Force would require activating buff tracking on minions, huge performance impact and some code impact
            // TOCHECK Superiority Complex
            new BuffDamageModifierTarget(Boon.BoonsByName["Slow"], "Danger Time", DamageSource.All, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Mesmer, ByPresence, "https://wiki.guildwars2.com/images/3/33/Fragility.png", (x => x.Result == ParseEnum.Result.Crit)),
            /// NECROMANCER     
            new BuffDamageModifierTarget(Boon.BoonsByName["Number of Boons"], "Spiteful Talisman", DamageSource.All, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Necromancer, ByAbsence, "https://wiki.guildwars2.com/images/9/96/Spiteful_Talisman.png"),
            new BuffDamageModifierTarget(Boon.BoonsByName["Fear"], "Dread", DamageSource.All, 33.0, DamageType.Power, DamageType.Power, ModifierSource.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/e/e2/Unholy_Fervor.png"),
            new DamageLogDamageModifier("Close to Death", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.Power, ModifierSource.Necromancer,"https://wiki.guildwars2.com/images/b/b2/Close_to_Death.png", x => x.IsFifty, ByPresence),
            new BuffDamageModifierTarget(Boon.BoonsByName["Chilled"], "Cold Shoulder", DamageSource.All, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/7/78/Cold_Shoulder.png"),
            /// ELEMENTALIST
            new BuffDamageModifier(Boon.BoonsByName["Harmonious Conduit"], DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Elementalist, ByPresence),
            new BuffDamageModifier(Boon.BoonsByName["Weaver's Prowess"], DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.Condition, ModifierSource.Elementalist, ByPresence),
            new BuffDamageModifier(Boon.BoonsByName["Elements of Rage"], DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Elementalist, ByPresence),
            new BuffDamageModifier(Boon.BoonsByName["Fire Attunement"], "Pyromancer's Training", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/e/e6/Pyromancer%27s_Training.png"),
            new BuffDamageModifierTarget(Boon.BoonsByName["Burning"], "Burning Rage", DamageSource.All, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/b/bd/Burning_Rage.png"),
            new DamageLogDamageModifier("Bolt to the Heart", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.Power, ModifierSource.Elementalist,"https://wiki.guildwars2.com/images/f/f8/Bolt_to_the_Heart.png", x => x.IsFifty, ByPresence),
            new BuffDamageModifierTarget(Boon.BoonsByName["Bleeding"], "Serrated Stones", DamageSource.All, 5.0, DamageType.Power, DamageType.Power, ModifierSource.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/6/60/Serrated_Stones.png"),
            new DamageLogDamageModifier("Aquamancer's Training", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Elementalist,"https://wiki.guildwars2.com/images/8/81/Aquamancer%27s_Training.png", x => x.IsNinety, ByPresence),
            new BuffDamageModifier(Boon.BoonsByName["Number of Boons"], "Bountiful Power", DamageSource.All, 2.0, DamageType.Power, DamageType.Power, ModifierSource.Elementalist, ByStack, "https://wiki.guildwars2.com/images/7/75/Bountiful_Power.png"),
            // TODO Swift Revenge
        };

        public static Dictionary<ModifierSource, List<DamageModifier>> DamageModifiersPerSource = _allDamageModifier.GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());

        protected DamageModifier(string name, DamageSource damageSource,  double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, string url, GainComputer gainComputer, DamageLogChecker dlChecker = null)
        {
            Name = name;
            _dmgSrc = damageSource;
            GainPerStack = gainPerStack;
            _compareType = compareType;
            _srcType = srctype;
            Src = src;
            Url = url;
            GainComputer = gainComputer;
            DLChecker = dlChecker;
        }

        protected (int total, int count)  GetTotalDamageData(Player p, ParsedLog log, Target t, PhaseData phase)
        {
            List<DamageLog> dls = new List<DamageLog>();
            switch (_compareType)
            {
                case DamageType.All:
                    dls = _dmgSrc == DamageSource.All ? p.GetDamageLogs(t,log, phase.Start, phase.End) : p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End);
                    break;
                case DamageType.Condition:
                    dls = (_dmgSrc == DamageSource.All ? p.GetDamageLogs(t, log, phase.Start, phase.End) : p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End)).Where(x => x.IsIndirectDamage).ToList();
                    break;
                case DamageType.Power:
                    dls = (_dmgSrc == DamageSource.All ? p.GetDamageLogs(t, log, phase.Start, phase.End) : p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End)).Where(x => !x.IsIndirectDamage).ToList();
                    break;
            }
            return (dls.Sum(x => x.Damage), dls.Count);
        }

        protected List<DamageLog> GetDamageLogs(Player p, ParsedLog log, Target t, PhaseData phase)
        {
            switch (_srcType)
            {
                case DamageType.All:
                    return _dmgSrc == DamageSource.All ? p.GetDamageLogs(t, log, phase.Start, phase.End) : p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End);
                case DamageType.Condition:
                    return (_dmgSrc == DamageSource.All ? p.GetDamageLogs(t, log, phase.Start, phase.End) : p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End)).Where(x => x.IsIndirectDamage).ToList();
                case DamageType.Power:
                default:
                    return (_dmgSrc == DamageSource.All  ? p.GetDamageLogs(t, log, phase.Start, phase.End) : p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End)).Where(x => !x.IsIndirectDamage).ToList();
            }
        }

        public abstract void ComputeDamageModifier(Dictionary<string, List<ExtraBoonData>> data, Dictionary<Target, Dictionary<string, List<ExtraBoonData>>> dataTarget, Player p, ParsedLog log);
    }
}
