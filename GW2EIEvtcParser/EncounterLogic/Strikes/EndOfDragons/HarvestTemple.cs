﻿using System;
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
        public HarvestTemple(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                // General
                new PlayerDstEffectMechanic(new [] { EffectGUIDs.HarvestTempleSpreadNM, EffectGUIDs.HarvestTempleSpreadCM }, "Spread Bait", new MechanicPlotlySetting(Symbols.Circle, Colors.Yellow), "Spread.B", "Baited spread mechanic", "Spread Bait", 150),
                new PlayerDstEffectMechanic(new [] { EffectGUIDs.HarvestTempleRedPuddleSelectNM, EffectGUIDs.HarvestTempleRedPuddleCM}, "Red Bait", new MechanicPlotlySetting(Symbols.Circle, Colors.Red), "Red.B", "Baited red puddle mechanic", "Red Bait", 150),
                new PlayerDstBuffApplyMechanic(InfluenceOfTheVoidEffect, "Influence of the Void", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkPurple), "Void.D", "Received Void debuff", "Void Debuff", 150),
                new PlayerDstHitMechanic(InfluenceOfTheVoidSkill, "Influence of the Void Hit", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.DarkPurple), "Void.H", "Hit by Void", "Void Hit", 150),
                new PlayerDstHitMechanic(new [] { VoidPoolNM, VoidPoolCM }, "Void Pool", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkPurple), "Red.H", "Hit by Red Void Pool", "Void Pool", 150),
                new PlayerDstSkillMechanic(new [] { HarvestTempleTargetedExpulsionNM, HarvestTempleTargetedExpulsionCM }, "Targeted Expulsion", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Orange), "Spread.H", "Hit by Spread mechanic", "Targeted Expulsion (Spread)", 150).UsingChecker((@event, log) => @event.HasHit || @event.DoubleProcHit),
                new PlayerSrcAllHitsMechanic("Orb Push", new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightOrange), "Orb Push", "Orb was pushed by player", "Orb Push", 0).UsingChecker((de, log) => (de.To.IsSpecies(ArcDPSEnums.TrashID.PushableVoidAmalgamate) || de.To.IsSpecies(ArcDPSEnums.TrashID.KillableVoidAmalgamate)) && de is DirectHealthDamageEvent),
                // Jormag
                new PlayerDstHitMechanic(new [] { BreathOfJormag1, BreathOfJormag2, BreathOfJormag3 }, "Breath of Jormag", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Blue), "J.Breath.H", "Hit by Jormag Breath", "Jormag Breath", 150),
                new PlayerDstHitMechanic(FrostMeteor, "Frost Meteor", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Blue), "J.Meteor.H", "Hit by Jormag Meteor", "Jormag Meteor", 150),
                // Primordus
                new PlayerDstHitMechanic(LavaSlam, "Lava Slam", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Red), "Slam.H", "Hit by Primordus Slam", "Primordus Slam", 150),
                new PlayerDstHitMechanic(JawsOfDestruction, "Jaws of Destruction", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Red), "Jaws.H", "Hit by Primordus Jaws", "Primordus Jaws", 150),
                // Kralkatorrik 
                new PlayerDstHitMechanic(CrystalBarrage, "Crystal Barrage", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Purple), "Barrage.H", "Hit by Crystal Barrage", "Barrage", 150),
                new PlayerDstHitMechanic(BrandingBeam, "Branding Beam", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Purple), "Beam.H", "Hit by Kralkatorrik Beam", "Kralkatorrik Beam", 150),
                new PlayerDstHitMechanic(BrandedArtillery, "Branded Artillery", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Purple), "Artillery.H", "Hit by Brandbomber Artillery", "Brandbomber Artillery", 150),
                new PlayerDstHitMechanic(VoidPoolKralkatorrik, "Void Pool", new MechanicPlotlySetting(Symbols.Circle, Colors.Black), "K.Pool.H", "Hit by Kralkatorrik Void Pool", "Kralkatorrik Void Pool", 150),
                // Purification 2
                new PlayerDstHitMechanic(SwarmOfMordremoth_PoolOfUndeath, "Swarm of Mordremoth", new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "Goop.H", "Hit by goop left by heart", "Heart Goop", 150),
                new PlayerDstHitMechanic(SwarmOfMordremoth, "Swarm of Mordremoth", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.Red), "Bees.H", "Hit by bees from heart", "Heart Bees", 150),
                // Mordremoth
                new PlayerDstHitMechanic(Shockwave, "Shock Wave", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Green), "ShckWv.H", "Hit by Mordremoth Shockwave", "Mordremoth Shockwave", 150),
                new PlayerDstHitMechanic(Kick, "Kick", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Green), "Kick.H", "Kicked by Void Skullpiercer", "Skullpiercer Kick", 150),
                new PlayerDstHitMechanic(PoisonRoar, "Poison Roar", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Green), "M.Poison.H", "Hit by Mordremoth Poison", "Mordremoth Poison", 150),
                // Zhaitan
                new PlayerDstHitMechanic(new []{ ScreamOfZhaitanNM, ScreamOfZhaitanCM }, "Scream of Zhaitan", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.DarkGreen), "Scream.H", "Hit by Zhaitan Scream", "Zhaitan Scream", 150),
                new PlayerDstHitMechanic(PutridDeluge, "Putrid Deluge", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.DarkGreen), "Z.Poison.H", "Hit by Zhaitan Poison", "Zhaitan Poison", 150),
                // Saltspray
                new PlayerDstHitMechanic(HydroBurst, "Hydro Burst", new MechanicPlotlySetting(Symbols.Circle, Colors.LightBlue), "Whrlpl.H", "Hit by Whirlpool", "Whirlpool", 150),
                new PlayerDstHitMechanic(SwarmOfMordremoth_CorruptedWaters, "Swarm of Mordremoth", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.LightBlue), "Prjtile.H", "Hit by Heart Projectile", "Heart Projectile", 150),
                // Soo Won
                new PlayerDstHitMechanic(new [] { TsunamiSlam1, TsunamiSlam2 }, "Tsunami Slam", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.LightBlue), "Tsunami.H", "Hit by Soo-Won Tsunami", "Soo-Won Tsunami", 150),
                new PlayerDstHitMechanic(ClawSlap, "Claw Slap", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.LightBlue), "Claw.H", "Hit by Soo-Won Claw", "Soo-Won Claw", 150),
                new PlayerDstHitMechanic(VoidPoolSooWon, "Void Pool", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkPink), "SW.Pool.H", "Hit by Soo-Won Void Pool", "Soo-Won Void Pool", 150),
                new PlayerDstHitMechanic(TailSlam, "Tail Slam", new MechanicPlotlySetting(Symbols.Square, Colors.LightBlue), "Tail.H", "Hit by Soo-Won Tail", "Soo-Won Tail", 150),
                new PlayerDstHitMechanic(TormentOfTheVoid, "Torment of the Void", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkMagenta), "Torment.H", "Hit by Torment of the Void (Bouncing Orbs)", "Torment of the Void", 150),
                new EnemySrcEffectMechanic(EffectGUIDs.HarvestTempleGreen, "Success Green", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkGreen), "S.Green", "Green Successful", "Success Green", 0),
                new EnemySrcEffectMechanic(EffectGUIDs.HarvestTempleFailedGreen, "Failed Green", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "F.Green", "Green Failed", "Failed Green", 0)
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
                            (788, 788),
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
                ArcDPSEnums.TrashID.DragonEnergyOrb
            };
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "The Dragonvoid";
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
                        if (!AtLeastOnePlayerAlive(combatData, fightData, Math.Min(targetOffs[1].Time + 200, fightData.FightEnd), playerAgents))
                        {
                            return;
                        }
                        HealthUpdateEvent lastHPUpdate = combatData.GetHealthUpdateEvents(soowon.AgentItem).LastOrDefault();
                        if (lastHPUpdate != null && lastHPUpdate.HPPercent > 0.01)
                        {
                            return;
                        }
                        fightData.SetSuccess(true, targetOffs[1].Time);
                    }
                }
            }
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            bool needRefreshAgentPool = false;
            //
            var dragonOrbMaxHPs = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate && x.DstAgent == 491550).ToList();
            foreach (CombatItem dragonOrbMaxHP in dragonOrbMaxHPs)
            {
                AgentItem dragonOrb = agentData.GetAgent(dragonOrbMaxHP.SrcAgent, dragonOrbMaxHP.Time);
                if (dragonOrb != ParserHelper._unknownAgent)
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
            int index = 0;
            foreach (CombatItem at in attackTargetEvents)
            {
                AgentItem dragonVoid = agentData.GetAgent(at.DstAgent, at.Time);
                AgentItem atAgent = agentData.GetAgent(at.SrcAgent, at.Time);
                // We take attack events, filter out the first one, present at spawn, that is always a non targetable event
                var targetables = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.Targetable && x.SrcMatchesAgent(atAgent) && x.Time > 2000).ToList();
                // There are only two relevant attack targets, one represents the first five and the last one Soo Won
                if (!targetables.Any())
                {
                    continue;
                }
                var targetOns = targetables.Where(x => x.DstAgent == 1).ToList();
                var targetOffs = targetables.Where(x => x.DstAgent == 0).ToList();
                // Events to be copied
                var posFacingHPEventsToCopy = combatData.Where(x => x.SrcMatchesAgent(dragonVoid) && (x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate)).ToList();
                posFacingHPEventsToCopy.AddRange(combatData.Where(x => x.SrcMatchesAgent(atAgent) && (x.IsStateChange == ArcDPSEnums.StateChange.Position || x.IsStateChange == ArcDPSEnums.StateChange.Rotation)));
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
                    AgentItem extra = agentData.AddCustomNPCAgent(start, end, dragonVoid.Name, dragonVoid.Spec, id, false, dragonVoid.Toughness, dragonVoid.Healing, dragonVoid.Condition, dragonVoid.Concentration, atAgent.HitboxWidth, atAgent.HitboxHeight);
                    ulong lastHPUpdate = ulong.MaxValue;
                    foreach (CombatItem c in combatData)
                    {
                        if (extra.InAwareTimes(c.Time))
                        {
                            if (c.SrcMatchesAgent(dragonVoid, extensions))
                            {
                                // Avoid making the gadget go back to 100% hp on "death"
                                if (c.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate)
                                {
                                    // Regenerating back to full HP, override to 0
                                    if (c.DstAgent > lastHPUpdate && c.DstAgent > 9900)
                                    {
                                        c.OverrideDstAgent(0);
                                    }
                                    // Remember last hp
                                    lastHPUpdate = c.DstAgent;
                                }
                                c.OverrideSrcAgent(extra.Agent);
                            }
                            // Redirect effects from attack target to main body
                            if (c.IsStateChange == ArcDPSEnums.StateChange.Effect && c.SrcMatchesAgent(atAgent, extensions))
                            {
                                c.OverrideSrcAgent(extra.Agent);
                            }
                            if (c.DstMatchesAgent(dragonVoid, extensions))
                            {
                                c.OverrideDstAgent(extra.Agent);
                            }
                        }
                    }
                    var attackTargetCopy = new CombatItem(at);
                    attackTargetCopy.OverrideTime(extra.FirstAware);
                    attackTargetCopy.OverrideDstAgent(extra.Agent);
                    combatData.Add(attackTargetCopy);
                    foreach (CombatItem c in posFacingHPEventsToCopy)
                    {
                        var cExtra = new CombatItem(c);
                        cExtra.OverrideTime(extra.FirstAware);
                        cExtra.OverrideSrcAgent(extra.Agent);
                        combatData.Add(cExtra);
                    }
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
            if (needRefreshAgentPool)
            {
                agentData.Refresh();
            }
            // Add missing agents
            for (int i = index; i < idsToUse.Count; i++)
            {
                agentData.AddCustomNPCAgent(long.MaxValue - idsToUse.Count + i, long.MaxValue, "Dragonvoid", Spec.NPC, idsToUse[i], false);
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
                            TormentOfTheVoid
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
                            SlamZhaitan,
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
        }

        internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleGreen, out IReadOnlyList<EffectEvent> greenEffects))
            {
                if (greenEffects.Count > 0)
                {
                    AddShareTheVoidDecoration(greenEffects, true);
                }
            }
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleFailedGreen, out IReadOnlyList<EffectEvent> failedGreenEffects))
            {
                if (failedGreenEffects.Count > 0)
                {
                    AddShareTheVoidDecoration(failedGreenEffects, false);
                }
            }
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleRedPuddleCM, out IReadOnlyList<EffectEvent> redPuddleEffectsCM))
            {
                if (redPuddleEffectsCM.Count > 0)
                {
                    AddPlacedVoidPoolDecoration(redPuddleEffectsCM, 400, 300000);
                }
            }
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleRedPuddleNM, out IReadOnlyList<EffectEvent> redPuddleEffectsNM))
            {
                if (redPuddleEffectsNM.Count > 0)
                {
                    AddPlacedVoidPoolDecoration(redPuddleEffectsNM, 300, 25000);
                }
            }
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            var knownEffectsIDs = new HashSet<long>();
            IReadOnlyList<AbstractCastEvent> casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

            switch (target.ID)
            {
                case (int)ArcDPSEnums.TrashID.PushableVoidAmalgamate:
                    //
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
                            int end = start + purificationZoneEffect.Duration;
                            int radius = 280;
                            if (voidShellRemovalOffset < voidShellRemovals.Count)
                            {
                                end = (int)voidShellRemovals[voidShellRemovalOffset++].Time;
                            }
                            replay.Decorations.Add(new CircleDecoration(true, 0, radius, (start, end), "rgba(0, 180, 255, 0.3)", new PositionConnector(purificationZoneEffect.Position)));
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
                    //
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationLightnings, out IReadOnlyList<EffectEvent> lightningEffects))
                    {
                        foreach (EffectEvent lightningEffect in lightningEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                        {
                            int duration = 3000;
                            int start = (int)lightningEffect.Time - duration;
                            int end = (int)lightningEffect.Time;
                            replay.Decorations.Add(new CircleDecoration(true, end, 180, (start, end), "rgba(255, 180, 0, 0.2)", new PositionConnector(lightningEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 180, (start, end), "rgba(255, 180, 0, 0.2)", new PositionConnector(lightningEffect.Position)));
                        }
                    }
                    //
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationFireBalls, out IReadOnlyList<EffectEvent> fireBallEffects))
                    {
                        foreach (EffectEvent fireBallEffect in fireBallEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                        {
                            int startLoad = (int)fireBallEffect.Time - 2000;
                            int endLoad = (int)fireBallEffect.Time;
                            replay.Decorations.Add(new CircleDecoration(true, endLoad, 180, (startLoad, endLoad), "rgba(250, 0, 0, 0.2)", new PositionConnector(fireBallEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 180, (startLoad, endLoad), "rgba(250, 0, 0, 0.2)", new PositionConnector(fireBallEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 180, (endLoad, endLoad + 2000), "rgba(250, 0, 0, 0.4)", new PositionConnector(fireBallEffect.Position)));
                        }
                    }
                    //
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationVoidZones, out IReadOnlyList<EffectEvent> voidZoneEffects))
                    {
                        foreach (EffectEvent voidZoneEffect in voidZoneEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                        {
                            int start = (int)voidZoneEffect.Time;
                            int end = start + 5000;
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, 90, 230, RadianToDegreeF(voidZoneEffect.Orientation.Z), (start, end), "rgba(150, 0, 150, 0.2)", new PositionConnector(voidZoneEffect.Position)));
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, end, 90, 230, RadianToDegreeF(voidZoneEffect.Orientation.Z), (start, end), "rgba(250, 0, 250, 0.3)", new PositionConnector(voidZoneEffect.Position)));
                        }
                    }
                    //
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationBeeLaunch, out IReadOnlyList<EffectEvent> beeLaunchEffects))
                    {
                        foreach (EffectEvent beeLaunchEffect in beeLaunchEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                        {
                            int start = (int)beeLaunchEffect.Time;
                            int end = start + 3000;
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, 380, 30, RadianToDegreeF(beeLaunchEffect.Orientation.Z), 190, (start, end), "rgba(250, 50, 0, 0.4)", new PositionConnector(beeLaunchEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, end, 280, (start, end), "rgba(250, 150, 0, 0.2)", new PositionConnector(beeLaunchEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 280, (start, end), "rgba(250, 150, 0, 0.2)", new PositionConnector(beeLaunchEffect.Position)));
                            var initialPosition = new ParametricPoint3D(beeLaunchEffect.Position, end);
                            int velocity = 50;
                            int lifespan = 15000;
                            var finalPosition = new ParametricPoint3D(initialPosition + (velocity * lifespan / 1000.0f) * new Point3D((float)Math.Cos(beeLaunchEffect.Orientation.Z), (float)Math.Sin(beeLaunchEffect.Orientation.Z)), end + lifespan);
                            //replay.Decorations.Add(new CircleDecoration(true, 0, 280, (end, end + lifespan), "rgba(250, 50, 0, 0.4)", new InterpolationConnector(new List<Point3D>() { initialPosition, finalPosition})));
                        }
                    }
                    //
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationPoisonTrail, out IReadOnlyList<EffectEvent> poisonTrailEffects))
                    {
                        foreach (EffectEvent poisonTrailEffect in poisonTrailEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                        {
                            int startLoad = (int)poisonTrailEffect.Time - 1000;
                            int start = (int)poisonTrailEffect.Time;
                            int end = start + 15000;
                            replay.Decorations.Add(new CircleDecoration(true, start, 220, (startLoad, start), "rgba(0, 150, 0, 0.2)", new PositionConnector(poisonTrailEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 220, (start, end), "rgba(0, 150, 0, 0.4)", new PositionConnector(poisonTrailEffect.Position)));
                        }
                    }
                    //
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleOrbExplosion, out IReadOnlyList<EffectEvent> orbEffects))
                    {
                        foreach (EffectEvent orbEffect in orbEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                        {
                            int duration = 3000;
                            int start = (int)orbEffect.Time;
                            int end = start + duration;
                            // Radius is an estimate - orb exploding on edge doesn't quite cover the entirety of the arena
                            int radius = 2700;
                            replay.Decorations.Add(new CircleDecoration(true, end, radius, (start, end), "rgba(250, 250, 250, 0.05)", new PositionConnector(orbEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, radius, (start, end), "rgba(250, 250, 250, 0.05)", new PositionConnector(orbEffect.Position)));
                        }
                    }
                    //
                    BreakbarStateEvent breakbar = log.CombatData.GetBreakbarStateEvents(target.AgentItem).FirstOrDefault(x => x.State == ArcDPSEnums.BreakbarState.Active);
                    if (breakbar != null)
                    {
                        int start = (int)breakbar.Time;
                        int end = (int)target.LastAware;
                        replay.Decorations.Add(new CircleDecoration(true, 0, 120, (start, end), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    //
                    break;
                case (int)ArcDPSEnums.TargetID.TheDragonVoidJormag:
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleJormagIceShards, out IReadOnlyList<EffectEvent> iceShardEffects))
                    {
                        foreach (EffectEvent iceShardEffect in iceShardEffects)
                        {
                            int duration = 2500;
                            int start = (int)iceShardEffect.Time;
                            int end = start + duration;
                            replay.Decorations.Add(new CircleDecoration(true, end, 160, (start, end), "rgba(0, 50, 180, 0.2)", new PositionConnector(iceShardEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 160, (start, end), "rgba(0, 50, 180, 0.2)", new PositionConnector(iceShardEffect.Position)));
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
                            replay.Decorations.Add(new CircleDecoration(true, start, 600, (start - indicatorDuration, start), "rgba(250, 120, 0, 0.2)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 600, (start - indicatorDuration, start), "rgba(250, 120, 0, 0.2)", new PositionConnector(effect.Position)));
                            // ice field
                            replay.Decorations.Add(new CircleDecoration(true, start + spreadDuration, 1200, (start, fieldEnd), "rgba(69, 182, 254, 0.1)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 1200, (start, fieldEnd), "rgba(69, 182, 254, 0.1)", new PositionConnector(effect.Position)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.DragonEnergyOrb:
                    (int dragonOrbStart, int dragonOrbEnd) = ((int)target.FirstAware, (int)target.LastAware);
                    replay.Decorations.Add(new CircleDecoration(false, 0, 160, (dragonOrbStart, dragonOrbEnd), "rgba(200, 50, 0, 0.5)", new AgentConnector(target)));
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
                            replay.Decorations.Add(new CircleDecoration(true, end, 560, (start, end), "rgba(200, 100, 0, 0.2)", new PositionConnector(jawPosition)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 560, (start, end), "rgba(200, 100, 0, 0.2)", new PositionConnector(jawPosition)));
                        }
                    }
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePrimordusBigJaw, out IReadOnlyList<EffectEvent> bigJawEffects))
                    {
                        foreach (EffectEvent bigJawEffect in bigJawEffects)
                        {
                            int start = (int)bigJawEffect.Time;
                            int end = start + 7500;
                            replay.Decorations.Add(new CircleDecoration(true, end, 1700, (start, end), "rgba(200, 100, 0, 0.2)", new AgentConnector(target)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 1700, (start, end), "rgba(200, 100, 0, 0.2)", new AgentConnector(target)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 1700, (end, end + 4000), "rgba(200, 0, 0, 0.4)", new AgentConnector(target)));
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
                            replay.Decorations.Add(new RectangleDecoration(true, aoeStart, 700, 2900, (indicatorStart, aoeEnd), "rgba(255, 127, 0, 0.2)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new RectangleDecoration(true, 0, 700, 2900, (indicatorStart, aoeEnd), "rgba(255, 127, 0, 0.2)", new PositionConnector(effect.Position)));
                        }
                    }
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleKralkatorrikBeamAoe, out IReadOnlyList<EffectEvent> kralkBeamAoeEffects))
                    {
                        foreach (EffectEvent effect in kralkBeamAoeEffects)
                        {
                            int start = (int)effect.Time;
                            int end = Math.Min((int)effect.Time + 5000, (int)target.LastAware);
                            replay.Decorations.Add(new CircleDecoration(true, 0, 350, (start, end), "rgba(0, 0, 0, 0.4)", new PositionConnector(effect.Position)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.DragonBodyVoidAmalgamate:
                    break;
                case (int)ArcDPSEnums.TrashID.VoidAmalgamate:
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidPool, out IReadOnlyList<EffectEvent> allPoolEffects))
                    {
                        var poolEffects = allPoolEffects.Where(x => x.Src == target.AgentItem).ToList();
                        if (!poolEffects.Any())
                        {
                            break;
                        }
                        // sort pool effects by time so that we can grow each effect
                        poolEffects.Sort((a, b) => a.Time.CompareTo(b.Time));
                        double radius = 100.0;
                        double radiusIncrement = log.FightData.IsCM ? 35.0 : 35.0 / 2;
                        for (int i = 0; i < poolEffects.Count - 1; i++)
                        {
                            EffectEvent curEffect = poolEffects[i];
                            EffectEvent nextEffect = poolEffects[i + 1];
                            int start = (int)curEffect.Time;
                            int end = (int)nextEffect.Time;
                            replay.Decorations.Add(new CircleDecoration(true, 0, (int)radius, (start, end), "rgba(59, 0, 16, 0.2)", new PositionConnector(curEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(false, 0, (int)radius, (start, end), "rgba(255, 0, 0, 0.5)", new PositionConnector(curEffect.Position)));
                            radius += radiusIncrement;
                        }
                        // last pool effect ends slightly differently depending on phase
                        // in general, purification -> boss: 2 seconds
                        // boss -> boss: 2 seconds
                        // boss -> purification: 6 seconds
                        // - purification 1 -> jormag: 2 seconds after breakbar broken
                        // - jormag -> primordus: 2 seconds after phasing
                        // - primordus -> kralkatorrik: 2 seconds after phasing
                        // - kralk -> puri 2: 6 seconds after phasing (about when timecaster spawns)
                        // - puri 2 -> mordremoth: 2 seconds after breakbar broken
                        // - mordremoth -> zhaitan: 2 seconds after phasing
                        // - zhaitan -> puri 3: 6 seconds after phasing (about when saltspray spawns)
                        // - puri 3 -> soo won 1: 2 seconds after breakbar broken
                        // - soo won 1 -> puri 4: 1 second
                        // Ideally there's a purification effect that signals the end of the void pools, but it has not been identified yet.
                        EffectEvent lastEffect = poolEffects.Last();
                        int lastStart = (int)lastEffect.Time;
                        int lastEnd = Int32.MaxValue;
                        AbstractSingleActor phaseTarget = FindActiveOrNextPhase(target.FirstAware);
                        switch (phaseTarget.ID)
                        {
                            case (int)ArcDPSEnums.TrashID.PushableVoidAmalgamate:
                            case (int)ArcDPSEnums.TargetID.TheDragonVoidJormag:
                            case (int)ArcDPSEnums.TargetID.TheDragonVoidPrimordus:
                            case (int)ArcDPSEnums.TargetID.TheDragonVoidMordremoth:
                            case (int)ArcDPSEnums.TrashID.VoidTimeCaster:
                            case (int)ArcDPSEnums.TrashID.VoidSaltsprayDragon:
                                lastEnd = (int)(phaseTarget.LastAware + 2000);
                                break;
                            case (int)ArcDPSEnums.TargetID.TheDragonVoidKralkatorrik:
                            case (int)ArcDPSEnums.TargetID.TheDragonVoidZhaitan:
                                lastEnd = (int)(phaseTarget.LastAware + 6000);
                                break;
                            case (int)ArcDPSEnums.TargetID.TheDragonVoidSooWon:
                                if (phaseTarget.GetCurrentHealthPercent(log, target.FirstAware) > 50)
                                {
                                    // Soo-Won 1
                                    AbstractSingleActor killableHeart = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TrashID.KillableVoidAmalgamate));
                                    if (killableHeart != null)
                                    {
                                        lastEnd = (int)(killableHeart.FirstAware + 1000);
                                    }
                                }
                                break;
                        }
                        replay.Decorations.Add(new CircleDecoration(true, 0, (int)radius, (lastStart, lastEnd), "rgba(59, 0, 16, 0.2)", new PositionConnector(lastEffect.Position)));
                        replay.Decorations.Add(new CircleDecoration(false, 0, (int)radius, (lastStart, lastEnd), "rgba(255, 0, 0, 0.5)", new PositionConnector(lastEffect.Position)));
                    }
                    break;
                case (int)ArcDPSEnums.TargetID.TheDragonVoidMordremoth:
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleMordremothPoisonRoarImpact, out IReadOnlyList<EffectEvent> mordremothPoisonEffects))
                    {
                        foreach (EffectEvent effect in mordremothPoisonEffects)
                        {
                            int end = (int)effect.Time;
                            int start = end - 2000;
                            replay.Decorations.Add(new CircleDecoration(true, end, 200, (start, end), "rgba(49, 71, 0, 0.2)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 200, (start, end), "rgba(49, 71, 0, 0.2)", new PositionConnector(effect.Position)));
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
                            replay.Decorations.Add(new CircleDecoration(true, end, 200, (start, end), "rgba(49, 71, 0, 0.2)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 200, (start, end), "rgba(49, 71, 0, 0.2)", new PositionConnector(effect.Position)));
                        }
                    }
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleZhaitanPutridDelugeAoE, out IReadOnlyList<EffectEvent> zhaitanPoisonAoeEffects))
                    {
                        foreach (EffectEvent effect in zhaitanPoisonAoeEffects)
                        {
                            int start = (int)effect.Time;
                            int end = (int)Math.Min(target.LastAware, start + 10000);
                            replay.Decorations.Add(new CircleDecoration(false, 0, 200, (start, end), "rgba(200, 0, 0, 0.5)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 200, (start, end), "rgba(49, 71, 0, 0.2)", new PositionConnector(effect.Position)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TargetID.TheDragonVoidSooWon:
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleSooWonClaw, out IReadOnlyList<EffectEvent> sooWonClawEffects))
                    {
                        foreach (EffectEvent effect in sooWonClawEffects)
                        {
                            int start = (int)effect.Time;
                            int end = start + 2300;
                            replay.Decorations.Add(new PieDecoration(true, 0, 1060, 99.6f, 145, (start, end), "rgba(200, 0, 0, 0.4)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new PieDecoration(true, end, 1060, 99.6f, 145, (start, end), "rgba(200, 0, 0, 0.4)", new PositionConnector(effect.Position)));
                        }
                    }
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleTormentOfTheVoidClawIndicator, out IReadOnlyList<EffectEvent> bouncingOrbClawEffects))
                    {
                        foreach (EffectEvent effect in bouncingOrbClawEffects)
                        {
                            int start = (int)effect.Time;
                            int end = start + 550;
                            replay.Decorations.Add(new CircleDecoration(true, 0, 200, (start, end), "rgba(71, 35, 32, 0.2)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, end, 200, (start, end), "rgba(71, 35, 32, 0.2)", new PositionConnector(effect.Position)));
                        }
                    }
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleTormentOfTheVoidTailIndicator, out IReadOnlyList<EffectEvent> bouncingOrbTailEffects))
                    {
                        foreach (EffectEvent effect in bouncingOrbTailEffects)
                        {
                            int start = (int)effect.Time;
                            int end = start + 1600;
                            replay.Decorations.Add(new CircleDecoration(true, 0, 200, (start, end), "rgba(71, 35, 32, 0.2)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, end, 200, (start, end), "rgba(71, 35, 32, 0.2)", new PositionConnector(effect.Position)));
                        }
                    }
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleTailSlamIndicator, out IReadOnlyList<EffectEvent> tailSlamEffects))
                    {
                        foreach (EffectEvent effect in tailSlamEffects)
                        {
                            int start = (int)effect.Time;
                            int end = start + 1600;
                            replay.Decorations.Add(new RectangleDecoration(true, 0, 3000, 750, (start, end), "rgba(200, 0, 0, 0.2)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new RectangleDecoration(true, end, 3000, 750, (start, end), "rgba(200, 0, 0, 0.2)", new PositionConnector(effect.Position)));
                        }
                    }
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleTsunami1, out IReadOnlyList<EffectEvent> tsunamiEffects))
                    {
                        foreach (EffectEvent effect in tsunamiEffects)
                        {
                            // AoE indicator
                            int indicatorEnd = (int)effect.Time;
                            int indicatorStart = indicatorEnd - 1600;
                            replay.Decorations.Add(new CircleDecoration(true, 0, 235, (indicatorStart, indicatorEnd), "rgba(250, 0, 0, 0.2)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, indicatorEnd, 235, (indicatorStart, indicatorEnd), "rgba(250, 0, 0, 0.2)", new PositionConnector(effect.Position)));
                            // Expanding wave - radius and duration are estimates, can't seem to line up the decoration with actual hits
                            int waveStart = (int)effect.Time;
                            int waveEnd = waveStart + 4500;
                            replay.Decorations.Add(new CircleDecoration(false, waveEnd, 2000, (waveStart, waveEnd), "rgba(0, 0, 250, 0.5)", new PositionConnector(effect.Position)));
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

                        replay.Decorations.Add(new DoughnutDecoration(true, endTime, 300, 500, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new DoughnutDecoration(true, 0, 300, 500, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                    }
                    // Ground Slam - AoE that knocks out
                    var groundSlam = casts.Where(x => x.SkillId == ZhaitansReachGroundSlam || x.SkillId == ZhaitansReachGroundSlamHT).ToList();
                    foreach (AbstractCastEvent c in groundSlam)
                    {
                        int castTime = 0;
                        int radius = 400;
                        int endTime = 0;
                        // 66534 -> Fast AoE -- 64526 -> Slow AoE
                        if (c.SkillId == ZhaitansReachGroundSlam) { castTime = 800; } else if (c.SkillId == ZhaitansReachGroundSlamHT) { castTime = 2500; }
                        endTime = (int)c.Time + castTime;

                        replay.Decorations.Add(new CircleDecoration(true, endTime, radius, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, radius, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
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
            AbstractSingleActor activeDragon = Targets.FirstOrDefault(x => x.FirstAware <= time && x.LastAware >= time && dragonVoidIDs.Contains(x.ID));
            return activeDragon ?? Targets.FirstOrDefault(x => x.FirstAware >= time);
        }

        private AbstractSingleActor FindActiveOrNextPhase(long time)
        {
            var targetIDs = new List<int> {
                (int)ArcDPSEnums.TargetID.TheDragonVoidJormag,
                (int)ArcDPSEnums.TargetID.TheDragonVoidPrimordus,
                (int)ArcDPSEnums.TargetID.TheDragonVoidKralkatorrik,
                (int)ArcDPSEnums.TrashID.VoidTimeCaster,
                (int)ArcDPSEnums.TargetID.TheDragonVoidMordremoth,
                (int)ArcDPSEnums.TargetID.TheDragonVoidZhaitan,
                (int)ArcDPSEnums.TrashID.VoidSaltsprayDragon,
                (int)ArcDPSEnums.TargetID.TheDragonVoidSooWon,
            };
            AbstractSingleActor activeTarget = Targets.FirstOrDefault(x => x.FirstAware <= time && x.LastAware >= time && targetIDs.Contains(x.ID));
            return activeTarget ?? Targets.FirstOrDefault(x => x.FirstAware >= time);
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            var knownEffectsIDs = new HashSet<long>();
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleSpreadCM, out IReadOnlyList<EffectEvent> spreadEffectsCM))
            {
                if (spreadEffectsCM.Count > 0)
                {
                    AddSpreadSelectionDecoration(p, log, replay, spreadEffectsCM, 300, 5500);
                }
            }
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleSpreadNM, out IReadOnlyList<EffectEvent> spreadEffectsNM))
            {
                if (spreadEffectsNM.Count > 0)
                {
                    AddSpreadSelectionDecoration(p, log, replay, spreadEffectsNM, 240, 5000);
                }
            }
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleRedPuddleSelectCM, out IReadOnlyList<EffectEvent> redSelectedEffectsCM))
            {
                if (redSelectedEffectsCM.Count > 0)
                {
                    AddVoidPoolSelectionDecoration(p, replay, redSelectedEffectsCM, 400);
                }
            }
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleRedPuddleSelectNM, out IReadOnlyList<EffectEvent> redSelectedEffectsNM))
            {
                if (redSelectedEffectsNM.Count > 0)
                {
                    AddVoidPoolSelectionDecoration(p, replay, redSelectedEffectsNM, 300);
                }
            }
        }

        private void AddVoidPoolSelectionDecoration(AbstractPlayer p, CombatReplay replay, IReadOnlyList<EffectEvent> redSelectedEffects, int radius)
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
                replay.Decorations.Add(new CircleDecoration(true, end, radius, (start, end), "rgba(250, 50, 0, 0.2)", new AgentConnector(p)));
                replay.Decorations.Add(new CircleDecoration(true, 0, radius, (start, end), "rgba(250, 50, 0, 0.2)", new AgentConnector(p)));
            }
        }

        private void AddSpreadSelectionDecoration(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay, IReadOnlyList<EffectEvent> spreadEffects, int radius, int duration)
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
                replay.Decorations.Add(new CircleDecoration(true, end, radius, (start, effectEnd), "rgba(250, 120, 0, 0.2)", new AgentConnector(p)));
                replay.Decorations.Add(new CircleDecoration(true, 0, radius, (start, effectEnd), "rgba(250, 120, 0, 0.2)", new AgentConnector(p)));
            }
        }

        private void AddPlacedVoidPoolDecoration(IReadOnlyList<EffectEvent> redPuddleEffects, int radius, int duration)
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
                EnvironmentDecorations.Add(new CircleDecoration(true, start + inactiveDuration, radius, (start, puddleEnd), "rgba(250, 0, 0, 0.3)", new PositionConnector(effect.Position)));
                EnvironmentDecorations.Add(new CircleDecoration(true, 0, radius, (start, puddleEnd), "rgba(250, 0, 0, 0.3)", new PositionConnector(effect.Position)));
            }
        }

        private void AddShareTheVoidDecoration(IReadOnlyList<EffectEvent> greenEffects, bool isSuccessful)
        {
            foreach (EffectEvent green in greenEffects)
            {
                int duration = 5000;
                int start = (int)green.Time - duration;
                int end = (int)green.Time;
                string color = isSuccessful ? "rgba(0, 120, 0, 0.4)" : "rgba(120, 0, 0, 0.4)";
                EnvironmentDecorations.Add(new CircleDecoration(true, end, 180, (start, end), "rgba(0, 120, 0, 0.4)", new PositionConnector(green.Position)));
                EnvironmentDecorations.Add(new CircleDecoration(true, 0, 180, (start, end), color, new PositionConnector(green.Position)));
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
                if (voidMelters.Count(x => Math.Abs(x.FirstAware - firstAware) < ParserHelper.ServerDelayConstant) > 5)
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

            if (log.FightData.Success && log.FightData.IsCM)
            {
                IReadOnlyList<AbstractBuffEvent> voidwalker = log.CombatData.GetBuffData(AchievementEligibilityVoidwalker);
                bool hasVoidwalkerBeenAdded = false;
                // Added a CM mode check because the eligibility has been bugged for a while and showed up in normal mode.
                if (voidwalker.Any())
                {
                    foreach (Player p in log.PlayerList)
                    {
                        if (p.HasBuff(log, AchievementEligibilityVoidwalker, log.FightData.FightEnd - ServerDelayConstant))
                        {
                            InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityVoidwalker], 1));
                            hasVoidwalkerBeenAdded = true;
                            break;
                        }
                    }
                }
                if (!hasVoidwalkerBeenAdded && CustomCheckVoidwalkerEligibility(log)) // In case all 10 players already have voidwalker
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
                    if (bgm.BuffChart.Any(x => x.Value >= 3)) { return false; }
                }
            }
            return true;
        }
    }
}
