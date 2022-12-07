using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Interfaces
{
    internal interface IVersionable
    {
        bool Available(CombatData combatData);
    }
}
