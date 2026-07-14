using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;

namespace TrajectoryLab.Core.Physics;

// Applique une gravité constante dirigée vers le bas.
public sealed class ConstantGravityModel : IAccelerationModel
{
    public Vector3D GetAcceleration(
        SimulationState State,
        SimulationParameters Parameters)
    {
        // La norme est positive dans les paramètres, mais l'axe Z pointe vers le haut.
        return new Vector3D(
            X: 0.0,
            Y: 0.0,
            Z: -Parameters.Environment.GravityAcceleration
        );
    }
}