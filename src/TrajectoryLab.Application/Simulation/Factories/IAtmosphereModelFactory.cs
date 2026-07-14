using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;

namespace TrajectoryLab.Application.Simulation.Factories;

public interface IAtmosphereModelFactory
{
    IAtmosphereModel Create(
        AtmosphereInput Input,
        IdealGasModel GasModel);
}
