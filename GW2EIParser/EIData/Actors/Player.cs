using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GW2EIParser.Logic;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.EIData
{
    public class Player : AbstractSingleActor
    {
        // Fields
        public string Account { get; protected set; }
        public int Group { get; }

        private List<Consumable> _consumeList;
        private List<DeathRecap> _deathRecaps;
        private Dictionary<string, List<DamageModifierStat>> _damageModifiers;
        private HashSet<string> _presentDamageModifiers;
        private Dictionary<NPC, Dictionary<string, List<DamageModifierStat>>> _damageModifiersTargets;
        // statistics
        private List<FinalPlayerSupport> _playerSupport;
        private List<Dictionary<long, FinalPlayerBuffs>> _selfBuffs;
        private List<Dictionary<long, FinalPlayerBuffs>> _groupBuffs;
        private List<Dictionary<long, FinalPlayerBuffs>> _offGroupBuffs;
        private List<Dictionary<long, FinalPlayerBuffs>> _squadBuffs;
        private List<Dictionary<long, FinalPlayerBuffs>> _selfBuffsActive;
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
                    foreach (Buff boon in log.Buffs.BuffsByNature[BuffNature.Boon])
                    {
                        // add everything from total
                        if (totals.Removals.TryGetValue(boon.ID, out (long count, double time) item))
                        {
                            playerSup.BoonStrips += item.count;
                            playerSup.BoonStripsTime += item.time;
                        }
                        // remove everything from self
                        if (self.Removals.TryGetValue(boon.ID, out item))
                        {
                            playerSup.BoonStrips -= item.count;
                            playerSup.BoonStripsTime -= item.time;
                        }
                    }
                    foreach (Buff condition in log.Buffs.BuffsByNature[BuffNature.Condition])
                    {
                        // add everything from self
                        if (self.Removals.TryGetValue(condition.ID, out (long count, double time) item))
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
                            if (other.Removals.TryGetValue(condition.ID, out item))
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
            if (_selfBuffsActive == null)
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
                    return _selfBuffsActive[phaseIndex];
            }
        }

        public List<Dictionary<long, FinalPlayerBuffs>> GetActiveBuffs(ParsedLog log, BuffEnum type)
        {
            if (_selfBuffsActive == null)
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
                    return _selfBuffsActive;
            }
        }

        private void SetBuffs(ParsedLog log)
        {
            // Boons applied to self

            (_selfBuffs, _selfBuffsActive) = FinalPlayerBuffs.GetBuffsForSelf(log, this);

            // Boons applied to player's group
            var otherPlayersInGroup = log.PlayerList
                .Where(p => p.Group == Group && Agent != p.Agent)
                .ToList();
            (_groupBuffs, _groupActiveBuffs) = FinalPlayerBuffs.GetBuffsForPlayers(otherPlayersInGroup, log, AgentItem);

            // Boons applied to other groups
            var offGroupPlayers = log.PlayerList.Where(p => p.Group != Group).ToList();
            (_offGroupBuffs, _offGroupActiveBuffs) = FinalPlayerBuffs.GetBuffsForPlayers(offGroupPlayers, log, AgentItem);

            // Boons applied to squad
            var otherPlayers = log.PlayerList.Where(p => p.Agent != Agent).ToList();
            (_squadBuffs, _squadActiveBuffs) = FinalPlayerBuffs.GetBuffsForPlayers(otherPlayers, log, AgentItem);
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

        public Dictionary<string, List<DamageModifierStat>> GetDamageModifierStats(ParsedLog log, NPC target)
        {
            if (_damageModifiers == null)
            {
                SetDamageModifiersData(log);
            }
            if (target != null)
            {
                if (_damageModifiersTargets.TryGetValue(target, out Dictionary<string, List<DamageModifierStat>> res))
                {
                    return res;
                }
                else
                {
                    return new Dictionary<string, List<DamageModifierStat>>();
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
            _damageModifiers = new Dictionary<string, List<DamageModifierStat>>();
            _damageModifiersTargets = new Dictionary<NPC, Dictionary<string, List<DamageModifierStat>>>();
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
            List<AbstractDamageEvent> damageLogs = GetDamageTakenLogs(null, log, 0, log.FightData.FightEnd);
            foreach (DeadEvent dead in deads)
            {
                res.Add(new DeathRecap(damageLogs, dead, downs, lastDeathTime));
                lastDeathTime = dead.Time;
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
            List<AbstractCastEvent> casting = GetCastLogs(log, 0, log.FightData.FightEnd);
            int swapped = -1;
            long swappedTime = 0;
            var swaps = casting.OfType<WeaponSwapEvent>().Select(x =>
            {
                return x.SwappedTo;
            }).ToList();
            foreach (AbstractCastEvent cl in casting)
            {
                if (cl.Duration == 0 && cl.SkillId != SkillItem.WeaponSwapId)
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

        public override AbstractSingleActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return new PlayerSerializable(this, log, map, CombatReplay);
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
            CombatReplay.PollingRate(log.FightData.FightEnd, true);
        }
    }
}
