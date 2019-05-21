namespace LuckParser.Models.ParseModels
{
    public class DamageLogCondition : DamageLog
    {
        public DamageLogCondition(long time, CombatItem c, BoonsContainer boons) : base(time, c)
        {
            Damage = c.BuffDmg;
            IsIndirectDamage = true;
            if (boons.BoonsByIds.TryGetValue(c.SkillID, out Boon boon))
            {
                IsCondi = (boon.Nature == Boon.BoonNature.Condition);
            }
        }
    }
}