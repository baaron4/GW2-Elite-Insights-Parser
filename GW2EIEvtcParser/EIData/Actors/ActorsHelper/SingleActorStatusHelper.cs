using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParsedData.AgentItem;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

partial class SingleActor
{

    private List<Segment>? _deads;
    private List<Segment>? _downs;
    private List<Segment>? _dcs;
    private List<Segment>? _actives;


    private CachingCollection<long>? _activeDurations;

    private List<DeathRecap>? _deathRecaps;

    private List<Segment>? _breakbarNones;
    private List<Segment>? _breakbarActives;
    private List<Segment>? _breakbarImmunes;
    private List<Segment>? _breakbarRecoverings;
    //weaponslist
    private List<WeaponSet>? _weaponSets;

    private static void AddSegment(List<Segment> segments, long start, long end)
    {
        if (start < end)
        {
            segments.Add(new Segment(start, end, 1));
        }
    }

    private static void AddValueToStatusList(List<Segment> dead, List<Segment> down, List<Segment> dc, List<Segment> actives, (long Time, StatusEvent evt) cur, (long Time, StatusEvent? evt) next, long minTime, int index)
    {
        long cTime = cur.Time;
        var curEvt = cur.evt;
        if (curEvt is DownEvent)
        {
            if (index == 0)
            {
                AddSegment(actives, minTime, cTime);
            }
            AddSegment(down, cTime, next.Time);
        }
        else if (curEvt is DeadEvent)
        {
            if (index == 0)
            {
                AddSegment(actives, minTime, cTime);
            }
            AddSegment(dead, cTime, next.Time);
        }
        else if (curEvt is DespawnEvent)
        {
            if (index == 0)
            {
                AddSegment(actives, minTime, cTime);
            }
            AddSegment(dc, cTime, next.Time);
        }
        else
        {
            if (index == 0 && cTime - minTime > 50)
            {
                AddSegment(dc, minTime, cTime);
            }
            AddSegment(actives, cTime, next.Time);
        }
    }
    #region STATUS
    private void GetAgentStatus(ParsedEvtcLog log, List<Segment> dead, List<Segment> down, List<Segment> dc, List<Segment> actives, CombatData combatData)
    {
        //TODO(Rennorb) @perf: find average complexity
        var downEvents = combatData.GetDownEvents(AgentItem);
        var aliveEvents = combatData.GetAliveEvents(AgentItem);
        var deadEvents = combatData.GetDeadEvents(AgentItem);
        var spawnEvents = combatData.GetSpawnEvents(AgentItem);
        var despawnEvents = combatData.GetDespawnEvents(AgentItem);

        var status = new List<(long Time, StatusEvent evt)>(
            downEvents.Count +
            aliveEvents.Count +
            deadEvents.Count +
            spawnEvents.Count +
            despawnEvents.Count +
            (AgentItem.IsEnglobedAgent ? 1 : 0) +
            2 * AgentItem.EnglobingAgentItem.Regrouped.Count
        );
        if (AgentItem.IsEnglobedAgent)
        {
            var englobingRegroupedEvents = AgentItem.EnglobingAgentItem.Regrouped.Where(x => x.MergeStart >= AgentItem.EnglobingAgentItem.FirstAware && x.MergeEnd <= AgentItem.EnglobingAgentItem.LastAware).Select(x => (new SpawnEvent(AgentItem, x.MergeStart), new DespawnEvent(AgentItem, x.MergeEnd)));
            List<StatusEvent?> firstEvents = [
                combatData.GetDownEvents(EnglobingAgentItem).LastOrDefault(x => x.Time < FirstAware),
                combatData.GetAliveEvents(EnglobingAgentItem).LastOrDefault(x => x.Time < FirstAware),
                combatData.GetDeadEvents(EnglobingAgentItem).LastOrDefault(x => x.Time < FirstAware),
                combatData.GetSpawnEvents(EnglobingAgentItem).LastOrDefault(x => x.Time < FirstAware),
                combatData.GetDespawnEvents(EnglobingAgentItem).LastOrDefault(x => x.Time < FirstAware),
                englobingRegroupedEvents.Select(x => x.Item1).LastOrDefault(x => x.Time < FirstAware),
                englobingRegroupedEvents.Select(x => x.Item2).LastOrDefault(x => x.Time < FirstAware)
            ];
            var firstEvent = firstEvents.Where(x => x != null).OrderBy(x => x!.Time).LastOrDefault();
            if (firstEvent != null)
            {
                status.Add((FirstAware, firstEvent));
            }
        }
        var regroupedEvents = AgentItem.EnglobingAgentItem.Regrouped.Where(x => x.MergeStart >= FirstAware && x.MergeEnd <= LastAware).Select(x => (new SpawnEvent(AgentItem, x.MergeStart), new DespawnEvent(AgentItem, x.MergeEnd)));
        status.AddRange(downEvents.Select(x => (x.Time, (StatusEvent)x)));
        status.AddRange(aliveEvents.Select(x => (x.Time, (StatusEvent)x)));
        status.AddRange(deadEvents.Select(x => (x.Time, (StatusEvent)x)));
        status.AddRange(regroupedEvents.Select(x => (x.Item1.Time, (StatusEvent)x.Item1)));
        status.AddRange(spawnEvents.Select(x => (x.Time, (StatusEvent)x)));
        status.AddRange(despawnEvents.Select(x => (x.Time, (StatusEvent)x)));
        status.AddRange(regroupedEvents.Select(x => (x.Item2.Time, (StatusEvent)x.Item2)));

        AddSegment(dc, long.MinValue, FirstAware);

        if (status.Count == 0)
        {
            AddSegment(actives, FirstAware, LastAware);
            AddSegment(dc, LastAware, long.MaxValue);
            return;
        }
        status = status.OrderBy(x => x.Time).ToList();

        for (int i = 0; i < status.Count - 1; i++)
        {
            var cur = status[i];
            var next = status[i + 1];
            AddValueToStatusList(dead, down, dc, actives, cur, next, FirstAware, i);
        }

        // check last value
        if (status.Count > 0)
        {
            var cur = status.Last();
            AddValueToStatusList(dead, down, dc, actives, cur, (LastAware, null), FirstAware, status.Count - 1);
            if (cur.evt is DeadEvent)
            {
                AddSegment(dead, LastAware, long.MaxValue);
            }
            else
            {
                AddSegment(dc, LastAware, long.MaxValue);
            }
        }
    }
    [MemberNotNull(nameof(_downs))]
    [MemberNotNull(nameof(_dcs))]
    [MemberNotNull(nameof(_actives))]
    [MemberNotNull(nameof(_deads))]
    public (IReadOnlyList<Segment> deads, IReadOnlyList<Segment> downs, IReadOnlyList<Segment> dcs, IReadOnlyList<Segment> actives) GetStatus(ParsedEvtcLog log)
    {
        _downs ??= [];
        _dcs ??= [];
        _actives ??= [];
        if (_deads == null)
        {
            _deads = [];
            GetAgentStatus(log, _deads, _downs, _dcs, _actives, log.CombatData);
        }
        return (_deads, _downs, _dcs, _actives);
    }
    public bool IsDowned(ParsedEvtcLog log, long time)
    {
        (_, IReadOnlyList<Segment> downs, _, _) = GetStatus(log);
        return downs.Any(x => x.ContainsPoint(time));
    }
    public bool IsDowned(ParsedEvtcLog log, long start, long end)
    {
        (_, IReadOnlyList<Segment> downs, _, _) = GetStatus(log);
        return downs.Any(x => x.Intersects(start, end));
    }
    public bool IsDead(ParsedEvtcLog log, long time)
    {
        (IReadOnlyList<Segment> deads, _, _, _) = GetStatus(log);
        return deads.Any(x => x.ContainsPoint(time));
    }
    public bool IsDead(ParsedEvtcLog log, long start, long end)
    {
        (IReadOnlyList<Segment> deads, _, _, _) = GetStatus(log);
        return deads.Any(x => x.Intersects(start, end));
    }
    public bool IsDC(ParsedEvtcLog log, long time)
    {
        (_, _, IReadOnlyList<Segment> dcs, _) = GetStatus(log);
        return dcs.Any(x => x.ContainsPoint(time));
    }
    public bool IsDC(ParsedEvtcLog log, long start, long end)
    {
        (_, _, IReadOnlyList<Segment> dcs, _) = GetStatus(log);
        return dcs.Any(x => x.Intersects(start, end));
    }
    public bool IsActive(ParsedEvtcLog log, long time)
    {
        (_, _, _, IReadOnlyList<Segment> actives) = GetStatus(log);
        return actives.Any(x => x.ContainsPoint(time));
    }
    public bool IsActive(ParsedEvtcLog log, long start, long end)
    {
        (_, _, _, IReadOnlyList<Segment> actives) = GetStatus(log);
        return actives.Any(x => x.Intersects(start, end));
    }
    #endregion STATUS
    #region BREAKBAR
    internal void GetAgentBreakbarStatus(List<Segment> nones, List<Segment> actives, List<Segment> immunes, List<Segment> recovering, CombatData combatData)
    {
        var status = new List<(long Time, BreakbarStateEvent evt)>();
        if (AgentItem.IsEnglobedAgent)
        {
            var firstEvent = combatData.GetBreakbarStateEvents(EnglobingAgentItem).LastOrDefault(x => x.Time < FirstAware);
            if (firstEvent != null)
            {
                status.Add((FirstAware, firstEvent));
            }
        }
        status.AddRange(combatData.GetBreakbarStateEvents(AgentItem).Select(x => (x.Time, x)));
        // State changes are not reliable on non squad actors, so we check if arc provided us with some, we skip events created by EI.
        if (AgentItem.Type == AgentType.NonSquadPlayer && !status.Any(x => !x.evt.IsCustom))
        {
            return;
        }

        if (status.Count == 0)
        {
            AddSegment(nones, FirstAware, LastAware);
            return;
        }
        for (int i = 0; i < status.Count - 1; i++)
        {
            var cur = status[i];
            if (i == 0 && cur.Time > FirstAware)
            {
                AddSegment(nones, FirstAware, cur.Time);
            }
            var next = status[i + 1];
            switch (cur.evt.State)
            {
                case BreakbarState.Active:
                    AddSegment(actives, cur.Time, next.Time);
                    break;
                case BreakbarState.Immune:
                    AddSegment(immunes, cur.Time, next.Time);
                    break;
                case BreakbarState.None:
                    AddSegment(nones, cur.Time, next.Time);
                    break;
                case BreakbarState.Recover:
                    AddSegment(recovering, cur.Time, next.Time);
                    break;
            }
        }
        // check last value
        if (status.Count > 0)
        {
            var cur = status.Last();
            if (LastAware - cur.Time >= ParserHelper.ServerDelayConstant)
            {
                switch (cur.evt.State)
                {
                    case BreakbarState.Active:
                        AddSegment(actives, cur.Time, LastAware);
                        break;
                    case BreakbarState.Immune:
                        AddSegment(immunes, cur.Time, LastAware);
                        break;
                    case BreakbarState.None:
                        AddSegment(nones, cur.Time, LastAware);
                        break;
                    case BreakbarState.Recover:
                        AddSegment(recovering, cur.Time, LastAware);
                        break;
                }
            }

        }
    }
    [MemberNotNull(nameof(_breakbarNones))]
    [MemberNotNull(nameof(_breakbarActives))]
    [MemberNotNull(nameof(_breakbarImmunes))]
    [MemberNotNull(nameof(_breakbarRecoverings))]
    public (IReadOnlyList<Segment> breakbarNones, IReadOnlyList<Segment> breakbarActives, IReadOnlyList<Segment> breakbarImmunes, IReadOnlyList<Segment> breakbarRecoverings) GetBreakbarStatus(ParsedEvtcLog log)
    {
        _breakbarActives ??= [];
        _breakbarImmunes ??= [];
        _breakbarRecoverings ??= [];
        if (_breakbarNones == null)
        {
            _breakbarNones = [];
            GetAgentBreakbarStatus(_breakbarNones, _breakbarActives, _breakbarImmunes, _breakbarRecoverings, log.CombatData);
        }
        return (_breakbarNones, _breakbarActives, _breakbarImmunes, _breakbarRecoverings);
    }

