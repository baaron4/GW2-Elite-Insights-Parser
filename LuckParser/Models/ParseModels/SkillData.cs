using System;
using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public class SkillData
    {
        // Fields
        private List<SkillItem> skill_list;
        private Dictionary<long, string> apiMissingID = new Dictionary<long, string>()
        {
            {1066, "Resurrect"},
            {1175, "Bandage" },
            {65001, "Dodge" },
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

        // Constructors
        public SkillData()
        {
            skill_list = new List<SkillItem>();
        }

        // Public Methods
        public void addItem(SkillItem item)
        {
            skill_list.Add(item);
        }

        public String getName(long ID)
        {

            // Custom
            if (apiMissingID.ContainsKey(ID))
            {
                return apiMissingID[ID];
            }

            // Normal
            foreach (SkillItem s in skill_list)
            {
                if (s.getID() == ID)
                {
                    return s.getName();
                }
            }

            // Unknown
            return "uid: " + ID.ToString();
        }

        // Getters
        public List<SkillItem> getSkillList()
        {
            return skill_list;
        }
    }
}