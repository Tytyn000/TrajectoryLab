using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Application.Simulation.Factories;

public interface IWindModelFactory
{
    IWindModel Create(WindInput Input);
}