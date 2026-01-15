using System.Numerics;
using GW2EIEvtcParser.LogLogic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParsedData.LogData;

namespace GW2EIEvtcParser.EIData;

public abstract class PhaseDataWithMetaData : PhaseData
{
    public readonly bool Success;
    public readonly Mode Mode = Mode.NotSet;
    public readonly StartStatus StartStatus = StartStatus.NotSet;
    public bool IsLateStart => StartStatus == StartStatus.Late || MissingPreEvent;
    public bool MissingPreEvent => StartStatus == StartStatus.NoPreEvent;
    public bool IsCM => Mode == Mode.CMNoName || Mode == Mode.CM;
    public bool IsLegendaryCM => Mode == Mode.LegendaryCM;

    public readonly long ID = LogIDs.Unknown;

    public string NameNoMode { get; internal set; } = "";
    public readonly string Icon;

    internal PhaseDataWithMetaData(long start, long end, string name, bool success, string icon, Mode mode, long logID, PhaseType type) : base(start, end, type)
    {
        ID = logID;
        Success = success;
        Mode = mode;
        Icon = icon;
        NameNoMode = name;
        Name = name
            + (Mode == Mode.CM ? " CM" : "")
            + (Mode == Mode.LegendaryCM ? " LCM" : "")
            + (Mode == Mode.Story ? " Story" : "");
    }

    internal PhaseDataWithMetaData(long start, long end, string name, bool success, string icon, Mode mode, StartStatus startStatus, long logID, PhaseType type) : this(start, end, name, success, icon, mode, logID, type)
    {
        StartStatus = startStatus;
        Name = name
            + (IsLateStart && !MissingPreEvent ? " (Late Start)" : "")
            + (MissingPreEvent ? " (No Pre-Event)" : ""); ;
    }

    internal PhaseDataWithMetaData(long start, long end, string name, ParsedEvtcLog log, PhaseType type) : this(start, end, name, log.LogData.Success, log.LogData.Logic.Icon, log.LogData.LogMode, log.LogData.Logic.LogID, type)
    {
        StartStatus = log.LogData.LogStartStatus;
        Name = NameNoMode;
    }
}
