using System.Collections.Generic;
using System.Linq;
using LuckParser.EIData;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData.CombatEvents;
namespace LuckParser.Builders.HtmlModels
{
    public class MechanicDto
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public bool EnemyMech { get; set; }
        public bool PlayerMech { get; set; }

        public static List<int[]> GetMechanicData(HashSet<Mechanic> presMech, ParsedLog log, DummyActor actor, PhaseData phase)
        {
            var res = new List<int[]>();

            foreach (Mechanic mech in presMech)
            {
                long timeFilter = 0;
                int filterCount = 0;
                var mls = log.MechanicData.GetMechanicLogs(log, mech).Where(x => x.Actor.InstID == actor.InstID && phase.InInterval(x.Time)).ToList();
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
                    EnemyMech = mech.IsEnemyMechanic
                };
                mechsDtos.Add(dto);
            }
        }
    }
}
