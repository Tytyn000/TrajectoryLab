using TrajectoryLab.Core.Physics;

namespace TrajectoryLab.Tests.Physics;

public sealed class ConstantDragCoefficientModelTests
{
    [Fact]
    public void Constructor_StoresDragCoefficient()
    {
        ConstantDragCoefficientModel Model =
            new(
                DragCoefficient: 0.47);

        Assert.Equal(
            0.47,
            Model.DragCoefficient);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    [InlineData(5.0)]
    public void GetDragCoefficient_ReturnsConstantValue(
        double MachNumber)
    {
        ConstantDragCoefficientModel Model =
            new(
                DragCoefficient: 0.47);

        double DragCoefficient =
            Model.GetDragCoefficient(
                MachNumber);

        Assert.Equal(
            0.47,
            DragCoefficient);
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
                new ConstantDragCoefficientModel(
                    DragCoefficient));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void GetDragCoefficient_WithInvalidMachNumber_Throws(
        double MachNumber)
    {
        ConstantDragCoefficientModel Model =
            new(
                DragCoefficient: 0.47);

        Assert.Throws<
            ArgumentOutOfRangeException>(
            () =>
                Model.GetDragCoefficient(
                    MachNumber));
    }
}