using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.ParsedData;
using GW2EIUtils;

namespace GW2EIEvtcParser.EIData
{
    public abstract class DamageModifier
    {

        public enum DamageModifierMode { PvE, sPvP, WvW, All, sPvPWvW };
        public enum DamageType { All, Power, Condition };
        public enum DamageSource { All, NoPets };

        private DamageType _compareType { get; }
        private DamageType _srcType { get; }
        private DamageSource _dmgSrc { get; }
        protected double GainPerStack { get; }
        protected GainComputer GainComputer { get; }
        public ulong MinBuild { get; } = ulong.MaxValue;
        public ulong MaxBuild { get; } = ulong.MinValue;
        public bool Multiplier => GainComputer.Multiplier;
        public bool SkillBased => GainComputer.SkillBased;
        public ParseHelper.Source Src { get; }
        public string Icon { get; protected set; }
        public string Name { get; protected set; }
        public string Tooltip { get; protected set; }
        public delegate bool DamageLogChecker(AbstractDamageEvent dl);

        protected DamageModifierMode Mode { get; set; } = DamageModifierMode.All;
        protected DamageLogChecker DLChecker { get; set; }

        protected DamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParseHelper.Source src, string icon, GainComputer gainComputer, DamageLogChecker dlChecker, ulong minBuild, ulong maxBuild, DamageModifierMode mode)
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
            Mode = mode;
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

        public bool Keep(FightLogic.ParseMode mode)
        {
            if (Mode == DamageModifierMode.All)
            {
                if (mode == FightLogic.ParseMode.WvW)
                {
                    return !(this is BuffDamageModifierTarget);
                }
                return true;
            }
            switch(mode)
            {
                case FightLogic.ParseMode.Unknown:
                case FightLogic.ParseMode.Instanced5:
                case FightLogic.ParseMode.Instanced10:
                case FightLogic.ParseMode.Benchmark:
                    return Mode == DamageModifierMode.PvE;
                case FightLogic.ParseMode.WvW:
                    return !(this is BuffDamageModifierTarget) && (Mode == DamageModifierMode.WvW || Mode == DamageModifierMode.sPvPWvW);
                case FightLogic.ParseMode.sPvP:
                    return Mode == DamageModifierMode.sPvP || Mode == DamageModifierMode.sPvPWvW;
            }
            return false;
        }

        public int GetTotalDamage(Player p, ParsedEvtcLog log, NPC t, int phaseIndex)
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

        public List<AbstractDamageEvent> GetDamageLogs(Player p, ParsedEvtcLog log, NPC t, PhaseData phase)
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

        public abstract void ComputeDamageModifier(Dictionary<string, List<DamageModifierStat>> data, Dictionary<NPC, Dictionary<string, List<DamageModifierStat>>> dataTarget, Player p, ParsedEvtcLog log);

        protected static GainComputer ByPresence = new GainComputerByPresence();
        protected static GainComputer ByPresenceNonMulti = new GainComputerNonMultiplier();
        protected static GainComputer BySkill = new GainComputerBySkill();
        protected static GainComputer ByStack = new GainComputerByStack();
        protected static GainComputer ByAbsence = new GainComputerByAbsence();

