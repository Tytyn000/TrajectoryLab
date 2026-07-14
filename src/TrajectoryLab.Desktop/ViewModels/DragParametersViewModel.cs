using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Desktop.Models;

namespace TrajectoryLab.Desktop.ViewModels;

public sealed class DragParametersViewModel
{
    public IReadOnlyList<SelectionOption<DragCoefficientModelKind>>
        DragCoefficientModelOptions =>
            SimulationOptionCatalog.DragCoefficientModels;

    public bool IsEnabled { get; set; }

    public DragCoefficientModelKind
        SelectedDragCoefficientModel { get; set; } =
            DragCoefficientModelKind.Constant;

    public string TabulatedDragProfile { get; set; } =
        "0;0.47" + Environment.NewLine +
        "1;0.47";
}