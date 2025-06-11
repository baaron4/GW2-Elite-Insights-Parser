using System.Text.Json.Serialization;

[assembly: CLSCompliant(false)]
namespace GW2EIJSON;


// compile-time generated serialization logic
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata | JsonSourceGenerationMode.Serialization,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    IncludeFields = true, WriteIndented = false, NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
    Converters = [
        typeof(Tuple2ToExceptionConverterFactory),
    ]
)]
[JsonSerializable(typeof(JsonLog))]
public partial class JsonLogSerializerContext : JsonSerializerContext {  }

/// <summary>
/// The root of the JSON.
/// </summary>
/// <remarks>
/// Use <see cref="JsonLogSerializerContext"/> or manually use an <see cref="Tuple2ToExceptionConverterFactory"/> instance with your (de)serializer.
/// </remarks>
public class JsonLog
{
    /// <summary>
    /// Describes the skill item
    /// </summary>
    public class SkillDesc
    {
        /// <summary>
        /// Name of the skill
        /// </summary>
        public string? Name;

        /// <summary>
        /// If the skill is an auto attack
        /// </summary>
        public bool AutoAttack;

        /// <summary>
        /// If the skill can crit
        /// </summary>
        public bool CanCrit;

        /// <summary>
        /// Icon of the skill
        /// </summary>
        public string? Icon;

        /// <summary>
        /// True if this skill can trigger on weapon swap sigils
        /// </summary>
        public bool IsSwap;
        /// <summary>
        /// True when the skill is an instant cast
        /// </summary>
        public bool IsInstantCast;
        /// <summary>
        /// True when the skill represents a trait proc.\
        /// <see cref="IsInstantCast"/> is necessarily true.
        /// </summary>
        public bool IsTraitProc;
        /// <summary>
        /// True when the skill represents a trait proc.\
        /// <see cref="IsInstantCast"/> is necessarily true.
        /// </summary>
        public bool IsGearProc;
        /// <summary>
        /// True when the skill is an instant cast and the detection may have missed some
        /// </summary>
        public bool IsNotAccurate;
        /// <summary>
        /// If the skill is encountered in a healing context, true if healing happened because of conversion, false otherwise
        /// </summary>
        public bool ConversionBasedHealing;
        /// <summary>
        /// If the skill is encountered in a healing context, true if healing could have happened due to conversion or healing power
        /// </summary>
        public bool HybridHealing;
    }

    /// <summary>
    /// Describs the buff item
    /// </summary>
    public class BuffDesc
    {
        /// <summary>
        /// Name of the buff
        /// </summary>
        public string? Name;

        /// <summary>
        /// Icon of the buff
        /// </summary>
        public string? Icon;

        /// <summary>
        /// True if the buff is stacking
        /// </summary>
        public bool Stacking;

        /// <summary>
        /// If the buff is encountered in a healing context, true if healing happened because of conversion, false otherwise
        /// </summary>
        public bool ConversionBasedHealing;
        /// <summary>
        /// If the buff is encountered in a healing context, true if healing could have happened due to conversion or healing power
        /// </summary>
        public bool HybridHealing;

        /// <summary>
        /// Descriptions of the buffs (no traits)
        /// </summary>
        public IReadOnlyList<string>? Descriptions;
    }

    /// <summary>
    /// Describes an extension
    /// </summary>
    public class ExtensionDesc
    {
        /// <summary>
        /// Name of the extension
        /// </summary>
        public string? Name;

        /// <summary>
        /// Version of the extension, "Unknown" if missing
        /// </summary>
        public string? Version;

        /// <summary>
        /// Revision of the extension
        /// </summary>
        public uint Revision;

        /// <summary>
        /// Signature of the extension
        /// </summary>
        public uint Signature;

        /// <summary>
        /// List of <see cref="JsonActor.Name"/> running the extension.
        /// </summary>
        public IReadOnlyList<string>? RunningExtension;
    }

    /// <summary>
    /// Describs the damage modifier item
    /// </summary>
    public class DamageModDesc
    {
        /// <summary>
        /// Name of the damage modifier
        /// </summary>
        public string? Name;

