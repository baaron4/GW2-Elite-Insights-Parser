using System.Collections.Generic;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class ScrapperHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new EffectCastFinder(BulwarkGyro, EffectGUIDs.ScrapperBulwarkGyro)
                .UsingSrcSpecChecker(Spec.Scrapper),
            new EffectCastFinder(PurgeGyro, EffectGUIDs.ScrapperPurgeGyro)
                .UsingSrcSpecChecker(Spec.Scrapper),
            new EffectCastFinder(DefenseField, EffectGUIDs.ScrapperDefenseField)
                .UsingSrcSpecChecker(Spec.Scrapper),
            new EffectCastFinder(BypassCoating, EffectGUIDs.ScrapperBypassCoating)
                .UsingSrcSpecChecker(Spec.Scrapper),
        };

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(new long[] { Swiftness, Superspeed, Stability }, "Object in Motion", "5% under swiftness/superspeed/stability, cumulative", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Scrapper, ByMultiPresence, BuffImages.ObjectInMotion, DamageModifierMode.All)
                .WithBuilds(GW2Builds.July2019Balance)
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new DamageLogDamageModifier("Adaptive Armor", "-20%", DamageSource.NoPets, -20.0, DamageType.Condition, DamageType.All, Source.Scrapper, BuffImages.AdaptiveArmor, (x, log) => x.ShieldDamage > 0 , DamageModifierMode.All).WithBuilds(GW2Builds.July2019Balance),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Watchful Eye", WatchfulEye, Source.Scrapper, BuffClassification.Defensive, BuffImages.BulwarkGyro),
            new Buff("Watchful Eye PvP", WatchfulEyePvP, Source.Scrapper, BuffClassification.Defensive, BuffImages.BulwarkGyro),
        };

        private static HashSet<int> Minions = new HashSet<int>()
        {
            (int)MinionID.BlastGyro,
            (int)MinionID.BulwarkGyro,
            (int)MinionID.FunctionGyro,
            (int)MinionID.MedicGyro,
            (int)MinionID.ShredderGyro,
            (int)MinionID.SneakGyro,
            (int)MinionID.PurgeGyro,
        };

        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Engineer;

            // Function Gyro
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ScrapperFunctionGyro, out IReadOnlyList<EffectEvent> functionGyros))
            {
                var skill = new SkillModeDescriptor(player, Spec.Scrapper, FunctionGyro, SkillModeCategory.ShowOnSelect);
                foreach (EffectEvent effect in functionGyros)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectFunctionGyro, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
            // Defense Field
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ScrapperDefenseField, out IReadOnlyList<EffectEvent> defenseFields))
            {
                var skill = new SkillModeDescriptor(player, Spec.Scrapper, DefenseField, SkillModeCategory.ProjectileManagement | SkillModeCategory.ImportantBuffs);
                foreach (EffectEvent effect in defenseFields)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectDefenseField, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
        }
    }
}
