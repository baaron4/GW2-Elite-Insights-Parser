using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using GW2EIUtils;

namespace GW2EIEvtcParser.EIData
{
    internal static class BuffInfoSolver
    {
        private const int _anyPositive = int.MinValue;
        private const int _anyNegative = int.MaxValue;
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
                    if (formula.ConstantOffset == _constantOffset || (formula.ConstantOffset > 0 && _constantOffset == _anyPositive) || (formula.ConstantOffset < 0 && _constantOffset == _anyNegative))
                    {
                        if (formula.LevelOffset == _levelOffset || (formula.LevelOffset > 0 && _levelOffset == _anyPositive) || (formula.LevelOffset < 0 && _levelOffset == _anyNegative))
                        {
                            if (formula.Variable == _variable || (formula.Variable > 0 && _variable == _anyPositive) || (formula.Variable < 0 && _variable == _anyNegative))
                            {
                                if (formula.TraitSelf == _traitSelf || (formula.TraitSelf > 0 && _traitSelf == _anyPositive) || (formula.TraitSelf < 0 && _traitSelf == _anyNegative))
                                {
                                    if (formula.TraitSrc == _traitSrc || (formula.TraitSrc > 0 && _traitSrc == _anyPositive) || (formula.TraitSrc < 0 && _traitSrc == _anyNegative))
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
            {new BuffFormulaDescriptor(_anyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.CriticalChance), 725 },
            // ConditionDurationIncrease
            {new BuffFormulaDescriptor(_anyPositive, 0, 0, _anyPositive, 0, ArcDPSEnums.BuffAttribute.ConditionDurationIncrease), 725 },
            // SkillCooldown
            { new BuffFormulaDescriptor(_anyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.SkillCooldownReduction), 30328 },
            // HealingOutputFormula
            {new BuffFormulaDescriptor(_anyPositive, _anyPositive, _anyPositive, 0, 0, ArcDPSEnums.BuffAttribute.HealingOutputFormula), 718 },
            // EnduranceRegeneration
            { new BuffFormulaDescriptor(_anyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.EnduranceRegeneration), 726 },
            // MovementSpeed
            { new BuffFormulaDescriptor(_anyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.MovementSpeed), 719 },
            // BuffPowerDamageFormula
            {  new BuffFormulaDescriptor(_anyPositive, _anyPositive, _anyPositive, 0, 0, ArcDPSEnums.BuffAttribute.BuffPowerDamageFormula), 873 },
            // ConditionDamageFormula
            { new BuffFormulaDescriptor(_anyPositive, _anyPositive, _anyPositive, 0, 0, ArcDPSEnums.BuffAttribute.ConditionDamageFormula), 736 },
            { new BuffFormulaDescriptor(_anyPositive, _anyPositive, _anyPositive, 0, 0, ArcDPSEnums.BuffAttribute.ConditionDamageFormula), 737 },
            { new BuffFormulaDescriptor(_anyPositive, _anyPositive, _anyPositive, 0, 0, ArcDPSEnums.BuffAttribute.ConditionDamageFormula), 723 },
            { new BuffFormulaDescriptor(_anyPositive, _anyPositive, 0, 0, 0, ArcDPSEnums.BuffAttribute.ConditionDamageFormula), 861 },
            // ConditionSkillActivationFormula
            { new BuffFormulaDescriptor(_anyPositive, _anyPositive, _anyPositive, 0, 0, ArcDPSEnums.BuffAttribute.ConditionSkillActivationFormula), 861 },
            // ConditionMovementActivationFormula
            { new BuffFormulaDescriptor(_anyPositive, _anyPositive, _anyPositive, 0, _anyPositive, ArcDPSEnums.BuffAttribute.ConditionMovementActivationFormula), 19426 },
            // IncomingHealingEffectiveness
            { new BuffFormulaDescriptor(_anyNegative, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.IncomingHealingEffectiveness), 723 },
            // GlancingBlow
            { new BuffFormulaDescriptor(_anyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.GlancingBlow), 742 },
            // OutgoingHealingEffectivenessFlatInc
            { new BuffFormulaDescriptor(_anyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.OutgoingHealingEffectivenessFlatInc), 53285 },
            { new BuffFormulaDescriptor(_anyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.OutgoingHealingEffectivenessFlatInc), 26529 },
            { new BuffFormulaDescriptor(_anyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.OutgoingHealingEffectivenessFlatInc), 29025 },
            { new BuffFormulaDescriptor(_anyPositive, 0, 0, _anyPositive, 0, ArcDPSEnums.BuffAttribute.OutgoingHealingEffectivenessFlatInc), 31508 },
            { new BuffFormulaDescriptor(_anyPositive, 0, 0, _anyPositive, 0, ArcDPSEnums.BuffAttribute.OutgoingHealingEffectivenessFlatInc), 30449 },
        };

        public static void AdjustBuffs(CombatData combatData, Dictionary<long, Buff> buffsByID, OperationTracer operation)
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
                    pair.Value.AttachBuffInfoEvent(buffInfoEvent, operation);
                    buffInfoEvent.AdjustBuffInfo(solved);
                }
            }
        }
    }
}
