using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
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

        public delegate bool SpecialCondition(long value);
        // Fields
        
        private readonly string _inGameName;
        private readonly string _shortName;
        private readonly string _description;
        private readonly string _plotlyName;
        private readonly MechType _mechType;
        private readonly long _skillId;
        private readonly ParseEnum.BossIDS _bossid;
        private readonly string _plotlyShape;
        private readonly int _internalCoolDown;
        private readonly SpecialCondition _condition;


        public Mechanic(long skillId, string inGameName, MechType mechtype, ParseEnum.BossIDS bossid, string plotlyShape, string shortName, int internalCoolDown, SpecialCondition condition = null)
        {
            _inGameName = inGameName.Replace("\""," ").Replace("'"," ");
            _skillId = skillId;
            _mechType = mechtype;
            _bossid = bossid;
            _plotlyShape = plotlyShape;
            _shortName = shortName.Replace("\"", " ").Replace("'", " ");
            _description = null;
            _plotlyName = null;
            _internalCoolDown = internalCoolDown;
            _condition = condition;
        }

        public Mechanic(long skillId,string inGameName, MechType mechtype, ParseEnum.BossIDS bossid, string plotlyShape,string shortName, string description, string plotlyName, int internalCoolDown, SpecialCondition condition = null)
        {
            _inGameName = inGameName.Replace("\"", " ").Replace("'", " ");
            _skillId = skillId;
            _mechType = mechtype;
            _bossid = bossid;
            _plotlyShape = plotlyShape;
            _shortName = shortName.Replace("\"", " ").Replace("'", " ");
            _description = description.Replace("\"", " ").Replace("'", " ");
            _plotlyName = plotlyName.Replace("\"", " ").Replace("'", " ");
            _internalCoolDown = internalCoolDown;
            _condition = condition;
        }
        //getters
        public SpecialCondition GetSpecialCondition()
        {
            return _condition;
        }
        public string GetInGameName()
        {
            return _inGameName;
        }
        public long GetSkill()
        {
            return _skillId;
        }
        public MechType GetMechType()
        {
            return _mechType;
        }
        public ParseEnum.BossIDS GetBossID() {
            return _bossid;
        }

        public string GetPlotly()
        {
            return _plotlyShape;
        }
        public string GetShortName()
        {
            return _shortName;
        }
        public string GetDescription()
        {
            return _description ?? _inGameName;
        }
        public string GetPlotlyName()
        {
            return _plotlyName ?? _inGameName;
        }
        public int GetICD()
        {
            return _internalCoolDown;
        }

        public bool IsEnemyMechanic()
        {
            return _mechType == MechType.EnemyBoon || _mechType == MechType.EnemyBoonStrip || _mechType == MechType.EnemyCastEnd || _mechType == MechType.EnemyCastStart || _mechType == MechType.Spawn;
        }      
    }
}
