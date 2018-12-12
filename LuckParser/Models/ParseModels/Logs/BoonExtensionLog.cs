using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public class BoonExtensionLog : BoonLog
    {

        private readonly long _oldValue;

        public BoonExtensionLog(long time, long value, long oldValue) : base(time, 0, value)
        {
            _oldValue = oldValue;
        }

        public override void UpdateSimulator(BoonSimulator simulator)
        {
            simulator.Extend(Value, _oldValue);
        }
    }
}