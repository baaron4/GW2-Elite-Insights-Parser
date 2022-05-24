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
    internal static class VirtuosoHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new DamageLogDamageModifier("Mental Focus", "10% to foes within 600 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Virtuoso, "https://wiki.guildwars2.com/images/d/da/Mental_Focus.png", (x,log) =>
            {
                Point3D currentPosition = x.From.GetCurrentPosition(log, x.Time);
                Point3D currentTargetPosition = x.To.GetCurrentPosition(log, x.Time);
                if (currentPosition == null || currentTargetPosition == null)
                {
                    return false;
                }
                return currentPosition.DistanceToPoint(currentTargetPosition) <= 600;
            }, ByPresence, GW2Builds.EODBeta1, GW2Builds.EndOfLife, DamageModifierMode.PvE).UsingApproximate(true),
            new BuffDamageModifier(63409, "Deadly Blades", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Virtuoso, ByPresence, "https://wiki.guildwars2.com/images/1/15/Deadly_Blades.png", GW2Builds.EODBeta4, GW2Builds.EndOfLife, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Deadly Blades", DeadlyBlades, Source.Virtuoso, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/15/Deadly_Blades.png"),
            new Buff("Virtuoso Blade", VirtuosoBlades, Source.Virtuoso, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, "https://wiki.guildwars2.com/images/thumb/d/d6/Power_attribute.png/20px-Power_attribute.png"),
        };

        public static List<AbstractBuffEvent> TransformVirtuosoBladeStorage(IReadOnlyList<AbstractBuffEvent> buffs, AgentItem a, SkillData skillData)
        {
            var res = new List<AbstractBuffEvent>();
            var bladeIDs = new HashSet<long>
            {
                VirtuosoBlade1,
                VirtuosoBlade2,
                VirtuosoBlade3,
                VirtuosoBlade4,
                VirtuosoBlade5,
            };
            var blades = buffs.Where(x => bladeIDs.Contains(x.BuffID)).ToList();
            SkillItem skill = skillData.Get(VirtuosoBlades);
            var lastAddedBuffInstance = new Dictionary<long, uint>();
            foreach (AbstractBuffEvent blade in blades)
            {
                if (blade is BuffApplyEvent bae)
                {
                    res.Add(new BuffApplyEvent(a, a, bae.Time, bae.AppliedDuration, skill, bae.BuffInstance, true));
                    lastAddedBuffInstance[blade.BuffID] = bae.BuffInstance;
                } 
                else if (blade is BuffRemoveAllEvent brae)
                {
                    if (!lastAddedBuffInstance.TryGetValue(blade.BuffID, out uint remmovedInstance))
                    {
                        remmovedInstance = 0;
                    }
                    res.Add(new BuffRemoveSingleEvent(a, a, brae.Time, brae.RemovedDuration, skill, true, remmovedInstance));
                }
                else if (blade is BuffRemoveSingleEvent brse)
                {
                    res.Add(new BuffRemoveSingleEvent(a, a, brse.Time, brse.RemovedDuration, skill, true, brse.BuffInstance));
                }
            }
            return res;
        }

        private static HashSet<long> Minions = new HashSet<long>();
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }
    }
}
