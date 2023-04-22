using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.Extensions;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class XunlaiJadeJunkyard : EndOfDragonsStrike
    {
        public XunlaiJadeJunkyard(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(GraspingHorror, "GraspingHorror", new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.LightOrange), "Hands.H", "Hit by Hands AoE", "Hands Hit", 150),
                new PlayerDstHitMechanic(DeathsEmbraceSkill, "Death's Embrace", new MechanicPlotlySetting(Symbols.CircleCross, Colors.DarkRed), "AnkkaPull.H", "Hit by Death's Embrace (Ankka's Pull)", "Death's Embrace Hit", 150),
                new PlayerDstHitMechanic(DeathsHandInBetween, "Death's Hand", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Yellow), "Sctn.AoE.H", "Hit by in-between sections AoE", "Death's Hand Hit (transitions)", 150),
                new PlayerDstHitMechanic(DeathsHandDropped, "Death's Hand", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Green), "Sprd.AoE.H", "Hit by placeable Death's Hand AoE", "Death's Hand Hit (placeable)", 150),
                new PlayerDstHitMechanic(WallOfFear, "Wall of Fear", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.DarkRed), "Krait.H", "Hit by Krait AoE", "Krait Hit", 150),
                new PlayerDstHitMechanic(new long[] { WaveOfTormentNM, WaveOfTormentCM }, "Wave of Torment", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "Quaggan.H", "Hit by Quaggan Explosion", "Quaggan Hit", 150),
                new PlayerDstHitMechanic(TerrifyingApparition, "Terrifying Apparition", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.DarkRed), "Lich.H", "Hit by Lich AoE", "Lich Hit", 150),
                new PlayerDstHitMechanic(new long[] { ZhaitansReachThrashXJJ1, ZhaitansReachThrashXJJ2 }, "Thrash", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.DarkGreen), "ZhtRch.Pull", "Pulled by Zhaitan's Reach", "Zhaitan's Reach Pull", 150),
                new PlayerDstHitMechanic(new long[] { ZhaitansReachGroundSlam, ZhaitansReachGroundSlamXJJ }, "Ground Slam", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.DarkGreen), "ZhtRch.Knck", "Knocked by Zhaitan's Reach", "Zhaitan's Reach Knock", 150),
                new PlayerDstHitMechanic(ImminentDeathSkill, "Imminent Death", new MechanicPlotlySetting(Symbols.DiamondTall, Colors.Green), "Imm.Death.H", "Hit by Imminent Death", "Imminent Death Hit", 0),
                new EnemyCastStartMechanic(InevitabilityOfDeath, "Inevitability of Death", new MechanicPlotlySetting(Symbols.Octagon, Colors.LightRed), "Inev.Death.C", "Casted Inevitability of Death (Enrage)", "Inevitability of Death (Enrage)", 150),
                new EnemyCastStartMechanic(DeathsEmbraceSkill, "Death's Embrace", new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Blue), "AnkkaPull.C", "Casted Death's Embrace", "Death's Embrace Cast", 150),
                new EnemyDstBuffApplyMechanic(PowerOfTheVoid, "Power of the Void", new MechanicPlotlySetting(Symbols.Star, Colors.Yellow), "Pwrd.Up", "Ankka has powered up", "Ankka powered up", 150),
                new PlayerDstBuffApplyMechanic(ImminentDeathBuff, "Imminent Death", new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.Green), "Imm.Death.B", "Placed Death's Hand AoE and gained Imminent Death Buff", "Imminent Death Buff", 150),
                new PlayerDstBuffApplyMechanic(FixatedAnkkaKainengOverlook, "Fixated", new MechanicPlotlySetting(Symbols.Diamond, Colors.Purple), "Fxt.Hatred", "Fixated by Reanimated Hatred", "Fixated Hatred", 150),
                new PlayerDstBuffApplyMechanic(Hallucinations, "Hallucinations", new MechanicPlotlySetting(Symbols.Square, Colors.LightBlue), "Hallu", "Received Hallucinations Debuff", "Hallucinations Debuff", 150),
                new PlayerDstBuffApplyMechanic(DeathsHandSpreadBuff, "Death's Hand Spread", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.Green), "Sprd.AoE.B", "Received Death's Hand Spread", "Death's Hand Spread", 150),
            }
            );
            Icon = EncounterIconXunlaiJadeJunkyard;
            Extension = "xunjadejunk";
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000002;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayXunlaiJadeJunkyard,
                            (1485, 1292),
                            (-7090, -2785, 3647, 6556)/*,
                            (-15360, -36864, 15360, 39936),
                            (3456, 11012, 4736, 14212)*/);
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor ankka = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Ankka));
            if (ankka == null)
            {
                throw new MissingKeyActorsException("Ankka not found");
            }
            phases[0].AddTarget(ankka);
            if (!requirePhases)
            {
                return phases;
            }
            List<PhaseData> subPhases = GetPhasesByInvul(log, AnkkaPlateformChanging, ankka, false, true);
            for (int i = 0; i < subPhases.Count; i++)
            {
                subPhases[i].Name = "Location " + (i + 1);
                subPhases[i].AddTarget(ankka);
            }
            phases.AddRange(subPhases);
            List<PhaseData> subSubPhases = GetPhasesByInvul(log, Determined895, ankka, false, false);
            subSubPhases.RemoveAll(x => subPhases.Any(y => Math.Abs(y.Start - x.Start) < ServerDelayConstant && Math.Abs(y.End - x.End) < ServerDelayConstant));
            int curSubSubPhaseID = 0;
            PhaseData previousSubPhase = null;
            for (int i = 0; i < subSubPhases.Count; i++)
            {
                PhaseData subsubPhase = subSubPhases[i];
                PhaseData subPhase = subPhases.FirstOrDefault(x => x.Start - ServerDelayConstant <= subsubPhase.Start && x.End + ServerDelayConstant >= subsubPhase.End);
                if (previousSubPhase != subPhase)
                {
                    previousSubPhase = subPhase;
                    curSubSubPhaseID = 0;
                }
                if (subPhase != null)
                {
                    int index = subPhases.IndexOf(subPhase);
                    subsubPhase.OverrideStart(Math.Max(subsubPhase.Start, subPhase.Start));
                    subsubPhase.OverrideEnd(Math.Min(subsubPhase.End, subPhase.End));
                    subsubPhase.Name = "Location " + (index + 1) + " - " + (++curSubSubPhaseID);
                    subsubPhase.AddTarget(ankka);
                }
            }
            phases.AddRange(subSubPhases);
            //
            return phases;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                AbstractSingleActor ankka = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Ankka));
                if (ankka == null)
                {
                    throw new MissingKeyActorsException("Ankka not found");
                }
                var buffApplies = combatData.GetBuffData(Determined895).OfType<BuffApplyEvent>().Where(x => x.To == ankka.AgentItem && !x.Initial && x.AppliedDuration > int.MaxValue / 2 && x.Time >= fightData.FightStart + 5000).ToList();
                if (buffApplies.Count == 3)
                {
                    fightData.SetSuccess(true, buffApplies.LastOrDefault().Time);
                }
            }
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.Ankka,
            };
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Ankka,
                (int)ArcDPSEnums.TrashID.ReanimatedAntipathy,
                (int)ArcDPSEnums.TrashID.ReanimatedSpite,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Ankka,
                ArcDPSEnums.TrashID.ReanimatedMalice1,
                ArcDPSEnums.TrashID.ReanimatedMalice2,
                ArcDPSEnums.TrashID.ReanimatedHatred,
                ArcDPSEnums.TrashID.ZhaitansReach,
                ArcDPSEnums.TrashID.KraitsHallucination,
                ArcDPSEnums.TrashID.LichHallucination,
                ArcDPSEnums.TrashID.QuaggansHallucinationNM,
                ArcDPSEnums.TrashID.QuaggansHallucinationCM,
                ArcDPSEnums.TrashID.SanctuaryPrism,
            };
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor ankka = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Ankka));
            if (ankka == null)
            {
                throw new MissingKeyActorsException("Ankka not found");
            }
            MapIDEvent map = combatData.GetMapIDEvents().FirstOrDefault();
            if (map != null && map.MapID == 1434)
            {
                return FightData.EncounterMode.Story;
            }
            return ankka.GetHealth(combatData) > 50e6 ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            var sanctuaryPrism = combatData.Where(x => x.DstAgent == 14940 && x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget).ToList();
            foreach (AgentItem sanctuary in sanctuaryPrism)
            {
                IEnumerable<CombatItem> items = combatData.Where(x => x.SrcMatchesAgent(sanctuary) && x.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate && x.DstAgent == 0);
                sanctuary.OverrideType(AgentItem.AgentType.NPC);
                sanctuary.OverrideID(ArcDPSEnums.TrashID.SanctuaryPrism);
                sanctuary.OverrideAwareTimes(fightData.LogStart, items.Any() ? items.FirstOrDefault().Time : fightData.LogEnd);
            }
            agentData.Refresh();
            ComputeFightTargets(agentData, combatData, extensions);
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);

            if (log.FightData.Success && log.FightData.IsCM && CustomCheckGazeIntoTheVoidEligibility(log))
            {
                InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityGazeIntoTheVoid], 1));
            }
        }

        private static bool CustomCheckGazeIntoTheVoidEligibility(ParsedEvtcLog log)
        {
            IReadOnlyList<AgentItem> agents = log.AgentData.GetNPCsByID((int)ArcDPSEnums.TargetID.Ankka);

            foreach (AgentItem agent in agents)
            {
                IReadOnlyDictionary<long, BuffsGraphModel> bgms = log.FindActor(agent).GetBuffGraphs(log);
                if (bgms != null && bgms.TryGetValue(PowerOfTheVoid, out BuffsGraphModel bgm))
                {
                    if (bgm.BuffChart.Any(x => x.Value == 6)) { return true; }
                }
            }
            return false;
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Ankka:
                    var deathsEmbrace = casts.Where(x => x.SkillId == DeathsEmbraceSkill).ToList();
                    foreach (AbstractCastEvent c in deathsEmbrace)
                    {
                        int durationCast = 10143;
                        int endTime = (int)c.Time + durationCast;

                        Point3D ankkaPosition = target.GetCurrentPosition(log, c.Time);
                        if (ankkaPosition == null) { continue; }

                        EffectGUIDEvent effectGUID = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.DeathsEmbrace);

                        if (effectGUID != null)
                        {
                            var radius = 500; // Zone 1
                            // Zone 2
                            if (ankkaPosition.X > 0 && ankkaPosition.X < 4000)
                            {
                                radius = 340;
                            }
                            // Zone 3
                            if (ankkaPosition.Y > 4000 && ankkaPosition.Y < 6000)
                            {
                                radius = 380;
                            }
                            var effects = log.CombatData.GetEffectEventsByEffectID(effectGUID.ContentID).Where(x => x.Time >= c.Time && x.Time <= c.EndTime).ToList();
                            foreach (EffectEvent effectEvt in effects)
                            {
                                AddDeathEmbraceDecoration(replay, (int)c.Time, durationCast, radius, (int)(effectEvt.Time - c.Time), effectEvt.Position);
                            }
                        }
                        else
                        {
                            // logs without effects
                            int delay = 1833 * 2;
                            // Zone 1
                            if (ankkaPosition.X > -6000 && ankkaPosition.X < -2500 && ankkaPosition.Y < 1000 && ankkaPosition.Y > -1000)
                            {
                                AddDeathEmbraceDecoration(replay, (int)c.Time, durationCast, 500, delay, new Point3D(-3941.78f, 66.76819f, -3611.2f)); // CENTER
                            }
                            // Zone 2
                            if (ankkaPosition.X > 0 && ankkaPosition.X < 4000)
                            {
                                AddDeathEmbraceDecoration(replay, (int)c.Time, durationCast, 340, delay, new Point3D(1663.69f, 1739.87f, -4639.695f)); // NW
                                AddDeathEmbraceDecoration(replay, (int)c.Time, durationCast, 340, delay, new Point3D(2563.689f, 1739.87f, -4664.611f)); // NE
                                AddDeathEmbraceDecoration(replay, (int)c.Time, durationCast, 340, delay, new Point3D(1663.69f, 839.8699f, -4640.633f)); // SW
                                AddDeathEmbraceDecoration(replay, (int)c.Time, durationCast, 340, delay, new Point3D(2563.689f, 839.8699f, -4636.368f)); // SE
                            }
                            // Zone 3
                            if (ankkaPosition.Y > 4000 && ankkaPosition.Y < 6000)
                            {
                                AddDeathEmbraceDecoration(replay, (int)c.Time, durationCast, 380, delay, new Point3D(-2547.61f, 5466.439f, -6257.504f)); // NW
                                AddDeathEmbraceDecoration(replay, (int)c.Time, durationCast, 380, delay, new Point3D(-1647.61f, 5466.439f, -6256.795f)); // NE
                                AddDeathEmbraceDecoration(replay, (int)c.Time, durationCast, 380, delay, new Point3D(-2547.61f, 4566.439f, -6256.799f)); // SW
                                AddDeathEmbraceDecoration(replay, (int)c.Time, durationCast, 380, delay, new Point3D(-1647.61f, 4566.439f, -6257.402f)); // SE
                            }
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.KraitsHallucination:
                    // Wall of Fear
                    int firstMovementTime = 2550;
                    int kraitsRadius = 420;

                    replay.Decorations.Add(new CircleDecoration(true, (int)target.FirstAware + firstMovementTime, kraitsRadius, ((int)target.FirstAware, (int)target.FirstAware + firstMovementTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                    replay.Decorations.Add(new CircleDecoration(true, 0, kraitsRadius, ((int)target.FirstAware + firstMovementTime, (int)target.LastAware), "rgba(250, 0, 0, 0.2)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.LichHallucination:
                    // Terrifying Apparition
                    int awareTime = 1000;
                    int lishRadius = 280;

                    replay.Decorations.Add(new CircleDecoration(true, (int)target.FirstAware + awareTime, lishRadius, ((int)target.FirstAware, (int)target.FirstAware + awareTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                    replay.Decorations.Add(new CircleDecoration(true, 0, lishRadius, ((int)target.FirstAware + awareTime, (int)target.LastAware), "rgba(250, 0, 0, 0.2)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.QuaggansHallucinationNM:
                    var waveOfTormentNM = casts.Where(x => x.SkillId == WaveOfTormentNM).ToList();
                    foreach (AbstractCastEvent c in waveOfTormentNM)
                    {
                        int castTime = 2800;
                        int radius = 300;
                        int endTime = (int)c.Time + castTime;

                        replay.Decorations.Add(new CircleDecoration(true, endTime, radius, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, radius, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.QuaggansHallucinationCM:
                    var waveOfTormentCM = casts.Where(x => x.SkillId == WaveOfTormentCM).ToList();
                    foreach (AbstractCastEvent c in waveOfTormentCM)
                    {
                        int castTime = 5600;
                        int radius = 450;
                        int endTime = (int)c.Time + castTime;

                        replay.Decorations.Add(new CircleDecoration(true, endTime, radius, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, radius, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.ZhaitansReach:
                    // Thrash - Circle that pulls in
                    var thrash = casts.Where(x => x.SkillId == ZhaitansReachThrashXJJ1 || x.SkillId == ZhaitansReachThrashXJJ2).ToList();
                    foreach (AbstractCastEvent c in thrash)
                    {
                        int castTime = 1900;
                        int endTime = (int)c.Time + castTime;

                        replay.Decorations.Add(new DoughnutDecoration(true, endTime, 300, 500, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new DoughnutDecoration(true, 0, 300, 500, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                    }
                    // Ground Slam - AoE that knocks out
                    var groundSlam = casts.Where(x => x.SkillId == ZhaitansReachGroundSlam || x.SkillId == ZhaitansReachGroundSlamXJJ).ToList();
                    foreach (AbstractCastEvent c in groundSlam)
                    {
                        int castTime = 0;
                        int radius = 400;
                        int endTime = 0;
                        // 66534 -> Fast AoE -- 66397 -> Slow AoE
                        if (c.SkillId == ZhaitansReachGroundSlam) { castTime = 800; } else if (c.SkillId == ZhaitansReachGroundSlamXJJ) { castTime = 2500; }
                        endTime = (int)c.Time + castTime;

                        replay.Decorations.Add(new CircleDecoration(true, endTime, radius, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, radius, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.ReanimatedSpite:
                    break;
                case (int)ArcDPSEnums.TrashID.SanctuaryPrism:
                    if (!log.FightData.IsCM)
                    {
                        replay.Trim(log.FightData.LogStart, log.FightData.LogStart);
                    }
                    break;
                default:
                    break;
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            if (p.GetBuffGraphs(log).TryGetValue(DeathsHandSpreadBuff, out BuffsGraphModel value))
            {
                foreach (Segment segment in value.BuffChart)
                {
                    if (segment != null && segment.Start > 0 && segment.Value == 1)
                    {
                        int radius = 0;
                        int duration = 0;
                        if (!log.FightData.IsCM)
                        {
                            radius = 300;
                            duration = 16000;
                        }
                        else
                        {
                            radius = 380;
                            duration = 36000;
                        }

                        // AoE on player
                        replay.Decorations.Add(new CircleDecoration(true, (int)segment.End, radius, ((int)segment.Start, (int)segment.End), "rgba(250, 120, 0, 0.2)", new AgentConnector(p)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, radius, ((int)segment.Start, (int)segment.End), "rgba(250, 120, 0, 0.2)", new AgentConnector(p)));

                        ParametricPoint3D playerPosition = p.GetCombatReplayPolledPositions(log).Where(x => x.Time <= (int)segment.End).LastOrDefault();
                        if (playerPosition != null)
                        {
                            // Growing AoE
                            replay.Decorations.Add(new CircleDecoration(true, (int)segment.End + 3000, radius, ((int)segment.End, (int)segment.End + 3000), "rgba(250, 120, 0, 0.2)", new PositionConnector(playerPosition)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, radius, ((int)segment.End, (int)segment.End + 3000), "rgba(250, 120, 0, 0.2)", new PositionConnector(playerPosition)));
                            // Damaging AoE
                            replay.Decorations.Add(new DoughnutDecoration(true, 0, radius - 10, radius, ((int)segment.End + 3000, (int)segment.End + duration), "rgba(255, 0, 0, 0.4)", new PositionConnector(playerPosition)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, radius, ((int)segment.End + 3000, (int)segment.End + duration), "rgba(0, 100, 0, 0.2)", new PositionConnector(playerPosition)));
                        }
                    }
                }
            }
        }

        private static void AddDeathEmbraceDecoration(CombatReplay replay, int startCast, int durationCast, int radius, int delay, Point3D position)
        {
            int endTime = startCast + durationCast;
            var connector = new PositionConnector(position);
            replay.Decorations.Add(new CircleDecoration(true, startCast + delay, radius, (startCast, startCast + delay), "rgba(250, 120, 0, 0.2)", connector));
            replay.Decorations.Add(new CircleDecoration(true, 0, radius, (startCast + delay, endTime), "rgba(250, 0, 0, 0.2)", connector));
        }
    }
}
