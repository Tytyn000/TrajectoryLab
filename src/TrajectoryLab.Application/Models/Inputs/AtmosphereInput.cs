namespace TrajectoryLab.Application.Models.Inputs;

public sealed record AtmosphereInput
{
    public AtmosphereModelKind ModelKind { get; init; } =
        AtmosphereModelKind.Vacuum;

    public ConstantAtmosphereDefinitionKind ConstantDefinition { get; init; } =
        ConstantAtmosphereDefinitionKind.DensityAndTemperature;

    public double ConstantAirDensity { get; init; } =
        1.225;

    public double ConstantPressure { get; init; } =
        101_325.0;

    public double ConstantTemperature { get; init; } =
        288.15;
}
