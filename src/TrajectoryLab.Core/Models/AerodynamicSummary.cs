namespace TrajectoryLab.Core;

// Résume les principales grandeurs aérodynamiques d'une trajectoire.
public sealed class AerodynamicSummary
{
    public double InitialMachNumber { get; }

    public double MaximumMachNumber { get; }

    public double ImpactMachNumber { get; }

    public double InitialRelativeSpeed { get; }

    public double MaximumRelativeSpeed { get; }

    public double ImpactRelativeSpeed { get; }

    public double MaximumMachTime { get; }

    public double MaximumMachAltitude { get; }

    internal AerodynamicSummary(
        double InitialMachNumber,
        double MaximumMachNumber,
        double ImpactMachNumber,
        double InitialRelativeSpeed,
        double MaximumRelativeSpeed,
        double ImpactRelativeSpeed,
        double MaximumMachTime,
        double MaximumMachAltitude)
    {
        this.InitialMachNumber = InitialMachNumber;
        this.MaximumMachNumber = MaximumMachNumber;
        this.ImpactMachNumber = ImpactMachNumber;
        this.InitialRelativeSpeed = InitialRelativeSpeed;
        this.MaximumRelativeSpeed = MaximumRelativeSpeed;
        this.ImpactRelativeSpeed = ImpactRelativeSpeed;
        this.MaximumMachTime = MaximumMachTime;
        this.MaximumMachAltitude = MaximumMachAltitude;
    }
}