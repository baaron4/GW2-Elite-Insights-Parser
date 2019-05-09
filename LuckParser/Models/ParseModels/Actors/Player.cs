using LuckParser.Controllers;
using LuckParser.Models.Logic;
using LuckParser.Exceptions;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;
using static LuckParser.Models.Statistics;

namespace LuckParser.Models.ParseModels
{
    public class Player : AbstractMasterActor
    {
        // Fields
        public readonly string Account;
        public readonly int Group;
       
        private List<Consumable> _consumeList;
        private List<DeathRecap> _deathRecaps;
        private Dictionary<string, List<DamageModifierData>> _damageModifiers;
        private HashSet<string> _presentDamageModifiers;
        private Dictionary<Target, Dictionary<string, List<DamageModifierData>>> _damageModifiersTargets; 
        // statistics
        private Dictionary<Target, List<FinalDPS>> _dpsTarget;
        private Dictionary<Target, List<FinalStats>> _statsTarget;
        private List<FinalStatsAll> _statsAll;
        private List<FinalDefenses> _defenses;
        private List<FinalSupport> _support;
        private List<Dictionary<long, FinalBuffs>> _selfBuffs;
        private List<Dictionary<long, FinalBuffs>> _groupBuffs;
        private List<Dictionary<long, FinalBuffs>> _offGroupBuffs;
        private List<Dictionary<long, FinalBuffs>> _squadBuffs;
        //weaponslist
        private string[] _weaponsArray;

