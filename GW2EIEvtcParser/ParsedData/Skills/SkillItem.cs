using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIGW2API;
using GW2EIGW2API.GW2API;

namespace GW2EIEvtcParser.ParsedData
{
    public class SkillItem
    {
        public const long DodgeId = 65001;
        public const long MirageCloakDodgeId = 65002;
        public const long ResurrectId = 1066;
        public const long BandageId = 1175;
        public const long WeaponSwapId = -2;
        public const long DeathId = -4;
        public const long DownId = -3;
        public const long DCId = -5;
        public const long AliveId = -6;
        public const long RespawnId = -7;

        private const int FirstLandSet = 4;
        private const int SecondLandSet = 5;
        private const int FirstWaterSet = 0;
        private const int SecondWaterSet = 1;
        private static readonly Dictionary<long, string> _overrideNames = new Dictionary<long, string>()
        {
            {ResurrectId, "Resurrect"},
            {BandageId, "Bandage" },
            {DodgeId, "Dodge" },
            {MirageCloakDodgeId, "Mirage Cloak" },
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
            {52325, "Conjured Slash" },
            {52780, "Conjured Protection" },
            // Sabir
            {56349, "Regenerative Breakbar" },
            // P.Qadim
            {56296, "Ruinous Nova Charge" },
            //{56036, "Magma Bomb" },
            {56405, "Force of Retaliation Cast" },
            {56375, "Teleport Center" },
            {56446, "Eat Pylon" },
            {56329, "Big Magma Drop" },
            // Weaver attunements
            {ProfHelper.FireDual, "Dual Fire Attunement" },
            {ProfHelper.FireWater, "Fire Water Attunement" },
            {ProfHelper.FireAir, "Fire Air Attunement" },
            {ProfHelper.FireEarth, "Fire Earth Attunement" },

            {ProfHelper.WaterDual, "Dual Water Attunement" },
            {ProfHelper.WaterFire, "Water Fire Attunement" },
            {ProfHelper.WaterAir, "Water Air Attunement" },
            {ProfHelper.WaterEarth, "Water Earth Attunement" },

            {ProfHelper.AirDual, "Dual Air Attunement" },
            {ProfHelper.AirFire, "Air Fire Attunement" },
            {ProfHelper.AirWater, "Air Water Attunement" },
            {ProfHelper.AirEarth, "Air Earth Attunement" },

            {ProfHelper.EarthDual, "Dual Earth Attunement" },
            {ProfHelper.EarthFire, "Earth Fire Attunement" },
            {ProfHelper.EarthWater, "Earth Water Attunement" },
            {ProfHelper.EarthAir, "Earth Air Attunement" },
        };
        private static readonly Dictionary<long, string> _overrideIcons = new Dictionary<long, string>()
        {
            {ResurrectId, "https://wiki.guildwars2.com/images/3/3d/Downed_ally.png"},
            {BandageId, "https://wiki.guildwars2.com/images/0/0c/Bandage.png"},
            {DodgeId, "https://wiki.guildwars2.com/images/archive/b/b2/20150601155307%21Dodge.png"},
            {MirageCloakDodgeId, "https://wiki.guildwars2.com/images/a/a5/Mirage_Cloak_%28effect%29.png"},
            {WeaponSwapId, "https://wiki.guildwars2.com/images/c/ce/Weapon_Swap_Button.png"},
            {49112, "https://wiki.guildwars2.com/images/e/e7/Throw_Magnetic_Bomb.png"},
            {49063, "https://wiki.guildwars2.com/images/3/3d/Detonate_Plasma.png"},
            {49123, "https://wiki.guildwars2.com/images/d/dd/Unstable_Artifact.png"},
            {31686, "https://wiki.guildwars2.com/images/4/4b/Overload_Air.png" },
            {52325, "https://wiki.guildwars2.com/images/5/59/Conjured_Slash.png" },
            {52780, "https://wiki.guildwars2.com/images/0/02/Conjured_Protection.png" },
            //{41243, "https://wiki.guildwars2.com/images/f/fb/Full_Counter.png" },
            {31600, "https://wiki.guildwars2.com/images/3/30/Bounding_Dodger.png"},
            //{10281, "https://wiki.guildwars2.com/images/9/91/Illusionary_Riposte.png"},
            //{38769, "https://wiki.guildwars2.com/images/4/48/Phantasmal_Swordsman.png"},
            {45534, "https://wiki.guildwars2.com/images/8/85/Loss_Aversion.png" },
            {9292, "https://wiki.guildwars2.com/images/c/c3/Superior_Sigil_of_Air.png" },
            {9433, "https://wiki.guildwars2.com/images/4/43/Superior_Sigil_of_Geomancy.png" },
            {40015, "https://wiki.guildwars2.com/images/c/c9/Chapter_4-_Scorched_Aftermath.png"},
            {41258, "https://wiki.guildwars2.com/images/d/d3/Chapter_1-_Searing_Spell.png"},
            {46618, "https://wiki.guildwars2.com/images/a/a8/Flame_Rush.png"},
            {40635, "https://wiki.guildwars2.com/images/5/53/Chapter_2-_Igniting_Burst.png"},
            {42898, "https://wiki.guildwars2.com/images/6/6d/Epilogue-_Ashes_of_the_Just.png"},
            {42986, "https://wiki.guildwars2.com/images/3/30/Chapter_1-_Unflinching_Charge.png"},
            {31289, "https://wiki.guildwars2.com/images/7/70/King_of_Fires.png"},
            {56885, "https://wiki.guildwars2.com/images/e/e2/Earthen_Blast.png"},
            {40071, "https://wiki.guildwars2.com/images/4/40/Garish_Pillar.png" },
            {46726, "https://wiki.guildwars2.com/images/0/08/Desert_Shroud.png" },
            {41968,  "https://wiki.guildwars2.com/images/7/79/Chapter_2-_Daring_Challenge.png"},
            {46616, "https://wiki.guildwars2.com/images/7/7e/Flame_Surge.png" },
            {42449,  "https://wiki.guildwars2.com/images/e/e7/Chapter_3-_Heated_Rebuke.png"},
            {40988, "https://wiki.guildwars2.com/images/8/89/Chapter_4-_Stalwart_Stand.png"},
            {44455, "https://wiki.guildwars2.com/images/d/d8/Epilogue-_Unbroken_Lines.png"},
            {43734, "https://wiki.guildwars2.com/images/6/68/Consuming_Bite.png"},
            {45262,  "https://wiki.guildwars2.com/images/8/84/Narcotic_Spores.png"},
            {41864, "https://wiki.guildwars2.com/images/c/c8/Crippling_Anguish.png" },
            {43593, "https://wiki.guildwars2.com/images/b/bc/Kick_%28gazelle%29.png" },
            {44051, "https://wiki.guildwars2.com/images/a/af/Charge_%28gazelle%29.png" },
            {44733, "https://wiki.guildwars2.com/images/8/82/Headbutt_%28gazelle%29.png" },
            {41836,  "https://wiki.guildwars2.com/images/7/73/Chapter_3-_Valiant_Bulwark.png"},
            {9428, "https://wiki.guildwars2.com/images/3/33/Superior_Sigil_of_Hydromancy.png" },
            {56911, "https://wiki.guildwars2.com/images/6/67/Pitfall.png" },
            {40679, "https://wiki.guildwars2.com/images/9/95/Chapter_2-_Radiant_Recovery.png" },
            {42008, "https://wiki.guildwars2.com/images/1/16/Chapter_4-_Shining_River.png" },
            {42925, "https://wiki.guildwars2.com/images/5/5f/Epilogue-_Eternal_Oasis.png"},
            {43630, "https://wiki.guildwars2.com/images/0/0c/Thermal_Release_Valve.png" },
            {22499, "https://wiki.guildwars2.com/images/d/d0/Shattered_Aegis.png" },
            {29604, "https://wiki.guildwars2.com/images/8/82/Chilling_Nova.png" },
            // Weaver attunements
            {ProfHelper.FireDual, "https://wiki.guildwars2.com/images/b/b4/Fire_Attunement.png" },
            {ProfHelper.FireWater, "https://i.imgur.com/ihqKuUJ.png" },
            {ProfHelper.FireAir, "https://i.imgur.com/kKFJ8cT.png" },
            {ProfHelper.FireEarth, "https://i.imgur.com/T4187h0.png" },

            {ProfHelper.WaterDual, "https://wiki.guildwars2.com/images/3/31/Water_Attunement.png" },
            {ProfHelper.WaterFire, "https://i.imgur.com/vMUkzxH.png" },
            {ProfHelper.WaterAir, "https://i.imgur.com/5G5OFud.png" },
            {ProfHelper.WaterEarth, "https://i.imgur.com/QKEtF2P.png" },

            {ProfHelper.AirDual, "https://wiki.guildwars2.com/images/9/91/Air_Attunement.png" },
            {ProfHelper.AirFire, "https://i.imgur.com/vf68GJm.png" },
            {ProfHelper.AirWater, "https://i.imgur.com/Tuj5Sro.png" },
            {ProfHelper.AirEarth, "https://i.imgur.com/lHcOSwk.png" },

            {ProfHelper.EarthDual, "https://wiki.guildwars2.com/images/a/a8/Earth_Attunement.png" },
            {ProfHelper.EarthFire, "https://i.imgur.com/aJWvE0I.png" },
            {ProfHelper.EarthWater, "https://i.imgur.com/jtjj2TG.png" },
            {ProfHelper.EarthAir, "https://i.imgur.com/4Eti7Pb.png" },
        };

