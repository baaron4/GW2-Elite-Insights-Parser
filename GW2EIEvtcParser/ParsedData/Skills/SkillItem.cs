using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using GW2EIGW2API.GW2API;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParsedData.WeaponDescriptor;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.ParsedData;

public class SkillItem
{
    internal static (long, long) GetArcDPSCustomIDs(EvtcVersionEvent evtcVersion)
    {
        if (evtcVersion.Build >= ArcDPSBuilds.InternalSkillIDsChange)
        {
            return (ArcDPSDodge20220307, ArcDPSGenericBreakbar20220307);
        }
        else
        {
            return (ArcDPSDodge, ArcDPSGenericBreakbar);
        }
    }
    private const string DefaultIcon = SkillImages.MonsterSkill;

    // Fields
    public readonly long ID;
    //public int Range { get; private set; } = 0;
    private readonly bool AA;

    public bool IsSwap => ID == WeaponSwap
        || ElementalistHelper.IsAttunementSwap(ID)
        || WeaverHelper.IsAttunementSwap(ID)
        || RevenantHelper.IsLegendSwap(ID)
        || HeraldHelper.IsLegendSwap(ID)
        || RenegadeHelper.IsLegendSwap(ID)
        || VindicatorHelper.IsLegendSwap(ID)
        || ConduitHelper.IsLegendSwap(ID)
        || NecromancerHelper.IsDeathShroudTransform(ID)
        || HarbingerHelper.IsHarbingerShroudTransform(ID)
        || RitualistHelper.IsRitualistShroudTransform(ID);
    public bool IsDodge(SkillData skillData) => ID == MirageCloakDodge
        || IsAnimatedDodge(skillData);
    public bool IsAnimatedDodge(SkillData skillData) => ID == skillData.DodgeID;
    public bool IsAutoAttack(ParsedEvtcLog log) => AA
        || FirebrandHelper.IsAutoAttack(log, ID)
        || BladeswornHelper.IsAutoAttack(log, ID);
    public string Name { get; private set; } = "";
    public string Icon { get; private set; } = "";
    private readonly WeaponDescriptor? _weaponDescriptor;
    public bool IsWeaponSkill => _weaponDescriptor != null;
    internal readonly GW2APISkill? ApiSkill;
    private SkillInfoEvent? _skillInfo;

    internal const string DefaultName = "UNKNOWN";

    public bool UnknownSkill => Name == DefaultName;

    // Constructor

    [Obsolete("Dont use this, testing only")] //TODO(Rennorb) @cleanup
    public SkillItem(bool swap) { ID = swap ? WeaponSwap : default; }

    internal SkillItem(long ID, string name, GW2APIController apiController)
    {
        this.ID = ID;
        Name = name.Replace("\0", "");
        ApiSkill = apiController.GetAPISkill(ID);
        //
        if (SkillItemOverrides.OverridenSkillNames.TryGetValue(ID, out var overrideName))
        {
            Name = overrideName;
        }
        else if (ApiSkill != null && (UnknownSkill || Name.All(char.IsDigit)))
        {
            Name = ApiSkill.Name;
        }
        if (SkillItemOverrides.OverridenSkillIcons.TryGetValue(ID, out var icon))
        {
            Icon = icon;
        }
        else
        {
            Icon = ApiSkill != null ? ApiSkill.Icon : DefaultIcon;
        }
        if (ApiSkill != null && ApiSkill.Type == "Weapon"
            && ApiSkill.WeaponType != "None" && ApiSkill.Professions.Count > 0
            && IsWeaponSlot(ApiSkill.Slot))
        {
            // Special handling of specter shroud as it is not done in the same way
            var isSpecterShroud = ApiSkill.Professions.Contains("Thief") && ApiSkill.Facts.Any(x => x.Text != null && x.Text.Contains("Tethered Ally"));
            if (!isSpecterShroud)
            {
                _weaponDescriptor = new WeaponDescriptor(ApiSkill);
            }
        }
        AA = (ApiSkill?.Slot == "Weapon_1" || ApiSkill?.Slot == "Downed_1");
        if (AA)
        {
            if (ApiSkill?.Categories != null)
            {
                AA = AA && !ApiSkill.Categories.Contains("StealthAttack") && !ApiSkill.Categories.Contains("Ambush"); // Ambush in case one day it's added
            }
            if (ApiSkill?.Description != null)
            {
                AA = AA && !ApiSkill.Description.Contains("Ambush.");
            }
        }
#if DEBUG
        Name = ID + "-" + Name;
#endif
    }

    public static bool CanCrit(long id, ulong gw2Build)
    {
        if (SkillItemOverrides.NonCritableSkills.TryGetValue(id, out ulong build))
        {
            return gw2Build < build;
        }
        return true;
    }

    internal void OverrideFromBuff(Buff buff)
    {
        Name = buff.Name;
        Icon = buff.Link;
    }

    internal int FindFirstWeaponSet(IReadOnlyList<WeaponSwapEvent> swaps)
    {
        int swapped = WeaponSetIDs.NoSet;
        // we started on a proper weapon set
        if (_weaponDescriptor != null)
        {
            swapped = _weaponDescriptor.FindFirstWeaponSet(swaps);
        }
        return swapped;
    }

    internal WeaponEstimateResult EstimateWeapons(WeaponSet weaponSet, long time, int swapped, bool validForCurrentSwap)
    {
        bool keep = WeaponSetIDs.IsWeaponSet(swapped);
        if (_weaponDescriptor == null || !keep || !validForCurrentSwap || ApiSkill == null)
        {
            weaponSet.End = time;
            return WeaponEstimateResult.NotApplicable;
        }
        return weaponSet.SetWeapons(_weaponDescriptor, ApiSkill, time, swapped) ? WeaponEstimateResult.Updated : WeaponEstimateResult.NeedNewSet;
    }

    internal void AttachSkillInfoEvent(SkillInfoEvent skillInfo)
    {
        if (ID == skillInfo.SkillID)
        {
            _skillInfo = skillInfo;
        }
    }
}
