using TrajectoryLab.Core.Physics;

namespace TrajectoryLab.Tests.Physics;

public sealed class DragCoefficientPointTests
{
    [Fact]
    public void Constructor_StoresValues()
    {
        DragCoefficientPoint Point = new(
            MachNumber: 1.2,
            DragCoefficient: 0.47);

        Assert.Equal(
            1.2,
            Point.MachNumber);

        Assert.Equal(
            0.47,
            Point.DragCoefficient);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Constructor_WithInvalidMachNumber_Throws(
        double MachNumber)
    {
        Assert.Throws<
            ArgumentOutOfRangeException>(
            () =>
                new DragCoefficientPoint(
                    MachNumber,
                    DragCoefficient: 0.47));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Constructor_WithInvalidDragCoefficient_Throws(
        double DragCoefficient)
    {
        Assert.Throws<
            ArgumentOutOfRangeException>(
            () =>
                new DragCoefficientPoint(
                    MachNumber: 1.0,
                    DragCoefficient));
    }
}