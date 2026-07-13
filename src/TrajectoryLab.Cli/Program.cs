using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Simulation;
using TrajectoryLab.Core.Solvers;

const double Speed = 50.0;
const double ElevationDegrees = 45.0;
const double AzimuthDegrees = 0.0;

SimulationParameters Parameters = new()
{
    InitialPosition = Vector3D.Zero,

    InitialVelocity =
        LaunchVelocity.FromSpeedAndAngles(
            Speed,
            ElevationDegrees,
            AzimuthDegrees
        ),

    Mass = 1.0,
    DragCoefficient = 0.47,
    CrossSectionalArea = 0.01,
    AirDensity = 1.225,
    WindVelocity = Vector3D.Zero,

    TimeStep = 0.01,
    MaximumDuration = 60.0
};

TrajectorySimulator Simulator = new();
RungeKutta4Solver Solver = new();

CompositeAccelerationModel VacuumModel = new(
    new ConstantGravityModel()
);

CompositeAccelerationModel AirModel = new(
    new ConstantGravityModel(),
    new DragAccelerationModel()
);

RunAndDisplay(
    "Sans traînée",
    Solver,
    Simulator,
    VacuumModel,
    Parameters
);

Console.WriteLine();

RunAndDisplay(
    "Avec traînée",
    Solver,
    Simulator,
    AirModel,
    Parameters
);

static void RunAndDisplay(
    string ScenarioName,
    INumericalSolver Solver,
    TrajectorySimulator Simulator,
    IAccelerationModel AccelerationModel,
    SimulationParameters Parameters)
{
    SimulationResult Result = Simulator.Simulate(
        Parameters,
        Solver,
        AccelerationModel
    );

    Console.WriteLine($"Scénario : {ScenarioName}");
    Console.WriteLine($"Méthode : {Solver.Name}");
    Console.WriteLine(
        $"Durée du vol : {Result.FlightTime:F6} s"
    );
    Console.WriteLine(
        $"Portée : {Result.Range:F6} m"
    );
    Console.WriteLine(
        $"Altitude maximale : {Result.MaximumAltitude:F6} m"
    );
    Console.WriteLine(
        $"Vitesse d'impact : {Result.ImpactSpeed:F6} m/s"
    );
    Console.WriteLine(
        $"Nombre d'états : {Result.States.Count}"
    );
}