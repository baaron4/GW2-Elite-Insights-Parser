using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using GW2EIGW2API;

namespace GW2EIEvtcParser.LogLogic;

internal class SpiritRace : SpiritVale
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(SpiritFog, new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Red), "SpiritFog.H", "Hit by Spirit Fog", "Spirit Fog Hit", 0),
            new PlayerDstBuffApplyMechanic(Crippled, new MechanicPlotlySetting(Symbols.Diamond, Colors.Pink), "Outrun.Achiv", "Achievement Eligibility: I Can Outrun A...Ghost", "I Can Outrun A...Ghost", 0)
                .UsingAchievementEligibility(),
        ]);
    public SpiritRace(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "sprtrace";
        Icon = EncounterIconSpiritRace;
        LogCategoryInformation.InSubCategoryOrder = 1;
        LogID |= 0x000004;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (581, 1193),
                        (-11188, -13757, -4700, -436));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplaySpiritRun, crMap);
        return crMap;
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

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        RewardEvent? reward = GetOldRaidReward2Event(combatData, logData.LogStart, logData.EvtcLogEnd);
        if (reward != null)
        {
            logData.SetSuccess(true, reward.Time);
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

    internal override LogData.LogStartStatus GetLogStartStatus(CombatData combatData, AgentData agentData, LogData logData)
    {

        AgentItem? wallOfGhosts = agentData.GetNPCsByID(TargetID.WallOfGhosts).FirstOrDefault();
        if (wallOfGhosts == null)
        {
            return LogData.LogStartStatus.Late;
        }
        var position = combatData.GetMovementData(wallOfGhosts).FirstOrDefault(x => x is PositionEvent positionEvt);
        if (position != null)
        {
            var initialPosition = new Vector3(-5669.139f, -7814.589f, -1138.749f);
            return (position.GetPoint3D() - initialPosition).Length() > 10 ? LogData.LogStartStatus.Late : LogData.LogStartStatus.Normal;
        }
        // To investigate
        return LogData.LogStartStatus.Late;
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
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
        return LogLogicTimeUtils.GetGenericLogOffset(logData);
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
                var positions = combatData.Where(x => x.IsStateChange == StateChange.Position && x.SrcMatchesAgent(candidate)).Select(MovementEvent.GetPointXY).ToList();
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

    internal override void HandleCriticalAgents(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (!FindEtherealBarriers(agentData, combatData))
        {
            agentData.AddCustomNPCAgent(logData.LogStart, logData.LogEnd, "Dummy Spirit Race", Spec.NPC, TargetID.DummyTarget, true);
            Targetless = true;
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        RenameEtherealBarriersAndOverrideID(Targets, agentData);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Spirit Race";
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>
        {
            {TargetID._EtherealBarrier1, 0 },
            {TargetID._EtherealBarrier2, 1 },
            {TargetID._EtherealBarrier3, 2 },
            {TargetID._EtherealBarrier4, 3 },
        };
    }

    private static long AddHideForBarrier(NPC target, ParsedEvtcLog log, CombatReplay replay, long offset)
    {
        HealthUpdateEvent? hpZeroUpdate = log.CombatData.GetHealthUpdateEvents(target.AgentItem).FirstOrDefault(x => x.HealthPercent == 0 && x.Time > offset);
        if (hpZeroUpdate != null)
        {
            var hpRestored = log.CombatData.GetHealthUpdateEvents(target.AgentItem).FirstOrDefault(x => x.HealthPercent > 0 && x.Time > hpZeroUpdate.Time);
            if (hpRestored != null)
            {
                replay.Hidden.Add(new(hpZeroUpdate.Time, hpRestored.Time));
                return hpRestored.Time;
            }
            else
            {
                replay.Trim(replay.TimeOffsets.start, hpZeroUpdate.Time);
            }
        }
        return long.MaxValue;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
        switch (target.ID)
        {
            case (int)TargetID.EtherealBarrier:
                long encounterOffset = 0;
                while(encounterOffset != long.MaxValue)
                {
                    encounterOffset = AddHideForBarrier(target, log, replay, encounterOffset);
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
    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }
    }
    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
    }
}
