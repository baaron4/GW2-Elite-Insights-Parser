using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class MechanicData : List<MechanicLog>
    {

        private List<Dictionary<string, HashSet<Mechanic>>> presentOnPlayerMechanics = new List<Dictionary<string, HashSet<Mechanic>>>();
        private List<Dictionary<string, HashSet<Mechanic>>> presentOnEnemyMechanics = new List<Dictionary<string, HashSet<Mechanic>>>();
        private List<HashSet<Mechanic>> presentMechanics = new List<HashSet<Mechanic>>();
        private List<List<AbstractMasterPlayer>> enemyList = new List<List<AbstractMasterPlayer>>();

        public MechanicData()
        {

        }

        public void computePresentMechanics(ParsedLog log, List<PhaseData> phases)
        {
            if (presentOnEnemyMechanics.Count > 0)
            {
                return;
            }
            List<ushort> pIds = log.getPlayerList().Select(x => x.getInstid()).ToList();
            this.OrderBy(x => x.GetTime());
            // ready present mechanics
            List<Mechanic> bossMechanics = log.getBossData().getBossBehavior().getMechanics();
            foreach (PhaseData phase in phases)
            {
                List<AbstractMasterPlayer> toAdd = new List<AbstractMasterPlayer>();
                enemyList.Add(toAdd);
                // ready enemy list
                toAdd.Add(log.getBoss());
                List<MechanicLog> presentMechanicLogs = this.Where(x => phase.inInterval(x.GetTime())).ToList();
                foreach (AbstractMasterPlayer p in presentMechanicLogs.Select(x => x.GetPlayer()).Distinct().ToList())
                {
                    if (pIds.Contains(p.getInstid()))
                    {
                        continue;
                    }
                    if (toAdd.FirstOrDefault(x => x.getInstid() == p.getInstid()) == null)
                    {
                        toAdd.Add(p);
                    }
                }
                Dictionary<string, HashSet<Mechanic>> toAddPlayer = new Dictionary<string, HashSet<Mechanic>>();
                Dictionary<string, HashSet<Mechanic>> toAddEnemy = new Dictionary<string, HashSet<Mechanic>>();
                HashSet<Mechanic> toAddAll = new HashSet<Mechanic>();
                presentOnPlayerMechanics.Add(toAddPlayer);
                presentOnEnemyMechanics.Add(toAddEnemy);
                presentMechanics.Add(toAddAll);
                foreach (MechanicLog ml in presentMechanicLogs)
                {
                    Mechanic correspondingMechanic = bossMechanics.Find(x => x.GetSkill() == ml.GetSkill());
                    if (correspondingMechanic != null)
                    {
                        string altName = correspondingMechanic.GetAltName();
                        toAddAll.Add(correspondingMechanic);
                        if (correspondingMechanic.isEnemyMechanic())
                        {
                            if (!toAddEnemy.ContainsKey(altName))
                            {
                                toAddEnemy[altName] = new HashSet<Mechanic>();
                            }
                            toAddEnemy[altName].Add(correspondingMechanic);
                        }
                        else
                        {
                            if (!toAddPlayer.ContainsKey(altName))
                            {
                                toAddPlayer[altName] = new HashSet<Mechanic>();
                            }
                            toAddPlayer[altName].Add(correspondingMechanic);
                        }
                    }
                }
            }
        }

        public Dictionary<string, HashSet<Mechanic>> getPresentEnemyMechs(int phase_index = 0)
        {
            return presentOnEnemyMechanics[phase_index];
        }
        public Dictionary<string, HashSet<Mechanic>> getPresentPlayerMechs(int phase_index = 0)
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
