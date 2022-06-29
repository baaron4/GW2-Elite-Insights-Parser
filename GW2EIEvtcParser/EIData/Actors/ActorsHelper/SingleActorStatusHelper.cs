using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class SingleActorStatusHelper : AbstractSingleActorHelper
    {

        private List<(long start, long end)> _deads;
        private List<(long start, long end)> _downs;
        private List<(long start, long end)> _dcs;
        //
        private List<DeathRecap> _deathRecaps;
        //weaponslist
        private WeaponSets _weaponSets;

        public SingleActorStatusHelper(AbstractSingleActor actor) : base(actor)
        {
        }



        public (IReadOnlyList<(long start, long end)> deads, IReadOnlyList<(long start, long end)> downs, IReadOnlyList<(long start, long end)> dcs) GetStatus(ParsedEvtcLog log)
        {
            if (_deads == null)
            {
                _deads = new List<(long start, long end)>();
                _downs = new List<(long start, long end)>();
                _dcs = new List<(long start, long end)>();
                AgentItem.GetAgentStatus(_deads, _downs, _dcs, log.CombatData, log.FightData);
            }
            return (_deads, _downs, _dcs);
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
            (IReadOnlyList<(long start, long end)> dead, IReadOnlyList<(long start, long end)> down, IReadOnlyList<(long start, long end)> dc) = GetStatus(log);
            return (end - start) -
                dead.Sum(x =>
                {
                    if (x.start <= end && x.end >= start)
                    {
                        long s = Math.Max(x.start, start);
                        long e = Math.Min(x.end, end);
                        return e - s;
                    }
                    return 0;
                }) -
                dc.Sum(x =>
                {
                    if (x.start <= end && x.end >= start)
                    {
                        long s = Math.Max(x.start, start);
                        long e = Math.Min(x.end, end);
                        return e - s;
                    }
                    return 0;
                });
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
            IReadOnlyList<AbstractCastEvent> casting = Actor.GetCastEvents(log, 0, log.FightData.FightEnd);
            int swapped = -1;
            long swappedTime = 0;
            var swaps = casting.OfType<WeaponSwapEvent>().Select(x =>
            {
                return x.SwappedTo;
            }).ToList();
            foreach (AbstractCastEvent cl in casting)
            {
                if (cl.ActualDuration == 0 && cl.SkillId != SkillIDs.WeaponSwap)
                {
                    continue;
                }
                SkillItem skill = cl.Skill;
                // first iteration
                if (swapped == -1)
                {
                    swapped = skill.FindWeaponSlot(swaps);
                }
                if (!skill.EstimateWeapons(_weaponSets, swapped, cl.Time > swappedTime + WeaponSwapDelayConstant) && cl is WeaponSwapEvent swe)
                {
                    //wepswap  
                    swapped = swe.SwappedTo;
                    swappedTime = swe.Time;
                }
            }
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
            IReadOnlyList<AbstractHealthDamageEvent> damageLogs = Actor.GetDamageTakenEvents(null, log, 0, log.FightData.FightEnd);
            foreach (DeadEvent dead in deads)
            {
                _deathRecaps.Add(new DeathRecap(log, damageLogs, dead, downs, ups, lastDeathTime));
                lastDeathTime = dead.Time;
            }
        }

    }
}
