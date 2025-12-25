using System.Linq;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Escort : StrongholdOfTheFaithful
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new PlayerDstHealthDamageMechanic(DetonateMineEscort, new MechanicPlotlySetting(Symbols.CircleCross, Colors.Red), "Mine.H", "Hit by Mine Detonation", "Mine Detonation Hit", 150).UsingChecker((de, log) => de.CreditedFrom.IsSpecies(TargetID.Mine)),
            new PlayerDstHealthDamageMechanic(GlennaBombHit, new MechanicPlotlySetting(Symbols.Hexagon, Colors.LightGrey), "Bomb.H", "Hit by Glenna's Bomb", "Glenna's Bomb Hit", 0),
            new PlayerDstHealthDamageHitMechanic(FireMortarEscortHit, new MechanicPlotlySetting(Symbols.Hourglass, Colors.DarkPurple), "Shrd.H", "Hit by Mortar Fire (Bloodstone Turrets)", "Mortar Fire Hit", 0),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(RadiantAttunementPhantasm, new MechanicPlotlySetting(Symbols.Diamond, Colors.White), "Rad.A", "Radiant Attunement Application", "Radiant Attunement Application", 150),
                new PlayerDstBuffApplyMechanic(CrimsonAttunementPhantasm, new MechanicPlotlySetting(Symbols.Diamond, Colors.Red), "Crim.A", "Crimson Attunement Application", "Crimson Attunement Application", 150),
            ]),
            new PlayerSrcEffectMechanic(EffectGUIDs.EscortOverHere, new MechanicPlotlySetting(Symbols.Star, Colors.White), "OverHere.C", "Used Over Here! (Special Action Key)", "Over Here! Cast", 0),
            new EnemyDstBuffApplyMechanic(Invulnerability757, new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.LightBlue), "Inv.A", "Invulnerability Applied", "Invulnerability Applied", 150),
            new EnemyCastStartMechanic(TeleportDisplacementField, new MechanicPlotlySetting(Symbols.Square, Colors.LightPurple), "Tel.C", "Teleport Cast", "Teleport Cast", 150),
        ]);

    public Escort(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        ChestID = ChestID.SiegeChest;
        Extension = "escort";
        Icon = EncounterIconEscort;
        LogCategoryInformation.InSubCategoryOrder = 0;
        GenericFallBackMethod = FallBackMethod.None;
        LogID |= 0x000001;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (1080, 676),
                        (-6081.86, 13624.72, 8956.86, 23099.28));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayEscort, crMap);
        return crMap;
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Siege the Stronghold";
    }

    private static IReadOnlyList<PhaseData> GetMcLeodPhases(SingleActor mcLeod, IReadOnlyList<SingleActor> targets, ParsedEvtcLog log, EncounterPhaseData encounterPhase)
    {
        var phases = new List<PhaseData>();
        //
        DeadEvent? mcLeodDeath = log.CombatData.GetDeadEvents(mcLeod.AgentItem).LastOrDefault();
        long mcLeodStart = Math.Max(mcLeod.FirstAware, encounterPhase.Start);
        long mcLeodEnd = Math.Min(mcLeodDeath != null ? mcLeodDeath.Time : mcLeod.LastAware, encounterPhase.End);
        var mainPhase = new SubPhasePhaseData(mcLeodStart, mcLeodEnd)
        {
            Name = "McLeod The Silent"
        };
        mainPhase.AddTarget(mcLeod, log);
        phases.Add(mainPhase);
        //
        phases.AddRange(GetPhasesByInvul(log, Invulnerability757, mcLeod, true, true, mcLeodStart, mcLeodEnd));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.AddParentPhase(mainPhase);
            if (i % 2 == 0)
            {
                phase.Name = "McLeod Split " + (i) / 2;
                phase.AddTargets(targets.Where(x => x.IsAnySpecies([TargetID.RadiantMcLeod, TargetID.CrimsonMcLeod])), log);
                phase.OverrideTimes(log);
            }
            else
            {
                phase.Name = "McLeod Phase " + (i + 1) / 2;
                phase.AddTarget(mcLeod, log);
            }
        }
        //
        return phases;
    }

    internal static List<PhaseData> ComputePhases(ParsedEvtcLog log, SingleActor? mcLeod, IReadOnlyList<SingleActor> targets, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = new List<PhaseData>(9);
        var wargs = targets.Where(x => x.IsSpecies(TargetID.WargBloodhound));
        {
            long preEventEnd = mcLeod != null ? mcLeod.FirstAware : encounterPhase.End;
            var preEventWargs = wargs.Where(x => x.FirstAware <= preEventEnd);
            var preEventPhase = new SubPhasePhaseData(encounterPhase.Start, preEventEnd)
            {
                Name = "Escort",
            };
            preEventPhase.AddTargets(preEventWargs, log);
            preEventPhase.AddParentPhase(encounterPhase);
            if (preEventPhase.Targets.Count == 0)
            {
                preEventPhase.AddTarget(targets.FirstOrDefault(x => x.ID == (int)TargetID.DummyTarget && x.Character == "Escort"), log);
            }
            phases.Add(preEventPhase);
        }
        if (mcLeod != null)
        {
            var mcLeodPhases = GetMcLeodPhases(mcLeod, targets, log, encounterPhase);
            foreach (var mcLeodPhase in mcLeodPhases)
            {
                mcLeodPhase.AddParentPhase(encounterPhase);
            }
            phases.AddRange(mcLeodPhases);
            var mcLeodWargs = wargs.Where(x => x.FirstAware >= mcLeod.FirstAware && x.FirstAware <= mcLeod.LastAware);
            if (mcLeodWargs.Any())
            {
                var phase = new SubPhasePhaseData(encounterPhase.Start, encounterPhase.End, "McLeod Wargs");
                phase.AddTargets(mcLeodWargs, log);
                phase.OverrideTimes(log);
                phases.Add(phase);
            }
        }
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mcLeod = Targets.FirstOrDefault(x => x.ID == (int)TargetID.McLeodTheSilent) ?? throw new MissingKeyActorsException("McLeod not found");
        phases[0].AddTarget(mcLeod, log);
        phases.AddRange(ComputePhases(log, mcLeod, Targets, (EncounterPhaseData)phases[0], requirePhases));
        return phases;
    }

    internal static void FindMines(AgentData agentData, List<CombatItem> combatData)
    {
        var mineAgents = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 1494 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth == 100 && x.HitboxHeight == 300);
        foreach (AgentItem mine in mineAgents)
        {
            mine.OverrideID(TargetID.Mine, agentData);
            mine.OverrideType(AgentItem.AgentType.NPC, agentData);
        }
    }

    internal static void RenameSubMcLeods(IReadOnlyList<SingleActor> targets)
    {
        foreach (SingleActor target in targets)
        {
            if (target.IsSpecies(TargetID.CrimsonMcLeod))
            {
                target.OverrideName("Crimson " + target.Character);
            }
            if (target.IsSpecies(TargetID.RadiantMcLeod))
            {
                target.OverrideName("Radiant " + target.Character);
            }
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (!agentData.TryGetFirstAgentItem(TargetID.McLeodTheSilent, out var mcLeod))
        {
            throw new MissingKeyActorsException("McLeod not found");
        }
        //
        FindMines(agentData, combatData);
        var duplicateGlennaPosition = new Vector3(-4326.979f, 13687.298f, -5561.857f);
        foreach (var glenna in agentData.GetNPCsByID(TargetID.Glenna))
        {
            var positions = combatData.Where(x => x.IsStateChange == StateChange.Position && x.SrcMatchesAgent(glenna)).Take(5).Select(x => new PositionEvent(x, agentData).GetParametricPoint3D());
            if (positions.Any(x => (duplicateGlennaPosition.XY() - x.XYZ.XY()).LengthSquared() < 10))
            {
                glenna.OverrideID(IgnoredSpecies, agentData);
            }
        }
        // to keep the pre event as we need targets
        if (!agentData.GetNPCsByID(TargetID.WargBloodhound).Any(x => x.FirstAware < mcLeod.FirstAware))
        {
            agentData.AddCustomNPCAgent(logData.LogStart, logData.LogEnd, "Escort", Spec.NPC, TargetID.DummyTarget, true);
        }
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        RenameSubMcLeods(Targets);
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        if (!agentData.TryGetFirstAgentItem(TargetID.McLeodTheSilent, out var mcLeod))
        {
            throw new MissingKeyActorsException("McLeod not found");
        }
        long startToUse = GetGenericLogOffset(logData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            if (mcLeod.FirstAware - logData.EvtcLogStart > MinimumInCombatDuration)
            {
                // Is this reliable?
                /*CombatItem achievementTrackApply = combatData.Where(x => (x.SkillID == AchievementEligibilityMineControl || x.SkillID == AchievementEligibilityFastSiege) && x.IsBuffApply()).FirstOrDefault();
                if (achievementTrackApply != null)
                {
                    startToUse = achievementTrackApply.Time;
                }*/
            }
            else
            {
                startToUse = GetEnterCombatTime(logData, agentData, combatData, logStartNPCUpdate.Time, (int)TargetID.McLeodTheSilent, logStartNPCUpdate.DstAgent);
            }
        }
        return startToUse;
    }

    internal override LogData.LogStartStatus GetLogStartStatus(CombatData combatData, AgentData agentData, LogData logData)
    {
        if (!agentData.TryGetFirstAgentItem(TargetID.McLeodTheSilent, out var mcLeod))
        {
            throw new MissingKeyActorsException("McLeod not found");
        }
        if (mcLeod.FirstAware - logData.EvtcLogStart > MinimumInCombatDuration)
        {
            if (!agentData.TryGetFirstAgentItem(TargetID.Glenna, out var glenna))
            {
                throw new MissingKeyActorsException("Glenna not found");
            }
            if (combatData.HasMovementData)
            {
                var glennaInitialPosition = new Vector2(9092.697f, 21477.2969f/*, -2946.81885f*/);
                if (!combatData.GetMovementData(glenna).Any(x => x is PositionEvent pe && pe.Time < glenna.FirstAware + MinimumInCombatDuration && (pe.GetPointXY() - glennaInitialPosition).Length() < 100))
                {
                    return LogData.LogStartStatus.Late;
                }
            }
            return LogData.LogStartStatus.Normal;
        }
        else if (combatData.GetLogNPCUpdateEvents().Any())
        {
            return LogData.LogStartStatus.NoPreEvent;
        }
        else
        {
            return LogData.LogStartStatus.Normal;
        }
    }
    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.McLeodTheSilent,
            TargetID.RadiantMcLeod,
            TargetID.CrimsonMcLeod,
            TargetID.WargBloodhound,
            TargetID.DummyTarget,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return [
            TargetID.MushroomCharger,
            TargetID.MushroomKing,
            TargetID.MushroomSpikeThrower,
            TargetID.WhiteMantleBattleMage1Escort,
            TargetID.WhiteMantleBattleMage2Escort,
            TargetID.WhiteMantleBattleCultist1,
            TargetID.WhiteMantleBattleCultist2,
            TargetID.WhiteMantleBattleKnight1,
            TargetID.WhiteMantleBattleKnight2,
            TargetID.WhiteMantleBattleCleric1,
            TargetID.WhiteMantleBattleCleric2,
            TargetID.WhiteMantleBattleSeeker1,
            TargetID.WhiteMantleBattleSeeker2,
            TargetID.Mine,
        ];
    }

    internal override IReadOnlyList<TargetID>  GetFriendlyNPCIDs()
    {
        return
        [
            TargetID.Glenna
        ];
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
        if (log.CombatData.GetBuffData(AchievementEligibilityLoveIsBunny).Any() || log.CombatData.GetBuffData(AchievementEligibilityFastSiege).Any())
        {
            var encounterPhases = log.LogData.GetPhases(log).OfType<EncounterPhaseData>().Where(x => x.LogID == LogID);
            foreach (var encounterPhase in encounterPhases)
            {
                if (encounterPhase.Success)
                {
                    instanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, encounterPhase, AchievementEligibilityLoveIsBunny));
                    instanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, encounterPhase, AchievementEligibilityFastSiege));
                }
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
            case (int)TargetID.McLeodTheSilent:
                replay.AddHideByBuff(target, log, Invulnerability757);
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
        // Attunements Overhead
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, CrimsonAttunementPhantasm).Where(x => x.Value > 0), p, ParserIcons.CrimsonAttunementOverhead);
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, RadiantAttunementPhantasm).Where(x => x.Value > 0), p, ParserIcons.RadiantAttunementOverhead);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new EffectCastFinder(OverHere, EffectGUIDs.EscortOverHere),
            new DamageCastFinder(TeleportDisplacementField, TeleportDisplacementField).UsingICD(50),
        ];
    }
}

