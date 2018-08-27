using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using Newtonsoft.Json;

namespace LuckParser.Controllers
{
    class JSONBuilder
    {
        readonly SettingsContainer _settings;

        readonly ParsedLog _log;

        readonly Statistics _statistics;
        readonly StreamWriter _sw;

        public static void UpdateStatisticSwitches(StatisticsCalculator.Switches switches)
        {
            switches.CalculateBoons = true;
            switches.CalculateDPS = true;
            switches.CalculateConditions = true;
            switches.CalculateDefense = true;
            switches.CalculateStats = true;
            switches.CalculateSupport = true;
            switches.CalculateCombatReplay = true;
            switches.CalculateMechanics = true;
        }

        public JSONBuilder(StreamWriter sw, ParsedLog log, SettingsContainer settings, Statistics statistics)
        {
            _log = log;
            _sw = sw;
            _settings = settings;

            _statistics = statistics;
        }

        /*
         * Creating the JSON
         */
        public void CreateJSON()
        {
            var log = new JsonLog();

            double fightDuration = _log.GetBossData().GetAwareDuration() / 1000.0;
            var duration = TimeSpan.FromSeconds(fightDuration);
            string durationString = duration.ToString("mm") + "m " + duration.ToString("ss") + "s";
            if (duration.ToString("hh") != "00")
            {
                durationString = duration.ToString("hh") + "h " + durationString;
            }

            log.EliteInsightsVersion = Application.ProductVersion;
            log.ArcVersion = _log.GetLogData().GetBuildVersion();
            log.RecordedBy = _log.GetLogData().GetPOV().Split(':')[0].TrimEnd('\u0000');
            log.TimeStart = _log.GetLogData().GetLogStart();
            log.TimeEnd = _log.GetLogData().GetLogEnd();
            log.Duration = durationString;
            log.Success = _log.GetLogData().GetBosskill();
            log.StackCenterPositions = _statistics.StackCenterPositions;

            SetBoss(log);
            SetPlayers(log);
            SetPhases(log);

            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            var writer = new JsonTextWriter(_sw);
            writer.Formatting = Formatting.Indented;
            serializer.Serialize(writer, log);
        }


        private void SetBoss(JsonLog log)
        {
            log.Boss.Id = _log.GetBossData().GetID();
            log.Boss.Name = _log.GetBossData().GetName();
            log.Boss.TotalHealth = _log.GetBossData().GetHealth();
            int finalBossHealth = _log.GetBossData().GetHealthOverTime().Count > 0
                ? _log.GetBossData().GetHealthOverTime().Last().Y
                : 10000;
            log.Boss.FinalHealth = _log.GetBossData().GetHealth() * (100.0 - finalBossHealth * 0.01);
            log.Boss.HealthPercentBurned = 100.0 - finalBossHealth * 0.01;

            log.Boss.Dps = BuildDPS(_statistics.BossDps);
            log.Boss.HealthOverTime = _log.GetBossData().GetHealthOverTime();
            log.Boss.Conditions = BuildBossBoons(_statistics.BossConditions);
        }

        private void SetPlayers(JsonLog log)
        {
            log.Players = new ArrayList();

            foreach (var player in _log.GetPlayerList())
            {
                var currentPlayer = new JsonLog.JsonPlayer
                {
                    Character = player.GetCharacter(),
                    Account = player.GetAccount(),
                    Condition = player.GetCondition(),
                    Concentration = player.GetConcentration(),
                    Healing = player.GetHealing(),
                    Toughness = player.GetToughness(),
                    Weapons = player.GetWeaponsArray(_log).Where(w => w != null).ToArray(),
                    Group = player.GetGroup(),
                    Profession = player.GetProf(),
                    Dps = BuildDPS(_statistics.Dps[player]),
                    Stats = BuildStats(_statistics.Stats[player]),
                    Defenses = BuildDefenses(_statistics.Defenses[player]),
                    Support = BuildSupport(_statistics.Support[player]),
                    SelfBoons = BuildBoonUptime(_statistics.SelfBoons[player]),
                    GroupBoons = BuildBoonUptime(_statistics.GroupBoons[player]),
                    OffGroupBoons = BuildBoonUptime(_statistics.OffGroupBoons[player]),
                    SquadBoons = BuildBoonUptime(_statistics.SquadBoons[player])
                };

                log.Players.Add(currentPlayer);
            }
        }

