using TrajectoryLab.Application.Models;
using TrajectoryLab.Core.Simulation;

namespace TrajectoryLab.Application.Simulation;

public interface ISimulationScenarioFactory
{
    SimulationScenario Create(
        SimulationInput Input);
}