    public BreakbarState GetCurrentBreakbarState(ParsedEvtcLog log, long time)
    {
        var (nones, actives, immunes, recoverings) = GetBreakbarStatus(log);
        if (nones.Any(x => x.ContainsPoint(time)))
        {
            return BreakbarState.None;
        }

        if (actives.Any(x => x.ContainsPoint(time)))
        {
            return BreakbarState.Active;
        }

        if (immunes.Any(x => x.ContainsPoint(time)))
        {
            return BreakbarState.Immune;
        }

        if (recoverings.Any(x => x.ContainsPoint(time)))
        {
            return BreakbarState.Recover;
        }

        return BreakbarState.None;
    }
    #endregion BREAKBAR
    public long GetTimeSpentInCombat(ParsedEvtcLog log, long start, long end)
    {
        if (AgentItem.IsEnglobedAgent)
        {
            return log.FindActor(EnglobingAgentItem).GetTimeSpentInCombat(log, Math.Max(start, FirstAware), Math.Min(end, LastAware));
        }
        long timeInCombat = 0;
        foreach (EnterCombatEvent enTe in log.CombatData.GetEnterCombatEvents(AgentItem))
        {
            ExitCombatEvent? exCe = log.CombatData.GetExitCombatEvents(AgentItem).FirstOrDefault(x => x.Time > enTe.Time);
            if (exCe != null)
            {
                timeInCombat += Math.Max(Math.Min(exCe.Time, end) - Math.Max(enTe.Time, start), 0);
            }
            else
            {
                timeInCombat += Math.Max(end - Math.Max(enTe.Time, start), 0);
            }
        }
        if (timeInCombat == 0)
        {
            ExitCombatEvent? exCe = log.CombatData.GetExitCombatEvents(AgentItem).FirstOrDefault(x => x.Time > start);
            if (exCe != null)
            {
                timeInCombat += Math.Max(Math.Min(exCe.Time, end) - start, 0);
            }
            else
            {
                timeInCombat = Math.Max(end - start, 0);
            }
        }
        return timeInCombat;
    }

