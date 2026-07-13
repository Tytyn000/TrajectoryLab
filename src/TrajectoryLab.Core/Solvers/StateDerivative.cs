using TrajectoryLab.Core.Mathematics;

namespace TrajectoryLab.Core.Solvers;

// Dérivées de la position et de la vitesse.
public readonly record struct StateDerivative(
    Vector3D PositionDerivative,
    Vector3D VelocityDerivative
);