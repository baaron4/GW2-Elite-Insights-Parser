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

        public static AbstractMovementEvent CreateMovementEvent(CombatItem c, AgentData agentData, long offset)
        {
            switch(c.IsStateChange)
            {
                case Parser.ParseEnum.StateChange.Velocity:
                    return new VelocityEvent(c, agentData, offset);
                case Parser.ParseEnum.StateChange.Rotation:
                    return new RotationEvent(c, agentData, offset);
                case Parser.ParseEnum.StateChange.Position:
                    return new PositionEvent(c, agentData, offset);
                default:
                    throw new InvalidOperationException("Invalid state change in CreateMovementEvent");
            }
        }

        public static AbstractDamageEvent CreateDamageEvent(CombatItem c, AgentData agentData, BoonsContainer boons, long offset)
        {
            if ((c.IsBuff != 0 && c.Value == 0))
            {
                return new NonDirectDamageEvent(c, agentData, boons, offset);
            }
            else if (c.IsBuff == 0)
            {
                return new DirectDamageEvent(c, agentData, offset);
            }
            throw new InvalidOperationException("Invalid data in CreateMovementEvent");
        }

    }
}
