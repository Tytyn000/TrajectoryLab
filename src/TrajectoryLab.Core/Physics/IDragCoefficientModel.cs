namespace TrajectoryLab.Core.Physics;

// Fournit le coefficient de traînée correspondant
// au nombre de Mach local du projectile.
public interface IDragCoefficientModel
{
    double GetDragCoefficient(
        double MachNumber);
}