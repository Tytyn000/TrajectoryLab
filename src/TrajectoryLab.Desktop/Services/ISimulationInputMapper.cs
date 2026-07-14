using TrajectoryLab.Application.Models;
using TrajectoryLab.Desktop.ViewModels;

namespace TrajectoryLab.Desktop.Services;

public interface ISimulationInputMapper
{
    SimulationInput Map(
        SimulationParametersViewModel Source);
}