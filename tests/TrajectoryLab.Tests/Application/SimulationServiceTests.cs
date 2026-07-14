using TrajectoryLab.Application.Models;
using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Application.Simulation;
using TrajectoryLab.Application.Simulation.Factories;
using TrajectoryLab.Application.Validation;
using TrajectoryLab.Core.Physics;
using Xunit;

namespace TrajectoryLab.Tests.Application;

public sealed class SimulationServiceTests
{
    [Fact]
    public void Validator_WithInvalidTimeStep_Throws()
    {
        SimulationInputValidator Validator = new();

        SimulationInput Input =
            new()
            {
                Solver =
                    new SolverInput
                    {
                        TimeStep = 0.0
                    }
            };

        Assert.Throws<SimulationInputValidationException>(
            () => Validator.Validate(Input));
    }

    [Fact]
    public void ScenarioFactory_UsesContinuousSphereGravity()
    {
        ISimulationScenarioFactory Factory =
            CreateScenarioFactory();

        var Scenario =
            Factory.Create(new SimulationInput());

        Assert.IsType<UniformSphereGravityModel>(
            Scenario.AccelerationModel);
    }

    [Fact]
    public async Task SimulationService_DefaultInput_ReturnsExpectedTrajectory()
    {
        ISimulationService Service = CreateService();

        SimulationRunResult Result =
            await Service.RunAsync(
                new SimulationInput(),
                CancellationToken.None);

        Assert.True(Result.Samples.Count > 700);

        Assert.InRange(
            Result.Summary.FlightTime,
            7.20,
            7.22);

        Assert.InRange(
            Result.Summary.Range,
            254.8,
            255.1);

        Assert.InRange(
            Result.Summary.MaximumAltitude,
            63.6,
            63.9);
    }

    [Fact]
    public async Task SimulationService_NorthLaunch_UsesPositiveYAxis()
    {
        ISimulationService Service = CreateService();

        SimulationRunResult Result =
            await Service.RunAsync(
                new SimulationInput
                {
                    InitialConditions =
                        new InitialConditionsInput
                        {
                            AzimuthDegrees = 90.0
                        }
                },
                CancellationToken.None);

        TrajectorySample Impact = Result.Samples[^1];

        Assert.True(Impact.Y > 250.0);

        Assert.InRange(
            Math.Abs(Impact.X),
            0.0,
            1.0e-9);
    }

    private static ISimulationService CreateService()
    {
        ITrajectoryProjectionService Projection =
            new TrajectoryProjectionService();

        return new SimulationService(
            CreateScenarioFactory(),
            Projection);
    }

    private static ISimulationScenarioFactory CreateScenarioFactory()
    {
        IGravityModelFactory GravityModelFactory =
            new GravityModelFactory();

        IDragCoefficientModelFactory DragCoefficientModelFactory =
            new DragCoefficientModelFactory();

        IAccelerationModelFactory AccelerationModelFactory =
            new AccelerationModelFactory(
                GravityModelFactory,
                DragCoefficientModelFactory);

        return new DefaultSimulationScenarioFactory(
            new SimulationInputValidator(),
            new AtmosphereModelFactory(),
            new WindModelFactory(),
            GravityModelFactory,
            AccelerationModelFactory,
            new SolverFactory());
    }
}