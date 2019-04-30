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
        public const long DCId = -5;
        public const long AliveId = -6;
        public const long RespawnId = -7;

        public const int FirstLandSet = 4;
        public const int SecondLandSet = 5;
        public const int FirstWaterSet = 0;
        public const int SecondWaterSet = 1;

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
            //{-5, "Phase out" },
            // Deimos
            //{-6, "Roleplay" },
            // Dhuum
            {47396, "Major Soul Split" },
            // Keep Construct
            {35048, "Magic Blast Charge" },
            // CA
            {52325, "Conjured Greatsword" }
        };

        readonly static Dictionary<long, string> _overrideIcons = new Dictionary<long, string>()
        {
            {ResurrectId, "https://wiki.guildwars2.com/images/3/3d/Downed_ally.png"},
            {BandageId, "https://wiki.guildwars2.com/images/0/0c/Bandage.png"},
            {DodgeId, "https://wiki.guildwars2.com/images/archive/b/b2/20150601155307%21Dodge.png"},
            {WeaponSwapId, "https://wiki.guildwars2.com/images/c/ce/Weapon_Swap_Button.png"},
            {49112, "https://wiki.guildwars2.com/images/e/e7/Throw_Magnetic_Bomb.png"},
            {49063, "https://wiki.guildwars2.com/images/3/3d/Detonate_Plasma.png"},
            {49123, "https://wiki.guildwars2.com/images/d/dd/Unstable_Artifact.png"},
            {31686, "https://wiki.guildwars2.com/images/4/4b/Overload_Air.png" },
        };

        private const string _defaultIcon = "https://render.guildwars2.com/file/1D55D34FB4EE20B1962E315245E40CA5E1042D0E/62248.png";

        // Fields
        public long ID { get; private set; }
        public int Range { get; private set; } = 0;
        public bool AA => _apiSkill?.Slot == "Weapon_1" || _apiSkill?.Slot == "Downed_1";
        public string Name { get; private set; }
        public string Icon { get; private set; }
        public WeaponDescriptor WeaponDescriptor { get; private set; }
        private GW2APISkill _apiSkill;

        // Constructor
        public SkillItem(long ID, string name)
        {
            this.ID = ID;
            Name = name.Replace("\0", "");
            CompleteItem();
        }

        public SkillItem(long ID, string name, GW2APIController apiController)
        {
            this.ID = ID;
            Name = name.Replace("\0", "");
            _apiSkill = apiController.GetSkill(ID);
            CompleteItem();
        }

        public bool EstimateWeapons(string[] weapons, int swapped, bool swapCheck)
        {
            if (weapons.Length != 8)
            {
                throw new InvalidOperationException("Invalid count in weapons array");
            }
            if (_apiSkill == null || !swapCheck)
            {
                return false;
            }
            int id = swapped == FirstLandSet ? 0 : swapped == SecondLandSet ? 2 : swapped == FirstWaterSet ? 4 : swapped == SecondWaterSet ? 6 : -1;
            if (WeaponDescriptor != null && id > -1)
            {
                if (WeaponDescriptor.WeaponSlot == WeaponDescriptor.Hand.Dual)
                {
                    weapons[id] = _apiSkill.WeaponType;
                    weapons[id + 1] = _apiSkill.DualWield;
                }
                else if (WeaponDescriptor.WeaponSlot == WeaponDescriptor.Hand.TwoHand)
                {
                    weapons[id] = _apiSkill.WeaponType;
                    weapons[id + 1] = "2Hand";
                }
                else if (WeaponDescriptor.WeaponSlot == WeaponDescriptor.Hand.MainHand)
                {
                    weapons[id] = _apiSkill.WeaponType;
                }
                else
                {
                    weapons[id + 1] = _apiSkill.WeaponType;
                }
            }
            return true;
        }

        private void CompleteItem()
        {
            if (_apiSkill == null && _overrideNames.TryGetValue(ID, out string name))
            {
                Name = name;
            }
            else if (_apiSkill != null)
            {
                Name = _apiSkill.Name;
                if (_apiSkill.Facts != null)
                {
                    foreach (GW2APIFact fact in _apiSkill.Facts)
                    {
                        if (fact.Text != null && fact.Text == "Range" && fact.Value != null)
                        {
                            Range = Convert.ToInt32(fact.Value);
                        }
                    }
                }
            }
            if (_apiSkill == null && _overrideIcons.TryGetValue(ID, out string icon))
            {
                Icon = icon;
            }
            else
            {
                Icon = _apiSkill != null ? _apiSkill.Icon : _defaultIcon;
            }
            if (_apiSkill != null && _apiSkill.Type == "Weapon" && _apiSkill.Professions.Length > 0 && (_apiSkill.Categories == null || (_apiSkill.Categories.Length == 1 && (_apiSkill.Categories[0] == "Phantasm" || _apiSkill.Categories[0] == "DualWield"))))
            {
                WeaponDescriptor = new WeaponDescriptor(_apiSkill);
            }
        }

        // Public Methods

        /*public void SetCCAPI()//this is 100% off the GW2 API is not a reliable source of finding skill CC
        {
            CC = 0;
            if (_apiSkill != null)
            {
                GW2APISkillDetailed apiskilldet = (GW2APISkillDetailed)_apiSkill;
                GW2APISkillCheck apiskillchec = (GW2APISkillCheck)_apiSkill;
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
        }*/
    }
}