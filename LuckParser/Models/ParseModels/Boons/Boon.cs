using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Boon
    {
        // Boon
        public enum BoonEnum { Condition, Boon, OffensiveBuffTable, DefensiveBuffTable, GraphOnlyBuff, Food, Utility };
        public enum BoonSource { Mixed, Necromancer, Elementalist, Mesmer, Warrior, Revenant, Guardian, Thief, Ranger, Engineer, Item, Boss };
        private enum RemoveType { CleanseFoe, CleanseFriend, Manual, None, All };
        public enum BoonType { Duration, Intensity };
        private enum Logic { Queue, HealingPower, Override, ForceOverride };

        private static BoonSource ProfToEnum(string prof)
        {
            switch (prof)
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
        private readonly string _name;
        private readonly long _id;
        private readonly BoonEnum _nature;
        private readonly BoonSource _source;
        private readonly RemoveType _removeType;
        private readonly BoonType _type;
        private readonly int _capacity;
        private readonly string _link;
        private readonly Logic _logic;

        private Boon(string name, int id, BoonSource source, BoonType type, int capacity, BoonEnum nature, RemoveType removeType, Logic logic, string link = "")
        {
            _name = name;
            _id = id;
            _source = source;
            _type = type;
            _capacity = capacity;
            _nature = nature;
            _link = link;
            _removeType = removeType;
            _logic = logic;
        }
        // Public Methods

        private static List<Boon> _allBoons = new List<Boon>
            {
                //Base boons
                new Boon("Might", 740, BoonSource.Mixed, BoonType.Intensity, 25, BoonEnum.Boon, RemoveType.CleanseFoe, Logic.Override, "https://wiki.guildwars2.com/images/7/7c/Might.png"),
                new Boon("Fury", 725, BoonSource.Mixed, BoonType.Duration, 9, BoonEnum.Boon, RemoveType.CleanseFoe, Logic.Queue, "https://wiki.guildwars2.com/images/4/46/Fury.png"),//or 3m and 30s
                new Boon("Quickness", 1187, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Boon, RemoveType.CleanseFoe, Logic.Queue, "https://wiki.guildwars2.com/images/b/b4/Quickness.png"),
                new Boon("Alacrity", 30328, BoonSource.Mixed, BoonType.Duration, 9, BoonEnum.Boon, RemoveType.CleanseFoe, Logic.Queue, "https://wiki.guildwars2.com/images/thumb/4/4c/Alacrity.png/20px-Alacrity.png"),
                new Boon("Protection", 717, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Boon, RemoveType.CleanseFoe, Logic.Queue, "https://wiki.guildwars2.com/images/6/6c/Protection.png"),
                new Boon("Regeneration", 718, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Boon, RemoveType.CleanseFoe, Logic.HealingPower, "https://wiki.guildwars2.com/images/5/53/Regeneration.png"),
                new Boon("Vigor", 726, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Boon, RemoveType.CleanseFoe, Logic.Queue, "https://wiki.guildwars2.com/images/f/f4/Vigor.png"),
                new Boon("Aegis", 743, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Boon, RemoveType.All, Logic.Queue, "https://wiki.guildwars2.com/images/e/e5/Aegis.png"),
                new Boon("Stability", 1122, BoonSource.Mixed, BoonType.Intensity, 25, BoonEnum.Boon, RemoveType.All, Logic.Override, "https://wiki.guildwars2.com/images/a/ae/Stability.png"),
                new Boon("Swiftness", 719, BoonSource.Mixed, BoonType.Duration, 9, BoonEnum.Boon, RemoveType.CleanseFoe, Logic.Queue, "https://wiki.guildwars2.com/images/a/af/Swiftness.png"),
                new Boon("Retaliation", 873, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Boon, RemoveType.CleanseFoe, Logic.Queue, "https://wiki.guildwars2.com/images/5/53/Retaliation.png"),
                new Boon("Resistance", 26980, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Boon, RemoveType.CleanseFoe, Logic.Queue, "https://wiki.guildwars2.com/images/thumb/e/e9/Resistance_40px.png/20px-Resistance_40px.png"),
                // Condis         
                new Boon("Bleeding", 736, BoonSource.Mixed, BoonType.Intensity, 1500, BoonEnum.Condition, RemoveType.CleanseFriend, Logic.Override, "https://wiki.guildwars2.com/images/thumb/3/33/Bleeding.png/20px-Bleeding.png"),
                new Boon("Burning", 737, BoonSource.Mixed, BoonType.Intensity, 1500, BoonEnum.Condition, RemoveType.CleanseFriend, Logic.Override, "https://wiki.guildwars2.com/images/thumb/4/45/Burning.png/20px-Burning.png"),
                new Boon("Confusion", 861, BoonSource.Mixed, BoonType.Intensity, 1500, BoonEnum.Condition, RemoveType.CleanseFriend, Logic.Override, "https://wiki.guildwars2.com/images/thumb/e/e6/Confusion.png/20px-Confusion.png"),
                new Boon("Poison", 723, BoonSource.Mixed, BoonType.Intensity, 1500, BoonEnum.Condition, RemoveType.CleanseFriend, Logic.Override, "https://wiki.guildwars2.com/images/thumb/0/05/Poison.png/20px-Poison.png"),
                new Boon("Torment", 19426, BoonSource.Mixed, BoonType.Intensity, 1500, BoonEnum.Condition, RemoveType.CleanseFriend, Logic.Override, "https://wiki.guildwars2.com/images/thumb/0/08/Torment.png/20px-Torment.png"),
                new Boon("Blind", 720, BoonSource.Mixed, BoonType.Duration, 9, BoonEnum.Condition, RemoveType.CleanseFriend, Logic.Queue, "https://wiki.guildwars2.com/images/thumb/3/33/Blinded.png/20px-Blinded.png"),
                new Boon("Chilled", 722, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Condition, RemoveType.CleanseFriend, Logic.Queue, "https://wiki.guildwars2.com/images/thumb/a/a6/Chilled.png/20px-Chilled.png"),
                new Boon("Crippled", 721, BoonSource.Mixed, BoonType.Duration, 9, BoonEnum.Condition, RemoveType.CleanseFriend, Logic.Queue, "https://wiki.guildwars2.com/images/thumb/f/fb/Crippled.png/20px-Crippled.png"),
                new Boon("Fear", 791, BoonSource.Mixed, BoonType.Duration, 9, BoonEnum.Condition, RemoveType.CleanseFriend, Logic.Queue, "https://wiki.guildwars2.com/images/thumb/e/e6/Fear.png/20px-Fear.png"),
                new Boon("Immobile", 727, BoonSource.Mixed, BoonType.Duration, 3, BoonEnum.Condition, RemoveType.CleanseFriend, Logic.Queue, "https://wiki.guildwars2.com/images/thumb/3/32/Immobile.png/20px-Immobile.png"),
                new Boon("Slow", 26766, BoonSource.Mixed, BoonType.Duration, 9, BoonEnum.Condition, RemoveType.CleanseFriend, Logic.Queue, "https://wiki.guildwars2.com/images/thumb/f/fb/Slow_40px.png/20px-Slow_40px.png"),
                new Boon("Weakness", 742, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Condition, RemoveType.CleanseFriend, Logic.Queue, "https://wiki.guildwars2.com/images/thumb/f/f9/Weakness.png/20px-Weakness.png"),
                new Boon("Taunt", 27705, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Condition, RemoveType.CleanseFriend, Logic.Queue, "https://wiki.guildwars2.com/images/thumb/c/cc/Taunt.png/20px-Taunt.png"),
                new Boon("Vulnerability", 738, BoonSource.Mixed, BoonType.Intensity, 25, BoonEnum.Condition, RemoveType.CleanseFriend, Logic.Override, "https://wiki.guildwars2.com/images/thumb/a/af/Vulnerability.png/20px-Vulnerability.png"),
                new Boon("Retaliation", 873, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.Condition, RemoveType.CleanseFoe, Logic.Queue, "https://wiki.guildwars2.com/images/5/53/Retaliation.png"),
                // Generic
                new Boon("Stealth", 13017, BoonSource.Mixed, BoonType.Duration, 5, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Queue),
                new Boon("Revealed", 890, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Superspeed", 5974, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.None, Logic.ForceOverride,"https://wiki.guildwars2.com/images/1/1a/Super_Speed.png"),
                //new Boon("Invulnerability", 801, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Boon("Unblockable",-1, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                // Sigils
                new Boon("Sigil of Concentration", 33719, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.None, Logic.Override),
                //Auras
                new Boon("Chaos Armor", 10332, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Fire Shield", 5677, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Frost Aura", 5579, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Light Aura", 25518, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Magnetic Aura", 5684, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Shocking Aura", 5577, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                //race
                new Boon("Take Root", 12459, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Become the Bear",12426, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Become the Raven",12405, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Become the Snow Leopard",12400, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Become the Wolf",12393, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Avatar of Melandru", 12368, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Power Suit",12326, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Reaper of Grenth", 12366, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Charrzooka",43503, BoonSource.Mixed, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                // BOSS
                new Boon("Unnatural Signet",38224, BoonSource.Boss, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Compromised",35096, BoonSource.Boss, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Spirited Fusion",31722, BoonSource.Boss, BoonType.Intensity, 500, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Blood Shield",34376, BoonSource.Boss, BoonType.Intensity, 18, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Blood Shield",34518, BoonSource.Boss, BoonType.Intensity, 18, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Blood Fueled",34422, BoonSource.Boss, BoonType.Intensity, 20, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Blood Fueled",34428, BoonSource.Boss, BoonType.Intensity, 20, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //REVENANT
                //skills
                new Boon("Crystal Hibernation", 28262, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Vengeful Hammers", 27273, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Rite of the Great Dwarf", 26596, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, RemoveType.Manual, Logic.Override, "https://wiki.guildwars2.com/images/6/69/Rite_of_the_Great_Dwarf.png"),
                new Boon("Embrace the Darkness", 28001, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Enchanted Daggers", 28557, BoonSource.Revenant, BoonType.Intensity, 6, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Impossible Odds", 27581, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //facets
                new Boon("Facet of Light",27336, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Infuse Light",27737, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, RemoveType.Manual, Logic.Override, "https://wiki.guildwars2.com/images/6/60/Infuse_Light.png"),
                new Boon("Facet of Darkness",28036, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, RemoveType.Manual, Logic.Override),
                new Boon("Facet of Elements",28243, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, RemoveType.Manual, Logic.Override),
                new Boon("Facet of Strength",27376, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, RemoveType.Manual, Logic.Override),
                new Boon("Facet of Chaos",27983, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, RemoveType.Manual, Logic.Override),
                new Boon("Facet of Nature",29275, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Naturalistic Resonance", 29379, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, RemoveType.Manual, Logic.Override, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),
                //legends
                new Boon("Legendary Centaur Stance",27972, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Legendary Dragon Stance",27732, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Legendary Dwarf Stance",27205, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Legendary Demon Stance",27928, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Legendary Assassin Stance",27890, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Legendary Renegade Stance",44272, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //summons
                new Boon("Breakrazor's Bastion",44682, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/a/a7/Breakrazor%27s_Bastion.png"),
                new Boon("Razorclaw's Rage",41016, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/7/73/Razorclaw%27s_Rage.png"),
                new Boon("Soulcleave's Summit",45026, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Soulcleave%27s_Summit.png"),
                //traits
                new Boon("Vicious Lacerations",29395, BoonSource.Revenant, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Assassin's Presence", 26854, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/5/54/Assassin%27s_Presence.png"),
                //new Boon("Expose Defenses", 48894, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Invoking Harmony",29025, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                //new Boon("Selfless Amplification",30509, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Hardening Persistence",28957, BoonSource.Revenant, BoonType.Intensity, 10, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Soothing Bastion",34136, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Kalla's Fervor",42883, BoonSource.Revenant, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Improved Kalla's Fervor",45614, BoonSource.Revenant, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                //WARRIOR
                //skills
                new Boon("Riposte",14434, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Flames of War", 31708, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Blood Reckoning", 29466 , BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Rock Guard", 34256 , BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Sight beyond Sight",40616, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                //signets
                new Boon("Healing Signet",786, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Dolyak Signet",14458, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Fury",14459, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Might",14444, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Stamina",14478, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Rage",14496, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //banners
                new Boon("Banner of Strength", 14417, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/thumb/e/e1/Banner_of_Strength.png/33px-Banner_of_Strength.png"),
                new Boon("Banner of Discipline", 14449, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/thumb/5/5f/Banner_of_Discipline.png/33px-Banner_of_Discipline.png"),
                new Boon("Banner of Tactics",14450, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/thumb/3/3f/Banner_of_Tactics.png/33px-Banner_of_Tactics.png"),
                new Boon("Banner of Defense",14543, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/thumb/f/f1/Banner_of_Defense.png/33px-Banner_of_Defense.png"),
                //stances
                new Boon("Shield Stance",756, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Berserker's Stance",14453, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Enduring Pain",787, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Balanced Stance",34778, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Defiant Stance",21816, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Rampage",14484, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //traits
                new Boon("Empower Allies", 14222, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/thumb/4/4c/Empower_Allies.png/20px-Empower_Allies.png"),
                new Boon("Peak Performance",46853, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Furious Surge", 30204, BoonSource.Warrior, BoonType.Intensity, 25, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Health Gain per Adrenaline bar Spent",-1, BoonSource.Warrior, BoonType.Intensity, 3, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Rousing Resilience",24383, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Always Angry",34099, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Full Counter",43949, BoonSource.Warrior, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Attacker's Insight",41963, BoonSource.Warrior, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                // GUARDIAN
                //skills
                new Boon("Zealot's Flame", 9103, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Purging Flames",21672, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/2/28/Purging_Flames.png"),
                new Boon("Litany of Wrath",21665, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Renewed Focus",9255, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Ashes of the Just",41957, BoonSource.Guardian, BoonType.Intensity, 25, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override, "https://wiki.guildwars2.com/images/6/6d/Epilogue-_Ashes_of_the_Just.png"),
                new Boon("Eternal Oasis",44871, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/5/5f/Epilogue-_Eternal_Oasis.png"),
                new Boon("Unbroken Lines",43194, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/d/d8/Epilogue-_Unbroken_Lines.png"),
                //signets
                new Boon("Signet of Resolve",9220, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Bane Signet",9092, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Judgment",9156, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Mercy",9162, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Wrath",9100, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Courage",29633, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //virtues
                new Boon("Virtue of Justice", 9114, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Spears of Justice", 29632, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Virtue of Courage", 9113, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Shield of Courage", 29523, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Virtue of Resolve", 9119, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Wings of Resolve", 30308, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Tome of Justice",40530, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Tome of Courage",43508,BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Tome of Resolve",46298, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //traits
                new Boon("Strength in Numbers",13796, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/7/7b/Strength_in_Numbers.png"),
                new Boon("Invigorated Bulwark",30207, BoonSource.Guardian, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Battle Presence", 17046, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/2/27/Battle_Presence.png"),
                //new Boon("Force of Will",29485, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),//not sure if intensity
                new Boon("Quickfire",45123, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //ENGINEER
                //skills
                new Boon("Static Shield",6055, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Absorb",6056, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("A.E.D.",21660, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Elixir S",5863, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Elixir X", -1,BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Utility Goggles",5864, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Slick Shoes",5833, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Watchful Eye",-1, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Cooling Vapor",46444, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Photon Wall Deployed",46094, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Spectrum Shield",43066, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Gear Shield",5997, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                //Transforms
                new Boon("Rampage",-1, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Photon Forge",43708, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //Traits
                new Boon("Laser's Edge",44414, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Afterburner",42210, BoonSource.Engineer, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Iron Blooded",49065, BoonSource.Engineer, BoonType.Intensity, 25, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Streamlined Kits",18687, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Kinetic Charge",45781, BoonSource.Engineer, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Pinpoint Distribution", 38333, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/b/bf/Pinpoint_Distribution.png"),
                new Boon("Heat Therapy",40694, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Overheat", 40397, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Thermal Vision", 51389, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                //RANGER
                new Boon("Celestial Avatar", 31508, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Counterattack",14509, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //signets
                new Boon("Signet of Renewal",41147, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Stone",12627, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of the Hunt",12626, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of the Wild",12636, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //spirits
                // new Boon("Water Spirit", 50386, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/0/06/Water_Spirit.png/33px-Water_Spirit.png"),
                new Boon("Frost Spirit", 12544, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png"),
                new Boon("Sun Spirit", 12540, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/thumb/d/dd/Sun_Spirit.png/33px-Sun_Spirit.png"),
                new Boon("Stone Spirit", 12547, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/thumb/3/35/Stone_Spirit.png/20px-Stone_Spirit.png"),
                //new Boon("Storm Spirit", 50381, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/2/25/Storm_Spirit.png/30px-Storm_Spirit.png"),
                //reworked
                new Boon("Water Spirit", 50386, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/thumb/0/06/Water_Spirit.png/33px-Water_Spirit.png"),
                new Boon("Frost Spirit", 50421, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png"),
                new Boon("Sun Spirit", 50413, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/thumb/d/dd/Sun_Spirit.png/33px-Sun_Spirit.png"),
                new Boon("Stone Spirit", 50415, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/thumb/3/35/Stone_Spirit.png/20px-Stone_Spirit.png"),
                new Boon("Storm Spirit", 50381, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/thumb/2/25/Storm_Spirit.png/30px-Storm_Spirit.png"),
                //skills
                new Boon("Attack of Opportunity",12574, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Call of the Wild",36781, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Strength of the pack!",12554, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Sick 'Em!",33902, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Sharpening Stones",12536, BoonSource.Ranger, BoonType.Intensity, 10, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Ancestral Grace", 31584, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Glyph of Empowerment", 31803, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/thumb/f/f0/Glyph_of_Empowerment.png/33px-Glyph_of_Empowerment.png"),
                new Boon("Dolyak Stance",41815, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/7/71/Dolyak_Stance.png"),
                new Boon("Griffon Stance",46280, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/9/98/Griffon_Stance.png"),
                new Boon("Moa Stance",45038, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/6/66/Moa_Stance.png"),
                new Boon("Vulture Stance",44651, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/8/8f/Vulture_Stance.png"),
                new Boon("Bear Stance",40045, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/f/f0/Bear_Stance.png"),
                new Boon("One Wolf Pack",44139, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/3/3b/One_Wolf_Pack.png"),
                new Boon("Sharpen Spines",43266, BoonSource.Ranger, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //traits
                new Boon("Spotter", 14055, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/b/b0/Spotter.png"),
                new Boon("Opening Strike",13988, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Quick Draw",29703, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Light on your feet",30673, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Natural Mender",30449, BoonSource.Ranger, BoonType.Intensity, 10, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Lingering Light",32248, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Deadly",44932, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Ferocious",41720, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Supportive",40069, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Versatile",44693, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Stout",40272, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Unstoppable Union",44439, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Twice as Vicious",45600, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //THIEF
                //signets
                new Boon("Signet of Malice",13049, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Assassin's Signet (Passive)",13047, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Assassin's Signet (Active)",44597, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Infiltrator's Signet",13063, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Agility",13061, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Shadows",13059, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //venoms
                new Boon("Skelk Venom",-1, BoonSource.Thief, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override, "https://wiki.guildwars2.com/images/7/75/Skelk_Venom.png"),
                new Boon("Ice Drake Venom",13095, BoonSource.Thief, BoonType.Intensity, 4, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override, "https://wiki.guildwars2.com/images/7/7b/Ice_Drake_Venom.png"),
                new Boon("Devourer Venom", 13094, BoonSource.Thief, BoonType.Intensity, 2, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override, "https://wiki.guildwars2.com/images/4/4d/Devourer_Venom.png"),
                new Boon("Skale Venom", 13054, BoonSource.Thief, BoonType.Intensity, 4, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override, "https://wiki.guildwars2.com/images/1/14/Skale_Venom.png"),
                new Boon("Spider Venom",13036, BoonSource.Thief, BoonType.Intensity, 6, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override, "https://wiki.guildwars2.com/images/3/39/Spider_Venom.png"),
                new Boon("Basilisk Venom", 13133, BoonSource.Thief, BoonType.Intensity, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override, "https://wiki.guildwars2.com/images/3/3a/Basilisk_Venom.png"),
                //physical
                new Boon("Palm Strike",30423, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Pulmonary Impact",30510, BoonSource.Thief, BoonType.Intensity, 2, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //weapon
                new Boon("Infiltration",13135, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //transforms
                new Boon("Dagger Storm",13134, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Kneeling",42869, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //traits
                //new Boon("Deadeye's Gaze",46333, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                //new Boon("Maleficent Seven",43606, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Hidden Killer",42720, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Lead Attacks",34659, BoonSource.Thief, BoonType.Intensity, 15, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Instant Reflexes",34283, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Lotus Training", 32200, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Unhindered Combatant", 32931, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Bounding Dodger", 33162, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //MESMER
                //signets
                new Boon("Signet of the Ether", 21751, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Domination",10231, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Illusions",10246, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Inspiration",10235, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Midnight",10233, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Humility",30739, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //skills
                new Boon("Distortion",10243, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Blur", 10335 , BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Mirror",10357, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Echo",29664, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Illusion of Life",-1, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //new Boon("Time Block",30134, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff), What is this?
                new Boon("Time Echo",29582, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Time Anchored",30136, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //traits
                new Boon("Fencer's Finesse", 30426 , BoonSource.Mesmer, BoonType.Intensity, 10, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Illusionary Defense",49099, BoonSource.Mesmer, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Compounding Power",49058, BoonSource.Mesmer, BoonType.Intensity, 5, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Phantasmal Force", 44691 , BoonSource.Mesmer, BoonType.Intensity, 25, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Mirage Cloak",40408, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //NECROMANCER
                //forms
                new Boon("Lich Form",10631, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Death Shroud", 790, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Reaper's Shroud", 29446, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //signets
                new Boon("Signet of Vampirism",21761, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Plague Signet",10630, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Spite",10621, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of the Locust",10614, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Undeath",10610, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //skills
                new Boon("Spectral Walk",15083, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Infusing Terror", 30129, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //traits
                new Boon("Corrupter's Defense",30845, BoonSource.Necromancer, BoonType.Intensity, 10, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Vampiric Aura", 30285, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/d/da/Vampiric_Presence.png"),
                new Boon("Last Rites",29726, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/1/1a/Last_Rites_%28effect%29.png"),
                new Boon("Sadistic Searing",43626, BoonSource.Necromancer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //ELEMENTALIST
                //signets
                new Boon("Signet of Restoration",739, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Air",5590, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Earth",5592, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Fire",5544, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Signet of Water",5591, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //attunements
                new Boon("Fire Attunement", 5585, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Water Attunement", 5586, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Air Attunement", 5575, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Earth Attunement", 5580, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //forms
                new Boon("Mist Form",5543, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Ride the Lightning",5588, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Vapor Form",5620, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Tornado",5534, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Whirlpool", -1,BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //conjures
                new Boon("Conjure Earth Shield", 15788, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, RemoveType.Manual, Logic.Override, "https://wiki.guildwars2.com/images/7/7a/Conjure_Earth_Shield.png"),
                new Boon("Conjure Flame Axe", 15789, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, RemoveType.Manual, Logic.Override, "https://wiki.guildwars2.com/images/a/a1/Conjure_Flame_Axe.png"),
                new Boon("Conjure Frost Bow", 15790, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, RemoveType.Manual, Logic.Override, "https://wiki.guildwars2.com/images/c/c3/Conjure_Frost_Bow.png"),
                new Boon("Conjure Lightning Hammer", 15791, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, RemoveType.Manual, Logic.Override, "https://wiki.guildwars2.com/images/1/1f/Conjure_Lightning_Hammer.png"),
                new Boon("Conjure Fiery Greatsword", 15792, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.OffensiveBuffTable, RemoveType.Manual, Logic.Override, "https://wiki.guildwars2.com/images/e/e2/Conjure_Fiery_Greatsword.png"),
                //skills
                new Boon("Arcane Power",5582, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override, "https://wiki.guildwars2.com/images/7/72/Arcane_Power.png"),
                new Boon("Arcane Shield",5640, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Renewal of Fire",5764, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Glyph of Elemental Power (Fire)",5739, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Glyph of Elemental Power (Water)",5741, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Glyph of Elemental Power (Air)",5740, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Glyph of Elemental Power (Earth)",5742, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Rebound",31337, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, RemoveType.Manual, Logic.Override, "https://wiki.guildwars2.com/images/0/03/%22Rebound%21%22.png"),
                new Boon("Rock Barrier",34633, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),//750?
                new Boon("Magnetic Wave",15794, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Obsidian Flesh",5667, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                //traits
                new Boon("Harmonious Conduit",31353, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Fresh Air",34241, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Soothing Mist", 5587, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/f/f7/Soothing_Mist.png"),
                new Boon("Lesser Arcane Shield",25579, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Manual, Logic.Override),
                new Boon("Weaver's Prowess",42061, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                new Boon("Elements of Rage",42416, BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff,RemoveType.None, Logic.Override),
                // FOODS
                new Boon("Plate of Truffle Steak",9769, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/4/4c/Plate_of_Truffle_Steak.png"),
                new Boon("Bowl of Sweet and Spicy Butternut Squash Soup",17825, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/d/df/Bowl_of_Sweet_and_Spicy_Butternut_Squash_Soup.png"),
                new Boon("Bowl Curry Butternut Squash Soup",9829, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/1/16/Bowl_of_Curry_Butternut_Squash_Soup.png"),
                new Boon("Red-Lentil Saobosa",46273, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/a/a8/Red-Lentil_Saobosa.png"),
                new Boon("Super Veggie Pizza",10008, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/6/66/Super_Veggie_Pizza.png"),
                new Boon("Rare Veggie Pizza",10009, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/a/a0/Rare_Veggie_Pizza.png"),
                new Boon("Bowl of Garlic Kale Sautee",-1, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/0/04/Bowl_of_Garlic_Kale_Sautee.png"),
                new Boon("Koi Cake",10009, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/1/14/Koi_Cake.png"),
                new Boon("Prickly Pear Pie",24800, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/0/0a/Prickly_Pear_Pie.png"),
                new Boon("Bowl of Nopalitos Sauté",-1, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/f/f1/Bowl_of_Nopalitos_Saut%C3%A9.png"),
                new Boon("Loaf of Candy Cactus Cornbread",24797, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/b/b2/Loaf_of_Candy_Cactus_Cornbread.png"),
                new Boon("Delicious Rice Ball",26529, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/5/5d/Delicious_Rice_Ball.png"),
                new Boon("Slice of Allspice Cake",33792, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/1/13/Slice_of_Allspice_Cake.png"),
                new Boon("Fried Golden Dumpling",26530, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/1/14/Fried_Golden_Dumpling.png"),
                new Boon("Bowl of Seaweed Salad",10080, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/1/1c/Bowl_of_Seaweed_Salad.png"),
                new Boon("Bowl of Orrian Truffle and Meat Stew",10096, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/b/b8/Bowl_of_Orrian_Truffle_and_Meat_Stew.png"),
                new Boon("Plate of Mussels Gnashblade",33476, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/7/7b/Plate_of_Mussels_Gnashblade.png"),
                new Boon("Spring Roll",26534, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/d/da/Spring_Roll.png"),
                new Boon("Plate of Beef Rendang",49686, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/d/d0/Plate_of_Beef_Rendang.png"),
                new Boon("Dragon's Revelry Starcake",19451, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/2/2b/Dragon%27s_Revelry_Starcake.png"),
                new Boon("Avocado Smoothie",50091, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/8/83/Avocado_Smoothie.png"),
                new Boon("Carrot Souffle",-1, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/7/71/Carrot_Souffl%C3%A9.png"), //same as Dragon's_Breath_Bun
                new Boon("Plate of Truffle Steak Dinner",-1, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/9/92/Plate_of_Truffle_Steak_Dinner.png"), //same as Dragon's Breath Bun
                new Boon("Dragon's Breath Bun",9750, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/5/53/Dragon%27s_Breath_Bun.png"),
                new Boon("Karka Egg Omelet",9756, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/9/9e/Karka_Egg_Omelet.png"),
                new Boon("Steamed Red Dumpling",26536, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/8/8c/Steamed_Red_Dumpling.png"),
                new Boon("Saffron Stuffed Mushroom",-1, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Food,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/e/e2/Saffron_Stuffed_Mushroom.png"), //same as Karka Egg Omelet
                // UTILITIES 
                // 1h versions have the same ID as 30 min versions 
                new Boon("Superior Sharpening Stone",9963, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                new Boon("Potent Superior Sharpening Stone",-1, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"), 
                new Boon("Master Maintenance Oil",9968, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Boon("Potent Master Maintenance Oil",-1, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"), 
                new Boon("Tuning Icicle",34206, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/7/75/Tuning_Icicle.png"),
                new Boon("Master Tuning Crystal",9967, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/5/58/Master_Tuning_Crystal.png"),
                new Boon("Potent Master Tuning Crystal",-1, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/5/58/Master_Tuning_Crystal.png"), 
                new Boon("Toxic Sharpening Stone",21826, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/d/db/Toxic_Sharpening_Stone.png"),
                new Boon("Toxic Maintenance Oil",21827, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/a/a6/Toxic_Maintenance_Oil.png"),
                new Boon("Toxic Focusing Crystal",21828, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/d/de/Toxic_Focusing_Crystal.png"),
                new Boon("Magnanimous Maintenance Oil",38605, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/5/53/Magnanimous_Maintenance_Oil.png"),
                new Boon("Peppermint Oil",34187, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/b/bc/Peppermint_Oil.png"),
                new Boon("Furious Maintenance Oil",25881, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Boon("Furious Sharpening Stone",25882, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                new Boon("Bountiful Maintenance Oil",25879, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Boon("Tin of Fruitcake",34211, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/a/af/Tin_of_Fruitcake.png"),
                new Boon("Writ of Masterful Malice",33836, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/2/20/Writ_of_Masterful_Malice.png"),
                new Boon("Writ of Masterful Strength",33297, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility,RemoveType.None, Logic.Override, "https://wiki.guildwars2.com/images/2/2b/Writ_of_Masterful_Strength.png"),
                // new Boon("Hylek Maintenance Oil",9968, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"), when wiki says "same stats" its literally the same buff
        };


        public static bool RemovePermission(long boonid, ParseEnum.BuffRemove buffremove, ParseEnum.IFF iff)
        {
            if (buffremove == 0)
            {
                return false;
            }
            Boon toCheck = _allBoons.Find(x => x.GetID() == boonid);
            if (toCheck != null)
            {
                switch (toCheck._removeType)
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
        public static List<Boon> GetBoonByName(string name)
        {
            return _allBoons.Where(x => x.GetName() == name).ToList();
        }

        // get everything
        public static List<Boon> GetAll()
        {
            return _allBoons;
        }

        // Conditions
        public static List<Boon> GetCondiBoonList()
        {
            return _allBoons.Where(x => x._nature == BoonEnum.Condition).ToList();
        }
        // Boons
        public static List<Boon> GetBoonList()
        {
            return _allBoons.Where(x => x._nature == BoonEnum.Boon).ToList();
        }
        // Shareable buffs
        public static List<Boon> GetOffensiveTableList()
        {
            return _allBoons.Where(x => x._nature == BoonEnum.OffensiveBuffTable).ToList();
        }
        private static List<Boon> GetOffensiveTableList(BoonSource source)
        {
            return GetOffensiveTableList().Where(x => x._source == source).ToList();
        }
        public static List<Boon> GetOffensiveTableList(String source)
        {
            return GetOffensiveTableList(ProfToEnum(source));
        }
        public static List<Boon> GetDefensiveTableList()
        {
            return _allBoons.Where(x => x._nature == BoonEnum.DefensiveBuffTable).ToList();
        }
        private static List<Boon> GetDefensiveTableList(BoonSource source)
        {
            return GetDefensiveTableList().Where(x => x._source == source).ToList();
        }
        public static List<Boon> GetDefensiveTableList(String source)
        {
            return GetDefensiveTableList(ProfToEnum(source));
        }
        // Table + graph
        public static List<Boon> GetTableProfList()
        {
            return _allBoons.Where(x => x._nature == BoonEnum.OffensiveBuffTable || x._nature == BoonEnum.DefensiveBuffTable).ToList();
        }
        private static List<Boon> GetTableProfList(BoonSource source)
        {
            return GetTableProfList().Where(x => x._source == source).ToList();
        }
        public static List<Boon> GetTableProfList(String source)
        {
            return GetTableProfList(ProfToEnum(source));
        }
        // Foods
        public static List<Boon> GetFoodList()
        {
            return _allBoons.Where(x => x._nature == BoonEnum.Food).ToList();
        }
        // Utilities
        public static List<Boon> GetUtilityList()
        {
            return _allBoons.Where(x => x._nature == BoonEnum.Utility).ToList();
        }
        // Boss
        public static List<Boon> GetBossBoonList()
        {
            return _allBoons.Where(x => x._source == BoonSource.Boss).ToList();
        }
        // All buffs
        public static List<Boon> GetAllBuffList()
        {
            List<Boon> res = new List<Boon>();
            // correct order for the boon graph
            res.AddRange(GetBoonList());
            res.AddRange(GetOffensiveTableList());
            res.AddRange(GetDefensiveTableList());
            res.AddRange(GetRemainingBuffsList());
            return res;
        }
        // Non shareable buffs
        public static List<Boon> GetRemainingBuffsList()
        {
            return _allBoons.Where(x => x._nature == BoonEnum.GraphOnlyBuff).ToList();
        }
        private static List<Boon> GetRemainingBuffsList(BoonSource source)
        {
            return GetRemainingBuffsList().Where(x => x._source == source).ToList();
        }
        public static List<Boon> GetRemainingBuffsList(String source)
        {
            return GetRemainingBuffsList(ProfToEnum(source));
        }


        // Getters
        public string GetName()
        {
            return _name;
        }
        public long GetID()
        {
            return _id;
        }
        public BoonSource GetSource()
        {
            return _source;
        }
        public BoonEnum GetNature()
        {
            return _nature;
        }

        public BoonType GetBoonType()
        {
            return _type;
        }

        public int GetCapacity()
        {
            return _capacity;
        }

        public string GetLink()
        {
            return _link;
        }

        public BoonSimulator CreateSimulator(ParsedLog log)
        {
            StackingLogic logicToUse;
            switch (_logic)
            {
                case Logic.Queue:
                    logicToUse = new QueueLogic();
                    break;
                case Logic.HealingPower:
                    logicToUse = new HealingLogic();
                    break;
                case Logic.ForceOverride:
                    logicToUse = new ForceOverrideLogic();
                    break;
                default:
                    logicToUse = new OverrideLogic();
                    break;
            }
            switch (_type)
            {
                case BoonType.Intensity: return new BoonSimulatorIntensity(_capacity, log, logicToUse);
                case BoonType.Duration: return new BoonSimulatorDuration(_capacity, log, logicToUse);
                default: throw new InvalidOperationException();
            }
        }
    }
}
