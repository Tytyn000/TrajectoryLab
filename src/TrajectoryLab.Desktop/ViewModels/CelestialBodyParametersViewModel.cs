using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Desktop.Models;

namespace TrajectoryLab.Desktop.ViewModels;

public sealed class CelestialBodyParametersViewModel
{
    public IReadOnlyList<SelectionOption<GravityModelKind>>
        GravityModelOptions =>
            SimulationOptionCatalog.GravityModels;

    public IReadOnlyList<SelectionOption<UniformSphereDefinitionKind>>
        UniformSphereDefinitionOptions =>
            SimulationOptionCatalog.UniformSphereDefinitions;

    public GravityModelKind SelectedGravityModel { get; set; } =
        GravityModelKind.UniformSphere;

    public UniformSphereDefinitionKind
        SelectedUniformSphereDefinition { get; set; } =
            UniformSphereDefinitionKind.SurfaceGravity;

    public string BodyRadius { get; set; } = "6371000";
    public string SurfaceGravityAcceleration { get; set; } = "9.80665";
    public string BodyDensity { get; set; } = "5513.258738589093";
    public string BodyMass { get; set; } = "5.972168494074286e24";
}