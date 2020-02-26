using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public abstract class DamageModifier
    {
        public enum DamageType { All, Power, Condition };
        public enum DamageSource { All, NoPets };
        public enum ModifierSource
        {
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
        protected GainComputer GainComputer { get; }
        public ulong MinBuild { get; } = ulong.MaxValue;
        public ulong MaxBuild { get; } = ulong.MinValue;
        public bool Multiplier => GainComputer.Multiplier;
        public bool SkillBased => GainComputer.SkillBased;
        public ModifierSource Src { get; }
        public string Icon { get; protected set; }
        public string Name { get; protected set; }
        public string Tooltip { get; protected set; }
        public delegate bool DamageLogChecker(AbstractDamageEvent dl);
        protected DamageLogChecker DLChecker { get; set; }

        protected DamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, string icon, GainComputer gainComputer, DamageLogChecker dlChecker, ulong minBuild, ulong maxBuild)
        {
            Tooltip = tooltip;
            Name = name;
            _dmgSrc = damageSource;
            GainPerStack = gainPerStack;
            _compareType = compareType;
            _srcType = srctype;
            Src = src;
            Icon = icon;
            GainComputer = gainComputer;
            DLChecker = dlChecker;
            MaxBuild = maxBuild;
            MinBuild = minBuild;
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
            if (!Multiplier)
            {
                Tooltip += "<br>Non multiplier";
            }
        }

        public int GetTotalDamage(Player p, ParsedLog log, NPC t, int phaseIndex)
        {
            FinalDPS damageData = p.GetDPSTarget(log, phaseIndex, t);
            switch (_compareType)
            {
                case DamageType.All:
                    return _dmgSrc == DamageSource.All ? damageData.Damage : damageData.ActorDamage;
                case DamageType.Condition:
                    return _dmgSrc == DamageSource.All ? damageData.CondiDamage : damageData.ActorCondiDamage;
                case DamageType.Power:
                    return _dmgSrc == DamageSource.All ? damageData.PowerDamage : damageData.ActorPowerDamage;
            }
            return 0;
        }

        public List<AbstractDamageEvent> GetDamageLogs(Player p, ParsedLog log, NPC t, PhaseData phase)
        {
            switch (_srcType)
            {
                case DamageType.All:
                    return _dmgSrc == DamageSource.All ? p.GetDamageLogs(t, log, phase) : p.GetJustPlayerDamageLogs(t, log, phase);
                case DamageType.Condition:
                    return (_dmgSrc == DamageSource.All ? p.GetDamageLogs(t, log, phase) : p.GetJustPlayerDamageLogs(t, log, phase)).Where(x => x.IsCondi(log)).ToList();
                case DamageType.Power:
                default:
                    return (_dmgSrc == DamageSource.All ? p.GetDamageLogs(t, log, phase) : p.GetJustPlayerDamageLogs(t, log, phase)).Where(x => !x.IsCondi(log)).ToList();
            }
        }

        public abstract void ComputeDamageModifier(Dictionary<string, List<DamageModifierStat>> data, Dictionary<NPC, Dictionary<string, List<DamageModifierStat>>> dataTarget, Player p, ParsedLog log);

        protected static GainComputer ByPresence = new GainComputerByPresence();
        protected static GainComputer ByPresenceNonMulti = new GainComputerNonMultiplier();
        protected static GainComputer BySkill = new GainComputerBySkill();
        protected static GainComputer ByStack = new GainComputerByStack();
        protected static GainComputer ByAbsence = new GainComputerByAbsence();

        private static readonly List<DamageModifier> _gearDamageModifiers = new List<DamageModifier>
        {
            new DamageLogDamageModifier("Scholar Rune", "5% over 90% HP", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff,"https://wiki.guildwars2.com/images/2/2b/Superior_Rune_of_the_Scholar.png", x => x.IsOverNinety, ByPresence, 93543, ulong.MaxValue ),
            new DamageLogDamageModifier("Scholar Rune", "10% over 90% HP", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff,"https://wiki.guildwars2.com/images/2/2b/Superior_Rune_of_the_Scholar.png", x => x.IsOverNinety, ByPresence, 0, 93543 ),
            new DamageLogDamageModifier("Eagle Rune", "10% if target <50% HP", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff,"https://wiki.guildwars2.com/images/9/9b/Superior_Rune_of_the_Eagle.png", x => x.AgainstUnderFifty, ByPresence, 93543, ulong.MaxValue),
            new DamageLogDamageModifier("Eagle Rune", "6% if target <50% HP", DamageSource.NoPets, 6.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff,"https://wiki.guildwars2.com/images/9/9b/Superior_Rune_of_the_Eagle.png", x => x.AgainstUnderFifty, ByPresence , 0 , 93543),
            new DamageLogDamageModifier("Thief Rune", "10% while flanking", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff,"https://wiki.guildwars2.com/images/9/96/Superior_Rune_of_the_Thief.png", x => x.IsFlanking , ByPresence),
            new DamageLogDamageModifier("Moving Bonus","Seaweed Salad (and the likes) – 5% while moving", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff,"https://wiki.guildwars2.com/images/1/1c/Bowl_of_Seaweed_Salad.png", x => x.IsMoving, ByPresence),
            new BuffDamageModifier(740, "Strength Rune", "5% under might",  DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByPresence, "https://wiki.guildwars2.com/images/2/2b/Superior_Rune_of_Strength.png"),
            new BuffDamageModifier(5677, "Fire Rune", "10% under fire aura",  DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByPresence, "https://wiki.guildwars2.com/images/4/4a/Superior_Rune_of_the_Fire.png", 93543 , ulong.MaxValue),
            new BuffDamageModifierTarget(737, "Flame Legion Rune", "7% on burning target",  DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByPresence, "https://wiki.guildwars2.com/images/4/4a/Superior_Rune_of_the_Flame_Legion.png"),
            new BuffDamageModifierTarget(ProfHelper.NumberOfBoonsID, "Spellbreaker Rune", "7% on boonless target",  DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByAbsence, "https://wiki.guildwars2.com/images/1/1a/Superior_Rune_of_the_Spellbreaker.png"),
            new BuffDamageModifierTarget(722, "Ice Rune", "7% on chilled target",  DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByPresence, "https://wiki.guildwars2.com/images/7/78/Superior_Rune_of_the_Ice.png"),
            new BuffDamageModifier(725, "Rage Rune", "5% under fury",  DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.ItemBuff, ByPresence, "https://wiki.guildwars2.com/images/9/9e/Superior_Rune_of_Rage.png", null),
        };
        private static readonly List<DamageModifier> _commonDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(738, "Vulnerability", "1% per Stack", DamageSource.All, 1.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack, "https://wiki.guildwars2.com/images/a/af/Vulnerability.png"),
            new BuffDamageModifier(50421, "Frost Spirit", "5%", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, ModifierSource.CommonBuff, ByPresenceNonMulti, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png", 88541, ulong.MaxValue),
            new BuffDamageModifier(56123, "Violent Currents", "5%", DamageSource.NoPets, 5.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack, "https://wiki.guildwars2.com/images/0/06/Violent_Currents.png"),
            new DamageLogDamageModifier("Soulcleave's Summit", "per hit (no ICD)", DamageSource.NoPets, 0, DamageType.Power, DamageType.All, ModifierSource.CommonBuff,"https://wiki.guildwars2.com/images/7/78/Soulcleave%27s_Summit.png", (x => x.SkillId == 45026), BySkill),
            new DamageLogDamageModifier("One Wolf Pack", "per hit (max. once every 0.25s)", DamageSource.NoPets, 0, DamageType.Power, DamageType.All, ModifierSource.CommonBuff, "https://wiki.guildwars2.com/images/3/3b/One_Wolf_Pack.png", (x => x.SkillId == 42145), BySkill),
            //new DamageLogDamageModifier(Boon.GetBoonByName("Static Charge"), 0, DamageSource.NoPets, DamageType.Power, DamageType.All, ModifierSource.CommonBuff, ByPresence),
            //new BuffDamageModifier(Boon.GetBoonByName("Glyph of Empowerment"), DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.All, ByPresence),
            new BuffDamageModifierTarget(38224,"Unnatural Signet", "100%", DamageSource.All, 100.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByPresence, "https://wiki.guildwars2.com/images/2/20/Unnatural_Signet.png"),
            new BuffDamageModifierTarget(35096, "Compromised", "75% per stack", DamageSource.All, 75.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack, "https://wiki.guildwars2.com/images/4/48/Compromised.png"),
            new BuffDamageModifierTarget(56582, "Erratic Energy", "5% per stack", DamageSource.All, 5.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack, "https://wiki.guildwars2.com/images/0/06/Values_Mastery.png"),
            new BuffDamageModifierTarget(53030, "Fractured - Enemy", "10% per stack", DamageSource.All, 10.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
            new BuffDamageModifier(34422, "Blood Fueled", "10% per stack", DamageSource.NoPets, 10.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
            new BuffDamageModifier(34428, "Blood Fueled Abo", "10% per stack", DamageSource.NoPets, 10.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
            new BuffDamageModifier(32473, "Fractal Offensive", "3% per stack", DamageSource.NoPets, 3.0, DamageType.All, DamageType.All, ModifierSource.CommonBuff, ByStack, "https://wiki.guildwars2.com/images/thumb/8/8d/Mist_Offensive_Potion.png/40px-Mist_Offensive_Potion.png"),
        };
        private static readonly List<DamageModifier> _revenantDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifier(29395, "Vicious Lacerations", "3% per Stack", DamageSource.NoPets, 3.0, DamageType.Power, DamageType.All, ModifierSource.Revenant, ByStack, "https://wiki.guildwars2.com/images/c/cd/Vicious_Lacerations.png", 92715, 102321),
            new BuffDamageModifier(29395, "Vicious Lacerations", "2% per Stack", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ModifierSource.Revenant, ByStack, "https://wiki.guildwars2.com/images/c/cd/Vicious_Lacerations.png", 0, 92715),
            new BuffDamageModifier(873, "Vicious Reprisal", "10% under retaliation", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Revenant, ByPresence, "https://wiki.guildwars2.com/images/c/cf/Vicious_Reprisal.png"),
            new BuffDamageModifierTarget(742, "Dwarven Battle Training", "10% on weakened target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Revenant, ByPresence, "https://wiki.guildwars2.com/images/5/50/Dwarven_Battle_Training.png", 94051, ulong.MaxValue),
            new BuffDamageModifier(ProfHelper.NumberOfBoonsID, "Reinforced Potency", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ModifierSource.Herald, ByStack, "https://wiki.guildwars2.com/images/0/0a/Envoy_of_Sustenance.png"),
            new BuffDamageModifier(725, "Ferocious Aggression", "7% under fury", DamageSource.NoPets, 7.0, DamageType.All, DamageType.All, ModifierSource.Revenant, ByPresence, "https://wiki.guildwars2.com/images/e/ec/Ferocious_Aggression.png"),
            new BuffDamageModifier(42883, "Kalla's Fervor", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Condition, DamageType.All, ModifierSource.Renegade, ByStack, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png"),
            new BuffDamageModifier(45614, "Improved Kalla's Fervor", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Condition, DamageType.All, ModifierSource.Renegade, ByStack, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png"),
            new BuffDamageModifierTarget(738, "Targeted Destruction", "0.5% per stack vuln", DamageSource.NoPets, 0.5, DamageType.Power, DamageType.All, ModifierSource.Revenant, ByStack, "https://wiki.guildwars2.com/images/e/ed/Targeted_Destruction.png", 95535, ulong.MaxValue),
            new BuffDamageModifierTarget(738, "Targeted Destruction", "10.0% if vuln", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Revenant, ByPresence, "https://wiki.guildwars2.com/images/e/ed/Targeted_Destruction.png", 92715, 95535),
            new BuffDamageModifierTarget(738, "Targeted Destruction", "7.0% if vuln", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Revenant, ByPresence, "https://wiki.guildwars2.com/images/e/ed/Targeted_Destruction.png", 0, 92715),
            new DamageLogDamageModifier("Swift Termination", "20% if target <50%", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Revenant,"https://wiki.guildwars2.com/images/b/bb/Swift_Termination.png", x => x.AgainstUnderFifty, ByPresence),
        };
        private static readonly List<DamageModifier> _warriorDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifier(46853, "Peak Performance", "20%", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Warrior, ByPresence, "https://wiki.guildwars2.com/images/9/98/Peak_Performance.png", 90455, 100690),
            new BuffDamageModifier(46853, "Peak Performance", "33%", DamageSource.NoPets, 33.0, DamageType.Power, DamageType.All, ModifierSource.Warrior, ByPresence, "https://wiki.guildwars2.com/images/9/98/Peak_Performance.png", 0, 90455),
            new BuffDamageModifier(34099, "Always Angry", "7% per stack", DamageSource.NoPets, 7.0, DamageType.All, DamageType.All, ModifierSource.Berserker, ByPresence, "https://wiki.guildwars2.com/images/6/63/Always_Angry.png", 0 , 96406),
            new BuffDamageModifier(29502, "Bloody Roar", "10% while in berserk", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Berserker, ByPresenceNonMulti, "https://wiki.guildwars2.com/images/e/e1/Bloody_Roar.png", 0 , 96406),
            new BuffDamageModifier(29502, "Bloody Roar", "20% while in berserk", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Berserker, ByPresenceNonMulti, "https://wiki.guildwars2.com/images/e/e1/Bloody_Roar.png", 96406 , 97950),
            new BuffDamageModifier(29502, "Bloody Roar", "20% while in berserk", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Berserker, ByPresence, "https://wiki.guildwars2.com/images/e/e1/Bloody_Roar.png", 97950 , ulong.MaxValue),
            new BuffDamageModifierTarget(742, "Cull the Weak", "7% on weakened target", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Warrior, ByPresence, "https://wiki.guildwars2.com/images/7/72/Cull_the_Weak.png"),
            new BuffDamageModifier(ProfHelper.NumberOfBoonsID, "Empowered", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ModifierSource.Warrior, ByStack, "https://wiki.guildwars2.com/images/c/c2/Empowered.png"),
            new BuffDamageModifier(42539, "Berserker's Power", "7% per stack", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Warrior, ByStack, "https://wiki.guildwars2.com/images/6/6f/Berserker%27s_Power.png"),
            new BuffDamageModifierTarget(ProfHelper.NumberOfBoonsID, "Destruction of the Empowered", "3% per target boon", DamageSource.NoPets, 3.0, DamageType.Power, DamageType.All, ModifierSource.Warrior, ByStack, "https://wiki.guildwars2.com/images/5/5c/Destruction_of_the_Empowered.png"),
            new BuffDamageModifierTarget(new long[] {721, 727, 722}, "Leg Specialist", "7% to movement-impaired foes", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Warrior, ByPresence, "https://wiki.guildwars2.com/images/9/9e/Leg_Specialist.png", 99526, ulong.MaxValue)
            //new BuffDamageModifierTarget(Boon.GetBoonByName("Number of Boons"), "100.0, DamageType.Power, DamageType.Power, ModifierSource.Warrior, ByAbsence, "https://wiki.guildwars2.com/images/7/76/Pure_Strike_%28trait%29.png", x => x.Result == ParseEnum.Result.Crit), // Could use a different logic, like a dual gain per stack
            // TO TRACK Berserker's Power

        };
        private static readonly List<DamageModifier> _guardianDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(737, "Fiery Wrath", "7% on burning target", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Guardian, ByPresence, "https://wiki.guildwars2.com/images/7/70/Fiery_Wrath.png"),
            new BuffDamageModifier(873, "Retribution", "10% under retaliation", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Guardian, ByPresence, "https://wiki.guildwars2.com/images/d/d7/Retribution_%28trait%29.png"),
            new BuffDamageModifier(56890, "Symbolic Avenger", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ModifierSource.Guardian, ByStack, "https://wiki.guildwars2.com/images/e/e5/Symbolic_Avenger.png", 97950, ulong.MaxValue),
            new BuffDamageModifier(743, "Unscathed Contender", "20% under aegis", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Guardian, ByPresence, "https://wiki.guildwars2.com/images/b/b4/Unscathed_Contender.png"),
            new BuffDamageModifier(ProfHelper.NumberOfBoonsID, "Power of the Virtuous", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ModifierSource.Guardian, ByStack, "https://wiki.guildwars2.com/images/e/ee/Power_of_the_Virtuous.png"),
            new BuffDamageModifierTarget(721, "Zealot's Aggression", "10% on crippled target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Dragonhunter, ByPresence, "https://wiki.guildwars2.com/images/7/7e/Zealot%27s_Aggression.png"),
            new BuffDamageModifierTarget(738, "Symbolic Exposure", "5% on vuln target", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, ModifierSource.Guardian, ByPresence, "https://wiki.guildwars2.com/images/c/cd/Symbolic_Exposure.png"),
        };
        private static readonly List<DamageModifier> _engineerDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifier(44414, "Laser's Edge", "15%", DamageSource.NoPets, 15.0, DamageType.Power, DamageType.All, ModifierSource.Holosmith, ByPresenceNonMulti, "https://wiki.guildwars2.com/images/5/5d/Laser%27s_Edge.png", 0, 97950),
            //new BuffDamageModifier(new long[] {719,1122, 5974  }, "Object in Motion", "5% per swiftness/stability/superspeed", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, ModifierSource.Scrapper, ByStack, "https://wiki.guildwars2.com/images/d/da/Object_in_Motion.png", 97950, ulong.MaxValue),
            new BuffDamageModifier(51389, "Thermal Vision", "5% (4s) after burning foe", DamageSource.NoPets, 5.0, DamageType.Condition, DamageType.All, ModifierSource.Engineer, ByPresence, "https://wiki.guildwars2.com/images/8/8a/Skilled_Marksman.png", 92069, ulong.MaxValue),
            new BuffDamageModifier(51389, "Thermal Vision", "10% (4s) after burning foe", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, ModifierSource.Engineer, ByPresence, "https://wiki.guildwars2.com/images/8/8a/Skilled_Marksman.png", 0, 92069),
            new BuffDamageModifier(726, "Excessive Energy", "10% under vigor", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Engineer, ByPresence, "https://wiki.guildwars2.com/images/1/1f/Excessive_Energy.png"),
            new BuffDamageModifierTarget(738, "Shaped Charge", "10% on vulnerable enemies", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Engineer, ByPresence, "https://wiki.guildwars2.com/images/f/f3/Explosive_Powder.png", 0, 99526),
            new BuffDamageModifierTarget(738, "Shaped Charge", "0.5% per stack vuln", DamageSource.NoPets, 0.5, DamageType.Power, DamageType.All, ModifierSource.Engineer, ByStack, "https://wiki.guildwars2.com/images/f/f3/Explosive_Powder.png", 99526, ulong.MaxValue),
            new BuffDamageModifierTarget(ProfHelper.NumberOfConditionsID, "Modified Ammunition", "2% per condition on target", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ModifierSource.Engineer, ByStack, "https://wiki.guildwars2.com/images/9/94/Modified_Ammunition.png"),
        };
        private static readonly List<DamageModifier> _thiefDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifier(34659, "Lead Attacks", "1% (10s) per initiative spent", DamageSource.NoPets, 1.0, DamageType.All, DamageType.All, ModifierSource.Thief, ByStack, "https://wiki.guildwars2.com/images/0/01/Lead_Attacks.png"),
            new BuffDamageModifier(32200, "Lotus Training", "10% cDam (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, ModifierSource.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/e/ea/Lotus_Training.png"),
            new BuffDamageModifier(33162, "Bounding Dodger", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/3/30/Bounding_Dodger.png"),
            new BuffDamageModifierTarget(742, "Weakening Strikes", "7% if weakness on target", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/7/7c/Weakening_Strikes.png", 96406, ulong.MaxValue),
            new BuffDamageModifierTarget(ProfHelper.NumberOfConditionsID, "Exposed Weakness", "2% per condition on target", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ModifierSource.Thief, ByStack, "https://wiki.guildwars2.com/images/0/02/Exposed_Weakness.png", 90455, ulong.MaxValue),
            new BuffDamageModifierTarget(ProfHelper.NumberOfConditionsID, "Exposed Weakness", "10% if condition on target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Thief, ByPresence, "https://wiki.guildwars2.com/images/0/02/Exposed_Weakness.png", 0, 90455),
            new DamageLogDamageModifier("Executioner", "20% if target <50% HP", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Thief,"https://wiki.guildwars2.com/images/9/93/Executioner.png", x => x.AgainstUnderFifty, ByPresence),
            new DamageLogDamageModifier("Ferocious Strikes", "10% on critical strikes if target >50%", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Thief,"https://wiki.guildwars2.com/images/d/d1/Ferocious_Strikes.png", x => !x.AgainstUnderFifty && x.HasCrit, ByPresence),
            //new BuffDamageModifierTarget(Boon.GetBoonByName("Crippled").ID, "Ankle Shots", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ModifierSource.Thief, _byPresence, "https://wiki.guildwars2.com/images/b/b4/Unscathed_Contender.png"), // It's not always possible to detect the presence of pistol and the trait is additive with itself. Staff master is worse as we can't detect endurance at all
            new DamageLogDamageModifier("Twin Fangs","7% over 90%", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Thief,"https://wiki.guildwars2.com/images/d/d1/Ferocious_Strikes.png", x => x.IsOverNinety && x.HasCrit, ByPresence),
            new BuffDamageModifier(ProfHelper.NumberOfBoonsID, "Premeditation", "1% per boon",DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ModifierSource.Deadeye, ByStack, "https://wiki.guildwars2.com/images/d/d7/Premeditation.png"),
        };
        private static readonly List<DamageModifier> _rangerDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifier(33902, "Sic 'Em!", "40%", DamageSource.NoPets, 40.0, DamageType.Power, DamageType.All, ModifierSource.Ranger, ByPresence, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png"),
            new DamageLogDamageModifier("Hunter's Tactics", "10% while flanking", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Ranger,"https://wiki.guildwars2.com/images/b/bb/Hunter%27s_Tactics.png", x => x.IsFlanking , ByPresence, 102321, ulong.MaxValue),
            new BuffDamageModifier(30673, "Light on your Feet", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Ranger, ByPresence, "https://wiki.guildwars2.com/images/2/22/Light_on_your_Feet.png"),
            new BuffDamageModifier(45600, "Twice as Vicious", "5% (4s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.All, DamageType.All, ModifierSource.Soulbeast, ByPresence, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png",0 ,97950),
            new BuffDamageModifier(45600, "Twice as Vicious", "5% (10s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.All, DamageType.All, ModifierSource.Soulbeast, ByPresence, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png", 97950, 102321),
            new BuffDamageModifier(45600, "Twice as Vicious", "10% (10s) after disabling foe", DamageSource.NoPets, 10.0, DamageType.All, DamageType.All, ModifierSource.Soulbeast, ByPresence, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png", 102321, ulong.MaxValue),
            new BuffDamageModifier(ProfHelper.NumberOfBoonsID, "Bountiful Hunter", "1% per boon", DamageSource.All, 1.0, DamageType.Power, DamageType.All, ModifierSource.Ranger, ByStack, "https://wiki.guildwars2.com/images/2/25/Bountiful_Hunter.png"),
            new BuffDamageModifier(725, "Furious Strength", "7% under fury", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Soulbeast, ByStack, "https://wiki.guildwars2.com/images/c/ca/Furious_Strength.png"),
            new BuffDamageModifierTarget(new long[] { 872, 833, 721, 727, 791, 722, 27705}, "Predator's Onslaught", "15% to disabled or movement-impaired foes", DamageSource.All, 15.0, DamageType.Power, DamageType.All, ModifierSource.Ranger, ByPresence, "https://wiki.guildwars2.com/images/a/ac/Predator%27s_Onslaught.png")
            // TODO Predator's Onslaught ? can daze and stun be tracked?
        };
        private static readonly List<DamageModifier> _mesmerDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifier(49058, "Compounding Power", "2% per stack (8s) after creating an illusion ", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ModifierSource.Mesmer, ByStack, "https://wiki.guildwars2.com/images/e/e5/Compounding_Power.png"),
            new BuffDamageModifierTarget(738, "Fragility", "0.5% per stack vuln on target", DamageSource.NoPets, 0.5, DamageType.Power, DamageType.All, ModifierSource.Mesmer, ByStack, "https://wiki.guildwars2.com/images/3/33/Fragility.png"),
            // Phantasmal Force would require activating buff tracking on minions, huge performance impact and some code impact
            // TOCHECK Superiority Complex
            new BuffDamageModifierTarget(26766, "Danger Time", "10% on slowed target", DamageSource.All, 10.0, DamageType.Power, DamageType.All, ModifierSource.Chronomancer, ByPresence, "https://wiki.guildwars2.com/images/3/33/Fragility.png", (x => x.HasCrit)),
        };
        private static readonly List<DamageModifier> _necromancerDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(ProfHelper.NumberOfBoonsID, "Spiteful Talisman", "10% on boonless target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Necromancer, ByAbsence, "https://wiki.guildwars2.com/images/9/96/Spiteful_Talisman.png"),
            new BuffDamageModifierTarget(791, "Dread", "33% on feared target", DamageSource.NoPets, 33.0, DamageType.Power, DamageType.All, ModifierSource.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/e/e2/Unholy_Fervor.png",92069, ulong.MaxValue),
            new BuffDamageModifierTarget(791, "Dread", "20% on feared target", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/e/e2/Unholy_Fervor.png", 0, 92069),
            new DamageLogDamageModifier("Close to Death", "20% below 90% HP", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Necromancer,"https://wiki.guildwars2.com/images/b/b2/Close_to_Death.png", x => x.AgainstUnderFifty, ByPresence),
            new BuffDamageModifierTarget(722, "Cold Shoulder", "15% on chilled target", DamageSource.NoPets, 15.0, DamageType.Power, DamageType.All, ModifierSource.Reaper, ByPresence, "https://wiki.guildwars2.com/images/7/78/Cold_Shoulder.png", 95535, ulong.MaxValue),
            new BuffDamageModifierTarget(722, "Cold Shoulder", "10% on chilled target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Reaper, ByPresence, "https://wiki.guildwars2.com/images/7/78/Cold_Shoulder.png", 0, 95535),
            new BuffDamageModifier(53489, "Soul Barbs", "10% after entering or exiting shroud", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/b/bd/Soul_Barbs.png", 94051, ulong.MaxValue),
        };
        private static readonly List<DamageModifier> _elementalistDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifier(31353, "Harmonious Conduit", "10% (4s) after overload", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Tempest, ByPresence, "https://wiki.guildwars2.com/images/b/b3/Harmonious_Conduit.png", 0 , 99526),
            new BuffDamageModifier(31353, "Transcendent Tempest", "7% (7s) after overload", DamageSource.NoPets, 7.0, DamageType.All, DamageType.All, ModifierSource.Tempest, ByPresence, "https://wiki.guildwars2.com/images/a/ac/Transcendent_Tempest_%28effect%29.png", 99526 , ulong.MaxValue),
            new BuffDamageModifier(42061, "Weaver's Prowess", "10% cDam (8s) after switching element",  DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, ModifierSource.Weaver, ByPresence, "https://wiki.guildwars2.com/images/7/75/Weaver%27s_Prowess.png"),
            new BuffDamageModifier(42416, "Elements of Rage", "10% (8s) after double attuning", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Weaver, ByPresence, "https://wiki.guildwars2.com/images/a/a2/Elements_of_Rage.png"),
            new BuffDamageModifier(new long[] { 5585, ProfHelper.FireWater, ProfHelper.FireAir, ProfHelper.FireEarth, ProfHelper.FireDual }, "Pyromancer's Training", "10% while fire attuned", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/e/e6/Pyromancer%27s_Training.png", 0, 97950),
            new BuffDamageModifierTarget(737, "Burning Rage", "10% on burning target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/b/bd/Burning_Rage.png", 0, 97950),
            new BuffDamageModifierTarget(737, "Pyromancer's Training", "10% on burning target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/e/e6/Pyromancer%27s_Training.png", 97950, ulong.MaxValue),
            new DamageLogDamageModifier( "Bolt to the Heart", "20% if target <50% HP", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist,"https://wiki.guildwars2.com/images/f/f8/Bolt_to_the_Heart.png", x => x.AgainstUnderFifty, ByPresence),
            new BuffDamageModifierTarget(736, "Serrated Stones", "5% to bleeding target", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/6/60/Serrated_Stones.png"),
            new DamageLogDamageModifier("Aquamancer's Training", "10% over 90% HP", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist,"https://wiki.guildwars2.com/images/8/81/Aquamancer%27s_Training.png", x => x.IsOverNinety, ByPresence, 0, 97950),
            new BuffDamageModifierTarget(738, new long[] { 5586, ProfHelper.WaterAir, ProfHelper.WaterEarth, ProfHelper.WaterFire, ProfHelper.WaterDual}, "Piercing Shards w/ Water", "20% on vuln target while on water", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByPresence, ByPresence, "https://wiki.guildwars2.com/images/4/4b/Piercing_Shards.png", 97950, ulong.MaxValue),
            new BuffDamageModifierTarget(738, new long[] { 5586, ProfHelper.WaterAir, ProfHelper.WaterEarth, ProfHelper.WaterFire, ProfHelper.WaterDual}, "Piercing Shards", "10% on vuln target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByPresence, ByAbsence, "https://wiki.guildwars2.com/images/4/4b/Piercing_Shards.png", 97950, ulong.MaxValue),
            new BuffDamageModifierTarget(738, new long[] { 5586, ProfHelper.WaterAir, ProfHelper.WaterEarth, ProfHelper.WaterFire, ProfHelper.WaterDual}, "Piercing Shards", "20% on vuln target while on water", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByPresence, ByPresence, "https://wiki.guildwars2.com/images/4/4b/Piercing_Shards.png", 0, 97950),
            //new DamageLogDamageModifier("Flow like Water", "10% over 75% HP", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist,"https://wiki.guildwars2.com/images/0/0a/Flow_like_Water.png", x => x.IsOverNinety, ByPresence, 97950, ulong.MaxValue),
            new BuffDamageModifier(ProfHelper.NumberOfBoonsID, "Bountiful Power", "2% per boon", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ModifierSource.Elementalist, ByStack, "https://wiki.guildwars2.com/images/7/75/Bountiful_Power.png"),
            new BuffDamageModifier(new long[] { 719, 5974}, "Swift Revenge", "7% under swiftness/superspeed", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ModifierSource.Weaver, ByPresence, "https://wiki.guildwars2.com/images/9/94/Swift_Revenge.png", 0, 97950),
            new BuffDamageModifier(new long[] { 719, 5974}, "Swift Revenge", "10% under swiftness/superspeed", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ModifierSource.Weaver, ByPresence, "https://wiki.guildwars2.com/images/9/94/Swift_Revenge.png", 97950, ulong.MaxValue)
            // TODO Piercing Shards
        };

        public static List<List<DamageModifier>> AllDamageModifiers = new List<List<DamageModifier>>
        {
            _gearDamageModifiers,
            _commonDamageModifiers,
            _revenantDamageModifiers,
            _warriorDamageModifiers,
            _guardianDamageModifiers,
            _engineerDamageModifiers,
            _thiefDamageModifiers,
            _rangerDamageModifiers,
            _mesmerDamageModifiers,
            _necromancerDamageModifiers,
            _elementalistDamageModifiers
        };
    }
}
