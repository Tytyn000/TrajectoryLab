using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Desktop.Models;

namespace TrajectoryLab.Desktop.ViewModels;

public sealed class RotationParametersViewModel
{
    public IReadOnlyList<SelectionOption<CoriolisDefinitionKind>>
        CoriolisDefinitionOptions =>
            SimulationOptionCatalog.CoriolisDefinitions;

    public bool IsCoriolisEnabled { get; set; }

    public CoriolisDefinitionKind
        SelectedCoriolisDefinition { get; set; } =
            CoriolisDefinitionKind.Latitude;

    public string LatitudeDegrees { get; set; } = "48.8566";
    public string AngularVelocity { get; set; } = "7.292115e-5";
    public string AngularVelocityX { get; set; } = "0";
    public string AngularVelocityY { get; set; } = "0";
    public string AngularVelocityZ { get; set; } = "7.292115e-5";
}