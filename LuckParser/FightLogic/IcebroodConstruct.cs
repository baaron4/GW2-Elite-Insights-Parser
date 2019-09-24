using System;
using System.Collections.Generic;
using LuckParser.EIData;
using LuckParser.Parser;

namespace LuckParser.Logic
{
    public class IcebroodConstruct : StrikeMissionLogic
    {
        public IcebroodConstruct(ushort triggerID) : base(triggerID)
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
            Icon = "https://wiki.guildwars2.com/images/thumb/e/e2/Icebrood_Construct.jpg/320px-Icebrood_Construct.jpg";
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.IcebroodConstruct);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Invul check
            phases.AddRange(GetPhasesByInvul(log, 757, mainTarget, false, true));
            string[] phaseNames = new[] { "Phase 1", "Phase 2"};
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = phaseNames[i - 1];
                phase.Targets.Add(mainTarget);
            }
            return phases;
        }
    }
}
