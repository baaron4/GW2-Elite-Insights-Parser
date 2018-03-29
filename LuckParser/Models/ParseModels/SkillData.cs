using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class SkillData
    {
        // Fields
        private List<SkillItem> skill_list;

        // Constructors
        public SkillData()
        {
            this.skill_list = new List<SkillItem>();
        }

        // Public Methods
        public void addItem(SkillItem item)
        {
            skill_list.Add(item);
        }

        public String getName(int ID)
        {

            // Custom
            if (ID == 1066 ) {
                return "Resurrect";
            }
            if ( ID == 1175  ) {
                return "Bandage";
            }
            if (ID == 65001) {
                return "Dodge";
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