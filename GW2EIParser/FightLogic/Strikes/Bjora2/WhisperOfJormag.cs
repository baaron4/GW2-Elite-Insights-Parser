using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Logic
{
    public class WhisperOfJormag : StrikeMissionLogic
    {
        public WhisperOfJormag(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            }
            );
            Extension = "woj";
            Icon = "https://i.imgur.com/8GLwgfL.png";
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC woj = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.WhisperOfJormag);
            if (woj == null)
            {
                throw new InvalidOperationException("Error Encountered: Whisper of Jormag not found");
            }
            phases[0].Targets.Add(woj);
            if (!requirePhases)
            {
                return phases;
            }
            long start, end;
            var tpOutEvents = log.CombatData.GetBuffData(59223).Where(x => x is BuffRemoveAllEvent).ToList();
            var tpBackEvents = log.CombatData.GetBuffData(59054).Where(x => x is BuffRemoveAllEvent).ToList();
            // 75% tp happened
            if (tpOutEvents.Count > 0)
            {
                end = tpOutEvents.Min(x => x.Time);
                phases.Add(new PhaseData(0, end, "Pre Doppelganger 1"));
                // remove everything related to 75% tp out
                tpOutEvents.RemoveAll(x => x.Time <= end + 1000);
            }
            // 75% tp finished
            if (tpBackEvents.Count > 0)
            {
                start = tpBackEvents.Min(x => x.Time);
                // 25% tp happened
                if (tpOutEvents.Count > 0)
                {
                    end = tpOutEvents.Min(x => x.Time);
                    tpOutEvents.Clear();
                    tpBackEvents.RemoveAll(x => x.Time <= end);
                } 
                // 25% tp did not happen
                else
                {
                    end = log.FightData.FightEnd;
                    tpBackEvents.Clear();
                }
                phases.Add(new PhaseData(start, end, "Pre Doppelganger 2"));
                // 25% tp finished
                if (tpBackEvents.Count > 0)
                {
                    start = tpBackEvents.Min(x => x.Time);
                    phases.Add(new PhaseData(start, log.FightData.FightEnd, "Final"));
                }
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Targets.Add(woj);
            }
            return phases;
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                ParseEnum.TrashIDS.WhisperEcho,
                ParseEnum.TrashIDS.DoppelgangerGuardian1,
                ParseEnum.TrashIDS.DoppelgangerGuardian2,
                ParseEnum.TrashIDS.DoppelgangerNecro,
                ParseEnum.TrashIDS.DoppelgangerRevenant,
                ParseEnum.TrashIDS.DoppelgangerThief1,
                ParseEnum.TrashIDS.DoppelgangerThief2,
                ParseEnum.TrashIDS.DoppelgangerWarrior,
            };
        }
    }
}
