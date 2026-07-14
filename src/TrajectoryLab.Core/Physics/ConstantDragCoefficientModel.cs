namespace TrajectoryLab.Core.Physics;

// Fournit un coefficient de traînée constant,
// indépendamment du nombre de Mach.
public sealed class ConstantDragCoefficientModel
    : IDragCoefficientModel
{
    public double DragCoefficient { get; }

    public ConstantDragCoefficientModel(
        double DragCoefficient)
    {
        if (!double.IsFinite(DragCoefficient)
            || DragCoefficient < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(DragCoefficient),
                DragCoefficient,
                "Le coefficient de traînée doit être fini et positif ou nul.");
        }

        this.DragCoefficient =
            DragCoefficient;
    }

    public double GetDragCoefficient(
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

        return DragCoefficient;
    }
}