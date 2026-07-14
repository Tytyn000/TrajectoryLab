using TrajectoryLab.Application.Models;

namespace TrajectoryLab.Application.Simulation;

public interface ISimulationService
{
    Task<SimulationRunResult> RunAsync(
        SimulationInput Input,
        CancellationToken CancellationToken,
        IProgress<double>? Progress = null);
}
