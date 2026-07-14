using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Tests.Physics;

public sealed class MagnusAccelerationModelTests
{
    private const double Tolerance =
        1.0e-12;

    public static TheoryData<Vector3D>
        InvalidAngularVelocities =>
        new()
        {
            new Vector3D(
                double.NaN,
                0.0,
                0.0),

            new Vector3D(
                double.PositiveInfinity,
                0.0,
                0.0),

            new Vector3D(
                0.0,
                double.NaN,
                0.0),

            new Vector3D(
                0.0,
                double.NegativeInfinity,
                0.0),

            new Vector3D(
                0.0,
                0.0,
                double.NaN),

            new Vector3D(
                0.0,
                0.0,
                double.PositiveInfinity)
        };

    [Fact]
    public void Constructor_StoresValues()
    {
        Vector3D AngularVelocity =
            new(
                1.0,
                2.0,
                3.0);

        MagnusAccelerationModel Model =
            new(
                AngularVelocity,
                ProjectileRadius: 0.05,
                MagnusCoefficient: 0.4);

        Assert.Equal(
            AngularVelocity,
            Model.AngularVelocity);

        Assert.Equal(
            0.05,
            Model.ProjectileRadius);

        Assert.Equal(
            0.4,
            Model.MagnusCoefficient);
    }

