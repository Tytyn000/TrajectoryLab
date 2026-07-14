namespace TrajectoryLab.Application.Models.Inputs;

public sealed record SimulationLimitsInput
{
    public double MaximumSimulationTime { get; init; } = 60.0;

    public double GroundAltitude { get; init; } = 0.0;
}