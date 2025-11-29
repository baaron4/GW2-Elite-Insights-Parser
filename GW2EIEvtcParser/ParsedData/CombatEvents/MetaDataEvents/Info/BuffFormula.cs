using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ArcDPSEnums.BuffAttribute;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData;

public class BuffFormula
{
    private static string GetAttributeString(BuffAttribute attribute)
    {
        return BuffAttributesStrings.TryGetValue(attribute, out var output) ? output : "";
    }

    private static string GetVariableStat(BuffAttribute attribute, int type)
    {
        switch (attribute)
        {
            case DamageFormulaSquaredLevel:
            case DamageFormula:
            case SkillActivationDamageFormula:
            case MovementActivationDamageFormula:
                return type > 10 ? "Power" : "Condition Damage";
            case HealingOutputFormula:
                return "Healing Power";
            case Unknown:
                return "Unknown";
            default:
                return "";
        }
    }

    private static string GetPercent(BuffAttribute attribute1, BuffAttribute attribute2)
    {
        if (attribute2 != Unknown && attribute2 != None)
        {
            return "%";
        }
        return BuffAttributesPercent.TryGetValue(attribute1, out var value) ? value : "";
    }

    // Effect type
    public readonly int Type;
    // Effect attributes
    public readonly byte ByteAttr1;
    public BuffAttribute Attr1 { get; private set; }
    public readonly byte ByteAttr2;
    public BuffAttribute Attr2 { get; private set; }
    // Effect parameters
    public readonly float ConstantOffset;
    public readonly float LevelOffset;
    public readonly float Variable;
    // Effect Condition
    public readonly int TraitSrc;
    public readonly int TraitSelf;
    public readonly float ContentReference;
    public readonly int BuffSrc;
    public readonly int BuffSelf;
    internal long SortKey => TraitSrc + TraitSelf + BuffSrc + BuffSelf;
    public bool IsConditional => SortKey > 0;
    // Meta data
    private readonly bool Npc;
    private readonly bool Player;
    private readonly bool Break;
    // Extra number
    private readonly byte ExtraNumberState;
    private readonly uint ExtraNumber;
    private bool IsExtraNumberBuffID => ExtraNumberState == 2;
    private bool IsExtraNumberNone => ExtraNumberState == 0;
    private bool IsExtraNumberSomething => ExtraNumberState == 1;

    private bool IsFlippedFormula => Attr1 == PhysIncomingMultiplicative || Attr1 == CondIncomingMultiplicative || Attr1 == HealingEffectivenessIncomingMultiplicative;

    private bool MultiplyBy100 => Attr2 != None || Attr1 == HealingEffectivenessIncomingMultiplicative;

    private string? _solvedDescription = null;


    private int Level(Buff buff) => buff.Classification == Buff.BuffClassification.Enhancement || buff.Classification == Buff.BuffClassification.Nourishment || buff.Classification == Buff.BuffClassification.OtherConsumable ? 0 : (Attr1 == DamageFormulaSquaredLevel ? 6400 : 80);

