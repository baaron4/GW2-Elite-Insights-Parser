using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class SkillModeDescription
{
    public readonly ConnectorDescription? Owner;

    public readonly uint Category;

    public readonly long SkillID;
    public readonly bool IsBuff;

    internal SkillModeDescription(SkillModeDescriptor skillModeDescriptor, CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs) 
    {
        Category = (uint)skillModeDescriptor.Category;
        if (log.Buffs.BuffsByIDs.TryGetValue(skillModeDescriptor.SkillID, out var buff))
        {
            usedBuffs.TryAdd(buff.ID, buff);
            IsBuff = true;
        }
        else
        {
            SkillItem skill = log.SkillData.Get(skillModeDescriptor.SkillID);
            usedSkills.TryAdd(skill.ID, skill);
            IsBuff = false;
        }
        if (skillModeDescriptor.Owner != null)
        {
            Owner = skillModeDescriptor.Owner.GetConnectedTo(map, log);
        }
    }
}
