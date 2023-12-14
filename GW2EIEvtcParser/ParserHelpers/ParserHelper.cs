using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser
{
    public static class ParserHelper
    {

        internal static readonly AgentItem _unknownAgent = new AgentItem();

        public const int CombatReplayPollingRate = 150;
        internal const int CombatReplaySkillDefaultSizeInPixel = 22;
        internal const int CombatReplaySkillDefaultSizeInWorld = 90;
        internal const int CombatReplayOverheadDefaultSizeInPixel = 20;
        internal const float CombatReplayOverheadDefaultOpacity = 1.0f;

        public const int MinionLimit = 1500;

        internal const int BuffDigit = 3;
        internal const int DamageModGainDigit = 3;
        internal const int AccelerationDigit = 3;
        internal const int CombatReplayDataDigit = 3;
        internal const int TimeDigit = 3;

        internal const long ServerDelayConstant = 10;
        internal const long BuffSimulatorDelayConstant = 15;
        internal const long BuffSimulatorStackActiveDelayConstant = 50;
        internal const long WeaponSwapDelayConstant = 75;
        internal const long TimeThresholdConstant = 150;

        internal const long InchDistanceThreshold = 10;

        internal const long MinimumInCombatDuration = 2200;

        internal const int PhaseTimeLimit = 2000;


        public enum Source
        {
            Common,
            Item, Gear,
            Necromancer, Reaper, Scourge, Harbinger,
            Elementalist, Tempest, Weaver, Catalyst,
            Mesmer, Chronomancer, Mirage, Virtuoso,
            Warrior, Berserker, Spellbreaker, Bladesworn,
            Revenant, Herald, Renegade, Vindicator,
            Guardian, Dragonhunter, Firebrand, Willbender,
            Thief, Daredevil, Deadeye, Specter,
            Ranger, Druid, Soulbeast, Untamed,
            Engineer, Scrapper, Holosmith, Mechanist,
            PetSpecific,
            FightSpecific,
            FractalInstability,
            Unknown
        };

        public enum Spec
        {
            Necromancer, Reaper, Scourge, Harbinger,
            Elementalist, Tempest, Weaver, Catalyst,
            Mesmer, Chronomancer, Mirage, Virtuoso,
            Warrior, Berserker, Spellbreaker, Bladesworn,
            Revenant, Herald, Renegade, Vindicator,
            Guardian, Dragonhunter, Firebrand, Willbender,
            Thief, Daredevil, Deadeye, Specter,
            Ranger, Druid, Soulbeast, Untamed,
            Engineer, Scrapper, Holosmith, Mechanist,
            NPC, Gadget,
            Unknown
        };

        [Flags]
        public enum DamageType
        {
            All = 0,
            Power = 1 << 0,
            Strike = 1 << 1,
            Condition = 1 << 2,
            StrikeAndCondition = Strike | Condition, // Common
            LifeLeech = 1 << 3,
            StrikeAndConditionAndLifeLeech = Strike | Condition | LifeLeech, // Common
        };
        public static string DamageTypeToString(this DamageType damageType)
        {
            if (damageType == DamageType.All)
            {
                return "All Damage";
            }
            string str = "";
            bool addComa = false;
            if ((damageType & DamageType.Power) > 0)
            {
                if (addComa)
                {
                    str += ", ";
                }
                str += "Power";
                addComa = true;
            }
            if ((damageType & DamageType.Strike) > 0)
            {
                if (addComa)
                {
                    str += ", ";
                }
                str += "Strike";
                addComa = true;
            }
            if ((damageType & DamageType.Condition) > 0)
            {
                if (addComa)
                {
                    str += ", ";
                }
                str += "Condition";
                addComa = true;
            }
            if ((damageType & DamageType.LifeLeech) > 0)
            {
                if (addComa)
                {
                    str += ", ";
                }
                str += "Life Leech";
                addComa = true;
            }
            str += " Damage";
            return str;
        }
        public enum BuffEnum { Self, Group, OffGroup, Squad };

        internal static Dictionary<long, List<T>> GroupByTime<T>(IReadOnlyList<T> list) where T : AbstractTimeCombatEvent
        {
            var groupByTime = new Dictionary<long, List<T>>();
            foreach (T c in list)
            {
                long key = groupByTime.Keys.FirstOrDefault(x => Math.Abs(x - c.Time) < ServerDelayConstant);
                if (key != 0)
                {
                    groupByTime[key].Add(c);
                }
                else
                {
                    groupByTime[c.Time] = new List<T>
                            {
                                c
                            };
                }
            }
            return groupByTime;
        }

        internal static double RadianToDegree(double radian)
        {
            return radian * 180.0 / Math.PI;
        }

        internal static float RadianToDegreeF(double radian)
        {
            return (float)RadianToDegree(radian);
        }

        internal static double DegreeToRadian(double degree)
        {
            return degree * Math.PI / 180.0;
        }

        internal static float DegreeToRadianF(double degree)
        {
            return (float)DegreeToRadian(degree);
        }

        /// <summary>
        /// Given a <paramref name="degree"/>, calculates the <see cref="Math.Cos(double)"/> and <see cref="Math.Sin(double)"/> values.
        /// </summary>
        /// <param name="degree"></param>
        /// <returns>(<see cref="float"/>, <see cref="float"/>) tuple containing the resulting X and Y values.</returns>
        internal static (float, float) DegreeToRadiansTrigonometricF(double degree)
        {
            return ((float, float))DegreeToRadiansTrigonometric(degree);
        }

        /// <summary>
        /// Given a <paramref name="degree"/>, calculates the <see cref="Math.Cos(double)"/> and <see cref="Math.Sin(double)"/> values.
        /// </summary>
        /// <param name="degree"></param>
        /// <returns>(<see cref="double"/>, <see cref="double"/>) tuple containing the resulting X and Y values.</returns>
        internal static (double, double) DegreeToRadiansTrigonometric(double degree)
        {
            double x = Math.Cos(DegreeToRadian(degree));
            double y = Math.Sin(DegreeToRadian(degree));
            return (x, y);
        }

        internal static T MaxBy<T, TComparable>(this IEnumerable<T> en, Func<T, TComparable> evaluate) where TComparable : IComparable<TComparable>
        {
            return en.Select(t => (value: t, eval: evaluate(t)))
                .Aggregate((max, next) => next.eval.CompareTo(max.eval) > 0 ? next : max).value;
        }

        internal static T MinBy<T, TComparable>(this IEnumerable<T> en, Func<T, TComparable> evaluate) where TComparable : IComparable<TComparable>
        {
            return en.Select(t => (value: t, eval: evaluate(t)))
                .Aggregate((max, next) => next.eval.CompareTo(max.eval) < 0 ? next : max).value;
        }

        internal static string ToHexString(byte[] bytes, int start, int end)
        {
            string res = "";
            for (int i = start; i < end; i++)
            {
                res += bytes[i].ToString("X2");
            }
            return res;
        }

        internal static bool IsSupportedStateChange(StateChange state)
        {
            return state != StateChange.Unknown && state != StateChange.ReplInfo && state != StateChange.StatReset && state != StateChange.APIDelayed && state != StateChange.Idle;
        }

        /*
        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }


        public static string FindPattern(string source, string regex)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            Match match = Regex.Match(source, regex);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }
        */
        public static Exception GetFinalException(Exception ex)
        {
            Exception final = ex;
            while (final.InnerException != null)
            {
                final = final.InnerException;
            }
            return final;
        }



        internal delegate bool ExtraRedirection(CombatItem evt, AgentItem from, AgentItem to);
        /// <summary>
        /// Method used to redirect a subset of events from redirectFrom to to
        /// </summary>
        /// <param name="combatData"></param>
        /// <param name="extensions"></param>
        /// <param name="agentData"></param>
        /// <param name="redirectFrom">AgentItem the events need to be redirected from</param>
        /// <param name="stateCopyFroms">AgentItems from where last known states (hp, position, etc) will be copied from</param>
        /// <param name="to">AgentItem the events need to be redirected to</param>
        /// <param name="copyPositionalDataFromAttackTarget">If true, "to" will get the positional data from attack targets, if possible</param>
        /// <param name="extraRedirections">function to handle special conditions, given event either src or dst matches from</param>
        internal static void RedirectEventsAndCopyPreviousStates(List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions, AgentData agentData, AgentItem redirectFrom, List<AgentItem> stateCopyFroms, AgentItem to, bool copyPositionalDataFromAttackTarget, ExtraRedirection extraRedirections = null)
        {
            // Redirect combat events
            foreach (CombatItem evt in combatData)
            {
                if (to.InAwareTimes(evt.Time))
                {
                    var srcMatchesAgent = evt.SrcMatchesAgent(redirectFrom, extensions);
                    var dstMatchesAgent = evt.DstMatchesAgent(redirectFrom, extensions);
                    if (extraRedirections != null && !extraRedirections(evt, redirectFrom, to))
                    {
                        continue;
                    }
                    if (srcMatchesAgent)
                    {
                        evt.OverrideSrcAgent(to.Agent);
                    }
                    if (dstMatchesAgent)
                    {
                        evt.OverrideDstAgent(to.Agent);
                    }
                }
            }
            // Copy attack targets
            var attackTargetAgents = new HashSet<AgentItem>();
            var attackTargets = combatData.Where(x => x.IsStateChange == StateChange.AttackTarget && x.DstMatchesAgent(redirectFrom)).ToList();
            var targetableOns = combatData.Where(x => x.IsStateChange == StateChange.Targetable && x.DstAgent == 1).ToList();
            foreach (CombatItem c in attackTargets)
            {
                var cExtra = new CombatItem(c);
                cExtra.OverrideTime(to.FirstAware);
                cExtra.OverrideDstAgent(to.Agent);
                combatData.Add(cExtra);
                AgentItem at = agentData.GetAgent(c.SrcAgent, c.Time);
                if (targetableOns.Any(x => x.SrcMatchesAgent(at)))
                {
                    attackTargetAgents.Add(at);
                }
            }
            // Copy states
            var toCopy = new List<CombatItem>();
            Func<CombatItem, bool> canCopyFromAgent = (evt) => stateCopyFroms.Any(x => evt.SrcMatchesAgent(x));
            var stateChangeCopyFromAgentConditions = new List<Func<CombatItem, bool>>()
            {
                (x) => x.IsStateChange == StateChange.BreakbarState,
                (x) => x.IsStateChange == StateChange.MaxHealthUpdate,
                (x) => x.IsStateChange == StateChange.HealthUpdate,
                (x) => x.IsStateChange == StateChange.BreakbarPercent,
                (x) => x.IsStateChange == StateChange.BarrierUpdate,
                (x) => (x.IsStateChange == StateChange.EnterCombat || x.IsStateChange == StateChange.ExitCombat),
                (x) => (x.IsStateChange == StateChange.Spawn || x.IsStateChange == StateChange.Despawn || x.IsStateChange == StateChange.ChangeDead || x.IsStateChange == StateChange.ChangeDown || x.IsStateChange == StateChange.ChangeUp),
            };
            if (!copyPositionalDataFromAttackTarget || !attackTargetAgents.Any())
            {
                stateChangeCopyFromAgentConditions.Add((x) => x.IsStateChange == StateChange.Position);
                stateChangeCopyFromAgentConditions.Add((x) => x.IsStateChange == StateChange.Rotation);
                stateChangeCopyFromAgentConditions.Add((x) => x.IsStateChange == StateChange.Velocity);
            }
            foreach (Func<CombatItem, bool> stateChangeCopyCondition in stateChangeCopyFromAgentConditions)
            {
                CombatItem stateToCopy = combatData.LastOrDefault(x => stateChangeCopyCondition(x) && canCopyFromAgent(x) && x.Time <= to.FirstAware);
                if (stateToCopy != null)
                {
                    toCopy.Add(stateToCopy);
                }
            }
            // Copy positional data from attack targets
            if (copyPositionalDataFromAttackTarget && attackTargetAgents.Any())
            {
                Func<CombatItem, bool> canCopyFromAttackTarget = (evt) => attackTargetAgents.Any(x => evt.SrcMatchesAgent(x));
                var stateChangeCopyFromAttackTargetConditions = new List<Func<CombatItem, bool>>()
                {
                    (x) => x.IsStateChange == StateChange.Position,
                    (x) => x.IsStateChange == StateChange.Rotation,
                    (x) => x.IsStateChange == StateChange.Velocity,
                };
                foreach (Func<CombatItem, bool> stateChangeCopyCondition in stateChangeCopyFromAttackTargetConditions)
                {
                    CombatItem stateToCopy = combatData.LastOrDefault(x => stateChangeCopyCondition(x) && canCopyFromAttackTarget(x) && x.Time <= to.FirstAware);
                    if (stateToCopy != null)
                    {
                        toCopy.Add(stateToCopy);
                    }
                }
            }
            foreach (CombatItem c in toCopy)
            {
                var cExtra = new CombatItem(c);
                cExtra.OverrideTime(to.FirstAware);
                cExtra.OverrideSrcAgent(to.Agent);
                combatData.Add(cExtra);
            }
            // Redirect NPC masters
            foreach (AgentItem ag in agentData.GetAgentByType(AgentItem.AgentType.NPC))
            {
                if (ag.Master == redirectFrom && to.InAwareTimes(ag.FirstAware))
                {
                    ag.SetMaster(to);
                }
            }
            // Redirect Gadget masters
            foreach (AgentItem ag in agentData.GetAgentByType(AgentItem.AgentType.Gadget))
            {
                if (ag.Master == redirectFrom && to.InAwareTimes(ag.FirstAware))
                {
                    ag.SetMaster(to);
                }
            }
        }

        /// <summary>
        /// Method used to redirect all events from redirectFrom to to
        /// </summary>
        /// <param name="combatData"></param>
        /// <param name="extensions"></param>
        /// <param name="agentData"></param>
        /// <param name="redirectFrom">AgentItem the events need to be redirected from</param>
        /// <param name="to">AgentItem the events need to be redirected to</param>
        /// <param name="extraRedirections">function to handle special conditions, given event either src or dst matches from</param>
        internal static void RedirectAllEvents(IReadOnlyList<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions, AgentData agentData, AgentItem redirectFrom, AgentItem to, ExtraRedirection extraRedirections = null)
        {
            // Redirect combat events
            foreach (CombatItem evt in combatData)
            {
                var srcMatchesAgent = evt.SrcMatchesAgent(redirectFrom, extensions);
                var dstMatchesAgent = evt.DstMatchesAgent(redirectFrom, extensions);
                if (!dstMatchesAgent && !srcMatchesAgent)
                {
                    continue;
                }
                if (extraRedirections != null && !extraRedirections(evt, redirectFrom, to))
                {
                    continue;
                }
                if (srcMatchesAgent)
                {
                    evt.OverrideSrcAgent(to.Agent);
                }
                if (dstMatchesAgent)
                {
                    evt.OverrideDstAgent(to.Agent);
                }
            }
            agentData.SwapMasters(redirectFrom, to);
        }

        /// <summary>
        /// Dictionary to find the <see cref="Spec"/> Specialization / Profession given a <see cref="string"/> as reference.
        /// </summary>
        private static IReadOnlyDictionary<string, Spec> ProfToSpecDictionary { get; set; } = new Dictionary<string, Spec>()
        {
            { "NPC", Spec.NPC },
            { "GDG", Spec.Gadget },
            { "Untamed", Spec.Untamed },
            { "Druid", Spec.Druid },
            { "Soulbeast", Spec.Soulbeast },
            { "Ranger", Spec.Ranger },
            { "Scrapper", Spec.Scrapper },
            { "Holosmith", Spec.Holosmith },
            { "Mechanist", Spec.Mechanist },
            { "Engineer", Spec.Engineer },
            { "Specter", Spec.Specter },
            { "Daredevil", Spec.Daredevil },
            { "Deadeye", Spec.Deadeye },
            { "Thief", Spec.Thief },
            { "Catalyst", Spec.Catalyst },
            { "Weaver", Spec.Weaver },
            { "Tempest", Spec.Tempest },
            { "Elementalist", Spec.Elementalist },
            { "Virtuoso", Spec.Virtuoso },
            { "Mirage", Spec.Mirage },
            { "Chronomancer", Spec.Chronomancer },
            { "Mesmer", Spec.Mesmer },
            { "Harbinger", Spec.Harbinger },
            { "Scourge", Spec.Scourge },
            { "Reaper", Spec.Reaper },
            { "Necromancer", Spec.Necromancer },
            { "Bladesworn", Spec.Bladesworn },
            { "Spellbreaker", Spec.Spellbreaker },
            { "Berserker", Spec.Berserker },
            { "Warrior", Spec.Warrior },
            { "Willbender", Spec.Willbender },
            { "Firebrand", Spec.Firebrand },
            { "Dragonhunter", Spec.Dragonhunter },
            { "Guardian", Spec.Guardian },
            { "Vindicator", Spec.Vindicator },
            { "Renegade", Spec.Renegade },
            { "Herald", Spec.Herald },
            { "Revenant", Spec.Revenant },
            { "", Spec.Unknown },
        };

        internal static Spec ProfToSpec(string prof)
        {
            return ProfToSpecDictionary.TryGetValue(prof, out Spec spec) ? spec : Spec.Unknown;
        }

        /// <summary>
        /// Dictionary to find the base <see cref="Spec"/> Profession given a <see cref="Spec"/> Elite Specialization.
        /// </summary>
        private static IReadOnlyDictionary<Spec, Spec> SpecToBaseProfDictionary { get; set; } = new Dictionary<Spec, Spec>()
        {
            { Spec.Untamed, Spec.Ranger },
            { Spec.Soulbeast, Spec.Ranger },
            { Spec.Druid, Spec.Ranger },
            { Spec.Ranger, Spec.Ranger },
            { Spec.Mechanist, Spec.Engineer },
            { Spec.Holosmith, Spec.Engineer },
            { Spec.Scrapper, Spec.Engineer },
            { Spec.Engineer, Spec.Engineer },
            { Spec.Specter, Spec.Thief },
            { Spec.Deadeye, Spec.Thief },
            { Spec.Daredevil, Spec.Thief },
            { Spec.Thief, Spec.Thief },
            { Spec.Catalyst, Spec.Elementalist },
            { Spec.Weaver, Spec.Elementalist },
            { Spec.Tempest, Spec.Elementalist },
            { Spec.Elementalist, Spec.Elementalist },
            { Spec.Virtuoso, Spec.Mesmer },
            { Spec.Mirage, Spec.Mesmer },
            { Spec.Chronomancer, Spec.Mesmer },
            { Spec.Mesmer, Spec.Mesmer },
            { Spec.Harbinger, Spec.Necromancer },
            { Spec.Scourge, Spec.Necromancer },
            { Spec.Reaper, Spec.Necromancer },
            { Spec.Necromancer, Spec.Necromancer },
            { Spec.Bladesworn, Spec.Warrior },
            { Spec.Spellbreaker, Spec.Warrior },
            { Spec.Berserker, Spec.Warrior },
            { Spec.Warrior, Spec.Warrior },
            { Spec.Willbender, Spec.Guardian },
            { Spec.Firebrand, Spec.Guardian },
            { Spec.Dragonhunter, Spec.Guardian },
            { Spec.Guardian, Spec.Guardian },
            { Spec.Vindicator, Spec.Revenant },
            { Spec.Renegade, Spec.Revenant },
            { Spec.Herald, Spec.Revenant },
            { Spec.Revenant, Spec.Revenant },
        };

        internal static Spec SpecToBaseSpec(Spec spec)
        {
            return SpecToBaseProfDictionary.TryGetValue(spec, out Spec prof) ? prof : spec;
        }

        /// <summary>
        /// Dictionary to find the <see cref="Source"/> given a specific <see cref="Spec"/>.
        /// </summary>
        private static IReadOnlyDictionary<Spec, List<Source>> SpecToSourcesDictionary { get; set; } = new Dictionary<Spec, List<Source>>()
        {
            { Spec.Untamed, new List<Source> { Source.Ranger, Source.Untamed } },
            { Spec.Soulbeast, new List<Source> { Source.Ranger, Source.Soulbeast } },
            { Spec.Druid, new List<Source> { Source.Ranger, Source.Druid } },
            { Spec.Ranger, new List<Source> { Source.Ranger } },
            { Spec.Mechanist, new List<Source> { Source.Engineer, Source.Mechanist } },
            { Spec.Holosmith, new List<Source> { Source.Engineer, Source.Holosmith } },
            { Spec.Scrapper, new List<Source> { Source.Engineer, Source.Scrapper } },
            { Spec.Engineer, new List<Source> { Source.Engineer } },
            { Spec.Specter, new List<Source> { Source.Thief, Source.Specter } },
            { Spec.Deadeye, new List<Source> { Source.Thief, Source.Deadeye } },
            { Spec.Daredevil, new List<Source> { Source.Thief, Source.Daredevil } },
            { Spec.Thief, new List<Source> { Source.Thief } },
            { Spec.Catalyst, new List<Source> { Source.Elementalist, Source.Catalyst } },
            { Spec.Weaver, new List<Source> { Source.Elementalist, Source.Weaver } },
            { Spec.Tempest, new List<Source> { Source.Elementalist, Source.Tempest } },
            { Spec.Elementalist, new List<Source> { Source.Elementalist } },
            { Spec.Virtuoso, new List<Source> { Source.Mesmer, Source.Virtuoso } },
            { Spec.Mirage, new List<Source> { Source.Mesmer, Source.Mirage } },
            { Spec.Chronomancer, new List<Source> { Source.Mesmer, Source.Chronomancer } },
            { Spec.Mesmer, new List<Source> { Source.Mesmer } },
            { Spec.Harbinger, new List<Source> { Source.Necromancer, Source.Harbinger } },
            { Spec.Scourge, new List<Source> { Source.Necromancer, Source.Scourge } },
            { Spec.Reaper, new List<Source> { Source.Necromancer, Source.Reaper } },
            { Spec.Necromancer, new List<Source> { Source.Necromancer } },
            { Spec.Bladesworn, new List<Source> { Source.Warrior, Source.Bladesworn } },
            { Spec.Spellbreaker, new List<Source> { Source.Warrior, Source.Spellbreaker } },
            { Spec.Berserker, new List<Source> { Source.Warrior, Source.Berserker } },
            { Spec.Warrior, new List<Source> { Source.Warrior } },
            { Spec.Willbender, new List<Source> { Source.Guardian, Source.Willbender } },
            { Spec.Firebrand, new List<Source> { Source.Guardian, Source.Firebrand } },
            { Spec.Dragonhunter, new List<Source> { Source.Guardian, Source.Dragonhunter } },
            { Spec.Guardian, new List<Source> { Source.Guardian } },
            { Spec.Vindicator, new List<Source> { Source.Revenant, Source.Vindicator } },
            { Spec.Renegade, new List<Source> { Source.Revenant, Source.Renegade } },
            { Spec.Herald, new List<Source> { Source.Revenant, Source.Herald } },
            { Spec.Revenant, new List<Source> { Source.Revenant } },
        };

        public static IReadOnlyList<Source> SpecToSources(Spec spec)
        {
            return SpecToSourcesDictionary.TryGetValue(spec, out List<Source> list) ? list : new List<Source> { };
        }

        internal static string GetHighResolutionProfIcon(Spec spec)
        {
            return ParserIcons.HighResProfIcons.TryGetValue(spec, out string icon) ? icon : ParserIcons.UnknownProfessionIcon;
        }

        internal static string GetProfIcon(Spec spec)
        {
            return ParserIcons.BaseResProfIcons.TryGetValue(spec, out string icon) ? icon : ParserIcons.UnknownProfessionIcon;
        }

        internal static string GetNPCIcon(int id)
        {
            if (id == 0)
            {
                return ParserIcons.UnknownNPCIcon;
            }

            TargetID target = GetTargetID(id);
            if (target != TargetID.Unknown)
            {
                return ParserIcons.TargetNPCIcons.TryGetValue(target, out string targetIcon) ? targetIcon : ParserIcons.GenericEnemyIcon;
            }
            TrashID trash = GetTrashID(id);
            if (trash != TrashID.Unknown)
            {
                return ParserIcons.TrashNPCIcons.TryGetValue(trash, out string trashIcon) ? trashIcon : ParserIcons.GenericEnemyIcon;
            }
            MinionID minion = GetMinionID(id);
            if (minion != MinionID.Unknown)
            {
                return ParserIcons.MinionNPCIcons.TryGetValue(minion, out string minionIcon) ? minionIcon : ParserIcons.GenericEnemyIcon;
            }

            return ParserIcons.GenericEnemyIcon;
        }

        public static IReadOnlyDictionary<BuffAttribute, string> BuffAttributesStrings { get; private set; } = new Dictionary<BuffAttribute, string>()
        {
            { BuffAttribute.Power, "Power" },
            { BuffAttribute.Precision, "Precision" },
            { BuffAttribute.Toughness, "Toughness" },
            { BuffAttribute.DefensePercent, "Defense" },
            { BuffAttribute.Vitality, "Vitality" },
            { BuffAttribute.VitalityPercent, "Vitality" },
            { BuffAttribute.Ferocity, "Ferocity" },
            { BuffAttribute.Healing, "Healing Power" },
            { BuffAttribute.Condition, "Condition Damage" },
            { BuffAttribute.Concentration, "Concentration" },
            { BuffAttribute.Expertise, "Expertise" },
            { BuffAttribute.FishingPower, "Fishing Power" },
            { BuffAttribute.Armor, "Armor" },
            { BuffAttribute.Agony, "Agony" },
            { BuffAttribute.StatInc, "Stat Increase" },
            { BuffAttribute.FlatInc, "Flat Increase" },
            { BuffAttribute.PhysInc, "Outgoing Strike Damage" },
            { BuffAttribute.CondInc, "Outgoing Condition Damage" },
            { BuffAttribute.SiphonInc, "Outgoing Life Leech Damage" },
            { BuffAttribute.SiphonRec, "Incoming Life Leech Damage" },
            { BuffAttribute.CondRec, "Incoming Condition Damage" },
            { BuffAttribute.CondRec2, "Incoming Condition Damage (Mult)" },
            { BuffAttribute.PhysRec, "Incoming Strike Damage" },
            { BuffAttribute.PhysRec2, "Incoming Strike Damage (Mult)" },
            { BuffAttribute.AttackSpeed, "Attack Speed" },
            { BuffAttribute.ConditionDurationInc, "Outgoing Condition Duration" },
            { BuffAttribute.BoonDurationInc, "Outgoing Boon Duration" },
            { BuffAttribute.DamageFormulaSquaredLevel, "Damage Formula" },
            { BuffAttribute.DamageFormula, "Damage Formula" },
            { BuffAttribute.GlancingBlow, "Glancing Blow" },
            { BuffAttribute.CriticalChance, "Critical Chance" },
            { BuffAttribute.StrikeDamageToHP, "Strike Damage to Health" },
            { BuffAttribute.ConditionDamageToHP, "Condition Damage to Health" },
            { BuffAttribute.SkillActivationDamageFormula, "Damage Formula on Skill Activation" },
            { BuffAttribute.MovementActivationDamageFormula, "Damage Formula based on Movement" },
            { BuffAttribute.EnduranceRegeneration, "Endurance Regeneration" },
            { BuffAttribute.HealingEffectivenessRec, "Incoming Healing Effectiveness" },
            { BuffAttribute.HealingEffectivenessRec2, "Incoming Healing Effectiveness" },
            { BuffAttribute.HealingEffectivenessConvInc, "Outgoing Healing Effectiveness" },
            { BuffAttribute.HealingEffectivenessFlatInc, "Outgoing Healing Effectiveness" },
            { BuffAttribute.HealingOutputFormula, "Healing Formula" },
            { BuffAttribute.ExperienceFromKills, "Experience From Kills" },
            { BuffAttribute.ExperienceFromAll, "Experience From All" },
            { BuffAttribute.GoldFind, "GoldFind" },
            { BuffAttribute.MovementSpeed, "Movement Speed" },
            { BuffAttribute.MovementSpeedStacking, "Movement Speed (Stacking)" },
            { BuffAttribute.MovementSpeedStacking2, "Movement Speed (Stacking)" },
            { BuffAttribute.MaximumHP, "Maximum Health" },
            { BuffAttribute.KarmaBonus, "Karma Bonus" },
            { BuffAttribute.SkillRechargeSpeedIncrease, "Skill Recharge Speed Increase" },
            { BuffAttribute.MagicFind, "Magic Find" },
            { BuffAttribute.WXP, "WXP" },
            { BuffAttribute.Unknown, "Unknown" },
        };

        public static IReadOnlyDictionary<BuffAttribute, string> BuffAttributesPercent { get; private set; } = new Dictionary<BuffAttribute, string>()
        {
            { BuffAttribute.FlatInc, "%" },
            { BuffAttribute.PhysInc, "%" },
            { BuffAttribute.CondInc, "%" },
            { BuffAttribute.CondRec, "%" },
            { BuffAttribute.CondRec2, "%" },
            { BuffAttribute.PhysRec, "%" },
            { BuffAttribute.PhysRec2, "%" },
            { BuffAttribute.AttackSpeed, "%" },
            { BuffAttribute.ConditionDurationInc, "%" },
            { BuffAttribute.BoonDurationInc, "%" },
            { BuffAttribute.GlancingBlow, "%" },
            { BuffAttribute.CriticalChance, "%" },
            { BuffAttribute.StrikeDamageToHP, "%" },
            { BuffAttribute.ConditionDamageToHP, "%" },
            { BuffAttribute.EnduranceRegeneration, "%" },
            { BuffAttribute.HealingEffectivenessRec, "%" },
            { BuffAttribute.HealingEffectivenessRec2, "%" },
            { BuffAttribute.SiphonInc, "%" },
            { BuffAttribute.SiphonRec, "%" },
            { BuffAttribute.HealingEffectivenessConvInc , "%" },
            { BuffAttribute.HealingEffectivenessFlatInc , "%" },
            { BuffAttribute.ExperienceFromKills, "%" },
            { BuffAttribute.ExperienceFromAll, "%" },
            { BuffAttribute.GoldFind, "%" },
            { BuffAttribute.MovementSpeed, "%" },
            { BuffAttribute.MovementSpeedStacking, "%" },
            { BuffAttribute.MovementSpeedStacking2, "%" },
            { BuffAttribute.MaximumHP, "%" },
            { BuffAttribute.KarmaBonus, "%" },
            { BuffAttribute.SkillRechargeSpeedIncrease, "%" },
            { BuffAttribute.MagicFind, "%" },
            { BuffAttribute.WXP, "%" },
            { BuffAttribute.DefensePercent, "%" },
            { BuffAttribute.VitalityPercent, "%" },
            { BuffAttribute.MovementActivationDamageFormula, " adds" },
            { BuffAttribute.SkillActivationDamageFormula, " replaces" },
            { BuffAttribute.Unknown, "Unknown" },
        };

        public static bool IsKnownMinionID(AgentItem minion, Spec spec)
        {
            if (minion.Type == AgentItem.AgentType.Gadget)
            {
                return false;
            }
            int id = minion.ID;
            bool res = ProfHelper.IsKnownMinionID(id);
            switch (spec)
            {
                //
                case Spec.Elementalist:
                case Spec.Tempest:
                case Spec.Weaver:
                case Spec.Catalyst:
                    res |= ElementalistHelper.IsKnownMinionID(id);
                    break;
                //
                case Spec.Necromancer:
                case Spec.Scourge:
                case Spec.Harbinger:
                    res |= NecromancerHelper.IsKnownMinionID(id);
                    break;
                case Spec.Reaper:
                    res |= NecromancerHelper.IsKnownMinionID(id);
                    res |= ReaperHelper.IsKnownMinionID(id);
                    break;
                //
                case Spec.Mesmer:
                    res |= MesmerHelper.IsKnownMinionID(id);
                    break;
                case Spec.Chronomancer:
                    res |= MesmerHelper.IsKnownMinionID(id);
                    res |= ChronomancerHelper.IsKnownMinionID(id);
                    break;
                case Spec.Mirage:
                    res |= MesmerHelper.IsKnownMinionID(id);
                    res |= MirageHelper.IsKnownMinionID(id);
                    break;
                case Spec.Virtuoso:
                    res |= MesmerHelper.IsKnownMinionID(id);
                    res |= VirtuosoHelper.IsKnownMinionID(id);
                    break;
                //
                case Spec.Thief:
                    res |= ThiefHelper.IsKnownMinionID(id);
                    break;
                case Spec.Daredevil:
                    res |= ThiefHelper.IsKnownMinionID(id);
                    res |= DaredevilHelper.IsKnownMinionID(id);
                    break;
                case Spec.Deadeye:
                    res |= ThiefHelper.IsKnownMinionID(id);
                    res |= DeadeyeHelper.IsKnownMinionID(id);
                    break;
                case Spec.Specter:
                    res |= ThiefHelper.IsKnownMinionID(id);
                    res |= SpecterHelper.IsKnownMinionID(id);
                    break;
                //
                case Spec.Engineer:
                case Spec.Holosmith:
                    res |= EngineerHelper.IsKnownMinionID(id);
                    break;
                case Spec.Scrapper:
                    res |= EngineerHelper.IsKnownMinionID(id);
                    res |= ScrapperHelper.IsKnownMinionID(id);
                    break;
                case Spec.Mechanist:
                    res |= EngineerHelper.IsKnownMinionID(id);
                    res |= MechanistHelper.IsKnownMinionID(id);
                    break;
                //
                case Spec.Ranger:
                case Spec.Druid:
                case Spec.Soulbeast:
                case Spec.Untamed:
                    res |= RangerHelper.IsKnownMinionID(id);
                    break;
                //
                case Spec.Revenant:
                case Spec.Herald:
                case Spec.Vindicator:
                    res |= RevenantHelper.IsKnownMinionID(id);
                    break;
                case Spec.Renegade:
                    res |= RevenantHelper.IsKnownMinionID(id);
                    res |= RenegadeHelper.IsKnownMinionID(id);
                    break;
                //
                case Spec.Guardian:
                case Spec.Dragonhunter:
                case Spec.Firebrand:
                case Spec.Willbender:
                    res |= GuardianHelper.IsKnownMinionID(id);
                    break;
            }
            return res;
        }
        public static void Add<TKey, TValue>(Dictionary<TKey, List<TValue>> dict, TKey key, TValue evt)
        {
            if (dict.TryGetValue(key, out List<TValue> list))
            {
                list.Add(evt);
            }
            else
            {
                dict[key] = new List<TValue>()
                {
                    evt
                };
            }
        }
        public static void Add<TKey, TValue>(Dictionary<TKey, HashSet<TValue>> dict, TKey key, TValue evt)
        {
            if (dict.TryGetValue(key, out HashSet<TValue> list))
            {
                list.Add(evt);
            }
            else
            {
                dict[key] = new HashSet<TValue>()
                {
                    evt
                };
            }
        }

        public static int IndexOf<T>(this IReadOnlyList<T> self, T elementToFind)
        {
            int i = 0;
            foreach (T element in self)
            {
                if (Equals(element, elementToFind))
                {
                    return i;
                }

                i++;
            }
            return -1;
        }
    }
}
