using TrajectoryLab.Core;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Tests.Wind;

public sealed class LinearWindModelTests
{
    private const double Tolerance = 1.0e-12;

    [Fact]
    public void Constructor_StoresProvidedValues()
    {
        Vector3D LowerWindVelocity = new(2.0, 3.0, 4.0);
        Vector3D UpperWindVelocity = new(12.0, 13.0, 14.0);

        LinearWindModel Model = new(
            LowerAltitude: 100.0,
            LowerWindVelocity: LowerWindVelocity,
            UpperAltitude: 500.0,
            UpperWindVelocity: UpperWindVelocity);

        Assert.Equal(100.0, Model.LowerAltitude);
        AssertVectorEqual(LowerWindVelocity, Model.LowerWindVelocity);

        Assert.Equal(500.0, Model.UpperAltitude);
        AssertVectorEqual(UpperWindVelocity, Model.UpperWindVelocity);
    }

    [Fact]
    public void GetWindVelocity_BelowLowerAltitude_ReturnsLowerWindVelocity()
    {
        Vector3D LowerWindVelocity = new(5.0, 2.0, -1.0);

        LinearWindModel Model = CreateModel(
            LowerWindVelocity,
            new Vector3D(15.0, 12.0, 3.0));

        Vector3D Result = Model.GetWindVelocity(
            new Vector3D(0.0, 0.0, -50.0),
            0.0);

        AssertVectorEqual(LowerWindVelocity, Result);
    }

    [Fact]
    public void GetWindVelocity_AtLowerAltitude_ReturnsLowerWindVelocity()
    {
        Vector3D LowerWindVelocity = new(5.0, 2.0, -1.0);

        LinearWindModel Model = CreateModel(
            LowerWindVelocity,
            new Vector3D(15.0, 12.0, 3.0));

        Vector3D Result = Model.GetWindVelocity(
            new Vector3D(0.0, 0.0, 0.0),
            0.0);

        AssertVectorEqual(LowerWindVelocity, Result);
    }

    [Fact]
    public void GetWindVelocity_BetweenAltitudes_InterpolatesEveryComponent()
    {
        LinearWindModel Model = new(
            LowerAltitude: 100.0,
            LowerWindVelocity: new Vector3D(0.0, 10.0, -4.0),
            UpperAltitude: 300.0,
            UpperWindVelocity: new Vector3D(20.0, 30.0, 8.0));

        Vector3D Result = Model.GetWindVelocity(
            new Vector3D(500.0, -200.0, 150.0),
            12.0);

        Vector3D Expected = new(
            5.0,
            15.0,
            -1.0);

        AssertVectorEqual(Expected, Result);
    }

    [Fact]
    public void GetWindVelocity_AtUpperAltitude_ReturnsUpperWindVelocity()
    {
        Vector3D UpperWindVelocity = new(15.0, 12.0, 3.0);

        LinearWindModel Model = CreateModel(
            new Vector3D(5.0, 2.0, -1.0),
            UpperWindVelocity);

        Vector3D Result = Model.GetWindVelocity(
            new Vector3D(0.0, 0.0, 100.0),
            0.0);

        AssertVectorEqual(UpperWindVelocity, Result);
    }

    [Fact]
    public void GetWindVelocity_AboveUpperAltitude_ReturnsUpperWindVelocity()
    {
        Vector3D UpperWindVelocity = new(15.0, 12.0, 3.0);

        LinearWindModel Model = CreateModel(
            new Vector3D(5.0, 2.0, -1.0),
            UpperWindVelocity);

        Vector3D Result = Model.GetWindVelocity(
            new Vector3D(0.0, 0.0, 500.0),
            0.0);

        AssertVectorEqual(UpperWindVelocity, Result);
    }

    [Fact]
    public void GetWindVelocity_WithDifferentValidTimes_ReturnsSameVelocity()
    {
        LinearWindModel Model = CreateModel(
            new Vector3D(0.0, 0.0, 0.0),
            new Vector3D(20.0, 10.0, 5.0));

        Vector3D Position = new(0.0, 0.0, 50.0);

        Vector3D FirstResult = Model.GetWindVelocity(Position, 0.0);
        Vector3D SecondResult = Model.GetWindVelocity(Position, 1000.0);

        AssertVectorEqual(FirstResult, SecondResult);
    }

