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
    UniformSphereGravityTrajectoryIntegrationTests
{
    [Fact]
    public void VariableGravity_IncreasesHighAltitudeTrajectoryMetrics()
    {
        const double SurfaceGravityAcceleration =
            9.80665;

        UniformSphereGravityModel
            VariableGravityModel =
                UniformSphereGravityModel
                    .FromSurfaceGravity(
                        BodyRadius:
                            6_371_000.0,
                        SurfaceGravityAcceleration:
                            SurfaceGravityAcceleration);

        SimulationParameters Parameters =
            CreateParameters(
                InitialSpeed: 1000.0,
                TimeStep: 0.02,
                SurfaceGravityAcceleration:
                    SurfaceGravityAcceleration);

        SimulationResult ConstantResult =
            Simulate(
                Parameters,
                new ConstantGravityModel());

        SimulationResult VariableResult =
            Simulate(
                Parameters,
                VariableGravityModel);

        Assert.True(
            VariableResult.FlightTime
            > ConstantResult.FlightTime,
            "La diminution de la gravité avec l'altitude doit augmenter la durée du vol.");

        Assert.True(
            VariableResult.Range
            > ConstantResult.Range,
            "La diminution de la gravité avec l'altitude doit augmenter la portée.");

        Assert.True(
            VariableResult.MaximumAltitude
            > ConstantResult.MaximumAltitude,
            "La diminution de la gravité avec l'altitude doit augmenter l'altitude maximale.");
    }

    [Fact]
    public void VacuumTrajectory_ReturningToSurface_PreservesImpactSpeed()
    {
        const double InitialSpeed =
            1000.0;

        const double SurfaceGravityAcceleration =
            9.80665;

        UniformSphereGravityModel
            GravityModel =
                UniformSphereGravityModel
                    .FromSurfaceGravity(
                        BodyRadius:
                            6_371_000.0,
                        SurfaceGravityAcceleration:
                            SurfaceGravityAcceleration);

        SimulationParameters Parameters =
            CreateParameters(
                InitialSpeed:
                    InitialSpeed,
                TimeStep: 0.01,
                SurfaceGravityAcceleration:
                    SurfaceGravityAcceleration);

        SimulationResult Result =
            Simulate(
                Parameters,
                GravityModel);

        AssertClose(
            InitialSpeed,
            Result.ImpactSpeed,
            Tolerance: 1.0e-3);
    }

    [Fact]
    public void VeryLargeBodyRadius_ApproachesConstantGravity()
    {
        const double SurfaceGravityAcceleration =
            9.80665;

        UniformSphereGravityModel
            VariableGravityModel =
                UniformSphereGravityModel
                    .FromSurfaceGravity(
                        BodyRadius:
                            1.0e15,
                        SurfaceGravityAcceleration:
                            SurfaceGravityAcceleration);

        SimulationParameters Parameters =
            CreateParameters(
                InitialSpeed: 300.0,
                TimeStep: 0.01,
                SurfaceGravityAcceleration:
                    SurfaceGravityAcceleration);

        SimulationResult ConstantResult =
            Simulate(
                Parameters,
                new ConstantGravityModel());

        SimulationResult VariableResult =
            Simulate(
                Parameters,
                VariableGravityModel);

        AssertClose(
            ConstantResult.FlightTime,
            VariableResult.FlightTime,
            Tolerance: 1.0e-6);

        AssertClose(
            ConstantResult.Range,
            VariableResult.Range,
            Tolerance: 1.0e-6);

        AssertClose(
            ConstantResult.MaximumAltitude,
            VariableResult.MaximumAltitude,
            Tolerance: 1.0e-6);

        AssertClose(
            ConstantResult.ImpactSpeed,
            VariableResult.ImpactSpeed,
            Tolerance: 1.0e-6);
    }

    private static SimulationParameters
        CreateParameters(
            double InitialSpeed,
            double TimeStep,
            double SurfaceGravityAcceleration)
    {
        return new SimulationParameters(
            InitialPosition:
                Vector3D.Zero,
            InitialVelocity:
                LaunchVelocity
                    .FromSpeedAndAngles(
                        Speed:
                            InitialSpeed,
                        ElevationDegrees:
                            45.0,
                        AzimuthDegrees:
                            0.0),
            Projectile:
                new ProjectileParameters(
                    Mass: 1.0,
                    DragCoefficient: 0.0,
                    CrossSectionalArea:
                        0.01),
            Environment:
                new EnvironmentParameters(
                    GravityAcceleration:
                        SurfaceGravityAcceleration,
                    AtmosphereModel:
                        new ConstantAtmosphereModel(
                            AirDensity: 0.0),
                    WindModel:
                        new ConstantWindModel(
                            Vector3D.Zero)),
            Settings:
                new SimulationSettings(
                    TimeStep:
                        TimeStep,
                    MaximumSimulationTime:
                        300.0,
                    GroundAltitude:
                        0.0));
    }

    private static SimulationResult Simulate(
        SimulationParameters Parameters,
        IAccelerationModel GravityModel)
    {
        return new TrajectorySimulator()
            .Simulate(
                Parameters,
                new RungeKutta4Solver(),
                GravityModel);
    }

    private static void AssertClose(
        double Expected,
        double Actual,
        double Tolerance)
    {
        Assert.InRange(
            Math.Abs(Expected - Actual),
            0.0,
            Tolerance);
    }
}