using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class RotationItem
    {
        private string name;
        private float duration;
        private GW2APISkill skill;
        private ParseEnum.Activation end_status;
        private ParseEnum.Activation start_status;

        public RotationItem()
        {
        }

        public void findName(SkillData skill_data, int id)
        {
            name = "";
            skill = null;
            List<SkillItem> s_list = skill_data.getSkillList();
            if (s_list.FirstOrDefault(x => x.getID() == id) != null)
            {
                skill = s_list.FirstOrDefault(x => x.getID() == id).GetGW2APISkill();
            }
            if (id == -2)
            {
                name = "Weapon Swap";
            }
            if (skill == null)
            {
                name = skill_data.getName(id);
            }
            else
            {
                name = skill.name;
            }
            name = name.Replace("\"", "");
        }

        public void setDuration(int act_dur)
        {
            switch (name)
            {
                case "Dodge":
                    duration = 0.5f;
                    break;
                case "Weapon Swap":
                    duration = 0.1f;
                    break;
                case "Resurrect":
                case "Bandage":
                default:
                    duration = act_dur / 1000f;
                    break;
            }
        }

        public void setEndStatus(ParseEnum.Activation end_status)
        {
            this.end_status = end_status;
        }

        public void setStartStatus(ParseEnum.Activation start_status)
        {
            this.start_status = start_status;
        }
    }
}
