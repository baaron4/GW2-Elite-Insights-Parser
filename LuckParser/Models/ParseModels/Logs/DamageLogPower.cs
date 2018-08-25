namespace LuckParser.Models.ParseModels
{
    public class DamageLogPower : DamageLog
    {

        // Constructor
        public DamageLogPower(long time, CombatItem c) : base(time, c)
        {
            Damage =  c.Value;
        }
    }
}