using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class IcebroodConstruct : IcebroodSagaStrike
    {
        public IcebroodConstruct(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(DeadlyIceShockWave, "Deadly Ice Shock Wave", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Red), "D.IceWave", "Hit by Deadly Ice Shock Wave", "Deadly Ice Shock Wave", 0),
                new PlayerDstHitMechanic(IceArmSwing, "Ice Arm Swing", new MechanicPlotlySetting(Symbols.Star, Colors.Orange), "A.Swing", "Hit by Ice Arm Swing (Spin)", "Ice Arm Swing", 0),
                new PlayerDstHitMechanic(IceArmSwing, "Ice Arm Swing", new MechanicPlotlySetting(Symbols.Star, Colors.Yellow), "ArmSwing.CC", "Knocked by Ice Arm Swing (Spin)", "Ice Arm Swing", 0).UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ServerDelayConstant)),
                new PlayerDstHitMechanic(IceShatter, "Ice Shatter", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Pink), "Ice Orbs", "Hit by Rotating Ice Shatter (Orbs)", "Ice Shatter (Orbs)", 50),
                new PlayerDstHitMechanic(IceCrystal, "Ice Crystal", new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "I.Crystal", "Hit by Ice Crystal (Chill AoE)", "Ice Crystal", 50),
                new PlayerDstHitMechanic(Frostbite, "Frostbite", new MechanicPlotlySetting(Symbols.Square, Colors.Blue), "Frostbite.H", "Hit by Frostbite", "Frostbite", 0),
                new PlayerDstHitMechanic(new long[] { IceFrail1, IceFrail2 }, "Ice Flail", new MechanicPlotlySetting(Symbols.Square, Colors.Orange), "I.Flail", "Hit by Ice Flail (Arm Swipe)", "Ice Flail", 50),
                new PlayerDstHitMechanic(new long[] { IceFrail1, IceFrail2 }, "Ice Flail", new MechanicPlotlySetting(Symbols.Square, Colors.Yellow), "IceFlail.CC", "Knocked by Ice Flail (Arm Swipe)", "Ice Flail", 50).UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ServerDelayConstant)),
                new PlayerDstHitMechanic(new long[] { IceShockWave1, IceShockWave2, IceShockWave3 }, "Ice Shock Wave", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.LightOrange), "ShockWave.H", "Hit by Ice Shock Wave", "Ice Shock Wave", 0),
                new PlayerDstHitMechanic(new long[] { SpinningIce1, SpinningIce2, SpinningIce3, SpinningIce4 }, "Spinning Ice", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.White), "SpinIce.H", "Hit by Spinning Ice", "Spinning Ice", 0),
                new EnemyCastEndMechanic(DeadlyIceShockWave, "Deadly Ice Shock Wave", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.White), "Deadly Ice Shock Wave", "Cast Deadly Ice Shock Wave", "Cast Deadly Ice Shock Wave", 0),
                new EnemyCastEndMechanic(IceArmSwing, "Ice Arm Swing", new MechanicPlotlySetting(Symbols.Star, Colors.White), "Ice Arm Swing", "Cast Ice Arm Swing (Spin)", "Cast Ice Arm Swing", 0),
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
                new DamageCastFinder(FrostbiteAuraIcebroodConstruct, FrostbiteAuraIcebroodConstruct),
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.IcebroodConstruct)) ?? throw new MissingKeyActorsException("Icebrood Construct not found");
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Invul check
            phases.AddRange(GetPhasesByInvul(log, Invulnerability757, mainTarget, false, true));
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
