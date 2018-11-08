using System;

namespace LuckParser.Models.ParseModels
{
    public class LogData
    {
        // Fields
        public readonly string BuildVersion;
        public string PoV { get; private set; } = "N/A";
        public string LogStart { get; private set; } = "yyyy-MM-dd HH:mm:ss z";
        public string LogEnd { get; private set; } = "yyyy-MM-dd HH:mm:ss z";
        public long EncounterLength { get; set; } = 0;
        public bool Success { get; set; }
       // private SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss z");

        // Constructors
        public LogData(string buildVersion)
        {
            BuildVersion = buildVersion;
           // this.sdf.setTimeZone(TimeZone.getDefault());
        }

        // Public Methods
        public string[] ToStringArray()
        {
            string[] array = new string[4];
            array[0] = BuildVersion;
            array[1] = PoV;
            array[2] = LogStart;
            array[3] = LogEnd;
            return array;
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