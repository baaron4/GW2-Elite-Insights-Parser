using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Mordremoth : StoryInstance
{
    public Mordremoth(int triggerID) : base(triggerID)
    {
        Extension = "mordr";
        Icon = EncounterIconMordremoth;
        EncounterCategoryInformation.InSubCategoryOrder = 0;
        EncounterID |= 0x000201;
    }

    protected override List<TrashID> GetTrashMobsIDs()
    {
        return
        [
            TrashID.SmotheringShadow,
        ];
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayMordremoth,
                        (899, 1172),
                        (-9059, 10171, -6183, 13149));
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Mordremoth)) ?? throw new MissingKeyActorsException("Vale Guardian not found");
        phases[0].AddTarget(mainTarget);
        if (!requirePhases)
        {
            return phases;
        }
        // Invul check
        phases.AddRange(GetPhasesByInvul(log, Determined762, mainTarget, false, true));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.Name = "Phase " + i;
            phase.AddTarget(mainTarget);
        }
        return phases;
    }

    protected override ReadOnlySpan<int> GetTargetsIDs()
    {
        return
        [
            (int)TargetID.Mordremoth,
            (int)TrashID.BlightedRytlock,
            //TrashID.BlightedCanach,
            (int)TrashID.BlightedBraham,
            (int)TrashID.BlightedMarjory,
            (int)TrashID.BlightedCaithe,
            (int)TrashID.BlightedForgal,
            (int)TrashID.BlightedSieran,
            //TrashID.BlightedTybalt,
            //TrashID.BlightedPaleTree,
            //TrashID.BlightedTrahearne,
            //TrashID.BlightedEir,
        ];
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        SingleActor mordremoth = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Mordremoth)) ?? throw new EvtcAgentException("Mordremoth not found");
        BuffApplyEvent? buffApply = combatData.GetBuffDataByIDByDst(Determined895, mordremoth.AgentItem).OfType<BuffApplyEvent>().LastOrDefault();
        if (buffApply != null)
        {
            fightData.SetSuccess(true, mordremoth.LastAware);
        }
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        SingleActor mordremoth = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Mordremoth)) ?? throw new MissingKeyActorsException("Mordremoth not found");
        return (mordremoth.GetHealth(combatData) > 9e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Story;
    }

    protected override ReadOnlySpan<int> GetFriendlyNPCIDs()
    {
        return
        [
            (int)TrashID.Canach,
            (int)TrashID.Braham,
            (int)TrashID.Caithe,
        ];
    }

    protected override ReadOnlySpan<int> GetUniqueNPCIDs()
    {
        return
        [
            (int)TargetID.Mordremoth,
            (int)TrashID.Canach,
            (int)TrashID.Braham,
            (int)TrashID.Caithe,
            (int)TrashID.BlightedRytlock,
            //TrashID.BlightedCanach,
            (int)TrashID.BlightedBraham,
            (int)TrashID.BlightedMarjory,
            (int)TrashID.BlightedCaithe,
            (int)TrashID.BlightedForgal,
            (int)TrashID.BlightedSieran,
            //TrashID.BlightedTybalt,
            //TrashID.BlightedPaleTree,
            //TrashID.BlightedTrahearne,
            //TrashID.BlightedEir,           
        ];
    }
}
