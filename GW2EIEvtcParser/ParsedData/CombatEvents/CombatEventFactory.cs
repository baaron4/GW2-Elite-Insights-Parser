using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.ParsedData
{
    internal static class CombatEventFactory
    {
        private static void Add<TKey, TValue>(Dictionary<TKey, List<TValue>> dict, TKey key, TValue evt)
        {
            if (dict.TryGetValue(key, out List<TValue> list))
            {
                list.Add(evt);
            }
            else
            {
                dict[key] = new List<TValue>()
                {
                    evt
                };
            }
        }

        public static Dictionary<AgentItem, List<AbstractMovementEvent>> CreateMovementEvents(List<CombatItem> movementEvents, AgentData agentData)
        {
            var res = new Dictionary<AgentItem, List<AbstractMovementEvent>>();
            foreach (CombatItem c in movementEvents)
            {
                AbstractMovementEvent evt = null;
                switch (c.IsStateChange)
                {
                    case ArcDPSEnums.StateChange.Velocity:
                        evt = new VelocityEvent(c, agentData);
                        break;
                    case ArcDPSEnums.StateChange.Rotation:
                        evt = new RotationEvent(c, agentData);
                        break;
                    case ArcDPSEnums.StateChange.Position:
                        evt = new PositionEvent(c, agentData);
                        break;
                    default:
                        break;
                }
                if (evt != null)
                {
                    if (res.TryGetValue(evt.AgentItem, out List<AbstractMovementEvent> list))
                    {
                        list.Add(evt);
                    }
                    else
                    {
                        res[evt.AgentItem] = new List<AbstractMovementEvent>()
                        {
                            evt
                        };
                    }
                }
            }
            return res;
        }

        public static void CreateStateChangeEvents(List<CombatItem> stateChangeEvents, MetaEventsContainer metaDataEvents, StatusEventsContainer statusEvents, List<RewardEvent> rewardEvents, AgentData agentData)
        {
            foreach (CombatItem c in stateChangeEvents)
            {
                switch (c.IsStateChange)
                {
                    case ArcDPSEnums.StateChange.EnterCombat:
                        var enterCombatEvt = new EnterCombatEvent(c, agentData);
                        Add(statusEvents.EnterCombatEvents, enterCombatEvt.Src, enterCombatEvt);
                        break;
                    case ArcDPSEnums.StateChange.ExitCombat:
                        var exitCombatEvt = new ExitCombatEvent(c, agentData);
                        Add(statusEvents.ExitCombatEvents, exitCombatEvt.Src, exitCombatEvt);
                        break;
                    case ArcDPSEnums.StateChange.ChangeUp:
                        var aliveEvt = new AliveEvent(c, agentData);
                        Add(statusEvents.AliveEvents, aliveEvt.Src, aliveEvt);
                        break;
                    case ArcDPSEnums.StateChange.ChangeDead:
                        var deadEvt = new DeadEvent(c, agentData);
                        Add(statusEvents.DeadEvents, deadEvt.Src, deadEvt);
                        break;
                    case ArcDPSEnums.StateChange.ChangeDown:
                        var downEvt = new DownEvent(c, agentData);
                        Add(statusEvents.DownEvents, downEvt.Src, downEvt);
                        break;
                    case ArcDPSEnums.StateChange.Spawn:
                        var spawnEvt = new SpawnEvent(c, agentData);
                        Add(statusEvents.SpawnEvents, spawnEvt.Src, spawnEvt);
                        break;
                    case ArcDPSEnums.StateChange.Despawn:
                        var despawnEvt = new DespawnEvent(c, agentData);
                        Add(statusEvents.DespawnEvents, despawnEvt.Src, despawnEvt);
                        break;
                    case ArcDPSEnums.StateChange.HealthUpdate:
                        var healthEvt = new HealthUpdateEvent(c, agentData);
                        Add(statusEvents.HealthUpdateEvents, healthEvt.Src, healthEvt);
                        break;
                    case ArcDPSEnums.StateChange.LogStart:
                        if (c.Value == 0 || c.BuffDmg == 0)
                        {
                            continue;
                        }
                        metaDataEvents.LogStartEvent = new LogStartEvent(c);
                        break;
                    case ArcDPSEnums.StateChange.LogEnd:
                        if (c.Value == 0 || c.BuffDmg == 0)
                        {
                            continue;
                        }
                        metaDataEvents.LogEndEvent = new LogEndEvent(c);
                        break;
                    case ArcDPSEnums.StateChange.MaxHealthUpdate:
                        var maxHealthEvt = new MaxHealthUpdateEvent(c, agentData);
                        Add(statusEvents.MaxHealthUpdateEvents, maxHealthEvt.Src, maxHealthEvt);
                        break;
                    case ArcDPSEnums.StateChange.PointOfView:
                        if (c.SrcAgent == 0)
                        {
                            continue;
                        }
                        metaDataEvents.PointOfViewEvent = new PointOfViewEvent(c, agentData);
                        break;
                    case ArcDPSEnums.StateChange.Language:
                        metaDataEvents.LanguageEvent = new LanguageEvent(c);
                        break;
                    case ArcDPSEnums.StateChange.GWBuild:
                        if (c.SrcAgent == 0)
                        {
                            continue;
                        }
                        metaDataEvents.BuildEvent = new BuildEvent(c);
                        break;
                    case ArcDPSEnums.StateChange.ShardId:
                        metaDataEvents.ShardEvents.Add(new ShardEvent(c));
                        break;
                    case ArcDPSEnums.StateChange.Reward:
#if !NO_REWARDS
                        rewardEvents.Add(new RewardEvent(c));
#endif
                        break;
                    case ArcDPSEnums.StateChange.TeamChange:
                        var tcEvt = new TeamChangeEvent(c, agentData);
                        Add(statusEvents.TeamChangeEvents, tcEvt.Src, tcEvt);
                        break;
                    case ArcDPSEnums.StateChange.AttackTarget:
                        var aTEvt = new AttackTargetEvent(c, agentData);
                        Add(statusEvents.AttackTargetEvents, aTEvt.Src, aTEvt);
                        break;
                    case ArcDPSEnums.StateChange.Targetable:
                        var tarEvt = new TargetableEvent(c, agentData);
                        Add(statusEvents.TargetableEvents, tarEvt.Src, tarEvt);
                        break;
                    case ArcDPSEnums.StateChange.MapID:
                        metaDataEvents.MapIDEvents.Add(new MapIDEvent(c));
                        break;
                    case ArcDPSEnums.StateChange.Guild:
                        var gEvt = new GuildEvent(c, agentData);
                        Add(metaDataEvents.GuildEvents, gEvt.Src, gEvt);
                        break;
                    case ArcDPSEnums.StateChange.BuffInfo:
                    case ArcDPSEnums.StateChange.BuffFormula:
                        if (metaDataEvents.BuffInfoEvents.TryGetValue(c.SkillID, out BuffInfoEvent buffInfoEvent))
                        {
                            buffInfoEvent.CompleteBuffInfoEvent(c);
                        } 
                        else
                        {
                            buffInfoEvent = new BuffInfoEvent(c);
                            metaDataEvents.BuffInfoEvents[c.SkillID] = buffInfoEvent;
                        }
                        if (c.IsStateChange == ArcDPSEnums.StateChange.BuffInfo)
                        {
                            if (metaDataEvents.BuffInfoEventsByCategory.TryGetValue(buffInfoEvent.Category, out List<BuffInfoEvent> bdEvtList))
                            {
                                bdEvtList.Add(buffInfoEvent);
                            }
                            else
                            {
                                metaDataEvents.BuffInfoEventsByCategory[buffInfoEvent.Category] = new List<BuffInfoEvent> { buffInfoEvent };
                            }
                        }
                        break;
                    case ArcDPSEnums.StateChange.SkillInfo:
                    case ArcDPSEnums.StateChange.SkillTiming:
                        if (metaDataEvents.SkillInfoEvents.TryGetValue(c.SkillID, out SkillInfoEvent skillInfoEvent))
                        {
                            skillInfoEvent.CompleteSkillInfoEvent(c);
                        }
                        else
                        {
                            skillInfoEvent = new SkillInfoEvent(c);
                            metaDataEvents.SkillInfoEvents[c.SkillID] = skillInfoEvent;
                        }
                        break;
                    case ArcDPSEnums.StateChange.BreakbarState:
                        var bSEvt = new BreakbarStateEvent(c, agentData);
                        Add(statusEvents.BreakbarStateEvents, bSEvt.Src, bSEvt);
                        break;
                    case ArcDPSEnums.StateChange.BreakbarPercent:
                        var bPEvt = new BreakbarPercentEvent(c, agentData);
                        Add(statusEvents.BreakbarPercentEvents, bPEvt.Src, bPEvt);
                        break;
                    case ArcDPSEnums.StateChange.Error:
                        metaDataEvents.ErrorEvents.Add(new ErrorEvent(c));
                        break;
                    case ArcDPSEnums.StateChange.Tag:
                        // Getting ready in case this becomes an actual state event
                        _ = new TagEvent(c, agentData);
                        //Add(metaDataEvents.TagEvents, tagEvt.Src, tagEvt);
                        break;
                }
            }
        }

        public static List<WeaponSwapEvent> CreateWeaponSwapEvents(List<CombatItem> swapEvents, AgentData agentData, SkillData skillData)
        {
            var res = new List<WeaponSwapEvent>();
            foreach (CombatItem swapEvent in swapEvents)
            {
                res.Add(new WeaponSwapEvent(swapEvent, agentData, skillData));
            }
            return res;
        }

        public static List<AbstractBuffEvent> CreateBuffEvents(List<CombatItem> buffEvents, AgentData agentData, SkillData skillData)
        {
            var res = new List<AbstractBuffEvent>();
            foreach (CombatItem c in buffEvents)
            {
                switch (c.IsStateChange)
                {
                    case ArcDPSEnums.StateChange.StackActive:
                        res.Add(new BuffStackActiveEvent(c, agentData, skillData));
                        break;
                    case ArcDPSEnums.StateChange.StackReset:
                        res.Add(new BuffStackResetEvent(c, agentData, skillData));
                        break;
                    default:
                        switch (c.IsBuffRemove)
                        {
                            case ArcDPSEnums.BuffRemove.None:
                                if (c.IsOffcycle > 0)
                                {
                                    res.Add(new BuffExtensionEvent(c, agentData, skillData));
                                }
                                else
                                {
                                    res.Add(new BuffApplyEvent(c, agentData, skillData));
                                }
                                break;
                            case ArcDPSEnums.BuffRemove.Single:
                                res.Add(new BuffRemoveSingleEvent(c, agentData, skillData));
                                break;
                            case ArcDPSEnums.BuffRemove.All:
                                res.Add(new BuffRemoveAllEvent(c, agentData, skillData));
                                break;
                            case ArcDPSEnums.BuffRemove.Manual:
                                res.Add(new BuffRemoveManualEvent(c, agentData, skillData));
                                break;
                        }
                        break;
                }
            }
            return res;
        }

        public static List<AnimatedCastEvent> CreateCastEvents(List<CombatItem> castEvents, AgentData agentData, SkillData skillData)
        {
            var res = new List<AnimatedCastEvent>();
            var castEventsBySrcAgent = castEvents.GroupBy(x => x.SrcAgent).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<ulong, List<CombatItem>> pair in castEventsBySrcAgent)
            {
                CombatItem startItem = null;
                foreach (CombatItem c in pair.Value)
                {
                    if (c.IsActivation.StartCasting())
                    {
                        // missing end
                        if (startItem != null)
                        {
                            res.Add(new AnimatedCastEvent(startItem, agentData, skillData, c.Time));
                        }
                        startItem = c;
                    }
                    else if (c.IsActivation.EndCasting())
                    {
                        if (startItem != null && startItem.SkillID == c.SkillID)
                        {
                            res.Add(new AnimatedCastEvent(startItem, c, agentData, skillData));
                            startItem = null;
                        }
                    }
                }
            }
            res.Sort((x, y) => x.Time.CompareTo(y.Time));
            return res;
        }

        public static (List<AbstractHealthDamageEvent>, List<AbstractBreakbarDamageEvent>) CreateDamageEvents(List<CombatItem> damageEvents, AgentData agentData, SkillData skillData)
        {
            var hpDamage = new List<AbstractHealthDamageEvent>();
            var brkBarDamage = new List<AbstractBreakbarDamageEvent>();
            foreach (CombatItem c in damageEvents)
            {
                if ((c.IsBuff != 0 && c.Value == 0))
                {
                    ArcDPSEnums.ConditionResult result = ArcDPSEnums.GetConditionResult(c.Result);
                    switch (result)
                    {
                        /*case ArcDPSEnums.ConditionResult.BreakbarDamage:
                            brkBarDamage.Add(new NonDirectBreakbarDamageEvent(c, agentData, skillData));
                            break;*/
                        case ArcDPSEnums.ConditionResult.Unknown:
                            break;
                        default:
                            hpDamage.Add(new NonDirectHealthDamageEvent(c, agentData, skillData, result));
                            break;
                    }
                }
                else if (c.IsBuff == 0)
                {
                    ArcDPSEnums.PhysicalResult result = ArcDPSEnums.GetPhysicalResult(c.Result);
                    switch (result)
                    {
                        case ArcDPSEnums.PhysicalResult.BreakbarDamage:
                            brkBarDamage.Add(new DirectBreakbarDamageEvent(c, agentData, skillData));
                            break;
                        case ArcDPSEnums.PhysicalResult.Unknown:
                            break;
                        default:
                            hpDamage.Add(new DirectHealthDamageEvent(c, agentData, skillData, result));
                            break;
                    }
                }
            }
            return (hpDamage, brkBarDamage);
        }

    }
}
