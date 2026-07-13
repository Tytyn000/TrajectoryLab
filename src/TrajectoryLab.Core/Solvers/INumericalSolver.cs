using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;

namespace TrajectoryLab.Core.Solvers;

// Fait avancer la simulation d'un pas de temps.
public interface INumericalSolver
{
    string Name { get; }

    SimulationState Step(
        SimulationState State,
        double TimeStep,
        IAccelerationModel AccelerationModel,
        SimulationParameters Parameters
    );
}