using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class EICombatEventFactory
    {

        public static Dictionary<AgentItem, List<AbstractMovementEvent>> CreateMovementEvents(List<CombatItem> movementEvents, AgentData agentData, long offset)
        {
            Dictionary<AgentItem, List<AbstractMovementEvent>> res = new Dictionary<AgentItem, List<AbstractMovementEvent>>();
            foreach (CombatItem c in movementEvents)
            {
                AbstractMovementEvent evt = null;
                switch (c.IsStateChange)
                {
                    case ParseEnum.StateChange.Velocity:
                        evt = new VelocityEvent(c, agentData, offset);
                        break;
                    case ParseEnum.StateChange.Rotation:
                        evt = new RotationEvent(c, agentData, offset);
                        break;
                    case ParseEnum.StateChange.Position:
                        evt = new PositionEvent(c, agentData, offset);
                        break;
                    default:
                        break;
                }
                if (evt != null)
                {
                    if (res.TryGetValue(evt.AgentItem, out var list))
                    {
                        list.Add(evt);
                    } else
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

        public static void CreateStateChangeEvents(List<CombatItem> stateChangeEvents, MetaEventsContainer metaDataEvents, StatusEventsContainer statusEvents, AgentData agentData, long offset) 
        {
            foreach (CombatItem c in stateChangeEvents)
            {
                switch (c.IsStateChange)
                {
                    case ParseEnum.StateChange.EnterCombat:
                        EnterCombatEvent enterCombatEvt = new EnterCombatEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.EnterCombatEvents, enterCombatEvt.Src, enterCombatEvt);
                        break;
                    case ParseEnum.StateChange.ExitCombat:
                        ExitCombatEvent exitCombatEvt = new ExitCombatEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.ExitCombatEvents, exitCombatEvt.Src, exitCombatEvt);
                        break;
                    case ParseEnum.StateChange.ChangeUp:
                        AliveEvent aliveEvt = new AliveEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.AliveEvents, aliveEvt.Src, aliveEvt);
                        break;
                    case ParseEnum.StateChange.ChangeDead:
                        DeadEvent deadEvt = new DeadEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.DeadEvents, deadEvt.Src, deadEvt);
                        break;
                    case ParseEnum.StateChange.ChangeDown:
                        DownEvent downEvt = new DownEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.DownEvents, downEvt.Src, downEvt);
                        break;
                    case ParseEnum.StateChange.Spawn:
                        SpawnEvent spawnEvt = new SpawnEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.SpawnEvents, spawnEvt.Src, spawnEvt);
                        break;
                    case ParseEnum.StateChange.Despawn:
                        DespawnEvent despawnEvt = new DespawnEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.DespawnEvents, despawnEvt.Src, despawnEvt);
                        break;
                    case ParseEnum.StateChange.HealthUpdate:
                        HealthUpdateEvent healthEvt = new HealthUpdateEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.HealthUpdateEvents, healthEvt.Src, healthEvt);
                        break;
                    case ParseEnum.StateChange.LogStart:
                        metaDataEvents.LogStartEvents.Add(new LogStartEvent(c, offset));
                        break;
                    case ParseEnum.StateChange.LogEnd:
                        metaDataEvents.LogEndEvents.Add(new LogEndEvent(c, offset));
                        break;
                    case ParseEnum.StateChange.MaxHealthUpdate:
                        MaxHealthUpdateEvent maxHealthEvt = new MaxHealthUpdateEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.MaxHealthUpdateEvents, maxHealthEvt.Src, maxHealthEvt);
                        break;
                    case ParseEnum.StateChange.PointOfView:
                        metaDataEvents.PointOfViewEvents.Add(new PointOfViewEvent(c, agentData, offset));
                        break;
                    case ParseEnum.StateChange.Language:
                        metaDataEvents.LanguageEvents.Add(new LanguageEvent(c, offset));
                        break;
                    case ParseEnum.StateChange.GWBuild:
                        metaDataEvents.BuildEvents.Add(new BuildEvent(c, offset));
                        break;
                    case ParseEnum.StateChange.ShardId:
                        metaDataEvents.ShardEvents.Add(new ShardEvent(c, offset));
                        break;
                    case ParseEnum.StateChange.Reward:
                        metaDataEvents.RewardEvents.Add(new RewardEvent(c, offset));
                        break;
                    case ParseEnum.StateChange.TeamChange:
                        TeamChangeEvent tcEvt = new TeamChangeEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.TeamChangeEvents, tcEvt.Src, tcEvt);
                        break;
                    case ParseEnum.StateChange.AttackTarget:
                        AttackTargetEvent aTEvt = new AttackTargetEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.AttackTargetEvents, aTEvt.Src, aTEvt);
                        break;
                    case ParseEnum.StateChange.Targetable:
                        TargetableEvent tarEvt = new TargetableEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.TargetableEvents, tarEvt.Src, tarEvt);
                        break;
                    case ParseEnum.StateChange.MapID:
                        metaDataEvents.MapIDEvents.Add(new MapIDEvent(c, offset));
                        break;
                    case ParseEnum.StateChange.Guild:
                        GuildEvent gEvt = new GuildEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.GuildEvents, gEvt.Src, gEvt);
                        break;
                }
            }
        }

        public static List<WeaponSwapEvent> CreateWeaponSwapEvents(List<CombatItem> swapEvents, AgentData agentData, long offset)
        {
            List<WeaponSwapEvent> res = new List<WeaponSwapEvent>();
            foreach (CombatItem swapEvent in swapEvents)
            {
                res.Add(new WeaponSwapEvent(swapEvent, agentData, offset));
            }
            return res;
        }

        public static List<AbstractBuffEvent> CreateBuffEvents(List<CombatItem> buffEvents, AgentData agentData, long offset)
        {
            List<AbstractBuffEvent> res = new List<AbstractBuffEvent>();
            foreach (CombatItem c in buffEvents)
            {
                switch (c.IsBuffRemove)
                {
                    case ParseEnum.BuffRemove.None:
                        if (c.IsOffcycle > 0)
                        {
                            res.Add(new BuffExtensionEvent(c, agentData, offset));
                        }
                        else
                        {
                            res.Add(new BuffApplyEvent(c, agentData, offset));
                        }
                        break;
                    case ParseEnum.BuffRemove.Single:
                        res.Add(new BuffRemoveSingleEvent(c, agentData, offset));
                        break;
                    case ParseEnum.BuffRemove.All:
                        res.Add(new BuffRemoveAllEvent(c, agentData, offset));
                        break;
                    case ParseEnum.BuffRemove.Manual:
                        res.Add(new BuffRemoveManualEvent(c, agentData, offset));
                        break;
                }
            }
            return res;
        }

        public static List<AnimatedCastEvent> CreateCastEvents(List<CombatItem> castEvents, AgentData agentData, long offset)
        {
            List<AnimatedCastEvent> res = new List<AnimatedCastEvent>();
            Dictionary<ushort, List<CombatItem>> castEventsBySrcAgent = castEvents.GroupBy(x => x.SrcInstid).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var pair in castEventsBySrcAgent)
            {
                CombatItem startItem = null;
                foreach (CombatItem c in pair.Value)
                {
                    if (c.IsActivation.StartCasting())
                    {
                        // missing end
                        if (startItem != null)
                        {
                            res.Add(new AnimatedCastEvent(startItem, agentData, offset, c.LogTime));
                        }
                        startItem = c;
                    } else
                    {
                        if (startItem != null && startItem.SkillID == c.SkillID)
                        {
                            res.Add(new AnimatedCastEvent(startItem, c, agentData, offset));
                            startItem = null;
                        }
                    }
                }
            }
            res.Sort((x, y) => x.Time.CompareTo(y.Time));
            return res;
        }

        public static List<AbstractDamageEvent> CreateDamageEvents(List<CombatItem> damageEvents, AgentData agentData, long offset)
        {
            List<AbstractDamageEvent> res = new List<AbstractDamageEvent>();
            foreach (CombatItem c in damageEvents)
            {
                if ((c.IsBuff != 0 && c.Value == 0))
                {
                    res.Add(new NonDirectDamageEvent(c, agentData, offset));
                }
                else if (c.IsBuff == 0)
                {
                    res.Add(new DirectDamageEvent(c, agentData, offset));
                }
            }
            return res;
        }

    }
}
