namespace GW2EIBuilders;

public class UploadResults
{

    public readonly string DPSReportEILink;

    public UploadResults()
    {
        DPSReportEILink = "";
    }

    /// <summary>
    /// Links to different external websites. Set them to null if absent
    /// </summary>
    /// <param name="dpsReportEI"></param>
    public UploadResults(string dpsReportEI)
    {
        DPSReportEILink = dpsReportEI ?? "";
    }
}
