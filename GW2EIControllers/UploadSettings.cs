namespace GW2EIControllers
{
    public class UploadSettings
    {
        public bool UploadToDPSReportsUsingEI { get; }
        public bool UploadToDPSReportsUsingRH { get; }
        public bool UploadToRaidar { get; }

        public UploadSettings(bool uploadToDPSReportsUsingEI, bool uploadToDPSReportsUsingRH)
        {
            UploadToDPSReportsUsingEI = uploadToDPSReportsUsingEI;
            UploadToDPSReportsUsingRH = uploadToDPSReportsUsingRH;
            UploadToRaidar = false;
        }

        public UploadSettings(bool uploadToDPSReportsUsingEI) : this(uploadToDPSReportsUsingEI, false)
        {
        }
    }
}
