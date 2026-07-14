using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;

namespace TrajectoryLab.Core.Physics;

// Calcule l'accélération aérodynamique produite
// par la rotation du projectile dans le fluide.
public sealed class MagnusAccelerationModel
    : IAccelerationModel
{
    public Vector3D AngularVelocity { get; }

    public double ProjectileRadius { get; }

    public double MagnusCoefficient { get; }

    public MagnusAccelerationModel(
        Vector3D AngularVelocity,
        double ProjectileRadius,
        double MagnusCoefficient)
    {
        ValidateVector(
            AngularVelocity,
            nameof(AngularVelocity));

        if (!double.IsFinite(ProjectileRadius)
            || ProjectileRadius <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(ProjectileRadius),
                ProjectileRadius,
                "Le rayon du projectile doit être fini et strictement positif.");
        }

        if (!double.IsFinite(MagnusCoefficient)
            || MagnusCoefficient < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MagnusCoefficient),
                MagnusCoefficient,
                "Le coefficient de Magnus doit être fini et positif ou nul.");
        }

        this.AngularVelocity =
            AngularVelocity;

        this.ProjectileRadius =
            ProjectileRadius;

        this.MagnusCoefficient =
            MagnusCoefficient;
    }

    public Vector3D GetAcceleration(
        SimulationState State,
        SimulationParameters Parameters)
    {
        ArgumentNullException.ThrowIfNull(
            Parameters);

        if (MagnusCoefficient == 0.0
            || AngularVelocity.LengthSquared() == 0.0
            || Parameters.Projectile
                .CrossSectionalArea == 0.0)
        {
            return Vector3D.Zero;
        }

        Vector3D WindVelocity =
            Parameters.Environment.WindModel
                .GetWindVelocity(
                    State.Position,
                    State.Time);

        ValidateModelVector(
            WindVelocity,
            "La vitesse du vent");

        Vector3D RelativeVelocity =
            State.Velocity
            - WindVelocity;

        if (RelativeVelocity.LengthSquared() == 0.0)
        {
            return Vector3D.Zero;
        }

        double AirDensity =
            Parameters.Environment.AtmosphereModel
                .GetAirDensity(
                    State.Position.Z);

        if (!double.IsFinite(AirDensity)
            || AirDensity < 0.0)
        {
            throw new InvalidOperationException(
                "Le modèle atmosphérique doit retourner une densité finie et positive ou nulle.");
        }

        if (AirDensity == 0.0)
        {
            return Vector3D.Zero;
        }

        Vector3D RotationCrossVelocity =
            Vector3D.Cross(
                AngularVelocity,
                RelativeVelocity);

        if (RotationCrossVelocity.LengthSquared()
            == 0.0)
        {
            return Vector3D.Zero;
        }

        double AccelerationFactor =
            0.5
            * AirDensity
            * MagnusCoefficient
            * Parameters.Projectile
                .CrossSectionalArea
            * ProjectileRadius
            / Parameters.Projectile.Mass;

        if (!double.IsFinite(
                AccelerationFactor))
        {
            throw new InvalidOperationException(
                "Le facteur d'accélération de Magnus doit être fini.");
        }

        Vector3D Acceleration =
            RotationCrossVelocity
            * AccelerationFactor;

        ValidateModelVector(
            Acceleration,
            "L'accélération de Magnus");

        return Acceleration;
    }

    private static void ValidateVector(
        Vector3D Vector,
        string ParameterName)
    {
        if (!double.IsFinite(Vector.X)
            || !double.IsFinite(Vector.Y)
            || !double.IsFinite(Vector.Z))
        {
            throw new ArgumentOutOfRangeException(
                ParameterName,
                "Toutes les composantes du vecteur doivent être finies.");
        }
    }

    private static void ValidateModelVector(
        Vector3D Vector,
        string Description)
    {
        if (!double.IsFinite(Vector.X)
            || !double.IsFinite(Vector.Y)
            || !double.IsFinite(Vector.Z))
        {
            throw new InvalidOperationException(
                $"{Description} doit avoir des composantes finies.");
        }
    }
}