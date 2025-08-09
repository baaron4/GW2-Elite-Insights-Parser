using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

internal static class RitualistHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder = [];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = [];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = [];

    internal static readonly IReadOnlyList<Buff> Buffs = [];

    private static readonly HashSet<int> Minions = [];

    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }

    private static readonly HashSet<long> _ritualistShroudTransform = [];

    public static bool IsRitualistShroudTransform(long id)
    {
        return _ritualistShroudTransform.Contains(id);
    }
}
