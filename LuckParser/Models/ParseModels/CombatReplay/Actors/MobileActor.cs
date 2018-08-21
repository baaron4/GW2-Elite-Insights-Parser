namespace LuckParser.Models.ParseModels
{
    public class MobileActor : Mobility
    {
        public override string GetPosition(string id, CombatReplayMap map)
        {
            return "'" + id + "'";
        }
    }
}