        private void SetPhases(JsonLog log)
        {
            log.Phases = new ArrayList();

            foreach (var phase in _statistics.Phases)
            {
                log.Phases.Add(new JsonLog.JsonPhase
                {
                    Duration = phase.GetDuration(),
                    Name = phase.GetName()
                });
            }
        }

        // Statistics to Json Converters ////////////////////////////////////////////////////

        private bool ContainsBossBoon(long boon, Dictionary<long, Statistics.FinalBossBoon>[] statBoons)
        {
            int phases = _statistics.Phases.Count;
            for (int phaseIndex = 0; phaseIndex < phases; phaseIndex++)
            {
                if (statBoons[phaseIndex][boon].Uptime > 0) return true;
                if (statBoons[phaseIndex][boon].Generated.Any())
                {
                    foreach (KeyValuePair<Player, double> playerBoon in statBoons[phaseIndex][boon].Generated)
                    {
                        if (playerBoon.Value > 0) return true;
                    }
                }

                if (statBoons[phaseIndex][boon].Overstacked.Any())
                {
                    foreach (KeyValuePair<Player, double> playerBoon in statBoons[phaseIndex][boon].Generated)
                    {
                        if (playerBoon.Value > 0) return true;
                    }
                }
            }

            return false;
        }

        private Dictionary<long, JsonLog.JsonBossBoon> BuildBossBoons(Dictionary<long, Statistics.FinalBossBoon>[] statBoons)
        {
            int phases = _statistics.Phases.Count;
            Dictionary<long, JsonLog.JsonBossBoon> boons = new Dictionary<long, JsonLog.JsonBossBoon>();

            List<long> boonsFound = new List<long>();
            List<long> boonsNotFound = new List<long>();

            for (int phaseIndex = 0; phaseIndex < phases; phaseIndex++)
            {
                foreach (KeyValuePair<long, Statistics.FinalBossBoon> boon in statBoons[phaseIndex])
                {
                    if (boonsFound.Contains(boon.Key))
                    {
                        boons[boon.Key].Uptime[phaseIndex] = boon.Value.Uptime;

                        if (boons[boon.Key].Generated[phaseIndex] == null)
                        {
                            boons[boon.Key].Generated[phaseIndex] = new Dictionary<string, double>();
                        }

                        foreach (KeyValuePair<Player, double> playerBoon in boon.Value.Generated)
                        {
                            if (playerBoon.Value != 0)
                            {
                                boons[boon.Key].Generated[phaseIndex][playerBoon.Key.GetCharacter()] = playerBoon.Value;
                            }
                        }

                        if (boons[boon.Key].Overstacked[phaseIndex] == null)
                        {
                            boons[boon.Key].Overstacked[phaseIndex] = new Dictionary<string, double>();
                        }

                        foreach (KeyValuePair<Player, double> playerBoon in boon.Value.Overstacked)
                        {
                            if (playerBoon.Value != 0)
                            {
                                boons[boon.Key].Overstacked[phaseIndex][playerBoon.Key.GetCharacter()] = playerBoon.Value;
                            }
                        }
                    }
                    else if (!boonsNotFound.Contains(boon.Key))
                    {
                        if (ContainsBossBoon(boon.Key, statBoons))
                        {
                            boonsFound.Add(boon.Key);

                            JsonLog.JsonBossBoon newBoon = new JsonLog.JsonBossBoon
                            {
                                Uptime = new double[phases],
                                Generated = new Dictionary<string, double>[phases],
                                Overstacked = new Dictionary<string, double>[phases]
                            };
                            boons[boon.Key] = newBoon;

                            boons[boon.Key].Uptime[phaseIndex] = boon.Value.Uptime;

                            if (boons[boon.Key].Generated[phaseIndex] == null)
                            {
                                boons[boon.Key].Generated[phaseIndex] = new Dictionary<string, double>();
                            }

                            foreach (KeyValuePair<Player, double> playerBoon in boon.Value.Generated)
                            {
                                if (playerBoon.Value != 0)
                                {
                                    boons[boon.Key].Generated[phaseIndex][playerBoon.Key.GetCharacter()] = playerBoon.Value;
                                }
                            }

                            if (boons[boon.Key].Overstacked[phaseIndex] == null)
                            {
                                boons[boon.Key].Overstacked[phaseIndex] = new Dictionary<string, double>();
                            }

                            foreach (KeyValuePair<Player, double> playerBoon in boon.Value.Overstacked)
                            {
                                if (playerBoon.Value != 0)
                                {
                                    boons[boon.Key].Overstacked[phaseIndex][playerBoon.Key.GetCharacter()] = playerBoon.Value;
                                }
                            }
                        }
                        else
                        {
                            boonsNotFound.Add(boon.Key);
                        }
                    }
                }
            }

            return boons;
        }