        /// <summary>
        /// Icon of the damage modifier
        /// </summary>
        public string? Icon;

        /// <summary>
        /// Description of the damage modifier
        /// </summary>
        public string? Description;

        /// <summary>
        /// False if the modifier is multiplicative \n
        /// If true then the correspond <see cref="JsonDamageModifierData.JsonDamageModifierItem.DamageGain"/> are damage done under the effect. One will have to deduce the gain manualy depending on your gear.
        /// </summary>
        public bool NonMultiplier;

        /// <summary>
        /// True if the modifier is skill based
        /// </summary>
        public bool SkillBased;
        /// <summary>
        /// True if the modifier is an approximation
        /// </summary>
        public bool Approximate;
        /// <summary>
        /// True if the modifier is an incoming damage modifier
        /// </summary>
        public bool Incoming;
    }

    /// <summary>
    /// The used EI version.
    /// </summary>
    public string? EliteInsightsVersion;

    /// <summary>
    /// The id with which the log has been triggered
    /// </summary>
    public int TriggerID;
    /// <summary>
    /// The elite insight id of the log, indicates which encounter the log corresponds to. \n
    /// see https://github.com/baaron4/GW2-Elite-Insights-Parser/blob/master/EncounterIDs.md/
    /// </summary>
    public long EIEncounterID;

    /// <summary>
    /// The name of the fight
    /// </summary>
    public string? FightName;

    /// <summary>
    /// The icon of the fight
    /// </summary>
    public string? FightIcon;

    /// <summary>
    /// The used arcdps version
    /// </summary>
    public string? ArcVersion;

    /// <summary>
    /// The used arcdps revision
    /// </summary>
    public int ArcRevision;

    /// <summary>
    /// GW2 build
    /// </summary>
    public ulong GW2Build;

    /// <summary>
    /// Language with which the evtc was generated
    /// </summary>
    public string? Language;

    /// <summary>
    /// Scale of the fractal, only applicable for fractal encounters. \n
    /// Valued at 0 if missing.
    /// </summary>
    public int FractalScale;

    /// <summary>
    /// ID of the language
    /// </summary>
    public byte LanguageID;

    /// <summary>
    /// The player who recorded the fight
    /// </summary>
    public string? RecordedBy;

    /// <summary>
    /// The account name of the player who recorded the fight
    /// </summary>
    public string? RecordedAccountBy;

    /// <summary>
    /// DEPRECATED: use TimeStartStd instead \n
    /// The time at which the fight started in "yyyy-mm-dd hh:mm:ss zz" format \n
    /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
    /// </summary>
    public string? TimeStart;

    /// <summary>
    /// DEPRECATED: use TimeEndStd instead \n
    /// The time at which the fight ended in "yyyy-mm-dd hh:mm:ss zz" format \n
    /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
    /// </summary>
    public string? TimeEnd;


    /// <summary>
    /// The time at which the fight started in "yyyy-mm-dd hh:mm:ss zzz" format \n
    /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
    /// </summary>
    public string? TimeStartStd;

    /// <summary>
    /// The time at which the fight ended in "yyyy-mm-dd hh:mm:ss zzz" format \n
    /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
    /// </summary>
    public string? TimeEndStd;

    /// <summary>
    /// The duration of the fight in "xh xm xs xms" format
    /// </summary>
    public string? Duration;

    /// <summary>
    /// The duration of the fight in ms
    /// </summary>
    public long DurationMS;

    /// <summary>
    /// Offset between fight start and log start
    /// </summary>
    public long LogStartOffset;
    /// <summary>
    /// The time at which the instance started in "yyyy-mm-dd hh:mm:ss zzz" format \n
    /// The value will be null if the event does not exist
    /// </summary>
    public string? InstanceTimeStartStd;


    /// <summary>
    /// XXX.XXX.XXX.XXX IP address of the instance \n
    /// The value will be null if the event does not exist
    /// </summary>
    public string? InstanceIP;

    /// <summary>
    /// Type of instance privacy \n
    /// Possible values are "Unknown", "Not Applicable", "Public Instance" and "Private Instance"
    /// </summary>
    public string InstancePrivacy;

