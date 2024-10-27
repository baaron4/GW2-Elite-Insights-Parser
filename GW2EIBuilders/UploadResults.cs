namespace GW2EIBuilders;

public class UploadResults
{

    public readonly string DPSReportEILink;
    public readonly string RaidarLink;

    public UploadResults()
    {
        DPSReportEILink = "";
        RaidarLink = "";
    }

    /// <summary>
    /// Links to different external websites. Set them to null if absent
    /// </summary>
    /// <param name="dpsReportEI"></param>
    /// <param name="dpsReportRH"></param>
    /// <param name="raidar"></param>
    public UploadResults(string dpsReportEI, string raidar)
    {
        DPSReportEILink = dpsReportEI ?? "";
        RaidarLink = raidar ?? "";
    }

    internal string[] ToArray()
    {
        return [DPSReportEILink, RaidarLink];
    }
}
