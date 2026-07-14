using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;

namespace TrajectoryLab.Core.Simulation;

// Exécute un scénario et calcule les grandeurs dérivées de la trajectoire.
public sealed class SimulationRunner
{
    public SimulationReport Run(
        SimulationScenario Scenario)
    {
        return Run(
            Scenario,
            CancellationToken.None,
            Progress: null);
    }

    public SimulationReport Run(
        SimulationScenario Scenario,
        CancellationToken CancellationToken,
        IProgress<double>? Progress = null)
    {
        ArgumentNullException.ThrowIfNull(Scenario);

        CancellationToken.ThrowIfCancellationRequested();

        TrajectorySimulator Simulator = new();

        SimulationResult Result = Simulator.Simulate(
            Scenario.Parameters,
            Scenario.Solver,
            Scenario.AccelerationModel,
            CancellationToken,
            Progress);

        CancellationToken.ThrowIfCancellationRequested();

        FlightConditionCalculator FlightConditionCalculator = new(
            Scenario.GasModel);

        IReadOnlyList<FlightCondition> FlightConditions =
            FlightConditionCalculator.CalculateAll(
                Result,
                Scenario.Parameters);

        CancellationToken.ThrowIfCancellationRequested();

        AerodynamicSummaryCalculator SummaryCalculator = new();

        AerodynamicSummary AerodynamicSummary =
            SummaryCalculator.Calculate(
                FlightConditions);

        CancellationToken.ThrowIfCancellationRequested();

        return new SimulationReport(
            Result,
            FlightConditions,
            AerodynamicSummary);
    }
}
