using TrajectoryLab.Core.Mathematics;

namespace TrajectoryLab.Core;

public sealed class EnvironmentParameters
{
    public double GravityAcceleration { get; }

    public double AirDensity { get; }

    public Vector3D WindVelocity { get; }

    public EnvironmentParameters(
        double GravityAcceleration,
        double AirDensity,
        Vector3D WindVelocity)
    {
        // La valeur représente la norme positive de l'accélération gravitationnelle.
        if (!double.IsFinite(GravityAcceleration) || GravityAcceleration <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(GravityAcceleration), "L'accélération gravitationnelle doit être finie et strictement positive.");
        }

        // Une densité nulle représente une simulation dans le vide.
        if (!double.IsFinite(AirDensity) || AirDensity < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(AirDensity), "La densité de l'air doit être finie et positive ou nulle.");
        }

        if (!double.IsFinite(WindVelocity.X) || 
            !double.IsFinite(WindVelocity.Y) ||
            !double.IsFinite(WindVelocity.Z))
        {
            throw new ArgumentOutOfRangeException(nameof(WindVelocity),"Les composantes de la vitesse du vent doivent être finies.");
        }

        this.GravityAcceleration = GravityAcceleration;
        this.AirDensity = AirDensity;
        this.WindVelocity = WindVelocity;
    }
}