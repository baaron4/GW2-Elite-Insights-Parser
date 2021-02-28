namespace GW2EIBuilders
{
    public class UploadResults
    {

        public string DPSReportEILink { get; }
        public string DPSReportRHLink { get; }
        public string RaidarLink { get; }

        public UploadResults()
        {
            DPSReportEILink = "";
            RaidarLink = "";
            DPSReportRHLink = "";
        }

        /// <summary>
        /// Links to different external websites. Set them to null if absent
        /// </summary>
        /// <param name="dpsReportEI"></param>
        /// <param name="dpsReportRH"></param>
        /// <param name="raidar"></param>
        public UploadResults(string dpsReportEI, string dpsReportRH, string raidar)
        {
            DPSReportEILink = dpsReportEI ?? "";
            DPSReportRHLink = dpsReportRH ?? "";
            RaidarLink = raidar ?? "";
        }

        internal string[] ToArray()
        {
            return new string[] { DPSReportEILink, DPSReportRHLink, RaidarLink };
        }
    }
}
