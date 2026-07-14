using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;

namespace TrajectoryLab.Core.Solvers;

/// Définit un solveur capable d'accepter ou de rejeter dynamiquement un pas.
public interface IAdaptiveNumericalSolver : INumericalSolver
{
    double MinimumTimeStep { get; }

    double MaximumTimeStep { get; }

    AdaptiveStepResult StepAdaptive(
        SimulationState State,
        double TimeStep,
        IAccelerationModel AccelerationModel,
        SimulationParameters Parameters);
}
