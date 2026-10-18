using Avalonia.Controls;
using Avalonia.Interactivity;
using GW2EIParserAvalonia.Services;

namespace GW2EIParserAvalonia.Views;

internal partial class MessageWindow : Window
{
    public string Message { get; } = string.Empty;
    private readonly IApplicationTrace _trace = null!;

    public MessageWindow()
    {
        InitializeComponent();
    }

    public MessageWindow(string message, IApplicationTrace trace, string title = "GW2 Elite Insights Parser")
    {
        InitializeComponent();

        Title = title;
        Message = message;
        _trace = trace;

        DataContext = this;
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        _trace.Add("UI: Message Window Closed");
        Close();
    }
}
