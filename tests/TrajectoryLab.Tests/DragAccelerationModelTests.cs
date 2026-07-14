using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Simulation;
using TrajectoryLab.Core.Solvers;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Tests.Physics;

public sealed class DragAccelerationModelTests
{
    private const double Tolerance =
        1.0e-12;

    [Fact]
    public void GetAcceleration_WithoutExplicitModel_UsesProjectileCoefficient()
    {
        SimulationParameters Parameters =
            CreateParameters(
                ProjectileDragCoefficient: 0.5);

        SimulationState State =
            new(
                Time: 0.0,
                Position: Vector3D.Zero,
                Velocity: new Vector3D(
                    10.0,
                    0.0,
                    0.0));

        DragAccelerationModel Model =
            new();

        Vector3D Acceleration =
            Model.GetAcceleration(
                State,
                Parameters);

        AssertClose(
            -1.25,
            Acceleration.X);

        AssertClose(
            0.0,
            Acceleration.Y);

        AssertClose(
            0.0,
            Acceleration.Z);
    }

    [Fact]
    public void GetAcceleration_WithExplicitModel_UsesModelCoefficient()
    {
        SimulationParameters Parameters =
            CreateParameters(
                ProjectileDragCoefficient: 0.1);

        SimulationState State =
            new(
                Time: 0.0,
                Position: Vector3D.Zero,
                Velocity: new Vector3D(
                    10.0,
                    0.0,
                    0.0));

        DragAccelerationModel Model =
            new(
                new ConstantDragCoefficientModel(
                    DragCoefficient: 0.5));

        Vector3D Acceleration =
            Model.GetAcceleration(
                State,
                Parameters);

        AssertClose(
            -1.25,
            Acceleration.X);

        AssertClose(
            0.0,
            Acceleration.Y);

        AssertClose(
            0.0,
            Acceleration.Z);
    }

    [Fact]
    public void GetAcceleration_PassesLocalMachNumberToModel()
    {
        const double Temperature =
            288.15;

        IdealGasModel GasModel =
            new();

        double SpeedOfSound =
            GasModel.GetSpeedOfSound(
                Temperature);

        RecordingDragCoefficientModel
            DragCoefficientModel =
                new(
                    ReturnedDragCoefficient: 0.47);

        DragAccelerationModel Model =
            new(
                DragCoefficientModel,
                GasModel);

        SimulationParameters Parameters =
            CreateParameters(
                ProjectileDragCoefficient: 0.1,
                Temperature: Temperature);

        SimulationState State =
            new(
                Time: 0.0,
                Position: Vector3D.Zero,
                Velocity: new Vector3D(
                    SpeedOfSound,
                    0.0,
                    0.0));

        Model.GetAcceleration(
            State,
            Parameters);

        Assert.NotNull(
            DragCoefficientModel
                .LastMachNumber);

        AssertClose(
            1.0,
            DragCoefficientModel
                .LastMachNumber!.Value);
    }

    [Fact]
    public void GetAcceleration_WithInvalidModelOutput_Throws()
    {
        SimulationParameters Parameters =
            CreateParameters(
                ProjectileDragCoefficient: 0.47);

        SimulationState State =
            new(
                Time: 0.0,
                Position: Vector3D.Zero,
                Velocity: new Vector3D(
                    10.0,
                    0.0,
                    0.0));

        DragAccelerationModel Model =
            new(
                new InvalidDragCoefficientModel());

        Assert.Throws<
            InvalidOperationException>(
            () =>
                Model.GetAcceleration(
                    State,
                    Parameters));
    }

    [Fact]
    public void Constructor_WithNullDragCoefficientModel_Throws()
    {
        Assert.Throws<
            ArgumentNullException>(
            () =>
                new DragAccelerationModel(
                    null!));
    }

    [Fact]
    public void Constructor_WithNullGasModel_Throws()
    {
        ConstantDragCoefficientModel
            DragCoefficientModel =
                new(
                    DragCoefficient: 0.47);

        Assert.Throws<
            ArgumentNullException>(
            () =>
                new DragAccelerationModel(
                    DragCoefficientModel,
                    null!));
    }

    [Fact]
    public void Constructor_StoresDependencies()
    {
        ConstantDragCoefficientModel
            DragCoefficientModel =
                new(
                    DragCoefficient: 0.47);

        IdealGasModel GasModel =
            new(
                SpecificHeatRatio: 1.3,
                SpecificGasConstant: 300.0);

        DragAccelerationModel Model =
            new(
                DragCoefficientModel,
                GasModel);

        Assert.Same(
            DragCoefficientModel,
            Model.DragCoefficientModel);

        Assert.Same(
            GasModel,
            Model.GasModel);
    }

    [Fact]
    public void GetAcceleration_WithZeroRelativeSpeed_ReturnsZero()
    {
        SimulationParameters Parameters =
            CreateParameters(
                ProjectileDragCoefficient: 0.47);

        SimulationState State =
            new(
                Time: 0.0,
                Position: Vector3D.Zero,
                Velocity: Vector3D.Zero);

        DragAccelerationModel Model =
            new(
                new ThrowingDragCoefficientModel());

        Vector3D Acceleration =
            Model.GetAcceleration(
                State,
                Parameters);

        Assert.Equal(
            Vector3D.Zero,
            Acceleration);
    }

