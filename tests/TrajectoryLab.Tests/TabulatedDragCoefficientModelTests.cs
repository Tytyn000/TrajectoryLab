using TrajectoryLab.Core.Physics;

namespace TrajectoryLab.Tests.Physics;

public sealed class
    TabulatedDragCoefficientModelTests
{
    private const double Tolerance =
        1.0e-12;

    [Fact]
    public void Constructor_StoresPoints()
    {
        DragCoefficientPoint[] Points =
            CreatePoints();

        TabulatedDragCoefficientModel Model =
            new(Points);

        Assert.Equal(
            Points.Length,
            Model.Points.Count);

        for (int Index = 0;
             Index < Points.Length;
             Index++)
        {
            Assert.Same(
                Points[Index],
                Model.Points[Index]);
        }
    }

    [Fact]
    public void Constructor_CopiesSourceCollection()
    {
        DragCoefficientPoint[] Points =
            CreatePoints();

        DragCoefficientPoint OriginalFirstPoint =
            Points[0];

        TabulatedDragCoefficientModel Model =
            new(Points);

        Points[0] =
            new DragCoefficientPoint(
                MachNumber: 0.0,
                DragCoefficient: 10.0);

        Assert.Same(
            OriginalFirstPoint,
            Model.Points[0]);
    }

    [Fact]
    public void Constructor_WithNullCollection_Throws()
    {
        Assert.Throws<
            ArgumentNullException>(
            () =>
                new TabulatedDragCoefficientModel(
                    null!));
    }

    [Fact]
    public void Constructor_WithFewerThanTwoPoints_Throws()
    {
        DragCoefficientPoint[][] Collections =
        [
            [],
            [
                new DragCoefficientPoint(
                    MachNumber: 0.0,
                    DragCoefficient: 0.47)
            ]
        ];

        foreach (
            DragCoefficientPoint[] Points
            in Collections)
        {
            Assert.Throws<ArgumentException>(
                () =>
                    new TabulatedDragCoefficientModel(
                        Points));
        }
    }

    [Fact]
    public void Constructor_WithNullPoint_Throws()
    {
        DragCoefficientPoint[] Points =
        [
            new DragCoefficientPoint(
                MachNumber: 0.0,
                DragCoefficient: 0.2),

            null!,

            new DragCoefficientPoint(
                MachNumber: 1.0,
                DragCoefficient: 0.4)
        ];

        Assert.Throws<ArgumentException>(
            () =>
                new TabulatedDragCoefficientModel(
                    Points));
    }

    [Fact]
    public void Constructor_WithNonIncreasingMachNumbers_Throws()
    {
        DragCoefficientPoint[][] Collections =
        [
            [
                new DragCoefficientPoint(
                    MachNumber: 0.5,
                    DragCoefficient: 0.2),

                new DragCoefficientPoint(
                    MachNumber: 0.5,
                    DragCoefficient: 0.4)
            ],
            [
                new DragCoefficientPoint(
                    MachNumber: 1.0,
                    DragCoefficient: 0.2),

                new DragCoefficientPoint(
                    MachNumber: 0.5,
                    DragCoefficient: 0.4)
            ]
        ];

        foreach (
            DragCoefficientPoint[] Points
            in Collections)
        {
            Assert.Throws<ArgumentException>(
                () =>
                    new TabulatedDragCoefficientModel(
                        Points));
        }
    }

    [Fact]
    public void GetDragCoefficient_BelowFirstPoint_ReturnsFirstValue()
    {
        TabulatedDragCoefficientModel Model =
            CreateModel();

        double Result =
            Model.GetDragCoefficient(
                MachNumber: 0.0);

        AssertClose(
            Expected: 0.2,
            Actual: Result);
    }

    [Fact]
    public void GetDragCoefficient_AtFirstPoint_ReturnsFirstValue()
    {
        TabulatedDragCoefficientModel Model =
            CreateModel();

        double Result =
            Model.GetDragCoefficient(
                MachNumber: 0.2);

        AssertClose(
            Expected: 0.2,
            Actual: Result);
    }

    [Fact]
    public void GetDragCoefficient_BetweenFirstAndSecondPoints_Interpolates()
    {
        TabulatedDragCoefficientModel Model =
            CreateModel();

        double Result =
            Model.GetDragCoefficient(
                MachNumber: 0.5);

        AssertClose(
            Expected: 0.35,
            Actual: Result);
    }

    [Fact]
    public void GetDragCoefficient_AtIntermediatePoint_ReturnsExactValue()
    {
        TabulatedDragCoefficientModel Model =
            CreateModel();

        double Result =
            Model.GetDragCoefficient(
                MachNumber: 0.8);

        AssertClose(
            Expected: 0.5,
            Actual: Result);
    }

    [Fact]
    public void GetDragCoefficient_BetweenSecondAndThirdPoints_Interpolates()
    {
        TabulatedDragCoefficientModel Model =
            CreateModel();

        double Result =
            Model.GetDragCoefficient(
                MachNumber: 1.0);

        AssertClose(
            Expected: 0.65,
            Actual: Result);
    }

    [Fact]
    public void GetDragCoefficient_BetweenThirdAndFourthPoints_Interpolates()
    {
        TabulatedDragCoefficientModel Model =
            CreateModel();

        double Result =
            Model.GetDragCoefficient(
                MachNumber: 1.6);

        AssertClose(
            Expected: 0.7,
            Actual: Result);
    }

    [Fact]
    public void GetDragCoefficient_AtLastPoint_ReturnsLastValue()
    {
        TabulatedDragCoefficientModel Model =
            CreateModel();

        double Result =
            Model.GetDragCoefficient(
                MachNumber: 2.0);

        AssertClose(
            Expected: 0.6,
            Actual: Result);
    }

    [Fact]
    public void GetDragCoefficient_AboveLastPoint_ReturnsLastValue()
    {
        TabulatedDragCoefficientModel Model =
            CreateModel();

        double Result =
            Model.GetDragCoefficient(
                MachNumber: 10.0);

        AssertClose(
            Expected: 0.6,
            Actual: Result);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void GetDragCoefficient_WithInvalidMachNumber_Throws(
        double MachNumber)
    {
        TabulatedDragCoefficientModel Model =
            CreateModel();

        Assert.Throws<
            ArgumentOutOfRangeException>(
            () =>
                Model.GetDragCoefficient(
                    MachNumber));
    }

    [Fact]
    public void GetDragCoefficient_AroundIntermediatePoint_IsContinuous()
    {
        TabulatedDragCoefficientModel Model =
            CreateModel();

        const double Difference =
            1.0e-8;

        double Below =
            Model.GetDragCoefficient(
                0.8 - Difference);

        double AtPoint =
            Model.GetDragCoefficient(
                0.8);

        double Above =
            Model.GetDragCoefficient(
                0.8 + Difference);

        Assert.InRange(
            Math.Abs(Below - AtPoint),
            0.0,
            1.0e-7);

        Assert.InRange(
            Math.Abs(Above - AtPoint),
            0.0,
            1.0e-7);
    }

    private static
        TabulatedDragCoefficientModel
        CreateModel()
    {
        return new TabulatedDragCoefficientModel(
            CreatePoints());
    }

    private static DragCoefficientPoint[]
        CreatePoints()
    {
        return
        [
            new DragCoefficientPoint(
                MachNumber: 0.2,
                DragCoefficient: 0.2),

            new DragCoefficientPoint(
                MachNumber: 0.8,
                DragCoefficient: 0.5),

            new DragCoefficientPoint(
                MachNumber: 1.2,
                DragCoefficient: 0.8),

            new DragCoefficientPoint(
                MachNumber: 2.0,
                DragCoefficient: 0.6)
        ];
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