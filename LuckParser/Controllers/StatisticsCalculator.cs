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

            statistics.phases = log.GetBoss().getPhases(log, settings.ParsePhases);
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
                log.GetMechanicData().computePresentMechanics(log, statistics.phases);
            }

            return statistics;
        }

        private Statistics.FinalDPS GetFinalDPS(AbstractPlayer player, int phaseIndex, bool checkRedirection)
        {
            Statistics.FinalDPS final = new Statistics.FinalDPS();

            PhaseData phase = statistics.phases[phaseIndex];

            double phaseDuration = (phase.getDuration()) / 1000.0;

            double damage = 0.0;
            double dps = 0.0;

            ////////// ALL
            //DPS
            damage = player.GetDamageLogs(0, log, phase.getStart(),
                    phase.getEnd())
                .Sum(x => x.getDamage());
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.allDps = (int)dps;
            final.allDamage = (int)damage;
            //Condi DPS
            damage = player.GetDamageLogs(0, log, phase.getStart(),
                    phase.getEnd())
                .Where(x => x.isCondi() > 0).Sum(x => x.getDamage());
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
                phase.getStart(), phase.getEnd()).Where(x => x.isCondi() == 0).Sum(x => x.getDamage());
            /////////// BOSS
            //DPS
            if (checkRedirection && phase.getRedirection().Count > 0)
            {
                damage = player.GetDamageLogs(phase.getRedirection(), log,
                    phase.getStart(), phase.getEnd()).Sum(x => x.getDamage());
            } else
            {
                damage = player.GetDamageLogs(log.GetBossData().getInstid(), log,
                    phase.getStart(), phase.getEnd()).Sum(x => x.getDamage());
            }
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.bossDps = (int)dps;
            final.bossDamage = (int)damage;
            //Condi DPS
            if (checkRedirection && phase.getRedirection().Count > 0)
            {
                damage = player.GetDamageLogs(phase.getRedirection(), log,
                    phase.getStart(), phase.getEnd()).Where(x => x.isCondi() > 0).Sum(x => x.getDamage());
            }
            else
            {
                damage = player.GetDamageLogs(log.GetBossData().getInstid(), log,
                    phase.getStart(), phase.getEnd()).Where(x => x.isCondi() > 0).Sum(x => x.getDamage());
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
            if (checkRedirection && phase.getRedirection().Count > 0)
            {
                final.playerBossPowerDamage = player.GetJustPlayerDamageLogs(phase.getRedirection(), log,
                    phase.getStart(), phase.getEnd()).Where(x => x.isCondi() == 0).Sum(x => x.getDamage());
            }
            else
            {
                final.playerBossPowerDamage = player.GetJustPlayerDamageLogs(log.GetBossData().getInstid(), log,
                    phase.getStart(), phase.getEnd()).Where(x => x.isCondi() == 0).Sum(x => x.getDamage());
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
                    long start = phase.getStart() + log.GetBossData().getFirstAware();
                    long end = phase.getEnd() + log.GetBossData().getFirstAware();

                    List<DamageLog> damageLogs  = player.GetJustPlayerDamageLogs(0, log, phase.getStart(), phase.getEnd());
                    List<CastLog> castLogs = player.GetCastLogs(log, phase.getStart(), phase.getEnd());

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
                    if (phase.getRedirection().Count > 0)
                    {
                        foreach (AgentItem a in phase.getRedirection())
                        {
                            idsToCheck.Add(a.getInstid());
                        }
                    } else
                    {
                        idsToCheck.Add(log.GetBossData().getInstid());
                    }
                    foreach (DamageLog dl in damageLogs)
                    {
                        if (dl.isCondi() == 0)
                        {

                            if (idsToCheck.Contains(dl.getDstInstidt()))
                            {
                                if (idsToCheck.Count > 1)
                                {
                                    AgentItem target = phase.getRedirection().Find(x => x.getInstid() == dl.getDstInstidt());
                                    if (dl.getTime() < target.getFirstAware() - log.GetBossData().getFirstAware() || dl.getTime() > target.getLastAware() - log.GetBossData().getFirstAware())
                                    {
                                        continue;
                                    }
                                }
                                if (dl.getResult() == ParseEnum.Result.Crit)
                                {
                                    final.criticalRateBoss++;
                                    final.criticalDmgBoss += dl.getDamage();
                                }

                                if (dl.isNinety() > 0)
                                {
                                    final.scholarRateBoss++;
                                    final.scholarDmgBoss += (int)(dl.getDamage() / 11.0); //regular+10% damage
                                }

                                if (dl.isMoving() > 0)
                                {
                                    final.movingRateBoss++;
                                    final.movingDamageBoss += (int)(dl.getDamage() / 21.0);
                                }
                                
                                final.flankingRateBoss += dl.isFlanking();

                                if (dl.getResult() == ParseEnum.Result.Glance)
                                {
                                    final.glanceRateBoss++;
                                }

                                if (dl.getResult() == ParseEnum.Result.Blind)
                                {
                                    final.missedBoss++;
                                }

                                if (dl.getResult() == ParseEnum.Result.Interrupt)
                                {
                                    final.interuptsBoss++;
                                }

                                if (dl.getResult() == ParseEnum.Result.Absorb)
                                {
                                    final.invulnedBoss++;
                                }
                                final.powerLoopCountBoss++;
                                if (!nonCritable.Contains(dl.getID()))
                                {
                                    final.critablePowerLoopCountBoss++;
                                }
                            }

                            if (dl.getResult() == ParseEnum.Result.Crit)
                            {
                                final.criticalRate++;
                                final.criticalDmg += dl.getDamage();
                            }

                            if (dl.isNinety() > 0)
                            {
                                final.scholarRate++;
                                final.scholarDmg += (int)(dl.getDamage() / 11.0); //regular+10% damage
                            }

                            if (dl.isMoving() > 0)
                            {
                                final.movingRate++;
                                final.movingDamage += (int)(dl.getDamage() / 21.0);
                            }
                            
                            final.flankingRate += dl.isFlanking();

                            if (dl.getResult() == ParseEnum.Result.Glance)
                            {
                                final.glanceRate++;
                            }

                            if (dl.getResult() == ParseEnum.Result.Blind)
                            {
                                final.missed++;
                            }

                            if (dl.getResult() == ParseEnum.Result.Interrupt)
                            {
                                final.interupts++;
                            }

                            if (dl.getResult() == ParseEnum.Result.Absorb)
                            {
                                final.invulned++;
                            }
                            final.powerLoopCount++;
                            if (!nonCritable.Contains(dl.getID()))
                            {
                                final.critablePowerLoopCount++;
                            }
                        }
                    }
                    foreach (CastLog cl in castLogs)
                    {
                        if (cl.endActivation() == ParseEnum.Activation.CancelCancel)
                        {
                            final.wasted++;
                            final.timeWasted += cl.getActDur();
                        }
                        if (cl.endActivation() == ParseEnum.Activation.CancelFire)
                        {
                            final.saved++;
                            if (cl.getActDur() < cl.getExpDur())
                            {
                                final.timeSaved += cl.getExpDur() - cl.getActDur();
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
                                List<Point3D> list = p.GetCombatReplay().getActivePositions();  
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
                        List<Point3D> positions = player.GetCombatReplay().getPositions().Where(x => x.time >= phase.getStart() && x.time <= phase.getEnd()).ToList();
                        int offset = player.GetCombatReplay().getPositions().Count(x => x.time < phase.getStart());
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
                        final.died = dead.Last().getTime() - start;
                    }

                    List<CombatItem> disconect = combatData.GetStates(instid, ParseEnum.StateChange.Despawn, start, end);
                    final.dcd = 0.0;
                    if (disconect.Count > 0)
                    {
                        final.dcd = disconect.Last().getTime() - start;
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
                    long start = phase.getStart() + log.GetBossData().getFirstAware();
                    long end = phase.getEnd() + log.GetBossData().getFirstAware();

                    List<DamageLog> damageLogs = player.GetDamageTakenLogs(log, phase.getStart(), phase.getEnd());
                    //List<DamageLog> healingLogs = player.getHealingReceivedLogs(log, phase.getStart(), phase.getEnd());

                    int instID = player.GetInstid();
                 
                    final.damageTaken = damageLogs.Sum(x => (long)x.getDamage());
                    //final.allHealReceived = healingLogs.Sum(x => x.getDamage());
                    final.blockedCount = damageLogs.Count(x => x.getResult() == ParseEnum.Result.Block);
                    final.invulnedCount = 0;
                    final.damageInvulned = 0;
                    final.evadedCount = damageLogs.Count(x => x.getResult() == ParseEnum.Result.Evade);
                    final.damageBarrier = damageLogs.Sum(x => x.isShields() == 1 ? x.getDamage() : 0);
                    foreach (DamageLog log in damageLogs.Where(x => x.getResult() == ParseEnum.Result.Absorb))
                    {
                        final.invulnedCount++;
                        final.damageInvulned += log.getDamage();
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
                    long start = phase.getStart() + log.GetBossData().getFirstAware();
                    long end = phase.getEnd() + log.GetBossData().getFirstAware();

                    // List<DamageLog> damage_logs = p.getDamageTakenLogs(b_data, c_data.getCombatList(), getAgentData());
                    int instid = player.GetInstid();

                    int[] resArray = player.GetReses(log, phase.getStart(), phase.getEnd());
                    int[] cleanseArray = player.GetCleanses(log, phase.getStart(), phase.getEnd());
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
            long fightDuration = phase.getEnd() - phase.getStart();

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
                    if (boons.ContainsKey(boon.getID()))
                    {
                        totalGeneration += boons.getGeneration(boon.getID(), player.GetInstid());
                        totalOverstack += boons.getOverstack(boon.getID(), player.GetInstid());
                    }
                }

                Statistics.FinalBoonUptime uptime = new Statistics.FinalBoonUptime();

                if (boon.getType() == Boon.BoonType.Duration)
                {
                    uptime.generation = Math.Round(100.0f * totalGeneration / fightDuration / playerList.Count, 1);
                    uptime.overstack = Math.Round(100.0f * (totalOverstack + totalGeneration)/ fightDuration / playerList.Count, 1);
                }
                else if (boon.getType() == Boon.BoonType.Intensity)
                {
                    uptime.generation = Math.Round((double)totalGeneration / fightDuration / playerList.Count, 1);
                    uptime.overstack = Math.Round((double)(totalOverstack + totalGeneration) / fightDuration / playerList.Count, 1);
                }
                
                final[boon.getID()] = uptime;
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

                    long fightDuration = phase.getEnd() - phase.getStart();
                    foreach (Boon boon in boon_to_track)
                    {
                        Statistics.FinalBoonUptime uptime = new Statistics.FinalBoonUptime
                        {
                            uptime = 0,
                            generation = 0,
                            overstack = 0
                        };
                        if (selfBoons.ContainsKey(boon.getID()))
                        {
                            long generation = selfBoons.getGeneration(boon.getID(), player.GetInstid());
                            if (boon.getType() == Boon.BoonType.Duration)
                            {
                                uptime.uptime = Math.Round(100.0 * selfBoons.getUptime(boon.getID()) / fightDuration, 1);
                                uptime.generation = Math.Round(100.0f * generation / fightDuration, 1);
                                uptime.overstack = Math.Round(100.0f * (selfBoons.getOverstack(boon.getID(), player.GetInstid()) + generation) / fightDuration, 1);
                            }
                            else if (boon.getType() == Boon.BoonType.Intensity)
                            {
                                uptime.uptime = Math.Round((double)selfBoons.getUptime(boon.getID()) / fightDuration, 1);
                                uptime.generation = Math.Round((double)generation / fightDuration, 1);
                                uptime.overstack = Math.Round((double)(selfBoons.getOverstack(boon.getID(), player.GetInstid()) + generation) / fightDuration, 1);
                            }
                        }
                        final[boon.getID()] = uptime;
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
            List<Boon> boon_to_track = Boon.getCondiBoonList();
            boon_to_track.AddRange(Boon.getBoonList());
            boon_to_track.AddRange(Boon.getBossBoonList());
            for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
            {
                BoonDistribution boonDistribution = log.GetBoss().GetBoonDistribution(log,statistics.phases, boon_to_track, phaseIndex);
                Dictionary<long, Statistics.FinalBossBoon> rates = new Dictionary<long, Statistics.FinalBossBoon>();

                PhaseData phase =statistics.phases[phaseIndex];
                long fightDuration = phase.getDuration();

                foreach (Boon boon in boon_to_track)
                {
                    Statistics.FinalBossBoon condition = new Statistics.FinalBossBoon(log.GetPlayerList());
                    rates[boon.getID()] = condition;
                    if (boonDistribution.ContainsKey(boon.getID()))
                    {
                        if (boon.getType() == Boon.BoonType.Duration)
                        {
                            condition.uptime = Math.Round(100.0 * boonDistribution.getUptime(boon.getID()) / fightDuration, 1);
                            foreach(Player p in log.GetPlayerList())
                            {
                                long gen = boonDistribution.getGeneration(boon.getID(), p.GetInstid());
                                condition.generated[p] = Math.Round(100.0 * gen / fightDuration, 1);
                                condition.overstacked[p] = Math.Round(100.0 * (boonDistribution.getOverstack(boon.getID(), p.GetInstid()) + gen) / fightDuration, 1);
                            }
                        }
                        else if (boon.getType() == Boon.BoonType.Intensity)
                        {
                            condition.uptime = Math.Round((double) boonDistribution.getUptime(boon.getID()) / fightDuration, 1);
                            foreach (Player p in log.GetPlayerList())
                            {
                                long gen = boonDistribution.getGeneration(boon.getID(), p.GetInstid());
                                condition.generated[p] = Math.Round((double) gen / fightDuration, 1);
                                condition.overstacked[p] = Math.Round((double)(boonDistribution.getOverstack(boon.getID(), p.GetInstid())+ gen) / fightDuration, 1);
                            }
                        }

                        rates[boon.getID()] = condition;
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
                foreach (Boon boon in Boon.getBoonList())
                {
                    if (c_list.Exists(x => x.getSkillID() == boon.getID()))
                    {
                        statistics.present_boons.Add(boon);
                    }
                }
            }
            if (settings.PlayerBoonsImpProf)
            {//Important Class specefic boons
                foreach (Boon boon in Boon.getOffensiveTableList())
                {
                    if (c_list.Exists(x => x.getSkillID() == boon.getID()))
                    {
                        statistics.present_offbuffs.Add(boon);
                    }
                }
                foreach (Boon boon in Boon.getDefensiveTableList())
                {
                    if (c_list.Exists(x => x.getSkillID() == boon.getID()))
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
                    List<Boon> notYetFoundBoons = Boon.getRemainingBuffsList();
                    c_list.ForEach(item =>
                    {
                        if (item.getDstInstid() == p.GetInstid()) {
                            Boon foundBoon = notYetFoundBoons.Find(boon => boon.getID() == item.getSkillID());
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
