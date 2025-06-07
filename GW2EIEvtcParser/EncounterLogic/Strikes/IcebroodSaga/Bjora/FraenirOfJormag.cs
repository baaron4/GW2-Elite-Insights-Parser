using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class FraenirOfJormag : Bjora
{
    public FraenirOfJormag(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([      
            new PlayerDstHitMechanic(Icequake, new MechanicPlotlySetting(Symbols.Hexagram, Colors.Red), "Icequake", "Knocked by Icequake", "Icequake", 4000).UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ServerDelayConstant)),
            new PlayerDstHitMechanic(IceShockWaveFraenir, new MechanicPlotlySetting(Symbols.Square, Colors.Red), "Ice Shock Wave", "Knocked by Ice Shock Wave", "Ice Shock Wave", 4000).UsingChecker( (de, log) => !de.To.HasBuff(log, Stability, de.Time - ServerDelayConstant)),
            new PlayerDstHitMechanic(IceArmSwingFraenir, new MechanicPlotlySetting(Symbols.Pentagon, Colors.Orange), "IceArmSwing.CC", "Knocked by Ice Arm Swing", "Ice Arm Swing", 4000).UsingChecker( (de, log) => !de.To.HasBuff(log, Stability, de.Time - ServerDelayConstant)),
            new MechanicGroup([          
                new PlayerDstHitMechanic(FrozenMissile, new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.Orange), "FrozenMissile.CC", "Launched by Frozen Missile", "Frozen Missile", 4000)
                .UsingChecker( (de, log) => !de.To.HasBuff(log, Stability, de.Time - ServerDelayConstant)),
                new EnemyCastStartMechanic(FrozenMissile, new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.LightOrange), "Frozen Missile", "Cast Frozen Missile", "Frozen Missile", 4000),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(SeismicCrush, new MechanicPlotlySetting(Symbols.Circle, Colors.Orange), "SeismicCrush.CC", "Knocked by Seismic Crush", "Seismic Crush", 4000)
                    .UsingChecker( (de, log) => !de.To.HasBuff(log, Stability, de.Time - ServerDelayConstant)),
                new EnemyCastStartMechanic(SeismicCrush, new MechanicPlotlySetting(Symbols.Square, Colors.Purple), "Seismic Crush (Breakbar)", "Cast Seismic Crush & Breakbar", "Seismic Crush", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(FrigidFusillade, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Teal), "FrigidFusillade.H", "Hit by Frigid Fusillade (Fraenir Arrows)", "Frigid Fusillade", 0),
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
        EncounterCategoryInformation.InSubCategoryOrder = 0;
        EncounterID |= 0x000002;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayFraenirOfJormag,
                        (905, 789),
                        (-833, -1780, 2401, 1606)/*,
                        (-0, -0, 0, 0),
                        (0, 0, 0, 0)*/);
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
        BuffEvent? invulApplyFraenir = log.CombatData.GetBuffDataByIDByDst(Determined762, fraenir.AgentItem).Where(x => x is BuffApplyEvent).FirstOrDefault();
        if (invulApplyFraenir != null)
        {
            // split happened
            phases.Add(new PhaseData(0, invulApplyFraenir.Time, "Fraenir 100-75%").WithParentPhase(phases[0]));
            if (icebrood != null)
            {
                // icebrood enters combat
                EnterCombatEvent? enterCombatIce = log.CombatData.GetEnterCombatEvents(icebrood.AgentItem).LastOrDefault();
                if (enterCombatIce != null)
                {
                    var icebroodPhases = new List<PhaseData>(2);
                    // icebrood phasing
                    BuffEvent? invulApplyIce = log.CombatData.GetBuffDataByIDByDst(Invulnerability757, icebrood.AgentItem).Where(x => x is BuffApplyEvent).FirstOrDefault();
                    BuffEvent? invulRemoveIce = log.CombatData.GetBuffDataByIDByDst(Invulnerability757, icebrood.AgentItem).Where(x => x is BuffRemoveAllEvent).FirstOrDefault();
                    long icebroodStart = enterCombatIce.Time;
                    long icebroodEnd = log.FightData.FightEnd;
                    if (invulApplyIce != null && invulRemoveIce != null)
                    {
                        long icebrood2Start = invulRemoveIce.Time;
                        phases.Add(new PhaseData(icebroodStart + 1, invulApplyIce.Time, "Construct Intact"));
                        icebroodPhases.Add(phases[^1]);
                        BuffEvent? invulRemoveFraenir = log.CombatData.GetBuffDataByIDByDst(Determined762, fraenir.AgentItem).Where(x => x is BuffRemoveAllEvent).FirstOrDefault();
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
                            phases.Add(new PhaseData(invulRemoveFraenir.Time, log.FightData.FightEnd, "Fraenir 25-0%").WithParentPhase(phases[0]));
                        }
                        phases.Add(new PhaseData(icebrood2Start, icebroodEnd, "Damaged Construct & Fraenir"));
                        icebroodPhases.Add(phases[^1]);
                    }
                    phases.Add(new PhaseData(icebroodStart, icebroodEnd, "Icebrood Construct").WithParentPhase(phases[0]));
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

    protected override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.FraenirOfJormag,
            TargetID.IcebroodConstructFraenir,
        ];
    }

    protected override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.IcebroodElemental,
            TargetID.BoundIcebroodElemental,
        ];
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var boundElementals = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 14940 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxHeight == 300 && x.HitboxWidth == 100 && x.FirstAware > 10);
        IReadOnlyList<AgentItem> spawnedElementals = agentData.GetNPCsByID(TargetID.IcebroodElemental);
        foreach (AgentItem boundElemental in boundElementals)
        {
            IEnumerable<CombatItem> boundElementalKilled = combatData.Where(x => x.SrcMatchesAgent(boundElemental) && x.IsStateChange == StateChange.HealthUpdate && HealthUpdateEvent.GetHealthPercent(x) == 0);
            boundElemental.OverrideType(AgentItem.AgentType.NPC, agentData);
            boundElemental.OverrideID(TargetID.BoundIcebroodElemental, agentData);

            // If a Bound Icebrood Elemental gets killed, the log contains a Health update event of 0
            if (boundElementalKilled.Any())
            {
                long firstAware = boundElementalKilled.Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).First().FirstAware;
                boundElemental.OverrideAwareTimes(firstAware, boundElementalKilled.First().Time);
            }
            else
            {
                // If the Bound Icebrood Elemental hatches, an Icebrood Elemental spawns
                // Due to the randomness of the time to hatch, we check the Elemental spawn position to match the Bound one
                // When they match, override the Bound's LastAware to the Elemental's FirstAware
                foreach (AgentItem spawnedElemental in spawnedElementals)
                {
                    CombatItem? itemBound = combatData.FirstOrDefault(x => x.SrcMatchesAgent(boundElemental) && x.IsStateChange == StateChange.Position);
                    CombatItem? itemElem = combatData.FirstOrDefault(x => x.SrcMatchesAgent(spawnedElemental) && x.IsStateChange == StateChange.Position);
                    if (itemBound != null && itemElem != null)
                    {
                        var bound3D = MovementEvent.GetPoint3D(itemBound);
                        var elem3D = MovementEvent.GetPoint3D(itemElem);
                        if ((bound3D - elem3D).XY().LengthSquared() < 1)
                        {
                            long firstAwareBound = boundElemental.FirstAware;
                            boundElemental.OverrideAwareTimes(firstAwareBound, spawnedElemental.FirstAware);
                        }
                    }
                }
            }
        }
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log)
    {
        base.SetInstanceBuffs(log);

        if (log.FightData.Success && log.CombatData.GetBuffData(AchievementEligibilityElementalElegy).Any())
        {
            InstanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, AchievementEligibilityElementalElegy));
        }
    }
}
