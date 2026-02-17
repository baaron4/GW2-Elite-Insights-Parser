using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels;

internal class MechanicDto
{
    public string? Name { get; set; }

    public int Icd { get; set; }
    public string? ShortName { get; set; }
    public string? Description { get; set; }
    public bool EnemyMech { get; set; }
    public bool PlayerMech { get; set; }

    private static List<double[]> GetMechanicData(IReadOnlyCollection<Mechanic> presMech, ParsedEvtcLog log, SingleActor actor, PhaseData phase)
    {
        var res = new List<double[]>(presMech.Count);

        foreach (Mechanic mech in presMech)
        {
            double filterCount = 0;
            double count = 0;
            if (mech.InternalCooldown > 0)
            {
                long timeFilter = 0;
                IReadOnlyList<MechanicEvent> mls = log.MechanicData.GetMechanicLogs(log, mech, actor, log.LogData.LogStart, log.LogData.LogEnd);
                foreach (MechanicEvent ml in mls)
                {
                    bool inInterval = phase.InInterval(ml.Time);
                    if (ml.Time - timeFilter < mech.InternalCooldown)//ICD check
                    {
                        if (inInterval)
                        {
                            filterCount += ml.GetWeight();
                        }
                    }
                    timeFilter = ml.Time;
                    if (inInterval)
                    {
                        count += ml.GetWeight();
                    }
                }
            }
            else
            {
                count = log.MechanicData.GetMechanicLogs(log, mech, actor, phase.Start, phase.End).Sum(x => x.GetWeight());
            }
            res.Add([Math.Round(count - filterCount, 2), Math.Round(count, 2)]);
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

    public static List<List<double[]>> BuildPlayerMechanicData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<List<double[]>>(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(GetMechanicData(log.MechanicData.GetPresentFriendlyMechs(log, log.LogData.LogStart, log.LogData.LogEnd), log, actor, phase));
        }
        return list;
    }

    public static List<List<double[]>> BuildEnemyMechanicData(ParsedEvtcLog log, PhaseData phase)
    {
        var enemies = log.MechanicData.GetEnemyList(log, log.LogData.LogStart, log.LogData.LogEnd);
        var list = new List<List<double[]>>(enemies.Count);
        foreach (SingleActor enemy in enemies)
        {
            list.Add(GetMechanicData(log.MechanicData.GetPresentEnemyMechs(log, log.LogData.LogStart, log.LogData.LogEnd), log, enemy, phase));
        }
        return list;
    }
}
