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
        public string Name { get; }
        public ulong ID { get; }
        private readonly uint _isElite;
        private readonly uint _prof;

        // Constructor
        public Agent(ulong ID, String name, uint prof, uint elite)
        {
            Name = name;
            this.ID = ID;
            _prof = prof;
            _isElite = elite;
        }

        // Public Methods
        public string GetProf(string build, GW2APIController apiController) {
            if (_isElite == 0xFFFFFFFF) {
                if ((_prof & 0xffff0000) == 0xffff0000)
                {
                    return "GDG";
                }
                else
                {
                    return "NPC";
                }
            } else if (_isElite == 0)
            {
                switch (_prof)
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

                if (_isElite == 1)
                {
                    switch (_prof + 9)
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
                if (_isElite == 1)
                {
                    switch (_prof + 9)
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
                if (_isElite > 1)
                {
                    switch (_isElite)
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

                    GW2APISpec spec = apiController.GetSpec((int)_isElite);
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
    }
}