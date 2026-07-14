namespace TrajectoryLab.Core.Physics;

// Représente une valeur du coefficient de traînée
// associée à un nombre de Mach donné.
public sealed class DragCoefficientPoint
{
    public double MachNumber { get; }

    public double DragCoefficient { get; }

    public DragCoefficientPoint(
        double MachNumber,
        double DragCoefficient)
    {
        if (!double.IsFinite(MachNumber)
            || MachNumber < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MachNumber),
                MachNumber,
                "Le nombre de Mach doit être fini et positif ou nul.");
        }

        if (!double.IsFinite(DragCoefficient)
            || DragCoefficient < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(DragCoefficient),
                DragCoefficient,
                "Le coefficient de traînée doit être fini et positif ou nul.");
        }

        this.MachNumber = MachNumber;
        this.DragCoefficient = DragCoefficient;
    }
}