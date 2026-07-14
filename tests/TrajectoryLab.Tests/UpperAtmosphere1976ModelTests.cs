using TrajectoryLab.Core.Atmosphere;
using Xunit;

namespace TrajectoryLab.Tests;

public sealed class UpperAtmosphere1976ModelTests
{
    [Theory]
    [InlineData(
        86_000.0,
        3.6850e-6,
        5.680e-6)]
    [InlineData(
        100_000.0,
        3.1593e-7,
        4.575e-7)]
    [InlineData(
        200_000.0,
        8.3628e-10,
        2.074e-10)]
    [InlineData(
        500_000.0,
        2.9840e-12,
        4.257e-13)]
    [InlineData(
        1_000_000.0,
        7.4155e-14,
        2.907e-15)]
    public void ReturnsReferenceValuesAtTableAltitude(
        double Altitude,
        double ExpectedPressureRatio,
        double ExpectedDensityRatio)
    {
        UpperAtmosphere1976Model AtmosphereModel =
            new();

        double ActualPressure =
            AtmosphereModel.GetPressure(
                Altitude
            );

        double ActualDensity =
            AtmosphereModel.GetAirDensity(
                Altitude
            );

        double ExpectedPressure =
            UpperAtmosphere1976Model
                .SeaLevelPressure *
            ExpectedPressureRatio;

        double ExpectedDensity =
            UpperAtmosphere1976Model
                .SeaLevelDensity *
            ExpectedDensityRatio;

        double PressureRelativeError =
            Math.Abs(
                ActualPressure -
                ExpectedPressure
            ) /
            ExpectedPressure;

        double DensityRelativeError =
            Math.Abs(
                ActualDensity -
                ExpectedDensity
            ) /
            ExpectedDensity;

        Assert.InRange(
            PressureRelativeError,
            0.0,
            1e-12
        );

        Assert.InRange(
            DensityRelativeError,
            0.0,
            1e-12
        );
    }

    [Fact]
    public void PressureAndDensityDecreaseWithAltitude()
    {
        UpperAtmosphere1976Model AtmosphereModel =
            new();

        double PreviousPressure =
            AtmosphereModel.GetPressure(
                Altitude: 86_000.0
            );

        double PreviousDensity =
            AtmosphereModel.GetAirDensity(
                Altitude: 86_000.0
            );

        for (
            double Altitude = 87_000.0;
            Altitude <= 1_000_000.0;
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
    [InlineData(93_000.0)]
    [InlineData(100_000.0)]
    [InlineData(121_000.0)]
    [InlineData(200_000.0)]
    [InlineData(500_000.0)]
    [InlineData(900_000.0)]
    public void InterpolationIsContinuousAtTableBoundary(
        double BoundaryAltitude)
    {
        const double Offset = 0.001;

        UpperAtmosphere1976Model AtmosphereModel =
            new();

        double PressureBelow =
            AtmosphereModel.GetPressure(
                BoundaryAltitude -
                Offset
            );

        double PressureAbove =
            AtmosphereModel.GetPressure(
                BoundaryAltitude +
                Offset
            );

        double DensityBelow =
            AtmosphereModel.GetAirDensity(
                BoundaryAltitude -
                Offset
            );

        double DensityAbove =
            AtmosphereModel.GetAirDensity(
                BoundaryAltitude +
                Offset
            );

        double PressureRelativeDifference =
            Math.Abs(
                PressureAbove -
                PressureBelow
            ) /
            PressureBelow;

        double DensityRelativeDifference =
            Math.Abs(
                DensityAbove -
                DensityBelow
            ) /
            DensityBelow;

        Assert.InRange(
            PressureRelativeDifference,
            0.0,
            1e-6
        );

        Assert.InRange(
            DensityRelativeDifference,
            0.0,
            1e-6
        );
    }

    [Fact]
    public void UsesUpperAtmosphereTemperatureModel()
    {
        UpperAtmosphere1976Model AtmosphereModel =
            new();

        double Temperature =
            AtmosphereModel.GetTemperature(
                Altitude: 120_000.0
            );

        Assert.InRange(
            Math.Abs(
                Temperature -
                360.0
            ),
            0.0,
            1e-10
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
        UpperAtmosphere1976Model AtmosphereModel =
            new();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            AtmosphereModel.GetAirDensity(
                Altitude
            )
        );
    }
}