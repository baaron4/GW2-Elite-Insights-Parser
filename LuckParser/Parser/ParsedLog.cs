using System.Collections.Generic;
using System.Linq;
using LuckParser.Exceptions;
using LuckParser.Models;
using LuckParser.Models.Logic;
using LuckParser.Models.ParseModels;

namespace LuckParser.Parser
{
    public class ParsedLog
    {
        public readonly LogData LogData;
        public readonly FightData FightData;
        public readonly AgentData AgentData;
        public readonly SkillData SkillData;
        public readonly CombatData CombatData;
        public readonly MechanicData MechanicData;
        public readonly List<Player> PlayerList;
        public readonly HashSet<ushort> PlayerIDs;
        public readonly BoonSourceFinder BoonSourceFinder;
        public bool IsBenchmarkMode => FightData.Logic.Mode == FightLogic.ParseMode.Golem;
        public readonly Dictionary<string, List<Player>> PlayerListBySpec;
        public readonly Target LegacyTarget;
        public readonly Statistics Statistics;
        public readonly DamageModifierContainer DamageModifiers;

        

        public ParsedLog(LogData logData, FightData fightData, AgentData agentData, SkillData skillData, 
                CombatData combatData, List<Player> playerList, Target target)
        {
            LogData = logData;
            FightData = fightData;
            AgentData = agentData;
            SkillData = skillData;
            CombatData = combatData;
            PlayerList = playerList;
            BoonSourceFinder = Boon.GetBoonSourceFinder(logData.GW2Version);
            LegacyTarget = target;
            MechanicData = new MechanicData(fightData);
            PlayerListBySpec = playerList.GroupBy(x => x.Prof).ToDictionary(x => x.Key, x => x.ToList());
            PlayerIDs = new HashSet<ushort>(playerList.Select(x => x.InstID));
            FightData.SetSuccess(this);
            FightData.SetCM(this);
            CombatData.Update(FightData.FightEnd);
            if (FightData.FightDuration <= 2200)
            {
                throw new TooShortException();
            }
            if (Properties.Settings.Default.SkipFailedTries && !FightData.Success)
            {
                throw new SkipException();
            }

            // init combat replay
            if (Properties.Settings.Default.ParseCombatReplay && FightData.Logic.CanCombatReplay)
            {
                foreach (Player p in PlayerList)
                {
                    if (p.Account == ":Conjured Sword")
                    {
                        continue;
                    }
                    p.InitCombatReplay(this, GeneralHelper.PollingRate, false, true);
                }
                foreach (Target tar in FightData.Logic.Targets)
                {
                    tar.InitCombatReplay(this, GeneralHelper.PollingRate, true, FightData.GetMainTargets(this).Contains(tar));
                }
                FightData.Logic.InitTrashMobCombatReplay(this, GeneralHelper.PollingRate);

                // Ensuring all combat replays are initialized before extra data (and agent interaction) is computed
                foreach (Player p in PlayerList)
                {
                    if (p.Account == ":Conjured Sword")
                    {
                        continue;
                    }
                    p.ComputeAdditionalCombatReplayData(this);
                }
                foreach (Target tar in FightData.Logic.Targets)
                {
                    tar.ComputeAdditionalCombatReplayData(this);
                }
                foreach (Mob mob in FightData.Logic.TrashMobs)
                {
                    mob.ComputeAdditionalCombatReplayData(this);
                }
            }
            FightData.Logic.ComputeMechanics(this);
            Statistics = new Statistics(this);
            DamageModifiers = new DamageModifierContainer(logData.GW2Version);
        }
    }
}
