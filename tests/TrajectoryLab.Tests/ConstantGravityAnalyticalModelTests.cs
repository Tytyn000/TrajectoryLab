using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Simulation;
using Xunit;

namespace TrajectoryLab.Tests;

public sealed class
    ConstantGravityAnalyticalModelTests
{
    [Fact]
    public void ConstructorRejectsInvalidGravity()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ConstantGravityAnalyticalModel(
                Vector3D.Zero,
                Vector3D.Zero,
                GravityAcceleration: 0.0));
    }

    [Fact]
    public void EvaluatePreservesInitialStateAtZero()
    {
        Vector3D InitialPosition =
            new(
                1.0,
                2.0,
                3.0);

        Vector3D InitialVelocity =
            new(
                4.0,
                5.0,
                6.0);

        ConstantGravityAnalyticalModel Model =
            new(
                InitialPosition,
                InitialVelocity,
                9.80665);

        SimulationState State =
            Model.Evaluate(
                0.0);

        Assert.Equal(
            InitialPosition,
            State.Position);

        Assert.Equal(
            InitialVelocity,
            State.Velocity);
    }

    [Fact]
    public void EvaluateMatchesConstantAccelerationFormula()
    {
        ConstantGravityAnalyticalModel Model =
            new(
                Vector3D.Zero,
                new Vector3D(
                    10.0,
                    0.0,
                    20.0),
                10.0);

        SimulationState State =
            Model.Evaluate(
                2.0);

        Assert.Equal(
            20.0,
            State.Position.X,
            12);

        Assert.Equal(
            20.0,
            State.Position.Z,
            12);

        Assert.Equal(
            0.0,
            State.Velocity.Z,
            12);
    }

    [Fact]
    public void VerticalMotionIsIdentifiedAsFreeFall()
    {
        ConstantGravityAnalyticalModel Model =
            new(
                Vector3D.Zero,
                new Vector3D(
                    0.0,
                    0.0,
                    10.0),
                9.80665);

        Assert.Contains(
            "Chute libre",
            Model.MotionName);
    }

    [Fact]
    public void HorizontalMotionIsIdentifiedAsParabolic()
    {
        ConstantGravityAnalyticalModel Model =
            new(
                Vector3D.Zero,
                new Vector3D(
                    10.0,
                    0.0,
                    10.0),
                9.80665);

        Assert.Contains(
            "parabolique",
            Model.MotionName);
    }

    [Fact]
    public void AltitudeEquationContainsVariablesAndUnits()
    {
        ConstantGravityAnalyticalModel Model =
            new(
                Vector3D.Zero,
                new Vector3D(
                    10.0,
                    0.0,
                    10.0),
                9.80665);

        AnalyticalEquationDescription Equation =
            Model.GetEquation(
                TrajectoryQuantity.Altitude);

        Assert.Contains(
            "z(t)",
            Equation.Latex);

        Assert.Contains(
            Equation.Variables,
            Variable =>
                Variable.Symbol == "g_0" &&
                Variable.Unit == "m/s²");
    }

    [Fact]
    public void ChiSquareIsZeroForExactReferenceStates()
    {
        ConstantGravityAnalyticalModel Model =
            new(
                Vector3D.Zero,
                new Vector3D(
                    10.0,
                    0.0,
                    10.0),
                9.80665);

        SimulationState[] States =
        [
            Model.Evaluate(0.0),
            Model.Evaluate(0.5),
            Model.Evaluate(1.0)
        ];

        ChiSquareComparisonResult Result =
            Model.Compare(
                States,
                TrajectoryQuantity.Altitude,
                StandardUncertainty: 0.01);

        Assert.Equal(
            0.0,
            Result.ChiSquare,
            12);

        Assert.Equal(
            0.0,
            Result.RootMeanSquareError,
            12);
    }

    [Fact]
    public void ChiSquareDetectsDeviation()
    {
        ConstantGravityAnalyticalModel Model =
            new(
                Vector3D.Zero,
                new Vector3D(
                    10.0,
                    0.0,
                    10.0),
                9.80665);

        SimulationState ExactState =
            Model.Evaluate(
                1.0);

        SimulationState DeviatedState =
            ExactState with
            {
                Position =
                    new Vector3D(
                        ExactState.Position.X,
                        ExactState.Position.Y,
                        ExactState.Position.Z +
                        0.1)
            };

        ChiSquareComparisonResult Result =
            Model.Compare(
                new[]
                {
                    DeviatedState
                },
                TrajectoryQuantity.Altitude,
                StandardUncertainty: 0.01);

        Assert.Equal(
            100.0,
            Result.ChiSquare,
            10);

        Assert.Equal(
            0.1,
            Result.MaximumAbsoluteError,
            10);
    }
}
