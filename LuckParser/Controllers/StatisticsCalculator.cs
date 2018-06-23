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
        }

        private SettingsContainer settings;

        private Statistics statistics;

        private ParsedLog log;

        private List<PhaseData> phases;

        public StatisticsCalculator(SettingsContainer settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Calculate a statistic from a log
        /// </summary>
        /// <param name="log">log to calculate stats from</param>
        /// <returns></returns>
        public Statistics calculateStatistics(ParsedLog log, Switches switches)
        {
            statistics = new Statistics();

            this.log = log;

            phases = log.getBoss().getPhases(log, settings.ParsePhases);

            if (switches.calculateDPS) calculateDPS();
            if (switches.calculateStats) calculateStats();
            if (switches.calculateDefense) calculateDefenses();
            if (switches.calculateSupport) calculateSupport();
            if (switches.calculateBoons)
            {
                setPresentBoons();
                calculateBoons();
            } 
                      
            if (switches.calculateConditions) calculateConditions();
            // WIP
            /*if (settings.PlayerRot)
            {
                foreach (Player p in log.getPlayerList())
                {
                    p.getRotation(log, settings.PlayerRotIcons);
                }
                log.getBoss().getRotation(log, settings.PlayerRotIcons);
            }*/

            return statistics;
        }

        private Statistics.FinalDPS getFinalDPS(AbstractPlayer player, int phaseIndex)
        {
            Statistics.FinalDPS final = new Statistics.FinalDPS();

            PhaseData phase = phases[phaseIndex];

            double phaseDuration = (phase.getDuration()) / 1000.0;

            double damage = 0.0;
            double dps = 0.0;

            // All DPS
            damage = player.getDamageLogs(0, log, phase.getStart(),
                    phase.getEnd())
                .Sum(x => x.getDamage());
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }

            final.allDps = (int)dps;
            final.allDamage = (int)damage;

            // All Condi DPS
            damage = player.getDamageLogs(0, log, phase.getStart(),
                    phase.getEnd())
                .Where(x => x.isCondi() > 0).Sum(x => x.getDamage());
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }

            final.allCondiDps = (int)dps;
            final.allCondiDamage = (int)damage;

            // All Power DPS
            damage = final.allDamage - damage;
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }

            final.allPowerDps = (int)dps;
            final.allPowerDamage = (int)damage;

            // Boss DPS
            damage = player.getDamageLogs(log.getBossData().getInstid(), log,
                phase.getStart(), phase.getEnd()).Sum(x => x.getDamage());
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }

            final.bossDps = (int)dps;
            final.bossDamage = (int)damage;


            // Boss Condi DPS
            damage = player.getDamageLogs(log.getBossData().getInstid(), log,
                phase.getStart(), phase.getEnd()).Where(x => x.isCondi() > 0).Sum(x => x.getDamage());
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }

            final.bossCondiDps = (int)dps;
            final.bossCondiDamage = (int)dps;

            // Boss Power DPS
            damage = final.bossDamage - damage;
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }

            final.bossPowerDps = (int)dps;
            final.bossPowerDamage = (int)damage;

            return final;
        }

        private void calculateDPS()
        {
            foreach (Player player in log.getPlayerList())
            {
                Statistics.FinalDPS[] phaseDps = new Statistics.FinalDPS[phases.Count];
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {
                    phaseDps[phaseIndex] = getFinalDPS(player,phaseIndex);
                }

                statistics.dps[player] = phaseDps;
            }

            Statistics.FinalDPS[] phaseBossDps = new Statistics.FinalDPS[phases.Count];
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                phaseBossDps[phaseIndex] = getFinalDPS(log.getBoss(), phaseIndex);
            }

            statistics.bossDps = phaseBossDps;
        }

        private void calculateStats()
        {
            foreach (Player player in log.getPlayerList())
            {
                Statistics.FinalStats[] phaseStats = new Statistics.FinalStats[phases.Count];
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {
                    Statistics.FinalStats final = new Statistics.FinalStats();

                    PhaseData phase = phases[phaseIndex];
                    long start = phase.getStart() + log.getBossData().getFirstAware();
                    long end = phase.getEnd() + log.getBossData().getFirstAware();

                    List<DamageLog> damageLogs = player.getDamageLogs(0, log, phase.getStart(), phase.getEnd());
                    List<DamageLog> damageLogsBoss = player.getDamageLogs(log.getBoss().getInstid(), log, phase.getStart(), phase.getEnd());
                    List<CastLog> castLogs = player.getCastLogs(log, phase.getStart(), phase.getEnd());

                    int instid = player.getInstid();

                    final.powerLoopCount = 0;
                    final.criticalRate = 0;
                    final.criticalDmg = 0;
                    final.scholarRate = 0;
                    final.scholarDmg = 0;
                    final.movingRate = 0;
                    final.flankingRate = 0;
                    final.glanceRate = 0;
                    final.missed = 0;
                    final.interupts = 0;
                    final.invulned = 0;
                    final.wasted = 0;
                    final.timeWasted = 0;
                    final.saved = 0;
                    final.timeSaved = 0;

                    final.powerLoopCountBoss = 0;
                    final.criticalRateBoss = 0;
                    final.criticalDmgBoss = 0;
                    final.scholarRateBoss = 0;
                    final.scholarDmgBoss = 0;
                    final.movingRateBoss = 0;
                    final.flankingRateBoss = 0;
                    final.glanceRateBoss = 0;
                    final.missedBoss = 0;
                    final.interuptsBoss = 0;
                    final.invulnedBoss = 0;

                    foreach (DamageLog log in damageLogs)
                    {
                        if (log.isCondi() == 0)
                        {
                            if (log.getResult() == ParseEnum.Result.Crit)
                            {
                                final.criticalRate++;
                                final.criticalDmg += log.getDamage();
                            }

                            if (log.isNinety() > 0)
                            {
                                final.scholarRate++;
                                final.scholarDmg += (int)(log.getDamage() / 11.0); //regular+10% damage
                            }

                            final.movingRate += log.isMoving();
                            final.flankingRate += log.isFlanking();

                            if (log.getResult() == ParseEnum.Result.Glance)
                            {
                                final.glanceRate++;
                            }

                            if (log.getResult() == ParseEnum.Result.Blind)
                            {
                                final.missed++;
                            }

                            if (log.getResult() == ParseEnum.Result.Interrupt)
                            {
                                final.interupts++;
                            }

                            if (log.getResult() == ParseEnum.Result.Absorb)
                            {
                                final.invulned++;
                            }
                            final.powerLoopCount++;
                        }
                    }
                    foreach (DamageLog log in damageLogsBoss)
                    {
                        if (log.isCondi() == 0)
                        {
                            if (log.getResult() == ParseEnum.Result.Crit)
                            {
                                final.criticalRateBoss++;
                                final.criticalDmgBoss += log.getDamage();
                            }

                            if (log.isNinety() > 0)
                            {
                                final.scholarRateBoss++;
                                final.scholarDmgBoss += (int)(log.getDamage() / 11.0); //regular+10% damage
                            }

                            final.movingRateBoss += log.isMoving();
                            final.flankingRateBoss += log.isFlanking();

                            if (log.getResult() == ParseEnum.Result.Glance)
                            {
                                final.glanceRateBoss++;
                            }

                            if (log.getResult() == ParseEnum.Result.Blind)
                            {
                                final.missedBoss++;
                            }

                            if (log.getResult() == ParseEnum.Result.Interrupt)
                            {
                                final.interuptsBoss++;
                            }

                            if (log.getResult() == ParseEnum.Result.Absorb)
                            {
                                final.invulnedBoss++;
                            }
                            final.powerLoopCountBoss++;
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

                    final.totalDmg = damageLogs.Sum(x => x.getDamage());
                    final.powerLoopCount = final.powerLoopCount == 0 ? 1 : final.powerLoopCount;

                    final.totalDmgBoss = damageLogsBoss.Sum(x => x.getDamage());
                    final.powerLoopCountBoss = final.powerLoopCountBoss == 0 ? 1 : final.powerLoopCountBoss;

                    // Counts
                    CombatData combatData = log.getCombatData();
                    final.swapCount = combatData.getStates(instid, ParseEnum.StateChange.WeaponSwap, start, end).Count();
                    final.downCount = combatData.getStates(instid, ParseEnum.StateChange.ChangeDown, start, end).Count();
                    final.dodgeCount = combatData.getSkillCount(instid, 65001, start, end) + combatData.getBuffCount(instid, 40408, start, end);//dodge = 65001 mirage cloak =40408
                    final.ressCount = combatData.getSkillCount(instid, 1066, start, end); //Res = 1066

                    // R.I.P
                    List<CombatItem> dead = combatData.getStates(instid, ParseEnum.StateChange.ChangeDead, start, end);
                    final.died = 0.0;
                    if (dead.Count() > 0)
                    {
                        final.died = dead[0].getTime() - start;
                    }

                    List<CombatItem> disconect = combatData.getStates(instid, ParseEnum.StateChange.Despawn, start, end);
                    final.dcd = 0.0;
                    if (disconect.Count() > 0)
                    {
                        final.dcd = disconect[0].getTime() - start;
                    }

                    phaseStats[phaseIndex] = final;
                }
                statistics.stats[player] = phaseStats;
            }
        }

        private void calculateDefenses()
        {
            foreach (Player player in log.getPlayerList())
            {
                Statistics.FinalDefenses[] phaseDefense = new Statistics.FinalDefenses[phases.Count];
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {
                    Statistics.FinalDefenses final = new Statistics.FinalDefenses();

                    PhaseData phase = phases[phaseIndex];
                    long start = phase.getStart() + log.getBossData().getFirstAware();
                    long end = phase.getEnd() + log.getBossData().getFirstAware();

                    List<DamageLog> damageLogs = player.getDamageTakenLogs(log, phase.getStart(), phase.getEnd());

                    int instID = player.getInstid();

                    // TODO damageBlocked and damageEvaded

                    final.damageTaken = damageLogs.Select(x => (long)x.getDamage()).Sum();
                    final.blockedCount = 0;
                    final.invulnedCount = 0;
                    final.damageInvulned = 0;
                    final.evadedCount = 0;
                    final.damageBarrier = 0;
                    foreach (DamageLog log in damageLogs.Where(x => x.getResult() == ParseEnum.Result.Block))
                    {
                        final.blockedCount++;
                    }
                    foreach (DamageLog log in damageLogs.Where(x => x.getResult() == ParseEnum.Result.Absorb))
                    {
                        final.invulnedCount++;
                        final.damageInvulned += log.getDamage();
                    }
                    foreach (DamageLog log in damageLogs.Where(x => x.getResult() == ParseEnum.Result.Evade))
                    {
                        final.evadedCount++;
                    }
                    foreach (DamageLog log in damageLogs.Where(x => x.isShields() == 1))
                    {
                        final.damageBarrier += log.getDamage();
                    }

                    phaseDefense[phaseIndex] = final;
                }
                statistics.defenses[player] = phaseDefense;
            }
        }

        // TODO ensure it is working correctly
        private void calculateSupport()
        {
            foreach (Player player in log.getPlayerList())
            {
                Statistics.FinalSupport[] phaseSupport = new Statistics.FinalSupport[phases.Count];
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {
                    Statistics.FinalSupport final = new Statistics.FinalSupport();

                    PhaseData phase = phases[phaseIndex];
                    long start = phase.getStart() + log.getBossData().getFirstAware();
                    long end = phase.getEnd() + log.getBossData().getFirstAware();

                    // List<DamageLog> damage_logs = p.getDamageTakenLogs(b_data, c_data.getCombatList(), getAgentData());
                    int instid = player.getInstid();

                    int[] resArray = player.getReses(log, phase.getStart(), phase.getEnd());
                    int[] cleanseArray = player.getCleanses(log, phase.getStart(), phase.getEnd());
                    final.resurrects = resArray[0];
                    final.ressurrectTime = resArray[1]/1000f;
                    final.condiCleanse = cleanseArray[0];
                    final.condiCleanseTime = cleanseArray[1]/1000f;

                    phaseSupport[phaseIndex] = final;
                }
                statistics.support[player] = phaseSupport;
            }
        }

        private Dictionary<int, Statistics.FinalBoonUptime> getBoonsForList(List<Player> playerList, Player player, List<Boon> to_track, int phaseIndex)
        {
            PhaseData phase = phases[phaseIndex];
            long fightDuration = phase.getEnd() - phase.getStart();

            Dictionary<Player, BoonDistribution> boonDistributions = new Dictionary<Player, BoonDistribution>();
            foreach (Player p in playerList)
            {
                boonDistributions[p] = p.getBoonDistribution(log, phases, to_track, phaseIndex);
            }

            Dictionary<int, Statistics.FinalBoonUptime> final =
                new Dictionary<int, Statistics.FinalBoonUptime>();

            foreach (Boon boon in Boon.getAllBuffList())
            {
                long totalGeneration = 0;
                long totalOverstack = 0;
                long totalUptime = 0;

                foreach (BoonDistribution boons in boonDistributions.Values)
                {
                    if (boons.ContainsKey(boon.getID()))
                    {
                        totalGeneration += boons.getGeneration(boon.getID(), player.getInstid());
                        totalOverstack += boons.getOverstack(boon.getID(), player.getInstid());
                        totalUptime += boons.getUptime(boon.getID());
                    }
                }

                Statistics.FinalBoonUptime uptime = new Statistics.FinalBoonUptime();

                if (boon.getType() == Boon.BoonType.Duration)
                {
                    uptime.uptime = Math.Round(100.0 * totalUptime / fightDuration / playerList.Count, 1);
                    uptime.generation = Math.Round(100.0f * totalGeneration / fightDuration / playerList.Count, 1);
                    uptime.overstack = Math.Round(100.0f * totalOverstack / fightDuration / playerList.Count, 1);
                }
                else if (boon.getType() == Boon.BoonType.Intensity)
                {
                    uptime.uptime = Math.Round((double)totalUptime / fightDuration / playerList.Count, 1);
                    uptime.generation = Math.Round((double)totalGeneration / fightDuration / playerList.Count, 1);
                    uptime.overstack = Math.Round((double)totalOverstack / fightDuration / playerList.Count, 1);
                }

                uptime.boonType = boon.getType();

                final[boon.getID()] = uptime;
            }

            return final;
        }

        private void calculateBoons()
        {
            // Player Boons
            foreach (Player player in log.getPlayerList())
            {
                List<Boon> boon_to_track = new List<Boon>();
                boon_to_track.AddRange(statistics.present_boons);
                boon_to_track.AddRange(statistics.present_offbuffs);
                boon_to_track.AddRange(statistics.present_defbuffs);
                boon_to_track.AddRange(statistics.present_personnal[player.getInstid()]);
                Dictionary<int, Statistics.FinalBoonUptime>[] phaseBoons = new Dictionary<int, Statistics.FinalBoonUptime>[phases.Count];
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {
                    Dictionary<int, Statistics.FinalBoonUptime> final = new Dictionary<int, Statistics.FinalBoonUptime>();

                    PhaseData phase = phases[phaseIndex];

                    BoonDistribution selfBoons = player.getBoonDistribution(log, phases, boon_to_track, phaseIndex);

                    long fightDuration = phase.getEnd() - phase.getStart();
                    foreach (Boon boon in Boon.getAllBuffList())
                    {
                        Statistics.FinalBoonUptime uptime = new Statistics.FinalBoonUptime();

                        uptime.uptime = 0;
                        uptime.generation = 0;
                        uptime.overstack = 0;
                        if (selfBoons.ContainsKey(boon.getID()))
                        {
                            if (boon.getType() == Boon.BoonType.Duration)
                            {
                                uptime.uptime = Math.Round(100.0 * selfBoons.getUptime(boon.getID()) / fightDuration, 1);
                                uptime.generation = Math.Round(100.0f * selfBoons.getGeneration(boon.getID(), player.getInstid()) / fightDuration, 1);
                                uptime.overstack = Math.Round(100.0f * selfBoons.getOverstack(boon.getID(), player.getInstid()) / fightDuration, 1);
                            }
                            else if (boon.getType() == Boon.BoonType.Intensity)
                            {
                                uptime.uptime = Math.Round((double)selfBoons.getUptime(boon.getID()) / fightDuration, 1);
                                uptime.generation = Math.Round((double)selfBoons.getGeneration(boon.getID(), player.getInstid()) / fightDuration, 1);
                                uptime.overstack = Math.Round((double)selfBoons.getOverstack(boon.getID(), player.getInstid()) / fightDuration, 1);
                            }

                            uptime.boonType = boon.getType();
                        }
                        final[boon.getID()] = uptime;
                    }

                    phaseBoons[phaseIndex] = final;
                }
                statistics.selfBoons[player] = phaseBoons;
            }

            // Group Boons
            foreach (Player player in log.getPlayerList())
            {
                List<Boon> boon_to_track = new List<Boon>();
                boon_to_track.AddRange(statistics.present_boons);
                boon_to_track.AddRange(statistics.present_offbuffs);
                boon_to_track.AddRange(statistics.present_defbuffs);
                boon_to_track.AddRange(statistics.present_personnal[player.getInstid()]);
                List<Player> groupPlayers = new List<Player>();
                foreach (Player p in log.getPlayerList())
                {
                    if (p.getGroup() == player.getGroup()) groupPlayers.Add(p);
                }
                Dictionary<int, Statistics.FinalBoonUptime>[] phaseBoons = new Dictionary<int, Statistics.FinalBoonUptime>[phases.Count];
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {
                    phaseBoons[phaseIndex] = getBoonsForList(groupPlayers, player, boon_to_track, phaseIndex);
                }
                statistics.groupBoons[player] = phaseBoons;
            }

            // Off Group Boons
            foreach (Player player in log.getPlayerList())
            {
                List<Boon> boon_to_track = new List<Boon>();
                boon_to_track.AddRange(statistics.present_boons);
                boon_to_track.AddRange(statistics.present_offbuffs);
                boon_to_track.AddRange(statistics.present_defbuffs);
                boon_to_track.AddRange(statistics.present_personnal[player.getInstid()]);
                List<Player> groupPlayers = new List<Player>();
                foreach (Player p in log.getPlayerList())
                {
                    if (p.getGroup() != player.getGroup()) groupPlayers.Add(p);
                }
                Dictionary<int, Statistics.FinalBoonUptime>[] phaseBoons = new Dictionary<int, Statistics.FinalBoonUptime>[phases.Count];
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {                    
                    phaseBoons[phaseIndex] = getBoonsForList(groupPlayers, player, boon_to_track, phaseIndex);
                }
                statistics.offGroupBoons[player] = phaseBoons;
            }

            // Squad Boons
            foreach (Player player in log.getPlayerList())
            {
                List<Boon> boon_to_track = new List<Boon>();
                boon_to_track.AddRange(statistics.present_boons);
                boon_to_track.AddRange(statistics.present_offbuffs);
                boon_to_track.AddRange(statistics.present_defbuffs);
                boon_to_track.AddRange(statistics.present_personnal[player.getInstid()]);
                List<Player> groupPlayers = new List<Player>();
                foreach (Player p in log.getPlayerList())
                {
                    groupPlayers.Add(p);
                }
                Dictionary<int, Statistics.FinalBoonUptime>[] phaseBoons = new Dictionary<int, Statistics.FinalBoonUptime>[phases.Count];
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {                
                    phaseBoons[phaseIndex] = getBoonsForList(groupPlayers, player, boon_to_track, phaseIndex);
                }
                statistics.squadBoons[player] = phaseBoons;
            }
        }

        public void calculateConditions()
        {
            statistics.bossConditions = new Dictionary<int, Statistics.FinalBossBoon>[phases.Count];
            List<Boon> boon_to_track = Boon.getCondiBoonList();
            boon_to_track.AddRange(Boon.getBoonList());
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                List<PhaseData> phases = log.getBoss().getPhases(log, settings.ParsePhases);
                BoonDistribution boonDistribution = log.getBoss().getBoonDistribution(log, phases, boon_to_track, phaseIndex);
                Dictionary<int, Statistics.FinalBossBoon> rates = new Dictionary<int, Statistics.FinalBossBoon>();

                PhaseData phase = phases[phaseIndex];
                long fightDuration = phase.getDuration();

                foreach (Boon boon in Boon.getCondiBoonList())
                {
                    Statistics.FinalBossBoon condition = new Statistics.FinalBossBoon();
                    rates[boon.getID()] = condition;
                    if (boonDistribution.ContainsKey(boon.getID()))
                    {
                        if (boon.getType() == Boon.BoonType.Duration)
                        {
                            condition.boonType = Boon.BoonType.Duration;
                            condition.uptime = Math.Round(100.0 * boonDistribution.getUptime(boon.getID()) / fightDuration, 1);
                        }
                        else if (boon.getType() == Boon.BoonType.Intensity)
                        {
                            condition.boonType = Boon.BoonType.Intensity;
                            condition.uptime = Math.Round((double) boonDistribution.getUptime(boon.getID()) / fightDuration, 1);
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
        private void setPresentBoons()
        {
            List<SkillItem> s_list = log.getSkillData().getSkillList();
            if (settings.PlayerBoonsUniversal)
            {//Main boons
                foreach (Boon boon in Boon.getBoonList())
                {
                    if (s_list.Exists(x => x.getID() == boon.getID()))
                    {
                        statistics.present_boons.Add(boon);
                    }
                }
            }
            if (settings.PlayerBoonsImpProf)
            {//Important Class specefic boons
                foreach (Boon boon in Boon.getOffensiveTableList())
                {
                    if (s_list.Exists(x => x.getID() == boon.getID()))
                    {
                        statistics.present_offbuffs.Add(boon);
                    }
                }
                foreach (Boon boon in Boon.getDefensiveTableList())
                {
                    if (s_list.Exists(x => x.getID() == boon.getID()))
                    {
                        statistics.present_defbuffs.Add(boon);
                    }
                }
            }
            List<CombatItem> c_list = log.getCombatData().getCombatList();
            foreach (Player p in log.getPlayerList())
            {
                statistics.present_personnal[p.getInstid()] = new List<Boon>();
                if (settings.PlayerBoonsAllProf)
                {//All class specefic boons
                    foreach (Boon boon in Boon.getRemainingBuffsList())
                    {
                        if (c_list.Exists(x => x.getSkillID() == boon.getID() && x.getDstInstid() == p.getInstid()))
                        {
                            statistics.present_personnal[p.getInstid()].Add(boon);
                        }
                    }
                }
            }
        }
    }
}
