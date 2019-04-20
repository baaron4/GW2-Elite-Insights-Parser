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
        public ulong MaxBuild { get; }
        public bool Multiplier => GainComputer.Multiplier;
        public ModifierSource Src { get; }
        public string Url { get; }
        public string Name { get; }
        public string Tooltip { get; }
        public delegate bool DamageLogChecker(DamageLog dl);
        protected DamageLogChecker DLChecker;

        protected DamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, string url, GainComputer gainComputer, DamageLogChecker dlChecker, ulong maxBuild)
        {
            Tooltip = tooltip;
            Name = name;
            _dmgSrc = damageSource;
            GainPerStack = gainPerStack;
            _compareType = compareType;
            _srcType = srctype;
            Src = src;
            Url = url;
            GainComputer = gainComputer;
            DLChecker = dlChecker;
            MaxBuild = maxBuild;
            switch (_dmgSrc)
            {
                case DamageSource.All:
                    Tooltip += "<br>Actor + Minions";
                    break;
                case DamageSource.NoPets:
                    Tooltip += "<br>No Minions";
                    break;
            }
            switch (_srcType)
            {
                case DamageType.All:
                    Tooltip += "<br>All Damage type";
                    break;
                case DamageType.Power:
                    Tooltip += "<br>Power Damage only";
                    break;
                case DamageType.Condition:
                    Tooltip += "<br>Condition Damage only";
                    break;
            }
            switch (_compareType)
            {
                case DamageType.All:
                    Tooltip += "<br>Compared against All Damage";
                    break;
                case DamageType.Power:
                    Tooltip += "<br>Compared against Power Damage";
                    break;
                case DamageType.Condition:
                    Tooltip += "<br>Compared against Condition Damage";
                    break;
            }
        }

        public int GetTotalDamage(Player p, ParsedLog log, Target t, int phaseIndex)
        {
            FinalDPS damageData = p.GetDPSTarget(log, phaseIndex, t);
            switch (_compareType)
            {
                case DamageType.All:
                    return _dmgSrc == DamageSource.All ? damageData.Damage  : damageData.ActorDamage;
                case DamageType.Condition:
                    return _dmgSrc == DamageSource.All ? damageData.CondiDamage : damageData.ActorCondiDamage;
                case DamageType.Power:
                    return _dmgSrc == DamageSource.All ? damageData.PowerDamage : damageData.ActorPowerDamage;
            }
            return 0;
        }

        public List<DamageLog> GetDamageLogs(Player p, ParsedLog log, Target t, PhaseData phase)
        {
            switch (_srcType)
            {
                case DamageType.All:
                    return _dmgSrc == DamageSource.All ? p.GetDamageLogs(t, log, phase) : p.GetJustPlayerDamageLogs(t, log, phase);
                case DamageType.Condition:
                    return (_dmgSrc == DamageSource.All ? p.GetDamageLogs(t, log, phase) : p.GetJustPlayerDamageLogs(t, log, phase)).Where(x => x.IsCondi).ToList();
                case DamageType.Power:
                default:
                    return (_dmgSrc == DamageSource.All ? p.GetDamageLogs(t, log, phase) : p.GetJustPlayerDamageLogs(t, log, phase)).Where(x => !x.IsCondi).ToList();
            }
        }

        public abstract void ComputeDamageModifier(Dictionary<string, List<DamageModifierData>> data, Dictionary<Target, Dictionary<string, List<DamageModifierData>>> dataTarget, Player p, ParsedLog log);

        protected static GainComputer ByPresence = new GainComputerByPresence();
        protected static GainComputer ByStack = new GainComputerByStack();
        protected static GainComputer ByAbsence = new GainComputerByAbsence();

        public static List<DamageModifier> AllDamageModifiers = new List<DamageModifier>
        {
            // Gear
            new DamageLogDamageModifier("Scholar Rune", "Scholar Rune – 5% over 90% HP", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff,"https://wiki.guildwars2.com/images/2/2b/Superior_Rune_of_the_Scholar.png", x => x.IsNinety, ByPresence ),
            new DamageLogDamageModifier("Eagle Rune", "Eagle Rune – 10% if target <50% HP", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff,"https://wiki.guildwars2.com/images/9/9b/Superior_Rune_of_the_Eagle.png", x => x.IsFifty, ByPresence ),
            new DamageLogDamageModifier("Thief Rune", "Thief Rune – 10% while flanking", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff,"https://wiki.guildwars2.com/images/9/96/Superior_Rune_of_the_Thief.png", x => x.IsFlanking , ByPresence),
            new DamageLogDamageModifier("Moving Bonus","Seaweed Salad (and the likes) – 5% while moving", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff,"https://wiki.guildwars2.com/images/1/1c/Bowl_of_Seaweed_Salad.png", x => x.IsMoving, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Might"), "Strength Rune", "Strength Rune – 5% under might",  DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByPresence, "https://wiki.guildwars2.com/images/2/2b/Superior_Rune_of_Strength.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Fire Shield"), "Fire Rune", "Fire Rune – 10% under fire aura",  DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByPresence, "https://wiki.guildwars2.com/images/4/4a/Superior_Rune_of_the_Fire.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Burning"), "Flame Legion Rune", "Flame Legion Rune - 7% on burning target",  DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByPresence, "https://wiki.guildwars2.com/images/4/4a/Superior_Rune_of_the_Flame_Legion.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Number of Boons"), "Spellbreaker Rune", "Spellbreaker Rune – 7% on boonless target",  DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByAbsence, "https://wiki.guildwars2.com/images/1/1a/Superior_Rune_of_the_Spellbreaker.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Chilled"), "Ice Rune", "Ice Rune – 7% on chilled target",  DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByPresence, "https://wiki.guildwars2.com/images/7/78/Superior_Rune_of_the_Ice.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Fury"), "Rage Rune", "Rage Rune – 5% under fury",  DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByPresence, "https://wiki.guildwars2.com/images/9/9e/Superior_Rune_of_Rage.png"),
            /// commons
            new BuffDamageModifierTarget(Boon.GetBoonByName("Vulnerability"), "Vulnerability – 1% per Stack", DamageSource.All, 1.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack),
            new BuffDamageModifier(Boon.GetBoonByName("Frost Spirit"), "Frost Spirit – 5%", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, ModifierSource.CommonBuff, ByPresence),
            new DamageLogDamageModifier(Boon.GetBoonByName("Soulcleave's Summit"), "Soulcleave's Summit – per hit (no ICD)", 45026, DamageSource.NoPets, DamageType.Power, DamageType.All, ModifierSource.CommonBuff, ByPresence),
            new DamageLogDamageModifier(Boon.GetBoonByName("One Wolf Pack"), "One Wolf Pack – per hit (max. once every 0.25s)", 42145, DamageSource.NoPets, DamageType.Power, DamageType.All, ModifierSource.CommonBuff, ByPresence),
            //new DamageLogDamageModifier(Boon.GetBoonByName("Static Charge"), 0, DamageSource.NoPets, DamageType.Power, DamageType.All, ModifierSource.CommonBuff, ByPresence),
            //new BuffDamageModifier(Boon.GetBoonByName("Glyph of Empowerment"), DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.All, _nonMultiplier),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Unnatural Signet"), "Unnatural Signet – 100%", DamageSource.All, 100.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByPresence),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Compromised"), "Compromised – 75% per stack", DamageSource.All, 75.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Fractured - Enemy"), "Fractured - Enemy – 10% per stack", DamageSource.All, 10.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack),
            new BuffDamageModifier(Boon.GetBoonByName("Blood Fueled"), "Blood Fueled – 20% per stack", DamageSource.NoPets, 20.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack),
            new BuffDamageModifier(Boon.GetBoonByName("Blood Fueled Abo"), "Blood Fueled Abo – 20% per stack", DamageSource.NoPets, 20.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack),
            new BuffDamageModifier(Boon.GetBoonByName("Fractal Offensive"), "Fractal Offensive – 3% per stack", DamageSource.NoPets, 3.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack),
            /// Revenant
            new BuffDamageModifier(Boon.GetBoonByName("Vicious Lacerations"), "Vicious Lacerations – 3% per Stack", DamageSource.NoPets, 3.0, DamageType.Power, DamageType.All, ModifierSource.Revenant, ByStack),
            new BuffDamageModifier(Boon.GetBoonByName("Retaliation"), "Vicious Reprisal", "Vicious Reprisal – 10% under retaliation", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Revenant, ByPresence, "https://wiki.guildwars2.com/images/c/cf/Vicious_Reprisal.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Weakness"), "Dwarven Battle Training", "Dwarven Battle Training – 10% on weakened target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Revenant, ByPresence, "https://wiki.guildwars2.com/images/5/50/Dwarven_Battle_Training.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Number of Boons"), "Reinforced Potency", "Reinforced Potency – 1% per boon", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ModifierSource.Herald, ByStack, "https://wiki.guildwars2.com/images/0/0a/Envoy_of_Sustenance.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Fury"), "Ferocious Aggression", "Ferocious Aggression – 7% under fury", DamageSource.NoPets, 7.0, DamageType.All, DamageType.All, ModifierSource.Revenant, ByPresence, "https://wiki.guildwars2.com/images/e/ec/Ferocious_Aggression.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Kalla's Fervor"), "Kalla's Fervor – 2% per stack", DamageSource.NoPets, 2.0, DamageType.Condition, DamageType.All, ModifierSource.Renegade, ByStack),
            new BuffDamageModifier(Boon.GetBoonByName("Improved Kalla's Fervor"), "Improved Kalla's Fervor – 3% per stack", DamageSource.NoPets, 3.0, DamageType.Condition, DamageType.All, ModifierSource.Renegade, ByStack),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Vulnerability"), "Targeted Destruction",  "Targeted Destruction – 0.5% per stack vuln", DamageSource.NoPets, 0.5, DamageType.Power, DamageType.All, ModifierSource.Revenant, ByStack, "https://wiki.guildwars2.com/images/e/ed/Targeted_Destruction.png"),
            new DamageLogDamageModifier("Swift Termination", "Swift Termination – 20% if target <50%", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Revenant,"https://wiki.guildwars2.com/images/b/bb/Swift_Termination.png", x => x.IsFifty, ByPresence),
            /// Warrior
            new BuffDamageModifier(Boon.GetBoonByName("Peak Performance"), "Peak Performance – 20%", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Warrior, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Always Angry"), "Always Angry – 7% per stack", DamageSource.NoPets, 7.0, DamageType.All, DamageType.All, ModifierSource.Berserker, ByPresence),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Weakness"), "Cull the Weak", "Cull the Weak – 7% on weakened target", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Warrior, ByPresence, "https://wiki.guildwars2.com/images/7/72/Cull_the_Weak.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Number of Boons"), "Empowered", "Empowered – 1% per boon", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ModifierSource.Warrior, ByStack, "https://wiki.guildwars2.com/images/c/c2/Empowered.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Number of Boons"), "Destruction of the Empowered", "Destruction of the Empowered – 3% per target boon", DamageSource.NoPets, 3.0, DamageType.Power, DamageType.All, ModifierSource.Warrior, ByStack, "https://wiki.guildwars2.com/images/5/5c/Destruction_of_the_Empowered.png"),
            //new BuffDamageModifierTarget(Boon.GetBoonByName("Number of Boons"), "Pure Strike", DamageSource.NoPets, (1.14/1.07 * 100.0) - 100.0, DamageType.Power, DamageType.Power, ModifierSource.Warrior, ByAbsence, "https://wiki.guildwars2.com/images/7/76/Pure_Strike_%28trait%29.png", x => x.Result == ParseEnum.Result.Crit), // Could use a different logic, like a dual gain per stack
            // TO TRACK Berserker's Power
            /// Guardian
            new BuffDamageModifierTarget(Boon.GetBoonByName("Burning"), "Fiery Wrath", "Fiery Wrath – 7% on burning target", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Guardian, ByPresence, "https://wiki.guildwars2.com/images/7/70/Fiery_Wrath.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Retaliation"), "Retribution","Retribution – 10% under retaliation", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Guardian, ByPresence, "https://wiki.guildwars2.com/images/d/d7/Retribution_%28trait%29.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Aegis"), "Unscathed Contender", "Unscathed Contender – 20% under aegis", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Guardian, ByPresence, "https://wiki.guildwars2.com/images/b/b4/Unscathed_Contender.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Number of Boons"), "Power of the Virtuous", "Power of the Virtuous – 1% per boon", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ModifierSource.Guardian, ByStack, "https://wiki.guildwars2.com/images/e/ee/Power_of_the_Virtuous.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Crippled"), "Zealot's Aggression", "Zealot's Aggression – 10% on crippled target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Dragonhunter, ByPresence, "https://wiki.guildwars2.com/images/7/7e/Zealot%27s_Aggression.png"),
            /// ENGINEER
            new BuffDamageModifier(Boon.GetBoonByName("Laser's Edge"), "Laser's Edge – 15%", DamageSource.NoPets, 15.0, DamageType.Power, DamageType.All, ModifierSource.Holosmith, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Thermal Vision"), "Thermal Vision – 5% (4s) after burning foe", DamageSource.NoPets, 5.0, DamageType.Condition, DamageType.All, ModifierSource.Engineer, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Vigor"), "Excessive Energy", "Excessive Energy – 10% under vigor", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Engineer, ByPresence, "https://wiki.guildwars2.com/images/1/1f/Excessive_Energy.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Vulnerability"), "Shaped Charge", "Shaped Charge – 10% on vulnerable enemies", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Engineer, ByPresence, "https://wiki.guildwars2.com/images/f/f3/Explosive_Powder.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Number of Conditions"), "Modified Ammunition", "Modified Ammunition – 2% per condition on target", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ModifierSource.Engineer, ByStack, "https://wiki.guildwars2.com/images/9/94/Modified_Ammunition.png"),
            /// RANGER
            new BuffDamageModifier(Boon.GetBoonByName("Sic 'Em!"), "Sic 'Em! – 40%", DamageSource.NoPets, 40.0, DamageType.Power, DamageType.All, ModifierSource.Ranger, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Light on your Feet"), "Light on your Feet – 10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Ranger, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Twice as Vicious"), "Twice as Vicious – 5% (4s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.All, DamageType.All, ModifierSource.Soulbeast, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Number of Boons"), "Bountiful Hunter", "Number of Boons – 1% per boon", DamageSource.All, 1.0, DamageType.Power, DamageType.All, ModifierSource.Ranger, ByStack, "https://wiki.guildwars2.com/images/2/25/Bountiful_Hunter.png"),
            new BuffDamageModifier(Boon.GetBoonByName("Fury"), "Furious Strength", "Furious Strength – 7% under fury", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Soulbeast, ByStack, "https://wiki.guildwars2.com/images/c/ca/Furious_Strength.png"),
            // TODO Predator's Onslaught ? can daze and stun be tracked?
            /// THIEF
            new BuffDamageModifier(Boon.GetBoonByName("Lead Attacks"), "Lead Attacks – 1% (10s) per initiative spent", DamageSource.NoPets, 1.0, DamageType.All, DamageType.All, ModifierSource.Thief, ByStack),
            new BuffDamageModifier(Boon.GetBoonByName("Lotus Training"), "Lotus Training – 10% cDam (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, ModifierSource.Daredevil, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Bounding Dodger"), "Bounding Dodger – 10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Daredevil, ByPresence),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Number of Conditions"), "Exposed Weakness", "Exposed Weakness – 2% per condition on target", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ModifierSource.Thief, ByStack, "https://wiki.guildwars2.com/images/0/02/Exposed_Weakness.png"),
            new DamageLogDamageModifier("Executioner", "Executioner – 20% if target <50% HP", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Thief,"https://wiki.guildwars2.com/images/9/93/Executioner.png", x => x.IsFifty, ByPresence),
            new DamageLogDamageModifier("Ferocius Strikes", "Ferocius Strikes – 10% on critical strikes if target >50%", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Thief,"https://wiki.guildwars2.com/images/d/d1/Ferocious_Strikes.png", x => !x.IsFifty && x.Result == ParseEnum.Result.Crit, ByPresence),
            //new BuffDamageModifierTarget(Boon.GetBoonByName("Crippled").ID, "Ankle Shots", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.Thief, _byPresence, "https://wiki.guildwars2.com/images/b/b4/Unscathed_Contender.png"), // It's not always possible to detect the presence of pistol and the trait is additive with itself. Staff master is worse as we can't detect endurance at all
            new DamageLogDamageModifier("Twin Fangs","Twin Fangs – 7% over 90%", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Thief,"https://wiki.guildwars2.com/images/d/d1/Ferocious_Strikes.png", x => x.IsNinety && x.Result == ParseEnum.Result.Crit, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Number of Boons"), "Premeditation", "Premeditation – 1% per boon",DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ModifierSource.Deadeye, ByStack, "https://wiki.guildwars2.com/images/d/d7/Premeditation.png"),
            /// MESMER
            new BuffDamageModifier(Boon.GetBoonByName("Compounding Power"), "Compounding Power – 2% per stack (8s) after creating an illusion ", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ModifierSource.Mesmer, ByStack),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Vulnerability"), "Fragility", "Fragility – 0.5% per stack vuln on target", DamageSource.NoPets, 0.5, DamageType.Power, DamageType.All, ModifierSource.Mesmer, ByStack, "https://wiki.guildwars2.com/images/3/33/Fragility.png"),
            // Phantasmal Force would require activating buff tracking on minions, huge performance impact and some code impact
            // TOCHECK Superiority Complex
            new BuffDamageModifierTarget(Boon.GetBoonByName("Slow"), "Danger Time", "Danger Time – 10% on slowed target", DamageSource.All, 10.0, DamageType.Power, DamageType.All, ModifierSource.Chronomancer, ByPresence, "https://wiki.guildwars2.com/images/3/33/Fragility.png", (x => x.Result == ParseEnum.Result.Crit)),
            /// NECROMANCER     
            // todo Soul barbs
            new BuffDamageModifierTarget(Boon.GetBoonByName("Number of Boons"), "Spiteful Talisman", "Spiteful Talisman – 10% on boonless target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Necromancer, ByAbsence, "https://wiki.guildwars2.com/images/9/96/Spiteful_Talisman.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Fear"), "Dread", "Dread – 33% on feared target", DamageSource.NoPets, 33.0, DamageType.Power, DamageType.All, ModifierSource.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/e/e2/Unholy_Fervor.png"),
            new DamageLogDamageModifier("Close to Death", "Close to Death – 20% below 90% HP", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Necromancer,"https://wiki.guildwars2.com/images/b/b2/Close_to_Death.png", x => x.IsFifty, ByPresence),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Chilled"), "Cold Shoulder", "Cold Shoulder – 15% on chilled target", DamageSource.NoPets, 15.0, DamageType.Power, DamageType.All, ModifierSource.Reaper, ByPresence, "https://wiki.guildwars2.com/images/7/78/Cold_Shoulder.png"),
            /// ELEMENTALIST
            new BuffDamageModifier(Boon.GetBoonByName("Harmonious Conduit"), "Harmonious Conduit – 10% (4s) after overload", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Tempest, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Weaver's Prowess"), "Weaver's Prowess – 10% cDam (8s) after switching element",  DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, ModifierSource.Weaver, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Elements of Rage"), "Elements of Rage – 10% (8s) after double attuning", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Weaver, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Fire Attunement"), "Pyromancer's Training", "Pyromancer's Training – 10% while fire attuned", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/e/e6/Pyromancer%27s_Training.png"),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Burning"), "Burning Rage", "Burning Rage – 10% on burning target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/b/bd/Burning_Rage.png"),
            new DamageLogDamageModifier( "Bolt to the Heart", "Bolt to the Heart – 20% if target <50% HP", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist,"https://wiki.guildwars2.com/images/f/f8/Bolt_to_the_Heart.png", x => x.IsFifty, ByPresence),
            new BuffDamageModifierTarget(Boon.GetBoonByName("Bleeding"), "Serrated Stones", "Serrated Stones – 5% to bleeding target", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/6/60/Serrated_Stones.png"),
            new DamageLogDamageModifier("Aquamancer's Training", "Aquamancer's Training – 10% over 90% HP", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist,"https://wiki.guildwars2.com/images/8/81/Aquamancer%27s_Training.png", x => x.IsNinety, ByPresence),
            new BuffDamageModifier(Boon.GetBoonByName("Number of Boons"), "Bountiful Power", "Bountiful Power – 2% per boon", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByStack, "https://wiki.guildwars2.com/images/7/75/Bountiful_Power.png"),
            new BuffDamageModifier(new BuffsTrackerMulti(Boon.GetBoonByName("Swiftness"), Boon.GetBoonByName("Superspeed")), "Swift Revenge", "Swift Revenge – 7% under swiftness/superspeed", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Weaver, "https://wiki.guildwars2.com/images/9/94/Swift_Revenge.png")
            // TODO Piercing Shards
        };
    }
}
