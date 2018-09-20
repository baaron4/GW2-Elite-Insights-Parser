using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class BossLogic
    {

        public enum ParseMode { Raid, Fractal, Golem, Unknown };

        protected readonly List<Mechanic> MechanicList = new List<Mechanic> {
            new Mechanic(-2, "Dead", Mechanic.MechType.PlayerStatus, ParseEnum.BossIDS.Unknown, "symbol:'x',color:'rgb(0,0,0)',", "Dead",0),
            new Mechanic(-3, "Downed", Mechanic.MechType.PlayerStatus, ParseEnum.BossIDS.Unknown, "symbol:'cross',color:'rgb(255,0,0)',", "Downed",0),
            new Mechanic(SkillItem.ResurrectId, "Resurrect", Mechanic.MechType.PlayerStatus, ParseEnum.BossIDS.Unknown, "symbol:'cross-open',color:'rgb(0,255,255)',", "Res",0)}; //Resurrects (start), Resurrect
        protected ParseMode Mode;
        public bool CanCombatReplay { get; set; }
        public string Extension { get; protected set; }
        public string IconUrl { get; protected set; }

        public BossLogic()
        {
            Mode = ParseMode.Unknown;
            CanCombatReplay = false;
            IconUrl = "https://wiki.guildwars2.com/images/d/d2/Guild_emblem_004.png";
            Extension = "boss";
        }

        public virtual CombatReplayMap GetCombatMap()
        {
            return null;
        }

        protected List<PhaseData> GetInitialPhase(ParsedLog log)
        {
            List<PhaseData> phases = new List<PhaseData>();
            long fightDuration = log.FightData.FightDuration;
            phases.Add(new PhaseData(0, fightDuration));
            phases[0].Name = "Full Fight";
            return phases;
        }

        public virtual List<PhaseData> GetPhases(Boss boss, ParsedLog log, List<CastLog> castLogs)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            return phases;
        }

        public virtual List<ParseEnum.TrashIDS> GetAdditionalBossData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            List<ParseEnum.TrashIDS> ids = new List<ParseEnum.TrashIDS>();
            return ids;
        }

        public virtual int IsCM(List<CombatItem> clist, int health)
        {
            return -1;
        }

        public virtual void GetAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {
        }

        protected void SetSuccessByDeath(CombatData combatData, LogData logData, FightData fightData, List<Player> pList)
        {
            CombatItem killed = combatData.GetStatesData(ParseEnum.StateChange.ChangeDead).LastOrDefault(x => x.SrcInstid == fightData.InstID);
            if (killed != null)
            {
                logData.Success = true;
                fightData.FightEnd = killed.Time;
            }
        }

        public virtual void SetSuccess(CombatData combatData, LogData logData, FightData fightData, List<Player> pList)
        {
            SetSuccessByDeath(combatData,logData, fightData,pList);
        }

        public virtual string GetReplayIcon()
        {
            return "";
        }

        public List<Mechanic> GetMechanics()
        {
            return MechanicList;
        }
       
        public ParseMode GetMode()
        {
            return Mode;
        }
        //
        protected static List<CombatItem> GetFilteredList(ParsedLog log, long skillID, ushort instid)
        {
            bool needStart = true;
            List<CombatItem> main = log.GetBoonData(skillID).Where(x => ((x.DstInstid == instid && x.IsBuffRemove == ParseEnum.BuffRemove.None) || (x.SrcInstid == instid && x.IsBuffRemove != ParseEnum.BuffRemove.None))).ToList();
            List<CombatItem> filtered = new List<CombatItem>();
            for (int i = 0; i < main.Count; i++)
            {
                CombatItem c = main[i];
                if (needStart && c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    needStart = false;
                    filtered.Add(c);
                }
                else if (!needStart && c.IsBuffRemove != ParseEnum.BuffRemove.None)
                {
                    // consider only last remove event before another application
                    if ((i == main.Count - 1) || (i < main.Count - 1 && main[i + 1].IsBuffRemove == ParseEnum.BuffRemove.None))
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
