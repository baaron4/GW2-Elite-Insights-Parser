using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;

namespace GW2EIEvtcParser.EncounterLogic;

internal abstract class ShatteredObservatory : FractalLogic
{
    public ShatteredObservatory(int triggerID) : base(triggerID)
    {
        EncounterCategoryInformation.SubCategory = SubFightCategory.ShatteredObservatory;
        EncounterID |= EncounterIDs.FractalMasks.ShatteredObservatoryMask;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        // Set manual FractalScale for old logs without the event
        AddFractalScaleEvent(gw2Build, combatData, new List<(ulong, byte)>
        {
            ( GW2Builds.July2017ShatteredObservatoryRelease, 100),
            ( GW2Builds.September2020SunquaPeakRelease, 99),
            ( GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM, 98),
        });
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
    }

    /// <summary>
    /// Returns true if the buff count was not reached so that another method can be called, if necessary
    /// </summary>
    protected static bool SetSuccessByBuffCount(CombatData combatData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents, SingleActor target, long buffID, int count)
    {
        if (target == null)
        {
            return false;
        }
        var invulsTarget = GetBuffApplyRemoveSequence(combatData, buffID, target, true, false).Where(x => x.Time >= 0);
        if (invulsTarget.Count() == count)
        {
            BuffEvent last = invulsTarget.Last();
            if (!(last is BuffApplyEvent))
            {
                SetSuccessByCombatExit(new List<SingleActor> { target }, combatData, fightData, playerAgents);
                return false;
            }
        }
        return true;
    }

    protected static void AddCorporealReassignmentDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        // Corporeal Reassignment domes & explosions
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.CorporealReassignmentDome, out var domes))
        {
            foreach (EffectEvent effect in domes)
            {
                environmentDecorations.Add(new CircleDecoration(220, effect.ComputeDynamicLifespan(log, 0), Colors.LightBlue, 0.4, new PositionConnector(effect.Position)).UsingFilled(false));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.CorporealReassignmentExplosionDome, out var domeExplosions))
        {
            foreach (EffectEvent effect in domeExplosions)
            {
                (long start, long end) lifespan = (effect.Time - 500, effect.Time);
                environmentDecorations.Add(new CircleDecoration(220, lifespan, Colors.LightBlue, 0.4, new PositionConnector(effect.Position)).UsingGrowingEnd(lifespan.end));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.CorporealReassignmentExplosion1, out var explosions))
        {
            foreach (EffectEvent effect in explosions)
            {
                (long start, long end) lifespan = (effect.Time, effect.Time + 500);
                environmentDecorations.Add(new CircleDecoration(2000, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position)).UsingGrowingEnd(lifespan.end));
            }
        }
    }
}
