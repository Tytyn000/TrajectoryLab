using TrajectoryLab.Core.Mathematics;

namespace TrajectoryLab.Core.Models;

/// Paramètres utilisés pour lancer une simulation.
public sealed record SimulationParameters
{
    public required Vector3D InitialPosition { get; init; }

    public required Vector3D InitialVelocity { get; init; }

    public Vector3D Gravity { get; init; } =
        new(0.0, 0.0, -9.80665);

    // Masse du projectile en kilogrammes.
    public double Mass { get; init; } = 1.0;

    // Coefficient de traînée sans unité.
    public double DragCoefficient { get; init; } = 0.47;

    // Surface frontale en mètres carrés.
    public double CrossSectionalArea { get; init; } = 0.01;

    // Densité de l'air en kilogrammes par mètre cube.
    public double AirDensity { get; init; } = 1.225;

    // Vitesse du vent en mètres par seconde.
    public Vector3D WindVelocity { get; init; } =
        Vector3D.Zero;

    public double GroundAltitude { get; init; } = 0.0;

    public double TimeStep { get; init; } = 0.01;

    public double MaximumDuration { get; init; } = 300.0;
}