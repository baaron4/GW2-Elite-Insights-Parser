using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class FinalOffensiveStats
    {
        public int TotalDamageCount { get; }
        public int DirectDamageCount { get; }
        public int ConnectedDamageCount { get; }
        public int ConnectedDirectDamageCount { get; }
        public int CritableDirectDamageCount { get; }
        public int CriticalCount { get; }
        public int CriticalDmg { get; }
        public int FlankingCount { get; }
        public int GlanceCount { get; }
        public int AgainstMovingCount { get; }
        public int Missed { get; }
        public int Blocked { get; }
        public int Evaded { get; }
        public int Interrupts { get; }
        public int Invulned { get; }
        public int Killed { get; }
        public int Downed { get; }


        internal FinalOffensiveStats(ParsedEvtcLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor target)
        {
            IReadOnlyList<AbstractHealthDamageEvent> dls = actor.GetDamageEvents(target, log, start, end);
            foreach (AbstractHealthDamageEvent dl in dls)
            {
                if (dl.From == actor.AgentItem)
                {
                    if (!(dl is NonDirectHealthDamageEvent))
                    {
                        if (dl.HasHit)
                        {
                            if (SkillItem.CanCrit(dl.SkillId, log.LogData.GW2Build))
                            {
                                if (dl.HasCrit)
                                {
                                    CriticalCount++;
                                    CriticalDmg += dl.HealthDamage;
                                }
                                CritableDirectDamageCount++;
                            }
                            if (dl.IsFlanking)
                            {
                                FlankingCount++;
                            }

                            if (dl.HasGlanced)
                            {
                                GlanceCount++;
                            }
                            ConnectedDirectDamageCount++;
                        }

                        if (dl.IsBlind)
                        {
                            Missed++;
                        }
                        if (dl.IsEvaded)
                        {
                            Evaded++;
                        }
                        if (dl.IsBlocked)
                        {
                            Blocked++;
                        }
                        if (!dl.DoubleProcHit)
                        {
                            DirectDamageCount++;
                        }
                    }
                    if (dl.IsAbsorbed)
                    {
                        Invulned++;
                    }
                    if (!dl.DoubleProcHit)
                    {
                        TotalDamageCount++;
                    }

                    if (dl.HasHit)
                    {
                        ConnectedDamageCount++;
                        if (dl.AgainstMoving)
                        {
                            AgainstMovingCount++;
                        }
                    }
                }

                if (!(dl is NonDirectHealthDamageEvent))
                {
                    if (dl.HasInterrupted)
                    {
                        Interrupts++;
                    }
                }
                if (dl.HasKilled)
                {
                    Killed++;
                }
                if (dl.HasDowned)
                {
                    Downed++;
                }

            }
        }
    }
}
