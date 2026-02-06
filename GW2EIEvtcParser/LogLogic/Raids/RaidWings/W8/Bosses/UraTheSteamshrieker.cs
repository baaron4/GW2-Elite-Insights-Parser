using System;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.AchievementEligibilityIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class UraTheSteamshrieker : MountBalrior
{
    internal readonly MechanicGroup Mechanics = new([
            // Sulfuric Geysers
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(SulfuricEruption, new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightBlue), "SulfErup.H", "Hit by Sulfuric Eruption (Geyser Spawn)", "Sulfuric Eruption Hit", 0),
                new PlayerDstHealthDamageHitMechanic(EruptionVent, new MechanicPlotlySetting(Symbols.TriangleNW, Colors.DarkPink), "ErupVent.H", "Hit by Eruption Vent (Geyser Shockwave)", "Eruption Vent Hit", 0),
                new PlayerDstEffectMechanic([EffectGUIDs.UraSulfuricGeyserTarget, EffectGUIDs.UraSulfuricGeyserTargetCM], new MechanicPlotlySetting(Symbols.Hexagon, Colors.Blue), "SulfGey.T", "Targeted by Sulfuric Geyser (Spawn)", "Sulfuric Geyser Spawn Target", 0),
                new PlayerSrcBuffRemoveFromMechanic(HardenedCrust, new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.White), "Dispel.Sulf", "Dispelled Sulfuric Geyser (Removed Hardened Crust)", "Dispelled Sulfuric Geyser", 0)
                    .UsingChecker((brae, log) => brae.To.IsSpecies(TargetID.SulfuricGeyser)),
                new MechanicGroup([
                    new AchievementEligibilityMechanic(Ach_HopscotchMaster, new MechanicPlotlySetting(Symbols.TriangleSW, Colors.DarkPink), "Achiv.Hop.L", "Achievement Eligibility: Hopscotch Master Lost", "Achiv: Hopscotch Master Lost", 0)
                        .UsingChecker((evt, log) => evt.Lost),
                    new AchievementEligibilityMechanic(Ach_HopscotchMaster, new MechanicPlotlySetting(Symbols.TriangleSW, Colors.Pink), "Achiv.Hop.K", "Achievement Eligibility: Hopscotch Master Kept", "Achiv: Hopscotch Master Kept", 0)
                        .UsingChecker((evt, log) => !evt.Lost)
                ]),
            ]),
            // Titanspawn Geysers
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(CreateTitanspawnGeyser, new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Orange), "UraJump.H", "Hit by Create Titanspawn Geyser AoE (Ura jump in place)", "Create Titanspawn Geyser Hit", 0)
                    .WithStabilitySubMechanic(
                        new PlayerDstHealthDamageHitMechanic(CreateTitanspawnGeyser, new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.LightOrange), "UraJump.CC", "CC by Create Titanspawn Geyser AoE (Ura jump in place)", "Create Titanspawn Geyser CC", 0),
                        false
                    ),
                new PlayerSrcBuffRemoveFromMechanic(HardenedCrust, new MechanicPlotlySetting(Symbols.CircleX, Colors.White), "Dispel.Titn", "Dispelled Titanspawn Geyser (Removed Hardened Crust)", "Dispelled Titanspawn Geyser", 0)
                    .UsingChecker((brae, log) => brae.To.IsAnySpecies([TargetID.TitanspawnGeyser, TargetID.TitanspawnGeyserGadget])),
            ]),
            // Toxic Geysers
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([ToxicGeyser1, ToxicGeyser2, ToxicGeyserCM], new MechanicPlotlySetting(Symbols.StarSquare, Colors.GreenishYellow), "ToxGeyser.H", "Hit by Toxic Geyser", "Toxic Geyser Hit", 0),
                new PlayerSrcBuffRemoveFromMechanic(HardenedCrust, new MechanicPlotlySetting(Symbols.CircleCross, Colors.White), "Dispel.Toxc", "Dispelled Toxic Geyser (Removed Hardened Crust)", "Dispelled Toxic Geyser", 0)
                    .UsingChecker((brae, log) => brae.To.IsSpecies(TargetID.ToxicGeyser)),
            ]),
            // Ura
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(ScaldingAura, new MechanicPlotlySetting(Symbols.Pentagon, Colors.LightPink), "ScalAura.H", "Hit by Scalding Aura (Ura's Hitbox)", "Scalding Aura Hit", 0),
                new PlayerDstBuffApplyMechanic(SulfuricAcid, new MechanicPlotlySetting(Symbols.TriangleNEOpen, Colors.Purple), "SulfAcid.A", "Received Sulfuric Acid", "Sulfuric Acid Application", 0),
                new PlayerDstHealthDamageHitMechanic(SulfuricFroth, new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.LightGrey), "SulfFroth.H", "Hit by Sulfuric Froth (Acid Projectile from Ura)", "Sulfuric Froth Hit", 0),
            ]),
            // Steam Prison
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(SteamPrison, new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Orange), "Ste.Prison.H", "Hit by Steam Prison", "Steam Prison Hit", 0),
                new PlayerDstEffectMechanic(EffectGUIDs.UraSteamPrisonIndicator, new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.LightOrange), "Ste.Prison.T", "Targeted by Steam Prison (Ring)", "Steam Prison Target", 0),
            ]),
            // Pressure Blast
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(PressureBlastTargetBuff, new MechanicPlotlySetting(Symbols.CircleOpen, Colors.LightBlue), "Pres.Blast.T", "Targeted by Pressure Blast (Bubbles)", "Pressure Blast Target", 0),
                new PlayerDstBuffApplyMechanic(PressureBlastBubbleBuff, new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Blue), "Pres.Blast.Up", "Lifted in a bubble by Pressure Blast", "Pressure Blast Bubble", 0),
                new PlayerSrcBuffRemoveFromMechanic(PressureBlastBubbleBuff, new MechanicPlotlySetting(Symbols.CircleOpen, Colors.White), "Dispel.P", "Dispelled Player (Removed Pressure Blast)", "Dispelled Player", 0)
                    .UsingChecker((brae, log) => brae.To.IsPlayer),
            ]),
            // Dispel
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(Deterrence, new MechanicPlotlySetting(Symbols.Diamond, Colors.LightRed), "Pick-up Shard", "Picked up the Bloodstone Shard", "Bloodstone Shard Pick-up", 0),
                new PlayerDstBuffApplyMechanic(BloodstoneSaturation, new MechanicPlotlySetting(Symbols.Diamond, Colors.DarkPurple), "Dispel", "Used Dispel (SAK)", "Used Dispel", 0),
            ]),
            // Fumaroller
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(BreakingGround, new MechanicPlotlySetting(Symbols.Star, Colors.Red), "BreGround.H", "Hit by Breaking Ground (Fumaroller 8 pointed star)", "Breaking Ground Hit", 0),
                new PlayerDstHealthDamageHitMechanic(MantleGrinder, new MechanicPlotlySetting(Symbols.HourglassOpen, Colors.DarkPurple), "MantGrind.H", "Hit by Mantle Grinder (Fumaroller Roll)", "Mantle Grinder Hit", 0),
                new PlayerDstHealthDamageHitMechanic(FullSteam, new MechanicPlotlySetting(Symbols.Octagon, Colors.GreyishGreen), "FullSteam.H", "Hit by Full Steam (Fumaroller Dash)", "Full Steam Hit", 0),
            ]),
            // Ventshot
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(PressureRelease, new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.CobaltBlue), "PresRel.H", "Hit by Pressure Release (Ventshot Jump AoE)", "Pressure Release Hit", 0),
                new PlayerDstHealthDamageHitMechanic(ForcedEruption, new MechanicPlotlySetting(Symbols.PentagonOpen, Colors.Blue), "ForcErup.H", "Hit by Forced Eruption (Ventshot Homing Orb)", "Forced Eruption Hit", 0),
                new PlayerDstHealthDamageHitMechanic(SearingSnipe, new MechanicPlotlySetting(Symbols.StarTriangleUpOpen, Colors.LightBlue), "SearSnipe.H", "Hit by Searing Snipe (Ventshot Projectile)", "Searing Snipe Hit", 0),
                new PlayerDstHealthDamageHitMechanic(StoneSlamConeKnockback, new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.Orange), "StnSlam.CC", "CC by Stone Slam (Ventshot Cone)", "Stone Slam CC", 0)
                    .UsingBuffChecker(Stability, false),
            ]),
            new EnemySrcHealthDamageMechanic(Return, new MechanicPlotlySetting(Symbols.TriangleRightOpen, Colors.White), "Return", "Ura returned to the center", "Return", 100),
            new EnemyDstBuffApplyMechanic(RisingPressure, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.LightOrange), "Rising Pressure", "Applied Rising Pressure", "Rising Pressure", 0),
            new EnemyDstBuffApplyMechanic(TitanicResistance, new MechanicPlotlySetting(Symbols.TriangleUpOpen, Colors.LightOrange), "Titanic Resistance", "Applied Titanic Resistance", "Titanic Resistance", 0),
        ]);
    public UraTheSteamshrieker(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "ura";
        Icon = EncounterIconUra;
        ChestID = ChestID.UrasChest;
        LogCategoryInformation.InSubCategoryOrder = 2;
        LogID |= 0x000003;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (1746, 1860),
                        (2550, 6200, 9010, 13082));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayUraTheSteamshrieker, crMap);
        return crMap;
    }


    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.Ura,
            TargetID.LegendaryVentshot,
            TargetID.ChampionFumaroller,
            TargetID.EliteFumaroller,
        ];
    }

    internal override HashSet<TargetID> ForbidBreakbarPhasesFor()
    {
        return
        [
            TargetID.LegendaryVentshot,
            TargetID.ChampionFumaroller,
            TargetID.EliteFumaroller,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return 
        [
            TargetID.SulfuricGeyser,
            TargetID.TitanspawnGeyser,
            TargetID.TitanspawnGeyserGadget,
            TargetID.ToxicGeyser,
            TargetID.UraGadget_BloodstoneShard,
        ];
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new BuffGainCastFinder(UraDispelSAK, BloodstoneSaturation),
        ];
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = GetGenericLogOffset(logData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            var deterrences = combatData.Where(x => (x.IsBuffApply() || x.IsBuffRemoval()) && x.SkillID == Deterrence);
            var activeDeterrences = new Dictionary<ulong, long>();
            foreach (var deterrence in deterrences)
            {
                if (deterrence.IsBuffApply())
                {
                    activeDeterrences[deterrence.DstAgent] = deterrence.Time;
                }
                else
                {
                    activeDeterrences.Remove(deterrence.SrcAgent);
                }
                if (activeDeterrences.Count == 2)
                {
                    break;
                }
            }
            startToUse = GetEnterCombatTime(logData, agentData, combatData, logStartNPCUpdate.Time, GenericTriggerID, logStartNPCUpdate.DstAgent);
            if (activeDeterrences.Count == 2)
            {
                startToUse = Math.Min(startToUse, activeDeterrences.Values.Max());
            }
        }
        return startToUse;
    }

    internal static void FindGeysers(EvtcVersionEvent evtcVersion, AgentData agentData, List<CombatItem> combatData)
    {
        // titanspawn geysers
        var titanGeyserMarkerGUID = combatData
            .Where(x => x.IsStateChange == StateChange.IDToGUID &&
                GetContentLocal((byte)x.OverstackValue) == ContentLocal.Marker &&
                MarkerGUIDs.UraTitanspawnGeyserMarker.Equals(x.SrcAgent, x.DstAgent))
            .Select(x => new MarkerGUIDEvent(x, evtcVersion))
            .FirstOrDefault();
        if (titanGeyserMarkerGUID != null)
        {
            var titanAgents = combatData
                .Where(x => x.IsStateChange == StateChange.Marker && x.Value == titanGeyserMarkerGUID.ContentID)
                .Select(x => agentData.GetAgent(x.SrcAgent, x.Time))
                .Where(x => x.Type == AgentItem.AgentType.Gadget)
                .Distinct();
            foreach (var titanAgent in titanAgents)
            {
                titanAgent.OverrideID(TargetID.TitanspawnGeyserGadget, agentData);
                titanAgent.OverrideType(AgentItem.AgentType.NPC, agentData);
            }
        }
        // Toxic geysers
        var toxicEffectGUID = combatData
            .Where(x => x.IsStateChange == StateChange.IDToGUID &&
                GetContentLocal((byte)x.OverstackValue) == ContentLocal.Effect &&
                (EffectGUIDs.UraToxicGeyserSpawn.Equals(x.SrcAgent, x.DstAgent) || EffectGUIDs.UraToxicGeyserSpawnCM.Equals(x.SrcAgent, x.DstAgent)))
            .Select(x => new EffectGUIDEvent(x, evtcVersion))
            .FirstOrDefault();
        if (toxicEffectGUID != null)
        {
            var toxicAgents = combatData
                .Where(x => x.IsEffect && x.SkillID == toxicEffectGUID.ContentID)
                .Select(x => agentData.GetAgent(x.SrcAgent, x.Time))
                .Where(x => x.Type == AgentItem.AgentType.Gadget)
                .Distinct();
            foreach (var toxicAgent in toxicAgents)
            {
                toxicAgent.OverrideID(TargetID.ToxicGeyser, agentData);
                toxicAgent.OverrideType(AgentItem.AgentType.NPC, agentData);
            }
        }
        // Sulfuric geysers
        var sulfuricAgents = combatData
            .Where(x => x.IsBuffApply() && x.SkillID == HardenedCrust)
            .Select(x => agentData.GetAgent(x.SrcAgent, x.Time))
            .Where(x => x.Type == AgentItem.AgentType.Gadget)
            .Distinct();
        foreach (var sulfuricAgent in sulfuricAgents)
        {
            sulfuricAgent.OverrideID(TargetID.SulfuricGeyser, agentData);
            sulfuricAgent.OverrideType(AgentItem.AgentType.NPC, agentData);
        }
        // Those can only be toxic ones
        var remainingGeysers = combatData
            .Where(x => x.IsStateChange == StateChange.MaxHealthUpdate && MaxHealthUpdateEvent.GetMaxHealth(x) == 448200)
            .Select(x => agentData.GetAgent(x.SrcAgent, x.Time))
            .Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth > 100)
            .Distinct();
        foreach (var remainingGeyser in remainingGeysers)
        {
            remainingGeyser.OverrideID(TargetID.ToxicGeyser, agentData);
            remainingGeyser.OverrideType(AgentItem.AgentType.NPC, agentData);
        }
    }

    internal static void FindBloodstoneShards(EvtcVersionEvent evtcVersion, AgentData agentData, List<CombatItem> combatData)
    {
        // Bloodstone shards
        var bloodstoneShardMarkerGUID = combatData
            .Where(x => x.IsStateChange == StateChange.IDToGUID &&
                GetContentLocal((byte)x.OverstackValue) == ContentLocal.Marker &&
                MarkerGUIDs.UraBloodstoneShardMarker.Equals(x.SrcAgent, x.DstAgent))
            .Select(x => new MarkerGUIDEvent(x, evtcVersion))
            .FirstOrDefault();
        if (bloodstoneShardMarkerGUID != null)
        {
            var bloodstoneShardAgents = combatData
                .Where(x => x.IsStateChange == StateChange.Marker && x.Value == bloodstoneShardMarkerGUID.ContentID)
                .Select(x => agentData.GetAgent(x.SrcAgent, x.Time))
                .Where(x => x.Type == AgentItem.AgentType.Gadget)
                .Distinct();
            foreach (var toxicAgent in bloodstoneShardAgents)
            {
                toxicAgent.OverrideID(TargetID.UraGadget_BloodstoneShard, agentData);
                toxicAgent.OverrideType(AgentItem.AgentType.NPC, agentData);
            }
        }
    }

    internal static void RenameFumarollers(IReadOnlyList<SingleActor> targets)
    {
        foreach (SingleActor target in targets)
        {
            switch (target.ID)
            {
                case (int)TargetID.EliteFumaroller:
                    target.OverrideName("Elite " + target.Character);
                    break;
                case (int)TargetID.ChampionFumaroller:
                    target.OverrideName("Champion " + target.Character);
                    break;
                case (int)TargetID.LegendaryVentshot:
                    target.OverrideName("Legendary " + target.Character);
                    break;
                default:
                    break;
            }
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindGeysers(evtcVersion, agentData, combatData);
        FindBloodstoneShards(evtcVersion, agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        RenameFumarollers(Targets);
    }
    internal static IReadOnlyList<SubPhasePhaseData> ComputePhases(ParsedEvtcLog log, SingleActor ura, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = new List<SubPhasePhaseData>(6);
        PhaseData parentPhase = encounterPhase;
        bool isCm = encounterPhase.IsCM || encounterPhase.IsLegendaryCM;
        long start = encounterPhase.Start;
        long end = encounterPhase.End;

        var hp1 = log.CombatData.GetBuffData(Determined895).FirstOrDefault(x => x is BuffApplyEvent && x.To.Is(ura.AgentItem) && encounterPhase.InInterval(x.Time));
        // Healed CM
        if (hp1 != null)
        {
            var before1 = new SubPhasePhaseData(start, hp1.Time, "100% - 1%");
            before1.AddParentPhase(parentPhase);
            before1.AddTarget(ura, log);
            phases.Add(before1);
            var determinedLost = log.CombatData.GetBuffData(Determined895).FirstOrDefault(x => x is BuffRemoveAllEvent && x.To.Is(ura.AgentItem) && x.Time >= hp1.Time && encounterPhase.InInterval(x.Time));
            if (determinedLost != null)
            {
                var after1 = new SubPhasePhaseData(determinedLost.Time, end, "Healed");
                after1.AddParentPhase(parentPhase);
                after1.AddTarget(ura, log);
                phases.Add(after1);
            }
            parentPhase = before1;
        }
        // HP based phases
        var hpUpdates = ura.GetHealthUpdates(log);
        var hp70 = hpUpdates.FirstOrDefault(x => x.Value < 70 && x.Value != 0);
        var hp40 = hpUpdates.FirstOrDefault(x => x.Value < 40 && x.Value != 0);
        // 100-70
        var before70 = new SubPhasePhaseData(start, hp70.Value > 0 ? hp70.Start : end, "100% - 70%");
        before70.AddParentPhase(parentPhase);
        before70.AddTarget(ura, log);
        phases.Add(before70);
        // 70-40
        if (!hp70.IsEmpty())
        {
            // 40 - 1 CM / 40 - 0 NM
            if (!hp40.IsEmpty())
            {
                var after70before40 = new SubPhasePhaseData(hp70.Start, Math.Min(hp40.Start, end), "70% - 40%");
                after70before40.AddTarget(ura, log);
                phases.Add(after70before40);

                var after40 = isCm ? new SubPhasePhaseData(hp40.Start, hp1 != null ? Math.Min(hp1.Time, end) : end, "40% - 1%") : new SubPhasePhaseData(hp40.Start, end, "40% - 0%");
                after40.AddTarget(ura, log);
                phases.Add(after40);

                var after70 = new SubPhasePhaseData(after70before40.Start, after40.End, isCm ? "70% - 1%" : "70% - 0%");
                after70.AddParentPhase(parentPhase);
                after70.AddTarget(ura, log);
                phases.Add(after70);

                after70before40.AddParentPhase(after70);
                after40.AddParentPhase(after70);
            }
            else
            {
                var after70 = new SubPhasePhaseData(hp70.Start, end, "70% - 40%");
                after70.AddTarget(ura, log);
                phases.Add(after70);
                after70.AddParentPhase(parentPhase);
            }
        }
        return phases;
    }
    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor ura = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Ura)) ?? throw new MissingKeyActorsException("Ura not found");
        phases[0].AddTarget(ura, log);
        phases.AddRange(ComputePhases(log, ura, (EncounterPhaseData)phases[0], requirePhases));
        return phases;
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
        long castDuration;
        long growing;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.Ura:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Propel - Arrow Dash
                        case Propel:
                            castDuration = 3000;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                            if (target.TryGetCurrentFacingDirection(log, cast.Time + 1000, out var facing))
                            {
                                var offset = new Vector3(250, 0, 0);
                                var rotation = new AngleConnector(facing);
                                replay.Decorations.Add(new RectangleDecoration(500, 70, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target).WithOffset(offset, true)).UsingRotationConnector(rotation));
                            }
                            break;
                        // Create Titanspawn Geyser - Ura jumps in places and creates an AoE underneath
                        case CreateTitanspawnGeyser:
                            castDuration = 3100;
                            growing = cast.Time + castDuration;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                            replay.Decorations.AddWithGrowing(new CircleDecoration(800, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target)), growing);
                            break;
                        default:
                            break;
                    }
                }

                // Slam - Cone - Third step of the attack
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.UraSlamCone, out var slams))
                {
                    foreach (EffectEvent effect in slams)
                    {
                        long duration = 2000;
                        growing = effect.Time + duration;
                        lifespan = effect.ComputeLifespan(log, duration);
                        lifespan.end = Math.Min(lifespan.end, ComputeEndCastTimeByBuffApplication(log, target, Stun, effect.Time, duration));
                        if (target.TryGetCurrentFacingDirection(log, effect.Time, out var facingDirection, duration))
                        {
                            var pie = (PieDecoration)new PieDecoration(1000, 60, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target)).UsingRotationConnector(new AngleConnector(facingDirection));
                            replay.Decorations.AddWithGrowing(pie, growing);
                        }
                    }
                }

                // Steam Prison - Ground doughnut walls
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.UraSteamPrisonGround, out var steamPrisons))
                {
                    foreach (var effect in steamPrisons)
                    {
                        lifespan = effect.ComputeLifespan(log, 10000);
                        replay.Decorations.Add(new CircleDecoration(400, lifespan, Colors.LightGrey, 0.1, new PositionConnector(effect.Position)));
                        replay.Decorations.Add(new DoughnutDecoration(400, 500, lifespan, Colors.Grey, 0.2, new PositionConnector(effect.Position)));
                    }
                }

                // Return - Whirlpool
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.UraReturn2, out var returns))
                {
                    foreach (var effect in returns)
                    {
                        // Radius is indicative only, anyone in the arena gets pulled.
                        lifespan = effect.ComputeLifespan(log, 6000);
                        replay.Decorations.AddWithGrowing(new CircleDecoration(900, lifespan, Colors.Sand, 0.2, new PositionConnector(effect.Position)), lifespan.end, true);
                    }
                }

                // Sulfuric Eruption - Sulfuric Geyser Shockwave Spawn
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.UraSulfuricGeyserShockwave, out var shockwaves))
                {
                    foreach (var effect in shockwaves)
                    {
                        // Looks like in game it loses velocity as it gets further away from the starting point, can't display it as velocity is unknown.
                        lifespan = effect.ComputeLifespan(log, 8000);
                        replay.Decorations.AddShockwave(new PositionConnector(effect.Position), lifespan, Colors.LightGrey, 0.5, 5000);
                    }
                }
                break;
            case (int)TargetID.ToxicGeyser:
                // Damage field ring
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.UraToxicGeyserGrowing, out var effects))
                {
                    uint initialRadius = 480;
                    uint radiusIncrease = 12; // Aprox
                    uint counter = 0;

                    // Find the first effect appearing after the breakbar has started to recover.
                    var (breakbarNones, breakbarActives, breakbarImmunes, breakbarRecoverings) = target.GetBreakbarStatus(log);
                    List<long> resetTimes = [];
                    foreach (var segment in breakbarRecoverings)
                    {
                        var effect = effects.FirstOrDefault(x => x.Time > segment.Start);
                        if (effect != null)
                        {
                            resetTimes.Add(effect.Time);
                        }
                    }

                    foreach (var effect in effects)
                    {
                        // If the breakbar has been broken, reset the effect radius.
                        if (resetTimes.Contains(effect.Time))
                        {
                            counter = 0;
                        }
                        lifespan = effect.ComputeDynamicLifespan(log, 1200);
                        uint radius = initialRadius + (radiusIncrease * counter);
                        replay.Decorations.Add(new CircleDecoration(radius, lifespan, Colors.Red, 0.2, new AgentConnector(target)).UsingFilled(false));
                        counter++;
                    }

                    // Hardened Crust - Overhead
                    replay.Decorations.AddOverheadIcons(target.GetBuffStatus(log, HardenedCrust).Where(x => x.Value > 0), target, BuffImages.HardenedCrust);

                    // Breakbar
                    var toxicPercentUpdates = target.GetBreakbarPercentUpdates(log);
                    var toxicStates = target.GetBreakbarStatus(log);
                    replay.Decorations.AddBreakbar(target, toxicPercentUpdates, toxicStates);
                }
                else
                {
                    replay.Decorations.Add(new CircleDecoration(480, (target.FirstAware, target.LastAware), Colors.Red, 0.2, new AgentConnector(target)).UsingFilled(false));
                }
                break;
            case (int)TargetID.SulfuricGeyser:
                // Hardened Crust - Overhead
                replay.Decorations.AddOverheadIcons(target.GetBuffStatus(log, HardenedCrust).Where(x => x.Value > 0), target, BuffImages.HardenedCrust);

                // Damage field ring
                replay.Decorations.Add(new CircleDecoration(580, (target.FirstAware, target.LastAware), Colors.Red, 0.2, new AgentConnector(target)).UsingFilled(false));
                break;
            case (int)TargetID.TitanspawnGeyserGadget:
            case (int)TargetID.TitanspawnGeyser:
                // Hardened Crust - Overhead
                replay.Decorations.AddOverheadIcons(target.GetBuffStatus(log, HardenedCrust).Where(x => x.Value > 0), target, BuffImages.HardenedCrust);

                // Breakbar
                var titanspawnPercentUpdates = target.GetBreakbarPercentUpdates(log);
                var titanspawnStates = target.GetBreakbarStatus(log);
                replay.Decorations.AddBreakbar(target, titanspawnPercentUpdates, titanspawnStates);
                break;
            case (int)TargetID.LegendaryVentshot:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Stone Slam - Autoattack with cone
                        case StoneSlamConeKnockback:
                            lifespan = (cast.Time, cast.GetInterruptedByStunTime(log));
                            growing = cast.Time + 2000; // 2000 Cast Duration
                            target.TryGetCurrentFacingDirection(log, cast.Time, out var rotation, 300);
                            var cone = (PieDecoration)new PieDecoration(350, 90, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target)).UsingRotationConnector(new AngleConnector(rotation));
                            replay.Decorations.AddWithGrowing(cone, growing);
                            break;
                        default:
                            break;
                    }
                }

                // Pressure Release - Jump underneath Ventshot indicator
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.UraVentshotPressureRelease, out var pressureReleases))
                {
                    foreach (EffectEvent effect in pressureReleases)
                    {
                        // Value found from Jet effect time - Indicator time
                        long durationIndicator = 1240;
                        lifespan = effect.ComputeLifespanWithSecondaryEffectAndPosition(log, EffectGUIDs.UraVentshotPressureReleaseWaterJet);
                        lifespan.end = Math.Min(lifespan.end, ComputeEndCastTimeByBuffApplication(log, target, Stun, lifespan.start, durationIndicator));
                        var indicator = new CircleDecoration(225, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.AddWithGrowing(indicator, lifespan.end);
                    }
                }

                // Pressure Release - Water Jet
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.UraVentshotPressureReleaseWaterJet, out var pressureReleaseJets))
                {
                    foreach (EffectEvent effect in pressureReleaseJets)
                    {
                        // Pressure Release has 5 jets, one underneath and 4 on the cardinal sides
                        // If the effect has a duration of 3000, it's the jet underneath the Ventshot, otherwise it includes indicator and jet together.
                        if (effect.Duration != 3000)
                        {
                            long durationIndicator = 1240;
                            long startIndicator = effect.Time - durationIndicator;
                            growing = startIndicator + durationIndicator;

                            (long start, long end) lifespanIndicator = (startIndicator, ComputeEndCastTimeByBuffApplication(log, target, Stun, startIndicator, durationIndicator));
                            (long start, long end) lifespanJet = effect.ComputeLifespan(log, 3000);
                            
                            var indicator = new CircleDecoration(225, lifespanIndicator, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                            var jet = new CircleDecoration(225, lifespanJet, Colors.LightBlue, 0.2, new PositionConnector(effect.Position));
                            
                            replay.Decorations.AddWithGrowing(indicator, growing);
                            replay.Decorations.Add(jet);
                        }
                        else
                        {
                            (long start, long end) lifespanJet = effect.ComputeLifespan(log, 3000);
                            var jet = new CircleDecoration(225, lifespanJet, Colors.LightBlue, 0.2, new PositionConnector(effect.Position));
                            replay.Decorations.Add(jet);
                        }
                    }
                }

                // Blue Tether - Applies Rising Pressure to targets - Skill ID is 75295
                IEnumerable<AbstractBuffApplyEvent> tethers = log.CombatData.GetBuffApplyData(RisingPressure).Where(x => 
                    x is BuffApplyEvent &&
                    x.By.Is(target.AgentItem) &&
                    !x.To.Is(target.AgentItem));

                foreach (BuffEvent tether in tethers)
                {
                    // Manually setting the tether duration to 2 seconds.
                    lifespan = (tether.Time, tether.Time + 2000);
                    // Setting opacity to 0.1 because the Ventshot can apply multiple stacks of Rising Pressure, this makes it more visible in the replay.
                    replay.Decorations.Add(new LineDecoration(lifespan, Colors.LightBlue, 0.1, new AgentConnector(tether.To), new AgentConnector(tether.By)).WithThickess(5, false));
                }
                break;
            case (int)TargetID.ChampionFumaroller:
                // Breaking Ground - 8 pointed star
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.UraFumarollerBreakingGround, out var breakingGrounds))
                {
                    foreach (EffectEvent effect in breakingGrounds)
                    {
                        lifespan = effect.ComputeLifespan(log, 20000);
                        var rotation = new AngleConnector(effect.Rotation.Z);
                        replay.Decorations.Add(new RectangleDecoration(10, 75, lifespan, Colors.Red, 0.5, new PositionConnector(effect.Position)).UsingRotationConnector(rotation));
                    }
                }
                break;
            case (int)TargetID.EliteFumaroller:

                break;
            default:
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(player, log, replay);
        }

        // Deterrence - Pick-Up Bloodstone Shard
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, Deterrence).Where(x => x.Value > 0), player, ParserIcons.CrimsonAttunementOverhead);

        // Pressure Blast - Bubble AoE Indicator
        var pressureBlastTarget = player.GetBuffStatus(log, PressureBlastTargetBuff).Where(x => x.Value > 0);
        foreach (var segment in pressureBlastTarget)
        {
            replay.Decorations.AddWithGrowing(new CircleDecoration(180, segment.TimeSpan, Colors.LightOrange, 0.2, new AgentConnector(player)), segment.End);
        }

        // Pressure Blast - Bubble Lift Up
        var pressureBlastBubble = player.GetBuffStatus(log, PressureBlastBubbleBuff).Where(x => x.Value > 0);
        foreach (var segment in pressureBlastBubble)
        {
            replay.Decorations.Add(new CircleDecoration(100, segment.TimeSpan, Colors.LightBlue, 0.2, new AgentConnector(player)));
        }

        // Steam Prison - Ring
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(player.AgentItem, EffectGUIDs.UraSteamPrisonIndicator, out var steamPrisons))
        {
            foreach (EffectEvent effect in steamPrisons)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 3000);
                replay.Decorations.Add(new DoughnutDecoration(400, 500, lifespan, Colors.LightOrange, 0.2, new AgentConnector(player)));
            }
        }

        // Sulfuric Geyser - Target
        if (log.CombatData.TryGetEffectEventsByDstWithGUIDs(player.AgentItem, [EffectGUIDs.UraSulfuricGeyserTarget, EffectGUIDs.UraSulfuricGeyserTargetCM], out var sulfuricGeyserTarget))
        {
            foreach (var effect in sulfuricGeyserTarget)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 5000);
                int pulseCycle = 1000;
                (long start, long end) pulse = (lifespan.start, lifespan.start + pulseCycle);
                for (int i = 0; i < 5; i++)
                {
                    replay.Decorations.AddShockwave(new AgentConnector(player), pulse, Colors.LightOrange, 0.2, 300);
                    pulse.start = pulse.end;
                    pulse.end = pulse.start + pulseCycle;
                }
                replay.Decorations.AddOverheadIcon(lifespan, player, ParserIcons.BombTimerFullOverhead);
            }
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }

        // Bloodstone Radiation - AoE
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.UraBloodstoneRadiationPulse, out var bloodstoneRadiation))
        {
            foreach (var effect in bloodstoneRadiation)
            {
                // Radius is indicative only, matching the in game size. Damage is incremental hp% per pulse to everyone in the arena.
                (long start, long end) lifespan = (effect.Time, effect.Time + 500);
                environmentDecorations.AddWithBorder(new CircleDecoration(300, lifespan, Colors.Red, 0.4, new PositionConnector(effect.Position)), Colors.Red, 0.4);
            }
        }
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Ura)) ?? throw new MissingKeyActorsException("Ura not found");
        var uraHP = target.GetHealth(combatData);
        if (uraHP > 70e6)
        {
            target.OverrideName("Godscream Ura");
            return uraHP > 100e6 ? LogData.Mode.LegendaryCM : LogData.Mode.CMNoName;
        }
        target.OverrideName("Ura, the Steamshrieker");
        return LogData.Mode.Normal;
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
        var toxicGeysers = log.AgentData.GetNPCsByID(TargetID.ToxicGeyser);
        var encounterPhases = log.LogData.GetEncounterPhases(log).Where(x => x.ID == LogID);
        foreach (var encounterPhase in encounterPhases)
        {
            if (encounterPhase.Success && (encounterPhase.IsCM || encounterPhase.IsLegendaryCM))
            {
                bool eligible = true;
                foreach (var geyser in toxicGeysers)
                {
                    if (encounterPhase.IntersectsWindow(geyser.FirstAware, geyser.LastAware))
                    {
                        // Each eruption has to be active for less than 30 seconds
                        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(geyser, EffectGUIDs.UraToxicGeyserSpawnCM, out var eruptions))
                        {
                            long effectDuration = 800000;
                            foreach (var effect in eruptions.Where(x => x.Duration == effectDuration))
                            {
                                (long start, long end) = effect.ComputeDynamicLifespan(log, effectDuration);
                                // Making sure we don't use start + 800000 if an Effect End isn't present due to the encounter ending without interrupting the geyser.
                                if (Math.Min(end, log.LogData.LogEnd) - start > 30000)
                                {
                                    eligible = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (eligible)
                {
                    instanceBuffs.Add(new(log.Buffs.BuffsByIDs[AchievementEligibilityNoGeysersNoProblems], 1, encounterPhase));
                }
            }
        }
    }

    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }
        {
            var hopscotchMasterEligibilityEvents = new List<AchievementEligibilityEvent>();
            var uraPhases = log.LogData.GetEncounterPhases(log).Where(x => x.ID == LogID && (x.IsCM || x.IsLegendaryCM) && x.IntersectsWindow(p.FirstAware, p.LastAware)).ToHashSet();
            List<HealthDamageEvent> damageData = [
                ..log.CombatData.GetDamageData(EruptionVent),
                ..log.CombatData.GetDamageData(SulfuricEruption)
            ];
            damageData.SortByTime();
            foreach (var evt in damageData)
            {
                if (evt.HasHit && evt.To.Is(p.AgentItem) && p.InAwareTimes(evt.Time))
                {
                    InsertAchievementEligibityEventAndRemovePhase(uraPhases, hopscotchMasterEligibilityEvents, evt.Time, Ach_HopscotchMaster, p);
                }
            }
            AddSuccessBasedAchievementEligibityEvents(uraPhases, hopscotchMasterEligibilityEvents, Ach_HopscotchMaster, p);
            achievementEligibilityEvents.AddRange(hopscotchMasterEligibilityEvents);
        }
    }
}
