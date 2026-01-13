using GW2EIEvtcParser.EIData.BuffSimulators;
using GW2EIEvtcParser.EIData.BuffSourceFinders;
using GW2EIEvtcParser.Interfaces;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

public class Buff : IVersionable
{

    // Boon
    public enum BuffClassification
    {
        Condition,
        Boon,
        Offensive,
        Defensive,
        Support,
        Debuff,
        Gear,
        Other,
        Enhancement,
        Nourishment,
        OtherConsumable,
        Hidden,
        Unknown
    };

    public enum BuffType
    {
        Duration = 0,
        Intensity = 1,
        Unknown = 2,
    }

    // Fields
    public readonly string Name;
    public readonly long ID;
    public readonly BuffClassification Classification;
    public readonly IReadOnlyCollection<Source> Sources;
    public readonly BuffStackType StackType;
    public BuffType Type => StackType switch
    {
        BuffStackType.Queue or
        BuffStackType.Regeneration or
        BuffStackType.Force
            => BuffType.Duration,
        BuffStackType.Stacking or
        BuffStackType.StackingUniquePerSrc or
        BuffStackType.StackingConditionalLoss
            => BuffType.Intensity,
        _ => BuffType.Unknown,
    };

    private ulong _maxBuild = GW2Builds.EndOfLife;
    private ulong _minBuild = GW2Builds.StartOfLife;
    private int _maxEvtcBuild = ArcDPSBuilds.EndOfLife;
    private int _minEvtcBuild = ArcDPSBuilds.StartOfLife;
    public readonly int Capacity;
    public readonly string Link;

    /// <summary>
    /// Buff constructor
    /// </summary>
    /// <param name="name">The name of the boon</param>
    /// <param name="id">The id of the buff</param>
    /// <param name="type">Stack Type of the buff<see cref="BuffStackType"/></param>
    /// <param name="capacity">Maximun amount of buff in stack</param>
    /// <param name="nature">Nature of the buff, dictates in which category the buff will appear <see cref="BuffClassification"/></param>
    /// <param name="link">URL to the icon of the buff</param>
    private Buff(string name, long id, BuffStackType type, int capacity, BuffClassification nature, string link)
    {
        Name = name;
        ID = id;
        StackType = type;
        Capacity = capacity;
        Classification = nature;
#if DEBUG
        if (Classification == BuffClassification.Hidden)
        {
            Classification = BuffClassification.Other;
        }
#endif
        Link = link;
    }

    /// <summary>
    /// Buff constructor
    /// </summary>
    /// <param name="name">The name of the boon</param>
    /// <param name="id">The id of the buff</param>
    /// <param name="source">Source of the buff <see cref="ParserHelper.Source"/></param>
    /// <param name="type">Stack Type of the buff<see cref="BuffStackType"/></param>
    /// <param name="capacity">Maximun amount of buff in stack</param>
    /// <param name="nature">Nature of the buff, dictates in which category the buff will appear <see cref="BuffClassification"/></param>
    /// <param name="link">URL to the icon of the buff</param>
    internal Buff(string name, long id, Source source, BuffStackType type, int capacity, BuffClassification nature, string link) : this(name, id, type, capacity, nature, link)
    {
        Sources = new HashSet<Source> { source };
    }
    /// <summary>
    /// Buff constructor
    /// </summary>
    /// <param name="name">The name of the boon</param>
    /// <param name="id">The id of the buff</param>
    /// <param name="sources">List of sources of the buff <see cref="ParserHelper.Source"/></param>
    /// <param name="type">Stack Type of the buff<see cref="BuffStackType"/></param>
    /// <param name="capacity">Maximun amount of buff in stack</param>
    /// <param name="nature">Nature of the buff, dictates in which category the buff will appear <see cref="BuffClassification"/></param>
    /// <param name="link">URL to the icon of the buff</param>
    internal Buff(string name, long id, HashSet<Source> sources, BuffStackType type, int capacity, BuffClassification nature, string link) : this(name, id, type, capacity, nature, link)
    {
        Sources = sources;
    }

    internal Buff(string name, long id, Source source, BuffClassification nature, string link) : this(name, id, source, BuffStackType.Force, 1, nature, link)
    {
    }
    internal Buff(string name, long id, HashSet<Source> sources, BuffClassification nature, string link) : this(name, id, sources, BuffStackType.Force, 1, nature, link)
    {
    }

    internal Buff WithBuilds(ulong minBuild, ulong maxBuild = GW2Builds.EndOfLife)
    {
        _minBuild = minBuild;
        _maxBuild = maxBuild;
        return this;
    }

    internal Buff WithEvtcBuilds(int minBuild, int maxBuild = ArcDPSBuilds.EndOfLife)
    {
        _minEvtcBuild = minBuild;
        _maxEvtcBuild = maxBuild;
        return this;
    }

    public Buff(string name, long id, string link)
    {
        Name = name;
        ID = id;
        Sources = [Source.Unknown];
        StackType = BuffStackType.Unknown;
        Capacity = 1;
        Classification = BuffClassification.Unknown;
        Link = link;
    }

