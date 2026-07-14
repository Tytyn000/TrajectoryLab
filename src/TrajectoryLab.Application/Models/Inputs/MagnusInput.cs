namespace TrajectoryLab.Application.Models.Inputs;

public sealed record MagnusInput
{
    public bool IsEnabled { get; init; }

    public double AngularVelocityX { get; init; }

    public double AngularVelocityY { get; init; }

    public double AngularVelocityZ { get; init; }

    public double MagnusCoefficient { get; init; } = 0.0;
}