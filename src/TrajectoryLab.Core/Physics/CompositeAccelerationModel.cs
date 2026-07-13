using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;

namespace TrajectoryLab.Core.Physics;

// Additionne les accélérations produites par plusieurs modèles physiques.
public sealed class CompositeAccelerationModel : IAccelerationModel
{
    private readonly IReadOnlyList<IAccelerationModel> Models;

    public CompositeAccelerationModel(
        params IAccelerationModel[] Models)
    {
        ArgumentNullException.ThrowIfNull(Models);

        this.Models = Models;
    }

    public Vector3D GetAcceleration(
        SimulationState State,
        SimulationParameters Parameters)
    {
        Vector3D TotalAcceleration = Vector3D.Zero;

        foreach (IAccelerationModel Model in Models)
        {
            TotalAcceleration += Model.GetAcceleration(
                State,
                Parameters
            );
        }

        return TotalAcceleration;
    }
}