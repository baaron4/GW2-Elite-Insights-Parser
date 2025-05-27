using GW2EIEvtcParser.Interfaces;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

internal abstract class InstantCastFinder : IVersionable
{

    public enum InstantCastOrigin
    {
        Skill,
        Trait,
        Gear
    }

    public InstantCastOrigin CastOrigin { get; private set; } = InstantCastOrigin.Skill;

    public delegate bool InstantCastEnableChecker(CombatData combatData);
    private readonly List<InstantCastEnableChecker> _enableConditions;


    public const long DefaultICD = 50;
    public readonly long SkillID;

    public bool NotAccurate { get; private set; } = false;

    protected long TimeOffset = 0;

    protected bool BeforeWeaponSwap = false;
    protected bool AfterWeaponSwap = false;

    protected long ICD { get; private set; } = DefaultICD;

    private ulong _maxBuild = GW2Builds.EndOfLife;
    private ulong _minBuild = GW2Builds.StartOfLife;
    private int _maxEvtcBuild = ArcDPSBuilds.EndOfLife;
    private int _minEvtcBuild = ArcDPSBuilds.StartOfLife;

    protected InstantCastFinder(long skillID)
    {
        SkillID = skillID;
        _enableConditions = [];
    }

    internal InstantCastFinder WithBuilds(ulong minBuild, ulong maxBuild = GW2Builds.EndOfLife)
    {
        _maxBuild = maxBuild;
        _minBuild = minBuild;
        return this;
    }

    internal InstantCastFinder WithEvtcBuilds(int minBuild, int maxBuild = ArcDPSBuilds.EndOfLife)
    {
        _maxEvtcBuild = maxBuild;
        _minEvtcBuild = minBuild;
        return this;
    }

    internal InstantCastFinder UsingICD(long icd = DefaultICD)
    {
        ICD = icd;
        return this;
    }

    internal InstantCastFinder UsingNotAccurate()
    {
        NotAccurate = true;
        return this;
    }

    internal InstantCastFinder UsingOrigin(InstantCastOrigin origin)
    {
        CastOrigin = origin;
        return this;
    }

    internal InstantCastFinder UsingEnable(InstantCastEnableChecker checker)
    {
        _enableConditions.Add(checker);
        return this;
    }

    internal InstantCastFinder UsingDisableWithEffectData()
    {
        return UsingEnable(combatData => !combatData.HasEffectData);
    }

    internal InstantCastFinder UsingDisableWithMissileData()
    {
        return UsingEnable(combatData => !combatData.HasMissileData);
    }

    internal virtual InstantCastFinder UsingTimeOffset(long timeOffset)
    {
        TimeOffset = timeOffset;
        return this;
    }

    internal virtual InstantCastFinder UsingBeforeWeaponSwap(bool beforeWeaponSwap)
    {
        BeforeWeaponSwap = beforeWeaponSwap;
        AfterWeaponSwap = false;
        return this;
    }

    internal virtual InstantCastFinder UsingAfterWeaponSwap(bool afterWeaponSwap)
    {
        AfterWeaponSwap = afterWeaponSwap;
        BeforeWeaponSwap = false;
        return this;
    }

    protected long GetTime(TimeCombatEvent evt, AgentItem caster, CombatData combatData)
    {
        long time = evt.Time + TimeOffset;
        if (BeforeWeaponSwap || AfterWeaponSwap)
        {
            var wepSwaps = combatData.GetWeaponSwapData(caster).Where(x => Math.Abs(x.Time - time) < ServerDelayConstant / 2).ToList();
            if (wepSwaps.Count != 0)
            {
                return BeforeWeaponSwap ? Math.Min(wepSwaps[0].Time - 1, time) : Math.Max(wepSwaps[0].Time + 1, time);
            }
        }
        return time;
    }


    public bool Available(CombatData combatData)
    {
        if (!_enableConditions.All(checker => checker(combatData)))
        {
            return false;
        }
        ulong gw2Build = combatData.GetGW2BuildEvent().Build;
        if (gw2Build < _maxBuild && gw2Build >= _minBuild)
        {
            int evtcBuild = combatData.GetEvtcVersionEvent().Build;
            if (evtcBuild < _maxEvtcBuild && evtcBuild >= _minEvtcBuild)
            {
                return true;
            }
        }
        return false;
    }

    public abstract List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData);
}
