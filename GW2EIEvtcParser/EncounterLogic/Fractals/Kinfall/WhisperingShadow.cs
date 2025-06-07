using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.SkillIDs;
using GW2EIEvtcParser.ParserHelpers;

namespace GW2EIEvtcParser.EncounterLogic;

internal class WhisperingShadow : Kinfall
{
    public WhisperingShadow(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(DeathlyGrime, new MechanicPlotlySetting(Symbols.Diamond, Colors.Purple), "DeathGr.A", "Gained Deathly Grime", "Deathly Grime Application", 0),
                new PlayerDstBuffRemoveMechanic(LifeFire, new MechanicPlotlySetting(Symbols.PentagonOpen, Colors.LightBlue), "LifeFire.R", "Lost Life-Fire (Protective Circle)", "Life-Fire Remove", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic([VitreousSpikeHit1, VitreousSpikeHit2], new MechanicPlotlySetting(Symbols.TriangleUp, Colors.SkyBlue), "Spike.H", "Hit by Vitreous Spike", "Vitreous Spike Hit", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(FrozenTeeth, new MechanicPlotlySetting(Symbols.XThinOpen, Colors.SkyBlue), "Fissure.H", "Hit by Frozen Teeth (Fissures)", "Frozen Teeth Hit", 0),
                new PlayerDstHitMechanic(LoftedCryoflash, new MechanicPlotlySetting(Symbols.StarTriangleDownOpen, Colors.Red), "HighCryo.H", "Hit by Lofted Cryoflash (High Shockwave)", "Lofted Cryoflash Hit", 0),
                new PlayerDstHitMechanic(TerrestialCryoflash, new MechanicPlotlySetting(Symbols.StarTriangleUpOpen, Colors.Red), "LowCryo.H", "Hit by Terrestrial Cryoflash (Low Shockwave)", "Terrestrial Cryoflash Hit", 0),
            ]),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(GorefrostTarget, new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.Red), "Arrow.A", "Targeted by Gorefrost (Arrows)", "Gorefrost Target", 0),
                new PlayerDstHitMechanic(Gorefrost, new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.Orange), "Arrow.H", "Hit by Gorefrost (Arrows)", "Gorefrost Hit", 0),
            ]),
            new MechanicGroup([
                new EnemyDstBuffApplyMechanic(EmpoweredWatchknightTriumverate, new MechanicPlotlySetting(Symbols.Square, Colors.Red), "Emp.A", "Gained Empowered", "Empowered Application", 0),
            ]),
        ]));
        Extension = "whispshadow";
        Icon = EncounterIconGeneric;
        EncounterID |= 0x000001;
    }

    protected override ReadOnlySpan<TargetID> GetTargetsIDs()
    {
        return [
            TargetID.WhisperingShadow,
        ];
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return FightData.EncounterMode.Normal;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor shadow = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.WhisperingShadow)) ?? throw new MissingKeyActorsException("Whispering Shadow not found");
        phases[0].AddTarget(shadow, log);
        if (!requirePhases)
        {
            return phases;
        }

        // breakbars queue up at 80%, 50%, 20%
        // frozen teeth & gorefrost become more powerful below these thresholds
        var (_, breakbarActives, _, _) = shadow.GetBreakbarStatus(log);
        if (breakbarActives.Count > 0)
        {
            int i = 1;
            var start = phases[0].Start;
            foreach (var breakbarActive in breakbarActives)
            {
                var phase = new PhaseData(start, breakbarActive.Start, "Phase " + i);
                phase.AddTarget(shadow, log);
                phases.Add(phase);
                start = phase.End;
                i++;
            }
            var finalPhase = new PhaseData(start, phases[0].End, "Phase " + i);
            finalPhase.AddTarget(shadow, log);
            phases.Add(finalPhase);
        }
        return phases;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(player, log, replay);

        // gorefrost (arrow) target
        IEnumerable<Segment> gorefrost = player.GetBuffStatus(log, GorefrostTarget, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(gorefrost, player, ParserIcons.TargetOverhead);
    }
}
