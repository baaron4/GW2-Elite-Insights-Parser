using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using GW2EIGW2API;

namespace GW2EIEvtcParser.LogLogic;

internal class StatueOfIce : HallOfChains
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup(
        [
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(KingsWrathConeAoE, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.White), "Cone AoE", "Hit by King's Wrath (Cone AoEs)", "King's Wrath Cone AoE Hit", 0),
                new PlayerDstHealthDamageHitMechanic(KingsWrathConeShards, new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.LightBlue), "Cone Shards", "Hit by King's Wrath (Frontal Cone Shards)", "King's Wrath Cone Shards Hit", 0),
            ]),
            new PlayerDstHealthDamageHitMechanic(NumbingBreach, new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.LightBlue), "Cracks", "Stood on Numbing Breach (Ice Cracks in the Ground)", "Cracks", 0),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(FrozenWind, new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Green), "Green", "Frozen Wind (Stood in Green)", "Green Stack", 0),
                new PlayerDstBuffApplyMechanic(Glaciate, new MechanicPlotlySetting(Symbols.Square, Colors.Purple), "Glaciate", "Glaciated (Frozen by 4th Stack of Frozen Wind)", "Glaciate", 0),
                new EnemySrcEffectMechanic(EffectGUIDs.BrokenKingIceBreakerGreenExplosion, new MechanicPlotlySetting(Symbols.CircleX, Colors.DarkGreen), "Ice Breaker", "Hailstorm Explosion (Missed Green)", "Ice Breaker (Green Missed)", 0),
            ]),
        ]);
    public StatueOfIce(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "brokenking";
        Icon = EncounterIconStatueOfIce;
        LogCategoryInformation.InSubCategoryOrder = 2;
        LogID |= 0x000003;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (999, 890),
                        (2497, 5388, 7302, 9668));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayStatueOfIce, crMap);
        return crMap;
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        if (!agentData.TryGetFirstAgentItem(TargetID.BrokenKing, out var brokenKing))
        {
            throw new MissingKeyActorsException("Broken King not found");
        }
        long startToUse = GetGenericLogOffset(logData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            CombatItem? initialCast = combatData.FirstOrDefault(x => x.StartCasting() && x.SkillID == BrokenKingFirstCast && x.SrcMatchesAgent(brokenKing));
            startToUse = initialCast?.Time ?? GetFirstDamageEventTime(logData, agentData, combatData, brokenKing);
        }
        return startToUse;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
        var green = log.CombatData.GetBuffDataByIDByDst(FrozenWind, p.AgentItem).Where(x => x is BuffApplyEvent);
        foreach (BuffEvent c in green)
        {
            int duration = 45000;
            BuffEvent? removedBuff = log.CombatData.GetBuffRemoveAllDataByIDByDst(FrozenWind, p.AgentItem).FirstOrDefault(x => x.Time > c.Time && x.Time < c.Time + duration);
            (long start, long end) lifespan = (c.Time, c.Time + duration);
            if (removedBuff != null)
            {
                lifespan.end = removedBuff.Time;
            }
            replay.Decorations.Add(new CircleDecoration(100, lifespan, "rgba(100, 200, 255, 0.25)", new AgentConnector(p)));
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
        switch (target.ID)
        {
            case (int)TargetID.BrokenKing:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        case KingsWrathConeShards:
                            (long start, long end) lifespan = (cast.Time, cast.EndTime);
                            if (target.TryGetCurrentFacingDirection(log, lifespan.start + 1000, out var facing))
                            {
                                uint range = 450;
                                int angle = 100;
                                var connector = new AgentConnector(target);
                                var rotationConnector = new AngleConnector(facing);
                                replay.Decorations.Add(new PieDecoration(range, angle, lifespan, Colors.LightBlue, 0.2, connector).UsingRotationConnector(rotationConnector));
                                replay.Decorations.Add(new PieDecoration(range, angle, (lifespan.start + 1900, lifespan.end), Colors.LightBlue, 0.3, connector).UsingRotationConnector(rotationConnector));
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

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }

        // Numbing Breach - Cracks - White smoke indicator
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.BrokenKingNumbingBreachIndicator, out var cracksIndicators))
        {
            foreach (EffectEvent effect in cracksIndicators)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 1000);
                var connector = new PositionConnector(effect.Position);
                var circle = new CircleDecoration(115, lifespan, "rgba(219, 233, 244, 0.2)", connector);
                environmentDecorations.Add(circle);
            }
        }

        // Numbing Breach - Cracks - Damage zone
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.BrokenKingNumbingBreachDamage, out var cracks))
        {
            foreach (EffectEvent effect in cracks)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 30000);
                var connector = new PositionConnector(effect.Position);
                var rotationConnector = new AngleConnector(effect.Rotation.Z);
                var rectangle = (RectangleDecoration)new RectangleDecoration(40, 230, lifespan, "rgba(66, 130, 253, 0.2)", connector).UsingRotationConnector(rotationConnector);
                environmentDecorations.Add(rectangle);
            }
        }

        // Hailstorm - Greens
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.BrokenKingHailstormGreen, out var greens))
        {
            foreach (EffectEvent green in greens)
            {
                Color color = Colors.DarkGreen;

                // Ice Breaker - Failed Greens
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.BrokenKingIceBreakerGreenExplosion, out var failedGreens))
                {
                    EffectEvent? failedGreen = failedGreens.FirstOrDefault(x => (x.Position - green.Position).XY().Length() < 1e-6 && Math.Abs(x.Time - green.Time - 15000) <= 650);
                    if (failedGreen != null)
                    {
                        color = Colors.DarkRed;
                    }
                }

                (long, long) lifespan = green.ComputeLifespan(log, 15000);
                var circle = new CircleDecoration(120, lifespan, color, 0.4, new PositionConnector(green.Position));
                environmentDecorations.Add(circle);
                environmentDecorations.Add(circle.Copy().UsingGrowingEnd(lifespan.Item2, true));
            }
        }
    }
    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(BitingAura, BitingAura),
            new EffectCastFinder(Hailstorm, EffectGUIDs.BrokenKingHailstormGreen),
            new EffectCastFinder(IceBreaker, EffectGUIDs.BrokenKingIceBreakerGreenExplosion)
                .UsingAgentRedirectionIfUnknown((int)TargetID.BrokenKing),
        ];
    }
    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        NoBouncyChestGenericCheckSucess(combatData, agentData, logData, playerAgents);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Statue of Ice";
    }
}
