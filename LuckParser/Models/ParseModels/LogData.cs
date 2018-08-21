using System;

namespace LuckParser.Models.ParseModels
{
    public class LogData
    {
        // Fields
        private String _buildVersion;
        private String _pov = "N/A";
        private String _logStart = "yyyy-MM-dd HH:mm:ss z";
        private String _logEnd = "yyyy-MM-dd HH:mm:ss z";
        private bool _bossKill = false;
       // private SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss z");

        // Constructors
        public LogData(String buildVersion)
        {
            _buildVersion = buildVersion;
           // this.sdf.setTimeZone(TimeZone.getDefault());
        }

        // Public Methods
        public String[] ToStringArray()
        {
            String[] array = new String[4];
            array[0] = _buildVersion.ToString();
            array[1] = _pov.ToString();
            array[2] = _logStart.ToString();
            array[3] = _logEnd.ToString();
            return array;
        }

        // Getters
        public String GetBuildVersion()
        {
            return _buildVersion;
        }

        public String GetPOV()
        {
            return _pov;
        }

        public String GetLogStart()
        {
            return _logStart;
        }

        public String GetLogEnd()
        {
            return _logEnd;
        }
        public bool GetBosskill() {
            return _bossKill;
        }
        // Setters
        public void SetPOV(String pov)
        {
            _pov = pov.Substring(0, pov.LastIndexOf('\0'));
        }

        public void SetLogStart(long unixSeconds)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixSeconds).ToLocalTime();
            _logStart = dtDateTime.ToString("yyyy-MM-dd HH:mm:ss z");
        }

        public void SetLogEnd(long unixSeconds)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixSeconds).ToLocalTime();
            _logEnd = dtDateTime.ToString("yyyy-MM-dd HH:mm:ss z");;
        }
        public void SetBossKill(bool killed) {
            _bossKill = killed;
        }
    }
}