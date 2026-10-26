using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Interfaces;

public interface ITimeComparable
{
    bool IsAtTime(long time, long epsilon = ParserHelper.ServerDelayConstant);
    bool IsAtTime(TimeCombatEvent evt, long epsilon = ParserHelper.ServerDelayConstant);
    bool IsAtTime(CombatItem evt, long epsilon = ParserHelper.ServerDelayConstant);
}
