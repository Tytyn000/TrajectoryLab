using TrajectoryLab.Core.Atmosphere;
using Xunit;

namespace TrajectoryLab.Tests;

public sealed class AtmosphereModelTests
{
    [Fact]
    public void ConstantAtmosphereReturnsSameDensityAtAllAltitudes()
    {
        ConstantAtmosphereModel AtmosphereModel = new(
            AirDensity: 1.225
        );

        double SeaLevelDensity =
            AtmosphereModel.GetAirDensity(
                Altitude: 0.0
            );

        double HighAltitudeDensity =
            AtmosphereModel.GetAirDensity(
                Altitude: 10_000.0
            );

        Assert.Equal(1.225, SeaLevelDensity);
        Assert.Equal(1.225, HighAltitudeDensity);
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void ConstantAtmosphereRejectsInvalidDensity(
        double AirDensity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ConstantAtmosphereModel(
                AirDensity: AirDensity
            )
        );
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void ConstantAtmosphereRejectsInvalidAltitude(
        double Altitude)
    {
        ConstantAtmosphereModel AtmosphereModel = new(
            AirDensity: 1.225
        );

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            AtmosphereModel.GetAirDensity(
                Altitude: Altitude
            )
        );
    }

    [Fact]
    public void ConstantAtmosphereAllowsVacuum()
    {
        ConstantAtmosphereModel AtmosphereModel = new(
            AirDensity: 0.0
        );

        Assert.Equal(
            0.0,
            AtmosphereModel.GetAirDensity(
                Altitude: 0.0
            )
        );
    }
}