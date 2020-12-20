using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class IcebroodConstruct : StrikeMissionLogic
    {
        public IcebroodConstruct(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new HitOnPlayerMechanic(57832, "Deadly Ice Shock Wave",new MechanicPlotlySetting("square","rgb(255,0,0)"),"D.IceWave","Deadly Ice Shock Wave", "Deadly Shock Wave", 0),
                new HitOnPlayerMechanic(57516, "Ice Arm Swing",new MechanicPlotlySetting("triangle","rgb(255,150,0)"),"A.Swing","Ice Arm Swing", "Ice Arm Swing", 0),
                new HitOnPlayerMechanic(57948, "Ice Shock Wave",new MechanicPlotlySetting("square","rgb(255,150,0)"),"ShockWave","Ice Shock Wave", "Ice Shock Wave", 0),
                new HitOnPlayerMechanic(57472, "Ice Shock Wave",new MechanicPlotlySetting("square","rgb(255,150,0)"),"ShockWave","Ice Shock Wave", "Ice Shock Wave", 0),
                new HitOnPlayerMechanic(57779, "Ice Shock Wave",new MechanicPlotlySetting("square","rgb(255,150,0)"),"ShockWave","Ice Shock Wave", "Ice Shock Wave", 0),
                new HitOnPlayerMechanic(57690, "Ice Shatter",new MechanicPlotlySetting("triangle-open","rgb(255,0,150)"),"Ice Orbs","Rotating Ice Orbs", "Ice Orbs", 50),
                new HitOnPlayerMechanic(57663, "Ice Crystal",new MechanicPlotlySetting("circle-open","rgb(255,150,0)"),"I.Crystal","Ice Crystal", "Ice Crystal", 50),
                new HitOnPlayerMechanic(57678, "Ice Flail",new MechanicPlotlySetting("square","rgb(255,0,150)"),"I.Flail","Ice Flail", "Ice Flail", 50),
                new HitOnPlayerMechanic(57463, "Ice Flail",new MechanicPlotlySetting("square","rgb(255,0,150)"),"I.Flail","Ice Flail", "Ice Flail", 50),
            }
            );
            Extension = "icebrood";
            Icon = "https://wiki.guildwars2.com/images/thumb/0/07/Icebrood_Construct_%28partially_buried%29.jpg/320px-Icebrood_Construct_%28partially_buried%29.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/sXvx6AL.png",
                            (729, 581),
                            (-32118, -11470, -28924, -8274),
                            (-0, -0, 0, 0),
                            (0, 0, 0, 0));
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.IcebroodConstruct);
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
