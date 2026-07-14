using TrajectoryLab.Core.Atmosphere;
using Xunit;

namespace TrajectoryLab.Tests;

public sealed class UpperAtmosphere1976TemperatureModelTests
{
    [Theory]
    [InlineData(86_000.0, 186.8673, 1e-10)]
    [InlineData(91_000.0, 186.8673, 1e-10)]
    [InlineData(110_000.0, 240.0, 0.001)]
    [InlineData(120_000.0, 360.0, 1e-10)]
    [InlineData(1_000_000.0, 1_000.0, 0.001)]
    public void ReturnsExpectedTemperature(
        double Altitude,
        double ExpectedTemperature,
        double Tolerance)
    {
        UpperAtmosphere1976TemperatureModel Model =
            new();

        double ActualTemperature =
            Model.GetTemperature(
                Altitude
            );

        Assert.InRange(
            Math.Abs(
                ActualTemperature -
                ExpectedTemperature
            ),
            0.0,
            Tolerance
        );
    }

    [Theory]
    [InlineData(91_000.0)]
    [InlineData(110_000.0)]
    [InlineData(120_000.0)]
    public void TemperatureIsContinuousAtBoundary(
        double BoundaryAltitude)
    {
        const double Offset = 0.001;

        UpperAtmosphere1976TemperatureModel Model =
            new();

        double TemperatureBelow =
            Model.GetTemperature(
                BoundaryAltitude -
                Offset
            );

        double TemperatureAbove =
            Model.GetTemperature(
                BoundaryAltitude +
                Offset
            );

        double Difference =
            Math.Abs(
                TemperatureAbove -
                TemperatureBelow
            );

        Assert.InRange(
            Difference,
            0.0,
            0.001
        );
    }

    [Fact]
    public void TemperatureApproachesExosphericTemperature()
    {
        UpperAtmosphere1976TemperatureModel Model =
            new();

        double TemperatureAt120Kilometers =
            Model.GetTemperature(
                Altitude: 120_000.0
            );

        double TemperatureAt200Kilometers =
            Model.GetTemperature(
                Altitude: 200_000.0
            );

        double TemperatureAt500Kilometers =
            Model.GetTemperature(
                Altitude: 500_000.0
            );

        double TemperatureAt1000Kilometers =
            Model.GetTemperature(
                Altitude: 1_000_000.0
            );

        Assert.True(
            TemperatureAt200Kilometers >
            TemperatureAt120Kilometers
        );

        Assert.True(
            TemperatureAt500Kilometers >
            TemperatureAt200Kilometers
        );

        Assert.True(
            TemperatureAt1000Kilometers >
            TemperatureAt500Kilometers
        );

        Assert.True(
            TemperatureAt1000Kilometers <=
            UpperAtmosphere1976TemperatureModel
                .ExosphericTemperature
        );
    }

    [Theory]
    [InlineData(85_999.9)]
    [InlineData(1_000_000.1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void RejectsUnsupportedAltitude(
        double Altitude)
    {
        UpperAtmosphere1976TemperatureModel Model =
            new();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Model.GetTemperature(
                Altitude
            )
        );
    }
}