
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class DecimaTheStormsinger : MountBalrior
{
    public DecimaTheStormsinger(int triggerID) : base(triggerID)
    {
        Extension = "decima";
        Icon = EncounterIconDecima;
        EncounterCategoryInformation.InSubCategoryOrder = 1;
        EncounterID |= 0x000002;
    }


    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor Decima = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Decima)) ?? throw new MissingKeyActorsException("Decima not found");
        phases[0].AddTarget(Decima);
        if (!requirePhases)
        {
            return phases;
        }
        // Invul check
        phases.AddRange(GetPhasesByInvul(log, NovaShield, Decima, true, true));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            if (i % 2 == 0)
            {
                phase.Name = "Split " + (i) / 2;
                phase.AddTarget(Decima);
            }
            else
            {
                phase.Name = "Phase " + (i + 1) / 2;
                phase.AddTarget(Decima);
            }
        }
        return phases;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(player, log, replay);

        replay.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder1, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder1Overhead);
        replay.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder2, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder2Overhead);
        replay.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder3, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder3Overhead);
        replay.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder4, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder4Overhead);
        replay.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder5, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder5Overhead);
    }
}
