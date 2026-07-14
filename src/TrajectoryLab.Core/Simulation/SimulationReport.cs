using TrajectoryLab.Core.Models;

namespace TrajectoryLab.Core.Simulation;

/// Regroupe le résultat brut et les grandeurs dérivées d'une simulation.
public sealed class SimulationReport
{
    public SimulationResult Result { get; }

    public IReadOnlyList<FlightCondition> FlightConditions { get; }

    public AerodynamicSummary AerodynamicSummary { get; }

    public SimulationReport(
        SimulationResult Result,
        IReadOnlyList<FlightCondition> FlightConditions,
        AerodynamicSummary AerodynamicSummary)
    {
        ArgumentNullException.ThrowIfNull(Result);
        ArgumentNullException.ThrowIfNull(FlightConditions);
        ArgumentNullException.ThrowIfNull(AerodynamicSummary);

        this.Result = Result;

        this.FlightConditions = Array.AsReadOnly(
            FlightConditions.ToArray());

        this.AerodynamicSummary = AerodynamicSummary;
    }
}