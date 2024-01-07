using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Freezie : FestivalStrikeMissionLogic
    {
        public Freezie(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstBuffApplyMechanic(AuroraBeamTargetBuff, "Aurora Beam", new MechanicPlotlySetting(Symbols.Star, Colors.Purple), "AuroraBeam.T", "Targetted by Aurora Beam", "Aurora Beam Target", 0),
                new PlayerDstBuffApplyMechanic(GiantSnowballFreezieTargetBuff1, "Giant Snowball", new MechanicPlotlySetting(Symbols.Star, Colors.Purple, 5), "GiantSnowball.T", "Targetted by Giant Snowball", "Giant Snowball Target", 0),
                new PlayerDstHitMechanic(new long [] { AuroraBeam1, AuroraBeam2, AuroraBeam3 }, "Aurora Beam", new MechanicPlotlySetting(Symbols.StarDiamond, Colors.Purple), "AuroraBeam.H", "Hit by Aurora Beam", "Aurora Beam Hit", 0),
                new PlayerDstHitMechanic(GiantSnowballFreezieDamage, "Giant Snowball", new MechanicPlotlySetting(Symbols.Circle, Colors.White), "GiantSnowball.H", "Hit by Giant Snowball", "Giant Snowball Hit", 0),
                new PlayerDstHitMechanic(Blizzard, "Blizzard", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Orange), "Blizzard.H", "Hit by Blizzard (Outer circle)", "Blizzard Hit", 0),
                new PlayerDstHitMechanic(FrostPatchDamage, "Frost Patch", new MechanicPlotlySetting(Symbols.Octagon, Colors.Blue), "FrostPatch.H", "Hit by Frost Patch (Cracks)", "Frost Patch Hit (Cracks)", 0),
                new PlayerDstHitMechanic(new long [] { JuttingIceSpikes1, JuttingIceSpikes2 }, "Jutting Ice Spike", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.LightGrey), "IceSpike.H", "Hit by Jutting Ice Spike", "Jutting Ice Spike Hit", 0),
                new PlayerCastStartMechanic(FireSnowball, "Snowball", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.White), "Snowball.C", "Used SAK: Throw Snowball", "Threw Snowball", 0),
                new EnemyDstBuffApplyMechanic(IcyBarrier, "Icy Barrier", new MechanicPlotlySetting(Symbols.Square, Colors.DarkBlue), "IcyBarrier.A", "Icy Barrier Applied", "Icy Barrier Application", 0),
                new EnemyCastStartMechanic(new long [] { AuroraBeam1, AuroraBeam2, AuroraBeam3 }, "Aurora Beam", new MechanicPlotlySetting(Symbols.StarDiamondOpen, Colors.Purple), "AuroraBeam.C", "Casted Aurora Beam", "Aurora Beam Cast", 0),
                new EnemyCastStartMechanic(FrostPatchSkill, "Frost Patch", new MechanicPlotlySetting(Symbols.Octagon, Colors.LightBlue), "FrostPatch.C", "Casted Frost Patch", "Frost Patch Cast", 0),
                new EnemyCastStartMechanic(GiantSnowballFreezieCast, "Giant Snowball", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.White), "GiantSnowball.C", "Casted Giant Snowball", "Giant Snowball Cast", 0),
            });
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
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(ColdHeartedAura, ColdHeartedAura),
            };
        }

        internal override void EIEvtcParse(ulong gw2Build, int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            // Snow Piles
            var snowPiles = combatData.Where(x => x.DstAgent == 0 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth == 2 && x.HitboxHeight == 300).ToList();
            foreach (AgentItem pile in snowPiles)
            {
                pile.OverrideType(AgentItem.AgentType.NPC);
                pile.OverrideID(TrashID.SnowPile);
            }
            agentData.Refresh();
            ComputeFightTargets(agentData, combatData, extensions);
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Freezie));
            AbstractSingleActor heartTarget = Targets.FirstOrDefault(x => x.IsSpecies(TrashID.FreeziesFrozenHeart));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Freezie not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, Determined895, mainTarget, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 1)
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(mainTarget);
                }
                else
                {
                    phase.Name = "Heal " + (i) / 2;
                    phase.AddTarget(heartTarget);
                }
            }
            return phases;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            RewardEvent reward = combatData.GetRewardEvents().FirstOrDefault(x => x.RewardType == RewardTypes.OldRaidReward1);
            if (reward != null)
            {
                fightData.SetSuccess(true, reward.Time);
            }
        }

        internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AgentItem heart = agentData.GetNPCsByID(TrashID.FreeziesFrozenHeart).FirstOrDefault();
            AgentItem freezie = agentData.GetNPCsByID(TargetID.Freezie).FirstOrDefault();
            HealthUpdateEvent heartHpUpdate = combatData.GetHealthUpdateEvents(heart).FirstOrDefault(x => x.Time >= freezie.FirstAware);
            HealthUpdateEvent freezieHpUpdate = combatData.GetHealthUpdateEvents(freezie).FirstOrDefault(x => x.Time >= freezie.FirstAware);
            if (heartHpUpdate.HPPercent > 0 || freezieHpUpdate.HPPercent <= 90)
            {
                return FightData.EncounterStartStatus.Late;
            }
            return FightData.EncounterStartStatus.Normal;
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)TargetID.Freezie,
                (int)TrashID.FreeziesFrozenHeart
            };
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)TargetID.Freezie,
                (int)TrashID.FreeziesFrozenHeart
            };
        }

        protected override List<TrashID> GetTrashMobsIDs()
        {
            return new List<TrashID>()
            {
                TrashID.IceStormer,
                TrashID.IceSpiker,
                TrashID.IcyProtector,
                TrashID.SnowPile,
            };
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

            switch (target.ID)
            {
                case (int)TargetID.Freezie:
                    // Fixation tether to Icy Protector
                    List<AbstractBuffEvent> fixations = GetFilteredList(log.CombatData, IcyBarrier, target, true, true);
                    replay.AddTether(fixations, "rgba(30, 144, 255, 0.4)");
                    break;
                default:
                    break;
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);

            // Fixation Aurora Beam
            var fixatedBeam = p.GetBuffStatus(log, AuroraBeamTargetBuff, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            replay.AddOverheadIcons(fixatedBeam, p, ParserIcons.FixationPurpleOverhead);

            // Fixation Giant Snowball
            var fixated = p.GetBuffStatus(log, GiantSnowballFreezieTargetBuff1, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            replay.AddOverheadIcons(fixated, p, ParserIcons.FixationYellowOverhead);
        }

        internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log);

            // Frost Patch
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.FreezieFrozenPatch, out IReadOnlyList<EffectEvent> frozenPatches))
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
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.FreezieOrangeAoE120, out IReadOnlyList<EffectEvent> orangeAoEs))
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
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.FreezieDoughnutRing, out IReadOnlyList<EffectEvent> blizzards))
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
}
