using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using Xunit;

namespace TrajectoryLab.Tests;

public sealed class ParameterTests
{
    [Fact]
    public void ProjectileParametersStoreValidValues()
    {
        ProjectileParameters Parameters = new(
            Mass: 2.0,
            DragCoefficient: 0.47,
            CrossSectionalArea: 0.01
        );

        Assert.Equal(2.0, Parameters.Mass);
        Assert.Equal(0.47, Parameters.DragCoefficient);
        Assert.Equal(0.01, Parameters.CrossSectionalArea);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void ProjectileParametersRejectInvalidMass(double Mass)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ProjectileParameters(
                Mass: Mass,
                DragCoefficient: 0.47,
                CrossSectionalArea: 0.01
            )
        );
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void ProjectileParametersRejectInvalidDragCoefficient(
        double DragCoefficient)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ProjectileParameters(
                Mass: 2.0,
                DragCoefficient: DragCoefficient,
                CrossSectionalArea: 0.01
            )
        );
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void ProjectileParametersRejectInvalidCrossSectionalArea(
        double CrossSectionalArea)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ProjectileParameters(
                Mass: 2.0,
                DragCoefficient: 0.47,
                CrossSectionalArea: CrossSectionalArea
            )
        );
    }

    [Fact]
    public void EnvironmentParametersStoreValidValues()
    {
        ConstantAtmosphereModel AtmosphereModel =
            new(
                AirDensity: 1.225
            );

        Vector3D WindVelocity =
            new(
                X: 5.0,
                Y: -2.0,
                Z: 0.5
            );

        EnvironmentParameters Parameters =
            new(
                GravityAcceleration: 9.80665,
                AtmosphereModel: AtmosphereModel,
                WindVelocity: WindVelocity
            );

        Vector3D ActualWindVelocity =
            Parameters.WindModel.GetWindVelocity(
                Position: Vector3D.Zero,
                Time: 0.0
            );

        Assert.Equal(
            9.80665,
            Parameters.GravityAcceleration
        );

        Assert.Same(
            AtmosphereModel,
            Parameters.AtmosphereModel
        );

        Assert.Equal(
            1.225,
            Parameters.AtmosphereModel
                .GetAirDensity(
                    Altitude: 0.0
                )
        );

        Assert.Equal(
            5.0,
            ActualWindVelocity.X
        );

        Assert.Equal(
            -2.0,
            ActualWindVelocity.Y
        );

        Assert.Equal(
            0.5,
            ActualWindVelocity.Z
        );
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-9.80665)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void EnvironmentParametersRejectInvalidGravity(
        double GravityAcceleration)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new EnvironmentParameters(
                GravityAcceleration: GravityAcceleration,
                AtmosphereModel: new ConstantAtmosphereModel(
                    AirDensity: 1.225
                ),
                WindVelocity: Vector3D.Zero
            )
        );
    }

    [Fact]
    public void EnvironmentParametersRejectNullAtmosphereModel()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new EnvironmentParameters(
                GravityAcceleration: 9.80665,
                AtmosphereModel: null!,
                WindVelocity: Vector3D.Zero
            )
        );
    }

    [Fact]
    public void SimulationSettingsStoreValidValues()
    {
        SimulationSettings Settings = new(
            TimeStep: 0.01,
            MaximumSimulationTime: 60.0,
            GroundAltitude: 100.0
        );

        Assert.Equal(0.01, Settings.TimeStep);
        Assert.Equal(60.0, Settings.MaximumSimulationTime);
        Assert.Equal(100.0, Settings.GroundAltitude);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-0.01)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void SimulationSettingsRejectInvalidTimeStep(
        double TimeStep)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SimulationSettings(
                TimeStep: TimeStep,
                MaximumSimulationTime: 60.0
            )
        );
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-60.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void SimulationSettingsRejectInvalidMaximumTime(
        double MaximumSimulationTime)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SimulationSettings(
                TimeStep: 0.01,
                MaximumSimulationTime:
                    MaximumSimulationTime
            )
        );
    }

    [Fact]
    public void SimulationSettingsRejectTimeStepGreaterThanMaximumTime()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SimulationSettings(
                TimeStep: 10.0,
                MaximumSimulationTime: 5.0
            )
        );
    }
}