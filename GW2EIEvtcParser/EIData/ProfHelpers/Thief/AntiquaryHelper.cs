using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.ProfHelper;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

internal static class AntiquaryHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder = [];
    // TODO Exalted Hammer, Chak Shield, Unstable Skritt Bomb, Inquest Portal Device Backfired, Emergency Jade Shield (and backfire), Canach-Coin Toss
    // Verify if any of them already work https://wiki.guildwars2.com/wiki/Antiquary

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
        new Buff("Unstable Skritt Bomb", UnstableSkrittBombBuff, Source.Antiquary, BuffClassification.Other, SkillImages.UnstableSkrittBomb),
        new Buff("Forged Surfer Dash", ForgedSurferDashBuff, Source.Antiquary, BuffClassification.Other, SkillImages.ForgedSurferDash),
        new Buff("Zephyrite Sun Crystal", ZephyriteSunCrystalBuff, Source.Antiquary, BuffStackType.Stacking, 2, BuffClassification.Other, SkillImages.ZephyriteSunCrystal),
        new Buff("Holo-Dancer Decoy", HoloDancerDecoyBuff, Source.Antiquary, BuffStackType.StackingConditionalLoss, 2, BuffClassification.Other, SkillImages.HoloDancerDecoy),
        new Buff("Summon Kryptis Turret", SummonKryptisTurretBuff, Source.Antiquary, BuffClassification.Other, SkillImages.SummonKryptisTurret),
        new Buff("Metal Legion Guitar", MetalLegionGuitarBuff, Source.Antiquary, BuffStackType.StackingConditionalLoss, 4, BuffClassification.Other, SkillImages.MetalLegionGuitar),
        new Buff("Mistburn Mortar", MistburnMortarBuff, Source.Antiquary, BuffStackType.StackingConditionalLoss, 7, BuffClassification.Other, SkillImages.MistburnMortar),
        new Buff("Exalted Hammer", ExaltedHammerBuff, Source.Antiquary, BuffStackType.StackingConditionalLoss, 2, BuffClassification.Other, SkillImages.ExaltedHammer),
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
        var kryptisTurrets = combatData.GetBuffApplyData(KryptisTurretBuff1).ToList().Concat(combatData.GetBuffApplyData(KryptisTurretBuff2)).Select(x => x.To).Distinct();
        foreach (var kryptisTurret in kryptisTurrets)
        {
            if (kryptisTurret.Type == AgentItem.AgentType.Gadget)
            {
                kryptisTurret.OverrideType(AgentItem.AgentType.NPC, agentData);
                kryptisTurret.OverrideID(MinionID.KryptisTurret, agentData);
                kryptisTurret.OverrideName("Kryptis Turret");
            }
        }
        // TODO Improve this if necessary
        var gadgets = agentData.GetAgentByType(AgentItem.AgentType.Gadget);
        foreach (AgentItem gadget in gadgets)
        {
            var master = gadget.GetFinalMaster();
            if (master.IsPlayer && master.Spec == Spec.Antiquary)
            {
                if (gadget.HitboxWidth == 118 && gadget.HitboxHeight == 0)
                {
                    // The Holo-Dancer is correctly named
                    gadget.OverrideType(AgentItem.AgentType.NPC, agentData);
                    gadget.OverrideID(MinionID.HoloDancer, agentData);
                }
            }
        }
    }

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Thief;

        // Skritt Scuffle
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.AntiquarySkrittScuffle, out var skrittScuffle))
        {
            var skill = new SkillModeDescriptor(player, Spec.Antiquary, SkrittScuffle);
            foreach (EffectEvent effect in skrittScuffle)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 15000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 180, EffectImages.EffectSkrittScuffle);
            }
        }
    }
}
