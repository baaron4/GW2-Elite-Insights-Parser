using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;
using static GW2EIParser.Parser.ParseEnum;

namespace GW2EIParser.EIData
{
    public class BuffsContainer
    {

        private readonly List<Buff> _boons = new List<Buff>
        {
                new Buff("Might", 740, GeneralHelper.Source.Common, BuffStackType.Stacking, 25, BuffNature.Boon, "https://wiki.guildwars2.com/images/7/7c/Might.png"),
                new Buff("Fury", 725, GeneralHelper.Source.Common, BuffStackType.Queue, 9, BuffNature.Boon, "https://wiki.guildwars2.com/images/4/46/Fury.png"),//or 3m and 30s
                new Buff("Quickness", 1187, GeneralHelper.Source.Common, BuffStackType.Queue, 5, BuffNature.Boon, "https://wiki.guildwars2.com/images/b/b4/Quickness.png"),
                new Buff("Alacrity", 30328, GeneralHelper.Source.Common, BuffStackType.Queue, 9, BuffNature.Boon, "https://wiki.guildwars2.com/images/4/4c/Alacrity.png"),
                new Buff("Protection", 717, GeneralHelper.Source.Common, BuffStackType.Queue, 5, BuffNature.Boon, "https://wiki.guildwars2.com/images/6/6c/Protection.png"),
                new Buff("Regeneration", 718, GeneralHelper.Source.Common, BuffStackType.Regeneration, 5, BuffNature.Boon, "https://wiki.guildwars2.com/images/5/53/Regeneration.png"),
                new Buff("Vigor", 726, GeneralHelper.Source.Common, BuffStackType.Queue, 5, BuffNature.Boon, "https://wiki.guildwars2.com/images/f/f4/Vigor.png"),
                new Buff("Aegis", 743, GeneralHelper.Source.Common, BuffStackType.Queue, 9, BuffNature.Boon, "https://wiki.guildwars2.com/images/e/e5/Aegis.png"),
                new Buff("Stability", 1122, GeneralHelper.Source.Common, BuffStackType.StackingConditionalLoss, 25, BuffNature.Boon, "https://wiki.guildwars2.com/images/a/ae/Stability.png"),
                new Buff("Swiftness", 719, GeneralHelper.Source.Common, BuffStackType.Queue, 9, BuffNature.Boon, "https://wiki.guildwars2.com/images/a/af/Swiftness.png"),
                new Buff("Retaliation", 873, GeneralHelper.Source.Common, BuffStackType.Queue, 5, BuffNature.Boon, "https://wiki.guildwars2.com/images/5/53/Retaliation.png"),
                new Buff("Resistance", 26980, GeneralHelper.Source.Common, BuffStackType.Queue, 5, BuffNature.Boon, "https://wiki.guildwars2.com/images/4/4b/Resistance.png"),
                new Buff("Number of Boons", ProfHelper.NumberOfBoonsID, GeneralHelper.Source.Common, BuffStackType.Stacking, 0, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/44/Boon_Duration.png"),
        };

        private readonly List<Buff> _conditions = new List<Buff>
        {
                new Buff("Bleeding", 736, GeneralHelper.Source.Common, BuffStackType.Stacking, 1500, BuffNature.Condition, "https://wiki.guildwars2.com/images/3/33/Bleeding.png"),
                new Buff("Burning", 737, GeneralHelper.Source.Common, BuffStackType.Stacking, 1500, BuffNature.Condition, "https://wiki.guildwars2.com/images/4/45/Burning.png"),
                new Buff("Confusion", 861, GeneralHelper.Source.Common, BuffStackType.Stacking, 1500, BuffNature.Condition, "https://wiki.guildwars2.com/images/e/e6/Confusion.png"),
                new Buff("Poison", 723, GeneralHelper.Source.Common, BuffStackType.Stacking, 1500, BuffNature.Condition, "https://wiki.guildwars2.com/images/1/11/Poisoned.png"),
                new Buff("Torment", 19426, GeneralHelper.Source.Common, BuffStackType.Stacking, 1500, BuffNature.Condition, "https://wiki.guildwars2.com/images/0/08/Torment.png"),
                new Buff("Blind", 720, GeneralHelper.Source.Common, BuffStackType.Queue, 9, BuffNature.Condition, "https://wiki.guildwars2.com/images/3/33/Blinded.png"),
                new Buff("Chilled", 722, GeneralHelper.Source.Common, BuffStackType.Queue, 5, BuffNature.Condition, "https://wiki.guildwars2.com/images/a/a6/Chilled.png"),
                new Buff("Crippled", 721, GeneralHelper.Source.Common, BuffStackType.Queue, 5, BuffNature.Condition, "https://wiki.guildwars2.com/images/f/fb/Crippled.png"),
                new Buff("Fear", 791, GeneralHelper.Source.Common, BuffStackType.Queue, 5, BuffNature.Condition, "https://wiki.guildwars2.com/images/e/e6/Fear.png"),
                new Buff("Immobile", 727, GeneralHelper.Source.Common, BuffStackType.Queue, 3, BuffNature.Condition, "https://wiki.guildwars2.com/images/3/32/Immobile.png"),
                new Buff("Slow", 26766, GeneralHelper.Source.Common, BuffStackType.Queue, 9, BuffNature.Condition, "https://wiki.guildwars2.com/images/f/f5/Slow.png"),
                new Buff("Weakness", 742, GeneralHelper.Source.Common, BuffStackType.Queue, 5, BuffNature.Condition, "https://wiki.guildwars2.com/images/f/f9/Weakness.png"),
                new Buff("Taunt", 27705, GeneralHelper.Source.Common, BuffStackType.Queue, 5, BuffNature.Condition, "https://wiki.guildwars2.com/images/c/cc/Taunt.png"),
                new Buff("Vulnerability", 738, GeneralHelper.Source.Common, BuffStackType.Stacking, 25, BuffNature.Condition, "https://wiki.guildwars2.com/images/a/af/Vulnerability.png"),
                new Buff("Number of Conditions", ProfHelper.NumberOfConditionsID, GeneralHelper.Source.Common, BuffStackType.Stacking, 0, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/38/Condition_Duration.png"),
        };

        private readonly List<Buff> _commons = new List<Buff>
        {
                new Buff("Downed", 770, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/dd/Downed.png"),
                new Buff("Stealth", 13017, GeneralHelper.Source.Common, BuffStackType.Queue, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/19/Stealth.png"),
                new Buff("Hide in Shadows", 10269, GeneralHelper.Source.Common, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/19/Stealth.png"),
                new Buff("Revealed", 890, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/db/Revealed.png"),
                new Buff("Superspeed", 5974, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/1/1a/Super_Speed.png"),
                new Buff("Determined (762)", 762, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Determined (788)", 788, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Determined (895)", 895, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Determined (3892)", 3892, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Determined (31450)", 31450, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Determined (52271)", 52271, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Invulnerability (757)", 757, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Invulnerability (801)", 801, GeneralHelper.Source.Common, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Spawn Protection?", 34113, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Stun", 872, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/97/Stun.png"),
                new Buff("Daze", 833, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/79/Daze.png"),
                new Buff("Exposed", 48209, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/f/f4/Exposed_%28effect%29.png"),
                new Buff("Unblockable",36781, GeneralHelper.Source.Common, BuffStackType.StackingConditionalLoss, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/f0/Unblockable_%28effect%29.png",102321 , ulong.MaxValue),
                //Auras
                new Buff("Chaos Aura", 10332, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/ec/Chaos_Aura.png"),
                new Buff("Fire Aura", 5677, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/c/ce/Fire_Aura.png"),
                new Buff("Frost Aura", 5579, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/8/87/Frost_Aura_%28effect%29.png"),
                new Buff("Light Aura", 25518, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/5/5a/Light_Aura.png"),
                new Buff("Magnetic Aura", 5684, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/0b/Magnetic_Aura_%28effect%29.png"),
                new Buff("Shocking Aura", 5577, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/5/5d/Shocking_Aura_%28effect%29.png"),
                new Buff("Dark Aura", 39978, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/ef/Dark_Aura.png", 96406, ulong.MaxValue),
                //race
                new Buff("Take Root", 12459, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/b/b2/Take_Root.png"),
                new Buff("Become the Bear",12426, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/7/7e/Become_the_Bear.png"),
                new Buff("Become the Raven",12405, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/2/2c/Become_the_Raven.png"),
                new Buff("Become the Snow Leopard",12400, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/7/78/Become_the_Snow_Leopard.png"),
                new Buff("Become the Wolf",12393, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/f1/Become_the_Wolf.png"),
                new Buff("Avatar of Melandru", 12368, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/30/Avatar_of_Melandru.png"),
                new Buff("Power Suit",12326, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/89/Summon_Power_Suit.png"),
                new Buff("Reaper of Grenth", 12366, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/07/Reaper_of_Grenth.png"),
                new Buff("Charrzooka",43503, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/1/17/Charrzooka.png"),
                // Fractals 
                new Buff("Rigorous Certainty", 33652, GeneralHelper.Source.Common, BuffNature.DefensiveBuffTable,"https://wiki.guildwars2.com/images/6/60/Desert_Carapace.png"),
                //
                new Buff("Guild Item Research", 33833, GeneralHelper.Source.Common, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/c/c6/Guild_Magic_Find_Banner_Boost.png"),
        };

        private readonly List<Buff> _gear = new List<Buff>
        {
                new Buff("Sigil of Concentration", 33719, GeneralHelper.Source.Item, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b3/Superior_Sigil_of_Concentration.png",0 , 93543),
                new Buff("Superior Rune of the Monk", 53285, GeneralHelper.Source.Item, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/18/Superior_Rune_of_the_Monk.png", 93543, ulong.MaxValue),
                new Buff("Sigil of Corruption", 9374, GeneralHelper.Source.Item, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/18/Superior_Sigil_of_Corruption.png"),
                new Buff("Sigil of Life", 9386, GeneralHelper.Source.Item, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a7/Superior_Sigil_of_Life.png"),
                new Buff("Sigil of Perception", 9385, GeneralHelper.Source.Item, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/cc/Superior_Sigil_of_Perception.png"),
                new Buff("Sigil of Bloodlust", 9286, GeneralHelper.Source.Item, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/fb/Superior_Sigil_of_Bloodlust.png"),
                new Buff("Sigil of Bounty", 38588, GeneralHelper.Source.Item, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/f8/Superior_Sigil_of_Bounty.png"),
                new Buff("Sigil of Benevolence", 9398, GeneralHelper.Source.Item, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/59/Superior_Sigil_of_Benevolence.png"),
                new Buff("Sigil of Momentum", 22144, GeneralHelper.Source.Item, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/30/Superior_Sigil_of_Momentum.png"),
                new Buff("Sigil of the Stars", 46953, GeneralHelper.Source.Item, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/dc/Superior_Sigil_of_the_Stars.png"),
        };

        private readonly List<Buff> _fightSpecific = new List<Buff>
        {
                // Whisper of Jormalg
                new Buff("Whisper Teleport Out", 59223, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Whisper Teleport Back", 59054, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Frigid Vortex", 59105, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Chains of Frost Active", 59100, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Chains of Frost Application", 59120, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Brain Freeze", 59073, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 20, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                // Voice and Claw            
                new Buff("Enraged", 58619, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                // Fraenir of Jormag
                new Buff("Frozen", 58376, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7a/Bloodstone_Blessed.png" ),
                new Buff("Snowblind", 58276, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7a/Bloodstone_Blessed.png" ),
                // Twisted Castle
                new Buff("Spatial Distortion", 34918, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7a/Bloodstone_Blessed.png" ),
                new Buff("Madness", 35006, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 99, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/ee/Madness.png" ),
                new Buff("Still Waters", 35106, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/5c/Still_Waters_%28effect%29.png" ),
                new Buff("Soothing Waters", 34955, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8f/Soothing_Waters.png" ),
                new Buff("Chaotic Haze", 34963, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/48/Lava_Font.png" ),
                new Buff("Timed Bomb", 31485, GeneralHelper.Source.FightSpecific, BuffStackType.Queue, 1, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/91/Time_Bomb.png" ),
                // Deimos
                new Buff("Unnatural Signet",38224, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/20/Unnatural_Signet.png"),
                // KC
                new Buff("Compromised",35096, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/48/Compromised.png"),
                new Buff("Xera's Boon",35025, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/04/Xera%27s_Boon.png"),
                new Buff("Xera's Fury",35103, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/dd/Xera%27s_Fury.png"),
                new Buff("Statue Fixated",34912, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/dd/Xera%27s_Fury.png"),
                new Buff("Crimson Attunement (Orb)",35091, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/3e/Crimson_Attunement.png"),
                new Buff("Radiant Attunement (Orb)",35109, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/68/Radiant_Attunement.png"),
                new Buff("Magic Blast",35119, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 35, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a9/Magic_Blast_Intensity.png"),
                // Gorseval
                new Buff("Spirited Fusion",31722, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 99, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/eb/Spirited_Fusion.png"),
                new Buff("Protective Shadow", 31877, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/8/87/Protective_Shadow.png"),
                // Matthias
                new Buff("Blood Shield Abo",34376, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 18, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a6/Blood_Shield.png"),
                new Buff("Blood Shield",34518, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 18, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a6/Blood_Shield.png"),
                new Buff("Blood Fueled",34422, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 1, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Blood Fueled Abo",34428, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 15, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                // Qadim
                new Buff("Flame Armor",52568, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Fiery Surge",52588, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 99, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                // Soulless Horror
                new Buff("Necrosis",47414, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/47/Ichor.png"),
                // CA
                new Buff("Greatsword Power",52667 , GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/3b/Greatsword_Power_%28effect%29.png"),
                new Buff("Fractured - Enemy",53030, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/78/Branded_Aura.png"),
                new Buff("Fractured - Allied",52213, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 2, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/78/Branded_Aura.png"),
                new Buff("Conjured Shield",52754 , GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/83/Conjured_Shield_%28effect%29.png"),
                new Buff("Conjured Protection",52973 , GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/83/Bloodstone-Infused_shield.png"),
                new Buff("Shielded",53003 , GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/47/Golem-Powered_Shielding.png"),
                new Buff("Augmented Power",52074  , GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/47/Golem-Powered_Shielding.png"),
                new Buff("Scepter Lock-on",53075  , GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("CA Invul",52255 , GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Arm Up",52430 , GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                // Twin Largos
                //new Buff("Aquatic Detainment",52931 , GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Aquatic Aura (Kenut)",52211 , GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 80, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Aquatic Aura (Nikare)",52929 , GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 80, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Waterlogged",51935 , GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                // Slothasor
                new Buff("Narcolepsy", 34467, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                // VG
                new Buff("Blue Pylon Power", 31413, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/6/6e/Blue_Pylon_Power.png"),
                new Buff("Pylon Attunement: Red", 31695, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/9/94/Pylon_Attunement-_Red.png"),
                new Buff("Pylon Attunement: Blue", 31317, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/6/6e/Blue_Pylon_Power.png"),
                new Buff("Pylon Attunement: Green", 31852, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/a/aa/Pylon_Attunement-_Green.png"),
                new Buff("Unstable Pylon: Red", 31539, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/3/36/Unstable_Pylon_%28Red%29.png"),
                new Buff("Unstable Pylon: Blue", 31884, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/c/c3/Unstable_Pylon_%28Blue%29.png"),
                new Buff("Unstable Pylon: Green", 31828, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/9/9d/Unstable_Pylon_%28Green%29.png"),
                new Buff("Unbreakable", 34979, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 2, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/5/56/Xera%27s_Embrace.png"),
                // Trio
                new Buff("Not the Bees!", 34434, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/0/08/Throw_Jar.png"),
                new Buff("Targeted", 34392, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                // Dhuum
                new Buff("Spirit Transfrom", 48281, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 30, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/4/48/Compromised.png"),
                new Buff("Fractured Spirit", 46950, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/c/c3/Fractured_Spirit.png"),
                new Buff("Residual Affliction", 47476, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/1/12/Residual_Affliction.png"),
                new Buff("Arcing Affliction", 47646, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/f/f0/Arcing_Affliction.png"),
                // Adina
                new Buff("Pillar Pandemonium", 56204, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 99, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Radiant Blindness", 56593, GeneralHelper.Source.FightSpecific, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Diamond Palisade (Damage)", 56099, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Diamond Palisade", 56636, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Eroding Curse", 56440, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 99, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                // Sabir
                new Buff("Ion Shield", 56100, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 80, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Violent Currents", 56123, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/0/06/Violent_Currents.png"),
                new Buff("Repulsion Field", 56172, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Electrical Repulsion", 56391, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://render.guildwars2.com/file/5AF8170865B353E26127E10E34EFE8B90B9096D6/1451806.png"),
                new Buff("Electro-Repulsion", 56474, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/7/74/Unbridled_Chaos.png"),
                // Peerless Qadim
                new Buff("Erratic Energy", 56582, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/4/45/Unstable.png"),
                new Buff("Power Share", 56104, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 3, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Sapping Surge", 56118, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/6/6f/Guilt_Exploitation.png"),
                new Buff("Chaos Corrosion", 56182, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/f/fd/Fractured_%28effect%29.png"),
                new Buff("Peerless Fixated", 56510, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Magma Drop", 56475, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Kinetic Abundance", 56609, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/0/06/Values_Mastery.png"),
                new Buff("Unbridled Chaos", 56467, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking, 3, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/7/74/Unbridled_Chaos.png"),
                // Cairn        
                new Buff("Shared Agony", 38049, GeneralHelper.Source.FightSpecific, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/5/53/Shared_Agony.png"),
                new Buff("Unseen Burden", 38153, GeneralHelper.Source.FightSpecific, BuffStackType.Stacking , 99, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/b/b9/Unseen_Burden.png"),
        };

        private readonly List<Buff> _revenant = new List<Buff>
        {         
                //skills
                new Buff("Crystal Hibernation", 29303, GeneralHelper.Source.Herald, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/4/4a/Crystal_Hibernation.png"),
                new Buff("Vengeful Hammers", 27273, GeneralHelper.Source.Revenant, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/c/c8/Vengeful_Hammers.png"),
                new Buff("Rite of the Great Dwarf", 26596, GeneralHelper.Source.Revenant, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/6/69/Rite_of_the_Great_Dwarf.png"),
                new Buff("Embrace the Darkness", 28001, GeneralHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/51/Embrace_the_Darkness.png"),
                new Buff("Enchanted Daggers", 28557, GeneralHelper.Source.Revenant, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/fa/Enchanted_Daggers.png"),
                new Buff("Phase Traversal", 28395, GeneralHelper.Source.Revenant, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/f2/Phase_Traversal.png"),
                new Buff("Impossible Odds", 27581, GeneralHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/87/Impossible_Odds.png"),
                //facets
                new Buff("Facet of Light",27336, GeneralHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/18/Facet_of_Light.png"),
                new Buff("Facet of Light (Traited)",51690, GeneralHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/18/Facet_of_Light.png"), //Lingering buff with Draconic Echo trait
                new Buff("Infuse Light",27737, GeneralHelper.Source.Herald, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/6/60/Infuse_Light.png"),
                new Buff("Facet of Darkness",28036, GeneralHelper.Source.Herald, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/e4/Facet_of_Darkness.png"),
                new Buff("Facet of Darkness (Traited)",51695, GeneralHelper.Source.Herald, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/e4/Facet_of_Darkness.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Elements",28243, GeneralHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ce/Facet_of_Elements.png"),
                new Buff("Facet of Elements (Traited)",51706, GeneralHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ce/Facet_of_Elements.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Strength",27376, GeneralHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a8/Facet_of_Strength.png"),
                new Buff("Facet of Strength (Traited)",51700, GeneralHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a8/Facet_of_Strength.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Chaos",27983, GeneralHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c7/Facet_of_Chaos.png"),
                new Buff("Facet of Nature",29275, GeneralHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),
                new Buff("Facet of Nature (Traited)",51681, GeneralHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Nature-Assassin",51692, GeneralHelper.Source.Herald, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/c/cd/Facet_of_Nature%E2%80%95Assassin.png"),
                new Buff("Facet of Nature-Dragon",51674, GeneralHelper.Source.Herald, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/4/47/Facet_of_Nature%E2%80%95Dragon.png"),
                new Buff("Facet of Nature-Demon",51704, GeneralHelper.Source.Herald, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/f/ff/Facet_of_Nature%E2%80%95Demon.png"),
                new Buff("Facet of Nature-Dwarf",51677, GeneralHelper.Source.Herald, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/4/4c/Facet_of_Nature%E2%80%95Dwarf.png"),
                new Buff("Facet of Nature-Centaur",51699, GeneralHelper.Source.Herald, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/7/74/Facet_of_Nature%E2%80%95Centaur.png"),
                new Buff("Naturalistic Resonance", 29379, GeneralHelper.Source.Herald, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),
                //legends
                new Buff("Legendary Centaur Stance",27972, GeneralHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8a/Legendary_Centaur_Stance.png"),
                new Buff("Legendary Dragon Stance",27732, GeneralHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d5/Legendary_Dragon_Stance.png"),
                new Buff("Legendary Dwarf Stance",27205, GeneralHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b2/Legendary_Dwarf_Stance.png"),
                new Buff("Legendary Demon Stance",27928, GeneralHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d1/Legendary_Demon_Stance.png"),
                new Buff("Legendary Assassin Stance",27890, GeneralHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/02/Legendary_Assassin_Stance.png"),
                new Buff("Legendary Renegade Stance",44272, GeneralHelper.Source.Renegade, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/19/Legendary_Renegade_Stance.png"),
                //summons
                new Buff("Breakrazor's Bastion",44682, GeneralHelper.Source.Renegade, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/a/a7/Breakrazor%27s_Bastion.png"),
                new Buff("Razorclaw's Rage",41016, GeneralHelper.Source.Renegade, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/7/73/Razorclaw%27s_Rage.png"),
                new Buff("Soulcleave's Summit",45026, GeneralHelper.Source.Renegade, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/7/78/Soulcleave%27s_Summit.png"),
                //traits
                new Buff("Vicious Lacerations",29395, GeneralHelper.Source.Revenant, BuffStackType.Stacking, 3, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/cd/Vicious_Lacerations.png", 0, 102321),
                new Buff("Rising Momentum",51683, GeneralHelper.Source.Revenant, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8c/Rising_Momentum.png"),
                new Buff("Assassin's Presence", 26854, GeneralHelper.Source.Revenant, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/5/54/Assassin%27s_Presence.png"),
                new Buff("Expose Defenses", 48894, GeneralHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/5c/Mutilate_Defenses.png"),
                new Buff("Invoking Harmony",29025, GeneralHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/ec/Invoking_Harmony.png"),
                new Buff("Unyielding Devotion",55044, GeneralHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4f/Unyielding_Devotion.png", 96406, ulong.MaxValue),
                //new Boon("Selfless Amplification",29025, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/23/Selfless_Amplification.png"),
                new Buff("Hardening Persistence",28957, GeneralHelper.Source.Herald, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/0f/Hardening_Persistence.png"),
                new Buff("Soothing Bastion",34136, GeneralHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/04/Soothing_Bastion.png"),
                new Buff("Kalla's Fervor",42883, GeneralHelper.Source.Renegade, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png"),
                new Buff("Improved Kalla's Fervor",45614, GeneralHelper.Source.Renegade, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png"),
                new Buff("Battle Scars", 26646, GeneralHelper.Source.Revenant, BuffStackType.StackingConditionalLoss, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/30/Thrill_of_Combat.png", 102321, ulong.MaxValue),
        };

        private readonly List<Buff> _warrior = new List<Buff>
        {
                new Buff("Berserk",29502, GeneralHelper.Source.Berserker, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/4/44/Berserk.png"),
            //skills
                new Buff("Riposte",14434, GeneralHelper.Source.Warrior, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/d/de/Riposte.png"),
                new Buff("Flames of War", 31708, GeneralHelper.Source.Berserker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/6f/Flames_of_War_%28warrior_skill%29.png"),
                new Buff("Blood Reckoning", 29466 , GeneralHelper.Source.Berserker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d6/Blood_Reckoning.png"),
                new Buff("Rock Guard", 34256 , GeneralHelper.Source.Berserker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c7/Shattering_Blow.png"),
                new Buff("Sight beyond Sight",40616, GeneralHelper.Source.Spellbreaker, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/d/d7/Sight_beyond_Sight.png"),
                //signets
                new Buff("Healing Signet",786, GeneralHelper.Source.Warrior, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/8/85/Healing_Signet.png"),
                new Buff("Dolyak Signet",14458, GeneralHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/60/Dolyak_Signet.png"),
                new Buff("Signet of Fury",14459, GeneralHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c1/Signet_of_Fury.png"),
                new Buff("Signet of Might",14444, GeneralHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/40/Signet_of_Might.png"),
                new Buff("Signet of Stamina",14478, GeneralHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/6b/Signet_of_Stamina.png"),
                new Buff("Signet of Rage",14496, GeneralHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/bc/Signet_of_Rage.png"),
                //banners
                new Buff("Banner of Strength", 14417, GeneralHelper.Source.Warrior, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/e/e1/Banner_of_Strength.png/33px-Banner_of_Strength.png"),
                new Buff("Banner of Discipline", 14449, GeneralHelper.Source.Warrior, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/5/5f/Banner_of_Discipline.png/33px-Banner_of_Discipline.png"),
                new Buff("Banner of Tactics",14450, GeneralHelper.Source.Warrior, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/3/3f/Banner_of_Tactics.png/33px-Banner_of_Tactics.png"),
                new Buff("Banner of Defense",14543, GeneralHelper.Source.Warrior, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/f/f1/Banner_of_Defense.png/33px-Banner_of_Defense.png"),
                //stances
                new Buff("Shield Stance",756, GeneralHelper.Source.Warrior, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/d/de/Shield_Stance.png"),
                new Buff("Berserker's Stance",14453, GeneralHelper.Source.Warrior, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/8/8a/Berserker_Stance.png"),
                new Buff("Enduring Pain",787, GeneralHelper.Source.Warrior, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/24/Endure_Pain.png"),
                new Buff("Balanced Stance",34778, GeneralHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/27/Balanced_Stance.png"),
                new Buff("Defiant Stance",21816, GeneralHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/db/Defiant_Stance.png"),
                new Buff("Rampage",14484, GeneralHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e4/Rampage.png"),
                //traits
                new Buff("Soldier's Focus", 58102, GeneralHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/29/Soldier%27s_Focus.png", 99526, ulong.MaxValue),
                new Buff("Brave Stride", 43063, GeneralHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b8/Death_from_Above.png"),
                new Buff("Empower Allies", 14222, GeneralHelper.Source.Warrior, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/4/4c/Empower_Allies.png/20px-Empower_Allies.png"),
                new Buff("Peak Performance",46853, GeneralHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/98/Peak_Performance.png"),
                new Buff("Furious Surge", 30204, GeneralHelper.Source.Warrior, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/65/Furious.png"),
                //new Boon("Health Gain per Adrenaline bar Spent",-1, BoonSource.Warrior, BoonType.Intensity, 3, BoonEnum.GraphOnlyBuff,RemoveType.Normal),
                new Buff("Rousing Resilience",24383, GeneralHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ca/Rousing_Resilience.png"),
                new Buff("Feel No Pain (Savage Instinct)",55030, GeneralHelper.Source.Berserker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4d/Savage_Instinct.png", 96406, ulong.MaxValue),
                new Buff("Always Angry",34099, GeneralHelper.Source.Berserker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/63/Always_Angry.png", 0 , 96406),
                new Buff("Full Counter",43949, GeneralHelper.Source.Spellbreaker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/fb/Full_Counter.png"),
                new Buff("Attacker's Insight",41963, GeneralHelper.Source.Spellbreaker, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/89/Attacker%27s_Insight.png"),
                new Buff("Berserker's Power",42539, GeneralHelper.Source.Warrior, BuffStackType.Stacking, 3, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/6f/Berserker%27s_Power.png"),
        };

        private readonly List<Buff> _guardian = new List<Buff>
        {        
                //skills
                new Buff("Zealot's Flame", 9103, GeneralHelper.Source.Guardian, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7a/Zealot%27s_Flame.png"),
                new Buff("Purging Flames",21672, GeneralHelper.Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/28/Purging_Flames.png"),
                new Buff("Litany of Wrath",21665, GeneralHelper.Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4a/Litany_of_Wrath.png"),
                new Buff("Renewed Focus",9255, GeneralHelper.Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/10/Renewed_Focus.png"),
                new Buff("Ashes of the Just",41957, GeneralHelper.Source.Firebrand, BuffStackType.StackingConditionalLoss, 25, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/6/6d/Epilogue-_Ashes_of_the_Just.png"),
                new Buff("Eternal Oasis",44871, GeneralHelper.Source.Firebrand, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/5/5f/Epilogue-_Eternal_Oasis.png"),
                new Buff("Unbroken Lines",43194, GeneralHelper.Source.Firebrand, BuffStackType.Stacking, 3, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/d/d8/Epilogue-_Unbroken_Lines.png"),
                new Buff("Shield of Wrath",9123, GeneralHelper.Source.Guardian, BuffStackType.Stacking, 3, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/bc/Shield_of_Wrath.png"),
                new Buff("Binding Blade (Self)",9225, GeneralHelper.Source.Guardian, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/de/Binding_Blade.png"),
                new Buff("Binding Blade",9148, GeneralHelper.Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/de/Binding_Blade.png"),
                new Buff("Justice",30232, GeneralHelper.Source.Dragonhunter, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b0/Spear_of_Light.png"),
                new Buff("Shield of Courage (Active)", 29906, GeneralHelper.Source.Dragonhunter, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/63/Shield_of_Courage.png"),
                //signets
                new Buff("Signet of Resolve",9220, GeneralHelper.Source.Guardian, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/75/Signet_of_Resolve.png"),
                new Buff("Signet of Resolve (Shared)", 46554, GeneralHelper.Source.Guardian, BuffStackType.Stacking, 25, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/7/75/Signet_of_Resolve.png"),
                new Buff("Bane Signet",9092, GeneralHelper.Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/56/Bane_Signet.png"),
                new Buff("Bane Signet (PI)",9240, GeneralHelper.Source.Guardian, BuffStackType.Stacking, 25, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/5/56/Bane_Signet.png"),
                new Buff("Signet of Judgment",9156, GeneralHelper.Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/fe/Signet_of_Judgment.png"),
                new Buff("Signet of Judgment (PI)",9239, GeneralHelper.Source.Guardian, BuffStackType.Stacking, 25, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/f/fe/Signet_of_Judgment.png"),
                new Buff("Signet of Mercy",9162, GeneralHelper.Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/37/Signet_of_Mercy.png"),
                new Buff("Signet of Mercy (PI)",9238, GeneralHelper.Source.Guardian, BuffStackType.Stacking, 25, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/3/37/Signet_of_Mercy.png"),
                new Buff("Signet of Wrath",9100, GeneralHelper.Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/18/Signet_of_Wrath.png"),
                new Buff("Signet of Wrath (PI)",9237, GeneralHelper.Source.Guardian, BuffStackType.Stacking, 25, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/1/18/Signet_of_Wrath.png"),
                new Buff("Signet of Courage",29633, GeneralHelper.Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/5d/Signet_of_Courage.png"),
                new Buff("Signet of Courage (Shared)",43487 , GeneralHelper.Source.Guardian, BuffStackType.Stacking, 25, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/5/5d/Signet_of_Courage.png"),
                //virtues
                new Buff("Virtue of Justice", 9114, GeneralHelper.Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/10/Virtue_of_Justice.png"),
                new Buff("Spear of Justice", 29632, GeneralHelper.Source.Dragonhunter, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/f1/Spear_of_Justice.png"),
                new Buff("Virtue of Courage", 9113, GeneralHelper.Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a9/Virtue_of_Courage.png"),
                new Buff("Shield of Courage", 29523, GeneralHelper.Source.Dragonhunter, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/63/Shield_of_Courage.png"),
                new Buff("Virtue of Resolve", 9119, GeneralHelper.Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b2/Virtue_of_Resolve.png"),
                new Buff("Wings of Resolve", 30308, GeneralHelper.Source.Dragonhunter, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/cb/Wings_of_Resolve.png"),
                new Buff("Tome of Justice",40530, GeneralHelper.Source.Firebrand, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/ae/Tome_of_Justice.png"),
                new Buff("Tome of Courage",43508,GeneralHelper.Source.Firebrand, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/54/Tome_of_Courage.png"),
                new Buff("Tome of Resolve",46298, GeneralHelper.Source.Firebrand, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a9/Tome_of_Resolve.png"),
                //traits
                new Buff("Strength in Numbers",13796, GeneralHelper.Source.Guardian, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/7/7b/Strength_in_Numbers.png"),
                new Buff("Invigorated Bulwark",30207, GeneralHelper.Source.Guardian, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/00/Invigorated_Bulwark.png"),
                new Buff("Battle Presence", 17046, GeneralHelper.Source.Guardian, BuffStackType.Queue, 2, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/2/27/Battle_Presence.png"),
                //new Boon("Force of Will",29485, BoonSource.Guardian, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),//not sure if intensity
                new Buff("Quickfire",45123, GeneralHelper.Source.Firebrand, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d6/Quickfire.png"),
                new Buff("Symbolic Avenger",56890, GeneralHelper.Source.Guardian, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e5/Symbolic_Avenger.png", 97950, ulong.MaxValue),
                new Buff("Inspiring Virtue",59592, GeneralHelper.Source.Guardian, BuffStackType.Queue, 99, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8f/Virtuous_Solace.png", 102321, 102389),
                new Buff("Inspiring Virtue",59592, GeneralHelper.Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8f/Virtuous_Solace.png", 102389, ulong.MaxValue),
        };

        private readonly List<Buff> _engineer = new List<Buff>
        {       //skills
                new Buff("Static Shield",6055, GeneralHelper.Source.Engineer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/90/Static_Shield.png"),
                new Buff("Absorb",6056, GeneralHelper.Source.Engineer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8a/Absorb.png"),
                new Buff("A.E.D.",21660, GeneralHelper.Source.Engineer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e6/A.E.D..png"),
                new Buff("Elixir S",5863, GeneralHelper.Source.Engineer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d8/Elixir_S.png"),
                //new Boon("Elixir X", -1,BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Normal),
                new Buff("Utility Goggles",5864, GeneralHelper.Source.Engineer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/02/Utility_Goggles.png"),
                new Buff("Slick Shoes",5833, GeneralHelper.Source.Engineer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/3d/Slick_Shoes.png"),
                //new Boon("Watchful Eye",-1, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Normal),
                new Buff("Cooling Vapor",46444, GeneralHelper.Source.Holosmith, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b1/Coolant_Blast.png"),
                new Buff("Photon Wall Deployed",46094, GeneralHelper.Source.Holosmith, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/ea/Photon_Wall.png"),
                new Buff("Spectrum Shield",43066, GeneralHelper.Source.Holosmith, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/29/Spectrum_Shield.png"),
                new Buff("Gear Shield",5997, GeneralHelper.Source.Engineer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ca/Gear_Shield.png"),
                //Transforms
                //new Boon("Rampage",-1, BoonSource.Engineer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Normal),
                new Buff("Photon Forge",43708, GeneralHelper.Source.Holosmith, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/dd/Engage_Photon_Forge.png"),
                //Traits
                new Buff("Laser's Edge",44414, GeneralHelper.Source.Holosmith, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/5d/Laser%27s_Edge.png",0 , 97950),
                new Buff("Afterburner",42210, GeneralHelper.Source.Holosmith, BuffStackType.StackingConditionalLoss, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/51/Solar_Focusing_Lens.png"),
                new Buff("Iron Blooded",49065, GeneralHelper.Source.Engineer, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1e/Iron_Blooded.png"),
                new Buff("Streamlined Kits",18687, GeneralHelper.Source.Engineer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/cb/Streamlined_Kits.png"),
                new Buff("Kinetic Charge",45781, GeneralHelper.Source.Engineer, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e0/Kinetic_Battery.png"),
                new Buff("Pinpoint Distribution", 38333, GeneralHelper.Source.Engineer, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/b/bf/Pinpoint_Distribution.png"),
                new Buff("Heat Therapy",40694, GeneralHelper.Source.Holosmith, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/34/Heat_Therapy.png"),
                new Buff("Overheat", 41037, GeneralHelper.Source.Holosmith, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4b/Overheat.png"),
                new Buff("Thermal Vision", 51389, GeneralHelper.Source.Engineer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8a/Skilled_Marksman.png"),
                new Buff("Explosive Entrance",59579, GeneralHelper.Source.Engineer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/33/Explosive_Entrance.png", 102321, ulong.MaxValue),
                new Buff("Explosive Temper",59528, GeneralHelper.Source.Engineer, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c1/Explosive_Temper.png", 102321, ulong.MaxValue),

        };

        private readonly List<Buff> _ranger = new List<Buff>
        {

                new Buff("Celestial Avatar", 31508, GeneralHelper.Source.Druid, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/59/Celestial_Avatar.png"),
                new Buff("Counterattack",14509, GeneralHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c1/Counterattack.png"),
                //signets
                new Buff("Signet of Renewal",41147, GeneralHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/11/Signet_of_Renewal.png"),
                new Buff("Signet of Stone (Passive)",12627, GeneralHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/63/Signet_of_Stone.png"),
                new Buff("Signet of the Hunt (Passive)",12626, GeneralHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/df/Signet_of_the_Hunt.png"),
                new Buff("Signet of the Wild",12518, GeneralHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/23/Signet_of_the_Wild.png"),
                new Buff("Signet of the Wild (Pet)",12636, GeneralHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/23/Signet_of_the_Wild.png"),
                new Buff("Signet of Stone (Active)",12543, GeneralHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/63/Signet_of_Stone.png"),
                new Buff("Signet of the Hunt (Active)",12541, GeneralHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/df/Signet_of_the_Hunt.png"),
                //spirits
                // new Boon("Water Spirit (old)", 50386, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/0/06/Water_Spirit.png/33px-Water_Spirit.png"),
                new Buff("Frost Spirit", 12544, GeneralHelper.Source.Ranger, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png", 0, 88541),
                new Buff("Sun Spirit", 12540, GeneralHelper.Source.Ranger, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/d/dd/Sun_Spirit.png/33px-Sun_Spirit.png", 0, 88541),
                new Buff("Stone Spirit", 12547, GeneralHelper.Source.Ranger, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/3/35/Stone_Spirit.png/20px-Stone_Spirit.png", 0, 88541),
                //new Boon("Storm Spirit (old)", 50381, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/2/25/Storm_Spirit.png/30px-Storm_Spirit.png"),
                //reworked
                new Buff("Water Spirit", 50386, GeneralHelper.Source.Ranger, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/0/06/Water_Spirit.png/33px-Water_Spirit.png", 88541, ulong.MaxValue),
                new Buff("Frost Spirit", 50421, GeneralHelper.Source.Ranger, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png", 88541, ulong.MaxValue),
                new Buff("Sun Spirit", 50413, GeneralHelper.Source.Ranger, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/d/dd/Sun_Spirit.png/33px-Sun_Spirit.png", 88541, ulong.MaxValue),
                new Buff("Stone Spirit", 50415, GeneralHelper.Source.Ranger, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/3/35/Stone_Spirit.png/20px-Stone_Spirit.png", 88541, ulong.MaxValue),
                new Buff("Storm Spirit", 50381, GeneralHelper.Source.Ranger, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/2/25/Storm_Spirit.png/30px-Storm_Spirit.png", 88541, ulong.MaxValue),
                //skills
                new Buff("Attack of Opportunity",12574, GeneralHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/47/Moment_of_Clarity.png"),
                new Buff("Call of the Wild",36781, GeneralHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8d/Call_of_the_Wild.png",0 , 97950),
                new Buff("Call of the Wild",36781, GeneralHelper.Source.Ranger, BuffStackType.Stacking, 3, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8d/Call_of_the_Wild.png",97950 , 102321),
                new Buff("Strength of the Pack!",12554, GeneralHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4b/%22Strength_of_the_Pack%21%22.png"),
                new Buff("Sic 'Em!",33902, GeneralHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png"),
                new Buff("Sic 'Em! (PvP)",56923, GeneralHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png"),
                new Buff("Sharpening Stones",12536, GeneralHelper.Source.Ranger, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/af/Sharpening_Stone.png"),
                new Buff("Ancestral Grace", 31584, GeneralHelper.Source.Druid, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4b/Ancestral_Grace.png"),
                new Buff("Glyph of Empowerment", 31803, GeneralHelper.Source.Druid, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/d/d7/Glyph_of_the_Stars.png", 0 , 96406),
                new Buff("Glyph of Unity", 31385, GeneralHelper.Source.Druid, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b1/Glyph_of_Unity.png"),
                new Buff("Glyph of Unity (CA)", 31556, GeneralHelper.Source.Druid, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/4/4c/Glyph_of_Unity_%28Celestial_Avatar%29.png"),
                new Buff("Glyph of the Stars", 55048, GeneralHelper.Source.Druid, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/d/d7/Glyph_of_the_Stars.png", 96406, ulong.MaxValue),
                new Buff("Glyph of the Stars (CA)", 55049, GeneralHelper.Source.Druid, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/d/d7/Glyph_of_the_Stars.png", 96406, ulong.MaxValue),
                new Buff("Dolyak Stance",41815, GeneralHelper.Source.Soulbeast, BuffStackType.Queue, 25, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/7/71/Dolyak_Stance.png"),
                new Buff("Griffon Stance",46280, GeneralHelper.Source.Soulbeast, BuffStackType.Queue, 25, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/9/98/Griffon_Stance.png"),
                new Buff("Moa Stance",45038, GeneralHelper.Source.Soulbeast, BuffStackType.Queue, 25, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/6/66/Moa_Stance.png"),
                new Buff("Vulture Stance",44651, GeneralHelper.Source.Soulbeast, BuffStackType.Queue, 25, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/8/8f/Vulture_Stance.png"),
                new Buff("Bear Stance",40045, GeneralHelper.Source.Soulbeast, BuffStackType.Queue, 25, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/f/f0/Bear_Stance.png"),
                new Buff("One Wolf Pack",44139, GeneralHelper.Source.Soulbeast, BuffStackType.Queue, 25, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/3/3b/One_Wolf_Pack.png"),
                new Buff("Sharpen Spines",43266, GeneralHelper.Source.Ranger, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/95/Sharpen_Spines.png"),
                //traits
                new Buff("Spotter", 14055, GeneralHelper.Source.Ranger, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/b/b0/Spotter.png"),
                new Buff("Opening Strike",13988, GeneralHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/44/Opening_Strike_%28effect%29.png"),
                new Buff("Quick Draw",29703, GeneralHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/39/Quick_Draw.png"),
                new Buff("Light on your Feet",30673, GeneralHelper.Source.Ranger, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/22/Light_on_your_Feet.png"),
                new Buff("Natural Mender",30449, GeneralHelper.Source.Druid, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e9/Natural_Mender.png"),
                new Buff("Lingering Light",32248, GeneralHelper.Source.Druid, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/5d/Lingering_Light.png"),
                new Buff("Deadly",44932, GeneralHelper.Source.Soulbeast, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/94/Deadly_%28Archetype%29.png"),
                new Buff("Ferocious",41720, GeneralHelper.Source.Soulbeast, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e9/Ferocious_%28Archetype%29.png"),
                new Buff("Supportive",40069, GeneralHelper.Source.Soulbeast, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/32/Supportive_%28Archetype%29.png"),
                new Buff("Versatile",44693, GeneralHelper.Source.Soulbeast, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/bb/Versatile_%28Archetype%29.png"),
                new Buff("Stout",40272, GeneralHelper.Source.Soulbeast, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/80/Stout_%28Archetype%29.png"),
                new Buff("Unstoppable Union",44439, GeneralHelper.Source.Soulbeast, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b2/Unstoppable_Union.png"),
                new Buff("Twice as Vicious",45600, GeneralHelper.Source.Soulbeast, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png"),
        };

        private readonly List<Buff> _thief = new List<Buff>
        {
                //signets
                new Buff("Signet of Malice",13049, GeneralHelper.Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/ae/Signet_of_Malice.png"),
                new Buff("Assassin's Signet (Passive)",13047, GeneralHelper.Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/23/Assassin%27s_Signet.png"),
                new Buff("Assassin's Signet (Active)",44597, GeneralHelper.Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/23/Assassin%27s_Signet.png"),
                new Buff("Infiltrator's Signet",13063, GeneralHelper.Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8e/Infiltrator%27s_Signet.png"),
                new Buff("Signet of Agility",13061, GeneralHelper.Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1d/Signet_of_Agility.png"),
                new Buff("Signet of Shadows",13059, GeneralHelper.Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/17/Signet_of_Shadows.png"),
                //venoms // src is always the user, makes generation data useless
                new Buff("Skelk Venom",21780, GeneralHelper.Source.Thief, BuffStackType.StackingConditionalLoss, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/75/Skelk_Venom.png"),
                new Buff("Ice Drake Venom",13095, GeneralHelper.Source.Thief, BuffStackType.StackingConditionalLoss, 4, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7b/Ice_Drake_Venom.png"),
                new Buff("Devourer Venom", 13094, GeneralHelper.Source.Thief, BuffStackType.StackingConditionalLoss, 2, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4d/Devourer_Venom.png"),
                new Buff("Skale Venom", 13054, GeneralHelper.Source.Thief, BuffStackType.StackingConditionalLoss, 4, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/14/Skale_Venom.png"),
                new Buff("Spider Venom",13036, GeneralHelper.Source.Thief, BuffStackType.StackingConditionalLoss, 6, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/39/Spider_Venom.png"),
                new Buff("Basilisk Venom", 13133, GeneralHelper.Source.Thief, BuffStackType.StackingConditionalLoss, 2, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/3a/Basilisk_Venom.png"),
                //physical
                new Buff("Palm Strike",30423, GeneralHelper.Source.Daredevil, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
                new Buff("Pulmonary Impact",30510, GeneralHelper.Source.Daredevil, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
                //weapon
                new Buff("Infiltration",13135, GeneralHelper.Source.Thief, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/6/62/Infiltrator%27s_Return.png"),
                //transforms
                new Buff("Dagger Storm",13134, GeneralHelper.Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c0/Dagger_Storm.png"),
                new Buff("Kneeling",42869, GeneralHelper.Source.Deadeye, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/56/Kneel.png"),
                //traits
                //new Boon("Deadeye's Gaze",46333, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                //new Boon("Maleficent Seven",43606, BoonSource.Thief, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff),
                new Buff("Hidden Killer",42720, GeneralHelper.Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/ec/Hidden_Killer.png"),
                new Buff("Lead Attacks",34659, GeneralHelper.Source.Thief, BuffStackType.Stacking, 15, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/01/Lead_Attacks.png"),
                new Buff("Instant Reflexes",34283, GeneralHelper.Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7d/Instant_Reflexes.png"),
                new Buff("Lotus Training", 32200, GeneralHelper.Source.Daredevil, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/ea/Lotus_Training.png"),
                new Buff("Unhindered Combatant", 32931, GeneralHelper.Source.Daredevil, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a1/Unhindered_Combatant.png"),
                new Buff("Bounding Dodger", 33162, GeneralHelper.Source.Daredevil, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/30/Bounding_Dodger.png"),
                new Buff("Weakening Strikes", 34081, GeneralHelper.Source.Daredevil, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7c/Weakening_Strikes.png", 96406, ulong.MaxValue),

        };

        private readonly List<Buff> _necromancer = new List<Buff>
        {
            
                //forms
                new Buff("Lich Form",10631, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/ab/Lich_Form.png"),
                new Buff("Death Shroud", 790, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/f5/Death_Shroud.png"),
                new Buff("Reaper's Shroud", 29446, GeneralHelper.Source.Reaper, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/11/Reaper%27s_Shroud.png"),
                //signets
                new Buff("Signet of Vampirism (Passive)",21761, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/73/Signet_of_Vampirism.png"),
                new Buff("Signet of Vampirism (Active)",21766, GeneralHelper.Source.Necromancer, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/73/Signet_of_Vampirism.png"),
                new Buff("Signet of Vampirism (Shroud)",43885, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/73/Signet_of_Vampirism.png"),
                new Buff("Plague Signet (Passive)",10630, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c5/Plague_Signet.png"),
                new Buff("Plague Signet (Shroud)",44164, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c5/Plague_Signet.png"),
                new Buff("Signet of Spite (Passive)",10621, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/df/Signet_of_Spite.png"),
                new Buff("Signet of Spite (Shroud)",43772, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/df/Signet_of_Spite.png"),
                new Buff("Signet of the Locust (Passive)",10614, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a3/Signet_of_the_Locust.png"),
                new Buff("Signet of the Locust (Shroud)",40283, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a3/Signet_of_the_Locust.png"),
                new Buff("Signet of Undeath (Passive)",10610, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/9c/Signet_of_Undeath.png"),
                new Buff("Signet of Undeath (Shroud)",40583, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/9c/Signet_of_Undeath.png"),
                //skills
                new Buff("Spectral Walk",15083, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/33/Spectral_Walk.png", 0, 94051),
                new Buff("Spectral Walk",53476, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/33/Spectral_Walk.png", 94051, ulong.MaxValue),
                new Buff("Spectral Armor",10582, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d1/Spectral_Armor.png"),
                new Buff("Infusing Terror", 30129, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/db/Infusing_Terror.png"),
                new Buff("Locust Swarm", 10567, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/77/Locust_Swarm.png"),
                //new Boon("Sand Cascade", 43759, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1e/Sand_Cascade.png"),
                //traits
                new Buff("Corrupter's Defense",30845, GeneralHelper.Source.Necromancer, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/11/Corrupter%27s_Fervor.png", 0, 99526),
                new Buff("Death's Carapace",30845, GeneralHelper.Source.Necromancer, BuffStackType.Stacking, 30, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/58/Death%27s_Carapace.png", 99526, ulong.MaxValue),
                new Buff("Flesh of the Master",13810, GeneralHelper.Source.Necromancer, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e9/Flesh_of_the_Master.png", 0, 99526),
                new Buff("Vampiric Aura", 30285, GeneralHelper.Source.Necromancer, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/d/da/Vampiric_Presence.png"),
                new Buff("Vampiric Strikes", 30398, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/da/Vampiric_Presence.png"),
                new Buff("Last Rites",29726, GeneralHelper.Source.Necromancer, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/1/1a/Last_Rites_%28effect%29.png"),
                new Buff("Sadistic Searing",43626, GeneralHelper.Source.Scourge, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/dd/Sadistic_Searing.png"),
                new Buff("Soul Barbs",53489, GeneralHelper.Source.Necromancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/bd/Soul_Barbs.png"),
        };

        private readonly List<Buff> _mesmer = new List<Buff>
        {
            
                //signets
                new Buff("Signet of the Ether", 21751, GeneralHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7a/Signet_of_the_Ether.png"),
                new Buff("Signet of Domination",10231, GeneralHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/3b/Signet_of_Domination.png"),
                new Buff("Signet of Illusions",10246, GeneralHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ce/Signet_of_Illusions.png"),
                new Buff("Signet of Inspiration",10235, GeneralHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/ed/Signet_of_Inspiration.png"),
                new Buff("Signet of Midnight",10233, GeneralHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/24/Signet_of_Midnight.png"),
                new Buff("Signet of Humility",30739, GeneralHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b5/Signet_of_Humility.png"),
                //skills
                new Buff("Distortion",10243, GeneralHelper.Source.Mesmer, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/22/Distortion.png"),
                new Buff("Blur", 10335 , GeneralHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/22/Distortion.png"),
                new Buff("Mirror",10357, GeneralHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b8/Mirror.png"),
                new Buff("Echo",29664, GeneralHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ce/Echo.png"),
                //new Boon("Illusion of Life",-1, BoonSource.Mesmer, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Normal),
                //new Boon("Time Block",30134, BoonSource.Mesmer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8d/Deja_Vu.png"),
                new Buff("Time Echo",29582, GeneralHelper.Source.Chronomancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8d/Deja_Vu.png"),
                new Buff("Illusionary Counter",10278, GeneralHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e5/Illusionary_Counter.png"),
                new Buff("Time Anchored",30136, GeneralHelper.Source.Chronomancer, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/db/Continuum_Split.png"),
                new Buff("Illusionary Riposte",10279, GeneralHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/91/Illusionary_Riposte.png"),
                new Buff("Illusionary Leap",10353, GeneralHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/18/Illusionary_Leap.png"),
                //traits
                new Buff("Fencer's Finesse", 30426 , GeneralHelper.Source.Mesmer, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e7/Fencer%27s_Finesse.png"),
                new Buff("Illusionary Defense",49099, GeneralHelper.Source.Mesmer, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e0/Illusionary_Defense.png"),
                new Buff("Compounding Power",49058, GeneralHelper.Source.Mesmer, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e5/Compounding_Power.png"),
                new Buff("Phantasmal Force", 44691 , GeneralHelper.Source.Mesmer, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/5f/Mistrust.png"),
                new Buff("Mirage Cloak",40408, GeneralHelper.Source.Mirage, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a5/Mirage_Cloak_%28effect%29.png"),
        };

        private readonly List<Buff> _elementalist = new List<Buff>
        {
            
                //signets
                new Buff("Signet of Restoration",739, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/dd/Signet_of_Restoration.png"),
                new Buff("Signet of Air",5590, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/92/Signet_of_Air.png"),
                new Buff("Signet of Earth",5592, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ce/Signet_of_Earth.png"),
                new Buff("Signet of Fire",5544, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b0/Signet_of_Fire.png"),
                new Buff("Signet of Water",5591, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/fd/Signet_of_Water.png"),
                ///attunements
                // Fire
                new Buff("Fire Attunement", 5585, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b4/Fire_Attunement.png"),
                new Buff("Dual Fire Attunement", ProfHelper.FireDual, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b4/Fire_Attunement.png"),
                new Buff("Fire Water Attunement", ProfHelper.FireWater, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/ihqKuUJ.png"),
                new Buff("Fire Air Attunement", ProfHelper.FireAir, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/kKFJ8cT.png"),
                new Buff("Fire Earth Attunement", ProfHelper.FireEarth, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/T4187h0.png"),
                // Water
                new Buff("Water Attunement", 5586, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/31/Water_Attunement.png"),
                new Buff("Dual Water Attunement", ProfHelper.WaterDual, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/31/Water_Attunement.png"),
                new Buff("Water Fire Attunement", ProfHelper.WaterFire, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/vMUkzxH.png"),
                new Buff("Water Air Attunement", ProfHelper.WaterAir, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/5G5OFud.png"),
                new Buff("Water Earth Attunement", ProfHelper.WaterEarth, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/QKEtF2P.png"),
                // Air
                new Buff("Air Attunement", 5575, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/91/Air_Attunement.png"),
                new Buff("Dual Air Attunement", ProfHelper.AirDual, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/91/Air_Attunement.png"),
                new Buff("Air Fire Attunement", ProfHelper.AirFire, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/vf68GJm.png"),
                new Buff("Air Water Attunement", ProfHelper.AirWater, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/Tuj5Sro.png"),
                new Buff("Air Earth Attunement", ProfHelper.AirEarth, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/lHcOSwk.png"),
                // Earth
                new Buff("Earth Attunement", 5580, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a8/Earth_Attunement.png"),
                new Buff("Dual Earth Attunement", ProfHelper.EarthDual, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a8/Earth_Attunement.png"),
                new Buff("Earth Fire Attunement", ProfHelper.EarthFire, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/aJWvE0I.png"),
                new Buff("Earth Water Attunement", ProfHelper.EarthWater, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/jtjj2TG.png"),
                new Buff("Earth Air Attunement", ProfHelper.EarthAir, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/4Eti7Pb.png"),
                //forms
                new Buff("Mist Form",5543, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1b/Mist_Form.png"),
                new Buff("Ride the Lightning",5588, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/59/Ride_the_Lightning.png"),
                new Buff("Vapor Form",5620, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/6c/Vapor_Form.png"),
                new Buff("Tornado",5583, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/40/Tornado.png"),
                //new Boon("Whirlpool", -1,BoonSource.Elementalist, BoonType.Duration, 1, BoonEnum.GraphOnlyBuff, RemoveType.Normal),
                //conjures
                new Buff("Conjure Earth Shield", 15788, GeneralHelper.Source.Elementalist, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/7/7a/Conjure_Earth_Shield.png"),
                new Buff("Conjure Flame Axe", 15789, GeneralHelper.Source.Elementalist, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/a/a1/Conjure_Flame_Axe.png"),
                new Buff("Conjure Frost Bow", 15790, GeneralHelper.Source.Elementalist, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/c/c3/Conjure_Frost_Bow.png"),
                new Buff("Conjure Lightning Hammer", 15791, GeneralHelper.Source.Elementalist, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/1/1f/Conjure_Lightning_Hammer.png"),
                new Buff("Conjure Fiery Greatsword", 15792, GeneralHelper.Source.Elementalist, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/e/e2/Conjure_Fiery_Greatsword.png"),
                //skills
                new Buff("Arcane Power",5582, GeneralHelper.Source.Elementalist, BuffStackType.Stacking, 6, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/72/Arcane_Power.png"),
                new Buff("Primordial Stance",42086, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/3a/Primordial_Stance.png"),
                new Buff("Unravel",42683, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4b/Unravel.png"),
                new Buff("Arcane Shield",5640, GeneralHelper.Source.Elementalist, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/9d/Arcane_Shield.png"),
                new Buff("Renewal of Fire",5764, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/63/Renewal_of_Fire.png"),
                new Buff("Glyph of Elemental Power (Fire)",5739, GeneralHelper.Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/f2/Glyph_of_Elemental_Power_%28fire%29.png"),
                new Buff("Glyph of Elemental Power (Water)",5741, GeneralHelper.Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/bf/Glyph_of_Elemental_Power_%28water%29.png"),
                new Buff("Glyph of Elemental Power (Air)",5740, GeneralHelper.Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/cb/Glyph_of_Elemental_Power_%28air%29.png"),
                new Buff("Glyph of Elemental Power (Earth)",5742, GeneralHelper.Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/0a/Glyph_of_Elemental_Power_%28earth%29.png"),
                new Buff("Rebound",31337, GeneralHelper.Source.Tempest, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/0/03/%22Rebound%21%22.png"),
                new Buff("Rock Barrier",34633, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/dd/Rock_Barrier.png"),//750?
                new Buff("Magnetic Wave",15794, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/32/Magnetic_Wave.png"),
                new Buff("Obsidian Flesh",5667, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c1/Obsidian_Flesh.png"),
                new Buff("Grinding Stones",51658, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/3d/Grinding_Stones.png"),
                new Buff("Static Charge",31487, GeneralHelper.Source.Elementalist, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/4/4b/Overload_Air.png"),
                //traits
                new Buff("Harmonious Conduit",31353, GeneralHelper.Source.Tempest, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b3/Harmonious_Conduit.png", 0, 99526),
                new Buff("Transcendent Tempest",31353, GeneralHelper.Source.Tempest, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/ac/Transcendent_Tempest_%28effect%29.png", 99526, ulong.MaxValue),
                new Buff("Fresh Air",34241, GeneralHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d8/Fresh_Air.png"),
                new Buff("Soothing Mist", 5587, GeneralHelper.Source.Elementalist, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/f/f7/Soothing_Mist.png"),
                new Buff("Weaver's Prowess",42061, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/75/Weaver%27s_Prowess.png"),
                new Buff("Elements of Rage",42416, GeneralHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a2/Elements_of_Rage.png"),
        };

        private readonly List<Buff> _consumables = new List<Buff>
        {
                new Buff("Malnourished",46587, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/6/67/Malnourished.png"),
                new Buff("Plate of Truffle Steak",9769, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/4/4c/Plate_of_Truffle_Steak.png"),
                new Buff("Bowl of Sweet and Spicy Butternut Squash Soup",17825, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/d/df/Bowl_of_Sweet_and_Spicy_Butternut_Squash_Soup.png"),
                new Buff("Bowl Curry Butternut Squash Soup",9829, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/1/16/Bowl_of_Curry_Butternut_Squash_Soup.png"),
                new Buff("Red-Lentil Saobosa",46273, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/a/a8/Red-Lentil_Saobosa.png"),
                new Buff("Super Veggie Pizza",10008, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/6/66/Super_Veggie_Pizza.png"),
                new Buff("Rare Veggie Pizza",10009, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/a/a0/Rare_Veggie_Pizza.png"),
                new Buff("Bowl of Garlic Kale Sautee",-1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/0/04/Bowl_of_Garlic_Kale_Sautee.png"),
                new Buff("Koi Cake",-1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/1/14/Koi_Cake.png"),
                new Buff("Prickly Pear Pie",24800, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/0/0a/Prickly_Pear_Pie.png"),
                new Buff("Bowl of Nopalitos Sauté",-1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/f/f1/Bowl_of_Nopalitos_Saut%C3%A9.png"),
                new Buff("Loaf of Candy Cactus Cornbread",24797, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/b/b2/Loaf_of_Candy_Cactus_Cornbread.png"),
                new Buff("Delicious Rice Ball",26529, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/5d/Delicious_Rice_Ball.png"),
                new Buff("Slice of Allspice Cake",33792, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/1/13/Slice_of_Allspice_Cake.png"),
                new Buff("Fried Golden Dumpling",26530, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/1/14/Fried_Golden_Dumpling.png"),
                new Buff("Bowl of Seaweed Salad",10080, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/1/1c/Bowl_of_Seaweed_Salad.png"),
                new Buff("Bowl of Orrian Truffle and Meat Stew",10096, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/b/b8/Bowl_of_Orrian_Truffle_and_Meat_Stew.png"),
                new Buff("Plate of Mussels Gnashblade",33476, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/7b/Plate_of_Mussels_Gnashblade.png"),
                new Buff("Spring Roll",26534, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/d/da/Spring_Roll.png"),
                new Buff("Plate of Beef Rendang",49686, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/d/d0/Plate_of_Beef_Rendang.png"),
                new Buff("Dragon's Revelry Starcake",19451, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/2/2b/Dragon%27s_Revelry_Starcake.png"),
                new Buff("Avocado Smoothie",50091, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/8/83/Avocado_Smoothie.png"),
                new Buff("Carrot Souffle",-1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/71/Carrot_Souffl%C3%A9.png"), //same as Dragon's_Breath_Bun
                new Buff("Plate of Truffle Steak Dinner",-1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/9/92/Plate_of_Truffle_Steak_Dinner.png"), //same as Dragon's Breath Bun
                new Buff("Dragon's Breath Bun",9750, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/53/Dragon%27s_Breath_Bun.png"),
                new Buff("Karka Egg Omelet",9756, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/9/9e/Karka_Egg_Omelet.png"),
                new Buff("Steamed Red Dumpling",26536, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/8/8c/Steamed_Red_Dumpling.png"),
                new Buff("Saffron Stuffed Mushroom",-1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/e/e2/Saffron_Stuffed_Mushroom.png"), //same as Karka Egg Omelet
                new Buff("Soul Pastry",53222, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/2/2c/Soul_Pastry.png"),
                new Buff("Bowl of Fire Meat Chili",10119, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/b/ba/Bowl_of_Fire_Meat_Chili.png"),
                new Buff("Plate of Fire Flank Steak",9765, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/2/27/Plate_of_Fire_Flank_Steak.png"),
                new Buff("Plate of Orrian Steak Frittes",9773, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/4/4d/Plate_of_Orrian_Steak_Frittes.png"),
                new Buff("Spicier Flank Steak",9764, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/0/01/Spicier_Flank_Steak.png"),
                new Buff("Mango Pie",9993, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/3/3d/Mango_Pie.png"),
                // UTILITIES 
                // 1h versions have the same ID as 30 min versions 
                new Buff("Diminished",46668, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/71/Diminished.png"),
                new Buff("Rough Sharpening Stone", 9958, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/1/14/Rough_Sharpening_Stone.png"),
                new Buff("Simple Sharpening Stone", 9959, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/e/ef/Simple_Sharpening_Stone.png"),
                new Buff("Standard Sharpening Stone", 9960, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/8/82/Standard_Sharpening_Stone.png"),
                new Buff("Quality Sharpening Stone", 9961, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/6/68/Quality_Sharpening_Stone.png"),
                new Buff("Hardened Sharpening Stone", 9962, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/8/8d/Hardened_Sharpening_Stone.png"),
                new Buff("Superior Sharpening Stone", 9963, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                //new Buff("Ogre Sharpening Stone", 9963, GeneralHelper.Source.Item, BuffType.Duration, 1, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                new Buff("Apprentice Maintenance Oil", 10111, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/51/Apprentice_Maintenance_Oil.png"),
                new Buff("Journeyman Maintenance Oil", 10112, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/b/b1/Journeyman_Maintenance_Oil.png"),
                new Buff("Standard Maintenance Oil", 9971, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/a/a6/Standard_Maintenance_Oil.png"),
                new Buff("Artisan Maintenance Oil", 9970, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/75/Artisan_Maintenance_Oil.png"),
                new Buff("Quality Maintenance Oil", 9969, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/2/23/Quality_Maintenance_Oil.png"),
                new Buff("Master Maintenance Oil", 9968, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                //new Buff("Hylek Maintenance Oil", 9968, GeneralHelper.Source.Item, BuffType.Duration, 1, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Buff("Apprentice Tuning Crystal", 10113, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/7d/Apprentice_Tuning_Crystal.png"),
                new Buff("Journeyman Tuning Crystal", 10114, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/1/1e/Journeyman_Tuning_Crystal.png"),
                new Buff("Standard Tuning Crystal", 9964, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/1/1e/Standard_Tuning_Crystal.png"),
                new Buff("Artisan Tuning Crystal", 9965, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/e/eb/Artisan_Tuning_Crystal.png"),
                new Buff("Quality Tuning Crystal", 9966, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/3/3b/Quality_Tuning_Crystal.png"),
                new Buff("Master Tuning Crystal", 9967, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/58/Master_Tuning_Crystal.png"),
                //new Buff("Krait Tuning Crystal", 9967, GeneralHelper.Source.Item, BuffType.Duration, 1, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/58/Master_Tuning_Crystal.png"),
                new Buff("Compact Hardened Sharpening Stone", 34657, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/1/1f/Compact_Hardened_Sharpening_Stone.png"),
                new Buff("Tin of Fruitcake", 34211, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/a/af/Tin_of_Fruitcake.png"),
                new Buff("Bountiful Sharpening Stone", 25880, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                new Buff("Toxic Sharpening Stone", 21826, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/d/db/Toxic_Sharpening_Stone.png"),
                new Buff("Magnanimous Sharpening Stone", 38522, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/a/aa/Magnanimous_Sharpening_Stone.png"),
                new Buff("Corsair Sharpening Stone", 46925, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/0/06/Corsair_Sharpening_Stone.png"),
                new Buff("Furious Sharpening Stone", 25882, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                new Buff("Holographic Super Cheese", 50320, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/f/fa/Holographic_Super_Cheese.png"),
                new Buff("Compact Quality Maintenance Oil", 34671, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/d/d8/Compact_Quality_Maintenance_Oil.png"),
                new Buff("Peppermint Oil", 34187, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/b/bc/Peppermint_Oil.png"),
                new Buff("Toxic Maintenance Oil", 21827, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/a/a6/Toxic_Maintenance_Oil.png"),
                new Buff("Magnanimous Maintenance Oil", 38605, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/53/Magnanimous_Maintenance_Oil.png"),
                new Buff("Enhanced Lucent Oil", 53304, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/e/ee/Enhanced_Lucent_Oil.png"),
                new Buff("Potent Lucent Oil", 53374, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/1/16/Potent_Lucent_Oil.png"),
                new Buff("Corsair Maintenance Oil", 47734, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/53/Magnanimous_Maintenance_Oil.png"),
                new Buff("Furious Maintenance Oil", 25881, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Buff("Holographic Super Drumstick", 50302, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/1/1d/Holographic_Super_Drumstick.png"),
                new Buff("Bountiful Maintenance Oil", 25879, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Buff("Compact Quality Tuning Crystal", 34677, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/b/ba/Compact_Quality_Tuning_Crystal.png"),
                new Buff("Tuning Icicle", 34206, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/75/Tuning_Icicle.png"),
                new Buff("Bountiful Tuning Crystal", 25877, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/58/Master_Tuning_Crystal.png"),
                new Buff("Toxic Focusing Crystal", 21828, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/d/de/Toxic_Focusing_Crystal.png"),
                new Buff("Magnanimous Tuning Crystal", 38678, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/2/23/Magnanimous_Tuning_Crystal.png"),
                new Buff("Furious Tuning Crystal", 25878, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/58/Master_Tuning_Crystal.png"),
                new Buff("Corsair Tuning Crystal", 48348, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/f/f7/Corsair_Tuning_Crystal.png"),
                new Buff("Holographic Super Apple", 50307, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/e/ee/Holographic_Super_Apple.png"),
                new Buff("Sharpening Skull", 25630, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/e/ee/Sharpening_Skull.png"),
                new Buff("Flask of Pumpkin Oil", 25632, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/71/Flask_of_Pumpkin_Oil.png"),
                new Buff("Lump of Crystallized Nougat", 25631, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/8/8f/Lump_of_Crystallized_Nougat.png"),
                new Buff("Writ of Basic Strength", 33160, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/7e/Writ_of_Basic_Strength.png"),
                new Buff("Writ of Strength", 32105, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/5e/Writ_of_Strength.png"),
                new Buff("Writ of Studied Strength", 33647, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/2/23/Writ_of_Studied_Strength.png"),
                new Buff("Writ of Calculated Strength", 32401, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/8/8d/Writ_of_Calculated_Strength.png"),
                new Buff("Writ of Learned Strength", 32044, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/8/8d/Writ_of_Calculated_Strength.png"),
                new Buff("Writ of Masterful Strength", 33297, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/2/2b/Writ_of_Masterful_Strength.png"),
                new Buff("Writ of Basic Accuracy", 33572, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/8/82/Writ_of_Basic_Accuracy.png"),
                new Buff("Writ of Accuracy", 32805, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/2/29/Writ_of_Accuracy.png"),
                new Buff("Writ of Studied Accuracy", 32429, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/a/ad/Writ_of_Studied_Accuracy.png"),
                new Buff("Writ of Calculated Accuracy", 33798, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/2/27/Writ_of_Calculated_Accuracy.png"),
                new Buff("Writ of Learned Accuracy", 32374, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/5a/Writ_of_Masterful_Accuracy.png"),
                new Buff("Writ of Masterful Accuracy", 31970, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/5a/Writ_of_Masterful_Accuracy.png"),
                new Buff("Writ of Basic Malice", 33310, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/9/9e/Writ_of_Basic_Malice.png"),
                new Buff("Writ of Malice", 33803, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/c/c4/Writ_of_Malice.png"),
                new Buff("Writ of Studied Malice", 32927, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/b/bd/Writ_of_Studied_Malice.png"),
                new Buff("Writ of Calculated Malice", 32316, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/75/Writ_of_Calculated_Malice.png"),
                new Buff("Writ of Learned Malice", 31959, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/9/9b/Writ_of_Learned_Malice.png"),
                new Buff("Writ of Masterful Malice", 33836, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/2/20/Writ_of_Masterful_Malice.png"),
                new Buff("Writ of Basic Speed", 33776, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/e/e6/Writ_of_Basic_Speed.png"),
                new Buff("Writ of Studied Speed", 33005, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/d/d1/Recipe_sheet_fine_boots.png"),
                new Buff("Writ of Masterful Speed", 33040, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/8/8e/Recipe_sheet_masterwork_boots.png"),
                new Buff("Potion Of Karka Toughness", 18704, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/f/fb/Powerful_Potion_of_Inquest_Slaying.png"),
                new Buff("Skale Venom (Consumable)", 972, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/9/96/Skale_Venom_%28consumable%29.png"),
                new Buff("Swift Moa Feather", 23239, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/f/f0/Swift_Moa_Feather.png"),
                // Slaying Potions
                new Buff("Powerful Potion of Flame Legion Slaying",9925, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/e/e2/Powerful_Potion_of_Flame_Legion_Slaying.png"),
                new Buff("Powerful Potion of Halloween Slaying",15279, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/f/fe/Powerful_Potion_of_Halloween_Slaying.png"),
                new Buff("Powerful Potion of Centaur Slaying",9845, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/3/3b/Powerful_Potion_of_Centaur_Slaying.png"),
                new Buff("Powerful Potion of Krait Slaying",9885, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/b/b4/Powerful_Potion_of_Krait_Slaying.png"),
                new Buff("Powerful Potion of Ogre Slaying",9877, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/b/b5/Powerful_Potion_of_Ogre_Slaying.png"),
                new Buff("Powerful Potion of Elemental Slaying",9893, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/5f/Powerful_Potion_of_Elemental_Slaying.png"),
                new Buff("Powerful Potion of Destroyer Slaying",9869, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/b/bd/Powerful_Potion_of_Destroyer_Slaying.png"),
                new Buff("Powerful Potion of Nightmare Court Slaying",9941, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/74/Powerful_Potion_of_Nightmare_Court_Slaying.png"),
                new Buff("Powerful Potion of Slaying Scarlet's Armies",23228, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/e/ee/Powerful_Potion_of_Demon_Slaying.png"),
                new Buff("Powerful Potion of Undead Slaying",9837, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/b/bd/Powerful_Potion_of_Undead_Slaying.png"),
                new Buff("Powerful Potion of Dredge Slaying",9949, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/9/9a/Powerful_Potion_of_Dredge_Slaying.png"),
                new Buff("Powerful Potion of Inquest Slaying",9917, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/f/fb/Powerful_Potion_of_Inquest_Slaying.png"),
                new Buff("Powerful Potion of Demon Slaying",9901, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/e/ee/Powerful_Potion_of_Demon_Slaying.png"),
                new Buff("Powerful Potion of Grawl Slaying",9853, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/1/15/Powerful_Potion_of_Grawl_Slaying.png"),
                new Buff("Powerful Potion of Sons of Svanir Slaying",9909, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/3/33/Powerful_Potion_of_Sons_of_Svanir_Slaying.png"),
                new Buff("Powerful Potion of Outlaw Slaying",9933, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/e/ec/Powerful_Potion_of_Outlaw_Slaying.png"),
                new Buff("Powerful Potion of Ice Brood Slaying",9861, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/0/0d/Powerful_Potion_of_Ice_Brood_Slaying.png"),
                // Fractals 
                new Buff("Fractal Mobility", 33024, GeneralHelper.Source.Item, BuffStackType.Stacking, 5, BuffNature.Consumable,"https://wiki.guildwars2.com/images/thumb/2/22/Mist_Mobility_Potion.png/40px-Mist_Mobility_Potion.png"),
                new Buff("Fractal Defensive", 32134, GeneralHelper.Source.Item, BuffStackType.Stacking, 5, BuffNature.Consumable,"https://wiki.guildwars2.com/images/thumb/e/e6/Mist_Defensive_Potion.png/40px-Mist_Defensive_Potion.png"),
                new Buff("Fractal Offensive", 32473, GeneralHelper.Source.Item, BuffStackType.Stacking, 5, BuffNature.Consumable,"https://wiki.guildwars2.com/images/thumb/8/8d/Mist_Offensive_Potion.png/40px-Mist_Offensive_Potion.png"),
                // Ascended Food
                // Feasts with yet unknown IDs are also added with ID of -1, the IDs can be added later on demand
                new Buff("Bowl of Fruit Salad with Cilantro Garnish", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/0/08/Bowl_of_Fruit_Salad_with_Cilantro_Garnish.png"),
                new Buff("Bowl of Fruit Salad with Mint Garnish", 57100, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/4/47/Bowl_of_Fruit_Salad_with_Mint_Garnish.png"),
                new Buff("Bowl of Fruit Salad with Orange-Clove Syrup", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/d/dc/Bowl_of_Fruit_Salad_with_Orange-Clove_Syrup.png"),
                new Buff("Bowl of Sesame Fruit Salad", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/0/01/Bowl_of_Sesame_Fruit_Salad.png"),
                new Buff("Bowl of Spiced Fruit Salad", 57276, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/9/9c/Bowl_of_Spiced_Fruit_Salad.png"),
                new Buff("Cilantro Lime Sous-Vide Steak", 57244, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/6/65/Cilantro_Lime_Sous-Vide_Steak.png"),
                new Buff("Cilantro and Cured Meat Flatbread", 57409, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/8/87/Cilantro_and_Cured_Meat_Flatbread.png"),
                new Buff("Clove and Veggie Flatbread", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/6/6e/Clove_and_Veggie_Flatbread.png"),
                new Buff("Clove-Spiced Creme Brulee", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/a/a2/Clove-Spiced_Creme_Brulee.png"),
                new Buff("Clove-Spiced Eggs Benedict", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/7d/Clove-Spiced_Eggs_Benedict.png"),
                new Buff("Clove-Spiced Pear and Cured Meat Flatbread", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/c/c5/Clove-Spiced_Pear_and_Cured_Meat_Flatbread.png"),
                new Buff("Eggs Benedict with Mint-Parsley Sauce", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/9/92/Eggs_Benedict_with_Mint-Parsley_Sauce.png"),
                new Buff("Mango Cilantro Creme Brulee", 57267, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/3/3d/Mango_Cilantro_Creme_Brulee.png"),
                new Buff("Mint Creme Brulee", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/3/31/Mint_Creme_Brulee.png"),
                new Buff("Mint Strawberry Cheesecake", 57384, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/6/64/Mint_Strawberry_Cheesecake.png"),
                new Buff("Mint and Veggie Flatbread", 57263, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/f/f9/Mint_and_Veggie_Flatbread.png"),
                new Buff("Mint-Pear Cured Meat Flatbread", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/6/60/Mint-Pear_Cured_Meat_Flatbread.png"),
                new Buff("Mushroom Clove Sous-Vide Steak", 57393, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/b/ba/Mushroom_Clove_Sous-Vide_Steak.png"),
                new Buff("Orange Clove Cheesecake", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/3/3f/Orange_Clove_Cheesecake.png"),
                new Buff("Peppercorn and Veggie Flatbread", 57382, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/9/9d/Peppercorn_and_Veggie_Flatbread.png"),
                new Buff("Peppercorn-Crusted Sous-Vide Steak", 57051, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/2/2e/Peppercorn-Crusted_Sous-Vide_Steak.png"),
                new Buff("Peppercorn-Spiced Eggs Benedict", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/c/c6/Peppercorn-Spiced_Eggs_Benedict.png"),
                new Buff("Peppered Cured Meat Flatbread", 57127, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/2/2d/Peppered_Cured_Meat_Flatbread.png"),
                new Buff("Plate of Beef Carpaccio with Mint Garnish", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/4/42/Plate_of_Beef_Carpaccio_with_Mint_Garnish.png"),
                new Buff("Plate of Clear Truffle and Cilantro Ravioli", 57295, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/0/05/Plate_of_Clear_Truffle_and_Cilantro_Ravioli.png"),
                new Buff("Plate of Clear Truffle and Mint Ravioli", 57112, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/9/9e/Plate_of_Clear_Truffle_and_Mint_Ravioli.png"),
                new Buff("Plate of Clear Truffle and Sesame Ravioli", 57213, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/d/de/Plate_of_Clear_Truffle_and_Sesame_Ravioli.png"),
                new Buff("Plate of Clove-Spiced Beef Carpaccio", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/4/44/Plate_of_Clove-Spiced_Beef_Carpaccio.png"),
                new Buff("Plate of Clove-Spiced Clear Truffle Ravioli", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/c/c2/Plate_of_Clove-Spiced_Clear_Truffle_Ravioli.png"),
                new Buff("Plate of Clove-Spiced Coq Au Vin", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/d/dc/Plate_of_Clove-Spiced_Coq_Au_Vin.png"),
                new Buff("Plate of Clove-Spiced Poultry Aspic", 57302, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/50/Plate_of_Clove-Spiced_Poultry_Aspic.png"),
                new Buff("Plate of Coq Au Vin with Mint Garnish", 57362, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/7c/Plate_of_Coq_Au_Vin_with_Mint_Garnish.png"),
                new Buff("Plate of Coq Au Vin with Salsa", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/8/80/Plate_of_Coq_Au_Vin_with_Salsa.png"),
                new Buff("Plate of Peppercorn-Spiced Beef Carpaccio", 57114, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/4/40/Plate_of_Peppercorn-Spiced_Beef_Carpaccio.png"),
                new Buff("Plate of Peppercorn-Spiced Coq Au Vin", 57260, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/4/43/Plate_of_Peppercorn-Spiced_Coq_Au_Vin.png"),
                new Buff("Plate of Peppercorn-Spiced Poultry Aspic", 57299, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/4/4f/Plate_of_Peppercorn-Spiced_Poultry_Aspic.png"),
                new Buff("Plate of Peppered Clear Truffle Ravioli", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/f/fe/Plate_of_Peppered_Clear_Truffle_Ravioli.png"),
                new Buff("Plate of Poultry Aspic with Mint Garnish", 57178, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/9/91/Plate_of_Poultry_Aspic_with_Mint_Garnish.png"),
                new Buff("Plate of Poultry Aspic with Salsa Garnish", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/5b/Plate_of_Poultry_Aspic_with_Salsa_Garnish.png"),
                new Buff("Plate of Sesame Poultry Aspic", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/6/64/Plate_of_Sesame_Poultry_Aspic.png"),
                new Buff("Plate of Sesame-Crusted Coq Au Vin", 57290, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/3/3e/Plate_of_Sesame-Crusted_Coq_Au_Vin.png"),
                new Buff("Plate of Sesame-Ginger Beef Carpaccio", 57231, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/b/b7/Plate_of_Sesame-Ginger_Beef_Carpaccio.png"),
                new Buff("Salsa Eggs Benedict", 57117, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/79/Salsa_Eggs_Benedict.png"),
                new Buff("Salsa-Topped Veggie Flatbread", 57269, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/f/f3/Salsa-Topped_Veggie_Flatbread.png"),
                new Buff("Sesame Cheesecake", 57328, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/e/ef/Sesame_Cheesecake.png"),
                new Buff("Sesame Creme Brulee", 57194, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/6/63/Sesame_Creme_Brulee.png"),
                new Buff("Sesame Eggs Benedict", 57084, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/f/f5/Sesame_Eggs_Benedict.png"),
                new Buff("Sesame Veggie Flatbread", 57050, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/d/db/Sesame_Veggie_Flatbread.png"),
                new Buff("Sesame-Asparagus and Cured Meat Flatbread", 57222, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/8/87/Sesame-Asparagus_and_Cured_Meat_Flatbread.png"),
                new Buff("Sous-Vide Steak with Mint-Parsley Sauce", 57342, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/9/99/Sous-Vide_Steak_with_Mint-Parsley_Sauce.png"),
                new Buff("Soy-Sesame Sous-Vide Steak", 57241, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/d/da/Soy-Sesame_Sous-Vide_Steak.png"),
                new Buff("Spherified Cilantro Oyster Soup", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/e/e1/Spherified_Cilantro_Oyster_Soup.png"),
                new Buff("Spherified Clove-Spiced Oyster Soup", 57374, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/f/fa/Spherified_Clove-Spiced_Oyster_Soup.png"),
                new Buff("Spherified Oyster Soup with Mint Garnish", 57201, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/6/63/Spherified_Oyster_Soup_with_Mint_Garnish.png"),
                new Buff("Spherified Peppercorn-Spiced Oyster Soup", 57165, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/4/43/Spherified_Peppercorn-Spiced_Oyster_Soup.png"),
                new Buff("Spherified Sesame Oyster Soup", 57037, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/51/Spherified_Sesame_Oyster_Soup.png"),
                new Buff("Spiced Pepper Creme Brulee", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/b/ba/Spiced_Pepper_Creme_Brulee.png"),
                new Buff("Spiced Peppercorn Cheesecake",-1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/9/9c/Spiced_Peppercorn_Cheesecake.png"),
                new Buff("Strawberry Cilantro Cheesecake", -1, GeneralHelper.Source.Item, BuffNature.Consumable, "https://wiki.guildwars2.com/images/8/8d/Strawberry_Cilantro_Cheesecake.png"),
        };

        public Dictionary<long, Buff> BuffsByIds { get; }
        public Dictionary<BuffNature, List<Buff>> BuffsByNature { get; }
        public Dictionary<GeneralHelper.Source, List<Buff>> BuffsBySource { get; }
        private readonly Dictionary<string, Buff> _buffsByName;

        private readonly BuffSourceFinder _buffSourceFinder;


        public BuffsContainer(ulong build, CombatData combatData, OperationController operation)
        {
            var AllBuffs = new List<List<Buff>>()
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
            var currentBuffs = new List<Buff>();
            foreach (List<Buff> buffs in AllBuffs)
            {
                currentBuffs.AddRange(buffs.Where(x => x.MaxBuild > build && build >= x.MinBuild));
            }
            _buffsByName = currentBuffs.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList().Count > 1 ? throw new InvalidOperationException("Same name present multiple times in buffs - " + x.First().Name) : x.First());
            // Unknown consumables
            var buffIDs = new HashSet<long>(currentBuffs.Select(x => x.ID));
            var foodAndUtility = new List<BuffInfoEvent>(combatData.GetBuffInfoEvent(BuffCategory.Enhancement));
            foodAndUtility.AddRange(combatData.GetBuffInfoEvent(BuffCategory.Food));
            foreach (BuffInfoEvent buffInfoEvent in foodAndUtility)
            {
                if (!buffIDs.Contains(buffInfoEvent.BuffID))
                {
                    string name = buffInfoEvent.Category == BuffCategory.Enhancement ? "Utility" : "Food";
                    string link = buffInfoEvent.Category == BuffCategory.Enhancement ? "https://wiki.guildwars2.com/images/2/23/Nourishment_utility.png" : "https://wiki.guildwars2.com/images/c/ca/Nourishment_food.png";
                    operation.UpdateProgressWithCancellationCheck("Unknown " + name + " " + buffInfoEvent.BuffID);
                    currentBuffs.Add(CreateCustomConsumable(name, buffInfoEvent.BuffID, link, buffInfoEvent.MaxStacks));
                }
            }
            //
            BuffsByIds = currentBuffs.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.First());
            BuffInfoSolver.AdjustBuffs(combatData, BuffsByIds, operation);
            foreach (Buff buff in currentBuffs)
            {
                BuffInfoEvent buffInfoEvt = buff.BuffInfo;
                if (buffInfoEvt != null)
                {
                    foreach (BuffFormula formula in buffInfoEvt.Formulas)
                    {
                        if (formula.Attr1 == BuffAttribute.Unknown)
                        {
                            operation.UpdateProgressWithCancellationCheck("Unknown Formula for " + buff.Name + ": " + formula.GetDescription(true, BuffsByIds));
                        }
                    }
                }
            }
            BuffsByNature = currentBuffs.GroupBy(x => x.Nature).ToDictionary(x => x.Key, x => x.ToList());
            BuffsBySource = currentBuffs.GroupBy(x => x.Source).ToDictionary(x => x.Key, x => x.ToList());
            //
            _buffSourceFinder = GetBuffSourceFinder(build, new HashSet<long>(BuffsByNature[BuffNature.Boon].Select(x => x.ID)));
        }

        public Buff GetBuffByName(string name)
        {
            if (_buffsByName.TryGetValue(name, out Buff buff))
            {
                return buff;
            }
            throw new InvalidOperationException("Buff " + name + " does not exist");
        }

        public AgentItem TryFindSrc(AgentItem dst, long time, long extension, ParsedLog log, long buffID)
        {
            return _buffSourceFinder.TryFindSrc(dst, time, extension, log, buffID);
        }

        // Non shareable buffs
        public List<Buff> GetRemainingBuffsList(string source)
        {
            var result = new List<Buff>();
            foreach (GeneralHelper.Source src in GeneralHelper.ProfToEnum(source))
            {
                if (BuffsBySource.TryGetValue(src, out List<Buff> list))
                {
                    result.AddRange(list.Where(x => x.Nature == BuffNature.GraphOnlyBuff));
                }
            }
            return result;
        }
    }
}
