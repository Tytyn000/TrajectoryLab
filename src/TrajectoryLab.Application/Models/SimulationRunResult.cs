namespace TrajectoryLab.Application.Models;

public sealed record SimulationRunResult(
    IReadOnlyList<TrajectorySample> Samples,
    SimulationSummary Summary);
