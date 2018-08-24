using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public abstract class DamageLog
    {
        // Fields
        private readonly long _time;
        protected int Damage;
        private readonly long _skillId;
        private readonly int _buff;
        private readonly ParseEnum.Result _result;
        private readonly ushort _isNinety;
        private readonly ushort _isMoving;
        private readonly ushort _isFlanking;
        private readonly ParseEnum.Activation _isActivation;
        private readonly ushort _isShields;
        private readonly ulong _srcAgent;
        private readonly ushort _srcInstid;
        private readonly ulong _dstAgent;
        private readonly ushort _dstInstid;

        // Constructor
        protected DamageLog(long time, CombatItem c)
        {
            _time = time;
            _skillId = c.GetSkillID();
            _buff = c.IsBuff();
            _result = c.GetResult();
            _isNinety = c.IsNinety();
            _isMoving = c.IsMoving();
            _isFlanking = c.IsFlanking();
            _isActivation = c.IsActivation();
            _srcAgent = c.GetSrcAgent();
            _srcInstid = c.GetSrcInstid();
            _isShields = c.IsShields();
            _dstAgent = c.GetDstAgent();
            _dstInstid = c.GetDstInstid();

        }
        // Getters
        public long GetTime()
        {
            return _time;
        }

        public int GetDamage()
        {
            return Damage;
        }

        public long GetID()
        {
            return _skillId;
        }

        public int IsCondi()
        {
            return _buff;
        }

        public ParseEnum.Result GetResult()
        {
            return _result;
        }

        public ushort IsNinety()
        {
            return _isNinety;
        }

        public ushort IsMoving()
        {
            return _isMoving;
        }

        public ushort IsFlanking()
        {
            return _isFlanking;
        }
        public ParseEnum.Activation IsActivation()
        {
            return _isActivation;
        }
        public ushort IsShields() {
            return _isShields;
        }
        public ulong GetSrcAgent()
        {
            return _srcAgent;
        }
        public ushort GetSrcInstidt()
        {
            return _srcInstid;
        }

        public ulong GetDstAgent()
        {
            return _dstAgent;
        }
        public ushort GetDstInstidt()
        {
            return _dstInstid;
        }
    }
}