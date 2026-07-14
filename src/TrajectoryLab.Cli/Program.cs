using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Simulation;
using TrajectoryLab.Core.Solvers;
using TrajectoryLab.Core.Wind;

const double Speed = 50.0;
const double HighSpeed = 1000.0;
const double ElevationDegrees = 45.0;
const double AzimuthDegrees = 0.0;
const double LatitudeDegrees = 45.0;
const double EarthMeanRadius = 6_371_000.0;
const double EarthSurfaceGravity = 9.80665;
const double ProjectileRadius = 0.05;
const double MagnusCoefficient = 0.5;
const double SpinRate = 200.0;

Vector3D InitialVelocity =
    LaunchVelocity.FromSpeedAndAngles(
        Speed,
        ElevationDegrees,
        AzimuthDegrees
    );

Vector3D HighSpeedInitialVelocity =
    LaunchVelocity.FromSpeedAndAngles(
        HighSpeed,
        ElevationDegrees,
        AzimuthDegrees
    );

Vector3D VerticalSpin =
    new(
        X: 0.0,
        Y: 0.0,
        Z: SpinRate
    );

ProjectileParameters Projectile =
    new(
        Mass: 1.0,
        DragCoefficient: 0.47,
        CrossSectionalArea: 0.01
    );

SimulationSettings Settings =
    new(
        TimeStep: 0.01,
        MaximumSimulationTime: 60.0,
        GroundAltitude: 0.0
    );

SimulationSettings HighSpeedSettings =
    new(
        TimeStep: 0.02,
        MaximumSimulationTime: 300.0,
        GroundAltitude: 0.0
    );

IWindModel CalmWindModel =
    new ConstantWindModel(
        WindVelocity: Vector3D.Zero
    );

EnvironmentParameters ConstantEnvironment =
    new(
        GravityAcceleration:
            EarthSurfaceGravity,
        AtmosphereModel:
            new ConstantAtmosphereModel(
                AirDensity: 1.225
            ),
        WindModel: CalmWindModel
    );

EnvironmentParameters StandardEnvironment =
    new(
        GravityAcceleration:
            EarthSurfaceGravity,
        AtmosphereModel:
            new StandardAtmosphere1976Model(),
        WindModel: CalmWindModel
    );

SimulationParameters ConstantParameters =
    new(
        InitialPosition: Vector3D.Zero,
        InitialVelocity: InitialVelocity,
        Projectile: Projectile,
        Environment: ConstantEnvironment,
        Settings: Settings
    );

SimulationParameters StandardParameters =
    new(
        InitialPosition: Vector3D.Zero,
        InitialVelocity: InitialVelocity,
        Projectile: Projectile,
        Environment: StandardEnvironment,
        Settings: Settings
    );

SimulationParameters HighSpeedParameters =
    new(
        InitialPosition: Vector3D.Zero,
        InitialVelocity:
            HighSpeedInitialVelocity,
        Projectile: Projectile,
        Environment: StandardEnvironment,
        Settings: HighSpeedSettings
    );

TrajectorySimulator Simulator =
    new();

RungeKutta4Solver Solver =
    new();

FlightConditionCalculator ConditionCalculator =
    new(
        new IdealGasModel()
    );

AerodynamicSummaryCalculator SummaryCalculator =
    new();

ConstantDragCoefficientModel
    ConstantDragCoefficientModel =
        new(
            DragCoefficient:
                Projectile.DragCoefficient
        );

UniformSphereGravityModel
    EarthUniformGravityModel =
        UniformSphereGravityModel
            .FromSurfaceGravity(
                BodyRadius:
                    EarthMeanRadius,
                SurfaceGravityAcceleration:
                    EarthSurfaceGravity
            );

MagnusAccelerationModel
    VerticalSpinMagnusModel =
        new(
            AngularVelocity:
                VerticalSpin,
            ProjectileRadius:
                ProjectileRadius,
            MagnusCoefficient:
                MagnusCoefficient
        );

CompositeAccelerationModel VacuumModel =
    new(
        new ConstantGravityModel()
    );

CompositeAccelerationModel DragModel =
    new(
        new ConstantGravityModel(),
        new DragAccelerationModel(
            ConstantDragCoefficientModel
        )
    );

CompositeAccelerationModel CoriolisDragModel =
    new(
        new ConstantGravityModel(),
        new CoriolisAccelerationModel(
            LatitudeDegrees
        ),
        new DragAccelerationModel(
            ConstantDragCoefficientModel
        )
    );

CompositeAccelerationModel MagnusDragModel =
    new(
        new ConstantGravityModel(),
        new DragAccelerationModel(
            ConstantDragCoefficientModel
        ),
        VerticalSpinMagnusModel
    );

CompositeAccelerationModel
    UniformGravityVacuumModel =
        new(
            EarthUniformGravityModel
        );

RunAndDisplay(
    ScenarioName: "Sans traînée",
    Solver: Solver,
    Simulator: Simulator,
    AccelerationModel: VacuumModel,
    Parameters: StandardParameters,
    ConditionCalculator: ConditionCalculator,
    SummaryCalculator: SummaryCalculator
);

