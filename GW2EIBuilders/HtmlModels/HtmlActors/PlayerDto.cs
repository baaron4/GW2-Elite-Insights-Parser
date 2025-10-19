using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.HTMLActors;


internal class PlayerDto : ActorDto
{

    internal class PlayerWeaponSetDto
    {
        public readonly List<string> L1Set = [];
        public readonly List<string> L2Set = [];
        public readonly List<string> A1Set = [];
        public readonly List<string> A2Set = [];
        public readonly List<long> Timeframe = [];
    }

    public int Group;
    public string Acc;
    public string Profession;

    public bool IsPoV;

    public bool IsCommander;
    public List<string[]>? CommanderStates;
    public readonly List<string> L1Set = [];
    public readonly List<string> L2Set = [];
    public readonly List<string> A1Set = [];
    public readonly List<string> A2Set = [];
    public string ColTarget;
    public string ColCleave;
    public string ColTotal;
    public bool IsFake;
    public bool NotInSquad;

    private static (string, string, string) GetSpecGraphColor(ParserHelper.Spec baseSpec)
    {
        switch (baseSpec)
        {

            case ParserHelper.Spec.Warrior:
                return ("rgb(255,209,102)", "rgb(190,159,84)", "rgb(125,109,66)");
            case ParserHelper.Spec.Guardian:
                return ("rgb(114,193,217)", "rgb(88,147,165)", "rgb(62,101,113)");
            case ParserHelper.Spec.Revenant:
                return ("rgb(209,110,90)", "rgb(159,85,70)", "rgb(110,60,50)");
            case ParserHelper.Spec.Engineer:
                return ("rgb(208,156,89)", "rgb(158,119,68)", "rgb(109,83,48)");
            case ParserHelper.Spec.Ranger:
                return ("rgb(140,220,130)", "rgb(107,167,100)", "rgb(75,115,70)");
            case ParserHelper.Spec.Thief:
                return ("rgb(192,143,149)", "rgb(146,109,114)", "rgb(101,76,79)");
            case ParserHelper.Spec.Elementalist:
                return ("rgb(246,138,135)", "rgb(186,106,103)", "rgb(127,74,72)");
            case ParserHelper.Spec.Mesmer:
                return ("rgb(182,121,213)", "rgb(139,90,162)", "rgb(96,60,111)");
            case ParserHelper.Spec.Necromancer:
                return ("rgb(82,167,111)", "rgb(64,127,85)", "rgb(46,88,60)");
            default:
                return ("", "", "");
        }

    }

    public PlayerDto(SingleActor actor, ParsedEvtcLog log, ActorDetailsDto details) : base(actor, log, details)
    {
        Group = actor.Group;
        Acc = actor.Account;
        Profession = actor.Spec.ToString();
        IsPoV = actor.AgentItem.Is(log.LogMetadata.PoV);
        if (actor is Player p)
        {
            IsCommander = p.IsCommander(log);
            CommanderStates = IsCommander ? p.GetCommanderStatesNoTagValues(log).Select(x => new string[2] { ParserHelper.ToDurationString(x.Start), ParserHelper.ToDurationString(x.End) }).ToList() : null;
        }
        (ColTarget, ColCleave, ColTotal) = GetSpecGraphColor(actor.BaseSpec);
        IsFake = actor.IsFakeActor;
        NotInSquad = !(actor is Player);
        BuildWeaponSets(actor, log);
    }

    private static void BuildWeaponSets((string mh, string oh) set, List<string> listToSet)
    {
        if (set.mh == WeaponSet.Unknown && set.oh == WeaponSet.Unknown)
        {
            return;
        }
        listToSet.Add(set.mh);
        if (set.oh != WeaponSet.TwoHand)
        {
            listToSet.Add(set.oh);
        }
    }

    private void BuildWeaponSets(SingleActor actor, ParsedEvtcLog log)
    {
        // TODO transform LXSet and AXSet into a class
        WeaponSet weps = actor.GetWeaponSets(log)[0];
        BuildWeaponSets(weps.LandSet1, L1Set);
        BuildWeaponSets(weps.LandSet2, L2Set);
        BuildWeaponSets(weps.WaterSet1, A1Set);
        BuildWeaponSets(weps.WaterSet2, A2Set);
    }
}
