using System;
using System.Collections.Generic;
using LuckParser.Models.ParseModels;

namespace LuckParser.Models.DataModels
{
    class JsonLog
    {

        public struct MechanicDesc
        {
            public string PlotlyShape { get; }
            public string Description { get; }
            public string PlotlyName { get; }
            public string InGameName { get; }
        }

        public class JsonExtraLog
        {
            public Dictionary<string, string> BuffIcons;
            public Dictionary<long, string> SkillIcons;
            public string FightIcon;
            public Dictionary<string, string> GeneralIcons;
            public Dictionary<string, MechanicDesc> MechanicData;
        }

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
        public Dictionary<long, string> SkillNames;
        public Dictionary<long, string> BuffNames;
        public JsonExtraLog ExtraData;
    }
}