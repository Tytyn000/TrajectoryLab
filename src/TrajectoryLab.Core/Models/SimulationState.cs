using TrajectoryLab.Core.Mathematics;

namespace TrajectoryLab.Core.Models;

/// <summary>
/// Représente l'état du projectile à un instant donné.
/// </summary>
public readonly record struct SimulationState(
    double Time,
    Vector3D Position,
    Vector3D Velocity
);