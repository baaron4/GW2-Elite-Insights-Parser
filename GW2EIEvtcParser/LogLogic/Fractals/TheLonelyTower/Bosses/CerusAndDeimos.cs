using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class CerusAndDeimos : LonelyTower
{
    public CerusAndDeimos(int triggerID) : base(triggerID)
    {
        MechanicList.AddRange(
        [
        ]);
        Extension = "cerdei";
        Icon = EncounterIconCerusAndDeimos;
        LogCategoryInformation.InSubCategoryOrder = 0;
        LogID |= 0x000001;
    }

    internal override LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        SingleActor cerus = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.CerusLonelyTower)) ?? throw new MissingKeyActorsException("Cerus not found");
        SingleActor deimos = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.DeimosLonelyTower)) ?? throw new MissingKeyActorsException("Deimos not found");
        if (cerus.GetHealth(combatData) < 5e6 || deimos.GetHealth(combatData) < 5e6)
        {
            return LogData.LogMode.Normal;
        }
        return LogData.LogMode.CM;
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Cerus and Deimos";
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = base.GetLogOffset(evtcVersion, logData, agentData, combatData);
        if (evtcVersion.Build >= ArcDPSBuilds.NewLogStart)
        {
            AgentItem cerus = agentData.GetNPCsByID(TargetID.CerusLonelyTower).FirstOrDefault() ?? throw new MissingKeyActorsException("Cerus not found");
            AgentItem deimos = agentData.GetNPCsByID(TargetID.DeimosLonelyTower).FirstOrDefault() ?? throw new MissingKeyActorsException("Deimos not found");
            CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
            if (logStartNPCUpdate != null)
            {
                startToUse = Math.Min(GetEnterCombatTime(logData, agentData, combatData, logStartNPCUpdate.Time, (int)TargetID.CerusLonelyTower, logStartNPCUpdate.DstAgent),
                        GetEnterCombatTime(logData, agentData, combatData, logStartNPCUpdate.Time, (int)TargetID.DeimosLonelyTower, logStartNPCUpdate.DstAgent));
                return startToUse;
            }
            CombatItem? initialDamageToPlayers = combatData.Where(x => x.IsDamagingDamage() && agentData.GetAgent(x.DstAgent, x.Time).IsPlayer && (
                  agentData.GetAgent(x.SrcAgent, x.Time).Is(cerus) || agentData.GetAgent(x.SrcAgent, x.Time).Is(deimos))).FirstOrDefault();
            long initialDamageTimeToTargets = Math.Min(GetFirstDamageEventTime(logData, agentData, combatData, cerus), GetFirstDamageEventTime(logData, agentData, combatData, deimos));
            if (initialDamageToPlayers != null)
            {
                return Math.Min(initialDamageToPlayers.Time, initialDamageTimeToTargets);
            }
            return initialDamageTimeToTargets;
        }
        return startToUse;
    }

    protected override IReadOnlyList<TargetID> GetSuccessCheckIDs()
    {
        return [TargetID.CerusLonelyTower, TargetID.DeimosLonelyTower];
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        SingleActor deimos = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.DeimosLonelyTower)) ?? throw new MissingKeyActorsException("Deimos not found");
        SingleActor cerus = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.CerusLonelyTower)) ?? throw new MissingKeyActorsException("Cerus not found");
        BuffApplyEvent? determinedApplyCerus = combatData.GetBuffApplyDataByIDByDst(Determined762, cerus.AgentItem).OfType<BuffApplyEvent>().LastOrDefault();
        BuffApplyEvent? determinedApplyDeimos = combatData.GetBuffApplyDataByIDByDst(Determined762, deimos.AgentItem).OfType<BuffApplyEvent>().LastOrDefault();
        if (determinedApplyCerus != null && determinedApplyDeimos != null)
        {
            logData.SetSuccess(true, Math.Max(determinedApplyCerus.Time, determinedApplyDeimos.Time));
        } 
        else
        {
            logData.SetSuccess(false, Math.Max(deimos.LastAware, cerus.LastAware));
        }
    }

    internal override LogData.LogStartStatus GetLogStartStatus(CombatData combatData, AgentData agentData, LogData logData)
    {
        if (TargetHPPercentUnderThreshold(TargetID.CerusLonelyTower, logData.LogStart, combatData, Targets))
        {
            return LogData.LogStartStatus.Late;
        }
        if (TargetHPPercentUnderThreshold(TargetID.DeimosLonelyTower, logData.LogStart, combatData, Targets))
        {
            return LogData.LogStartStatus.Late;
        }
        return LogData.LogStartStatus.Normal;
    }

    private static PhaseData GetBossPhase(ParsedEvtcLog log, SingleActor target, string phaseName)
    {
        BuffApplyEvent? determinedApply = log.CombatData.GetBuffApplyDataByIDByDst(Determined762, target.AgentItem).OfType<BuffApplyEvent>().LastOrDefault();
        long end = determinedApply != null ? determinedApply.Time : target.LastAware;
        var bossPhase = new SubPhasePhaseData(log.LogData.LogStart, end, phaseName);
        bossPhase.AddTarget(target, log);
        return bossPhase;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor cerus = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.CerusLonelyTower)) ?? throw new MissingKeyActorsException("Cerus not found");
        SingleActor deimos = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.DeimosLonelyTower)) ?? throw new MissingKeyActorsException("Deimos not found");
        phases[0].AddTarget(cerus, log);
        phases[0].AddTarget(deimos, log);
        if (!requirePhases)
        {
            return phases;
        }
        phases.Add(GetBossPhase(log, cerus, "Cerus"));
        phases.Add(GetBossPhase(log, deimos, "Deimos"));
        for (var i = 1; i < phases.Count; i++)
        {
            phases[i].AddParentPhase(phases[0]);
        }
        return phases;
    }

    private static void DoBrotherTether(ParsedEvtcLog log, SingleActor target, SingleActor? brother, CombatReplay replay)
    {
        if (brother != null)
        {
            var brothers = target.GetBuffStatus(log, BrothersUnited).Where(x => x.Value > 0);
            foreach (var seg in brothers)
            {
                replay.Decorations.Add(new LineDecoration(seg, Colors.LightBlue, 0.5, new AgentConnector(target), new AgentConnector(brother)));
            }
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
            case (int)TargetID.DeimosLonelyTower:
                SingleActor? cerus = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.CerusLonelyTower));
                DoBrotherTether(log, target, cerus, replay);
                break;
            case (int)TargetID.CerusLonelyTower:
                SingleActor? deimos = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.DeimosLonelyTower));
                DoBrotherTether(log, target, deimos, replay);
                break;
            default:
                break;
        }
    }

    private static void DoFixationTether(ParsedEvtcLog log, PlayerActor p, CombatReplay replay, SingleActor? target, long fixationID, Color color)
    {
        if (target != null)
        {
            var fixated = p.GetBuffStatus(log, fixationID).Where(x => x.Value > 0);
            foreach (var seg in fixated)
            {
                replay.Decorations.Add(new LineDecoration(seg, color, 0.3, new AgentConnector(p), new AgentConnector(target)));
                replay.Decorations.AddOverheadIcon(seg, p, ParserIcons.FixationPurpleOverhead);
            }
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
        SingleActor? cerus = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.CerusLonelyTower));
        DoFixationTether(log, p, replay, cerus, CerussFocus, Colors.Orange);
        SingleActor? deimos = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.DeimosLonelyTower));
        DoFixationTether(log, p, replay, deimos, DeimossFocus, Colors.Red);
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.CerusLonelyTower,
            TargetID.DeimosLonelyTower,
        ];
    }
    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>()
        {
            {TargetID.CerusLonelyTower, 0 },
            {TargetID.DeimosLonelyTower, 0 },
        };
    }
}
