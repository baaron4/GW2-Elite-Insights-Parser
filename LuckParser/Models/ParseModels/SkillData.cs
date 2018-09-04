using System;
using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public class SkillData
    {
        public ICollection<SkillItem> Values
        {
            get
            {
                return _skills.Values;
            }
        }
        // Fields
        private readonly Dictionary<long, SkillItem> _skills = new Dictionary<long, SkillItem>();
        readonly static Dictionary<long, string> _apiMissingID = new Dictionary<long, string>()
        {
            {SkillItem.ResurrectId, "Resurrect"},
            {SkillItem.BandageId, "Bandage" },
            {SkillItem.DodgeId, "Dodge" },
            // Gorseval
            {31834,"Ghastly Rampage" },
            {31759,"Protective Shadow" },
            {31466,"Ghastly Rampage (Begin)" },
            // Sabetha
            {31372, "Shadow Step" },
            // Slothasor
            {34547, "Tantrum Start" },
            {34515, "Sleeping" },
            {34340, "Fear Me!" },
            // Matthias
            { 34468, "Shield (Human)"},
            { 34427, "Abomination Transformation"},
            { 34510, "Shield (Abomination)"},
            // Generic
            {-5, "Phase out" },
            // Deimos
            {-6, "Roleplay" },
            // Dhuum
            {47396, "Major Soul Split" },
            // Keep Construct
            {35048, "Magic Blast Charge" }
        };
        
        // Public Methods

        public SkillItem Get(long ID)
        {
            if (_skills.TryGetValue(ID, out SkillItem value))
            {
                return value;
            }
            return null;
        }

        public void Add(SkillItem skillItem)
        {
            _skills.Add(skillItem.GetID(), skillItem);
        }

        public String GetName(long ID)
        {
            // Custom
            if (_apiMissingID.ContainsKey(ID))
            {
                return _apiMissingID[ID];
            }

            // Normal
            SkillItem skillItem = Get(ID);
            if (skillItem != null)
            {
                return skillItem.GetName();
            }

            // Unknown
            return "uid: " + ID.ToString();
        }    
    }
}