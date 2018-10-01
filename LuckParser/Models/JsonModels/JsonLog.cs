using System;
using System.Collections.Generic;
using LuckParser.Models.ParseModels;

namespace LuckParser.Models.DataModels
{
    class JsonLog
    {

        public class MechanicDesc
        {
            public string PlotlySymbol;
            public string PlotlyColor;
            public int Enemy;
            public string Description;
            public string PlotlyName;
        }

        public class BuffDesc
        {
            public string Icon;
            public int Stacking;
            public int Table;
        }

        public class JsonExtraLog
        {
            public Dictionary<long, BuffDesc> BuffData = null;
            public Dictionary<long, string> SkillIcons = null;
            public Dictionary<string, string> ActorIcons = null;
            public string FightIcon = null;
            public Dictionary<string, string> GeneralIcons = null;
            public Dictionary<string, MechanicDesc> MechanicData = null;
            public Dictionary<string, List<long>> PersonalBuffs = null;
        }

        public string EliteInsightsVersion;
        public int TriggerID;
        public string FightName;
        public string ArcVersion;
        public string RecordedBy;
        public string TimeStart;
        public string TimeEnd;
        public string Duration;
        public int Success;
        public List<JsonBoss> Boss;
        public List<JsonPlayer> Players;
        public List<JsonPhase> Phases;
        public Dictionary<string, List<JsonMechanic>> Mechanics;
        public string[] UploadLinks;
        public Dictionary<long, string> SkillNames;
        public Dictionary<long, string> BuffNames;
        public JsonExtraLog ED = null;
    }
}