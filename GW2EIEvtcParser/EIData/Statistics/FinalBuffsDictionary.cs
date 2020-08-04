using System;
using System.Collections.Generic;
using GW2EIUtils;
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
                buffs.Generated[actor] = Math.Round(generated / phaseDuration, ParseHelper._buffDigit);
                buffs.Overstacked[actor] = Math.Round(overstacked / phaseDuration, ParseHelper._buffDigit);
                buffs.Wasted[actor] = Math.Round(wasted / phaseDuration, ParseHelper._buffDigit);
                buffs.UnknownExtension[actor] = Math.Round(unknownExtension / phaseDuration, ParseHelper._buffDigit);
                buffs.Extension[actor] = Math.Round(extension / phaseDuration, ParseHelper._buffDigit);
                buffs.Extended[actor] = Math.Round(extended / phaseDuration, ParseHelper._buffDigit);
                if (activePhaseDuration > 0)
                {
                    buffsActive.Generated[actor] = Math.Round(generated / activePhaseDuration, ParseHelper._buffDigit);
                    buffsActive.Overstacked[actor] = Math.Round(overstacked / activePhaseDuration, ParseHelper._buffDigit);
                    buffsActive.Wasted[actor] = Math.Round(wasted / activePhaseDuration, ParseHelper._buffDigit);
                    buffsActive.UnknownExtension[actor] = Math.Round(unknownExtension / activePhaseDuration, ParseHelper._buffDigit);
                    buffsActive.Extension[actor] = Math.Round(extension / activePhaseDuration, ParseHelper._buffDigit);
                    buffsActive.Extended[actor] = Math.Round(extended / activePhaseDuration, ParseHelper._buffDigit);
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
