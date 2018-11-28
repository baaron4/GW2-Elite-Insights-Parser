using LuckParser.Controllers;
using LuckParser.Models.DataModels;
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
        public string color { get; }
        public int size { get; }
        public string symbol { get; }

        public MechanicPlotlySetting(string symbol, string color, int size = 15)
        {
            this.color = color;
            this.symbol = symbol;
            this.size = size;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
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
        public string PlotlyJson => PlotlySetting.ToJson();
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
            InternalCooldown = internalCoolDown;
            SpecialCondition = condition;
            IsEnemyMechanic = MechanicType == MechType.EnemyBoon || MechanicType == MechType.EnemyBoonStrip ||
                              MechanicType == MechType.EnemyCastEnd || MechanicType == MechType.EnemyCastStart ||
                              MechanicType == MechType.Spawn;
        }

    }
}
