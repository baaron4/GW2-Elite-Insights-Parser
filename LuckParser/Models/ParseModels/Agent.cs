using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Formatting;


namespace LuckParser.Models.ParseModels
{

    public class Agent
    {
        
        // Constants
        //NPC(-1, "NPC"),
        //GADGET(0, "GDG"),
        //GUARDIAN(1, "Guardian"),
        //WARRIOR(2, "Warrior"),
        //ENGINEER(3, "Engineer"),
        //RANGER(4, "Ranger"),
        //THIEF(5, "Thief"),
        //ELEMENTALIST(6, "Elementalist"),
        //MESMER(7, "Mesmer"),
        //NECROMANCER(8, "Necromancer"),
        //REVENANT(9, "Revenant"),
        //DRAGONHUNTER(10, "Dragonhunter"),
        //BERSERKER(11, "Berserker"),
        //SCRAPPER(12, "Scrapper"),
        //DRUID(13, "Druid"),
        //DAREDEVIL(14, "Daredevil"),
        //TEMPEST(15, "Tempest"),
        //CHRONOMANCER(16, "Chronomancer"),
        //REAPER(17, "Reaper"),
        //HERALD(18, "Herald");

        // Fields
        private String name;
        private long ID;
        private int is_elite;
        private int prof;

        // Constructor
       public Agent(long ID, String name,int prof,int elite)
        {
            this.name = name;
            this.ID = ID;
            this.prof = prof;
            this.is_elite = elite;
        }

        // Public Methods
        public string getProf(string build) {
            if (is_elite == -1) {
                if ((ID & 0xffff0000) == 0xffff0000)
                {
                    return "GDG";
                }
                else
                {
                    return "NPC";
                }
            }else if (is_elite == 0)
            {
                switch (prof)
                {
                    case 1:
                        return "Guardian";
                    case 2:
                        return "Warrior";
                    case 3:
                        return "Engineer";
                    case 4:
                        return "Ranger";
                    case 5:
                        return "Thief";
                    case 6:
                        return "Elementalist";
                    case 7:
                        return "Mesmer";
                    case 8:
                        return "Necromancer";
                    case 9:
                        return "Revenant";
                    default:
                        return "UNKNOWN";

                }


            }
            else if (Convert.ToInt32(build.Substring(4, 8)) < 20170914) {
                
                if (is_elite == 1)
                {
                    switch (prof + 9)
                    {
                        case 10:
                            return "Dragonhunter";
                        case 11:
                            return "Berserker";
                        case 12:
                            return "Scrapper";
                        case 13:
                            return "Druid";
                        case 14:
                            return "Daredevil";
                        case 15:
                            return "Tempest";
                        case 16:
                            return "Chronomancer";
                        case 17:
                            return "Reaper";
                        case 18:
                            return "Herald";
                        default:
                            return "UNKNOWN";

                    }

                }
            }
            else if (Convert.ToInt32(build.Substring(4, 8)) >= 20170914) {
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
                GW2APISpec spec = null;
                HttpResponseMessage response = APIClient.GetAsync("/v2/specializations/" + is_elite).Result;
                if (response.IsSuccessStatusCode)
                {
                    spec = response.Content.ReadAsAsync<GW2APISpec>().Result;
                    if (spec.elite)
                    {
                        return spec.name;
                    }
                    else {
                        return spec.profession;
                    }               
                }
                
            }
     
            return null;
        }
        // Getters
        public String getName()
        {
            return name;
        }

        public long getID()
        {
            return ID;
        }

    }
}