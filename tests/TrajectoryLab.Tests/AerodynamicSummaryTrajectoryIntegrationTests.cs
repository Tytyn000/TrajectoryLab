using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Simulation;
using TrajectoryLab.Core.Solvers;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Tests.Physics;

public sealed class AerodynamicSummaryTrajectoryIntegrationTests
{
    private const double Tolerance = 1.0e-12;

    [Fact]
    public void Calculate_ForTrajectory_ReturnsExpectedExtrema()
    {
        SimulationParameters Parameters =
            CreateParameters(
                new LinearWindModel(
                    LowerAltitude: 0.0,
                    LowerWindVelocity: Vector3D.Zero,
                    UpperAltitude: 100.0,
                    UpperWindVelocity: new Vector3D(
                        0.0,
                        20.0,
                        0.0)));

        SimulationResult Result =
            Simulate(Parameters);

        IReadOnlyList<FlightCondition> Conditions =
            new FlightConditionCalculator(
                new IdealGasModel())
                .CalculateAll(
                    Result,
                    Parameters);

        AerodynamicSummary Summary =
            new AerodynamicSummaryCalculator()
                .Calculate(Conditions);

        int MaximumMachIndex = 0;

        double MaximumRelativeSpeed =
            Conditions[0].RelativeSpeed;

        for (int Index = 1;
             Index < Conditions.Count;
             Index++)
        {
            if (Conditions[Index].MachNumber
                > Conditions[MaximumMachIndex].MachNumber)
            {
                MaximumMachIndex = Index;
            }

            if (Conditions[Index].RelativeSpeed
                > MaximumRelativeSpeed)
            {
                MaximumRelativeSpeed =
                    Conditions[Index].RelativeSpeed;
            }
        }

        AssertClose(
            Conditions[0].MachNumber,
            Summary.InitialMachNumber);

        AssertClose(
            Conditions[^1].MachNumber,
            Summary.ImpactMachNumber);

        AssertClose(
            Conditions[MaximumMachIndex].MachNumber,
            Summary.MaximumMachNumber);

        AssertClose(
            MaximumRelativeSpeed,
            Summary.MaximumRelativeSpeed);

        AssertClose(
            Conditions[MaximumMachIndex].Time,
            Summary.MaximumMachTime);

        AssertClose(
            Conditions[MaximumMachIndex].Position.Z,
            Summary.MaximumMachAltitude);
    }

    [Fact]
    public void Calculate_WithTailwind_UsesRelativeSpeedsRatherThanGroundSpeeds()
    {
        ConstantWindModel WindModel = new(
            new Vector3D(
                10.0,
                0.0,
                0.0));

        SimulationParameters Parameters =
            CreateParameters(WindModel);

        SimulationResult Result =
            Simulate(Parameters);

        IReadOnlyList<FlightCondition> Conditions =
            new FlightConditionCalculator(
                new IdealGasModel())
                .CalculateAll(
                    Result,
                    Parameters);

        AerodynamicSummary Summary =
            new AerodynamicSummaryCalculator()
                .Calculate(Conditions);

        double InitialGroundSpeed =
            Parameters.InitialVelocity.Length();

        Assert.True(
            Summary.InitialRelativeSpeed
            < InitialGroundSpeed,
            "Un vent arrière doit réduire la vitesse relative initiale.");

        AssertClose(
            Conditions[0].RelativeSpeed,
            Summary.InitialRelativeSpeed);

        AssertClose(
            Conditions[^1].RelativeSpeed,
            Summary.ImpactRelativeSpeed);
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

        return new TrajectorySimulator().Simulate(
            Parameters,
            new RungeKutta4Solver(),
            AccelerationModel);
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