        private JsonLog.JsonDps BuildDPS(Statistics.FinalDPS[] statDps)
        {
            int phases = _statistics.Phases.Count;
            JsonLog.JsonDps dps = new JsonLog.JsonDps
            {
                AllCondiDamage = new int[phases],
                AllCondiDps = new int[phases],
                AllDamage = new int[phases],
                AllDps = new int[phases],
                AllPowerDamage = new int[phases],
                AllPowerDps = new int[phases],
                BossCondiDamage = new int[phases],
                BossCondiDps = new int[phases],
                BossDamage = new int[phases],
                BossDps = new int[phases],
                BossPowerDamage = new int[phases],
                BossPowerDps = new int[phases],
                PlayerBossPowerDamage = new int[phases],
                PlayerPowerDamage = new int[phases]
            };

            for (int phaseIndex = 0; phaseIndex < phases; phaseIndex++)
            {
                dps.AllDps[phaseIndex] = statDps[phaseIndex].AllDps;
                dps.AllDamage[phaseIndex] = statDps[phaseIndex].AllDamage;
                dps.AllPowerDps[phaseIndex] = statDps[phaseIndex].AllPowerDps;
                dps.AllCondiDamage[phaseIndex] = statDps[phaseIndex].AllCondiDamage;
                dps.AllCondiDps[phaseIndex] = statDps[phaseIndex].AllCondiDps;
                dps.AllPowerDamage[phaseIndex] = statDps[phaseIndex].AllPowerDamage;
                dps.BossCondiDamage[phaseIndex] = statDps[phaseIndex].BossCondiDamage;
                dps.BossPowerDamage[phaseIndex] = statDps[phaseIndex].BossPowerDamage;
                dps.BossCondiDps[phaseIndex] = statDps[phaseIndex].BossCondiDps;
                dps.BossPowerDps[phaseIndex] = statDps[phaseIndex].BossPowerDps;
                dps.BossDamage[phaseIndex] = statDps[phaseIndex].BossDamage;
                dps.BossDps[phaseIndex] = statDps[phaseIndex].BossDps;
                dps.PlayerPowerDamage[phaseIndex] = statDps[phaseIndex].PlayerPowerDamage;
                dps.PlayerBossPowerDamage[phaseIndex] = statDps[phaseIndex].PlayerBossPowerDamage;
            }

            if (!dps.AllDps.Any(e => e > 0)) dps.AllDps = null;
            if (!dps.AllDamage.Any(e => e > 0)) dps.AllDamage = null;
            if (!dps.AllPowerDps.Any(e => e > 0)) dps.AllPowerDps = null;
            if (!dps.AllCondiDamage.Any(e => e > 0)) dps.AllCondiDamage = null;
            if (!dps.AllCondiDps.Any(e => e > 0)) dps.AllCondiDps = null;
            if (!dps.AllPowerDamage.Any(e => e > 0)) dps.AllPowerDamage = null;
            if (!dps.BossCondiDamage.Any(e => e > 0)) dps.BossCondiDamage = null;
            if (!dps.BossPowerDamage.Any(e => e > 0)) dps.BossPowerDamage = null;
            if (!dps.BossCondiDps.Any(e => e > 0)) dps.BossCondiDps = null;
            if (!dps.BossPowerDps.Any(e => e > 0)) dps.BossPowerDps = null;
            if (!dps.BossDamage.Any(e => e > 0)) dps.BossDamage = null;
            if (!dps.BossDps.Any(e => e > 0)) dps.BossDps = null;
            if (!dps.PlayerPowerDamage.Any(e => e > 0)) dps.PlayerPowerDamage = null;
            if (!dps.PlayerBossPowerDamage.Any(e => e > 0)) dps.PlayerBossPowerDamage = null;

            return dps;
        }

