using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BuffRemoveSingleEvent : AbstractBuffRemoveEvent
    {
        private readonly ParseEnum.IFF _iff;
        public BuffRemoveSingleEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            _iff = evtcItem.IFF;
        }

        public override bool IsBoonSimulatorCompliant(long fightEnd)
        {
            return BuffID != BoonHelper.NoBuff &&
                !(_iff == ParseEnum.IFF.Unknown && To == GeneralHelper.UnknownAgent) && // weird single stack remove
                !(RemovedDuration <= 50) &&// low value single stack remove that can mess up with the simulator if server delay
                 Time <= fightEnd - 50; // don't take into account removal that are close to the end of the fight
        }

        public override void UpdateSimulator(BoonSimulator simulator)
        {
            simulator.Remove(By, RemovedDuration, Time, ParseEnum.BuffRemove.Single);
        }
    }
}
