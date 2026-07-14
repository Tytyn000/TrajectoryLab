using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Simulation;
using TrajectoryLab.Core.Solvers;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Tests.Physics;

public sealed class MagnusTrajectoryIntegrationTests
{
    private const double Tolerance =
        1.0e-10;

    [Fact]
    public void ZeroSpin_MatchesTrajectoryWithoutMagnus()
    {
        SimulationParameters Parameters =
            CreateParameters();

        CompositeAccelerationModel
            BaselineAccelerationModel =
                new(
                    new ConstantGravityModel());

        CompositeAccelerationModel
            ZeroSpinAccelerationModel =
                new(
                    new ConstantGravityModel(),
                    new MagnusAccelerationModel(
                        AngularVelocity:
                            Vector3D.Zero,
                        ProjectileRadius:
                            0.05,
                        MagnusCoefficient:
                            0.5));

        SimulationResult BaselineResult =
            Simulate(
                Parameters,
                BaselineAccelerationModel);

        SimulationResult MagnusResult =
            Simulate(
                Parameters,
                ZeroSpinAccelerationModel);

        Assert.Equal(
            BaselineResult.States.Count,
            MagnusResult.States.Count);

        AssertClose(
            BaselineResult.FlightTime,
            MagnusResult.FlightTime);

        AssertClose(
            BaselineResult.Range,
            MagnusResult.Range);

        AssertClose(
            BaselineResult.MaximumAltitude,
            MagnusResult.MaximumAltitude);

        AssertClose(
            BaselineResult.ImpactSpeed,
            MagnusResult.ImpactSpeed);

        for (int Index = 0;
             Index < BaselineResult.States.Count;
             Index++)
        {
            SimulationState BaselineState =
                BaselineResult.States[Index];

            SimulationState MagnusState =
                MagnusResult.States[Index];

            AssertClose(
                BaselineState.Time,
                MagnusState.Time);

            AssertVectorClose(
                BaselineState.Position,
                MagnusState.Position);

            AssertVectorClose(
                BaselineState.Velocity,
                MagnusState.Velocity);
        }
    }

    [Fact]
    public void PositiveVerticalSpin_DeflectsEastwardTrajectoryNorth()
    {
        SimulationResult Result =
            SimulateWithSpin(
                AngularVelocity:
                    new Vector3D(
                        0.0,
                        0.0,
                        50.0));

        Assert.True(
            Result.ImpactState.Position.Y
            > 0.0,
            "Une rotation autour de +Z doit dévier une trajectoire vers l'Est en direction du Nord.");
    }

    [Fact]
    public void NegativeVerticalSpin_DeflectsEastwardTrajectorySouth()
    {
        SimulationResult Result =
            SimulateWithSpin(
                AngularVelocity:
                    new Vector3D(
                        0.0,
                        0.0,
                        -50.0));

        Assert.True(
            Result.ImpactState.Position.Y
            < 0.0,
            "Une rotation autour de -Z doit dévier une trajectoire vers l'Est en direction du Sud.");
    }

    [Fact]
    public void MagnusWithoutDrag_PreservesImpactSpeedApproximately()
    {
        const double InitialSpeed =
            100.0;

        SimulationParameters Parameters =
            CreateParameters(
                InitialSpeed:
                    InitialSpeed);

        CompositeAccelerationModel
            AccelerationModel =
                new(
                    new ConstantGravityModel(),
                    new MagnusAccelerationModel(
                        AngularVelocity:
                            new Vector3D(
                                0.0,
                                0.0,
                                50.0),
                        ProjectileRadius:
                            0.05,
                        MagnusCoefficient:
                            0.5));

        SimulationResult Result =
            Simulate(
                Parameters,
                AccelerationModel);

        Assert.InRange(
            Math.Abs(
                Result.ImpactSpeed
                - InitialSpeed),
            0.0,
            1.0e-3);
    }

    private static SimulationResult
        SimulateWithSpin(
            Vector3D AngularVelocity)
    {
        SimulationParameters Parameters =
            CreateParameters();

        CompositeAccelerationModel
            AccelerationModel =
                new(
                    new ConstantGravityModel(),
                    new MagnusAccelerationModel(
                        AngularVelocity:
                            AngularVelocity,
                        ProjectileRadius:
                            0.05,
                        MagnusCoefficient:
                            0.5));

        return Simulate(
            Parameters,
            AccelerationModel);
    }

    private static SimulationParameters
        CreateParameters(
            double InitialSpeed = 100.0)
    {
        return new SimulationParameters(
            InitialPosition:
                Vector3D.Zero,
            InitialVelocity:
                LaunchVelocity
                    .FromSpeedAndAngles(
                        Speed:
                            InitialSpeed,
                        ElevationDegrees:
                            30.0,
                        AzimuthDegrees:
                            0.0),
            Projectile:
                new ProjectileParameters(
                    Mass: 1.0,
                    DragCoefficient: 0.0,
                    CrossSectionalArea:
                        0.01),
            Environment:
                new EnvironmentParameters(
                    GravityAcceleration:
                        9.80665,
                    AtmosphereModel:
                        new ConstantAtmosphereModel(
                            AirDensity:
                                1.225),
                    WindModel:
                        new ConstantWindModel(
                            Vector3D.Zero)),
            Settings:
                new SimulationSettings(
                    TimeStep: 0.01,
                    MaximumSimulationTime:
                        30.0,
                    GroundAltitude:
                        0.0));
    }

    private static SimulationResult Simulate(
        SimulationParameters Parameters,
        IAccelerationModel AccelerationModel)
    {
        return new TrajectorySimulator()
            .Simulate(
                Parameters,
                new RungeKutta4Solver(),
                AccelerationModel);
    }

    private static void AssertVectorClose(
        Vector3D Expected,
        Vector3D Actual)
    {
        AssertClose(
            Expected.X,
            Actual.X);

        AssertClose(
            Expected.Y,
            Actual.Y);

        AssertClose(
            Expected.Z,
            Actual.Z);
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