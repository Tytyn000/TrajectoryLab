using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Simulation;
using TrajectoryLab.Core.Solvers;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Tests.Physics;

public sealed class FlightConditionTrajectoryIntegrationTests
{
    [Fact]
    public void CalculateAll_ForTrajectory_ReturnsOneConditionPerState()
    {
        LinearWindModel WindModel = new(
            LowerAltitude: 0.0,
            LowerWindVelocity: Vector3D.Zero,
            UpperAltitude: 100.0,
            UpperWindVelocity: new Vector3D(
                0.0,
                20.0,
                0.0));

        SimulationParameters Parameters =
            CreateParameters(WindModel);

        SimulationResult Result =
            Simulate(Parameters);

        FlightConditionCalculator Calculator = new(
            new IdealGasModel());

        IReadOnlyList<FlightCondition> Conditions =
            Calculator.CalculateAll(
                Result,
                Parameters);

        Assert.Equal(
            Result.States.Count,
            Conditions.Count);

        Assert.True(
            Conditions.Count > 1,
            "La simulation doit produire plusieurs conditions de vol.");

        Assert.Equal(
            Result.States[0].Time,
            Conditions[0].Time);

        Assert.Equal(
            Result.States[^1].Time,
            Conditions[^1].Time);
    }

    [Fact]
    public void CalculateAll_ForSubsonicTrajectory_ProducesValidConditions()
    {
        LinearWindModel WindModel = new(
            LowerAltitude: 0.0,
            LowerWindVelocity: Vector3D.Zero,
            UpperAltitude: 100.0,
            UpperWindVelocity: new Vector3D(
                0.0,
                20.0,
                0.0));

        SimulationParameters Parameters =
            CreateParameters(WindModel);

        SimulationResult Result =
            Simulate(Parameters);

        FlightConditionCalculator Calculator = new(
            new IdealGasModel());

        IReadOnlyList<FlightCondition> Conditions =
            Calculator.CalculateAll(
                Result,
                Parameters);

        foreach (FlightCondition Condition
                 in Conditions)
        {
            Assert.True(
                double.IsFinite(Condition.Temperature)
                && Condition.Temperature > 0.0);

            Assert.True(
                double.IsFinite(Condition.SpeedOfSound)
                && Condition.SpeedOfSound > 0.0);

            Assert.True(
                double.IsFinite(Condition.RelativeSpeed)
                && Condition.RelativeSpeed >= 0.0);

            Assert.InRange(
                Condition.MachNumber,
                0.0,
                1.0);
        }
    }

    [Fact]
    public void CalculateAll_WithNorthWind_ProducesExpectedInitialRelativeVelocity()
    {
        LinearWindModel WindModel = new(
            LowerAltitude: 0.0,
            LowerWindVelocity: new Vector3D(
                0.0,
                5.0,
                0.0),
            UpperAltitude: 100.0,
            UpperWindVelocity: new Vector3D(
                0.0,
                20.0,
                0.0));

        SimulationParameters Parameters =
            CreateParameters(WindModel);

        SimulationResult Result =
            Simulate(Parameters);

        FlightConditionCalculator Calculator = new(
            new IdealGasModel());

        IReadOnlyList<FlightCondition> Conditions =
            Calculator.CalculateAll(
                Result,
                Parameters);

        FlightCondition InitialCondition =
            Conditions[0];

        Assert.True(
            InitialCondition.WindVelocity.Y > 0.0);

        Assert.True(
            InitialCondition.RelativeVelocity.Y < 0.0,
            "Un vent initial vers le Nord doit produire une composante relative initiale vers le Sud.");
    }

    private static SimulationParameters
        CreateParameters(IWindModel WindModel)
    {
        ProjectileParameters Projectile = new(
            Mass: 1.0,
            DragCoefficient: 0.47,
            CrossSectionalArea: 0.01);

        EnvironmentParameters Environment = new(
            GravityAcceleration: 9.80665,
            AtmosphereModel:
                new StandardAtmosphere1976Model(),
            WindModel: WindModel);

        SimulationSettings Settings = new(
            GroundAltitude: 0.0,
            TimeStep: 0.01,
            MaximumSimulationTime: 30.0);

        double HorizontalVelocity =
            50.0 / Math.Sqrt(2.0);

        return new SimulationParameters(
            InitialPosition: Vector3D.Zero,
            InitialVelocity: new Vector3D(
                HorizontalVelocity,
                0.0,
                HorizontalVelocity),
            Projectile: Projectile,
            Environment: Environment,
            Settings: Settings);
    }

    private static SimulationResult Simulate(
        SimulationParameters Parameters)
    {
        IAccelerationModel AccelerationModel =
            new CompositeAccelerationModel(
                new ConstantGravityModel(),
                new DragAccelerationModel());

        TrajectorySimulator Simulator = new();

        return Simulator.Simulate(
            Parameters,
            new RungeKutta4Solver(),
            AccelerationModel);
    }
}