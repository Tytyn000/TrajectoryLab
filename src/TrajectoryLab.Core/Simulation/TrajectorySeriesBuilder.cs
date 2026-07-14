using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;

namespace TrajectoryLab.Core.Simulation;

/// Construit une série graphique à partir d'une trajectoire continue.
public sealed class TrajectorySeriesBuilder
{
    public TrajectorySeries Build(
        ContinuousTrajectory Trajectory,
        SimulationParameters Parameters,
        IdealGasModel GasModel,
        TrajectoryQuantity HorizontalQuantity,
        TrajectoryQuantity VerticalQuantity,
        int SampleCount)
    {
        ArgumentNullException.ThrowIfNull(Trajectory);
        ArgumentNullException.ThrowIfNull(Parameters);
        ArgumentNullException.ThrowIfNull(GasModel);

        if (SampleCount < 2)
        {
            throw new ArgumentOutOfRangeException(
                nameof(SampleCount),
                "Le nombre d'échantillons doit être au moins égal à deux.");
        }

        IReadOnlyList<SimulationState> Samples =
            Trajectory.Sample(
                SampleCount);

        double[] HorizontalValues =
            new double[Samples.Count];

        double[] VerticalValues =
            new double[Samples.Count];

        Vector3D InitialPosition =
            Trajectory.StartState.Position;

        FlightConditionCalculator Calculator =
            new(GasModel);

        for (
            int Index = 0;
            Index < Samples.Count;
            Index++)
        {
            SimulationState State =
                Samples[Index];

            FlightCondition Condition =
                Calculator.Calculate(
                    State,
                    Parameters);

            HorizontalValues[Index] =
                GetValue(
                    HorizontalQuantity,
                    State,
                    Condition,
                    InitialPosition);

            VerticalValues[Index] =
                GetValue(
                    VerticalQuantity,
                    State,
                    Condition,
                    InitialPosition);
        }

        return new TrajectorySeries(
            HorizontalValues,
            VerticalValues,
            TrajectoryQuantityCatalog.Get(
                HorizontalQuantity),
            TrajectoryQuantityCatalog.Get(
                VerticalQuantity));
    }

    private static double GetValue(
        TrajectoryQuantity Quantity,
        SimulationState State,
        FlightCondition Condition,
        Vector3D InitialPosition)
    {
        return Quantity switch
        {
            TrajectoryQuantity.Time =>
                State.Time,

            TrajectoryQuantity.PositionX =>
                State.Position.X,

            TrajectoryQuantity.PositionY =>
                State.Position.Y,

            TrajectoryQuantity.Altitude =>
                State.Position.Z,

            TrajectoryQuantity.HorizontalDistance =>
                GetHorizontalDistance(
                    InitialPosition,
                    State.Position),

            TrajectoryQuantity.Speed =>
                State.Velocity.Length(),

            TrajectoryQuantity.RelativeSpeed =>
                Condition.RelativeSpeed,

            TrajectoryQuantity.MachNumber =>
                Condition.MachNumber,

            TrajectoryQuantity.Temperature =>
                Condition.Temperature,

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(Quantity),
                    "La grandeur de trajectoire est inconnue.")
        };
    }

    private static double GetHorizontalDistance(
        Vector3D InitialPosition,
        Vector3D Position)
    {
        double DeltaX =
            Position.X -
            InitialPosition.X;

        double DeltaY =
            Position.Y -
            InitialPosition.Y;

        return Math.Sqrt(
            DeltaX * DeltaX +
            DeltaY * DeltaY);
    }
}
