using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public class FinalGameplayStats
    {
        public int TotalHitCount { get; internal set; }
        public int DirectHitCount { get; internal set; }
        public int ConnectedDirectHitCount { get; internal set; }
        public int CritableDirectHitCount { get; internal set; }
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
                    if (dl.HasHit) {
                        if (SkillItem.CanCrit(dl.SkillId, log.LogData.GW2Build))
                        {
                            if (dl.HasCrit)
                            {
                                CriticalCount++;
                                CriticalDmg += dl.Damage;
                            }
                            CritableDirectHitCount++;
                        }
                        ConnectedDirectHitCount++;
                    }

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
                    if (dl.IsEvaded)
                    {
                        Evaded++;
                    }
                    if (dl.IsBlocked)
                    {
                        Blocked++;
                    }
                    DirectHitCount++;
                }
                if (dl.IsAbsorbed)
                {
                    Invulned++;
                    DamageInvulned += dl.Damage;
                }
            }
        }
    }
}
