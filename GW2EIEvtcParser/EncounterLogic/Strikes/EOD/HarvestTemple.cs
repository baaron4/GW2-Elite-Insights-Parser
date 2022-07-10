using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class HarvestTemple : EODStrike
    {
        public HarvestTemple(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerBuffApplyMechanic(InfluenceOfTheVoidEffect, "Influence of the Void", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkPurple), "Void.D", "Received Void debuff", "Void Debuff", 150),
                new HitOnPlayerMechanic(InfluenceOfTheVoidSkill, "Influence of the Void Hit", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.DarkPurple), "Void.H", "Hit by Void", "Void Hit", 150),
                new HitOnPlayerMechanic(new long[] {BreathOfJormag1, BreathOfJormag2, BreathOfJormag3 }, "Breath of Jormag", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Blue), "J.Breath.H", "Hit by Jormag Breath", "Jormag Breath", 150),
                new HitOnPlayerMechanic(LavaSlam, "Lava Slam", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Red), "Slam.H", "Hit by Primordus Slam", "Primordus Slam", 150),
                new HitOnPlayerMechanic(CrystalBarrage, "Crystal Barrage", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Purple), "Barrage.H", "Hit by Crystal Barrage", "Barrage", 150),
                new HitOnPlayerMechanic(BrandingBeam, "Branding Beam", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Purple), "Beam.H", "Hit by Kralkatorrik Beam", "Kralkatorrik Beam", 150),
                new HitOnPlayerMechanic(Shockwave, "Shock Wave", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Green), "ShckWv.H", "Hit by Mordremoth Shockwave", "Mordremoth Shockwave", 150),
                new HitOnPlayerMechanic(ScreamOfZhaitan, "Scream of Zhaitan", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.DarkGreen), "Scream.H", "Hit by Zhaitan Scream", "Zhaitan Scream", 150),
                new HitOnPlayerMechanic(HydroBurst, "Hydro Burst", new MechanicPlotlySetting(Symbols.Circle, Colors.LightBlue), "Whrlpl.H", "Hit by Whirlpool", "Whirlpool", 150),
                new HitOnPlayerMechanic(new long[] {TsunamiSlam1, TsunamiSlam2 }, "Tsunami Slam", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.LightBlue), "Tsunami.H", "Hit by Soo-Won Tsunami", "Soo-Won Tsunami", 150),
                new HitOnPlayerMechanic(ClawSlap, "Claw Slap", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.LightBlue), "Claw.H", "Hit by Soo-Won Claw", "Soo-Won Claw", 150),
            }
            );
            Icon = "https://i.imgur.com/gZRqzlr.png";
            Extension = "harvsttmpl";
            EncounterCategoryInformation.InSubCategoryOrder = 3;
            EncounterID |= 0x000004;
        }
        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/WCygAeH.png",
                            (788, 788),
                            (-812,-21820,2037,-18971)/*,
                            (-15360, -36864, 15360, 39936),
                            (3456, 11012, 4736, 14212)*/);
        }
        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            var subPhasesData = new List<(long start, long end, string name, NPC target)>();
            foreach (NPC target in Targets)
            {
                long mainPhaseEnd = Math.Min(target.LastAware, log.FightData.FightEnd);
                switch (target.ID)
                {
                    case (int)ArcDPSEnums.TrashID.VoidSaltsprayDragon:
                    case (int)ArcDPSEnums.TrashID.VoidTimeCaster:
                    case (int)ArcDPSEnums.TrashID.VoidObliterator:
                        subPhasesData.Add((target.FirstAware, mainPhaseEnd, target.Character, target));
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidJormag:
                        phases[0].AddTarget(target);
                        subPhasesData.Add((target.FirstAware, mainPhaseEnd, "Jormag", target));
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidKralkatorrik:
                        phases[0].AddTarget(target);
                        subPhasesData.Add((target.FirstAware, mainPhaseEnd, "Kralkatorrik", target));
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidMordremoth:
                        phases[0].AddTarget(target);
                        subPhasesData.Add((target.FirstAware, mainPhaseEnd, "Mordremoth", target));
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidPrimordus:
                        phases[0].AddTarget(target);
                        subPhasesData.Add((target.FirstAware, mainPhaseEnd, "Primordus", target));
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidSooWon:
                        phases[0].AddTarget(target);
                        subPhasesData.Add((target.FirstAware, mainPhaseEnd, "Soo-Won", target));
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
                                subPhasesData.Add((start, end, "Soo-Won " + (++id), target));
                            }
                        }
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidZhaitan:
                        phases[0].AddTarget(target);
                        subPhasesData.Add((target.FirstAware, mainPhaseEnd, "Zhaitan", target));
                        break;
                    case (int)ArcDPSEnums.TargetID.DummyTarget:
                        phases[0].AddTarget(target);
                        break;
                }
            }
            if (!requirePhases)
            {
                return phases;
            }
            foreach ((long start, long end, string name, NPC target) subPhaseData in subPhasesData)
            {
                var subPhase = new PhaseData(subPhaseData.start, subPhaseData.end, subPhaseData.name);
                subPhase.AddTarget(subPhaseData.target);
                phases.Add(subPhase);
            }
            int purificationID = 0;
            foreach (NPC voidAmal in Targets.Where(x => x.ID == (int)ArcDPSEnums.TrashID.PushableVoidAmalgamate || x.ID == (int)ArcDPSEnums.TrashID.KillableVoidAmalgamate))
            {
                long end;
                DeadEvent deadEvent = log.CombatData.GetDeadEvents(voidAmal.AgentItem).LastOrDefault();
                if (deadEvent == null)
                {
                    DespawnEvent despawnEvent = log.CombatData.GetDespawnEvents(voidAmal.AgentItem).LastOrDefault();
                    if (despawnEvent == null)
                    {
                        end = voidAmal.LastAware;
                    } else
                    {
                        end = despawnEvent.Time;
                    }
                } else
                {
                    end = deadEvent.Time;
                }
                var purificationPhase = new PhaseData(Math.Max(voidAmal.FirstAware, 0), Math.Min(end, log.FightData.FightEnd), "Purification " + (++purificationID));
                purificationPhase.AddTarget(voidAmal);
                phases.Add(purificationPhase);
            }
            return phases;
        }

        protected override List<int> GetSuccessCheckIds()
        {
            return new List<int>
            {
            };
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.DummyTarget,
                (int)ArcDPSEnums.TargetID.TheDragonVoidJormag,
                (int)ArcDPSEnums.TargetID.TheDragonVoidKralkatorrik,
                (int)ArcDPSEnums.TargetID.TheDragonVoidMordremoth,
                (int)ArcDPSEnums.TargetID.TheDragonVoidPrimordus,
                (int)ArcDPSEnums.TargetID.TheDragonVoidSooWon,
                (int)ArcDPSEnums.TargetID.TheDragonVoidZhaitan,
                (int)ArcDPSEnums.TrashID.VoidSaltsprayDragon,
                (int)ArcDPSEnums.TrashID.VoidObliterator,
                (int)ArcDPSEnums.TrashID.VoidTimeCaster,
                (int)ArcDPSEnums.TrashID.PushableVoidAmalgamate,
                (int)ArcDPSEnums.TrashID.KillableVoidAmalgamate
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
                ArcDPSEnums.TrashID.VoidGiant,
                ArcDPSEnums.TrashID.VoidMelter,
                ArcDPSEnums.TrashID.VoidRotswarmer,
                ArcDPSEnums.TrashID.VoidSaltsprayDragon,
                ArcDPSEnums.TrashID.VoidSkullpiercer,
                ArcDPSEnums.TrashID.VoidStormseer,
                ArcDPSEnums.TrashID.VoidTangler,
                ArcDPSEnums.TrashID.VoidTimeCaster,
                ArcDPSEnums.TrashID.VoidWarforged1,
                ArcDPSEnums.TrashID.VoidWarforged2,
                ArcDPSEnums.TrashID.DragonBodyVoidAmalgamate
            };
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "The Dragonvoid";
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            // no bouny chest detection, the reward is delayed
            AbstractSingleActor soowon = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.TheDragonVoidSooWon);
            if (soowon != null)
            {
                AttackTargetEvent attackTargetEvent = combatData.GetAttackTargetEvents(soowon.AgentItem).FirstOrDefault();
                var targetables = combatData.GetTargetableEvents(attackTargetEvent.AttackTarget).Where(x => x.Time >= soowon.FirstAware).ToList();
                var targetOffs = targetables.Where(x => !x.Targetable).ToList();
                if (targetOffs.Count == 2)
                {
                    AbstractHealthDamageEvent lastDamageTaken = combatData.GetDamageTakenData(soowon.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                    if (lastDamageTaken != null)
                    {
                        if (!AtLeastOnePlayerAlive(combatData, fightData, Math.Min(targetOffs[1].Time + 100, fightData.FightEnd), playerAgents))
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
                var posFacingHP = combatData.Where(x => x.SrcMatchesAgent(dragonVoid) && (x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate)).ToList();
                posFacingHP.AddRange(combatData.Where(x => x.SrcMatchesAgent(atAgent) && (x.IsStateChange == ArcDPSEnums.StateChange.Position || x.IsStateChange == ArcDPSEnums.StateChange.Rotation)));
                //
                foreach (CombatItem targetOn in targetOns)
                {
                    // If Soo Won has been already created, we break
                    if (index >= idsToUse.Count)
                    {
                        break;
                    }
                    int id = (int)idsToUse[index++];
                    long start = targetOn.Time;
                    long end = fightData.FightEnd;
                    CombatItem targetOff = targetOffs.FirstOrDefault(x => x.Time > start);
                    // Don't split Soo won into two
                    if (targetOff != null && id != (int)ArcDPSEnums.TargetID.TheDragonVoidSooWon)
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
                                if (c.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate) {
                                    // Discard hp update that goes up close to death time
                                    if (c.DstAgent >= lastHPUpdate && c.Time > extra.LastAware - 2000)
                                    {
                                        continue;
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
                    foreach (CombatItem c in posFacingHP)
                    {
                        var cExtra = new CombatItem(c);
                        cExtra.OverrideTime(extra.FirstAware);
                        cExtra.OverrideSrcAgent(extra.Agent);
                        combatData.Add(cExtra);
                    }
                }
            }
            //
            IReadOnlyList<AgentItem> voidAmalgamates = agentData.GetNPCsByID((int)ArcDPSEnums.TrashID.VoidAmalgamate);
            bool needRefresh = false;
            foreach (AgentItem voidAmal in voidAmalgamates)
            {
                if (combatData.Where(x => x.SkillID == VoidShell && x.IsBuffApply() && x.SrcMatchesAgent(voidAmal)).Any())
                {
                    voidAmal.OverrideID(ArcDPSEnums.TrashID.PushableVoidAmalgamate);
                    needRefresh = true;
                }
            }
            AgentItem dragonBodyVoidAmalgamate = voidAmalgamates.MaxBy(x => x.LastAware - x.FirstAware);
            if (dragonBodyVoidAmalgamate != null)
            {
                dragonBodyVoidAmalgamate.OverrideID(ArcDPSEnums.TrashID.DragonBodyVoidAmalgamate);
                needRefresh = true;
            }
            if (needRefresh)
            {
                agentData.Refresh();
            }
            if (index == 0)
            {
                // Add dummy target as there are no dragon voids
                agentData.AddCustomNPCAgent(0, fightData.FightEnd, "Dummy Harvest Temple", Spec.NPC, (int)ArcDPSEnums.TargetID.DummyTarget, true);
                Targetless = true;
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
                switch(target.ID)
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
                            ScreamOfZhaitan,
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

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            if (!Targetless)
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
            }
            IReadOnlyList<AgentItem> voidMelters = agentData.GetNPCsByID((int)ArcDPSEnums.TrashID.VoidMelter);
            if (voidMelters.Count > 5)
            {
                long firstAware = voidMelters[0].FirstAware;
                if (voidMelters.Count(x => Math.Abs(x.FirstAware - firstAware) < ParserHelper.ServerDelayConstant) > 5)
                {
                    return FightData.EncounterMode.CM;
                }
            }
            return FightData.EncounterMode.Normal;
        }
    }
}
