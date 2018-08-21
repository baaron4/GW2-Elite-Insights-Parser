namespace LuckParser.Models.ParseModels
{
    public abstract class Mobility
    {
        public abstract string GetPosition(string id, CombatReplayMap map);
    }
}
