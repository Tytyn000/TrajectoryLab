using TrajectoryLab.Core;
using TrajectoryLab.Application.Models;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Physics;

namespace TrajectoryLab.Application.Simulation.Factories;

public sealed class AccelerationModelFactory :
    IAccelerationModelFactory
{
    private readonly IGravityModelFactory GravityModelFactory;
    private readonly IDragCoefficientModelFactory DragCoefficientModelFactory;

    public AccelerationModelFactory(
        IGravityModelFactory GravityModelFactory,
        IDragCoefficientModelFactory DragCoefficientModelFactory)
    {
        ArgumentNullException.ThrowIfNull(GravityModelFactory);
        ArgumentNullException.ThrowIfNull(DragCoefficientModelFactory);

        this.GravityModelFactory = GravityModelFactory;
        this.DragCoefficientModelFactory = DragCoefficientModelFactory;
    }

    public IAccelerationModel Create(
        SimulationInput Input,
        IdealGasModel GasModel)
    {
        ArgumentNullException.ThrowIfNull(Input);
        ArgumentNullException.ThrowIfNull(GasModel);

        List<IAccelerationModel> Models =
        [
            GravityModelFactory.Create(Input.CelestialBody)
        ];

        if (Input.Drag.IsEnabled)
        {
            IDragCoefficientModel DragCoefficientModel =
                DragCoefficientModelFactory.Create(
                    Input.Projectile,
                    Input.Drag);

            Models.Add(
                new DragAccelerationModel(
                    DragCoefficientModel,
                    GasModel));
        }

        if (Input.Rotation.IsCoriolisEnabled)
        {
            Models.Add(
                CreateCoriolisModel(
                    Input.Rotation));
        }

        if (Input.Magnus.IsEnabled)
        {
            Models.Add(
                new MagnusAccelerationModel(
                    new Vector3D(
                        Input.Magnus.AngularVelocityX,
                        Input.Magnus.AngularVelocityY,
                        Input.Magnus.AngularVelocityZ),
                    Input.Projectile.Radius,
                    Input.Magnus.MagnusCoefficient));
        }

        return Models.Count == 1
            ? Models[0]
            : new CompositeAccelerationModel(Models.ToArray());
    }

    private static CoriolisAccelerationModel
        CreateCoriolisModel(
            TrajectoryLab.Application.Models.Inputs.RotationInput Input)
    {
        return Input.DefinitionKind switch
        {
            TrajectoryLab.Application.Models.Inputs.CoriolisDefinitionKind.Latitude =>
                new CoriolisAccelerationModel(
                    Input.LatitudeDegrees,
                    Input.AngularVelocity),

            TrajectoryLab.Application.Models.Inputs.CoriolisDefinitionKind.AngularVelocityVector =>
                new CoriolisAccelerationModel(
                    new Vector3D(
                        Input.AngularVelocityX,
                        Input.AngularVelocityY,
                        Input.AngularVelocityZ)),

            _ => throw new ArgumentOutOfRangeException(
                nameof(Input),
                Input.DefinitionKind,
                "La dÃ©finition de la rotation de l'astre est inconnue.")
        };
    }

}