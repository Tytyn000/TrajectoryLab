using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Tests.Physics;

public sealed class AerodynamicSummaryCalculatorTests
{
    private const double Tolerance = 1.0e-12;

    [Fact]
    public void Calculate_ReturnsExpectedSummary()
    {
        FlightCondition[] Conditions =
        [
            CreateCondition(
                RelativeSpeed: 100.0,
                Time: 0.0,
                Altitude: 0.0),

            CreateCondition(
                RelativeSpeed: 300.0,
                Time: 1.0,
                Altitude: 50.0),

            CreateCondition(
                RelativeSpeed: 200.0,
                Time: 2.0,
                Altitude: 0.0)
        ];

        AerodynamicSummaryCalculator Calculator =
            new();

        AerodynamicSummary Summary =
            Calculator.Calculate(Conditions);

        AssertClose(
            Conditions[0].MachNumber,
            Summary.InitialMachNumber);

        AssertClose(
            Conditions[1].MachNumber,
            Summary.MaximumMachNumber);

        AssertClose(
            Conditions[2].MachNumber,
            Summary.ImpactMachNumber);

        AssertClose(
            100.0,
            Summary.InitialRelativeSpeed);

        AssertClose(
            300.0,
            Summary.MaximumRelativeSpeed);

        AssertClose(
            200.0,
            Summary.ImpactRelativeSpeed);

        AssertClose(
            1.0,
            Summary.MaximumMachTime);

        AssertClose(
            50.0,
            Summary.MaximumMachAltitude);
    }

    [Fact]
    public void Calculate_WhenMaximumMachAndSpeedDiffer_TracksThemIndependently()
    {
        FlightCondition[] Conditions =
        [
            CreateCondition(
                RelativeSpeed: 300.0,
                Time: 0.0,
                Altitude: 10.0,
                Temperature: 400.0),

            CreateCondition(
                RelativeSpeed: 280.0,
                Time: 1.0,
                Altitude: 20.0,
                Temperature: 200.0),

            CreateCondition(
                RelativeSpeed: 100.0,
                Time: 2.0,
                Altitude: 0.0,
                Temperature: 288.15)
        ];

        AerodynamicSummary Summary =
            new AerodynamicSummaryCalculator()
                .Calculate(Conditions);

        AssertClose(
            300.0,
            Summary.MaximumRelativeSpeed);

        AssertClose(
            Conditions[1].MachNumber,
            Summary.MaximumMachNumber);

        AssertClose(
            1.0,
            Summary.MaximumMachTime);

        AssertClose(
            20.0,
            Summary.MaximumMachAltitude);
    }

    [Fact]
    public void Calculate_WithEqualMaximumMachNumbers_UsesFirstOccurrence()
    {
        FlightCondition[] Conditions =
        [
            CreateCondition(
                RelativeSpeed: 100.0,
                Time: 0.0,
                Altitude: 0.0),

            CreateCondition(
                RelativeSpeed: 300.0,
                Time: 1.0,
                Altitude: 10.0),

            CreateCondition(
                RelativeSpeed: 300.0,
                Time: 2.0,
                Altitude: 20.0)
        ];

        AerodynamicSummary Summary =
            new AerodynamicSummaryCalculator()
                .Calculate(Conditions);

        AssertClose(
            1.0,
            Summary.MaximumMachTime);

        AssertClose(
            10.0,
            Summary.MaximumMachAltitude);
    }

    [Fact]
    public void Calculate_WithSingleCondition_UsesItForAllValues()
    {
        FlightCondition Condition =
            CreateCondition(
                RelativeSpeed: 250.0,
                Time: 3.0,
                Altitude: 75.0);

        AerodynamicSummary Summary =
            new AerodynamicSummaryCalculator()
                .Calculate([Condition]);

        AssertClose(
            Condition.MachNumber,
            Summary.InitialMachNumber);

        AssertClose(
            Condition.MachNumber,
            Summary.MaximumMachNumber);

        AssertClose(
            Condition.MachNumber,
            Summary.ImpactMachNumber);

        AssertClose(
            250.0,
            Summary.InitialRelativeSpeed);

        AssertClose(
            250.0,
            Summary.MaximumRelativeSpeed);

        AssertClose(
            250.0,
            Summary.ImpactRelativeSpeed);

        AssertClose(
            3.0,
            Summary.MaximumMachTime);

        AssertClose(
            75.0,
            Summary.MaximumMachAltitude);
    }

    [Fact]
    public void Calculate_WithNullConditions_ThrowsArgumentNullException()
    {
        AerodynamicSummaryCalculator Calculator =
            new();

        Assert.Throws<ArgumentNullException>(
            () => Calculator.Calculate(null!));
    }

    [Fact]
    public void Calculate_WithEmptyConditions_ThrowsArgumentException()
    {
        AerodynamicSummaryCalculator Calculator =
            new();

        Assert.Throws<ArgumentException>(
            () => Calculator.Calculate(
                Array.Empty<FlightCondition>()));
    }

    [Fact]
    public void Calculate_WithNullCondition_ThrowsArgumentException()
    {
        FlightCondition[] Conditions =
        [
            CreateCondition(
                RelativeSpeed: 100.0),
            null!
        ];

        AerodynamicSummaryCalculator Calculator =
            new();

        Assert.Throws<ArgumentException>(
            () => Calculator.Calculate(Conditions));
    }

    private static FlightCondition CreateCondition(
        double RelativeSpeed,
        double Time = 0.0,
        double Altitude = 0.0,
        double Temperature = 288.15)
    {
        EnvironmentParameters Environment = new(
            GravityAcceleration: 9.80665,
            AtmosphereModel:
                new ConstantAtmosphereModel(
                    AirDensity: 1.225,
                    Temperature: Temperature),
            WindModel:
                new ConstantWindModel(
                    Vector3D.Zero));

        FlightConditionCalculator Calculator = new(
            new IdealGasModel());

        return Calculator.Calculate(
            Position: new Vector3D(
                0.0,
                0.0,
                Altitude),
            Velocity: new Vector3D(
                RelativeSpeed,
                0.0,
                0.0),
            Time: Time,
            Environment: Environment);
    }

    private static void AssertClose(
        double Expected,
        double Actual)
    {
        Assert.InRange(
            Math.Abs(Expected - Actual),
            0.0,
            Tolerance);
    }
}