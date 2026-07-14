using TrajectoryLab.Core;
using TrajectoryLab.Core.Mathematics;

namespace TrajectoryLab.Core.Wind;

public sealed class WindLayer
{
    public double Altitude { get; }

    public Vector3D WindVelocity { get; }

    public WindLayer(
        double Altitude,
        Vector3D WindVelocity)
    {
        ValidateAltitude(Altitude);
        ValidateVector(WindVelocity);

        this.Altitude = Altitude;
        this.WindVelocity = WindVelocity;
    }

    private static void ValidateAltitude(double Altitude)
    {
        if (!double.IsFinite(Altitude))
        {
            throw new ArgumentOutOfRangeException(
                nameof(Altitude),
                Altitude,
                "L'altitude de la couche doit être un nombre fini.");
        }
    }

    private static void ValidateVector(Vector3D WindVelocity)
    {
        if (!double.IsFinite(WindVelocity.X)
            || !double.IsFinite(WindVelocity.Y)
            || !double.IsFinite(WindVelocity.Z))
        {
            throw new ArgumentException(
                "La vitesse du vent doit contenir uniquement des composantes finies.",
                nameof(WindVelocity));
        }
    }
}