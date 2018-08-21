using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public class CombatItem
    {

        // Fields
        private long _time;
        private ulong _srcAgent;
        private ulong _dstAgent;
        private int _value;
        private int _buffDmg;
        private uint _overstackValue;
        private long _skillId;
        private ushort _srcInstid;
        private ushort _dstInstid;
        private ushort _srcMasterInstid;
        private ushort _dstMasterInstid;
        private ParseEnum.IFF _iff;
        private ushort _isBuff;
        private ParseEnum.Result _result;
        private ParseEnum.Activation _isActivation;
        private ParseEnum.BuffRemove _isBuffRemove;
        private ushort _isNinety;
        private ushort _isFifty;
        private ushort _isMoving;
        private ParseEnum.StateChange _isStateChange;
        private ushort _isFlanking;
        private ushort _isShields;
        // Constructor        
        public CombatItem(long time, ulong srcAgent, ulong dstAgent, int value, int buffDmg, uint overstackValue,
               long skillId, ushort srcInstid, ushort dstInstid, ushort srcMasterInstid, ushort dstMasterInstid, ParseEnum.IFF iff, ushort isBuff, ParseEnum.Result result,
               ParseEnum.Activation isActivation, ParseEnum.BuffRemove isBuffRemove, ushort isNinety, ushort isFifty, ushort isMoving,
               ParseEnum.StateChange isStateChange, ushort isFlanking, ushort isShields)
        {
            _time = time;
            _srcAgent = srcAgent;
            _dstAgent = dstAgent;
            _value = value;
            _buffDmg = buffDmg;
            _overstackValue = overstackValue;
            _skillId = skillId;
            _srcInstid = srcInstid;
            _dstInstid = dstInstid;
            _srcMasterInstid = srcMasterInstid;
            _dstMasterInstid = dstMasterInstid;
            _iff = iff;
            _isBuff = isBuff;
            _result = result;
            _isActivation = isActivation;
            _isBuffRemove = isBuffRemove;
            _isNinety = isNinety;
            _isFifty = isFifty;
            _isMoving = isMoving;
            _isStateChange = isStateChange;
            _isFlanking = isFlanking;
            _isShields = isShields;
        }

        // Getters
        public long GetTime()
        {
            return _time;
        }

        public ulong GetSrcAgent()
        {
            return _srcAgent;
        }

        public ulong GetDstAgent()
        {
            return _dstAgent;
        }

        public int GetValue()
        {
            return _value;
        }

        public int GetBuffDmg()
        {
            return _buffDmg;
        }

        public uint GetOverstackValue()
        {
            return _overstackValue;
        }

        public long GetSkillID()
        {
            return _skillId;
        }

        public ushort GetSrcInstid()
        {
            return _srcInstid;
        }

        public ushort GetDstInstid()
        {
            return _dstInstid;
        }

        public ushort GetSrcMasterInstid()
        {
            return _srcMasterInstid;
        }

        public ushort GetDstMasterInstid()
        {
            return _dstMasterInstid;
        }

        public ParseEnum.IFF GetIFF()
        {
            return _iff;
        }

        public ushort IsBuff()
        {
            return _isBuff;
        }

        public ParseEnum.Result GetResult()
        {
            return _result;
        }

        public ParseEnum.Activation IsActivation()
        {
            return _isActivation;
        }

        public ParseEnum.BuffRemove IsBuffremove()
        {
            return _isBuffRemove;
        }

        public ushort IsNinety()
        {
            return _isNinety;
        }

        public ushort IsFifty()
        {
            return _isFifty;
        }

        public ushort IsMoving()
        {
            return _isMoving;
        }

        public ushort IsFlanking()
        {
            return _isFlanking;
        }

        public ParseEnum.StateChange IsStateChange()
        {
            return _isStateChange;
        }
        public ushort IsShields()
        {
            return _isShields;
        }
        // Setters
        public void SetSrcAgent(ulong srcAgent)
        {
            _srcAgent = srcAgent;
        }

        public void SetDstAgent(ulong dstAgent)
        {
            _dstAgent = dstAgent;
        }

        public void SetSrcInstid(ushort srcInstid)
        {
            _srcInstid = srcInstid;
        }

        public void SetDstInstid(ushort dstInstid)
        {
            _dstInstid = dstInstid;
        }
    }
}