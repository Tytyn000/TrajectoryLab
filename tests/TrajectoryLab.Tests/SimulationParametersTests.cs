using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using Xunit;

namespace TrajectoryLab.Tests;

public sealed class SimulationParametersTests
{
    [Fact]
    public void SimulationParametersStoreGroupedParameters()
    {
        ProjectileParameters Projectile = new(
            Mass: 1.0,
            DragCoefficient: 0.47,
            CrossSectionalArea: 0.01
        );

        EnvironmentParameters Environment = new(
            GravityAcceleration: 9.80665,
            AtmosphereModel: new ConstantAtmosphereModel(
                AirDensity: 1.225
            ),
            WindVelocity: Vector3D.Zero
        );

        SimulationSettings Settings = new(
            TimeStep: 0.01,
            MaximumSimulationTime: 60.0
        );

        SimulationParameters Parameters = new(
            InitialPosition: Vector3D.Zero,
            InitialVelocity: new Vector3D(
                X: 10.0,
                Y: 0.0,
                Z: 10.0
            ),
            Projectile: Projectile,
            Environment: Environment,
            Settings: Settings
        );

        Assert.Same(Projectile, Parameters.Projectile);
        Assert.Same(Environment, Parameters.Environment);
        Assert.Same(Settings, Parameters.Settings);
    }

    [Fact]
    public void SimulationParametersRejectInvalidInitialPosition()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SimulationParameters(
                InitialPosition: new Vector3D(
                    X: double.NaN,
                    Y: 0.0,
                    Z: 0.0
                ),
                InitialVelocity: Vector3D.Zero,
                Projectile: CreateProjectile(),
                Environment: CreateEnvironment(),
                Settings: CreateSettings()
            )
        );
    }

    [Fact]
    public void SimulationParametersRejectInvalidInitialVelocity()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SimulationParameters(
                InitialPosition: Vector3D.Zero,
                InitialVelocity: new Vector3D(
                    X: double.PositiveInfinity,
                    Y: 0.0,
                    Z: 0.0
                ),
                Projectile: CreateProjectile(),
                Environment: CreateEnvironment(),
                Settings: CreateSettings()
            )
        );
    }

    private static ProjectileParameters CreateProjectile()
    {
        return new ProjectileParameters(
            Mass: 1.0,
            DragCoefficient: 0.47,
            CrossSectionalArea: 0.01
        );
    }

    private static EnvironmentParameters CreateEnvironment()
    {
        return new EnvironmentParameters(
            GravityAcceleration: 9.80665,
            AtmosphereModel: new ConstantAtmosphereModel(
                AirDensity: 1.225
            ),
            WindVelocity: Vector3D.Zero
        );
    }

    private static SimulationSettings CreateSettings()
    {
        return new SimulationSettings(
            TimeStep: 0.01,
            MaximumSimulationTime: 60.0
        );
    }
}