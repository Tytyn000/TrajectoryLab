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

public sealed class TrajectorySimulatorTests
{
    [Fact]
    public void DragReducesProjectileRange()
    {
        SimulationParameters Parameters =
            CreateParameters(
                AirDensity: 1.225);

        TrajectorySimulator Simulator = new();
        RungeKutta4Solver Solver = new();

        SimulationResult VacuumResult = Simulator.Simulate(
            Parameters,
            Solver,
            new CompositeAccelerationModel(
                new ConstantGravityModel()));

        SimulationResult DragResult = Simulator.Simulate(
            Parameters,
            Solver,
            new CompositeAccelerationModel(
                new ConstantGravityModel(),
                new DragAccelerationModel()));

        Assert.True(
            DragResult.Range < VacuumResult.Range);
    }

    [Fact]
    public void Simulate_WithPreCanceledToken_ThrowsOperationCanceledException()
    {
        SimulationParameters Parameters =
            CreateParameters(
                AirDensity: 0.0);

        UniformSphereGravityModel GravityModel =
            CreateContinuousGravityModel();

        using CancellationTokenSource CancellationSource = new();

        CancellationSource.Cancel();

        TrajectorySimulator Simulator = new();

        Assert.Throws<OperationCanceledException>(
            () => Simulator.Simulate(
                Parameters,
                new RungeKutta4Solver(),
                GravityModel,
                CancellationSource.Token));
    }

    [Fact]
    public void Simulate_ReportsMonotonicProgressFromZeroToOne()
    {
        SimulationParameters Parameters =
            CreateParameters(
                AirDensity: 0.0);

        UniformSphereGravityModel GravityModel =
            CreateContinuousGravityModel();

        RecordingProgress Progress = new();

        TrajectorySimulator Simulator = new();

        SimulationResult Result = Simulator.Simulate(
            Parameters,
            new RungeKutta4Solver(),
            GravityModel,
            CancellationToken.None,
            Progress);

        Assert.NotNull(Result);
        Assert.NotEmpty(Progress.Values);

        Assert.Equal(
            0.0,
            Progress.Values[0],
            12);

        Assert.Equal(
            1.0,
            Progress.Values[^1],
            12);

        for (
            int Index = 0;
            Index < Progress.Values.Count;
            Index++)
        {
            Assert.InRange(
                Progress.Values[Index],
                0.0,
                1.0);

            if (Index > 0)
            {
                Assert.True(
                    Progress.Values[Index] >=
                    Progress.Values[Index - 1]);
            }
        }
    }

    [Fact]
    public void Simulate_WhenProgressCancelsToken_ThrowsOperationCanceledException()
    {
        SimulationParameters Parameters =
            CreateParameters(
                AirDensity: 0.0);

        UniformSphereGravityModel GravityModel =
            CreateContinuousGravityModel();

        using CancellationTokenSource CancellationSource = new();

        CancelingProgress Progress = new(
            CancellationSource,
            CancellationThreshold: 0.01);

        TrajectorySimulator Simulator = new();

        Assert.Throws<OperationCanceledException>(
            () => Simulator.Simulate(
                Parameters,
                new RungeKutta4Solver(),
                GravityModel,
                CancellationSource.Token,
                Progress));
    }

    private static SimulationParameters CreateParameters(
        double AirDensity)
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
                    DragCoefficient: 0.47,
                    CrossSectionalArea: 0.01),
            Environment:
                new EnvironmentParameters(
                    GravityAcceleration: 9.80665,
                    AtmosphereModel:
                        new ConstantAtmosphereModel(
                            AirDensity),
                    WindModel:
                        new ConstantWindModel(
                            Vector3D.Zero)),
            Settings:
                new SimulationSettings(
                    TimeStep: 0.01,
                    MaximumSimulationTime: 30.0));
    }

    private static UniformSphereGravityModel
        CreateContinuousGravityModel()
    {
        return UniformSphereGravityModel.FromSurfaceGravity(
            BodyRadius: 6_371_000.0,
            SurfaceGravityAcceleration: 9.80665);
    }

    private sealed class RecordingProgress :
        IProgress<double>
    {
        public List<double> Values { get; } = [];

        public void Report(
            double Value)
        {
            Values.Add(Value);
        }
    }

    private sealed class CancelingProgress :
        IProgress<double>
    {
        private readonly CancellationTokenSource
            CancellationSource;

        private readonly double CancellationThreshold;

        public CancelingProgress(
            CancellationTokenSource CancellationSource,
            double CancellationThreshold)
        {
            this.CancellationSource =
                CancellationSource;

            this.CancellationThreshold =
                CancellationThreshold;
        }

        public void Report(
            double Value)
        {
            if (Value >= CancellationThreshold)
            {
                CancellationSource.Cancel();
            }
        }
    }
}
