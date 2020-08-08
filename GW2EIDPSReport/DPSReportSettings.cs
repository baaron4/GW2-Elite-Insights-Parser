namespace GW2EIDPSReport
{
    public class DPSReportSettings
    {
        public string UserToken { get; }

        public DPSReportSettings(string userToken)
        {
            UserToken = userToken;
        }

        public DPSReportSettings() : this(null)
        {
        }
    }
}