        private static readonly List<DamageModifier> _itemDamageModifiers = new List<DamageModifier>
        {
            new DamageLogDamageModifier("Scholar Rune", "5% over 90% HP", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ParseHelper.Source.Item,"https://wiki.guildwars2.com/images/2/2b/Superior_Rune_of_the_Scholar.png", x => x.IsOverNinety, ByPresence, 93543, ulong.MaxValue, DamageModifierMode.All ),
            new DamageLogDamageModifier("Scholar Rune", "10% over 90% HP", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ParseHelper.Source.Item,"https://wiki.guildwars2.com/images/2/2b/Superior_Rune_of_the_Scholar.png", x => x.IsOverNinety, ByPresence, 0, 93543, DamageModifierMode.All ),
            new DamageLogDamageModifier("Eagle Rune", "10% if target <50% HP", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ParseHelper.Source.Item,"https://wiki.guildwars2.com/images/9/9b/Superior_Rune_of_the_Eagle.png", x => x.AgainstUnderFifty, ByPresence, 93543, ulong.MaxValue, DamageModifierMode.All),
            new DamageLogDamageModifier("Eagle Rune", "6% if target <50% HP", DamageSource.NoPets, 6.0, DamageType.Power, DamageType.Power, ParseHelper.Source.Item,"https://wiki.guildwars2.com/images/9/9b/Superior_Rune_of_the_Eagle.png", x => x.AgainstUnderFifty, ByPresence , 0 , 93543, DamageModifierMode.All),
            new DamageLogDamageModifier("Thief Rune", "10% while flanking", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ParseHelper.Source.Item,"https://wiki.guildwars2.com/images/9/96/Superior_Rune_of_the_Thief.png", x => x.IsFlanking , ByPresence, DamageModifierMode.All),
            new DamageLogDamageModifier("Moving Bonus","Seaweed Salad (and the likes) – 5% while moving", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ParseHelper.Source.Item,"https://wiki.guildwars2.com/images/1/1c/Bowl_of_Seaweed_Salad.png", x => x.IsMoving, ByPresence, DamageModifierMode.All),
            new BuffDamageModifier(740, "Strength Rune", "5% under might",  DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ParseHelper.Source.Item, ByPresence, "https://wiki.guildwars2.com/images/2/2b/Superior_Rune_of_Strength.png", DamageModifierMode.All),
            new BuffDamageModifier(5677, "Fire Rune", "10% under fire aura",  DamageSource.NoPets, 10.0, DamageType.Power, DamageType.Power, ParseHelper.Source.Item, ByPresence, "https://wiki.guildwars2.com/images/4/4a/Superior_Rune_of_the_Fire.png", 93543 , ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifierTarget(737, "Flame Legion Rune", "7% on burning target",  DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ParseHelper.Source.Item, ByPresence, "https://wiki.guildwars2.com/images/4/4a/Superior_Rune_of_the_Flame_Legion.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(ProfHelper.NumberOfBoonsID, "Spellbreaker Rune", "7% on boonless target",  DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ParseHelper.Source.Item, ByAbsence, "https://wiki.guildwars2.com/images/1/1a/Superior_Rune_of_the_Spellbreaker.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(722, "Ice Rune", "7% on chilled target",  DamageSource.NoPets, 7.0, DamageType.Power, DamageType.Power, ParseHelper.Source.Item, ByPresence, "https://wiki.guildwars2.com/images/7/78/Superior_Rune_of_the_Ice.png", DamageModifierMode.All),
            new BuffDamageModifier(725, "Rage Rune", "5% under fury",  DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ParseHelper.Source.Item, ByPresence, "https://wiki.guildwars2.com/images/9/9e/Superior_Rune_of_Rage.png", DamageModifierMode.All),
            new BuffDamageModifier(32473, "Fractal Offensive", "3% per stack", DamageSource.NoPets, 3.0, DamageType.All, DamageType.All, ParseHelper.Source.Item, ByStack, "https://wiki.guildwars2.com/images/thumb/8/8d/Mist_Offensive_Potion.png/40px-Mist_Offensive_Potion.png", DamageModifierMode.PvE),
        };
        private static readonly List<DamageModifier> _commonDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(738, "Vulnerability", "1% per Stack", DamageSource.All, 1.0, DamageType.All, DamageType.All, ParseHelper.Source.Common, ByStack, "https://wiki.guildwars2.com/images/a/af/Vulnerability.png", DamageModifierMode.All),
            new BuffDamageModifier(50421, "Frost Spirit", "5%", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, ParseHelper.Source.Common, ByPresenceNonMulti, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png", 88541, ulong.MaxValue, DamageModifierMode.All),
            new DamageLogDamageModifier("Soulcleave's Summit", "per hit (no ICD)", DamageSource.NoPets, 0, DamageType.Power, DamageType.All, ParseHelper.Source.Common,"https://wiki.guildwars2.com/images/7/78/Soulcleave%27s_Summit.png", (x => x.SkillId == 45026), BySkill, DamageModifierMode.All),
            new DamageLogDamageModifier("One Wolf Pack", "per hit (max. once every 0.25s)", DamageSource.NoPets, 0, DamageType.Power, DamageType.All, ParseHelper.Source.Common, "https://wiki.guildwars2.com/images/3/3b/One_Wolf_Pack.png", (x => x.SkillId == 42145), BySkill, DamageModifierMode.All),
        };

        private static readonly List<DamageModifier> _fightSpecificDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifier(56123, "Violent Currents", "5%", DamageSource.NoPets, 5.0, DamageType.All, DamageType.All, ParseHelper.Source.FightSpecific, ByStack, "https://wiki.guildwars2.com/images/0/06/Violent_Currents.png", DamageModifierMode.PvE),
            new BuffDamageModifierTarget(38224,"Unnatural Signet", "100%", DamageSource.All, 100.0, DamageType.All, DamageType.All, ParseHelper.Source.FightSpecific, ByPresence, "https://wiki.guildwars2.com/images/2/20/Unnatural_Signet.png", DamageModifierMode.PvE),
            new BuffDamageModifierTarget(35096, "Compromised", "75% per stack", DamageSource.All, 75.0, DamageType.All, DamageType.All, ParseHelper.Source.FightSpecific, ByStack, "https://wiki.guildwars2.com/images/4/48/Compromised.png", DamageModifierMode.PvE),
            new BuffDamageModifierTarget(56582, "Erratic Energy", "5% per stack", DamageSource.All, 5.0, DamageType.All, DamageType.All, ParseHelper.Source.FightSpecific, ByStack, "https://wiki.guildwars2.com/images/0/06/Values_Mastery.png", DamageModifierMode.PvE),
            new BuffDamageModifierTarget(53030, "Fractured - Enemy", "10% per stack", DamageSource.All, 10.0, DamageType.All, DamageType.All, ParseHelper.Source.FightSpecific, ByStack, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png", DamageModifierMode.PvE),
            new BuffDamageModifier(34422, "Blood Fueled", "10% per stack", DamageSource.NoPets, 10.0, DamageType.All, DamageType.All, ParseHelper.Source.FightSpecific, ByStack, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png", DamageModifierMode.PvE),
            new BuffDamageModifier(34428, "Blood Fueled Abo", "10% per stack", DamageSource.NoPets, 10.0, DamageType.All, DamageType.All, ParseHelper.Source.FightSpecific, ByStack, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png", DamageModifierMode.PvE),
        };
        private static readonly List<DamageModifier> _revenantDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifier(29395, "Vicious Lacerations", "3% per Stack", DamageSource.NoPets, 3.0, DamageType.Power, DamageType.All, ParseHelper.Source.Revenant, ByStack, "https://wiki.guildwars2.com/images/c/cd/Vicious_Lacerations.png", 92715, 102321, DamageModifierMode.PvE),
            new BuffDamageModifier(29395, "Vicious Lacerations", "2% per Stack", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ParseHelper.Source.Revenant, ByStack, "https://wiki.guildwars2.com/images/c/cd/Vicious_Lacerations.png", 0, 92715, DamageModifierMode.PvE),
            new BuffDamageModifier(873, "Vicious Reprisal", "10% under retaliation", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Revenant, ByPresence, "https://wiki.guildwars2.com/images/c/cf/Vicious_Reprisal.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(742, "Dwarven Battle Training", "10% on weakened target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Revenant, ByPresence, "https://wiki.guildwars2.com/images/5/50/Dwarven_Battle_Training.png", 94051, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(ProfHelper.NumberOfBoonsID, "Reinforced Potency", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ParseHelper.Source.Herald, ByStack, "https://wiki.guildwars2.com/images/0/0a/Envoy_of_Sustenance.png", DamageModifierMode.All),
            new BuffDamageModifier(725, "Ferocious Aggression", "7% under fury", DamageSource.NoPets, 7.0, DamageType.All, DamageType.All, ParseHelper.Source.Revenant, ByPresence, "https://wiki.guildwars2.com/images/e/ec/Ferocious_Aggression.png", DamageModifierMode.All),
            new BuffDamageModifier(42883, "Kalla's Fervor", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Condition, DamageType.All, ParseHelper.Source.Renegade, ByStack, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png", DamageModifierMode.All),
            new BuffDamageModifier(45614, "Improved Kalla's Fervor", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Condition, DamageType.All, ParseHelper.Source.Renegade, ByStack, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(738, "Targeted Destruction", "0.5% per stack vuln", DamageSource.NoPets, 0.5, DamageType.Power, DamageType.All, ParseHelper.Source.Revenant, ByStack, "https://wiki.guildwars2.com/images/e/ed/Targeted_Destruction.png", 95535, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifierTarget(738, "Targeted Destruction", "10.0% if vuln", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Revenant, ByPresence, "https://wiki.guildwars2.com/images/e/ed/Targeted_Destruction.png", 92715, 95535, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(738, "Targeted Destruction", "7.0% if vuln", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParseHelper.Source.Revenant, ByPresence, "https://wiki.guildwars2.com/images/e/ed/Targeted_Destruction.png", 0, 92715, DamageModifierMode.PvE),
            new DamageLogDamageModifier("Swift Termination", "20% if target <50%", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ParseHelper.Source.Revenant,"https://wiki.guildwars2.com/images/b/bb/Swift_Termination.png", x => x.AgainstUnderFifty, ByPresence, DamageModifierMode.All),
        };
        private static readonly List<DamageModifier> _warriorDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifier(46853, "Peak Performance (absent)", "5%", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, ParseHelper.Source.Warrior, ByAbsence, "https://wiki.guildwars2.com/images/9/98/Peak_Performance.png", 90455, ulong.MaxValue, DamageModifierMode.PvE),
            new BuffDamageModifier(46853, "Peak Performance (present)", "20%", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ParseHelper.Source.Warrior, ByPresence, "https://wiki.guildwars2.com/images/9/98/Peak_Performance.png", 90455, ulong.MaxValue, DamageModifierMode.PvE),
            new BuffDamageModifier(46853, "Peak Performance (absent)", "3%", DamageSource.NoPets, 3.0, DamageType.Power, DamageType.All, ParseHelper.Source.Warrior, ByAbsence, "https://wiki.guildwars2.com/images/9/98/Peak_Performance.png", 90455, ulong.MaxValue, DamageModifierMode.sPvPWvW),
            new BuffDamageModifier(46853, "Peak Performance (present)", "10%", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Warrior, ByPresence, "https://wiki.guildwars2.com/images/9/98/Peak_Performance.png", 90455, ulong.MaxValue, DamageModifierMode.sPvPWvW),
            new BuffDamageModifier(46853, "Peak Performance", "33%", DamageSource.NoPets, 33.0, DamageType.Power, DamageType.All, ParseHelper.Source.Warrior, ByPresence, "https://wiki.guildwars2.com/images/9/98/Peak_Performance.png", 0, 90455, DamageModifierMode.PvE),
            new BuffDamageModifier(34099, "Always Angry", "7% per stack", DamageSource.NoPets, 7.0, DamageType.All, DamageType.All, ParseHelper.Source.Berserker, ByPresence, "https://wiki.guildwars2.com/images/6/63/Always_Angry.png", 0 , 96406, DamageModifierMode.PvE),
            new BuffDamageModifier(29502, "Bloody Roar", "10% while in berserk", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Berserker, ByPresenceNonMulti, "https://wiki.guildwars2.com/images/e/e1/Bloody_Roar.png", 0 , 96406, DamageModifierMode.PvE),
            new BuffDamageModifier(29502, "Bloody Roar", "20% while in berserk", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ParseHelper.Source.Berserker, ByPresenceNonMulti, "https://wiki.guildwars2.com/images/e/e1/Bloody_Roar.png", 96406 , 97950, DamageModifierMode.PvE),
            new BuffDamageModifier(29502, "Bloody Roar", "20% while in berserk", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ParseHelper.Source.Berserker, ByPresence, "https://wiki.guildwars2.com/images/e/e1/Bloody_Roar.png", 97950 , ulong.MaxValue, DamageModifierMode.PvE),
            new BuffDamageModifier(29502, "Bloody Roar", "15% while in berserk", DamageSource.NoPets, 15.0, DamageType.Power, DamageType.All, ParseHelper.Source.Berserker, ByPresence, "https://wiki.guildwars2.com/images/e/e1/Bloody_Roar.png", 97950 , ulong.MaxValue, DamageModifierMode.sPvPWvW),
            new BuffDamageModifier(719, "Warrior's Sprint", "7% under swiftness", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParseHelper.Source.Warrior, ByPresence, "https://wiki.guildwars2.com/images/e/e3/Warrior%27s_Sprint.png", 86181 , ulong.MaxValue, DamageModifierMode.PvE),
            new BuffDamageModifier(719, "Warrior's Sprint", "3% under swiftness", DamageSource.NoPets, 3.0, DamageType.Power, DamageType.All, ParseHelper.Source.Warrior, ByPresence, "https://wiki.guildwars2.com/images/e/e3/Warrior%27s_Sprint.png", 86181 , ulong.MaxValue, DamageModifierMode.sPvPWvW),
            new BuffDamageModifierTarget(742, "Cull the Weak", "7% on weakened target", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParseHelper.Source.Warrior, ByPresence, "https://wiki.guildwars2.com/images/7/72/Cull_the_Weak.png", DamageModifierMode.All),
            new BuffDamageModifier(ProfHelper.NumberOfBoonsID, "Empowered", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ParseHelper.Source.Warrior, ByStack, "https://wiki.guildwars2.com/images/c/c2/Empowered.png", DamageModifierMode.All),
            new BuffDamageModifier(42539, "Berserker's Power", "7% per stack", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParseHelper.Source.Warrior, ByStack, "https://wiki.guildwars2.com/images/6/6f/Berserker%27s_Power.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(ProfHelper.NumberOfBoonsID, "Destruction of the Empowered", "3% per target boon", DamageSource.NoPets, 3.0, DamageType.Power, DamageType.All, ParseHelper.Source.Warrior, ByStack, "https://wiki.guildwars2.com/images/5/5c/Destruction_of_the_Empowered.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(new long[] {721, 727, 722}, "Leg Specialist", "7% to movement-impaired foes", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParseHelper.Source.Warrior, ByPresence, "https://wiki.guildwars2.com/images/9/9e/Leg_Specialist.png", 99526, ulong.MaxValue, DamageModifierMode.All)

        };
        private static readonly List<DamageModifier> _guardianDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(737, "Fiery Wrath", "7% on burning target", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParseHelper.Source.Guardian, ByPresence, "https://wiki.guildwars2.com/images/7/70/Fiery_Wrath.png", DamageModifierMode.All),
            new BuffDamageModifier(873, "Retribution", "10% under retaliation", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Guardian, ByPresence, "https://wiki.guildwars2.com/images/d/d7/Retribution_%28trait%29.png", DamageModifierMode.All),
            new BuffDamageModifier(56890, "Symbolic Avenger", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ParseHelper.Source.Guardian, ByStack, "https://wiki.guildwars2.com/images/e/e5/Symbolic_Avenger.png", 97950, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(743, "Unscathed Contender", "20% under aegis", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ParseHelper.Source.Guardian, ByPresence, "https://wiki.guildwars2.com/images/b/b4/Unscathed_Contender.png", DamageModifierMode.All),
            new BuffDamageModifier(ProfHelper.NumberOfBoonsID, "Power of the Virtuous", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ParseHelper.Source.Guardian, ByStack, "https://wiki.guildwars2.com/images/e/ee/Power_of_the_Virtuous.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(721, "Zealot's Aggression", "10% on crippled target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Dragonhunter, ByPresence, "https://wiki.guildwars2.com/images/7/7e/Zealot%27s_Aggression.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(738, "Symbolic Exposure", "5% on vuln target", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, ParseHelper.Source.Guardian, ByPresence, "https://wiki.guildwars2.com/images/c/cd/Symbolic_Exposure.png", DamageModifierMode.All),
            new BuffDamageModifier(59592, "Inspiring Virtue", "10% (6s) after activating a virtue ", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Guardian, ByPresence, "https://wiki.guildwars2.com/images/8/8f/Virtuous_Solace.png", 102321, ulong.MaxValue, DamageModifierMode.All),
        };
        private static readonly List<DamageModifier> _engineerDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifier(44414, "Laser's Edge", "15%", DamageSource.NoPets, 15.0, DamageType.Power, DamageType.All, ParseHelper.Source.Holosmith, ByPresenceNonMulti, "https://wiki.guildwars2.com/images/5/5d/Laser%27s_Edge.png", 0, 97950, DamageModifierMode.PvE),
            //new BuffDamageModifier(new long[] {719,1122, 5974  }, "Object in Motion", "5% per swiftness/stability/superspeed", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, ParseHelper.Source.Scrapper, ByStack, "https://wiki.guildwars2.com/images/d/da/Object_in_Motion.png", 97950, ulong.MaxValue),
            new BuffDamageModifier(51389, "Thermal Vision", "5% (4s) after burning foe", DamageSource.NoPets, 5.0, DamageType.Condition, DamageType.All, ParseHelper.Source.Engineer, ByPresence, "https://wiki.guildwars2.com/images/8/8a/Skilled_Marksman.png", 92069, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(51389, "Thermal Vision", "10% (4s) after burning foe", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, ParseHelper.Source.Engineer, ByPresence, "https://wiki.guildwars2.com/images/8/8a/Skilled_Marksman.png", 0, 92069, DamageModifierMode.PvE),
            new BuffDamageModifier(726, "Excessive Energy", "10% under vigor", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Engineer, ByPresence, "https://wiki.guildwars2.com/images/1/1f/Excessive_Energy.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(738, "Shaped Charge", "10% on vulnerable enemies", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Engineer, ByPresence, "https://wiki.guildwars2.com/images/f/f3/Explosive_Powder.png", 0, 99526, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(738, "Shaped Charge", "0.5% per stack vuln", DamageSource.NoPets, 0.5, DamageType.Power, DamageType.All, ParseHelper.Source.Engineer, ByStack, "https://wiki.guildwars2.com/images/f/f3/Explosive_Powder.png", 99526, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifierTarget(ProfHelper.NumberOfConditionsID, "Modified Ammunition", "2% per condition on target", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ParseHelper.Source.Engineer, ByStack, "https://wiki.guildwars2.com/images/9/94/Modified_Ammunition.png", DamageModifierMode.All),
        };
        private static readonly List<DamageModifier> _thiefDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifier(34659, "Lead Attacks", "1% (10s) per initiative spent", DamageSource.NoPets, 1.0, DamageType.All, DamageType.All, ParseHelper.Source.Thief, ByStack, "https://wiki.guildwars2.com/images/0/01/Lead_Attacks.png", DamageModifierMode.All),
            new BuffDamageModifier(32200, "Lotus Training", "10% cDam (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, ParseHelper.Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/e/ea/Lotus_Training.png", DamageModifierMode.All),
            new BuffDamageModifier(33162, "Bounding Dodger", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/3/30/Bounding_Dodger.png", DamageModifierMode.PvE),
            new BuffDamageModifier(33162, "Bounding Dodger", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/3/30/Bounding_Dodger.png", 0, 102321, DamageModifierMode.sPvPWvW),
            new BuffDamageModifier(33162, "Bounding Dodger", "15% (4s) after dodging", DamageSource.NoPets, 15.0, DamageType.Power, DamageType.All, ParseHelper.Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/3/30/Bounding_Dodger.png", 102321, ulong.MaxValue, DamageModifierMode.sPvPWvW),
            new BuffDamageModifierTarget(742, "Weakening Strikes", "7% if weakness on target", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParseHelper.Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/7/7c/Weakening_Strikes.png", 96406, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifierTarget(ProfHelper.NumberOfConditionsID, "Exposed Weakness", "2% per condition on target", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ParseHelper.Source.Thief, ByStack, "https://wiki.guildwars2.com/images/0/02/Exposed_Weakness.png", 90455, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifierTarget(ProfHelper.NumberOfConditionsID, "Exposed Weakness", "10% if condition on target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Thief, ByPresence, "https://wiki.guildwars2.com/images/0/02/Exposed_Weakness.png", 0, 90455, DamageModifierMode.PvE),
            new DamageLogDamageModifier("Executioner", "20% if target <50% HP", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ParseHelper.Source.Thief,"https://wiki.guildwars2.com/images/9/93/Executioner.png", x => x.AgainstUnderFifty, ByPresence, DamageModifierMode.All),
            new DamageLogDamageModifier("Ferocious Strikes", "10% on critical strikes if target >50%", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Thief,"https://wiki.guildwars2.com/images/d/d1/Ferocious_Strikes.png", x => !x.AgainstUnderFifty && x.HasCrit, ByPresence, DamageModifierMode.All),
            //new BuffDamageModifierTarget(Boon.GetBoonByName("Crippled").ID, "Ankle Shots", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.Power, ParseHelper.Source.Thief, _byPresence, "https://wiki.guildwars2.com/images/b/b4/Unscathed_Contender.png"), // It's not always possible to detect the presence of pistol and the trait is additive with itself. Staff master is worse as we can't detect endurance at all
            new DamageLogDamageModifier("Twin Fangs","7% over 90%", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParseHelper.Source.Thief,"https://wiki.guildwars2.com/images/d/d1/Ferocious_Strikes.png", x => x.IsOverNinety && x.HasCrit, ByPresence, DamageModifierMode.All),
            new BuffDamageModifier(ProfHelper.NumberOfBoonsID, "Premeditation", "1% per boon",DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ParseHelper.Source.Deadeye, ByStack, "https://wiki.guildwars2.com/images/d/d7/Premeditation.png", DamageModifierMode.All),
        };
        private static readonly List<DamageModifier> _rangerDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifier(33902, "Sic 'Em!", "40%", DamageSource.NoPets, 40.0, DamageType.Power, DamageType.All, ParseHelper.Source.Ranger, ByPresence, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png", DamageModifierMode.PvE),
            new DamageLogDamageModifier("Hunter's Tactics", "10% while flanking", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Ranger,"https://wiki.guildwars2.com/images/b/bb/Hunter%27s_Tactics.png", x => x.IsFlanking , ByPresence, 102321, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(30673, "Light on your Feet", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Ranger, ByPresence, "https://wiki.guildwars2.com/images/2/22/Light_on_your_Feet.png", DamageModifierMode.All),
            new BuffDamageModifier(45600, "Twice as Vicious", "5% (4s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.All, DamageType.All, ParseHelper.Source.Soulbeast, ByPresence, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png",0 ,97950, DamageModifierMode.PvE),
            new BuffDamageModifier(45600, "Twice as Vicious", "5% (10s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.All, DamageType.All, ParseHelper.Source.Soulbeast, ByPresence, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png", 97950, 102321, DamageModifierMode.PvE),
            new BuffDamageModifier(45600, "Twice as Vicious", "10% (10s) after disabling foe", DamageSource.NoPets, 10.0, DamageType.All, DamageType.All, ParseHelper.Source.Soulbeast, ByPresence, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png", 102321, ulong.MaxValue, DamageModifierMode.PvE),
            new BuffDamageModifier(45600, "Twice as Vicious", "5% (10s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.All, DamageType.All, ParseHelper.Source.Soulbeast, ByPresence, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png", 102321, ulong.MaxValue, DamageModifierMode.sPvPWvW),
            new BuffDamageModifier(ProfHelper.NumberOfBoonsID, "Bountiful Hunter", "1% per boon", DamageSource.All, 1.0, DamageType.Power, DamageType.All, ParseHelper.Source.Ranger, ByStack, "https://wiki.guildwars2.com/images/2/25/Bountiful_Hunter.png", DamageModifierMode.All),
            new BuffDamageModifier(725, "Furious Strength", "7% under fury", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParseHelper.Source.Soulbeast, ByStack, "https://wiki.guildwars2.com/images/c/ca/Furious_Strength.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(new long[] { 872, 833, 721, 727, 791, 722, 27705}, "Predator's Onslaught", "15% to disabled or movement-impaired foes", DamageSource.All, 15.0, DamageType.Power, DamageType.All, ParseHelper.Source.Ranger, ByPresence, "https://wiki.guildwars2.com/images/a/ac/Predator%27s_Onslaught.png", DamageModifierMode.All)
            // TODO Predator's Onslaught ? can daze and stun be tracked?
        };
        private static readonly List<DamageModifier> _mesmerDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifier(49058, "Compounding Power", "2% per stack (8s) after creating an illusion ", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ParseHelper.Source.Mesmer, ByStack, "https://wiki.guildwars2.com/images/e/e5/Compounding_Power.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(738, "Fragility", "0.5% per stack vuln on target", DamageSource.NoPets, 0.5, DamageType.Power, DamageType.All, ParseHelper.Source.Mesmer, ByStack, "https://wiki.guildwars2.com/images/3/33/Fragility.png", DamageModifierMode.All),
            // Phantasmal Force would require activating buff tracking on minions, huge performance impact and some code impact
            // TOCHECK Superiority Complex
            new BuffDamageModifierTarget(26766, "Danger Time", "10% crit damage on slowed target", DamageSource.All, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Chronomancer, ByPresence, "https://wiki.guildwars2.com/images/3/33/Fragility.png", DamageModifierMode.All, (x => x.HasCrit)),
            new BuffDamageModifierTarget(ProfHelper.NumberOfBoonsID, "Vicious Expression", "25% on boonless target",  DamageSource.All, 25.0, DamageType.Power, DamageType.All, ParseHelper.Source.Mesmer, ByAbsence, "https://wiki.guildwars2.com/images/f/f6/Confounding_Suggestions.png", 102321, 102389, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(ProfHelper.NumberOfBoonsID, "Vicious Expression", "15% on boonless target",  DamageSource.All, 15.0, DamageType.Power, DamageType.All, ParseHelper.Source.Mesmer, ByAbsence, "https://wiki.guildwars2.com/images/f/f6/Confounding_Suggestions.png", 102389, ulong.MaxValue, DamageModifierMode.All),
        };
        private static readonly List<DamageModifier> _necromancerDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(ProfHelper.NumberOfBoonsID, "Spiteful Talisman", "10% on boonless target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Necromancer, ByAbsence, "https://wiki.guildwars2.com/images/9/96/Spiteful_Talisman.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(791, "Dread", "33% on feared target", DamageSource.NoPets, 33.0, DamageType.Power, DamageType.All, ParseHelper.Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/e/e2/Unholy_Fervor.png",92069, 104844, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(791, "Dread", "20% on feared target", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ParseHelper.Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/e/e2/Unholy_Fervor.png", 0, 92069, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(791, "Dread", "15% on feared target", DamageSource.NoPets, 15.0, DamageType.Power, DamageType.All, ParseHelper.Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/e/e2/Unholy_Fervor.png",102321, 104844, DamageModifierMode.sPvPWvW),
            new DamageLogDamageModifier("Close to Death", "20% below 90% HP", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ParseHelper.Source.Necromancer,"https://wiki.guildwars2.com/images/b/b2/Close_to_Death.png", x => x.AgainstUnderFifty, ByPresence, DamageModifierMode.All),
            new BuffDamageModifierTarget(722, "Cold Shoulder", "15% on chilled target", DamageSource.NoPets, 15.0, DamageType.Power, DamageType.All, ParseHelper.Source.Reaper, ByPresence, "https://wiki.guildwars2.com/images/7/78/Cold_Shoulder.png", 95535, ulong.MaxValue, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(722, "Cold Shoulder", "10% on chilled target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Reaper, ByPresence, "https://wiki.guildwars2.com/images/7/78/Cold_Shoulder.png", 95535, ulong.MaxValue, DamageModifierMode.sPvPWvW),
            new BuffDamageModifierTarget(722, "Cold Shoulder", "10% on chilled target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Reaper, ByPresence, "https://wiki.guildwars2.com/images/7/78/Cold_Shoulder.png", 0, 95535, DamageModifierMode.PvE),
            new BuffDamageModifier(53489, "Soul Barbs", "10% after entering or exiting shroud", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/b/bd/Soul_Barbs.png", 94051, ulong.MaxValue, DamageModifierMode.All),
        };
        private static readonly List<DamageModifier> _elementalistDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifier(31353, "Harmonious Conduit", "10% (4s) after overload", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Tempest, ByPresence, "https://wiki.guildwars2.com/images/b/b3/Harmonious_Conduit.png", 0 , 99526, DamageModifierMode.PvE),
            new BuffDamageModifier(13342, "Persisting Flames", "1% per stack", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ParseHelper.Source.Elementalist, ByStack, "https://wiki.guildwars2.com/images/5/5f/Persisting_Flames.png", 104844 , ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(31353, "Transcendent Tempest", "7% (7s) after overload", DamageSource.NoPets, 7.0, DamageType.All, DamageType.All, ParseHelper.Source.Tempest, ByPresence, "https://wiki.guildwars2.com/images/a/ac/Transcendent_Tempest_%28effect%29.png", 99526 , ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(42061, "Weaver's Prowess", "10% cDam (8s) after switching element",  DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, ParseHelper.Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/7/75/Weaver%27s_Prowess.png", DamageModifierMode.All),
            new BuffDamageModifier(42416, "Elements of Rage", "10% (8s) after double attuning", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/a/a2/Elements_of_Rage.png", DamageModifierMode.All),
            new BuffDamageModifier(new long[] { 5585, ProfHelper.FireWater, ProfHelper.FireAir, ProfHelper.FireEarth, ProfHelper.FireDual }, "Pyromancer's Training", "10% while fire attuned", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/e/e6/Pyromancer%27s_Training.png", 0, 97950, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(737, "Burning Rage", "10% on burning target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/b/bd/Burning_Rage.png", 0, 97950, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(737, "Pyromancer's Training", "10% on burning target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/e/e6/Pyromancer%27s_Training.png", 97950, ulong.MaxValue, DamageModifierMode.All),
            new DamageLogDamageModifier( "Bolt to the Heart", "20% if target <50% HP", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ParseHelper.Source.Elementalist,"https://wiki.guildwars2.com/images/f/f8/Bolt_to_the_Heart.png", x => x.AgainstUnderFifty, ByPresence, DamageModifierMode.All),
            new BuffDamageModifierTarget(736, "Serrated Stones", "5% to bleeding target", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, ParseHelper.Source.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/6/60/Serrated_Stones.png", DamageModifierMode.All),
            new DamageLogDamageModifier("Aquamancer's Training", "10% over 90% HP", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Elementalist,"https://wiki.guildwars2.com/images/8/81/Aquamancer%27s_Training.png", x => x.IsOverNinety, ByPresence, 0, 97950, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(738, new long[] { 5586, ProfHelper.WaterAir, ProfHelper.WaterEarth, ProfHelper.WaterFire, ProfHelper.WaterDual}, "Piercing Shards w/ Water", "20% on vuln target while on water", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ParseHelper.Source.Elementalist, ByPresence, ByPresence, "https://wiki.guildwars2.com/images/4/4b/Piercing_Shards.png", 97950, ulong.MaxValue, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(738, new long[] { 5586, ProfHelper.WaterAir, ProfHelper.WaterEarth, ProfHelper.WaterFire, ProfHelper.WaterDual}, "Piercing Shards", "10% on vuln target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Elementalist, ByPresence, ByAbsence, "https://wiki.guildwars2.com/images/4/4b/Piercing_Shards.png", 97950, ulong.MaxValue, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(738, new long[] { 5586, ProfHelper.WaterAir, ProfHelper.WaterEarth, ProfHelper.WaterFire, ProfHelper.WaterDual}, "Piercing Shards w/ Water", "10% on vuln target while on water", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Elementalist, ByPresence, ByPresence, "https://wiki.guildwars2.com/images/4/4b/Piercing_Shards.png", 97950, ulong.MaxValue, DamageModifierMode.sPvPWvW),
            new BuffDamageModifierTarget(738, new long[] { 5586, ProfHelper.WaterAir, ProfHelper.WaterEarth, ProfHelper.WaterFire, ProfHelper.WaterDual}, "Piercing Shards", "5% on vuln target", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, ParseHelper.Source.Elementalist, ByPresence, ByAbsence, "https://wiki.guildwars2.com/images/4/4b/Piercing_Shards.png", 97950, ulong.MaxValue, DamageModifierMode.sPvPWvW),
            new BuffDamageModifierTarget(738, new long[] { 5586, ProfHelper.WaterAir, ProfHelper.WaterEarth, ProfHelper.WaterFire, ProfHelper.WaterDual}, "Piercing Shards", "20% on vuln target while on water", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Elementalist, ByPresence, ByPresence, "https://wiki.guildwars2.com/images/4/4b/Piercing_Shards.png", 0, 97950, DamageModifierMode.PvE),
            //new DamageLogDamageModifier("Flow like Water", "10% over 75% HP", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Elementalist,"https://wiki.guildwars2.com/images/0/0a/Flow_like_Water.png", x => x.IsOverNinety, ByPresence, 97950, ulong.MaxValue),
            new BuffDamageModifier(ProfHelper.NumberOfBoonsID, "Bountiful Power", "2% per boon", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ParseHelper.Source.Elementalist, ByStack, "https://wiki.guildwars2.com/images/7/75/Bountiful_Power.png", DamageModifierMode.All),
            new BuffDamageModifier(45110, "Woven Fire", "20%", DamageSource.NoPets, 20.0, DamageType.Condition, DamageType.All, ParseHelper.Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/b/b1/Woven_Fire.png", DamageModifierMode.All),
            new BuffDamageModifier(45267, "Perfect Weave", "20%", DamageSource.NoPets, 20.0, DamageType.Condition, DamageType.All, ParseHelper.Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/2/2a/Weave_Self.png", DamageModifierMode.All),
            new BuffDamageModifier(new long[] { 719, 5974}, "Swift Revenge", "7% under swiftness/superspeed", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParseHelper.Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/9/94/Swift_Revenge.png", 0, 97950, DamageModifierMode.PvE),
            new BuffDamageModifier(new long[] { 719, 5974}, "Swift Revenge", "10% under swiftness/superspeed", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/9/94/Swift_Revenge.png", 97950, ulong.MaxValue, DamageModifierMode.All)
        };

        public static List<List<DamageModifier>> AllDamageModifiers = new List<List<DamageModifier>>
        {
            _itemDamageModifiers,
            _commonDamageModifiers,
            _fightSpecificDamageModifiers,
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
