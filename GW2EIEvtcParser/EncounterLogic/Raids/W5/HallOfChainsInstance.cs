using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class HallOfChainsInstance : HallOfChains
{
    public HallOfChainsInstance(int triggerID) : base(triggerID)
    {
        EncounterID = 0;
        Icon = InstanceIconHallOfChains;
        Extension = "hallchains";
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Hall Of Chains";
    }

    protected override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.SoullessHorror,
            TargetID.BrokenKing,
            TargetID.EaterOfSouls,
            TargetID.EyeOfFate,
            TargetID.EyeOfJudgement,
            TargetID.Dhuum,
        ];
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        return GetGenericFightOffset(fightData);
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases;
        if (Targets.Count == 0)
        {
            phases = base.GetPhases(log, requirePhases);
            if (log.CombatData.GetEvtcVersionEvent().Build >= ArcDPSBuilds.LogStartLogEndPerCombatSequenceOnInstanceLogs)
            {
                var fightPhases = GetPhasesBySquadCombatStartEnd(log);
                fightPhases.ForEach(x =>
                {
                    x.AddTargets(phases[0].Targets.Keys, log);
                    x.AddParentPhase(phases[0]);
                });
                phases.AddRange(fightPhases);
            }
            return phases;
        }
        phases = GetInitialPhase(log);
        phases[0].AddTargets(Targets, log);
        int phaseCount = 0;
        foreach (SingleActor target in Targets)
        {
            var phase = new PhaseData(Math.Max(log.FightData.FightStart, target.FirstAware), Math.Min(target.LastAware, log.FightData.FightEnd), "Phase " + (++phaseCount));
            phase.AddTarget(target, log);
            phases.Add(phase);
        }
        return phases;
    }
}
