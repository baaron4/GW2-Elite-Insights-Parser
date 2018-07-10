using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Boon
    {
        // Boon
        public enum BoonEnum { Condition, Boon, OffensiveBuffTable, DefensiveBuffTable, GraphOnlyBuff, Food, Utility};
        public enum BoonSource { Mixed, Necromancer, Elementalist, Mesmer, Warrior, Revenant, Guardian, Thief, Ranger, Engineer, Item  };
        public enum RemoveType { CleanseFoe, CleanseFriend, Manual, None, All};
        public enum BoonType { Duration, Intensity};

        private static BoonSource ProfToEnum(string prof)
        {
            switch(prof)
            {
                case "Druid":
                case "Ranger":
                case "Soulbeast":
                    return BoonSource.Ranger;
                case "Scrapper":
                case "Holosmith":
                case "Engineer":
                    return BoonSource.Engineer;
                case "Daredevil":
                case "Deadeye":
                case "Thief":
                    return BoonSource.Thief;
                case "Weaver":
                case "Tempest":
                case "Elementalist":
                    return BoonSource.Elementalist;
                case "Mirage":
                case "Chronomancer":
                case "Mesmer":
                    return BoonSource.Mesmer;
                case "Scourge":
                case "Reaper":
                case "Necromancer":
                    return BoonSource.Necromancer;
                case "Spellbreaker":
                case "Berserker":
                case "Warrior":
                    return BoonSource.Warrior;
                case "Firebrand":
                case "Dragonhunter":
                case "Guardian":
                    return BoonSource.Guardian;
                case "Renegade":
                case "Herald":
                case "Revenant":
                    return BoonSource.Revenant;
            }
            return BoonSource.Mixed;
        }

        // Fields
        private string name;
        private long id;
        private BoonEnum nature;
        private BoonSource source;
        private RemoveType remove_type;
        private BoonType type;
        private int capacity;
        private string link;


        private Boon(string name, BoonSource source, BoonType type, int capacity, BoonEnum nature, RemoveType remove_type = RemoveType.None)
        {
            this.name = name;
            this.source = source;
            this.type = type;
            this.id = -1;
            this.capacity = capacity;
            this.nature = nature;
            this.link = "";
            this.remove_type = remove_type;
        }
        private Boon(string name, int id, BoonSource source, BoonType type, int capacity, BoonEnum nature, RemoveType remove_type = RemoveType.None)
        {
            this.name = name;
            this.id = id;
            this.source = source;
            this.type = type;
            this.capacity = capacity;
            this.nature = nature;
            this.link = "";
            this.remove_type = remove_type;
        }

        private Boon(string name, int id, BoonSource source, BoonType type, int capacity, BoonEnum nature, string link, RemoveType remove_type = RemoveType.None)
        {
            this.name = name;
            this.id = id;
            this.source = source;
            this.type = type;
            this.capacity = capacity;
            this.nature = nature;
            this.link = link;
            this.remove_type = remove_type;
        }
        // Public Methods

        private static List<Boon> allBoons = new List<Boon>
            {
                //Base boons
                new Boon("Might", 740, BoonSource.Mixed, BoonType.Intensity, 25, BoonEnum.Boon, "https://wiki.guildwars2.com/images/7/7c/Might.png", RemoveType.CleanseFoe),
                new Boon("Fury", 725, BoonSource.Mixed, BoonType.Duration, 9, BoonEnum.Boon, "https://wiki.guildwars2.com/images/4/46/Fury.png", RemoveType.CleanseFoe),//or 3m and 30s
                new Boon("Quickness", 1187, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Boon, "https://wiki.guildwars2.com/images/b/b4/Quickness.png", RemoveType.CleanseFoe),
                new Boon("Alacrity", 30328, BoonSource.Mixed, BoonType.Duration, 9, BoonEnum.Boon, "https://wiki.guildwars2.com/images/thumb/4/4c/Alacrity.png/20px-Alacrity.png", RemoveType.CleanseFoe),
                new Boon("Protection", 717, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Boon, "https://wiki.guildwars2.com/images/6/6c/Protection.png", RemoveType.CleanseFoe),
                new Boon("Regeneration", 718, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Boon, "https://wiki.guildwars2.com/images/5/53/Regeneration.png", RemoveType.CleanseFoe),
                new Boon("Vigor", 726, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Boon, "https://wiki.guildwars2.com/images/f/f4/Vigor.png", RemoveType.CleanseFoe),
                new Boon("Aegis", 743, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Boon, "https://wiki.guildwars2.com/images/e/e5/Aegis.png", RemoveType.All),
                new Boon("Stability", 1122, BoonSource.Mixed, BoonType.Intensity, 25, BoonEnum.Boon, "https://wiki.guildwars2.com/images/a/ae/Stability.png", RemoveType.All),
                new Boon("Swiftness", 719, BoonSource.Mixed, BoonType.Duration, 9, BoonEnum.Boon, "https://wiki.guildwars2.com/images/a/af/Swiftness.png", RemoveType.CleanseFoe),
                new Boon("Retaliation", 873, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Boon, "https://wiki.guildwars2.com/images/5/53/Retaliation.png", RemoveType.CleanseFoe),
                new Boon("Resistance", 26980, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Boon, "https://wiki.guildwars2.com/images/thumb/e/e9/Resistance_40px.png/20px-Resistance_40px.png", RemoveType.CleanseFoe),
                // Condis         
                new Boon("Bleeding", 736, BoonSource.Mixed, BoonType.Intensity, 1500, BoonEnum.Condition, "https://wiki.guildwars2.com/images/thumb/3/33/Bleeding.png/20px-Bleeding.png", RemoveType.CleanseFriend),
                new Boon("Burning", 737, BoonSource.Mixed, BoonType.Intensity, 1500, BoonEnum.Condition, "https://wiki.guildwars2.com/images/thumb/4/45/Burning.png/20px-Burning.png", RemoveType.CleanseFriend),
                new Boon("Confusion", 861, BoonSource.Mixed, BoonType.Intensity, 1500, BoonEnum.Condition, "https://wiki.guildwars2.com/images/thumb/e/e6/Confusion.png/20px-Confusion.png", RemoveType.CleanseFriend),
                new Boon("Poison", 723, BoonSource.Mixed, BoonType.Intensity, 1500, BoonEnum.Condition, "https://wiki.guildwars2.com/images/thumb/0/05/Poison.png/20px-Poison.png", RemoveType.CleanseFriend),
                new Boon("Torment", 19426, BoonSource.Mixed, BoonType.Intensity, 1500, BoonEnum.Condition, "https://wiki.guildwars2.com/images/thumb/0/08/Torment.png/20px-Torment.png", RemoveType.CleanseFriend),
                new Boon("Blind", 720, BoonSource.Mixed, BoonType.Duration, 9, BoonEnum.Condition, "https://wiki.guildwars2.com/images/thumb/3/33/Blinded.png/20px-Blinded.png", RemoveType.CleanseFriend),
                new Boon("Chilled", 722, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Condition, "https://wiki.guildwars2.com/images/thumb/a/a6/Chilled.png/20px-Chilled.png", RemoveType.CleanseFriend),
                new Boon("Crippled", 721, BoonSource.Mixed, BoonType.Duration, 9, BoonEnum.Condition, "https://wiki.guildwars2.com/images/thumb/f/fb/Crippled.png/20px-Crippled.png", RemoveType.CleanseFriend),
                new Boon("Fear", 791, BoonSource.Mixed, BoonType.Duration, 9, BoonEnum.Condition, "https://wiki.guildwars2.com/images/thumb/e/e6/Fear.png/20px-Fear.png", RemoveType.CleanseFriend),
                new Boon("Immobile", 727, BoonSource.Mixed, BoonType.Duration, 3, BoonEnum.Condition, "https://wiki.guildwars2.com/images/thumb/3/32/Immobile.png/20px-Immobile.png", RemoveType.CleanseFriend),
                new Boon("Slow", 26766, BoonSource.Mixed, BoonType.Duration, 9, BoonEnum.Condition, "https://wiki.guildwars2.com/images/thumb/f/fb/Slow_40px.png/20px-Slow_40px.png", RemoveType.CleanseFriend),
                new Boon("Weakness", 742, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Condition, "https://wiki.guildwars2.com/images/thumb/f/f9/Weakness.png/20px-Weakness.png", RemoveType.CleanseFriend),
                new Boon("Taunt", 27705, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Condition, "https://wiki.guildwars2.com/images/thumb/c/cc/Taunt.png/20px-Taunt.png", RemoveType.CleanseFriend),
                new Boon("Vulnerability", 738, BoonSource.Mixed, BoonType.Intensity, 25, BoonEnum.Condition, "https://wiki.guildwars2.com/images/thumb/a/af/Vulnerability.png/20px-Vulnerability.png", RemoveType.CleanseFriend),
                new Boon("Retaliation", 873, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Condition, "https://wiki.guildwars2.com/images/5/53/Retaliation.png", RemoveType.CleanseFoe),
                // Generic
                new Boon("Stealth", 13017, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Revealed", 890, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Superspeed", 5974, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Super_Speed.png"),
                //new Boon("Invulnerability", 801, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Boon("Unblockable", BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                //Auras
                new Boon("Chaos Armor", 10332, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Fire Shield", 5677, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Frost Aura", 5579, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Light Aura", 25518, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Magnetic Aura", 5684, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Shocking Aura", 5577, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                //race
                new Boon("Take Root", 12459, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Become the Bear",12426, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Become the Raven",12405, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Become the Snow Leopard",12400, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Become the Wolf",12393, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Avatar of Melandru", 12368, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Power Suit",12326, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Reaper of Grenth", 12366, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Charrzooka",43503, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                ///REVENANT
                //skills
                new Boon("Crystal Hibernation", 28262, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Vengeful Hammers", 27273, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Rite of the Great Dwarf", 26596, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/6/69/Rite_of_the_Great_Dwarf.png"),
                new Boon("Embrace the Darkness", 28001, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Enchanted Daggers", 28557, BoonSource.Revenant, BoonType.Intensity, 6, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Impossible Odds", 27581, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //facets
                new Boon("Facet of Light",27336, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Infuse Light",27737, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/6/60/Infuse_Light.png", RemoveType.Manual),
                new Boon("Facet of Darkness",28036, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, RemoveType.Manual),
                new Boon("Facet of Elements",28243, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, RemoveType.Manual),
                new Boon("Facet of Strength",27376, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, RemoveType.Manual),
                new Boon("Facet of Chaos",27983, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, RemoveType.Manual),
                new Boon("Facet of Nature",29275, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Naturalistic Resonance", 29379, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png", RemoveType.Manual),
                //legends
                new Boon("Legendary Centaur Stance",27972, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Legendary Dragon Stance",27732, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Legendary Dwarf Stance",27205, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Legendary Demon Stance",27928, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Legendary Assassin Stance",27890, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Legendary Renegade Stance",44272, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //summons
                new Boon("Breakrazor's Bastion",44682, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/a/a7/Breakrazor%27s_Bastion.png"),
                new Boon("Razorclaw's Rage",41016, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/7/73/Razorclaw%27s_Rage.png"),
                new Boon("Soulcleave's Summit",45026, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/7/78/Soulcleave%27s_Summit.png"),
                //traits
                new Boon("Vicious Lacerations",29395, BoonSource.Revenant, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff),
                new Boon("Assassin's Presence", 26854, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/5/54/Assassin%27s_Presence.png"),
                //new Boon("Expose Defenses", 48894, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Invoking Harmony",29025, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                //new Boon("Selfless Amplification",30509, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Hardening Persistence",28957, BoonSource.Revenant, BoonType.Intensity, 10, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Soothing Bastion",34136, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Kalla's Fervor",42883, BoonSource.Revenant, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff),
                new Boon("Improved Kalla's Fervor",45614, BoonSource.Revenant, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff),
                ///WARRIOR
                //skills
                new Boon("Riposte",14434, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Flames of War", 31708, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Blood Reckoning", 29466 , BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Rock Guard", 34256 , BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Sight beyond Sight",40616, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                //signets
                new Boon("Healing Signet",786, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Dolyak Signet",14458, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Fury",14459, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Might",14444, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Stamina",14478, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Rage",14496, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //banners
                new Boon("Banner of Strength", 14417, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/e/e1/Banner_of_Strength.png/33px-Banner_of_Strength.png"),
                new Boon("Banner of Discipline", 14449, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/5/5f/Banner_of_Discipline.png/33px-Banner_of_Discipline.png"),
                new Boon("Banner of Tactics",14450, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/3/3f/Banner_of_Tactics.png/33px-Banner_of_Tactics.png"),
                new Boon("Banner of Defense",14543, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/f/f1/Banner_of_Defense.png/33px-Banner_of_Defense.png"),
                //stances
                new Boon("Shield Stance",756, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Berserker's Stance",14453, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Enduring Pain",787, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Balanced Stance",34778, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Defiant Stance",21816, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Rampage",14484, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //traits
                new Boon("Empower Allies", 14222, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/4/4c/Empower_Allies.png/20px-Empower_Allies.png"),
                new Boon("Peak Performance",46853, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Furious Surge", 30204, BoonSource.Warrior, BoonType.Intensity, 25, BoonEnum.GraphOnlyBuff),
                new Boon("Health Gain per Adrenaline bar Spent", BoonSource.Warrior, BoonType.Intensity, 3, BoonEnum.GraphOnlyBuff),
                new Boon("Rousing Resilience",24383, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Always Angry",34099, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Full Counter",43949, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Attacker's Insight",41963, BoonSource.Warrior, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff),
                /// GUARDIAN
                //skills
                new Boon("Zealot's Flame", 9103, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Purging Flames",21672, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/28/Purging_Flames.png"),
                new Boon("Litany of Wrath",21665, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Renewed Focus",9255, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Ashes of the Just",41957, BoonSource.Guardian, BoonType.Intensity, 25, BoonEnum.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/6d/Epilogue-_Ashes_of_the_Just.png", RemoveType.Manual),
                new Boon("Eternal Oasis",44871, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/5/5f/Epilogue-_Eternal_Oasis.png"),
                new Boon("Unbroken Lines",43194, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/d/d8/Epilogue-_Unbroken_Lines.png"),
                //signets
                new Boon("Signet of Resolve",9220, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Bane Signet",9092, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Judgment",9156, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Mercy",9162, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Wrath",9100, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Courage",29633, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //virtues
                new Boon("Virtue of Justice", 9114, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Spears of Justice", 29632, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Virtue of Courage", 9113, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Shield of Courage", 29523, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Virtue of Resolve", 9119, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Wings of Resolve", 30308, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Tome of Justice",40530, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Tome of Courage",43508,BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Tome of Resolve",46298, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //traits
                new Boon("Strength in Numbers",13796, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/7/7b/Strength_in_Numbers.png"),
                new Boon("Invigorated Bulwark",30207, BoonSource.Guardian, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff),
                new Boon("Battle Presence", 17046, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/2/27/Battle_Presence.png"),
                //new Boon("Force of Will",29485, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),//not sure if intensity
                new Boon("Quickfire",45123, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                ///ENGINEER
                //skills
                new Boon("Static Shield",6055, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Absorb",6056, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("A.E.D.",21660, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Elixir S",5863, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Elixir X", BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Utility Goggles",5864, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Slick Shoes",5833, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Watchful Eye", BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Cooling Vapor",46444, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Photon Wall Deployed",46094, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Spectrum Shield",43066, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Gear Shield",5997, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                //Transforms
                new Boon("Rampage", BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Photon Forge",43708, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //Traits
                new Boon("Laser's Edge",44414, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Afterburner",42210, BoonSource.Engineer, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Iron Blooded",49065, BoonSource.Engineer, BoonType.Intensity, 25, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Streamlined Kits",18687, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Kinetic Charge",45781, BoonSource.Engineer, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Pinpoint Distribution", 38333, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/b/bf/Pinpoint_Distribution.png"),
                new Boon("Heat Therapy",40694, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Overheat", 40397, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                ///RANGER
                new Boon("Celestial Avatar", 31508, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Counterattack",14509, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //signets
                new Boon("Signet of Renewal",41147, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Stone",12627, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of the Hunt",12626, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of the Wild",12636, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //spirits
                // new Boon("Water Spirit", 50386, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/0/06/Water_Spirit.png/33px-Water_Spirit.png"),
                new Boon("Frost Spirit", 12544, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png"),
                new Boon("Sun Spirit", 12540, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/d/dd/Sun_Spirit.png/33px-Sun_Spirit.png"),
                new Boon("Stone Spirit", 12547, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/3/35/Stone_Spirit.png/20px-Stone_Spirit.png"),
                //new Boon("Storm Spirit", 50381, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/2/25/Storm_Spirit.png/30px-Storm_Spirit.png"),
                //reworked
                new Boon("Water Spirit", 50386, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/0/06/Water_Spirit.png/33px-Water_Spirit.png"),
                new Boon("Frost Spirit", 50421, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png"),
                new Boon("Sun Spirit", 50413, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/d/dd/Sun_Spirit.png/33px-Sun_Spirit.png"),
                new Boon("Stone Spirit", 50415, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/3/35/Stone_Spirit.png/20px-Stone_Spirit.png"),
                new Boon("Storm Spirit", 50381, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/2/25/Storm_Spirit.png/30px-Storm_Spirit.png"),
                //skills
                new Boon("Attack of Opportunity",12574, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Call of the Wild",36781, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Strength of the pack!",12554, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Sick 'Em!",33902, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Sharpening Stones",12536, BoonSource.Ranger, BoonType.Intensity, 10, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Ancestral Grace", 31584, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Glyph of Empowerment", 31803, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/f/f0/Glyph_of_Empowerment.png/33px-Glyph_of_Empowerment.png"),
                new Boon("Dolyak Stance",41815, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/7/71/Dolyak_Stance.png"),
                new Boon("Griffon Stance",46280, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/9/98/Griffon_Stance.png"),
                new Boon("Moa Stance",45038, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/6/66/Moa_Stance.png"),
                new Boon("Vulture Stance",44651, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/8/8f/Vulture_Stance.png"),
                new Boon("Bear Stance",40045, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/f/f0/Bear_Stance.png"),
                new Boon("One Wolf Pack",44139, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/3/3b/One_Wolf_Pack.png"),
                new Boon("Sharpen Spines",43266, BoonSource.Ranger, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //traits
                new Boon("Spotter", 14055, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/b/b0/Spotter.png"),
                new Boon("Opening Strike",13988, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Quick Draw",29703, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Light on your feet",30673, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Natural Mender",30449, BoonSource.Ranger, BoonType.Intensity, 10, BoonEnum.GraphOnlyBuff),
                new Boon("Lingering Light",32248, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Deadly",44932, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Ferocious",41720, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Supportive",40069, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Versatile",44693, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Stout",40272, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Unstoppable Union",44439, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Twice as Vicious",45600, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                ///THIEF
                //signets
                new Boon("Signet of Malice",13049, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Assassin's Signet (Passive)",13047, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Assassin's Signet (Active)",44597, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Infiltrator's Signet",13063, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Agility",13061, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Shadows",13059, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //venoms
                new Boon("Skelk Venom",-1, BoonSource.Thief, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/75/Skelk_Venom.png", RemoveType.Manual),
                new Boon("Ice Drake Venom",13095, BoonSource.Thief, BoonType.Intensity, 4, BoonEnum.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7b/Ice_Drake_Venom.png", RemoveType.Manual),
                new Boon("Devourer Venom", 13094, BoonSource.Thief, BoonType.Intensity, 2, BoonEnum.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4d/Devourer_Venom.png", RemoveType.Manual),
                new Boon("Skale Venom", 13036, BoonSource.Thief, BoonType.Intensity, 4, BoonEnum.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/14/Skale_Venom.png", RemoveType.Manual),
                new Boon("Spider Venom",13036, BoonSource.Thief, BoonType.Intensity, 6, BoonEnum.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/39/Spider_Venom.png", RemoveType.Manual),
                new Boon("Basilisk Venom", 13133, BoonSource.Thief, BoonType.Intensity, 1, BoonEnum.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/3a/Basilisk_Venom.png", RemoveType.Manual),
                //physical
                new Boon("Palm Strike",30423, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Pulmonary Impact",30510, BoonSource.Thief, BoonType.Intensity, 2, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //weapon
                new Boon("Infiltration",13135, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //transforms
                new Boon("Dagger Storm",13134, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Kneeling",42869, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //traits
                //new Boon("Deadeyes's Gaze",46333, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                //new Boon("Maleficent Seven",43606, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Hidden Killer",42720, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Lead Attacks",34659, BoonSource.Thief, BoonType.Intensity, 15, BoonEnum.GraphOnlyBuff),
                new Boon("Instant Reflexes",34283, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Lotus Training", 32200, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Unhindered Combatant", 32931, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Bounding Dodger", 33162, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                ///MESMER
                //signets
                new Boon("Signet of the Ether", 21751, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Domination",10231, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Illusions",10246, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Inspiration",10235, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Midnight",10233, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Humility",30739, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //skills
                new Boon("Distortion",10243, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Blur", 10335 , BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Mirror",10357, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Echo",29664, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Illusion of Life", BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //new Boon("Time Block",30134, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff), What is this?
                new Boon("Time Echo",29582, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Time Anchored",30136, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //traits
                new Boon("Fencer's Finesse", 30426 , BoonSource.Mesmer, BoonType.Intensity, 10, BoonEnum.GraphOnlyBuff),
                new Boon("Illusionary Defense",49099, BoonSource.Mesmer, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff),
                new Boon("Compunding Power",49058, BoonSource.Mesmer, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff),
                new Boon("Phantasmal Force", 44691 , BoonSource.Mesmer, BoonType.Intensity, 25, BoonEnum.GraphOnlyBuff),
                new Boon("Mirage Cloak",40408, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                ///NECROMANCER
                //forms
                new Boon("Lich Form",10631, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Death Shroud", 790, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Reaper's Shroud", 29446, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //signets
                new Boon("Signet of Vampirism",21761, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Plague Signet",10630, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Spite",10621, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of the Locust",10614, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Undeath",10610, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //skills
                new Boon("Spectral Walk",15083, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Infusing Terror", 30129, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //traits
                new Boon("Corrupter's Defense",30845, BoonSource.Necromancer, BoonType.Intensity, 10, BoonEnum.GraphOnlyBuff),
                new Boon("Vampiric Aura", 30285, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/d/da/Vampiric_Presence.png"),
                new Boon("Last Rites",29726, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/1/1a/Last_Rites_%28effect%29.png"),
                new Boon("Sadistic Searing",43626, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                ///ELEMENTALIST
                //signets
                new Boon("Signet of Restoration",739, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Air",5590, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Earth",5592, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Fire",5544, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Signet of Water",5591, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //attunments
                new Boon("Fire Attunement", 5585, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Water Attunement", 5586, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Air Attunement", 5575, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Earth Attunement", 5580, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //forms
                new Boon("Mist Form",5543, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Ride the Lightning",5588, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Vapor Form",5620, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Tornado",5534, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Whirlpool", BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //conjures
                new Boon("Conjure Earth Shield", 15788, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/7/7a/Conjure_Earth_Shield.png", RemoveType.Manual),
                new Boon("Conjure Flame Axe", 15789, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/a/a1/Conjure_Flame_Axe.png", RemoveType.Manual),
                new Boon("Conjure Frost Bow", 15790, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/c/c3/Conjure_Frost_Bow.png", RemoveType.Manual),
                new Boon("Conjure Lightning Hammer", 15791, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/1/1f/Conjure_Lightning_Hammer.png", RemoveType.Manual),
                new Boon("Conjure Fiery Greatsword", 15792, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, "https://wiki.guildwars2.com/images/e/e2/Conjure_Fiery_Greatsword.png", RemoveType.Manual),
                //skills
                new Boon("Arcane Power",5582, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/72/Arcane_Power.png", RemoveType.Manual),
                new Boon("Arcane Shield",5640, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Renewal of Fire",5764, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Glyph of Elemental Power (Fire)",5739, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Glyph of Elemental Power (Water)",5741, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Glyph of Elemental Power (Air)",5740, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Glyph of Elemental Power (Earth)",5742, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Rebound",31337, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/0/03/%22Rebound%21%22.png", RemoveType.Manual),
                new Boon("Rock Barrier",34633, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),//750?
                new Boon("Magnetic Wave",15794, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Obsidian Flesh",5667, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                //traits
                new Boon("Harmonious Conduit",31353, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Fresh Air",31353, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Soothing Mist", 5587, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/f/f7/Soothing_Mist.png"),
                new Boon("Lesser Arcane Shield",25579, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual),
                new Boon("Weaver's Prowess",42061, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Elements of Rage",42416, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                /// FOODS
                new Boon("Plate of Truffle Steak",9769, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food, "https://wiki.guildwars2.com/images/4/4c/Plate_of_Truffle_Steak.png"),
                new Boon("Bowl of Sweet and Spicy Butternut Squash Soup",17825, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food, "https://wiki.guildwars2.com/images/d/df/Bowl_of_Sweet_and_Spicy_Butternut_Squash_Soup.png"),
                new Boon("Red-Lentil Saobosa",46273, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food, "https://wiki.guildwars2.com/images/a/a8/Red-Lentil_Saobosa.png"),
                new Boon("Super Veggie Pizza",10008, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food, "https://wiki.guildwars2.com/images/6/66/Super_Veggie_Pizza.png"),
                new Boon("Rare Veggie Pizza",10009, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food, "https://wiki.guildwars2.com/images/a/a0/Rare_Veggie_Pizza.png"),
                new Boon("Bowl of Garlic Kale Sautee",-1, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food, "https://wiki.guildwars2.com/images/0/04/Bowl_of_Garlic_Kale_Sautee.png"),
                new Boon("Koi Cake",-1, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food, "https://wiki.guildwars2.com/images/1/14/Koi_Cake.png"),
                new Boon("Prickly Pear Pie",24800, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food, "https://wiki.guildwars2.com/images/0/0a/Prickly_Pear_Pie.png"),
                new Boon("Bowl of Nopalitos Sauté",-1, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food, "https://wiki.guildwars2.com/images/f/f1/Bowl_of_Nopalitos_Saut%C3%A9.png"),
                new Boon("Delicious Rice Ball",26529, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food, "https://wiki.guildwars2.com/images/5/5d/Delicious_Rice_Ball.png"),
                new Boon("Slice of Allspice Cake",33792, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food, "https://wiki.guildwars2.com/images/1/13/Slice_of_Allspice_Cake.png"),
                new Boon("Fried Golden Dumpling",26530, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food, "https://wiki.guildwars2.com/images/1/14/Fried_Golden_Dumpling.png"),
                new Boon("Bowl of Seaweed Salad",10080, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food, "https://wiki.guildwars2.com/images/1/1c/Bowl_of_Seaweed_Salad.png"),
                new Boon("Bowl of Orrian Truffle and Meat Stew",10096, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food, "https://wiki.guildwars2.com/images/b/b8/Bowl_of_Orrian_Truffle_and_Meat_Stew.png"),
                new Boon("Plate of Mussels Gnashblade",-1, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food, "https://wiki.guildwars2.com/images/7/7b/Plate_of_Mussels_Gnashblade.png"),
                /// UTILITIES
                new Boon("Superior Sharpening Stone",9963, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                new Boon("Master Maintenance Oil",-1, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Boon("Tuning Icicle",34206, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/7/75/Tuning_Icicle.png"),
                new Boon("Master Tuning Crystal",9967, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/5/58/Master_Tuning_Crystal.png"),
                new Boon("Toxic Sharpening Stone",-1, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/d/db/Toxic_Sharpening_Stone.png"),
                new Boon("Potent Superior Sharpening Stone",-1, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                new Boon("Toxic Maintenance Oil",21827, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/a/a6/Toxic_Maintenance_Oil.png"),
                new Boon("Magnanimous Maintenance Oil",38605, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/5/53/Magnanimous_Maintenance_Oil.png"),
                new Boon("Peppermint Oil",34187, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/b/bc/Peppermint_Oil.png"),
                new Boon("Potent Master Maintenance Oil",9968, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Boon("Furious Maintenance Oil",-1, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Boon("Furious Sharpening Stone",25882, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                new Boon("Bountiful Maintenance Oil",25879, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Boon("Toxic Focusing Crystal",21828, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/d/de/Toxic_Focusing_Crystal.png"),
                new Boon("Tin of Fruitcake",34211, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/a/af/Tin_of_Fruitcake.png"),
               // new Boon("Hylek Maintenance Oil",9968, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"), when wiki says "same stats" its litteraly the same buff
        };


        public static bool removePermission(long boonid, ParseEnum.BuffRemove buffremove, ParseEnum.IFF iff)
        {
            if (buffremove == 0)
            {
                return false;
            }
            Boon toCheck = allBoons.Find(x => x.getID() == boonid);
            if (toCheck != null)
            {
                switch (toCheck.remove_type)
                {
                    case RemoveType.CleanseFriend:
                        return iff != ParseEnum.IFF.Foe && (buffremove == ParseEnum.BuffRemove.All || buffremove == ParseEnum.BuffRemove.Single);
                    case RemoveType.CleanseFoe:
                        return iff != ParseEnum.IFF.Friend && (buffremove == ParseEnum.BuffRemove.All || buffremove == ParseEnum.BuffRemove.Single);
                    case RemoveType.Manual:
                        return buffremove == ParseEnum.BuffRemove.Manual || buffremove == ParseEnum.BuffRemove.All;
                    case RemoveType.All:
                        return buffremove == ParseEnum.BuffRemove.All || buffremove == ParseEnum.BuffRemove.Single || buffremove == ParseEnum.BuffRemove.Manual;
                    default:
                        return false;
                }
            }
            return false;
        }

        // debug
        public static List<Boon> getBoonByName(string name)
        {
            return allBoons.Where(x => x.getName() == name).ToList();
        }


        // Conditions
        public static List<Boon> getCondiBoonList()
        {
            return allBoons.Where(x => x.nature == BoonEnum.Condition).ToList();
        }
        // Boons
        public static List<Boon> getBoonList()
        {
            return allBoons.Where(x => x.nature == BoonEnum.Boon).ToList();
        }
        // Shareable buffs
        public static List<Boon> getOffensiveTableList()
        {
            return allBoons.Where(x => x.nature == BoonEnum.OffensiveBuffTable).ToList();
        }
        private static List<Boon> getOffensiveTableList(BoonSource source)
        {
            return getOffensiveTableList().Where(x => x.source == source).ToList();
        }
        public static List<Boon> getOffensiveTableList(String source)
        {
            return getOffensiveTableList(ProfToEnum(source));
        }
        public static List<Boon> getDefensiveTableList()
        {
            return allBoons.Where(x => x.nature == BoonEnum.DefensiveBuffTable).ToList();
        }
        private static List<Boon> getDefensiveTableList(BoonSource source)
        {
            return getDefensiveTableList().Where(x => x.source == source).ToList();
        }
        public static List<Boon> getDefensiveTableList(String source)
        {
            return getDefensiveTableList(ProfToEnum(source));
        }
        // Table + graph
        public static List<Boon> getTableProfList()
        {
            return allBoons.Where(x => x.nature == BoonEnum.OffensiveBuffTable || x.nature == BoonEnum.DefensiveBuffTable).ToList();   
        }
        private static List<Boon> getTableProfList(BoonSource source)
        {
            return getTableProfList().Where(x => x.source == source).ToList();
        }
        public static List<Boon> getTableProfList(String source)
        {
            return getTableProfList(ProfToEnum(source));
        }
        // Foods
        public static List<Boon> getFoodList()
        {
            return allBoons.Where(x => x.nature == BoonEnum.Food).ToList();
        }
        // Utilities
        public static List<Boon> getUtilityList()
        {
            return allBoons.Where(x => x.nature == BoonEnum.Utility).ToList();
        }
        // All buffs
        public static List<Boon> getAllBuffList()
        {
            List<Boon> res = new List<Boon>();
            // correct order for the boon graph
            res.AddRange(getBoonList());
            res.AddRange(getOffensiveTableList());
            res.AddRange(getDefensiveTableList());
            res.AddRange(getRemainingBuffsList());
            return res;
        }
        // Non shareable buffs
        public static List<Boon> getRemainingBuffsList()
        {
            return allBoons.Where(x => x.nature == BoonEnum.GraphOnlyBuff).ToList();
        }
        private static List<Boon> getRemainingBuffsList(BoonSource source)
        {
            return getRemainingBuffsList().Where(x => x.source == source).ToList();
        }
        public static List<Boon> getRemainingBuffsList(String source)
        {
            return getRemainingBuffsList(ProfToEnum(source));
        }


        // Getters
        public string getName()
        {
            return this.name;
        }
        public long getID()
        {
            return id;
        }
        public BoonSource getSource()
        {
            return source;
        }

        public BoonType getType()
        {
            return type;
        }

        public int getCapacity()
        {
            return capacity;
        }

        public string getLink()
        {
            return link;
        }

        public BoonSimulator CreateSimulator()
        {
            switch(type)
            {
                case BoonType.Intensity: return new BoonSimulatorIntensity(capacity);
                case BoonType.Duration:  return new BoonSimulatorDuration(capacity);
                default: throw new InvalidOperationException();
            }
        }
    }
}
