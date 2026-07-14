using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Desktop.Models;

namespace TrajectoryLab.Desktop.ViewModels;

public sealed class WindParametersViewModel
{
    public IReadOnlyList<SelectionOption<WindModelKind>>
        WindModelOptions =>
            SimulationOptionCatalog.WindModels;

    public WindModelKind SelectedWindModel { get; set; } =
        WindModelKind.None;

    public string ConstantWindX { get; set; } = "0";
    public string ConstantWindY { get; set; } = "0";
    public string ConstantWindZ { get; set; } = "0";

    public string LowerAltitude { get; set; } = "0";
    public string LowerWindX { get; set; } = "0";
    public string LowerWindY { get; set; } = "0";
    public string LowerWindZ { get; set; } = "0";

    public string UpperAltitude { get; set; } = "1000";
    public string UpperWindX { get; set; } = "0";
    public string UpperWindY { get; set; } = "0";
    public string UpperWindZ { get; set; } = "0";

    public string LayeredWindProfile { get; set; } =
        "0;0;0;0" + Environment.NewLine +
        "1000;0;0;0";
}