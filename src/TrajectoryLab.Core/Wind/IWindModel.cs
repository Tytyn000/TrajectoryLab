using TrajectoryLab.Core.Mathematics;

namespace TrajectoryLab.Core.Wind;

// Donne la vitesse locale de l'air par rapport au sol.
public interface IWindModel
{
    Vector3D GetWindVelocity(
        Vector3D Position,
        double Time
    );
}