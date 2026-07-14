using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Tests.Physics;

public sealed class CoriolisAccelerationModelTests
{
    private const double Tolerance =
        1.0e-12;

    [Fact]
    public void Constructor_WithLatitude_UsesEarthAngularVelocity()
    {
        CoriolisAccelerationModel Model =
            new(
                LatitudeDegrees: 45.0);

        Assert.Equal(
            45.0,
            Model.LatitudeDegrees);

        Assert.Equal(
            CoriolisAccelerationModel
                .EarthAngularVelocity,
            Model.AngularVelocity);
    }

    [Fact]
    public void Constructor_WithCustomAngularVelocity_StoresValues()
    {
        CoriolisAccelerationModel Model =
            new(
                LatitudeDegrees: -30.0,
                AngularVelocity: 2.0);

        Assert.Equal(
            -30.0,
            Model.LatitudeDegrees);

        Assert.Equal(
            2.0,
            Model.AngularVelocity);
    }

    [Theory]
    [InlineData(-90.0001)]
    [InlineData(90.0001)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Constructor_WithInvalidLatitude_Throws(
        double LatitudeDegrees)
    {
        Assert.Throws<
            ArgumentOutOfRangeException>(
            () =>
                new CoriolisAccelerationModel(
                    LatitudeDegrees));
    }

    [Theory]
    [InlineData(-0.0001)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Constructor_WithInvalidAngularVelocity_Throws(
        double AngularVelocity)
    {
        Assert.Throws<
            ArgumentOutOfRangeException>(
            () =>
                new CoriolisAccelerationModel(
                    LatitudeDegrees: 45.0,
                    AngularVelocity:
                        AngularVelocity));
    }

    [Fact]
    public void GetAcceleration_WithZeroVelocity_ReturnsZero()
    {
        CoriolisAccelerationModel Model =
            new(
                LatitudeDegrees: 45.0);

        Vector3D Acceleration =
            GetAcceleration(
                Model,
                Vector3D.Zero);

        AssertVectorClose(
            Vector3D.Zero,
            Acceleration);
    }

    [Fact]
    public void GetAcceleration_WithZeroAngularVelocity_ReturnsZero()
    {
        CoriolisAccelerationModel Model =
            new(
                LatitudeDegrees: 45.0,
                AngularVelocity: 0.0);

        Vector3D Acceleration =
            GetAcceleration(
                Model,
                new Vector3D(
                    100.0,
                    50.0,
                    20.0));

        AssertVectorClose(
            Vector3D.Zero,
            Acceleration);
    }

    [Fact]
    public void GetAcceleration_AtNorthPoleWithNorthwardVelocity_DeflectsEast()
    {
        CoriolisAccelerationModel Model =
            new(
                LatitudeDegrees: 90.0,
                AngularVelocity: 2.0);

        Vector3D Acceleration =
            GetAcceleration(
                Model,
                new Vector3D(
                    0.0,
                    3.0,
                    0.0));

        AssertClose(
            12.0,
            Acceleration.X);

        AssertClose(
            0.0,
            Acceleration.Y);

        AssertClose(
            0.0,
            Acceleration.Z);
    }

    [Fact]
    public void GetAcceleration_AtNorthPoleWithEastwardVelocity_DeflectsSouth()
    {
        CoriolisAccelerationModel Model =
            new(
                LatitudeDegrees: 90.0,
                AngularVelocity: 2.0);

        Vector3D Acceleration =
            GetAcceleration(
                Model,
                new Vector3D(
                    3.0,
                    0.0,
                    0.0));

        AssertClose(
            0.0,
            Acceleration.X);

        AssertClose(
            -12.0,
            Acceleration.Y);

        AssertClose(
            0.0,
            Acceleration.Z);
    }

    [Fact]
    public void GetAcceleration_AtEquatorWithEastwardVelocity_DeflectsUpward()
    {
        CoriolisAccelerationModel Model =
            new(
                LatitudeDegrees: 0.0,
                AngularVelocity: 2.0);

        Vector3D Acceleration =
            GetAcceleration(
                Model,
                new Vector3D(
                    3.0,
                    0.0,
                    0.0));

        AssertClose(
            0.0,
            Acceleration.X);

        AssertClose(
            0.0,
            Acceleration.Y);

        AssertClose(
            12.0,
            Acceleration.Z);
    }

    [Fact]
    public void GetAcceleration_AtEquatorWithUpwardVelocity_DeflectsWest()
    {
        CoriolisAccelerationModel Model =
            new(
                LatitudeDegrees: 0.0,
                AngularVelocity: 2.0);

        Vector3D Acceleration =
            GetAcceleration(
                Model,
                new Vector3D(
                    0.0,
                    0.0,
                    3.0));

        AssertClose(
            -12.0,
            Acceleration.X);

        AssertClose(
            0.0,
            Acceleration.Y);

        AssertClose(
            0.0,
            Acceleration.Z);
    }

    [Fact]
    public void GetAcceleration_AtSouthPoleWithEastwardVelocity_DeflectsNorth()
    {
        CoriolisAccelerationModel Model =
            new(
                LatitudeDegrees: -90.0,
                AngularVelocity: 2.0);

        Vector3D Acceleration =
            GetAcceleration(
                Model,
                new Vector3D(
                    3.0,
                    0.0,
                    0.0));

        AssertClose(
            0.0,
            Acceleration.X);

        AssertClose(
            12.0,
            Acceleration.Y);

        AssertClose(
            0.0,
            Acceleration.Z);
    }

    [Fact]
    public void GetAcceleration_WithGeneralVelocity_MatchesLocalFormula()
    {
        CoriolisAccelerationModel Model =
            new(
                LatitudeDegrees: 30.0,
                AngularVelocity: 2.0);

        Vector3D Acceleration =
            GetAcceleration(
                Model,
                new Vector3D(
                    3.0,
                    4.0,
                    5.0));

        double ExpectedEastAcceleration =
            8.0
            - 10.0 * Math.Sqrt(3.0);

        double ExpectedNorthAcceleration =
            -6.0;

        double ExpectedVerticalAcceleration =
            6.0 * Math.Sqrt(3.0);

        AssertClose(
            ExpectedEastAcceleration,
            Acceleration.X);

        AssertClose(
            ExpectedNorthAcceleration,
            Acceleration.Y);

        AssertClose(
            ExpectedVerticalAcceleration,
            Acceleration.Z);
    }

    private static Vector3D GetAcceleration(
        CoriolisAccelerationModel Model,
        Vector3D Velocity)
    {
        SimulationState State =
            new(
                Time: 0.0,
                Position: Vector3D.Zero,
                Velocity: Velocity);

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
                    CrossSectionalArea: 0.01),
            Environment:
                new EnvironmentParameters(
                    GravityAcceleration: 9.80665,
                    AtmosphereModel:
                        new ConstantAtmosphereModel(
                            AirDensity: 1.225),
                    WindModel:
                        new ConstantWindModel(
                            Vector3D.Zero)),
            Settings:
                new SimulationSettings(
                    TimeStep: 0.01,
                    MaximumSimulationTime: 60.0,
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
        Assert.InRange(
            Math.Abs(Expected - Actual),
            0.0,
            Tolerance);
    }
}