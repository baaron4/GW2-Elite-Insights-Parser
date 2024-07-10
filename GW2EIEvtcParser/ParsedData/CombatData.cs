using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.ParsedData
{
    public class CombatData
    {
        public bool HasMovementData { get; }

        //private List<CombatItem> _healingData;
        //private List<CombatItem> _healingReceivedData;
        private readonly StatusEventsContainer _statusEvents = new StatusEventsContainer();
        private readonly MetaEventsContainer _metaDataEvents = new MetaEventsContainer();
        private readonly HashSet<long> _skillIds;
        private readonly Dictionary<long, List<AbstractBuffEvent>> _buffData;
        private Dictionary<long, Dictionary<uint, List<AbstractBuffEvent>>> _buffDataByInstanceID;
        private Dictionary<long, List<BuffRemoveAllEvent>> _buffRemoveAllData;
        private readonly Dictionary<AgentItem, List<AbstractBuffEvent>> _buffDataByDst;
        private Dictionary<long, Dictionary<AgentItem, List<AbstractBuffEvent>>> _buffDataByIDByDst;
        private readonly Dictionary<AgentItem, List<AbstractHealthDamageEvent>> _damageData;
        private readonly Dictionary<AgentItem, List<AbstractBreakbarDamageEvent>> _breakbarDamageData;
        private readonly Dictionary<AgentItem, List<CrowdControlEvent>> _crowControlData;
        private readonly Dictionary<long, List<AbstractBreakbarDamageEvent>> _breakbarDamageDataById;
        private readonly Dictionary<long, List<AbstractHealthDamageEvent>> _damageDataById;
        private readonly Dictionary<long, List<CrowdControlEvent>> _crowControlDataById;
        private readonly Dictionary<AgentItem, List<AnimatedCastEvent>> _animatedCastData;
        private readonly Dictionary<AgentItem, List<InstantCastEvent>> _instantCastData;
        private readonly Dictionary<AgentItem, List<WeaponSwapEvent>> _weaponSwapData;
        private readonly Dictionary<long, List<AnimatedCastEvent>> _animatedCastDataById;
        private readonly Dictionary<long, List<InstantCastEvent>> _instantCastDataById;
        private readonly Dictionary<AgentItem, List<AbstractHealthDamageEvent>> _damageTakenData;
        private readonly Dictionary<AgentItem, List<AbstractBreakbarDamageEvent>> _breakbarDamageTakenData;
        private readonly Dictionary<AgentItem, List<CrowdControlEvent>> _crowControlTakenData;
        private readonly List<RewardEvent> _rewardEvents = new List<RewardEvent>();
        // EXTENSIONS
        public EXTHealingCombatData EXTHealingCombatData { get; internal set; }
        public EXTBarrierCombatData EXTBarrierCombatData { get; internal set; }
        public bool HasEXTHealing => EXTHealingCombatData != null;
        public bool HasEXTBarrier => EXTBarrierCombatData != null;

        internal bool UseBuffInstanceSimulator { get; } = false;

        internal bool HasStackIDs { get; }

        public bool HasBreakbarDamageData { get; } = false;
        public bool HasEffectData { get; } = false;

        private void EIBuffParse(IReadOnlyList<Player> players, SkillData skillData, FightData fightData, EvtcVersionEvent evtcVersion)
        {
            var toAdd = new List<AbstractBuffEvent>();
            foreach (Player p in players)
            {
                if (p.Spec == Spec.Weaver)
                {
                    toAdd.AddRange(WeaverHelper.TransformWeaverAttunements(GetBuffDataByDst(p.AgentItem), _buffData, p.AgentItem, skillData));
                }
                if (p.Spec == Spec.Virtuoso)
                {
                    toAdd.AddRange(VirtuosoHelper.TransformVirtuosoBladeStorage(GetBuffDataByDst(p.AgentItem), p.AgentItem, skillData, evtcVersion));
                }
                if (p.BaseSpec == Spec.Elementalist && p.Spec != Spec.Weaver)
                {
                    ElementalistHelper.RemoveDualBuffs(GetBuffDataByDst(p.AgentItem), _buffData, skillData);
                }
            }
            toAdd.AddRange(fightData.Logic.SpecialBuffEventProcess(this, skillData));
            var buffIDsToSort = new HashSet<long>();
            var buffAgentsToSort = new HashSet<AgentItem>();
            foreach (AbstractBuffEvent bf in toAdd)
            {
                if (_buffDataByDst.TryGetValue(bf.To, out List<AbstractBuffEvent> buffByDstList))
                {
                    buffByDstList.Add(bf);
                }
                else
                {
                    _buffDataByDst[bf.To] = new List<AbstractBuffEvent>()
                    {
                        bf
                    };
                }
                buffAgentsToSort.Add(bf.To);
                if (_buffData.TryGetValue(bf.BuffID, out List<AbstractBuffEvent> buffByIDList))
                {
                    buffByIDList.Add(bf);
                }
                else
                {
                    _buffData[bf.BuffID] = new List<AbstractBuffEvent>()
                    {
                        bf
                    };
                }
                buffIDsToSort.Add(bf.BuffID);
            }
            foreach (long buffID in buffIDsToSort)
            {
                _buffData[buffID] = _buffData[buffID].OrderBy(x => x.Time).ToList();
            }
            foreach (AgentItem a in buffAgentsToSort)
            {
                _buffDataByDst[a] = _buffDataByDst[a].OrderBy(x => x.Time).ToList();
            }
            if (toAdd.Count != 0)
            {
                BuildBuffDependentContainers();
            }
        }

        private void EIDamageParse(SkillData skillData, FightData fightData)
        {
            var toAdd = new List<AbstractHealthDamageEvent>();
            toAdd.AddRange(fightData.Logic.SpecialDamageEventProcess(this, skillData));
            var idsToSort = new HashSet<long>();
            var dstToSort = new HashSet<AgentItem>();
            var srcToSort = new HashSet<AgentItem>();
            foreach (AbstractHealthDamageEvent de in toAdd)
            {
                if (_damageTakenData.TryGetValue(de.To, out List<AbstractHealthDamageEvent> damageTakenList))
                {
                    damageTakenList.Add(de);
                }
                else
                {
                    _damageTakenData[de.To] = new List<AbstractHealthDamageEvent>()
                    {
                        de
                    };
                }
                dstToSort.Add(de.To);
                if (_damageData.TryGetValue(de.From, out List<AbstractHealthDamageEvent> damageDoneList))
                {
                    damageDoneList.Add(de);
                }
                else
                {
                    _damageData[de.From] = new List<AbstractHealthDamageEvent>()
                    {
                        de
                    };
                }
                srcToSort.Add(de.From);
                if (_damageDataById.TryGetValue(de.SkillId, out List<AbstractHealthDamageEvent> damageDoneByIDList))
                {
                    damageDoneByIDList.Add(de);
                }
                else
                {
                    _damageDataById[de.SkillId] = new List<AbstractHealthDamageEvent>()
                    {
                        de
                    };
                }
                idsToSort.Add(de.SkillId);
            }
            foreach (long buffID in idsToSort)
            {
                _damageDataById[buffID] = _damageDataById[buffID].OrderBy(x => x.Time).ToList();
            }
            foreach (AgentItem a in dstToSort)
            {
                _damageTakenData[a] = _damageTakenData[a].OrderBy(x => x.Time).ToList();
            }
            foreach (AgentItem a in srcToSort)
            {
                _damageData[a] = _damageData[a].OrderBy(x => x.Time).ToList();
            }
        }

        private IReadOnlyList<InstantCastEvent> ComputeInstantCastEventsFromFinders(AgentData agentData, SkillData skillData, IReadOnlyList<InstantCastFinder> instantCastFinders)
        {
            var res = new List<InstantCastEvent>();
            foreach (InstantCastFinder icf in instantCastFinders)
            {
                if (icf.Available(this))
                {
                    if (icf.NotAccurate)
                    {
                        skillData.NotAccurate.Add(icf.SkillID);
                    }
                    switch (icf.CastOrigin)
                    {
                        case InstantCastFinder.InstantCastOrigin.Trait:
                            skillData.TraitProc.Add(icf.SkillID);
                            break;
                        case InstantCastFinder.InstantCastOrigin.Gear:
                            skillData.GearProc.Add(icf.SkillID);
                            break;
                        case InstantCastFinder.InstantCastOrigin.Skill:
                        default:
                            break;
                    }
                    res.AddRange(icf.ComputeInstantCast(this, skillData, agentData));
                }
            }
            return res;
        }
        private void EICastParse(IReadOnlyList<Player> players, SkillData skillData, FightData fightData, AgentData agentData)
        {
            List<AbstractCastEvent> toAdd = fightData.Logic.SpecialCastEventProcess(this, skillData);
            ulong gw2Build = GetGW2BuildEvent().Build;
            foreach (Player p in players)
            {
                switch (p.Spec)
                {
                    case Spec.Willbender:
                        toAdd.AddRange(ProfHelper.ComputeEndWithBuffApplyCastEvents(p, this, skillData, FlowingResolveSkill, 440, 500, FlowingResolveBuff));
                        break;
                    default:
                        break;
                }
                switch (p.BaseSpec)
                {
                    case Spec.Necromancer:
                        if (gw2Build < GW2Builds.March2024BalanceAndCerusLegendary)
                        {
                            toAdd.AddRange(ProfHelper.ComputeEndWithBuffApplyCastEvents(p, this, skillData, PathOfGluttony, 750, 750, PathOfGluttonyFlipBuff));
                        }
                        break;
                    case Spec.Ranger:
                        toAdd.AddRange(ProfHelper.ComputeUnderBuffCastEvents(p, this, skillData, AncestralGraceSkill, AncestralGraceBuff));
                        break;
                    case Spec.Elementalist:
                        toAdd.AddRange(ProfHelper.ComputeEffectCastEvents(p, this, skillData, Updraft, EffectGUIDs.ElementalistUpdraft2, 0, 1000));
                        toAdd.AddRange(ProfHelper.ComputeUnderBuffCastEvents(p, this, skillData, RideTheLightningSkill, RideTheLightningBuff));
                        break;
                    case Spec.Engineer:
                        toAdd.AddRange(ProfHelper.ComputeEffectCastEvents(p, this, skillData, Devastator, EffectGUIDs.EngineerSpearDevastator1, -1000, 1000));
                        toAdd.AddRange(ProfHelper.ComputeUnderBuffCastEvents(p, this, skillData, ConduitSurge, ConduitSurgeBuff));
                        break;
                    case Spec.Revenant:
                        toAdd.AddRange(ProfHelper.ComputeEffectCastEvents(p, this, skillData, AbyssalBlitz, EffectGUIDs.RevenantSpearAbyssalBlitz1, 0, 3000, 
                            (abyssalBlitz, effect, combatData, skllData) =>
                            {
                                return abyssalBlitz.Where(x => x.Time < effect.Time && Math.Abs(x.Time - effect.Time) < 300).Count() == 0;
                            }));
                        break;
                    default:
                        break;
                }
            }
            // Generic instant cast finders
            var instantCastsFinder = new HashSet<InstantCastFinder>(ProfHelper.GetProfessionInstantCastFinders(players));
            fightData.Logic.GetInstantCastFinders().ForEach(x => instantCastsFinder.Add(x));
            toAdd.AddRange(ComputeInstantCastEventsFromFinders(agentData, skillData, instantCastsFinder.ToList()));
            //
            var castIDsToSort = new HashSet<long>();
            var castAgentsToSort = new HashSet<AgentItem>();
            var wepSwapAgentsToSort = new HashSet<AgentItem>();
            var instantAgentsToSort = new HashSet<AgentItem>();
            var instantIDsToSort = new HashSet<long>();
            foreach (AbstractCastEvent cast in toAdd)
            {
                if (cast is AnimatedCastEvent ace)
                {
                    if (_animatedCastData.TryGetValue(ace.Caster, out List<AnimatedCastEvent> animatedCastList))
                    {
                        animatedCastList.Add(ace);
                    }
                    else
                    {
                        _animatedCastData[ace.Caster] = new List<AnimatedCastEvent>()
                        {
                            ace
                        };
                    }
                    castAgentsToSort.Add(ace.Caster);
                    if (_animatedCastDataById.TryGetValue(ace.SkillId, out List<AnimatedCastEvent> animatedCastByIDList))
                    {
                        animatedCastByIDList.Add(ace);
                    }
                    else
                    {
                        _animatedCastDataById[ace.SkillId] = new List<AnimatedCastEvent>()
                        {
                            ace
                        };
                    }
                    castIDsToSort.Add(ace.SkillId);
                }
                if (cast is WeaponSwapEvent wse)
                {
                    if (_weaponSwapData.TryGetValue(wse.Caster, out List<WeaponSwapEvent> weaponSwapList))
                    {
                        weaponSwapList.Add(wse);
                    }
                    else
                    {
                        _weaponSwapData[wse.Caster] = new List<WeaponSwapEvent>()
                        {
                            wse
                        };
                    }
                    wepSwapAgentsToSort.Add(wse.Caster);
                }
                if (cast is InstantCastEvent ice)
                {

                    if (_instantCastData.TryGetValue(ice.Caster, out List<InstantCastEvent> instantCastList))
                    {
                        instantCastList.Add(ice);
                    }
                    else
                    {
                        _instantCastData[ice.Caster] = new List<InstantCastEvent>()
                        {
                            ice
                        };
                    }
                    instantAgentsToSort.Add(ice.Caster);
                    if (_instantCastDataById.TryGetValue(ice.SkillId, out List<InstantCastEvent> instantCastListByID))
                    {
                        instantCastListByID.Add(ice);
                    }
                    else
                    {
                        _instantCastDataById[ice.SkillId] = new List<InstantCastEvent>()
                        {
                            ice
                        };
                    }
                    instantIDsToSort.Add(ice.SkillId);
                }
            }
            //
            foreach (long castID in castIDsToSort)
            {
                _animatedCastDataById[castID] = _animatedCastDataById[castID].OrderBy(x => x.Time).ToList();
            }
            foreach (AgentItem a in castAgentsToSort)
            {
                _animatedCastData[a] = _animatedCastData[a].OrderBy(x => x.Time).ToList();
            }
            foreach (AgentItem a in wepSwapAgentsToSort)
            {
                _weaponSwapData[a] = _weaponSwapData[a].OrderBy(x => x.Time).ToList();
            }
            foreach (AgentItem a in instantAgentsToSort)
            {
                _instantCastData[a] = _instantCastData[a].OrderBy(x => x.Time).ToList();
            }
            foreach (long instantID in instantIDsToSort)
            {
                _instantCastDataById[instantID] = _instantCastDataById[instantID].OrderBy(x => x.Time).ToList();
            }
        }

        private void EIMetaAndStatusParse(FightData fightData, EvtcVersionEvent evtcVersion)
        {
            foreach (KeyValuePair<AgentItem, List<AbstractHealthDamageEvent>> pair in _damageTakenData)
            {
                if (pair.Key.IsSpecies(TargetID.WorldVersusWorld))
                {
                    continue;
                }
                bool setDeads = false;
                if (!_statusEvents.DeadEvents.TryGetValue(pair.Key, out List<DeadEvent> agentDeaths))
                {
                    agentDeaths = new List<DeadEvent>();
                    setDeads = true;
                }
                bool setDowns = false;
                if (!_statusEvents.DownEvents.TryGetValue(pair.Key, out List<DownEvent> agentDowns))
                {
                    agentDowns = new List<DownEvent>();
                    setDowns = true;
                }
                foreach (AbstractHealthDamageEvent evt in pair.Value)
                {
                    if (evt.HasKilled)
                    {
                        if (!agentDeaths.Exists(x => Math.Abs(x.Time - evt.Time) < 500))
                        {
                            agentDeaths.Add(new DeadEvent(pair.Key, evt.Time));
                        }
                    }
                    if (evt.HasDowned)
                    {
                        if (!agentDowns.Exists(x => Math.Abs(x.Time - evt.Time) < 500))
                        {
                            agentDowns.Add(new DownEvent(pair.Key, evt.Time));
                        }
                    }
                }
                if (setDeads && agentDeaths.Count > 0)
                {
                    _statusEvents.DeadEvents[pair.Key] = agentDeaths.OrderBy(x => x.Time).ToList();
                }
                if (setDowns && agentDowns.Count > 0)
                {
                    _statusEvents.DownEvents[pair.Key] = agentDowns.OrderBy(x => x.Time).ToList();
                }
            }
            _metaDataEvents.ErrorEvents.AddRange(fightData.Logic.GetCustomWarningMessages(fightData, evtcVersion));
        }

        private void EIExtraEventProcess(IReadOnlyList<Player> players, SkillData skillData, AgentData agentData, FightData fightData, ParserController operation, EvtcVersionEvent evtcVersion)
        {
            // Add missing breakbar active state
            foreach (KeyValuePair<AgentItem, List<BreakbarStateEvent>> pair in _statusEvents.BreakbarStateEvents)
            {
                BreakbarStateEvent first = pair.Value.FirstOrDefault();
                if (first != null && first.State != BreakbarState.Active && first.Time > pair.Key.FirstAware + 500)
                {
                    pair.Value.Insert(0, new BreakbarStateEvent(pair.Key, pair.Key.FirstAware, BreakbarState.Active));
                }
            }
            // master attachements
            operation.UpdateProgressWithCancellationCheck("Parsing: Processing Warrior Gadgets");
            WarriorHelper.ProcessGadgets(players, this);
            operation.UpdateProgressWithCancellationCheck("Parsing: Processing Engineer Gadgets");
            EngineerHelper.ProcessGadgets(players, this);
            operation.UpdateProgressWithCancellationCheck("Parsing: Attaching Ranger Gadgets");
            RangerHelper.ProcessGadgets(players, this);
            operation.UpdateProgressWithCancellationCheck("Parsing: Processing Revenant Gadgets");
            RevenantHelper.ProcessGadgets(players, this, agentData);
            operation.UpdateProgressWithCancellationCheck("Parsing: Processing Racial Gadget");
            ProfHelper.ProcessRacialGadgets(players, this);
            // Custom events
            operation.UpdateProgressWithCancellationCheck("Parsing: Creating Custom Buff Events");
            EIBuffParse(players, skillData, fightData, evtcVersion);
            operation.UpdateProgressWithCancellationCheck("Parsing: Creating Custom Damage Events");
            EIDamageParse(skillData, fightData);
            operation.UpdateProgressWithCancellationCheck("Parsing: Creating Custom Cast Events");
            EICastParse(players, skillData, fightData, agentData);
            operation.UpdateProgressWithCancellationCheck("Parsing: Creating Custom Status Events");
            EIMetaAndStatusParse(fightData, evtcVersion);
        }

        private void OffsetBuffExtensionEvents(EvtcVersionEvent evtcVersion)
        {
            if (evtcVersion.Build <= ArcDPSBuilds.BuffExtensionBroken)
            {
                return;
            }
            foreach (KeyValuePair<AgentItem, List<AbstractBuffEvent>> pair in _buffDataByDst)
            {
                var dictApply = pair.Value.OfType<BuffApplyEvent>().GroupBy(x => x.BuffInstance).ToDictionary(x => x.Key, x => x.GroupBy(y => y.BuffID).ToDictionary(y => y.Key, y => y.ToList()));
                var dictStacks = pair.Value.OfType<AbstractBuffStackEvent>().GroupBy(x => x.BuffInstance).ToDictionary(x => x.Key, x => x.GroupBy(y => y.BuffID).ToDictionary(y => y.Key, y => y.ToList()));
                var dictExtensions = pair.Value.OfType<BuffExtensionEvent>().GroupBy(x => x.BuffInstance).ToDictionary(x => x.Key, x => x.GroupBy(y => y.BuffID).ToDictionary(y => y.Key, y => y.ToList()));
                var extensions = pair.Value.OfType<BuffExtensionEvent>().ToList();
                foreach (KeyValuePair<uint, Dictionary<long, List<BuffExtensionEvent>>> extensionPair in dictExtensions)
                {
                    if (extensionPair.Key == 0)
                    {
                        continue;
                    }
                    if (dictApply.TryGetValue(extensionPair.Key, out Dictionary<long, List<BuffApplyEvent>> appliesPerBuffID))
                    {
                        foreach (KeyValuePair<long, List<BuffExtensionEvent>> extensionByBuffIDPair in extensionPair.Value)
                        {
                            if (appliesPerBuffID.TryGetValue(extensionByBuffIDPair.Key, out List<BuffApplyEvent> applies))
                            {
                                BuffExtensionEvent previousExtension = null;
                                foreach (BuffExtensionEvent extensionEvent in extensionByBuffIDPair.Value)
                                {
                                    BuffApplyEvent initialStackApplication = applies.LastOrDefault(x => x.Time <= extensionEvent.Time);
                                    if (initialStackApplication != null)
                                    {
                                        var sequence = new List<AbstractBuffEvent>() { initialStackApplication };
                                        if (dictStacks.TryGetValue(extensionEvent.BuffInstance, out Dictionary<long, List<AbstractBuffStackEvent>> stacksPerBuffID))
                                        {
                                            if (stacksPerBuffID.TryGetValue(extensionEvent.BuffID, out List<AbstractBuffStackEvent> stacks))
                                            {
                                                sequence.AddRange(stacks.Where(x => x.Time >= initialStackApplication.Time && x.Time <= extensionEvent.Time));
                                            }
                                        }
                                        if (previousExtension != null && previousExtension.Time >= initialStackApplication.Time)
                                        {
                                            sequence.Add(previousExtension);
                                        }
                                        previousExtension = extensionEvent;
                                        sequence = sequence.OrderBy(x => x.Time).ToList();
                                        extensionEvent.OffsetNewDuration(sequence, evtcVersion);
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }

        internal CombatData(IReadOnlyList<CombatItem> allCombatItems, FightData fightData, AgentData agentData, SkillData skillData, IReadOnlyList<Player> players, ParserController operation, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions, EvtcVersionEvent evtcVersion)
        {
            _metaDataEvents.EvtcVersionEvent = evtcVersion;
            var combatEvents = allCombatItems.OrderBy(x => x.Time).ToList();
            _skillIds = new HashSet<long>();
            var castCombatEvents = new Dictionary<ulong, List<CombatItem>>();
            var buffEvents = new List<AbstractBuffEvent>();
            var wepSwaps = new List<WeaponSwapEvent>();
            var brkDamageData = new List<AbstractBreakbarDamageEvent>();
            var crowdControlData = new List<CrowdControlEvent>();
            var damageData = new List<AbstractHealthDamageEvent>();
            operation.UpdateProgressWithCancellationCheck("Parsing: Creating EI Combat Data");
            foreach (CombatItem combatItem in combatEvents)
            {
                bool insertToSkillIDs = false;
                if (combatItem.IsStateChange != StateChange.None)
                {
                    if (combatItem.IsExtension)
                    {
                        if (extensions.TryGetValue(combatItem.Pad, out AbstractExtensionHandler handler))
                        {
                            insertToSkillIDs = handler.IsSkillID(combatItem);
                            handler.InsertEIExtensionEvent(combatItem, agentData, skillData);
                        }
                    }
                    else
                    {
                        insertToSkillIDs = combatItem.IsStateChange == StateChange.BuffInitial;
                        CombatEventFactory.AddStateChangeEvent(combatItem, agentData, skillData, _metaDataEvents, _statusEvents, _rewardEvents, wepSwaps, buffEvents, evtcVersion);
                    }

                }
                else if (combatItem.IsActivation != Activation.None)
                {
                    insertToSkillIDs = true;
                    if (castCombatEvents.TryGetValue(combatItem.SrcAgent, out List<CombatItem> list))
                    {
                        list.Add(combatItem);
                    }
                    else
                    {
                        castCombatEvents[combatItem.SrcAgent] = new List<CombatItem>() { combatItem };
                    }
                }
                else if (combatItem.IsBuffRemove != BuffRemove.None)
                {
                    insertToSkillIDs = true;
                    CombatEventFactory.AddBuffRemoveEvent(combatItem, buffEvents, agentData, skillData);
                }
                else
                {
                    insertToSkillIDs = true;
                    if (combatItem.IsBuff != 0 && combatItem.BuffDmg == 0 && combatItem.Value > 0)
                    {
                        CombatEventFactory.AddBuffApplyEvent(combatItem, buffEvents, agentData, skillData, evtcVersion);
                    }
                    else if (combatItem.IsBuff == 0)
                    {
                        CombatEventFactory.AddDirectDamageEvent(combatItem, damageData, brkDamageData, crowdControlData, agentData, skillData);
                    }
                    else if (combatItem.IsBuff != 0 && combatItem.Value == 0)
                    {
                        CombatEventFactory.AddIndirectDamageEvent(combatItem, damageData, brkDamageData, agentData, skillData);
                    }
                }
                if (insertToSkillIDs)
                {
                    _skillIds.Add(combatItem.SkillID);
                }
            }
            HasStackIDs = evtcVersion.Build > ArcDPSBuilds.ProperConfusionDamageSimulation && buffEvents.Any(x => x is BuffStackActiveEvent || x is BuffStackResetEvent);
            UseBuffInstanceSimulator = false;// evtcVersion.Build > ArcDPSBuilds.RemovedDurationForInfiniteDurationStacksChanged && HasStackIDs && (fightData.Logic.ParseMode == EncounterLogic.FightLogic.ParseModeEnum.Instanced10 || fightData.Logic.ParseMode == EncounterLogic.FightLogic.ParseModeEnum.Instanced5 || fightData.Logic.ParseMode == EncounterLogic.FightLogic.ParseModeEnum.Benchmark);
            HasMovementData = _statusEvents.MovementEvents.Count > 1;
            HasBreakbarDamageData = brkDamageData.Count != 0;
            HasEffectData = _statusEvents.EffectEvents.Count != 0;
            //
            operation.UpdateProgressWithCancellationCheck("Parsing: Combining SkillInfo with SkillData");
            skillData.CombineWithSkillInfo(_metaDataEvents.SkillInfoEvents);
            //
            operation.UpdateProgressWithCancellationCheck("Parsing: Creating Cast Events");
            List<AnimatedCastEvent> animatedCastData = CombatEventFactory.CreateCastEvents(castCombatEvents, agentData, skillData, fightData);
            _weaponSwapData = wepSwaps.GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList());
            _animatedCastData = animatedCastData.GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList());
            _instantCastData = new Dictionary<AgentItem, List<InstantCastEvent>>();
            _instantCastDataById = new Dictionary<long, List<InstantCastEvent>>();
            _animatedCastDataById = animatedCastData.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            //
            operation.UpdateProgressWithCancellationCheck("Parsing: Creating Buff Events");
            _buffDataByDst = buffEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            _buffData = buffEvents.GroupBy(x => x.BuffID).ToDictionary(x => x.Key, x => x.ToList());
            OffsetBuffExtensionEvents(evtcVersion);
            // damage events
            operation.UpdateProgressWithCancellationCheck("Parsing: Creating Damage Events");
            _damageData = damageData.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            _damageTakenData = damageData.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            _damageDataById = damageData.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            _breakbarDamageData = brkDamageData.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            _breakbarDamageDataById = brkDamageData.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            _breakbarDamageTakenData = brkDamageData.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            _crowControlData = crowdControlData.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            _crowControlDataById = crowdControlData.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            _crowControlTakenData = crowdControlData.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            BuildBuffDependentContainers();
            //
            foreach (AbstractExtensionHandler handler in extensions.Values)
            {
                handler.AttachToCombatData(this, operation, GetGW2BuildEvent().Build);
            }
            EIExtraEventProcess(players, skillData, agentData, fightData, operation, evtcVersion);
        }

        private void BuildBuffDependentContainers()
        {
            _buffRemoveAllData = _buffData.ToDictionary(x => x.Key, x => x.Value.OfType<BuffRemoveAllEvent>().ToList());
            _buffDataByIDByDst = _buffData.ToDictionary(x => x.Key, x => x.Value.GroupBy(y => y.To).ToDictionary(y => y.Key, y => y.ToList()));
            _buffDataByInstanceID = new Dictionary<long, Dictionary<uint, List<AbstractBuffEvent>>>();
            foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in _buffData)
            {
                foreach (AbstractBuffEvent abe in pair.Value)
                {
                    if (!_buffDataByInstanceID.TryGetValue(abe.BuffID, out Dictionary<uint, List<AbstractBuffEvent>> dict))
                    {
                        dict = new Dictionary<uint, List<AbstractBuffEvent>>();
                        _buffDataByInstanceID[abe.BuffID] = dict;
                    }
                    uint buffInstance = 0;
                    if (abe is AbstractBuffApplyEvent abae)
                    {
                        buffInstance = abae.BuffInstance;
                    }
                    else if (abe is AbstractBuffStackEvent abse)
                    {
                        buffInstance = abse.BuffInstance;
                    }
                    else if (abe is BuffRemoveSingleEvent brse)
                    {
                        buffInstance = brse.BuffInstance;
                    }
                    if (buffInstance > 0)
                    {
                        if (dict.TryGetValue(buffInstance, out List<AbstractBuffEvent> list))
                        {
                            list.Add(abe);
                        }
                        else
                        {
                            dict[buffInstance] = new List<AbstractBuffEvent> { abe };
                        }
                    }
                }
            }
        }

        // getters

        public IReadOnlyCollection<long> GetSkills()
        {
            return _skillIds;
        }

        public IReadOnlyList<AliveEvent> GetAliveEvents(AgentItem src)
        {
            if (_statusEvents.AliveEvents.TryGetValue(src, out List<AliveEvent> list))
            {
                return list;
            }
            return new List<AliveEvent>();
        }

        public IReadOnlyList<AttackTargetEvent> GetAttackTargetEvents(AgentItem src)
        {
            if (_statusEvents.AttackTargetEvents.TryGetValue(src, out List<AttackTargetEvent> list))
            {
                return list;
            }
            return new List<AttackTargetEvent>();
        }

        public IReadOnlyList<AttackTargetEvent> GetAttackTargetEventsByAttackTarget(AgentItem attackTarget)
        {
            if (_statusEvents.AttackTargetEventsByAttackTarget.TryGetValue(attackTarget, out List<AttackTargetEvent> list))
            {
                return list;
            }
            return new List<AttackTargetEvent>();
        }

        public IReadOnlyList<DeadEvent> GetDeadEvents(AgentItem src)
        {
            if (_statusEvents.DeadEvents.TryGetValue(src, out List<DeadEvent> list))
            {
                return list;
            }
            return new List<DeadEvent>();
        }

        public IReadOnlyList<DespawnEvent> GetDespawnEvents(AgentItem src)
        {
            if (_statusEvents.DespawnEvents.TryGetValue(src, out List<DespawnEvent> list))
            {
                return list;
            }
            return new List<DespawnEvent>();
        }

        public IReadOnlyList<DownEvent> GetDownEvents(AgentItem src)
        {
            if (_statusEvents.DownEvents.TryGetValue(src, out List<DownEvent> list))
            {
                return list;
            }
            return new List<DownEvent>();
        }

        public IReadOnlyList<EnterCombatEvent> GetEnterCombatEvents(AgentItem src)
        {
            if (_statusEvents.EnterCombatEvents.TryGetValue(src, out List<EnterCombatEvent> list))
            {
                return list;
            }
            return new List<EnterCombatEvent>();
        }

        public IReadOnlyList<ExitCombatEvent> GetExitCombatEvents(AgentItem src)
        {
            if (_statusEvents.ExitCombatEvents.TryGetValue(src, out List<ExitCombatEvent> list))
            {
                return list;
            }
            return new List<ExitCombatEvent>();
        }

        public IReadOnlyList<GuildEvent> GetGuildEvents(AgentItem src)
        {
            if (_metaDataEvents.GuildEvents.TryGetValue(src, out List<GuildEvent> list))
            {
                return list;
            }
            return new List<GuildEvent>();
        }

        public IReadOnlyList<HealthUpdateEvent> GetHealthUpdateEvents(AgentItem src)
        {
            if (_statusEvents.HealthUpdateEvents.TryGetValue(src, out List<HealthUpdateEvent> list))
            {
                return list;
            }
            return new List<HealthUpdateEvent>();
        }

        public IReadOnlyList<BarrierUpdateEvent> GetBarrierUpdateEvents(AgentItem src)
        {
            if (_statusEvents.BarrierUpdateEvents.TryGetValue(src, out List<BarrierUpdateEvent> list))
            {
                return list;
            }
            return new List<BarrierUpdateEvent>();
        }

        public IReadOnlyList<MaxHealthUpdateEvent> GetMaxHealthUpdateEvents(AgentItem src)
        {
            if (_statusEvents.MaxHealthUpdateEvents.TryGetValue(src, out List<MaxHealthUpdateEvent> list))
            {
                return list;
            }
            return new List<MaxHealthUpdateEvent>();
        }

        public PointOfViewEvent GetPointOfViewEvent()
        {
            return _metaDataEvents.PointOfViewEvent;
        }

        public EvtcVersionEvent GetEvtcVersionEvent()
        {
            return _metaDataEvents.EvtcVersionEvent;
        }

        public FractalScaleEvent GetFractalScaleEvent()
        {
            return _metaDataEvents.FractalScaleEvent;
        }

        public IReadOnlyList<SpawnEvent> GetSpawnEvents(AgentItem src)
        {
            if (_statusEvents.SpawnEvents.TryGetValue(src, out List<SpawnEvent> list))
            {
                return list;
            }
            return new List<SpawnEvent>();
        }

        public IReadOnlyList<TargetableEvent> GetTargetableEvents(AgentItem attackTarget)
        {
            if (_statusEvents.TargetableEvents.TryGetValue(attackTarget, out List<TargetableEvent> list))
            {
                return list;
            }
            return new List<TargetableEvent>();
        }
        /// <summary>
        /// Returns squad marker events of given marker index
        /// </summary>
        /// <param name="markerIndex">marker index</param>
        /// <returns></returns>
        public IReadOnlyList<SquadMarkerEvent> GetSquadMarkerEvents(SquadMarkerIndex markerIndex)
        {
            if (_statusEvents.SquadMarkerEventsByIndex.TryGetValue(markerIndex, out List<SquadMarkerEvent> list))
            {
                return list;
            }
            return new List<SquadMarkerEvent>();
        }
        /// <summary>
        /// Returns marker events owned by agent
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        public IReadOnlyList<MarkerEvent> GetMarkerEvents(AgentItem agent)
        {
            if (_statusEvents.MarkerEvents.TryGetValue(agent, out List<MarkerEvent> list))
            {
                return list;
            }
            return new List<MarkerEvent>();
        }
        /// <summary>
        /// Returns marker events of given marker ID
        /// </summary>
        /// <param name="markerID">marker ID</param>
        /// <returns></returns>
        public IReadOnlyList<MarkerEvent> GetMarkerEvents(long markerID)
        {
            if (_statusEvents.MarkerEventsByID.TryGetValue(markerID, out List<MarkerEvent> list))
            {
                return list;
            }
            return new List<MarkerEvent>();
        }
        /// <summary>
        /// True if marker events of given marker GUID has been found
        /// </summary>
        /// <param name="markerGUID">marker GUID</param>
        /// <param name="markerEvents">Found marker events</param>
        /// <returns></returns>
        public bool TryGetMarkerEventsByGUID(string markerGUID, out IReadOnlyList<MarkerEvent> markerEvents)
        {
            MarkerGUIDEvent markerGUIDEvent = GetMarkerGUIDEvent(markerGUID);
            markerEvents = null;
            if (markerGUIDEvent != null)
            {
                markerEvents = GetMarkerEvents(markerGUIDEvent.ContentID);
                return true;
            }
            return false;
        }
        /// <summary>
        /// True if marker events of given marker GUID has been found on given agent
        /// </summary>
        /// <param name="agent">marker owner</param>
        /// <param name="markerGUID">marker GUID</param>
        /// <param name="markerEvents">Found marker events</param>
        /// <returns></returns>
        public bool TryGetMarkerEventsBySrcWithGUID(AgentItem agent, string markerGUID, out IReadOnlyList<MarkerEvent> markerEvents)
        {
            markerEvents = null;
            if (TryGetMarkerEventsByGUID(markerGUID, out IReadOnlyList<MarkerEvent> markers))
            {
                markerEvents = markers.Where(effect => effect.Src == agent).ToList();
                return true;
            }
            return false;
        }

        public IReadOnlyList<TeamChangeEvent> GetTeamChangeEvents(AgentItem src)
        {
            if (_statusEvents.TeamChangeEvents.TryGetValue(src, out List<TeamChangeEvent> list))
            {
                return list;
            }
            return new List<TeamChangeEvent>();
        }

        public IReadOnlyList<BreakbarStateEvent> GetBreakbarStateEvents(AgentItem src)
        {
            if (_statusEvents.BreakbarStateEvents.TryGetValue(src, out List<BreakbarStateEvent> list))
            {
                return list;
            }
            return new List<BreakbarStateEvent>();
        }

        public IReadOnlyList<BreakbarPercentEvent> GetBreakbarPercentEvents(AgentItem src)
        {
            if (_statusEvents.BreakbarPercentEvents.TryGetValue(src, out List<BreakbarPercentEvent> list))
            {
                return list;
            }
            return new List<BreakbarPercentEvent>();
        }

        public GW2BuildEvent GetGW2BuildEvent()
        {
            return _metaDataEvents.GW2BuildEvent;
        }

        public LanguageEvent GetLanguageEvent()
        {
            return _metaDataEvents.LanguageEvent;
        }

        public InstanceStartEvent GetInstanceStartEvent()
        {
            return _metaDataEvents.InstanceStartEvent;
        }

        public LogStartEvent GetLogStartEvent()
        {
            return _metaDataEvents.LogStartEvent;
        }

        public IReadOnlyList<LogNPCUpdateEvent> GetLogNPCUpdateEvents()
        {
            return _metaDataEvents.LogNPCUpdateEvents;
        }

        public LogEndEvent GetLogEndEvent()
        {
            return _metaDataEvents.LogEndEvent;
        }

        public IReadOnlyList<MapIDEvent> GetMapIDEvents()
        {
            return _metaDataEvents.MapIDEvents;
        }

        public IReadOnlyList<RewardEvent> GetRewardEvents()
        {
            return _rewardEvents;
        }

        public IReadOnlyList<ErrorEvent> GetErrorEvents()
        {
            return _metaDataEvents.ErrorEvents;
        }

        public IReadOnlyList<ShardEvent> GetShardEvents()
        {
            return _metaDataEvents.ShardEvents;
        }

        public IReadOnlyList<TickRateEvent> GetTickRateEvents()
        {
            return _metaDataEvents.TickRateEvents;
        }

        public BuffInfoEvent GetBuffInfoEvent(long buffID)
        {
            if (_metaDataEvents.BuffInfoEvents.TryGetValue(buffID, out BuffInfoEvent evt))
            {
                return evt;
            }
            return null;
        }

        public IReadOnlyList<BuffInfoEvent> GetBuffInfoEvent(byte category)
        {
            if (_metaDataEvents.BuffInfoEventsByCategory.TryGetValue(category, out List<BuffInfoEvent> evts))
            {
                return evts;
            }
            return new List<BuffInfoEvent>();
        }

        public SkillInfoEvent GetSkillInfoEvent(long skillID)
        {
            if (_metaDataEvents.SkillInfoEvents.TryGetValue(skillID, out SkillInfoEvent evt))
            {
                return evt;
            }
            return null;
        }

        public IReadOnlyList<Last90BeforeDownEvent> GetLast90BeforeDownEvents()
        {
            return _statusEvents.Last90BeforeDownEvents;
        }

        public IReadOnlyList<Last90BeforeDownEvent> GetLast90BeforeDownEvents(AgentItem src)
        {
            if (_statusEvents.Last90BeforeDownEventsBySrc.TryGetValue(src, out List<Last90BeforeDownEvent> res))
            {
                return res;
            }
            return new List<Last90BeforeDownEvent>();
        }

        public IReadOnlyList<AbstractBuffEvent> GetBuffData(long buffID)
        {
            if (_buffData.TryGetValue(buffID, out List<AbstractBuffEvent> res))
            {
                return res;
            }
            return new List<AbstractBuffEvent>();
        }

        /// <summary>
        /// Returns list of buff events applied on agent for given id
        /// </summary>
        /// <param name="buffID"></param> buff id
        /// <param name="dst"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AbstractBuffEvent> GetBuffDataByIDByDst(long buffID, AgentItem dst)
        {
            if (_buffDataByIDByDst.TryGetValue(buffID, out Dictionary<AgentItem, List<AbstractBuffEvent>> agentDict))
            {
                if (agentDict.TryGetValue(dst, out List<AbstractBuffEvent> res))
                {
                    return res;
                }
            }
            return new List<AbstractBuffEvent>();
        }

        public IReadOnlyList<AbstractBuffEvent> GetBuffDataByInstanceID(long buffID, uint instanceID)
        {
            if (instanceID == 0)
            {
                return GetBuffData(buffID);
            }
            if (_buffDataByInstanceID.TryGetValue(buffID, out Dictionary<uint, List<AbstractBuffEvent>> dict))
            {
                if (dict.TryGetValue(instanceID, out List<AbstractBuffEvent> list))
                {
                    return list;
                }
            }
            return new List<AbstractBuffEvent>();
        }

        public IReadOnlyList<BuffRemoveAllEvent> GetBuffRemoveAllData(long buffID)
        {
            if (_buffRemoveAllData.TryGetValue(buffID, out List<BuffRemoveAllEvent> res))
            {
                return res;
            }
            return new List<BuffRemoveAllEvent>();
        }

        /// <summary>
        /// Returns list of buff events applied on agent
        /// </summary>
        /// <param name="dst"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AbstractBuffEvent> GetBuffDataByDst(AgentItem dst)
        {
            if (_buffDataByDst.TryGetValue(dst, out List<AbstractBuffEvent> res))
            {
                return res;
            }
            return new List<AbstractBuffEvent>();
        }

        /// <summary>
        /// Returns list of damage events done by agent
        /// </summary>
        /// <param name="src"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AbstractHealthDamageEvent> GetDamageData(AgentItem src)
        {
            if (_damageData.TryGetValue(src, out List<AbstractHealthDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractHealthDamageEvent>();
        }

        /// <summary>
        /// Returns list of breakbar damage events done by agent
        /// </summary>
        /// <param name="src"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageData(AgentItem src)
        {
            if (_breakbarDamageData.TryGetValue(src, out List<AbstractBreakbarDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractBreakbarDamageEvent>();
        }

        /// <summary>
        /// Returns list of breakbar damage events done by skill id
        /// </summary>
        /// <param name="long"></param> ID
        /// <returns></returns>
        public IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageData(long skillID)
        {
            if (_breakbarDamageDataById.TryGetValue(skillID, out List<AbstractBreakbarDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractBreakbarDamageEvent>();
        }

        /// <summary>
        /// Returns list of damage events applied by a skill
        /// </summary>
        /// <param name="skillID"></param> Id of the skill
        /// <returns></returns>
        public IReadOnlyList<AbstractHealthDamageEvent> GetDamageData(long skillID)
        {
            if (_damageDataById.TryGetValue(skillID, out List<AbstractHealthDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractHealthDamageEvent>();
        }

        /// <summary>
        /// Returns list of crowd control events done by agent
        /// </summary>
        /// <param name="src"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<CrowdControlEvent> GetCrowdControlData(AgentItem src)
        {
            if (_crowControlData.TryGetValue(src, out List<CrowdControlEvent> res))
            {
                return res;
            }
            return new List<CrowdControlEvent>();
        }

        /// <summary>
        /// Returns list of crowd control events done by skill id
        /// </summary>
        /// <param name="long"></param> ID
        /// <returns></returns>
        public IReadOnlyList<CrowdControlEvent> GetCrowdControlData(long skillID)
        {
            if (_crowControlDataById.TryGetValue(skillID, out List<CrowdControlEvent> res))
            {
                return res;
            }
            return new List<CrowdControlEvent>();
        }

        /// <summary>
        /// Returns list of animated cast events done by Agent
        /// </summary>
        /// <param name="caster"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AnimatedCastEvent> GetAnimatedCastData(AgentItem caster)
        {
            if (_animatedCastData.TryGetValue(caster, out List<AnimatedCastEvent> res))
            {
                return res;
            }
            return new List<AnimatedCastEvent>();
        }

        /// <summary>
        /// Returns list of instant cast events done by Agent
        /// </summary>
        /// <param name="caster"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<InstantCastEvent> GetInstantCastData(AgentItem caster)
        {
            if (_instantCastData.TryGetValue(caster, out List<InstantCastEvent> res))
            {
                return res;
            }
            return new List<InstantCastEvent>();
        }

        /// <summary>
        /// Returns list of instant cast events done by Agent
        /// </summary>
        /// <param name="caster"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<InstantCastEvent> GetInstantCastData(long skillID)
        {
            if (_instantCastDataById.TryGetValue(skillID, out List<InstantCastEvent> res))
            {
                return res;
            }
            return new List<InstantCastEvent>();
        }

        /// <summary>
        /// Returns list of weapon swap events done by Agent
        /// </summary>
        /// <param name="caster"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<WeaponSwapEvent> GetWeaponSwapData(AgentItem caster)
        {
            if (_weaponSwapData.TryGetValue(caster, out List<WeaponSwapEvent> res))
            {
                return res;
            }
            return new List<WeaponSwapEvent>();
        }

        /// <summary>
        /// Returns list of cast events from skill
        /// </summary>
        /// <param name="skillID"></param> ID of the skill
        /// <returns></returns>
        public IReadOnlyList<AnimatedCastEvent> GetAnimatedCastData(long skillID)
        {
            if (_animatedCastDataById.TryGetValue(skillID, out List<AnimatedCastEvent> res))
            {
                return res;
            }
            return new List<AnimatedCastEvent>();
        }

        /// <summary>
        /// Returns list of damage events taken by Agent
        /// </summary>
        /// <param name="dst"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AbstractHealthDamageEvent> GetDamageTakenData(AgentItem dst)
        {
            if (_damageTakenData.TryGetValue(dst, out List<AbstractHealthDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractHealthDamageEvent>();
        }

        /// <summary>
        /// Returns list of breakbar damage events taken by Agent
        /// </summary>
        /// <param name="dst"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageTakenData(AgentItem dst)
        {
            if (_breakbarDamageTakenData.TryGetValue(dst, out List<AbstractBreakbarDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractBreakbarDamageEvent>();
        }

        /// <summary>
        /// Returns list of crowd control events taken by Agent
        /// </summary>
        /// <param name="dst"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<CrowdControlEvent> GetCrowdControlTakenData(AgentItem dst)
        {
            if (_crowControlTakenData.TryGetValue(dst, out List<CrowdControlEvent> res))
            {
                return res;
            }
            return new List<CrowdControlEvent>();
        }

        public IReadOnlyList<AbstractMovementEvent> GetMovementData(AgentItem src)
        {
            if (_statusEvents.MovementEvents.TryGetValue(src, out List<AbstractMovementEvent> res))
            {
                return res;
            }
            return new List<AbstractMovementEvent>();
        }

        public IReadOnlyList<EffectEvent> GetEffectEventsBySrc(AgentItem src)
        {
            if (_statusEvents.EffectEventsBySrc.TryGetValue(src, out List<EffectEvent> list))
            {
                return list;
            }
            return new List<EffectEvent>();
        }

        public IReadOnlyList<EffectEvent> GetEffectEventsByDst(AgentItem dst)
        {
            if (_statusEvents.EffectEventsByDst.TryGetValue(dst, out List<EffectEvent> list))
            {
                return list;
            }
            return new List<EffectEvent>();
        }

        public IReadOnlyList<EffectEvent> GetEffectEventsByEffectID(long effectID)
        {
            if (_statusEvents.EffectEventsByEffectID.TryGetValue(effectID, out List<EffectEvent> list))
            {
                return list;
            }
            return new List<EffectEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <param name="effectEvents"></param>
        /// <returns></returns>
        public bool TryGetEffectEventsByGUID(string effectGUID, out IReadOnlyList<EffectEvent> effectEvents)
        {
            EffectGUIDEvent effectGUIDEvent = GetEffectGUIDEvent(effectGUID);
            effectEvents = null;
            if (effectGUIDEvent != null)
            {
                IReadOnlyList<EffectEvent> result = GetEffectEventsByEffectID(effectGUIDEvent.ContentID);
                if (result.Count > 0)
                {
                    effectEvents = result;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectGUIDs">Strings in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <param name="effectEvents"></param>
        /// <returns></returns>
        public bool TryGetEffectEventsByGUIDs(string[] effectGUIDs, out IReadOnlyList<EffectEvent> effectEvents)
        {
            effectEvents = null;
            var result = new List<EffectEvent>();
            foreach (string effectGUID in effectGUIDs)
            {
                if (TryGetEffectEventsByGUID(effectGUID, out IReadOnlyList<EffectEvent> effects))
                {
                    result.AddRange(effects);
                }
            }
            if (result.Count > 0)
            {
                effectEvents = result;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns effect events by the given agent and effect GUID.
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="effectGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <param name="effectEvents"></param>
        /// <returns></returns>
        public bool TryGetEffectEventsBySrcWithGUID(AgentItem agent, string effectGUID, out IReadOnlyList<EffectEvent> effectEvents)
        {
            effectEvents = null;
            if (TryGetEffectEventsByGUID(effectGUID, out IReadOnlyList<EffectEvent> effects))
            {
                var result = effects.Where(effect => effect.Src == agent).ToList();
                if (result.Count > 0)
                {
                    effectEvents = result;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Returns effect events on the given agent and effect GUID.
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="effectGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <param name="effectEvents"></param>
        /// <returns></returns>
        public bool TryGetEffectEventsByDstWithGUID(AgentItem agent, string effectGUID, out IReadOnlyList<EffectEvent> effectEvents)
        {
            effectEvents = null;
            if (TryGetEffectEventsByGUID(effectGUID, out IReadOnlyList<EffectEvent> effects))
            {
                var result = effects.Where(effect => effect.Dst == agent).ToList();
                if (result.Count > 0)
                {
                    effectEvents = result;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Returns effect events by the given agent and effect GUIDs.
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="effectGUIDs">Strings in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <param name="effectEvents"></param>
        /// <returns></returns>
        public bool TryGetEffectEventsBySrcWithGUIDs(AgentItem agent, string[] effectGUIDs, out IReadOnlyList<EffectEvent> effectEvents)
        {
            effectEvents = null;
            var result = new List<EffectEvent>();
            foreach (string effectGUID in effectGUIDs)
            {
                if (TryGetEffectEventsBySrcWithGUID(agent, effectGUID, out IReadOnlyList<EffectEvent> effects))
                {
                    result.AddRange(effects);
                }
            }
            if (result.Count > 0)
            {
                effectEvents = result;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Returns effect events on the given agent and effect GUIDs.
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="effectGUIDs">Strings in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <param name="effectEvents"></param>
        /// <returns></returns>
        public bool TryGetEffectEventsByDstWithGUIDs(AgentItem agent, string[] effectGUIDs, out IReadOnlyList<EffectEvent> effectEvents)
        {
            effectEvents = null;
            var result = new List<EffectEvent>();
            foreach (string effectGUID in effectGUIDs)
            {
                if (TryGetEffectEventsByDstWithGUID(agent, effectGUID, out IReadOnlyList<EffectEvent> effects))
                {
                    result.AddRange(effects);
                }
            }
            if (result.Count > 0)
            {
                effectEvents = result;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns effect events by the given agent <b>including</b> minions and the given effect GUID.
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="effectGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <param name="effectEvents"></param>
        /// <returns></returns>
        public bool TryGetEffectEventsByMasterWithGUID(AgentItem agent, string effectGUID, out IReadOnlyList<EffectEvent> effectEvents)
        {
            effectEvents = null;
            if (TryGetEffectEventsByGUID(effectGUID, out IReadOnlyList<EffectEvent> effects))
            {
                var result = effects.Where(effect => effect.Src.GetFinalMaster() == agent).ToList();
                if (result.Count > 0)
                {
                    effectEvents = result;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns effect events by the given agent <b>including</b> minions and the given effect GUIDs.
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="effectGUIDs">Strings in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <param name="effectEvents"></param>
        /// <returns></returns>
        public bool TryGetEffectEventsByMasterWithGUIDs(AgentItem agent, string[] effectGUIDs, out IReadOnlyList<EffectEvent> effectEvents)
        {
            effectEvents = null;
            var result = new List<EffectEvent>();
            foreach (string effectGUID in effectGUIDs)
            {
                if (TryGetEffectEventsByMasterWithGUID(agent, effectGUID, out IReadOnlyList<EffectEvent> effects))
                {
                    result.AddRange(effects);
                }
            }
            if (result.Count > 0)
            {
                effectEvents = result;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns effect events by the given agent and effect GUID.
        /// The same effects happening within epsilon milliseconds are grouped together.
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="effectGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <param name="groupedEffectEvents"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool TryGetGroupedEffectEventsBySrcWithGUID(AgentItem agent, string effectGUID, out IReadOnlyList<IReadOnlyList<EffectEvent>> groupedEffectEvents, long epsilon = ServerDelayConstant)
        {
            var effectGroups = new List<List<EffectEvent>>();
            groupedEffectEvents = null;
            if (TryGetEffectEventsBySrcWithGUID(agent, effectGUID, out IReadOnlyList<EffectEvent> effects))
            {
                var processedTimes = new HashSet<long>();
                foreach (EffectEvent first in effects)
                {
                    if (processedTimes.Contains(first.Time))
                    {
                        continue;
                    }
                    var group = effects.Where(effect => effect.Time >= first.Time && effect.Time < first.Time + epsilon).ToList();
                    foreach (EffectEvent effect in group)
                    {
                        processedTimes.Add(effect.Time);
                    }
                    effectGroups.Add(group);
                }
                groupedEffectEvents = effectGroups;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Returns effect events for the given effect GUID.
        /// The same effects happening within epsilon milliseconds are grouped together.
        /// </summary>
        /// <param name="effectGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <param name="groupedEffectEvents"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool TryGetGroupedEffectEventsByGUID(string effectGUID, out IReadOnlyList<IReadOnlyList<EffectEvent>> groupedEffectEvents, long epsilon = ServerDelayConstant)
        {
            var effectGroups = new List<List<EffectEvent>>();
            groupedEffectEvents = null;
            if (TryGetEffectEventsByGUID(effectGUID, out IReadOnlyList<EffectEvent> effects))
            {
                var processedTimes = new HashSet<long>();
                foreach (EffectEvent first in effects)
                {
                    if (processedTimes.Contains(first.Time))
                    {
                        continue;
                    }
                    var group = effects.Where(effect => effect.Time >= first.Time && effect.Time < first.Time + epsilon).ToList();
                    foreach (EffectEvent effect in group)
                    {
                        processedTimes.Add(effect.Time);
                    }

                    effectGroups.Add(group);
                }
                groupedEffectEvents = effectGroups;
                return true;
            }
            return false;
        }

        public IReadOnlyList<EffectEvent> GetEffectEvents()
        {
            return _statusEvents.EffectEvents;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <returns></returns>
        public EffectGUIDEvent GetEffectGUIDEvent(string effectGUID)
        {
            if (_metaDataEvents.EffectGUIDEventsByGUID.TryGetValue(effectGUID, out EffectGUIDEvent evt))
            {
                return evt;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectID">ID of the effect</param>
        /// <returns></returns>
        public EffectGUIDEvent GetEffectGUIDEvent(long effectID)
        {
            if (_metaDataEvents.EffectGUIDEventsByEffectID.TryGetValue(effectID, out EffectGUIDEvent evt))
            {
                return evt;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="markerGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <returns></returns>
        public MarkerGUIDEvent GetMarkerGUIDEvent(string markerGUID)
        {
            if (_metaDataEvents.MarkerGUIDEventsByGUID.TryGetValue(markerGUID, out MarkerGUIDEvent evt))
            {
                return evt;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="markerID">ID of the marker</param>
        /// <returns></returns>
        public MarkerGUIDEvent GetMarkerGUIDEvent(long markerID)
        {
            if (_metaDataEvents.MarkerGUIDEventsByMarkerID.TryGetValue(markerID, out MarkerGUIDEvent evt))
            {
                return evt;
            }
            return null;
        }

        public IReadOnlyList<GliderEvent> GetGliderEvents(AgentItem src)
        {
            if (_statusEvents.GliderEventsBySrc.TryGetValue(src, out List<GliderEvent> list))
            {
                return list;
            }
            return new List<GliderEvent>();
        }

        public IReadOnlyList<StunBreakEvent> GetStunBreakEvents(AgentItem src)
        {
            if (_statusEvents.StunBreakEventsBySrc.TryGetValue(src, out List<StunBreakEvent> list))
            {
                return list;
            }
            return new List<StunBreakEvent>();
        }

        /// 

        public static IEnumerable<T> FindRelatedEvents<T>(IEnumerable<T> events, long time, long epsilon = ServerDelayConstant) where T : AbstractTimeCombatEvent
        {
            return events.Where(evt => Math.Abs(evt.Time - time) < epsilon);
        }

        public bool HasRelatedHit(long skillID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetDamageData(skillID), time, epsilon)
                .Any(hit => hit.CreditedFrom == agent);
        }

        public bool HasPreviousCast(long skillID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetAnimatedCastData(skillID), time, epsilon)
                .Any(cast => cast.Caster == agent && cast.Time <= time);
        }

        public bool IsCasting(long skillID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return GetAnimatedCastData(skillID)
                .Any(cast => cast.Caster == agent && cast.Time - epsilon <= time && cast.EndTime + epsilon >= time);
        }

        public bool HasGainedBuff(long buffID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffApplyEvent>(), time, epsilon)
                .Any();
        }

        public bool HasGainedBuff(long buffID, AgentItem agent, long time, AgentItem source, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffApplyEvent>(), time, epsilon)
                .Any(apply => apply.CreditedBy == source);
        }

        public bool HasGainedBuff(long buffID, AgentItem agent, long time, long appliedDuration, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffApplyEvent>(), time, epsilon)
                .Any(apply => Math.Abs(apply.AppliedDuration - appliedDuration) < epsilon);
        }

        public bool HasGainedBuff(long buffID, AgentItem agent, long time, long appliedDuration, AgentItem source, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffApplyEvent>(), time, epsilon)
                .Any(apply => apply.CreditedBy == source && Math.Abs(apply.AppliedDuration - appliedDuration) < epsilon);
        }

        public bool HasLostBuff(long buffID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffRemoveAllEvent>(), time, epsilon)
                .Any();
        }

        public bool HasLostBuffStack(long buffID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<AbstractBuffRemoveEvent>(), time, epsilon)
                .Any();
        }

        public bool HasRelatedEffect(string effectGUID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            if (TryGetEffectEventsBySrcWithGUID(agent, effectGUID, out IReadOnlyList<EffectEvent> effectEvents))
            {
                return FindRelatedEvents(effectEvents, time, epsilon).Any();
            }
            return false;
        }

        public bool HasRelatedEffectDst(string effectGUID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            if (TryGetEffectEventsByDstWithGUID(agent, effectGUID, out IReadOnlyList<EffectEvent> effectEvents))
            {
                return FindRelatedEvents(effectEvents, time, epsilon).Any();
            }
            return false;
        }

        public bool HasExtendedBuff(long buffID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffExtensionEvent>(), time, epsilon)
                .Any();
        }

        public bool HasExtendedBuff(long buffID, AgentItem agent, long time, AgentItem source, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffExtensionEvent>(), time, epsilon)
                .Any(apply => apply.CreditedBy == source);
        }

        public bool HasExtendedBuff(long buffID, AgentItem agent, long time, long extendedDuration, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffExtensionEvent>(), time, epsilon)
                .Any(apply => Math.Abs(apply.ExtendedDuration - extendedDuration) < epsilon);
        }

        public bool HasExtendedBuff(long buffID, AgentItem agent, long time, long extendedDuration, AgentItem source, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffExtensionEvent>(), time, epsilon)
                .Any(apply => apply.CreditedBy == source && Math.Abs(apply.ExtendedDuration - extendedDuration) < epsilon);
        }

    }
}
