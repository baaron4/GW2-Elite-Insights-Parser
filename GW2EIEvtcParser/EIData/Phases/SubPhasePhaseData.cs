using System.Numerics;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class SubPhasePhaseData : PhaseData
{
    public EncounterPhaseData? EncounterPhase { get; private set; }

    internal SubPhasePhaseData(long start, long end) : base(start, end, PhaseType.TimeFrame)
    {
    }

    internal SubPhasePhaseData(long start, long end, string name) : this(start, end)
    {
        Name = name;
    }

    internal void AttachToEncounter(EncounterPhaseData encounterPhase)
    {
        EncounterPhase = encounterPhase;
    }

    internal override void AddParentPhase(PhaseData? phase)
    {
        if (phase != null)
        {
            base.AddParentPhase(phase);
            // Once a parent is provided to a TimeFrame phase, it becomes a subphase
            if (Type == PhaseType.TimeFrame)
            {
                Type = PhaseType.SubPhase;
            }
        }
    }
}
