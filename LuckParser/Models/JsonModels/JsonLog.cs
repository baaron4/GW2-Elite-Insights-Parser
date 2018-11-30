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
        public int Success;
        public List<JsonTarget> Targets;
        public List<JsonPlayer> Players;
        public List<JsonPhase> Phases;
        public Dictionary<string, List<JsonMechanic>> Mechanics;
        public string[] UploadLinks;
        public Dictionary<long, string> SkillNames;
        public Dictionary<long, string> BuffNames;
        public Dictionary<string, HashSet<long>> PersonalBuffs;
    }
}