using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class SingleActorStatusHelper : AbstractSingleActorHelper
    {

        private List<Segment> _deads;
        private List<Segment> _downs;
        private List<Segment> _dcs;
        //
        private List<DeathRecap> _deathRecaps;
        //
        private List<Segment> _breakbarNones;
        private List<Segment> _breakbarActives;
        private List<Segment> _breakbarImmunes;
        private List<Segment> _breakbarRecoverings;
        //weaponslist
        private WeaponSets _weaponSets;

        public SingleActorStatusHelper(AbstractSingleActor actor) : base(actor)
        {
        }



        public (IReadOnlyList<Segment> deads, IReadOnlyList<Segment> downs, IReadOnlyList<Segment> dcs) GetStatus(ParsedEvtcLog log)
        {
            if (_deads == null)
            {
                _deads = new List<Segment>();
                _downs = new List<Segment>();
                _dcs = new List<Segment>();
                AgentItem.GetAgentStatus(_deads, _downs, _dcs, log.CombatData, log.FightData);
            }
            return (_deads, _downs, _dcs);
        }

        public (IReadOnlyList<Segment> breakbarNones, IReadOnlyList<Segment> breakbarActives, IReadOnlyList<Segment> breakbarImmunes, IReadOnlyList<Segment> breakbarRecoverings) GetBreakbarStatus(ParsedEvtcLog log)
        {
            if (_breakbarNones == null)
            {
                _breakbarNones = new List<Segment>();
                _breakbarActives = new List<Segment>();
                _breakbarImmunes = new List<Segment>();
                _breakbarRecoverings = new List<Segment>();
                AgentItem.GetAgentBreakbarStatus(_breakbarNones, _breakbarActives, _breakbarImmunes, _breakbarRecoverings, log.CombatData, log.FightData);
            }
            return (_breakbarNones, _breakbarActives, _breakbarImmunes, _breakbarRecoverings);
        }

        public long GetTimeSpentInCombat(ParsedEvtcLog log, long start, long end)
        {
            long timeInCombat = 0;
            foreach (EnterCombatEvent enTe in log.CombatData.GetEnterCombatEvents(Actor.AgentItem))
            {
                ExitCombatEvent exCe = log.CombatData.GetExitCombatEvents(Actor.AgentItem).FirstOrDefault(x => x.Time > enTe.Time);
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
                ExitCombatEvent exCe = log.CombatData.GetExitCombatEvents(Actor.AgentItem).FirstOrDefault(x => x.Time > start);
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
            (IReadOnlyList<Segment> dead, IReadOnlyList<Segment> down, IReadOnlyList<Segment> dc) = GetStatus(log);
            return (end - start) -
                (long)dead.Sum(x => x.IntersectingArea(start, end)) -
                (long)dc.Sum(x => x.IntersectingArea(start, end));
        }

        public bool IsDownBeforeNext90(ParsedEvtcLog log, long curTime)
        {
            (IReadOnlyList<Segment> dead, IReadOnlyList<Segment> down, IReadOnlyList<Segment> dc) = GetStatus(log);

            // get remaining fight segment
            var remainingFightTime = new Segment(curTime, log.FightData.FightEnd);

            // return false if actor currently above 90 or already downed
            if (Actor.GetCurrentHealthPercent(log, curTime) > 90 || Actor.IsDowned(log, curTime))
            {
                return false;
            }
            
            // return false if fight ends before any down events
            Segment nextDown = down.FirstOrDefault(downSegment => downSegment.IntersectSegment(remainingFightTime));
            if (nextDown == null)
            {
                return false;
            }

            IReadOnlyList<Segment> healthUpdatesBeforeEnd = Actor.GetHealthUpdates(log).Where(update => update.Start > curTime).ToList();
            Segment next90 = healthUpdatesBeforeEnd.FirstOrDefault(update => update.Value > 90);
           
            // If there are no more 90 events before combat end and the actor has a down event remaining then the actor must down before next 90
            if(next90 == null) { return true; }

            // Otherwise return false if the next 90 is before the next down
            if (next90.Start < nextDown.Start)
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
            return _weaponSets;
        }

        private void EstimateWeapons(ParsedEvtcLog log)
        {
            _weaponSets = new WeaponSets();
            if (!(Actor is AbstractPlayer))
            {
                return;
            }
            IReadOnlyList<AbstractCastEvent> casting = Actor.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            int swapped = WeaponSetIDs.NoSet;
            long swappedTime = 0;
            List<(int swappedTo, int swappedFrom)> swaps = log.CombatData.GetWeaponSwapData(AgentItem).Select(x =>
            {
                return (x.SwappedTo, x.SwappedFrom);
            }).ToList();
            foreach (AbstractCastEvent cl in casting)
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
            return _deathRecaps;
        }
        private void SetDeathRecaps(ParsedEvtcLog log)
        {
            _deathRecaps = new List<DeathRecap>();
            IReadOnlyList<DeadEvent> deads = log.CombatData.GetDeadEvents(AgentItem);
            IReadOnlyList<DownEvent> downs = log.CombatData.GetDownEvents(AgentItem);
            IReadOnlyList<AliveEvent> ups = log.CombatData.GetAliveEvents(AgentItem);
            long lastDeathTime = 0;
            IReadOnlyList<AbstractHealthDamageEvent> damageLogs = Actor.GetDamageTakenEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd);
            foreach (DeadEvent dead in deads)
            {
                _deathRecaps.Add(new DeathRecap(log, damageLogs, dead, downs, ups, lastDeathTime));
                lastDeathTime = dead.Time;
            }
        }

    }
}
