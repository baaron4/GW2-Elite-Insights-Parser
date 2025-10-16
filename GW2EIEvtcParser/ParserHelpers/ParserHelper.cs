using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser;

public static class ParserHelper
{

    internal static readonly AgentItem _unknownAgent = new();
    internal static readonly AgentItem _nullAgent = new();

    public const int CombatReplayPollingRate = 300;
    internal const uint CombatReplaySkillDefaultSizeInPixel = 22;
    internal const uint CombatReplaySkillDefaultSizeInWorld = 90;
    internal const uint CombatReplayOverheadDefaultSizeInPixel = 20;
    internal const uint CombatReplayOverheadProgressBarMinorSizeInPixel = 20;
    internal const uint CombatReplayOverheadProgressBarMajorSizeInPixel = 35;
    internal const float CombatReplayOverheadDefaultOpacity = 0.8f;

    //TODO(Rennorb) @cleanup: Rename this whole block. These are rounding precisions, maybe 'digits' if im being generous.
    internal const int BuffDigit = 3;
    internal const int DamageModGainDigit = 3;
    internal const int AccelerationDigit = 3;
    internal const int CombatReplayDataDigit = 3;
    public const int TimeDigit = 3;

    public const long ServerDelayConstant = 10;
    internal const long BuffSimulatorDelayConstant = 15;
    internal const long BuffSimulatorStackActiveDelayConstant = 50;
    internal const long WeaponSwapDelayConstant = 75;
    internal const long TimeThresholdConstant = 150;

    internal const long InchDistanceThreshold = 10;

    public const long MinimumInCombatDuration = 2200;

    internal const int PhaseTimeLimit = 1000;
    internal const int BreakbarPhaseTimeBuildup = 2000;


    public enum Source
    {
        Common,
        Item, Gear,
        // professions, sort alphabetically per base spec then add elite spec per expansion
        Elementalist, Tempest, Weaver, Catalyst, Evoker,
        Engineer, Scrapper, Holosmith, Mechanist, Amalgam,
        Guardian, Dragonhunter, Firebrand, Willbender, Luminary,
        Mesmer, Chronomancer, Mirage, Virtuoso, Troubadour,
        Necromancer, Reaper, Scourge, Harbinger, Ritualist,
        Ranger, Druid, Soulbeast, Untamed, Galeshot,
        Revenant, Herald, Renegade, Vindicator, Conduit,
        Thief, Daredevil, Deadeye, Specter, Antiquary,
        Warrior, Berserker, Spellbreaker, Bladesworn, Paragon,
        //
        PetSpecific,
        EncounterSpecific,
        FractalInstability,
        Unknown
    };

    public enum Spec
    {
        // professions, sort alphabetically per base spec then add elite spec per expansion
        Elementalist, Tempest, Weaver, Catalyst, Evoker,
        Engineer, Scrapper, Holosmith, Mechanist, Amalgam,
        Guardian, Dragonhunter, Firebrand, Willbender, Luminary,
        Mesmer, Chronomancer, Mirage, Virtuoso, Troubadour,
        Necromancer, Reaper, Scourge, Harbinger, Ritualist,
        Ranger, Druid, Soulbeast, Untamed, Galeshot,
        Revenant, Herald, Renegade, Vindicator, Conduit,
        Thief, Daredevil, Deadeye, Specter, Antiquary,
        Warrior, Berserker, Spellbreaker, Bladesworn, Paragon,
        //
        NPC, Gadget,
        Unknown
    };

