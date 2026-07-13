using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;

namespace TrajectoryLab.Core.Solvers;

/// <summary>
/// Méthode de Runge-Kutta d'ordre 4.
/// </summary>
public sealed class RungeKutta4Solver : INumericalSolver
{
    public string Name => "Runge-Kutta 4";

    public SimulationState Step(
        SimulationState State,
        double TimeStep,
        IAccelerationModel AccelerationModel,
        SimulationParameters Parameters)
    {
        StateDerivative K1 = Evaluate(
            State,
            AccelerationModel,
            Parameters
        );

        SimulationState K2State = ApplyDerivative(
            State,
            K1,
            TimeStep / 2.0
        );

        StateDerivative K2 = Evaluate(
            K2State,
            AccelerationModel,
            Parameters
        );

        SimulationState K3State = ApplyDerivative(
            State,
            K2,
            TimeStep / 2.0
        );

        StateDerivative K3 = Evaluate(
            K3State,
            AccelerationModel,
            Parameters
        );

        SimulationState K4State = ApplyDerivative(
            State,
            K3,
            TimeStep
        );

        StateDerivative K4 = Evaluate(
            K4State,
            AccelerationModel,
            Parameters
        );

        Vector3D PositionIncrement =
            (
                K1.PositionDerivative +
                2.0 * K2.PositionDerivative +
                2.0 * K3.PositionDerivative +
                K4.PositionDerivative
            ) * (TimeStep / 6.0);

        Vector3D VelocityIncrement =
            (
                K1.VelocityDerivative +
                2.0 * K2.VelocityDerivative +
                2.0 * K3.VelocityDerivative +
                K4.VelocityDerivative
            ) * (TimeStep / 6.0);

        return new SimulationState(
            State.Time + TimeStep,
            State.Position + PositionIncrement,
            State.Velocity + VelocityIncrement
        );
    }

    private static StateDerivative Evaluate(
        SimulationState State,
        IAccelerationModel AccelerationModel,
        SimulationParameters Parameters)
    {
        return new StateDerivative(
            State.Velocity,
            AccelerationModel.GetAcceleration(
                State,
                Parameters
            )
        );
    }

    private static SimulationState ApplyDerivative(
        SimulationState State,
        StateDerivative Derivative,
        double DeltaTime)
    {
        return new SimulationState(
            State.Time + DeltaTime,

            State.Position +
            Derivative.PositionDerivative * DeltaTime,

            State.Velocity +
            Derivative.VelocityDerivative * DeltaTime
        );
    }
}