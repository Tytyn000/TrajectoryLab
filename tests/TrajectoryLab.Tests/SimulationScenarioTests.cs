using TrajectoryLab.Core;
using System.Runtime.CompilerServices;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Simulation;
using TrajectoryLab.Core.Solvers;

namespace TrajectoryLab.Tests.Simulation;

public sealed class SimulationScenarioTests
{
    [Fact]
    public void Constructor_WithDefaultGasModel_StoresDependencies()
    {
        SimulationParameters Parameters =
            CreateUninitialized<SimulationParameters>();

        RungeKutta4Solver Solver = new();

        IAccelerationModel AccelerationModel =
            CreateContinuousGravityModel();

        SimulationScenario Scenario = new(
            Parameters,
            Solver,
            AccelerationModel);

        Assert.Same(
            Parameters,
            Scenario.Parameters);

        Assert.Same(
            Solver,
            Scenario.Solver);

        Assert.Same(
            AccelerationModel,
            Scenario.AccelerationModel);

        Assert.NotNull(
            Scenario.GasModel);
    }

    [Fact]
    public void Constructor_WithExplicitGasModel_StoresGasModel()
    {
        SimulationParameters Parameters =
            CreateUninitialized<SimulationParameters>();

        RungeKutta4Solver Solver = new();

        IAccelerationModel AccelerationModel =
            CreateContinuousGravityModel();

        IdealGasModel GasModel =
            CreateUninitialized<IdealGasModel>();

        SimulationScenario Scenario = new(
            Parameters,
            Solver,
            AccelerationModel,
            GasModel);

        Assert.Same(
            GasModel,
            Scenario.GasModel);
    }

    [Fact]
    public void Constructor_WithNullParameters_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new SimulationScenario(
                null!,
                new RungeKutta4Solver(),
                CreateContinuousGravityModel()));
    }

    [Fact]
    public void Constructor_WithNullSolver_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new SimulationScenario(
                CreateUninitialized<SimulationParameters>(),
                null!,
                CreateContinuousGravityModel()));
    }

    [Fact]
    public void Constructor_WithNullAccelerationModel_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new SimulationScenario(
                CreateUninitialized<SimulationParameters>(),
                new RungeKutta4Solver(),
                null!));
    }

    [Fact]
    public void Constructor_WithNullGasModel_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new SimulationScenario(
                CreateUninitialized<SimulationParameters>(),
                new RungeKutta4Solver(),
                CreateContinuousGravityModel(),
                null!));
    }

    private static UniformSphereGravityModel
        CreateContinuousGravityModel()
    {
        return UniformSphereGravityModel.FromSurfaceGravity(
            BodyRadius: 6_371_000.0,
            SurfaceGravityAcceleration: 9.80665);
    }

    private static T CreateUninitialized<T>()
        where T : class
    {
        return (T)RuntimeHelpers.GetUninitializedObject(
            typeof(T));
    }
}