namespace GW2EIEvtcParser.EIData
{
    // to match non generic support stats
    public class FinalPlayerSupport
    {
        //public long allHeal;
        public int Resurrects { get; internal set; }
        public double ResurrectTime { get; internal set; }
        public int CondiCleanse { get; internal set; }
        public double CondiCleanseTime { get; internal set; }
        public int CondiCleanseSelf { get; internal set; }
        public double CondiCleanseTimeSelf { get; internal set; }
        public int BoonStrips { get; internal set; }
        public double BoonStripsTime { get; internal set; }
    }
}
