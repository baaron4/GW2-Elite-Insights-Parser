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
        public int Interrupts { get; internal set; }
        public int Invulned { get; internal set; }


        internal FinalGameplayStats(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor actor, AbstractSingleActor target)
        {
            List<AbstractDamageEvent> dls = actor.GetJustPlayerDamageLogs(target, log, phase);
            foreach (AbstractDamageEvent dl in dls)
            {
                if (!(dl is NonDirectDamageEvent))
                {
                    if (dl.HasCrit)
                    {
                        CriticalCount++;
                        CriticalDmg += dl.Damage;
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

                    if (dl.IsAbsorbed)
                    {
                        Invulned++;
                    }
                    DirectDamageCount++;
                    if (SkillItem.CanCrit(dl.Skill.ID, log.LogData.GW2Build))
                    {
                        CritableDirectDamageCount++;
                    }
                }
            }
        }
    }
}
