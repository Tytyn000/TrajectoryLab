using TrajectoryLab.Core;

namespace TrajectoryLab.Tests.Physics;

public sealed class IdealGasModelTests
{
    private const double Tolerance = 1.0e-9;

    [Fact]
    public void Constructor_WithoutParameters_UsesDryAirProperties()
    {
        IdealGasModel Model = new();

        Assert.Equal(
            IdealGasModel.DryAirSpecificHeatRatio,
            Model.SpecificHeatRatio);

        Assert.Equal(
            IdealGasModel.DryAirSpecificGasConstant,
            Model.SpecificGasConstant);
    }

    [Fact]
    public void Constructor_WithCustomProperties_StoresProvidedValues()
    {
        IdealGasModel Model = new(
            SpecificHeatRatio: 1.3,
            SpecificGasConstant: 188.92);

        Assert.Equal(
            1.3,
            Model.SpecificHeatRatio);

        Assert.Equal(
            188.92,
            Model.SpecificGasConstant);
    }

    [Fact]
    public void GetSpeedOfSound_AtSeaLevelTemperature_ReturnsExpectedValue()
    {
        IdealGasModel Model = new();

        double Result = Model.GetSpeedOfSound(
            Temperature: 288.15);

        Assert.InRange(
            Math.Abs(Result - 340.293988026089),
            0.0,
            Tolerance);
    }

    [Fact]
    public void GetSpeedOfSound_AtColdAtmosphericTemperature_ReturnsExpectedValue()
    {
        IdealGasModel Model = new();

        double Result = Model.GetSpeedOfSound(
            Temperature: 216.65);

        Assert.InRange(
            Math.Abs(Result - 295.069493509072),
            0.0,
            Tolerance);
    }

    [Fact]
    public void GetSpeedOfSound_WithCustomGasProperties_ReturnsExpectedValue()
    {
        IdealGasModel Model = new(
            SpecificHeatRatio: 1.3,
            SpecificGasConstant: 188.92);

        double Result = Model.GetSpeedOfSound(
            Temperature: 250.0);

        Assert.InRange(
            Math.Abs(Result - 247.788216023281),
            0.0,
            Tolerance);
    }

    [Fact]
    public void GetSpeedOfSound_WhenTemperatureIncreases_Increases()
    {
        IdealGasModel Model = new();

        double LowTemperatureResult =
            Model.GetSpeedOfSound(200.0);

        double HighTemperatureResult =
            Model.GetSpeedOfSound(300.0);

        Assert.True(
            HighTemperatureResult > LowTemperatureResult,
            "La vitesse du son doit augmenter lorsque la température augmente.");
    }

    [Fact]
    public void Constructor_WithInvalidSpecificHeatRatio_ThrowsArgumentOutOfRangeException()
    {
        double[] InvalidValues =
        [
            -1.0,
            0.0,
            1.0,
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity
        ];

        foreach (double InvalidValue in InvalidValues)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new IdealGasModel(
                    SpecificHeatRatio: InvalidValue,
                    SpecificGasConstant:
                        IdealGasModel.DryAirSpecificGasConstant));
        }
    }

    [Fact]
    public void Constructor_WithInvalidSpecificGasConstant_ThrowsArgumentOutOfRangeException()
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
                () => new IdealGasModel(
                    SpecificHeatRatio:
                        IdealGasModel.DryAirSpecificHeatRatio,
                    SpecificGasConstant: InvalidValue));
        }
    }

    [Fact]
    public void GetSpeedOfSound_WithInvalidTemperature_ThrowsArgumentOutOfRangeException()
    {
        IdealGasModel Model = new();

        double[] InvalidTemperatures =
        [
            -1.0,
            0.0,
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity
        ];

        foreach (double InvalidTemperature in InvalidTemperatures)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => Model.GetSpeedOfSound(
                    InvalidTemperature));
        }
    }
}