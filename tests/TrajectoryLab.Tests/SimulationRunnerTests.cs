using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Simulation;
using TrajectoryLab.Core.Solvers;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Tests.Simulation;

public sealed class SimulationRunnerTests
{
    [Fact]
    public void Run_WithNullScenario_ThrowsArgumentNullException()
    {
        SimulationRunner Runner = new();

        Assert.Throws<ArgumentNullException>(
            () => Runner.Run(null!));
    }

    [Fact]
    public void Run_WithContinuousGravity_ReturnsCompleteReport()
    {
        SimulationScenario Scenario =
            CreateScenario();

        SimulationRunner Runner = new();

        SimulationReport Report =
            Runner.Run(Scenario);

        Assert.NotNull(
            Report.Result);

        Assert.True(
            Report.FlightConditions.Count > 2);

        Assert.True(
            double.IsFinite(
                Report.AerodynamicSummary.InitialMachNumber));

        Assert.True(
            double.IsFinite(
                Report.AerodynamicSummary.MaximumMachNumber));

        Assert.True(
            double.IsFinite(
                Report.AerodynamicSummary.ImpactMachNumber));

        Assert.Equal(
            Report.FlightConditions[0].MachNumber,
            Report.AerodynamicSummary.InitialMachNumber,
            12);
    }

    [Fact]
    public void Run_WithPreCanceledToken_ThrowsOperationCanceledException()
    {
        SimulationScenario Scenario =
            CreateScenario();

        using CancellationTokenSource CancellationSource = new();

        CancellationSource.Cancel();

        SimulationRunner Runner = new();

        Assert.Throws<OperationCanceledException>(
            () => Runner.Run(
                Scenario,
                CancellationSource.Token));
    }

    [Fact]
    public void Run_ForwardsProgressToSimulator()
    {
        SimulationScenario Scenario =
            CreateScenario();

        RecordingProgress Progress = new();

        SimulationRunner Runner = new();

        SimulationReport Report = Runner.Run(
            Scenario,
            CancellationToken.None,
            Progress);

        Assert.NotNull(Report);
        Assert.NotEmpty(Progress.Values);

        Assert.Equal(
            0.0,
            Progress.Values[0],
            12);

        Assert.Equal(
            1.0,
            Progress.Values[^1],
            12);
    }

    private static SimulationScenario CreateScenario()
    {
        ProjectileParameters Projectile = new(
            Mass: 1.0,
            CrossSectionalArea: 0.01,
            DragCoefficient: 0.47);

        EnvironmentParameters Environment = new(
            GravityAcceleration: 9.80665,
            AtmosphereModel:
                new ConstantAtmosphereModel(
                    AirDensity: 0.0),
            WindModel:
                new ConstantWindModel(
                    Vector3D.Zero));

        SimulationSettings Settings = new(
            TimeStep: 0.01,
            MaximumSimulationTime: 30.0);

        SimulationParameters Parameters = new(
            InitialPosition:
                Vector3D.Zero,
            InitialVelocity:
                new Vector3D(
                    35.35533905932738,
                    0.0,
                    35.35533905932738),
            Projectile:
                Projectile,
            Environment:
                Environment,
            Settings:
                Settings);

        UniformSphereGravityModel GravityModel =
            UniformSphereGravityModel.FromSurfaceGravity(
                BodyRadius: 6_371_000.0,
                SurfaceGravityAcceleration: 9.80665);

        return new SimulationScenario(
            Parameters,
            new RungeKutta4Solver(),
            GravityModel);
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
}
