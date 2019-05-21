using LuckParser.Parser;

namespace LuckParser.Models.ParseModels
{
    public  class CastLog
    {
        public long Time { get; }
        public long SkillId { get; }
        public int ExpectedDuration { get; }
        public int ActualDuration { get; private set; }
        public ulong SrcAgent { get; }
        public ushort SrcInstId { get; }
        public ParseEnum.Activation StartActivation { get; }
        public ParseEnum.Activation EndActivation { get; private set; }

        /*public CastLog(long time, long skillId, int expDuration, ParseEnum.Activation startActivation,int actDur, ParseEnum.Activation endActivation, ulong srcAgent, ushort srcInstId)
        {
            Time = time;
            SkillId = skillId;
            ExpectedDuration = expDuration;
            StartActivation = startActivation;
            ActualDuration = actDur;
            EndActivation = endActivation;
            SrcAgent = srcAgent;
            SrcInstId = srcInstId;
        }*/

        public CastLog(long time, long skillId, int expDuration, ParseEnum.Activation startActivation, ulong srcAgent, ushort srcInstId)
        {
            Time = time;
            SkillId = skillId;
            ExpectedDuration = expDuration;
            StartActivation = startActivation;
            SrcAgent = srcAgent;
            SrcInstId = srcInstId;
        }

        public void SetEndStatus(int actDuration, ParseEnum.Activation endActivation, long end)
        {
            ActualDuration = actDuration;
            if (Time + ActualDuration > end)
            {
                ActualDuration = (int)(end - Time);
                EndActivation = ParseEnum.Activation.CancelCancel;
            } else
            {
                EndActivation = endActivation;
            }
        }
    }
}

