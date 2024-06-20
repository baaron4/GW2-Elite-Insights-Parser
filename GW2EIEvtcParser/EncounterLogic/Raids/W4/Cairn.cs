using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Cairn : BastionOfThePenitent
    {
        public Cairn(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            // (ID, ingame name, Type, BossID, plotly marker, Table header name, ICD, Special condition) // long table hover name, graph legend name
            new PlayerDstHitMechanic(CairnDisplacement, "Displacement", new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Port","Orange Teleport Field", "Orange TP",0),
            new PlayerDstHitMechanic(new long[] { SpatialManipulation1, SpatialManipulation2, SpatialManipulation3, SpatialManipulation4, SpatialManipulation5, SpatialManipulation6 }, "Spatial Manipulation", new MechanicPlotlySetting(Symbols.Circle,Colors.Green), "Green","Green Spatial Manipulation Field (lift)", "Green (lift)",0).UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new PlayerDstHitMechanic(new long[] { SpatialManipulation1, SpatialManipulation2, SpatialManipulation3, SpatialManipulation4, SpatialManipulation5, SpatialManipulation6 }, "Spatial Manipulation", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Green), "Stab.Green","Green Spatial Manipulation Field while affected by stability", "Stabilized Green",0).UsingChecker((de, log) => de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new PlayerDstHitMechanic(MeteorSwarm, "Meteor Swarm", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "KB","Knockback Crystals", "KB Crystal",1000),
            new PlayerSrcSkillMechanic(MeteorSwarm, "Meteor Swarm", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Orange), "Refl.KB","Reflected Knockback Crystals", "Reflected KB Crystal",1000).WithMinions(true).UsingChecker((evt, log) => evt.ToFriendly),
            new PlayerDstBuffApplyMechanic(SharedAgony, "Shared Agony", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Agony","Shared Agony Debuff Application", "Shared Agony",0),//could flip
            new PlayerDstBuffApplyMechanic(SharedAgony25, "Shared Agony", new MechanicPlotlySetting(Symbols.StarTriangleUpOpen,Colors.Pink), "Agony 25","Shared Agony Damage (25% Player's HP)", "SA dmg 25%",0), // Seems to be a (invisible) debuff application for 1 second from the Agony carrier to the closest(?) person in the circle.
            new PlayerDstBuffApplyMechanic(SharedAgony50, "Shared Agony", new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.Orange), "Agony 50","Shared Agony Damage (50% Player's HP)", "SA dmg 50%",0), //Chaining from the first person hit by 38170, applying a 1 second debuff to the next person.
            new PlayerDstBuffApplyMechanic(SharedAgony75, "Shared Agony", new MechanicPlotlySetting(Symbols.StarOpen,Colors.Red), "Agony 75","Shared Agony Damage (75% Player's HP)", "SA dmg 75%",0), //Chaining from the first person hit by 37768, applying a 1 second debuff to the next person.
            // new Mechanic(SharedAgony2, "Shared Agony", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, new MechanicPlotlySetting(Symbols.CircleOpen,Color.Red), "Agony Damage",0), from old raidheroes logs? Small damage packets. Is also named "Shared Agony" in the evtc. Doesn't seem to occur anymore.
            // new Mechanic(SharedAgony3, "Shared Agony", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, new MechanicPlotlySetting(Symbols.CircleOpen,Color.Red), "SA.dmg","Shared Agony Damage dealt", "Shared Agony dmg",0), //could flip. HP% attack, thus only shows on down/absorb hits.
            new PlayerDstHitMechanic(EnergySurge, "Energy Surge", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.DarkGreen), "Leap","Jump between green fields", "Leap",100),
            new PlayerDstHitMechanic(OrbitalSweep, "Orbital Sweep", new MechanicPlotlySetting(Symbols.DiamondWide,Colors.Magenta), "Sweep","Sword Spin (Knockback)", "Sweep",100),//short cooldown because of multihits. Would still like to register second hit at the end of spin though, thus only 0.1s
            new PlayerDstHitMechanic(GravityWave, "Gravity Wave", new MechanicPlotlySetting(Symbols.Octagon,Colors.Magenta), "Donut","Expanding Crystal Donut Wave (Knockback)", "Crystal Donut",0)
            // Spatial Manipulation IDs correspond to the following: 1st green when starting the fight: 37629;
            // Greens after Energy Surge/Orbital Sweep: 38302
            //100% - 75%: 37611
            // 75% - 50%: 38074
            // 50% - 25%: 37673
            // 25% -  0%: 37642
            });
            Extension = "cairn";
            Icon = EncounterIconCairn;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000001;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayCairn,
                            (607, 607),
                            (12981, 642, 15725, 3386)/*,
                            (-27648, -9216, 27648, 12288),
                            (11774, 4480, 14078, 5376)*/);
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(CosmicAura, CosmicAura), // Cosmic Aura
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor cairn = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Cairn)) ?? throw new MissingKeyActorsException("Cairn not found");
            phases[0].AddTarget(cairn);
            if (!requirePhases)
            {
                return phases;
            }
            BuffApplyEvent enrageApply = log.CombatData.GetBuffDataByIDByDst(EnragedCairn, cairn.AgentItem).OfType<BuffApplyEvent>().FirstOrDefault();
            if (enrageApply != null)
            {
                var normalPhase = new PhaseData(log.FightData.FightStart, enrageApply.Time)
                {
                    Name = "Calm"
                };
                normalPhase.AddTarget(cairn);

                var enragePhase = new PhaseData(enrageApply.Time, log.FightData.FightEnd)
                {
                    Name = "Angry"
                };
                enragePhase.AddTarget(cairn);

                phases.Add(normalPhase);
                phases.Add(enragePhase);
            }
            return phases;
        }

        internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log);

            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.CairnDisplacement, out IReadOnlyList<EffectEvent> displacementEffects))
            {
                foreach (EffectEvent displacement in displacementEffects)
                {
                    var circle = new CircleDecoration(90, (displacement.Time, displacement.Time + 3000), Colors.Orange, 0.4, new PositionConnector(displacement.Position));
                    EnvironmentDecorations.Add(circle);
                    EnvironmentDecorations.Add(circle.Copy().UsingGrowingEnd(displacement.Time + 3000));
                }
            }
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Cairn:
                    var swordSweep = cls.Where(x => x.SkillId == OrbitalSweep).ToList();
                    foreach (AbstractCastEvent c in swordSweep)
                    {
                        int start = (int)c.Time;
                        int preCastTime = 1400;
                        int initialHitDuration = 850;
                        int sweepDuration = 1100;
                        uint width = 1400; uint height = 80;
                        Point3D facing = target.GetCurrentRotation(log, start);
                        if (facing != null)
                        {
                            var positionConnector = (AgentConnector)new AgentConnector(target).WithOffset(new Point3D(width / 2, 0), true);
                            var rotationConnector = new AngleConnector(facing);
                            replay.Decorations.Add(new RectangleDecoration(width, height, (start, start + preCastTime), Colors.Purple, 0.1, positionConnector).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new RectangleDecoration(width, height, (start + preCastTime, start + preCastTime + initialHitDuration), Colors.DarkPurple, 0.5, positionConnector).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new RectangleDecoration(width, height, (start + preCastTime + initialHitDuration, start + preCastTime + initialHitDuration + sweepDuration), Colors.DarkPurple, 0.5, positionConnector).UsingRotationConnector(new AngleConnector(facing, 360)));
                        }
                    }
                    var wave = cls.Where(x => x.SkillId == GravityWave).ToList();
                    foreach (AbstractCastEvent c in wave)
                    {
                        int start = (int)c.Time;
                        int preCastTime = 1200;
                        int duration = 600;
                        uint firstRadius = 400;
                        uint secondRadius = 700;
                        uint thirdRadius = 1000;
                        uint fourthRadius = 1300;
                        replay.Decorations.Add(new DoughnutDecoration(firstRadius, secondRadius, (start + preCastTime, start + preCastTime + duration), Colors.Purple, 0.3, new AgentConnector(target)));
                        replay.Decorations.Add(new DoughnutDecoration(secondRadius, thirdRadius, (start + preCastTime + 2 * duration, start + preCastTime + 3 * duration), Colors.Purple, 0.3, new AgentConnector(target)));
                        replay.Decorations.Add(new DoughnutDecoration(thirdRadius, fourthRadius, (start + preCastTime + 5 * duration, start + preCastTime + 6 * duration), Colors.Purple, 0.3, new AgentConnector(target)));
                    }
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.CairnDashGreen, out IReadOnlyList<EffectEvent> dashGreenEffects))
                    {
                        var spatialManipulations = cls.Where(x => x.SkillId == SpatialManipulation6).ToList();
                        foreach (EffectEvent dashGreen in dashGreenEffects)
                        {
                            long dashGreenStart = dashGreen.Time;
                            long dashGreenEnd = target.LastAware;
                            AbstractCastEvent endEvent = spatialManipulations.FirstOrDefault(x => x.EndTime >= dashGreenStart);
                            if (endEvent != null)
                            {
                                dashGreenEnd = endEvent.Time + 3300; // from skill def
                            }
                            replay.Decorations.Add(new CircleDecoration(110, (dashGreenStart, dashGreenEnd), Colors.DarkGreen, 0.4, new PositionConnector(dashGreen.Position)));
                            replay.Decorations.Add(new CircleDecoration(110, (dashGreenEnd - 200, dashGreenEnd), Colors.DarkGreen, 0.4, new PositionConnector(dashGreen.Position)));
                        }
                    }
                    //CombatReplay.DebugAllNPCEffects(log, replay, new HashSet<long>(), 50000, 63000);
                    break;
                default:
                    break;
            }
        }

        internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            if (!agentData.TryGetFirstAgentItem(ArcDPSEnums.TargetID.Cairn, out AgentItem cairn))
            {
                throw new MissingKeyActorsException("Cairn not found");
            }
            // spawn protection loss -- most reliable
            CombatItem spawnProtectionLoss = combatData.Find(x => x.IsBuffRemove == ArcDPSEnums.BuffRemove.All && x.SrcMatchesAgent(cairn) && x.SkillID == SpawnProtection);
            if (spawnProtectionLoss != null)
            {
                return spawnProtectionLoss.Time;
            }
            else
            {
                // get first end casting
                CombatItem firstCastEnd = combatData.FirstOrDefault(x => x.EndCasting() && (x.Time - fightData.LogStart) < 2000 && x.SrcMatchesAgent(cairn));
                // It has to Impact(38102), otherwise anomaly, player may have joined mid fight, do nothing
                if (firstCastEnd != null && firstCastEnd.SkillID == CairnImpact)
                {
                    // Action 4 from skill dump for 38102
                    long actionHappened = 1025;
                    // Adds around 10 to 15 ms diff compared to buff loss
                    if (firstCastEnd.BuffDmg > 0)
                    {
                        double nonScaledToScaledRatio = (double)firstCastEnd.Value / firstCastEnd.BuffDmg;
                        return firstCastEnd.Time - firstCastEnd.Value + (long)Math.Round(nonScaledToScaledRatio * actionHappened) - 1;
                    }
                    // Adds around 15 to 20 ms diff compared to buff loss
                    else
                    {
                        return firstCastEnd.Time - firstCastEnd.Value + actionHappened;
                    }
                }
            }
            return GetGenericFightOffset(fightData);
        }

        internal override List<AbstractCastEvent> SpecialCastEventProcess(CombatData combatData, SkillData skillData)
        {
            List<AbstractCastEvent> res = base.SpecialCastEventProcess(combatData, skillData);
            res.AddRange(ProfHelper.ComputeUnderBuffCastEvents(combatData, skillData, CelestialDashSAK, CelestialDashBuff));
            return res;
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
            // shared agony
            var agony = log.CombatData.GetBuffDataByIDByDst(SharedAgony, p.AgentItem).Where(x => x is BuffApplyEvent).ToList();
            foreach (AbstractBuffEvent c in agony)
            {
                long agonyStart = c.Time;
                long agonyEnd = agonyStart + 62000;
                replay.Decorations.Add(new CircleDecoration(220, (agonyStart, agonyEnd), Colors.Red, 0.5, new AgentConnector(p)).UsingFilled(false));
            }
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return combatData.GetSkills().Contains(Countdown) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Cairn";
        }
    }
}
