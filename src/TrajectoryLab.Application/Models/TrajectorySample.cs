namespace TrajectoryLab.Application.Models;

public sealed record TrajectorySample(
    double Time,
    double X,
    double Y,
    double Z,
    double HorizontalDistance,
    double Speed);
