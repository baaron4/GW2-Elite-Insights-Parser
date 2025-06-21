using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class ColdWar : Drizzlewood
{
    public ColdWar(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([ 
            new PlayerDstHitMechanic(IcyEchoes, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "Icy.Ech", "Tight stacking damage","Icy Echoes", 0),
            new PlayerDstHitMechanic(Detonate, new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "Det.", "Hit by Detonation","Detonate", 50),
            new PlayerDstHitMechanic(LethalCoalescence, new MechanicPlotlySetting(Symbols.Hexagram,Colors.Orange), "Leth.Coal.", "Soaked damage","Lethal Coalescence", 50),
            new PlayerDstHitMechanic(FlameWall, new MechanicPlotlySetting(Symbols.Square,Colors.Orange), "Flm.Wall", "Stood in Flame Wall","Flame Wall", 50),
            new PlayerDstHitMechanic(CallAssassins, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.LightRed), "Call Ass.", "Hit by Assassins","Call Assassins", 50),
            new PlayerDstHitMechanic(Charge, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Orange), "Charge!", "Hit by Charge","Charge!", 50),
        ])
        );
        Extension = "coldwar";
        Icon = EncounterIconColdWar;
        EncounterCategoryInformation.InSubCategoryOrder = 0;
        EncounterID |= 0x000006;
    }

    /*protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayColdWar,
                        (729, 581),
                        (-32118, -11470, -28924, -8274),
                        (-0, -0, 0, 0),
                        (0, 0, 0, 0));
    }*/

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor varinia = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.VariniaStormsounder)) ?? throw new MissingKeyActorsException("Varinia Stormsounder not found");
        phases[0].AddTarget(varinia, log);
        //
        // TODO - add phases if applicable
        //
        for (int i = 1; i < phases.Count; i++)
        {
            phases[i].AddTarget(varinia, log);
            phases[i].AddParentPhase(phases[0]);
        }
        return phases;
    }

    // TODO - complete IDs
    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.PropagandaBallon,
            TargetID.DominionBladestorm,
            TargetID.DominionStalker,
            TargetID.DominionSpy1,
            TargetID.DominionSpy2,
            TargetID.DominionAxeFiend,
            TargetID.DominionEffigy,
            TargetID.FrostLegionCrusher,
            TargetID.FrostLegionMusketeer,
            TargetID.BloodLegionBlademaster,
            TargetID.CharrTank,
            TargetID.SonsOfSvanirHighShaman,
        ];
    }
}
