using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Core.Solvers;

namespace TrajectoryLab.Application.Simulation.Factories;

public sealed class SolverFactory :
    ISolverFactory
{
    public INumericalSolver Create(SolverInput Input)
    {
        ArgumentNullException.ThrowIfNull(Input);

        return Input.ModelKind switch
        {
            SolverKind.Euler =>
                new EulerSolver(),

            SolverKind.RungeKutta4 =>
                new RungeKutta4Solver(),

            SolverKind.RungeKutta45 =>
                new RungeKutta45Solver(
                    AbsoluteTolerance: Input.AbsoluteTolerance,
                    RelativeTolerance: Input.RelativeTolerance,
                    MinimumTimeStep: Input.MinimumTimeStep,
                    MaximumTimeStep: Input.MaximumTimeStep),

            _ => throw new ArgumentOutOfRangeException(
                nameof(Input),
                Input.ModelKind,
                "Le solveur sÃ©lectionnÃ© est inconnu.")
        };
    }
}