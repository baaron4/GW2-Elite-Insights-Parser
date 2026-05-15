
using System.Text.Json;
using System.Text.Json.Serialization;
using GW2EIParserCommons;

namespace GW2EIParser;
internal class ConsoleResultObject
{
    public static readonly JsonSerializerOptions Serializer = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
    };

    private readonly OperationController Controller;
    public string FileName => Controller.InputFile;

    public bool Parsed => Controller.Parsed;
    public string Status => Controller.Status;
    public IReadOnlyList<string> GeneratedFiles => Controller.GeneratedFiles;
    public bool DPSReportUploadTentative => Controller.DPSReportUploadTentative;
    public bool DPSReportUploadFailed => Controller.DPSReportUploadFailed;
    public string DPSReportLink => Controller.DPSReportLink;
    public bool WingmanUploadTentative => Controller.WingmanUploadTentative;
    public bool WingmanUploadFailed => Controller.WingmanUploadFailed;
    public bool WingmanUploadRefused => Controller.WingmanUploadRefused;
    public bool MistWarriorUploadTentative  => Controller.MistWarriorUploadTentative;
    public bool MistWarriorUploadFailed => Controller.MistWarriorUploadFailed;

    public long Elapsed => Controller.Elapsed;

    public ConsoleResultObject(OperationController controller)
    {
        Controller = controller;
    }
}
