using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;
using static GW2EIParser.Parser.ParsedData.CombatEvents.BuffDataEvent;

namespace GW2EIParser.EIData
{
    public static class BuffFormulaSolver
    {
        private const int AnyPositive = int.MinValue;
        private const int AnyNegative = int.MaxValue;
        private class BuffFormulaDescriptor
        {
            private readonly int _type;
            private readonly float _param1;
            private readonly float _param2;
            private readonly float _param3;
            private readonly int _traitSrc;
            private readonly int _traitSelf;
            private readonly ParseEnum.BuffAttribute _result;

            public BuffFormulaDescriptor(float param1, float param2, float param3, int type, int traitSelf, int traitSrc, ParseEnum.BuffAttribute result)
            {
                _param1 = param1;
                _param2 = param2;
                _param3 = param3;
                _type = type;
                _traitSrc = traitSrc;
                _traitSelf = traitSelf;
                _result = result;
            }

            public void Match(BuffFormula formula, Dictionary<byte, ParseEnum.BuffAttribute> toFill)
            {
                if (formula.Attr1 == ParseEnum.BuffAttribute.Unknown && !toFill.ContainsKey(formula.ByteAttr1))
                {
                    if (formula.Param1 == _param1 || (formula.Param1 > 0 && _param1 == AnyPositive) || (formula.Param1 < 0 && _param1 == AnyNegative))
                    {
                        if (formula.Param2 == _param2 || (formula.Param2 > 0 && _param2 == AnyPositive) || (formula.Param2 < 0 && _param2 == AnyNegative))
                        {
                            if (formula.Param3 == _param3 || (formula.Param3 > 0 && _param3 == AnyPositive) || (formula.Param3 < 0 && _param3 == AnyNegative))
                            {
                                if (formula.Type == _type || (formula.Type > 0 && _type == AnyPositive) || (formula.Type < 0 && _type == AnyNegative))
                                {
                                    if (formula.TraitSelf == _traitSelf || (formula.TraitSelf > 0 && _traitSelf == AnyPositive) || (formula.TraitSelf < 0 && _traitSelf == AnyNegative))
                                    {
                                        if (formula.TraitSrc == _traitSrc || (formula.TraitSrc > 0 && _traitSrc == AnyPositive) || (formula.TraitSrc < 0 && _traitSrc == AnyNegative))
                                        {
                                            toFill[formula.ByteAttr1] = _result;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        // VERY IMPORTANT: if using an id multiple time, make sure the stricter checking conditions are done first
        private static readonly Dictionary<BuffFormulaDescriptor, long> _recognizer = new Dictionary<BuffFormulaDescriptor, long> {
            // CriticalChance
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 4, 0, 0, ParseEnum.BuffAttribute.CriticalChance), 725 },
            // ConditionDurationIncrease
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 4, AnyPositive, 0, ParseEnum.BuffAttribute.ConditionDurationIncrease), 725 },
            // SkillCooldown
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 4, 0, 0, ParseEnum.BuffAttribute.SkillCooldown), 30328 },
            // HealingOutputFormula
            {new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 5, 0, 0, ParseEnum.BuffAttribute.HealingOutputFormula), 718 },
            // EnduranceRegeneration
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 4, 0, 0, ParseEnum.BuffAttribute.EnduranceRegeneration), 726 },
            // MovementSpeed
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 4, 0, 0, ParseEnum.BuffAttribute.MovementSpeed), 719 },
            // BuffPowerDamageFormula
            {  new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 12, 0, 0, ParseEnum.BuffAttribute.BuffPowerDamageFormula), 873 },
            // ConditionDamageFormula
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 1, 0, 0, ParseEnum.BuffAttribute.ConditionDamageFormula), 736 },
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 1, 0, 0, ParseEnum.BuffAttribute.ConditionDamageFormula), 737 },
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 1, 0, 0, ParseEnum.BuffAttribute.ConditionDamageFormula), 723 },
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, 0, 1, 0, 0, ParseEnum.BuffAttribute.ConditionDamageFormula), 861 },
            // ConditionSkillActivationFormula
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 1, 0, 0, ParseEnum.BuffAttribute.ConditionSkillActivationFormula), 861 },
            // ConditionMovementActivationFormula
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 1, 0, AnyPositive, ParseEnum.BuffAttribute.ConditionMovementActivationFormula), 19426 },
            // IncomingHealingEffectiveness
            { new BuffFormulaDescriptor(AnyNegative, 0, 0, 4, 0, 0, ParseEnum.BuffAttribute.IncomingHealingEffectiveness), 723 },
            // GlancingBlow
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 4, 0, 0, ParseEnum.BuffAttribute.GlancingBlow), 742 },
            // OutgoingHealingEffectivenessFlatInc
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 4, 0, 0, ParseEnum.BuffAttribute.OutgoingHealingEffectivenessFlatInc), 53285 },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 4, 0, 0, ParseEnum.BuffAttribute.OutgoingHealingEffectivenessFlatInc), 26529 },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 4, 0, 0, ParseEnum.BuffAttribute.OutgoingHealingEffectivenessFlatInc), 29025 },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 4, AnyPositive, 0, ParseEnum.BuffAttribute.OutgoingHealingEffectivenessFlatInc), 31508 },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 4, AnyPositive, 0, ParseEnum.BuffAttribute.OutgoingHealingEffectivenessFlatInc), 30449 },
        };

        public static void SolveBuffFormula(CombatData combatData, Dictionary<long, Buff> buffsByID)
        {
            var solved = new Dictionary<byte, ParseEnum.BuffAttribute>();
            foreach (KeyValuePair<BuffFormulaDescriptor, long> pair in _recognizer)
            {
                if (buffsByID.TryGetValue(pair.Value, out Buff buff))
                {
                    BuffDataEvent buffDataEvent = combatData.GetBuffDataEvent(buff.ID);
                    if (buffDataEvent != null)
                    {
                        foreach (BuffFormula formula in buffDataEvent.FormulaList)
                        {
                            pair.Key.Match(formula, solved);
                        }
                    }
                }
            }
#if DEBUG
            if (solved.Values.Distinct().Count() != solved.Values.Count)
            {
                throw new InvalidDataException("Bad data in solved buff formula");
            }
#endif
            foreach (long key in buffsByID.Keys)
            {
                BuffDataEvent buffDataEvent = combatData.GetBuffDataEvent(key);
                if (buffDataEvent != null)
                {
                    buffDataEvent.AdjustUnknownFormulaAttributes(solved);
                }
            }
        }
    }
}
