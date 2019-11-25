namespace GW2EIParser.EIData
{
    // to match non generic support stats
    public class FinalPlayerSupport
    {
        //public long allHeal;
        public long Resurrects { get; set; }
        public double ResurrectTime { get; set; }
        public long CondiCleanse { get; set; }
        public double CondiCleanseTime { get; set; }
        public long CondiCleanseSelf { get; set; }
        public double CondiCleanseTimeSelf { get; set; }
        public long BoonStrips { get; set; }
        public double BoonStripsTime { get; set; }
    }
}
