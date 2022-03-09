using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class HarvestTemple : CanthaStrike
    {
        public HarvestTemple(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            }
            );
            Icon = "https://i.imgur.com/gZRqzlr.png";
            Extension = "harvsttmpl";
            EncounterCategoryInformation.InSubCategoryOrder = 3;
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
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidPrimordious:
                        phases[0].AddTarget(target);
                        subPhasesData.Add((target.FirstAware, mainPhaseEnd, "Primordious", target));
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
                (int)ArcDPSEnums.TargetID.TheDragonVoidPrimordious,
                (int)ArcDPSEnums.TargetID.TheDragonVoidSooWon,
                (int)ArcDPSEnums.TargetID.TheDragonVoidZhaitan,
                (int)ArcDPSEnums.TrashID.VoidSaltsprayDragon,
                (int)ArcDPSEnums.TrashID.VoidObliterator,
                (int)ArcDPSEnums.TrashID.VoidTimeCaster,
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
                ArcDPSEnums.TrashID.VoidAmalgamate1,
                ArcDPSEnums.TrashID.VoidAmalgamate2,
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
                    fightData.SetSuccess(true, targetOffs[1].Time);
                }
            }
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            var attackTargetEvents = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.AttackTarget).ToList();
            var idsToUse = new List<ArcDPSEnums.TargetID> { 
                ArcDPSEnums.TargetID.TheDragonVoidJormag,
                ArcDPSEnums.TargetID.TheDragonVoidPrimordious,
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
                var posFacingHP = combatData.Where(x => x.SrcMatchesAgent(dragonVoid) && (x.IsStateChange == ArcDPSEnums.StateChange.Position || x.IsStateChange == ArcDPSEnums.StateChange.Rotation || x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate)).ToList();
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
                    AgentItem extra = agentData.AddCustomNPCAgent(start, end, dragonVoid.Name, dragonVoid.Spec, id, false, dragonVoid.Toughness, dragonVoid.Healing, dragonVoid.Condition, dragonVoid.Concentration, dragonVoid.HitboxWidth, dragonVoid.HitboxHeight);
                    foreach (CombatItem c in combatData)
                    {
                        if (extra.InAwareTimes(c.Time))
                        {
                            if (c.SrcMatchesAgent(dragonVoid, extensions))
                            {
                                // Avoid making the gadget go back to 100% hp on "death"
                                if (c.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate && c.DstAgent == 10000 && c.Time > extra.LastAware - 2000) {
                                    continue;
                                }
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
            if (index == 0)
            {
                // Add dummy target as there are no dragon voids
                agentData.AddCustomNPCAgent(0, fightData.FightEnd, "Dummy Harvest Temple", Spec.NPC, (int)ArcDPSEnums.TargetID.DummyTarget, true);
                Targetless = true;
            }
            ComputeFightTargets(agentData, combatData, extensions);
            foreach (NPC target in Targets)
            {
                switch(target.ID)
                {
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidJormag:
                        target.OverrideName("The JormagVoid");
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidKralkatorrik:
                        target.OverrideName("The KralkatorrikVoid");
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidMordremoth:
                        target.OverrideName("The MordremothVoid");
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidPrimordious:
                        target.OverrideName("The PrimordiusVoid");
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidSooWon:
                        target.OverrideName("The SooWonVoid");
                        break;
                    case (int)ArcDPSEnums.TargetID.TheDragonVoidZhaitan:
                        target.OverrideName("The ZhaitanVoid");
                        break;
                }
            }
        }
    }
}