    [Flags]
    public enum DamageType
    {
        All = 0,
        Power = 1 << 0,
        Strike = 1 << 1,
        Condition = 1 << 2,
        StrikeAndCondition = Strike | Condition, // Common
        LifeLeech = 1 << 3,
        StrikeAndLifeLeech = Strike | LifeLeech, // Common
        ConditionAndLifeLeech = Condition | LifeLeech, // Common
        StrikeAndConditionAndLifeLeech = Strike | Condition | LifeLeech, // Common
    };
    public static string DamageTypeToString(this DamageType damageType)
    {
        if (damageType == DamageType.All)
        {
            return "All Damage";
        }
        string str = "";
        bool addComa = false;
        if ((damageType & DamageType.Power) > 0)
        {
            if (addComa)
            {
                str += ", ";
            }
            str += "Power";
            addComa = true;
        }
        if ((damageType & DamageType.Strike) > 0)
        {
            if (addComa)
            {
                str += ", ";
            }
            str += "Strike";
            addComa = true;
        }
        if ((damageType & DamageType.Condition) > 0)
        {
            if (addComa)
            {
                str += ", ";
            }
            str += "Condition";
            addComa = true;
        }
        if ((damageType & DamageType.LifeLeech) > 0)
        {
            if (addComa)
            {
                str += ", ";
            }
            str += "Life Leech";
            addComa = true;
        }
        str += " Damage";
        return str;
    }
    public enum BuffEnum { Self, Group, OffGroup, Squad };

    internal static Dictionary<long, List<T>> GroupByTime<T>(IEnumerable<T> list) where T : TimeCombatEvent
    {
        var groupByTime = new Dictionary<long, List<T>>();
        foreach (T c in list)
        {
            long key = groupByTime.Keys.FirstOrDefault(x => Math.Abs(x - c.Time) < ServerDelayConstant);
            if (key != 0)
            {
                groupByTime[key].Add(c);
            }
            else
            {
                groupByTime[c.Time] =
                        [
                            c
                        ];
            }
        }
        return groupByTime;
    }

    internal static T MaxBy<T, TComparable>(this IEnumerable<T> en, Func<T, TComparable> evaluate) where TComparable : IComparable<TComparable>
    {
        return en.Select(t => (value: t, eval: evaluate(t)))
            .Aggregate((max, next) => next.eval.CompareTo(max.eval) > 0 ? next : max).value;
    }

    internal static T MinBy<T, TComparable>(this IEnumerable<T> en, Func<T, TComparable> evaluate) where TComparable : IComparable<TComparable>
    {
        return en.Select(t => (value: t, eval: evaluate(t)))
            .Aggregate((max, next) => next.eval.CompareTo(max.eval) < 0 ? next : max).value;
    }

    internal static string ToHexString(ReadOnlySpan<byte> bytes)
    {
        using var buffer = new ArrayPoolReturner<char>(bytes.Length * 2);
        AppendHexString(buffer, bytes);
        return new String(buffer);
    }
    internal static void AppendHexString(Span<char> destination, ReadOnlySpan<byte> bytes)
    {
        const string CHARSET = "0123456789ABCDEF";
        int offset = 0;
        foreach(var c in bytes)
        {
            destination[offset++] = CHARSET[(c & 0xf0) >> 4];
            destination[offset++] = CHARSET[c & 0x0f];
        }
    }

    internal static bool IsSupportedStateChange(StateChange state)
    {
        return state != StateChange.Unknown && state != StateChange.ReplInfo && state != StateChange.StatReset && state != StateChange.APIDelayed && state != StateChange.Idle && state != StateChange.AgentChange;
    }
    internal static bool IsSupportedStateChangeForInstanceLogs(StateChange state)
    {
        return state != StateChange.BuffInitial;
    }

    /*
    public static string UppercaseFirst(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }


    public static string FindPattern(string source, string regex)
    {
        if (string.IsNullOrEmpty(source))
        {
            return null;
        }

        Match match = Regex.Match(source, regex);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return null;
    }
    */
    public static Exception GetFinalException(Exception ex)
    {
        Exception final = ex;
        while (final.InnerException != null)
        {
            final = final.InnerException;
        }
        return final;
    }


