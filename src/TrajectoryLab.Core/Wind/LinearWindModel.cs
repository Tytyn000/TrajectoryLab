using TrajectoryLab.Core;
using TrajectoryLab.Core.Mathematics;

namespace TrajectoryLab.Core.Wind;

public sealed class LinearWindModel : IWindModel
{
    public double LowerAltitude { get; }

    public Vector3D LowerWindVelocity { get; }

    public double UpperAltitude { get; }

    public Vector3D UpperWindVelocity { get; }

    public LinearWindModel(
        double LowerAltitude,
        Vector3D LowerWindVelocity,
        double UpperAltitude,
        Vector3D UpperWindVelocity)
    {
        ValidateAltitude(LowerAltitude, nameof(LowerAltitude));
        ValidateAltitude(UpperAltitude, nameof(UpperAltitude));
        ValidateVector(LowerWindVelocity, nameof(LowerWindVelocity));
        ValidateVector(UpperWindVelocity, nameof(UpperWindVelocity));

        if (UpperAltitude <= LowerAltitude)
        {
            throw new ArgumentException(
                "L'altitude haute doit être strictement supérieure à l'altitude basse.",
                nameof(UpperAltitude));
        }

        this.LowerAltitude = LowerAltitude;
        this.LowerWindVelocity = LowerWindVelocity;
        this.UpperAltitude = UpperAltitude;
        this.UpperWindVelocity = UpperWindVelocity;
    }

    public Vector3D GetWindVelocity(
        Vector3D Position,
        double Time)
    {
        ValidateVector(Position, nameof(Position));
        ValidateTime(Time);

        if (Position.Z <= LowerAltitude)
        {
            return LowerWindVelocity;
        }

        if (Position.Z >= UpperAltitude)
        {
            return UpperWindVelocity;
        }

        double InterpolationFactor =
            (Position.Z - LowerAltitude)
            / (UpperAltitude - LowerAltitude);

        return LowerWindVelocity
            + (UpperWindVelocity - LowerWindVelocity)
            * InterpolationFactor;
    }

    private static void ValidateAltitude(
        double Altitude,
        string ParameterName)
    {
        if (!double.IsFinite(Altitude))
        {
            throw new ArgumentOutOfRangeException(
                ParameterName,
                Altitude,
                "L'altitude doit être un nombre fini.");
        }
    }

    private static void ValidateVector(
        Vector3D Vector,
        string ParameterName)
    {
        if (!double.IsFinite(Vector.X)
            || !double.IsFinite(Vector.Y)
            || !double.IsFinite(Vector.Z))
        {
            throw new ArgumentException(
                "Le vecteur doit contenir uniquement des composantes finies.",
                ParameterName);
        }
    }

    private static void ValidateTime(double Time)
    {
        if (!double.IsFinite(Time) || Time < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Time),
                Time,
                "Le temps doit être un nombre fini supérieur ou égal à zéro.");
        }
    }
}