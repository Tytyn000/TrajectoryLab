using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Core.Physics;

namespace TrajectoryLab.Application.Simulation.Factories;

public interface IGravityModelFactory
{
    IAccelerationModel Create(
        CelestialBodyInput Input);

    double GetSurfaceGravityAcceleration(
        CelestialBodyInput Input);
}
