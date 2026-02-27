using System.Numerics;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class BreakbarPhaseData : SubPhasePhaseData
{
    public double BreakbarRecovered { get; private set; }
    public readonly long Offset;
    internal BreakbarPhaseData(long start, long end, string name, long startOffset) : base(start, end, name)
    {
        BreakbarPhase = true;
        Offset = -startOffset;
    }

    internal override void RemoveTarget(SingleActor target)
    {
        base.RemoveTarget(target);
        BreakbarRecovered = 0;
    }

    internal override void AddTarget(SingleActor? target, ParsedEvtcLog log, TargetPriority priority = TargetPriority.Main)
    {
        if (target != null)
        {
            if (Targets.Any())
            {
                throw new InvalidOperationException("Breakbar phases can only have one target");
            }
            BreakbarRecovered = Math.Round(log.CombatData.GetBreakbarRecoveredData(target.AgentItem).Where(x => x.Time >= Start + Offset && x.Time <= End).Sum(x => x.BreakbarRecovered), 1);
        }
        base.AddTarget(target, log, TargetPriority.Main);
    }

    internal override void AddTargets(IEnumerable<SingleActor?> targets, ParsedEvtcLog log, TargetPriority priority = TargetPriority.Main)
    {
        throw new InvalidOperationException("Breakbar phases can only have one target");
    }
}
