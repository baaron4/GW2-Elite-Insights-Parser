using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;
using static GW2EIParser.Parser.ParsedData.CombatEvents.BuffInfoEvent;

namespace GW2EIParser.EIData
{
    public static class BuffInfoSolver
    {
        private const int AnyPositive = int.MinValue;
        private const int AnyNegative = int.MaxValue;
        private class BuffFormulaDescriptor
        {
            private readonly int _type;
            private readonly float _constantOffset;
            private readonly float _levelOffset;
            private readonly float _variable;
            private readonly int _traitSrc;
            private readonly int _traitSelf;
            private readonly ParseEnum.BuffAttribute _result;

            public BuffFormulaDescriptor(float constantOffset, float levelOffset, float variable, int type, int traitSelf, int traitSrc, ParseEnum.BuffAttribute result)
            {
                _constantOffset = constantOffset;
                _levelOffset = levelOffset;
                _variable = variable;
                _type = type;
                _traitSrc = traitSrc;
                _traitSelf = traitSelf;
                _result = result;
            }

            public void Match(BuffFormula formula, Dictionary<byte, ParseEnum.BuffAttribute> toFill)
            {
                if (formula.Attr1 == ParseEnum.BuffAttribute.Unknown && !toFill.ContainsKey(formula.ByteAttr1))
                {
                    if (formula.ConstantOffset == _constantOffset || (formula.ConstantOffset > 0 && _constantOffset == AnyPositive) || (formula.ConstantOffset < 0 && _constantOffset == AnyNegative))
                    {
                        if (formula.LevelOffset == _levelOffset || (formula.LevelOffset > 0 && _levelOffset == AnyPositive) || (formula.LevelOffset < 0 && _levelOffset == AnyNegative))
                        {
                            if (formula.Variable == _variable || (formula.Variable > 0 && _variable == AnyPositive) || (formula.Variable < 0 && _variable == AnyNegative))
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
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 4, 0, 0, ParseEnum.BuffAttribute.SkillCooldownReduction), 30328 },
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

        public static void AdjustBuffs(CombatData combatData, Dictionary<long, Buff> buffsByID)
        {
            var solved = new Dictionary<byte, ParseEnum.BuffAttribute>();
            foreach (KeyValuePair<BuffFormulaDescriptor, long> pair in _recognizer)
            {
                if (buffsByID.TryGetValue(pair.Value, out Buff buff))
                {
                    BuffInfoEvent buffInfoEvent = combatData.GetBuffInfoEvent(buff.ID);
                    if (buffInfoEvent != null)
                    {
                        foreach (BuffFormula formula in buffInfoEvent.FormulaList)
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
            foreach (KeyValuePair<long, Buff> pair in buffsByID)
            {
                BuffInfoEvent buffInfoEvent = combatData.GetBuffInfoEvent(pair.Key);
                if (buffInfoEvent != null)
                {
                    pair.Value.AdjustBuff(buffInfoEvent);
                    buffInfoEvent.AdjustBuffInfo(solved);
                }
            }
        }
    }
}
