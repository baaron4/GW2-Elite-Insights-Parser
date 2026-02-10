using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.AchievementEligibilityIDs;
using GW2EIGW2API;

namespace GW2EIEvtcParser.LogLogic;

internal class CosmicObservatory : SecretOfTheObscureRaidEncounter
{
    public CosmicObservatory(int triggerID) : base(triggerID)
    {
        MechanicList.Add( new MechanicGroup([
        
            new MechanicGroup([
                new MechanicGroup([
                    new AchievementEligibilityMechanic(Ach_DancedStars, new MechanicPlotlySetting(Symbols.TriangleDownOpen, Colors.DarkBlue), "DancStars.Achiv.L", "Achievement Eligibility: Danced with the Stars Lost", "Danced with the Stars Lost", 0)
                            .UsingChecker((evt, log) => evt.Lost),
                    new AchievementEligibilityMechanic(Ach_DancedStars, new MechanicPlotlySetting(Symbols.TriangleDownOpen, Colors.Blue), "DancStars.Achiv.K", "Achievement Eligibility: Danced with the Stars Kept", "Danced with the Stars Kept", 0)
                            .UsingChecker((evt, log) => !evt.Lost)
                ]),
                new PlayerDstHealthDamageHitMechanic([ SpinningNebulaCentral, SpinningNebulaWithTeleport ], new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkBlue), "Spin.Neb.H", "Spining Nebula Hit (Spin Projectiles)", "Spinning Nebula Hit", 0),
                new EnemyCastStartMechanic([ SpinningNebulaCentral, SpinningNebulaWithTeleport ], new MechanicPlotlySetting(Symbols.CircleCross, Colors.LightRed), "Spinning Nebula", "Spinning Nebula Cast", "Cast Spinning Nebula", 0),
            ]),
            new PlayerDstHealthDamageHitMechanic(DemonicBlast, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Red), "Dmn.Blst.H", "Demonic Blast Hit (Cones AoEs)", "Demonic Blast Hit", 0),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(SoulFeast, new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "Sl.Fst.H", "Soul Feat (Pulsing Orb AoEs)", "Soul Feast Hit", 0),
                new PlayerDstBuffApplyMechanic(Revealed, new MechanicPlotlySetting(Symbols.Bowtie, Colors.Teal), "Sl.Fst.T", "Soul Feast Target", "Targeted by Soul Feast", 0)
                    .UsingChecker((bae, log) => bae.CreditedBy.IsSpecies(TargetID.Dagda)),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(PlanetCrashProjectileSkill, new MechanicPlotlySetting(Symbols.StarDiamond, Colors.White), "PlnCrhProj.H", "Planet Crash (Projectiles Hits)", "Planet Crash Projectiles Hit", 0),
                new EnemyCastStartMechanic(PlanetCrashSkill, new MechanicPlotlySetting(Symbols.Star, Colors.Blue), "Planet Crash", "Planet Crash Cast", "Cast Planet Crash", 0),
                new EnemyDstBuffApplyMechanic(Exposed31589, new MechanicPlotlySetting(Symbols.Star, Colors.LightBlue), "Planet Crash (Int)", "Interrupted Planet Crash", "Interrupted Planet Crash", 0)
                    .UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.Dagda)),
                new EnemySrcHealthDamageMechanic(PlanetCrashSkill, new MechanicPlotlySetting(Symbols.Star, Colors.DarkBlue), "Planet Crash (Land)", "Planet Crash Landed", "Fully Casted Planet Crash", 1000)
                    .UsingChecker((ahde, log) => ahde.HealthDamage >= 0 && ahde.To.IsPlayer),
            ]),
            new PlayerDstHealthDamageHitMechanic(ChargingConstellationDamage, new MechanicPlotlySetting(Symbols.Star, Colors.White), "ChargCons.H", "Charging Constellation Hit", "Charging Constellation Hit", 0),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(ShootingStarsTargetBuff, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Green), "StarsTarg.A", "Shooting Stars Target (Green Arrow)", "Targeted by Shooting Stars", 0),
                new EnemyCastStartMechanic(ShootingStars, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Green), "Shooting Stars", "Shooting Stars Cast", "Cast Shooting Stars", 0),
            ]),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(ResidualAnxiety, new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.Red), "Rsdl.Anxty", "Residual Anxiety", "Residual Anxiety", 0),
                new PlayerDstBuffApplyMechanic(CosmicObservatoryLostControlBuff, new MechanicPlotlySetting(Symbols.Diamond, Colors.Red), "Lst.Ctrl", "Lost Control (10 stacks of Residual Anxiety)", "Lost Control", 0),
            ]),
            new PlayerDstBuffApplyMechanic(DagdaSharedDestruction_MeteorCrash, new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "Shar.Des.T", "Targeted by Shared Destruction (Greens - Meteor Crash)", "Shared Destruction Target (Green - Meteor Crash)", 0),
            new PlayerDstBuffApplyMechanic([ TargetOrder1, TargetOrder2, TargetOrder3, TargetOrder4, TargetOrder5 ], new MechanicPlotlySetting(Symbols.Star, Colors.LightOrange), "Targ.Ord.A", "Received Target Order", "Target Order Application", 0),
            new PlayerDstBuffApplyMechanic(ExtremeVulnerability, new MechanicPlotlySetting(Symbols.X, Colors.DarkRed), "ExtVuln.A", "Applied Extreme Vulnerability", "Extreme Vulnerability Application", 0),
            new MechanicGroup([
                new PlayerSrcBuffRemoveSingleFromMechanic(DagdaDemonicAura, new MechanicPlotlySetting(Symbols.Bowtie, Colors.LightBlue), "DemAur.R", "Removed Stacks of Demonic Aura", "Demonic Aura Stacks Removed"),
                new EnemyDstBuffRemoveSingleMechanic(DagdaDemonicAura, new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.LightBlue), "DemAur.L", "Lost stacks of Demonic Aura", "Demonic Aura Stacks Lost")
                    .UsingChecker((abre, log) => abre.CreditedBy.IsPlayer),
            ]),
            new MechanicGroup([
                new PlayerSrcHealthDamageMechanic(PurifyingLight, new MechanicPlotlySetting(Symbols.Hourglass, Colors.LightBlue), "PurLight.C", "Casted Purifying Light", "Casted Purifying Light", 0),
                new PlayerSrcHealthDamageHitMechanic(PurifyingLight, new MechanicPlotlySetting(Symbols.HourglassOpen, Colors.LightBlue), "PurLight.Soul.C", "Casted Purifying Light (Hit Soul Feast)", "Purifying Light Hit Soul Feast", 0)
                    .UsingChecker((ahde, log) => ahde.To.IsSpecies(TargetID.SoulFeast)),
                new PlayerSrcHealthDamageHitMechanic(PurifyingLight, new MechanicPlotlySetting(Symbols.HourglassOpen, Colors.Blue), "PurLight.Dagda.C", "Casted Purifying Light (Hit Dagda)", "Purifying Light Hit Dagda", 0)
                    .UsingChecker((ahde, log) => ahde.To.IsSpecies(TargetID.Dagda)),
            ]),
            new PlayerDstEffectMechanic(EffectGUIDs.CosmicObservatoryDemonicFever, new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "DemFev.T", "Targeted by Demonic Fever (Orange Spread AoEs)", "Demonic Fever Target", 0),
        ])
        );
        Icon = EncounterIconCosmicObservatory;
        Extension = "cosobs";
        LogCategoryInformation.InSubCategoryOrder = 0;
        LogID |= 0x000001;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (1169, 1169),
                        (-1388, -779, 1991, 2610));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayCosmicObservatory, crMap);
        return crMap;
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
        (long start, long end) lifespan;
        var casts = target.GetAnimatedCastEvents(log).ToList();

        switch (target.ID)
        {
            case (int)TargetID.Dagda:
                var demonicBlasts = casts.Where(x => x.SkillID == DemonicBlast);

                foreach (CastEvent cast in casts)
                {
                    switch (cast.SkillID)
                    {
                        // Demonic Blast - Red AoE during 75-50-25 % phases
                        case DemonicBlast:
                            var phaseBuffs = target.GetBuffStatus(log, DagdaDuringPhase75_50_25).Where(x => x.Value > 0);
                            // Dagda uses Demonic Blast at 90% but she will not spawn the red pushback AoE
                            // We check if she has gained the buff to be sure that the phase has started.
                            var phaseBuff = phaseBuffs.Where(x => x.Start >= cast.Time).FirstOrNull();
                            if (phaseBuff is null || Math.Abs(cast.Time - phaseBuff.Value.Start) > 4000)
                            {
                                continue;
                            }
                            // Hardcoded positional value, Dagda isn't in the center but the AoE is
                            lifespan = (cast.Time, phaseBuff.Value.End);
                            var connector = new PositionConnector(new(305.26892f, 920.6105f, -5961.992f));
                            var circle = new CircleDecoration(800, lifespan, Colors.Red, 0.4, connector);
                            replay.Decorations.Add(circle);
                            break;
                        // Planet Crash - Breakbar
                        case PlanetCrashSkill:
                            lifespan = (cast.Time, cast.EndTime);
                            replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMajorSizeInPixel, lifespan, Colors.Red, 0.6, Colors.Black, 0.2, [(cast.Time, 0), (cast.Time + 10000, 100)], new AgentConnector(target))
                                .UsingRotationConnector(new AngleConnector(180)));
                            break;
                        // Spinning Nebula
                        case SpinningNebulaCentral:
                        case SpinningNebulaWithTeleport:
                            lifespan = (cast.Time, cast.Time + cast.ActualDuration);
                            replay.Decorations.AddWithGrowing(new CircleDecoration(300, lifespan, Colors.MidTeal, 0.2, new AgentConnector(target)), lifespan.end);
                            break;
                        // Shooting Stars - Green Arrow
                        case ShootingStars:
                            uint length = 1500;
                            uint width = 100;
                            int castDuration = 6700;
                            lifespan = (cast.Time, cast.Time + castDuration);

                            // The mechanic gets cancelled during the intermission phases since the CM release.
                            // Before then, the mechanic would continue during the phase and shoot.
                            if (log.LogMetadata.GW2Build >= GW2Builds.DagdaNMHPChangedAndCMRelease)
                            {
                                foreach (CastEvent demonicBlastCast in demonicBlasts)
                                {
                                    if (lifespan.start < demonicBlastCast.Time && lifespan.end > demonicBlastCast.Time)
                                    {
                                        lifespan.end = demonicBlastCast.Time;
                                        break;
                                    }
                                }
                            }

                            // Find the targeted player
                            Player? player = log.PlayerList.FirstOrDefault(x => x.HasBuff(log, ShootingStarsTargetBuff, cast.Time, ServerDelayConstant));
                            if (player != null)
                            {
                                var rotation = new AgentFacingAgentConnector(target, player);
                                var agentConnector = (AgentConnector)new AgentConnector(target).WithOffset(new(length / 2, 0, 0), true);
                                replay.Decorations.Add(new RectangleDecoration(length, width, lifespan, Colors.DarkGreen, 0.4, agentConnector).UsingRotationConnector(rotation));
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            default:
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }

        // Lost Control
        var lostControls = p.GetBuffStatus(log, CosmicObservatoryLostControlBuff).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(lostControls, p, ParserIcons.CallTarget);

        // Shooting Stars Target Overhead
        var shootingStarsTarget = p.GetBuffStatus(log, ShootingStarsTargetBuff).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(shootingStarsTarget, p, ParserIcons.TargetOverhead);

        // Target Order (CM)
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder1).Where(x => x.Value > 0), p, ParserIcons.TargetOrder1Overhead);
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder2).Where(x => x.Value > 0), p, ParserIcons.TargetOrder2Overhead);
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder3).Where(x => x.Value > 0), p, ParserIcons.TargetOrder3Overhead);
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder4).Where(x => x.Value > 0), p, ParserIcons.TargetOrder4Overhead);
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder5).Where(x => x.Value > 0), p, ParserIcons.TargetOrder5Overhead);

        // Tethering the player to the Soul Feast.
        // The buff is applied by Dagda to the player and the Soul Feast follows that player until death.
        var buffAppliesAll = log.CombatData.GetBuffApplyData(Revealed).OfType<BuffApplyEvent>().Where(x => x.CreditedBy.IsSpecies(TargetID.Dagda));
        var buffAppliesPlayer = buffAppliesAll.Where(x => x.To.Is(p.AgentItem));
        var agentsToTether = log.AgentData.GetNPCsByID(TargetID.SoulFeast);

        foreach (BuffApplyEvent buffApply in buffAppliesPlayer)
        {
            // We check for the next Revealed application instead of its end event because it can be extended by normal player skills.
            // Additionally, the spawning of Soul Feasts on a player can be interrupted when a 75-50-25 % phase start and moved to another player.                
            // Checking for the next buff application prevents cross-tethering between players and feasts.

            // The next application to any player
            BuffApplyEvent? nextApplicationEvent = buffAppliesAll.FirstOrDefault(x => x.Time > buffApply.Time);
            long endTime = nextApplicationEvent != null ? nextApplicationEvent.Time : log.LogData.EvtcLogEnd;

            foreach (AgentItem agent in agentsToTether)
            {
                // Decoration life span ends on the Soul Feast dying
                DeadEvent? deathEvent = log.CombatData.GetDeadEvents(agent).FirstOrDefault();
                var deathTime = deathEvent != null ? deathEvent.Time : agent.LastAware;
                (long, long) lifespan = (buffApply.Time, deathTime);

                // For each Soul Feast spawned, check the spawn time to be after the current buff apply and before the next
                if (agent.FirstAware >= buffApply.Time && agent.FirstAware < endTime)
                {
                    replay.Decorations.Add(new LineDecoration(lifespan, Colors.Magenta, 0.5, new AgentConnector(agent), new AgentConnector(p)));
                }
            }
        }

        // Demonic Fever - 7 Spreads
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.CosmicObservatoryDemonicFever, out var demonicFever))
        {
            foreach (EffectEvent effect in demonicFever)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                var connector = new AgentConnector(p);
                var circle = new CircleDecoration(285, lifespan, Colors.Orange, 0.2, connector);
                replay.Decorations.AddWithGrowing(circle, lifespan.Item2);
            }
        }

        // Shared Destruction - Cosmic Meteor
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.CosmicObservatorySharedDestructionCosmicMeteorGreen, out var cosmicMeteors))
        {
            foreach (EffectEvent effect in cosmicMeteors)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 6250, p.AgentItem, DagdaSharedDestruction_MeteorCrash);
                var connector = new AgentConnector(p);
                var circle = new CircleDecoration(160, lifespan, Colors.DarkGreen, 0.4, connector);
                replay.Decorations.AddWithGrowing(circle, lifespan.Item2, true);
            }
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);

        // Demonic Blast - 8 Slices
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.CosmicObservatoryDemonicBlastSliceIndicator, out var demonicBlasts))
        {
            foreach (EffectEvent effect in demonicBlasts)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 4000);
                var connector = new PositionConnector(effect.Position);
                var rotation = new AngleConnector(effect.Rotation.Z);
                // Correcting life span for the hit time, 4000 is the entire animation, 2000 looks to be correct
                lifespan.Item2 -= 2000;
                var slice = new PieDecoration(1500, 30, lifespan, Colors.Red, 0.2, connector);
                environmentDecorations.Add(slice.UsingRotationConnector(rotation));
                environmentDecorations.Add(slice.Copy().UsingGrowingEnd(lifespan.Item2).UsingRotationConnector(rotation));
            }
        }

        // Demonic Pools - Indicator
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.CosmicObservatoryDemonicPoolsIndicator, out var demonicPoolsindicators))
        {
            foreach (EffectEvent effect in demonicPoolsindicators)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                var connector = new PositionConnector(effect.Position);
                var circle = new CircleDecoration(300, lifespan, Colors.Orange, 0.2, connector);
                environmentDecorations.Add(circle);
                environmentDecorations.Add(circle.Copy().UsingGrowingEnd(lifespan.Item2));
            }
        }

        // Demonic Pools - Damage
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.CosmicObservatoryDemonicPoolsDamage, out var demonicPoolsDamage))
        {
            foreach (EffectEvent effect in demonicPoolsDamage)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 20000);
                var connector = new PositionConnector(effect.Position);
                var circle = new CircleDecoration(300, lifespan, Colors.Red, 0.2, connector);
                environmentDecorations.Add(circle);
            }
        }

        // Rain of Comets - Semicircle covering half arena
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.CosmicObservatoryRainOfComets, out var rainOfComets))
        {
            foreach (EffectEvent effect in rainOfComets)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                var connector = new PositionConnector(effect.Position);
                var rotation = new AngleConnector(effect.Rotation.Z + 90);
                var semicircle = new PieDecoration(1400, 180, lifespan, Colors.Red, 0.4, connector);
                environmentDecorations.Add(semicircle.UsingRotationConnector(rotation));
            }
        }

        // Planet Crash - Impact
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.CosmicObservatoryPlanetCrash, out var planetCrash))
        {
            foreach (EffectEvent effect in planetCrash)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 666);
                var connector = new PositionConnector(effect.Position);
                var circle = new CircleDecoration(1500, lifespan, Colors.Orange, 0.2, connector);
                environmentDecorations.Add(circle);
            }
        }

        // Purifying Light - SAK projectile from Dagda
        // TODO - Find out if the ground effect is now logged
        var purifyingLight = log.CombatData.GetMissileEventsBySkillID(DagdaPurifyingLightProjectileSkill);
        environmentDecorations.AddNonHomingMissiles(log, purifyingLight, Colors.White, 0.5, 50);

        // Spinning Nebula - Projectiles
        var spinningNebula = log.CombatData.GetMissileEventsBySkillIDs([SpinningNebulaCentral, SpinningNebulaWithTeleport]);
        environmentDecorations.AddNonHomingMissiles(log, spinningNebula, Colors.MidTeal, 0.4, 20);

        // Charging Constellation - Numbers Projectiles
        var chargingConstellation = log.CombatData.GetMissileEventsBySkillID(ChargingConstellationDamage);
        environmentDecorations.AddNonHomingMissiles(log, chargingConstellation, Colors.Blue, 0.4, 30);
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents, LogData.LogSuccessHandler successHandler)
    {
        base.CheckSuccess(combatData, agentData, logData, playerAgents, successHandler);
        // Special check since CM release, normal mode broke too, but we always trust reward events
        if (combatData.GetGW2BuildEvent().Build >= GW2Builds.DagdaNMHPChangedAndCMRelease && combatData.GetRewardEvents().FirstOrDefault(x => x.RewardType == RewardTypes.PostEoDRaidEncounterReward && x.Time > logData.LogStart) == null)
        {
            SingleActor dagda = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Dagda)) ?? throw new MissingKeyActorsException("Dagda not found");
            HealthUpdateEvent? hpUpdate = combatData.GetHealthUpdateEvents(dagda.AgentItem).FirstOrDefault(x => x.HealthPercent <= 1e-6);
            if (hpUpdate != null)
            {
                HealthDamageEvent? lastDamageEvent = combatData.GetDamageTakenData(dagda.AgentItem).LastOrDefault(x => x.HealthDamage > 0 && x.Time <= hpUpdate.Time + ServerDelayConstant);
                if (lastDamageEvent != null)
                {
                    successHandler.SetSuccess(true, successHandler.Success ? Math.Min(lastDamageEvent.Time, logData.LogEnd) : lastDamageEvent.Time);
                }
            }
        }
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Dagda)) ?? throw new MissingKeyActorsException("Dagda not found");
        phases[0].AddTarget(mainTarget, log);
        if (!requirePhases)
        {
            return phases;
        }
        // Cast check
        var tormentedIDs = new List<TargetID>()
        {
            TargetID.VeteranTheTormented,
            TargetID.EliteTheTormented,
            TargetID.ChampionTheTormented,
        };
        var tormentedAgents = new List<AgentItem>();
        foreach (TargetID tormentedID in tormentedIDs)
        {
            tormentedAgents.AddRange(log.AgentData.GetNPCsByID(tormentedID));
        }
        tormentedAgents.SortByFirstAware();
        var tormentedGroups = new List<List<AgentItem>>();
        var processedAgents = new HashSet<AgentItem>();
        foreach (AgentItem tormentedAgent in tormentedAgents)
        {
            if (processedAgents.Contains(tormentedAgent))
            {
                continue;
            }
            var group = new List<AgentItem>();
            AgentItem currentReferenceTormented = tormentedAgent;
            foreach (AgentItem tormentedAgentToBeGrouped in tormentedAgents)
            {
                if (tormentedAgentToBeGrouped.FirstAware >= currentReferenceTormented.FirstAware && tormentedAgentToBeGrouped.FirstAware <= currentReferenceTormented.LastAware)
                {
                    group.Add(tormentedAgentToBeGrouped);
                    processedAgents.Add(tormentedAgentToBeGrouped);
                    currentReferenceTormented = tormentedAgentToBeGrouped;
                }
            }
            tormentedGroups.Add(group);
        }
        var phaseTimes = new List<(long start, long end)>();
        long previousStart = log.LogData.LogStart;
        for (int i = 0; i < tormentedGroups.Count; i++)
        {
            List<AgentItem> group = tormentedGroups[i];
            long start = Math.Max(log.LogData.LogStart, group.Min(x => x.FirstAware));
            long end = Math.Min(log.LogData.LogEnd, group.Max(x =>
            {
                long res = x.LastAware;
                if (log.CombatData.GetDeadEvents(x).Any())
                {
                    res = log.CombatData.GetDeadEvents(x).Last().Time;
                }
                return res;
            }));

            phaseTimes.Add((previousStart, start));
            phaseTimes.Add((start, end));
            previousStart = end;
            if (i == tormentedGroups.Count - 1)
            {
                phaseTimes.Add((end, log.LogData.LogEnd));
            }
        }
        for (int i = 0; i < phaseTimes.Count; i++)
        {
            (long start, long end) = phaseTimes[i];
            PhaseData phase;
            if (i % 2 == 1)
            {
                phase = new SubPhasePhaseData(start, end)
                {
                    Name = "Tormenteds " + (i + 1) / 2
                };
                var ids = new List<TargetID>
                {
                    TargetID.VeteranTheTormented,
                    TargetID.EliteTheTormented,
                    TargetID.ChampionTheTormented,
                };
                AddTargetsToPhase(phase, ids, log);
            }
            else
            {
                BuffEvent? phasingBuffLoss = log.CombatData.GetBuffDataByIDByDst(DagdaDuringPhase75_50_25, mainTarget.AgentItem).FirstOrDefault(x => x.Time >= start && x.Time <= end && x is BuffRemoveAllEvent);
                if (phasingBuffLoss != null)
                {
                    start = phasingBuffLoss.Time;
                }
                phase = new SubPhasePhaseData(start, end)
                {
                    Name = "Phase " + (i + 2) / 2
                };
            }
            phase.AddParentPhase(phases[0]);
            phase.AddTarget(mainTarget, log);
            phases.Add(phase);
        }
        return phases;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        foreach (SingleActor target in Targets)
        {
            switch (target.ID)
            {
                case (int)TargetID.VeteranTheTormented:
                    target.OverrideName("Veteran " + target.Character);
                    break;
                case (int)TargetID.EliteTheTormented:
                    target.OverrideName("Elite " + target.Character);
                    break;
                case (int)TargetID.ChampionTheTormented:
                    target.OverrideName("Champion " + target.Character);
                    break;
                default:
                    break;
            }
        }
        SingleActor dagda = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Dagda)) ?? throw new MissingKeyActorsException("Dagda not found");
        // Security check to stop dagda from going back to 100%
        var dagdaHPUpdates = combatData.Where(x => x.SrcMatchesAgent(dagda.AgentItem) && x.IsStateChange == StateChange.HealthUpdate).ToList();
        if (dagdaHPUpdates.Count > 1 && HealthUpdateEvent.GetHealthPercent(dagdaHPUpdates.LastOrDefault()!) == 100)
        {
            dagdaHPUpdates.Last().OverrideDstAgent(dagdaHPUpdates[^2].DstAgent);
        }
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.Dagda,
            TargetID.ChampionTheTormented,
            TargetID.EliteTheTormented,
            TargetID.VeteranTheTormented,
        ];
    }

    internal override HashSet<TargetID> ForbidBreakbarPhasesFor()
    {
        return
        [
            TargetID.EliteTheTormented,
            TargetID.VeteranTheTormented,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.SoulFeast,
            TargetID.TheTormented,
            TargetID.TormentedPhantom,
        ];
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        if (combatData.GetGW2BuildEvent().Build < GW2Builds.DagdaNMHPChangedAndCMRelease)
        {
            return LogData.Mode.Normal;
        }
        SingleActor dagda = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Dagda)) ?? throw new MissingKeyActorsException("Dagda not found");
        return (dagda.GetHealth(combatData) > 56e6) ? LogData.Mode.CM : LogData.Mode.Normal;
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Cosmic Observatory";
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        base.SetInstanceBuffs(log, instanceBuffs);
        var encounterPhases = log.LogData.GetEncounterPhases(log).Where(x => x.ID == LogID);
        foreach (var encounterPhase in encounterPhases)
        {
            if (encounterPhase.Success && encounterPhase.IsCM)
            {
                int buffCounter = 0;
                int aliveCounter = 0;
                int playerCounter = 0;

                foreach (Player player in log.PlayerList)
                {
                    if (player.InAwareTimes(encounterPhase.Start, encounterPhase.End))
                    {
                        playerCounter++;
                        IReadOnlyDictionary<long, BuffGraph> bgms = player.GetBuffGraphs(log);
                        if (player.HasBuff(log, AchievementEligibilityPrecisionAnxiety, encounterPhase.End - ServerDelayConstant))
                        {
                            buffCounter++;
                        }
                        IReadOnlyList<DeadEvent> deaths = log.CombatData.GetDeadEvents(player.AgentItem);
                        if (deaths.Count == 0)
                        {
                            aliveCounter++;
                        }
                    }
                }
                if (playerCounter == 10 && buffCounter == playerCounter && aliveCounter == playerCounter)
                {
                    instanceBuffs.Add(new(log.Buffs.BuffsByIDs[AchievementEligibilityPrecisionAnxiety], 1, encounterPhase));
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
            var dancedStarsEligibilityEvents = new List<AchievementEligibilityEvent>();
            var coCMPhases = log.LogData.GetEncounterPhases(log).Where(x => x.ID == LogID && x.IsCM && x.IntersectsWindow(p.FirstAware, p.LastAware)).ToHashSet();
            List<HealthDamageEvent> damageData = [
                ..log.CombatData.GetDamageData(SpinningNebulaCentral),
                ..log.CombatData.GetDamageData(SpinningNebulaWithTeleport)
            ];
            damageData.SortByTime();
            foreach (var evt in damageData)
            {
                if (evt.HasHit && evt.To.Is(p.AgentItem) && p.InAwareTimes(evt.Time))
                {
                    InsertAchievementEligibityEventAndRemovePhase(coCMPhases, dancedStarsEligibilityEvents, evt.Time, Ach_DancedStars, p);
                }
            }
            AddSuccessBasedAchievementEligibityEvents(coCMPhases, dancedStarsEligibilityEvents, Ach_DancedStars, p);
            achievementEligibilityEvents.AddRange(dancedStarsEligibilityEvents);
        }
    }
}
