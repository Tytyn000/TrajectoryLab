using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Solvers;

namespace TrajectoryLab.Core.Simulation;

/// <summary>
/// Exécute une simulation jusqu'à l'impact avec le sol.
/// </summary>
public sealed class TrajectorySimulator
{
    public SimulationResult Simulate(
        SimulationParameters Parameters,
        INumericalSolver Solver,
        IAccelerationModel AccelerationModel)
    {
        ValidateParameters(Parameters);

        List<SimulationState> States = [];

        SimulationState CurrentState = new(
            Time: 0.0,
            Position: Parameters.InitialPosition,
            Velocity: Parameters.InitialVelocity
        );

        States.Add(CurrentState);

        while (CurrentState.Time < Parameters.MaximumDuration)
        {
            SimulationState PreviousState = CurrentState;

            CurrentState = Solver.Step(
                CurrentState,
                Parameters.TimeStep,
                AccelerationModel,
                Parameters
            );

            if (HasHitGround(
                PreviousState,
                CurrentState,
                Parameters.GroundAltitude))
            {
                SimulationState ImpactState =
                    InterpolateGroundImpact(
                        PreviousState,
                        CurrentState,
                        Parameters.GroundAltitude
                    );

                States.Add(ImpactState);

                return new SimulationResult
                {
                    States = States,
                    ImpactState = ImpactState
                };
            }

            States.Add(CurrentState);
        }

        throw new InvalidOperationException(
            "La simulation a dépassé sa durée maximale."
        );
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
            PreviousState.Position.Z - GroundAltitude;

        double CurrentHeight =
            CurrentState.Position.Z - GroundAltitude;

        double Denominator =
            PreviousHeight - CurrentHeight;

        double Ratio = Denominator == 0.0
            ? 0.0
            : PreviousHeight / Denominator;

        Ratio = Math.Clamp(Ratio, 0.0, 1.0);

        return new SimulationState(
            PreviousState.Time +
            Ratio * (
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
            ) * Ratio
        );
    }

    private static void ValidateParameters(
        SimulationParameters Parameters)
    {
        if (Parameters.TimeStep <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Parameters.TimeStep),
                "Le pas de temps doit être strictement positif."
            );
        }

        if (Parameters.MaximumDuration <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Parameters.MaximumDuration),
                "La durée maximale doit être strictement positive."
            );
        }

        if (
            Parameters.InitialPosition.Z <
            Parameters.GroundAltitude)
        {
            throw new ArgumentException(
                "La position initiale est sous le niveau du sol.",
                nameof(Parameters.InitialPosition)
            );
        }
    }
}