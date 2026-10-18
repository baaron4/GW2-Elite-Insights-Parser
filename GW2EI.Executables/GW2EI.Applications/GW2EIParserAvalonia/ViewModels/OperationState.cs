namespace GW2EIParserAvalonia.ViewModels;

internal enum OperationState
{
    Ready = 0,
    Parsing = 1,
    Cancelling = 2,
    Complete = 3,
    Pending = 4,
    ClearOnCancel = 5,
    Queued = 6,
    UnComplete = 7,
    RemoveFromQueue = 8,
    RemoveFromQueueAndClear = 9,
}
