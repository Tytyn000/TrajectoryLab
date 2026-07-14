using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Desktop.Models;

namespace TrajectoryLab.Desktop.ViewModels;

public sealed class SolverParametersViewModel
{
    public IReadOnlyList<SelectionOption<SolverKind>>
        SolverOptions =>
            SimulationOptionCatalog.Solvers;

    public SolverKind SelectedSolver { get; set; } =
        SolverKind.RungeKutta4;

    public string TimeStep { get; set; } = "0.01";
    public string AbsoluteTolerance { get; set; } = "1e-8";
    public string RelativeTolerance { get; set; } = "1e-8";
    public string MinimumTimeStep { get; set; } = "1e-6";
    public string MaximumTimeStep { get; set; } = "1";
}