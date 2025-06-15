using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class SpiritRace : SpiritVale
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new PlayerDstHitMechanic(SpiritFog, new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Red), "SpiritFog.H", "Hit by Spirit Fog", "Spirit Fog Hit", 0),
            new PlayerDstBuffApplyMechanic(Crippled, new MechanicPlotlySetting(Symbols.Diamond, Colors.Pink), "Outrun.Achiv", "Achievement Eligibility: I Can Outrun A...Ghost", "I Can Outrun A...Ghost", 0).UsingAchievementEligibility(),
        ]);
    public SpiritRace(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "sprtrace";
        Icon = EncounterIconSpiritRace;
        EncounterCategoryInformation.InSubCategoryOrder = 1;
        EncounterID |= 0x000004;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplaySpiritRun,
                        (581, 1193),
                        (-11188, -13757, -4700, -436)
                        /*,
                        (-15360, -36864, 15360, 39936),
                        (3456, 11012, 4736, 14212)*/);
    }
    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.DummyTarget,
            TargetID._EtherealBarrier1,
            TargetID._EtherealBarrier2,
            TargetID._EtherealBarrier3,
            TargetID._EtherealBarrier4,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.WallOfGhosts,
            TargetID.AngeredSpiritSR,
            TargetID.AngeredSpiritSR2,
            TargetID.EnragedSpiritSR,
            TargetID.DerangedSpiritSR,
            TargetID.DerangedSpiritSR2,
        ];
    }

    internal override int GetTriggerID()
    {
        return (int)TargetID.WallOfGhosts;
    }

    internal static RewardEvent? GetRewardEvent(CombatData combatData, long start, long end)
    {
        return combatData.GetRewardEvents().FirstOrDefault(x => x.RewardType == RewardTypes.OldRaidReward2 && x.Time > start && x.Time < end);
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        RewardEvent? reward = GetRewardEvent(combatData, fightData.FightStart, fightData.LogEnd);
        if (reward != null)
        {
            fightData.SetSuccess(true, reward.Time);
        }
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        if (Targetless)
        {
            return base.GetPhases(log, requirePhases);
        }
        List<PhaseData> phases = GetInitialPhase(log);
        phases[0].AddTargets(Targets.Where(x => x.IsSpecies(TargetID.EtherealBarrier)), log);
        return phases;
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {

        AgentItem? wallOfGhosts = agentData.GetNPCsByID(TargetID.WallOfGhosts).FirstOrDefault();
        if (wallOfGhosts == null)
        {
            return FightData.EncounterStartStatus.Late;
        }
        var position = combatData.GetMovementData(wallOfGhosts).Where(x => x is PositionEvent positionEvt).FirstOrDefault();
        if (position != null)
        {
            var initialPosition = new Vector3(-5669.139f, -7814.589f, -1138.749f);
            return (position.GetPoint3D() - initialPosition).Length() > 10 ? FightData.EncounterStartStatus.Late : FightData.EncounterStartStatus.Normal;
        }
        // To investigate
        return FightData.EncounterStartStatus.Late;
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        AgentItem? wallOfGhosts = agentData.GetNPCsByID(TargetID.WallOfGhosts).FirstOrDefault();
        if (wallOfGhosts != null)
        {
            foreach(var velocityEvent in combatData.Where(x => x.IsStateChange == StateChange.Velocity && x.SrcMatchesAgent(wallOfGhosts)))
            {
                if(MovementEvent.GetPointXY(velocityEvent) != default)
                {
                    //first velocity
                    return velocityEvent.Time;
                }

            }
        }
        return EncounterLogicTimeUtils.GetGenericFightOffset(fightData);
    }

    internal static bool FindEtherealBarriers(AgentData agentData, List<CombatItem> combatData)
    {
        var maxHPs = combatData.Where(x => x.IsStateChange == StateChange.MaxHealthUpdate && MaxHealthUpdateEvent.GetMaxHealth(x) == 1494000);
        bool needsDummy = true;
        var position1 = new Vector2(-7607.929f, -12493.7051f/*, -1112.468f*/);
        var position2 = new Vector2(-8423.886f, -9858.193f/*, -1335.1134f*/);
        var position3 = new Vector2(-9104.786f, -6910.657f/*, -2405.52222f*/);
        var position4 = new Vector2(-8552.994f, -863.6334f/*, -1416.31714f*/);
        foreach (CombatItem maxHP in maxHPs)
        {
            AgentItem candidate = agentData.GetAgent(maxHP.SrcAgent, maxHP.Time);
            if (candidate.Type == AgentItem.AgentType.Gadget)
            {
                needsDummy = false;
                var positions = combatData.Where(x => x.IsStateChange == StateChange.Position && x.SrcMatchesAgent(candidate)).Select(MovementEvent.GetPointXY);
                if (positions.Any(x => (x - position1).Length() < 10))
                {
                    candidate.OverrideID(TargetID._EtherealBarrier1, agentData);
                }
                else if (positions.Any(x => (x - position2).Length() < 10))
                {
                    candidate.OverrideID(TargetID._EtherealBarrier2, agentData);
                }
                else if (positions.Any(x => (x - position3).Length() < 10))
                {
                    candidate.OverrideID(TargetID._EtherealBarrier3, agentData);
                }
                else if (positions.Any(x => (x - position4).Length() < 10))
                {
                    candidate.OverrideID(TargetID._EtherealBarrier4, agentData);
                }
                candidate.OverrideType(AgentItem.AgentType.NPC, agentData);
            }
        }
        return !needsDummy;
    }

    internal static void RenameEtherealBarriersAndOverrideID(IReadOnlyList<SingleActor> targets, AgentData agentData)
    {
        foreach (SingleActor target in targets)
        {
            switch (target.ID)
            {
                case (int)TargetID._EtherealBarrier1:
                    target.OverrideName("First " + target.Character);
                    target.AgentItem.OverrideID(TargetID.EtherealBarrier, agentData);
                    break;
                case (int)TargetID._EtherealBarrier2:
                    target.OverrideName("Second " + target.Character);
                    target.AgentItem.OverrideID(TargetID.EtherealBarrier, agentData);
                    break;
                case (int)TargetID._EtherealBarrier3:
                    target.OverrideName("Third " + target.Character);
                    target.AgentItem.OverrideID(TargetID.EtherealBarrier, agentData);
                    break;
                case (int)TargetID._EtherealBarrier4:
                    target.OverrideName("Fourth " + target.Character);
                    target.AgentItem.OverrideID(TargetID.EtherealBarrier, agentData);
                    break;
                default:
                    break;
            }
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (!FindEtherealBarriers(agentData, combatData))
        {
            agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "Dummy Spirit Race", Spec.NPC, TargetID.DummyTarget, true);
            Targetless = true;
        }
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        RenameEtherealBarriersAndOverrideID(Targets, agentData);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Spirit Race";
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>()
        {
            {TargetID._EtherealBarrier1, 0 },
            {TargetID._EtherealBarrier2, 1 },
            {TargetID._EtherealBarrier3, 2 },
            {TargetID._EtherealBarrier4, 3 },
        };
    }


    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        switch (target.ID)
        {
            case (int)TargetID.EtherealBarrier:
                HealthUpdateEvent? hpZeroUpdate = log.CombatData.GetHealthUpdateEvents(target.AgentItem).FirstOrDefault(x => x.HealthPercent == 0);
                if (hpZeroUpdate != null)
                {
                    replay.Trim(replay.TimeOffsets.start, hpZeroUpdate.Time);
                }
                break;
            case (int)TargetID.WallOfGhosts:
                (long, long) lifespan = (target.FirstAware, target.LastAware);
                uint innerRadius = 400;
                uint outerRadius = 500;
                replay.Decorations.Add(new DoughnutDecoration(innerRadius, outerRadius, lifespan, Colors.Red, 0.5, new AgentConnector(target)).UsingFilled(false));
                break;
            default:
                break;
        }
    }
}
