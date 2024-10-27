using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.Interfaces;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

internal abstract class DamageModifierDescriptor : IVersionable
{
    public readonly DamageType CompareType;
    public readonly DamageType SrcType;
    internal readonly DamageSource DmgSrc;
    protected readonly double GainPerStack;
    internal readonly GainComputer GainComputer;
    private ulong _minBuild = GW2Builds.StartOfLife;
    private ulong _maxBuild = GW2Builds.EndOfLife;
    private int _minEvtcBuild = ArcDPSBuilds.StartOfLife;
    private int _maxEvtcBuild = ArcDPSBuilds.EndOfLife;
    public bool Multiplier => GainComputer.Multiplier;
    public bool SkillBased => GainComputer.SkillBased;

    public bool Approximate { get; protected set; } = false;
    public readonly ParserHelper.Source Src;
    public readonly string Icon;
    public readonly string Name;
    public string InitialTooltip { get; protected set; }

    internal readonly DamageModifierMode Mode = DamageModifierMode.All;
    private List<DamageLogChecker> _dlCheckers;

    internal DamageModifierDescriptor(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, string icon, GainComputer gainComputer, DamageModifierMode mode)
    {
        InitialTooltip = tooltip;
        Name = name;
        DmgSrc = damageSource;
        GainPerStack = gainPerStack;
        if (GainPerStack == 0.0)
        {
            throw new InvalidOperationException("Gain per stack can't be 0");
        }
        CompareType = compareType;
        SrcType = srctype;
        Src = src;
        Icon = icon;
        GainComputer = gainComputer;
        Mode = mode;
        _dlCheckers = new List<DamageLogChecker>();
    }

    internal DamageModifierDescriptor WithBuilds(ulong minBuild, ulong maxBuild = GW2Builds.EndOfLife)
    {
        _minBuild = minBuild;
        _maxBuild = maxBuild;
        return this;
    }

    internal DamageModifierDescriptor WithEvtcBuilds(int minBuild, int maxBuild = ArcDPSBuilds.EndOfLife)
    {
        _minEvtcBuild = minBuild;
        _maxEvtcBuild = maxBuild;
        return this;
    }

    internal virtual DamageModifierDescriptor UsingChecker(DamageLogChecker dlChecker)
    {
        _dlCheckers.Add(dlChecker);
        return this;
    }

    protected bool CheckCondition(AbstractHealthDamageEvent dl, ParsedEvtcLog log)
    {
        return _dlCheckers.All(checker => checker(dl, log));
    }

    public bool Available(CombatData combatData)
    {
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

    internal virtual bool Keep(FightLogic.ParseModeEnum parseMode, FightLogic.SkillModeEnum skillMode, EvtcParserSettings parserSettings)
    {
        // Remove approx based damage mods from PvP contexts
        if (Approximate)
        {
            if (parseMode == FightLogic.ParseModeEnum.WvW || parseMode == FightLogic.ParseModeEnum.sPvP)
            {
                return false;
            }
        }
        if (Mode == DamageModifierMode.All)
        {
            return true;
        }
        switch (skillMode)
        {
            case FightLogic.SkillModeEnum.PvE:
                if (parseMode == FightLogic.ParseModeEnum.OpenWorld || parseMode == FightLogic.ParseModeEnum.Unknown)
                {
                    return !Approximate && (Mode == DamageModifierMode.PvE || Mode == DamageModifierMode.PvEWvW || Mode == DamageModifierMode.PvEsPvP);
                }
                return Mode == DamageModifierMode.PvE || Mode == DamageModifierMode.PvEInstanceOnly || Mode == DamageModifierMode.PvEWvW || Mode == DamageModifierMode.PvEsPvP;
            case FightLogic.SkillModeEnum.WvW:
                return (Mode == DamageModifierMode.WvW || Mode == DamageModifierMode.sPvPWvW || Mode == DamageModifierMode.PvEWvW);
            case FightLogic.SkillModeEnum.sPvP:
                return Mode == DamageModifierMode.sPvP || Mode == DamageModifierMode.sPvPWvW || Mode == DamageModifierMode.PvEsPvP;
        }
        return false;
    }

    protected abstract bool ComputeGain(IReadOnlyDictionary<long, BuffsGraphModel> bgms, AbstractHealthDamageEvent dl, ParsedEvtcLog log, out double gain);

    internal DamageModifierDescriptor UsingApproximate(bool approximate)
    {
        Approximate = approximate;
        return this;
    }
    internal abstract List<DamageModifierEvent> ComputeDamageModifier(AbstractSingleActor actor, ParsedEvtcLog log, DamageModifier damageModifier);

}
