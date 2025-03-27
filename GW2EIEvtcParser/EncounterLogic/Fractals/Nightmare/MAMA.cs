using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class MAMA : Nightmare
{
    public MAMA(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup(
        [
            new PlayerDstHitMechanic([Blastwave1, Blastwave2], new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "KB", "Blastwave (Spinning Knockback)","KB Spin", 0),
            new PlayerDstHitMechanic(TantrumMAMA, new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.Green), "Tantrum", "Tantrum (Double hit or Slams)","Dual Spin/Slams", 700),
            new PlayerDstHitMechanic(Leap, new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Red), "Jump", "Leap (<33% only)","Leap", 0),
            new PlayerDstHitMechanic(ShootGreenBalls, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Brown), "Shoot", "Toxic Shoot (Green Bullets)","Toxic Shoot", 0),
            new PlayerDstHitMechanic(ExplosiveImpact, new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Knight Jump", "Explosive Impact (Knight Jump)","Knight Jump", 0),
            new PlayerDstHitMechanic(SweepingStrikes, new MechanicPlotlySetting(Symbols.BowtieOpen,Colors.Red), "Sweep", "Swings (Many rapid front spins)","Sweeping Strikes", 200),
            new PlayerDstHitMechanic(NightmareMiasmaMAMA, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Magenta), "Goo", "Nightmare Miasma (Poison Puddle)","Poison Goo", 700),
            new PlayerDstHitMechanic([GrenadeBarrage, GrenadeBarrage2], new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Barrage", "Grenade Barrage (Red Bullets with AoEs)","Ball Barrage", 0),
            new PlayerDstHitMechanic([ShootRedBalls, ShootRedBalls2], new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Ball", "Shoot (Direct Red Bullets)","Bullet", 0),
            new PlayerDstHitMechanic(Extraction, new MechanicPlotlySetting(Symbols.Bowtie,Colors.LightOrange), "Pull", "Extraction (Knight Pull Circle)","Knight Pull", 0),
            new PlayerDstHitMechanic([HomingGrenades, HomingGrenades2], new MechanicPlotlySetting(Symbols.StarTriangleDownOpen,Colors.Red), "Grenades", "Homing Grenades","Homing Grenades", 0),
            new PlayerDstHitMechanic([CascadeOfTorment1, CascadeOfTorment2], new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Rings", "Cascade of Torment (Alternating Rings)","Rings", 0),
            new PlayerDstHitMechanic(KnightsGaze, new MechanicPlotlySetting(Symbols.SquareOpen,Colors.LightPurple), "Daze", "Knight's Daze","Daze", 0),
            new PlayerDstSkillMechanic([NightmareDevastation1, NightmareDevastation3, NightmareDevastation4], new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Blue), "Bubble", "Nightmare Devastation (not stood in Arkk's Shield)", "Bubble", 0),
        ]));
        Extension = "mama";
        Icon = EncounterIconMAMA;
        EncounterCategoryInformation.InSubCategoryOrder = 0;
        EncounterID |= 0x000001;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayMAMA,
                        (664, 407),
                        (1653, 4555, 5733, 7195)/*,
                        (-6144, -6144, 9216, 9216),
                        (11804, 4414, 12444, 5054)*/);
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return FightData.EncounterMode.CMNoName;
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = base.GetFightOffset(evtcVersion, fightData, agentData, combatData);
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

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mama = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.MAMA)) ?? throw new MissingKeyActorsException("MAMA not found");
        phases[0].AddTarget(mama);
        var knightIds = new List<TargetID>
        {
            TargetID.GreenKnight,
            TargetID.RedKnight,
            TargetID.BlueKnight,
        };
        phases[0].AddTargets(Targets.Where(x => x.IsAnySpecies(knightIds)), PhaseData.TargetPriority.Blocking);
        if (!requirePhases)
        {
            return phases;
        }
        phases.AddRange(GetPhasesByInvul(log, Determined762, mama, true, true));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.AddParentPhase(phases[0]);
            if (i % 2 == 0)
            {
                AddTargetsToPhaseAndFit(phase, knightIds, log);
                if (phase.Targets.Count > 0)
                {
                    SingleActor phaseTar = phase.Targets.Keys.First();
                    phase.Name = PhaseNames.TryGetValue(phaseTar.ID, out string? phaseName) ? phaseName : "Unknown";
                }
            }
            else
            {
                phase.Name = "Phase " + (i + 1) / 2;
                phase.AddTarget(mama);
            }
        }
        return phases;
    }

    protected override ReadOnlySpan<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.MAMA,
            TargetID.GreenKnight,
            TargetID.RedKnight,
            TargetID.BlueKnight
        ];
    }
    protected override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>()
        {
            {TargetID.MAMA, 0 },
            {TargetID.GreenKnight, 1 },
            {TargetID.RedKnight, 1 },
            {TargetID.BlueKnight, 1 },
        };
    }

    protected override List<TargetID> GetTrashMobsIDs()
    {
        var trashIDs = base.GetTrashMobsIDs();
        trashIDs.Add(TargetID.TwistedHorror);
        return trashIDs;
    }

    private static readonly Dictionary<int, string> PhaseNames = new()
    {
        { (int)TargetID.GreenKnight, "Green Knight" },
        { (int)TargetID.RedKnight, "Red Knight" },
        { (int)TargetID.BlueKnight, "Blue Knight" }
    };

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        var casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

        switch (target.ID)
        {
            case (int)TargetID.MAMA:
                // Blastwave - AoE Knockback
                var blastwave = casts.Where(x => x.SkillId == Blastwave1 || x.SkillId == Blastwave2);
                foreach (CastEvent c in blastwave)
                {
                    int castDuration = 2750;
                    long expectedEndCast = c.Time + castDuration;
                    (long start, long end) lifespan = (c.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));

                    if (c.SkillId == Blastwave1)
                    {
                        replay.Decorations.AddWithBorder(new CircleDecoration(530, lifespan, Colors.Orange, 0.2, new AgentConnector(target)).UsingGrowingEnd(expectedEndCast), 0, Colors.Red, 0.2, false);
                    }
                    else if (c.SkillId == Blastwave2)
                    {
                        replay.Decorations.AddWithBorder(new CircleDecoration(480, lifespan, Colors.Orange, 0.2, new AgentConnector(target)).UsingGrowingEnd(expectedEndCast), 0, Colors.Red, 0.2, false);
                    }
                }

                // Leap with shockwaves
                var leap = casts.Where(x => x.SkillId == Leap);
                foreach (CastEvent c in leap)
                {
                    int castDuration = 2400;
                    long expectedEndCast = c.Time + castDuration;
                    (long start, long end) lifespan = (c.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));

                    // Find position at the end of the leap time
                    if (target.TryGetCurrentPosition(log, expectedEndCast + 1000, out var targetPosition))
                    {
                        replay.Decorations.AddWithGrowing(new CircleDecoration(350, lifespan, Colors.Orange, 0.2, new PositionConnector(targetPosition)), expectedEndCast);

                        // 3 rounds of decorations for the 3 waves
                        if (lifespan.end == expectedEndCast)
                        {
                            uint shockwaveRadius = 1300;
                            int duration = 2680;
                            for (int i = 0; i < 3; i++)
                            {
                                long shockWaveStart = expectedEndCast + i * 120;
                                (long, long) lifespanShockwave = (shockWaveStart, shockWaveStart + duration);
                                GeographicalConnector connector = new PositionConnector(targetPosition);
                                replay.Decorations.AddShockwave(connector, lifespanShockwave, Colors.Yellow, 0.3, shockwaveRadius);
                            }
                        }
                    }
                }
                break;
            case (int)TargetID.BlueKnight:
            case (int)TargetID.RedKnight:
            case (int)TargetID.GreenKnight:
                // Explosive Launch - Knight Jump in air
                var explosiveLaunch = casts.Where(x => x.SkillId == ExplosiveLaunch);
                foreach (CastEvent c in explosiveLaunch)
                {
                    int castDuration = 1714;
                    long expectedEndCast = c.Time + castDuration;
                    (long start, long end) lifespan = (c.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));
                    replay.Decorations.AddWithGrowing(new CircleDecoration(600, lifespan, Colors.Orange, 0.2, new AgentConnector(target)), expectedEndCast, true);
                }

                // Explosive Impact - Knight fall and knockback AoE
                var explosiveImpact = casts.Where(x => x.SkillId == ExplosiveImpact);
                foreach (CastEvent c in explosiveImpact)
                {
                    int castDuration = 533;
                    long expectedEndCast = c.Time + castDuration;
                    (long start, long end) lifespan = (c.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));
                    replay.Decorations.AddWithGrowing(new CircleDecoration(600, lifespan, Colors.Orange, 0.2, new AgentConnector(target)), expectedEndCast);
                }

                // Pull AoE
                var extraction = casts.Where(x => x.SkillId == Extraction);
                foreach (CastEvent c in extraction)
                {
                    int castDuration = 3835;
                    long expectedEndCast = c.Time + castDuration;
                    (long start, long end) lifespan = (c.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));
                    replay.Decorations.AddWithGrowing(new DoughnutDecoration(300, 2000, lifespan, Colors.Orange, 0.2, new AgentConnector(target)), expectedEndCast);
                }
                break;
            default:
                break;
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log);

        // Nightmare Miasma AoE Indicator
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.NightmareMiasmaIndicator, out var miasmaIndicators))
        {
            foreach (EffectEvent effect in miasmaIndicators)
            {
                // Effect duration is slightly too long, reducing it from 3300 to 3000
                (long start, long end) lifespan = (effect.Time, effect.Time + 3000);
                var circle = new CircleDecoration(540, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                EnvironmentDecorations.Add(circle);
                EnvironmentDecorations.Add(circle.Copy().UsingGrowingEnd(lifespan.end));
            }
        }

        // Nightmare Miasma AoE Damage
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.NightmareMiasmaDamage, out var miasmaDamage))
        {
            foreach (EffectEvent effect in miasmaDamage)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 76800);
                EnvironmentDecorations.Add(new CircleDecoration(540, lifespan, Colors.Chocolate, 0.2, new PositionConnector(effect.Position)));
            }
        }

        // Arkk's Shield
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.ArkkShieldIndicator, out var shieldEffects))
        {
            foreach (EffectEvent effect in shieldEffects)
            {
                int duration = 6400;
                (long start, long end) lifespan = (effect.Time, effect.Time + duration);
                var positionConnector = new PositionConnector(effect.Position);
                var arkkShield = new CircleDecoration(300, lifespan, Colors.Blue, 0.4, positionConnector);
                var doughnutHit = (DoughnutDecoration)new DoughnutDecoration(300, 5000, lifespan, Colors.Red, 0.2, positionConnector).UsingGrowingEnd(lifespan.end, true);
                EnvironmentDecorations.Add(arkkShield);
                EnvironmentDecorations.Add(doughnutHit);
            }
        }

        // Grenade Barrage
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.MAMAGrenadeBarrageIndicator, out var effects))
        {
            foreach (EffectEvent effect in effects)
            {
                (long start, long end) lifespan = (effect.Time, effect.Time + effect.Duration);
                var positionConnector = new PositionConnector(effect.Position);
                var circleIndicator = (CircleDecoration)new CircleDecoration(140, lifespan, Colors.Orange, 0.2, positionConnector).UsingFilled(false);
                var circleFiller = (CircleDecoration)new CircleDecoration(140, lifespan, Colors.Orange, 0.2, positionConnector).UsingGrowingEnd(lifespan.end);
                EnvironmentDecorations.Add(circleIndicator);
                EnvironmentDecorations.Add(circleFiller);
            }
        }

        // Cascade Of Torment
        AddCascadeOfTormentDecoration(log, EnvironmentDecorations, EffectGUIDs.CascadeOfTormentRing0, 0, 150);
        AddCascadeOfTormentDecoration(log, EnvironmentDecorations, EffectGUIDs.CascadeOfTormentRing1, 150, 250);
        AddCascadeOfTormentDecoration(log, EnvironmentDecorations, EffectGUIDs.CascadeOfTormentRing2, 250, 350);
        AddCascadeOfTormentDecoration(log, EnvironmentDecorations, EffectGUIDs.CascadeOfTormentRing3, 350, 450);
        AddCascadeOfTormentDecoration(log, EnvironmentDecorations, EffectGUIDs.CascadeOfTormentRing4, 450, 550);
        AddCascadeOfTormentDecoration(log, EnvironmentDecorations, EffectGUIDs.CascadeOfTormentRing5, 550, 650);
    }
}
