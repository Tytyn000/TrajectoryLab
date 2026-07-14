namespace TrajectoryLab.Application.Models.Inputs;

public sealed record InitialConditionsInput
{
    public double InitialX { get; init; } = 0.0;

    public double InitialY { get; init; } = 0.0;

    public double InitialZ { get; init; } = 0.0;

    public double InitialSpeed { get; init; } = 50.0;

    public double ElevationDegrees { get; init; } = 45.0;

    public double AzimuthDegrees { get; init; } = 0.0;
}