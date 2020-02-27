using System;
using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class Buff
    {
        // Boon
        public enum BuffNature { Condition, Boon, OffensiveBuffTable, DefensiveBuffTable, GraphOnlyBuff, Consumable, Unknow };
        public enum BuffSource { Mixed, Necromancer, Elementalist, Mesmer, Warrior, Revenant, Guardian, Thief, Ranger, Engineer, Item, Enemy, Unknown };
        public enum BuffType { Duration, Intensity, Unknown };
        private enum Logic { Queue, HealingPower, Override, ForceOverride, Unknown };

        public static BuffSource ProfToEnum(string prof)
        {
            switch (prof)
            {
                case "Druid":
                case "Ranger":
                case "Soulbeast":
                    return BuffSource.Ranger;
                case "Scrapper":
                case "Holosmith":
                case "Engineer":
                    return BuffSource.Engineer;
                case "Daredevil":
                case "Deadeye":
                case "Thief":
                    return BuffSource.Thief;
                case "Weaver":
                case "Tempest":
                case "Elementalist":
                    return BuffSource.Elementalist;
                case "Mirage":
                case "Chronomancer":
                case "Mesmer":
                    return BuffSource.Mesmer;
                case "Scourge":
                case "Reaper":
                case "Necromancer":
                    return BuffSource.Necromancer;
                case "Spellbreaker":
                case "Berserker":
                case "Warrior":
                    return BuffSource.Warrior;
                case "Firebrand":
                case "Dragonhunter":
                case "Guardian":
                    return BuffSource.Guardian;
                case "Renegade":
                case "Herald":
                case "Revenant":
                    return BuffSource.Revenant;
            }
            return BuffSource.Mixed;
        }

        // Fields
        public string Name { get; }
        public long ID { get; }
        public BuffNature Nature { get; }
        public BuffSource Source { get; }
        public BuffType Type { get; }
        public ulong MaxBuild { get; } = ulong.MaxValue;
        public ulong MinBuild { get; } = ulong.MinValue;
        public int Capacity { get; }
        public string Link { get; }
        private readonly Logic _logic;

        /// <summary>
        /// Buff constructor
        /// </summary>
        /// <param name="name">The name of the boon</param>
        /// <param name="id">The id of the boon</param>
        /// <param name="source">Source of the boon <see cref="BuffSource"/></param>
        /// <param name="type">Type of the boon (duration or intensity) <see cref="BuffType"/></param>
        /// <param name="capacity">Maximun amount of boon in the queue (duration) or stack (intensity)</param>
        /// <param name="nature">Nature of the boon, dictates in which category the boon will appear <see cref="BuffNature"/></param>
        /// <param name="logic">Stacking logic of the boon <see cref="Logic"/>, in doubt use Override</param>
        /// <param name="link">URL to the icon of the buff</param>
        private Buff(string name, long id, BuffSource source, BuffType type, int capacity, BuffNature nature, Logic logic, string link)
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

        private Buff(string name, long id, BuffSource source, BuffType type, int capacity, BuffNature nature, Logic logic, string link, ulong minBuild, ulong maxBuild)
        {
            Name = name;
            ID = id;
            Source = source;
            Type = type;
            Capacity = capacity;
            Nature = nature;
            Link = link;
            _logic = logic;
            MaxBuild = maxBuild;
            MinBuild = minBuild;
        }

        public Buff(string name, long id, string link)
        {
            Name = name;
            ID = id;
            Source = BuffSource.Unknown;
            Type = BuffType.Unknown;
            Capacity = 0;
            Nature = BuffNature.Unknow;
            Link = link;
            _logic = Logic.Unknown;
        }

        private static readonly List<Buff> _boons = new List<Buff>
        {
                new Buff("Might", 740, BuffSource.Mixed, BuffType.Intensity, 25, BuffNature.Boon, Logic.Override, "https://wiki.guildwars2.com/images/7/7c/Might.png"),
                new Buff("Fury", 725, BuffSource.Mixed, BuffType.Duration, 9, BuffNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/4/46/Fury.png"),//or 3m and 30s
                new Buff("Quickness", 1187, BuffSource.Mixed, BuffType.Duration, 5, BuffNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/b/b4/Quickness.png"),
                new Buff("Alacrity", 30328, BuffSource.Mixed, BuffType.Duration, 9, BuffNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/4/4c/Alacrity.png"),
                new Buff("Protection", 717, BuffSource.Mixed, BuffType.Duration, 5, BuffNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/6/6c/Protection.png"),
                new Buff("Regeneration", 718, BuffSource.Mixed, BuffType.Duration, 5, BuffNature.Boon, Logic.HealingPower, "https://wiki.guildwars2.com/images/5/53/Regeneration.png"),
                new Buff("Vigor", 726, BuffSource.Mixed, BuffType.Duration, 5, BuffNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/f/f4/Vigor.png"),
                new Buff("Aegis", 743, BuffSource.Mixed, BuffType.Duration, 9, BuffNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/e/e5/Aegis.png"),
                new Buff("Stability", 1122, BuffSource.Mixed, BuffType.Intensity, 25, BuffNature.Boon, Logic.Override, "https://wiki.guildwars2.com/images/a/ae/Stability.png"),
                new Buff("Swiftness", 719, BuffSource.Mixed, BuffType.Duration, 9, BuffNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/a/af/Swiftness.png"),
                new Buff("Retaliation", 873, BuffSource.Mixed, BuffType.Duration, 5, BuffNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/5/53/Retaliation.png"),
                new Buff("Resistance", 26980, BuffSource.Mixed, BuffType.Duration, 5, BuffNature.Boon, Logic.Queue, "https://wiki.guildwars2.com/images/4/4b/Resistance.png"),
                new Buff("Number of Boons", ProfHelper.NumberOfBoonsID, BuffSource.Mixed, BuffType.Intensity, 0, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/44/Boon_Duration.png"),
        };

        private static readonly List<Buff> _conditions = new List<Buff>
        {
                new Buff("Bleeding", 736, BuffSource.Mixed, BuffType.Intensity, 1500, BuffNature.Condition, Logic.Override, "https://wiki.guildwars2.com/images/3/33/Bleeding.png"),
                new Buff("Burning", 737, BuffSource.Mixed, BuffType.Intensity, 1500, BuffNature.Condition, Logic.Override, "https://wiki.guildwars2.com/images/4/45/Burning.png"),
                new Buff("Confusion", 861, BuffSource.Mixed, BuffType.Intensity, 1500, BuffNature.Condition, Logic.Override, "https://wiki.guildwars2.com/images/e/e6/Confusion.png"),
                new Buff("Poison", 723, BuffSource.Mixed, BuffType.Intensity, 1500, BuffNature.Condition, Logic.Override, "https://wiki.guildwars2.com/images/1/11/Poisoned.png"),
                new Buff("Torment", 19426, BuffSource.Mixed, BuffType.Intensity, 1500, BuffNature.Condition, Logic.Override, "https://wiki.guildwars2.com/images/0/08/Torment.png"),
                new Buff("Blind", 720, BuffSource.Mixed, BuffType.Duration, 9, BuffNature.Condition, Logic.Queue, "https://wiki.guildwars2.com/images/3/33/Blinded.png"),
                new Buff("Chilled", 722, BuffSource.Mixed, BuffType.Duration, 5, BuffNature.Condition, Logic.Queue, "https://wiki.guildwars2.com/images/a/a6/Chilled.png"),
                new Buff("Crippled", 721, BuffSource.Mixed, BuffType.Duration, 5, BuffNature.Condition, Logic.Queue, "https://wiki.guildwars2.com/images/f/fb/Crippled.png"),
                new Buff("Fear", 791, BuffSource.Mixed, BuffType.Duration, 5, BuffNature.Condition, Logic.Queue, "https://wiki.guildwars2.com/images/e/e6/Fear.png"),
                new Buff("Immobile", 727, BuffSource.Mixed, BuffType.Duration, 3, BuffNature.Condition, Logic.Queue, "https://wiki.guildwars2.com/images/3/32/Immobile.png"),
                new Buff("Slow", 26766, BuffSource.Mixed, BuffType.Duration, 9, BuffNature.Condition, Logic.Queue, "https://wiki.guildwars2.com/images/f/f5/Slow.png"),
                new Buff("Weakness", 742, BuffSource.Mixed, BuffType.Duration, 5, BuffNature.Condition, Logic.Queue, "https://wiki.guildwars2.com/images/f/f9/Weakness.png"),
                new Buff("Taunt", 27705, BuffSource.Mixed, BuffType.Duration, 5, BuffNature.Condition, Logic.Queue, "https://wiki.guildwars2.com/images/c/cc/Taunt.png"),
                new Buff("Vulnerability", 738, BuffSource.Mixed, BuffType.Intensity, 25, BuffNature.Condition, Logic.Override, "https://wiki.guildwars2.com/images/a/af/Vulnerability.png"),
                new Buff("Number of Conditions", ProfHelper.NumberOfConditionsID, BuffSource.Mixed, BuffType.Intensity, 0, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/38/Condition_Duration.png"),
        };

        private static readonly List<Buff> _commons = new List<Buff>
        {
                new Buff("Downed", 770, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/dd/Downed.png"),
                new Buff("Stealth", 13017, BuffSource.Mixed, BuffType.Duration, 5, BuffNature.GraphOnlyBuff, Logic.Queue, "https://wiki.guildwars2.com/images/1/19/Stealth.png"),
                new Buff("Hide in Shadows", 10269, BuffSource.Mixed, BuffType.Duration, 5, BuffNature.GraphOnlyBuff, Logic.Queue, "https://wiki.guildwars2.com/images/1/19/Stealth.png"),
                new Buff("Revealed", 890, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.ForceOverride, "https://wiki.guildwars2.com/images/d/db/Revealed.png"),
                new Buff("Superspeed", 5974, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.ForceOverride,"https://wiki.guildwars2.com/images/1/1a/Super_Speed.png"),
                new Buff("Determined (762)", 762, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Determined (788)", 788, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Determined (895)", 895, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Determined (3892)", 3892, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Determined (31450)", 31450, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Determined (52271)", 52271, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Invulnerability (757)", 757, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Invulnerability (801)", 801, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Stun", 872, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.ForceOverride, "https://wiki.guildwars2.com/images/9/97/Stun.png"),
                new Buff("Daze", 833, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.ForceOverride, "https://wiki.guildwars2.com/images/7/79/Daze.png"),
                new Buff("Exposed", 48209, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.ForceOverride,"https://wiki.guildwars2.com/images/f/f4/Exposed_%28effect%29.png"),
                new Buff("Unblockable",36781, BuffSource.Mixed, BuffType.Intensity, 99, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/f0/Unblockable_%28effect%29.png",102321 , ulong.MaxValue),
                //Auras
                new Buff("Chaos Aura", 10332, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/ec/Chaos_Aura.png"),
                new Buff("Fire Aura", 5677, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/c/ce/Fire_Aura.png"),
                new Buff("Frost Aura", 5579, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/8/87/Frost_Aura_%28effect%29.png"),
                new Buff("Light Aura", 25518, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/5/5a/Light_Aura.png"),
                new Buff("Magnetic Aura", 5684, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/0b/Magnetic_Aura_%28effect%29.png"),
                new Buff("Shocking Aura", 5577, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/5/5d/Shocking_Aura_%28effect%29.png"),
                new Buff("Dark Aura", 39978, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/ef/Dark_Aura.png", 96406, ulong.MaxValue),
                //race
                new Buff("Take Root", 12459, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/b/b2/Take_Root.png"),
                new Buff("Become the Bear",12426, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/7/7e/Become_the_Bear.png"),
                new Buff("Become the Raven",12405, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/2/2c/Become_the_Raven.png"),
                new Buff("Become the Snow Leopard",12400, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/7/78/Become_the_Snow_Leopard.png"),
                new Buff("Become the Wolf",12393, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/f1/Become_the_Wolf.png"),
                new Buff("Avatar of Melandru", 12368, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/30/Avatar_of_Melandru.png"),
                new Buff("Power Suit",12326, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/89/Summon_Power_Suit.png"),
                new Buff("Reaper of Grenth", 12366, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/07/Reaper_of_Grenth.png"),
                new Buff("Charrzooka",43503, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/1/17/Charrzooka.png"),
                // Fractals 
                new Buff("Rigorous Certainty", 33652, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.ForceOverride,"https://wiki.guildwars2.com/images/6/60/Desert_Carapace.png"),
        };

        private static readonly List<Buff> _gear = new List<Buff>
        {
                new Buff("Sigil of Concentration", 33719, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b3/Superior_Sigil_of_Concentration.png",0 , 93543),
                new Buff("Superior Rune of the Monk", 53285, BuffSource.Mixed, BuffType.Intensity, 10, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/18/Superior_Rune_of_the_Monk.png", 93543, ulong.MaxValue),
                new Buff("Sigil of Corruption", 9374, BuffSource.Mixed, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/18/Superior_Sigil_of_Corruption.png"),
                new Buff("Sigil of Life", 9386, BuffSource.Mixed, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a7/Superior_Sigil_of_Life.png"),
                new Buff("Sigil of Perception", 9385, BuffSource.Mixed, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/cc/Superior_Sigil_of_Perception.png"),
                new Buff("Sigil of Bloodlust", 9286, BuffSource.Mixed, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/fb/Superior_Sigil_of_Bloodlust.png"),
                new Buff("Sigil of Bounty", 38588, BuffSource.Mixed, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/f8/Superior_Sigil_of_Bounty.png"),
                new Buff("Sigil of Benevolence", 9398, BuffSource.Mixed, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/59/Superior_Sigil_of_Benevolence.png"),
                new Buff("Sigil of Momentum", 22144, BuffSource.Mixed, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/30/Superior_Sigil_of_Momentum.png"),
                new Buff("Sigil of the Stars", 46953, BuffSource.Mixed, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/dc/Superior_Sigil_of_the_Stars.png"),
        };

        private static readonly List<Buff> _fightSpecific = new List<Buff>
        {
                // Whisper of Jormalg
                new Buff("Whisper Teleport Out", 59223, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Whisper Teleport Back", 59054, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Frigid Vortex", 59105, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Chains of Frost Active", 59100, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Chains of Frost Application", 59120, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Brain Freeze", 59073, BuffSource.Enemy, BuffType.Intensity, 6, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                // Voice and Claw            
                new Buff("Enraged", 58619, BuffSource.Enemy, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                // Fraenir of Jormag
                new Buff("Frozen", 58376, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/7a/Bloodstone_Blessed.png" ),
                new Buff("Snowblind", 58276, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/7a/Bloodstone_Blessed.png" ),
                // Twisted Castle
                new Buff("Spatial Distortion", 34918, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/7a/Bloodstone_Blessed.png" ),
                new Buff("Madness", 35006, BuffSource.Enemy, BuffType.Intensity, 99, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/ee/Madness.png" ),
                new Buff("Still Waters", 35106, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/5c/Still_Waters_%28effect%29.png" ),
                new Buff("Soothing Waters", 34955, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8f/Soothing_Waters.png" ),
                new Buff("Chaotic Haze", 34963, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/48/Lava_Font.png" ),
                new Buff("Timed Bomb", 31485, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/91/Time_Bomb.png" ),
                // Deimos
                new Buff("Unnatural Signet",38224, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/20/Unnatural_Signet.png"),
                new Buff("Compromised",35096, BuffSource.Enemy, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/48/Compromised.png"),
                // KC
                new Buff("Xera's Boon",35025, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/04/Xera%27s_Boon.png"),
                new Buff("Xera's Fury",35103, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/dd/Xera%27s_Fury.png"),
                new Buff("Statue Fixated",34912, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/dd/Xera%27s_Fury.png"),
                new Buff("Crimson Attunement (Orb)",35091, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/3e/Crimson_Attunement.png"),
                new Buff("Radiant Attunement (Orb)",35109, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/68/Radiant_Attunement.png"),
                new Buff("Magic Blast",35119, BuffSource.Enemy, BuffType.Intensity, 35, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a9/Magic_Blast_Intensity.png"),
                // Gorseval
                new Buff("Spirited Fusion",31722, BuffSource.Enemy, BuffType.Intensity, 500, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/eb/Spirited_Fusion.png"),
                // Matthias
                new Buff("Blood Shield Abo",34376, BuffSource.Enemy, BuffType.Intensity, 18, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a6/Blood_Shield.png"),
                new Buff("Blood Shield",34518, BuffSource.Enemy, BuffType.Intensity, 18, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a6/Blood_Shield.png"),
                new Buff("Blood Fueled",34422, BuffSource.Enemy, BuffType.Intensity, 20, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Blood Fueled Abo",34428, BuffSource.Enemy, BuffType.Intensity, 20, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                // Qadim
                new Buff("Flame Armor",52568, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Fiery Surge",52588, BuffSource.Enemy, BuffType.Intensity, 99, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                // Soulless Horror
                new Buff("Necrosis",47414, BuffSource.Enemy, BuffType.Intensity, 99, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/47/Ichor.png"),
                // CA
                new Buff("Fractured - Enemy",53030, BuffSource.Enemy, BuffType.Intensity, 10, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Fractured - Allied",52213, BuffSource.Enemy, BuffType.Intensity, 10, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Conjured Protection",52973 , BuffSource.Enemy, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Conjured Shield",52754 , BuffSource.Enemy, BuffType.Intensity, 10, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Greatsword Power",52667 , BuffSource.Enemy, BuffType.Intensity, 10, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Conjured Barrier",53003 , BuffSource.Enemy, BuffType.Intensity, 10, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Scepter Lock-on",53075  , BuffSource.Enemy, BuffType.Intensity, 4, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Augmented Power",52074  , BuffSource.Enemy, BuffType.Intensity, 10, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("CA Invul",52255 , BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Arm Up",52430 , BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                // Twin Largos
                new Buff("Aquatic Detainment",52931 , BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Aquatic Aura (Kenut)",52211 , BuffSource.Enemy, BuffType.Intensity, 80, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Aquatic Aura (Nikare)",52929 , BuffSource.Enemy, BuffType.Intensity, 80, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Waterlogged",51935 , BuffSource.Enemy, BuffType.Intensity, 10, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Protective Shadow", 31877, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/8/87/Protective_Shadow.png"),
                // Slothasor
                new Buff("Narcolepsy", 34467, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                // VG
                new Buff("Blue Pylon Power", 31413, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/6/6e/Blue_Pylon_Power.png"),
                new Buff("Unbreakable", 34979, BuffSource.Enemy, BuffType.Intensity, 2, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/5/56/Xera%27s_Embrace.png"),
                // Trio
                new Buff("Not the Bees!", 34434, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/0/08/Throw_Jar.png"),
                new Buff("Targeted", 34392, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                // Dhuum
                new Buff("Spirit Transfrom", 48281, BuffSource.Enemy, BuffType.Intensity, 30, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/4/48/Compromised.png"),
                new Buff("Fractured Spirit", 46950, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/c/c3/Fractured_Spirit.png"),
                new Buff("Residual Affliction", 47476, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/1/12/Residual_Affliction.png"),
                new Buff("Arcing Affliction", 47646, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/f/f0/Arcing_Affliction.png"),
                // Adina
                new Buff("Pillar Pandemonium", 56204, BuffSource.Enemy, BuffType.Intensity, 99, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Radiant Blindness", 56593, BuffSource.Enemy, BuffType.Duration, 25, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Diamond Palisade (Damage)", 56099, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Diamond Palisade", 56636, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Eroding Curse", 56440, BuffSource.Enemy, BuffType.Intensity, 99, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                // Sabir
                new Buff("Ion Shield", 56100, BuffSource.Enemy, BuffType.Intensity, 80, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Violent Currents", 56123, BuffSource.Enemy, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/0/06/Violent_Currents.png"),
                new Buff("Repulsion Field", 56172, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Electrical Repulsion", 56391, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://render.guildwars2.com/file/5AF8170865B353E26127E10E34EFE8B90B9096D6/1451806.png"),
                new Buff("Electro-Repulsion", 56474, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/7/74/Unbridled_Chaos.png"),
                // Peerless Qadim
                new Buff("Erratic Energy", 56582, BuffSource.Enemy, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/4/45/Unstable.png"),
                new Buff("Power Share", 56104, BuffSource.Enemy, BuffType.Intensity, 3, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Sapping Surge", 56118, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/6/6f/Guilt_Exploitation.png"),
                new Buff("Chaos Corrosion", 56182, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/f/fd/Fractured_%28effect%29.png"),
                new Buff("Peerless Fixated", 56510, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Magma Drop", 56475, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Kinetic Abundance", 56609, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/0/06/Values_Mastery.png"),
                new Buff("Unbridled Chaos", 56467, BuffSource.Enemy, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/7/74/Unbridled_Chaos.png"),
        };

        private static readonly List<Buff> _revenant = new List<Buff>
        {         
                //skills
                new Buff("Crystal Hibernation", 28262, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/4/4a/Crystal_Hibernation.png"),
                new Buff("Vengeful Hammers", 27273, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/c/c8/Vengeful_Hammers.png"),
                new Buff("Rite of the Great Dwarf", 26596, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/6/69/Rite_of_the_Great_Dwarf.png"),
                new Buff("Embrace the Darkness", 28001, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/51/Embrace_the_Darkness.png"),
                new Buff("Enchanted Daggers", 28557, BuffSource.Revenant, BuffType.Intensity, 6, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/fa/Enchanted_Daggers.png"),
                new Buff("Phase Traversal", 28395, BuffSource.Revenant, BuffType.Intensity, 2, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/f2/Phase_Traversal.png"),
                new Buff("Impossible Odds", 27581, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/87/Impossible_Odds.png"),
                //facets
                new Buff("Facet of Light",27336, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/18/Facet_of_Light.png"),
                new Buff("Facet of Light (Traited)",51690, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/18/Facet_of_Light.png"), //Lingering buff with Draconic Echo trait
                new Buff("Infuse Light",27737, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/6/60/Infuse_Light.png"),
                new Buff("Facet of Darkness",28036, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/e4/Facet_of_Darkness.png"),
                new Buff("Facet of Darkness (Traited)",51695, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/e/e4/Facet_of_Darkness.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Elements",28243, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/ce/Facet_of_Elements.png"),
                new Buff("Facet of Elements (Traited)",51706, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/ce/Facet_of_Elements.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Strength",27376, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a8/Facet_of_Strength.png"),
                new Buff("Facet of Strength (Traited)",51700, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a8/Facet_of_Strength.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Chaos",27983, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/c7/Facet_of_Chaos.png"),
                new Buff("Facet of Nature",29275, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),
                new Buff("Facet of Nature (Traited)",51681, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Nature-Assassin",51692, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/c/cd/Facet_of_Nature%E2%80%95Assassin.png"),
                new Buff("Facet of Nature-Dragon",51674, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/4/47/Facet_of_Nature%E2%80%95Dragon.png"),
                new Buff("Facet of Nature-Demon",51704, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/f/ff/Facet_of_Nature%E2%80%95Demon.png"),
                new Buff("Facet of Nature-Dwarf",51677, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/4/4c/Facet_of_Nature%E2%80%95Dwarf.png"),
                new Buff("Facet of Nature-Centaur",51699, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/7/74/Facet_of_Nature%E2%80%95Centaur.png"),
                new Buff("Naturalistic Resonance", 29379, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),
                //legends
                new Buff("Legendary Centaur Stance",27972, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8a/Legendary_Centaur_Stance.png"),
                new Buff("Legendary Dragon Stance",27732, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d5/Legendary_Dragon_Stance.png"),
                new Buff("Legendary Dwarf Stance",27205, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b2/Legendary_Dwarf_Stance.png"),
                new Buff("Legendary Demon Stance",27928, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d1/Legendary_Demon_Stance.png"),
                new Buff("Legendary Assassin Stance",27890, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/02/Legendary_Assassin_Stance.png"),
                new Buff("Legendary Renegade Stance",44272, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/19/Legendary_Renegade_Stance.png"),
                //summons
                new Buff("Breakrazor's Bastion",44682, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/a/a7/Breakrazor%27s_Bastion.png"),
                new Buff("Razorclaw's Rage",41016, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/7/73/Razorclaw%27s_Rage.png"),
                new Buff("Soulcleave's Summit",45026, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Soulcleave%27s_Summit.png"),
                //traits
                new Buff("Vicious Lacerations",29395, BuffSource.Revenant, BuffType.Intensity, 3, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/cd/Vicious_Lacerations.png", 0, 102321),
                new Buff("Rising Momentum",51683, BuffSource.Revenant, BuffType.Intensity, 10, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8c/Rising_Momentum.png"),
                new Buff("Assassin's Presence", 26854, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/5/54/Assassin%27s_Presence.png"),
                new Buff("Expose Defenses", 48894, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/5c/Mutilate_Defenses.png"),
                new Buff("Invoking Harmony",29025, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/ec/Invoking_Harmony.png"),
                new Buff("Unyielding Devotion",55044, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/4f/Unyielding_Devotion.png", 96406, ulong.MaxValue),
                //new Boon("Selfless Amplification",29025, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/23/Selfless_Amplification.png"),
                new Buff("Hardening Persistence",28957, BuffSource.Revenant, BuffType.Intensity, 10, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/0f/Hardening_Persistence.png"),
                new Buff("Soothing Bastion",34136, BuffSource.Revenant, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/04/Soothing_Bastion.png"),
                new Buff("Kalla's Fervor",42883, BuffSource.Revenant, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png"),
                new Buff("Improved Kalla's Fervor",45614, BuffSource.Revenant, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png"),
        };

        private static readonly List<Buff> _warrior = new List<Buff>
        {
                new Buff("Berserk",29502, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/4/44/Berserk.png"),
            //skills
                new Buff("Riposte",14434, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/d/de/Riposte.png"),
                new Buff("Flames of War", 31708, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/6f/Flames_of_War_%28warrior_skill%29.png"),
                new Buff("Blood Reckoning", 29466 , BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d6/Blood_Reckoning.png"),
                new Buff("Rock Guard", 34256 , BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/c7/Shattering_Blow.png"),
                new Buff("Sight beyond Sight",40616, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/d/d7/Sight_beyond_Sight.png"),
                //signets
                new Buff("Healing Signet",786, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/8/85/Healing_Signet.png"),
                new Buff("Dolyak Signet",14458, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/60/Dolyak_Signet.png"),
                new Buff("Signet of Fury",14459, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/c1/Signet_of_Fury.png"),
                new Buff("Signet of Might",14444, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/40/Signet_of_Might.png"),
                new Buff("Signet of Stamina",14478, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/6b/Signet_of_Stamina.png"),
                new Buff("Signet of Rage",14496, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/bc/Signet_of_Rage.png"),
                //banners
                new Buff("Banner of Strength", 14417, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/e/e1/Banner_of_Strength.png/33px-Banner_of_Strength.png"),
                new Buff("Banner of Discipline", 14449, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/5/5f/Banner_of_Discipline.png/33px-Banner_of_Discipline.png"),
                new Buff("Banner of Tactics",14450, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/3/3f/Banner_of_Tactics.png/33px-Banner_of_Tactics.png"),
                new Buff("Banner of Defense",14543, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/f/f1/Banner_of_Defense.png/33px-Banner_of_Defense.png"),
                //stances
                new Buff("Shield Stance",756, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/d/de/Shield_Stance.png"),
                new Buff("Berserker's Stance",14453, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/8/8a/Berserker_Stance.png"),
                new Buff("Enduring Pain",787, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/24/Endure_Pain.png"),
                new Buff("Balanced Stance",34778, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/27/Balanced_Stance.png"),
                new Buff("Defiant Stance",21816, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/db/Defiant_Stance.png"),
                new Buff("Rampage",14484, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e4/Rampage.png"),
                //traits
                new Buff("Soldier's Focus", 58102, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/thumb/4/4c/Empower_Allies.png/20px-Empower_Allies.png", 99526, ulong.MaxValue),
                new Buff("Brave Stride", 43063, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b8/Death_from_Above.png"),
                new Buff("Empower Allies", 14222, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/4/4c/Empower_Allies.png/20px-Empower_Allies.png"),
                new Buff("Peak Performance",46853, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/98/Peak_Performance.png"),
                new Buff("Furious Surge", 30204, BuffSource.Warrior, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/65/Furious.png"),
                //new Boon("Health Gain per Adrenaline bar Spent",-1, BoonSource.Warrior, BoonType.Intensity, 3, BoonEnum.GraphOnlyBuff,RemoveType.Normal, Logic.Override),
                new Buff("Rousing Resilience",24383, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/ca/Rousing_Resilience.png"),
                new Buff("Feel No Pain (Savage Instinct)",55030, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/4d/Savage_Instinct.png", 96406, ulong.MaxValue),
                new Buff("Always Angry",34099, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/63/Always_Angry.png", 0 , 96406),
                new Buff("Full Counter",43949, BuffSource.Warrior, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/fb/Full_Counter.png"),
                new Buff("Attacker's Insight",41963, BuffSource.Warrior, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/89/Attacker%27s_Insight.png"),
                new Buff("Berserker's Power",42539, BuffSource.Warrior, BuffType.Intensity, 3, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/6f/Berserker%27s_Power.png"),
        };

        private static readonly List<Buff> _guardian = new List<Buff>
        {        
                //skills
                new Buff("Zealot's Flame", 9103, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/7a/Zealot%27s_Flame.png"),
                new Buff("Purging Flames",21672, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/28/Purging_Flames.png"),
                new Buff("Litany of Wrath",21665, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/4a/Litany_of_Wrath.png"),
                new Buff("Renewed Focus",9255, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/10/Renewed_Focus.png"),
                new Buff("Ashes of the Just",41957, BuffSource.Guardian, BuffType.Intensity, 25, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/6/6d/Epilogue-_Ashes_of_the_Just.png"),
                new Buff("Eternal Oasis",44871, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/5/5f/Epilogue-_Eternal_Oasis.png"),
                new Buff("Unbroken Lines",43194, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/d/d8/Epilogue-_Unbroken_Lines.png"),
                new Buff("Shield of Wrath",9123, BuffSource.Guardian, BuffType.Intensity, 3, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/bc/Shield_of_Wrath.png"),
                //signets
                new Buff("Signet of Resolve",9220, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/75/Signet_of_Resolve.png"),
                new Buff("Signet of Resolve (Shared)", 46554, BuffSource.Guardian, BuffType.Intensity, 99, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/7/75/Signet_of_Resolve.png"),
                new Buff("Bane Signet",9092, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/56/Bane_Signet.png"),
                new Buff("Bane Signet (PI)",9240, BuffSource.Guardian, BuffType.Intensity, 99, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/5/56/Bane_Signet.png"),
                new Buff("Signet of Judgment",9156, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/fe/Signet_of_Judgment.png"),
                new Buff("Signet of Judgment (PI)",9239, BuffSource.Guardian, BuffType.Intensity, 99, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/f/fe/Signet_of_Judgment.png"),
                new Buff("Signet of Mercy",9162, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/37/Signet_of_Mercy.png"),
                new Buff("Signet of Mercy (PI)",9238, BuffSource.Guardian, BuffType.Intensity, 99, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/3/37/Signet_of_Mercy.png"),
                new Buff("Signet of Wrath",9100, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/18/Signet_of_Wrath.png"),
                new Buff("Signet of Wrath (PI)",9237, BuffSource.Guardian, BuffType.Intensity, 99, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/1/18/Signet_of_Wrath.png"),
                new Buff("Signet of Courage",29633, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/5d/Signet_of_Courage.png"),
                new Buff("Signet of Courage (Shared)",43487 , BuffSource.Guardian, BuffType.Intensity, 99, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/5/5d/Signet_of_Courage.png"),
                //virtues
                new Buff("Virtue of Justice", 9114, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/10/Virtue_of_Justice.png"),
                new Buff("Spear of Justice", 29632, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/f1/Spear_of_Justice.png"),
                new Buff("Virtue of Courage", 9113, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a9/Virtue_of_Courage.png"),
                new Buff("Shield of Courage", 29523, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/63/Shield_of_Courage.png"),
                new Buff("Virtue of Resolve", 9119, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b2/Virtue_of_Resolve.png"),
                new Buff("Wings of Resolve", 30308, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/cb/Wings_of_Resolve.png"),
                new Buff("Tome of Justice",40530, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/ae/Tome_of_Justice.png"),
                new Buff("Tome of Courage",43508,BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/54/Tome_of_Courage.png"),
                new Buff("Tome of Resolve",46298, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a9/Tome_of_Resolve.png"),
                //traits
                new Buff("Strength in Numbers",13796, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/7/7b/Strength_in_Numbers.png"),
                new Buff("Invigorated Bulwark",30207, BuffSource.Guardian, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/00/Invigorated_Bulwark.png"),
                new Buff("Battle Presence", 17046, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/2/27/Battle_Presence.png"),
                //new Boon("Force of Will",29485, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),//not sure if intensity
                new Buff("Quickfire",45123, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d6/Quickfire.png"),
                new Buff("Symbolic Avenger",56890, BuffSource.Guardian, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e5/Symbolic_Avenger.png", 97950, ulong.MaxValue),
                new Buff("Inspiring Virtue",59592, BuffSource.Guardian, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8f/Virtuous_Solace.png", 102321, ulong.MaxValue),
        };

        private static readonly List<Buff> _engineer = new List<Buff>
        {       //skills
                new Buff("Static Shield",6055, BuffSource.Engineer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/90/Static_Shield.png"),
                new Buff("Absorb",6056, BuffSource.Engineer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8a/Absorb.png"),
                new Buff("A.E.D.",21660, BuffSource.Engineer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e6/A.E.D..png"),
                new Buff("Elixir S",5863, BuffSource.Engineer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d8/Elixir_S.png"),
                //new Boon("Elixir X", -1,BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Normal, Logic.Override),
                new Buff("Utility Goggles",5864, BuffSource.Engineer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/02/Utility_Goggles.png"),
                new Buff("Slick Shoes",5833, BuffSource.Engineer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/3d/Slick_Shoes.png"),
                //new Boon("Watchful Eye",-1, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Normal, Logic.Override),
                new Buff("Cooling Vapor",46444, BuffSource.Engineer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b1/Coolant_Blast.png"),
                new Buff("Photon Wall Deployed",46094, BuffSource.Engineer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/ea/Photon_Wall.png"),
                new Buff("Spectrum Shield",43066, BuffSource.Engineer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/29/Spectrum_Shield.png"),
                new Buff("Gear Shield",5997, BuffSource.Engineer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/ca/Gear_Shield.png"),
                //Transforms
                //new Boon("Rampage",-1, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Normal, Logic.Override),
                new Buff("Photon Forge",43708, BuffSource.Engineer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/dd/Engage_Photon_Forge.png"),
                //Traits
                new Buff("Laser's Edge",44414, BuffSource.Engineer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/5d/Laser%27s_Edge.png",0 , 97950),
                new Buff("Afterburner",42210, BuffSource.Engineer, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/51/Solar_Focusing_Lens.png"),
                new Buff("Iron Blooded",49065, BuffSource.Engineer, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/1e/Iron_Blooded.png"),
                new Buff("Streamlined Kits",18687, BuffSource.Engineer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/cb/Streamlined_Kits.png"),
                new Buff("Kinetic Charge",45781, BuffSource.Engineer, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e0/Kinetic_Battery.png"),
                new Buff("Pinpoint Distribution", 38333, BuffSource.Engineer, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/b/bf/Pinpoint_Distribution.png"),
                new Buff("Heat Therapy",40694, BuffSource.Engineer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/34/Heat_Therapy.png"),
                new Buff("Overheat", 40397, BuffSource.Engineer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/4b/Overheat.png"),
                new Buff("Thermal Vision", 51389, BuffSource.Engineer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8a/Skilled_Marksman.png"),

        };

        private static readonly List<Buff> _ranger = new List<Buff>
        {

                new Buff("Celestial Avatar", 31508, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/59/Celestial_Avatar.png"),
                new Buff("Counterattack",14509, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/c1/Counterattack.png"),
                //signets
                new Buff("Signet of Renewal",41147, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/11/Signet_of_Renewal.png"),
                new Buff("Signet of Stone (Passive)",12627, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/63/Signet_of_Stone.png"),
                new Buff("Signet of the Hunt (Passive)",12626, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/df/Signet_of_the_Hunt.png"),
                new Buff("Signet of the Wild",12518, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/23/Signet_of_the_Wild.png"),
                new Buff("Signet of the Wild (Pet)",12636, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/23/Signet_of_the_Wild.png"),
                new Buff("Signet of Stone (Active)",12543, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/63/Signet_of_Stone.png"),
                new Buff("Signet of the Hunt (Active)",12541, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/df/Signet_of_the_Hunt.png"),
                //spirits
                // new Boon("Water Spirit (old)", 50386, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/0/06/Water_Spirit.png/33px-Water_Spirit.png"),
                new Buff("Frost Spirit", 12544, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png", 0, 88541),
                new Buff("Sun Spirit", 12540, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/d/dd/Sun_Spirit.png/33px-Sun_Spirit.png", 0, 88541),
                new Buff("Stone Spirit", 12547, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/3/35/Stone_Spirit.png/20px-Stone_Spirit.png", 0, 88541),
                //new Boon("Storm Spirit (old)", 50381, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/2/25/Storm_Spirit.png/30px-Storm_Spirit.png"),
                //reworked
                new Buff("Water Spirit", 50386, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/0/06/Water_Spirit.png/33px-Water_Spirit.png", 88541, ulong.MaxValue),
                new Buff("Frost Spirit", 50421, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png", 88541, ulong.MaxValue),
                new Buff("Sun Spirit", 50413, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/d/dd/Sun_Spirit.png/33px-Sun_Spirit.png", 88541, ulong.MaxValue),
                new Buff("Stone Spirit", 50415, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/3/35/Stone_Spirit.png/20px-Stone_Spirit.png", 88541, ulong.MaxValue),
                new Buff("Storm Spirit", 50381, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/thumb/2/25/Storm_Spirit.png/30px-Storm_Spirit.png", 88541, ulong.MaxValue),
                //skills
                new Buff("Attack of Opportunity",12574, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/47/Moment_of_Clarity.png"),
                new Buff("Call of the Wild",36781, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8d/Call_of_the_Wild.png",0 , 97950),
                new Buff("Call of the Wild",36781, BuffSource.Ranger, BuffType.Intensity, 3, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8d/Call_of_the_Wild.png",97950 , 102321),
                new Buff("Strength of the Pack!",12554, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/4b/%22Strength_of_the_Pack%21%22.png"),
                new Buff("Sic 'Em!",33902, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png"),
                new Buff("Sharpening Stones",12536, BuffSource.Ranger, BuffType.Intensity, 10, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/af/Sharpening_Stone.png"),
                new Buff("Ancestral Grace", 31584, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/4b/Ancestral_Grace.png"),
                new Buff("Glyph of Empowerment", 31803, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/d/d7/Glyph_of_the_Stars.png", 0 , 96406),
                new Buff("Glyph of Unity", 31385, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b1/Glyph_of_Unity.png"),
                new Buff("Glyph of Unity (CA)", 31556, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/4/4c/Glyph_of_Unity_%28Celestial_Avatar%29.png"),
                new Buff("Glyph of the Stars", 55048, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/d/d7/Glyph_of_the_Stars.png", 96406, ulong.MaxValue),
                new Buff("Dolyak Stance",41815, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/7/71/Dolyak_Stance.png"),
                new Buff("Griffon Stance",46280, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/9/98/Griffon_Stance.png"),
                new Buff("Moa Stance",45038, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/6/66/Moa_Stance.png"),
                new Buff("Vulture Stance",44651, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/8/8f/Vulture_Stance.png"),
                new Buff("Bear Stance",40045, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/f/f0/Bear_Stance.png"),
                new Buff("One Wolf Pack",44139, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/3/3b/One_Wolf_Pack.png"),
                new Buff("Sharpen Spines",43266, BuffSource.Ranger, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/95/Sharpen_Spines.png"),
                //traits
                new Buff("Spotter", 14055, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/b/b0/Spotter.png"),
                new Buff("Opening Strike",13988, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/44/Opening_Strike_%28effect%29.png"),
                new Buff("Quick Draw",29703, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/39/Quick_Draw.png"),
                new Buff("Light on your Feet",30673, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/22/Light_on_your_Feet.png"),
                new Buff("Natural Mender",30449, BuffSource.Ranger, BuffType.Intensity, 10, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e9/Natural_Mender.png"),
                new Buff("Lingering Light",32248, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/5d/Lingering_Light.png"),
                new Buff("Deadly",44932, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/94/Deadly_%28Archetype%29.png"),
                new Buff("Ferocious",41720, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e9/Ferocious_%28Archetype%29.png"),
                new Buff("Supportive",40069, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/32/Supportive_%28Archetype%29.png"),
                new Buff("Versatile",44693, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/bb/Versatile_%28Archetype%29.png"),
                new Buff("Stout",40272, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/80/Stout_%28Archetype%29.png"),
                new Buff("Unstoppable Union",44439, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b2/Unstoppable_Union.png"),
                new Buff("Twice as Vicious",45600, BuffSource.Ranger, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png"),
        };

        private static readonly List<Buff> _thief = new List<Buff>
        {
                //signets
                new Buff("Signet of Malice",13049, BuffSource.Thief, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/ae/Signet_of_Malice.png"),
                new Buff("Assassin's Signet (Passive)",13047, BuffSource.Thief, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/23/Assassin%27s_Signet.png"),
                new Buff("Assassin's Signet (Active)",44597, BuffSource.Thief, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/23/Assassin%27s_Signet.png"),
                new Buff("Infiltrator's Signet",13063, BuffSource.Thief, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8e/Infiltrator%27s_Signet.png"),
                new Buff("Signet of Agility",13061, BuffSource.Thief, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/1d/Signet_of_Agility.png"),
                new Buff("Signet of Shadows",13059, BuffSource.Thief, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/17/Signet_of_Shadows.png"),
                //venoms // src is always the user, makes generation data useless
                new Buff("Skelk Venom",21780, BuffSource.Thief, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/75/Skelk_Venom.png", 0, 97950),
                new Buff("Ice Drake Venom",13095, BuffSource.Thief, BuffType.Intensity, 4, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/7b/Ice_Drake_Venom.png", 0, 97950),
                new Buff("Devourer Venom", 13094, BuffSource.Thief, BuffType.Intensity, 2, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/4d/Devourer_Venom.png", 0, 97950),
                new Buff("Skale Venom", 13054, BuffSource.Thief, BuffType.Intensity, 4, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/14/Skale_Venom.png", 0, 97950),
                new Buff("Spider Venom",13036, BuffSource.Thief, BuffType.Intensity, 6, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/39/Spider_Venom.png", 0, 97950),
                new Buff("Basilisk Venom", 13133, BuffSource.Thief, BuffType.Intensity, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/3a/Basilisk_Venom.png", 0, 97950),
                new Buff("Skelk Venom",21780, BuffSource.Thief, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/75/Skelk_Venom.png", 97950, ulong.MaxValue),
                new Buff("Ice Drake Venom",13095, BuffSource.Thief, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/7b/Ice_Drake_Venom.png", 97950, ulong.MaxValue),
                new Buff("Devourer Venom", 13094, BuffSource.Thief, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/4d/Devourer_Venom.png", 97950, ulong.MaxValue),
                new Buff("Skale Venom", 13054, BuffSource.Thief, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/14/Skale_Venom.png", 97950, ulong.MaxValue),
                new Buff("Spider Venom",13036, BuffSource.Thief, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/39/Spider_Venom.png", 97950, ulong.MaxValue),
                new Buff("Basilisk Venom", 13133, BuffSource.Thief, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/3a/Basilisk_Venom.png", 97950, ulong.MaxValue),
                //physical
                new Buff("Palm Strike",30423, BuffSource.Thief, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
                new Buff("Pulmonary Impact",30510, BuffSource.Thief, BuffType.Intensity, 2, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
                //weapon
                new Buff("Infiltration",13135, BuffSource.Thief, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override,"https://wiki.guildwars2.com/images/6/62/Infiltrator%27s_Return.png"),
                //transforms
                new Buff("Dagger Storm",13134, BuffSource.Thief, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/c0/Dagger_Storm.png"),
                new Buff("Kneeling",42869, BuffSource.Thief, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/56/Kneel.png"),
                //traits
                //new Boon("Deadeye's Gaze",46333, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                //new Boon("Maleficent Seven",43606, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Buff("Hidden Killer",42720, BuffSource.Thief, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/ec/Hidden_Killer.png"),
                new Buff("Lead Attacks",34659, BuffSource.Thief, BuffType.Intensity, 15, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/01/Lead_Attacks.png"),
                new Buff("Instant Reflexes",34283, BuffSource.Thief, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/7d/Instant_Reflexes.png"),
                new Buff("Lotus Training", 32200, BuffSource.Thief, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/ea/Lotus_Training.png"),
                new Buff("Unhindered Combatant", 32931, BuffSource.Thief, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a1/Unhindered_Combatant.png"),
                new Buff("Bounding Dodger", 33162, BuffSource.Thief, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/30/Bounding_Dodger.png"),
                new Buff("Weakening Strikes", 34081, BuffSource.Thief, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/7c/Weakening_Strikes.png", 96406, ulong.MaxValue),

        };

        private static readonly List<Buff> _necromancer = new List<Buff>
        {
            
                //forms
                new Buff("Lich Form",10631, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/ab/Lich_Form.png"),
                new Buff("Death Shroud", 790, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/f5/Death_Shroud.png"),
                new Buff("Reaper's Shroud", 29446, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/11/Reaper%27s_Shroud.png"),
                //signets
                new Buff("Signet of Vampirism (Passive)",21761, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/73/Signet_of_Vampirism.png"),
                new Buff("Signet of Vampirism (Active)",21765, BuffSource.Necromancer, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/73/Signet_of_Vampirism.png"),
                new Buff("Lesser Signet of Vampirism",29799, BuffSource.Necromancer, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/73/Signet_of_Vampirism.png"),
                new Buff("Signet of Vampirism (Shroud)",43885, BuffSource.Necromancer, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/73/Signet_of_Vampirism.png"),
                new Buff("Plague Signet (Passive)",10630, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/c5/Plague_Signet.png"),
                new Buff("Plague Signet (Shroud)",44164, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/c5/Plague_Signet.png"),
                new Buff("Signet of Spite (Passive)",10621, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/df/Signet_of_Spite.png"),
                new Buff("Signet of Spite (Shroud)",43772, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/df/Signet_of_Spite.png"),
                new Buff("Signet of the Locust (Passive)",10614, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a3/Signet_of_the_Locust.png"),
                new Buff("Signet of the Locust (Shroud)",40283, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a3/Signet_of_the_Locust.png"),
                new Buff("Signet of Undeath (Passive)",10610, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/9c/Signet_of_Undeath.png"),
                new Buff("Signet of Undeath (Shroud)",40583, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/9c/Signet_of_Undeath.png"),
                //skills
                new Buff("Spectral Walk",15083, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/33/Spectral_Walk.png"),
                new Buff("Spectral Armor",10582, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d1/Spectral_Armor.png"),
                new Buff("Infusing Terror", 30129, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/db/Infusing_Terror.png"),
                new Buff("Locust Swarm", 10567, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/77/Locust_Swarm.png"),
                //new Boon("Sand Cascade", 43759, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/1e/Sand_Cascade.png"),
                //traits
                new Buff("Corrupter's Defense",30845, BuffSource.Necromancer, BuffType.Intensity, 10, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/11/Corrupter%27s_Fervor.png", 0, 99526),
                new Buff("Death's Carapace",30845, BuffSource.Necromancer, BuffType.Intensity, 30, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/58/Death%27s_Carapace.png", 99526, ulong.MaxValue),
                new Buff("Flesh of the Master",13810, BuffSource.Necromancer, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e9/Flesh_of_the_Master.png", 0, 99526),
                new Buff("Vampiric Aura", 30285, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/d/da/Vampiric_Presence.png"),
                new Buff("Last Rites",29726, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/1/1a/Last_Rites_%28effect%29.png"),
                new Buff("Sadistic Searing",43626, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/dd/Sadistic_Searing.png"),
                new Buff("Soul Barbs",53489, BuffSource.Necromancer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/bd/Soul_Barbs.png"),
        };

        private static readonly List<Buff> _mesmer = new List<Buff>
        {
            
                //signets
                new Buff("Signet of the Ether", 21751, BuffSource.Mesmer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/7a/Signet_of_the_Ether.png"),
                new Buff("Signet of Domination",10231, BuffSource.Mesmer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/3b/Signet_of_Domination.png"),
                new Buff("Signet of Illusions",10246, BuffSource.Mesmer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/ce/Signet_of_Illusions.png"),
                new Buff("Signet of Inspiration",10235, BuffSource.Mesmer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/ed/Signet_of_Inspiration.png"),
                new Buff("Signet of Midnight",10233, BuffSource.Mesmer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/24/Signet_of_Midnight.png"),
                new Buff("Signet of Humility",30739, BuffSource.Mesmer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b5/Signet_of_Humility.png"),
                //skills
                new Buff("Distortion",10243, BuffSource.Mesmer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/22/Distortion.png"),
                new Buff("Blur", 10335 , BuffSource.Mesmer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/2/22/Distortion.png"),
                new Buff("Mirror",10357, BuffSource.Mesmer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b8/Mirror.png"),
                new Buff("Echo",29664, BuffSource.Mesmer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/ce/Echo.png"),
                //new Boon("Illusion of Life",-1, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Normal, Logic.Override),
                //new Boon("Time Block",30134, BoonSource.Mesmer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8d/Deja_Vu.png"),
                new Buff("Time Echo",29582, BuffSource.Mesmer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/8/8d/Deja_Vu.png"),
                new Buff("Illusionary Counter",10278, BuffSource.Mesmer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e5/Illusionary_Counter.png"),
                new Buff("Time Anchored",30136, BuffSource.Mesmer, BuffType.Duration, 3, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/db/Continuum_Split.png"),
                new Buff("Illusionary Riposte",10279, BuffSource.Mesmer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/91/Illusionary_Riposte.png"),
                new Buff("Illusionary Leap",10353, BuffSource.Mesmer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/18/Illusionary_Leap.png"),
                //traits
                new Buff("Fencer's Finesse", 30426 , BuffSource.Mesmer, BuffType.Intensity, 10, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e7/Fencer%27s_Finesse.png"),
                new Buff("Illusionary Defense",49099, BuffSource.Mesmer, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e0/Illusionary_Defense.png"),
                new Buff("Compounding Power",49058, BuffSource.Mesmer, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/e/e5/Compounding_Power.png"),
                new Buff("Phantasmal Force", 44691 , BuffSource.Mesmer, BuffType.Intensity, 25, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/5f/Mistrust.png"),
                new Buff("Mirage Cloak",40408, BuffSource.Mesmer, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a5/Mirage_Cloak_%28effect%29.png"),
        };

        private static readonly List<Buff> _elementalist = new List<Buff>
        {
            
                //signets
                new Buff("Signet of Restoration",739, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/dd/Signet_of_Restoration.png"),
                new Buff("Signet of Air",5590, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/92/Signet_of_Air.png"),
                new Buff("Signet of Earth",5592, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/ce/Signet_of_Earth.png"),
                new Buff("Signet of Fire",5544, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b0/Signet_of_Fire.png"),
                new Buff("Signet of Water",5591, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/fd/Signet_of_Water.png"),
                ///attunements
                // Fire
                new Buff("Fire Attunement", 5585, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b4/Fire_Attunement.png"),
                new Buff("Dual Fire Attunement", ProfHelper.FireDual, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b4/Fire_Attunement.png"),
                new Buff("Fire Water Attunement", ProfHelper.FireWater, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://i.imgur.com/ihqKuUJ.png"),
                new Buff("Fire Air Attunement", ProfHelper.FireAir, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://i.imgur.com/kKFJ8cT.png"),
                new Buff("Fire Earth Attunement", ProfHelper.FireEarth, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://i.imgur.com/T4187h0.png"),
                // Water
                new Buff("Water Attunement", 5586, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/31/Water_Attunement.png"),
                new Buff("Dual Water Attunement", ProfHelper.WaterDual, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/31/Water_Attunement.png"),
                new Buff("Water Fire Attunement", ProfHelper.WaterFire, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://i.imgur.com/vMUkzxH.png"),
                new Buff("Water Air Attunement", ProfHelper.WaterAir, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://i.imgur.com/5G5OFud.png"),
                new Buff("Water Earth Attunement", ProfHelper.WaterEarth, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://i.imgur.com/QKEtF2P.png"),
                // Air
                new Buff("Air Attunement", 5575, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/91/Air_Attunement.png"),
                new Buff("Dual Air Attunement", ProfHelper.AirDual, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/91/Air_Attunement.png"),
                new Buff("Air Fire Attunement", ProfHelper.AirFire, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://i.imgur.com/vf68GJm.png"),
                new Buff("Air Water Attunement", ProfHelper.AirWater, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://i.imgur.com/Tuj5Sro.png"),
                new Buff("Air Earth Attunement", ProfHelper.AirEarth, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://i.imgur.com/lHcOSwk.png"),
                // Earth
                new Buff("Earth Attunement", 5580, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a8/Earth_Attunement.png"),
                new Buff("Dual Earth Attunement", ProfHelper.EarthDual, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a8/Earth_Attunement.png"),
                new Buff("Earth Fire Attunement", ProfHelper.EarthFire, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://i.imgur.com/aJWvE0I.png"),
                new Buff("Earth Water Attunement", ProfHelper.EarthWater, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://i.imgur.com/jtjj2TG.png"),
                new Buff("Earth Air Attunement", ProfHelper.EarthAir, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://i.imgur.com/4Eti7Pb.png"),
                //forms
                new Buff("Mist Form",5543, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/1/1b/Mist_Form.png"),
                new Buff("Ride the Lightning",5588, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/5/59/Ride_the_Lightning.png"),
                new Buff("Vapor Form",5620, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/6c/Vapor_Form.png"),
                new Buff("Tornado",5534, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/40/Tornado.png"),
                //new Boon("Whirlpool", -1,BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Normal, Logic.Override),
                //conjures
                new Buff("Conjure Earth Shield", 15788, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/7/7a/Conjure_Earth_Shield.png"),
                new Buff("Conjure Flame Axe", 15789, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/a/a1/Conjure_Flame_Axe.png"),
                new Buff("Conjure Frost Bow", 15790, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/c/c3/Conjure_Frost_Bow.png"),
                new Buff("Conjure Lightning Hammer", 15791, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/1/1f/Conjure_Lightning_Hammer.png"),
                new Buff("Conjure Fiery Greatsword", 15792, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/e/e2/Conjure_Fiery_Greatsword.png"),
                //skills
                new Buff("Arcane Power",5582, BuffSource.Elementalist, BuffType.Intensity, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/72/Arcane_Power.png"),
                new Buff("Primordial Stance",42086, BuffSource.Elementalist, BuffType.Intensity, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/3a/Primordial_Stance.png"),
                new Buff("Unravel",42683, BuffSource.Elementalist, BuffType.Intensity, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/4/4b/Unravel.png"),
                new Buff("Arcane Shield",5640, BuffSource.Elementalist, BuffType.Intensity, 3, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/9/9d/Arcane_Shield.png"),
                new Buff("Renewal of Fire",5764, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/6/63/Renewal_of_Fire.png"),
                new Buff("Glyph of Elemental Power (Fire)",5739, BuffSource.Elementalist, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/f/f2/Glyph_of_Elemental_Power_%28fire%29.png"),
                new Buff("Glyph of Elemental Power (Water)",5741, BuffSource.Elementalist, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/bf/Glyph_of_Elemental_Power_%28water%29.png"),
                new Buff("Glyph of Elemental Power (Air)",5740, BuffSource.Elementalist, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/cb/Glyph_of_Elemental_Power_%28air%29.png"),
                new Buff("Glyph of Elemental Power (Earth)",5742, BuffSource.Elementalist, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/0/0a/Glyph_of_Elemental_Power_%28earth%29.png"),
                new Buff("Rebound",31337, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/0/03/%22Rebound%21%22.png"),
                new Buff("Rock Barrier",34633, BuffSource.Elementalist, BuffType.Intensity, 5, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/dd/Rock_Barrier.png"),//750?
                new Buff("Magnetic Wave",15794, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/32/Magnetic_Wave.png"),
                new Buff("Obsidian Flesh",5667, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/c/c1/Obsidian_Flesh.png"),
                new Buff("Grinding Stones",51658, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/3/3d/Grinding_Stones.png"),
                new Buff("Static Charge",31487, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.OffensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/4/4b/Overload_Air.png"),
                //traits
                new Buff("Harmonious Conduit",31353, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/b/b3/Harmonious_Conduit.png", 0, 99526),
                new Buff("Transcendent Tempest",31353, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/ac/Transcendent_Tempest_%28effect%29.png", 99526, ulong.MaxValue),
                new Buff("Fresh Air",34241, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/d/d8/Fresh_Air.png"),
                new Buff("Soothing Mist", 5587, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.DefensiveBuffTable, Logic.Override, "https://wiki.guildwars2.com/images/f/f7/Soothing_Mist.png"),
                new Buff("Weaver's Prowess",42061, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/7/75/Weaver%27s_Prowess.png"),
                new Buff("Elements of Rage",42416, BuffSource.Elementalist, BuffType.Duration, 1, BuffNature.GraphOnlyBuff, Logic.Override, "https://wiki.guildwars2.com/images/a/a2/Elements_of_Rage.png"),
        };

        private static readonly List<Buff> _consumables = new List<Buff>
        {

                new Buff("Malnourished",46587, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/6/67/Malnourished.png"),
                new Buff("Plate of Truffle Steak",9769, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/4/4c/Plate_of_Truffle_Steak.png"),
                new Buff("Bowl of Sweet and Spicy Butternut Squash Soup",17825, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/d/df/Bowl_of_Sweet_and_Spicy_Butternut_Squash_Soup.png"),
                new Buff("Bowl Curry Butternut Squash Soup",9829, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/1/16/Bowl_of_Curry_Butternut_Squash_Soup.png"),
                new Buff("Red-Lentil Saobosa",46273, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/a/a8/Red-Lentil_Saobosa.png"),
                new Buff("Super Veggie Pizza",10008, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/6/66/Super_Veggie_Pizza.png"),
                new Buff("Rare Veggie Pizza",10009, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/a/a0/Rare_Veggie_Pizza.png"),
                new Buff("Bowl of Garlic Kale Sautee",-1, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/0/04/Bowl_of_Garlic_Kale_Sautee.png"),
                new Buff("Koi Cake",-1, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/1/14/Koi_Cake.png"),
                new Buff("Prickly Pear Pie",24800, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/0/0a/Prickly_Pear_Pie.png"),
                new Buff("Bowl of Nopalitos Sauté",-1, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/f/f1/Bowl_of_Nopalitos_Saut%C3%A9.png"),
                new Buff("Loaf of Candy Cactus Cornbread",24797, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/b/b2/Loaf_of_Candy_Cactus_Cornbread.png"),
                new Buff("Delicious Rice Ball",26529, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/5d/Delicious_Rice_Ball.png"),
                new Buff("Slice of Allspice Cake",33792, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/1/13/Slice_of_Allspice_Cake.png"),
                new Buff("Fried Golden Dumpling",26530, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/1/14/Fried_Golden_Dumpling.png"),
                new Buff("Bowl of Seaweed Salad",10080, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/1/1c/Bowl_of_Seaweed_Salad.png"),
                new Buff("Bowl of Orrian Truffle and Meat Stew",10096, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/b/b8/Bowl_of_Orrian_Truffle_and_Meat_Stew.png"),
                new Buff("Plate of Mussels Gnashblade",33476, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/7/7b/Plate_of_Mussels_Gnashblade.png"),
                new Buff("Spring Roll",26534, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/d/da/Spring_Roll.png"),
                new Buff("Plate of Beef Rendang",49686, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/d/d0/Plate_of_Beef_Rendang.png"),
                new Buff("Dragon's Revelry Starcake",19451, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/2/2b/Dragon%27s_Revelry_Starcake.png"),
                new Buff("Avocado Smoothie",50091, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/8/83/Avocado_Smoothie.png"),
                new Buff("Carrot Souffle",-1, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/7/71/Carrot_Souffl%C3%A9.png"), //same as Dragon's_Breath_Bun
                new Buff("Plate of Truffle Steak Dinner",-1, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/9/92/Plate_of_Truffle_Steak_Dinner.png"), //same as Dragon's Breath Bun
                new Buff("Dragon's Breath Bun",9750, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/53/Dragon%27s_Breath_Bun.png"),
                new Buff("Karka Egg Omelet",9756, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/9/9e/Karka_Egg_Omelet.png"),
                new Buff("Steamed Red Dumpling",26536, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/8/8c/Steamed_Red_Dumpling.png"),
                new Buff("Saffron Stuffed Mushroom",-1, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/e/e2/Saffron_Stuffed_Mushroom.png"), //same as Karka Egg Omelet
                new Buff("Soul Pastry",53222, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/2/2c/Soul_Pastry.png"),
                new Buff("Bowl of Fire Meat Chili",10119, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/b/ba/Bowl_of_Fire_Meat_Chili.png"),
                new Buff("Plate of Fire Flank Steak",9765, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/2/27/Plate_of_Fire_Flank_Steak.png"),
                new Buff("Plate of Orrian Steak Frittes",9773, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/4/4d/Plate_of_Orrian_Steak_Frittes.png"),
                new Buff("Spicier Flank Steak",9764, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/0/01/Spicier_Flank_Steak.png"),
                new Buff("Mango Pie",9993, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/3/3d/Mango_Pie.png"),
                // UTILITIES 
                // 1h versions have the same ID as 30 min versions 
                new Buff("Diminished",46668, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/7/71/Diminished.png"),
                new Buff("Superior Sharpening Stone",9963, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                new Buff("Potent Superior Sharpening Stone",-1, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                new Buff("Master Maintenance Oil",9968, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Buff("Potent Master Maintenance Oil",-1, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Buff("Tuning Icicle",34206, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/7/75/Tuning_Icicle.png"),
                new Buff("Master Tuning Crystal",9967, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/58/Master_Tuning_Crystal.png"),
                new Buff("Potent Master Tuning Crystal",-1, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/58/Master_Tuning_Crystal.png"),
                new Buff("Toxic Sharpening Stone",21826, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/d/db/Toxic_Sharpening_Stone.png"),
                new Buff("Toxic Maintenance Oil",21827, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/a/a6/Toxic_Maintenance_Oil.png"),
                new Buff("Toxic Focusing Crystal",21828, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/d/de/Toxic_Focusing_Crystal.png"),
                new Buff("Magnanimous Maintenance Oil",38605, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/53/Magnanimous_Maintenance_Oil.png"),
                new Buff("Peppermint Oil",34187, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/b/bc/Peppermint_Oil.png"),
                new Buff("Potent Lucent Oil",53374, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/1/16/Potent_Lucent_Oil.png"),
                new Buff("Enhanced Lucent Oil",53304, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/e/ee/Enhanced_Lucent_Oil.png"),
                new Buff("Furious Maintenance Oil",25881, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Buff("Furious Sharpening Stone",25882, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                new Buff("Bountiful Maintenance Oil",25879, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Buff("Tin of Fruitcake",34211, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/a/af/Tin_of_Fruitcake.png"),
                new Buff("Holographic Super Cheese",50320, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/f/fa/Holographic_Super_Cheese.png"),
                new Buff("Writ of Masterful Malice",33836, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/2/20/Writ_of_Masterful_Malice.png"),
                new Buff("Writ of Masterful Strength",33297, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/2/2b/Writ_of_Masterful_Strength.png"),
                new Buff("Powerful Potion of Flame Legion Slaying",9925, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/e/e2/Powerful_Potion_of_Flame_Legion_Slaying.png"),
                new Buff("Powerful Potion of Halloween Slaying",15279, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/f/fe/Powerful_Potion_of_Halloween_Slaying.png"),
                new Buff("Powerful Potion of Centaur Slaying",9845, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/3/3b/Powerful_Potion_of_Centaur_Slaying.png"),
                new Buff("Powerful Potion of Krait Slaying",9885, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/b/b4/Powerful_Potion_of_Krait_Slaying.png"),
                new Buff("Powerful Potion of Ogre Slaying",9877, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/b/b5/Powerful_Potion_of_Ogre_Slaying.png"),
                new Buff("Powerful Potion of Elemental Slaying",9893, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/5/5f/Powerful_Potion_of_Elemental_Slaying.png"),
                new Buff("Powerful Potion of Destroyer Slaying",9869, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/b/bd/Powerful_Potion_of_Destroyer_Slaying.png"),
                new Buff("Powerful Potion of Nightmare Court Slaying",9941, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/7/74/Powerful_Potion_of_Nightmare_Court_Slaying.png"),
                new Buff("Powerful Potion of Slaying Scarlet's Armies",23228, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/e/ee/Powerful_Potion_of_Demon_Slaying.png"),
                new Buff("Powerful Potion of Undead Slaying",9837, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/b/bd/Powerful_Potion_of_Undead_Slaying.png"),
                new Buff("Powerful Potion of Dredge Slaying",9949, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/9/9a/Powerful_Potion_of_Dredge_Slaying.png"),
                new Buff("Powerful Potion of Inquest Slaying",9917, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/f/fb/Powerful_Potion_of_Inquest_Slaying.png"),
                new Buff("Powerful Potion of Demon Slaying",9901, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/e/ee/Powerful_Potion_of_Demon_Slaying.png"),
                new Buff("Powerful Potion of Grawl Slaying",9853, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/1/15/Powerful_Potion_of_Grawl_Slaying.png"),
                new Buff("Powerful Potion of Sons of Svanir Slaying",9909, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/3/33/Powerful_Potion_of_Sons_of_Svanir_Slaying.png"),
                new Buff("Powerful Potion of Outlaw Slaying",9933, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/e/ec/Powerful_Potion_of_Outlaw_Slaying.png"),
                new Buff("Powerful Potion of Ice Brood Slaying",9861, BuffSource.Item, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override, "https://wiki.guildwars2.com/images/0/0d/Powerful_Potion_of_Ice_Brood_Slaying.png"),
                // new Boon("Hylek Maintenance Oil",9968, BoonSource.Item, BoonType.Duration, 1, BoonEnum.Utility, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"), when wiki says "same stats" its literally the same buff
                // Fractals 
                new Buff("Fractal Mobility", 33024, BuffSource.Mixed, BuffType.Intensity, 5, BuffNature.Consumable, Logic.Override,"https://wiki.guildwars2.com/images/thumb/2/22/Mist_Mobility_Potion.png/40px-Mist_Mobility_Potion.png"),
                new Buff("Fractal Defensive", 32134, BuffSource.Mixed, BuffType.Intensity, 5, BuffNature.Consumable, Logic.Override,"https://wiki.guildwars2.com/images/thumb/e/e6/Mist_Defensive_Potion.png/40px-Mist_Defensive_Potion.png"),
                new Buff("Fractal Offensive", 32473, BuffSource.Mixed, BuffType.Intensity, 5, BuffNature.Consumable, Logic.Override,"https://wiki.guildwars2.com/images/thumb/8/8d/Mist_Offensive_Potion.png/40px-Mist_Offensive_Potion.png"),
                // Ascended Food
                new Buff("Cilantro Lime Sous-Vide Steak", 57244, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override,"https://wiki.guildwars2.com/images/6/65/Cilantro_Lime_Sous-Vide_Steak.png"),
                new Buff("Peppercorn and Veggie Flatbread", 57382, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override,"https://wiki.guildwars2.com/images/9/9d/Peppercorn_and_Veggie_Flatbread.png"),
                new Buff("Bowl of Fruit Salad with Mint Garnish", 57100, BuffSource.Mixed, BuffType.Duration, 1, BuffNature.Consumable, Logic.Override,"https://wiki.guildwars2.com/images/4/47/Bowl_of_Fruit_Salad_with_Mint_Garnish.png"),

        };

        public static List<List<Buff>> AllBuffs = new List<List<Buff>>()
            {
                _boons,
                _conditions,
                _commons,
                _gear,
                _consumables,
                _fightSpecific,
                _revenant,
                _warrior,
                _guardian,
                _ranger,
                _thief,
                _engineer,
                _mesmer,
                _necromancer,
                _elementalist
        };

        public AbstractBuffSimulator CreateSimulator(ParsedLog log)
        {
            if (!log.CombatData.HasStackIDs)
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
                    case Logic.Override:
                        logicToUse = new OverrideLogic();
                        break;
                    case Logic.Unknown:
                    default:
                        throw new InvalidOperationException("Error Encountered: Cannot simulate unknown/custom buffs");
                }
                switch (Type)
                {
                    case BuffType.Intensity: return new BuffSimulatorIntensity(Capacity, log, logicToUse);
                    case BuffType.Duration: return new BuffSimulatorDuration(Capacity, log, logicToUse);
                    case BuffType.Unknown:
                    default: throw new InvalidOperationException("Error Encountered: Cannot simulate typeless boons");
                }
            }
            switch (Type)
            {
                case BuffType.Intensity: return new BuffSimulatorIDIntensity(log);
                case BuffType.Duration: return new BuffSimulatorIDDuration(log);
                case BuffType.Unknown:
                default: throw new InvalidOperationException("Error Encountered: Cannot simulate typeless boons");
            }
        }

        public static BuffSourceFinder GetBuffSourceFinder(ulong version, HashSet<long> boonIds)
        {
            if (version > 99526)
            {
                return new BuffSourceFinder01102019(boonIds);
            }
            if (version > 95112)
            {
                return new BuffSourceFinder05032019(boonIds);
            }
            return new BuffSourceFinder11122018(boonIds);
        }
    }
}
