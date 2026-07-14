using TrajectoryLab.Core.Atmosphere;

namespace TrajectoryLab.Tests.Atmosphere;

public sealed class ConstantAtmosphereModelTests
{
    [Fact]
    public void Constructor_WithAirDensityOnly_UsesDefaultTemperature()
    {
        ConstantAtmosphereModel Model = new(
            AirDensity: 1.225);

        Assert.Equal(
            1.225,
            Model.AirDensity);

        Assert.Equal(
            ConstantAtmosphereModel.DefaultTemperature,
            Model.Temperature);
    }

    [Fact]
    public void Constructor_WithTemperature_StoresProvidedValues()
    {
        ConstantAtmosphereModel Model = new(
            AirDensity: 0.8,
            Temperature: 250.0);

        Assert.Equal(
            0.8,
            Model.AirDensity);

        Assert.Equal(
            250.0,
            Model.Temperature);
    }

    [Fact]
    public void GetAirDensity_AtDifferentAltitudes_ReturnsConstantDensity()
    {
        ConstantAtmosphereModel Model = new(
            AirDensity: 1.225);

        Assert.Equal(
            1.225,
            Model.GetAirDensity(-1000.0));

        Assert.Equal(
            1.225,
            Model.GetAirDensity(0.0));

        Assert.Equal(
            1.225,
            Model.GetAirDensity(100000.0));
    }

    [Fact]
    public void GetTemperature_AtDifferentAltitudes_ReturnsConstantTemperature()
    {
        ConstantAtmosphereModel Model = new(
            AirDensity: 1.225,
            Temperature: 270.0);

        Assert.Equal(
            270.0,
            Model.GetTemperature(-1000.0));

        Assert.Equal(
            270.0,
            Model.GetTemperature(0.0));

        Assert.Equal(
            270.0,
            Model.GetTemperature(100000.0));
    }

    [Fact]
    public void Constructor_WithInvalidAirDensity_ThrowsArgumentOutOfRangeException()
    {
        double[] InvalidValues =
        [
            -1.0,
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity
        ];

        foreach (double InvalidValue in InvalidValues)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new ConstantAtmosphereModel(
                    AirDensity: InvalidValue));
        }
    }

    [Fact]
    public void Constructor_WithInvalidTemperature_ThrowsArgumentOutOfRangeException()
    {
        double[] InvalidValues =
        [
            -1.0,
            0.0,
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity
        ];

        foreach (double InvalidValue in InvalidValues)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new ConstantAtmosphereModel(
                    AirDensity: 1.225,
                    Temperature: InvalidValue));
        }
    }

    [Fact]
    public void GetAirDensity_WithInvalidAltitude_ThrowsArgumentOutOfRangeException()
    {
        ConstantAtmosphereModel Model = new(
            AirDensity: 1.225);

        double[] InvalidAltitudes =
        [
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity
        ];

        foreach (double InvalidAltitude in InvalidAltitudes)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => Model.GetAirDensity(
                    InvalidAltitude));
        }
    }

    [Fact]
    public void GetTemperature_WithInvalidAltitude_ThrowsArgumentOutOfRangeException()
    {
        ConstantAtmosphereModel Model = new(
            AirDensity: 1.225);

        double[] InvalidAltitudes =
        [
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity
        ];

        foreach (double InvalidAltitude in InvalidAltitudes)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => Model.GetTemperature(
                    InvalidAltitude));
        }
    }
}