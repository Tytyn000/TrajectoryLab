using TrajectoryLab.Application.Models;
using TrajectoryLab.Application.Simulation.Factories;
using TrajectoryLab.Application.Validation;
using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Simulation;
using TrajectoryLab.Core.Solvers;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Application.Simulation;

public sealed class DefaultSimulationScenarioFactory :
    ISimulationScenarioFactory
{
    private readonly ISimulationInputValidator InputValidator;
    private readonly IAtmosphereModelFactory AtmosphereModelFactory;
    private readonly IWindModelFactory WindModelFactory;
    private readonly IGravityModelFactory GravityModelFactory;
    private readonly IAccelerationModelFactory AccelerationModelFactory;
    private readonly ISolverFactory SolverFactory;

    public DefaultSimulationScenarioFactory(
        ISimulationInputValidator InputValidator,
        IAtmosphereModelFactory AtmosphereModelFactory,
        IWindModelFactory WindModelFactory,
        IGravityModelFactory GravityModelFactory,
        IAccelerationModelFactory AccelerationModelFactory,
        ISolverFactory SolverFactory)
    {
        ArgumentNullException.ThrowIfNull(InputValidator);
        ArgumentNullException.ThrowIfNull(AtmosphereModelFactory);
        ArgumentNullException.ThrowIfNull(WindModelFactory);
        ArgumentNullException.ThrowIfNull(GravityModelFactory);
        ArgumentNullException.ThrowIfNull(AccelerationModelFactory);
        ArgumentNullException.ThrowIfNull(SolverFactory);

        this.InputValidator = InputValidator;
        this.AtmosphereModelFactory = AtmosphereModelFactory;
        this.WindModelFactory = WindModelFactory;
        this.GravityModelFactory = GravityModelFactory;
        this.AccelerationModelFactory = AccelerationModelFactory;
        this.SolverFactory = SolverFactory;
    }

    public SimulationScenario Create(SimulationInput Input)
    {
        InputValidator.Validate(Input);

        IdealGasModel GasModel =
            new(
                Input.Gas.SpecificHeatRatio,
                Input.Gas.SpecificGasConstant);

        IAtmosphereModel AtmosphereModel =
            AtmosphereModelFactory.Create(
                Input.Atmosphere,
                GasModel);

        IWindModel WindModel =
            WindModelFactory.Create(Input.Wind);

        ProjectileParameters Projectile =
            new(
                Mass: Input.Projectile.Mass,
                DragCoefficient: Input.Projectile.ReferenceDragCoefficient,
                CrossSectionalArea: Input.Projectile.CrossSectionalArea);

        EnvironmentParameters Environment =
            new(
                GravityAcceleration:
                    GravityModelFactory
                        .GetSurfaceGravityAcceleration(
                            Input.CelestialBody),
                AtmosphereModel: AtmosphereModel,
                WindModel: WindModel);

        SimulationSettings Settings =
            new(
                TimeStep: Input.Solver.TimeStep,
                MaximumSimulationTime:
                    Input.Limits.MaximumSimulationTime,
                GroundAltitude: Input.Limits.GroundAltitude);

        Vector3D InitialVelocity =
            LaunchVelocity.FromSpeedAndAngles(
                Input.InitialConditions.InitialSpeed,
                Input.InitialConditions.ElevationDegrees,
                Input.InitialConditions.AzimuthDegrees);

        SimulationParameters Parameters =
            new(
                InitialPosition:
                    new Vector3D(
                        Input.InitialConditions.InitialX,
                        Input.InitialConditions.InitialY,
                        Input.InitialConditions.InitialZ),
                InitialVelocity: InitialVelocity,
                Projectile: Projectile,
                Environment: Environment,
                Settings: Settings);

        IAccelerationModel AccelerationModel =
            AccelerationModelFactory.Create(Input, GasModel);

        INumericalSolver Solver =
            SolverFactory.Create(Input.Solver);

        return new SimulationScenario(
            Parameters,
            Solver,
            AccelerationModel,
            GasModel);
    }
}