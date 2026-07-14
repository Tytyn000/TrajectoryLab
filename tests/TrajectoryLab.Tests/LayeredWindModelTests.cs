using TrajectoryLab.Core;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Tests.Wind;

public sealed class LayeredWindModelTests
{
    private const double Tolerance = 1.0e-12;

    [Fact]
    public void Constructor_StoresLayers()
    {
        WindLayer[] Layers =
        [
            new WindLayer(
                0.0,
                new Vector3D(1.0, 0.0, 0.0)),

            new WindLayer(
                100.0,
                new Vector3D(2.0, 0.0, 0.0)),

            new WindLayer(
                200.0,
                new Vector3D(3.0, 0.0, 0.0))
        ];

        LayeredWindModel Model = new(Layers);

        Assert.Equal(3, Model.Layers.Count);
        Assert.Same(Layers[0], Model.Layers[0]);
        Assert.Same(Layers[1], Model.Layers[1]);
        Assert.Same(Layers[2], Model.Layers[2]);
    }

    [Fact]
    public void Constructor_WithNullCollection_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new LayeredWindModel(null!));
    }

    [Fact]
    public void Constructor_WithFewerThanTwoLayers_ThrowsArgumentException()
    {
        WindLayer[][] InvalidCollections =
        [
            [],
            [
                new WindLayer(
                    0.0,
                    Vector3D.Zero)
            ]
        ];

        foreach (WindLayer[] InvalidCollection in InvalidCollections)
        {
            Assert.Throws<ArgumentException>(
                () => new LayeredWindModel(InvalidCollection));
        }
    }

    [Fact]
    public void Constructor_WithNullLayer_ThrowsArgumentException()
    {
        WindLayer[] Layers =
        [
            new WindLayer(
                0.0,
                Vector3D.Zero),

            null!,

            new WindLayer(
                100.0,
                Vector3D.Zero)
        ];

        Assert.Throws<ArgumentException>(
            () => new LayeredWindModel(Layers));
    }

    [Fact]
    public void Constructor_WithNonIncreasingAltitudes_ThrowsArgumentException()
    {
        WindLayer[][] InvalidCollections =
        [
            [
                new WindLayer(
                    0.0,
                    Vector3D.Zero),

                new WindLayer(
                    0.0,
                    Vector3D.Zero)
            ],
            [
                new WindLayer(
                    100.0,
                    Vector3D.Zero),

                new WindLayer(
                    50.0,
                    Vector3D.Zero)
            ]
        ];

        foreach (WindLayer[] InvalidCollection in InvalidCollections)
        {
            Assert.Throws<ArgumentException>(
                () => new LayeredWindModel(InvalidCollection));
        }
    }

    [Fact]
    public void GetWindVelocity_BelowFirstLayer_ReturnsFirstVelocity()
    {
        LayeredWindModel Model = CreateModel();

        Vector3D Result = Model.GetWindVelocity(
            new Vector3D(0.0, 0.0, -50.0),
            0.0);

        AssertVectorEqual(
            new Vector3D(0.0, 0.0, 0.0),
            Result);
    }

    [Fact]
    public void GetWindVelocity_AtFirstLayer_ReturnsFirstVelocity()
    {
        LayeredWindModel Model = CreateModel();

        Vector3D Result = Model.GetWindVelocity(
            new Vector3D(0.0, 0.0, 0.0),
            0.0);

        AssertVectorEqual(
            new Vector3D(0.0, 0.0, 0.0),
            Result);
    }

    [Fact]
    public void GetWindVelocity_BetweenFirstAndSecondLayers_InterpolatesVelocity()
    {
        LayeredWindModel Model = CreateModel();

        Vector3D Result = Model.GetWindVelocity(
            new Vector3D(100.0, -100.0, 25.0),
            5.0);

        AssertVectorEqual(
            new Vector3D(5.0, 2.5, -1.0),
            Result);
    }

    [Fact]
    public void GetWindVelocity_AtIntermediateLayer_ReturnsLayerVelocity()
    {
        LayeredWindModel Model = CreateModel();

        Vector3D Result = Model.GetWindVelocity(
            new Vector3D(0.0, 0.0, 50.0),
            0.0);

        AssertVectorEqual(
            new Vector3D(10.0, 5.0, -2.0),
            Result);
    }

    [Fact]
    public void GetWindVelocity_BetweenSecondAndThirdLayers_InterpolatesVelocity()
    {
        LayeredWindModel Model = CreateModel();

        Vector3D Result = Model.GetWindVelocity(
            new Vector3D(0.0, 0.0, 75.0),
            0.0);

        AssertVectorEqual(
            new Vector3D(20.0, 10.0, 1.0),
            Result);
    }

    [Fact]
    public void GetWindVelocity_AboveLastLayer_ReturnsLastVelocity()
    {
        LayeredWindModel Model = CreateModel();

        Vector3D Result = Model.GetWindVelocity(
            new Vector3D(0.0, 0.0, 500.0),
            0.0);

        AssertVectorEqual(
            new Vector3D(30.0, 15.0, 4.0),
            Result);
    }

    [Fact]
    public void GetWindVelocity_WithDifferentValidTimes_ReturnsSameVelocity()
    {
        LayeredWindModel Model = CreateModel();

        Vector3D Position = new(
            0.0,
            0.0,
            25.0);

        Vector3D FirstResult =
            Model.GetWindVelocity(Position, 0.0);

        Vector3D SecondResult =
            Model.GetWindVelocity(Position, 1000.0);

        AssertVectorEqual(
            FirstResult,
            SecondResult);
    }

    [Fact]
    public void GetWindVelocity_WithInvalidPosition_ThrowsArgumentException()
    {
        LayeredWindModel Model = CreateModel();

        Vector3D[] InvalidPositions =
        [
            new Vector3D(double.NaN, 0.0, 0.0),
            new Vector3D(0.0, double.PositiveInfinity, 0.0),
            new Vector3D(0.0, 0.0, double.NegativeInfinity)
        ];

        foreach (Vector3D InvalidPosition in InvalidPositions)
        {
            Assert.Throws<ArgumentException>(
                () => Model.GetWindVelocity(
                    InvalidPosition,
                    0.0));
        }
    }

    [Fact]
    public void GetWindVelocity_WithInvalidTime_ThrowsArgumentOutOfRangeException()
    {
        LayeredWindModel Model = CreateModel();

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
                () => Model.GetWindVelocity(
                    Vector3D.Zero,
                    InvalidTime));
        }
    }

    [Fact]
    public void GetWindVelocity_AroundIntermediateLayer_IsContinuous()
    {
        LayeredWindModel Model = CreateModel();

        double AltitudeDifference = 1.0e-8;

        Vector3D BelowResult = Model.GetWindVelocity(
            new Vector3D(
                0.0,
                0.0,
                50.0 - AltitudeDifference),
            0.0);

        Vector3D AtLayerResult = Model.GetWindVelocity(
            new Vector3D(
                0.0,
                0.0,
                50.0),
            0.0);

        Vector3D AboveResult = Model.GetWindVelocity(
            new Vector3D(
                0.0,
                0.0,
                50.0 + AltitudeDifference),
            0.0);

        Assert.InRange(
            Math.Abs(BelowResult.X - AtLayerResult.X),
            0.0,
            1.0e-6);

        Assert.InRange(
            Math.Abs(AboveResult.X - AtLayerResult.X),
            0.0,
            1.0e-6);

        Assert.InRange(
            Math.Abs(BelowResult.Y - AtLayerResult.Y),
            0.0,
            1.0e-6);

        Assert.InRange(
            Math.Abs(AboveResult.Y - AtLayerResult.Y),
            0.0,
            1.0e-6);

        Assert.InRange(
            Math.Abs(BelowResult.Z - AtLayerResult.Z),
            0.0,
            1.0e-6);

        Assert.InRange(
            Math.Abs(AboveResult.Z - AtLayerResult.Z),
            0.0,
            1.0e-6);
    }

    private static LayeredWindModel CreateModel()
    {
        WindLayer[] Layers =
        [
            new WindLayer(
                Altitude: 0.0,
                WindVelocity: new Vector3D(
                    0.0,
                    0.0,
                    0.0)),

            new WindLayer(
                Altitude: 50.0,
                WindVelocity: new Vector3D(
                    10.0,
                    5.0,
                    -2.0)),

            new WindLayer(
                Altitude: 100.0,
                WindVelocity: new Vector3D(
                    30.0,
                    15.0,
                    4.0))
        ];

        return new LayeredWindModel(Layers);
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