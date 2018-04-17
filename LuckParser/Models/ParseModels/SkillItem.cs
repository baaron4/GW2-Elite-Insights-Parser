using LuckParser.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class SkillItem
    {
        // Fields
        private int ID;
        private String name;
        private GW2APISkill apiSkill = null;

        // Constructor
        public SkillItem(int ID, String name)
        {
            name = name.Replace("\0", "");
            this.ID = ID;
            this.name = name;
        }

        // Public Methods
        public String[] toStringArray()
        {
            String[] array = new String[2];
            array[0] = ID.ToString();
            array[1] = name.ToString();
            return array;
        }
        //setter
        public void SetGW2APISkill(GW2APIController apiController)
        {
            if (apiSkill == null)
            {
                GW2APISkill skillAPI = apiController.GetSkill(ID);

                if (skillAPI != null) {
                    this.apiSkill = skillAPI;
                    this.name = skillAPI.name;
                }
                
            }
            
        }
        // Getters
        public int getID()
        {
            return ID;
        }

        public String getName()
        {
            if (ID == 1066) {
                return "Resurrect";
            }
            return name;
        }

        public GW2APISkill GetGW2APISkill() {
            return apiSkill;
        }
    }
}