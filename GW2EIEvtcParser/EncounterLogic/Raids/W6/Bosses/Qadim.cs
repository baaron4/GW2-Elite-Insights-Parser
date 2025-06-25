using System.Diagnostics;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Qadim : MythwrightGambit
{
    private bool _manualPlatforms = true;
    public Qadim(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
            new MechanicGroup([
                new EnemyCastStartMechanic(QadimCC, new MechanicPlotlySetting(Symbols.StarDiamond,Colors.DarkTeal), "Q.CC", "Qadim CC","Qadim CC", 0),
                new EnemyCastEndMechanic(QadimCC, new MechanicPlotlySetting(Symbols.StarDiamond,Colors.DarkGreen), "Q.CCed", "Qadim Breakbar broken","Qadim CCed", 0)
                    .UsingChecker((ce, log) => ce.ActualDuration < 6500),
                new EnemyCastStartMechanic(QadimRiposte, new MechanicPlotlySetting(Symbols.StarDiamond,Colors.DarkRed), "Q.CC Fail", "Qadim Breakbar failed","Qadim CC Fail", 0),
                new PlayerDstHitMechanic(QadimRiposte, new MechanicPlotlySetting(Symbols.Circle,Colors.Magenta), "NoCC Attack", "Riposte (Attack if CC on Qadim failed)", "Riposte (No CC)", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic([ FieryDance1, FieryDance2, FieryDance3, FieryDance4 ], new MechanicPlotlySetting(Symbols.BowtieOpen,Colors.Orange), "F.Dance", "Fiery Dance (Fire running along metal edges)", "Fire on Lines", 0),
                new PlayerDstHitMechanic(SeaOfFlame, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Q.Hitbox", "Sea of Flame (Stood in Qadim Hitbox)","Qadim Hitbox AoE", 0),
                new PlayerDstHitMechanic(ShatteringImpact, new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Stun", "Shattering Impact (Stunning flame bolt)","Flame Bolt Stun", 0),
                new PlayerDstHitMechanic(FlameWave, new MechanicPlotlySetting(Symbols.StarTriangleUpOpen,Colors.Pink), "KB", "Flame Wave (Knockback Frontal Beam)","KB Push", 0),
                new PlayerDstHitMechanic(FireWaveQadim, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Orange), "Q.Wave", "Fire Wave (Shockwave after Qadim's Mace attack)","Mace Shockwave", 0),
                new PlayerDstHitMechanic(BigHit, new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Mace", "Big Hit (Mace Impact)","Mace Impact", 0),
                new PlayerDstHitMechanic(Inferno, new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Red), "Inf.", "Inferno (Lava Pool drop  on long platform spokes)","Inferno Pool", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(ElementalBreath, new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Red), "Hydra Breath", "Elemental Breath (Hydra Breath)","Hydra Breath", 0),
                new PlayerDstHitMechanic(Fireball, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow,10), "H.FBall", "Fireball (Hydra)","Hydra Fireball", 0),
                new MechanicGroup([
                    new PlayerDstHitMechanic(FieryMeteor, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Pink), "H.Meteor", "Fiery Meteor (Hydra)","Hydra Meteor", 0),
                    new EnemyCastStartMechanic(FieryMeteor, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "H.CC", "Fiery Meteor (Hydra Breakbar)","Hydra CC", 0),
                    //new Mechanic(718, "Fiery Meteor (Spawn)", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Qadim, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkRed), "H.CC.Fail","Fiery Meteor Spawned (Hydra Breakbar)", "Hydra CC Fail",0,(condition =>  condition.CombatItem.IFF == ParseEnum.IFF.Foe)),
                    new EnemyCastEndMechanic(FieryMeteor, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "H.CCed", "Fiery Meteor (Hydra Breakbar broken)","Hydra CCed", 0)
                        .UsingChecker((ce, log) => ce.ActualDuration < 12364),
                    new EnemyCastEndMechanic(FieryMeteor, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkRed), "H.CC Fail", "Fiery Meteor (Hydra Breakbar not broken)","Hydra CC Failed", 0)
                        .UsingChecker((ce,log) => ce.ActualDuration >= 12364),
                ]),
                new PlayerDstHitMechanic(TeleportHydra, new MechanicPlotlySetting(Symbols.Circle,Colors.Purple), "H.KB", "Teleport Knockback (Hydra)","Hydra TP KB", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(FireWaveDestroyer, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.DarkRed), "D.Wave", "Fire Wave (Shockwave after Destroyer's Jump or Stomp)","Destroyer Shockwave", 0),
                new PlayerDstHitMechanic(SeismicStomp, new MechanicPlotlySetting(Symbols.StarOpen,Colors.Yellow), "D.Stomp", "Seismic Stomp (Destroyer Stomp)","Seismic Stomp (Destroyer)", 0),
                new PlayerDstHitMechanic(ShatteredEarth, new MechanicPlotlySetting(Symbols.HexagramOpen,Colors.Red), "D.Slam", "Shattered Earth (Destroyer Jump Slam)","Jump Slam (Destroyer)", 0),
                new PlayerDstHitMechanic(WaveOfForce, new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.Orange), "D.Pizza", "Wave of Force (Destroyer Pizza)","Destroyer Auto", 0),
                new MechanicGroup([
                    new EnemyCastStartMechanic(SummonDestroyer, new MechanicPlotlySetting(Symbols.StarTriangleDown,Colors.DarkTeal), "D.CC", "Summon (Destroyer Breakbar)","Destroyer CC", 0),
                    new EnemyCastEndMechanic(SummonDestroyer, new MechanicPlotlySetting(Symbols.StarTriangleDown,Colors.DarkGreen), "D.CCed", "Summon (Destroyer Breakbar broken)","Destroyer CCed", 0).UsingChecker((ce, log) => ce.ActualDuration < 8332),
                    new EnemyCastEndMechanic(SummonDestroyer, new MechanicPlotlySetting(Symbols.StarTriangleDown,Colors.DarkRed), "D.CC Fail", "Summon (Destroyer Breakbar failed)","Destroyer CC Fail", 0).UsingChecker((ce,log) => ce.ActualDuration >= 8332),
                    new SpawnMechanic(SummonSpawn, new MechanicPlotlySetting(Symbols.DiamondWide,Colors.DarkRed), "D.Spwn", "Summon (Destroyer Trolls summoned)","Destroyer Summoned", 0),
                ]),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(SlashWyvern, new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Yellow), "Slash", "Wyvern Slash (Double attack: knock into pin down)","KB/Pin down", 0),
                new PlayerDstHitMechanic(TailSwipe, new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.Yellow), "W.Pizza", "Wyvern Tail Swipe (Pizza attack)","Tail Swipe", 0),
                new PlayerDstHitMechanic(FireBreath, new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.Orange), "W.Breath", "Fire Breath (Wyvern)","Fire Breath", 0),
                new PlayerDstHitMechanic(WingBuffet, new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.DarkTeal), "W.Wing", "Wing Buffet (Wyvern Launching Wing Storm)","Wing Buffet", 0),
                new MechanicGroup([
                    new EnemyCastStartMechanic(PatriarchCC, new MechanicPlotlySetting(Symbols.StarSquare,Colors.DarkTeal), "W.BB", "Platform Destruction (Patriarch CC)","Patriarch CC", 0),
                    new EnemyCastEndMechanic(PatriarchCC, new MechanicPlotlySetting(Symbols.StarSquare,Colors.DarkGreen), "W.CCed", "Platform Destruction (Patriarch Breakbar broken)","Patriarch CCed", 0)
                        .UsingChecker((ce, log) => ce.ActualDuration < 6500),
                    new EnemyCastStartMechanic(PatriarchCCJumpInAir, new MechanicPlotlySetting(Symbols.StarSquare,Colors.DarkRed), "Wyv CC Fail", "Platform Destruction (Patriarch Breakbar failed)","Patriarch CC Fail", 0),
                ]),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(SwapQadim, new MechanicPlotlySetting(Symbols.CircleCrossOpen,Colors.Magenta), "Port", "Swap (Ported from below Legendary Creature to Qadim)","Port to Qadim", 0),
                new PlayerDstBuffApplyMechanic(PowerOfTheLamp, new MechanicPlotlySetting(Symbols.TriangleUp,Colors.LightPurple,10), "Lamp", "Power of the Lamp (Returned from the Lamp)","Lamp Return", 0),
                new PlayerStatusMechanic<DeadEvent>(new MechanicPlotlySetting(Symbols.Bowtie, Colors.Black), "Taking Turns", "Achievement Eligibility: Taking Turns", "Taking Turns", 0, (log, a) => log.CombatData.GetDeadEvents(a))
                    .UsingEnable((log) => CustomCheckTakingTurns(log))
                    .UsingAchievementEligibility(),
                new PlayerDstHitMechanic(Claw, new MechanicPlotlySetting(Symbols.TriangleLeftOpen,Colors.DarkTeal,10), "Claw", "Claw (Reaper of Flesh attack)","Reaper Claw", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(BodyOfFlame, new MechanicPlotlySetting(Symbols.StarOpen,Colors.Pink,10), "P.AoE", "Body of Flame (Pyre Ground AoE (CM))","Pyre Hitbox AoE", 0),
                new EnemyStatusMechanic<DeadEvent>(new MechanicPlotlySetting(Symbols.Bowtie,Colors.Red), "Pyre.K", "Pyre Killed","Pyre Killed", 0,(log, a) => a.IsSpecies(TargetID.PyreGuardian) ? log.CombatData.GetDeadEvents(a) : new List<DeadEvent>()),
                new EnemyStatusMechanic<DeadEvent>(new MechanicPlotlySetting(Symbols.Bowtie,Colors.LightOrange), "Pyre.S.K", "Stab Pyre Killed","Stab Pyre Killed", 0,(log, a) => a.IsSpecies(TargetID.PyreGuardianStab) ? log.CombatData.GetDeadEvents(a) : new List<DeadEvent>()),
                new EnemyStatusMechanic<DeadEvent>(new MechanicPlotlySetting(Symbols.Bowtie,Colors.Orange), "Pyre.P.K", "Protect Pyre Killed","Protect Pyre Killed", 0,(log, a) => a.IsSpecies(TargetID.PyreGuardianProtect) ? log.CombatData.GetDeadEvents(a) : new List<DeadEvent>()),
                new EnemyStatusMechanic<DeadEvent>(new MechanicPlotlySetting(Symbols.Bowtie,Colors.LightRed), "Pyre.R.K", "Retal Pyre Killed","Retal Pyre Killed", 0,(log, a) => a.IsSpecies(TargetID.PyreGuardianRetal) ? log.CombatData.GetDeadEvents(a) : new List<DeadEvent>()),
                new EnemyStatusMechanic<DeadEvent>(new MechanicPlotlySetting(Symbols.Bowtie,Colors.DarkRed), "Pyre.R.K", "Resolution Pyre Killed","Resolution Pyre Killed", 0,(log, a) => a.IsSpecies(TargetID.PyreGuardianResolution) ? log.CombatData.GetDeadEvents(a) : new List<DeadEvent>()),
            ]),
        ]));
        Extension = "qadim";
        Icon = EncounterIconQadim;
        GenericFallBackMethod = FallBackMethod.Death | FallBackMethod.CombatExit;
        EncounterCategoryInformation.InSubCategoryOrder = 2;
        EncounterID |= 0x000003;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayQadim,
                        (1000, 994),
                        (-11676, 8825, -3870, 16582)/*,
                        (-21504, -21504, 24576, 24576),
                        (13440, 14336, 15360, 16256)*/);
    }


    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
           TargetID.Qadim,
           TargetID.AncientInvokedHydra,
           TargetID.ApocalypseBringer,
           TargetID.WyvernMatriarch,
           TargetID.WyvernPatriarch,
           TargetID.QadimLamp,
           TargetID.PyreGuardian,
           TargetID.PyreGuardianProtect,
           TargetID.PyreGuardianRetal,
           TargetID.PyreGuardianResolution,
           TargetID.PyreGuardianStab,
        ];
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var maxHPUpdates = combatData
            .Where(x => x.IsStateChange == StateChange.MaxHealthUpdate)
            .Select(x => new MaxHealthUpdateEvent(x, agentData))
            .GroupBy(x => x.MaxHealth).ToDictionary(x => x.Key, x => x.ToList());
        if (evtcVersion.Build >= ArcDPSBuilds.FunctionalEffect2Events)
        {
            if (maxHPUpdates.TryGetValue(14940, out var potentialPlatformAgentMaxHPs))
            {
                var platformAgents = potentialPlatformAgentMaxHPs.Select(x => x.Src).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth >= 2576 && x.HitboxWidth <= 2578);
                foreach (AgentItem platform in platformAgents)
                {
                    platform.OverrideType(AgentItem.AgentType.NPC, agentData);
                    platform.OverrideID(TargetID.QadimPlatform, agentData);
                    platform.OverrideAwareTimes(platform.FirstAware, fightData.LogEnd);
                }
            }
        }
        IReadOnlyList<AgentItem> pyres = agentData.GetNPCsByID(TargetID.PyreGuardian);
        // Lamps
        var qadimLampMarkerGUID = combatData
            .Where(x => x.IsStateChange == StateChange.IDToGUID &&
                GetContentLocal((byte)x.OverstackValue) == ContentLocal.Marker &&
                MarkerGUIDs.QadimLampMarker.Equals(x.SrcAgent, x.DstAgent))
            .Select(x => new MarkerGUIDEvent(x, evtcVersion))
            .FirstOrDefault();
        if (qadimLampMarkerGUID != null)
        {
            var lamps = combatData
                .Where(x => x.IsStateChange == StateChange.Marker && x.Value == qadimLampMarkerGUID.ContentID)
                .Select(x => agentData.GetAgent(x.SrcAgent, x.Time))
                .Where(x => x.Type == AgentItem.AgentType.Gadget)
                .Distinct();
            foreach (var lamp in lamps)
            {
                lamp.OverrideID(TargetID.UraGadget_BloodstoneShard, agentData);
                lamp.OverrideType(AgentItem.AgentType.NPC, agentData);
            }
        } 
        else
        {
            if (maxHPUpdates.TryGetValue(14940, out var potentialLampAgentMaxHPs))
            {
                var lampAgents = potentialLampAgentMaxHPs.Select(x => x.Src).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth == 202);
                foreach (AgentItem lamp in lampAgents)
                {
                    lamp.OverrideType(AgentItem.AgentType.NPC, agentData);
                    lamp.OverrideID(TargetID.QadimLamp, agentData);
                }
            }
        }
        // Pyres
        var protectPyrePositions = new Vector2[] { new(-8947, 14728), new(-10834, 12477) };
        var stabilityPyrePositions = new Vector2[] { new(-4356, 12076), new(-5889, 14723), new(-7851, 13550) };
        var resolutionRetaliationPyrePositions = new Vector2[] { new(-8951, 9429), new(-5716, 9325), new(-7846, 10612) };
        foreach (AgentItem pyre in pyres)
        {
            CombatItem? positionEvt = combatData.FirstOrDefault(x => x.SrcMatchesAgent(pyre) && x.IsStateChange == StateChange.Position);
            if (positionEvt != null)
            {
                var position = MovementEvent.GetPoint3D(positionEvt).XY();
                if (protectPyrePositions.Any(x => (x - position).Length() < InchDistanceThreshold))
                {
                    pyre.OverrideID(TargetID.PyreGuardianProtect, agentData);
                }
                else if (stabilityPyrePositions.Any(x => (x - position).Length() < InchDistanceThreshold))
                {
                    pyre.OverrideID(TargetID.PyreGuardianStab, agentData);
                }
                else if (resolutionRetaliationPyrePositions.Any(x => (x - position).Length() < InchDistanceThreshold))
                {
                    pyre.OverrideID(gw2Build >= GW2Builds.May2021Balance ? TargetID.PyreGuardianResolution : TargetID.PyreGuardianRetal, agentData);
                }
            }
        }
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        foreach (SingleActor target in Targets)
        {
            if (target.IsSpecies(TargetID.PyreGuardianProtect))
            {
                target.OverrideName("Protect " + target.Character);
            }
            if (target.IsSpecies(TargetID.PyreGuardianRetal))
            {
                target.OverrideName("Retal " + target.Character);
            }
            if (target.IsSpecies(TargetID.PyreGuardianResolution))
            {
                target.OverrideName("Resolution " + target.Character);
            }
            if (target.IsSpecies(TargetID.PyreGuardianStab))
            {
                target.OverrideName("Stab " + target.Character);
            }
        }
        var platformNames = new List<string>()
        {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "00",
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
        };
        _manualPlatforms = TrashMobs.Count(x => platformNames.Contains(x.Character)) != 12;
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        if (!agentData.TryGetFirstAgentItem(TargetID.Qadim, out var qadim))
        {
            throw new MissingKeyActorsException("Qadim not found");
        }
        if (combatData.HasMovementData)
        {
            var qadimInitialPosition = new Vector3(-9742.406f, 12075.2627f, -4731.031f);
            var positions = combatData.GetMovementData(qadim).Where(x => x is PositionEvent pe && pe.Time < qadim.FirstAware + MinimumInCombatDuration).Select(x => x.GetPoint3D());
            if (!positions.Any(x => (x - qadimInitialPosition).XY().Length() < 150))
            {
                return FightData.EncounterStartStatus.Late;
            }
        }
        if (TargetHPPercentUnderThreshold(TargetID.Qadim, fightData.FightStart, combatData, Targets) ||
            (Targets.Any(x => x.IsSpecies(TargetID.AncientInvokedHydra)) && TargetHPPercentUnderThreshold((int)TargetID.AncientInvokedHydra, fightData.FightStart, combatData, Targets)))
        {
            return FightData.EncounterStartStatus.Late;
        }
        return FightData.EncounterStartStatus.Normal;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return [new DamageCastFinder(BurningCrucible, BurningCrucible)];
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        // Find target
        if (!agentData.TryGetFirstAgentItem(TargetID.Qadim, out var qadim))
        {
            throw new MissingKeyActorsException("Qadim not found");
        }
        CombatItem? startCast = combatData.FirstOrDefault(x => x.SkillID == QadimInitialCast && x.StartCasting());
        CombatItem? sanityCheckCast = combatData.FirstOrDefault(x => (x.SkillID == FlameSlash3 || x.SkillID == FlameSlash || x.SkillID == FlameWave) && x.StartCasting());
        if (startCast == null || sanityCheckCast == null)
        {
            return fightData.LogStart;
        }
        // sanity check
        if (sanityCheckCast.Time - startCast.Time > 0)
        {
            return startCast.Time;
        }
        return GetGenericFightOffset(fightData);
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        // Warning: Combat replay relies on these phases.
        // If changing phase detection, combat replay platform timings may have to be updated.

        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor qadim = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Qadim)) ?? throw new MissingKeyActorsException("Qadim not found");
        phases[0].AddTarget(qadim, log);
        var secondaryTargetIds = new HashSet<TargetID>
                    {
                       TargetID.WyvernMatriarch,
                       TargetID.WyvernPatriarch,
                       TargetID.AncientInvokedHydra,
                       TargetID.ApocalypseBringer,
                    };
        phases[0].AddTargets(Targets.Where(x => x.IsAnySpecies(secondaryTargetIds)), log, PhaseData.TargetPriority.Blocking);
        if (!requirePhases)
        {
            return phases;
        }
        phases.AddRange(GetPhasesByInvul(log, QadimInvulnerable, qadim, true, false));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.AddParentPhase(phases[0]);
            if (i % 2 == 0)
            {
                phase.Name = "Qadim P" + (i) / 2;
                var pyresFirstAware = new List<long>();
                var pyres = new List<TargetID>
                    {
                        TargetID.PyreGuardian,
                        TargetID.PyreGuardianProtect,
                        TargetID.PyreGuardianStab,
                        TargetID.PyreGuardianRetal,
                        TargetID.PyreGuardianResolution,
                    };
                foreach (var pyreId in pyres)
                {
                    pyresFirstAware.AddRange(log.AgentData.GetNPCsByID(pyreId).Where(x => phase.InInterval(x.FirstAware)).Select(x => x.FirstAware));
                }
                if (pyresFirstAware.Count > 0 && pyresFirstAware.Max() > phase.Start)
                {
                    phase.OverrideStart(pyresFirstAware.Max());
                }
                phase.AddTarget(qadim, log);
                phase.AddTargets(Targets.Where(x => x.IsAnySpecies(pyres)), log, PhaseData.TargetPriority.NonBlocking); 
            }
            else
            {
                var ids = new List<TargetID>
                    {
                       TargetID.WyvernMatriarch,
                       TargetID.WyvernPatriarch,
                       TargetID.AncientInvokedHydra,
                       TargetID.ApocalypseBringer,
                       TargetID.QadimLamp
                    };
                AddTargetsToPhaseAndFit(phase, ids, log);
                if (phase.Targets.Count > 0)
                {
                    var phaseTarIDs = new HashSet<int>(phase.Targets.Keys.Select(x => x.ID));
                    if (phaseTarIDs.Contains((int)TargetID.AncientInvokedHydra))
                    {
                        phase.Name = "Hydra";
                    }
                    else if (phaseTarIDs.Contains((int)TargetID.ApocalypseBringer))
                    {
                        phase.Name = "Apocalypse";
                    }
                    else if (phaseTarIDs.Contains((int)TargetID.WyvernPatriarch) || phaseTarIDs.Contains((int)TargetID.WyvernMatriarch))
                    {
                        phase.Name = "Wyvern";
                    }
                    else
                    {
                        phase.Name = "Unknown";
                    }
                }
            }
        }
        phases.RemoveAll(x => x.Start >= x.End);
        return phases;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.QadimPlatform,
            TargetID.LavaElemental1,
            TargetID.LavaElemental2,
            TargetID.IcebornHydra,
            TargetID.GreaterMagmaElemental1,
            TargetID.GreaterMagmaElemental2,
            TargetID.FireElemental,
            TargetID.FireImp,
            TargetID.ReaperOfFlesh,
            TargetID.DestroyerTroll,
            TargetID.IceElemental,
            TargetID.AngryZommoros,
            TargetID.AssaultCube,
            TargetID.AwakenedSoldier,
            TargetID.Basilisk,
            TargetID.BlackMoa,
            TargetID.BrandedCharr,
            TargetID.BrandedDevourer,
            TargetID.ChakDrone,
            TargetID.CrazedKarkaHatchling,
            TargetID.FireImpLamp,
            TargetID.GhostlyPirateFighter,
            TargetID.GiantBrawler,
            TargetID.GiantHunter,
            TargetID.GoldOoze,
            TargetID.GrawlBascher,
            TargetID.GrawlTrapper,
            TargetID.GuildInitiateModusSceleris,
            TargetID.IcebroodAtrocity,
            TargetID.IcebroodKodan,
            TargetID.IcebroodQuaggan,
            TargetID.Jotun,
            TargetID.JungleWurm,
            TargetID.Karka,
            TargetID.MinotaurBull,
            TargetID.ModnirrBerserker,
            TargetID.MoltenDisaggregator,
            TargetID.MoltenProtector,
            TargetID.MoltenReverberant,
            TargetID.MordremVinetooth,
            TargetID.Murellow,
            TargetID.NightmareCourtier,
            TargetID.OgreHunter,
            TargetID.PirareSkrittSentry,
            TargetID.PolarBear,
            TargetID.Rabbit,
            TargetID.ReefSkelk,
            TargetID.RisenKraitDamoss,
            TargetID.RottingAncientOakheart,
            TargetID.RottingDestroyer,
            TargetID.ShadowSkelk,
            TargetID.SpiritOfExcess,
            TargetID.TamedWarg,
            TargetID.TarElemental,
            TargetID.WindRider,
        ];
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);

        if (_manualPlatforms)
        {
            AddManuallyAnimatedPlatformsToCombatReplay(Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Qadim)), log, environmentDecorations);
        }

        // Incineration Orbs - CM
        if (log.CombatData.TryGetGroupedEffectEventsByGUID(EffectGUIDs.QadimCMIncinerationOrbs, out var cmOrbs))
        {
            foreach (var orbs in cmOrbs)
            {
                var positions = orbs.Select(x => x.Position.XY());
                var middle = positions.FirstOrDefault(x => x.IsInTriangle(positions.Where(y => y != x).ToList()));
                EffectEvent? middleEvent = orbs.FirstOrDefault(x => x.Position.XY() == middle);
                if (middleEvent != null)
                {
                    foreach (EffectEvent effect in orbs)
                    {
                        uint radius = (uint)(effect == middleEvent ? 540 : 180);
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 2600);
                        var circle = new CircleDecoration(radius, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position));
                        var circle2 = new CircleDecoration(radius, lifespan, Colors.Red, 0.4, new PositionConnector(effect.Position));
                        environmentDecorations.Add(circle);
                        environmentDecorations.Add(circle2.UsingGrowingEnd(lifespan.end));
                    }
                }
            }
        }

        // Incineration Orbs - Pyres
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimPyresIncinerationOrbs, out var pyreOrbs))
        {
            foreach (EffectEvent effect in pyreOrbs)
            {
                uint radius = 240;
                (long start, long end) lifespan = effect.ComputeLifespan(log, 2300);
                var circle = new CircleDecoration(radius, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position));
                var circleRed = new CircleDecoration(radius, lifespan, Colors.Red, 0.4, new PositionConnector(effect.Position));
                environmentDecorations.Add(circle);
                environmentDecorations.Add(circleRed.UsingGrowingEnd(lifespan.end));
            }
        }

        // Bouncing blue orbs
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimJumpingBlueOrbs, out var blueOrbEvents))
        {
            foreach (EffectEvent effect in blueOrbEvents)
            {
                (long start, long end) lifespan = effect.ComputeDynamicLifespan(log, effect.Duration);
                var circle = new CircleDecoration(100, lifespan, Colors.Blue, 0.5, new PositionConnector(effect.Position));
                environmentDecorations.Add(circle);
            }
        }

        // Inferno - Qadim's AoEs on every platform
        // ! Disabled until we have a working solution for effects on moving platforms
        /*if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimInfernoAoEs, out var infernoAoEs))
        {
            foreach (EffectEvent effect in infernoAoEs)
            {
                int radius = 150;
                (long start, long end) lifespan = effect.ComputeLifespan(log, 3000);
                var circle = new CircleDecoration(radius, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position));
                var circleRed = new CircleDecoration(radius, lifespan, Colors.Red, 0.4, new PositionConnector(effect.Position));
                environmentDecorations.Add(circle);
                environmentDecorations.Add(circleRed.UsingGrowingEnd(lifespan.end));
            }
        }*/
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        var cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

        long castDuration;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.Qadim:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    switch (cast.SkillId)
                    {
                        // Breakbar CC
                        case QadimCC:
                            lifespan = (cast.Time, cast.EndTime);
                            replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMajorSizeInPixel, lifespan, Colors.Red, 0.6, Colors.Black, 0.2, [(cast.Time, 0), (cast.ExpectedEndTime, 100)], new AgentConnector(target))
                                .UsingRotationConnector(new AngleConnector(180)));
                            break;
                        // Riposte
                        case QadimRiposte:
                            lifespan = (cast.Time, cast.EndTime);
                            replay.Decorations.Add(new CircleDecoration(2200, lifespan, Colors.Red, 0.5, new AgentConnector(target)));
                            break;
                        // Big Hit - Mace smash to the ground
                        case BigHit:
                            long start = cast.Time;
                            int delay = 2230;
                            castDuration = 2680;
                            uint radius = 2000;
                            uint impactRadius = 40;
                            int spellCenterDistance = 300;
                            if (target.TryGetCurrentFacingDirection(log, start + 1000, out var facing)
                                && target.TryGetCurrentPosition(log, start + 1000, out var targetPosition))
                            {
                                var position = new Vector3(targetPosition.X + (facing.X * spellCenterDistance), targetPosition.Y + (facing.Y * spellCenterDistance), targetPosition.Z);
                                (long, long) lifespanShockwave = (start + delay, start + delay + castDuration);
                                GeographicalConnector connector = new PositionConnector(position);
                                replay.Decorations.Add(new CircleDecoration(impactRadius, (start, start + delay), Colors.Orange, 0.2, connector));
                                replay.Decorations.Add(new CircleDecoration(impactRadius, (start + delay - 10, start + delay + 100), Colors.Orange, 0.7, connector));
                                replay.Decorations.AddShockwave(connector, lifespanShockwave, Colors.Yellow, 0.7, radius);
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.AncientInvokedHydra:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    switch (cast.SkillId)
                    {
                        // Fiery Meteor - Breakbar CC
                        case FieryMeteor:
                            lifespan = (cast.Time, cast.EndTime);
                            replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMajorSizeInPixel, lifespan, Colors.Red, 0.6, Colors.Black, 0.2, [(cast.Time, 0), (cast.ExpectedEndTime, 100)], new AgentConnector(target))
                                .UsingRotationConnector(new AngleConnector(180)));
                            break;
                        // Elemental Breath - Triple fire breath
                        case ElementalBreath:
                            {
                                long delay = 2600;
                                castDuration = 1000;
                                lifespan = (cast.Time + delay, cast.Time + delay + castDuration);
                                if (target.TryGetCurrentFacingDirection(log, lifespan.start + 1000, out var facing))
                                {
                                    replay.Decorations.Add(new PieDecoration(1300, 70, lifespan, Colors.LightOrange, 0.3, new AgentConnector(target)).UsingRotationConnector(new AngleConnector(facing)));
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.WyvernMatriarch:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    switch (cast.SkillId)
                    {
                        // Wing Buffet
                        case WingBuffet:
                            {
                                long start = cast.Time;
                                int preCast = Math.Min(3500, cast.ActualDuration);
                                int duration = Math.Min(6500, cast.ActualDuration);
                                uint range = 2800;
                                uint span = 2400;
                                if (target.TryGetCurrentFacingDirection(log, start + 1000, out var facing))
                                {
                                    var positionConnector = (AgentConnector)new AgentConnector(target).WithOffset(new(range / 2, 0, 0), true);
                                    var rotationConnextor = new AngleConnector(facing);
                                    replay.Decorations.Add(new RectangleDecoration(range, span, (start, start + preCast), Colors.LightBlue, 0.2, positionConnector).UsingRotationConnector(rotationConnextor));
                                    replay.Decorations.Add(new RectangleDecoration(range, span, (start + preCast, start + duration), Colors.LightBlue, 0.5, positionConnector).UsingRotationConnector(rotationConnextor));
                                }
                            }
                            break;
                        // Fire Breath
                        case FireBreath:
                            {
                                long start = cast.Time;
                                uint radius = 1000;
                                int delay = 1600;
                                int duration = 3000;
                                int openingAngle = 70;
                                int fieldDuration = 10000;
                                if (target.TryGetCurrentFacingDirection(log, start + 1000, out var facing) && target.TryGetCurrentPosition(log, start + 1000, out var pos))
                                {
                                    var rotationConnector = new AngleConnector(facing);
                                    replay.Decorations.Add(new PieDecoration(radius, openingAngle, (start + delay, start + delay + duration), Colors.Yellow, 0.3, new AgentConnector(target)).UsingRotationConnector(rotationConnector));
                                    replay.Decorations.Add(new PieDecoration(radius, openingAngle, (start + delay + duration, start + delay + fieldDuration), Colors.Red, 0.3, new PositionConnector(pos)).UsingRotationConnector(rotationConnector));
                                }
                            }
                            break;
                        // Tail Swipe
                        case TailSwipe:
                            {
                                long start = cast.Time;
                                uint maxRadius = 700;
                                uint radiusDecrement = 100;
                                int delay = 1435;
                                int openingAngle = 59;
                                int angleIncrement = 60;
                                int coneAmount = 4;
                                if (target.TryGetCurrentFacingDirection(log, start + 1000, out var facing))
                                {
                                    float initialAngle = facing.GetRoundedZRotationDeg();
                                    var connector = new AgentConnector(target);
                                    for (uint i = 0; i < coneAmount; i++)
                                    {
                                        var rotationConnector = new AngleConnector(initialAngle - (i * angleIncrement));
                                        replay.Decorations.AddWithBorder((PieDecoration)new PieDecoration(maxRadius - (i * radiusDecrement), openingAngle, (start, start + delay), Colors.LightOrange, 0.3, connector).UsingRotationConnector(rotationConnector));

                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.WyvernPatriarch:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    switch (cast.SkillId)
                    {
                        // Breakbar CC
                        case PatriarchCC:
                            lifespan = (cast.Time, cast.EndTime);
                            replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMajorSizeInPixel, lifespan, Colors.Red, 0.6, Colors.Black, 0.2, [(cast.Time, 0), (cast.ExpectedEndTime, 100)], new AgentConnector(target))
                                .UsingRotationConnector(new AngleConnector(180)));
                            break;
                        // Fire Breath
                        case FireBreath:
                            {
                                long start = cast.Time;
                                uint radius = 1000;
                                int delay = 1600;
                                int duration = 3000;
                                int openingAngle = 60;
                                int fieldDuration = 10000;
                                if (target.TryGetCurrentFacingDirection(log, start + 1000, out var facing) && target.TryGetCurrentPosition(log, start + 1000, out var pos))
                                {
                                    var rotationConnector = new AngleConnector(facing);
                                    replay.Decorations.Add(new PieDecoration(radius, openingAngle, (start + delay, start + delay + duration), Colors.Yellow, 0.3, new AgentConnector(target)).UsingRotationConnector(rotationConnector));
                                    replay.Decorations.Add(new PieDecoration(radius, openingAngle, (start + delay + duration, start + delay + fieldDuration), Colors.Red, 0.3, new PositionConnector(pos)).UsingRotationConnector(rotationConnector));
                                }
                            }
                            break;
                        // Tail Swipe
                        case TailSwipe:
                            {
                                long start = cast.Time;
                                uint maxRadius = 700;
                                uint radiusDecrement = 100;
                                int delay = 1435;
                                int openingAngle = 59;
                                int angleIncrement = 60;
                                int coneAmount = 4;
                                if (target.TryGetCurrentFacingDirection(log, start + 1000, out var facing))
                                {
                                    float initialAngle = facing.GetRoundedZRotationDeg();
                                    var connector = new AgentConnector(target);
                                    for (uint i = 0; i < coneAmount; i++)
                                    {
                                        var rotationConnector = new AngleConnector(initialAngle - (i * angleIncrement));
                                        replay.Decorations.AddWithBorder((PieDecoration)new PieDecoration(maxRadius - (i * radiusDecrement), openingAngle, (start, start + delay), Colors.LightOrange, 0.4, connector).UsingRotationConnector(rotationConnector));
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.ApocalypseBringer:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    switch (cast.SkillId)
                    {
                        // Shattered Earth - Jump with shockwave
                        case ShatteredEarth:
                            {
                                long start = cast.Time;
                                int delay = 1800;
                                int duration = 3000;
                                uint maxRadius = 2000;
                                lifespan = (start + delay, start + delay + duration);
                                GeographicalConnector connector = new AgentConnector(target);
                                replay.Decorations.AddShockwave(connector, lifespan, Colors.Yellow, 0.7, maxRadius);
                            }
                            break;
                        // Seismic Stomp - Stomp with shockwave
                        case SeismicStomp:
                            {
                                long start = cast.Time;
                                int delay = 1600;
                                int duration = 3500;
                                uint maxRadius = 2000;
                                uint impactRadius = 500;
                                int spellCenterDistance = 270; //hitbox radius
                                if (target.TryGetCurrentFacingDirection(log, start + 1000, out var facing) && target.TryGetCurrentPosition(log, start + 1000, out var targetPosition))
                                {
                                    var position = new Vector3(targetPosition.X + facing.X * spellCenterDistance, targetPosition.Y + facing.Y * spellCenterDistance, targetPosition.Z);
                                    replay.Decorations.Add(new CircleDecoration(impactRadius, (start, start + delay), Colors.Orange, 0.1, new PositionConnector(position)));
                                    replay.Decorations.Add(new CircleDecoration(impactRadius, (start + delay - 10, start + delay + 100), Colors.Orange, 0.5, new PositionConnector(position)));
                                    replay.Decorations.Add(new CircleDecoration(maxRadius, (start + delay, start + delay + duration), Colors.Yellow, 0.5, new PositionConnector(position)).UsingFilled(false).UsingGrowingEnd(start + delay + duration));
                                }
                            }
                            break;
                        // Wave of Force - Cones Swipe
                        case WaveOfForce:
                            {
                                long start = cast.Time;
                                uint maxRadius = 1000;
                                uint radiusDecrement = 200;
                                int delay = 1560;
                                int openingAngle = 44;
                                int angleIncrement = 45;
                                int coneAmount = 3;
                                if (target.TryGetCurrentFacingDirection(log, start + 1000, out var facing))
                                {
                                    float initialAngle = facing.GetRoundedZRotationDeg();
                                    var connector = new AgentConnector(target);
                                    for (uint i = 0; i < coneAmount; i++)
                                    {
                                        var rotationConnector = new AngleConnector(initialAngle - (i * angleIncrement));
                                        replay.Decorations.AddWithBorder((PieDecoration)new PieDecoration(maxRadius - (i * radiusDecrement), openingAngle, (start, start + delay), Colors.LightOrange, 0.4, connector).UsingRotationConnector(rotationConnector));
                                    }
                                }
                            }
                            break;
                        // Summon Destroyer - Breakbar CC
                        case SummonDestroyer:
                            lifespan = (cast.Time, cast.EndTime);
                            replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMajorSizeInPixel, lifespan, Colors.Red, 0.6, Colors.Black, 0.2, [(cast.Time, 0), (cast.ExpectedEndTime, 100)], new AgentConnector(target))
                                .UsingRotationConnector(new AngleConnector(180)));
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.QadimPlatform:
                if (_manualPlatforms)
                {
                    return;
                }
                const float hiddenOpacity = 0.1f;
                const float visibleOpacity = 1f;
                const float noOpacity = -1f;
                var heights = replay.Positions.Select(x => new ParametricPoint1D(x.XYZ.Z, x.Time));
                var opacities = new List<ParametricPoint1D> { new(visibleOpacity, target.FirstAware) };
                int velocityIndex = 0;
                SingleActor qadim = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Qadim)) ?? throw new MissingKeyActorsException("Qadim not found");
                HealthUpdateEvent? below21Percent = log.CombatData.GetHealthUpdateEvents(qadim.AgentItem).FirstOrDefault(x => x.HealthPercent < 21);
                long finalPhasePlatformSwapTime = below21Percent != null ? below21Percent.Time + 9000 : log.FightData.LogEnd;
                float threshold = 1f;
                switch (target.Character)
                {
                    case "00":
                    case "0":
                        if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(-76.52588f, 44.1894531f, 22.7294922f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                        {
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(0, 0, 0), noOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                            {
                                AddOpacityUsingVelocity(replay.Velocities, opacities, new(0, 0, 0), visibleOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                            }
                        }
                        break;
                    case "01":
                    case "1":
                        foreach (var velocity in replay.Velocities)
                        {
                            if ((velocity.XYZ - new Vector3(-28.3569336f, -49.2431641f, 90.90576f)).Length() < threshold)
                            {
                                opacities.Add(new ParametricPoint1D(hiddenOpacity, velocity.Time));
                                break;
                            }
                        }
                        break;
                    case "02":
                    case "2":
                        if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(-0.122070313f, 77.88086f, 4.54101563f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                        {
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(37.0361328f, -13.94043f, -22.7294922f), visibleOpacity, velocityIndex, out velocityIndex, 10000, 0, hiddenOpacity))
                            {
                                if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(153.723145f, -110.742188f, -3.63769531f), hiddenOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                {
                                    AddOpacityUsingVelocity(replay.Velocities, opacities, new(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                                }
                            }
                        }
                        break;
                    case "03":
                    case "3":
                        if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(348.474121f, -123.4375f, 10.9130859f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                        {
                            AddOpacityUsingVelocity(replay.Velocities, opacities, new(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                        }
                        break;
                    case "04":
                    case "4":
                        if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(37.20703f, 13.94043f, 22.7294922f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                        {
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(-0.29296875f, -59.6923828f, -13.6352539f), visibleOpacity, velocityIndex, out velocityIndex, 10000, 0, hiddenOpacity))
                            {
                                if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(357.592773f, -294.018555f, 13.6352539f), hiddenOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                {
                                    AddOpacityUsingVelocity(replay.Velocities, opacities, new(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                                }
                            }
                        }
                        break;
                    case "05":
                    case "5":
                        bool doNormalPlat5 = true;
                        if (log.FightData.IsCM)
                        {
                            doNormalPlat5 = false;
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(-8.0078125f, 0, 0), hiddenOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                            {
                                if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(6.7871094f, 18.188477f, -9.094238f), noOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                {
                                    doNormalPlat5 = AddOpacityUsingVelocity(replay.Velocities, opacities, new(0, 0, 0), visibleOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity);
                                }
                            }
                        }
                        if (doNormalPlat5)
                        {
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(255.712891f, -69.43359f, 2.722168f), hiddenOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                            {
                                AddOpacityUsingVelocity(replay.Velocities, opacities, new(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                            }
                        }
                        break;
                    case "06":
                    case "6":
                        if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(182.8125f, -80.15137f, 22.7294922f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                        {
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(0, 0, 0), noOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                            {
                                if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(0, 0, 0), visibleOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                {
                                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.QadimJumpingBlueOrbs, out var blueOrbs))
                                    {
                                        EffectEvent? lastBlueOrb = blueOrbs.FirstOrDefault(x => x.Time > opacities.Last().Time);
                                        if (lastBlueOrb != null)
                                        {
                                            (long start, long end) = lastBlueOrb.ComputeDynamicLifespan(log, lastBlueOrb.Duration);
                                            if (Math.Abs(end - log.FightData.FightEnd) > 500)
                                            {
                                                opacities.Add(new ParametricPoint1D(hiddenOpacity, end));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "07":
                    case "7":
                        if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(-98.53516f, 49.2919922f, -19.0917969f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                        {
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(0, 0, 0), visibleOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                            {
                                if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(46.75293f, 0, -6.35986328f), hiddenOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                {
                                    AddOpacityUsingVelocity(replay.Velocities, opacities, new(0, 0, 0), visibleOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity);
                                }
                            }
                        }
                        break;
                    case "08":
                    case "8":
                        if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(37.20703f, -14.0136719f, 18.17627f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                        {
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(15.234375f, 31.9580078f, -9.094238f), noOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                            {
                                if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                {
                                    if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(87.25586f, -70.87402f, 4.54101563f), hiddenOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                    {
                                        if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(0f, 0f, 0f), noOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                        {
                                            AddOpacityUsingVelocity(replay.Velocities, opacities, new(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "09":
                    case "9":
                        if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(50.7568359f, 69.3847656f, -6.35986328f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                        {
                            AddOpacityUsingVelocity(replay.Velocities, opacities, new(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                        }
                        break;
                    case "10":
                        if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(-0.122070313f, -77.92969f, 4.54101563f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                        {
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(0f, 0f, 0f), noOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                            {
                                if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                {
                                    bool doNormalPlat10 = true;
                                    if (log.FightData.IsCM)
                                    {
                                        doNormalPlat10 = AddOpacityUsingVelocity(replay.Velocities, opacities, new(49.8291f, -43.5791f, 4.5410156f), hiddenOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity);
                                    }
                                    if (doNormalPlat10)
                                    {
                                        if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(-51.3793945f, 110.473633f, -3.63769531f), log.FightData.IsCM ? noOpacity : hiddenOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                        {
                                            AddOpacityUsingVelocity(replay.Velocities, opacities, new(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "11":
                        if (AddOpacityUsingVelocity(replay.Velocities, opacities, new(143.493652f, 114.282227f, 17.27295f), noOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                        {
                            AddOpacityUsingVelocity(replay.Velocities, opacities, new(0f, 0f, 0f), noOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                        }
                        break;
                    default:
                        break;
                }
                var platformDecoration = new BackgroundIconDecoration(ParserIcons.QadimPlatform, 0, 2247, opacities, heights, (target.FirstAware, target.LastAware), new AgentConnector(target));
                RotationConnector platformRotationConnector = new AgentFacingConnector(target, 180, AgentFacingConnector.RotationOffsetMode.AddToMaster);
                replay.Decorations.Add(platformDecoration.UsingRotationConnector(platformRotationConnector));
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Returns true if velocity was found
    /// </summary>
    /// <param name="velocities">Velocities of the platform</param>
    /// <param name="opacities">Opacities of the platform, will be filled</param>
    /// <param name="referenceVelocity">Velocity to find</param>
    /// <param name="opacity">Opacity to add, won't be added if <0</param>
    /// <param name="timeOffset">Time to be added to found velocity time</param>
    /// <param name="forceHideTime">If > 0, forces the addition of a hidden opacity at given time</param>
    /// <param name="hiddenOpacity">Hidden opacity value</param>
    private static bool AddOpacityUsingVelocity(IReadOnlyList<ParametricPoint3D> velocities, List<ParametricPoint1D> opacities, in Vector3 referenceVelocity, float opacity, int startIndex, out int foundIndexPlusOne, long timeOffset, long forceHideTime, float hiddenOpacity)
    {
        float threshold = 1f;
        for (int velocityIndex = startIndex; velocityIndex < velocities.Count; velocityIndex++)
        {
            if ((referenceVelocity - velocities[velocityIndex].XYZ).Length() < threshold)
            {
                if (opacity >= 0)
                {
                    opacities.Add(new ParametricPoint1D(opacity, velocities[velocityIndex].Time + timeOffset));
                }

                if (forceHideTime > 0 && opacity != hiddenOpacity)
                {
                    opacities.Add(new ParametricPoint1D(hiddenOpacity, forceHideTime));
                }

                foundIndexPlusOne = velocityIndex + 1;
                return true;
            }
        }
        foundIndexPlusOne = 0;
        return false;
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Qadim)) ?? throw new MissingKeyActorsException("Qadim not found");
        return (target.GetHealth(combatData) > 21e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
    }

    private static void AddManuallyAnimatedPlatformsToCombatReplay(SingleActor? qadim, ParsedEvtcLog log, CombatReplayDecorationContainer decorations)
    {
        if (qadim == null)
        {
            return;
        }
        // We later use the target to find out the timing of the last move
        Debug.Assert(qadim.IsSpecies(TargetID.Qadim));

        // These values were all calculated by hand.
        // It would be way nicer to calculate them here, but we don't have a nice vector library
        // and it would double the amount of work.

        const string platformImageUrl = ParserIcons.QadimPlatform;
        const float hiddenOpacity = 0.1f;

        bool isCM = log.FightData.IsCM;

        const float xLeft = -7975;
        const float xLeftLeft = -8537;
        const float xLeftLeftLeft = -9661;
        const float xRight = -6851;
        const float xRightRight = -6289;
        const float xRightRightRight = -5165;
        const float yMid = 12077;
        const float yUp = 13050;
        const float yUpUp = 14023;
        const float yDown = 11104;
        const float yDownDown = 10131;
        const float xGapsLeft = -8018;
        const float xGapsLeftLeft = -8618;
        const float xGapsLeftLeftLeft = -9822;
        const float xGapsRight = -6815;
        const float xGapsRightRight = -6215;
        const float xGapsRightRightRight = -5011;
        const float yGapsUp = 13118;
        const float yGapsUpUp = 14161;
        const float yGapsDown = 11037;
        const float yGapsDownDown = 9993;

        const float xDestroyerLeftLeftLeft = -9732;
        const float xDestroyerLeftLeft = xGapsLeftLeft + 5;
        const float xDestroyerLeft = -8047;
        const float xDestroyerRight = -6778;
        const float xDestroyerRightRight = xGapsRightRight - 5;
        const float xDestroyerRightRightRight = -5101;

        var retaliationPyre1 = new Vector2(-8951, 9429);
        var protectionPyre1 = new Vector2(-8947, 14728);
        var stabilityPyre1 = new Vector2(-4356, yMid);

        var retaliationPyre2 = new Vector2(-5717, 9325);
        var protectionPyre2 = new Vector2(-10834, 12477);
        var stabilityPyre2 = new Vector2(-5889, 14723);

        const float wyvernPhaseMiddleRotation = 0.34f;

        const float yJumpingPuzzleOffset1 = 12077 - 11073; // Easternmost two platforms
        const float yJumpingPuzzleOffset2 = 12077 - 10612; // Two platforms on each side, including pyres
        const float yJumpingPuzzleOffset3 = 12077 - 10056; // Northernmost and southernmost rotating platforms
        const float xJumpingPuzzleQadim = -10237; // Qadim's platform
        const float xJumpingPuzzlePreQadim = -8808;
        const float xJumpingPuzzlePyres = -7851;
        const float xJumpingPuzzlePrePyres = -6289;
        const float xJumpingPuzzleRotatingPrePyres = -5736;
        const float xJumpingPuzzleFirstRotating = -5736;
        const float xJumpingPuzzleFirstPlatform = -4146;

        const float jumpingPuzzleRotationRate = 2 * MathF.PI / 30; // rad/sec, TODO: Not perfect, it's a bit off

        const float xFinalPlatform = -8297;
        var qadimFinalXY = new Vector2(-7356, 12077);

        const float zDefault = -4731;
        const float zJumpingPuzzlePyres = -4871;
        const float zJumpingPuzzlePrePyres = -4801;
        const float zJumpingPuzzlePreQadim = -4941;
        const float zJumpingPuzzleFirstPlatform = -4591; // The first platform Zommoros visits
        const float zJumpingPuzzleSecondPlatform = -4661; // The second platform Zommoros visits
        const float zFinalPlatforms = -5011;

        const long timeAfterPhase2 = 4000;
        const long timeAfterWyvernPhase = 25000;
        const long jumpingPuzzleShuffleDuration = 11000;
        const long lastPhasePreparationDuration = 13000;

        // If phase data is not calculated, only the first layout is used
        var phases = log.FightData.GetPhases(log).Where(x => !x.BreakbarPhase).ToList();

        long qadimPhase1Time = phases.Count > 1 ? phases[1].End : long.MaxValue;
        long destroyerPhaseTime = phases.Count > 2 ? phases[2].End : long.MaxValue;
        long qadimPhase2Time = phases.Count > 3 ? phases[3].End : long.MaxValue;
        long wyvernPhaseTime = phases.Count > 4 ? phases[4].End + timeAfterPhase2 : long.MaxValue;
        long jumpingPuzzleTime = phases.Count > 5 ? phases[5].End + timeAfterWyvernPhase : long.MaxValue;
        long finalPhaseTime = int.MaxValue;
        if (phases.Count > 6)
        {
            //TODO(Rennorb) @perf: Why was there an unused reference to last phase here?
            foreach (var pos in qadim.GetCombatReplayNonPolledPositions(log))
            {
                if ((pos.XYZ.XY() - qadimFinalXY).LengthSquared() < 25)
                {
                    finalPhaseTime = (int)pos.Time;
                }
            }
        }

        long jumpingPuzzleDuration = finalPhaseTime - lastPhasePreparationDuration - jumpingPuzzleShuffleDuration - jumpingPuzzleTime;

        const int platformCount = 12;

        // The following monstrosity is needed to avoid the final platform rotating all the way back
        // must be an odd multiple of PI
        int finalPlatformHalfRotationCount =
            (int)MathF.Round((MathF.PI + jumpingPuzzleDuration / 1000.0f * jumpingPuzzleRotationRate) / MathF.PI);
        if (finalPlatformHalfRotationCount % 2 == 0)
        {
            finalPlatformHalfRotationCount++;
        }
        float finalPlatformRotation = MathF.PI * finalPlatformHalfRotationCount;


        // Proper skipping of phases (if even possible) is not implemented.
        // Right now transitioning to another state while still moving behaves weirdly.
        // Interpolating to find the position to stop in would be necessary.
        long startOffset = -log.FightData.FightStartOffset;
        (long start, long duration, (float x, float y, float z, float angle, float opacity)[] platforms)[] movements =
        [
            (
                // Initial position, all platforms tightly packed

                startOffset, 0, new[]
                {
                    (xLeftLeftLeft, yMid, zDefault, 0, 1f),
                    (xLeftLeft, yUpUp, zDefault, MathF.PI, 1f),
                    (xRightRight, yUpUp, zDefault, 0, 1f),
                    (xRightRightRight, yMid, zDefault, MathF.PI, 1f),
                    (xRightRight, yDownDown, zDefault, 0, 1f),
                    (xLeftLeft, yDownDown, zDefault, MathF.PI, 1f),
                    (xLeftLeft, yMid, zDefault, MathF.PI, 1f),
                    (xLeft, yUp, zDefault, 0, 1f),
                    (xRight, yUp, zDefault, MathF.PI, 1f),
                    (xRightRight, yMid, zDefault, 0, 1f),
                    (xRight, yDown, zDefault, MathF.PI, 1f),
                    (xLeft, yDown, zDefault, 0, 1f),
                }
            ),
            (
                // Hydra phase, all platforms have a small gap between them
                startOffset, 12000, new[]
                {
                    (xGapsLeftLeftLeft, yMid, zDefault, 0, 1f),
                    (xGapsLeftLeft, yGapsUpUp, zDefault, MathF.PI, 1f),
                    (xGapsRightRight, yGapsUpUp, zDefault, 0, 1f),
                    (xGapsRightRightRight, yMid, zDefault, MathF.PI, 1f),
                    (xGapsRightRight, yGapsDownDown, zDefault, 0, 1f),
                    (xGapsLeftLeft, yGapsDownDown, zDefault, MathF.PI, 1f),
                    (xGapsLeftLeft, yMid, zDefault, MathF.PI, 1f),
                    (xGapsLeft, yGapsUp, zDefault, 0, 1f),
                    (xGapsRight, yGapsUp, zDefault, MathF.PI, 1f),
                    (xGapsRightRight, yMid, zDefault, 0, 1f),
                    (xGapsRight, yGapsDown, zDefault, MathF.PI, 1f),
                    (xGapsLeft, yGapsDown, zDefault, 0, 1f),
                }
            ),
            (
                // First Qadim phase, packed together except for pyre platforms
                qadimPhase1Time, 10000, new[]
                {
                    (xLeftLeftLeft, yMid, zDefault, 0, 1f),
                    (protectionPyre1.X, protectionPyre1.Y, zDefault, MathF.PI, 1f),
                    (xRightRight, yUpUp, zDefault, 0, 1f),
                    (stabilityPyre1.X, stabilityPyre1.Y, zDefault, MathF.PI, 1f),
                    (xRightRight, yDownDown, zDefault, 0, 1f),
                    (retaliationPyre1.X, retaliationPyre1.Y, zDefault, MathF.PI, 1f),
                    (xLeftLeft, yMid, zDefault, MathF.PI, 1f),
                    (xLeft, yUp, zDefault, 0, 1f),
                    (xRight, yUp, zDefault, MathF.PI, 1f),
                    (xRightRight, yMid, zDefault, 0, 1f),
                    (xRight, yDown, zDefault, MathF.PI, 1f),
                    (xLeft, yDown, zDefault, 0, 1f),
                }
            ),
            (
                // Destroyer phase, packed together, bigger vertical gap in the middle, 4 platforms hidden
                destroyerPhaseTime, 15000, new[]
                {
                    (xDestroyerLeftLeftLeft, yMid, zDefault, 0, 1f),
                    (xGapsLeftLeft, yGapsUpUp, zDefault, MathF.PI, hiddenOpacity), // TODO: Unknown position while hidden
                    (xGapsRightRight, yGapsUpUp, zDefault, 0, hiddenOpacity), // TODO: Unknown position while hidden
                    (xDestroyerRightRightRight, yMid, zDefault, MathF.PI, 1f),
                    (xGapsRightRight, yGapsDownDown, zDefault, 0, hiddenOpacity), // TODO: Unknown position while hidden
                    (xGapsLeftLeft, yGapsDownDown, zDefault, MathF.PI, hiddenOpacity), // TODO: Unknown position while hidden
                    (xDestroyerLeftLeft, yMid, zDefault, MathF.PI, 1f),
                    (xDestroyerLeft, yUp, zDefault, 0, isCM ? hiddenOpacity : 1f),
                    (xDestroyerRight, yUp, zDefault, MathF.PI, 1f),
                    (xDestroyerRightRight, yMid, zDefault, 0, 1f),
                    (xDestroyerRight, yDown, zDefault, MathF.PI, 1f),
                    (xDestroyerLeft, yDown, zDefault, 0, 1f),
                }
            ),
            (
                // Second Qadim phase
                qadimPhase2Time, 10000, new[]
                {
                    (protectionPyre2.X, protectionPyre2.Y, zDefault, 0, 1f),
                    (-8540, 14222, zDefault, MathF.PI, 1f),
                    (stabilityPyre2.X, stabilityPyre2.Y, zDefault, 0, 1f),
                    (-5160, yMid, zDefault, MathF.PI, 1f),
                    (retaliationPyre2.X, retaliationPyre2.Y, zDefault, 0, 1f),
                    (-8369, 9640, zDefault, MathF.PI, 1f),
                    (protectionPyre2.X + 1939, protectionPyre2.Y, zDefault, MathF.PI, 1f),
                    (-7978, 13249, zDefault, 0, 1f),
                    (-6846, 13050, zDefault, MathF.PI, 1f),
                    (-6284, yMid, zDefault, 0, 1f),
                    (retaliationPyre2.X - 1931 / 2, retaliationPyre2.Y + 1672, zDefault, MathF.PI, 1f),
                    (-7807, 10613, zDefault, 0, 1f),
                }
            ),
            (
                // TODO: Heights are not correct, they differ here, currently not important for the replay
                // Wyvern phase
                wyvernPhaseTime, 11000, new[]
                {
                    (protectionPyre2.X, protectionPyre2.Y, zDefault, 0f, hiddenOpacity), // TODO: Unknown position while hidden
                    (-9704, 15323, zDefault, MathF.PI, 1f),
                    (-7425, 15312, zDefault, 0, 1f),
                    (-5160, yMid, zDefault, MathF.PI, hiddenOpacity), // TODO: Unknown position while hidden
                    (-5169, 8846, zDefault, 0, isCM ? hiddenOpacity : 1f),
                    (-7414, 8846, zDefault, MathF.PI, hiddenOpacity),
                    (-7728, 11535, zDefault, MathF.PI + wyvernPhaseMiddleRotation, 1f),
                    (-9108, 14335, zDefault, 0, 1f),
                    (-7987, 14336, zDefault, MathF.PI, 1f),
                    (-7106, 12619, zDefault, wyvernPhaseMiddleRotation, 1f),
                    (-5729, 9821, zDefault, MathF.PI, 1f),
                    (-6854, 9821, zDefault, 0, 1f),
                }
            ),
            (
                // Jumping puzzle preparation, platforms hide
                jumpingPuzzleTime - 500, 0, new[]
                {
                    (protectionPyre2.X, protectionPyre2.Y, zDefault, 0, hiddenOpacity),
                    (-9704f, 15323f, zDefault, MathF.PI, hiddenOpacity),
                    (-7425, 15312, zDefault, 0, hiddenOpacity),
                    (-5160, yMid, zDefault, MathF.PI, hiddenOpacity),
                    (-5169, 8846, zDefault, 0, hiddenOpacity),
                    (-7414, 8846, zDefault, MathF.PI, hiddenOpacity),
                    (-7728, 11535, zDefault, MathF.PI + wyvernPhaseMiddleRotation, hiddenOpacity),
                    (-9108, 14335, zDefault, 0, hiddenOpacity),
                    (-7987, 14336, zDefault, MathF.PI, hiddenOpacity),
                    (-7106, 12619, zDefault, wyvernPhaseMiddleRotation, hiddenOpacity),
                    (-5729, 9821, zDefault, MathF.PI, 1f),
                    (-6854, 9821, zDefault, 0, hiddenOpacity),
                }
            ),
            (
                // Jumping puzzle, platforms move
                jumpingPuzzleTime, jumpingPuzzleShuffleDuration - 1, new[]
                {
                    (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0, hiddenOpacity),
                    (xJumpingPuzzleFirstRotating, yMid, zDefault, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, 0, hiddenOpacity),
                    (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, 0, hiddenOpacity),
                    (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzlePreQadim, yMid, zJumpingPuzzlePreQadim, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, MathF.PI, 1f),
                    (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, MathF.PI, hiddenOpacity),
                }
            ),
            (
                // Jumping puzzle, platforms appear
                jumpingPuzzleTime + jumpingPuzzleShuffleDuration - 1, 1, new[]
                {
                    (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0, 1f),
                    (xJumpingPuzzleFirstRotating, yMid, zDefault, MathF.PI, 1f),
                    (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, 0, 1f),
                    (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, MathF.PI, 1f),
                    (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, 0, 1f),
                    (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, MathF.PI, 1f),
                    (xJumpingPuzzlePreQadim, yMid, zJumpingPuzzlePreQadim, MathF.PI, 1f),
                    (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, MathF.PI, 1f),
                    (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, MathF.PI, 1f),
                    (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, MathF.PI, 1f),
                    (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, MathF.PI, 1f),
                }
            ),
            (
                // Jumping puzzle appears, platforms rotate...
                // Jumping puzzle platform breaks are not shown for now because their timing is rather tricky.
                jumpingPuzzleTime + jumpingPuzzleShuffleDuration, jumpingPuzzleDuration, new[]
                {
                    (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0f, 1f),
                    (xJumpingPuzzleFirstRotating, yMid, zDefault, MathF.PI + jumpingPuzzleDuration / 1000f * jumpingPuzzleRotationRate, 1f),
                    (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, -jumpingPuzzleDuration / 1000f * jumpingPuzzleRotationRate, 1f),
                    (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, MathF.PI, 1f),
                    (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, jumpingPuzzleDuration / 1000f * jumpingPuzzleRotationRate, 1f),
                    (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, MathF.PI, 1f),
                    (xJumpingPuzzlePreQadim, yMid, zJumpingPuzzlePreQadim, MathF.PI + jumpingPuzzleDuration / 1000f * jumpingPuzzleRotationRate, 1f),
                    (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, MathF.PI, 1f),
                    (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, MathF.PI, 1f),
                    (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, MathF.PI, 1f),
                    (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, MathF.PI, 1f),
                }
            ),
            (
                // Final phase preparation.
                finalPhaseTime - lastPhasePreparationDuration, lastPhasePreparationDuration, new[]
                {
                    (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0, 1f),
                    (xJumpingPuzzleFirstRotating, yMid, zDefault, MathF.PI + jumpingPuzzleDuration / 1000f * jumpingPuzzleRotationRate, hiddenOpacity),
                    (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, -jumpingPuzzleDuration / 1000f * jumpingPuzzleRotationRate, hiddenOpacity),
                    (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, jumpingPuzzleDuration / 1000f * jumpingPuzzleRotationRate, hiddenOpacity),
                    (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, MathF.PI, hiddenOpacity),
                    (xFinalPlatform, yMid, zJumpingPuzzlePreQadim, finalPlatformRotation, hiddenOpacity),
                    (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, MathF.PI, hiddenOpacity),
                }
            ),
            (
                // Final phase.
                finalPhaseTime, 0, new[]
                {
                    (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0, 1f),
                    (xJumpingPuzzleFirstRotating, yMid, zDefault, MathF.PI + jumpingPuzzleDuration / 1000f * jumpingPuzzleRotationRate, hiddenOpacity),
                    (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, -jumpingPuzzleDuration / 1000f * jumpingPuzzleRotationRate, hiddenOpacity),
                    (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, jumpingPuzzleDuration / 1000f * jumpingPuzzleRotationRate, hiddenOpacity),
                    (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, MathF.PI, hiddenOpacity),
                    (xFinalPlatform, yMid, zJumpingPuzzlePreQadim, finalPlatformRotation, 1f),
                    (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, MathF.PI, hiddenOpacity),
                }
            ),
            (
                // Second to last platform is destroyed
                finalPhaseTime, 7000, new[]
                {
                    (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0, hiddenOpacity),
                    (xJumpingPuzzleFirstRotating, yMid, zDefault, MathF.PI + jumpingPuzzleDuration / 1000f * jumpingPuzzleRotationRate, hiddenOpacity),
                    (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, -jumpingPuzzleDuration / 1000f * jumpingPuzzleRotationRate, hiddenOpacity),
                    (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, jumpingPuzzleDuration / 1000f * jumpingPuzzleRotationRate, hiddenOpacity),
                    (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, MathF.PI, hiddenOpacity),
                    (xFinalPlatform, yMid, zJumpingPuzzlePreQadim, finalPlatformRotation, 1f),
                    (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, MathF.PI, hiddenOpacity),
                    (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, MathF.PI, hiddenOpacity),
                }
            ),
        ];

        // All platforms have to have positions in all phases
        Debug.Assert(movements.All(x => x.platforms.Length == platformCount));

        var platforms = new MovingPlatformDecoration[platformCount];
        for (int i = 0; i < platformCount; i++)
        {
            platforms[i] = new MovingPlatformDecoration(platformImageUrl, 2247, 2247, (int.MinValue, int.MaxValue));
            decorations.Add(platforms[i]);
        }

        // Add movement "keyframes" on a movement end and on the start of the next one.
        // This approach requires one extra movement at the start for initial positions (should be of duration 0)
        for (int i = 0; i < movements.Length; i++)
        {
            var movement = movements[i];

            for (int platformIndex = 0; platformIndex < platformCount; platformIndex++)
            {
                var platform = platforms[platformIndex];
                var (x, y, z, angle, opacity) = movement.platforms[platformIndex];

                // Add a keyframe for movement end.
                platform.AddPosition(x, y, z, angle, opacity, movement.start + movement.duration);

                if (i != movements.Length - 1)
                {
                    // Add a keyframe for next movement start to ensure that there is no change
                    // between the end of this movement and the start of the next one
                    platform.AddPosition(x, y, z, angle, opacity, movements[i + 1].start);
                }
            }
        }
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log)
    {
        base.SetInstanceBuffs(log);

        if (log.FightData.Success)
        {
            if (log.CombatData.GetBuffData(AchievementEligibilityManipulateTheManipulator).Any())
            {
                InstanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, AchievementEligibilityManipulateTheManipulator));
            }
            else if (CustomCheckManipulateTheManipulator(log))
            {
                InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityManipulateTheManipulator], 1));
            }
        }
    }

    /// <summary>
    /// Check the player positions for the achievement eligiblity.<br></br>
    /// </summary>
    /// <returns><see langword="true"/> if eligible, otherwise <see langword="false"/>.</returns>
    private static bool CustomCheckTakingTurns(ParsedEvtcLog log)
    {
        // Z coordinates info:
        // The player in the lamp is roughly at -81
        // The death zone from falling off the platform is roughly at -2950
        // The main fight platform is at roughly at -4700

        var lamps = log.AgentData.GetNPCsByID(TargetID.QadimLamp).ToList();
        int lampLabyrinthZ = -250; // Height Threshold

        foreach (Player p in log.PlayerList)
        {
            IReadOnlyList<ParametricPoint3D> positions = p.GetCombatReplayPolledPositions(log);
            var exitBuffs = log.CombatData.GetBuffDataByIDByDst(PowerOfTheLamp, p.AgentItem).OfType<BuffApplyEvent>();

            // Count the times the player has entered and exited the lamp.
            // A player that has entered the lamp but never exites and remains alive is elible for the achievement.

            int entered = 0;
            int exited = 0;

            for (int i = 0; i < lamps.Count; i++)
            {
                if (positions.Any(x => x.XYZ.Z > lampLabyrinthZ && x.Time >= lamps[i].FirstAware && x.Time <= lamps[i].LastAware) && entered == exited)
                {
                    entered++;
                }

                var end = i < lamps.Count - 1 ? lamps[i + 1].FirstAware : log.FightData.FightEnd;
                var segment = new Segment(lamps[i].LastAware, end, 1);

                if (exitBuffs.Any(x => segment.ContainsPoint(x.Time)))
                {
                    exited++;
                }

                if (entered > 1) { return false; } // Failed achievement
            }
        }

        return true; // Successful achievement
    }

    /// <summary>
    /// Check the NPC positions for the achievement eligiblity.<br></br>
    /// </summary>
    /// <returns><see langword="true"/> if eligible, otherwise <see langword="false"/>.</returns>
    private bool CustomCheckManipulateTheManipulator(ParsedEvtcLog log)
    {
        SingleActor? qadim = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Qadim)) ?? throw new MissingKeyActorsException("Qadim not found");
        SingleActor? hydra = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.AncientInvokedHydra));
        SingleActor? bringer = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.ApocalypseBringer));
        SingleActor? matriarch = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.WyvernMatriarch));
        SingleActor? patriarch = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.WyvernPatriarch));

        if (qadim != null && hydra != null && bringer != null && matriarch != null && patriarch != null)
        {
            return !DistanceCheck(log, qadim, hydra) &&
                !DistanceCheck(log, qadim, bringer) &&
                !DistanceCheck(log, qadim, matriarch) &&
                !DistanceCheck(log, qadim, patriarch);
        }

        return false;
    }

    /// <summary>
    /// Find out if the distance points between <paramref name="qadim"/> and an <paramref name="add"/> goes under 2000 range.
    /// </summary>
    /// <returns><see langword="true"/> if distance goes under 2000, otherwise <see langword="false"/>.</returns>
    private static bool DistanceCheck(ParsedEvtcLog log, SingleActor qadim, SingleActor add)
    {
        // Get positions of Ancient Invoked Hydra, Apocalypse Bringer, Wyvern Matriarch and Patriarch
        IReadOnlyList<ParametricPoint3D> addPositions = add.GetCombatReplayPolledPositions(log);
        if (addPositions.Count == 0)
        {
            return true;
        }

        // Get positions of Qadim during the times of the adds being present
        var qadimPositions = qadim.GetCombatReplayPolledPositions(log).Where(x => x.Time >= addPositions.First().Time && x.Time <= addPositions.Last().Time).ToList();
        if (qadimPositions.Count == 0)
        {
            return true;
        }

        // For each matching position polled, check if the distance between points is under 2000
        for (int i = 0; i < Math.Min(addPositions.Count, qadimPositions.Count); i++)
        {
            if ((qadimPositions[i].XYZ - addPositions[i].XYZ).Length() < 2000)
            {
                return true;
            }
        }

        // Never went under 2000 range
        return false;
    }
}