    public static string ToDurationString(long duration)
    {
        var durationTimeSpan = TimeSpan.FromMilliseconds(Math.Abs(duration));
        string durationString = durationTimeSpan.ToString("mm") + "m " + durationTimeSpan.ToString("ss") + "s " + durationTimeSpan.Milliseconds + "ms";
        if (durationTimeSpan.Hours > 0)
        {
            durationString = durationTimeSpan.ToString("hh") + "h " + durationString;
        }
        if (duration < 0)
        {
            durationString = "-" + durationString;
        }
        return durationString;
    }

    /// <summary>
    /// Dictionary to find the <see cref="Spec"/> Specialization / Profession given a <see cref="string"/> as reference.
    /// </summary>
    private static IReadOnlyDictionary<string, Spec> ProfToSpecDictionary = new Dictionary<string, Spec>()
    {
        { "NPC", Spec.NPC },
        { "GDG", Spec.Gadget },
        //
        { "Galeshot", Spec.Galeshot },
        { "Untamed", Spec.Untamed },
        { "Druid", Spec.Druid },
        { "Soulbeast", Spec.Soulbeast },
        { "Ranger", Spec.Ranger },
        //
        { "Amalgam", Spec.Amalgam },
        { "Scrapper", Spec.Scrapper },
        { "Holosmith", Spec.Holosmith },
        { "Mechanist", Spec.Mechanist },
        { "Engineer", Spec.Engineer },
        //
        { "Antiquary", Spec.Antiquary },
        { "Specter", Spec.Specter },
        { "Daredevil", Spec.Daredevil },
        { "Deadeye", Spec.Deadeye },
        { "Thief", Spec.Thief },
        //
        { "Evoker", Spec.Evoker },
        { "Catalyst", Spec.Catalyst },
        { "Weaver", Spec.Weaver },
        { "Tempest", Spec.Tempest },
        { "Elementalist", Spec.Elementalist },
        //
        { "Troubadour", Spec.Troubadour },
        { "Virtuoso", Spec.Virtuoso },
        { "Mirage", Spec.Mirage },
        { "Chronomancer", Spec.Chronomancer },
        { "Mesmer", Spec.Mesmer },
        //
        { "Ritualist", Spec.Ritualist },
        { "Harbinger", Spec.Harbinger },
        { "Scourge", Spec.Scourge },
        { "Reaper", Spec.Reaper },
        { "Necromancer", Spec.Necromancer },
        //
        { "Paragon", Spec.Paragon },
        { "Bladesworn", Spec.Bladesworn },
        { "Spellbreaker", Spec.Spellbreaker },
        { "Berserker", Spec.Berserker },
        { "Warrior", Spec.Warrior },
        //
        { "Luminary", Spec.Luminary },
        { "Willbender", Spec.Willbender },
        { "Firebrand", Spec.Firebrand },
        { "Dragonhunter", Spec.Dragonhunter },
        { "Guardian", Spec.Guardian },
        //
        { "Conduit", Spec.Conduit },
        { "Vindicator", Spec.Vindicator },
        { "Renegade", Spec.Renegade },
        { "Herald", Spec.Herald },
        { "Revenant", Spec.Revenant },
        //
        { "", Spec.Unknown },
    };

    internal static Spec ProfToSpec(string prof)
    {
        return ProfToSpecDictionary.TryGetValue(prof, out Spec spec) ? spec : Spec.Unknown;
    }

