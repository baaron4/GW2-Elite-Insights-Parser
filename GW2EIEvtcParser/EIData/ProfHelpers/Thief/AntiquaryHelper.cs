using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

internal static class AntiquaryHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder = [];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = 
    [
        // Exhilarating Ephemera
        new BuffOnActorDamageModifier(Mod_ExhilaratingEphemera, ExhilaratingEphemera, "Exhilarating Ephemera", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Antiquary, ByPresence, TraitImages.ExhilaratingEphemera, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_ExhilaratingEphemera, ExhilaratingEphemera, "Exhilarating Ephemera", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Antiquary, ByPresence, TraitImages.ExhilaratingEphemera, DamageModifierMode.sPvPWvW),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = [];

    internal static readonly IReadOnlyList<Buff> Buffs = 
    [
        new Buff("Kryptis Turret (1)", KryptisTurretBuff1, Source.Antiquary, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Kryptis Turret (2)", KryptisTurretBuff2, Source.Antiquary, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Exhilarating Ephemera", ExhilaratingEphemera, Source.Antiquary, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.ExhilaratingEphemera),
        new Buff("Prodigious Pincher", ProdigiousPincher, Source.Antiquary, BuffStackType.Stacking, 15, BuffClassification.Other, BuffImages.Unknown),
    ];

    private static readonly HashSet<int> Minions = 
    [
        (int)MinionID.KryptisTurret,
        (int)MinionID.HoloDancer,
        (int)MinionID.SkrittThievesGuild,
    ];

    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }

    public static void ProcessGadgets(IReadOnlyList<AgentItem> players, CombatData combatData, AgentData agentData)
    {
        // TODO Improve this if necessary
        var gadgets = agentData.GetAgentByType(AgentItem.AgentType.Gadget);
        foreach (AgentItem gadget in gadgets)
        {
            var master = gadget.GetFinalMaster();
            if (master.IsPlayer && master.Spec == Spec.Antiquary)
            {
                if (gadget.HitboxWidth == 40 && gadget.HitboxHeight == 0)
                {
                    gadget.OverrideType(AgentItem.AgentType.NPC, agentData);
                    gadget.OverrideID(MinionID.KryptisTurret, agentData);
                    gadget.OverrideName("Kryptis Turret");
                }
                if (gadget.HitboxWidth == 118 && gadget.HitboxHeight == 0)
                {
                    // The Holo-Dancer is correctly named
                    gadget.OverrideType(AgentItem.AgentType.NPC, agentData);
                    gadget.OverrideID(MinionID.HoloDancer, agentData);
                }
            }
        }
    }
}