    internal unsafe BuffFormula(CombatItem evtcItem, EvtcVersionEvent evtcVersion)
    {
        Npc = evtcItem.IsFlanking == 0;
        Player = evtcItem.IsShields == 0;
        Break = evtcItem.IsOffcycle > 0;
        var formulaBytes = new ByteBuffer(stackalloc byte[11 * sizeof(float)]);
        // 2 
        formulaBytes.PushNative(evtcItem.Time);
        // 2
        formulaBytes.PushNative(evtcItem.SrcAgent);
        // 2
        formulaBytes.PushNative(evtcItem.DstAgent);
        // 1
        formulaBytes.PushNative(evtcItem.Value);
        // 1
        formulaBytes.PushNative(evtcItem.BuffDmg);
        // 1
        formulaBytes.PushNative(evtcItem.OverstackValue);
        // 0.5
        formulaBytes.PushNative(evtcItem.SrcInstid);
        // 0.5
        formulaBytes.PushNative(evtcItem.DstInstid);
        // 0.5
        formulaBytes.PushNative(evtcItem.SrcMasterInstid);
        // 0.5
        formulaBytes.PushNative(evtcItem.DstMasterInstid);

        fixed(byte* ptr = formulaBytes.Span) {
            var formulaFloats = (float*)ptr;

            Type = (int)formulaFloats[0];
            ByteAttr1 = (byte)formulaFloats[1];
            ByteAttr2 = (byte)formulaFloats[2];
            Attr1 = GetBuffAttribute(ByteAttr1, Type, evtcVersion.Build);
            Attr2 = GetBuffAttribute(ByteAttr2, Type,evtcVersion.Build);
            ConstantOffset = formulaFloats[3];
            LevelOffset = formulaFloats[4];
            Variable = formulaFloats[5];
            TraitSrc = (int)formulaFloats[6];
            TraitSelf = (int)formulaFloats[7];
            ContentReference = formulaFloats[8];
            BuffSrc = (int)formulaFloats[9];
            BuffSelf = (int)formulaFloats[10];
        }
        ExtraNumber = evtcItem.OverstackValue;
        ExtraNumberState = evtcItem.Pad1;
    }

    internal void AdjustUnknownFormulaAttributes(Dictionary<byte, BuffAttribute> solved)
    {
        if (Attr1 == Unknown && solved.TryGetValue(ByteAttr1, out BuffAttribute solvedAttr))
        {
            Attr1 = solvedAttr;
        }
        if (Attr2 == Unknown && solved.TryGetValue(ByteAttr2, out solvedAttr))
        {
            Attr2 = solvedAttr;
        }
    }

    public string GetDescription(bool authorizeUnknowns, IReadOnlyDictionary<long, Buff> buffsByIDs, Buff buff)
    {
        if (!authorizeUnknowns && (Attr1 == Unknown || Attr2 == Unknown))
        {
            return "";
        }
        if (_solvedDescription != null)
        {
            return _solvedDescription;
        }
        _solvedDescription = "";
        if (Attr1 == None)
        {
            return _solvedDescription;
        }
        string stat1 = GetAttributeString(Attr1);
        if (Attr1 == Unknown)
        {
            stat1 += " " + ByteAttr1;
        }
        if (IsExtraNumberBuffID)
        {
            if (buffsByIDs.TryGetValue(ExtraNumber, out var otherBuff))
            {
                stat1 += " (" + otherBuff.Name + ")";
            }
        }
        string stat2 = GetAttributeString(Attr2);
        if (Attr2 == Unknown)
        {
            stat2 += " " + ByteAttr2;
        }
        _solvedDescription += stat1;
        double variable = Math.Round(Variable, 6);
        double totalOffset = Math.Round(Level(buff) * LevelOffset + ConstantOffset, 6);
        bool addParenthesis = totalOffset != 0 && Variable != 0;
        if (Attr2 != None)
        {
            _solvedDescription += " from " + stat2;
        }
        if (MultiplyBy100)
        {
            totalOffset *= 100.0;
            variable *= 100.0;
        }
        if (IsFlippedFormula)
        {
            variable = variable - 100.0;
            totalOffset = totalOffset - 100.0;
        }
        _solvedDescription += ": ";
        if (addParenthesis)
        {
            _solvedDescription += "(";
        }
        bool prefix = false;
        if (Variable != 0)
        {
            _solvedDescription += variable + " * " + GetVariableStat(Attr1, Type);
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
        if (Npc && !Player)
        {
            _solvedDescription += ", on NPCs";
        }
        if (!Npc && Player)
        {
            _solvedDescription += ", on Players";
        }
        if (TraitSelf > 0)
        {
            _solvedDescription += ", using " + TraitSelf;
        }
        if (TraitSrc > 0)
        {
            _solvedDescription += ", source using " + TraitSrc;
        }
        if (BuffSelf > 0)
        {
            _solvedDescription += ", under " + BuffSelf;
        }
        if (BuffSrc > 0)
        {
            _solvedDescription += ", source under " + BuffSrc;
        }
        return _solvedDescription;
    }
}
