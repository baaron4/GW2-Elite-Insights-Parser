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

        private static List<int[]> GetMechanicData(IReadOnlyCollection<Mechanic> presMech, ParsedEvtcLog log, AbstractActor actor, PhaseData phase)
        {
            var res = new List<int[]>();

            foreach (Mechanic mech in presMech)
            {
                int filterCount = 0;
                int count = 0;
                if (mech.InternalCooldown > 0)
                {
                    long timeFilter = 0;
                    var mls = log.MechanicData.GetMechanicLogs(log, mech).Where(x => x.Actor.Agent == actor.Agent).ToList();
                    foreach (MechanicEvent ml in mls)
                    {
                        bool inInterval = phase.InInterval(ml.Time);
                        if (ml.Time - timeFilter < mech.InternalCooldown)//ICD check
                        {
                            if (inInterval)
                            {
                                filterCount++;
                            }
                        }
                        timeFilter = ml.Time;
                        if (inInterval)
                        {
                            count++;
                        }
                    }
                } 
                else
                {
                    count = log.MechanicData.GetMechanicLogs(log, mech).Where(x => x.Actor.Agent == actor.Agent && phase.InInterval(x.Time)).Count();
                }
                res.Add(new int[] { count - filterCount, count });
            }
            return res;
        }

        public static void BuildMechanics(IReadOnlyCollection<Mechanic> mechs, List<MechanicDto> mechsDtos)
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

            foreach (Player p in log.PlayerList)
            {
                list.Add(GetMechanicData(log.MechanicData.GetPresentPlayerMechs(log, log.FightData.FightStart, log.FightData.FightEnd), log, p, phase));
            }
            return list;
        }

        public static List<List<int[]>> BuildEnemyMechanicData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<int[]>>();
            foreach (AbstractSingleActor enemy in log.MechanicData.GetEnemyList(log, log.FightData.FightStart, log.FightData.FightEnd))
            {
                list.Add(GetMechanicData(log.MechanicData.GetPresentEnemyMechs(log, log.FightData.FightStart, log.FightData.FightEnd), log, enemy, phase));
            }
            return list;
        }
    }
}
