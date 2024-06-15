using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.Extensions
{
    public class HealingStatsExtensionHandler : AbstractExtensionHandler
    {

        public const uint EXT_HealingStats = 0x9c9b3c99;
        public enum EXTHealingType { All, HealingPower, ConversionBased, Hybrid };

        // from https://github.com/Krappa322/arcdps_healing_stats/blob/master/src/Skills.cpp
        internal readonly HashSet<long> HybridHealIDs = new HashSet<long>()
        {
            CrashingWaves,
            WaterBlast,
            WaterTrident,
            SignetOfWaterHeal,
            WaterArrow,
            LeapOfFaith,
            SymbolOfPunishment,
            SymbolOfJudgement,
            SymbolOfBlades,
            HolyStrike,
            SymbolOfFaith,
            FaithfulStrike,
            SymbolOfSwiftness,
            SymbolOfWrath_SymbolOfResolution,
            SymbolOfProtection,
            SymbolOfSpears,
            SymbolOfLight,
            BlueberryPieAndSliceOfRainbowCake,
            StrawberryPieAndCupcake,
            CherryPie,
            BlackberryPie,
            MixedBerryPie,
            OmnomberryPieAndSliceOfCandiedDragonRoll,
            CryOfFrustration,
            MindWrack,
            MindWrackAmmo,
            LifeLeech,
            LifeSiphonOld,
            LifeSiphon,
            DeadlyFeast,
            LifeLeechUW,
            BloodFrenzy,
            LesserSymbolOfProtection,
            OmnomberryGhost,
            ArcaneBrilliance,
            PricklyPearPie,
            VengefulHammersSkill,
            BattleScars,
            MendersRebuke,
            SymbolOfEnergy,
            WellOfRecall_Senility,
            GravityWell,
            VampiricAura,
            VampiricStrikes,
            VampiricStrikes2,
            YourSoulIsMine,
            WellOfCalamity,
            WellOfAction,
            TidalSurge,
            SliceOfAllspiceCake,
            ScoopOfMintberrySwirlIceCream,
            WinterberryPie,
            SymbolOfVengeance,
            Sieche,
            BreakingWave,
            Riptide,
            SoulcleavesSummitBuff,
            Claptosis,
            TransmuteFrost,
            FacetOfNatureAssassin,
            Rewinder,
            SplitSecond,
            SalsaEggsBenedict,
            StrawberryCilantroCheesecake,
            CilantroLimeSousVideSteak,
            PlateOfCoqAuVinWithSalsa,
            MangoCilantroCremeBrulee,
            SalsaToppedVeggieFlatbread,
            PlateOfClearTruffleAndCilantroRavioli,
            PlateOfPoultryAspicWithSalsaGarnish,
            SpherifiedCilantroOysterSoup,
            BowlOfFruitSaladWithCilantroGarnish,
            CilantroAndCuredMeatFlatbread,
            LifeTransfer,
            GatheringPlague,
            SoulSpiral,
            GarishPillarSkill,
            WingsOfResolveSkill,
            ElixirOfPromise,
            HauntShot,
            GraspingShadows,
            DawnsRepose,
            EternalNight,
            MindShock,
            Flourish,
            EchoingErosion,
            FrigidFlurry,
            SoothingSplash,
            Journey,
            FriendlyFire,
            FriendlyFireIllu,
            InspiringImagery,
            Effervescence,
            RampartSplitter,
            PathToVictory,
            ValiantLeap,
            Gorge,
            PathOfGluttony,
            HungeringMaelstrom,
            EnervationBlade,
            EnervationEcho,
            DeathlyEnervation,
            EssenceOfLivingShadows,
        };

        private readonly List<EXTAbstractHealingEvent> _healingEvents = new List<EXTAbstractHealingEvent>();
        private readonly List<EXTAbstractBarrierEvent> _barrierEvents = new List<EXTAbstractBarrierEvent>();
        internal HealingStatsExtensionHandler(CombatItem c, uint revision) : base(EXT_HealingStats, "Healing Stats")
        {
            Revision = revision;
            SetVersion(c);
        }
        private void SetVersion(CombatItem c)
        {
            ulong size = (c.SrcAgent & 0xFF00000000000000) >> 56;
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
        public static bool SanitizeForSrc<T>(List<T> events) where T : EXTAbstractHealingExtensionEvent
        {
            if (events.Any(x => x.SrcIsPeer))
            {
                events.RemoveAll(x => !x.SrcIsPeer);
                return true;
            }
            return false;
        }

        public static bool SanitizeForDst<T>(List<T> events) where T : EXTAbstractHealingExtensionEvent
        {
            if (events.Any(x => x.DstIsPeer))
            {
                events.RemoveAll(x => !x.DstIsPeer);
                return true;
            }
            return false;
        }

        private static bool IsHealingEvent(CombatItem c)
        {
            return c.IsShields == 0 && ((c.IsBuff == 0 && c.Value < 0) || (c.IsBuff != 0 && c.Value == 0 && c.BuffDmg < 0));
        }

        // To be exploited later
        private static bool IsBarrierEvent(CombatItem c)
        {
            return c.IsShields > 0 && ((c.IsBuff == 0 && c.Value < 0) || (c.IsBuff != 0 && c.Value == 0 && c.BuffDmg < 0));
        }

        private HashSet<long> GetHybridIDs(ulong gw2Build)
        {
            return new HashSet<long>(HybridHealIDs);
        }

        internal override bool HasTime(CombatItem c)
        {
            return true;
        }

        internal override bool IsSkillID(CombatItem c)
        {
            return true;
        }

        internal override bool SrcIsAgent(CombatItem c)
        {
            return IsHealingEvent(c) || IsBarrierEvent(c);
        }
        internal override bool DstIsAgent(CombatItem c)
        {
            return SrcIsAgent(c);
        }

        internal override bool IsDamage(CombatItem c)
        {
            return SrcIsAgent(c);
        }

        internal override bool IsDamagingDamage(CombatItem c)
        {
            return IsDamage(c);
        }

        internal override void InsertEIExtensionEvent(CombatItem c, AgentData agentData, SkillData skillData)
        {
            bool isHealing = IsHealingEvent(c);
            bool isBarrier = IsBarrierEvent(c);
            if (!isHealing && !isBarrier)
            {
                return;
            }
            if (c.IsBuff == 0 && c.Value < 0)
            {
                if (isHealing)
                {
                    _healingEvents.Add(new EXTDirectHealingEvent(c, agentData, skillData));
                }
                else if (isBarrier)
                {
                    _barrierEvents.Add(new EXTDirectBarrierEvent(c, agentData, skillData));
                }
            }
            else if (c.IsBuff != 0 && c.Value == 0 && c.BuffDmg < 0)
            {
                if (isHealing)
                {
                    _healingEvents.Add(new EXTNonDirectHealingEvent(c, agentData, skillData));
                }
                else if (isBarrier)
                {
                    _barrierEvents.Add(new EXTNonDirectBarrierEvent(c, agentData, skillData));
                }
            }
        }

        internal override void AdjustCombatEvent(CombatItem combatItem, AgentData agentData)
        {
            if (!IsHealingEvent(combatItem) && !IsBarrierEvent(combatItem))
            {
                return;
            }
            // Prefer instid fetch for healing events
            AgentItem src = agentData.GetAgentByInstID(combatItem.SrcInstid, combatItem.Time);
            combatItem.OverrideSrcAgent(src.Agent);
            AgentItem dst = agentData.GetAgentByInstID(combatItem.DstInstid, combatItem.Time);
            combatItem.OverrideDstAgent(dst.Agent);
        }

        internal override void AttachToCombatData(CombatData combatData, ParserController operation, ulong gw2Build)
        {
            operation.UpdateProgressWithCancellationCheck("Parsing: Attaching healing extension revision " + Revision + " combat events");
            //
            {
                var healData = _healingEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
                foreach (KeyValuePair<AgentItem, List<EXTAbstractHealingEvent>> pair in healData)
                {
                    if (SanitizeForSrc(pair.Value) && pair.Key.IsPlayer)
                    {
                        RunningExtensionInternal.Add(pair.Key);
                    }
                }
                var healReceivedData = _healingEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
                foreach (KeyValuePair<AgentItem, List<EXTAbstractHealingEvent>> pair in healReceivedData)
                {
                    if (SanitizeForDst(pair.Value) && pair.Key.IsPlayer)
                    {
                        RunningExtensionInternal.Add(pair.Key);
                    }
                }
                var healDataById = _healingEvents.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
                combatData.EXTHealingCombatData = new EXTHealingCombatData(healData, healReceivedData, healDataById, GetHybridIDs(gw2Build));
                operation.UpdateProgressWithCancellationCheck("Parsing: Attached " + _healingEvents.Count + " heal events to CombatData");
            }
            //
            if (_barrierEvents.Count != 0)
            {
                var barrierData = _barrierEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
                foreach (KeyValuePair<AgentItem, List<EXTAbstractBarrierEvent>> pair in barrierData)
                {
                    if (SanitizeForSrc(pair.Value) && pair.Key.IsPlayer)
                    {
                        RunningExtensionInternal.Add(pair.Key);
                    }
                }
                var barrierReceivedData = _barrierEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
                foreach (KeyValuePair<AgentItem, List<EXTAbstractBarrierEvent>> pair in barrierReceivedData)
                {
                    if (SanitizeForDst(pair.Value) && pair.Key.IsPlayer)
                    {
                        RunningExtensionInternal.Add(pair.Key);
                    }
                }
                var barrierDataById = _barrierEvents.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
                combatData.EXTBarrierCombatData = new EXTBarrierCombatData(barrierData, barrierReceivedData, barrierDataById);
                operation.UpdateProgressWithCancellationCheck("Parsing: Attached " + _barrierEvents.Count + " barrier events to CombatData");
            }
            int running = Math.Max(RunningExtensionInternal.Count, 1);
            operation.UpdateProgressWithCancellationCheck("Parsing: " + (running != 1 ? running + " players have the extension running" : running + " player has the extension running"));
            //
            operation.UpdateProgressWithCancellationCheck("Parsing: Attached healing extension combat events");
        }

    }
}