    internal static Buff CreateCustomBuff(string name, long id, string link, int capacity, BuffClassification classification)
    {
        return new Buff(name + " " + id, id, Source.Item, capacity > 1 ? BuffStackType.Stacking : BuffStackType.Force, capacity, classification, link);
    }

    private bool IsIncompatibleStackLogic(BuffStackType expectedStackType)
    {
        switch (StackType)
        {
            // Simulation logic does not change
            case BuffStackType.StackingConditionalLoss:
            case BuffStackType.Stacking:
            case BuffStackType.StackingUniquePerSrc:
                return !(expectedStackType == BuffStackType.StackingConditionalLoss || expectedStackType == BuffStackType.Stacking || expectedStackType == BuffStackType.StackingUniquePerSrc);
            default:
                break;
        }
        return expectedStackType != StackType;
    }

    internal void VerifyBuffInfoEvent(BuffInfoEvent buffInfoEvent, EvtcVersionEvent versionEvent, ParserController operation)
    {
        if (buffInfoEvent.BuffID != ID)
        {
            return;
        }
        if (Capacity != buffInfoEvent.MaxStacks)
        {
            operation.UpdateProgressWithCancellationCheck("Parsing: Adjusted capacity for " + Name + " from " + Capacity + " to " + buffInfoEvent.MaxStacks);
        }
        if (buffInfoEvent.StackingType != StackType && buffInfoEvent.StackingType != BuffStackType.Unknown)
        {
            var message = "Incoherent stack type for " + Name + ": is " + StackType + " but expected " + buffInfoEvent.StackingType + " with stacks " + buffInfoEvent.MaxStacks;
#if DEBUG
            // I don't exactly remember when stack type on buff info event was fixed on arc's side
            if (versionEvent.Build > 20240600 && IsIncompatibleStackLogic(buffInfoEvent.StackingType))
            {
                throw new InvalidDataException(message);
            }
#endif
            operation.UpdateProgressWithCancellationCheck("Parsing: " + message);
        }
    }

    internal AbstractBuffSimulator CreateSimulator(ParsedEvtcLog log, BuffStackItemPool pool, bool forceNoId)
    {
        BuffInfoEvent? buffInfoEvent = log.CombatData.GetBuffInfoEvent(ID);
        int capacity = Capacity;
        if (buffInfoEvent != null && buffInfoEvent.MaxStacks != capacity && buffInfoEvent.MaxStacks > 0)
        {
            capacity = buffInfoEvent.MaxStacks;
        }

        if (!log.CombatData.UseBuffInstanceSimulator || forceNoId)
        {

            return Type switch
            {
                BuffType.Intensity => new BuffSimulatorIntensity(log, this, pool, capacity),
                BuffType.Duration => new BuffSimulatorDuration(log, this, pool, capacity),
                _ => throw new InvalidDataException("Buffs can not be stackless"),
            };
        }

        return Type switch
        {
            BuffType.Intensity => new BuffSimulatorIDIntensity(log, this, pool, capacity),
            BuffType.Duration => new BuffSimulatorIDDuration(log, this, pool, capacity),
            _ => throw new InvalidDataException("Buffs can not be stackless"),
        };
    }

    internal static BuffSourceFinder GetBuffSourceFinder(CombatData combatData, HashSet<long> boonIDs)
    {
        ulong gw2Build = combatData.GetGW2BuildEvent().Build;

        if (gw2Build >= GW2Builds.January2026Balance)
        {
            return new BuffSourceFinder20260113(boonIDs);
        }
        if (gw2Build >= GW2Builds.March2024BalanceAndCerusLegendary)
        {
            return new BuffSourceFinder20240319(boonIDs);
        }
        if (gw2Build >= GW2Builds.October2022BalanceHotFix)
        {
            return new BuffSourceFinder20221018(boonIDs);
        }
        if (gw2Build >= GW2Builds.EODBeta2)
        {
            return new BuffSourceFinder20210921(boonIDs);
        }
        if (gw2Build >= GW2Builds.May2021Balance)
        {
            return new BuffSourceFinder20210511(boonIDs);
        }
        if (gw2Build >= GW2Builds.October2019Balance)
        {
            return new BuffSourceFinder20191001(boonIDs);
        }
        if (gw2Build >= GW2Builds.March2019Balance)
        {
            return new BuffSourceFinder20190305(boonIDs);
        }
        return new BuffSourceFinder20181211(boonIDs);
    }

    public bool Available(CombatData combatData)
    {
        ulong gw2Build = combatData.GetGW2BuildEvent().Build;
        if (gw2Build < _maxBuild && gw2Build >= _minBuild)
        {
            int evtcBuild = combatData.GetEvtcVersionEvent().Build;
            if (evtcBuild < _maxEvtcBuild && evtcBuild >= _minEvtcBuild)
            {
                return true;
            }
        }
        return false;
    }
}
