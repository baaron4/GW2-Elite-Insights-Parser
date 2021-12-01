using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Interfaces;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal static class BuffInfoSolver
    {
        private const int AnyPositive = int.MinValue;
        private const int AnyNegative = int.MaxValue;
        private class BuffFormulaDescriptor : IVersionable
        {
            private readonly float _constantOffset;
            private readonly float _levelOffset;
            private readonly float _variable;
            private readonly int _traitSrc;
            private readonly int _traitSelf;
            private readonly ulong _minBuild;
            private readonly ulong _maxBuild;
            private readonly ArcDPSEnums.BuffAttribute _result;

            public BuffFormulaDescriptor(float constantOffset, float levelOffset, float variable, int traitSelf, int traitSrc, ArcDPSEnums.BuffAttribute result, ulong minBuild = 0, ulong maxBuild = ulong.MaxValue)
            {
                _constantOffset = constantOffset;
                _levelOffset = levelOffset;
                _variable = variable;
                _traitSrc = traitSrc;
                _traitSelf = traitSelf;
                _result = result;
                _minBuild = minBuild;
                _maxBuild = maxBuild;
            }

            public bool Available(ulong gw2Build)
            {
                return gw2Build < _maxBuild && gw2Build >= _minBuild;
            }

            public bool Match(BuffFormula formula, Dictionary<byte, ArcDPSEnums.BuffAttribute> toFill)
            {
                // No need to match anything if we already associated the result
                if (toFill.ContainsValue(_result))
                {
                    return true;
                }
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
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            }
        }
        // VERY IMPORTANT: if using an id multiple time, make sure the stricter checking conditions are done first
        private static readonly Dictionary<BuffFormulaDescriptor, long> _recognizer = new Dictionary<BuffFormulaDescriptor, long> {
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.PhysRec2, 121168), 717 },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.CondRec2, 121168), 873 },
            // CriticalChance
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.CriticalChance), 725 },
            // Life Leech      
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.SiphonInc, 115190), 42883 },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.SiphonInc, 115190), 45614 },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, ArcDPSEnums.BuffAttribute.SiphonInc, 115190), 725 },
            // ConditionDurationIncrease
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, ArcDPSEnums.BuffAttribute.ConditionDurationInc), 725 },
            // SkillRechargeSpeedIncrease
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.SkillRechargeSpeedIncrease), 30328 },
            // HealingOutputFormula
            {new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, ArcDPSEnums.BuffAttribute.HealingOutputFormula), 718 },
            // EnduranceRegeneration
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.EnduranceRegeneration), 726 },
            // MovementSpeed
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.MovementSpeed), 719 },
            // DamageFormulaSquaredLevel
            {  new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, ArcDPSEnums.BuffAttribute.DamageFormulaSquaredLevel, 0, 115190), 873 },
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
            { new BuffFormulaDescriptor(AnyNegative, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.HealingEffectivenessRec), 723 },
            // GlancingBlow
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.GlancingBlow), 742 },
            // OutgoingHealingEffectivenessFlatInc
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.HealingEffectivenessFlatInc), 53285 },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.HealingEffectivenessFlatInc), 26529 },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.HealingEffectivenessFlatInc), 29025 },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, ArcDPSEnums.BuffAttribute.HealingEffectivenessFlatInc), 31508 },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, ArcDPSEnums.BuffAttribute.HealingEffectivenessFlatInc), 30449 },
            // Damage to HP
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.StrikeDamageToHP), 29466 },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.StrikeDamageToHP), 21665 },
            // Condition to HP
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ConditionDamageToHP), 29466 },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ConditionDamageToHP), 21665 },
            // BoonDurationIncrease
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.BoonDurationInc), 43499 },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.BoonDurationInc), 45267 },
            // Experience from kills
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), 10009 },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), 49686 },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), 53222 },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), 10119 },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), 10119 },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), 9963 },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), 21828 },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), 17825 },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), 10119 },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), 9968 },
            // Experience from all
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromAll), 46273 },
            // HealingEffectivenessRec2
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.HealingEffectivenessRec2), 44871 },
            // MagicFind
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.MagicFind), 33833 },
            // Stacking Movement Speed
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.MovementSpeedStacking), 51683 },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.MovementSpeedStacking), 37871 },
        };

        public static void AdjustBuffs(CombatData combatData, IReadOnlyDictionary<long, Buff> buffsByID, ParserController operation, ulong gw2Build)
        {
            var solved = new Dictionary<byte, ArcDPSEnums.BuffAttribute>();
            foreach (KeyValuePair<BuffFormulaDescriptor, long> pair in _recognizer)
            {
                if (!pair.Key.Available(gw2Build))
                {
                    continue;
                }
                if (buffsByID.TryGetValue(pair.Value, out Buff buff))
                {
                    BuffInfoEvent buffInfoEvent = combatData.GetBuffInfoEvent(buff.ID);
                    if (buffInfoEvent != null)
                    {
                        foreach (BuffFormula formula in buffInfoEvent.Formulas)
                        {
                            if(pair.Key.Match(formula, solved))
                            {
                                break;
                            }
                        }
                    }
                }
            }
            if (solved.Values.Distinct().Count() != solved.Values.Count)
            {
                operation.UpdateProgressWithCancellationCheck("Incoherent Data in Buff Info Solver, no formula attribute adjustement will be done");
                solved.Clear();
            } else if (solved.Any())
            {
                operation.UpdateProgressWithCancellationCheck("Deduced "+ solved.Count + " unknown buff formulas");
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
