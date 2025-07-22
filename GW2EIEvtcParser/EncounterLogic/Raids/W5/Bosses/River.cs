using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class River : HallOfChains
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([

            new PlayerDstHealthDamageHitMechanic(BombShellRiverOfSouls, new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "Bomb Hit","Hit by Hollowed Bomber Exlosion", "Hit by Bomb", 0 ),
            new PlayerDstHealthDamageHitMechanic(SoullessTorrent, new MechanicPlotlySetting(Symbols.Square,Colors.Orange), "Stun Bomb", "Stunned by Soulless Torrent (Mini Bomb)", "Stun Bomb", 0)
                .UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ServerDelayConstant)),
            new EnemySrcHealthDamageHitMechanic(BombShellRiverOfSouls, new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "Bomb Hit Desmina", "Hollowed Bomber hit Desmina", "Bomb Desmina", 0)
                .UsingChecker((de, log) => de.To.IsSpecies(TargetID.Desmina)),
            new EnemySrcHealthDamageHitMechanic(EnervatorDamageSkillToDesmina, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.GreenishYellow), "Tether Desmina", "Enervator tethers and damages Desmina", "Enervator Tether", 0)
                .UsingChecker((de, log) => de.To.IsSpecies(TargetID.Desmina)),
        ]);
    public River(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        ChestID = ChestID.ChestOfSouls;
        Extension = "river";
        Targetless = true;
        Icon = EncounterIconRiver;
        EncounterCategoryInformation.InSubCategoryOrder = 1;
        EncounterID |= 0x000002;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayRiver,
                        (1000, 387),
                        (-12201, -4866, 7742, 2851)/*,
                        (-21504, -12288, 24576, 12288),
                        (19072, 15484, 20992, 16508)*/);
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.Enervator,
            TargetID.HollowedBomber,
            TargetID.RiverOfSouls,
            TargetID.SpiritHorde1,
            TargetID.SpiritHorde2,
            TargetID.SpiritHorde3
        ];
    }
    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        return
        [
            TargetID.Desmina
        ];
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = GetGenericFightOffset(fightData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            IReadOnlyList<AgentItem> enervators = agentData.GetNPCsByID(TargetID.Enervator);
            if (!enervators.Any())
            {
                throw new MissingKeyActorsException("Enervators not found");
            }
            AgentItem enervator = enervators.MinBy(x => x.FirstAware);
            if (enervator != null)
            {
                startToUse = enervator.FirstAware;
            }
        }
        return startToUse;
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        if (!agentData.TryGetFirstAgentItem(TargetID.Desmina, out var desmina))
        {
            throw new MissingKeyActorsException("Desmina not found");
        }
        if (combatData.HasMovementData)
        {
            var desminaEncounterStartPosition = new Vector2(-9239.706f, 635.445435f/*, -813.8115f*/);
            var positions = combatData.GetMovementData(desmina).Where(x => x is PositionEvent pe && pe.Time < desmina.FirstAware + MinimumInCombatDuration).Select(x => x.GetPoint3D());
            if (!positions.Any(x => x.X < desminaEncounterStartPosition.X + 100 && x.X > desminaEncounterStartPosition.X - 1300))
            {
                return FightData.EncounterStartStatus.Late;
            }
        }
        return FightData.EncounterStartStatus.Normal;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "River of Souls", Spec.NPC, (int)TargetID.DummyTarget, true);
        foreach (var desmina in agentData.GetNPCsByID(TargetID.Desmina))
        {
            var positions = combatData.Where(x => x.IsStateChange == StateChange.Position && x.SrcMatchesAgent(desmina)).Take(5).Select(x => new PositionEvent(x, agentData).GetParametricPoint3D());
            if (positions.Any(x => x.XYZ.X >= 7500))
            {
                desmina.OverrideID(IgnoredSpecies, agentData);
            }
        }
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
    }

    internal override FightLogic AdjustLogic(AgentData agentData, List<CombatItem> combatData, EvtcParserSettings parserSettings)
    {
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        // Handle potentially wrongly associated logs
        if (logStartNPCUpdate != null)
        {
            if (agentData.GetNPCsByID(TargetID.BrokenKing).Any(brokenKing => combatData.Any(evt => evt.IsDamagingDamage() && evt.DstMatchesAgent(brokenKing))))
            {
                return new StatueOfIce((int)TargetID.BrokenKing);
            }
            if (agentData.GetNPCsByID(TargetID.EaterOfSouls).Any(soulEater => combatData.Any(evt => evt.IsDamagingDamage() && evt.DstMatchesAgent(soulEater))))
            {
                return new StatueOfDeath((int)TargetID.EaterOfSouls);
            }
            if (agentData.GetNPCsByID(TargetID.EyeOfFate).Any(eyeOfFate => combatData.Any(evt => evt.IsDamagingDamage() && evt.DstMatchesAgent(eyeOfFate))))
            {
                return new StatueOfDarkness((int)TargetID.EyeOfFate);
            }
            if (agentData.GetNPCsByID(TargetID.EyeOfJudgement).Any(eyeOfJudgement => combatData.Any(evt => evt.IsDamagingDamage() && evt.DstMatchesAgent(eyeOfJudgement))))
            {
                return new StatueOfDarkness((int)TargetID.EyeOfJudgement);
            }
            if (agentData.GetNPCsByID(TargetID.Dhuum).Any(dhuum => combatData.Any(evt => evt.IsDamagingDamage() && (evt.DstMatchesAgent(dhuum) || evt.SrcMatchesAgent(dhuum)))))
            {
                return new Dhuum((int)TargetID.Dhuum);
            }
        }
        return base.AdjustLogic(agentData, combatData, parserSettings);
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);

        (long start, long end) lifespan;

        // Soulless Torrent - Player Bomb AoE - Inner Circle
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.RiverSoullessTorrentInnerPlayerAoE, out var soullessTorrentInner))
        {
            foreach (EffectEvent effect in soullessTorrentInner)
            {
                lifespan = effect.ComputeLifespan(log, 3000);
                replay.Decorations.AddWithGrowing(new CircleDecoration(70, lifespan, Colors.LightOrange, 0.2, new AgentConnector(p)), lifespan.end);
            }
        }

        // Soulless Torrent - Player Bomb AoE - Outer Circle
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.RiverSoullessTorrentOuterPlayerAoE, out var soullessTorrentOuter))
        {
            foreach (EffectEvent effect in soullessTorrentOuter)
            {
                // The effect duration is 4000 but at 3000 it snaps off the player to the ground.
                // The ground effect is added in Environment Decorations.
                lifespan = (effect.Time, effect.Time + 3000);
                replay.Decorations.Add(new CircleDecoration(100, lifespan, Colors.LightOrange, 0.2, new AgentConnector(p)));
            }
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        long castDuration;
        long growing;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.Desmina:
                var asylums = target.GetBuffStatus(log, FollowersAsylum).Where(x => x.Value > 0);
                foreach (var asylum in asylums)
                {
                    replay.Decorations.Add(new CircleDecoration(300, asylum, Colors.LightBlue, 0.2, new AgentConnector(target)));
                }
                break;

            case (int)TargetID.HollowedBomber:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    switch (cast.SkillID)
                    {
                        case BombShellRiverOfSouls:
                            castDuration = 3500;
                            growing = cast.Time + castDuration;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                            var circle = new CircleDecoration(480, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target));
                            replay.Decorations.AddWithGrowing(circle, growing);
                            break;
                        default:
                            break;
                    }
                }

                ParametricPoint3D firstBomberMovement = replay.Velocities.FirstOrDefault(x => x.XYZ != default);
                if (firstBomberMovement.XYZ != default)
                {
                    replay.Trim(firstBomberMovement.Time - 1000, replay.TimeOffsets.end);
                }
                break;
            case (int)TargetID.RiverOfSouls:
                ParametricPoint3D firstRiverMovement = replay.Velocities.FirstOrDefault(x => x.XYZ != default);
                if (firstRiverMovement.XYZ != default)
                {
                    replay.Trim(firstRiverMovement.Time - 1000, replay.TimeOffsets.end);
                }

                if (replay.Rotations.Count != 0)
                {
                    lifespan = (replay.TimeOffsets.start, replay.TimeOffsets.end);
                    replay.Decorations.Add(new RectangleDecoration(160, 390, lifespan, Colors.Orange, 0.5, new AgentConnector(target)).UsingRotationConnector(new AgentFacingConnector(target)));
                }
                break;

            case (int)TargetID.Enervator:
                // Tether between Enervator and Desmina
                // We use the damage events due to the lack of buff or effect applications between the two NPCs.
                var damageEvents = log.CombatData.GetDamageData(target.AgentItem).Where(x => x.To.IsSpecies(TargetID.Desmina));
                foreach (var dmg in damageEvents)
                {
                    long damageInterval = 600; // The damage is applied every 600ms
                    lifespan = (dmg.Time, dmg.Time + damageInterval);
                    replay.Decorations.Add(new LineDecoration(lifespan, Colors.GreenishYellow, 0.5, new AgentConnector(dmg.To), new AgentConnector(dmg.CreditedFrom)).WithThickess(15, true));
                }
                break;
            case (int)TargetID.SpiritHorde1:
            case (int)TargetID.SpiritHorde2:
            case (int)TargetID.SpiritHorde3:
                break;
            default:
                break;
        }

    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);

        // Soulless Torrent - Player Bomb AoE - Ground Circle & Damage effect
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.RiverSoullessTorrentLightningStrikeDamage, out var soullessTorrentLightnings))
        {
            foreach (var effect in soullessTorrentLightnings)
            {
                (long start, long end) lifespanLightning = effect.ComputeLifespan(log, 1000);
                (long start, long end) lifespanIndicator = (lifespanLightning.start - 1000, lifespanLightning.start);
                environmentDecorations.Add(new CircleDecoration(100, lifespanIndicator, Colors.LightOrange, 0.2, new PositionConnector(effect.Position)));
                environmentDecorations.Add(new CircleDecoration(100, lifespanLightning, Colors.CobaltBlue, 0.2, new PositionConnector(effect.Position)));
            }
        }
    }
    internal override int GetTriggerID()
    {
        return (int)TargetID.Desmina;
    }
    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "River of Souls";
    }
}
