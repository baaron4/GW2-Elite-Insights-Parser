using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class Boon
    {
        // Boon
        public enum BoonEnum { Condition, Boon, OffensiveBuff, DefensiveBuff, NonShareableBuff}
        public enum BoonSource { Mixed, Necromancer, Elementalist, Mesmer, Warrior, Revenant, Guardian, Thief, Ranger, Engineer  }

        // Fields
        private String name;
        private int id;
        private BoonEnum priority;
        private BoonSource plotlyGroup;
        private String type;
        private int capacity;


        private Boon(String name, BoonSource group, String type, int capacity, BoonEnum priority)
        {
            this.name = name;
            this.plotlyGroup = group;
            this.type = type;
            this.id = -1;
            this.capacity = capacity;
            this.priority = priority;
        }
        private Boon(String name, int id, BoonSource group, String type, int capacity, BoonEnum priority)
        {
            this.name = name;
            this.id = id;
            this.plotlyGroup = group;
            this.type = type;
            this.capacity = capacity;
            this.priority = priority;
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

        private static List<Boon> allBoons = new List<Boon>
            {
                //Base boons
                new Boon("Might", 740, BoonSource.Mixed, "intensity", 25, BoonEnum.Boon),
                new Boon("Fury", 725, BoonSource.Mixed, "duration", 9, BoonEnum.Boon),//or 3m and 30s
                new Boon("Quickness", 1187, BoonSource.Mixed, "duration", 5, BoonEnum.Boon),
                new Boon("Alacrity", 30328, BoonSource.Mixed, "duration", 9, BoonEnum.Boon),
                new Boon("Protection", 717, BoonSource.Mixed, "duration", 5, BoonEnum.Boon),
                new Boon("Regeneration", 718, BoonSource.Mixed, "duration", 5, BoonEnum.Boon),
                new Boon("Vigor", 726, BoonSource.Mixed, "duration", 5, BoonEnum.Boon),
                new Boon("Aegis", 743, BoonSource.Mixed, "duration", 9, BoonEnum.Boon),
                new Boon("Stability", 1122, BoonSource.Mixed, "intensity", 25, BoonEnum.Boon),
                new Boon("Swiftness", 719, BoonSource.Mixed, "duration", 9, BoonEnum.Boon),
                new Boon("Retaliation", 873, BoonSource.Mixed, "duration", 9, BoonEnum.Boon),
                new Boon("Resistance", 26980, BoonSource.Mixed, "duration", 5, BoonEnum.Boon),
                // Condis         
                new Boon("Bleeding", 736, BoonSource.Mixed, "intensity", 1500, BoonEnum.Condition),
                new Boon("Burning", 737, BoonSource.Mixed, "intensity", 1500, BoonEnum.Condition),
                new Boon("Confusion", 861, BoonSource.Mixed, "intensity", 1500, BoonEnum.Condition),
                new Boon("Poison", 723, BoonSource.Mixed, "intensity", 1500, BoonEnum.Condition),
                new Boon("Torment", 19426, BoonSource.Mixed, "intensity", 1500, BoonEnum.Condition),
                new Boon("Blinded", 737, BoonSource.Mixed, "duration", 9, BoonEnum.Condition),
                new Boon("Chilled", 722, BoonSource.Mixed, "duration", 5, BoonEnum.Condition),
                new Boon("Crippled", 721, BoonSource.Mixed, "duration", 9, BoonEnum.Condition),
                new Boon("Fear", 791, BoonSource.Mixed, "duration", 9, BoonEnum.Condition),
                new Boon("Immobile", 727, BoonSource.Mixed, "duration", 3, BoonEnum.Condition),
                new Boon("Slow", 26766, BoonSource.Mixed, "duration", 9, BoonEnum.Condition),
                new Boon("Weakness", 737, BoonSource.Mixed, "duration", 5, BoonEnum.Condition),
                new Boon("Vulnerability", 738, BoonSource.Mixed, "intensity", 25, BoonEnum.Condition),
                new Boon("Retaliation Condi", 873, BoonSource.Mixed, "duration", 9, BoonEnum.Condition),
                // Generic
                new Boon("Stealth", 13017, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Revealed", 890, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Superspeed", 5974, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Invulnerability", 801, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Unblockable", BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                //Auras
                new Boon("Chaos Armor", 10332, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Fire Shield", 5677, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Frost Aura", 5579, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Light Aura", 25518, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Magnetic Aura", 5684, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Shocking Aura", 5577, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                //race
                new Boon("Take Root", 12459, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Become the Bear",12426, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Become the Raven",12405, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Become the Snow Leopard",12400, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Become the Wolf",12393, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Avatar of Melandru", 12368, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Power Suit",12326, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Reaper of Grenth", 12366, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Charrooka",43503, BoonSource.Mixed, "duration", 1, BoonEnum.NonShareableBuff),
                ///REVENANT
                //skills
                new Boon("Crystal Hibernation", 28262, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Vengeful Hammers", 27273, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Rite of the Great Dwarf", 26596, BoonSource.Revenant, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Embrace the Darkness", 28001, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Enchanted Daggers", 28557, BoonSource.Revenant, "intensity", 6, BoonEnum.NonShareableBuff),
                new Boon("Impossible Odds", 27581, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                //facets
                new Boon("Facet of Light",27336, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Infuse Light",27737, BoonSource.Revenant, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Facet of Darkness",28036, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Facet of Elements",28243, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Facet of Strength",27376, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Facet of Chaos",27983, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Facet of Nature",29275, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Naturalistic Resonance", 29379, BoonSource.Revenant, "duration", 1, BoonEnum.DefensiveBuff),
                //legends
                new Boon("Legendary Centaur Stance",27972, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Legendary Dragon Stance",27732, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Legendary Dwarf Stance",27205, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Legendary Demon Stance",27928, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Legendary Assassin Stance",27890, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Legendary Renegade Stance",44272, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                //summons
                new Boon("Breakrazor's Bastion",44682, BoonSource.Revenant, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Razorclaw's Rage",41016, BoonSource.Revenant, "duration", 1, BoonEnum.OffensiveBuff),
                new Boon("Soulcleave's Summit",45026, BoonSource.Revenant, "duration", 1, BoonEnum.OffensiveBuff),
                //traits
                new Boon("Vicious Lacerations",29395, BoonSource.Revenant, "intensity", 5, BoonEnum.NonShareableBuff),
                new Boon("Assassin's Presence", 26854, BoonSource.Revenant, "duration", 1, BoonEnum.OffensiveBuff),
                new Boon("Expose Defenses", 48894, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Invoking Harmony",29025, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Selfless Amplification",30509, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Hardening Persistence",28957, BoonSource.Revenant, "intensity", 8, BoonEnum.NonShareableBuff),
                new Boon("Soothing Bastion",34136, BoonSource.Revenant, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Kalla's Fervor",42883, BoonSource.Revenant, "duration", 5, BoonEnum.NonShareableBuff),
                new Boon("Improved Kalla's Fervor",45614, BoonSource.Revenant, "duration", 5, BoonEnum.NonShareableBuff),
                ///WARRIOR
                //skills
                new Boon("Riposte",14434, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Counterattack",14509, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Flames of War", 31708, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Blood Reckoning", 29466 , BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Rock Guard", 34256 , BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Sight beyond Sight",40616, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                //signets
                new Boon("Healing Signet",786, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Dolyak Signet",14458, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Fury",14459, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Might",14444, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Stamina",14478, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Rage",14496, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                //banners
                new Boon("Banner of Strength", 14417, BoonSource.Warrior, "duration", 1, BoonEnum.OffensiveBuff),
                new Boon("Banner of Discipline", 14449, BoonSource.Warrior, "duration", 1, BoonEnum.OffensiveBuff),
                new Boon("Banner of Tactics",14450, BoonSource.Warrior, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Banner of Defense",14543, BoonSource.Warrior, "duration", 1, BoonEnum.DefensiveBuff),
                //stances
                new Boon("Shield Stance",756, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Berserker's Stance",14453, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Enduring Pain",787, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Balanced Stance",34778, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Defiant Stance",21816, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Rampage",14484, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                //traits
                new Boon("Empower Allies", 14222, BoonSource.Warrior, "duration", 1, BoonEnum.OffensiveBuff),
                new Boon("Peak Performance",46853, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Furious Surge", 30204, BoonSource.Warrior, "intensity", 25, BoonEnum.NonShareableBuff),
                new Boon("Health Gain per Adrenaline bar Spent", BoonSource.Warrior, "intensity", 3, BoonEnum.NonShareableBuff),
                new Boon("Rousing Resilience",24383, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Always Angry",34099, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Full Counter",43949, BoonSource.Warrior, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Attacker's Insight",41963, BoonSource.Warrior, "intensity", 5, BoonEnum.NonShareableBuff),
                /// GUARDIAN
                //skills
                new Boon("Zealot's Flame", 9103, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Purging Flames",21672, BoonSource.Guardian, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Litany of Wrath",21665, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Renewed Focus",9255, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Ashes of the Just",41957, BoonSource.Guardian, "intensity", 25, BoonEnum.OffensiveBuff),
                new Boon("Eternal Oasis",44871, BoonSource.Guardian, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Unbroken Lines",43194, BoonSource.Guardian, "duration", 1, BoonEnum.DefensiveBuff),
                //signets
                new Boon("Signet of Resolve",9220, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Bane Signet",9092, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Judgment",9156, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Mercy",9162, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Wrath",9100, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Courage",29633, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                //virtues
                new Boon("Virtue of Justice", 9114, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Spears of Justice", 29632, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Virtue of Courage", 9113, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Shield of Courage", 29523, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Virtue of Resolve", 9119, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Wings of Resolve", 30308, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Tome of Justice",40530, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Tome of Courage",43508,BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Tome of Resolve",46298, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                //traits
                new Boon("Strength in Numbers",13796, BoonSource.Guardian, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Invigorated Bulwark",30207, BoonSource.Guardian, "intensity", 5, BoonEnum.NonShareableBuff),
                new Boon("Battle Presence", 17046, BoonSource.Guardian, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Force of Will",29485, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),//not sure if intensity
                new Boon("Quickfire",45123, BoonSource.Guardian, "duration", 1, BoonEnum.NonShareableBuff),
                ///ENGINEER
                //skills
                new Boon("Static Shield",6055, BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Absorb",6056, BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("A.E.D.",21660, BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Elixir S",5863, BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Elixir X", BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Utility Goggles",5864, BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Slick Shoes",5833, BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Watchful Eye", BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Cooling Vapor",46444, BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Photon Wall Deployed",46094, BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Spectrum Shield",43066, BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Gear Shield",5997, BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                //Transforms
                new Boon("Rampage", BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Photon Forge",43708, BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                //Traits
                new Boon("Laser's Edge",44414, BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Afterburner",42210, BoonSource.Engineer, "intensity", 5, BoonEnum.NonShareableBuff),
                new Boon("Iron Blooded",49065, BoonSource.Engineer, "intensity", 25, BoonEnum.NonShareableBuff),
                new Boon("Streamlined Kits",18687, BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Kinetic Charge",45781, BoonSource.Engineer, "intensity", 5, BoonEnum.NonShareableBuff),
                new Boon("Pinpoint Distribution", 38333, BoonSource.Engineer, "duration", 1, BoonEnum.OffensiveBuff),
                new Boon("Heat Therapy",40694, BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Overheat", 40397, BoonSource.Engineer, "duration", 1, BoonEnum.NonShareableBuff),
                ///RANGER
                new Boon("Celestial Avatar", 31508, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                //signets
                new Boon("Signet of Renewal",41147, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Stone",12627, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of the Hunt",12626, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of the Wild",12636, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                //spirits
                new Boon("Water Spirit", 50386, BoonSource.Ranger, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Frost Spirit", 50421, BoonSource.Ranger, "duration", 1, BoonEnum.OffensiveBuff),
                new Boon("Sun Spirit", 50413, BoonSource.Ranger, "duration", 1, BoonEnum.OffensiveBuff),
                new Boon("Stone Spirit", 50415, BoonSource.Ranger, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Storm Spirit", 50381, BoonSource.Ranger, "duration", 1, BoonEnum.OffensiveBuff),
                //skills
                new Boon("Attack of Opportunity",12574, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Call of the Wild",36781, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Strength of the pack!",12554, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Sick 'Em!",33902, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Sharpening Stones",12536, BoonSource.Ranger, "intenstiy", 10, BoonEnum.NonShareableBuff),
                new Boon("Ancestral Grace", 31584, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Glyph of Empowerment", 31803, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Dolyak Stance",41815, BoonSource.Ranger, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Griffon Stance",46280, BoonSource.Ranger, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Moa Stance",45038, BoonSource.Ranger, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Vulture Stance",44651, BoonSource.Ranger, "duration", 1, BoonEnum.OffensiveBuff),
                new Boon("Bear Stance",40045, BoonSource.Ranger, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("One Wolf Pack",44139, BoonSource.Ranger, "duration", 1, BoonEnum.OffensiveBuff),
                new Boon("Sharpen Spines",43266, BoonSource.Ranger, "intensity", 5, BoonEnum.NonShareableBuff),
                //traits
                new Boon("Spotter", 14055, BoonSource.Ranger, "duration", 1, BoonEnum.OffensiveBuff),
                new Boon("Opening Strike",13988, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Quick Draw",29703, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Light on your feet",30673, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Natural Mender",30449, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Lingering Light",32248, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Deadly",44932, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Ferocious",41720, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Supportive",40069, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Versatile",44693, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Stout",40272, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Unstoppable Union",44439, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Twice as Vicious",45600, BoonSource.Ranger, "duration", 1, BoonEnum.NonShareableBuff),
                ///THIEF
                //signets
                new Boon("Signet of Malice",13049, BoonSource.Thief, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Assassin's Signet (Passive)",13047, BoonSource.Thief, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Assassin's Signet (Active)",44597, BoonSource.Thief, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Infiltrator's Signet",13063, BoonSource.Thief, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Agility",13061, BoonSource.Thief, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Shadows",13059, BoonSource.Thief, "duration", 1, BoonEnum.NonShareableBuff),
                //venoms
                new Boon("Skelk Venom", BoonSource.Thief, "intensity", 4, BoonEnum.DefensiveBuff),
                new Boon("Ice Drake Venom",13095, BoonSource.Thief, "intensity", 4, BoonEnum.DefensiveBuff),
                new Boon("Devourer Venom", 13094, BoonSource.Thief, "intensity", 2, BoonEnum.DefensiveBuff),
                new Boon("Skale Venom", 13036, BoonSource.Thief, "intensity", 4, BoonEnum.OffensiveBuff),
                new Boon("Spider Venom",13036, BoonSource.Thief, "intensity", 6, BoonEnum.OffensiveBuff),
                new Boon("Basilisk Venom", 13133, BoonSource.Thief, "intensity", 6, BoonEnum.OffensiveBuff),
                //physical
                new Boon("Palm Strike",30423, BoonSource.Thief, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Pulmonary Impact",30510, BoonSource.Thief, "intensity", 2, BoonEnum.NonShareableBuff),
                //weapon
                new Boon("Infiltration",13135, BoonSource.Thief, "duration", 1, BoonEnum.NonShareableBuff),
                //transforms
                new Boon("Dagger Storm",13134, BoonSource.Thief, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Kneeling",42869, BoonSource.Thief, "duration", 1, BoonEnum.NonShareableBuff),
                //traits
                new Boon("Deadeyes's Gaze",46333, BoonSource.Thief, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Maleficent Seven",43606, BoonSource.Thief, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Hidden Killer",42720, BoonSource.Thief, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Lead Attacks",34659, BoonSource.Thief, "intensity", 15, BoonEnum.NonShareableBuff),
                new Boon("Instant Reflexes",34283, BoonSource.Thief, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Lotus Training", 32200, BoonSource.Thief, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Unhindered Combatant", 32931, BoonSource.Thief, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Bounding Dodger", 33162, BoonSource.Thief, "duration", 1, BoonEnum.NonShareableBuff),
                ///MESMER
                //signets
                new Boon("Signet of the Ether", 21751, BoonSource.Mesmer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Domination",10231, BoonSource.Mesmer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Illusions",10246, BoonSource.Mesmer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Inspiration",10235, BoonSource.Mesmer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Midnight",10233, BoonSource.Mesmer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Humility",30739, BoonSource.Mesmer, "duration", 1, BoonEnum.NonShareableBuff),
                //skills
                new Boon("Distortion",10243, BoonSource.Mesmer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Blur", 10335 , BoonSource.Mesmer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Mirror",10357, BoonSource.Mesmer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Echo",29664, BoonSource.Mesmer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Illusion of Life", BoonSource.Mesmer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Time Block",30134, BoonSource.Mesmer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Time Echo",29582, BoonSource.Mesmer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Time Anchored",30136, BoonSource.Mesmer, "duration", 1, BoonEnum.NonShareableBuff),
                //traits
                new Boon("Fencer's Finesse", 30426 , BoonSource.Mesmer, "intensity", 10, BoonEnum.NonShareableBuff),
                new Boon("Illusionary Defense",49099, BoonSource.Mesmer, "intensity", 5, BoonEnum.NonShareableBuff),
                new Boon("Compunding Power",49058, BoonSource.Mesmer, "intensity", 5, BoonEnum.NonShareableBuff),
                new Boon("Phantasmal Force", 44691 , BoonSource.Mesmer, "intensity", 25, BoonEnum.NonShareableBuff),
                new Boon("Mirage Cloak",40408, BoonSource.Mesmer, "duration", 1, BoonEnum.NonShareableBuff),
                ///NECROMANCER
                //forms
                new Boon("Lich Form",10631, BoonSource.Necromancer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Death Shroud", 790, BoonSource.Necromancer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Reaper's Shroud", 29446, BoonSource.Necromancer, "duration", 1, BoonEnum.NonShareableBuff),
                //signets
                new Boon("Signet of Vampirism",21761, BoonSource.Necromancer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Plague Signet",10630, BoonSource.Necromancer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Spite",10621, BoonSource.Necromancer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of the Locust",10614, BoonSource.Necromancer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Undeath",10610, BoonSource.Necromancer, "duration", 1, BoonEnum.NonShareableBuff),
                //skills
                new Boon("Spectral Walk",15083, BoonSource.Necromancer, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Infusing Terror", 30129, BoonSource.Necromancer, "duration", 1, BoonEnum.NonShareableBuff),
                //traits
                new Boon("Corrupter's Defense",30845, BoonSource.Necromancer, "intenstiy", 10, BoonEnum.NonShareableBuff),
                new Boon("Vampiric Aura", 30285, BoonSource.Necromancer, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Last Rites",29726, BoonSource.Necromancer, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Sadistic Searing",43626, BoonSource.Necromancer, "duration", 1, BoonEnum.NonShareableBuff),
                ///ELEMENTALIST
                //signets
                new Boon("Signet of Restoration",739, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Air",5590, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Earth",5592, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Fire",5544, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Signet of Water",5591, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                //attunments
                new Boon("Fire Attunement", 5585, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Water Attunement", 5586, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Air Attunement", 5575, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Earth Attunement", 5580, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                //forms
                new Boon("Mist Form",5543, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Ride the Lightning",5588, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Vapor Form",5620, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Tornado",5534, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Whirlpool", BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                //conjures
                new Boon("Conjure Earth Shield", 15788, BoonSource.Elementalist, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Conjure Flame Axe", 15789, BoonSource.Elementalist, "duration", 1, BoonEnum.OffensiveBuff),
                new Boon("Conjure Frost Bow", 15790, BoonSource.Elementalist, "duration", 1, BoonEnum.OffensiveBuff),
                new Boon("Conjure Lightning Hammer", 15791, BoonSource.Elementalist, "duration", 1, BoonEnum.OffensiveBuff),
                new Boon("Conjure Fiery Greatsword", 15792, BoonSource.Elementalist, "duration", 1, BoonEnum.OffensiveBuff),
                //skills
                new Boon("Arcane Power",5582, BoonSource.Elementalist, "duration", 1, BoonEnum.OffensiveBuff),
                new Boon("Arcane Shield",5640, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Renewal of Fire",5764, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Glyph of Elemental Power (Fire)",5739, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Glyph of Elemental Power (Water)",5741, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Glyph of Elemental Power (Air)",5740, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Glyph of Elemental Power (Earth)",5742, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Rebound",31337, BoonSource.Elementalist, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Rock Barrier",34633, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),//750?
                new Boon("Magnetic Wave",15794, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Obsidian Flesh",5667, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                //traits
                new Boon("Harmonious Conduit",31353, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Fresh Air",31353, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Soothing Mist", 5587, BoonSource.Elementalist, "duration", 1, BoonEnum.DefensiveBuff),
                new Boon("Lesser Arcane Shield",25579, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Weaver's Prowess",42061, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
                new Boon("Elements of Rage",42416, BoonSource.Elementalist, "duration", 1, BoonEnum.NonShareableBuff),
            };

        public static List<Boon> getCondiBoonList()
        {
            return allBoons.Where(x => x.priority == BoonEnum.Condition).ToList();
        }
        public static List<Boon> getList()
        {
            return allBoons.Where(x => x.priority == BoonEnum.Boon || x.priority == BoonEnum.OffensiveBuff || x.priority == BoonEnum.DefensiveBuff).ToList();
        }

        public static List<Boon> getOffensiveList()
        {
            return allBoons.Where(x => x.priority == BoonEnum.OffensiveBuff).ToList();
        }
        public static List<Boon> getDefensiveList()
        {
            return allBoons.Where(x => x.priority == BoonEnum.DefensiveBuff).ToList();
        }

        public static List<Boon> getMainList()
        {
            return allBoons.Where(x => x.priority == BoonEnum.Boon).ToList();
        }
        public static List<Boon> getSharableProfList()
        {
            return allBoons.Where(x => x.priority == BoonEnum.OffensiveBuff || x.priority == BoonEnum.DefensiveBuff).ToList();   
        }
        public static List<Boon> getAllProfList()
        {
            return allBoons.Where(x => x.priority != BoonEnum.Condition).ToList();
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
        public BoonSource getPloltyGroup()
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