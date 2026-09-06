using GW2EIEvtcParser.ParsedData;

namespace GW2EIParserAvalonia.Models;

public sealed class SkillDataModel
{
    public long ID => _skill.ID;
    public string Name => _skill.Name;
    public string Icon => _skill.Icon;
    public bool UnknownSkill => _skill.UnknownSkill;
    public bool IsSwap => _skill.IsSwap;
    public bool IsAnimatedDodge => _skill.IsAnimatedDodge(_skillData);
    public bool IsDodge => _skill.IsDodge(_skillData);
    public bool IsWeaponSkill => _skill.IsWeaponSkill;
    public bool CanHeal => _skill.CanHeal;
    public bool IsNotAccurate => _skillData.IsNotAccurate(_skill.ID);
    public bool IsGearProc => _skillData.IsGearProc(_skill.ID);
    public bool IsTraitProc => _skillData.IsTraitProc(_skill.ID);
    public bool IsUnconditionalProc => _skillData.IsUnconditionalProc(_skill.ID);
    public string ApiName => _skill.ApiSkill?.Name ?? string.Empty;
    public string ApiIcon => _skill.ApiSkill?.Icon ?? string.Empty;
    public string ApiType => _skill.ApiSkill?.Type ?? string.Empty;
    public string ApiSlot => _skill.ApiSkill?.Slot ?? string.Empty;
    public string ApiWeaponType => _skill.ApiSkill?.WeaponType ?? string.Empty;
    public string ApiDescription => _skill.ApiSkill?.Description ?? string.Empty;
    public string ApiProfessions => _skill.ApiSkill?.Professions != null ? string.Join(", ", _skill.ApiSkill.Professions) : string.Empty;
    public string ApiCategories => _skill.ApiSkill?.Categories != null ? string.Join(", ", _skill.ApiSkill.Categories) : string.Empty;
    public SkillItem SkillItem => _skill;
    private readonly SkillItem _skill;
    private readonly SkillData _skillData;

    public SkillDataModel(SkillItem skill, SkillData skillData)
    {
        _skill = skill;
        _skillData = skillData;
    }
}