        private static readonly Dictionary<long, ulong> _nonCritable = new Dictionary<long, ulong>
                    {
                        { 9292, ulong.MaxValue }, // Lightning Strike (Sigil)
                        { 5492, 94051 },  // Fire Attunement
                        { 13014, ulong.MaxValue }, // Mug
                        { 30770, 54485 }, // Pulmonary Impact
                        { 52370, ulong.MaxValue },
                        { 31686, ulong.MaxValue }, // Lightning Jolt
                        { 56883, 94051 }, // Sunspot
                        { 56885, 94051 }, // Earthen Blast
                        { 29604, 94051 }, // Chilling Nova
                                         // Spontaneous Destruction 94051
                        {13907, 94051 }, // Weakening Shroud
                        {29560, 94051 }, // Spiteful Spirit
                        {13906, 94051 }, // Chill of Death
                                         // Power block 94051
                        {22499, 94051 }, // Shattered Aegis
                        {21795, 94051 }, // Glacial Heart
                        {43630, 94051 }, // Thermal Release Valve
                        {45534, 82356 }, // Loss Aversion
                        // 
                    };

        private const string DefaultIcon = "https://render.guildwars2.com/file/1D55D34FB4EE20B1962E315245E40CA5E1042D0E/62248.png";

        // Fields
        public long ID { get; private set; }
        //public int Range { get; private set; } = 0;
        public bool AA => _apiSkill?.Slot == "Weapon_1" || _apiSkill?.Slot == "Downed_1";

