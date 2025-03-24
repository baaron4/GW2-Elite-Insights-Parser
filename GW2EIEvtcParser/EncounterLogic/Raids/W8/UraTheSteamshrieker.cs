
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class UraTheSteamshrieker : MountBalrior
{
    public UraTheSteamshrieker(int triggerID) : base(triggerID)
    {
        MechanicList.AddRange(new List<Mechanic>()
        {
            new PlayerDstHitMechanic(EruptionVent, "Eruption Vent", new MechanicPlotlySetting(Symbols.TriangleSW, Colors.DarkPink), "ErupVent.H", "Hit by Eruption Vent (Geyser Spawn)", "Eruption Vent Hit", 0),

            new PlayerDstHitMechanic(CreateTitanspawnGeyser, "Create Titanspawn Geyser", new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Orange), "UraJump.H", "Hit by Create Titanspawn Geyser AoE (Ura jump in place)", "Create Titanspawn Geyser Hit", 0),
            new PlayerDstHitMechanic(CreateTitanspawnGeyser, "Create Titanspawn Geyser", new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.LightOrange), "UraJump.CC", "CC by Create Titanspawn Geyser AoE (Ura jump in place)", "Create Titanspawn Geyser CC", 0)
                .UsingChecker((hde, log) => hde.To.HasBuff(log, Stability, hde.Time, ServerDelayConstant)),

            new PlayerDstHitMechanic([ToxicGeyser1, ToxicGeyser2, ToxicGeyserCM], "Toxic Geyser", new MechanicPlotlySetting(Symbols.StarSquare, Colors.GreenishYellow), "ToxGeyser.H", "Hit by Toxic Geyser", "Toxic Geyser Hit", 0),

            new PlayerDstEffectMechanic(EffectGUIDs.UraSulfuricGeyserTarget, "Sulfuric Geyser", new MechanicPlotlySetting(Symbols.Hexagon, Colors.Blue), "SulfGey.T", "Targeted by Sulfuric Geyser (Spawn)", "Sulfuric Geyser Spawn Target", 0),
            new PlayerSrcBuffRemoveFromMechanic(HardenedCrust, "Hardened Crust", new MechanicPlotlySetting(Symbols.Hourglass, Colors.LightOrange), "Dispel.G", "Dispelled Sulfuric Geyser (Removed Hardened Crust)", "Dispelled Sulfuric Geyser", 0)
                .UsingChecker((brae, log) => brae.To.IsSpecies(TargetID.SulfuricGeyser)),

            new PlayerDstHitMechanic(SteamPrison, "Steam Prison", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Orange), "Ste.Prison.H", "Hit by Steam Prison", "Steam Prison Hit", 0),
            new PlayerDstEffectMechanic(EffectGUIDs.UraSteamPrisonIndicator, "Steam Prison", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.LightOrange), "Ste.Prison.T", "Targeted by Steam Prison (Ring)", "Steam Prison Target", 0),

            new PlayerDstHitMechanic(ScaldingAura, "Scalding Aura", new MechanicPlotlySetting(Symbols.Pentagon, Colors.LightPink), "ScalAura.H", "Hit by Scalding Aura (Ura's Hitbox)", "Scalding Aura Hit", 0),

            new PlayerDstHitMechanic(SulfuricEruption, "Sulfuric Eruption", new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightBlue), "SulfErup.H", "Hit by Sulfuric Eruption (Shockwave)", "Sulfuric Eruption Hit", 0),
            new PlayerDstBuffApplyMechanic(SulfuricAcid, "Sulfuric Acid", new MechanicPlotlySetting(Symbols.TriangleNEOpen, Colors.Purple), "SulfAcid.A", "Received Sulfuric Acid", "Sulfuric Acid Application", 0),
            new PlayerDstHitMechanic(SulfuricFroth, "Sulfuric Froth", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.LightGrey), "SulfFroth.H", "Hit by Sulfuric Froth (Acid Projectile from Ura)", "Sulfuric Froth Hit", 0),

            new PlayerDstHitMechanic(BreakingGround, "Breaking Ground", new MechanicPlotlySetting(Symbols.Star, Colors.Red), "BreGround.H", "Hit by Breaking Ground (8 pointed star)", "Breaking Ground Hit", 0),

            new PlayerDstBuffApplyMechanic(PressureBlastTargetBuff, "Pressure Blast", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.LightBlue), "Pres.Blast.T", "Targeted by Pressure Blast (Bubbles)", "Pressure Blast Target", 0),
            new PlayerDstBuffApplyMechanic(PressureBlastBubbleBuff, "Pressure Blast", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Blue), "Pres.Blast.Up", "Lifted in a bubble by Pressure Blast", "Pressure Blast Bubble", 0),

            new PlayerDstBuffApplyMechanic(Deterrence, "Deterrence", new MechanicPlotlySetting(Symbols.Diamond, Colors.LightRed), "Pick-up Shard", "Picked up the Bloodstone Shard", "Bloodstone Shard Pick-up", 0),
            new PlayerDstBuffApplyMechanic(BloodstoneSaturation, "Bloodstone Saturation", new MechanicPlotlySetting(Symbols.Diamond, Colors.DarkPurple), "Dispel", "Used Dispel (SAK)", "Used Dispel", 0),

            new PlayerSrcBuffRemoveFromMechanic(HardenedCrust, "Hardened Crust", new MechanicPlotlySetting(Symbols.CircleCross, Colors.White), "Dispel.Toxc", "Dispelled Toxic Geyser (Removed Hardened Crust)", "Dispelled Toxic Geyser", 0)
                .UsingChecker((brae, log) => brae.To.IsSpecies(TargetID.ToxicGeyser)),
            new PlayerSrcBuffRemoveFromMechanic(HardenedCrust, "Hardened Crust", new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.White), "Dispel.Sulf", "Dispelled Sulfuric Geyser (Removed Hardened Crust)", "Dispelled Sulfuric Geyser", 0)
                .UsingChecker((brae, log) => brae.To.IsSpecies(TargetID.SulfuricGeyser)),
            new PlayerSrcBuffRemoveFromMechanic(HardenedCrust, "Hardened Crust", new MechanicPlotlySetting(Symbols.CircleX, Colors.White), "Dispel.Titn", "Dispelled Titanspawn Geyser (Removed Hardened Crust)", "Dispelled Titanspawn Geyser", 0)
                .UsingChecker((brae, log) => brae.To.IsSpecies(TargetID.TitanspawnGeyser)),
            new PlayerSrcBuffRemoveFromMechanic(PressureBlastBubbleBuff, "Pressure Blast", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.White), "Dispel.P", "Dispelled Player (Removed Pressure Blast)", "Dispelled Player", 0)
                .UsingChecker((brae, log) => brae.To.IsPlayer),

            new EnemySrcSkillMechanic(Return, "Return", new MechanicPlotlySetting(Symbols.TriangleRightOpen, Colors.White), "Return", "Ura returned to the center", "Return", 100),

            new EnemyDstBuffApplyMechanic(Exposed31589, "Exposed", new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.LightPurple), "Exposed", "Got Exposed (Broke Breakbar)", "Exposed", 0),

            new EnemyDstBuffApplyMechanic(RisingPressure, "Rising Pressure", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.LightOrange), "Rising Pressure", "Applied Rising Pressure", "Rising Pressure", 0),
            new EnemyDstBuffApplyMechanic(TitanicResistance, "Titanic Resistance", new MechanicPlotlySetting(Symbols.TriangleUpOpen, Colors.LightOrange), "Titanic Resistance", "Applied Titanic Resistance", "Titanic Resistance", 0),
        });
        Extension = "ura";
        Icon = EncounterIconUra;
        EncounterCategoryInformation.InSubCategoryOrder = 2;
        EncounterID |= 0x000003;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayUratheSteamshrieker,
                        (1746, 1860),
                        (2550, 6200, 9010, 13082));
    }


    protected override ReadOnlySpan<int> GetTargetsIDs()
    {
        return
        [
            (int)TargetID.Ura,
            (int)TargetID.ChampionFumaroller,
            (int)TargetID.EliteFumaroller,
        ];
    }

    protected override HashSet<int> ForbidBreakbarPhasesFor()
    {
        return
        [
            (int)TargetID.ChampionFumaroller,
            (int)TargetID.EliteFumaroller,
        ];
    }

    protected override List<TargetID> GetTrashMobsIDs()
    {
        return 
        [
            TargetID.SulfuricGeyser,
            TargetID.TitanspawnGeyser,
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

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        // Toxic geysers
        var toxicEffectGUID = combatData
            .Where(x => x.IsStateChange == StateChange.EffectIDToGUID &&
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
        // titanspawn geysers
        var titanGeyserMarkerGUID = combatData
            .Where(x => x.IsStateChange == StateChange.EffectIDToGUID &&
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
                titanAgent.OverrideID(TargetID.TitanspawnGeyser, agentData);
                titanAgent.OverrideType(AgentItem.AgentType.NPC, agentData);
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
        // Bloodstone shards
        var bloodstoneShardMarkerGUID = combatData
            .Where(x => x.IsStateChange == StateChange.EffectIDToGUID &&
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
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);

        int[] curFumarollers = [1, 1];
        foreach (SingleActor target in Targets)
        {
            switch (target.ID)
            {
                case (int)TargetID.EliteFumaroller:
                    target.OverrideName("Elite " + target.Character + " " + curFumarollers[0]++);
                    break;
                case (int)TargetID.ChampionFumaroller:
                    target.OverrideName("Champion " + target.Character + " " + curFumarollers[1]++);
                    break;
                default:
                    break;
            }
        }
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor ura = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Ura)) ?? throw new MissingKeyActorsException("Ura not found"); ;
        phases[0].AddTarget(ura);
        if (!requirePhases)
        {
            return phases;
        }

        bool isCm = log.FightData.IsCM;
        long start = log.FightData.FightStart;
        long end = log.FightData.FightEnd;
        var hpUpdates = ura.GetHealthUpdates(log);

        var hp70 = hpUpdates.FirstOrDefault(x => x.Value < 70);
        var hp40 = hpUpdates.FirstOrDefault(x => x.Value < 40);
        var hp1 = log.CombatData.GetBuffData(Determined895).FirstOrDefault(x => x is BuffApplyEvent && x.To == ura.AgentItem);
        // 100-70
        var propelPhase = new PhaseData(start, hp70.Value > 0 ? hp70.Start : end, "100% - 70%");
        propelPhase.AddTarget(ura);
        phases.Add(propelPhase);
        // 70-40
        if (hp70.Value > 0)
        {
            var after70 = new PhaseData(hp70.Start, Math.Min(hp40.Start, end), "70% - 40%");
            after70.AddTarget(ura);
            phases.Add(after70);
            // 40 - 1 CM / 40 - 0 NM
            if (hp40.Value > 0)
            {
                var after40 = isCm ? new PhaseData(hp40.Start, hp1 != null ? Math.Min(hp1.Time, end) : end, "40% - 1%") : new PhaseData(hp40.Start, end, "40% - 0%");
                after40.AddTarget(ura);
                phases.Add(after40);
                var noPropelPhase = new PhaseData(after70.Start, after40.End, isCm  ? "70% - 1%" : "70% - 0%");
                noPropelPhase.AddTarget(ura);
                phases.Add(noPropelPhase);
            } 
        }
        // Healed CM
        if (hp1 != null)
        {
            var before1 = new PhaseData(start, hp1.Time, "100% - 1%");
            before1.AddTarget(ura);
            phases.Add(before1);
            var determinedLost = log.CombatData.GetBuffData(Determined895).FirstOrDefault(x => x is BuffRemoveAllEvent && x.To == ura.AgentItem && x.Time >= hp1.Time);
            if (determinedLost != null)
            {
                var after1 = new PhaseData(determinedLost.Time, end, "Healed");
                after1.AddTarget(ura);
                phases.Add(after1);
            }
        }

        return phases;
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        switch (target.ID)
        {
            case (int)TargetID.Ura:
                var casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).ToList();

                // Create Titanspawn Geyser - Ura jumps in places and creates an AoE underneath
                var ctg = casts.Where(x => x.SkillId == CreateTitanspawnGeyser);
                foreach (var cast in ctg)
                {
                    long castDuration = 3100;
                    long growing = cast.Time + castDuration;
                    (long start, long end) lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                    replay.Decorations.AddWithGrowing(new CircleDecoration(800, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target)), growing);
                }

                // Propel - Arrow Dash
                var propel = casts.Where(x => x.SkillId == Propel);
                foreach (var cast in propel)
                {
                    long castDuration = 3000;
                    (long start, long end) lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                    if (target.TryGetCurrentFacingDirection(log, cast.Time, out var facing, 1000))
                    {
                        var offset = new Vector3(250, 0, 0);
                        var rotation = new AngleConnector(facing);
                        replay.Decorations.Add(new RectangleDecoration(500, 70, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target).WithOffset(offset, true)).UsingRotationConnector(rotation));
                    }
                }

                // Slam - Cone - Third step of the attack
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.UraSlamCone, out var slams))
                {
                    foreach (EffectEvent effect in slams)
                    {
                        long duration = 2000;
                        long growing = effect.Time + duration;
                        (long start, long end) lifespan = effect.ComputeLifespan(log, duration);
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
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 10000);
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
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 6000);
                        replay.Decorations.AddWithGrowing(new CircleDecoration(900, lifespan, Colors.Sand, 0.2, new PositionConnector(effect.Position)), lifespan.end, true);
                    }
                }

                // Sulfuric Eruption - Sulfuric Geyser Shockwave Spawn
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.UraSulfuricGeyserShockwave, out var shockwaves))
                {
                    foreach (var effect in shockwaves)
                    {
                        // Looks like in game it loses velocity as it gets further away from the starting point, can't display it as velocity is unknown.
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 8000);
                        replay.Decorations.AddShockwave(new PositionConnector(effect.Position), lifespan, Colors.LightGrey, 0.5, 5000);
                    }
                }
                break;
            case (int)TargetID.ToxicGeyser:
                if (log.FightData.IsCM)
                {
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
                            (long start, long end) lifespan = effect.ComputeDynamicLifespan(log, 1200);
                            uint radius = initialRadius + (radiusIncrease * counter);
                            replay.Decorations.Add(new CircleDecoration(radius, lifespan, Colors.Red, 0.2, new AgentConnector(target)).UsingFilled(false));
                            counter++;
                        }
                    }

                    // Hardened Crust - Overhead
                    replay.Decorations.AddOverheadIcons(target.GetBuffStatus(log, HardenedCrust, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), target, BuffImages.HardenedCrust);

                    // Breakbar
                    var toxicPercentUpdates = target.GetBreakbarPercentUpdates(log);
                    var toxicStates = target.GetBreakbarStatus(log);
                    replay.Decorations.AddBreakbar(target, toxicPercentUpdates, toxicStates);
                }
                else
                {
                    // Damage field ring
                    replay.Decorations.Add(new CircleDecoration(480, (target.FirstAware, target.LastAware), Colors.Red, 0.2, new AgentConnector(target)).UsingFilled(false));
                }
                break;
            case (int)TargetID.SulfuricGeyser:
                // Hardened Crust - Overhead
                replay.Decorations.AddOverheadIcons(target.GetBuffStatus(log, HardenedCrust, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), target, BuffImages.HardenedCrust);

                // Damage field ring
                replay.Decorations.Add(new CircleDecoration(580, (target.FirstAware, target.LastAware), Colors.Red, 0.2, new AgentConnector(target)).UsingFilled(false));
                break;
            case (int)TargetID.TitanspawnGeyser:
                // Hardened Crust - Overhead
                replay.Decorations.AddOverheadIcons(target.GetBuffStatus(log, HardenedCrust, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), target, BuffImages.HardenedCrust);

                // Breakbar
                var titanspawnPercentUpdates = target.GetBreakbarPercentUpdates(log);
                var titanspawnStates = target.GetBreakbarStatus(log);
                replay.Decorations.AddBreakbar(target, titanspawnPercentUpdates, titanspawnStates);
                break;
            case (int)TargetID.ChampionFumaroller:
                // Breaking Ground - 8 pointed star
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.UraFumarollerBreakingGround, out var breakingGrounds))
                {
                    foreach (EffectEvent effect in breakingGrounds)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 20000);
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
        base.ComputePlayerCombatReplayActors(player, log, replay);

        // Deterrence - Pick-Up Bloodstone Shard
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, Deterrence, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.CrimsonAttunementOverhead);

        // Pressure Blast - Bubble AoE Indicator
        var pressureBlastTarget = player.GetBuffStatus(log, PressureBlastTargetBuff, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        foreach (var segment in pressureBlastTarget)
        {
            replay.Decorations.AddWithGrowing(new CircleDecoration(180, segment.TimeSpan, Colors.LightOrange, 0.2, new AgentConnector(player)), segment.End);
        }

        // Pressure Blast - Bubble Lift Up
        var pressureBlastBubble = player.GetBuffStatus(log, PressureBlastBubbleBuff, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
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
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(player.AgentItem, EffectGUIDs.UraSulfuricGeyserTarget, out var sulfuricGeyserTarget))
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

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log);

        // Bloodstone Radiation - AoE
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.UraBloodstoneRadiationPulse, out var bloodstoneRadiation))
        {
            foreach (var effect in bloodstoneRadiation)
            {
                // Radius is indicative only, matching the in game size. Damage is incremental hp% per pulse to everyone in the arena.
                (long start, long end) lifespan = (effect.Time, effect.Time + 500);
                EnvironmentDecorations.AddWithBorder(new CircleDecoration(300, lifespan, Colors.Red, 0.4, new PositionConnector(effect.Position)), Colors.Red, 0.4);
            }
        }
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Ura)) ?? throw new MissingKeyActorsException("Ura not found");
        if (target.GetHealth(combatData) > 70e6)
        {
            target.OverrideName("Godscream Ura");
            return FightData.EncounterMode.CMNoName;
        }
        target.OverrideName("Ura, the Streamshrieker");
        return FightData.EncounterMode.Normal;
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log)
    {
        base.SetInstanceBuffs(log);

        if (log.FightData.Success && log.FightData.IsCM)
        {
            var toxicGeysers = log.AgentData.GetNPCsByID(TargetID.ToxicGeyser);
            bool eligible = true;
            foreach (var geyser in toxicGeysers)
            {
                if (geyser.LastAware - geyser.FirstAware > 30000)
                {
                    eligible = false;
                    break;
                }
            }
            if (eligible)
            {
                InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityNoGeysersNoProblems], 1));
            }
        }
    }
}
