using System;
using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    public class FinalBuffsDictionary
    {
        private Dictionary<AbstractSingleActor, double> _generatedBy { get; } = new Dictionary<AbstractSingleActor, double>();
        public IReadOnlyDictionary<AbstractSingleActor, double> GeneratedBy => _generatedBy;
        private Dictionary<AbstractSingleActor, double> _overstackedBy { get; } = new Dictionary<AbstractSingleActor, double>();
        public IReadOnlyDictionary<AbstractSingleActor, double> OverstackedBy => _overstackedBy;
        private Dictionary<AbstractSingleActor, double> _wastedFrom { get; } = new Dictionary<AbstractSingleActor, double>();
        public IReadOnlyDictionary<AbstractSingleActor, double> WastedFrom => _wastedFrom;
        private Dictionary<AbstractSingleActor, double> _unknownExtensionFrom { get; } = new Dictionary<AbstractSingleActor, double>();
        public IReadOnlyDictionary<AbstractSingleActor, double> UnknownExtensionFrom => _unknownExtensionFrom;
        private Dictionary<AbstractSingleActor, double> _extensionBy { get; } = new Dictionary<AbstractSingleActor, double>();
        public IReadOnlyDictionary<AbstractSingleActor, double> ExtensionBy => _extensionBy;
        private Dictionary<AbstractSingleActor, double> _extendedFrom { get; } = new Dictionary<AbstractSingleActor, double>();
        public IReadOnlyDictionary<AbstractSingleActor, double> ExtendedFrom => _extendedFrom;


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
                buffs._generatedBy[actor] = Math.Round(generated / phaseDuration, ParserHelper.BuffDigit);
                buffs._overstackedBy[actor] = Math.Round(overstacked / phaseDuration, ParserHelper.BuffDigit);
                buffs._wastedFrom[actor] = Math.Round(wasted / phaseDuration, ParserHelper.BuffDigit);
                buffs._unknownExtensionFrom[actor] = Math.Round(unknownExtension / phaseDuration, ParserHelper.BuffDigit);
                buffs._extensionBy[actor] = Math.Round(extension / phaseDuration, ParserHelper.BuffDigit);
                buffs._extendedFrom[actor] = Math.Round(extended / phaseDuration, ParserHelper.BuffDigit);
                if (activePhaseDuration > 0)
                {
                    buffsActive._generatedBy[actor] = Math.Round(generated / activePhaseDuration, ParserHelper.BuffDigit);
                    buffsActive._overstackedBy[actor] = Math.Round(overstacked / activePhaseDuration, ParserHelper.BuffDigit);
                    buffsActive._wastedFrom[actor] = Math.Round(wasted / activePhaseDuration, ParserHelper.BuffDigit);
                    buffsActive._unknownExtensionFrom[actor] = Math.Round(unknownExtension / activePhaseDuration, ParserHelper.BuffDigit);
                    buffsActive._extensionBy[actor] = Math.Round(extension / activePhaseDuration, ParserHelper.BuffDigit);
                    buffsActive._extendedFrom[actor] = Math.Round(extended / activePhaseDuration, ParserHelper.BuffDigit);
                }
                else
                {
                    buffsActive._generatedBy[actor] = 0.0;
                    buffsActive._overstackedBy[actor] = 0.0;
                    buffsActive._wastedFrom[actor] = 0.0;
                    buffsActive._unknownExtensionFrom[actor] = 0.0;
                    buffsActive._extensionBy[actor] = 0.0;
                    buffsActive._extendedFrom[actor] = 0.0;
                }
            }
            return (buffs, buffsActive);
        }

    }

}
