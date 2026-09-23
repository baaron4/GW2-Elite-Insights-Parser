using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Mechanic.MechanicSeverity;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.MechanicIDs;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class ColdWar : Drizzlewood
{
    public ColdWar(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(IcyEchoes, Mech_IcyEchoes, new (Symbols.DiamondTall,Colors.Red), new ("Icy.Ech", "Tight stacking damage","Icy Echoes"), Sev2),
            new PlayerDstHealthDamageHitMechanic(Detonate, Mech_Detonate, new (Symbols.Circle,Colors.Orange), new ("Det.", "Hit by Detonation","Detonate"), Sev2, 50),
            new PlayerDstHealthDamageHitMechanic(LethalCoalescence, Mech_LethalCoalescence, new (Symbols.Hexagram,Colors.Orange), new ("Leth.Coal.", "Soaked damage","Lethal Coalescence"), Sev0, 50),
            new PlayerDstHealthDamageHitMechanic(FlameWall, Mech_FlameWall, new (Symbols.Square,Colors.Orange), new ("Flm.Wall", "Stood in Flame Wall","Flame Wall"), Sev1, 50),
            new PlayerDstHealthDamageHitMechanic(CallAssassins, Mech_CallAssassins, new (Symbols.DiamondTall,Colors.LightRed), new ("Call Ass.", "Hit by Assassins","Call Assassins"), Sev1, 50),
            new PlayerDstHealthDamageHitMechanic(Charge, Mech_ChargeCW, new (Symbols.DiamondTall,Colors.Orange), new ("Charge!", "Hit by Charge","Charge!"), Sev1, 50),
        ])
        );
        Extension = "coldwar";
        Icon = EncounterIconColdWar;
        LogCategoryInformation.InSubCategoryOrder = 0;
        LogID |= 0x000006;
    }

    /*internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
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
    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }
    }
    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
    }

    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }
    }
}
