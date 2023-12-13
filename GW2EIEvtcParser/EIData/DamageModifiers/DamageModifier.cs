using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public abstract class DamageModifier
    {
        internal DamageModifierDescriptor DamageModDescriptor { get; }

        public DamageType CompareType => DamageModDescriptor.CompareType;
        public DamageType SrcType => DamageModDescriptor.SrcType;
        internal DamageSource DmgSrc { get; }
        public bool Multiplier => DamageModDescriptor.Multiplier;
        public bool SkillBased => DamageModDescriptor.SkillBased;

        public bool Approximate => DamageModDescriptor.Approximate;
        public ParserHelper.Source Src => DamageModDescriptor.Src;
        public string Icon => DamageModDescriptor.Icon;
        public string Name => DamageModDescriptor.Name;
        public int ID { get; protected set; }
        public string Tooltip { get; protected set; }

        public bool Incoming { get; protected set; }

        internal DamageModifier(DamageModifierDescriptor damageModDescriptor, DamageSource dmgSrc)
        {
            DamageModDescriptor = damageModDescriptor;
            Tooltip = damageModDescriptor.InitialTooltip;
            DmgSrc = dmgSrc;
            switch (DmgSrc)
            {
                case DamageSource.All:
                    Tooltip += "<br>Actor + Minions";
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
        internal abstract AgentItem GetFoe(AbstractHealthDamageEvent evt);


        internal List<DamageModifierEvent> ComputeDamageModifier(AbstractSingleActor actor, ParsedEvtcLog log)
        {
            return DamageModDescriptor.ComputeDamageModifier(actor, log, this);
        }

        public abstract int GetTotalDamage(AbstractSingleActor actor, ParsedEvtcLog log, AbstractSingleActor t, long start, long end);

        public abstract IReadOnlyList<AbstractHealthDamageEvent> GetHitDamageEvents(AbstractSingleActor actor, ParsedEvtcLog log, AbstractSingleActor t, long start, long end);
    }
}
