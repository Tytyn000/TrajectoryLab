using TrajectoryLab.Core.Mathematics;

namespace TrajectoryLab.Core.Models;

// Regroupe les conditions initiales et les paramètres d'une simulation.
public sealed class SimulationParameters
{
    public Vector3D InitialPosition { get; }

    public Vector3D InitialVelocity { get; }

    public ProjectileParameters Projectile { get; }

    public EnvironmentParameters Environment { get; }

    public SimulationSettings Settings { get; }

    public SimulationParameters(
        Vector3D InitialPosition,
        Vector3D InitialVelocity,
        ProjectileParameters Projectile,
        EnvironmentParameters Environment,
        SimulationSettings Settings)
    {
        ArgumentNullException.ThrowIfNull(Projectile);
        ArgumentNullException.ThrowIfNull(Environment);
        ArgumentNullException.ThrowIfNull(Settings);

        ValidateVector(
            InitialPosition,
            nameof(InitialPosition)
        );

        ValidateVector(
            InitialVelocity,
            nameof(InitialVelocity)
        );

        this.InitialPosition = InitialPosition;
        this.InitialVelocity = InitialVelocity;
        this.Projectile = Projectile;
        this.Environment = Environment;
        this.Settings = Settings;
    }

    private static void ValidateVector(
        Vector3D Vector,
        string ParameterName)
    {
        if (!double.IsFinite(Vector.X) ||
            !double.IsFinite(Vector.Y) ||
            !double.IsFinite(Vector.Z))
        {
            throw new ArgumentOutOfRangeException(ParameterName, "Toutes les composantes du vecteur doivent être finies."
            );
        }
    }
}