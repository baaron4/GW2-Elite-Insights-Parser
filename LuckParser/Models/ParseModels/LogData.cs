using System;

namespace LuckParser.Models.ParseModels
{
    public class LogData
    {
        public static readonly string DefaultTimeValue = "MISSING";

        // Fields
        public readonly string BuildVersion;
        public ulong GW2Version { get; set; }
        public string PoV { get; private set; } = "N/A";
        private readonly string _dateFormat = "yyyy-MM-dd HH:mm:ss zz";
        public string LogStart { get; private set; }
        public long LogStartUnixSeconds { get; private set; }
        public string LogEnd { get; private set; }
        public long LogEndUnixSeconds { get; private set; }
        // private SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss z");

        // Constructors
        public LogData(string buildVersion)
        {
            BuildVersion = buildVersion;
            LogStart = DefaultTimeValue;
            LogEnd = DefaultTimeValue;
           // this.sdf.setTimeZone(TimeZone.getDefault());
        }
        
        // Setters
        public void SetPOV(string pov)
        {
            PoV = pov.Substring(0, pov.LastIndexOf('\0'));
        }

        private string GetDateTime(long unixSeconds)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixSeconds).ToLocalTime();
            return dtDateTime.ToString(_dateFormat);
        }

        public void SetLogStart(long unixSeconds)
        {
            LogStartUnixSeconds = unixSeconds;
            LogStart = GetDateTime(unixSeconds);
        }

        public void SetLogEnd(long unixSeconds)
        {
            LogEndUnixSeconds = unixSeconds;
            LogEnd = GetDateTime(unixSeconds);
        }
    }
}