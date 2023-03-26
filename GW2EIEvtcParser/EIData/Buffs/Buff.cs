using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using GW2EIEvtcParser.EIData.Buffs;
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
            Enhancement,
            Nourishment,
            OtherConsumable,
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

        internal static Buff CreateCustomBuff(string name, long id, string link, int capacity, BuffClassification classification)
        {
            return new Buff(name + " " + id, id, Source.Item, capacity > 1 ? BuffStackType.Stacking : BuffStackType.Force, capacity, classification, link);
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
            if (buffInfoEvent.StackingType != StackType && buffInfoEvent.StackingType != BuffStackType.Unknown)
            {
                operation.UpdateProgressWithCancellationCheck("Incoherent stack type for " + Name + ": is " + StackType + " but expected " + buffInfoEvent.StackingType);
            }
        }

        internal AbstractBuffSimulator CreateSimulator(ParsedEvtcLog log, bool forceNoId)
        {
            BuffInfoEvent buffInfoEvent = log.CombatData.GetBuffInfoEvent(ID);
            int capacity = Capacity;
            if (buffInfoEvent != null && buffInfoEvent.MaxStacks != capacity)
            {
                capacity = buffInfoEvent.MaxStacks;
            }
            if (!log.CombatData.UseBuffInstanceSimulator || forceNoId)
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

        internal static BuffSourceFinder GetBuffSourceFinder(CombatData combatData, HashSet<long> boonIds)
        {
            ulong gw2Build = combatData.GetBuildEvent().Build;
            if (gw2Build >= GW2Builds.EODBeta2)
            {
                return new BuffSourceFinder20210921(boonIds);
            }
            if (gw2Build >= GW2Builds.May2021Balance)
            {
                return new BuffSourceFinder20210511(boonIds);
            }
            if (gw2Build >= GW2Builds.October2019Balance)
            {
                return new BuffSourceFinder20191001(boonIds);
            }
            if (gw2Build >= GW2Builds.March2019Balance)
            {
                return new BuffSourceFinder20190305(boonIds);
            }
            return new BuffSourceFinder20181211(boonIds);
        }

        public bool Available(CombatData combatData)
        {
            ulong gw2Build = combatData.GetBuildEvent().Build;
            return gw2Build < _maxBuild && gw2Build >= _minBuild;
        }

        internal static readonly List<Buff> Boons = new List<Buff>
        {
            new Buff("Might", Might, Source.Common, BuffStackType.Stacking, 25, BuffClassification.Boon, BuffImages.Might),
            new Buff("Fury", Fury, Source.Common, BuffStackType.Queue, 9, BuffClassification.Boon, BuffImages.Fury/*, 0, GW2Builds.May2021Balance*/),
            new Buff("Quickness", Quickness, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, BuffImages.Quickness/*, 0, GW2Builds.May2021Balance*/),
            new Buff("Alacrity", Alacrity, Source.Common, BuffStackType.Queue, 9, BuffClassification.Boon, BuffImages.Alacrity/*, 0, GW2Builds.May2021Balance*/),
            new Buff("Protection", Protection, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, BuffImages.Protection/*, 0 , GW2Builds.May2021Balance*/),
            new Buff("Regeneration", Regeneration, Source.Common, BuffStackType.Regeneration, 5, BuffClassification.Boon, BuffImages.Regeneration),
            new Buff("Vigor", Vigor, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, BuffImages.Vigor/*, 0, GW2Builds.May2021Balance*/),
            new Buff("Aegis", Aegis, Source.Common, BuffStackType.Queue, 9, BuffClassification.Boon, BuffImages.Aegis),
            new Buff("Stability", Stability, Source.Common, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Boon, BuffImages.Stability),
            new Buff("Swiftness", Swiftness, Source.Common, BuffStackType.Queue, 9, BuffClassification.Boon, BuffImages.Swiftness/*, 0, GW2Builds.May2021Balance*/),
            new Buff("Retaliation", Retaliation, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, BuffImages.Retaliation, 0, GW2Builds.May2021Balance),
            new Buff("Resistance", Resistance, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, BuffImages.Resistance/*, 0, GW2Builds.May2021Balance*/),
            //
            /*new Buff("Fury", 725, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Boon, "https://wiki.guildwars2.com/images/4/46/Fury.png", GW2Builds.May2021Balance, ulong.MaxValue),
            new Buff("Quickness", 1187, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Boon, "https://wiki.guildwars2.com/images/b/b4/Quickness.png", GW2Builds.May2021Balance, ulong.MaxValue),
            new Buff("Alacrity", 30328, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Boon, "https://wiki.guildwars2.com/images/4/4c/Alacrity.png", GW2Builds.May2021Balance, ulong.MaxValue),
            new Buff("Protection", 717, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Boon, "https://wiki.guildwars2.com/images/6/6c/Protection.png", GW2Builds.May2021Balance, ulong.MaxValue),
            new Buff("Vigor", 726, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Boon, "https://wiki.guildwars2.com/images/f/f4/Vigor.png", GW2Builds.May2021Balance, ulong.MaxValue),
            new Buff("Swiftness", 719, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Boon, "https://wiki.guildwars2.com/images/a/af/Swiftness.png", GW2Builds.May2021Balance, ulong.MaxValue),
            new Buff("Resolution", 873, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Boon, "https://wiki.guildwars2.com/images/0/06/Resolution.png", GW2Builds.May2021Balance, ulong.MaxValue),
            new Buff("Resistance", 26980, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Boon, "https://wiki.guildwars2.com/images/4/4b/Resistance.png", GW2Builds.May2021Balance, ulong.MaxValue),*/
            new Buff("Resolution", Resolution, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, BuffImages.Resolution, GW2Builds.May2021Balance, GW2Builds.EndOfLife),
            //
            new Buff("Number of Boons", NumberOfBoons, Source.Common, BuffStackType.Stacking, 0, BuffClassification.Other, BuffImages.BoonDuration),
        };

        internal static readonly List<Buff> Conditions = new List<Buff>
        {
            new Buff("Bleeding", Bleeding, Source.Common, BuffStackType.Stacking, 1500, BuffClassification.Condition, BuffImages.Bleeding),
            new Buff("Burning", Burning, Source.Common, BuffStackType.Stacking, 1500, BuffClassification.Condition, BuffImages.Burning),
            new Buff("Confusion", Confusion, Source.Common, BuffStackType.Stacking, 1500, BuffClassification.Condition, BuffImages.Confusion),
            new Buff("Poison", Poison, Source.Common, BuffStackType.Stacking, 1500, BuffClassification.Condition, BuffImages.Poison),
            new Buff("Torment", Torment, Source.Common, BuffStackType.Stacking, 1500, BuffClassification.Condition, BuffImages.Torment),
            new Buff("Blind", Blind, Source.Common, BuffStackType.Queue, 9, BuffClassification.Condition, BuffImages.Blind/*, 0, GW2Builds.May2021Balance*/),
            new Buff("Chilled", Chilled, Source.Common, BuffStackType.Queue, 5, BuffClassification.Condition, BuffImages.Chilled/*, 0, GW2Builds.May2021Balance*/),
            new Buff("Crippled", Crippled, Source.Common, BuffStackType.Queue, 5, BuffClassification.Condition, BuffImages.Crippled/*, 0, GW2Builds.May2021Balance*/),
            new Buff("Fear", Fear, Source.Common, BuffStackType.Queue, 5, BuffClassification.Condition, BuffImages.Fear),
            new Buff("Immobile", Immobile, Source.Common, BuffStackType.Queue, 3, BuffClassification.Condition, BuffImages.Immobile/*, 0, GW2Builds.May2021Balance*/),
            new Buff("Slow", Slow, Source.Common, BuffStackType.Queue, 9, BuffClassification.Condition, BuffImages.Slow/*, 0, GW2Builds.May2021Balance*/),
            new Buff("Weakness", Weakness, Source.Common, BuffStackType.Queue, 5, BuffClassification.Condition, BuffImages.Weakness/*, 0, GW2Builds.May2021Balance*/),
            new Buff("Taunt", Taunt, Source.Common, BuffStackType.Queue, 5, BuffClassification.Condition, BuffImages.Taunt),
            new Buff("Vulnerability", Vulnerability, Source.Common, BuffStackType.Stacking, 25, BuffClassification.Condition, BuffImages.Vulnerability),
            //          
            /* new Buff("Blind", 720, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Condition, "https://wiki.guildwars2.com/images/3/33/Blinded.png", GW2Builds.May2021Balance, ulong.MaxValue),
            new Buff("Chilled", 722, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Condition, "https://wiki.guildwars2.com/images/a/a6/Chilled.png", GW2Builds.May2021Balance, ulong.MaxValue),
            new Buff("Crippled", 721, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Condition, "https://wiki.guildwars2.com/images/f/fb/Crippled.png", GW2Builds.May2021Balance, ulong.MaxValue),
            new Buff("Immobile", 727, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Condition, "https://wiki.guildwars2.com/images/3/32/Immobile.png", GW2Builds.May2021Balance, ulong.MaxValue),
            new Buff("Slow", 26766, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Condition, "https://wiki.guildwars2.com/images/f/f5/Slow.png", GW2Builds.May2021Balance, ulong.MaxValue),
            new Buff("Weakness", 742, Source.Common, BuffStackType.CappedDuration, 0, BuffNature.Condition, "https://wiki.guildwars2.com/images/f/f9/Weakness.png", GW2Builds.May2021Balance, ulong.MaxValue),*/
            //
            new Buff("Number of Conditions", NumberOfConditions, Source.Common, BuffStackType.Stacking, 0, BuffClassification.Other, BuffImages.ConditionDuration),
        };

        internal static readonly List<Buff> Commons = new List<Buff>
        {
            new Buff("Number of Active Combat Minions", NumberOfActiveCombatMinions, Source.Common, BuffStackType.Stacking, 99, BuffClassification.Other, BuffImages.RuneOfRanger),
            new Buff("Number of Clones", NumberOfClones, Source.Common, BuffStackType.Stacking, 99, BuffClassification.Other, BuffImages.RuneOfMesmer),
            new Buff("Downed", Downed, Source.Common, BuffClassification.Other, BuffImages.Downed),
            new Buff("Exhaustion", Exhaustion, Source.Common, BuffStackType.Queue, 3, BuffClassification.Debuff, BuffImages.Exhaustion),
            new Buff("Stealth", Stealth, Source.Common, BuffStackType.Queue, 5, BuffClassification.Support, BuffImages.Stealth),
            new Buff("Hide in Shadows", HideInShadows, Source.Common, BuffStackType.Queue, 25, BuffClassification.Other, BuffImages.Stealth),
            new Buff("Revealed", Revealed, Source.Common, BuffClassification.Support, BuffImages.Revealed),
            new Buff("Superspeed", Superspeed, Source.Common, BuffClassification.Support, BuffImages.Superspeed, 0, GW2Builds.June2021Balance),
            new Buff("Superspeed", Superspeed, Source.Common, BuffStackType.Queue, 9, BuffClassification.Support, BuffImages.Superspeed, GW2Builds.June2021Balance, GW2Builds.EndOfLife),
            new Buff("Determined (762)", Determined762, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Determined (788)", Determined788, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Resurrection", Resurrection, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Determined (895)", Determined895, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Determined (3892)", Determined3892, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Determined (31450)", Determined31450, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Determined (52271)", Determined52271, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Invulnerability (757)", Invulnerability757, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Invulnerability (56227)", Invulnerability56227, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Invulnerability (801)", Invulnerability801, Source.Common, BuffStackType.Queue, 25, BuffClassification.Other, BuffImages.Determined),
            new Buff("Spawn Protection?", SpawnProtection, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Stun", Stun, Source.Common, BuffClassification.Other, BuffImages.Stun),
            new Buff("Daze", Daze, Source.Common, BuffClassification.Other, BuffImages.Daze),
            new Buff("Exposed (48209)", Exposed48209, Source.Common, BuffClassification.Other, BuffImages.Exposed),
            new Buff("Exposed (31589)", Exposed31589, Source.Common, BuffClassification.Other, BuffImages.Exposed),
            new Buff("Old Exposed", OldExposed, Source.Common, BuffClassification.Other, BuffImages.Exposed),
            new Buff("Unblockable", Unblockable, Source.Common, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, BuffImages.Unblockable, GW2Builds.February2020Balance, GW2Builds.EndOfLife),
            new Buff("Encumbered", Encumbered, Source.Common, BuffStackType.Queue, 9, BuffClassification.Debuff, BuffImages.Encumbered),
            new Buff("Celeritas Spores", CeleritasSpores, Source.FightSpecific, BuffClassification.Other, BuffImages.SpeedMushroom),
            new Buff("Branded Accumulation", BrandedAccumulation, Source.Common, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.AchillesBane),
            //Auras
            new Buff("Chaos Aura", ChaosAura, Source.Common, BuffClassification.Support, BuffImages.ChaosAura),
            new Buff("Fire Aura", FireAura, Source.Common, BuffClassification.Support, BuffImages.FireAura),
            new Buff("Frost Aura", FrostAura, Source.Common, BuffClassification.Support, BuffImages.FrostAura),
            new Buff("Light Aura", LightAura, Source.Common, BuffClassification.Support, BuffImages.LightAura),
            new Buff("Magnetic Aura", MagneticAura, Source.Common, BuffClassification.Support, BuffImages.MagneticAura),
            new Buff("Shocking Aura", ShockingAura, Source.Common, BuffClassification.Support, BuffImages.ShockingAura),
            new Buff("Dark Aura", DarkAura, Source.Common, BuffClassification.Support, BuffImages.DarkAura, GW2Builds.April2019Balance, GW2Builds.EndOfLife),
            //race
            new Buff("Take Root", TakeRootEffect, Source.Common, BuffClassification.Other, BuffImages.TakeRoot),
            new Buff("Become the Bear", BecomeTheBear, Source.Common, BuffClassification.Other, BuffImages.BecomeBear),
            new Buff("Become the Raven", BecomeTheRaven, Source.Common, BuffClassification.Other, BuffImages.BecomeRaven),
            new Buff("Become the Snow Leopard", BecomeTheSnowLeopard, Source.Common, BuffClassification.Other, BuffImages.BecomeLeopard),
            new Buff("Become the Wolf", BecomeTheWolf, Source.Common, BuffClassification.Other, BuffImages.BecomeWolf),
            new Buff("Avatar of Melandru", AvatarOfMelandru, Source.Common, BuffClassification.Other, BuffImages.AvatarOfMelandru),
            new Buff("Power Suit", PowerSuit, Source.Common, BuffClassification.Other, BuffImages.PowerSuit),
            new Buff("Reaper of Grenth", ReaperOfGrenth, Source.Common, BuffClassification.Other, BuffImages.ReaperOfGrenth),
            new Buff("Charrzooka", Charrzooka, Source.Common, BuffClassification.Other, BuffImages.Charrzooka),
            //
            new Buff("Guild Item Research", GuildItemResearch, Source.Common, BuffClassification.Other, BuffImages.GuildMagicFind),
            //
            new Buff("Crystalline Heart", CrystallineHeart, Source.Common, BuffClassification.Other, BuffImages.CrystallineHeart),
            // WvW
            new Buff("Minor Borderlands Bloodlust", MinorBorderlandsBloodlust, Source.Common, BuffClassification.Support, BuffImages.BorderlandBloodlust),
            new Buff("Major Borderlands Bloodlust", MajorBorderlandsBloodlust, Source.Common, BuffClassification.Support, BuffImages.BorderlandBloodlust),
            new Buff("Superior Borderlands Bloodlust", SuperiorBorderlandsBloodlust, Source.Common, BuffClassification.Support, BuffImages.BorderlandBloodlust),
            new Buff("Blessing of Elements", BlessingOfElements, Source.Common, BuffClassification.Support, BuffImages.BlessingElements),
            new Buff("Flame's Embrace", FlamesEmbrace, Source.Common, BuffClassification.Support, BuffImages.FlamesEmbrace),
        };

        internal static readonly List<Buff> Gear = new List<Buff>
        {
            // Sigils
            new Buff("Sigil of Concentration", SigilOfConcentration, Source.Gear, BuffClassification.Gear, BuffImages.SigilConcentration, 0, 93543),
            new Buff("Sigil of Corruption", SigilOfCorruption, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.SigilCorruption),
            new Buff("Sigil of Life", SigilOfLife, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.SigilLife),
            new Buff("Sigil of Perception", SigilOfPerception, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.SigilPerception),
            new Buff("Sigil of Bloodlust", SigilOfBloodlust, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.SigilBloodlust),
            new Buff("Sigil of Bounty", SigilOfBounty, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.SigilBounty),
            new Buff("Sigil of Benevolence", SigilOfBenevolence, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.SigilBenevolence),
            new Buff("Sigil of Momentum", SigilOfMomentum, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.SigilMomentum),
            new Buff("Sigil of the Stars", SigilOfTheStars, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.SigilStars),
            new Buff("Sigil of Severance", SigilOfSeverance, Source.Gear, BuffClassification.Gear, BuffImages.SigilSeverance),
            new Buff("Sigil of Doom", SigilOfDoom, Source.Gear, BuffClassification.Gear, BuffImages.SigilDoom),
            // Runes
            new Buff("Superior Rune of the Monk", SuperiorRuneOfTheMonk, Source.Gear, BuffStackType.Stacking, 10, BuffClassification.Gear, BuffImages.RuneMonk, 93543, GW2Builds.EndOfLife),
        };

        internal static readonly List<Buff> FractalInstabilities = new List<Buff>()
        {
            // Legacy
            new Buff("Mistlock Instability: Fleeting Precision", MistlockInstabilityFleetingPrecision, Source.FractalInstability, BuffClassification.Other, BuffImages.Instability),
            new Buff("Mistlock Instability: Impaired Immunity", MistlockInstabilityImpairedImmunity, Source.FractalInstability, BuffClassification.Other, BuffImages.Instability),
            new Buff("Mistlock Instability: Losing Control", MistlockInstabilityLosingControl, Source.FractalInstability, BuffClassification.Other, BuffImages.Instability),
            new Buff("Mistlock Instability: Mist Stalker", MistlockInstabilityMistStalker, Source.FractalInstability, BuffClassification.Other, BuffImages.Instability),
            new Buff("Mistlock Instability: Slippery Slope 1", MistlockInstabilitySlipperySlope1, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilitySlipperySlope),
            new Buff("Mistlock Instability: Slippery Slope 2", MistlockInstabilitySlipperySlope2, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilitySlipperySlope),
            new Buff("Mistlock Instability: Stormy Weather", MistlockInstabilityStormyWeather, Source.FractalInstability, BuffClassification.Other, BuffImages.Instability),
            new Buff("Mistlock Instability: Birds", MistlockInstabilityBirds, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilityBirds),
            new Buff("Mistlock Instability: Tainted Renewal", MistlockInstabilityTaintedRenewal, Source.FractalInstability, BuffClassification.Other, BuffImages.Instability),
            new Buff("Mistlock Instability: Stamina", MistlockInstabilityStamina, Source.FractalInstability, BuffClassification.Other, BuffImages.Instability),
            new Buff("Mistlock Instability: Playing Favorites", MistlockInstabilityPlayingFavorites, Source.FractalInstability, BuffClassification.Other, BuffImages.Instability),
            new Buff("Mistlock Instability: Overextended", MistlockInstabilityOverextended, Source.FractalInstability, BuffClassification.Other, BuffImages.Instability),
            new Buff("Mistlock Instability: Keep Them in Line", MistlockInstabilityKeepTheminLine, Source.FractalInstability, BuffClassification.Other, BuffImages.Instability),
            new Buff("Mistlock Instability: Ill and Chill", MistlockInstabilityIllAndChill, Source.FractalInstability, BuffClassification.Other, BuffImages.Instability),
            new Buff("Mistlock Instability: Frosty", MistlockInstabilityFrosty, Source.FractalInstability, BuffClassification.Other, BuffImages.Instability),
            new Buff("Mistlock Instability: Bloodlust", MistlockInstabilityBloodlust, Source.FractalInstability, BuffClassification.Other, BuffImages.Instability),
            new Buff("Mistlock Instability: Antielitism", MistlockInstabilityAntielitism, Source.FractalInstability, BuffClassification.Other, BuffImages.Instability),
            new Buff("Mistlock Instability: Agonizing Expedition", MistlockInstabilityAgonizingExpedition, Source.FractalInstability, BuffClassification.Other, BuffImages.Instability),
            // Current
            new Buff("Mistlock Instability: Adrenaline Rush", MistlockInstabilityAdrenalineRush, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilityAdrenalineRush),
            new Buff("Mistlock Instability: Afflicted", MistlockInstabilityAfflicted, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilityAfflicted),
            new Buff("Mistlock Instability: Boon Overload", MistlockInstabilityBoonOverload, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilityBoonOverload),
            new Buff("Mistlock Instability: Flux Bomb", MistlockInstabilityFluxBomb, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilityFluxBomb),
            new Buff("Mistlock Instability: Fractal Vindicators", MistlockInstabilityFractalVindicators, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilityFractalVindicator),
            new Buff("Mistlock Instability: Frailty", MistlockInstabilityFrailty, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilityFrailty),
            new Buff("Mistlock Instability: Hamstrung", MistlockInstabilityHamstrung, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilityHamstrung),
            new Buff("Mistlock Instability: Last Laugh", MistlockInstabilityLastLaugh, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilityLastLaugh),
            new Buff("Mistlock Instability: Mists Convergence", MistlockInstabilityMistsConvergence, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilityMistsConvergence),
            new Buff("Mistlock Instability: No Pain, No Gain", MistlockInstabilityNoPainNoGain, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilityNoPainNoGain),
            new Buff("Mistlock Instability: Outflanked", MistlockInstabilityOutflanked, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilityOutflanked),
            new Buff("Mistlock Instability: Social Awkwardness", MistlockInstabilitySocialAwkwardness, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilitySocialAwkwardness),
            new Buff("Mistlock Instability: Stick Together", MistlockInstabilityStickTogether, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilityStickTogether),
            new Buff("Mistlock Instability: Sugar Rush", MistlockInstabilitySugarRush, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilitySugarRush),
            new Buff("Mistlock Instability: Toxic Trail", MistlockInstabilityToxicTrail, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilityToxicTrail),
            new Buff("Mistlock Instability: Vengeance", MistlockInstabilityVengeance, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilityVengeance),
            new Buff("Mistlock Instability: We Bleed Fire", MistlockInstabilityWeBleedFire, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilityWeBleedFire),
            new Buff("Mistlock Instability: Toxic Sickness", MistlockInstabilityToxicSickness, Source.FractalInstability, BuffClassification.Other, BuffImages.InstabilityToxicSickness),
        };

        internal static readonly List<Buff> FightSpecific = new List<Buff>
        {
            // Generic
            new Buff("Emboldened", Emboldened, Source.FightSpecific,BuffStackType.Stacking, 5, BuffClassification.Offensive, BuffImages.Emboldened),
            new Buff("Spectral Agony", SpectralAgony, Source.FightSpecific,BuffStackType.Stacking, 25, BuffClassification.Debuff, BuffImages.SpectralAgony),
            new Buff("Agony", Agony, Source.FightSpecific,BuffStackType.Stacking, 25, BuffClassification.Debuff, BuffImages.Agony),
            new Buff("Hamstrung", Hamstrung, Source.FightSpecific,BuffStackType.Stacking, 99, BuffClassification.Debuff, BuffImages.Hamstrung),
            new Buff("Enraged (?)", Enraged1_Unknown, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Enraged (??)", Enraged2_Unknown, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Enraged 1 (100%)", Enraged1_100, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Enraged 2 (100%)", Enraged2_100, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Enraged 1 (200%)", Enraged1_200, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Enraged 2 (200%)", Enraged2_200, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Enraged 3 (200%)", Enraged3_200, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Enraged 4 (200%)", Enraged4_200, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Enraged (300%)", Enraged_300, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Enraged (500%)", Enraged_500, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Call of the Mists", CallOfTheMists, Source.FightSpecific, BuffClassification.Other, BuffImages.CallOfTheMists),
            new Buff("Untargetable", Untargetable, Source.FightSpecific, BuffClassification.Other, BuffImages.Determined ),
            // Strike Essences
            new Buff("Essence of Vigilance Tier 1", EssenceOfVigilanceTier1, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, BuffImages.EssenceOfVigilance),
            new Buff("Essence of Vigilance Tier 2", EssenceOfVigilanceTier2, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, BuffImages.EssenceOfVigilance),
            new Buff("Essence of Vigilance Tier 3", EssenceOfVigilanceTier3, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, BuffImages.EssenceOfVigilance),
            new Buff("Essence of Vigilance Tier 4", EssenceOfVigilanceTier4, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, BuffImages.EssenceOfVigilance),
            new Buff("Power of Vigilance Tier 2", PowerOfVigilanceTier2, Source.FightSpecific, BuffClassification.Other, BuffImages.PowerOfVigilance),
            new Buff("Power of Vigilance Tier 3", PowerOfVigilanceTier3, Source.FightSpecific, BuffClassification.Other, BuffImages.PowerOfVigilance),
            new Buff("Essence of Resilience Tier 1", EssenceOfResilienceTier1, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, BuffImages.EssenceOfResilience),
            new Buff("Essence of Resilience Tier 2", EssenceOfResilienceTier2, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, BuffImages.EssenceOfResilience),
            new Buff("Essence of Resilience Tier 3", EssenceOfResilienceTier3, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, BuffImages.EssenceOfResilience),
            new Buff("Essence of Resilience Tier 4", EssenceOfResilienceTier4, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, BuffImages.EssenceOfResilience),
            new Buff("Power of Resilience Tier 2", PowerOfResilienceTier2, Source.FightSpecific, BuffClassification.Other, BuffImages.PowerOfResilience),
            new Buff("Power of Resilience Tier 4", PowerOfResilienceTier4, Source.FightSpecific, BuffClassification.Other, BuffImages.PowerOfResilience),
            new Buff("Essence of Valor Tier 1", EssenceOfValorTier1, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, BuffImages.EssenceOfValor),
            new Buff("Essence of Valor Tier 2", EssenceOfValorTier2, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, BuffImages.EssenceOfValor),
            new Buff("Essence of Valor Tier 3", EssenceOfValorTier3, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, BuffImages.EssenceOfValor),
            new Buff("Essence of Valor Tier 4", EssenceOfValorTier4, Source.FightSpecific,BuffStackType.StackingConditionalLoss, 30, BuffClassification.Other, BuffImages.EssenceOfValor),
            new Buff("Power of Valor Tier 1", PowerOfValorTier1, Source.FightSpecific, BuffClassification.Other, BuffImages.PowerOfValor),
            new Buff("Power of Valor Tier 2", PowerOfValorTier2, Source.FightSpecific, BuffClassification.Other, BuffImages.PowerOfValor),
            // Unknown Fixation            
            new Buff("Fixated 1(???)", Fixated1, Source.FightSpecific, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Fixated 2(???)", Fixated2, Source.FightSpecific, BuffClassification.Other, BuffImages.Fixated),
            //////////////////////////////////////////////
            // Mordremoth
            new Buff("Parietal Mastery", ParietalMastery, Source.FightSpecific, BuffClassification.Other, BuffImages.ParietalMastery),
            new Buff("Parietal Origin", ParietalOrigin, Source.FightSpecific, BuffClassification.Other, BuffImages.ParietalMastery),
            new Buff("Temporal Mastery", TemporalMastery, Source.FightSpecific, BuffClassification.Other, BuffImages.TemporalMastery),
            new Buff("Temporal Origin", TemporalOrigin, Source.FightSpecific, BuffClassification.Other, BuffImages.TemporalMastery),
            new Buff("Occipital Mastery", OccipitalMastery, Source.FightSpecific, BuffClassification.Other, BuffImages.OccipitalMastery),
            new Buff("Occipital Origin", OccipitalOrigin, Source.FightSpecific, BuffClassification.Other, BuffImages.OccipitalMastery),
            new Buff("Frontal Mastery", FrontalMastery, Source.FightSpecific, BuffClassification.Other, BuffImages.FrontalMastery),
            new Buff("Frontal Origin", FrontalOrigin, Source.FightSpecific, BuffClassification.Other, BuffImages.FrontalMastery),
            new Buff("Exposed (Mordremoth)", ExposedMordremoth, Source.Common, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.Exposed),
            new Buff("Weakened (Effect 1)", WeakenedEffect1, Source.Common, BuffClassification.Other, BuffImages.Weakened),
            new Buff("Weakened (Effect 2)", WeakenedEffect2, Source.Common, BuffClassification.Other, BuffImages.Weakened),
            new Buff("Empowered (Hearts and Minds)", EmpoweredHeartsAndMinds, Source.Common, BuffClassification.Other, BuffImages.EmpoweredHeartAndMinds),
            new Buff("Power (Hearts and Minds)", PowerHeartsAndMinds, Source.Common, BuffClassification.Other, BuffImages.PowerHeartAndMinds),
            new Buff("Shifty Aura", ShiftyAura, Source.Common, BuffClassification.Other, BuffImages.BrandedAura),
            new Buff("Fiery Block", FieryBlock, Source.Common, BuffClassification.Other, BuffImages.ShieldStance),
            //////////////////////////////////////////////
            // VG
            new Buff("Blue Pylon Power", BluePylonPower, Source.FightSpecific, BuffClassification.Other, BuffImages.BluePylon),
            new Buff("Pylon Attunement: Red", PylonAttunementRed, Source.FightSpecific, BuffClassification.Other, BuffImages.RedPylon),
            new Buff("Pylon Attunement: Blue", PylonAttunementBlue, Source.FightSpecific, BuffClassification.Other, BuffImages.BluePylon),
            new Buff("Pylon Attunement: Green", PylonAttunementGreen, Source.FightSpecific, BuffClassification.Other, BuffImages.GreenPylon),
            new Buff("Unstable Pylon: Red", UnstablePylonRed, Source.FightSpecific, BuffClassification.Other, BuffImages.UnstableRed),
            new Buff("Unstable Pylon: Blue", UnstablePylonBlue, Source.FightSpecific, BuffClassification.Other, BuffImages.UnstableBlue),
            new Buff("Unstable Pylon: Green", UnstablePylonGreen, Source.FightSpecific, BuffClassification.Other, BuffImages.UnstableGreen),
            // Gorseval
            new Buff("Spirited Fusion", SpiritedFusion, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Other, BuffImages.SpiritedFusion),
            new Buff("Protective Shadow", ProtectiveShadow, Source.FightSpecific, BuffClassification.Other, BuffImages.ProtectiveShadow),
            new Buff("Ghastly Prison", GhastlyPrison, Source.FightSpecific, BuffClassification.Debuff, BuffImages.GhastlyPrison),
            new Buff("Vivid Echo", VividEcho, Source.FightSpecific, BuffStackType.Queue, 5, BuffClassification.Other, BuffImages.VividEcho),
            new Buff("Spectral Darkness", SpectralDarkness, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.SpectralDarkness),
            // Sabetha    
            new Buff("Shell-Shocked", ShellShocked, Source.FightSpecific, BuffClassification.Other, BuffImages.ShellShocked),
            new Buff("Sapper Bomb", SapperBomb, Source.FightSpecific, BuffClassification.Other, BuffImages.SapperBomb),
            new Buff("Time Bomb", TimeBomb, Source.FightSpecific, BuffClassification.Other, BuffImages.TimeBomb),
            //////////////////////////////////////////////
            // Slothasor
            new Buff("Narcolepsy", NarcolepsyEffect, Source.FightSpecific, BuffClassification.Other, BuffImages.Determined),
            new Buff("Nauseated", Nauseated, Source.FightSpecific, BuffClassification.Other, BuffImages.Nauseated),
            new Buff("Magic Transformation", MagicTransformation, Source.FightSpecific, BuffClassification.Other, BuffImages.MagicTransformation),
            new Buff("Fixated (Slothasor)", FixatedSlothasor, Source.FightSpecific, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Volatile Poison", VolatilePoisonEffect, Source.FightSpecific, BuffClassification.Other, BuffImages.VolatilePoison),
            new Buff("Slippery Slubling", SlipperySlubling, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // Trio
            new Buff("Not the Bees!", NotTheBees, Source.FightSpecific, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.ThrowJar),
            new Buff("Slow Burn", SlowBurn, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.HeatWave),
            new Buff("Targeted", Targeted, Source.FightSpecific, BuffClassification.Other, BuffImages.TargetedLocust),
            new Buff("Target!", Target, Source.FightSpecific, BuffStackType.Stacking, 20, BuffClassification.Other, BuffImages.Target),
            new Buff("Locust Trail", LocustTrail, Source.FightSpecific, BuffClassification.Debuff, BuffImages.Target),
            new Buff("Environmentally Friendly", EnvironmentallyFriendly, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // Matthias
            new Buff("Blood Shield Abo", BloodShieldAbo, Source.FightSpecific, BuffStackType.Stacking, 18, BuffClassification.Other, BuffImages.BloodShield),
            new Buff("Blood Shield", BloodShield, Source.FightSpecific, BuffStackType.Stacking, 18, BuffClassification.Other, BuffImages.BloodShield),
            new Buff("Blood Fueled", BloodFueled, Source.FightSpecific, BuffStackType.Stacking, 1, BuffClassification.Offensive, BuffImages.BloodFueled),
            new Buff("Blood Fueled Abo", BloodFueledAbo, Source.FightSpecific, BuffStackType.Stacking, 15, BuffClassification.Offensive, BuffImages.BloodFueled),
            new Buff("Unstable Blood Magic", UnstableBloodMagic, Source.FightSpecific, BuffClassification.Other, BuffImages.UnstableBloodMagic),
            new Buff("Corruption", Corruption1, Source.FightSpecific, BuffClassification.Other, BuffImages.LocustTrail),
            new Buff("Corruption 2", Corruption2, Source.FightSpecific, BuffClassification.Other, BuffImages.LocustTrail),
            new Buff("Sacrifice", MatthiasSacrifice, Source.FightSpecific, BuffClassification.Other, BuffImages.MonsterSkill),
            new Buff("Unbalanced", Unbalanced, Source.FightSpecific, BuffClassification.Other, BuffImages.Unbalanced),
            new Buff("Zealous Benediction", ZealousBenediction, Source.FightSpecific, BuffClassification.Other, BuffImages.Unstable),
            new Buff("Snowstorm", SnowstormEffect, Source.FightSpecific, BuffClassification.Other, BuffImages.Snowstorm),
            new Buff("Heat Wave", HeatWave, Source.FightSpecific, BuffClassification.Other, BuffImages.HeatWave),
            new Buff("Downpour", DownpourEffect, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.Downpour),
            new Buff("Snowstorm (Matthias)", SnowstormMatthias, Source.FightSpecific, BuffClassification.Other, BuffImages.Snowstorm),
            new Buff("Heat Wave (Matthias)", HeatWaveMatthias, Source.FightSpecific, BuffClassification.Other, BuffImages.HeatWave),
            new Buff("Downpour (Matthias)", DownpourMatthias, Source.FightSpecific, BuffClassification.Other, BuffImages.Downpour),
            new Buff("Unstable", Unstable, Source.FightSpecific, BuffClassification.Other, BuffImages.Unstable),      
            //////////////////////////////////////////////
            // Escort
            new Buff("Toxic Spores (Escort)", EscortToxicSpores, Source.FightSpecific, BuffClassification.Debuff, BuffImages.ToxicSpores),
            new Buff("Healing Cleanse (Escort)", EscortHealingCleanse, Source.FightSpecific, BuffClassification.Support, BuffImages.Elasticity),
            new Buff("Surveilled (Escort)", EscortSurveilled, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Debuff, BuffImages.Fear),
            new Buff("Achievement Eligibility: Fast Siege", AchievementEligibilityFastSiege, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Love Is Bunny", AchievementEligibilityLoveIsBunny, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Mine Control", AchievementEligibilityMineControl, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // KC
            // This achievement buff can be found during Twisted Castle and Xera too, very rare though
            new Buff("Achievement Eligibility: Down, Down, Downed", AchievementEligibilityDownDownDowned, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Compromised", Compromised, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.Compromised),
            new Buff("Xera's Boon", XerasBoon, Source.FightSpecific, BuffClassification.Other, BuffImages.XerasBoon),
            new Buff("Xera's Fury", XerasFury, Source.FightSpecific, BuffClassification.Other, BuffImages.XerasFury),
            new Buff("Statue Fixated", StatueFixated1, Source.FightSpecific, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Statue Fixated 2", StatueFixated2, Source.FightSpecific, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Incoming!", Incoming, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Crimson Attunement (Orb)", CrimsonAttunementOrb, Source.FightSpecific, BuffClassification.Other, BuffImages.CrimsonAttunement),
            new Buff("Radiant Attunement (Orb)", RadiantAttunementOrb, Source.FightSpecific, BuffClassification.Other, BuffImages.RadiantAttunement),
            new Buff("Crimson Attunement (Phantasm)", CrimsonAttunementPhantasm, Source.FightSpecific, BuffClassification.Other, BuffImages.CrimsonAttunement),
            new Buff("Radiant Attunement (Phantasm)", RadiantAttunementPhantasm, Source.FightSpecific, BuffClassification.Other, BuffImages.RadiantAttunement),
            new Buff("Magic Blast", MagicBlast, Source.FightSpecific, BuffStackType.Stacking, 35, BuffClassification.Other, BuffImages.MagicBlast),
            new Buff("Gaining Power", GainingPower, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Unbreakable", Unbreakable, Source.FightSpecific, BuffStackType.Stacking, 2, BuffClassification.Other, BuffImages.XerasEmbrace),
            // Twisted Castle
            new Buff("Spatial Distortion", SpatialDistortion, Source.FightSpecific, BuffClassification.Other, BuffImages.BloodstoneBlessed),
            new Buff("Madness", Madness, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Debuff, BuffImages.Madness),
            new Buff("Still Waters", StillWaters, Source.FightSpecific, BuffClassification.Other, BuffImages.StillWaters),
            new Buff("Soothing Waters", SoothingWaters, Source.FightSpecific, BuffClassification.Other, BuffImages.SoothingWaters),
            new Buff("Chaotic Haze", ChaoticHaze, Source.FightSpecific, BuffClassification.Other, BuffImages.LavaFont),
            new Buff("Creeping Pursuit", CreepingPursuit, Source.FightSpecific, BuffClassification.Other, BuffImages.CreepingPursuit),
            new Buff("Achievement Eligibility: Evasive Maneuver", AchievementEligibilityEvasiveManeuver, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Mildly Insane", AchievementEligibilityMildlyInsane, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // Xera      
            new Buff("Derangement", Derangement, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Debuff, BuffImages.Derangement),
            new Buff("Bending Chaos", BendingChaos, Source.FightSpecific, BuffStackType.Stacking, 11, BuffClassification.Other, BuffImages.Target2),
            new Buff("Shifting Chaos", ShiftingChaos, Source.FightSpecific, BuffStackType.Stacking, 11, BuffClassification.Other, BuffImages.ShiftingChaos),
            new Buff("Twisting Chaos", TwistingChaos, Source.FightSpecific, BuffStackType.Stacking, 11, BuffClassification.Other, BuffImages.TwistingChaos),
            new Buff("Intervention", Intervention, Source.FightSpecific, BuffClassification.Other, BuffImages.Intervention),
            new Buff("Bloodstone Protection", BloodstoneProtection, Source.FightSpecific, BuffClassification.Other, BuffImages.BloodstoneProtection),
            new Buff("Bloodstone Blessed", BloodstoneBlessed, Source.FightSpecific, BuffClassification.Other, BuffImages.BloodstoneBlessed),
            new Buff("Void Zone", VoidZone, Source.FightSpecific, BuffClassification.Other, BuffImages.VoidZone),
            new Buff("Gravity Well (Xera)", GravityWellXera, Source.FightSpecific, BuffClassification.Other, BuffImages.MonsterSkill),
            new Buff("Hero's Departure", HerosDeparture, Source.FightSpecific, BuffClassification.Other, BuffImages.Determined),
            new Buff("Hero's Return", HerosReturn, Source.FightSpecific, BuffClassification.Other, BuffImages.Determined),
            //////////////////////////////////////////////
            // Cairn        
            new Buff("Shared Agony", SharedAgony, Source.FightSpecific, BuffClassification.Debuff, BuffImages.SharedAgony),
            new Buff("Enraged (Cairn)", EnragedCairn, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Unseen Burden", UnseenBurden, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Debuff, BuffImages.UnseenBurden),
            new Buff("Countdown", Countdown, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.Countdown),
            new Buff("Gaze Avoidance", GazeAvoidance, Source.FightSpecific, BuffClassification.Other, BuffImages.GazeAvoidance),
            // MO             
            new Buff("Empowered", Empowered, Source.FightSpecific, BuffStackType.Stacking, 4, BuffClassification.Other, BuffImages.Empowered),
            new Buff("Mursaat Overseer's Shield", MursaatOverseersShield, Source.FightSpecific, BuffClassification.Other, BuffImages.Dispel),
            new Buff("Protect (SAK)", ProtectSAK, Source.FightSpecific, BuffClassification.Other, BuffImages.Protect),
            new Buff("Dispel (SAK)", DispelSAK, Source.FightSpecific, BuffClassification.Other, BuffImages.Dispel),
            new Buff("Claim (SAK)", ClaimSAK, Source.FightSpecific, BuffClassification.Other, BuffImages.Claim),
            // Samarog            
            new Buff("Fixated (Samarog)", FixatedSamarog, Source.FightSpecific, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Fixated (Guldhem)", FixatedGuldhem, Source.FightSpecific, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Fixated (Rigom)", FixatedRigom, Source.FightSpecific, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Inevitable Betrayal (Big)", InevitableBetrayalBig, Source.FightSpecific, BuffClassification.Other, BuffImages.FeedingFrenzy),
            new Buff("Inevitable Betrayal (Small)", InevitableBetrayalSmall, Source.FightSpecific, BuffClassification.Other, BuffImages.FeedingFrenzy),
            new Buff("Soul Swarm", SoulSwarm, Source.FightSpecific, BuffClassification.Other, BuffImages.SoulSwarm),
            new Buff("Fanatical Resilience", FanaticalResilience, Source.FightSpecific, BuffClassification.Other, BuffImages.Burrowed),
            new Buff("Brutalized", BrutalizeEffect, Source.FightSpecific, BuffClassification.Other, BuffImages.Burrowed),
            // Deimos
            new Buff("Unnatural Signet", UnnaturalSignet, Source.FightSpecific, BuffClassification.Other, BuffImages.UnnaturalSignet),
            new Buff("Weak Minded", WeakMinded, Source.FightSpecific, BuffClassification.Debuff, BuffImages.UnseenBurdenDeimos),
            new Buff("Tear Instability", TearInstability, Source.FightSpecific, BuffClassification.Other, BuffImages.TearInstability),
            new Buff("Form Up and Advance!", FormUpAndAdvance, Source.FightSpecific, BuffClassification.Other, BuffImages.FormUpAndAdvance),
            new Buff("Devour", Devour, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Other, BuffImages.Devour),
            new Buff("Unseen Burden (Deimos)", UnseenBurdenDeimos, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Other, BuffImages.UnseenBurdenDeimos),
            //////////////////////////////////////////////
            // Soulless Horror
            new Buff("Exile's Embrace", ExilesEmbrace, Source.FightSpecific, BuffClassification.Other, BuffImages.ExilesEmbrace),
            new Buff("Fixated (SH)", FixatedSH, Source.FightSpecific, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Necrosis", Necrosis, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Debuff, BuffImages.Ichor),
            new Buff("Achievement Eligibility: Necro Dancer", AchievementEligibilityNecroDancer, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // River
            // This achievement buff can be found during River and every statue encounter
            new Buff("Achievement Eligibility: Statues of Limitation", AchievementEligibilityStatuesOfLimitation, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Soul Siphon", SoulSiphon, Source.FightSpecific, BuffClassification.Debuff, BuffImages.SoulSiphon),
            new Buff("Desmina's Protection", DesminasProtection, Source.FightSpecific, BuffClassification.Other, BuffImages.DesminasProtection),
            new Buff("Follower's Asylum", FollowersAsylum, Source.FightSpecific, BuffClassification.Other, BuffImages.DesminasProtection),
            new Buff("Spirit Form", SpiritForm, Source.FightSpecific, BuffClassification.Other, BuffImages.SpiritForm),
            new Buff("Mortal Coil (River)", MortalCoilRiver, Source.FightSpecific, BuffStackType.Stacking, 30, BuffClassification.Other, BuffImages.InternalCoil),
            new Buff("Energy Threshold (River)", EnergyThresholdRiver, Source.FightSpecific, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.SpiritForm),
            // Broken King          
            new Buff("Frozen Wind", FrozenWind, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Debuff, BuffImages.FrozenWind),
            new Buff("Shield of Ice", ShieldOfIce, Source.FightSpecific, BuffStackType.StackingConditionalLoss, 99, BuffClassification.Other, BuffImages.ShieldOfIce),
            new Buff("Glaciate", Glaciate, Source.FightSpecific, BuffClassification.Debuff, BuffImages.Glaciate),
            // Eater of Soul         
            new Buff("Soul Digestion", SoulDigestion, Source.FightSpecific, BuffClassification.Other, BuffImages.SoulDigestion),
            new Buff("Reclaimed Energy", ReclaimedEnergy, Source.FightSpecific, BuffClassification.Other, BuffImages.ReclaimedEnergy),
            new Buff("Mortal Coil (Statue of Death)", MortalCoilStatueOfDeath, Source.FightSpecific, BuffStackType.Stacking, 30, BuffClassification.Other, BuffImages.MortalCoil),
            new Buff("Empowered (Statue of Death)", EmpoweredStatueOfDeath, Source.FightSpecific, BuffClassification.Offensive, BuffImages.EmpoweredEater),
            //new Buff("Energy Threshold (Statue of Death)", 48583, Source.FightSpecific, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, BuffImages.SpiritForm),
            // Eyes
            new Buff("Last Grasp (Judgment)", LastGraspJudgment, Source.FightSpecific, BuffClassification.Other, BuffImages.LastGrasp),
            new Buff("Last Grasp (Fate)", LastGraspFate, Source.FightSpecific, BuffClassification.Other, BuffImages.LastGrasp),
            new Buff("Exposed (Statue of Darkness)", ExposedStatueOfDarkness, Source.FightSpecific, BuffClassification.Other, BuffImages.ExposedEyes),
            new Buff("Light Carrier", LightCarrier, Source.FightSpecific, BuffClassification.Other, BuffImages.TorchFielder),
            new Buff("Empowered (Light Thief)", EmpoweredLightThief, Source.FightSpecific, BuffClassification.Other, BuffImages.SoulDigestion),
            // Dhuum
            new Buff("Mortal Coil (Dhuum)", MortalCoilDhuum, Source.FightSpecific, BuffStackType.Stacking, 30, BuffClassification.Other, BuffImages.Compromised),
            new Buff("Fractured Spirit", FracturedSpirit, Source.FightSpecific, BuffClassification.Other, BuffImages.FracturedSpirit),
            new Buff("Residual Affliction", ResidualAffliction, Source.FightSpecific, BuffClassification.Other, BuffImages.ResidualAffliction),
            new Buff("Arcing Affliction", ArcingAffliction, Source.FightSpecific, BuffClassification.Other, BuffImages.ArcingAffliction),
            new Buff("One-Track Mind", OneTrackMind, Source.FightSpecific, BuffClassification.Other, BuffImages.Tracked),
            new Buff("Imminent Demise", ImminentDemise, Source.FightSpecific, BuffClassification.Other, BuffImages.SuperheatedMetal),
            new Buff("Lethal Report", LethalReport, Source.FightSpecific, BuffClassification.Other, BuffImages.MantraOfSignets),
            new Buff("Hastened Demise", HastenedDemise, Source.FightSpecific, BuffClassification.Other, BuffImages.HastenedDemise),
            new Buff("Echo's Pick up", EchosPickup, Source.FightSpecific, BuffClassification.Other, BuffImages.Unstable),
            new Buff("Energy Threshold (Dhuum)", EnergyThresholdDhuum, Source.FightSpecific, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.SpiritForm),
            //////////////////////////////////////////////
            // CA
            new Buff("Greatsword Power", GreatswordPower, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.GreatswordPower),
            new Buff("Fractured - Enemy", FracturedEnemy, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.BrandedAura),
            new Buff("Fractured - Allied", FracturedAllied, Source.FightSpecific, BuffStackType.Stacking, 2, BuffClassification.Other, BuffImages.BrandedAura),
            new Buff("Conjured Shield", ConjuredShield, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.ConjuredShield),
            new Buff("Conjured Protection", ConjuredProtection, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.InfusedShield),
            new Buff("Shielded", Shielded, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.PoweredShielding),
            new Buff("Augmented Power", AugmentedPower, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.PoweredShielding),
            new Buff("Locked On", LockedOn, Source.FightSpecific, BuffClassification.Debuff, BuffImages.Target2),
            new Buff("CA Invul", CAInvul, Source.FightSpecific, BuffClassification.Other, BuffImages.BloodFueled),
            new Buff("Arm Up", ArmUp, Source.FightSpecific, BuffClassification.Other, BuffImages.BloodFueled),
            new Buff("Fixation", Fixation, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Achievement Eligibility: Stacking Swords and Shields", AchievementEligibilityStackingSwordsAndShields, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // Twin Largos
            new Buff("Aquatic Detainment", AquaticDetainmentEffect, Source.FightSpecific, BuffClassification.Other, BuffImages.MonsterSkill),
            new Buff("Tidal Pool", TidalPoolEffect, Source.FightSpecific, BuffClassification.Other, BuffImages.MonsterSkill),
            new Buff("Aquatic Aura (Kenut)", AquaticAuraKenut, Source.FightSpecific, BuffStackType.Stacking, 80, BuffClassification.Other, BuffImages.ExposeWeakness),
            new Buff("Aquatic Aura (Nikare)", AquaticAuraNikare, Source.FightSpecific, BuffStackType.Stacking, 80, BuffClassification.Other, BuffImages.Fractured),
            new Buff("Waterlogged", Waterlogged, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Debuff, BuffImages.Waterlogged),
            new Buff("Enraged (Twin Largos)", EnragedTwinLargos, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Achievement Eligibility: Don't Go in the Water", AchievementEligibilityDontGoInTheWater, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // Qadim
            new Buff("Flame Armor", FlameArmor, Source.FightSpecific, BuffClassification.Other, BuffImages.MagmaOrb),
            new Buff("Fiery Surge", FierySurge, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Other, BuffImages.FierySurge),
            new Buff("Power of the Lamp", PowerOfTheLamp, Source.FightSpecific, BuffClassification.Other, BuffImages.BreakOut),
            new Buff("Unbearable Flames", UnbearableFlames, Source.FightSpecific, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, BuffImages.ExcessMagic),
            new Buff("Parry", Parry, Source.FightSpecific, BuffClassification.Other, BuffImages.Parry),
            new Buff("Mythwright Surge", MythwrightSurge, Source.FightSpecific, BuffClassification.Other, BuffImages.SwiftnessEffect),
            new Buff("Lamp Bond", LampBond, Source.FightSpecific, BuffClassification.Other, BuffImages.LampBond),
            new Buff("Enraged (Wywern)", EnragedWywern, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Enraged (Qadim)", EnragedQadim, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Resistance (Lava Elemental)", ResistanceLavaElemental, Source.FightSpecific, BuffStackType.Queue, 5, BuffClassification.Other, BuffImages.FireShield),
            new Buff("Shielded (Lava Elemental)", ShieldedLavaElemental, Source.FightSpecific, BuffClassification.Other, BuffImages.FireShield),
            new Buff("Achievement Eligibility: Manipulate the Manipulator", AchievementEligibilityManipulateTheManipulator, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Taking Turns", AchievementEligibilityTakingTurns, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            //////////////////////////////////////////////
            // Adina
            new Buff("Pillar Pandemonium", PillarPandemonium, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Other, BuffImages.CaptainsInspiration),
            new Buff("Radiant Blindness", RadiantBlindness, Source.FightSpecific, BuffStackType.Queue, 25, BuffClassification.Debuff, BuffImages.PersistentlyBlinded),
            new Buff("Diamond Palisade (Damage)", DiamondPalisadeDamage, Source.FightSpecific, BuffClassification.Other, BuffImages.MonsterSkill),
            new Buff("Diamond Palisade", DiamondPalisade, Source.FightSpecific, BuffClassification.Other, BuffImages.MonsterSkill),
            new Buff("Eroding Curse", ErodingCurse, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Other, BuffImages.ToxicGas),
            new Buff("Achievement Eligibility: Conserve the Land", AchievementEligibilityConserveTheLand, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // Sabir
            new Buff("Ion Shield", IonShield, Source.FightSpecific, BuffStackType.Stacking, 80, BuffClassification.Other, BuffImages.IonShield),
            new Buff("Violent Currents", ViolentCurrents, Source.FightSpecific, BuffStackType.Stacking, 5, BuffClassification.Offensive, BuffImages.ViolentCurrents),
            new Buff("Repulsion Field", RepulsionField, Source.FightSpecific, BuffClassification.Other, BuffImages.TargetedLocust),
            new Buff("Electrical Repulsion", ElectricalRepulsion, Source.FightSpecific, BuffClassification.Other, BuffImages.XerasFury),
            new Buff("Electro-Repulsion", ElectroRepulsion, Source.FightSpecific, BuffClassification.Other, BuffImages.ExposedEyes),
            new Buff("Eye of the Storm", EyeOfTheStorm, Source.FightSpecific, BuffClassification.Other, BuffImages.MendingWaters),
            new Buff("Bolt Break", BoltBreak, Source.FightSpecific, BuffClassification.Other, BuffImages.ViolentCurrents),
            new Buff("Achievement Eligibility: Charged Winds", AchievementEligibilityChargedWinds, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // Peerless Qadim
            new Buff("Erratic Energy", ErraticEnergy, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.Unstable),
            new Buff("Power Share", PowerShare, Source.FightSpecific, BuffStackType.Stacking, 3, BuffClassification.Other, BuffImages.PowerShare),
            new Buff("Sapping Surge", SappingSurge, Source.FightSpecific, BuffClassification.Other, BuffImages.GuiltExploitation),
            new Buff("Chaos Corrosion", ChaosCorrosion, Source.FightSpecific, BuffClassification.Other, BuffImages.Fractured),
            new Buff("Fixated (Qadim the Peerless)", FixatedQadimThePeerless, Source.FightSpecific, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Magma Drop", MagmaDrop, Source.FightSpecific, BuffClassification.Other, BuffImages.TargetedLocust),
            new Buff("Critical Mass", CriticalMass, Source.FightSpecific, BuffClassification.Other, BuffImages.OrbOfAscension),
            new Buff("Kinetic Abundance", KineticAbundance, Source.FightSpecific, BuffClassification.Other, BuffImages.KineticAbundance),
            new Buff("Enfeebled Force", EnfeebledForce, Source.FightSpecific, BuffClassification.Other, BuffImages.EnfeebledForce),
            new Buff("Backlashing Beam", BacklashingBeam, Source.FightSpecific, BuffClassification.Other, BuffImages.XerasBoon),
            new Buff("Clutched by Chaos", ClutchedByChaos, Source.FightSpecific, BuffClassification.Other, BuffImages.ProtectiveShadow),
            new Buff("Cleansed Conductor", CleansedConductor, Source.FightSpecific, BuffClassification.Other, BuffImages.MagicBlastIntensity),
            new Buff("Poisoned Power", PoisonedPower, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Incorporeal", Incorporeal, Source.FightSpecific, BuffClassification.Other, BuffImages.MagicAura),
            new Buff("Flare-Up", FlareUp, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.MagicAura),
            new Buff("Unbridled Chaos", UnbridledChaos, Source.FightSpecific, BuffStackType.Stacking, 3, BuffClassification.Other, BuffImages.ExposedEyes),
            new Buff("Achievement Eligibility: Power Surge", AchievementEligibilityPowerSurge, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            //////////////////////////////////////////////
            // Fractals 
            new Buff("Rigorous Certainty", RigorousCertainty, Source.Common, BuffClassification.Defensive, BuffImages.DesertCarapace),
            new Buff("Fractal Savant", FractalSavant, Source.Common, BuffClassification.Offensive, BuffImages.Malign9Infusion),
            new Buff("Fractal Prodigy", FractalProdigy, Source.Common, BuffClassification.Offensive, BuffImages.Mighty9Infusion),
            new Buff("Fractal Champion", FractalChampion, Source.Common, BuffClassification.Offensive, BuffImages.Precise9Infusion),
            new Buff("Fractal God", FractalGod, Source.Common, BuffClassification.Offensive, BuffImages.Healing9Infusion),
            new Buff("Debilitated (Toxic Sickness)", DebilitatedToxicSickness, Source.FightSpecific, BuffStackType.Stacking, 3, BuffClassification.Debuff, BuffImages.Debilitated),
            // Siax 
            new Buff("Fixated (Nightmare)", FixatedNightmare, Source.FightSpecific, BuffClassification.Other, BuffImages.Fixated),
            // Ensolyss 
            new Buff("Determination (Ensolyss)", DeterminationEnsolyss, Source.FightSpecific, BuffClassification.Other, BuffImages.GambitExhausted),
            // Shattered Observatory
            new Buff("Achievement Eligibility: Be Dynamic", AchievementEligibilityBeDynamic, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // Artsariiv
            new Buff("Enraged (Fractal)", EnragedFractal, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Corporeal Reassignment", CorporealReassignment, Source.FightSpecific, BuffClassification.Other, BuffImages.RedirectAnomaly),
            new Buff("Blinding Radiance", BlindingRadiance, Source.FightSpecific, BuffClassification.Other, BuffImages.MonsterSkill),
            new Buff("Determination (Viirastra)", DeterminationViirastra, Source.FightSpecific, BuffClassification.Other, BuffImages.GambitExhausted),
            // Arkk 
            new Buff("Fixated (Bloom 3)", FixatedBloom3, Source.FightSpecific, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Fixated (Bloom 2)", FixatedBloom2, Source.FightSpecific, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Fixated (Bloom 1)", FixatedBloom1, Source.FightSpecific, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Fixated (Bloom 4)", FixatedBloom4, Source.FightSpecific, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Cosmic Meteor", CosmicMeteor, Source.FightSpecific, BuffClassification.Other, BuffImages.MonsterSkill),
            new Buff("Diaphanous Shielding", DiaphanousShielding, Source.FightSpecific, BuffStackType.Stacking, 4, BuffClassification.Other, BuffImages.RedirectAnomaly),
            new Buff("Electrocuted", Electrocuted, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.AirAttunement),
            // Ai, Keeper of the Peak
            new Buff("Tidal Barrier", TidalBarrier, Source.FightSpecific, BuffClassification.Other, BuffImages.PrimedBottle),
            new Buff("Whirlwind Shield", WhirlwindShield, Source.FightSpecific, BuffClassification.Other, BuffImages.PrimedBottle),
            new Buff("Resilient Form", ResilientForm, Source.FightSpecific, BuffClassification.Other, BuffImages.CrowdFavor),
            new Buff("Cacophonous Mind", CacophonousMind, Source.FightSpecific, BuffStackType.Stacking, 20, BuffClassification.Other, BuffImages.CrowdFavor),
            new Buff("Crushing Guilt", CrushingGuilt, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Debuff, BuffImages.CrowdFavor),
            new Buff("Fixated (Fear 3)", FixatedFear3, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Fixated (Fear 2)", FixatedFear2, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Fixated (Fear 1)", FixatedFear1, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Fixated (Fear 4)", FixatedFear4, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Charged Leap", ChargedLeap, Source.FightSpecific, BuffStackType.StackingConditionalLoss, 3, BuffClassification.Other, BuffImages.CrowdFavor),
            new Buff("Tidal Bargain", TidalBargain, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Debuff, BuffImages.CrowdFavor),
            new Buff("Achievement Eligibility: Dancing with Demons", AchievementEligibilityDancingWithDemons, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Energy Dispersal", AchievementEligibilityEnergyDispersal, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            //////////////////////////////////////////////
            // Icebrood
            new Buff("Hypothermia", Hypothermia, Source.FightSpecific, BuffClassification.Debuff, BuffImages.Hypothermia),
            new Buff("Achievement Eligibility: Ice Breaker", AchievementEligibilityIceBreaker, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Smooth as Ice", AchievementEligibilitySmoothAsIce, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // Fraenir of Jormag
            new Buff("Frozen", Frozen, Source.FightSpecific, BuffClassification.Debuff, BuffImages.Frostbite),
            new Buff("Snowblind", Snowblind, Source.FightSpecific, BuffClassification.Other, BuffImages.Frostbite),
            new Buff("Achievement Eligibility: Elemental Elegy", AchievementEligibilityElementalElegy, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Fraenir Frolic", AchievementEligibilityFraenirFrolic, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: High Shaman, High Stakes", AchievementEligibilityHighShamanHighStakes, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // Voice and Claw            
            new Buff("Enraged (V&C)", EnragedVC, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Achievement Eligibility: Break It Up", AchievementEligibilityBreakItUp, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Flawless Fallen", AchievementEligibilityFlawlessFallen, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Kodan Dodger", AchievementEligibilityKodanDodger, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // Boneskinner     
            new Buff("Tormenting Aura", TormentingAura, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.Darkness),
            new Buff("Achievement Eligibility: Deathless Hunt", AchievementEligibilityDeathlessHunt, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Hold onto the Light", AchievementEligibilityHoldOntoTheLight, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // Whisper of Jormag
            new Buff("Whisper Teleport Out", WhisperTeleportOut, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Whisper Teleport Back", WhisperTeleportBack, Source.FightSpecific, BuffClassification.Other, BuffImages.Enraged),
            new Buff("Frigid Vortex", FrigidVortex, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Chains of Frost Active", ChainsOfFrostActive, Source.FightSpecific, BuffClassification.Other, BuffImages.SoulBinder),
            new Buff("Chains of Frost Application", ChainsOfFrostApplication, Source.FightSpecific, BuffClassification.Other, BuffImages.MonsterSkill),
            new Buff("Brain Freeze", BrainFreeze, Source.FightSpecific, BuffStackType.Stacking, 20, BuffClassification.Other, BuffImages.Frostbite),
            new Buff("Achievement Eligibility: Reflections in the Ice", AchievementEligibilityReflectionsInTheIce, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Slither-less", AchievementEligibilitySlitherLess, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Vortex, Interrupted", AchievementEligibilityVortexInterrupted, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // Frezie      
            new Buff("Icy Barrier", IcyBarrier, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.IcyBarrier),
            // Mai Trin
            new Buff("Shared Destruction (Mai Trin)", SharedDestructionMaiTrin, Source.FightSpecific, BuffClassification.Other, BuffImages.MonsterSkill),
            new Buff("Shared Destruction CM (Mai Trin)", SharedDestructionMaiTrinCM, Source.FightSpecific, BuffClassification.Other, BuffImages.MonsterSkill),
            new Buff("Magnetic Bomb", MagneticBomb, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Immune to damage and conditions.", ImmuneToDamageAndConditions, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            //new Buff("Mai Trin ???", 63858, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Mai Trin ?????", MaiTrinSomething, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Chaos and Destruction", ChaosAndDestruction, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.MonsterSkill),
            new Buff("Exposed (EOD Strike)", ExposedEODStrike, Source.Common, BuffStackType.Stacking, 10, BuffClassification.Debuff, BuffImages.Exposed),
            new Buff("Photon Saturation", PhotonSaturation, Source.FightSpecific, BuffClassification.Other, BuffImages.PhotonSaturation),
            new Buff("Achievement Eligibility: Leaps Abound", AchievementEligibilityLeapsAbound, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Total Coverage", AchievementEligibilityTotalCoverage, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Triangulation", AchievementEligibilityTriangulation, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // Ankka
            new Buff("Necrotic Ritual", NecroticRitual, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Ankka ???", AnkkaPlateformChanging, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Hallucinations", Hallucinations, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Energy Transfer", EnergyTransfer, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Death's Embrace", DeathsEmbraceEffect, Source.FightSpecific, BuffClassification.Other, BuffImages.MonsterSkill),
            new Buff("Death's Embrace 2", DeathsEmbrace2Effect, Source.FightSpecific, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, BuffImages.MonsterSkill),
            new Buff("Power of the Void", PowerOfTheVoid, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.PowerOfTheVoid),
            new Buff("Achievement Eligibility: Clarity", AchievementEligibilityClarity, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Gaze into the Void", AchievementEligibilityGazeIntoTheVoid, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Undevoured", AchievementEligibilityUndevoured, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // Minister Li   
            new Buff("Target Order: 1", TargetOrder1, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Target Order: 2", TargetOrder2, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Target Order: 3", TargetOrder3, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Target Order: 4", TargetOrder4, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Target Order: 5", TargetOrder5, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Stronger Together", StrongerTogether, Source.FightSpecific, BuffClassification.Other, BuffImages.GiantGrowth),
            new Buff("Vitality Equalizer 1", VitalityEqualizer1, Source.FightSpecific, BuffClassification.Other, BuffImages.SynchronizedVitality),
            new Buff("Vitality Equalizer CM", VitalityEqualizerCM, Source.FightSpecific, BuffClassification.Other, BuffImages.SynchronizedVitality),
            new Buff("Vitality Equalizer 2", VitalityEqualizer2, Source.FightSpecific, BuffClassification.Other, BuffImages.SynchronizedVitality),
            new Buff("Destructive Aura", DestructiveAura, Source.FightSpecific, BuffStackType.StackingConditionalLoss, 10, BuffClassification.Other, BuffImages.ToxicGas),
            new Buff("Equalization Matrix", EqualizationMatrix, Source.FightSpecific, BuffStackType.Stacking, 99, BuffClassification.Other, BuffImages.EqualizationMatrix),
            new Buff("Lethal Inspiration", LethalInspiration, Source.FightSpecific, BuffStackType.Stacking, 1, BuffClassification.Other, BuffImages.PowerOfTheVoid),
            new Buff("Extreme Vulnerability", ExtremeVulnerability, Source.FightSpecific, BuffClassification.Debuff, BuffImages.ExtremeVulnerability),
            new Buff("Fixated (Ankka & Kaineng Overlook)", FixatedAnkkaKainengOverlook, Source.FightSpecific, BuffStackType.CappedDuration, 999, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Shared Destruction (Li)", SharedDestructionLi, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Shared Destruction (Li CM)", SharedDestructionLiCM, Source.FightSpecific, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Debilitated", Debilitated, Source.FightSpecific, BuffStackType.Stacking, 3, BuffClassification.Debuff, BuffImages.Debilitated),
            new Buff("Infirmity", Infirmity, Source.FightSpecific, BuffStackType.Stacking, 4, BuffClassification.Debuff, BuffImages.DebilitatingVoid),
            //Harvest Temple
            new Buff("Influence of the Void", InfluenceOfTheVoidEffect, Source.FightSpecific, BuffStackType.Stacking, 20, BuffClassification.Other, BuffImages.ThrowCursedArtifact),
            new Buff("Targeted (Dragon Void)", TargetedDragonVoid, Source.FightSpecific, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Void Repulsion 1", VoidRepulsion1, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Void Repulsion 2", VoidRepulsion2, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Aerial Defense", AerialDefense, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Void Immunity", VoidImmunity, Source.FightSpecific, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Void Empowerment", VoidEmpowerment, Source.FightSpecific, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Void Shell", VoidShell, Source.FightSpecific, BuffStackType.Stacking, 3, BuffClassification.Other, BuffImages.Windfall),
            new Buff("Achievement Eligibility: Jumping the Nope Ropes", AchievementEligibilityJumpingTheNopeRopes, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Standing Together", AchievementEligibilityStandingTogether, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Voidwalker", AchievementEligibilityVoidwalker, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            // Old Lion's Court
            new Buff("Fixated (Old Lion's Court)", FixatedOldLionsCourt, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.Fixated),
            new Buff("Ergo Shear", ErgoShear, Source.FightSpecific, BuffClassification.Other, BuffImages.ErgoShear),
            new Buff("Ergo Shear CM", ErgoShearCM, Source.FightSpecific, BuffClassification.Other, BuffImages.ErgoShear),
            new Buff("Tidal Torment", TidalTorment, Source.FightSpecific, BuffClassification.Other, BuffImages.TidalTorment),
            new Buff("Tidal Torment CM", TidalTormentCM, Source.FightSpecific, BuffClassification.Other, BuffImages.TidalTorment),
            new Buff("Naked Singularity", NakedSingularity, Source.FightSpecific, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.Unknown),
            new Buff("Naked Singularity CM", NakedSingularityCM, Source.FightSpecific, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.Unknown),
            new Buff("Ley-Woven Shielding", LeyWovenShielding, Source.FightSpecific, BuffClassification.Other, BuffImages.LeyEnergyShield),
            new Buff("Malfunctioning Ley-Woven Shielding", MalfunctioningLeyWovenShielding, Source.FightSpecific, BuffClassification.Other, BuffImages.Unblockable),
            new Buff("Power Transfer", PowerTransfer, Source.FightSpecific, BuffStackType.Queue, 99, BuffClassification.Other, BuffImages.SpiritEnergyTracker),
            new Buff("Empowered (Watchknight Triumverate)", EmpowererWatchknightTriumverate, Source.FightSpecific, BuffStackType.Queue, 99, BuffClassification.Other, BuffImages.ChargingEnergies),
            new Buff("Achievement Eligibility: Aether Aversion", AchievementEligibilityAetherAversion, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Fear Not This Knight", AchievementEligibilityFearNotThisKnight, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Static-Dynamic Synergy", AchievementEligibilityStaticDynamicSynergy, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            //Open World Soo-Won
            new Buff("Jade Tech Offensive Overcharge", JadeTechOffensiveOvercharge, Source.FightSpecific, BuffStackType.Queue, 9, BuffClassification.Offensive, BuffImages.JadeTechOffensive),
            new Buff("Jade Tech Defensive Overcharge", JadeTechDefensiveOvercharge, Source.FightSpecific, BuffStackType.Queue, 9, BuffClassification.Defensive, BuffImages.JadeTechDefensive),
            new Buff("Enhancement (+5% Damage vs. Dragonvoid)", EnhancementDragonsEnd, Source.FightSpecific, BuffClassification.Offensive, BuffImages.NourishmentUtility),
            new Buff("Soul Reunited", SoulReunited, Source.FightSpecific, BuffClassification.Offensive, BuffImages.AllysAidPoweredUp),
            new Buff("Wisp Form", WispForm, Source.FightSpecific, BuffClassification.Other, BuffImages.FracturedSpirit),
            new Buff("Void Corruption", VoidCorruption, Source.FightSpecific, BuffStackType.Stacking, 10, BuffClassification.Debuff, BuffImages.VoidCorruption),
            new Buff("Void Chaos", VoidChaos, Source.FightSpecific, BuffClassification.Debuff, BuffImages.SpectralAgony),
            new Buff("Hardened Shell", HardenedShell, Source.FightSpecific, BuffClassification.Other, BuffImages.DefensiveInspiration),
            new Buff("Dragon's End Contributor 1", DragonsEndContributor1, Source.FightSpecific, BuffClassification.Support, BuffImages.SeraphMorale01),
            new Buff("Dragon's End Contributor 2", DragonsEndContributor2, Source.FightSpecific, BuffClassification.Support, BuffImages.SeraphMorale02),
            new Buff("Dragon's End Contributor 3", DragonsEndContributor3, Source.FightSpecific, BuffClassification.Support, BuffImages.SeraphMorale03),
            new Buff("Dragon's End Contributor 4", DragonsEndContributor4, Source.FightSpecific, BuffClassification.Support, BuffImages.SeraphMorale04),
            new Buff("Dragon's End Contributor 5", DragonsEndContributor5, Source.FightSpecific, BuffClassification.Support, BuffImages.SeraphMorale05),
            new Buff("Dragon's End Contributor 6", DragonsEndContributor6, Source.FightSpecific, BuffClassification.Support, BuffImages.SeraphMorale06),
            new Buff("Dragon's End Contributor 7", DragonsEndContributor7, Source.FightSpecific, BuffClassification.Support, BuffImages.SeraphMorale07),
            new Buff("Dragon's End Contributor 8", DragonsEndContributor8, Source.FightSpecific, BuffClassification.Support, BuffImages.SeraphMorale08),
            new Buff("Dragon's End Contributor 9", DragonsEndContributor9, Source.FightSpecific, BuffClassification.Support, BuffImages.SeraphMorale09),
            new Buff("Dragon's End Contributor 10", DragonsEndContributor10, Source.FightSpecific, BuffClassification.Support, BuffImages.SeraphMorale10),
            new Buff("Achievement Eligibility: Competent Commander", AchievementEligibilityCompetentCommander, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: The Floor Is Void", AchievementEligibilityTheFloorIsVoid, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: True Ending", AchievementEligibilityTrueEnding, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
            new Buff("Achievement Eligibility: Untarnished Spirits", AchievementEligibilityUntarnishedSpirits, Source.FightSpecific, BuffClassification.Other, BuffImages.AchievementEffect),
        };

        internal static readonly List<Buff> NormalFoods = new List<Buff>
        {
            new Buff("Malnourished", Malnourished, Source.Item, BuffClassification.Nourishment, BuffImages.Malnourished),
            new Buff("Plate of Truffle Steak", PlateOfTruffleSteak, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfTruffleSteak),
            new Buff("Bowl of Sweet and Spicy Butternut Squash Soup", BowlOfSweetAndSpicyButternutSquashSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSweetAndSpicyButternutSquashSoup),
            new Buff("Bowl of Curry Butternut Squash Soup", BowlOfCurryButternutSquashSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfCurryButternutSquashSoup),
            new Buff("Bowl of Fancy Potatoe and Leek Soup", BowlOfFancyPotatoeAndLeekSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfFancyPotatoeAndLeekSoup),
            new Buff("Red-Lentil Saobosa", RedLentilSaobosa, Source.Item, BuffClassification.Nourishment, BuffImages.RedLentilSaobosa),
            new Buff("Super Veggie Pizza", SuperVeggiePizza, Source.Item, BuffClassification.Nourishment, BuffImages.SuperVeggiePizza),
            new Buff("Rare Veggie Pizza", RareVeggiePizza, Source.Item, BuffClassification.Nourishment, BuffImages.RareVeggiePizza),
            new Buff("Bowl of Garlic Kale Sautee", Unknown, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfGarlicKaleSautee),
            new Buff("Koi Cake", Unknown, Source.Item, BuffClassification.Nourishment, BuffImages.KoiCake),
            new Buff("Prickly Pear Pie", PricklyPearPie, Source.Item, BuffClassification.Nourishment, BuffImages.PricklyPearPie),
            new Buff("Bowl of Nopalitos Sauté", Unknown, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfNopalitosSaute),
            new Buff("Loaf of Candy Cactus Cornbread", LoafOfCandyCactusCornbread, Source.Item, BuffClassification.Nourishment, BuffImages.LoafOfCandyCactusCornbread),
            new Buff("Delicious Rice Ball", DeliciousRiceBall, Source.Item, BuffClassification.Nourishment, BuffImages.DeliciousRiceBall),
            new Buff("Slice of Allspice Cake", SliceOfAllspiceCake, Source.Item, BuffClassification.Nourishment, BuffImages.SliceOfAllspiceCake),
            new Buff("Fried Golden Dumpling", FriedGoldenDumpling, Source.Item, BuffClassification.Nourishment, BuffImages.FriedGoldenDumpling),
            new Buff("Bowl of Seaweed Salad", BowlOfSeaweedSalad, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSeaweedSalad),
            new Buff("Bowl of Orrian Truffle and Meat Stew", BowlOfOrrianTruffleAndMeatStew, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfOrrianTruffleAndMeatStew),
            new Buff("Plate of Mussels Gnashblade", PlateOfMusselsGnashblade, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfMusselsGnashblade),
            new Buff("Spring Roll", SpringRoll, Source.Item, BuffClassification.Nourishment, BuffImages.SpringRoll),
            new Buff("Plate of Beef Rendang", PlateOfBeefRendang, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfBeefRendang),
            new Buff("Dragon's Revelry Starcake", DragonsRevelryStarcake, Source.Item, BuffClassification.Nourishment, BuffImages.DragonsRevelryStarcake),
            new Buff("Avocado Smoothie", AvocadoSmoothie, Source.Item, BuffClassification.Nourishment, BuffImages.AvocadoSmoothie),
            new Buff("Carrot Souffle", Unknown, Source.Item, BuffClassification.Nourishment, BuffImages.CarrotSouffle), //same as Dragon's_Breath_Bun
            new Buff("Plate of Truffle Steak Dinner", Unknown, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfTruffleSteakDinner), //same as Dragon's Breath Bun
            new Buff("Dragon's Breath Bun", DragonsBreathBun, Source.Item, BuffClassification.Nourishment, BuffImages.DragonsBreathBun),
            new Buff("Karka Egg Omelet", KarkaEggOmelet, Source.Item, BuffClassification.Nourishment, BuffImages.KarkaEggOmelet),
            new Buff("Steamed Red Dumpling", SteamedRedDumpling, Source.Item, BuffClassification.Nourishment, BuffImages.SteamedRedDumpling),
            new Buff("Saffron Stuffed Mushroom", Unknown, Source.Item, BuffClassification.Nourishment, BuffImages.SaffronStuffedMushroom), //same as Karka Egg Omelet
            new Buff("Soul Pastry", SoulPastry, Source.Item, BuffClassification.Nourishment, BuffImages.SoulPastry),
            new Buff("Bowl of Fire Meat Chili", BowlOfFireMeatChili, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfFireMeatChili),
            new Buff("Plate of Fire Flank Steak", PlateOfFireFlankSteak, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfFireFlankSteak),
            new Buff("Plate of Orrian Steak Frittes", PlateOfOrrianSteakFrittes, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfOrrianSteakFrittes),
            new Buff("Spicier Flank Steak", SpicierFlankSteak, Source.Item, BuffClassification.Nourishment, BuffImages.SpicierFlankSteak),
            new Buff("Mango Pie", MangoPie, Source.Item, BuffClassification.Nourishment, BuffImages.MangoPie),
            new Buff("Block of Tofu", BlockOfTofu, Source.Item, BuffClassification.Nourishment, BuffImages.BlockOfTofu),
            new Buff("Fishy Rice Bowl", FishyRiceBowl, Source.Item, BuffClassification.Nourishment, BuffImages.FishyRiceBowl),
            new Buff("Meaty Rice Bowl", MeatyRiceBowl, Source.Item, BuffClassification.Nourishment, BuffImages.MeatyRiceBowl),
            new Buff("Plate of Kimchi Pancakes", PlateOfKimchiPancakes, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfKimchiPancakes),
            new Buff("Bowl of Kimchi Tofu Stew", BowlOfKimchiTofuStew, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfKimchiTofuStew),
            new Buff("Meaty Asparagus Skewer", MeatyAsparagusSkewer, Source.Item, BuffClassification.Nourishment, BuffImages.MeatyAsparagusSkewer),
            new Buff("Bloodstone Bisque", BloodstoneBisque, Source.Item, BuffClassification.Nourishment, BuffImages.BloodstoneBisque),
            new Buff("Plate of Spicy Moa Wings", PlateOfSpicyMoaWings, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfSpicyMoaWings),
            new Buff("Steak with Winterberry Sauce", SteakWithWinterberrySauce, Source.Item, BuffClassification.Nourishment, BuffImages.SteakWithWinterberrySauce),
            new Buff("Feast of Delectable Birthday Cake", FeastOfDelectableBirthdayCake, Source.Item, BuffClassification.Nourishment, BuffImages.FeastOfDelectableBirthdayCake),
            new Buff("Jerk Poultry and Nopal Flatbread Sandwich", JerkPoultryAndNopallFlatbreadSandwich, Source.Item, BuffClassification.Nourishment, BuffImages.JerkPoultryAndNopallFlatbreadSandwich),
            new Buff("Spicy Flank Steak", SpicyFlankSteak, Source.Item, BuffClassification.Nourishment, BuffImages.SpicyFlankSteak),
            new Buff("Plate of Jerk Poultry", PlateOfJerkPoultry, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfJerkPoultry),
            new Buff("Spicy Marinated Mushroom", SpicyMarinatedMushroom, Source.Item, BuffClassification.Nourishment, BuffImages.SpicyMarinatedMushroom),
            new Buff("Bowl of Passion Fruit Tapioca Pudding", BowlOfPassionFruitTapiocaPudding, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfPassionFruitTapiocaPudding),
            new Buff("Bowl of Sawgill Mushroom Risotto", BowlOfSawgillMushroomRisotto, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSawgillMushroomRisotto),
            new Buff("Bowl of Curry Pumpkin Soup", BowlOfCurryPumpkinSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfCurryPumpkinSoup),
            new Buff("Plate of Spicy Herbed Chicken", PlateOfSpicyHerbedChicken, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfSpicyHerbedChicken),
            new Buff("Bowl of Tropical Mousse", BowlOfTropicalMousse, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfTropicalMousse),
            new Buff("Bowl of Chocolate Tapioca Pudding", BowlOfChocolateTapiocaPudding, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfChocolateTapiocaPudding),
            new Buff("Bowl of Refugee's Beet Soup", BowlOfRefugeesBeetSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfRefugeesBeetSoup),
            new Buff("Flatbread", Flatbread, Source.Item, BuffClassification.Nourishment, BuffImages.Flatbread),
            new Buff("Mushroom Loaf", MushroomLoaf, Source.Item, BuffClassification.Nourishment, BuffImages.MushroomLoaf),
            new Buff("Loaf of Omnomberry Bread", LoafOfOmnomberryBread, Source.Item, BuffClassification.Nourishment, BuffImages.LoafOfOmnomberryBread),
            new Buff("Bowl of Zesty Turnip Soup", BowlOfZestyTurnipSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfZestyTurnipSoup),
            new Buff("Plate of Eggs Benedict", PlateOfEggsBenedict, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfEggsBenedict),
            new Buff("Bowl of Firebreather Chili", BowlOfFirebreatherChili, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfFirebreatherChili),
            new Buff("Bowl of Green Chile Ice Cream", BowlOfGreenChileIceCream, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfGreenChileIceCream),
            new Buff("Bowl of Carne Khan Chili", BowlOfCarneKhanChili, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfCarneKhanChili),
            new Buff("Bowl of Truffle Risotto", BowlOfTruffleRisotto, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfTruffleRisotto),
            new Buff("Bowl of Orrian Truffle Soup", BowlOfOrrianTruffleSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfOrrianTruffleSoup),
            new Buff("Bowl of Prickly Pear Tapioca Pudding", BowlOfPricklyPearTapiocaPudding, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfPricklyPearTapiocaPudding),
            new Buff("Bowl of Sweet and Spicy Beans", BowlOfSweetAndSpicyBeans, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSweetAndSpicyBeans),
            new Buff("Bowl of \"Elon Red\"", BowlOfElonRed, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfElonRed),
            new Buff("Chocolate Omnomberry Cake", ChocolateOmnomberryCake, Source.Item, BuffClassification.Nourishment, BuffImages.ChocolateOmnomberryCake),
            new Buff("Fancy Truffle Burger", FancyTruffleBurger, Source.Item, BuffClassification.Nourishment, BuffImages.FancyTruffleBurger),
            new Buff("Bowl of Fire Veggie Chili", BowlOfFireVeggieChili, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfFireVeggieChili),
            new Buff("Bowl of Prickly Pear Sorber", BowlOfPricklyPearSorber, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfPricklyPearSorber),
            new Buff("Winterberry Sorbet", WinterberrySorbet, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfPricklyPearSorber),
            new Buff("Plate of Roasted Cactus", PlateOfRoastedCactus, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfRoastedCactus),
            new Buff("Spicy Chocolate Cookie", SpicyChocolateCookie, Source.Item, BuffClassification.Nourishment, BuffImages.SpicyChocolateCookie),
            new Buff("Omnomberry Cookie", OmnomberryCookie, Source.Item, BuffClassification.Nourishment, BuffImages.OmnomberryCookie),
            new Buff("Bowl of Tropical Fruit Salad", BowlOfTropicalFruitSalad, Source.Item, BuffClassification.Nourishment,BuffImages.BowlOfTropicalFruitSalad),
            new Buff("Bowl of Poultry Satay", BowlOfPoultrySatay, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfPoultrySatay),
            new Buff("Bowl of Saffron-scented Poultry Soup", BowlOfSaffronScentedPoultrySoup, Source.Item, BuffClassification.Nourishment,BuffImages.BowlOfSaffronScentedPoultrySoup),
            new Buff("Bowl of Saffron-Mango Ice Cream", BowlOfSaffronMangoIceCream, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSaffronMangoIceCream),
            new Buff("Whitefish Sushi", WhitefishSushi, Source.Item, BuffClassification.Nourishment, BuffImages.WhitefishSushi),
            new Buff("Yellowfish Sushi", YellowfishSushi, Source.Item, BuffClassification.Nourishment, BuffImages.YellowfishSushi),
            new Buff("Orangefish Sushi", OrangefishSushi, Source.Item, BuffClassification.Nourishment, BuffImages.OrangefishSushi),
            new Buff("Redfish Sushi", RedfishSushi, Source.Item, BuffClassification.Nourishment, BuffImages.RedfishSushi),
            new Buff("Bowl of Fish Stew", BowlOfFishStew, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfFishStew),
            new Buff("Plate of Island Pudding", PlateOfIslandPudding, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfIslandPudding),
            new Buff("Dragonfly Cupcake", DragonflyCupcake, Source.Item, BuffClassification.Nourishment, BuffImages.DragonflyCupcake),
            new Buff("Chocolate Omnomberry Cream", ChocolateOmnomberryCream, Source.Item, BuffClassification.Nourishment, BuffImages.ChocolateOmnomberryCream),
            new Buff("Cup of Lotus Fries", CupOfLotusFries, Source.Item, BuffClassification.Nourishment, BuffImages.CupOfLotusFries),
            new Buff("Spicy Pumpkin Cookie", SpicyPumpkinCookie, Source.Item, BuffClassification.Nourishment, BuffImages.SpicyPumpkinCookie),
            new Buff("Peppermint Omnomberry Bar", PeppermintOmnomberryBar, Source.Item, BuffClassification.Nourishment, BuffImages.PeppermintOmnomberryBar),
            new Buff("Can of Stewed Oysters", CanOfStewedOysters, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfStewedOysters),
            new Buff("Fried Oysters", FriedOysters, Source.Item, BuffClassification.Nourishment, BuffImages.FriedOysters),
            new Buff("Fried Oyster Sandwich", FriedOysterSandwich, Source.Item, BuffClassification.Nourishment, BuffImages.FriedOysterSandwich),
            new Buff("Oysters With Cocktail Sauce", OystersWithCocktailSauce, Source.Item, BuffClassification.Nourishment, BuffImages.OystersWithCocktailSauce),
            new Buff("Bowl of Lemongrass Mussel Pasta", BowlOfLemongrassMusselPasta, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfLemongrassMusselPasta),
            new Buff("Bowl of Mussel Soup", BowlOfMusselSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfMusselSoup),
            new Buff("Oysters with Spicy Sauce", OystersWithSpicySauce, Source.Item, BuffClassification.Nourishment, BuffImages.OystersWithSpicySauce),
            new Buff("Oysters Gnashblade", OystersGnashblade, Source.Item, BuffClassification.Nourishment, BuffImages.OystersGnashblade),
            new Buff("Oysters with Zesty Sauce", OystersWithZestySauce, Source.Item, BuffClassification.Nourishment, BuffImages.OystersWithZestySauce),
            new Buff("Oysters with Pesto Sauce", OystersWithPestoSauce, Source.Item, BuffClassification.Nourishment, BuffImages.OystersWithPestoSauce),
            new Buff("Bowl of Curry Mussel Soup", BowlOfCurryMusselSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfCurryMusselSoup),
            new Buff("Loaf of Saffron Bread", LoafOfSaffronBread, Source.Item, BuffClassification.Nourishment, BuffImages.LoafOfSaffronBread),
            new Buff("Bowl of Lemongrass Poultry Soup", BowlOfLemongrassPoultrySoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfLemongrassPoultrySoup),
            new Buff("Sweet Bean Bun", SweetBeanBun, Source.Item, BuffClassification.Nourishment, BuffImages.SweetBeanBun),
            new Buff("Bowl of Roasted Lotus Root", BowlOfRoastedLotusRoot, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfRoastedLotusRoot),
            new Buff("Bowl of Cactus Soup", BowlOfCactusSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfCactusSoup),
            new Buff("Tropical Peppermint Cake", TropicalPeppermintCake, Source.Item, BuffClassification.Nourishment, BuffImages.TropicalPeppermintCake),
            new Buff("Bowl of Red Lentil Soup", BowlOfRedLentilSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfRedLentilSoup),
            new Buff("Holographic Super Cake", HolographicSuperCake, Source.Item, BuffClassification.Nourishment, BuffImages.HolographicSuperCake),
            new Buff("Omnomberry Pie", OmnomberryPieAndSliceOfCandiedDragonRoll, Source.Item, BuffClassification.Nourishment, BuffImages.OmnomberryPie),
            new Buff("Scoop of Mintberry Swirl Ice Cream", ScoopOfMintberrySwirlIceCream, Source.Item, BuffClassification.Nourishment, BuffImages.ScoopOfMintberrySwirlIceCream),
            new Buff("Omnomberry Ghost", OmnomberryGhost, Source.Item, BuffClassification.Nourishment, BuffImages.OmnomberryGhost),
            new Buff("Winterberry Pie", WinterberryPie, Source.Item, BuffClassification.Nourishment, BuffImages.WinterberryPie),
            new Buff("Slice of Allspice Cake with Ice Cream", SliceOfAllspiceCakeWithIceCream, Source.Item, BuffClassification.Nourishment, BuffImages.SliceOfAllspiceCakeWithIceCream),
            new Buff("Plate of Sugar Rib Roast", PlateOfSugarRibRoast, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfSugarRibRoast),
            new Buff("Ghost Pepper Popper", GhostPepperPopper, Source.Item, BuffClassification.Nourishment, BuffImages.GhostPepperPopper),
            new Buff("Bowl of Marjorys Experimental Chili", BowlOfMarjorysExperimentalChili, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfMarjorysExperimentalChili),
            new Buff("Bowl of Tapioca Pudding", BowlOfTapiocaPudding, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfTapiocaPudding),
            new Buff("Cheesy Cassava Roll", CheesyCassavaRoll, Source.Item, BuffClassification.Nourishment, BuffImages.CheesyCassavaRoll),
            new Buff("Bowl of Lotus Stirfry", BowlOfLotusStirfry, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfLotusStirfry),
            new Buff("Bowl of Truffle Sautee", BowlOfTruffleSautee, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfTruffleSautee),
            new Buff("Omnomberry Compote", OmnomberryCompote, Source.Item, BuffClassification.Nourishment, BuffImages.OmnomberryCompote),
            new Buff("Bowl of Asparagus and Sage Salad", BowlOfAsparagusAndSageSalad, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfAsparagusAndSageSalad),
            new Buff("Bowl of Winterberry Seaweed Salad", BowlOfWinterberrySeaweedSalad, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfWinterberrySeaweedSalad),
            new Buff("Jerk Poultry Flatbread Sandwich", JerkPoultryFlatbreadSandwich, Source.Item, BuffClassification.Nourishment, BuffImages.JerkPoultryFlatbreadSandwich),
            new Buff("Plate of Sweet Curried Mussels", PlateOfSweetCurriedMussels, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfSweetCurriedMussels),
            new Buff("Bowl of Spiced Red Lentil Stew", BowlOfSpicedRedLentilStew, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSpicedRedLentilStew),
            new Buff("Tarragon Stuffed Poultry", TarragonStuffedPoultry, Source.Item, BuffClassification.Nourishment, BuffImages.TarragonStuffedPoultry),
            new Buff("Plate of Lemongrass Poultry", PlateOfLemongrassPoultry, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfLemongrassPoultry),
            new Buff("Cup of Light-Roasted Coffee", CupOfLightRoastedCoffee, Source.Item, BuffClassification.Nourishment, BuffImages.CupOfLightRoastedCoffee),
            new Buff("Bowl of Fire Salsa", BowlOfFireSalsa, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfFireSalsa),
            new Buff("Bowl of Cold Wurm Stew", BowlOfColdWurmStew, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfColdWurmStew),
            new Buff("Stick of Mystery Meat", StickOfMysteryMeat, Source.Item, BuffClassification.Nourishment, BuffImages.StickOfMysteryMeat),
            new Buff("Filet of Sesame-Roasted Meat", FiletOfSesameRoastedMeat, Source.Item, BuffClassification.Nourishment, BuffImages.FiletOfSesameRoastedMeat),
            new Buff("Can of Steak and Asparagus", CanOfSteakAndAsparagus, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfSteakAndAsparagus),
            new Buff("Plate of Steak and Asparagus", PlateOfSteakAndAsparagus, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfSteakAndAsparagus),
            new Buff("Plate of Coriander Crusted Meat", PlateOfCorianderCrustedMeat, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfCorianderCrustedMeat),
            new Buff("Filet of Rosemary-Roasted Meat", FiletOfRosemaryRoastedMeat, Source.Item, BuffClassification.Nourishment, BuffImages.FiletOfRosemaryRoastedMeat),
            new Buff("Plate of Roast Meat with Mint Sauce", PlateOfRoastMeatWithMintSauce, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfRoastMeatWithMintSauce),
            new Buff("Plate of Roast Meat with Braised Leeks", PlateOfRoastMeatWithBraisedLeeks, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfRoastMeatWithBraisedLeeks),
            new Buff("Can of Roasted Meat with Braised Leeks", CanOfRoastedMeatWithBraisedLeeks, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfRoastedMeatWithBraisedLeeks),
            new Buff("Spicy Lime Steak", SpicyLimeSteak, Source.Item, BuffClassification.Nourishment, BuffImages.SpicyLimeSteak),
            new Buff("Plate of Citrus Clove Meat", PlateOfCitrusCloveMeat, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfCitrusCloveMeat),
            new Buff("Canned Spicier Flank Steak", CannedSpicierFlankSteak, Source.Item, BuffClassification.Nourishment, BuffImages.CannedSpicierFlankSteak),
            new Buff("Bowl of Simple Vegetable Soup", BowlOfSimpleVegetableSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSimpleVegetableSoup),
            new Buff("Bowl of Ice Wurm Bisque", BowlOfIceWurmBisque, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfIceWurmBisque),
            new Buff("Bowl of Kale Soup", BowlOfKaleSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfKaleSoup),
            new Buff("Bowl of Chickpea Soup", BowlOfChickpeaSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfChickpeaSoup),
            new Buff("Bowl of Artichoke Soup", BowlOfArtichokeSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfArtichokeSoup),
            new Buff("Can of Artichoke Soup", CanOfArtichokeSoup, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfArtichokeSoup),
            new Buff("Can of Potato and Leek Soup", CanOfPotatoAndLeekSoup, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfPotatoAndLeekSoup),
            new Buff("Bowl of Potato and Leek Soup", BowlOfPotatoAndLeekSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfPotatoAndLeekSoup),
            new Buff("Bowl of Pumpkin Bisque", BowlOfPumpkinBisque, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfPumpkinBisque),
            new Buff("Bowl of Yam Soup", BowlOfYamSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfYamSoup),
            new Buff("Bowl of Tomato Zucchini Soup", BowlOfTomatoZucchiniSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfTomatoZucchiniSoup),
            new Buff("Bowl of Cauliflower Soup", BowlOfCauliflowerSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfCauliflowerSoup),
            new Buff("Bowl of Butternut Squash Soup", BowlOfButternutSquashSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfButternutSquashSoup),
            new Buff("Can of Butternut Squash Soup", CanOfButternutSquashSoup, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfButternutSquashSoup),
            new Buff("Plate of Pasta with Tomato Sauce", PlateOfPastaWithTomatoSauce, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfPastaWithTomatoSauce),
            new Buff("Meatball Dinner", MeatballDinner, Source.Item, BuffClassification.Nourishment, BuffImages.MeatballDinner),
            new Buff("Bowl of Krytan Meatball Dinner", BowlOfKrytanMeatballDinner, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfKrytanMeatballDinner),
            new Buff("Bowl of Pesto Pasta Salad", BowlOfPestoPastaSalad, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfPestoPastaSalad),
            new Buff("Bowl of Poultry Tarragon Pasta", BowlOfPoultryTarragonPasta, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfPoultryTarragonPasta),
            new Buff("Can of Poultry Tarragon Pasta", CanOfPoultryTarragonPasta, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfPoultryTarragonPasta),
            new Buff("Plate of Frostgorge Clams", PlateOfFrostgorgeClams, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfFrostgorgeClams),
            new Buff("Bowl of Salad a la Consortium", BowlOfSaladALaConsortium, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSaladALaConsortium),
            new Buff("Passion Fruit Coconut Cookie", PassionFruitCoconutCookie, Source.Item, BuffClassification.Nourishment, BuffImages.PassionFruitCoconutCookie),
            new Buff("Canned Stuffed Artichoke with Tropical Dressing", CannedStuffedArtichokeWithTropicalDressing, Source.Item, BuffClassification.Nourishment, BuffImages.CannedStuffedArtichokeWithTropicalDressing),
            new Buff("Stuffed Artichoke with Tropical Dressing", StuffedArtichokeWithTropicalDressing, Source.Item, BuffClassification.Nourishment, BuffImages.StuffedArtichokeWithTropicalDressing),
            new Buff("Grumble Cake", GrumbleCake, Source.Item, BuffClassification.Nourishment, BuffImages.GrumbleCake),
            new Buff("Fried Banana Chips", FriedBananaChips, Source.Item, BuffClassification.Nourishment, BuffImages.FriedBananaChips),
            new Buff("Bowl of Dolyak Stew", BowlOfDolyakStew, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfDolyakStew),
            new Buff("Loaf of Banana Bread", LoafOfBananaBread, Source.Item, BuffClassification.Nourishment, BuffImages.LoafOfBananaBread),
            new Buff("Loaf of Zucchini Bread", LoafOfZucchiniBread, Source.Item, BuffClassification.Nourishment, BuffImages.LoafOfZucchiniBread),
            new Buff("Slice of Pumpkin Bread", SliceOfPumpkinBread, Source.Item, BuffClassification.Nourishment, BuffImages.SliceOfPumpkinBread),
            new Buff("Loaf of Raspberry Peach Bread", LoafOfRaspberryPeachBread, Source.Item, BuffClassification.Nourishment, BuffImages.LoafOfRaspberryPeachBread),
            new Buff("Can of Raspberry Peach Bread", CanOfRaspberryPeachBread, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfRaspberryPeachBread),
            new Buff("Grilled Mushroom", GrilledMushroom, Source.Item, BuffClassification.Nourishment, BuffImages.GrilledMushroom),
            new Buff("Veggie Burger", VeggieBurger, Source.Item, BuffClassification.Nourishment, BuffImages.VeggieBurger),
            new Buff("Grilled Portobello Mushroom", GrilledPortobelloMushroom, Source.Item, BuffClassification.Nourishment, BuffImages.GrilledPortobelloMushroom),
            new Buff("Bowl of Mushroom Risotto", BowlOfMushroomRisotto, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfMushroomRisotto),
            new Buff("Bowl of Mushroom and Asparagus Risotto", BowlOfMushroomAndAsparagusRisotto, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfMushroomAndAsparagusRisotto),
            new Buff("Bowl of Creamy Portobello Soup", BowlOfCreamyPortobelloSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfCreamyPortobelloSoup),
            new Buff("Can of Mushroom and Asparagus Risotto", CanOfMushroomAndAsparagusRisotto, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfMushroomAndAsparagusRisotto),
            new Buff("Bowl of Fancy Creamy Mushroom Soup", BowlOfFancyCreamyMushroomSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfFancyCreamyMushroomSoup),
            new Buff("Can of Snow Truffle Soup", CanOfSnowTruffleSoup, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfSnowTruffleSoup),
            new Buff("Bowl of Snow Truffle Soup", BowlOfSnowTruffleSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSnowTruffleSoup),
            new Buff("Cheese Pizza", CheesePizza, Source.Item, BuffClassification.Nourishment, BuffImages.CheesePizza),
            new Buff("Veggie Pizza", VeggiePizza, Source.Item, BuffClassification.Nourishment, BuffImages.VeggiePizza),
            new Buff("Mushroom Pizza", MushroomPizza, Source.Item, BuffClassification.Nourishment, BuffImages.MushroomPizza),
            new Buff("Fancy Veggie Pizza", FancyVeggiePizza, Source.Item, BuffClassification.Nourishment, BuffImages.FancyVeggiePizza),
            new Buff("Bowl of Simple Meat Chili", BowlOfSimpleMeatChili, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSimpleMeatChili),
            new Buff("Bowl of Chili and Avocado", BowlOfChiliAndAvocado, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfChiliAndAvocado),
            new Buff("Bowl of Spiced Meat Chili", BowlOfSpicedMeatChili, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSpicedMeatChili),
            new Buff("Bowl of Spicy Meat Chili", BowlOfSpicyMeatChili, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSpicyMeatChili),
            new Buff("Can of Spicy Meat Chili", CanOfSpicyMeatChili, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfSpicyMeatChili),
            new Buff("WhiteCake", WhiteCake, Source.Item, BuffClassification.Nourishment, BuffImages.WhiteCake),
            new Buff("Chocolate Cake", ChocolateCake, Source.Item, BuffClassification.Nourishment, BuffImages.ChocolateCake),
            new Buff("Chocolate Cherry Cake", ChocolateCherryCake, Source.Item, BuffClassification.Nourishment, BuffImages.ChocolateCherryCake),
            new Buff("Orange Coconut Cake", OrangeCoconutCake, Source.Item, BuffClassification.Nourishment, BuffImages.OrangeCoconutCake),
            new Buff("Chocolate Raspberry Cake", ChocolateRaspberryCake, Source.Item, BuffClassification.Nourishment, BuffImages.ChocolateRaspberryCake),
            new Buff("Canned Chocolate Raspberry Cake", CannedChocolateRaspberryCake, Source.Item, BuffClassification.Nourishment, BuffImages.CannedChocolateRaspberryCake),
            new Buff("Cheeseburger", Cheeseburger, Source.Item, BuffClassification.Nourishment, BuffImages.Cheeseburger),
            new Buff("Spinach Burger", SpinachBurger, Source.Item, BuffClassification.Nourishment, BuffImages.SpinachBurger),
            new Buff("Deluxe Burger", DeluxeBurger, Source.Item, BuffClassification.Nourishment, BuffImages.DeluxeBurger),
            new Buff("Horseradish Burger", HorseradishBurger, Source.Item, BuffClassification.Nourishment, BuffImages.HorseradishBurger),
            new Buff("Spicy Cheeseburger", SpicyCheeseburger, Source.Item, BuffClassification.Nourishment, BuffImages.SpicyCheeseburger),
            new Buff("Boxed Spicy Cheeseburger", BoxedSpicyCheeseburger, Source.Item, BuffClassification.Nourishment, BuffImages.BoxedSpicyCheeseburger),
            new Buff("Bowl of Simple Bean Chili", BowlOfSimpleBeanChili, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSimpleBeanChili),
            new Buff("Bowl of Fancy Bean Chili", BowlOfFancyBeanChili, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfFancyBeanChili),
            new Buff("Bowl of Zucchini Chili", BowlOfZucchiniChili, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfZucchiniChili),
            new Buff("Bowl of Spiced Veggie Chili", BowlOfSpicedVeggieChili, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSpicedVeggieChili),
            new Buff("Bowl of Spicy Veggie Chili", BowlOfSpicyVeggieChili, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSpicyVeggieChili),
            new Buff("Sugar Cookie", SugarCookie, Source.Item, BuffClassification.Nourishment, BuffImages.SugarCookie),
            new Buff("Strawberry Cookie", StrawberryCookie, Source.Item, BuffClassification.Nourishment, BuffImages.StrawberryCookie),
            new Buff("Chocolate Chip Cookie", ChocolateChipCookie, Source.Item, BuffClassification.Nourishment, BuffImages.ChocolateChipCookie),
            new Buff("Candy Corn Cookie", CandyCornCookie, Source.Item, BuffClassification.Nourishment, BuffImages.CandyCornCookie),
            new Buff("Chocolate Mint Cookie", ChocolateMintCookie, Source.Item, BuffClassification.Nourishment, BuffImages.ChocolateMintCookie),
            new Buff("Chocolate Raspberry Cookie", ChocolateRaspberryCookie, Source.Item, BuffClassification.Nourishment, BuffImages.ChocolateRaspberryCookie),
            new Buff("Glazed Chocolate Raspberry Cookie", GlazedChocolateRaspberryCookie, Source.Item, BuffClassification.Nourishment, BuffImages.GlazedChocolateRaspberryCookie),
            new Buff("Boxed Chocolate Raspberry Cookie", BoxedChocolateRaspberryCookie, Source.Item, BuffClassification.Nourishment, BuffImages.BoxedChocolateRaspberryCookie),
            new Buff("Cherry Cookie", CherryCookie, Source.Item, BuffClassification.Nourishment, BuffImages.CherryCookie),
            new Buff("Blackberry Cookie", BlackberryCookie, Source.Item, BuffClassification.Nourishment, BuffImages.BlackberryCookie),
            new Buff("Peach Cookie", PeachCookie, Source.Item, BuffClassification.Nourishment, BuffImages.PeachCookie),
            new Buff("Boxed Peach Cookie", BoxedPeachCookie, Source.Item, BuffClassification.Nourishment, BuffImages.BoxedPeachCookie),
            new Buff("Cherry Passion Fruit Cake", CherryPassionFruitCake, Source.Item, BuffClassification.Nourishment, BuffImages.CherryPassionFruitCake),
            new Buff("Orange Passion Fruit Tart", OrangePassionFruitTart, Source.Item, BuffClassification.Nourishment, BuffImages.OrangePassionFruitTart),
            new Buff("Canned Raspberry Passion Fruit Compote", CannedRaspberryPassionFruitCompote, Source.Item, BuffClassification.Nourishment, BuffImages.CannedRaspberryPassionFruitCompote),
            new Buff("Raspberry Passion Fruit Compote", RaspberryPassionFruitCompote, Source.Item, BuffClassification.Nourishment, BuffImages.RaspberryPassionFruitCompote),
            new Buff("Bowl of Poultry Noodle Soup", BowlOfPoultryNoodleSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfPoultryNoodleSoup),
            new Buff("Bowl of Savory Spinach and Poultry Soup", BowlOfSavorySpinachAndPoultrySoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSavorySpinachAndPoultrySoup),
            new Buff("Bowl of Kale and Poultry Soup", BowlOfKaleAndPoultrySoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfKaleAndPoultrySoup),
            new Buff("Bowl of Hearty Poultry Soup", BowlOfHeartyPoultrySoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfHeartyPoultrySoup),
            new Buff("Bowl of Poultry and Winter Vegetable Soup", BowlOfPoultryAndWinterVegetableSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfPoultryAndWinterVegetableSoup),
            new Buff("Can of Hearty Poultry Soup", CanOfHeartyPoultrySoup, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfHeartyPoultrySoup),
            new Buff("Bowl of Chocolate Chip Ice Cream", BowlOfChocolateChipIceCream, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfChocolateChipIceCream),
            new Buff("Bowl of Candy Corn Ice Cream", BowlOfCandyCornIceCream, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfCandyCornIceCream),
            new Buff("Bowl of Blueberry Chocolate Chunk Ice Cream", BowlOfBlueberryChocolateChunkIceCream, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfBlueberryChocolateChunkIceCream),
            new Buff("Bowl of Ginger-Lime Ice Cream", BowlOfGingerLimeIceCream, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfGingerLimeIceCream),
            new Buff("Bowl of Mint Chocolate Chip Ice Cream", BowlOfMintChocolateChipIceCream, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfMintChocolateChipIceCream),
            new Buff("Bowl of Peach Raspberry Swirl Ice Cream", BowlOfPeachRaspberrySwirlIceCream, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfPeachRaspberrySwirlIceCream),
            new Buff("Meat Pie", MeatPie, Source.Item, BuffClassification.Nourishment, BuffImages.MeatPie),
            new Buff("Candied Apple", CandiedApple, Source.Item, BuffClassification.Nourishment, BuffImages.CandiedApple),
            new Buff("Caramel", Caramel, Source.Item, BuffClassification.Nourishment, BuffImages.Caramel),
            new Buff("Chocolate Banana", ChocolateBanana, Source.Item, BuffClassification.Nourishment, BuffImages.ChocolateBanana),
            new Buff("Lemon Bar", LemonBar, Source.Item, BuffClassification.Nourishment, BuffImages.LemonBar),
            new Buff("Candy Corn Cake", CandyCornCake, Source.Item, BuffClassification.Nourishment, BuffImages.CandyCornCake),
            new Buff("Turrón Slice", TurronSlice, Source.Item, BuffClassification.Nourishment, BuffImages.TurronSlice),
            new Buff("Cherry Tart", CherryTart, Source.Item, BuffClassification.Nourishment, BuffImages.CherryTart),
            new Buff("Ginger Pear Tart", GingerPearTart, Source.Item, BuffClassification.Nourishment, BuffImages.GingerPearTart),
            new Buff("Glazed Pear Tart", GlazedPearTart, Source.Item, BuffClassification.Nourishment, BuffImages.GlazedPearTart),
            new Buff("Peach Tart", PeachTart, Source.Item, BuffClassification.Nourishment, BuffImages.PeachTart),
            new Buff("Canned Peach Tart", CannedPeachTart, Source.Item, BuffClassification.Nourishment, BuffImages.CannedPeachTart),
            new Buff("Glazed Peach Tart", GlazedPeachTart, Source.Item, BuffClassification.Nourishment, BuffImages.GlazedPeachTart),
            new Buff("Strawberry Ghost", StrawberryGhost, Source.Item, BuffClassification.Nourishment, BuffImages.StrawberryGhost),
            new Buff("Chocolate Cherry", ChocolateCherry, Source.Item, BuffClassification.Nourishment, BuffImages.ChocolateCherry),
            new Buff("Chocolate Orange", ChocolateOrange, Source.Item, BuffClassification.Nourishment, BuffImages.ChocolateOrange),
            new Buff("Chocolate Raspberry Cream", ChocolateRaspberryCream, Source.Item, BuffClassification.Nourishment, BuffImages.ChocolateRaspberryCream),
            new Buff("Boxed Chocolate Raspberry Cream", BoxedChocolateRaspberryCream, Source.Item, BuffClassification.Nourishment, BuffImages.BoxedChocolateRaspberryCream),
            new Buff("Yam Fritter", YamFritter, Source.Item, BuffClassification.Nourishment, BuffImages.YamFritter),
            new Buff("Chickpea Fritter", ChickpeaFritter, Source.Item, BuffClassification.Nourishment, BuffImages.ChickpeaFritter),
            new Buff("Eggplant Fritter", EggplantFritter, Source.Item, BuffClassification.Nourishment, BuffImages.EggplantFritter),
            new Buff("Canned Eggplant Fritter", CannedEggplantFritter, Source.Item, BuffClassification.Nourishment, BuffImages.CannedEggplantFritter),
            new Buff("Cherry Almond Bar", CherryAlmondBar, Source.Item, BuffClassification.Nourishment, BuffImages.CherryAlmondBar),
            new Buff("Piece of Candy Corn Almond Brittle", PieceOfCandyCornAlmondBrittle, Source.Item, BuffClassification.Nourishment, BuffImages.PieceOfCandyCornAlmondBrittle),
            new Buff("Orange Coconut Bar", OrangeCoconutBar, Source.Item, BuffClassification.Nourishment, BuffImages.OrangeCoconutBar),
            new Buff("Boxed Raspberry Peach Bar", BoxedRaspberryPeachBar, Source.Item, BuffClassification.Nourishment, BuffImages.BoxedRaspberryPeachBar),
            new Buff("Homemade Campfire Treat", HomemadeCampfireTreat, Source.Item, BuffClassification.Nourishment, BuffImages.HomemadeCampfireTreat),
            new Buff("Raspberry Peach Bar", RaspberryPeachBar, Source.Item, BuffClassification.Nourishment, BuffImages.RaspberryPeachBar),
            new Buff("Omnomberry Bar", OmnomberryBar, Source.Item, BuffClassification.Nourishment, BuffImages.OmnomberryBar),
            new Buff("Longevity Noodles", LongevityNoodles, Source.Item, BuffClassification.Nourishment, BuffImages.LongevityNoodles),
            new Buff("Slice of Cinnamon Toast", SliceOfCinnamonToast, Source.Item, BuffClassification.Nourishment, BuffImages.SliceOfCinnamonToast),
            new Buff("Slice of Garlic Bread", SliceOfGarlicBread, Source.Item, BuffClassification.Nourishment, BuffImages.SliceOfGarlicBread),
            new Buff("Loaf of Rosemary Bread", LoafOfRosemaryBread, Source.Item, BuffClassification.Nourishment, BuffImages.LoafOfRosemaryBread),
            new Buff("Slice of Spiced Bread", SliceOfSpicedBread, Source.Item, BuffClassification.Nourishment, BuffImages.SliceOfSpicedBread),
            new Buff("Loaf of Tarragon Bread", LoafOfTarragonBread, Source.Item, BuffClassification.Nourishment, BuffImages.LoafOfTarragonBread),
            new Buff("Canned Tarragon Bread", CannedTarragonBread, Source.Item, BuffClassification.Nourishment, BuffImages.CannedTarragonBread),
            new Buff("Bowl of Simple Poultry Soup", BowlOfSimplePoultrySoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSimplePoultrySoup),
            new Buff("Bowl of Clam Chowder", BowlOfClamChowder, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfClamChowder),
            new Buff("Spinach Salad", SpinachSalad, Source.Item, BuffClassification.Nourishment, BuffImages.SpinachSalad),
            new Buff("Bowl of Dilled Clam Chowder", BowlOfDilledClamChowder, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfDilledClamChowder),
            new Buff("Bowl of Chickpea and Poultry Soup", BowlOfChickpeaAndPoultrySoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfChickpeaAndPoultrySoup),
            new Buff("Bowl of Poultry and Leek Soup", BowlOfPoultryAndLeekSoup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfPoultryAndLeekSoup),
            new Buff("Can of Poultry and Leek Soup", CanOfPoultryAndLeekSoup, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfPoultryAndLeekSoup),
            new Buff("Mashed Potato", MashedPotato, Source.Item, BuffClassification.Nourishment, BuffImages.MashedPotato),
            new Buff("Bowl of Outrider Stew", BowlOfOutriderStew, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfOutriderStew),
            new Buff("Bowl of Mashed Yams", BowlOfMashedYams, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfMashedYams),
            new Buff("Roasted Rutabaga", RoastedRutabaga, Source.Item, BuffClassification.Nourishment, BuffImages.RoastedRutabaga),
            new Buff("Roasted Parsnip", RoastedParsnip, Source.Item, BuffClassification.Nourishment, BuffImages.RoastedParsnip),
            new Buff("Apple Pie", ApplePie, Source.Item, BuffClassification.Nourishment, BuffImages.ApplePie),
            new Buff("Bowl of Candy Corn Custard", BowlOfCandyCornCustard, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfCandyCornCustard),
            new Buff("Heart-Shaped Cookie", HeartShapedCookie, Source.Item, BuffClassification.Nourishment, BuffImages.HeartShapedCookie),
            new Buff("Grape Pie", GrapePie, Source.Item, BuffClassification.Nourishment, BuffImages.GrapePie),
            new Buff("Pumpkin Pie", PumpkinPie, Source.Item, BuffClassification.Nourishment, BuffImages.PumpkinPie),
            new Buff("Glazed Pumpkin Pie", GlazedPumpkinPie, Source.Item, BuffClassification.Nourishment, BuffImages.GlazedPumpkinPie),
            new Buff("Peach Pie", PeachPie, Source.Item, BuffClassification.Nourishment, BuffImages.PeachPie),
            new Buff("Canned Peach Pie Filling", CannedPeachPieFilling, Source.Item, BuffClassification.Nourishment, BuffImages.CannedPeachPieFilling),
            new Buff("Blueberry Pie", BlueberryPieAndSliceOfRainbowCake, Source.Item, BuffClassification.Nourishment, BuffImages.BlueberryPieAndSliceOfRainbowCake),
            new Buff("Strawberry Pie", StrawberryPieAndCupcake, Source.Item, BuffClassification.Nourishment, BuffImages.StrawberryPieAndCupcake),
            new Buff("Cherry Pie", CherryPie, Source.Item, BuffClassification.Nourishment, BuffImages.CherryPie),
            new Buff("Blackberry Pie", BlackberryPie, Source.Item, BuffClassification.Nourishment, BuffImages.BlackberryPie),
            new Buff("Canned Mixed Berry Pie", CannedMixedBerryPie, Source.Item, BuffClassification.Nourishment, BuffImages.CannedMixedBerryPie),
            new Buff("Mixed Berry Pie", MixedBerryPie, Source.Item, BuffClassification.Nourishment, BuffImages.MixedBerryPie),
            new Buff("Can of Spicy Veggie Chili", CanOfSpicyVeggieChili, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfSpicyVeggieChili),
            new Buff("Chili Pepper Popper", ChiliPepperPopper, Source.Item, BuffClassification.Nourishment, BuffImages.ChiliPepperPopper),
            new Buff("Stuffed Pepper", StuffedPepper, Source.Item, BuffClassification.Nourishment,BuffImages.StuffedPepper),
            new Buff("Stuffed Zucchini", StuffedZucchini, Source.Item, BuffClassification.Nourishment, BuffImages.StuffedZucchini),
            new Buff("Bowl of Hummus", BowlOfHummus, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfHummus),
            new Buff("Roasted Artichoke", RoastedArtichoke, Source.Item, BuffClassification.Nourishment, BuffImages.RoastedArtichoke),
            new Buff("Canned Roasted Artichoke", CannedRoastedArtichoke, Source.Item, BuffClassification.Nourishment, BuffImages.CannedRoastedArtichoke),
            new Buff("Bowl of Simple Meat Stew", BowlOfSimpleMeatStew, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSimpleMeatStew),
            new Buff("Bowl of Hearty Red Meat Stew", BowlOfHeartyRedMeatStew, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfHeartyRedMeatStew),
            new Buff("Bowl of Meat and Cabbage Stew", BowlOfMeatAndCabbageStew, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfMeatAndCabbageStew),
            new Buff("Bowl of Spiced Meat and Cabbage Stew", BowlOfSpicedMeatAndCabbageStew, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSpicedMeatAndCabbageStew),
            new Buff("Can of Meat and Winter Vegetable Stew", CanOfMeatAndWinterVegetableStew, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfMeatAndWinterVegetableStew),
            new Buff("Bowl of Meat and Winter Vegetable Stew", BowlOfMeatAndWinterVegetableStew, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfMeatAndWinterVegetableStew),
            new Buff("Bowl of Simple Stirfry", BowlOfSimpleStirfry, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSimpleStirfry),
            new Buff("Bowl of Garlic Spinach Sautee", BowlOfGarlicSpinachSautee, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfGarlicSpinachSautee),
            new Buff("Bowl of Avocado Stirfry", BowlOfAvocadoStirfry, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfAvocadoStirfry),
            new Buff("Bowl of Eggplant Stirfry", BowlOfEggplantStirfry, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfEggplantStirfry),
            new Buff("Can of Eggplant Stir-Fry", CanOfEggplantStirfry, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfEggplantStirfry),
            new Buff("Bowl of Sauteed Zucchini with Nutmeg", BowlOfSauteedZucchiniWithNutmeg, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSauteedZucchiniWithNutmeg),
            new Buff("Bowl of Cauliflower Sautee", BowlOfCauliflowerSautee, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfCauliflowerSautee),
            new Buff("Bowl of Eggplant Sautee", BowlOfEggplantSautee, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfEggplantSautee),
            new Buff("Can of Eggplant Saute", CanOfEggplantSaute, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfEggplantSaute),
            new Buff("Bowl of Blueberry Apple Compote", BowlOfBlueberryAppleCompote, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfBlueberryAppleCompote),
            new Buff("Bowl of Strawberry Apple Compote", BowlOfStrawberryAppleCompote, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfStrawberryAppleCompote),
            new Buff("Bowl of Cherry Vanilla Compote", BowlOfCherryVanillaCompote, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfCherryVanillaCompote),
            new Buff("Bowl of Blackberry Pear Compote", BowlOfBlackberryPearCompote, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfBlackberryPearCompote),
            new Buff("Raspberry Peach Compote", RaspberryPeachCompote, Source.Item, BuffClassification.Nourishment, BuffImages.RaspberryPeachCompote),
            new Buff("Canned Raspberry Peach Compote", CannedRaspberryPeachCompote, Source.Item, BuffClassification.Nourishment, BuffImages.CannedRaspberryPeachCompote),
            new Buff("Bowl of Simple Salad", BowlOfSimpleSalad, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSimpleSalad),
            new Buff("ExplorersKit", ExplorersKit, Source.Item, BuffClassification.Nourishment, BuffImages.ExplorersKit),
            new Buff("Bowl of Ascalonian Salad", BowlOfAscalonianSalad, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfAscalonianSalad),
            new Buff("Bowl of Coleslaw", BowlOfColeslaw, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfColeslaw),
            new Buff("Bowl of Cabbage and Chickpea Salad", BowlOfCabbageAndChickpeaSalad, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfCabbageAndChickpeaSalad),
            new Buff("Can of Asparagus and Sage Salad", CanOfAsparagusAndSageSalad, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfAsparagusAndSageSalad),
            new Buff("Spicy Meat Kabob", SpicyMeatKabob, Source.Item, BuffClassification.Nourishment, BuffImages.SpicyMeatKabob),
            new Buff("Piece of Undersea Wurm Sushi", PieceOfUnderseaWurmSushi, Source.Item, BuffClassification.Nourishment, BuffImages.PieceOfUnderseaWurmSushi),
            new Buff("Pepper Steak Dinner", PepperSteakDinner, Source.Item, BuffClassification.Nourishment, BuffImages.PepperSteakDinner),
            new Buff("Chef's Tasting Platter", ChefsTastingPlatter, Source.Item, BuffClassification.Nourishment, BuffImages.ChefsTastingPlatter),
            new Buff("Sesame-Roasted Dinner", SesameRoastedDinner, Source.Item, BuffClassification.Nourishment, BuffImages.SesameRoastedDinner),
            new Buff("Plate of Coriander Crusted Meat Dinner", PlateOfCorianderCrustedMeatDinner, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfCorianderCrustedMeatDinner),
            new Buff("Can of Steak and Asparagus Dinner", CanOfSteakAndAsparagusDinner, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfSteakAndAsparagusDinner),
            new Buff("Plate of Steak and Asparagus Dinner", PlateOfSteakAndAsparagusDinner, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfSteakAndAsparagusDinner),
            new Buff("Canned Spicy Stuffed Mushroom", CannedSpicyStuffedMushroom, Source.Item, BuffClassification.Nourishment, BuffImages.CannedSpicyStuffedMushroom),
            new Buff("Marinated Mushroom", MarinatedMushroom, Source.Item, BuffClassification.Nourishment, BuffImages.MarinatedMushroom),
            new Buff("Sage-Stuffed Mushroom", SageStuffedMushroom, Source.Item, BuffClassification.Nourishment, BuffImages.SageStuffedMushroom),
            new Buff("Divinity Stuffed Mushroom", DivinityStuffedMushroom, Source.Item, BuffClassification.Nourishment, BuffImages.DivinityStuffedMushroom),
            new Buff("Eztlitl Stuffed Mushroom", EztlitlStuffedMushroom, Source.Item, BuffClassification.Nourishment, BuffImages.EztlitlStuffedMushroom),
            new Buff("Spicy Stuffed Mushroom", SpicyStuffedMushroom, Source.Item, BuffClassification.Nourishment, BuffImages.SpicyStuffedMushroom),
            new Buff("Sage-Stuffed Poultry", SageStuffedPoultry, Source.Item, BuffClassification.Nourishment, BuffImages.SageStuffedPoultry),
            new Buff("Poultry Piccata", PoultryPiccata, Source.Item, BuffClassification.Nourishment, BuffImages.PoultryPiccata),
            new Buff("Dilled Poultry Piccata", DilledPoultryPiccata, Source.Item, BuffClassification.Nourishment, BuffImages.DilledPoultryPiccata),
            new Buff("Plate of Citrus Poultry with Almonds", PlaceOfCitrusPoultryWithAlmonds, Source.Item, BuffClassification.Nourishment, BuffImages.PlaceOfCitrusPoultryWithAlmonds),
            new Buff("Canned Tarragon Stuffed Poultry", CannedTarragonStuffedPoultry, Source.Item, BuffClassification.Nourishment, BuffImages.CannedTarragonStuffedPoultry),
            new Buff("Bowl of Salsa", BowlOfSalsa, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSalsa),
            new Buff("Bowl of Bean Salad", BowlOfBeanSalad, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfBeanSalad),
            new Buff("Bowl of Avocado Salsa", BowlOfAvocadoSalsa, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfAvocadoSalsa),
            new Buff("Bowl of Chickpea Salad", BowlOfChickpeaSalad, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfChickpeaSalad),
            new Buff("Bowl of Mango Salsa", BowlOfMangoSalsa, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfMangoSalsa),
            new Buff("Can of Mango Salsa", CanOfMangoSalsa, Source.Item, BuffClassification.Nourishment, BuffImages.CanOfMangoSalsa),
            new Buff("Bloodstone Pot Pie", BloodstonePotPie, Source.Item, BuffClassification.Nourishment, BuffImages.BloodstonePotPie),
            new Buff("Poor Nourishment", PoorNourishment, Source.Item, BuffClassification.Nourishment, BuffImages.NourishmentFood),
            new Buff("Nourishment (Guild Banquet Table)", NourishmentGuildBanquetTable, Source.Item, BuffClassification.Nourishment, BuffImages.NourishmentBirthdayBlaster),
        };

        internal static readonly List<Buff> Utilities = new List<Buff>
        {     
            // UTILITIES 
            // 1h versions have the same ID as 30 min versions 
            new Buff("Diminished",Diminished, Source.Item, BuffClassification.Enhancement, BuffImages.Diminished),
            new Buff("Rough Sharpening Stone", RoughSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.RoughSharpeningStone),
            new Buff("Simple Sharpening Stone", SimpleSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.SimpleSharpeningStone),
            new Buff("Standard Sharpening Stone", StandardSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.StandardSharpeningStone),
            new Buff("Quality Sharpening Stone", QualitySharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.QualitySharpeningStone),
            new Buff("Hardened Sharpening Stone", HardenedSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.HardenedSharpeningStone),
            new Buff("Superior Sharpening Stone", SuperiorSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.SuperiorSharpeningStone),
            new Buff("Apprentice Maintenance Oil", ApprenticeMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.ApprenticeMaintenanceOil),
            new Buff("Journeyman Maintenance Oil", JourneymanMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.JourneymanMaintenanceOil),
            new Buff("Standard Maintenance Oil", StandardMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.StandardMaintenanceOil),
            new Buff("Artisan Maintenance Oil", ArtisanMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.ArtisanMaintenanceOil),
            new Buff("Quality Maintenance Oil", QualityMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.QualityMaintenanceOil),
            new Buff("Master Maintenance Oil", MasterMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.MasterMaintenanceOil),
            new Buff("Apprentice Tuning Crystal", ApprenticeTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.ApprenticeTuningCrystal),
            new Buff("Journeyman Tuning Crystal", JourneymanTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.JourneymanTuningCrystal),
            new Buff("Standard Tuning Crystal", StandardTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.StandardTuningCrystal),
            new Buff("Artisan Tuning Crystal", ArtisanTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.ArtisanTuningCrystal),
            new Buff("Quality Tuning Crystal", QualityTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.QualityTuningCrystal),
            new Buff("Master Tuning Crystal", MasterTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.MasterTuningCrystal),
            new Buff("Compact Hardened Sharpening Stone", CompactHardenedSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.CompactHardenedSharpeningStone),
            new Buff("Tin of Fruitcake", TinOfFruitcake, Source.Item, BuffClassification.Enhancement, BuffImages.TinOfFruitcake),
            new Buff("Bountiful Sharpening Stone", BountifulSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.SuperiorSharpeningStone),
            new Buff("Toxic Sharpening Stone", ToxicSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.ToxicSharpeningStone),
            new Buff("Magnanimous Sharpening Stone", MagnanimousSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.MagnanimousSharpeningStone),
            new Buff("Corsair Sharpening Stone", CorsairSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.CorsairSharpeningStone),
            new Buff("Furious Sharpening Stone", FuriousSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.SuperiorSharpeningStone),
            new Buff("Holographic Super Cheese", HolographicSuperCheese, Source.Item, BuffClassification.Enhancement, BuffImages.HolographicSuperCheese),
            new Buff("Compact Quality Maintenance Oil", CompactQualityMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.CompactQualityMaintenanceOil),
            new Buff("Peppermint Oil", PeppermintOil, Source.Item, BuffClassification.Enhancement, BuffImages.PeppermintOil),
            new Buff("Toxic Maintenance Oil", ToxicMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.ToxicMaintenanceOil),
            new Buff("Magnanimous Maintenance Oil", MagnanimousMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.MagnanimousMaintenanceOil),
            new Buff("Enhanced Lucent Oil", EnhancedLucentOil, Source.Item, BuffClassification.Enhancement, BuffImages.EnhancedLucentOil),
            new Buff("Potent Lucent Oil", PotentLucentOil, Source.Item, BuffClassification.Enhancement, BuffImages.PotentLucentOil),
            new Buff("Corsair Maintenance Oil", CorsairMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.MagnanimousMaintenanceOil),
            new Buff("Furious Maintenance Oil", FuriousMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.MasterMaintenanceOil),
            new Buff("Holographic Super Drumstick", HolographicSuperDrumstick, Source.Item, BuffClassification.Enhancement, BuffImages.HolographicSuperDrumstick),
            new Buff("Bountiful Maintenance Oil", BountifulMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.MasterMaintenanceOil),
            new Buff("Compact Quality Tuning Crystal", CompactQualityTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.CompactQualityTuningCrystal),
            new Buff("Tuning Icicle", TuningIcicle, Source.Item, BuffClassification.Enhancement, BuffImages.TuningIcicle),
            new Buff("Bountiful Tuning Crystal", BountifulTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.MasterTuningCrystal),
            new Buff("Toxic Focusing Crystal", ToxicFocusingCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.ToxicFocusingCrystal),
            new Buff("Magnanimous Tuning Crystal", MagnanimousTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.MagnanimousTuningCrystal),
            new Buff("Furious Tuning Crystal", FuriousTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.MasterTuningCrystal),
            new Buff("Corsair Tuning Crystal", CorsairTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.CorsairTuningCrystal),
            new Buff("Holographic Super Apple", HolographicSuperApple, Source.Item, BuffClassification.Enhancement, BuffImages.HolographicSuperApple),
            new Buff("Sharpening Skull", SharpeningSkull, Source.Item, BuffClassification.Enhancement, BuffImages.SharpeningSkull),
            new Buff("Flask of Pumpkin Oil", FlaskOfPumpkinOil, Source.Item, BuffClassification.Enhancement, BuffImages.FlaskOfPumpkinOil),
            new Buff("Lump of Crystallized Nougat", LumpOfCrystallizedNougat, Source.Item, BuffClassification.Enhancement, BuffImages.LumpOfCrystallizedNougat),
            new Buff("Decade Enhancement", DecadeEnhancement, Source.Item, BuffClassification.Enhancement, BuffImages.DecadeEnhancement),
        };

        internal static readonly List<Buff> OtherConsumables = new List<Buff>
        {       
            //
            new Buff("Reinforced Armor", ReinforcedArmor, Source.Item, BuffClassification.OtherConsumable, BuffImages.ReinforcedArmor, GW2Builds.June2022Balance, GW2Builds.EndOfLife),
            //
            new Buff("15% Speed Bonus", SpeedBonus15, Source.Item, BuffStackType.Queue, 9, BuffClassification.OtherConsumable, BuffImages.SpeedBonus15),
            new Buff("5% Damage Reduction", DamageReduction5, Source.Item, BuffStackType.Queue, 9, BuffClassification.OtherConsumable, BuffImages.DamageReduction5),
            new Buff("Healthful Rejuvenation", HealthfulRejuvenation, Source.Item, BuffStackType.Queue, 9, BuffClassification.OtherConsumable, BuffImages.HealthfulRejuvenation),
            new Buff("5% Damage Bonus", DamageBonus5, Source.Item, BuffStackType.Queue, 9, BuffClassification.OtherConsumable, BuffImages.DamageBonus5),
            //
            new Buff("Skale Venom (Consumable)", SkaleVenomConsumable, Source.Item, BuffClassification.OtherConsumable, BuffImages.SkaleVenomConsumable),
            new Buff("Swift Moa Feather", SwiftMoaFeather, Source.Item, BuffClassification.OtherConsumable, BuffImages.SwiftMoaFeather),
        };

        internal static readonly List<Buff> Writs = new List<Buff>
        {
            new Buff("Writ of Basic Strength", WritOfBasicStrength, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfBasicStrength),
            new Buff("Writ of Strength", WritOfStrength, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfStrength),
            new Buff("Writ of Studied Strength", WritOfStudiedStrength, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfStudiedStrength),
            new Buff("Writ of Calculated Strength", WritOfCalculatedStrength, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfCalculatedStrength),
            new Buff("Writ of Learned Strength", WritOfLearnedStrength, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfLearnedStrength),
            new Buff("Writ of Masterful Strength", WritOfMasterfulStrength, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfMasterfulStrength),
            new Buff("Writ of Basic Accuracy", WritOfBasicAccuracy, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfBasicAccuracy),
            new Buff("Writ of Accuracy", WritOfAccuracy, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfAccuracy),
            new Buff("Writ of Studied Accuracy", WritOfStudiedAccuracy, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfStudiedAccuracy),
            new Buff("Writ of Calculated Accuracy", WritOfCalculatedAccuracy, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfCalculatedAccuracy),
            new Buff("Writ of Learned Accuracy", WritOfLearnedAccuracy, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfLearnedAccuracy),
            new Buff("Writ of Masterful Accuracy", WritOfMasterfulAccuracy, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfMasterfulAccuracy),
            new Buff("Writ of Basic Malice", WritOfBasicMalice, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfBasicMalice),
            new Buff("Writ of Malice", WritOfMalice, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfMalice),
            new Buff("Writ of Studied Malice", WritOfStudiedMalice, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfStudiedMalice),
            new Buff("Writ of Calculated Malice", WritOfCalculatedMalice, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfCalculatedMalice),
            new Buff("Writ of Learned Malice", WritOfLearnedMalice, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfLearnedMalice),
            new Buff("Writ of Masterful Malice", WritOfMasterfulMalice, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfMasterfulMalice),
            new Buff("Writ of Basic Speed", WritOfBasicSpeed, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfBasicSpeed),
            new Buff("Writ of Studied Speed", WritOfStudiedSpeed, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfStudiedSpeed),
            new Buff("Writ of Masterful Speed", WritOfMasterfulSpeed, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfMasterfulSpeed),
        };

        internal static readonly List<Buff> Potions = new List<Buff>
        {
            // Slaying Potions
            // Branded
            new Buff("Minor Potion of Branded Slaying", MinorPotionOfBrandedSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfBrandedSlaying),
            new Buff("Powerful Potion of Branded Slaying", PowerfulPotionOfBrandedSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfBrandedSlaying),
            new Buff("Dragon Crystal Potion", DragonCrystalPotion, Source.Item, BuffClassification.OtherConsumable, BuffImages.DragonCrystalPotion),
            // Flame Legion
            new Buff("Weak Potion of Flame Legion Slaying", WeakPotionOfFlameLegionSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfFlameLegionSlaying),
            new Buff("Minor Potion of Flame Legion Slaying", MinorPotionOfFlameLegionSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfFlameLegionSlaying),
            new Buff("Potion of Flame Legion Slaying", PotionOfFlameLegionSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfFlameLegionSlaying),
            new Buff("Strong Potion of Flame Legion Slaying", StrongPotionOfFlameLegionSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfFlameLegionSlaying),
            new Buff("Potent Potion of Flame Legion Slaying", PotentPotionOfFlameLegionSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfFlameLegionSlaying),
            new Buff("Powerful Potion of Flame Legion Slaying", PowerfulPotionOfFlameLegionSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfFlameLegionSlaying),
            // Halloween
            new Buff("Weak Potion of Halloween Slaying", WeakPotionOfHalloweenSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfHalloweenSlaying),
            new Buff("Minor Potion of Halloween Slaying", MinorPotionOfHalloweenSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfGrawlSlaying),
            new Buff("Potion of Halloween Slaying", PotionOfHalloweenSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfOgreSlaying),
            new Buff("Strong Potion of Halloween Slaying", StrongPotionOfHalloweenSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfOgreSlaying),
            new Buff("Potent Potion of Halloween Slaying", PotentPotionOfHalloweenSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfOgreSlaying),
            new Buff("Powerful Potion of Halloween Slaying", PowerfulPotionOfHalloweenSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfHalloweenSlaying),
            // Centaur
            new Buff("Weak Potion of Centaur Slaying", WeakPotionOfCentaurSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfCentaurSlaying),
            new Buff("Minor Potion of Centaur Slaying", MinorPotionOfCentaurSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfCentaurSlaying),
            new Buff("Potion of Centaur Slaying", PotionOfCentaurSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfCentaurSlaying),
            new Buff("Strong Potion of Centaur Slaying", StrongPotionOfCentaurSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfCentaurSlaying),
            new Buff("Potent Potion of Centaur Slaying", PotentPotionOfCentaurSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfCentaurSlaying),
            new Buff("Powerful Potion of Centaur Slaying", PowerfulPotionOfCentaurSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfCentaurSlaying),
            // Krait
            new Buff("Weak Potion of Krait Slaying", WeakPotionOfKraitSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfKraitSlaying),
            new Buff("Minor Potion of Krait Slaying", MinorPotionOfKraitSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfKraitSlaying),
            new Buff("Potion of Krait Slaying", PotionOfKraitSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfKraitSlaying),
            new Buff("Strong Potion of Krait Slaying", StrongPotionOfKraitSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfKraitSlaying),
            new Buff("Potent Potion of Krait Slaying", PotentPotionOfKraitSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfKraitSlaying),
            new Buff("Powerful Potion of Krait Slaying", PowerfulPotionOfKraitSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfKraitSlaying),
            // Ogre
            new Buff("Weak Potion of Ogre Slaying", WeakPotionOfOgreSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfHalloweenSlaying),
            new Buff("Minor Potion of Ogre Slaying", MinorPotionOfOgreSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfOgreSlaying),
            new Buff("Potion of Ogre Slaying", PotionOfOgreSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfOgreSlaying),
            new Buff("Strong Potion of Ogre Slaying", StrongPotionOfOgreSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfOgreSlaying),
            new Buff("Potent Potion of Ogre Slaying", PotentPotionOfOgreSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfOgreSlaying),
            new Buff("Powerful Potion of Ogre Slaying", PowerfulPotionOfOgreSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfOgreSlaying),
            // Elemental
            new Buff("Weak Potion of Elemental Slaying", WeakPotionOfElementalSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfElementalSlaying),
            new Buff("Minor Potion of Elemental Slaying", MinorPotionOfElementalSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfElementalSlaying),
            new Buff("Potion of Elemental Slaying", PotionOfElementalSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfElementalSlaying),
            new Buff("Strong Potion of Elemental Slaying", StrongPotionOfElementalSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfElementalSlaying),
            new Buff("Potent Potion of Elemental Slaying", PotentPotionOfElementalSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfElementalSlaying),
            new Buff("Powerful Potion of Elemental Slaying", PowerfulPotionOfElementalSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfElementalSlaying),
            // Destroyer
            new Buff("Weak Potion of Destroyer Slaying", WeakPotionOfDestroyerSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfDestroyerSlaying),
            new Buff("Minor Potion of Destroyer Slaying", MinorPotionOfDestroyerSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfDestroyerSlaying),
            new Buff("Potion of Destroyer Slaying", PotionOfDestroyerSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfDestroyerSlaying),
            new Buff("Strong Potion of Destroyer Slaying", StrongPotionOfDestroyerSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfDestroyerSlaying),
            new Buff("Potent Potion of Destroyer Slaying", PotentPotionOfDestroyerSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfDestroyerSlaying),
            new Buff("Powerful Potion of Destroyer Slaying", PowerfulPotionOfDestroyerSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfDestroyerSlaying),
            // Nightmare Court
            new Buff("Weak Potion of Nightmare Court Slaying", WeakPotionOfNightmareCourtSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfNightmareCourtSlaying),
            new Buff("Minor Potion of Nightmare Court Slaying", MinorPotionOfNightmareCourtSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfNightmareCourtSlaying),
            new Buff("Potion of Nightmare Court Slaying", PotionOfNightmareCourtSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfNightmareCourtSlaying),
            new Buff("Strong Potion of Nightmare Court Slaying", StrongPotionOfNightmareCourtSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfNightmareCourtSlaying),
            new Buff("Potent Potion of Nightmare Court Slaying", PotentPotionOfNightmareCourtSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfNightmareCourtSlaying),
            new Buff("Powerful Potion of Nightmare Court Slaying", PowerfulPotionOfNightmareCourtSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfNightmareCourtSlaying),
            // Scarlet's Armies
            new Buff("Minor Potion of Slaying Scarlet's Armies", MinorPotionOfSlayingScarletsArmies, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfSlayingScarletsArmies),
            new Buff("Powerful Potion of Slaying Scarlet's Armies", PowerfulPotionOfSlayingScarletsArmies, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfDemonSlaying),
            // Undead
            new Buff("Weak Potion of Undead Slaying", WeakPotionOfUndeadSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.ThimbleOfLiquidKarma),
            new Buff("Minor Potion of Undead Slaying", MinorPotionOfUndeadSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.TasteOfLiquidKarma),
            new Buff("Potion of Undead Slaying", PotionOfUndeadSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.SwigOfLiquidKarma),
            new Buff("Strong Potion of Undead Slaying", StrongPotionOfUndeadSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.FlaskOfLiquidKarma),
            new Buff("Potent Potion of Undead Slaying", PotentPotionOfUndeadSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfUndeadSlaying),
            new Buff("Powerful Potion of Undead Slaying", PowerfulPotionOfUndeadSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfUndeadSlaying),
            // Dredge
            new Buff("Weak Potion of Dredge Slaying", WeakPotionOfDredgeSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfDredgeSlaying),
            new Buff("Minor Potion of Dredge Slaying", MinorPotionOfDredgeSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfDredgeSlaying),
            new Buff("Potion of Dredge Slaying", PotionOfDredgeSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfDredgeSlaying),
            new Buff("Strong Potion of Dredge Slaying", StrongPotionOfDredgeSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfDredgeSlaying),
            new Buff("Potent Potion of Dredge Slaying", PotentPotionOfDredgeSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfDredgeSlaying),
            new Buff("Powerful Potion of Dredge Slaying", PowerfulPotionOfDredgeSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfDredgeSlaying),
            // Inquest
            new Buff("Weak Potion of Inquest Slaying", WeakPotionOfInquestSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.DripOfLiquidKarma),
            new Buff("Minor Potion of Inquest Slaying", MinorPotionOfInquestSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.SipOfLiquidKarma),
            new Buff("Potion of Inquest Slaying", PotionOfInquestSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.VialOfLiquidKarma),
            new Buff("Strong Potion of Inquest Slaying", StrongPotionOfInquestSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.GulpOfLiquidKarma),
            new Buff("Potent Potion of Inquest Slaying", PotentPotionOfInquestSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.JugOfLiquidKarma),
            new Buff("Powerful Potion of Inquest Slaying", PowerfulPotionOfInquestSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfInquestSlaying),
            // Demon
            new Buff("Weak Potion of Demon Slaying", WeakPotionOfDemonSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfDemonSlaying),
            new Buff("Minor Potion of Demon Slaying", MinorPotionOfDemonSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfDemonSlaying),
            new Buff("Potion of Demon Slaying", PotionOfDemonSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfDemonSlaying),
            new Buff("Strong Potion of Demon Slaying", StrongPotionOfDemonSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfDemonSlaying),
            new Buff("Potent Potion of Demon Slaying", PotentPotionOfDemonSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfDemonSlaying),
            new Buff("Powerful Potion of Demon Slaying", PowerfulPotionOfDemonSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfDemonSlaying),
            // Grawl
            new Buff("Weak Potion of Grawl Slaying", WeakPotionOfGrawlSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfGrawlSlaying),
            new Buff("Minor Potion of Grawl Slaying", MinorPotionOfGrawlSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfGrawlSlaying),
            new Buff("Potion of Grawl Slaying", PotionOfGrawlSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfGrawlSlaying),
            new Buff("Strong Potion of Grawl Slaying", StrongPotionOfGrawlSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfGrawlSlaying),
            new Buff("Potent Potion of Grawl Slaying", PotentPotionOfGrawlSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfGrawlSlaying),
            new Buff("Powerful Potion of Grawl Slaying", PowerfulPotionOfGrawlSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfGrawlSlaying),
            // Sons of Svanir
            new Buff("Weak Potion of Sons of Svanir Slaying", WeakPotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MysteryTonicFurniture),
            new Buff("Minor Potion of Sons of Svanir Slaying", MinorPotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.Tonic),
            new Buff("Potion of Sons of Svanir Slaying", PotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfSonsOfSvanirSlaying),
            new Buff("Strong Potion of Sons of Svanir Slaying", StrongPotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfSonsOfSvanirSlaying),
            new Buff("Potent Potion of Sons of Svanir Slaying", PotentPotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfSonsOfSvanirSlaying),
            new Buff("Powerful Potion of Sons of Svanir Slaying", PowerfulPotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfSonsOfSvanirSlaying),
            // Outlaw
            new Buff("Weak Potion of Outlaw Slaying", WeakPotionOfOutlawSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfOutlawSlaying),
            new Buff("Minor Potion of Outlaw Slaying", MinorPotionOfOutlawSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfOutlawSlaying),
            new Buff("Potion of Outlaw Slaying", PotionOfOutlawSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfOutlawSlaying),
            new Buff("Strong Potion of Outlaw Slaying", StrongPotionOfOutlawSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfOutlawSlaying),
            new Buff("Potent Potion of Outlaw Slaying", PotentPotionOfOutlawSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfOutlawSlaying),
            new Buff("Powerful Potion of Outlaw Slaying", PowerfulPotionOfOutlawSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfOutlawSlaying),
            // Ice Brood
            new Buff("Weak Potion of Ice Brood Slaying", WeakPotionOfIceBroodSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfIceBroodSlaying),
            new Buff("Minor Potion of Ice Brood Slaying", MinorPotionOfIceBroodSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfIceBroodSlaying),
            new Buff("Potion of Ice Brood Slaying", PotionOfIceBroodSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfIceBroodSlaying),
            new Buff("Strong Potion of Ice Brood Slaying", StrongPotionOfIceBroodSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfIceBroodSlaying),
            new Buff("Potent Potion of Ice Brood Slaying", PotentPotionOfIceBroodSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfIceBroodSlaying),
            new Buff("Powerful Potion of Ice Brood Slaying", PowerfulPotionOfIceBroodSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfIceBroodSlaying),
            // Ghost
            new Buff("Extended Potion of Ghost Slaying", ExtendedPotionOfGhostSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.GlassOfButtermilk),
            // Hologram
            new Buff("Minor Potion of Hologram Slaying", MinorPotionOfHologramSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfHologramSlaying),
            // Karka
            new Buff("Potion Of Karka Slaying", PotionOfKarkaSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfInquestSlaying),
            // Mordrem
            new Buff("Minor Potion of Mordrem Slaying", MinorPotionOfMordremSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfMordremSlaying),
            new Buff("Powerful Potion of Mordrem Slaying", PowerfulPotionOfMordremSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfMordremSlaying),
            // Fractals
            new Buff("Fractal Mobility", FractalMobility, Source.Item, BuffStackType.Stacking, 5, BuffClassification.OtherConsumable, BuffImages.FractalMobility),
            new Buff("Fractal Defensive", FractalDefensive, Source.Item, BuffStackType.Stacking, 5, BuffClassification.OtherConsumable, BuffImages.FractalDefensive),
            new Buff("Fractal Offensive", FractalOffensive, Source.Item, BuffStackType.Stacking, 5, BuffClassification.OtherConsumable, BuffImages.FractalOffensive),
            // Misc
            new Buff("Potion Of Karka Toughness", PotionOfKarkaToughness, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfInquestSlaying),
        };

        internal static readonly List<Buff> AscendedFood = new List<Buff>
        {
            // Ascended Food
            new Buff("Bowl of Fruit Salad with Cilantro Garnish", BowlOfFruitSaladWithCilantroGarnish, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfFruitSaladWithCilantroGarnish),
            new Buff("Bowl of Fruit Salad with Mint Garnish", BowlOfFruitSaladWithMintGarnish, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfFruitSaladWithMintGarnish),
            new Buff("Bowl of Fruit Salad with Orange-Clove Syrup", BowlOfFruitSaladWithOrangeCloveSyrup, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfFruitSaladWithOrangeCloveSyrup),
            new Buff("Bowl of Sesame Fruit Salad", BowlOfSesameFruitSalad, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSesameFruitSalad),
            new Buff("Bowl of Spiced Fruit Salad", BowlOfSpicedFruitSalad, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfSpicedFruitSalad),
            new Buff("Cilantro Lime Sous-Vide Steak", CilantroLimeSousVideSteak, Source.Item, BuffClassification.Nourishment, BuffImages.CilantroLimeSousVideSteak),
            new Buff("Cilantro and Cured Meat Flatbread", CilantroAndCuredMeatFlatbread, Source.Item, BuffClassification.Nourishment, BuffImages.CilantroAndCuredMeatFlatbread),
            new Buff("Clove and Veggie Flatbread", CloveAndVeggieFlatbread, Source.Item, BuffClassification.Nourishment, BuffImages.CloveAndVeggieFlatbread),
            new Buff("Clove-Spiced Creme Brulee", CloveSpicedCremeBrulee, Source.Item, BuffClassification.Nourishment, BuffImages.CloveSpicedCremeBrulee),
            new Buff("Clove-Spiced Eggs Benedict", CloveSpicedEggsBenedict, Source.Item, BuffClassification.Nourishment, BuffImages.CloveSpicedEggsBenedict),
            new Buff("Clove-Spiced Pear and Cured Meat Flatbread", CloveSpicedPearAndCuredMeatFlatbread, Source.Item, BuffClassification.Nourishment, BuffImages.CloveSpicedPearAndCuredMeatFlatbread),
            new Buff("Eggs Benedict with Mint-Parsley Sauce", EggsBenedictWithMintParsleySauce, Source.Item, BuffClassification.Nourishment, BuffImages.EggsBenedictWithMintParsleySauce),
            new Buff("Mango Cilantro Creme Brulee", MangoCilantroCremeBrulee, Source.Item, BuffClassification.Nourishment, BuffImages.MangoCilantroCremeBrulee),
            new Buff("Mint Creme Brulee", MintCremeBrulee, Source.Item, BuffClassification.Nourishment, BuffImages.MintCremeBrulee),
            new Buff("Mint Strawberry Cheesecake", MintStrawberryCheesecake, Source.Item, BuffClassification.Nourishment, BuffImages.MintStrawberryCheesecake),
            new Buff("Mint and Veggie Flatbread", MintAndVeggieFlatbread, Source.Item, BuffClassification.Nourishment, BuffImages.MintAndVeggieFlatbread),
            new Buff("Mint-Pear Cured Meat Flatbread", MintPearCuredMeatFlatbread, Source.Item, BuffClassification.Nourishment, BuffImages.MintPearCuredMeatFlatbread),
            new Buff("Mushroom Clove Sous-Vide Steak", MushroomCloveSousVideSteak, Source.Item, BuffClassification.Nourishment, BuffImages.MushroomCloveSousVideSteak),
            new Buff("Orange Clove Cheesecake", OrangeCloveCheesecake, Source.Item, BuffClassification.Nourishment, BuffImages.OrangeCloveCheesecake),
            new Buff("Peppercorn and Veggie Flatbread", PeppercornAndVeggieFlatbread, Source.Item, BuffClassification.Nourishment, BuffImages.PeppercornAndVeggieFlatbread),
            new Buff("Peppercorn-Crusted Sous-Vide Steak", PeppercornCrustedSousVideSteak, Source.Item, BuffClassification.Nourishment, BuffImages.PeppercornCrustedSousVideSteak),
            new Buff("Peppercorn-Spiced Eggs Benedict", PeppercornSpicedEggsBenedict, Source.Item, BuffClassification.Nourishment, BuffImages.PeppercornSpicedEggsBenedict),
            new Buff("Peppered Cured Meat Flatbread", PepperedCuredMeatFlatbread, Source.Item, BuffClassification.Nourishment, BuffImages.PepperedCuredMeatFlatbread),
            new Buff("Plate of Beef Carpaccio with Salsa Garnish", PlateOfBeefCarpaccioWithSalsaGarnish, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfBeefCarpaccioWithSalsaGarnish),
            new Buff("Plate of Beef Carpaccio with Mint Garnish", PlateOfBeefCarpaccioWithMintGarnish, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfBeefCarpaccioWithMintGarnish),
            new Buff("Plate of Clear Truffle and Cilantro Ravioli", PlateOfClearTruffleAndCilantroRavioli, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfClearTruffleAndCilantroRavioli),
            new Buff("Plate of Clear Truffle and Mint Ravioli", PlateOfClearTruffleAndMintRavioli, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfClearTruffleAndMintRavioli),
            new Buff("Plate of Clear Truffle and Sesame Ravioli", PlateOfClearTruffleAndSesameRavioli, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfClearTruffleAndSesameRavioli),
            new Buff("Plate of Clove-Spiced Beef Carpaccio", PlateOfCloveSpicedBeefCarpaccio, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfCloveSpicedBeefCarpaccio),
            new Buff("Plate of Clove-Spiced Clear Truffle Ravioli", PlateOfCloveSpicedClearTruffleRavioli, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfCloveSpicedClearTruffleRavioli),
            new Buff("Plate of Clove-Spiced Coq Au Vin", PlateOfCloveSpicedCoqAuVin, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfCloveSpicedCoqAuVin),
            new Buff("Plate of Clove-Spiced Poultry Aspic", PlateOfCloveSpicedPoultryAspic, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfCloveSpicedPoultryAspic),
            new Buff("Plate of Coq Au Vin with Mint Garnish", PlateOfCoqAuVinWithMintGarnish, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfCoqAuVinWithMintGarnish),
            new Buff("Plate of Coq Au Vin with Salsa", PlateOfCoqAuVinWithSalsa, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfCoqAuVinWithSalsa),
            new Buff("Plate of Peppercorn-Spiced Beef Carpaccio", PlateOfPeppercornSpicedBeefCarpaccio, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfPeppercornSpicedBeefCarpaccio),
            new Buff("Plate of Peppercorn-Spiced Coq Au Vin", PlateOfPeppercornSpicedCoqAuVin, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfPeppercornSpicedCoqAuVin),
            new Buff("Plate of Peppercorn-Spiced Poultry Aspic", PlateOfPeppercornSpicedPoultryAspic, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfPeppercornSpicedPoultryAspic),
            new Buff("Plate of Peppered Clear Truffle Ravioli", PlateOfPepperedClearTruffleRavioli, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfPepperedClearTruffleRavioli),
            new Buff("Plate of Poultry Aspic with Mint Garnish", PlateOfPoultryAspicWithMintGarnish, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfPoultryAspicWithMintGarnish),
            new Buff("Plate of Poultry Aspic with Salsa Garnish", PlateOfPoultryAspicWithSalsaGarnish, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfPoultryAspicWithSalsaGarnish),
            new Buff("Plate of Sesame Poultry Aspic", PlateOfSesamePoultryAspic, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfSesamePoultryAspic),
            new Buff("Plate of Sesame-Crusted Coq Au Vin", PlateOfSesameCrustedCoqAuVin, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfSesameCrustedCoqAuVin),
            new Buff("Plate of Sesame-Ginger Beef Carpaccio", PlateOfSesameGingerBeefCarpaccio, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfSesameGingerBeefCarpaccio),
            new Buff("Salsa Eggs Benedict", SalsaEggsBenedict, Source.Item, BuffClassification.Nourishment, BuffImages.SalsaEggsBenedict),
            new Buff("Salsa-Topped Veggie Flatbread", SalsaToppedVeggieFlatbread, Source.Item, BuffClassification.Nourishment, BuffImages.SalsaToppedVeggieFlatbread),
            new Buff("Sesame Cheesecake", SesameCheesecake, Source.Item, BuffClassification.Nourishment, BuffImages.SesameCheesecake),
            new Buff("Sesame Creme Brulee", SesameCremeBrulee, Source.Item, BuffClassification.Nourishment, BuffImages.SesameCremeBrulee),
            new Buff("Sesame Eggs Benedict", SesameEggsBenedict, Source.Item, BuffClassification.Nourishment, BuffImages.SesameEggsBenedict),
            new Buff("Sesame Veggie Flatbread", SesameVeggieFlatbread, Source.Item, BuffClassification.Nourishment, BuffImages.SesameVeggieFlatbread),
            new Buff("Sesame-Asparagus and Cured Meat Flatbread", SesameAsparagusAndCuredMeatFlatbread, Source.Item, BuffClassification.Nourishment, BuffImages.SesameAsparagusAndCuredMeatFlatbread),
            new Buff("Sous-Vide Steak with Mint-Parsley Sauce", SousVideSteakWithMintParsleySauce, Source.Item, BuffClassification.Nourishment, BuffImages.SousVideSteakWithMintParsleySauce),
            new Buff("Soy-Sesame Sous-Vide Steak", SoySesameSousVideSteak, Source.Item, BuffClassification.Nourishment, BuffImages.SoySesameSousVideSteak),
            new Buff("Spherified Cilantro Oyster Soup", SpherifiedCilantroOysterSoup, Source.Item, BuffClassification.Nourishment, BuffImages.SpherifiedCilantroOysterSoup),
            new Buff("Spherified Clove-Spiced Oyster Soup", SpherifiedCloveSpicedOysterSoup, Source.Item, BuffClassification.Nourishment, BuffImages.SpherifiedCloveSpicedOysterSoup),
            new Buff("Spherified Oyster Soup with Mint Garnish", SpherifiedOysterSoupWithMintGarnish, Source.Item, BuffClassification.Nourishment, BuffImages.SpherifiedOysterSoupWithMintGarnish),
            new Buff("Spherified Peppercorn-Spiced Oyster Soup", SpherifiedPeppercornSpicedOysterSoup, Source.Item, BuffClassification.Nourishment, BuffImages.SpherifiedPeppercornSpicedOysterSoup),
            new Buff("Spherified Sesame Oyster Soup", SpherifiedSesameOysterSoup, Source.Item, BuffClassification.Nourishment, BuffImages.SpherifiedSesameOysterSoup),
            new Buff("Spiced Pepper Creme Brulee", SpicedPepperCremeBrulee, Source.Item, BuffClassification.Nourishment, BuffImages.SpicedPepperCremeBrulee),
            new Buff("Spiced Peppercorn Cheesecake", SpicedPeppercornCheesecake, Source.Item, BuffClassification.Nourishment, BuffImages.SpicedPeppercornCheesecake),
            new Buff("Strawberry Cilantro Cheesecake", StrawberryCilantroCheesecake, Source.Item, BuffClassification.Nourishment, BuffImages.StrawberryCilantroCheesecake),
            new Buff("Plate of Imperial Palace Special", PlateOfImperialPalaceSpecial, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfImperialPalaceSpecial),
            new Buff("Plate of Crispy Fish Pancakes", PlateOfCrispyFishPancakes, Source.Item, BuffClassification.Nourishment, BuffImages.PlateOfCrispyFishPancakes),
            new Buff("Bowl of Jade Sea Bounty", BowlOfJadeSeaBounty, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfJadeSeaBounty),
            new Buff("Bowl of Echovald Hotpot", BowlOfEchovaldHotpot, Source.Item, BuffClassification.Nourishment, BuffImages.BowlOfEchovaldHotpot),
            new Buff("Flight Of Sushi", FlightOfSushi, Source.Item, BuffClassification.Nourishment, BuffImages.FlightOfSushi),
        };

        internal static readonly List<Buff> FoodProcs = new List<Buff>
        {
            // Effect procs for On Kill Food
            new Buff("Nourishment (Bonus Power)", NourishmentBonusPower, Source.Item, BuffClassification.Offensive, BuffImages.ChampionOfTheCrown),
            new Buff("Nourishment (Bonus Power & Ferocity)", NourishmentBonusPowerFerocity, Source.Item, BuffClassification.Offensive, BuffImages.ChampionOfTheCrown),
            new Buff("Malice (Bonus Condition Damage)", MaliceBonusConditionDamage, Source.Item, BuffClassification.Offensive, BuffImages.ChampionOfTheCrown),
        };
    }
}
