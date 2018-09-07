namespace LuckParser.Models.ParseModels
{
    public class DamageLogCondition : DamageLog
    {
        public DamageLogCondition(long time, CombatItem c) : base(time, c)
        {
            Damage = c.BuffDmg;
        }
    }
}