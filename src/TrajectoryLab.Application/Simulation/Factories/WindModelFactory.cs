using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Application.Simulation.Factories;

public sealed class WindModelFactory :
    IWindModelFactory
{
    public IWindModel Create(WindInput Input)
    {
        ArgumentNullException.ThrowIfNull(Input);

        return Input.ModelKind switch
        {
            WindModelKind.None =>
                new ConstantWindModel(Vector3D.Zero),

            WindModelKind.Constant =>
                new ConstantWindModel(
                    new Vector3D(
                        Input.ConstantWindX,
                        Input.ConstantWindY,
                        Input.ConstantWindZ)),

            WindModelKind.Linear =>
                new LinearWindModel(
                    Input.LowerAltitude,
                    new Vector3D(
                        Input.LowerWindX,
                        Input.LowerWindY,
                        Input.LowerWindZ),
                    Input.UpperAltitude,
                    new Vector3D(
                        Input.UpperWindX,
                        Input.UpperWindY,
                        Input.UpperWindZ)),

            WindModelKind.Layered =>
                new LayeredWindModel(
                    Input.Layers.Select(
                        Layer => new WindLayer(
                            Layer.Altitude,
                            new Vector3D(
                                Layer.WindX,
                                Layer.WindY,
                                Layer.WindZ)))),

            _ => throw new ArgumentOutOfRangeException(
                nameof(Input),
                Input.ModelKind,
                "Le modÃ¨le de vent sÃ©lectionnÃ© est inconnu.")
        };
    }
}