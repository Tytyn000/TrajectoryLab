namespace TrajectoryLab.Desktop.Models;

public sealed record SelectionOption<TValue>(
    TValue Value,
    string Label);