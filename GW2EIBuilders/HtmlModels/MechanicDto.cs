using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels
{
    internal class MechanicDto
    {
        public string Name { get; set; }

        public int Icd { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public bool EnemyMech { get; set; }
        public bool PlayerMech { get; set; }

        private static List<int[]> GetMechanicData(HashSet<Mechanic> presMech, ParsedEvtcLog log, AbstractActor actor, PhaseData phase)
        {
            var res = new List<int[]>();

            foreach (Mechanic mech in presMech)
            {
                long timeFilter = 0;
                int filterCount = 0;
                var mls = log.MechanicData.GetMechanicLogs(log, mech).Where(x => x.Actor.Agent == actor.Agent && phase.InInterval(x.Time)).ToList();
                int count = mls.Count;
                foreach (MechanicEvent ml in mls)
                {
                    if (mech.InternalCooldown != 0 && ml.Time - timeFilter < mech.InternalCooldown)//ICD check
                    {
                        filterCount++;
                    }
                    timeFilter = ml.Time;

                }
                res.Add(new int[] { count - filterCount, count });
            }
            return res;
        }

        public static void BuildMechanics(HashSet<Mechanic> mechs, List<MechanicDto> mechsDtos)
        {
            foreach (Mechanic mech in mechs)
            {
                var dto = new MechanicDto
                {
                    Name = mech.FullName,
                    ShortName = mech.ShortName,
                    Description = mech.Description,
                    PlayerMech = mech.ShowOnTable && !mech.IsEnemyMechanic,
                    EnemyMech = mech.IsEnemyMechanic,
                    Icd = mech.InternalCooldown
                };
                mechsDtos.Add(dto);
            }
        }

        public static List<List<int[]>> BuildPlayerMechanicData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<int[]>>();
            HashSet<Mechanic> presMech = log.MechanicData.GetPresentPlayerMechs(log, 0);

            foreach (Player p in log.PlayerList)
            {
                list.Add(GetMechanicData(presMech, log, p, phase));
            }
            return list;
        }

        public static List<List<int[]>> BuildEnemyMechanicData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<int[]>>();
            HashSet<Mechanic> presMech = log.MechanicData.GetPresentEnemyMechs(log, 0);
            foreach (AbstractActor enemy in log.MechanicData.GetEnemyList(log, 0))
            {
                list.Add(GetMechanicData(presMech, log, enemy, phase));
            }
            return list;
        }
    }
}
