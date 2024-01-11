using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using GW2EIEvtcParser.Exceptions;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class StatueOfIce : HallOfChains
    {
        public StatueOfIce(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(KingsWrathConeAoE, "King's Wrath", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.White), "Cone AoE", "Hit by King's Wrath (Cone AoEs)", "King's Wrath Cone AoE Hit", 0),
                new PlayerDstHitMechanic(KingsWrathConeShards, "King's Wrath", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.LightBlue), "Cone Shards", "Hit by King's Wrath (Frontal Cone Shards)", "King's Wrath Cone Shards Hit", 0),
                new PlayerDstHitMechanic(NumbingBreach, "Numbing Breach", new MechanicPlotlySetting(Symbols.AsteriskOpen, Colors.LightBlue), "Cracks", "Stood on Numbing Breach (Ice Cracks in the Ground)", "Cracks", 0),
                new PlayerDstBuffApplyMechanic(FrozenWind, "Frozen Wind", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Green), "Green", "Frozen Wind (Stood in Green)", "Green Stack", 0),
                new PlayerDstBuffApplyMechanic(Glaciate, "Glaciate", new MechanicPlotlySetting(Symbols.Square, Colors.Purple), "Glaciate", "Glaciated (Frozen by 4th Stack of Frozen Wind)", "Glaciate", 0),
                new EnemySrcEffectMechanic(EffectGUIDs.BrokenKingIceBreakerGreenExplosion, "Ice Breaker", new MechanicPlotlySetting(Symbols.CircleX, Colors.DarkGreen), "Ice Breaker", "Hailstorm Explosion (Missed Green)", "Ice Breaker (Green Missed)", 0),
            }
            );
            Extension = "brokenking";
            Icon = EncounterIconStatueOfIce;
            EncounterCategoryInformation.InSubCategoryOrder = 2;
            EncounterID |= 0x000003;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayStatueOfIce,
                            (999, 890),
                            (2497, 5388, 7302, 9668)/*,
                            (-21504, -12288, 24576, 12288),
                            (19072, 15484, 20992, 16508)*/);
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            AgentItem brokenKing = agentData.GetNPCsByID(ArcDPSEnums.TargetID.BrokenKing).FirstOrDefault();
            if (brokenKing == null)
            {
                throw new MissingKeyActorsException("Broken King not found");
            }
            long startToUse = GetGenericFightOffset(fightData);
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogStartNPCUpdate);
            if (logStartNPCUpdate != null)
            {
                CombatItem initialCast = combatData.FirstOrDefault(x => x.StartCasting() && x.SkillID == BrokenKingFirstCast && x.SrcMatchesAgent(brokenKing));
                if (initialCast != null)
                {
                    startToUse = initialCast.Time;
                } 
                else
                {
                    startToUse = GetPostLogStartNPCUpdateDamageEventTime(fightData, agentData, combatData, logStartNPCUpdate.Time, brokenKing);
                }
            }
            return startToUse;
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
            var green = log.CombatData.GetBuffData(FrozenWind).Where(x => x.To == p.AgentItem && x is BuffApplyEvent).ToList();
            foreach (AbstractBuffEvent c in green)
            {
                int duration = 45000;
                AbstractBuffEvent removedBuff = log.CombatData.GetBuffRemoveAllData(FrozenWind).FirstOrDefault(x => x.To == p.AgentItem && x.Time > c.Time && x.Time < c.Time + duration);
                int start = (int)c.Time;
                int end = start + duration;
                if (removedBuff != null)
                {
                    end = (int)removedBuff.Time;
                }
                replay.Decorations.Add(new CircleDecoration(100, (start, end), "rgba(100, 200, 255, 0.25)", new AgentConnector(p)));
            }
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.BrokenKing:
                    var Cone = cls.Where(x => x.SkillId == KingsWrathConeShards).ToList();
                    foreach (AbstractCastEvent c in Cone)
                    {
                        int start = (int)c.Time;
                        int end = (int)c.EndTime;
                        uint range = 450;
                        int angle = 100;
                        Point3D facing = target.GetCurrentRotation(log, start + 1000);
                        if (facing == null)
                        {
                            continue;
                        }
                        var connector = new AgentConnector(target);
                        var rotationConnector = new AngleConnector(facing);
                        replay.Decorations.Add(new PieDecoration(range, angle, (start, end), Colors.LightBlue, 0.2, connector).UsingRotationConnector(rotationConnector));
                        replay.Decorations.Add(new PieDecoration(range, angle, (start + 1900, end), Colors.LightBlue, 0.3, connector).UsingRotationConnector(rotationConnector));
                    }
                    break;
                default:
                    break;
            }
        }

        internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log);

            // Numbing Breach - Cracks - White smoke indicator
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.BrokenKingNumbingBreachIndicator, out IReadOnlyList<EffectEvent> cracksIndicators))
            {
                foreach (EffectEvent effect in cracksIndicators)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 1000);
                    var connector = new PositionConnector(effect.Position);
                    var circle = new CircleDecoration(115, lifespan, "rgba(219, 233, 244, 0.2)", connector);
                    EnvironmentDecorations.Add(circle);
                }
            }

            // Numbing Breach - Cracks - Damage zone
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.BrokenKingNumbingBreachDamage, out IReadOnlyList<EffectEvent> cracks))
            {
                foreach (EffectEvent effect in cracks)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 30000);
                    var connector = new PositionConnector(effect.Position);
                    var rotationConnector = new AngleConnector(effect.Rotation.Z);
                    var rectangle = (RectangleDecoration)new RectangleDecoration(40, 230, lifespan, "rgba(66, 130, 253, 0.2)", connector).UsingRotationConnector(rotationConnector);
                    EnvironmentDecorations.Add(rectangle);
                }
            }

            // Hailstorm - Greens
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.BrokenKingHailstormGreen, out IReadOnlyList<EffectEvent> greens))
            {
                foreach (EffectEvent green in greens)
                {
                    Color color = Colors.DarkGreen;

                    // Ice Breaker - Failed Greens
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.BrokenKingIceBreakerGreenExplosion, out IReadOnlyList<EffectEvent> failedGreens))
                    {
                        EffectEvent failedGreen = failedGreens.FirstOrDefault(x => x.Position.Distance2DToPoint(green.Position) < 1e-6 && Math.Abs(x.Time - green.Time - 15000) <= 650);
                        if (failedGreen != null)
                        {
                            color = Colors.DarkRed;
                        }
                    }

                    (long, long) lifespan = green.ComputeLifespan(log, 15000);
                    var circle = new CircleDecoration(120, lifespan, color, 0.4, new PositionConnector(green.Position));
                    EnvironmentDecorations.Add(circle);
                    EnvironmentDecorations.Add(circle.Copy().UsingGrowingEnd(lifespan.Item2, true));
                }
            }
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(BitingAura, BitingAura),
                new EffectCastFinder(Hailstorm, EffectGUIDs.BrokenKingHailstormGreen),
                new EffectCastFinder(IceBreaker, EffectGUIDs.BrokenKingIceBreakerGreenExplosion)
                    .UsingAgentRedirectionIfUnknown((int)ArcDPSEnums.TargetID.BrokenKing),
            };
        }
        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            NoBouncyChestGenericCheckSucess(combatData, agentData, fightData, playerAgents);
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Statue of Ice";
        }
    }
}
