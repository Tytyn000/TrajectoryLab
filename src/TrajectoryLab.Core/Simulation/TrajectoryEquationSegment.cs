using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;

namespace TrajectoryLab.Core.Simulation;

/// Représente l'équation cubique locale d'un segment de trajectoire.
public readonly record struct TrajectoryEquationSegment(
    int Index,
    double StartTime,
    double EndTime,
    Vector3D CubicCoefficient,
    Vector3D QuadraticCoefficient,
    Vector3D LinearCoefficient,
    Vector3D ConstantCoefficient)
{
    public double Duration =>
        EndTime - StartTime;

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
                "Le temps doit appartenir au segment.");
        }

        double LocalTime =
            Time - StartTime;

        double LocalTimeSquared =
            LocalTime * LocalTime;

        double LocalTimeCubed =
            LocalTimeSquared * LocalTime;

        Vector3D Position =
            ConstantCoefficient +
            LinearCoefficient * LocalTime +
            QuadraticCoefficient * LocalTimeSquared +
            CubicCoefficient * LocalTimeCubed;

        Vector3D Velocity =
            LinearCoefficient +
            QuadraticCoefficient *
            (2.0 * LocalTime) +
            CubicCoefficient *
            (3.0 * LocalTimeSquared);

        return new SimulationState(
            Time,
            Position,
            Velocity);
    }
}
