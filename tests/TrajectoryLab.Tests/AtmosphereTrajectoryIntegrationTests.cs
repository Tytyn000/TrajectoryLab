using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Simulation;
using TrajectoryLab.Core.Solvers;
using Xunit;

namespace TrajectoryLab.Tests;

public sealed class AtmosphereTrajectoryIntegrationTests
{
    [Fact]
    public void StandardAtmosphereIncreasesRangeComparedWithConstantDensity()
    {
        SimulationResult ConstantResult =
            RunSimulation(
                new ConstantAtmosphereModel(
                    AirDensity: 1.225
                )
            );

        SimulationResult StandardResult =
            RunSimulation(
                new StandardAtmosphere1976Model()
            );

        // La densité diminue avec l'altitude,
        // donc la traînée moyenne est légèrement plus faible.
        Assert.True(
            StandardResult.Range >
            ConstantResult.Range
        );

        Assert.True(
            StandardResult.MaximumAltitude >
            ConstantResult.MaximumAltitude
        );

        Assert.True(
            StandardResult.ImpactSpeed >
            ConstantResult.ImpactSpeed
        );
    }

    [Fact]
    public void StandardAtmosphereProducesExpectedTrajectory()
    {
        SimulationResult Result =
            RunSimulation(
                new StandardAtmosphere1976Model()
            );

        Assert.InRange(
            Math.Abs(
                Result.FlightTime -
                6.364131
            ),
            0.0,
            1e-5
        );

        Assert.InRange(
            Math.Abs(
                Result.Range -
                167.372224
            ),
            0.0,
            1e-5
        );

        Assert.InRange(
            Math.Abs(
                Result.MaximumAltitude -
                49.898588
            ),
            0.0,
            1e-5
        );

        Assert.InRange(
            Math.Abs(
                Result.ImpactSpeed -
                34.629824
            ),
            0.0,
            1e-5
        );
    }

    private static SimulationResult RunSimulation(
        IAtmosphereModel AtmosphereModel)
    {
        SimulationParameters Parameters =
            new(
                InitialPosition: Vector3D.Zero,
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
                            AtmosphereModel,
                        WindVelocity:
                            Vector3D.Zero
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

        return new TrajectorySimulator().Simulate(
            Parameters,
            new RungeKutta4Solver(),
            AccelerationModel
        );
    }
}