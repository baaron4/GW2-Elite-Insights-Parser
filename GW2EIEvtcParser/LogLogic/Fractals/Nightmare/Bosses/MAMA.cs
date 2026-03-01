using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class MAMA : Nightmare
{
    internal readonly MechanicGroup Mechanics = new(
        [
            new PlayerDstHealthDamageHitMechanic([Blastwave1, Blastwave2], new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "KB", "Blastwave (Spinning Knockback)","KB Spin", 0),
            new PlayerDstHealthDamageHitMechanic(TantrumMAMA, new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.Green), "Tantrum", "Tantrum (Double hit or Slams)","Dual Spin/Slams", 700),
            new PlayerDstHealthDamageHitMechanic(Leap, new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Red), "Jump", "Leap (<33% only)","Leap", 0),
            new PlayerDstHealthDamageHitMechanic(ShootGreenBalls, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Brown), "Shoot", "Toxic Shoot (Green Bullets)","Toxic Shoot", 0),
            new PlayerDstHealthDamageHitMechanic(ExplosiveImpact, new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Knight Jump", "Explosive Impact (Knight Jump)","Knight Jump", 0),
            new PlayerDstHealthDamageHitMechanic(SweepingStrikes, new MechanicPlotlySetting(Symbols.BowtieOpen,Colors.Red), "Sweep", "Swings (Many rapid front spins)","Sweeping Strikes", 200),
            new PlayerDstHealthDamageHitMechanic(NightmareMiasmaMAMA, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Magenta), "Goo.M", "Nightmare Miasma (Poison Puddle)","Poison Goo", 700),
            new PlayerDstHealthDamageHitMechanic([GrenadeBarrage, GrenadeBarrage2], new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Barrage", "Grenade Barrage (Red Bullets with AoEs)","Ball Barrage", 0),
            new PlayerDstHealthDamageHitMechanic([ShootRedBalls, ShootRedBalls2], new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Ball", "Shoot (Direct Red Bullets)","Bullet", 0),
            new PlayerDstHealthDamageHitMechanic(Extraction, new MechanicPlotlySetting(Symbols.Bowtie,Colors.LightOrange), "Pull.K", "Extraction (Knight Pull Circle)","Knight Pull", 0),
            new PlayerDstHealthDamageHitMechanic([HomingGrenades, HomingGrenades2], new MechanicPlotlySetting(Symbols.StarTriangleDownOpen,Colors.Red), "Grenades", "Homing Grenades","Homing Grenades", 0),
            new PlayerDstHealthDamageHitMechanic(KnightsGaze, new MechanicPlotlySetting(Symbols.SquareOpen,Colors.LightPurple), "Daze", "Knight's Daze","Daze", 0),
            new PlayerDstHealthDamageMechanic([NightmareDevastation1, NightmareDevastation3, NightmareDevastation4], new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Blue), "Bubble.M", "Nightmare Devastation (not stood in Arkk's Shield)", "Bubble", 0),
        ]);
    public MAMA(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "mama";
        Icon = EncounterIconMAMA;
        LogCategoryInformation.InSubCategoryOrder = 0;
        LogID |= 0x000001;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (664, 407),
                        (1653, 4555, 5733, 7195));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayMAMA, crMap);
        return crMap;
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.Mode.CMNoName;
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = base.GetLogOffset(evtcVersion, logData, agentData, combatData);
        if (evtcVersion.Build >= ArcDPSBuilds.NewLogStart)
        {
            // players may enter combat with knights or an invisible hitbox before
            AgentItem? mama = agentData.GetNPCsByID(TargetID.MAMA).FirstOrDefault();
            if (mama != null)
            {
                // attempt to use mama combat enter to determine start
                // logs that start with an outgoing/incoming hit on mama may not have combat enter for mama
                // but logs that start with an earlier player combat enter should
                CombatItem? enterCombat = combatData.FirstOrDefault(x => x.SrcMatchesAgent(mama) && x.IsStateChange == StateChange.EnterCombat && x.Time >= startToUse);
                if (enterCombat != null)
                {
                    return enterCombat.Time;
                }
            }
        }
        return startToUse;
    }

    internal static IReadOnlyList<SubPhasePhaseData> ComputePhases(ParsedEvtcLog log, SingleActor mama, IReadOnlyList<SingleActor> targets, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var knightIDs = KnightPhases.Select(pair => pair.Item1).ToList();
        var phases = new List<SubPhasePhaseData>(7);
        phases.AddRange(GetSubPhasesByInvul(log, Determined762, mama, true, true, encounterPhase.Start, encounterPhase.End));
        for (int i = 0; i < phases.Count; i++)
        {
            var phase = phases[i];
            var index = i + 1;
            phase.AddParentPhase(encounterPhase);
            if (index % 2 == 0)
            {
                AddTargetsToPhaseAndFit(phase, targets, knightIDs, log);
                phase.Name = "Split " + index / 2;
                if (phase.Targets.Count > 0)
                {
                    foreach (var (species, name) in KnightPhases)
                    {
                        if (phase.Targets.Keys.Any(target => target.IsSpecies(species)))
                        {
                            phase.Name = name;
                            break;
                        }
                    }
                }
            }
            else
            {
                phase.Name = "Phase " + (index + 1) / 2;
                phase.AddTarget(mama, log);
            }
        }
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mama = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.MAMA)) ?? throw new MissingKeyActorsException("MAMA not found");
        phases[0].AddTarget(mama, log);
        var knightIDs = KnightPhases.Select(pair => pair.Item1).ToList();
        phases[0].AddTargets(Targets.Where(x => x.IsAnySpecies(knightIDs)), log, PhaseData.TargetPriority.Blocking);
        phases.AddRange(ComputePhases(log, mama, Targets, (EncounterPhaseData)phases[0], requirePhases));
        return phases;
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.MAMA,
            TargetID.GreenKnight,
            TargetID.RedKnight,
            TargetID.BlueKnight
        ];
    }
    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>
        {
            {TargetID.MAMA, 0 },
            {TargetID.GreenKnight, 1 },
            {TargetID.RedKnight, 1 },
            {TargetID.BlueKnight, 1 },
        };
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        var trashIDs = new List<TargetID>(1 + base.GetTrashMobsIDs().Count);
        trashIDs.AddRange(base.GetTrashMobsIDs());
        trashIDs.Add(TargetID.TwistedHorror);
        return trashIDs;
    }

    internal static readonly List<(TargetID, string)> KnightPhases =
    [
        (TargetID.BlueKnight, "Blue Knight"),
        (TargetID.GreenKnight, "Green Knight"),
        (TargetID.RedKnight, "Red Knight"),
    ];

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
            case (int)TargetID.MAMA:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Blastwave - AoE Knockback
                        case Blastwave1:
                            castDuration = 2750;
                            growing = cast.Time + castDuration;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                            replay.Decorations.AddWithBorder(new CircleDecoration(530, lifespan, Colors.Orange, 0.2, new AgentConnector(target)).UsingGrowingEnd(growing), 0, Colors.Red, 0.2, false);
                            break;
                        case Blastwave2:
                            castDuration = 2750;
                            growing = cast.Time + castDuration;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                            replay.Decorations.AddWithBorder(new CircleDecoration(480, lifespan, Colors.Orange, 0.2, new AgentConnector(target)).UsingGrowingEnd(growing), 0, Colors.Red, 0.2, false);
                            break;
                        // Leap with shockwaves
                        case Leap:
                            castDuration = 2400;
                            growing = cast.Time + castDuration;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));

                            // Find position at the end of the leap time
                            if (target.TryGetCurrentPosition(log, growing + 1000, out var targetPosition))
                            {
                                replay.Decorations.AddWithGrowing(new CircleDecoration(350, lifespan, Colors.Orange, 0.2, new PositionConnector(targetPosition)), growing);

                                // 3 rounds of decorations for the 3 waves
                                if (lifespan.end == growing)
                                {
                                    uint shockwaveRadius = 1300;
                                    int duration = 2680;
                                    for (int i = 0; i < 3; i++)
                                    {
                                        long shockWaveStart = growing + i * 120;
                                        (long, long) lifespanShockwave = (shockWaveStart, shockWaveStart + duration);
                                        GeographicalConnector connector = new PositionConnector(targetPosition);
                                        replay.Decorations.AddShockwave(connector, lifespanShockwave, Colors.Yellow, 0.3, shockwaveRadius);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.BlueKnight:
            case (int)TargetID.RedKnight:
            case (int)TargetID.GreenKnight:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Explosive Launch - Knight Jump in air
                        case ExplosiveLaunch:
                            castDuration = 1714;
                            growing = cast.Time + castDuration;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                            replay.Decorations.AddWithGrowing(new CircleDecoration(600, lifespan, Colors.Orange, 0.2, new AgentConnector(target)), growing, true);
                            break;
                        // Explosive Impact - Knight fall and knockback AoE
                        case ExplosiveImpact:
                            castDuration = 533;
                            growing = cast.Time + castDuration;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                            replay.Decorations.AddWithGrowing(new CircleDecoration(600, lifespan, Colors.Orange, 0.2, new AgentConnector(target)), growing);
                            break;
                        // Extraction - Pull AoE
                        case Extraction:
                            castDuration = 3835;
                            growing = cast.Time + castDuration;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                            replay.Decorations.AddWithGrowing(new DoughnutDecoration(300, 2000, lifespan, Colors.Orange, 0.2, new AgentConnector(target)), growing);
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
    }
    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }

        (long start, long end) lifespan;

        // Nightmare Miasma AoE Indicator
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.NightmareMiasmaIndicator, out var miasmaIndicators))
        {
            foreach (EffectEvent effect in miasmaIndicators)
            {
                // Effect duration is slightly too long, reducing it from 3300 to 3000
                lifespan = (effect.Time, effect.Time + 3000);
                var circle = new CircleDecoration(540, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.AddWithGrowing(circle, lifespan.end);
            }
        }

        // Nightmare Miasma AoE Damage
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.NightmareMiasmaDamage, out var miasmaDamage))
        {
            foreach (EffectEvent effect in miasmaDamage)
            {
                lifespan = effect.ComputeLifespan(log, 76800);
                environmentDecorations.Add(new CircleDecoration(540, lifespan, Colors.Chocolate, 0.2, new PositionConnector(effect.Position)));
            }
        }

        // Arkk's Shield
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.ArkkShieldIndicator, out var shieldEffects))
        {
            foreach (EffectEvent effect in shieldEffects)
            {
                int duration = 6400;
                lifespan = (effect.Time, effect.Time + duration);
                var positionConnector = new PositionConnector(effect.Position);
                var arkkShield = new CircleDecoration(300, lifespan, Colors.Blue, 0.4, positionConnector);
                var doughnutHit = (DoughnutDecoration)new DoughnutDecoration(300, 5000, lifespan, Colors.Red, 0.2, positionConnector).UsingGrowingEnd(lifespan.end, true);
                environmentDecorations.Add(arkkShield);
                environmentDecorations.Add(doughnutHit);
            }
        }

        // Grenade Barrage
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.MAMAGrenadeBarrageIndicator, out var effects))
        {
            foreach (EffectEvent effect in effects)
            {
                lifespan = (effect.Time, effect.Time + effect.Duration);
                var positionConnector = new PositionConnector(effect.Position);
                if (!log.CombatData.HasMissileData)
                {
                    var circle = new CircleDecoration(140, lifespan, Colors.LightOrange, 0.2, positionConnector);
                    environmentDecorations.AddWithFilledWithGrowing(circle, false, lifespan.end);
                }
                else
                {
                    var circle = new CircleDecoration(140, lifespan, Colors.LightOrange, 0.2, positionConnector);
                    environmentDecorations.Add(circle);
                }
            }
        }

        // Shoot Orbs - Field
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.MAMAShootGreenOrbField, out var shootField))
        {
            foreach (EffectEvent effect in shootField)
            {
                lifespan = effect.ComputeDynamicLifespan(log, 12000);
                environmentDecorations.Add(new CircleDecoration(160, lifespan, Colors.DarkGreen, 0.3, new PositionConnector(effect.Position)));
            }
        }

        // Cascade Of Torment
        AddCascadeOfTormentDecoration(log, environmentDecorations, EffectGUIDs.CascadeOfTormentRing0, 0, 150);
        AddCascadeOfTormentDecoration(log, environmentDecorations, EffectGUIDs.CascadeOfTormentRing1, 150, 250);
        AddCascadeOfTormentDecoration(log, environmentDecorations, EffectGUIDs.CascadeOfTormentRing2, 250, 350);
        AddCascadeOfTormentDecoration(log, environmentDecorations, EffectGUIDs.CascadeOfTormentRing3, 350, 450);
        AddCascadeOfTormentDecoration(log, environmentDecorations, EffectGUIDs.CascadeOfTormentRing4, 450, 550);
        AddCascadeOfTormentDecoration(log, environmentDecorations, EffectGUIDs.CascadeOfTormentRing5, 550, 650);

        // Grenade Barrage Orbs
        var grenadeBarrage = log.CombatData.GetMissileEventsBySkillIDs([GrenadeBarrage, GrenadeBarrage2]);
        environmentDecorations.AddNonHomingMissiles(log, grenadeBarrage, Colors.Red, 0.4, 50);

        // Shoot Orbs
        var shootRedOrbs = log.CombatData.GetMissileEventsBySkillIDs([ShootRedBalls, ShootRedBalls2]);
        var shootGreenOrbs = log.CombatData.GetMissileEventsBySkillID(ShootGreenBalls);
        environmentDecorations.AddNonHomingMissiles(log, shootRedOrbs, Colors.Red, 0.4, 50);
        environmentDecorations.AddNonHomingMissiles(log, shootGreenOrbs, Colors.DarkGreen, 0.4, 50);
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
    }
    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }
    }
}
