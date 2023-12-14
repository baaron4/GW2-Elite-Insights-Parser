using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class ItemDamageModifiers
    {
        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new DamageLogDamageModifier("Moving Bonus", "Seaweed Salad (and the likes) – 5% while moving", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.Strike, Source.Item, BuffImages.BowlOfSeaweedSalad, (x, log) => x.IsMoving, DamageModifierMode.All),
            new BuffOnActorDamageModifier(FractalOffensive, "Fractal Offensive", "3% per stack", DamageSource.NoPets, 3.0, DamageType.StrikeAndCondition, DamageType.All, Source.Item, ByStack, BuffImages.FractalOffensive, DamageModifierMode.PvE),
            new CounterOnActorDamageModifier(WritOfMasterfulMalice, "Writ of Masterful Malice", "200 condition damage if hp >=90%", DamageSource.NoPets, DamageType.Condition, DamageType.Condition, Source.Item,  BuffImages.WritOfMasterfulMalice, DamageModifierMode.All)
                .UsingChecker((x, log) => x.IsOverNinety),
            new CounterOnActorDamageModifier(WritOfMasterfulStrength, "Writ of Masterful Strength", "200 power if hp >=90%", DamageSource.NoPets, DamageType.Strike, DamageType.Strike, Source.Item, BuffImages.WritOfMasterfulStrength, DamageModifierMode.All)
                .UsingChecker((x, log) => x.IsOverNinety),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor> {
            new BuffOnActorDamageModifier(FractalDefensive, "Fractal Defensive", "5% per stack", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Item, ByStack, BuffImages.FractalDefensive, DamageModifierMode.PvE),
            // Regroup consumables that reduce inc damage by their reduction instead of adding one modifier per consumable to reduce cluttering
        };

    }
}