        private bool ContainsBoon(long boon, Dictionary<long, Statistics.FinalBoonUptime>[] statUptimes)
        {
            int phases = _statistics.Phases.Count;
            for (int phaseIndex = 0; phaseIndex < phases; phaseIndex++)
            {
                if (statUptimes[phaseIndex][boon].Uptime > 0) return true;
                if (statUptimes[phaseIndex][boon].Generation > 0) return true;
                if (statUptimes[phaseIndex][boon].Overstack > 0) return true;
            }

            return false;
        }

        private Dictionary<long, JsonLog.JsonBoonUptime> BuildBoonUptime(Dictionary<long, Statistics.FinalBoonUptime>[] statUptimes)
        {
            Dictionary<long, JsonLog.JsonBoonUptime> uptimes = new Dictionary<long, JsonLog.JsonBoonUptime>();
            int phases = _statistics.Phases.Count;

            List<long> boonsFound = new List<long>();
            List<long> boonsNotFound = new List<long>();

            for (int phaseIndex = 0; phaseIndex < phases; phaseIndex++)
            {
                foreach (KeyValuePair<long, Statistics.FinalBoonUptime> boon in statUptimes[phaseIndex])
                {
                    if (boonsFound.Contains(boon.Key))
                    {
                        uptimes[boon.Key].Overstack[phaseIndex] = boon.Value.Overstack;
                        uptimes[boon.Key].Generation[phaseIndex] = boon.Value.Generation;
                        uptimes[boon.Key].Uptime[phaseIndex] = boon.Value.Uptime;
                    }
                    else if (!boonsNotFound.Contains(boon.Key))
                    {
                        if (ContainsBoon(boon.Key, statUptimes))
                        {
                            boonsFound.Add(boon.Key);

                            JsonLog.JsonBoonUptime newUptime = new JsonLog.JsonBoonUptime
                            {
                                Generation = new double[phases],
                                Overstack = new double[phases],
                                Uptime = new double[phases]
                            };
                            uptimes[boon.Key] = newUptime;

                            uptimes[boon.Key].Overstack[phaseIndex] = boon.Value.Overstack;
                            uptimes[boon.Key].Generation[phaseIndex] = boon.Value.Generation;
                            uptimes[boon.Key].Uptime[phaseIndex] = boon.Value.Uptime;
                        }
                        else
                        {
                            boonsNotFound.Add(boon.Key);
                        }
                    }
                }
            }

            if (!uptimes.Any())
            {
                return null;
            }

            return uptimes;
        }

