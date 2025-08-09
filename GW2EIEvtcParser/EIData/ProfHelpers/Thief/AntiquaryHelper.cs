using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2EIEvtcParser.EIData;

internal static class AntiquaryHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder = [];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = [];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = [];

    internal static readonly IReadOnlyList<Buff> Buffs = [];
}
