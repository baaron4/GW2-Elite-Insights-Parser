using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class IcebroodConstruct : IBSStrike
    {
        public IcebroodConstruct(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new HitOnPlayerMechanic(57832, "Deadly Ice Shock Wave",new MechanicPlotlySetting(Symbols.Square,Colors.Red),"D.IceWave","Deadly Ice Shock Wave", "Deadly Shock Wave", 0),
                new HitOnPlayerMechanic(57516, "Ice Arm Swing",new MechanicPlotlySetting(Symbols.TriangleUp,Colors.LightOrange),"A.Swing","Ice Arm Swing", "Ice Arm Swing", 0),
                new HitOnPlayerMechanic(new long[] {57948, 57472, 57779 }, "Ice Shock Wave",new MechanicPlotlySetting(Symbols.Square,Colors.LightOrange),"ShockWave","Ice Shock Wave", "Ice Shock Wave", 0),
                new HitOnPlayerMechanic(57690, "Ice Shatter",new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Pink),"Ice Orbs","Rotating Ice Orbs", "Ice Orbs", 50),
                new HitOnPlayerMechanic(57663, "Ice Crystal",new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange),"I.Crystal","Ice Crystal", "Ice Crystal", 50),
                new HitOnPlayerMechanic(new long[] {57678, 57463 }, "Ice Flail",new MechanicPlotlySetting(Symbols.Square,Colors.Pink),"I.Flail","Ice Flail", "Ice Flail", 50),
            }
            );
            Extension = "icebrood";
            Icon = "https://wiki.guildwars2.com/images/thumb/0/07/Icebrood_Construct_%28partially_buried%29.jpg/320px-Icebrood_Construct_%28partially_buried%29.jpg";
            EncounterCategoryInformation.SubCategory = EncounterCategory.SubFightCategory.Grothmar;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/sXvx6AL.png",
                            (729, 581),
                            (-32118, -11470, -28924, -8274)/*,
                            (-0, -0, 0, 0),
                            (0, 0, 0, 0)*/);
        }
        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(57593, 57593, InstantCastFinder.DefaultICD), // Frostbite Aura
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.IcebroodConstruct);
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
