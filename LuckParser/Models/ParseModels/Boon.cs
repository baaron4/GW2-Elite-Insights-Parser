using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class Boon
    {
        // Boon
        

        // Fields
        private String name;
        private int id;
        private String plotlyGroup;
        private String type;
        private int capacity;

        // Constructor abrv does not matter unused
        private Boon(String name, String group, String type, int capacity)
        {
            this.name = name;
            this.plotlyGroup = group;
            this.type = type;
            this.capacity = capacity;
        }
        private Boon(String name, int id, String group, String type, int capacity)
        {
            this.name = name;
            this.id = id;
            this.plotlyGroup = group;
            this.type = type;
            this.capacity = capacity;
        }
        // Public Methods
        public void setID(int id)
        {
            this.id = id;
        }
        public static Boon getEnum(String name)
        {
            foreach (Boon b in getList())
            {
                if (b.getName() == name)
                {
                    return b;
                }
            }
            foreach (Boon b in getAllProfList())
            {
                if (b.getName() == name)
                {
                    return b;
                }
            }
            return null;
        }

        public static List<int> getCondiList() {
            List<int> condiList = new List<int>
            {
                736,//Bleeding
                737,//Burning
                861,//Confusion
                723,//Poisin
                19426,//Torment
                720,//Blind
                722,//Chilled
                721,//Cripplied
                791,//Fear
                727,//Immob
                26766,//Slow
                27705,//Taunt
                742,//Weakness
                738//Vuln
            };
            return condiList;
        }
        public static string getCondiName(int id) {
            switch (id) {
                case 736:
                    return "Bleeding";
                case 737:
                    return "Burning";
                case 861:
                    return "Confusion";
                case 723:
                    return "Poison";
                case 19426:
                    return "Torment";
                case 720:
                    return "Blinded";
                case 722:
                    return "Chilled";
                case 721:
                    return "Crippled";
                case 791:
                    return "Fear";
                case 727:
                    return "Immobile";
                case 26766:
                    return "Slow";
                case 27705:
                    return "Taunt";
                case 742:
                    return "Weakness";
                case 738:
                    return "Vulnerability";
                case 873:
                    return "Retaliation";
                default:
                    return "UNKNOWN";
            }
        }
        public static List<Boon> getCondiBoonList() {
            List<Boon> boonList = new List<Boon>
            {
                new Boon("Bleeding", 736, "main", "intensity", 1500),
                new Boon("Burning", 737, "main", "intensity", 1500),
                new Boon("Confusion", 861, "main", "intensity", 1500),
                new Boon("Poison", 723, "main", "intensity", 1500),
                new Boon("Torment", 19426, "main", "intensity", 1500),
                new Boon("Blinded", 737, "main", "duration", 9),
                new Boon("Chilled", 722, "main", "duration", 5),
                new Boon("Crippled", 721, "main", "duration", 9),
                new Boon("Fear", 791, "main", "duration", 9),
                new Boon("Immobile", 727, "main", "duration", 3),
                new Boon("Slow", 26766, "main", "duration", 9),
                new Boon("Weakness", 737, "main", "duration", 5),
                new Boon("Vulnerability", 738, "main", "intensity", 25)
            };
            return boonList;
        }
        public static List<Boon> getList(){
            List<Boon> boonList = new List<Boon>
            {
                new Boon("Might", 740, "main", "intensity", 25),
                new Boon("Fury", 725, "main", "duration", 9),//or 3m and 30s
                new Boon("Quickness", 1187, "main", "duration", 5),
                new Boon("Alacrity", 30328, "main", "duration", 9),

                new Boon("Protection", 717, "main", "duration", 5),
                new Boon("Regeneration", 718, "main", "duration", 5),
                new Boon("Vigor", 726, "main", "duration", 5),
                new Boon("Aegis", 743, "main", "duration", 5),
                new Boon("Stability", 1122, "main", "intensity", 25),
                new Boon("Swiftness", 719, "main", "duration", 9),
                new Boon("Retaliation", 873, "main", "duration", 5),
                new Boon("Resistance", 26980, "main", "duration", 5),
                // ranger
                // The generation and the uptime does not use the same id for spirits
                new Boon("Spotter", 14055, "ranger", "duration", 1),
                new Boon("Water Spirit", 50386, "ranger", "duration", 1),
                new Boon("Frost Spirit", 50421, "ranger", "duration", 1),
                new Boon("Sun Spirit", 50413, "ranger", "duration", 1),
                new Boon("Stone Spirit", 50415, "ranger", "duration", 1),
                new Boon("Storm Spirit", 50381, "ranger", "duration", 1),
                new Boon("Glyph of Empowerment", 31803, "ranger", "duration", 1),
                // warrior
                new Boon("Empower Allies", 14222, "warrior", "duration", 1),
                new Boon("Banner of Strength", 14417, "warrior", "duration", 1),
                new Boon("Banner of Discipline", 14449, "warrior", "duration", 1),
                // el
                new Boon("Soothing Mist", 5587, "ele", "duration", 1),
                // engie
                new Boon("Pinpoint Distribution", 38333, "engie", "duration", 1),
                // necro
                new Boon("Vampiric Aura", 30285, "necro", "duration", 1),
                // rev
                new Boon("Assassin's Presence", 26854, "rev", "duration", 1),
                // guard
                //new Boon("Battle Presence", 17046, "guard", "duration", 1)//Virtue of Resolve (ID:17046)
            };
            return boonList;
        }

        public static List<Boon> getMainList()
        {
            List<Boon> boonList = new List<Boon>
            {
                new Boon("Might", 740, "main", "intensity", 25),
                new Boon("Fury", 725, "main", "duration", 9),//or 3m and 30s
                new Boon("Quickness", 1187, "main", "duration", 5),
                new Boon("Alacrity", 30328, "main", "duration", 9),

                new Boon("Protection", 717, "main", "duration", 5),
                new Boon("Regeneration", 718, "main", "duration", 5),
                new Boon("Vigor", 726, "main", "duration", 5),
                new Boon("Aegis", 743, "main", "duration", 9),
                new Boon("Stability", 1122, "main", "duration", 9),
                new Boon("Swiftness", 719, "main", "duration", 9),
                new Boon("Retaliation", 873, "main", "duration", 9),
                new Boon("Resistance", 26980, "main", "duration", 5)
            };

            return boonList;
        }
        public static List<Boon> getSharableProfList()
        {
            List<Boon> boonList = new List<Boon>
            {
                // ranger
                new Boon("Spotter", 14055, "ranger", "duration", 1),
                new Boon("Water Spirit", 50386, "ranger", "duration", 1),
                new Boon("Frost Spirit", 50421, "ranger", "duration", 1),
                new Boon("Sun Spirit", 50413, "ranger", "duration", 1),
                new Boon("Stone Spirit", 50415, "ranger", "duration", 1),
                new Boon("Storm Spirit", 50381, "ranger", "duration", 1),
                new Boon("Glyph of Empowerment", 31803, "ranger", "duration", 1),
                // warrior
                new Boon("Empower Allies", 14222, "warrior", "duration", 1),
                new Boon("Banner of Strength", 14417, "warrior", "duration", 1),
                new Boon("Banner of Discipline", 14449, "warrior", "duration", 1),
                new Boon("Banner of Defence", 14543, "warrior", "duration", 1),
                new Boon("Banner of tactics", 14408, "warrior", "duration", 1),
                // guard
                new Boon("Battle Presence", 17046, "guard", "duration", 1),
                // rev
                new Boon("Assassin's Presence", 26854, "rev", "duration", 1),
                new Boon("Naturalistic Resonance", 29379, "rev", "duration", 1),
                // engie
                new Boon("Pinpoint Distribution", 38333, "engie", "duration", 1),
                // ele
                new Boon("Soothing Mist", 5587, "ele", "duration", 1),
                // necro
                new Boon("Vampiric Aura", 30285, "necro", "duration", 1)
            };
            return boonList;
        }
        public static List<Boon> getAllProfList()
        {
            List<Boon> boonList = new List<Boon>
            {
                new Boon("Stealth", 13017, "regular", "duration", 1),
                new Boon("Revealed", 890, "regular", "duration", 1),
                new Boon("Superspeed", 5974, "regular", "duration", 1),
                new Boon("Invulnerability", 801, "regular", "duration", 1),
                new Boon("Unblockable", "regular", "duration", 1),
                //Auras
                new Boon("Chaos Armor", 10332, "aura", "duration", 1),
                new Boon("Fire Shield", 5677, "aura", "duration", 1),
                new Boon("Frost Aura", 5579, "aura", "duration", 1),
                new Boon("Light Aura", 25518, "aura", "duration", 1),
                new Boon("Magnetic Aura", 5684, "aura", "duration", 1),
                new Boon("Shocking Aura", 5577, "aura", "duration", 1),
                //race
                new Boon("Take Root", 12459, "race", "duration", 1),
                new Boon("Become the Bear",12426, "race", "duration", 1),
                new Boon("Become the Raven",12405, "race", "duration", 1),
                new Boon("Become the Snow Leopard",12400, "race", "duration", 1),
                new Boon("Become the Wolf",12393, "race", "duration", 1),
                new Boon("Avatar of Melandru", 12368, "race", "duration", 1),
                new Boon("Power Suit",12326, "race", "duration", 1),
                new Boon("Reaper of Grenth", 12366, "race", "duration", 1),
                new Boon("Charrooka",43503, "race", "duration", 1),
                ///REVENANT
                //skills
                new Boon("Crystal Hibernation", 28262, "rev", "duration", 1),
                new Boon("Vengeful Hammers", 27273, "rev", "duration", 1),
                new Boon("Rite of the Great Dwarf", 26596, "rev", "duration", 1),
                new Boon("Embrace the Darkness", 28001, "rev", "duration", 1),
                new Boon("Enchanted Daggers", 28557, "rev", "intensity", 6),
                new Boon("Impossible Odds", 27581, "rev", "duration", 1),
                //facets
                new Boon("Facet of Light",27336, "rev", "duration", 1),
                new Boon("Infuse Light",27737, "rev", "duration", 1),
                new Boon("Facet of Darkness",28036, "rev", "duration", 1),
                new Boon("Facet of Elements",28243, "rev", "duration", 1),
                new Boon("Facet of Strength",27376, "rev", "duration", 1),
                new Boon("Facet of Chaos",27983, "rev", "duration", 1),
                new Boon("Facet of Nature",29275, "rev", "duration", 1),
                new Boon("Naturalistic Resonance", 29379, "rev", "duration", 1),
                //legends
                new Boon("Legendary Centaur Stance",27972, "rev", "duration", 1),
                new Boon("Legendary Dragon Stance",27732, "rev", "duration", 1),
                new Boon("Legendary Dwarf Stance",27205, "rev", "duration", 1),
                new Boon("Legendary Demon Stance",27928, "rev", "duration", 1),
                new Boon("Legendary Assassin Stance",27890, "rev", "duration", 1),
                new Boon("Legendary Renegade Stance",44272, "rev", "duration", 1),
                //summons
                new Boon("Breakrazor's Bastion",44682, "rev", "duration", 1),
                new Boon("Razorclaw's Rage",41016, "rev", "duration", 1),
                new Boon("Soulcleave's Summit",45026, "rev", "duration", 1),
                //traits
                new Boon("Vicious Lacerations",29395, "rev", "intensity", 5),
                new Boon("Assassin's Presence", 26854, "rev", "duration", 1),
                new Boon("Expose Defenses", 48894, "rev", "duration", 1),
                new Boon("Invoking Harmony",29025, "rev", "duration", 1),
                new Boon("Selfless Amplification",30509, "rev", "duration", 1),
                new Boon("Hardening Persistence",28957, "rev", "intensity", 8),
                new Boon("Soothing Bastion",34136, "rev", "duration", 1),
                new Boon("Kalla's Fervor",42883, "rev", "duration", 5),
                new Boon("Improved Kalla's Fervor",45614, "rev", "duration", 5),
                ///WARRIOR
                //skills
                new Boon("Riposte",14434, "warrior", "duration", 1),
                new Boon("Counterattack",14509, "warrior", "duration", 1),
                new Boon("Flames of War", 31708, "warrior", "duration", 1),
                new Boon("Blood Reckoning", 29466 , "warrior", "duration", 1),
                new Boon("Rock Guard", 34256 , "warrior", "duration", 1),
                new Boon("Sight beyond Sight",40616, "warrior", "duration", 1),
                //signets
                new Boon("Healing Signet",786, "warrior", "duration", 1),
                new Boon("Dolyak Signet",14458, "warrior", "duration", 1),
                new Boon("Signet of Fury",14459, "warrior", "duration", 1),
                new Boon("Signet of Might",14444, "warrior", "duration", 1),
                new Boon("Signet of Stamina",14478, "warrior", "duration", 1),
                new Boon("Signet of Rage",14496, "warrior", "duration", 1),
                //banners
                new Boon("Banner of Strength", 14417, "warrior", "duration", 1),
                new Boon("Banner of Discipline", 14449, "warrior", "duration", 1),
                new Boon("Banner of Tactics",14450, "warrior", "duration", 1),
                new Boon("Banner of Defense",14543, "warrior", "duration", 1),
                //stances
                new Boon("Shield Stance",756, "warrior", "duration", 1),
                new Boon("Berserker's Stance",14453, "warrior", "duration", 1),
                new Boon("Enduring Pain",787, "warrior", "duration", 1),
                new Boon("Balanced Stance",34778, "warrior", "duration", 1),
                new Boon("Defiant Stance",21816, "warrior", "duration", 1),
                new Boon("Rampage",14484, "warrior", "duration", 1),
                //traits
                new Boon("Empower Allies", 14222, "warrior", "duration", 1),
                new Boon("Peak Performance",46853, "warrior", "duration", 1),
                new Boon("Furious Surge", 30204, "warrior", "intensity", 25),
                new Boon("Health Gain per Adrenaline bar Spent", "warrior", "intensity", 3),
                new Boon("Rousing Resilience",24383, "warrior", "duration", 1),
                new Boon("Always Angry",34099, "warrior", "duration", 1),
                new Boon("Full Counter",43949, "warrior", "duration", 1),
                new Boon("Attacker's Insight",41963, "warrior", "intensity", 5),
                /// GUARDIAN
                //skills
                new Boon("Zealot's Flame", 9103, "guard", "duration", 1),
                new Boon("Purging Flames",21672, "guard", "duration", 1),
                new Boon("Litany of Wrath",21665, "guard", "duration", 1),
                new Boon("Renewed Focus",9255, "guard", "duration", 1),
                new Boon("Ashes of the Just",41957, "guard", "intensity", 25),
                new Boon("Eternal Oasis",44871, "guard", "duration", 1),
                new Boon("Unbroken Lines",43194, "guard", "duration", 1),
                //signets
                new Boon("Signet of Resolve",9220, "guard", "duration", 1),
                new Boon("Bane Signet",9092, "guard", "duration", 1),
                new Boon("Signet of Judgment",9156, "guard", "duration", 1),
                new Boon("Signet of Mercy",9162, "guard", "duration", 1),
                new Boon("Signet of Wrath",9100, "guard", "duration", 1),
                new Boon("Signet of Courage",29633, "guard", "duration", 1),
                //virtues
                new Boon("Virtue of Justice", 9114, "guard", "duration", 1),
                new Boon("Spears of Justice", 29632, "guard", "duration", 1),
                new Boon("Virtue of Courage", 9113, "guard", "duration", 1),
                new Boon("Shield of Courage", 29523, "guard", "duration", 1),
                new Boon("Virtue of Resolve", 9119, "guard", "duration", 1),
                new Boon("Wings of Resolve", 30308, "guard", "duration", 1),
                new Boon("Tome of Justice",40530, "guard", "duration", 1),
                new Boon("Tome of Courage",43508,"guard", "duration", 1),
                new Boon("Tome of Resolve",46298, "guard", "duration", 1),
                //traits
                new Boon("Strength in Numbers",13796, "guard", "duration", 1),
                new Boon("Invigorated Bulwark",30207, "guard", "intensity", 5),
                new Boon("Battle Presence", 17046, "guard", "duration", 1),
                new Boon("Force of Will",29485, "guard", "duration", 1),//not sure if intensity
                new Boon("Quickfire",45123, "guard", "duration", 1),
                ///ENGINEER
                //skills
                new Boon("Static Shield",6055, "engie", "duration", 1),
                new Boon("Absorb",6056, "engie", "duration", 1),
                new Boon("A.E.D.",21660, "engie", "duration", 1),
                new Boon("Elixir S",5863, "engie", "duration", 1),
                new Boon("Elixir X", "engie", "duration", 1),
                new Boon("Utility Goggles",5864, "engie", "duration", 1),
                new Boon("Slick Shoes",5833, "engie", "duration", 1),
                new Boon("Watchful Eye", "engie", "duration", 1),
                new Boon("Cooling Vapor",46444, "engie", "duration", 1),
                new Boon("Photon Wall Deployed",46094, "engie", "duration", 1),
                new Boon("Spectrum Shield",43066, "engie", "duration", 1),
                new Boon("Gear Shield",5997, "engie", "duration", 1),
                //Transforms
                new Boon("Rampage", "engie", "duration", 1),
                new Boon("Photon Forge",43708, "engie", "duration", 1),
                //Traits
                new Boon("Laser's Edge",44414, "engie", "duration", 1),
                new Boon("Afterburner",42210, "engie", "intensity", 5),
                new Boon("Iron Blooded",49065, "engie", "intensity", 25),
                new Boon("Streamlined Kits",18687, "engie", "duration", 1),
                new Boon("Kinetic Charge",45781, "engie", "intensity", 5),
                new Boon("Pinpoint Distribution", 38333, "engie", "duration", 1),
                new Boon("Heat Therapy",40694, "engie", "duration", 1),
                new Boon("Overheat", 40397, "engie", "duration", 1),
                ///RANGER
                new Boon("Celestial Avatar", 31508, "ranger", "duration", 1),
                //signets
                new Boon("Signet of Renewal",41147, "ranger", "duration", 1),
                new Boon("Signet of Stone",12627, "ranger", "duration", 1),
                new Boon("Signet of the Hunt",12626, "ranger", "duration", 1),
                new Boon("Signet of the Wild",12636, "ranger", "duration", 1),
                //spirits
                new Boon("Water Spirit", 50386, "ranger", "duration", 1),
                new Boon("Frost Spirit", 50421, "ranger", "duration", 1),
                new Boon("Sun Spirit", 50413, "ranger", "duration", 1),
                new Boon("Stone Spirit", 50415, "ranger", "duration", 1),
                new Boon("Storm Spirit", 50381, "ranger", "duration", 1),
                //skills
                new Boon("Attack of Opportunity",12574, "ranger", "duration", 1),
                new Boon("Call of the Wild",36781, "ranger", "duration", 1),
                new Boon("Strength of the pack!",12554, "ranger", "duration", 1),
                new Boon("Sick 'Em!",33902, "ranger", "duration", 1),
                new Boon("Sharpening Stones",12536, "ranger", "intenstiy", 10),
                new Boon("Ancestral Grace", 31584, "ranger", "duration", 1),
                new Boon("Glyph of Empowerment", 31803, "ranger", "duration", 1),
                new Boon("Dolyak Stance",41815, "ranger", "duration", 1),
                new Boon("Griffon Stance",46280, "ranger", "duration", 1),
                new Boon("Moa Stance",45038, "ranger", "duration", 1),
                new Boon("Vulture Stance",44651, "ranger", "duration", 1),
                new Boon("Bear Stance",40045, "ranger", "duration", 1),
                new Boon("One Wolf Pack",44139, "ranger", "duration", 1),
                new Boon("Sharpen Spines",43266, "ranger", "intensity", 5),
                //traits
                new Boon("Spotter", 14055, "ranger", "duration", 1),
                new Boon("Opening Strike",13988, "ranger", "duration", 1),
                new Boon("Quick Draw",29703, "ranger", "duration", 1),
                new Boon("Light on your feet",30673, "ranger", "duration", 1),
                new Boon("Natural Mender",30449, "ranger", "duration", 1),
                new Boon("Lingering Light",32248, "ranger", "duration", 1),
                new Boon("Deadly",44932, "ranger", "duration", 1),
                new Boon("Ferocious",41720, "ranger", "duration", 1),
                new Boon("Supportive",40069, "ranger", "duration", 1),
                new Boon("Versatile",44693, "ranger", "duration", 1),
                new Boon("Stout",40272, "ranger", "duration", 1),
                new Boon("Unstoppable Union",44439, "ranger", "duration", 1),
                new Boon("Twice as Vicious",45600, "ranger", "duration", 1),
                ///THIEF
                //signets
                new Boon("Signet of Malice",13049, "thief", "duration", 1),
                new Boon("Assassin's Signet (Passive)",13047, "thief", "duration", 1),
                new Boon("Assassin's Signet (Active)",44597, "thief", "duration", 1),
                new Boon("Infiltrator's Signet",13063, "thief", "duration", 1),
                new Boon("Signet of Agility",13061, "thief", "duration", 1),
                new Boon("Signet of Shadows",13059, "thief", "duration", 1),
                //venoms
                new Boon("Ice Drake Venom",13095, "thief", "intensity", 4),
                new Boon("Devourer Venom", 13094, "thief", "intensity", 2),
                new Boon("Skale Venom", 13036, "thief", "intensity", 4),
                new Boon("Spider Venom",13036, "thief", "intensity", 6),
                new Boon("Basilisk Venom", 13133, "thief", "intensity", 6),
                //physical
                new Boon("Palm Strike",30423, "thief", "duration", 1),
                new Boon("Pulmonary Impact",30510, "thief", "intensity", 2),
                //weapon
                new Boon("Infiltration",13135, "thief", "duration", 1),
                //transforms
                new Boon("Dagger Storm",13134, "thief", "duration", 1),
                new Boon("Kneeling",42869, "thief", "duration", 1),
                //traits
                new Boon("Deadeyes's Gaze",46333, "thief", "duration", 1),
                new Boon("Maleficent Seven",43606, "thief", "duration", 1),
                new Boon("Hidden Killer",42720, "thief", "duration", 1),
                new Boon("Lead Attacks",34659, "thief", "intensity", 15),
                new Boon("Instant Reflexes",34283, "thief", "duration", 1),
                new Boon("Lotus Training", 32200, "thief", "duration", 1),
                new Boon("Unhindered Combatant", 32931, "thief", "duration", 1),
                new Boon("Bounding Dodger", 33162, "thief", "duration", 1),
                ///MESMER
                //signets
                new Boon("Signet of the Ether", 21751, "mes", "duration", 1),
                new Boon("Signet of Domination",10231, "mes", "duration", 1),
                new Boon("Signet of Illusions",10246, "mes", "duration", 1),
                new Boon("Signet of Inspiration",10235, "mes", "duration", 1),
                new Boon("Signet of Midnight",10233, "mes", "duration", 1),
                new Boon("Signet of Humility",30739, "mes", "duration", 1),
                //skills
                new Boon("Distortion",10243, "mes", "duration", 1),
                new Boon("Blur", 10335 , "mes", "duration", 1),
                new Boon("Mirror",10357, "mes", "duration", 1),
                new Boon("Echo",29664, "mes", "duration", 1),
                new Boon("Illusion of Life", "mes", "duration", 1),
                new Boon("Time Block",30134, "mes", "duration", 1),
                new Boon("Time Echo",29582, "mes", "duration", 1),
                new Boon("Time Anchored",30136, "mes", "duration", 1),
                //traits
                new Boon("Fencer's Finesse", 30426 , "mes", "intensity", 10),
                new Boon("Illusionary Defense",49099, "mes", "intensity", 5),
                new Boon("Compunding Power",49058, "mes", "intensity", 5),
                new Boon("Phantasmal Force", 44691 , "mes", "intensity", 25),
                new Boon("Mirage Cloak",40408, "mes", "duration", 1),
                ///NECROMANCER
                //forms
                new Boon("Lich Form",10631, "necro", "duration", 1),
                new Boon("Death Shroud", 790, "necro", "duration", 1),
                new Boon("Reaper's Shroud", 29446, "necro", "duration", 1),
                //signets
                new Boon("Signet of Vampirism",21761, "necro", "duration", 1),
                new Boon("Plague Signet",10630, "necro", "duration", 1),
                new Boon("Signet of Spite",10621, "necro", "duration", 1),
                new Boon("Signet of the Locust",10614, "necro", "duration", 1),
                new Boon("Signet of Undeath",10610, "necro", "duration", 1),
                //skills
                new Boon("Spectral Walk",15083, "necro", "duration", 1),
                new Boon("Infusing Terror", 30129, "necro", "duration", 1),
                //traits
                new Boon("Corrupter's Defense",30845, "necro", "intenstiy", 10),
                new Boon("Vampiric Aura", 30285, "necro", "duration", 1),
                new Boon("Last Rites",29726, "necro", "duration", 1),
                new Boon("Sadistic Searing",43626, "necro", "duration", 1),
                ///ELEMENTALIST
                //signets
                new Boon("Signet of Restoration",739, "ele", "duration", 1),
                new Boon("Signet of Air",5590, "ele", "duration", 1),
                new Boon("Signet of Earth",5592, "ele", "duration", 1),
                new Boon("Signet of Fire",5544, "ele", "duration", 1),
                new Boon("Signet of Water",5591, "ele", "duration", 1),
                //attunments
                new Boon("Fire Attunement", 5585, "ele", "duration", 1),
                new Boon("Water Attunement", 5586, "ele", "duration", 1),
                new Boon("Air Attunement", 5575, "ele", "duration", 1),
                new Boon("Earth Attunement", 5580, "ele", "duration", 1),
                //forms
                new Boon("Mist Form",5543, "ele", "duration", 1),
                new Boon("Ride the Lightning",5588, "ele", "duration", 1),
                new Boon("Vapor Form",5620, "ele", "duration", 1),
                new Boon("Tornado",5534, "ele", "duration", 1),
                new Boon("Whirlpool", "ele", "duration", 1),
                //conjures
                new Boon("Conjure Earth Shield", 15788, "ele", "duration", 1),
                new Boon("Conjure Flame Axe", 15789, "ele", "duration", 1),
                new Boon("Conjure Frost Bow", 15790, "ele", "duration", 1),
                new Boon("Conjure Lightning Hammer", 15791, "ele", "duration", 1),
                new Boon("Conjure Fiery Greatsword", 15792, "ele", "duration", 1),
                //skills
                new Boon("Arcane Power",5582, "ele", "duration", 1),
                new Boon("Arcane Shield",5640, "ele", "duration", 1),
                new Boon("Renewal of Fire",5764, "ele", "duration", 1),
                new Boon("Glyph of Elemental Power (Fire)",5739, "ele", "duration", 1),
                new Boon("Glyph of Elemental Power (Water)",5741, "ele", "duration", 1),
                new Boon("Glyph of Elemental Power (Air)",5740, "ele", "duration", 1),
                new Boon("Glyph of Elemental Power (Earth)",5742, "ele", "duration", 1),
                new Boon("Rebound",31337, "ele", "duration", 1),
                new Boon("Rock Barrier",34633, "ele", "duration", 1),//750?
                new Boon("Magnetic Wave",15794, "ele", "duration", 1),
                new Boon("Obsidian Flesh",5667, "ele", "duration", 1),
                //traits
                new Boon("Harmonious Conduit",31353, "ele", "duration", 1),
                new Boon("Fresh Air",31353, "ele", "duration", 1),
                new Boon("Soothing Mist", 5587, "ele", "duration", 1),
                new Boon("Lesser Arcane Shield",25579, "ele", "duration", 1),
                new Boon("Weaver's Prowess",42061, "ele", "duration", 1),
                new Boon("Elements of Rage",42416, "ele", "duration", 1),
            };

            return boonList;
        }
       

        // Getters
        public String getName()
        {
            return this.name;
        }
        public int getID()
        {
            return this.id;
        }
        public String getPloltyGroup()
        {
            return this.plotlyGroup;
        }

        public String getType()
        {
            return this.type;
        }

        public int getCapacity()
        {
            return this.capacity;
        }
    }
}