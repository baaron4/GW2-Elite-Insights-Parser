using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
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
            var subPhasesData = new List<(long start, long end, string name, NPC target, bool canBeSubPhase)>();
            foreach (NPC target in Targets)
            {
                long mainPhaseEnd = Math.Min(target.LastAware, log.FightData.FightEnd);
                switch (target.ID)
                {
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
            foreach ((long start, long end, string name, NPC target, bool canBeSubPhase) in subPhasesData)
            {
                var subPhase = new PhaseData(start, end, name);
                subPhase.CanBeSubPhase = canBeSubPhase;
                subPhase.AddTarget(target);
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
                    int id = (int)idsToUse[index++];
                    long start = targetOn.Time;
                    long end = dragonVoid.LastAware;
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
            // Add missing agents
            for (int i = index; i < idsToUse.Count; i++)
            {
                agentData.AddCustomNPCAgent(long.MaxValue, long.MaxValue, "Dragonvoid", Spec.NPC, (int)idsToUse[i], false);
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

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            var knownEffectsIDs = new HashSet<long>();
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TrashID.PushableVoidAmalgamate:
                    //
                    EffectGUIDEvent purificationZone = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.HarvestTemplePurificationZones);
                    if (purificationZone != null)
                    {
                        var voidShells = log.CombatData.GetBuffData(VoidShell).Where(x => x.To == target.AgentItem).ToList();
                        var voidShellRemovals = voidShells.Where(x => x is BuffRemoveSingleEvent || x is BuffRemoveAllEvent).ToList();
                        int voidShellAppliesCount = voidShells.Where(x => x is BuffApplyEvent).Count();
                        int voidShellRemovalOffset = 0;
                        int purificationAdd = 0;
                        bool breakPurification = false;
                        var purificationZoneEffects = log.CombatData.GetEffectEvents(purificationZone.ContentID).Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware).ToList();
                        knownEffectsIDs.Add(purificationZone.ContentID);
                        foreach (EffectEvent purificationZoneEffect in purificationZoneEffects)
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
                    EffectGUIDEvent lightning = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.HarvestTemplePurificationLightnings);
                    if (lightning != null)
                    {
                        var lightningEffects = log.CombatData.GetEffectEvents(lightning.ContentID).Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware).ToList();
                        knownEffectsIDs.Add(lightning.ContentID);
                        foreach (EffectEvent lightningEffect in lightningEffects)
                        {
                            int duration = 3000;
                            int start = (int)lightningEffect.Time - duration;
                            int end = (int)lightningEffect.Time;
                            replay.Decorations.Add(new CircleDecoration(true, end, 180, (start, end), "rgba(255, 180, 0, 0.2)", new PositionConnector(lightningEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 180, (start, end), "rgba(255, 180, 0, 0.2)", new PositionConnector(lightningEffect.Position)));
                        }
                    }
                    //
                    EffectGUIDEvent fireBall = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.HarvestTemplePurificationFireBalls);
                    if (fireBall != null)
                    {
                        var fireBallEffects = log.CombatData.GetEffectEvents(fireBall.ContentID).Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware).ToList();
                        knownEffectsIDs.Add(fireBall.ContentID);
                        foreach (EffectEvent fireBallEffect in fireBallEffects)
                        {
                            int startLoad = (int)fireBallEffect.Time - 2000;
                            int endLoad = (int)fireBallEffect.Time;
                            replay.Decorations.Add(new CircleDecoration(true, endLoad, 180, (startLoad, endLoad), "rgba(250, 0, 0, 0.2)", new PositionConnector(fireBallEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 180, (startLoad, endLoad), "rgba(250, 0, 0, 0.2)", new PositionConnector(fireBallEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 180, (endLoad, endLoad + 2000), "rgba(250, 0, 0, 0.4)", new PositionConnector(fireBallEffect.Position)));
                        }
                    }
                    //
                    EffectGUIDEvent voidZone = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.HarvestTemplePurificationVoidZones);
                    if (voidZone != null)
                    {
                        var voidZoneEffects = log.CombatData.GetEffectEvents(voidZone.ContentID).Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware).ToList();
                        knownEffectsIDs.Add(voidZone.ContentID);
                        foreach (EffectEvent voidZoneEffect in voidZoneEffects)
                        {
                            int start = (int)voidZoneEffect.Time;
                            int end = start + 5000;
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, 90, 230, RadianToDegreeF(voidZoneEffect.Orientation.Z),  (start, end), "rgba(150, 0, 150, 0.2)", new PositionConnector(voidZoneEffect.Position)));
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, end, 90, 230, RadianToDegreeF(voidZoneEffect.Orientation.Z), (start, end), "rgba(250, 0, 250, 0.3)", new PositionConnector(voidZoneEffect.Position)));
                        }
                    }
                    //
                    EffectGUIDEvent beeLaunch = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.HarvestTemplePurificationBeeLaunch);
                    if (beeLaunch != null)
                    {
                        var beeLaunchEffects = log.CombatData.GetEffectEvents(beeLaunch.ContentID).Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware).ToList();
                        knownEffectsIDs.Add(beeLaunch.ContentID);
                        foreach (EffectEvent beeLaunchEffect in beeLaunchEffects)
                        {
                            int start = (int)beeLaunchEffect.Time;
                            int end = start + 3000;
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, 380, 30, RadianToDegreeF(beeLaunchEffect.Orientation.Z), 190, (start, end), "rgba(250, 50, 0, 0.4)", new PositionConnector(beeLaunchEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, end, 280, (start, end), "rgba(250, 150, 0, 0.2)", new PositionConnector(beeLaunchEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 280, (start, end), "rgba(250, 150, 0, 0.2)", new PositionConnector(beeLaunchEffect.Position)));
                        }
                    }
                    //
                    EffectGUIDEvent poisonTrail = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.HarvestTemplePurificationPoisonTrail);
                    if (poisonTrail != null)
                    {
                        var poisonTrailEffects = log.CombatData.GetEffectEvents(poisonTrail.ContentID).Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware).ToList();
                        knownEffectsIDs.Add(poisonTrail.ContentID);
                        foreach (EffectEvent poisonTrailEffect in poisonTrailEffects)
                        {
                            int startLoad = (int)poisonTrailEffect.Time - 1000;
                            int start = (int)poisonTrailEffect.Time;
                            int end = start + 15000;
                            replay.Decorations.Add(new CircleDecoration(true, start, 220, (startLoad, start), "rgba(0, 150, 0, 0.2)", new PositionConnector(poisonTrailEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 220, (start, end), "rgba(0, 150, 0, 0.4)", new PositionConnector(poisonTrailEffect.Position)));
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
                    EffectGUIDEvent iceShard = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.HarvestTempleJormagIceShards);
                    if (iceShard != null) 
                    {
                        IReadOnlyList<EffectEvent> iceShardEffects = log.CombatData.GetEffectEvents(iceShard.ContentID);
                        knownEffectsIDs.Add(iceShard.ContentID);
                        foreach (EffectEvent iceShardEffect in iceShardEffects)
                        {
                            int duration = 2500;
                            int start = (int)iceShardEffect.Time;
                            int end = start + duration;
                            replay.Decorations.Add(new CircleDecoration(true, end, 160, (start, end), "rgba(0, 50, 180, 0.2)", new PositionConnector(iceShardEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 160, (start, end), "rgba(0, 50, 180, 0.2)", new PositionConnector(iceShardEffect.Position)));
                        }
                    }
                    //CombatReplay.DebugEffects(target, log, replay, knownEffectsIDs, 54000, 57000);
                    break;
                case (int)ArcDPSEnums.TargetID.TheDragonVoidPrimordus:
                    EffectGUIDEvent smallJaw = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.HarvestTemplePrimordusSmallJaw);
                    if (smallJaw != null)
                    {
                        IReadOnlyList<EffectEvent> smallJawEffects = log.CombatData.GetEffectEvents(smallJaw.ContentID);
                        knownEffectsIDs.Add(smallJaw.ContentID);
                        // The effect is slightly shifted on X
                        var jawPosition = new Point3D(610, -21400.3f, -15417.3f);
                        foreach (EffectEvent smallJawEffect in smallJawEffects)
                        {
                            int duration = 3500;
                            int start = (int)smallJawEffect.Time - duration;
                            int end = (int)smallJawEffect.Time;
                            replay.Decorations.Add(new CircleDecoration(true, end, 580, (start, end), "rgba(200, 100, 0, 0.2)", new PositionConnector(jawPosition)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 580, (start, end), "rgba(200, 100, 0, 0.2)", new PositionConnector(jawPosition)));
                        }
                    }
                    EffectGUIDEvent bigJaw = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.HarvestTemplePrimordusBigJaw);
                    if (bigJaw != null)
                    {
                        IReadOnlyList<EffectEvent> bigJawEffects = log.CombatData.GetEffectEvents(bigJaw.ContentID);
                        knownEffectsIDs.Add(bigJaw.ContentID);
                        foreach (EffectEvent bigJawEffect in bigJawEffects)
                        {
                            int start = (int)bigJawEffect.Time;
                            int end = start + 7000;
                            replay.Decorations.Add(new CircleDecoration(true, end, 1700, (start, end), "rgba(200, 100, 0, 0.2)", new AgentConnector(target)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 1700, (start, end), "rgba(200, 100, 0, 0.2)", new AgentConnector(target)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 1700, (end, end + 5000), "rgba(200, 0, 0, 0.4)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TargetID.TheDragonVoidKralkatorrik:
                    //CombatReplay.DebugEffects(target, log, replay, knownEffectsIDs, 230000, 238000);
                    break;
                case (int)ArcDPSEnums.TrashID.DragonBodyVoidAmalgamate:
                    EffectGUIDEvent green = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.HarvestTempleGreen);
                    if (green != null)
                    {
                        IReadOnlyList<EffectEvent> greenEffects = log.CombatData.GetEffectEvents(green.ContentID);
                        knownEffectsIDs.Add(green.ContentID);
                        foreach (EffectEvent greenEffect in greenEffects)
                        {
                            int duration = 5000;
                            int start = (int)greenEffect.Time - duration;
                            int end = (int)greenEffect.Time;
                            replay.Decorations.Add(new CircleDecoration(true, end, 120, (start, end), "rgba(0, 120, 0, 0.4)", new PositionConnector(greenEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 120, (start, end), "rgba(0, 120, 0, 0.4)", new PositionConnector(greenEffect.Position)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.VoidWarforged1:
                case (int)ArcDPSEnums.TrashID.VoidWarforged2:
                    //CombatReplay.DebugEffects(target, log, replay, knownEffectsIDs, target.FirstAware, target.LastAware, true);
                    break;
                default:
                    break;
            }
        }

        private AbstractSingleActor FindActiveDragonVoid(long time)
        {
            var dragonVoidIDs = new List<int> {
                (int)ArcDPSEnums.TargetID.TheDragonVoidJormag,
                (int)ArcDPSEnums.TargetID.TheDragonVoidPrimordus,
                (int)ArcDPSEnums.TargetID.TheDragonVoidKralkatorrik,
                (int)ArcDPSEnums.TargetID.TheDragonVoidMordremoth,
                (int)ArcDPSEnums.TargetID.TheDragonVoidZhaitan,
                (int)ArcDPSEnums.TargetID.TheDragonVoidSooWon,
            };
            return Targets.FirstOrDefault(x => x.FirstAware <= time && x.LastAware >= time && dragonVoidIDs.Contains(x.ID));
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            var knownEffectsIDs = new HashSet<long>();
            EffectGUIDEvent spread = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.HarvestTempleSpread);
            if (spread != null)
            {
                var spreadEffects = log.CombatData.GetEffectEvents(spread.ContentID).Where(x => x.Dst == p.AgentItem).ToList();
                knownEffectsIDs.Add(spread.ContentID);
                foreach (EffectEvent spreadEffect in spreadEffects)
                {
                    int duration = 5500;
                    int start = (int)spreadEffect.Time;
                    int end = start + duration;
                    AbstractSingleActor dragonVoid = FindActiveDragonVoid(spreadEffect.Time);
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
                    replay.Decorations.Add(new CircleDecoration(true, end, 240, (start, effectEnd), "rgba(250, 120, 0, 0.2)", new AgentConnector(p)));
                    replay.Decorations.Add(new CircleDecoration(true, 0, 240, (start, effectEnd), "rgba(250, 120, 0, 0.2)", new AgentConnector(p)));
                }
            }
            EffectGUIDEvent redSelected = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.HarvestTempleRedPuddleSelect);
            if (redSelected != null)
            {
                var redSelectedEffects = log.CombatData.GetEffectEvents(redSelected.ContentID).Where(x => x.Dst == p.AgentItem).ToList();
                knownEffectsIDs.Add(redSelected.ContentID);
                foreach (EffectEvent redSelectedEffect in redSelectedEffects)
                {
                    int duration = 6500;
                    int start = (int)redSelectedEffect.Time;
                    int end = start + duration;
                    AbstractSingleActor dragonVoid = FindActiveDragonVoid(redSelectedEffect.Time);
                    if (dragonVoid == null)
                    {
                        continue;
                    }
                    int puddleEnd = (int)dragonVoid.LastAware;
                    int effectEnd = Math.Min(puddleEnd, end);
                    replay.Decorations.Add(new CircleDecoration(true, end, 280, (start, effectEnd), "rgba(250, 50, 0, 0.2)", new AgentConnector(p)));
                    replay.Decorations.Add(new CircleDecoration(true, 0, 280, (start, effectEnd), "rgba(250, 50, 0, 0.2)", new AgentConnector(p)));
                    Point3D pos = p.GetCurrentPosition(log, end);
                    if (pos == null)
                    {
                        continue;
                    }
                    replay.Decorations.Add(new CircleDecoration(true, end + 1000, 280, (end, puddleEnd), "rgba(250, 0, 0, 0.3)", new PositionConnector(pos)));
                    replay.Decorations.Add(new CircleDecoration(true, 0, 280, (end, puddleEnd), "rgba(250, 0, 0, 0.3)", new PositionConnector(pos)));
                }
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
