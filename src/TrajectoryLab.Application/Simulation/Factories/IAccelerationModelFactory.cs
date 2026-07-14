using TrajectoryLab.Core;
using TrajectoryLab.Application.Models;
using TrajectoryLab.Core.Physics;

namespace TrajectoryLab.Application.Simulation.Factories;

public interface IAccelerationModelFactory
{
    IAccelerationModel Create(
        SimulationInput Input,
        IdealGasModel GasModel);
}