using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Core.Solvers;

namespace TrajectoryLab.Application.Simulation.Factories;

public interface ISolverFactory
{
    INumericalSolver Create(SolverInput Input);
}