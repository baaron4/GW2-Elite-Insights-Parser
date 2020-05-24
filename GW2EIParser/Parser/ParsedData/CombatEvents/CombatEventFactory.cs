using System.Collections.Generic;
using System.Linq;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public static class CombatEventFactory
    {

        public static Dictionary<AgentItem, List<AbstractMovementEvent>> CreateMovementEvents(List<CombatItem> movementEvents, AgentData agentData)
        {
            var res = new Dictionary<AgentItem, List<AbstractMovementEvent>>();
            foreach (CombatItem c in movementEvents)
            {
                AbstractMovementEvent evt = null;
                switch (c.IsStateChange)
                {
                    case ParseEnum.StateChange.Velocity:
                        evt = new VelocityEvent(c, agentData);
                        break;
                    case ParseEnum.StateChange.Rotation:
                        evt = new RotationEvent(c, agentData);
                        break;
                    case ParseEnum.StateChange.Position:
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
                    case ParseEnum.StateChange.EnterCombat:
                        var enterCombatEvt = new EnterCombatEvent(c, agentData);
                        GeneralHelper.Add(statusEvents.EnterCombatEvents, enterCombatEvt.Src, enterCombatEvt);
                        break;
                    case ParseEnum.StateChange.ExitCombat:
                        var exitCombatEvt = new ExitCombatEvent(c, agentData);
                        GeneralHelper.Add(statusEvents.ExitCombatEvents, exitCombatEvt.Src, exitCombatEvt);
                        break;
                    case ParseEnum.StateChange.ChangeUp:
                        var aliveEvt = new AliveEvent(c, agentData);
                        GeneralHelper.Add(statusEvents.AliveEvents, aliveEvt.Src, aliveEvt);
                        break;
                    case ParseEnum.StateChange.ChangeDead:
                        var deadEvt = new DeadEvent(c, agentData);
                        GeneralHelper.Add(statusEvents.DeadEvents, deadEvt.Src, deadEvt);
                        break;
                    case ParseEnum.StateChange.ChangeDown:
                        var downEvt = new DownEvent(c, agentData);
                        GeneralHelper.Add(statusEvents.DownEvents, downEvt.Src, downEvt);
                        break;
                    case ParseEnum.StateChange.Spawn:
                        var spawnEvt = new SpawnEvent(c, agentData);
                        GeneralHelper.Add(statusEvents.SpawnEvents, spawnEvt.Src, spawnEvt);
                        break;
                    case ParseEnum.StateChange.Despawn:
                        var despawnEvt = new DespawnEvent(c, agentData);
                        GeneralHelper.Add(statusEvents.DespawnEvents, despawnEvt.Src, despawnEvt);
                        break;
                    case ParseEnum.StateChange.HealthUpdate:
                        var healthEvt = new HealthUpdateEvent(c, agentData);
                        GeneralHelper.Add(statusEvents.HealthUpdateEvents, healthEvt.Src, healthEvt);
                        break;
                    case ParseEnum.StateChange.LogStart:
                        if (c.Value == 0 || c.BuffDmg == 0)
                        {
                            continue;
                        }
                        metaDataEvents.LogStartEvent = new LogStartEvent(c);
                        break;
                    case ParseEnum.StateChange.LogEnd:
                        if (c.Value == 0 || c.BuffDmg == 0)
                        {
                            continue;
                        }
                        metaDataEvents.LogEndEvent = new LogEndEvent(c);
                        break;
                    case ParseEnum.StateChange.MaxHealthUpdate:
                        var maxHealthEvt = new MaxHealthUpdateEvent(c, agentData);
                        GeneralHelper.Add(statusEvents.MaxHealthUpdateEvents, maxHealthEvt.Src, maxHealthEvt);
                        break;
                    case ParseEnum.StateChange.PointOfView:
                        if (c.SrcAgent == 0)
                        {
                            continue;
                        }
                        metaDataEvents.PointOfViewEvent = new PointOfViewEvent(c, agentData);
                        break;
                    case ParseEnum.StateChange.Language:
                        if (c.SrcAgent == 0)
                        {
                            continue;
                        }
                        metaDataEvents.LanguageEvent = new LanguageEvent(c);
                        break;
                    case ParseEnum.StateChange.GWBuild:
                        if (c.SrcAgent == 0)
                        {
                            continue;
                        }
                        metaDataEvents.BuildEvent = new BuildEvent(c);
                        break;
                    case ParseEnum.StateChange.ShardId:
                        metaDataEvents.ShardEvents.Add(new ShardEvent(c));
                        break;
                    case ParseEnum.StateChange.Reward:
                        rewardEvents.Add(new RewardEvent(c));
                        break;
                    case ParseEnum.StateChange.TeamChange:
                        var tcEvt = new TeamChangeEvent(c, agentData);
                        GeneralHelper.Add(statusEvents.TeamChangeEvents, tcEvt.Src, tcEvt);
                        break;
                    case ParseEnum.StateChange.AttackTarget:
                        var aTEvt = new AttackTargetEvent(c, agentData);
                        GeneralHelper.Add(statusEvents.AttackTargetEvents, aTEvt.Src, aTEvt);
                        break;
                    case ParseEnum.StateChange.Targetable:
                        var tarEvt = new TargetableEvent(c, agentData);
                        GeneralHelper.Add(statusEvents.TargetableEvents, tarEvt.Src, tarEvt);
                        break;
                    case ParseEnum.StateChange.MapID:
                        metaDataEvents.MapIDEvents.Add(new MapIDEvent(c));
                        break;
                    case ParseEnum.StateChange.Guild:
                        var gEvt = new GuildEvent(c, agentData);
                        GeneralHelper.Add(metaDataEvents.GuildEvents, gEvt.Src, gEvt);
                        break;
                    case ParseEnum.StateChange.BuffInfo:
                    case ParseEnum.StateChange.BuffFormula:
                        BuffInfoEvent buffInfoEvent;
                        if (metaDataEvents.BuffInfoEvents.TryGetValue(c.SkillID, out buffInfoEvent))
                        {
                            buffInfoEvent.CompleteBuffDataEvent(c);
                        } 
                        else
                        {
                            buffInfoEvent = new BuffInfoEvent(c);
                            metaDataEvents.BuffInfoEvents[c.SkillID] = buffInfoEvent;
                        }
                        if (c.IsStateChange == ParseEnum.StateChange.BuffInfo)
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
                    case ParseEnum.StateChange.Error:
                        metaDataEvents.ErrorEvents.Add(new ErrorEvent(c));
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
                    case ParseEnum.StateChange.StackActive:
                        res.Add(new BuffStackActiveEvent(c, agentData, skillData));
                        break;
                    case ParseEnum.StateChange.StackReset:
                        res.Add(new BuffStackResetEvent(c, agentData, skillData));
                        break;
                    default:
                        switch (c.IsBuffRemove)
                        {
                            case ParseEnum.BuffRemove.None:
                                if (c.IsOffcycle > 0)
                                {
                                    res.Add(new BuffExtensionEvent(c, agentData, skillData));
                                }
                                else
                                {
                                    res.Add(new BuffApplyEvent(c, agentData, skillData));
                                }
                                break;
                            case ParseEnum.BuffRemove.Single:
                                res.Add(new BuffRemoveSingleEvent(c, agentData, skillData));
                                break;
                            case ParseEnum.BuffRemove.All:
                                res.Add(new BuffRemoveAllEvent(c, agentData, skillData));
                                break;
                            case ParseEnum.BuffRemove.Manual:
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
                    else
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

        public static List<AbstractDamageEvent> CreateDamageEvents(List<CombatItem> damageEvents, AgentData agentData, SkillData skillData)
        {
            var res = new List<AbstractDamageEvent>();
            foreach (CombatItem c in damageEvents)
            {
                if ((c.IsBuff != 0 && c.Value == 0))
                {
                    res.Add(new NonDirectDamageEvent(c, agentData, skillData));
                }
                else if (c.IsBuff == 0)
                {
                    res.Add(new DirectDamageEvent(c, agentData, skillData));
                }
            }
            return res;
        }

    }
}
