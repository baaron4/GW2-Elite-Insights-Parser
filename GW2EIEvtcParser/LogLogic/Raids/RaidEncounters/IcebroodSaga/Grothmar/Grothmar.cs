namespace GW2EIEvtcParser.LogLogic;

internal class Grothmar : IcebroodSagaRaidEncounter
{
    public Grothmar(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = LogCategories.SubLogCategory.Grothmar;
    }
}
