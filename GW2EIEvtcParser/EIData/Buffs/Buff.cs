using System;
using System.Collections.Generic;
using System.IO;
using GW2EIEvtcParser.EIData.BuffSimulators;
using GW2EIEvtcParser.EIData.BuffSourceFinders;
using GW2EIEvtcParser.Interfaces;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    public class Buff : IVersionable
    {

        // Boon
        public enum BuffClassification
        {
            Condition,
            Boon,
            Offensive,
            Defensive,
            Support,
            Debuff,
            Gear,
            Other,
            Consumable,
            Unknown
        };

        public enum BuffType
        {
            Duration = 0,
            Intensity = 1,
            Unknown = 2,
        }

        // Fields
        public string Name { get; }
        public long ID { get; }
        public BuffClassification Classification { get; }
        public ParserHelper.Source Source { get; }
        public BuffStackType StackType { get; }
        public BuffType Type
        {
            get
            {
                switch (StackType)
                {
                    case BuffStackType.Queue:
                    case BuffStackType.Regeneration:
                    case BuffStackType.CappedDuration:
                    case BuffStackType.Force:
                        return BuffType.Duration;
                    case BuffStackType.Stacking:
                    case BuffStackType.StackingConditionalLoss:
                        return BuffType.Intensity;
                    default:
                        return BuffType.Unknown;
                }
            }
        }

        private ulong _maxBuild { get; } = GW2Builds.EndOfLife;
        private ulong _minBuild { get; } = GW2Builds.StartOfLife;
        public int Capacity { get; }
        public string Link { get; }

        /// <summary>
        /// Buff constructor
        /// </summary>
        /// <param name="name">The name of the boon</param>
        /// <param name="id">The id of the buff</param>
        /// <param name="source">Source of the buff <see cref="ParserHelper.Source"/></param>
        /// <param name="type">Stack Type of the buff<see cref="BuffStackType"/></param>
        /// <param name="capacity">Maximun amount of buff in stack</param>
        /// <param name="nature">Nature of the buff, dictates in which category the buff will appear <see cref="BuffClassification"/></param>
        /// <param name="link">URL to the icon of the buff</param>
        internal Buff(string name, long id, ParserHelper.Source source, BuffStackType type, int capacity, BuffClassification nature, string link)
        {
            Name = name;
            ID = id;
            Source = source;
            StackType = type;
            Capacity = capacity;
            Classification = nature;
            Link = link;
        }

        internal Buff(string name, long id, ParserHelper.Source source, BuffClassification nature, string link) : this(name, id, source, BuffStackType.Force, 1, nature, link)
        {
        }

        internal Buff(string name, long id, ParserHelper.Source source, BuffStackType type, int capacity, BuffClassification nature, string link, ulong minBuild, ulong maxBuild) : this(name, id, source, type, capacity, nature, link)
        {
            _maxBuild = maxBuild;
            _minBuild = minBuild;
        }

        internal Buff(string name, long id, ParserHelper.Source source, BuffClassification nature, string link, ulong minBuild, ulong maxBuild) : this(name, id, source, BuffStackType.Force, 1, nature, link, minBuild, maxBuild)
        {
        }

        public Buff(string name, long id, string link)
        {
            Name = name;
            ID = id;
            Source = Source.Unknown;
            StackType = BuffStackType.Unknown;
            Capacity = 1;
            Classification = BuffClassification.Unknown;
            Link = link;
        }

        internal static Buff CreateCustomConsumable(string name, long id, string link, int capacity)
        {
            return new Buff(name + " " + id, id, Source.Item, capacity > 1 ? BuffStackType.Stacking : BuffStackType.Force, capacity, BuffClassification.Consumable, link);
        }

        internal void VerifyBuffInfoEvent(BuffInfoEvent buffInfoEvent, ParserController operation)
        {
            if (buffInfoEvent.BuffID != ID)
            {
                return;
            }
            if (Capacity != buffInfoEvent.MaxStacks)
            {
                operation.UpdateProgressWithCancellationCheck("Adjusted capacity for " + Name + " from " + Capacity + " to " + buffInfoEvent.MaxStacks);
            }
            /*if (buffInfoEvent.StackingType != StackType)
            {
                //_stackType = buffInfoEvent.StackingType; // might be unreliable due to its absence on some logs
                operation.UpdateProgressWithCancellationCheck("Incoherent stack type for " + Name + ": is " + StackType + " but expected " + buffInfoEvent.StackingType);
            }*/
        }
        internal AbstractBuffSimulator CreateSimulator(ParsedEvtcLog log, bool forceNoId)
        {
            BuffInfoEvent buffInfoEvent = log.CombatData.GetBuffInfoEvent(ID);
            int capacity = Capacity;
            if (buffInfoEvent != null && buffInfoEvent.MaxStacks != capacity)
            {
                capacity = buffInfoEvent.MaxStacks;
            }
            if (!log.CombatData.HasStackIDs || forceNoId)
            {

                switch (Type)
                {
                    case BuffType.Intensity: 
                        return new BuffSimulatorIntensity(log, this, capacity);
                    case BuffType.Duration: 
                        return new BuffSimulatorDuration(log, this, capacity);
                    case BuffType.Unknown:
                    default:
                        throw new InvalidDataException("Buffs can not be stackless");
                }
            }
            switch (Type)
            {
                case BuffType.Intensity:
                    return new BuffSimulatorIDIntensity(log, this, capacity);
                case BuffType.Duration:
                    return new BuffSimulatorIDDuration(log, this);
                case BuffType.Unknown:
                default:
                    throw new InvalidDataException("Buffs can not be stackless");
            }
        }

        internal static BuffSourceFinder GetBuffSourceFinder(ulong version, HashSet<long> boonIds)
        {
            if (version >= GW2Builds.EODBeta2)
            {
                return new BuffSourceFinder20210921(boonIds);
            }
            if (version >= GW2Builds.May2021Balance)
            {
                return new BuffSourceFinder20210511(boonIds);
            }
            if (version >= GW2Builds.October2019Balance)
            {
                return new BuffSourceFinder20191001(boonIds);
            }
            if (version >= GW2Builds.March2019Balance)
            {
                return new BuffSourceFinder20190305(boonIds);
            }
            return new BuffSourceFinder20181211(boonIds);
        }

        public bool Available(ulong gw2Build)
        {
            return gw2Build < _maxBuild && gw2Build >= _minBuild;
        }


        internal static readonly List<Buff> Boons = new List<Buff>
        {
                new Buff("Might", Might, Source.Common, BuffStackType.Stacking, 25, BuffClassification.Boon, "https://wiki.guildwars2.com/images/7/7c/Might.png"),
                new Buff("Fury", Fury, Source.Common, BuffStackType.Queue, 9, BuffClassification.Boon, "https://wiki.guildwars2.com/images/4/46/Fury.png"/*, 0, GW2Builds.May2021Balance*/),
                new Buff("Quickness", Quickness, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, "https://wiki.guildwars2.com/images/b/b4/Quickness.png"/*, 0, GW2Builds.May2021Balance*/),
                new Buff("Alacrity", Alacrity, Source.Common, BuffStackType.Queue, 9, BuffClassification.Boon, "https://wiki.guildwars2.com/images/4/4c/Alacrity.png"/*, 0, GW2Builds.May2021Balance*/),
                new Buff("Protection", Protection, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, "https://wiki.guildwars2.com/images/6/6c/Protection.png"/*, 0 , GW2Builds.May2021Balance*/),
                new Buff("Regeneration", Regeneration, Source.Common, BuffStackType.Regeneration, 5, BuffClassification.Boon, "https://wiki.guildwars2.com/images/5/53/Regeneration.png"),
                new Buff("Vigor", Vigor, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, "https://wiki.guildwars2.com/images/f/f4/Vigor.png"/*, 0, GW2Builds.May2021Balance*/),
                new Buff("Aegis", Aegis, Source.Common, BuffStackType.Queue, 9, BuffClassification.Boon, "https://wiki.guildwars2.com/images/e/e5/Aegis.png"),
                new Buff("Stability", Stability, Source.Common, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Boon, "https://wiki.guildwars2.com/images/a/ae/Stability.png"),
                new Buff("Swiftness", Swiftness, Source.Common, BuffStackType.Queue, 9, BuffClassification.Boon, "https://wiki.guildwars2.com/images/a/af/Swiftness.png"/*, 0, GW2Builds.May2021Balance*/),
                new Buff("Retaliation", Retaliation, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, "https://wiki.guildwars2.com/images/5/53/Retaliation.png", 0, GW2Builds.May2021Balance),
                new Buff("Resistance", Resistance, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, "https://wiki.guildwars2.com/images/4/4b/Resistance.png"/*, 0, GW2Builds.May2021Balance*/),
                //
                /*new Buff("Fury", 725, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Boon, "https://wiki.guildwars2.com/images/4/46/Fury.png", GW2Builds.May2021Balance, ulong.MaxValue),
                new Buff("Quickness", 1187, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Boon, "https://wiki.guildwars2.com/images/b/b4/Quickness.png", GW2Builds.May2021Balance, ulong.MaxValue),
                new Buff("Alacrity", 30328, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Boon, "https://wiki.guildwars2.com/images/4/4c/Alacrity.png", GW2Builds.May2021Balance, ulong.MaxValue),
                new Buff("Protection", 717, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Boon, "https://wiki.guildwars2.com/images/6/6c/Protection.png", GW2Builds.May2021Balance, ulong.MaxValue),
                new Buff("Vigor", 726, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Boon, "https://wiki.guildwars2.com/images/f/f4/Vigor.png", GW2Builds.May2021Balance, ulong.MaxValue),
                new Buff("Swiftness", 719, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Boon, "https://wiki.guildwars2.com/images/a/af/Swiftness.png", GW2Builds.May2021Balance, ulong.MaxValue),
                new Buff("Resolution", 873, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Boon, "https://wiki.guildwars2.com/images/0/06/Resolution.png", GW2Builds.May2021Balance, ulong.MaxValue),
                new Buff("Resistance", 26980, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Boon, "https://wiki.guildwars2.com/images/4/4b/Resistance.png", GW2Builds.May2021Balance, ulong.MaxValue),*/
                new Buff("Resolution", Resolution, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, "https://wiki.guildwars2.com/images/0/06/Resolution.png", GW2Builds.May2021Balance, GW2Builds.EndOfLife),
                //
                new Buff("Number of Boons", NumberOfBoons, Source.Common, BuffStackType.Stacking, 0, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/44/Boon_Duration.png"),
        };

        internal static readonly List<Buff> Conditions = new List<Buff>
        {
                new Buff("Bleeding", Bleeding, Source.Common, BuffStackType.Stacking, 1500, BuffClassification.Condition, "https://wiki.guildwars2.com/images/3/33/Bleeding.png"),
                new Buff("Burning", Burning, Source.Common, BuffStackType.Stacking, 1500, BuffClassification.Condition, "https://wiki.guildwars2.com/images/4/45/Burning.png"),
                new Buff("Confusion", Confusion, Source.Common, BuffStackType.Stacking, 1500, BuffClassification.Condition, "https://wiki.guildwars2.com/images/e/e6/Confusion.png"),
                new Buff("Poison", Poison, Source.Common, BuffStackType.Stacking, 1500, BuffClassification.Condition, "https://wiki.guildwars2.com/images/1/11/Poisoned.png"),
                new Buff("Torment", Torment, Source.Common, BuffStackType.Stacking, 1500, BuffClassification.Condition, "https://wiki.guildwars2.com/images/0/08/Torment.png"),
                new Buff("Blind", Blind, Source.Common, BuffStackType.Queue, 9, BuffClassification.Condition, "https://wiki.guildwars2.com/images/3/33/Blinded.png"/*, 0, GW2Builds.May2021Balance*/),
                new Buff("Chilled", Chilled, Source.Common, BuffStackType.Queue, 5, BuffClassification.Condition, "https://wiki.guildwars2.com/images/a/a6/Chilled.png"/*, 0, GW2Builds.May2021Balance*/),
                new Buff("Crippled", Crippled, Source.Common, BuffStackType.Queue, 5, BuffClassification.Condition, "https://wiki.guildwars2.com/images/f/fb/Crippled.png"/*, 0, GW2Builds.May2021Balance*/),
                new Buff("Fear", Fear, Source.Common, BuffStackType.Queue, 5, BuffClassification.Condition, "https://wiki.guildwars2.com/images/e/e6/Fear.png"),
                new Buff("Immobile", Immobile, Source.Common, BuffStackType.Queue, 3, BuffClassification.Condition, "https://wiki.guildwars2.com/images/3/32/Immobile.png"/*, 0, GW2Builds.May2021Balance*/),
                new Buff("Slow", Slow, Source.Common, BuffStackType.Queue, 9, BuffClassification.Condition, "https://wiki.guildwars2.com/images/f/f5/Slow.png"/*, 0, GW2Builds.May2021Balance*/),
                new Buff("Weakness", Weakness, Source.Common, BuffStackType.Queue, 5, BuffClassification.Condition, "https://wiki.guildwars2.com/images/f/f9/Weakness.png"/*, 0, GW2Builds.May2021Balance*/),
                new Buff("Taunt", Taunt, Source.Common, BuffStackType.Queue, 5, BuffClassification.Condition, "https://wiki.guildwars2.com/images/c/cc/Taunt.png"),
                new Buff("Vulnerability", Vulnerability, Source.Common, BuffStackType.Stacking, 25, BuffClassification.Condition, "https://wiki.guildwars2.com/images/a/af/Vulnerability.png"),
                //          
               /* new Buff("Blind", 720, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Condition, "https://wiki.guildwars2.com/images/3/33/Blinded.png", GW2Builds.May2021Balance, ulong.MaxValue),
                new Buff("Chilled", 722, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Condition, "https://wiki.guildwars2.com/images/a/a6/Chilled.png", GW2Builds.May2021Balance, ulong.MaxValue),
                new Buff("Crippled", 721, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Condition, "https://wiki.guildwars2.com/images/f/fb/Crippled.png", GW2Builds.May2021Balance, ulong.MaxValue),
                new Buff("Immobile", 727, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Condition, "https://wiki.guildwars2.com/images/3/32/Immobile.png", GW2Builds.May2021Balance, ulong.MaxValue),
                new Buff("Slow", 26766, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Condition, "https://wiki.guildwars2.com/images/f/f5/Slow.png", GW2Builds.May2021Balance, ulong.MaxValue),
                new Buff("Weakness", 742, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Condition, "https://wiki.guildwars2.com/images/f/f9/Weakness.png", GW2Builds.May2021Balance, ulong.MaxValue),*/
                //
                new Buff("Number of Conditions", NumberOfConditions, Source.Common, BuffStackType.Stacking, 0, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/38/Condition_Duration.png"),
        };

        internal static readonly List<Buff> Commons = new List<Buff>
        {
                new Buff("Number of Active Combat Minions", NumberOfActiveCombatMinions, Source.Common, BuffStackType.Stacking, 99, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/ad/Superior_Rune_of_the_Ranger.png"),
                new Buff("Number of Clones", NumberOfClones, Source.Common, BuffStackType.Stacking, 99, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/24/Superior_Rune_of_the_Mesmer.png"),
                new Buff("Downed", Downed, Source.Common, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/dd/Downed.png"),
                new Buff("Exhaustion", Exhaustion, Source.Common, BuffStackType.Queue, 3, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/8/88/Exhaustion.png"),
                new Buff("Stealth", Stealth, Source.Common, BuffStackType.Queue, 5, BuffClassification.Support, "https://wiki.guildwars2.com/images/1/19/Stealth.png"),
                new Buff("Hide in Shadows", HideInShadows, Source.Common, BuffStackType.Queue, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/19/Stealth.png"),
                new Buff("Revealed", Revealed, Source.Common, BuffClassification.Support, "https://wiki.guildwars2.com/images/d/db/Revealed.png"),
                new Buff("Superspeed", Superspeed, Source.Common, BuffClassification.Support,"https://wiki.guildwars2.com/images/c/c8/Superspeed.png", 0, GW2Builds.June2021Balance),
                new Buff("Superspeed", Superspeed, Source.Common, BuffStackType.Queue, 9, BuffClassification.Support,"https://wiki.guildwars2.com/images/c/c8/Superspeed.png", GW2Builds.June2021Balance, GW2Builds.EndOfLife),
                new Buff("Determined (762)", Determined762, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Determined (788)", Determined788, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Resurrection", Resurrection, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Determined (895)", Determined895, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Determined (3892)", Determined3892, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Determined (31450)", Determined31450, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Determined (52271)", Determined52271, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Invulnerability (757)", Invulnerability757, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Invulnerability (56227)", Invulnerability56227, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Invulnerability (801)", Invulnerability801, Source.Common, BuffStackType.Queue, 25, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Spawn Protection?", SpawnProtection, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Stun", Stun, Source.Common, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/97/Stun.png"),
                new Buff("Daze", Daze, Source.Common, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/79/Daze.png"),
                new Buff("Exposed (48209)", Exposed48209, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/6b/Exposed.png"),
                new Buff("Exposed (31589)", Exposed31589, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/6b/Exposed.png"),
                new Buff("Old Exposed", OldExposed, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/6b/Exposed.png"),
                new Buff("Unblockable",Unblockable, Source.Common, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/f/f0/Unblockable_%28effect%29.png",GW2Builds.February2020Balance , GW2Builds.EndOfLife),
                new Buff("Encumbered",Encumbered, Source.Common, BuffStackType.Queue, 9, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/d/d7/Encumbered.png"),
                new Buff("Celeritas Spores", CeleritasSpores, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/7/7b/Blazing_Speed_Mushrooms.png"),
                new Buff("Branded Accumulation", BrandedAccumulation, Source.Common, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/55/Achilles_Bane.png" ),
                //Auras
                new Buff("Chaos Aura", ChaosAura, Source.Common, BuffClassification.Support,"https://wiki.guildwars2.com/images/e/ec/Chaos_Aura.png"),
                new Buff("Fire Aura", FireAura, Source.Common, BuffClassification.Support,"https://wiki.guildwars2.com/images/c/ce/Fire_Aura.png"),
                new Buff("Frost Aura", FrostAura, Source.Common, BuffClassification.Support,"https://wiki.guildwars2.com/images/8/87/Frost_Aura_%28effect%29.png"),
                new Buff("Light Aura", LightAura, Source.Common, BuffClassification.Support,"https://wiki.guildwars2.com/images/5/5a/Light_Aura.png"),
                new Buff("Magnetic Aura", MagneticAura, Source.Common, BuffClassification.Support, "https://wiki.guildwars2.com/images/0/0b/Magnetic_Aura_%28effect%29.png"),
                new Buff("Shocking Aura", ShockingAura, Source.Common, BuffClassification.Support,"https://wiki.guildwars2.com/images/5/5d/Shocking_Aura_%28effect%29.png"),
                new Buff("Dark Aura", DarkAura, Source.Common, BuffClassification.Support,"https://wiki.guildwars2.com/images/e/ef/Dark_Aura.png", GW2Builds.April2019Balance, GW2Builds.EndOfLife),
                //race
                new Buff("Take Root", TakeRootEffect, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/b/b2/Take_Root.png"),
                new Buff("Become the Bear",BecomeTheBear, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/7/7e/Become_the_Bear.png"),
                new Buff("Become the Raven",BecomeTheRaven, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/2/2c/Become_the_Raven.png"),
                new Buff("Become the Snow Leopard",BecomeTheSnowLeopard, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/7/78/Become_the_Snow_Leopard.png"),
                new Buff("Become the Wolf",BecomeTheWolf, Source.Common, BuffClassification.Other, "https://wiki.guildwars2.com/images/f/f1/Become_the_Wolf.png"),
                new Buff("Avatar of Melandru", AvatarOfMelandru, Source.Common, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/30/Avatar_of_Melandru.png"),
                new Buff("Power Suit",PowerSuit, Source.Common, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/89/Summon_Power_Suit.png"),
                new Buff("Reaper of Grenth", ReaperOfGrenth, Source.Common, BuffClassification.Other, "https://wiki.guildwars2.com/images/0/07/Reaper_of_Grenth.png"),
                new Buff("Charrzooka",Charrzooka, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/1/17/Charrzooka.png"),
                //
                new Buff("Guild Item Research", GuildItemResearch, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/c/c6/Guild_Magic_Find_Banner_Boost.png"),
                //
                new Buff("Crystalline Heart", CrystallineHeart, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/5/56/Crystalline_Heart.png"),
                // WvW
                new Buff("Minor Borderlands Bloodlust", MinorBorderlandsBloodlust, Source.Common, BuffClassification.Support,"https://wiki.guildwars2.com/images/f/f7/Major_Borderlands_Bloodlust.png"),
                new Buff("Major Borderlands Bloodlust", MajorBorderlandsBloodlust, Source.Common, BuffClassification.Support,"https://wiki.guildwars2.com/images/f/f7/Major_Borderlands_Bloodlust.png"),
                new Buff("Superior Borderlands Bloodlust", SuperiorBorderlandsBloodlust, Source.Common, BuffClassification.Support,"https://wiki.guildwars2.com/images/f/f7/Major_Borderlands_Bloodlust.png"),
                new Buff("Blessing of Elements", BlessingOfElements, Source.Common, BuffClassification.Support,"https://wiki.guildwars2.com/images/3/3c/Blessing_of_Air.png"),
                new Buff("Flame's Embrace", FlamesEmbrace, Source.Common, BuffClassification.Support,"https://wiki.guildwars2.com/images/5/53/Flame%27s_Embrace.png"),
        };

        internal static readonly List<Buff> Gear = new List<Buff>
        {
                new Buff("Sigil of Concentration", SigilOfConcentration, Source.Gear, BuffClassification.Gear, "https://wiki.guildwars2.com/images/b/b3/Superior_Sigil_of_Concentration.png",0 , 93543),
                new Buff("Superior Rune of the Monk", SuperiorRuneOfTheMonk, Source.Gear, BuffStackType.Stacking, 10, BuffClassification.Gear, "https://wiki.guildwars2.com/images/1/18/Superior_Rune_of_the_Monk.png", 93543, GW2Builds.EndOfLife),
                new Buff("Sigil of Corruption", SigilOfCorruption, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, "https://wiki.guildwars2.com/images/1/18/Superior_Sigil_of_Corruption.png"),
                new Buff("Sigil of Life", SigilOfLife, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, "https://wiki.guildwars2.com/images/a/a7/Superior_Sigil_of_Life.png"),
                new Buff("Sigil of Perception", SigilOfPerception, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, "https://wiki.guildwars2.com/images/c/cc/Superior_Sigil_of_Perception.png"),
                new Buff("Sigil of Bloodlust", SigilOfBloodlust, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, "https://wiki.guildwars2.com/images/f/fb/Superior_Sigil_of_Bloodlust.png"),
                new Buff("Sigil of Bounty", SigilOfBounty, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, "https://wiki.guildwars2.com/images/f/f8/Superior_Sigil_of_Bounty.png"),
                new Buff("Sigil of Benevolence", SigilOfBenevolence, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, "https://wiki.guildwars2.com/images/5/59/Superior_Sigil_of_Benevolence.png"),
                new Buff("Sigil of Momentum", SigilOfMomentum, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, "https://wiki.guildwars2.com/images/3/30/Superior_Sigil_of_Momentum.png"),
                new Buff("Sigil of the Stars", SigilOfTheStars, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, "https://wiki.guildwars2.com/images/d/dc/Superior_Sigil_of_the_Stars.png"),
                new Buff("Sigil of Severance", SigilOfSeverance, Source.Gear, BuffClassification.Gear, "https://wiki.guildwars2.com/images/c/c2/Superior_Sigil_of_Severance.png"),
                new Buff("Sigil of Doom", SigilOfDoom, Source.Gear, BuffClassification.Gear, "https://wiki.guildwars2.com/images/6/67/Superior_Sigil_of_Doom.png"),
        };

        internal static readonly List<Buff> FractalInstabilities = new List<Buff>()
        {
            // Legacy
            new Buff("Mistlock Instability: Fleeting Precision", MistlockInstabilityFleetingPrecision, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Mistlock_Instability.png"),
            new Buff("Mistlock Instability: Impaired Immunity", MistlockInstabilityImpairedImmunity, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Mistlock_Instability.png"),
            new Buff("Mistlock Instability: Losing Control", MistlockInstabilityLosingControl, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Mistlock_Instability.png"),
            new Buff("Mistlock Instability: Mist Stalker", MistlockInstabilityMistStalker, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Mistlock_Instability.png"),
            new Buff("Mistlock Instability: Slippery Slope 1", MistlockInstabilitySlipperySlope1, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c2/Mistlock_Instability_Slippery_Slope.png"),
            new Buff("Mistlock Instability: Slippery Slope 2", MistlockInstabilitySlipperySlope2, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c2/Mistlock_Instability_Slippery_Slope.png"),
            new Buff("Mistlock Instability: Stormy Weather", MistlockInstabilityStormyWeather, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Mistlock_Instability.png"),
            new Buff("Mistlock Instability: Birds", MistlockInstabilityBirds, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/e8/Mistlock_Instability_Birds.png"),
            new Buff("Mistlock Instability: Tainted Renewal", MistlockInstabilityTaintedRenewal, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Mistlock_Instability.png"),
            new Buff("Mistlock Instability: Stamina", MistlockInstabilityStamina, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Mistlock_Instability.png"),
            new Buff("Mistlock Instability: Playing Favorites", MistlockInstabilityPlayingFavorites, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Mistlock_Instability.png"),
            new Buff("Mistlock Instability: Overextended", MistlockInstabilityOverextended, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Mistlock_Instability.png"),
            new Buff("Mistlock Instability: Keep Them in Line", MistlockInstabilityKeepTheminLine, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Mistlock_Instability.png"),
            new Buff("Mistlock Instability: Ill and Chill", MistlockInstabilityIllAndChill, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Mistlock_Instability.png"),
            new Buff("Mistlock Instability: Frosty", MistlockInstabilityFrosty, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Mistlock_Instability.png"),
            new Buff("Mistlock Instability: Bloodlust", MistlockInstabilityBloodlust, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Mistlock_Instability.png"),
            new Buff("Mistlock Instability: Antielitism", MistlockInstabilityAntielitism, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Mistlock_Instability.png"),
            new Buff("Mistlock Instability: Agonizing Expedition", MistlockInstabilityAgonizingExpedition, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Mistlock_Instability.png"),
            //
            new Buff("Mistlock Instability: Adrenaline Rush", MistlockInstabilityAdrenalineRush, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/72/Mistlock_Instability_Adrenaline_Rush.png"),
            new Buff("Mistlock Instability: Afflicted", MistlockInstabilityAfflicted, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/3f/Mistlock_Instability_Afflicted.png"),
            new Buff("Mistlock Instability: Boon Overload", MistlockInstabilityBoonOverload, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d7/Mistlock_Instability_Boon_Overload.png"),
            new Buff("Mistlock Instability: Flux Bomb", MistlockInstabilityFluxBomb, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/3f/Mistlock_Instability_Flux_Bomb.png"),
            new Buff("Mistlock Instability: Fractal Vindicators", MistlockInstabilityFractalVindicators, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/48/Mistlock_Instability_Fractal_Vindicators.png"),
            new Buff("Mistlock Instability: Frailty", MistlockInstabilityFrailty, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d6/Mistlock_Instability_Frailty.png"),
            new Buff("Mistlock Instability: Hamstrung", MistlockInstabilityHamstrung, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/99/Mistlock_Instability_Hamstrung.png"),
            new Buff("Mistlock Instability: Last Laugh", MistlockInstabilityLastLaugh, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/58/Mistlock_Instability_Last_Laugh.png"),
            new Buff("Mistlock Instability: Mists Convergence", MistlockInstabilityMistsConvergence, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/95/Mistlock_Instability_Mists_Convergence.png"),
            new Buff("Mistlock Instability: No Pain, No Gain", MistlockInstabilityNoPainNoGain, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c3/Mistlock_Instability_No_Pain%2C_No_Gain.png"),
            new Buff("Mistlock Instability: Outflanked", MistlockInstabilityOutflanked, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/0/0c/Mistlock_Instability_Outflanked.png"),
            new Buff("Mistlock Instability: Social Awkwardness", MistlockInstabilitySocialAwkwardness, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d2/Mistlock_Instability_Social_Awkwardness.png"),
            new Buff("Mistlock Instability: Stick Together", MistlockInstabilityStickTogether, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/59/Mistlock_Instability_Stick_Together.png"),
            new Buff("Mistlock Instability: Sugar Rush", MistlockInstabilitySugarRush, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/4c/Mistlock_Instability_Sugar_Rush.png"),
            new Buff("Mistlock Instability: Toxic Trail", MistlockInstabilityToxicTrail, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/df/Mistlock_Instability_Toxic_Trail.png"),
            new Buff("Mistlock Instability: Vengeance", MistlockInstabilityVengeance, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c6/Mistlock_Instability_Vengeance.png"),
            new Buff("Mistlock Instability: We Bleed Fire", MistlockInstabilityWeBleedFire, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/24/Mistlock_Instability_We_Bleed_Fire.png"),
            new Buff("Mistlock Instability: Toxic Sickness", MistlockInstabilityToxicSickness, Source.FractalInstability, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/6f/Mistlock_Instability_Toxic_Sickness.png"),
        };

        internal static readonly List<Buff> FightSpecific = new List<Buff>
        {
                // Generic
                new Buff("Emboldened", Emboldened, Source.FightSpecific,BuffStackType.Stacking, 5, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/6/69/Emboldened.png" ),
                new Buff("Spectral Agony", SpectralAgony, Source.FightSpecific,BuffStackType.Stacking, 25, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/7/70/Spectral_Agony.png" ),
                new Buff("Agony", Agony, Source.FightSpecific,BuffStackType.Stacking, 25, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/b/be/Agony.png" ),
                new Buff("Hamstrung", Hamstrung, Source.FightSpecific,BuffStackType.Stacking, 99, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/b/b9/Unseen_Burden.png" ),
                new Buff("Enraged (?)", Enraged1_Unknown, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Enraged (??)", Enraged2_Unknown, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Enraged 1 (100%)", Enraged1_100, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Enraged 2 (100%)", Enraged2_100, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Enraged 1 (200%)", Enraged1_200, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Enraged 2 (200%)", Enraged2_200, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Enraged 3 (200%)", Enraged3_200, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Enraged 4 (200%)", Enraged4_200, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Enraged (300%)", Enraged_300, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Enraged (500%)", Enraged_500, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Call of the Mists", CallOfTheMists, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/79/Call_of_the_Mists_%28raid_effect%29.png" ),
                new Buff("Untargetable", Untargetable, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/eb/Determined.png" ),
                // Strike Essences
                new Buff("Essence of Vigilance Tier 1", EssenceOfVigilanceTier1, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a4/Essence_of_Vigilance.png" ),
                new Buff("Essence of Vigilance Tier 2", EssenceOfVigilanceTier2, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a4/Essence_of_Vigilance.png" ),
                new Buff("Essence of Vigilance Tier 3", EssenceOfVigilanceTier3, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a4/Essence_of_Vigilance.png" ),
                new Buff("Essence of Vigilance Tier 4", EssenceOfVigilanceTier4, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a4/Essence_of_Vigilance.png" ),
                new Buff("Power of Vigilance Tier 2", PowerOfVigilanceTier2, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/86/Power_of_Vigilance.png" ),
                new Buff("Power of Vigilance Tier 3", PowerOfVigilanceTier3, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/86/Power_of_Vigilance.png" ),
                new Buff("Essence of Resilience Tier 1", EssenceOfResilienceTier1, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b6/Essence_of_Resilience.png" ),
                new Buff("Essence of Resilience Tier 2", EssenceOfResilienceTier2, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b6/Essence_of_Resilience.png" ),
                new Buff("Essence of Resilience Tier 3", EssenceOfResilienceTier3, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b6/Essence_of_Resilience.png" ),
                new Buff("Essence of Resilience Tier 4", EssenceOfResilienceTier4, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b6/Essence_of_Resilience.png" ),
                new Buff("Power of Resilience Tier 2", PowerOfResilienceTier2, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d3/Power_of_Resilience.png" ),
                new Buff("Power of Resilience Tier 4", PowerOfResilienceTier4, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d3/Power_of_Resilience.png" ),
                new Buff("Essence of Valor Tier 1", EssenceOfValorTier1, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/6f/Essence_of_Valor.png" ),
                new Buff("Essence of Valor Tier 2", EssenceOfValorTier2, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/6f/Essence_of_Valor.png" ),
                new Buff("Essence of Valor Tier 3", EssenceOfValorTier3, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/6f/Essence_of_Valor.png" ),
                new Buff("Essence of Valor Tier 4", EssenceOfValorTier4, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/6f/Essence_of_Valor.png" ),
                new Buff("Power of Valor Tier 1", PowerOfValorTier1, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/64/Power_of_Valor.png" ),
                new Buff("Power of Valor Tier 2", PowerOfValorTier2, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/64/Power_of_Valor.png" ),
                // Unknown Fixation            
                new Buff("Fixated 1(???)",Fixated1, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Fixated 2(???)",Fixated2, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                //////////////////////////////////////////////
                // Mordremoth
                new Buff("Parietal Mastery", ParietalMastery, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/7/76/Parietal_Mastery.png"),
                new Buff("Parietal Origin", ParietalOrigin, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/7/76/Parietal_Mastery.png"),
                new Buff("Temporal Mastery", TemporalMastery, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/8/80/Temporal_Mastery.png"),
                new Buff("Temporal Origin", TemporalOrigin, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/8/80/Temporal_Mastery.png"),
                new Buff("Occipital Mastery", OccipitalMastery, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/9/9a/Occipital_Mastery.png"),
                new Buff("Occipital Origin", OccipitalOrigin, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/9/9a/Occipital_Mastery.png"),
                new Buff("Frontal Mastery", FrontalMastery, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/4/44/Frontal_Mastery.png"),
                new Buff("Frontal Origin", FrontalOrigin, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/4/44/Frontal_Mastery.png"),
                new Buff("Exposed (Mordremoth)", ExposedMordremoth, Source.Common, BuffStackType.Stacking, 25, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/6b/Exposed.png"),
                new Buff("Weakened (Effect 1)", WeakenedEffect1, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/8/8a/Weakened.png"),
                new Buff("Weakened (Effect 2)", WeakenedEffect2, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/8/8a/Weakened.png"),
                new Buff("Empowered (Hearts and Minds)", EmpoweredHeartsAndMinds, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/5/5e/Empowered_%28Hearts_and_Minds%29.png"),
                new Buff("Power (Hearts and Minds)", PowerHeartsAndMinds, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/ec/Power_%28Hearts_and_Minds%29.png"),
                new Buff("Shifty Aura", ShiftyAura, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/7/78/Branded_Aura.png"),
                new Buff("Fiery Block", FieryBlock, Source.Common, BuffClassification.Other,"https://wiki.guildwars2.com/images/d/de/Shield_Stance.png"),
                //////////////////////////////////////////////
                // VG
                new Buff("Blue Pylon Power", BluePylonPower, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/6e/Blue_Pylon_Power.png"),
                new Buff("Pylon Attunement: Red", PylonAttunementRed, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/9/94/Pylon_Attunement-_Red.png"),
                new Buff("Pylon Attunement: Blue", PylonAttunementBlue, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/6e/Blue_Pylon_Power.png"),
                new Buff("Pylon Attunement: Green", PylonAttunementGreen, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/a/aa/Pylon_Attunement-_Green.png"),
                new Buff("Unstable Pylon: Red", UnstablePylonRed, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/3/36/Unstable_Pylon_%28Red%29.png"),
                new Buff("Unstable Pylon: Blue", UnstablePylonBlue, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/c/c3/Unstable_Pylon_%28Blue%29.png"),
                new Buff("Unstable Pylon: Green", UnstablePylonGreen, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/9/9d/Unstable_Pylon_%28Green%29.png"),
                new Buff("Unbreakable", Unbreakable, Source.FightSpecific, BuffStackType.Stacking, 2, BuffClassification.Other,"https://wiki.guildwars2.com/images/5/56/Xera%27s_Embrace.png"),            
                // Gorseval
                new Buff("Spirited Fusion",SpiritedFusion, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/eb/Spirited_Fusion.png"),
                new Buff("Protective Shadow", ProtectiveShadow, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/8/87/Protective_Shadow.png"),
                new Buff("Ghastly Prison", GhastlyPrison, Source.FightSpecific, BuffClassification.Debuff,"https://wiki.guildwars2.com/images/6/62/Ghastly_Prison.png"),
                new Buff("Vivid Echo", VividEcho, Source.FightSpecific, BuffStackType.Queue, 5, BuffClassification.Other,"https://wiki.guildwars2.com/images/4/4f/Vivid_Echo.png"),
                new Buff("Spectral Darkness", SpectralDarkness, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other,"https://wiki.guildwars2.com/images/a/a8/Spectral_Darkness.png"),
                // Sabetha    
                new Buff("Shell-Shocked", ShellShocked, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/3/39/Shell-Shocked.png"),
                new Buff("Sapper Bomb", SapperBomb, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/8/88/Sapper_Bomb_%28effect%29.png"),
                new Buff("Time Bomb", TimeBomb, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/9/91/Time_Bomb.png"),
                //////////////////////////////////////////////
                // Slothasor
                new Buff("Narcolepsy", NarcolepsyEffect, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Nauseated", Nauseated, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/3/30/Nauseated.png"),
                new Buff("Magic Transformation", MagicTransformation, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/4/45/Magic_Transformation.png"),
                new Buff("Fixated (Slothasor)", FixatedSlothasor, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Volatile Poison", VolatilePoisonEffect, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/1/12/Volatile_Poison.png"),
                // Trio
                new Buff("Not the Bees!", NotTheBees, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other,"https://wiki.guildwars2.com/images/0/08/Throw_Jar.png"),
                new Buff("Slow Burn", SlowBurn, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/6e/Heat_Wave_%28Matthias_Gabrel_effect%29.png"),
                new Buff("Targeted", Targeted, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Target!", Target, Source.FightSpecific, BuffStackType.Stacking, 20, BuffClassification.Other,"https://wiki.guildwars2.com/images/0/09/Target.png"),
                new Buff("Locust Trail", LocustTrail, Source.FightSpecific, BuffClassification.Debuff,"https://wiki.guildwars2.com/images/0/09/Target.png"),
                // Matthias
                new Buff("Blood Shield Abo",BloodShieldAbo, Source.FightSpecific, BuffStackType.Stacking, 18, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a6/Blood_Shield.png"),
                new Buff("Blood Shield",BloodShield, Source.FightSpecific, BuffStackType.Stacking, 18, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a6/Blood_Shield.png"),
                new Buff("Blood Fueled",BloodFueled, Source.FightSpecific, BuffStackType.Stacking, 1, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Blood Fueled Abo",BloodFueledAbo, Source.FightSpecific, BuffStackType.Stacking, 15, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Unstable Blood Magic", UnstableBloodMagic, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/0/09/Unstable_Blood_Magic.png"),
                new Buff("Corruption", Corruption1, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/3/34/Locust_Trail.png"),
                new Buff("Corruption 2", Corruption2, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/3/34/Locust_Trail.png"),
                new Buff("Sacrifice", MatthiasSacrifice, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/5/5f/Monster_Skill.png"),
                new Buff("Unbalanced", Unbalanced, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/8/80/Unbalanced.png"),
                new Buff("Zealous Benediction", ZealousBenediction, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/4/45/Unstable.png"),
                new Buff("Snowstorm", SnowstormEffect, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/2/26/Snowstorm_%28Matthias_Gabrel_effect%29.png"),
                new Buff("Heat Wave", HeatWave, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/6e/Heat_Wave_%28Matthias_Gabrel_effect%29.png"),
                new Buff("Downpour",DownpourEffect, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/4a/Downpour.png"),
                new Buff("Snowstorm (Matthias)", SnowstormMatthias, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/2/26/Snowstorm_%28Matthias_Gabrel_effect%29.png"),
                new Buff("Heat Wave (Matthias)", HeatWaveMatthias, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/6e/Heat_Wave_%28Matthias_Gabrel_effect%29.png"),
                new Buff("Downpour (Matthias)",DownpourMatthias, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/4a/Downpour.png"),
                new Buff("Unstable",Unstable, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/45/Unstable.png"),      
                //////////////////////////////////////////////
                // KC
                new Buff("Compromised",Compromised, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/48/Compromised.png"),
                new Buff("Xera's Boon",XerasBoon, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/0/04/Xera%27s_Boon.png"),
                new Buff("Xera's Fury",XerasFury, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/dd/Xera%27s_Fury.png"),
                new Buff("Statue Fixated",StatueFixated1, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Statue Fixated 2",StatueFixated2, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Incoming!",Incoming, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png"),
                new Buff("Crimson Attunement (Orb)",CrimsonAttunementOrb, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/3e/Crimson_Attunement.png"),
                new Buff("Radiant Attunement (Orb)",RadiantAttunementOrb, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/68/Radiant_Attunement.png"),
                new Buff("Crimson Attunement (Phantasm)",CrimsonAttunementPhantasm, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/3e/Crimson_Attunement.png"),
                new Buff("Radiant Attunement (Phantasm)",RadiantAttunementPhantasm, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/68/Radiant_Attunement.png"),
                new Buff("Magic Blast",MagicBlast, Source.FightSpecific, BuffStackType.Stacking, 35, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a9/Magic_Blast_Intensity.png"),
                new Buff("Gaining Power",GainingPower, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png"),            
                // Twisted Castle
                new Buff("Spatial Distortion", SpatialDistortion, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7a/Bloodstone_Blessed.png" ),
                new Buff("Madness", Madness, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/e/ee/Madness.png" ),
                new Buff("Still Waters", StillWaters, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/5c/Still_Waters_%28effect%29.png" ),
                new Buff("Soothing Waters", SoothingWaters, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/8f/Soothing_Waters.png" ),
                new Buff("Chaotic Haze", ChaoticHaze, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/48/Lava_Font.png" ),
                new Buff("Creeping Pursuit", CreepingPursuit, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/f/f8/Creeping_Pursuit.png" ),
                // Xera      
                new Buff("Derangement",Derangement, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/c/ca/Derangement.png"),
                new Buff("Bending Chaos",BendingChaos, Source.FightSpecific, BuffStackType.Stacking, 11, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/39/Target%21.png"),
                new Buff("Shifting Chaos",ShiftingChaos, Source.FightSpecific, BuffStackType.Stacking, 11, BuffClassification.Other, "https://wiki.guildwars2.com/images/0/04/Shifting_Chaos.png"),
                new Buff("Twisting Chaos",TwistingChaos, Source.FightSpecific, BuffStackType.Stacking, 11, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/60/Twisting_Chaos.png"),
                new Buff("Intervention",Intervention, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a2/Intervention_%28effect%29.png"),
                new Buff("Bloodstone Protection",BloodstoneProtection, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/4e/Bloodstone_Protection.png"),
                new Buff("Bloodstone Blessed",BloodstoneBlessed, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7a/Bloodstone_Blessed.png"),
                new Buff("Void Zone",VoidZone, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/56/Void_Zone.png"),
                new Buff("Gravity Well (Xera)",GravityWellXera, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/5f/Monster_Skill.png"),
                new Buff("Hero's Departure",HerosDeparture, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                new Buff("Hero's Return",HerosReturn, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/eb/Determined.png"),
                //////////////////////////////////////////////
                // Cairn        
                new Buff("Shared Agony", SharedAgony, Source.FightSpecific, BuffClassification.Debuff,"https://wiki.guildwars2.com/images/5/53/Shared_Agony.png"),
                new Buff("Enraged (Cairn)", EnragedCairn, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png"),
                new Buff("Unseen Burden", UnseenBurden, Source.FightSpecific, BuffStackType.Stacking , 99, BuffClassification.Debuff,"https://wiki.guildwars2.com/images/b/b9/Unseen_Burden.png"),
                new Buff("Countdown", Countdown, Source.FightSpecific, BuffStackType.Stacking , 10, BuffClassification.Other,"https://wiki.guildwars2.com/images/0/05/Countdown.png"),
                new Buff("Gaze Avoidance", GazeAvoidance, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/1/10/Gaze_Avoidance.png"),
                // MO             
                new Buff("Empowered", Empowered, Source.FightSpecific, BuffStackType.Stacking , 4, BuffClassification.Other,"https://wiki.guildwars2.com/images/9/9c/Empowered_%28Mursaat_Overseer%29.png"),
                new Buff("Mursaat Overseer's Shield", MursaatOverseersShield, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/8/84/Mursaat_Overseer%27s_Shield.png"),
                new Buff("Protect (SAK)", ProtectSAK, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/f/f6/Protect.png"),
                new Buff("Dispel (SAK)", DispelSAK, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/8/84/Mursaat_Overseer%27s_Shield.png"),
                new Buff("Claim (SAK)", ClaimSAK, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/ef/Claim.png"),
                // Samarog            
                new Buff("Fixated (Samarog)",FixatedSamarog, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Fixated (Guldhem)",FixatedGuldhem, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Fixated (Rigom)",FixatedRigom, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Inevitable Betrayal (Big)",InevitableBetrayalBig, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b8/Feeding_Frenzy_%28GW1%29.png"),
                new Buff("Inevitable Betrayal (Small)",InevitableBetrayalSmall, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b8/Feeding_Frenzy_%28GW1%29.png"),
                new Buff("Soul Swarm",SoulSwarm, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/0/0e/Soul_Swarm_%28effect%29.png"),
                // Deimos
                new Buff("Unnatural Signet",UnnaturalSignet, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/20/Unnatural_Signet.png"),
                new Buff("Weak Minded",WeakMinded, Source.FightSpecific, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/3/38/Unseen_Burden_%28Deimos%29.png"),
                new Buff("Tear Instability",TearInstability, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/11/Tear_Instability.png"),
                new Buff("Form Up and Advance!",FormUpAndAdvance, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/56/Form_Up_and_Advance%21.png"),
                new Buff("Devour", Devour, Source.FightSpecific, BuffStackType.Stacking , 99, BuffClassification.Other,"https://wiki.guildwars2.com/images/3/3d/Devour.png"),
                new Buff("Unseen Burden (Deimos)", UnseenBurdenDeimos, Source.FightSpecific, BuffStackType.Stacking , 99, BuffClassification.Other,"https://wiki.guildwars2.com/images/3/38/Unseen_Burden_%28Deimos%29.png"),
                //////////////////////////////////////////////
                // Soulless Horror
                new Buff("Exile's Embrace",ExilesEmbrace, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b4/Exile%27s_Embrace.png"),
                new Buff("Fixated (SH)",FixatedSH, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Necrosis",Necrosis, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/4/47/Ichor.png"),
                // River
                new Buff("Soul Siphon", SoulSiphon, Source.FightSpecific, BuffClassification.Debuff,"https://wiki.guildwars2.com/images/f/f7/Soul_Siphon.png"),
                new Buff("Desmina's Protection", DesminasProtection, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/b/b3/Desmina%27s_Protection.png"),
                new Buff("Follower's Asylum", FollowersAsylum, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/b/b3/Desmina%27s_Protection.png"),
                new Buff("Spirit Form", SpiritForm, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/2/2e/Spirit_Form_%28Hall_of_Chains%29.png"),
                new Buff("Mortal Coil (River)",MortalCoilRiver, Source.FightSpecific, BuffStackType.Stacking, 30, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/41/Mortal_Coil.png"),
                new Buff("Energy Threshold (River)", EnergyThresholdRiver, Source.FightSpecific, BuffStackType.Stacking, 5, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/2e/Spirit_Form_%28Hall_of_Chains%29.png"),
                // Broken King          
                new Buff("Frozen Wind", FrozenWind, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Debuff,"https://wiki.guildwars2.com/images/3/3a/Frozen_Wind.png"),
                new Buff("Shield of Ice", ShieldOfIce, Source.FightSpecific, BuffStackType.StackingConditionalLoss, 99, BuffClassification.Other,"https://wiki.guildwars2.com/images/3/38/Shield_of_Ice.png"),
                new Buff("Glaciate", Glaciate, Source.FightSpecific, BuffClassification.Debuff,"https://wiki.guildwars2.com/images/b/ba/Glaciate.png"),
                // Eater of Soul         
                new Buff("Soul Digestion", SoulDigestion, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/0/08/Soul_Digestion.png"),
                new Buff("Reclaimed Energy", ReclaimedEnergy, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/2/21/Reclaimed_Energy.png"),
                new Buff("Mortal Coil (Statue of Death)", MortalCoilStatueOfDeath, Source.FightSpecific, BuffStackType.Stacking, 30, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/41/Mortal_Coil.png"),
                new Buff("Empowered (Statue of Death)", EmpoweredStatueOfDeath, Source.FightSpecific, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/d/de/Empowered_%28Statue_of_Death%29.png"),
                //new Buff("Energy Threshold (Statue of Death)", 48583, Source.FightSpecific, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/2e/Spirit_Form_%28Hall_of_Chains%29.png"),
                //  Eyes
                new Buff("Last Grasp (Judgment)", LastGraspJudgment, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/26/Last_Grasp.png"),
                new Buff("Last Grasp (Fate)", LastGraspFate, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/26/Last_Grasp.png"),
                new Buff("Exposed (Statue of Darkness)", ExposedStatueOfDarkness, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/42/Exposed_%28Statue_of_Darkness%29.png"),
                new Buff("Light Carrier", LightCarrier, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/f/f1/Torch_Fielder.png"),
                new Buff("Empowered (Light Thief)", EmpoweredLightThief, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/0/08/Soul_Digestion.png"),
                // Dhuum
                new Buff("Mortal Coil (Dhuum)", MortalCoilDhuum, Source.FightSpecific, BuffStackType.Stacking, 30, BuffClassification.Other,"https://wiki.guildwars2.com/images/4/48/Compromised.png"),
                new Buff("Fractured Spirit", FracturedSpirit, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/c/c3/Fractured_Spirit.png"),
                new Buff("Residual Affliction", ResidualAffliction, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/1/12/Residual_Affliction.png"),
                new Buff("Arcing Affliction", ArcingAffliction, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/f/f0/Arcing_Affliction.png"),
                new Buff("One-Track Mind", OneTrackMind, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/68/Tracked.png"),
                new Buff("Imminent Demise", ImminentDemise, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/5/58/Superheated_Metal.png"),
                new Buff("Lethal Report", LethalReport, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/0/02/Mantra_of_Signets.png"),
                new Buff("Hastened Demise", HastenedDemise, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/5/5b/Hastened_Demise.png"),
                new Buff("Echo's Pick up", EchosPickup, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/4/45/Unstable.png"),
                new Buff("Energy Threshold (Dhuum)", EnergyThresholdDhuum, Source.FightSpecific, BuffStackType.Stacking, 5, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/2e/Spirit_Form_%28Hall_of_Chains%29.png"),
                //////////////////////////////////////////////
                // CA
                new Buff("Greatsword Power",GreatswordPower , Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/3b/Greatsword_Power_%28effect%29.png"),
                new Buff("Fractured - Enemy",FracturedEnemy, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Branded_Aura.png"),
                new Buff("Fractured - Allied",FracturedAllied, Source.FightSpecific, BuffStackType.Stacking, 2, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Branded_Aura.png"),
                new Buff("Conjured Shield",ConjuredShield , Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/83/Conjured_Shield_%28effect%29.png"),
                new Buff("Conjured Protection",ConjuredProtection , Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/83/Bloodstone-Infused_shield.png"),
                new Buff("Shielded",Shielded , Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/47/Golem-Powered_Shielding.png"),
                new Buff("Augmented Power",AugmentedPower  , Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/47/Golem-Powered_Shielding.png"),
                new Buff("Locked On",LockedOn  , Source.FightSpecific, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/3/39/Target%21.png"),
                new Buff("CA Invul",CAInvul , Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Arm Up",ArmUp , Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d3/Blood_Fueled.png"),
                new Buff("Fixation", Fixation, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                // Twin Largos
                new Buff("Aquatic Detainment", AquaticDetainmentEffect , Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/5f/Monster_Skill.png"),
                new Buff("Tidal Pool", TidalPool , Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/5f/Monster_Skill.png"),
                new Buff("Aquatic Aura (Kenut)",AquaticAuraKenut , Source.FightSpecific, BuffStackType.Stacking, 80, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/44/Expose_Weakness.png"),
                new Buff("Aquatic Aura (Nikare)",AquaticAuraNikare , Source.FightSpecific, BuffStackType.Stacking, 80, BuffClassification.Other, "https://wiki.guildwars2.com/images/f/fd/Fractured_%28effect%29.png"),
                new Buff("Waterlogged",Waterlogged , Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/8/89/Waterlogged.png"),
                new Buff("Enraged (Twin Largos)", EnragedTwinLargos, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png"),
                // Qadim
                new Buff("Flame Armor",FlameArmor, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/e7/Magma_Orb.png"),
                new Buff("Fiery Surge",FierySurge, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Other, "https://wiki.guildwars2.com/images/f/f9/Fiery_Surge.png"),
                new Buff("Power of the Lamp", PowerOfTheLamp, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/e5/Break_Out%21.png"),
                new Buff("Unbearable Flames", UnbearableFlames, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/2/21/Expel_Excess_Magic_Poison.png"),
                new Buff("Parry", Parry, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/2/28/Parry_%28effect%29.png"),
                new Buff("Mythwright Surge", MythwrightSurge, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/7/7a/Swiftness_%28effect%29.png"),
                new Buff("Lamp Bond", LampBond, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/d/db/Lamp_Bond.png"),
                new Buff("Enraged (Wywern)", EnragedWywern, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png"),
                new Buff("Enraged (Qadim)", EnragedQadim, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png"),
                new Buff("Resistance (Lava Elemental)", ResistanceLavaElemental, Source.FightSpecific, BuffStackType.Queue, 5, BuffClassification.Other,"https://wiki.guildwars2.com/images/1/18/Fire_Shield.png"),
                new Buff("Shielded (Lava Elemental)", ShieldedLavaElemental, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/1/18/Fire_Shield.png"),
                //////////////////////////////////////////////
                // Adina
                new Buff("Pillar Pandemonium", PillarPandemonium, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Other,"https://wiki.guildwars2.com/images/d/d9/Captain%27s_Inspiration.png"),
                new Buff("Radiant Blindness", RadiantBlindness, Source.FightSpecific, BuffStackType.Queue, 25, BuffClassification.Debuff,"https://wiki.guildwars2.com/images/c/c9/Persistently_Blinded.png"),
                new Buff("Diamond Palisade (Damage)", DiamondPalisadeDamage, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/5/5f/Monster_Skill.png"),
                new Buff("Diamond Palisade", DiamondPalisade, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/5/5f/Monster_Skill.png"),
                new Buff("Eroding Curse", ErodingCurse, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Other,"https://wiki.guildwars2.com/images/d/de/Toxic_Gas.png"),
                // Sabir
                new Buff("Ion Shield", IonShield, Source.FightSpecific, BuffStackType.Stacking, 80, BuffClassification.Other,"https://wiki.guildwars2.com/images/9/94/Ion_Shield.png"),
                new Buff("Violent Currents", ViolentCurrents, Source.FightSpecific, BuffStackType.Stacking, 5, BuffClassification.Offensive,"https://wiki.guildwars2.com/images/0/06/Violent_Currents.png"),
                new Buff("Repulsion Field", RepulsionField, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Electrical Repulsion", ElectricalRepulsion, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/d/dd/Xera%27s_Fury.png"),
                new Buff("Electro-Repulsion", ElectroRepulsion, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/4/42/Exposed_%28Statue_of_Darkness%29.png"),
                new Buff("Eye of the Storm", EyeOfTheStorm, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/5/52/Mending_Waters_%28effect%29.png"),
                new Buff("Bolt Break", BoltBreak, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/7/74/Mesmer_icon_white.png"),
                // Peerless Qadim
                new Buff("Erratic Energy", ErraticEnergy, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other,"https://wiki.guildwars2.com/images/4/45/Unstable.png"),
                new Buff("Power Share", PowerShare, Source.FightSpecific, BuffStackType.Stacking, 3, BuffClassification.Other,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Sapping Surge", SappingSurge, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/6f/Guilt_Exploitation.png"),
                new Buff("Chaos Corrosion", ChaosCorrosion, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/f/fd/Fractured_%28effect%29.png"),
                new Buff("Fixated (Qadim the Peerless)", FixatedQadimThePeerless, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Magma Drop", MagmaDrop, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/2/24/Targeted.png"),
                new Buff("Critical Mass", CriticalMass, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/b/bf/Orb_of_Ascension_%28effect%29.png"),
                new Buff("Kinetic Abundance", KineticAbundance, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/64/Kinetic_Abundance.png"),
                new Buff("Enfeebled Force", EnfeebledForce, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/b/b6/Enfeebled_Force.png"),
                new Buff("Backlashing Beam", BacklashingBeam, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/0/04/Xera%27s_Boon.png"),
                new Buff("Clutched by Chaos", ClutchedbyChaos, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/8/87/Protective_Shadow.png"),
                new Buff("Cleansed Conductor", CleansedConductor, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/a/a9/Magic_Blast_Intensity.png"),
                new Buff("Poisoned Power", PoisonedPower, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png"),
                new Buff("Incorporeal", Incorporeal, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/8/8b/Magic_Aura.png"),
                new Buff("Flare-Up", FlareUp, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other,"https://wiki.guildwars2.com/images/8/8b/Magic_Aura.png"),
                new Buff("Unbridled Chaos", UnbridledChaos, Source.FightSpecific, BuffStackType.Stacking, 3, BuffClassification.Other,"https://wiki.guildwars2.com/images/4/42/Exposed_%28Statue_of_Darkness%29.png"),
                //////////////////////////////////////////////
                // Fractals 
                new Buff("Rigorous Certainty", RigorousCertainty, Source.Common, BuffClassification.Defensive,"https://wiki.guildwars2.com/images/6/60/Desert_Carapace.png"),
                new Buff("Fractal Savant", FractalSavant, Source.Common, BuffClassification.Offensive,"https://wiki.guildwars2.com/images/c/cb/Malign_9_Agony_Infusion.png"),
                new Buff("Fractal Prodigy", FractalProdigy, Source.Common, BuffClassification.Offensive,"https://wiki.guildwars2.com/images/1/11/Mighty_9_Agony_Infusion.png"),
                new Buff("Fractal Champion", FractalChampion, Source.Common, BuffClassification.Offensive,"https://wiki.guildwars2.com/images/3/3d/Precise_9_Agony_Infusion.png"),
                new Buff("Fractal God", FractalGod, Source.Common, BuffClassification.Offensive,"https://wiki.guildwars2.com/images/2/22/Healing_9_Agony_Infusion.png"),
                // Siax 
                new Buff("Fixated (Nightmare)", FixatedNightmare, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                // Ensolyss 
                new Buff("Determination (Ensolyss)", DeterminationEnsolyss, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/4/41/Gambit_Exhausted.png"),
                // Artsariiv
                new Buff("Enraged (Fractal)", EnragedFractal, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png"),
                new Buff("Corporeal Reassignment", CorporealReassignment, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/9/94/Redirect_Anomaly.png"),
                new Buff("Blinding Radiance", BlindingRadiance, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/5/5f/Monster_Skill.png"),
                new Buff("Determination (Viirastra)", DeterminationViirastra, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/4/41/Gambit_Exhausted.png"),
                // Arkk 
                new Buff("Fixated (Bloom 3)", FixatedBloom3, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Fixated (Bloom 2)", FixatedBloom2, Source.FightSpecific, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Fixated (Bloom 1)", FixatedBloom1, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Fixated (Bloom 4)", FixatedBloom4, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Cosmic Meteor", CosmicMeteor, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/5f/Monster_Skill.png"),
                new Buff("Diaphanous Shielding", DiaphanousShielding, Source.FightSpecific, BuffStackType.Stacking, 4, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/57/Diaphanous_Shielding.png"),
                new Buff("Electrocuted", Electrocuted, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/91/Air_Attunement.png"),
                // Ai, Keeper of the Peak
                new Buff("Tidal Barrier", TidalBarrier, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b1/Primed_Bottle.png"),
                new Buff("Whirlwind Shield", WhirlwindShield, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b1/Primed_Bottle.png"),
                new Buff("Resilient Form", ResilientForm, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/13/Crowd_Favor.png"),
                new Buff("Cacophonous Mind", CacophonousMind, Source.FightSpecific, BuffStackType.Stacking, 20, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/13/Crowd_Favor.png"),
                new Buff("Crushing Guilt", CrushingGuilt, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/1/13/Crowd_Favor.png"),
                new Buff("Fixated (Fear 3)", FixatedFear3, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Fixated (Fear 2)", FixatedFear2, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Fixated (Fear 1)", FixatedFear1, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Fixated (Fear 4)", FixatedFear4, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Charged Leap", ChargedLeap, Source.FightSpecific, BuffStackType.StackingConditionalLoss, 3, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/13/Crowd_Favor.png"),
                new Buff("Tidal Bargain", TidalBargain, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/1/13/Crowd_Favor.png"),
                //////////////////////////////////////////////
                // Icebrood
                new Buff("Hypothermia", Hypothermia, Source.FightSpecific, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/d/d5/Hypothermia_%28story_effect%29.png" ),
                // Fraenir of Jormag
                new Buff("Frozen", Frozen, Source.FightSpecific, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/6/6a/Frostbite_%28Bitterfrost_Frontier%29.png" ),
                new Buff("Snowblind", Snowblind, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/6a/Frostbite_%28Bitterfrost_Frontier%29.png" ),
                // Voice and Claw            
                new Buff("Enraged (V&C)", EnragedVC, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                // Boneskinner     
                new Buff("Tormenting Aura", TormentingAura, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c0/Darkness.png" ),
                // Whisper of Jormag
                new Buff("Whisper Teleport Out", WhisperTeleportOut, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Whisper Teleport Back", WhisperTeleportBack, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Vengeance_%28Mordrem%29.png" ),
                new Buff("Frigid Vortex", FrigidVortex, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Chains of Frost Active", ChainsOfFrostActive, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/63/Use_Soul_Binder.png" ),
                new Buff("Chains of Frost Application", ChainsOfFrostApplication, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/5f/Monster_Skill.png" ),
                new Buff("Brain Freeze", BrainFreeze, Source.FightSpecific, BuffStackType.Stacking, 20, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/6a/Frostbite_%28Bitterfrost_Frontier%29.png" ),
                // Frezie      
                new Buff("Icy Barrier", IcyBarrier, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/38/Shield_of_Ice.png" ),
                // Mai Trin
                new Buff("Shared Destruction (Mai Trin)", SharedDestructionMaiTrin, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/5f/Monster_Skill.png" ),
                new Buff("Shared Destruction CM (Mai Trin)", SharedDestructionMaiTrinCM, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/5f/Monster_Skill.png" ),
                new Buff("Magnetic Bomb", MagneticBomb, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Immune to damage and conditions.", ImmuneToDamageAndConditions, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                //new Buff("Mai Trin ???", 63858, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Mai Trin ?????", MaiTrinSomething, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Chaos and Destruction", ChaosAndDestruction, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/5f/Monster_Skill.png" ),
                new Buff("Exposed (EOD Strike)", ExposedEODStrike, Source.Common, BuffStackType.Stacking, 10, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/6/6b/Exposed.png" ),
                new Buff("Photon Saturation", PhotonSaturation, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/51/Photon_Saturation.png" ),
                // Ankka
                new Buff("Necrotic Ritual", NecroticRitual, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Ankka ???", AnkkaPlateformChanging, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Hallucinations", Hallucinations, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Energy Transfer", EnergyTransfer, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Death's Embrace", DeathsEmbraceEffect, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/5f/Monster_Skill.png" ),
                new Buff("Death's Embrace 2", DeathsEmbrace2Effect, Source.FightSpecific, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/5f/Monster_Skill.png" ),
                new Buff("Power of the Void", PowerOfTheVoid, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/0/06/Power_of_the_Void.png" ),
                // Minister Li   
                new Buff("Target Order: 1", TargetOrder1, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Target Order: 2", TargetOrder2, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Target Order: 3", TargetOrder3, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Target Order: 4", TargetOrder4, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Target Order: 5", TargetOrder5, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Stronger Together", StrongerTogether, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/4c/Giant_Growth.png" ),
                new Buff("Vitality Equalizer 1", VitalityEqualizer1, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/82/Synchronized_Vitality.png" ),
                new Buff("Vitality Equalizer CM", VitalityEqualizerCM, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/82/Synchronized_Vitality.png" ),
                new Buff("Vitality Equalizer 2", VitalityEqualizer2, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/82/Synchronized_Vitality.png" ),
                new Buff("Destructive Aura", DestructiveAura, Source.FightSpecific, BuffStackType.StackingConditionalLoss, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/de/Toxic_Gas.png" ),
                new Buff("Equalization Matrix", EqualizationMatrix, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Other, "https://wiki.guildwars2.com/images/f/fc/Equalization_Matrix.png" ),
                new Buff("Lethal Inspiration", LethalInspiration, Source.FightSpecific, BuffStackType.Stacking, 1, BuffClassification.Other, "https://wiki.guildwars2.com/images/0/06/Power_of_the_Void.png" ),
                new Buff("Extreme Vulnerability", ExtremeVulnerability, Source.FightSpecific, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/c/c1/Extreme_Vulnerability.png" ),
                new Buff("Fixated (Kaineng Overlook)",FixatedKainengOverlook, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/66/Fixated.png"),
                new Buff("Shared Destruction (Li)",SharedDestructionLi, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png"),
                new Buff("Shared Destruction (Li CM)",SharedDestructionLiCM, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png"),
                new Buff("Debilitated",Debilitated, Source.FightSpecific, BuffStackType.Stacking, 3, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/8/80/Debilitated.png"),
                new Buff("Infirmity",Infirmity, Source.FightSpecific, BuffStackType.Stacking, 4, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/b/bb/Debilitating_Void.png"),
                //Harvest Temple
                new Buff("Influence of the Void", InfluenceOfTheVoidEffect, Source.FightSpecific, BuffStackType.Stacking, 20, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/61/Throw_Cursed_Artifact.png" ),
                new Buff("Targeted (Dragon Void)", TargetedDragonVoid, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/66/Fixated.png" ),
                new Buff("Void Repulsion 1", VoidRepulsion1, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Void Repulsion 2", VoidRepulsion2, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Aerial Defense", AerialDefense, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Void Immunity", VoidImmunity, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Void Empowerment", VoidEmpowerment, Source.FightSpecific, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                new Buff("Void Shell", VoidShell, Source.FightSpecific, BuffStackType.Stacking, 3, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/65/Windfall.png" ),
                //Open World Soo-Won
                new Buff("Jade Tech Offensive Overcharge", JadeTechOffensiveOvercharge, Source.FightSpecific, BuffStackType.Queue, 9, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/d/d2/Jade_Tech_Offensive_Overcharge.png"),
                new Buff("Jade Tech Defensive Overcharge", JadeTechDefensiveOvercharge, Source.FightSpecific, BuffStackType.Queue, 9, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/4/4e/Jade_Tech_Defensive_Overcharge.png"),
                new Buff("Enhancement (+5% Damage vs. Dragonvoid)", EnhancementDragonsEnd, Source.FightSpecific, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/2/23/Nourishment_utility.png"),
                new Buff("Soul Reunited", SoulReunited, Source.FightSpecific, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/b/b8/Ally%27s_Aid_Powered_Up.png"),
                new Buff("Wisp Form", WispForm, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c3/Fractured_Spirit.png"),
                new Buff("Void Corruption", VoidCorruption, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/b/b8/Void_Corruption.png"),
                new Buff("Void Chaos", VoidChaos, Source.FightSpecific, BuffClassification.Debuff, "https://wiki.guildwars2.com/images/7/70/Spectral_Agony.png"),
                new Buff("Hardened Shell", HardenedShell, Source.FightSpecific, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a2/Defensive_Inspiration.png"),
                new Buff("Dragon's End Contributor 1", DragonsEndContributor1, Source.FightSpecific, BuffClassification.Support, "https://wiki.guildwars2.com/images/a/ad/Seraph_Morale_01.png"),
                new Buff("Dragon's End Contributor 2", DragonsEndContributor2, Source.FightSpecific, BuffClassification.Support, "https://wiki.guildwars2.com/images/6/6b/Seraph_Morale_02.png"),
                new Buff("Dragon's End Contributor 3", DragonsEndContributor3, Source.FightSpecific, BuffClassification.Support, "https://wiki.guildwars2.com/images/3/30/Seraph_Morale_03.png"),
                new Buff("Dragon's End Contributor 4", DragonsEndContributor4, Source.FightSpecific, BuffClassification.Support, "https://wiki.guildwars2.com/images/5/51/Seraph_Morale_04.png"),
                new Buff("Dragon's End Contributor 5", DragonsEndContributor5, Source.FightSpecific, BuffClassification.Support, "https://wiki.guildwars2.com/images/9/90/Seraph_Morale_05.png"),
                new Buff("Dragon's End Contributor 6", DragonsEndContributor6, Source.FightSpecific, BuffClassification.Support, "https://wiki.guildwars2.com/images/0/06/Seraph_Morale_06.png"),
                new Buff("Dragon's End Contributor 7", DragonsEndContributor7, Source.FightSpecific, BuffClassification.Support, "https://wiki.guildwars2.com/images/1/1a/Seraph_Morale_07.png"),
                new Buff("Dragon's End Contributor 8", DragonsEndContributor8, Source.FightSpecific, BuffClassification.Support, "https://wiki.guildwars2.com/images/0/0a/Seraph_Morale_08.png"),
                new Buff("Dragon's End Contributor 9", DragonsEndContributor9, Source.FightSpecific, BuffClassification.Support, "https://wiki.guildwars2.com/images/9/9e/Seraph_Morale_09.png"),
                new Buff("Dragon's End Contributor 10", DragonsEndContributor10, Source.FightSpecific, BuffClassification.Support, "https://wiki.guildwars2.com/images/7/7b/Seraph_Morale_10.png"),
        };

        internal static readonly List<Buff> NormalFoods = new List<Buff>
        {
                new Buff("Malnourished",Malnourished, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/6/67/Malnourished.png"),
                new Buff("Plate of Truffle Steak",PlateOfTruffleSteak, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/4/4c/Plate_of_Truffle_Steak.png"),
                new Buff("Bowl of Sweet and Spicy Butternut Squash Soup",BowlOfSweetAndSpicyButternutSquashSoup, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/d/df/Bowl_of_Sweet_and_Spicy_Butternut_Squash_Soup.png"),
                new Buff("Bowl Curry Butternut Squash Soup",BowlCurryButternutSquashSoup, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/1/16/Bowl_of_Curry_Butternut_Squash_Soup.png"),
                new Buff("Red-Lentil Saobosa",RedLentilSaobosa, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/a/a8/Red-Lentil_Saobosa.png"),
                new Buff("Super Veggie Pizza",SuperVeggiePizza, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/6/66/Super_Veggie_Pizza.png"),
                new Buff("Rare Veggie Pizza",RareVeggiePizza, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/a/a0/Rare_Veggie_Pizza.png"),
                new Buff("Bowl of Garlic Kale Sautee",Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/0/04/Bowl_of_Garlic_Kale_Sautee.png"),
                new Buff("Koi Cake",Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/1/14/Koi_Cake.png"),
                new Buff("Prickly Pear Pie",PricklyPearPie, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/0/0a/Prickly_Pear_Pie.png"),
                new Buff("Bowl of Nopalitos Sauté",Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/f/f1/Bowl_of_Nopalitos_Saut%C3%A9.png"),
                new Buff("Loaf of Candy Cactus Cornbread",LoafOfCandyCactusCornbread, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/b/b2/Loaf_of_Candy_Cactus_Cornbread.png"),
                new Buff("Delicious Rice Ball",DeliciousRiceBall, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/5d/Delicious_Rice_Ball.png"),
                new Buff("Slice of Allspice Cake",SliceOfAllspiceCake, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/1/13/Slice_of_Allspice_Cake.png"),
                new Buff("Fried Golden Dumpling",FriedGoldenDumpling, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/1/14/Fried_Golden_Dumpling.png"),
                new Buff("Bowl of Seaweed Salad",BowlOfSeaweedSalad, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/1/1c/Bowl_of_Seaweed_Salad.png"),
                new Buff("Bowl of Orrian Truffle and Meat Stew",BowlOfOrrianTruffleAndMeatStew, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/b/b8/Bowl_of_Orrian_Truffle_and_Meat_Stew.png"),
                new Buff("Plate of Mussels Gnashblade",PlateOfMusselsGnashblade, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/7/7b/Plate_of_Mussels_Gnashblade.png"),
                new Buff("Spring Roll",SpringRoll, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/d/da/Spring_Roll.png"),
                new Buff("Plate of Beef Rendang",PlateOfBeefRendang, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/d/d0/Plate_of_Beef_Rendang.png"),
                new Buff("Dragon's Revelry Starcake",DragonsRevelryStarcake, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/2/2b/Dragon%27s_Revelry_Starcake.png"),
                new Buff("Avocado Smoothie",AvocadoSmoothie, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/8/83/Avocado_Smoothie.png"),
                new Buff("Carrot Souffle",Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/7/71/Carrot_Souffl%C3%A9.png"), //same as Dragon's_Breath_Bun
                new Buff("Plate of Truffle Steak Dinner",Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/9/92/Plate_of_Truffle_Steak_Dinner.png"), //same as Dragon's Breath Bun
                new Buff("Dragon's Breath Bun",DragonsBreathBun, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/53/Dragon%27s_Breath_Bun.png"),
                new Buff("Karka Egg Omelet",KarkaEggOmelet, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/9/9e/Karka_Egg_Omelet.png"),
                new Buff("Steamed Red Dumpling",SteamedRedDumpling, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/8/8c/Steamed_Red_Dumpling.png"),
                new Buff("Saffron Stuffed Mushroom",Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/e/e2/Saffron_Stuffed_Mushroom.png"), //same as Karka Egg Omelet
                new Buff("Soul Pastry",SoulPastry, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/2/2c/Soul_Pastry.png"),
                new Buff("Bowl of Fire Meat Chili",BowlOfFireMeatChili, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/b/ba/Bowl_of_Fire_Meat_Chili.png"),
                new Buff("Plate of Fire Flank Steak",PlateOfFireFlankSteak, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/2/27/Plate_of_Fire_Flank_Steak.png"),
                new Buff("Plate of Orrian Steak Frittes",PlateOfOrrianSteakFrittes, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/4/4d/Plate_of_Orrian_Steak_Frittes.png"),
                new Buff("Spicier Flank Steak",SpicierFlankSteak, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/0/01/Spicier_Flank_Steak.png"),
                new Buff("Mango Pie",MangoPie, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/3/3d/Mango_Pie.png"),
                new Buff("Block of Tofu", BlockOfTofu, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/f/fa/Block_of_Tofu.png"),
                new Buff("Fishy Rice Bowl", FishyRiceBowl, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/1/14/Fishy_Rice_Bowl.png"),
                new Buff("Meaty Rice Bowl", MeatyRiceBowl, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/6/65/Meaty_Rice_Bowl.png"),
                new Buff("Plate of Kimchi Pancakes", PlateOfKimchiPancakes, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/3/34/Plate_of_Kimchi_Pancakes.png"),
                new Buff("Bowl of Kimchi Tofu Stew", BowlOfKimchiTofuStew, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/2/22/Bowl_of_Kimchi_Tofu_Stew.png"),
                new Buff("Meaty Asparagus Skewer", MeatyAsparagusSkewer, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/d/d6/Meaty_Asparagus_Skewer.png"),
        };

        internal static readonly List<Buff> Utilities = new List<Buff>
        {     
                // UTILITIES 
                // 1h versions have the same ID as 30 min versions 
                new Buff("Diminished",Diminished, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/7/71/Diminished.png"),
                new Buff("Rough Sharpening Stone", RoughSharpeningStone, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/1/14/Rough_Sharpening_Stone.png"),
                new Buff("Simple Sharpening Stone", SimpleSharpeningStone, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/e/ef/Simple_Sharpening_Stone.png"),
                new Buff("Standard Sharpening Stone", StandardSharpeningStone, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/8/82/Standard_Sharpening_Stone.png"),
                new Buff("Quality Sharpening Stone", QualitySharpeningStone, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/6/68/Quality_Sharpening_Stone.png"),
                new Buff("Hardened Sharpening Stone", HardenedSharpeningStone, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/8/8d/Hardened_Sharpening_Stone.png"),
                new Buff("Superior Sharpening Stone", SuperiorSharpeningStone, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                //new Buff("Ogre Sharpening Stone", 9963, ParseHelper.Source.Item, BuffType.Duration, 1, BuffNature.Consumable, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                new Buff("Apprentice Maintenance Oil", ApprenticeMaintenanceOil, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/51/Apprentice_Maintenance_Oil.png"),
                new Buff("Journeyman Maintenance Oil", JourneymanMaintenanceOil, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/b/b1/Journeyman_Maintenance_Oil.png"),
                new Buff("Standard Maintenance Oil", StandardMaintenanceOil, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/a/a6/Standard_Maintenance_Oil.png"),
                new Buff("Artisan Maintenance Oil", ArtisanMaintenanceOil, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/7/75/Artisan_Maintenance_Oil.png"),
                new Buff("Quality Maintenance Oil", QualityMaintenanceOil, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/2/23/Quality_Maintenance_Oil.png"),
                new Buff("Master Maintenance Oil", MasterMaintenanceOil, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                //new Buff("Hylek Maintenance Oil", 9968, ParseHelper.Source.Item, BuffType.Duration, 1, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Buff("Apprentice Tuning Crystal", ApprenticeTuningCrystal, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/7/7d/Apprentice_Tuning_Crystal.png"),
                new Buff("Journeyman Tuning Crystal", JourneymanTuningCrystal, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/1/1e/Journeyman_Tuning_Crystal.png"),
                new Buff("Standard Tuning Crystal", StandardTuningCrystal, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/1/1e/Standard_Tuning_Crystal.png"),
                new Buff("Artisan Tuning Crystal", ArtisanTuningCrystal, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/e/eb/Artisan_Tuning_Crystal.png"),
                new Buff("Quality Tuning Crystal", QualityTuningCrystal, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/3/3b/Quality_Tuning_Crystal.png"),
                new Buff("Master Tuning Crystal", MasterTuningCrystal, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/58/Master_Tuning_Crystal.png"),
                //new Buff("Krait Tuning Crystal", 9967, ParseHelper.Source.Item, BuffType.Duration, 1, BuffNature.Consumable, "https://wiki.guildwars2.com/images/5/58/Master_Tuning_Crystal.png"),
                new Buff("Compact Hardened Sharpening Stone", CompactHardenedSharpeningStone, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/1/1f/Compact_Hardened_Sharpening_Stone.png"),
                new Buff("Tin of Fruitcake", TinOfFruitcake, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/a/af/Tin_of_Fruitcake.png"),
                new Buff("Bountiful Sharpening Stone", BountifulSharpeningStone, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                new Buff("Toxic Sharpening Stone", ToxicSharpeningStone, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/d/db/Toxic_Sharpening_Stone.png"),
                new Buff("Magnanimous Sharpening Stone", MagnanimousSharpeningStone, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/a/aa/Magnanimous_Sharpening_Stone.png"),
                new Buff("Corsair Sharpening Stone", CorsairSharpeningStone, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/0/06/Corsair_Sharpening_Stone.png"),
                new Buff("Furious Sharpening Stone", FuriousSharpeningStone, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/7/78/Superior_Sharpening_Stone.png"),
                new Buff("Holographic Super Cheese", HolographicSuperCheese, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/f/fa/Holographic_Super_Cheese.png"),
                new Buff("Compact Quality Maintenance Oil", CompactQualityMaintenanceOil, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/d/d8/Compact_Quality_Maintenance_Oil.png"),
                new Buff("Peppermint Oil", PeppermintOil, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/b/bc/Peppermint_Oil.png"),
                new Buff("Toxic Maintenance Oil", ToxicMaintenanceOil, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/a/a6/Toxic_Maintenance_Oil.png"),
                new Buff("Magnanimous Maintenance Oil", MagnanimousMaintenanceOil, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/53/Magnanimous_Maintenance_Oil.png"),
                new Buff("Enhanced Lucent Oil", EnhancedLucentOil, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/e/ee/Enhanced_Lucent_Oil.png"),
                new Buff("Potent Lucent Oil", PotentLucentOil, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/1/16/Potent_Lucent_Oil.png"),
                new Buff("Corsair Maintenance Oil", CorsairMaintenanceOil, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/53/Magnanimous_Maintenance_Oil.png"),
                new Buff("Furious Maintenance Oil", FuriousMaintenanceOil, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Buff("Holographic Super Drumstick", HolographicSuperDrumstick, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/1/1d/Holographic_Super_Drumstick.png"),
                new Buff("Bountiful Maintenance Oil", BountifulMaintenanceOil, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/5b/Master_Maintenance_Oil.png"),
                new Buff("Compact Quality Tuning Crystal", CompactQualityTuningCrystal, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/b/ba/Compact_Quality_Tuning_Crystal.png"),
                new Buff("Tuning Icicle", TuningIcicle, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/7/75/Tuning_Icicle.png"),
                new Buff("Bountiful Tuning Crystal", BountifulTuningCrystal, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/58/Master_Tuning_Crystal.png"),
                new Buff("Toxic Focusing Crystal", ToxicFocusingCrystal, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/d/de/Toxic_Focusing_Crystal.png"),
                new Buff("Magnanimous Tuning Crystal", MagnanimousTuningCrystal, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/2/23/Magnanimous_Tuning_Crystal.png"),
                new Buff("Furious Tuning Crystal", FuriousTuningCrystal, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/58/Master_Tuning_Crystal.png"),
                new Buff("Corsair Tuning Crystal", CorsairTuningCrystal, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/f/f7/Corsair_Tuning_Crystal.png"),
                new Buff("Holographic Super Apple", HolographicSuperApple, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/e/ee/Holographic_Super_Apple.png"),
                new Buff("Sharpening Skull", SharpeningSkull, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/e/ee/Sharpening_Skull.png"),
                new Buff("Flask of Pumpkin Oil", FlaskOfPumpkinOil, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/7/71/Flask_of_Pumpkin_Oil.png"),
                new Buff("Lump of Crystallized Nougat", LumpOfCrystallizedNougat, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/8/8f/Lump_of_Crystallized_Nougat.png"),
                new Buff("Skale Venom (Consumable)", SkaleVenomConsumable, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/9/96/Skale_Venom_%28consumable%29.png"),
                new Buff("Swift Moa Feather", SwiftMoaFeather, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/f/f0/Swift_Moa_Feather.png"),
                //
                new Buff("Reinforced Armor", ReinforcedArmor, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/8/83/Reinforced_Armor.png", GW2Builds.June2022Balance, GW2Builds.EndOfLife),
        };

        internal static readonly List<Buff> Writs = new List<Buff>
        {

                new Buff("Writ of Basic Strength", WritOfBasicStrength, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/7/7e/Writ_of_Basic_Strength.png"),
                new Buff("Writ of Strength", WritOfStrength, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/5e/Writ_of_Strength.png"),
                new Buff("Writ of Studied Strength", WritOfStudiedStrength, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/2/23/Writ_of_Studied_Strength.png"),
                new Buff("Writ of Calculated Strength", WritOfCalculatedStrength, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/8/8d/Writ_of_Calculated_Strength.png"),
                new Buff("Writ of Learned Strength", WritOfLearnedStrength, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/8/8d/Writ_of_Calculated_Strength.png"),
                new Buff("Writ of Masterful Strength", WritOfMasterfulStrength, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/2/2b/Writ_of_Masterful_Strength.png"),
                new Buff("Writ of Basic Accuracy", WritOfBasicAccuracy, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/8/82/Writ_of_Basic_Accuracy.png"),
                new Buff("Writ of Accuracy", WritOfAccuracy, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/2/29/Writ_of_Accuracy.png"),
                new Buff("Writ of Studied Accuracy", WritOfStudiedAccuracy, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/a/ad/Writ_of_Studied_Accuracy.png"),
                new Buff("Writ of Calculated Accuracy", WritOfCalculatedAccuracy, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/2/27/Writ_of_Calculated_Accuracy.png"),
                new Buff("Writ of Learned Accuracy", WritOfLearnedAccuracy, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/5a/Writ_of_Masterful_Accuracy.png"),
                new Buff("Writ of Masterful Accuracy", WritOfMasterfulAccuracy, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/5a/Writ_of_Masterful_Accuracy.png"),
                new Buff("Writ of Basic Malice", WritOfBasicMalice, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/9/9e/Writ_of_Basic_Malice.png"),
                new Buff("Writ of Malice", WritOfMalice, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/c/c4/Writ_of_Malice.png"),
                new Buff("Writ of Studied Malice", WritOfStudiedMalice, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/b/bd/Writ_of_Studied_Malice.png"),
                new Buff("Writ of Calculated Malice", WritOfCalculatedMalice, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/7/75/Writ_of_Calculated_Malice.png"),
                new Buff("Writ of Learned Malice", WritOfLearnedMalice, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/9/9b/Writ_of_Learned_Malice.png"),
                new Buff("Writ of Masterful Malice", WritOfMasterfulMalice, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/2/20/Writ_of_Masterful_Malice.png"),
                new Buff("Writ of Basic Speed", WritOfBasicSpeed, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/e/e6/Writ_of_Basic_Speed.png"),
                new Buff("Writ of Studied Speed", WritOfStudiedSpeed, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/d/d1/Recipe_sheet_fine_boots.png"),
                new Buff("Writ of Masterful Speed", WritOfMasterfulSpeed, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/8/8e/Recipe_sheet_masterwork_boots.png"),
        };

        internal static readonly List<Buff> Potions = new List<Buff>
        {
                new Buff("Potion Of Karka Toughness", PotionOfKarkaToughness, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/f/fb/Powerful_Potion_of_Inquest_Slaying.png"),
                // Slaying Potions
                new Buff("Powerful Potion of Flame Legion Slaying",PowerfulPotionOfFlameLegionSlaying, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/e/e2/Powerful_Potion_of_Flame_Legion_Slaying.png"),
                new Buff("Powerful Potion of Halloween Slaying",PowerfulPotionOfHalloweenSlaying, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/f/fe/Powerful_Potion_of_Halloween_Slaying.png"),
                new Buff("Powerful Potion of Centaur Slaying",PowerfulPotionOfCentaurSlaying, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/3/3b/Powerful_Potion_of_Centaur_Slaying.png"),
                new Buff("Powerful Potion of Krait Slaying",PowerfulPotionOfKraitSlaying, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/b/b4/Powerful_Potion_of_Krait_Slaying.png"),
                new Buff("Powerful Potion of Ogre Slaying",PowerfulPotionOfOgreSlaying, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/b/b5/Powerful_Potion_of_Ogre_Slaying.png"),
                new Buff("Powerful Potion of Elemental Slaying",PowerfulPotionOfElementalSlaying, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/5f/Powerful_Potion_of_Elemental_Slaying.png"),
                new Buff("Powerful Potion of Destroyer Slaying",PowerfulPotionOfDestroyerSlaying, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/b/bd/Powerful_Potion_of_Destroyer_Slaying.png"),
                new Buff("Powerful Potion of Nightmare Court Slaying",PowerfulPotionOfNightmareCourtSlaying, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/7/74/Powerful_Potion_of_Nightmare_Court_Slaying.png"),
                new Buff("Powerful Potion of Slaying Scarlet's Armies",PowerfulPotionOfSlayingScarletsArmies, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/e/ee/Powerful_Potion_of_Demon_Slaying.png"),
                new Buff("Powerful Potion of Undead Slaying",PowerfulPotionOfUndeadSlaying, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/b/bd/Powerful_Potion_of_Undead_Slaying.png"),
                new Buff("Powerful Potion of Dredge Slaying",PowerfulPotionOfDredgeSlaying, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/9/9a/Powerful_Potion_of_Dredge_Slaying.png"),
                new Buff("Powerful Potion of Inquest Slaying",PowerfulPotionOfInquestSlaying, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/f/fb/Powerful_Potion_of_Inquest_Slaying.png"),
                new Buff("Powerful Potion of Demon Slaying",PowerfulPotionOfDemonSlaying, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/e/ee/Powerful_Potion_of_Demon_Slaying.png"),
                new Buff("Powerful Potion of Grawl Slaying",PowerfulPotionOfGrawlSlaying, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/1/15/Powerful_Potion_of_Grawl_Slaying.png"),
                new Buff("Powerful Potion of Sons of Svanir Slaying",PowerfulPotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/3/33/Powerful_Potion_of_Sons_of_Svanir_Slaying.png"),
                new Buff("Powerful Potion of Outlaw Slaying",PowerfulPotionOfOutlawSlaying, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/e/ec/Powerful_Potion_of_Outlaw_Slaying.png"),
                new Buff("Powerful Potion of Ice Brood Slaying",PowerfulPotionOfIceBroodSlaying, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/0/0d/Powerful_Potion_of_Ice_Brood_Slaying.png"),
                // Fractals 
                new Buff("Fractal Mobility", FractalMobility, Source.Item, BuffStackType.Stacking, 5, BuffClassification.Consumable,"https://wiki.guildwars2.com/images/thumb/2/22/Mist_Mobility_Potion.png/40px-Mist_Mobility_Potion.png"),
                new Buff("Fractal Defensive", FractalDefensive, Source.Item, BuffStackType.Stacking, 5, BuffClassification.Consumable,"https://wiki.guildwars2.com/images/thumb/e/e6/Mist_Defensive_Potion.png/40px-Mist_Defensive_Potion.png"),
                new Buff("Fractal Offensive", FractalOffensive, Source.Item, BuffStackType.Stacking, 5, BuffClassification.Consumable,"https://wiki.guildwars2.com/images/thumb/8/8d/Mist_Offensive_Potion.png/40px-Mist_Offensive_Potion.png"),

        };

        internal static readonly List<Buff> AscendedFood = new List<Buff>
        {
                // Ascended Food
                // Feasts with yet unknown IDs are also added with ID of -1, the IDs can be added later on demand
                new Buff("Bowl of Fruit Salad with Cilantro Garnish", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/0/08/Bowl_of_Fruit_Salad_with_Cilantro_Garnish.png"),
                new Buff("Bowl of Fruit Salad with Mint Garnish", BowlOfFruitSaladWithMintGarnish, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/4/47/Bowl_of_Fruit_Salad_with_Mint_Garnish.png"),
                new Buff("Bowl of Fruit Salad with Orange-Clove Syrup", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/d/dc/Bowl_of_Fruit_Salad_with_Orange-Clove_Syrup.png"),
                new Buff("Bowl of Sesame Fruit Salad", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/0/01/Bowl_of_Sesame_Fruit_Salad.png"),
                new Buff("Bowl of Spiced Fruit Salad", BowlOfSpicedFruitSalad, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/9/9c/Bowl_of_Spiced_Fruit_Salad.png"),
                new Buff("Cilantro Lime Sous-Vide Steak", CilantroLimeSousVideSteak, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/6/65/Cilantro_Lime_Sous-Vide_Steak.png"),
                new Buff("Cilantro and Cured Meat Flatbread", CilantroAndCuredMeatFlatbread, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/8/87/Cilantro_and_Cured_Meat_Flatbread.png"),
                new Buff("Clove and Veggie Flatbread", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/6/6e/Clove_and_Veggie_Flatbread.png"),
                new Buff("Clove-Spiced Creme Brulee", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/a/a2/Clove-Spiced_Creme_Brulee.png"),
                new Buff("Clove-Spiced Eggs Benedict", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/7/7d/Clove-Spiced_Eggs_Benedict.png"),
                new Buff("Clove-Spiced Pear and Cured Meat Flatbread", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/c/c5/Clove-Spiced_Pear_and_Cured_Meat_Flatbread.png"),
                new Buff("Eggs Benedict with Mint-Parsley Sauce", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/9/92/Eggs_Benedict_with_Mint-Parsley_Sauce.png"),
                new Buff("Mango Cilantro Creme Brulee", MangoCilantroCremeBrulee, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/3/3d/Mango_Cilantro_Creme_Brulee.png"),
                new Buff("Mint Creme Brulee", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/3/31/Mint_Creme_Brulee.png"),
                new Buff("Mint Strawberry Cheesecake", MintStrawberryCheesecake, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/6/64/Mint_Strawberry_Cheesecake.png"),
                new Buff("Mint and Veggie Flatbread", MintAndVeggieFlatbread, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/f/f9/Mint_and_Veggie_Flatbread.png"),
                new Buff("Mint-Pear Cured Meat Flatbread", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/6/60/Mint-Pear_Cured_Meat_Flatbread.png"),
                new Buff("Mushroom Clove Sous-Vide Steak", MushroomCloveSousVideSteak, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/b/ba/Mushroom_Clove_Sous-Vide_Steak.png"),
                new Buff("Orange Clove Cheesecake", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/3/3f/Orange_Clove_Cheesecake.png"),
                new Buff("Peppercorn and Veggie Flatbread", PeppercornAndVeggieFlatbread, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/9/9d/Peppercorn_and_Veggie_Flatbread.png"),
                new Buff("Peppercorn-Crusted Sous-Vide Steak", PeppercornCrustedSousVideSteak, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/2/2e/Peppercorn-Crusted_Sous-Vide_Steak.png"),
                new Buff("Peppercorn-Spiced Eggs Benedict", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/c/c6/Peppercorn-Spiced_Eggs_Benedict.png"),
                new Buff("Peppered Cured Meat Flatbread", PepperedCuredMeatFlatbread, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/2/2d/Peppered_Cured_Meat_Flatbread.png"),
                new Buff("Plate of Beef Carpaccio with Mint Garnish", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/4/42/Plate_of_Beef_Carpaccio_with_Mint_Garnish.png"),
                new Buff("Plate of Clear Truffle and Cilantro Ravioli", PlateOfClearTruffleAndCilantroRavioli, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/0/05/Plate_of_Clear_Truffle_and_Cilantro_Ravioli.png"),
                new Buff("Plate of Clear Truffle and Mint Ravioli", PlateOfClearTruffleAndMintRavioli, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/9/9e/Plate_of_Clear_Truffle_and_Mint_Ravioli.png"),
                new Buff("Plate of Clear Truffle and Sesame Ravioli", PlateOfClearTruffleAndSesameRavioli, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/d/de/Plate_of_Clear_Truffle_and_Sesame_Ravioli.png"),
                new Buff("Plate of Clove-Spiced Beef Carpaccio", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/4/44/Plate_of_Clove-Spiced_Beef_Carpaccio.png"),
                new Buff("Plate of Clove-Spiced Clear Truffle Ravioli", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/c/c2/Plate_of_Clove-Spiced_Clear_Truffle_Ravioli.png"),
                new Buff("Plate of Clove-Spiced Coq Au Vin", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/d/dc/Plate_of_Clove-Spiced_Coq_Au_Vin.png"),
                new Buff("Plate of Clove-Spiced Poultry Aspic", PlateOfCloveSpicedPoultryAspic, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/50/Plate_of_Clove-Spiced_Poultry_Aspic.png"),
                new Buff("Plate of Coq Au Vin with Mint Garnish", PlateOfCoqAuVinWithMintGarnish, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/7/7c/Plate_of_Coq_Au_Vin_with_Mint_Garnish.png"),
                new Buff("Plate of Coq Au Vin with Salsa", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/8/80/Plate_of_Coq_Au_Vin_with_Salsa.png"),
                new Buff("Plate of Peppercorn-Spiced Beef Carpaccio", PlateOfPeppercornSpicedBeefCarpaccio, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/4/40/Plate_of_Peppercorn-Spiced_Beef_Carpaccio.png"),
                new Buff("Plate of Peppercorn-Spiced Coq Au Vin", PlateOfPeppercornSpicedCoqAuVin, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/4/43/Plate_of_Peppercorn-Spiced_Coq_Au_Vin.png"),
                new Buff("Plate of Peppercorn-Spiced Poultry Aspic", PlateOfPeppercornSpicedPoultryAspic, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/4/4f/Plate_of_Peppercorn-Spiced_Poultry_Aspic.png"),
                new Buff("Plate of Peppered Clear Truffle Ravioli", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/f/fe/Plate_of_Peppered_Clear_Truffle_Ravioli.png"),
                new Buff("Plate of Poultry Aspic with Mint Garnish", PlateOfPoultryAspicWithMintGarnish, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/9/91/Plate_of_Poultry_Aspic_with_Mint_Garnish.png"),
                new Buff("Plate of Poultry Aspic with Salsa Garnish", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/5b/Plate_of_Poultry_Aspic_with_Salsa_Garnish.png"),
                new Buff("Plate of Sesame Poultry Aspic", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/6/64/Plate_of_Sesame_Poultry_Aspic.png"),
                new Buff("Plate of Sesame-Crusted Coq Au Vin", PlateOfSesameCrustedCoqAuVin, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/3/3e/Plate_of_Sesame-Crusted_Coq_Au_Vin.png"),
                new Buff("Plate of Sesame-Ginger Beef Carpaccio", PlateOfSesameGingerBeefCarpaccio, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/b/b7/Plate_of_Sesame-Ginger_Beef_Carpaccio.png"),
                new Buff("Salsa Eggs Benedict", SalsaEggsBenedict, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/7/79/Salsa_Eggs_Benedict.png"),
                new Buff("Salsa-Topped Veggie Flatbread", SalsaToppedVeggieFlatbread, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/f/f3/Salsa-Topped_Veggie_Flatbread.png"),
                new Buff("Sesame Cheesecake", SesameCheesecake, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/e/ef/Sesame_Cheesecake.png"),
                new Buff("Sesame Creme Brulee", SesameCremeBrulee, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/6/63/Sesame_Creme_Brulee.png"),
                new Buff("Sesame Eggs Benedict", SesameEggsBenedict, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/f/f5/Sesame_Eggs_Benedict.png"),
                new Buff("Sesame Veggie Flatbread", SesameVeggieFlatbread, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/d/db/Sesame_Veggie_Flatbread.png"),
                new Buff("Sesame-Asparagus and Cured Meat Flatbread", SesameAsparagusAndCuredMeatFlatbread, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/8/87/Sesame-Asparagus_and_Cured_Meat_Flatbread.png"),
                new Buff("Sous-Vide Steak with Mint-Parsley Sauce", SousVideSteakWithMintParsleySauce, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/9/99/Sous-Vide_Steak_with_Mint-Parsley_Sauce.png"),
                new Buff("Soy-Sesame Sous-Vide Steak", SoySesameSousVideSteak, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/d/da/Soy-Sesame_Sous-Vide_Steak.png"),
                new Buff("Spherified Cilantro Oyster Soup", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/e/e1/Spherified_Cilantro_Oyster_Soup.png"),
                new Buff("Spherified Clove-Spiced Oyster Soup", SpherifiedCloveSpicedOysterSoup, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/f/fa/Spherified_Clove-Spiced_Oyster_Soup.png"),
                new Buff("Spherified Oyster Soup with Mint Garnish", SpherifiedOysterSoupWithMintGarnish, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/6/63/Spherified_Oyster_Soup_with_Mint_Garnish.png"),
                new Buff("Spherified Peppercorn-Spiced Oyster Soup", SpherifiedPeppercornSpicedOysterSoup, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/4/43/Spherified_Peppercorn-Spiced_Oyster_Soup.png"),
                new Buff("Spherified Sesame Oyster Soup", SpherifiedSesameOysterSoup, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/5/51/Spherified_Sesame_Oyster_Soup.png"),
                new Buff("Spiced Pepper Creme Brulee", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/b/ba/Spiced_Pepper_Creme_Brulee.png"),
                new Buff("Spiced Peppercorn Cheesecake",Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/9/9c/Spiced_Peppercorn_Cheesecake.png"),
                new Buff("Strawberry Cilantro Cheesecake", Unknown, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/8/8d/Strawberry_Cilantro_Cheesecake.png"),
                new Buff("Plate of Imperial Palace Special", PlateOfImperialPalaceSpecial , Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/c/c4/Plate_of_Imperial_Palace_Special.png"),
                new Buff("Plate of Crispy Fish Pancakes", PlateOfCrispyFishPancakes , Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/f/f5/Plate_of_Crispy_Fish_Pancakes.png"),
                new Buff("Bowl of Jade Sea Bounty", BowlOfJadeSeaBounty, Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/4/40/Bowl_of_Jade_Sea_Bounty.png"),
                new Buff("Bowl of Echovald Hotpot", BowlOfEchovaldHotpot , Source.Item, BuffClassification.Consumable, "https://wiki.guildwars2.com/images/e/e1/Bowl_of_Echovald_Hotpot.png"),
        };

        internal static readonly List<Buff> FoodProcs = new List<Buff>
        {
            // Effect procs for On Kill Food
            new Buff("Nourishment (Bonus Power)", NourishmentBonusPower, Source.Item, BuffClassification.Gear, "https://wiki.guildwars2.com/images/d/d6/Champion_of_the_Crown.png"),
            new Buff("Nourishment (Bonus Power & Ferocity)", NourishmentBonusPowerFerocity, Source.Item, BuffClassification.Gear, "https://wiki.guildwars2.com/images/d/d6/Champion_of_the_Crown.png"),
            new Buff("Malice (Bonus Condition Damage)", MaliceBonusConditionDamage, Source.Item, BuffClassification.Gear, "https://wiki.guildwars2.com/images/d/d6/Champion_of_the_Crown.png"),
        };
    }
}