Console.WriteLine();

RunAndDisplay(
    ScenarioName:
        "Avec traînée et densité constante",
    Solver: Solver,
    Simulator: Simulator,
    AccelerationModel: DragModel,
    Parameters: ConstantParameters,
    ConditionCalculator: ConditionCalculator,
    SummaryCalculator: SummaryCalculator
);

Console.WriteLine();

RunAndDisplay(
    ScenarioName:
        "Avec traînée et atmosphère standard 1976",
    Solver: Solver,
    Simulator: Simulator,
    AccelerationModel: DragModel,
    Parameters: StandardParameters,
    ConditionCalculator: ConditionCalculator,
    SummaryCalculator: SummaryCalculator
);

Console.WriteLine();

RunAndDisplay(
    ScenarioName:
        "Avec traînée, atmosphère standard 1976 et Coriolis à 45° N",
    Solver: Solver,
    Simulator: Simulator,
    AccelerationModel:
        CoriolisDragModel,
    Parameters: StandardParameters,
    ConditionCalculator: ConditionCalculator,
    SummaryCalculator: SummaryCalculator
);

Console.WriteLine();

RunAndDisplay(
    ScenarioName:
        "Avec traînée, atmosphère standard 1976 et effet Magnus",
    Solver: Solver,
    Simulator: Simulator,
    AccelerationModel:
        MagnusDragModel,
    Parameters: StandardParameters,
    ConditionCalculator: ConditionCalculator,
    SummaryCalculator: SummaryCalculator
);

Console.WriteLine();

RunAndDisplay(
    ScenarioName:
        "Tir rapide sans traînée et gravité constante",
    Solver: Solver,
    Simulator: Simulator,
    AccelerationModel:
        VacuumModel,
    Parameters:
        HighSpeedParameters,
    ConditionCalculator:
        ConditionCalculator,
    SummaryCalculator:
        SummaryCalculator
);

Console.WriteLine();

RunAndDisplay(
    ScenarioName:
        "Tir rapide sans traînée et astre uniforme",
    Solver: Solver,
    Simulator: Simulator,
    AccelerationModel:
        UniformGravityVacuumModel,
    Parameters:
        HighSpeedParameters,
    ConditionCalculator:
        ConditionCalculator,
    SummaryCalculator:
        SummaryCalculator
);

static void RunAndDisplay(
    string ScenarioName,
    INumericalSolver Solver,
    TrajectorySimulator Simulator,
    IAccelerationModel AccelerationModel,
    SimulationParameters Parameters,
    FlightConditionCalculator ConditionCalculator,
    AerodynamicSummaryCalculator SummaryCalculator)
{
    SimulationResult Result =
        Simulator.Simulate(
            Parameters,
            Solver,
            AccelerationModel
        );

    IReadOnlyList<FlightCondition> Conditions =
        ConditionCalculator.CalculateAll(
            Result,
            Parameters
        );

    AerodynamicSummary Summary =
        SummaryCalculator.Calculate(
            Conditions
        );

    SimulationState ImpactState =
        Result.ImpactState;

    Console.WriteLine(
        $"Scénario : {ScenarioName}"
    );

    Console.WriteLine(
        $"Méthode : {Solver.Name}"
    );

    Console.WriteLine(
        $"Durée du vol : {Result.FlightTime:F6} s"
    );

    Console.WriteLine(
        $"Portée : {Result.Range:F6} m"
    );

    Console.WriteLine(
        $"Déviation Nord-Sud à l'impact : " +
        $"{ImpactState.Position.Y:F6} m"
    );

    Console.WriteLine(
        $"Altitude maximale : " +
        $"{Result.MaximumAltitude:F6} m"
    );

    Console.WriteLine(
        $"Vitesse d'impact : " +
        $"{Result.ImpactSpeed:F6} m/s"
    );

    Console.WriteLine(
        $"Nombre d'états : " +
        $"{Result.States.Count}"
    );

    Console.WriteLine(
        $"Vitesse relative initiale : " +
        $"{Summary.InitialRelativeSpeed:F6} m/s"
    );

    Console.WriteLine(
        $"Vitesse relative maximale : " +
        $"{Summary.MaximumRelativeSpeed:F6} m/s"
    );

    Console.WriteLine(
        $"Vitesse relative à l'impact : " +
        $"{Summary.ImpactRelativeSpeed:F6} m/s"
    );

    Console.WriteLine(
        $"Mach initial : " +
        $"{Summary.InitialMachNumber:F6}"
    );

    Console.WriteLine(
        $"Mach maximal : " +
        $"{Summary.MaximumMachNumber:F6}"
    );

    Console.WriteLine(
        $"Mach à l'impact : " +
        $"{Summary.ImpactMachNumber:F6}"
    );

    Console.WriteLine(
        $"Instant du Mach maximal : " +
        $"{Summary.MaximumMachTime:F6} s"
    );

    Console.WriteLine(
        $"Altitude du Mach maximal : " +
        $"{Summary.MaximumMachAltitude:F6} m"
    );
}