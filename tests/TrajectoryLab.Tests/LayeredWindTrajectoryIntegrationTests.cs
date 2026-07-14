using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;
using TrajectoryLab.Core.Simulation;
using TrajectoryLab.Core.Solvers;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Tests.Wind;

public sealed class LayeredWindTrajectoryIntegrationTests
{
    [Fact]
    public void Simulate_WithLayeredNorthWind_ProducesNorthwardDeviation()
    {
        SimulationResult CalmResult = Simulate(
            new ConstantWindModel(Vector3D.Zero));

        WindLayer[] Layers =
        [
            new WindLayer(
                Altitude: 0.0,
                WindVelocity: Vector3D.Zero),

            new WindLayer(
                Altitude: 20.0,
                WindVelocity: new Vector3D(
                    0.0,
                    5.0,
                    0.0)),

            new WindLayer(
                Altitude: 40.0,
                WindVelocity: new Vector3D(
                    0.0,
                    15.0,
                    0.0))
        ];

        SimulationResult LayeredWindResult = Simulate(
            new LayeredWindModel(Layers));

        SimulationState CalmImpactState =
            CalmResult.States[^1];

        SimulationState LayeredWindImpactState =
            LayeredWindResult.States[^1];

        Assert.InRange(
            Math.Abs(CalmImpactState.Position.Y),
            0.0,
            1.0e-9);

        Assert.True(
            LayeredWindImpactState.Position.Y
                > CalmImpactState.Position.Y + 0.1,
            "Le vent par couches dirigé vers le Nord doit produire une déviation positive sur l'axe Y.");

        Assert.InRange(
            Math.Abs(LayeredWindImpactState.Position.Z),
            0.0,
            1.0e-9);
    }

    private static SimulationResult Simulate(IWindModel WindModel)
    {
        ProjectileParameters Projectile = new(
            Mass: 1.0,
            DragCoefficient: 0.47,
            CrossSectionalArea: 0.01);

        EnvironmentParameters Environment = new(
            GravityAcceleration: 9.80665,
            AtmosphereModel: new StandardAtmosphere1976Model(),
            WindModel: WindModel);

        SimulationSettings Settings = new(
            GroundAltitude: 0.0,
            TimeStep: 0.01,
            MaximumSimulationTime: 30.0);

        double HorizontalVelocity =
            50.0 / Math.Sqrt(2.0);

        SimulationParameters Parameters = new(
            InitialPosition: Vector3D.Zero,
            InitialVelocity: new Vector3D(
                HorizontalVelocity,
                0.0,
                HorizontalVelocity),
            Projectile: Projectile,
            Environment: Environment,
            Settings: Settings);

        IAccelerationModel AccelerationModel =
            new CompositeAccelerationModel(
                new ConstantGravityModel(),
                new DragAccelerationModel());

        TrajectorySimulator Simulator = new();

        return Simulator.Simulate(
            Parameters,
            new RungeKutta4Solver(),
            AccelerationModel);
    }
}