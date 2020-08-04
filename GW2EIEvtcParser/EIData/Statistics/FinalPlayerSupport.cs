namespace GW2EIEvtcParser.EIData
{
    // to match non generic support stats
    public class FinalPlayerSupport
    {
        //public long allHeal;
        public int Resurrects { get; set; }
        public double ResurrectTime { get; set; }
        public int CondiCleanse { get; set; }
        public double CondiCleanseTime { get; set; }
        public int CondiCleanseSelf { get; set; }
        public double CondiCleanseTimeSelf { get; set; }
        public int BoonStrips { get; set; }
        public double BoonStripsTime { get; set; }
    }
}
