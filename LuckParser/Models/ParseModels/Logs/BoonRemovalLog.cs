using LuckParser.Parser;

namespace LuckParser.Models.ParseModels
{
    public class BoonRemovalLog : BoonLog
    {
        private readonly ParseEnum.BuffRemove _removeType;

        public BoonRemovalLog(long time, AgentItem src, long value, ParseEnum.BuffRemove removeType) : base(time, src, value)
        {
            _removeType = removeType;
        }

        public override void UpdateSimulator(BoonSimulator simulator)
        {
            simulator.Remove(Src,Value,Time, _removeType);
        }
    }
}