        // Constructors
        public Player(AgentItem agent, bool noSquad) : base(agent)
        {
            string[] name = agent.Name.Split('\0');
            if (name.Length < 2)
            {
                throw new InvalidPlayerException(false);
            }
            if (name[1].Length == 0 || name[2].Length == 0 || Character.Contains("-"))
            {
                throw new InvalidPlayerException(name[1].Length != 0 && name[2].Length == 0);
            }
            Account = name[1].TrimStart(':');
            Group = noSquad ? 1 : int.Parse(name[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
            IsFakeActor = Account == "Conjured Sword";
        }
        
        // Public methods
        public int[] GetCleanses(ParsedLog log, int phaseIndex) {
            int[] cleanse = { 0, 0 };
            foreach (Player p in log.PlayerList)
            {
                foreach(List<long> list in p.GetCondiCleanse(log,phaseIndex, AgentItem).Values)
                {
                    cleanse[0] += list.Count;
                    cleanse[1] += (int)list.Sum();
                }
            }
            return cleanse;
        }
        public int[] GetReses(ParsedLog log, long start, long end)
        {
            List<CastLog> cls = GetCastLogs(log, start, end);
            int[] reses = { 0, 0 };
            foreach (CastLog cl in cls) {
                if (cl.SkillId == SkillItem.ResurrectId)
                {
                    reses[0]++;
                    reses[1] += cl.ActualDuration;
                }
            }
            return reses;
        }

        public FinalDPS GetDPSTarget(ParsedLog log, int phaseIndex, Target target)
        {
            if (_dpsTarget == null)
            {
                _dpsTarget = new Dictionary<Target, List<FinalDPS>>();
                foreach (Target tar in log.FightData.Logic.Targets)
                {
                    _dpsTarget[tar] = new List<FinalDPS>();
                    foreach (PhaseData phase in log.FightData.GetPhases(log))
                    {
                        _dpsTarget[tar].Add(GetFinalDPS(log, phase, tar));
                    }
                }
            }
            if (target == null)
            {
                return GetDPSAll(log, phaseIndex);
            }
            return _dpsTarget[target][phaseIndex];
        }

        public List<FinalDPS> GetDPSTarget(ParsedLog log, Target target)
        {
            if (_dpsTarget == null)
            {
                _dpsTarget = new Dictionary<Target, List<FinalDPS>>();
                foreach (Target tar in log.FightData.Logic.Targets)
                {
                    _dpsTarget[tar] = new List<FinalDPS>();
                    foreach (PhaseData phase in log.FightData.GetPhases(log))
                    {
                        _dpsTarget[tar].Add(GetFinalDPS(log, phase, tar));
                    }
                }
            }
            if (target == null)
            {
                return GetDPSAll(log);
            }
            return _dpsTarget[target];
        }

        public FinalStatsAll GetStatsAll(ParsedLog log, int phaseIndex)
        {
            if (_statsAll == null)
            {
                SetStats(log);
            }
            return _statsAll[phaseIndex];
        }

        public FinalStats GetStatsTarget(ParsedLog log, int phaseIndex, Target target)
        {
            if (_statsTarget == null)
            {
                SetStats(log);
            }
            if (target == null)
            {
                return GetStatsAll(log, phaseIndex);
            }
            return _statsTarget[target][phaseIndex];
        }

        public List<FinalStatsAll> GetStatsAll(ParsedLog log)
        {
            if (_statsAll == null)
            {
                SetStats(log);
            }
            return _statsAll;
        }

        public List<FinalStats> GetStatsTarget(ParsedLog log, Target target)
        {
            if (_statsTarget == null)
            {
                SetStats(log);
            }
            if (target == null)
            {
                return new List<FinalStats>(GetStatsAll(log));
            }
            return _statsTarget[target];
        }

        private void FillFinalStats(ParsedLog log, List<DamageLog> dls, FinalStats final, Dictionary<Target, FinalStats> targetsFinal)
        {
            HashSet<long> nonCritable = new HashSet<long>
                    {
                        9292,
                        5492,
                        13014,
                        30770,
                        52370
                    };
            // (x - 1) / x
            foreach (DamageLog dl in dls)
            {
                if (!dl.IsIndirectDamage)
                {
                    foreach (var pair in targetsFinal)
                    {
                        Target target = pair.Key;
                        if (dl.DstInstId == target.InstID && dl.Time <= log.FightData.ToFightSpace(target.LastAware) && dl.Time >= log.FightData.ToFightSpace(target.FirstAware))
                        {
                            FinalStats targetFinal = pair.Value;
                            if (dl.Result == ParseEnum.Result.Crit)
                            {
                                targetFinal.CriticalRate++;
                                targetFinal.CriticalDmg += dl.Damage;
                            }

                            if (dl.IsFlanking)
                            {
                                targetFinal.FlankingRate++;
                            }

                            if (dl.Result == ParseEnum.Result.Glance)
                            {
                                targetFinal.GlanceRate++;
                            }

                            if (dl.Result == ParseEnum.Result.Blind)
                            {
                                targetFinal.Missed++;
                            }
                            if (dl.Result == ParseEnum.Result.Interrupt)
                            {
                                targetFinal.Interrupts++;
                            }

                            if (dl.Result == ParseEnum.Result.Absorb)
                            {
                                targetFinal.Invulned++;
                            }
                            targetFinal.DirectDamageCount++;
                            if (!nonCritable.Contains(dl.SkillId))
                            {
                                targetFinal.CritableDirectDamageCount++;
                            }
                        }
                    }
                    if (dl.Result == ParseEnum.Result.Crit)
                    {
                        final.CriticalRate++;
                        final.CriticalDmg += dl.Damage;
                    }

                    if (dl.IsFlanking)
                    {
                        final.FlankingRate++;
                    }

                    if (dl.Result == ParseEnum.Result.Glance)
                    {
                        final.GlanceRate++;
                    }

                    if (dl.Result == ParseEnum.Result.Blind)
                    {
                        final.Missed++;
                    }
                    if (dl.Result == ParseEnum.Result.Interrupt)
                    {
                        final.Interrupts++;
                    }

                    if (dl.Result == ParseEnum.Result.Absorb)
                    {
                        final.Invulned++;
                    }
                    final.DirectDamageCount++;
                    if (!nonCritable.Contains(dl.SkillId))
                    {
                        final.CritableDirectDamageCount++;
                    }
                }
            }
        }

        private void SetStats(ParsedLog log)
        {
            int phaseIndex = -1;
            _statsAll = new List<FinalStatsAll>();
            _statsTarget = new Dictionary<Target, List<FinalStats>>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                phaseIndex++;
                Dictionary<Target, FinalStats> targetDict = new Dictionary<Target, FinalStats>();
                foreach (Target target in log.FightData.Logic.Targets)
                {
                    if (!_statsTarget.ContainsKey(target))
                    {
                        _statsTarget[target] = new List<FinalStats>();
                    }
                    _statsTarget[target].Add(new FinalStats());
                    targetDict[target] = _statsTarget[target].Last();
                }
                FinalStatsAll final = new FinalStatsAll();
                FillFinalStats(log, GetJustPlayerDamageLogs(null, log, phase), final, targetDict);
                _statsAll.Add(final);
                // If conjured sword, stop
                if (IsFakeActor)
                {
                    continue;
                }
                foreach (CastLog cl in GetCastLogs(log, phase.Start, phase.End))
                {
                    if (cl.EndActivation == ParseEnum.Activation.CancelCancel)
                    {
                        final.Wasted++;
                        final.TimeWasted += cl.ActualDuration;
                    }
                    if (cl.EndActivation == ParseEnum.Activation.CancelFire)
                    {
                        if (cl.ActualDuration < cl.ExpectedDuration)
                        {
                            final.Saved++;
                            final.TimeSaved += cl.ExpectedDuration - cl.ActualDuration;
                        }
                    }
                    if (cl.SkillId == SkillItem.WeaponSwapId)
                    {
                        final.SwapCount++;
                    }
                }
                final.TimeSaved = Math.Round(final.TimeSaved / 1000.0, GeneralHelper.TimeDigit);
                final.TimeWasted = Math.Round(final.TimeWasted / 1000.0, GeneralHelper.TimeDigit);

                double avgBoons = 0;
                foreach (long duration in GetBuffPresence(log, phaseIndex).Where(x => log.Boons.BoonsByIds[x.Key].Nature == Boon.BoonNature.Boon).Select(x => x.Value))
                {
                    avgBoons += duration;
                }
                final.AvgBoons = Math.Round(avgBoons / phase.DurationInMS, GeneralHelper.BoonDigit);

                double avgCondis = 0;
                foreach (long duration in GetBuffPresence(log, phaseIndex).Where(x => log.Boons.BoonsByIds[x.Key].Nature == Boon.BoonNature.Condition).Select(x => x.Value))
                {
                    avgCondis += duration;
                }
                final.AvgConditions = Math.Round(avgCondis / phase.DurationInMS, GeneralHelper.BoonDigit);

                if (Properties.Settings.Default.ParseCombatReplay && log.CanCombatReplay)
                {
                    List<Point3D> positions = CombatReplay.Positions.Where(x => x.Time >= phase.Start && x.Time <= phase.End).ToList();
                    List<Point3D> stackCenterPositions = log.Statistics.GetStackCenterPositions(log);
                    int offset = CombatReplay.Positions.Count(x => x.Time < phase.Start);
                    if (positions.Count > 1)
                    {
                        List<float> distances = new List<float>();
                        for (int time = 0; time < positions.Count; time++)
                        {

                            float deltaX = positions[time].X - stackCenterPositions[time + offset].X;
                            float deltaY = positions[time].Y - stackCenterPositions[time + offset].Y;
                            //float deltaZ = positions[time].Z - StackCenterPositions[time].Z;


                            distances.Add((float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
                        }
                        final.StackDist = distances.Sum() / distances.Count;
                    }
                    else
                    {
                        final.StackDist = -1;
                    }
                }
            }
        }

        public FinalDefenses GetDefenses(ParsedLog log, int phaseIndex)
        {
            if (_defenses == null)
            {
                SetDefenses(log);
            }
            return _defenses[phaseIndex];
        }

        public List<FinalDefenses> GetDefenses(ParsedLog log)
        {
            if (_defenses == null)
            {
                SetDefenses(log);
            }
            return _defenses;
        }

        private void SetDefenses(ParsedLog log)
        {
            List<(long start, long end)> dead = new List<(long start, long end)>();
            List<(long start, long end)> down = new List<(long start, long end)>();
            List<(long start, long end)> dc = new List<(long start, long end)>();
            log.CombatData.GetAgentStatus(FirstAware, LastAware, InstID, dead, down, dc, log.FightData.FightStart, log.FightData.FightEnd);
            _defenses = new List<FinalDefenses>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                FinalDefenses final = new FinalDefenses();
                _defenses.Add(final);
                long start = log.FightData.ToLogSpace(phase.Start);
                long end = log.FightData.ToLogSpace(phase.End);
                List<DamageLog> damageLogs = GetDamageTakenLogs(null, log, phase.Start, phase.End);
                //List<DamageLog> healingLogs = player.getHealingReceivedLogs(log, phase.getStart(), phase.getEnd());

                final.DamageTaken = damageLogs.Sum(x => (long)x.Damage);
                //final.allHealReceived = healingLogs.Sum(x => x.getDamage());
                final.BlockedCount = damageLogs.Count(x => x.Result == ParseEnum.Result.Block);
                final.InvulnedCount = 0;
                final.DamageInvulned = 0;
                final.EvadedCount = damageLogs.Count(x => x.Result == ParseEnum.Result.Evade);
                final.DodgeCount = GetCastLogs(log, 0, log.FightData.FightDuration).Count(x => x.SkillId == SkillItem.DodgeId);
                final.DamageBarrier = damageLogs.Sum(x => x.ShieldDamage);
                final.InterruptedCount = damageLogs.Count(x => x.Result == ParseEnum.Result.Interrupt);
                foreach (DamageLog dl in damageLogs.Where(x => x.Result == ParseEnum.Result.Absorb))
                {
                    final.InvulnedCount++;
                    final.DamageInvulned += dl.Damage;
                }
                final.DownCount = log.MechanicData.GetMechanicLogs(SkillItem.DownId).Count(x => x.Actor == this && x.Time >= start && x.Time <= end);
                final.DeadCount = log.MechanicData.GetMechanicLogs(SkillItem.DeathId).Count(x => x.Actor == this && x.Time >= start && x.Time <= end);
                final.DcCount = log.MechanicData.GetMechanicLogs(SkillItem.DCId).Count(x => x.Actor == this && x.Time >= start && x.Time <= end);

                //
                start = phase.Start;
                end = phase.End;
                final.DownDuration = (int)down.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
                final.DeadDuration = (int)dead.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
                final.DcDuration = (int)dc.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
            }
        }

        public FinalSupport GetSupport(ParsedLog log, int phaseIndex)
        {
            if (_support == null)
            {
                SetSupport(log);
            }
            return _support[phaseIndex];
        }

        public List<FinalSupport> GetSupport(ParsedLog log)
        {
            if (_support == null)
            {
                SetSupport(log);
            }
            return _support;
        }

        private void SetSupport(ParsedLog log)
        {
            _support = new List<FinalSupport>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                FinalSupport final = new FinalSupport();
                _support.Add(final);
                PhaseData phase = phases[phaseIndex];

                int[] resArray = GetReses(log, phase.Start, phase.End);
                int[] cleanseArray = GetCleanses(log, phaseIndex);
                //List<DamageLog> healingLogs = player.getHealingLogs(log, phase.getStart(), phase.getEnd());
                //final.allHeal = healingLogs.Sum(x => x.getDamage());
                final.Resurrects = resArray[0];
                final.ResurrectTime = resArray[1] / 1000.0;
                final.CondiCleanse = cleanseArray[0];
                final.CondiCleanseTime = cleanseArray[1] / 1000.0;
            }
        }

        public Dictionary<long, FinalBuffs> GetBuffs(ParsedLog log, int phaseIndex, BuffEnum type)
        {
            if (_selfBuffs == null)
            {
                SetBuffs(log);
            }
            switch(type)
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

        public List<Dictionary<long, FinalBuffs>> GetBuffs(ParsedLog log, BuffEnum type)
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

        private List<Dictionary<long, FinalBuffs>> GetBoonsForPlayers(List<Player> playerList, ParsedLog log)
        {
            List<Dictionary<long, FinalBuffs>> uptimesByPhase = new List<Dictionary<long, FinalBuffs>>();

            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                PhaseData phase = phases[phaseIndex];
                long fightDuration = phase.End - phase.Start;

                Dictionary<Player, BoonDistribution> boonDistributions = new Dictionary<Player, BoonDistribution>();
                foreach (Player p in playerList)
                {
                    boonDistributions[p] = p.GetBoonDistribution(log, phaseIndex);
                }

                HashSet<Boon> boonsToTrack = new HashSet<Boon>(boonDistributions.SelectMany(x => x.Value).Select(x => log.Boons.BoonsByIds[x.Key]));

                Dictionary<long, FinalBuffs> final =
                    new Dictionary<long, FinalBuffs>();

                foreach (Boon boon in boonsToTrack)
                {
                    long totalGeneration = 0;
                    long totalOverstack = 0;
                    long totalWasted = 0;
                    long totalUnknownExtension = 0;
                    long totalExtension = 0;
                    long totalExtended = 0;
                    bool hasGeneration = false;
                    foreach (BoonDistribution boons in boonDistributions.Values)
                    {
                        if (boons.ContainsKey(boon.ID))
                        {
                            hasGeneration = hasGeneration || boons.HasSrc(boon.ID, AgentItem);
                            totalGeneration += boons.GetGeneration(boon.ID, AgentItem);
                            totalOverstack += boons.GetOverstack(boon.ID, AgentItem);
                            totalWasted += boons.GetWaste(boon.ID, AgentItem);
                            totalUnknownExtension += boons.GetUnknownExtension(boon.ID, AgentItem);
                            totalExtension += boons.GetExtension(boon.ID, AgentItem);
                            totalExtended += boons.GetExtended(boon.ID, AgentItem);
                        }
                    }

                    if (hasGeneration)
                    {
                        FinalBuffs uptime = new FinalBuffs();
                        final[boon.ID] = uptime;
                        if (boon.Type == Boon.BoonType.Duration)
                        {
                            uptime.Generation = Math.Round(100.0 * totalGeneration / fightDuration / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Overstack = Math.Round(100.0 * (totalOverstack + totalGeneration) / fightDuration / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Wasted = Math.Round(100.0 * (totalWasted) / fightDuration / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.UnknownExtended = Math.Round(100.0 * (totalUnknownExtension) / fightDuration / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.ByExtension = Math.Round(100.0 * (totalExtension) / fightDuration / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Extended = Math.Round(100.0 * (totalExtended) / fightDuration / playerList.Count, GeneralHelper.BoonDigit);
                        }
                        else if (boon.Type == Boon.BoonType.Intensity)
                        {
                            uptime.Generation = Math.Round((double)totalGeneration / fightDuration / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Overstack = Math.Round((double)(totalOverstack + totalGeneration) / fightDuration / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Wasted = Math.Round((double)(totalWasted) / fightDuration / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.UnknownExtended = Math.Round((double)(totalUnknownExtension) / fightDuration / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.ByExtension = Math.Round((double)(totalExtension) / fightDuration / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Extended = Math.Round((double)(totalExtended) / fightDuration / playerList.Count, GeneralHelper.BoonDigit);
                        }
                    }
                }

                uptimesByPhase.Add(final);
            }

            return uptimesByPhase;
        }

        private void SetBuffs(ParsedLog log)
        {
            // Boons applied to self
            _selfBuffs = new List<Dictionary<long, FinalBuffs>>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                Dictionary<long, FinalBuffs> final = new Dictionary<long, FinalBuffs>();

                PhaseData phase = phases[phaseIndex];

                BoonDistribution selfBoons = GetBoonDistribution(log, phaseIndex);
                Dictionary<long, long> buffPresence = GetBuffPresence(log, phaseIndex);

                long fightDuration = phase.End - phase.Start;
                foreach (Boon boon in TrackedBoons)
                {
                    if (selfBoons.ContainsKey(boon.ID))
                    {
                        FinalBuffs uptime = new FinalBuffs
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
                        long generation = selfBoons.GetGeneration(boon.ID, AgentItem);
                        if (boon.Type == Boon.BoonType.Duration)
                        {
                            uptime.Uptime = Math.Round(100.0 * selfBoons.GetUptime(boon.ID) / fightDuration, GeneralHelper.BoonDigit);
                            uptime.Generation = Math.Round(100.0 * generation / fightDuration, GeneralHelper.BoonDigit);
                            uptime.Overstack = Math.Round(100.0 * (selfBoons.GetOverstack(boon.ID, AgentItem) + generation) / fightDuration, GeneralHelper.BoonDigit);
                            uptime.Wasted = Math.Round(100.0 * selfBoons.GetWaste(boon.ID, AgentItem) / fightDuration, GeneralHelper.BoonDigit);
                            uptime.UnknownExtended = Math.Round(100.0 * selfBoons.GetUnknownExtension(boon.ID, AgentItem) / fightDuration, GeneralHelper.BoonDigit);
                            uptime.ByExtension = Math.Round(100.0 * selfBoons.GetExtension(boon.ID, AgentItem) / fightDuration, GeneralHelper.BoonDigit);
                            uptime.Extended = Math.Round(100.0 * selfBoons.GetExtended(boon.ID, AgentItem) / fightDuration, GeneralHelper.BoonDigit);
                        }
                        else if (boon.Type == Boon.BoonType.Intensity)
                        {
                            uptime.Uptime = Math.Round((double)selfBoons.GetUptime(boon.ID) / fightDuration, GeneralHelper.BoonDigit);
                            uptime.Generation = Math.Round((double)generation / fightDuration, GeneralHelper.BoonDigit);
                            uptime.Overstack = Math.Round((double)(selfBoons.GetOverstack(boon.ID, AgentItem) + generation) / fightDuration, GeneralHelper.BoonDigit);
                            uptime.Wasted = Math.Round((double)selfBoons.GetWaste(boon.ID, AgentItem) / fightDuration, GeneralHelper.BoonDigit);
                            uptime.UnknownExtended = Math.Round((double)selfBoons.GetUnknownExtension(boon.ID, AgentItem) / fightDuration, GeneralHelper.BoonDigit);
                            uptime.ByExtension = Math.Round((double)selfBoons.GetExtension(boon.ID, AgentItem) / fightDuration, GeneralHelper.BoonDigit);
                            uptime.Extended = Math.Round((double)selfBoons.GetExtended(boon.ID, AgentItem) / fightDuration, GeneralHelper.BoonDigit);
                            if (buffPresence.TryGetValue(boon.ID, out long presenceValueBoon))
                            {
                                uptime.Presence = Math.Round(100.0 * presenceValueBoon / fightDuration, GeneralHelper.BoonDigit);
                            }
                        }
                    }
                }

                _selfBuffs.Add(final);
            }

            // Boons applied to player's group
            var otherPlayersInGroup = log.PlayerList
                .Where(p => p.Group == Group && InstID != p.InstID)
                .ToList();
            _groupBuffs = GetBoonsForPlayers(otherPlayersInGroup, log);

            // Boons applied to other groups
            var offGroupPlayers = log.PlayerList.Where(p => p.Group != Group).ToList();
            _offGroupBuffs = GetBoonsForPlayers(offGroupPlayers, log);

            // Boons applied to squad
            var otherPlayers = log.PlayerList.Where(p => p.InstID != InstID).ToList();
            _squadBuffs = GetBoonsForPlayers(otherPlayers, log);
        }

        public List<DeathRecap> GetDeathRecaps(ParsedLog log)
        {
            if(_deathRecaps == null)
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
                EstimateWeapons( log);
            }
            return _weaponsArray;
        }

        public List<Consumable> GetConsumablesList(ParsedLog log, long start, long end)
        {
            if (_consumeList == null)
            {
                SetConsumablesList(log);
            }
            return _consumeList.Where(x => x.Time >= start && x.Time <= end).ToList() ;
        }

        public Dictionary<string, List<DamageModifierData>> GetDamageModifierData(ParsedLog log, Target target)
        {
            if (_damageModifiers == null)
            {
                SetDamageModifiersData(log);
            }
            if (target != null)
            {
                if (_damageModifiersTargets.TryGetValue(target, out var res))
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
            _damageModifiersTargets = new Dictionary<Target, Dictionary<string, List<DamageModifierData>>>();
            _presentDamageModifiers = new HashSet<string>();
            // If conjured sword or WvW, stop
            if (IsFakeActor || log.FightData.Logic.Mode == FightLogic.ParseMode.WvW)
            {
                return;
            }
            List<DamageModifier> damageMods = new List<DamageModifier>(log.DamageModifiers.DamageModifiersPerSource[DamageModifier.ModifierSource.ItemBuff]);
            damageMods.AddRange(log.DamageModifiers.DamageModifiersPerSource[DamageModifier.ModifierSource.CommonBuff]);
            damageMods.AddRange(log.DamageModifiers.GetModifiersPerProf(Prof));
            foreach (DamageModifier mod in damageMods)
            {
                mod.ComputeDamageModifier(_damageModifiers, _damageModifiersTargets, this, log);
            }
            _presentDamageModifiers.UnionWith(_damageModifiers.Keys);
            foreach (Target tar in _damageModifiersTargets.Keys)
            {
                _presentDamageModifiers.UnionWith(_damageModifiersTargets[tar].Keys);
            }
        }

        private void SetDeathRecaps(ParsedLog log)
        {
            _deathRecaps = new List<DeathRecap>();
            List<DeathRecap> res = _deathRecaps;
            List<CombatItem> deads = log.CombatData.GetStatesData(InstID, ParseEnum.StateChange.ChangeDead, log.FightData.FightStart, log.FightData.FightEnd);
            List<CombatItem> downs = log.CombatData.GetStatesData(InstID, ParseEnum.StateChange.ChangeDown, log.FightData.FightStart, log.FightData.FightEnd);
            long lastTime = log.FightData.FightStart;
            List<DamageLog> damageLogs = GetDamageTakenLogs(null, log, 0, log.FightData.FightDuration);
            foreach (CombatItem dead in deads)
            {
                DeathRecap recap = new DeathRecap()
                {
                    DeathTime = (int)(log.FightData.ToFightSpace(dead.Time))
                };
                CombatItem downed = downs.LastOrDefault(x => x.Time <= dead.Time && x.Time >= lastTime);
                if (downed != null)
                {
                    List<DamageLog> damageToDown = damageLogs.Where(x => x.Time < log.FightData.ToFightSpace(downed.Time) && x.Damage > 0 && x.Time > log.FightData.ToFightSpace(lastTime)).ToList();
                    recap.ToDown = damageToDown.Count > 0 ? new List<DeathRecap.DeathRecapDamageItem>() : null;
                    int damage = 0;
                    for (int i = damageToDown.Count - 1; i >= 0; i--)
                    {
                        DamageLog dl = damageToDown[i];
                        AgentItem ag = log.AgentData.GetAgentByInstID(dl.SrcInstId, log.FightData.ToLogSpace(dl.Time));
                        DeathRecap.DeathRecapDamageItem item = new DeathRecap.DeathRecapDamageItem()
                        {
                            Time = (int)dl.Time,
                            IndirectDamage = dl.IsIndirectDamage,
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
                    List<DamageLog> damageToKill = damageLogs.Where(x => x.Time > log.FightData.ToFightSpace(downed.Time) && x.Time < log.FightData.ToFightSpace(dead.Time) && x.Damage > 0 && x.Time > log.FightData.ToFightSpace(lastTime)).ToList();
                    recap.ToKill = damageToKill.Count > 0 ? new List<DeathRecap.DeathRecapDamageItem>() : null;
                    for (int i = damageToKill.Count - 1; i >= 0; i--)
                    {
                        DamageLog dl = damageToKill[i];
                        AgentItem ag = log.AgentData.GetAgentByInstID(dl.SrcInstId, log.FightData.ToLogSpace(dl.Time));
                        DeathRecap.DeathRecapDamageItem item = new DeathRecap.DeathRecapDamageItem()
                        {
                            Time = (int)dl.Time,
                            IndirectDamage = dl.IsIndirectDamage,
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
                    List<DamageLog> damageToKill = damageLogs.Where(x => x.Time < log.FightData.ToFightSpace(dead.Time) && x.Damage > 0 && x.Time > log.FightData.ToFightSpace(lastTime)).ToList();
                    recap.ToKill = damageToKill.Count > 0 ? new List<DeathRecap.DeathRecapDamageItem>() : null;
                    int damage = 0;
                    for (int i = damageToKill.Count - 1; i >= 0; i--)
                    {
                        DamageLog dl = damageToKill[i];
                        AgentItem ag = log.AgentData.GetAgentByInstID(dl.SrcInstId, log.FightData.ToLogSpace(dl.Time));
                        DeathRecap.DeathRecapDamageItem item = new DeathRecap.DeathRecapDamageItem()
                        {
                            Time = (int)dl.Time,
                            IndirectDamage = dl.IsIndirectDamage,
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
                lastTime = dead.Time;
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
            SkillData skillList = log.SkillData;
            List<CastLog> casting = GetCastLogs(log, 0, log.FightData.FightDuration);      
            int swapped = -1;
            long swappedTime = 0;
            List<int> swaps = casting.Where(x => x.SkillId == SkillItem.WeaponSwapId).Select(x => x.ExpectedDuration).ToList();
            foreach (CastLog cl in casting)
            {
                if (cl.ActualDuration == 0 && cl.SkillId != SkillItem.WeaponSwapId)
                {
                    continue;
                }
                SkillItem skill = skillList.Get(cl.SkillId);
                // first iteration
                if (swapped == -1)
                {
                    swapped = skill.FindWeaponSlot(swaps);
                }
                if (!skill.EstimateWeapons(weapons, swapped, cl.Time > swappedTime) && cl.SkillId == SkillItem.WeaponSwapId)
                {
                    //wepswap  
                    swapped = cl.ExpectedDuration;
                    swappedTime = cl.Time;
                }
            }
            _weaponsArray = weapons;
        }    
        
        private void SetConsumablesList(ParsedLog log)
        {
            List<Boon> consumableList = log.Boons.GetConsumableList();
            _consumeList = new List<Consumable>();
            long fightDuration = log.FightData.FightDuration;
            foreach (Boon consumable in consumableList)
            {
                foreach (CombatItem c in log.CombatData.GetBoonData(consumable.ID))
                {
                    if (c.IsBuffRemove != ParseEnum.BuffRemove.None || (c.IsBuff != 18 && c.IsBuff != 1) || AgentItem.InstID != c.DstInstid)
                    {
                        continue;
                    }
                    long time = 0;
                    if (c.IsBuff != 18)
                    {
                        time = log.FightData.ToFightSpace(c.Time);
                    }
                    if (time <= fightDuration)
                    {
                        Consumable existing = _consumeList.Find(x => x.Time == time && x.Buff.ID == consumable.ID);
                        if (existing != null)
                        {
                            existing.Stack++;
                        } else
                        {
                            _consumeList.Add(new Consumable(consumable, time, c.Value));
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
                CombatReplay.Actors.Add(new FacingActor(((int)CombatReplay.TimeOffsets.start, (int)CombatReplay.TimeOffsets.end), new AgentConnector(this), CombatReplay.PolledRotations));
            }
        }

        //
        private class PlayerSerializable : AbstractMasterActorSerializable
        {
            public int Group { get; set; }
            public long[] Dead { get; set; }
            public long[] Down { get; set; }
            public long[] Dc { get; set; }
        }

        public override AbstractMasterActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            PlayerSerializable aux = new PlayerSerializable
            {
                Group = Group,
                Img = CombatReplay.Icon,
                Type = "Player",
                ID = GetCombatReplayID(log),
                Positions = new double[2 * CombatReplay.Positions.Count],
                Dead = new long[2 * CombatReplay.Deads.Count],
                Down = new long[2 * CombatReplay.Downs.Count],
                Dc = new long[2 * CombatReplay.DCs.Count]
            };
            int i = 0;
            foreach (Point3D pos in CombatReplay.Positions)
            {
                (double x, double y) = map.GetMapCoord(pos.X, pos.Y);
                aux.Positions[i++] = x;
                aux.Positions[i++] = y;
            }
            i = 0;
            foreach ((long start, long end) in CombatReplay.Deads)
            {
                aux.Dead[i++] = start;
                aux.Dead[i++] = end;
            }
            i = 0;
            foreach ((long start, long end) in CombatReplay.Downs)
            {
                aux.Down[i++] = start;
                aux.Down[i++] = end;
            }
            i = 0;
            foreach ((long start, long end) in CombatReplay.DCs)
            {
                aux.Dc[i++] = start;
                aux.Dc[i++] = end;
            }

            return aux;
        }

        protected override void InitCombatReplay(ParsedLog log)
        {
            if (!log.CanCombatReplay || IsFakeActor)
            {
                // no combat replay support on fight
                return;
            }
            CombatReplay = new CombatReplay
            {
                Icon = GeneralHelper.GetProfIcon(Prof)
            };
            SetMovements(log);
            // Down and deads
            List<(long, long)> dead = CombatReplay.Deads;
            List<(long, long)> down = CombatReplay.Downs;
            List<(long, long)> dc = CombatReplay.DCs;
            log.CombatData.GetAgentStatus(FirstAware, LastAware, InstID, dead, down, dc, log.FightData.FightStart, log.FightData.FightEnd);
            CombatReplay.PollingRate(log.FightData.FightDuration, true);
        }


        /*protected override void setHealingLogs(ParsedLog log)
        {
            long time_start = log.getBossData().getFirstAware();
            foreach (CombatItem c in log.getHealingData())
            {
                if (agent.InstID == c.getSrcInstid() && c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware())//selecting player or minion as caster
                {
                    long time = c.getTime() - time_start;
                    addHealingLog(time, c);
                }
            }
            Dictionary<string, Minions> min_list = getMinions(log);
            foreach (Minions mins in min_list.Values)
            {
                healing_logs.AddRange(mins.getHealingLogs(log, 0, log.getBossData().getAwareDuration()));
            }
            healing_logs.Sort((x, y) => x.getTime() < y.getTime() ? -1 : 1);
        }

        protected override void setHealingReceivedLogs(ParsedLog log)
        {
            long time_start = log.getBossData().getFirstAware();
            foreach (CombatItem c in log.getHealingReceivedData())
            {
                if (agent.InstID == c.getDstInstid() && c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware())
                {//selecting player as target
                    long time = c.getTime() - time_start;
                    addHealingReceivedLog(time, c);
                }
            }
        }*/
    }
}
