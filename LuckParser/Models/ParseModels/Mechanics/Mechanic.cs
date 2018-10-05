using LuckParser.Controllers;
using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public class SpecialConditionItem
    {
        public DamageLog DamageLog { get; }
        public CombatItem CombatItem { get; }

        //covers the special conditions that one might want to check when tracking mechanics
        public SpecialConditionItem(DamageLog damageLog)
        {
            DamageLog = damageLog;
            CombatItem = null;
        }

        public SpecialConditionItem(CombatItem combatItem)
        {
            DamageLog = null;
            CombatItem = combatItem;
        }
    }

    public class Mechanic
    {
        /// <summary>
        /// PlayerBoon 
        /// BossBoon 
        /// PlayerSkill
        /// SkillOnPlayer
        /// EnemyBoonStrip
        /// Spawn
        /// BossCast
        /// PlayerOnPlayer
        /// HitOnEnemy
        /// PlayerStatus
        /// EnemyCastStart
        /// EnemyCastEnd -> put skill id the same but negative so that you can also track the Start of the same skill if you want
        /// </summary>
        public enum MechType { PlayerBoon, PlayerBoonRemove, EnemyBoon, SkillOnPlayer, PlayerSkill, EnemyBoonStrip, Spawn, PlayerOnPlayer, HitOnEnemy, PlayerStatus, EnemyCastStart, EnemyCastEnd }

        public delegate bool CheckSpecialCondition(SpecialConditionItem conditionItem);

        public long SkillId { get; }
        public MechType MechanicType { get; }
        public ParseEnum.BossIDS BossID { get; }

        public int InternalCooldown { get; }
        public CheckSpecialCondition SpecialCondition { get; }

        public string PlotlyString { get; }
        public string PlotlySymbol { get; }
        public string PlotlyColor { get; }
        public string Description { get; }
        public string PlotlyName { get; }
        public string InGameName { get; }
        public string ShortName { get; }
        public bool IsEnemyMechanic { get; }

        public Mechanic(long skillId, string inGameName, MechType mechType, ParseEnum.BossIDS bossId,
            string plotlyString, string shortName, int internalCoolDown, CheckSpecialCondition condition = null) :
            this(skillId, inGameName, mechType, bossId, plotlyString, shortName, null, null, internalCoolDown, condition)
        {
        }

        public Mechanic(long skillId, string inGameName, MechType mechType, ParseEnum.BossIDS bossId,
            string plotlyString, string shortName, string description, string plotlyName, int internalCoolDown,
            CheckSpecialCondition condition = null)
        {
            InGameName = inGameName;
            SkillId = skillId;
            MechanicType = mechType;
            BossID = bossId;
            PlotlyString = plotlyString;
            PlotlySymbol = GeneralHelper.FindPattern(PlotlyString, "symbol\\s*:\\s*'([^']*)'");
            PlotlyColor= GeneralHelper.FindPattern(PlotlyString, "color\\s*:\\s*'([^']*)'");
            ShortName = shortName;
            Description = description ?? InGameName;
            PlotlyName = plotlyName ?? InGameName;
            InternalCooldown = internalCoolDown;
            SpecialCondition = condition;
            IsEnemyMechanic = MechanicType == MechType.EnemyBoon || MechanicType == MechType.EnemyBoonStrip ||
                              MechanicType == MechType.EnemyCastEnd || MechanicType == MechType.EnemyCastStart ||
                              MechanicType == MechType.Spawn;
        }

    }
}
