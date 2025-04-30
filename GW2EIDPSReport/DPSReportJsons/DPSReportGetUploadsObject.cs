
namespace GW2EIDPSReport.DPSReportJsons;

public class DPSReportGetUploadsObject
{
    public int? Pages;
    public uint? CurrentTime;
    public int? FoundUploads;
    public int? TotalUploads;
    public string? UserToken;
    public DPSReportUploadObject[]? Uploads;
}
