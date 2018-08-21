namespace LuckParser.Models.ParseModels
{
    public class MobileActor : Mobility
    {

        public MobileActor() : base()
        {
        }

        public override string GetPosition(string id, CombatReplayMap map)
        {
            return "'" + id + "'";
        }
    }
}
