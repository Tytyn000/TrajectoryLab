using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;

namespace TrajectoryLab.Core.Solvers;

// Méthode d'Euler
public sealed class EulerSolver : INumericalSolver
{
    public string Name => "Euler";

    public SimulationState Step(
        SimulationState State,
        double TimeStep,
        IAccelerationModel AccelerationModel,
        SimulationParameters Parameters)
    {
        Vector3D Acceleration =
            AccelerationModel.GetAcceleration(
                State,
                Parameters
            );

        Vector3D NewPosition =
            State.Position +
            State.Velocity * TimeStep;

        Vector3D NewVelocity =
            State.Velocity +
            Acceleration * TimeStep;

        return new SimulationState(
            State.Time + TimeStep,
            NewPosition,
            NewVelocity
        );
    }
}