        public bool IsSwap => ID == WeaponSwapId || ElementalistHelper.IsElementalSwap(ID);
        public string Name { get; private set; }
        public string Icon { get; private set; }
        private WeaponDescriptor _weaponDescriptor;
        private readonly GW2APISkill _apiSkill;
        private SkillInfoEvent _skillInfo { get; set; }

        // Constructor

        public SkillItem(long ID, string name)
        {
            this.ID = ID;
            Name = name.Replace("\0", "");
            _apiSkill = GW2APIController.GetAPISkill(ID);
            CompleteItem();
        }

        public static bool CanCrit(long id, ulong gw2Build)
        {
            if (_nonCritable.TryGetValue(id, out ulong build))
            {
                return gw2Build < build;
            }
            return true;
        }

        internal int FindWeaponSlot(List<int> swaps)
        {
            int swapped = -1;
            // we started on a proper weapon set
            if (_weaponDescriptor != null)
            {
                int firstSwap = swaps.Count > 0 ? swaps[0] : -1;
                if (_weaponDescriptor.IsLand)
                {
                    // if the first swap is not a land set that means the next time we get to a land set was the first set to begin with
                    if (firstSwap != FirstLandSet && firstSwap != SecondLandSet)
                    {
                        swapped = swaps.Exists(x => x == FirstLandSet || x == SecondLandSet) ? swaps.First(x => x == FirstLandSet || x == SecondLandSet) : FirstLandSet;
                    }
                    else
                    {
                        swapped = firstSwap == FirstLandSet ? SecondLandSet : FirstLandSet;
                    }
                }
                else
                {
                    // if the first swap is not a water set that means the next time we get to a water set was the first set to begin with
                    if (firstSwap != FirstWaterSet && firstSwap != SecondWaterSet)
                    {
                        swapped = swaps.Exists(x => x == FirstWaterSet || x == FirstWaterSet) ? swaps.First(x => x == FirstWaterSet || x == SecondWaterSet) : FirstWaterSet;
                    }
                    else
                    {
                        swapped = firstSwap == FirstWaterSet ? SecondWaterSet : FirstWaterSet;
                    }
                }
            }
            return swapped;
        }

