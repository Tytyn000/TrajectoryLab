using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Tests.Physics;

public sealed class MachNumberAtmosphereWindIntegrationTests
{
    private const double Tolerance = 1.0e-12;

    [Fact]
    public void StandardAtmosphere_WithCalmAirAndSonicVelocity_ReturnsMachOne()
    {
        StandardAtmosphere1976Model AtmosphereModel =
            new();

        ConstantWindModel WindModel = new(
            Vector3D.Zero);

        IdealGasModel GasModel = new();

        MachNumberCalculator Calculator = new(
            GasModel);

        Vector3D Position = Vector3D.Zero;
        double Time = 0.0;

        double Temperature =
            AtmosphereModel.GetTemperature(Position.Z);

        double SpeedOfSound =
            GasModel.GetSpeedOfSound(Temperature);

        Vector3D BodyVelocity = new(
            SpeedOfSound,
            0.0,
            0.0);

        Vector3D FluidVelocity =
            WindModel.GetWindVelocity(
                Position,
                Time);

        double MachNumber =
            Calculator.GetMachNumber(
                BodyVelocity,
                FluidVelocity,
                Temperature);

        Assert.InRange(
            Math.Abs(MachNumber - 1.0),
            0.0,
            Tolerance);
    }

    [Fact]
    public void StandardAtmosphere_WithLinearWind_UsesLocalWindVelocity()
    {
        StandardAtmosphere1976Model AtmosphereModel =
            new();

        LinearWindModel WindModel = new(
            LowerAltitude: 0.0,
            LowerWindVelocity: Vector3D.Zero,
            UpperAltitude: 100.0,
            UpperWindVelocity: new Vector3D(
                20.0,
                0.0,
                0.0));

        IdealGasModel GasModel = new();

        MachNumberCalculator Calculator = new(
            GasModel);

        Vector3D Position = new(
            0.0,
            0.0,
            50.0);

        Vector3D BodyVelocity = new(
            100.0,
            20.0,
            0.0);

        double Time = 5.0;

        double Temperature =
            AtmosphereModel.GetTemperature(Position.Z);

        Vector3D FluidVelocity =
            WindModel.GetWindVelocity(
                Position,
                Time);

        double Result =
            Calculator.GetMachNumber(
                BodyVelocity,
                FluidVelocity,
                Temperature);

        double ExpectedRelativeSpeed =
            Math.Sqrt(
                90.0 * 90.0
                + 20.0 * 20.0);

        double ExpectedSpeedOfSound =
            Math.Sqrt(
                IdealGasModel.DryAirSpecificHeatRatio
                * IdealGasModel.DryAirSpecificGasConstant
                * Temperature);

        double ExpectedMachNumber =
            ExpectedRelativeSpeed
            / ExpectedSpeedOfSound;

        Assert.InRange(
            Math.Abs(FluidVelocity.X - 10.0),
            0.0,
            Tolerance);

        Assert.InRange(
            Math.Abs(FluidVelocity.Y),
            0.0,
            Tolerance);

        Assert.InRange(
            Math.Abs(FluidVelocity.Z),
            0.0,
            Tolerance);

        Assert.InRange(
            Math.Abs(
                Result - ExpectedMachNumber),
            0.0,
            Tolerance);
    }
}