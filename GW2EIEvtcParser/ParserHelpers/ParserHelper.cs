using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser
{
    public static class ParserHelper
    {

        internal static readonly AgentItem _unknownAgent = new AgentItem();

        public const int CombatReplayPollingRate = 150;

        internal const int BuffDigit = 3;
        internal const int DamageModGainDigit = 3;
        internal const int AccelerationDigit = 3;
        internal const int CombatReplayDataDigit = 3;
        internal const int TimeDigit = 3;

        internal const long ServerDelayConstant = 10;
        internal const long BuffSimulatorDelayConstant = 15;
        internal const long BuffSimulatorStackActiveDelayConstant = 50;
        internal const long WeaponSwapDelayConstant = 75;
        internal const long MinimumInCombatDuration = 2200;

        internal static class GW2Builds
        {
            internal const ulong StartOfLife = ulong.MinValue;
            //
            internal const ulong May2018Balance = 88541;
            internal const ulong July2018Balance = 90455;
            internal const ulong August2018Balance = 92069;
            internal const ulong October2018Balance = 92715;
            internal const ulong December2018Balance = 94051;
            internal const ulong March2019Balance = 95535;
            internal const ulong April2019Balance = 96406;
            internal const ulong July2019Balance = 97950;
            internal const ulong October2019Balance = 99526;
            internal const ulong February2020Balance = 102321;
            internal const ulong May2021Balance = 115190;
            internal const ulong May2021BalanceHotFix = 115728;
            internal const ulong June2021Balance = 116210;
            internal const ulong EODBeta1 = 118697;
            internal const ulong EODBeta2 = 119939;
            internal const ulong EODBeta3 = 121168;
            internal const ulong EODBeta4 = 122479;
            //
            internal const ulong EndOfLife = ulong.MaxValue;
        }

        internal const int PhaseTimeLimit = 1000;


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

        // TODO: add life leech stuff
        public enum DamageType { All, Power, Strike, Condition, StrikeAndCondition, StrikeAndConditionAndLifeLeech };
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

        internal static Spec ProfToSpec(string prof)
        {
            switch (prof)
            {
                //
                case "NPC":
                    return Spec.NPC;
                case "GDG":
                    return Spec.Gadget;
                //
                case "Untamed":
                    return Spec.Untamed;
                case "Druid":
                    return Spec.Druid;
                case "Soulbeast":
                    return Spec.Soulbeast;
                case "Ranger":
                    return Spec.Ranger;
                //
                case "Scrapper":
                    return Spec.Scrapper;
                case "Holosmith":
                    return Spec.Holosmith;
                case "Mechanist":
                    return Spec.Mechanist;
                case "Engineer":
                    return Spec.Engineer;
                //
                case "Specter":
                    return Spec.Specter;
                case "Daredevil":
                    return Spec.Daredevil;
                case "Deadeye":
                    return Spec.Deadeye;
                case "Thief":
                    return Spec.Thief;
                //
                case "Catalyst":
                    return Spec.Catalyst;
                case "Weaver":
                    return Spec.Weaver;
                case "Tempest":
                    return Spec.Tempest;
                case "Elementalist":
                    return Spec.Elementalist;
                //
                case "Virtuoso":
                    return Spec.Virtuoso;
                case "Mirage":
                    return Spec.Mirage;
                case "Chronomancer":
                    return Spec.Chronomancer;
                case "Mesmer":
                    return Spec.Mesmer;
                //
                case "Harbinger":
                    return Spec.Harbinger;
                case "Scourge":
                    return Spec.Scourge;
                case "Reaper":
                    return Spec.Reaper;
                case "Necromancer":
                    return Spec.Necromancer;
                //
                case "Bladesworn":
                    return Spec.Bladesworn;
                case "Spellbreaker":
                    return Spec.Spellbreaker;
                case "Berserker":
                    return Spec.Berserker;
                case "Warrior":
                    return Spec.Warrior;
                //
                case "Willbender":
                    return Spec.Willbender;
                case "Firebrand":
                    return Spec.Firebrand;
                case "Dragonhunter":
                    return Spec.Dragonhunter;
                case "Guardian":
                    return Spec.Guardian;
                //
                case "Vindicator":
                    return Spec.Vindicator;
                case "Renegade":
                    return Spec.Renegade;
                case "Herald":
                    return Spec.Herald;
                case "Revenant":
                    return Spec.Revenant;
            }
            return Spec.Unknown;
        }

        internal static Spec SpecToBaseSpec(Spec spec)
        {
            switch (spec)
            {
                case Spec.Druid:
                case Spec.Soulbeast:
                case Spec.Untamed:
                case Spec.Ranger:
                    return Spec.Ranger;
                //
                case Spec.Scrapper:
                case Spec.Holosmith:
                case Spec.Mechanist:
                case Spec.Engineer:
                    return Spec.Engineer;
                //
                case Spec.Specter:
                case Spec.Daredevil:
                case Spec.Deadeye:
                case Spec.Thief:
                    return Spec.Thief;
                //
                case Spec.Catalyst:
                case Spec.Weaver:
                case Spec.Tempest:
                case Spec.Elementalist:
                    return Spec.Elementalist;
                //
                case Spec.Virtuoso:
                case Spec.Mirage:
                case Spec.Chronomancer:
                case Spec.Mesmer:
                    return Spec.Mesmer;
                //
                case Spec.Harbinger:
                case Spec.Scourge:
                case Spec.Reaper:
                case Spec.Necromancer:
                    return Spec.Necromancer;
                //
                case Spec.Bladesworn:
                case Spec.Spellbreaker:
                case Spec.Berserker:
                case Spec.Warrior:
                    return Spec.Warrior;
                //
                case Spec.Willbender:
                case Spec.Firebrand:
                case Spec.Dragonhunter:
                case Spec.Guardian:
                    return Spec.Guardian;
                //
                case Spec.Vindicator:
                case Spec.Renegade:
                case Spec.Herald:
                case Spec.Revenant:
                    return Spec.Revenant;
            }
            return spec;
        }

        public static IReadOnlyList<Source> SpecToSources(Spec spec)
        {
            switch (spec)
            {
                case Spec.Untamed:
                    return new List<Source> { Source.Ranger, Source.Untamed };
                case Spec.Druid:
                    return new List<Source> { Source.Ranger, Source.Druid };
                case Spec.Soulbeast:
                    return new List<Source> { Source.Ranger, Source.Soulbeast };
                case Spec.Ranger:
                    return new List<Source> { Source.Ranger };
                //
                case Spec.Scrapper:
                    return new List<Source> { Source.Engineer, Source.Scrapper };
                case Spec.Holosmith:
                    return new List<Source> { Source.Engineer, Source.Holosmith };
                case Spec.Mechanist:
                    return new List<Source> { Source.Engineer, Source.Mechanist };
                case Spec.Engineer:
                    return new List<Source> { Source.Engineer };
                //
                case Spec.Specter:
                    return new List<Source> { Source.Thief, Source.Specter };
                case Spec.Deadeye:
                    return new List<Source> { Source.Thief, Source.Deadeye };
                case Spec.Daredevil:
                    return new List<Source> { Source.Thief, Source.Daredevil };
                case Spec.Thief:
                    return new List<Source> { Source.Thief };
                //
                case Spec.Catalyst:
                    return new List<Source> { Source.Elementalist, Source.Catalyst };
                case Spec.Weaver:
                    return new List<Source> { Source.Elementalist, Source.Weaver };
                case Spec.Tempest:
                    return new List<Source> { Source.Elementalist, Source.Tempest };
                case Spec.Elementalist:
                    return new List<Source> { Source.Elementalist };
                //
                case Spec.Virtuoso:
                    return new List<Source> { Source.Mesmer, Source.Virtuoso };
                case Spec.Mirage:
                    return new List<Source> { Source.Mesmer, Source.Mirage };
                case Spec.Chronomancer:
                    return new List<Source> { Source.Mesmer, Source.Chronomancer };
                case Spec.Mesmer:
                    return new List<Source> { Source.Mesmer };
                //
                case Spec.Harbinger:
                    return new List<Source> { Source.Necromancer, Source.Harbinger };
                case Spec.Scourge:
                    return new List<Source> { Source.Necromancer, Source.Scourge };
                case Spec.Reaper:
                    return new List<Source> { Source.Necromancer, Source.Reaper };
                case Spec.Necromancer:
                    return new List<Source> { Source.Necromancer };
                //
                case Spec.Bladesworn:
                    return new List<Source> { Source.Warrior, Source.Bladesworn };
                case Spec.Spellbreaker:
                    return new List<Source> { Source.Warrior, Source.Spellbreaker };
                case Spec.Berserker:
                    return new List<Source> { Source.Warrior, Source.Berserker };
                case Spec.Warrior:
                    return new List<Source> { Source.Warrior };
                //
                case Spec.Willbender:
                    return new List<Source> { Source.Guardian, Source.Willbender };
                case Spec.Firebrand:
                    return new List<Source> { Source.Guardian, Source.Firebrand };
                case Spec.Dragonhunter:
                    return new List<Source> { Source.Guardian, Source.Dragonhunter };
                case Spec.Guardian:
                    return new List<Source> { Source.Guardian };
                //
                case Spec.Vindicator:
                    return new List<Source> { Source.Revenant, Source.Vindicator };
                case Spec.Renegade:
                    return new List<Source> { Source.Revenant, Source.Renegade };
                case Spec.Herald:
                    return new List<Source> { Source.Revenant, Source.Herald };
                case Spec.Revenant:
                    return new List<Source> { Source.Revenant };
            }
            return new List<Source> { };
        }

        internal static string GetHighResolutionProfIcon(Spec spec)
        {
            switch (spec)
            {
                case Spec.Warrior:
                    return "https://wiki.guildwars2.com/images/d/db/Warrior_tango_icon_200px.png";
                case Spec.Berserker:
                    return "https://wiki.guildwars2.com/images/8/80/Berserker_tango_icon_200px.png";
                case Spec.Spellbreaker:
                    return "https://wiki.guildwars2.com/images/7/78/Spellbreaker_tango_icon_200px.png";
                case Spec.Bladesworn:
                    return "https://wiki.guildwars2.com/images/c/c1/Bladesworn_tango_icon_200px.png";
                //
                case Spec.Guardian:
                    return "https://wiki.guildwars2.com/images/6/6c/Guardian_tango_icon_200px.png";
                case Spec.Dragonhunter:
                    return "https://wiki.guildwars2.com/images/1/1f/Dragonhunter_tango_icon_200px.png";
                case Spec.Firebrand:
                    return "https://wiki.guildwars2.com/images/7/73/Firebrand_tango_icon_200px.png";
                case Spec.Willbender:
                    return "https://wiki.guildwars2.com/images/5/57/Willbender_tango_icon_200px.png";
                //
                case Spec.Revenant:
                    return "https://wiki.guildwars2.com/images/a/a8/Revenant_tango_icon_200px.png";
                case Spec.Herald:
                    return "https://wiki.guildwars2.com/images/c/c7/Herald_tango_icon_200px.png";
                case Spec.Renegade:
                    return "https://wiki.guildwars2.com/images/b/bc/Renegade_tango_icon_200px.png";
                case Spec.Vindicator:
                    return "https://wiki.guildwars2.com/images/f/f0/Vindicator_tango_icon_200px.png";
                //
                case Spec.Engineer:
                    return "https://wiki.guildwars2.com/images/2/2f/Engineer_tango_icon_200px.png";
                case Spec.Scrapper:
                    return "https://wiki.guildwars2.com/images/3/3a/Scrapper_tango_icon_200px.png";
                case Spec.Holosmith:
                    return "https://wiki.guildwars2.com/images/a/ae/Holosmith_tango_icon_200px.png";
                case Spec.Mechanist:
                    return "https://wiki.guildwars2.com/images/8/8a/Mechanist_tango_icon_200px.png";
                //
                case Spec.Ranger:
                    return "https://wiki.guildwars2.com/images/5/51/Ranger_tango_icon_200px.png";
                case Spec.Druid:
                    return "https://wiki.guildwars2.com/images/6/6d/Druid_tango_icon_200px.png";
                case Spec.Soulbeast:
                    return "https://wiki.guildwars2.com/images/f/f6/Soulbeast_tango_icon_200px.png";
                case Spec.Untamed:
                    return "https://wiki.guildwars2.com/images/3/33/Untamed_tango_icon_200px.png";
                //
                case Spec.Thief:
                    return "https://wiki.guildwars2.com/images/1/19/Thief_tango_icon_200px.png";
                case Spec.Daredevil:
                    return "https://wiki.guildwars2.com/images/c/ca/Daredevil_tango_icon_200px.png";
                case Spec.Deadeye:
                    return "https://wiki.guildwars2.com/images/b/b0/Deadeye_tango_icon_200px.png";
                case Spec.Specter:
                    return "https://i.imgur.com/UbvyFSt.png";
                //
                case Spec.Elementalist:
                    return "https://wiki.guildwars2.com/images/a/a0/Elementalist_tango_icon_200px.png";
                case Spec.Tempest:
                    return "https://wiki.guildwars2.com/images/9/90/Tempest_tango_icon_200px.png";
                case Spec.Weaver:
                    return "https://wiki.guildwars2.com/images/3/31/Weaver_tango_icon_200px.png";
                case Spec.Catalyst:
                    return "https://i.imgur.com/UbvyFSt.png";
                //
                case Spec.Mesmer:
                    return "https://wiki.guildwars2.com/images/7/73/Mesmer_tango_icon_200px.png";
                case Spec.Chronomancer:
                    return "https://wiki.guildwars2.com/images/8/8b/Chronomancer_tango_icon_200px.png";
                case Spec.Mirage:
                    return "https://wiki.guildwars2.com/images/a/a9/Mirage_tango_icon_200px.png";
                case Spec.Virtuoso:
                    return "https://wiki.guildwars2.com/images/c/cd/Virtuoso_tango_icon_200px.png";
                //
                case Spec.Necromancer:
                    return "https://wiki.guildwars2.com/images/c/cd/Necromancer_tango_icon_200px.png";
                case Spec.Reaper:
                    return "https://wiki.guildwars2.com/images/9/95/Reaper_tango_icon_200px.png";
                case Spec.Scourge:
                    return "https://wiki.guildwars2.com/images/8/8a/Scourge_tango_icon_200px.png";
                case Spec.Harbinger:
                    return "https://wiki.guildwars2.com/images/b/b3/Harbinger_tango_icon_200px.png";
            }
            return "https://i.imgur.com/UbvyFSt.png";
        }

        internal static string GetProfIcon(Spec spec)
        {
            switch (spec)
            {
                case Spec.Warrior:
                    return "https://wiki.guildwars2.com/images/4/43/Warrior_tango_icon_20px.png";
                case Spec.Berserker:
                    return "https://wiki.guildwars2.com/images/d/da/Berserker_tango_icon_20px.png";
                case Spec.Spellbreaker:
                    return "https://wiki.guildwars2.com/images/e/ed/Spellbreaker_tango_icon_20px.png";
                case Spec.Bladesworn:
                    return "https://wiki.guildwars2.com/images/thumb/c/c1/Bladesworn_tango_icon_200px.png/20px-Bladesworn_tango_icon_200px.png";
                //
                case Spec.Guardian:
                    return "https://wiki.guildwars2.com/images/8/8c/Guardian_tango_icon_20px.png";
                case Spec.Dragonhunter:
                    return "https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png";
                case Spec.Firebrand:
                    return "https://wiki.guildwars2.com/images/0/02/Firebrand_tango_icon_20px.png";
                case Spec.Willbender:
                    return "https://wiki.guildwars2.com/images/3/3a/Willbender_tango_icon_20px.png";
                //
                case Spec.Revenant:
                    return "https://wiki.guildwars2.com/images/b/b5/Revenant_tango_icon_20px.png";
                case Spec.Herald:
                    return "https://wiki.guildwars2.com/images/6/67/Herald_tango_icon_20px.png";
                case Spec.Renegade:
                    return "https://wiki.guildwars2.com/images/7/7c/Renegade_tango_icon_20px.png";
                case Spec.Vindicator:
                    return "https://wiki.guildwars2.com/images/5/5a/Vindicator_tango_icon_20px.png";
                //
                case Spec.Engineer:
                    return "https://wiki.guildwars2.com/images/2/27/Engineer_tango_icon_20px.png";
                case Spec.Scrapper:
                    return "https://wiki.guildwars2.com/images/3/3a/Scrapper_tango_icon_200px.png";
                case Spec.Holosmith:
                    return "https://wiki.guildwars2.com/images/2/28/Holosmith_tango_icon_20px.png";
                case Spec.Mechanist:
                    return "https://wiki.guildwars2.com/images/thumb/8/8a/Mechanist_tango_icon_200px.png/20px-Mechanist_tango_icon_200px.png";
                //
                case Spec.Ranger:
                    return "https://wiki.guildwars2.com/images/4/43/Ranger_tango_icon_20px.png";
                case Spec.Druid:
                    return "https://wiki.guildwars2.com/images/d/d2/Druid_tango_icon_20px.png";
                case Spec.Soulbeast:
                    return "https://wiki.guildwars2.com/images/7/7c/Soulbeast_tango_icon_20px.png";
                case Spec.Untamed:
                    return "https://wiki.guildwars2.com/images/thumb/3/33/Untamed_tango_icon_200px.png/20px-Untamed_tango_icon_200px.png";
                //
                case Spec.Thief:
                    return "https://wiki.guildwars2.com/images/7/7a/Thief_tango_icon_20px.png";
                case Spec.Daredevil:
                    return "https://wiki.guildwars2.com/images/e/e1/Daredevil_tango_icon_20px.png";
                case Spec.Deadeye:
                    return "https://wiki.guildwars2.com/images/c/c9/Deadeye_tango_icon_20px.png";
                case Spec.Specter:
                    return "https://i.imgur.com/UbvyFSt.png";
                //
                case Spec.Elementalist:
                    return "https://wiki.guildwars2.com/images/a/aa/Elementalist_tango_icon_20px.png";
                case Spec.Tempest:
                    return "https://wiki.guildwars2.com/images/4/4a/Tempest_tango_icon_20px.png";
                case Spec.Weaver:
                    return "https://wiki.guildwars2.com/images/f/fc/Weaver_tango_icon_20px.png";
                case Spec.Catalyst:
                    return "https://i.imgur.com/UbvyFSt.png";
                //
                case Spec.Mesmer:
                    return "https://wiki.guildwars2.com/images/6/60/Mesmer_tango_icon_20px.png";
                case Spec.Chronomancer:
                    return "https://wiki.guildwars2.com/images/f/f4/Chronomancer_tango_icon_20px.png";
                case Spec.Mirage:
                    return "https://wiki.guildwars2.com/images/d/df/Mirage_tango_icon_20px.png";
                case Spec.Virtuoso:
                    return "https://wiki.guildwars2.com/images/6/62/Virtuoso_tango_icon_20px.png";
                //
                case Spec.Necromancer:
                    return "https://wiki.guildwars2.com/images/4/43/Necromancer_tango_icon_20px.png";
                case Spec.Reaper:
                    return "https://wiki.guildwars2.com/images/1/11/Reaper_tango_icon_20px.png";
                case Spec.Scourge:
                    return "https://wiki.guildwars2.com/images/0/06/Scourge_tango_icon_20px.png";
                case Spec.Harbinger:
                    return "https://wiki.guildwars2.com/images/7/7f/Harbinger_tango_icon_20px.png";
            }
            return "https://i.imgur.com/UbvyFSt.png";
        }

        internal static string GetNPCIcon(int id)
        {
            switch (ArcDPSEnums.GetTargetID(id))
            {
                case ArcDPSEnums.TargetID.WorldVersusWorld:
                    return "https://wiki.guildwars2.com/images/d/db/PvP_Server_Browser_%28map_icon%29.png";
                case ArcDPSEnums.TargetID.ValeGuardian:
                    return "https://i.imgur.com/MIpP5pK.png";
                case ArcDPSEnums.TargetID.Gorseval:
                    return "https://i.imgur.com/5hmMq12.png";
                case ArcDPSEnums.TargetID.Sabetha:
                    return "https://i.imgur.com/UqbFp9S.png";
                case ArcDPSEnums.TargetID.Slothasor:
                    return "https://i.imgur.com/h1xH3ER.png";
                case ArcDPSEnums.TargetID.Berg:
                    return "https://i.imgur.com/tLMXqL7.png";
                case ArcDPSEnums.TargetID.Narella:
                    return "https://i.imgur.com/FwMCoR0.png";
                case ArcDPSEnums.TargetID.Zane:
                    return "https://i.imgur.com/tkPWMST.png";
                case ArcDPSEnums.TargetID.Matthias:
                    return "https://i.imgur.com/3uMMmTS.png";
                case ArcDPSEnums.TargetID.KeepConstruct:
                    return "https://i.imgur.com/Kq0kL07.png";
                case ArcDPSEnums.TargetID.Xera:
                    return "https://i.imgur.com/lYwJEyV.png";
                case ArcDPSEnums.TargetID.Cairn:
                    return "https://i.imgur.com/gQY37Tf.png";
                case ArcDPSEnums.TargetID.MursaatOverseer:
                    return "https://i.imgur.com/5LNiw4Y.png";
                case ArcDPSEnums.TargetID.Samarog:
                    return "https://i.imgur.com/MPQhKfM.png";
                case ArcDPSEnums.TargetID.Deimos:
                    return "https://i.imgur.com/mWfxBaO.png";
                case ArcDPSEnums.TargetID.SoullessHorror:
                case ArcDPSEnums.TargetID.Desmina:
                    return "https://i.imgur.com/jAiRplg.png";
                case ArcDPSEnums.TargetID.BrokenKing:
                    return "https://i.imgur.com/FNgUmvL.png";
                case ArcDPSEnums.TargetID.SoulEater:
                    return "https://i.imgur.com/Sd6Az8M.png";
                case ArcDPSEnums.TargetID.EyeOfFate:
                case ArcDPSEnums.TargetID.EyeOfJudgement:
                    return "https://i.imgur.com/kAgdoa5.png";
                case ArcDPSEnums.TargetID.Dhuum:
                    return "https://i.imgur.com/RKaDon5.png";
                case ArcDPSEnums.TargetID.ConjuredAmalgamate:
                    return "https://i.imgur.com/C23rYTl.png";
                case ArcDPSEnums.TargetID.CALeftArm:
                    return "https://i.imgur.com/qrkQvEY.png";
                case ArcDPSEnums.TargetID.CARightArm:
                    return "https://i.imgur.com/MVwjtH7.png";
                case ArcDPSEnums.TargetID.Kenut:
                    return "https://i.imgur.com/6yq45Cc.png";
                case ArcDPSEnums.TargetID.Nikare:
                    return "https://i.imgur.com/TLykcrJ.png";
                case ArcDPSEnums.TargetID.Qadim:
                    return "https://i.imgur.com/IfoHTHT.png";
                case ArcDPSEnums.TargetID.Freezie:
                    return "https://wiki.guildwars2.com/images/d/d9/Mini_Freezie.png";
                case ArcDPSEnums.TargetID.Adina:
                    return "https://i.imgur.com/or3m1yb.png";
                case ArcDPSEnums.TargetID.Sabir:
                    return "https://i.imgur.com/Q4WUXqw.png";
                case ArcDPSEnums.TargetID.PeerlessQadim:
                    return "https://i.imgur.com/47uePpb.png";
                case ArcDPSEnums.TargetID.IcebroodConstruct:
                case ArcDPSEnums.TargetID.IcebroodConstructFraenir:
                    return "https://i.imgur.com/dpaZFa5.png";
                case ArcDPSEnums.TargetID.ClawOfTheFallen:
                    return "https://i.imgur.com/HF85QpV.png";
                case ArcDPSEnums.TargetID.VoiceOfTheFallen:
                    return "https://i.imgur.com/BdTGXMU.png";
                case ArcDPSEnums.TargetID.VoiceAndClaw:
                    return "https://i.imgur.com/V1rJBnq.png";
                case ArcDPSEnums.TargetID.FraenirOfJormag:
                    return "https://i.imgur.com/MxudnKp.png";
                case ArcDPSEnums.TargetID.Boneskinner:
                    return "https://i.imgur.com/7HPdKDQ.png";
                case ArcDPSEnums.TargetID.WhisperOfJormag:
                    return "https://i.imgur.com/lu9ZLVq.png";
                case ArcDPSEnums.TargetID.VariniaStormsounder:
                    return "https://i.imgur.com/2o8TtiM.png";
                case ArcDPSEnums.TargetID.MAMA:
                    return "https://i.imgur.com/1h7HOII.png";
                case ArcDPSEnums.TargetID.Siax:
                    return "https://i.imgur.com/5C60cQb.png";
                case ArcDPSEnums.TargetID.Ensolyss:
                    return "https://i.imgur.com/GUTNuyP.png";
                case ArcDPSEnums.TargetID.Skorvald:
                    return "https://i.imgur.com/IOPAHRE.png";
                case ArcDPSEnums.TargetID.Artsariiv:
                    return "https://wiki.guildwars2.com/images/b/b4/Artsariiv.jpg";
                case ArcDPSEnums.TargetID.Arkk:
                    return "https://i.imgur.com/u6vv8cW.png";
                case ArcDPSEnums.TargetID.AiKeeperOfThePeak:
                    return "https://i.imgur.com/eCXjoAS.png";
                case ArcDPSEnums.TargetID.AiKeeperOfThePeak2:
                    return "https://i.imgur.com/I8nwhAw.png";
                case ArcDPSEnums.TargetID.LGolem:
                case ArcDPSEnums.TargetID.VitalGolem:
                    return "https://wiki.guildwars2.com/images/4/47/Mini_Baron_von_Scrufflebutt.png";
                case ArcDPSEnums.TargetID.AvgGolem:
                    return "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                case ArcDPSEnums.TargetID.StdGolem:
                    return "https://wiki.guildwars2.com/images/8/8f/Mini_Professor_Mew.png";
                case ArcDPSEnums.TargetID.MassiveGolem10M:
                case ArcDPSEnums.TargetID.MassiveGolem4M:
                case ArcDPSEnums.TargetID.MassiveGolem1M:
                    return "https://wiki.guildwars2.com/images/3/33/Mini_Snuggles.png";
                case ArcDPSEnums.TargetID.PowerGolem:
                case ArcDPSEnums.TargetID.ConditionGolem:
                case ArcDPSEnums.TargetID.MedGolem:
                    return "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                //case ArcDPSEnums.TargetID.DummyTarget:
                    //return "https://i.imgur.com/ZBm5Uga.png";
            }
            switch (ArcDPSEnums.GetTrashID(id))
            {
                case ArcDPSEnums.TrashID.Spirit:
                case ArcDPSEnums.TrashID.Spirit2:
                case ArcDPSEnums.TrashID.ChargedSoul:
                case ArcDPSEnums.TrashID.HollowedBomber:
                case ArcDPSEnums.TrashID.GuiltDemon:
                case ArcDPSEnums.TrashID.DoubtDemon:
                    return "https://i.imgur.com/sHmksvO.png";
                case ArcDPSEnums.TrashID.Saul:
                    return "https://i.imgur.com/ck2IsoS.png";
                case ArcDPSEnums.TrashID.GamblerClones:
                    return "https://i.imgur.com/zMsBWEx.png";
                case ArcDPSEnums.TrashID.BloodstoneFragment:
                case ArcDPSEnums.TrashID.ChargedBloodstone:
                    return "https://i.imgur.com/PZ2VNAN.png";
                case ArcDPSEnums.TrashID.BloodstoneShard:
                case ArcDPSEnums.TrashID.GamblerReal:
                    return "https://i.imgur.com/J6oMITN.png";
                case ArcDPSEnums.TrashID.Pride:
                    return "https://i.imgur.com/ePTXx23.png";
                case ArcDPSEnums.TrashID.OilSlick:
                case ArcDPSEnums.TrashID.Oil:
                    return "https://i.imgur.com/R26VgEr.png";
                case ArcDPSEnums.TrashID.Tear:
                    return "https://i.imgur.com/N9seps0.png";
                case ArcDPSEnums.TrashID.Gambler:
                case ArcDPSEnums.TrashID.Drunkard:
                case ArcDPSEnums.TrashID.Thief:
                    return "https://i.imgur.com/vINeVU6.png";
                case ArcDPSEnums.TrashID.TormentedDead:
                case ArcDPSEnums.TrashID.Messenger:
                    return "https://i.imgur.com/1J2BTFg.png";
                case ArcDPSEnums.TrashID.Enforcer:
                    return "https://i.imgur.com/elHjamF.png";
                case ArcDPSEnums.TrashID.Echo:
                    return "https://i.imgur.com/kcN9ECn.png";
                case ArcDPSEnums.TrashID.Core:
                case ArcDPSEnums.TrashID.ExquisiteConjunction:
                    return "https://i.imgur.com/yI34iqw.png";
                case ArcDPSEnums.TrashID.Jessica:
                case ArcDPSEnums.TrashID.Olson:
                case ArcDPSEnums.TrashID.Engul:
                case ArcDPSEnums.TrashID.Faerla:
                case ArcDPSEnums.TrashID.Caulle:
                case ArcDPSEnums.TrashID.Henley:
                case ArcDPSEnums.TrashID.Galletta:
                case ArcDPSEnums.TrashID.Ianim:
                    return "https://i.imgur.com/qeYT1Bf.png";
                case ArcDPSEnums.TrashID.InsidiousProjection:
                    return "https://i.imgur.com/9EdItBS.png";
                case ArcDPSEnums.TrashID.EnergyOrb:
                    return "https://i.postimg.cc/NMNvyts0/Power-Ball.png";
                case ArcDPSEnums.TrashID.UnstableLeyRift:
                    return "https://i.imgur.com/YXM3igs.png";
                case ArcDPSEnums.TrashID.RadiantPhantasm:
                    return "https://i.imgur.com/O5VWLyY.png";
                case ArcDPSEnums.TrashID.CrimsonPhantasm:
                    return "https://i.imgur.com/zP7Bvb4.png";
                case ArcDPSEnums.TrashID.Storm:
                    return "https://i.imgur.com/9XtNPdw.png";
                case ArcDPSEnums.TrashID.IcePatch:
                    return "https://i.imgur.com/yxKJ5Yc.png";
                case ArcDPSEnums.TrashID.BanditSaboteur:
                    return "https://i.imgur.com/jUKMEbD.png";
                case ArcDPSEnums.TrashID.NarellaTornado:
                case ArcDPSEnums.TrashID.Tornado:
                    return "https://i.imgur.com/e10lZMa.png";
                case ArcDPSEnums.TrashID.Jade:
                    return "https://i.imgur.com/ivtzbSP.png";
                case ArcDPSEnums.TrashID.Zommoros:
                    return "https://i.imgur.com/BxbsRCI.png";
                case ArcDPSEnums.TrashID.AncientInvokedHydra:
                    return "https://i.imgur.com/YABLiBz.png";
                case ArcDPSEnums.TrashID.IcebornHydra:
                    return "https://i.imgur.com/LoYMBRU.png";
                case ArcDPSEnums.TrashID.IceElemental:
                    return "https://i.imgur.com/pEkBeNp.png";
                case ArcDPSEnums.TrashID.WyvernMatriarch:
                    return "https://i.imgur.com/kLKLSfv.png";
                case ArcDPSEnums.TrashID.WyvernPatriarch:
                    return "https://i.imgur.com/vjjNSpI.png";
                case ArcDPSEnums.TrashID.ApocalypseBringer:
                    return "https://i.imgur.com/0LGKCn2.png";
                case ArcDPSEnums.TrashID.ConjuredGreatsword:
                    return "https://i.imgur.com/vHka0QN.png";
                case ArcDPSEnums.TrashID.ConjuredPlayerSword:
                    return "https://wiki.guildwars2.com/images/0/07/Crimson_Antique_Blade.png";
                case ArcDPSEnums.TrashID.ConjuredShield:
                    return "https://i.imgur.com/wUiI19S.png";
                case ArcDPSEnums.TrashID.GreaterMagmaElemental1:
                case ArcDPSEnums.TrashID.GreaterMagmaElemental2:
                    return "https://i.imgur.com/sr146T6.png";
                case ArcDPSEnums.TrashID.LavaElemental1:
                case ArcDPSEnums.TrashID.LavaElemental2:
                    return "https://i.imgur.com/mydwiYy.png";
                case ArcDPSEnums.TrashID.PyreGuardian:
                case ArcDPSEnums.TrashID.SmallKillerTornado:
                case ArcDPSEnums.TrashID.BigKillerTornado:
                    return "https://i.imgur.com/6zNPTUw.png";
                case ArcDPSEnums.TrashID.QadimLamp:
                    return "https://i.imgur.com/89Kjv0N.png";
                case ArcDPSEnums.TrashID.PyreGuardianRetal:
                    return "https://i.imgur.com/WC6LRkO.png";
                case ArcDPSEnums.TrashID.PyreGuardianResolution:
                    return "https://i.imgur.com/26rY9IM.png";
                case ArcDPSEnums.TrashID.PyreGuardianStab:
                    return "https://i.imgur.com/ISa0urR.png";
                case ArcDPSEnums.TrashID.PyreGuardianProtect:
                    return "https://i.imgur.com/jLW7rpV.png";
                case ArcDPSEnums.TrashID.ReaperofFlesh:
                    return "https://i.imgur.com/Notctbt.png";
                case ArcDPSEnums.TrashID.Kernan:
                    return "https://i.imgur.com/WABRQya.png";
                case ArcDPSEnums.TrashID.Knuckles:
                    return "https://i.imgur.com/m1y8nJE.png";
                case ArcDPSEnums.TrashID.Karde:
                    return "https://i.imgur.com/3UGyosm.png";
                case ArcDPSEnums.TrashID.Rigom:
                    return "https://i.imgur.com/REcGMBe.png";
                case ArcDPSEnums.TrashID.Guldhem:
                    return "https://i.imgur.com/xa7Fefn.png";
                case ArcDPSEnums.TrashID.Scythe:
                    return "https://i.imgur.com/INCGLIK.png";
                case ArcDPSEnums.TrashID.BanditBombardier:
                case ArcDPSEnums.TrashID.SurgingSoul:
                case ArcDPSEnums.TrashID.MazeMinotaur:
                case ArcDPSEnums.TrashID.Enervator:
                case ArcDPSEnums.TrashID.WhisperEcho:
                case ArcDPSEnums.TrashID.CharrTank:
                case ArcDPSEnums.TrashID.PropagandaBallon:
                case ArcDPSEnums.TrashID.FearDemon:
                case ArcDPSEnums.TrashID.SorrowDemon1:
                case ArcDPSEnums.TrashID.SorrowDemon2:
                case ArcDPSEnums.TrashID.SorrowDemon3:
                case ArcDPSEnums.TrashID.SorrowDemon4:
                case ArcDPSEnums.TrashID.SorrowDemon5:
                    return "https://i.imgur.com/k79t7ZA.png";
                case ArcDPSEnums.TrashID.HandOfErosion:
                case ArcDPSEnums.TrashID.HandOfEruption:
                    return "https://i.imgur.com/reGQHhr.png";
                case ArcDPSEnums.TrashID.VoltaicWisp:
                    return "https://i.imgur.com/C1mvNGZ.png";
                case ArcDPSEnums.TrashID.ParalyzingWisp:
                    return "https://i.imgur.com/YBl8Pqo.png";
                case ArcDPSEnums.TrashID.Pylon2:
                    return "https://i.imgur.com/b33vAEQ.png";
                case ArcDPSEnums.TrashID.EntropicDistortion:
                    return "https://i.imgur.com/MIpP5pK.png";
                case ArcDPSEnums.TrashID.SmallJumpyTornado:
                    return "https://i.imgur.com/WBJNgp7.png";
                case ArcDPSEnums.TrashID.OrbSpider:
                    return "https://i.imgur.com/FB5VM9X.png";
                case ArcDPSEnums.TrashID.Seekers:
                    return "https://i.imgur.com/FrPoluz.png";
                case ArcDPSEnums.TrashID.BlueGuardian:
                    return "https://i.imgur.com/6CefnkP.png";
                case ArcDPSEnums.TrashID.GreenGuardian:
                    return "https://i.imgur.com/nauDVYP.png";
                case ArcDPSEnums.TrashID.RedGuardian:
                    return "https://i.imgur.com/73Uj4lG.png";
                case ArcDPSEnums.TrashID.UnderworldReaper:
                    return "https://i.imgur.com/Tq6SYVe.png";
                case ArcDPSEnums.TrashID.CagedWarg:
                case ArcDPSEnums.TrashID.GreenSpirit1:
                case ArcDPSEnums.TrashID.GreenSpirit2:
                case ArcDPSEnums.TrashID.BanditSapper:
                case ArcDPSEnums.TrashID.ProjectionArkk:
                case ArcDPSEnums.TrashID.PrioryExplorer:
                case ArcDPSEnums.TrashID.PrioryScholar:
                case ArcDPSEnums.TrashID.VigilRecruit:
                case ArcDPSEnums.TrashID.VigilTactician:
                case ArcDPSEnums.TrashID.Prisoner1:
                case ArcDPSEnums.TrashID.Prisoner2:
                case ArcDPSEnums.TrashID.Pylon1:
                    return "https://i.imgur.com/0koP4xB.png";
                case ArcDPSEnums.TrashID.FleshWurm:
                    return "https://i.imgur.com/o3vX9Zc.png";
                case ArcDPSEnums.TrashID.Hands:
                    return "https://i.imgur.com/8JRPEoo.png";
                case ArcDPSEnums.TrashID.TemporalAnomaly:
                case ArcDPSEnums.TrashID.TemporalAnomaly2:
                    return "https://i.imgur.com/MIpP5pK.png";
                case ArcDPSEnums.TrashID.DOC:
                case ArcDPSEnums.TrashID.BLIGHT:
                case ArcDPSEnums.TrashID.PLINK:
                case ArcDPSEnums.TrashID.CHOP:
                    return "https://wiki.guildwars2.com/images/4/47/Mini_Baron_von_Scrufflebutt.png";
                case ArcDPSEnums.TrashID.FreeziesFrozenHeart:
                    return "https://wiki.guildwars2.com/images/9/9e/Mini_Freezie%27s_Heart.png";
                case ArcDPSEnums.TrashID.RiverOfSouls:
                    return "https://i.imgur.com/4pXEnaX.png";
                case ArcDPSEnums.TrashID.DhuumDesmina:
                    return "https://i.imgur.com/jAiRplg.png";
                //case CastleFountain:
                //    return "https://i.imgur.com/xV0OPWL.png";
                case ArcDPSEnums.TrashID.HauntingStatue:
                    return "https://i.imgur.com/7IQDyuK.png";
                case ArcDPSEnums.TrashID.GreenKnight:
                case ArcDPSEnums.TrashID.RedKnight:
                case ArcDPSEnums.TrashID.BlueKnight:
                    return "https://i.imgur.com/lpBm4d6.png";
                case ArcDPSEnums.TrashID.CloneArtsariiv:
                    return "https://wiki.guildwars2.com/images/b/b4/Artsariiv.jpg";
            }
            return "https://i.imgur.com/HuJHqRZ.png";
        }


        private static readonly HashSet<string> _compressedFiles = new HashSet<string>()
        {
            ".zevtc",
            ".evtc.zip",
        };

        private static readonly HashSet<string> _tmpCompressedFiles = new HashSet<string>()
        {
            ".tmp.zip"
        };

        private static readonly HashSet<string> _tmpFiles = new HashSet<string>()
        {
            ""
        };

        private static readonly HashSet<string> _supportedFiles = new HashSet<string>(_compressedFiles)
        {
            ".evtc"
        };

        public static bool IsCompressedFormat(string fileName)
        {
            foreach (string format in _compressedFiles)
            {
                if (fileName.EndsWith(format, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static IReadOnlyList<string> GetSupportedFormats()
        {
            return new List<string>(_supportedFiles);
        }

        public static bool IsSupportedFormat(string fileName)
        {
            foreach (string format in _supportedFiles)
            {
                if (fileName.EndsWith(format, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
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

        public static bool IsTemporaryCompressedFormat(string fileName)
        {
            foreach (string format in _tmpCompressedFiles)
            {
                if (fileName.EndsWith(format, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsTemporaryFormat(string fileName)
        {
            foreach (string format in _tmpFiles)
            {
                if (fileName.EndsWith(format, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
                
    }

    public static class Colors 
    {
        public const string Red = "rgb(255,0,0)";
        public const string DarkRed = "rgb(128,0,0)";
        public const string Orange = "rgb(255,100,0)";
        public const string LightOrange = "rgb(255,160,0)";
        public const string Yellow = "rgb(255,220,0)";
        public const string Brown = "rgb(120,100,0)";
        public const string Green = "rgb(0,255,0)";
        public const string DarkGreen = "rgb(0,128,0)";
        public const string Teal = "rgb(0,255,255)";
        public const string DarkTeal = "rgb(0,160,150)";
        public const string LightBlue = "rgb(0,140,255)"; 
        public const string Purple = "rgb(150,0,255)";
        public const string DarkPurple = "rgb(50,0,150)";
        public const string LightPurple = "rgb(200, 140, 255)";
        public const string Pink = "rgb(255,0,150)";
        public const string Magenta = "rgb(255,0,255)";
        public const string Blue = "rgb(0,0,255)";
        public const string White = "rgb(255,255,255)";
        public const string Grey = "rgb(60,60,60)";
        public const string Black = "rgb(0,0,0)";
        public const string LightRed = "rgb(255,128,128)";
    }
}