    public long GetActiveDuration(ParsedEvtcLog log, long start, long end)
    {
        _activeDurations ??= new CachingCollection<long>(log);
        if (!_activeDurations.TryGetValue(start, end, out var activeDuration))
        {
            var (dead, down, dc, _) = GetStatus(log);
            activeDuration = (end - start) -
                (long)dead.Sum(x => x.IntersectingArea(start, end)) -
                (long)dc.Sum(x => x.IntersectingArea(start, end));
            _activeDurations.Set(start, end, activeDuration);
        }
        return activeDuration;
    }

    public bool IsDownBeforeNext90(ParsedEvtcLog log, long curTime)
    {
        (IReadOnlyList<Segment> dead, IReadOnlyList<Segment> down, IReadOnlyList<Segment> dc, _) = GetStatus(log);

        // get remaining fight segment
        var remainingLogTime = new Segment(curTime, log.LogData.LogEnd);

        // return false if actor currently above 90 or already downed
        if (GetCurrentHealthPercent(log, curTime) > 90 || IsDowned(log, curTime))
        {
            return false;
        }
        
        // return false if fight ends before any down events
        Segment? nextDown = down.FirstOrNull((in Segment downSegment) => downSegment.Intersects(remainingLogTime));
        if (nextDown == null)
        {
            return false;
        }

        var healthUpdatesBeforeEnd = GetHealthUpdates(log).Where(update => update.Start > curTime);
        Segment? next90 = healthUpdatesBeforeEnd.FirstOrNull((in Segment update) => update.Value > 90);
       
        // If there are no more 90 events before combat end and the actor has a down event remaining then the actor must down before next 90
        if(next90 == null) { return true; }

        // Otherwise return false if the next 90 is before the next down
        if (next90.Value.Start < nextDown.Value.Start)
        {
            return false;
        }

        // Actor is below 90, will down before end of combat, and will not be above 90 again before end of combat
        return true;
    }

