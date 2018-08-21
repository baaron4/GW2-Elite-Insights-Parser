using System;
using System.Collections.Generic;
using System.Drawing;
using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;

namespace LuckParser.Controllers
{
    class GraphHelper
    {
        public static SettingsContainer settings;

        public enum GraphMode { Full, s10, s30 };

        public static List<Point> GetDPSGraph(ParsedLog log, AbstractMasterPlayer p, int phase_index, PhaseData phase, ushort dstid, GraphMode mode)
        {
            int asked_id = (phase_index + "_" + dstid + "_" + mode).GetHashCode();
            if (p.GetDPSGraph(asked_id).Count > 0)
            {
                return p.GetDPSGraph(asked_id);
            }

            List<Point> dmgList = new List<Point>();
            List<Point> dmgList10s = new List<Point>();
            List<Point> dmgList30s = new List<Point>();
            List<DamageLog> damage_logs;
            if (dstid != 0 && phase.GetRedirection().Count > 0)
            {      
                damage_logs = p.GetDamageLogs(phase.GetRedirection(), log, phase.GetStart(), phase.GetEnd());
            } else
            {
                damage_logs = p.GetDamageLogs(dstid, log, phase.GetStart(), phase.GetEnd());
            }
            // fill the graph, full precision
            List<double> dmgListFull = new List<double>();
            for (int i = 0; i <= phase.GetDuration(); i++)
            {
                dmgListFull.Add(0.0);
            }
            int total_time = 1;
            int total_damage = 0;
            foreach (DamageLog dl in damage_logs)
            {
                int time = (int)(dl.GetTime() - phase.GetStart());
                // fill
                for (; total_time < time; total_time++)
                {
                    dmgListFull[total_time] = total_damage;
                }
                total_damage += dl.GetDamage();
                dmgListFull[total_time] = total_damage;
            }
            // fill
            for (; total_time <= phase.GetDuration(); total_time++)
            {
                dmgListFull[total_time] = total_damage;
            }
            CombatReplay replay = p.GetCombatReplay();
            if (replay != null && dstid == 0 && phase_index == 0)
            {
                foreach (int i in replay.GetTimes())
                {
                    int limit_id = 0;
                    replay.AddDPS((int)Math.Round(1000 * (dmgListFull[i] - dmgListFull[limit_id]) / (i - limit_id)));
                    if (settings.Show10s)
                    {
                        limit_id = Math.Max(i - 10000, 0);
                        replay.AddDPS10s((int)Math.Round(1000 * (dmgListFull[i] - dmgListFull[limit_id]) / (i - limit_id)));
                    }
                    if (settings.Show30s)
                    {
                        limit_id = Math.Max(i - 30000, 0);
                        replay.AddDPS30s((int)Math.Round(1000 * (dmgListFull[i] - dmgListFull[limit_id]) / (i - limit_id)));
                    }
                }
            }
            dmgList.Add(new Point(0, 0));
            dmgList10s.Add(new Point(0, 0));
            dmgList30s.Add(new Point(0, 0));
            for (int i = 1; i <= phase.GetDuration("s"); i++)
            {
                int limit_id = 0;
                dmgList.Add(new Point(i, (int)Math.Round((dmgListFull[1000 * i] - dmgListFull[1000 * limit_id]) / (i - limit_id))));
                if (settings.Show10s)
                {
                    limit_id = Math.Max(i - 10, 0);
                    dmgList10s.Add(new Point(i, (int)Math.Round((dmgListFull[1000 * i] - dmgListFull[1000 * limit_id]) / (i - limit_id))));
                }
                if (settings.Show30s)
                {
                    limit_id = Math.Max(i - 30, 0);
                    dmgList30s.Add(new Point(i, (int)Math.Round((dmgListFull[1000 * i] - dmgListFull[1000 * limit_id]) / (i - limit_id))));
                }
            }
            int id = (phase_index + "_" + dstid + "_" + GraphMode.Full).GetHashCode();
            p.AddDPSGraph(id, dmgList);
            if (settings.Show10s)
            {
                id = (phase_index + "_" + dstid + "_" + GraphMode.s10).GetHashCode();
                p.AddDPSGraph(id, dmgList10s);
            }
            if (settings.Show30s)
            {
                id = (phase_index + "_" + dstid + "_" + GraphMode.s30).GetHashCode();
                p.AddDPSGraph(id, dmgList30s);
            }
            return p.GetDPSGraph(asked_id);
        }
        /// <summary>
        /// Gets the points for the boss dps graph for a given player
        /// </summary>
        /// <param name="p">The player</param>
        /// <returns></returns>
        public static List<Point> GetBossDPSGraph(ParsedLog log, AbstractMasterPlayer p, int phase_index, PhaseData phase, GraphMode mode)
        {
            return GetDPSGraph(log, p, phase_index, phase, log.GetBossData().GetInstid(), mode);
        }

        /// <summary>
        /// Gets the points for the total dps graph for a given player
        /// </summary>
        /// <param name="p">The player</param>
        /// <returns></returns>
        public static List<Point> GetTotalDPSGraph(ParsedLog log, AbstractMasterPlayer p, int phase_index, PhaseData phase, GraphMode mode)
        {
            return GetDPSGraph(log, p, phase_index, phase, 0, mode);
        }

        /// <summary>
        /// Gets the points for the cleave dps graph for a given player
        /// </summary>
        /// <param name="p">The player</param>
        /// <returns></returns>
        public static List<Point> GetCleaveDPSGraph(ParsedLog log, AbstractMasterPlayer p, int phase_index, PhaseData phase, GraphMode mode)
        {           
            int asked_id = (phase_index + "_" + (-1) + "_" + mode).GetHashCode();
            if (p.GetDPSGraph(asked_id).Count > 0)
            {
                return p.GetDPSGraph(asked_id);
            }
            List<Point> total = GetTotalDPSGraph(log, p, phase_index, phase, mode);
            List<Point> boss = GetBossDPSGraph(log, p, phase_index, phase, mode);
            List<Point> cleave = new List<Point>();
            for (int i = 0; i < boss.Count; i++)
            {
                cleave.Add(new Point(boss[i].X, total[i].Y - boss[i].Y));
            }
            p.AddDPSGraph(asked_id, cleave);
            return cleave;
        }
    }
}
