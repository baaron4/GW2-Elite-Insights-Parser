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
                new Boon("Storm Spirit", 12549, "ranger", "duration", 1),
                new Boon("Glyph of Empowerment", 31803, "ranger", "duration", 1),
                // warrior
                new Boon("Empower Allies", 14222, "warrior", "duration", 1),
                new Boon("Banner of Strength", 14417, "warrior", "duration", 1),
                new Boon("Banner of Discipline", 14449, "warrior", "duration", 1),
                // el
                //new Boon("Soothing Mist", 5587, "ele", "duration", 1),
                // engie
                new Boon("Pinpoint Distribution", 38333, "engie", "duration", 1),
                // necro
                //new Boon("Vampiric Aura", 30285, "necro", "duration", 1),
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
                new Boon("Spotter", 14055, "ranger", "duration", 1),
                // The generation and the uptime does not use the same id for spirits
                new Boon("Water Spirit", 50386, "ranger", "duration", 1),
                new Boon("Frost Spirit", 50421, "ranger", "duration", 1),
                new Boon("Sun Spirit", 50413, "ranger", "duration", 1),
                new Boon("Stone Spirit", 50415, "ranger", "duration", 1),
                new Boon("Storm Spirit", 12549, "ranger", "duration", 1),
                new Boon("Glyph of Empowerment", 31803, "ranger", "duration", 1),
                new Boon("Empower Allies", 14222, "warrior", "duration", 1),
                new Boon("Banner of Strength", 14417, "warrior", "duration", 1),
                new Boon("Banner of Discipline", 14449, "warrior", "duration", 1),
                new Boon("Battle Presence", 17046, "guard", "duration", 1),//Virtue of Resolve (ID:17046)
                new Boon("Assassin's Presence", 26854, "rev", "duration", 1),
                new Boon("Naturalistic Resonance", 29379, "rev", "duration", 1),
                new Boon("Pinpoint Distribution", 38333, "engie", "duration", 1),
                new Boon("Increased Condition Damage", "engie", "duration", 1), // What is this?
                new Boon("Soothing Mist", 5587, "ele", "duration", 1),
                new Boon("Vampiric Aura", 30285, "necro", "duration", 1)
            };
            return boonList;
        }
        public static List<Boon> getAllProfList()
        {
            List<Boon> boonList = new List<Boon>
            {
                new Boon("Stealth", 13017, "regular", "duration", 1),//13017
                new Boon("Revealed", 890, "regular", "duration", 1),//890
                new Boon("Superspeed", 5974, "regular", "duration", 1),//5974
                new Boon("Invulnerability", 801, "regular", "duration", 1),
                new Boon("Unblockable", "regular", "duration", 1),
                //Auras
                new Boon("Chaos Armor", 10332, "aura", "duration", 1),
                new Boon("Fire Shield", 5677, "aura", "duration", 1),//5677
                new Boon("Frost Aura", 5579, "aura", "duration", 1),//5579
                new Boon("Light Aura", 25518, "aura", "duration", 1),
                new Boon("Magnetic Aura", 5684, "aura", "duration", 1),//5684
                new Boon("Shocking Aura", 5577, "aura", "duration", 1),//5577

                //Race sepecfic
                new Boon("Take Root", 12459, "race", "duration", 1),
                new Boon("Become the Bear", "race", "duration", 1),
                new Boon("Become the Raven", "race", "duration", 1),
                new Boon("Become the Snow Leopard", "race", "duration", 1),
                new Boon("Become the Wolf", "race", "duration", 1),
                new Boon("Avatar of Melandru", 12368, "race", "duration", 1),//12368
                new Boon("Power Suit", "race", "duration", 1),
                new Boon("Reaper of Grenth", 12366, "race", "duration", 1),//12366
                new Boon("Charrooka", "race", "duration", 1),

                //Profession specefic effects
                //revenant
                //skills
                new Boon("Crystal Hibernation", 28262, "rev", "duration", 1),
                new Boon("Vengeful Hammers", 27273, "rev", "duration", 1),
                new Boon("Rite of the Great Dwarf", 26596, "rev", "duration", 1),
                new Boon("Embrace the Darkness", 28001, "rev", "duration", 1),
                new Boon("Enchanted Daggers", 28557, "rev", "intensity", 6),
                new Boon("Impossible Odds", 27581, "rev", "duration", 1),
                //signets
                new Boon("Facet of Light", "rev", "duration", 1),
                new Boon("Infuse Light", "rev", "duration", 1),
                new Boon("Facet of Darkness", "rev", "duration", 1),
                new Boon("Facet of Elements", "rev", "duration", 1),
                new Boon("Facet of Strength", "rev", "duration", 1),
                new Boon("Facet of Chaos", "rev", "duration", 1),
                new Boon("Facet of Nature", "rev", "duration", 1),
                new Boon("Naturalistic Resonance", 29379, "rev", "duration", 1),
                //attunments
                new Boon("Legendary Centaur Stance", "rev", "duration", 1),
                new Boon("Legendary Dragon Stance", "rev", "duration", 1),
                new Boon("Legendary Dwarf Stance", "rev", "duration", 1),
                new Boon("Legendary Demon Stance", "rev", "duration", 1),
                new Boon("Legendary Assassin Stance", "rev", "duration", 1),
                new Boon("Legendary Renegade Stance", "rev", "duration", 1),
                //summons
                new Boon("Breakrazor's Bastion", "rev", "duration", 1),
                new Boon("Razorclaw's Rage", "rev", "duration", 1),
                new Boon("Soulcleave's Summit", "rev", "duration", 1),
                //traits
                new Boon("Vicious Lacerations", "rev", "intensity", 5),
                new Boon("Assassin's Presence", 26854, "rev", "duration", 1),
                new Boon("Expose Defenses", 48894, "rev", "duration", 1),
                new Boon("Invoking Harmony", "rev", "duration", 1),
                new Boon("Selfless Amplification", "rev", "duration", 1),
                new Boon("Hardening Persistence", "rev", "intensity", 8),
                new Boon("Soothing Bastion", "rev", "duration", 1),
                new Boon("Kalla's Fervor", "rev", "duration", 5),

                //warrior
                //skills
                new Boon("Riposte", "warrior", "duration", 1),
                new Boon("Counterattack", "warrior", "duration", 1),
                new Boon("Flames of War", 31708, "warrior", "duration", 1),
                new Boon("Blood Reckoning", 29466 , "warrior", "duration", 1),
                new Boon("Rock Guard", 34256 , "warrior", "duration", 1),
                new Boon("Sight beyond Sight", "warrior", "duration", 1),
                //signets
                new Boon("Healing Signet", "warrior", "duration", 1),
                new Boon("Dolyak Signet", "warrior", "duration", 1),
                new Boon("Signet of Fury", "warrior", "duration", 1),
                new Boon("Signet of Might", "warrior", "duration", 1),
                new Boon("Signet of Stamina", "warrior", "duration", 1),
                new Boon("Signet of Rage", "warrior", "duration", 1),
                new Boon("Signet of Ferocity", "warrior", "duration", 1),
                //summons
                new Boon("Banner of Strength", 14417, "warrior", "duration", 1),
                new Boon("Banner of Discipline", 14449, "warrior", "duration", 1),
                new Boon("Banner of Tactics", "warrior", "duration", 1),
                new Boon("Banner of Defense", "warrior", "duration", 1),
                //stancces
                new Boon("Shield Stance", "warrior", "duration", 1),
                new Boon("Berserker's Stance", "warrior", "duration", 1),
                new Boon("Enduring Pain", "warrior", "duration", 1),
                new Boon("Balanced Stance", "warrior", "duration", 1),
                new Boon("Defiant Stance", "warrior", "duration", 1),
                //traits
                new Boon("Empower Allies", 14222, "warrior", "duration", 1),
                new Boon("Peak Performance", "warrior", "duration", 1),
                new Boon("Furious Surge", 30204, "warrior", "intensity", 25),
                new Boon("Health Gain per Adrenaline bar Spent", "warrior", "intensity", 3),
                new Boon("Rousing Resilience", "warrior", "duration", 1),
                new Boon("Always Angry", "warrior", "duration", 1),
                new Boon("Full Counter", "warrior", "duration", 1),
                new Boon("Attacker's Insight", "warrior", "intensity", 5),
                new Boon("Virtue of Resolve", "warrior", "duration", 1),//Virtue of Resolve (ID:17046)

                //guardian
                //skills
                new Boon("Zealot's Flame", 9103, "guard", "duration", 1),
                new Boon("Purging Flames", "guard", "duration", 1),
                new Boon("Litany of Wrath", "guard", "duration", 1),
                new Boon("Renewed Focus", "guard", "duration", 1),
                //signets
                new Boon("Signet of Resolve", "guard", "duration", 1),
                new Boon("Bane Signet", "guard", "duration", 1),
                new Boon("Signet of Judgment", "guard", "duration", 1),
                new Boon("Signet of Mercy", "guard", "duration", 1),
                new Boon("Signet of Wrath", "guard", "duration", 1),
                new Boon("Signet of Courage", "guard", "duration", 1),
                //traits
                new Boon("Virtue of Justice", 9114, "guard", "duration", 1),
                new Boon("Spears of Justice", 29632, "guard", "duration", 1),
                new Boon("Virtue of Courage", 9113, "guard", "duration", 1),
                new Boon("Shield of Courage", 29523, "guard", "duration", 1),
                new Boon("Virtue of Resolve", 9119, "guard", "duration", 1),
                new Boon("Wings of Resolve", 30308, "guard", "duration", 1),
                new Boon("Strength in Numbers", "guard", "duration", 1),
                new Boon("Invigorated Bulwark", "guard", "intensity", 5),
                new Boon("Battle Presence", 17046, "guard", "duration", 1),//Virtue of Resolve (ID:17046)
                new Boon("Force of Will", "guard", "duration", 1),//not sure if intensity
                new Boon("Tome of Justice", "guard", "duration", 1),
                new Boon("Tome of Courage", "guard", "duration", 1),
                new Boon("Tome of Resolve", "guard", "duration", 1),
                new Boon("Ashes of the Just", "guard", "intensity", 25),
                new Boon("Eternal Oasis", "guard", "duration", 1),
                new Boon("Unbroken Lines", "guard", "duration", 1),
                new Boon("Quickfire", "guard", "duration", 1),

                //Engie
                //skills
                new Boon("Static Shield", "engie", "duration", 1),
                new Boon("Absorb", "engie", "duration", 1),
                new Boon("A.E.D.", "engie", "duration", 1),
                new Boon("Elixir S", "engie", "duration", 1),
                new Boon("Elixir X", "engie", "duration", 1),
                new Boon("Utility Goggles", "engie", "duration", 1),
                new Boon("Slick Shoes", "engie", "duration", 1),
                new Boon("Watchful Eye", "engie", "duration", 1),
                new Boon("Cooling Vapor", "engie", "duration", 1),
                new Boon("Photon Wall Deployed", "engie", "duration", 1),
                new Boon("Spectrum Shield", "engie", "duration", 1),
                new Boon("Gear Shield", "engie", "duration", 1),
                //Transforms
                new Boon("Rampage", "engie", "duration", 1),
                new Boon("Photon Forge", "engie", "duration", 1),
                //Traits
                new Boon("Laser's Edge", "engie", "duration", 1),
                new Boon("Afterburner", "engie", "intensity", 5),
                new Boon("Iron Blooded", "engie", "intensity", 25),
                new Boon("Streamlined Kits", "engie", "duration", 1),
                new Boon("Kinetic Charge", "engie", "intensity", 5),
                new Boon("Pinpoint Distribution", 38333, "engie", "duration", 1),
                new Boon("Increased Condition Damage", "engie", "duration", 1),

                //Ranger
                new Boon("Celestial Avatar", 31508, "ranger", "duration", 1),
                //signets
                new Boon("Signet of Renewal", "ranger", "duration", 1),
                new Boon("Signet of Stone", "ranger", "duration", 1),
                new Boon("Signet of the Hunt", "ranger", "duration", 1),
                new Boon("Signet of the Wild", "ranger", "duration", 1),
                //Summons
                //Need reloook
                // The generation and the uptime does not use the same id for spirits
                // It works on pre 8 may logs, something must have changed with arc dps?
                new Boon("Water Spirit", 50386, "ranger", "duration", 1),
                new Boon("Frost Spirit", 50421, "ranger", "duration", 1),
                new Boon("Sun Spirit", 50413, "ranger", "duration", 1),
                new Boon("Stone Spirit", 50415, "ranger", "duration", 1),
                new Boon("Storm Spirit", 12549, "ranger", "duration", 1),
                //skills
                new Boon("Attack of Opportunity", "ranger", "duration", 1),
                new Boon("Call of the Wild", "ranger", "duration", 1),
                new Boon("Strength of the pack!", "ranger", "duration", 1),
                new Boon("Sick 'Em!", "ranger", "duration", 1),
                new Boon("Sharpening Stones", "ranger", "intenstiy", 10),
                new Boon("Ancestral Grace", 31584, "ranger", "duration", 1),
                new Boon("Glyph of Empowerment", 31803, "ranger", "duration", 1),
                new Boon("Dolyak Stance", "ranger", "duration", 1),
                new Boon("Griffon Stance", "ranger", "duration", 1),
                new Boon("Moa Stance", "ranger", "duration", 1),
                new Boon("Vulture Stance", "ranger", "duration", 1),
                new Boon("Bear Stance", "ranger", "duration", 1),
                new Boon("One Wolf Pack", "ranger", "duration", 1),
                new Boon("Sharpen Spines", "ranger", "intensity", 5),
                //traits
                new Boon("Spotter", 14055, "ranger", "duration", 1),
                new Boon("Opening Strike", "ranger", "duration", 1),
                new Boon("Quick Draw", "ranger", "duration", 1),
                new Boon("On Dodge", "ranger", "duration", 1),
                new Boon("Natural Healing", 30449, "ranger", "duration", 1),
                new Boon("Natural Mender", "ranger", "duration", 1),
                new Boon("Lingering Light", "ranger", "duration", 1),
                new Boon("Deadly", "ranger", "duration", 1),
                new Boon("Ferocious", "ranger", "duration", 1),
                new Boon("Supportive", "ranger", "duration", 1),
                new Boon("Versatile", "ranger", "duration", 1),
                new Boon("Stout", "ranger", "duration", 1),
                new Boon("Unstoppable Union", "ranger", "duration", 1),
                new Boon("Twice as Vicious", "ranger", "duration", 1),

                //thief
                new Boon("Deadeyes's Gaze", "thief", "duration", 1),
                //signets
                new Boon("Signet of Malice", "thief", "duration", 1),
                new Boon("Assassin's Signet", "thief", "duration", 1),//13047 44597
                new Boon("Infiltrator's Signet", "thief", "duration", 1),//13063
                new Boon("Signet of Agility", "thief", "duration", 1),//13061
                new Boon("Signet of Shadows", "thief", "duration", 1),//13059
                //venoms
                new Boon("Ice Drake Venom", "thief", "intensity", 4),//13095
                new Boon("Devourer Venom", 13094, "thief", "intensity", 2),//
                new Boon("Skale Venom", 13036, "thief", "intensity", 4),//13054
                new Boon("Spider Venom", "thief", "intensity", 6),//
                new Boon("Basilisk Venom", 13133, "thief", "intensity", 6),//
                //Physical
                new Boon("Palm Strike", "thief", "duration", 1),//
                new Boon("Pulmonary Impact", "thief", "intensity", 2),//
                new Boon("Infiltration", "thief", "duration", 1),//
                //Transforms
                new Boon("Dagger Storm", "thief", "duration", 1),//
                new Boon("Kneeling", "thief", "duration", 1),//
                //traits
                new Boon("Maleficent Seven", "thief", "duration", 1),
                new Boon("Hidden Killer", "thief", "duration", 1),//
                new Boon("Lead Attacks", "thief", "intensity", 15),//
                new Boon("Instant Reflexes", "thief", "duration", 1),//
                new Boon("Lotus Training", 32200, "thief", "duration", 1),//
                new Boon("Unhindered Combatant", 32931, "thief", "duration", 1),//
                new Boon("Bounding Dodger", 33162, "thief", "duration", 1),//

                //mesmer 
                new Boon("Distortion", "mes", "duration", 1),
                //signets
                new Boon("Signet of the Ether", 21751, "mes", "duration", 1),
                new Boon("Signet of Domination", "mes", "duration", 1),
                new Boon("Signet of Illusions", "mes", "duration", 1),
                new Boon("Signet of Inspiration", "mes", "duration", 1),
                new Boon("Signet of Midnight", "mes", "duration", 1),
                new Boon("Signet of Humility", "mes", "duration", 1),
                //skills
                new Boon("Blur", 10335 , "mes", "duration", 1),
                new Boon("Mirror", "mes", "duration", 1),
                new Boon("Echo", "mes", "duration", 1),
                new Boon("Illusion of Life", "mes", "duration", 1),
                new Boon("Time Echo", "mes", "duration", 1),
                //traits
                new Boon("Fencer's Finesse", 30426 , "mes", "intensity", 10),
                new Boon("Illusionary Defense", "mes", "intensity", 5),
                new Boon("Compunding Power", "mes", "intensity", 5),
                new Boon("Phantasmal Force", 44691 , "mes", "intensity", 25),
                new Boon("Mirage Cloak", "mes", "duration", 1),

                //Necro
                //forms
                new Boon("Lich Form", "necro", "duration", 1),
                new Boon("Death Shroud", 790, "necro", "duration", 1),
                new Boon("Reaper's Shroud", 29446, "necro", "duration", 1),
                //Signets
                new Boon("Signet of Vampirism", "necro", "duration", 1),
                new Boon("Plague Signet", "necro", "duration", 1),
                new Boon("Signet of Spite", "necro", "duration", 1),
                new Boon("Signet of the Locust", "necro", "duration", 1),
                new Boon("Signet of Undeath", "necro", "duration", 1),
                //skills
                new Boon("Spectral Walk", "necro", "duration", 1),
                new Boon("Infusing Terror", 30129, "necro", "duration", 1),
                //traits
                new Boon("Corrupter's Defense", "necro", "intenstiy", 10),
                new Boon("Vampiric Aura", 30285, "necro", "duration", 1),
                new Boon("Last Rites", "necro", "duration", 1),
                new Boon("Sadistic Searing", "necro", "duration", 1),

                //ele
                //signets
                new Boon("Signet of Restoration", "ele", "duration", 1),//739
                new Boon("Signet of Air", "ele", "duration", 1),//5590
                new Boon("Signet of Earth", "ele", "duration", 1),//5592
                new Boon("Signet of Fire", "ele", "duration", 1),//5544
                new Boon("Signet of Water", "ele", "duration", 1),//5591
                                                                  //attunments
                new Boon("Fire Attunement", 5585, "ele", "duration", 1),//5585
                new Boon("Water Attunement", 5586, "ele", "duration", 1),
                new Boon("Air Attunement", 5575, "ele", "duration", 1),//5575
                new Boon("Earth Attunement", 5580, "ele", "duration", 1),//5580
                                                                   //forms
                new Boon("Mist Form", "ele", "duration", 1),//5543
                new Boon("Ride the Lightning", "ele", "duration", 1),//5588
                new Boon("Vapor Form", "ele", "duration", 1),
                new Boon("Tornado", "ele", "duration", 1),//5534
                new Boon("Whirlpool", "ele", "duration", 1),
                //conjures
                new Boon("Conjure Earth Shield", 15788, "ele", "duration", 1),//15788
                new Boon("Conjure Flame Axe", 15789, "ele", "duration", 1),//15789
                new Boon("Conjure Frost Bow", 15790, "ele", "duration", 1),//15790
                new Boon("Conjure Lightning Hammer", 15791, "ele", "duration", 1),//15791
                new Boon("Conjure Fiery Greatsword", 15792, "ele", "duration", 1),//15792
                //Extras
                new Boon("Arcane Power", "ele", "duration", 1),//5582
                new Boon("Arcane Shield", "ele", "duration", 1),//5640
                new Boon("Renewal of Fire", "ele", "duration", 1),//5764
                new Boon("Glyph of Elemental Power", "ele", "duration", 1),//5739 5741 5740 5742
                new Boon("Rebound", "ele", "duration", 1),//31337
                new Boon("Rock Barrier", "ele", "duration", 1),//34633 750
                new Boon("Magnetic Wave", "ele", "duration", 1),//15794
                new Boon("Obsidian Flesh", "ele", "duration", 1),//5667
                //Traits
                new Boon("Harmonious Conduit", "ele", "duration", 1),//31353
                new Boon("Fresh Air", "ele", "duration", 1),//31353
                new Boon("Soothing Mist", 5587, "ele", "duration", 1),
                new Boon("Lesser Arcane Shield", "ele", "duration", 1),
                new Boon("Weaver's Prowess", "ele", "duration", 1),
                new Boon("Elements of Rage", "ele", "duration", 1),
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