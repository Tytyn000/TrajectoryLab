using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Simulation;
using TrajectoryLab.Core.Solvers;
using Xunit;

namespace TrajectoryLab.Tests;

public sealed class TrajectorySimulatorTests
{
    [Fact]
    public void Drag_ReducesProjectileRange()
    {
        SimulationParameters Parameters = new()
        {
            InitialPosition = Vector3D.Zero,

            InitialVelocity =
                LaunchVelocity.FromSpeedAndAngles(
                    50.0,
                    45.0
                ),

            Mass = 1.0,
            DragCoefficient = 0.47,
            CrossSectionalArea = 0.01,
            AirDensity = 1.225,
            TimeStep = 0.01,
            MaximumDuration = 60.0
        };

        TrajectorySimulator Simulator = new();
        RungeKutta4Solver Solver = new();

        SimulationResult VacuumResult = Simulator.Simulate(
            Parameters,
            Solver,
            new CompositeAccelerationModel(
                new ConstantGravityModel()
            )
        );

        SimulationResult DragResult = Simulator.Simulate(
            Parameters,
            Solver,
            new CompositeAccelerationModel(
                new ConstantGravityModel(),
                new DragAccelerationModel()
            )
        );

        Assert.True(
            DragResult.Range < VacuumResult.Range
        );
    }
}