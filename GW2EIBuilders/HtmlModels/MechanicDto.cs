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
    public bool IsAchievementEligibility { get; set; }

    private static List<int[]> GetMechanicData(IReadOnlyCollection<Mechanic> presMech, ParsedEvtcLog log, SingleActor actor, PhaseData phase)
    {
        var res = new List<int[]>(presMech.Count);

        foreach (Mechanic mech in presMech)
        {
            int filterCount = 0;
            int count = 0;
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
                count = log.MechanicData.GetMechanicLogs(log, mech, actor, phase.Start, phase.End).Count;
            }
            res.Add([count - filterCount, count]);
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
                IsAchievementEligibility = mech.IsAchievementEligibility,
                Icd = mech.InternalCooldown
            };
            mechsDtos.Add(dto);
        }
    }

    public static List<List<int[]>> BuildPlayerMechanicData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<List<int[]>>(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(GetMechanicData(log.MechanicData.GetPresentFriendlyMechs(log, log.LogData.LogStart, log.LogData.LogEnd), log, actor, phase));
        }
        return list;
    }

    public static List<List<int[]>> BuildEnemyMechanicData(ParsedEvtcLog log, PhaseData phase)
    {
        var enemies = log.MechanicData.GetEnemyList(log, log.LogData.LogStart, log.LogData.LogEnd);
        var list = new List<List<int[]>>(enemies.Count);
        foreach (SingleActor enemy in enemies)
        {
            list.Add(GetMechanicData(log.MechanicData.GetPresentEnemyMechs(log, log.LogData.LogStart, log.LogData.LogEnd), log, enemy, phase));
        }
        return list;
    }
}
