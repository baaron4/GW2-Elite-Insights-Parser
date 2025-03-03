namespace GW2EIEvtcParser;

public static class ArcDPSEnums
{

    public const int ArcDPSPollingRate = 300;

    internal static class ArcDPSBuilds
    {
        internal const int StartOfLife = int.MinValue;
        //
        internal const int ProperConfusionDamageSimulation = 20210529;
        internal const int ScoringSystemChange = 20210800; // was somewhere around there
        internal const int DirectX11Update = 20210923;
        internal const int InternalSkillIDsChange = 20220304;
        internal const int BuffAttrFlatIncRemoved = 20220308;
        internal const int FunctionalIDToGUIDEvents = 20220709;
        internal const int NewLogStart = 20221111;
        internal const int Effect2Events = 20230718;
        internal const int FunctionalEffect2Events = 20230719;
        internal const int BuffExtensionBroken = 20230905;
        internal const int BuffExtensionOverstackValueChanged = 20231107;
        internal const int LingeringAgents = 20231110;
        internal const int RemovedDurationForInfiniteDurationStacksChanged = 20240211;
        internal const int NewMarkerEventBehavior = 20240418;
        internal const int Last90BeforeDownRetired = 20240529;
        internal const int StackType0ActiveChange = 20240609;
        internal const int TeamChangeOnDespawn = 20240612;
        internal const int WeaponSwapValueIsPrevious_CrowdControlEvents_GliderEvents = 20240627;
        internal const int MovementSkillDetection = 20240709;
        internal const int EICanDoManualBuffAttributes = 20240716;
        internal const int ExtraDataInGUIDEvents = 20241030;
        //
        internal const int EndOfLife = int.MaxValue;
    }

    public static class WeaponSetIDs
    {
        public const int NoSet = -1;
        public const int FirstLandSet = 4;
        public const int SecondLandSet = 5;
        public const int FirstWaterSet = 0;
        public const int SecondWaterSet = 1;
        public const int TransformSet = 3;
        public const int KitSet = 2;

        public static bool IsWeaponSet(int set)
        {
            return IsLandSet(set) || IsWaterSet(set);
        }

        public static bool IsLandSet(int set)
        {
            return set == FirstLandSet || set == SecondLandSet;
        }

        public static bool IsWaterSet(int set)
        {
            return set == FirstWaterSet || set == SecondWaterSet;
        }
    }

    /// <summary>
    /// Class containing <see cref="int"/> reward types.
    /// </summary>
    internal static class RewardTypes
    {
        internal const int Daily = 13;
        internal const int OldRaidReward1 = 55821; // On each kill
        internal const int OldRaidReward2 = 60685; // On each kill
        internal const int CurrentRaidReward = 22797; // Once per week
        internal const int PostEoDStrikeReward = 29453;
    }

    /// <summary>
    /// Class containing <see cref="ulong"/> reward IDs.
    /// </summary>
    internal static class RewardIDs
    {
        internal const ulong FreezieChest = 914;
        internal const ulong ShiverpeaksPassChests = 993; // Three chests, once a day
        internal const ulong KodansOldAndCurrentChest = 1035; // Old repeatable chest, now only once a day
        internal const ulong KodansCurrentChest1 = 1028; // Current, once a day
        internal const ulong KodansCurrentChest2 = 1032; // Current, once a day
        internal const ulong KodansCurrentRepeatableChest = 1091; // Current, repeatable
        internal const ulong FraenirRepeatableChest = 1007;
        internal const ulong BoneskinnerRepeatableChest = 1031;
        internal const ulong WhisperRepeatableChest = 1052;
    }

    // Activation
    public enum Activation : byte
    {
        None = 0,
        Normal = 1,
        Quickness = 2,
        CancelFire = 3,
        CancelCancel = 4,
        Reset = 5,

        Unknown
    };

