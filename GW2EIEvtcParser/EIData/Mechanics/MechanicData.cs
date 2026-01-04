using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class MechanicData
{
    private readonly Dictionary<Mechanic, List<MechanicEvent>> _mechanicLogs;

    private CachingCollection<HashSet<Mechanic>>? _presentOnFriendliesMechanics;
    private CachingCollection<HashSet<Mechanic>>? _presentOnEnemyMechanics;
    private CachingCollection<HashSet<Mechanic>>? _presentMechanics;
    private CachingCollection<List<SingleActor>>? _enemyList;

    internal MechanicData(List<Mechanic> logMechanics, bool isIntanceLog)
    {
        _mechanicLogs = new(logMechanics.Count);
        Tracing.Trace.TrackAverageStat("logMechanics", logMechanics.Count);
        foreach (Mechanic m in logMechanics.OrderBy(x => !x.IsAchievementEligibility && x is not AchievementEligibilityMechanic))
        {
            // Ignore achievement eligibility mechanics in instances for now
            if (isIntanceLog && m.IsAchievementEligibility)
            {
                continue;
            }
            _mechanicLogs.Add(m, []);
        }

    }

    private void CheckConfiguration(ParsedEvtcLog log)
    {
        var logMechanics = _mechanicLogs;
        //TODO_PERF(Rennorb) @find average complexity
        var errorMechanicConfig = new Dictionary<string, Dictionary<string, Dictionary<int, List<Mechanic>>>>(logMechanics.Count / 2);
        var errorMechanicNaming = new Dictionary<string, Dictionary<string, Dictionary<string, List<Mechanic>>>>(logMechanics.Count);
        foreach (Mechanic m in logMechanics.Keys)
        {
            if (!log.LogData.AllowDuplicateMechanicPlotlyConfigs)
            {
                if (!errorMechanicConfig.TryGetValue(m.PlotlySetting.Symbol, out var colorDict))
                {
                    //TODO_PERF(Rennorb)
                    colorDict = [];
                    errorMechanicConfig[m.PlotlySetting.Symbol] = colorDict;
                }
                if (!colorDict.TryGetValue(m.PlotlySetting.Color, out var sizeDict))
                {
                    //TODO_PERF(Rennorb)
                    sizeDict = [];
                    colorDict[m.PlotlySetting.Color] = sizeDict;
                }
                if (!sizeDict.TryGetValue(m.PlotlySetting.Size, out var mList))
                {
                    //TODO_PERF(Rennorb)
                    mList = [];
                    sizeDict[m.PlotlySetting.Size] = mList;
                }
                mList.Add(m);
                if (mList.Count > 1)
                {
                    throw new InvalidDataException(mList[0].FullName + " and " + mList[1].FullName + " share the same plotly configuration");
                }
            }
            {
                if (!errorMechanicNaming.TryGetValue(m.FullName, out var shortNameDict))
                {
                    //TODO_PERF(Rennorb)
                    shortNameDict = [];
                    errorMechanicNaming[m.FullName] = shortNameDict;
                }
                if (!shortNameDict.TryGetValue(m.ShortName, out var descriptionDict))
                {
                    //TODO_PERF(Rennorb)
                    descriptionDict = [];
                    shortNameDict[m.ShortName] = descriptionDict;
                }
                if (!descriptionDict.TryGetValue(m.Description, out var mList))
                {
                    //TODO_PERF(Rennorb)
                    mList = [];
                    descriptionDict[m.Description] = mList;
                }
                mList.Add(m);
                if (mList.Count > 1)
                {
                    throw new InvalidDataException(mList[0].FullName + " and " + mList[1].FullName + " share the same naming configuration");
                }
            }
        }

        Tracing.Trace.TrackAverageStat("errorMechanicConfig", errorMechanicConfig.Count);
        Tracing.Trace.TrackAverageStat("errorMechanicNaming", errorMechanicNaming.Count);
    }

    private void ComputeMechanics(ParsedEvtcLog log)
    {
        //TODO_PERF(Rennorb) <regroupedMobs> = 0
        var regroupedMobs = new Dictionary<int, SingleActor>();
        foreach(var x in _mechanicLogs.Keys.Where(x => !x.Available(log))) 
        { 
            _mechanicLogs.Remove(x); 
        }
        CheckConfiguration(log);
        foreach (Mechanic mech in _mechanicLogs.Keys)
        {
            if (mech.IsASubMechanic)
            {
                continue;
            }
            mech.CheckMechanic(log, _mechanicLogs, regroupedMobs);
        }
        Tracing.Trace.TrackAverageStat("_mechanicLogs", _mechanicLogs.Count);
        Tracing.Trace.TrackAverageStat("regroupedMobs", regroupedMobs.Count);
    }

    [MemberNotNull(nameof(_presentOnFriendliesMechanics))]
    [MemberNotNull(nameof(_presentOnEnemyMechanics))]
    [MemberNotNull(nameof(_presentMechanics))]
    [MemberNotNull(nameof(_enemyList))]
    private void ProcessMechanics(ParsedEvtcLog log)
    {
        if (_presentMechanics != null)
        {
            #nullable disable
            return;
            #nullable restore
        }

        _presentOnFriendliesMechanics = new(log);
        _presentOnEnemyMechanics      = new(log);
        _presentMechanics             = new(log);
        _enemyList                    = new(log);
        ComputeMechanics(log);
        foreach (var (mechanic, events) in _mechanicLogs)
        {
            if(events.Count != 0)
            {
                events.SortByTime();
            }
            else if(!mechanic.KeepIfEmpty(log))
            {
                //NOTE(Rennorb: Removing from dicts is allowed during iteration.
                _mechanicLogs.Remove(mechanic);
            }
        }
    }

    /// <summary>
    /// DEPRECATED, CSV Usage only
    /// </summary>
    /// <param name="log"></param>
    /// <returns></returns>
    public Dictionary<Mechanic, List<MechanicEvent>>.ValueCollection GetAllMechanicEvents(ParsedEvtcLog log)
    {
        ProcessMechanics(log);
        return _mechanicLogs.Values;
    }

    public IReadOnlyList<MechanicEvent> GetMechanicLogs(ParsedEvtcLog log, Mechanic mech, long start, long end)
    {
        ProcessMechanics(log);
        return _mechanicLogs.TryGetValue(mech, out var list) ? list.Where(x => x.Time >= start && x.Time <= end).ToList() : [];
    }

    public IReadOnlyList<MechanicEvent> GetMechanicLogs(ParsedEvtcLog log, Mechanic mech, SingleActor actor, long start, long end)
    {
        return GetMechanicLogs(log, mech, start, end).Where(x => x.Actor == actor).ToList();
    }

    private void ComputeMechanicData(ParsedEvtcLog log, long start, long end)
    {
        var presentMechanics = new HashSet<Mechanic>();
        var presentOnEnemyMechanics = new HashSet<Mechanic>();
        var presentOnFriendliesMechanics = new HashSet<Mechanic>();
        var enemyHash = new HashSet<SingleActor>();
        foreach (KeyValuePair<Mechanic, List<MechanicEvent>> pair in _mechanicLogs)
        {
            if (pair.Key.KeepIfEmpty(log) || pair.Value.Any(x => x.Time >= start && x.Time <= end))
            {
                presentMechanics.Add(pair.Key);
                if (pair.Key.ShowOnTable)
                {
                    if (pair.Key.IsEnemyMechanic)
                    {
                        presentOnEnemyMechanics.Add(pair.Key);
                    }
                    else
                    {
                        presentOnFriendliesMechanics.Add(pair.Key);
                    }
                }

            }
        }
        // ready enemy list
        foreach (Mechanic m in _mechanicLogs.Keys.Where(x => x.IsEnemyMechanic))
        {
            foreach (MechanicEvent mechanicEvent in _mechanicLogs[m].Where(x => x.Time >= start && x.Time <= end))
            {
                enemyHash.Add(mechanicEvent.Actor);
            }
        }
        _presentMechanics!.Set(start, end, presentMechanics);
        _presentOnEnemyMechanics!.Set(start, end, presentOnEnemyMechanics);
        _presentOnFriendliesMechanics!.Set(start, end, presentOnFriendliesMechanics);
        _enemyList!.Set(start, end, [..enemyHash]);
    }

    public IReadOnlyCollection<Mechanic> GetPresentEnemyMechs(ParsedEvtcLog log, long start, long end)
    {
        ProcessMechanics(log);
        if (!_presentOnEnemyMechanics.HasKeys(start, end))
        {
            ComputeMechanicData(log, start, end);
        }
        return _presentOnEnemyMechanics.Get(start, end)!;
    }
    public IReadOnlyCollection<Mechanic> GetPresentFriendlyMechs(ParsedEvtcLog log, long start, long end)
    {
        ProcessMechanics(log);
        if (!_presentOnFriendliesMechanics.HasKeys(start, end))
        {
            ComputeMechanicData(log, start, end);
        }
        return _presentOnFriendliesMechanics.Get(start, end)!;
    }
    public IReadOnlyCollection<Mechanic> GetPresentMechanics(ParsedEvtcLog log, long start, long end)
    {
        ProcessMechanics(log);
        if (!_presentMechanics.HasKeys(start, end))
        {
            ComputeMechanicData(log, start, end);
        }
        return _presentMechanics.Get(start, end)!;
    }

    public IReadOnlyList<SingleActor> GetEnemyList(ParsedEvtcLog log, long start, long end)
    {
        ProcessMechanics(log);
        if (!_enemyList.HasKeys(start, end))
        {
            ComputeMechanicData(log, start, end);
        }
        return _enemyList.Get(start, end)!;
    }
}
