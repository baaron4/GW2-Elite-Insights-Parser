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

        public byte StackingType { get; private set; }

        public bool ProbablyResistance { get; private set; }

        public ushort MaxStacks { get; private set; }

        public class BuffFormula
        {
            public int Type { get; }

            public ParseEnum.BuffAttribute Attr1 { get; private set; }

            public ParseEnum.BuffAttribute Attr2 { get; private set; }
            public byte ByteAttr1 { get; }

            public byte ByteAttr2 { get; }

            public float ConstantOffset { get; }

            public float LevelOffset { get; }

            public float Variable { get; }

            public int TraitSrc { get; }

            public int TraitSelf { get; }

            public bool NPC { get; }

            public bool Player { get; }

            public bool Break { get; }

            private string _solvedDescription = null;

            public BuffFormula(CombatItem evtcItem)
            {
                NPC = evtcItem.IsFlanking == 0;
                Player = evtcItem.IsShields == 0;
                Break = evtcItem.IsOffcycle > 0;
                byte[] formulaBytes = new byte[8 * sizeof(float)];
                int offset = 0;
                // 2 
                foreach (byte bt in BitConverter.GetBytes(evtcItem.Time))
                {
                    formulaBytes[offset++] = bt;
                }
                // 2
                foreach (byte bt in BitConverter.GetBytes(evtcItem.SrcAgent))
                {
                    formulaBytes[offset++] = bt;
                }
                // 2
                foreach (byte bt in BitConverter.GetBytes(evtcItem.DstAgent))
                {
                    formulaBytes[offset++] = bt;
                }
                // 1
                foreach (byte bt in BitConverter.GetBytes(evtcItem.Value))
                {
                    formulaBytes[offset++] = bt;
                }
                // 1
                foreach (byte bt in BitConverter.GetBytes(evtcItem.BuffDmg))
                {
                    formulaBytes[offset++] = bt;
                }
                //
                float[] formulaFloats = new float[8];
                Buffer.BlockCopy(formulaBytes, 0, formulaFloats, 0, formulaBytes.Length);
                //
                Type = (int)formulaFloats[0];
                ByteAttr1 = (byte)formulaFloats[1];
                ByteAttr2 = (byte)formulaFloats[2];
                Attr1 = ParseEnum.GetBuffAttribute(ByteAttr1);
                Attr2 = ParseEnum.GetBuffAttribute(ByteAttr2);
                ConstantOffset = formulaFloats[3];
                LevelOffset = formulaFloats[4];
                Variable = formulaFloats[5];
                TraitSrc = (int)formulaFloats[6];
                TraitSelf = (int)formulaFloats[7];
            }

            public void AdjustUnknownFormulaAttributes(Dictionary<byte, ParseEnum.BuffAttribute> solved)
            {
                if (Attr1 == ParseEnum.BuffAttribute.Unknown && solved.TryGetValue(ByteAttr1, out ParseEnum.BuffAttribute solvedAttr))
                {
                    Attr1 = solvedAttr;
                }
                if (Attr2 == ParseEnum.BuffAttribute.Unknown && solved.TryGetValue(ByteAttr2, out solvedAttr))
                {
                    Attr2 = solvedAttr;
                }
            }

            public string GetDescription(bool authorizeUnknowns, Dictionary<long, Buff> buffsByIds)
            {
                if (_solvedDescription != null)
                {
                    return _solvedDescription;
                }
                _solvedDescription = "";
                if (authorizeUnknowns || (Attr1 != ParseEnum.BuffAttribute.Unknown && Attr2 != ParseEnum.BuffAttribute.Unknown))
                {
                    if (Attr1 == ParseEnum.BuffAttribute.None)
                    {
                        return _solvedDescription;
                    }
                    var stat1 = Enum.GetName(typeof(ParseEnum.BuffAttribute), Attr1);
                    if (Attr1 == ParseEnum.BuffAttribute.Unknown)
                    {
                        stat1 += " " + ByteAttr1;
                    }
                    var stat2 = Enum.GetName(typeof(ParseEnum.BuffAttribute), Attr2);
                    if (Attr2 == ParseEnum.BuffAttribute.Unknown)
                    {
                        stat2 += " " + ByteAttr2;
                    }
                    _solvedDescription += stat1;
                    if (Attr2 != ParseEnum.BuffAttribute.None)
                    {
                        _solvedDescription += " from " + stat2;
                    }
                    _solvedDescription += ": ";
                    double totalOffset = Math.Round(80 * LevelOffset + ConstantOffset, 4);
                    bool addParanthesis = totalOffset != 0 && Variable != 0;
                    if (addParanthesis)
                    {
                        _solvedDescription += "(";
                    }
                    bool prefix = false;
                    if (Variable != 0)
                    {
                        _solvedDescription += Variable + " * Stat";
                        prefix = true;
                    }
                    if (totalOffset != 0)
                    {
                        _solvedDescription += (Math.Sign(totalOffset) < 0 ? " -" : " +")+ (prefix ? " " : "") + Math.Abs(totalOffset);
                    }
                    if (addParanthesis)
                    {
                        _solvedDescription += ")";
                    }
                }
                return _solvedDescription;
            }

        }
        public List<BuffFormula> FormulaList { get; } = new List<BuffFormula>();

        public BuffInfoEvent(CombatItem evtcItem) : base(evtcItem)
        {
            switch(evtcItem.IsStateChange)
            {
                case ParseEnum.StateChange.BuffFormula:
                    BuildFromBuffFormula(evtcItem);
                    break;
                case ParseEnum.StateChange.BuffInfo:
                    BuildFromBuffInfo(evtcItem);
                    break;
                default:
                    throw new InvalidOperationException("Invalid combat event in BuffDataEvent constructor");
            }
            BuffID = evtcItem.SkillID;
        }

        public void CompleteBuffDataEvent(CombatItem evtcItem)
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
            StackingType = evtcItem.Pad1;
            ProbablyResistance = evtcItem.Pad2 > 0;
        }

        public void AdjustUnknownFormulaAttributes(Dictionary<byte, ParseEnum.BuffAttribute> solved)
        {
            FormulaList.Sort((x, y) => (x.TraitSelf + x.TraitSrc).CompareTo(y.TraitSrc + y.TraitSelf));
            foreach (BuffFormula formula in FormulaList)
            {
                formula.AdjustUnknownFormulaAttributes(solved);
            }
        }

        private void BuildFromBuffFormula(CombatItem evtcItem)
        {
            FormulaList.Add(new BuffFormula(evtcItem));
        }

    }
}
