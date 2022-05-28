using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Interfaces;
using GW2EIEvtcParser.ParsedData;
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
            private readonly ulong _minBuild;
            private readonly ulong _maxBuild;
            private readonly ArcDPSEnums.BuffAttribute _result;

            public BuffFormulaDescriptor(float constantOffset, float levelOffset, float variable, int traitSelf, int traitSrc, ArcDPSEnums.BuffAttribute result, ulong minBuild = GW2Builds.StartOfLife, ulong maxBuild = GW2Builds.EndOfLife)
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
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.PhysRec2, GW2Builds.EODBeta3), Protection },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.CondRec2, GW2Builds.EODBeta3), Resolution },
            // CriticalChance
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.CriticalChance), Fury },
            // Fishing Power      
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.FishingPower), PlateOfImperialPalaceSpecial },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.FishingPower), PlateOfCrispyFishPancakes },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.FishingPower), BowlOfJadeSeaBounty },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.FishingPower), BowlOfEchovaldHotpot },
            // Life Leech      
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.SiphonInc, GW2Builds.May2021Balance), KallasFervor },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.SiphonInc, GW2Builds.May2021Balance), ImprovedKallasFervor },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, ArcDPSEnums.BuffAttribute.SiphonInc, GW2Builds.May2021Balance), Fury },
            // ConditionDurationIncrease
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, ArcDPSEnums.BuffAttribute.ConditionDurationInc), Fury },
            // SkillRechargeSpeedIncrease
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.SkillRechargeSpeedIncrease), Alacrity },
            // HealingOutputFormula
            {new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, ArcDPSEnums.BuffAttribute.HealingOutputFormula), Regeneration },
            // EnduranceRegeneration
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.EnduranceRegeneration), Vigor },
            // MovementSpeed
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.MovementSpeed), Swiftness },
            // DamageFormulaSquaredLevel
            {  new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, ArcDPSEnums.BuffAttribute.DamageFormulaSquaredLevel, 0, GW2Builds.May2021Balance), Retaliation },
            // DamageFormula
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, ArcDPSEnums.BuffAttribute.DamageFormula), Bleeding },
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, ArcDPSEnums.BuffAttribute.DamageFormula), Burning },
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, ArcDPSEnums.BuffAttribute.DamageFormula), Poison },
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, 0, 0, 0, ArcDPSEnums.BuffAttribute.DamageFormula), Confusion },
            // SkillActivationDamageFormula
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, ArcDPSEnums.BuffAttribute.SkillActivationDamageFormula), Confusion },
            // MovementActivationDamageFormula
            { new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, AnyPositive, ArcDPSEnums.BuffAttribute.MovementActivationDamageFormula), Torment },
            // IncomingHealingEffectiveness
            { new BuffFormulaDescriptor(AnyNegative, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.HealingEffectivenessRec), Poison },
            // GlancingBlow
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.GlancingBlow), Weakness },
            // OutgoingHealingEffectivenessFlatInc
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.HealingEffectivenessFlatInc), SuperiorRuneOfTheMonk },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.HealingEffectivenessFlatInc), DeliciousRiceBall },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.HealingEffectivenessFlatInc), InvokingHarmony },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, ArcDPSEnums.BuffAttribute.HealingEffectivenessFlatInc), CelestialAvatar },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, ArcDPSEnums.BuffAttribute.HealingEffectivenessFlatInc), NaturalMender },
            // Damage to HP
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.StrikeDamageToHP), BloodReckoning },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.StrikeDamageToHP), LitanyOfWrath },
            // Condition to HP
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ConditionDamageToHP), BloodReckoning },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ConditionDamageToHP), LitanyOfWrath },
            // BoonDurationIncrease
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.BoonDurationInc), WovenWater },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.BoonDurationInc), PerfectWeave },
            // Experience from kills
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), RareVeggiePizza },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), PlateOfBeefRendang },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), SoulPastry },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), BowlOfFireMeatChili },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), SuperiorSharpeningStone },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), ToxicFocusingCrystal },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), BowlOfSweetAndSpicyButternutSquashSoup },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromKills), MasterMaintenanceOil },
            // Experience from all
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.ExperienceFromAll), RedLentilSaobosa },
            // HealingEffectivenessRec2
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.HealingEffectivenessRec2), EternalOasis },
            // MagicFind
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.MagicFind), GuildItemResearch },
            // Stacking Movement Speed
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.MovementSpeedStacking), RisingMomentum },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.MovementSpeedStacking2), FormUpAndAdvance },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.MovementSpeedStacking2), UnseenBurden },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, ArcDPSEnums.BuffAttribute.MovementSpeedStacking2), Hamstrung },
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
