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
        private List<AbstractMasterPlayer> enemyList = new List<AbstractMasterPlayer>();

        public MechanicData()
        {

        }

        public void computePresentMechanics(ParsedLog log, List<PhaseData> phases)
        {
            if (presentOnEnemyMechanics.Count > 0)
            {
                return;
            }
            // ready enemy list
            List<ushort> pIds = log.getPlayerList().Select(x => x.getInstid()).ToList();
            enemyList.Add(log.getBoss());
            foreach (AbstractMasterPlayer p in this.Select(x => x.GetPlayer()).Distinct().ToList())
            {
                if (pIds.Contains(p.getInstid()))
                {
                    continue;
                }
                if (enemyList.FirstOrDefault(x => x.getInstid() == p.getInstid()) == null)
                {
                    enemyList.Add(p);
                }
            }
            // ready present mechanics
            List<Mechanic> bossMechanics = log.getBossData().getBossBehavior().getMechanics();
            foreach (PhaseData phase in phases)
            {
                Dictionary<string, HashSet<Mechanic>> toAddPlayer = new Dictionary<string, HashSet<Mechanic>>();
                Dictionary<string, HashSet<Mechanic>> toAddEnemy = new Dictionary<string, HashSet<Mechanic>>();
                presentOnPlayerMechanics.Add(toAddPlayer);
                presentOnEnemyMechanics.Add(toAddEnemy);
                List<MechanicLog> presentMechanics = this.Where(x => phase.inInterval(x.GetTime())).ToList();
                foreach (MechanicLog ml in presentMechanics)
                {
                    Mechanic correspondingMechanic = bossMechanics.Find(x => x.GetSkill() == ml.GetSkill());
                    if (correspondingMechanic != null)
                    {
                        string altName = correspondingMechanic.GetAltName();
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

        public List<Dictionary<string, HashSet<Mechanic>>> getPresentEnemyMechs()
        {
            return presentOnEnemyMechanics;
        }
        public List<Dictionary<string, HashSet<Mechanic>>> getPresentPlayerMechs()
        {
            return presentOnPlayerMechanics;
        }

        public List<AbstractMasterPlayer> getEnemyList()
        {
            return enemyList;
        }
    }
}
