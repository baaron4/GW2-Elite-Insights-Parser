using System;
using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Logic;
using static GW2EIParser.Parser.ParseEnum.BuffAttribute;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BuffFormula
    {
        private static string GetAttributeString(ParseEnum.BuffAttribute attribute)
        {
            switch (attribute)
            {
                case Power:
                    return "Power";
                case Precision:
                    return "Precision";
                case Toughness:
                    return "Toughness";
                case Vitality:
                    return "Vitality";
                case Ferocity:
                    return "Ferocity";
                case Healing:
                    return "Healing Power";
                case Condition:
                    return "Condition Damage";
                case Concentration:
                    return "Concentration";
                case Expertise:
                    return "Expertise";
                case Armor:
                    return "Armor";
                case Agony:
                    return "Agony";
                case PhysInc:
                    return "Outgoing Physical Damage";
                case CondInc:
                    return "Outgoing Condition Damage";
                case CondRec:
                    return "Incoming Condition Damage";
                case PhysRec:
                    return "Incoming Physical Damage";
                case AttackSpeed:
                    return "Attack Speed";
                case ConditionDurationIncrease:
                    return "Condition Duration Increase";
                case BuffPowerDamageFormula:
                case ConditionDamageFormula:
                    return "Damage Formula";
                case GlancingBlow:
                    return "Glancing Blow";
                case CriticalChance:
                    return "Critical Chance";
                case PowerDamageToHP:
                    return "Physical Damage to Health";
                case ConditionDamageToHP:
                    return "Condition Damage to Health";
                case ConditionSkillActivationFormula:
                    return "Damage Formula on Skill Activation";
                case ConditionMovementActivationFormula:
                    return "Damage Formula on Movement";
                case EnduranceRegeneration:
                    return "Endurance Regeneration";
                case IncomingHealingEffectiveness:
                    return "Incoming Healing Effectiveness";
                case OutgoingHealingEffectivenessConvInc:
                case OutgoingHealingEffectivenessFlatInc:
                    return "Outgoing Healing Effectiveness";
                case HealingOutputFormula:
                    return "Healing Formula";
                case ExperienceFromKills:
                    return "Experience From Kills";
                case ExperienceFromAll:
                    return "Experience From All";
                case GoldFind:
                    return "Gold Find";
                case MovementSpeed:
                    return "Movement Speed";
                case KarmaBonus:
                    return "Karma Bonus";
                case SkillCooldownReduction:
                    return "Skill Cooldown Reduction";
                case MagicFind:
                    return "Magic Find";
                case WXP:
                    return "WXP";
                case Unknown:
                    return "Unknown";
                default:
                    return "";
            }
        }

        private static string GetVariableStat(ParseEnum.BuffAttribute attribute)
        {
            switch (attribute)
            {
                case BuffPowerDamageFormula:
                    return "Power";
                case ConditionDamageFormula:
                case ConditionSkillActivationFormula:
                case ConditionMovementActivationFormula:
                    return "Condition Damage";
                case HealingOutputFormula:
                    return "Healing Power";
                case Unknown:
                    return "Unknown";
                default:
                    return "";
            }
        }

        private static string GetPercent(ParseEnum.BuffAttribute attribute1, ParseEnum.BuffAttribute attribute2)
        {
            if (attribute2 != Unknown && attribute2 != None)
            {
                return "%";
            }
            switch (attribute1)
            {
                case PhysInc:
                case CondInc:
                case CondRec:
                case PhysRec:
                case AttackSpeed:
                case ConditionDurationIncrease:
                case GlancingBlow:
                case CriticalChance:
                case PowerDamageToHP:
                case ConditionDamageToHP:
                case EnduranceRegeneration:
                case IncomingHealingEffectiveness:
                case OutgoingHealingEffectivenessConvInc:
                case OutgoingHealingEffectivenessFlatInc:
                case ExperienceFromKills:
                case ExperienceFromAll:
                case GoldFind:
                case MovementSpeed:
                case KarmaBonus:
                case SkillCooldownReduction:
                case MagicFind:
                case WXP:
                    return "%";
                case ConditionMovementActivationFormula:
                    return " adds";
                case ConditionSkillActivationFormula:
                    return " replaces";
                case Unknown:
                    return "Unknown";
                default:
                    return "";
            }
        }
        // Effect type
        public int Type { get; }
        // Effect attributes
        public byte ByteAttr1 { get; }
        public ParseEnum.BuffAttribute Attr1 { get; private set; }
        public byte ByteAttr2 { get; }
        public ParseEnum.BuffAttribute Attr2 { get; private set; }
        // Effect parameters
        public float ConstantOffset { get; }
        public float LevelOffset { get; }
        public float Variable { get; }
        // Effect Condition
        public int TraitSrc { get; }
        public int TraitSelf { get; }
        // Meta data
        private bool _npc { get; }
        private bool _player { get; }
        private bool _break { get; }
        // Extra number
        private byte _extraNumberState { get; }
        private uint _extraNumber { get; }
        private bool _isExtraNumberBuffID => _extraNumberState == 2;
        private bool _isExtraNumberNone => _extraNumberState == 0;
        private bool _isExtraNumberSomething => _extraNumberState == 1;

        private string _solvedDescription = null;

        private readonly BuffInfoEvent _buffInfoEvent;

        private int _level => (_buffInfoEvent.Category == ParseEnum.BuffCategory.Food || _buffInfoEvent.Category == ParseEnum.BuffCategory.Enhancement) ? 0 : (Type == 12 ? 6400 : 80);

        public BuffFormula(CombatItem evtcItem, BuffInfoEvent buffInfoEvent)
        {
            _buffInfoEvent = buffInfoEvent;
            _npc = evtcItem.IsFlanking == 0;
            _player = evtcItem.IsShields == 0;
            _break = evtcItem.IsOffcycle > 0;
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
            _extraNumber = evtcItem.OverstackValue;
            _extraNumberState = evtcItem.Pad1;
        }

        public void AdjustUnknownFormulaAttributes(Dictionary<byte, ParseEnum.BuffAttribute> solved)
        {
            if (Attr1 == Unknown && solved.TryGetValue(ByteAttr1, out ParseEnum.BuffAttribute solvedAttr))
            {
                Attr1 = solvedAttr;
            }
            if (Attr2 == Unknown && solved.TryGetValue(ByteAttr2, out solvedAttr))
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
            if (authorizeUnknowns || (Attr1 != Unknown && Attr2 != Unknown))
            {
#if DEBUG
                _solvedDescription = Type + " ";
#endif
                if (Attr1 == None)
                {
                    return _solvedDescription;
                }
                var stat1 = GetAttributeString(Attr1);
                if (Attr1 == Unknown)
                {
                    stat1 += " " + ByteAttr1;
                }
                if (_isExtraNumberBuffID)
                {
                    if (buffsByIds.TryGetValue(_extraNumber, out Buff buff))
                    {
                        stat1 += " (" + buff.Name + ")";
                    }
                }
                var stat2 = GetAttributeString(Attr2);
                if (Attr2 == Unknown)
                {
                    stat2 += " " + ByteAttr2;
                }
                _solvedDescription += stat1;
                if (Attr2 != None)
                {
                    _solvedDescription += " from " + stat2;
                }
                _solvedDescription += ": ";
                double totalOffset = Math.Round(_level * LevelOffset + ConstantOffset, 4);
                bool addParenthesis = totalOffset != 0 && Variable != 0;
                if (addParenthesis)
                {
                    _solvedDescription += "(";
                }
                bool prefix = false;
                if (Variable != 0)
                {
                    _solvedDescription += Variable + " * " + GetVariableStat(Attr1);
                    prefix = true;
                }
                if (totalOffset != 0)
                {
                    _solvedDescription += (Math.Sign(totalOffset) < 0 ? " -" : " +") + (prefix ? " " : "") + Math.Abs(totalOffset);
                }
                if (addParenthesis)
                {
                    _solvedDescription += ")";
                }
                _solvedDescription += GetPercent(Attr1, Attr2);
            }
            return _solvedDescription;
        }
    }
}
