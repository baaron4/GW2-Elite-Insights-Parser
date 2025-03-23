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
        var secondaryTargets = new List<int>(phase.Targets.Count);
        var targetPriorities = new Dictionary<int, string>();
        jsPhase.BreakbarPhase = phase.BreakbarPhase;
        foreach (var pair in phase.Targets)
        {
            var tar = pair.Key;
            var tarIndex = log.FightData.Logic.Targets.IndexOf(tar);
            if (pair.Value.IsPrioritary(PhaseData.TargetPriority.Blocking))
            {
                targets.Add(tarIndex);
            } 
            else
            {
                secondaryTargets.Add(tarIndex);
            }

            string priority = pair.Value.Priority switch
            {
                PhaseData.TargetPriority.Blocking => "BLOCKING",
                PhaseData.TargetPriority.Main => "MAIN",
                PhaseData.TargetPriority.NonBlocking => "NONBLOCKING",
                _ => throw new NotImplementedException("Support for given priority not implemented"),
            };
            targetPriorities[tarIndex] = priority;
        }
        jsPhase.Targets = targets;
        jsPhase.SecondaryTargets = secondaryTargets;
        jsPhase.TargetPriorities = targetPriorities;
        IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
        if (!jsPhase.BreakbarPhase)
        {
            var subPhases = new List<int>();
            for (int j = 1; j < phases.Count; j++)
            {
                PhaseData curPhase = phases[j];
                if (curPhase.Start < jsPhase.Start || curPhase.End > jsPhase.End ||
                     (curPhase.Start == jsPhase.Start && curPhase.End == jsPhase.End) || !curPhase.CanBeSubPhaseOf.Contains(curPhase))
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