    /// <summary>
    /// Dictionary to find the base <see cref="Spec"/> Profession given a <see cref="Spec"/> Elite Specialization.
    /// </summary>
    private static IReadOnlyDictionary<Spec, Spec> SpecToBaseProfDictionary = new Dictionary<Spec, Spec>()
    {
        { Spec.Galeshot, Spec.Ranger },
        { Spec.Untamed, Spec.Ranger },
        { Spec.Soulbeast, Spec.Ranger },
        { Spec.Druid, Spec.Ranger },
        { Spec.Ranger, Spec.Ranger },
        //
        { Spec.Amalgam, Spec.Engineer },
        { Spec.Mechanist, Spec.Engineer },
        { Spec.Holosmith, Spec.Engineer },
        { Spec.Scrapper, Spec.Engineer },
        { Spec.Engineer, Spec.Engineer },
        //
        { Spec.Antiquary, Spec.Thief },
        { Spec.Specter, Spec.Thief },
        { Spec.Deadeye, Spec.Thief },
        { Spec.Daredevil, Spec.Thief },
        { Spec.Thief, Spec.Thief },
        //
        { Spec.Evoker, Spec.Elementalist },
        { Spec.Catalyst, Spec.Elementalist },
        { Spec.Weaver, Spec.Elementalist },
        { Spec.Tempest, Spec.Elementalist },
        { Spec.Elementalist, Spec.Elementalist },
        //
        { Spec.Troubadour, Spec.Mesmer },
        { Spec.Virtuoso, Spec.Mesmer },
        { Spec.Mirage, Spec.Mesmer },
        { Spec.Chronomancer, Spec.Mesmer },
        { Spec.Mesmer, Spec.Mesmer },
        //
        { Spec.Ritualist, Spec.Necromancer },
        { Spec.Harbinger, Spec.Necromancer },
        { Spec.Scourge, Spec.Necromancer },
        { Spec.Reaper, Spec.Necromancer },
        { Spec.Necromancer, Spec.Necromancer },
        //
        { Spec.Paragon, Spec.Warrior },
        { Spec.Bladesworn, Spec.Warrior },
        { Spec.Spellbreaker, Spec.Warrior },
        { Spec.Berserker, Spec.Warrior },
        { Spec.Warrior, Spec.Warrior },
        //
        { Spec.Luminary, Spec.Guardian },
        { Spec.Willbender, Spec.Guardian },
        { Spec.Firebrand, Spec.Guardian },
        { Spec.Dragonhunter, Spec.Guardian },
        { Spec.Guardian, Spec.Guardian },
        //
        { Spec.Conduit, Spec.Revenant },
        { Spec.Vindicator, Spec.Revenant },
        { Spec.Renegade, Spec.Revenant },
        { Spec.Herald, Spec.Revenant },
        { Spec.Revenant, Spec.Revenant },
    };

    internal static Spec SpecToBaseSpec(Spec spec)
    {
        return SpecToBaseProfDictionary.TryGetValue(spec, out Spec prof) ? prof : spec;
    }

