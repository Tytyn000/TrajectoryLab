using System.Windows;
using TrajectoryLab.Application.Validation;
using TrajectoryLab.Desktop.Parsing;
using TrajectoryLab.Desktop.Services;
using TrajectoryLab.Desktop.ViewModels;

namespace TrajectoryLab.Desktop;

public partial class App : System.Windows.Application
{
    protected override void OnStartup(
        StartupEventArgs EventArgs)
    {
        base.OnStartup(EventArgs);

        IDoubleTextParser DoubleTextParser =
            new FlexibleDoubleTextParser();

        IWindLayerTextParser WindLayerTextParser =
            new WindLayerTextParser(DoubleTextParser);

        IDragCoefficientPointTextParser
            DragCoefficientPointTextParser =
                new DragCoefficientPointTextParser(
                    DoubleTextParser);

        ISimulationInputMapper SimulationInputMapper =
            new SimulationInputMapper(
                DoubleTextParser,
                WindLayerTextParser,
                DragCoefficientPointTextParser);

        ISimulationInputValidator SimulationInputValidator =
            new SimulationInputValidator();

        MainWindowViewModel ViewModel =
            new(
                SimulationInputMapper,
                SimulationInputValidator);

        MainWindow Window =
            new()
            {
                DataContext = ViewModel
            };

        Window.Show();
    }
}
