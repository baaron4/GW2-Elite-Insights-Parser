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

public abstract class Mechanic
{
    public readonly int InternalCooldown;
    public readonly MechanicPlotlySetting PlotlySetting;
    public readonly string Description;
    private readonly string _inGameName;
    public readonly string ShortName;
    public readonly string FullName;
    public bool IsEnemyMechanic { get; protected set; }
    public bool ShowOnTable { get; private set; }

    public bool IsAchievementEligibility { get; private set; }

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
    protected Mechanic(string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown)
    {
        _inGameName = inGameName;
        PlotlySetting = plotlySetting;
        ShortName = shortName;
        FullName = fullName;
        Description = description;
        InternalCooldown = internalCoolDown;
        ShowOnTable = true;
        _enableConditions = [];
    }

    internal abstract void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs);

    internal Mechanic UsingShowOnTable(bool showOnTable)
    {
        ShowOnTable = showOnTable;
        return this;
    }

    private static bool EligibilityKeeper(ParsedEvtcLog log)
    {
        return log.FightData.Success;
    }

    /// <summary>
    /// Eligibility mechanics will only be computed in successful logs
    /// </summary>
    /// <param name="isAchievementEligibility"></param>
    /// <returns></returns>
    internal Mechanic UsingAchievementEligibility(bool isAchievementEligibility)
    {
        IsAchievementEligibility = isAchievementEligibility;
        if (isAchievementEligibility)
        {
            _enableConditions.Add(EligibilityKeeper);
        }
        else
        {
            _enableConditions.Remove(EligibilityKeeper);
        }
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

    internal bool KeepIfEmpty(ParsedEvtcLog log)
    {
        return IsAchievementEligibility && log.FightData.Success;
    }

}
