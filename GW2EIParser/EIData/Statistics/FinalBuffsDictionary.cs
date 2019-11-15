using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.EIData
{
    public class FinalBuffsDictionary
    {
        public Dictionary<AbstractSingleActor, double> Generated { get; } = new Dictionary<AbstractSingleActor, double>();
        public Dictionary<AbstractSingleActor, double> Overstacked { get; } = new Dictionary<AbstractSingleActor, double>();
        public Dictionary<AbstractSingleActor, double> Wasted { get; } = new Dictionary<AbstractSingleActor, double>();
        public Dictionary<AbstractSingleActor, double> UnknownExtension { get; } = new Dictionary<AbstractSingleActor, double>();
        public Dictionary<AbstractSingleActor, double> Extension { get; } = new Dictionary<AbstractSingleActor, double>();
        public Dictionary<AbstractSingleActor, double> Extended { get; } = new Dictionary<AbstractSingleActor, double>();


        public static (FinalBuffsDictionary, FinalBuffsDictionary) GetFinalBuffsDictionary(ParsedLog log, Buff buff, BuffDistribution buffDistribution, long phaseDuration, long activePhaseDuration)
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


                if (buff.Type == Buff.BuffType.Duration)
                {
                    generated *= 100.0;
                    overstacked *= 100.0;
                    wasted *= 100.0;
                    unknownExtension *= 100.0;
                    extension *= 100.0;
                    extended *= 100.0;
                }
                buffs.Generated[actor] = Math.Round(generated / phaseDuration, GeneralHelper.BoonDigit);
                buffs.Overstacked[actor] = Math.Round(overstacked / phaseDuration, GeneralHelper.BoonDigit);
                buffs.Wasted[actor] = Math.Round(wasted / phaseDuration, GeneralHelper.BoonDigit);
                buffs.UnknownExtension[actor] = Math.Round(unknownExtension / phaseDuration, GeneralHelper.BoonDigit);
                buffs.Extension[actor] = Math.Round(extension / phaseDuration, GeneralHelper.BoonDigit);
                buffs.Extended[actor] = Math.Round(extended / phaseDuration, GeneralHelper.BoonDigit);
                if (activePhaseDuration > 0)
                {
                    buffsActive.Generated[actor] = Math.Round(generated / activePhaseDuration, GeneralHelper.BoonDigit);
                    buffsActive.Overstacked[actor] = Math.Round(overstacked / activePhaseDuration, GeneralHelper.BoonDigit);
                    buffsActive.Wasted[actor] = Math.Round(wasted / activePhaseDuration, GeneralHelper.BoonDigit);
                    buffsActive.UnknownExtension[actor] = Math.Round(unknownExtension / activePhaseDuration, GeneralHelper.BoonDigit);
                    buffsActive.Extension[actor] = Math.Round(extension / activePhaseDuration, GeneralHelper.BoonDigit);
                    buffsActive.Extended[actor] = Math.Round(extended / activePhaseDuration, GeneralHelper.BoonDigit);
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
