using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.SkillIDs;

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

    protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
    {
        return
        [
            ArcDPSEnums.TrashID.SmotheringShadow,
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
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Mordremoth)) ?? throw new MissingKeyActorsException("Vale Guardian not found");
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
            (int)ArcDPSEnums.TargetID.Mordremoth,
            (int)ArcDPSEnums.TrashID.BlightedRytlock,
            //ArcDPSEnums.TrashID.BlightedCanach,
            (int)ArcDPSEnums.TrashID.BlightedBraham,
            (int)ArcDPSEnums.TrashID.BlightedMarjory,
            (int)ArcDPSEnums.TrashID.BlightedCaithe,
            (int)ArcDPSEnums.TrashID.BlightedForgal,
            (int)ArcDPSEnums.TrashID.BlightedSieran,
            //ArcDPSEnums.TrashID.BlightedTybalt,
            //ArcDPSEnums.TrashID.BlightedPaleTree,
            //ArcDPSEnums.TrashID.BlightedTrahearne,
            //ArcDPSEnums.TrashID.BlightedEir,
        ];
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        SingleActor mordremoth = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Mordremoth)) ?? throw new EvtcAgentException("Mordremoth not found");
        BuffApplyEvent? buffApply = combatData.GetBuffDataByIDByDst(Determined895, mordremoth.AgentItem).OfType<BuffApplyEvent>().LastOrDefault();
        if (buffApply != null)
        {
            fightData.SetSuccess(true, mordremoth.LastAware);
        }
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        SingleActor mordremoth = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Mordremoth)) ?? throw new MissingKeyActorsException("Mordremoth not found");
        return (mordremoth.GetHealth(combatData) > 9e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Story;
    }

    protected override ReadOnlySpan<int> GetFriendlyNPCIDs()
    {
        return
        [
            (int)ArcDPSEnums.TrashID.Canach,
            (int)ArcDPSEnums.TrashID.Braham,
            (int)ArcDPSEnums.TrashID.Caithe,
        ];
    }

    protected override ReadOnlySpan<int> GetUniqueNPCIDs()
    {
        return
        [
            (int)ArcDPSEnums.TargetID.Mordremoth,
            (int)ArcDPSEnums.TrashID.Canach,
            (int)ArcDPSEnums.TrashID.Braham,
            (int)ArcDPSEnums.TrashID.Caithe,
            (int)ArcDPSEnums.TrashID.BlightedRytlock,
            //ArcDPSEnums.TrashID.BlightedCanach,
            (int)ArcDPSEnums.TrashID.BlightedBraham,
            (int)ArcDPSEnums.TrashID.BlightedMarjory,
            (int)ArcDPSEnums.TrashID.BlightedCaithe,
            (int)ArcDPSEnums.TrashID.BlightedForgal,
            (int)ArcDPSEnums.TrashID.BlightedSieran,
            //ArcDPSEnums.TrashID.BlightedTybalt,
            //ArcDPSEnums.TrashID.BlightedPaleTree,
            //ArcDPSEnums.TrashID.BlightedTrahearne,
            //ArcDPSEnums.TrashID.BlightedEir,           
        ];
    }
}