    internal static Activation GetActivation(byte bt)
    {
        return bt < (byte)Activation.Unknown ? (Activation)bt : Activation.Unknown;
    }

    // Buff remove
    public enum BuffRemove : byte
    {
        None = 0,
        All = 1,
        Single = 2,
        Manual = 3,

        Unknown
    };

    internal static BuffRemove GetBuffRemove(byte bt)
    {
        return bt < (byte)BuffRemove.Unknown ? (BuffRemove)bt : BuffRemove.Unknown;
    }

    // Buff cycle
    public enum BuffCycle : byte
    {
        Cycle, // damage happened on tick timer
        NotCycle, // damage happened outside tick timer (resistable)
        NotCycle_NoResit, // BEFORE MAY 2021: the others were lumped here, now retired
        NotCycle_DamageToTargetOnHit, // damage happened to target on hiting target
        NotCycle_DamageToSourceOnHit, // damage happened to source on hiting target
        NotCycle_DamageToTargetOnStackRemove, // damage happened to target on source losing a stack
        Unknown
    };

    internal static BuffCycle GetBuffCycle(byte bt)
    {
        return bt < (byte)BuffCycle.Unknown ? (BuffCycle)bt : BuffCycle.Unknown;
    }

    // Result

    public enum PhysicalResult : byte
    {
        Normal = 0,
        Crit = 1,
        Glance = 2,
        Block = 3,
        Evade = 4,
        Interrupt = 5,
        Absorb = 6,
        Blind = 7,
        KillingBlow = 8,
        Downed = 9,
        BreakbarDamage = 10,
        Activation = 11,
        CrowdControl = 12,

        Unknown
    };

    internal static PhysicalResult GetPhysicalResult(byte bt)
    {
        return bt < (byte)PhysicalResult.Unknown ? (PhysicalResult)bt : PhysicalResult.Unknown;
    }

    public enum ConditionResult : byte
    {
        ExpectedToHit = 0,
        InvulByBuff = 1,
        InvulByPlayerSkill1 = 2,
        InvulByPlayerSkill2 = 3,
        InvulByPlayerSkill3 = 4,
        //BreakbarDamage = 5,

        Unknown
    };
    internal static ConditionResult GetConditionResult(byte bt)
    {
        return bt < (byte)ConditionResult.Unknown ? (ConditionResult)bt : ConditionResult.Unknown;
    }

    // State Change    
    public enum StateChange : byte
    {
        None = 0,
        EnterCombat = 1,
        ExitCombat = 2,
        ChangeUp = 3,
        ChangeDead = 4,
        ChangeDown = 5,
        Spawn = 6,
        Despawn = 7,
        HealthUpdate = 8,
        SquadCombatStart = 9,
        SquadCombatEnd = 10,
        WeaponSwap = 11,
        MaxHealthUpdate = 12,
        PointOfView = 13,
        Language = 14,
        GWBuild = 15,
        ShardId = 16,
        Reward = 17,
        BuffInitial = 18,
        Position = 19,
        Velocity = 20,
        Rotation = 21,
        TeamChange = 22,
        AttackTarget = 23,
        Targetable = 24,
        MapID = 25,
        ReplInfo = 26,
        StackActive = 27,
        StackReset = 28,
        Guild = 29,
        BuffInfo = 30,
        BuffFormula = 31,
        SkillInfo = 32,
        SkillTiming = 33,
        BreakbarState = 34,
        BreakbarPercent = 35,
        Integrity = 36,
        Marker = 37,
        BarrierUpdate = 38,
        StatReset = 39,
        Extension = 40,
        APIDelayed = 41,
        InstanceStart = 42,
        TickRate = 43,
        Last90BeforeDown = 44,
        Effect_45 = 45,
        EffectIDToGUID = 46,
        LogNPCUpdate = 47,
        Idle = 48,
        ExtensionCombat = 49,
        FractalScale = 50,
        Effect_51 = 51,
        RuleSet = 52,
        SquadMarker = 53,
        ArcBuild = 54,
        Glider = 55,
        StunBreak = 56,
        Unknown
    };

