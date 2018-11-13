using LuckParser.Models.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class MechanicData : Dictionary<Mechanic,List<MechanicLog>>
    {

        private readonly List<HashSet<Mechanic>> _presentOnPlayerMechanics = new List<HashSet<Mechanic>>();
        private readonly List<HashSet<Mechanic>> _presentOnEnemyMechanics = new List<HashSet<Mechanic>>();
        private readonly List<HashSet<Mechanic>> _presentMechanics = new List<HashSet<Mechanic>>();
        private readonly List<List<AbstractMasterPlayer>> _enemyList = new List<List<AbstractMasterPlayer>>();

        public MechanicData(FightData fightData)
        {
            List<Mechanic> fightMechanics = fightData.Logic.MechanicList;
            foreach(Mechanic m in fightMechanics)
            {
                Add(m, new List<MechanicLog>());
            }
        }

        public void ComputePresentMechanics(ParsedLog log)
        {
            if (_presentOnPlayerMechanics.Count > 0)
            {
                return;
            }
            // regroup same mechanics with diff ids
            Dictionary<string, Mechanic> altNames = new Dictionary<string, Mechanic>();
            List<Mechanic> toRemove = new List<Mechanic>();
            foreach (Mechanic mech in Keys)
            {
                if (altNames.ContainsKey(mech.ShortName))
                {
                    this[altNames[mech.ShortName]].AddRange(this[mech]);
                    toRemove.Add(mech);
                } else
                {
                    altNames.Add(mech.ShortName, mech);
                }
            }
            foreach(Mechanic mech in toRemove)
            {
                Remove(mech);
            }
            // ready present mechanics
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                HashSet<Mechanic> toAddPlayer = new HashSet<Mechanic>();
                HashSet<Mechanic> toAddEnemy = new HashSet<Mechanic>();
                HashSet<Mechanic> toAddAll = new HashSet<Mechanic>();
                _presentOnPlayerMechanics.Add(toAddPlayer);
                _presentOnEnemyMechanics.Add(toAddEnemy);
                _presentMechanics.Add(toAddAll);
                foreach (KeyValuePair<Mechanic, List<MechanicLog>> pair in this)
                {
                    if (pair.Value.Count(x => phase.InInterval(x.Time)) > 0)
                    {
                        toAddAll.Add(pair.Key);
                        if (pair.Key.IsEnemyMechanic)
                        {
                            toAddEnemy.Add(pair.Key);
                        } else if (pair.Key.MechanicType != Mechanic.MechType.PlayerStatus)
                        {
                            toAddPlayer.Add(pair.Key);
                        }
                    }
                }
                // ready enemy list
                List<AbstractMasterPlayer> toAdd = new List<AbstractMasterPlayer>();
                _enemyList.Add(toAdd);
                foreach(Mechanic m in Keys.Where(x=> x.IsEnemyMechanic))
                {
                    foreach (AbstractMasterPlayer p in this[m].Where(x => phase.InInterval(x.Time)).Select(x => x.Player).Distinct())
                    {
                        if (toAdd.FirstOrDefault(x => x.InstID == p.InstID) == null)
                        {
                            toAdd.Add(p);
                        }
                    }
                }
            }
            List<Mechanic> emptyMechanic = this.Where(pair => pair.Value.Count == 0).Select(pair => pair.Key).ToList();
            foreach (Mechanic m in emptyMechanic)
            {
                Remove(m);
            }
        }

        public HashSet<Mechanic> GetPresentEnemyMechs(int phaseIndex)
        {
            return _presentOnEnemyMechanics[phaseIndex];
        }
        public HashSet<Mechanic> GetPresentPlayerMechs(int phaseIndex)
        {
            return _presentOnPlayerMechanics[phaseIndex];
        }
        public HashSet<Mechanic> GetPresentMechanics(int phaseIndex)
        {
            return _presentMechanics[phaseIndex];
        }

        public List<AbstractMasterPlayer> GetEnemyList(int phaseIndex)
        {
            return _enemyList[phaseIndex];
        }
    }
}
