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
    TabulatedDragCoefficientIntegrationTests
{
    private const double Tolerance =
        1.0e-10;

    [Fact]
    public void GetAcceleration_UsesInterpolatedCoefficientAtLocalMach()
    {
        const double Temperature =
            288.15;

        IdealGasModel GasModel =
            new();

        double SpeedOfSound =
            GasModel.GetSpeedOfSound(
                Temperature);

        double RelativeSpeed =
            0.75 * SpeedOfSound;

        TabulatedDragCoefficientModel
            DragCoefficientModel =
                new(
                [
                    new DragCoefficientPoint(
                        MachNumber: 0.0,
                        DragCoefficient: 0.2),

                    new DragCoefficientPoint(
                        MachNumber: 0.5,
                        DragCoefficient: 0.4),

                    new DragCoefficientPoint(
                        MachNumber: 1.0,
                        DragCoefficient: 0.8),

                    new DragCoefficientPoint(
                        MachNumber: 2.0,
                        DragCoefficient: 0.6)
                ]);

        DragAccelerationModel Model =
            new(
                DragCoefficientModel,
                GasModel);

        SimulationParameters Parameters =
            CreateParameters(
                InitialSpeed: 0.0,
                ProjectileDragCoefficient: 0.01,
                Mass: 2.0,
                CrossSectionalArea: 0.1,
                AirDensity: 1.0,
                Temperature: Temperature);

        SimulationState State =
            new(
                Time: 0.0,
                Position: Vector3D.Zero,
                Velocity: new Vector3D(
                    RelativeSpeed,
                    0.0,
                    0.0));

        Vector3D Acceleration =
            Model.GetAcceleration(
                State,
                Parameters);

        const double ExpectedDragCoefficient =
            0.6;

        double ExpectedAccelerationX =
            -0.5
            * 1.0
            * ExpectedDragCoefficient
            * 0.1
            / 2.0
            * RelativeSpeed
            * RelativeSpeed;

        AssertClose(
            ExpectedAccelerationX,
            Acceleration.X);

        AssertClose(
            0.0,
            Acceleration.Y);

        AssertClose(
            0.0,
            Acceleration.Z);
    }

    [Fact]
    public void ConstantTable_MatchesConstantDragCoefficientModel()
    {
        SimulationParameters Parameters =
            CreateParameters(
                InitialSpeed: 50.0,
                ProjectileDragCoefficient: 0.47);

        CompositeAccelerationModel
            ConstantAccelerationModel =
                new(
                    new ConstantGravityModel(),
                    new DragAccelerationModel(
                        new ConstantDragCoefficientModel(
                            DragCoefficient: 0.47)));

        CompositeAccelerationModel
            TabulatedAccelerationModel =
                new(
                    new ConstantGravityModel(),
                    new DragAccelerationModel(
                        new TabulatedDragCoefficientModel(
                        [
                            new DragCoefficientPoint(
                                MachNumber: 0.0,
                                DragCoefficient: 0.47),

                            new DragCoefficientPoint(
                                MachNumber: 5.0,
                                DragCoefficient: 0.47)
                        ])));

        SimulationResult ConstantResult =
            Simulate(
                Parameters,
                ConstantAccelerationModel);

        SimulationResult TabulatedResult =
            Simulate(
                Parameters,
                TabulatedAccelerationModel);

        Assert.Equal(
            ConstantResult.States.Count,
            TabulatedResult.States.Count);

        AssertClose(
            ConstantResult.FlightTime,
            TabulatedResult.FlightTime);

        AssertClose(
            ConstantResult.Range,
            TabulatedResult.Range);

        AssertClose(
            ConstantResult.MaximumAltitude,
            TabulatedResult.MaximumAltitude);

        AssertClose(
            ConstantResult.ImpactSpeed,
            TabulatedResult.ImpactSpeed);

        for (int Index = 0;
             Index < ConstantResult.States.Count;
             Index++)
        {
            SimulationState ConstantState =
                ConstantResult.States[Index];

            SimulationState TabulatedState =
                TabulatedResult.States[Index];

            AssertClose(
                ConstantState.Time,
                TabulatedState.Time);

            AssertVectorClose(
                ConstantState.Position,
                TabulatedState.Position);

            AssertVectorClose(
                ConstantState.Velocity,
                TabulatedState.Velocity);
        }
    }

    [Fact]
    public void IncreasingCoefficientAtHighMach_ReducesRange()
    {
        SimulationParameters Parameters =
            CreateParameters(
                InitialSpeed: 250.0,
                ProjectileDragCoefficient: 0.1,
                Mass: 2.0,
                CrossSectionalArea: 0.01);

        TabulatedDragCoefficientModel
            LowDragModel =
                new(
                [
                    new DragCoefficientPoint(
                        MachNumber: 0.0,
                        DragCoefficient: 0.1),

                    new DragCoefficientPoint(
                        MachNumber: 2.0,
                        DragCoefficient: 0.1)
                ]);

        TabulatedDragCoefficientModel
            MachDependentModel =
                new(
                [
                    new DragCoefficientPoint(
                        MachNumber: 0.0,
                        DragCoefficient: 0.1),

                    new DragCoefficientPoint(
                        MachNumber: 0.5,
                        DragCoefficient: 0.1),

                    new DragCoefficientPoint(
                        MachNumber: 0.8,
                        DragCoefficient: 0.6),

                    new DragCoefficientPoint(
                        MachNumber: 2.0,
                        DragCoefficient: 0.6)
                ]);

        CompositeAccelerationModel
            LowDragAccelerationModel =
                new(
                    new ConstantGravityModel(),
                    new DragAccelerationModel(
                        LowDragModel));

        CompositeAccelerationModel
            MachDependentAccelerationModel =
                new(
                    new ConstantGravityModel(),
                    new DragAccelerationModel(
                        MachDependentModel));

        SimulationResult LowDragResult =
            Simulate(
                Parameters,
                LowDragAccelerationModel);

        SimulationResult MachDependentResult =
            Simulate(
                Parameters,
                MachDependentAccelerationModel);

        Assert.True(
            MachDependentResult.Range
            < LowDragResult.Range,
            "Une augmentation du coefficient de traînée à Mach élevé doit réduire la portée.");
    }

    private static SimulationParameters
        CreateParameters(
            double InitialSpeed,
            double ProjectileDragCoefficient,
            double Mass = 1.0,
            double CrossSectionalArea = 0.01,
            double AirDensity = 1.225,
            double Temperature = 288.15)
    {
        Vector3D InitialVelocity =
            LaunchVelocity.FromSpeedAndAngles(
                Speed: InitialSpeed,
                ElevationDegrees: 45.0,
                AzimuthDegrees: 0.0);

        return new SimulationParameters(
            InitialPosition: Vector3D.Zero,
            InitialVelocity: InitialVelocity,
            Projectile:
                new ProjectileParameters(
                    Mass: Mass,
                    DragCoefficient:
                        ProjectileDragCoefficient,
                    CrossSectionalArea:
                        CrossSectionalArea),
            Environment:
                new EnvironmentParameters(
                    GravityAcceleration: 9.80665,
                    AtmosphereModel:
                        new ConstantAtmosphereModel(
                            AirDensity: AirDensity,
                            Temperature: Temperature),
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