    [Fact]
    public void ExplicitConstantModel_MatchesLegacyAcceleration()
    {
        SimulationParameters Parameters =
            CreateParameters(
                ProjectileDragCoefficient: 0.47,
                WindVelocity: new Vector3D(
                    2.0,
                    -1.0,
                    0.5));

        SimulationState State =
            new(
                Time: 2.0,
                Position: new Vector3D(
                    100.0,
                    20.0,
                    50.0),
                Velocity: new Vector3D(
                    30.0,
                    5.0,
                    -4.0));

        DragAccelerationModel LegacyModel =
            new();

        DragAccelerationModel ExplicitModel =
            new(
                new ConstantDragCoefficientModel(
                    DragCoefficient: 0.47));

        Vector3D LegacyAcceleration =
            LegacyModel.GetAcceleration(
                State,
                Parameters);

        Vector3D ExplicitAcceleration =
            ExplicitModel.GetAcceleration(
                State,
                Parameters);

        AssertVectorClose(
            LegacyAcceleration,
            ExplicitAcceleration);
    }

    [Fact]
    public void ExplicitConstantModel_MatchesLegacyTrajectory()
    {
        SimulationParameters Parameters =
            CreateTrajectoryParameters();

        TrajectorySimulator Simulator =
            new();

        RungeKutta4Solver Solver =
            new();

        CompositeAccelerationModel LegacyModel =
            new(
                new ConstantGravityModel(),
                new DragAccelerationModel());

        CompositeAccelerationModel ExplicitModel =
            new(
                new ConstantGravityModel(),
                new DragAccelerationModel(
                    new ConstantDragCoefficientModel(
                        DragCoefficient: 0.47)));

        SimulationResult LegacyResult =
            Simulator.Simulate(
                Parameters,
                Solver,
                LegacyModel);

        SimulationResult ExplicitResult =
            Simulator.Simulate(
                Parameters,
                Solver,
                ExplicitModel);

        Assert.Equal(
            LegacyResult.States.Count,
            ExplicitResult.States.Count);

        AssertClose(
            LegacyResult.FlightTime,
            ExplicitResult.FlightTime);

        AssertClose(
            LegacyResult.Range,
            ExplicitResult.Range);

        AssertClose(
            LegacyResult.MaximumAltitude,
            ExplicitResult.MaximumAltitude);

        AssertClose(
            LegacyResult.ImpactSpeed,
            ExplicitResult.ImpactSpeed);

        for (int Index = 0;
             Index < LegacyResult.States.Count;
             Index++)
        {
            SimulationState LegacyState =
                LegacyResult.States[Index];

            SimulationState ExplicitState =
                ExplicitResult.States[Index];

            AssertClose(
                LegacyState.Time,
                ExplicitState.Time);

            AssertVectorClose(
                LegacyState.Position,
                ExplicitState.Position);

            AssertVectorClose(
                LegacyState.Velocity,
                ExplicitState.Velocity);
        }
    }

    private static SimulationParameters
        CreateParameters(
            double ProjectileDragCoefficient,
            double Temperature = 288.15,
            Vector3D? WindVelocity = null)
    {
        return new SimulationParameters(
            InitialPosition: Vector3D.Zero,
            InitialVelocity: Vector3D.Zero,
            Projectile:
                new ProjectileParameters(
                    Mass: 2.0,
                    DragCoefficient:
                        ProjectileDragCoefficient,
                    CrossSectionalArea: 0.1),
            Environment:
                new EnvironmentParameters(
                    GravityAcceleration: 9.80665,
                    AtmosphereModel:
                        new ConstantAtmosphereModel(
                            AirDensity: 1.0,
                            Temperature:
                                Temperature),
                    WindModel:
                        new ConstantWindModel(
                            WindVelocity
                            ?? Vector3D.Zero)),
            Settings:
                new SimulationSettings(
                    TimeStep: 0.01,
                    MaximumSimulationTime:
                        60.0,
                    GroundAltitude: 0.0));
    }

    private static SimulationParameters
        CreateTrajectoryParameters()
    {
        return new SimulationParameters(
            InitialPosition:
                Vector3D.Zero,
            InitialVelocity:
                LaunchVelocity
                    .FromSpeedAndAngles(
                        Speed: 50.0,
                        ElevationDegrees: 45.0,
                        AzimuthDegrees: 0.0),
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
                            AirDensity: 1.225),
                    WindModel:
                        new ConstantWindModel(
                            Vector3D.Zero)),
            Settings:
                new SimulationSettings(
                    TimeStep: 0.01,
                    MaximumSimulationTime:
                        60.0,
                    GroundAltitude: 0.0));
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

    private sealed class
        RecordingDragCoefficientModel
        : IDragCoefficientModel
    {
        public double ReturnedDragCoefficient
        {
            get;
        }

        public double? LastMachNumber
        {
            get;
            private set;
        }

        public RecordingDragCoefficientModel(
            double ReturnedDragCoefficient)
        {
            this.ReturnedDragCoefficient =
                ReturnedDragCoefficient;
        }

        public double GetDragCoefficient(
            double MachNumber)
        {
            LastMachNumber =
                MachNumber;

            return ReturnedDragCoefficient;
        }
    }

    private sealed class
        InvalidDragCoefficientModel
        : IDragCoefficientModel
    {
        public double GetDragCoefficient(
            double MachNumber)
        {
            return double.NaN;
        }
    }

    private sealed class
        ThrowingDragCoefficientModel
        : IDragCoefficientModel
    {
        public double GetDragCoefficient(
            double MachNumber)
        {
            throw new InvalidOperationException(
                "Ce modèle ne doit pas être appelé.");
        }
    }
}