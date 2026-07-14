using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Core.Physics;

namespace TrajectoryLab.Application.Simulation.Factories;

public interface IDragCoefficientModelFactory
{
    IDragCoefficientModel Create(
        ProjectileInput Projectile,
        DragInput Drag);
}