using TrajectoryLab.Core;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Tests.Wind;

public sealed class WindLayerTests
{
    private const double Tolerance = 1.0e-12;

    [Fact]
    public void Constructor_StoresProvidedValues()
    {
        Vector3D WindVelocity = new(
            5.0,
            -2.0,
            1.0);

        WindLayer Layer = new(
            Altitude: 250.0,
            WindVelocity: WindVelocity);

        Assert.Equal(250.0, Layer.Altitude);

        AssertVectorEqual(
            WindVelocity,
            Layer.WindVelocity);
    }

    [Fact]
    public void Constructor_WithNonFiniteAltitude_ThrowsArgumentOutOfRangeException()
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
                () => new WindLayer(
                    Altitude: InvalidAltitude,
                    WindVelocity: Vector3D.Zero));
        }
    }

    [Fact]
    public void Constructor_WithNonFiniteWindVelocity_ThrowsArgumentException()
    {
        Vector3D[] InvalidVelocities =
        [
            new Vector3D(double.NaN, 0.0, 0.0),
            new Vector3D(0.0, double.PositiveInfinity, 0.0),
            new Vector3D(0.0, 0.0, double.NegativeInfinity)
        ];

        foreach (Vector3D InvalidVelocity in InvalidVelocities)
        {
            Assert.Throws<ArgumentException>(
                () => new WindLayer(
                    Altitude: 100.0,
                    WindVelocity: InvalidVelocity));
        }
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