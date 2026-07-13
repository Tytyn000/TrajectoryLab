using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;

namespace TrajectoryLab.Core.Physics;

// Applique une gravité constante
public sealed class ConstantGravityModel : IAccelerationModel
{
    public Vector3D GetAcceleration(
        SimulationState State,
        SimulationParameters Parameters)
    {
        return Parameters.Gravity;
    }
}