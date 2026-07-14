using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Core.Physics;

namespace TrajectoryLab.Application.Simulation.Factories;

public sealed class DragCoefficientModelFactory :
    IDragCoefficientModelFactory
{
    public IDragCoefficientModel Create(
        ProjectileInput Projectile,
        DragInput Drag)
    {
        ArgumentNullException.ThrowIfNull(Projectile);
        ArgumentNullException.ThrowIfNull(Drag);

        return Drag.ModelKind switch
        {
            DragCoefficientModelKind.Constant =>
                new ConstantDragCoefficientModel(
                    Projectile.ReferenceDragCoefficient),

            DragCoefficientModelKind.Tabulated =>
                new TabulatedDragCoefficientModel(
                    Drag.Points.Select(
                        Point => new DragCoefficientPoint(
                            Point.MachNumber,
                            Point.DragCoefficient))),

            _ => throw new ArgumentOutOfRangeException(
                nameof(Drag),
                Drag.ModelKind,
                "Le modÃ¨le de coefficient de traÃ®nÃ©e sÃ©lectionnÃ© est inconnu.")
        };
    }
}