    internal static StateChange GetStateChange(byte bt)
    {
        return bt < (byte)StateChange.Unknown ? (StateChange)bt : StateChange.Unknown;
    }
    // Breakbar State

    public enum BreakbarState : byte
    {
        Active = 0,
        Recover = 1,
        Immune = 2,
        None = 3,
        Unknown
    };
    internal static BreakbarState GetBreakbarState(int value)
    {
        return value < (int)BreakbarState.Unknown ? (BreakbarState)value : BreakbarState.Unknown;
    }

    // Buff Formula

    // this enum is updated regularly to match the in game enum. The matching between the two is simply cosmetic, for clarity while comparing against an updated skill defs
    public enum BuffStackType : byte
    {
        StackingConditionalLoss = 0, // the same thing as Stacking but individual stacks can be removed
        Queue = 1,
        StackingUniquePerSrc = 2, // the same thing as Stacking but individual stacks can be extended and one src can only have one stack active at a time
        Regeneration = 3,
        Stacking = 4,
        Force = 5,
        Unknown,
    };
    internal static BuffStackType GetBuffStackType(byte bt)
    {
        return bt < (byte)BuffStackType.Unknown ? (BuffStackType)bt : BuffStackType.Unknown;
    }

    public enum BuffAttribute : short
    {
        None = 0,
        Power = 1,
        Precision = 2,
        Toughness = 3,
        Vitality = 4,
        Ferocity = 5,
        Healing = 6,
        Condition = 7,
        Concentration = 8,
        Expertise = 9,
        Armor = 10,
        Agony = 11,
        StatOutgoing = 12,
        FlatOutgoing = 13,
        PhysOutgoing = 14,
        CondOutgoing = 15,
        PhysIncomingAdditive = 16,
        CondIncomingAdditive = 17,
        AttackSpeed = 18,
        UnusedSiphonOutgoing_Arc = 19, // Unused due to being auto detected by the solver
        SiphonIncomingAdditive1 = 20,
        //
        Unknown = short.MaxValue,
        //
        /*ConditionDurationIncrease = 24,
        RetaliationDamageOutput = 25,
        CriticalChance = 26,
        PowerDamageToHP = 34,
        ConditionDamageToHP = 35,
        GlancingBlow = 47,
        ConditionSkillActivationFormula = 52,
        ConditionDamageFormula = 54,
        ConditionMovementActivationFormula = 55,
        EnduranceRegeneration = 61,
        IncomingHealingEffectiveness = 65,
        OutgoingHealingEffectivenessFlatInc = 68,
        OutgoingHealingEffectivenessConvInc = 70,
        RegenerationHealingOutput = 71,
        ExperienceFromKills = 76,
        GoldFind = 77,
        MovementSpeed = 78,
        KarmaBonus = 87,
        SkillCooldown = 96,
        MagicFind = 92,
        ExperienceFromAll = 100,
        WXP = 112,*/
        // Custom Ids, matched using a very simple pattern detection, see BuffInfoSolver.cs
        ConditionDurationOutgoing = -1,
        DamageFormulaSquaredLevel = -2,
        CriticalChance = -3,
        StrikeDamageToHP = -4,
        ConditionDamageToHP = -5,
        GlancingBlow = -6,
        SkillActivationDamageFormula = -7,
        DamageFormula = -8,
        MovementActivationDamageFormula = -9,
        EnduranceRegeneration = -10,
        HealingEffectivenessIncomingNonStacking = -11,
        HealingEffectivenessOutgoingAdditive = -12,
        HealingEffectivenessConvOutgoing = -13,
        HealingOutputFormula = -14,
        ExperienceFromKills = -15,
        GoldFind = -16,
        MovementSpeed = -17,
        KarmaBonus = -18,
        SkillRechargeSpeedIncrease = -19,
        MagicFind = -20,
        ExperienceFromAll = -21,
        WXP = -22,
        SiphonOutgoing = -23,
        PhysIncomingMultiplicative = -24,
        CondIncomingMultiplicative = -25,
        BoonDurationOutgoing = -26,
        HealingEffectivenessIncomingAdditive = -27,
        MovementSpeedStacking = -28,
        MovementSpeedStacking2 = -29,
        FishingPower = -30,
        MaximumHP = -31,
        VitalityPercent = -32,
        DefensePercent = -33,
        SiphonIncomingAdditive2 = -34,
        HealingEffectivenessIncomingMultiplicative = -35,
        AllStatsPercent = -36,
    }

