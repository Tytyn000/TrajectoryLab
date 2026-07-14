using TrajectoryLab.Core;
using TrajectoryLab.Core.Mathematics;

namespace TrajectoryLab.Tests.Physics;

public sealed class MachNumberCalculatorTests
{
    private const double Tolerance = 1.0e-12;

    [Fact]
    public void Constructor_StoresGasModel()
    {
        IdealGasModel GasModel = new();

        MachNumberCalculator Calculator = new(
            GasModel);

        Assert.Same(
            GasModel,
            Calculator.GasModel);
    }

    [Fact]
    public void Constructor_WithNullGasModel_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new MachNumberCalculator(null!));
    }

    [Fact]
    public void GetMachNumber_WithZeroRelativeSpeed_ReturnsZero()
    {
        MachNumberCalculator Calculator =
            CreateCalculator();

        double Result = Calculator.GetMachNumber(
            RelativeSpeed: 0.0,
            Temperature: 288.15);

        Assert.Equal(0.0, Result);
    }

    [Fact]
    public void GetMachNumber_AtThreeHundredMetersPerSecond_ReturnsExpectedValue()
    {
        MachNumberCalculator Calculator =
            CreateCalculator();

        double Result = Calculator.GetMachNumber(
            RelativeSpeed: 300.0,
            Temperature: 288.15);

        Assert.InRange(
            Math.Abs(
                Result - 0.8815906555980654),
            0.0,
            Tolerance);
    }

    [Fact]
    public void GetMachNumber_WhenRelativeSpeedEqualsSpeedOfSound_ReturnsOne()
    {
        IdealGasModel GasModel = new();

        MachNumberCalculator Calculator = new(
            GasModel);

        double Temperature = 288.15;

        double SpeedOfSound =
            GasModel.GetSpeedOfSound(Temperature);

        double Result = Calculator.GetMachNumber(
            BodyVelocity: new Vector3D(
                SpeedOfSound,
                0.0,
                0.0),
            FluidVelocity: Vector3D.Zero,
            Temperature: Temperature);

        Assert.InRange(
            Math.Abs(Result - 1.0),
            0.0,
            Tolerance);
    }

    [Fact]
    public void GetMachNumber_WithTailwind_UsesLowerRelativeSpeed()
    {
        MachNumberCalculator Calculator =
            CreateCalculator();

        double Result = Calculator.GetMachNumber(
            BodyVelocity: new Vector3D(
                300.0,
                0.0,
                0.0),
            FluidVelocity: new Vector3D(
                20.0,
                0.0,
                0.0),
            Temperature: 288.15);

        Assert.InRange(
            Math.Abs(
                Result - 0.8228179452248611),
            0.0,
            Tolerance);
    }

    [Fact]
    public void GetMachNumber_WithHeadwind_UsesHigherRelativeSpeed()
    {
        MachNumberCalculator Calculator =
            CreateCalculator();

        double Result = Calculator.GetMachNumber(
            BodyVelocity: new Vector3D(
                300.0,
                0.0,
                0.0),
            FluidVelocity: new Vector3D(
                -20.0,
                0.0,
                0.0),
            Temperature: 288.15);

        Assert.InRange(
            Math.Abs(
                Result - 0.9403633659712698),
            0.0,
            Tolerance);
    }

    [Fact]
    public void GetMachNumber_WithCrosswind_UsesThreeDimensionalRelativeSpeed()
    {
        MachNumberCalculator Calculator =
            CreateCalculator();

        double Result = Calculator.GetMachNumber(
            BodyVelocity: new Vector3D(
                300.0,
                0.0,
                0.0),
            FluidVelocity: new Vector3D(
                0.0,
                40.0,
                0.0),
            Temperature: 288.15);

        Assert.InRange(
            Math.Abs(
                Result - 0.8893924948954073),
            0.0,
            Tolerance);
    }

    [Fact]
    public void GetMachNumber_WithInvalidRelativeSpeed_ThrowsArgumentOutOfRangeException()
    {
        MachNumberCalculator Calculator =
            CreateCalculator();

        double[] InvalidSpeeds =
        [
            -1.0,
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity
        ];

        foreach (double InvalidSpeed in InvalidSpeeds)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => Calculator.GetMachNumber(
                    RelativeSpeed: InvalidSpeed,
                    Temperature: 288.15));
        }
    }

    [Fact]
    public void GetMachNumber_WithInvalidBodyVelocity_ThrowsArgumentException()
    {
        MachNumberCalculator Calculator =
            CreateCalculator();

        Vector3D[] InvalidVelocities =
        [
            new Vector3D(
                double.NaN,
                0.0,
                0.0),

            new Vector3D(
                0.0,
                double.PositiveInfinity,
                0.0),

            new Vector3D(
                0.0,
                0.0,
                double.NegativeInfinity)
        ];

        foreach (Vector3D InvalidVelocity in InvalidVelocities)
        {
            Assert.Throws<ArgumentException>(
                () => Calculator.GetMachNumber(
                    BodyVelocity: InvalidVelocity,
                    FluidVelocity: Vector3D.Zero,
                    Temperature: 288.15));
        }
    }

    [Fact]
    public void GetMachNumber_WithInvalidFluidVelocity_ThrowsArgumentException()
    {
        MachNumberCalculator Calculator =
            CreateCalculator();

        Vector3D[] InvalidVelocities =
        [
            new Vector3D(
                double.NaN,
                0.0,
                0.0),

            new Vector3D(
                0.0,
                double.PositiveInfinity,
                0.0),

            new Vector3D(
                0.0,
                0.0,
                double.NegativeInfinity)
        ];

        foreach (Vector3D InvalidVelocity in InvalidVelocities)
        {
            Assert.Throws<ArgumentException>(
                () => Calculator.GetMachNumber(
                    BodyVelocity: Vector3D.Zero,
                    FluidVelocity: InvalidVelocity,
                    Temperature: 288.15));
        }
    }

    [Fact]
    public void GetMachNumber_WithInvalidTemperature_ThrowsArgumentOutOfRangeException()
    {
        MachNumberCalculator Calculator =
            CreateCalculator();

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
                () => Calculator.GetMachNumber(
                    RelativeSpeed: 100.0,
                    Temperature: InvalidTemperature));
        }
    }

    private static MachNumberCalculator CreateCalculator()
    {
        return new MachNumberCalculator(
            new IdealGasModel());
    }
}