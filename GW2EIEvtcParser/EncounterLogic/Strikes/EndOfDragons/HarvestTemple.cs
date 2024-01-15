using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class HarvestTemple : EndOfDragonsStrike
    {

        private IReadOnlyList<AbstractSingleActor> FirstAwareSortedTargets { get; set; }
        public HarvestTemple(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                // General
                new PlayerDstEffectMechanic(new [] { EffectGUIDs.HarvestTempleSpreadNM, EffectGUIDs.HarvestTempleSpreadCM }, "Spread Bait", new MechanicPlotlySetting(Symbols.Circle, Colors.Yellow), "Spread.B", "Baited spread mechanic", "Spread Bait", 150),
                new PlayerDstEffectMechanic(new [] { EffectGUIDs.HarvestTempleRedPuddleSelectNM, EffectGUIDs.HarvestTempleRedPuddleSelectCM }, "Red Bait", new MechanicPlotlySetting(Symbols.Circle, Colors.Red), "Red.B", "Baited red puddle mechanic", "Red Bait", 150),
                new PlayerDstBuffApplyMechanic(InfluenceOfTheVoidBuff, "Influence of the Void", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkPurple), "Void.D", "Received Void debuff", "Void Debuff", 150),
                new PlayerDstHitMechanic(InfluenceOfTheVoidSkill, "Influence of the Void Hit", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.DarkPurple), "Void.H", "Hit by Void", "Void Hit", 150),
                new PlayerDstHitMechanic(new [] { VoidPoolNM, VoidPoolCM }, "Void Pool", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkPurple), "Red.H", "Hit by Red Void Pool", "Void Pool", 150),
                new PlayerDstSkillMechanic(new [] { HarvestTempleTargetedExpulsionNM, HarvestTempleTargetedExpulsionCM }, "Targeted Expulsion", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Orange), "Spread.H", "Hit by Targeted Expulsion (Spread)", "Targeted Expulsion (Spread)", 150).UsingChecker((@event, log) => @event.HasHit || @event.DoubleProcHit),
                new PlayerSrcAllHitsMechanic("Orb Push", new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightOrange), "Orb Push", "Orb was pushed by player", "Orb Push", 0).UsingChecker((de, log) => (de.To.IsSpecies(ArcDPSEnums.TrashID.PushableVoidAmalgamate) || de.To.IsSpecies(ArcDPSEnums.TrashID.KillableVoidAmalgamate)) && de is DirectHealthDamageEvent),
                new PlayerDstHitMechanic(new [] { Shockwave, TsunamiSlam1, TsunamiSlam2 }, "Shockwaves", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Yellow), "NopeRopes.Achiv", "Achievement Elibigility: Jumping the Nope Ropes", "Achiv Jumping Nope Ropes", 150).UsingAchievementEligibility(true),
                new PlayerDstHitMechanic(new long [] { VoidExplosion, VoidExplosion2, VoidExplosion3 }, "Void Explosion", new MechanicPlotlySetting(Symbols.StarSquareOpenDot, Colors.Yellow), "VoidExp.H", "Hit by Void Explosion (Last Laugh)", "Void Explosion", 0),
                new PlayerDstHitMechanic(MagicDischarge, "Magic Discharge", new MechanicPlotlySetting(Symbols.Octagon, Colors.Grey), "MagicDisc.H", "Hit by Magic Discharge (Orb Explosion Wave)", "Magic Discharge", 0),
                new EnemySrcEffectMechanic(EffectGUIDs.HarvestTempleGreen, "Success Green", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkGreen), "S.Green", "Green Successful", "Success Green", 0),
                new EnemySrcEffectMechanic(EffectGUIDs.HarvestTempleFailedGreen, "Failed Green", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "F.Green", "Green Failed", "Failed Green", 0),
                // Purification 1
                new PlayerDstHitMechanic(LightningOfJormag, "Lightning of Jormag", new MechanicPlotlySetting(Symbols.StarTriangleDown, Colors.Ice), "Light.H", "Hit by Lightning of Jormag", "Lightning of Jormag", 0),
                new PlayerDstHitMechanic(FlamesOfPrimordus, "Flames of Primordus", new MechanicPlotlySetting(Symbols.StarTriangleDownOpen, Colors.Orange), "Flame.H", "Hit by Flames of Primordus", "Flames of Primordus", 0),
                new PlayerDstHitMechanic(Stormfall, "Stormfall", new MechanicPlotlySetting(Symbols.YUpOpen, Colors.Purple), "Storm.H", "Hit by Kralkatorrik's Stormfall", "Kralkatorrik's Stormfall", 0),
                // Jormag
                new PlayerDstHitMechanic(new [] { BreathOfJormag1, BreathOfJormag2, BreathOfJormag3 }, "Breath of Jormag", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Blue), "J.Breath.H", "Hit by Jormag Breath", "Jormag Breath", 150),
                new PlayerDstHitMechanic(GraspOfJormag, "Grasp of Jormag", new MechanicPlotlySetting(Symbols.StarOpen, Colors.DarkWhite), "J.Grasp.H", "Hit by Grasp of Jormag", "Grasp of Jormag", 0),
                new PlayerDstHitMechanic(FrostMeteor, "Frost Meteor", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Blue), "J.Meteor.H", "Hit by Jormag Meteor", "Jormag Meteor", 150),
                // Primordus
                new PlayerDstHitMechanic(LavaSlam, "Lava Slam", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Red), "Slam.H", "Hit by Primordus Slam", "Primordus Slam", 150),
                new PlayerDstHitMechanic(JawsOfDestruction, "Jaws of Destruction", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Red), "Jaws.H", "Hit by Primordus Jaws", "Primordus Jaws", 150),
                // Kralkatorrik 
                new PlayerDstHitMechanic(CrystalBarrage, "Crystal Barrage", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Purple), "Barrage.H", "Hit by Crystal Barrage", "Barrage", 150),
                new PlayerDstHitMechanic(BrandingBeam, "Branding Beam", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Purple), "Beam.H", "Hit by Kralkatorrik's Branding Beam", "Kralkatorrik Beam", 150),
                new PlayerDstHitMechanic(BrandedArtillery, "Branded Artillery", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Purple), "Artillery.H", "Hit by Brandbomber Artillery", "Brandbomber Artillery", 150),
                new PlayerDstHitMechanic(VoidPoolKralkatorrik, "Void Pool", new MechanicPlotlySetting(Symbols.Circle, Colors.Black), "K.Pool.H", "Hit by Kralkatorrik Void Pool", "Kralkatorrik Void Pool", 150),
                // Purification 2
                new PlayerDstHitMechanic(SwarmOfMordremoth_PoolOfUndeath, "Pool of Undeath", new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "Goop.H", "Hit by goop left by heart", "Heart Goop", 150),
                new PlayerDstHitMechanic(SwarmOfMordremoth, "Swarm of Mordremoth", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.Red), "Bees.H", "Hit by bees from heart", "Heart Bees", 150),
                // Timecaster
                new PlayerDstHitMechanic(GravityCrushDamage, "Gravity Crush", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Black), "Grav.Cru.H", "Hit by Gravity Crush", "Gravity Crush", 0),
                new PlayerDstHitMechanic(NightmareEpochDamage, "Nightmare Epoch", new MechanicPlotlySetting(Symbols.Hexagon, Colors.Pink), "NigEpoch.H", "Hit by Nightmare Epoch", "Nightmare Epoch", 0),
                // Mordremoth
                new PlayerDstHitMechanic(Shockwave, "Shock Wave", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Green), "ShckWv.H", "Hit by Mordremoth Shockwave", "Mordremoth Shockwave", 150),
                new PlayerDstHitMechanic(Kick, "Kick", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Green), "Kick.H", "Kicked by Void Skullpiercer", "Skullpiercer Kick", 150).UsingChecker((ahde, log) => !ahde.To.HasBuff(log, Stability, ahde.Time - ServerDelayConstant)),
                new PlayerDstHitMechanic(PoisonRoar, "Poison Roar", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Green), "M.Poison.H", "Hit by Mordremoth Poison", "Mordremoth Poison", 150),// Giants
                new PlayerDstHitMechanic(DeathScream, "Death Scream", new MechanicPlotlySetting(Symbols.SquareOpen, Colors.Grey), "Scream.G.CC", "CC'd by Giant's Death Scream", "Death Scream", 0).UsingChecker((ahde, log) => !ahde.To.HasBuff(log, Stability, ahde.Time - ServerDelayConstant)),
                new PlayerDstHitMechanic(RottingBile, "Rotting Bile", new MechanicPlotlySetting(Symbols.Square, Colors.GreenishYellow), "RotBile.H", "Hit by Giant's Rotting Bile", "Rotting Bile", 0),
                new PlayerDstHitMechanic(Stomp, "Stomp", new MechanicPlotlySetting(Symbols.StarSquare, Colors.Teal), "Stomp.CC", "CC'd by Giant's Stomp", "Stomp", 0).UsingChecker((ahde, log) => !ahde.To.HasBuff(log, Stability, ahde.Time - ServerDelayConstant)),
                // Zhaitan
                new PlayerDstHitMechanic(new []{ ScreamOfZhaitanNM, ScreamOfZhaitanCM }, "Scream of Zhaitan", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.DarkGreen), "Scream.H", "Hit by Zhaitan Scream", "Zhaitan Scream", 150),
                new PlayerDstHitMechanic(PutridDeluge, "Putrid Deluge", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.DarkGreen), "Z.Poison.H", "Hit by Zhaitan Poison", "Zhaitan Poison", 150),
                new PlayerDstHitMechanic(ZhaitanTailSlam, "Slam", new MechanicPlotlySetting(Symbols.Circle, Colors.Grey), "Slam.H", "Hit by Zhaitan's Tail Slam", "Zhaitan Slam", 0),
                // Purification 3
                new PlayerDstHitMechanic(SwarmOfMordremoth_CorruptedWaters, "Corrupted Waters", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.LightBlue), "Prjtile.H", "Hit by Corrupted Waters (Heart Projectile)", "Heart Projectile", 150),
                // Saltspray
                new PlayerDstHitMechanic(HydroBurst, "Hydro Burst", new MechanicPlotlySetting(Symbols.Circle, Colors.LightBlue), "Whrlpl.H", "Hit by Hydro Burst (Whirlpool)", "Hydro Burst (Whirlpool)", 150),
                new PlayerDstHitMechanic(CallLightning, "Call Lightning", new MechanicPlotlySetting(Symbols.TriangleDownOpen, Colors.Purple), "CallLigh.H", "Hit by Call Lightning", "Call Lightning", 0),
                new PlayerDstHitMechanic(FrozenFury, "Frozen Fury", new MechanicPlotlySetting(Symbols.TriangleRightOpen, Colors.Ice), "FrozFury.H", "Hit by Frozen Fury", "Frozen Fury", 0),
                new PlayerDstHitMechanic(RollingFlame, "Rolling Flame", new MechanicPlotlySetting(Symbols.Circle, Colors.LightRed), "RollFlame.H", "Hit by Rolling Flame", "Rolling Flame", 0),
                new PlayerDstHitMechanic(new long[] { ShatterEarth, ShatterEarth2 }, "Shatter Earth", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Brown), "ShatEarth.H", "Hit by Shatter Earth", "Shatter Earth", 0),
                // Soo Won
                new PlayerDstHitMechanic(new [] { TsunamiSlam1, TsunamiSlam2 }, "Tsunami Slam", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.LightBlue), "Tsunami.H", "Hit by Soo-Won Tsunami", "Soo-Won Tsunami", 150),
                new PlayerDstHitMechanic(ClawSlap, "Claw Slap", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.LightBlue), "Claw.H", "Hit by Soo-Won Claw", "Soo-Won Claw", 150),
                new PlayerDstHitMechanic(VoidPoolSooWon, "Void Pool", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkPink), "SW.Pool.H", "Hit by Soo-Won Void Pool", "Soo-Won Void Pool", 150),
                new PlayerDstHitMechanic(TailSlam, "Tail Slam", new MechanicPlotlySetting(Symbols.Square, Colors.LightBlue), "Tail.H", "Hit by Soo-Won Tail", "Soo-Won Tail", 150),
                new PlayerDstHitMechanic(TormentOfTheVoid, "Torment of the Void", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkMagenta), "Torment.H", "Hit by Torment of the Void (Bouncing Orbs)", "Torment of the Void", 150),
                // Purification 4
                new PlayerDstHitMechanic(GraspOfTheVoid, "Grasp of the Void", new MechanicPlotlySetting(Symbols.Hexagram, Colors.Black), "GraspVoid.H", "Hit by Grasp of the Void (Final Orb Projectile)", "Grasp of the Void", 0),
                // Obliterator
                new PlayerDstHitMechanic(VoidObliteratorFirebomb, "Firebomb", new MechanicPlotlySetting(Symbols.TriangleNW, Colors.DarkTeal), "Firebomb.H", "Hit by Firebomb", "Firebomb", 0),
                new PlayerDstHitMechanic(VoidObliteratorWyvernBreathDamage, "Wyvern Breath", new MechanicPlotlySetting(Symbols.TriangleNEOpen, Colors.Magenta), "WyvBreath.H", "Hit by Wyvern Breath", "Wyvern Breath", 0),
                new PlayerDstHitMechanic(VoidObliteratorCharge, "Charge", new MechanicPlotlySetting(Symbols.Diamond, Colors.Teal), "Charge.H", "Hit by Obliterator's Charge", "Charge Hit", 0),
                new PlayerDstHitMechanic(VoidObliteratorCharge, "Charge", new MechanicPlotlySetting(Symbols.PentagonOpen, Colors.Revenant), "Charge.CC", "CC'd by Obliterator's Charge", "Charge CC", 0).UsingChecker((ahde, log) => !ahde.To.HasBuff(log, Stability, ahde.Time - ServerDelayConstant)),
                // Goliath
                new PlayerDstHitMechanic(GlacialSlam, "Glacial Slam", new MechanicPlotlySetting(Symbols.CircleX, Colors.Ice), "GlaSlam.H", "Hit by Glacial Slam", "Glacial Slam Hit", 0),
                new PlayerDstHitMechanic(GlacialSlam, "Glacial Slam", new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Ice), "GlaSlam.CC", "CC'd by Glacial Slam", "Glacial Slam CC", 0).UsingChecker((ahde, log) => !ahde.To.HasBuff(log, Stability, ahde.Time - ServerDelayConstant)),
            }
            );
            Icon = EncounterIconHarvestTemple;
            Extension = "harvsttmpl";
            EncounterCategoryInformation.InSubCategoryOrder = 3;
            EncounterID |= 0x000004;
        }
        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayHarvestTemple,
                            (1024, 1024),
                            (-812, -21820, 2037, -18971)/*,
                            (-15360, -36864, 15360, 39936),
                            (3456, 11012, 4736, 14212)*/);
        }
        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            var subPhasesData = new List<(long start, long end, string name, NPC target, bool canBeSubPhase)>();
            var giants = new List<NPC>();
            foreach (NPC target in Targets)
            {
                long mainPhaseEnd = Math.Min(target.LastAware, log.FightData.FightEnd);
                switch (target.ID)
                {
                    case (int)ArcDPSEnums.TrashID.VoidGiant:
                        giants.Add(target);
                        break;
                    case (int)ArcDPSEnums.TrashID.VoidSaltsprayDragon:
                    case (int)ArcDPSEnums.TrashID.VoidTimeCaster:
                    case (int)ArcDPSEnums.TrashID.VoidObliterator:
                    case (int)ArcDPSEnums.TrashID.VoidGoliath:
                        subPhasesData.Add((target.FirstAware, mainPhaseEnd, target.Character, target, false));
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidJormag:
                        phases[0].AddTarget(target);
                        subPhasesData.Add((target.FirstAware, mainPhaseEnd, "Jormag", target, true));
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidKralkatorrik:
                        phases[0].AddTarget(target);
                        subPhasesData.Add((target.FirstAware, mainPhaseEnd, "Kralkatorrik", target, true));
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidMordremoth:
                        phases[0].AddTarget(target);
                        subPhasesData.Add((target.FirstAware, mainPhaseEnd, "Mordremoth", target, true));
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidPrimordus:
                        phases[0].AddTarget(target);
                        subPhasesData.Add((target.FirstAware, mainPhaseEnd, "Primordus", target, true));
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidSooWon:
                        phases[0].AddTarget(target);
                        subPhasesData.Add((target.FirstAware, mainPhaseEnd, "Soo-Won", target, true));
                        AttackTargetEvent attackTargetEvent = log.CombatData.GetAttackTargetEvents(target.AgentItem).FirstOrDefault();
                        if (attackTargetEvent != null)
                        {
                            var targetables = log.CombatData.GetTargetableEvents(attackTargetEvent.AttackTarget).Where(x => x.Time >= target.FirstAware).ToList();
                            var targetOns = targetables.Where(x => x.Targetable).ToList();
                            var targetOffs = targetables.Where(x => !x.Targetable).ToList();
                            int id = 0;
                            foreach (TargetableEvent targetOn in targetOns)
                            {
                                long start = targetOn.Time;
                                long end = log.FightData.FightEnd;
                                TargetableEvent targetOff = targetOffs.FirstOrDefault(x => x.Time > start);
                                if (targetOff != null)
                                {
                                    end = targetOff.Time;
                                }
                                subPhasesData.Add((start, end, "Soo-Won " + (++id), target, true));
                            }
                        }
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidZhaitan:
                        phases[0].AddTarget(target);
                        subPhasesData.Add((target.FirstAware, mainPhaseEnd, "Zhaitan", target, true));
                        break;
                }
            }
            if (!requirePhases)
            {
                return phases;
            }

            if (giants.Count > 0)
            {
                long start = log.FightData.FightEnd;
                long end = log.FightData.FightStart;
                foreach (NPC giant in giants)
                {
                    start = Math.Min(start, giant.FirstAware);
                    end = Math.Max(end, giant.LastAware);
                }
                var subPhase = new PhaseData(start, end, "Giants");
                subPhase.AddTargets(giants);
                subPhase.OverrideEndTime(log);
                phases.Add(subPhase);
            }

            foreach ((long start, long end, string name, NPC target, bool canBeSubPhase) in subPhasesData)
            {
                var subPhase = new PhaseData(start, end, name);
                subPhase.CanBeSubPhase = canBeSubPhase;
                subPhase.AddTarget(target);
                phases.Add(subPhase);
            }
            int purificationID = 0;
            foreach (NPC voidAmal in Targets.Where(x => x.IsSpecies(ArcDPSEnums.TrashID.PushableVoidAmalgamate) || x.IsSpecies(ArcDPSEnums.TrashID.KillableVoidAmalgamate)))
            {
                long end;
                DeadEvent deadEvent = log.CombatData.GetDeadEvents(voidAmal.AgentItem).LastOrDefault();
                if (deadEvent == null)
                {
                    DespawnEvent despawnEvent = log.CombatData.GetDespawnEvents(voidAmal.AgentItem).LastOrDefault();
                    if (despawnEvent == null)
                    {
                        end = voidAmal.LastAware;
                    }
                    else
                    {
                        end = despawnEvent.Time;
                    }
                }
                else
                {
                    end = deadEvent.Time;
                }
                var purificationPhase = new PhaseData(Math.Max(voidAmal.FirstAware, log.FightData.FightStart), Math.Min(end, log.FightData.FightEnd), "Purification " + (++purificationID));
                purificationPhase.AddTarget(voidAmal);
                phases.Add(purificationPhase);
            }
            return phases;
        }

        protected override List<int> GetSuccessCheckIDs()
        {
            return new List<int>
            {
            };
        }

        internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
        {
            // To investigate
            return FightData.EncounterStartStatus.Normal;
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            long startToUse = GetGenericFightOffset(fightData);
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogStartNPCUpdate);
            if (logStartNPCUpdate != null)
            {
                AgentItem firstAmalgamate = agentData.GetNPCsByID(ArcDPSEnums.TrashID.VoidAmalgamate).MinBy(x => x.FirstAware);
                if (firstAmalgamate != null)
                {
                    startToUse = firstAmalgamate.FirstAware;
                }
            }
            return startToUse;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.TheDragonVoidJormag,
                (int)ArcDPSEnums.TargetID.TheDragonVoidKralkatorrik,
                (int)ArcDPSEnums.TargetID.TheDragonVoidMordremoth,
                (int)ArcDPSEnums.TargetID.TheDragonVoidPrimordus,
                (int)ArcDPSEnums.TargetID.TheDragonVoidSooWon,
                (int)ArcDPSEnums.TargetID.TheDragonVoidZhaitan,
                (int)ArcDPSEnums.TrashID.VoidSaltsprayDragon,
                (int)ArcDPSEnums.TrashID.VoidObliterator,
                (int)ArcDPSEnums.TrashID.VoidGoliath,
                (int)ArcDPSEnums.TrashID.VoidTimeCaster,
                (int)ArcDPSEnums.TrashID.PushableVoidAmalgamate,
                (int)ArcDPSEnums.TrashID.KillableVoidAmalgamate,
                (int)ArcDPSEnums.TrashID.VoidGiant
            };
        }

        protected override Dictionary<int, int> GetTargetsSortIDs()
        {
            return new Dictionary<int, int>()
            {
                {(int)ArcDPSEnums.TargetID.TheDragonVoidJormag, 0 },
                {(int)ArcDPSEnums.TargetID.TheDragonVoidKralkatorrik, 0 },
                {(int)ArcDPSEnums.TargetID.TheDragonVoidMordremoth, 0 },
                {(int)ArcDPSEnums.TargetID.TheDragonVoidPrimordus, 0 },
                {(int)ArcDPSEnums.TargetID.TheDragonVoidZhaitan, 0 },
                {(int)ArcDPSEnums.TargetID.TheDragonVoidSooWon, 0 },
                {(int)ArcDPSEnums.TrashID.PushableVoidAmalgamate, 1 },
                {(int)ArcDPSEnums.TrashID.KillableVoidAmalgamate, 1 },
                {(int)ArcDPSEnums.TrashID.VoidSaltsprayDragon, 1 },
                {(int)ArcDPSEnums.TrashID.VoidObliterator, 1 },
                {(int)ArcDPSEnums.TrashID.VoidGoliath, 1 },
                {(int)ArcDPSEnums.TrashID.VoidTimeCaster, 1 },
                {(int)ArcDPSEnums.TrashID.VoidGiant, 1},
            };
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.ZhaitansReach,
                ArcDPSEnums.TrashID.VoidAbomination,
                ArcDPSEnums.TrashID.VoidAmalgamate,
                ArcDPSEnums.TrashID.VoidBrandbomber,
                ArcDPSEnums.TrashID.VoidBurster,
                ArcDPSEnums.TrashID.VoidColdsteel,
                ArcDPSEnums.TrashID.VoidMelter,
                ArcDPSEnums.TrashID.VoidRotswarmer,
                ArcDPSEnums.TrashID.VoidSkullpiercer,
                ArcDPSEnums.TrashID.VoidStormseer,
                ArcDPSEnums.TrashID.VoidTangler,
                ArcDPSEnums.TrashID.VoidWarforged1,
                ArcDPSEnums.TrashID.VoidWarforged2,
                ArcDPSEnums.TrashID.DragonBodyVoidAmalgamate,
                ArcDPSEnums.TrashID.DragonEnergyOrb,
                ArcDPSEnums.TrashID.GravityBall,
                ArcDPSEnums.TrashID.JormagMovingFrostBeam,
            };
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Harvest Temple";
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            // no bouny chest detection, the reward is delayed
            AbstractSingleActor soowon = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.TheDragonVoidSooWon));
            if (soowon != null)
            {
                AttackTargetEvent attackTargetEvent = combatData.GetAttackTargetEvents(soowon.AgentItem).FirstOrDefault();
                if (attackTargetEvent == null)
                {
                    return;
                }
                var targetables = combatData.GetTargetableEvents(attackTargetEvent.AttackTarget).Where(x => x.Time >= soowon.FirstAware).ToList();
                var targetOffs = targetables.Where(x => !x.Targetable).ToList();
                if (targetOffs.Count == 2)
                {
                    AbstractHealthDamageEvent lastDamageTaken = combatData.GetDamageTakenData(soowon.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                    if (lastDamageTaken != null)
                    {
                        bool isSuccess = false;
                        var determinedApplies = combatData.GetBuffData(Determined895).OfType<BuffApplyEvent>().Where(x => x.To.IsPlayer && x.Time >= targetOffs[1].Time - 150).ToList();
                        IReadOnlyList<AnimatedCastEvent> liftOffs = combatData.GetAnimatedCastData(HarvestTempleLiftOff);
                        foreach (AnimatedCastEvent liffOff in liftOffs)
                        {
                            isSuccess = true;
                            if (determinedApplies.Count(x => x.To == liffOff.Caster && Math.Abs(x.Time - liffOff.Time) < ServerDelayConstant) != 1)
                            {
                                isSuccess = false;
                                break;
                            }
                        }
                        if (isSuccess)
                        {
                            fightData.SetSuccess(true, targetOffs[1].Time);
                        }
                    }
                }
            }
        }

        internal override void EIEvtcParse(ulong gw2Build, int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            bool needRefreshAgentPool = false;
            //
            var dragonOrbMaxHPs = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate && x.DstAgent == 491550).ToList();
            foreach (CombatItem dragonOrbMaxHP in dragonOrbMaxHPs)
            {
                AgentItem dragonOrb = agentData.GetAgent(dragonOrbMaxHP.SrcAgent, dragonOrbMaxHP.Time);
                if (dragonOrb != _unknownAgent)
                {
                    dragonOrb.OverrideName("Dragon Orb");
                    dragonOrb.OverrideID(ArcDPSEnums.TrashID.DragonEnergyOrb);
                }
            }
            if (dragonOrbMaxHPs.Any())
            {
                needRefreshAgentPool = true;
            }
            //
            var attackTargetEvents = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.AttackTarget).ToList();
            var idsToUse = new List<ArcDPSEnums.TargetID> {
                ArcDPSEnums.TargetID.TheDragonVoidJormag,
                ArcDPSEnums.TargetID.TheDragonVoidPrimordus,
                ArcDPSEnums.TargetID.TheDragonVoidKralkatorrik,
                ArcDPSEnums.TargetID.TheDragonVoidMordremoth,
                ArcDPSEnums.TargetID.TheDragonVoidZhaitan,
                ArcDPSEnums.TargetID.TheDragonVoidSooWon,
            };
            var targetableEvents = combatData.Where(y => y.IsStateChange == ArcDPSEnums.StateChange.Targetable).GroupBy(x => agentData.GetAgent(x.SrcAgent, x.Time)).ToDictionary(x => x.Key, x => x.Where(y => y.Time > 2000).ToList());
            attackTargetEvents = attackTargetEvents.OrderBy(x =>
            {
                AgentItem atAgent = agentData.GetAgent(x.SrcAgent, x.Time);
                if (targetableEvents.TryGetValue(atAgent, out List<CombatItem> targetables))
                {
                    return targetables.Any() ? targetables.Min(y => y.Time) : long.MaxValue;
                }
                return long.MaxValue;
            }).ToList();
            int index = 0;
            var processedAttackTargets = new HashSet<AgentItem>();
            foreach (CombatItem at in attackTargetEvents)
            {
                AgentItem atAgent = agentData.GetAgent(at.SrcAgent, at.Time);
                // We take attack events, filter out the first one, present at spawn, that is always a non targetable event
                // There are only two relevant attack targets, one represents the first five and the last one Soo Won
                if (processedAttackTargets.Contains(atAgent) || !targetableEvents.TryGetValue(atAgent, out List<CombatItem> targetables) || !targetables.Any())
                {
                    continue;
                }
                AgentItem dragonVoid = agentData.GetAgent(at.DstAgent, at.Time);
                var copyEventsFrom = new List<AgentItem>() { dragonVoid };
                processedAttackTargets.Add(atAgent);
                var targetOns = targetables.Where(x => x.DstAgent == 1).ToList();
                var targetOffs = targetables.Where(x => x.DstAgent == 0).ToList();
                //
                foreach (CombatItem targetOn in targetOns)
                {
                    // If Soo Won has been already created, we break
                    if (index >= idsToUse.Count)
                    {
                        break;
                    }
                    ArcDPSEnums.TargetID id = idsToUse[index++];
                    long start = targetOn.Time;
                    long end = dragonVoid.LastAware;
                    CombatItem targetOff = targetOffs.FirstOrDefault(x => x.Time > start);
                    // Don't split Soo won into two
                    if (targetOff != null && id != ArcDPSEnums.TargetID.TheDragonVoidSooWon)
                    {
                        end = targetOff.Time;
                    }
                    ulong lastHPUpdate = ulong.MaxValue;
                    AgentItem extra = agentData.AddCustomNPCAgent(start, end, dragonVoid.Name, dragonVoid.Spec, id, false, dragonVoid.Toughness, dragonVoid.Healing, dragonVoid.Condition, dragonVoid.Concentration, atAgent.HitboxWidth, atAgent.HitboxHeight);
                    RedirectEventsAndCopyPreviousStates(combatData, extensions, agentData, dragonVoid, copyEventsFrom, extra, true,
                        (evt, from, to) =>
                        {
                            if (evt.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate)
                            {
                                // Avoid making the gadget go back to 100% hp on "death"
                                // Regenerating back to full HP
                                // use mid life check to allow hp going back up to 100% around first aware
                                if (evt.DstAgent > lastHPUpdate && evt.DstAgent > 9900 && evt.Time > (to.LastAware + to.FirstAware) / 2)
                                {
                                    return false;
                                }
                                // Remember last hp
                                lastHPUpdate = evt.DstAgent;
                            }
                            return true;
                        }
                    );
                    copyEventsFrom.Add(extra);
                }
            }
            //
            IReadOnlyList<AgentItem> voidAmalgamates = agentData.GetNPCsByID(ArcDPSEnums.TrashID.VoidAmalgamate);
            foreach (AgentItem voidAmal in voidAmalgamates)
            {
                if (combatData.Where(x => x.SkillID == VoidShell && x.IsBuffApply() && x.SrcMatchesAgent(voidAmal)).Any())
                {
                    voidAmal.OverrideID(ArcDPSEnums.TrashID.PushableVoidAmalgamate);
                    needRefreshAgentPool = true;
                }
            }
            AgentItem dragonBodyVoidAmalgamate = voidAmalgamates.MaxBy(x => x.LastAware - x.FirstAware);
            if (dragonBodyVoidAmalgamate != null)
            {
                dragonBodyVoidAmalgamate.OverrideID(ArcDPSEnums.TrashID.DragonBodyVoidAmalgamate);
                needRefreshAgentPool = true;
            }
            // Gravity Ball - Timecaster gadget
            AgentItem timecaster = agentData.GetNPCsByID(ArcDPSEnums.TrashID.VoidTimeCaster).FirstOrDefault();
            if (timecaster != null)
            {
                var gravityBalls = combatData.Where(x => x.DstAgent == 14940 && x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxHeight == 300 && x.HitboxWidth == 100 && x.Master == null && x.FirstAware > timecaster.FirstAware && x.FirstAware < timecaster.LastAware + 2000).ToList();
                var candidateVelocities = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.Velocity && gravityBalls.Any(y => x.SrcMatchesAgent(y))).ToList();
                int referenceLength = 200;
                gravityBalls = gravityBalls.Where(x => candidateVelocities.Any(y => Math.Abs(AbstractMovementEvent.GetPoint3D(y.DstAgent, y.Value).Length() - referenceLength) < 10)).ToList();
                foreach (AgentItem ball in gravityBalls)
                {
                    ball.OverrideType(AgentItem.AgentType.NPC);
                    ball.OverrideID(ArcDPSEnums.TrashID.GravityBall);
                    ball.SetMaster(timecaster);
                    needRefreshAgentPool = true;
                }
            }
            {
                AgentItem jormagAgent = agentData.GetNPCsByID(ArcDPSEnums.TargetID.TheDragonVoidJormag).FirstOrDefault();
                if (jormagAgent != null)
                {
                    var frostBeams = combatData.Where(evt => evt.SrcIsAgent() && agentData.GetAgent(evt.SrcAgent, evt.Time).IsSpecies(ArcDPSEnums.NonIdentifiedSpecies))
                        .Select(evt => agentData.GetAgent(evt.SrcAgent, evt.Time))
                        .Distinct()
                        .Where(agent => agent.IsNPC && agent.FirstAware >= jormagAgent.FirstAware && agent.LastAware <= jormagAgent.LastAware && combatData.Count(evt => evt.SrcMatchesAgent(agent) && evt.IsStateChange == ArcDPSEnums.StateChange.Velocity && AbstractMovementEvent.GetPoint3D(evt.DstAgent, evt.Value).Length() > 0) > 2)
                        .ToList();
                    foreach (AgentItem frostBeam in frostBeams)
                    {
                        frostBeam.OverrideID(ArcDPSEnums.TrashID.JormagMovingFrostBeam);
                        frostBeam.OverrideType(AgentItem.AgentType.NPC);
                        frostBeam.SetMaster(jormagAgent);
                        needRefreshAgentPool = true;
                    }
                }
            }
            if (needRefreshAgentPool)
            {
                agentData.Refresh();
            }
            // Add missing agents
            for (int i = index; i < idsToUse.Count; i++)
            {
                agentData.AddCustomNPCAgent(int.MaxValue - idsToUse.Count + i, int.MaxValue, "Dragonvoid", Spec.NPC, idsToUse[i], false);
            }
            //
            ComputeFightTargets(agentData, combatData, extensions);
            //
            int purificationID = 0;
            bool needRedirect = false;
            (HashSet<ulong> jormagDamagingAgents, NPC jormag) = (new HashSet<ulong>(), null);
            (HashSet<ulong> primordusDamagingAgents, NPC primordus) = (new HashSet<ulong>(), null);
            (HashSet<ulong> kralkDamagingAgents, NPC kralk) = (new HashSet<ulong>(), null);
            (HashSet<ulong> mordDamagingAgents, NPC mord) = (new HashSet<ulong>(), null);
            (HashSet<ulong> zhaitanDamagingAgents, NPC zhaitan) = (new HashSet<ulong>(), null);
            (HashSet<ulong> soowonDamagingAgents, NPC soowon) = (new HashSet<ulong>(), null);
            foreach (NPC target in Targets)
            {
                switch (target.ID)
                {
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidJormag:
                        target.OverrideName("The JormagVoid");
                        jormag = target;
                        needRedirect = true;
                        var jormagAttacks = new HashSet<long>()
                        {
                            BreathOfJormag1,
                            BreathOfJormag2,
                            BreathOfJormag3,
                            GraspOfJormag,
                            FrostMeteor,
                        };
                        jormagDamagingAgents = new HashSet<ulong>(combatData.Where(x => x.IsDamage() && jormagAttacks.Contains(x.SkillID)).Select(x => x.SrcAgent));
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidKralkatorrik:
                        target.OverrideName("The KralkatorrikVoid");
                        kralk = target;
                        needRedirect = true;
                        var kralkAttacks = new HashSet<long>()
                        {
                            BrandingBeam,
                            CrystalBarrage,
                            VoidPoolKralkatorrik
                        };
                        kralkDamagingAgents = new HashSet<ulong>(combatData.Where(x => x.IsDamage() && kralkAttacks.Contains(x.SkillID)).Select(x => x.SrcAgent));
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidMordremoth:
                        target.OverrideName("The MordremothVoid");
                        mord = target;
                        needRedirect = true;
                        var mordAttacks = new HashSet<long>()
                        {
                            Shockwave,
                            PoisonRoar,
                        };
                        mordDamagingAgents = new HashSet<ulong>(combatData.Where(x => x.IsDamage() && mordAttacks.Contains(x.SkillID)).Select(x => x.SrcAgent));
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidPrimordus:
                        target.OverrideName("The PrimordusVoid");
                        primordus = target;
                        needRedirect = true;
                        var primordusAttacks = new HashSet<long>()
                        {
                            LavaSlam,
                            JawsOfDestruction,
                        };
                        primordusDamagingAgents = new HashSet<ulong>(combatData.Where(x => x.IsDamage() && primordusAttacks.Contains(x.SkillID)).Select(x => x.SrcAgent));
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidSooWon:
                        target.OverrideName("The SooWonVoid");
                        soowon = target;
                        needRedirect = true;
                        var soowonAttacks = new HashSet<long>()
                        {
                            TsunamiSlam1,
                            TsunamiSlam2,
                            ClawSlap,
                            MagicHail,
                            VoidPurge,
                            VoidPoolSooWon,
                            TormentOfTheVoid,
                            TailSlam,
                        };
                        soowonDamagingAgents = new HashSet<ulong>(combatData.Where(x => x.IsDamage() && soowonAttacks.Contains(x.SkillID)).Select(x => x.SrcAgent));
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidZhaitan:
                        target.OverrideName("The ZhaitanVoid");
                        zhaitan = target;
                        needRedirect = true;
                        var zhaiAttacks = new HashSet<long>()
                        {
                            ScreamOfZhaitanNM,
                            ScreamOfZhaitanCM,
                            ZhaitanTailSlam,
                            PutridDeluge
                        };
                        zhaitanDamagingAgents = new HashSet<ulong>(combatData.Where(x => x.IsDamage() && zhaiAttacks.Contains(x.SkillID)).Select(x => x.SrcAgent));
                        break;
                    case (int)ArcDPSEnums.TrashID.PushableVoidAmalgamate:
                    case (int)ArcDPSEnums.TrashID.KillableVoidAmalgamate:
                        target.OverrideName("Heart " + (++purificationID));
                        break;
                }
            }
            if (needRedirect)
            {
                foreach (CombatItem cbt in combatData)
                {
                    if (cbt.IsDamage())
                    {
                        // sanity check
                        if (agentData.GetAgent(cbt.SrcAgent, cbt.Time).GetFinalMaster().IsPlayer)
                        {
                            continue;
                        }
                        if (jormagDamagingAgents.Any(x => cbt.SrcAgent == x && jormag.FirstAware <= cbt.Time && cbt.Time <= jormag.LastAware))
                        {
                            cbt.OverrideSrcAgent(jormag.AgentItem.Agent);
                        }
                        else if (primordusDamagingAgents.Any(x => cbt.SrcAgent == x && primordus.FirstAware <= cbt.Time && cbt.Time <= primordus.LastAware))
                        {
                            cbt.OverrideSrcAgent(primordus.AgentItem.Agent);
                        }
                        else if (kralkDamagingAgents.Any(x => cbt.SrcAgent == x && kralk.FirstAware <= cbt.Time && cbt.Time <= kralk.LastAware))
                        {
                            cbt.OverrideSrcAgent(kralk.AgentItem.Agent);
                        }
                        else if (mordDamagingAgents.Any(x => cbt.SrcAgent == x && mord.FirstAware <= cbt.Time && cbt.Time <= mord.LastAware))
                        {
                            cbt.OverrideSrcAgent(mord.AgentItem.Agent);
                        }
                        else if (zhaitanDamagingAgents.Any(x => cbt.SrcAgent == x && zhaitan.FirstAware <= cbt.Time && cbt.Time <= zhaitan.LastAware))
                        {
                            cbt.OverrideSrcAgent(zhaitan.AgentItem.Agent);
                        }
                        else if (soowonDamagingAgents.Any(x => cbt.SrcAgent == x && soowon.FirstAware <= cbt.Time && cbt.Time <= soowon.LastAware))
                        {
                            cbt.OverrideSrcAgent(soowon.AgentItem.Agent);
                        }
                    }
                }
            }
            FirstAwareSortedTargets = Targets.OrderBy(x => x.FirstAware).ToList();
        }

        internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleGreen, out IReadOnlyList<EffectEvent> greenEffects))
            {
                AddShareTheVoidDecoration(greenEffects, true);
            }
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleFailedGreen, out IReadOnlyList<EffectEvent> failedGreenEffects))
            {
                AddShareTheVoidDecoration(failedGreenEffects, false);
            }
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleRedPuddleCM, out IReadOnlyList<EffectEvent> redPuddleEffectsCM))
            {
                AddPlacedVoidPoolDecoration(redPuddleEffectsCM, 400, 300000);
            }
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleRedPuddleNM, out IReadOnlyList<EffectEvent> redPuddleEffectsNM))
            {
                AddPlacedVoidPoolDecoration(redPuddleEffectsNM, 300, 25000);
            }
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            var knownEffectsIDs = new HashSet<long>();
            IReadOnlyList<AbstractCastEvent> casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

            switch (target.ID)
            {
                case (int)ArcDPSEnums.TrashID.PushableVoidAmalgamate:
                    // Purification Zones
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationZones, out IReadOnlyList<EffectEvent> purificationZoneEffects))
                    {
                        var voidShells = log.CombatData.GetBuffData(VoidShell).Where(x => x.To == target.AgentItem).ToList();
                        var voidShellRemovals = voidShells.Where(x => x is BuffRemoveSingleEvent || x is BuffRemoveAllEvent).ToList();
                        int voidShellAppliesCount = voidShells.Where(x => x is BuffApplyEvent).Count();
                        int voidShellRemovalOffset = 0;
                        int purificationAdd = 0;
                        bool breakPurification = false;
                        foreach (EffectEvent purificationZoneEffect in purificationZoneEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                        {
                            int start = (int)purificationZoneEffect.Time;
                            int end = (int)log.FightData.FightEnd;
                            uint radius = 280;
                            if (voidShellRemovalOffset < voidShellRemovals.Count)
                            {
                                end = (int)voidShellRemovals[voidShellRemovalOffset++].Time;
                            }
                            replay.Decorations.Add(new CircleDecoration(radius, (start, end), Colors.White, 0.4, new PositionConnector(purificationZoneEffect.Position)));
                            purificationAdd++;
                            if (purificationAdd >= voidShellAppliesCount)
                            {
                                breakPurification = true;
                            }
                            if (breakPurification)
                            {
                                break;
                            }
                        }
                    }
                    // Jormag - Lightning of Jormag
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationLightningOfJormag, out IReadOnlyList<EffectEvent> lightningOfJormagEffects))
                    {
                        foreach (EffectEvent lightningEffect in lightningOfJormagEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                        {
                            int duration = 3000;
                            int start = (int)lightningEffect.Time - duration;
                            int end = (int)lightningEffect.Time;
                            var circle = new CircleDecoration(180, (start, end), Colors.LightBlue, 0.2, new PositionConnector(lightningEffect.Position));
                            replay.AddDecorationWithGrowing(circle, end);
                        }
                    }
                    // Primordus - Flames of Primordus
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationFlamesOfPrimordus, out IReadOnlyList<EffectEvent> flamesOfPrimordus))
                    {
                        foreach (EffectEvent fireBallEffect in flamesOfPrimordus.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                        {
                            int startLoad = (int)fireBallEffect.Time - 2000;
                            int endLoad = (int)fireBallEffect.Time;
                            var circle = new CircleDecoration(180, (startLoad, endLoad), Colors.Red, 0.2, new PositionConnector(fireBallEffect.Position));
                            replay.AddDecorationWithGrowing(circle, endLoad);
                            replay.Decorations.Add(new CircleDecoration(180, (endLoad, endLoad + 2000), Colors.Red, 0.4, new PositionConnector(fireBallEffect.Position)));
                        }
                    }
                    // Kralkatorrik - Stormfall (Cracks)
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationStormfall, out IReadOnlyList<EffectEvent> stormfallEffects))
                    {
                        foreach (EffectEvent voidZoneEffect in stormfallEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                        {
                            int start = (int)voidZoneEffect.Time;
                            int end = start + 5000;
                            var connector = new PositionConnector(voidZoneEffect.Position);
                            var rotationConnector = new AngleConnector(voidZoneEffect.Rotation.Z);
                            var rectangle = (RectangleDecoration)new RectangleDecoration(90, 230, (start, end), Colors.DarkMagenta, 0.2, connector).UsingRotationConnector(rotationConnector);
                            replay.AddDecorationWithGrowing(rectangle, end);
                        }
                    }
                    // Mordremoth - Swarm of Mordremoth (Bees)
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationBeeLaunch, out IReadOnlyList<EffectEvent> beeLaunchEffects))
                    {
                        foreach (EffectEvent beeLaunchEffect in beeLaunchEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                        {
                            int start = (int)beeLaunchEffect.Time;
                            int end = start + 3000;
                            replay.Decorations.Add(new RectangleDecoration(380, 30, (start, end), Colors.Red, 0.4, new PositionConnector(beeLaunchEffect.Position).WithOffset(new Point3D(190, 0), true)).UsingRotationConnector(new AngleConnector(beeLaunchEffect.Rotation.Z - 90)));
                            var circle = new CircleDecoration(280, (start, end), Colors.LightOrange, 0.2, new PositionConnector(beeLaunchEffect.Position));
                            replay.AddDecorationWithGrowing(circle, end);
                            var initialPosition = new ParametricPoint3D(beeLaunchEffect.Position, end);
                            int velocity = 210;
                            int lifespan = 15000;
                            var finalPosition = new ParametricPoint3D(initialPosition + (velocity * lifespan / 1000.0f) * new Point3D((float)Math.Cos(beeLaunchEffect.Orientation.Z - Math.PI / 2), (float)Math.Sin(beeLaunchEffect.Orientation.Z - Math.PI/2)), end + lifespan);
                            replay.Decorations.Add(new CircleDecoration(280, (end, end + lifespan), Colors.Red, 0.4, new InterpolationConnector(new List<ParametricPoint3D>() { initialPosition, finalPosition})));
                        }
                    }
                    // Zhaitan - Pool of Undeath
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationPoisonTrail, out IReadOnlyList<EffectEvent> poisonTrailEffects))
                    {
                        foreach (EffectEvent poisonTrailEffect in poisonTrailEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                        {
                            int startLoad = (int)poisonTrailEffect.Time - 1000;
                            int start = (int)poisonTrailEffect.Time;
                            int end = start + 15000;
                            replay.Decorations.Add(new CircleDecoration(220, (startLoad, start), "rgba(0, 150, 0, 0.2)", new PositionConnector(poisonTrailEffect.Position)).UsingGrowingEnd(start));
                            replay.Decorations.Add(new CircleDecoration(220, (start, end), "rgba(0, 150, 0, 0.4)", new PositionConnector(poisonTrailEffect.Position)));
                        }
                    }
                    // Magic Discharge - Orb Explosion
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleOrbExplosion, out IReadOnlyList<EffectEvent> orbEffects))
                    {
                        foreach (EffectEvent orbEffect in orbEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                        {
                            int duration = 3000;
                            int start = (int)orbEffect.Time;
                            int end = start + duration;
                            // Radius is an estimate - orb exploding on edge doesn't quite cover the entirety of the arena
                            uint radius = 2700;
                            var circle = new CircleDecoration(radius, (start, end), Colors.White, 0.05, new PositionConnector(orbEffect.Position));
                            replay.AddDecorationWithGrowing(circle, end);
                        }
                    }
                    // Breakbar Active
                    BreakbarStateEvent breakbar = log.CombatData.GetBreakbarStateEvents(target.AgentItem).FirstOrDefault(x => x.State == ArcDPSEnums.BreakbarState.Active);
                    if (breakbar != null)
                    {
                        int start = (int)breakbar.Time;
                        int end = (int)target.LastAware;
                        replay.Decorations.Add(new CircleDecoration(120, (start, end), Colors.LightBlue, 0.3, new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TargetID.TheDragonVoidJormag:
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleJormagIceShards, out IReadOnlyList<EffectEvent> iceShardEffects))
                    {
                        foreach (EffectEvent iceShardEffect in iceShardEffects)
                        {
                            int duration = 2500;
                            int start = (int)iceShardEffect.Time;
                            int end = start + duration;
                            replay.AddDecorationWithGrowing(new CircleDecoration(160, (start, end), "rgba(0, 50, 180, 0.2)", new PositionConnector(iceShardEffect.Position)), end);
                        }
                    }
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleJormagFrostMeteorIceField, out IReadOnlyList<EffectEvent> meteorEffects))
                    {
                        foreach (EffectEvent effect in meteorEffects)
                        {
                            int indicatorDuration = 1500;
                            int spreadDuration = 3000;
                            int lingerDuration = 9500;
                            int start = (int)effect.Time;
                            int fieldEnd = (int)Math.Min(start + lingerDuration, target.LastAware);
                            // meteor impact
                            replay.AddDecorationWithGrowing(new CircleDecoration(600, (start - indicatorDuration, start), Colors.Orange, 0.2, new PositionConnector(effect.Position)), start);
                            // ice field
                            replay.AddDecorationWithGrowing(new CircleDecoration(1200, (start, fieldEnd), "rgba(69, 182, 254, 0.1)", new PositionConnector(effect.Position)), start + spreadDuration);
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.JormagMovingFrostBeam:
                    VelocityEvent frostBeamMoveStartVelocity = log.CombatData.GetMovementData(target.AgentItem).OfType<VelocityEvent>().FirstOrDefault(x => x.GetPoint3D().Length() > 0);
                    // Beams are immobile at spawn for around 3 seconds
                    if (frostBeamMoveStartVelocity != null)
                    {
                        replay.Trim(frostBeamMoveStartVelocity.Time, target.LastAware);
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.DragonEnergyOrb:
                    (int dragonOrbStart, int dragonOrbEnd) = ((int)target.FirstAware, (int)target.LastAware);
                    replay.Decorations.Add(new CircleDecoration(160, (dragonOrbStart, dragonOrbEnd), "rgba(200, 50, 0, 0.5)", new AgentConnector(target)).UsingFilled(false));
                    break;
                case (int)ArcDPSEnums.TargetID.TheDragonVoidPrimordus:
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePrimordusSmallJaw, out IReadOnlyList<EffectEvent> smallJawEffects))
                    {
                        // The effect is slightly shifted on X
                        var jawPosition = new Point3D(610, -21400.3f, -15417.3f);
                        foreach (EffectEvent smallJawEffect in smallJawEffects)
                        {
                            int duration = 3500;
                            int start = (int)smallJawEffect.Time - duration;
                            int end = (int)smallJawEffect.Time;
                            replay.AddDecorationWithGrowing(new CircleDecoration(560, (start, end), Colors.Orange, 0.2, new PositionConnector(jawPosition)), end);
                        }
                    }
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePrimordusBigJaw, out IReadOnlyList<EffectEvent> bigJawEffects))
                    {
                        foreach (EffectEvent bigJawEffect in bigJawEffects)
                        {
                            int start = (int)bigJawEffect.Time;
                            int end = start + 7500;
                            replay.AddDecorationWithGrowing(new CircleDecoration( 1700, (start, end), Colors.Orange, 0.2, new AgentConnector(target)), end);
                            replay.Decorations.Add(new CircleDecoration(1700, (end, end + 4000), Colors.Red, 0.4, new AgentConnector(target)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TargetID.TheDragonVoidKralkatorrik:
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleKralkatorrikBeamIndicator, out IReadOnlyList<EffectEvent> kralkBeamEffects))
                    {
                        foreach (EffectEvent effect in kralkBeamEffects)
                        {
                            int indicatorDuration = 2000;
                            int aoeDuration = 5000;
                            int indicatorStart = (int)effect.Time;
                            int aoeStart = indicatorStart + indicatorDuration;
                            int aoeEnd = Math.Min(aoeStart + aoeDuration, (int)target.LastAware);
                            replay.AddDecorationWithGrowing(new RectangleDecoration(700, 2900, (indicatorStart, aoeEnd), Colors.Orange, 0.2, new PositionConnector(effect.Position)), aoeStart);
                        }
                    }
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleKralkatorrikBeamAoe, out IReadOnlyList<EffectEvent> kralkBeamAoeEffects))
                    {
                        foreach (EffectEvent effect in kralkBeamAoeEffects)
                        {
                            int start = (int)effect.Time;
                            int end = Math.Min((int)effect.Time + 5000, (int)target.LastAware);
                            replay.Decorations.Add(new CircleDecoration(350, (start, end), Colors.Black, 0.4, new PositionConnector(effect.Position)));
                        }
                    }
                    // Crystal Barrage
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleKralkatorrikCrystalBarrageImpact, out IReadOnlyList<EffectEvent> crystalBarrage))
                    {
                        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleScalableOrangeAoE, out IReadOnlyList<EffectEvent> aoeIndicator))
                        {
                            foreach (EffectEvent impactEffect in crystalBarrage)
                            {
                                foreach (EffectEvent indicator in aoeIndicator.Where(x => Math.Abs(x.Time - impactEffect.Time) < 5000 && impactEffect.Time > x.Time))
                                {
                                    if (impactEffect.Position.Distance2DToPoint(indicator.Position) < 60)
                                    {
                                        uint radius = 320;
                                        (long start, long end) lifespan = (indicator.Time, indicator.Time + (impactEffect.Time - indicator.Time));
                                        var positionConnector = new PositionConnector(impactEffect.Position);
                                        var warning = new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, positionConnector);
                                        var impect = new CircleDecoration(radius, (lifespan.end, lifespan.end + 250), Colors.White, 0.3, positionConnector);
                                        replay.AddDecorationWithGrowing(warning, lifespan.end);
                                        replay.AddDecorationWithBorder(impect, Colors.DarkPurple, 0.5);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.DragonBodyVoidAmalgamate:
                    break;
                case (int)ArcDPSEnums.TrashID.VoidAmalgamate:
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleInfluenceOfTheVoidPool, out IReadOnlyList<EffectEvent> poolEffects))
                    {
                        if (poolEffects.Any())
                        {
                            // To be safe
                            poolEffects = poolEffects.OrderBy(x => x.Time).ToList();
                            double radius = 100.0;
                            double radiusIncrement = log.FightData.IsCM ? 35.0 : 35.0 / 2;
                            for (int i = 0; i < poolEffects.Count - 1; i++)
                            {
                                EffectEvent curEffect = poolEffects[i];
                                EffectEvent nextEffect = poolEffects[i + 1];
                                int start = (int)curEffect.Time;
                                int end = (int)nextEffect.Time;
                                replay.AddDecorationWithBorder(new CircleDecoration((uint)radius, (start, end), "rgba(59, 0, 16, 0.2)", new PositionConnector(curEffect.Position)), Colors.Red, 0.5);
                                radius += radiusIncrement;
                            }
                            EffectEvent lastEffect = poolEffects.Last();
                            (long start, long end) lifespan = lastEffect.ComputeLifespanWithSecondaryEffectNoSrcCheck(log, EffectGUIDs.HarvestTempleVoidPoolOrbGettingReadyToBeDangerous);
                            (long start, long end) lifespanPuriOrb = lastEffect.ComputeLifespanWithSecondaryEffectNoSrcCheck(log, EffectGUIDs.HarvestTemplePurificationOrbSpawns);
                            lifespan.end = Math.Min(lifespan.end, lifespanPuriOrb.end);
                            // In case log ended before the event happens and we are on pre Effect51 events, we use the expected duration of the effect instead
                            if (lifespan.start == lifespan.end)
                            {
                                lifespan.end = lifespan.start + 4000;
                            }
                            replay.AddDecorationWithBorder(new CircleDecoration((uint)radius, lifespan, "rgba(59, 0, 16, 0.2)", new PositionConnector(lastEffect.Position)), Colors.Red, 0.5);
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TargetID.TheDragonVoidMordremoth:
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleMordremothPoisonRoarImpact, out IReadOnlyList<EffectEvent> mordremothPoisonEffects))
                    {
                        foreach (EffectEvent effect in mordremothPoisonEffects)
                        {
                            int end = (int)effect.Time;
                            int start = end - 2000;
                            replay.AddDecorationWithGrowing(new CircleDecoration( 200, (start, end), "rgba(49, 71, 0, 0.2)", new PositionConnector(effect.Position)), end);
                        }
                    }
                    // Shockwaves
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleMordremothShockwave1, out IReadOnlyList<EffectEvent> shockwaves))
                    {
                        foreach (EffectEvent effect in shockwaves)
                        {
                            (long start, long end) lifespan = (effect.Time, effect.Time + 1600); // Assumed duration, effect has 0
                            var positionConnector = new PositionConnector(effect.Position);
                            var shockwave = (CircleDecoration)new CircleDecoration(2000, lifespan, Colors.Black, 0.6, positionConnector).UsingFilled(false).UsingGrowingEnd(lifespan.end);
                            replay.Decorations.Add(shockwave);
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TargetID.TheDragonVoidZhaitan:
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleZhaitanPutridDelugeImpact, out IReadOnlyList<EffectEvent> zhaitanPoisonImpactEffects))
                    {
                        foreach (EffectEvent effect in zhaitanPoisonImpactEffects)
                        {
                            int end = (int)effect.Time;
                            int start = end - 2000;
                            replay.AddDecorationWithGrowing(new CircleDecoration(200, (start, end), "rgba(49, 71, 0, 0.2)", new PositionConnector(effect.Position)), end);
                        }
                    }
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleZhaitanPutridDelugeAoE, out IReadOnlyList<EffectEvent> zhaitanPoisonAoeEffects))
                    {
                        foreach (EffectEvent effect in zhaitanPoisonAoeEffects)
                        {
                            int start = (int)effect.Time;
                            int end = (int)Math.Min(target.LastAware, start + 10000);
                            replay.AddDecorationWithBorder(new CircleDecoration(200, (start, end), "rgba(49, 71, 0, 0.2)", new PositionConnector(effect.Position)), "rgba(200, 0, 0, 0.5)");
                        }
                    }
                    // Tail Slam
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleZhaitanTailSlamImpact, out IReadOnlyList<EffectEvent> zhaitainTailSlam))
                    {
                        foreach (EffectEvent effect in zhaitainTailSlam)
                        {
                            // We use effect time - 3000 because the AoE effect indicator isn't disambiguous, the impact is
                            (long start, long end) lifespan = (effect.Time - 3000, effect.Time);
                            var circle = new CircleDecoration(620, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                            replay.AddDecorationWithGrowing(circle, lifespan.end);
                        }
                    }
                    // Scream of Zhaitan
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleZhaitanScreamIndicator, out IReadOnlyList<EffectEvent> screamOfZhaitan))
                    {
                        foreach (EffectEvent effect in screamOfZhaitan)
                        {
                            (long start, long end) lifespan = effect.ComputeLifespan(log, 3000);
                            var circle = new CircleDecoration(1720, lifespan, Colors.LightRed, 0.1, new PositionConnector(effect.Position));
                            replay.AddDecorationWithGrowing(circle, lifespan.end);
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TargetID.TheDragonVoidSooWon:
                    // Claw Swipe
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleSooWonClaw, out IReadOnlyList<EffectEvent> sooWonClawEffects))
                    {
                        var rotationConnector = new AngleConnector(99.6f);
                        foreach (EffectEvent effect in sooWonClawEffects)
                        {
                            int start = (int)effect.Time;
                            int end = start + 2300;
                            var connector = new PositionConnector(effect.Position);
                            replay.AddDecorationWithGrowing((PieDecoration)new PieDecoration(1060, 145, (start, end), Colors.Red, 0.4, connector).UsingRotationConnector(rotationConnector), end);
                        }
                    }

                    // Claw Swipe - Bouncing Void Orbs
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleSooWonVoidOrbs1, out IReadOnlyList<EffectEvent> clawVoidOrbs))
                    {
                        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleTormentOfTheVoidClawIndicator, out IReadOnlyList<EffectEvent> clawVoidOrbsAoEs))
                        {
                            // The aoe indicator can be used by other attacks before soo won - filtering out the effects which happen before a claw swipe
                            var filteredBouncingOrbsAoEs = clawVoidOrbsAoEs.Where(x => x.Time > clawVoidOrbs.FirstOrDefault().Time).ToList();
                            List<(EffectEvent, EffectEvent, float)> orbToAoeMatches = MatchEffectToEffect(clawVoidOrbs, filteredBouncingOrbsAoEs);
                            List<(EffectEvent, EffectEvent, float)> aoeToAoeMatches = MatchEffectToEffect(filteredBouncingOrbsAoEs, filteredBouncingOrbsAoEs);

                            // Hard coded the orb positions and the durations for older logs
                            var positions = new List<ParametricPoint3D>()
                                {
                                    new ParametricPoint3D(1527.933f, -20447.47f, -15420.13f, 803),
                                    new ParametricPoint3D(74.92969f, -20728.86f, -15420.13f, 803),
                                    new ParametricPoint3D(-353.9098f, -21363.69f, -15420.13f, 803),
                                    new ParametricPoint3D(1873.578f, -20620.1f, -15420.13f, 803),
                                    new ParametricPoint3D(397.2551f, -20515.84f, -15420.13f, 803),
                                    new ParametricPoint3D(-181.2787f, -21018.05f, -15420.13f, 803),
                                    new ParametricPoint3D(763.7318f, -20393.5f, -15420.13f, 803),
                                    new ParametricPoint3D(1149.385f, -20370.18f, -15420.13f, 803),
                                    new ParametricPoint3D(1184.253f, -19876.46f, -15420.13f, 1133),
                                    new ParametricPoint3D(1689.397f, -19979.6f, -15420.13f, 1133),
                                    new ParametricPoint3D(-591.4208f, -20740.99f, -15420.13f, 1133),
                                    new ParametricPoint3D(-249.5301f, -20355.09f, -15420.13f, 1133),
                                    new ParametricPoint3D(180.5903f, -20070.83f, -15420.13f, 1133),
                                    new ParametricPoint3D(669.6267f, -19907.58f, -15420.13f, 1133),
                                    new ParametricPoint3D(-553.218f, -20005.25f, -15420.13f, 1133),
                                    new ParametricPoint3D(1216.888f, -19414.34f, -15420.13f, 1133),
                                    new ParametricPoint3D(-22.20401f, -19654.31f, -15420.13f, 1133),
                                    new ParametricPoint3D(581.5457f, -19452.76f, -15420.13f, 1133),
                                    new ParametricPoint3D(-224.9983f, -19237.79f, -15420.13f, 1133),
                                    new ParametricPoint3D(493.4647f, -18997.94f, -15420.13f, 1133),
                                };

                            // Orb indicator near the swipe cone
                            foreach (EffectEvent orb in clawVoidOrbs)
                            {
                                (long start, long end) lifespan = orb.ComputeLifespan(log, 2080);
                                var circle = new CircleDecoration(25, lifespan, Colors.Black, 0.5, new PositionConnector(orb.Position));
                                replay.Decorations.Add(circle);
                            }

                            // Orb to AoE effects
                            foreach ((EffectEvent aoe, EffectEvent orb, float distance) in orbToAoeMatches)
                            {
                                (long start, long end) lifespanAoE = (aoe.Time, aoe.Time + aoe.Duration);
                                if (aoe.Duration == 0)
                                {
                                    foreach (ParametricPoint3D point in positions)
                                    {
                                        if (aoe.Position.Distance2DToPoint(point) < 0.5)
                                        {
                                            lifespanAoE.end = lifespanAoE.start + point.Time;
                                            break;
                                        }
                                    }
                                }
                                // Add aoe
                                replay.AddDecorationWithGrowing(new CircleDecoration(200, lifespanAoE, Colors.LightOrange, 0.2, new PositionConnector(aoe.Position)), lifespanAoE.end);
                                // Add projectile
                                replay.AddProjectile(orb.Position, aoe.Position, lifespanAoE, Colors.Black, 0.5);
                            }

                            // AoE to AoE effects
                            foreach ((EffectEvent endingAoE, EffectEvent startingAoE, float distance) in aoeToAoeMatches)
                            {
                                long endingAoEDuration = endingAoE.Duration;
                                long startingAoEDuration = startingAoE.Duration;
                                if (endingAoEDuration == 0 || startingAoEDuration == 0)
                                {
                                    foreach (ParametricPoint3D point in positions)
                                    {
                                        if (endingAoEDuration != 0 && startingAoEDuration != 0)
                                        {
                                            break;
                                        }
                                        if (endingAoEDuration == 0 && endingAoE.Position.Distance2DToPoint(point) < 0.5)
                                        {
                                            endingAoEDuration = point.Time;
                                        }
                                        if (startingAoEDuration == 0 && startingAoE.Position.Distance2DToPoint(point) < 0.5)
                                        {
                                            startingAoEDuration = point.Time;
                                        }
                                    }
                                }
                                // Add aoe
                                (long start, long end) lifespanAoE = (endingAoE.Time, endingAoE.Time + endingAoEDuration);
                                replay.AddDecorationWithGrowing(new CircleDecoration(200, lifespanAoE, Colors.LightOrange, 0.2, new PositionConnector(endingAoE.Position)), lifespanAoE.end);
                                // Add projectile - Starts when the previous AoE ends because it's bouncing
                                (long start, long end) lifespanAnimation = (startingAoE.Time + startingAoEDuration, endingAoE.Time + endingAoEDuration);
                                replay.AddProjectile(startingAoE.Position, endingAoE.Position, lifespanAnimation, Colors.Black, 0.5);
                            }
                        }
                    }
                    // Tail Slam
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleTailSlamIndicator, out IReadOnlyList<EffectEvent> tailSlamEffects))
                    {
                        // Generic Orange AoE - Used in multiple sections of the encounter
                        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleScalableOrangeAoE, out IReadOnlyList<EffectEvent> genericOrangeAoE))
                        {
                            foreach (EffectEvent tailSlamEffect in tailSlamEffects)
                            {
                                // Filtering the effects
                                var filteredVoidOrbsAoEs = genericOrangeAoE.Where(x => Math.Abs(x.Time - tailSlamEffect.Time) < 2000 && x.Time < tailSlamEffect.Time + 10000).ToList();

                                // Tail Slam AoE
                                (long start, long end) lifespanTail = tailSlamEffect.ComputeLifespan(log, 1600);
                                replay.AddDecorationWithGrowing(new RectangleDecoration(3000, 750, lifespanTail, Colors.Red, 0.2, new PositionConnector(tailSlamEffect.Position)), lifespanTail.end);

                                // Void Orbs AoEs
                                foreach (EffectEvent orbAoeEffect in filteredVoidOrbsAoEs)
                                {
                                    (long start, long end) lifespanAoE = orbAoeEffect.ComputeLifespan(log, 1900);
                                    replay.AddDecorationWithGrowing(new CircleDecoration(200, lifespanAoE, Colors.LightOrange, 0.2, new PositionConnector(orbAoeEffect.Position)), lifespanAoE.end);
                                    // Add projectile
                                    replay.AddProjectile(tailSlamEffect.Position, orbAoeEffect.Position, lifespanAoE, Colors.Black, 0.5);
                                }
                            }
                        }
                    }

                    // Tsunami Slam AoE indicator
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleSooWonTsunamiSlamIndicator, out IReadOnlyList<EffectEvent> tsunamiSlamIndicators))
                    {
                        // Generic Orange AoE - Used in multiple sections of the encounter
                        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleScalableOrangeAoE, out IReadOnlyList<EffectEvent> genericOrangeAoE))
                        {
                            foreach (EffectEvent tsunamiSlamEffect in tsunamiSlamIndicators)
                            {
                                // Filtering the effects
                                var filteredVoidOrbsAoEs = genericOrangeAoE.Where(x => Math.Abs(x.Time - tsunamiSlamEffect.Time) < 2000 && x.Time < tsunamiSlamEffect.Time + 10000).ToList();

                                (long start, long end) lifespanClawAoE = tsunamiSlamEffect.ComputeLifespan(log, 1600);
                                replay.AddDecorationWithGrowing(new CircleDecoration(235, lifespanClawAoE, Colors.Red, 0.2, new PositionConnector(tsunamiSlamEffect.Position)), lifespanClawAoE.end);

                                // Void Orbs AoEs
                                foreach (EffectEvent orbAoeEffect in filteredVoidOrbsAoEs)
                                {
                                    (long start, long end) lifespanAoE = orbAoeEffect.ComputeLifespan(log, 1900);
                                    replay.AddDecorationWithGrowing(new CircleDecoration(200, lifespanAoE, Colors.LightOrange, 0.2, new PositionConnector(orbAoeEffect.Position)), lifespanAoE.end);
                                    // Add projectile
                                    replay.AddProjectile(tsunamiSlamEffect.Position, orbAoeEffect.Position, lifespanAoE, Colors.Black, 0.5);
                                }
                            }
                        }
                    }

                    // Tsunami
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleTsunami1, out IReadOnlyList<EffectEvent> tsunamiEffects))
                    {
                        foreach (EffectEvent effect in tsunamiEffects)
                        {
                            // Expanding wave - radius and duration are estimates, can't seem to line up the decoration with actual hits
                            int waveStart = (int)effect.Time;
                            int waveEnd = waveStart + 4500;
                            replay.Decorations.Add(new CircleDecoration(2000, (waveStart, waveEnd), Colors.Blue, 0.5, new PositionConnector(effect.Position)).UsingFilled(false).UsingGrowingEnd(waveEnd));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.VoidWarforged1:
                case (int)ArcDPSEnums.TrashID.VoidWarforged2:
                    //CombatReplay.DebugEffects(target, log, replay, knownEffectsIDs, target.FirstAware, target.LastAware, true);
                    break;
                case (int)ArcDPSEnums.TrashID.ZhaitansReach:
                    // Thrash - Circle that pulls in
                    var thrash = casts.Where(x => x.SkillId == ZhaitansReachThrashHT1 || x.SkillId == ZhaitansReachThrashHT2).ToList();
                    foreach (AbstractCastEvent c in thrash)
                    {
                        int castTime = 1900;
                        int endTime = (int)c.Time + castTime;
                        replay.AddDecorationWithGrowing(new DoughnutDecoration(240, 480, (c.Time, endTime), Colors.Orange, 0.2, new AgentConnector(target)), endTime);
                    }
                    // Ground Slam - AoE that knocks out
                    var groundSlam = casts.Where(x => x.SkillId == ZhaitansReachGroundSlam || x.SkillId == ZhaitansReachGroundSlamHT).ToList();
                    foreach (AbstractCastEvent c in groundSlam)
                    {
                        int castTime = c.SkillId == ZhaitansReachGroundSlam ? 800 : 2500;
                        uint radius = 400;
                        long endTime = c.Time + castTime;
                        replay.AddDecorationWithGrowing(new CircleDecoration(radius, (c.Time, endTime), Colors.Orange, 0.2, new AgentConnector(target)), endTime);
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.VoidBrandbomber:
                    // Branded Artillery
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidBrandbomberBrandedArtillery, out IReadOnlyList<EffectEvent> brandedArtilleryAoEs))
                    {
                        var brandedArtillery = casts.Where(x => x.SkillId == BrandedArtillery).ToList();
                        foreach (AbstractCastEvent c in brandedArtillery)
                        {
                            int castDuration = 2500;
                            EffectEvent brandedArtilleryAoE = brandedArtilleryAoEs.FirstOrDefault(x => x.Time > c.Time && x.Time < c.Time + castDuration + 100);
                            Point3D brandbomberPosition = target.GetCurrentPosition(log, c.Time, 1000);
                            if (brandedArtilleryAoE != null && brandbomberPosition != null)
                            {
                                // Shooting animation
                                long animationDuration = brandedArtilleryAoE.Time - c.Time;
                                (long start, long end) lifespan = (c.Time, c.Time + (animationDuration));

                                // Landing indicator
                                uint radius = 240;
                                var positionConnector = new PositionConnector(brandedArtilleryAoE.Position);
                                var aoeCircle = new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, positionConnector);
                                replay.AddDecorationWithGrowing(aoeCircle, lifespan.end);
                                // Projective decoration
                                replay.AddProjectile(brandbomberPosition, brandedArtilleryAoE.Position, lifespan, Colors.DarkPurple);
                            }
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.VoidTimeCaster:
                    // Gravity Crush - Indicator
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidTimecasterGravityCrushIndicator, out IReadOnlyList<EffectEvent> gravityCrushIndicators))
                    {
                        foreach (EffectEvent effect in gravityCrushIndicators)
                        {
                            uint minRadius = 0;
                            uint radiusIncrease = 40;
                            float initialOpacity = 0.5f;
                            (long start, long end) lifespan = effect.ComputeLifespan(log, 1600);
                            var positionConnector = new PositionConnector(effect.Position);
                            // The indicator has 8 circles
                            for (int i = 1; i <= 8; i++)
                            {
                                uint maxRadius = minRadius + radiusIncrease;
                                float opacity = initialOpacity / i;
                                var circle = new CircleDecoration(maxRadius, minRadius, lifespan, Colors.Orange, opacity, positionConnector);
                                replay.AddDecorationWithBorder(circle, Colors.Orange, 0.2);
                                minRadius = maxRadius;
                            }
                        }
                    }
                    // Nightmare Epoch - AoEs
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidTimecasterNightmareEpoch, out IReadOnlyList<EffectEvent> nightmareEpoch))
                    {
                        foreach(EffectEvent effect in nightmareEpoch)
                        {
                            (long start, long end) lifespan = effect.ComputeLifespan(log, 10000);
                            var positionConnector = new PositionConnector(effect.Position);
                            var circle = new CircleDecoration(100, lifespan, Colors.Black, 0.2, positionConnector);
                            replay.AddDecorationWithBorder(circle, Colors.LightRed, 0.4);
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.GravityBall:
                    // Setting the first aware to + 1600 due to the duration of the warning effect
                    (long start, long end) lifespanBall = (target.FirstAware + 1600, target.LastAware);
                    var perimeter = (CircleDecoration)new CircleDecoration(320, 300, lifespanBall, Colors.Red, 0.2, new AgentConnector(target)).UsingFilled(false);
                    replay.Decorations.Add(perimeter);
                    break;
                case (int)ArcDPSEnums.TrashID.VoidGiant:
                    // Death Scream - Fear
                    var deathScreams = casts.Where(x => x.SkillId == DeathScream).ToList();
                    foreach (AbstractCastEvent c in deathScreams)
                    {
                        uint radius = 500;
                        long castDuration = 1680;
                        long supposedEndCast = c.Time + castDuration;
                        long actualEndCast = ComputeEndCastTimeByStun(log, target, c.Time, castDuration);
                        (long start, long end) lifespan = (c.Time, actualEndCast);
                        var agentConnector = new AgentConnector(c.Caster);
                        var circle = new CircleDecoration(radius, lifespan, Colors.Orange, 0.2, agentConnector);
                        replay.AddDecorationWithGrowing(circle, supposedEndCast);
                    }

                    // Rotting Bile - Poison AoE Indicator
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidGiantRottingBileIndicator, out IReadOnlyList<EffectEvent> bileIndicators))
                    {
                        foreach (EffectEvent effect in bileIndicators)
                        {
                            uint radius = 250;
                            (long start, long end) lifespan = effect.ComputeLifespan(log, 1400);
                            var positionConnector = new PositionConnector(effect.Position);
                            var circle = new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, positionConnector);
                            replay.AddDecorationWithGrowing(circle, lifespan.end);
                        }
                    }

                    // Rotting Bile - Poison AoE Damage
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidGiantRottingBileDamage, out IReadOnlyList<EffectEvent> bileAoEs))
                    {
                        foreach (EffectEvent effect in bileAoEs)
                        {
                            uint radius = 250;
                            (long start, long end) lifespan = effect.ComputeLifespan(log, 10000);
                            var positionConnector = new PositionConnector(effect.Position);
                            var circle = new CircleDecoration(radius, lifespan, Colors.DarkGreen, 0.2, positionConnector);
                            replay.Decorations.Add(circle);
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.VoidSaltsprayDragon:
                    // Call Lightning
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidSaltsprayDragonCallLightning, out IReadOnlyList<EffectEvent> callLightnings))
                    {
                        foreach (EffectEvent effect in callLightnings)
                        {
                            uint radius = 50;
                            (long start, long end) lifespan = (effect.Time - 2000, effect.Time);
                            var positionConnector = new PositionConnector(effect.Position);
                            var circleIndicator = new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, positionConnector);
                            var lightningIndicator = new CircleDecoration(radius, (lifespan.end, lifespan.end + 250), Colors.LightPurple, 0.2, positionConnector);
                            replay.AddDecorationWithGrowing(circleIndicator, lifespan.start);
                            replay.Decorations.Add(lightningIndicator);
                        }
                    }

                    // Hydro Burst - Whirlpools
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidSaltsprayDragonHydroBurstWhirlpools, out IReadOnlyList<EffectEvent> hydroBurstWhirlpools))
                    {
                        uint radius = 90;
                        int counter = 1;
                        foreach (EffectEvent effect in hydroBurstWhirlpools)
                        {
                            (long start, long end) lifespan = effect.ComputeLifespan(log, 3000);
                            var positionConnector = new PositionConnector(effect.Position);
                            var circleIndicator = new CircleDecoration(radius, (lifespan.start - 2000, lifespan.start), Colors.LightOrange, 0.2, positionConnector);
                            var circleWhirlpool = new CircleDecoration(radius, lifespan, Colors.LightBlue, 0.2, positionConnector);
                            replay.AddDecorationWithGrowing(circleIndicator, lifespan.start);
                            replay.Decorations.Add(circleWhirlpool);
                            // The whirlpools increase in size every set of 3, find if there is a next effect within 500ms.
                            EffectEvent nextWhirlpool = hydroBurstWhirlpools.FirstOrDefault(x => Math.Abs(x.Time - effect.Time) < 500 && x.Time > effect.Time);
                            radius = counter % 3 == 0 ? radius + 10 : radius;
                            // if there isn't a next one, reset the radius to the starting value
                            if (nextWhirlpool == null)
                            {
                                radius = 90;
                            }
                            counter++;
                        }
                    }

                    // Frozen Fury - Cone Indicator
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidSaltsprayDragonFrozenFuryCone, out IReadOnlyList<EffectEvent> frozenFuryCone))
                    {
                        foreach (EffectEvent effect in frozenFuryCone)
                        {
                            uint radius = 690;
                            int angle = 60;
                            (long start, long end) lifespan = effect.ComputeLifespan(log, 1350);
                            var positionConnector = new PositionConnector(effect.Position);
                            var rotationConnector = new AngleConnector(effect.Rotation.Z + 90);
                            var cone = (PieDecoration)new PieDecoration(radius, angle, lifespan, Colors.LightOrange, 0.2, positionConnector).UsingRotationConnector(rotationConnector);
                            replay.AddDecorationWithGrowing(cone, lifespan.end);
                        }
                    }

                    // Frozen Fury - Rectangle Indicator
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidSaltsprayDragonFrozenFuryRectangle, out IReadOnlyList<EffectEvent> frozenFuryRectangles))
                    {
                        foreach (EffectEvent effect in frozenFuryRectangles)
                        {
                            uint width = 200;
                            uint height = 800;
                            (long start, long end) lifespan = effect.ComputeLifespan(log, 1600);
                            var positionConnector = (PositionConnector)new PositionConnector(effect.Position).WithOffset(new Point3D(0, -30), true);
                            var rotationConnector = new AngleConnector(effect.Rotation.Z + 90);
                            var rectangle = (RectangleDecoration)new RectangleDecoration(width, height, lifespan, Colors.LightOrange, 0.2, positionConnector).UsingRotationConnector(rotationConnector);
                            replay.AddDecorationWithGrowing(rectangle, lifespan.end);
                        }
                    }

                    // Rolling Flames
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidSaltsprayDragonRollingFlames, out IReadOnlyList<EffectEvent> rollingFlames))
                    {
                        foreach (EffectEvent effect in rollingFlames)
                        {
                            (long start, long end) lifespan = effect.ComputeLifespan(log, 1700);
                            var positionConnector = new PositionConnector(effect.Position);
                            var circle = new CircleDecoration(300, lifespan, Colors.LightOrange, 0.2, positionConnector);
                            replay.AddDecorationWithGrowing(circle, lifespan.end);
                        }
                    }

                    // Shatter Earth
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidSaltsprayDragonShatterEarth, out IReadOnlyList<EffectEvent> shatterEarth))
                    {
                        int counter = 0;
                        foreach (EffectEvent effect in shatterEarth)
                        {
                            (long start, long end) lifespan = effect.ComputeLifespan(log, 1550);
                            var positionConnector = new PositionConnector(effect.Position);
                            uint innerRadius = 0;
                            uint outerRadius = 0;
                            switch (counter)
                            {
                                case 0:
                                    innerRadius = 180;
                                    outerRadius = 300;
                                    var circle = new CircleDecoration(140, lifespan, Colors.LightOrange, 0.2, positionConnector);
                                    replay.AddDecorationWithGrowing(circle, lifespan.end);
                                    break;
                                case 1:
                                case 2:
                                case 3:
                                    innerRadius = 120;
                                    outerRadius = 180;
                                    break;
                                case 4:
                                case 5:
                                case 6:
                                    innerRadius = 100;
                                    outerRadius = 150;
                                    break;
                                default:
                                    break;
                            }
                            var doughnut = new DoughnutDecoration(innerRadius, outerRadius, lifespan, Colors.LightOrange, 0.2, positionConnector);
                            replay.AddDecorationWithGrowing(doughnut, lifespan.end);
                            counter = (counter + 1) % 7;
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.VoidAbomination:
                    // Abomination Swipe - Launch
                    var abominationSwipes = casts.Where(x => x.SkillId == AbominationSwipe).ToList();
                    foreach (AbstractCastEvent c in abominationSwipes)
                    {
                        uint radius = 300;
                        int angle = 40;
                        long castDuration = 2368;
                        long supposedEndCast = c.Time + castDuration;
                        long actualEndCast = c.Status == AbstractCastEvent.AnimationStatus.Interrupted && c.EndTime < c.Time + castDuration ? c.EndTime : supposedEndCast;
                        (long start, long end) lifespan = (c.Time, actualEndCast);
                        var agentConnector = new AgentConnector(c.Caster);
                        var cone = new PieDecoration(radius, angle, lifespan, Colors.Orange, 0.2, agentConnector);
                        replay.AddDecorationWithGrowing(cone, supposedEndCast);
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.VoidObliterator:
                    // Charge - Indicator
                    var charges = casts.Where(x => x.SkillId == VoidObliteratorChargeWindup).ToList();
                    foreach (AbstractCastEvent c in charges)
                    {
                        uint length = 2000;
                        uint width = target.HitboxWidth;
                        long castDuration = 1000;
                        long supposedEndCast = c.Time + castDuration;
                        long actualEndCast = ComputeEndCastTimeByStun(log, target, c.Time, castDuration);
                        Point3D facing = target.GetCurrentRotation(log, c.Time + castDuration);
                        if (facing != null)
                        {
                            (long start, long end) lifespan = (c.Time, actualEndCast);
                            var agentConnector = (AgentConnector)new AgentConnector(target).WithOffset(new Point3D(length / 2, 0), true);
                            var rotation = new AngleConnector(facing);
                            var rectangle = (RectangleDecoration)new RectangleDecoration(length, width, lifespan, Colors.Orange, 0.2, agentConnector).UsingRotationConnector(rotation);
                            replay.AddDecorationWithGrowing(rectangle, supposedEndCast);
                        }
                    }

                    // Wyvern Breath - Indicator
                    var wyvernBreaths = casts.Where(x => x.SkillId == VoidObliteratorWyvernBreathSkill).ToList();
                    foreach (AbstractCastEvent c in wyvernBreaths)
                    {
                        uint radius = 750;
                        int openingAngle = 60;
                        long castDuration = 3400;
                        long supposedEndCast = c.Time + castDuration;
                        long actualEndCast = ComputeEndCastTimeByStun(log, target, c.Time, castDuration);
                        Point3D facing = target.GetCurrentRotation(log, c.Time + castDuration);
                        if (facing != null)
                        {
                            (long start, long end) lifespan = (c.Time, actualEndCast);
                            var agentConnector = new AgentConnector(target);
                            var rotation = new AngleConnector(facing);
                            var warningCone = (PieDecoration)new PieDecoration(radius, openingAngle, lifespan, Colors.Orange, 0.2, agentConnector).UsingRotationConnector(rotation);
                            replay.AddDecorationWithGrowing(warningCone, supposedEndCast);
                            // Manually adding a fire decoration for old logs
                            if (!log.CombatData.HasEffectData)
                            {
                                int fireDuration = 30000;
                                (long start, long end) lifespanFire = (lifespan.end, lifespan.end + fireDuration);
                                var fireCone = (PieDecoration)new PieDecoration(radius, openingAngle, lifespanFire, Colors.Yellow, 0.2, agentConnector).UsingRotationConnector(rotation);
                                replay.Decorations.Add(fireCone);
                            }
                        }
                    }

                    // Wyvern Breath - Small fire AoEs
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidObliteratorWyvernBreathFire, out IReadOnlyList<EffectEvent> wyvernBreahFires))
                    {
                        foreach (EffectEvent effect in wyvernBreahFires)
                        {
                            uint radius = 80;
                            (long start, long end) lifespan = effect.ComputeLifespan(log, 30000);
                            var positionConnector = new PositionConnector(effect.Position);
                            var circle = new CircleDecoration(radius, lifespan, Colors.Yellow, 0.2, positionConnector);
                            replay.Decorations.Add(circle);
                        }
                    }

                    // Claw Shockwave
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidObliteratorShockwave, out IReadOnlyList<EffectEvent> clawShockwave))
                    {
                        foreach (EffectEvent effect in clawShockwave)
                        {
                            uint radius = 1000; // Assumed radius
                            (long start, long end) lifespan = effect.ComputeLifespan(log, 2500); // Assumed duration
                            var positionConnector = new PositionConnector(effect.Position);
                            var shockwave = (CircleDecoration)new CircleDecoration(radius, lifespan, Colors.LightGrey, 0.5, positionConnector).UsingFilled(false).UsingGrowingEnd(lifespan.end);
                            replay.Decorations.Add(shockwave);
                        }
                    }

                    // Firebomb
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidObliteratorFirebomb, out IReadOnlyList<EffectEvent> firebombAoEs))
                    {
                        var firebombs = casts.Where(x => x.SkillId == VoidObliteratorFirebomb).ToList();
                        foreach (AbstractCastEvent c in firebombs)
                        {
                            long castDuration = 1500;
                            EffectEvent bombAoE = firebombAoEs.FirstOrDefault(x => x.Time > c.Time && x.Time < c.Time + castDuration);
                            Point3D obliteratorPosition = target.GetCurrentPosition(log, c.Time);
                            if (bombAoE != null && obliteratorPosition != null)
                            {
                                // Shooting animation
                                long animationDuration = bombAoE.Time - c.Time;
                                (long start, long end) lifespan = (c.Time, c.Time + animationDuration);
                                replay.AddProjectile(obliteratorPosition, bombAoE.Position, lifespan, Colors.Red);

                                // Landed Firebomb
                                uint radius = 120;
                                (long start, long end) lifespanAoE = bombAoE.ComputeLifespan(log, 21000);
                                var positionConnector = new PositionConnector(bombAoE.Position);
                                var fireCircle = new CircleDecoration(radius, lifespanAoE, Colors.Yellow, 0.2, positionConnector);
                                replay.Decorations.Add(fireCircle);
                            }
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.VoidGoliath:
                    // Glacial Slam - Cast Indicator
                    var glacialSlams = casts.Where(x => x.SkillId == GlacialSlam).ToList();
                    foreach (AbstractCastEvent c in glacialSlams)
                    {
                        uint radius = 600;
                        long castDuration = 1880;
                        long supposedEndCast = c.Time + castDuration;
                        long actualEndCast = ComputeEndCastTimeByStun(log, target, c.Time, castDuration);
                        (long start, long end) lifespan = (c.Time, actualEndCast);
                        var agentConnector = new AgentConnector(c.Caster);
                        var circle = new CircleDecoration(radius, lifespan, Colors.Orange, 0.2, agentConnector);
                        replay.AddDecorationWithGrowing(circle, supposedEndCast);
                    }

                    // Glacial Slam - AoE
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidGoliathGlacialSlam, out IReadOnlyList<EffectEvent> glacialSlamsAoE))
                    {
                        foreach (EffectEvent effect in glacialSlamsAoE)
                        {
                            uint radius = 600;
                            int duration = 5000;
                            (long start, long end) lifespan = effect.ComputeLifespan(log, duration);
                            var positionConnector = new PositionConnector(effect.Position);
                            var circle = new CircleDecoration(radius, lifespan, Colors.Ice, 0.4, positionConnector);
                            replay.AddDecorationWithBorder(circle, Colors.Red, 0.5);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private AbstractSingleActor FindActiveOrNextDragonVoid(long time)
        {
            var dragonVoidIDs = new List<int> {
                (int)ArcDPSEnums.TargetID.TheDragonVoidJormag,
                (int)ArcDPSEnums.TargetID.TheDragonVoidPrimordus,
                (int)ArcDPSEnums.TargetID.TheDragonVoidKralkatorrik,
                (int)ArcDPSEnums.TargetID.TheDragonVoidMordremoth,
                (int)ArcDPSEnums.TargetID.TheDragonVoidZhaitan,
                (int)ArcDPSEnums.TargetID.TheDragonVoidSooWon,
            };
            AbstractSingleActor activeDragon = FirstAwareSortedTargets.FirstOrDefault(x => x.FirstAware <= time && x.LastAware >= time && dragonVoidIDs.Contains(x.ID));
            return activeDragon ?? FirstAwareSortedTargets.FirstOrDefault(x => x.FirstAware >= time);
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
            var knownEffectsIDs = new HashSet<long>();
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleSpreadCM, out IReadOnlyList<EffectEvent> spreadEffectsCM))
            {
                AddSpreadSelectionDecoration(p, log, replay, spreadEffectsCM, 300, 5500);
            }
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleSpreadNM, out IReadOnlyList<EffectEvent> spreadEffectsNM))
            {
                AddSpreadSelectionDecoration(p, log, replay, spreadEffectsNM, 240, 5000);
            }
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleRedPuddleSelectCM, out IReadOnlyList<EffectEvent> redSelectedEffectsCM))
            {
                AddVoidPoolSelectionDecoration(p, replay, redSelectedEffectsCM, 400);
            }
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleRedPuddleSelectNM, out IReadOnlyList<EffectEvent> redSelectedEffectsNM))
            {
                AddVoidPoolSelectionDecoration(p, replay, redSelectedEffectsNM, 300);
            }
        }

        /// <summary>
        /// Void Pools - Red Pool AoE on selected players.
        /// </summary>
        /// <param name="p">Selected player.</param>
        /// <param name="replay">Combat Replay.</param>
        /// <param name="redSelectedEffects">Effects List.</param>
        /// <param name="radius">Radius of the AoE.</param>
        private void AddVoidPoolSelectionDecoration(AbstractPlayer p, CombatReplay replay, IReadOnlyList<EffectEvent> redSelectedEffects, uint radius)
        {
            var redSelectedEffectsOnPlayer = redSelectedEffects.Where(x => x.Dst == p.AgentItem).ToList();
            foreach (EffectEvent redSelectedEffect in redSelectedEffectsOnPlayer)
            {
                int duration = 7000;
                int start = (int)redSelectedEffect.Time;
                AbstractSingleActor dragonVoid = FindActiveOrNextDragonVoid(redSelectedEffect.Time);
                if (dragonVoid == null)
                {
                    continue;
                }
                int end = Math.Min((int)dragonVoid.LastAware, start + duration);
                replay.AddDecorationWithGrowing(new CircleDecoration(radius, (start, end), Colors.Red, 0.2, new AgentConnector(p)), end);
            }
        }

        /// <summary>
        /// Targeted Expulsion - Spread AoE on players.
        /// </summary>
        /// <param name="p">Selected player.</param>
        /// <param name="log">The log.</param>
        /// <param name="replay">Combat Replay.</param>
        /// <param name="spreadEffects">Effects List.</param>
        /// <param name="radius">Radius of the AoE.</param>
        /// <param name="duration">Duration of the AoE.</param>
        private void AddSpreadSelectionDecoration(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay, IReadOnlyList<EffectEvent> spreadEffects, uint radius, int duration)
        {
            var spreadEffectsOnPlayer = spreadEffects.Where(x => x.Dst == p.AgentItem).ToList();
            foreach (EffectEvent spreadEffect in spreadEffectsOnPlayer)
            {
                int start = (int)spreadEffect.Time;
                int end = start + duration;
                AbstractSingleActor dragonVoid = FindActiveOrNextDragonVoid(spreadEffect.Time);
                if (dragonVoid == null)
                {
                    continue;
                }
                int effectEnd = Math.Min((int)dragonVoid.LastAware, end);
                DeadEvent deadEvent = log.CombatData.GetDeadEvents(p.AgentItem).FirstOrDefault(x => x.Time >= start);
                if (deadEvent != null && deadEvent.Time <= effectEnd)
                {
                    effectEnd = Math.Min((int)deadEvent.Time, end);
                }
                DespawnEvent despawnEvent = log.CombatData.GetDespawnEvents(p.AgentItem).FirstOrDefault(x => x.Time >= start);
                if (despawnEvent != null && despawnEvent.Time <= effectEnd)
                {
                    effectEnd = Math.Min((int)despawnEvent.Time, end);
                }
                replay.AddDecorationWithGrowing(new CircleDecoration(radius, (start, effectEnd), Colors.LightOrange, 0.2, new AgentConnector(p)), end);
            }
        }

        /// <summary>
        /// Void Pools - Red Pool AoEs placed.
        /// </summary>
        /// <param name="redPuddleEffects">Effects List.</param>
        /// <param name="radius">Radius of the AoE.</param>
        /// <param name="duration">Duration of the AoE.</param>
        private void AddPlacedVoidPoolDecoration(IReadOnlyList<EffectEvent> redPuddleEffects, uint radius, int duration)
        {
            foreach (EffectEvent effect in redPuddleEffects)
            {
                int inactiveDuration = 1500;
                int start = (int)effect.Time;
                AbstractSingleActor dragonVoid = FindActiveOrNextDragonVoid(effect.Time);
                if (dragonVoid == null)
                {
                    continue;
                }
                int puddleEnd = Math.Min((int)dragonVoid.LastAware, start + duration);
                EnvironmentDecorations.Add(new CircleDecoration( radius, (start, puddleEnd), "rgba(250, 0, 0, 0.3)", new PositionConnector(effect.Position)).UsingGrowingEnd(start + inactiveDuration));
                EnvironmentDecorations.Add(new CircleDecoration( radius, (start, puddleEnd), "rgba(250, 0, 0, 0.3)", new PositionConnector(effect.Position)));
            }
        }

        /// <summary>
        /// Share the Void - Greens in CM.
        /// </summary>
        /// <param name="greenEffects">Effects List.</param>
        /// <param name="isSuccessful">Wether the mechanic was successful or not.</param>
        private void AddShareTheVoidDecoration(IReadOnlyList<EffectEvent> greenEffects, bool isSuccessful)
        {
            foreach (EffectEvent green in greenEffects)
            {
                int duration = 5000;
                int start = (int)green.Time - duration;
                int end = (int)green.Time;
                Color color = isSuccessful ? Colors.DarkGreen : Colors.DarkRed;
                EnvironmentDecorations.Add(new CircleDecoration( 180, (start, end), Colors.DarkGreen, 0.4, new PositionConnector(green.Position)).UsingGrowingEnd(end));
                EnvironmentDecorations.Add(new CircleDecoration( 180, (start, end), color, 0.4, new PositionConnector(green.Position)));
            }
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            var targetIDs = new HashSet<int>()
                {
                    (int)ArcDPSEnums.TargetID.TheDragonVoidJormag,
                    (int)ArcDPSEnums.TargetID.TheDragonVoidKralkatorrik,
                    (int)ArcDPSEnums.TargetID.TheDragonVoidMordremoth,
                    (int)ArcDPSEnums.TargetID.TheDragonVoidPrimordus,
                    (int)ArcDPSEnums.TargetID.TheDragonVoidZhaitan,
                };
            if (Targets.Where(x => targetIDs.Contains(x.ID)).Any(x => x.GetHealth(combatData) > 16000000))
            {
                return FightData.EncounterMode.CM;
            }
            IReadOnlyList<AgentItem> voidMelters = agentData.GetNPCsByID(ArcDPSEnums.TrashID.VoidMelter);
            if (voidMelters.Count > 5)
            {
                long firstAware = voidMelters[0].FirstAware;
                if (voidMelters.Count(x => Math.Abs(x.FirstAware - firstAware) < ServerDelayConstant) > 5)
                {
                    return FightData.EncounterMode.CM;
                }
            }
            // fallback for late logs
            if (combatData.GetEffectGUIDEvent(EffectGUIDs.HarvestTempleGreen) != null || agentData.GetNPCsByID(ArcDPSEnums.TrashID.VoidGoliath).Any() || combatData.GetBuffData(VoidEmpowerment).Any())
            {
                return FightData.EncounterMode.CM;
            }
            return FightData.EncounterMode.Normal;
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);

            // Added a CM mode check because the eligibility had been bugged for some time and showed up in normal mode.
            if (log.FightData.Success && log.FightData.IsCM)
            {
                if (log.CombatData.GetBuffData(AchievementEligibilityVoidwalker).Any())
                {
                    InstanceBuffs.AddRange(GetOnPlayerCustomInstanceBuff(log, AchievementEligibilityVoidwalker));
                }
                else if (CustomCheckVoidwalkerEligibility(log)) // In case all 10 players already have voidwalker
                {
                    InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityVoidwalker], 1));
                }
            }
        }

        private static bool CustomCheckVoidwalkerEligibility(ParsedEvtcLog log)
        {
            IReadOnlyList<AgentItem> orbs = log.AgentData.GetNPCsByID((int)ArcDPSEnums.TrashID.PushableVoidAmalgamate);

            foreach (AgentItem orb in orbs)
            {
                IReadOnlyDictionary<long, BuffsGraphModel> bgms = log.FindActor(orb).GetBuffGraphs(log);
                if (bgms != null && bgms.TryGetValue(VoidEmpowerment, out BuffsGraphModel bgm))
                {
                    if (bgm.BuffChart.Any(x => x.Value >= 3))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
