using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Solvers;

namespace TrajectoryLab.Core.Simulation;

/// Regroupe tous les éléments scientifiques nécessaires à une simulation.
public sealed class SimulationScenario
{
    public SimulationParameters Parameters { get; }

    public INumericalSolver Solver { get; }

    public IAccelerationModel AccelerationModel { get; }

    public IdealGasModel GasModel { get; }

    public SimulationScenario(
        SimulationParameters Parameters,
        INumericalSolver Solver,
        IAccelerationModel AccelerationModel)
        : this(
            Parameters,
            Solver,
            AccelerationModel,
            new IdealGasModel())
    {
    }

    public SimulationScenario(
        SimulationParameters Parameters,
        INumericalSolver Solver,
        IAccelerationModel AccelerationModel,
        IdealGasModel GasModel)
    {
        ArgumentNullException.ThrowIfNull(Parameters);
        ArgumentNullException.ThrowIfNull(Solver);
        ArgumentNullException.ThrowIfNull(AccelerationModel);
        ArgumentNullException.ThrowIfNull(GasModel);

        this.Parameters = Parameters;
        this.Solver = Solver;
        this.AccelerationModel = AccelerationModel;
        this.GasModel = GasModel;
    }
}
