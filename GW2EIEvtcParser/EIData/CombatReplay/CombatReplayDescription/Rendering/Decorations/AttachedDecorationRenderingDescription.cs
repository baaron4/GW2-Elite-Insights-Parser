using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.AttachedDecoration;

namespace GW2EIEvtcParser.EIData;

public abstract class AttachedDecorationRenderingDescription : DecorationRenderingDescription
{
    public readonly object ConnectedTo;
    public readonly object? RotationConnectedTo;

    public class SkillModeDescription
    {
        public object? Owner { get; internal set; }

        public uint Category { get; internal set; }

        public long SkillID { get; internal set; }
        public bool IsBuff { get; internal set; }
    }
    public readonly object? SkillMode = null;

    internal AttachedDecorationRenderingDescription(ParsedEvtcLog log, AttachedDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(decoration, metadataSignature)
    {
        ConnectedTo = decoration.ConnectedTo.GetConnectedTo(map, log);
        RotationConnectedTo = decoration.RotationConnectedTo?.GetConnectedTo(map, log);
        IsMechanicOrSkill = true;
        if (decoration.SkillMode != null)
        {
            var skillModeDescription = new SkillModeDescription
            {
                Category = (uint)decoration.SkillMode.Category
            };
            if (log.Buffs.BuffsByIds.TryGetValue(decoration.SkillMode.SkillID, out var buff))
            {
                usedBuffs.TryAdd(buff.ID, buff);
                skillModeDescription.IsBuff = true;
            }
            else
            {
                SkillItem skill = log.SkillData.Get(decoration.SkillMode.SkillID);
                usedSkills.TryAdd(skill.ID, skill);
                skillModeDescription.IsBuff = false;
            }
            if (decoration.SkillMode.Owner != null)
            {
                skillModeDescription.Owner = decoration.SkillMode.Owner.GetConnectedTo(map, log);
            }
            SkillMode = skillModeDescription;
        }
    }
}
