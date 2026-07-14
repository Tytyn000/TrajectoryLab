using System.Collections.ObjectModel;

namespace TrajectoryLab.Core.Physics;

// Calcule le coefficient de traînée par interpolation
// linéaire entre plusieurs points définis selon le Mach.
public sealed class TabulatedDragCoefficientModel
    : IDragCoefficientModel
{
    private readonly
        ReadOnlyCollection<DragCoefficientPoint>
        _Points;

    public IReadOnlyList<DragCoefficientPoint>
        Points => _Points;

    public TabulatedDragCoefficientModel(
        IEnumerable<DragCoefficientPoint> Points)
    {
        if (Points is null)
        {
            throw new ArgumentNullException(
                nameof(Points),
                "La collection de points ne peut pas être nulle.");
        }

        DragCoefficientPoint[] PointArray =
            Points.ToArray();

        if (PointArray.Length < 2)
        {
            throw new ArgumentException(
                "Le modèle tabulé doit contenir au moins deux points.",
                nameof(Points));
        }

        ValidatePoints(PointArray);

        _Points = Array.AsReadOnly(
            PointArray);
    }

    public double GetDragCoefficient(
        double MachNumber)
    {
        ValidateMachNumber(MachNumber);

        DragCoefficientPoint FirstPoint =
            _Points[0];

        DragCoefficientPoint LastPoint =
            _Points[^1];

        if (MachNumber <= FirstPoint.MachNumber)
        {
            return FirstPoint.DragCoefficient;
        }

        if (MachNumber >= LastPoint.MachNumber)
        {
            return LastPoint.DragCoefficient;
        }

        int UpperPointIndex =
            FindUpperPointIndex(
                MachNumber);

        DragCoefficientPoint LowerPoint =
            _Points[UpperPointIndex - 1];

        DragCoefficientPoint UpperPoint =
            _Points[UpperPointIndex];

        return Interpolate(
            MachNumber,
            LowerPoint,
            UpperPoint);
    }

    private int FindUpperPointIndex(
        double MachNumber)
    {
        int LowerIndex = 0;
        int UpperIndex = _Points.Count - 1;

        while (UpperIndex - LowerIndex > 1)
        {
            int MiddleIndex =
                LowerIndex
                + (UpperIndex - LowerIndex) / 2;

            if (MachNumber
                <= _Points[MiddleIndex].MachNumber)
            {
                UpperIndex = MiddleIndex;
            }
            else
            {
                LowerIndex = MiddleIndex;
            }
        }

        return UpperIndex;
    }

    private static double Interpolate(
        double MachNumber,
        DragCoefficientPoint LowerPoint,
        DragCoefficientPoint UpperPoint)
    {
        double MachInterval =
            UpperPoint.MachNumber
            - LowerPoint.MachNumber;

        double InterpolationFactor =
            (MachNumber - LowerPoint.MachNumber)
            / MachInterval;

        double DragCoefficient =
            LowerPoint.DragCoefficient
            + (
                UpperPoint.DragCoefficient
                - LowerPoint.DragCoefficient
            )
            * InterpolationFactor;

        if (!double.IsFinite(DragCoefficient)
            || DragCoefficient < 0.0)
        {
            throw new InvalidOperationException(
                "Le coefficient de traînée interpolé doit être fini et positif ou nul.");
        }

        return DragCoefficient;
    }

    private static void ValidatePoints(
        DragCoefficientPoint[] Points)
    {
        for (int Index = 0;
             Index < Points.Length;
             Index++)
        {
            DragCoefficientPoint? Point =
                Points[Index];

            if (Point is null)
            {
                throw new ArgumentException(
                    "La collection ne peut pas contenir de point nul.",
                    nameof(Points));
            }

            if (Index == 0)
            {
                continue;
            }

            DragCoefficientPoint PreviousPoint =
                Points[Index - 1];

            if (Point.MachNumber
                <= PreviousPoint.MachNumber)
            {
                throw new ArgumentException(
                    "Les nombres de Mach doivent être strictement croissants.",
                    nameof(Points));
            }
        }
    }

    private static void ValidateMachNumber(
        double MachNumber)
    {
        if (!double.IsFinite(MachNumber)
            || MachNumber < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MachNumber),
                MachNumber,
                "Le nombre de Mach doit être fini et positif ou nul.");
        }
    }
}