using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class LogData
    {
        // Fields
        private String build_version;
        private String pov = "N/A";
        private String log_start = "yyyy-MM-dd HH:mm:ss z";
        private String log_end = "yyyy-MM-dd HH:mm:ss z";
        private bool boss_kill = false;
       // private SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss z");

        // Constructors
        public LogData(String build_version)
        {
            this.build_version = build_version;
           // this.sdf.setTimeZone(TimeZone.getDefault());
        }

        // Public Methods
        public String[] toStringArray()
        {
            String[] array = new String[4];
            array[0] = build_version.ToString();
            array[1] = pov.ToString();
            array[2] = log_start.ToString();
            array[3] = log_end.ToString();
            return array;
        }

        // Getters
        public String getBuildVersion()
        {
            return build_version;
        }

        public String getPOV()
        {
            return pov;
        }

        public String getLogStart()
        {
            return log_start;
        }

        public String getLogEnd()
        {
            return log_end;
        }
        public bool getBosskill() {
            return boss_kill;
        }
        // Setters
        public void setPOV(String pov)
        {
            this.pov = pov.Substring(0, pov.LastIndexOf('\0'));
        }

        public void setLogStart(int unix_seconds)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unix_seconds).ToLocalTime();
            this.log_start = dtDateTime.ToString("yyyy-MM-dd HH:mm:ss z");//sdf.format(new Date(unix_seconds * 1000L));
        }

        public void setLogEnd(int unix_seconds)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unix_seconds).ToLocalTime();
            this.log_end = dtDateTime.ToString("yyyy-MM-dd HH:mm:ss z"); //sdf.format(new Date(unix_seconds * 1000L));
        }
        public void setBossKill(bool killed) {
            boss_kill = killed;
        }
    }
}