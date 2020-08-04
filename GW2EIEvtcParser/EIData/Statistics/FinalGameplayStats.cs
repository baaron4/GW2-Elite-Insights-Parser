using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public class FinalGameplayStats
    {
        public int DirectDamageCount { get; set; }
        public int CritableDirectDamageCount { get; set; }
        public int CriticalCount { get; set; }
        public int CriticalDmg { get; set; }
        public int FlankingCount { get; set; }
        public int GlanceCount { get; set; }
        public int Missed { get; set; }
        public int Interrupts { get; set; }
        public int Invulned { get; set; }


        public FinalGameplayStats(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor actor, AbstractSingleActor target)
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
