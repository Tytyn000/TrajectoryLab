using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Simulation;
using TrajectoryLab.Core.Wind;
using Xunit;

namespace TrajectoryLab.Tests;

public sealed class TrajectorySeriesBuilderTests
{
    [Fact]
    public void CatalogContainsAllExpectedQuantities()
    {
        Assert.Equal(
            9,
            TrajectoryQuantityCatalog.All.Count);

        Assert.Equal(
            TrajectoryQuantityCatalog.All.Count,
            TrajectoryQuantityCatalog.All
                .Select(
                    Information =>
                        Information.Quantity)
                .Distinct()
                .Count());
    }

    [Fact]
    public void BuildReturnsRequestedAxesAndSampleCount()
    {
        ContinuousTrajectory Trajectory =
            new(
                new[]
                {
                    new SimulationState(
                        Time: 0.0,
                        Position:
                            Vector3D.Zero,
                        Velocity:
                            new Vector3D(
                                1.0,
                                0.0,
                                1.0)),

                    new SimulationState(
                        Time: 1.0,
                        Position:
                            new Vector3D(
                                1.0,
                                0.0,
                                1.0),
                        Velocity:
                            new Vector3D(
                                1.0,
                                0.0,
                                1.0))
                });

        SimulationParameters Parameters =
            new(
                InitialPosition:
                    Vector3D.Zero,
                InitialVelocity:
                    new Vector3D(
                        1.0,
                        0.0,
                        1.0),
                Projectile:
                    new ProjectileParameters(
                        Mass: 1.0,
                        DragCoefficient: 0.0,
                        CrossSectionalArea: 0.0),
                Environment:
                    new EnvironmentParameters(
                        GravityAcceleration:
                            9.80665,
                        AtmosphereModel:
                            new ConstantAtmosphereModel(
                                AirDensity: 0.0),
                        WindModel:
                            new ConstantWindModel(
                                Vector3D.Zero)),
                Settings:
                    new SimulationSettings(
                        TimeStep: 0.1,
                        MaximumSimulationTime: 2.0));

        TrajectorySeries Series =
            new TrajectorySeriesBuilder()
                .Build(
                    Trajectory,
                    Parameters,
                    new IdealGasModel(),
                    TrajectoryQuantity.Time,
                    TrajectoryQuantity.Altitude,
                    SampleCount: 3);

        Assert.Equal(
            3,
            Series.HorizontalValues.Length);

        Assert.Equal(
            3,
            Series.VerticalValues.Length);

        Assert.Equal(
            0.0,
            Series.HorizontalValues[0],
            12);

        Assert.Equal(
            0.5,
            Series.HorizontalValues[1],
            12);

        Assert.Equal(
            1.0,
            Series.HorizontalValues[2],
            12);

        Assert.Equal(
            0.5,
            Series.VerticalValues[1],
            12);

        Assert.Equal(
            TrajectoryQuantity.Time,
            Series.HorizontalQuantity.Quantity);

        Assert.Equal(
            TrajectoryQuantity.Altitude,
            Series.VerticalQuantity.Quantity);
    }
}
