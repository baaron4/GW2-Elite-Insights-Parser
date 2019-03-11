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
        public enum ModifierSource {
            CommonBuff,
            ItemBuff,
            Necromancer, Reaper, Scourge,
            Elementalist, Tempest, Weaver,
            Mesmer, Chronomancer, Mirage,
            Warrior, Berserker, Spellbreaker,
            Revenant, Herald, Renegade,
            Guardian, Dragonhunter, Firebrand,
            Thief, Daredevil, Deadeye,
            Ranger, Druid, Soulbeast,
            Engineer, Scrapper, Holosmith
        };

        private DamageType _compareType { get; }
        private DamageType _srcType { get; }
        private DamageSource _dmgSrc { get; }
        protected double GainPerStack { get; }
        protected readonly GainComputer GainComputer;
        public bool Multiplier => GainComputer.Multiplier;
        public ModifierSource Src { get; }
        public string Url { get; }
        public string Name { get; }
        public delegate bool DamageLogChecker(DamageLog dl);
        protected DamageLogChecker DLChecker;

        protected DamageModifier(string name, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, string url, GainComputer gainComputer, DamageLogChecker dlChecker = null)
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


        protected int GetTotalDamage(Player p, ParsedLog log, Target t, PhaseData phase)
        {
            List<DamageLog> dls = new List<DamageLog>();
            switch (_compareType)
            {
                case DamageType.All:
                    dls = _dmgSrc == DamageSource.All ? p.GetDamageLogs(t, log, phase.Start, phase.End) : p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End);
                    break;
                case DamageType.Condition:
                    dls = (_dmgSrc == DamageSource.All ? p.GetDamageLogs(t, log, phase.Start, phase.End) : p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End)).Where(x => x.IsCondi).ToList();
                    break;
                case DamageType.Power:
                    dls = (_dmgSrc == DamageSource.All ? p.GetDamageLogs(t, log, phase.Start, phase.End) : p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End)).Where(x => !x.IsIndirectDamage).ToList();
                    break;
            }
            return dls.Sum(x => x.Damage);
        }

        protected List<DamageLog> GetDamageLogs(Player p, ParsedLog log, Target t, PhaseData phase)
        {
            switch (_srcType)
            {
                case DamageType.All:
                    return _dmgSrc == DamageSource.All ? p.GetDamageLogs(t, log, phase.Start, phase.End) : p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End);
                case DamageType.Condition:
                    return (_dmgSrc == DamageSource.All ? p.GetDamageLogs(t, log, phase.Start, phase.End) : p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End)).Where(x => x.IsCondi).ToList();
                case DamageType.Power:
                default:
                    return (_dmgSrc == DamageSource.All ? p.GetDamageLogs(t, log, phase.Start, phase.End) : p.GetJustPlayerDamageLogs(t, log, phase.Start, phase.End)).Where(x => !x.IsIndirectDamage).ToList();
            }
        }

        public abstract void ComputeDamageModifier(Dictionary<string, List<DamageModifierData>> data, Dictionary<Target, Dictionary<string, List<DamageModifierData>>> dataTarget, Player p, ParsedLog log);

        private static List<ModifierSource> ProfToEnum(string prof)
        {
            switch (prof)
            {
                case "Druid":
                    return new List<ModifierSource> { ModifierSource.Ranger, ModifierSource.Druid };
                case "Ranger":
                    return new List<ModifierSource> { ModifierSource.Ranger, ModifierSource.Soulbeast };
                case "Soulbeast":
                    return new List<ModifierSource> { ModifierSource.Ranger};
                case "Scrapper":
                    return new List<ModifierSource> { ModifierSource.Engineer, ModifierSource.Scrapper };
                case "Holosmith":
                    return new List<ModifierSource> { ModifierSource.Engineer, ModifierSource.Holosmith };
                case "Engineer":
                    return new List<ModifierSource> { ModifierSource.Engineer};
                case "Daredevil":
                    return new List<ModifierSource> { ModifierSource.Thief, ModifierSource.Daredevil };
                case "Deadeye":
                    return new List<ModifierSource> { ModifierSource.Thief, ModifierSource.Deadeye };
                case "Thief":
                    return new List<ModifierSource> { ModifierSource.Thief};
                case "Weaver":
                    return new List<ModifierSource> { ModifierSource.Elementalist, ModifierSource.Weaver };
                case "Tempest":
                    return new List<ModifierSource> { ModifierSource.Elementalist, ModifierSource.Tempest };
                case "Elementalist":
                    return new List<ModifierSource> { ModifierSource.Elementalist};
                case "Mirage":
                    return new List<ModifierSource> { ModifierSource.Mesmer, ModifierSource.Mirage };
                case "Chronomancer":
                    return new List<ModifierSource> { ModifierSource.Mesmer, ModifierSource.Chronomancer };
                case "Mesmer":
                    return new List<ModifierSource> { ModifierSource.Mesmer};
                case "Scourge":
                    return new List<ModifierSource> { ModifierSource.Necromancer, ModifierSource.Scourge };
                case "Reaper":
                    return new List<ModifierSource> { ModifierSource.Necromancer, ModifierSource.Reaper };
                case "Necromancer":
                    return new List<ModifierSource> { ModifierSource.Necromancer};
                case "Spellbreaker":
                    return new List<ModifierSource> { ModifierSource.Warrior, ModifierSource.Spellbreaker };
                case "Berserker":
                    return new List<ModifierSource> { ModifierSource.Warrior, ModifierSource.Berserker };
                case "Warrior":
                    return new List<ModifierSource> { ModifierSource.Warrior};
                case "Firebrand":
                    return new List<ModifierSource> { ModifierSource.Guardian, ModifierSource.Firebrand };
                case "Dragonhunter":
                    return new List<ModifierSource> { ModifierSource.Guardian, ModifierSource.Dragonhunter };
                case "Guardian":
                    return new List<ModifierSource> { ModifierSource.Guardian};
                case "Renegade":
                    return new List<ModifierSource> { ModifierSource.Revenant, ModifierSource.Renegade };
                case "Herald":
                    return new List<ModifierSource> { ModifierSource.Revenant, ModifierSource.Herald };
                case "Revenant":
                    return new List<ModifierSource> { ModifierSource.Revenant};
            }
            throw new InvalidOperationException("Unknown spec in damage modifier");
        }

        protected static GainComputer ByPresence = new GainComputerByPresence();
        protected static GainComputer ByStack = new GainComputerByStack();
        protected static GainComputer ByAbsence = new GainComputerByAbsence();

        private static List<DamageModifier> _allDamageModifier = new List<DamageModifier>
        {
            // Gear
            new DamageLogDamageModifier("Scholar Rune", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff,"https://wiki.guildwars2.com/images/2/2b/Superior_Rune_of_the_Scholar.png", x => x.IsNinety, ByPresence ),
            new DamageLogDamageModifier("Eagle Rune", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff,"https://wiki.guildwars2.com/images/9/9b/Superior_Rune_of_the_Eagle.png", x => x.IsFifty, ByPresence ),
            new DamageLogDamageModifier("Thief Rune", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff,"https://wiki.guildwars2.com/images/9/96/Superior_Rune_of_the_Thief.png", x => x.IsFlanking , ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Might"), "Strength Rune",  DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByPresence, "https://wiki.guildwars2.com/images/2/2b/Superior_Rune_of_Strength.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Fire Shield"), "Fire Rune",  DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByPresence, "https://wiki.guildwars2.com/images/4/4a/Superior_Rune_of_the_Fire.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Burning"), "Flame Legion Rune",  DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByPresence, "https://wiki.guildwars2.com/images/4/4a/Superior_Rune_of_the_Flame_Legion.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Number of Boons"), "Spellbreaker Rune",  DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByAbsence, "https://wiki.guildwars2.com/images/1/1a/Superior_Rune_of_the_Spellbreaker.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Chilled"), "Ice Rune",  DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByPresence, "https://wiki.guildwars2.com/images/7/78/Superior_Rune_of_the_Ice.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Fury"), "Rage Rune",  DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByPresence, "https://wiki.guildwars2.com/images/9/9e/Superior_Rune_of_Rage.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Bowl of Seaweed Salad"), DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByPresence, x => x.IsMoving),
            /// commons
            new BuffDamageModifierTarget(Boon.GetBoonByName("Vulnerability"), DamageSource.All, 1.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack),
            new BuffDamageModifier(Boon.GetBoonByName("Frost Spirit"), DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, ModifierSource.CommonBuff, ByPresence),
            //new DamageLogDamageModifier(Boon.GetBoonByName("Soulcleave's Summit"), 0, DamageSource.NoPets, DamageType.Power, DamageType.All, ModifierSource.CommonBuff, ByPresence),
            new DamageLogDamageModifier(Boon.GetBoonByName("One Wolf Pack"), 42145, DamageSource.NoPets, DamageType.Power, DamageType.All, ModifierSource.CommonBuff, ByPresence),
            //new DamageLogDamageModifier(Boon.GetBoonByName("Static Charge"), 0, DamageSource.NoPets, DamageType.Power, DamageType.All, ModifierSource.CommonBuff, ByPresence),
            //new BuffDamageModifier(Boon.GetBoonByName("Glyph of Empowerment"), DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.All, _nonMultiplier),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Unnatural Signet"), DamageSource.All, 200.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByPresence),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Compromised"), DamageSource.All, 75.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Fractured - Enemy"), DamageSource.All, 10.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack),
            new BuffDamageModifier(Boon.GetBoonByName("Blood Fueled"), DamageSource.NoPets, 20.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack),
            new BuffDamageModifier(Boon.GetBoonByName("Blood Fueled Abo"), DamageSource.NoPets, 20.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack),
            new BuffDamageModifier(Boon.GetBoonByName("Fractal Offensive"),  DamageSource.NoPets, 3.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack),
            /// Revenant
            new BuffDamageModifier(Boon.GetBoonByName("Vicious Lacerations"), DamageSource.NoPets, 3.0, DamageType.Power, DamageType.All, ModifierSource.Revenant, ByStack),
            new BuffDamageModifier(Boon.GetBoonByName("Retaliation"), "Vicious Reprisal", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Revenant, ByPresence, "https://wiki.guildwars2.com/images/c/cf/Vicious_Reprisal.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Weakness"), "Dwarven Battle Training", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Revenant, ByPresence, "https://wiki.guildwars2.com/images/5/50/Dwarven_Battle_Training.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Number of Boons"), "Reinforced Potency", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ModifierSource.Revenant, ByStack, "https://wiki.guildwars2.com/images/0/0a/Envoy_of_Sustenance.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Fury"), "Ferocious Aggression", DamageSource.NoPets, 7.0, DamageType.All, DamageType.All, ModifierSource.Revenant, ByPresence, "https://wiki.guildwars2.com/images/e/ec/Ferocious_Aggression.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Kalla's Fervor"), DamageSource.NoPets, 2.0, DamageType.Condition, DamageType.All, ModifierSource.Revenant, ByStack),
            new BuffDamageModifier(Boon.GetBoonByName("Improved Kalla's Fervor"), DamageSource.NoPets, 3.0, DamageType.Condition, DamageType.All, ModifierSource.Revenant, ByStack),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Vulnerability"), "Targeted Destruction", DamageSource.NoPets, 0.5, DamageType.Power, DamageType.All, ModifierSource.Revenant, ByStack, "https://wiki.guildwars2.com/images/e/ed/Targeted_Destruction.png"),
            new DamageLogDamageModifier("Swift Termination", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Revenant,"https://wiki.guildwars2.com/images/b/bb/Swift_Termination.png", x => x.IsFifty, ByPresence),
            /// Warrior
            new BuffDamageModifier(Boon.GetBoonByName("Peak Performance"), DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Warrior, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Always Angry"), DamageSource.NoPets, 7.0, DamageType.All, DamageType.All, ModifierSource.Warrior, ByStack),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Weakness"), "Cull the Weak", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Warrior, ByPresence, "https://wiki.guildwars2.com/images/7/72/Cull_the_Weak.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Number of Boons"), "Empowered", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ModifierSource.Warrior, ByStack, "https://wiki.guildwars2.com/images/c/c2/Empowered.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Number of Boons"), "Destruction of the Empowered", DamageSource.NoPets, 3.0, DamageType.Power, DamageType.All, ModifierSource.Warrior, ByStack, "https://wiki.guildwars2.com/images/5/5c/Destruction_of_the_Empowered.png"),
            //new BuffDamageModifierTarget(Boon.GetBoonByName("Number of Boons"), "Pure Strike", DamageSource.NoPets, (1.14/1.07 * 100.0) - 100.0, DamageType.Power, DamageType.Power, ModifierSource.Warrior, ByAbsence, "https://wiki.guildwars2.com/images/7/76/Pure_Strike_%28trait%29.png", x => x.Result == ParseEnum.Result.Crit), // Could use a different logic, like a dual gain per stack
            // TO TRACK Berserker's Power
            /// Guardian
            new BuffDamageModifierTarget(Boon.GetBoonByName("Burning"), "Fiery Wrath", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Guardian, ByPresence, "https://wiki.guildwars2.com/images/7/70/Fiery_Wrath.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Retaliation"), "Retribution", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Guardian, ByStack, "https://wiki.guildwars2.com/images/d/d7/Retribution_%28trait%29.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Aegis"), "Unscathed Contender", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Guardian, ByPresence, "https://wiki.guildwars2.com/images/b/b4/Unscathed_Contender.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Number of Boons"), "Power of the Virtuous", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ModifierSource.Guardian, ByStack, "https://wiki.guildwars2.com/images/b/b4/Unscathed_Contender.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Crippled"), "Zealot's Aggression", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Guardian, ByPresence, "https://wiki.guildwars2.com/images/b/b4/Unscathed_Contender.png"),
            /// ENGINEER
            new BuffDamageModifier(Boon.GetBoonByName("Laser's Edge"), DamageSource.NoPets, 15.0, DamageType.Power, DamageType.All, ModifierSource.Engineer, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Thermal Vision"), DamageSource.NoPets, 5.0, DamageType.Condition, DamageType.All, ModifierSource.Engineer, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Vigor"), "Excessive Energy", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Engineer, ByPresence, "https://wiki.guildwars2.com/images/1/1f/Excessive_Energy.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Vulnerability"), "Shaped Charge", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Engineer, ByPresence, "https://wiki.guildwars2.com/images/f/f3/Explosive_Powder.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Number of Conditions"), "Modified Ammunition", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ModifierSource.Engineer, ByStack, "https://wiki.guildwars2.com/images/9/94/Modified_Ammunition.png"),
            /// RANGER
            new BuffDamageModifier(Boon.GetBoonByName("Sic 'Em!"), DamageSource.NoPets, 40.0, DamageType.Power, DamageType.All, ModifierSource.Ranger, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Light on your Feet"), DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Ranger, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Twice as Vicious"), DamageSource.NoPets, 5.0, DamageType.All, DamageType.All, ModifierSource.Ranger, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Number of Boons"), "Bountiful Hunter", DamageSource.All, 1.0, DamageType.Power, DamageType.All, ModifierSource.Ranger, ByStack, "https://wiki.guildwars2.com/images/2/25/Bountiful_Hunter.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Fury"), "Furious Strength", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Ranger, ByStack, "https://wiki.guildwars2.com/images/c/ca/Furious_Strength.png"),
            // TODO Predator's Onslaught ? can daze and stun be tracked?
            /// THIEF
            new BuffDamageModifier(Boon.GetBoonByName("Lead Attacks"), DamageSource.NoPets, 1.0, DamageType.All, DamageType.All, ModifierSource.Thief, ByStack),
            new BuffDamageModifier(Boon.GetBoonByName("Lotus Training"), DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, ModifierSource.Thief, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Bounding Dodger"), DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Thief, ByPresence),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Number of Conditions"), "Exposed Weakness", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ModifierSource.Thief, ByStack, "https://wiki.guildwars2.com/images/0/02/Exposed_Weakness.png"),
            new DamageLogDamageModifier("Executioner", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Thief,"https://wiki.guildwars2.com/images/9/93/Executioner.png", x => x.IsFifty, ByPresence),
            new DamageLogDamageModifier("Ferocius Strikes", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Thief,"https://wiki.guildwars2.com/images/d/d1/Ferocious_Strikes.png", x => !x.IsFifty, ByPresence),
            //new BuffDamageModifierTarget(Boon.GetBoonByName("Crippled").ID, "Ankle Shots", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.Thief, _byPresence, "https://wiki.guildwars2.com/images/b/b4/Unscathed_Contender.png"), // It's not always possible to detect the presence of pistol and the trait is additive with itself. Staff master is worse as we can't detect endurance at all
            new DamageLogDamageModifier("Twin Fangs", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Thief,"https://wiki.guildwars2.com/images/d/d1/Ferocious_Strikes.png", x => x.IsNinety && x.Result == ParseEnum.Result.Crit, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Number of Boons"), "Premeditation", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ModifierSource.Thief, ByStack, "https://wiki.guildwars2.com/images/d/d7/Premeditation.png"),
            /// MESMER
            new BuffDamageModifier(Boon.GetBoonByName("Compounding Power"), DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ModifierSource.Mesmer, ByStack),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Vulnerability"), "Fragility", DamageSource.NoPets, 0.5, DamageType.Power, DamageType.All, ModifierSource.Mesmer, ByStack, "https://wiki.guildwars2.com/images/3/33/Fragility.png"),
            // Phantasmal Force would require activating buff tracking on minions, huge performance impact and some code impact
            // TOCHECK Superiority Complex
            new BuffDamageModifierTarget(Boon.GetBoonByName("Slow"), "Danger Time", DamageSource.All, 10.0, DamageType.Power, DamageType.All, ModifierSource.Mesmer, ByPresence, "https://wiki.guildwars2.com/images/3/33/Fragility.png", (x => x.Result == ParseEnum.Result.Crit)),
            /// NECROMANCER     
            new BuffDamageModifierTarget(Boon.GetBoonByName("Number of Boons"), "Spiteful Talisman", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Necromancer, ByAbsence, "https://wiki.guildwars2.com/images/9/96/Spiteful_Talisman.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Fear"), "Dread", DamageSource.NoPets, 33.0, DamageType.Power, DamageType.All, ModifierSource.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/e/e2/Unholy_Fervor.png"),
            new DamageLogDamageModifier("Close to Death", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Necromancer,"https://wiki.guildwars2.com/images/b/b2/Close_to_Death.png", x => x.IsFifty, ByPresence),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Chilled"), "Cold Shoulder", DamageSource.NoPets, 15.0, DamageType.Power, DamageType.All, ModifierSource.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/7/78/Cold_Shoulder.png"),
            /// ELEMENTALIST
            new BuffDamageModifier(Boon.GetBoonByName("Harmonious Conduit"), DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Weaver's Prowess"), DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, ModifierSource.Elementalist, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Elements of Rage"), DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Fire Attunement"), "Pyromancer's Training", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/e/e6/Pyromancer%27s_Training.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Burning"), "Burning Rage", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/b/bd/Burning_Rage.png"),
            new DamageLogDamageModifier("Bolt to the Heart", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist,"https://wiki.guildwars2.com/images/f/f8/Bolt_to_the_Heart.png", x => x.IsFifty, ByPresence),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Bleeding"), "Serrated Stones", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/6/60/Serrated_Stones.png"),
            new DamageLogDamageModifier("Aquamancer's Training", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.Elementalist,"https://wiki.guildwars2.com/images/8/81/Aquamancer%27s_Training.png", x => x.IsNinety, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Number of Boons"), "Bountiful Power", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByStack, "https://wiki.guildwars2.com/images/7/75/Bountiful_Power.png"),
            new BuffDamageModifier(new BuffsTrackerMulti(Boon.GetBoonByName("Swiftness"), Boon.GetBoonByName("Superspeed")), "Swift Revenge", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, "https://wiki.guildwars2.com/images/9/94/Swift_Revenge.png")
            // TODO Piercing Shards
        };

        public static Dictionary<ModifierSource, List<DamageModifier>> DamageModifiersPerSource = _allDamageModifier.GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());

        public static List<DamageModifier> GetModifiersPerProf(string prof)
        {
            List<DamageModifier> res = new List<DamageModifier>();
            List<ModifierSource> srcs = ProfToEnum(prof);
            foreach (ModifierSource src in srcs)
            {
                if (DamageModifiersPerSource.TryGetValue(src, out var list))
                {
                    res.AddRange(list);
                }
            }
            return res;
        }

        public static Dictionary<string, DamageModifier> DamageModifiersByName = _allDamageModifier.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList().First());
    }
}