    /// <summary>
    /// The success status of the fight
    /// </summary>
    public bool Success;

    /// <summary>
    /// If the fight is in challenge mode
    /// </summary>
    public bool IsCM;
    /// <summary>
    /// If the fight is in legendary challenge mode. \n
    /// If this is true, <see cref="IsCM"/> will also be true
    /// </summary>
    public bool IsLegendaryCM;
    /// <summary>
    /// True if EI detected that the encounter started later than expected. \n
    /// This value being false does not mean the encounter could not have started later than expected.
    /// </summary>
    public bool IsLateStart;
    /// <summary>
    /// True if an encounter that is supposed to have a pre-event does not have it.
    /// </summary>
    public bool MissingPreEvent;

    /// <summary>
    /// If the log was parsed in anonymous mode
    /// </summary>
    public bool Anonymous;


    /// <summary>
    /// If the log was parsed in detailed mode. \n
    /// Only for WvW logs
    /// </summary>
    public bool DetailedWvW;

    /// <summary>
    /// The list of targets
    /// </summary>
    /// <seealso cref="JsonNPC"/>
    public IReadOnlyList<JsonNPC>? Targets;

    /// <summary>
    /// The list of players
    /// </summary>
    /// <seealso cref="JsonPlayer"/>
    public IReadOnlyList<JsonPlayer>? Players;

    /// <summary>
    /// The list of phases
    /// </summary>
    /// <seealso cref="JsonPhase"/>
    public IReadOnlyList<JsonPhase>? Phases;

    /// <summary>
    /// List of mechanics
    /// </summary>
    /// <seealso cref="JsonMechanics"/>
    public IReadOnlyList<JsonMechanics>? Mechanics;

    /// <summary>
    /// Upload links to dps.reports/raidar
    /// </summary>
    public IReadOnlyList<string>? UploadLinks;

    /// <summary>
    /// Dictionary of skills' description, the key is in "'s' + id" format
    /// </summary>
    /// <seealso cref="SkillDesc"/>
    public IReadOnlyDictionary<string, SkillDesc>? SkillMap;

    /// <summary>
    /// Dictionary of buffs' description, the key is in "'b' + id" format
    /// </summary>
    /// <seealso cref="BuffDesc"/>
    public IReadOnlyDictionary<string, BuffDesc>? BuffMap;

    /// <summary>
    /// Dictionary of damage modifiers' description, the key is in "'d' + id" format
    /// </summary>
    /// <seealso cref="DamageModDesc"/>
    public IReadOnlyDictionary<string, DamageModDesc>? DamageModMap;

    /// <summary>
    /// Dictionary of personal buffs. The key is the profession, the value is a list of buff ids
    /// </summary>
    /// <seealso cref="BuffMap"/>
    public IReadOnlyDictionary<string, IReadOnlyCollection<long>>? PersonalBuffs;

    /// <summary>
    /// Dictionary of damage modifiers. The key is the profession, the value is a list of damage mod ids
    /// </summary>
    /// <seealso cref="DamageModMap"/>
    public IReadOnlyDictionary<string, IReadOnlyCollection<long>>? PersonalDamageMods;

    /// <summary>
    /// List of present fractal instabilities, the values are buff ids. DEPRECATED: use PresentInstanceBuffs instead
    /// </summary>
    /// <seealso cref="BuffMap"/>
    public IReadOnlyList<long>? PresentFractalInstabilities;
    /// <summary>
    /// List of present instance buffs, values are arrays of 2 elements, value[0] is buff id, value[1] is number of stacks.
    /// </summary>
    /// <seealso cref="BuffMap"/>
    public IReadOnlyList<IReadOnlyList<long>>? PresentInstanceBuffs;

    /// <summary>
    /// List of error messages given by ArcDPS
    /// </summary>
    public IReadOnlyList<string>? LogErrors;

    /// <summary>
    /// List of used extensions
    /// </summary>
    public IReadOnlyList<ExtensionDesc>? UsedExtensions;
    /// <summary>
    /// Contains combat replay related meta data
    /// </summary>
    /// <seealso cref="JsonCombatReplayMetaData"/>
    public JsonCombatReplayMetaData? CombatReplayMetaData;
}
