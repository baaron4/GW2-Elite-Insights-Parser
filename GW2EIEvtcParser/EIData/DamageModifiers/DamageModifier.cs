using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

public abstract class DamageModifier
{
    internal readonly DamageModifierDescriptor DamageModDescriptor;

    public DamageType CompareType => DamageModDescriptor.CompareType;
    public DamageType SrcType => DamageModDescriptor.SrcType;
    internal readonly DamageSource DmgSrc;
    public bool Multiplier => DamageModDescriptor.Multiplier;
    public bool SkillBased => DamageModDescriptor.SkillBased;

    public bool Approximate => DamageModDescriptor.Approximate;
    public Source Src => DamageModDescriptor.Src;
    public string Icon => DamageModDescriptor.Icon;
    public string Name => DamageModDescriptor.Name;
    /// <remarks>Not stable across restarts because it uses `Name.GetHashCode()`.</remarks>
    public int ID => Incoming ? -DamageModDescriptor.ID : DamageModDescriptor.ID;
    public string Tooltip { get; protected set; }

    public bool Incoming { get; protected set; }

    public bool NeedsMinions => DmgSrc == DamageSource.All || DmgSrc == DamageSource.PetsOnly;

    internal DamageModifier(DamageModifierDescriptor damageModDescriptor)
    {
        DamageModDescriptor = damageModDescriptor;
        Tooltip = damageModDescriptor.InitialTooltip;
        DmgSrc = damageModDescriptor.DmgSrc;
        switch (DmgSrc)
        {
            case DamageSource.All:
                Tooltip += "<br>Actor + Minions";
                break;
            case DamageSource.PetsOnly:
                Tooltip += "<br>Minions only";
                break;
            case DamageSource.NoPets:
                Tooltip += "<br>No Minions";
                break;
            default:
                break;
        }
        Tooltip += "<br>Applied on " + SrcType.DamageTypeToString();
        Tooltip += "<br>Compared against " + CompareType.DamageTypeToString();
        if (!Multiplier)
        {
            Tooltip += "<br>Non multiplier";
        }
        if (Approximate)
        {
            Tooltip += "<br>Approximate";
        }
    }
    internal abstract AgentItem GetFoe(HealthDamageEvent evt);


    internal List<DamageModifierEvent> ComputeDamageModifier(SingleActor actor, ParsedEvtcLog log)
    {
        return DamageModDescriptor.ComputeDamageModifier(actor, log, this);
    }

    public abstract int GetTotalDamage(SingleActor actor, ParsedEvtcLog log, SingleActor? t, long start, long end);

    public abstract IEnumerable<HealthDamageEvent> GetHitDamageEvents(SingleActor actor, ParsedEvtcLog log, SingleActor? t, long start, long end);

    public IEnumerable<HealthDamageEvent> GetHitDamageEvents(SingleActor actor, ParsedEvtcLog log, SingleActor? t)
    {
        return GetHitDamageEvents(actor, log, t, log.FightData.FightStart, log.FightData.FightEnd);
    }
}
