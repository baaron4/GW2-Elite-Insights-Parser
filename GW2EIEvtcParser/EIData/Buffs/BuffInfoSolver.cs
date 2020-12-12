using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal static class BuffInfoSolver
    {
        private const int AnyPositive = int.MinValue;
        private const int AnyNegative = int.MaxValue;
        private class BuffFormulaDescriptor
        {
            private readonly float _constantOffset;
            private readonly float _levelOffset;
            private readonly float _variable;
            private readonly int _traitSrc;
            private readonly int _traitSelf;
            private readonly ArcDPSEnums.BuffAttribute _result;

            public BuffFormulaDescriptor(float constantOffset, float levelOffset, float variable, int traitSelf, int traitSrc, ArcDPSEnums.BuffAttribute result)
            {
                _constantOffset = constantOffset;
                _levelOffset = levelOffset;
                _variable = variable;
                _traitSrc = traitSrc;
                _traitSelf = traitSelf;
                _result = result;
            }

            public void Match(BuffFormula formula, Dictionary<byte, ArcDPSEnums.BuffAttribute> toFill)
            {
                if (formula.Attr1 == ArcDPSEnums.BuffAttribute.Unknown && !toFill.ContainsKey(formula.ByteAttr1))
                {
                    if (formula.ConstantOffset == _constantOffset || (formula.ConstantOffset > 0 && _constantOffset == AnyPositive) || (formula.ConstantOffset < 0 && _constantOffset == AnyNegative))
                    {
                        if (formula.LevelOffset == _levelOffset || (formula.LevelOffset > 0 && _levelOffset == AnyPositive) || (formula.LevelOffset < 0 && _levelOffset == AnyNegative))
                        {
                            if (formula.Variable == _variable || (formula.Variable > 0 && _variable == AnyPositive) || (formula.Variable < 0 && _variable == AnyNegative))
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
        // VERY IMPORTANT: if using an id multiple time, make sure the stricter checking conditions are done first
        private static readonly Dictionary<BuffFormulaDescriptor, long> _recognizer = new Dictionary<BuffFormulaDescriptor, long> {
            // CriticalChance
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.CriticalChance), 725 },
            // ConditionDurationIncrease
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, ArcDPSEnums.BuffAttribute.ConditionDurationIncrease), 725 },
            // SkillRechargeSpeedIncrease
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.SkillRechargeSpeedIncrease), 30328 },
            // HealingOutputFormula
            {new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, ArcDPSEnums.BuffAttribute.HealingOutputFormula), 718 },
            // EnduranceRegeneration
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.EnduranceRegeneration), 726 },
            // MovementSpeed
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.MovementSpeed), 719 },
            // DamageFormulaSquaredLevel
            {  new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, ArcDPSEnums.BuffAttribute.DamageFormulaSquaredLevel), 873 },
            // DamageFormula
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, ArcDPSEnums.BuffAttribute.DamageFormula), 736 },
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, ArcDPSEnums.BuffAttribute.DamageFormula), 737 },
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, ArcDPSEnums.BuffAttribute.DamageFormula), 723 },
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, 0, 0, 0, ArcDPSEnums.BuffAttribute.DamageFormula), 861 },
            // SkillActivationDamageFormula
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, ArcDPSEnums.BuffAttribute.SkillActivationDamageFormula), 861 },
            // MovementActivationDamageFormula
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, AnyPositive, ArcDPSEnums.BuffAttribute.MovementActivationDamageFormula), 19426 },
            // IncomingHealingEffectiveness
            { new BuffFormulaDescriptor(AnyNegative, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.IncomingHealingEffectiveness), 723 },
            // GlancingBlow
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.GlancingBlow), 742 },
            // OutgoingHealingEffectivenessFlatInc
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.OutgoingHealingEffectivenessFlatInc), 53285 },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.OutgoingHealingEffectivenessFlatInc), 26529 },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.OutgoingHealingEffectivenessFlatInc), 29025 },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, ArcDPSEnums.BuffAttribute.OutgoingHealingEffectivenessFlatInc), 31508 },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, ArcDPSEnums.BuffAttribute.OutgoingHealingEffectivenessFlatInc), 30449 },
        };

        public static void AdjustBuffs(CombatData combatData, IReadOnlyDictionary<long, Buff> buffsByID, ParserController operation)
        {
            var solved = new Dictionary<byte, ArcDPSEnums.BuffAttribute>();
            foreach (KeyValuePair<BuffFormulaDescriptor, long> pair in _recognizer)
            {
                if (buffsByID.TryGetValue(pair.Value, out Buff buff))
                {
                    BuffInfoEvent buffInfoEvent = combatData.GetBuffInfoEvent(buff.ID);
                    if (buffInfoEvent != null)
                    {
                        foreach (BuffFormula formula in buffInfoEvent.Formulas)
                        {
                            pair.Key.Match(formula, solved);
                        }
                    }
                }
            }
            if (solved.Values.Distinct().Count() != solved.Values.Count)
            {
                operation.UpdateProgressWithCancellationCheck("Incoherent Data in Buff Info Solver, no formula attribute adjustement will be done");
                solved.Clear();
            }
            foreach (KeyValuePair<long, Buff> pair in buffsByID)
            {
                BuffInfoEvent buffInfoEvent = combatData.GetBuffInfoEvent(pair.Key);
                if (buffInfoEvent != null)
                {
                    pair.Value.VerifyBuffInfoEvent(buffInfoEvent, operation);
                    buffInfoEvent.AdjustBuffInfo(solved);
                }
            }
        }
    }
}