    /// <summary>
    /// Dictionary to find the <see cref="Source"/> given a specific <see cref="Spec"/>.
    /// </summary>
    private static IReadOnlyDictionary<Spec, List<Source>> SpecToSourcesDictionary = new Dictionary<Spec, List<Source>>()
    {
        { Spec.Galeshot, new List<Source> { Source.Ranger, Source.Galeshot } },
        { Spec.Untamed, new List<Source> { Source.Ranger, Source.Untamed } },
        { Spec.Soulbeast, new List<Source> { Source.Ranger, Source.Soulbeast } },
        { Spec.Druid, new List<Source> { Source.Ranger, Source.Druid } },
        { Spec.Ranger, new List<Source> { Source.Ranger } },
        //
        { Spec.Amalgam, new List<Source> { Source.Engineer, Source.Amalgam } },
        { Spec.Mechanist, new List<Source> { Source.Engineer, Source.Mechanist } },
        { Spec.Holosmith, new List<Source> { Source.Engineer, Source.Holosmith } },
        { Spec.Scrapper, new List<Source> { Source.Engineer, Source.Scrapper } },
        { Spec.Engineer, new List<Source> { Source.Engineer } },
        //
        { Spec.Antiquary, new List<Source> { Source.Thief, Source.Antiquary } },
        { Spec.Specter, new List<Source> { Source.Thief, Source.Specter } },
        { Spec.Deadeye, new List<Source> { Source.Thief, Source.Deadeye } },
        { Spec.Daredevil, new List<Source> { Source.Thief, Source.Daredevil } },
        { Spec.Thief, new List<Source> { Source.Thief } },
        //
        { Spec.Evoker, new List<Source> { Source.Elementalist, Source.Evoker } },
        { Spec.Catalyst, new List<Source> { Source.Elementalist, Source.Catalyst } },
        { Spec.Weaver, new List<Source> { Source.Elementalist, Source.Weaver } },
        { Spec.Tempest, new List<Source> { Source.Elementalist, Source.Tempest } },
        { Spec.Elementalist, new List<Source> { Source.Elementalist } },
        //
        { Spec.Troubadour, new List<Source> { Source.Mesmer, Source.Troubadour } },
        { Spec.Virtuoso, new List<Source> { Source.Mesmer, Source.Virtuoso } },
        { Spec.Mirage, new List<Source> { Source.Mesmer, Source.Mirage } },
        { Spec.Chronomancer, new List<Source> { Source.Mesmer, Source.Chronomancer } },
        { Spec.Mesmer, new List<Source> { Source.Mesmer } },
        //
        { Spec.Ritualist, new List<Source> { Source.Necromancer, Source.Ritualist } },
        { Spec.Harbinger, new List<Source> { Source.Necromancer, Source.Harbinger } },
        { Spec.Scourge, new List<Source> { Source.Necromancer, Source.Scourge } },
        { Spec.Reaper, new List<Source> { Source.Necromancer, Source.Reaper } },
        { Spec.Necromancer, new List<Source> { Source.Necromancer } },
        //
        { Spec.Paragon, new List<Source> { Source.Warrior, Source.Paragon } },
        { Spec.Bladesworn, new List<Source> { Source.Warrior, Source.Bladesworn } },
        { Spec.Spellbreaker, new List<Source> { Source.Warrior, Source.Spellbreaker } },
        { Spec.Berserker, new List<Source> { Source.Warrior, Source.Berserker } },
        { Spec.Warrior, new List<Source> { Source.Warrior } },
        //
        { Spec.Luminary, new List<Source> { Source.Guardian, Source.Luminary } },
        { Spec.Willbender, new List<Source> { Source.Guardian, Source.Willbender } },
        { Spec.Firebrand, new List<Source> { Source.Guardian, Source.Firebrand } },
        { Spec.Dragonhunter, new List<Source> { Source.Guardian, Source.Dragonhunter } },
        { Spec.Guardian, new List<Source> { Source.Guardian } },
        //
        { Spec.Conduit, new List<Source> { Source.Revenant, Source.Conduit } },
        { Spec.Vindicator, new List<Source> { Source.Revenant, Source.Vindicator } },
        { Spec.Renegade, new List<Source> { Source.Revenant, Source.Renegade } },
        { Spec.Herald, new List<Source> { Source.Revenant, Source.Herald } },
        { Spec.Revenant, new List<Source> { Source.Revenant } },
    };

    public static IReadOnlyList<Source> SpecToSources(Spec spec)
    {
        return SpecToSourcesDictionary.TryGetValue(spec, out var sourceList) ? sourceList : [ ];
    }

    internal static string GetHighResolutionProfIcon(Spec spec)
    {
        return ParserIcons.HighResProfIcons.TryGetValue(spec, out var icon) ? icon : ParserIcons.UnknownProfessionIcon;
    }

    internal static string GetProfIcon(Spec spec)
    {
        return ParserIcons.BaseResProfIcons.TryGetValue(spec, out var icon) ? icon : ParserIcons.UnknownProfessionIcon;
    }

    internal static string GetGadgetIcon()
    {
        return ParserIcons.GenericGadgetIcon;
    }

