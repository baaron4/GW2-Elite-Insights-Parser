using System;
using System.Collections.Generic;
using LuckParser.Models.ParseModels;

namespace LuckParser.Models.JsonModels
{
    public class JsonLog
    {
        public class SkillDesc
        {
            public SkillDesc(SkillItem item)
            {
                Name = item.Name;
                AutoAttack = item.AA;
                Icon = item.Icon;
            }

            public string Name;
            public bool AutoAttack;
            public string Icon;
        }

        public class BuffDesc
        {
            public BuffDesc(Boon item)
            {
                Name = item.Name;
                Icon = item.Link;
                Stacking = item.Type == Boon.BoonType.Intensity;
            }

            public string Name;
            public string Icon;
            public bool Stacking;
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
        public List<JsonTarget> Targets;
        public List<JsonPlayer> Players;
        public List<JsonPhase> Phases;
        public Dictionary<string, List<JsonMechanic>> Mechanics;
        public string[] UploadLinks;
        public Dictionary<string, SkillDesc> SkillMap;
        public Dictionary<string, BuffDesc> BuffMap;
        public Dictionary<string, HashSet<long>> PersonalBuffs;
    }
}