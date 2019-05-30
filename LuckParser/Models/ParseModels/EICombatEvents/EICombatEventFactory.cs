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

        public static void CreateStateChangeEvents(List<CombatItem> stateChangeEvents, List<AbstractMetaDataEvent> metaDataEvents, List<AbstractStatusEvent> statusEvents, AgentData agentData, long offset) 
        {
            foreach (CombatItem c in stateChangeEvents)
            {
                switch (c.IsStateChange)
                {
                    case ParseEnum.StateChange.EnterCombat:
                        statusEvents.Add(new EnterCombatEvent(c, agentData, offset));
                        break;
                    case ParseEnum.StateChange.ExitCombat:
                        statusEvents.Add(new ExitCombatEvent(c, agentData, offset));
                        break;
                    case ParseEnum.StateChange.ChangeUp:
                        statusEvents.Add(new AliveEvent(c, agentData, offset));
                        break;
                    case ParseEnum.StateChange.ChangeDead:
                        statusEvents.Add(new DeadEvent(c, agentData, offset));
                        break;
                    case ParseEnum.StateChange.ChangeDown:
                        statusEvents.Add(new DownEvent(c, agentData, offset));
                        break;
                    case ParseEnum.StateChange.Spawn:
                        statusEvents.Add(new SpawnEvent(c, agentData, offset));
                        break;
                    case ParseEnum.StateChange.Despawn:
                        statusEvents.Add(new DespawnEvent(c, agentData, offset));
                        break;
                    case ParseEnum.StateChange.HealthUpdate:
                        statusEvents.Add(new HealthUpdateEvent(c, agentData, offset));
                        break;
                    case ParseEnum.StateChange.LogStart:
                        metaDataEvents.Add(new LogStartEvent(c, offset));
                        break;
                    case ParseEnum.StateChange.LogEnd:
                        metaDataEvents.Add(new LogEndEvent(c, offset));
                        break;
                    case ParseEnum.StateChange.MaxHealthUpdate:
                        statusEvents.Add(new MaxHealthUpdateEvent(c, agentData, offset));
                        break;
                    case ParseEnum.StateChange.PointOfView:
                        statusEvents.Add(new PointOfViewEvent(c, agentData, offset));
                        break;
                    case ParseEnum.StateChange.Language:
                        metaDataEvents.Add(new LanguageEvent(c, offset));
                        break;
                    case ParseEnum.StateChange.GWBuild:
                        metaDataEvents.Add(new BuildEvent(c, offset));
                        break;
                    case ParseEnum.StateChange.ShardId:
                        metaDataEvents.Add(new ShardEvent(c, offset));
                        break;
                    case ParseEnum.StateChange.Reward:
                        metaDataEvents.Add(new RewardEvent(c, offset));
                        break;
                    case ParseEnum.StateChange.TeamChange:
                        statusEvents.Add(new TeamChangeEvent(c, agentData, offset));
                        break;
                    case ParseEnum.StateChange.AttackTarget:
                        statusEvents.Add(new AttackTargetEvent(c, agentData, offset));
                        break;
                    case ParseEnum.StateChange.Targetable:
                        statusEvents.Add(new TargetableEvent(c, agentData, offset));
                        break;
                    case ParseEnum.StateChange.MapID:
                        metaDataEvents.Add(new MapIDEvent(c, offset));
                        break;
                    case ParseEnum.StateChange.Guild:
                        statusEvents.Add(new GuildEvent(c, agentData, offset));
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
            Dictionary<ulong, List<CombatItem>> castEventsBySrcAgent = castEvents.GroupBy(x => x.SrcAgent).ToDictionary(x => x.Key, x => x.ToList());
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
                            res.Add(new AnimatedCastEvent(startItem, null, agentData, offset));
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
