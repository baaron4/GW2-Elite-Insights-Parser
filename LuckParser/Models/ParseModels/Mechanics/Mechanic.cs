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
        // Fields

        private readonly string _description;
        private readonly string _plotlyName;

        public long SkillId { get; }
        public MechType MechanicType { get; }
        public ParseEnum.BossIDS BossID { get; }

        public int InternalCooldown { get; }
        public CheckSpecialCondition SpecialCondition { get; }

        public string PlotlyShape { get; }
        public string Description => _description ?? InGameName;
        public string PlotlyName => _plotlyName ?? InGameName;
        public string InGameName { get; }
        public string ShortName { get; }

        public Mechanic(long skillId, string inGameName, MechType mechtype, ParseEnum.BossIDS bossid, string plotlyShape, string shortName, int internalCoolDown, CheckSpecialCondition condition = null)
        {
            InGameName = inGameName;
            SkillId = skillId;
            MechanicType = mechtype;
            BossID = bossid;
            PlotlyShape = plotlyShape;
            ShortName = shortName;
            _description = null;
            _plotlyName = null;
            InternalCooldown = internalCoolDown;
            SpecialCondition = condition;
        }

        public Mechanic(long skillId,string inGameName, MechType mechtype, ParseEnum.BossIDS bossid, string plotlyShape,string shortName, string description, string plotlyName, int internalCoolDown, CheckSpecialCondition condition = null)
        {
            InGameName = inGameName;
            SkillId = skillId;
            MechanicType = mechtype;
            BossID = bossid;
            PlotlyShape = plotlyShape;
            ShortName = shortName;
            _description = description;
            _plotlyName = plotlyName;
            InternalCooldown = internalCoolDown;
            SpecialCondition = condition;
        }

        public bool IsEnemyMechanic => MechanicType == MechType.EnemyBoon || MechanicType == MechType.EnemyBoonStrip ||
                                       MechanicType == MechType.EnemyCastEnd || MechanicType == MechType.EnemyCastStart ||
                                       MechanicType == MechType.Spawn;
    }
}
