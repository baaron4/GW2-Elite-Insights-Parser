using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Interfaces;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

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
            private readonly long _buffSrc;
            private readonly long _buffSelf;
            private readonly ulong _minBuild;
            private readonly ulong _maxBuild;
            private readonly ArcDPSEnums.BuffAttribute _result;

            public BuffFormulaDescriptor(float constantOffset, float levelOffset, float variable, int traitSelf, int traitSrc, long buffSelf, int buffSrc, ArcDPSEnums.BuffAttribute result, ulong minBuild = GW2Builds.StartOfLife, ulong maxBuild = GW2Builds.EndOfLife)
            {
                _constantOffset = constantOffset;
                _levelOffset = levelOffset;
                _variable = variable;
                _traitSrc = traitSrc;
                _traitSelf = traitSelf;
                _buffSrc = buffSrc;
                _buffSelf = buffSelf;
                _result = result;
                _minBuild = minBuild;
                _maxBuild = maxBuild;
            }

            public bool Available(CombatData combatData)
            {
                ulong gw2Build = combatData.GetBuildEvent().Build;
                return gw2Build < _maxBuild && gw2Build >= _minBuild;
            }

            public bool Match(BuffFormula formula, Dictionary<byte, ArcDPSEnums.BuffAttribute> toFill)
            {
                // No need to match anything if we already associated the result
                if (toFill.ContainsValue(_result))
                {
                    return true;
                }
                if (formula.Attr1 == BuffAttribute.Unknown && !toFill.ContainsKey(formula.ByteAttr1))
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
                                        if (formula.BuffSrc == _buffSrc || (formula.BuffSrc > 0 && _buffSrc == AnyPositive) || (formula.BuffSrc < 0 && _buffSrc == AnyNegative))
                                        {
                                            if (formula.BuffSelf == _buffSelf || (formula.BuffSelf > 0 && _buffSelf == AnyPositive) || (formula.BuffSelf < 0 && _buffSelf == AnyNegative))
                                            {
                                                toFill[formula.ByteAttr1] = _result;
                                                return true;
                                            }
                                        }
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
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.PhysRec2, GW2Builds.EODBeta3), Protection },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.CondRec2, GW2Builds.EODBeta3), Resolution },
            // CriticalChance
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.CriticalChance), Fury },
            // Fishing Power      
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.FishingPower), PlateOfImperialPalaceSpecial },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.FishingPower), PlateOfCrispyFishPancakes },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.FishingPower), BowlOfJadeSeaBounty },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.FishingPower), BowlOfEchovaldHotpot },
            // Life Leech      
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.SiphonInc, GW2Builds.May2021Balance), KallasFervor },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.SiphonInc, GW2Builds.May2021Balance), ImprovedKallasFervor },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, 0, 0, BuffAttribute.SiphonInc, GW2Builds.May2021Balance), Fury },
            // ConditionDurationIncrease
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, 0, 0, BuffAttribute.ConditionDurationInc), Fury },
            // SkillRechargeSpeedIncrease
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.SkillRechargeSpeedIncrease), Alacrity },
            // HealingOutputFormula
            {new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, 0, 0, BuffAttribute.HealingOutputFormula), Regeneration },
            // EnduranceRegeneration
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.EnduranceRegeneration), Vigor },
            // MovementSpeed
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.MovementSpeed), Swiftness },
            // DamageFormulaSquaredLevel
            {  new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, 0, 0, BuffAttribute.DamageFormulaSquaredLevel, 0, GW2Builds.May2021Balance), Retaliation },
            // DamageFormula
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, 0, 0, BuffAttribute.DamageFormula), Bleeding },
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, 0, 0, BuffAttribute.DamageFormula), Burning },
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, 0, 0, BuffAttribute.DamageFormula), Poison },
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, 0, 0, 0, 0, 0, BuffAttribute.DamageFormula), Confusion },
            // SkillActivationDamageFormula
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, 0, 0, BuffAttribute.SkillActivationDamageFormula), Confusion },
            // MovementActivationDamageFormula
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, AnyPositive, 0, 0, BuffAttribute.MovementActivationDamageFormula), Torment },
            // IncomingHealingEffectiveness
            { new BuffFormulaDescriptor(AnyNegative, 0, 0, 0, 0, 0, 0, BuffAttribute.HealingEffectivenessRec), Poison },
            // GlancingBlow
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.GlancingBlow), Weakness },
            // OutgoingHealingEffectivenessFlatInc
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.HealingEffectivenessFlatInc), SuperiorRuneOfTheMonk },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.HealingEffectivenessFlatInc), DeliciousRiceBall },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.HealingEffectivenessFlatInc), InvokingHarmony },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, 0, 0, BuffAttribute.HealingEffectivenessFlatInc), CelestialAvatar },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, 0, 0, BuffAttribute.HealingEffectivenessFlatInc), NaturalMender },
            // Damage to HP
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.StrikeDamageToHP), BloodReckoning },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.StrikeDamageToHP), LitanyOfWrath },
            // Condition to HP
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.ConditionDamageToHP), BloodReckoning },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.ConditionDamageToHP), LitanyOfWrath },
            // BoonDurationIncrease
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.BoonDurationInc), WovenWater },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.BoonDurationInc), PerfectWeave },
            // Experience from kills
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.ExperienceFromKills), RareVeggiePizza },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.ExperienceFromKills), PlateOfBeefRendang },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.ExperienceFromKills), SoulPastry },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.ExperienceFromKills), BowlOfFireMeatChili },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.ExperienceFromKills), SuperiorSharpeningStone },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.ExperienceFromKills), ToxicFocusingCrystal },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.ExperienceFromKills), BowlOfSweetAndSpicyButternutSquashSoup },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.ExperienceFromKills), MasterMaintenanceOil },
            // Experience from all
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.ExperienceFromAll), RedLentilSaobosa },
            // HealingEffectivenessRec2
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.HealingEffectivenessRec2), EternalOasis },
            // MagicFind
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.MagicFind), GuildItemResearch },
            // Stacking Movement Speed
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.MovementSpeedStacking), RisingMomentum },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.MovementSpeedStacking2), FormUpAndAdvance },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.MovementSpeedStacking2), UnseenBurden },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.MovementSpeedStacking2), Hamstrung },
            // Maximum HP
            {new BuffFormulaDescriptor(AnyNegative, 0, 0, 0, 0, MistlockInstabilityBoonOverload, 0, BuffAttribute.MaximumHP), Might },
            // Vitality Percent
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.VitalityPercent), ReinforcedArmor },
            // Defense Percent
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.DefensePercent), ReinforcedArmor },
        };

        public static void AdjustBuffs(CombatData combatData, IReadOnlyDictionary<long, Buff> buffsByID, ParserController operation)
        {
            var solved = new Dictionary<byte, ArcDPSEnums.BuffAttribute>();
            foreach (KeyValuePair<BuffFormulaDescriptor, long> pair in _recognizer)
            {
                if (!pair.Key.Available(combatData))
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
                            if (pair.Key.Match(formula, solved))
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
            }
            else if (solved.Any())
            {
                operation.UpdateProgressWithCancellationCheck("Deduced " + solved.Count + " unknown buff formulas");
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