    internal static string GetNPCIcon(int id)
    {
        if (id == 0)
        {
            return ParserIcons.UnknownNPCIcon;
        }

        TargetID target = GetTargetID(id);
        if (target != TargetID.Unknown)
        {
            return ParserIcons.TargetNPCIcons.TryGetValue(target, out var targetIcon) ? targetIcon : ParserIcons.GenericEnemyIcon;
        }
        MinionID minion = GetMinionID(id);
        if (minion != MinionID.Unknown)
        {
            return ParserIcons.MinionNPCIcons.TryGetValue(minion, out var minionIcon) ? minionIcon : ParserIcons.GenericEnemyIcon;
        }

        return ParserIcons.GenericEnemyIcon;
    }

    public static IReadOnlyDictionary<BuffAttribute, string> BuffAttributesStrings { get; private set; } = new Dictionary<BuffAttribute, string>()
    {
        { BuffAttribute.Power, "Power" },
        { BuffAttribute.PowerSidekick, "Power" },
        { BuffAttribute.Precision, "Precision" },
        { BuffAttribute.PrecisionSidekick, "Precision" },
        { BuffAttribute.Toughness, "Toughness" },
        { BuffAttribute.ToughnessSidekick, "Toughness" },
        { BuffAttribute.DefensePercent, "Defense" },
        { BuffAttribute.Vitality, "Vitality" },
        { BuffAttribute.VitalitySidekick, "Vitality" },
        { BuffAttribute.VitalityPercent, "Vitality" },
        { BuffAttribute.Ferocity, "Ferocity" },
        { BuffAttribute.FerocitySidekick, "Ferocity" },
        { BuffAttribute.Healing, "Healing Power" },
        { BuffAttribute.HealingSidekick, "Healing Power" },
        { BuffAttribute.Condition, "Condition Damage" },
        { BuffAttribute.ConditionSidekick, "Condition Damage" },
        { BuffAttribute.Concentration, "Concentration" },
        { BuffAttribute.ConcentrationSidekick, "Concentration" },
        { BuffAttribute.Expertise, "Expertise" },
        { BuffAttribute.ExpertiseSidekick, "Expertise" },
        { BuffAttribute.AllStatsPercent, "All Stats" },
        { BuffAttribute.FishingPower, "Fishing Power" },
        { BuffAttribute.Armor, "Armor" },
        { BuffAttribute.Agony, "Agony" },
        { BuffAttribute.StatOutgoing, "Stat Increase" },
        { BuffAttribute.FlatOutgoing, "Flat Increase" },
        { BuffAttribute.PhysOutgoing, "Outgoing Strike Damage" },
        { BuffAttribute.CondOutgoing, "Outgoing Condition Damage" },
        { BuffAttribute.SiphonOutgoing, "Outgoing Life Leech Damage" },
        { BuffAttribute.SiphonIncomingAdditive1, "Incoming Life Leech Damage" },
        { BuffAttribute.SiphonIncomingAdditive2, "Incoming Life Leech Damage" },
        { BuffAttribute.CondIncomingAdditive, "Incoming Condition Damage" },
        { BuffAttribute.CondIncomingMultiplicative, "Incoming Condition Damage (Mult)" },
        { BuffAttribute.PhysIncomingAdditive, "Incoming Strike Damage" },
        { BuffAttribute.PhysIncomingMultiplicative, "Incoming Strike Damage (Mult)" },
        { BuffAttribute.AttackSpeed, "Attack Speed" },
        { BuffAttribute.ConditionDurationOutgoing, "Outgoing Condition Duration" },
        { BuffAttribute.BoonDurationOutgoing, "Outgoing Boon Duration" },
        { BuffAttribute.DamageFormulaSquaredLevel, "Damage Formula" },
        { BuffAttribute.DamageFormula, "Damage Formula" },
        { BuffAttribute.GlancingBlow, "Glancing Blow" },
        { BuffAttribute.CriticalChance, "Critical Chance" },
        { BuffAttribute.StrikeDamageToHP, "Strike Damage to Health" },
        { BuffAttribute.ConditionDamageToHP, "Condition Damage to Health" },
        { BuffAttribute.SkillActivationDamageFormula, "Damage Formula on Skill Activation" },
        { BuffAttribute.MovementActivationDamageFormula, "Damage Formula based on Movement" },
        { BuffAttribute.EnduranceRegeneration, "Endurance Regeneration" },
        { BuffAttribute.HealingEffectivenessIncomingNonStacking, "Incoming Healing Effectiveness" },
        { BuffAttribute.HealingEffectivenessIncomingAdditive, "Incoming Healing Effectiveness" },
        { BuffAttribute.HealingEffectivenessIncomingMultiplicative, "Incoming Healing Effectiveness (Mult)" },
        { BuffAttribute.HealingEffectivenessConvOutgoing, "Outgoing Healing Effectiveness" },
        { BuffAttribute.HealingEffectivenessOutgoingAdditive, "Outgoing Healing Effectiveness" },
        { BuffAttribute.HealingOutputFormula, "Healing Formula" },
        { BuffAttribute.ExperienceFromKills, "Experience From Kills" },
        { BuffAttribute.ExperienceFromAll, "Experience From All" },
        { BuffAttribute.GoldFind, "GoldFind" },
        { BuffAttribute.MovementSpeed, "Movement Speed" },
        { BuffAttribute.MovementSpeedStacking, "Movement Speed (Stacking)" },
        { BuffAttribute.MovementSpeedStacking2, "Movement Speed (Stacking)" },
        { BuffAttribute.MaximumHP, "Maximum Health" },
        { BuffAttribute.KarmaBonus, "Karma Bonus" },
        { BuffAttribute.SkillRechargeSpeedIncrease, "Skill Recharge Speed Increase" },
        { BuffAttribute.MagicFind, "Magic Find" },
        { BuffAttribute.WXP, "WXP" },
        { BuffAttribute.Unknown, "Unknown" },
    };

