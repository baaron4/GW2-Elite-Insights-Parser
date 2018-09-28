using System;
using System.Collections.Generic;
using LuckParser.Models.ParseModels;

namespace LuckParser.Models.DataModels
{
    class JsonLog
    {
        public string EliteInsightsVersion;
        public int TriggerID;
        public string FightName;
        public string ArcVersion;
        public string RecordedBy;
        public string TimeStart;
        public string TimeEnd;
        public string Duration;
        public bool Success;
        public List<JsonBoss> Boss;
        public List<JsonPlayer> Players;
        public List<JsonPhase> Phases;
        public List<JsonMechanic> Mechanics;
        public string[] UploadLinks;
    }
}