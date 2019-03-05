using System;

namespace LuckParser.Models.ParseModels
{
    public class LogData
    {
        // Fields
        public readonly string BuildVersion;
        public ulong GW2Version { get; set; }
        public string PoV { get; private set; } = "N/A";
        public string LogStart { get; private set; } = "yyyy-MM-dd HH:mm:ss z";
        public string LogEnd { get; private set; } = "yyyy-MM-dd HH:mm:ss z";
       // private SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss z");

        // Constructors
        public LogData(string buildVersion)
        {
            BuildVersion = buildVersion;
           // this.sdf.setTimeZone(TimeZone.getDefault());
        }
        
        // Setters
        public void SetPOV(string pov)
        {
            PoV = pov.Substring(0, pov.LastIndexOf('\0'));
        }

        public void SetLogStart(long unixSeconds)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixSeconds).ToLocalTime();
            LogStart = dtDateTime.ToString("yyyy-MM-dd HH:mm:ss z");
        }

        public void SetLogEnd(long unixSeconds)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixSeconds).ToLocalTime();
            LogEnd = dtDateTime.ToString("yyyy-MM-dd HH:mm:ss z");
        }
    }
}