    internal static BuffAttribute GetBuffAttribute(short bt, int evtcBuild)
    {
        if (evtcBuild >= ArcDPSBuilds.EICanDoManualBuffAttributes)
        {
            return bt == 0 ? BuffAttribute.None : BuffAttribute.Unknown;
        }
        BuffAttribute res;
        if (evtcBuild >= ArcDPSBuilds.BuffAttrFlatIncRemoved)
        {
            // Enum has shifted by -1
            if (bt <= (byte)BuffAttribute.SiphonIncomingAdditive1 - 1)
            {
                // only apply +1 shift to enum higher or equal to the one removed
                res = bt < (byte)BuffAttribute.FlatOutgoing ? (BuffAttribute)(bt) : (BuffAttribute)(bt + 1);
            }
            else
            {
                res = BuffAttribute.Unknown;
            }
        }
        else
        {
            res = bt <= (byte)BuffAttribute.SiphonIncomingAdditive1 ? (BuffAttribute)bt : BuffAttribute.Unknown;
        }
        if (res == BuffAttribute.UnusedSiphonOutgoing_Arc)
        {
            res = BuffAttribute.Unknown;
        }
        return res;
    }

    // Broken
    /*
    public enum BuffCategory : byte
    {
        Boon = 0,
        Any = 1,
        Condition = 2,
        Food = 4,
        Upgrade = 6,
        Boost = 8,
        Trait = 11,
        Enhancement = 13,
        Stance = 16,
        Unknown = byte.MaxValue
    }
    internal static BuffCategory GetBuffCategory(byte bt)
    {
        return Enum.IsDefined(typeof(BuffCategory), bt) ? (BuffCategory)bt : BuffCategory.Unknown;
    }*/
    // WIP
    public enum SkillAction : byte
    {
        EffectHappened = 4,
        AnimationCompleted = 5,
        Unknown = byte.MaxValue,
    }
    internal static SkillAction GetSkillAction(byte bt)
    {
        return Enum.IsDefined(typeof(SkillAction), bt) ? (SkillAction)bt : SkillAction.Unknown;
    }

    // Squad Marker index

    public enum SquadMarkerIndex : byte
    {
        // To be verified
        Arrow = 0,
        Circle = 1,
        Heart = 2,
        Square = 3,
        Star = 4,
        Swirl = 5,
        Triangle = 6,
        X = 7,
        Unknown
    }
    internal static SquadMarkerIndex GetSquadMarkerIndex(byte bt)
    {
        return bt < (byte)SquadMarkerIndex.Unknown ? (SquadMarkerIndex)bt : SquadMarkerIndex.Unknown;
    }

    // Content local
    public enum ContentLocal : byte
    {
        Effect = 0,
        Marker = 1,
        Unknown
    }
    internal static ContentLocal GetContentLocal(byte bt)
    {
        return bt < (byte)ContentLocal.Unknown ? (ContentLocal)bt : ContentLocal.Unknown;
    }

    // Friend of for

    public enum IFF : byte
    {
        Friend = 0,
        Foe = 1,

        Unknown
    };

    internal static IFF GetIFF(byte bt)
    {
        return bt < (byte)IFF.Unknown ? (IFF)bt : IFF.Unknown;
    }

}
