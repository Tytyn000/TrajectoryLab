using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;

namespace TrajectoryLab.Tests.Atmosphere;

public sealed class IdealGasAtmosphereIntegrationTests
{
    [Fact]
    public void StandardAtmosphere_AtSeaLevel_ProducesExpectedSpeedOfSound()
    {
        StandardAtmosphere1976Model AtmosphereModel = new();

        IdealGasModel GasModel = new();

        double Temperature =
            AtmosphereModel.GetTemperature(0.0);

        double SpeedOfSound =
            GasModel.GetSpeedOfSound(Temperature);

        Assert.InRange(
            Math.Abs(Temperature - 288.15),
            0.0,
            1.0e-9);

        Assert.InRange(
            Math.Abs(SpeedOfSound - 340.293988026089),
            0.0,
            1.0e-9);
    }

    [Fact]
    public void StandardAtmosphere_AroundEightySixKilometers_ProducesContinuousSpeedOfSound()
    {
        StandardAtmosphere1976Model AtmosphereModel = new();

        IdealGasModel GasModel = new();

        double TransitionAltitude = 86_000.0;
        double AltitudeDifference = 1.0e-3;

        double TemperatureBelow =
            AtmosphereModel.GetTemperature(
                TransitionAltitude - AltitudeDifference);

        double TemperatureAtTransition =
            AtmosphereModel.GetTemperature(
                TransitionAltitude);

        double TemperatureAbove =
            AtmosphereModel.GetTemperature(
                TransitionAltitude + AltitudeDifference);

        double SpeedBelow =
            GasModel.GetSpeedOfSound(TemperatureBelow);

        double SpeedAtTransition =
            GasModel.GetSpeedOfSound(
                TemperatureAtTransition);

        double SpeedAbove =
            GasModel.GetSpeedOfSound(TemperatureAbove);

        Assert.InRange(
            Math.Abs(SpeedBelow - SpeedAtTransition),
            0.0,
            1.0e-2);

        Assert.InRange(
            Math.Abs(SpeedAbove - SpeedAtTransition),
            0.0,
            1.0e-2);

        Assert.True(
            double.IsFinite(SpeedBelow)
            && SpeedBelow > 0.0);

        Assert.True(
            double.IsFinite(SpeedAtTransition)
            && SpeedAtTransition > 0.0);

        Assert.True(
            double.IsFinite(SpeedAbove)
            && SpeedAbove > 0.0);
    }
}