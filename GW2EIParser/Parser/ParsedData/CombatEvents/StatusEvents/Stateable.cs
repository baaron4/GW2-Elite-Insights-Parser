namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public interface Stateable
    {
        (long start, double value) ToState();

    }
}
