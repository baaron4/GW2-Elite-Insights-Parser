using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
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



    public (IReadOnlyList<Segment> deads, IReadOnlyList<Segment> downs, IReadOnlyList<Segment> dcs, IReadOnlyList<Segment> actives) GetStatus(ParsedEvtcLog log)
    {
        if (_deads == null)
        {
            _deads = [];
            _downs = [];
            _dcs = [];
            _actives = [];
            AgentItem.GetAgentStatus(_deads, _downs, _dcs, _actives, log.CombatData);
        }
        return (_deads, _downs!, _dcs!, _actives!);
    }

    public (IReadOnlyList<Segment> breakbarNones, IReadOnlyList<Segment> breakbarActives, IReadOnlyList<Segment> breakbarImmunes, IReadOnlyList<Segment> breakbarRecoverings) GetBreakbarStatus(ParsedEvtcLog log)
    {
        if (_breakbarNones == null)
        {
            _breakbarNones = [];
            _breakbarActives = [];
            _breakbarImmunes = [];
            _breakbarRecoverings = [];
            AgentItem.GetAgentBreakbarStatus(_breakbarNones, _breakbarActives, _breakbarImmunes, _breakbarRecoverings, log.CombatData);
        }
        return (_breakbarNones, _breakbarActives!, _breakbarImmunes!, _breakbarRecoverings!);
    }

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
        var casting = GetCastEvents(log, log.FightData.LogStart, log.FightData.FightEnd);
        int swapped = WeaponSetIDs.NoSet;
        long swappedTime = log.FightData.FightStart;
        List<(int swappedTo, int swappedFrom)> swaps = log.CombatData.GetWeaponSwapData(AgentItem).Select(x =>
        {
            return (x.SwappedTo, x.SwappedFrom);
        }).ToList();
        foreach (CastEvent cl in casting)
        {
            if (cl.ActualDuration == 0 && cl.SkillId != SkillIDs.WeaponSwap)
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
        var damageLogs = GetDamageTakenEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd);
        foreach (DeadEvent dead in deads)
        {
            _deathRecaps.Add(new DeathRecap(log, damageLogs, dead, downs, ups, lastDeathTime));
            lastDeathTime = dead.Time;
        }
    }

}
