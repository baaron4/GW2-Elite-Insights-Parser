using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class ShatteredObservatory : FractalLogic
{
    public ShatteredObservatory(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.ShatteredObservatory;
        MechanicList.Add(new MechanicGroup([
            new PlayerDstBuffApplyMechanic([ FixatedBloom1, FixatedBloom2, FixatedBloom3, FixatedBloom4 ], new MechanicPlotlySetting(Symbols.StarOpen,Colors.Magenta), "Bloom Fix", "Fixated by Solar Bloom","Bloom Fixate", 0),
            new PlayerDstBuffApplyMechanic(Fear, new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Red), "Eye", "Hit by the Overhead Eye Fear","Eye (Fear)" , 0)
                .UsingChecker((ba, log) => ba.AppliedDuration == 3000), //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new PlayerDstBuffApplyMechanic(CorporealReassignmentBuff, new MechanicPlotlySetting(Symbols.Diamond,Colors.Red), "Skull", "Exploding Skull mechanic application","Corporeal Reassignment", 0),
            ])
        );
        LogID |= LogIDs.FractalMasks.ShatteredObservatoryMask;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        // Set manual FractalScale for old logs without the event
        AddFractalScaleEvent(gw2Build, combatData, new List<(ulong, byte)>
        {
            ( GW2Builds.July2017ShatteredObservatoryRelease, 100),
            ( GW2Builds.September2020SunquaPeakRelease, 99),
            ( GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM, 98),
        });
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }

    /// <summary>
    /// Returns true if the buff count was not reached so that another method can be called, if necessary
    /// </summary>
    protected static bool SetSuccessByBuffCount(CombatData combatData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents, SingleActor target, long buffID, int count)
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
                SetSuccessByCombatExit(new List<SingleActor> { target }, combatData, logData, playerAgents);
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
    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        AddCorporealReassignmentDecorations(log, environmentDecorations);
    }
    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);

        // Corporeal Reassignment (skull)
        IEnumerable<Segment> corpReass = p.GetBuffStatus(log, CorporealReassignmentBuff).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(corpReass, p, ParserIcons.SkullOverhead);

        // Fixations
        var fixations = p.GetBuffStatus(log, [FixatedBloom1, FixatedBloom2, FixatedBloom3, FixatedBloom4]).Where(x => x.Value > 0);
        var fixationEvents = GetBuffApplyRemoveSequence(log.CombatData, [FixatedBloom1, FixatedBloom2, FixatedBloom3, FixatedBloom4], p, true, true);
        replay.Decorations.AddOverheadIcons(fixations, p, ParserIcons.FixationPurpleOverhead);
        replay.Decorations.AddTether(fixationEvents, Colors.Magenta, 0.5);
    }
}
