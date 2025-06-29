using System.Text.Json;
using System.Text.Json.Serialization;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.HTMLMetaData;


/// <summary> A struct holding skill data that gets serialized as an array of numbers. </summary>
[JsonConverter(typeof(Converter))]
public struct SkillCastDto
{
    public double Start;
    public long   SkillId;
    public int    ActualDuration;
    public int    Status;
    public double Acceleration;

    public class Converter : JsonConverter<SkillCastDto>
    {
        public override SkillCastDto Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, SkillCastDto value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.Start);
            writer.WriteNumberValue(value.SkillId);
            writer.WriteNumberValue(value.ActualDuration);
            writer.WriteNumberValue(value.Status);
            writer.WriteNumberValue(value.Acceleration);
            writer.WriteEndArray();
        }
    }
}


internal class SkillDto : IDItemDto
{
    public bool Aa;
    public bool IsSwap;
    public bool NotAccurate;
    public bool TraitProc;
    public bool GearProc;

    public SkillDto(SkillItem skill, ParsedEvtcLog log) : base(skill, log)
    {
        Aa = skill.IsAutoAttack(log);
        IsSwap = skill.IsSwap;
        NotAccurate = log.SkillData.IsNotAccurate(skill.ID);
        GearProc = log.SkillData.IsGearProc(skill.ID);
        TraitProc = log.SkillData.IsTraitProc(skill.ID);
    }

    public static void AssembleSkills(ICollection<SkillItem> skills, Dictionary<string, SkillDto> dict, ParsedEvtcLog log)
    {
        dict.EnsureCapacity(dict.Count + skills.Count);

        foreach (SkillItem skill in skills)
        {
            dict["s" + skill.ID] = new SkillDto(skill, log);
        }
    }

    private static SkillCastDto GetSkillData(CastEvent cl, long phaseStart)
    {
        return new SkillCastDto()
        {
            Start = (cl.Time - phaseStart) / 1000.0,
            SkillId = cl.SkillID,
            ActualDuration = cl.ActualDuration,
            Status = (int)cl.Status,
            Acceleration = cl.Acceleration,
        };
    }

    public static List<SkillCastDto> BuildRotationData(ParsedEvtcLog log, SingleActor p, PhaseData phase, Dictionary<long, SkillItem> usedSkills)
    {
        var casting = p.GetIntersectingCastEvents(log, phase.Start, phase.End).ToList();
        var list = new List<SkillCastDto>(casting.Count);
        foreach (CastEvent cl in casting)
        {
            if (!usedSkills.ContainsKey(cl.SkillID))
            {
                usedSkills.Add(cl.SkillID, cl.Skill);
            }

            list.Add(GetSkillData(cl, phase.Start));
        }
        return list;
    }
}
