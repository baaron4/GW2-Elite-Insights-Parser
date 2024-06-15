using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class ColdWar : IcebroodSagaStrike
    {
        public ColdWar(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(IcyEchoes, "Icy Echoes", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "Icy.Ech","Tight stacking damage", "Icy Echoes",0),
                new PlayerDstHitMechanic(Detonate, "Detonate", new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "Det.","Hit by Detonation", "Detonate",50),
                new PlayerDstHitMechanic(LethalCoalescence, "Lethal Coalescence", new MechanicPlotlySetting(Symbols.Hexagram,Colors.Orange), "Leth.Coal.","Soaked damage", "Lethal Coalescence",50),
                new PlayerDstHitMechanic(FlameWall, "Flame Wall", new MechanicPlotlySetting(Symbols.Square,Colors.Orange), "Flm.Wall","Stood in Flame Wall", "Flame Wall",50),
                new PlayerDstHitMechanic(CallAssassins, "Call Assassins", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.LightRed), "Call Ass.","Hit by Assassins", "Call Assassins",50),
                new PlayerDstHitMechanic(Charge, "Charge!", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Orange), "Charge!","Hit by Charge", "Charge!",50),
            }
            );
            Extension = "coldwar";
            Icon = EncounterIconColdWar;
            EncounterCategoryInformation.SubCategory = EncounterCategory.SubFightCategory.Drizzlewood;
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
            AbstractSingleActor varinia = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.VariniaStormsounder)) ?? throw new MissingKeyActorsException("Varinia Stormsounder not found");
            phases[0].AddTarget(varinia);
            //
            // TODO - add phases if applicable
            //
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].AddTarget(varinia);
            }
            return phases;
        }

        // TODO - complete IDs
        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.PropagandaBallon,
                ArcDPSEnums.TrashID.DominionBladestorm,
                ArcDPSEnums.TrashID.DominionStalker,
                ArcDPSEnums.TrashID.DominionSpy1,
                ArcDPSEnums.TrashID.DominionSpy2,
                ArcDPSEnums.TrashID.DominionAxeFiend,
                ArcDPSEnums.TrashID.DominionEffigy,
                ArcDPSEnums.TrashID.FrostLegionCrusher,
                ArcDPSEnums.TrashID.FrostLegionMusketeer,
                ArcDPSEnums.TrashID.BloodLegionBlademaster,
                ArcDPSEnums.TrashID.CharrTank,
                ArcDPSEnums.TrashID.SonsOfSvanirHighShaman,
            };
        }
    }
}
