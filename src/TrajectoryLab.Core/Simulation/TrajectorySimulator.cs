using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Solvers;

namespace TrajectoryLab.Core.Simulation;

// Exécute une simulation jusqu'à l'impact avec le sol.
public sealed class TrajectorySimulator
{
    public SimulationResult Simulate(
        SimulationParameters Parameters,
        INumericalSolver Solver,
        IAccelerationModel AccelerationModel)
    {
        return Simulate(
            Parameters,
            Solver,
            AccelerationModel,
            CancellationToken.None,
            Progress: null);
    }

    public SimulationResult Simulate(
        SimulationParameters Parameters,
        INumericalSolver Solver,
        IAccelerationModel AccelerationModel,
        CancellationToken CancellationToken,
        IProgress<double>? Progress = null)
    {
        ArgumentNullException.ThrowIfNull(Parameters);
        ArgumentNullException.ThrowIfNull(Solver);
        ArgumentNullException.ThrowIfNull(AccelerationModel);

        CancellationToken.ThrowIfCancellationRequested();

        ValidateParameters(Parameters);

        List<SimulationState> States = [];

        SimulationState CurrentState = new(
            Time: 0.0,
            Position: Parameters.InitialPosition,
            Velocity: Parameters.InitialVelocity);

        States.Add(CurrentState);

        Progress?.Report(0.0);

        double CurrentTimeStep =
            Parameters.Settings.TimeStep;

        while (
            CurrentState.Time <
            Parameters.Settings.MaximumSimulationTime)
        {
            CancellationToken.ThrowIfCancellationRequested();

            SimulationState PreviousState =
                CurrentState;

            if (
                Solver is
                IAdaptiveNumericalSolver AdaptiveSolver)
            {
                AdaptiveStepResult StepResult =
                    AdaptiveSolver.StepAdaptive(
                        CurrentState,
                        CurrentTimeStep,
                        AccelerationModel,
                        Parameters);

                CurrentTimeStep =
                    StepResult.SuggestedTimeStep;

                if (!StepResult.IsAccepted)
                {
                    continue;
                }

                CurrentState =
                    StepResult.State;
            }
            else
            {
                CurrentState = Solver.Step(
                    CurrentState,
                    Parameters.Settings.TimeStep,
                    AccelerationModel,
                    Parameters);
            }

            if (HasHitGround(
                PreviousState,
                CurrentState,
                Parameters.Settings.GroundAltitude))
            {
                SimulationState ImpactState =
                    InterpolateGroundImpact(
                        PreviousState,
                        CurrentState,
                        Parameters.Settings.GroundAltitude);

                States.Add(ImpactState);

                Progress?.Report(1.0);

                return new SimulationResult
                {
                    States = States,
                    ImpactState = ImpactState
                };
            }

            States.Add(CurrentState);

            ReportProgress(
                CurrentState.Time,
                Parameters.Settings.MaximumSimulationTime,
                Progress);
        }

        throw new InvalidOperationException(
            "La simulation a dépassé sa durée maximale.");
    }

    private static void ReportProgress(
        double CurrentTime,
        double MaximumSimulationTime,
        IProgress<double>? Progress)
    {
        if (Progress is null)
        {
            return;
        }

        double ProgressValue = Math.Clamp(
            CurrentTime / MaximumSimulationTime,
            0.0,
            1.0);

        Progress.Report(ProgressValue);
    }

    private static bool HasHitGround(
        SimulationState PreviousState,
        SimulationState CurrentState,
        double GroundAltitude)
    {
        return
            PreviousState.Position.Z >= GroundAltitude &&
            CurrentState.Position.Z < GroundAltitude;
    }

    private static SimulationState InterpolateGroundImpact(
        SimulationState PreviousState,
        SimulationState CurrentState,
        double GroundAltitude)
    {
        double PreviousHeight =
            PreviousState.Position.Z -
            GroundAltitude;

        double CurrentHeight =
            CurrentState.Position.Z -
            GroundAltitude;

        double Denominator =
            PreviousHeight -
            CurrentHeight;

        double Ratio = Denominator == 0.0
            ? 0.0
            : PreviousHeight /
              Denominator;

        Ratio = Math.Clamp(
            Ratio,
            0.0,
            1.0);

        return new SimulationState(
            PreviousState.Time +
            Ratio *
            (
                CurrentState.Time -
                PreviousState.Time
            ),
            PreviousState.Position +
            (
                CurrentState.Position -
                PreviousState.Position
            ) * Ratio,
            PreviousState.Velocity +
            (
                CurrentState.Velocity -
                PreviousState.Velocity
            ) * Ratio);
    }

    private static void ValidateParameters(
        SimulationParameters Parameters)
    {
        if (
            Parameters.InitialPosition.Z <
            Parameters.Settings.GroundAltitude)
        {
            throw new ArgumentException(
                "La position initiale est sous le niveau du sol.",
                nameof(Parameters.InitialPosition));
        }
    }
}
