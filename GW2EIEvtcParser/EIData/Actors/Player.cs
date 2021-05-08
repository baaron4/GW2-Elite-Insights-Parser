using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public class Player : AbstractSingleActor
    {
        // Fields
        public string Account { get; protected set; }
        public bool HasCommanderTag => AgentItem.HasCommanderTag;
        public int Group { get; private set; }

        private List<Consumable> _consumeList;
        private List<DeathRecap> _deathRecaps;
        private CachingCollectionWithTarget<Dictionary<string, DamageModifierStat>> _damageModifiersPerTargets;
        private CachingCollectionWithTarget<Dictionary<string, List<DamageModifierEvent>>> _damageModifierEventsPerTargets;
        // statistics
        private CachingCollection<FinalPlayerSupport> _playerSupportStats;
        private CachingCollectionCustom<BuffEnum, Dictionary<long, FinalPlayerBuffs>[]> _buffStats;
        //weaponslist
        private string[] _weaponsArray;

        private readonly string _forceIcon = null;

        private static int FriendlyPlayerCount = 0;
        // Constructors
        internal Player(AgentItem agent, bool noSquad) : base(agent)
        {
            if (agent.Type != AgentItem.AgentType.Player)
            {
                throw new EvtcAgentException("Agent is not a Player");
            }
            if (!agent.IsNotInSquadPlayer)
            {
                string[] name = agent.Name.Split('\0');
                if (name.Length < 2)
                {
                    throw new EvtcAgentException("Name problem on Player");
                }
                if (name[1].Length == 0 || name[2].Length == 0 || Character.Contains("-"))
                {
                    throw new EvtcAgentException("Missing Group on Player");
                }
                Account = name[1].TrimStart(':');
                Group = noSquad ? 1 : int.Parse(name[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
            } 
            else
            {
                IsCustomActor = true;
                Group = 51;
                Account = "Friendly Player " + (++FriendlyPlayerCount);
            }
        }

        internal Player(AgentItem agent, string account) : base(agent)
        {
            if (agent.Type == AgentItem.AgentType.Player)
            {
                throw new EvtcAgentException("Agent is Player, use proper player constructor");
            }
            Account = account;
            Group = 51;
            IsCustomActor = !IsDummyActor;
        }

        internal Player(AgentItem agent, string account, string icon) : this(agent, account)
        {
            _forceIcon = icon;
        }

        public override string GetIcon()
        {
            return _forceIcon ?? GetProfIcon(Prof);
        }


        internal void MakeSquadless()
        {
            if (!IsFakeActor)
            {
                Group = 1;
            }
        }

        // Public methods

        public FinalPlayerSupport GetPlayerSupportStats(ParsedEvtcLog log, long start, long end)
        {
            if(_playerSupportStats == null)
            {
                _playerSupportStats = new CachingCollection<FinalPlayerSupport>(log);
            }
            if (!_playerSupportStats.TryGetValue(start, end, out FinalPlayerSupport value))
            {
                value = new FinalPlayerSupport(log, this, start, end);
                _playerSupportStats.Set(start, end, value);
            }
            return value;
        }

        public IReadOnlyDictionary<long, FinalPlayerBuffs> GetBuffs(BuffEnum type, ParsedEvtcLog log, long start, long end)
        {
            if (_buffStats == null)
            {
                _buffStats = new CachingCollectionCustom<BuffEnum, Dictionary<long, FinalPlayerBuffs>[]>(log, BuffEnum.Self);
            }
            if (!_buffStats.TryGetValue(start, end, type, out Dictionary<long, FinalPlayerBuffs>[] value))
            {
                value = SetBuffs(log, start, end, type);
                _buffStats.Set(start, end, type, value);
            }
            return value[0];
        }

        public IReadOnlyDictionary<long, FinalPlayerBuffs> GetActiveBuffs(BuffEnum type, ParsedEvtcLog log, long start, long end)
        {
            if (_buffStats == null)
            {
                _buffStats = new CachingCollectionCustom<BuffEnum, Dictionary<long, FinalPlayerBuffs>[]>(log, BuffEnum.Self);
            }
            if (!_buffStats.TryGetValue(start, end, type, out Dictionary<long, FinalPlayerBuffs>[] value))
            {
                value = SetBuffs(log, start, end, type);
                _buffStats.Set(start, end, type, value);
            }
            return value[1];
        }

        private Dictionary<long, FinalPlayerBuffs>[] SetBuffs(ParsedEvtcLog log, long start, long end, BuffEnum type)
        {
            switch(type)
            {
                case BuffEnum.Group:
                    var otherPlayersInGroup = log.PlayerList
                        .Where(p => p.Group == Group && Agent != p.Agent && !p.IsFakeActor)
                        .ToList();
                    return FinalPlayerBuffs.GetBuffsForPlayers(otherPlayersInGroup, log, AgentItem, start, end);
                case BuffEnum.OffGroup:
                    var offGroupPlayers = log.PlayerList.Where(p => p.Group != Group && !p.IsFakeActor).ToList();
                    return FinalPlayerBuffs.GetBuffsForPlayers(offGroupPlayers, log, AgentItem, start, end);
                case BuffEnum.Squad:
                    var otherPlayers = log.PlayerList.Where(p => p.Agent != Agent && !p.IsFakeActor).ToList();
                    return FinalPlayerBuffs.GetBuffsForPlayers(otherPlayers, log, AgentItem, start, end);
                case BuffEnum.Self:
                default:
                    return FinalPlayerBuffs.GetBuffsForSelf(log, this, start, end);
            }
        }

        internal void Anonymize(int index)
        {
            Character = "Player " + index;
            Account = "Account " + index;
        }

        public IReadOnlyList<DeathRecap> GetDeathRecaps(ParsedEvtcLog log)
        {
            if (_deathRecaps == null)
            {
                SetDeathRecaps(log);
            }
            return _deathRecaps;
        }

        public IReadOnlyList<string> GetWeaponsArray(ParsedEvtcLog log)
        {
            if (_weaponsArray == null)
            {
                EstimateWeapons(log);
            }
            return _weaponsArray;
        }

        public IReadOnlyList<Consumable> GetConsumablesList(ParsedEvtcLog log, long start, long end)
        {
            if (_consumeList == null)
            {
                SetConsumablesList(log);
            }
            return _consumeList.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        private Dictionary<string, DamageModifierStat> ComputeDamageModifierStats(NPC target, ParsedEvtcLog log, long start, long end)
        {
            // Check if damage mods against target
            if (_damageModifierEventsPerTargets.TryGetValue(log.FightData.FightStart, log.FightData.FightEnd, target, out Dictionary<string, List<DamageModifierEvent>> events))
            {
                var res = new Dictionary<string, DamageModifierStat>();
                foreach (KeyValuePair<string, List<DamageModifierEvent>> pair in events)
                {
                    DamageModifier damageMod = pair.Value.FirstOrDefault()?.DamageModifier;
                    if (damageMod != null)
                    {
                        var eventsToUse = pair.Value.Where(x => x.Time >= start && x.Time <= end).ToList();
                        int totalDamage = damageMod.GetTotalDamage(this, log, target, start, end);
                        IReadOnlyList<AbstractHealthDamageEvent> typeHits = damageMod.GetHitDamageEvents(this, log, target, start, end);
                        res[pair.Key] = new DamageModifierStat(eventsToUse.Count, typeHits.Count, eventsToUse.Sum(x => x.DamageGain), totalDamage);
                    }
                }
                _damageModifiersPerTargets.Set(start, end, target, res);
                return res;
            } 
            // Check if we already filled the cache, that means no damage modifiers against given target
            else if (_damageModifierEventsPerTargets.TryGetValue(log.FightData.FightStart, log.FightData.FightEnd, null, out events))
            {
                var res = new Dictionary<string, DamageModifierStat>();
                _damageModifiersPerTargets.Set(start, end, target, res);
                return res;
            }
            return null;
        }

        public IReadOnlyDictionary<string, DamageModifierStat> GetDamageModifierStats(NPC target, ParsedEvtcLog log, long start, long end)
        {
            if (!log.ParserSettings.ComputeDamageModifiers || IsDummyActor)
            {
                return new Dictionary<string, DamageModifierStat>();
            }
            if (_damageModifiersPerTargets == null)
            {
                _damageModifiersPerTargets = new CachingCollectionWithTarget<Dictionary<string, DamageModifierStat>>(log);
                _damageModifierEventsPerTargets = new CachingCollectionWithTarget<Dictionary<string, List<DamageModifierEvent>>>(log);
            }
            if (_damageModifiersPerTargets.TryGetValue(start, end, target, out Dictionary<string, DamageModifierStat> res))
            {
                return res;
            }
            res = ComputeDamageModifierStats(target, log, start, end);
            if (res != null)
            {
                return res;
            }
            //
            var damageMods = new List<DamageModifier>();
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(Source.Item, out IReadOnlyList<DamageModifier> list))
            {
                damageMods.AddRange(list);
            }
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(Source.Gear, out list))
            {
                damageMods.AddRange(list);
            }
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(Source.Common, out list))
            {
                damageMods.AddRange(list);
            }
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(Source.FightSpecific, out list))
            {
                damageMods.AddRange(list);
            }
            damageMods.AddRange(log.DamageModifiers.GetModifiersPerProf(Prof));
            //
            var damageModifierEvents = new List<DamageModifierEvent>();
            foreach (DamageModifier damageMod in damageMods)
            {
                damageModifierEvents.AddRange(damageMod.ComputeDamageModifier(this, log));
            }
            damageModifierEvents.Sort((x, y) => x.Time.CompareTo(y.Time));
            var damageModifiersEvents = damageModifierEvents.GroupBy(y => y.DamageModifier.Name).ToDictionary(y => y.Key, y => y.ToList());
            _damageModifierEventsPerTargets.Set(log.FightData.FightStart, log.FightData.FightEnd, null, damageModifiersEvents);
            var damageModifiersEventsByTarget = damageModifierEvents.GroupBy(x => x.Dst).ToDictionary(x => x.Key, x => x.GroupBy(y => y.DamageModifier.Name).ToDictionary(y => y.Key, y=> y.ToList()));
            foreach (AgentItem actor in damageModifiersEventsByTarget.Keys)
            {
                _damageModifierEventsPerTargets.Set(log.FightData.FightStart, log.FightData.FightEnd, log.FindActor(actor), damageModifiersEventsByTarget[actor]);
            }
            //
            res = ComputeDamageModifierStats(target, log, start, end);
            return res;
        }

        public IReadOnlyCollection<string> GetPresentDamageModifier(ParsedEvtcLog log)
        {
            return new HashSet<string>(GetDamageModifierStats(null, log, log.FightData.FightStart, log.FightData.FightEnd).Keys);
        }

        // Private Methods

        private void SetDeathRecaps(ParsedEvtcLog log)
        {
            _deathRecaps = new List<DeathRecap>();
            IReadOnlyList<DeadEvent> deads = log.CombatData.GetDeadEvents(AgentItem);
            IReadOnlyList<DownEvent> downs = log.CombatData.GetDownEvents(AgentItem);
            IReadOnlyList<AliveEvent> ups = log.CombatData.GetAliveEvents(AgentItem);
            long lastDeathTime = 0;
            IReadOnlyList<AbstractHealthDamageEvent> damageLogs = GetDamageTakenEvents(null, log, 0, log.FightData.FightEnd);
            foreach (DeadEvent dead in deads)
            {
                _deathRecaps.Add(new DeathRecap(log, damageLogs, dead, downs, ups, lastDeathTime));
                lastDeathTime = dead.Time;
            }
        }

        private void EstimateWeapons(ParsedEvtcLog log)
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
            IReadOnlyList<AbstractCastEvent> casting = GetCastEvents(log, 0, log.FightData.FightEnd);
            int swapped = -1;
            long swappedTime = 0;
            var swaps = casting.OfType<WeaponSwapEvent>().Select(x =>
            {
                return x.SwappedTo;
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
                if (!skill.EstimateWeapons(weapons, swapped, cl.Time > swappedTime + WeaponSwapDelayConstant) && cl is WeaponSwapEvent swe)
                {
                    //wepswap  
                    swapped = swe.SwappedTo;
                    swappedTime = swe.Time;
                }
            }
            _weaponsArray = weapons;
        }

        private void SetConsumablesList(ParsedEvtcLog log)
        {
            IReadOnlyList<Buff> consumableList = log.Buffs.BuffsByNature[BuffNature.Consumable];
            _consumeList = new List<Consumable>();
            long fightDuration = log.FightData.FightEnd;
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

        protected override void InitAdditionalCombatReplayData(ParsedEvtcLog log)
        {
            // Fight related stuff
            log.FightData.Logic.ComputePlayerCombatReplayActors(this, log, CombatReplay);
            if (CombatReplay.Rotations.Any())
            {
                CombatReplay.Decorations.Add(new FacingDecoration(((int)CombatReplay.TimeOffsets.start, (int)CombatReplay.TimeOffsets.end), new AgentConnector(this), CombatReplay.PolledRotations));
            }
        }

        //

        public override AbstractSingleActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedEvtcLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return new PlayerSerializable(this, log, map, CombatReplay);
        }

        public List<Point3D> GetCombatReplayActivePositions(ParsedEvtcLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            (IReadOnlyList<(long start, long end)> deads, _, IReadOnlyList<(long start, long end)> dcs) = GetStatus(log);
            var activePositions = new List<Point3D>(GetCombatReplayPolledPositions(log));
            for (int i = 0; i < activePositions.Count; i++)
            {
                Point3D cur = activePositions[i];
                foreach ((long start, long end) in deads)
                {
                    if (cur.Time >= start && cur.Time <= end)
                    {
                        activePositions[i] = null;
                    }
                }
                foreach ((long start, long end) in dcs)
                {
                    if (cur.Time >= start && cur.Time <= end)
                    {
                        activePositions[i] = null;
                    }
                }
            }
            return activePositions;
        }
    }
}
