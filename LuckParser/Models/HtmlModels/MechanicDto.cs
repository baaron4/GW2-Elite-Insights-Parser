using LuckParser.Models.ParseModels;
using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
namespace LuckParser.Models.HtmlModels
{  
    public class MechanicDto
    {       
        public string Name;       
        public string ShortName;        
        public string Description;       
        public bool EnemyMech;       
        public bool PlayerMech;

        public static List<int[]> GetMechanicData(HashSet<Mechanic> presMech, ParsedLog log, DummyActor actor, PhaseData phase)
        {
            List<int[]> res = new List<int[]>();

            foreach (Mechanic mech in presMech)
            {
                long timeFilter = 0;
                int filterCount = 0;
                List<MechanicEvent> mls = log.MechanicData.GetMechanicLogs(log, mech).Where(x => x.Actor.InstID == actor.InstID && phase.InInterval(x.Time)).ToList();
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
                MechanicDto dto = new MechanicDto
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
