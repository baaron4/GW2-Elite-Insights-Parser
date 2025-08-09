using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class Nightmare : FractalLogic
{
    public Nightmare(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.Nightmare;
        LogID |= LogIDs.FractalMasks.NightmareMask;
    }

    protected static void AddCascadeOfTormentDecoration(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations, GUID guid, uint innerRadius, uint outerRadius)
    {
        int duration = 1000;
        if (log.CombatData.TryGetEffectEventsByGUID(guid, out var cascadeOfTorment))
        {
            foreach (EffectEvent effect in cascadeOfTorment)
            {
                (long start, long end) lifespanIndicator = (effect.Time, effect.Time + duration);
                (long start, long end) lifespanDamage = (lifespanIndicator.end, lifespanIndicator.end + 150);
                if (innerRadius == 0)
                {
                    environmentDecorations.Add(new CircleDecoration(outerRadius, lifespanIndicator, Colors.Orange, 0.2, new PositionConnector(effect.Position)));
                    environmentDecorations.Add(new CircleDecoration(outerRadius, lifespanDamage, Colors.Orange, 0.4, new PositionConnector(effect.Position)));
                }
                else
                {
                    environmentDecorations.Add(new DoughnutDecoration(innerRadius, outerRadius, lifespanIndicator, Colors.Orange, 0.2, new PositionConnector(effect.Position)));
                    environmentDecorations.Add(new DoughnutDecoration(innerRadius, outerRadius, lifespanDamage, Colors.Orange, 0.4, new PositionConnector(effect.Position)));
                }
            }
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        // Set manual FractalScale for old logs without the event
        AddFractalScaleEvent(gw2Build, combatData, new List<(ulong, byte)>
        {
            ( GW2Builds.November2016NightmareRelease, 100),
            ( GW2Builds.July2017ShatteredObservatoryRelease, 99),
            ( GW2Builds.September2020SunquaPeakRelease, 98),
            ( GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM, 97),
        });
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }
}
