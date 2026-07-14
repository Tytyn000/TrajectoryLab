namespace TrajectoryLab.Core;

public sealed class IdealGasModel
{
    public const double DryAirSpecificHeatRatio = 1.4;

    public const double DryAirSpecificGasConstant = 287.05287;

    public double SpecificHeatRatio { get; }

    public double SpecificGasConstant { get; }

    public IdealGasModel()
        : this(
            DryAirSpecificHeatRatio,
            DryAirSpecificGasConstant)
    {
    }

    public IdealGasModel(
        double SpecificHeatRatio,
        double SpecificGasConstant)
    {
        ValidateSpecificHeatRatio(SpecificHeatRatio);
        ValidateSpecificGasConstant(SpecificGasConstant);

        this.SpecificHeatRatio = SpecificHeatRatio;
        this.SpecificGasConstant = SpecificGasConstant;
    }

    public double GetSpeedOfSound(double Temperature)
    {
        ValidateTemperature(Temperature);

        return Math.Sqrt(
            SpecificHeatRatio
            * SpecificGasConstant
            * Temperature);
    }

    private static void ValidateSpecificHeatRatio(
        double SpecificHeatRatio)
    {
        if (!double.IsFinite(SpecificHeatRatio)
            || SpecificHeatRatio <= 1.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(SpecificHeatRatio),
                SpecificHeatRatio,
                "Le rapport des capacités thermiques doit être un nombre fini strictement supérieur à un.");
        }
    }

    private static void ValidateSpecificGasConstant(
        double SpecificGasConstant)
    {
        if (!double.IsFinite(SpecificGasConstant)
            || SpecificGasConstant <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(SpecificGasConstant),
                SpecificGasConstant,
                "La constante spécifique du gaz doit être un nombre fini strictement positif.");
        }
    }

    private static void ValidateTemperature(double Temperature)
    {
        if (!double.IsFinite(Temperature)
            || Temperature <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Temperature),
                Temperature,
                "La température absolue doit être un nombre fini strictement positif.");
        }
    }
}