using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Simulation;
using Xunit;

namespace TrajectoryLab.Tests;

public sealed class ContinuousTrajectoryTests
{
    [Fact]
    public void ConstructorRejectsNullStates()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ContinuousTrajectory(
                null!));
    }

    [Fact]
    public void ConstructorRejectsLessThanTwoStates()
    {
        Assert.Throws<ArgumentException>(
            () => new ContinuousTrajectory(
                new[]
                {
                    CreateState(
                        Time: 0.0,
                        PositionX: 0.0,
                        VelocityX: 1.0)
                }));
    }

    [Fact]
    public void ConstructorRejectsNonIncreasingTimes()
    {
        Assert.Throws<ArgumentException>(
            () => new ContinuousTrajectory(
                new[]
                {
                    CreateState(
                        Time: 0.0,
                        PositionX: 0.0,
                        VelocityX: 1.0),

                    CreateState(
                        Time: 0.0,
                        PositionX: 1.0,
                        VelocityX: 1.0)
                }));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void EvaluateRejectsTimeOutsideDomain(
        double Time)
    {
        ContinuousTrajectory Trajectory =
            CreateLinearTrajectory();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => Trajectory.Evaluate(
                Time));
    }

    [Fact]
    public void EvaluatePreservesEndpoints()
    {
        ContinuousTrajectory Trajectory =
            CreateLinearTrajectory();

        SimulationState Start =
            Trajectory.Evaluate(
                0.0);

        SimulationState End =
            Trajectory.Evaluate(
                1.0);

        Assert.Equal(
            0.0,
            Start.Position.X,
            12);

        Assert.Equal(
            1.0,
            End.Position.X,
            12);

        Assert.Equal(
            1.0,
            Start.Velocity.X,
            12);

        Assert.Equal(
            1.0,
            End.Velocity.X,
            12);
    }

    [Fact]
    public void EvaluateReproducesConstantAcceleration()
    {
        SimulationState Start =
            new(
                Time: 0.0,
                Position:
                    Vector3D.Zero,
                Velocity:
                    Vector3D.Zero);

        SimulationState End =
            new(
                Time: 1.0,
                Position:
                    new Vector3D(
                        1.0,
                        0.0,
                        0.0),
                Velocity:
                    new Vector3D(
                        2.0,
                        0.0,
                        0.0));

        ContinuousTrajectory Trajectory =
            new(
                new[]
                {
                    Start,
                    End
                });

        SimulationState Middle =
            Trajectory.Evaluate(
                0.5);

        Assert.Equal(
            0.25,
            Middle.Position.X,
            12);

        Assert.Equal(
            1.0,
            Middle.Velocity.X,
            12);
    }

    [Fact]
    public void VelocityIsContinuousAtInternalState()
    {
        ContinuousTrajectory Trajectory =
            new(
                new[]
                {
                    CreateState(
                        Time: 0.0,
                        PositionX: 0.0,
                        VelocityX: 1.0),

                    CreateState(
                        Time: 1.0,
                        PositionX: 1.0,
                        VelocityX: 1.0),

                    CreateState(
                        Time: 2.0,
                        PositionX: 2.0,
                        VelocityX: 1.0)
                });

        double LeftVelocity =
            Trajectory.EquationSegments[0]
                .Evaluate(1.0)
                .Velocity.X;

        double RightVelocity =
            Trajectory.EquationSegments[1]
                .Evaluate(1.0)
                .Velocity.X;

        Assert.Equal(
            LeftVelocity,
            RightVelocity,
            12);

        Assert.Equal(
            1.0,
            LeftVelocity,
            12);
    }

    [Fact]
    public void SampleIncludesEndpoints()
    {
        ContinuousTrajectory Trajectory =
            CreateLinearTrajectory();

        IReadOnlyList<SimulationState> Samples =
            Trajectory.Sample(
                5);

        Assert.Equal(
            5,
            Samples.Count);

        Assert.Equal(
            0.0,
            Samples[0].Time,
            12);

        Assert.Equal(
            1.0,
            Samples[^1].Time,
            12);
    }

    [Fact]
    public void EquationSegmentMatchesTrajectoryEvaluation()
    {
        ContinuousTrajectory Trajectory =
            CreateLinearTrajectory();

        SimulationState FromTrajectory =
            Trajectory.Evaluate(
                0.35);

        SimulationState FromEquation =
            Trajectory.EquationSegments[0]
                .Evaluate(
                    0.35);

        Assert.Equal(
            FromTrajectory.Position.X,
            FromEquation.Position.X,
            12);

        Assert.Equal(
            FromTrajectory.Velocity.X,
            FromEquation.Velocity.X,
            12);
    }

    private static ContinuousTrajectory
        CreateLinearTrajectory()
    {
        return new ContinuousTrajectory(
            new[]
            {
                CreateState(
                    Time: 0.0,
                    PositionX: 0.0,
                    VelocityX: 1.0),

                CreateState(
                    Time: 1.0,
                    PositionX: 1.0,
                    VelocityX: 1.0)
            });
    }

    private static SimulationState CreateState(
        double Time,
        double PositionX,
        double VelocityX)
    {
        return new SimulationState(
            Time,
            new Vector3D(
                PositionX,
                0.0,
                0.0),
            new Vector3D(
                VelocityX,
                0.0,
                0.0));
    }
}
