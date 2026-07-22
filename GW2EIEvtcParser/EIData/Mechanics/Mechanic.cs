using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EIData;

/// <summary>
/// Plot description of the mechanic
/// </summary>
public class MechanicPlotlySetting
{
    public readonly string Color;
    public readonly int Size = 15;
    public readonly string Symbol;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="symbol">Symbol to use, see https://plot.ly/javascript/reference/#scatter-marker-symbol </param>
    /// <param name="color">The color of the symbol</param>
    internal MechanicPlotlySetting(Symbol symbol, Color color)
    {
        Color = color.ToString(false);
        Symbol = symbol.Str;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="symbol">Symbol to use, see https://plot.ly/javascript/reference/#scatter-marker-symbol </param>
    /// <param name="color">The color of the symbol</param>
    /// <param name="size">Size, in pixel, of the symbol, defaults to 15</param>
    internal MechanicPlotlySetting(Symbol symbol, Color color, int size)
    {
        Color = color.ToString(false);
        Symbol = symbol.Str;
        Size = size;
    }

}

public class MechanicDescription
{
    public readonly string Description;
    public readonly string ShortName;
    public readonly string FullName;

    internal MechanicDescription(string shortName, string description, string fullName)
    {
        Description = description;
        ShortName = shortName;
        FullName = fullName;
    }
    internal MechanicDescription(string shortName, string fullName) : this(shortName, fullName, fullName)
    {
    }
    internal MechanicDescription(string fullName) : this(fullName, fullName, fullName)
    {
    }
}

public abstract class Mechanic : MechanicContainer
{
    [Flags]
    public enum MechanicSeverity
    {
        Sev0 = 0,
        Sev1 = 1,
        Sev2 = 2,
        Sev3 = 3,
        Sev4 = 4,

        SeverityMask = Sev0 | Sev1 | Sev2 | Sev3 | Sev4,

        Success = 8,
        Failure = 16,
        Neutral = 32,

        Sev0Success = Sev0 | Success,
        Sev0Failure = Sev0 | Failure,
        Sev0Neutral = Sev0 | Neutral,

        Sev1Success = Sev1 | Success,
        Sev1Failure = Sev1 | Failure,
        Sev1Neutral = Sev1 | Neutral,

        Sev2Success = Sev2 | Success,
        Sev2Failure = Sev2 | Failure,
        Sev2Neutral = Sev2 | Neutral,

        Sev3Success = Sev3 | Success,
        Sev3Failure = Sev3 | Failure,
        Sev3Neutral = Sev3 | Neutral,
    }

    public readonly int ID;

    public readonly int InternalCooldown;
    public readonly MechanicPlotlySetting PlotlySetting;
    private readonly MechanicDescription _MechDescription;
    public string Description => _MechDescription.Description;
    public string FullName => _MechDescription.FullName;
    public string ShortName => _MechDescription.ShortName;

    public readonly MechanicSeverity Severity;

    public bool IsEnemyMechanic { get; protected set; }
    public bool ShowOnTable { get; private set; }
    public bool Ignored { get; private set; }

    public delegate bool Keeper(ParsedEvtcLog log);
    private readonly List<Keeper> _enableConditions;
    private ulong _maxBuild = GW2Builds.EndOfLife;
    private ulong _minBuild = GW2Builds.StartOfLife;
    private int _maxEvtcBuild = ArcDPSBuilds.EndOfLife;
    private int _minEvtcBuild = ArcDPSBuilds.StartOfLife;

    /// <summary>
    /// Full constructor without special checks
    /// </summary>
    /// <param name="id">unique id of the mechanic</param>
    /// <param name="plotlySetting">html plot settings <seealso cref="MechanicPlotlySetting"/></param>
    /// <param name="description">description of the mechanic</param>
    /// <param name="severity">severity category of the mechanic</param>
    /// <param name="internalCoolDown">grace period, in ms, during which getting hit by the mechanic does not count</param>
    protected Mechanic(int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown = 0)
    {
        ID = id;
        PlotlySetting = plotlySetting;
        _MechDescription = description;
        InternalCooldown = internalCoolDown;
        ShowOnTable = true;
        Severity = severity;
        _enableConditions = [];
    }

    internal abstract void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs);

    internal Mechanic UsingNoShowOnTable()
    {
        ShowOnTable = false;
        return this;
    }

    internal Mechanic UsingIgnored()
    {
        Ignored = true;
        return this;
    }

    internal Mechanic UsingEnable(Keeper keeper)
    {
        _enableConditions.Add(keeper);
        return this;
    }

    internal Mechanic UsingDisableWithEffectData()
    {
        return UsingEnable(log => !log.CombatData.HasEffectData);
    }

    internal Mechanic UsingDisableWithMissileData()
    {
        return UsingEnable(log => !log.CombatData.HasMissileData);
    }

    internal Mechanic UsingDisableWithCrowControlData()
    {
        return UsingEnable(log => !log.CombatData.HasCrowdControlData);
    }

    internal Mechanic WithBuilds(ulong minBuild, ulong maxBuild = GW2Builds.EndOfLife)
    {
        _maxBuild = maxBuild;
        _minBuild = minBuild;
        return this;
    }
    internal Mechanic WithEvtcBuilds(int minBuild, int maxBuild = ArcDPSBuilds.EndOfLife)
    {
        _minEvtcBuild = minBuild;
        _maxEvtcBuild = maxBuild;
        return this;
    }

    internal bool Available(ParsedEvtcLog log)
    {
        if (!_enableConditions.All(checker => checker(log)))
        {
            return false;
        }
        ulong gw2Build = log.CombatData.GetGW2BuildEvent().Build;
        if (gw2Build < _maxBuild && gw2Build >= _minBuild)
        {
            int evtcBuild = log.CombatData.GetEvtcVersionEvent().Build;
            if (evtcBuild < _maxEvtcBuild && evtcBuild >= _minEvtcBuild)
            {
                return true;
            }
        }
        return false;
    }

    public override IReadOnlyList<Mechanic> GetMechanics()
    {
        return [this];
    }
}
