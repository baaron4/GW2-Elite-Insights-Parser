using Avalonia.Controls;
using GW2EIEvtcParser;
using GW2EIParserAvalonia.Services;
using GW2EIParserAvalonia.ViewModels;

namespace GW2EIParserAvalonia;

public partial class InspectorWindow : Window
{
    private readonly IApplicationTrace _trace = null!;

    public InspectorWindow()
    {
        InitializeComponent();
    }

    public InspectorWindow(RawEvtcLog log, IApplicationTrace trace)
    {
        _trace = trace;

        InitializeComponent();

        DataContext = new InspectorViewModel(log);
    }
}
