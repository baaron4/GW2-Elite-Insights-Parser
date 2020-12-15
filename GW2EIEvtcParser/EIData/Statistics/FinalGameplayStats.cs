using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class FinalGameplayStats
    {
        public int TotalDamageCount { get; internal set; }
        public int DirectDamageCount { get; internal set; }
        public int ConnectedDirectDamageCount { get; internal set; }
        public int CritableDirectDamageCount { get; internal set; }
        public int CriticalCount { get; internal set; }
        public int CriticalDmg { get; internal set; }
        public int FlankingCount { get; internal set; }
        public int GlanceCount { get; internal set; }
        public int Missed { get; internal set; }
        public int Blocked { get; internal set; }
        public int Evaded { get; internal set; }
        public int Interrupts { get; internal set; }
        public int Invulned { get; internal set; }
        public int Killed { get; internal set; }
        public int Downed { get; internal set; }


        internal FinalGameplayStats(ParsedEvtcLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor target)
        {
            List<AbstractHealthDamageEvent> dls = actor.GetJustActorDamageEvents(target, log, start, end);
            foreach (AbstractHealthDamageEvent dl in dls)
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

                    if (dl.HasInterrupted)
                    {
                        Interrupts++;
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
