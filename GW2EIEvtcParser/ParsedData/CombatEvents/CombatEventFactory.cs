using System.Collections.Generic;
using System.IO;
using System.Linq;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData
{
    internal static class CombatEventFactory
    {

        public static void AddStateChangeEvent(CombatItem stateChangeEvent, AgentData agentData, SkillData skillData, MetaEventsContainer metaDataEvents, StatusEventsContainer statusEvents, List<RewardEvent> rewardEvents, List<WeaponSwapEvent> wepSwaps, List<AbstractBuffEvent> buffEvents, int evtcVersion)
        {
            switch (stateChangeEvent.IsStateChange)
            {
                case StateChange.EnterCombat:
                    var enterCombatEvt = new EnterCombatEvent(stateChangeEvent, agentData);
                    Add(statusEvents.EnterCombatEvents, enterCombatEvt.Src, enterCombatEvt);
                    break;
                case StateChange.ExitCombat:
                    var exitCombatEvt = new ExitCombatEvent(stateChangeEvent, agentData);
                    Add(statusEvents.ExitCombatEvents, exitCombatEvt.Src, exitCombatEvt);
                    break;
                case StateChange.ChangeUp:
                    var aliveEvt = new AliveEvent(stateChangeEvent, agentData);
                    Add(statusEvents.AliveEvents, aliveEvt.Src, aliveEvt);
                    break;
                case StateChange.ChangeDead:
                    var deadEvt = new DeadEvent(stateChangeEvent, agentData);
                    Add(statusEvents.DeadEvents, deadEvt.Src, deadEvt);
                    break;
                case StateChange.ChangeDown:
                    var downEvt = new DownEvent(stateChangeEvent, agentData);
                    Add(statusEvents.DownEvents, downEvt.Src, downEvt);
                    break;
                case StateChange.Spawn:
                    var spawnEvt = new SpawnEvent(stateChangeEvent, agentData);
                    Add(statusEvents.SpawnEvents, spawnEvt.Src, spawnEvt);
                    break;
                case StateChange.Despawn:
                    var despawnEvt = new DespawnEvent(stateChangeEvent, agentData);
                    Add(statusEvents.DespawnEvents, despawnEvt.Src, despawnEvt);
                    break;
                case StateChange.HealthUpdate:
                    var healthEvt = new HealthUpdateEvent(stateChangeEvent, agentData);
                    Add(statusEvents.HealthUpdateEvents, healthEvt.Src, healthEvt);
                    break;                
                case StateChange.BarrierUpdate:
                    var barrierEvt = new BarrierUpdateEvent(stateChangeEvent, agentData);
                    Add(statusEvents.BarrierUpdateEvents, barrierEvt.Src, barrierEvt);
                    break;
                case StateChange.InstanceStart:
                    metaDataEvents.InstanceStartEvent = new InstanceStartEvent(stateChangeEvent);
                    break;
                case StateChange.LogStart:
                    if (stateChangeEvent.Value == 0 || stateChangeEvent.BuffDmg == 0)
                    {
                        return;
                    }
                    metaDataEvents.LogStartEvent = new LogStartEvent(stateChangeEvent);
                    break;
                case StateChange.LogStartNPCUpdate:
                    metaDataEvents.LogStartNPCUpdateEvents.Add(new LogStartNPCUpdateEvent(stateChangeEvent, agentData));
                    break;
                case StateChange.LogEnd:
                    if (stateChangeEvent.Value == 0 || stateChangeEvent.BuffDmg == 0)
                    {
                        return;
                    }
                    metaDataEvents.LogEndEvent = new LogEndEvent(stateChangeEvent);
                    break;
                case StateChange.MaxHealthUpdate:
                    var maxHealthEvt = new MaxHealthUpdateEvent(stateChangeEvent, agentData);
                    Add(statusEvents.MaxHealthUpdateEvents, maxHealthEvt.Src, maxHealthEvt);
                    break;
                case StateChange.PointOfView:
                    if (stateChangeEvent.SrcAgent == 0)
                    {
                        return;
                    }
                    metaDataEvents.PointOfViewEvent = new PointOfViewEvent(stateChangeEvent, agentData);
                    break;
                case StateChange.Language:
                    metaDataEvents.LanguageEvent = new LanguageEvent(stateChangeEvent);
                    break;
                case StateChange.GWBuild:
                    if (stateChangeEvent.SrcAgent == 0)
                    {
                        return;
                    }
                    metaDataEvents.BuildEvent = new BuildEvent(stateChangeEvent);
                    break;
                case StateChange.ShardId:
                    metaDataEvents.ShardEvents.Add(new ShardEvent(stateChangeEvent));
                    break;
                case StateChange.Reward:
#if !NO_REWARDS
                    rewardEvents.Add(new RewardEvent(stateChangeEvent));
#endif
                    break;
                case StateChange.TeamChange:
                    var tcEvt = new TeamChangeEvent(stateChangeEvent, agentData);
                    Add(statusEvents.TeamChangeEvents, tcEvt.Src, tcEvt);
                    break;
                case StateChange.AttackTarget:
                    var aTEvt = new AttackTargetEvent(stateChangeEvent, agentData);
                    Add(statusEvents.AttackTargetEvents, aTEvt.Src, aTEvt);
                    Add(statusEvents.AttackTargetEventsByAttackTarget, aTEvt.AttackTarget, aTEvt);
                    break;
                case StateChange.Targetable:
                    var tarEvt = new TargetableEvent(stateChangeEvent, agentData);
                    Add(statusEvents.TargetableEvents, tarEvt.Src, tarEvt);
                    break;
                case StateChange.MapID:
                    metaDataEvents.MapIDEvents.Add(new MapIDEvent(stateChangeEvent));
                    break;
                case StateChange.Guild:
                    var gEvt = new GuildEvent(stateChangeEvent, agentData);
                    Add(metaDataEvents.GuildEvents, gEvt.Src, gEvt);
                    break;
                case StateChange.BuffInfo:
                case StateChange.BuffFormula:
                    if (metaDataEvents.BuffInfoEvents.TryGetValue(stateChangeEvent.SkillID, out BuffInfoEvent buffInfoEvent))
                    {
                        buffInfoEvent.CompleteBuffInfoEvent(stateChangeEvent, evtcVersion);
                    }
                    else
                    {
                        buffInfoEvent = new BuffInfoEvent(stateChangeEvent, evtcVersion);
                        metaDataEvents.BuffInfoEvents[stateChangeEvent.SkillID] = buffInfoEvent;
                    }
                    if (stateChangeEvent.IsStateChange == StateChange.BuffInfo)
                    {
                        if (metaDataEvents.BuffInfoEventsByCategory.TryGetValue(buffInfoEvent.CategoryByte, out List<BuffInfoEvent> bdEvtList))
                        {
                            bdEvtList.Add(buffInfoEvent);
                        }
                        else
                        {
                            metaDataEvents.BuffInfoEventsByCategory[buffInfoEvent.CategoryByte] = new List<BuffInfoEvent> { buffInfoEvent };
                        }
                    }
                    break;
                case StateChange.SkillInfo:
                case StateChange.SkillTiming:
                    if (metaDataEvents.SkillInfoEvents.TryGetValue(stateChangeEvent.SkillID, out SkillInfoEvent skillInfoEvent))
                    {
                        skillInfoEvent.CompleteSkillInfoEvent(stateChangeEvent);
                    }
                    else
                    {
                        skillInfoEvent = new SkillInfoEvent(stateChangeEvent);
                        metaDataEvents.SkillInfoEvents[stateChangeEvent.SkillID] = skillInfoEvent;
                    }
                    break;
                case StateChange.BreakbarState:
                    var bSEvt = new BreakbarStateEvent(stateChangeEvent, agentData);
                    Add(statusEvents.BreakbarStateEvents, bSEvt.Src, bSEvt);
                    break;
                case StateChange.BreakbarPercent:
                    var bPEvt = new BreakbarPercentEvent(stateChangeEvent, agentData);
                    Add(statusEvents.BreakbarPercentEvents, bPEvt.Src, bPEvt);
                    break;
                case StateChange.Error:
                    metaDataEvents.ErrorEvents.Add(new ErrorEvent(stateChangeEvent));
                    break;
                case StateChange.Tag:
                    var tagEvent = new TagEvent(stateChangeEvent, agentData);
                    Add(statusEvents.TagEvents, tagEvent.Src, tagEvent);
                    break;
                case StateChange.Velocity:
                    var velEvt = new VelocityEvent(stateChangeEvent, agentData);
                    Add(statusEvents.MovementEvents, velEvt.Src, velEvt);
                    break;
                case StateChange.Rotation:
                    var rotEvt = new RotationEvent(stateChangeEvent, agentData);
                    Add(statusEvents.MovementEvents, rotEvt.Src, rotEvt);
                    break;
                case StateChange.Position:
                    var posEvt = new PositionEvent(stateChangeEvent, agentData);
                    Add(statusEvents.MovementEvents, posEvt.Src, posEvt);
                    break;
                case StateChange.WeaponSwap:
                    wepSwaps.Add(new WeaponSwapEvent(stateChangeEvent, agentData, skillData));
                    break;
                case StateChange.StackActive:
                    buffEvents.Add(new BuffStackActiveEvent(stateChangeEvent, agentData, skillData));
                    break;
                case StateChange.StackReset:
                    buffEvents.Add(new BuffStackResetEvent(stateChangeEvent, agentData, skillData));
                    break;
                case StateChange.BuffInitial:
                    buffEvents.Add(new BuffApplyEvent(stateChangeEvent, agentData, skillData, evtcVersion));
                    break;
                case StateChange.TickRate:
                    metaDataEvents.TickRateEvents.Add(new TickRateEvent(stateChangeEvent));
                    break;
                case StateChange.Last90BeforeDown:
                    var last90Evt = new Last90BeforeDownEvent(stateChangeEvent, agentData);
                    statusEvents.Last90BeforeDownEvents.Add(last90Evt);
                    Add(statusEvents.Last90BeforeDownEventsBySrc, last90Evt.Src, last90Evt);
                    break;
                case StateChange.Effect_45:
                case StateChange.Effect_51:
                    EffectEvent effectEvt = null;
                    switch(stateChangeEvent.IsStateChange)
                    {
                        case StateChange.Effect_45:
                            // End event, not supported for 45
                            if (stateChangeEvent.SkillID == 0)
                            {
                                return;
                            }
                            effectEvt = new EffectEventCBTS45(stateChangeEvent, agentData);
                            break;
                        case StateChange.Effect_51:
                            if (stateChangeEvent.SkillID == 0)
                            {
                                var endEvent = new EffectEndEventCBTS51(stateChangeEvent, agentData, statusEvents.EffectEventsByTrackingID);
                                return;
                            } 
                            else
                            {
                                effectEvt = new EffectEventCBTS51(stateChangeEvent, agentData, statusEvents.EffectEventsByTrackingID);
                            }
                            break;
                        default:
                            throw new InvalidDataException("Invalid effect state change");
                    }
                    statusEvents.EffectEvents.Add(effectEvt);
                    Add(statusEvents.EffectEventsBySrc, effectEvt.Src, effectEvt);
                    Add(statusEvents.EffectEventsByEffectID, effectEvt.EffectID, effectEvt);
                    if (effectEvt.IsAroundDst)
                    {
                        Add(statusEvents.EffectEventsByDst, effectEvt.Dst, effectEvt);
                    }
                    break;
                case StateChange.EffectIDToGUID:
                    if (evtcVersion >= ArcDPSBuilds.FunctionalIDToGUIDEvents)
                    {
                        switch (GetContentLocal((byte)stateChangeEvent.OverstackValue))
                        {
                            case ContentLocal.Effect:
                                var effectGUID = new EffectGUIDEvent(stateChangeEvent);
                                metaDataEvents.EffectGUIDEventsByEffectID[effectGUID.ContentID] = effectGUID;
                                metaDataEvents.EffectGUIDEventsByGUID[effectGUID.ContentGUID] = effectGUID;
                                break;
                            case ContentLocal.Marker:
                                var markerGUID = new MarkerGUIDEvent(stateChangeEvent);
                                metaDataEvents.MarkerGUIDEventsByMarkerID[markerGUID.ContentID] = markerGUID;
                                metaDataEvents.MarkerGUIDEventsByGUID[markerGUID.ContentGUID] = markerGUID;
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case StateChange.FractalScale:
                    // Sanity check
                    if (stateChangeEvent.SrcAgent == 0)
                    {
                        return;
                    }
                    metaDataEvents.FractalScaleEvent = new FractalScaleEvent(stateChangeEvent);
                    break;
                default:
                    break;
            }
        }

        public static void AddBuffApplyEvent(CombatItem buffEvent, List<AbstractBuffEvent> buffEvents, AgentData agentData, SkillData skillData, int evtcVersion)
        {
            if (buffEvent.IsOffcycle > 0)
            {
                buffEvents.Add(new BuffExtensionEvent(buffEvent, agentData, skillData));
            }
            else
            {
                buffEvents.Add(new BuffApplyEvent(buffEvent, agentData, skillData, evtcVersion));
            }
        }

        public static void AddBuffRemoveEvent(CombatItem buffEvent, List<AbstractBuffEvent> buffEvents, AgentData agentData, SkillData skillData)
        {
            switch (buffEvent.IsBuffRemove)
            {
                case BuffRemove.Single:
                    buffEvents.Add(new BuffRemoveSingleEvent(buffEvent, agentData, skillData));
                    break;
                case BuffRemove.All:
                    buffEvents.Add(new BuffRemoveAllEvent(buffEvent, agentData, skillData));
                    break;
                case BuffRemove.Manual:
                    buffEvents.Add(new BuffRemoveManualEvent(buffEvent, agentData, skillData));
                    break;
            }
        }

        public static List<AnimatedCastEvent> CreateCastEvents(Dictionary<ulong, List<CombatItem>> castEventsBySrcAgent, AgentData agentData, SkillData skillData, FightData fightData)
        {
            var res = new List<AnimatedCastEvent>();
            foreach (KeyValuePair<ulong, List<CombatItem>> pairBySrcAgent in castEventsBySrcAgent)
            {
                var resBySrcAgent = new List<AnimatedCastEvent>();
                var castEventsBySrcAgentBySkillID = pairBySrcAgent.Value.GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList());
                foreach (KeyValuePair<uint, List<CombatItem>> pairBySrcAgentBySkillID in castEventsBySrcAgentBySkillID)
                {
                    var resBySrcAgentBySkillID = new List<AnimatedCastEvent>();
                    CombatItem startItem = null;
                    foreach (CombatItem c in pairBySrcAgentBySkillID.Value)
                    {
                        if (c.StartCasting())
                        {
                            // missing end
                            if (startItem != null)
                            {
                                resBySrcAgentBySkillID.Add(new AnimatedCastEvent(startItem, agentData, skillData, fightData.LogEnd));
                            }
                            startItem = c;
                        }
                        else if (c.EndCasting())
                        {
                            if (startItem != null && startItem.SkillID == c.SkillID)
                            {
                                resBySrcAgentBySkillID.Add(new AnimatedCastEvent(startItem, agentData, skillData, c));
                                startItem = null;
                            }
                            // missing start
                            else
                            {
                                var toCheck = new AnimatedCastEvent(agentData, skillData, c);
                                // we are only interested in animations started before log starts
                                if (toCheck.Time < fightData.LogStart)
                                {
                                    resBySrcAgentBySkillID.Add(toCheck);
                                }
                            }
                        }
                    }
                    // missing end
                    if (startItem != null)
                    {
                        resBySrcAgentBySkillID.Add(new AnimatedCastEvent(startItem, agentData, skillData, fightData.LogEnd));
                    }
                    resBySrcAgentBySkillID.RemoveAll(x => x.Caster.IsPlayer && x.ActualDuration <= 1);
                    resBySrcAgent.AddRange(resBySrcAgentBySkillID);
                }
                resBySrcAgent = resBySrcAgent.OrderBy(x => x.Time).ToList();
                // sanitize 
                for (int i = 0; i < resBySrcAgent.Count - 1; i++)
                {
                    resBySrcAgent[i].CutAt(resBySrcAgent[i + 1].Time + ParserHelper.ServerDelayConstant);
                }
                res.AddRange(resBySrcAgent);
            }
            res = res.OrderBy(x => x.Time).ToList();
            return res;
        }

        public static void AddDirectDamageEvent(CombatItem damageEvent, List<AbstractHealthDamageEvent> hpDamage, List<AbstractBreakbarDamageEvent> brkBarDamage, AgentData agentData, SkillData skillData)
        {
            ArcDPSEnums.PhysicalResult result = GetPhysicalResult(damageEvent.Result);
            switch (result)
            {
                case PhysicalResult.BreakbarDamage:
                    brkBarDamage.Add(new DirectBreakbarDamageEvent(damageEvent, agentData, skillData));
                    break;
                case PhysicalResult.Activation:
                case PhysicalResult.Unknown:
                    break;
                default:
                    hpDamage.Add(new DirectHealthDamageEvent(damageEvent, agentData, skillData, result));
                    break;
            }
        }

        public static void AddIndirectDamageEvent(CombatItem damageEvent, List<AbstractHealthDamageEvent> hpDamage, List<AbstractBreakbarDamageEvent> brkBarDamage, AgentData agentData, SkillData skillData)
        {
            ArcDPSEnums.ConditionResult result = GetConditionResult(damageEvent.Result);
            switch (result)
            {
                /*case ArcDPSEnums.ConditionResult.BreakbarDamage:
                    brkBarDamage.Add(new NonDirectBreakbarDamageEvent(c, agentData, skillData));
                    break;*/
                case ConditionResult.Unknown:
                    break;
                default:
                    hpDamage.Add(new NonDirectHealthDamageEvent(damageEvent, agentData, skillData, result));
                    break;
            }
        }

    }
}
