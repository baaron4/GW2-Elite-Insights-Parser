using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIGW2API;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class NexusOfEternityConvergenceInstance : ConvergenceLogic
{
    public NexusOfEternityConvergenceInstance(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.NexusOfEternityConvergence;
        LogID |= LogIDs.ConvergenceMasks.NexusOfEternityConvergenceMask;
        Icon = InstanceIconNexusOfEternityConvergence;
        Extension = "noeconv";
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        var name = "Convergence: Nexus of Eternity";
        return name;
    }
    internal static void AdjustVloxHP(SingleActor vloxx, bool phased)
    {
        vloxx.SetManualHealth(24085950, new List<(int hpValue, double percent)>()
        {
            (222495200, 100),
            (227243568, 70)
        });
        vloxx.SetHealthBars([
           (100, 70, 222495200, !phased),
           (70, 0, 227243568, phased),
        ]);
    }

    internal static void MergeVloxxes(IReadOnlyList<AgentItem> vloxxes, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions, EvtcVersionEvent evtcVersion)
    {
        if (vloxxes.Count == 0)
        {
            return;
        }
        var firstVloxx = vloxxes[0];
        firstVloxx.AddMergeFrom(firstVloxx, firstVloxx.FirstAware, firstVloxx.LastAware);
        for (var i = 1; i < vloxxes.Count; i++)
        {
            var curVloxx = vloxxes[i];
            firstVloxx.OverrideAwareTimes(firstVloxx.FirstAware, curVloxx.LastAware);
            AgentManipulationHelper.RedirectAllEvents(combatData, extensions, agentData, curVloxx, firstVloxx);
            curVloxx.OverrideID(TargetID.Unknown, agentData);
        }
        var firstDeterminedApply = combatData.FirstOrDefault(x => x.IsBuffApplyEvent() && x.SkillID == Determined762 && x.DstMatchesAgent(firstVloxx));
        if (firstDeterminedApply != null)
        {
            var vloxxHPUpdates = combatData.Where(x => x.SrcMatchesAgent(firstVloxx) && x.IsStateChange == StateChange.HealthUpdate && HealthUpdateEvent.GetHealthPercent(x) > 95 && x.Time > firstDeterminedApply.Time).ToList();
            foreach (var evt in vloxxHPUpdates)
            {
                evt.OverrideSrcAgent(ParserHelper._unknownAgent);
            }
        }
    }



    internal static void RenameAdds(IReadOnlyList<SingleActor> actors)
    {
        foreach (SingleActor actor in actors)
        {
            switch (actor.ID)
            {
                case (int)TargetID.ScarabQueen:
                    actor.OverrideName("Champion " + actor.Character + " Queen");
                    break;
                case (int)TargetID.ChampionWaterElemental:
                case (int)TargetID.ChampionIceElemental:
                case (int)TargetID.ChampionAatxe:
                case (int)TargetID.ChampionCosmicBulwark:
                case (int)TargetID.ChampionCosmicPiercer:
                case (int)TargetID.ChampionCosmicSunderer:
                    actor.OverrideName("Champion " + actor.Character);
                    break;
                case (int)TargetID.EliteCosmicBulwark:
                case (int)TargetID.EliteCosmicPiercer:
                    actor.OverrideName("Elite " + actor.Character);
                    break;
            }
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        MergeVloxxes(agentData.GetStableSpeciesByID(TargetID.VloxxConv), agentData, combatData, extensions, evtcVersion);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        var vloxx = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.VloxxConv)) ?? throw new MissingKeyActorsException("Vloxx not found");
        AdjustVloxHP(vloxx, vloxx.AgentItem.Merges.Count > 0);
        RenameAdds(TrashMobs);
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations, CombatReplayMap? parentMap = null)
    {
        return base.GetCombatMapInternal(log, arenaDecorations, parentMap);
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.VloxxConv,
            TargetID.ChampionIceElemental,
            TargetID.ChampionWaterElemental,
            TargetID.Megadestroyer,
            TargetID.ScarabQueen,
            TargetID.ChampionAatxe,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return [
            TargetID.CosmicBulwark1,
            // Center
            TargetID.ChampionCosmicBulwark,
            TargetID.EliteCosmicBulwark,
            TargetID.CosmicBulwark4,
            TargetID.CosmicPiercer1,
            TargetID.ChampionCosmicPiercer,
            TargetID.EliteCosmicPiercer,
            TargetID.SomethingCosmicPiercer,
            TargetID.CosmicPiercer5,
            TargetID.CosmicSunderer1,
            TargetID.CosmicSunderer2,
            TargetID.CosmicSunderer3,
            // Scarab
            TargetID.ScarabSwarmConv,
            // Underworld
            TargetID.FleshReaver,
            TargetID.ShadowImp,
            TargetID.ShadeConv,
            TargetID.Aatxe,
            // Ice
            TargetID.IceElemental2,
            TargetID.IceElemental3,
            // Dominion
            TargetID.DominionEngineer,
            TargetID.DominionIgniter,
            TargetID.DominionDuelist,
            TargetID.DominionSentinel,
            TargetID.BloodLegionBlademaster2,
            TargetID.DominionBlademaster,
            TargetID.DominionSentinel2,
            TargetID.DominionEngineer2,
            TargetID.DominionSolider,
            TargetID.DominionFireSHamar,
            TargetID.DominionSoldier2,
            TargetID.DominionDuelist2,
            TargetID.BloodLegionDuelist,
            TargetID.DominionShadow,
            TargetID.DominionScout,
            TargetID.DominionBlademaster2,
            TargetID.DominionSoldier3,
            TargetID.DominionMarksman,
            TargetID.BloodLegionSoldier,
            TargetID.BloodLegionDuelist3,
            TargetID.DominionSentinel3,
            TargetID.DominionSmokeShaman,
            TargetID.DominionSpy,
            TargetID.DominionSoldier,
            TargetID.DominionMarksman2,
            TargetID.DominionBladestorm2,
            TargetID.DominionBlademaster3,
            TargetID.FrostLegionShaman,
            // Destroyers
            TargetID.DetroyerTroll,
            TargetID.DetroyerHarpy,
            TargetID.DestroyerCrab,
            TargetID.DestroyerTroll2,
            TargetID.DestroyerCrabling,
            TargetID.DestroyerHarpy2,
            TargetID.DestroyerCrab2,
            // Titans
            TargetID.RotStrider,
            TargetID.SentientConduit,
            TargetID.LucidBoulder,
            TargetID.RotStrider2 ,
            TargetID.BlightedBeast,
            TargetID.RotStrider3,
            TargetID.LucidBoulder2,
            TargetID.SentientConduit2,
            TargetID.RotStrider4,
            TargetID.BlightedBeast2,
            TargetID.LucidBoulder3,
            TargetID.BlightedBeast3,
            TargetID.LiminalConduit,
            TargetID.Ventshot,
            TargetID.Fumaroller,
        ];
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        return
        [
        ];
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.Mode.Normal;
    }

    internal override LogData.InstancePrivacyMode GetInstancePrivacyMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return combatData.GetMapIDEvent()?.MapID == MapIDs.NexusOfEternityPublicConvergence ? LogData.InstancePrivacyMode.Public : LogData.InstancePrivacyMode.Private;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        var phases = GetInitialPhase(log);
        var instancePhase = (InstancePhaseData)phases[0];

        var vloxx = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.VloxxConv)) ?? throw new MissingKeyActorsException("Vloxx not found");
        instancePhase.AddTarget(vloxx, log);
        var determinedApplies = log.CombatData.GetBuffApplyDataByIDByDst(Determined762, vloxx.AgentItem);
        var determinedRemoves = log.CombatData.GetBuffRemoveAllDataByIDByDst(Determined762, vloxx.AgentItem);
        var fullEnd = vloxx.LastAware;
        if (determinedApplies.Count > 0)
        {
            var lastRemove = determinedRemoves.LastOrDefault();
            if (lastRemove != null)
            {
                var lastApply = determinedApplies.FirstOrDefault(x => x.Time >= lastRemove.Time);
                if (lastApply != null)
                {
                    fullEnd = lastApply.Time;
                }
            }
        }
        var fullPhase = log.LogData.CreateEncounterPhase(Math.Max(log.LogData.LogStart, vloxx.FirstAware), Math.Min(fullEnd, log.LogData.LogEnd), "Full Vloxx", EncounterIconNexusOfEternity).WithParentPhase(instancePhase);
        fullPhase.AddTarget(vloxx, log);
        phases.Add(fullPhase);

        var blockingTargets = Targets.Where(x => !x.IsAnySpecies([TargetID.VloxxConv, TargetID.Instance])).ToList();
        instancePhase.AddTargets(blockingTargets, log, PhaseData.TargetPriority.Blocking);
        // TODO detect rift entering to create encounter phases + add specific blocking targets to them

        if (!requirePhases)
        {
            return phases;
        }
        if (vloxx.AgentItem.Merges.Count > 0)
        {
            var prevMergeStart = long.MinValue;
            var prevStart = long.MinValue;
            foreach (var determinedApply in determinedApplies)
            {
                var subVloxx = vloxx.AgentItem.Merges.FirstOrNull((in AgentItem.MergedAgentItem x) => x.MergeEnd + ParserHelper.ServerDelayConstant >= determinedApply.Time);
                if (subVloxx == null)
                {
                    break;
                }
                var start = Math.Max(subVloxx.Value.MergeStart, fullPhase.Start);
                if (prevMergeStart == start)
                {
                    var remove = determinedRemoves.LastOrDefault(x => x.Time >= start && x.Time <= determinedApply.Time);
                    if (remove == null)
                    {
                        break;
                    }
                    start = Math.Max(remove.Time, fullPhase.Start);
                    if (start == prevStart)
                    {
                        break;
                    }
                }
                var end = Math.Min(determinedApply.Time, fullPhase.End);
                var subPhaseData = new SubPhasePhaseData(start, end).WithParentPhase(fullPhase);
                var hpUpdate = vloxx.GetCurrentHealthPercent(log, subPhaseData.Start + subPhaseData.DurationInMS / 2);
                string phaseName = "";
                switch (hpUpdate)
                {
                    case >= 70:
                        phaseName = "100% - 70%";
                        break;
                    case >= 40:
                        phaseName = "70% - 40%";
                        break;
                    case >= 10:
                        phaseName = "40% - 10%";
                        break;
                    case >= 0:
                        phaseName = "10% - 0%";
                        break;
                }
                subPhaseData.Name = phaseName;
                subPhaseData.AddTarget(vloxx, log);
                phases.Add(subPhaseData);

                prevMergeStart = subVloxx.Value.MergeStart;
                prevStart = start;
            }
        }
        return phases;
    }
}
