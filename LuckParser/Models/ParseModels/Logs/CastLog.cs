using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public  class CastLog
    {
        // Fields
        private long _time;
        private long _skillId;
        private int _expectedDuration;
        private int _actualDuration;
        private ParseEnum.Activation _startActivation;
        private ParseEnum.Activation _endActivation;


        // Constructor
        public CastLog(long time, long skillId, int expDuration, ParseEnum.Activation startActivation,int actDur, ParseEnum.Activation endActivation)
        {
            _time = time;
            _skillId = skillId;
            _expectedDuration = expDuration;
            _startActivation = startActivation;
            _actualDuration = actDur;
            _endActivation = endActivation;
        }
        //start cast log
        public CastLog(long time, long skillId, int expDuration, ParseEnum.Activation startActivation)
        {
            _time = time;
            _skillId = skillId;
            _expectedDuration = expDuration;
            _startActivation = startActivation;
            
        }

        // setters
        public void SetEndStatus(int actDuration, ParseEnum.Activation endActivation)
        {
            _actualDuration = actDuration;
            _endActivation = endActivation;
        }

        // Getters
        public long GetTime()
        {
            return _time;
        }
        public long GetID()
        {
            return _skillId;
        }
        public int GetExpDur()
        {
            return _expectedDuration;
        }
        public ParseEnum.Activation StartActivation()
        {
            return _startActivation;
        }
        public int GetActDur()
        {
            return _actualDuration;
        }
        public ParseEnum.Activation EndActivation()
        {
            return _endActivation;
        }
    }
}