    public static IReadOnlyDictionary<BuffAttribute, string> BuffAttributesPercent { get; private set; } = new Dictionary<BuffAttribute, string>()
    {
        { BuffAttribute.FlatOutgoing, "%" },
        { BuffAttribute.PhysOutgoing, "%" },
        { BuffAttribute.CondOutgoing, "%" },
        { BuffAttribute.CondIncomingAdditive, "%" },
        { BuffAttribute.CondIncomingMultiplicative, "%" },
        { BuffAttribute.PhysIncomingAdditive, "%" },
        { BuffAttribute.PhysIncomingMultiplicative, "%" },
        { BuffAttribute.AttackSpeed, "%" },
        { BuffAttribute.ConditionDurationOutgoing, "%" },
        { BuffAttribute.BoonDurationOutgoing, "%" },
        { BuffAttribute.GlancingBlow, "%" },
        { BuffAttribute.CriticalChance, "%" },
        { BuffAttribute.StrikeDamageToHP, "%" },
        { BuffAttribute.ConditionDamageToHP, "%" },
        { BuffAttribute.EnduranceRegeneration, "%" },
        { BuffAttribute.HealingEffectivenessIncomingNonStacking, "%" },
        { BuffAttribute.HealingEffectivenessIncomingAdditive, "%" },
        { BuffAttribute.HealingEffectivenessIncomingMultiplicative, "%" },
        { BuffAttribute.SiphonOutgoing, "%" },
        { BuffAttribute.SiphonIncomingAdditive1, "%" },
        { BuffAttribute.SiphonIncomingAdditive2, "%" },
        { BuffAttribute.HealingEffectivenessConvOutgoing , "%" },
        { BuffAttribute.HealingEffectivenessOutgoingAdditive , "%" },
        { BuffAttribute.ExperienceFromKills, "%" },
        { BuffAttribute.ExperienceFromAll, "%" },
        { BuffAttribute.GoldFind, "%" },
        { BuffAttribute.MovementSpeed, "%" },
        { BuffAttribute.MovementSpeedStacking, "%" },
        { BuffAttribute.MovementSpeedStacking2, "%" },
        { BuffAttribute.MaximumHP, "%" },
        { BuffAttribute.KarmaBonus, "%" },
        { BuffAttribute.SkillRechargeSpeedIncrease, "%" },
        { BuffAttribute.MagicFind, "%" },
        { BuffAttribute.WXP, "%" },
        { BuffAttribute.DefensePercent, "%" },
        { BuffAttribute.VitalityPercent, "%" },
        { BuffAttribute.AllStatsPercent, "%" },
        { BuffAttribute.MovementActivationDamageFormula, " adds" },
        { BuffAttribute.SkillActivationDamageFormula, " replaces" },
        { BuffAttribute.Unknown, "Unknown" },
    };

