using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class CosmicObservatory : SecretOfTheObscureStrike
{
    public CosmicObservatory(int triggerID) : base(triggerID)
    {
        MechanicList.Add( new MechanicGroup([
        
            new MechanicGroup([
                new PlayerDstHitMechanic([ SpinningNebulaCentral, SpinningNebulaWithTeleport ], "Danced with the Stars", new MechanicPlotlySetting(Symbols.TriangleDownOpen, Colors.DarkBlue), "DancStars.Achiv", "Achievement Eligibility: Danced with the Stars", "Danced with the Stars", 0)
                    .UsingEnable(x => x.FightData.IsCM)
                    .UsingAchievementEligibility(true),
                new PlayerDstHitMechanic([ SpinningNebulaCentral, SpinningNebulaWithTeleport ], "Spinning Nebula", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkBlue), "Spin.Neb.H", "Spining Nebula Hit (Spin Projectiles)", "Spinning Nebula Hit", 0),
                new EnemyCastStartMechanic([ SpinningNebulaCentral, SpinningNebulaWithTeleport ], "Spinning Nebula", new MechanicPlotlySetting(Symbols.CircleCross, Colors.LightRed), "Spinning Nebula", "Spinning Nebula Cast", "Cast Spinning Nebula", 0),
            ]),
            new PlayerDstHitMechanic(DemonicBlast, "Demonic Blast", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Red), "Dmn.Blst.H", "Demonic Blast Hit (Cones AoEs)", "Demonic Blast Hit", 0),
            new MechanicGroup([
                new PlayerDstHitMechanic(SoulFeast, "Soul Feast", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "Sl.Fst.H", "Soul Feat (Pulsing Orb AoEs)", "Soul Feast Hit", 0),
                new PlayerDstBuffApplyMechanic(Revealed, "Revealed", new MechanicPlotlySetting(Symbols.Bowtie, Colors.Teal), "Sl.Fst.T", "Soul Feast Target", "Targeted by Soul Feast", 0)
                    .UsingChecker((bae, log) => bae.CreditedBy.IsSpecies(TargetID.Dagda)),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(PlanetCrashProjectileSkill, "Planet Crash", new MechanicPlotlySetting(Symbols.StarDiamond, Colors.White), "PlnCrhProj.H", "Planet Crash (Projectiles Hits)", "Planet Crash Projectiles Hit", 0),
                new EnemyCastStartMechanic(PlanetCrashSkill, "Planet Crash", new MechanicPlotlySetting(Symbols.Star, Colors.Blue), "Planet Crash", "Planet Crash Cast", "Cast Planet Crash", 0),
                new EnemyDstBuffApplyMechanic(Exposed31589, "Planet Crash", new MechanicPlotlySetting(Symbols.Star, Colors.LightBlue), "Planet Crash (Int)", "Interrupted Planet Crash", "Interrupted Planet Crash", 0)
                    .UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.Dagda)),
                new EnemySrcSkillMechanic(PlanetCrashSkill, "Planet Crash", new MechanicPlotlySetting(Symbols.Star, Colors.DarkBlue), "Planet Crash (Land)", "Planet Crash Landed", "Fully Casted Planet Crash", 1000)
                    .UsingChecker((ahde, log) => ahde.HealthDamage >= 0 && ahde.To.IsPlayer),
            ]),
            new PlayerDstHitMechanic(ChargingConstellationDamage, "Charging Constellation", new MechanicPlotlySetting(Symbols.Star, Colors.White), "ChargCons.H", "Charging Constellation Hit", "Charging Constellation Hit", 0),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(ShootingStarsTargetBuff, "Shooting Stars", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Green), "StarsTarg.A", "Shooting Stars Target (Green Arrow)", "Targeted by Shooting Stars", 0),
                new EnemyCastStartMechanic(ShootingStars, "Shooting Stars", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Green), "Shooting Stars", "Shooting Stars Cast", "Cast Shooting Stars", 0),
            ]),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(ResidualAnxiety, "Residual Anxiety", new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.Red), "Rsdl.Anxty", "Residual Anxiety", "Residual Anxiety", 0),
                new PlayerDstBuffApplyMechanic(CosmicObservatoryLostControlBuff, "Lost Control", new MechanicPlotlySetting(Symbols.Diamond, Colors.Red), "Lst.Ctrl", "Lost Control (10 stacks of Residual Anxiety)", "Lost Control", 0),
            ]),
            new PlayerDstBuffApplyMechanic(DagdaSharedDestruction_MeteorCrash, "Shared Destruction", new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "Shar.Des.T", "Targeted by Shared Destruction (Greens - Meteor Crash)", "Shared Destruction Target (Green - Meteor Crash)", 0),
            new PlayerDstBuffApplyMechanic([ TargetOrder1, TargetOrder2, TargetOrder3, TargetOrder4, TargetOrder5 ], "Target Order", new MechanicPlotlySetting(Symbols.Star, Colors.LightOrange), "Targ.Ord.A", "Received Target Order", "Target Order Application", 0),
            new PlayerDstBuffApplyMechanic(ExtremeVulnerability, "Extreme Vulnerability", new MechanicPlotlySetting(Symbols.X, Colors.DarkRed), "ExtVuln.A", "Applied Extreme Vulnerability", "Extreme Vulnerability Application", 0),
            new MechanicGroup([
                new PlayerSrcBuffRemoveSingleFromMechanic(DagdaDemonicAura, "Demonic Aura", new MechanicPlotlySetting(Symbols.Bowtie, Colors.LightBlue), "DemAur.R", "Removed Stacks of Demonic Aura", "Demonic Aura Stacks Removed"),
                new EnemyDstBuffRemoveSingleMechanic(DagdaDemonicAura, "Demonic Aura", new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.LightBlue), "DemAur.L", "Lost stacks of Demonic Aura", "Demonic Aura Stacks Lost")
                    .UsingChecker((abre, log) => abre.CreditedBy.IsPlayer),
            ]),
            new MechanicGroup([
                new PlayerSrcSkillMechanic(PurifyingLight, "Purifying Light", new MechanicPlotlySetting(Symbols.Hourglass, Colors.LightBlue), "PurLight.C", "Casted Purifying Light", "Casted Purifying Light", 0),
                new PlayerSrcHitMechanic(PurifyingLight, "Purifying Light", new MechanicPlotlySetting(Symbols.HourglassOpen, Colors.LightBlue), "PurLight.Soul.C", "Casted Purifying Light (Hit Soul Feast)", "Purifying Light Hit Soul Feast", 0)
                    .UsingChecker((ahde, log) => ahde.To.IsSpecies(TargetID.SoulFeast)),
                new PlayerSrcHitMechanic(PurifyingLight, "Purifying Light", new MechanicPlotlySetting(Symbols.HourglassOpen, Colors.Blue), "PurLight.Dagda.C", "Casted Purifying Light (Hit Dagda)", "Purifying Light Hit Dagda", 0)
                    .UsingChecker((ahde, log) => ahde.To.IsSpecies(TargetID.Dagda))
                    .UsingEnable(x => x.FightData.IsCM),
            ]),
            new PlayerDstEffectMechanic(EffectGUIDs.CosmicObservatoryDemonicFever, "Demonic Fever", new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "DemFev.T", "Targeted by Demonic Fever (Orange Spread AoEs)", "Demonic Fever Target", 0),
        ])
        );
        Icon = EncounterIconCosmicObservatory;
        Extension = "cosobs";
        EncounterCategoryInformation.InSubCategoryOrder = 0;
        EncounterID |= 0x000001;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayCosmicObservatory,
                        (1169, 1169),
                        (-1388, -779, 1991, 2610));
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        var casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

        switch (target.ID)
        {
            case (int)TargetID.Dagda:
                var phaseBuffs = target.GetBuffStatus(log, DagdaDuringPhase75_50_25, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);

                // Red AoE during 75-50-25 % phases
                var demonicBlasts = casts.Where(x => x.SkillId == DemonicBlast);
                foreach (CastEvent cast in demonicBlasts)
                {
                    // Dagda uses Demonic Blast at 90% but she will not spawn the red pushback AoE
                    // We check if she has gained the buff to be sure that the phase has started.
                    var phaseBuff = phaseBuffs.Where(x => x.Start >= cast.Time).FirstOrNull();
                    if (phaseBuff is null || Math.Abs(cast.Time - phaseBuff.Value.Start) > 4000)
                    {
                        continue;
                    }
                    // Hardcoded positional value, Dagda isn't in the center but the AoE is
                    var connector = new PositionConnector(new(305.26892f, 920.6105f, -5961.992f));
                    var circle = new CircleDecoration(800, (cast.Time, phaseBuff.Value.End), Colors.Red, 0.4, connector);
                    replay.Decorations.Add(circle);
                }

                //
                var planetCrashes = casts.Where(x => x.SkillId == PlanetCrashSkill);
                foreach (CastEvent c in planetCrashes)
                {
                    replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMajorSizeInPixel, (c.Time, c.EndTime), Colors.Red, 0.6, Colors.Black, 0.2, [(c.Time, 0), (c.Time + 10000, 100)], new AgentConnector(target))
                        .UsingRotationConnector(new AngleConnector(180)));
                }

                // Spinning nebula
                var spinningNebulas = casts.Where(x => x.SkillId == SpinningNebulaCentral || x.SkillId == SpinningNebulaWithTeleport);
                foreach (CastEvent cast in spinningNebulas)
                {
                    (long, long) lifespan = (cast.Time, cast.Time + cast.ActualDuration);
                    var connector = new AgentConnector(target);
                    var circle = new CircleDecoration(300, lifespan, "rgba(0, 191, 255, 0.2)", connector);
                    replay.Decorations.AddWithGrowing(circle, lifespan.Item2);
                }

                // Shooting Stars - Green Arrow
                var shootingStars = casts.Where(x => x.SkillId == ShootingStars);
                foreach (CastEvent cast in shootingStars)
                {
                    uint length = 1500;
                    uint width = 100;
                    int castDuration = 6700;
                    (long, long) lifespan = (cast.Time, cast.Time + castDuration);

                    // The mechanic gets cancelled during the intermission phases since the CM release.
                    // Before then, the mechanic would continue during the phase and shoot.
                    if (log.LogData.GW2Build >= GW2Builds.DagdaNMHPChangedAndCMRelease)
                    {
                        foreach (CastEvent demonicBlastCast in demonicBlasts)
                        {
                            if (lifespan.Item1 < demonicBlastCast.Time && lifespan.Item2 > demonicBlastCast.Time)
                            {
                                lifespan.Item2 = demonicBlastCast.Time;
                                break;
                            }
                        }
                    }

                    // Find the targeted player
                    Player? player = log.PlayerList.FirstOrDefault(x => x.HasBuff(log, ShootingStarsTargetBuff, cast.Time, ServerDelayConstant));
                    if (player != null)
                    {
                        var rotation = new AgentFacingAgentConnector(target, player);
                        var connector = (AgentConnector)new AgentConnector(target).WithOffset(new(length / 2, 0, 0), true);
                        replay.Decorations.Add(new RectangleDecoration(length, width, lifespan, Colors.DarkGreen, 0.4, connector).UsingRotationConnector(rotation));
                    }
                }
                break;
            default:
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);

        // Lost Control
        var lostControls = p.GetBuffStatus(log, CosmicObservatoryLostControlBuff, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(lostControls, p, ParserIcons.CallTarget);

        // Shooting Stars Target Overhead
        var shootingStarsTarget = p.GetBuffStatus(log, ShootingStarsTargetBuff, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(shootingStarsTarget, p, ParserIcons.TargetOverhead);

        // Target Order (CM)
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder1, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), p, ParserIcons.TargetOrder1Overhead);
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder2, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), p, ParserIcons.TargetOrder2Overhead);
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder3, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), p, ParserIcons.TargetOrder3Overhead);
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder4, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), p, ParserIcons.TargetOrder4Overhead);
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder5, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), p, ParserIcons.TargetOrder5Overhead);

        // Tethering the player to the Soul Feast.
        // The buff is applied by Dagda to the player and the Soul Feast follows that player until death.
        var buffAppliesAll = log.CombatData.GetBuffData(Revealed).OfType<BuffApplyEvent>().Where(x => x.CreditedBy.IsSpecies(TargetID.Dagda));
        var buffAppliesPlayer = buffAppliesAll.Where(x => x.To == p.AgentItem);
        var agentsToTether = log.AgentData.GetNPCsByID(TargetID.SoulFeast);

        foreach (BuffApplyEvent buffApply in buffAppliesPlayer)
        {
            // We check for the next Revealed application instead of its end event because it can be extended by normal player skills.
            // Additionally, the spawning of Soul Feasts on a player can be interrupted when a 75-50-25 % phase start and moved to another player.                
            // Checking for the next buff application prevents cross-tethering between players and feasts.

            // The next application to any player
            BuffApplyEvent? nextApplicationEvent = buffAppliesAll.FirstOrDefault(x => x.Time > buffApply.Time);
            long endTime = nextApplicationEvent != null ? nextApplicationEvent.Time : log.FightData.LogEnd;

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

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log);

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
                EnvironmentDecorations.Add(slice.UsingRotationConnector(rotation));
                EnvironmentDecorations.Add(slice.Copy().UsingGrowingEnd(lifespan.Item2).UsingRotationConnector(rotation));
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
                EnvironmentDecorations.Add(circle);
                EnvironmentDecorations.Add(circle.Copy().UsingGrowingEnd(lifespan.Item2));
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
                EnvironmentDecorations.Add(circle);
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
                EnvironmentDecorations.Add(semicircle.UsingRotationConnector(rotation));
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
                EnvironmentDecorations.Add(circle);
            }
        }
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        base.CheckSuccess(combatData, agentData, fightData, playerAgents);
        // Special check since CM release, normal mode broke too, but we always trust reward events
        if (combatData.GetGW2BuildEvent().Build >= GW2Builds.DagdaNMHPChangedAndCMRelease && combatData.GetRewardEvents().FirstOrDefault(x => x.RewardType == RewardTypes.PostEoDStrikeReward && x.Time > fightData.FightStart) == null)
        {
            SingleActor dagda = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Dagda)) ?? throw new MissingKeyActorsException("Dagda not found");
            HealthUpdateEvent? hpUpdate = combatData.GetHealthUpdateEvents(dagda.AgentItem).FirstOrDefault(x => x.HealthPercent <= 1e-6);
            if (hpUpdate != null)
            {
                HealthDamageEvent? lastDamageEvent = combatData.GetDamageTakenData(dagda.AgentItem).LastOrDefault(x => x.HealthDamage > 0 && x.Time <= hpUpdate.Time + ServerDelayConstant);
                if (lastDamageEvent != null)
                {
                    if (fightData.Success)
                    {
                        fightData.SetSuccess(true, Math.Min(lastDamageEvent.Time, fightData.FightEnd));
                    }
                    else
                    {
                        fightData.SetSuccess(true, lastDamageEvent.Time);
                    }
                }
            }
        }
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Dagda)) ?? throw new MissingKeyActorsException("Dagda not found");
        phases[0].AddTarget(mainTarget);
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
        long previousStart = log.FightData.FightStart;
        for (int i = 0; i < tormentedGroups.Count; i++)
        {
            List<AgentItem> group = tormentedGroups[i];
            long start = Math.Max(log.FightData.FightStart, group.Min(x => x.FirstAware));
            long end = Math.Min(log.FightData.FightEnd, group.Max(x =>
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
                phaseTimes.Add((end, log.FightData.FightEnd));
            }
        }
        for (int i = 0; i < phaseTimes.Count; i++)
        {
            (long start, long end) = phaseTimes[i];
            PhaseData phase;
            if (i % 2 == 1)
            {
                phase = new PhaseData(start, end)
                {
                    Name = "Tormenteds " + (i + 1) / 2
                };
                var ids = new List<int>
                {
                    (int)TargetID.VeteranTheTormented,
                    (int)TargetID.EliteTheTormented,
                    (int)TargetID.ChampionTheTormented,
                };
                AddTargetsToPhase(phase, ids);
            }
            else
            {
                BuffEvent? phasingBuffLoss = log.CombatData.GetBuffDataByIDByDst(DagdaDuringPhase75_50_25, mainTarget.AgentItem).FirstOrDefault(x => x.Time >= start && x.Time <= end && x is BuffRemoveAllEvent);
                if (phasingBuffLoss != null)
                {
                    start = phasingBuffLoss.Time;
                }
                phase = new PhaseData(start, end)
                {
                    Name = "Phase " + (i + 2) / 2
                };
            }
            phase.AddTarget(mainTarget);
            phases.Add(phase);
        }
        return phases;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        int[] curTormenteds = [1, 1, 1, 1];
        foreach (SingleActor target in Targets)
        {
            switch (target.ID)
            {
                case (int)TargetID.TheTormented:
                    target.OverrideName(target.Character + " " + curTormenteds[0]++);
                    break;
                case (int)TargetID.VeteranTheTormented:
                    target.OverrideName("Veteran " + target.Character + " " + curTormenteds[1]++);
                    break;
                case (int)TargetID.EliteTheTormented:
                    target.OverrideName("Elite " + target.Character + " " + curTormenteds[2]++);
                    break;
                case (int)TargetID.ChampionTheTormented:
                    target.OverrideName("Champion " + target.Character + " " + curTormenteds[3]++);
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

    protected override ReadOnlySpan<int> GetTargetsIDs()
    {
        return
        [
            (int)TargetID.Dagda,
            (int)TargetID.ChampionTheTormented,
            (int)TargetID.EliteTheTormented,
            (int)TargetID.VeteranTheTormented,
        ];
    }

    protected override List<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.SoulFeast,
            TargetID.TheTormented,
            TargetID.TormentedPhantom,
        ];
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        if (combatData.GetGW2BuildEvent().Build < GW2Builds.DagdaNMHPChangedAndCMRelease)
        {
            return FightData.EncounterMode.Normal;
        }
        SingleActor dagda = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Dagda)) ?? throw new MissingKeyActorsException("Dagda not found");
        return (dagda.GetHealth(combatData) > 56e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Cosmic Observatory";
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log)
    {
        base.SetInstanceBuffs(log);

        if (log.FightData.Success && log.FightData.IsCM)
        {
            var check = log.CombatData.GetBuffData(AchievementEligibilityPrecisionAnxiety).Count;
            int buffCounter = 0;
            int aliveCounter = 0;

            foreach (Player player in log.PlayerList)
            {
                IReadOnlyDictionary<long, BuffGraph> bgms = player.GetBuffGraphs(log);
                if (bgms != null && bgms.TryGetValue(AchievementEligibilityPrecisionAnxiety, out var bgm))
                {
                    if (bgm.Values.Any(x => x.Value == 1))
                    {
                        buffCounter++;
                    }
                }
                IReadOnlyList<DeadEvent> deaths = log.CombatData.GetDeadEvents(player.AgentItem);
                if (deaths.Count == 0)
                {
                    aliveCounter++;
                }
            }

            if (buffCounter == log.PlayerList.Count && aliveCounter == log.PlayerList.Count)
            {
                InstanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, AchievementEligibilityPrecisionAnxiety));
            }
        }
    }
}
