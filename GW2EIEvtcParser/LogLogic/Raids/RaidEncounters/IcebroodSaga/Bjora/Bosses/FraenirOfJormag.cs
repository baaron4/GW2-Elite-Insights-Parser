using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class FraenirOfJormag : Bjora
{
    public FraenirOfJormag(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([      
            new PlayerDstHealthDamageHitMechanic(Icequake, new MechanicPlotlySetting(Symbols.Hexagram, Colors.Red), "Icequake", "Knocked by Icequake", "Icequake", 4000)
                .UsingBuffChecker(Stability, false),
            new PlayerDstHealthDamageHitMechanic(IceShockWaveFraenir, new MechanicPlotlySetting(Symbols.Square, Colors.Red), "Ice Shock Wave", "Knocked by Ice Shock Wave", "Ice Shock Wave", 4000)
                .UsingBuffChecker(Stability, false),
            new PlayerDstHealthDamageHitMechanic(IceArmSwingFraenir, new MechanicPlotlySetting(Symbols.Pentagon, Colors.Orange), "IceArmSwing.CC", "Knocked by Ice Arm Swing", "Ice Arm Swing", 4000)
                .UsingBuffChecker(Stability, false),
            new MechanicGroup([          
                new PlayerDstHealthDamageHitMechanic(FrozenMissile, new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.Orange), "FrozenMissile.CC", "Launched by Frozen Missile", "Frozen Missile", 4000)
                    .UsingBuffChecker(Stability, false),
                new EnemyCastStartMechanic(FrozenMissile, new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.LightOrange), "Frozen Missile", "Cast Frozen Missile", "Frozen Missile", 4000),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(SeismicCrush, new MechanicPlotlySetting(Symbols.Circle, Colors.Orange), "SeismicCrush.CC", "Knocked by Seismic Crush", "Seismic Crush", 4000)
                    .UsingBuffChecker(Stability, false),
                new EnemyCastStartMechanic(SeismicCrush, new MechanicPlotlySetting(Symbols.Square, Colors.Purple), "Seismic Crush (Breakbar)", "Cast Seismic Crush & Breakbar", "Seismic Crush", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(FrigidFusillade, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Teal), "FrigidFusillade.H", "Hit by Frigid Fusillade (Fraenir Arrows)", "Frigid Fusillade", 0),
                new EnemyCastStartMechanic(FrigidFusillade, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkTeal), "Frigid Fusillade", "Cast Frigid Fusillade", "Frigid Fusillade", 0),
            ]),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(Frozen, new MechanicPlotlySetting(Symbols.Circle, Colors.Blue), "Frozen", "Frozen", "Frozen", 500),
                new PlayerDstBuffRemoveMechanic(Frozen, new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Blue), "Unfrozen", "Unfrozen", "Unfrozen", 500),
            ]),
            new PlayerDstBuffApplyMechanic(Snowblind, new MechanicPlotlySetting(Symbols.Square, Colors.Blue), "Snowblind", "Snowblind", "Snowblind", 500),
        ])
        );
        Extension = "fraenir";
        Icon = EncounterIconFraenirOfJormag;
        LogCategoryInformation.InSubCategoryOrder = 0;
        LogID |= 0x000002;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations, CombatReplayMap? parentMap = null)
    {
        var crMap = new CombatReplayMap(
                        (905, 789),
                        (-833, -1530, 2401, 1306));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayFraenirOfJormag, crMap, parentMap);
        return crMap;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(FrostbiteAuraFraenir, FrostbiteAuraFraenir),
        ];
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor fraenir = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.FraenirOfJormag)) ?? throw new MissingKeyActorsException("Fraenir of Jormag not found");
        phases[0].AddTarget(fraenir, log);
        SingleActor? icebrood = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.IcebroodConstructFraenir));
        if (icebrood != null)
        {
            phases[0].AddTarget(icebrood, log);
        }
        if (!requirePhases)
        {
            return phases;
        }
        BuffEvent? invulApplyFraenir = log.CombatData.GetBuffDataByIDByDst(Determined762, fraenir.AgentItem).FirstOrDefault(x => x is BuffApplyEvent);
        if (invulApplyFraenir != null)
        {
            // split happened
            phases.Add(new SubPhasePhaseData(0, invulApplyFraenir.Time, "Fraenir 100-75%").WithParentPhase(phases[0]));
            if (icebrood != null)
            {
                // icebrood enters combat
                EnterCombatEvent? enterCombatIce = log.CombatData.GetEnterCombatEvents(icebrood.AgentItem).LastOrDefault();
                if (enterCombatIce != null)
                {
                    var icebroodPhases = new List<PhaseData>(2);
                    // icebrood phasing
                    BuffEvent? invulApplyIce = log.CombatData.GetBuffDataByIDByDst(Invulnerability757, icebrood.AgentItem).FirstOrDefault(x => x is BuffApplyEvent);
                    BuffEvent? invulRemoveIce = log.CombatData.GetBuffDataByIDByDst(Invulnerability757, icebrood.AgentItem).FirstOrDefault(x => x is BuffRemoveAllEvent);
                    long icebroodStart = enterCombatIce.Time;
                    long icebroodEnd = log.LogData.LogEnd;
                    if (invulApplyIce != null && invulRemoveIce != null)
                    {
                        long icebrood2Start = invulRemoveIce.Time;
                        phases.Add(new SubPhasePhaseData(icebroodStart + 1, invulApplyIce.Time, "Construct Intact"));
                        icebroodPhases.Add(phases[^1]);
                        BuffEvent? invulRemoveFraenir = log.CombatData.GetBuffDataByIDByDst(Determined762, fraenir.AgentItem).FirstOrDefault(x => x is BuffRemoveAllEvent);
                        if (invulRemoveFraenir != null)
                        {
                            // fraenir came back
                            DeadEvent? deadIce = log.CombatData.GetDeadEvents(icebrood.AgentItem).LastOrDefault();
                            if (deadIce != null)
                            {
                                icebroodEnd = deadIce.Time;
                            }
                            else
                            {
                                icebroodEnd = invulRemoveFraenir.Time - 1;
                            }
                            phases.Add(new SubPhasePhaseData(invulRemoveFraenir.Time, log.LogData.LogEnd, "Fraenir 25-0%").WithParentPhase(phases[0]));
                        }
                        phases.Add(new SubPhasePhaseData(icebrood2Start, icebroodEnd, "Damaged Construct & Fraenir"));
                        icebroodPhases.Add(phases[^1]);
                    }
                    phases.Add(new SubPhasePhaseData(icebroodStart, icebroodEnd, "Icebrood Construct").WithParentPhase(phases[0]));
                    foreach (var icebroodPhase in icebroodPhases)
                    {
                        icebroodPhase.AddParentPhase(phases[^1]);
                    }
                }
            }
        }
        phases.Sort((x, y) => x.Start.CompareTo(y.Start));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            if (i == 1 || i == 5)
            {
                phase.AddTarget(fraenir, log);
            }
            else
            {
                phase.AddTarget(icebrood, log);
            }
        }
        return phases;
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.FraenirOfJormag,
            TargetID.IcebroodConstructFraenir,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.IcebroodElemental,
            TargetID.BoundIcebroodElemental,
        ];
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var boundElementals = combatData
            .Where(x => x.IsBuffApplyEvent() && x.SkillID == BoundIceElemental)
            .Select(x => agentData.GetAgent(x.DstAgent, x.Time))
            .ToHashSet();
        var spawnedElementals = agentData.GetStableSpeciesByID(TargetID.IcebroodElemental).ToHashSet();

        var positionEvents = combatData
            .Where(x => x.IsStateChange == StateChange.Position)
            .GroupBy(x => agentData.GetAgent(x.SrcAgent, x.Time))
            .Where(x => boundElementals.Contains(x.Key) || x.Key.IsSpecies(TargetID.IcebroodElemental))
            .ToDictionary(x => x.Key, x => x.ToList());
        var healthUpdateEvents = combatData
            .Where(x => x.IsStateChange == StateChange.HealthUpdate)
            .GroupBy(x => agentData.GetAgent(x.SrcAgent, x.Time))
            .Where(x => boundElementals.Contains(x.Key))
            .ToDictionary(x => x.Key, x => x.ToList());

        foreach (AgentItem boundElemental in boundElementals)
        {
            boundElemental.OverrideID(TargetID.BoundIcebroodElemental, agentData);
            var killed = false;
            // If a Bound Icebrood Elemental gets killed, the log contains a Health update event of 0
            if (healthUpdateEvents.TryGetValue(boundElemental, out var hpUpdates))
            {
                var boundElementalKilled = hpUpdates.Where(x => HealthUpdateEvent.GetHealthPercent(x) == 0).FirstOrDefault();
                if (boundElementalKilled != null)
                {
                    killed = true;
                    boundElemental.OverrideAwareTimes(boundElemental.FirstAware, boundElementalKilled.Time);
                }
            }
            if (!killed && positionEvents.TryGetValue(boundElemental, out var positions))
            {
                // If the Bound Icebrood Elemental hatches, an Icebrood Elemental spawns
                // Due to the randomness of the time to hatch, we check the Elemental spawn position to match the Bound one
                // When they match, override the Bound's LastAware to the Elemental's FirstAware
                foreach (AgentItem spawnedElemental in spawnedElementals)
                {
                    CombatItem? itemBound = positions.FirstOrDefault();
                    if (itemBound != null && positionEvents.TryGetValue(spawnedElemental, out var spawnedPositions))
                    {
                        CombatItem? itemElem = spawnedPositions.FirstOrDefault();
                        if (itemElem != null)
                        {
                            var bound3D = MovementEvent.GetPoint3D(itemBound);
                            var elem3D = MovementEvent.GetPoint3D(itemElem);
                            if ((bound3D - elem3D).XY().LengthSquared() < 1)
                            {
                                boundElemental.OverrideAwareTimes(boundElemental.FirstAware, spawnedElemental.FirstAware);
                                spawnedElementals.Remove(spawnedElemental);
                                break;
                            }
                        }
                    }
                }
            }
        }
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
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
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }
    }

    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
        if (log.CombatData.GetBuffData(AchievementEligibilityElementalElegy).Any())
        {
            var encounterPhases = log.LogData.GetEncounterPhases(log, LogID);
            foreach (var encounterPhase in encounterPhases)
            {
                if (encounterPhase.Success)
                {
                    instanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, encounterPhase, AchievementEligibilityElementalElegy));
                }
            }
        }
    }
}