    public static bool IsKnownMinionID(AgentItem minion, Spec spec)
    {
        if (minion.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        int id = minion.ID;
        bool res = ProfHelper.IsKnownMinionID(id);
        var baseSpec = SpecToBaseSpec(spec);
        switch (baseSpec)
        {
            //
            case Spec.Elementalist:
                res |= ElementalistHelper.IsKnownMinionID(id);
                res |= EvokerHelper.IsKnownMinionID(id);
                break;
            //
            case Spec.Necromancer:
                res |= NecromancerHelper.IsKnownMinionID(id);
                res |= ReaperHelper.IsKnownMinionID(id);
                res |= RitualistHelper.IsKnownMinionID(id);
                break;
            //
            case Spec.Mesmer:
                res |= MesmerHelper.IsKnownMinionID(id);
                res |= ChronomancerHelper.IsKnownMinionID(id);
                res |= MirageHelper.IsKnownMinionID(id);
                res |= VirtuosoHelper.IsKnownMinionID(id);
                break;
            //
            case Spec.Thief:
                res |= ThiefHelper.IsKnownMinionID(id);
                res |= DaredevilHelper.IsKnownMinionID(id);
                res |= DeadeyeHelper.IsKnownMinionID(id);
                res |= SpecterHelper.IsKnownMinionID(id);
                res |= AntiquaryHelper.IsKnownMinionID(id);
                break;
            //
            case Spec.Engineer:
                res |= EngineerHelper.IsKnownMinionID(id);
                res |= ScrapperHelper.IsKnownMinionID(id);
                res |= MechanistHelper.IsKnownMinionID(id);
                break;
            //
            case Spec.Ranger:
                res |= RangerHelper.IsKnownMinionID(id);
                break;
            //
            case Spec.Revenant:
                res |= RevenantHelper.IsKnownMinionID(id);
                res |= RenegadeHelper.IsKnownMinionID(id);
                break;
            //
            case Spec.Guardian:
                res |= GuardianHelper.IsKnownMinionID(id);
                break;
        }
        return res;
    }
    public static void Add<TKey, TValue>(Dictionary<TKey, List<TValue>> dict, TKey key, TValue evt)
    {
        if (dict.TryGetValue(key, out List<TValue>? list))
        {
            list.Add(evt);
        }
        else
        {
            dict[key] =
            [
                evt
            ];
        }
    }
    public static void Add<TKey, TValue>(Dictionary<TKey, HashSet<TValue>> dict, TKey key, TValue evt)
    {
        if (dict.TryGetValue(key, out HashSet<TValue>? list))
        {
            list.Add(evt);
        }
        else
        {
            dict[key] =
            [
                evt
            ];
        }
    }

    public static int IndexOf<T>(this IReadOnlyList<T> self, T elementToFind)
    {
        int i = 0;
        foreach (T element in self)
        {
            if (Equals(element, elementToFind))
            {
                return i;
            }

            i++;
        }
        return -1;
    }
}
