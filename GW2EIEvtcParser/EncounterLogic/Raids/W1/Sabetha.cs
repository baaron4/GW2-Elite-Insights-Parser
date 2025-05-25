using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Sabetha : SpiritVale
{
    public Sabetha(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
       
            // NOTE: Time Bomb damage is registered only for the user that has the bomb, damage to others is not logged.
            new PlayerDstBuffApplyMechanic(ShellShocked, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.DarkGreen), "Launched", "Shell-Shocked (launched up to cannons)","Shell-Shocked", 0),
            new PlayerDstBuffApplyMechanic(SapperBombBuff, new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "Sap Bomb", "Got a Sapper Bomb","Sapper Bomb", 0),
            new PlayerDstSkillMechanic(Firestorm, new MechanicPlotlySetting(Symbols.Square,Colors.Red), "Flamewall", "Firestorm (killed by Flamewall)","Flamewall", 0)
                .UsingChecker((de, log) => de.HasKilled),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(TimeBomb, new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Timed Bomb", "Got a Timed Bomb (Expanding circle)","Timed Bomb", 0),
                new PlayerDstSkillMechanic([TimeBombDamage, TimeBombDamage2], new MechanicPlotlySetting(Symbols.Hexagram, Colors.DarkMagenta), "TimeB Down", "Downed by Time Bomb", "Time Bomb Down", 0)
                    .UsingChecker((hde, log) => hde.HasDowned),
                new PlayerDstSkillMechanic([TimeBombDamage, TimeBombDamage2], new MechanicPlotlySetting(Symbols.HexagramOpen, Colors.DarkMagenta), "TimeB Kill", "Killed by Time Bomb", "Time Bomb Kill", 0)
                    .UsingChecker((hde, log) => hde.HasKilled),
            ]),
            new PlayerDstHitMechanic(FlakShot, new MechanicPlotlySetting(Symbols.HexagramOpen,Colors.LightOrange), "Flak", "Flak Shot (Fire Patches)","Flak Shot", 0),
            new PlayerDstHitMechanic(CannonBarrage, new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Cannon", "Cannon Barrage (stood in AoE)","Cannon Shot", 0),
            new PlayerDstHitMechanic(FlameBlast, new MechanicPlotlySetting(Symbols.TriangleLeftOpen,Colors.Yellow), "Karde Flame", "Flame Blast (Karde's Flamethrower)","Flamethrower (Karde)", 0),
            new PlayerDstHitMechanic(BanditKick, new MechanicPlotlySetting(Symbols.TriangleRight,Colors.Magenta), "Kick", "Kicked by Bandit","Bandit Kick", 0)
                .UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new PlayerCastStartMechanic(KickHeavyBomb, new MechanicPlotlySetting(Symbols.Cross, Colors.CobaltBlue), "Kick Bomb", "Kicked Heavy Bomb", "Heavy Bomb Kick", 0)
                .UsingChecker((ce, log) => !ce.IsInterrupted && !ce.IsUnknown),
            new MechanicGroup([
                new EnemyCastStartMechanic(PlatformQuake, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC", "Platform Quake (Breakbar)","Breakbar",0),
                new EnemyCastEndMechanic(PlatformQuake, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed", "Platform Quake (Breakbar broken) ","CCed", 0)
                    .UsingChecker((ce, log) => ce.ActualDuration <= 4400),
                new EnemyCastEndMechanic(PlatformQuake, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail", "Platform Quake (Breakbar failed) ","CC Fail", 0)
                    .UsingChecker( (ce,log) =>  ce.ActualDuration > 4400),
            ]),
        ]));
        Extension = "sab";
        Icon = EncounterIconSabetha;
        EncounterCategoryInformation.InSubCategoryOrder = 3;
        EncounterID |= 0x000003;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplaySabetha,
                        (1000, 990),
                        (-8587, -162, -1601, 6753)/*,
                        (-15360, -36864, 15360, 39936),
                        (3456, 11012, 4736, 14212)*/);
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        // Cannons
        var cannons = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 74700 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget);
        foreach (AgentItem cannon in cannons)
        {
            cannon.OverrideType(AgentItem.AgentType.NPC, agentData);
            cannon.OverrideID(TargetID.Cannon, agentData);
        }

        // Heavy Bombs
        var heavyBombs = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 14940 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxHeight == 300 && x.HitboxWidth == 2);
        foreach (AgentItem bomb in heavyBombs)
        {
            bomb.OverrideType(AgentItem.AgentType.NPC, agentData);
            bomb.OverrideID(TargetID.HeavyBomb, agentData);
        }

        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor sabetha = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Sabetha)) ?? throw new MissingKeyActorsException("Sabetha not found");
        phases[0].AddTarget(sabetha, log);
        var miniBossIds = new List<TargetID>
        {
             TargetID.Karde, // reverse order for mini boss phase detection
             TargetID.Knuckles,
             TargetID.Kernan,
        };
        phases[0].AddTargets(Targets.Where(x => x.IsAnySpecies(miniBossIds)), log, PhaseData.TargetPriority.Blocking);
        if (!requirePhases)
        {
            return phases;
        }
        // Invul check
        phases.AddRange(GetPhasesByInvul(log, Invulnerability757, sabetha, true, true));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.AddParentPhase(phases[0]);
            if (i % 2 == 0)
            {
                int phaseID = i / 2;
                phase.Name = "Unknown";
                foreach (var miniBossId in miniBossIds)
                {
                    AddTargetsToPhaseAndFit(phase, [miniBossId], log);
                    if (phase.Targets.Count > 0)
                    {
                        SingleActor phaseTarget = phase.Targets.Keys.First();
                        if (PhaseNames.TryGetValue(phaseTarget.ID, out var phaseName))
                        {
                            phase.Name = phaseName;
                        }
                        break; // we found our main target
                    }
                }
                AddTargetsToPhase(phase, miniBossIds, log, PhaseData.TargetPriority.NonBlocking);
            }
            else
            {
                int phaseID = (i + 1) / 2;
                phase.Name = "Phase " + phaseID;
                phase.AddTarget(sabetha, log);
                AddTargetsToPhase(phase, miniBossIds, log, PhaseData.TargetPriority.NonBlocking);
            }
        }
        return phases;
    }

    protected override ReadOnlySpan<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.Sabetha,
            TargetID.Kernan,
            TargetID.Knuckles,
            TargetID.Karde,
            TargetID.Cannon,
        ];
    }

    protected override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>()
        {
            {TargetID.Sabetha, 0 },
            {TargetID.Kernan, 1 },
            {TargetID.Knuckles, 1 },
            {TargetID.Karde, 1 },
            {TargetID.Cannon, 2 },
        };
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        long castDuration;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.Sabetha:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    switch (cast.SkillId)
                    {
                        // Firestorm - Flame Wall
                        case Firestorm:
                            uint width = 1300;
                            uint height = 60;
                            long preCastTime = 2800;
                            castDuration = 10000;
                            lifespan = (cast.Time, cast.Time + preCastTime);
                            (long start, long end) lifespanWall = (lifespan.end, lifespan.end + castDuration);
                            if (target.TryGetCurrentFacingDirection(log, cast.Time, out var facingFirestorm))
                            {
                                var positionConnector = (AgentConnector)new AgentConnector(target).WithOffset(new(width / 2, 0, 0), true);
                                replay.Decorations.Add(new RectangleDecoration(width, height, lifespan, Colors.Orange, 0.2, positionConnector).UsingRotationConnector(new AngleConnector(facingFirestorm)));
                                replay.Decorations.Add(new RectangleDecoration(width, height, lifespanWall, Colors.Red, 0.5, positionConnector).UsingRotationConnector(new SpinningConnector(facingFirestorm, 360)));
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.Kernan:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    switch (cast.SkillId)
                    {
                        // Bullet Hail - 3 Cones
                        case BulletHail:
                            uint radius = 1500;
                            (long start, long end) firstCone = (cast.Time, cast.Time + 400);
                            (long start, long end) secondCone = (cast.Time + 800, cast.Time + 800 + 400);
                            (long start, long end) thirdCone = (cast.Time + 1600, cast.Time + 1600 + 400);
                            if (target.TryGetCurrentFacingDirection(log, cast.Time, out var facingBulletHail))
                            {
                                var connector = new AgentConnector(target);
                                var rotationConnector = new AngleConnector(facingBulletHail);
                                replay.Decorations.Add(new PieDecoration(radius, 28, firstCone, Colors.LightOrange, 0.2, connector).UsingRotationConnector(rotationConnector));
                                replay.Decorations.Add(new PieDecoration(radius, 54, secondCone, Colors.LightOrange, 0.2, connector).UsingRotationConnector(rotationConnector));
                                replay.Decorations.Add(new PieDecoration(radius, 81, thirdCone, Colors.LightOrange, 0.2, connector).UsingRotationConnector(rotationConnector));
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;

            case (int)TargetID.Knuckles:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    switch (cast.SkillId)
                    {
                        // Platform Quake
                        case PlatformQuake:
                            lifespan = (cast.Time, cast.EndTime);
                            replay.Decorations.Add(new CircleDecoration(180, lifespan, Colors.LightBlue, 0.3, new AgentConnector(target)));
                            break;
                        default:
                            break;
                    }
                }
                break;

            case (int)TargetID.Karde:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    switch (cast.SkillId)
                    {
                        // Flame Blast
                        case FlameBlast:
                            castDuration = 4000;
                            lifespan = (cast.Time, cast.Time + castDuration);
                            if (target.TryGetCurrentFacingDirection(log, lifespan.start, out var facingFlameBlast))
                            {
                                replay.Decorations.Add(new PieDecoration(600, 60, lifespan, Colors.Yellow, 0.5, new AgentConnector(target)).UsingRotationConnector(new AngleConnector(facingFlameBlast)));
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.Cannon:
                if (log.CombatData.TryGetMarkerEventsBySrcWithGUID(target.AgentItem, MarkerGUIDs.SabethaCannonRedCrossSwordsMarker, out var swords))
                {
                    long hideStart = target.FirstAware;
                    foreach (var marker in swords)
                    {
                        lifespan = (marker.Time, marker.EndTime);
                        replay.Hidden.Add(new Segment(hideStart, lifespan.start));
                        hideStart = lifespan.end;
                        replay.Decorations.AddOverheadIcon(lifespan, target, ParserIcons.RedCrossSwordsMarker);
                    }
                    replay.Hidden.Add(new Segment(hideStart, target.LastAware));
                }
                break;
            default:
                break;
        }
    }

    protected override List<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.BanditSapper,
            TargetID.BanditThug,
            TargetID.BanditArsonist,
            TargetID.HeavyBomb,
        ];
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);

        // Timed bombs
        var timedBombs = p.GetBuffStatus(log, TimeBomb, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        foreach (var seg in timedBombs)
        {
            // Buff lasts 4000ms, damage event happens at 3000ms.
            (long start, long end) lifespan = (seg.Start, seg.Start + 3000);
            replay.Decorations.AddWithGrowing(new CircleDecoration(280, lifespan, Colors.LightOrange, 0.2, new AgentConnector(p)), lifespan.end);
        }

        // Sapper bombs
        var sapperBombs = p.GetBuffStatus(log, SapperBombBuff, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        foreach (var seg in sapperBombs)
        {
            long growing = seg.Start + 5000;
            replay.Decorations.AddWithGrowing(new CircleDecoration(180, seg, Colors.Lime, 0.5, new AgentConnector(p)), growing);
            replay.Decorations.AddOverheadIcon(seg, p, ParserIcons.BombOverhead);
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log);

        // Cannon Barrage
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.SabethaCannonBarrage, out var cannonBarrage))
        {
            foreach (EffectEvent effect in cannonBarrage)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 3500);
                var circle = new CircleDecoration(240, lifespan, Colors.LightOrange, 0.1, new PositionConnector(effect.Position));
                EnvironmentDecorations.AddWithGrowing(circle, lifespan.end);
            }
        }

        // Platform Crush - Platform debris falling down
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.SabethaPlatformCrush, out var platformCrush))
        {
            foreach (EffectEvent effect in platformCrush)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 1000);
                var circle = new CircleDecoration(80, lifespan, Colors.Orange, 0.3, new PositionConnector(effect.Position));
                EnvironmentDecorations.AddWithGrowing(circle, lifespan.end);
            }
        }

        // Flak Shot
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.SabethaFlakShot, out var flakShot))
        {
            foreach (EffectEvent effect in flakShot)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 25000);
                var circle = new CircleDecoration(100, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position)); // 100 radius aprox.
                EnvironmentDecorations.AddWithFilled(circle, false);
            }
        }

        // Sapper Bomb - Ground AoE (after throw)
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.SabethaSapperBombGroundAoE, out var sapperBomb))
        {
            foreach (EffectEvent effect in sapperBomb)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 1000);
                var circle = new CircleDecoration(180, lifespan, Colors.Lime, 0.5, new PositionConnector(effect.Position));
                EnvironmentDecorations.AddWithGrowing(circle, lifespan.end);
            }
        }
    }

    protected override ReadOnlySpan<TargetID> GetUniqueNPCIDs()
    {
        return
        [
            TargetID.Sabetha,
            TargetID.Kernan,
            TargetID.Karde,
            TargetID.Knuckles,
        ];
    }

    private readonly IReadOnlyDictionary<int, string> PhaseNames = new Dictionary<int, string>()
    {
        { (int)TargetID.Kernan, "Kernan" },
        { (int)TargetID.Karde, "Karde" },
        { (int)TargetID.Knuckles, "Knuckles" }
    };
}
