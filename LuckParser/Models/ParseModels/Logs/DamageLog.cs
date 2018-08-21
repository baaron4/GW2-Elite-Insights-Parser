using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public abstract class DamageLog
    {
        // Fields
        private long _time;
        protected int _damage;
        private long _skillId;
        private int _buff;
        private ParseEnum.Result _result;
        private ushort _isNinety;
        private ushort _isMoving;
        private ushort _isFlanking;
        private ParseEnum.Activation _isActivation;
        private ushort _isShields;
        private ulong _srcAgent;
        private ushort _srcInstid;
        private ulong _dstAgent;
        private ushort _dstInstid;

        // Constructor
        public DamageLog(long time, CombatItem c)
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
            return _damage;
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