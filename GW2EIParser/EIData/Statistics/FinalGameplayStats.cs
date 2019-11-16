using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.EIData
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


        public FinalGameplayStats(ParsedLog log, PhaseData phase, AbstractSingleActor actor, AbstractSingleActor target)
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
                    if (dl.Skill.CanCrit)
                    {
                        CritableDirectDamageCount++;
                    }
                }
            }
        }
    }
}
