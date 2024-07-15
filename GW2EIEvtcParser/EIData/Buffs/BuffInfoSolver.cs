using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Interfaces;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
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
            private ulong _minBuild = GW2Builds.StartOfLife;
            private ulong _maxBuild = GW2Builds.EndOfLife;
            private int _minEvtcBuild = ArcDPSBuilds.StartOfLife;
            private int _maxEvtcBuild = ArcDPSBuilds.EndOfLife;
            private readonly ArcDPSEnums.BuffAttribute _result;

            public BuffFormulaDescriptor(float constantOffset, float levelOffset, float variable, int traitSelf, int traitSrc, long buffSelf, int buffSrc, ArcDPSEnums.BuffAttribute result)
            {
                _constantOffset = constantOffset;
                _levelOffset = levelOffset;
                _variable = variable;
                _traitSrc = traitSrc;
                _traitSelf = traitSelf;
                _buffSrc = buffSrc;
                _buffSelf = buffSelf;
                _result = result;
            }

            internal BuffFormulaDescriptor WithBuilds(ulong minBuild, ulong maxBuild = GW2Builds.EndOfLife)
            {
                _minBuild = minBuild;
                _maxBuild = maxBuild;
                return this;
            }

            internal BuffFormulaDescriptor WithEvtcBuilds(int minBuild, int maxBuild = ArcDPSBuilds.EndOfLife)
            {
                _minEvtcBuild = minBuild;
                _maxEvtcBuild = maxBuild;
                return this;
            }

            public bool Available(CombatData combatData)
            {
                ulong gw2Build = combatData.GetGW2BuildEvent().Build;
                if (gw2Build < _maxBuild && gw2Build >= _minBuild)
                {
                    int evtcBuild = combatData.GetEvtcVersionEvent().Build;
                    if (evtcBuild < _maxEvtcBuild && evtcBuild >= _minEvtcBuild)
                    {
                        return true;
                    }
                }
                return false;
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
            
            //
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.AttackSpeed).WithEvtcBuilds(ArcDPSBuilds.EICanDoManualBuffAttributes), Quickness },
            //
            {new BuffFormulaDescriptor(AnyPositive, AnyPositive, 0, 0, 0, 0, 0, BuffAttribute.Power).WithEvtcBuilds(ArcDPSBuilds.EICanDoManualBuffAttributes), Might },
            //
            {new BuffFormulaDescriptor(AnyPositive, AnyPositive, 0, 0, 0, 0, 0, BuffAttribute.Condition).WithEvtcBuilds(ArcDPSBuilds.EICanDoManualBuffAttributes), Might },
            //
            {new BuffFormulaDescriptor(AnyPositive, AnyPositive, 0, 0, 0, 0, 0, BuffAttribute.Precision).WithEvtcBuilds(ArcDPSBuilds.EICanDoManualBuffAttributes), SignetOfFuryBuff },
            {new BuffFormulaDescriptor(AnyPositive, AnyPositive, 0, 0, 0, 0, 0, BuffAttribute.Precision).WithEvtcBuilds(ArcDPSBuilds.EICanDoManualBuffAttributes), SignetOfAgilityBuff },
            //
            {new BuffFormulaDescriptor(AnyPositive, AnyPositive, 0, 0, 0, 0, 0, BuffAttribute.Toughness).WithEvtcBuilds(ArcDPSBuilds.EICanDoManualBuffAttributes), DolyakSignetBuff },
            //
            {new BuffFormulaDescriptor(AnyPositive, AnyPositive, 0, 0, 0, 0, 0, BuffAttribute.Vitality).WithEvtcBuilds(ArcDPSBuilds.EICanDoManualBuffAttributes), ConjureEarthShield },
            //
            {new BuffFormulaDescriptor(AnyPositive, AnyPositive, 0, 0, 0, 0, 0, BuffAttribute.Healing).WithEvtcBuilds(ArcDPSBuilds.EICanDoManualBuffAttributes), ConjureFrostBow },
            //
            {new BuffFormulaDescriptor(AnyPositive, AnyPositive, 0, 0, 0, 0, 0, BuffAttribute.Concentration).WithEvtcBuilds(ArcDPSBuilds.EICanDoManualBuffAttributes), SignetOfMercyBuff },
            //
            {new BuffFormulaDescriptor(AnyPositive, AnyPositive, 0, 0, 0, 0, 0, BuffAttribute.Expertise).WithEvtcBuilds(ArcDPSBuilds.EICanDoManualBuffAttributes), SignetOfMidnightBuff },
            //
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.PhysIncomingAdditive).WithEvtcBuilds(ArcDPSBuilds.EICanDoManualBuffAttributes), Vulnerability },
            //
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.CondIncomingAdditive).WithEvtcBuilds(ArcDPSBuilds.EICanDoManualBuffAttributes), Vulnerability },
            //
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.PhysOutgoing).WithEvtcBuilds(ArcDPSBuilds.EICanDoManualBuffAttributes), KallasFervor },
            //
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.CondOutgoing).WithEvtcBuilds(ArcDPSBuilds.EICanDoManualBuffAttributes), KallasFervor },
            // 
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.PhysIncomingMultiplicative).WithBuilds(GW2Builds.EODBeta3), Protection },
            //
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.CondIncomingMultiplicative).WithBuilds(GW2Builds.EODBeta3), Resolution },
            // CriticalChance
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.CriticalChance), Fury },
            // Fishing Power      
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.FishingPower), PlateOfImperialPalaceSpecial },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.FishingPower), PlateOfCrispyFishPancakes },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.FishingPower), BowlOfJadeSeaBounty },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.FishingPower), BowlOfEchovaldHotpot },
            // Life Leech      
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.SiphonOutgoing).WithBuilds(GW2Builds.May2021Balance), KallasFervor },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.SiphonOutgoing).WithBuilds(GW2Builds.May2021Balance), ImprovedKallasFervor },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, 0, 0, BuffAttribute.SiphonOutgoing).WithBuilds(GW2Builds.May2021Balance), Fury },
            // ConditionDurationIncrease
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.ConditionDurationOutgoing).WithEvtcBuilds(ArcDPSBuilds.EICanDoManualBuffAttributes), ConjureFrostBow },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, 0, 0, BuffAttribute.ConditionDurationOutgoing), Fury },
            // SkillRechargeSpeedIncrease
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.SkillRechargeSpeedIncrease), Alacrity },
            // HealingOutputFormula
            {new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, 0, 0, BuffAttribute.HealingOutputFormula), Regeneration },
            // EnduranceRegeneration
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.EnduranceRegeneration), Vigor },
            // MovementSpeed
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.MovementSpeed), Swiftness },
            // DamageFormulaSquaredLevel
            {  new BuffFormulaDescriptor(AnyPositive, AnyPositive, AnyPositive, 0, 0, 0, 0, BuffAttribute.DamageFormulaSquaredLevel).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance), Retaliation },
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
            { new BuffFormulaDescriptor(AnyNegative, 0, 0, 0, 0, 0, 0, BuffAttribute.HealingEffectivenessIncomingNonStacking), Poison },
            // GlancingBlow
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.GlancingBlow), Weakness },
            // OutgoingHealingEffectivenessFlatInc
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.HealingEffectivenessOutgoingAdditive), SuperiorRuneOfTheMonk },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.HealingEffectivenessOutgoingAdditive), RelicOfTheMonk },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.HealingEffectivenessOutgoingAdditive), DeliciousRiceBall },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.HealingEffectivenessOutgoingAdditive), InvokingHarmony },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, 0, 0, BuffAttribute.HealingEffectivenessOutgoingAdditive), CelestialAvatar },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, AnyPositive, 0, 0, 0, BuffAttribute.HealingEffectivenessOutgoingAdditive), NaturalMender },
            // HealingEffectivenessIncomingMultiplicative
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.HealingEffectivenessIncomingMultiplicative), Infirmity },
            // SiphonIncomingAdditive
            { new BuffFormulaDescriptor(AnyNegative, 0, 0, 0, 0, 0, 0, BuffAttribute.SiphonIncomingAdditive2), Infirmity },
            // Damage to HP
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.StrikeDamageToHP), BloodReckoning },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.StrikeDamageToHP), LitanyOfWrath },
            // Condition to HP
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.ConditionDamageToHP), BloodReckoning },
            { new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.ConditionDamageToHP), LitanyOfWrath },
            // BoonDurationIncrease
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.BoonDurationOutgoing), WovenWater },
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.BoonDurationOutgoing), PerfectWeave },
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
            {new BuffFormulaDescriptor(AnyPositive, 0, 0, 0, 0, 0, 0, BuffAttribute.HealingEffectivenessIncomingAdditive), EternalOasis },
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
                operation.UpdateProgressWithCancellationCheck("Parsing: Incoherent Data in Buff Info Solver, no formula attribute adjustement will be done");
                solved.Clear();
            }
            else if (solved.Count != 0)
            {
                operation.UpdateProgressWithCancellationCheck("Parsing: Deduced " + solved.Count + " unknown buff formulas");
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
