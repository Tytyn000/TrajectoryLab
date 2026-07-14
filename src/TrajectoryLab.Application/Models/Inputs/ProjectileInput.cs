namespace TrajectoryLab.Application.Models.Inputs;

public sealed record ProjectileInput
{
    public double Mass { get; init; } = 1.0;

    public double CrossSectionalArea { get; init; } = 0.01;

    public double ReferenceDragCoefficient { get; init; } = 0.47;

    public double Radius { get; init; } = 0.05;
}