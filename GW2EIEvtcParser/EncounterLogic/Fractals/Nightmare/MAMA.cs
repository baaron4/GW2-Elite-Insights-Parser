using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class MAMA : Nightmare
    {
        public MAMA(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(37408, "Blastwave", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "KB","Blastwave (Spinning Knockback)", "KB Spin",0),
            new HitOnPlayerMechanic(37103, "Blastwave", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "KB","Blastwave (Spinning Knockback)", "KB Spin",0),
            new HitOnPlayerMechanic(37391, "Tantrum", new MechanicPlotlySetting("star-diamond-open","rgb(0,255,0)"), "Tantrum","Tantrum (Double hit or Slams)", "Dual Spin/Slams",700),
            new HitOnPlayerMechanic(37577, "Leap", new MechanicPlotlySetting("triangle-down","rgb(255,0,0)"), "Jump","Leap (<33% only)", "Leap",0),
            new HitOnPlayerMechanic(37437, "Shoot", new MechanicPlotlySetting("circle-open","rgb(130,180,0)"), "Shoot","Toxic Shoot (Green Bullets)", "Toxic Shoot",0),
            new HitOnPlayerMechanic(37185, "Explosive Impact", new MechanicPlotlySetting("circle","rgb(255,200,0)"), "Knight Jump","Explosive Impact (Knight Jump)", "Knight Jump",0),
            new HitOnPlayerMechanic(37085, "Sweeping Strikes", new MechanicPlotlySetting("asterisk-open","rgb(255,0,0)"), "Sweep","Swings (Many rapid front spins)", "Sweeping Strikes",200),
            new HitOnPlayerMechanic(37217, "Nightmare Miasma", new MechanicPlotlySetting("circle-open","rgb(255,0,255)"), "Goo","Nightmare Miasma (Poison Puddle)", "Poison Goo",700),
            new HitOnPlayerMechanic(37180, "Grenade Barrage", new MechanicPlotlySetting("circle-open","rgb(255,200,0)"), "Barrage","Grenade Barrage (many projectiles in all directions)", "Ball Barrage",0),
            new HitOnPlayerMechanic(37173, "Red Ball Shot", new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Ball","Small Red Bullets", "Bullet",0),
            new HitOnPlayerMechanic(36903, "Extraction", new MechanicPlotlySetting("bowtie","rgb(255,140,0)"), "Pull","Extraction (Knight Pull Circle)", "Knight Pull",0),
            new HitOnPlayerMechanic(36887, "Homing Grenades", new MechanicPlotlySetting("star-triangle-down-open","rgb(255,0,0)"), "Grenades","Homing Grenades", "Homing Grenades",0),
            new HitOnPlayerMechanic(37303, "Cascade of Torment", new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0),
            new HitOnPlayerMechanic(36984, "Cascade of Torment", new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0),
            new HitOnPlayerMechanic(37315, "Knight's Daze", new MechanicPlotlySetting("square-open","rgb(200,140,255)"), "Daze","Knight's Daze", "Daze", 0),

            });
            Extension = "mama";
            Icon = "http://dulfy.net/wp-content/uploads/2016/11/gw2-nightmare-fractal-teaser.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/lFGNKuf.png",
                            (664, 407),
                            (1653, 4555, 5733, 7195),
                            (-6144, -6144, 9216, 9216),
                            (11804, 4414, 12444, 5054));
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.CMStatus.CMnoName;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mama = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.MAMA);
            if (mama == null)
            {
                throw new InvalidOperationException("MAMA not found");
            }
            phases[0].Targets.Add(mama);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, 762, mama, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i%2 == 0)
                {
                    var ids = new List<int>
                    {
                       (int) ArcDPSEnums.TrashID.GreenKnight,
                       (int) ArcDPSEnums.TrashID.RedKnight,
                       (int) ArcDPSEnums.TrashID.BlueKnight,
                    };
                    AddTargetsToPhase(phase, ids, log);
                    if (phase.Targets.Count > 0)
                    {
                        NPC phaseTar = phase.Targets[0];
                        switch (phaseTar.ID)
                        {
                            case (int)ArcDPSEnums.TrashID.GreenKnight:
                                phase.Name = "Green Knight";
                                break;
                            case (int)ArcDPSEnums.TrashID.RedKnight:
                                phase.Name = "Red Knight";
                                break;
                            case (int)ArcDPSEnums.TrashID.BlueKnight:
                                phase.Name = "Blue Knight";
                                break;
                            default:
                                throw new InvalidOperationException("Unknown phase target in MAMA");
                        }
                    }
                }
                else
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.Targets.Add(mama);
                }
            }
            return phases;
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.MAMA,
                (int)ArcDPSEnums.TrashID.GreenKnight,
                (int)ArcDPSEnums.TrashID.RedKnight,
                (int)ArcDPSEnums.TrashID.BlueKnight
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.TwistedHorror
            };
        }

        internal override long GetFightOffset(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            return GetFightOffsetByFirstInvulFilter(fightData, agentData, combatData, (int)ArcDPSEnums.TargetID.MAMA, 762, 1500);
        }
    }
}