    /// <summary>
    /// Returns weapons used by Single Actor.
    /// Will always contain at least one element.
    /// </summary>
    /// <param name="log"></param>
    /// <returns></returns>
    public IReadOnlyList<WeaponSet> GetWeaponSets(ParsedEvtcLog log)
    {
        if (_weaponSets == null)
        {
            EstimateWeapons(log);
        }
        return _weaponSets!;
    }

    private void EstimateWeapons(ParsedEvtcLog log)
    {
        var firstWeaponSet = new WeaponSet(FirstAware, LastAware);
        _weaponSets = [firstWeaponSet];
        if (this is not PlayerActor)
        {
            return;
        }
        var currentWeaponSet = firstWeaponSet;
        var casting = GetCastEvents(log);
        int swapped = WeaponSetIDs.NoSet;
        long swappedTime = log.LogData.LogStart;
        var swaps = log.CombatData.GetWeaponSwapData(AgentItem);
        bool land1Swapped = false;
        bool land2Swapped = false;
        bool water1Swapped = false;
        bool water2Swapped = false;
        foreach (CastEvent cl in casting)
        {
            if (cl.ActualDuration == 0 && cl.SkillID != SkillIDs.WeaponSwap)
            {
                continue;
            }
            SkillItem skill = cl.Skill;
            // first iteration
            if (swapped == WeaponSetIDs.NoSet)
            {
                swapped = skill.FindFirstWeaponSet(swaps);
                land1Swapped = swapped == WeaponSetIDs.FirstLandSet;
                land2Swapped = swapped == WeaponSetIDs.SecondLandSet;
                water1Swapped = swapped == WeaponSetIDs.FirstWaterSet;
                water2Swapped = swapped == WeaponSetIDs.SecondWaterSet;
            }
            var estimateResult = skill.EstimateWeapons(currentWeaponSet, cl.Time, swapped, cl.Time > swappedTime + WeaponSwapDelayConstant);
            if (estimateResult != WeaponDescriptor.WeaponEstimateResult.NeedNewSet && cl is WeaponSwapEvent swe)
            {
                //wepswap  
                swapped = swe.SwappedTo;
                swappedTime = swe.Time;
                land1Swapped = land1Swapped || swapped == WeaponSetIDs.FirstLandSet;
                land2Swapped = land2Swapped || swapped == WeaponSetIDs.SecondLandSet;
                water1Swapped = water1Swapped || swapped == WeaponSetIDs.FirstWaterSet;
                water2Swapped = water2Swapped || swapped == WeaponSetIDs.SecondWaterSet;
            } 
            else if (estimateResult == WeaponDescriptor.WeaponEstimateResult.NeedNewSet)
            {
                currentWeaponSet.HasLandSwapped = land1Swapped && land2Swapped;
                currentWeaponSet.HasWaterSwapped = water1Swapped && water2Swapped;
                currentWeaponSet = new WeaponSet(currentWeaponSet.End, LastAware);
                // Reset count
                land1Swapped = swapped == WeaponSetIDs.FirstLandSet;
                land2Swapped = swapped == WeaponSetIDs.SecondLandSet;
                water1Swapped = swapped == WeaponSetIDs.FirstWaterSet;
                water2Swapped = swapped == WeaponSetIDs.SecondWaterSet;
            }
        }
    }
    public IReadOnlyList<DeathRecap> GetDeathRecaps(ParsedEvtcLog log)
    {
        if (_deathRecaps == null)
        {
            SetDeathRecaps(log);
        }
        return _deathRecaps!;
    }
    private void SetDeathRecaps(ParsedEvtcLog log)
    {
        IReadOnlyList<DeadEvent> deads = log.CombatData.GetDeadEvents(AgentItem);
        IReadOnlyList<DownEvent> downs = log.CombatData.GetDownEvents(AgentItem);
        IReadOnlyList<AliveEvent> ups = log.CombatData.GetAliveEvents(AgentItem);
        _deathRecaps = new List<DeathRecap>(deads.Count);
        long lastDeathTime = 0;
        var damageLogs = GetDamageTakenEvents(null, log);
        foreach (DeadEvent dead in deads)
        {
            _deathRecaps.Add(new DeathRecap(log, damageLogs, dead, downs, ups, lastDeathTime));
            lastDeathTime = dead.Time;
        }
    }

}
