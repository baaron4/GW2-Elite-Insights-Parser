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
        private Dictionary<AbstractSingleActor, double> _incomingBy { get; } = new Dictionary<AbstractSingleActor, double>();
        public IReadOnlyDictionary<AbstractSingleActor, double> IncomingBy => _incomingBy;
        private Dictionary<AbstractSingleActor, double> _incomingByExtensionBy { get; } = new Dictionary<AbstractSingleActor, double>();
        public IReadOnlyDictionary<AbstractSingleActor, double> IncomingByExtensionBy => _incomingByExtensionBy;


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
                double incoming = 0;
                double incomingByExtension = 0;
                foreach (AbstractBuffApplyEvent abae in pair.Value)
                {
                    if (abae.Time >= start && abae.Time <= end)
                    {
                        if (abae is BuffApplyEvent bae)
                        {
                            // We ignore infinite duration buffs
                            if (bae.AppliedDuration >= int.MaxValue)
                            {
                                continue;
                            }
                            incoming += bae.AppliedDuration;
                        }
                        if (abae is BuffExtensionEvent bee)
                        {
                            incomingByExtension += bee.ExtendedDuration;
                            if (activePhaseDuration > 0)
                            {
                                incomingByExtension += bee.ExtendedDuration;
                            }
                        }
                    }

                }
                incoming += incomingByExtension;

                if (buff.Type == BuffType.Duration)
                {
                    incoming *= 100.0;
                    incomingByExtension *= 100.0;
                }
                buffs._incomingBy[actor] = Math.Round(incoming / phaseDuration, ParserHelper.BuffDigit);
                buffs._incomingByExtensionBy[actor] = Math.Round(incomingByExtension / phaseDuration, ParserHelper.BuffDigit);
                if (activePhaseDuration > 0)
                {
                    buffsActive._incomingBy[actor] = Math.Round(incoming / activePhaseDuration, ParserHelper.BuffDigit);
                    buffsActive._incomingByExtensionBy[actor] = Math.Round(incomingByExtension / activePhaseDuration, ParserHelper.BuffDigit);
                }
                else
                {
                    buffsActive._incomingBy[actor] = 0.0;
                    buffsActive._incomingByExtensionBy[actor] = 0.0;
                }
            }
            return (buffs, buffsActive);
        }

    }

}
