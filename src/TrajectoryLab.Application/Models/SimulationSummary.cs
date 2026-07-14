namespace TrajectoryLab.Application.Models;

public sealed record SimulationSummary(
    double FlightTime,
    double Range,
    double MaximumAltitude,
    double ImpactSpeed,
    double NorthDeviation,
    double MaximumMachNumber,
    int StateCount,
    string SolverName);