        private JsonLog.JsonSupport BuildSupport(Statistics.FinalSupport[] statSupport)
        {
            int phases = _statistics.Phases.Count;
            JsonLog.JsonSupport support = new JsonLog.JsonSupport
            {
                CondiCleanse = new int[phases],
                CondiCleanseTime = new float[phases],
                ResurrectTime = new float[phases],
                Resurrects = new int[phases]
            };

            for (int phaseIndex = 0; phaseIndex < phases; phaseIndex++)
            {
                support.Resurrects[phaseIndex] = statSupport[phaseIndex].Resurrects;
                support.ResurrectTime[phaseIndex] = statSupport[phaseIndex].ResurrectTime;
                support.CondiCleanse[phaseIndex] = statSupport[phaseIndex].CondiCleanse;
                support.CondiCleanseTime[phaseIndex] = statSupport[phaseIndex].CondiCleanseTime;
            }

            if (!support.Resurrects.Any(e => e > 0)) support.Resurrects = null;
            if (!support.ResurrectTime.Any(e => e > 0)) support.ResurrectTime = null;
            if (!support.CondiCleanse.Any(e => e > 0)) support.CondiCleanse = null;
            if (!support.CondiCleanseTime.Any(e => e > 0)) support.CondiCleanseTime = null;

            return support;
        }

        private JsonLog.JsonDefenses BuildDefenses(Statistics.FinalDefenses[] statDefense)
        {
            int phases = _statistics.Phases.Count;
            JsonLog.JsonDefenses defense = new JsonLog.JsonDefenses
            {
                BlockedCount = new int[phases],
                DamageBarrier = new int[phases],
                DamageInvulned = new int[phases],
                DamageTaken = new long[phases],
                EvadedCount = new int[phases],
                InvulnedCount = new int[phases]
            };

            for (int phaseIndex = 0; phaseIndex < phases; phaseIndex++)
            {
                defense.EvadedCount[phaseIndex] = statDefense[phaseIndex].EvadedCount;
                defense.InvulnedCount[phaseIndex] = statDefense[phaseIndex].InvulnedCount;
                defense.DamageTaken[phaseIndex] = statDefense[phaseIndex].DamageTaken;
                defense.DamageInvulned[phaseIndex] = statDefense[phaseIndex].DamageInvulned;
                defense.DamageBarrier[phaseIndex] = statDefense[phaseIndex].DamageBarrier;
                defense.BlockedCount[phaseIndex] = statDefense[phaseIndex].BlockedCount;
            }

            if (!defense.EvadedCount.Any(e => e > 0)) defense.EvadedCount = null;
            if (!defense.InvulnedCount.Any(e => e > 0)) defense.InvulnedCount = null;
            if (!defense.DamageTaken.Any(e => e > 0)) defense.DamageTaken = null;
            if (!defense.DamageInvulned.Any(e => e > 0)) defense.DamageInvulned = null;
            if (!defense.DamageBarrier.Any(e => e > 0)) defense.DamageBarrier = null;
            if (!defense.BlockedCount.Any(e => e > 0)) defense.BlockedCount = null;

            return defense;
        }

