using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Desktop.Models;

namespace TrajectoryLab.Desktop.ViewModels;

public sealed class AtmosphereParametersViewModel
{
    public IReadOnlyList<SelectionOption<AtmosphereModelKind>>
        AtmosphereModelOptions =>
            SimulationOptionCatalog.AtmosphereModels;

    public IReadOnlyList<SelectionOption<ConstantAtmosphereDefinitionKind>>
        ConstantAtmosphereDefinitionOptions =>
            SimulationOptionCatalog.ConstantAtmosphereDefinitions;

    public AtmosphereModelKind SelectedAtmosphereModel { get; set; } =
        AtmosphereModelKind.Vacuum;

    public ConstantAtmosphereDefinitionKind
        SelectedConstantAtmosphereDefinition { get; set; } =
            ConstantAtmosphereDefinitionKind
                .DensityAndTemperature;

    public string ConstantAirDensity { get; set; } = "1.225";
    public string ConstantPressure { get; set; } = "101325";
    public string ConstantTemperature { get; set; } = "288.15";
}