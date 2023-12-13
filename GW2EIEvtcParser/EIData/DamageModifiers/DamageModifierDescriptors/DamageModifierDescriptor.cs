using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.Interfaces;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class DamageModifierDescriptor : IVersionable
    {

        public DamageType CompareType { get; }
        public DamageType SrcType { get; }
        internal DamageSource DmgSrc { get; }
        protected double GainPerStack { get; }
        internal GainComputer GainComputer { get; }
        private ulong _minBuild { get; set; } = GW2Builds.StartOfLife;
        private ulong _maxBuild { get; set; } = GW2Builds.EndOfLife;
        public bool Multiplier => GainComputer.Multiplier;
        public bool SkillBased => GainComputer.SkillBased;

        public bool Approximate { get; protected set; } = false;
        public ParserHelper.Source Src { get; }
        public string Icon { get; }
        public string Name { get; }
        public string InitialTooltip { get; protected set; }

        internal DamageModifierMode Mode { get; } = DamageModifierMode.All;
        private List<DamageLogChecker> _dlCheckers { get; set; }

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
            ulong gw2Build = combatData.GetBuildEvent().Build;
            return gw2Build < _maxBuild && gw2Build >= _minBuild;
        }

        internal virtual bool Keep(FightLogic.ParseMode mode, EvtcParserSettings parserSettings)
        {
            // Remove approx based damage mods from PvP contexts
            if (Approximate)
            {
                if (mode == FightLogic.ParseMode.WvW || mode == FightLogic.ParseMode.sPvP)
                {
                    return false;
                }
            }
            if (Mode == DamageModifierMode.All)
            {     
                return true;
            }
            switch (mode)
            {
                case FightLogic.ParseMode.Unknown:
                case FightLogic.ParseMode.OpenWorld:
                    return Mode == DamageModifierMode.PvE;
                case FightLogic.ParseMode.FullInstance:
                case FightLogic.ParseMode.Instanced5:
                case FightLogic.ParseMode.Instanced10:
                case FightLogic.ParseMode.Benchmark:
                    return Mode == DamageModifierMode.PvE || Mode == DamageModifierMode.PvEInstanceOnly || Mode == DamageModifierMode.PvEWvW;
                case FightLogic.ParseMode.WvW:
                    return (Mode == DamageModifierMode.WvW || Mode == DamageModifierMode.sPvPWvW || Mode == DamageModifierMode.PvEWvW);
                case FightLogic.ParseMode.sPvP:
                    return Mode == DamageModifierMode.sPvP || Mode == DamageModifierMode.sPvPWvW;
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
}
