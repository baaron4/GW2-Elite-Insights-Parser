using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using GW2EIParserAvalonia.Services;
using GW2EIParserAvalonia.ViewModels;
using GW2EIParserAvalonia.Views;
using GW2EIParserCommons;

namespace GW2EIParserAvalonia;

public partial class App : Application
{
    private const int ArgFaulty = 1;
    private const int InspectLogFaulty = 2;

    public CommandLineOptions? CommandLineOptions { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void EarlyShutdown(IClassicDesktopStyleApplicationLifetime desktop, int exitCode)
    {
        desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        Dispatcher.UIThread.Post(() => desktop.Shutdown(exitCode));
        base.OnFrameworkInitializationCompleted();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            CommandLineOptions = new CommandWindow(desktop.Args).Initialize();
            if (CommandLineOptions is null)
            {
                EarlyShutdown(desktop, ArgFaulty);
                return;
            }
            else if (CommandLineOptions == CommandWindow.HelpCommandLineOptions)
            {
                EarlyShutdown(desktop, 0);
                return;
            }

            ApplicationTrace trace = new();

            if (CommandLineOptions.InspectOnly)
            {
                var Settings = CustomSettingsManager.GetProgramSettings();
                using var parserService = new ParserService(Settings);
                var controller = new InspectorOperationController(CommandLineOptions.LogFiles[0]);
                parserService.InspectParseSync(controller);
                if (controller.InspectLog != null)
                {
                    desktop.MainWindow = new InspectorWindow(controller.InspectLog, trace)
                    {
                        DataContext = new InspectorViewModel(controller.InspectLog),
                    };
                }
                else
                {
                    EarlyShutdown(desktop, InspectLogFaulty);
                    return;
                }
            }
            else
            {
                desktop.MainWindow = new MainWindow(trace)
                {
                    DataContext = new MainWindowViewModel(trace, CommandLineOptions),
                };
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}
