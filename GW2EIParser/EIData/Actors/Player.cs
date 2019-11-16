using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GW2EIParser.Logic;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;
using static GW2EIParser.EIData.GeneralStatistics;

namespace GW2EIParser.EIData
{
    public class Player : AbstractSingleActor
    {

        public class DamageModifierData
        {
            public int HitCount { get; }
            public int TotalHitCount { get; }
            public double DamageGain { get; }
            public int TotalDamage { get; }

            public DamageModifierData(int hitCount, int totalHitCount, double damageGain, int totalDamage)
            {
                HitCount = hitCount;
                TotalHitCount = totalHitCount;
                DamageGain = damageGain;
                TotalDamage = totalDamage;
            }
        }


        public class Consumable
        {
            public Buff Buff { get; }
            public long Time { get; }
            public int Duration { get; }
            public int Stack { get; set; }

            public Consumable(Buff item, long time, int duration)
            {
                Buff = item;
                Time = time;
                Duration = duration;
                Stack = 1;
            }
        }

        public class DeathRecap
        {
            public class DeathRecapDamageItem
            {
                public long ID { get; set; }
                public bool IndirectDamage { get; set; }
                public string Src { get; set; }
                public int Damage { get; set; }
                public int Time { get; set; }
            }

            public int DeathTime { get; set; }
            public List<DeathRecapDamageItem> ToDown { get; set; }
            public List<DeathRecapDamageItem> ToKill { get; set; }
        }


        // Fields
        public string Account { get; protected set; }
        public int Group { get; }

        private List<Consumable> _consumeList;
        private List<DeathRecap> _deathRecaps;
        private Dictionary<string, List<DamageModifierData>> _damageModifiers;
        private HashSet<string> _presentDamageModifiers;
        private Dictionary<NPC, Dictionary<string, List<DamageModifierData>>> _damageModifiersTargets;
        // statistics
        private List<FinalPlayerSupport> _playerSupport;
        private List<Dictionary<long, FinalPlayerBuffs>> _selfBuffs;
        private List<Dictionary<long, FinalPlayerBuffs>> _groupBuffs;
        private List<Dictionary<long, FinalPlayerBuffs>> _offGroupBuffs;
        private List<Dictionary<long, FinalPlayerBuffs>> _squadBuffs;
        private List<Dictionary<long, FinalPlayerBuffs>> _selfActiveBuffs;
        private List<Dictionary<long, FinalPlayerBuffs>> _groupActiveBuffs;
        private List<Dictionary<long, FinalPlayerBuffs>> _offGroupActiveBuffs;
        private List<Dictionary<long, FinalPlayerBuffs>> _squadActiveBuffs;
        //weaponslist
        private string[] _weaponsArray;

