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

        public static List<Point> getDPSGraph(ParsedLog log, AbstractMasterPlayer p, int phase_index, ushort dstid, GraphMode mode)
        {
            int asked_id = (phase_index + "_" + dstid + "_" + mode).GetHashCode();
            if (p.getDPSGraph(asked_id).Count > 0)
            {
                return p.getDPSGraph(asked_id);
            }

            List<Point> dmgList = new List<Point>();
            List<Point> dmgList10s = new List<Point>();
            List<Point> dmgList30s = new List<Point>();
            PhaseData phase = log.getBoss().getPhases(log, settings.ParsePhases)[phase_index];
            List<DamageLog> damage_logs = p.getDamageLogs(dstid, log, phase.getStart(), phase.getEnd());
            // fill the graph, full precision
            List<double> dmgListFull = new List<double>();
            for (int i = 0; i <= phase.getDuration(); i++)
            {
                dmgListFull.Add(0.0);
            }
            int total_time = 1;
            int total_damage = 0;
            foreach (DamageLog dl in damage_logs)
            {
                int time = (int)(dl.getTime() - phase.getStart());
                // fill
                for (; total_time < time; total_time++)
                {
                    dmgListFull[total_time] = total_damage;
                }
                total_damage += dl.getDamage();
                dmgListFull[total_time] = total_damage;
            }
            // fill
            for (; total_time <= phase.getDuration(); total_time++)
            {
                dmgListFull[total_time] = total_damage;
            }
            CombatReplay replay = p.getCombatReplay();
            if (replay != null && dstid == 0 && phase_index == 0)
            {
                foreach (int i in replay.getTimes())
                {
                    int limit_id = 0;
                    replay.addDPS((int)Math.Round(1000 * (dmgListFull[i] - dmgListFull[limit_id]) / (i - limit_id)));
                    if (settings.Show10s)
                    {
                        limit_id = Math.Max(i - 10000, 0);
                        replay.addDPS10s((int)Math.Round(1000 * (dmgListFull[i] - dmgListFull[limit_id]) / (i - limit_id)));
                    }
                    if (settings.Show30s)
                    {
                        limit_id = Math.Max(i - 30000, 0);
                        replay.addDPS30s((int)Math.Round(1000 * (dmgListFull[i] - dmgListFull[limit_id]) / (i - limit_id)));
                    }
                }
            }
            dmgList.Add(new Point(0, 0));
            dmgList10s.Add(new Point(0, 0));
            dmgList30s.Add(new Point(0, 0));
            for (int i = 1; i <= phase.getDuration("s"); i++)
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
            p.addDPSGraph(id, dmgList);
            if (settings.Show10s)
            {
                id = (phase_index + "_" + dstid + "_" + GraphMode.s10).GetHashCode();
                p.addDPSGraph(id, dmgList10s);
            }
            if (settings.Show30s)
            {
                id = (phase_index + "_" + dstid + "_" + GraphMode.s30).GetHashCode();
                p.addDPSGraph(id, dmgList30s);
            }
            return p.getDPSGraph(asked_id);
        }
        /// <summary>
        /// Gets the points for the boss dps graph for a given player
        /// </summary>
        /// <param name="p">The player</param>
        /// <returns></returns>
        public static List<Point> getBossDPSGraph(ParsedLog log, AbstractMasterPlayer p, int phase_index, GraphMode mode)
        {
            return getDPSGraph(log, p, phase_index, log.getBossData().getInstid(), mode);
        }

        /// <summary>
        /// Gets the points for the total dps graph for a given player
        /// </summary>
        /// <param name="p">The player</param>
        /// <returns></returns>
        public static List<Point> getTotalDPSGraph(ParsedLog log, AbstractMasterPlayer p, int phase_index, GraphMode mode)
        {
            return getDPSGraph(log, p, phase_index, 0, mode);
        }
    }
}
