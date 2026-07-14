namespace TrajectoryLab.Application.Models.Inputs;

public sealed record SolverInput
{
    public SolverKind ModelKind { get; init; } =
        SolverKind.RungeKutta4;

    public double TimeStep { get; init; } = 0.01;

    public double AbsoluteTolerance { get; init; } = 1.0e-8;

    public double RelativeTolerance { get; init; } = 1.0e-8;

    public double MinimumTimeStep { get; init; } = 1.0e-6;

    public double MaximumTimeStep { get; init; } = 1.0;
}