        // Constructors
        public Player(AgentItem agent, bool noSquad) : base(agent)
        {
            string[] name = agent.Name.Split('\0');
            if (name.Length < 2)
            {
                throw new InvalidOperationException("Name problem on Player");
            }
            if (name[1].Length == 0 || name[2].Length == 0 || Character.Contains("-"))
            {
                throw new InvalidOperationException("Missing Group on Player");
            }
            Account = name[1].TrimStart(':');
            Group = noSquad ? 1 : int.Parse(name[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
            IsFakeActor = Account == "Conjured Sword";
        }

        // Public methods

        public FinalPlayerSupport GetPlayerSupport(ParsedLog log, int phaseIndex)
        {
            return GetPlayerSupport(log)[phaseIndex];
        }

        public List<FinalPlayerSupport> GetPlayerSupport(ParsedLog log)
        {
            if (_playerSupport == null)
            {
                _playerSupport = new List<FinalPlayerSupport>();
                List<PhaseData> phases = log.FightData.GetPhases(log);
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {
                    var playerSup = new FinalPlayerSupport();
                    _playerSupport.Add(playerSup);
                    FinalSupportAll totals = GetSupport(log, phaseIndex);
                    playerSup.Resurrects = totals.Resurrects;
                    playerSup.ResurrectTime = totals.ResurrectTime;
                    FinalSupport self = GetSupport(log, this, phaseIndex);
                    foreach (Buff buff in log.Buffs.BuffsByNature[BuffNature.Boon])
                    {
                        // add everything from total
                        if (totals.Removals.TryGetValue(buff.ID, out (long count, double time) item))
                        {
                            playerSup.BoonStrips += item.count;
                            playerSup.BoonStripsTime += item.time;
                        }
                        // remove everything from self
                        if (self.Removals.TryGetValue(buff.ID, out item))
                        {
                            playerSup.BoonStrips -= item.count;
                            playerSup.BoonStripsTime -= item.time;
                        }
                    }
                    foreach (Buff buff in log.Buffs.BuffsByNature[BuffNature.Boon])
                    {
                        // add everything from self
                        if (self.Removals.TryGetValue(buff.ID, out (long count, double time) item))
                        {
                            playerSup.CondiCleanseSelf += item.count;
                            playerSup.CondiCleanseTimeSelf += item.time;
                        }
                        foreach (Player p in log.PlayerList)
                        {
                            if (p == this)
                            {
                                continue;
                            }
                            FinalSupport other = GetSupport(log, p, phaseIndex);
                            // Add everything from other
                            if (other.Removals.TryGetValue(buff.ID, out item))
                            {
                                playerSup.CondiCleanse += item.count;
                                playerSup.CondiCleanseTime += item.time;
                            }
                        }
                    }
                }
            }
            return _playerSupport;
        }

        public Dictionary<long, FinalPlayerBuffs> GetBuffs(ParsedLog log, int phaseIndex, BuffEnum type)
        {
            if (_selfBuffs == null)
            {
                SetBuffs(log);
            }
            switch (type)
            {
                case BuffEnum.Group:
                    return _groupBuffs[phaseIndex];
                case BuffEnum.OffGroup:
                    return _offGroupBuffs[phaseIndex];
                case BuffEnum.Squad:
                    return _squadBuffs[phaseIndex];
                case BuffEnum.Self:
                default:
                    return _selfBuffs[phaseIndex];
            }
        }

        public List<Dictionary<long, FinalPlayerBuffs>> GetBuffs(ParsedLog log, BuffEnum type)
        {
            if (_selfBuffs == null)
            {
                SetBuffs(log);
            }
            switch (type)
            {
                case BuffEnum.Group:
                    return _groupBuffs;
                case BuffEnum.OffGroup:
                    return _offGroupBuffs;
                case BuffEnum.Squad:
                    return _squadBuffs;
                case BuffEnum.Self:
                default:
                    return _selfBuffs;
            }
        }

        public Dictionary<long, FinalPlayerBuffs> GetActiveBuffs(ParsedLog log, int phaseIndex, BuffEnum type)
        {
            if (_selfActiveBuffs == null)
            {
                SetBuffs(log);
            }
            switch (type)
            {
                case BuffEnum.Group:
                    return _groupActiveBuffs[phaseIndex];
                case BuffEnum.OffGroup:
                    return _offGroupActiveBuffs[phaseIndex];
                case BuffEnum.Squad:
                    return _squadActiveBuffs[phaseIndex];
                case BuffEnum.Self:
                default:
                    return _selfActiveBuffs[phaseIndex];
            }
        }

        public List<Dictionary<long, FinalPlayerBuffs>> GetActiveBuffs(ParsedLog log, BuffEnum type)
        {
            if (_selfActiveBuffs == null)
            {
                SetBuffs(log);
            }
            switch (type)
            {
                case BuffEnum.Group:
                    return _groupActiveBuffs;
                case BuffEnum.OffGroup:
                    return _offGroupActiveBuffs;
                case BuffEnum.Squad:
                    return _squadActiveBuffs;
                case BuffEnum.Self:
                default:
                    return _selfActiveBuffs;
            }
        }

        private (List<Dictionary<long, FinalPlayerBuffs>>, List<Dictionary<long, FinalPlayerBuffs>>) GetBoonsForPlayers(List<Player> playerList, ParsedLog log)
        {
            var uptimesByPhase = new List<Dictionary<long, FinalPlayerBuffs>>();
            var uptimesActiveByPhase = new List<Dictionary<long, FinalPlayerBuffs>>();

            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                PhaseData phase = phases[phaseIndex];
                long phaseDuration = phase.DurationInMS;

                var boonDistributions = new Dictionary<Player, BuffDistribution>();
                foreach (Player p in playerList)
                {
                    boonDistributions[p] = p.GetBuffDistribution(log, phaseIndex);
                }

                var boonsToTrack = new HashSet<Buff>(boonDistributions.SelectMany(x => x.Value).Select(x => log.Buffs.BuffsByIds[x.Key]));

                var final =
                    new Dictionary<long, FinalPlayerBuffs>();
                var finalActive =
                    new Dictionary<long, FinalPlayerBuffs>();

                foreach (Buff boon in boonsToTrack)
                {
                    double totalGeneration = 0;
                    double totalOverstack = 0;
                    double totalWasted = 0;
                    double totalUnknownExtension = 0;
                    double totalExtension = 0;
                    double totalExtended = 0;
                    //
                    double totalActiveGeneration = 0;
                    double totalActiveOverstack = 0;
                    double totalActiveWasted = 0;
                    double totalActiveUnknownExtension = 0;
                    double totalActiveExtension = 0;
                    double totalActiveExtended = 0;
                    bool hasGeneration = false;
                    int activePlayerCount = 0;
                    foreach (KeyValuePair<Player, BuffDistribution> pair in boonDistributions)
                    {
                        BuffDistribution boons = pair.Value;
                        long playerActiveDuration = phase.GetActorActiveDuration(pair.Key, log);
                        if (boons.ContainsKey(boon.ID))
                        {
                            hasGeneration = hasGeneration || boons.HasSrc(boon.ID, AgentItem);
                            double generation = boons.GetGeneration(boon.ID, AgentItem);
                            double overstack = boons.GetOverstack(boon.ID, AgentItem);
                            double wasted = boons.GetWaste(boon.ID, AgentItem);
                            double unknownExtension = boons.GetUnknownExtension(boon.ID, AgentItem);
                            double extension = boons.GetExtension(boon.ID, AgentItem);
                            double extended = boons.GetExtended(boon.ID, AgentItem);

                            totalGeneration += generation;
                            totalOverstack += overstack;
                            totalWasted += wasted;
                            totalUnknownExtension += unknownExtension;
                            totalExtension += extension;
                            totalExtended += extended;
                            if (playerActiveDuration > 0)
                            {
                                activePlayerCount++;
                                totalActiveGeneration += generation / playerActiveDuration;
                                totalActiveOverstack += overstack / playerActiveDuration;
                                totalActiveWasted += wasted / playerActiveDuration;
                                totalActiveUnknownExtension += unknownExtension / playerActiveDuration;
                                totalActiveExtension += extension / playerActiveDuration;
                                totalActiveExtended += extended / playerActiveDuration;
                            }
                        }
                    }
                    totalGeneration /= phaseDuration;
                    totalOverstack /= phaseDuration;
                    totalWasted /= phaseDuration;
                    totalUnknownExtension /= phaseDuration;
                    totalExtension /= phaseDuration;
                    totalExtended /= phaseDuration;

                    if (hasGeneration)
                    {
                        var uptime = new FinalPlayerBuffs();
                        var uptimeActive = new FinalPlayerBuffs();
                        final[boon.ID] = uptime;
                        finalActive[boon.ID] = uptimeActive;
                        if (boon.Type == BuffType.Duration)
                        {
                            uptime.Generation = Math.Round(100.0 * totalGeneration / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Overstack = Math.Round(100.0 * (totalOverstack + totalGeneration) / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Wasted = Math.Round(100.0 * (totalWasted) / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.UnknownExtended = Math.Round(100.0 * (totalUnknownExtension) / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.ByExtension = Math.Round(100.0 * (totalExtension) / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Extended = Math.Round(100.0 * (totalExtended) / playerList.Count, GeneralHelper.BoonDigit);
                            //
                            if (activePlayerCount > 0)
                            {
                                uptimeActive.Generation = Math.Round(100.0 * totalActiveGeneration / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.Overstack = Math.Round(100.0 * (totalActiveOverstack + totalActiveGeneration) / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.Wasted = Math.Round(100.0 * (totalActiveWasted) / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.UnknownExtended = Math.Round(100.0 * (totalActiveUnknownExtension) / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.ByExtension = Math.Round(100.0 * (totalActiveExtension) / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.Extended = Math.Round(100.0 * (totalActiveExtended) / activePlayerCount, GeneralHelper.BoonDigit);
                            }
                        }
                        else if (boon.Type == BuffType.Intensity)
                        {
                            uptime.Generation = Math.Round(totalGeneration / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Overstack = Math.Round((totalOverstack + totalGeneration) / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Wasted = Math.Round((totalWasted) / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.UnknownExtended = Math.Round((totalUnknownExtension) / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.ByExtension = Math.Round((totalExtension) / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Extended = Math.Round((totalExtended) / playerList.Count, GeneralHelper.BoonDigit);
                            //
                            if (activePlayerCount > 0)
                            {
                                uptimeActive.Generation = Math.Round(totalActiveGeneration / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.Overstack = Math.Round((totalActiveOverstack + totalActiveGeneration) / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.Wasted = Math.Round((totalActiveWasted) / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.UnknownExtended = Math.Round((totalActiveUnknownExtension) / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.ByExtension = Math.Round((totalActiveExtension) / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.Extended = Math.Round((totalActiveExtended) / activePlayerCount, GeneralHelper.BoonDigit);
                            }
                        }
                    }
                }

                uptimesByPhase.Add(final);
                uptimesActiveByPhase.Add(finalActive);
            }

            return (uptimesByPhase, uptimesActiveByPhase);
        }

        private void SetBuffs(ParsedLog log)
        {
            // Boons applied to self
            _selfBuffs = new List<Dictionary<long, FinalPlayerBuffs>>();
            _selfActiveBuffs = new List<Dictionary<long, FinalPlayerBuffs>>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                var final = new Dictionary<long, FinalPlayerBuffs>();
                var finalActive = new Dictionary<long, FinalPlayerBuffs>();

                PhaseData phase = phases[phaseIndex];

                BuffDistribution selfBoons = GetBuffDistribution(log, phaseIndex);
                Dictionary<long, long> buffPresence = GetBuffPresence(log, phaseIndex);

                long phaseDuration = phase.DurationInMS;
                long playerActiveDuration = phase.GetActorActiveDuration(this, log);
                foreach (Buff boon in TrackedBuffs)
                {
                    if (selfBoons.ContainsKey(boon.ID))
                    {
                        var uptime = new FinalPlayerBuffs
                        {
                            Uptime = 0,
                            Generation = 0,
                            Overstack = 0,
                            Wasted = 0,
                            UnknownExtended = 0,
                            ByExtension = 0,
                            Extended = 0
                        };
                        var uptimeActive = new FinalPlayerBuffs
                        {
                            Uptime = 0,
                            Generation = 0,
                            Overstack = 0,
                            Wasted = 0,
                            UnknownExtended = 0,
                            ByExtension = 0,
                            Extended = 0
                        };
                        final[boon.ID] = uptime;
                        finalActive[boon.ID] = uptimeActive;
                        double generationValue = selfBoons.GetGeneration(boon.ID, AgentItem);
                        double uptimeValue = selfBoons.GetUptime(boon.ID);
                        double overstackValue = selfBoons.GetOverstack(boon.ID, AgentItem);
                        double wasteValue = selfBoons.GetWaste(boon.ID, AgentItem);
                        double unknownExtensionValue = selfBoons.GetUnknownExtension(boon.ID, AgentItem);
                        double extensionValue = selfBoons.GetExtension(boon.ID, AgentItem);
                        double extendedValue = selfBoons.GetExtended(boon.ID, AgentItem);
                        if (boon.Type == BuffType.Duration)
                        {
                            uptime.Uptime = Math.Round(100.0 * uptimeValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.Generation = Math.Round(100.0 * generationValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.Overstack = Math.Round(100.0 * (overstackValue + generationValue) / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.Wasted = Math.Round(100.0 * wasteValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.UnknownExtended = Math.Round(100.0 * unknownExtensionValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.ByExtension = Math.Round(100.0 * extensionValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.Extended = Math.Round(100.0 * extendedValue / phaseDuration, GeneralHelper.BoonDigit);
                            //
                            if (playerActiveDuration > 0)
                            {
                                uptimeActive.Uptime = Math.Round(100.0 * uptimeValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.Generation = Math.Round(100.0 * generationValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.Overstack = Math.Round(100.0 * (overstackValue + generationValue) / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.Wasted = Math.Round(100.0 * wasteValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.UnknownExtended = Math.Round(100.0 * unknownExtensionValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.ByExtension = Math.Round(100.0 * extensionValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.Extended = Math.Round(100.0 * extendedValue / playerActiveDuration, GeneralHelper.BoonDigit);
                            }
                        }
                        else if (boon.Type == BuffType.Intensity)
                        {
                            uptime.Uptime = Math.Round(uptimeValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.Generation = Math.Round(generationValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.Overstack = Math.Round((overstackValue + generationValue) / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.Wasted = Math.Round(wasteValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.UnknownExtended = Math.Round(unknownExtensionValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.ByExtension = Math.Round(extensionValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.Extended = Math.Round(extendedValue / phaseDuration, GeneralHelper.BoonDigit);
                            //
                            if (playerActiveDuration > 0)
                            {
                                uptimeActive.Uptime = Math.Round(uptimeValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.Generation = Math.Round(generationValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.Overstack = Math.Round((overstackValue + generationValue) / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.Wasted = Math.Round(wasteValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.UnknownExtended = Math.Round(unknownExtensionValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.ByExtension = Math.Round(extensionValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.Extended = Math.Round(extendedValue / playerActiveDuration, GeneralHelper.BoonDigit);
                            }
                            //
                            if (buffPresence.TryGetValue(boon.ID, out long presenceValueBoon))
                            {
                                uptime.Presence = Math.Round(100.0 * presenceValueBoon / phaseDuration, GeneralHelper.BoonDigit);
                                //
                                if (playerActiveDuration > 0)
                                {
                                    uptimeActive.Presence = Math.Round(100.0 * presenceValueBoon / playerActiveDuration, GeneralHelper.BoonDigit);
                                }
                            }
                        }
                    }
                }

                _selfBuffs.Add(final);
                _selfActiveBuffs.Add(finalActive);
            }

            // Boons applied to player's group
            var otherPlayersInGroup = log.PlayerList
                .Where(p => p.Group == Group && Agent != p.Agent)
                .ToList();
            (_groupBuffs, _groupActiveBuffs) = GetBoonsForPlayers(otherPlayersInGroup, log);

            // Boons applied to other groups
            var offGroupPlayers = log.PlayerList.Where(p => p.Group != Group).ToList();
            (_offGroupBuffs, _offGroupActiveBuffs) = GetBoonsForPlayers(offGroupPlayers, log);

            // Boons applied to squad
            var otherPlayers = log.PlayerList.Where(p => p.Agent != Agent).ToList();
            (_squadBuffs, _squadActiveBuffs) = GetBoonsForPlayers(otherPlayers, log);
        }

        internal void Anonymize(int index)
        {
            Character = "Player " + index;
            Account = "Account " + index;
        }

        public List<DeathRecap> GetDeathRecaps(ParsedLog log)
        {
            if (_deathRecaps == null)
            {
                SetDeathRecaps(log);
            }
            if (_deathRecaps.Count == 0)
            {
                return null;
            }
            return _deathRecaps;
        }

        public string[] GetWeaponsArray(ParsedLog log)
        {
            if (_weaponsArray == null)
            {
                EstimateWeapons(log);
            }
            return _weaponsArray;
        }

        public List<Consumable> GetConsumablesList(ParsedLog log, long start, long end)
        {
            if (_consumeList == null)
            {
                SetConsumablesList(log);
            }
            return _consumeList.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public Dictionary<string, List<DamageModifierData>> GetDamageModifierData(ParsedLog log, NPC target)
        {
            if (_damageModifiers == null)
            {
                SetDamageModifiersData(log);
            }
            if (target != null)
            {
                if (_damageModifiersTargets.TryGetValue(target, out Dictionary<string, List<DamageModifierData>> res))
                {
                    return res;
                }
                else
                {
                    return new Dictionary<string, List<DamageModifierData>>();
                }
            }
            return _damageModifiers;
        }

        public HashSet<string> GetPresentDamageModifier(ParsedLog log)
        {
            if (_presentDamageModifiers == null)
            {
                SetDamageModifiersData(log);
            }
            return _presentDamageModifiers;
        }

        // Private Methods

        private void SetDamageModifiersData(ParsedLog log)
        {
            _damageModifiers = new Dictionary<string, List<DamageModifierData>>();
            _damageModifiersTargets = new Dictionary<NPC, Dictionary<string, List<DamageModifierData>>>();
            _presentDamageModifiers = new HashSet<string>();
            // If conjured sword, targetless or WvW, stop
            if (IsFakeActor || log.FightData.Logic.Targetless || log.FightData.Logic.Mode == FightLogic.ParseMode.WvW)
            {
                return;
            }
            var damageMods = new List<DamageModifier>(log.DamageModifiers.DamageModifiersPerSource[DamageModifier.ModifierSource.ItemBuff]);
            damageMods.AddRange(log.DamageModifiers.DamageModifiersPerSource[DamageModifier.ModifierSource.CommonBuff]);
            damageMods.AddRange(log.DamageModifiers.GetModifiersPerProf(Prof));
            foreach (DamageModifier mod in damageMods)
            {
                mod.ComputeDamageModifier(_damageModifiers, _damageModifiersTargets, this, log);
            }
            _presentDamageModifiers.UnionWith(_damageModifiers.Keys);
            foreach (NPC tar in _damageModifiersTargets.Keys)
            {
                _presentDamageModifiers.UnionWith(_damageModifiersTargets[tar].Keys);
            }
        }

        private void SetDeathRecaps(ParsedLog log)
        {
            _deathRecaps = new List<DeathRecap>();
            List<DeathRecap> res = _deathRecaps;
            List<DeadEvent> deads = log.CombatData.GetDeadEvents(AgentItem);
            List<DownEvent> downs = log.CombatData.GetDownEvents(AgentItem);
            long lastDeathTime = 0;
            List<AbstractDamageEvent> damageLogs = GetDamageTakenLogs(null, log, 0, log.FightData.FightDuration);
            foreach (DeadEvent dead in deads)
            {
                var recap = new DeathRecap()
                {
                    DeathTime = (int)dead.Time
                };
                DownEvent downed = downs.LastOrDefault(x => x.Time <= dead.Time && x.Time >= lastDeathTime);
                if (downed != null)
                {
                    var damageToDown = damageLogs.Where(x => x.Time <= downed.Time && (x.HasHit || x.HasDowned) && x.Time > lastDeathTime).ToList();
                    recap.ToDown = damageToDown.Count > 0 ? new List<DeathRecap.DeathRecapDamageItem>() : null;
                    int damage = 0;
                    for (int i = damageToDown.Count - 1; i >= 0; i--)
                    {
                        AbstractDamageEvent dl = damageToDown[i];
                        AgentItem ag = dl.From;
                        var item = new DeathRecap.DeathRecapDamageItem()
                        {
                            Time = (int)dl.Time,
                            IndirectDamage = dl is NonDirectDamageEvent,
                            ID = dl.SkillId,
                            Damage = dl.Damage,
                            Src = ag != null ? ag.Name.Replace("\u0000", "").Split(':')[0] : ""
                        };
                        damage += dl.Damage;
                        recap.ToDown.Add(item);
                        if (damage > 20000)
                        {
                            break;
                        }
                    }
                    var damageToKill = damageLogs.Where(x => x.Time > downed.Time && x.Time <= dead.Time && (x.HasHit || x.HasDowned) && x.Time > lastDeathTime).ToList();
                    recap.ToKill = damageToKill.Count > 0 ? new List<DeathRecap.DeathRecapDamageItem>() : null;
                    for (int i = damageToKill.Count - 1; i >= 0; i--)
                    {
                        AbstractDamageEvent dl = damageToKill[i];
                        AgentItem ag = dl.From;
                        var item = new DeathRecap.DeathRecapDamageItem()
                        {
                            Time = (int)dl.Time,
                            IndirectDamage = dl is NonDirectDamageEvent,
                            ID = dl.SkillId,
                            Damage = dl.Damage,
                            Src = ag != null ? ag.Name.Replace("\u0000", "").Split(':')[0] : ""
                        };
                        recap.ToKill.Add(item);
                    }
                }
                else
                {
                    recap.ToDown = null;
                    var damageToKill = damageLogs.Where(x => x.Time < dead.Time && x.Damage > 0 && x.Time > lastDeathTime).ToList();
                    recap.ToKill = damageToKill.Count > 0 ? new List<DeathRecap.DeathRecapDamageItem>() : null;
                    int damage = 0;
                    for (int i = damageToKill.Count - 1; i >= 0; i--)
                    {
                        AbstractDamageEvent dl = damageToKill[i];
                        AgentItem ag = dl.From;
                        var item = new DeathRecap.DeathRecapDamageItem()
                        {
                            Time = (int)dl.Time,
                            IndirectDamage = dl is NonDirectDamageEvent,
                            ID = dl.SkillId,
                            Damage = dl.Damage,
                            Src = ag != null ? ag.Name.Replace("\u0000", "").Split(':')[0] : ""
                        };
                        damage += dl.Damage;
                        recap.ToKill.Add(item);
                        if (damage > 20000)
                        {
                            break;
                        }
                    }
                }
                lastDeathTime = dead.Time;
                res.Add(recap);
            }
        }

        private void EstimateWeapons(ParsedLog log)
        {
            if (Prof == "Sword")
            {
                _weaponsArray = new string[]
                {
                    "Sword",
                    "2Hand",
                    null,
                    null,
                    null,
                    null,
                    null,
                    null
                };
                return;
            }
            string[] weapons = new string[8];//first 2 for first set next 2 for second set, second sets of 4 for underwater
            List<AbstractCastEvent> casting = GetCastLogs(log, 0, log.FightData.FightDuration);
            int swapped = -1;
            long swappedTime = 0;
            var swaps = casting.Where(x => x.SkillId == SkillItem.WeaponSwapId).Select(x =>
            {
                if (x is WeaponSwapEvent wse)
                {
                    return wse.SwappedTo;
                }
                return -1;
            }).ToList();
            foreach (AbstractCastEvent cl in casting)
            {
                if (cl.ActualDuration == 0 && cl.SkillId != SkillItem.WeaponSwapId)
                {
                    continue;
                }
                SkillItem skill = cl.Skill;
                // first iteration
                if (swapped == -1)
                {
                    swapped = skill.FindWeaponSlot(swaps);
                }
                if (!skill.EstimateWeapons(weapons, swapped, cl.Time > swappedTime) && cl is WeaponSwapEvent swe)
                {
                    //wepswap  
                    swapped = swe.SwappedTo;
                    swappedTime = swe.Time;
                }
            }
            _weaponsArray = weapons;
        }

        private void SetConsumablesList(ParsedLog log)
        {
            List<Buff> consumableList = log.Buffs.BuffsByNature[BuffNature.Consumable];
            _consumeList = new List<Consumable>();
            long fightDuration = log.FightData.FightDuration;
            foreach (Buff consumable in consumableList)
            {
                foreach (AbstractBuffEvent c in log.CombatData.GetBuffData(consumable.ID))
                {
                    if (!(c is BuffApplyEvent ba) || AgentItem != ba.To)
                    {
                        continue;
                    }
                    long time = 0;
                    if (!ba.Initial)
                    {
                        time = ba.Time;
                    }
                    if (time <= fightDuration)
                    {
                        Consumable existing = _consumeList.Find(x => x.Time == time && x.Buff.ID == consumable.ID);
                        if (existing != null)
                        {
                            existing.Stack++;
                        }
                        else
                        {
                            _consumeList.Add(new Consumable(consumable, time, ba.AppliedDuration));
                        }
                    }
                }
            }
            _consumeList.Sort((x, y) => x.Time.CompareTo(y.Time));

        }

        protected override void InitAdditionalCombatReplayData(ParsedLog log)
        {
            if (IsFakeActor)
            {
                return;
            }
            // Fight related stuff
            log.FightData.Logic.ComputePlayerCombatReplayActors(this, log, CombatReplay);
            if (CombatReplay.Rotations.Any())
            {
                CombatReplay.Decorations.Add(new FacingDecoration(((int)CombatReplay.TimeOffsets.start, (int)CombatReplay.TimeOffsets.end), new AgentConnector(this), CombatReplay.PolledRotations));
            }
        }

        //
        private class PlayerSerializable : AbstractMasterActorSerializable
        {
            public int Group { get; set; }
            public List<long> Dead { get; set; }
            public List<long> Down { get; set; }
            public List<long> Dc { get; set; }
        }

        public override AbstractMasterActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            (List<(long start, long end)> deads, List<(long start, long end)> downs, List<(long start, long end)> dcs) = GetStatus(log);
            var aux = new PlayerSerializable
            {
                Group = Group,
                Img = Icon,
                Type = "Player",
                ID = GetCombatReplayID(log),
                Positions = new List<double>(),
                Dead = new List<long>(),
                Down = new List<long>(),
                Dc = new List<long>()
            };
            foreach (Point3D pos in CombatReplay.PolledPositions)
            {
                (double x, double y) = map.GetMapCoord(pos.X, pos.Y);
                aux.Positions.Add(x);
                aux.Positions.Add(y);
            }
            foreach ((long start, long end) in deads)
            {
                aux.Dead.Add(start);
                aux.Dead.Add(end);
            }
            foreach ((long start, long end) in downs)
            {
                aux.Down.Add(start);
                aux.Down.Add(end);
            }
            foreach ((long start, long end) in dcs)
            {
                aux.Dc.Add(start);
                aux.Dc.Add(end);
            }

            return aux;
        }

        protected override void InitCombatReplay(ParsedLog log)
        {
            if (!log.CombatData.HasMovementData || IsFakeActor)
            {
                // no combat replay support on fight
                return;
            }
            CombatReplay = new CombatReplay();
            SetMovements(log);
            CombatReplay.PollingRate(log.FightData.FightDuration, true);
        }
    }
}
