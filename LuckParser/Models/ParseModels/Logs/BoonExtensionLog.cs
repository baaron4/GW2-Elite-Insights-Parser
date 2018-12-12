using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public class BoonExtensionLog : BoonLog
    {

        public BoonExtensionLog(long time, ushort srcInstid, long value) : base(time, srcInstid, value)
        {
        }

        public override void UpdateSimulator(BoonSimulator simulator)
        {
            simulator.Extend(Value, SrcInstid);
        }
    }
}