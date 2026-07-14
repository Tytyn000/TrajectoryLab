using TrajectoryLab.Application.Models;
using TrajectoryLab.Core.Simulation;

namespace TrajectoryLab.Application.Simulation;

public interface ITrajectoryProjectionService
{
    SimulationRunResult Project(
        SimulationReport Report,
        string SolverName);
}
