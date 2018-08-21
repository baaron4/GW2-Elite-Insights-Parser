using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class BossLogic
    {

        public enum ParseMode { Raid, Fractal, Golem, Unknown };

        protected List<Mechanic> mechanicList = new List<Mechanic> {
            new Mechanic(-2, "Deads", Mechanic.MechType.PlayerStatus, ParseEnum.BossIDS.Unknown, "symbol:'x',color:'rgb(0,0,0)',", "Deads",0),
            new Mechanic(-3, "Downs", Mechanic.MechType.PlayerStatus, ParseEnum.BossIDS.Unknown, "symbol:'cross',color:'rgb(255,0,0)',", "Downs",0),
            new Mechanic(1066, "Resurrect", Mechanic.MechType.PlayerStatus, ParseEnum.BossIDS.Unknown, "symbol:'circle-dot',color:'rgb(0,120,180)',", "Resurrect",0)};
        protected ParseMode mode;

        public BossLogic()
        {
            mode = ParseMode.Unknown;
        }

        public virtual CombatReplayMap GetCombatMap()
        {
            return null;
        }

        protected List<PhaseData> GetInitialPhase(ParsedLog log)
        {
            List<PhaseData> phases = new List<PhaseData>();
            long fight_dur = log.GetBossData().GetAwareDuration();
            phases.Add(new PhaseData(0, fight_dur));
            phases[0].SetName("Full Fight");
            return phases;
        }

        public virtual List<PhaseData> GetPhases(Boss boss, ParsedLog log, List<CastLog> cast_logs)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            return phases;
        }

        public virtual List<ParseEnum.ThrashIDS> GetAdditionalData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>();
            return ids;
        }

        public virtual int IsCM(List<CombatItem> clist, int health)
        {
            return -1;
        }

        public virtual void GetAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {
            return;
        }

        public virtual string GetReplayIcon()
        {
            return "";
        }

        public List<Mechanic> GetMechanics()
        {
            return mechanicList;
        }
       
        public ParseMode GetMode()
        {
            return mode;
        }
        //
        protected static List<CombatItem> GetFilteredList(ParsedLog log, long skillID, ushort instid)
        {
            bool needStart = true;
            List<CombatItem> main = log.GetBoonData().Where(x => x.GetSkillID() == skillID && ((x.GetDstInstid() == instid && x.IsBuffremove() == ParseEnum.BuffRemove.None) || (x.GetSrcInstid() == instid && x.IsBuffremove() != ParseEnum.BuffRemove.None))).ToList();
            List<CombatItem> filtered = new List<CombatItem>();
            for (int i = 0; i < main.Count; i++)
            {
                CombatItem c = main[i];
                if (needStart && c.IsBuffremove() == ParseEnum.BuffRemove.None)
                {
                    needStart = false;
                    filtered.Add(c);
                }
                else if (!needStart && c.IsBuffremove() != ParseEnum.BuffRemove.None)
                {
                    // consider only last remove event before another application
                    if ((i == main.Count - 1) || (i < main.Count - 1 && main[i + 1].IsBuffremove() == ParseEnum.BuffRemove.None))
                    {
                        needStart = true;
                        filtered.Add(c);
                    }
                }
            }
            return filtered;
        }
    }
}
