using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Wind;
using Xunit;

namespace TrajectoryLab.Tests;

public sealed class WindModelTests
{
    [Fact]
    public void ConstantWindReturnsSameVelocityEverywhere()
    {
        Vector3D ExpectedWindVelocity =
            new(
                X: 10.0,
                Y: -4.0,
                Z: 1.0
            );

        ConstantWindModel WindModel =
            new(
                ExpectedWindVelocity
            );

        Vector3D FirstVelocity =
            WindModel.GetWindVelocity(
                Position: Vector3D.Zero,
                Time: 0.0
            );

        Vector3D SecondVelocity =
            WindModel.GetWindVelocity(
                Position: new Vector3D(
                    X: 5_000.0,
                    Y: -2_000.0,
                    Z: 20_000.0
                ),
                Time: 3_600.0
            );

        Assert.Equal(
            ExpectedWindVelocity,
            FirstVelocity
        );

        Assert.Equal(
            ExpectedWindVelocity,
            SecondVelocity
        );
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void ConstantWindRejectsInvalidVelocity(
        double InvalidComponent)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ConstantWindModel(
                WindVelocity:
                    new Vector3D(
                        X: InvalidComponent,
                        Y: 0.0,
                        Z: 0.0
                    )
            )
        );
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void ConstantWindRejectsInvalidPosition(
        double InvalidComponent)
    {
        ConstantWindModel WindModel =
            new(
                WindVelocity: Vector3D.Zero
            );

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            WindModel.GetWindVelocity(
                Position:
                    new Vector3D(
                        X: InvalidComponent,
                        Y: 0.0,
                        Z: 0.0
                    ),
                Time: 0.0
            )
        );
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void ConstantWindRejectsInvalidTime(
        double Time)
    {
        ConstantWindModel WindModel =
            new(
                WindVelocity: Vector3D.Zero
            );

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            WindModel.GetWindVelocity(
                Position: Vector3D.Zero,
                Time: Time
            )
        );
    }

    [Fact]
    public void EnvironmentParametersStoreWindModel()
    {
        ConstantWindModel WindModel =
            new(
                WindVelocity:
                    new Vector3D(
                        X: 2.0,
                        Y: 3.0,
                        Z: 0.0
                    )
            );

        EnvironmentParameters Parameters =
            new(
                GravityAcceleration: 9.80665,
                AtmosphereModel:
                    new ConstantAtmosphereModel(
                        AirDensity: 1.225
                    ),
                WindModel: WindModel
            );

        Assert.Same(
            WindModel,
            Parameters.WindModel
        );
    }

    [Fact]
    public void VectorConstructorCreatesConstantWindModel()
    {
        Vector3D ExpectedVelocity =
            new(
                X: 5.0,
                Y: 0.0,
                Z: 0.0
            );

        EnvironmentParameters Parameters =
            new(
                GravityAcceleration: 9.80665,
                AtmosphereModel:
                    new ConstantAtmosphereModel(
                        AirDensity: 1.225
                    ),
                WindVelocity: ExpectedVelocity
            );

        Vector3D ActualVelocity =
            Parameters.WindModel.GetWindVelocity(
                Position: Vector3D.Zero,
                Time: 0.0
            );

        Assert.IsType<ConstantWindModel>(
            Parameters.WindModel
        );

        Assert.Equal(
            ExpectedVelocity,
            ActualVelocity
        );
    }
}