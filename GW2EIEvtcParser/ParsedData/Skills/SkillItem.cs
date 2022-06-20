using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIGW2API;
using GW2EIGW2API.GW2API;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.ParsedData
{
    public class SkillItem
    {

        internal static (long, long) GetArcDPSCustomIDs(int evtcVersion)
        {
            if (evtcVersion >= ArcDPSBuilds.InternalSkillIDsChange)
            {
                return (ArcDPSDodge20220307, ArcDPSGenericBreakbar20220307);
            }
            else
            {
                return (ArcDPSDodge, ArcDPSGenericBreakbar);
            }
        }

        private static readonly Dictionary<long, string> _overrideNames = new Dictionary<long, string>()
        {
            {Resurrect, "Resurrect"},
            {Bandage, "Bandage" },
            {ArcDPSDodge, "Dodge" },
            {ArcDPSDodge20220307, "Dodge" },
            {ArcDPSGenericBreakbar, "Generic Breakbar" },
            {ArcDPSGenericBreakbar20220307, "Generic Breakbar" },
            {MirageCloakDodge, "Mirage Cloak" },
            {WeaponSwap, "Weapon Swap" },
            {SelflessDaring, "Selfless Daring"}, // The game maps this name incorrectly to "Selflessness Daring"
	        {14024, "Natural Healing"}, // The game does not map this one at all
	        {26558, "Energy Expulsion"},
            {29863, "Live Vicariously"}, // The game maps this name incorrectly to "Vigorous Recovery"
	        {30313, "Escapist's Fortitude"}, // The game maps this to the wrong skill
            // Gorseval
            {GhastlyRampage,"Ghastly Rampage" },
            {31759,"Protective Shadow" },
            {31466,"Ghastly Rampage (Begin)" },
            // Sabetha
            {31372, "Shadow Step" },
            // Slothasor
            {TantrumSkill, "Tantrum Start" },
            {NarcolepsySkill, "Sleeping" },
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
            // Adina
            {56035, "Double Rotating Earth Rays" },
            {56381, "Triple Rotating Earth Rays" },
            {56049, "Terraform" },
            // Sabir
            {56349, "Regenerative Breakbar" },
            // P.Qadim
            {56296, "Ruinous Nova Charge" },
            //{56036, "Magma Bomb" },
            {ForceOfRetaliationCast, "Force of Retaliation Cast" },
            {PeerlessQadimTPCenter, "Teleport Center" },
            {56446, "Eat Pylon" },
            {56329, "Big Magma Drop" },
            // Voice and Claw
            {58382, "Kodan Teleport" },
            // Weaver attunements
            {DualFireAttunement, "Dual Fire Attunement" },
            {FireWaterAttunement, "Fire Water Attunement" },
            {FireAirAttunement, "Fire Air Attunement" },
            {FireEarthAttunement, "Fire Earth Attunement" },

            {DualWaterAttunement, "Dual Water Attunement" },
            {WaterFireAttunement, "Water Fire Attunement" },
            {WaterAirAttunement, "Water Air Attunement" },
            {WaterEarthAttunement, "Water Earth Attunement" },

            {DualAirAttunement, "Dual Air Attunement" },
            {AirFireAttunement, "Air Fire Attunement" },
            {AirWaterAttunement, "Air Water Attunement" },
            {AirEarthAttunement, "Air Earth Attunement" },

            {DualEarthAttunement, "Dual Earth Attunement" },
            {EarthFireAttunement, "Earth Fire Attunement" },
            {EarthWaterAttunement, "Earth Water Attunement" },
            {EarthAirAttunement, "Earth Air Attunement" },

            {TrueNature, "True Nature - Dragon" },
            {51714, "True Nature - Demon" },
            {51675, "True Nature - Dwarf" },
            {51667, "True Nature - Assassin" },
            {51713, "True Nature - Centaur" },
            {SoulStoneVenomSkill, "Soul Stone Venom" },
            {49077, "Soul Stone Venom Strike" },
            {MantraOfSolace, "Mantra of Solace" },
            {43260, "Desert Empowerment" },
        };
        private static readonly Dictionary<long, string> _overrideIcons = new Dictionary<long, string>()
        {
            {Resurrect, "https://wiki.guildwars2.com/images/3/3d/Downed_ally.png"},
            {Bandage, "https://wiki.guildwars2.com/images/0/0c/Bandage.png"},
            {ArcDPSGenericBreakbar, "https://wiki.guildwars2.com/images/a/ae/Unshakable.png"},
            {ArcDPSDodge, "https://wiki.guildwars2.com/images/archive/b/b2/20150601155307%21Dodge.png"},
            {ArcDPSGenericBreakbar20220307, "https://wiki.guildwars2.com/images/a/ae/Unshakable.png"},
            {ArcDPSDodge20220307, "https://wiki.guildwars2.com/images/archive/b/b2/20150601155307%21Dodge.png"},
            {MirageCloakDodge, "https://wiki.guildwars2.com/images/a/a5/Mirage_Cloak_%28effect%29.png"},
            {WeaponSwap, "https://wiki.guildwars2.com/images/c/ce/Weapon_Swap_Button.png"},
            {49112, "https://wiki.guildwars2.com/images/e/e7/Throw_Magnetic_Bomb.png"},
            {49063, "https://wiki.guildwars2.com/images/3/3d/Detonate_Plasma.png"},
            {49123, "https://wiki.guildwars2.com/images/d/dd/Unstable_Artifact.png"},
            {31686, "https://wiki.guildwars2.com/images/4/4b/Overload_Air.png" },
            {52325, "https://wiki.guildwars2.com/images/5/59/Conjured_Slash.png" },
            {52780, "https://wiki.guildwars2.com/images/0/02/Conjured_Protection.png" },
            //{41243, "https://wiki.guildwars2.com/images/f/fb/Full_Counter.png" },
            {BoundingDodgerSkill, "https://wiki.guildwars2.com/images/3/30/Bounding_Dodger.png"},
            //{10281, "https://wiki.guildwars2.com/images/9/91/Illusionary_Riposte.png"},
            //{38769, "https://wiki.guildwars2.com/images/4/48/Phantasmal_Swordsman.png"},
            {LossAversion, "https://wiki.guildwars2.com/images/8/85/Loss_Aversion.png" },
            {SigilOfAir, "https://wiki.guildwars2.com/images/c/c3/Superior_Sigil_of_Air.png" },
            {SigilOfEarth, "https://wiki.guildwars2.com/images/4/43/Superior_Sigil_of_Geomancy.png" },
            {40015, "https://wiki.guildwars2.com/images/c/c9/Chapter_4-_Scorched_Aftermath.png"},
            {45128, "https://wiki.guildwars2.com/images/b/bf/Chapter_3-_Azure_Sun.png"},
            {41258, "https://wiki.guildwars2.com/images/d/d3/Chapter_1-_Searing_Spell.png"},
            {45022, "https://wiki.guildwars2.com/images/f/fd/Chapter_1-_Desert_Bloom.png"},
            {40787, "https://wiki.guildwars2.com/images/f/fd/Chapter_1-_Desert_Bloom.png"},
            {FlameRush, "https://wiki.guildwars2.com/images/a/a8/Flame_Rush.png"},
            {40635, "https://wiki.guildwars2.com/images/5/53/Chapter_2-_Igniting_Burst.png"},
            {42898, "https://wiki.guildwars2.com/images/6/6d/Epilogue-_Ashes_of_the_Just.png"},
            {42986, "https://wiki.guildwars2.com/images/3/30/Chapter_1-_Unflinching_Charge.png"},
            {KingOfFires, "https://wiki.guildwars2.com/images/7/70/King_of_Fires.png"},
            {EarthenBlast, "https://wiki.guildwars2.com/images/e/e2/Earthen_Blast.png"},
            {40071, "https://wiki.guildwars2.com/images/4/40/Garish_Pillar.png" },
            {46726, "https://wiki.guildwars2.com/images/0/08/Desert_Shroud.png" },
            {54870, "https://wiki.guildwars2.com/images/3/34/Sandstorm_Shroud.png" },
            {41968,  "https://wiki.guildwars2.com/images/7/79/Chapter_2-_Daring_Challenge.png"},
            {FlameSurge, "https://wiki.guildwars2.com/images/7/7e/Flame_Surge.png" },
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
            {SigilOfHydromancy, "https://wiki.guildwars2.com/images/3/33/Superior_Sigil_of_Hydromancy.png" },
            {56911, "https://wiki.guildwars2.com/images/6/67/Pitfall.png" },
            {40679, "https://wiki.guildwars2.com/images/9/95/Chapter_2-_Radiant_Recovery.png" },
            {42008, "https://wiki.guildwars2.com/images/1/16/Chapter_4-_Shining_River.png" },
            {42925, "https://wiki.guildwars2.com/images/5/5f/Epilogue-_Eternal_Oasis.png"},
            {ThermalReleaseValve, "https://wiki.guildwars2.com/images/0/0c/Thermal_Release_Valve.png" },
            {ShatteredAegis, "https://wiki.guildwars2.com/images/d/d0/Shattered_Aegis.png" },
            {ChillingNova, "https://wiki.guildwars2.com/images/8/82/Chilling_Nova.png" },
            {SelflessDaring, "https://wiki.guildwars2.com/images/9/9c/Selfless_Daring.png" },
            {12538, "https://wiki.guildwars2.com/images/1/11/Signet_of_Renewal.png" },
            {OpeningStrike, "https://wiki.guildwars2.com/images/9/9e/Opening_Strike.png" },
            {43558, "https://wiki.guildwars2.com/images/7/73/Rugged_Growth.png" },
            {12836, "https://wiki.guildwars2.com/images/thumb/f/f3/Healing.png/30px-Healing.png" }, // Water Blast Combo
            {12825, "https://wiki.guildwars2.com/images/thumb/f/f3/Healing.png/30px-Healing.png" }, // Water Blast Combo
            {MantraOfTruthDamage, "https://wiki.guildwars2.com/images/f/ff/Echo_of_Truth.png" }, // Echo of Truth
            {54935, "https://render.guildwars2.com/file/E60C094A2349552EA6F6250D9B14E69BE91E4468/1128595.png" }, // Shredder Gyro damage
            {SoulStoneVenomSkill, "https://wiki.guildwars2.com/images/d/d6/Soul_Stone_Venom.png" }, // Soul Stone Venom
            {49077, "https://wiki.guildwars2.com/images/d/d6/Soul_Stone_Venom.png" }, // Soul Stone Venom Strike
            {20479, "https://wiki.guildwars2.com/images/e/ec/Superior_Rune_of_Tormenting.png" },
            {31707, "https://wiki.guildwars2.com/images/a/a2/Cultivated_Synergy.png" },
            {31629, "https://wiki.guildwars2.com/images/a/a2/Cultivated_Synergy.png" },
            {24241, "https://wiki.guildwars2.com/images/f/f9/Superior_Sigil_of_Water.png" },
            {31371, "https://wiki.guildwars2.com/images/a/a9/Solar_Beam.png" },
            {12567, "https://wiki.guildwars2.com/images/a/a3/Spirit_of_Nature.png" },
            {29997, "https://wiki.guildwars2.com/images/f/f4/Healing_Prism.png" },
            {29901, "https://wiki.guildwars2.com/images/5/5e/Life_from_Death.png" },
            {46232, "https://wiki.guildwars2.com/images/a/a7/Breakrazor%27s_Bastion.png" },
            {40774, "https://wiki.guildwars2.com/images/f/f0/Sand_Flare.png" },
            {SandCascadeBarrier, "https://wiki.guildwars2.com/images/1/1e/Sand_Cascade.png" },
            {31536, "https://wiki.guildwars2.com/images/f/ff/Astral_Wisp.png" },
            {29863, "https://wiki.guildwars2.com/images/6/64/Live_Vicariously.png" },
            {49103, "https://wiki.guildwars2.com/images/7/7a/Signet_of_the_Ether.png" },
            {14024, "https://wiki.guildwars2.com/images/c/c1/Natural_Healing_%28ranger_trait%29.png" },
            {46299, "https://wiki.guildwars2.com/images/c/cc/Predator%27s_Cunning.png" },
            {21776, "https://wiki.guildwars2.com/images/0/07/Aqua_Surge.png" },
            {43260, "https://wiki.guildwars2.com/images/c/c3/Desert_Empowerment.png" },
            {26997, "https://wiki.guildwars2.com/images/d/d9/Natural_Harmony.png" },
            {27101, "https://wiki.guildwars2.com/images/e/e7/Project_Tranquility.png" },
            {35417, "https://wiki.guildwars2.com/images/b/b6/Ventari%27s_Will.png" },
            {DeathDropDodge, "https://wiki.guildwars2.com/images/archive/b/b2/20150601155307%21Dodge.png" },
            {SaintsShieldDodge, "https://wiki.guildwars2.com/images/archive/b/b2/20150601155307%21Dodge.png" },
            {ImperialImpactDodge, "https://wiki.guildwars2.com/images/archive/b/b2/20150601155307%21Dodge.png" },
            // Weaver attunements
            {DualFireAttunement, "https://wiki.guildwars2.com/images/b/b4/Fire_Attunement.png" },
            {FireWaterAttunement, "https://i.imgur.com/ar8Hn8G.png" },
            {FireAirAttunement, "https://i.imgur.com/YU31LwG.png" },
            {FireEarthAttunement, "https://i.imgur.com/64g3rto.png" },

            {DualWaterAttunement, "https://wiki.guildwars2.com/images/3/31/Water_Attunement.png" },
            {WaterFireAttunement, "https://i.imgur.com/H1peqpz.png" },
            {WaterAirAttunement, "https://i.imgur.com/Gz1XwEw.png" },
            {WaterEarthAttunement, "https://i.imgur.com/zqX3y4c.png" },

            {DualAirAttunement, "https://wiki.guildwars2.com/images/9/91/Air_Attunement.png" },
            {AirFireAttunement, "https://i.imgur.com/4ekncW5.png" },
            {AirWaterAttunement, "https://i.imgur.com/HIcUaXG.png" },
            {AirEarthAttunement, "https://i.imgur.com/MCCrMls.png" },

            {DualEarthAttunement, "https://wiki.guildwars2.com/images/a/a8/Earth_Attunement.png" },
            {EarthFireAttunement, "https://i.imgur.com/Vgu0B54.png" },
            {EarthWaterAttunement, "https://i.imgur.com/exrTKSW.png" },
            {EarthAirAttunement, "https://i.imgur.com/Z3P8cPa.png" },
            //
            {HauntShot, "https://wiki.guildwars2.com/images/b/be/Haunt_Shot.png" },
            {GraspingShadows, "https://wiki.guildwars2.com/images/e/ef/Grasping_Shadows.png" },
            {DawnsRepose, "https://wiki.guildwars2.com/images/3/31/Dawn%27s_Repose.png" },
            {EternalNight, "https://wiki.guildwars2.com/images/7/7b/Eternal_Night.png" },
            {MindShock, "https://wiki.guildwars2.com/images/e/e6/Mind_Shock.png" },
        };

        private static readonly Dictionary<long, ulong> _nonCritable = new Dictionary<long, ulong>
                    {
                        { SigilOfAir, GW2Builds.StartOfLife }, // Lightning Strike (Sigil)
                        { FireAttunementSkill, GW2Builds.December2018Balance },  // Fire Attunement
                        { Mug, GW2Builds.StartOfLife }, // Mug
                        { 30770, 54485 }, // Pulmonary Impact
                        { 52370, GW2Builds.StartOfLife },
                        { 31686, GW2Builds.StartOfLife }, // Lightning Jolt
                        { Sunspot, GW2Builds.December2018Balance }, // Sunspot
                        { EarthenBlast, GW2Builds.December2018Balance }, // Earthen Blast
                        { ChillingNova, GW2Builds.December2018Balance }, // Chilling Nova
                                         // Spontaneous Destruction GW2Builds.December2018Balance
                        {LesserEnfeeble, GW2Builds.December2018Balance }, // Weakening Shroud
                        {SpitefulSpirit, GW2Builds.December2018Balance }, // Spiteful Spirit
                        {LesserSpinalShivers, GW2Builds.December2018Balance }, // Chill of Death
                                         // Power block GW2Builds.December2018Balance
                        {ShatteredAegis, GW2Builds.December2018Balance }, // Shattered Aegis
                        {GlacialHeart, GW2Builds.December2018Balance }, // Glacial Heart
                        {ThermalReleaseValve, GW2Builds.December2018Balance }, // Thermal Release Valve
                        {LossAversion, GW2Builds.December2018Balance }, // Loss Aversion
                        // 
                    };

        private const string DefaultIcon = "https://render.guildwars2.com/file/1D55D34FB4EE20B1962E315245E40CA5E1042D0E/62248.png";

        // Fields
        public long ID { get; }
        //public int Range { get; private set; } = 0;
        public bool AA { get; }

        public bool IsSwap => ID == WeaponSwap || ElementalistHelper.IsElementalSwap(ID) || RevenantHelper.IsLegendSwap(ID);
        public bool IsInstantTransformation => NecromancerHelper.IsShroudTransform(ID) || EngineerHelper.IsEngineerKit(ID) || HolosmithHelper.IsPhotonForgeTransform(ID) || DruidHelper.IsCelestialAvatarTransform(ID) || SpecterHelper.IsShroudTransform(ID) || BladeswornHelper.IsGunsaberForm(ID);
        public bool IsDodge(SkillData skillData) => IsAnimatedDodge(skillData) || ID == MirageCloakDodge;
        public bool IsAnimatedDodge(SkillData skillData) => ID == skillData.DodgeId || VindicatorHelper.IsVindicatorDodge(ID);
        public string Name { get; }
        public string Icon { get; }
        private readonly WeaponDescriptor _weaponDescriptor;
        public bool IsWeaponSkill => _weaponDescriptor != null;
        internal GW2APISkill ApiSkill { get; }
        private SkillInfoEvent _skillInfo { get; set; }

        internal const string DefaultName = "UNKNOWN";

        public bool UnknownSkill => Name == DefaultName;

        // Constructor

        internal SkillItem(long ID, string name, GW2APIController apiController)
        {
            this.ID = ID;
            Name = name.Replace("\0", "");
            ApiSkill = apiController.GetAPISkill(ID);
            //
            if (_overrideNames.TryGetValue(ID, out string overrideName))
            {
                Name = overrideName;
            } 
            else if (ApiSkill != null && (UnknownSkill || Name.All(char.IsDigit)))
            {
                Name = ApiSkill.Name;
            }
            if (_overrideIcons.TryGetValue(ID, out string icon))
            {
                Icon = icon;
            }
            else
            {
                Icon = ApiSkill != null ? ApiSkill.Icon : DefaultIcon;
            }
            if (ApiSkill != null && ApiSkill.Type == "Weapon" 
                && ApiSkill.WeaponType != "None" && ApiSkill.Professions.Count > 0 
                && (ApiSkill.Categories == null || ApiSkill.Categories.Count == 0 
                    || ApiSkill.Categories.Contains("Clone") || ApiSkill.Categories.Contains("Phantasm") 
                    || ApiSkill.Categories.Contains("DualWield")))
            {
                _weaponDescriptor = new WeaponDescriptor(ApiSkill);
            }
            AA = (ApiSkill?.Slot == "Weapon_1" || ApiSkill?.Slot == "Downed_1");
            if (AA)
            {
                if (ApiSkill.Categories != null)
                {
                    AA = AA && !ApiSkill.Categories.Contains("StealthAttack") && !ApiSkill.Categories.Contains("Ambush"); // Ambush in case one day it's added
                }
                if (ApiSkill.Description != null)
                {
                    AA = AA && !ApiSkill.Description.Contains("Ambush.");
                }
            }
#if DEBUG
            Name += " (" + ID + ")";
#endif
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
                swapped = _weaponDescriptor.FindWeaponSlot(swaps);
            }
            return swapped;
        }

        internal bool EstimateWeapons(WeaponSets weaponSets, int swapped, bool validForCurrentSwap)
        {
            bool keep = swapped == WeaponSetIDs.FirstLandSet || swapped == WeaponSetIDs.SecondLandSet || swapped == WeaponSetIDs.FirstWaterSet || swapped == WeaponSetIDs.SecondWaterSet;
            if (_weaponDescriptor == null || !keep || !validForCurrentSwap)
            {
                return false;
            }
            weaponSets.SetWeapons(_weaponDescriptor, ApiSkill, swapped);
            return true;
        }

        internal void AttachSkillInfoEvent(SkillInfoEvent skillInfo)
        {
            if (ID == skillInfo.SkillID)
            {
                _skillInfo = skillInfo;
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
