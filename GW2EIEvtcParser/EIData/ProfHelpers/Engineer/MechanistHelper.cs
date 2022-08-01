using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class MechanistHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new EffectCastFinder(ShiftSignetSkill, EffectGUIDs.MechanistShiftSignet).UsingChecker((evt, log) => evt.Src.Spec == Spec.Mechanist),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Need to check mech specy id for those
            new BuffDamageModifier(ForceSignet, "Force Signet", "10%, including Mech", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Mechanist, ByPresence, "https://wiki.guildwars2.com/images/b/b3/Force_Signet.png", DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta4).UsingChecker((x,log) =>
            {
                return x.From == x.CreditedFrom || x.From.ID == (int)MinionID.JadeMech;
            }),
            new BuffDamageModifier(SuperconductingSignet, "Superconducting Signet", "10%, including Mech", DamageSource.All, 10.0, DamageType.Condition, DamageType.All, Source.Mechanist, ByPresence, "https://wiki.guildwars2.com/images/5/51/Superconducting_Signet.png", DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta4).UsingChecker((x,log) =>
            {
                return x.From == x.CreditedFrom || x.From.ID == (int)MinionID.JadeMech;
            }),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Rectifier Signet",RectifierSignet, Source.Mechanist, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c4/Rectifier_Signet.png"),
            new Buff("Barrier Signet",BarrierSignet, Source.Mechanist, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b8/Barrier_Signet.png"),
            new Buff("Force Signet",ForceSignet, Source.Mechanist, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b3/Force_Signet.png"),
            new Buff("Shift Signet",ShiftSignetEffect, Source.Mechanist, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d1/Shift_Signet.png"),
            new Buff("Superconducting Signet",SuperconductingSignet, Source.Mechanist, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/51/Superconducting_Signet.png"),
            new Buff("Overclock Signet",OverclockSignet, Source.Mechanist, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c7/Overclock_Signet.png"),
            //
            //new Buff("Rectifier Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c4/Rectifier_Signet.png"),
            new Buff("Barrier Signet (J-Drive)",BarrierSignetJDrive, Source.Mechanist, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b8/Barrier_Signet.png"),
            //new Buff("Force Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b3/Force_Signet.png"),
            //new Buff("Shift Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d1/Shift_Signet.png"),
            //new Buff("Superconducting Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/51/Superconducting_Signet.png"),
            new Buff("Overclock Signet (J-Drive)",OverclockSignetJDrive, Source.Mechanist, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c7/Overclock_Signet.png"),
        };

        private static HashSet<long> Minions = new HashSet<long>()
        {
            (int)MinionID.JadeMech,
        };
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }

    }
}
