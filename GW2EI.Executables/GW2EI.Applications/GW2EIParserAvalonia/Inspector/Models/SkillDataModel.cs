using GW2EIEvtcParser.ParsedData;

namespace GW2EIParserAvalonia.Models;

public sealed class SkillDataModel
{
    public long ID { get; }
    public string Name { get; }
    public string Icon { get; }
    public bool UnknownSkill { get; }
    public bool IsSwap { get; }
    public bool IsAnimatedDodge { get; }
    public bool IsDodge { get; }
    public bool IsWeaponSkill { get; }
    public bool CanHeal { get; }
    public bool IsNotAccurate { get; }
    public bool IsGearProc { get; }
    public bool IsTraitProc { get; }
    public bool IsUnconditionalProc { get; }

    public string ApiName { get; }
    public string ApiIcon { get; }
    public string ApiType { get; }
    public string ApiSlot { get; }
    public string ApiWeaponType { get; }
    public string ApiDescription { get; }
    public string ApiProfessions { get; }
    public string ApiCategories { get; }

    public SkillItem SkillItem { get; }

    public SkillDataModel(SkillItem skill, SkillData skillData)
    {
        SkillItem = skill;

        ID = skill.ID;
        Name = skill.Name;
        Icon = skill.Icon;
        UnknownSkill = skill.UnknownSkill;
        IsSwap = skill.IsSwap;
        IsAnimatedDodge = skill.IsAnimatedDodge(skillData);
        IsDodge = skill.IsDodge(skillData);
        IsWeaponSkill = skill.IsWeaponSkill;
        CanHeal = skill.CanHeal;

        IsNotAccurate = skillData.IsNotAccurate(skill.ID);
        IsGearProc = skillData.IsGearProc(skill.ID);
        IsTraitProc = skillData.IsTraitProc(skill.ID);
        IsUnconditionalProc = skillData.IsUnconditionalProc(skill.ID);

        ApiName = skill.ApiSkill?.Name ?? string.Empty;
        ApiIcon = skill.ApiSkill?.Icon ?? string.Empty;
        ApiType = skill.ApiSkill?.Type ?? string.Empty;
        ApiSlot = skill.ApiSkill?.Slot ?? string.Empty;
        ApiWeaponType = skill.ApiSkill?.WeaponType ?? string.Empty;
        ApiDescription = skill.ApiSkill?.Description ?? string.Empty;
        ApiProfessions = skill.ApiSkill?.Professions != null ? string.Join(", ", skill.ApiSkill.Professions) : string.Empty;
        ApiCategories = skill.ApiSkill?.Categories != null ? string.Join(", ", skill.ApiSkill.Categories) : string.Empty;
    }
}
