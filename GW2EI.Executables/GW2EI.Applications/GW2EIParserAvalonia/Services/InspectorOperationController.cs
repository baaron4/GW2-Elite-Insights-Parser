using GW2EIEvtcParser;
using GW2EIParserCommons;

namespace GW2EIParserAvalonia.Services;

public class InspectorOperationController : OperationController
{

    public ParsedEvtcLog? InspectLog { get; internal set; }
    public bool Errored { get; internal set;  }
    public InspectorOperationController(string location) : base(location, "Ready to parse")
    {

    }
}
