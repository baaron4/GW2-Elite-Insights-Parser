using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    public class FinalBuffVolumesDictionary
    {
        private Dictionary<AbstractSingleActor, double> _generated { get; } = new Dictionary<AbstractSingleActor, double>();
        public IReadOnlyDictionary<AbstractSingleActor, double> Generated => _generated;
        private Dictionary<AbstractSingleActor, double> _extension { get; } = new Dictionary<AbstractSingleActor, double>();
        public IReadOnlyDictionary<AbstractSingleActor, double> Extension => _extension;


        internal static (FinalBuffVolumesDictionary, FinalBuffVolumesDictionary) GetFinalBuffVolumesDictionary(ParsedEvtcLog log, Buff buff, AbstractSingleActor dstActor, long start, long end)
        {
            long phaseDuration = end - start;
            long activePhaseDuration = dstActor.GetActiveDuration(log, start, end);

            var buffs = new FinalBuffVolumesDictionary();
            var buffsActive = new FinalBuffVolumesDictionary();

            var applies = log.CombatData.GetBuffDataByIDByDst(buff.ID, dstActor.AgentItem).OfType<AbstractBuffApplyEvent>().ToList();
            applies.ForEach(x => x.TryFindSrc(log));
            var appliesBySrc = applies.GroupBy(x => x.CreditedBy).ToDictionary(x => x.Key, x => x.ToList());

            foreach (KeyValuePair<AgentItem, List<AbstractBuffApplyEvent>> pair in appliesBySrc)
            {
                AbstractSingleActor actor = log.FindActor(pair.Key);
                double generated = 0;
                double extension = 0;
                foreach (AbstractBuffApplyEvent abae in pair.Value)
                {
                    if (abae is BuffApplyEvent bae)
                    {
                        generated += bae.AppliedDuration;
                    }
                    if (abae is BuffExtensionEvent bee)
                    {
                        extension += bee.ExtendedDuration;
                        if (activePhaseDuration > 0)
                        {
                            extension += bee.ExtendedDuration;
                        }
                    }

                }
                generated += extension;

                if (buff.Type == BuffType.Duration)
                {
                    generated *= 100.0;
                    extension *= 100.0;
                }
                buffs._generated[actor] = Math.Round(generated / phaseDuration, ParserHelper.BuffDigit);
                buffs._extension[actor] = Math.Round(extension / phaseDuration, ParserHelper.BuffDigit);
                if (activePhaseDuration > 0)
                {
                    buffsActive._generated[actor] = Math.Round(generated / activePhaseDuration, ParserHelper.BuffDigit);
                    buffsActive._extension[actor] = Math.Round(extension / activePhaseDuration, ParserHelper.BuffDigit);
                }
                else
                {
                    buffsActive._generated[actor] = 0.0;
                    buffsActive._extension[actor] = 0.0;
                }
            }
            return (buffs, buffsActive);
        }

    }

}
