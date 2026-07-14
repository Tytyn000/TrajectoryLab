using TrajectoryLab.Core.Atmosphere;
using Xunit;

namespace TrajectoryLab.Tests;

public sealed class LowerAtmosphere1976ModelTests
{
    [Fact]
    public void ReturnsStandardSeaLevelValues()
    {
        LowerAtmosphere1976Model AtmosphereModel =
            new();

        double Temperature =
            AtmosphereModel.GetTemperature(
                Altitude: 0.0
            );

        double Pressure =
            AtmosphereModel.GetPressure(
                Altitude: 0.0
            );

        double AirDensity =
            AtmosphereModel.GetAirDensity(
                Altitude: 0.0
            );

        Assert.Equal(
            288.15,
            Temperature
        );

        Assert.Equal(
            101_325.0,
            Pressure
        );

        Assert.InRange(
            Math.Abs(AirDensity - 1.225), 0.0, 1e-6);
    }

    [Fact]
    public void ReturnsExpectedValuesAtTropopause()
    {
        LowerAtmosphere1976Model AtmosphereModel =
            new();

        double GeometricAltitude =
            LowerAtmosphere1976Model
                .GeopotentialToGeometricAltitude(
                    GeopotentialAltitude: 11_000.0
                );

        double Temperature =
            AtmosphereModel.GetTemperature(
                GeometricAltitude
            );

        double Pressure =
            AtmosphereModel.GetPressure(
                GeometricAltitude
            );

        Assert.InRange(
            Math.Abs(Temperature - 216.65),
            0.0,
            1e-10);

        Assert.InRange(
            Math.Abs(Pressure - 22_632.0),
            0.0,
            0.2
        );
    }

    [Theory]
    [InlineData(11_000.0)]
    [InlineData(20_000.0)]
    [InlineData(32_000.0)]
    [InlineData(47_000.0)]
    [InlineData(51_000.0)]
    [InlineData(71_000.0)]
    public void IsContinuousAtLayerBoundary(
        double BoundaryGeopotentialAltitude)
    {
        const double Offset = 0.001;

        LowerAtmosphere1976Model AtmosphereModel =
            new();

        double BoundaryGeometricAltitude =
            LowerAtmosphere1976Model
                .GeopotentialToGeometricAltitude(
                    BoundaryGeopotentialAltitude
                );

        double PressureBelow =
            AtmosphereModel.GetPressure(
                BoundaryGeometricAltitude -
                Offset
            );

        double PressureAbove =
            AtmosphereModel.GetPressure(
                BoundaryGeometricAltitude +
                Offset
            );

        double DensityBelow =
            AtmosphereModel.GetAirDensity(
                BoundaryGeometricAltitude -
                Offset
            );

        double DensityAbove =
            AtmosphereModel.GetAirDensity(
                BoundaryGeometricAltitude +
                Offset
            );

        double RelativePressureDifference =
            Math.Abs(
                PressureAbove - PressureBelow
            ) / PressureBelow;

        double RelativeDensityDifference =
            Math.Abs(
                DensityAbove - DensityBelow
            ) / DensityBelow;

        Assert.InRange(
            RelativePressureDifference,
            0.0,
            1e-6
        );

        Assert.InRange(
            RelativeDensityDifference,
            0.0,
            1e-6
        );
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(5_000.0)]
    [InlineData(15_000.0)]
    [InlineData(25_000.0)]
    [InlineData(40_000.0)]
    [InlineData(50_000.0)]
    [InlineData(70_000.0)]
    [InlineData(85_000.0)]
    public void PressureAndDensityRemainPositive(
        double Altitude)
    {
        LowerAtmosphere1976Model AtmosphereModel =
            new();

        double Pressure =
            AtmosphereModel.GetPressure(
                Altitude
            );

        double AirDensity =
            AtmosphereModel.GetAirDensity(
                Altitude
            );

        Assert.True(Pressure > 0.0);
        Assert.True(AirDensity > 0.0);
    }

    [Fact]
    public void DensityGenerallyDecreasesWithAltitude()
    {
        LowerAtmosphere1976Model AtmosphereModel =
            new();

        double PreviousDensity =
            AtmosphereModel.GetAirDensity(
                Altitude: 0.0
            );

        for (
            double Altitude = 1_000.0;
            Altitude <= 85_000.0;
            Altitude += 1_000.0)
        {
            double CurrentDensity =
                AtmosphereModel.GetAirDensity(
                    Altitude
                );

            Assert.True(
                CurrentDensity <
                PreviousDensity
            );

            PreviousDensity =
                CurrentDensity;
        }
    }

    [Fact]
    public void ConvertsAltitudeInBothDirections()
    {
        const double GeometricAltitude =
            50_000.0;

        double GeopotentialAltitude =
            LowerAtmosphere1976Model
                .GeometricToGeopotentialAltitude(
                    GeometricAltitude
                );

        double ReconstructedAltitude =
            LowerAtmosphere1976Model
                .GeopotentialToGeometricAltitude(
                    GeopotentialAltitude
                );

        Assert.InRange(
            Math.Abs(
                ReconstructedAltitude -
                GeometricAltitude
            ),
            0.0,
            1e-9
        );
    }

    [Theory]
    [InlineData(-5_000.1)]
    [InlineData(86_001.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void RejectsUnsupportedAltitude(
        double Altitude)
    {
        LowerAtmosphere1976Model AtmosphereModel =
            new();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            AtmosphereModel.GetAirDensity(
                Altitude
            )
        );
    }
}