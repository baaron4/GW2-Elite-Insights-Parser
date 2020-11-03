using System;
using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    public class FinalBuffsDictionary
    {
        private Dictionary<AbstractSingleActor, double> _generated { get; } = new Dictionary<AbstractSingleActor, double>();
        private Dictionary<AbstractSingleActor, double> _overstacked { get; } = new Dictionary<AbstractSingleActor, double>();
        private Dictionary<AbstractSingleActor, double> _wasted { get; } = new Dictionary<AbstractSingleActor, double>();
        private Dictionary<AbstractSingleActor, double> _unknownExtension { get; } = new Dictionary<AbstractSingleActor, double>();
        private Dictionary<AbstractSingleActor, double> _extension { get; } = new Dictionary<AbstractSingleActor, double>();
        private Dictionary<AbstractSingleActor, double> _extended { get; } = new Dictionary<AbstractSingleActor, double>();

        public IReadOnlyDictionary<AbstractSingleActor, double> Generated => _generated;
        public IReadOnlyDictionary<AbstractSingleActor, double> Overstacked => _overstacked;
        public IReadOnlyDictionary<AbstractSingleActor, double> Wasted => _wasted;
        public IReadOnlyDictionary<AbstractSingleActor, double> UnknownExtension => _unknownExtension;
        public IReadOnlyDictionary<AbstractSingleActor, double> Extension => _extension;
        public IReadOnlyDictionary<AbstractSingleActor, double> Extended => _extended;


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
                buffs._generated[actor] = Math.Round(generated / phaseDuration, ParserHelper.BuffDigit);
                buffs._overstacked[actor] = Math.Round(overstacked / phaseDuration, ParserHelper.BuffDigit);
                buffs._wasted[actor] = Math.Round(wasted / phaseDuration, ParserHelper.BuffDigit);
                buffs._unknownExtension[actor] = Math.Round(unknownExtension / phaseDuration, ParserHelper.BuffDigit);
                buffs._extension[actor] = Math.Round(extension / phaseDuration, ParserHelper.BuffDigit);
                buffs._extended[actor] = Math.Round(extended / phaseDuration, ParserHelper.BuffDigit);
                if (activePhaseDuration > 0)
                {
                    buffsActive._generated[actor] = Math.Round(generated / activePhaseDuration, ParserHelper.BuffDigit);
                    buffsActive._overstacked[actor] = Math.Round(overstacked / activePhaseDuration, ParserHelper.BuffDigit);
                    buffsActive._wasted[actor] = Math.Round(wasted / activePhaseDuration, ParserHelper.BuffDigit);
                    buffsActive._unknownExtension[actor] = Math.Round(unknownExtension / activePhaseDuration, ParserHelper.BuffDigit);
                    buffsActive._extension[actor] = Math.Round(extension / activePhaseDuration, ParserHelper.BuffDigit);
                    buffsActive._extended[actor] = Math.Round(extended / activePhaseDuration, ParserHelper.BuffDigit);
                }
                else
                {
                    buffsActive._generated[actor] = 0.0;
                    buffsActive._overstacked[actor] = 0.0;
                    buffsActive._wasted[actor] = 0.0;
                    buffsActive._unknownExtension[actor] = 0.0;
                    buffsActive._extension[actor] = 0.0;
                    buffsActive._extended[actor] = 0.0;
                }
            }
            return (buffs, buffsActive);
        }

    }

}
