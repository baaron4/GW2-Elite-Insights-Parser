namespace LuckParser.Models.ParseModels
{
    public class DamageLogCondition : DamageLog
    {

        // Constructor
        public DamageLogCondition(long time, CombatItem c) : base(time, c)
        {
            _damage = c.GetBuffDmg();
        }
    }
}