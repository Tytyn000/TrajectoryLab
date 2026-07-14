using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;

namespace TrajectoryLab.Core.Physics;

// Calcule la gravitÃ© d'un astre sphÃ©rique dont la masse
// est supposÃ©e uniformÃ©ment rÃ©partie dans son volume.
public sealed class UniformSphereGravityModel
    : IAccelerationModel
{
    public const double GravitationalConstant =
        6.67430e-11;

    private const double SphereVolumeFactor =
        4.0 / 3.0 * Math.PI;

    public double BodyRadius { get; }

    public double BodyDensity { get; }

    public double BodyMass { get; }

    public double SurfaceGravityAcceleration { get; }

    public UniformSphereGravityModel(
        double BodyRadius,
        double BodyDensity)
    {
        ValidateBodyRadius(
            BodyRadius);

        ValidateBodyDensity(
            BodyDensity);

        this.BodyRadius =
            BodyRadius;

        this.BodyDensity =
            BodyDensity;

        BodyMass =
            SphereVolumeFactor
            * BodyDensity
            * BodyRadius
            * BodyRadius
            * BodyRadius;

        if (!double.IsFinite(BodyMass)
            || BodyMass <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(BodyDensity),
                BodyDensity,
                "La masse calculÃ©e de l'astre doit Ãªtre finie et strictement positive.");
        }

        SurfaceGravityAcceleration =
            SphereVolumeFactor
            * GravitationalConstant
            * BodyDensity
            * BodyRadius;

        if (!double.IsFinite(
                SurfaceGravityAcceleration)
            || SurfaceGravityAcceleration <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(BodyDensity),
                BodyDensity,
                "La gravitÃ© de surface calculÃ©e doit Ãªtre finie et strictement positive.");
        }
    }

    public static UniformSphereGravityModel
        FromSurfaceGravity(
            double BodyRadius,
            double SurfaceGravityAcceleration)
    {
        ValidateBodyRadius(
            BodyRadius);

        if (!double.IsFinite(
                SurfaceGravityAcceleration)
            || SurfaceGravityAcceleration <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(SurfaceGravityAcceleration),
                SurfaceGravityAcceleration,
                "La gravitÃ© de surface doit Ãªtre finie et strictement positive.");
        }

        // g(R) = 4/3 Ã— Ï€ Ã— G Ã— Ï Ã— R
        // donc Ï = 3g(R) / (4Ï€GR).
        double BodyDensity =
            SurfaceGravityAcceleration
            / (
                SphereVolumeFactor
                * GravitationalConstant
                * BodyRadius
            );

        if (!double.IsFinite(BodyDensity)
            || BodyDensity <= 0.0)
        {
            throw new InvalidOperationException(
                "La densitÃ© uniforme calculÃ©e doit Ãªtre finie et strictement positive.");
        }

        return new UniformSphereGravityModel(
            BodyRadius,
            BodyDensity);
    }


    public static UniformSphereGravityModel
        FromMass(
            double BodyRadius,
            double BodyMass)
    {
        ValidateBodyRadius(
            BodyRadius);

        if (!double.IsFinite(BodyMass)
            || BodyMass <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(BodyMass),
                BodyMass,
                "La masse de l'astre doit Ãªtre finie et strictement positive.");
        }

        double BodyVolume =
            SphereVolumeFactor
            * BodyRadius
            * BodyRadius
            * BodyRadius;

        double BodyDensity =
            BodyMass
            / BodyVolume;

        if (!double.IsFinite(BodyDensity)
            || BodyDensity <= 0.0)
        {
            throw new InvalidOperationException(
                "La densitÃ© uniforme calculÃ©e doit Ãªtre finie et strictement positive.");
        }

        return new UniformSphereGravityModel(
            BodyRadius,
            BodyDensity);
    }

    public Vector3D GetAcceleration(
        SimulationState State,
        SimulationParameters Parameters)
    {
        ArgumentNullException.ThrowIfNull(
            Parameters);

        double Altitude =
            State.Position.Z;

        if (!double.IsFinite(Altitude))
        {
            throw new ArgumentOutOfRangeException(
                nameof(State),
                "L'altitude doit Ãªtre finie.");
        }

        double RadialDistance =
            BodyRadius + Altitude;

        if (!double.IsFinite(RadialDistance)
            || RadialDistance < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(State),
                "L'altitude ne peut pas placer le projectile au-delÃ  du centre de l'astre.");
        }

        double GravityAcceleration;

        if (RadialDistance <= BodyRadius)
        {
            // Ã€ l'intÃ©rieur d'une sphÃ¨re uniforme :
            // g(r) = 4/3 Ã— Ï€ Ã— G Ã— Ï Ã— r.
            GravityAcceleration =
                SphereVolumeFactor
                * GravitationalConstant
                * BodyDensity
                * RadialDistance;
        }
        else
        {
            // Ã€ l'extÃ©rieur de la sphÃ¨re :
            // g(r) = G Ã— M / rÂ².
            GravityAcceleration =
                GravitationalConstant
                * BodyMass
                / (
                    RadialDistance
                    * RadialDistance
                );
        }

        if (!double.IsFinite(GravityAcceleration)
            || GravityAcceleration < 0.0)
        {
            throw new InvalidOperationException(
                "L'accÃ©lÃ©ration gravitationnelle calculÃ©e doit Ãªtre finie et positive ou nulle.");
        }

        return new Vector3D(
            X: 0.0,
            Y: 0.0,
            Z: -GravityAcceleration);
    }

    private static void ValidateBodyRadius(
        double BodyRadius)
    {
        if (!double.IsFinite(BodyRadius)
            || BodyRadius <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(BodyRadius),
                BodyRadius,
                "Le rayon de l'astre doit Ãªtre fini et strictement positif.");
        }
    }

    private static void ValidateBodyDensity(
        double BodyDensity)
    {
        if (!double.IsFinite(BodyDensity)
            || BodyDensity <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(BodyDensity),
                BodyDensity,
                "La densitÃ© de l'astre doit Ãªtre finie et strictement positive.");
        }
    }
}