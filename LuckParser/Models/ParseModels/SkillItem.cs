using LuckParser.Controllers;
using System;
using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public class SkillItem
    {
        public const long DodgeId = 65001;
        public const long ResurrectId = 1066;
        public const long BandageId = 1175;
        public const long WeaponSwapId = -2;
        public const long DeathId = -4;
        public const long DownId = -3;

        readonly static Dictionary<long, string> _overrideNames = new Dictionary<long, string>()
        {
            {ResurrectId, "Resurrect"},
            {BandageId, "Bandage" },
            {DodgeId, "Dodge" },
            {WeaponSwapId, "Weapon Swap" },
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

        readonly static Dictionary<long, string> _overrideIcons = new Dictionary<long, string>()
        {
            {ResurrectId, "https://wiki.guildwars2.com/images/3/3d/Downed_ally.png"},
            {BandageId, "https://wiki.guildwars2.com/images/0/0c/Bandage.png" },
            {DodgeId, "https://wiki.guildwars2.com/images/archive/b/b2/20150601155307%21Dodge.png" },
            {WeaponSwapId, "https://wiki.guildwars2.com/images/c/ce/Weapon_Swap_Button.png" },
            {49112, "https://wiki.guildwars2.com/images/e/e7/Throw_Magnetic_Bomb.png" },
        };

        private const string _defaultIcon = "https://render.guildwars2.com/file/1D55D34FB4EE20B1962E315245E40CA5E1042D0E/62248.png";

        // Fields
        public readonly long ID;
        public string Name { get; private set; }
        public string Icon { get; private set; }
        public GW2APISkill ApiSkill { get; private set; }
        public int CC { get; private set; }

        // Constructor
        public SkillItem(long ID, String name)
        {
            this.ID = ID;
            Name = name.Replace("\0", "");
            CompleteItem();
        }

        public SkillItem(long ID, String name, GW2APIController apiController)
        {
            this.ID = ID;
            Name = name.Replace("\0", "");
            ApiSkill = apiController.GetSkill(ID);
            if (ApiSkill != null)
            {
                Name = ApiSkill.name;
            }
            CompleteItem();
        }

        private void CompleteItem()
        {
            if (_overrideNames.TryGetValue(ID,out string name))
            {
                Name = name;
            }
            if (_overrideIcons.TryGetValue(ID, out string icon))
            {
                Icon = icon;
            } else
            {
                Icon = ApiSkill != null ? ApiSkill.icon : _defaultIcon;
            }
        }

        // Public Methods

        public void SetCCAPI()//this is 100% off the GW2 API is not a reliable source of finding skill CC
        {
            CC = 0;
            if (ApiSkill != null)
            {
                GW2APISkillDetailed apiskilldet = (GW2APISkillDetailed)ApiSkill;
                GW2APISkillCheck apiskillchec = (GW2APISkillCheck)ApiSkill;
                GW2APIfacts[] factsList = apiskilldet != null ? apiskilldet.facts : apiskillchec.facts;
                bool daze = false;
                bool stun = false;
                bool knockdown = false;
                bool flaot = false;
                bool knockback = false;
                bool launch = false;
                bool pull = false;
               
                foreach (GW2APIfacts fact in factsList)
                {
                    if (daze == false)
                    {
                        if (fact.text == "Daze" || fact.status == "Daze")
                        {
                            if (fact.duration < 1)
                            {
                                CC += 100;
                            }
                            else
                            {
                                CC += fact.duration * 100;
                            }
                            daze = true;
                        }

                    }
                    if (stun == false)
                    {
                        if (fact.text == "Stun" || fact.status == "Stun")
                        {
                            if (fact.duration < 1)
                            {
                                CC += 100;
                            }
                            else
                            {
                                CC += fact.duration * 100;
                            }
                            stun = true;
                        }
                    }
                    if (knockdown == false)
                    {
                        if (fact.text == "Knockdown" || fact.status == "Knockdown")
                        {
                            if (fact.duration < 1)
                            {
                                CC += 100;
                            }
                            else
                            {
                                CC += fact.duration * 100;
                            }
                            knockdown = true;
                        }
                    }
                    if (launch == false)
                    {
                        if (fact.text == "Launch" || fact.status == "Launch")
                        {

                            CC += 232;//Wiki says either 232 or 332 based on duration? launch doesn't provide duration in api however
                           
                            launch = true;
                        }
                    }
                    if (knockback == false)
                    {
                        if (fact.text == "Knockback" || fact.status == "Knockback")
                        {

                            CC += 150;//always 150 unless special case of 232 for ranger pet?
                            knockback = true;
                        }
                    }
                    if (pull == false)
                    {
                        if (fact.text == "Pull" || fact.status == "Pull")
                        {

                            CC += 150;

                            pull = true;
                        }
                    }
                    if (flaot == false)
                    {
                        if (fact.text == "Float" || fact.status == "Float")
                        {
                            if (fact.duration < 1)
                            {
                                CC += 100;
                            }
                            else
                            {
                                CC += fact.duration * 100;
                            }
                            flaot = true;
                        }
                    }
                    if (fact.text == "Stone Duration" || fact.status == "Stone Duration")
                    {
                        if (fact.duration < 1)
                        {
                            CC += 100;
                        }
                        else
                        {
                            CC += fact.duration * 100;
                        }
                        
                    }

                
                }
                if (ID == 30725)//toss elixir x
                {
                    CC = 300;
                }
                if (ID == 29519)//MOA signet
                {
                    CC = 1000;
                }
               
            }
        }
    }
}