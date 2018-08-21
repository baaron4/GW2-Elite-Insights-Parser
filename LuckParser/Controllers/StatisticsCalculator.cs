using System;
using System.Collections.Generic;
using System.Linq;
using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;

namespace LuckParser.Controllers
{
    /// <summary>
    /// Calculates statistical information from a log
    /// </summary>
    class StatisticsCalculator
    {
        public class Switches
        {
            public bool calculateDPS = false;
            public bool calculateStats = false;
            public bool calculateDefense = false;
            public bool calculateSupport = false;
            public bool calculateBoons = false;
            public bool calculateConditions = false;
            public bool calculateCombatReplay = false;
            public bool calculateMechanics = false;
        }

        private SettingsContainer settings;

        private Statistics statistics;

        private ParsedLog log;

        public StatisticsCalculator(SettingsContainer settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Calculate a statistic from a log
        /// </summary>
        /// <param name="log">log to calculate stats from</param>
        /// <returns></returns>
        public Statistics CalculateStatistics(ParsedLog log, Switches switches)
        {
            statistics = new Statistics();

            this.log = log;

            statistics.phases = log.GetBoss().GetPhases(log, settings.ParsePhases);
            if (switches.calculateCombatReplay && settings.ParseCombatReplay)
            {
                foreach (Player p in log.GetPlayerList())
                {
                    p.InitCombatReplay(log, settings.PollingRate, false, true);
                }
                log.GetBoss().InitCombatReplay(log, settings.PollingRate, false, true);
            }
            if (switches.calculateDPS) CalculateDPS();
            if (switches.calculateStats) CalculateStats();
            if (switches.calculateDefense) CalculateDefenses();
            if (switches.calculateSupport) CalculateSupport();
            if (switches.calculateBoons)
            {
                SetPresentBoons();
                CalculateBoons();
            } 
                      
            if (switches.calculateConditions) CalculateConditions();
            if (switches.calculateMechanics)
            {
                log.GetBoss().AddMechanics(log);
                foreach (Player p in log.GetPlayerList())
                {
                    p.AddMechanics(log);
                }
                log.GetMechanicData().ComputePresentMechanics(log, statistics.phases);
            }

            return statistics;
        }

        private Statistics.FinalDPS GetFinalDPS(AbstractPlayer player, int phaseIndex, bool checkRedirection)
        {
            Statistics.FinalDPS final = new Statistics.FinalDPS();

            PhaseData phase = statistics.phases[phaseIndex];

            double phaseDuration = (phase.GetDuration()) / 1000.0;

            double damage = 0.0;
            double dps = 0.0;

            ////////// ALL
            //DPS
            damage = player.GetDamageLogs(0, log, phase.GetStart(),
                    phase.GetEnd())
                .Sum(x => x.GetDamage());
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.allDps = (int)dps;
            final.allDamage = (int)damage;
            //Condi DPS
            damage = player.GetDamageLogs(0, log, phase.GetStart(),
                    phase.GetEnd())
                .Where(x => x.IsCondi() > 0).Sum(x => x.GetDamage());
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.allCondiDps = (int)dps;
            final.allCondiDamage = (int)damage;
            //Power DPS
            damage = final.allDamage - final.allCondiDamage;
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.allPowerDps = (int)dps;
            final.allPowerDamage = (int)damage;
            final.playerPowerDamage = player.GetJustPlayerDamageLogs(0, log,
                phase.GetStart(), phase.GetEnd()).Where(x => x.IsCondi() == 0).Sum(x => x.GetDamage());
            /////////// BOSS
            //DPS
            if (checkRedirection && phase.GetRedirection().Count > 0)
            {
                damage = player.GetDamageLogs(phase.GetRedirection(), log,
                    phase.GetStart(), phase.GetEnd()).Sum(x => x.GetDamage());
            } else
            {
                damage = player.GetDamageLogs(log.GetBossData().GetInstid(), log,
                    phase.GetStart(), phase.GetEnd()).Sum(x => x.GetDamage());
            }
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.bossDps = (int)dps;
            final.bossDamage = (int)damage;
            //Condi DPS
            if (checkRedirection && phase.GetRedirection().Count > 0)
            {
                damage = player.GetDamageLogs(phase.GetRedirection(), log,
                    phase.GetStart(), phase.GetEnd()).Where(x => x.IsCondi() > 0).Sum(x => x.GetDamage());
            }
            else
            {
                damage = player.GetDamageLogs(log.GetBossData().GetInstid(), log,
                    phase.GetStart(), phase.GetEnd()).Where(x => x.IsCondi() > 0).Sum(x => x.GetDamage());
            }
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.bossCondiDps = (int)dps;
            final.bossCondiDamage = (int)damage;
            //Power DPS
            damage = final.bossDamage - final.bossCondiDamage;
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.bossPowerDps = (int)dps;
            final.bossPowerDamage = (int)damage;
            if (checkRedirection && phase.GetRedirection().Count > 0)
            {
                final.playerBossPowerDamage = player.GetJustPlayerDamageLogs(phase.GetRedirection(), log,
                    phase.GetStart(), phase.GetEnd()).Where(x => x.IsCondi() == 0).Sum(x => x.GetDamage());
            }
            else
            {
                final.playerBossPowerDamage = player.GetJustPlayerDamageLogs(log.GetBossData().GetInstid(), log,
                    phase.GetStart(), phase.GetEnd()).Where(x => x.IsCondi() == 0).Sum(x => x.GetDamage());
            }

            return final;
        }

        private void CalculateDPS()
        {
            foreach (Player player in log.GetPlayerList())
            {
                Statistics.FinalDPS[] phaseDps = new Statistics.FinalDPS[statistics.phases.Count];
                for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
                {
                    phaseDps[phaseIndex] = GetFinalDPS(player,phaseIndex, true);
                }

                statistics.dps[player] = phaseDps;
            }

            Statistics.FinalDPS[] phaseBossDps = new Statistics.FinalDPS[statistics.phases.Count];
            for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
            {
                phaseBossDps[phaseIndex] = GetFinalDPS(log.GetBoss(), phaseIndex, false);
            }

            statistics.bossDps = phaseBossDps;
        }

        private void CalculateStats()
        {
            foreach (Player player in log.GetPlayerList())
            {
                Statistics.FinalStats[] phaseStats = new Statistics.FinalStats[statistics.phases.Count];
                for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
                {
                    Statistics.FinalStats final = new Statistics.FinalStats();

                    PhaseData phase = statistics.phases[phaseIndex];
                    long start = phase.GetStart() + log.GetBossData().GetFirstAware();
                    long end = phase.GetEnd() + log.GetBossData().GetFirstAware();

                    List<DamageLog> damageLogs  = player.GetJustPlayerDamageLogs(0, log, phase.GetStart(), phase.GetEnd());
                    List<CastLog> castLogs = player.GetCastLogs(log, phase.GetStart(), phase.GetEnd());

                    int instid = player.GetInstid();

                    final.powerLoopCount = 0;
                    final.critablePowerLoopCount = 0;
                    final.criticalRate = 0;
                    final.criticalDmg = 0;
                    final.scholarRate = 0;
                    final.scholarDmg = 0;
                    final.movingRate = 0;
                    final.movingDamage = 0;
                    final.flankingRate = 0;
                    final.glanceRate = 0;
                    final.missed = 0;
                    final.interupts = 0;
                    final.invulned = 0;
                    final.wasted = 0;
                    final.timeWasted = 0;
                    final.saved = 0;
                    final.timeSaved = 0;
                    final.stackDist = 0;
                    
                    final.powerLoopCountBoss = 0;
                    final.critablePowerLoopCountBoss = 0;
                    final.criticalRateBoss = 0;
                    final.criticalDmgBoss = 0;
                    final.scholarRateBoss = 0;
                    final.scholarDmgBoss = 0;
                    final.movingRateBoss = 0;
                    final.movingDamageBoss = 0;
                    final.flankingRateBoss = 0;
                    final.glanceRateBoss = 0;
                    final.missedBoss = 0;
                    final.interuptsBoss = 0;
                    final.invulnedBoss = 0;

                    // Add non critable sigil/rune procs here
                    HashSet<long> nonCritable = new HashSet<long>
                    {
                        9292
                    };
                    HashSet<long> idsToCheck = new HashSet<long>();
                    if (phase.GetRedirection().Count > 0)
                    {
                        foreach (AgentItem a in phase.GetRedirection())
                        {
                            idsToCheck.Add(a.GetInstid());
                        }
                    } else
                    {
                        idsToCheck.Add(log.GetBossData().GetInstid());
                    }
                    foreach (DamageLog dl in damageLogs)
                    {
                        if (dl.IsCondi() == 0)
                        {

                            if (idsToCheck.Contains(dl.GetDstInstidt()))
                            {
                                if (idsToCheck.Count > 1)
                                {
                                    AgentItem target = phase.GetRedirection().Find(x => x.GetInstid() == dl.GetDstInstidt());
                                    if (dl.GetTime() < target.GetFirstAware() - log.GetBossData().GetFirstAware() || dl.GetTime() > target.GetLastAware() - log.GetBossData().GetFirstAware())
                                    {
                                        continue;
                                    }
                                }
                                if (dl.GetResult() == ParseEnum.Result.Crit)
                                {
                                    final.criticalRateBoss++;
                                    final.criticalDmgBoss += dl.GetDamage();
                                }

                                if (dl.IsNinety() > 0)
                                {
                                    final.scholarRateBoss++;
                                    final.scholarDmgBoss += (int)(dl.GetDamage() / 11.0); //regular+10% damage
                                }

                                if (dl.IsMoving() > 0)
                                {
                                    final.movingRateBoss++;
                                    final.movingDamageBoss += (int)(dl.GetDamage() / 21.0);
                                }
                                
                                final.flankingRateBoss += dl.IsFlanking();

                                if (dl.GetResult() == ParseEnum.Result.Glance)
                                {
                                    final.glanceRateBoss++;
                                }

                                if (dl.GetResult() == ParseEnum.Result.Blind)
                                {
                                    final.missedBoss++;
                                }

                                if (dl.GetResult() == ParseEnum.Result.Interrupt)
                                {
                                    final.interuptsBoss++;
                                }

                                if (dl.GetResult() == ParseEnum.Result.Absorb)
                                {
                                    final.invulnedBoss++;
                                }
                                final.powerLoopCountBoss++;
                                if (!nonCritable.Contains(dl.GetID()))
                                {
                                    final.critablePowerLoopCountBoss++;
                                }
                            }

                            if (dl.GetResult() == ParseEnum.Result.Crit)
                            {
                                final.criticalRate++;
                                final.criticalDmg += dl.GetDamage();
                            }

                            if (dl.IsNinety() > 0)
                            {
                                final.scholarRate++;
                                final.scholarDmg += (int)(dl.GetDamage() / 11.0); //regular+10% damage
                            }

                            if (dl.IsMoving() > 0)
                            {
                                final.movingRate++;
                                final.movingDamage += (int)(dl.GetDamage() / 21.0);
                            }
                            
                            final.flankingRate += dl.IsFlanking();

                            if (dl.GetResult() == ParseEnum.Result.Glance)
                            {
                                final.glanceRate++;
                            }

                            if (dl.GetResult() == ParseEnum.Result.Blind)
                            {
                                final.missed++;
                            }

                            if (dl.GetResult() == ParseEnum.Result.Interrupt)
                            {
                                final.interupts++;
                            }

                            if (dl.GetResult() == ParseEnum.Result.Absorb)
                            {
                                final.invulned++;
                            }
                            final.powerLoopCount++;
                            if (!nonCritable.Contains(dl.GetID()))
                            {
                                final.critablePowerLoopCount++;
                            }
                        }
                    }
                    foreach (CastLog cl in castLogs)
                    {
                        if (cl.EndActivation() == ParseEnum.Activation.CancelCancel)
                        {
                            final.wasted++;
                            final.timeWasted += cl.GetActDur();
                        }
                        if (cl.EndActivation() == ParseEnum.Activation.CancelFire)
                        {
                            final.saved++;
                            if (cl.GetActDur() < cl.GetExpDur())
                            {
                                final.timeSaved += cl.GetExpDur() - cl.GetActDur();
                            }
                        }
                    }

                    final.timeSaved = final.timeSaved / 1000f;
                    final.timeWasted = final.timeWasted / 1000f;
                    
                    final.powerLoopCount = final.powerLoopCount == 0 ? 1 : final.powerLoopCount;
                    
                    final.powerLoopCountBoss = final.powerLoopCountBoss == 0 ? 1 : final.powerLoopCountBoss;

                    // Counts
                    CombatData combatData = log.GetCombatData();
                    final.swapCount = combatData.GetStates(instid, ParseEnum.StateChange.WeaponSwap, start, end).Count;
                    final.downCount = combatData.GetStates(instid, ParseEnum.StateChange.ChangeDown, start, end).Count;
                    final.dodgeCount = combatData.GetSkillCount(instid, 65001, start, end) + combatData.GetBuffCount(instid, 40408, start, end);//dodge = 65001 mirage cloak =40408
                    final.ressCount = combatData.GetSkillCount(instid, 1066, start, end); //Res = 1066

                    //Stack Distance
                    if (settings.ParseCombatReplay && log.GetBoss().GetCombatReplay() != null)
                    {
                        if (statistics.StackCenterPositions == null)
                        {
                            statistics.StackCenterPositions = new List<Point3D>();
                            List<List<Point3D>> GroupsPosList = new List<List<Point3D>>();
                            foreach (Player p in log.GetPlayerList())
                            {
                                List<Point3D> list = p.GetCombatReplay().GetActivePositions();  
                                if (list.Count > 1)
                                {
                                    GroupsPosList.Add(list);
                                }
                            }                       
                            for (int time = 0; time < GroupsPosList[0].Count; time++)
                            {
                                float x = 0;
                                float y = 0;
                                float z = 0;
                                int active_players = GroupsPosList.Count;
                                for (int play = 0; play < GroupsPosList.Count; play++)
                                {
                                    Point3D point = GroupsPosList[play][time];
                                    if (point != null)
                                    {
                                        x += point.X;
                                        y += point.Y;
                                        z += point.Z;
                                    }
                                    else
                                    {
                                        active_players--;
                                    }
                                   
                                }
                                x = x /active_players;
                                y = y / active_players;
                                z = z / active_players;
                                statistics.StackCenterPositions.Add(new Point3D(x, y, z, time));
                            }
                        }
                        List<Point3D> positions = player.GetCombatReplay().GetPositions().Where(x => x.Time >= phase.GetStart() && x.Time <= phase.GetEnd()).ToList();
                        int offset = player.GetCombatReplay().GetPositions().Count(x => x.Time < phase.GetStart());
                        if (positions.Count > 1)
                        {
                            List<float> distances = new List<float>();
                            for (int time = 0; time < positions.Count; time++)
                            {

                                float deltaX = positions[time].X - statistics.StackCenterPositions[time + offset].X;
                                float deltaY = positions[time].Y - statistics.StackCenterPositions[time + offset].Y;
                                //float deltaZ = positions[time].Z - Statistics.StackCenterPositions[time].Z;


                                distances.Add((float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
                            }
                            final.stackDist = distances.Sum() / distances.Count;
                        }
                        else
                        {
                            final.stackDist = -1;
                        }
                    }
                    // R.I.P
                    List<CombatItem> dead = combatData.GetStates(instid, ParseEnum.StateChange.ChangeDead, start, end);
                    final.died = 0.0;
                    if (dead.Count > 0)
                    {
                        final.died = dead.Last().GetTime() - start;
                    }

                    List<CombatItem> disconect = combatData.GetStates(instid, ParseEnum.StateChange.Despawn, start, end);
                    final.dcd = 0.0;
                    if (disconect.Count > 0)
                    {
                        final.dcd = disconect.Last().GetTime() - start;
                    }

                    phaseStats[phaseIndex] = final;
                }
                statistics.stats[player] = phaseStats;
            }
        }

        private void CalculateDefenses()
        {
            foreach (Player player in log.GetPlayerList())
            {
                Statistics.FinalDefenses[] phaseDefense = new Statistics.FinalDefenses[statistics.phases.Count];
                for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
                {
                    Statistics.FinalDefenses final = new Statistics.FinalDefenses();

                    PhaseData phase =statistics.phases[phaseIndex];
                    long start = phase.GetStart() + log.GetBossData().GetFirstAware();
                    long end = phase.GetEnd() + log.GetBossData().GetFirstAware();

                    List<DamageLog> damageLogs = player.GetDamageTakenLogs(log, phase.GetStart(), phase.GetEnd());
                    //List<DamageLog> healingLogs = player.getHealingReceivedLogs(log, phase.getStart(), phase.getEnd());

                    int instID = player.GetInstid();
                 
                    final.damageTaken = damageLogs.Sum(x => (long)x.GetDamage());
                    //final.allHealReceived = healingLogs.Sum(x => x.getDamage());
                    final.blockedCount = damageLogs.Count(x => x.GetResult() == ParseEnum.Result.Block);
                    final.invulnedCount = 0;
                    final.damageInvulned = 0;
                    final.evadedCount = damageLogs.Count(x => x.GetResult() == ParseEnum.Result.Evade);
                    final.damageBarrier = damageLogs.Sum(x => x.IsShields() == 1 ? x.GetDamage() : 0);
                    foreach (DamageLog log in damageLogs.Where(x => x.GetResult() == ParseEnum.Result.Absorb))
                    {
                        final.invulnedCount++;
                        final.damageInvulned += log.GetDamage();
                    }

                    phaseDefense[phaseIndex] = final;
                }
                statistics.defenses[player] = phaseDefense;
            }
        }

       
        private void CalculateSupport()
        {
            foreach (Player player in log.GetPlayerList())
            {
                Statistics.FinalSupport[] phaseSupport = new Statistics.FinalSupport[statistics.phases.Count];
                for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
                {
                    Statistics.FinalSupport final = new Statistics.FinalSupport();

                    PhaseData phase =statistics.phases[phaseIndex];
                    long start = phase.GetStart() + log.GetBossData().GetFirstAware();
                    long end = phase.GetEnd() + log.GetBossData().GetFirstAware();

                    // List<DamageLog> damage_logs = p.getDamageTakenLogs(b_data, c_data.getCombatList(), getAgentData());
                    int instid = player.GetInstid();

                    int[] resArray = player.GetReses(log, phase.GetStart(), phase.GetEnd());
                    int[] cleanseArray = player.GetCleanses(log, phase.GetStart(), phase.GetEnd());
                    //List<DamageLog> healingLogs = player.getHealingLogs(log, phase.getStart(), phase.getEnd());
                    //final.allHeal = healingLogs.Sum(x => x.getDamage());
                    final.resurrects = resArray[0];
                    final.ressurrectTime = resArray[1]/1000f;
                    final.condiCleanse = cleanseArray[0];
                    final.condiCleanseTime = cleanseArray[1]/1000f;

                    phaseSupport[phaseIndex] = final;
                }
                statistics.support[player] = phaseSupport;
            }
        }

        private Dictionary<long, Statistics.FinalBoonUptime> GetBoonsForList(List<Player> playerList, Player player, List<Boon> to_track, int phaseIndex)
        {
            PhaseData phase =statistics.phases[phaseIndex];
            long fightDuration = phase.GetEnd() - phase.GetStart();

            Dictionary<Player, BoonDistribution> boonDistributions = new Dictionary<Player, BoonDistribution>();
            foreach (Player p in playerList)
            {
                boonDistributions[p] = p.GetBoonDistribution(log,statistics.phases, to_track, phaseIndex);
            }

            Dictionary<long, Statistics.FinalBoonUptime> final =
                new Dictionary<long, Statistics.FinalBoonUptime>();

            foreach (Boon boon in to_track)
            {
                long totalGeneration = 0;
                long totalOverstack = 0;

                foreach (BoonDistribution boons in boonDistributions.Values)
                {
                    if (boons.ContainsKey(boon.GetID()))
                    {
                        totalGeneration += boons.GetGeneration(boon.GetID(), player.GetInstid());
                        totalOverstack += boons.GetOverstack(boon.GetID(), player.GetInstid());
                    }
                }

                Statistics.FinalBoonUptime uptime = new Statistics.FinalBoonUptime();

                if (boon.GetBoonType() == Boon.BoonType.Duration)
                {
                    uptime.generation = Math.Round(100.0f * totalGeneration / fightDuration / playerList.Count, 1);
                    uptime.overstack = Math.Round(100.0f * (totalOverstack + totalGeneration)/ fightDuration / playerList.Count, 1);
                }
                else if (boon.GetBoonType() == Boon.BoonType.Intensity)
                {
                    uptime.generation = Math.Round((double)totalGeneration / fightDuration / playerList.Count, 1);
                    uptime.overstack = Math.Round((double)(totalOverstack + totalGeneration) / fightDuration / playerList.Count, 1);
                }
                
                final[boon.GetID()] = uptime;
            }

            return final;
        }

        private void CalculateBoons()
        {
            // Player Boons
            foreach (Player player in log.GetPlayerList())
            {
                List<Boon> boon_to_track = new List<Boon>();
                boon_to_track.AddRange(statistics.present_boons);
                boon_to_track.AddRange(statistics.present_offbuffs);
                boon_to_track.AddRange(statistics.present_defbuffs);
                boon_to_track.AddRange(statistics.present_personnal[player.GetInstid()]);
                Dictionary<long, Statistics.FinalBoonUptime>[] phaseBoons = new Dictionary<long, Statistics.FinalBoonUptime>[statistics.phases.Count];
                for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
                {
                    Dictionary<long, Statistics.FinalBoonUptime> final = new Dictionary<long, Statistics.FinalBoonUptime>();

                    PhaseData phase =statistics.phases[phaseIndex];

                    BoonDistribution selfBoons = player.GetBoonDistribution(log,statistics.phases, boon_to_track, phaseIndex);

                    long fightDuration = phase.GetEnd() - phase.GetStart();
                    foreach (Boon boon in boon_to_track)
                    {
                        Statistics.FinalBoonUptime uptime = new Statistics.FinalBoonUptime
                        {
                            uptime = 0,
                            generation = 0,
                            overstack = 0
                        };
                        if (selfBoons.ContainsKey(boon.GetID()))
                        {
                            long generation = selfBoons.GetGeneration(boon.GetID(), player.GetInstid());
                            if (boon.GetBoonType() == Boon.BoonType.Duration)
                            {
                                uptime.uptime = Math.Round(100.0 * selfBoons.GetUptime(boon.GetID()) / fightDuration, 1);
                                uptime.generation = Math.Round(100.0f * generation / fightDuration, 1);
                                uptime.overstack = Math.Round(100.0f * (selfBoons.GetOverstack(boon.GetID(), player.GetInstid()) + generation) / fightDuration, 1);
                            }
                            else if (boon.GetBoonType() == Boon.BoonType.Intensity)
                            {
                                uptime.uptime = Math.Round((double)selfBoons.GetUptime(boon.GetID()) / fightDuration, 1);
                                uptime.generation = Math.Round((double)generation / fightDuration, 1);
                                uptime.overstack = Math.Round((double)(selfBoons.GetOverstack(boon.GetID(), player.GetInstid()) + generation) / fightDuration, 1);
                            }
                        }
                        final[boon.GetID()] = uptime;
                    }

                    phaseBoons[phaseIndex] = final;
                }
                statistics.selfBoons[player] = phaseBoons;
            }

            // Group Boons
            foreach (Player player in log.GetPlayerList())
            {
                List<Boon> boon_to_track = new List<Boon>();
                boon_to_track.AddRange(statistics.present_boons);
                boon_to_track.AddRange(statistics.present_offbuffs);
                boon_to_track.AddRange(statistics.present_defbuffs);
                boon_to_track.AddRange(statistics.present_personnal[player.GetInstid()]);
                List<Player> groupPlayers = new List<Player>();
                foreach (Player p in log.GetPlayerList())
                {
                    if (p.GetGroup() == player.GetGroup() && player.GetInstid() != p.GetInstid()) groupPlayers.Add(p);
                }
                Dictionary<long, Statistics.FinalBoonUptime>[] phaseBoons = new Dictionary<long, Statistics.FinalBoonUptime>[statistics.phases.Count];
                for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
                {
                    phaseBoons[phaseIndex] = GetBoonsForList(groupPlayers, player, boon_to_track, phaseIndex);
                }
                statistics.groupBoons[player] = phaseBoons;
            }

            // Off Group Boons
            foreach (Player player in log.GetPlayerList())
            {
                List<Boon> boon_to_track = new List<Boon>();
                boon_to_track.AddRange(statistics.present_boons);
                boon_to_track.AddRange(statistics.present_offbuffs);
                boon_to_track.AddRange(statistics.present_defbuffs);
                boon_to_track.AddRange(statistics.present_personnal[player.GetInstid()]);
                List<Player> groupPlayers = new List<Player>();
                foreach (Player p in log.GetPlayerList())
                {
                    if (p.GetGroup() != player.GetGroup()) groupPlayers.Add(p);
                }
                Dictionary<long, Statistics.FinalBoonUptime>[] phaseBoons = new Dictionary<long, Statistics.FinalBoonUptime>[statistics.phases.Count];
                for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
                {                    
                    phaseBoons[phaseIndex] = GetBoonsForList(groupPlayers, player, boon_to_track, phaseIndex);
                }
                statistics.offGroupBoons[player] = phaseBoons;
            }

            // Squad Boons
            foreach (Player player in log.GetPlayerList())
            {
                List<Boon> boon_to_track = new List<Boon>();
                boon_to_track.AddRange(statistics.present_boons);
                boon_to_track.AddRange(statistics.present_offbuffs);
                boon_to_track.AddRange(statistics.present_defbuffs);
                boon_to_track.AddRange(statistics.present_personnal[player.GetInstid()]);
                List<Player> groupPlayers = new List<Player>();
                foreach (Player p in log.GetPlayerList())
                {
                    if (p.GetInstid() != player.GetInstid())
                        groupPlayers.Add(p);
                }
                Dictionary<long, Statistics.FinalBoonUptime>[] phaseBoons = new Dictionary<long, Statistics.FinalBoonUptime>[statistics.phases.Count];
                for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
                {                
                    phaseBoons[phaseIndex] = GetBoonsForList(groupPlayers, player, boon_to_track, phaseIndex);
                }
                statistics.squadBoons[player] = phaseBoons;
            }
        }

        public void CalculateConditions()
        {
            statistics.bossConditions = new Dictionary<long, Statistics.FinalBossBoon>[statistics.phases.Count];
            List<Boon> boon_to_track = Boon.GetCondiBoonList();
            boon_to_track.AddRange(Boon.GetBoonList());
            boon_to_track.AddRange(Boon.GetBossBoonList());
            for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
            {
                BoonDistribution boonDistribution = log.GetBoss().GetBoonDistribution(log,statistics.phases, boon_to_track, phaseIndex);
                Dictionary<long, Statistics.FinalBossBoon> rates = new Dictionary<long, Statistics.FinalBossBoon>();

                PhaseData phase =statistics.phases[phaseIndex];
                long fightDuration = phase.GetDuration();

                foreach (Boon boon in boon_to_track)
                {
                    Statistics.FinalBossBoon condition = new Statistics.FinalBossBoon(log.GetPlayerList());
                    rates[boon.GetID()] = condition;
                    if (boonDistribution.ContainsKey(boon.GetID()))
                    {
                        if (boon.GetBoonType() == Boon.BoonType.Duration)
                        {
                            condition.uptime = Math.Round(100.0 * boonDistribution.GetUptime(boon.GetID()) / fightDuration, 1);
                            foreach(Player p in log.GetPlayerList())
                            {
                                long gen = boonDistribution.GetGeneration(boon.GetID(), p.GetInstid());
                                condition.generated[p] = Math.Round(100.0 * gen / fightDuration, 1);
                                condition.overstacked[p] = Math.Round(100.0 * (boonDistribution.GetOverstack(boon.GetID(), p.GetInstid()) + gen) / fightDuration, 1);
                            }
                        }
                        else if (boon.GetBoonType() == Boon.BoonType.Intensity)
                        {
                            condition.uptime = Math.Round((double) boonDistribution.GetUptime(boon.GetID()) / fightDuration, 1);
                            foreach (Player p in log.GetPlayerList())
                            {
                                long gen = boonDistribution.GetGeneration(boon.GetID(), p.GetInstid());
                                condition.generated[p] = Math.Round((double) gen / fightDuration, 1);
                                condition.overstacked[p] = Math.Round((double)(boonDistribution.GetOverstack(boon.GetID(), p.GetInstid())+ gen) / fightDuration, 1);
                            }
                        }

                        rates[boon.GetID()] = condition;
                    }
                }

                statistics.bossConditions[phaseIndex] = rates;
            }
        }
        /// <summary>
        /// Checks the combat data and gets buffs that were present during the fight
        /// </summary>
        private void SetPresentBoons()
        {
            List<CombatItem> c_list = log.GetCombatData();
            if (settings.PlayerBoonsUniversal)
            {//Main boons
                foreach (Boon boon in Boon.GetBoonList())
                {
                    if (c_list.Exists(x => x.GetSkillID() == boon.GetID()))
                    {
                        statistics.present_boons.Add(boon);
                    }
                }
            }
            if (settings.PlayerBoonsImpProf)
            {//Important Class specefic boons
                foreach (Boon boon in Boon.GetOffensiveTableList())
                {
                    if (c_list.Exists(x => x.GetSkillID() == boon.GetID()))
                    {
                        statistics.present_offbuffs.Add(boon);
                    }
                }
                foreach (Boon boon in Boon.GetDefensiveTableList())
                {
                    if (c_list.Exists(x => x.GetSkillID() == boon.GetID()))
                    {
                        statistics.present_defbuffs.Add(boon);
                    }
                }
            }

            foreach (Player p in log.GetPlayerList())
            {
                statistics.present_personnal[p.GetInstid()] = new List<Boon>();
                if (settings.PlayerBoonsAllProf)
                {//All class specefic boons
                    List<Boon> notYetFoundBoons = Boon.GetRemainingBuffsList();
                    c_list.ForEach(item =>
                    {
                        if (item.GetDstInstid() == p.GetInstid()) {
                            Boon foundBoon = notYetFoundBoons.Find(boon => boon.GetID() == item.GetSkillID());
                            if (foundBoon != null)
                            {
                                notYetFoundBoons.Remove(foundBoon);
                                statistics.present_personnal[p.GetInstid()].Add(foundBoon);
                            }
                        }
                    });
                }
            }
        }
    }
}
