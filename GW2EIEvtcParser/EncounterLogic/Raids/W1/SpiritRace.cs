using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class SpiritRace : SpiritVale
{
    public SpiritRace(int triggerID) : base(triggerID)
    {
        MechanicList.AddRange(new List<Mechanic>
        {
            new PlayerDstHitMechanic(SpiritFog, "Spirit Fog", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Red), "SpiritFog.H", "Hit by Spirit Fog", "Spirit Fog Hit", 0),
            new PlayerDstBuffApplyMechanic(Crippled, "I Can Outrun A...Ghost", new MechanicPlotlySetting(Symbols.Diamond, Colors.Pink), "Outrun.Achiv", "Achievement Eligibility: I Can Outrun A...Ghost", "I Can Outrun A...Ghost", 0).UsingAchievementEligibility(true),
        });
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
    protected override ReadOnlySpan<int> GetTargetsIDs()
    {
        return
        [
            (int)ArcDPSEnums.TargetID.DummyTarget,
            (int)ArcDPSEnums.TargetID.EtherealBarrier,
        ];
    }

    protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
    {
        return
        [
            ArcDPSEnums.TrashID.WallOfGhosts,
            ArcDPSEnums.TrashID.AngeredSpiritSR,
            ArcDPSEnums.TrashID.AngeredSpiritSR2,
            ArcDPSEnums.TrashID.EnragedSpiritSR,
            ArcDPSEnums.TrashID.DerangedSpiritSR,
            ArcDPSEnums.TrashID.DerangedSpiritSR2,
        ];
    }

    internal override int GetTriggerID()
    {
        return (int)ArcDPSEnums.TrashID.WallOfGhosts;
    }

    protected override ReadOnlySpan<int> GetUniqueNPCIDs()
    {
        return [];
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        RewardEvent? reward = combatData.GetRewardEvents().FirstOrDefault(x => x.RewardType == ArcDPSEnums.RewardTypes.OldRaidReward2 && x.Time > fightData.FightStart);
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
        phases[0].AddTargets(Targets.Where(x => x.IsSpecies(ArcDPSEnums.TargetID.EtherealBarrier)));
        return phases;
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {

        AgentItem? wallOfGhosts = agentData.GetNPCsByID(ArcDPSEnums.TrashID.WallOfGhosts).FirstOrDefault();
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
        AgentItem? wallOfGhosts = agentData.GetNPCsByID(ArcDPSEnums.TrashID.WallOfGhosts).FirstOrDefault();
        if (wallOfGhosts != null)
        {
            foreach(var @event in combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.Velocity && x.SrcMatchesAgent(wallOfGhosts)))
            {
                if(MovementEvent.GetPointXY(@event) != default)
                {
                    //first velocity
                    return @event.Time;
                }

            }
        }
        return EncounterLogicTimeUtils.GetGenericFightOffset(fightData);
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var maxHPs = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate && MaxHealthUpdateEvent.GetMaxHealth(x) == 1494000);
        bool needsDummy = true;
        foreach (CombatItem maxHP in maxHPs)
        {
            AgentItem candidate = agentData.GetAgent(maxHP.SrcAgent, maxHP.Time);
            if (candidate.Type == AgentItem.AgentType.Gadget)
            {
                needsDummy = false;
                candidate.OverrideID(ArcDPSEnums.TargetID.EtherealBarrier, agentData);
                candidate.OverrideType(AgentItem.AgentType.NPC, agentData);
            }
        }
        if (needsDummy)
        {
            agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "Dummy Spirit Race", Spec.NPC, ArcDPSEnums.TargetID.DummyTarget, true);
            Targetless = true;
        }
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        var position1 = new Vector2(-7607.929f, -12493.7051f/*, -1112.468f*/);
        var position2 = new Vector2(-8423.886f, -9858.193f/*, -1335.1134f*/);
        var position3 = new Vector2(-9104.786f, -6910.657f/*, -2405.52222f*/);
        var position4 = new Vector2(-8552.994f, -863.6334f/*, -1416.31714f*/);
        foreach (SingleActor target in Targets)
        {
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.EtherealBarrier:
                    var posititions = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.Position && x.SrcMatchesAgent(target.AgentItem)).Select(MovementEvent.GetPointXY);
                    if (posititions.Any(x => (x - position1).Length() < 10)) {
                        target.OverrideName(target.Character + " 1" );
                    } 
                    else if (posititions.Any(x => (x - position2).Length() < 10))
                    {
                        target.OverrideName(target.Character + " 2");
                    } 
                    else if (posititions.Any(x => (x - position3).Length() < 10))
                    {
                        target.OverrideName(target.Character + " 3");
                    } 
                    else if (posititions.Any(x => (x - position4).Length() < 10))
                    {
                        target.OverrideName(target.Character + " 4");
                    }
                    break;
                default:
                    break;
            }
        }
        _targets.Sort((x, y) => string.Compare(x.Character, y.Character, StringComparison.Ordinal));
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Spirit Race";
    }


    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        switch (target.ID)
        {
            case (int)ArcDPSEnums.TargetID.EtherealBarrier:
                HealthUpdateEvent? hpZeroUpdate = log.CombatData.GetHealthUpdateEvents(target.AgentItem).FirstOrDefault(x => x.HealthPercent == 0);
                if (hpZeroUpdate != null)
                {
                    replay.Trim(replay.TimeOffsets.start, hpZeroUpdate.Time);
                }
                break;
            case (int)ArcDPSEnums.TrashID.WallOfGhosts:
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
