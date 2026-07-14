using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;

namespace TrajectoryLab.Core.Physics;

// Calcule l'accÃ©lÃ©ration de Coriolis dans le repÃ¨re local
// X Est, Y Nord et Z altitude.
public sealed class CoriolisAccelerationModel :
    IAccelerationModel
{
    public const double EarthAngularVelocity =
        7.2921150e-5;

    public double LatitudeDegrees { get; }

    public double AngularVelocity { get; }

    public Vector3D AngularVelocityVector { get; }

    public bool UsesLatitudeDefinition { get; }

    public CoriolisAccelerationModel(
        double LatitudeDegrees)
        : this(
            LatitudeDegrees,
            EarthAngularVelocity)
    {
    }

    public CoriolisAccelerationModel(
        double LatitudeDegrees,
        double AngularVelocity)
    {
        if (!double.IsFinite(LatitudeDegrees)
            || LatitudeDegrees < -90.0
            || LatitudeDegrees > 90.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(LatitudeDegrees),
                LatitudeDegrees,
                "La latitude doit Ãªtre finie et comprise entre -90 et 90 degrÃ©s.");
        }

        if (!double.IsFinite(AngularVelocity)
            || AngularVelocity < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(AngularVelocity),
                AngularVelocity,
                "La vitesse angulaire doit Ãªtre finie et positive ou nulle.");
        }

        this.LatitudeDegrees =
            LatitudeDegrees;

        this.AngularVelocity =
            AngularVelocity;

        UsesLatitudeDefinition =
            true;

        double LatitudeRadians =
            LatitudeDegrees
            * Math.PI
            / 180.0;

        AngularVelocityVector =
            new Vector3D(
                X: 0.0,
                Y: AngularVelocity
                    * Math.Cos(LatitudeRadians),
                Z: AngularVelocity
                    * Math.Sin(LatitudeRadians));
    }

    public CoriolisAccelerationModel(
        Vector3D AngularVelocityVector)
    {
        ValidateVector(
            AngularVelocityVector,
            nameof(AngularVelocityVector));

        this.AngularVelocityVector =
            AngularVelocityVector;

        AngularVelocity =
            AngularVelocityVector.Length();

        LatitudeDegrees =
            0.0;

        UsesLatitudeDefinition =
            false;
    }

    public Vector3D GetAcceleration(
        SimulationState State,
        SimulationParameters Parameters)
    {
        Vector3D RotationCrossVelocity =
            Vector3D.Cross(
                AngularVelocityVector,
                State.Velocity);

        return RotationCrossVelocity
            * -2.0;
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
                "Toutes les composantes du vecteur doivent Ãªtre finies.");
        }
    }
}
