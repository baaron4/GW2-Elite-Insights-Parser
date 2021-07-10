using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public abstract class HealingStatsExtensionHandler : AbstractExtensionHandler
    {

        public const uint EXT_HealingStats = 0x9c9b3c99;
        public enum EXTHealingType { All, HealingPower, ConversionBased };

        internal HealingStatsExtensionHandler(CombatItem c) : base(EXT_HealingStats, "Healing Stats")
        {
            Revision = 0;
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

    }
}
