using TrajectoryLab.Core.Mathematics;

using TrajectoryLab.Core.Models;

namespace TrajectoryLab.Core.Physics;

/// Calcule l'accélération appliquée au projectile.
public interface IAccelerationModel
{
    Vector3D GetAcceleration(
        SimulationState State,
        SimulationParameters Parameters
    );
}