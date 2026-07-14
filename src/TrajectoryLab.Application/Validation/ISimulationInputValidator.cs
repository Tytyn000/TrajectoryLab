using TrajectoryLab.Application.Models;

namespace TrajectoryLab.Application.Validation;

public interface ISimulationInputValidator
{
    void Validate(
        SimulationInput Input);
}
