using System;
using System.Collections.Generic;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BuffDataEvent : AbstractMetaDataEvent
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

            public ParseEnum.BuffAttribute Attr1 { get; }

            public ParseEnum.BuffAttribute Attr2 { get; }

#if DEBUG
            public byte DebugAttr1 { get; }

            public byte DebugAttr2 { get; }
#endif

            public float Param1 { get; }

            public float Param2 { get; }

            public float Param3 { get; }

            public int TraitSrc { get; }

            public int TraitSelf { get; }

            public bool NPC { get; }

            public bool Player { get; }

            public bool Break { get; }

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
#if DEBUG
                DebugAttr1 = (byte)formulaFloats[1];
                DebugAttr2 = (byte)formulaFloats[2];
#endif
                Attr1 = ParseEnum.GetBuffAttribute((byte) formulaFloats[1]);
                Attr2 = ParseEnum.GetBuffAttribute((byte)formulaFloats[2]);
                Param1 = formulaFloats[3];
                Param2 = formulaFloats[4];
                Param3 = formulaFloats[5];
                TraitSrc = (int)formulaFloats[6];
                TraitSelf = (int)formulaFloats[7];
            }
        }
        public List<BuffFormula> FormulaList { get; } = new List<BuffFormula>();

        private string _description = null;

        public BuffDataEvent(CombatItem evtcItem) : base(evtcItem)
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
            byte[] pads = evtcItem.BreakPad();
            StackingType = pads[0];
            ProbablyResistance = pads[1] > 0;
        }

        private void BuildFromBuffFormula(CombatItem evtcItem)
        {
            var formula = new BuffFormula(evtcItem);
#if !DEBUG
            if (formula.Attr1 == ParseEnum.BuffAttribute.Unknown || formula.Attr2 == ParseEnum.BuffAttribute.Unknown)
            {
                return;
            }
#endif
            FormulaList.Add(new BuffFormula(evtcItem));
        }


        public string GetDescription()
        {
            if (_description == null)
            {
                _description = "";
            }
            return _description;
        }

    }
}
