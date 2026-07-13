namespace TrajectoryLab.Core;

public sealed class SimulationSettings
{
    public double TimeStep { get; }

    public double MaximumSimulationTime { get; }

    public double GroundAltitude { get; }

    public SimulationSettings(
        double TimeStep,
        double MaximumSimulationTime,
        double GroundAltitude = 0.0)
    {
        // Un pas nul ou négatif empêcherait la progression de la simulation.
        if (!double.IsFinite(TimeStep) || TimeStep <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(TimeStep), "Le pas de temps doit être fini et strictement positif.");
        }

        if (!double.IsFinite(MaximumSimulationTime) || MaximumSimulationTime <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(MaximumSimulationTime), "La durée maximale doit être finie et strictement positive.");
        }

        if (TimeStep > MaximumSimulationTime)
        {
            throw new ArgumentOutOfRangeException(nameof(TimeStep), "Le pas de temps ne peut pas être supérieur à la durée maximale.");
        }

        if (!double.IsFinite(GroundAltitude))
        {
            throw new ArgumentOutOfRangeException(nameof(GroundAltitude), "L'altitude du sol doit être finie.");
        }

        this.TimeStep = TimeStep;
        this.MaximumSimulationTime = MaximumSimulationTime;
        this.GroundAltitude = GroundAltitude;
    }
}