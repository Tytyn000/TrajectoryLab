using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Simulation;
using TrajectoryLab.Core.Solvers;
using TrajectoryLab.Core.Wind;
using Xunit;

namespace TrajectoryLab.Tests;

public sealed class
    RungeKutta45TrajectoryIntegrationTests
{
    [Fact]
    public void AdaptiveSolverMatchesFineRk4WithContinuousGravity()
    {
        UniformSphereGravityModel GravityModel =
            UniformSphereGravityModel.FromSurfaceGravity(
                BodyRadius: 6_371_000.0,
                SurfaceGravityAcceleration: 9.80665);

        SimulationResult ReferenceResult =
            new TrajectorySimulator().Simulate(
                CreateParameters(
                    TimeStep: 0.001),
                new RungeKutta4Solver(),
                GravityModel);

        SimulationResult AdaptiveResult =
            new TrajectorySimulator().Simulate(
                CreateParameters(
                    TimeStep: 0.05),
                new RungeKutta45Solver(
                    AbsoluteTolerance: 1.0e-10,
                    RelativeTolerance: 1.0e-10,
                    MinimumTimeStep: 1.0e-6,
                    MaximumTimeStep: 0.05),
                GravityModel);

        Assert.InRange(
            Math.Abs(
                AdaptiveResult.FlightTime -
                ReferenceResult.FlightTime),
            0.0,
            0.01);

        Assert.InRange(
            Math.Abs(
                AdaptiveResult.Range -
                ReferenceResult.Range),
            0.0,
            0.1);

        Assert.InRange(
            Math.Abs(
                AdaptiveResult.MaximumAltitude -
                ReferenceResult.MaximumAltitude),
            0.0,
            0.05);

        Assert.InRange(
            Math.Abs(
                AdaptiveResult.ImpactSpeed -
                ReferenceResult.ImpactSpeed),
            0.0,
            0.01);

        Assert.True(
            AdaptiveResult.States.Count <
            ReferenceResult.States.Count);
    }

    private static SimulationParameters
        CreateParameters(
            double TimeStep)
    {
        return new SimulationParameters(
            InitialPosition:
                Vector3D.Zero,
            InitialVelocity:
                LaunchVelocity.FromSpeedAndAngles(
                    Speed: 50.0,
                    ElevationDegrees: 45.0),
            Projectile:
                new ProjectileParameters(
                    Mass: 1.0,
                    DragCoefficient: 0.0,
                    CrossSectionalArea: 0.0),
            Environment:
                new EnvironmentParameters(
                    GravityAcceleration: 9.80665,
                    AtmosphereModel:
                        new ConstantAtmosphereModel(
                            AirDensity: 0.0),
                    WindModel:
                        new ConstantWindModel(
                            Vector3D.Zero)),
            Settings:
                new SimulationSettings(
                    TimeStep: TimeStep,
                    MaximumSimulationTime: 30.0));
    }
}
