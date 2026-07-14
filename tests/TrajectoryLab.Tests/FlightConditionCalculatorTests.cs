using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Tests.Physics;

public sealed class FlightConditionCalculatorTests
{
    private const double Tolerance = 1.0e-12;

    [Fact]
    public void Constructor_StoresGasModel()
    {
        IdealGasModel GasModel = new();

        FlightConditionCalculator Calculator = new(
            GasModel);

        Assert.Same(
            GasModel,
            Calculator.GasModel);
    }

    [Fact]
    public void Constructor_WithNullGasModel_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new FlightConditionCalculator(null!));
    }

    [Fact]
    public void Calculate_InCalmAir_ReturnsExpectedCondition()
    {
        EnvironmentParameters Environment =
            CreateEnvironment(
                new ConstantWindModel(
                    Vector3D.Zero));

        FlightConditionCalculator Calculator =
            CreateCalculator();

        Vector3D Position = Vector3D.Zero;

        Vector3D Velocity = new(
            300.0,
            0.0,
            0.0);

        FlightCondition Condition =
            Calculator.Calculate(
                Position,
                Velocity,
                Time: 0.0,
                Environment);

        double ExpectedSpeedOfSound =
            Calculator.GasModel.GetSpeedOfSound(
                288.15);

        double ExpectedMachNumber =
            300.0 / ExpectedSpeedOfSound;

        Assert.Equal(
            0.0,
            Condition.Time);

        AssertVectorEqual(
            Position,
            Condition.Position);

        AssertVectorEqual(
            Vector3D.Zero,
            Condition.WindVelocity);

        AssertVectorEqual(
            Velocity,
            Condition.RelativeVelocity);

        Assert.InRange(
            Math.Abs(
                Condition.RelativeSpeed - 300.0),
            0.0,
            Tolerance);

        Assert.InRange(
            Math.Abs(
                Condition.Temperature - 288.15),
            0.0,
            Tolerance);

        Assert.InRange(
            Math.Abs(
                Condition.SpeedOfSound
                - ExpectedSpeedOfSound),
            0.0,
            Tolerance);

        Assert.InRange(
            Math.Abs(
                Condition.MachNumber
                - ExpectedMachNumber),
            0.0,
            Tolerance);
    }

    [Fact]
    public void Calculate_WithTailwind_UsesRelativeVelocity()
    {
        EnvironmentParameters Environment =
            CreateEnvironment(
                new ConstantWindModel(
                    new Vector3D(
                        20.0,
                        0.0,
                        0.0)));

        FlightConditionCalculator Calculator =
            CreateCalculator();

        FlightCondition Condition =
            Calculator.Calculate(
                Position: Vector3D.Zero,
                Velocity: new Vector3D(
                    300.0,
                    0.0,
                    0.0),
                Time: 0.0,
                Environment);

        AssertVectorEqual(
            new Vector3D(
                20.0,
                0.0,
                0.0),
            Condition.WindVelocity);

        AssertVectorEqual(
            new Vector3D(
                280.0,
                0.0,
                0.0),
            Condition.RelativeVelocity);

        Assert.InRange(
            Math.Abs(
                Condition.RelativeSpeed - 280.0),
            0.0,
            Tolerance);
    }

    [Fact]
    public void Calculate_WithCrosswind_UsesThreeDimensionalMagnitude()
    {
        EnvironmentParameters Environment =
            CreateEnvironment(
                new ConstantWindModel(
                    new Vector3D(
                        0.0,
                        40.0,
                        0.0)));

        FlightConditionCalculator Calculator =
            CreateCalculator();

        FlightCondition Condition =
            Calculator.Calculate(
                Position: Vector3D.Zero,
                Velocity: new Vector3D(
                    300.0,
                    0.0,
                    0.0),
                Time: 0.0,
                Environment);

        double ExpectedRelativeSpeed =
            Math.Sqrt(
                300.0 * 300.0
                + 40.0 * 40.0);

        AssertVectorEqual(
            new Vector3D(
                300.0,
                -40.0,
                0.0),
            Condition.RelativeVelocity);

        Assert.InRange(
            Math.Abs(
                Condition.RelativeSpeed
                - ExpectedRelativeSpeed),
            0.0,
            Tolerance);
    }

    [Fact]
    public void Calculate_WithLinearWind_UsesAltitudeDependentWind()
    {
        LinearWindModel WindModel = new(
            LowerAltitude: 0.0,
            LowerWindVelocity: Vector3D.Zero,
            UpperAltitude: 100.0,
            UpperWindVelocity: new Vector3D(
                20.0,
                10.0,
                0.0));

        EnvironmentParameters Environment =
            CreateEnvironment(WindModel);

        FlightConditionCalculator Calculator =
            CreateCalculator();

        FlightCondition Condition =
            Calculator.Calculate(
                Position: new Vector3D(
                    0.0,
                    0.0,
                    50.0),
                Velocity: new Vector3D(
                    100.0,
                    20.0,
                    0.0),
                Time: 5.0,
                Environment);

        AssertVectorEqual(
            new Vector3D(
                10.0,
                5.0,
                0.0),
            Condition.WindVelocity);

        AssertVectorEqual(
            new Vector3D(
                90.0,
                15.0,
                0.0),
            Condition.RelativeVelocity);

        Assert.InRange(
            Math.Abs(
                Condition.RelativeSpeed
                - Math.Sqrt(
                    90.0 * 90.0
                    + 15.0 * 15.0)),
            0.0,
            Tolerance);
    }

    [Fact]
    public void Calculate_WithInvalidPosition_ThrowsArgumentException()
    {
        EnvironmentParameters Environment =
            CreateEnvironment(
                new ConstantWindModel(
                    Vector3D.Zero));

        FlightConditionCalculator Calculator =
            CreateCalculator();

        Vector3D[] InvalidPositions =
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

        foreach (Vector3D InvalidPosition
                 in InvalidPositions)
        {
            Assert.Throws<ArgumentException>(
                () => Calculator.Calculate(
                    InvalidPosition,
                    Vector3D.Zero,
                    0.0,
                    Environment));
        }
    }

    [Fact]
    public void Calculate_WithInvalidVelocity_ThrowsArgumentException()
    {
        EnvironmentParameters Environment =
            CreateEnvironment(
                new ConstantWindModel(
                    Vector3D.Zero));

        FlightConditionCalculator Calculator =
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

        foreach (Vector3D InvalidVelocity
                 in InvalidVelocities)
        {
            Assert.Throws<ArgumentException>(
                () => Calculator.Calculate(
                    Vector3D.Zero,
                    InvalidVelocity,
                    0.0,
                    Environment));
        }
    }

    [Fact]
    public void Calculate_WithInvalidTime_ThrowsArgumentOutOfRangeException()
    {
        EnvironmentParameters Environment =
            CreateEnvironment(
                new ConstantWindModel(
                    Vector3D.Zero));

        FlightConditionCalculator Calculator =
            CreateCalculator();

        double[] InvalidTimes =
        [
            -1.0,
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity
        ];

        foreach (double InvalidTime in InvalidTimes)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => Calculator.Calculate(
                    Vector3D.Zero,
                    Vector3D.Zero,
                    InvalidTime,
                    Environment));
        }
    }

    [Fact]
    public void Calculate_WithNullEnvironment_ThrowsArgumentNullException()
    {
        FlightConditionCalculator Calculator =
            CreateCalculator();

        Assert.Throws<ArgumentNullException>(
            () => Calculator.Calculate(
                Vector3D.Zero,
                Vector3D.Zero,
                0.0,
                null!));
    }

    private static FlightConditionCalculator
        CreateCalculator()
    {
        return new FlightConditionCalculator(
            new IdealGasModel());
    }

    private static EnvironmentParameters
        CreateEnvironment(IWindModel WindModel)
    {
        return new EnvironmentParameters(
            GravityAcceleration: 9.80665,
            AtmosphereModel:
                new ConstantAtmosphereModel(
                    AirDensity: 1.225),
            WindModel: WindModel);
    }

    private static void AssertVectorEqual(
        Vector3D Expected,
        Vector3D Actual)
    {
        Assert.InRange(
            Math.Abs(Expected.X - Actual.X),
            0.0,
            Tolerance);

        Assert.InRange(
            Math.Abs(Expected.Y - Actual.Y),
            0.0,
            Tolerance);

        Assert.InRange(
            Math.Abs(Expected.Z - Actual.Z),
            0.0,
            Tolerance);
    }
}