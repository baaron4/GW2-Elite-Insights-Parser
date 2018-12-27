using System;
using System.Collections.Generic;
using LuckParser.Models.ParseModels;

namespace LuckParser.Models.JsonModels
{
    class JsonLog
    {
        public string eliteInsightsVersion;
        public int triggerID;
        public string fightName;
        public string arcVersion;
        public string recordedBy;
        public string timeStart;
        public string timeEnd;
        public string duration;
        public bool success;
        public List<JsonTarget> targets;
        public List<JsonPlayer> players;
        public List<JsonPhase> phases;
        public List<JsonBenchData> benchData;
        public Dictionary<string, List<JsonMechanic>> mechanics;
        public string[] uploadLinks;
        public Dictionary<string, string> skillNames;
        public Dictionary<string, string> buffNames;
        public Dictionary<string, HashSet<long>> personalBuffs;
    }
}