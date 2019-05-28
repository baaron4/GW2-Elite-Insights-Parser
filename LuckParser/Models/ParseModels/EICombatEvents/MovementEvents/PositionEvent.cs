using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class PositionEvent : AbstractMovementEvent
    {

        public PositionEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
        }

        public override void AddPoint3D(CombatReplay replay, ParsedLog log)
        {
            long time = log.FightData.ToFightSpace(Time);
            (float x, float y, float z) = Unpack();
            replay.Positions.Add(new Point3D(x, y, z, time));

        }
    }
}
