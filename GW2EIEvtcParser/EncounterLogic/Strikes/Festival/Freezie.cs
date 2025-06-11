using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Freezie : FestivalStrikeMissionLogic
{
    public Freezie(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(AuroraBeamTargetBuff, new MechanicPlotlySetting(Symbols.Star, Colors.Purple), "AuroraBeam.T", "Targeted by Aurora Beam", "Aurora Beam Target", 0),
                new PlayerDstHitMechanic([AuroraBeam1, AuroraBeam2, AuroraBeam3], new MechanicPlotlySetting(Symbols.StarDiamond, Colors.Purple), "AuroraBeam.H", "Hit by Aurora Beam", "Aurora Beam Hit", 0),
                new EnemyCastStartMechanic([AuroraBeam1, AuroraBeam2, AuroraBeam3], new MechanicPlotlySetting(Symbols.StarDiamondOpen, Colors.Purple), "AuroraBeam.C", "Casted Aurora Beam", "Aurora Beam Cast", 0),
            ]),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(GiantSnowballFreezieTargetBuff1, new MechanicPlotlySetting(Symbols.Star, Colors.Purple, 5), "GiantSnowball.T", "Targeted by Giant Snowball", "Giant Snowball Target", 0),
                new PlayerDstHitMechanic(GiantSnowballFreezieDamage, new MechanicPlotlySetting(Symbols.Circle, Colors.White), "GiantSnowball.H", "Hit by Giant Snowball", "Giant Snowball Hit", 0),
                new EnemyCastStartMechanic(GiantSnowballFreezieCast, new MechanicPlotlySetting(Symbols.CircleOpen, Colors.White), "GiantSnowball.C", "Casted Giant Snowball", "Giant Snowball Cast", 0),
            ]),
            new PlayerDstHitMechanic(Blizzard, new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Orange), "Blizzard.H", "Hit by Blizzard (Outer circle)", "Blizzard Hit", 0),
            new MechanicGroup([
                new PlayerDstHitMechanic(FrostPatchDamage, new MechanicPlotlySetting(Symbols.Octagon, Colors.Blue), "FrostPatch.H", "Hit by Frost Patch (Cracks)", "Frost Patch Hit (Cracks)", 0),
                new EnemyCastStartMechanic(FrostPatchSkill, new MechanicPlotlySetting(Symbols.Octagon, Colors.LightBlue), "FrostPatch.C", "Casted Frost Patch", "Frost Patch Cast", 0),
            ]),
            new PlayerDstHitMechanic([JuttingIceSpikes1, JuttingIceSpikes2], new MechanicPlotlySetting(Symbols.TriangleDown, Colors.LightGrey), "IceSpike.H", "Hit by Jutting Ice Spike", "Jutting Ice Spike Hit", 0),
            new PlayerCastStartMechanic(FireSnowball, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.White), "Snowball.C", "Used SAK: Throw Snowball", "Threw Snowball", 0),
            new EnemyDstBuffApplyMechanic(IcyBarrier, new MechanicPlotlySetting(Symbols.Square, Colors.DarkBlue), "IcyBarrier.A", "Icy Barrier Applied", "Icy Barrier Application", 0),
        ])
        );
        Extension = "freezie";
        Icon = EncounterIconFreezie;
        EncounterID |= 0x000001;
    }

    //protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    //{
    //    return new CombatReplayMap(CombatReplayFreezie,
    //                    (1008, 1008),
    //                    (-1420, 3010, 1580, 6010));
    //}

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(ColdHeartedAura, ColdHeartedAura),
        ];
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        // Snow Piles
        var snowPiles = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 0 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth == 2 && x.HitboxHeight == 300);
        foreach (AgentItem pile in snowPiles)
        {
            pile.OverrideType(AgentItem.AgentType.NPC, agentData);
            pile.OverrideID(TargetID.SnowPile, agentData);
        }
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor? mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Freezie));
        SingleActor? heartTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.FreeziesFrozenHeart));
        if (mainTarget == null)
        {
            throw new MissingKeyActorsException("Freezie not found");
        }
        phases[0].AddTarget(mainTarget, log);
        if (!requirePhases)
        {
            return phases;
        }
        phases.AddRange(GetPhasesByInvul(log, Determined895, mainTarget, true, true));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.AddParentPhase(phases[0]);
            if (i % 2 == 1)
            {
                phase.Name = "Phase " + (i + 1) / 2;
                phase.AddTarget(mainTarget, log);
            }
            else
            {
                phase.Name = "Heal " + (i) / 2;
                phase.AddTarget(heartTarget, log);
            }
        }
        return phases;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        RewardEvent? reward = combatData.GetRewardEvents().FirstOrDefault(x => x.RewardType == RewardTypes.OldRaidReward1 && x.Time > fightData.FightStart);
        if (reward != null)
        {
            fightData.SetSuccess(true, reward.Time);
        }
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        AgentItem freezie = agentData.GetNPCsByID(TargetID.Freezie).FirstOrDefault() ?? throw new MissingKeyActorsException("Freezie not found");
        HealthUpdateEvent? freezieHpUpdate = combatData.GetHealthUpdateEvents(freezie).FirstOrDefault(x => x.Time >= freezie.FirstAware);
        if ((freezieHpUpdate != null && freezieHpUpdate.HealthPercent <= 90))
        {
            return FightData.EncounterStartStatus.Late;
        }
        AgentItem? heart = agentData.GetNPCsByID(TargetID.FreeziesFrozenHeart).FirstOrDefault();
        if (heart != null)
        {
            HealthUpdateEvent? heartHpUpdate = combatData.GetHealthUpdateEvents(heart).FirstOrDefault(x => x.Time >= freezie.FirstAware);
            if ((heartHpUpdate != null && heartHpUpdate.HealthPercent > 0) )
            {
                return FightData.EncounterStartStatus.Late;
            }
        }
        return FightData.EncounterStartStatus.Normal;
    }

    protected override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.Freezie,
            TargetID.FreeziesFrozenHeart
        ];
    }

    protected override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.IceStormer,
            TargetID.IceSpiker,
            TargetID.IcyProtector,
            TargetID.SnowPile,
        ];
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        switch (target.ID)
        {
            case (int)TargetID.Freezie:
                // Fixation tether to Icy Protector
                var fixations = GetFilteredList(log.CombatData, IcyBarrier, target, true, true);
                replay.Decorations.AddTether(fixations, "rgba(30, 144, 255, 0.4)");
                break;
            default:
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);

        // Fixation Aurora Beam
        var fixatedBeam = p.GetBuffStatus(log, AuroraBeamTargetBuff, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(fixatedBeam, p, ParserIcons.FixationPurpleOverhead);

        // Fixation Giant Snowball
        var fixated = p.GetBuffStatus(log, GiantSnowballFreezieTargetBuff1, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(fixated, p, ParserIcons.FixationYellowOverhead);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log);

        // Frost Patch
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.FreezieFrozenPatch, out var frozenPatches))
        {
            foreach (EffectEvent effect in frozenPatches)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 30000);
                var connector = new PositionConnector(effect.Position);
                var rotationConnector = new AngleConnector(effect.Rotation.Z);
                EnvironmentDecorations.Add(new RectangleDecoration(50, 190, lifespan, Colors.White, 0.4, connector).UsingRotationConnector(rotationConnector));
            }
        }

        // This effect is the orange AoE spawned by the Giant Snowball and the AoEs during the healing phase and last phase
        // The effect for the snowball last 1000, the others 2700
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.FreezieOrangeAoE120, out var orangeAoEs))
        {
            foreach (EffectEvent effect in orangeAoEs)
            {
                (long, long) lifespan = (effect.Time, effect.Time + effect.Duration);
                var connector = new PositionConnector(effect.Position);
                var circle = new CircleDecoration(120, lifespan, Colors.LightOrange, 0.2, connector);
                EnvironmentDecorations.Add(circle);
                EnvironmentDecorations.Add(circle.Copy().UsingGrowingEnd(lifespan.Item2));
            }
        }

        // Blizzard Ring
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.FreezieDoughnutRing, out var blizzards))
        {
            foreach (EffectEvent effect in blizzards)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 10000);
                var connector = new PositionConnector(effect.Position);
                EnvironmentDecorations.Add(new DoughnutDecoration(760, 1000, lifespan, Colors.Orange, 0.2, connector));
                // Thicker Borders
                EnvironmentDecorations.Add(new DoughnutDecoration(760, 780, lifespan, Colors.Orange, 0.2, connector));
                EnvironmentDecorations.Add(new DoughnutDecoration(980, 1000, lifespan, Colors.Orange, 0.2, connector));
            }
        }
    }
}
