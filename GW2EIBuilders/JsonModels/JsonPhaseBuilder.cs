using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;
using Newtonsoft.Json;

namespace GW2EIBuilders.JsonModels
{
    /// <summary>
    /// Class corresponding to a phase
    /// </summary>
    internal static class JsonPhaseBuilder
    {

        public static JsonPhase BuildJsonPhase(PhaseData phase, ParsedEvtcLog log)
        {
            var jsPhase = new JsonPhase();
            jsPhase.Start = phase.Start;
            jsPhase.End = phase.End;
            jsPhase.Name = phase.Name;
            var targets = new List<int>();
            jsPhase.BreakbarPhase = phase.BreakbarPhase;
            foreach (AbstractSingleActor tar in phase.Targets)
            {
                targets.Add(log.FightData.Logic.Targets.IndexOf(tar));
            }
            jsPhase.Targets = targets;
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            if (!jsPhase.BreakbarPhase)
            {
                var subPhases = new List<int>();
                for (int j = 1; j < phases.Count; j++)
                {
                    PhaseData curPhase = phases[j];
                    if (curPhase.Start < jsPhase.Start || curPhase.End > jsPhase.End ||
                         (curPhase.Start == jsPhase.Start && curPhase.End == jsPhase.End) || !curPhase.CanBeSubPhase)
                    {
                        continue;
                    }
                    subPhases.Add(j);
                }
                if (subPhases.Any())
                {
                    jsPhase.SubPhases = subPhases;
                }
            }
            return jsPhase;
        }
    }
}
