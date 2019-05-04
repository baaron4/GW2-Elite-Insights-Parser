using System;
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
        public readonly DamageModifiersContainer DamageModifiers;
        public readonly BoonsContainer Boons;



        public ParsedLog(LogData logData, FightData fightData, AgentData agentData, SkillData skillData, 
                CombatData combatData, List<Player> playerList, Target target)
        {
            LogData = logData;
            FightData = fightData;
            AgentData = agentData;
            SkillData = skillData;
            CombatData = combatData;
            //test.RemoveAll(x => tes2[x.Item1] > 1);
            PlayerList = playerList;
            Boons = new BoonsContainer(logData.GW2Version);
            /*HashSet<long> ignore = new HashSet<long>
            {
                5587,
                45502,
                42811,
                40926,
                42811,
                37192,
                30955,
                37192
            };
            var test = combatData.GetBoonDataByDst(22, 0, long.MaxValue).Where(x => x.IsBuffRemove == ParseEnum.BuffRemove.None && x.IsStateChange != ParseEnum.StateChange.BuffInitial && !ignore.Contains(x.SkillID) && !Boons.BoonsByIds.ContainsKey(x.SkillID)).Select(x => (x.SkillID, fightData.ToFightSpace(x.Time))).ToList();
            var tes2 = new Dictionary<long, List<long>>();
            foreach (var ii in test)
            {
                if (tes2.TryGetValue(ii.Item1, out var iii))
                {
                    tes2[ii.Item1].Add(ii.Item2);
                }
                else
                {
                    tes2[ii.Item1] = new List<long> { ii.Item2 };
                }
            }*/
            BoonSourceFinder = Boon.GetBoonSourceFinder(logData.GW2Version, Boons);
            DamageModifiers = new DamageModifiersContainer(logData.GW2Version);
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
            FightData.Logic.ComputeMechanics(this);
            Statistics = new Statistics(this);
        }
    }
}
