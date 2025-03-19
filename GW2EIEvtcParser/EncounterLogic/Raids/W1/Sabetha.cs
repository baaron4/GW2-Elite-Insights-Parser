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
            new PlayerDstBuffApplyMechanic(ShellShocked, "Shell-Shocked", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.DarkGreen), "Launched","Shell-Shocked (launched up to cannons)", "Shell-Shocked",0),
            new PlayerDstBuffApplyMechanic(SapperBombBuff, "Sapper Bomb", new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "Sap Bomb","Got a Sapper Bomb", "Sapper Bomb",0),
            new PlayerDstSkillMechanic(Firestorm, "Firestorm", new MechanicPlotlySetting(Symbols.Square,Colors.Red), "Flamewall","Firestorm (killed by Flamewall)", "Flamewall",0)
                .UsingChecker((de, log) => de.HasKilled),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(TimeBomb, "Time Bomb", new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Timed Bomb","Got a Timed Bomb (Expanding circle)", "Timed Bomb",0),
                new PlayerDstSkillMechanic([TimeBombDamage, TimeBombDamage2], "Time Bomb", new MechanicPlotlySetting(Symbols.Hexagram, Colors.DarkMagenta), "TimeB Down", "Downed by Time Bomb", "Time Bomb Down", 0)
                    .UsingChecker((hde, log) => hde.HasDowned),
                new PlayerDstSkillMechanic([TimeBombDamage, TimeBombDamage2], "Time Bomb", new MechanicPlotlySetting(Symbols.HexagramOpen, Colors.DarkMagenta), "TimeB Kill", "Killed by Time Bomb", "Time Bomb Kill", 0)
                    .UsingChecker((hde, log) => hde.HasKilled),
            ]),
            new PlayerDstHitMechanic(FlakShot, "Flak Shot", new MechanicPlotlySetting(Symbols.HexagramOpen,Colors.LightOrange), "Flak","Flak Shot (Fire Patches)", "Flak Shot",0),
            new PlayerDstHitMechanic(CannonBarrage, "Cannon Barrage", new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Cannon","Cannon Barrage (stood in AoE)", "Cannon Shot",0),
            new PlayerDstHitMechanic(FlameBlast, "Flame Blast", new MechanicPlotlySetting(Symbols.TriangleLeftOpen,Colors.Yellow), "Karde Flame","Flame Blast (Karde's Flamethrower)", "Flamethrower (Karde)",0),
            new PlayerDstHitMechanic(BanditKick, "Kick", new MechanicPlotlySetting(Symbols.TriangleRight,Colors.Magenta), "Kick","Kicked by Bandit", "Bandit Kick",0)
                .UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new PlayerCastStartMechanic(KickHeavyBomb, "Kick Heavy Bomb", new MechanicPlotlySetting(Symbols.Cross, Colors.CobaltBlue), "Kick Bomb", "Kicked Heavy Bomb", "Heavy Bomb Kick", 0)
                .UsingChecker((ce, log) => !ce.IsInterrupted && !ce.IsUnknown),
            new MechanicGroup([
                new EnemyCastStartMechanic(PlatformQuake, "Platform Quake", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC","Platform Quake (Breakbar)","Breakbar",0),
                new EnemyCastEndMechanic(PlatformQuake, "Platform Quake", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed","Platform Quake (Breakbar broken) ", "CCed",0)
                    .UsingChecker((ce, log) => ce.ActualDuration <= 4400),
                new EnemyCastEndMechanic(PlatformQuake, "Platform Quake", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail","Platform Quake (Breakbar failed) ", "CC Fail",0)
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
            cannon.OverrideID(TrashID.Cannon, agentData);
        }

        // Heavy Bombs
        var heavyBombs = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 14940 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxHeight == 300 && x.HitboxWidth == 2);
        foreach (AgentItem bomb in heavyBombs)
        {
            bomb.OverrideType(AgentItem.AgentType.NPC, agentData);
            bomb.OverrideID(TrashID.HeavyBomb, agentData);
        }

        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor sabetha = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Sabetha)) ?? throw new MissingKeyActorsException("Sabetha not found");
        phases[0].AddTarget(sabetha);
        var miniBossIds = new List<int>
        {
            (int) TrashID.Karde, // reverse order for mini boss phase detection
            (int) TrashID.Knuckles,
            (int) TrashID.Kernan,
        };
        phases[0].AddTargets(Targets.Where(x => x.IsAnySpecies(miniBossIds)), PhaseData.TargetPriority.Blocking);
        if (!requirePhases)
        {
            return phases;
        }
        // Invul check
        phases.AddRange(GetPhasesByInvul(log, Invulnerability757, sabetha, true, true));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
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
                AddTargetsToPhase(phase, miniBossIds, PhaseData.TargetPriority.NonBlocking);
            }
            else
            {
                int phaseID = (i + 1) / 2;
                phase.Name = "Phase " + phaseID;
                phase.AddTarget(sabetha);
                AddTargetsToPhase(phase, miniBossIds, PhaseData.TargetPriority.NonBlocking);
            }
        }
        return phases;
    }

    protected override ReadOnlySpan<int> GetTargetsIDs()
    {
        return
        [
            (int)TargetID.Sabetha,
            (int)TrashID.Kernan,
            (int)TrashID.Knuckles,
            (int)TrashID.Karde,
            (int)TrashID.Cannon,
        ];
    }

    protected override Dictionary<int, int> GetTargetsSortIDs()
    {
        return new Dictionary<int, int>()
        {
            {(int)TargetID.Sabetha, 0 },
            {(int)TrashID.Kernan, 1 },
            {(int)TrashID.Knuckles, 1 },
            {(int)TrashID.Karde, 1 },
            {(int)TrashID.Cannon, 2 },
        };
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        var cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
        switch (target.ID)
        {
            case (int)TargetID.Sabetha:
                var flameWall = cls.Where(x => x.SkillId == Firestorm);
                foreach (CastEvent c in flameWall)
                {
                    int start = (int)c.Time;
                    int preCastTime = 2800;
                    int duration = 10000;
                    uint width = 1300; uint height = 60;
                    if (target.TryGetCurrentFacingDirection(log, start, out var facing))
                    {
                        var positionConnector = (AgentConnector)new AgentConnector(target).WithOffset(new(width / 2, 0, 0), true);
                        replay.Decorations.Add(new RectangleDecoration(width, height, (start, start + preCastTime), Colors.Orange, 0.2, positionConnector).UsingRotationConnector(new AngleConnector(facing)));
                        replay.Decorations.Add(new RectangleDecoration(width, height, (start + preCastTime, start + preCastTime + duration), Colors.Red, 0.5, positionConnector).UsingRotationConnector(new AngleConnector(facing, 360)));
                    }
                }
                break;

            case (int)TrashID.Kernan:
                var bulletHail = cls.Where(x => x.SkillId == BulletHail);
                foreach (CastEvent c in bulletHail)
                {
                    int start = (int)c.Time;
                    int firstConeStart = start;
                    int secondConeStart = start + 800;
                    int thirdConeStart = start + 1600;
                    int firstConeEnd = firstConeStart + 400;
                    int secondConeEnd = secondConeStart + 400;
                    int thirdConeEnd = thirdConeStart + 400;
                    uint radius = 1500;
                    if (target.TryGetCurrentFacingDirection(log, start, out var facing))
                    {
                        var connector = new AgentConnector(target);
                        var rotationConnector = new AngleConnector(facing);
                        replay.Decorations.Add(new PieDecoration(radius, 28, (firstConeStart, firstConeEnd), Colors.Yellow, 0.3, connector).UsingRotationConnector(rotationConnector));
                        replay.Decorations.Add(new PieDecoration(radius, 54, (secondConeStart, secondConeEnd), Colors.Yellow, 0.3, connector).UsingRotationConnector(rotationConnector));
                        replay.Decorations.Add(new PieDecoration(radius, 81, (thirdConeStart, thirdConeEnd), Colors.Yellow, 0.3, connector).UsingRotationConnector(rotationConnector));
                    }
                }
                break;

            case (int)TrashID.Knuckles:
                var breakbar = cls.Where(x => x.SkillId == PlatformQuake);
                foreach (CastEvent c in breakbar)
                {
                    replay.Decorations.Add(new CircleDecoration(180, ((int)c.Time, (int)c.EndTime), Colors.LightBlue, 0.3, new AgentConnector(target)));
                }
                break;

            case (int)TrashID.Karde:
                var flameBlast = cls.Where(x => x.SkillId == FlameBlast);
                foreach (CastEvent c in flameBlast)
                {
                    int start = (int)c.Time;
                    int end = start + 4000;
                    uint radius = 600;
                    if (target.TryGetCurrentFacingDirection(log, start, out var facing))
                    {
                        replay.Decorations.Add(new PieDecoration(radius, 60, (start, end), Colors.Yellow, 0.5, new AgentConnector(target)).UsingRotationConnector(new AngleConnector(facing)));
                    }
                }
                break;
            case (int)TrashID.Cannon:
                if (log.CombatData.TryGetMarkerEventsBySrcWithGUID(target.AgentItem, MarkerGUIDs.SabethaCannonRedCrossSwordsMarker, out var swords))
                {
                    long hideStart = target.FirstAware;
                    foreach (var marker in swords)
                    {
                        (long start, long end) lifespan = (marker.Time, marker.EndTime);
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

    protected override List<TrashID> GetTrashMobsIDs()
    {
        return
        [
            TrashID.BanditSapper,
            TrashID.BanditThug,
            TrashID.BanditArsonist,
            TrashID.HeavyBomb,
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

    protected override ReadOnlySpan<int> GetUniqueNPCIDs()
    {
        return
        [
            (int)TargetID.Sabetha,
            (int)TrashID.Kernan,
            (int)TrashID.Karde,
            (int)TrashID.Knuckles,
        ];
    }

    private readonly IReadOnlyDictionary<int, string> PhaseNames = new Dictionary<int, string>()
    {
        { (int)TrashID.Kernan, "Kernan" },
        { (int)TrashID.Karde, "Karde" },
        { (int)TrashID.Knuckles, "Knuckles" }
    };
}
