namespace TrajectoryLab.Application.Models.Inputs;

public sealed record DragCoefficientPointInput
{
    public double MachNumber { get; init; }

    public double DragCoefficient { get; init; }
}