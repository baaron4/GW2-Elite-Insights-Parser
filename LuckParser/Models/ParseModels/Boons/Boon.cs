using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Boon
    {
        // Boon
        public enum BoonNature { Condition, Boon, OffensiveBuffTable, DefensiveBuffTable, GraphOnlyBuff, Consumable };
        public enum BoonSource { Mixed, Necromancer, Elementalist, Mesmer, Warrior, Revenant, Guardian, Thief, Ranger, Engineer, Item, Enemy };
        public enum BoonType { Duration, Intensity };
        private enum Logic { Queue, HealingPower, Override, ForceOverride };

        public const long NumberOfConditionsID = -3;
        public const long NumberOfBoonsID = -2;

        public static BoonSource ProfToEnum(string prof)
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
        public readonly string Name;
        public readonly long ID;
        public readonly BoonNature Nature;
        public readonly BoonSource Source;
        public readonly BoonType Type;
        public readonly int Capacity;
        public readonly string Link;
        private readonly Logic _logic;

        private Boon(string name, long id, BoonSource source, BoonType type, int capacity, BoonNature nature, Logic logic, string link)
        {
            Name = name;
            ID = id;
            Source = source;
            Type = type;
            Capacity = capacity;
            Nature = nature;
            Link = link;
            _logic = logic;
        }
        // Public Methods

        private static List<Boon> _allBoons = new List<Boon>
            {
                // Custom Boons
                new Boon("Number of Conditions", NumberOfConditionsID, BoonSource.Mixed, BoonType.Intensity, 0, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/38/Condition_Duration.png"),
                new Boon("Number of Boons", NumberOfBoonsID, BoonSource.Mixed, BoonType.Intensity, 0, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/44/Boon_Duration.png"),
                //Base boons
                new Boon("Might", 740, BoonSource.Mixed, BoonType.Intensity, 25, BoonNature.Boon, Logic.Override, "https://wiki.guildwars2.com/images/7/7c/Might.png"),
                new Boon("Fury", 725, BoonSource.Mixed, BoonType.Duration, 9, BoonNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/4/46/Fury.png"),//or 3m and 30s
                new Boon("Quickness", 1187, BoonSource.Mixed, BoonType.Duration, 5, BoonNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/b/b4/Quickness.png"),
                new Boon("Alacrity", 30328, BoonSource.Mixed, BoonType.Duration, 9, BoonNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/4/4c/Alacrity.png"),
                new Boon("Protection", 717, BoonSource.Mixed, BoonType.Duration, 5, BoonNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/6/6c/Protection.png"),
                new Boon("Regeneration", 718, BoonSource.Mixed, BoonType.Duration, 5, BoonNature.Boon, Logic.HealingPower, "https://wiki.guildwars2.com/images/5/53/Regeneration.png"),
                new Boon("Vigor", 726, BoonSource.Mixed, BoonType.Duration, 5, BoonNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/f/f4/Vigor.png"),
                new Boon("Aegis", 743, BoonSource.Mixed, BoonType.Duration, 9, BoonNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/e/e5/Aegis.png"),
                new Boon("Stability", 1122, BoonSource.Mixed, BoonType.Intensity, 25, BoonNature.Boon, Logic.Override, "https://wiki.guildwars2.com/images/a/ae/Stability.png"),
                new Boon("Swiftness", 719, BoonSource.Mixed, BoonType.Duration, 9, BoonNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/a/af/Swiftness.png"),
                new Boon("Retaliation", 873, BoonSource.Mixed, BoonType.Duration, 5, BoonNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/5/53/Retaliation.png"),
                new Boon("Resistance", 26980, BoonSource.Mixed, BoonType.Duration, 5, BoonNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/4/4b/Resistance.png"),
                // Condis         
                new Boon("Bleeding", 736, BoonSource.Mixed, BoonType.Intensity, 1500, BoonNature.Condition, Logic.Override, "https://wiki.guildwars2.com/images/3/33/Bleeding.png"),
                new Boon("Burning", 737, BoonSource.Mixed, BoonType.Intensity, 1500, BoonNature.Condition, Logic.Override, "https://wiki.guildwars2.com/images/4/45/Burning.png"),
                new Boon("Confusion", 861, BoonSource.Mixed, BoonType.Intensity, 1500, BoonNature.Condition, Logic.Override, "https://wiki.guildwars2.com/images/e/e6/Confusion.png"),
                new Boon("Poison", 723, BoonSource.Mixed, BoonType.Intensity, 1500, BoonNature.Condition, Logic.Override, "https://wiki.guildwars2.com/images/1/11/Poisoned.png"),
                new Boon("Torment", 19426, BoonSource.Mixed, BoonType.Intensity, 1500, BoonNature.Condition, Logic.Override, "https://wiki.guildwars2.com/images/0/08/Torment.png"),
                new Boon("Blind", 720, BoonSource.Mixed, BoonType.Duration, 9, BoonNature.Condition, Logic.Queue, "https://wiki.guildwars2.com/images/3/33/Blinded.png"),
                new Boon("Chilled", 722, BoonSource.Mixed, BoonType.Duration, 5, BoonNature.Condition, Logic.Queue, "https://wiki.guildwars2.com/images/a/a6/Chilled.png"),
                new Boon("Crippled", 721, BoonSource.Mixed, BoonType.Duration, 9, BoonNature.Condition, Logic.Queue, "https://wiki.guildwars2.com/images/f/fb/Crippled.png"),
                new Boon("Fear", 791, BoonSource.Mixed, BoonType.Duration, 9, BoonNature.Condition, Logic.Queue, "https://wiki.guildwars2.com/images/e/e6/Fear.png"),
                new Boon("Immobile", 727, BoonSource.Mixed, BoonType.Duration, 3, BoonNature.Condition, Logic.Queue, "https://wiki.guildwars2.com/images/3/32/Immobile.png"),
                new Boon("Slow", 26766, BoonSource.Mixed, BoonType.Duration, 9, BoonNature.Condition, Logic.Queue, "https://wiki.guildwars2.com/images/f/f5/Slow.png"),
                new Boon("Weakness", 742, BoonSource.Mixed, BoonType.Duration, 5, BoonNature.Condition, Logic.Queue, "https://wiki.guildwars2.com/images/f/f9/Weakness.png"),
                new Boon("Taunt", 27705, BoonSource.Mixed, BoonType.Duration, 5, BoonNature.Condition, Logic.Queue, "https://wiki.guildwars2.com/images/c/cc/Taunt.png"),
                new Boon("Vulnerability", 738, BoonSource.Mixed, BoonType.Intensity, 25, BoonNature.Condition, Logic.Override, "https://wiki.guildwars2.com/images/a/af/Vulnerability.png"),
                // Generic
                new Boon("Stealth", 13017, BoonSource.Mixed, BoonType.Duration, 5, BoonNature.GraphOnlyBuff, Logic.Queue, "https://wiki.guildwars2.com/images/1/19/Stealth.png"),
                new Boon("Revealed", 890, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/db/Revealed.png"),
                new Boon("Superspeed", 5974, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.ForceOverride,"https://wiki.guildwars2.com/images/1/1a/Super_Speed.png"),
                new Boon("Determined", 762, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Boon("Determined", 788, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Boon("Determined", 895, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Boon("Determined", 3892, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Boon("Determined", 31450, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Boon("Determined", 52271, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Boon("Invulnerability", 757, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Boon("Invulnerability", 801, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                // Fractals 
                new Boon("Rigorous Certainty", 33652, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.ForceOverride,"https://wiki.guildwars2.com/images/6/60/Desert_Carapace.png"),
                new Boon("Fractal Mobility", 33024, BoonSource.Mixed, BoonType.Intensity, 5, BoonNature.Consumable, Logic.ForceOverride,"https://wiki.guildwars2.com/images/thumb/2/22/Mist_Mobility_Potion.png/40px-Mist_Mobility_Potion.png"),
                new Boon("Fractal Defensive", 32134, BoonSource.Mixed, BoonType.Intensity, 5, BoonNature.Consumable, Logic.ForceOverride,"https://wiki.guildwars2.com/images/thumb/e/e6/Mist_Defensive_Potion.png/40px-Mist_Defensive_Potion.png"),
                new Boon("Fractal Offensive", 32473, BoonSource.Mixed, BoonType.Intensity, 5, BoonNature.Consumable, Logic.ForceOverride,"https://wiki.guildwars2.com/images/thumb/8/8d/Mist_Offensive_Potion.png/40px-Mist_Offensive_Potion.png"),
                // Sigils and Runes
                new Boon("Sigil of Concentration", 33719, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b3/Superior_Sigil_of_Concentration.png"),
                new Boon("Superior Rune of the Monk", 53285, BoonSource.Mixed, BoonType.Intensity, 10, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/18/Superior_Rune_of_the_Monk.png"),
                new Boon("Sigil of Corruption", 9374, BoonSource.Mixed, BoonType.Intensity, 25, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/18/Superior_Sigil_of_Corruption.png"),
                new Boon("Sigil of Life", 9386, BoonSource.Mixed, BoonType.Intensity, 25, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a7/Superior_Sigil_of_Life.png"),
                new Boon("Sigil of Perception", 9385, BoonSource.Mixed, BoonType.Intensity, 25, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/cc/Superior_Sigil_of_Perception.png"),
                new Boon("Sigil of Bloolust", 9286, BoonSource.Mixed, BoonType.Intensity, 25, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/fb/Superior_Sigil_of_Bloodlust.png"),
                new Boon("Sigil of Bounty", 38588, BoonSource.Mixed, BoonType.Intensity, 25, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/f8/Superior_Sigil_of_Bounty.png"),
                new Boon("Sigil of Benevolence", 9398, BoonSource.Mixed, BoonType.Intensity, 25, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/59/Superior_Sigil_of_Benevolence.png"),
                new Boon("Sigil of Momentum", 22144, BoonSource.Mixed, BoonType.Intensity, 25, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/30/Superior_Sigil_of_Momentum.png"),
                new Boon("Sigil of the Stars", 46953, BoonSource.Mixed, BoonType.Intensity, 25, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/dc/Superior_Sigil_of_the_Stars.png"),
                //Auras
                new Boon("Chaos Armor", 10332, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/1/1b/Chaos_Armor.png"),
                new Boon("Fire Shield", 5677, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/1/18/Fire_Shield.png"),
                new Boon("Frost Aura", 5579, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/6/68/Frost_Aura.png"),
                new Boon("Light Aura", 25518, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/5/5a/Light_Aura.png"),
                new Boon("Magnetic Aura", 5684, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/5a/Magnetic_Aura.png"),
                new Boon("Shocking Aura", 5577, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/3/31/Shocking_Aura.png"),
                //race
                new Boon("Take Root", 12459, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/b/b2/Take_Root.png"),
                new Boon("Become the Bear",12426, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/7/7e/Become_the_Bear.png"),
                new Boon("Become the Raven",12405, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/2/2c/Become_the_Raven.png"),
                new Boon("Become the Snow Leopard",12400, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/7/78/Become_the_Snow_Leopard.png"),
                new Boon("Become the Wolf",12393, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/f1/Become_the_Wolf.png"),
                new Boon("Avatar of Melandru", 12368, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/30/Avatar_of_Melandru.png"),
                new Boon("Power Suit",12326, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/89/Summon_Power_Suit.png"),
                new Boon("Reaper of Grenth", 12366, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/07/Reaper_of_Grenth.png"),
                new Boon("Charrzooka",43503, BoonSource.Mixed, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/1/17/Charrzooka.png"),
                // ENEMY
                new Boon("Unnatural Signet",38224, BoonSource.Enemy, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/20/Unnatural_Signet.png"),
                new Boon("Compromised",35096, BoonSource.Enemy, BoonType.Intensity, 5, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/48/Compromised.png"),
                new Boon("Spirited Fusion",31722, BoonSource.Enemy, BoonType.Intensity, 500, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/eb/Spirited_Fusion.png"),
                new Boon("Blood Shield",34376, BoonSource.Enemy, BoonType.Intensity, 18, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a6/Blood_Shield.png"),
                new Boon("Blood Shield",34518, BoonSource.Enemy, BoonType.Intensity, 18, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a6/Blood_Shield.png"),
                new Boon("Blood Fueled",34422, BoonSource.Enemy, BoonType.Intensity, 20, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("Blood Fueled",34428, BoonSource.Enemy, BoonType.Intensity, 20, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("Flame Armor",52568, BoonSource.Enemy, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("Fiery Surge",52588, BoonSource.Enemy, BoonType.Intensity, 99, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("Fractured - Enemy",53030, BoonSource.Enemy, BoonType.Intensity, 10, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("Fractured - Allied",52213, BoonSource.Enemy, BoonType.Intensity, 10, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("Conjured Protection",52973 , BoonSource.Enemy, BoonType.Intensity, 25, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("Conjured Shield",52754 , BoonSource.Enemy, BoonType.Intensity, 10, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("Greatsword Power",52667 , BoonSource.Enemy, BoonType.Intensity, 10, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("Conjured Barrier",53003 , BoonSource.Enemy, BoonType.Intensity, 10, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("Scepter Lock-on",53075  , BoonSource.Enemy, BoonType.Intensity, 4, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("Augmented Power",52074  , BoonSource.Enemy, BoonType.Intensity, 10, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("CA Invul",52255 , BoonSource.Enemy, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("Arm Up",52430 , BoonSource.Enemy, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("Aquatic Detainment",52931 , BoonSource.Enemy, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("Aquatic Aura (Kenut)",52211 , BoonSource.Enemy, BoonType.Intensity, 80, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("Aquatic Aura (Nikare)",52929 , BoonSource.Enemy, BoonType.Intensity, 80, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("Waterlogged",51935 , BoonSource.Enemy, BoonType.Intensity, 10, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Boon("Protective Shadow", 31877, BoonSource.Enemy, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/8/87/Protective_Shadow.png"),
                new Boon("Narcolepsy", 34467, BoonSource.Enemy, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Boon("Blue Pylon Power", 31413, BoonSource.Enemy, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/6/6e/Blue_Pylon_Power.png"),
                new Boon("Unbreakable", 34979, BoonSource.Enemy, BoonType.Intensity, 2, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/5/56/Xera%27s_Embrace.png"),
                new Boon("Not the Bees!", 34434, BoonSource.Enemy, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/0/08/Throw_Jar.png"),
                new Boon("Targeted", 34392, BoonSource.Enemy, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                //REVENANT
                //skills
                new Boon("Crystal Hibernation", 28262, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/4/4a/Crystal_Hibernation.png"),
                new Boon("Vengeful Hammers", 27273, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/c/c8/Vengeful_Hammers.png"),
                new Boon("Rite of the Great Dwarf", 26596, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/6/69/Rite_of_the_Great_Dwarf.png"),
                new Boon("Embrace the Darkness", 28001, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/51/Embrace_the_Darkness.png"),
                new Boon("Enchanted Daggers", 28557, BoonSource.Revenant, BoonType.Intensity, 6, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/fa/Enchanted_Daggers.png"),
                new Boon("Phase Traversal", 28395, BoonSource.Revenant, BoonType.Intensity, 2, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/f2/Phase_Traversal.png"),
                new Boon("Impossible Odds", 27581, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/87/Impossible_Odds.png"),
                //facets
                new Boon("Facet of Light",27336, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/18/Facet_of_Light.png"),
                new Boon("Facet of Light (Traited)",51690, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/18/Facet_of_Light.png"), //Lingering buff with Draconic Echo trait
                new Boon("Infuse Light",27737, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/6/60/Infuse_Light.png"),
                new Boon("Facet of Darkness",28036, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/e4/Facet_of_Darkness.png"),
                new Boon("Facet of Darkness (Traited)",51695, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/e4/Facet_of_Darkness.png"),//Lingering buff with Draconic Echo trait
                new Boon("Facet of Elements",28243, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/ce/Facet_of_Elements.png"),
                new Boon("Facet of Elements (Traited)",51706, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/ce/Facet_of_Elements.png"),//Lingering buff with Draconic Echo trait
                new Boon("Facet of Strength",27376, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a8/Facet_of_Strength.png"),
                new Boon("Facet of Strength (Traited)",51700, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a8/Facet_of_Strength.png"),//Lingering buff with Draconic Echo trait
                new Boon("Facet of Chaos",27983, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/c7/Facet_of_Chaos.png"),
                new Boon("Facet of Nature",29275, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),
                new Boon("Facet of Nature (Traited)",51681, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),//Lingering buff with Draconic Echo trait
                new Boon("Facet of Nature-Assassin",51692, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/c/cd/Facet_of_Nature%E2%80%95Assassin.png"),
                new Boon("Facet of Nature-Dragon",51674, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/4/47/Facet_of_Nature%E2%80%95Dragon.png"),
                new Boon("Facet of Nature-Demon",51704, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/f/ff/Facet_of_Nature%E2%80%95Demon.png"),
                new Boon("Facet of Nature-Dwarf",51677, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/4/4c/Facet_of_Nature%E2%80%95Dwarf.png"),
                new Boon("Facet of Nature-Centaur",51699, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/7/74/Facet_of_Nature%E2%80%95Centaur.png"),
                new Boon("Naturalistic Resonance", 29379, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),
                //legends
                new Boon("Legendary Centaur Stance",27972, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8a/Legendary_Centaur_Stance.png"),
                new Boon("Legendary Dragon Stance",27732, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d5/Legendary_Dragon_Stance.png"),
                new Boon("Legendary Dwarf Stance",27205, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b2/Legendary_Dwarf_Stance.png"),
                new Boon("Legendary Demon Stance",27928, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d1/Legendary_Demon_Stance.png"),
                new Boon("Legendary Assassin Stance",27890, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/02/Legendary_Assassin_Stance.png"),
                new Boon("Legendary Renegade Stance",44272, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/19/Legendary_Renegade_Stance.png"),
                //summons
                new Boon("Breakrazor's Bastion",44682, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/a/a7/Breakrazor%27s_Bastion.png"),
                new Boon("Razorclaw's Rage",41016, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/73/Razorclaw%27s_Rage.png"),
                new Boon("Soulcleave's Summit",45026, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Soulcleave%27s_Summit.png"),
                //traits
                new Boon("Vicious Lacerations",29395, BoonSource.Revenant, BoonType.Intensity, 3, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/cd/Vicious_Lacerations.png"),
                new Boon("Rising Momentum",51683, BoonSource.Revenant, BoonType.Intensity, 10, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8c/Rising_Momentum.png"),
                new Boon("Assassin's Presence", 26854, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/5/54/Assassin%27s_Presence.png"),
                //new Boon("Expose Defenses", 48894, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Invoking Harmony",29025, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/ec/Invoking_Harmony.png"),
                //new Boon("Selfless Amplification",30509, BoonSource.Revenant, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Hardening Persistence",28957, BoonSource.Revenant, BoonType.Intensity, 10, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/0f/Hardening_Persistence.png"),
                new Boon("Soothing Bastion",34136, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/04/Soothing_Bastion.png"),
                new Boon("Kalla's Fervor",42883, BoonSource.Revenant, BoonType.Intensity, 5, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png"),
                new Boon("Improved Kalla's Fervor",45614, BoonSource.Revenant, BoonType.Intensity, 5, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png"),
                //WARRIOR
                //skills
                new Boon("Riposte",14434, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/d/de/Riposte.png"),
                new Boon("Flames of War", 31708, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/6f/Flames_of_War_%28warrior_skill%29.png"),
                new Boon("Blood Reckoning", 29466 , BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d6/Blood_Reckoning.png"),
                new Boon("Rock Guard", 34256 , BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/c7/Shattering_Blow.png"),
                new Boon("Sight beyond Sight",40616, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/d/d7/Sight_beyond_Sight.png"),
                //signets
                new Boon("Healing Signet",786, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/8/85/Healing_Signet.png"),
                new Boon("Dolyak Signet",14458, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/60/Dolyak_Signet.png"),
                new Boon("Signet of Fury",14459, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/c1/Signet_of_Fury.png"),
                new Boon("Signet of Might",14444, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/40/Signet_of_Might.png"),
                new Boon("Signet of Stamina",14478, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/6b/Signet_of_Stamina.png"),
                new Boon("Signet of Rage",14496, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/bc/Signet_of_Rage.png"),
                //banners
                new Boon("Banner of Strength", 14417, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/e/e1/Banner_of_Strength.png/33px-Banner_of_Strength.png"),
                new Boon("Banner of Discipline", 14449, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/5/5f/Banner_of_Discipline.png/33px-Banner_of_Discipline.png"),
                new Boon("Banner of Tactics",14450, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/3/3f/Banner_of_Tactics.png/33px-Banner_of_Tactics.png"),
                new Boon("Banner of Defense",14543, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/f/f1/Banner_of_Defense.png/33px-Banner_of_Defense.png"),
                //stances
                new Boon("Shield Stance",756, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/d/de/Shield_Stance.png"),
                new Boon("Berserker's Stance",14453, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/8/8a/Berserker_Stance.png"),
                new Boon("Enduring Pain",787, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/24/Endure_Pain.png"),
                new Boon("Balanced Stance",34778, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/27/Balanced_Stance.png"),
                new Boon("Defiant Stance",21816, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/db/Defiant_Stance.png"),
                new Boon("Rampage",14484, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e4/Rampage.png"),
                //traits
                new Boon("Empower Allies", 14222, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/4/4c/Empower_Allies.png/20px-Empower_Allies.png"),
                new Boon("Peak Performance",46853, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/98/Peak_Performance.png"),
                new Boon("Furious Surge", 30204, BoonSource.Warrior, BoonType.Intensity, 25, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/65/Furious.png"),
                //new Boon("Health Gain per Adrenaline bar Spent",-1, BoonSource.Warrior, BoonType.Intensity, 3, BoonEnum.GraphOnlyBuff,RemoveType.Normal, Logic.Override),
                new Boon("Rousing Resilience",24383, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/ca/Rousing_Resilience.png"),
                new Boon("Always Angry",34099, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/63/Always_Angry.png"),
                new Boon("Full Counter",43949, BoonSource.Warrior, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/fb/Full_Counter.png"),
                new Boon("Attacker's Insight",41963, BoonSource.Warrior, BoonType.Intensity, 5, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/89/Attacker%27s_Insight.png"),
                // GUARDIAN
                //skills
                new Boon("Zealot's Flame", 9103, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/7a/Zealot%27s_Flame.png"),
                new Boon("Purging Flames",21672, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/28/Purging_Flames.png"),
                new Boon("Litany of Wrath",21665, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/4a/Litany_of_Wrath.png"),
                new Boon("Renewed Focus",9255, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/10/Renewed_Focus.png"),
                new Boon("Ashes of the Just",41957, BoonSource.Guardian, BoonType.Intensity, 25, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/6d/Epilogue-_Ashes_of_the_Just.png"),
                new Boon("Eternal Oasis",44871, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/5/5f/Epilogue-_Eternal_Oasis.png"),
                new Boon("Unbroken Lines",43194, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/d/d8/Epilogue-_Unbroken_Lines.png"),
                //signets
                new Boon("Signet of Resolve",9220, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/75/Signet_of_Resolve.png"),
                new Boon("Bane Signet",9092, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/56/Bane_Signet.png"),
                new Boon("Bane Signet",9240, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/56/Bane_Signet.png"),
                new Boon("Signet of Judgment",9156, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/fe/Signet_of_Judgment.png"),
                new Boon("Signet of Judgment",9239, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/fe/Signet_of_Judgment.png"),
                new Boon("Signet of Mercy",9162, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/37/Signet_of_Mercy.png"),
                new Boon("Signet of Mercy",9238, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/37/Signet_of_Mercy.png"),
                new Boon("Signet of Wrath",9100, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/18/Signet_of_Wrath.png"),
                new Boon("Signet of Wrath",9237, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/18/Signet_of_Wrath.png"),
                new Boon("Signet of Courage",29633, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/5d/Signet_of_Courage.png"),
                //virtues
                new Boon("Virtue of Justice", 9114, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/10/Virtue_of_Justice.png"),
                new Boon("Spear of Justice", 29632, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/f1/Spear_of_Justice.png"),
                new Boon("Virtue of Courage", 9113, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a9/Virtue_of_Courage.png"),
                new Boon("Shield of Courage", 29523, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/63/Shield_of_Courage.png"),
                new Boon("Virtue of Resolve", 9119, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b2/Virtue_of_Resolve.png"),
                new Boon("Wings of Resolve", 30308, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/cb/Wings_of_Resolve.png"),
                new Boon("Tome of Justice",40530, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/ae/Tome_of_Justice.png"),
                new Boon("Tome of Courage",43508,BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/54/Tome_of_Courage.png"),
                new Boon("Tome of Resolve",46298, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a9/Tome_of_Resolve.png"),
                //traits
                new Boon("Strength in Numbers",13796, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/7/7b/Strength_in_Numbers.png"),
                new Boon("Invigorated Bulwark",30207, BoonSource.Guardian, BoonType.Intensity, 5, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/00/Invigorated_Bulwark.png"),
                new Boon("Battle Presence", 17046, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/2/27/Battle_Presence.png"),
                //new Boon("Force of Will",29485, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),//not sure if intensity
                new Boon("Quickfire",45123, BoonSource.Guardian, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d6/Quickfire.png"),
                //ENGINEER
                //skills
                new Boon("Static Shield",6055, BoonSource.Engineer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/90/Static_Shield.png"),
                new Boon("Absorb",6056, BoonSource.Engineer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8a/Absorb.png"),
                new Boon("A.E.D.",21660, BoonSource.Engineer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e6/A.E.D..png"),
                new Boon("Elixir S",5863, BoonSource.Engineer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d8/Elixir_S.png"),
                //new Boon("Elixir X", -1,BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Normal, Logic.Override),
                new Boon("Utility Goggles",5864, BoonSource.Engineer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/02/Utility_Goggles.png"),
                new Boon("Slick Shoes",5833, BoonSource.Engineer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/3d/Slick_Shoes.png"),
                //new Boon("Watchful Eye",-1, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Normal, Logic.Override),
                new Boon("Cooling Vapor",46444, BoonSource.Engineer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b1/Coolant_Blast.png"),
                new Boon("Photon Wall Deployed",46094, BoonSource.Engineer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/ea/Photon_Wall.png"),
                new Boon("Spectrum Shield",43066, BoonSource.Engineer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/29/Spectrum_Shield.png"),
                new Boon("Gear Shield",5997, BoonSource.Engineer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/ca/Gear_Shield.png"),
                //Transforms
                //new Boon("Rampage",-1, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Normal, Logic.Override),
                new Boon("Photon Forge",43708, BoonSource.Engineer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/dd/Engage_Photon_Forge.png"),
                //Traits
                new Boon("Laser's Edge",44414, BoonSource.Engineer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/5d/Laser%27s_Edge.png"),
                new Boon("Afterburner",42210, BoonSource.Engineer, BoonType.Intensity, 5, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/51/Solar_Focusing_Lens.png"),
                new Boon("Iron Blooded",49065, BoonSource.Engineer, BoonType.Intensity, 25, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/1e/Iron_Blooded.png"),
                new Boon("Streamlined Kits",18687, BoonSource.Engineer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/cb/Streamlined_Kits.png"),
                new Boon("Kinetic Charge",45781, BoonSource.Engineer, BoonType.Intensity, 5, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e0/Kinetic_Battery.png"),
                new Boon("Pinpoint Distribution", 38333, BoonSource.Engineer, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/b/bf/Pinpoint_Distribution.png"),
                new Boon("Heat Therapy",40694, BoonSource.Engineer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/34/Heat_Therapy.png"),
                new Boon("Overheat", 40397, BoonSource.Engineer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/4b/Overheat.png"),
                new Boon("Thermal Vision", 51389, BoonSource.Engineer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8a/Skilled_Marksman.png"),
                //RANGER
                new Boon("Celestial Avatar", 31508, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/59/Celestial_Avatar.png"),
                new Boon("Counterattack",14509, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/c1/Counterattack.png"),
                //signets
                new Boon("Signet of Renewal",41147, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/11/Signet_of_Renewal.png"),
                new Boon("Signet of Stone (Passive)",12627, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/63/Signet_of_Stone.png"),
                new Boon("Signet of the Hunt (Passive)",12626, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/df/Signet_of_the_Hunt.png"),
                new Boon("Signet of the Wild",12636, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/23/Signet_of_the_Wild.png"),
                new Boon("Signet of Stone (Active)",12543, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/63/Signet_of_Stone.png"),
                new Boon("Signet of the Hunt (Active)",12541, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/df/Signet_of_the_Hunt.png"),
                //spirits
                // new Boon("Water Spirit", 50386, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/0/06/Water_Spirit.png/33px-Water_Spirit.png"),
                new Boon("Frost Spirit", 12544, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png"),
                new Boon("Sun Spirit", 12540, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/d/dd/Sun_Spirit.png/33px-Sun_Spirit.png"),
                new Boon("Stone Spirit", 12547, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/3/35/Stone_Spirit.png/20px-Stone_Spirit.png"),
                //new Boon("Storm Spirit", 50381, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/2/25/Storm_Spirit.png/30px-Storm_Spirit.png"),
                //reworked
                new Boon("Water Spirit", 50386, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/0/06/Water_Spirit.png/33px-Water_Spirit.png"),
                new Boon("Frost Spirit", 50421, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png"),
                new Boon("Sun Spirit", 50413, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/d/dd/Sun_Spirit.png/33px-Sun_Spirit.png"),
                new Boon("Stone Spirit", 50415, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/3/35/Stone_Spirit.png/20px-Stone_Spirit.png"),
                new Boon("Storm Spirit", 50381, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/2/25/Storm_Spirit.png/30px-Storm_Spirit.png"),
                //skills
                new Boon("Attack of Opportunity",12574, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/47/Moment_of_Clarity.png"),
                new Boon("Call of the Wild",36781, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8d/Call_of_the_Wild.png"),
                new Boon("Strength of the pack!",12554, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/4b/%22Strength_of_the_Pack%21%22.png"),
                new Boon("Sic 'Em!",33902, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png"),
                new Boon("Sharpening Stones",12536, BoonSource.Ranger, BoonType.Intensity, 10, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/af/Sharpening_Stone.png"),
                new Boon("Ancestral Grace", 31584, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/4b/Ancestral_Grace.png"),
                new Boon("Glyph of Empowerment", 31803, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/f/f0/Glyph_of_Empowerment.png/33px-Glyph_of_Empowerment.png"),
                new Boon("Dolyak Stance",41815, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/7/71/Dolyak_Stance.png"),
                new Boon("Griffon Stance",46280, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/9/98/Griffon_Stance.png"),
                new Boon("Moa Stance",45038, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/6/66/Moa_Stance.png"),
                new Boon("Vulture Stance",44651, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/8/8f/Vulture_Stance.png"),
                new Boon("Bear Stance",40045, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/f/f0/Bear_Stance.png"),
                new Boon("One Wolf Pack",44139, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/3/3b/One_Wolf_Pack.png"),
                new Boon("Sharpen Spines",43266, BoonSource.Ranger, BoonType.Intensity, 5, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/95/Sharpen_Spines.png"),
                //traits
                new Boon("Spotter", 14055, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/b/b0/Spotter.png"),
                new Boon("Opening Strike",13988, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/44/Opening_Strike_%28effect%29.png"),
                new Boon("Quick Draw",29703, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/39/Quick_Draw.png"),
                new Boon("Light on your feet",30673, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/22/Light_on_your_Feet.png"),
                new Boon("Natural Mender",30449, BoonSource.Ranger, BoonType.Intensity, 10, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e9/Natural_Mender.png"),
                new Boon("Lingering Light",32248, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/5d/Lingering_Light.png"),
                new Boon("Deadly",44932, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/94/Deadly_%28Archetype%29.png"),
                new Boon("Ferocious",41720, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e9/Ferocious_%28Archetype%29.png"),
                new Boon("Supportive",40069, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/32/Supportive_%28Archetype%29.png"),
                new Boon("Versatile",44693, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/bb/Versatile_%28Archetype%29.png"),
                new Boon("Stout",40272, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/80/Stout_%28Archetype%29.png"),
                new Boon("Unstoppable Union",44439, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b2/Unstoppable_Union.png"),
                new Boon("Twice as Vicious",45600, BoonSource.Ranger, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png"),
                //THIEF
                //signets
                new Boon("Signet of Malice",13049, BoonSource.Thief, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/ae/Signet_of_Malice.png"),
                new Boon("Assassin's Signet (Passive)",13047, BoonSource.Thief, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/23/Assassin%27s_Signet.png"),
                new Boon("Assassin's Signet (Active)",44597, BoonSource.Thief, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/23/Assassin%27s_Signet.png"),
                new Boon("Infiltrator's Signet",13063, BoonSource.Thief, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8e/Infiltrator%27s_Signet.png"),
                new Boon("Signet of Agility",13061, BoonSource.Thief, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/1d/Signet_of_Agility.png"),
                new Boon("Signet of Shadows",13059, BoonSource.Thief, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/17/Signet_of_Shadows.png"),
                //venoms
                new Boon("Skelk Venom",21780, BoonSource.Thief, BoonType.Intensity, 5, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/75/Skelk_Venom.png"),
                new Boon("Ice Drake Venom",13095, BoonSource.Thief, BoonType.Intensity, 4, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/7b/Ice_Drake_Venom.png"),
                new Boon("Devourer Venom", 13094, BoonSource.Thief, BoonType.Intensity, 2, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/4d/Devourer_Venom.png"),
                new Boon("Skale Venom", 13054, BoonSource.Thief, BoonType.Intensity, 4, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/14/Skale_Venom.png"),
                new Boon("Spider Venom",13036, BoonSource.Thief, BoonType.Intensity, 6, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/39/Spider_Venom.png"),
                new Boon("Basilisk Venom", 13133, BoonSource.Thief, BoonType.Intensity, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/3a/Basilisk_Venom.png"),
                //physical
                new Boon("Palm Strike",30423, BoonSource.Thief, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
                new Boon("Pulmonary Impact",30510, BoonSource.Thief, BoonType.Intensity, 2, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
                //weapon
                new Boon("Infiltration",13135, BoonSource.Thief, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/6/62/Infiltrator%27s_Return.png"),
                //transforms
                new Boon("Dagger Storm",13134, BoonSource.Thief, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/c0/Dagger_Storm.png"),
                new Boon("Kneeling",42869, BoonSource.Thief, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/56/Kneel.png"),
                //traits
                //new Boon("Deadeye's Gaze",46333, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                //new Boon("Maleficent Seven",43606, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Boon("Hidden Killer",42720, BoonSource.Thief, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/ec/Hidden_Killer.png"),
                new Boon("Lead Attacks",34659, BoonSource.Thief, BoonType.Intensity, 15, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/01/Lead_Attacks.png"),
                new Boon("Instant Reflexes",34283, BoonSource.Thief, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/7d/Instant_Reflexes.png"),
                new Boon("Lotus Training", 32200, BoonSource.Thief, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/ea/Lotus_Training.png"),
                new Boon("Unhindered Combatant", 32931, BoonSource.Thief, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a1/Unhindered_Combatant.png"),
                new Boon("Bounding Dodger", 33162, BoonSource.Thief, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/30/Bounding_Dodger.png"),
                //MESMER
                //signets
                new Boon("Signet of the Ether", 21751, BoonSource.Mesmer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/7a/Signet_of_the_Ether.png"),
                new Boon("Signet of Domination",10231, BoonSource.Mesmer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/3b/Signet_of_Domination.png"),
                new Boon("Signet of Illusions",10246, BoonSource.Mesmer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/ce/Signet_of_Illusions.png"),
                new Boon("Signet of Inspiration",10235, BoonSource.Mesmer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/ed/Signet_of_Inspiration.png"),
                new Boon("Signet of Midnight",10233, BoonSource.Mesmer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/24/Signet_of_Midnight.png"),
                new Boon("Signet of Humility",30739, BoonSource.Mesmer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b5/Signet_of_Humility.png"),
                //skills
                new Boon("Distortion",10243, BoonSource.Mesmer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/22/Distortion.png"),
                new Boon("Blur", 10335 , BoonSource.Mesmer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/22/Distortion.png"),
                new Boon("Mirror",10357, BoonSource.Mesmer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b8/Mirror.png"),
                new Boon("Echo",29664, BoonSource.Mesmer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/ce/Echo.png"),
                //new Boon("Illusion of Life",-1, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Normal, Logic.Override),
                //new Boon("Time Block",30134, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff), What is this?
                new Boon("Time Echo",29582, BoonSource.Mesmer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8d/Deja_Vu.png"),
                new Boon("Illusionary Counter",10278, BoonSource.Mesmer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e5/Illusionary_Counter.png"),
                new Boon("Time Anchored",30136, BoonSource.Mesmer, BoonType.Duration, 3, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/db/Continuum_Split.png"),
                //traits
                new Boon("Fencer's Finesse", 30426 , BoonSource.Mesmer, BoonType.Intensity, 10, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e7/Fencer%27s_Finesse.png"),
                new Boon("Illusionary Defense",49099, BoonSource.Mesmer, BoonType.Intensity, 5, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e0/Illusionary_Defense.png"),
                new Boon("Compounding Power",49058, BoonSource.Mesmer, BoonType.Intensity, 5, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e5/Compounding_Power.png"),
                new Boon("Phantasmal Force", 44691 , BoonSource.Mesmer, BoonType.Intensity, 25, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/5f/Mistrust.png"),
                new Boon("Mirage Cloak",40408, BoonSource.Mesmer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a5/Mirage_Cloak_%28effect%29.png"),
                //NECROMANCER
                //forms
                new Boon("Lich Form",10631, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/ab/Lich_Form.png"),
                new Boon("Death Shroud", 790, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/f5/Death_Shroud.png"),
                new Boon("Reaper's Shroud", 29446, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/11/Reaper%27s_Shroud.png"),
                //signets
                new Boon("Signet of Vampirism (Passive)",21761, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/73/Signet_of_Vampirism.png"),
                new Boon("Signet of Vampirism (Active)",21765, BoonSource.Necromancer, BoonType.Intensity, 25, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/73/Signet_of_Vampirism.png"),
                new Boon("Lesser Signet of Vampirism",29799, BoonSource.Necromancer, BoonType.Intensity, 5, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/73/Signet_of_Vampirism.png"),
                new Boon("Signet of Vampirism (Shroud)",43885, BoonSource.Necromancer, BoonType.Intensity, 25, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/73/Signet_of_Vampirism.png"),
                new Boon("Plague Signet (Passive)",10630, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/c5/Plague_Signet.png"),
                new Boon("Plague Signet (Shroud)",44164, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/c5/Plague_Signet.png"),
                new Boon("Signet of Spite (Passive)",10621, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/df/Signet_of_Spite.png"),
                new Boon("Signet of Spite (Shroud)",43772, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/df/Signet_of_Spite.png"),
                new Boon("Signet of the Locust (Passive)",10614, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a3/Signet_of_the_Locust.png"),
                new Boon("Signet of the Locust (Shroud)",40283, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a3/Signet_of_the_Locust.png"),
                new Boon("Signet of Undeath (Passive)",10610, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/9c/Signet_of_Undeath.png"),
                new Boon("Signet of Undeath (Shroud)",40583, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/9c/Signet_of_Undeath.png"),
                //skills
                new Boon("Spectral Walk",15083, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/33/Spectral_Walk.png"),
                new Boon("Infusing Terror", 30129, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/db/Infusing_Terror.png"),
                //traits
                new Boon("Corrupter's Defense",30845, BoonSource.Necromancer, BoonType.Intensity, 10, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/11/Corrupter%27s_Fervor.png"),
                new Boon("Vampiric Aura", 30285, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/d/da/Vampiric_Presence.png"),
                new Boon("Last Rites",29726, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/1/1a/Last_Rites_%28effect%29.png"),
                new Boon("Sadistic Searing",43626, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/dd/Sadistic_Searing.png"),
                //ELEMENTALIST
                //signets
                new Boon("Signet of Restoration",739, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/dd/Signet_of_Restoration.png"),
                new Boon("Signet of Air",5590, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/92/Signet_of_Air.png"),
                new Boon("Signet of Earth",5592, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/ce/Signet_of_Earth.png"),
                new Boon("Signet of Fire",5544, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b0/Signet_of_Fire.png"),
                new Boon("Signet of Water",5591, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/fd/Signet_of_Water.png"),
                //attunements
                new Boon("Fire Attunement", 5585, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b4/Fire_Attunement.png"),
                new Boon("Water Attunement", 5586, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/31/Water_Attunement.png"),
                new Boon("Air Attunement", 5575, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/91/Air_Attunement.png"),
                new Boon("Earth Attunement", 5580, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a8/Earth_Attunement.png"),
                //forms
                new Boon("Mist Form",5543, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/1b/Mist_Form.png"),
                new Boon("Ride the Lightning",5588, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/59/Ride_the_Lightning.png"),
                new Boon("Vapor Form",5620, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/6c/Vapor_Form.png"),
                new Boon("Tornado",5534, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/40/Tornado.png"),
                //new Boon("Whirlpool", -1,BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Normal, Logic.Override),
                //conjures
                new Boon("Conjure Earth Shield", 15788, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/7/7a/Conjure_Earth_Shield.png"),
                new Boon("Conjure Flame Axe", 15789, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/a/a1/Conjure_Flame_Axe.png"),
                new Boon("Conjure Frost Bow", 15790, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/c/c3/Conjure_Frost_Bow.png"),
                new Boon("Conjure Lightning Hammer", 15791, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/1/1f/Conjure_Lightning_Hammer.png"),
                new Boon("Conjure Fiery Greatsword", 15792, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/e/e2/Conjure_Fiery_Greatsword.png"),
                //skills
                new Boon("Arcane Power",5582, BoonSource.Elementalist, BoonType.Intensity, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/72/Arcane_Power.png"),
                new Boon("Arcane Shield",5640, BoonSource.Elementalist, BoonType.Intensity, 3, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/9d/Arcane_Shield.png"),
                new Boon("Renewal of Fire",5764, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/63/Renewal_of_Fire.png"),
                new Boon("Glyph of Elemental Power (Fire)",5739, BoonSource.Elementalist, BoonType.Intensity, 5, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/f2/Glyph_of_Elemental_Power_%28fire%29.png"),
                new Boon("Glyph of Elemental Power (Water)",5741, BoonSource.Elementalist, BoonType.Intensity, 5, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/bf/Glyph_of_Elemental_Power_%28water%29.png"),
                new Boon("Glyph of Elemental Power (Air)",5740, BoonSource.Elementalist, BoonType.Intensity, 5, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/cb/Glyph_of_Elemental_Power_%28air%29.png"),
                new Boon("Glyph of Elemental Power (Earth)",5742, BoonSource.Elementalist, BoonType.Intensity, 5, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/0a/Glyph_of_Elemental_Power_%28earth%29.png"),
                new Boon("Rebound",31337, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/0/03/%22Rebound%21%22.png"),
                new Boon("Rock Barrier",34633, BoonSource.Elementalist, BoonType.Intensity, 5, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/dd/Rock_Barrier.png"),//750?
                new Boon("Magnetic Wave",15794, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/32/Magnetic_Wave.png"),
                new Boon("Obsidian Flesh",5667, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/c1/Obsidian_Flesh.png"),
                new Boon("Grinding Stones",51658, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/3d/Grinding_Stones.png"),
                new Boon("Static Charge",31487, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/4/4b/Overload_Air.png"),
                //traits
                new Boon("Harmonious Conduit",31353, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b3/Harmonious_Conduit.png"),
                new Boon("Fresh Air",34241, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d8/Fresh_Air.png"),
                new Boon("Soothing Mist", 5587, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/f/f7/Soothing_Mist.png"),
                new Boon("Weaver's Prowess",42061, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/75/Weaver%27s_Prowess.png"),
                new Boon("Elements of Rage",42416, BoonSource.Elementalist, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a2/Elements_of_Rage.png"),
                // FOODS
                new Boon("Malnourished",46587, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/6/67/Malnourished.png"),
                new Boon("Plate of Truffle Steak",9769, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/4/4c/Plate_of_Truffle_Steak.png"),
                new Boon("Bowl of Sweet and Spicy Butternut Squash Soup",17825, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/d/df/Bowl_of_Sweet_and_Spicy_Butternut_Squash_Soup.png"),
                new Boon("Bowl Curry Butternut Squash Soup",9829, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/1/16/Bowl_of_Curry_Butternut_Squash_Soup.png"),
                new Boon("Red-Lentil Saobosa",46273, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/a/a8/Red-Lentil_Saobosa.png"),
                new Boon("Super Veggie Pizza",10008, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/6/66/Super_Veggie_Pizza.png"),
                new Boon("Rare Veggie Pizza",10009, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/a/a0/Rare_Veggie_Pizza.png"),
                new Boon("Bowl of Garlic Kale Sautee",-1, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/0/04/Bowl_of_Garlic_Kale_Sautee.png"),
                new Boon("Koi Cake",-1, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/1/14/Koi_Cake.png"),
                new Boon("Prickly Pear Pie",24800, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/0/0a/Prickly_Pear_Pie.png"),
                new Boon("Bowl of Nopalitos Sauté",-1, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/f/f1/Bowl_of_Nopalitos_Saut%C3%A9.png"),
                new Boon("Loaf of Candy Cactus Cornbread",24797, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/b/b2/Loaf_of_Candy_Cactus_Cornbread.png"),
                new Boon("Delicious Rice Ball",26529, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/5d/Delicious_Rice_Ball.png"),
                new Boon("Slice of Allspice Cake",33792, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/1/13/Slice_of_Allspice_Cake.png"),
                new Boon("Fried Golden Dumpling",26530, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/1/14/Fried_Golden_Dumpling.png"),
                new Boon("Bowl of Seaweed Salad",10080, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/1/1c/Bowl_of_Seaweed_Salad.png"),
                new Boon("Bowl of Orrian Truffle and Meat Stew",10096, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/b/b8/Bowl_of_Orrian_Truffle_and_Meat_Stew.png"),
                new Boon("Plate of Mussels Gnashblade",33476, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/7/7b/Plate_of_Mussels_Gnashblade.png"),
                new Boon("Spring Roll",26534, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/d/da/Spring_Roll.png"),
                new Boon("Plate of Beef Rendang",49686, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/d/d0/Plate_of_Beef_Rendang.png"),
                new Boon("Dragon's Revelry Starcake",19451, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/2/2b/Dragon%27s_Revelry_Starcake.png"),
                new Boon("Avocado Smoothie",50091, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/8/83/Avocado_Smoothie.png"),
                new Boon("Carrot Souffle",-1, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/7/71/Carrot_Souffl%C3%A9.png"), //same as Dragon's_Breath_Bun
                new Boon("Plate of Truffle Steak Dinner",-1, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/9/92/Plate_of_Truffle_Steak_Dinner.png"), //same as Dragon's Breath Bun
                new Boon("Dragon's Breath Bun",9750, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/53/Dragon%27s_Breath_Bun.png"),
                new Boon("Karka Egg Omelet",9756, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/9/9e/Karka_Egg_Omelet.png"),
                new Boon("Steamed Red Dumpling",26536, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/8/8c/Steamed_Red_Dumpling.png"),
                new Boon("Saffron Stuffed Mushroom",-1, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/e/e2/Saffron_Stuffed_Mushroom.png"), //same as Karka Egg Omelet
                new Boon("Soul Pastry",53222, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/2/2c/Soul_Pastry.png"),
                // UTILITIES 
                // 1h versions have the same ID as 30 min versions 
                new Boon("Diminished",46668, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/7/71/Diminished.png"),
                new Boon("Superior Sharpening Stone",9963, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                new Boon("Potent Superior Sharpening Stone",-1, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                new Boon("Master Maintenance Oil",9968, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Boon("Potent Master Maintenance Oil",-1, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Boon("Tuning Icicle",34206, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/7/75/Tuning_Icicle.png"),
                new Boon("Master Tuning Crystal",9967, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/58/Master_Tuning_Crystal.png"),
                new Boon("Potent Master Tuning Crystal",-1, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/58/Master_Tuning_Crystal.png"),
                new Boon("Toxic Sharpening Stone",21826, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/d/db/Toxic_Sharpening_Stone.png"),
                new Boon("Toxic Maintenance Oil",21827, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/a/a6/Toxic_Maintenance_Oil.png"),
                new Boon("Toxic Focusing Crystal",21828, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/d/de/Toxic_Focusing_Crystal.png"),
                new Boon("Magnanimous Maintenance Oil",38605, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/53/Magnanimous_Maintenance_Oil.png"),
                new Boon("Peppermint Oil",34187, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/b/bc/Peppermint_Oil.png"),
                new Boon("Potent Lucent Oil",53374, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/1/16/Potent_Lucent_Oil.png"),
                new Boon("Enhanced Lucent Oil",53304, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/e/ee/Enhanced_Lucent_Oil.png"),
                new Boon("Furious Maintenance Oil",25881, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Boon("Furious Sharpening Stone",25882, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                new Boon("Bountiful Maintenance Oil",25879, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Boon("Tin of Fruitcake",34211, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/a/af/Tin_of_Fruitcake.png"),
                new Boon("Writ of Masterful Malice",33836, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/2/20/Writ_of_Masterful_Malice.png"),
                new Boon("Writ of Masterful Strength",33297, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/2/2b/Writ_of_Masterful_Strength.png"),
                new Boon("Powerful Potion of Flame Legion Slaying",9925, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/e/e2/Powerful_Potion_of_Flame_Legion_Slaying.png"),
                new Boon("Powerful Potion of Halloween Slaying",15279, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/f/fe/Powerful_Potion_of_Halloween_Slaying.png"),
                new Boon("Powerful Potion of Centaur Slaying",9845, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/3/3b/Powerful_Potion_of_Centaur_Slaying.png"),
                new Boon("Powerful Potion of Krait Slaying",9885, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/b/b4/Powerful_Potion_of_Krait_Slaying.png"),
                new Boon("Powerful Potion of Ogre Slaying",9877, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/b/b5/Powerful_Potion_of_Ogre_Slaying.png"),
                new Boon("Powerful Potion of Elemental Slaying",9893, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/5f/Powerful_Potion_of_Elemental_Slaying.png"),
                new Boon("Powerful Potion of Destroyer Slaying",9869, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/b/bd/Powerful_Potion_of_Destroyer_Slaying.png"),
                new Boon("Powerful Potion of Nightmare Court Slaying",9941, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/7/74/Powerful_Potion_of_Nightmare_Court_Slaying.png"),
                new Boon("Powerful Potion of Slaying Scarlet's Armies",23228, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/e/ee/Powerful_Potion_of_Demon_Slaying.png"),
                new Boon("Powerful Potion of Undead Slaying",9837, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/b/bd/Powerful_Potion_of_Undead_Slaying.png"),
                new Boon("Powerful Potion of Dredge Slaying",9949, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/9/9a/Powerful_Potion_of_Dredge_Slaying.png"),
                new Boon("Powerful Potion of Inquest Slaying",9917, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/f/fb/Powerful_Potion_of_Inquest_Slaying.png"),
                new Boon("Powerful Potion of Demon Slaying",9901, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/e/ee/Powerful_Potion_of_Demon_Slaying.png"),
                new Boon("Powerful Potion of Grawl Slaying",9853, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/1/15/Powerful_Potion_of_Grawl_Slaying.png"),
                new Boon("Powerful Potion of Sons of Svanir Slaying",9909, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/3/33/Powerful_Potion_of_Sons_of_Svanir_Slaying.png"),
                new Boon("Powerful Potion of Outlaw Slaying",9933, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/e/ec/Powerful_Potion_of_Outlaw_Slaying.png"),
                new Boon("Powerful Potion of Ice Brood Slaying",9861, BoonSource.Item, BoonType.Duration, 1, BoonNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/0/0d/Powerful_Potion_of_Ice_Brood_Slaying.png"),
                // new Boon("Hylek Maintenance Oil",9968, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"), when wiki says "same stats" its literally the same buff
        };


        public static Dictionary<long, Boon> BoonsByIds = _allBoons.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.First());
        public static Dictionary<BoonNature, List<Boon>> BoonsByNature = _allBoons.GroupBy(x => x.Nature).ToDictionary(x => x.Key, x => x.ToList());
        public static Dictionary<BoonSource, List<Boon>> BoonsBySource = _allBoons.GroupBy(x => x.Source).ToDictionary(x => x.Key, x => x.ToList());
        public static Dictionary<BoonType, List<Boon>> BoonsByType = _allBoons.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToList());
        public static Dictionary<int, List<Boon>> BoonsByCapacity = _allBoons.GroupBy(x => x.Capacity).ToDictionary(x => x.Key, x => x.ToList());

        // debug
        public static List<Boon> GetBoonByName(string name)
        {
            return _allBoons.Where(x => x.Name == name).ToList();
        }

        // get everything
        public static List<Boon> GetAll()
        {
            return _allBoons;
        }

        // Conditions
        public static List<Boon> GetCondiBoonList()
        {
            return BoonsByNature[BoonNature.Condition];
        }
        // Boons
        public static List<Boon> GetBoonList()
        {
            return BoonsByNature[BoonNature.Boon];
        }
        // Shareable buffs
        public static List<Boon> GetOffensiveTableList()
        {
            return BoonsByNature[BoonNature.OffensiveBuffTable];
        }
        public static List<Boon> GetDefensiveTableList()
        {
            return BoonsByNature[BoonNature.DefensiveBuffTable];
        }
        // Consumables (Food and Utility)
        public static List<Boon> GetConsumableList()
        {
            return BoonsByNature[BoonNature.Consumable];
        }
        // Enemy
        public static List<Boon> GetEnemyBoonList()
        {
            return BoonsBySource[BoonSource.Enemy];
        }
        // All buffs
        public static List<Boon> GetAllBuffList()
        {
            List<Boon> res = new List<Boon>();
            // correct order for the boon graph
            res.AddRange(BoonsByNature[BoonNature.Boon]);
            res.AddRange(BoonsByNature[BoonNature.DefensiveBuffTable]);
            res.AddRange(BoonsByNature[BoonNature.OffensiveBuffTable]);
            res.AddRange(BoonsByNature[BoonNature.GraphOnlyBuff]);
            return res;
        }
        // Non shareable buffs
        public static List<Boon> GetRemainingBuffsList()
        {
            return BoonsByNature[BoonNature.GraphOnlyBuff];
        }
        private static List<Boon> GetRemainingBuffsList(BoonSource source)
        {
            return BoonsBySource[source].Where(x => x.Nature == BoonNature.GraphOnlyBuff).ToList();
        }
        public static List<Boon> GetRemainingBuffsList(string source)
        {
            return GetRemainingBuffsList(ProfToEnum(source));
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
            switch (Type)
            {
                case BoonType.Intensity: return new BoonSimulatorIntensity(Capacity, log, logicToUse);
                case BoonType.Duration: return new BoonSimulatorDuration(Capacity, log, logicToUse);
                default: throw new InvalidOperationException();
            }
        }
    }
}
