using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.Extensions;

public class HealingStatsExtensionHandler : ExtensionHandler
{

    public const uint EXT_HealingStats = 0x9c9b3c99;
    public enum EXTHealingType { All, HealingPower, ConversionBased, Hybrid };

    // from https://github.com/Krappa322/arcdps_healing_stats/blob/master/src/Skills.cpp
    internal readonly HashSet<long> HybridHealIDs =
    [
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
        DeadlyFeast,
        LifeLeechUW,
        BloodFrenzy,
        LesserSymbolOfProtection,
        BattleStandard,
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
        VampiricStrikesMinion,
        VampiricStrikesPlayer,
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
        EternalNight,
        GraspingShadows,
        DawnsRepose,
        MindShock,
        HauntShot,
        LifeSiphon,
        PathOfGluttony,
        Effervescence,
        HungeringMaelstrom,
        EnervationEcho,
        Gorge,
        RampartSplitter,
        EssenceOfLivingShadows,
        FriendlyFire,
        Journey,
        LineBreakerHeal,
        PathToVictoryWarrior,
        PathToVictoryBerserker,
        PathToVictorySpellbreaker,
        FriendlyFireIllu,
        EnervationBlade,
        Flourish,
        ValiantLeap,
        InspiringImagery,
        FrigidFlurry,
        PathToVictoryBladesworn,
        SoothingSplash,
        DeathlyEnervation,
        EchoingErosion,
        PathToVictoryParagon,
        RelicOfTheNauticalBeastDamageHealing,
    ];

    private readonly List<EXTHealingEvent> _healingEvents = [];
    private readonly List<EXTBarrierEvent> _barrierEvents = [];
    internal HealingStatsExtensionHandler(CombatItem c, uint revision) : base(EXT_HealingStats, "Healing Stats")
    {
        Revision = revision;
        SetVersion(c);
    }
    private void SetVersion(CombatItem c)
    {
        //int size = (int)(c.SrcAgent & 0xFF00000000000000) >> 56;
        //NOTE(Rennorb): size * 1 was used before, but the number of bytes required is predetermined 
        var bytes = new ByteBuffer(stackalloc byte[32]); // 32 * sizeof(char), char as in C not C#
        // 8 bytes
        bytes.PushNative(c.DstAgent);
        // 4 bytes
        bytes.PushNative(c.Value);
        // 4 bytes
        bytes.PushNative(c.BuffDmg);
        // 4 bytes
        bytes.PushNative(c.OverstackValue);
        // 4 bytes
        bytes.PushNative(c.SkillID);
        // 2 bytes
        bytes.PushNative(c.SrcInstid);
        // 2 bytes
        bytes.PushNative(c.DstInstid);
        // 2 bytes
        bytes.PushNative(c.SrcMasterInstid);
        // 2 bytes
        bytes.PushNative(c.DstMasterInstid);

        var nullTerm = bytes.AsUsedSpan().IndexOf((byte)0);
        Version = System.Text.Encoding.UTF8.GetString(nullTerm != -1 ? bytes.AsUsedSpan()[..nullTerm] : bytes);
    }
    public static bool SanitizeForSrc<T>(List<T> events) where T : EXTHealingExtensionEvent
    {
        if (events.Any(x => x.SrcIsPeer))
        {
            events.RemoveAll(x => !x.SrcIsPeer);
            return true;
        }
        return false;
    }

    public static bool SanitizeForDst<T>(List<T> events) where T : EXTHealingExtensionEvent
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
        combatItem.OverrideSrcAgent(src);
        AgentItem dst = agentData.GetAgentByInstID(combatItem.DstInstid, combatItem.Time);
        combatItem.OverrideDstAgent(dst);
    }

    internal override void AttachToCombatData(CombatData combatData, ParserController operation, ulong gw2Build)
    {
        operation.UpdateProgressWithCancellationCheck("Parsing: Attaching healing extension revision " + Revision + " combat events");
        //
        {
            var healData = _healingEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var (agent, events) in healData)
            {
                if (SanitizeForSrc(events) && agent.IsPlayer)
                {
                    RunningExtensionInternal.Add(agent);
                }
            }
            var healReceivedData = _healingEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var (agent, events) in healReceivedData)
            {
                if (SanitizeForDst(events) && agent.IsPlayer)
                {
                    RunningExtensionInternal.Add(agent);
                }
            }
            var healDataByID = _healingEvents.GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList());
            combatData.EXTHealingCombatData = new EXTHealingCombatData(healData, healReceivedData, healDataByID, HybridHealIDs);
            operation.UpdateProgressWithCancellationCheck("Parsing: Attached " + _healingEvents.Count + " heal events to CombatData");
        }
        //
        if (_barrierEvents.Count != 0)
        {
            var barrierData = _barrierEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<AgentItem, List<EXTBarrierEvent>> pair in barrierData)
            {
                if (SanitizeForSrc(pair.Value) && pair.Key.IsPlayer)
                {
                    RunningExtensionInternal.Add(pair.Key);
                }
            }
            var barrierReceivedData = _barrierEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<AgentItem, List<EXTBarrierEvent>> pair in barrierReceivedData)
            {
                if (SanitizeForDst(pair.Value) && pair.Key.IsPlayer)
                {
                    RunningExtensionInternal.Add(pair.Key);
                }
            }
            var barrierDataByID = _barrierEvents.GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList());
            combatData.EXTBarrierCombatData = new EXTBarrierCombatData(barrierData, barrierReceivedData, barrierDataByID);
            operation.UpdateProgressWithCancellationCheck("Parsing: Attached " + _barrierEvents.Count + " barrier events to CombatData");
        }
        int running = Math.Max(RunningExtensionInternal.Count, 1);
        operation.UpdateProgressWithCancellationCheck("Parsing: " + (running != 1 ? running + " players have the extension running" : running + " player has the extension running"));
        //
        operation.UpdateProgressWithCancellationCheck("Parsing: Attached healing extension combat events");
    }

}
