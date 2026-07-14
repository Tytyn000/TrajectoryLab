using TrajectoryLab.Application.Models;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Simulation;

namespace TrajectoryLab.Application.Simulation;

public sealed class TrajectoryProjectionService :
    ITrajectoryProjectionService
{
    public SimulationRunResult Project(
        SimulationReport Report,
        string SolverName)
    {
        ArgumentNullException.ThrowIfNull(
            Report);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            SolverName);

        IReadOnlyList<SimulationState> States =
            Report.Result.States;

        if (States.Count == 0)
        {
            throw new InvalidOperationException(
                "Le rapport ne contient aucun Ã©tat.");
        }

        SimulationState InitialState =
            States[0];

        TrajectorySample[] Samples =
            States
                .Select(
                    State =>
                    {
                        double DeltaX =
                            State.Position.X -
                            InitialState.Position.X;

                        double DeltaY =
                            State.Position.Y -
                            InitialState.Position.Y;

                        double HorizontalDistance =
                            Math.Sqrt(
                                DeltaX *
                                DeltaX +
                                DeltaY *
                                DeltaY);

                        return new TrajectorySample(
                            Time:
                                State.Time,

                            X:
                                State.Position.X,

                            Y:
                                State.Position.Y,

                            Z:
                                State.Position.Z,

                            HorizontalDistance:
                                HorizontalDistance,

                            Speed:
                                State.Velocity.Length());
                    })
                .ToArray();

        double NorthDeviation =
            Report.Result
                .ImpactState
                .Position
                .Y -
            InitialState.Position.Y;

        SimulationSummary Summary =
            new(
                FlightTime:
                    Report.Result.FlightTime,

                Range:
                    Report.Result.Range,

                MaximumAltitude:
                    Report.Result.MaximumAltitude,

                ImpactSpeed:
                    Report.Result.ImpactSpeed,

                NorthDeviation:
                    NorthDeviation,

                MaximumMachNumber:
                    Report.AerodynamicSummary
                        .MaximumMachNumber,

                StateCount:
                    States.Count,

                SolverName:
                    SolverName);

        return new SimulationRunResult(
            Samples,
            Summary);
    }
}
