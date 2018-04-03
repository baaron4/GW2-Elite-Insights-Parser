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
        public void SetGW2APISkill()
        {
            if (apiSkill == null)
            {
                //Connecting to API everytime would be bad so

                HttpClient APIClient = null;

                if (APIClient == null)
                {
                    APIClient = new HttpClient();
                    APIClient.BaseAddress = new Uri("https://api.guildwars2.com");
                    APIClient.DefaultRequestHeaders.Accept.Clear();
                    APIClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                }

                //System.Threading.Thread.Sleep(100);
                GW2APISkill skill = null;
                HttpResponseMessage response = APIClient.GetAsync("/v2/skills/" + ID).Result;
                if (response.IsSuccessStatusCode)
                {
                    skill = response.Content.ReadAsAsync<GW2APISkill>().Result;
                    this.apiSkill = skill;
                    this.name = skill.name;
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
            return name;
        }

        public GW2APISkill GetGW2APISkill() {
            if (apiSkill == null) {
                SetGW2APISkill();
            }
            return apiSkill;
        }
    }
}