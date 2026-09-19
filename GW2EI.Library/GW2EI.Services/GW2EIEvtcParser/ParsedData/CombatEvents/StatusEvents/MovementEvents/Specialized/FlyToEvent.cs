using System.Numerics;
using GW2EIEvtcParser.ParserHelpers;

namespace GW2EIEvtcParser.ParsedData;

public class FlyToEvent : StatusEvent
{
    private const float ConvertConstant = 10.0f;

    public readonly bool OnLanding;

    public readonly Vector3 TargetPosition;
    public readonly float Speed;

    private FlyToEvent? Landing;

    public long LandingTime => Landing?.Time ?? Time;
    internal FlyToEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        OnLanding = evtcItem.IsFlanking == 0;
        if (OnLanding)
        {
            Landing = this;
        }// Vectors
        var vectorBytes = new ByteBuffer(stackalloc byte[6 * sizeof(short)]);
        // 2 
        vectorBytes.PushNative(evtcItem.DstAgent);
        unsafe
        {
            fixed (byte* ptr = vectorBytes.Span)
            {
                var vectorShorts = (short*)ptr;
                TargetPosition = new(
                        vectorShorts[0] * ConvertConstant,
                        vectorShorts[1] * ConvertConstant,
                        vectorShorts[2] * ConvertConstant
                    );
                Speed = vectorShorts[4] * ConvertConstant;
            }
        }
    }

    internal bool SetLanding(FlyToEvent landing)
    {
        if (Landing == null)
        {
            Landing = landing;
            return true;
        }
        return false;
    }

}
