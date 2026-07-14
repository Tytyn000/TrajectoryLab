using TrajectoryLab.Application.Models;
using TrajectoryLab.Core.Simulation;

namespace TrajectoryLab.Application.Simulation;

public sealed class SimulationService :
    ISimulationService
{
    private readonly ISimulationScenarioFactory
        ScenarioFactory;

    private readonly ITrajectoryProjectionService
        ProjectionService;

    public SimulationService(
        ISimulationScenarioFactory ScenarioFactory,
        ITrajectoryProjectionService ProjectionService)
    {
        ArgumentNullException.ThrowIfNull(
            ScenarioFactory);

        ArgumentNullException.ThrowIfNull(
            ProjectionService);

        this.ScenarioFactory =
            ScenarioFactory;

        this.ProjectionService =
            ProjectionService;
    }

    public Task<SimulationRunResult> RunAsync(
        SimulationInput Input,
        CancellationToken CancellationToken,
        IProgress<double>? Progress = null)
    {
        ArgumentNullException.ThrowIfNull(
            Input);

        return Task.Run(
            () =>
            {
                CancellationToken
                    .ThrowIfCancellationRequested();

                SimulationScenario Scenario =
                    ScenarioFactory.Create(
                        Input);

                SimulationRunner Runner =
                    new();

                SimulationReport Report =
                    Runner.Run(
                        Scenario,
                        CancellationToken,
                        Progress);

                CancellationToken
                    .ThrowIfCancellationRequested();

                return ProjectionService.Project(
                    Report,
                    Scenario.Solver.Name);
            },
            CancellationToken);
    }
}
