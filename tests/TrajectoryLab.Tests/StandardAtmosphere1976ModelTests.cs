using TrajectoryLab.Core.Atmosphere;
using Xunit;

namespace TrajectoryLab.Tests;

public sealed class StandardAtmosphere1976ModelTests
{
    [Fact]
    public void UsesLowerAtmosphereBelowTransition()
    {
        const double Altitude = 50_000.0;

        LowerAtmosphere1976Model LowerModel =
            new();

        StandardAtmosphere1976Model StandardModel =
            new();

        Assert.Equal(
            LowerModel.GetTemperature(Altitude),
            StandardModel.GetTemperature(Altitude)
        );

        Assert.Equal(
            LowerModel.GetPressure(Altitude),
            StandardModel.GetPressure(Altitude)
        );

        Assert.Equal(
            LowerModel.GetAirDensity(Altitude),
            StandardModel.GetAirDensity(Altitude)
        );
    }

    [Fact]
    public void IsContinuousAtTransitionAltitude()
    {
        const double Offset = 0.001;

        StandardAtmosphere1976Model AtmosphereModel =
            new();

        double TemperatureBelow =
            AtmosphereModel.GetTemperature(
                StandardAtmosphere1976Model
                    .TransitionAltitude -
                Offset
            );

        double TemperatureAbove =
            AtmosphereModel.GetTemperature(
                StandardAtmosphere1976Model
                    .TransitionAltitude +
                Offset
            );

        double PressureBelow =
            AtmosphereModel.GetPressure(
                StandardAtmosphere1976Model
                    .TransitionAltitude -
                Offset
            );

        double PressureAbove =
            AtmosphereModel.GetPressure(
                StandardAtmosphere1976Model
                    .TransitionAltitude +
                Offset
            );

        double DensityBelow =
            AtmosphereModel.GetAirDensity(
                StandardAtmosphere1976Model
                    .TransitionAltitude -
                Offset
            );

        double DensityAbove =
            AtmosphereModel.GetAirDensity(
                StandardAtmosphere1976Model
                    .TransitionAltitude +
                Offset
            );

        Assert.InRange(
            Math.Abs(
                TemperatureAbove -
                TemperatureBelow
            ),
            0.0,
            1e-5
        );

        Assert.InRange(
            Math.Abs(
                PressureAbove -
                PressureBelow
            ) / PressureBelow,
            0.0,
            1e-6
        );

        Assert.InRange(
            Math.Abs(
                DensityAbove -
                DensityBelow
            ) / DensityBelow,
            0.0,
            1e-6
        );
    }

    [Theory]
    [InlineData(91_000.0)]
    [InlineData(120_000.0)]
    [InlineData(500_000.0)]
    public void UsesUpperTemperatureAfterBlend(
        double Altitude)
    {
        UpperAtmosphere1976Model UpperModel =
            new();

        StandardAtmosphere1976Model StandardModel =
            new();

        Assert.Equal(
            UpperModel.GetTemperature(Altitude),
            StandardModel.GetTemperature(Altitude)
        );
    }

    [Fact]
    public void PressureAndDensityDecreaseAcrossCompleteRange()
    {
        StandardAtmosphere1976Model AtmosphereModel =
            new();

        double PreviousPressure =
            AtmosphereModel.GetPressure(
                Altitude:
                    StandardAtmosphere1976Model
                        .MinimumSupportedAltitude
            );

        double PreviousDensity =
            AtmosphereModel.GetAirDensity(
                Altitude:
                    StandardAtmosphere1976Model
                        .MinimumSupportedAltitude
            );

        for (
            double Altitude = -4_000.0;
            Altitude <=
                StandardAtmosphere1976Model
                    .MaximumSupportedAltitude;
            Altitude += 1_000.0)
        {
            double CurrentPressure =
                AtmosphereModel.GetPressure(
                    Altitude
                );

            double CurrentDensity =
                AtmosphereModel.GetAirDensity(
                    Altitude
                );

            Assert.True(
                CurrentPressure <
                PreviousPressure
            );

            Assert.True(
                CurrentDensity <
                PreviousDensity
            );

            PreviousPressure =
                CurrentPressure;

            PreviousDensity =
                CurrentDensity;
        }
    }

    [Theory]
    [InlineData(-5_000.0)]
    [InlineData(0.0)]
    [InlineData(86_000.0)]
    [InlineData(1_000_000.0)]
    public void ValuesRemainPositive(
        double Altitude)
    {
        StandardAtmosphere1976Model AtmosphereModel =
            new();

        Assert.True(
            AtmosphereModel.GetTemperature(
                Altitude
            ) > 0.0
        );

        Assert.True(
            AtmosphereModel.GetPressure(
                Altitude
            ) > 0.0
        );

        Assert.True(
            AtmosphereModel.GetAirDensity(
                Altitude
            ) > 0.0
        );
    }

    [Theory]
    [InlineData(-5_000.1)]
    [InlineData(1_000_000.1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void RejectsUnsupportedAltitude(
        double Altitude)
    {
        StandardAtmosphere1976Model AtmosphereModel =
            new();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            AtmosphereModel.GetAirDensity(
                Altitude
            )
        );
    }
}