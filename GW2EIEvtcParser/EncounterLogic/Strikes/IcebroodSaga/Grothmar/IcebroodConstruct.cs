using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class IcebroodConstruct : IcebroodSagaStrike
    {
        public IcebroodConstruct(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(DeatlyIceShockWave, "Deadly Ice Shock Wave",new MechanicPlotlySetting(Symbols.Square,Colors.Red),"D.IceWave","Deadly Ice Shock Wave", "Deadly Shock Wave", 0),
                new PlayerDstHitMechanic(IceArmSwing, "Ice Arm Swing",new MechanicPlotlySetting(Symbols.TriangleUp,Colors.LightOrange),"A.Swing","Ice Arm Swing", "Ice Arm Swing", 0),
                new PlayerDstHitMechanic(new long[] { IceShockWave1, IceShockWave2, IceShockWave3 }, "Ice Shock Wave",new MechanicPlotlySetting(Symbols.Square,Colors.LightOrange),"ShockWave","Ice Shock Wave", "Ice Shock Wave", 0),
                new PlayerDstHitMechanic(IceShatter, "Ice Shatter",new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Pink),"Ice Orbs","Rotating Ice Orbs", "Ice Orbs", 50),
                new PlayerDstHitMechanic(IceCrystal, "Ice Crystal",new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange),"I.Crystal","Ice Crystal", "Ice Crystal", 50),
                new PlayerDstHitMechanic(new long[] { IceFrail1, IceFrail2 }, "Ice Flail",new MechanicPlotlySetting(Symbols.Square,Colors.Pink),"I.Flail","Ice Flail", "Ice Flail", 50),
            }
            );
            Extension = "icebrood";
            Icon = EncounterIconIcebroodConstruct;
            EncounterCategoryInformation.SubCategory = EncounterCategory.SubFightCategory.Grothmar;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000001;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayIcebroodConstruct,
                            (729, 581),
                            (-32118, -11470, -28924, -8274)/*,
                            (-0, -0, 0, 0),
                            (0, 0, 0, 0)*/);
        }
        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(FrostbiteAuraIcebroodConstruct, FrostbiteAuraIcebroodConstruct), // Frostbite Aura
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.IcebroodConstruct));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Icebrood Construct not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Invul check
            phases.AddRange(GetPhasesByInvul(log, 757, mainTarget, false, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = "Phase " + i;
                phase.AddTarget(mainTarget);
            }
            return phases;
        }
    }
}
