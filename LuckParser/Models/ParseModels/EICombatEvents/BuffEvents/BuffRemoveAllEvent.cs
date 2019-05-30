using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BuffRemoveAllEvent : AbstractBuffRemoveEvent
    {
        private readonly int _removedStacks;
        private readonly int _lastRemovedDuration;

        public BuffRemoveAllEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            _lastRemovedDuration = evtcItem.BuffDmg;
            _removedStacks = evtcItem.Result;
        }

        public BuffRemoveAllEvent(AgentItem by, AgentItem to, long time, int removedDuration, long buffID, int removedStacks, int lastRemovedDuration) : base(by, to, time, removedDuration, buffID)
        {
            _lastRemovedDuration = lastRemovedDuration;
            _removedStacks = removedStacks;
        }

        public override bool IsBoonSimulatorCompliant(long fightEnd)
        {
            return BuffID != BoonHelper.NoBuff &&
                !(Value <= 50 && _lastRemovedDuration <= 50) && // low value all stack remove that can mess up with the simulator if server delay
                 Time <= fightEnd - 50; // don't take into account removal that are close to the end of the fight
        }

        public override void UpdateSimulator(BoonSimulator simulator)
        {
            simulator.Remove(By, Value, Time, ParseEnum.BuffRemove.All);
        }
    }
}
