using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;

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

    public class MechanicPlotlySetting
    {
        public string Color { get; }
        public int Size { get; }
        public string Symbol { get; }

        public MechanicPlotlySetting(string symbol, string color, int size = 15)
        {
            Color = color;
            Symbol = symbol;
            Size = size;
        }

    }

    public class Mechanic
    {
        /// <summary>
        /// PlayerBoon 
        /// TargetBoon 
        /// PlayerSkill
        /// SkillOnPlayer
        /// EnemyBoonStrip
        /// Spawn
        /// TargetCast
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

        public int InternalCooldown { get; }
        public CheckSpecialCondition SpecialCondition { get; }
        public MechanicPlotlySetting PlotlySetting { get; }
        public string Description { get; }
        public string InGameName { get; }
        public string ShortName { get; }
        public string FullName { get; }
        public bool IsEnemyMechanic { get; }

        public Mechanic(long skillId, string inGameName, MechType mechType, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, CheckSpecialCondition condition = null) : this(skillId, inGameName, mechType, plotlySetting, shortName, shortName, shortName, internalCoolDown, null)
        {
        }

        public Mechanic(long skillId, string inGameName, MechType mechType, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CheckSpecialCondition condition = null)
        {
            InGameName = inGameName;
            SkillId = skillId;
            MechanicType = mechType;
            PlotlySetting = plotlySetting;
            ShortName = shortName;
            FullName = fullName;
            Description = description;
            InternalCooldown = internalCoolDown;
            SpecialCondition = condition;
            IsEnemyMechanic = MechanicType == MechType.EnemyBoon || MechanicType == MechType.EnemyBoonStrip ||
                              MechanicType == MechType.EnemyCastEnd || MechanicType == MechType.EnemyCastStart ||
                              MechanicType == MechType.Spawn;
        }

    }
}
