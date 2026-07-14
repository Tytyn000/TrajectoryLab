using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Simulation;
using TrajectoryLab.Core.Solvers;
using TrajectoryLab.Core.Wind;
using Xunit;

namespace TrajectoryLab.Tests;

public sealed class WindTrajectoryIntegrationTests
{
    [Fact]
    public void TailwindIncreasesRange()
    {
        SimulationResult CalmResult =
            RunSimulation(
                WindVelocity: Vector3D.Zero
            );

        SimulationResult TailwindResult =
            RunSimulation(
                WindVelocity:
                    new Vector3D(
                        X: 10.0,
                        Y: 0.0,
                        Z: 0.0
                    )
            );

        Assert.True(
            TailwindResult.Range >
            CalmResult.Range
        );
    }

    [Fact]
    public void HeadwindReducesRange()
    {
        SimulationResult CalmResult =
            RunSimulation(
                WindVelocity: Vector3D.Zero
            );

        SimulationResult HeadwindResult =
            RunSimulation(
                WindVelocity:
                    new Vector3D(
                        X: -10.0,
                        Y: 0.0,
                        Z: 0.0
                    )
            );

        Assert.True(
            HeadwindResult.Range <
            CalmResult.Range
        );
    }

    [Fact]
    public void CrosswindProducesLateralDisplacement()
    {
        SimulationResult Result =
            RunSimulation(
                WindVelocity:
                    new Vector3D(
                        X: 0.0,
                        Y: 10.0,
                        Z: 0.0
                    )
            );

        // Un vent vers le nord produit
        // un déplacement final positif sur l'axe Y.
        Assert.True(
            Result.ImpactState.Position.Y >
            0.0
        );
    }

    private static SimulationResult RunSimulation(
        Vector3D WindVelocity)
    {
        SimulationParameters Parameters =
            new(
                InitialPosition:
                    Vector3D.Zero,
                InitialVelocity:
                    LaunchVelocity.FromSpeedAndAngles(
                        Speed: 50.0,
                        ElevationDegrees: 45.0,
                        AzimuthDegrees: 0.0
                    ),
                Projectile:
                    new ProjectileParameters(
                        Mass: 1.0,
                        DragCoefficient: 0.47,
                        CrossSectionalArea: 0.01
                    ),
                Environment:
                    new EnvironmentParameters(
                        GravityAcceleration: 9.80665,
                        AtmosphereModel:
                            new StandardAtmosphere1976Model(),
                        WindModel:
                            new ConstantWindModel(
                                WindVelocity
                            )
                    ),
                Settings:
                    new SimulationSettings(
                        TimeStep: 0.01,
                        MaximumSimulationTime: 60.0,
                        GroundAltitude: 0.0
                    )
            );

        CompositeAccelerationModel AccelerationModel =
            new(
                new ConstantGravityModel(),
                new DragAccelerationModel()
            );

        return new TrajectorySimulator()
            .Simulate(
                Parameters,
                new RungeKutta4Solver(),
                AccelerationModel
            );
    }
}