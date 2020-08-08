using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public class FinalGameplayStats
    {
        public int DirectDamageCount { get; internal set; }
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
        public int DamageInvulned { get; internal set; }


        internal FinalGameplayStats(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor actor, AbstractSingleActor target)
        {
            List<AbstractDamageEvent> dls = actor.GetJustPlayerDamageLogs(target, log, phase);
            foreach (AbstractDamageEvent dl in dls)
            {
                if (!(dl is NonDirectDamageEvent))
                {

                    if (dl.IsFlanking)
                    {
                        FlankingCount++;
                    }

                    if (dl.HasGlanced)
                    {
                        GlanceCount++;
                    }

                    if (dl.IsBlind)
                    {
                        Missed++;
                    }
                    if (dl.HasInterrupted)
                    {
                        Interrupts++;
                    }

                    if (dl.IsAbsorbed)
                    {
                        Invulned++;
                        DamageInvulned += dl.Damage;
                    }
                    if (dl.IsEvaded)
                    {
                        Evaded++;
                    }
                    if (dl.IsBlocked)
                    {
                        Blocked++;
                    }
                    DirectDamageCount++;
                    if (SkillItem.CanCrit(dl.SkillId, log.LogData.GW2Build) && dl.HasHit)
                    {
                        if (dl.HasCrit)
                        {
                            CriticalCount++;
                            CriticalDmg += dl.Damage;
                        }
                        CritableDirectDamageCount++;
                    }
                }
            }
        }
    }
}
