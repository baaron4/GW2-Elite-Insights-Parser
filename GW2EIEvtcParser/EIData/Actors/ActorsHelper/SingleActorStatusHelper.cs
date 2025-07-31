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

    private List<DeathRecap>? _deathRecaps;

    private List<Segment>? _breakbarNones;
    private List<Segment>? _breakbarActives;
    private List<Segment>? _breakbarImmunes;
    private List<Segment>? _breakbarRecoverings;
    //weaponslist
    private WeaponSets? _weaponSets;

    private static void AddSegment(List<Segment> segments, long start, long end)
    {
        if (start < end)
        {
            segments.Add(new Segment(start, end, 1));
        }
    }

    private static void AddValueToStatusList(List<Segment> dead, List<Segment> down, List<Segment> dc, List<Segment> actives, StatusEvent cur, long nextTime, long minTime, int index)
    {
        long cTime = cur.Time;
        if (cur is DownEvent)
        {
            if (index == 0)
            {
                AddSegment(actives, minTime, cTime);
            }
            AddSegment(down, cTime, nextTime);
        }
        else if (cur is DeadEvent)
        {
            if (index == 0)
            {
                AddSegment(actives, minTime, cTime);
            }
            AddSegment(dead, cTime, nextTime);
        }
        else if (cur is DespawnEvent)
        {
            if (index == 0)
            {
                AddSegment(actives, minTime, cTime);
            }
            AddSegment(dc, cTime, nextTime);
        }
        else
        {
            if (index == 0 && cTime - minTime > 50)
            {
                AddSegment(dc, minTime, cTime);
            }
            AddSegment(actives, cTime, nextTime);
        }
    }
    #region STATUS
    internal void GetAgentStatus(List<Segment> dead, List<Segment> down, List<Segment> dc, List<Segment> actives, CombatData combatData)
    {
        //TODO(Rennorb) @perf: find average complexity
        var downEvents = combatData.GetDownEvents(AgentItem);
        var aliveEvents = combatData.GetAliveEvents(AgentItem);
        var deadEvents = combatData.GetDeadEvents(AgentItem);
        var spawnEvents = combatData.GetSpawnEvents(AgentItem);
        var despawnEvents = combatData.GetDespawnEvents(AgentItem);

        var status = new List<StatusEvent>(
            downEvents.Count +
            aliveEvents.Count +
            deadEvents.Count +
            spawnEvents.Count +
            despawnEvents.Count
        );
        status.AddRange(downEvents);
        status.AddRange(aliveEvents);
        status.AddRange(deadEvents);
        status.AddRange(spawnEvents);
        status.AddRange(despawnEvents);

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
            AddValueToStatusList(dead, down, dc, actives, cur, next.Time, FirstAware, i);
        }

        // check last value
        if (status.Count > 0)
        {
            var cur = status.Last();
            AddValueToStatusList(dead, down, dc, actives, cur, LastAware, FirstAware, status.Count - 1);
            if (cur is DeadEvent)
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
            GetAgentStatus(_deads, _downs, _dcs, _actives, log.CombatData);
        }
        return (_deads, _downs!, _dcs!, _actives!);
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
        var status = combatData.GetBreakbarStateEvents(AgentItem);
        // State changes are not reliable on non squad actors, so we check if arc provided us with some, we skip events created by EI.
        if (AgentItem.Type == AgentType.NonSquadPlayer && !status.Any(x => !x.IsCustom))
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
            switch (cur.State)
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
                switch (cur.State)
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
        return (_breakbarNones, _breakbarActives!, _breakbarImmunes!, _breakbarRecoverings!);
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
        var (dead, down, dc, _) = GetStatus(log);
        return (end - start) -
            (long)dead.Sum(x => x.IntersectingArea(start, end)) -
            (long)dc.Sum(x => x.IntersectingArea(start, end));
    }

    public bool IsDownBeforeNext90(ParsedEvtcLog log, long curTime)
    {
        (IReadOnlyList<Segment> dead, IReadOnlyList<Segment> down, IReadOnlyList<Segment> dc, _) = GetStatus(log);

        // get remaining fight segment
        var remainingFightTime = new Segment(curTime, log.FightData.FightEnd);

        // return false if actor currently above 90 or already downed
        if (GetCurrentHealthPercent(log, curTime) > 90 || IsDowned(log, curTime))
        {
            return false;
        }
        
        // return false if fight ends before any down events
        Segment? nextDown = down.FirstOrNull((in Segment downSegment) => downSegment.Intersects(remainingFightTime));
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


    public WeaponSets GetWeaponSets(ParsedEvtcLog log)
    {
        if (_weaponSets == null)
        {
            EstimateWeapons(log);
        }
        return _weaponSets!;
    }

    private void EstimateWeapons(ParsedEvtcLog log)
    {
        _weaponSets = new WeaponSets();
        if (this is not PlayerActor)
        {
            return;
        }
        var casting = GetCastEvents(log);
        int swapped = WeaponSetIDs.NoSet;
        long swappedTime = log.FightData.FightStart;
        List<(int swappedTo, int swappedFrom)> swaps = log.CombatData.GetWeaponSwapData(AgentItem).Select(x =>
        {
            return (x.SwappedTo, x.SwappedFrom);
        }).ToList();
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
            }
            if (!skill.EstimateWeapons(_weaponSets, swapped, cl.Time > swappedTime + WeaponSwapDelayConstant) && cl is WeaponSwapEvent swe)
            {
                //wepswap  
                swapped = swe.SwappedTo;
                swappedTime = swe.Time;
            }
        }
        int land1Swaps = swaps.Count(x => x.swappedTo == WeaponSetIDs.FirstLandSet);
        int land2Swaps = swaps.Count(x => x.swappedTo == WeaponSetIDs.SecondLandSet);
        int water1Swaps = swaps.Count(x => x.swappedTo == WeaponSetIDs.FirstWaterSet);
        int water2Swaps = swaps.Count(x => x.swappedTo == WeaponSetIDs.SecondWaterSet);
        _weaponSets.HasLandSwapped = land1Swaps > 0 && land2Swaps > 0;
        _weaponSets.HasWaterSwapped = water1Swaps > 0 && water2Swaps > 0;
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
