using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class Golem : BossLogic
    {
        public Golem() : base()
        {
            mode = ParseMode.Golem;          
        }

        public override List<PhaseData> getPhases(Boss boss, ParsedLog log, List<CastLog> cast_logs)
        {
            List<PhaseData> phases = getInitialPhase(log);          
            return phases;
        }     
    }
}
