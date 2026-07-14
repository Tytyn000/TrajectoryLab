using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;

namespace TrajectoryLab.Core.Simulation;

/// Fournit une représentation continue d'une trajectoire discrète.
public sealed class ContinuousTrajectory
{
    public IReadOnlyList<SimulationState> States { get; }

    public IReadOnlyList<TrajectoryEquationSegment>
        EquationSegments { get; }

    public SimulationState StartState =>
        States[0];

    public SimulationState EndState =>
        States[^1];

    public double StartTime =>
        StartState.Time;

    public double EndTime =>
        EndState.Time;

    public ContinuousTrajectory(
        IReadOnlyList<SimulationState> States)
    {
        ArgumentNullException.ThrowIfNull(States);

        if (States.Count < 2)
        {
            throw new ArgumentException(
                "Une trajectoire continue nécessite au moins deux états.",
                nameof(States));
        }

        SimulationState[] StateCopy =
            States.ToArray();

        ValidateStates(
            StateCopy);

        TrajectoryEquationSegment[] Segments =
            new TrajectoryEquationSegment[
                StateCopy.Length - 1];

        for (
            int Index = 0;
            Index < Segments.Length;
            Index++)
        {
            Segments[Index] =
                CreateSegment(
                    Index,
                    StateCopy[Index],
                    StateCopy[Index + 1]);
        }

        this.States =
            Array.AsReadOnly(
                StateCopy);

        EquationSegments =
            Array.AsReadOnly(
                Segments);
    }

    public SimulationState Evaluate(
        double Time)
    {
        if (!double.IsFinite(Time))
        {
            throw new ArgumentOutOfRangeException(
                nameof(Time),
                "Le temps doit être fini.");
        }

        if (Time < StartTime || Time > EndTime)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Time),
                "Le temps doit appartenir au domaine de la trajectoire.");
        }

        if (Time == StartTime)
        {
            return StartState;
        }

        if (Time == EndTime)
        {
            return EndState;
        }

        int LowerIndex = 0;
        int UpperIndex =
            EquationSegments.Count - 1;

        while (LowerIndex <= UpperIndex)
        {
            int MiddleIndex =
                LowerIndex +
                (UpperIndex - LowerIndex) / 2;

            TrajectoryEquationSegment Segment =
                EquationSegments[MiddleIndex];

            if (Time < Segment.StartTime)
            {
                UpperIndex =
                    MiddleIndex - 1;
            }
            else if (Time > Segment.EndTime)
            {
                LowerIndex =
                    MiddleIndex + 1;
            }
            else
            {
                return Segment.Evaluate(
                    Time);
            }
        }

        throw new InvalidOperationException(
            "Aucun segment ne contient le temps demandé.");
    }

    public IReadOnlyList<SimulationState> Sample(
        int SampleCount)
    {
        if (SampleCount < 2)
        {
            throw new ArgumentOutOfRangeException(
                nameof(SampleCount),
                "Le nombre d'échantillons doit être au moins égal à deux.");
        }

        SimulationState[] Samples =
            new SimulationState[SampleCount];

        double Duration =
            EndTime - StartTime;

        for (
            int Index = 0;
            Index < SampleCount;
            Index++)
        {
            double Ratio =
                (double)Index /
                (SampleCount - 1);

            double Time =
                StartTime +
                Duration * Ratio;

            Samples[Index] =
                Evaluate(Time);
        }

        return Array.AsReadOnly(
            Samples);
    }

    private static TrajectoryEquationSegment
        CreateSegment(
            int Index,
            SimulationState StartState,
            SimulationState EndState)
    {
        double Duration =
            EndState.Time -
            StartState.Time;

        double DurationSquared =
            Duration * Duration;

        double DurationCubed =
            DurationSquared * Duration;

        Vector3D CubicCoefficient =
            (
                2.0 *
                (
                    StartState.Position -
                    EndState.Position
                ) +
                Duration *
                (
                    StartState.Velocity +
                    EndState.Velocity
                )
            ) *
            (1.0 / DurationCubed);

        Vector3D QuadraticCoefficient =
            (
                3.0 *
                (
                    EndState.Position -
                    StartState.Position
                ) -
                Duration *
                (
                    2.0 *
                    StartState.Velocity +
                    EndState.Velocity
                )
            ) *
            (1.0 / DurationSquared);

        return new TrajectoryEquationSegment(
            Index,
            StartState.Time,
            EndState.Time,
            CubicCoefficient,
            QuadraticCoefficient,
            StartState.Velocity,
            StartState.Position);
    }

    private static void ValidateStates(
        IReadOnlyList<SimulationState> States)
    {
        for (
            int Index = 0;
            Index < States.Count;
            Index++)
        {
            SimulationState State =
                States[Index];

            if (
                !double.IsFinite(State.Time) ||
                !IsFinite(State.Position) ||
                !IsFinite(State.Velocity))
            {
                throw new ArgumentException(
                    "Tous les états doivent contenir des valeurs finies.",
                    nameof(States));
            }

            if (
                Index > 0 &&
                State.Time <=
                States[Index - 1].Time)
            {
                throw new ArgumentException(
                    "Les temps des états doivent être strictement croissants.",
                    nameof(States));
            }
        }
    }

    private static bool IsFinite(
        Vector3D Vector)
    {
        return
            double.IsFinite(Vector.X) &&
            double.IsFinite(Vector.Y) &&
            double.IsFinite(Vector.Z);
    }
}
