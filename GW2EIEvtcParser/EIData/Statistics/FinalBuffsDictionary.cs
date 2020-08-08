using System;
using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    public class FinalBuffsDictionary
    {
        public Dictionary<AbstractSingleActor, double> Generated { get; } = new Dictionary<AbstractSingleActor, double>();
        public Dictionary<AbstractSingleActor, double> Overstacked { get; } = new Dictionary<AbstractSingleActor, double>();
        public Dictionary<AbstractSingleActor, double> Wasted { get; } = new Dictionary<AbstractSingleActor, double>();
        public Dictionary<AbstractSingleActor, double> UnknownExtension { get; } = new Dictionary<AbstractSingleActor, double>();
        public Dictionary<AbstractSingleActor, double> Extension { get; } = new Dictionary<AbstractSingleActor, double>();
        public Dictionary<AbstractSingleActor, double> Extended { get; } = new Dictionary<AbstractSingleActor, double>();


        internal static (FinalBuffsDictionary, FinalBuffsDictionary) GetFinalBuffsDictionary(ParsedEvtcLog log, Buff buff, BuffDistribution buffDistribution, long phaseDuration, long activePhaseDuration)
        {
            var buffs = new FinalBuffsDictionary();
            var buffsActive = new FinalBuffsDictionary();
            foreach (AbstractSingleActor actor in buffDistribution.GetSrcs(buff.ID, log))
            {
                long gen = buffDistribution.GetGeneration(buff.ID, actor.AgentItem);
                double generated = gen;
                double overstacked = (buffDistribution.GetOverstack(buff.ID, actor.AgentItem) + gen);
                double wasted = buffDistribution.GetWaste(buff.ID, actor.AgentItem);
                double unknownExtension = buffDistribution.GetUnknownExtension(buff.ID, actor.AgentItem);
                double extension = buffDistribution.GetExtension(buff.ID, actor.AgentItem);
                double extended = buffDistribution.GetExtended(buff.ID, actor.AgentItem);


                if (buff.Type == BuffType.Duration)
                {
                    generated *= 100.0;
                    overstacked *= 100.0;
                    wasted *= 100.0;
                    unknownExtension *= 100.0;
                    extension *= 100.0;
                    extended *= 100.0;
                }
                buffs.Generated[actor] = Math.Round(generated / phaseDuration, ParserHelper.BuffDigit);
                buffs.Overstacked[actor] = Math.Round(overstacked / phaseDuration, ParserHelper.BuffDigit);
                buffs.Wasted[actor] = Math.Round(wasted / phaseDuration, ParserHelper.BuffDigit);
                buffs.UnknownExtension[actor] = Math.Round(unknownExtension / phaseDuration, ParserHelper.BuffDigit);
                buffs.Extension[actor] = Math.Round(extension / phaseDuration, ParserHelper.BuffDigit);
                buffs.Extended[actor] = Math.Round(extended / phaseDuration, ParserHelper.BuffDigit);
                if (activePhaseDuration > 0)
                {
                    buffsActive.Generated[actor] = Math.Round(generated / activePhaseDuration, ParserHelper.BuffDigit);
                    buffsActive.Overstacked[actor] = Math.Round(overstacked / activePhaseDuration, ParserHelper.BuffDigit);
                    buffsActive.Wasted[actor] = Math.Round(wasted / activePhaseDuration, ParserHelper.BuffDigit);
                    buffsActive.UnknownExtension[actor] = Math.Round(unknownExtension / activePhaseDuration, ParserHelper.BuffDigit);
                    buffsActive.Extension[actor] = Math.Round(extension / activePhaseDuration, ParserHelper.BuffDigit);
                    buffsActive.Extended[actor] = Math.Round(extended / activePhaseDuration, ParserHelper.BuffDigit);
                }
                else
                {
                    buffsActive.Generated[actor] = 0.0;
                    buffsActive.Overstacked[actor] = 0.0;
                    buffsActive.Wasted[actor] = 0.0;
                    buffsActive.UnknownExtension[actor] = 0.0;
                    buffsActive.Extension[actor] = 0.0;
                    buffsActive.Extended[actor] = 0.0;
                }
            }
            return (buffs, buffsActive);
        }

    }

}
