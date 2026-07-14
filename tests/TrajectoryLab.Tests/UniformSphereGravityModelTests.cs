using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Tests.Physics;

public sealed class
    UniformSphereGravityModelTests
{
    private const double Tolerance =
        1.0e-12;

    [Fact]
    public void Constructor_StoresBodyProperties()
    {
        const double BodyRadius =
            1000.0;

        const double BodyDensity =
            2000.0;

        UniformSphereGravityModel Model =
            new(
                BodyRadius,
                BodyDensity);

        double ExpectedMass =
            4.0
            / 3.0
            * Math.PI
            * BodyDensity
            * BodyRadius
            * BodyRadius
            * BodyRadius;

        double ExpectedSurfaceGravity =
            4.0
            / 3.0
            * Math.PI
            * UniformSphereGravityModel
                .GravitationalConstant
            * BodyDensity
            * BodyRadius;

        Assert.Equal(
            BodyRadius,
            Model.BodyRadius);

        Assert.Equal(
            BodyDensity,
            Model.BodyDensity);

        AssertClose(
            ExpectedMass,
            Model.BodyMass);

        AssertClose(
            ExpectedSurfaceGravity,
            Model.SurfaceGravityAcceleration);
    }

    [Fact]
    public void FromSurfaceGravity_ReproducesRequestedSurfaceGravity()
    {
        UniformSphereGravityModel Model =
            UniformSphereGravityModel
                .FromSurfaceGravity(
                    BodyRadius:
                        6_371_000.0,
                    SurfaceGravityAcceleration:
                        9.80665);

        AssertClose(
            9.80665,
            Model.SurfaceGravityAcceleration);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Constructor_WithInvalidBodyRadius_Throws(
        double BodyRadius)
    {
        Assert.Throws<
            ArgumentOutOfRangeException>(
            () =>
                new UniformSphereGravityModel(
                    BodyRadius,
                    BodyDensity: 2000.0));
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Constructor_WithInvalidBodyDensity_Throws(
        double BodyDensity)
    {
        Assert.Throws<
            ArgumentOutOfRangeException>(
            () =>
                new UniformSphereGravityModel(
                    BodyRadius: 1000.0,
                    BodyDensity));
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void FromSurfaceGravity_WithInvalidSurfaceGravity_Throws(
        double SurfaceGravityAcceleration)
    {
        Assert.Throws<
            ArgumentOutOfRangeException>(
            () =>
                UniformSphereGravityModel
                    .FromSurfaceGravity(
                        BodyRadius: 1000.0,
                        SurfaceGravityAcceleration));
    }

    [Fact]
    public void GetAcceleration_AtCenter_ReturnsZero()
    {
        UniformSphereGravityModel Model =
            CreateEarthModel();

        Vector3D Acceleration =
            GetAcceleration(
                Model,
                Altitude:
                    -Model.BodyRadius);

        Assert.Equal(
            Vector3D.Zero,
            Acceleration);
    }

    [Fact]
    public void GetAcceleration_AtHalfRadius_ReturnsHalfSurfaceGravity()
    {
        UniformSphereGravityModel Model =
            CreateEarthModel();

        Vector3D Acceleration =
            GetAcceleration(
                Model,
                Altitude:
                    -Model.BodyRadius / 2.0);

        AssertClose(
            -Model.SurfaceGravityAcceleration
            / 2.0,
            Acceleration.Z);
    }

    [Fact]
    public void GetAcceleration_AtSurface_ReturnsSurfaceGravity()
    {
        UniformSphereGravityModel Model =
            CreateEarthModel();

        Vector3D Acceleration =
            GetAcceleration(
                Model,
                Altitude: 0.0);

        AssertClose(
            0.0,
            Acceleration.X);

        AssertClose(
            0.0,
            Acceleration.Y);

        AssertClose(
            -9.80665,
            Acceleration.Z);
    }

    [Fact]
    public void GetAcceleration_AtTwiceRadius_ReturnsQuarterSurfaceGravity()
    {
        UniformSphereGravityModel Model =
            CreateEarthModel();

        Vector3D Acceleration =
            GetAcceleration(
                Model,
                Altitude:
                    Model.BodyRadius);

        AssertClose(
            -Model.SurfaceGravityAcceleration
            / 4.0,
            Acceleration.Z);
    }

    [Fact]
    public void GetAcceleration_AtMinus100Kilometers_UsesInteriorFormula()
    {
        const double Altitude =
            -100_000.0;

        UniformSphereGravityModel Model =
            CreateEarthModel();

        double RadialDistance =
            Model.BodyRadius
            + Altitude;

        double ExpectedGravity =
            4.0
            / 3.0
            * Math.PI
            * UniformSphereGravityModel
                .GravitationalConstant
            * Model.BodyDensity
            * RadialDistance;

        Vector3D Acceleration =
            GetAcceleration(
                Model,
                Altitude);

        AssertClose(
            -ExpectedGravity,
            Acceleration.Z);

        Assert.True(
            Math.Abs(Acceleration.Z)
            < Model.SurfaceGravityAcceleration);
    }

    [Fact]
    public void GetAcceleration_AtPlus1000Kilometers_UsesExteriorFormula()
    {
        const double Altitude =
            1_000_000.0;

        UniformSphereGravityModel Model =
            CreateEarthModel();

        double RadialDistance =
            Model.BodyRadius
            + Altitude;

        double ExpectedGravity =
            UniformSphereGravityModel
                .GravitationalConstant
            * Model.BodyMass
            / (
                RadialDistance
                * RadialDistance
            );

        Vector3D Acceleration =
            GetAcceleration(
                Model,
                Altitude);

        AssertClose(
            -ExpectedGravity,
            Acceleration.Z);

        Assert.True(
            Math.Abs(Acceleration.Z)
            < Model.SurfaceGravityAcceleration);
    }

    [Fact]
    public void GetAcceleration_AroundSurface_IsContinuous()
    {
        const double Difference =
            1.0e-3;

        UniformSphereGravityModel Model =
            CreateEarthModel();

        double BelowSurface =
            Math.Abs(
                GetAcceleration(
                    Model,
                    Altitude:
                        -Difference)
                .Z);

        double AtSurface =
            Math.Abs(
                GetAcceleration(
                    Model,
                    Altitude: 0.0)
                .Z);

        double AboveSurface =
            Math.Abs(
                GetAcceleration(
                    Model,
                    Altitude:
                        Difference)
                .Z);

        Assert.InRange(
            Math.Abs(
                BelowSurface
                - AtSurface),
            0.0,
            1.0e-8);

        Assert.InRange(
            Math.Abs(
                AboveSurface
                - AtSurface),
            0.0,
            1.0e-8);
    }

    [Fact]
    public void GetAcceleration_WithAnotherBody_UsesItsOwnParameters()
    {
        UniformSphereGravityModel MoonModel =
            UniformSphereGravityModel
                .FromSurfaceGravity(
                    BodyRadius:
                        1_737_400.0,
                    SurfaceGravityAcceleration:
                        1.62);

        Vector3D SurfaceAcceleration =
            GetAcceleration(
                MoonModel,
                Altitude: 0.0);

        Vector3D InteriorAcceleration =
            GetAcceleration(
                MoonModel,
                Altitude:
                    -100_000.0);

        Vector3D ExteriorAcceleration =
            GetAcceleration(
                MoonModel,
                Altitude:
                    1_000_000.0);

        AssertClose(
            -1.62,
            SurfaceAcceleration.Z);

        Assert.True(
            Math.Abs(
                InteriorAcceleration.Z)
            < 1.62);

        Assert.True(
            Math.Abs(
                ExteriorAcceleration.Z)
            < 1.62);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(-1000.0001)]
    public void GetAcceleration_WithInvalidAltitude_Throws(
        double Altitude)
    {
        UniformSphereGravityModel Model =
            new(
                BodyRadius: 1000.0,
                BodyDensity: 2000.0);

        SimulationState State =
            new(
                Time: 0.0,
                Position: new Vector3D(
                    0.0,
                    0.0,
                    Altitude),
                Velocity: Vector3D.Zero);

        Assert.Throws<
            ArgumentOutOfRangeException>(
            () =>
                Model.GetAcceleration(
                    State,
                    CreateParameters()));
    }

    [Fact]
    public void GetAcceleration_WithNullParameters_Throws()
    {
        UniformSphereGravityModel Model =
            CreateEarthModel();

        SimulationState State =
            new(
                Time: 0.0,
                Position: Vector3D.Zero,
                Velocity: Vector3D.Zero);

        Assert.Throws<
            ArgumentNullException>(
            () =>
                Model.GetAcceleration(
                    State,
                    null!));
    }

    private static UniformSphereGravityModel
        CreateEarthModel()
    {
        return UniformSphereGravityModel
            .FromSurfaceGravity(
                BodyRadius:
                    6_371_000.0,
                SurfaceGravityAcceleration:
                    9.80665);
    }

    private static Vector3D GetAcceleration(
        UniformSphereGravityModel Model,
        double Altitude)
    {
        SimulationState State =
            new(
                Time: 0.0,
                Position: new Vector3D(
                    0.0,
                    0.0,
                    Altitude),
                Velocity: Vector3D.Zero);

        return Model.GetAcceleration(
            State,
            CreateParameters());
    }

    private static SimulationParameters
        CreateParameters()
    {
        return new SimulationParameters(
            InitialPosition:
                Vector3D.Zero,
            InitialVelocity:
                Vector3D.Zero,
            Projectile:
                new ProjectileParameters(
                    Mass: 1.0,
                    DragCoefficient: 0.0,
                    CrossSectionalArea:
                        0.01),
            Environment:
                new EnvironmentParameters(
                    GravityAcceleration:
                        9.80665,
                    AtmosphereModel:
                        new ConstantAtmosphereModel(
                            AirDensity: 0.0),
                    WindModel:
                        new ConstantWindModel(
                            Vector3D.Zero)),
            Settings:
                new SimulationSettings(
                    TimeStep: 0.01,
                    MaximumSimulationTime:
                        60.0,
                    GroundAltitude: 0.0));
    }

    private static void AssertClose(
        double Expected,
        double Actual)
    {
        double Scale =
            Math.Max(
                1.0,
                Math.Abs(Expected));

        Assert.InRange(
            Math.Abs(Expected - Actual),
            0.0,
            Tolerance * Scale);
    }
}