    [Fact]
    public void Constructor_WithNonFiniteLowerAltitude_ThrowsArgumentOutOfRangeException()
    {
        double[] InvalidAltitudes =
        [
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity
        ];

        foreach (double InvalidAltitude in InvalidAltitudes)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LinearWindModel(
                    LowerAltitude: InvalidAltitude,
                    LowerWindVelocity: Vector3D.Zero,
                    UpperAltitude: 100.0,
                    UpperWindVelocity: Vector3D.Zero));
        }
    }

    [Fact]
    public void Constructor_WithNonFiniteUpperAltitude_ThrowsArgumentOutOfRangeException()
    {
        double[] InvalidAltitudes =
        [
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity
        ];

        foreach (double InvalidAltitude in InvalidAltitudes)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LinearWindModel(
                    LowerAltitude: 0.0,
                    LowerWindVelocity: Vector3D.Zero,
                    UpperAltitude: InvalidAltitude,
                    UpperWindVelocity: Vector3D.Zero));
        }
    }

    [Fact]
    public void Constructor_WhenUpperAltitudeIsNotAboveLowerAltitude_ThrowsArgumentException()
    {
        double[] InvalidUpperAltitudes =
        [
            100.0,
            50.0
        ];

        foreach (double InvalidUpperAltitude in InvalidUpperAltitudes)
        {
            Assert.Throws<ArgumentException>(
                () => new LinearWindModel(
                    LowerAltitude: 100.0,
                    LowerWindVelocity: Vector3D.Zero,
                    UpperAltitude: InvalidUpperAltitude,
                    UpperWindVelocity: Vector3D.Zero));
        }
    }

    [Fact]
    public void Constructor_WithNonFiniteLowerWindVelocity_ThrowsArgumentException()
    {
        Vector3D[] InvalidVectors =
        [
            new Vector3D(double.NaN, 0.0, 0.0),
            new Vector3D(0.0, double.PositiveInfinity, 0.0),
            new Vector3D(0.0, 0.0, double.NegativeInfinity)
        ];

        foreach (Vector3D InvalidVector in InvalidVectors)
        {
            Assert.Throws<ArgumentException>(
                () => new LinearWindModel(
                    LowerAltitude: 0.0,
                    LowerWindVelocity: InvalidVector,
                    UpperAltitude: 100.0,
                    UpperWindVelocity: Vector3D.Zero));
        }
    }

    [Fact]
    public void Constructor_WithNonFiniteUpperWindVelocity_ThrowsArgumentException()
    {
        Vector3D[] InvalidVectors =
        [
            new Vector3D(double.NaN, 0.0, 0.0),
            new Vector3D(0.0, double.PositiveInfinity, 0.0),
            new Vector3D(0.0, 0.0, double.NegativeInfinity)
        ];

        foreach (Vector3D InvalidVector in InvalidVectors)
        {
            Assert.Throws<ArgumentException>(
                () => new LinearWindModel(
                    LowerAltitude: 0.0,
                    LowerWindVelocity: Vector3D.Zero,
                    UpperAltitude: 100.0,
                    UpperWindVelocity: InvalidVector));
        }
    }

    [Fact]
    public void GetWindVelocity_WithNonFinitePosition_ThrowsArgumentException()
    {
        LinearWindModel Model = CreateModel(
            Vector3D.Zero,
            new Vector3D(10.0, 0.0, 0.0));

        Vector3D[] InvalidPositions =
        [
            new Vector3D(double.NaN, 0.0, 0.0),
            new Vector3D(0.0, double.PositiveInfinity, 0.0),
            new Vector3D(0.0, 0.0, double.NegativeInfinity)
        ];

        foreach (Vector3D InvalidPosition in InvalidPositions)
        {
            Assert.Throws<ArgumentException>(
                () => Model.GetWindVelocity(InvalidPosition, 0.0));
        }
    }

    [Fact]
    public void GetWindVelocity_WithInvalidTime_ThrowsArgumentOutOfRangeException()
    {
        LinearWindModel Model = CreateModel(
            Vector3D.Zero,
            new Vector3D(10.0, 0.0, 0.0));

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
                () => Model.GetWindVelocity(Vector3D.Zero, InvalidTime));
        }
    }

    private static LinearWindModel CreateModel(
        Vector3D LowerWindVelocity,
        Vector3D UpperWindVelocity)
    {
        return new LinearWindModel(
            LowerAltitude: 0.0,
            LowerWindVelocity: LowerWindVelocity,
            UpperAltitude: 100.0,
            UpperWindVelocity: UpperWindVelocity);
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