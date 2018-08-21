using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class MechanicData : Dictionary<Mechanic,List<MechanicLog>>
    {

        private List<HashSet<Mechanic>> presentOnPlayerMechanics = new List<HashSet<Mechanic>>();
        private List<HashSet<Mechanic>> presentOnEnemyMechanics = new List<HashSet<Mechanic>>();
        private List<HashSet<Mechanic>> presentMechanics = new List<HashSet<Mechanic>>();
        private List<List<AbstractMasterPlayer>> enemyList = new List<List<AbstractMasterPlayer>>();

        public MechanicData(BossData b_data)
        {
            List<Mechanic> bossMechanics = b_data.getBossBehavior().getMechanics();
            foreach(Mechanic m in bossMechanics)
            {
                this.Add(m, new List<MechanicLog>());
            }
        }

        public void computePresentMechanics(ParsedLog log, List<PhaseData> phases)
        {
            if (presentOnPlayerMechanics.Count > 0)
            {
                return;
            }
            // regroup same mechanics with diff ids
            Dictionary<string, Mechanic> altNames = new Dictionary<string, Mechanic>();
            List<Mechanic> toRemove = new List<Mechanic>();
            foreach (Mechanic mech in this.Keys)
            {
                if (altNames.ContainsKey(mech.GetAltName()))
                {
                    this[altNames[mech.GetAltName()]].AddRange(this[mech]);
                    toRemove.Add(mech);
                } else
                {
                    altNames.Add(mech.GetAltName(), mech);
                }
            }
            foreach(Mechanic mech in toRemove)
            {
                Remove(mech);
            }
            // ready present mechanics
            foreach (PhaseData phase in phases)
            {
                HashSet<Mechanic> toAddPlayer = new HashSet<Mechanic>();
                HashSet<Mechanic> toAddEnemy = new HashSet<Mechanic>();
                HashSet<Mechanic> toAddAll = new HashSet<Mechanic>();
                presentOnPlayerMechanics.Add(toAddPlayer);
                presentOnEnemyMechanics.Add(toAddEnemy);
                presentMechanics.Add(toAddAll);
                foreach (KeyValuePair<Mechanic, List<MechanicLog>> pair in this)
                {
                    if (pair.Value.Count(x => phase.inInterval(x.GetTime())) > 0)
                    {
                        toAddAll.Add(pair.Key);
                        if (pair.Key.IsEnemyMechanic())
                        {
                            toAddEnemy.Add(pair.Key);
                        } else if (pair.Key.GetMechType() != Mechanic.MechType.PlayerStatus)
                        {
                            toAddPlayer.Add(pair.Key);
                        }
                    }
                }
                // ready enemy list
                List<AbstractMasterPlayer> toAdd = new List<AbstractMasterPlayer>();
                enemyList.Add(toAdd);
                toAdd.Add(log.GetBoss());
                foreach(Mechanic m in Keys.Where(x=> x.IsEnemyMechanic()))
                {
                    foreach (AbstractMasterPlayer p in this[m].Where(x => phase.inInterval(x.GetTime())).Select(x => x.GetPlayer()).Distinct())
                    {
                        if (toAdd.FirstOrDefault(x => x.GetInstid() == p.GetInstid()) == null)
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

        public HashSet<Mechanic> getPresentEnemyMechs(int phase_index = 0)
        {
            return presentOnEnemyMechanics[phase_index];
        }
        public HashSet<Mechanic> getPresentPlayerMechs(int phase_index = 0)
        {
            return presentOnPlayerMechanics[phase_index];
        }
        public HashSet<Mechanic> getPresentMechanics(int phase_index = 0)
        {
            return presentMechanics[phase_index];
        }

        public List<AbstractMasterPlayer> getEnemyList(int phase_index = 0)
        {
            return enemyList[phase_index];
        }
    }
}
