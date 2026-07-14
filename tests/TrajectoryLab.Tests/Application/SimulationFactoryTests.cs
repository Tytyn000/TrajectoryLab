using TrajectoryLab.Application.Models;
using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Application.Simulation.Factories;
using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Solvers;
using TrajectoryLab.Core.Wind;
using Xunit;

namespace TrajectoryLab.Tests.Application;

public sealed class SimulationFactoryTests
{
    [Fact]
    public void AtmosphereFactory_WithStandard1976_ReturnsStandardModel()
    {
        AtmosphereModelFactory Factory = new();

        IAtmosphereModel Model =
            Factory.Create(
                new AtmosphereInput
                {
                    ModelKind = AtmosphereModelKind.Standard1976
                },
                new IdealGasModel());

        Assert.IsType<StandardAtmosphere1976Model>(Model);
    }

    [Fact]
    public void AtmosphereFactory_WithPressureDefinition_UsesIdealGasLaw()
    {
        AtmosphereModelFactory Factory = new();

        ConstantAtmosphereModel Model =
            Assert.IsType<ConstantAtmosphereModel>(
                Factory.Create(
                    new AtmosphereInput
                    {
                        ModelKind = AtmosphereModelKind.Constant,
                        ConstantDefinition =
                            ConstantAtmosphereDefinitionKind
                                .PressureAndTemperature,
                        ConstantPressure = 101_325.0,
                        ConstantTemperature = 288.15
                    },
                    new IdealGasModel()));

        Assert.Equal(
            101_325.0
                / (
                    IdealGasModel.DryAirSpecificGasConstant
                    * 288.15
                ),
            Model.AirDensity,
            12);
    }

    [Fact]
    public void GravityFactory_WithBodyMass_DerivesSurfaceGravity()
    {
        GravityModelFactory Factory = new();

        CelestialBodyInput Input =
            new()
            {
                ModelKind = GravityModelKind.UniformSphere,
                UniformSphereDefinition =
                    UniformSphereDefinitionKind.Mass,
                BodyRadius = 6_371_000.0,
                BodyMass = 5.972168494074286e24
            };

        UniformSphereGravityModel Model =
            Assert.IsType<UniformSphereGravityModel>(
                Factory.Create(Input));

        Assert.Equal(
            Model.SurfaceGravityAcceleration,
            Factory.GetSurfaceGravityAcceleration(Input),
            12);
    }

    [Fact]
    public void WindFactory_WithLinearInput_ReturnsLinearModel()
    {
        WindModelFactory Factory = new();

        IWindModel Model =
            Factory.Create(
                new WindInput
                {
                    ModelKind = WindModelKind.Linear,
                    LowerAltitude = 0.0,
                    LowerWindX = 2.0,
                    UpperAltitude = 1_000.0,
                    UpperWindX = 12.0
                });

        LinearWindModel LinearModel =
            Assert.IsType<LinearWindModel>(Model);

        Assert.Equal(0.0, LinearModel.LowerAltitude);
        Assert.Equal(1_000.0, LinearModel.UpperAltitude);
    }

    [Fact]
    public void DragFactory_WithTabulatedInput_ReturnsTabulatedModel()
    {
        DragCoefficientModelFactory Factory = new();

        IDragCoefficientModel Model =
            Factory.Create(
                new ProjectileInput(),
                new DragInput
                {
                    IsEnabled = true,
                    ModelKind = DragCoefficientModelKind.Tabulated,
                    Points =
                    [
                        new DragCoefficientPointInput
                        {
                            MachNumber = 0.0,
                            DragCoefficient = 0.30
                        },
                        new DragCoefficientPointInput
                        {
                            MachNumber = 1.0,
                            DragCoefficient = 0.50
                        }
                    ]
                });

        TabulatedDragCoefficientModel TabulatedModel =
            Assert.IsType<TabulatedDragCoefficientModel>(Model);

        Assert.Equal(0.40, TabulatedModel.GetDragCoefficient(0.5), 12);
    }

    [Fact]
    public void SolverFactory_WithRungeKutta45_UsesRequestedTolerances()
    {
        SolverFactory Factory = new();

        INumericalSolver Solver =
            Factory.Create(
                new SolverInput
                {
                    ModelKind = SolverKind.RungeKutta45,
                    AbsoluteTolerance = 1.0e-9,
                    RelativeTolerance = 2.0e-9,
                    MinimumTimeStep = 1.0e-5,
                    MaximumTimeStep = 0.2
                });

        RungeKutta45Solver AdaptiveSolver =
            Assert.IsType<RungeKutta45Solver>(Solver);

        Assert.Equal(1.0e-9, AdaptiveSolver.AbsoluteTolerance);
        Assert.Equal(2.0e-9, AdaptiveSolver.RelativeTolerance);
        Assert.Equal(1.0e-5, AdaptiveSolver.MinimumTimeStep);
        Assert.Equal(0.2, AdaptiveSolver.MaximumTimeStep);
    }

    [Fact]
    public void AccelerationFactory_WithVectorCoriolis_ReturnsCompositeModel()
    {
        IAccelerationModelFactory Factory =
            new AccelerationModelFactory(
                new GravityModelFactory(),
                new DragCoefficientModelFactory());

        SimulationInput Input =
            new()
            {
                Atmosphere =
                    new AtmosphereInput
                    {
                        ModelKind = AtmosphereModelKind.Constant,
                        ConstantAirDensity = 1.225,
                        ConstantTemperature = 288.15
                    },
                Drag =
                    new DragInput
                    {
                        IsEnabled = true,
                        ModelKind = DragCoefficientModelKind.Constant
                    },
                Rotation =
                    new RotationInput
                    {
                        IsCoriolisEnabled = true,
                        DefinitionKind =
                            CoriolisDefinitionKind.AngularVelocityVector,
                        AngularVelocityZ = 1.0e-4
                    },
                Magnus =
                    new MagnusInput
                    {
                        IsEnabled = true,
                        AngularVelocityZ = 100.0,
                        MagnusCoefficient = 0.2
                    }
            };

        IAccelerationModel Model =
            Factory.Create(
                Input,
                new IdealGasModel());

        Assert.IsType<CompositeAccelerationModel>(Model);
    }

    [Fact]
    public void CoriolisVectorConstructor_UsesMinusTwoOmegaCrossVelocity()
    {
        CoriolisAccelerationModel Model =
            new(
                new Vector3D(
                    X: 0.0,
                    Y: 0.0,
                    Z: 2.0));

        SimulationState State =
            new(
                Time: 0.0,
                Position: Vector3D.Zero,
                Velocity: new Vector3D(
                    X: 3.0,
                    Y: 0.0,
                    Z: 0.0));

        Vector3D Acceleration =
            Model.GetAcceleration(
                State,
                Parameters: null!);

        Assert.Equal(0.0, Acceleration.X, 12);
        Assert.Equal(-12.0, Acceleration.Y, 12);
        Assert.Equal(0.0, Acceleration.Z, 12);
    }
}
