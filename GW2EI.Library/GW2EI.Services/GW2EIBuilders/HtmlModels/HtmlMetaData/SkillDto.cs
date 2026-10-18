using System.Text.Json;
using System.Text.Json.Serialization;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIBuilders.HtmlModels.LogDataDto;

namespace GW2EIBuilders.HtmlModels.HTMLMetaData;


/// <summary> A struct holding skill data that gets serialized as an array of numbers. </summary>
[JsonConverter(typeof(Converter))]
public struct SkillCastDto
{
    public double Start;
    public long SkillId;
    public int ActualDuration;
    public int Status;
    public double Quickness;
    public int Expected;
    public int Target;

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
            writer.WriteNumberValue(value.Quickness);
            writer.WriteNumberValue(value.Expected);
            writer.WriteNumberValue(value.Target);
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
    public bool UnconditionalProc;

    public SkillDto(SkillItem skill, ParsedEvtcLog log) : base(skill, log)
    {
        Aa = skill.IsAutoAttack(log);
        IsSwap = skill.IsSwap;
        NotAccurate = log.SkillData.IsNotAccurate(skill.ID);
        GearProc = log.SkillData.IsGearProc(skill.ID);
        TraitProc = log.SkillData.IsTraitProc(skill.ID);
        UnconditionalProc = log.SkillData.IsUnconditionalProc(skill.ID);
    }

    public static void AssembleSkills(ICollection<SkillItem> skills, Dictionary<string, SkillDto> dict, ParsedEvtcLog log)
    {
        dict.EnsureCapacity(dict.Count + skills.Count);

        foreach (SkillItem skill in skills)
        {
            dict["s" + skill.ID] = new SkillDto(skill, log);
        }
    }

    public static List<SkillCastDto> BuildRotationData(ParsedEvtcLog log, Actor p, PhaseData phase, UsedData used)
    {
        var casting = p.GetIntersectingCastEvents(log, phase.Start, phase.End).ToList();
        var list = new List<SkillCastDto>(casting.Count);
        foreach (CastEvent cl in casting)
        {
            if (cl.IgnoreOnRotationRender())
            {
                continue;
            }
            if (!used.skills.ContainsKey(cl.SkillID))
            {
                used.skills.Add(cl.SkillID, cl.Skill);
            }

            var target = cl.Target;
            var targetId = 0;
            if (target.IsPlayer)
            {
                targetId = -target.UniqueID;
            }
            else if (target.IsNPC && !target.IsUnamedSpecies())
            {
                used.npcs.TryAdd(target.ID, target);
                targetId = target.ID;
            }

            var cast = new SkillCastDto()
            {
                Start = (cl.Time - phase.Start) / 1000.0,
                SkillId = cl.SkillID,
                ActualDuration = cl.ActualDuration,
                Status = (int)cl.Status,
                Quickness = cl.AccelerationQuicknessSlow,
                Expected = cl.ExpectedScaledDuration,
                Target = targetId,
            };
            list.Add(cast);
        }
        return list;
    }
}
