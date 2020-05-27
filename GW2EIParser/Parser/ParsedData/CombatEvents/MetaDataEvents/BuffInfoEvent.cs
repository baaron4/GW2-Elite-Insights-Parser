using System;
using System.Collections.Generic;
using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BuffInfoEvent : AbstractMetaDataEvent
    {
        public long BuffID { get; }

        public bool ProbablyInvul { get; private set; }

        public bool ProbablyInvert { get; private set; }

        public ParseEnum.BuffCategory Category { get; private set; }

        public byte StackingTypeByte { get; private set; }
        public ParseEnum.BuffStackType StackingType { get; private set; }

        public bool ProbablyResistance { get; private set; }

        public ushort MaxStacks { get; private set; }
        public List<BuffFormula> FormulaList { get; } = new List<BuffFormula>();

        public BuffInfoEvent(CombatItem evtcItem) : base(evtcItem)
        {
            BuffID = evtcItem.SkillID;
            CompleteBuffInfoEvent(evtcItem);
        }

        public void CompleteBuffInfoEvent(CombatItem evtcItem)
        {
            if (evtcItem.SkillID != BuffID)
            {
                throw new InvalidOperationException("Non matching buff id in BuffDataEvent complete method");
            }
            switch (evtcItem.IsStateChange)
            {
                case ParseEnum.StateChange.BuffFormula:
                    BuildFromBuffFormula(evtcItem);
                    break;
                case ParseEnum.StateChange.BuffInfo:
                    BuildFromBuffInfo(evtcItem);
                    break;
                default:
                    throw new InvalidOperationException("Invalid combat event in BuffDataEvent complete method");
            }
        }

        private void BuildFromBuffInfo(CombatItem evtcItem)
        {
            ProbablyInvul = evtcItem.IsFlanking > 0;
            ProbablyInvert = evtcItem.IsShields > 0;
            Category = ParseEnum.GetBuffCategory(evtcItem.IsOffcycle);
            MaxStacks = evtcItem.SrcMasterInstid;
            StackingTypeByte = evtcItem.Pad1;
            StackingType = ParseEnum.GetBuffStackType(StackingTypeByte);
            ProbablyResistance = evtcItem.Pad2 > 0;
        }

        public void AdjustBuffInfo(Dictionary<byte, ParseEnum.BuffAttribute> solved)
        {
            FormulaList.Sort((x, y) => (x.TraitSelf + x.TraitSrc).CompareTo(y.TraitSrc + y.TraitSelf));
            if (solved.Count == 0)
            {
                return;
            }
            foreach (BuffFormula formula in FormulaList)
            {
                formula.AdjustUnknownFormulaAttributes(solved);
            }
        }

        private void BuildFromBuffFormula(CombatItem evtcItem)
        {
            FormulaList.Add(new BuffFormula(evtcItem, this));
        }

    }
}
