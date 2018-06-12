using System;
using LuckParser.Controllers;

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
        public Agent(long ID, String name, int prof, int elite)
        {
            this.name = name;
            this.ID = ID;
            this.prof = prof;
            this.is_elite = elite;
        }

        // Public Methods
        public string getProf(string build, GW2APIController apiController) {
            if (is_elite == -1) {
                if ((ID & 0xffff0000) == 0xffff0000)
                {
                    return "GDG";
                }
                else
                {
                    return "NPC";
                }
            } else if (is_elite == 0)
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


                    }

                }
            }
            else if (Convert.ToInt32(build.Substring(4, 8)) >= 20170914) {
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


                    }

                } else
                if (is_elite > 1)
                {
                    switch (is_elite)
                    {
                        case 55:
                            return "Soulbeast";
                        case 56:
                            return "Weaver";
                        case 57:
                            return "Holosmith";
                        case 58:
                            return "Deadeye";
                        case 59:
                            return "Mirage";
                        case 60:
                            return "Scourge";
                        case 61:
                            return "Spellbreaker";
                        case 62:
                            return "Firebrand";
                        case 63:
                            return "Renegade";



                    }

                    GW2APISpec spec = apiController.GetSpec(is_elite);
                    if (spec.elite)
                    {
                        return spec.name;
                    }
                    else
                    {
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