using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;

namespace GW2EIBuilders.JsonModels;

/// <summary>
/// Class corresponding to a phase
/// </summary>
internal static class JsonPhaseBuilder
{

    public static JsonPhase BuildJsonPhase(PhaseData phase, ParsedEvtcLog log)
    {
        var jsPhase = new JsonPhase
        {
            Start = phase.Start,
            End = phase.End,
            Name = phase.Name
        };
        var targets = new List<int>(phase.Targets.Count);
        jsPhase.BreakbarPhase = phase.BreakbarPhase;
        foreach (SingleActor tar in phase.Targets)
        {
            targets.Add(log.FightData.Logic.Targets.IndexOf(tar));
        }
        var secondaryTargets = new List<int>(phase.SecondaryTargets.Count);
        foreach (SingleActor tar in phase.SecondaryTargets)
        {
            secondaryTargets.Add(log.FightData.Logic.Targets.IndexOf(tar));
        }
        jsPhase.Targets = targets;
        jsPhase.SecondaryTargets = secondaryTargets;
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
            if (subPhases.Count != 0)
            {
                jsPhase.SubPhases = subPhases;
            }
        }
        return jsPhase;
    }
}
