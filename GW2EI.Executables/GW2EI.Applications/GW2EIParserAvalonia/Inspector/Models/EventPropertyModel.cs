using System.Collections.ObjectModel;

namespace GW2EIParserAvalonia.Models;

internal sealed class EventPropertyModel
{
    public string Name { get; }
    public string Value { get; }
    public ObservableCollection<EventPropertyModel> Children { get; } = [];

    public EventPropertyModel(string name, string value)
    {
        Name = name;
        Value = value;
    }
}
