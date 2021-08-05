using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public abstract class HealingStatsExtensionHandler : AbstractExtensionHandler
    {

        public const uint EXT_HealingStats = 0x9c9b3c99;
        public enum EXTHealingType { All, HealingPower, ConversionBased, Hybrid };

        // from https://github.com/Krappa322/arcdps_healing_stats/blob/master/src/Skills.cpp
        internal readonly HashSet<long> HybridHealIDs = new HashSet<long>()
        {
            2654,  // Crashing Waves
            5549,  // Water Blast (Elementalist)
            5570,  // Signet of Water
            5595,  // Water Arrow
            9080,  // Leap of Faith
            9090,  // Symbol of Punishment (Writ of Persistence)
            9095,  // Symbol of Judgement (Writ of Persistence)
            9097,  // Symbol of Blades (Writ of Persistence)
            9108,  // Holy Strike
            9111,  // Symbol of Faith (Writ of Persistence)
            9140,  // Faithful Strike
            9143,  // Symbol of Swiftness (Writ of Persistence)
            9146,  // Symbol of Wrath (Writ of Persistence)
            9161,  // Symbol of Protection (Writ of Persistence)
            9192,  // Symbol of Spears (Writ of Persistence)
            9208,  // Symbol of Light (Writ of Persistence)
            9950,  // Nourishment (Blueberry Pie AND Slice of Rainbow Cake)
            9952,  // Nourishment (Strawberry Pie AND Cupcake)
            9954,  // Nourishment (Cherry Pie)
            9955,  // Nourishment (Blackberry Pie)
            9956,  // Nourishment (Mixed Berry Pie)
            9957,  // Nourishment (Omnomberry Pie AND Slice of Candied Dragon Roll)
            10190, // Cry of Frustration (Restorative Illusions)
            10191, // Mind Wrack (Restorative Illusions)
            10560, // Life Leech
            10563, // Life Siphon
            10619, // Deadly Feast
            12424, // Blood Frenzy
            15259, // Nourishment (Omnomberry Ghost)
            21656, // Arcane Brilliance
            24800, // Nourishment (Prickly Pear Pie AND Bowl of Cactus Fruit Salad)
            26557, // Vengeful Hammers
            26646, // Battle Scars
            29145, // Mender's Rebuke
            29789, // Symbol of Energy (Writ of Persistence)
            29856, // Well of Recall (All's Well That Ends Well)
            30359, // Gravity Well (All's Well That Ends Well)
            30285, // Vampiric Aura
            30488, // "Your Soul is Mine!"
            30525, // Well of Calamity (All's Well That Ends Well)
            30814, // Well of Action (All's Well That Ends Well)
            30864, // Tidal Surge
            33792, // Slice of Allspice Cake
            34207, // Nourishment (Scoop of Mintberry Swirl Ice Cream)
            37475, // Nourishment (Winterberry Pie)
            40624, // Symbol of Vengeance (Writ of Persistence)
            41052, // Sieche
            43199, // Breaking Wave
            44405, // Riptide
            45026, // Soulcleave's Summit
            45983, // Claptosis
            51646, // Transmute Frost
            51692, // Facet of Nature - Assassin
            56928, // Rewinder (Restorative Illusions)
            56930, // Split Second (Restorative Illusions)
            57117, // Nourishment (Salsa Eggs Benedict)
            57239, // Nourishment (Strawberry Cilantro Cheesecake) - Apparently this one has a separate id from the damage event
            57244, // Nourishment (Cilantro Lime Sous-Vide Steak)
            57253, // Nourishment (Coq Au Vin with Salsa)
            57267, // Nourishment (Mango Cilantro Creme Brulee)
            57269, // Nourishment (Salsa-Topped Veggie Flatbread)
            57295, // Nourishment (Clear Truffle and Cilantro Ravioli)
            57341, // Nourishment (Poultry Aspic with Salsa Garnish)
            57356, // Nourishment (Spherified Cilantro Oyster Soup)
            57401, // Nourishment (Fruit Salad with Cilantro Garnish)
            57409, // Nourishment (Cilantro and Cured Meat Flatbread)
        };

        internal virtual void SetVersion(CombatItem c)
        {
            var size = (c.SrcAgent & 0xFF00000000000000) >> 56;
            byte[] bytes = new byte[size * 1]; // 32 * sizeof(char), char as in C not C#
            uint offset = 0;
            // 8 bytes
            foreach (byte bt in BitConverter.GetBytes(c.DstAgent))
            {
                if (offset == size)
                {
                    break;
                }
                bytes[offset++] = bt;
            }
            // 4 bytes
            foreach (byte bt in BitConverter.GetBytes(c.Value))
            {
                if (offset == size)
                {
                    break;
                }
                bytes[offset++] = bt;
            }
            // 4 bytes
            foreach (byte bt in BitConverter.GetBytes(c.BuffDmg))
            {
                if (offset == size)
                {
                    break;
                }
                bytes[offset++] = bt;
            }
            // 4 bytes
            foreach (byte bt in BitConverter.GetBytes(c.OverstackValue))
            {
                if (offset == size)
                {
                    break;
                }
                bytes[offset++] = bt;
            }
            // 4 bytes
            foreach (byte bt in BitConverter.GetBytes(c.SkillID))
            {
                if (offset == size)
                {
                    break;
                }
                bytes[offset++] = bt;
            }
            // 2 bytes
            foreach (byte bt in BitConverter.GetBytes(c.SrcInstid))
            {
                if (offset == size)
                {
                    break;
                }
                bytes[offset++] = bt;
            }

            // 2 bytes
            foreach (byte bt in BitConverter.GetBytes(c.DstInstid))
            {
                if (offset == size)
                {
                    break;
                }
                bytes[offset++] = bt;
            }
            // 2 bytes
            foreach (byte bt in BitConverter.GetBytes(c.SrcMasterInstid))
            {
                if (offset == size)
                {
                    break;
                }
                bytes[offset++] = bt;
            }
            // 2 bytes
            foreach (byte bt in BitConverter.GetBytes(c.DstMasterInstid))
            {
                if (offset == size)
                {
                    break;
                }
                bytes[offset++] = bt;
            }
            Version = System.Text.Encoding.UTF8.GetString(bytes);
        }

        internal HashSet<long> GetHybridIDs(ulong gw2Build)
        {
            return new HashSet<long>(HybridHealIDs);
        }

        internal HealingStatsExtensionHandler() : base(EXT_HealingStats, "Healing Stats")
        {
        }

    }
}
