using System;
using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using static GW2EIParser.Parser.ParseEnum.TrashIDS;

namespace GW2EIParser.Logic
{
    public class MAMA : FractalLogic
    {
        public MAMA(ushort triggerID) : base(triggerID)
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

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/lFGNKuf.png",
                            (664, 407),
                            (1653, 4555, 5733, 7195),
                            (-6144, -6144, 9216, 9216),
                            (11804, 4414, 12444, 5054));
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mama = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.MAMA);
            if (mama == null)
            {
                throw new InvalidOperationException("Error Encountered: MAMA not found");
            }
            phases[0].Targets.Add(mama);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, 762, mama, true, true));
            string[] namesMAMA = new[] { "Phase 1", "Red Knight", "Phase 2", "Green Knight", "Phase 3", "Blue Knight", "Phase 4" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = namesMAMA[i - 1];
                if (i == 2 || i == 4 || i == 6)
                {
                    var ids = new List<ushort>
                    {
                       (ushort) GreenKnight,
                       (ushort) RedKnight,
                       (ushort) BlueKnight,
                    };
                    AddTargetsToPhase(phase, ids, log);
                }
                else
                {
                    phase.Targets.Add(mama);
                }
            }
            return phases;
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                GreenKnight,
                RedKnight,
                BlueKnight,
                TwistedHorror
            };
        }

        public override long GetFightOffset(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            return GetFightOffsetByFirstInvulFilter(fightData, agentData, combatData, (ushort)ParseEnum.TargetIDS.MAMA, 762, 2000);
        }
    }
}
