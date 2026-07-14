using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Core.Physics;

namespace TrajectoryLab.Application.Simulation.Factories;

public sealed class GravityModelFactory :
    IGravityModelFactory
{
    public IAccelerationModel Create(
        CelestialBodyInput Input)
    {
        ArgumentNullException.ThrowIfNull(Input);

        return Input.ModelKind switch
        {
            GravityModelKind.UniformSphere =>
                CreateUniformSphere(Input),

            GravityModelKind.Constant =>
                new ConstantGravityModel(),

            _ => throw new ArgumentOutOfRangeException(
                nameof(Input),
                Input.ModelKind,
                "Le modÃ¨le de gravitÃ© sÃ©lectionnÃ© est inconnu.")
        };
    }

    public double GetSurfaceGravityAcceleration(
        CelestialBodyInput Input)
    {
        ArgumentNullException.ThrowIfNull(Input);

        return Input.ModelKind switch
        {
            GravityModelKind.Constant =>
                Input.SurfaceGravityAcceleration,

            GravityModelKind.UniformSphere =>
                CreateUniformSphere(Input)
                    .SurfaceGravityAcceleration,

            _ => throw new ArgumentOutOfRangeException(
                nameof(Input),
                Input.ModelKind,
                "Le modÃ¨le de gravitÃ© sÃ©lectionnÃ© est inconnu.")
        };
    }

    private static UniformSphereGravityModel
        CreateUniformSphere(
            CelestialBodyInput Input)
    {
        return Input.UniformSphereDefinition switch
        {
            UniformSphereDefinitionKind.SurfaceGravity =>
                UniformSphereGravityModel.FromSurfaceGravity(
                    Input.BodyRadius,
                    Input.SurfaceGravityAcceleration),

            UniformSphereDefinitionKind.Density =>
                new UniformSphereGravityModel(
                    Input.BodyRadius,
                    Input.BodyDensity),

            UniformSphereDefinitionKind.Mass =>
                UniformSphereGravityModel.FromMass(
                    Input.BodyRadius,
                    Input.BodyMass),

            _ => throw new ArgumentOutOfRangeException(
                nameof(Input),
                Input.UniformSphereDefinition,
                "La dÃ©finition de la sphÃ¨re uniforme est inconnue.")
        };
    }
}