        internal bool EstimateWeapons(string[] weapons, int swapped, bool swapCheck)
        {
            if (weapons.Length != 8)
            {
                throw new InvalidOperationException("Invalid count in weapons array");
            }
            int id = swapped == FirstLandSet ? 0 : swapped == SecondLandSet ? 2 : swapped == FirstWaterSet ? 4 : swapped == SecondWaterSet ? 6 : -1;
            if (_weaponDescriptor == null || id == -1 || !swapCheck)
            {
                return false;
            }
            if (_weaponDescriptor.WeaponSlot == WeaponDescriptor.Hand.Dual)
            {
                weapons[id] = _apiSkill.WeaponType;
                weapons[id + 1] = _apiSkill.DualWield;
            }
            else if (_weaponDescriptor.WeaponSlot == WeaponDescriptor.Hand.TwoHand)
            {
                weapons[id] = _apiSkill.WeaponType;
                weapons[id + 1] = "2Hand";
            }
            else if (_weaponDescriptor.WeaponSlot == WeaponDescriptor.Hand.MainHand)
            {
                weapons[id] = _apiSkill.WeaponType;
            }
            else
            {
                weapons[id + 1] = _apiSkill.WeaponType;
            }
            return true;
        }

        internal void AttachSkillInfoEvent(SkillInfoEvent skillInfo)
        {
            if (ID == skillInfo.SkillID)
            {
                _skillInfo = skillInfo;
            }
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
                /*if (_apiSkill.Facts != null)
                {
                    foreach (GW2APIFact fact in _apiSkill.Facts)
                    {
                        if (fact.Text != null && fact.Text == "Range" && fact.Value != null)
                        {
                            Range = Convert.ToInt32(fact.Value);
                        }
                    }
                }*/
            }
            if (_overrideIcons.TryGetValue(ID, out string icon))
            {
                Icon = icon;
            }
            else
            {
                Icon = _apiSkill != null ? _apiSkill.Icon : DefaultIcon;
            }
            if (_apiSkill != null && _apiSkill.Type == "Weapon" && _apiSkill.WeaponType != "None" && _apiSkill.Professions.Count > 0 && (_apiSkill.Categories == null || (_apiSkill.Categories.Count == 1 && (_apiSkill.Categories[0] == "Phantasm" || _apiSkill.Categories[0] == "DualWield"))))
            {
                _weaponDescriptor = new WeaponDescriptor(_apiSkill);
            }
#if DEBUG
            Name += " (" + ID + ")";
#endif
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
