using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Simulation;
using TrajectoryLab.Core.Solvers;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Tests.Physics;

public sealed class
    CoriolisTrajectoryIntegrationTests
{
    private const double Tolerance =
        1.0e-10;

    [Fact]
    public void ZeroAngularVelocity_MatchesTrajectoryWithoutCoriolis()
    {
        SimulationParameters Parameters =
            CreateParameters();

        CompositeAccelerationModel
            BaselineAccelerationModel =
                new(
                    new ConstantGravityModel());

        CompositeAccelerationModel
            ZeroCoriolisAccelerationModel =
                new(
                    new ConstantGravityModel(),
                    new CoriolisAccelerationModel(
                        LatitudeDegrees: 45.0,
                        AngularVelocity: 0.0));

        SimulationResult BaselineResult =
            Simulate(
                Parameters,
                BaselineAccelerationModel);

        SimulationResult CoriolisResult =
            Simulate(
                Parameters,
                ZeroCoriolisAccelerationModel);

        Assert.Equal(
            BaselineResult.States.Count,
            CoriolisResult.States.Count);

        AssertClose(
            BaselineResult.FlightTime,
            CoriolisResult.FlightTime);

        AssertClose(
            BaselineResult.Range,
            CoriolisResult.Range);

        AssertClose(
            BaselineResult.MaximumAltitude,
            CoriolisResult.MaximumAltitude);

        AssertClose(
            BaselineResult.ImpactSpeed,
            CoriolisResult.ImpactSpeed);

        for (int Index = 0;
             Index < BaselineResult.States.Count;
             Index++)
        {
            SimulationState BaselineState =
                BaselineResult.States[Index];

            SimulationState CoriolisState =
                CoriolisResult.States[Index];

            AssertClose(
                BaselineState.Time,
                CoriolisState.Time);

            AssertVectorClose(
                BaselineState.Position,
                CoriolisState.Position);

            AssertVectorClose(
                BaselineState.Velocity,
                CoriolisState.Velocity);
        }
    }

    [Fact]
    public void NorthernHemisphere_EastwardTrajectoryDeflectsSouth()
    {
        SimulationParameters Parameters =
            CreateParameters();

        CompositeAccelerationModel
            AccelerationModel =
                new(
                    new ConstantGravityModel(),
                    new CoriolisAccelerationModel(
                        LatitudeDegrees: 45.0));

        SimulationResult Result =
            Simulate(
                Parameters,
                AccelerationModel);

        SimulationState ImpactState =
            Result.States[^1];

        Assert.True(
            ImpactState.Position.Y < 0.0,
            "Dans l'hémisphère Nord, une trajectoire vers l'Est doit être déviée vers le Sud.");
    }

    [Fact]
    public void SouthernHemisphere_EastwardTrajectoryDeflectsNorth()
    {
        SimulationParameters Parameters =
            CreateParameters();

        CompositeAccelerationModel
            AccelerationModel =
                new(
                    new ConstantGravityModel(),
                    new CoriolisAccelerationModel(
                        LatitudeDegrees: -45.0));

        SimulationResult Result =
            Simulate(
                Parameters,
                AccelerationModel);

        SimulationState ImpactState =
            Result.States[^1];

        Assert.True(
            ImpactState.Position.Y > 0.0,
            "Dans l'hémisphère Sud, une trajectoire vers l'Est doit être déviée vers le Nord.");
    }

    private static SimulationParameters
        CreateParameters()
    {
        return new SimulationParameters(
            InitialPosition:
                Vector3D.Zero,
            InitialVelocity:
                LaunchVelocity
                    .FromSpeedAndAngles(
                        Speed: 300.0,
                        ElevationDegrees: 20.0,
                        AzimuthDegrees: 0.0),
            Projectile:
                new ProjectileParameters(
                    Mass: 10.0,
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
                    MaximumSimulationTime: 120.0,
                    GroundAltitude: 0.0));
    }

    private static SimulationResult Simulate(
        SimulationParameters Parameters,
        IAccelerationModel AccelerationModel)
    {
        return new TrajectorySimulator()
            .Simulate(
                Parameters,
                new RungeKutta4Solver(),
                AccelerationModel);
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