using TrajectoryLab.Core.Mathematics;

namespace TrajectoryLab.Core.Wind;

// Représente un vent constant dans l'espace et dans le temps.
public sealed class ConstantWindModel : IWindModel
{
    public Vector3D WindVelocity { get; }

    public ConstantWindModel(
        Vector3D WindVelocity)
    {
        ValidateVector(
            WindVelocity,
            nameof(WindVelocity)
        );

        this.WindVelocity =
            WindVelocity;
    }

    public Vector3D GetWindVelocity(
        Vector3D Position,
        double Time)
    {
        ValidateVector(
            Position,
            nameof(Position)
        );

        if (!double.IsFinite(Time))
        {
            throw new ArgumentOutOfRangeException(
                nameof(Time),
                "Le temps doit être fini."
            );
        }

        return WindVelocity;
    }

    private static void ValidateVector(
        Vector3D Vector,
        string ParameterName)
    {
        if (!double.IsFinite(Vector.X) ||
            !double.IsFinite(Vector.Y) ||
            !double.IsFinite(Vector.Z))
        {
            throw new ArgumentOutOfRangeException(
                ParameterName,
                "Toutes les composantes du vecteur doivent être finies."
            );
        }
    }
}