using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class Xera : BossLogic
    {
        public Xera() : base()
        {
            mode = ParseMode.Raid;
            mechanicList.AddRange(new List<Mechanic>
            {

            new Mechanic(35128, "Temporal Shred", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Xera, "symbol:'circle',color:'rgb(255,0,0)',", "Orb",0), //Temporal Shred (Hit by Red Orb), Red Orb
            new Mechanic(34913, "Temporal Shred", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Xera, "symbol:'circle-open',color:'rgb(255,0,0)',", "O.Aoe",0),//Temporal Shred (Stood in Orb Aoe), Orb AoE
            /*new Mechanic(35000, "Intervention", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'hourglass',color:'rgb(128,0,128)',", "Bubble",0),*/
            new Mechanic(35168, "Bloodstone Protection", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'hourglass-open',color:'rgb(128,0,128)',", "InBble",0), //Bloodstone Protection (Stood in Bubble), Inside Bubble
            new Mechanic(34887, "Summon Fragment Start", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Xera, "symbol:'diamond-tall',color:'rgb(255,0,255)',", "Breakbar",0),
            new Mechanic(34887, "Summon Fragment End", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Xera, "symbol:'diamond-tall',color:'rgb(255,0,255)',", "CC.Fail",0,delegate(long value){return value == 11940;}), //Summon Fragment (Failed CC), CC Fail
            new Mechanic(34965, "Derangement", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'square-open',color:'rgb(200,140,255)',", "Drgmnt",0), //Derangement (Stacking Debuff), Derangement
            new Mechanic(35084, "Bending Chaos", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'triangle-down-open',color:'rgb(255,200,0)',", "Btn1",0), //Bending Chaos (Stood on 1st Button), Button 1
            new Mechanic(35162, "Shifting Chaos", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'triangle-ne-open',color:'rgb(255,200,0)',", "Btn2",0), //Bending Chaos (Stood on 2nd Button), Button 2
            new Mechanic(35032, "Twisting Chaos", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'triangle-nw-open',color:'rgb(255,200,0)',", "Btn3",0), //Bending Chaos (Stood on 3rd Button), Button 3
            new Mechanic(34956, "Intervention", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'square',color:'rgb(0,0,255)',", "Shld",0), //Intervention (got Special Action Key), Shield
            new Mechanic(34921, "Gravity Well", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Xera, "symbol:'circle-x-open',color:'rgb(255,0,255)',", "GrWell",4000), //Half-platform Gravity Well, Gravity Well
            //teleport
            new Mechanic(35034, "Disruption", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Xera, "symbol:'square',color:'rgb(0,128,0)',", "TP",0), //Disruption (Teleport to Platform),  TP
            new Mechanic(34997, "Teleport Out", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'circle',color:'rgb(255,0,255)',", "TP.Out",0), //Teleport Out (Took Portal), TP Portal
            new Mechanic(35076, "Hero's Return", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'circle',color:'rgb(0,128,0)',", "TP.Back",0), //Hero's Return (Teleport back), TP back
            });
        }

        public override CombatReplayMap getCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/BoHwwY6.png",
                            Tuple.Create(7112, 6377),
                            Tuple.Create(-5992, -5992, 69, -522),
                            Tuple.Create(-12288, -27648, 12288, 27648),
                            Tuple.Create(1920, 12160, 2944, 14464));
        }

        public override List<PhaseData> getPhases(Boss boss, ParsedLog log, List<CastLog> cast_logs)
        {
            long start = 0;
            long end = 0;
            long fight_dur = log.getBossData().getAwareDuration();
            List<PhaseData> phases = getInitialPhase(log);
            // split happened
            if (boss.getPhaseData().Count == 1)
            {
                CombatItem invulXera = log.getBoonData().Find(x => x.getDstInstid() == boss.getInstid() && (x.getSkillID() == 762 || x.getSkillID() == 34113));
                end = invulXera.getTime() - log.getBossData().getFirstAware();
                phases.Add(new PhaseData(start, end));
                start = boss.getPhaseData()[0] - log.getBossData().getFirstAware();
                cast_logs.Add(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None));
            }
            if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
            {
                phases.Add(new PhaseData(start, fight_dur));
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].setName("Phase " + i);
            }
            return phases;
        }

        public override List<ParseEnum.ThrashIDS> getAdditionalData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            // TODO: needs facing information for hadouken
            List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
            {
                ParseEnum.ThrashIDS.WhiteMantleSeeker1,
                ParseEnum.ThrashIDS.WhiteMantleSeeker2,
                ParseEnum.ThrashIDS.WhiteMantleKnight,
                ParseEnum.ThrashIDS.WhiteMantleBattleMage,
                ParseEnum.ThrashIDS.ExquisiteConjunction
            };
            List<CastLog> summon = cls.Where(x => x.getID() == 34887).ToList();
            foreach (CastLog c in summon)
            {
                replay.addCircleActor(new CircleActor(true, 0, 180, new Tuple<int, int>((int)c.getTime(), (int)c.getTime() + c.getActDur()), "rgba(0, 180, 255, 0.3)"));
            }
            return ids;
        }

        public override string getReplayIcon()
        {
            return "https://i.imgur.com/lYwJEyV.png";
        }
    }
}
