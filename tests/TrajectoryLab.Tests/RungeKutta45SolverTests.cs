using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Solvers;
using TrajectoryLab.Core.Wind;
using Xunit;

namespace TrajectoryLab.Tests;

public sealed class RungeKutta45SolverTests
{
    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void ConstructorRejectsInvalidAbsoluteTolerance(
        double AbsoluteTolerance)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new RungeKutta45Solver(
                AbsoluteTolerance:
                    AbsoluteTolerance));
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void ConstructorRejectsInvalidRelativeTolerance(
        double RelativeTolerance)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new RungeKutta45Solver(
                RelativeTolerance:
                    RelativeTolerance));
    }

    [Theory]
    [InlineData(0.0, 1.0)]
    [InlineData(-1.0, 1.0)]
    [InlineData(1.0, 0.0)]
    [InlineData(1.0, -1.0)]
    [InlineData(2.0, 1.0)]
    public void ConstructorRejectsInvalidTimeStepBounds(
        double MinimumTimeStep,
        double MaximumTimeStep)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new RungeKutta45Solver(
                MinimumTimeStep:
                    MinimumTimeStep,
                MaximumTimeStep:
                    MaximumTimeStep));
    }

    [Fact]
    public void StepAdaptiveIntegratesConstantAcceleration()
    {
        RungeKutta45Solver Solver = new(
            AbsoluteTolerance: 1.0e-10,
            RelativeTolerance: 1.0e-10,
            MinimumTimeStep: 1.0e-8,
            MaximumTimeStep: 1.0);

        SimulationState InitialState = new(
            Time: 0.0,
            Position: Vector3D.Zero,
            Velocity:
                new Vector3D(
                    1.0,
                    0.0,
                    0.0));

        ConstantTestAccelerationModel AccelerationModel =
            new(
                new Vector3D(
                    0.0,
                    0.0,
                    -9.80665));

        AdaptiveStepResult Result =
            Solver.StepAdaptive(
                InitialState,
                TimeStep: 0.1,
                AccelerationModel,
                CreateParameters());

        Assert.True(Result.IsAccepted);

        Assert.Equal(0.1, Result.State.Time, 12);
        Assert.Equal(0.1, Result.State.Position.X, 12);
        Assert.Equal(-0.04903325, Result.State.Position.Z, 10);
        Assert.Equal(1.0, Result.State.Velocity.X, 12);
        Assert.Equal(-0.980665, Result.State.Velocity.Z, 10);
    }

    [Fact]
    public void StepAdaptiveRejectsInaccurateLargeStep()
    {
        RungeKutta45Solver Solver = new(
            AbsoluteTolerance: 1.0e-12,
            RelativeTolerance: 1.0e-12,
            MinimumTimeStep: 1.0e-8,
            MaximumTimeStep: 1.0);

        SimulationState InitialState = new(
            Time: 0.0,
            Position:
                new Vector3D(
                    1.0,
                    0.0,
                    0.0),
            Velocity:
                Vector3D.Zero);

        AdaptiveStepResult Result =
            Solver.StepAdaptive(
                InitialState,
                TimeStep: 1.0,
                new HarmonicAccelerationModel(),
                CreateParameters());

        Assert.False(Result.IsAccepted);
        Assert.True(Result.ErrorNorm > 1.0);

        Assert.True(
            Result.SuggestedTimeStep <
            Result.UsedTimeStep);
    }

    [Fact]
    public void StepReducesTimeStepUntilAccepted()
    {
        RungeKutta45Solver Solver = new(
            AbsoluteTolerance: 1.0e-10,
            RelativeTolerance: 1.0e-10,
            MinimumTimeStep: 1.0e-8,
            MaximumTimeStep: 1.0);

        SimulationState InitialState = new(
            Time: 0.0,
            Position:
                new Vector3D(
                    1.0,
                    0.0,
                    0.0),
            Velocity:
                Vector3D.Zero);

        SimulationState Result = Solver.Step(
            InitialState,
            TimeStep: 1.0,
            new HarmonicAccelerationModel(),
            CreateParameters());

        Assert.True(Result.Time > 0.0);
        Assert.True(Result.Time < 1.0);
    }

    private static SimulationParameters
        CreateParameters()
    {
        return new SimulationParameters(
            InitialPosition:
                Vector3D.Zero,
            InitialVelocity:
                Vector3D.Zero,
            Projectile:
                new ProjectileParameters(
                    Mass: 1.0,
                    DragCoefficient: 0.0,
                    CrossSectionalArea: 0.0),
            Environment:
                new EnvironmentParameters(
                    GravityAcceleration: 9.80665,
                    AtmosphereModel:
                        new ConstantAtmosphereModel(
                            AirDensity: 0.0),
                    WindModel:
                        new ConstantWindModel(
                            Vector3D.Zero)),
            Settings:
                new SimulationSettings(
                    TimeStep: 0.01,
                    MaximumSimulationTime: 10.0));
    }

    private sealed class ConstantTestAccelerationModel :
        IAccelerationModel
    {
        private readonly Vector3D Acceleration;

        public ConstantTestAccelerationModel(
            Vector3D Acceleration)
        {
            this.Acceleration = Acceleration;
        }

        public Vector3D GetAcceleration(
            SimulationState State,
            SimulationParameters Parameters)
        {
            return Acceleration;
        }
    }

    private sealed class HarmonicAccelerationModel :
        IAccelerationModel
    {
        public Vector3D GetAcceleration(
            SimulationState State,
            SimulationParameters Parameters)
        {
            return new Vector3D(
                -State.Position.X,
                0.0,
                0.0);
        }
    }
}
