using LuckParser.Parser;

namespace LuckParser.Models.ParseModels
{
    public class BoonRemovalLog : BoonLog
    {
        private readonly ParseEnum.BuffRemove _removeType;

        public BoonRemovalLog(long time, ushort srcInstid, long value, ParseEnum.BuffRemove removeType) : base(time, srcInstid, value)
        {
            _removeType = removeType;
        }

        public override void UpdateSimulator(BoonSimulator simulator)
        {
            simulator.Remove(SrcInstid,Value,Time, _removeType);
        }
    }
}