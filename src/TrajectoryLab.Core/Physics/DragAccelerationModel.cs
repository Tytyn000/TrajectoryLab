using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;

namespace TrajectoryLab.Core.Physics;

/// Calcule l'accélération produite par la traînée aérodynamique quadratique.
public sealed class DragAccelerationModel : IAccelerationModel
{
    public Vector3D GetAcceleration(
        SimulationState State,
        SimulationParameters Parameters)
    {
        Vector3D RelativeVelocity =
            State.Velocity - Parameters.WindVelocity;

        double RelativeSpeed =
            RelativeVelocity.Length();

        if (RelativeSpeed <= 1e-12)
        {
            return Vector3D.Zero;
        }

        double Factor =
            -0.5
            * Parameters.AirDensity
            * Parameters.DragCoefficient
            * Parameters.CrossSectionalArea
            / Parameters.Mass;

        return RelativeVelocity
            * (Factor * RelativeSpeed);
    }
}