    [Theory]
    [MemberData(
        nameof(InvalidAngularVelocities))]
    public void Constructor_WithInvalidAngularVelocity_Throws(
        Vector3D AngularVelocity)
    {
        Assert.Throws<
            ArgumentOutOfRangeException>(
            () =>
                new MagnusAccelerationModel(
                    AngularVelocity,
                    ProjectileRadius: 0.05,
                    MagnusCoefficient: 0.4));
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Constructor_WithInvalidProjectileRadius_Throws(
        double ProjectileRadius)
    {
        Assert.Throws<
            ArgumentOutOfRangeException>(
            () =>
                new MagnusAccelerationModel(
                    AngularVelocity:
                        new Vector3D(
                            0.0,
                            0.0,
                            10.0),
                    ProjectileRadius:
                        ProjectileRadius,
                    MagnusCoefficient:
                        0.4));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Constructor_WithInvalidMagnusCoefficient_Throws(
        double MagnusCoefficient)
    {
        Assert.Throws<
            ArgumentOutOfRangeException>(
            () =>
                new MagnusAccelerationModel(
                    AngularVelocity:
                        new Vector3D(
                            0.0,
                            0.0,
                            10.0),
                    ProjectileRadius:
                        0.05,
                    MagnusCoefficient:
                        MagnusCoefficient));
    }

    [Fact]
    public void GetAcceleration_WithZeroAngularVelocity_ReturnsZero()
    {
        MagnusAccelerationModel Model =
            new(
                AngularVelocity:
                    Vector3D.Zero,
                ProjectileRadius: 0.05,
                MagnusCoefficient: 0.5);

        Vector3D Acceleration =
            Model.GetAcceleration(
                CreateState(
                    Velocity:
                        new Vector3D(
                            20.0,
                            0.0,
                            0.0)),
                CreateParameters());

        Assert.Equal(
            Vector3D.Zero,
            Acceleration);
    }

    [Fact]
    public void GetAcceleration_WithZeroRelativeVelocity_ReturnsZero()
    {
        Vector3D FluidVelocity =
            new(
                20.0,
                0.0,
                0.0);

        MagnusAccelerationModel Model =
            CreateModel();

        Vector3D Acceleration =
            Model.GetAcceleration(
                CreateState(
                    Velocity:
                        FluidVelocity),
                CreateParameters(
                    WindVelocity:
                        FluidVelocity));

        Assert.Equal(
            Vector3D.Zero,
            Acceleration);
    }

    [Fact]
    public void GetAcceleration_WithZeroAirDensity_ReturnsZero()
    {
        MagnusAccelerationModel Model =
            CreateModel();

        Vector3D Acceleration =
            Model.GetAcceleration(
                CreateState(
                    Velocity:
                        new Vector3D(
                            20.0,
                            0.0,
                            0.0)),
                CreateParameters(
                    AirDensity: 0.0));

        Assert.Equal(
            Vector3D.Zero,
            Acceleration);
    }

    [Fact]
    public void GetAcceleration_WithZeroCrossSectionalArea_ReturnsZero()
    {
        MagnusAccelerationModel Model =
            CreateModel();

        Vector3D Acceleration =
            Model.GetAcceleration(
                CreateState(
                    Velocity:
                        new Vector3D(
                            20.0,
                            0.0,
                            0.0)),
                CreateParameters(
                    CrossSectionalArea:
                        0.0));

        Assert.Equal(
            Vector3D.Zero,
            Acceleration);
    }

    [Fact]
    public void GetAcceleration_WithZeroMagnusCoefficient_ReturnsZero()
    {
        MagnusAccelerationModel Model =
            new(
                AngularVelocity:
                    new Vector3D(
                        0.0,
                        0.0,
                        10.0),
                ProjectileRadius: 0.2,
                MagnusCoefficient: 0.0);

        Vector3D Acceleration =
            Model.GetAcceleration(
                CreateState(
                    Velocity:
                        new Vector3D(
                            20.0,
                            0.0,
                            0.0)),
                CreateParameters());

        Assert.Equal(
            Vector3D.Zero,
            Acceleration);
    }

    [Fact]
    public void GetAcceleration_MatchesMagnusFormula()
    {
        MagnusAccelerationModel Model =
            new(
                AngularVelocity:
                    new Vector3D(
                        0.0,
                        0.0,
                        10.0),
                ProjectileRadius: 0.2,
                MagnusCoefficient: 0.5);

        SimulationParameters Parameters =
            CreateParameters(
                AirDensity: 1.2,
                CrossSectionalArea: 0.1,
                Mass: 2.0);

        SimulationState State =
            CreateState(
                Velocity:
                    new Vector3D(
                        20.0,
                        0.0,
                        0.0));

        Vector3D Acceleration =
            Model.GetAcceleration(
                State,
                Parameters);

        // ω × v = (0, 200, 0).
        // Facteur = 0,5 × 1,2 × 0,5 × 0,1 × 0,2 / 2
        //         = 0,003.
        // a = (0, 200, 0) × 0,003
        //   = (0, 0,6, 0).
        AssertClose(
            0.0,
            Acceleration.X);

        AssertClose(
            0.6,
            Acceleration.Y);

        AssertClose(
            0.0,
            Acceleration.Z);
    }

    [Fact]
    public void GetAcceleration_UsesVelocityRelativeToWind()
    {
        MagnusAccelerationModel Model =
            new(
                AngularVelocity:
                    new Vector3D(
                        0.0,
                        0.0,
                        10.0),
                ProjectileRadius: 0.2,
                MagnusCoefficient: 0.5);

        SimulationParameters Parameters =
            CreateParameters(
                AirDensity: 1.2,
                CrossSectionalArea: 0.1,
                Mass: 2.0,
                WindVelocity:
                    new Vector3D(
                        5.0,
                        0.0,
                        0.0));

        SimulationState State =
            CreateState(
                Velocity:
                    new Vector3D(
                        20.0,
                        0.0,
                        0.0));

        Vector3D Acceleration =
            Model.GetAcceleration(
                State,
                Parameters);

        // La vitesse relative vaut 15 m/s.
        // ω × v_relative = (0, 150, 0).
        // a = (0, 150, 0) × 0,003.
        AssertClose(
            0.0,
            Acceleration.X);

        AssertClose(
            0.45,
            Acceleration.Y);

        AssertClose(
            0.0,
            Acceleration.Z);
    }

    [Fact]
    public void GetAcceleration_ReversingSpinReversesAcceleration()
    {
        MagnusAccelerationModel PositiveModel =
            new(
                AngularVelocity:
                    new Vector3D(
                        0.0,
                        0.0,
                        10.0),
                ProjectileRadius: 0.2,
                MagnusCoefficient: 0.5);

        MagnusAccelerationModel NegativeModel =
            new(
                AngularVelocity:
                    new Vector3D(
                        0.0,
                        0.0,
                        -10.0),
                ProjectileRadius: 0.2,
                MagnusCoefficient: 0.5);

        SimulationState State =
            CreateState(
                Velocity:
                    new Vector3D(
                        20.0,
                        0.0,
                        0.0));

        SimulationParameters Parameters =
            CreateParameters();

        Vector3D PositiveAcceleration =
            PositiveModel.GetAcceleration(
                State,
                Parameters);

        Vector3D NegativeAcceleration =
            NegativeModel.GetAcceleration(
                State,
                Parameters);

        AssertVectorClose(
            -PositiveAcceleration,
            NegativeAcceleration);
    }

    [Fact]
    public void GetAcceleration_IsPerpendicularToSpinAndRelativeVelocity()
    {
        Vector3D AngularVelocity =
            new(
                1.0,
                2.0,
                3.0);

        Vector3D RelativeVelocity =
            new(
                4.0,
                -5.0,
                6.0);

        MagnusAccelerationModel Model =
            new(
                AngularVelocity,
                ProjectileRadius: 0.2,
                MagnusCoefficient: 0.5);

        Vector3D Acceleration =
            Model.GetAcceleration(
                CreateState(
                    Velocity:
                        RelativeVelocity),
                CreateParameters(
                    WindVelocity:
                        Vector3D.Zero));

        AssertClose(
            0.0,
            Vector3D.Dot(
                Acceleration,
                AngularVelocity));

        AssertClose(
            0.0,
            Vector3D.Dot(
                Acceleration,
                RelativeVelocity));
    }

    [Fact]
    public void GetAcceleration_WithNullParameters_Throws()
    {
        MagnusAccelerationModel Model =
            CreateModel();

        Assert.Throws<
            ArgumentNullException>(
            () =>
                Model.GetAcceleration(
                    CreateState(
                        Velocity:
                            Vector3D.Zero),
                    null!));
    }

    private static MagnusAccelerationModel
        CreateModel()
    {
        return new MagnusAccelerationModel(
            AngularVelocity:
                new Vector3D(
                    0.0,
                    0.0,
                    10.0),
            ProjectileRadius: 0.2,
            MagnusCoefficient: 0.5);
    }

    private static SimulationState CreateState(
        Vector3D Velocity)
    {
        return new SimulationState(
            Time: 0.0,
            Position: Vector3D.Zero,
            Velocity: Velocity);
    }

    private static SimulationParameters
        CreateParameters(
            double AirDensity = 1.2,
            double CrossSectionalArea = 0.1,
            double Mass = 2.0,
            Vector3D? WindVelocity = null)
    {
        return new SimulationParameters(
            InitialPosition:
                Vector3D.Zero,
            InitialVelocity:
                Vector3D.Zero,
            Projectile:
                new ProjectileParameters(
                    Mass: Mass,
                    DragCoefficient: 0.0,
                    CrossSectionalArea:
                        CrossSectionalArea),
            Environment:
                new EnvironmentParameters(
                    GravityAcceleration:
                        9.80665,
                    AtmosphereModel:
                        new ConstantAtmosphereModel(
                            AirDensity:
                                AirDensity),
                    WindModel:
                        new ConstantWindModel(
                            WindVelocity
                            ?? Vector3D.Zero)),
            Settings:
                new SimulationSettings(
                    TimeStep: 0.01,
                    MaximumSimulationTime:
                        60.0,
                    GroundAltitude: 0.0));
    }

    private static void AssertVectorClose(
        Vector3D Expected,
        Vector3D Actual)
    {
        AssertClose(
            Expected.X,
            Actual.X);

        AssertClose(
            Expected.Y,
            Actual.Y);

        AssertClose(
            Expected.Z,
            Actual.Z);
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