        private JsonLog.JsonStats BuildStats(Statistics.FinalStats[] statStat)
        {
            int phases = _statistics.Phases.Count;
            JsonLog.JsonStats stats = new JsonLog.JsonStats
            {
                AvgBoons = new double[phases],
                CritablePowerLoopCount = new int[phases],
                CritablePowerLoopCountBoss = new int[phases],
                CriticalDmg = new int[phases],
                CriticalDmgBoss = new int[phases],
                CriticalRate = new int[phases],
                CriticalRateBoss = new int[phases],
                Dcd = new double[phases],
                Died = new double[phases],
                DodgeCount = new int[phases],
                DownCount = new int[phases],
                FlankingRate = new int[phases],
                FlankingRateBoss = new int[phases],
                GlanceRate = new int[phases],
                GlanceRateBoss = new int[phases],
                Interrupts = new int[phases],
                InterruptsBoss = new int[phases],
                Invulned = new int[phases],
                InvulnedBoss = new int[phases],
                Missed = new int[phases],
                MissedBoss = new int[phases],
                MovingDamage = new int[phases],
                MovingDamageBoss = new int[phases],
                MovingRate = new int[phases],
                MovingRateBoss = new int[phases],
                PowerLoopCount = new int[phases],
                PowerLoopCountBoss = new int[phases],
                Saved = new int[phases],
                ScholarDmg = new int[phases],
                ScholarDmgBoss = new int[phases],
                ScholarRate = new int[phases],
                ScholarRateBoss = new int[phases],
                StackDist = new double[phases],
                SwapCount = new int[phases],
                TimeSaved = new double[phases],
                TimeWasted = new double[phases],
                Wasted = new int[phases]
            };

            for (int phaseIndex = 0; phaseIndex < phases; phaseIndex++)
            {
                stats.PowerLoopCount[phaseIndex] = statStat[phaseIndex].PowerLoopCount;
                stats.CritablePowerLoopCount[phaseIndex] = statStat[phaseIndex].CritablePowerLoopCount;
                stats.CriticalRate[phaseIndex] = statStat[phaseIndex].CriticalRate;
                stats.CriticalDmg[phaseIndex] = statStat[phaseIndex].CriticalDmg;
                stats.ScholarRate[phaseIndex] = statStat[phaseIndex].ScholarRate;
                stats.ScholarDmg[phaseIndex] = statStat[phaseIndex].ScholarDmg;
                stats.MovingRate[phaseIndex] = statStat[phaseIndex].MovingRate;
                stats.MovingDamage[phaseIndex] = statStat[phaseIndex].MovingDamage;
                stats.FlankingRate[phaseIndex] = statStat[phaseIndex].FlankingRate;
                stats.GlanceRate[phaseIndex] = statStat[phaseIndex].GlanceRate;
                stats.Missed[phaseIndex] = statStat[phaseIndex].Missed;
                stats.Interrupts[phaseIndex] = statStat[phaseIndex].Interrupts;
                stats.Invulned[phaseIndex] = statStat[phaseIndex].Invulned;
                stats.Wasted[phaseIndex] = statStat[phaseIndex].Wasted;
                stats.TimeWasted[phaseIndex] = statStat[phaseIndex].TimeWasted;
                stats.Saved[phaseIndex] = statStat[phaseIndex].Saved;
                stats.TimeSaved[phaseIndex] = statStat[phaseIndex].TimeSaved;
                stats.AvgBoons[phaseIndex] = statStat[phaseIndex].AvgBoons;
                stats.StackDist[phaseIndex] = statStat[phaseIndex].StackDist;
                stats.PowerLoopCountBoss[phaseIndex] = statStat[phaseIndex].PowerLoopCountBoss;
                stats.CritablePowerLoopCountBoss[phaseIndex] = statStat[phaseIndex].CritablePowerLoopCountBoss;
                stats.CriticalRateBoss[phaseIndex] = statStat[phaseIndex].CriticalRateBoss;
                stats.CriticalDmgBoss[phaseIndex] = statStat[phaseIndex].CriticalDmgBoss;
                stats.ScholarRateBoss[phaseIndex] = statStat[phaseIndex].ScholarRateBoss;
                stats.ScholarDmgBoss[phaseIndex] = statStat[phaseIndex].ScholarDmgBoss;
                stats.MovingRateBoss[phaseIndex] = statStat[phaseIndex].MovingRateBoss;
                stats.MovingDamageBoss[phaseIndex] = statStat[phaseIndex].MovingDamageBoss;
                stats.FlankingRateBoss[phaseIndex] = statStat[phaseIndex].FlankingRateBoss;
                stats.GlanceRateBoss[phaseIndex] = statStat[phaseIndex].GlanceRateBoss;
                stats.MissedBoss[phaseIndex] = statStat[phaseIndex].MissedBoss;
                stats.InterruptsBoss[phaseIndex] = statStat[phaseIndex].InterruptsBoss;
                stats.InvulnedBoss[phaseIndex] = statStat[phaseIndex].InvulnedBoss;
                stats.SwapCount[phaseIndex] = statStat[phaseIndex].SwapCount;
                stats.DownCount[phaseIndex] = statStat[phaseIndex].DownCount;
                stats.DodgeCount[phaseIndex] = statStat[phaseIndex].DodgeCount;
                stats.Died[phaseIndex] = statStat[phaseIndex].Died;
                stats.Dcd[phaseIndex] = statStat[phaseIndex].Dcd;
            }

            if (!stats.Dcd.Any(e => e > 0)) stats.Dcd = null;
            if (!stats.Died.Any(e => e > 0)) stats.Died = null;
            if (!stats.DownCount.Any(e => e > 0)) stats.DownCount = null;
            if (!stats.SwapCount.Any(e => e > 0)) stats.SwapCount = null;
            if (!stats.InvulnedBoss.Any(e => e > 0)) stats.InvulnedBoss = null;
            if (!stats.InterruptsBoss.Any(e => e > 0)) stats.InterruptsBoss = null;
            if (!stats.MissedBoss.Any(e => e > 0)) stats.MissedBoss = null;
            if (!stats.GlanceRateBoss.Any(e => e > 0)) stats.GlanceRateBoss = null;
            if (!stats.FlankingRateBoss.Any(e => e > 0)) stats.FlankingRateBoss = null;
            if (!stats.MovingDamageBoss.Any(e => e > 0)) stats.MovingDamageBoss = null;
            if (!stats.MovingRateBoss.Any(e => e > 0)) stats.MovingRateBoss = null;
            if (!stats.ScholarDmgBoss.Any(e => e > 0)) stats.ScholarDmgBoss = null;
            if (!stats.ScholarRateBoss.Any(e => e > 0)) stats.ScholarRateBoss = null;
            if (!stats.CriticalDmgBoss.Any(e => e > 0)) stats.CriticalDmgBoss = null;
            if (!stats.CriticalRateBoss.Any(e => e > 0)) stats.CriticalRateBoss = null;
            if (!stats.CritablePowerLoopCountBoss.Any(e => e > 0)) stats.CritablePowerLoopCountBoss = null;
            if (!stats.PowerLoopCountBoss.Any(e => e > 0)) stats.PowerLoopCountBoss = null;
            if (!stats.StackDist.Any(e => e > 0)) stats.StackDist = null;
            if (!stats.AvgBoons.Any(e => e > 0)) stats.AvgBoons = null;
            if (!stats.TimeSaved.Any(e => e > 0)) stats.TimeSaved = null;
            if (!stats.Saved.Any(e => e > 0)) stats.Saved = null;
            if (!stats.TimeWasted.Any(e => e > 0)) stats.TimeWasted = null;
            if (!stats.Wasted.Any(e => e > 0)) stats.Wasted = null;
            if (!stats.Invulned.Any(e => e > 0)) stats.Invulned = null;
            if (!stats.Interrupts.Any(e => e > 0)) stats.Interrupts = null;
            if (!stats.Missed.Any(e => e > 0)) stats.Missed = null;
            if (!stats.GlanceRate.Any(e => e > 0)) stats.GlanceRate = null;
            if (!stats.FlankingRate.Any(e => e > 0)) stats.FlankingRate = null;
            if (!stats.MovingDamage.Any(e => e > 0)) stats.MovingDamage = null;
            if (!stats.MovingRate.Any(e => e > 0)) stats.MovingRate = null;
            if (!stats.ScholarDmg.Any(e => e > 0)) stats.ScholarDmg = null;
            if (!stats.ScholarRate.Any(e => e > 0)) stats.ScholarRate = null;
            if (!stats.CriticalDmg.Any(e => e > 0)) stats.CriticalDmg = null;
            if (!stats.CriticalRate.Any(e => e > 0)) stats.CriticalRate = null;
            if (!stats.CritablePowerLoopCount.Any(e => e > 0)) stats.CritablePowerLoopCount = null;
            if (!stats.PowerLoopCount.Any(e => e > 0)) stats.PowerLoopCount = null;

            return stats;
        }
    }
}