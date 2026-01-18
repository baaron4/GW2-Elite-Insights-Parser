using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using GW2EIGW2API;

namespace GW2EIEvtcParser.LogLogic;

internal class StatueOfDeath : HallOfChains
{
    internal readonly MechanicGroup Mechanics = new([
            new PlayerDstHealthDamageHitMechanic(HungeringMiasma, new MechanicPlotlySetting(Symbols.TriangleLeftOpen,Colors.DarkGreen), "Vomit", "Hungering Miasma (Vomit Goo)","Vomit Dmg", 0),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(ReclaimedEnergyBuff, new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Light Orb Collected", "Applied when taking a light orb","Light Orb", 0),
                new PlayerCastStartMechanic(ReclaimedEnergySkill, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Light Orb Thrown", "Has thrown a light orb","Light Orb Thrown", 0)
                    .UsingChecker((evt, log) => !evt.IsInterrupted),
            ]),
        ]);


    public StatueOfDeath(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "souleater";
        Icon = EncounterIconStatueOfDeath;
        LogCategoryInformation.InSubCategoryOrder = 2;
        LogID |= 0x000004;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (710, 709),
                        (1306, -9381, 4720, -5968));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayStatueOfDeath, crMap);
        return crMap;
    }
    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(HungeringAura , HungeringAura ),
        ];
    }
    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.OrbSpider,
            TargetID.SpiritHorde1,
            TargetID.SpiritHorde2,
            TargetID.SpiritHorde3,
            TargetID.GreenSpirit1,
            TargetID.GreenSpirit2
        ];
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        if (!agentData.TryGetFirstAgentItem(TargetID.EaterOfSouls, out var eaterOfSouls))
        {
            throw new MissingKeyActorsException("Eater of Souls not found");
        }
        long startToUse = GetGenericLogOffset(logData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            var peasants = new List<AgentItem>(agentData.GetNPCsByID(TargetID.AscalonianPeasant1));
            peasants.AddRange(agentData.GetNPCsByID(TargetID.AscalonianPeasant2));
            if (peasants.Count != 0)
            {
                startToUse = peasants.Max(x => x.LastAware);
            }
            else
            {
                startToUse = GetFirstDamageEventTime(logData, agentData, combatData, eaterOfSouls);
            }
        }
        return startToUse;
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
        (long start, long end) lifespan = (replay.TimeOffsets.start, replay.TimeOffsets.end);

        switch (target.ID)
        {
            case (int)TargetID.EaterOfSouls:
                {
                    foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                    {
                        switch (cast.SkillID)
                        {
                            case Imbibe:
                                replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMajorSizeInPixel, (cast.Time, cast.EndTime), Colors.Red, 0.6, Colors.Black, 0.2, [(cast.Time, 0), (cast.ExpectedEndTime, 100)], new AgentConnector(target))
                                .UsingRotationConnector(new AngleConnector(180)));
                                break;
                            case HungeringMiasma:
                                int cascading = 1500;
                                int duration = 15000 + cascading;
                                lifespan = (cast.Time + 2100, cast.Time + 2100 + duration);
                                uint radius = 900;
                                if (target.TryGetCurrentFacingDirection(log, lifespan.start, out var facing) && target.TryGetCurrentPosition(log, lifespan.start, out var position))
                                {
                                    replay.Decorations.Add(new PieDecoration(radius, 60, lifespan, Colors.GreenishYellow, 0.5, new PositionConnector(position)).UsingGrowingEnd(lifespan.start + cascading).UsingRotationConnector(new AngleConnector(facing)));
                                }
                                break;
                            case PseudoDeathEaterOfSouls:
                                lifespan = (cast.Time, cast.EndTime);
                                replay.Decorations.Add(new CircleDecoration(180, lifespan, "rgba(255, 180, 220, 0.7)", new AgentConnector(target)).UsingGrowingEnd(lifespan.end));
                                break;
                            default:
                                break;
                        }
                    }
                }
                break;
            case (int)TargetID.GreenSpirit1:
            case (int)TargetID.GreenSpirit2:
                {
                    foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                    {
                        switch (cast.SkillID)
                        {
                            case GreensEaterofSouls:
                                long greenDuration = 5000;
                                lifespan = (cast.Time + 667, cast.Time + 667 + greenDuration);
                                replay.Decorations.AddWithGrowing(new CircleDecoration(240, lifespan, Colors.Green, 0.2, new AgentConnector(target)), lifespan.end);
                                break;
                            default:
                                break;
                        }
                    }
                }
                break;
            case (int)TargetID.SpiritHorde1:
            case (int)TargetID.SpiritHorde2:
            case (int)TargetID.SpiritHorde3:
            case (int)TargetID.OrbSpider:
                break;
            default:
                break;
        }

    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }

        (long start, long end) lifespan;

        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EaterOfSoulsSpiritOrbs, out var orbEffectEvents))
        {
            foreach (EffectEvent effectEvent in orbEffectEvents)
            {
                lifespan = effectEvent.ComputeDynamicLifespan(log, 0);
                // The size of the orb seems to be radius 100 but it looks way too big in the replay, 50 is more than enough
                environmentDecorations.Add(new CircleDecoration(50, lifespan, Colors.Purple, 0.4, new PositionConnector(effectEvent.Position)));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EaterOfSoulsSpiderWeb, out var webEffectEvents))
        {
            foreach (EffectEvent effectEvent in webEffectEvents)
            {
                lifespan = effectEvent.ComputeLifespan(log, effectEvent.Duration);
                uint webRadius = 320;
                var webIndicator = new CircleDecoration(webRadius, lifespan, Colors.Orange, 0.1, new PositionConnector(effectEvent.Position));
                var web = new CircleDecoration(webRadius, (lifespan.end, lifespan.end + 750), Colors.Orange, 0.3, new PositionConnector(effectEvent.Position));
                environmentDecorations.Add(webIndicator.GetBorderDecoration(Colors.Orange, 0.3));
                environmentDecorations.AddWithGrowing(webIndicator, lifespan.end);
                environmentDecorations.Add(web);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EaterOfSoulsLightOrbOnGround, out var orbOnGroundEffectEvents))
        {
            foreach (EffectEvent effectEvent in orbOnGroundEffectEvents)
            {
                lifespan = effectEvent.ComputeDynamicLifespan(log, 0);
                environmentDecorations.Add(new CircleDecoration(80, lifespan, Colors.Yellow, 0.6, new PositionConnector(effectEvent.Position)));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EaterOfSoulsLightOrbThrowHitGround, out var orbThrowEffectEvents))
        {
            foreach (EffectEvent effectEvent in orbThrowEffectEvents)
            {
                lifespan = (effectEvent.Time, effectEvent.Time + 500);
                environmentDecorations.Add(new CircleDecoration(40, lifespan, Colors.Yellow, 0.6, new PositionConnector(effectEvent.Position)));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EaterOfSoulsSpiritShockwave2, out var shockwaveEffectEvents))
        {
            foreach (EffectEvent effectEvent in shockwaveEffectEvents)
            {
                lifespan = effectEvent.ComputeLifespan(log, 3600);
                environmentDecorations.AddShockwave(new PositionConnector(effectEvent.Position), lifespan, Colors.Red, 0.3, 1400);
            }
        }

        // Yellow orbs thrown by players
        var orbs2 = log.CombatData.GetMissileEventsBySkillID(ReclaimedEnergySkill);
        environmentDecorations.AddNonHomingMissiles(log, orbs2, Colors.Yellow, 0.3, 80);

        // Collection orbs by eater
        var orbs = log.CombatData.GetMissileEventsBySkillID(PseudoDeathEaterOfSouls);
        environmentDecorations.AddNonHomingMissiles(log, orbs, Colors.Purple, 0.4, 50);
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
        var spiritTransform = p.GetBuffPresenceStatus(log, MortalCoilStatueOfDeath).Where(x => x.Value > 0);
        foreach (var c in spiritTransform)
        {
            int duration = 30000;
            // Progress Bar
            replay.Decorations.Add(new OverheadProgressBarDecoration(
                CombatReplayOverheadProgressBarMinorSizeInPixel, c, Colors.CobaltBlue, 0.6,
                Colors.Black, 0.2, [(c.Start, 0), (c.Start + duration, 100)], new AgentConnector(p))
                .UsingRotationConnector(new AngleConnector(130)));
            // Overhead Icon
            replay.Decorations.AddRotatedOverheadIcon(c, p, ParserIcons.GenericGreenArrowUp, 40f);
        }
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

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents, LogData.LogSuccessHandler successHandler)
    {
        NoBouncyChestGenericCheckSucess(combatData, agentData, logData, playerAgents, successHandler);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Statue of Death";
    }
}
