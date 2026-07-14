using System.Collections.ObjectModel;
using TrajectoryLab.Core;
using TrajectoryLab.Core.Mathematics;

namespace TrajectoryLab.Core.Wind;

public sealed class LayeredWindModel : IWindModel
{
    private readonly ReadOnlyCollection<WindLayer> _Layers;

    public IReadOnlyList<WindLayer> Layers => _Layers;

    public LayeredWindModel(IEnumerable<WindLayer> Layers)
    {
        if (Layers is null)
        {
            throw new ArgumentNullException(
                nameof(Layers),
                "La collection de couches de vent ne peut pas être nulle.");
        }

        WindLayer[] LayerArray = Layers.ToArray();

        if (LayerArray.Length < 2)
        {
            throw new ArgumentException(
                "Le modèle de vent par couches doit contenir au moins deux couches.",
                nameof(Layers));
        }

        ValidateLayers(LayerArray);

        _Layers = Array.AsReadOnly(LayerArray);
    }

    public Vector3D GetWindVelocity(
        Vector3D Position,
        double Time)
    {
        ValidateVector(Position, nameof(Position));
        ValidateTime(Time);

        WindLayer FirstLayer = _Layers[0];
        WindLayer LastLayer = _Layers[^1];

        if (Position.Z <= FirstLayer.Altitude)
        {
            return FirstLayer.WindVelocity;
        }

        if (Position.Z >= LastLayer.Altitude)
        {
            return LastLayer.WindVelocity;
        }

        for (int Index = 1; Index < _Layers.Count; Index++)
        {
            WindLayer UpperLayer = _Layers[Index];

            if (Position.Z <= UpperLayer.Altitude)
            {
                WindLayer LowerLayer = _Layers[Index - 1];

                return Interpolate(
                    Position.Z,
                    LowerLayer,
                    UpperLayer);
            }
        }

        throw new InvalidOperationException(
            "Aucun intervalle de couches n'a été trouvé pour l'altitude demandée.");
    }

    private static Vector3D Interpolate(
        double Altitude,
        WindLayer LowerLayer,
        WindLayer UpperLayer)
    {
        double InterpolationFactor =
            (Altitude - LowerLayer.Altitude)
            / (UpperLayer.Altitude - LowerLayer.Altitude);

        return LowerLayer.WindVelocity
            + (UpperLayer.WindVelocity - LowerLayer.WindVelocity)
            * InterpolationFactor;
    }

    private static void ValidateLayers(WindLayer[] Layers)
    {
        for (int Index = 0; Index < Layers.Length; Index++)
        {
            WindLayer? Layer = Layers[Index];

            if (Layer is null)
            {
                throw new ArgumentException(
                    "La collection ne peut pas contenir de couche nulle.",
                    nameof(Layers));
            }

            if (Index == 0)
            {
                continue;
            }

            WindLayer PreviousLayer = Layers[Index - 1];

            if (Layer.Altitude <= PreviousLayer.Altitude)
            {
                throw new ArgumentException(
                    "Les altitudes des couches doivent être strictement croissantes.",
                    nameof(Layers));
            }
        }
    }

    private static void ValidateVector(
        Vector3D Vector,
        string ParameterName)
    {
        if (!double.IsFinite(Vector.X)
            || !double.IsFinite(Vector.Y)
            || !double.IsFinite(Vector.Z))
        {
            throw new ArgumentException(
                "Le vecteur doit contenir uniquement des composantes finies.",
                ParameterName);
        }
    }

    private static void ValidateTime(double Time)
    {
        if (!double.IsFinite(Time) || Time < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Time),
                Time,
                "Le temps doit être un nombre fini supérieur ou égal à zéro.");
        }
    }
}