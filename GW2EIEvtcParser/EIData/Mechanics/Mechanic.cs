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

public abstract class Mechanic : MechanicContainer
{
    [Flags]
    public enum MechanicSeverity
    {
        P0 = 0,
        P1 = 1,
        P2 = 2,
        P3 = 3,

        SeverityMask = P0 | P1 | P2 | P3,

        Success = 8,
        Failure = 16,
        Neutral = 32,

        P0Success = P0 | Success,
        P0Failure = P0 | Failure,
        P0Neutral = P0 | Neutral,

        P1Success = P1 | Success,
        P1Failure = P1 | Failure,
        P1Neutral = P1 | Neutral,

        P2Success = P2 | Success,
        P2Failure = P2 | Failure,
        P2Neutral = P2 | Neutral,

        P3Success = P3 | Success,
        P3Failure = P3 | Failure,
        P3Neutral = P3 | Neutral,
    }

    public readonly int InternalCooldown;
    public readonly MechanicPlotlySetting PlotlySetting;
    public readonly string Description;
    public readonly string ShortName;
    public readonly string FullName;

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
    /// <param name="inGameName">official name of the mechanic</param>
    /// <param name="plotlySetting">html plot settings <seealso cref="MechanicPlotlySetting"/></param>
    /// <param name="shortName">shortened name of the mechanic</param>
    /// <param name="description">description of the mechanic</param>
    /// <param name="fullName">full name of the mechanic</param>
    /// <param name="internalCoolDown">grace period, in ms, during which getting hit by the mechanic does not count</param>
    protected Mechanic(MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity, int internalCoolDown)
    {
        PlotlySetting = plotlySetting;
        ShortName = shortName;
        FullName = fullName;
        Description = description;
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
